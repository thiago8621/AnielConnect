using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using OpenXmlPowerTools;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class TecnologiaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/TecnologiaApi/GetAll")]
        public async Task<DataTableResponse<Tecnologia_ListModel>> GetAll(Tecnologia_DataTableRequest request)
        {

            List<Tecnologia_ListModel> retorno = new List<Tecnologia_ListModel>();
            IOrderedEnumerable<Tecnologia_ListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                sqlFrom.Append(" FROM TB_TECNOLOGIA A WHERE A.ID > 0 ");

                if (!String.IsNullOrEmpty(request.Status))
                {
                    sqlFrom.AppendLine(" AND A.STATUS = @STATUS ");
                    parametros.Add("@STATUS", Convert.ToInt16(request.Status));
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO ");
                    sql.Append(sqlFrom);

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

                    sql.AppendLine(" SELECT A.ID, A.CODIGO, A.DESCRICAO, A.STATUS ");
                    sql.Append(sqlFrom);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Tecnologia_ListModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToString(reader["STATUS"]) == "0" ? RsCadastros.INATIVO : RsCadastros.ATIVO
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                var coluna = request.columns[sortColumnIndex].data;
                string sortDirection = request.order[0].dir;
                switch (coluna)
                {
                    case "codigo":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Codigo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                            break;
                        }

                    case "status":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }

                    case "descricao":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }


                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Tecnologia_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/TecnologiaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Tecnologia_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Tecnologia modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tecnologia>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Tecnologia
                            {
                                UsuarioCriacao = value.Usuario,
                                Id = value.Objeto.Id,
                                Codigo = value.Objeto.Codigo,
                                Descricao = value.Objeto.Descricao
                            };
                            value.Objeto.Status = true;
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
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
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private Tecnologia_CadastroModel GetViewObj(Tecnologia obj)
        {
            return new Tecnologia_CadastroModel
            {
                Id = obj.Id,
                Codigo = obj.Codigo,
                Descricao = obj.Descricao,
                Status = obj?.Status == Status_Default.Ativo,
            };
        }

        private void BindViewToObj(ISession session, ref Tecnologia objBanco, Tecnologia_CadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Codigo = view.Codigo;
            objBanco.Descricao = view.Descricao;
            objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
        }
        [HttpGet()]
        [Route("api/TecnologiaApi/FindById")]
        public async Task<GetApiRetorno<Tecnologia_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Tecnologia_CadastroModel> retorno = new GetApiRetorno<Tecnologia_CadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Tecnologia>(session))
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
        [Route("api/TecnologiaApi/GetCombo")]
        public async Task<ObjCombo<Int64?>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64?> retorno = new ObjCombo<Int64?>
            {
                results = new List<IObjComboResult<Int64?>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_TECNOLOGIA A ");

                    List<String> condicoes = new List<String>();


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%' ) OR (UPPER(A.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%' ))");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.CODIGO");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<Int64?>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = String.Format("{0} - {1}", reader["CODIGO"], reader["DESCRICAO"])
                                });
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


    }
}