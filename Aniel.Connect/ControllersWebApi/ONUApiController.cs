using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.T5.InventarioCliente;
using Aniel.Connect.Repository.T5.Cliente;
using Microsoft.AspNet.Identity;
using NHibernate;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Xceed.Words.NET;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ONUApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/ONUapi/FindById")]
        public async Task<GetApiRetorno<VinculoONUCliente>> FindById(Int64 Id)
        {
            GetApiRetorno<VinculoONUCliente> retorno = new GetApiRetorno<VinculoONUCliente>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<ONU>(session))
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

        private VinculoONUCliente GetViewObj(ONU obj)
        {
            return new VinculoONUCliente
            {
                Id = obj.Id,
                DescricaoPon = $"{obj.PON?.Descricao}-(PONID: {obj.PON?.GetPonID()})",
                //Equipe = obj.
                Estatico_Gatway = obj.Estatico_Gatway,
                Estatico_Ip_Mask = obj.Estatico_Ip_Mask,
                Estatico_Nro_Ip = obj.Estatico_Nro_Ip,
                Id_Cliente = obj.Cliente_PlanoProduto?.Cliente?.Id,
                Id_Cliente_PlanoProduto = obj.Cliente_PlanoProduto?.Id,
                Id_PlanoProduto = obj.Cliente_PlanoProduto?.Plano?.Id ?? obj.Cliente_PlanoProduto?.Produto?.Id,
                Id_Pon = obj.PON.Id,
                Ip_Final = obj.Ip_Final,
                Ip_Inicio = obj.Ip_Inicio,
                Ip_Mask = obj.Ip_Mask,
                Login = obj.Cliente_PlanoProduto?.Login_Radius,
                Senha = obj.Cliente_PlanoProduto?.Senha_Radius,
                LoginAdmin = obj.Login,
                SenhaAdmin = obj.Senha,
                Mac = obj.Mac,
                Nro_Ip = obj.Nro_Ip,
                PONID = obj.PON?.GetPonID(),
                PortaVLan = obj.PortaVLan,
                Senha_WPA2 = obj.Senha_WPA2,
                Serial = obj.Serial,
                TipoONU = obj.Descricao,
                Tipo_Conexao = obj.Tipo_Conexao,
                Tipo_Identificacao = obj.Tipo_Identificacao,
                Wireless_SSID = obj.Wireless_SSID,
            };
        }

        [HttpPost()]
        [Route("api/ONUapi/GravarONUVinculoCliente")]
        public async Task<PostApiRetorno<Int64>> GravarONUVinculoCliente([FromBody] ParamWebApi<VinculoONUCliente> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repoCliente = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPON = new Conexao<PON>(session))
                {
                    try
                    {
                        Cliente_PlanoProduto cliente_PlanoProduto = await repoCliente.selectFirst(p => p.Id == value.Objeto.Id_Cliente_PlanoProduto);
                        ONU onu = await repoONU.selectFirst(p => p.Cliente_PlanoProduto == cliente_PlanoProduto);

                        if (onu == null)
                        {
                            onu = new ONU
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            onu.UsuarioAlteracao = value.Usuario;
                        }

                        onu.Cliente_PlanoProduto = cliente_PlanoProduto;
                        onu.PON = await repoPON.selectFirst(p => p.Id == value.Objeto.Id_Pon);

                        onu.Mac = value.Objeto.Mac;
                        onu.Serial = value.Objeto.Serial;
                        onu.Descricao = value.Objeto.TipoONU;

                        onu.Nro_Ip = value.Objeto.Nro_Ip;
                        onu.Ip_Inicio = value.Objeto.Ip_Inicio;
                        onu.Ip_Final = value.Objeto.Ip_Final;
                        onu.Ip_Mask = value.Objeto.Ip_Mask;

                        onu.Estatico_Gatway = value.Objeto.Estatico_Gatway;
                        onu.Estatico_Ip_Mask = value.Objeto.Estatico_Ip_Mask;
                        onu.Estatico_Nro_Ip = value.Objeto.Estatico_Nro_Ip;

                        onu.Ip_Final = value.Objeto.Ip_Final;
                        onu.Ip_Inicio = value.Objeto.Ip_Inicio;
                        onu.Ip_Mask = value.Objeto.Ip_Mask;
                        onu.Login = value.Objeto.LoginAdmin;
                        onu.Senha = value.Objeto.SenhaAdmin;
                        onu.Mac = value.Objeto.Mac;
                        onu.Nro_Ip = value.Objeto.Nro_Ip;
                        onu.PortaVLan = value.Objeto.PortaVLan;
                        onu.Senha_WPA2 = value.Objeto.Senha_WPA2;
                        onu.Tipo_Conexao = value.Objeto.Tipo_Conexao;
                        onu.Tipo_Identificacao = value.Objeto.Tipo_Identificacao;
                        onu.Wireless_SSID = value.Objeto.Wireless_SSID;

                        await repoONU.SaveOrUpdate(onu);

                        cliente_PlanoProduto.Login_Radius = value.Objeto.Login;
                        cliente_PlanoProduto.Senha_Radius = value.Objeto.Senha;

                        await repoCliente.SaveOrUpdate(cliente_PlanoProduto);

                        retorno.IdObjeto = onu.Id;
                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagem = ex.Message;
                        if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                    }
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/ClienteApi/GetTreeViewFibraExport")]
        public async Task<DataTableResponse<GetTreeViewFibraExportModels>> GetTreeViewFibraExport(EstruturaFibraDataTableRequest request)
        {
            List<GetTreeViewFibraExportModels> retorno = new List<GetTreeViewFibraExportModels>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.LOGIN, C.NOMEFANTASIA, D.DESCRICAO AS PLANO, B.ID AS ID_CLIENTE_PLANO_PRODUTO, C.ID AS ID_CLIENTE,");
                    sql.AppendLine("E.DESCRICAO PON, F.DESCRICAO SLOT, G.DESCRICAO OLT, H.NOME FILIAL, I.NOME EMPRESA");
                    sql.AppendLine("FROM TB_ONU A");
                    sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B");
                    sql.AppendLine("ON(A.ID_CLIENTE_PLANO_PRODUTO = B.ID)");
                    sql.AppendLine("INNER JOIN TB_CLIENTE C");
                    sql.AppendLine("ON(B.ID_CLIENTE = C.ID)");
                    sql.AppendLine("INNER JOIN TB_PLANO D");
                    sql.AppendLine("ON(B.ID_PLANO = D.ID)");
                    sql.AppendLine("INNER JOIN TB_PON E");
                    sql.AppendLine("ON(A.ID_PON = E.ID)");
                    sql.AppendLine("INNER JOIN TB_SLOT F");
                    sql.AppendLine("ON(E.ID_SLOT = F.ID)");
                    sql.AppendLine("INNER JOIN TB_OLT G");
                    sql.AppendLine("ON(F.ID_OLT = G.ID)");
                    sql.AppendLine("INNER JOIN TB_FILIAL H");
                    sql.AppendLine("ON(G.ID_FILIAL = H.ID)");
                    sql.AppendLine("INNER JOIN TB_EMPRESA I");
                    sql.AppendLine("ON(H.ID_EMPRESA = I.ID)");
                    sql.AppendLine(" WHERE A.ID > 0");


                    if (request.Id_Empresa.HasValue)
                    {
                        sql.AppendLine("AND I.ID  = @ID_EMPRESA");
                        parametros.Add("@ID_EMPRESA", request.Id_Empresa.Value);
                    }

                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine("AND H.ID  = @ID_FILIAL");
                        parametros.Add("@ID_FILIAL", request.Id_Filial.Value);
                    }

                    if (request.Id_OLT.HasValue)
                    {
                        sql.AppendLine("AND G.ID  = @ID_OLT");
                        parametros.Add("@ID_OLT", request.Id_OLT.Value);
                    }

                    if (request.Id_SLOT.HasValue)
                    {
                        sql.AppendLine("AND F.ID  = @ID_SLOT");
                        parametros.Add("@ID_SLOT", request.Id_SLOT.Value);
                    }

                    if (request.Id_PON.HasValue)
                    {
                        sql.AppendLine("AND I.ID  = @ID_PON");
                        parametros.Add("@ID_PON", request.Id_PON.Value);
                    }


                    var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new GetTreeViewFibraExportModels
                        {

                            Id = Convert.ToInt64(reader["ID"]),
                            Equipamento = $"{reader["DESCRICAO"]} - {reader["LOGIN"]}",
                            Cliente = Convert.ToString(reader["NOMEFANTASIA"]),
                            Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                            Id_ClientePlanoProduto = Convert.ToInt64(reader["ID_CLIENTE_PLANO_PRODUTO"]),
                            Login = Convert.ToString(reader["LOGIN"]),
                            Plano = Convert.ToString(reader["PLANO"]),
                            Empresa = Convert.ToString(reader["EMPRESA"]),
                            Filial = Convert.ToString(reader["FILIAL"]),
                            OLT = Convert.ToString(reader["OLT"]),
                            PON = Convert.ToString(reader["PON"]),
                            SLOT = Convert.ToString(reader["SLOT"]),
                        });
                    }


                }
            }
            return new DataTableResponse<GetTreeViewFibraExportModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count,
                data = retorno,
                error = ""
            };
        }

        public async Task<GetApiRetorno> ImportacaoModelo(ONU_Importacao item)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {

                var onuModel = new Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel();


                var existeOnu = await this.ExisteONUMesmoSerialOuLogin(item.Serial?.Trim(), item.Login_Radius?.Trim());

                if (existeOnu?.MesmoLogin == true)
                {
                    throw new Exception("O Login Radius já está vinculado a outra ONU. ONU não foi importada.");
                }

                if (existeOnu?.MesmoSerial == true)
                {
                    throw new Exception("Já existe ONU cadastrada em sistema com o mesmo serial. ONU não foi importada.");
                }

                PerfilAtivacao perfilAtivacao = null;
                ImportacaoONUDadosLoginModel login = null;


                if (String.IsNullOrEmpty(item.Descricao_OLT?.Trim()) || item.Num_Slot == null || item.Num_Pon == null)
                {
                    throw new Exception("Não é possível importar ONU sem informações de OLT, Slot e Pon.");
                }

                var interfacePON = await new PonApiController().GetPonByInterface(item.Descricao_OLT?.Trim() ?? string.Empty, item.Num_Slot.Value, item.Num_Pon.Value);


                //olt = await repoOLT.selectFirst(p => p.Descricao == item.Descricao_OLT);
                if (interfacePON?.Id_OLT == null)
                    throw new Exception("OLT não encontrada no sistema.");

                //slot = await repoSlot.selectFirst(p => p.OLT == olt && p.NumSlot == item.Num_Slot);
                if (interfacePON?.Id_Slot == null)
                    throw new Exception("Slot não encontrado no sistema.");

                //pon = await repoPON.selectFirst(p => p.OLT == olt && p.Slot == slot && p.Num_PON == item.Num_Pon);
                if (interfacePON?.Id == null)
                    throw new Exception("Pon não encontrada no sistema.");

                    
                if(!String.IsNullOrEmpty(item.ONTID?.Trim()))
                {
                    var ontid = Convert.ToInt32(item.ONTID?.Trim());
                    if (ontid > 0)
                    {
                        if (await this.ExisteONUMesmoONTID(interfacePON.Id_OLT.Value, interfacePON.Id_Slot.Value, interfacePON.Id, ontid))
                        {
                            throw new Exception("Já existe uma ONU cadastrada em sistema para esta interface (OLT/Slot/Pon/OntID). ONU não foi importada.");
                        }
                    }

                }

                /*if ((await repoONU.selectFirst(p => p.OLT.Id == olt.Id && p.Slot.Id == slot.Id && p.PON.Id == pon.Id && p.OntID == item.ONTID)) != null)
                {
                    throw new Exception("Já existe uma ONU cadastrada em sistema para esta interface (OLT/Slot/Pon/OntID). ONU não foi importada.");
                } */


                //login = await repoLogin.selectFirst(p => p.Login == item.Login_Radius);                    
                login = await this.GetDadosLogin(item.Login_Radius?.Trim());
                if (login == null)
                    throw new Exception("Login Radius não encontrado no sistema.");


                if (item.Id_Perfil_Ativacao.HasValue)
                {
                    //perfilAtivacao = await repoPerfilAtivacao.selectFirst(p => p.Id == item.Id_Perfil_Ativacao);
                    perfilAtivacao = await new PerfilAtivacaoApiController().FindByIdSQL(item.Id_Perfil_Ativacao.Value);
                    if (perfilAtivacao == null)
                        throw new Exception("Perfil de Ativação não encontrado no sistema.");
                }


                if(!String.IsNullOrEmpty(item.ONTID?.Trim()))
                {
                    onuModel.OntID = Convert.ToInt32(item.ONTID?.Trim());
                }
                else
                {
                    onuModel.OntID = 0;
                }

                onuModel.Descricao = item.Descricao_ONT?.Trim();
                onuModel.Serial = item.Serial?.Trim();
                onuModel.Mac = item.MAC?.Trim() ?? StringService.Truncate(item.Serial?.Trim(), 20);

                using (var session = HibernateUtil.GetSession())
                {
                    //Conversão de serial
                    onuModel.SerialConvertido = await new AutorizarONUApiController().ConverteONUVendorHex(session, onuModel.Serial ?? onuModel.Mac);
                }
                    
                onuModel.GemPort = item.Gem_Port ?? perfilAtivacao?.GemPort;
                onuModel.ServicePort = item.Service_Port ?? perfilAtivacao?.ServicePort;
                onuModel.SenorId = item.Senor_Id ?? perfilAtivacao?.SenorId;
                onuModel.PortaTelnet = item.Porta_Telnet ?? perfilAtivacao?.PortaTelnet;
                onuModel.PortaHttp = item.Porta_Http ?? perfilAtivacao?.PortaHttp;
                onuModel.VLAN_Uplink = item.Vlan_Uplink;
                onuModel.VLAN_PPPOE = item.Vlan_PPPOE;
                onuModel.VLAN_DHCP = item.Vlan_DHCP;
                onuModel.VLAN_IPTV = item.Vlan_IPTV;
                onuModel.VLAN_VOIP = item.Vlan_VOIP;
                onuModel.VLAN_TR69 = item.Vlan_TR69;
                onuModel.VLAN_Outros = item.Vlan_Outros;
                onuModel.VLAN_Profile = item.Vlan_Profile?.Trim() ?? string.Empty;

                onuModel.Tipo_Identificacao = interfacePON.ONU_Tipo_Identificacao == 2
                    ? Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID
                    : Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC;

                onuModel.Login = login?.Login.Trim();
                onuModel.IdLogin = login?.Id;

                if (login.Id_Cliente_PlanoProduto.HasValue)
                    onuModel.IdPlano = login.Id_Cliente_PlanoProduto.Value;
                else
                    throw new Exception("Login não possui Plano vinculado no sistema.");

                onuModel.IdOLT = interfacePON.Id_OLT;
                onuModel.IdSlot = interfacePON.Id_Slot;
                onuModel.IdPon = interfacePON.Id;

                if (!string.IsNullOrEmpty(item.Gerar_Inventario_Mat?.Trim()) && item.Gerar_Inventario_Mat.Trim().ToUpperInvariant() == "S")
                {
                    if (item.validaMaterial()?.Count() > 0)
                    {
                        throw new Exception(item.validaMaterial().FirstOrDefault()?.Erro ?? "Erro na validação de material para geração do Inventário de Cliente");
                    }
                    else
                    {
                        using (var sessionT5 = HibernateUtil.GetSessionT5())
                        using (var transactionT5 = sessionT5.BeginTransaction())
                        {
                            try
                            {
                                IInventarioClienteAnielRepository inventarioClienteAnielRepository = new InventarioClienteAnielRepository(sessionT5, transactionT5);
                                InventarioClienteAnielBusiness inventarioClienteAnielBusiness = new InventarioClienteAnielBusiness(inventarioClienteAnielRepository);
                                IClienteAnielRepository clienteAnielRepository = new ClienteAnielRepository(sessionT5);
                                ClienteAnielBusiness clienteAnielBusiness = new ClienteAnielBusiness(clienteAnielRepository);

                                var clienteAniel = clienteAnielBusiness.GetClientebyCPFCNPJ(login.CpfCnpj_Cliente);


                                if (await inventarioClienteAnielBusiness.ExisteInventarioPendenteAvaliacao(clienteAniel.CPF, item.CodMat?.Trim(), onuModel.Serial))
                                    throw new Exception("Já existe um inventário pendente de avaliação para este equipamento.");

                                await inventarioClienteAnielBusiness.GerarInventarioCliente(clienteAniel, User.Identity.GetUserId(), item.CodMat?.Trim(), onuModel.Serial);
                                await transactionT5.CommitAsync();



                            }
                            catch (Exception ex)
                            {
                                await transactionT5.RollbackAsync();
                                CONSTANTES.log4Net.Error(ex.Message, ex);
                                throw new Exception("Ocorreu um erro ao gerar o Inventário do Cliente.", ex);
                            }
                        }
                    }
                }

                await this.InsertONU(onuModel, User.Identity.GetUserId());
                retorno.Ok = true;
            }
            catch (Exception e)
            {
                var logImportacaoOnu = new LogImportacaoOnu
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Serial_Equipamento = (item.Serial ?? string.Empty).Trim(),
                    Erro = e.InnerException?.Message != null ? (e.Message + e.InnerException?.Message) : e.Message,
                    Usuario = User.Identity.GetUserId()
                };

                await GravaLogImportacao(logImportacaoOnu);

                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/ONUapi/LogImportacaoOnu")]
        public async Task<DataTableResponse<LogImportacaoOnu>> Log(LogImportacaoONURequest request)
        {
            List<LogImportacaoOnu> retorno = new List<LogImportacaoOnu>();
            var recordsTotal = 0;

            using (var session = HibernateUtil.GetSessionLog())
            {
                StringBuilder sqlWhere = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("DATA", Convert.ToDateTime(request.Data).ToString("dd-MM-yyyy"));

                    sqlWhere.AppendLine(" WHERE DATA = @DATA ");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) AS CO FROM TB_LOG_IMPORTACAO_ONU ");
                    sqlCount.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT * FROM TB_LOG_IMPORTACAO_ONU ");
                    sql.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogImportacaoOnu { Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy"), Hora = Convert.ToDateTime(reader["HORA"].ToString()).ToString("HH:mm"), Erro = reader["ERRO"].ToString(), Serial_Equipamento = reader["SERIAL_EQUIPAMENTO"].ToString(), Usuario = reader["USUARIO"].ToString() });
                        }
                    }
                }
            }

            return new DataTableResponse<LogImportacaoOnu>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        public async Task GravaLogImportacao(LogImportacaoOnu log)
        {
            using (var session = HibernateUtil.GetSessionLog())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_LOG_IMPORTACAO_ONU (ID,DATA,HORA,ERRO,SERIAL_EQUIPAMENTO,USUARIO)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_LOG_IMPORTACAO_ONU, 1) FROM RDB$DATABASE),@DATA,@HORA,@ERRO,@OPERACAO,@USUARIO)");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@DATA", log.Data.Replace('/', '-'));
                parametros.Add("@HORA", log.Hora);
                parametros.Add("@ERRO", StringService.Truncate(log.Erro, 255));
                parametros.Add("@OPERACAO", log.Serial_Equipamento);
                parametros.Add("@USUARIO", log.Usuario);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task InsertONU(Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel model, string usuarioCriacao)
        {
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_ONU (ID, DATAHORACRIACAO, USUARIOCRIACAO, DESCRICAO, MAC, SERIAL, GEM_PORT, SERVICEPORT, SENORID, LOGIN, ");
                sql.AppendLine(" PORTA_TELNET, ONT_ID, PORTA_HTTP, VLAN_UPLINK, VLAN_PPPOE, VLAN_DHCP, VLAN_IPTV, VLAN_VOIP, VLAN_TR69, VLAN_OUTROS, VLAN_PROFILE, ID_LOGIN_RADIUS, ID_CLIENTE_PLANO_PRODUTO, ID_OLT, ID_SLOT, ID_PON, TIPO_IDENTIFICACAO, SERIAL_CONVERTIDO)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_ONU, 1) FROM RDB$DATABASE), CURRENT_TIMESTAMP, @USUARIOCRIACAO, @DESCRICAO, @MAC, @SERIAL, @GEM_PORT, @SERVICEPORT, @SENORID, @LOGIN, ");
                sql.AppendLine($" @PORTA_TELNET, @ONT_ID, @PORTA_HTTP, @VLAN_UPLINK, @VLAN_PPPOE, @VLAN_DHCP, @VLAN_IPTV, @VLAN_VOIP, @VLAN_TR69, @VLAN_OUTROS, @VLAN_PROFILE, @ID_LOGIN_RADIUS, @ID_CLIENTE_PLANO_PRODUTO, @ID_OLT, @ID_SLOT, @ID_PON, @TIPO_IDENTIFICACAO, @SERIAL_CONVERTIDO) ");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@USUARIOCRIACAO", usuarioCriacao);
                parametros.Add("@DESCRICAO", model.Descricao);
                parametros.Add("@MAC", model.Mac);
                parametros.Add("@SERIAL", model.Serial);
                parametros.Add("@GEM_PORT", model.GemPort);
                parametros.Add("@SERVICEPORT", model.ServicePort);
                parametros.Add("@SENORID", model.SenorId);
                parametros.Add("@LOGIN", model.Login);
                parametros.Add("@PORTA_TELNET", model.PortaTelnet);
                parametros.Add("@PORTA_HTTP", model.PortaHttp);
                parametros.Add("@ONT_ID", model.OntID.HasValue ? model.OntID.Value : (object)DBNull.Value);
                parametros.Add("@VLAN_UPLINK", model.VLAN_Uplink);
                parametros.Add("@VLAN_PPPOE", model.VLAN_PPPOE);
                parametros.Add("@VLAN_DHCP", model.VLAN_DHCP);
                parametros.Add("@VLAN_IPTV", model.VLAN_IPTV);
                parametros.Add("@VLAN_VOIP", model.VLAN_VOIP);
                parametros.Add("@VLAN_TR69", model.VLAN_TR69);
                parametros.Add("@VLAN_OUTROS", model.VLAN_Outros);
                parametros.Add("@VLAN_PROFILE", model.VLAN_Profile);
                parametros.Add("@ID_LOGIN_RADIUS", model.IdLogin);
                parametros.Add("@ID_CLIENTE_PLANO_PRODUTO", model.IdPlano);
                parametros.Add("@ID_OLT", model.IdOLT);
                parametros.Add("@ID_SLOT", model.IdSlot);
                parametros.Add("@ID_PON", model.IdPon);
                parametros.Add("@TIPO_IDENTIFICACAO", model.Tipo_Identificacao);
                parametros.Add("@SERIAL_CONVERTIDO", model.SerialConvertido);

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<Models.OnuExistenteModel> ExisteONUMesmoSerialOuLogin(string serial, string login)
        {
            Models.OnuExistenteModel retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 A.ID_LOGIN_RADIUS, COALESCE(A.SERIAL,A.MAC) AS SERIAL_EQUIPAMENTO, B.LOGIN FROM TB_ONU A ");
                sql.AppendLine(" LEFT JOIN TB_LOGIN_RADIUS B ");
                sql.AppendLine(" ON (A.ID_LOGIN_RADIUS = B.ID) ");
                sql.AppendLine(" WHERE UPPER(A.SERIAL) = UPPER(@SERIAL) OR UPPER(A.MAC) = UPPER(@SERIAL) OR B.LOGIN = @LOGIN ");

                parametros.Add("@SERIAL", serial);
                parametros.Add("@LOGIN", login);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new Models.OnuExistenteModel();
                        retorno.MesmoLogin = reader["LOGIN"] != DBNull.Value && Convert.ToString(reader["LOGIN"]) == login ? true : false;
                        retorno.MesmoSerial = reader["SERIAL_EQUIPAMENTO"] != DBNull.Value && Convert.ToString(reader["SERIAL_EQUIPAMENTO"]).ToUpperInvariant() == serial.ToUpperInvariant() ? true : false;
                    }
                }
            }

            return retorno;
        }

        public async Task<bool> ExisteONUMesmoONTID(long idOLT, long idSlot, long idPon, int ONTID)
        {
            bool retorno = false;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 A.ID FROM TB_ONU A ");
                sql.AppendLine(" WHERE A.ID_OLT = @ID_OLT AND A.ID_SLOT = @ID_SLOT AND A.ID_PON = @ID_PON AND ONT_ID = @ONTID ");

                parametros.Add("@ID_OLT", idOLT);
                parametros.Add("@ID_SLOT", idSlot);
                parametros.Add("@ID_PON", idPon);
                parametros.Add("@ONTID", ONTID);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = true;
                    }
                }
            }

            return retorno;
        }

        public async Task<ImportacaoONUDadosLoginModel> GetDadosLogin(string loginRadius)
        {
            ImportacaoONUDadosLoginModel retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT FIRST 1 A.ID, A.LOGIN, A.ID_PLANO_PRODUTO, B.CPF AS CPF_CNPJ");
                sql.AppendLine("FROM TB_LOGIN_RADIUS A");
                sql.AppendLine("LEFT JOIN TB_CLIENTE B ON A.ID_CLIENTE = B.ID");
                sql.AppendLine("WHERE UPPER(A.LOGIN) = UPPER(@LOGIN_RADIUS)");

                parametros.Add("@LOGIN_RADIUS", loginRadius);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new ImportacaoONUDadosLoginModel();
                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Id_Cliente_PlanoProduto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : (long?)null;
                        retorno.Login = Convert.ToString(reader["LOGIN"]);
                        retorno.CpfCnpj_Cliente = Convert.ToString(reader["CPF_CNPJ"]);
                    }
                }
            }

            return retorno;
        }


        public async Task<GetApiRetorno> ExclusaoModelo(ONU_Exclusao item)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                if(String.IsNullOrEmpty(item.Serial))
                {
                    throw new Exception("Por favor, insira um serial válido.");
                }

                var serial = item.Serial.Trim();
               
                var existeOnu = await this.GetDadosONUBySerial(serial);

                if (existeOnu == null)
                {
                    throw new Exception("Não foi encontrada uma ONU com este Serial.");
                }

                var infoONU = $"Informações do equipamento -> SERIAL: {existeOnu.Serial}, ID DA ONU: {existeOnu.Ont_Id}, OLT: {existeOnu.DescOLT}, SLOT: {existeOnu.NumSlot}, " +
                    $"PON: {existeOnu.NumPon}, LOGIN: {existeOnu.Login}";

                var logExclusaoOnu = new LogExclusaoOnu
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Info_Equipamento = infoONU,
                    Erro = "",
                    Usuario = User.Identity.GetUserId()
                };

                
                await this.DeleteONUByID(existeOnu.Id);

                await this.GravaLogExclusao(logExclusaoOnu);

                retorno.Ok = true;
            }
            catch (Exception e)
            {
                var logExclusaoOnu = new LogExclusaoOnu
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Info_Equipamento = (item.Serial ?? string.Empty).Trim(),
                    Erro = e.InnerException?.Message != null ? (e.Message + e.InnerException?.Message) : e.Message,
                    Usuario = User.Identity.GetUserId()
                };

                await GravaLogExclusao(logExclusaoOnu);

                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;
            }

            return retorno;
        }

        public async Task<ExclusaoONUDadosEquipamento> GetDadosONUBySerial(string serial)
        {
            ExclusaoONUDadosEquipamento retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT FIRST 1 A.ID, A.SERIAL, A.ONT_ID, B.NUM_SLOT, C.NUM_PON, D.DESCRICAO, E.LOGIN");
                sql.AppendLine("FROM TB_ONU A");
                sql.AppendLine("INNER JOIN TB_SLOT B ON A.ID_SLOT = B.ID");
                sql.AppendLine("INNER JOIN TB_PON C ON A.ID_PON  = C.ID");
                sql.AppendLine("INNER JOIN TB_OLT D ON A.ID_OLT = D.ID");
                sql.AppendLine("INNER JOIN TB_LOGIN_RADIUS E ON A.ID_LOGIN_RADIUS = E.ID");
                sql.AppendLine("WHERE UPPER(A.SERIAL) = UPPER(@SERIAL)");
                sql.AppendLine("OR UPPER(A.MAC) = UPPER(@SERIAL)");
                sql.AppendLine("OR UPPER(A.SERIAL_CONVERTIDO) = UPPER(@SERIAL)");


                parametros.Add("@SERIAL", serial);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new ExclusaoONUDadosEquipamento();
                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Serial = Convert.ToString(reader["SERIAL"]);
                        retorno.DescOLT = Convert.ToString(reader["DESCRICAO"]);
                        retorno.NumSlot = Convert.ToString(reader["NUM_SLOT"]);
                        retorno.NumPon = Convert.ToString(reader["NUM_PON"]);
                        retorno.Login = Convert.ToString(reader["LOGIN"]);
                        retorno.Ont_Id = Convert.ToString(reader["ONT_ID"]);
                    }
                }
            }

            return retorno;
        }


        public async Task DeleteONUByID(long id)
        {
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                try
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" DELETE FROM TB_ONU WHERE ID = @ID");

                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("@ID", id);


                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
               
            }
        }

        public async Task GravaLogExclusao(LogExclusaoOnu log)
        {
            using (var session = HibernateUtil.GetSessionLog())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_LOG_EXCLUSAO_ONU (ID,DATA,HORA,ERRO,INFO_EQUIPAMENTO,USUARIO)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_LOG_EXCLUSAO_ONU, 1) FROM RDB$DATABASE),@DATA,@HORA,@ERRO,@OPERACAO,@USUARIO)");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@DATA", log.Data.Replace('/', '-'));
                parametros.Add("@HORA", log.Hora);
                parametros.Add("@ERRO", StringService.Truncate(log.Erro, 255));
                parametros.Add("@OPERACAO", log.Info_Equipamento);
                parametros.Add("@USUARIO", log.Usuario);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }


        [HttpPost]
        [Route("api/ONUapi/LogExclusaoOnu")]
        public async Task<DataTableResponse<LogExclusaoOnu>> LogExclusao(LogExclusaoONURequest request)
        {
            List<LogExclusaoOnu> retorno = new List<LogExclusaoOnu>();
            var recordsTotal = 0;

            using (var session = HibernateUtil.GetSessionLog())
            {
                StringBuilder sqlWhere = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("DATA", Convert.ToDateTime(request.Data).ToString("dd-MM-yyyy"));

                    sqlWhere.AppendLine(" WHERE DATA = @DATA ");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) AS CO FROM TB_LOG_EXCLUSAO_ONU ");
                    sqlCount.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT * FROM TB_LOG_EXCLUSAO_ONU ");
                    sql.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogExclusaoOnu { Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy"), Hora = Convert.ToDateTime(reader["HORA"].ToString()).ToString("HH:mm"), Erro = reader["ERRO"].ToString(), Info_Equipamento = reader["INFO_EQUIPAMENTO"].ToString(), Usuario = reader["USUARIO"].ToString() });
                        }
                    }
                }
            }

            return new DataTableResponse<LogExclusaoOnu>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        public async Task<ExclusaoONUDadosEquipamento> GetDadosONUById(string id)
        {
            ExclusaoONUDadosEquipamento retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT FIRST 1 A.ID, A.SERIAL, A.ONT_ID, B.NUM_SLOT, C.NUM_PON, D.DESCRICAO, E.LOGIN");
                sql.AppendLine("FROM TB_ONU A");
                sql.AppendLine("INNER JOIN TB_SLOT B ON A.ID_SLOT = B.ID");
                sql.AppendLine("INNER JOIN TB_PON C ON A.ID_PON  = C.ID");
                sql.AppendLine("INNER JOIN TB_OLT D ON A.ID_OLT = D.ID");
                sql.AppendLine("INNER JOIN TB_LOGIN_RADIUS E ON A.ID_LOGIN_RADIUS = E.ID");
                sql.AppendLine("WHERE A.ID = @ID");

                parametros.Add("@ID", id);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new ExclusaoONUDadosEquipamento();
                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Serial = Convert.ToString(reader["SERIAL"]);
                        retorno.DescOLT = Convert.ToString(reader["DESCRICAO"]);
                        retorno.NumSlot = Convert.ToString(reader["NUM_SLOT"]);
                        retorno.NumPon = Convert.ToString(reader["NUM_PON"]);
                        retorno.Login = Convert.ToString(reader["LOGIN"]);
                        retorno.Ont_Id = Convert.ToString(reader["ONT_ID"]);
                    }
                }
            }

            return retorno;
        }

        //Api de exclusão de ONU utilizada no recurso 
        //Excluir Registro ONU, na tela de Listagem de ONUs
        [HttpPost]
        [Route("api/ONUapi/DeleteONUByIDApi")]
        public async Task<Response> DeleteONUByIDApi(ExcluirONURegistroRequest request)
        {
            var response = new Response();
            ExclusaoONUDadosEquipamento existeOnu;

            try
            {
                existeOnu = await this.GetDadosONUById(request.id);

                if (existeOnu == null)
                {
                    throw new Exception("Não foi encontrada uma ONU com este ID.");
                }

                var infoONU = $"Informações do equipamento (Exclusão realizada via recurso Excluir Registro ONU) -> SERIAL: {existeOnu.Serial}, ID DA ONU: {existeOnu.Ont_Id}, OLT: {existeOnu.DescOLT}, SLOT: {existeOnu.NumSlot}, " +
                    $"PON: {existeOnu.NumPon}, LOGIN: {existeOnu.Login}";

                var logExclusaoOnu = new LogExclusaoOnu
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Info_Equipamento = infoONU,
                    Erro = "",
                    Usuario = User.Identity.GetUserId()
                };


                await this.DeleteONUByID(existeOnu.Id);

                await this.GravaLogExclusao(logExclusaoOnu);

                response.result = true;
            }
            catch(Exception ex)
            {
                response.result = false;
                var logExclusaoOnu = new LogExclusaoOnu
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Info_Equipamento = $"Tentativa realizada via Recurso Excluir Registro ONU {request.id}",
                    Erro = ex.InnerException?.Message != null ? (ex.Message + ex.InnerException?.Message) : ex.Message,
                    Usuario = User.Identity.GetUserId()
                };

                await GravaLogExclusao(logExclusaoOnu);

                response.message = ex.Message;
                response.exception = ex.InnerException?.Message;

            }

            return response;
        }
    }

}

