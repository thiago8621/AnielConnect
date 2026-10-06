using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class VencimentoApiController : BaseApiController
    {
        //[HttpPost()]
        //[Route("api/PlanoProdutoApi/GetAll")]
        //public async Task<DataTableResponse<PlanoProdutoIndexModel>> GetAll(PlanoProduto_DataTableRequest request)
        //{

        //    List<PlanoProdutoIndexModel> retorno = new List<PlanoProdutoIndexModel>();
        //    IOrderedEnumerable<PlanoProdutoIndexModel> listaOrdenada = null;
        //    Int32 recordsTotal = 0;

        //    using (ISession session = HibernateUtil.GetSession())
        //    {

        //        Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //        //Count
        //        using (var com = session.Connection.CreateCommand())
        //        {
        //            StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PLANO"); // WHERE STATUS = 1

        //            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    recordsTotal = Convert.ToInt32(reader["CO"]);
        //                }
        //            }
        //        }

        //        //Select
        //        using (var com = session.Connection.CreateCommand())
        //        {

        //            StringBuilder sql = new StringBuilder();

        //            sql.AppendLine("SELECT A.ID, A.CODIGO, A.DESCRICAO, 1 TIPO"); // A.DETALHESPLANO
        //            sql.AppendLine(" FROM TB_PLANO A ");
        //            sql.AppendLine(" WHERE A.SITUACAOPLANO = 1 ");

        //            if (!String.IsNullOrEmpty(request.search.value))
        //            {
        //                sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
        //                parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
        //            }

        //            sql.AppendLine(" UNION ");

        //            sql.AppendLine("SELECT B.ID, B.CODIGO, B.DESCRICAO, 2 TIPO");
        //            sql.AppendLine(" FROM TB_PRODUTO B ");

        //            if (!String.IsNullOrEmpty(request.search.value))
        //            {
        //                sql.AppendLine("WHERE ((UPPER(B.DESCRICAO) LIKE UPPER(@SEARCH))) ");
        //                parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
        //            }

        //            //sql.AppendLine(" WHERE A.STATUS = 1");

        //            //Busca com paginacao
        //            using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    retorno.Add(new PlanoProdutoIndexModel
        //                    {
        //                        Id = Convert.ToInt64(reader["ID"]),
        //                        Descricao = Convert.ToString(reader["DESCRICAO"]),
        //                        Codigo = Convert.ToString(reader["CODIGO"]),
        //                        Tipo = HtmlEnumExtensions.GetEnumDescription<PlanoProdutoEnum>(Convert.ToString(reader["TIPO"])),


        //                    });
        //                }
        //            }

        //        }
        //    }

        //    //Ordenacao
        //    if (request.order != null && request.order.Any())
        //    {
        //        int sortColumnIndex = request.order[0].column;
        //        string sortDirection = request.order[0].dir;
        //        switch (sortColumnIndex)
        //        {
        //            //case 1:     // Nome
        //            //    {
        //            //        if (sortDirection == "asc")
        //            //            listaOrdenada = retorno.OrderBy(p => p.Descricao);
        //            //        else
        //            //            listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
        //            //        break;
        //            //    }
        //            default:
        //                {
        //                    listaOrdenada = retorno.OrderBy(p => p.Descricao);
        //                    break;
        //                }
        //        }
        //    }



        //    //Page e retornando DataTable
        //    return new DataTableResponse<PlanoProdutoIndexModel>
        //    {
        //        draw = request.draw,
        //        recordsTotal = recordsTotal,
        //        recordsFiltered = recordsTotal,
        //        data = listaOrdenada,
        //        error = ""
        //    };

        //}





        //private AlterarLoginRadiusClienteViewModel GetViewObjLogin(Cliente_PlanoProduto obj)
        //{
        //    return new AlterarLoginRadiusClienteViewModel
        //    {
        //        Id = obj.Id,
        //        Login = obj.Login_Radius,
        //        Confirmar_Senha = obj.Senha_Radius,
        //        Senha = obj.Senha_Radius
        //    };
        //}




        //private void BindViewToObj(ISession session, Cliente_PlanoProduto objBanco, AlterarLoginRadiusClienteViewModel view)
        //{
        //    objBanco.Login_Radius = view.Login;
        //    objBanco.Senha_Radius = view.Senha;
        //}


        [HttpPost()]
        [Route("api/VencimentoApi/GetList")]
        public async Task<GetApiRetorno<List<VencimentoListModel>>> GetList()
        {
            GetApiRetorno<List<VencimentoListModel>> retorno = new GetApiRetorno<List<VencimentoListModel>>
            {
                Objeto = new List<VencimentoListModel>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                                        
                    var listaVencimentos = await new Conexao<Vencimento>(session).queryover().ListAsync();

                    retorno.Objeto.AddRange(listaVencimentos.Select(p => new VencimentoListModel
                    {
                        Id = p.Id,
                        Descricao = Convert.ToString(p.DiaVencimento),

                    }));

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
        [Route("api/VencimentoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_VENCIMENTO A ")
                        .AppendLine("INNER JOIN TB_FORMA_PAGAMENTO_VENCIMENTO B ")
                        .AppendLine("ON (A.ID = B.ID_VENCIMENTO)")
                        .AppendLine("INNER JOIN TB_FECHAMENTO C ")
                        .AppendLine("ON C.ID = A.ID_FECHAMENTO");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                        condicoes.Add($"C.STATUS = 1");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"B.ID_FORMA_PAGAMENTO = {request.idpai}");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    try
                    {
                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(A.DIA_VENCIMENTO = {Convert.ToInt32(request.q)})");
                        }
                    }
                    catch
                    {

                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DIA_VENCIMENTO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.DIA_VENCIMENTO");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["DIA_VENCIMENTO"])
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

        [Route("api/VencimentoApi/GetListDiasVencimento")]
        [HttpGet()]
        public async Task<GetApiRetorno<List<object>>> GetListDiasVencimento(Int64 idFormaPagamento)
        {
            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    List<Forma_Pagamento_Vencimento> lista = null;
                    using (var repository = new Conexao<Forma_Pagamento_Vencimento>(session))
                    {
                        lista = (await repository.select(s => s.Forma_Pagamento.Id == idFormaPagamento)).ToList();
                    }

                    retorno.Objeto = new List<object>(lista.Where(w => w.Vencimento.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Select(s => new
                    {
                        id = s.Vencimento.Id,
                        diaVencimento = s.Vencimento.DiaVencimento
                    }));

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

        //[HttpPost()]
        //[Route("api/PlanoProdutoApi/excluirlista")]
        //public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        //{
        //    PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                String _ids = $"({String.Join(",", value.ids)})";

        //                StringBuilder sql = new StringBuilder($"DELETE FROM TB_DESCONTO WHERE ID IN {_ids}");

        //                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

        //                retorno.Ok = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Ok = true;
        //        retorno.Mensagem = ex.Message;
        //        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
        //    }

        //    return retorno;
        //}

        //[Route("api/Plano_Produtoapi/ExcluirGrid")]
        //[HttpGet()]
        //public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value, String usuario)
        //{
        //    DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            using (var repository = new Conexao<Cliente_PlanoProduto>(session))
        //            {

        //                Cliente_PlanoProduto modelo = null;
        //                if (value != 0)
        //                {
        //                    modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
        //                    modelo.Status = Status_PlanoProduto.Cancelado;
        //                    modelo.UsuarioAlteracao = usuario;

        //                    await repository.SaveOrUpdate(modelo);

        //                    //repository.Remove(modelo);

        //                    retorno.IdObjeto = modelo.Id;
        //                    retorno.Ok = true;
        //                }
        //                else
        //                {
        //                    throw new Exception(Resources.RsBase.NAOENCONTRADO);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.IdObjeto = 0;
        //        retorno.Mensagem = ex.Message;
        //        if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
        //    }

        //    return retorno;
        //}



    }
}
