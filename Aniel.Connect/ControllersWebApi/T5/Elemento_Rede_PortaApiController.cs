using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.Geosite.Models;
using Aniel.Connect.Integracoes.Geosite.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Newtonsoft.Json;
using NHibernate;
using OpenXmlPowerTools;
using RestSharp;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Model;
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
using Situacao_Porta = Sinapse.Model.enumerados.Situacao_Porta;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Elemento_Rede_PortaApiController : BaseApiController
    {
        /// <summary>
        /// Listagem das Portas dos Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/GetAll")]
        #region GetAll
        //
        public async Task<DataTableResponse<Elemento_Rede_PortaListModel>> GetAll(Elemento_Rede_PortaRequest request)
        {
            List<Elemento_Rede_PortaListModel> retorno = new List<Elemento_Rede_PortaListModel>();
            var recordsTotal = 0;
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ELEMENTO_REDE_PORTA A");
                    StringBuilder sqlWhere = new StringBuilder();

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sqlCount = new StringBuilder();
                        sqlCount.AppendLine("SELECT COUNT (A.ID) CO")
                                .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A LEFT JOIN TB_ELEMENTO_REDE B")
                           .AppendLine("ON A.ID_CAIXA = B.ID");

                        #region Condicoes 
                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            condicoes.Add("B.DESCRICAO LIKE @SEARCH ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        }
                        if (!String.IsNullOrEmpty(request.Id))
                        {
                            condicoes.Add("A.ID = @ID");
                            parametros.Add("@ID", request.Id);
                        }
                        if (request.Status.Count != 0)
                        {
                            condicoes.Add($"(A.STATUS IN ({String.Join(", ", request.Status)}))");
                            parametros.Add("@STATUS", request.Status);
                        }
                        if (!String.IsNullOrEmpty(request.Bairro))
                        {
                            condicoes.Add("B.BAIRRO = @BAIRRO");
                            parametros.Add("@BAIRRO", request.Bairro);
                        }
                        if (!String.IsNullOrEmpty(request.Caixa))
                        {
                            condicoes.Add("A.ID_CAIXA LIKE @CAIXA");
                            parametros.Add("@CAIXA", request.Caixa);
                        }
                        if (!String.IsNullOrEmpty(request.Cidade))
                        {
                            condicoes.Add("B.CIDADE = @CIDADE");
                            parametros.Add("@CIDADE", request.Cidade);
                        }
                        if (!String.IsNullOrEmpty(request.Contrato))
                        {
                            condicoes.Add("B.CONTRATO = @CONTRATO");
                            parametros.Add("@CONTRATO", request.Contrato);
                        }
                        //if (!String.IsNullOrEmpty(request.Login))    
                        //{
                        //    condicoes.Add("B.LOGIN = @LOGIN");        
                        //    parametros.Add("@LOGIN", request.Login);
                        //}
                        if (!String.IsNullOrEmpty(request.Olt))
                        {
                            condicoes.Add("B.OLT = @OLT");
                            parametros.Add("@OLT", request.Olt);
                        }
                        if (!String.IsNullOrEmpty(request.Pop))
                        {
                            condicoes.Add("B.POP = @POP");
                            parametros.Add("@POP", request.Pop);
                        }
                        if (!String.IsNullOrEmpty(request.Porta))
                        {
                            condicoes.Add("B.PORTA = @PORTA");
                            parametros.Add("@PORTA", request.Porta);
                        }
                        if (!String.IsNullOrEmpty(request.Projeto))
                        {
                            condicoes.Add("B.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.Projeto);
                        }
                        if (!String.IsNullOrEmpty(request.Uf))
                        {
                            condicoes.Add("B.UF = @UF");
                            parametros.Add("@UF", request.Uf);
                        }
                        if (request.IdTipoElemento.HasValue)
                        {
                            condicoes.Add("B.ID_TIPO = @TIPO");
                            parametros.Add("@TIPO", request.IdTipoElemento.Value);
                        }

                        if (condicoes.Count > 0)
                        {
                            sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                            sqlCount.AppendLine(sqlWhere.ToString());
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.PORTA, A.LOGIN, A.STATUS, B.DESCRICAO, B.CIDADE, B.BAIRRO, B.CONTRATO, B.PROJETO, B.POP, B.OLT")
                           .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A LEFT JOIN TB_ELEMENTO_REDE B")
                           .AppendLine("ON A.ID_CAIXA = B.ID");
                        //search by Description


                        sql.AppendLine(sqlWhere.ToString());


                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Elemento_Rede_PortaListModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    Caixa = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    Porta = reader["PORTA"] != DBNull.Value ? Convert.ToInt32(reader["PORTA"]) : 0,
                                    Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Situacao_Porta>(Convert.ToString(reader["STATUS"])) : "-",
                                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : "-",

                                    Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : "-",
                                    Contrato = reader["CONTRATO"] != DBNull.Value ? Convert.ToString(reader["CONTRATO"]) : "-",
                                    Login = reader["LOGIN"] != DBNull.Value ? Convert.ToString(reader["LOGIN"]) : "-",
                                    Olt = reader["OLT"] != DBNull.Value ? Convert.ToString(reader["OLT"]) : "-",
                                    Pop = reader["POP"] != DBNull.Value ? Convert.ToString(reader["POP"]) : "-",

                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "-"
                                });
                            }
                        }

                        return new DataTableResponse<Elemento_Rede_PortaListModel>
                        {
                            recordsFiltered = recordsTotal,
                            draw = request.draw,
                            data = retorno,
                            error = ""
                        };
                    }
                }
            }
            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                return new DataTableResponse<Elemento_Rede_PortaListModel>
                {
                    recordsFiltered = 0,
                    error = erro.Message
                };
            }
        }
        #endregion

        [HttpGet()]
        [Route("api/Elemento_Rede_PortaApi/GetListByCaixa")]
        #region GetListByCaixa
        public async Task<GetApiRetorno<List<Elemento_Rede_PortaGetListModel>>> GetListByCaixa(long IdCaixa)
        {
            var retorno = new GetApiRetorno<List<Elemento_Rede_PortaGetListModel>>
            {
                Objeto = new List<Elemento_Rede_PortaGetListModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoDoor = new Conexao<Elemento_Rede_Porta>(session))
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        List<String> queryParams = new List<String>();

                        StringBuilder sql = new StringBuilder(),
                                      sqlFrom = new StringBuilder(),
                                      sqlWhere = new StringBuilder(),
                                      sqlOrder = new StringBuilder();

                        sqlOrder.AppendLine("ORDER BY A.PORTA");
                        sqlWhere.AppendLine($"Where A.ID_CAIXA = '{IdCaixa}'");
                        sqlFrom.AppendLine("FROM TB_ELEMENTO_REDE_PORTA A");

                        queryParams.AddRange(new List<String> { sqlFrom.ToString(), sqlWhere.ToString(), sqlOrder.ToString() });

                        sql.AppendLine($"SELECT A.ID, A.ID_CAIXA, A.PORTA, A.STATUS {String.Join(" \r\n ", queryParams)}");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var objeto = new Elemento_Rede_PortaGetListModel
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    //Login=  Convert.ToString(reader["LOGIN"]),

                                    Caixa = Convert.ToString(reader["ID_CAIXA"]),
                                    Porta = Convert.ToInt32(reader["PORTA"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription((Situacao_Porta)(reader["STATUS"])),
                                };
                                retorno.Objeto.Add(objeto);
                            }
                        }
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
            }

            return retorno;
        }
        #endregion

        /// <summary>
        /// GetCombo dos registros das portas dos Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/GetCombo")]
        #region GetCombo
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>();
            retorno.results = new List<IObjComboResult<String>>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT ")
                              .AppendLine(" A.ID, A.STATUS")
                              .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A");

                        if (request.soativos)
                        {
                            //condicoes.Add($"A.STATUS = 1");
                        }

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            condicoes.Add($"A.ID = '{request.id}'");
                        }

                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            condicoes.Add($"A.ID_CAIXA = '{request.idpai}'");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                        }

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var Obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = HtmlEnumExtensions.GetEnumDescription<Situacao_Porta>(Convert.ToString(reader["STATUS"]))
                                };
                                retorno.results.Add(Obj);
                            }
                        }

                    }
                }

                return retorno;
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                throw erro;
            }
        }
        #endregion

        /// <summary>
        /// GetCombo dos registros das portas dos Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/GetComboPorta")]
        #region GetCombo
        public async Task<ObjCombo<String>> GetComboPorta(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>();
            retorno.results = new List<IObjComboResult<String>>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT ")
                              .AppendLine(" A.ID, A.PORTA")
                              .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A ");
                        if (request.soativos)
                        {
                            condicoes.Add($"A.STATUS = 1");
                        }

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            condicoes.Add($" A.ID = '{request.id}'");
                        }

                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            condicoes.Add($"A.ID_CAIXA = '{request.idpai}'");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                        }

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var Obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["PORTA"]),
                                    //Text = HtmlEnumExtensions.GetEnumDescription<Situacao_Porta>(Convert.ToString(reader["STATUS"]))
                                };
                                retorno.results.Add(Obj);
                            }
                        }

                    }
                }

                return retorno;
            }
            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                throw erro;
            }
        }
        #endregion

        /// <summary>
        /// GetCombo dos registros das portas disponiveis dos Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/GetComboPortaDisponivel")]
        #region GetComboPortaDisponivel
        public async Task<ObjCombo<String>> GetComboPortaDisponivel(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>();
            retorno.results = new List<IObjComboResult<String>>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT ")
                              .AppendLine(" A.ID, A.PORTA")
                              .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A ");

                        if (request.soativos)
                        {
                            condicoes.Add($"A.STATUS = 1"); //Porta Disponivel                                                
                        }

                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            condicoes.Add($"A.ID_CAIXA = '{request.idpai}'");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                        }

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE  {String.Join(" AND ", condicoes)} ");

                            if (!String.IsNullOrEmpty(request.id))
                            {
                                sql.AppendLine($" AND A.ID = '{request.id}'");
                            }
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var Obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["PORTA"]),
                                    //Text = HtmlEnumExtensions.GetEnumDescription<Situacao_Porta>(Convert.ToString(reader["STATUS"]))
                                };
                                retorno.results.Add(Obj);
                            }
                        }
                    }
                }

                return retorno;
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                throw erro;
            }
        }
        #endregion

        /// <summary>
        /// Gravar a porta de um Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/Gravar")]
        #region Gravar
        public async Task<PostApiRetorno<Int64?>> Gravar([FromBody] ParamWebApi<Elemento_Rede_PortaCadastroModel> request)
        {
            PostApiRetorno<Int64?> retorno = new PostApiRetorno<Int64?>();
            Elemento_Rede_Porta door = null;
            Elemento_Rede caixa = null;

            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var repoDoor = new Conexao<Elemento_Rede_Porta>(session))
                    using (var repoRede = new Conexao<Elemento_Rede>(session))
                    using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(session))
                    {
                        caixa = await repoRede.selectFirst(p => p.Id == request.Objeto.Id);
                        door = new Elemento_Rede_Porta
                        {
                            Caixa = caixa,
                            Status = caixa.Situacao == Status_Elemento.Ativo ? Situacao_Porta.Disponivel : Situacao_Porta.Indisponivel,
                        };

                        if (retorno.Ok == true)
                        {
                            retorno.IdObjeto = door.Id;

                            await repoDoor.Save(door);
                        }
                    }
                }
            }

            catch (Exception erro)
            {
                retorno.InnerException = erro.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
                return retorno;
            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// Alterar o Status da Porta de um Elemento de Rede
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/AlterarStatus")]
        #region AlterarStatus
        public async Task<PostApiRetorno<Elemento_RedeListaModel>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Elemento_RedeListaModel> retorno = new PostApiRetorno<Elemento_RedeListaModel>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {
                StringBuilder sqlUp = new StringBuilder(" UPDATE TB_ELEMENTO_REDE_PORTA ");
                StringBuilder sql = new StringBuilder();
               
                using (var session = HibernateUtil.GetSessionT5())
                {
                    string ids = $"({String.Join(",", request.ids)})";

                    using (var com = session.Connection.CreateCommand())
                    {
                        if (request.status == 0 || request.status ==1)
                        {
                            parametros.Add("@LOGIN", null);
                            sqlUp.Append("SET LOGIN = @LOGIN, STATUS = @STATUS ");
                        }
                        else
                        {
                            sqlUp.Append(" SET STATUS = @STATUS ");
                        }
                        sql.AppendLine(sqlUp.ToString());

                        sql.AppendLine($" WHERE ID IN {ids} "); 
                        parametros.Add("@ID", request.ids);
                        parametros.Add("@STATUS", request.status);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }
                using (var session = HibernateUtil.GetSessionT5())
                {

                    Elemento_Reserva reservar = new Elemento_Reserva();
                    foreach (var idPorta in request.ids)
                    {
                        using (var com = session.Connection.CreateCommand())


                        {
                            StringBuilder sqlPorta = new StringBuilder();
                            sqlPorta.AppendLine("SELECT ID_CAIXA, ID_LOGIN FROM TB_ELEMENTO_REDE_PORTA WHERE ID = @PORTA");

                            parametros = new Dictionary<string, object>
                            {
                                { "@PORTA", idPorta }
                            };


                            if(request.status != 0)
                            {
                                using (var reader = await CommandHelper.ExecuteReader(com, sqlPorta, parametros, session.Connection, null))
                                {
                                    while (await reader.ReadAsync())
                                    {

                                        await new Elemento_Rede_PortaApiController().Atribuir_Porta(new Elemento_Reserva
                                        {
                                            Cliente_PlanoProduto = 0,
                                            Id_Login = null,
                                            Login = null,
                                            Id_Porta = Convert.ToInt64(idPorta),
                                            Id_Caixa = Convert.ToInt64(reservar.Id_Caixa),
                                            Reset = false

                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/ConsultaPlanoPortaLivre")]
        #region Reserva_Porta
        public async Task<PostApiRetorno<bool>> ConsultaPlanoPortaLivre([FromBody] Elemento_Reserva request)
        {
            var retorno = new PostApiRetorno<bool>();
            retorno.IdObjeto = false;
            retorno.Ok = true;

            try
            {
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessiont5))
                {
                    var porta = await repoPorta.selectFirst(p => p.Cliente_Plano_Prod == request.Cliente_PlanoProduto);
                    if (porta != null)
                        retorno.IdObjeto = false;
                    else
                        retorno.IdObjeto = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException?.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
            }

            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/GetElementoRedePorta")]
        #region Reserva_Porta
        public async Task<PostApiRetorno<Int64>> GetElementoRedePorta([FromBody] Elemento_Rede_PortaCadastroModel request)
        {
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessiont5))
                {
                    var porta = await repoPorta.selectFirst(p => p.Caixa.Id == request.Id_Caixa && p.Porta == request.Porta);
                    if (porta != null)
                    {
                        retorno.IdObjeto = porta.Id;
                    }
                    else
                    {
                        retorno.IdObjeto = 0;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException?.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
            }

            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/GetClientePortaReservada")]
        public async Task<GetApiRetorno<List<long?>>> GetClientesPortasReservadas([FromBody] List<long> portas)
        {
            var retorno = new GetApiRetorno<List<long?>>();
            retorno.Objeto = new List<long?>();

            if (portas == null || portas.Count == 0)
                return retorno;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (System.Data.Common.DbCommand com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine($"SELECT A.CLIENTE_PLANO_PRODUTO FROM TB_ELEMENTO_REDE_PORTA A WHERE A.ID IN (" + string.Join(",", portas) + ")");
                using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {

                        retorno.Objeto.Add(Convert.IsDBNull(reader["CLIENTE_PLANO_PRODUTO"]) ? null as long? :
                                         Convert.ToInt64(reader["CLIENTE_PLANO_PRODUTO"])
                        );
                    }
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Reserva_Porta")]
        #region Reserva_Porta
        public async Task<PostApiRetorno> Reserva_Porta([FromBody] Elemento_Reserva request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                if (request == null)
                    throw new Exception("Requisição sem parametros necessários");

                if (request.Id_Porta == null && request.Porta == null)
                    throw new Exception("Requisição sem porta Especificada")
;
                //String codigo = "";
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPortas = new Conexao<Elemento_Rede_Porta>(sessiont5))
                {
                    var parametros = new Dictionary<String, Object>();
                    parametros.Add("@CLIENTE", request.Cod_Cliente);
                    parametros.Add("@CLIENTEPLANOPRODUTO", request.Cliente_PlanoProduto);
                    parametros.Add("@CAIXA", request.Id_Caixa);
                    parametros.Add("@NUM_PORTA", request.Porta);
                    parametros.Add("@PORTA", request.Id_Porta);
                    StringBuilder sql = new StringBuilder();
                    StringBuilder updateSql = new StringBuilder();
                    using (var com = sessiont5.Connection.CreateCommand())
                    {
                        StringBuilder sqlPorta = new StringBuilder();
                        sqlPorta.AppendLine(@$"SELECT * FROM  TB_ELEMENTO_REDE_PORTA WHERE ID= @PORTA");/*AND B.ID_CLIENTE = @CLIENTE*/

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlPorta, parametros, sessiont5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                request.Id_Caixa = Convert.IsDBNull(reader["ID_CAIXA"]) ? (Int64?)null : Convert.ToInt64(reader["ID_CAIXA"]);
                                request.Id_Login = Convert.IsDBNull(reader["ID_LOGIN"]) ? (Int64?)null : Convert.ToInt64(reader["ID_LOGIN"]);
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(@$"SELECT B.LOGIN
                                          FROM TB_LOGIN_RADIUS B
                                          WHERE  B.ID_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO");/*AND B.ID_CLIENTE = @CLIENTE*/

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                request.Login = Convert.ToString(reader["LOGIN"]);
                            }
                        }
                    }

                    parametros.Add("@LOGIN", request.Login);
                    await new Elemento_Rede_PortaApiController().Atribuir_Porta(new Elemento_Reserva
                    {
                        Cliente_PlanoProduto = Convert.ToInt64(request?.Cliente_PlanoProduto),
                        Id_Login = null,
                        Login = null,
                        Id_Porta = Convert.ToInt64(request.Id_Porta),
                        Id_Caixa = Convert.ToInt64(request.Id_Caixa),
                        Reset = false,

                    });
                    //SQL Limpeza de Caixa/Porta
                    using (var comt5 = sessiont5.Connection.CreateCommand())
                    {
                        if (request.Id_Porta != null)
                        {
                            updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = NULL, A.ID_LOGIN = NULL, DATAHORAALTERACAO = CURRENT_TIMESTAMP,
                                A.CLIENTE_PLANO_PRODUTO = NULL, A.STATUS = 1                                 
                                WHERE A.ID = @PORTA ");
                        }
                        else
                        {
                            updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = NULL, A.ID_LOGIN = NULL, DATAHORAALTERACAO = CURRENT_TIMESTAMP,
                                A.CLIENTE_PLANO_PRODUTO = NULL, A.STATUS = 1                                 
                                WHERE A.ID_CAIXA = @CAIXA AND A.PORTA = @NUM_PORTA ");
                        }

                        await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                    }

                    if (!request.Reset)
                    {
                        using (var comt5 = sessiont5.Connection.CreateCommand())
                        {
                            updateSql = new StringBuilder();
                            if (request.Id_Porta != null)
                            {
                                updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = @LOGIN,
                                A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO, A.STATUS = 4, 
                                A.DATA_EXPIRACAO_RESERVA = '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd hh:mm:ss")}'
                                WHERE A.ID = @PORTA");
                            }
                            else
                            {
                                updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = @LOGIN,
                                A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO, A.STATUS = 4, 
                                A.DATA_EXPIRACAO_RESERVA = '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd hh:mm:ss")}'
                                WHERE A.ID_CAIXA = @CAIXA AND A.PORTA = @NUM_PORTA");
                            }

                            await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                        }

                        var obj = new AlterarStatusViewModel
                        {
                            status = (short)(request.Reset == true ? 1 : 4),
                            ids = new List<Int64>
                            { request.Id_Porta.Value },
                        };
                        await AlterarStatus(obj);
                    }
                }


                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException?.Message;

                CONSTANTES.log4Net.Error(erro.Message, erro);
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Ativar_Porta")]
        #region Reserva_Porta

        public async Task<PostApiRetorno> Ativar_Porta([FromBody] Elemento_Reserva request)
        {
            var retorno = new PostApiRetorno();

            var portasParaProcessar = request.Ids_Porta ?? (request.Id_Porta != null ? new List<long> { request.Id_Porta.Value } : new List<long>());
            try
            {
                foreach (var porta in portasParaProcessar)
                {
                    if (request == null)
                        throw new Exception("Requisição sem parametros necessários");

                    using (ISession session = HibernateUtil.GetSession())
                    using (ISession sessiont5 = HibernateUtil.GetSessionT5())
                    using (Conexao<Elemento_Rede_Porta> repoPortas = new Conexao<Elemento_Rede_Porta>(sessiont5))
                    {
                        var situacaoporta = await repoPortas.selectFirst(p => p.Id == porta);

                        if (situacaoporta.Status == Situacao_Porta.Disponivel)
                            throw new Exception("Porta está disponivel");

                        var parametros = new Dictionary<String, Object>();
                        parametros.Add("@CLIENTE", request.Cod_Cliente);
                        parametros.Add("@CLIENTEPLANOPRODUTO", request.Cliente_PlanoProduto);
                        parametros.Add("@CAIXA", request.Id_Caixa);
                        parametros.Add("@NUM_PORTA", request.Porta);
                        parametros.Add("@PORTA", porta);
                        StringBuilder sql = new StringBuilder();
                        StringBuilder updateSql = new StringBuilder();
                        using (var com = sessiont5.Connection.CreateCommand())
                        {
                            StringBuilder sqlPorta = new StringBuilder();
                            sqlPorta.AppendLine(@$"SELECT ID_CAIXA,ID_LOGIN FROM  TB_ELEMENTO_REDE_PORTA WHERE ID= @PORTA");/*AND B.ID_CLIENTE = @CLIENTE*/

                            using (var reader = await CommandHelper.ExecuteReader(com, sqlPorta, parametros, sessiont5.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    request.Id_Caixa = reader["ID_CAIXA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CAIXA"]) : (long?)null;
                                    request.Id_Login = reader["ID_LOGIN"] != DBNull.Value ? Convert.ToInt64(reader["ID_LOGIN"]) : (long?)null;
                                }
                            }
                        }

                        using (var com = session.Connection.CreateCommand())
                        {
                            sql.AppendLine(@$"SELECT B.LOGIN
                                      FROM TB_LOGIN_RADIUS B
                                      WHERE  B.ID_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO");

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    request.Login = Convert.ToString(reader["LOGIN"]);
                                }
                            }
                        }

                        parametros.Add("@LOGIN", request.Login);
                        await new Elemento_Rede_PortaApiController().Atribuir_Porta(new Elemento_Reserva
                        {
                            Cliente_PlanoProduto = Convert.ToInt64(request?.Cliente_PlanoProduto),
                            Id_Login = null,
                            Login = null,
                            Id_Porta = Convert.ToInt64(porta),
                            Id_Caixa = Convert.ToInt64(request.Id_Caixa),
                            Reset = false,

                        });
                        //SQL Limpeza de Caixa/Porta
                        using (var comt5 = sessiont5.Connection.CreateCommand())
                        {
                            updateSql.AppendLine(@$"
                                 UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = NULL, A.ID_LOGIN = NULL, DATAHORAALTERACAO = CURRENT_TIMESTAMP,
                                 A.CLIENTE_PLANO_PRODUTO = NULL, A.STATUS = 1                                 
                                 WHERE A.ID = @PORTA ");


                            await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                        }

                        if (!request.Reset)
                        {
                            using (var comt5 = sessiont5.Connection.CreateCommand())
                            {
                                updateSql = new StringBuilder();

                                updateSql.AppendLine(@$"
                                     UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = @LOGIN,
                                     A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO, A.STATUS = 4, 
                                     A.DATA_EXPIRACAO_RESERVA = '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd hh:mm:ss")}'
                                     WHERE A.ID = @PORTA");


                                await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                            }

                            var obj = new AlterarStatusViewModel
                            {
                                status = (short)(request.Reset == true ? 1 : 4),
                                ids = new List<Int64>
                                { porta },
                            };
                            await AlterarStatus(obj);
                        }
                    }


                    retorno.Ok = true;


                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException?.Message;

                CONSTANTES.log4Net.Error(erro.Message, erro);
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Atribuir_Porta")]
        #region Reserva_Porta
        public async Task<PostApiRetorno> Atribuir_Porta([FromBody] Elemento_Reserva request)
        {
            var retorno = new PostApiRetorno();

            try
            {
                if (request == null)
                    throw new Exception("Requisição sem parametros necessários");

                //String codigo = "";
                using (var session = HibernateUtil.GetSession())
                using (var sessiont5 = HibernateUtil.GetSessionT5())
                using (var sessiont5NH = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessiont5NH))
                {
                    Elemento_Rede_Porta porta = null;
                    if (request.Id_Porta.HasValue)
                    {
                        porta = await repoPorta.selectFirst(p => p.Id == request.Id_Porta);
                        request.Porta = porta?.Porta;
                        request.Id_Caixa = porta?.Caixa?.Id;
                    }
                    else if (!request.Id_Porta.HasValue && request.Id_Caixa.HasValue && request.Porta.HasValue)
                    {
                        porta = await repoPorta.selectFirst(p => p.Caixa.Id == request.Id_Caixa && p.Porta == request.Porta);
                        request.Id_Porta = porta?.Id;
                        request.Id_Caixa = porta?.Caixa?.Id;
                    }

                    if (request.Id_Porta == null)
                    {
                        throw new Exception("Requisição sem porta Especificada");

                    }

                    var parametros = new Dictionary<String, Object>();

                    parametros.Add("@ID_CAIXA_FTTH", request.Id_Caixa);
                    parametros.Add("@ID_PORTA_FTTH", request.Id_Porta);
                    parametros.Add("@CLIENTE", request.Cod_Cliente);
                    parametros.Add("@CLIENTEPLANOPRODUTO", request.Cliente_PlanoProduto);
                    parametros.Add("@PORTA", request.Id_Porta);
                    StringBuilder sql = new StringBuilder(),
                    updateSql = new StringBuilder();

                    if (request.Cliente_PlanoProduto != 0)
                    {
                        using (var com = session.Connection.CreateCommand())
                        {
                            sql.AppendLine(@$"SELECT B.LOGIN, B.ID
                                          FROM TB_LOGIN_RADIUS B
                                          WHERE  B.ID_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO");/*AND B.ID_CLIENTE = @CLIENTE*/

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    request.Login = Convert.IsDBNull(reader["LOGIN"]) ? "" : Convert.ToString(reader["LOGIN"]);
                                    request.Id_Login = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null;
                                }
                            }
                        }
                    }

                    parametros.Add("@LOGIN", request.Login);
                    parametros.Add("@ID_LOGIN", request.Id_Login);

                    //SQL Limpeza de Caixa/Porta
                    using (var comt5 = sessiont5.Connection.CreateCommand())
                    {

                        if (request.Cliente_PlanoProduto != 0)
                        {
                            updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = NULL, A.ID_LOGIN = NULL,
                                A.CLIENTE_PLANO_PRODUTO = NULL,A.STATUS = 1
                                WHERE A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO ");
                        }
                        else if (request.Id_Login.HasValue)
                        {
                            updateSql.AppendLine(@$"UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = NULL, A.ID_LOGIN = NULL,
                                                    A.CLIENTE_PLANO_PRODUTO = NULL,A.STATUS = 1
                                                    WHERE A.ID_LOGIN = @ID_LOGIN ");
                        }
                        if (updateSql.Length > 0)
                        {

                            await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                        }
                    }

                    //SQL Atribuição de nova Caixa/Porta
                    if (!request.Reset && request.Id_Login != null)
                    {
                        using (var comt5 = sessiont5.Connection.CreateCommand())
                        {
                            updateSql = new StringBuilder();
                            updateSql.AppendLine(@$"
                                UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = @LOGIN, A.ID_LOGIN = @ID_LOGIN,
                                A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO,A.STATUS = 5
                                WHERE A.ID = @PORTA ");

                            await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, sessiont5.Connection, null);
                        }

                        using (var com = session.Connection.CreateCommand())
                        {
                            updateSql = new StringBuilder();
                            updateSql.AppendLine(@$"
                                UPDATE TB_LOGIN_RADIUS A SET A.ID_CAIXA_FTTH = @ID_CAIXA_FTTH, A.ID_PORTA_FTTH = @ID_PORTA_FTTH
                                WHERE A.ID = @ID_LOGIN ");

                            await CommandHelper.ExecuteNonQuery(com, updateSql, parametros, session.Connection, null);
                        }
                    }

                    var obj = new AlterarStatusViewModel
                    {
                        ids = new List<Int64>
                        {
                            request.Id_Porta.Value
                        },
                        status = (short)(request.Reset == true ? 1 : 5)
                    };


                    await AtualizaPortaIntegracaoAniel(request.Id_Login, request.Id_Porta, request.Id_Caixa, Convert.ToInt16(request?.Porta));
                }

                retorno.Ok = true;
            }

            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException?.Message;
                CONSTANTES.log4Net.Error(erro.Message, erro);
            }
            return retorno;

        }
        #endregion

        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/GetComboLogin")]
        #region GetComboLogin 
        public async Task<ObjCombo<String>> GetComboLogin(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>();
            retorno.results = new List<IObjComboResult<String>>();
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();


                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT ")
                              .AppendLine(" A.ID, A.LOGIN")
                              .AppendLine("FROM TB_ELEMENTO_REDE_PORTA A");

                        if (request.soativos)
                        {
                            //condicoes.Add($"A.STATUS = 1");
                        }

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            condicoes.Add($"A.ID = '{request.id}'");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.LOGIN) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                        }

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var Obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["LOGIN"])
                                };
                                retorno.results.Add(Obj);
                            }
                        }

                    }
                }

                return retorno;
            }

            catch (Exception erro)
            {
                CONSTANTES.log4Net.Error(erro.Message, erro);
                throw erro;
            }
        }
        #endregion

        ///<summary>
        ///Endpoint para interação com a api de Reserva de Porta Geosite
        ///</summary>
        ///<param name="request"></param>
        ///<returns></returns>
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Ligacao_ReservaPorta_Geosite")]
        #region Geosite_ReservaPorta
        public async Task<PostApiRetorno> Ligacao_ReservaPorta_Geosite([FromBody] Ligacao_Porta_GeositeRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            IntegracoesExternas obj = new IntegracoesExternas();

            long? requestReserva = null;

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(session))
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    IntegracaoExternaGeositeService apiGeosite = new IntegracaoExternaGeositeService(session);

                    //Connect
                    obj = (await apiGeosite.GetListConfiguracoes(com, IntegracaoExterna.GEOSITE)).FirstOrDefault();

                    //Aniel TV
                    Elemento_Rede_Porta portaObj = await repoPorta.selectFirst(p => p.Id == request.porta);

                    //Requisição Geosite
                    //Aniel Connect - (CLIENTE PLANO PRODUTO)
                    Cliente_PlanoProduto clipp = (await repoClipp.selectFirst(p => p.Id == request.cliente_PlanoProduto));

                    //Requisição Geosite
                    ResponseGeosite<ClienteGeosite> checkCliente = apiGeosite.CheckClienteGeosite(obj, clipp.Cliente.CpfCnpj);
                    ResponseGeosite operSaveCliente;

                    if (checkCliente.total == null || checkCliente.total == "0")
                    {

                        ClienteGeosite clientGeosite = apiGeosite.BindClienteGeosite(clipp.Cliente, portaObj);
                        operSaveCliente = apiGeosite.SaveCliente(obj, clientGeosite);

                        if (operSaveCliente.success)
                        {
                            requestReserva = operSaveCliente.fid.Value;
                        }
                    }
                    else
                    {
                        requestReserva = checkCliente.records[0].fid;
                    }

                    ResponseGeositeReserva content = apiGeosite.Reservar_Porta(obj, portaObj, requestReserva.Value);
                    if (content.success)
                    {
                        portaObj.Data_Expiracao_Reserva = Convert.ToDateTime(content.dataExpiracao);
                        await repoPorta.Save(portaObj);

                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new EAnielException(content.message);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
        #endregion

        ///<summary>
        ///Endpoint para interação com a api de Update da Reserva de Porta Geosite
        ///</summary>
        ///<param name="request"></param>
        ///<returns></returns>
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Ligacao_UpdateReserva_Geosite")]
        #region Geosite_UpdateReserva
        public async Task<PostApiRetorno> Ligacao_UpdateReserva_Geosite([FromBody] Ligacao_Porta_GeositeRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            IntegracoesExternas obj = new IntegracoesExternas();
            long? requestReserva = null;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(session))
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    IntegracaoExternaGeositeService apiGeosite = new IntegracaoExternaGeositeService(session, sessionT5);
                    //Connect
                    obj = (await apiGeosite.GetListConfiguracoes(com, IntegracaoExterna.GEOSITE)).FirstOrDefault();

                    //Aniel TV
                    Elemento_Rede_Porta portaObj = await repoPorta.selectFirst(p => p.Id == request.porta);

                    //Aniel Connect - (CLIENTE PLANO PRODUTO)
                    Cliente_PlanoProduto clipp = (await repoClipp.selectFirst(p => p.Id == request.cliente_PlanoProduto));

                    //Requisição Geosite
                    ResponseGeosite<ClienteGeosite> checkCliente = apiGeosite.CheckClienteGeosite(obj, clipp.Cliente.CpfCnpj);
                    ResponseGeosite content;
                    if (checkCliente.total == "0")
                    {
                        throw new Exception("Cliente não encontrado no Geosite para update de Reserva.");
                    }
                    else
                    {
                        requestReserva = checkCliente.records[0].fid;
                        content = apiGeosite.Update_Reserva(obj, portaObj, requestReserva);

                        if (content.success)
                        {
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new EAnielException(content.message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
        #endregion

        /// <summary>
        /// Endpoint para interação com a api de Adicionar Conexão Geosite
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Ligacao_AdicionarConexao_Geosite")]
        #region Geosite_AdicionarConexao
        public async Task<PostApiRetorno> Ligacao_AdicionarConexao_Geosite([FromBody] Ligacao_Porta_GeositeRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            IntegracoesExternas obj = new IntegracoesExternas();
            long? requestAddConnection = null;
            try
            {

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(session))
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    IntegracaoExternaGeositeService apiGeosite = new IntegracaoExternaGeositeService(session, sessionT5);
                    //Connect
                    obj = (await apiGeosite.GetListConfiguracoes(com, IntegracaoExterna.GEOSITE)).FirstOrDefault();

                    //Aniel TV
                    Elemento_Rede_Porta portaObj = await repoPorta.selectFirst(p => p.Id == request.porta);

                    //Aniel Connect - (CLIENTE PLANO PRODUTO)
                    Cliente_PlanoProduto clipp = (await repoClipp.selectFirst(p => p.Id == request.cliente_PlanoProduto));

                    //Requisição Geosite
                    ResponseGeosite<ClienteGeosite> checkCliente = apiGeosite.CheckClienteGeosite(obj, clipp.Cliente.CpfCnpj);

                    if (checkCliente.total == "0" || checkCliente.total == null)
                    {

                        ClienteGeosite clientGeosite = apiGeosite.BindClienteGeosite(clipp.Cliente, portaObj);

                        ResponseGeosite SaveCliente = apiGeosite.SaveCliente(obj, clientGeosite);
                        if (SaveCliente.success)
                        {
                            requestAddConnection = SaveCliente.fid.Value;
                        }
                    }
                    else
                    {
                        requestAddConnection = checkCliente.records[0].fid;
                        ResponseGeosite content = await apiGeosite.Adicionar_ConexaoCliente(obj, portaObj, requestAddConnection.Value);

                        if (content.success)
                        {
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new EAnielException(content.message);
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
        #endregion
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/ValidatePorta")]
        public async Task<bool> ValidatePorta([FromBody] List<long> request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (Conexao<Elemento_Rede_Porta> repoPortas = new Conexao<Elemento_Rede_Porta>(sessionT5))
                {
                    var portas = await repoPortas.select(p => request.Contains(p.Id));

                    if (!portas.Any())
                        return false;

                    IntegracaoExternaGeositeService integracao = new IntegracaoExternaGeositeService(session, sessionT5);

                    // valida todas as portas
                    foreach (var porta in portas)
                    {
                        if (!integracao.ValidateIntegracaoByUsuario(porta.Caixa))
                            return false;
                    }

                    return true;
                }
            }
        }

        ///<summary>
        ///Endpoint para interação com a api de Remoção de Conexão da Reserva de Porta Geosite
        ///</summary>
        ///<param name="request"></param>
        ///<returns></returns>
        [HttpPost()]
        [Route("api/Elemento_Rede_PortaApi/Ligacao_RemoverConexao_Geosite")]
        #region Geosite_RemoverConexao
        public async Task<PostApiRetorno> Ligacao_RemoverConexao_Geosite([FromBody] Ligacao_Porta_GeositeRequestModel request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            IntegracoesExternas obj = new IntegracoesExternas();
            long? requestRemoveConnection = null;
            try
            {
                var portasParaProcessar = request.portas ?? (request.porta != null ? new List<long> { request.porta } : new List<long>());

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))

                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(session))
                using (System.Data.Common.DbCommand com = session.Connection.CreateCommand())
                {
                    foreach (var porta in portasParaProcessar)
                    {
                        IntegracaoExternaGeositeService apiGeosite = new IntegracaoExternaGeositeService(session);
                        //Connect
                        obj = (await apiGeosite.GetListConfiguracoes(com, IntegracaoExterna.GEOSITE)).FirstOrDefault();

                        //Aniel TV
                        Elemento_Rede_Porta portaObj = await repoPorta.selectFirst(p => p.Id == porta);

                        //Aniel Connect - (CLIENTE PLANO PRODUTO)
                        Cliente_PlanoProduto clipp = (await repoClipp.selectFirst(p => p.Id == request.cliente_PlanoProduto));

                        //Requisição Geosite
                        ResponseGeosite<ClienteGeosite> checkCliente = apiGeosite.CheckClienteGeosite(obj, clipp.Cliente.CpfCnpj);
                        ResponseGeosite content = new ResponseGeosite();

                        //sem cliente vinculado
                        if (checkCliente.total == "0" || checkCliente.total == null)
                        {
                            throw new Exception("Cliente não encontrado na Integração Geosite.");

                        }

                        // cliente com conexões
                        else if (checkCliente.total != "0")
                        {
                            requestRemoveConnection = checkCliente.records[0].fid;

                            if (Convert.ToInt32(checkCliente.total) > 1)
                            {
                                content = apiGeosite.Remover_ConexaoCliente(obj, portaObj, requestRemoveConnection.Value);
                            }

                            else
                            {
                                content = apiGeosite.Remover_Cliente(obj, requestRemoveConnection.Value);
                            }

                        }


                        if (content.success)
                        {
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new EAnielException(content.message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
        #endregion

        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/ReservarPortaByVenda")]
        public async Task<PostApiRetorno> ReservarPortaByVenda([FromBody] ReservaRequest request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (var session = HibernateUtil.GetSession())
                {
                    IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, HibernateUtil.GetSessionT5());
                    var requestReserva = await repository.GetParametrosReservaPortaByClientePlanoProduto(request);
                    await this.Reserva_Porta(requestReserva);
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/Elemento_Rede_PortaApi/GetPortaReservadaClientePlanoProduto")]
        public async Task<GetApiRetorno<CaixaPortaReservada>> GetPortaReservadaClientePlanoProduto([FromBody] CaixaPortaReservadaRequest request)
        {
            var retorno = new GetApiRetorno<CaixaPortaReservada>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, sessionT5);
                    retorno = await repository.GetPortaReservadaClientePlanoProduto(request);
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
        [HttpPost]
        [Route("api/porta/integracao")]
        public async Task<Response> AtualizaPortaIntegracaoAniel(long? idLogin, long? idPorta, long? idCaixa, Int16? numeroPorta)
        {

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            var usuarioApi = "";
            var senhaApi = "";
            var tokenApi = "";
            var idIntegracao = 0;
            Elemento_Rede_Porta porta = null;
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (Conexao<Elemento_Rede_Porta> repoPortas = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                {
                    if (idPorta.HasValue)
                        porta = await repoPortas.selectFirst(p => p.Id == idPorta);
                    else if (idCaixa.HasValue && numeroPorta.HasValue)
                        porta = await repoPortas.selectFirst(p => p.Caixa.Id == idCaixa.Value && p.Porta == numeroPorta.Value);

                    if (porta == null)
                    {
                        throw new Exception("porta não encontrada");
                    }


                }

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT TIPO_INTEGRACAO_CTO FROM TB_MASTER ");

                using (var reader = await CommandHelper.ExecuteReader(sessionT5.Connection.CreateCommand(), sql, new Dictionary<string, object>(), sessionT5.Connection, null))
                    if (reader.Read())
                    {
                        idIntegracao = Convert.IsDBNull(reader["TIPO_INTEGRACAO_CTO"]) ? 0 : Convert.ToInt32(reader["TIPO_INTEGRACAO_CTO"]);
                    }


                sql = new StringBuilder();
                sql.AppendLine("SELECT USUARIO_APIANIEL, SENHA_APIANIEL,TOKEN_APIANIEL FROM TB_CONFIGURACOES_GERAIS");

                using (var reader = await CommandHelper.ExecuteReader(session.Connection.CreateCommand(), sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        usuarioApi = Convert.IsDBNull(reader["USUARIO_APIANIEL"]) ? "" : Convert.ToString(reader["USUARIO_APIANIEL"]);
                        senhaApi = Convert.IsDBNull(reader["SENHA_APIANIEL"]) ? "" : Convert.ToString(reader["SENHA_APIANIEL"]);
                        tokenApi = Convert.IsDBNull(reader["TOKEN_APIANIEL"]) ? "" : Convert.ToString(reader["TOKEN_APIANIEL"]);
                    }
                }

            }

            var payload = new
            {
                obj = new
                {
                    idLogin = idLogin,
                    idCaixa = porta.Caixa.Id,
                    numeroPorta = porta.Porta

                },
                tipo_Integracao = idIntegracao,
                settings = new
                {
                    start = 0,
                    length = 0,
                    search = "string",
                    order = new
                    {
                        column = 0,
                        dir = "string"
                    },
                    user = usuarioApi,
                    password = senhaApi,
                    token = tokenApi
                }
            };



            try
            {
                return AtualizarCaixaPortaAsync(payload);
            }
            catch (Exception ex)
            {
                return new Response
                {
                    result = false,
                    message = ex.Message
                };
            }
        }
        public Response AtualizarCaixaPortaAsync(object payload)
        {
            try
            {
                CONSTANTES.log4Net.Error("payload API ANIEL: " + JsonService.SerializeObject(payload));
                var client = new RestClient($"{CONSTANTES.URLPortalApiAniel}/api/Login_RadiusApi/AtualizarCaixaPorta");
                var request = new RestRequest(Method.POST);
                request.AddHeader("postman-token", "d6a5b694-ec96-c2cd-ff0a-5e9efe83f981");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Accept", "application/json");
                request.AddJsonBody(payload);
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                client.Timeout = 180 * 1000;

                IRestResponse<PostApiRetorno> response = client.Execute<PostApiRetorno>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new Response
                    {
                        result = true,
                        message = "Ok"
                    };
                }
                else
                {
                    return new Response
                    {
                        result = false,
                        message = response.ErrorMessage
                    };
                }



            }
            catch (Exception ex)
            {
                return new Response
                {
                    result = false,
                    message = ex.Message
                };
            }
        }

    }
}
