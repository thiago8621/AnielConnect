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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class MetaApiController : ApiController
    {
        [HttpPost()]
        [Route("api/metaApi/GetAll")]
        public async Task<DataTableResponse<MetaListaModel>> GetAll(Meta_DataTableRequest request)
        {
            List<MetaListaModel> retorno = new List<MetaListaModel>();
            IOrderedEnumerable<MetaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.ANO, A.STATUS, A.JANEIRO, A.FEVEREIRO, ");
                    sql.AppendLine("A.MARCO, A.ABRIL, A.MAIO, A.JUNHO, A.JULHO, ");
                    sql.AppendLine("A.AGOSTO, A.SETEMBRO, A.OUTUBRO, A.NOVEMBRO, A.DEZEMBRO ");
                    sql.AppendLine("FROM TB_META A ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        if (sql.ToString().Contains("WHERE"))
                            sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        else
                            sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");

                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        if (sql.ToString().Contains("WHERE"))
                            sql.AppendLine(" AND A.STATUS = @STATUS ");
                        else
                            sql.AppendLine(" WHERE A.STATUS = @STATUS ");

                        parametros.Add("@STATUS", request.Status);
                    }


                    if (!String.IsNullOrEmpty(request.Ano))
                    {
                        if (sql.ToString().Contains("WHERE"))
                            sql.AppendLine(" AND A.ANO = @ANO ");
                        else
                            sql.AppendLine(" WHERE A.ANO = @ANO ");

                        parametros.Add("@ANO", request.Ano);
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new MetaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Ano = Convert.ToString(reader["ANO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Janeiro = Convert.ToString(reader["JANEIRO"]),
                                Fevereiro = Convert.ToString(reader["FEVEREIRO"]),
                                Marco = Convert.ToString(reader["MARCO"]),
                                Abril = Convert.ToString(reader["ABRIL"]),
                                Maio = Convert.ToString(reader["MAIO"]),
                                Junho = Convert.ToString(reader["JUNHO"]),
                                Julho = Convert.ToString(reader["JULHO"]),
                                Agosto = Convert.ToString(reader["AGOSTO"]),
                                Setembro = Convert.ToString(reader["SETEMBRO"]),
                                Outubro = Convert.ToString(reader["OUTUBRO"]),
                                Novembro = Convert.ToString(reader["NOVEMBRO"]),
                                Dezembro = Convert.ToString(reader["DEZEMBRO"])

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
                    case 0:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case 1:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                    case 2:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Ano);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Ano);
                            break;
                        }
                    case 3:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<MetaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private void BindViewToObj(ISession session, ref Meta objBanco, MetaCadastroModel view)
        {
            objBanco.Id = view.Id;

            objBanco.Descricao = view.Descricao;

            using (var repository = new Conexao<Meta>(session))
            {
                var meta = repository.selectFirst(p => p.Id == view.Id).Result;
                if (meta != null)
                {
                    objBanco.Ano = meta.Ano;
                }
                else
                {
                    objBanco.Ano = view.Ano;
                }
            }

            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status;
            }

            objBanco.Janeiro = view.Janeiro;
            objBanco.Fevereiro = view.Fevereiro;
            objBanco.Marco = view.Marco;
            objBanco.Abril = view.Abril;
            objBanco.Maio = view.Maio;
            objBanco.Junho = view.Junho;
            objBanco.Julho = view.Julho;
            objBanco.Agosto = view.Agosto;
            objBanco.Setembro = view.Setembro;
            objBanco.Outubro = view.Outubro;
            objBanco.Novembro = view.Novembro;
            objBanco.Dezembro = view.Dezembro;
        }

        private MetaCadastroModel GetViewObj(Meta obj)
        {
            return new MetaCadastroModel
            {
                Id = obj.Id,
                Ano = obj.Ano,
                Descricao = obj.Descricao,

                Status = obj.Status,

                Janeiro = obj.Janeiro,
                Fevereiro = obj.Fevereiro,
                Marco = obj.Marco,
                Abril = obj.Abril,
                Maio = obj.Maio,
                Junho = obj.Junho,
                Julho = obj.Julho,
                Agosto = obj.Agosto,
                Setembro = obj.Setembro,
                Outubro = obj.Outubro,
                Novembro = obj.Novembro,
                Dezembro = obj.Dezembro,
            };

        }

        [HttpPost()]
        [Route("api/MetaApi/excluirlista")]
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

                        StringBuilder sql2 = new StringBuilder($"UPDATE TB_FILIAL SET ID_META = NULL WHERE ID_META IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql2, new Dictionary<String, Object>(), session.Connection, null);

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_META WHERE ID IN {_ids}");

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
        [Route("api/MetaApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_META SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Metaapi/FindById")]
        public async Task<GetApiRetorno<MetaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<MetaCadastroModel> retorno = new GetApiRetorno<MetaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoMeta = new Conexao<Meta>(session))
            using (var repoFilial = new Conexao<Filial>(session))
            {
                try
                {
                    var meta = await repoMeta.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(meta);

                    var filialAtual = await repoFilial.selectFirst(p => p.Meta == meta);
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
        [Route("api/Metaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<MetaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Meta modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repositoryFilial = new Conexao<Filial>(session))
                    using (var repository = new Conexao<Meta>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        //var modelo_ano = (await repository.select(p => p.Ano == value.Objeto.Ano)).FirstOrDefault();

                        //if(modelo_ano == null)
                        //{ 

                        if (modelo == null)
                        {
                            modelo = new Meta
                            {
                                Status = true,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        var filialAnterior = await repositoryFilial.selectFirst(p => p.Meta == modelo);

                        if (filialAnterior != null && filialAnterior?.Id != value.Objeto.Id_Filial.Value)
                        {
                            filialAnterior.Meta = null;
                            await repositoryFilial.SaveOrUpdate(filialAnterior);
                        }

                        var filialAtual = await repositoryFilial.selectFirst(p => p.Id == value.Objeto.Id_Filial.Value);
                        filialAtual.Meta = modelo;
                        await repositoryFilial.SaveOrUpdate(filialAtual);

                        retorno.IdObjeto = modelo.Id;
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
        [Route("api/Metaapi/busca")]
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_META A");
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

        [HttpGet()]
        [Route("api/Metaapi/Excluir")]
        public async Task<DeleteApiRetorno<Int64>> Excluir(Int64 Id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Meta>(session))
                    {
                        Meta modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();

                        await repository.Remove(modelo);

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
        public async Task<GetApiRetorno<IEnumerable<MetaListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<MetaListaModel>> retorno = new GetApiRetorno<IEnumerable<MetaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Meta>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new MetaListaModel
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

        [HttpGet()]
        [Route("api/Metaapi/exibirMetaPeriodo")]
        public async Task<GetApiRetorno<List<MetaCampanha>>> exibirMetaPeriodoBySupervisor(Int64 id_Supervisor, String data_Ini, String data_Fim)
        {
            GetApiRetorno<List<MetaCampanha>> retorno = new GetApiRetorno<List<MetaCampanha>>
            {
                Objeto = new List<MetaCampanha>()
            };
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

                        sql.AppendLine(" SELECT EXTRACT(MONTH FROM A.DATA_VENDA) AS MES, CASE (EXTRACT(MONTH FROM A.DATA_VENDA)) ");
                        sql.AppendLine(" WHEN 1 THEN B.JANEIRO ");
                        sql.AppendLine(" WHEN 2 THEN B.FEVEREIRO ");
                        sql.AppendLine(" WHEN 3 THEN B.MARCO ");
                        sql.AppendLine(" WHEN 4 THEN B.ABRIL ");
                        sql.AppendLine(" WHEN 5 THEN B.MAIO ");
                        sql.AppendLine(" WHEN 6 THEN B.JUNHO ");
                        sql.AppendLine(" WHEN 7 THEN B.JULHO ");
                        sql.AppendLine(" WHEN 8 THEN B.AGOSTO ");
                        sql.AppendLine(" WHEN 9 THEN B.SETEMBRO ");
                        sql.AppendLine(" WHEN 10 THEN B.OUTUBRO ");
                        sql.AppendLine(" WHEN 11 THEN B.NOVEMBRO ");
                        sql.AppendLine(" ELSE B.DEZEMBRO ");
                        sql.AppendLine(" END AS META, COUNT(A.ID) AS VENDAS ");
                        sql.AppendLine(" FROM TB_VENDA A ");

                        sql.AppendLine(" LEFT OUTER JOIN TB_FUNCIONARIO C ");
                        sql.AppendLine(" ON(A.ID_VENDEDOR = C.ID) ");

                        sql.AppendLine(" LEFT OUTER JOIN TB_FUNCIONARIO D ");
                        sql.AppendLine(" ON(D.id = C.ID_SUPERVISOR) ");

                        sql.AppendLine(" LEFT OUTER JOIN TB_META B ");
                        sql.AppendLine(" ON(C.ID_META = B.ID) ");

                        sql.AppendLine(" WHERE A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM ");
                        sql.AppendLine(" AND D.ID = @ID ");
                        sql.AppendLine(" AND A.STATUS = 6 ");

                        sql.AppendLine(" GROUP BY EXTRACT(MONTH FROM A.DATA_VENDA), META ");

                        parametros.Add("@ID", id_Supervisor);
                        parametros.Add("@DATAINI", Convert.ToDateTime(data_Ini).ToString("dd/MM/yyyy"));
                        parametros.Add("@DATAFIM", Convert.ToDateTime(data_Fim).ToString("dd/MM/yyyy"));

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new MetaCampanha
                                {
                                    Mes = Convert.ToInt32(reader["MES"]),
                                    Periodo = "",
                                    Valor_meta = Convert.ToInt64(reader["META"]),
                                    Valor_alcancado = Convert.ToInt32(reader["VENDAS"])
                                });

                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        public async Task<GetApiRetorno<Int32>> GetMetaFuncionario(Int64 Id_Funcionario)
        {
            GetApiRetorno<Int32> retorno = new GetApiRetorno<int>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        String _mes = DateTime.Now.ToString("MMMM").ToUpperInvariant();

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine($@" SELECT COALESCE(C.{_mes}, D.{_mes}) AS META_MES ");

                        sql.AppendLine(" FROM TB_FUNCIONARIO A ");

                        sql.AppendLine(" INNER JOIN TB_FUNCIONARIO B ");
                        sql.AppendLine(" ON(B.E_SUPERVISOR = 1 AND  A.ID_SUPERVISOR = B.ID) ");

                        sql.AppendLine(" LEFT JOIN TB_META C ");
                        sql.AppendLine(" ON(C.ID = A.ID_META) ");

                        sql.AppendLine(" LEFT JOIN TB_META D ");
                        sql.AppendLine(" ON(D.ID = B.ID_META) ");

                        sql.AppendLine(" WHERE A.ID = @ID_FUNC ");

                        parametros.Add("@ID_FUNC", Id_Funcionario);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = Convert.ToInt32(reader["META_MES"]);
                                retorno.Ok = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        #region MOBILE exibirMetaCampanha
        [HttpGet()]
        [Route("api/Metaapi/exibirMetaCampanha")]
        public async Task<GetApiRetorno<List<MetaCampanha>>> exibirMetaCampanha(Int64 id_Vendedor, String e_Supervisor)
        {
            GetApiRetorno<List<MetaCampanha>> retorno = new GetApiRetorno<List<MetaCampanha>>
            {
                Objeto = new List<MetaCampanha>()
            };
            try
            {
                retorno.Objeto = await getMetaVendaCampanhaVendedor(id_Vendedor, e_Supervisor);
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task<List<MetaCampanha>> getMetaVendaCampanhaVendedor(Int64 id_Vendedor, String e_Supervisor)
        {
            List<MetaCampanha> retorno = new List<MetaCampanha>();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Funcionario func = await getFuncionario(id_Vendedor);

                        sql.AppendLine("SELECT A.ID, sum(A.META) as META, A.TITULO, A.DATA_INICIO, A.DATA_FINAL ");
                        sql.AppendLine("FROM TB_CAMPANHA  A ");
                        if (e_Supervisor == "S")
                        {
                            sql.AppendLine("INNER JOIN TB_FUNCIONARIO B ON (B.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        sql.AppendLine("WHERE ");
                        if (e_Supervisor == "N")
                        {
                            sql.AppendLine("(A.ID_FUNCIONARIO = @ID_VENDEDOR ");
                            sql.AppendLine("OR(A.ID_FUNCIONARIO IS null AND A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        else
                        {
                            sql.AppendLine("(A.ID_FUNCIONARIO = B.ID ");
                            sql.AppendLine("OR(A.ID_FUNCIONARIO IS null AND A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        sql.AppendLine("OR(A.ID_FUNCIONARIO IS NULL AND A.ID_SUPERVISOR IS NULL AND A.ID_FILIAL = @ID_FILIAL)) ");
                        sql.AppendLine("AND A.STATUS = 1  AND A.DATA_INICIO <= current_date AND A.DATA_FINAL >= current_date GROUP BY A.ID, A.META, A.TITULO, A.DATA_INICIO, A.DATA_FINAL ");
                        if (e_Supervisor == "N")
                        {
                            parametros.Add("@ID_VENDEDOR", func.Id);
                            parametros.Add("@ID_SUPERVISOR", func.Supervisor.Id);
                        }
                        else
                        {
                            parametros.Add("@ID_SUPERVISOR", func.Id);
                        }
                        parametros.Add("@ID_FILIAL", func.Filial.Id);


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                Int64 idCampanha = Convert.ToInt64(reader["ID"]);
                                MetaCampanha metaCampanha = new MetaCampanha();
                                metaCampanha.Valor_meta = reader["META"] != DBNull.Value ? Convert.ToInt64(reader["META"]) : 0;
                                metaCampanha.Periodo = Convert.ToString(reader["TITULO"]);
                                metaCampanha.Data_inicio = Convert.ToDateTime(reader["DATA_INICIO"]);
                                metaCampanha.Data_final = Convert.ToDateTime(reader["DATA_FINAL"]);

                                StringBuilder sql2 = new StringBuilder();
                                Dictionary<String, Object> parametros2 = new Dictionary<string, object>();

                                sql2.AppendLine("SELECT SUM(PONTOS) AS PONTOS FROM (SELECT SUM(E.QUANTIDADE) AS PONTOS ");
                                sql2.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO A ");
                                sql2.AppendLine("LEFT JOIN TB_VENDA V ON(A.ID_VENDA = V.ID) ");
                                sql2.AppendLine("LEFT JOIN TB_FUNCIONARIO F ON(V.ID_VENDEDOR = F.ID) ");
                                sql2.AppendLine("LEFT JOIN TB_CLIENTE B ON(A.ID_CLIENTE = B.ID) ");
                                sql2.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E ON((E.ID_PLANO = A.ID_PLANO or E.ID_PRODUTO = A.ID_PRODUTO) ");
                                sql2.AppendLine("AND V.ID_COMISSIONAMENTO = E.ID_COMISSIONAMENTO) ");
                                sql2.AppendLine("LEFT JOIN TB_CAMPANHA C ON(A.ID_CAMPANHA = C.ID) ");
                                sql2.AppendLine("WHERE(V.STATUS = 6 ");
                                if (e_Supervisor == "N")
                                {
                                    sql2.AppendLine(" AND V.ID_VENDEDOR = @ID_VENDEDOR ");
                                }
                                else
                                {
                                    sql2.AppendLine("AND V.ID_SUPERVISOR = @ID_VENDEDOR ");
                                }
                                sql2.AppendLine("AND C.ID = @ID_CAMPANHA ) ");
                                sql2.AppendLine("GROUP BY B.CPF, B.NOMEFANTASIA, V.DATA_VENDA, C.TITULO,F.NOME ");
                                sql2.AppendLine("ORDER BY V.DATA_VENDA ) ");
                                parametros2.Add("@ID_VENDEDOR", id_Vendedor);
                                parametros2.Add("@ID_CAMPANHA", idCampanha);

                                using (var com2 = session.Connection.CreateCommand())
                                {
                                    var reader2 = await CommandHelper.ExecuteReader(com2, sql2, parametros2, session.Connection, null);
                                    while (reader2.Read())
                                    {
                                        metaCampanha.Valor_alcancado = reader2["PONTOS"] != DBNull.Value ? Convert.ToInt32(reader2["PONTOS"]) : 0;
                                    }
                                }
                                retorno.Add(metaCampanha);

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return retorno;
        }
        #endregion

        #region MOBILE exibirMetaPeriodo
        [HttpGet()]
        [Route("api/Metaapi/exibirMetaPeriodo")]
        public async Task<GetApiRetorno<List<MetaCampanha>>> exibirMetaPeriodo(Int64 id_Vendedor, String data_Ini, String data_Fim)
        {
            GetApiRetorno<List<MetaCampanha>> retorno = new GetApiRetorno<List<MetaCampanha>>
            {
                Objeto = new List<MetaCampanha>()
            };
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
                        sql.AppendLine("SELECT EXTRACT(MONTH FROM A.DATA_VENDA) AS MES, CASE (EXTRACT(MONTH FROM A.DATA_VENDA)) ");
                        sql.AppendLine("WHEN 1 THEN B.JANEIRO ");
                        sql.AppendLine("WHEN 2 THEN B.FEVEREIRO ");
                        sql.AppendLine("WHEN 3 THEN B.MARCO ");
                        sql.AppendLine("WHEN 4 THEN B.ABRIL ");
                        sql.AppendLine("WHEN 5 THEN B.MAIO ");
                        sql.AppendLine("WHEN 6 THEN B.JUNHO ");
                        sql.AppendLine("WHEN 7 THEN B.JULHO ");
                        sql.AppendLine("WHEN 8 THEN B.AGOSTO ");
                        sql.AppendLine("WHEN 9 THEN B.SETEMBRO ");
                        sql.AppendLine("WHEN 10 THEN B.OUTUBRO ");
                        sql.AppendLine("WHEN 11 THEN B.NOVEMBRO ");
                        sql.AppendLine("ELSE B.DEZEMBRO ");
                        sql.AppendLine("END AS META, COUNT(A.ID) AS VENDAS ");
                        sql.AppendLine("FROM TB_VENDA A LEFT OUTER JOIN TB_FUNCIONARIO C ON(A.ID_VENDEDOR = C.ID) ");
                        sql.AppendLine("LEFT OUTER JOIN TB_META B ON (C.ID_META = B.ID) ");
                        sql.AppendLine("WHERE A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM AND C.ID = @ID AND A.STATUS = 6 ");
                        sql.AppendLine("GROUP BY EXTRACT(MONTH FROM A.DATA_VENDA), META ");
                        parametros.Add("@ID", id_Vendedor);
                        parametros.Add("@DATAINI", Convert.ToDateTime(data_Ini).ToString("dd/MM/yyyy"));
                        parametros.Add("@DATAFIM", Convert.ToDateTime(data_Fim).ToString("dd/MM/yyyy 23:59:00"));

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new MetaCampanha
                                {
                                    Mes = Convert.ToInt32(reader["MES"]),
                                    Periodo = "",
                                    Valor_meta = Convert.ToInt64(reader["META"]),
                                    Valor_alcancado = Convert.ToInt32(reader["VENDAS"])
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        
        [HttpGet()]
        [Route("api/Metaapi/exibirMetaMes")]
        public async Task<GetApiRetorno<List<MetaCampanha>>> exibirMetaMes(Int64 id_Vendedor, String e_Supervisor, String data_Ini, String data_Fim)
        {
            GetApiRetorno<List<MetaCampanha>> retorno = new GetApiRetorno<List<MetaCampanha>>
            {
                Objeto = new List<MetaCampanha>()
            };

            using (var configApi = new Configuracoes_GeraisApiController())
            {
                var diaConfig = configApi.FindById(1).Result.Objeto.Dia_Mes_Finaliza_Medicao_Comissao;
                if (diaConfig == null)
                {
                    throw new Exception("É necessário configurar o dia do mês em que finaliza a medição para comissão.");
                }

                try
                {
                    List<MetaPeriodo> listPeriodos = getPeriodos(diaConfig.Value);
                    if (e_Supervisor == "S")
                    {
                        List<Int64> vendedores = await getVendedores(id_Vendedor);
                        if (vendedores != null)
                        {
                            for (int i = 0; i < listPeriodos.Count; i++)
                            {
                                Int64 totalMetas = new Int64();
                                Int32? totalVendas = new Int32();
                                MetaCampanha metaCampanha = new MetaCampanha();
                                for (int v = 0; v < vendedores.Count; v++)
                                {
                                    Task<Int64?> idMeta = getMeta(vendedores[v], Convert.ToInt16(listPeriodos[i].Periodo), Convert.ToInt16(listPeriodos[i].Ano));
                                    if (idMeta.Result == null)
                                    {
                                        throw new Exception("Sem meta vinculada");
                                    }
                                    MetaCampanha m = await getMetaVendaVendedor(vendedores[v], listPeriodos[i].Primeiro_Dia, listPeriodos[i].Ultimo_Dia, listPeriodos[i].Periodo, idMeta.Result, e_Supervisor, id_Vendedor);
                                    totalMetas += m.Valor_meta;
                                    totalVendas += m.Valor_alcancado;
                                }
                                metaCampanha.Valor_alcancado = totalVendas;
                                metaCampanha.Valor_meta = totalMetas;
                                metaCampanha.Mes = listPeriodos[i].Periodo;
                                metaCampanha.Periodo = "";

                                retorno.Objeto.Add(metaCampanha);
                            }
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception("Supervisor sem vendedores vinculados");
                        }
                    }
                    else
                    {
                        for (int i = 0; i < listPeriodos.Count; i++)
                        {
                            Task<Int64?> meta = getMeta(id_Vendedor, Convert.ToInt16(listPeriodos[i].Periodo), Convert.ToInt16(listPeriodos[i].Ano));
                            if (meta.Result == null)
                            {
                                throw new Exception("Vendedor sem meta cadastrada");
                            }
                            MetaCampanha metaCampanha = await getMetaVendaVendedor(id_Vendedor, listPeriodos[i].Primeiro_Dia, listPeriodos[i].Ultimo_Dia, listPeriodos[i].Periodo, meta.Result, e_Supervisor, null);
                            retorno.Objeto.Add(metaCampanha);
                        }
                        retorno.Ok = true;
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

        public async Task<MetaCampanha> getMetaVendaVendedor(Int64 id_Vendedor, DateTime dataIni, DateTime dataFim, Int32 Periodo, Int64? meta, String e_Supervisor, Int64? id_Supervisor)
        {
            MetaCampanha retorno = new MetaCampanha();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT SUM(PONTOS) AS PONTOS FROM (SELECT SUM(E.QUANTIDADE) AS PONTOS ");
                        sql.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO A ");
                        sql.AppendLine("LEFT JOIN TB_VENDA V ON(A.ID_VENDA = V.ID) ");
                        sql.AppendLine("LEFT JOIN TB_FUNCIONARIO F ON(V.ID_VENDEDOR = F.ID) ");
                        sql.AppendLine("LEFT JOIN TB_CLIENTE B ON(A.ID_CLIENTE = B.ID) ");
                        sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E ON((E.ID_PLANO = A.ID_PLANO or E.ID_PRODUTO = A.ID_PRODUTO) ");
                        sql.AppendLine("AND V.ID_COMISSIONAMENTO = E.ID_COMISSIONAMENTO) ");
                        sql.AppendLine("WHERE(V.STATUS = 6 ");
                        if (e_Supervisor == "S")
                        {
                            sql.AppendLine("AND V.ID_VENDEDOR = @ID_VENDEDOR AND V.ID_SUPERVISOR = @ID_SUPERVISOR AND ");
                        }
                        else
                        {
                            sql.AppendLine("AND V.ID_VENDEDOR = @ID_VENDEDOR AND ");
                        }
                        sql.AppendLine("V.DATA_VENDA >= @DATA_INICIO AND V.DATA_VENDA <= @DATA_FIM) ");
                        sql.AppendLine("GROUP BY B.CPF, B.NOMEFANTASIA, V.DATA_VENDA, F.NOME ");
                        sql.AppendLine("ORDER BY V.DATA_VENDA ) ");
                        parametros.Add("@ID_VENDEDOR", id_Vendedor);
                        parametros.Add("@ID_SUPERVISOR", id_Supervisor);
                        parametros.Add("@DATA_INICIO", Convert.ToDateTime(dataIni).ToString("dd/MM/yyyy"));
                        parametros.Add("@DATA_FIM", Convert.ToDateTime(dataFim).ToString("dd/MM/yyyy 23:59:00"));

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                retorno.Valor_alcancado = reader["PONTOS"] != DBNull.Value ? Convert.ToInt32(reader["PONTOS"]) : 0;
                            }
                            retorno.Mes = Periodo;
                            retorno.Periodo = "";
                            retorno.Valor_meta = (Int64)meta;
                        }
                        parametros.Clear();

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return retorno;
        }

        public async Task<Int64?> getMeta(Int64 id_Vendedor, Int16 mes, Int16 ano)
        {
            Int64? retorno = null;
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT (SELECT ");
                        sql.AppendLine("CASE(@MES) ");
                        sql.AppendLine("WHEN 1 THEN B.JANEIRO ");
                        sql.AppendLine("WHEN 2 THEN B.FEVEREIRO ");
                        sql.AppendLine("WHEN 3 THEN B.MARCO ");
                        sql.AppendLine("WHEN 4 THEN B.ABRIL ");
                        sql.AppendLine("WHEN 5 THEN B.MAIO ");
                        sql.AppendLine("WHEN 6 THEN B.JUNHO ");
                        sql.AppendLine("WHEN 7 THEN B.JULHO ");
                        sql.AppendLine("WHEN 8 THEN B.AGOSTO ");
                        sql.AppendLine("WHEN 9 THEN B.SETEMBRO ");
                        sql.AppendLine("WHEN 10 THEN B.OUTUBRO ");
                        sql.AppendLine("WHEN 11 THEN B.NOVEMBRO ");
                        sql.AppendLine("ELSE B.DEZEMBRO ");
                        sql.AppendLine("END ");
                        sql.AppendLine("FROM TB_META B ");
                        sql.AppendLine("WHERE B.ID = M.ID AND B.ANO = @ANO ) AS META ");
                        sql.AppendLine("FROM TB_META M ");
                        sql.AppendLine("WHERE EXISTS(SELECT * ");
                        sql.AppendLine("FROM TB_FUNCIONARIO V ");
                        sql.AppendLine("LEFT OUTER JOIN TB_FUNCIONARIO S ON(V.ID_SUPERVISOR = S.ID) ");
                        sql.AppendLine("WHERE V.ID = @ID_VENDEDOR AND(M.ID = COALESCE(V.ID_META, S.ID_META, (");
                        sql.AppendLine("SELECT ID FROM TB_META  WHERE S.ID_FILIAL = V.ID_FILIAL)))) ");
                        parametros.Add("@ID_VENDEDOR", id_Vendedor);
                        parametros.Add("@MES", mes);
                        parametros.Add("@ANO", ano);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                retorno = reader["META"] != DBNull.Value ? Convert.ToInt64(reader["META"]) : (Int64?)null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return retorno;
        }

        public async Task<List<Int64>> getVendedores(Int64 id_Supervisor)
        {
            List<Int64> retorno = new List<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ID FROM TB_FUNCIONARIO WHERE ID_SUPERVISOR = @ID_SUPERVISOR ");
                        parametros.Add("@ID_SUPERVISOR", id_Supervisor);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                retorno.Add(Convert.ToInt64(reader["ID"]));
                            }
                        }
                    }
                }
                catch
                {
                    retorno = null;
                }
            }
            return retorno;
        }

        public async Task<Funcionario> getFuncionario(Int64 id_Vendedor)
        {
            Funcionario retorno = new Funcionario();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ID, ID_SUPERVISOR, ID_FILIAL, E_SUPERVISOR FROM TB_FUNCIONARIO WHERE ID = @ID_VENDEDOR ");
                        parametros.Add("@ID_VENDEDOR", id_Vendedor);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                retorno.Id = Convert.ToInt64(reader["ID"]);
                                retorno.Supervisor = new Funcionario();
                                if (reader["ID_SUPERVISOR"] == DBNull.Value && Convert.ToInt64(reader["E_SUPERVISOR"]) == 0)
                                {
                                    throw new Exception("Funcionário sem supervisor vinculado");
                                }
                                if (reader["ID_SUPERVISOR"] != DBNull.Value)
                                {
                                    retorno.Supervisor.Id = Convert.ToInt64(reader["ID_SUPERVISOR"]);
                                }
                                retorno.Filial = new Filial();
                                if (reader["ID_FILIAL"] == DBNull.Value)
                                {
                                    throw new Exception("Funcionário sem filial vinculada");
                                }
                                else
                                {
                                    retorno.Filial.Id = Convert.ToInt64(reader["ID_FILIAL"]);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return retorno;
        }

        public List<MetaPeriodo> getPeriodos(Int32 diaConfig)
        {
            List<MetaPeriodo> retorno = new List<MetaPeriodo>();
            {
                try
                {
                    DateTime hoje = DateTime.Now;
                    MetaPeriodo periodoAnterior = new MetaPeriodo();
                    MetaPeriodo periodoAtual = new MetaPeriodo();
                    if (hoje.Day > diaConfig)
                    {
                        periodoAtual.Primeiro_Dia = new DateTime(hoje.Year, hoje.Month, diaConfig);
                        periodoAtual.Ultimo_Dia = new DateTime(hoje.Year, hoje.Month, diaConfig).AddMonths(1).AddDays(-1);
                        periodoAnterior.Primeiro_Dia = periodoAtual.Primeiro_Dia.AddMonths(-1);
                        periodoAnterior.Ultimo_Dia = periodoAtual.Primeiro_Dia.AddDays(-1);
                        if (hoje.Month == 12)
                        {
                            periodoAtual.Ano = hoje.Year + 1;
                            periodoAtual.Periodo = 1;
                            periodoAnterior.Ano = hoje.Year;
                            periodoAnterior.Periodo = 12;
                        }
                        else
                        {
                            periodoAtual.Ano = hoje.Year;
                            periodoAtual.Periodo = hoje.Month + 1;
                            periodoAnterior.Ano = hoje.Year;
                            periodoAnterior.Periodo = hoje.Month;
                        }
                    }
                    else
                    {
                        if (diaConfig > DateTime.DaysInMonth(hoje.Year, hoje.Month))
                        {

                            DateTime dataPrevia = new DateTime(hoje.Year, hoje.Month, DateTime.DaysInMonth(hoje.Year, hoje.Month)).AddMonths(-1);

                            if (DateTime.DaysInMonth(dataPrevia.Year, dataPrevia.Month) >= diaConfig)
                            {
                                periodoAtual.Primeiro_Dia = new DateTime(dataPrevia.Year, dataPrevia.Month, diaConfig).AddDays(1);
                            }
                            else
                            {
                                periodoAtual.Primeiro_Dia = dataPrevia.AddDays(1);
                            }

                            periodoAtual.Ultimo_Dia = new DateTime(hoje.Year, hoje.Month, DateTime.DaysInMonth(hoje.Year, hoje.Month));
                        }
                        else
                        {
                            periodoAtual.Primeiro_Dia = new DateTime(hoje.Year, hoje.Month, diaConfig).AddMonths(-1).AddDays(1);
                            periodoAtual.Ultimo_Dia = new DateTime(hoje.Year, hoje.Month, diaConfig);
                        }

                        periodoAnterior.Primeiro_Dia = periodoAtual.Primeiro_Dia.AddMonths(-1);
                        periodoAnterior.Ultimo_Dia = periodoAtual.Primeiro_Dia.AddDays(-1);
                        if (hoje.Month == 1)
                        {
                            periodoAtual.Ano = hoje.Year;
                            periodoAtual.Periodo = 1;
                            periodoAnterior.Ano = hoje.Year - 1;
                            periodoAnterior.Periodo = 12;
                        }
                        else
                        {
                            periodoAtual.Ano = hoje.Year;
                            periodoAtual.Periodo = hoje.Month;
                            periodoAnterior.Ano = hoje.Year;
                            periodoAnterior.Periodo = hoje.Month - 1;
                        }
                    }
                    retorno.Add(periodoAnterior);
                    retorno.Add(periodoAtual);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return retorno;
        }
        #endregion 
    }
}
