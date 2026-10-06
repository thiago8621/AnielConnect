using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Tasks;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.nhibernate.mongo;
using Hangfire;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.mongo;
using NHibernate.Hql.Ast;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Area_ManutencaoApiController : BaseApiController
    {
        /// <summary>
        /// GetAll das Manutenções de Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Area_ManutencaoApi/GetAll")]

        #region GetAll

        public async Task<DataTableResponse<Area_ManutencaoModels>> GetAll(Area_ManutencaoDataTableRequest request)
        {
            try
            {
                List<Area_ManutencaoModels> retorno = new List<Area_ManutencaoModels>();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var RecordsTotal = 0;
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())

                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sqlFrom = new StringBuilder("FROM TB_AREA_MANUTENCAO A");
                        sqlFrom.AppendLine("");

                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                RecordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }

                    }
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sqlWhere = new StringBuilder();
                        StringBuilder sqlFrom = new StringBuilder("FROM TB_AREA_MANUTENCAO A ");


                        StringBuilder sql = new StringBuilder("SELECT");
                        sql.AppendLine(@" A.ID, A.DESCRICAO, A.DATA_HORA_INICIAL, A.DATA_HORA_FIM, A.STATUS, A.ID_ELEMENTO_REDE ");
                        sql.AppendLine(sqlFrom.ToString());
                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            sqlWhere.AppendLine("WHERE A.DESCRICAO LIKE @SEARCH");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                            sql.AppendLine(sqlWhere.ToString());
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                            while (await reader.ReadAsync())
                            {

                                var diaInicial = reader["DATA_HORA_INICIAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_INICIAL"]) : (DateTime?)null;
                                var diaFinal = reader["DATA_HORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FIM"]) : (DateTime?)null;

                                var obj = new Area_ManutencaoModels
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,

                                    DataFinal = reader["DATA_HORA_FIM"] != DBNull.Value ? Convert.ToString(diaFinal.Value.ToString("dd/MM/yyyy")) : "-",
                                    DataInicial = reader["DATA_HORA_INICIAL"] != DBNull.Value ? Convert.ToString(diaInicial.Value.ToString("dd/MM/yyyy")) : "-",
                                    HoraFinal = reader["DATA_HORA_FIM"] != DBNull.Value ? Convert.ToString(diaFinal.Value.ToString("hh:mm")) : "-",
                                    HoraInicial = reader["DATA_HORA_INICIAL"] != DBNull.Value ? Convert.ToString(diaInicial.Value.ToString("hh:mm")) : "-",
                                    ElementoRede = reader["ID_ELEMENTO_REDE"] != DBNull.Value ? Convert.ToString(reader["ID_ELEMENTO_REDE"]) : "-",
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",

                                    //Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "-",

                                    Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])) : "-",

                                };
                                retorno.Add(obj);
                            }
                    }
                }


                return new DataTableResponse<Area_ManutencaoModels>
                {
                    draw = request.draw,
                    data = retorno,
                    error = "",
                    recordsFiltered = RecordsTotal
                };
            }
            catch (Exception e)
            {

                return new DataTableResponse<Area_ManutencaoModels>
                {
                    error = e.Message,
                };
            }
        }
        #endregion

        /// <summary>
        /// GetAll das Manutenções de Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Area_ManutencaoApi/GetClienteManutencao")]

        #region GetAll
        //public async Task<List<string>> GetClienteManutencao(Int64? CPF)
        public async Task<GetApiRetorno<List<Area_Cliente_ManutencaoModels>>> GetClienteManutencao(Area_Cliente_ManutencaoModels request)
        {
            GetApiRetorno<List<Area_Cliente_ManutencaoModels>> retorno = new GetApiRetorno<List<Area_Cliente_ManutencaoModels>>
            {
                Objeto = new List<Area_Cliente_ManutencaoModels>()
            };
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())

                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlWhere = new StringBuilder("WHERE A.CPF = @CPF");
                        StringBuilder sqlFrom = new StringBuilder(@"FROM TB_CLIENTE A 
                                                                    LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B
                                                                    ON A.ID = B.ID_CLIENTE"
                        );

                        StringBuilder sqlConnect = new StringBuilder(@"SELECT B.ID AS ID_COD_PLAN_PROD ");

                        sqlConnect.AppendLine(sqlFrom.ToString());
                        sqlConnect.AppendLine(sqlWhere.ToString());
                        parametros.Add("@CPF", request.CPF);

                        var sb = new System.Text.StringBuilder();

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlConnect, parametros, session.Connection, null))
                            while (await reader.ReadAsync())
                            {
                                var Id_cod_plan_prod = Convert.ToString(reader["ID_COD_PLAN_PROD"]);
                                sb.AppendLine("," + Id_cod_plan_prod);
                            }

                        sb = sb.Remove(0, 1);

                        using (var comT5 = sessionT5.Connection.CreateCommand())
                        {
                            Dictionary<String, Object> parametrosT5 = new Dictionary<string, object>();
                            StringBuilder sqlWhereT5 = new StringBuilder(@$"WHERE A.CLIENTE_PLANO_PRODUTO IN ({sb}) AND C.STATUS = 1");
                            StringBuilder sqlFromT5 = new StringBuilder(@"FROM TB_ELEMENTO_REDE_PORTA A
                                                                    INNER JOIN TB_ELEMENTO_REDE B   
                                                                    ON A.ID_CAIXA = B.ID
                                                                    LEFT JOIN TB_AREA_MANUTENCAO_ELEMENTOS C
                                                                    ON B.ID = C.ID_ELEMENTOS_AFETADOS "
                            )
                            {

                            };
                            StringBuilder sqlT5 = new StringBuilder("SELECT FIRST 1 C.DATA_HORA_FIM ");

                            sqlT5.AppendLine(sqlFromT5.ToString());
                            sqlT5.AppendLine(sqlWhereT5.ToString());

                            using (var readerT5 = await CommandHelper.ExecuteReader(comT5, sqlT5, parametrosT5, sessionT5.Connection, null))
                                while (await readerT5.ReadAsync())
                                {
                                    var diaFinal = readerT5["DATA_HORA_FIM"] != DBNull.Value ? Convert.ToDateTime(readerT5["DATA_HORA_FIM"]) : (DateTime?)null;

                                    var obj = new Area_Cliente_ManutencaoModels
                                    {
                                        DataFinal = readerT5["DATA_HORA_FIM"] != DBNull.Value ? Convert.ToString(diaFinal.Value.ToString("dd/MM/yyyy HH:mm")) : "-",
                                    };
                                    retorno.Objeto.Add(obj);
                                }
                        }
                    }
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }
        #endregion


        #region PontosManutencaoAfetados

        [HttpPost]
        [Route("api/Area_ManutencaoApi/ObterCadastrosEnderecos")]
        public async Task<ManutResponse<List<EnderecoManutResponse>>> ObterCadastrosEnderecos(EnderecosRequestManut request)
        {
            ManutResponse<List<EnderecoManutResponse>> response = new ManutResponse<List<EnderecoManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<EnderecoManutResponse> enderecos = new List<EnderecoManutResponse>();
            long countTotal = 0;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT FIRST 1 ");
                        sql.AppendLine(" CASE WHEN EXISTS(SELECT 1 FROM TB_ENDERECO_LOGRADOURO A WHERE A.STATUS = '1')  THEN 'SIM' ELSE 'NAO' END AS POSSUI_ENDERECO, ");
                        sql.AppendLine(" CASE WHEN EXISTS(SELECT 1 FROM TB_CONDOMINIO_PREDIO B WHERE B.STATUS = '1') THEN 'SIM' ELSE 'NAO' END AS POSSUI_COND_PREDIO ");
                        sql.AppendLine(" FROM TB_ENDERECO_LOGRADOURO ");
                        sql.AppendLine(" RIGHT JOIN TB_CONDOMINIO_PREDIO B ON(B.ID IS NOT NULL) ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                bool possuiEnderecos = reader["POSSUI_ENDERECO"] != DBNull.Value ? Convert.ToString(reader["POSSUI_ENDERECO"]) == "SIM" : false;
                                bool possuiCondominios = reader["POSSUI_COND_PREDIO"] != DBNull.Value ? Convert.ToString(reader["POSSUI_COND_PREDIO"]) == "SIM" : false;

                                if (possuiEnderecos)
                                {
                                    string desc = "ENDEREÇO/LOGRADOURO";
                                    string id = "END-00";

                                    EnderecoManutResponse end = new EnderecoManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        TipoEndereco = "END"
                                    };

                                    enderecos.Add(end);
                                }

                                if (possuiCondominios)
                                {
                                    string desc = "CONDOMÍNIO/PRÉDIO";
                                    string id = "COND-00";

                                    EnderecoManutResponse end = new EnderecoManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        TipoEndereco = "COND"
                                    };

                                    enderecos.Add(end);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Ok = false;
                response.Mensagem = ex.Message;
                response.recordsTotal = 0;

                return response;
            }

            response.recordsTotal = countTotal;
            response.IdObjeto = enderecos;
            response.Ok = true;
            response.Mensagem = "Enderecos encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/ObterDescricaoElementoRede")]
        public async Task<ManutResponse<List<ElementoRedeManutResponse>>> ObterDescricaoElementoRede(ElementoRedeRequestManut request)
        {
            ManutResponse<List<ElementoRedeManutResponse>> response = new ManutResponse<List<ElementoRedeManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<ElementoRedeManutResponse> elementos = new List<ElementoRedeManutResponse>();
            long countTotal = 0;

            try
            {
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A WHERE A.SITUACAO = 1 ");

                if (!string.IsNullOrEmpty(request.pesquisa))
                    sqlFrom.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                countTotal = reader["CO"] != DBNull.Value ? Convert.ToInt64(reader["CO"]) : 0;
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                long id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    ElementoRedeManutResponse elem = new ElementoRedeManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc
                                    };

                                    elementos.Add(elem);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Ok = false;
                response.Mensagem = ex.Message;
                response.recordsTotal = 0;

                return response;
            }

            response.recordsTotal = countTotal;
            response.IdObjeto = elementos;
            response.Ok = true;
            response.Mensagem = "Elementos de Redes encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarPortasElementoRede")]
        public async Task<ManutResponse<List<ElementoRedeManutResponse>>> BuscarPortasElementoRede(ElementoRedeRequestManut request)
        {
            ManutResponse<List<ElementoRedeManutResponse>> response = new ManutResponse<List<ElementoRedeManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<ElementoRedeManutResponse> elementos = new List<ElementoRedeManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_PORTA, 'PORTA-'|| A.PORTA AS DESC_PORTA, B.ID AS ID_ELEMENTO, C.ID AS ID_TIPO ");
                        sql.AppendLine(" FROM TB_ELEMENTO_REDE_PORTA A ");
                        sql.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE B ON (B.ID = A.ID_CAIXA) ");
                        sql.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE C ON (C.ID = B.ID_TIPO) ");
                        sql.AppendLine(" WHERE A.ID_CAIXA = @ID_CAIXA ");
                        sql.AppendLine(" GROUP BY A.ID, A.PORTA, B.ID, C.ID ");

                        parametros.Add("@ID_CAIXA", request.IdElemento);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESC_PORTA"] != DBNull.Value ? Convert.ToString(reader["DESC_PORTA"]) : "";
                                long id = reader["ID_PORTA"] != DBNull.Value ? Convert.ToInt64(reader["ID_PORTA"]) : 0;
                                long idPai = reader["ID_ELEMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ELEMENTO"]) : 0;
                                long idAvo = reader["ID_TIPO"] != DBNull.Value ? Convert.ToInt64(reader["ID_TIPO"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    ElementoRedeManutResponse elem = new ElementoRedeManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                        IdAvo = idAvo
                                    };

                                    elementos.Add(elem);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = elementos;
            response.Ok = true;
            response.Mensagem = "Portas encontradas com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/ObterDescricaoPop")]
        public async Task<ManutResponse<List<PopManutResponse>>> ObterDescricaoPop(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();
            long countTotal = 0;

            try
            {
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_POP A ");

                if (!string.IsNullOrEmpty(request.pesquisa))
                {
                    sqlFrom.AppendLine($" WHERE UPPER(A.NOME) LIKE '%{request.pesquisa.ToUpper()}%' ");
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                countTotal = reader["CO"] != DBNull.Value ? Convert.ToInt64(reader["CO"]) : 0;
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "";
                                long id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Ok = false;
                response.Mensagem = ex.Message;
                response.recordsTotal = 0;

                return response;
            }

            response.recordsTotal = countTotal;
            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Pops encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarItensAtreladosPop")]
        public async Task<ManutResponse<PopItensManutResponse>> BuscarItensAtreladosPop(PopRequestManut request)
        {
            ManutResponse<PopItensManutResponse> response = new ManutResponse<PopItensManutResponse>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            PopItensManutResponse verificaPop = new PopItensManutResponse();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT DISTINCT 1, ");
                        sql.AppendLine(" CASE WHEN B.ID IS NOT NULL THEN 'TRUE' ELSE 'FALSE' END AS EXISTE_CONCENTRADORA, ");
                        sql.AppendLine(" CASE WHEN C.ID IS NOT NULL THEN 'TRUE' ELSE 'FALSE' END AS EXISTE_RADIUS, ");
                        sql.AppendLine(" CASE WHEN D.ID IS NOT NULL THEN 'TRUE' ELSE 'FALSE' END AS EXISTE_OLT  ");
                        sql.AppendLine(" FROM TB_POP A ");
                        sql.AppendLine(" LEFT JOIN TB_CONCENTRADORA B ON (B.ID_POP = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_RADIUS C ON (C.ID = B.ID_RADIUS) ");
                        sql.AppendLine(" LEFT JOIN TB_OLT D ON (D.ID_POP = A.ID) ");
                        sql.AppendLine(" WHERE A.ID = @ID_POP ");

                        parametros.Add("@ID_POP", request.IdPop);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                verificaPop.TemConcentradora = reader["EXISTE_CONCENTRADORA"] != null ? Convert.ToBoolean(reader["EXISTE_CONCENTRADORA"]) : false;
                                verificaPop.TemRadius = reader["EXISTE_RADIUS"] != null ? Convert.ToBoolean(reader["EXISTE_RADIUS"]) : false;
                                verificaPop.TemOlt = reader["EXISTE_OLT"] != null ? Convert.ToBoolean(reader["EXISTE_OLT"]) : false;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = verificaPop;
            response.Ok = true;
            response.Mensagem = "Pops encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarRadiusPop")]
        public async Task<ManutResponse<List<PopManutResponse>>> BuscarRadiusPop(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_POP, C.ID AS ID_RADIUS, C.DESCRICAO ");
                        sql.AppendLine(" FROM TB_POP A ");
                        sql.AppendLine(" LEFT JOIN TB_CONCENTRADORA B ON (B.ID_POP = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_RADIUS C ON (C.ID = B.ID_RADIUS) ");
                        sql.AppendLine(" WHERE A.ID = @ID_POP ");

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(C.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%'");

                        sql.AppendLine(" GROUP BY A.ID, C.ID, C.DESCRICAO ");

                        parametros.Add("@ID_POP", request.IdPop);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                long id = reader["ID_RADIUS"] != DBNull.Value ? Convert.ToInt64(reader["ID_RADIUS"]) : 0;
                                long idPai = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Radius encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarConcentradorasPop")]
        public async Task<ManutResponse<List<PopManutResponse>>> BuscarConcentradorasPop(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_CONCENTRADORA, A.NOME AS NOME_CONCENTRADORA, B.ID AS ID_POP ");
                        sql.AppendLine(" FROM TB_CONCENTRADORA A ");
                        sql.AppendLine(" LEFT JOIN TB_POP B ON (A.ID_POP = B.ID) ");
                        sql.AppendLine(" WHERE A.ID_POP = @ID_POP ");

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(A.NOME) LIKE '%{request.pesquisa.ToUpper()}%' ");

                        sql.AppendLine(" GROUP BY A.ID, A.NOME, B.ID ");

                        parametros.Add("@ID_POP", request.IdPop);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["NOME_CONCENTRADORA"] != DBNull.Value ? Convert.ToString(reader["NOME_CONCENTRADORA"]) : "";
                                long id = reader["ID_CONCENTRADORA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONCENTRADORA"]) : 0;
                                long idPai = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Concentradoras encontradas com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarOLTSPop")]
        public async Task<ManutResponse<List<PopManutResponse>>> BuscarOLTSPop(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_OLT, A.DESCRICAO AS NOME_OLT, B.ID AS ID_POP ");
                        sql.AppendLine(" FROM TB_OLT A ");
                        sql.AppendLine(" LEFT JOIN TB_POP B ON (A.ID_POP = B.ID) ");
                        sql.AppendLine(" WHERE A.ID_POP = @ID_POP ");

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");

                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, B.ID ");

                        parametros.Add("@ID_POP", request.IdPop);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["NOME_OLT"] != DBNull.Value ? Convert.ToString(reader["NOME_OLT"]) : "";
                                long id = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : 0;
                                long idPai = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Olts encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarSlotsOLT")]
        public async Task<ManutResponse<List<PopManutResponse>>> BuscarSlotsOLT(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_SLOT, A.DESCRICAO AS DESC_SLOT, B.ID AS ID_OLT, C.ID AS ID_POP ");
                        sql.AppendLine(" FROM TB_SLOT A ");
                        sql.AppendLine(" LEFT JOIN TB_OLT B ON (A.ID_OLT = B.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_POP C ON (B.ID_POP = C.ID) ");

                        if (request.IdsOlts != null && request.IdsOlts.Count > 0)
                            sql.AppendLine($" WHERE A.ID_OLT IN ({String.Join(",", request.IdsOlts)}) AND C.ID = @ID_POP ");
                        else
                            sql.AppendLine($" WHERE A.ID_OLT = @ID_OLT AND C.ID = @ID_POP ");

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");

                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, B.ID, C.ID ");
                        parametros.Add("@ID_POP", request.IdPop);
                        parametros.Add("@ID_OLT", request.IdOlt);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESC_SLOT"] != DBNull.Value ? Convert.ToString(reader["DESC_SLOT"]) : "";
                                long id = reader["ID_SLOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_SLOT"]) : 0;
                                long idPai = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : 0;
                                long idAvo = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                        IdAvo = idAvo,
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Slots encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarPonsSlots")]
        public async Task<ManutResponse<List<PopManutResponse>>> BuscarPonsSlots(PopRequestManut request)
        {
            ManutResponse<List<PopManutResponse>> response = new ManutResponse<List<PopManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<PopManutResponse> pops = new List<PopManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_PON, A.DESCRICAO AS DESC_PON, B.ID AS ID_SLOT, C.ID AS ID_OLT, D.ID AS ID_POP ");
                        sql.AppendLine(" FROM TB_PON A ");
                        sql.AppendLine(" LEFT JOIN TB_SLOT B ON (B.ID = A.ID_SLOT) ");
                        sql.AppendLine(" LEFT JOIN TB_OLT C ON (C.ID = B.ID_OLT) ");
                        sql.AppendLine(" LEFT JOIN TB_POP D ON (D.ID = C.ID_POP) ");

                        if (request.IdsSlots != null && request.IdsSlots.Count > 0)
                            sql.AppendLine($" WHERE B.ID  IN ({String.Join(",", request.IdsSlots)}) AND D.ID = @ID_POP ");
                        else
                            sql.AppendLine(" WHERE B.ID = @ID_SLOT AND C.ID = @ID_OLT AND D.ID = @ID_POP ");

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");

                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, B.ID,C.ID, D.ID ");
                        parametros.Add("@ID_SLOT", request.IdSlot);
                        parametros.Add("@ID_OLT", request.IdOlt);
                        parametros.Add("@ID_POP", request.IdPop);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESC_PON"] != DBNull.Value ? Convert.ToString(reader["DESC_PON"]) : "";
                                long id = reader["ID_PON"] != DBNull.Value ? Convert.ToInt64(reader["ID_PON"]) : 0;
                                long idPai = reader["ID_SLOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_SLOT"]) : 0;
                                long idAvo = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : 0;
                                long idBizAvo = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                if (desc != "" && id != 0)
                                {
                                    PopManutResponse pop = new PopManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                        IdAvo = idAvo,
                                        IdBizAvo = idBizAvo,
                                    };

                                    pops.Add(pop);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = pops;
            response.Ok = true;
            response.Mensagem = "Pons encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarElementosTipo")]
        public async Task<ManutResponse<List<ElementoRedeManutResponse>>> BuscarElementos(ElementoRedeRequestManut request)
        {
            ManutResponse<List<ElementoRedeManutResponse>> response = new ManutResponse<List<ElementoRedeManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<ElementoRedeManutResponse> elementos = new List<ElementoRedeManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT B.ID AS ID_ELEMENTO, B.DESCRICAO AS DESC_ELEMENTO, A.ID AS ID_TIPO, ");
                        sql.AppendLine(" CASE WHEN A.E_CAIXA IS NOT NULL AND  A.E_CAIXA <> '0' THEN 'SIM' ELSE 'NAO' END AS IS_CAIXA ");
                        sql.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A ");
                        sql.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE B ON (B.ID_TIPO = A.ID) ");
                        sql.AppendLine(" WHERE B.SITUACAO <> '0' ");

                        if (request.IdsTiposElementos != null && request.IdsTiposElementos.Count > 0)
                        {
                            sql.AppendLine($" AND A.ID IN ({String.Join(",", request.IdsTiposElementos)})");
                        }
                        else
                        {
                            sql.AppendLine($" AND A.ID = @ID_TIPO ");
                            parametros.Add("@ID_TIPO", request.IdTipoElemento);
                        }

                        if (!string.IsNullOrEmpty(request.pesquisa))
                            sql.AppendLine($" AND UPPER(B.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");

                        sql.AppendLine(" GROUP BY B.ID, B.DESCRICAO, A.ID, A.E_CAIXA ");


                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string desc = reader["DESC_ELEMENTO"] != DBNull.Value ? Convert.ToString(reader["DESC_ELEMENTO"]) : "";
                                long id = reader["ID_ELEMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ELEMENTO"]) : 0;
                                long idPai = reader["ID_TIPO"] != DBNull.Value ? Convert.ToInt64(reader["ID_TIPO"]) : 0;
                                bool isCaixa = reader["IS_CAIXA"] != DBNull.Value ? (Convert.ToString(reader["IS_CAIXA"]) == "SIM") : false;

                                if (desc != "" && id != 0)
                                {
                                    ElementoRedeManutResponse elem = new ElementoRedeManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        IdPai = idPai,
                                        IsCaixa = isCaixa
                                    };

                                    elementos.Add(elem);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = elementos;
            response.Ok = true;
            response.Mensagem = "Elementos encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/GravarPontosAfetados")]
        public async Task<PostApiRetorno<bool>> GravarPontosAfetados(PontosAfetadosManut request)
        {
            PostApiRetorno<bool> response = new PostApiRetorno<bool>();

            try
            {
                //if (request.IdPop is null && request.IdTipoElementoRede is null && request.IdTipoEndereco is null)
                //    throw new Exception("Para adicionar um ponto afetado, selecione um registro na busca.");

                if (string.IsNullOrEmpty(request.Tipo))
                    throw new Exception("Informe o tipo do ponto afetado");

                var tipo = new List<string> { "POP", "CONCENTRADORA", "RADIUS", "OLT" };
                if ((request.Ids is null || request.Ids.Count == 0) && (request.ElementosRedeRequests is null || request.ElementosRedeRequests.Count == 0) && (!tipo.Any(x => request.Tipo.ToUpper().Contains(x))))
                    throw new Exception("Ids dos pontos não informados");

                if (string.IsNullOrEmpty(request.Codct))
                    throw new Exception("Contrato não informados");

                if (string.IsNullOrEmpty(request.Projeto))
                    throw new Exception("Projeto não informados");

                if (string.IsNullOrEmpty(request.NumDoc))
                    throw new Exception("NumDoc não informados");

                if (string.IsNullOrEmpty(request.NumObra))
                    throw new Exception("NumObra não informados");

                using (var apiPontosAfetados = new PontosAfetadosManutencaoApiController())
                {
                    var responseData = await apiPontosAfetados.GravarPontosAfetados(request);

                    if (!responseData.Ok)
                        throw new Exception("Erro ao Gravar Pontos afetados, tente novamente.");
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
                response.IdObjeto = false;
                return response;
            }

            response.Ok = true;
            response.Mensagem = "Pontos Afetados foram gravados com sucesso!";
            response.IdObjeto = true;
            return response;
        }

        [HttpPost()]
        [Route("api/Area_ManutencaoApi/BuscarPontosAfetados")]
        public async Task<ManutResponse<List<PontosAfetadosTree>>> BuscarPontosAfetados(PontosAfetadosRequestModel request)
        {
            ManutResponse<List<PontosAfetadosTree>> response = new ManutResponse<List<PontosAfetadosTree>>();
            var pontosAfetadosList = new List<PontosAfetadosTree>();
            long countClientesAfetados = 0;
            long countPontosAfetados = 0;

            try
            {
                if (string.IsNullOrEmpty(request.Codct))
                    throw new Exception("Contrato não informados");

                if (string.IsNullOrEmpty(request.Projeto))
                    throw new Exception("Projeto não informados");

                if (string.IsNullOrEmpty(request.NumDoc))
                    throw new Exception("NumDoc não informados");

                if (string.IsNullOrEmpty(request.NumObra))
                    throw new Exception("NumObra não informados");

                using (var apiPontoManutencao = new PontosAfetadosManutencaoApiController())
                {
                    var obterPontosManut = apiPontoManutencao.GetAllPontosAfetados(request).Result.IdObjeto;

                    if (obterPontosManut == null)
                    {
                        response.IdObjeto = null;
                        response.Ok = true;
                        return response;
                    }

                    countClientesAfetados = obterPontosManut.clientesAfetados != null ? obterPontosManut.clientesAfetados.Count : 0;

                    var pontosManutencaoValidos = await ChecarPontosAfetados(request, obterPontosManut);

                    var popsAfetados = await ObterPontosAfetadosPop(request, obterPontosManut, pontosManutencaoValidos);

                    if (popsAfetados != null && popsAfetados.Any())
                        pontosAfetadosList.AddRange(popsAfetados);

                    var elementosRedeAfetados = await ObterPontosAfetadosElementosRede(request, obterPontosManut, pontosManutencaoValidos);

                    if (elementosRedeAfetados != null && elementosRedeAfetados.Any())
                        pontosAfetadosList.AddRange(elementosRedeAfetados);

                    var enderecosCondominiosAfetados = await ObterPontosAfetadosEnderecosCondominios(request, obterPontosManut, pontosManutencaoValidos);

                    if (enderecosCondominiosAfetados != null && enderecosCondominiosAfetados.Any())
                        pontosAfetadosList.AddRange(enderecosCondominiosAfetados);

                    countPontosAfetados = await CountQuantidadePontos(request, obterPontosManut);
                }
            }

            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Ok = false;
                response.IdObjeto = null;
                return response;
            }

            response.IdObjeto = pontosAfetadosList;
            response.recordsClientes = countClientesAfetados;
            response.recordsTotal = countPontosAfetados;
            response.Ok = true;
            return response;
        }


        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarCondominiosPredios")]
        public async Task<ManutResponse<List<EnderecoManutResponse>>> BuscarCondominiosPredios(EnderecosRequestManut request)
        {
            ManutResponse<List<EnderecoManutResponse>> response = new ManutResponse<List<EnderecoManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<EnderecoManutResponse> condominios = new List<EnderecoManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_COND, A.DESCRICAO, A.LOGRADOURO, A.CEP, A.NUMERO, A.BAIRRO, ");
                        sql.AppendLine(" B.DESCRICAO AS CIDADE, C.DESCRICAO AS ESTADO, D.NOME AS PAIS ");
                        sql.AppendLine(" FROM TB_CONDOMINIO_PREDIO A ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE B ON (B.ID = A.ID_CIDADE) ");
                        sql.AppendLine(" LEFT JOIN TB_UF C ON (C.ID = B.ID_UF) ");
                        sql.AppendLine(" LEFT JOIN TB_PAIS D ON (D.ID = C.ID_PAIS) ");
                        sql.AppendLine(" WHERE A.STATUS = '1' ");

                        if (!string.IsNullOrEmpty(request.TipoEndereco))
                        {
                            //cep
                            if (request.TipoEndereco == "0")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.CEP) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }

                            //nome do condomio
                            if (request.TipoEndereco == "1")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }

                            //UF
                            if (request.TipoEndereco == "2")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND C.ID = '{request.IdUf}' ");
                            }

                            //CIDADE
                            if (request.TipoEndereco == "3")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND C.ID = '{request.IdUf}' ");

                                if (request.IdCidade != null)
                                    sql.AppendLine($" AND B.ID = '{request.IdCidade}' ");
                            }

                            //BAIRRO
                            if (request.TipoEndereco == "4")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND C.ID = '{request.IdUf}' ");

                                if (request.IdCidade != null)
                                    sql.AppendLine($" AND B.ID = '{request.IdCidade}' ");

                                if (!string.IsNullOrEmpty(request.DescBairro))
                                    sql.AppendLine($" AND UPPER(A.BAIRRO) LIKE '${request.DescBairro.ToUpper()}%' ");
                            }

                            //LOGRADOURO
                            if (request.TipoEndereco == "5")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.LOGRADOURO) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }
                        }

                        sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, A.LOGRADOURO, A.CEP, A.NUMERO, A.BAIRRO, ");
                        sql.AppendLine(" B.DESCRICAO, C.DESCRICAO, D.NOME ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string id = reader["ID_COND"] != DBNull.Value ? Convert.ToString(reader["ID_COND"]) : "";

                                string descCondominio = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                string logradouro = reader["LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["LOGRADOURO"]) : "";
                                string cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "";
                                string numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : "";
                                string bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "";
                                string cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "";
                                string estado = reader["ESTADO"] != DBNull.Value ? Convert.ToString(reader["ESTADO"]) : "";
                                string pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : "";

                                string desc = $"{descCondominio} - {numero} - {logradouro} - {bairro} - {cidade} - {estado} - {pais}";

                                if (!string.IsNullOrEmpty(desc))
                                {
                                    EnderecoManutResponse cond = new EnderecoManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        TipoEndereco = "CONDOMINIO"
                                    };
                                    condominios.Add(cond);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = condominios;
            response.Ok = true;
            response.Mensagem = "Condominios encontrados com sucesso!";

            return response;
        }

        [HttpPost]
        [Route("api/Area_ManutencaoApi/BuscarEnderecoLogradouro")]
        public async Task<ManutResponse<List<EnderecoManutResponse>>> BuscarEnderecoLogradouro(EnderecosRequestManut request)
        {
            ManutResponse<List<EnderecoManutResponse>> response = new ManutResponse<List<EnderecoManutResponse>>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<EnderecoManutResponse> enderecos = new List<EnderecoManutResponse>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID AS ID_END, A.DESCRICAO, A.CEP, B.DESCRICAO AS BAIRRO, ");
                        sql.AppendLine(" C.DESCRICAO AS CIDADE, D.DESCRICAO AS ESTADO, E.NOME AS PAIS ");
                        sql.AppendLine(" FROM TB_ENDERECO_LOGRADOURO A ");
                        sql.AppendLine(" LEFT JOIN TB_BAIRRO B ON (B.ID = A.ID_BAIRRO) ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE C ON (C.ID = A.ID_CIDADE) ");
                        sql.AppendLine(" LEFT JOIN TB_UF D ON (D.ID = C.ID_UF) ");
                        sql.AppendLine(" LEFT JOIN TB_PAIS E ON (E.ID = D.ID_PAIS) ");
                        sql.AppendLine(" WHERE A.STATUS = '1' ");

                        if (!string.IsNullOrEmpty(request.TipoEndereco))
                        {
                            //cep
                            if (request.TipoEndereco == "0")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.CEP) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }

                            //nome do condomio
                            if (request.TipoEndereco == "1")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }

                            //UF
                            if (request.TipoEndereco == "2")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND D.ID = '{request.IdUf}' ");
                            }

                            //CIDADE
                            if (request.TipoEndereco == "3")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND D.ID = '{request.IdUf}' ");

                                if (request.IdCidade != null)
                                    sql.AppendLine($" AND C.ID = '{request.IdCidade}' ");
                            }

                            //BAIRRO
                            if (request.TipoEndereco == "4")
                            {
                                if (request.IdUf != null)
                                    sql.AppendLine($" AND D.ID = '{request.IdUf}' ");

                                if (request.IdCidade != null)
                                    sql.AppendLine($" AND C.ID = '{request.IdCidade}' ");

                                if (request.IdBairro != null)
                                    sql.AppendLine($" AND B.ID = '{request.IdBairro}' ");
                            }

                            //LOGRADOURO
                            if (request.TipoEndereco == "5")
                            {
                                if (!string.IsNullOrEmpty(request.pesquisa))
                                    sql.AppendLine($" AND UPPER(A.DESCRICAO) LIKE '%{request.pesquisa.ToUpper()}%' ");
                            }
                        }

                        sql.AppendLine(" GROUP BY  A.ID, A.DESCRICAO, A.CEP, B.DESCRICAO, C.DESCRICAO, D.DESCRICAO, E.NOME ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string id = reader["ID_END"] != DBNull.Value ? Convert.ToString(reader["ID_END"]) : "";

                                string descEndereco = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                string cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "";
                                string bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "";
                                string cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "";
                                string estado = reader["ESTADO"] != DBNull.Value ? Convert.ToString(reader["ESTADO"]) : "";
                                string pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : "";

                                string desc = $"{descEndereco} - {bairro} - {cidade} - {estado} - {pais}";

                                if (!string.IsNullOrEmpty(desc))
                                {
                                    EnderecoManutResponse end = new EnderecoManutResponse
                                    {
                                        Id = id,
                                        Descricao = desc,
                                        TipoEndereco = "ENDERECO"
                                    };
                                    enderecos.Add(end);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Mensagem = ex.Message;
                response.Ok = false;

                return response;
            }

            response.IdObjeto = enderecos;
            response.Ok = true;
            response.Mensagem = "Enderecos encontrados com sucesso!";

            return response;
        }


        private async Task<List<PontosAfetadosTree>> ObterPontosAfetadosPop(PontosAfetadosRequestModel request,
                                                                             PontosAfetadosManutencaoMongo obterPontosManut,
                                                                             PontosAfetadosCheck pontosManutencaoValidos)
        {
            List<PontosAfetadosTree> pontosAfetadosList = new List<PontosAfetadosTree>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<long?> idsPons = new List<long?>();
            List<long?> idsSlots = new List<long?>();
            List<long?> idsOlt = new List<long?>();
            List<long?> idsConcentradoras = new List<long?>();
            List<long?> idsRadius = new List<long?>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = null;

                    if (request.NodeId == "#")
                    {
                        var idsPops = obterPontosManut.pops.Where(x => x.IdPop != null).ToList().Count > 0 ? obterPontosManut.pops.Select(x => x.IdPop).ToList() : null;

                        if (idsPops != null && idsPops.Count > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_POP_RAIZ, A.NOME AS DESC_POP ");
                            sql.AppendLine(" FROM TB_POP A ");
                            sql.AppendLine($" WHERE A.ID IN ({String.Join(",", idsPops)}) ");
                            sql.AppendLine(" GROUP BY A.ID, A.NOME ");
                        }
                    }
                    else if (request.NodeProximo == "segundo" && request.Tipo == "pop")
                    {
                        if (pontosManutencaoValidos.existemConcentradoras.HasValue &&
                            pontosManutencaoValidos.existemConcentradoras.Value)
                        {
                            pontosAfetadosList.Add(new PontosAfetadosTree
                            {
                                id = $"CONCENTRADORA-{request.NodeId}",
                                children = true,
                                text = "CONCENTRADORA",
                                parent = $"{request.NodeId}",
                                li_attr = new Dictionary<string, object>
                                        {
                                            { "data-type","concentradora"},
                                            { "data-proximo","terceiro"},
                                            { "data-pai",$"{request.NodeId}"}
                                        }
                            });
                        }

                        if (pontosManutencaoValidos.existemRadius.HasValue &&
                            pontosManutencaoValidos.existemRadius.Value)
                        {
                            pontosAfetadosList.Add(new PontosAfetadosTree
                            {
                                id = $"RADIUS-{request.NodeId}",
                                children = true,
                                text = "RADIUS",
                                parent = $"{request.NodeId}",
                                li_attr = new Dictionary<string, object>
                                        {
                                            { "data-type","radius"},
                                            { "data-proximo","terceiro"},
                                            { "data-pai",$"{request.NodeId}"}
                                        }
                            });
                        }

                        if (pontosManutencaoValidos.existemPons.HasValue && pontosManutencaoValidos.existemPons.Value)
                        {
                            if (!pontosAfetadosList.Any(p => p.id.Contains("OLTS-")))
                            {
                                pontosAfetadosList.Add(new PontosAfetadosTree
                                {
                                    id = $"OLTS-{request.NodeId}",
                                    children = true,
                                    text = "OLTS",
                                    parent = $"{request.NodeId}",
                                    li_attr = new Dictionary<string, object>
                                         {
                                            { "data-type","olt"},
                                            { "data-proximo","terceiro"},
                                            { "data-pai",$"{request.NodeId}"}
                                        }
                                });
                            }
                        }

                        return pontosAfetadosList;
                    }
                    else if (request.NodeProximo == "terceiro")
                    {
                        if (request.Tipo == "concentradora" &&
                            pontosManutencaoValidos.existemConcentradoras.HasValue &&
                            pontosManutencaoValidos.existemConcentradoras.Value)
                        {
                            idsConcentradoras.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsConcentradoras);

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_CONCENTRADORA, A.NOME AS NOME_CONCENTRADORA, B.ID AS ID_POP ");
                            sql.AppendLine(" FROM TB_CONCENTRADORA A ");
                            sql.AppendLine(" LEFT JOIN TB_POP B ON (A.ID_POP = B.ID) ");
                            sql.AppendLine($" WHERE A.ID_POP = @ID_POP AND A.ID IN ({String.Join(",", idsConcentradoras)}) ");
                            sql.AppendLine(" GROUP BY A.ID, A.NOME, B.ID ");

                            parametros.Add("@ID_POP", request.NodePai);
                        }

                        else if (request.Tipo == "radius" &&
                            pontosManutencaoValidos.existemRadius.HasValue &&
                            pontosManutencaoValidos.existemRadius.Value)
                        {
                            idsRadius.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsRadius);

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_POP, C.ID AS ID_RADIUS, C.DESCRICAO ");
                            sql.AppendLine(" FROM TB_POP A ");
                            sql.AppendLine(" LEFT JOIN TB_CONCENTRADORA B ON (B.ID_POP = A.ID) ");
                            sql.AppendLine(" LEFT JOIN TB_RADIUS C ON (C.ID = B.ID_RADIUS) ");
                            sql.AppendLine($" WHERE A.ID = @ID_POP AND C.ID IN ({String.Join(",", idsRadius)}) ");
                            sql.AppendLine(" GROUP BY A.ID, C.ID, C.DESCRICAO ");

                            parametros.Add("@ID_POP", request.NodePai);
                        }
                        else if (request.Tipo == "olt")
                        {
                            if (pontosManutencaoValidos.existemOlts.HasValue && pontosManutencaoValidos.existemOlts.Value)
                                idsOlt.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsOlts);

                            if (pontosManutencaoValidos.existemPons.HasValue && pontosManutencaoValidos.existemPons.Value)
                                idsPons.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsPons);

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT C.ID AS ID_OLT, C.DESCRICAO AS NOME_OLT, D.ID AS ID_POP ");
                            sql.AppendLine(" FROM TB_PON A ");
                            sql.AppendLine(" LEFT JOIN TB_SLOT B ON (B.ID = A.ID_SLOT) ");
                            sql.AppendLine(" LEFT JOIN TB_OLT C ON (C.ID = B.ID_OLT) ");
                            sql.AppendLine(" LEFT JOIN TB_POP D ON (D.ID = C.ID_POP) ");
                            sql.AppendLine($" WHERE A.ID IN ({String.Join(",", idsPons)}) AND D.ID = @ID_POP ");
                            sql.AppendLine($" GROUP BY C.ID, C.DESCRICAO, D.ID  ");

                            parametros.Add("@ID_POP", request.NodePai);
                        }
                        else
                        {
                            return pontosAfetadosList;
                        }
                    }
                    else if (request.NodeProximo == "quarto")
                    {
                        var ponto = obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault();

                        if (request.Tipo == "olt")
                        {
                            if (pontosManutencaoValidos.existemPons.HasValue && pontosManutencaoValidos.existemPons.Value)
                                idsPons.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsPons);

                            if (pontosManutencaoValidos.existemSlots.HasValue && pontosManutencaoValidos.existemSlots.Value)
                                idsSlots.AddRange(obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai)).FirstOrDefault().idsSlots);

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT B.ID AS ID_SLOT, B.DESCRICAO AS DESC_SLOT, C.ID AS ID_OLT, D.ID AS ID_POP ");
                            sql.AppendLine(" FROM TB_PON A ");
                            sql.AppendLine(" LEFT JOIN TB_SLOT B ON (B.ID = A.ID_SLOT) ");
                            sql.AppendLine(" LEFT JOIN TB_OLT C ON (C.ID = B.ID_OLT) ");
                            sql.AppendLine(" LEFT JOIN TB_POP D ON (D.ID = C.ID_POP) ");
                            sql.AppendLine($" WHERE A.ID IN ({String.Join(",", idsPons)}) AND D.ID = @ID_POP ");
                            sql.AppendLine($" GROUP BY B.ID, B.DESCRICAO, C.ID, D.ID  ");

                            parametros.Add("@ID_POP", request.NodePai);
                        }
                        else
                        {
                            return pontosAfetadosList;
                        }
                    }
                    else if (request.NodeProximo == "quinto")
                    {
                        var ponto = obterPontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeAvo)).FirstOrDefault();

                        if (request.Tipo == "slot")
                        {
                            if (ponto != null && ponto.idsPons != null && ponto.idsPons.Any())
                                idsPons.AddRange(ponto.idsPons);

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_PON, A.DESCRICAO AS DESC_PON, B.ID AS ID_SLOT, C.ID AS ID_OLT, D.ID AS ID_POP ");
                            sql.AppendLine(" FROM TB_PON A ");
                            sql.AppendLine(" LEFT JOIN TB_SLOT B ON (B.ID = A.ID_SLOT) ");
                            sql.AppendLine(" LEFT JOIN TB_OLT C ON (C.ID = B.ID_OLT) ");
                            sql.AppendLine(" LEFT JOIN TB_POP D ON (D.ID = C.ID_POP) ");
                            sql.AppendLine($" WHERE A.ID IN ({String.Join(",", idsPons)}) AND B.ID = @ID_SLOT AND D.ID = @ID_POP ");
                            sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO, B.ID,C.ID, D.ID ");

                            parametros.Add("@ID_POP", request.NodeAvo);
                            parametros.Add("@ID_SLOT", request.NodeId);
                        }
                        else
                        {
                            return pontosAfetadosList;
                        }

                    }

                    if (sql != null)
                    {
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (request.NodeId == "#")
                                {
                                    string idRaiz = reader["ID_POP_RAIZ"] != DBNull.Value ? Convert.ToString(reader["ID_POP_RAIZ"]) : "";
                                    string desc = reader["DESC_POP"] != DBNull.Value ? Convert.ToString(reader["DESC_POP"]) : "";

                                    pontosAfetadosList.Add(new PontosAfetadosTree
                                    {
                                        id = idRaiz,
                                        children = true,
                                        text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{idRaiz}' data-tipo='pop' value='{idRaiz}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                        parent = "#",
                                        li_attr = new Dictionary<string, object>
                                                {
                                                    { "data-type","pop"},
                                                    { "data-proximo","segundo"}
                                                }
                                    });
                                }
                                else if (request.NodeProximo == "terceiro")
                                {
                                    if (request.Tipo == "concentradora" &&
                                        pontosManutencaoValidos.existemConcentradoras.HasValue &&
                                        pontosManutencaoValidos.existemConcentradoras.Value)
                                    {
                                        string idRaiz = reader["ID_POP"] != DBNull.Value ? Convert.ToString(reader["ID_POP"]) : "";
                                        string id = reader["ID_CONCENTRADORA"] != DBNull.Value ? Convert.ToString(reader["ID_CONCENTRADORA"]) : "";
                                        string desc = reader["NOME_CONCENTRADORA"] != DBNull.Value ? Convert.ToString(reader["NOME_CONCENTRADORA"]) : "";

                                        pontosAfetadosList.Add(new PontosAfetadosTree
                                        {
                                            id = id,
                                            children = false,
                                            text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' 
                                                   data-pai='{idRaiz}' data-tipo='concentradora' value='{id}' style='margin-right: 5px;
                                                   cursor: pointer;'></i> {desc}",
                                            parent = $"CONCENTRADORA-{request.NodePai}",
                                            li_attr = new Dictionary<string, object>
                                                {
                                                    { "data-type","concentradora"},
                                                }
                                        });
                                    }

                                    if (request.Tipo == "radius" &&
                                        pontosManutencaoValidos.existemRadius.HasValue &&
                                        pontosManutencaoValidos.existemRadius.Value)
                                    {
                                        string idRaiz = reader["ID_POP"] != DBNull.Value ? Convert.ToString(reader["ID_POP"]) : "";
                                        string id = reader["ID_RADIUS"] != DBNull.Value ? Convert.ToString(reader["ID_RADIUS"]) : "";
                                        string desc = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";

                                        pontosAfetadosList.Add(new PontosAfetadosTree
                                        {
                                            id = id,
                                            children = false,
                                            text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' 
                                                   data-pai='{idRaiz}' data-tipo='radius' value='{id}' style='margin-right: 5px; 
                                                   cursor: pointer;'></i> {desc}",
                                            parent = $"RADIUS-{request.NodePai}",
                                            li_attr = new Dictionary<string, object>
                                                {
                                                    { "data-type","radius"},
                                                }
                                        });
                                    }

                                    if (request.Tipo == "olt")
                                    {
                                        string idRaiz = reader["ID_POP"] != DBNull.Value ? Convert.ToString(reader["ID_POP"]) : "";
                                        string id = reader["ID_OLT"] != DBNull.Value ? Convert.ToString(reader["ID_OLT"]) : "";
                                        string desc = reader["NOME_OLT"] != DBNull.Value ? Convert.ToString(reader["NOME_OLT"]) : "";

                                        pontosAfetadosList.Add(new PontosAfetadosTree
                                        {
                                            id = id,
                                            children = true,
                                            text = idsOlt.Contains(Convert.ToInt64(id)) ?
                                                   $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' 
                                                   data-pai='{idRaiz}' data-tipo='olt' value='{id}' style='margin-right: 5px; 
                                                   cursor: pointer;'></i> {desc}" : desc,
                                            parent = $"OLTS-{request.NodePai}",
                                            li_attr = new Dictionary<string, object>
                                                {
                                                    { "data-type","olt"},
                                                    { "data-proximo","quarto"},
                                                    { "data-pai",$"{request.NodePai}"}
                                                }
                                        });
                                    }
                                }
                                else if (request.NodeProximo == "quarto")
                                {
                                    string desc = reader["DESC_SLOT"] != DBNull.Value ? Convert.ToString(reader["DESC_SLOT"]) : "";
                                    string id = reader["ID_SLOT"] != DBNull.Value ? Convert.ToString(reader["ID_SLOT"]) : "";
                                    long idPai = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : 0;
                                    long idAvo = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                    pontosAfetadosList.Add(new PontosAfetadosTree
                                    {
                                        id = id,
                                        children = true,
                                        text = idsSlots.Contains(Convert.ToInt64(id)) ?
                                                   $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' 
                                                   data-pai='{idPai}' data-avo='{idAvo}' data-tipo='slot' value='{id}' style='margin-right: 5px; 
                                                   cursor: pointer;'></i> {desc}" : desc,
                                        parent = $"{request.NodeId}",
                                        li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","slot"},
                                                { "data-proximo","quinto"},
                                                { "data-pai",$"{idPai}"},
                                                { "data-avo",$"{idAvo}"}
                                            }
                                    });
                                }
                                else if (request.NodeProximo == "quinto")
                                {
                                    string desc = reader["DESC_PON"] != DBNull.Value ? Convert.ToString(reader["DESC_PON"]) : "";
                                    string id = reader["ID_PON"] != DBNull.Value ? Convert.ToString(reader["ID_PON"]) : "";
                                    long idPai = reader["ID_SLOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_SLOT"]) : 0;
                                    long idAvo = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : 0;
                                    long idBizAvo = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : 0;

                                    pontosAfetadosList.Add(new PontosAfetadosTree
                                    {
                                        id = id,
                                        children = false,
                                        text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' 
                                                 data-pai='{idPai}' data-avo='{idAvo}' data-bizavo='{idAvo}' data-tipo='pon' 
                                                 value='{id}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                        parent = $"{request.NodeId}",
                                        li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","pon"},
                                                { "data-pai",$"{idPai}"},
                                                { "data-avo",$"{idAvo}"},
                                                { "data-bizavo",$"{idBizAvo}"}
                                            }
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return pontosAfetadosList;
        }

        private async Task<List<PontosAfetadosTree>> ObterPontosAfetadosElementosRede(PontosAfetadosRequestModel request,
                                                                             PontosAfetadosManutencaoMongo obterPontosManut,
                                                                             PontosAfetadosCheck pontosManutencaoValidos)
        {
            List<PontosAfetadosTree> pontosAfetadosList = new List<PontosAfetadosTree>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<long> idsElementos = new List<long>();
            List<long> idsPortas = new List<long>();
            int skip = 30;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = null;

                    if (request.NodeId == "#")
                    {
                        var elementosTiposId = obterPontosManut?.tiposElementosRede?.Select(x => x.Id_Tipo)?.ToList() ?? null;

                        if (elementosTiposId != null && elementosTiposId.Count > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_TIPO_RAIZ, A.DESCRICAO AS DESC_TIPO ");
                            sql.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A WHERE A.SITUACAO = 1 ");
                            sql.AppendLine($"  AND A.ID IN ({String.Join(",", elementosTiposId)}) ");
                            sql.AppendLine(" GROUP BY A.ID, A.DESCRICAO ");
                        }
                    }
                    else if (request.NodeProximo == "segundo" && request.Tipo == "TIPO_ELEMENTO")
                    {
                        if (pontosManutencaoValidos.existemElementos.HasValue &&
                            pontosManutencaoValidos.existemElementos.Value)
                        {
                            var elementosAdicionados = obterPontosManut?.tiposElementosRede?.Where(x => x.Id_Tipo == Convert.ToInt64(request.NodeId))
                                                                                            .FirstOrDefault().ElementosRede;

                            if (elementosAdicionados != null && elementosAdicionados.Count > 0)
                                idsElementos.AddRange(elementosAdicionados.Select(x => x.Id).ToList());

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT B.ID AS ID_ELEMENTO, B.DESCRICAO AS DESC_ELEMENTO, A.ID AS ID_TIPO, ");
                            sql.AppendLine(" CASE WHEN A.E_CAIXA IS NOT NULL AND  A.E_CAIXA <> '0' THEN 'SIM' ELSE 'NAO' END AS IS_CAIXA ");
                            sql.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A ");
                            sql.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE B ON (B.ID_TIPO = A.ID) ");
                            sql.AppendLine($" WHERE A.ID = @ID_TIPO AND B.SITUACAO <> '0'  AND B.ID IN ({String.Join(",", idsElementos)}) ");
                            sql.AppendLine(" GROUP BY B.ID, B.DESCRICAO, A.ID, A.E_CAIXA ");
                            parametros.Add("@ID_TIPO", request.NodeId);
                        }
                    }
                    else if (request.NodeProximo == "terceiro" && request.Tipo == "ELEMENTO" ||
                             request.NodeProximo == "terceiro" && request.Tipo == "PORTA")
                    {
                        if (pontosManutencaoValidos.existemPortas.HasValue &&
                            pontosManutencaoValidos.existemPortas.Value)
                        {

                            var elementosAdicionados = obterPontosManut?.tiposElementosRede?
                                .Where(x => x.Id_Tipo == (request.Tipo == "ELEMENTO" ?
                                 Convert.ToInt64(request.NodePai) :
                                 Convert.ToInt64(request.NodeAvo)))
                                .FirstOrDefault().ElementosRede;

                            idsPortas.AddRange(elementosAdicionados.SelectMany(x => x.Ids_Portas).ToList());

                            bool utilizaSkip = false;
                            if (!string.IsNullOrEmpty(request.Skip))
                            {
                                skip = Convert.ToInt32(request.Skip);
                                utilizaSkip = true;
                            }

                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_PORTA, 'PORTA-' || A.PORTA AS DESC_PORTA, B.ID AS ID_ELEMENTO, C.ID AS ID_AVO ");
                            sql.AppendLine(" FROM TB_ELEMENTO_REDE_PORTA A ");
                            sql.AppendLine(" LEFT JOIN TB_ELEMENTO_REDE B ON (B.ID = A.ID_CAIXA) ");
                            sql.AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_REDE C ON (C.ID = B.ID_TIPO) ");
                            sql.AppendLine($" WHERE A.ID_CAIXA = @ID_CAIXA AND C.ID = @ID_TIPO AND A.ID IN ({String.Join(",", idsPortas)}) ");
                            sql.AppendLine(" GROUP BY A.ID, A.PORTA, B.ID, C.ID ");

                            if (utilizaSkip)
                            {
                                sql.AppendLine(" ORDER BY A.ID ASC ");
                                sql.AppendLine($" OFFSET {skip} ROWS ");
                            }

                            parametros.Add("@ID_CAIXA", request.Tipo == "ELEMENTO" ? request.NodeId : request.NodePai);
                            parametros.Add("@ID_TIPO", request.Tipo == "ELEMENTO" ? request.NodePai : request.NodeAvo);
                        }
                    }

                    if (sql != null)
                    {
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (request.NodeId == "#")
                                {
                                    string idRaiz = reader["ID_TIPO_RAIZ"] != DBNull.Value ? Convert.ToString(reader["ID_TIPO_RAIZ"]) : "";
                                    string desc = reader["DESC_TIPO"] != DBNull.Value ? Convert.ToString(reader["DESC_TIPO"]) : "";

                                    pontosAfetadosList.Add(new PontosAfetadosTree
                                    {
                                        id = idRaiz,
                                        children = true,
                                        text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{idRaiz}' data-tipo='TIPO_ELEMENTO' value='{idRaiz}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                        parent = "#",
                                        li_attr = new Dictionary<string, object>
                                                {
                                                    { "data-type","TIPO_ELEMENTO"},
                                                    { "data-proximo","segundo"}
                                                }
                                    });
                                }
                                else if (request.NodeProximo == "segundo")
                                {
                                    string id = reader["ID_ELEMENTO"] != DBNull.Value ? Convert.ToString(reader["ID_ELEMENTO"]) : "";
                                    long idPai = reader["ID_TIPO"] != DBNull.Value ? Convert.ToInt64(reader["ID_TIPO"]) : 0;
                                    string desc = reader["DESC_ELEMENTO"] != DBNull.Value ? Convert.ToString(reader["DESC_ELEMENTO"]) : "";
                                    bool isCaixa = reader["IS_CAIXA"] != DBNull.Value ? Convert.ToString(reader["IS_CAIXA"]) == "SIM" : false;

                                    pontosAfetadosList.Add(new PontosAfetadosTree
                                    {
                                        id = id,
                                        children = isCaixa ? true : false,
                                        text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' data-tipo='ELEMENTO' value='{id}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                        parent = $"{request.NodeId}",
                                        li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","ELEMENTO"},
                                                { "data-proximo","terceiro"},
                                                { "data-pai",$"{idPai}"},
                                            }
                                    });
                                }
                                else if (request.NodeProximo == "terceiro")
                                {
                                    string id = reader["ID_PORTA"] != DBNull.Value ? Convert.ToString(reader["ID_PORTA"]) : "";
                                    long idPai = reader["ID_ELEMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ELEMENTO"]) : 0;
                                    string desc = reader["DESC_PORTA"] != DBNull.Value ? Convert.ToString(reader["DESC_PORTA"]) : "";
                                    long idAvo = reader["ID_AVO"] != DBNull.Value ? Convert.ToInt64(reader["ID_AVO"]) : 0;

                                    if (pontosAfetadosList.Count >= 30)
                                    {
                                        string skipString = (skip == 30 ? "30" : $"{skip * 2}");
                                        pontosAfetadosList.Add(new PontosAfetadosTree
                                        {
                                            id = $"PORTAS-{skipString}",
                                            children = true,
                                            text = $"Carregar mais Portas!",
                                            parent = $"{request.NodeId}",
                                            li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","PORTA"},
                                                { "data-pai",$"{idPai}"},
                                                { "data-avo",$"{idAvo}"},
                                                { "data-skip", skip == 30 ? "30" : $"{ skip * 2}" },
                                                { "data-proximo","terceiro"},
                                            }
                                        });
                                        break;
                                    }
                                    else
                                    {
                                        pontosAfetadosList.Add(new PontosAfetadosTree
                                        {
                                            id = id,
                                            children = false,
                                            text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{id}' data-pai='{idPai}' data-avo='{idAvo}' data-tipo='PORTA' value='{id}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                            parent = request.Skip == null ? $"{request.NodeId}" : $"PORTAS-{skip}",
                                            li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","PORTA"},
                                                { "data-pai",$"{idPai}"},
                                            }
                                        });
                                    }


                                }
                            }
                        }
                    }
                }
            }

            return pontosAfetadosList.Distinct().ToList();
        }

        private async Task<List<PontosAfetadosTree>> ObterPontosAfetadosEnderecosCondominios(PontosAfetadosRequestModel request,
                                                                             PontosAfetadosManutencaoMongo obterPontosManut,
                                                                            PontosAfetadosCheck pontosManutencaoValidos)
        {
            List<PontosAfetadosTree> pontosAfetadosList = new List<PontosAfetadosTree>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = null;

                    if (request.NodeId == "#")
                    {
                        var enderecosId = obterPontosManut?.enderecosAfetados?.Ids_Enderecos?.ToList() ?? null;

                        if (enderecosId != null && enderecosId.Count > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_END, A.DESCRICAO, A.CEP, B.DESCRICAO AS BAIRRO, ");
                            sql.AppendLine(" C.DESCRICAO AS CIDADE, D.DESCRICAO AS ESTADO, E.NOME AS PAIS ");
                            sql.AppendLine(" FROM TB_ENDERECO_LOGRADOURO A ");
                            sql.AppendLine(" LEFT JOIN TB_BAIRRO B ON (B.ID = A.ID_BAIRRO) ");
                            sql.AppendLine(" LEFT JOIN TB_CIDADE C ON (C.ID = A.ID_CIDADE) ");
                            sql.AppendLine(" LEFT JOIN TB_UF D ON (D.ID = C.ID_UF) ");
                            sql.AppendLine(" LEFT JOIN TB_PAIS E ON (E.ID = D.ID_PAIS) ");
                            sql.AppendLine(" WHERE A.STATUS = '1' ");
                            sql.AppendLine($" AND A.ID IN ({String.Join(",", enderecosId)}) ");

                            if (sql != null)
                            {
                                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        if (request.NodeId == "#")
                                        {
                                            long idRaiz = reader["ID_END"] != DBNull.Value ? Convert.ToInt64(reader["ID_END"]) : 0;
                                            string descEndereco = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                            string cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "";
                                            string bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "";
                                            string cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "";
                                            string estado = reader["ESTADO"] != DBNull.Value ? Convert.ToString(reader["ESTADO"]) : "";
                                            string pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : "";
                                            string desc = $"ENDEREÇO: {descEndereco} - {bairro} - {cidade} - {estado} - {pais}";

                                            pontosAfetadosList.Add(new PontosAfetadosTree
                                            {
                                                id = idRaiz.ToString(),
                                                children = false,
                                                text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{idRaiz}' data-tipo='ENDERECO' value='{idRaiz}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                                parent = "#",
                                                li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","ENDERECO"},
                                            }
                                            });
                                        }

                                    }
                                }
                            }
                        }

                        var condominiosId = obterPontosManut?.enderecosAfetados?.Ids_Condominios?.ToList() ?? null;

                        if (condominiosId != null && condominiosId.Count > 0)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine(" SELECT A.ID AS ID_COND, A.DESCRICAO, A.LOGRADOURO, A.CEP, A.NUMERO, A.BAIRRO, ");
                            sql.AppendLine(" B.DESCRICAO AS CIDADE, C.DESCRICAO AS ESTADO, D.NOME AS PAIS ");
                            sql.AppendLine(" FROM TB_CONDOMINIO_PREDIO A ");
                            sql.AppendLine(" LEFT JOIN TB_CIDADE B ON (B.ID = A.ID_CIDADE) ");
                            sql.AppendLine(" LEFT JOIN TB_UF C ON (C.ID = B.ID_UF) ");
                            sql.AppendLine(" LEFT JOIN TB_PAIS D ON (D.ID = C.ID_PAIS) ");
                            sql.AppendLine(" WHERE A.STATUS = '1' ");

                            if (condominiosId.Count > 0)
                                sql.AppendLine($" AND A.ID IN ({String.Join(",", condominiosId)}) ");

                            if (sql != null)
                            {
                                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        if (request.NodeId == "#")
                                        {
                                            string idRaiz = reader["ID_COND"] != DBNull.Value ? Convert.ToString(reader["ID_COND"]) : "";
                                            string descCondominio = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "";
                                            string logradouro = reader["LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["LOGRADOURO"]) : "";
                                            string cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : "";
                                            string numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : "";
                                            string bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "";
                                            string cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "";
                                            string estado = reader["ESTADO"] != DBNull.Value ? Convert.ToString(reader["ESTADO"]) : "";
                                            string pais = reader["PAIS"] != DBNull.Value ? Convert.ToString(reader["PAIS"]) : "";
                                            string desc = $"{descCondominio} - {numero} - {logradouro} - {bairro} - {cidade} - {estado} - {pais}";

                                            pontosAfetadosList.Add(new PontosAfetadosTree
                                            {
                                                id = idRaiz.ToString(),
                                                children = false,
                                                text = $@"<i class='fas fa-trash text-danger custom-trash-icon icon-lixeira btnExcluirPonto' data-id='{idRaiz}' data-tipo='CONDOMINIO' value='{idRaiz}' style='margin-right: 5px; cursor: pointer;'></i> {desc}",
                                                parent = "#",
                                                li_attr = new Dictionary<string, object>
                                            {
                                                { "data-type","CONDOMINIO"},
                                            }
                                            });
                                        }

                                    }
                                }
                            }
                        }




                    }
                }
            }
            return pontosAfetadosList.Distinct().ToList();
        }

        private async Task<PontosAfetadosCheck> ChecarPontosAfetados(PontosAfetadosRequestModel request, PontosAfetadosManutencaoMongo pontosManut)
        {
            bool? existemConcentradoras = null;
            bool? existemRadius = null;
            bool? existemOlts = null;
            bool? existemSlots = null;
            bool? existemPons = null;
            bool? existemPortas = null;
            bool? existemElementos = null;
            bool? existemCondominios = null;
            bool? existemEnderecos = null;

            if (request.NodeProximo == "segundo")
            {
                existemConcentradoras = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeId))?
                                                                 .Any(c => c.idsConcentradoras != null && c.idsConcentradoras.Count > 0) : false;

                existemRadius = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeId))?
                                                          .Any(c => c.idsRadius != null && c.idsRadius.Count > 0) : false;

                existemOlts = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeId))?
                                                       .Any(c => c.idsOlts != null && c.idsOlts.Count > 0) : false;

                existemSlots = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeId))?
                                                    .Any(c => c.idsSlots != null && c.idsSlots.Count > 0) : false;

                existemPons = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodeId))?
                                                    .Any(c => c.idsPons != null && c.idsPons.Count > 0) : false;

                existemElementos = pontosManut.tiposElementosRede != null ? pontosManut.tiposElementosRede.Where(p => p.Id_Tipo == Convert.ToInt64(request.NodeId))?
                                                     .Any(c => c.ElementosRede != null && c.ElementosRede.Count > 0) : false;

                existemCondominios = pontosManut.enderecosAfetados != null ? pontosManut.enderecosAfetados.Ids_Condominios.Any() : false;

                existemEnderecos = pontosManut.enderecosAfetados != null ? pontosManut.enderecosAfetados.Ids_Enderecos.Any() : false;
            }
            if (request.NodeProximo == "terceiro" || request.NodeProximo == "quarto")
            {
                existemConcentradoras = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai))?
                                                                 .Any(c => c.idsConcentradoras != null && c.idsConcentradoras.Count > 0) : false;

                existemRadius = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai))?
                                                          .Any(c => c.idsRadius != null && c.idsRadius.Count > 0) : false;

                existemOlts = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai))?
                                                       .Any(c => c.idsOlts != null && c.idsOlts.Count > 0) : false;

                existemSlots = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai))?
                                                    .Any(c => c.idsSlots != null && c.idsSlots.Count > 0) : false;

                existemPons = pontosManut.pops != null ? pontosManut.pops.Where(p => p.IdPop == Convert.ToInt64(request.NodePai))?
                                                    .Any(c => c.idsPons != null && c.idsPons.Count > 0) : false;

                if (request.Tipo == "PORTA")
                    existemPortas = pontosManut.tiposElementosRede != null ? pontosManut.tiposElementosRede.Where(p => p.Id_Tipo == Convert.ToInt64(request.NodeAvo))?
                                                    .Any(c => c.ElementosRede != null && c.ElementosRede.Count > 0 && c.ElementosRede.Any(x => x.Ids_Portas != null && x.Ids_Portas.Count > 0)) : false;
                else if (request.Tipo == "ELEMENTO")
                    existemPortas = pontosManut.tiposElementosRede != null ? pontosManut.tiposElementosRede.Where(p => p.Id_Tipo == Convert.ToInt64(request.NodePai))?
                                .Any(c => c.ElementosRede != null && c.ElementosRede.Count > 0 && c.ElementosRede.Any(x => x.Ids_Portas != null && x.Ids_Portas.Count > 0)) : false;
            }


            var response = new PontosAfetadosCheck
            {
                existemConcentradoras = existemConcentradoras,
                existemPons = existemPons,
                existemOlts = existemOlts,
                existemRadius = existemRadius,
                existemSlots = existemSlots,
                existemPortas = existemPortas,
                existemElementos = existemElementos,
            };

            return response;
        }


        private async Task<long> CountQuantidadePontos(PontosAfetadosRequestModel request, PontosAfetadosManutencaoMongo pontosManut)
        {
            long countConcentradoras = 0;
            long countRadius = 0;
            long countOLTS = 0;
            long countSlots = 0;
            long countPons = 0;
            long countPOPS = 0;
            long countTiposElementos = 0;
            long countElementos = 0;
            long countPortas = 0;
            long countCondominios = 0;
            long countEnderecos = 0;

            if (pontosManut.pops != null)
            {
                countPOPS = pontosManut.pops != null ? pontosManut.pops.Count(x => x.IdPop != null) : 0;

                foreach (var pop in pontosManut.pops)
                {
                    countConcentradoras = countConcentradoras + (pop.idsConcentradoras != null ? pop.idsConcentradoras.Count() : 0);
                    countRadius = countRadius + (pop.idsRadius != null ? pop.idsRadius.Count() : 0);
                    countOLTS = countOLTS + (pop.idsOlts != null ? pop.idsOlts.Count() : 0);
                    countSlots = countSlots + (pop.idsSlots != null ? pop.idsSlots.Count() : 0);
                    countPons = countPons + (pop.idsPons != null ? pop.idsPons.Count() : 0);
                }
            }

            if (pontosManut.tiposElementosRede != null)
            {
                countTiposElementos = pontosManut.tiposElementosRede.Count(x => x.Id_Tipo != null);

                foreach (var elem in pontosManut.tiposElementosRede)
                {
                    countElementos = countElementos + elem.ElementosRede.Count();

                    foreach (var item in elem.ElementosRede)
                    {
                        countPortas = countPortas + (item.Ids_Portas != null ? item.Ids_Portas.Count() : 0);
                    }
                }
            }

            if (pontosManut.enderecosAfetados != null)
            {
                countCondominios = countCondominios + pontosManut.enderecosAfetados.Ids_Condominios.Count();
                countEnderecos = countEnderecos + pontosManut.enderecosAfetados.Ids_Enderecos.Count();
            }

            return countConcentradoras + countRadius + countOLTS + countSlots + countPons + countTiposElementos +
                   countCondominios + countEnderecos + countPOPS + countElementos + countPortas;
        }
        #endregion


        /// <summary>
        /// Encontrar uma Manutenção por Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Area_ManutencaoApi/FindById")]

        #region FindById

        public async Task<GetApiRetorno<Area_Cadastro_ManutencaoModels>> FindById(Int64? Id)
        {
            GetApiRetorno<Area_Cadastro_ManutencaoModels> retorno = new GetApiRetorno<Area_Cadastro_ManutencaoModels>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    using (var RepoElementosAfetados = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                    using (var RepoManutencao = new Conexao<Area_Manutencao>(sessionT5))
                    {
                        retorno.Objeto = GetViewObj(await RepoManutencao.selectFirst(p => p.Id == Id));
                        retorno.Objeto.Ids_Nodes = String.Empty;
                        var area_manutencao_elementos = await RepoElementosAfetados.select(p => p.AreaManutencao.Id == Id);
                        if (area_manutencao_elementos.Count() > 0)
                        {
                            foreach (var node in area_manutencao_elementos)
                            {
                                if (!String.IsNullOrEmpty(node.Id_Node))
                                {
                                    if (node.Id_Node.Contains('-'))
                                        retorno.Objeto.Ids_Nodes = retorno.Objeto.Ids_Nodes.Insert(0, node.Id_Node + ",");
                                }
                            }
                            retorno.Objeto.Ids_Nodes = retorno.Objeto.Ids_Nodes.TrimEnd(',');
                        }
                    }
                    retorno.Ok = true;

                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }

        #endregion

        #region GetViewObj

        private Area_Cadastro_ManutencaoModels GetViewObj(Area_Manutencao banco)
        {
            var horaAtual = DateTime.Now;
            return new Area_Cadastro_ManutencaoModels
            {

                Id = banco.Id,

                Descricao = banco.Descricao,

                DataFinal = banco.Data_HoraFinal.HasValue ? banco.Data_HoraFinal.Value.Date.ToString("dd/MM/yyyy") : String.Empty,
                DataInicial = banco.Data_HoraInicial.HasValue ? banco.Data_HoraInicial.Value.Date.ToString("dd/MM/yyyy") : String.Empty,
                HoraFinal = banco.Data_HoraFinal.HasValue ? banco.Data_HoraFinal.Value.ToString("HH:mm") : String.Empty,
                HoraInicial = banco.Data_HoraInicial.HasValue ? banco.Data_HoraInicial.Value.ToString("HH:mm") : String.Empty,
                Contrato = banco.CodCt,
                Projeto = banco.Projeto,
                Status = banco.Status == Status_Default.Ativo

            };

        }
        #endregion


        /// <summary>
        /// Gravação de uma nova Manutenção de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/Gravar")]

        #region Gravar

        public async Task<PostApiRetorno<Int64?>> Gravar([FromBody] ParamWebApi<Area_Cadastro_ManutencaoModels> request)
        {
            PostApiRetorno<Int64?> retorno = new PostApiRetorno<Int64?>();
            List<long> Ids = new List<long>();
            List<String> node_Id = new List<String>();

            if (!String.IsNullOrEmpty(request.Objeto.Ids_Elementos_Afetados))
            {
                string[] parts = request.Objeto.Ids_Elementos_Afetados.Split(',');

                if (parts.Length > 0)
                {
                    Ids.Add(Int64.Parse(parts[0]));
                }
            }

            if (!String.IsNullOrEmpty(request.Objeto.Ids_Nodes))
            {
                string[] nodes = request.Objeto.Ids_Nodes.Split(',');
                if (nodes.Length > 0)
                {
                    node_Id.Add(nodes[0]);
                }
            }

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                    using (var repositorio = new Conexao<Area_Manutencao>(sessionT5))
                    using (var repoArea = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                    {
                        Area_Manutencao area = null;
                        Area_Manutencao areaAnterior = null;
                        if (request.Objeto.Id != null)
                        {
                            area = await repositorio.selectFirst(p => p.Id == request.Objeto.Id);
                            areaAnterior = (Area_Manutencao)area.Clone();
                        }
                        if (area == null)
                        {
                            area = new Area_Manutencao();
                        }

                        BindviewToObj(area, request.Objeto);


                        List<Area_Manutencao_Elementos> elementos_afetadosView = Ids.Zip(node_Id, (Id, param) =>
                        {
                            return new Area_Manutencao_Elementos
                            {
                                AreaManutencao = area,
                                Elemento_de_Rede = repoElemento.selectFirst(element => element.Id == Id).Result,
                                Id_Node = param,
                                Data_HoraInicial = area.Data_HoraInicial,
                                Data_HoraFinal = area.Data_HoraFinal,
                                Status = area.Status,
                                Descricao = area.Descricao
                            };
                        }).ToList();


                        if (elementos_afetadosView.Any())
                        {

                            foreach (var item in elementos_afetadosView)
                            {
                                var hasManutencaoElemento = await HasManutencaoElemento(area.Id, item.Elemento_de_Rede.Id);

                                if (hasManutencaoElemento.Ok)
                                {
                                    await repositorio.SaveOrUpdate(area);
                                    await repoArea.Update(item, hasManutencaoElemento.IdObjeto);

                                    retorno.IdObjeto = area.Id;
                                    retorno.Ok = true;
                                }
                                else
                                {
                                    await repositorio.SaveOrUpdate(area);
                                    await repoArea.SaveOrUpdate(elementos_afetadosView);

                                    retorno.IdObjeto = area.Id;
                                    retorno.Ok = true;
                                }
                            }
                        }
                        else
                        {
                            await repositorio.SaveOrUpdate(area);

                            var areaElemento = await repoArea.select(x => x.AreaManutencao.Id == area.Id);

                            areaElemento.ForEach(x =>
                            {
                                x.AreaManutencao = area;
                                x.DataHoraAlteracao = area.DataHoraAlteracao;
                                x.DataHoraCriacao = area.DataHoraCriacao;
                                x.Data_HoraFinal = area.Data_HoraFinal;
                                x.Data_HoraInicial = area.Data_HoraInicial;
                                x.Status = area.Status;
                            });

                            await repoArea.SaveOrUpdate(areaElemento);


                            retorno.IdObjeto = area.Id;
                            retorno.Ok = true;
                        }

                        if ((area.Data_HoraInicial.HasValue && area.Data_HoraFinal.HasValue) && area.Data_HoraFinal != areaAnterior?.Data_HoraFinal)
                        {
                            this.BackgroundRoutine(area);
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = $"{erro.Message}{erro.InnerException?.Message}";
            }
            return retorno;
        }
        #endregion

        #region VerificaManutencaoElemento
        private async Task<PostApiRetorno<Int64?>> HasManutencaoElemento(Int64 areaManutencaoId, long? elementoAfetadoId)
        {
            var retorno = new PostApiRetorno<Int64?>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {

                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    sql.AppendLine("SELECT AME.ID");
                    sql.AppendLine("FROM TB_AREA_MANUTENCAO_ELEMENTOS AME ");
                    sql.AppendLine("WHERE AME.ID_AREA_MANUTENCAO = @AREAMANU ");
                    parametros.Add("@AREAMANU", areaManutencaoId);
                    sql.AppendLine("AND AME.ID_ELEMENTOS_AFETADOS = @ELEMID ");
                    parametros.Add("@ELEMID", elementoAfetadoId);
                    sql.AppendLine("AND AME.STATUS = 1");

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.IdObjeto = Convert.ToInt64(reader["ID"]);
                                retorno.Ok = retorno.IdObjeto != null ? true : false;
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
        private void BackgroundRoutine(Area_Manutencao area)
        {

            var tempoAteExpirar = area.Data_HoraFinal.Value - DateTime.Now;

            //     --      Notificação ClientesAfestados
            //BackgroundJob.Schedule(() => new Area_ManutencaoTask().NotificaçoaClientesAfetadosd(area.Id), area.Data_HoraInicial.Value);
            BackgroundJob.Schedule(() => new Area_ManutencaoTask().AtivaAutomatico(area.Id), area.Data_HoraInicial.Value);
            BackgroundJob.Schedule(() => new Area_ManutencaoTask().DesativaAutomatico(area.Id), tempoAteExpirar);
        }

        #region BindviewToObj
        private void BindviewToObj(Area_Manutencao area, Area_Cadastro_ManutencaoModels view)
        {


            area.Descricao = view.Descricao;
            area.Status = view.Id == null ? Status_Default.Ativo : view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;

            area.CodCt = view.Contrato;

            if (view.Projeto == null)
            {
                area.Projeto = area.Projeto;
            }
            else
            {
                area.Projeto = view.Projeto;
            }

            area.UsuarioCriacao = User.Identity.GetUserId();
            area.UsuarioAlteracao = area.Id != 0 ? User.Identity.GetUserId() : null;

            var horaini = TimeSpan.Parse(view.HoraInicial);
            var horafim = TimeSpan.Parse(view.HoraFinal);

            area.Data_HoraInicial = Convert.ToDateTime(view.DataInicial).Add(horaini);
            area.Data_HoraFinal = Convert.ToDateTime(view.DataFinal).Add(horafim);

        }
        #endregion

        /// <summary>
        /// Deleta registros por Id da Manutenção
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                Area_Manutencao Area_manut = null;
                Area_Manutencao_Elementos Area_Elementos = null;

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoArea_Elementos = new Conexao<Area_Manutencao_Elementos>(sessionT5))
                    using (var repositorio = new Conexao<Area_Manutencao>(sessionT5))
                    {
                        Area_manut = await repositorio.selectFirst(p => p.Id == value.Objeto);
                        Area_Elementos = await repoArea_Elementos.selectFirst(prop => prop.AreaManutencao == Area_manut);
                        await repositorio.Remove(Area_manut);

                        retorno.IdObjeto = Area_manut.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
            }
            return retorno;

        }
        #endregion

        [HttpPost()]
        [Route("api/Area_ManutencaoApi/DesativaAutomatico")]
        #region DesativaAutomatico
        public async Task<GetApiRetorno<List<Area_Manutencao>>> DesativaAutomatico()
        {
            var retorno = new GetApiRetorno<List<Area_Manutencao>>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoArea = new Conexao<Area_Manutencao>(sessionT5))
                {
                    DateTime? data_vencimento = DateTime.Now;

                    IList<Area_Manutencao> lista = await repoArea.select(p => p.Data_HoraFinal < data_vencimento);
                    var alterar = new AlterarStatusViewModel
                    {
                        ids = lista.Select(param => param.Id).ToList(),
                        status = Convert.ToInt16(Status_Default.Inativo),
                    };
                    await AlterarStatus(alterar);
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// Alterar o Status de uma Área de Manutencao
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/AlterarStatus")]

        #region AlterarStatus
        public async Task<PostApiRetorno<Area_ManutencaoModels>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Area_ManutencaoModels> retorno = new PostApiRetorno<Area_ManutencaoModels>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {

                StringBuilder sqlUp = new StringBuilder(" UPDATE TB_AREA_MANUTENCAO ");
                StringBuilder sql = new StringBuilder();
                using (var session = HibernateUtil.GetSessionT5())
                {
                    String ids = $"({string.Join(",", request.ids)})";
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlUp.ToString());
                        sql.AppendLine($"  SET STATUS = @STATUS WHERE ID IN {ids}  ");


                        parametros.Add("@STATUS", request.status);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }

                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Elemento_RedeApi/GetTreeView_Manutencao")]
        #region TreeView_Manutencao
        public async Task<GetApiRetorno<List<ITreeViewModel>>> GetTreeView_Manutencao(String Contrato = "", String Projeto = "")
        {
            var retorno = new GetApiRetorno<List<ITreeViewModel>>
            {
                Objeto = new List<ITreeViewModel>()
            };
            List<String> Condicoes = new List<string>();

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            try
            {
                //D.ID AS ID_AREA, D.DESCRICAO AS DESCRICAO_AREA
                //  LEFT JOIN TB_AREA_COBERTURA D                 ON A.CODCT = B.CONTRATO

                List<Elemento_RedeManutencaoViewModel> listamanutencaoObject = new List<Elemento_RedeManutencaoViewModel>();
                Elemento_RedeManutencaoViewModel manutencaoObject = null;

                if (!String.IsNullOrEmpty(Contrato))
                {
                    Condicoes.Add("A.CONTRATO = @CODCT");
                    parametros.Add("@CODCT", Contrato);
                }

                if (!String.IsNullOrEmpty(Projeto))
                {
                    Condicoes.Add("A.PROJETO = @PROJETO");
                    parametros.Add("@PROJETO", Projeto);
                }

                StringBuilder sqlWhere = new StringBuilder(),

                              sqlFrom = new StringBuilder(@"
                                           FROM TB_ELEMENTO_REDE A LEFT JOIN TB_TIPO_ELEMENTO_REDE B
                                           ON A.ID_TIPO = B.ID
                                           LEFT JOIN TB_MODELO_ELEMENTO_REDE C
                                           ON B.ID_MODELO = C.ID    
                                                          "),
                              sql = new StringBuilder();

                using (var conexao = new ConexaoMongoGeography())
                using (var session = HibernateUtil.GetSessionT5())
                {
                    parametros.Add("@OBJ_CONEXAO", conexao);

                    using (var com = session.Connection.CreateCommand())
                    {
                        if (parametros.Count > 0)
                        {
                            sqlWhere.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                        }

                        sql.AppendLine(@" SELECT A.ID, A.DESCRICAO,A.UF,A.CIDADE,A.BAIRRO, A.CONTRATO, A.PROJETO, B.ID ID_TIPO, B.DESCRICAO DESCRICAO_TIPO,
                                          C. ID ID_MODELO, C. DESCRICAO DESCRICAO_MODELO");
                        sql.AppendFormat("{0} \r\n {1}", sqlFrom.ToString(), sqlWhere.ToString());
                        //sql.AppendLine("UNION");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                manutencaoObject = new Elemento_RedeManutencaoViewModel
                                {
                                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "Sem Bairro vinculado",
                                    Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]).ToUpper() : "Sem Cidade vinculada",
                                    Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "Sem UF vinculada",
                                    Contrato = reader["CONTRATO"] != DBNull.Value ? Convert.ToString(reader["CONTRATO"]) : Contrato,

                                    Elemento = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : null,
                                    Tipo_Elemento = reader["ID_TIPO"] != DBNull.Value ? Convert.ToString(reader["ID_TIPO"]) : null,
                                    Modelo_Elemento = reader["ID_MODELO"] != DBNull.Value ? Convert.ToString(reader["ID_MODELO"]) : null,
                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : Projeto,

                                    Descricao_Elemento = reader["DESCRICAO"] != DBNull.Value ? "Elemento: " + Convert.ToString(reader["DESCRICAO"]) : "Elemento sem Descrição",
                                    Descricao_Tipo_Elemento = reader["DESCRICAO_TIPO"] != DBNull.Value ? "Tipo de Elemento: " + Convert.ToString(reader["DESCRICAO_TIPO"]) : null,
                                    Descricao_Modelo_Elemento = reader["DESCRICAO_MODELO"] != DBNull.Value ? "Modelo: " + Convert.ToString(reader["DESCRICAO_MODELO"]) : null,
                                };

                                listamanutencaoObject.Add(manutencaoObject);

                            }
                        }
                    }
                    parametros.Add("@OBJ_AGRUPAMENTO", listamanutencaoObject);

                    retorno.Objeto.AddRange(await Agrupamento_Itens(parametros));
                }

                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }

        public async Task<List<ITreeViewModel>> Agrupamento_Itens(Dictionary<String, Object> parametros)
        {
            List<ITreeViewModel> retorno = new List<ITreeViewModel>();

            var ObjRegiao = new List<Elemento_RedeTreeViewModel>();
            var ObjArea = new List<Area_ManutencaoTreeViewModel>();
            IEnumerable<Elemento_RedeManutencaoViewModel> listamanutencaoObject = (List<Elemento_RedeManutencaoViewModel>)parametros["@OBJ_AGRUPAMENTO"];

            using (var repoAreas = new RepositoryMongo<Area_CoberturaMongo>((ConexaoMongoGeography)parametros["@OBJ_CONEXAO"]))
            using (var AC_api = new Area_CoberturaApicontroller())
            {
                List<Area_CoberturaMongo> bancoarea = await repoAreas.FindAll();

                foreach (var area in bancoarea)
                {
                    var elements = (await AC_api.GetElementosInArea(area)).ToList();

                    if (parametros.ContainsKey("@CODCT") && parametros.ContainsKey("@PROJETO"))
                    {
                        elements = elements.Where(element =>
                                    element.Contrato == parametros["@CODCT"] && element.Projeto == parametros["@PROJETO"])
                             .ToList();

                    }
                    var elementsByArea = elements.Select(element =>
                    {
                        return new Area_ManutencaoTreeViewModel
                        {
                            Id = $"{area.Id}-{element.Id}",
                            Text = element.Descricao,

                            Contrato = element.Contrato,
                            Projeto = element.Projeto,
                            Tipo = TipoAgrupamento.ElementoRede,
                            factor = true,
                        };
                    }).ToList();

                    if (elementsByArea.Count > 0)
                    {
                        ObjArea.Add(new Area_ManutencaoTreeViewModel
                        {
                            Id = "Area Cobertura" + "-" + area.Id,
                            Text = "Area Cobertura: " + area.Descricao,
                            Children = elementsByArea
                        });
                    }
                }
            }

            IEnumerable<IGrouping<KeyValuePair<String, String>, Elemento_RedeManutencaoViewModel>> agrupamento_Uf = listamanutencaoObject
                .GroupBy(group => new KeyValuePair<String, string>("Uf", group.Uf));

            foreach (var itemUf in agrupamento_Uf)
            {
                var AgrupamentoCidades = itemUf.ToList().GroupBy(group => new KeyValuePair<String, String>("Cidade", group.Cidade));
                List<Elemento_RedeTreeViewModel> element_cidade = new List<Elemento_RedeTreeViewModel>();

                foreach (var itemCidade in AgrupamentoCidades)
                {
                    var AgrupamentoBairros = itemCidade.ToList().GroupBy(group => new KeyValuePair<String, String>("Bairro", group.Bairro));
                    List<Elemento_RedeTreeViewModel> element_bairro = new List<Elemento_RedeTreeViewModel>();

                    foreach (var itemBairro in AgrupamentoBairros)
                    {
                        var itemElementos = itemBairro.ToList();

                        Elemento_RedeTreeViewModel Bairro = new Elemento_RedeTreeViewModel
                        {
                            Id = $"{itemUf.Key.Value} - {itemCidade.Key.Value} - {itemBairro.Key.Value}",
                            Text = itemBairro.Key.Value,
                            Children = itemElementos.Select(param =>
                            {
                                return new Elemento_RedeTreeViewModel
                                {
                                    Id = $"{itemUf.Key.Value} - {itemCidade.Key.Value} - {itemBairro.Key.Value} - {param.Elemento}",
                                    Text = param.Descricao_Elemento,
                                    Contrato = param.Contrato,
                                    Projeto = param.Projeto,
                                    factor = true,
                                    Tipo = TipoAgrupamento.ElementoRede,
                                };
                            }).ToList()
                        };

                        element_bairro.Add(Bairro);

                    }

                    Elemento_RedeTreeViewModel Cidade = new Elemento_RedeTreeViewModel
                    {
                        Id = $"{itemUf.Key.Value} - {itemCidade.Key.Value}",
                        Text = itemCidade.Key.Value,
                        Children = element_bairro,
                    };

                    element_cidade.Add(Cidade);

                }
                Elemento_RedeTreeViewModel Uf = new Elemento_RedeTreeViewModel
                {
                    Id = itemUf.Key.Value,
                    Text = itemUf.Key.Value,
                    Children = element_cidade,

                };

                ObjRegiao.Add(Uf);

            }

            var listamanutencaoObjectSemRegiao = listamanutencaoObject
                .Where(p => agrupamento_Uf
                            .All(agrup => !agrup
                                .Any(item => item.Elemento == p.Elemento)));

            IEnumerable<IGrouping<KeyValuePair<string, (String modelo, String desc_modelo)>, Elemento_RedeManutencaoViewModel>> agrupamentoModelo = listamanutencaoObjectSemRegiao
            .GroupBy(group => new KeyValuePair<string, (String, String)>("Modelo_Elemento_Rede", (group.Modelo_Elemento, group.Descricao_Modelo_Elemento)));

            foreach (var item in agrupamentoModelo)
            {

                List<Elemento_RedeTreeViewModel> ListaAgrupamentoTipo = new List<Elemento_RedeTreeViewModel>();

                IEnumerable<IGrouping<KeyValuePair<string, (String tipo, String desc_tipo)>, Elemento_RedeManutencaoViewModel>> AgrupamentoTipo = item.ToList()
                    .GroupBy(group => new KeyValuePair<string, (string, string)>("Tipo_Elemento_Rede", (group.Tipo_Elemento, group.Descricao_Tipo_Elemento)));


                foreach (var itemTipo in AgrupamentoTipo)
                {
                    List<Elemento_RedeManutencaoViewModel> objetosDoAgrupamentoTipo = itemTipo.ToList();
                    Elemento_RedeTreeViewModel tipo = new Elemento_RedeTreeViewModel
                    {
                        Id = item.Key.Value.modelo + "-" + itemTipo.Key.Value.tipo,
                        Text = itemTipo.Key.Value.desc_tipo,
                        Children = objetosDoAgrupamentoTipo.Select(s => new Elemento_RedeTreeViewModel
                        {
                            Id = $"{item.Key.Value.modelo}-{itemTipo.Key.Value.tipo}-{s.Elemento}",
                            factor = true,
                            Tipo = TipoAgrupamento.ElementoRede,
                            Text = s.Descricao_Elemento,

                        }).ToList()
                    };

                    ListaAgrupamentoTipo.Add(tipo);
                }

                Elemento_RedeTreeViewModel Obj = new Elemento_RedeTreeViewModel
                {
                    Id = item.Key.Value.modelo,
                    Text = item.Key.Value.desc_modelo,
                    Children = ListaAgrupamentoTipo,
                };

                retorno.Add(Obj);

            }
            if (ObjRegiao.Count() > 0)
            {
                Elemento_RedeTreeViewModel Obj2 = new Elemento_RedeTreeViewModel
                {
                    Id = "Regiao Mencionada",
                    Text = "UF",
                    Children = ObjRegiao,
                };

                retorno.Add(Obj2);
            }

            if (ObjArea.Count > 0)
            {
                Area_ManutencaoTreeViewModel Obj3 = new Area_ManutencaoTreeViewModel
                {
                    Id = "Area de Cobertura",
                    Text = "Areas de Cobertura",
                    Children = ObjArea,
                };

                retorno.Add(Obj3);
            }


            return retorno;

        }

        #endregion



        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/GetManutencaoRede")]
        #region GetManutencaoRedeByCodigoContrato
        public async Task<PostApiRetorno<string>> GetManutencaoRede(String codCT)
        {
            var retorno = new PostApiRetorno<string>();
            Area_Manutencao areaDados = new Area_Manutencao();
            Int64 totalCount = 0;

            StringBuilder sqlFrom = new StringBuilder("FROM TB_AREA_MANUTENCAO AM ");

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        StringBuilder sql = new StringBuilder("SELECT COUNT(AM.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" WHERE AM.CODCT = @CODCT ");
                        parametros.Add("@CODCT", codCT);
                        sql.AppendLine("AND AM.STATUS = 1");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                totalCount = Convert.ToInt64(reader["CO"]);
                        }
                    }

                    if (totalCount > 0)
                    {
                        using (var com = sessionT5.Connection.CreateCommand())
                        {
                            Dictionary<string, object> parametros = new Dictionary<string, object>();

                            StringBuilder sql = new StringBuilder("SELECT AM.DATA_HORA_FIM, AM.DATA_HORA_INICIAL ");
                            sql.AppendLine(sqlFrom.ToString());
                            sql.AppendLine("WHERE AM.STATUS = 1 ");
                            sql.AppendLine("AND AM.CODCT = @CODCT ");
                            parametros.Add("@CODCT", codCT);

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    areaDados.Data_HoraFinal = Convert.ToDateTime(reader["DATA_HORA_FIM"]).ToLocalTime();
                                    areaDados.Data_HoraInicial = Convert.ToDateTime(reader["DATA_HORA_INICIAL"]).ToLocalTime();
                                }
                            }
                        }
                        retorno.Ok = true;
                        retorno.Mensagem = $"A área está em manutenção prevista com ínicio {areaDados.Data_HoraInicial} e fim {areaDados.Data_HoraFinal}";
                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = $"Não foi encontrada uma área de manutenção com contrato informado";
                    }
                }
                catch (Exception e)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = $"{e.Message}{e.InnerException?.Message}";
                }
            }

            return retorno;
        }
        #endregion

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/ValidarAreaManutencaoCli")]
        #region ValidarAreaManutencaoCli
        public async Task<bool> ValidarAreaManutencaoCli(VerificaAreaModels value)
        {
            //CONTAGEM SE ENCONTRADO PASSADO OS VALORES
            int total_count = 0;

            //recebe em timespan, converte para date e converte para o local
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(value.Data)).LocalDateTime;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    sql.AppendLine("SELECT COUNT(AM.ID) AS CO");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA ERP ");
                    sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE ER ");
                    sql.AppendLine("ON ERP.ID_CAIXA =  ER.ID");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO_ELEMENTOS AME ");
                    sql.AppendLine("ON AME.ID_ELEMENTOS_AFETADOS = ERP.ID_CAIXA ");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO AM ");
                    sql.AppendLine("ON AM.ID = AME.ID_AREA_MANUTENCAO ");

                    sql.AppendLine("WHERE ERP.CLIENTE_PLANO_PRODUTO = @CLIENTE ");
                    parametros.Add("@CLIENTE", value.ClientePlanoId);

                    sql.AppendLine(" AND AME.STATUS = 1");

                    sql.AppendLine("AND @DATA BETWEEN AM.DATA_HORA_INICIAL AND AM.DATA_HORA_FIM ");
                    parametros.Add("@DATA", date);

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                //caso maior que zero, há uma area de manutencao com o STATUS = 1, e a DATA INFORMADA menor que a hora final da manutenção.
                return total_count > 0;
            }
        }
        #endregion

        #region Mobile
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Area_ManutencaoApi/ValidarAreaManutencaoCliMobile")]
        public async Task<bool> ValidarAreaManutencaoCliMobile(VerificaAreaModels value)
        {
            //CONTAGEM SE ENCONTRADO PASSADO OS VALORES
            int total_count = 0;

            //recebe em timespan, converte para date e converte para o local
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(value.Data)).LocalDateTime;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    sql.AppendLine("SELECT COUNT(AM.ID) AS CO");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA ERP ");
                    sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE ER ");
                    sql.AppendLine("ON ERP.ID_CAIXA =  ER.ID");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO_ELEMENTOS AME ");
                    sql.AppendLine("ON AME.ID_ELEMENTOS_AFETADOS = ERP.ID_CAIXA ");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO AM ");
                    sql.AppendLine("ON AM.ID = AME.ID_AREA_MANUTENCAO ");

                    sql.AppendLine("WHERE ERP.CLIENTE_PLANO_PRODUTO = @CLIENTE ");
                    parametros.Add("@CLIENTE", value.ClientePlanoId);

                    sql.AppendLine(" AND AME.STATUS = 1");

                    sql.AppendLine("AND @DATA BETWEEN AM.DATA_HORA_INICIAL AND AM.DATA_HORA_FIM ");
                    parametros.Add("@DATA", date);

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                //caso maior que zero, há uma area de manutencao com o STATUS = 1, e a DATA INFORMADA menor que a hora final da manutenção.
                return total_count > 0;
            }
        }


        [HttpPost()]
        [Route("api/Area_ManutencaoApi/ValidarAreaManutencaoCliConnect")]
        public async Task<bool> ValidarAreaManutencaoCliConnect(VerificaAreaModels value)
        {
            //CONTAGEM SE ENCONTRADO PASSADO OS VALORES
            int total_count = 0;

            //recebe em timespan, converte para date e converte para o local
            DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(Int64.Parse(value.Data)).LocalDateTime;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    sql.AppendLine("SELECT COUNT(AM.ID) AS CO");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA ERP ");
                    sql.AppendLine("INNER JOIN TB_ELEMENTO_REDE ER ");
                    sql.AppendLine("ON ERP.ID_CAIXA =  ER.ID");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO_ELEMENTOS AME ");
                    sql.AppendLine("ON AME.ID_ELEMENTOS_AFETADOS = ERP.ID_CAIXA ");
                    sql.AppendLine("INNER JOIN TB_AREA_MANUTENCAO AM ");
                    sql.AppendLine("ON AM.ID = AME.ID_AREA_MANUTENCAO ");

                    sql.AppendLine("WHERE ERP.CLIENTE_PLANO_PRODUTO = @CLIENTE ");
                    parametros.Add("@CLIENTE", value.ClientePlanoId);

                    sql.AppendLine(" AND AME.STATUS = 1");

                    sql.AppendLine("AND @DATA BETWEEN AM.DATA_HORA_INICIAL AND AM.DATA_HORA_FIM ");
                    parametros.Add("@DATA", date);

                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                total_count = Convert.ToInt32(reader["CO"]);



                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }







                //caso maior que zero, há uma area de manutencao com o STATUS = 1, e a DATA INFORMADA menor que a hora final da manutenção.
                return total_count > 0;
            }
        }
        #endregion
    }


}