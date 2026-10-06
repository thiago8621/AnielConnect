using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.CondominioPredio;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRede;
using Microsoft.AspNet.Identity;
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
    public class Condominio_PredioApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/CondominioPredioapi/GetAll")]
        public async Task<DataTableResponse<Condominio_PredioListaModel>> GetAll(Condominio_Predio_DataTableRequest request)
        {
            List<Condominio_PredioListaModel> retorno = new List<Condominio_PredioListaModel>();
            IOrderedEnumerable<Condominio_PredioListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CONDOMINIO_PREDIO");

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


                    sql.AppendLine("SELECT DISTINCT A.ID, A.DESCRICAO, A.NUMERO, A.STATUS, A.CEP, COALESCE(A.LOGRADOURO, B.DESCRICAO) AS ENDERECO_LOGRADOURO, B.TIPO,")
                       .AppendLine(" COALESCE(A.ID_PAIS, B.ID_PAIS) AS ID_PAIS, COALESCE((SELECT NOME FROM TB_PAIS PAIS WHERE PAIS.ID = A.ID_PAIS), C.NOME) AS PAIS,")
                       .AppendLine(" COALESCE(A.UF, D.SIGLA) AS UF, COALESCE(A.BAIRRO, F.DESCRICAO) AS BAIRRO,")
                       .AppendLine(" COALESCE((SELECT CID.DESCRICAO FROM TB_CIDADE CID WHERE CID.ID = A.ID_CIDADE), E.DESCRICAO )")
                       .AppendLine(" AS CIDADE")



                       .AppendLine("FROM TB_CONDOMINIO_PREDIO A")

                       .AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO B")
                       .AppendLine("ON A.ID_ENDERECO_LOGRADOURO = B.ID")

                       .AppendLine("LEFT JOIN TB_PAIS C")
                       .AppendLine("ON B.ID_PAIS = C.ID")

                       .AppendLine("LEFT JOIN TB_UF D")
                       .AppendLine("ON B.ID_UF = D.ID")

                       .AppendLine("LEFT JOIN TB_CIDADE E")
                       .AppendLine("ON B.ID_CIDADE = E.ID")

                       .AppendLine("LEFT JOIN TB_BAIRRO F")
                       .AppendLine("ON B.ID_BAIRRO = F.ID")

                       .AppendLine("WHERE A.ID > 0");



                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (COALESCE(A.UF, D.SIGLA) = @SIGLA)");
                        parametros.Add("@SIGLA", Convert.ToString(request.UF));
                    }

                    if (!String.IsNullOrEmpty(request.Pais))
                    {
                        sql.AppendLine("AND (B.ID_PAIS = @ID_PAIS)");
                        parametros.Add("@ID_PAIS", Convert.ToInt32(request.Pais));
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sql.AppendLine("AND (E.DESCRICAO = @CIDADE)");
                        parametros.Add("@CIDADE", Convert.ToString(request.Cidade));
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sql.AppendLine("AND (COALESCE(A.BAIRRO, F.DESCRICAO) LIKE @BAIRRO)");
                        parametros.Add("@BAIRRO", String.Format("%{0}%", request.Bairro));
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Endereco_Logradouro))
                    {
                        sql.AppendLine("AND (B.TIPO = @TIPO_ENDERECO_LOGRADOURO)");
                        parametros.Add("@TIPO_ENDERECO_LOGRADOURO", Convert.ToString(request.Tipo_Endereco_Logradouro));
                    }

                    if (!String.IsNullOrEmpty(request.Endereco_Logradouro))
                    {
                        sql.AppendLine("AND (B.ID = @ENDERECO_LOGRADOURO)");
                        parametros.Add("@ENDERECO_LOGRADOURO", Convert.ToString(request.Endereco_Logradouro));
                    }

                    if (!String.IsNullOrEmpty(request.Endereco_Logradouro_Desc))
                    {
                        sql.AppendLine("AND (COALESCE(A.LOGRADOURO, B.DESCRICAO) LIKE @ENDERECO_LOGRADOURO_DES)");
                        parametros.Add("@ENDERECO_LOGRADOURO_DES", String.Format("%{0}%", request.Endereco_Logradouro_Desc));
                    }

                    if (!String.IsNullOrEmpty(request.Cep))
                    {
                        sql.AppendLine("AND (A.CEP LIKE @CEP) ");
                        parametros.Add("@CEP", String.Format("%{0}%", request.Cep));
                    }

                    if (!String.IsNullOrEmpty(request.Status_Filtro))
                    {
                        sql.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Condominio_PredioListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Cep = Convert.ToString(reader["CEP"]),
                                Numero = Convert.ToString(reader["NUMERO"]),
                                Endereco_Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]),
                                Tipo_Endereco_Logradouro = HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Uf = Convert.ToString(reader["UF"]),
                                Pais = Convert.ToString(reader["PAIS"]),
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
                    case 1:     // Id
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }

                    case 3:     // Cep
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cep);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cep);
                            break;
                        }

                    case 4:     // Tipo
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Endereco_Logradouro);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Endereco_Logradouro);
                            break;
                        }

                    case 5:     // Logradouro
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Endereco_Logradouro);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Endereco_Logradouro);
                            break;
                        }

                    case 6:     // Numero
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }

                    case 7:     // Bairro
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Bairro);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Bairro);
                            break;
                        }

                    case 8:     // Cidade
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cidade);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cidade);
                            break;
                        }

                    case 9:     // UF
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Uf);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Uf);
                            break;
                        }

                    case 10:     // Pais
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Pais);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Pais);
                            break;
                        }

                    case 11:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                }
            }
            var lista = retorno;
            if (listaOrdenada != null)
            {
                lista = listaOrdenada.ToList();
            }
            //Page e retornando DataTable
            return new DataTableResponse<Condominio_PredioListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = lista,
                error = ""
            };

        }



        [HttpPost()]
        [Route("api/CondominioPredioapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CONDOMINIO_PREDIO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/CondominioPredioapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_CONDOMINIO_PREDIO WHERE ID IN {_ids}");

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
        [Route("api/CondominioPredioapi/FindById")]
        public GetApiRetorno<Condominio_PredioCadastroModel> FindById(Int64 Id)
        {
            GetApiRetorno<Condominio_PredioCadastroModel> retorno = new GetApiRetorno<Condominio_PredioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Condominio_Predio>(session))
            {
                try
                {
                    ICondominioPredioRepository repositorio = new CondominioPredioRepository(session);
                    IElementoRedeAnielRepository repositorioRede = new ElementoRedeAnielRepository(sessionT5);
                    var business = new CondominioPredioBusiness(repositorio, repositorioRede);
                    retorno.Objeto = business.GetViewObj(Id);
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
        [Route("api/CondominioPredioapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Condominio_PredioCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    ICondominioPredioRepository repositorio = new CondominioPredioRepository(session);
                    IElementoRedeAnielRepository repositorioElement = new ElementoRedeAnielRepository(sessionT5);
                    var business = new CondominioPredioBusiness(repositorio, repositorioElement);

                    retorno = await business.Gravar(value, User.Identity.GetUserId());
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
        [Route("api/CondominioPredioapi/busca")]
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
                        sql.AppendLine("FROM TB_CONDOMINIO_PREDIO A");
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

        [Route("api/CondominioPredioapi/ExcluirApartamentoGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Condominio_PredioBloco>(session))
                    using (var repositoryApartamento = new Conexao<Condominio_PredioApartamento>(session))                    
                    {                        
                        Condominio_PredioBloco bloco = null;
                        if (value != 0)
                        {
                            var apartamento = (await repositoryApartamento.selectFirst(p => p.Id == value));
                            var apartamentosinBloco = (await repositoryApartamento.select(p => p.Bloco == apartamento.Bloco)).ToList();
                            
                            if (apartamentosinBloco.Count >= 1)
                            {
                                bloco = apartamento?.Bloco;                                
                                await repositoryApartamento.Remove(apartamento);
                                if (bloco != null && !apartamentosinBloco.Where(p => p.Id != apartamento.Id).Any(p => p.Bloco == bloco))
                                {
                                    await repository.Remove(bloco);
                                }
                            }
                            retorno.IdObjeto = apartamento.Id;
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

        [Route("api/CondominioPredioapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Endereco_Logradouro>(session))
                    {

                        Endereco_Logradouro modelo = null;
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
        public async Task<GetApiRetorno<IEnumerable<Condominio_PredioListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Condominio_PredioListaModel>> retorno = new GetApiRetorno<IEnumerable<Condominio_PredioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Condominio_Predio>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Condominio_PredioListaModel
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

        [HttpPost()]
        [Route("api/CondominioPredioapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CONDOMINIO_PREDIO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_UF = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
        [Route("api/CondominioPredioapi/GetComboDescription")]
        public async Task<ObjCombo<Int64>> GetComboDescription(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlWhere = new StringBuilder(" WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_UF = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO FROM TB_CONDOMINIO_PREDIO A");
                        sql.AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.NUMERO, COALESCE(A.LOGRADOURO, B.DESCRICAO) AS RUA, C.DESCRICAO AS BAIRRO, D.DESCRICAO AS CIDADE, E.SIGLA AS UF, F.NOME AS PAIS ");
                        sql.AppendLine(" FROM TB_CONDOMINIO_PREDIO A ");
                        sql.AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO B ON B.ID = A.ID_ENDERECO_LOGRADOURO ");
                        sql.AppendLine(" LEFT JOIN TB_BAIRRO C ON C.ID = B.ID_BAIRRO ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE D ON D.ID = B.ID_CIDADE ");
                        sql.AppendLine(" LEFT JOIN TB_UF E ON E.ID = B.ID_UF ");
                        sql.AppendLine(" LEFT JOIN TB_PAIS F ON F.ID = B.ID_PAIS ");
                        sql.AppendLine(sqlWhere.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]) + ", " + Convert.ToString(reader["NUMERO"]) + ", " + Convert.ToString(reader["RUA"]) + ", " + Convert.ToString(reader["BAIRRO"]) + ", " + Convert.ToString(reader["CIDADE"]) + ", " + Convert.ToString(reader["UF"]) + ", " + Convert.ToString(reader["PAIS"])
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
        [Route("api/CondominioPredioapi/GetComboEndereco")]
        public async Task<ObjCombo<String>> GetComboEndereco(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ENDERECO_LOGRADOURO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai != null)
                    {
                        condicoes.Add(" A.CEP LIKE UPPER(@SEARCH) ");

                        parametros.Add("@SEARCH", String.Format("%{0}%", request.idpai));
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder(@"SELECT A.ID, A.DESCRICAO, A.NUMERO, Coalesce(A.CEP,B.CEP) AS CEP, COALESCE(A.LOGRADOURO, B.DESCRICAO) AS ENDERECO_LOGRADOURO, 
                                                                       B.TIPO, B.TIPO_ZONA, COALESCE(A.ID_PAIS, B.ID_PAIS) AS ID_PAIS, COALESCE((SELECT NOME FROM TB_PAIS PAIS WHERE PAIS.ID = A.ID_PAIS), C.NOME) AS PAIS,
                                                                               COALESCE(A.UF, D.SIGLA) AS UF, COALESCE(A.BAIRRO, F.DESCRICAO) AS BAIRRO, E.ID AS ID_CIDADE, F.ID AS ID_BAIRRO, D.ID AS ID_UF,
                                                                               COALESCE((SELECT CID.DESCRICAO FROM TB_CIDADE CID WHERE CID.ID = A.ID_CIDADE), E.DESCRICAO ) AS CIDADE,
                                                               B.COD_AREA, B.COD_ESCRITORIO, B.COD_LOCALIDADE                                                                        
                                                               FROM TB_CONDOMINIO_PREDIO A
                                                               LEFT JOIN TB_ENDERECO_LOGRADOURO B
                                                               ON A.ID_ENDERECO_LOGRADOURO = B.ID                                                              
                                                               LEFT JOIN TB_PAIS C
                                                               ON B.ID_PAIS = C.ID
                                                               LEFT JOIN TB_UF D
                                                               ON B.ID_UF = D.ID
                                                               LEFT JOIN TB_CIDADE E
                                                               ON B.ID_CIDADE = E.ID
                                                               LEFT JOIN TB_BAIRRO F
                                                               ON B.ID_BAIRRO = F.ID                                                              
                                                               WHERE");
                        sql.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResultTipoCondominio<String>();

                                obj.Id = Convert.ToString(reader["ID"]);
                                obj.Text = Convert.ToString(reader["DESCRICAO"]) + ',' + Convert.ToString(reader["ENDERECO_LOGRADOURO"]) + ',' + Convert.ToString(reader["NUMERO"]) + ',' + Convert.ToString(reader["BAIRRO"]) + ',' + Convert.ToString(reader["CIDADE"]) + ',' + Convert.ToString(reader["UF"]) + ',' + Convert.ToString(reader["PAIS"]);
                                obj.Id_Pais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : (long?)null;
                                obj.Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : (long?)null;
                                obj.Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : (long?)null;
                                obj.Id_Uf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : (long?)null;

                                obj.Pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : String.Empty;
                                obj.Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : String.Empty;
                                obj.Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : String.Empty;
                                obj.Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : String.Empty;
                                obj.Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty;
                                obj.Endereco_Logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]) : String.Empty;
                                obj.Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : String.Empty;
                                obj.Tipo_Endereco_Logradouro = reader["TIPO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])) : String.Empty;
                                obj.Tipo_Atendimento_Zona = reader["TIPO_ZONA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Atendimento_Zona>(Convert.ToString(reader["TIPO_ZONA"])) : String.Empty;
                                obj.Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : String.Empty;
                                obj.CodArea = reader["COD_AREA"] != DBNull.Value ? Convert.ToString(reader["COD_AREA"]) : String.Empty;
                                obj.CodGrupoArea = reader["COD_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["COD_ESCRITORIO"]) : String.Empty;
                                obj.Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["COD_LOCALIDADE"]) : String.Empty;

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
        [Route("api/CondominioPredioapi/GetComboBloco")]
        public async Task<ObjCombo<Int64>> GetComboBloco(RequestObjComboPai<string, long?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_COND_PR_BLOCO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CONDOMINIO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.ID_CONDOMINIO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResultBloco<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    IdCondominio = Convert.ToString(reader["ID_CONDOMINIO"]),
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }

                    retorno.results = retorno.results.OrderBy(p => Char.IsLower(p.Text[0])).ThenBy(p => p.Text).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/CondominioPredioapi/GetComboApartamento")]
        public async Task<ObjCombo<Int64>> GetComboApartamento(RequestObjComboAvo<long?, string, long?> request)
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

                    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_COND_PR_APARTAMENTO A 
                                                                INNER JOIN TB_COND_PR_BLOCO B ON A.ID_BLOCO = B.ID 
                                                                INNER JOIN TB_CONDOMINIO_PREDIO C ON B.ID_CONDOMINIO = C.ID WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.ID = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                    retorno.results = retorno.results.OrderBy(p => Convert.ToInt64(p.Text)).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }
        [HttpPost()]
        [Route("api/CondominioPredioapi/GetComboBlocoDesc")]
        public async Task<ObjCombo<String>> GetComboBlocoDesc(RequestObjComboPai<string, Int64?> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_COND_PR_BLOCO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CONDOMINIO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.ID_CONDOMINIO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResultBloco<string>
                                {
                                    Id = Convert.ToString(reader["DESCRICAO"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    IdCondominio = Convert.ToString(reader["ID_CONDOMINIO"]),
                                    Codigo = Convert.ToInt64(reader["ID"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }

                    retorno.results = retorno.results.OrderBy(p => Char.IsLower(p.Text[0])).ThenBy(p => p.Text).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;
        }
        [HttpPost()]
        [Route("api/CondominioPredioapi/GetComboApartamentoDesc")]
        public async Task<ObjCombo<String>> GetComboApartamentoDesc(RequestObjComboAvo<long?, string, long?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_COND_PR_APARTAMENTO A 
                                                                INNER JOIN TB_COND_PR_BLOCO B ON A.ID_BLOCO = B.ID 
                                                                INNER JOIN TB_CONDOMINIO_PREDIO C ON B.ID_CONDOMINIO = C.ID WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.ID = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["DESCRICAO"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                    retorno.results = retorno.results.OrderBy(p => Convert.ToInt64(p.Text)).ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }


        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Condominio_PredioImportacao item, Condominio_Predio condominioPredios)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Condominio_Predio> Condominio_Predio = new Conexao<Condominio_Predio>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoCidade = new Conexao<Cidade>(session))

            {
                var config = repoConfig.selectFirst().Result;

                if (string.IsNullOrEmpty(item.Id) == false)
                {
                    var condominio = await Condominio_Predio.selectFirst(p => p.Id == Convert.ToInt64(item.Id));
                    if (condominio == null)
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Id,
                            Erro = "Condomínio/Prédio não foi encontrado."
                        });
                    }
                    else
                    {
                        condominioPredios = condominio;
                    }
                }

                if (config.Utiliza_Cadastro_Endereco)
                {
                    var enderecoLogradouro = repoEnderecoLogradouro.selectFirst(p => p.Id == Convert.ToInt64(item.Id_Endereço_Logradouro)).Result;
                    condominioPredios.Endereco_Logradouro = enderecoLogradouro;

                    if (enderecoLogradouro == null)
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Id_Endereço_Logradouro,
                            Erro = "Endereço/Logradouro não existente."
                        });

                    var condominioExistente = await Condominio_Predio.selectFirst(p => enderecoLogradouro != null && p.Numero == item.Numero && p.Descricao == item.Descricao_Condominio_Predio);

                    if (condominioExistente != null)
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Id_Endereço_Logradouro,
                            Erro = "Condominio/Predio já existente. Condominio/Predio não foi importado."
                        });
                }

                if (String.IsNullOrEmpty(item.Descricao_Condominio_Predio))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Descricao_Condominio_Predio,
                        Erro = "Descrição não foi importado."
                    });

                if (retorno.Count() > 0)
                    return retorno;

                if (condominioPredios.Id == 0)
                    condominioPredios.Id = Convert.ToInt16(item?.Id);

                var cidade = await repoCidade.selectFirst(p => p.Descricao == item.Cidade.ToUpper());

                if (cidade == null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Cidade,
                        Erro = "Cidade não existente."
                    });
                else
                {

                    condominioPredios.Cep = item?.Cep;
                    condominioPredios.Descricao = item.Descricao_Condominio_Predio.ToUpper();
                    condominioPredios.Numero = item?.Numero;
                    condominioPredios.Uf = item?.UF;
                    condominioPredios.Id_Pais = Convert.ToInt16(item?.País);
                    condominioPredios.Logradouro = item?.Logradouro;
                    condominioPredios.Bairro = item?.Bairro;
                    condominioPredios.Id_Cidade = cidade?.Id;
                    condominioPredios.Bloco = item?.Bloco_Quadra;
                    condominioPredios.Apto_Numero = item?.Apto_Numero;
                    condominioPredios.Status = Status_Default.Ativo;
                }

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(Condominio_PredioImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Condominio_Predio> Condominio_Predio = new Conexao<Condominio_Predio>(session))
            using (Conexao<Condominio_PredioBloco> CondominioPredioBloco = new Conexao<Condominio_PredioBloco>(session))
            using (Conexao<Condominio_PredioApartamento> CondominioPredioApto = new Conexao<Condominio_PredioApartamento>(session))
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                var config = repoConfig.selectFirst().Result;
                Condominio_Predio condominioPredios = new Condominio_Predio();

                retorno = await ImportacaoModeloBase(session, item, condominioPredios);

                // Se houver erros, retorna a lista de logs
                if (retorno.Count > 0)
                    return retorno;

                //condominioPredios.Descricao = item.Descricao_Condominio_Predio.ToUpper();

                //condominioPredios.Uf = item?.UF;

                var condominio = await Condominio_Predio.selectFirst(p => p.Descricao == item.Descricao_Condominio_Predio);
                if (condominio != null)
                {
                    condominioPredios = condominio;
                }
                await Condominio_Predio.SaveOrUpdate(condominioPredios);

                // Se o condomínio foi salvo/atualizado com sucesso, processa bloco e apartamento
                if (condominioPredios.Id != 0)
                {
                    var bloco = await CondominioPredioBloco.selectFirst(p => p.Descricao == condominioPredios.Bloco && p.Condominio.Id == condominioPredios.Id);
                    if (bloco == null)
                    {
                        bloco = new Condominio_PredioBloco
                        {
                            Condominio = condominioPredios,
                            Status = Status_Default.Ativo,
                            Descricao = condominioPredios.Bloco
                        };
                        await CondominioPredioBloco.SaveOrUpdate(bloco);
                    }

                    var apto = await CondominioPredioApto.selectFirst(p => p.Bloco.Id == bloco.Id && p.Descricao == condominioPredios.Apto_Numero);

                    if (apto == null)
                    {
                        apto = new Condominio_PredioApartamento
                        {
                            Bloco = bloco,
                            Status = Status_Default.Ativo,
                            Descricao = condominioPredios.Apto_Numero,
                        };
                        await CondominioPredioApto.SaveOrUpdate(apto);
                    }

                }

                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Descricao_Condominio_Predio,
                    Erro = "Inserido/Atualizado com sucesso"
                });

            }

            return retorno;
        }

        #region Mobile
        [HttpPost]
        [Route("api/CondominioPredioapi/GetListaCondominiosMobile")]
        public async Task<GetApiRetorno<List<Condominio_PredioMobileModel>>> GetListaCondominiosMobile(ParamWebApi<string> value)
        {
            GetApiRetorno<List<Condominio_PredioMobileModel>> retorno = new GetApiRetorno<List<Condominio_PredioMobileModel>>();

            var listaCondominios = new List<Condominio_PredioMobileModel>();
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.CEP AS CEP, C.DESCRICAO AS CIDADE  , D.SIGLA AS UF , E.NOME AS PAIS , F.DESCRICAO AS BAIRRO , ");
                    sql.AppendLine("A.NUMERO AS NUMERO , COALESCE(A.LOGRADOURO, B.DESCRICAO) AS LOGRADOURO , A.DESCRICAO AS CONDOMINIO , A.STATUS AS STATUS , B.TIPO , B.ID_PAIS , B.ID_UF , B.ID_CIDADE , B.ID_BAIRRO ");
                    sql.AppendLine("FROM TB_CONDOMINIO_PREDIO A ");
                    sql.AppendLine("INNER JOIN TB_ENDERECO_LOGRADOURO B ON (A.ID_ENDERECO_LOGRADOURO = B.ID) ");
                    sql.AppendLine("INNER JOIN TB_CIDADE C ON (B.ID_CIDADE = C.ID) ");
                    sql.AppendLine("INNER JOIN TB_UF D ON (C.ID_UF = D.ID) ");
                    sql.AppendLine("INNER JOIN TB_PAIS E ON (D.ID_PAIS = E.ID) ");
                    sql.AppendLine("INNER JOIN TB_BAIRRO F ON (B.ID_BAIRRO = F.ID) ");
                    sql.AppendLine("WHERE A.STATUS = 1 AND UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                    parametros.Add("@SEARCH", String.Format("%{0}%", value.Objeto));


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            listaCondominios.Add(new Condominio_PredioMobileModel
                            {
                                Id_Condominio = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                Cep = Convert.ToString(reader["CEP"]),
                                Cidade = Convert.ToString(reader["CIDADE"]),
                                Uf = Convert.ToString(reader["UF"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Bairro = Convert.ToString(reader["BAIRRO"]),
                                Numero = Convert.ToString(reader["NUMERO"]),
                                Logradouro = Convert.ToString(reader["LOGRADOURO"]),
                                Descricao_Condominio = Convert.ToString(reader["CONDOMINIO"]),
                                //TipoLogradouro =  HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(Convert.ToString(reader["TIPO"])),
                                TipoLogradouro = (Tipo_Endereco_Logradouro)Enum.Parse(typeof(Tipo_Endereco_Logradouro), Convert.ToString(reader["TIPO"])),
                                Id_Pais = reader["ID_PAIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_PAIS"]) : 0,
                                Id_Cidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0,
                                Id_Uf = reader["ID_UF"] != DBNull.Value ? Convert.ToInt64(reader["ID_UF"]) : 0,
                                Id_Bairro = reader["ID_BAIRRO"] != DBNull.Value ? Convert.ToInt64(reader["ID_BAIRRO"]) : 0,

                            });
                        }
                    }
                }

                retorno.Objeto = listaCondominios;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
        #endregion
    }
}
