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
    public class FranquiaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/FranquiaApi/GetAll")]
        public async Task<DataTableResponse<FranquiaListaModel>> GetAll(Franquia_DataTableRequest request)
        {
            List<FranquiaListaModel> retorno = new List<FranquiaListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_FRANQUIA A");

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
                    StringBuilder sql = new StringBuilder(@"SELECT A.ID,A.DESCRICAO,A.STATUS,A.VC1,
                                                            A.VC2,A.VC3,A.LDN,A.LC,A.MINUTOS 
                                                            FROM TB_FRANQUIA A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {

                        sql.AppendLine(" WHERE (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FranquiaListaModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Minutos = Convert.ToString(reader["MINUTOS"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Ldn = Convert.ToString(reader["LDN"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                Lc = Convert.ToString(reader["LC"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                Vc1 = Convert.ToString(reader["VC1"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                Vc2 = Convert.ToString(reader["VC2"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                                Vc3 = Convert.ToString(reader["VC3"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                            });
                        }
                    }
                }

            }



            return new DataTableResponse<FranquiaListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno
            };

        }

        [HttpGet()]
        [Route("api/FranquiaApi/FindById")]
        public async Task<GetApiRetorno<FranquiaCadastroModel>> FindById(Int64 Id)
        {
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Franquia>(session))
            {
                try
                {
                    Franquia franquiaModel = await repository.selectFirst(p => p.Id == Id);
                    FranquiaCadastroModel retorno = GetViewObj(franquiaModel);
                    return new GetApiRetorno<FranquiaCadastroModel>
                    {
                        Ok = true,
                        Mensagem = "",
                        InnerException = "",
                        Objeto = retorno
                    };
                }
                catch (Exception ex)
                {
                    return new GetApiRetorno<FranquiaCadastroModel>
                    {
                        Ok = false,
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty
                    };
                }
            }
        }

        [HttpPost()]
        [Route("api/FranquiaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<FranquiaCadastroModel> value)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<Franquia>(session))
                using (var repositoryPlano = new Conexao<Plano>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                {

                    Franquia franquia = null;
                    Franquia modeloAnterior = null;
                    if (value.Objeto.Id == 0)
                    {
                        franquia = new Franquia();
                        franquia.UsuarioCriacao = value.Usuario;
                        franquia.DataHoraCriacao = DateTime.Now;
                        franquia.Status = Status_Default.Ativo;
                    }
                    else
                    {
                        franquia = await repository.selectFirst(p => p.Id == value.Objeto.Id);
                        franquia.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        franquia.UsuarioAlteracao = value.Usuario;
                        franquia.DataHoraAlteracao = DateTime.Now;
                        modeloAnterior = (Franquia)franquia.Clone();
                    }

                    BindViewObj(value.Objeto, franquia);
                    await repository.SaveOrUpdate(franquia);

                    if (modeloAnterior != null)
                    {
                        using (var repositoryLogs = new Conexao<LogFranquia>(sessionLog))
                        {
                            var logs = franquia.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                    }
                    else
                    {
                        using (var repositoryLogs = new Conexao<LogFranquia>(sessionLog))
                        {
                            var logs = franquia.GeraLog(modeloAnterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                            await repositoryLogs.Save(logs);
                        }
                    }

                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        if (value.Objeto.Id != 0)
                        {
                            var planos = await repositoryPlano.select(p => p.Franquia == franquia && p.CodigoAuxiliarFranquia != null);

                            foreach (var item in planos)
                            {
                                if (String.IsNullOrEmpty(item.CodigoAuxiliarFranquia) == false)
                                    await Integracao_Editar_Franquia(franquia, item);
                            }

                        }
                    }

                    return new PostApiRetorno<long>
                    {
                        IdObjeto = franquia.Id,
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

        private FranquiaCadastroModel GetViewObj(Franquia franquia)
        {
            return new FranquiaCadastroModel
            {
                Id = franquia.Id,
                Descricao = franquia.Descricao,
                Ldn = franquia.Ldn,
                Lc = franquia.Lc,
                Minutos = franquia.Minutos.HasValue ? franquia.Minutos.ToString() : String.Empty,
                Vc1 = franquia.Vc1,
                Vc2 = franquia.Vc2,
                Vc3 = franquia.Vc3,
                Status = franquia.Status == Status_Default.Ativo
            };
        }

        private Franquia BindViewObj(FranquiaCadastroModel viewModel, Franquia model)
        {


            model.Descricao = viewModel.Descricao;
            model.Ldn = viewModel.Ldn;
            model.Lc = viewModel.Lc;
            model.Minutos = Convert.ToInt32(viewModel.Minutos);
            model.Vc1 = viewModel.Vc1;
            model.Vc2 = viewModel.Vc2;
            model.Vc3 = viewModel.Vc3;



            return model;

        }

        [HttpPost()]
        [Route("api/FranquiaApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_FRANQUIA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/FranquiaApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FRANQUIA A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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
        public async Task<FranquiaIntegracaoResponse> Integracao_Cadastro_Franquia(Franquia obj, Plano plano)
        {
            FranquiaIntegracaoSaperxService integracao = new FranquiaIntegracaoSaperxService();
            return integracao.Cadastrar(obj, plano);

        }

        public async Task<FranquiaIntegracaoResponse> Integracao_Editar_Franquia(Franquia obj, Plano plano)
        {
            FranquiaIntegracaoSaperxService integracao = new FranquiaIntegracaoSaperxService();
            return integracao.Atualizar(obj, plano);

        }

    }
}