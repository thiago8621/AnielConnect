using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ProdutosCompApiController : BaseApiController
    {
        [AcceptVerbs("Get", "Post")]
        [Route("api/ProdutosCompApi/GetAll")]
        #region [GetAll]
        public async Task<DataTableResponse<ProdutosComp_ListModel>> GetAll(ProdutosComp_DataTableRequest request)
        {
            List<ProdutosComp_ListModel> retorno = new List<ProdutosComp_ListModel>();

            IOrderedEnumerable<ProdutosComp_ListModel> listaOrdenada = null;

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PRODUTOS_COMP");

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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO,A.CODIGO_NCM,A.CEST, A.STATUS ");

                    sql.AppendLine(" FROM TB_PRODUTOS_COMP A");

                    sql.AppendLine("WHERE A.ID > 0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sql.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ProdutosComp_ListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo_NCM = Convert.ToString(reader["CODIGO_NCM"]),
                                Codigo_CEST = Convert.ToString(reader["CEST"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Id);
            //Page e retornando DataTable
            return new DataTableResponse<ProdutosComp_ListModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }
        #endregion

        [HttpPost()]
        [Route("api/ProdutosCompApi/Gravar")]
        #region [Gravar]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ProdutosComp_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                ProdutosComp modelo = new ProdutosComp();

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<ProdutosComp>(session))
                {
                    if (value.Objeto.Id != null)
                        modelo = (await repository.selectFirst(p => p.Id == Convert.ToInt32(value.Objeto.Id)));

                    if (modelo == null)
                    {
                        modelo = new ProdutosComp
                        {
                            UsuarioCriacao = value.Usuario,
                            Status = Status_Default.Ativo
                        };
                    }

                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }

                    BindViewToObj(ref modelo, value.Objeto);

                    await repository.SaveOrUpdate(modelo);
                }

                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = Convert.ToInt32(value.Objeto.Id);
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        #endregion

        [HttpGet()]
        [Route("api/ProdutosCompApi/FindById")]
        #region [FindById]
        public async Task<GetApiRetorno<ProdutosComp_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ProdutosComp_CadastroModel> retorno = new GetApiRetorno<ProdutosComp_CadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<ProdutosComp>(session))
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

        #endregion

        [HttpPost()]
        [Route("api/ProdutosCompApi/AlterarStatusAtivoInativo")]
        #region [AlterarStatusAtivoInativo]
        public async Task<PostApiRetorno<Int64>> AlterarStatusAtivoInativo(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PRODUTOS_COMP SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        #endregion

        [HttpPost()]
        [Route("api/ProdutosCompApi/GetCombo")]
        #region [GetCombo]
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

                    StringBuilder sqlFromProdutosComp = new StringBuilder(" FROM TB_PRODUTOS_COMP A ");

                    #region Filtros

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($" A.STATUS = 1 ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($" A.ID = {request.id} ");
                    }

                    if (!String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER (A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFromProdutosComp.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFromProdutosComp.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    #endregion

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFromProdutosComp.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ProdutosCompComboResult
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
        #endregion

        [HttpPost()]
        [Route("api/ProdutosCompApi/GetComboNCM")]
        #region [GetComboNCM]
        public async Task<ObjCombo<String>> GetComboNCM(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession sessiont5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                   
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_NCM A");

                    #region Filtros

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty (request.id))
                    {
                        condicoes.Add($" A.CODIGO_NCM = '{request.id}' ");
                    }

                    if (!String.IsNullOrEmpty (request.idpai))
                    {
                        condicoes.Add($" A.CEST = '{request.idpai}' ");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }                   

                    #endregion


                    //Select
                    using (var com = sessiont5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.CODIGO_NCM, A.NCM, A.CEST ");

                        sql.AppendLine(sqlFrom.ToString());                       
                        sql.AppendLine(" ORDER BY UPPER(A.CODIGO_NCM) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, sessiont5.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var desc = Convert.ToString(reader["CODIGO_NCM"]) + " - " + Convert.ToString(reader["NCM"]) + " (CEST: " + Convert.ToString(reader["CEST"] + " )");

                               
                                if (!String.IsNullOrEmpty(Convert.ToString(reader["CODIGO_NCM"])))
                                {
                                    retorno.results.Add(new NCMComboResult
                                    {
                                        Id = Convert.ToString(reader["CODIGO_NCM"]),
                                        Text = desc,
                                        Codigo_CEST = Convert.ToString(reader["CEST"])
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
        #endregion

        [HttpGet()]
        [Route("api/ProdutosCompApi/ExcluirPlanoProdutosComp")]
        #region [ExcluirPlanoProdutosComp]
        public async Task<DeleteApiRetorno<Int64>> ExcluirPlanoProdutosComp(Int64 value, Int64 IdPlano)

        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Plano_Produto_Comp>(session))
                    using (var repoConfigNF = new Conexao<Config_NF_PlanoProduto>(session))
                    {

                        if (value != 0)
                        {
                            var modelo = (await repository.select(p => p.Id == value && p.Plano.Id == IdPlano)).FirstOrDefault();
                            
                            var objConfigNF = (await repoConfigNF.select(p => p.ProdutosComp == modelo.ProdutosComp && p.Plano.Id == IdPlano)).ToList();
                            if (objConfigNF.Any())
                            {
                                await repoConfigNF.Remove(objConfigNF);
                            }


                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
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
        #endregion

        #region [metodos privados]

        private void BindViewToObj(ref ProdutosComp objBanco, ProdutosComp_CadastroModel view)
        {
            objBanco.Id = Convert.ToInt64(view.Id);
            objBanco.Status = Status_Default.Ativo;
            objBanco.Descricao = view.Descricao;

            objBanco.Codigo_NCM = view.Codigo_NCM;
            objBanco.Codigo_CEST = view.Codigo_CEST;

        }

        private ProdutosComp_CadastroModel GetViewObj(ProdutosComp banco)
        {
            return new ProdutosComp_CadastroModel
            {

                Id = Convert.ToString(banco.Id),
                Descricao = banco.Descricao,

                Status = banco.Status == Status_Default.Ativo,
                Codigo_NCM = banco.Codigo_NCM,
                Codigo_CEST = banco.Codigo_CEST
            };
        }

        #endregion

    }
}