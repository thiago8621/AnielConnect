using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Twilio.Rest.Trunking.V1;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Motivo_InviabilidadeApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/Motivo_Inviabilidadeapi/GetAll")]
        public async Task<DataTableResponse<Motivo_InviabilidadeListaModel>> GetAll(Motivo_InviabilidadeDataTableRequest request)
        {
            List<Motivo_InviabilidadeListaModel> retorno = new List<Motivo_InviabilidadeListaModel>();
            IOrderedEnumerable<Motivo_InviabilidadeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MOTIVO_INVIABILIDADE");

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

                    sql.AppendLine("SELECT A.ID, A.MOTIVO, A.STATUS ");
                    sql.AppendLine(" FROM TB_MOTIVO_INVIABILIDADE A ");

                    //   sql.AppendLine(" WHERE A.STATUS = 1 ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.MOTIVO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_InviabilidadeListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Motivo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Motivo);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Motivo);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Motivo_InviabilidadeListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private Motivo_InviabilidadeCadastroModel GetViewObj(Motivo_Inviabilidade obj)
        {
            return new Motivo_InviabilidadeCadastroModel
            {
                Id = obj.Id,
                Motivo = obj.Motivo,
                Status = obj.Status.Value == Status_Default.Ativo ? true : false

            };

        }

        private void BindViewToObj(ISession session, ref Motivo_Inviabilidade objBanco, Motivo_InviabilidadeCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Motivo = view.Motivo;
            objBanco.Motivo_Principal = view.Motivo_Principal;
        }


        [HttpPost()]
        [Route("api/Motivo_Inviabilidadeapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Motivo_InviabilidadeCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Inviabilidade modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Inviabilidade>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Motivo_Inviabilidade
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };

                        }

                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

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

        [HttpPost()]
        [Route("api/Motivo_Inviabilidadeapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_INVIABILIDADE SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpGet()]
        [Route("api/Motivo_Inviabilidadeapi/FindById")]
        public async Task<GetApiRetorno<Motivo_InviabilidadeCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_InviabilidadeCadastroModel> retorno = new GetApiRetorno<Motivo_InviabilidadeCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Motivo_Inviabilidade>(session))
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

        [HttpPost()]
        [Route("api/Motivo_Inviabilidadeapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_INVIABILIDADE A ");

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
                        condicoes.Add($"(UPPER(A.MOTIVO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.MOTIVO != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.MOTIVO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.MOTIVO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["MOTIVO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["MOTIVO"])
                                    });

                                }
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

        [HttpPost()]
        [Route("api/Motivo_Inviabilidadeapi/GetComboOptGroup")]
        public async Task<ObjComboOptGroup<Int64, long?>> GetComboOptGroup(RequestObjComboConfig<Int64?, Motivo_Principal_InviabilidadeEnum> request)
        {
            var retorno = new ObjComboOptGroup<Int64, Int64?>
            {
                results = new List<IObjComboOptGroupResult<long, long?>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_INVIABILIDADE A ");

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
                        condicoes.Add($"(UPPER(A.MOTIVO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.MOTIVO != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                    List<Motivo_InviabilidadeComboResult<long>> records = new List<Motivo_InviabilidadeComboResult<long>>();
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.MOTIVO, A.MOTIVO_PRINCIPAL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.MOTIVO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {

                            while (await reader.ReadAsync())
                            {
                                var obj = new Motivo_InviabilidadeComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["MOTIVO"]),
                                    Motivo = reader["MOTIVO_PRINCIPAL"] != DBNull.Value ? (Motivo_Principal_InviabilidadeEnum?)Convert.ToInt32(reader["MOTIVO_PRINCIPAL"]) :
                                                                                          (Motivo_Principal_InviabilidadeEnum?)null,
                                };

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["MOTIVO"])))
                                {
                                    records.Add(obj);
                                }
                            }
                        }
                        var grouped = records.GroupBy(p => p.Motivo);
                        grouped.ForEach(param =>
                        {
                            var itens = new List<IObjComboResult<long>>();
                            param.ForEach(result =>
                            {
                                itens.Add(result);
                            });

                            retorno.results.Add(new ObjComboOptGroupResult<Int64, long?>
                            {
                                Id_Grupo = param.Key != null ? Convert.ToInt64(param.Key) : (long?)null,
                                Text = param.Key != null ? HtmlEnumExtensions.GetEnumDescription(param.Key) : "Outros Motivos",
                                Children = itens,
                            });
                        });
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }


    }
}