
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
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
    public class ComissionamentoApiController : ApiController
    {
        [HttpPost()]
        [Route("api/Comissionamentoapi/GetAll")]
        public async Task<DataTableResponse<ComissionamentoListaModel>> GetAll(Comissionamento_DataTableRequest request)
        {
            List<ComissionamentoListaModel> retorno = new List<ComissionamentoListaModel>();
            IOrderedEnumerable<ComissionamentoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_COMISSIONAMENTO A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FILIAL B ");
                sqlFrom.AppendLine(" ON(B.ID_COMISSIONAMENTO = A.ID) ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine(" SELECT A.ID, A.CODIGO, A.DESCRICAO, A.STATUS, B.NOME ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((A.CODIGO LIKE @SEARCH) OR (UPPER(A.DESCRICAO) LIKE @SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));

                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ComissionamentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Codigo = Convert.ToString(reader["CODIGO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Filial = Convert.ToString(reader["NOME"]),
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
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ComissionamentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Comissionamentoapi/GetAll_PlanoProduto")]
        public async Task<DataTableResponse<Comissionamento_PlanoProdutoListaModel>> GetAll_PlanoProduto(Comissionamento_PlanoProdutoDataTableRequest request)
        {
            List<Comissionamento_PlanoProdutoListaModel> retorno = new List<Comissionamento_PlanoProdutoListaModel>();
            IOrderedEnumerable<Comissionamento_PlanoProdutoListaModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@ID_COMISSIONAMENTO", request.Id_Comissionamento);

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.UNIDADE, A.QUANTIDADE, B.DESCRICAO ");
                    sql.AppendLine(" FROM TB_COMISSAO_PLANO_PRODUTO A ");
                    sql.AppendLine(" INNER JOIN TB_PLANO B ");
                    sql.AppendLine("  ON (A.ID_PLANO = B.ID) ");
                    sql.AppendLine("WHERE A.ID_COMISSIONAMENTO = @ID_COMISSIONAMENTO");

                    sql.AppendLine(" UNION ");

                    sql.AppendLine(" SELECT A.ID, A.UNIDADE, A.QUANTIDADE, B.DESCRICAO ");
                    sql.AppendLine(" FROM TB_COMISSAO_PLANO_PRODUTO A ");
                    sql.AppendLine(" INNER JOIN TB_PRODUTO B ");
                    sql.AppendLine("  ON (A.ID_PRODUTO = B.ID) ");
                    sql.AppendLine("WHERE A.ID_COMISSIONAMENTO = @ID_COMISSIONAMENTO");



                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Comissionamento_PlanoProdutoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                IdComissionamento = request.Id_Comissionamento,
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty,
                                Unidade = Convert.ToString(reader["UNIDADE"]),
                                Quantidade = 
                                    reader["QUANTIDADE"] != DBNull.Value ?
                                        Convert.ToString(reader["QUANTIDADE"]) :
                                        "0"
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Descricao);

            //Page e retornando DataTable
            return new DataTableResponse<Comissionamento_PlanoProdutoListaModel>
            {
                draw = request.draw,
                //recordsTotal = listaOrdenada.Count(),
                recordsFiltered = listaOrdenada.Count(),
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/ComissionamentoApi/busca")]
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

                        sql.AppendLine("SELECT A.ID,A.CODIGO, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_COMISSIONAMENTO A");
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

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<ComissionamentoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<ComissionamentoListaModel>> retorno = new GetApiRetorno<IEnumerable<ComissionamentoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Comissionamento>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new ComissionamentoListaModel
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Descricao = p.Descricao,



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

        private ComissionamentoCadastroModel GetViewObj(Comissionamento obj)
        {
            var _obj = new ComissionamentoCadastroModel();

            _obj.Id = obj.Id;
            _obj.Codigo = Convert.ToString(obj.Codigo);
            _obj.Descricao = obj.Descricao;
            _obj.Status = obj.Status.Value == Status_Default.Ativo ? true : false;

            return _obj;
        }

        private void BindViewToObj(ISession session, ref Comissionamento objBanco, ComissionamentoCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }
        }

        [HttpPost()]
        [Route("api/ComissionamentoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_COMISSIONAMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/ComissionamentoApi/FindById")]
        public async Task<GetApiRetorno<ComissionamentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ComissionamentoCadastroModel> retorno = new GetApiRetorno<ComissionamentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Comissionamento>(session))
            using (var repoFilial = new Conexao<Filial>(session))
            {
                try
                {
                    var modelo = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(modelo);

                    var filialAtual = await repoFilial.selectFirst(p => p.Comissionamento == modelo);
                    retorno.Objeto.Id_Filial = filialAtual?.Id;

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
        [Route("api/ComissionamentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<ComissionamentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {

                    Comissionamento modelo = null;
                    using (var repositoryFilial = new Conexao<Filial>(session))
                    using (var repository = new Conexao<Comissionamento>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Comissionamento
                            {
                                //Status = Status_Default.Ativo,
                                Status = Status_Default.Ativo,
                                Codigo = Convert.ToString((await repository.MaxString("CODIGO", "TB_COMISSIONAMENTO")) + 1),
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        if (modelo.Id != 0)
                        {
                            var filialAnterior = await repositoryFilial.selectFirst(p => p.Comissionamento.Id == modelo.Id);

                            if (filialAnterior != null)
                            {
                                if (value.Objeto.Id_Filial.HasValue && filialAnterior.Id != value.Objeto.Id_Filial.Value)
                                {
                                    filialAnterior.Meta = null;
                                    await repositoryFilial.SaveOrUpdate(filialAnterior);
                                }
                            }
                        }

                        var filialAtual = await repositoryFilial.selectFirst(p => p.Id == value.Objeto.Id_Filial.Value);
                        filialAtual.Comissionamento = modelo;
                        modelo.Id_Filial = filialAtual.Id;
                        await repository.SaveOrUpdate(modelo);
                        await repositoryFilial.SaveOrUpdate(filialAtual);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }


            }


            return retorno;
        }

        [Route("api/ComissionamentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Comissionamento>(session))
                    {

                        Comissionamento modelo = null;
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

        [HttpPost()]
        [Route("api/ComissionamentoApi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, String> parametros = new Dictionary<string, string>();

                    using (var repoComissaoPlanoProduto = new Conexao<Comissionamento_PlanoProduto>(session))
                    {
                        foreach (var id in value.ids)
                        {
                            var _comissao_plano_produto = repoComissaoPlanoProduto.select(p => p.Comissionamento.Id == id).Result;

                            if (_comissao_plano_produto.Count != 0)
                            {
                                throw new Exception("Remova os Planos do Comissionamento antes de apagá-lo");
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        //Correção de erro por foreign key
                        StringBuilder sql2 = new StringBuilder($"UPDATE  TB_FILIAL SET ID_COMISSIONAMENTO = NULL WHERE ID_COMISSIONAMENTO IN { _ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql2, new Dictionary<String, Object>(), session.Connection, null);

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_COMISSIONAMENTO WHERE ID IN {_ids}");

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

        [Route("api/ComissionamentoApi/CarregarPlanoProduto")]
        [HttpGet()]
        public async Task<PostApiRetorno<Int64>> CarregarPlanoProduto(Int64 IdComisionamento, Boolean CarregaValores)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Comissionamento>(session))
                    using (var repoPlano = new Conexao<Plano>(session))
                    using (var repoProduto = new Conexao<Produto>(session))
                    using (var repoComissionamento_PlanoProduto = new Conexao<Comissionamento_PlanoProduto>(session))
                    {
                        Comissionamento comissionamento = null;
                        if (IdComisionamento != 0)
                        {
                            comissionamento = (await repository.select(p => p.Id == IdComisionamento)).FirstOrDefault();

                            //Limpa planos e produtos anteriores
                            await LimpaPlanoProduto(session, comissionamento);

                            //Preenche o que vai gravar
                            List<Comissionamento_PlanoProduto> _agravar = new List<Comissionamento_PlanoProduto>();

                            IList<Plano> planos = repoPlano.select().Result;

                            _agravar.AddRange(planos.Select(p => new Comissionamento_PlanoProduto
                            {
                                Comissionamento = comissionamento,
                                Plano = p,
                                Quantidade = CarregaValores ? p.ValorPlano : 0,
                                Unidade = String.Empty
                            }));

                            IList<Produto> produtos = repoProduto.select().Result;

                            _agravar.AddRange(produtos.Select(p => new Comissionamento_PlanoProduto
                            {
                                Comissionamento = comissionamento,
                                Produto = p,
                                Quantidade = CarregaValores ? p.ValorProduto : 0,
                                Unidade = String.Empty
                            }));

                            // Grava a lista
                            await repoComissionamento_PlanoProduto.SaveOrUpdate(_agravar);

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


        [Route("api/ComissionamentoApi/LimpaPlanoProduto")]
        [HttpGet()]
        public async Task<PostApiRetorno<Int64>> LimpaPlanoProduto(Int64 IdComisionamento)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Comissionamento>(session))
                    {
                        Comissionamento comissionamento = null;
                        if (IdComisionamento != 0)
                        {
                            comissionamento = (await repository.select(p => p.Id == IdComisionamento)).FirstOrDefault();

                            await LimpaPlanoProduto(session, comissionamento);

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

        public async Task<Boolean> LimpaPlanoProduto(ISession session, Comissionamento comissionamento)
        {
            using (var repository = new Conexao<Comissionamento_PlanoProduto>(session))
            {
                await repository.Remove((await repository.select(p => p.Comissionamento.Id == comissionamento.Id)).ToList());
            }

            return true;
        }

        public async Task<GetApiRetorno<List<ComissionamentoExtrato>>> GetValorNoPeriodo(Int64 Id_Funcionario, DateTime Data_Inicio, DateTime Data_Fim)
        {
            GetApiRetorno<List<ComissionamentoExtrato>> retorno = new GetApiRetorno<List<ComissionamentoExtrato>>();
            retorno.Objeto = new List<ComissionamentoExtrato>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder()

                        .AppendLine(" SELECT COALESCE(F.DESCRICAO, G.DESCRICAO) AS NOME_PLANO_PRODUTO, ")
                        .AppendLine("      E.QUANTIDADE AS PONTOS, I.COEFICIENTE, B.DATA_VENDA ")

                        .AppendLine(" FROM TB_FUNCIONARIO A ")

                        .AppendLine(" INNER JOIN TB_VENDA B ")
                        .AppendLine(" ON(B.ID_VENDEDOR = A.ID) ")
                        .AppendLine(" INNER JOIN TB_CLIENTE_PLANO_PRODUTO C ")
                        .AppendLine(" ON(C.ID_VENDA = B.ID) ")
                        .AppendLine(" INNER JOIN TB_COMISSIONAMENTO D ")
                        .AppendLine(" ON(A.ID_COMISSIONAMENTO = D.ID) ")
                        .AppendLine(" INNER JOIN TB_COMISSAO_PLANO_PRODUTO E ")
                        .AppendLine(" ON(D.ID = E.ID_COMISSIONAMENTO AND(C.ID_PLANO = E.ID_PLANO OR C.ID_PRODUTO = E.ID_PRODUTO)) ")
                        .AppendLine(" LEFT JOIN TB_FAIXA_COMISSIONAMENTO H ")
                        .AppendLine(" ON(H.ID = A.ID_FAIXA) ")
                        .AppendLine(" LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM I ")
                        .AppendLine(" ON(I.ID_FAIXA = H.ID) ")
                        .AppendLine(" LEFT JOIN TB_PLANO F ")
                        .AppendLine(" ON(F.ID = E.ID_PLANO) ")
                        .AppendLine(" LEFT JOIN TB_PRODUTO G ")
                        .AppendLine(" ON(G.ID = E.ID_PRODUTO) ")

                        .AppendLine(" WHERE B.STATUS = 6 AND(B.DATA_VENDA BETWEEN @PDATA_VENDA_INI AND @PDATA_VENDA_FIN) ")
                        .AppendLine("   AND A.ID = @ID_FUNCIONARIO ")
                        .AppendLine("   AND E.QUANTIDADE BETWEEN I.FAIXA_INICIAL AND I.FAIXA_FINAL ");

                        parametros.Add("@ID_FUNCIONARIO", Id_Funcionario);
                        parametros.Add("@PDATA_VENDA_INI", Data_Inicio);
                        parametros.Add("@PDATA_VENDA_FIN", Data_Fim);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ComissionamentoExtrato
                                {
                                    Coeficiente = Convert.ToDouble(reader["COEFICIENTE"]),
                                    Pontos = Convert.ToDouble(reader["PONTOS"]),
                                    DataVenda = Convert.ToDateTime(reader["DATA_VENDA"]),
                                    Comissao = Convert.ToDouble(Convert.ToInt64(reader["PONTOS"]) * Convert.ToDouble(reader["COEFICIENTE"]))
                                });
                            }
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        #region PlanoProduto
        [HttpGet()]
        [Route("api/PlanoApi/GetComissionamentoPlanoProduto")]
        public GetApiRetorno<Comissionamento_PlanoProdutoListaModel> GetComissionamentoPlanoProduto(Int64 Id)
        {
            GetApiRetorno<Comissionamento_PlanoProdutoListaModel> retorno = new GetApiRetorno<Comissionamento_PlanoProdutoListaModel>
            {
                Objeto = new Comissionamento_PlanoProdutoListaModel()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Comissionamento_PlanoProduto>(session))
            {
                try
                {
                    var queryPlanos = repository.queryover().Where(p => p.Id == Id).List();

                    if (queryPlanos.Count() > 0)
                    {
                        List<Comissionamento_PlanoProdutoListaModel> planos = queryPlanos.Select(p => new Comissionamento_PlanoProdutoListaModel
                        {
                            Id = p.Id,
                            IdComissionamento = p.Comissionamento.Id,
                            Descricao = p.Plano == null ? p.Produto.Descricao : p.Plano.Descricao,
                            Quantidade = Convert.ToString(p.Quantidade.GetValueOrDefault(0)),
                            Unidade = p.Unidade
                        }).ToList();

                        retorno.Objeto = planos[0];
                    }
                    else
                    {
                        retorno.Objeto = new Comissionamento_PlanoProdutoListaModel();
                    }

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
        [Route("api/ComissionamentoApi/GravarPlanoProduto")]
        public async Task<PostApiRetorno<Int64>> GravarPlanoProduto(Comissionamento_PlanoProdutoListaModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Comissionamento_PlanoProduto modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Comissionamento_PlanoProduto>(session))
                    {
                        if (value.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            throw new Exception("Plano/Produto não encontrado");
                        }
                        else
                        {
                            modelo.Unidade = value.Unidade;
                            modelo.Quantidade = !String.IsNullOrEmpty(value.Quantidade) ? Convert.ToDouble(value.Quantidade) : 0;
                        }

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

        #endregion

        #region EstratoComissaoPorVendedor Mobile Premier
        [HttpGet()]
        [Route("api/ComissionamentoApi/ExtratoComissao")]
        public async Task<GetApiRetorno<List<ComissionamentoExtrato>>> ExtratoComissao(Int64 id_Vendedor, int ano)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                var diaConfig = (await repoConfig.selectFirst()).Dia_Mes_Finaliza_Medicao_Comissao;

                string dataInicio;
                string dataFim;

                if (diaConfig <= 30)
                    dataInicio = (Convert.ToInt32(ano) - 1).ToString() + "-12-" + (Convert.ToInt32(diaConfig) + 1).ToString();
                else
                    dataInicio = ano + "-01-" + "01";

                dataFim = ano + "-12-" + diaConfig;

                List<ComissionamentoExtrato> retornoSql = new List<ComissionamentoExtrato>();
                GetApiRetorno<List<ComissionamentoExtrato>> retorno = new GetApiRetorno<List<ComissionamentoExtrato>>
                {
                    Objeto = new List<ComissionamentoExtrato>()
                };


                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT PONTOS, COEFICIENTE, VALOR_FINAL, NOME_CLIENTE, DATA_VENDA, PRODUTO_PLANO, ID_COMISSIONAMENTO");
                        sql.AppendLine("FROM (SELECT A.NOME AS NOME_FUNCIONARIO,");
                        sql.AppendLine("             B.ID AS ID_SUPERVISOR,");
                        sql.AppendLine("             COALESCE(A.ID_FAIXA, B.ID_FAIXA, C.ID_FAIXA_COMISSIONAMENTO) AS ID_COMISSIONAMENTO,");
                        sql.AppendLine("             O.NOMEFANTASIA AS NOME_CLIENTE,");
                        sql.AppendLine("             E.ID_PLANO,");
                        sql.AppendLine("             E.ID_PRODUTO,");
                        sql.AppendLine("             COALESCE(I.QUANTIDADE,  0) AS PONTOS,");
                        sql.AppendLine("             COALESCE(E.VALOR_FINAL, 0) AS VALOR_FINAL,");
                        sql.AppendLine("             D.DATA_VENDA AS DATA_VENDA,");
                        sql.AppendLine("             COALESCE(L.DESCRICAO, M.DESCRICAO) AS PRODUTO_PLANO,");
                        sql.AppendLine("             COALESCE(( SELECT N.COEFICIENTE");
                        sql.AppendLine("                        FROM TB_FAIXA_COMISSIONAMENTO_ITEM N");
                        sql.AppendLine("                        WHERE N.ID_FAIXA = COALESCE(A.ID_FAIXA, B.ID_FAIXA, C.ID_FAIXA_COMISSIONAMENTO)");
                        sql.AppendLine("                              AND (I.QUANTIDADE BETWEEN N.FAIXA_INICIAL AND N.FAIXA_FINAL)), 0) AS COEFICIENTE");

                        sql.AppendLine("      FROM TB_FUNCIONARIO A");
                        sql.AppendLine("      INNER JOIN TB_FUNCIONARIO B ON(A.ID_SUPERVISOR = B.ID)");
                        sql.AppendLine("      LEFT JOIN TB_FILIAL C ON(A.ID_FILIAL = C.ID)");
                        sql.AppendLine("      INNER JOIN TB_VENDA D ON(A.ID = D.ID_VENDEDOR)");
                        sql.AppendLine("      INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ON(E.ID_VENDA = D.ID)");

                        sql.AppendLine("      LEFT JOIN TB_COMISSIONAMENTO G ON(G.ID_FILIAL = A.ID_FILIAL) -- FILIAL");
                        sql.AppendLine("      LEFT JOIN TB_COMISSIONAMENTO P ON(P.ID = A.ID_COMISSIONAMENTO) -- VENDEDOR");
                        sql.AppendLine("      LEFT JOIN TB_COMISSIONAMENTO Q ON(Q.ID = B.ID_COMISSIONAMENTO) -- SUPERVISOR");

                        sql.AppendLine("      LEFT JOIN TB_COMISSAO_PLANO_PRODUTO I");
                        sql.AppendLine("      ON(I.ID_COMISSIONAMENTO = COALESCE(P.ID, Q.ID, G.ID) AND (I.ID_PLANO = E.ID_PLANO OR I.ID_PRODUTO = E.ID_PRODUTO))");

                        sql.AppendLine("      LEFT JOIN TB_PRODUTO L ON (L.ID = E.ID_PRODUTO)");
                        sql.AppendLine("      LEFT JOIN TB_PLANO M ON (M.ID = E.ID_PLANO)");
                        sql.AppendLine("      INNER JOIN TB_CLIENTE O ON (O.ID = D.ID_CLIENTE)");

                        sql.AppendLine("      WHERE D.STATUS = 6 AND (A.ID = @ID_VENDEDOR) AND (CAST(D.DATA_VENDA AS DATE) BETWEEN @DATA_INICIO AND @DATA_FIM))");
                        sql.AppendLine("GROUP BY COEFICIENTE, PONTOS, NOME_CLIENTE, VALOR_FINAL, DATA_VENDA, PRODUTO_PLANO, ID_COMISSIONAMENTO");
                        parametros.Add("@ID_VENDEDOR", id_Vendedor);
                        parametros.Add("@DATA_INICIO", dataInicio);
                        parametros.Add("@DATA_FIM", dataFim);

                        Int64? idfaixa = 0;
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                idfaixa = String.IsNullOrEmpty(reader["ID_COMISSIONAMENTO"].ToString()) ? (Int64?)null : Convert.ToInt64(reader["ID_COMISSIONAMENTO"]);
                                retornoSql.Add(new ComissionamentoExtrato
                                {
                                    NomeCliente = reader["NOME_CLIENTE"]?.ToString(),
                                    DataVenda = Convert.ToDateTime(reader["DATA_VENDA"]),
                                    PlanoProduto = reader["PRODUTO_PLANO"]?.ToString(),
                                    IdComissionamento = String.IsNullOrEmpty(reader["ID_COMISSIONAMENTO"].ToString()) ? (Int64?)null : Convert.ToInt64(reader["ID_COMISSIONAMENTO"]),
                                    ValorFinal = Convert.ToDouble(reader["VALOR_FINAL"]),
                                    Pontos = Convert.ToDouble(reader["PONTOS"])
                                });
                            }
                            retorno.Ok = true;
                        }

                        List<object> faixas = new List<object>();

                        StringBuilder sqlFaixa = new StringBuilder();
                        Dictionary<String, Object> parametroSqlFaixa = new Dictionary<string, object>();

                        sqlFaixa.AppendLine("SELECT A.ID, A.FAIXA_INICIAL AS FAIXA_INICIAL, A.FAIXA_FINAL AS FAIXA_FINAL, A.COEFICIENTE AS COEFICIENTE");
                        sqlFaixa.AppendLine("FROM TB_FAIXA_COMISSIONAMENTO_ITEM A");
                        sqlFaixa.AppendLine("WHERE A.ID_FAIXA = @ID_FAIXA");
                        parametroSqlFaixa.Add("@ID_FAIXA", idfaixa);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqlFaixa, parametroSqlFaixa, session.Connection, null, 0, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                faixas.Add(new
                                {
                                    id = CommandHelper.convertReturnValue<Int64?>(reader["ID"], null),
                                    faixa_inicial = String.IsNullOrEmpty(reader["FAIXA_INICIAL"].ToString()) ? (Int64?)null : Convert.ToInt64(reader["FAIXA_INICIAL"]),
                                    faixa_final = String.IsNullOrEmpty(reader["FAIXA_FINAL"].ToString()) ? (Int64?)null : Convert.ToInt64(reader["FAIXA_FINAL"]),
                                    coeficiente = CommandHelper.convertReturnValue<Double?>(reader["COEFICIENTE"], null)
                                });
                            }
                        }

                        for (int i = 1; i <= 12; i++)
                        {
                            DateTime inicioPeriodo;
                            DateTime finalPeriodo;

                            var qtdDiasDoMesAtual = DateTime.DaysInMonth(ano, i);
                            if (qtdDiasDoMesAtual >= diaConfig)
                            {
                                finalPeriodo = new DateTime(ano, i, diaConfig ?? 0);
                            }
                            else
                            {
                                finalPeriodo = new DateTime(ano, i, qtdDiasDoMesAtual);
                            }

                            var mesAnt = finalPeriodo.AddMonths(-1);
                            var qtdDiasDoMesAnt = DateTime.DaysInMonth(mesAnt.Year, mesAnt.Month);

                            if (qtdDiasDoMesAnt >= diaConfig + 1)
                            {
                                inicioPeriodo = new DateTime(mesAnt.Year, mesAnt.Month, (diaConfig + 1) ?? 0);
                            }
                            else
                            {
                                inicioPeriodo = new DateTime(ano, i, 1);
                            }

                            finalPeriodo = finalPeriodo.AddHours(23).AddMinutes(59).AddSeconds(59);
                            List<ComissionamentoExtrato> itensMes = retornoSql.Where(w => w.DataVenda >= inicioPeriodo && w.DataVenda <= finalPeriodo).ToList();

                            if (itensMes.Count() > 0)
                            {
                                double somatorioPontosDoPeriodo = itensMes.Sum(s => s.Pontos ?? 0);

                                dynamic faixa = faixas.Where(w => somatorioPontosDoPeriodo >= ((dynamic)w).faixa_inicial && somatorioPontosDoPeriodo <= ((dynamic)w).faixa_final).FirstOrDefault();
                                double coeficiente = 0;
                                if (faixa != null) coeficiente = faixa.coeficiente;

                                retorno.Objeto.AddRange(itensMes.Select(s =>
                                {
                                    s.IdFaixaItem = faixa?.id;
                                    s.Coeficiente = coeficiente;
                                    s.Comissao = s.Coeficiente * s.Pontos;
                                    s.mesDaComissao = i;
                                    return s;
                                }).ToList());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }


                return retorno;
            }
        }

        #endregion
    }
}
