using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class SegmentoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Segmentoapi/GetAll")]
        public async Task<DataTableResponse<Segmento_ListaModel>> GetAll(Segmento_DataTableRequest request)
        {
            List<Segmento_ListaModel> retorno = new List<Segmento_ListaModel>();
            IOrderedEnumerable<Segmento_ListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            //count
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_SEGMENTO_SERVICO SEG ");

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(" SELECT COUNT(CODIGO) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select do banco de dados
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT SEG.CODIGO, SEG.DESCRICAO, SEG.STATUS ");
                    sql.AppendLine(sqlFrom.ToString());

                    List<string> filtros = new List<string>();

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        filtros.Add("SEG.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(SEG.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Segmento_ListaModel
                            {
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }
                }
            }


            //Ordenacao dos dados selecionados
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(seg => seg.Codigo);
                            else
                                listaOrdenada = retorno.OrderByDescending(seg => seg.Codigo);
                            break;
                        }

                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(seg => seg.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(seg => seg.Descricao);
                            break;
                        }

                    case 3:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(seg => seg.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(seg => seg.Status);
                            break;
                        }
                }
            }


            return new DataTableResponse<Segmento_ListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpGet()]
        [Route("api/Segmentoapi/FindById")]
        public async Task<GetApiRetorno<Segmento_CadastroModel>> FindById(String codigo)
        {

            GetApiRetorno<Segmento_CadastroModel> retorno = new GetApiRetorno<Segmento_CadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Segmento>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(x => x.Codigo == codigo));
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


        private void BindViewToObj(ISession session, ref Segmento objBanco, Segmento_CadastroModel view)
        {
            objBanco.Descricao = view.Descricao;
            objBanco.Descricao = view.Descricao;
            if (objBanco.Codigo != null)
            {
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }


        }


        [HttpPost()]
        [Route("api/Segmentoapi/Gravar")]
        public async Task<PostApiRetorno<String>> Gravar([FromBody] ParamWebApi<Segmento_CadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                Segmento modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Segmento>(session))
                    {
                        if (value.Objeto.Codigo != null) modelo = (await repository.select(x => x.Codigo == value.Objeto.Codigo)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Segmento
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario,
                                Codigo = value.Objeto.Codigo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }

                    retorno.IdObjeto = modelo.Codigo;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }



        [HttpPost()]
        [Route("api/Segmentoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarSegmentoStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        List<string> codigos = new List<string>();

                        foreach (var co in value.cods)
                        {
                            codigos.Add($"'{co}'");
                        }

                        StringBuilder sql = new StringBuilder($"UPDATE TB_SEGMENTO_SERVICO SEG SET SEG.STATUS = {value.status} WHERE SEG.CODIGO IN ({String.Join(",", codigos)})");

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
        [Route("api/Segmentoapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SEGMENTO_SERVICO A ");

                    List<String> condicoes = new List<String>();



                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.CODIGO) = '{StringService.Truncate(request.id.ToUpperInvariant(),15)}'");
                    }

                    if (!String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }


                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODIGO) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODIGO,A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["CODIGO"]),
                                    Text = $"{Convert.ToString(reader["DESCRICAO"])}"
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

        private Segmento_CadastroModel GetViewObj(Segmento model)
        {
            return new Segmento_CadastroModel
            {
                Codigo = model.Codigo,
                Descricao = model.Descricao,
                Status = model.Status == Status_Default.Ativo
            };
        }
    }
}