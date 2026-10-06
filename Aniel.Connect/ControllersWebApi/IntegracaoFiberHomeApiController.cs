using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using PrimS.Telnet;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Threading.Tasks;
using System.Text;
using System.Web.Http;
using Sinapse.Model;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Sinapse.Integracao.Ativacao.Models;

namespace Aniel.Connect.ControllersWebApi
{

    [Obsolete("Classe depreciada, os métodos Fiberhome ficam nas classes de implementação Fiberhome_A_B e Fiberhome_A_1")]
    [Authorize]
    public class IntegracaoFiberHomeApiController : BaseApiController
    {

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ONUsPendentesRegistro")]
        public DataTableResponse<AutorizarONUListModel> ONUsPendentesRegistro(Int64 Id_OLT)
        {
            var retorno = new DataTableResponse<AutorizarONUListModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {
                    #region Comandos FiberHome

                    //OLT olt = await repository.selectFirst(p => p.Id == Id_OLT);

                    //using (var client = new Client(olt.IP, Convert.ToInt32(olt.PortaSSH.GetValueOrDefault(22)), new CancellationToken()))
                    //{
                    //    if (client.IsConnected)
                    //    {
                    //        Console.WriteLine("Cliente Conectado");
                    //        var logado = LoginFiberhome(client, olt.Login, olt.Senha);

                    //        if (logado["STATUS"].Equals("COMPLD"))
                    //        {
                    //            // -=-=-=-=-=-=- Comando Para Listar ONUs -=-=-=-=-=-=-=-//
                    //            await client.WriteLineAsync($"LST-UNREGONU::OLTID={olt.IP}:CTAG::;");
                    //            var resposta = client.ReadAsync().Result;

                    //            var mensagem = TratarRetornoListaONUsPendentesRegistro(resposta);
                    //            if (mensagem["STATUS"].Equals("COMPLD"))
                    //            {
                    //                var lista = new List<AutorizarONUListModel>();

                    //                Console.WriteLine(mensagem["BLOCK RECORDS"]);

                    //                foreach (var onu in mensagem["ONUs"] as List<Dictionary<string, string>>)
                    //                {
                    //                    lista.Add(new AutorizarONUListModel
                    //                    {
                    //                        Id = olt.Id,
                    //                        OLT = olt.Descricao,
                    //                        Slot = onu["SLOTNO"],
                    //                        PON = onu["PONNO"],
                    //                        Mac_Serial = onu["MAC"]
                    //                        //Onu_Loid = onu["LOID"],
                    //                        //Onu_Pwd = onu["PWD"],
                    //                        //Onu_Error = onu["ERROR"],
                    //                        //Onu_Authime = onu["AUTHTIME"],
                    //                        //Onu_Modelo = onu["DT"],
                    //                    });
                    //                }

                    //                retorno.data = lista;
                    //            }

                    //            DeslogarServidor(client);
                    //        }
                    //    }
                    //}

                    #endregion

                    // Mock data
                    var data = new List<AutorizarONUListModel>();

                    //data.Add(new AutorizarONUListModel
                    //{
                    //    Id = 14,
                    //    OLT = "DADO FIXO P/ TESTE",
                    //    Slot = "12",
                    //    PON = "22",
                    //    Mac_Serial = "00-88-14-4D-4C-FB"
                    //});

                    retorno.data = data;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoFiberHomeApi/AtivarONU")]
        public async Task<GetApiRetorno<Int64>> AtivarONU(ParamWebApi<AtivacaoONU> value)
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoCodCpl = new Conexao<Codigo_ComplementarAniel>(sessionT5))
            using (ISession session = HibernateUtil.GetSession())
            using (var repoONU = new Conexao<ONU>(session))
            {
                ONU onu = await repoONU.selectFirst(p => p.Cliente_PlanoProduto.Id == value.Objeto.Id_Cliente_Plano_Produto);

                GetApiRetorno<Int64> retorno = null;

                onu.Descricao = value.Objeto.DescricaoMat;

                if (!onu.Registrado)
                {
                    retorno = await Processa_RegistaONUs(onu);
                    onu.Registrado = retorno.Ok;
                }
                else
                {
                    retorno = new GetApiRetorno<long> { Ok = true };
                }

                await repoONU.SaveOrUpdate(onu);

                if (retorno.Ok)
                {
                    retorno = await Processa_LimparConfiguracoesPortaWanOnu(onu);
                }

                if (retorno.Ok)
                {
                    retorno = await Processa_ConfigurarAcessoInternetDaONU(onu);
                }

                if (retorno.Ok)
                {
                    retorno = await Processa_ConfigurarRedeInternaOnu(onu);
                }

                if (retorno.Ok)
                {
                    retorno = await Processa_ModificarInterfaceWebAdministrator(onu);
                }

                var codcpl = await repoCodCpl.selectFirst(p => p.CodCt == value.Objeto.CodCt &&
                                                    p.Projeto == value.Objeto.Projeto &&
                                                    p.CodMat == value.Objeto.CodMat &&
                                                    p.CodCpl == value.Objeto.CodCpl);


                if (codcpl != null)
                {

                    codcpl.Sinal_Ativo = retorno.Ok == true ? "S" : "N";
                    if (codcpl.Sinal_Ativo == "N")
                    {
                        codcpl.Mensagem_Erro = retorno.Mensagem;
                    }
                    else
                    {
                        codcpl.Mensagem_Erro = null;
                    }
                    await repoCodCpl.SaveOrUpdate(codcpl);
                }
                return retorno;
            }
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/RegistraONUs")]
        public async Task<GetApiRetorno<Int64>> RegistraONUs(Int64 Id_ONU)
        {
            return await Processa_RegistaONUs(await GetONU(Id_ONU));
        }

        private async Task<GetApiRetorno<Int64>> Processa_RegistaONUs(ONU onu)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();
            try
            {
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoRegistraONU(onu);

                using (Client client = new Client(olt.IP, Convert.ToInt32(olt.PortaSSH.GetValueOrDefault(22)), new CancellationToken()))
                {

                    if (client.IsConnected)
                    {
                        Console.WriteLine("Cliente conectado");
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            //-=-=-=-=-=-= Comando para registrar as ONUs =-=-=-=-=-=-
                            /* 
                             * AUTHTYPE  pode ser mudado para  
                             * 
                             * MAC
                             * LOID (NUMERO DE SERIE LOGICO)
                             * LOIDONCEON (LOID e MAC será vinculado )
                             * 
                             * */

                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"ADD-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()}:CTAG::AUTHTYPE=MAC,ONUID={onu.Mac},ONUTYPE={onu.Descricao};");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"ADD-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()}:CTAG::AUTHTYPE=LOID,ONUID={onu.Serial},ONUTYPE={onu.Descricao};");
                            }
                            var resposta = client.ReadAsync().Result;

                            var mensagem = TratarMensagemPadrao(resposta);

                            if (mensagem["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Registra ONU \n Status.: {mensagem["STATUS"]} \n Desc.: {mensagem["CODIGO"]} - {mensagem["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Registra ONU \n Status.: {RetornoErroTratado(mensagem)}";
                            }


                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Registra ONU \n Status.: {RetornoErroTratado(logado)}";
                        }

                        DeslogarServidor(client);
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = " Registra ONU \n Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Registra ONU \n {ex.Message}";
            }
            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/LimparConfiguracoesPortaWanOnu")]
        public async Task<GetApiRetorno<Int64>> LimparConfiguracoesPortaWanOnu(Int64 Id_ONU)
        {
            return await Processa_LimparConfiguracoesPortaWanOnu(await GetONU(Id_ONU));
        }

        private async Task<GetApiRetorno<Int64>> Processa_LimparConfiguracoesPortaWanOnu(ONU onu)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoCampoLimpaConfiguracao(onu, pon);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        Console.WriteLine("Cliente Conectado");
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.BRIDGE)
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=2,MODE=2,CONNTYPE=1,VLAN={pon.Id_VLAN_Dados},COS=4,UPORT={onu.PortaVLan};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=2,MODE=2,CONNTYPE=1,VLAN={pon.Id_VLAN_Dados},COS=4,UPORT={onu.PortaVLan};");
                                }
                            }
                            else
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=2,MODE=2,CONNTYPE=2,VLAN={pon.Id_VLAN_Dados},COS=4,UPORT={onu.PortaVLan};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=2,MODE=2,CONNTYPE=2,VLAN={pon.Id_VLAN_Dados},COS=4,UPORT={onu.PortaVLan};");
                                }
                            }
                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Limpa Config Wan \n Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Limpa Config Wan \n Status.: {RetornoErroTratado(m)}";
                            }

                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Limpa Config Wan \n Status.: {RetornoErroTratado(logado)}";
                        }
                        DeslogarServidor(client);
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Limpa Config Wan \n Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Limpa Config Wan- {ex.Message}";
            }
            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ConfigurarAcessoInternetDaONU")]
        public async Task<GetApiRetorno<Int64>> ConfigurarAcessoInternetDaONU(Int64 Id_ONU)
        {
            return await Processa_ConfigurarAcessoInternetDaONU(await GetONU(Id_ONU));
        }

        private async Task<GetApiRetorno<Int64>> Processa_ConfigurarAcessoInternetDaONU(ONU onu)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();
            try
            {
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoCampoConfigurarRede(onu, olt, pon);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {

                        Console.WriteLine("Cliente Conectado");
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {

                            if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.BRIDGE)
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=1,MODE=2,CONNTYPE=1,VLAN={pon.Id_VLAN_Dados},COS=4,QOS=2,NAT=1,UPORT={onu.PortaVLan};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=1,MODE=2,CONNTYPE=1,VLAN={pon.Id_VLAN_Dados},COS=4,QOS=2,NAT=1,UPORT={onu.PortaVLan};");
                                }
                                var m = TratarMensagemPadrao(client.ReadAsync().Result);
                                if (m["STATUS"].Equals("COMPLD"))
                                {
                                    response.Ok = true;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                                }
                                else
                                {
                                    response.Ok = false;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {RetornoErroTratado(m)} ";
                                }
                            }
                            else if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.PPPoE)
                            {
                                Cliente_PlanoProduto cliente_PlanoProduto = onu.Cliente_PlanoProduto;
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,VLAN={pon.Id_VLAN_Dados},COS=4,QOS=2,NAT=1,IPMODE={Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.PPPoE)},PPPOEPROXY=2,PPPOEUSER={cliente_PlanoProduto.Login_Radius},PPPOEPASSWD={cliente_PlanoProduto.Senha_Radius},PPPOENAME=,PPPOEMODE=1,UPORT={onu.PortaVLan};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,VLAN={pon.Id_VLAN_Dados},COS=4,QOS=2,NAT=1,IPMODE={Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.PPPoE)},PPPOEPROXY=2,PPPOEUSER={cliente_PlanoProduto.Login_Radius},PPPOEPASSWD={cliente_PlanoProduto.Senha_Radius},PPPOENAME=,PPPOEMODE=1,UPORT={onu.PortaVLan};");
                                }

                                var m = TratarMensagemPadrao(client.ReadAsync().Result);

                                if (m["STATUS"].Equals("COMPLD"))
                                {
                                    response.Ok = true;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                                }
                                else
                                {
                                    response.Ok = false;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {RetornoErroTratado(m)}";
                                }
                            }
                            else if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.IP_ESTATICO)
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,COS=4,VLAN={pon.Id_VLAN_Dados},QOS=2,NAT=1,IPMODE={Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.IP_ESTATICO)},WANIP={onu.Estatico_Nro_Ip},WANMASK={onu.Estatico_Ip_Mask},WANGATEWAY={onu.Estatico_Gatway},MASTERDNS={olt.DHCP_DNS_Primario},SLAVEDNS={olt.DHCP_DNS_Secundario};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,COS=4,VLAN={pon.Id_VLAN_Dados},QOS=2,NAT=1,IPMODE={Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.IP_ESTATICO)},WANIP={onu.Estatico_Nro_Ip},WANMASK={onu.Estatico_Ip_Mask},WANGATEWAY={onu.Estatico_Gatway},MASTERDNS={olt.DHCP_DNS_Primario},SLAVEDNS={olt.DHCP_DNS_Secundario};");
                                }
                                var m = TratarMensagemPadrao(client.ReadAsync().Result);
                                if (m["STATUS"].Equals("COMPLD"))
                                {
                                    response.Ok = true;
                                    response.Mensagem = $"Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                                }
                                else
                                {
                                    response.Ok = false;
                                    response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                                }
                            }
                            else if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.DHCP)
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,COS=4,VLAN={pon.Id_VLAN_Dados},QOS=2,NAT=1,IPMODE=${Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.DHCP)};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"SET-WANSERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::STATUS=1,MODE=2,CONNTYPE=2,COS=4,VLAN={pon.Id_VLAN_Dados},QOS=2,NAT=1,IPMODE=${Convert.ToInt32(Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.DHCP)};");
                                }
                                var m = TratarMensagemPadrao(client.ReadAsync().Result);
                                if (m["STATUS"].Equals("COMPLD"))
                                {
                                    response.Ok = true;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                                }
                                else
                                {
                                    response.Ok = false;
                                    response.Mensagem = $"Config Acesso Internet \n Status.: {RetornoErroTratado(m)}";
                                }
                            }

                            DeslogarServidor(client);


                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Config Acesso Internet \n Status.: {RetornoErroTratado(logado)}";

                        }

                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Config Acesso Internet \n Erro ao estabelecer conexão.";
                    }

                }

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Config Acesso Internet - {ex.Message}";
            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ConfigurarRedeInternaOnu")]
        public async Task<GetApiRetorno<Int64>> ConfigurarRedeInternaOnu(Int64 Id_ONU)
        {
            return await Processa_ConfigurarRedeInternaOnu(await GetONU(Id_ONU));
        }

        private async Task<GetApiRetorno<Int64>> Processa_ConfigurarRedeInternaOnu(ONU onu)
        {

            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();
            try
            {
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoConfigurarRedeInternar(onu);

                TratarClasseIP(onu.Nro_Ip, onu.Ip_Inicio, onu.Ip_Final);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"CFG-USERDHCPSERVER::OLTID={olt.Id},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::LANIP={onu.Nro_Ip},ENABLE=True,DHCPPOOLSTART={onu.Ip_Inicio},DHCPPOOLEND={onu.Ip_Final},DHCPMASK={onu.Ip_Mask};");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"CFG-USERDHCPSERVER::OLTID={olt.Id},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::LANIP={onu.Nro_Ip},ENABLE=True,DHCPPOOLSTART={onu.Ip_Inicio},DHCPPOOLEND={onu.Ip_Final},DHCPMASK={onu.Ip_Mask};");
                            }

                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Config Rede Interna \n Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Config Rede Interna \n Status.: {RetornoErroTratado(m)}";
                            }

                            DeslogarServidor(client);

                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Config Rede Interna \n Status.: {RetornoErroTratado(logado)}";
                        }

                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Config Rede Interna \n Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Config Rede Interna \n {ex.Message}";

            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ModificarInterfaceWebAdministrator")]
        public async Task<GetApiRetorno<Int64>> ModificarInterfaceWebAdministrator(Int64 Id_ONU)
        {
            return await Processa_ModificarInterfaceWebAdministrator(await GetONU(Id_ONU));
        }

        private async Task<GetApiRetorno<Int64>> Processa_ModificarInterfaceWebAdministrator(ONU onu)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoAcessoAdministrador(onu);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"CFG-WEBADMINISTRATOR::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::WEBUSERNAME={onu.Login},WEBPASSWORD={onu.Senha};");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"CFG-WEBADMINISTRATOR::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::WEBUSERNAME={onu.Login},WEBPASSWORD={onu.Senha};");
                            }
                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Config Web Administrador \n Status.: {RetornoErroTratado(m)}";
                            }

                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Config Web Administrador \n Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Config Web Administrador \n Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"Config Web Administrador \n {ex.Message}";
            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ListarUsuariosCadastroNaWebInterface")]
        public async Task<GetApiRetorno<List<Object>>> ListarUsuariosCadastroNaWebInterface(Int64 Id_ONU)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {

                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"LST-WEBADMINISTRATOR::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"LST-WEBADMINISTRATOR::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }
                            var resposta = client.ReadAsync().Result;

                            try
                            {
                                var mensagem = TratarRetornoListaUsuariosCadastradoWebInterface(resposta);
                                if (mensagem["STATUS"].Equals("COMPLD"))
                                {
                                    var lista = new List<Object>();
                                    Console.WriteLine(mensagem["BLOCK RECORDS"]);
                                    foreach (var usuarios in mensagem["USUARIOS"] as List<Dictionary<string, string>>)
                                    {
                                        lista.Add(new
                                        {
                                            usuario = usuarios["WEBUSERNAME"],
                                            senha = usuarios["WEBPASSWORD"],
                                            grupo = usuarios["GROUP"],

                                        });

                                    }
                                    response.Objeto = lista;
                                }
                            }
                            catch
                            {
                                var m = TratarMensagemPadrao(resposta);
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }

                            DeslogarServidor(client);

                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }


            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ConfigurarRedeWifi")]
        public async Task<GetApiRetorno<Int64>> ConfigurarRedeWifi(Int64 Id_ONU)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoConfigurarRedeWifi(onu);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"CFG-WIFISERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::WILESS-AREA=1,WILESS-CHANNEL=10,WILESS-STANDARD=802.11n,T-POWER=20,SSID=1,SSID-ENABLE=1,SSID-NAME={onu.Wireless_SSID},SSID-VISIBALE=0,AUTH-MODE=WPA2 WPA,ENCRYPT-TYPE=AES,WEP-KEYINDEX=1,WEPKEY1={onu.Senha_WPA2};");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"CFG-WIFISERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::WILESS-AREA=1,WILESS-CHANNEL=10,WILESS-STANDARD=802.11n,T-POWER=20,SSID=1,SSID-ENABLE=1,SSID-NAME={onu.Wireless_SSID},SSID-VISIBALE=0,AUTH-MODE=WPA2 WPA,ENCRYPT-TYPE=AES,WEP-KEYINDEX=1,WEPKEY1={onu.Senha_WPA2};");
                            }
                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }

                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }


            return response;
        }


        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/EditarConfiguracaoWifi")]
        public async Task<GetApiRetorno<Int64>> EditarConfiguracaoWifi(Int64 Id_ONU)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {
                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoConfigurarRedeWifi(onu);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"MODIFY-WIFISERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::SSID=1,SSID-NAME={onu.Wireless_SSID},PRESHAREDKEY={onu.Senha_WPA2};");
                            }
                            else
                            {
                                await client.WriteLineAsync($"MODIFY-WIFISERVICE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::SSID=1,SSID-NAME={onu.Wireless_SSID},PRESHAREDKEY={onu.Senha_WPA2};");
                            }
                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }


            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/InformacoesBasicasDaONU")]
        public async Task<GetApiRetorno<List<Object>>> InformacoesBasicasDaONU(Int64 Id_ONU)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);


                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"LST-ONUSTATE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"LST-ONUSTATE::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }
                            var resposta = client.ReadAsync().Result;
                            try
                            {
                                var mensagem = TratarRetornoStatusDaONU(resposta);
                                if (mensagem["STATUS"].Equals("COMPLD"))
                                {
                                    var lista = new List<Object>();
                                    foreach (var onus in mensagem["ONUSTATE"] as List<Dictionary<string, string>>)
                                    {
                                        lista.Add(new
                                        {
                                            Onu_ID = onus["ONUID"],
                                            AdminState = onus["AdminState"],
                                            OperState = onus["OperStat"],
                                            AUTH = onus["AUTH"],
                                            AUTHINFO = onus["AUTHINFO"],
                                            ONUIP = onus["ONUIP"],
                                            LASTOFFTIME = onus["LASTOFFTIME"]
                                        });
                                    }

                                    response.Ok = true;
                                    response.Objeto = lista;
                                }

                            }
                            catch (Exception)
                            {
                                var m = TratarMensagemPadrao(resposta);
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }

                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }


            return response;

        }


        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/TaxaDeSinalDaONU")]
        public async Task<GetApiRetorno<List<Object>>> TaxaDeSinalDaONU(Int64 Id_ONU)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"LST-OMDDM::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"LST-OMDDM::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }

                            var resposta = client.ReadAsync().Result;

                            try
                            {
                                var mensagem = TratarRetornoSinalDaONU(resposta);
                                if (mensagem["STATUS"].Equals("COMPLD"))
                                {
                                    var lista = new List<Object>();
                                    foreach (var onus in mensagem["ONUSINAL"] as List<Dictionary<string, string>>)
                                    {
                                        lista.Add(new
                                        {
                                            Onu_ID = onus["ONUID"],
                                            Onu_RxPower = onus["RxPower"],
                                            Onu_RxPowerStatus = onus["RxPowerR"],
                                            Onu_TxPower = onus["TxPower"],
                                            Onu_TxPowerStatus = onus["TxPowerR"],
                                            Onu_CurrTxBias = onus["CurrTxBias"],
                                            Onu_CurrTxBiasStatus = onus["CurrTxBiasR"],
                                            Onu_Temperature = onus["Temperature"],
                                            Onu_TemperatureStatus = onus["TemperatureR"],
                                            Onu_Voltage = onus["Voltage"],
                                            Onu_VoltageStatus = onus["VoltageR"],
                                            Onu_PTxPower = onus["PTxPower"],
                                            Onu_PRxPower = onus["PRxPower"]
                                        });

                                    }
                                    response.Ok = true;
                                    response.Objeto = lista;
                                }
                            }
                            catch (Exception)
                            {
                                var m = TratarMensagemPadrao(resposta);
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }

            return response;
        }


        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/TestePingONU")]
        public async Task<GetApiRetorno<List<String>>> TestePingONU(Int64 Id_ONU)
        {
            GetApiRetorno<List<String>> response = new GetApiRetorno<List<String>>();
            List<String> testePing = new List<String>();
            String resposta = "";
            try
            {
                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);
                TratamentoTestePing(olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (!String.IsNullOrEmpty(olt.Teste_IP1))
                        {
                            if (logado["STATUS"].Equals("COMPLD"))
                            {
                                if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"PING::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::IP={olt.Teste_IP1};");
                                }
                                else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                {
                                    await client.WriteLineAsync($"PING::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::IP={olt.Teste_IP1};");
                                }

                                resposta = client.ReadAsync().Result;

                                testePing = new List<String>();
                                testePing.Add(TratarRetornoPing(resposta, olt.Teste_IP1));
                            }
                            if (!String.IsNullOrEmpty(olt.Teste_IP2))
                            {
                                try
                                {

                                    if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                    {
                                        await client.WriteLineAsync($"PING::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::IP={olt.Teste_IP2};");
                                    }
                                    else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                                    {
                                        await client.WriteLineAsync($"PING::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::IP={olt.Teste_IP2};");
                                    }

                                    resposta = client.ReadAsync().Result;

                                    testePing.Add(TratarRetornoPing(resposta, olt.Teste_IP2));
                                    response.Ok = true;
                                    response.Objeto = testePing;
                                }
                                catch (Exception)
                                {

                                    var m = TratarMensagemPadrao(resposta);
                                    response.Ok = false;
                                    response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                                }
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/RestartONU")]
        public async Task<GetApiRetorno<Int64>> RestartONU(Int64 Id_ONU)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;
                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"RESET-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::RESETTYPE=0;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"RESET-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::RESETTYPE=0;");
                            }
                            var resposta = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (resposta["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Status.: {resposta["STATUS"]} \n Desc.: {resposta["CODIGO"]} - {resposta["DESCRICAO"]}";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(resposta)}";
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message} - {ex.StackTrace}";
            }
            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ResetarConfiguracaoDefault")]
        public async Task<GetApiRetorno<Int64>> ResetarConfiguracaoDefault(Int64 Id_ONU)
        {
            GetApiRetorno<Int64> response = new GetApiRetorno<Int64>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"RESTORE-DEFAULTCFG::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"RESTORE-DEFAULTCFG::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }
                            var m = TratarMensagemPadrao(client.ReadAsync().Result);
                            if (m["STATUS"].Equals("COMPLD"))
                            {
                                response.Ok = true;
                                response.Mensagem = $"Status.: {m["STATUS"]} \n Desc.: {m["CODIGO"]} - {m["DESCRICAO"]}";
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message} - {ex.StackTrace}";
            }


            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ListaConfiguracaoONU")]
        public async Task<GetApiRetorno<List<Object>>> ListaConfiguracaoONU(Int64 Id_ONU)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"LST-ONUWANSERVICECFG::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"LST-ONUWANSERVICECFG::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }
                            var resposta = client.ReadAsync().Result;
                            try
                            {
                                var mensagem = TratarRetornoListaConfiguracaoDaONU(resposta);
                                if (mensagem["STATUS"].Equals("COMPLD"))
                                {
                                    var lista = new List<Object>();
                                    foreach (var config in mensagem["CONFIGURACAO"] as List<Dictionary<string, object>>)
                                    {
                                        lista.Add(new
                                        {
                                            Nome_Servico = config["SVCNAME"],
                                            Modo_Conexao = config["CONMODE"],
                                            Tipo_Conexao = config["CONNTYPE"],
                                            VLAN_ID = config["VLANID"],
                                            VLAN_COS = config["VLANCOS"],
                                            NAT = config["NATFLAG"],
                                            Tipo_IP = config["IPOBTAINTYPE"],
                                            IP_Estatico = config["STATICIPADDRESS"],
                                            IP_Mascara = config["STATICIPSUBNET"],
                                            IP_Gateway = config["STATICGATEWAY"],
                                            DNS_Master = config["MASTERDNS"],
                                            DNS_Slave = config["SLAVEDNS"],
                                            PPPOE_Proxy = config["PPPOEPROXYFLAG"],
                                            PPPOE_Usuario = config["PPPOEUSERNAME"],
                                            PPPOE_Senha = config["PPPOEPASSWD"],
                                            PPPOE_ServicoName = config["PPPOESVCNAME"],
                                            PPPOE_Mode = config["PPPOEMODE"],
                                            QoS = config["QOSFLAG"],
                                            Porta_ONU = config["BINDPORTNO"]
                                        });

                                    }
                                    response.Ok = true;
                                    response.Objeto = lista;
                                }
                            }
                            catch (Exception)
                            {
                                var m = TratarMensagemPadrao(resposta);
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";

                            }

                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = $"{ex.Message}";
            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/InformacaoDoHardwareOLT")]
        public async Task<GetApiRetorno<List<Object>>> InformacaoDoHardwareOLT(Int64 Id_OLT)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();


            try
            {



                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {
                    OLT olt = await repository.selectFirst(p => p.Id == Id_OLT);

                    TratamentoHardwareOLT(olt);

                    using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                    {
                        if (client.IsConnected)
                        {
                            var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                            if (logado["STATUS"].Equals("COMPLD"))
                            {
                                await client.WriteLineAsync($"LST-DEVINFO::OLTID={olt.IP}:CTAG::;");
                                var resposta = client.ReadAsync().Result;
                                try
                                {
                                    var m = TratarRetornoInformacaoHardware(resposta);
                                    if (m["STATUS"].Equals("COMPLD"))
                                    {
                                        var lista = new List<Object>();
                                        foreach (var hardware in m["HARDWARE"] as List<Dictionary<string, string>>)
                                        {
                                            lista.Add(new
                                            {
                                                OLT_Name = hardware["DEVNAME"],
                                                OLT_IP = hardware["DEVIP"],
                                                OLT_Modelo = hardware["DT"],
                                                OLT_Versao_Software = hardware["DEVER"],
                                                OLT_Memoria = hardware["MEM"],
                                                OLT_CPU = hardware["CPU"],
                                                OLT_Temperatura = hardware["TEMPERATURE"]

                                            });
                                        }

                                        response.Ok = true;
                                        response.Objeto = lista;

                                    }
                                }
                                catch (Exception)
                                {
                                    var m = TratarMensagemPadrao(resposta);
                                    response.Ok = false;
                                    response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                                }
                                DeslogarServidor(client);
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                            }
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = "Erro ao estabelecer conexão.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
            }


            return response;

        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/InformacaoDoHardwareONU")]
        public async Task<GetApiRetorno<List<Object>>> InformacaoDoHardwareONU(Int64 Id_ONU)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();


            try
            {


                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;


                TratamentoCampoPadrao(onu, olt);


                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {
                    if (client.IsConnected)
                    {
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {

                                await client.WriteLineAsync($"LST-DEVINFO::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=MAC,ONUID={onu.Mac}:CTAG::;");
                            }
                            else 
                            {
                                await client.WriteLineAsync($"LST-DEVINFO::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()},ONUIDTYPE=LOID,ONUID={onu.Serial}:CTAG::;");
                            }
                            var resposta = client.ReadAsync().Result;
                            try
                            {
                                var m = TratarRetornoInformacaoHardware(resposta);
                                if (m["STATUS"].Equals("COMPLD"))
                                {
                                    var lista = new List<Object>();
                                    foreach (var hardware in m["HARDWARE"] as List<Dictionary<string, string>>)
                                    {
                                        lista.Add(new
                                        {
                                            ONU_Name = hardware["DEVNAME"],
                                            ONU_IP = hardware["DEVIP"],
                                            ONU_Modelo = hardware["DT"],
                                            ONU_Versao_Software = hardware["DEVER"],
                                            ONU_Memoria = hardware["MEM"],
                                            ONU_CPU = hardware["CPU"],
                                            ONU_Temperatura = hardware["TEMPERATURE"]

                                        });
                                    }

                                    response.Ok = true;
                                    response.Objeto = lista;

                                }
                            }
                            catch (Exception)
                            {
                                var m = TratarMensagemPadrao(resposta);
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                            }
                            DeslogarServidor(client);
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                        }
                    }
                    else
                    {
                        response.Ok = false;
                        response.Mensagem = "Erro ao estabelecer conexão.";
                    }
                }

            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
            }


            return response;

        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/InformacaoStatusDaPon")]
        public async Task<GetApiRetorno<List<Object>>> InformacaoStatusDaPon(Int64 Id_PON)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<PON>(session))
                {

                    PON pon = await repository.selectFirst(p => p.Id == Id_PON);
                    OLT olt = pon.Slot.OLT;

                    TratamentoHardwareOLT(olt);

                    using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                    {
                        if (client.IsConnected)
                        {
                            var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                            if (logado["STATUS"].Equals("COMPLD"))
                            {
                                await client.WriteLineAsync($"LST-PONINFO::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()}:CTAG::;");
                                var resposta = client.ReadAsync().Result;
                                try
                                {
                                    var m = TratarRetornoInformacaoPon(resposta);
                                    if (m["STATUS"].Equals("COMPLD"))
                                    {
                                        var lista = new List<Object>();
                                        foreach (var hardware in m["HARDWARE-PON"] as List<Dictionary<string, string>>)
                                        {
                                            lista.Add(new
                                            {
                                                AdminState = hardware["AdminState"],
                                                OperState = hardware["OperState"],
                                            });
                                        }

                                        response.Ok = true;
                                        response.Objeto = lista;

                                    }
                                }
                                catch (Exception)
                                {
                                    var m = TratarMensagemPadrao(resposta);
                                    response.Ok = false;
                                    response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                                }
                                DeslogarServidor(client);
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                            }
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = "Erro ao estabelecer conexão.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/ListaONUsConectadas")]
        public async Task<GetApiRetorno<List<Object>>> ListaONUsConectadas(Int64 Id_OLT)
        {
            GetApiRetorno<List<Object>> response = new GetApiRetorno<List<Object>>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {

                    OLT olt = await repository.selectFirst(p => p.Id == Id_OLT);

                    TratamentoHardwareOLT(olt);

                    using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                    {
                        if (client.IsConnected)
                        {
                            var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                            if (logado["STATUS"].Equals("COMPLD"))
                            {
                                await client.WriteLineAsync($"LST-ONU::OLTID={olt.IP}:CTAG::;");
                                var resposta = client.ReadAsync().Result;
                                try
                                {
                                    var m = TratarRetornoInformacaoHardwareONU(resposta);
                                    if (m["STATUS"].Equals("COMPLD"))
                                    {
                                        var lista = new List<Object>();
                                        foreach (var onu in m["ONUs"] as List<Dictionary<string, string>>)
                                        {
                                            lista.Add(new
                                            {
                                                OLT_ID = onu["OLTID"],
                                                PON_ID = onu["PONID"],
                                                ONU_NO = onu["ONUNO"],
                                                ONU_NOME = onu["NAME"],
                                                ONU_DESC = onu["DESC"],
                                                ONU_ONUTYPE = onu["ONUTYPE"],
                                                ONU_IP = onu["IP"],
                                                ONU_AUTH = onu["AUTH"],
                                                ONU_MAC = onu["MAC"],
                                                ONU_LOID = onu["LOID"],
                                                ONU_PWD = onu["PWD"],
                                                ONU_VersaoSoftware = onu["SWVER"]

                                            });
                                        }

                                        response.Ok = true;
                                        response.Objeto = lista;

                                    }
                                }
                                catch (Exception)
                                {
                                    var m = TratarMensagemPadrao(resposta);
                                    response.Ok = false;
                                    response.Mensagem = $"Status.: {RetornoErroTratado(m)}";
                                }
                                DeslogarServidor(client);
                            }
                            else
                            {
                                response.Ok = false;
                                response.Mensagem = $"Status.: {RetornoErroTratado(logado)}";
                            }
                        }
                        else
                        {
                            response.Ok = false;
                            response.Mensagem = "Erro ao estabelecer conexão.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
            }


            return response;
        }

        /*
         * A exclusão de uma ONU excluirá simultaneamente os serviços que a ONU usa.
         * Para reativar os serviços na ONU, adicione e configure o 
         * serviços novamente.
         * */
        [HttpGet]
        [Route("api/IntegracaoFiberHomeApi/DeletarConfiguracaoDaONU")]
        public async Task<Response> DeletarConfiguracaoDaONU(Int64 Id_ONU)
        {

            Response response = new Response();
            try
            {

                ONU onu = await GetONU(Id_ONU);
                OLT olt = onu.PON.Slot.OLT;
                PON pon = onu.PON;

                TratamentoCampoPadrao(onu, olt);

                using (Client client = new Client(olt.IP, olt.PortaSSH.GetValueOrDefault(22), new CancellationToken()))
                {

                    if (client.IsConnected)
                    {
                        Console.WriteLine("Cliente Conectado");
                        var logado = LoginFiberhome(client, olt.Login, olt.Senha);
                        if (logado["STATUS"].Equals("COMPLD"))
                        {
                            // -=-=-=-=-=-=-= Comando para Deletar ONUs =-=-=-=-=-=-
                            /*
                             * ONUIDTYPE TEM QUE SER IGUAL AO QUE FOI ADICIONADO A ONU O TIPO DE AUTENTICAÇÃO
                             * */
                            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
                            {
                                await client.WriteLineAsync($"DEL-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()}:CTAG::ONUIDTYPE=MAC,ONUID={onu.Mac};");
                            }
                            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
                            {
                                await client.WriteLineAsync($"DEL-ONU::OLTID={olt.IP},PONID={pon.GetPonIDFiberhome()}:CTAG::ONUIDTYPE=LOID,ONUID={onu.Serial};");
                            }
                            var resposta = client.ReadAsync().Result;
                            var mensagem = TratarMensagemPadrao(resposta);

                            if (mensagem["STATUS"].Equals("COMPLD"))
                            {
                                response.result = true;
                                response.message = $"Status.: {mensagem["STATUS"]} \n Desc.: {mensagem["CODIGO"]} - {mensagem["DESCRICAO"]} \n";
                            }
                            else
                            {
                                response.result = false;
                                response.message = $"Status.: {RetornoErroTratado(mensagem)}";
                            }

                        }
                        else
                        {
                            response.result = false;
                            response.message = $"Status.: {RetornoErroTratado(logado)}";

                        }
                    }
                    else
                    {
                        response.result = false;
                        response.message = "Erro ao estabelecer conexão.";
                    }

                }
            }
            catch (Exception ex)
            {
                response.result = false;
                response.exception = $" {ex.Message}";
            }
            return response;
        }

        #region metodos privados

        private Dictionary<string, object> TratarRetornoInformacaoHardwareONU(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var onu = new List<Dictionary<string, object>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var dispositivo = new Dictionary<string, object>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                dispositivo.Add("OLTID", lista[i].Split('\t')[0]);
                dispositivo.Add("PONID", lista[i].Split('\t')[1]);
                dispositivo.Add("ONUNO", lista[i].Split('\t')[2]);
                dispositivo.Add("NAME", lista[i].Split('\t')[3]);
                dispositivo.Add("DESC", lista[i].Split('\t')[4]);
                dispositivo.Add("ONUTYPE", lista[i].Split('\t')[5]);
                dispositivo.Add("IP", lista[i].Split('\t')[6]);
                dispositivo.Add("AUTH", lista[i].Split('\t')[7]);
                dispositivo.Add("MAC", lista[i].Split('\t')[8]);
                dispositivo.Add("LOID", lista[i].Split('\t')[9]);
                dispositivo.Add("PWD", lista[i].Split('\t')[10]);
                dispositivo.Add("SWVER", lista[i].Split('\t')[11]);


                onu.Add(dispositivo);
            }

            mensagem.Add("ONUs", onu);
            return mensagem;
        }
       
        private Dictionary<string, object> TratarRetornoInformacaoPon(string resposta)
        {

            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var hardware = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var dispositivo = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                dispositivo.Add("AdminState", lista[i].Split('\t')[0]);
                dispositivo.Add("OperState", lista[i].Split('\t')[1]);



                hardware.Add(dispositivo);
            }

            mensagem.Add("HARDWARE-PON", hardware);

            return mensagem;
        }

        private Dictionary<string, object> TratarRetornoInformacaoHardware(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var hardware = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var dispositivo = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                dispositivo.Add("DEVNAME", lista[i].Split('\t')[0]);
                dispositivo.Add("DEVIP", lista[i].Split('\t')[1]);
                dispositivo.Add("DT", lista[i].Split('\t')[2]);
                dispositivo.Add("DEVER", lista[i].Split('\t')[3]);
                dispositivo.Add("MEM", lista[i].Split('\t')[4]);
                dispositivo.Add("CPU", lista[i].Split('\t')[5]);
                dispositivo.Add("TEMPERATURE", lista[i].Split('\t')[6]);


                hardware.Add(dispositivo);
            }

            mensagem.Add("HARDWARE", hardware);
            return mensagem;
        }

        private Dictionary<string, object> TratarRetornoListaConfiguracaoDaONU(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var configuracao = new List<Dictionary<string, object>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var config = new Dictionary<string, object>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }
                config.Add("SVCNAME", lista[i].Split('\t')[0]);
                config.Add("CONMODE", modoConexao(lista[i].Split('\t')[1]));
                config.Add("CONNTYPE", tipoConexaoWan(lista[i].Split('\t')[2]));
                config.Add("VLANID", lista[i].Split('\t')[3]);
                config.Add("VLANCOS", lista[i].Split('\t')[4]);
                config.Add("NATFLAG", natAtivo(lista[i].Split('\t')[5]));
                config.Add("IPOBTAINTYPE", modoConexaoWan(lista[i].Split('\t')[6]));
                config.Add("STATICIPADDRESS", lista[i].Split('\t')[7]);
                config.Add("STATICIPSUBNET", lista[i].Split('\t')[8]);
                config.Add("STATICGATEWAY", lista[i].Split('\t')[9]);
                config.Add("MASTERDNS", lista[i].Split('\t')[10]);
                config.Add("SLAVEDNS", lista[i].Split('\t')[11]);
                config.Add("PPPOEPROXYFLAG", modoProxyFlag(lista[i].Split('\t')[12]));
                config.Add("PPPOEUSERNAME", lista[i].Split('\t')[13]);
                config.Add("PPPOEPASSWD", lista[i].Split('\t')[14]);
                config.Add("PPPOESVCNAME", lista[i].Split('\t')[15]);
                config.Add("PPPOEMODE", modoPPPoeMode(lista[i].Split('\t')[16]));
                config.Add("QOSFLAG", modoQos(lista[i].Split('\t')[17]));
                config.Add("BINDPORTNO", lista[i].Split('\t')[18]);



                configuracao.Add(config);
            }
            mensagem.Add("CONFIGURACAO", configuracao);
            return mensagem;
        }

        private String TratarRetornoPing(string resposta, string IP)
        {
            StringBuilder statusPing = new StringBuilder();
            var lista = resposta.Split('\r', '\n');

            statusPing.Append(lista[15] += $"{IP}\n");


            for (int i = 16; i < lista.Length; i++)
            {
                var usuario = new Dictionary<string, string>();
                if (lista[i].Contains("----"))
                {
                    statusPing.Append(lista[i] + "\r\n");
                    continue;
                }
                if (lista[i].Contains(";"))
                {

                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                statusPing.Append(lista[i]);

            }

            return statusPing.ToString();
        }

        private Dictionary<string, object> TratarRetornoSinalDaONU(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var listaUsuarios = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var usuario = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                usuario.Add("ONUID", lista[i].Split('\t')[0]);
                usuario.Add("RxPower", lista[i].Split('\t')[1]);
                usuario.Add("RxPowerR", lista[i].Split('\t')[2]);
                usuario.Add("TxPower", lista[i].Split('\t')[3]);
                usuario.Add("TxPowerR", lista[i].Split('\t')[4]);
                usuario.Add("CurrTxBias", lista[i].Split('\t')[5]);
                usuario.Add("CurrTxBiasR", lista[i].Split('\t')[6]);
                usuario.Add("Temperature", lista[i].Split('\t')[7]);
                usuario.Add("TemperatureR", lista[i].Split('\t')[8]);
                usuario.Add("Voltage", lista[i].Split('\t')[9]);
                usuario.Add("VoltageR", lista[i].Split('\t')[10]);
                usuario.Add("PTxPower", lista[i].Split('\t')[11]);
                usuario.Add("PRxPower", lista[i].Split('\t')[12]);


                listaUsuarios.Add(usuario);

            }
            mensagem.Add("ONUSINAL", listaUsuarios);
            return mensagem;


        }

        private Dictionary<string, object> TratarRetornoStatusDaONU(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var listaUsuarios = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var usuario = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }
                usuario.Add("ONUID", lista[i].Split('\t')[0]);
                usuario.Add("AdminState", lista[i].Split('\t')[1]);
                usuario.Add("OperStat", lista[i].Split('\t')[2]);
                usuario.Add("AUTH", lista[i].Split('\t')[3]);
                usuario.Add("AUTHINFO", lista[i].Split('\t')[4]);
                usuario.Add("ONUIP", lista[i].Split('\t')[5]);
                usuario.Add("LASTOFFTIME", lista[i].Split('\t')[6]);



                listaUsuarios.Add(usuario);

            }
            mensagem.Add("ONUSTATE", listaUsuarios);
            return mensagem;
        }

        private Dictionary<string, object> TratarRetornoListaONUsPendentesRegistro(string resposta)
        {
            var lista = resposta.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var listaOnus = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var onu = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }

                onu.Add("SLOTNO", lista[i].Split('\t')[0]);
                onu.Add("PONNO", lista[i].Split('\t')[1]);
                onu.Add("MAC", lista[i].Split('\t')[2]);
                onu.Add("LOID", lista[i].Split('\t')[3]);
                onu.Add("PWD", lista[i].Split('\t')[4]);
                onu.Add("ERROR", lista[i].Split('\t')[5]);
                onu.Add("AUTHTIME", lista[i].Split('\t')[6]);
                onu.Add("DT", lista[i].Split('\t')[7]);

                listaOnus.Add(onu);
                Console.WriteLine(lista[i]);
            }

            mensagem.Add("ONUs", listaOnus);

            return mensagem;
        }

        private void DeslogarServidor(Client client)
        {
            try
            {
                client.TerminatedReadAsync("LOGOUT:::CTAG::;");

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
        }

        private Dictionary<string, string> LoginFiberhome(Client client, string login, string senha)
        {
            //client.WriteLineAsync(";\r");
            client.WriteLineAsync($"LOGIN:::CTAG::UN={login},PWD={senha};");
            String resultado = client.ReadAsync().Result;
            return TratarMensagemPadrao(resultado);

        }


        private Dictionary<string, string> TratarMensagemPadrao(string resultado)
        {
            var lista = resultado.Split('\r', '\n');
            var mensagem = new Dictionary<string, string>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ')[3]);
            mensagem.Add("CODIGO", lista[7].Trim().Split(' ')[0].Split('=')[1]);
            mensagem.Add("DESCRICAO", lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Contains("  ") ? lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Substring(0, lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().IndexOf("  ")).Split('=')[1] : lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Split('=')[1]);
            mensagem.Add("ARGUMENTO", lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Contains("  ") ? lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Replace(lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().Substring(0, lista[7].Trim().Replace(lista[7].Trim().Split(' ', ' ')[0], "").Trim().IndexOf("  ")), "").Trim() : " ");


            return mensagem;
        }

        private void TratarClasseIP(String ip, String ipInicio, String ipFinal)
        {
            if (!ipInicio.Substring(0, ipInicio.LastIndexOf('.')).Equals(ipFinal.Substring(0, ipFinal.LastIndexOf('.'))))
            {
                throw new ArgumentException("Range de IPs com classes diferente. ");
            }

            if (!ipInicio.Substring(0, ipInicio.IndexOf('.')).Equals(ip.Substring(0, ip.IndexOf('.'))))
            {
                throw new ArgumentException("IP fora da Classe fornecida anteriormente pelo Range de IPs ");
            }
        }


        private Dictionary<string, object> TratarRetornoListaUsuariosCadastradoWebInterface(string resultado)
        {
            var lista = resultado.Split('\r', '\n');
            var mensagem = new Dictionary<string, object>();
            mensagem.Add("LOG", lista[3].Trim());
            mensagem.Add("STATUS", lista[5].Split(' ', ' ').GetValue(lista[5].Split(' ', ' ').Length - 1));
            mensagem.Add("TOTAL BLOCKS", lista[7].Trim().Split('=')[1]);
            mensagem.Add("BLOCK NUMBER", lista[9].Trim().Split('=')[1]);
            mensagem.Add("BLOCK RECORDS", lista[11].Trim().Split('=')[1]);

            var listaUsuarios = new List<Dictionary<string, string>>();

            for (int i = 20; i < lista.Length; i++)
            {
                var usuario = new Dictionary<string, string>();
                if (lista[i].Contains("-----"))
                {
                    break;
                }

                if (String.IsNullOrEmpty(lista[i]))
                {
                    continue;
                }
                usuario.Add("WEBUSERNAME", lista[i].Split('\t')[0]);
                usuario.Add("WEBPASSWORD", lista[i].Split('\t')[1]);
                usuario.Add("GROUP", lista[i].Split('\t')[2]);




                listaUsuarios.Add(usuario);

            }
            mensagem.Add("USUARIOS", listaUsuarios);
            return mensagem;
        }

        
        private async Task<ONU> GetONU(Int64 Id_ONU)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                ONU onuAliases = null;
                PON pon = null;
                Slot slot = null;
                OLT olt = null;
                Filial filial = null;
                Empresa empresa = null;

                IList<ONU> onus = (await session.QueryOver(() => onuAliases)
                                               .JoinAlias(() => onuAliases.PON, () => pon)
                                               .JoinAlias(() => pon.Slot, () => slot)
                                               .JoinAlias(() => slot.OLT, () => olt)
                                               .JoinAlias(() => olt.Filial, () => filial)
                                               .JoinAlias(() => filial.Empresa, () => empresa)
                                               .Where(p => p.Id == Id_ONU).Take(1).ListAsync());

                if (onus.Count == 0)
                {
                    throw new Exception($"ONU não encontrada");
                }

                if (onus[0].PON == null)
                {
                    throw new Exception($"PON não cadastrada na ONU {onus[0].Descricao}");
                }

                return onus[0];
            }
        }


        private static object modoQos(string flag)
        {
            Dictionary<String, String> QoS = new Dictionary<String, String>();
            QoS.Add("1", "ENABLE");
            QoS.Add("2", "DISABLE");
            return QoS[flag];
        }

        private static object modoPPPoeMode(string ID_Conexao)
        {
            Dictionary<String, String> conexao = new Dictionary<String, String>();
            conexao.Add("1", "AUTO CONNECT");
            conexao.Add("2", "CONNECT WHEN TRAFFIC IS DELETED");
            return conexao[ID_Conexao];
        }

        private static object modoProxyFlag(string flag)
        {
            Dictionary<String, String> proxy = new Dictionary<String, String>();
            proxy.Add("1", "ENABLE");
            proxy.Add("2", "DISABLE");
            return proxy[flag];

        }

        private static object modoConexaoWan(string ID_Conexao)
        {
            Dictionary<String, String> conexao = new Dictionary<String, String>();
            conexao.Add("1", "DHCP");
            conexao.Add("2", "STATIC");
            conexao.Add("3", "PPPOE");

            return conexao[ID_Conexao];
        }

        private static object natAtivo(string ID_NAT)
        {
            Dictionary<String, String> conexao = new Dictionary<String, String>();
            conexao.Add("1", "ENABLE");
            conexao.Add("2", "DISABLE");
            return conexao[ID_NAT];
        }

        private static object tipoConexaoWan(string ID_ConexaoWan)
        {
            Dictionary<String, String> conexao = new Dictionary<String, String>();
            conexao.Add("1", "BRIDGE");
            conexao.Add("2", "ROUTE");
            return conexao[ID_ConexaoWan];
        }

        private static object modoConexao(string ID_Conexao)
        {
            Dictionary<String, String> conexao = new Dictionary<String, String>();
            conexao.Add("1", "TR069");
            conexao.Add("2", "INTERNET");
            conexao.Add("3", "TR069 INTERNET");
            conexao.Add("4", "OTHER");


            return conexao[ID_Conexao];

        }

        private String RetornoErroTratado(Dictionary<string, string> mensagem)
        {
            String retorno;
            if (mensagem["CODIGO"].ToString().Equals("IRNE"))
            {
                retorno = "O recurso não existe. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("IANE"))
            {
                retorno = "O alarme não existe. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("IMP"))
            {
                retorno = "Parâmetro ausente. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("IIPF"))
            {
                retorno = "Formato de parâmetro inválido. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("IIPE"))
            {
                retorno = "Erro de parâmetro de entrada. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("DDNS"))
            {
                retorno = "O dispositivo pode não suportar esta operação";
            }
            else if (mensagem["CODIGO"].ToString().Equals("DDOF"))
            {
                retorno = "Operação do dispositivo falhou. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("DDB"))
            {
                retorno = "Dispositivo está ocupado. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("SENS"))
            {
                retorno = "O EMS pode não suportar esta operação. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("SEOF"))
            {
                retorno = "Falha na operação do EMS. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("EEEH"))
            {
                retorno = "A exceção do EMS. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("TUB"))
            {
                retorno = "Usuário está ocupado. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("TUT"))
            {
                retorno = "Usuário está testando. ";
            }
            else if (mensagem["CODIGO"].ToString().Equals("TTMB"))
            {
                retorno = "O Módulo de teste está ocupado. ";
            }
            else
            {
                retorno = $"{mensagem["STATUS"]} \n Desc.: {mensagem["CODIGO"]} - {mensagem["DESCRICAO"]} \n Motivo.: {mensagem["ARGUMENTO"]}";
            }

            return retorno;

        }

        #endregion

        #region Metodos de Verificacao de Campos

        private void TratamentoCampoConfigurarRede(ONU onu, OLT olt, PON pon)
        {

            if (pon.Id_VLAN_Dados == 0)
            {
                throw new Exception("VLAN Dados não informado para a porta PON");
            }


            if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.PPPoE)
            {
                if (String.IsNullOrEmpty(onu.Cliente_PlanoProduto.Login_Radius))
                {
                    throw new Exception("Login PPPoE não informado. ");
                }

                if (String.IsNullOrEmpty(onu.Cliente_PlanoProduto.Senha_Radius))
                {
                    throw new Exception("Senha PPPoE não informado. ");
                }
            }
            if (onu.Tipo_Conexao == Aniel.Connect.Model.enumerado.Tipo_Conexao_ONU.IP_ESTATICO)
            {
                if (String.IsNullOrEmpty(onu.Estatico_Nro_Ip))
                {
                    throw new Exception("IP Estatico não configurado.");
                }
                if (String.IsNullOrEmpty(onu.Estatico_Ip_Mask))
                {
                    throw new Exception("Mascara de IP estatico não configurado.");
                }
                if (String.IsNullOrEmpty(onu.Estatico_Gatway))
                {
                    throw new Exception("Gateway estatico não informado.");
                }

                if (String.IsNullOrEmpty(olt.DHCP_DNS_Primario))
                {
                    throw new Exception("DNS Primario não informado. ");
                }
                if (String.IsNullOrEmpty(olt.DHCP_DNS_Secundario))
                {
                    throw new Exception("DNS Secundario não informado. ");
                }

            }

        }

        private void TratamentoConfigurarRedeInternar(ONU onu)
        {
            if (String.IsNullOrEmpty(onu.Nro_Ip))
            {
                throw new Exception("IP Interno não informado.");
            }
            if (String.IsNullOrEmpty(onu.Ip_Inicio))
            {
                throw new Exception("Inicio da Faixa de IP não informado.");
            }
            if (String.IsNullOrEmpty(onu.Ip_Final))
            {
                throw new Exception("Final da Faixa de IP não informado.");
            }
            if (String.IsNullOrEmpty(onu.Ip_Mask))
            {
                throw new Exception("Mascara de IP interno não informado.");
            }
        }

        private void TratamentoAcessoAdministrador(ONU onu)
        {
            if (String.IsNullOrEmpty(onu.Login))
            {
                throw new Exception("Login de Acesso não informado.");
            }
            if (String.IsNullOrEmpty(onu.Senha))
            {
                throw new Exception("Senha de Acesso não informado.");
            }
        }

        private void TratamentoConfigurarRedeWifi(ONU onu)
        {
            if (String.IsNullOrEmpty(onu.Wireless_SSID))
            {
                throw new Exception("Wireless SSID não informado.");
            }

            if (String.IsNullOrEmpty(onu.Senha_WPA2))
            {
                throw new Exception("Senha Wireless não informado.");
            }
        }

        private void TratamentoRegistraONU(ONU onu)
        {
            if (String.IsNullOrEmpty(onu.Descricao))
            {
                throw new Exception("Tipo da ONU não informado. ");
            }
        }

        private void TratamentoCampoLimpaConfiguracao(ONU onu, PON pon)
        {
            if (pon.Id_VLAN_Dados == 0)
            {

                throw new Exception("VLAN Dados não informado para a porta PON.");

            }
            if (onu.PortaVLan == 0)
            {
                throw new Exception("Porta VLAN da ONU não foi informado.");
            }

        }

        private void TratamentoCampoPadrao(ONU onu, OLT olt)
        {

            if (String.IsNullOrEmpty(olt.IP))
            {
                throw new Exception("IP da OLT não informado. ");
            }
            if (String.IsNullOrEmpty(olt.IP))
            {
                throw new Exception("IP do Servidor de Gerencia não informado.");
            }
            if (String.IsNullOrEmpty(olt.Login))
            {
                throw new Exception("Login de Acesso da OLT não informado.");
            }
            if (String.IsNullOrEmpty(olt.Senha))
            {
                throw new Exception("Senha de Acesso da OLT não informado.");
            }


            if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.MAC)
            {
                if (String.IsNullOrEmpty(onu.Mac))
                {
                    throw new Exception("MAC não informado. ");
                }
            }
            else if (onu.Tipo_Identificacao == Sinapse.Model.enumerados.ONU_Tipo_Identificacao.LOID)
            {
                throw new Exception("Serial não informado. ");
            }

        }

        private void TratamentoTestePing(OLT olt)
        {
            if (String.IsNullOrEmpty(olt.Teste_IP1) && String.IsNullOrEmpty(olt.Teste_IP2))
            {
                throw new Exception("Não foi informado nenhum IP para Teste de ping");
            }
        }

        private void TratamentoHardwareOLT(OLT olt)
        {
            if (String.IsNullOrEmpty(olt.Login))
            {
                throw new Exception("Login de Acesso da OLT não informado.");
            }
            if (String.IsNullOrEmpty(olt.Senha))
            {
                throw new Exception("Senha de Acesso da OLT não informado.");
            }
            if (String.IsNullOrEmpty(olt.IP))
            {
                throw new Exception("IP da OLT não informado. ");
            }
        }

        #endregion
    }
}