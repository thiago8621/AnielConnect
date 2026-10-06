using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class TipoTarefaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/TipoTarefaapi/GetAll")]
        public async Task<DataTableResponse<TipoTarefaListaModel>> GetAll(TipoTarefaDataTableRequest request)
        {
            List<TipoTarefaListaModel> retorno = new List<TipoTarefaListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_TIPO_TAREFA");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS ");
                    sql.AppendLine(" FROM TB_TIPO_TAREFA A ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "id":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY ID ");
                                    else
                                        sql.AppendLine(" ORDER BY ID DESC ");
                                    break;
                                }
                            case "descricao":     
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY DESCRICAO DESC");
                                    break;
                                }
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY STATUS");
                                    else
                                        sql.AppendLine(" ORDER BY STATUS DESC");
                                    break;
                                }

                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null,request.length,request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new TipoTarefaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            

            //Page e retornando DataTable
            return new DataTableResponse<TipoTarefaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/TipoTarefaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<TipoTarefaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                TipoTarefa modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<TipoTarefa>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new TipoTarefa
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }

                        else
                        {
                            // modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private void BindViewToObj(ISession session, TipoTarefa objBanco, TipoTarefaCadastroModel view)
        {
            objBanco.Descricao = view.Descricao;
        }

        [HttpGet()]
        [Route("api/TipoTarefaApi/FindById")]
        public async Task<GetApiRetorno<TipoTarefaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<TipoTarefaCadastroModel> retorno = new GetApiRetorno<TipoTarefaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<TipoTarefa>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        private TipoTarefaCadastroModel GetViewObj(TipoTarefa tipoTarefa)
        {
            return new TipoTarefaCadastroModel
            {
                Id = tipoTarefa.Id,
                Descricao = tipoTarefa.Descricao,
                Status = tipoTarefa.Status == Status_Default.Ativo
            };
        }

        [HttpPost()]
        [Route("api/TipoTarefaApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<TipoTarefa>(session))
            {
                foreach (var tarefa in await repo.select())
                {
                    retorno.results.Add(new ObjComboResult<Int64>
                    {
                        Id = tarefa.Id,
                        Text = $"{tarefa.Id} - {tarefa.Descricao}"
                    });
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/TipoTarefaApi/GetComboPrioridade")]
        public ObjCombo<Int64> GetComboPrioridade(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            retorno.results.Add(new ObjComboResult<Int64>
            {
                Id = (long)NivelPrioridadeTarefaVendedorEnum.Baixo,
                Text = HtmlEnumExtensions.GetEnumDescription(NivelPrioridadeTarefaVendedorEnum.Baixo)
            });

            retorno.results.Add(new ObjComboResult<Int64>
            {
                Id = (long)NivelPrioridadeTarefaVendedorEnum.Medio,
                Text = HtmlEnumExtensions.GetEnumDescription(NivelPrioridadeTarefaVendedorEnum.Medio)
            });

            retorno.results.Add(new ObjComboResult<Int64>
            {
                Id = (long)NivelPrioridadeTarefaVendedorEnum.Alto,
                Text = HtmlEnumExtensions.GetEnumDescription(NivelPrioridadeTarefaVendedorEnum.Alto)
            });


            return retorno;
        }

        [HttpPost()]
        [Route("api/TipoTarefaApi/GetById")]
        public async Task<TipoTarefa> GetById(long idTipo)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<TipoTarefa>(session))
            {
                return await repo.selectFirst(x => x.Id == idTipo);
            }
        }
    }
}