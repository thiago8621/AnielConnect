using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
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
    public class Meio_CaptacaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Meio_Captacaoapi/GetAll")]
        public async Task<DataTableResponse<Meio_CaptacaoListaModel>> GetAll(Meio_Captacao_DataTableRequest request)
        {

            List<Meio_CaptacaoListaModel> retorno = new List<Meio_CaptacaoListaModel>();
            IOrderedEnumerable<Meio_CaptacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MEIO_CAPTACAO WHERE STATUS = 1");  

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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS ");
                    sql.AppendLine(" FROM TB_MEIO_CAPTACAO A ");

                  //  sql.AppendLine(" WHERE A.STATUS = 1 ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Meio_CaptacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
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
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Id);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Meio_CaptacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Meio_CaptacaoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Meio_CaptacaoListaModel>> retorno = new GetApiRetorno<IEnumerable<Meio_CaptacaoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Meio_Captacao>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Status == Status_Default.Ativo);

                    retorno.Objeto = lista.Select(p => new Meio_CaptacaoListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao
                        
                    });

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

        #region mobile
        
        [HttpGet()]
        [Route("api/Meio_Captacaoapi/GetListCaptacaoDesconto")]
        public async Task<GetApiRetorno<List<Meio_CaptacaoDescontoModel>>> GetListCaptacaoDesconto()
        {
            GetApiRetorno<List<Meio_CaptacaoDescontoModel>> retorno = new GetApiRetorno<List<Meio_CaptacaoDescontoModel>>();
            retorno.Objeto = new List<Meio_CaptacaoDescontoModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID AS ID_MEIO, A.DESCRICAO AS MEIO_DESCRICAO, C.ID AS ID_DESCONTO, C.DESCRICAO AS DESCONTO_DESCRICAO, ");
                        sql.AppendLine(" C.DESCONTO_CAPTACAO, C.VALOR_DESCONTO, C.PERCENTUAL_DESCONTO, D.DESCRICAO AS TIPO_DESCONTO");
                        sql.AppendLine(" FROM TB_MEIO_CAPTACAO A ");
                        sql.AppendLine(" left JOIN TB_DESCONTO_MEIO_CAPTACAO B");
                        sql.AppendLine(" ON(B.ID_MEIO_CAPTACAO = A.ID)");
                        sql.AppendLine(" left JOIN TB_DESCONTO C ");
                        sql.AppendLine(" ON(B.ID_DESCONTO = C.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_TIPO_DESCONTO D");
                        sql.AppendLine(" ON(C.ID_TIPO_DESCONTO = D.ID)");
                        sql.AppendLine(" WHERE A.STATUS = 1 ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                DescontoListaModel desconto = new DescontoListaModel();
                                if (reader["ID_DESCONTO"] != DBNull.Value)
                                {
                                    desconto.Id = Convert.ToInt64(reader["ID_DESCONTO"]);
                                    desconto.Descricao = reader["DESCONTO_DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCONTO_DESCRICAO"]) : null;
                                    desconto.Desconto_Captacao = reader["DESCONTO_CAPTACAO"]?.ToString(); //   reader["DESCONTO_CAPTACAO"] != DBNull.Value ? (Desconto_Captacao)reader["DESCONTO_CAPTACAO"] : (Desconto_Captacao?)null;
                                    desconto.Valor_Desconto = reader["VALOR_DESCONTO"]?.ToString(); // != DBNull.Value ? Convert.ToInt64(reader["VALOR_DESCONTO"]) : 0.0;
                                    desconto.Percentual_Desconto = reader["PERCENTUAL_DESCONTO"]?.ToString(); //!= DBNull.Value ? Convert.ToInt64(reader["PERCENTUAL_DESCONTO"]) : 0.0;
                                    desconto.Tipo_Desconto = reader["TIPO_DESCONTO"]?.ToString();
                                }

                                retorno.Objeto.Add(new Meio_CaptacaoDescontoModel
                                {
                                    Id = Convert.ToInt64(reader["ID_MEIO"]),
                                    Descricao = reader["MEIO_DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["MEIO_DESCRICAO"]) : null,
                                    Desconto = desconto
                                });
                            }
                        }
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;

        }
        #endregion


        [HttpPost()]
        [Route("api/Meio_Captacaoapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MEIO_CAPTACAO A WHERE ");

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
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
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

        [HttpPost()]
        [Route("api/Meio_Captacaoapi/GetComboCliente")]
        public async Task<ObjCombo<Int64>> GetComboCliente(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MEIO_CAPTACAO A INNER JOIN TB_CLIENTE B ON (B.ID_MEIO_CAPTACAO = A.ID)");

                    List<String> condicoes = new List<String>();

                   
                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT COUNT(A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT A.ID, A.DESCRICAO ");
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



        private Meio_CaptacaoCadastroModel GetViewObj(Meio_Captacao obj)
        {
            return new Meio_CaptacaoCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Status = obj.Status == Status_Default.Ativo

                //Valor_Desconto = obj.Valor_Desconto,
                //Tem_Desconto = obj.Tem_Desconto,
                //Repeticoes_Desconto = obj.Repeticoes_Desconto,
                //Percentual_Desconto = obj.Percentual_Desconto,
                //Desconto_Captacao = obj.Desconto_Captacao,

                //Id_Plano = obj.Plano != null ? obj.Plano.Id : (Int64?)null,
                //Id_Produto = obj.Produto != null ? obj.Produto.Id : (Int64?)null,

            };
        }

        private void BindViewToObj(ISession session, ref Meio_Captacao objBanco, Meio_CaptacaoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
           // objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;

            //objBanco.Valor_Desconto = view.Valor_Desconto;
            //objBanco.Tem_Desconto = view.Tem_Desconto;
            //objBanco.Repeticoes_Desconto = view.Repeticoes_Desconto;
            //objBanco.Percentual_Desconto = view.Percentual_Desconto;
            //objBanco.Id = view.Id;
            //objBanco.Desconto_Captacao = view.Desconto_Captacao;

            //objBanco.Plano = view.Id_Plano.HasValue ? new BuscaGenericaRepository<Plano>(session, null).FindById(view.Id_Plano.Value).Result : null;
            //objBanco.Produto = view.Id_Produto.HasValue ? new BuscaGenericaRepository<Produto>(session, null).FindById(view.Id_Produto.Value).Result : null;

        }

        [HttpPost()]
        [Route("api/Meio_Captacaoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MEIO_CAPTACAO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Meio_Captacaoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MEIO_CAPTACAO WHERE ID IN {_ids}");

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
        [Route("api/Meio_Captacaoapi/FindById")]
        public async Task<GetApiRetorno<Meio_CaptacaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Meio_CaptacaoCadastroModel> retorno = new GetApiRetorno<Meio_CaptacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Meio_Captacao>(session))
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
        [Route("api/Meio_Captacaoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Meio_CaptacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Meio_Captacao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Meio_Captacao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Meio_Captacao
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
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
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Meio_Captacao/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Meio_Captacao>(session))
                    {

                        Meio_Captacao modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

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


        [Route("api/Meio_Captacaoapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Desconto_Meio_Captacao>(session))
                    {

                        Desconto_Meio_Captacao modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

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

        [HttpGet()]
        [Route("api/Meio_Captacao/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine("FROM TB_MEIO_CAPTACAO A");
                        sql.AppendLine("WHERE (UPPER(A.DESCRICAO) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
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
