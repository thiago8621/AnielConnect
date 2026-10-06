using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Text;
using Sinapse.Model;
using Sinapse.Integracao.Ativacao.Models;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ConfigurarONUApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/ConfigurarONUapi/FindById")]
        public async Task<Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel> FindById(Int64? idONU)
        {
            Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel retorno = new Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<ONU>(session))
            {
                var objBanco = idONU.HasValue ? await repository.selectFirst(p => p.Id == idONU): null;
                retorno = GetViewObj(objBanco);
            }

            return retorno;
        }

        private Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel GetViewObj(ONU objBanco)
        {
            Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel model = new Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel();

            model.Id = objBanco != null ? objBanco.Id : 0;
            model.OntID = objBanco != null ? objBanco.OntID : (int?)null;
            model.Descricao = objBanco != null ? objBanco.Descricao : "";

            model.Mac = objBanco != null ? objBanco.Mac : "";
            model.Serial = objBanco != null ? objBanco.Serial : "";
            model.MarcaModelo = objBanco?.OLT?.Marca_Modelo ?? objBanco?.PON?.OLT?.Marca_Modelo;

            model.IdOLT = objBanco?.OLT?.Id ?? objBanco?.PON?.OLT?.Id;
            model.IdSlot = objBanco?.Slot?.Id ?? objBanco?.PON?.Slot?.Id;
            model.IdPon = objBanco?.PON?.Id;
            model.IdPerfilAtivacao = objBanco?.PerfilAtivacao?.Id;
            model.IdLogin = objBanco?.LoginRadius?.Id;

            model.IdCliente = objBanco?.Cliente_PlanoProduto?.Cliente?.Id ?? 0;
            model.IdPlano = objBanco?.Cliente_PlanoProduto?.Plano?.Id ?? 0;

            model.GemPort = objBanco?.GemPort;
            model.ServicePort = objBanco?.ServicePort;
            model.SenorId = objBanco?.SenorId;

            model.IpGerencia = objBanco?.IpGerencia;
            model.Login = objBanco?.Login;
            model.Senha = objBanco?.Senha;
            model.PortaTelnet = objBanco?.PortaTelnet;
            model.PortaHttp = objBanco?.PortaHttp;

            model.VLAN_VOIP = objBanco?.VLAN_VOIP ?? objBanco?.PON?.VLAN_VOIP;
            model.VLAN_IPTV = objBanco?.VLAN_IPTV ?? objBanco?.PON?.VLAN_IPTV;
            model.VLAN_DHCP = objBanco?.VLAN_DHCP ?? objBanco?.PON?.VLAN_DHCP;
            model.VLAN_PPPOE = objBanco?.VLAN_PPPOE ?? objBanco?.PON?.VLAN_PPPOE;
            model.VLAN_TR69 = objBanco?.VLAN_TR69 ?? objBanco?.PON?.VLAN_TR69;
            model.VLAN_Uplink = objBanco?.VLAN_Uplink ?? objBanco?.PON?.VLAN_Uplink;
            model.VLAN_Outros = objBanco?.VLAN_Outros ?? objBanco?.PON?.VLAN_Outros;
            model.VLAN_Profile = objBanco?.VLAN_Profile ?? objBanco?.PON?.VLAN_Profile;

            model.StatusOnu = objBanco?.StatusOnu.HasValue == true ? objBanco?.StatusOnu : Sinapse.Model.enumerados.StatusONU.Indisponivel;
            model.UltimaAtualizacaoStatus = objBanco?.UltimaAtualizacaoStatus.HasValue == true ? objBanco.UltimaAtualizacaoStatus.Value.ToString("dd/MM/yyyy HH:mm:ss") : "N/A";

            model.IdCaixaFTTH = objBanco?.LoginRadius?.Id_Caixa_FTTH;
            model.IdPortaFTTH = objBanco?.LoginRadius?.Id_Porta_FTTH;

            if (objBanco != null)
            {
                if (objBanco.ScriptOLT != null)
                    model.ScriptOLT = Encoding.UTF8.GetString(objBanco.ScriptOLT);

                if (objBanco.ScriptONU != null)
                    model.ScriptONU = Encoding.UTF8.GetString(objBanco.ScriptONU);
            }

            return model;
        }

        private async Task BindViewToObj(ISession session, ONU objBanco, Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel view)
        {
            using (var repoPON = new Conexao<PON>(session))
            using (var repoOLT = new Conexao<OLT>(session))
            using (var repoSlot = new Conexao<Slot>(session))
            using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repoPerfilAtivacao = new Conexao<PerfilAtivacao>(session))
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            {

                objBanco.Descricao = view.Descricao;

                //Só preenche os campos abaixo se for registro novo.
                if (objBanco.Id == 0)
                {                    
                    objBanco.OntID = view.OntID;
                    objBanco.Serial = view.Serial;
                    objBanco.Mac = view.Mac;
                    objBanco.OLT = view.IdOLT.HasValue ? await repoOLT.selectFirst(p => p.Id == view.IdOLT.Value) : null;
                    objBanco.Slot = view.IdSlot.HasValue ? await repoSlot.selectFirst(p => p.Id == view.IdSlot.Value) : null;
                    objBanco.PON = view.IdPon.HasValue ? await repoPON.selectFirst(p => p.Id == view.IdPon.Value) : null;
                    objBanco.LoginRadius = view.IdLogin.HasValue ? await repoLoginRadius.selectFirst(p => p.Id == view.IdLogin.Value) : null;
                    objBanco.Cliente_PlanoProduto = view.IdCliente.HasValue && view.IdPlano.HasValue ? await repoClientePlanoProduto.selectFirst(p => p.Cliente.Id == view.IdCliente && p.Plano.Id == view.IdPlano) : null;
                }
                
                objBanco.PerfilAtivacao = view.IdPerfilAtivacao.HasValue ? await repoPerfilAtivacao.selectFirst(p => p.Id == view.IdPerfilAtivacao.Value): null;                              
                objBanco.GemPort = view.GemPort;
                objBanco.ServicePort = view.ServicePort;
                objBanco.SenorId = view.SenorId;

                objBanco.IpGerencia = view.IpGerencia;
                objBanco.Login = view.Login;
                objBanco.Senha = view.Senha;
                objBanco.PortaTelnet = view.PortaTelnet;
                objBanco.PortaHttp = view.PortaHttp;

                objBanco.VLAN_VOIP = view.VLAN_VOIP;
                objBanco.VLAN_IPTV = view.VLAN_IPTV;
                objBanco.VLAN_DHCP = view.VLAN_DHCP;
                objBanco.VLAN_PPPOE = view.VLAN_PPPOE;
                objBanco.VLAN_TR69 = view.VLAN_TR69;
                objBanco.VLAN_Uplink = view.VLAN_Uplink;
                objBanco.VLAN_Outros = view.VLAN_Outros;
                objBanco.VLAN_Profile = view.VLAN_Profile;

                if (!String.IsNullOrEmpty(view.ScriptOLT))
                    objBanco.ScriptOLT = Encoding.UTF8.GetBytes(view.ScriptOLT);
                
                if (!String.IsNullOrEmpty(view.ScriptONU))
                    objBanco.ScriptONU = Encoding.UTF8.GetBytes(view.ScriptONU);
            }
        }

        [HttpPost]
        [Route("api/ConfigurarONUApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                ONU modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<ONU>(session))
                    using (var repoLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
                    {
                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                        }



                        var onuDuplicada = (await repository.select(p => p.OntID == value.Objeto.OntID && p.OLT.Id == value.Objeto.IdOLT && p.Slot.Id == value.Objeto.IdSlot && p.PON.Id == value.Objeto.IdPon)).FirstOrDefault();
                        if (modelo == null && onuDuplicada != null)
                        {
                            throw new Exception("Já existe uma ONU cadastrada com mesmo OntId para esta OLT/Slot/Pon");
                        }

                        var onuMesmoLogin = (await repository.select(p => p.LoginRadius.Id == value.Objeto.IdLogin)).FirstOrDefault();

                        if (modelo == null && onuMesmoLogin != null)
                        {
                            throw new Exception("Já existe uma ONU vinculada a este Login Radius");
                        }

                        if (modelo == null)
                        {
                            modelo = new ONU
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        bool atualizarFTTH = false;
                        if (modelo?.LoginRadius?.Id_Caixa_FTTH != value.Objeto.IdCaixaFTTH || modelo?.LoginRadius?.Id_Porta_FTTH != value.Objeto.IdPortaFTTH)
                        {
                            atualizarFTTH = true;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        #region Caixa e Porta FTTH
                        if (atualizarFTTH && modelo.Cliente_PlanoProduto != null && modelo.LoginRadius != null)
                        {
                            var loginRadius = modelo.LoginRadius;
                            loginRadius.Id_Caixa_FTTH = value.Objeto.IdCaixaFTTH;
                            loginRadius.Id_Porta_FTTH = value.Objeto.IdPortaFTTH;

                            await repoLogin.SaveOrUpdate(loginRadius);

                            var requestPorta = new Elemento_Reserva();
                            requestPorta.Cliente_PlanoProduto = modelo.Cliente_PlanoProduto.Id;
                            requestPorta.Id_Porta = loginRadius.Id_Porta_FTTH;
                            requestPorta.Id_Login = modelo.Id;
                            requestPorta.Login = loginRadius.Login;

                            if (!loginRadius.Id_Caixa_FTTH.HasValue || !loginRadius.Id_Porta_FTTH.HasValue)
                                requestPorta.Reset = true;

                            await new T5.Elemento_Rede_PortaApiController().Atribuir_Porta(requestPorta);
                        }
                        #endregion
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
    }
}