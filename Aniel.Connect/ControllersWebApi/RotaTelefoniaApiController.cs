using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Integracoes.Saperx.Services;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class RotaTelefoniaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/RotaTelefoniaApi/GetAll")]
        public async Task<DataTableResponse<RotaTelefoniaListaModel>> GetAll(RotaTelefonia_DataTableRequest request)
        {
            List<RotaTelefoniaListaModel> retorno = new List<RotaTelefoniaListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_ROTA_TELEFONIA A");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }


                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT A.ID,A.NOME,A.STATUS
                                                            FROM TB_ROTA_TELEFONIA A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {

                        sql.AppendLine(" WHERE (UPPER(A.NOME) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new RotaTelefoniaListaModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }
                }

            }



            return new DataTableResponse<RotaTelefoniaListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno
            };

        }

        [HttpGet()]
        [Route("api/RotaTelefoniaApi/FindById")]
        public async Task<GetApiRetorno<RotaTelefoniaCadastroModel>> FindById(Int64 Id)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<RotaTelefonia>(session))
            {
                try
                {
                    RotaTelefonia rotaTelefoniaModel = await repository.selectFirst(p => p.Id == Id);
                    RotaTelefoniaCadastroModel retorno = GetViewObj(rotaTelefoniaModel);
                    return new GetApiRetorno<RotaTelefoniaCadastroModel>
                    {
                        Ok = true,
                        Mensagem = "",
                        InnerException = "",
                        Objeto = retorno
                    };
                }
                catch (Exception ex)
                {
                    return new GetApiRetorno<RotaTelefoniaCadastroModel>
                    {
                        Ok = false,
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty
                    };
                }
            }
        }

        [HttpPost()]
        [Route("api/RotaTelefoniaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<RotaTelefoniaCadastroModel> value)
        {
            try
            {
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryLogs = new Conexao<LogRotaTelefonia>(sessionLog))
                using (var session = HibernateUtil.GetSession())
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                using (var repository = new Conexao<RotaTelefonia>(session))
                {

                    RotaTelefonia rotaTelefonia = null;
                    RotaTelefonia modeloAnterior = null;
                    if (value.Objeto.Id == 0)
                    {
                        rotaTelefonia = new RotaTelefonia();
                        rotaTelefonia.UsuarioCriacao = value.Usuario;
                        rotaTelefonia.DataHoraCriacao = DateTime.Now;
                        rotaTelefonia.Status = Status_Default.Ativo;
                    }
                    else
                    {
                        rotaTelefonia = await repository.selectFirst(p => p.Id == value.Objeto.Id);
                        rotaTelefonia.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        rotaTelefonia.UsuarioAlteracao = value.Usuario;
                        rotaTelefonia.DataHoraAlteracao = DateTime.Now;
                        modeloAnterior = (RotaTelefonia)rotaTelefonia.Clone();
                    }

                    BindViewObj(value.Objeto, rotaTelefonia);
                    await repository.SaveOrUpdate(rotaTelefonia);

                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        if (value.Objeto.Id == 0)
                        {
                            var response = await Integracao_Cadastro(rotaTelefonia);
                            rotaTelefonia.CodigoAuxiliar = response?.id;
                            await repository.Update(rotaTelefonia);
                        }
                        else if(String.IsNullOrEmpty(rotaTelefonia.CodigoAuxiliar) == false)
                        {
                            await Integracao_Editar(rotaTelefonia);
                        }
                    }

                    if (modeloAnterior != null)
                    {

                        var logs = rotaTelefonia.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);

                    }
                    else
                    {

                        var logs = rotaTelefonia.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                        await repositoryLogs.Save(logs);

                    }

                    return new PostApiRetorno<long>
                    {
                        IdObjeto = rotaTelefonia.Id,
                        Ok = true,
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno<long>
                {
                    Mensagem = ex.Message,
                    Ok = false
                };
            }
        }

        private RotaTelefoniaCadastroModel GetViewObj(RotaTelefonia rotaTelefonia)
        {
            return new RotaTelefoniaCadastroModel
            {
                Id = rotaTelefonia.Id,
                Nome = rotaTelefonia.Nome,
                Status = rotaTelefonia.Status == Status_Default.Ativo
            };
        }

        private RotaTelefonia BindViewObj(RotaTelefoniaCadastroModel viewModel, RotaTelefonia model)
        {
            model.Nome = viewModel.Nome;
            return model;
        }

        [HttpPost()]
        [Route("api/RotaTelefoniaApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ROTA_TELEFONIA SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/RotaTelefoniaApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ROTA_TELEFONIA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        public async Task<RotaIntegracaoResponseResult> Integracao_Cadastro(RotaTelefonia obj)
        {
            RotaIntegracaoSaperxService integracao = new RotaIntegracaoSaperxService();
            return integracao.Cadastrar(obj);

        }

        public async Task<RotaIntegracaoResponseResult> Integracao_Editar(RotaTelefonia obj)
        {
            RotaIntegracaoSaperxService integracao = new RotaIntegracaoSaperxService();
            return integracao.Atualizar(obj);

        }
    }
}