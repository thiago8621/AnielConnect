using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Sinapse.Util.RemoteConnection;
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
    public class OLTApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/OLTapi/GetAll")]
        public async Task<DataTableResponse<OLTListaModel>> GetAll(OLT_DataTableRequest request)
        {

            List<OLTListaModel> retorno = new List<OLTListaModel>();
            IOrderedEnumerable<OLTListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Sql From
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_OLT A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_FILIAL B ON (A.ID_FILIAL = B.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_EMPRESA C ON (B.ID_EMPRESA = C.ID) ");
                sqlFrom.AppendLine(" WHERE A.ID > 0");

                //Filtrando                    
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "ip")
                    {
                        sqlFrom.AppendLine(" AND (A.IP LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "olt")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }


                if (!String.IsNullOrEmpty(request.Id_Olt))
                {
                    sqlFrom.AppendLine("AND (A.ID = @IDOLT)");
                    parametros.Add("@IDOLT", Convert.ToInt32(request.Id_Olt));
                }

                if (!String.IsNullOrEmpty(request.Id_Empresa))
                {
                    sqlFrom.AppendLine("AND (B.ID_EMPRESA = @ID_EMPRESA)");
                    parametros.Add("@ID_EMPRESA", Convert.ToInt32(request.Id_Empresa));
                }

                if (!String.IsNullOrEmpty(request.Id_Filial))
                {
                    sqlFrom.AppendLine("AND (A.ID_FILIAL = @ID_FILIAL)");
                    parametros.Add("@ID_FILIAL", Convert.ToInt32(request.Id_Filial));
                }

                if (!String.IsNullOrEmpty(request.Ip))
                {
                    sqlFrom.AppendLine("AND (A.IP = @IP)");
                    parametros.Add("@IP", request.Ip);
                }

                if (!String.IsNullOrEmpty(request.Tipo_Olt))
                {
                    sqlFrom.AppendLine("AND (A.MARCA_MODELO = @MARCA_MODELO)");
                    parametros.Add("@MARCA_MODELO", Convert.ToInt32(request.Tipo_Olt));
                }

                if (!String.IsNullOrEmpty(request.Status_Filtro))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
                }

                //sqlFrom.AppendLine(" ORDER BY C.CODEMPRESA, B.CODFILIAL, A.OLTID ");

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.Append(sqlFrom.ToString());

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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.NUM_OLT, A.IP, A.MARCA_MODELO, A.STATUS, B.NOME AS FILIAL, C.NOME AS EMPRESA, A.OLTID, C.CODEMPRESA, B.CODFILIAL ");
                    sql.Append(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new OLTListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                NumOLT = Convert.ToString(reader["NUM_OLT"]),
                                Tipo = HtmlEnumExtensions.GetEnumDescription<Marca_Modelo_OLT>(Convert.ToString(reader["MARCA_MODELO"])),
                                Ip = Convert.ToString(reader["IP"]),
                                Situacao = reader["STATUS"] != DBNull.Value ? Convert.ToInt64(reader["STATUS"]) == 1 ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO : String.Empty
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
            return new DataTableResponse<OLTListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/OLTApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            return await GetComboTyped(request, "filial");
        }

        [HttpPost()]
        [Route("api/OLTApi/GetComboByPOP")]
        public async Task<ObjCombo<Int64>> GetComboByPOP(RequestObjComboPai<Int64?, Int64?> request)
        {
            return await GetComboTyped(request, "pop");
        }

        private async Task<ObjCombo<Int64>> GetComboTyped(RequestObjComboPai<Int64?, Int64?> request, string type)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_OLT A ");

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
                        if (type == "filial")
                            condicoes.Add($"A.ID_FILIAL = {request.idpai}");
                        else if (type == "pop")
                            condicoes.Add($"A.ID_POP = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.IP, A.GABINETE, A.NUM_OLT, A.SUBRACK ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new OLTComboResult
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = $"{reader["DESCRICAO"]} ({reader["IP"]})",
                                    Ip = Convert.ToString(reader["IP"]),
                                    Gabinete = Convert.ToString(reader["GABINETE"]),
                                    NumOlt = Convert.ToString(reader["NUM_OLT"]),
                                    Subrack = Convert.ToString(reader["SUBRACK"])
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

        [HttpPost]
        [Route("api/OLTApi/TesteConexao")]
        public async Task<GetApiRetorno<List<TestarConexaoRetorno>>> TesteConexao(TestarConexaoRequest request)
        {
            GetApiRetorno<List<TestarConexaoRetorno>> retorno = new GetApiRetorno<List<TestarConexaoRetorno>>();
            retorno.Objeto = new List<TestarConexaoRetorno>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOLT = new Conexao<OLT>(session))
                {
                    if (request.PortaSsh.HasValue)
                    {
                        var resultado = await TestarConexaoProtocolo(request.IP, request.PortaSsh.Value, request.Login, request.Senha, Protocol.Ssh);
                        retorno.Objeto.Add(resultado);
                    }

                    if (request.PortaTelnet.HasValue)
                    {
                        var resultado = await TestarConexaoProtocolo(request.IP, request.PortaTelnet.Value, request.Login, request.Senha, Protocol.Telnet);
                        retorno.Objeto.Add(resultado);
                    }

                    if(request.PortaTL1.HasValue)
                    {
                        var resultado = await TestarConexaoProtocoloTL1(request.IPTL1, request.PortaTL1.Value, request.UsuarioTL1, request.SenhaTL1, Protocol.Telnet, true);
                        retorno.Objeto.Add(resultado);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private async Task<TestarConexaoRetorno> TestarConexaoProtocolo(string ip, int porta, string login, string senha, Protocol protocolo)
        {
            try
            {
                using (var rc = new RemoteConnection(ip, porta, login, senha, protocolo))
                {
                    await rc.Connect();
                    if(rc.IsConnected())
                    {
                        await rc.ExecuteCommandAsync("");

                        return new TestarConexaoRetorno
                        {
                            Protocolo = HtmlEnumExtensions.GetEnumDescription(protocolo),
                            Conectado = true,
                            Erro = false,
                        };
                    }
                    else
                    {
                        return new TestarConexaoRetorno
                        {
                            Protocolo = HtmlEnumExtensions.GetEnumDescription(protocolo),
                            Conectado = false,
                            Erro = true,
                        };
                    }

                    


                }
            }
            catch
            {
                return new TestarConexaoRetorno
                {
                    Protocolo = HtmlEnumExtensions.GetEnumDescription(protocolo),
                    Conectado = false,
                    Erro = true
                };
            }
        }

        private async Task<TestarConexaoRetorno> TestarConexaoProtocoloTL1(string ip, int porta, string login, string senha, Protocol protocolo, bool usarTCP)
        {
            var resultado = new TestarConexaoRetorno();
            try
            {
                using (var rc = new RemoteConnection(ip, porta, login, senha, protocolo, usarTCP))
                {
                    await rc.ConnectTL1();
                    if(rc.IsConnected())
                    {

                        await rc.LogoutTL1();


                        resultado.Protocolo = "TL1";
                        resultado.Conectado = true;
                        resultado.Erro = false;

                    }
                    else
                    {
                        resultado.Protocolo = "TL1";
                        resultado.Conectado = false;
                        resultado.Erro = true;
                    }
                   
                }
            }
            catch
            {
                return new TestarConexaoRetorno
                {
                    Protocolo = HtmlEnumExtensions.GetEnumDescription(protocolo),
                    Conectado = false,
                    Erro = true
                };
            }

            return resultado;
        }

        [HttpGet()]
        [Route("api/OLTApi/GetTreeView")]
        public async Task<GetApiRetorno<List<OLTTreeViewModel>>> GetTreeView(Int64? IdFilial,Int64? IdOLT)
        {
            GetApiRetorno<List<OLTTreeViewModel>> retorno = new GetApiRetorno<List<OLTTreeViewModel>>
            {
                Objeto = new List<OLTTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                        StringBuilder sql = new StringBuilder($"SELECT ID, DESCRICAO FROM TB_OLT WHERE STATUS = 1 ");

                        if (IdFilial.HasValue)
                        {
                            sql.AppendLine(" AND ID_FILIAL = @ID_FILIAL");
                            parametros.Add("@ID_FILIAL", IdFilial);
                        }

                        if (IdOLT.HasValue)
                        {
                            sql.AppendLine(" AND ID = @ID_OLT");
                            parametros.Add("@ID_OLT", IdOLT);
                        }

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new OLTTreeViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"])
                            });
                        }

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

        private OLTCadastroModel GetViewObj(OLT obj)
        {
            
            var listaGruposAreaView = new List<OLT_Grupo_AreaListModel>();
            if (obj.GruposArea != null && obj.GruposArea.Count > 0)
            {
                foreach (var grupo in obj.GruposArea)
                {
                    if (grupo.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        listaGruposAreaView.Add(new OLT_Grupo_AreaListModel
                        {
                            Id = grupo.Id,
                            Cod_Area = grupo.Cod_Area,
                            Cod_Escritorio = grupo.Cod_Escritorio,                           
                            Desc_Area = grupo.Desc_Area,
                            Desc_Escritorio = grupo.Desc_Escritorio,                           
                        });
                }
            }

            return new OLTCadastroModel
            {
                Id = obj.Id,
                Delay = obj.Delay != null ? (long?)obj.Delay : null,
                CodigoAuxiliar = obj.CodigoAuxiliar,
                Descricao = obj.Descricao,
                OLTID = $"{obj.Filial?.Empresa?.CodEmpresa}-{obj.Filial?.CodFilial}-{obj.Descricao}",
                Marca_Modelo = obj.Marca_Modelo,
                Versao_OLT = obj.Versao_OLT,

                NumOLT = obj.NumOLT,

                IdPOP = obj.POP?.Id ?? 0,

                //Cod_Escritorio = obj.Cod_Escritorio,
                //Cod_Area = obj.Cod_Area,
                GruposArea = listaGruposAreaView,


                #region Dados Conexão

                Ip = obj.IP,
                Login = obj.Login,
                Senha = obj.Senha,
                PortaTelnet = obj.PortaTelnet,
                PortaAPI = obj.PortaAPI,
                PortaHTTP = obj.PortaHTTP,
                PortaSSH = obj.PortaSSH,


                #endregion

                #region Dados TL1 

                IpTL1 = obj.IpTL1,
                LoginTL1 = obj.LoginTL1,
                SenhaTL1 = obj.SenhaTL1,
                PortaTL1 = obj.PortaTL1,
                PortaTL1_SSH = obj.PortaTL1_SSH,

                Gabinete = obj.Gabinete,
                Subrack = obj.Subrack,

                #endregion

                Status = obj.Status == Status_Default.Ativo,
                ConverteOlt = obj.ConverteOlt == Status_Default.Ativo,
                Tipo_Identificacao = obj.Tipo_Identificacao,
                VLAN_Uplink = obj.VLAN_Uplink,
                Config_PPPOE_Manual = obj.Config_PPPOE_Manual,
                LimitePotenciaONUBaixa = Convert.ToString(obj.LimitePotenciaONUBaixa, System.Globalization.CultureInfo.InvariantCulture)
            };

        }

        private GetOLTApiResponse GetOLTApiResponseObj(OLT obj)
        {

            var listaGruposAreaView = new List<OLT_Grupo_AreaListModel>();
            if (obj.GruposArea != null && obj.GruposArea.Count > 0)
            {
                foreach (var grupo in obj.GruposArea)
                {
                    if (grupo.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        listaGruposAreaView.Add(new OLT_Grupo_AreaListModel
                        {
                            Id = grupo.Id,
                            Cod_Area = grupo.Cod_Area,
                            Cod_Escritorio = grupo.Cod_Escritorio,
                            Desc_Area = grupo.Desc_Area,
                            Desc_Escritorio = grupo.Desc_Escritorio,
                        });
                }
            }

            return new GetOLTApiResponse
            {
                Id = obj.Id,
                Delay = obj.Delay != null ? (long?)obj.Delay : null,
                CodigoAuxiliar = obj.CodigoAuxiliar,
                Descricao = obj.Descricao,
                OLTID = $"{obj.Filial?.Empresa?.CodEmpresa}-{obj.Filial?.CodFilial}-{obj.Descricao}",
                Marca_Modelo = obj.Marca_Modelo,
                Versao_OLT = obj.Versao_OLT,

                NumOLT = obj.NumOLT,

                IdPOP = obj.POP?.Id ?? 0,

                //Cod_Escritorio = obj.Cod_Escritorio,
                //Cod_Area = obj.Cod_Area,
                GruposArea = listaGruposAreaView,


                #region Dados Conexão

                Ip = obj.IP,
                Login = obj.Login,
                Senha = obj.Senha,
                PortaTelnet = obj.PortaTelnet,
                PortaAPI = obj.PortaAPI,
                PortaHTTP = obj.PortaHTTP,
                PortaSSH = obj.PortaSSH, 


                #endregion

                #region Dados TL1 

                IpTL1 = obj.IpTL1,
                LoginTL1 = obj.LoginTL1,
                SenhaTL1 = obj.SenhaTL1,
                PortaTL1 = obj.PortaTL1,
                PortaTL1_SSH = obj.PortaTL1_SSH,

                Gabinete = obj.Gabinete,
                Subrack = obj.Subrack,

                #endregion

                Status = obj.Status == Status_Default.Ativo,
                ConverteOlt = obj.ConverteOlt == Status_Default.Ativo,
                Tipo_Identificacao = obj.Tipo_Identificacao,
                VLAN_Uplink = obj.VLAN_Uplink,
                LimitePotenciaONUBaixa = Convert.ToString(obj.LimitePotenciaONUBaixa, System.Globalization.CultureInfo.InvariantCulture),
                Config_PPPOE_Manual = obj.Config_PPPOE_Manual
            };

        }

        private async Task BindViewToObj(ISession session, OLT objBanco, OLTCadastroModel view)
        {
            using (var repoOLT = new Conexao<OLT>(session))
            using (var repoPOP = new Conexao<POP>(session))            
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Marca_Modelo = view.Marca_Modelo;
                objBanco.Versao_OLT = view.Versao_OLT;
                objBanco.Delay = view.Delay;

                //var OLTNumRepetido = await repoOLT.selectFirst(p => p.NumOLT == view.NumOLT && p.Id != objBanco.Id);

                //if (OLTNumRepetido != null)
                //    throw new Exception("Num. OLT já cadastrado no sistema");
                //else
                    objBanco.NumOLT = view.NumOLT;

                objBanco.POP = await repoPOP.selectFirst(p => p.Id == view.IdPOP);
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;
                objBanco.VLAN_Uplink = view.VLAN_Uplink;
                objBanco.Config_PPPOE_Manual = view.Config_PPPOE_Manual;
                
                #region Dados Comunicação

                objBanco.IP = view.Ip;
                objBanco.Login = view.Login;
                objBanco.Senha = view.Senha;

                objBanco.PortaTelnet = view.PortaTelnet;
                objBanco.PortaAPI = view.PortaAPI;
                objBanco.PortaHTTP = view.PortaHTTP;
                objBanco.PortaSSH = view.PortaSSH;


                #endregion

                #region Dados TL1

                objBanco.IpTL1 = view.IpTL1;
                objBanco.LoginTL1 = view.LoginTL1;
                objBanco.SenhaTL1 = view.SenhaTL1;
                objBanco.PortaTL1 = view.PortaTL1;
                objBanco.PortaTL1_SSH = view.PortaTL1_SSH;

                objBanco.Gabinete = view.Gabinete;
                objBanco.Subrack = view.Subrack;

                #endregion

                objBanco.Tipo_Identificacao = view.Tipo_Identificacao;
                objBanco.LimitePotenciaONUBaixa = !String.IsNullOrEmpty(view.LimitePotenciaONUBaixa) ? double.Parse(view.LimitePotenciaONUBaixa.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) : (double?)null;
            }
        }

        [HttpPost()]
        [Route("api/OLTapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_OLT WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = false;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/OLTapi/FindById")]
        public async Task<GetApiRetorno<OLTCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<OLTCadastroModel> retorno = new GetApiRetorno<OLTCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<OLT>(session))
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

        [AllowAnonymous]
        [HttpGet()]
        [Route("api/OLTapi/FindByCodigoAuxiliar")]
        public async Task<GetApiRetorno<GetOLTApiResponse>> FindById(String CodigoAuxiliar)
        {
            GetApiRetorno<GetOLTApiResponse> retorno = new GetApiRetorno<GetOLTApiResponse>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<OLT>(session))
            using (var repositoryPON = new Conexao<PON>(session))
            using (var configApi = new Configuracoes_GeraisApiController())
            {
                try
                {
                    var configGerais = (await configApi.FindById(1)).Objeto;
                    var obj = await repository.selectFirst(p => p.CodigoAuxiliar == CodigoAuxiliar);
                    retorno.Objeto = GetOLTApiResponseObj(obj);

                    if(retorno.Objeto != null)
                    {
                        retorno.Objeto.CriticarBaixaLoginRadiusOffline = configGerais.CriticarBaixaLoginRadiusOffline;
                        List<PON> PON = repositoryPON.select(m => m.OLT != null).Result.Where(p => p.OLT.NumOLT == obj.NumOLT).ToList();

                        if(PON != null && PON.Count > 0)
                        {
                            retorno.Objeto.ListaPON = new List<PON_ModelList>();
                            foreach (var item in PON)
                            {
                                var inserirItem = new PON_ModelList()
                                {
                                    Descricao = item.Descricao,
                                    PONID = item.PONID,
                                    Num_PON = item.Num_PON,
                                    NumSlot = item.Slot.NumSlot,
                                    LimitePotenciaONUBaixa = item.LimitePotenciaONUBaixa,
                                    VLAN_Uplink = item.VLAN_Uplink,
                                };

                                retorno.Objeto.ListaPON.Add(inserirItem);
                            }
                        }

                        if (configGerais.CriticarBaixaPotenciaONU && (!String.IsNullOrEmpty(configGerais.LimitePotenciaONUBaixa) || !String.IsNullOrEmpty(retorno.Objeto.LimitePotenciaONUBaixa)))
                        {
                            retorno.Objeto.CriticarBaixaPotenciaONU = true;

                            if(String.IsNullOrEmpty(retorno.Objeto.LimitePotenciaONUBaixa))
                            retorno.Objeto.LimitePotenciaONUBaixa = configGerais.LimitePotenciaONUBaixa;
                        }
                    }
                                       
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    //retorno.ErrorMessage = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("api/OLTapi/FindByGrupoArea")]
        public async Task<GetApiRetorno<List<GetOLTApiResponse>>> FindByGrupoArea(String Cod_Escritorio, String Cod_Area)
        {
            GetApiRetorno<List<GetOLTApiResponse>> retorno = new GetApiRetorno<List<GetOLTApiResponse>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<OLT>(session))
            using (var repoOLTGrupoArea = new Conexao<OLT_Grupo_Area>(session))
            using (var configApi = new Configuracoes_GeraisApiController())
            {
                try
                {
                    var configGerais = (await configApi.FindById(1)).Objeto;
                    var lista_grupo_area = (await repoOLTGrupoArea.select(p => p.Cod_Escritorio == Cod_Escritorio && p.Cod_Area == Cod_Area && p.Status == Status_Default.Ativo)).ToList();

                    var lista_olts = lista_grupo_area.GroupBy(g => g.OLT).Select(p => p.Key).Where(w => w.Status == Status_Default.Ativo).ToList();                    

                    if (lista_olts.Count > 0)
                    {
                        retorno.Objeto = new List<GetOLTApiResponse>();

                        foreach(var olt in lista_olts)
                        {
                            var oltModel = GetOLTApiResponseObj(olt);

                            if (oltModel != null)
                            {
                                oltModel.CriticarBaixaLoginRadiusOffline = configGerais.CriticarBaixaLoginRadiusOffline;

                                if (configGerais.CriticarBaixaPotenciaONU && (!String.IsNullOrEmpty(configGerais.LimitePotenciaONUBaixa) || !String.IsNullOrEmpty(oltModel.LimitePotenciaONUBaixa)))
                                {
                                    oltModel.CriticarBaixaPotenciaONU = true;

                                    if (String.IsNullOrEmpty(oltModel.LimitePotenciaONUBaixa))
                                        oltModel.LimitePotenciaONUBaixa = configGerais.LimitePotenciaONUBaixa;
                                }
                            }

                            retorno.Objeto.Add(oltModel);
                        }
                        
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        
        [AllowAnonymous]
        [HttpGet()]
        [Route("api/OLTapi/FindByNumCaixa")]
        public async Task<GetApiRetorno<GetOLTApiResponse>> DescricaoCaixa(int caixaNumero)
        {
            GetApiRetorno<GetOLTApiResponse> retorno = new GetApiRetorno<GetOLTApiResponse>();

            if (caixaNumero == 0)
            {
                retorno.Ok = false;
                retorno.Mensagem = "DescricaoCaixa cannot be empty.";
                return retorno;
            }

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<OLT>(session))
            using (var repoOLTGrupoArea = new Conexao<OLT_Grupo_Area>(session))
            using (var configApi = new Configuracoes_GeraisApiController())
            using (var repoElemRede = new Conexao<Elemento_Rede>(sessionT5))
            {
                try
                {
                    var configGerais = (await configApi.FindById(1)).Objeto;
                    var oltBydescricaoCaixa = (await repoElemRede.selectFirst(p => p.Id == caixaNumero));
                    var olt = (await repository.selectFirst(p => p.Id == oltBydescricaoCaixa.Id_OLT));
                    var oltModel = GetOLTApiResponseObj(olt);

                    if (oltModel != null)
                    {
                        oltModel.CriticarBaixaLoginRadiusOffline = configGerais.CriticarBaixaLoginRadiusOffline;

                        if (configGerais.CriticarBaixaPotenciaONU && (!String.IsNullOrEmpty(configGerais.LimitePotenciaONUBaixa) || !String.IsNullOrEmpty(oltModel.LimitePotenciaONUBaixa)))
                        {
                            oltModel.CriticarBaixaPotenciaONU = true;

                            if (String.IsNullOrEmpty(oltModel.LimitePotenciaONUBaixa))
                                oltModel.LimitePotenciaONUBaixa = configGerais.LimitePotenciaONUBaixa;
                        }
                    }

                    retorno.Objeto = oltModel; 
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/OLTapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<OLTCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                OLT modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<OLT>(session))
                    using (var repoOLTGrupoArea = new Conexao<OLT_Grupo_Area>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new OLT
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo,
                                ConverteOlt = Status_Default.Ativo,
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.ConverteOlt = value.Objeto.ConverteOlt ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        if(value.Objeto.Delay > 10000)
                        {
                            throw new Exception("O valor máximo permitido para o campo 'Delay' é 10.000 milissegundos. Por favor, insira um valor igual ou inferior a 10.000.");

                        }
                        await repository.SaveOrUpdate(modelo);

                        List<OLT_Grupo_Area> listagruposArea = new List<OLT_Grupo_Area>();
                        await BindViewToObjGrupoArea(session, modelo, value.Objeto, listagruposArea);

                        await repoOLTGrupoArea.SaveOrUpdate(listagruposArea);
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

        [HttpPost()]
        [Route("api/OLTapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_OLT SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        public async Task<GetApiRetorno<IEnumerable<OLTListaModel>>> GetListOLT()
        {
            GetApiRetorno<IEnumerable<OLTListaModel>> retorno = new GetApiRetorno<IEnumerable<OLTListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoOlt = new Conexao<OLT>(session))
            {

                try
                {
                    var lista = await repoOlt.queryover().Where(m => m.Status == Status_Default.Ativo).ListAsync();

                    retorno.Objeto = lista.Select(p => new OLTListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        //Nome = p.Nome

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
        public async Task<GetApiRetorno<IEnumerable<OLTListaModel>>> GetListIp()
        {
            GetApiRetorno<IEnumerable<OLTListaModel>> retorno = new GetApiRetorno<IEnumerable<OLTListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoOlt = new Conexao<OLT>(session))
            {

                try
                {
                    //var lista = await repoOlt.select().Result;
                    var lista = await repoOlt.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new OLTListaModel
                    {
                        Ip = p.IP,
                        //Descricao = p.Descricao,
                        
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

        private async Task BindViewToObjGrupoArea(ISession session, OLT objBanco, OLTCadastroModel view, List<OLT_Grupo_Area> listagruposArea)
        {
            foreach (var grupoArea in view.GruposArea)
            {
                if (grupoArea.Id == 0)
                {
                    listagruposArea.Add(new OLT_Grupo_Area
                    {
                        Cod_Escritorio = grupoArea.Cod_Escritorio,
                        Cod_Area = grupoArea.Cod_Area,
                        Desc_Escritorio = grupoArea.Desc_Escritorio,
                        Desc_Area = grupoArea.Desc_Area,
                        OLT = objBanco,
                        Status = Sinapse.Model.enumerados.Status_Default.Ativo
                    });
                }
            }
        }

        [HttpPost()]
        [Route("api/OLTApi/CriarSlotsEPONsEmMassa")]
        public async Task<GetApiRetorno<bool>> CriarSlotsEPONsEmMassa(CriarSlotsEPONsEmMassa_Request value)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();
            bool slot_existe = false;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOlt = new Conexao<OLT>(session))
                using (var repoSlot = new Conexao<Slot>(session))
                using (var repoPON = new Conexao<PON>(session))
                {
                    if (value.Vlan_PON_Incremento.HasValue && !value.Vlan_PON_Inicial.HasValue)
                    {
                        throw new Exception("Por favor, insira o valor da VLAN Uplink ou remova o valor do campo Incremento VLAN Uplink.");
                    }

                    OLT olt = await repoOlt.selectFirst(p => p.Id == value.Id_OLT.Value);
                    List<Slot> lista_slot = new List<Slot>();
                    var lista_slots_banco = (await repoSlot.select(p => p.OLT.Id == olt.Id)).ToList();

                    if (!value.Num_Slot_Fim.HasValue)
                    {
                        value.Num_Slot_Fim = value.Num_Slot_Inicial;
                    }                  

                    for (var i = value.Num_Slot_Inicial; i <= value.Num_Slot_Fim; i++)
                    {
                        if (!lista_slots_banco.Exists(x => x.NumSlot == i))
                        {
                            lista_slot.Add(new Slot
                            {
                                DataHoraCriacao = DateTime.Now,
                                OLT = olt,
                                SlotID = (await repoSlot.Max(p => p.SlotID, y => y.OLT.Id == olt.Id)) + 1,
                                NumSlot = i,
                                Status = Status_Default.Ativo,
                                Descricao = StringService.Truncate(String.Format("SLOT {0} - {1}", i, olt.Descricao),100)
                            });
                        }
                        else
                        {
                            slot_existe = true;
                            lista_slot.Add(lista_slots_banco.Find(x => x.NumSlot == i));
                        }
                    }
                    short? valorVlan = null;

                    //Variável responsável por sinalizar caso o looping esteja sendo iniciado pela primeira vez.
                    //Se sim (true), a VLAN Uplink será adicioanda sem incrementar o valor.
                    //Se não for a primeira vez (false), o valor da VLAN Uplink será incrementado.
                    bool primeiraVez = true;

                    if(value.Vlan_PON_Inicial.HasValue)
                    {
                        valorVlan = value.Vlan_PON_Inicial;
                    }

                    foreach (var slot in lista_slot)
                    {
                        await repoSlot.SaveOrUpdate(slot);
                        if (!value.Num_PON_Fim.HasValue)
                        {
                            value.Num_PON_Fim = value.Num_PON_Inicial;
                        }
                       
                            List<PON> lista_pons = new List<PON>();
                        var lista_pons_banco = (await repoPON.select(p => p.OLT.Id == olt.Id && p.Slot.Id == slot.Id)).ToList();

                        for (var i = value.Num_PON_Inicial; i <= value.Num_PON_Fim; i++)
                        {
                            //Caso haja o valor do incremento da VLAN
                            //e não for a primeira vez que o loop esteja iterando,
                            //o valor da vlan será incrementado.
                            if (value.Vlan_PON_Incremento.HasValue && primeiraVez == false)
                            {
                                valorVlan += value.Vlan_PON_Incremento;
                            }

                            if (!lista_pons_banco.Exists(x => x.Num_PON == i))
                            {
                                lista_pons.Add(new PON
                                {
                                    DataHoraCriacao = DateTime.Now,
                                    OLT = olt,
                                    Slot = slot,
                                    PONID = (await repoPON.Max(p => p.PONID, y => y.Slot.Id == slot.Id)) + 1,
                                    Num_PON = i,
                                    Status = Status_Default.Ativo,
                                    Descricao = StringService.Truncate(String.Format("PON {0} - {1} - {2}", i, slot.Descricao, olt.Descricao),100),
                                    Potencia_Saida = 0,
                                    VLAN_Uplink = value.Vlan_PON_Inicial.HasValue ? valorVlan : (short?)null

                                });
                            }
                            else
                            {
                                PON pon = lista_pons_banco.FirstOrDefault(x => x.Num_PON == i);
                                if (pon != null)
                                {   
                                    if (value.Vlan_PON_Incremento.HasValue && primeiraVez == false)
                                    {
                                        valorVlan += value.Vlan_PON_Incremento;
                                    }

                                    if (value.Vlan_PON_Inicial.HasValue)
                                        pon.VLAN_Uplink = valorVlan;

                                    lista_pons.Add(pon);
                                }
                            }

                            primeiraVez = false;
                        }
                        if(lista_pons.Count > 0)
                        {
                            await repoPON.SaveOrUpdate(lista_pons);
                        }
                    }
                }
                retorno.Ok = true;                
            }
            catch(Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }
            
            return retorno;
        }
    }
}
