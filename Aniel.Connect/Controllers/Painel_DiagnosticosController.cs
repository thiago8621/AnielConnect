using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Painel_DiagnosticosController: BaseController
    {
        public Painel_DiagnosticosController() : base("Painel_Diagnosticos",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Configuracoes_Geraisapi/FindById?id={0}",
                                                   "api/Configuracoes_Geraisapi/Gravar",
                                                   "api/Configuracoes_Geraisapi/Excluir",
                                                   String.Empty)
        {

        }

        public ActionResult Index()
        {           
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult PainelHistoricoAutenticacoes()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public async Task<ActionResult> RelatorioConexao(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);

                var retorno = new Sinapse.Integracao.Ativacao.Models.RelatorioConexaoModel
                {
                    Id_Login_Radius = loginRadius.Id,
                    Login_Radius = loginRadius.Login,
                    Data_Inicial = @DateTime.Now.ToString("dd/MM/yyyy"),
                    Data_Fim = @DateTime.Now.ToString("dd/MM/yyyy")
                };

                return View(retorno);
            }
            /*var retorno = await new Relatorio_ConexaoApiController().GetRelatorioConexaoByLogin(Id_Login_Radius);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);*/
        }

        public async Task<ActionResult> GraficoConsumoConexao(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())            
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);                
                var request = new GraficoConsumoPeriodo_Request
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Login_Radius = loginRadius.Login,
                    Periodo_Consumo = Model.enumerado.HistoricoConsumoPeriodoEnum.Dia,
                    //Data_Inicial = DateTime.Now.Date,
                    //Data_Fim = new DateTime(2022, 07, 27, 16, 30, 0),
                    //Data_Fim_String = new DateTime(2022, 07, 27, 16, 30, 0).ToString("dd/MM/yyyy HH:mm:ss")
                    Data_Fim = DateTime.Now,
                    Data_Fim_String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return View(request);
            }
         
            /*var retorno = await new Relatorio_ConexaoApiController().GetGraficoConsumoPeriodo(request);

            if(!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }*/            
        }

        public async Task<ActionResult> GraficoConexaoTempoReal(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                var planoProdutoRequest = new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest();
                planoProdutoRequest.IdClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id;

                var IpLogin = await new IntegracaoRadiusApiController().GetIPLoginSessao(planoProdutoRequest);
                var request = new ConexaoTempoReal_Request
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Login_Radius = loginRadius?.Login,
                    Desc_Cliente = loginRadius?.Cliente?.NomeFantasia,
                    Ip_Login_Radius = IpLogin,
                    Nas = loginRadius?.Concentradora?.Nome,
                    Data_Inicial = DateTime.Now,
                    Data_Inicial_String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Unidade_Medida_Velocidade = Model.enumerado.Medida_Velocidade_ConexaoEnum.Kbs
                };

                return View(request);
            }
        }

        public async Task<ActionResult> GraficoTempoOnlineOffline(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                var planoProdutoRequest = new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest();
                planoProdutoRequest.IdClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id;

                var IpLogin = await new IntegracaoRadiusApiController().GetIPLoginSessao(planoProdutoRequest);
                var request = new GraficoTempoOnlineOffline_Request
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Login_Radius = loginRadius?.Login,
                    Desc_Cliente = loginRadius?.Cliente?.NomeFantasia,
                    Ip_Login_Radius = IpLogin,
                    Periodo_Consumo = Model.enumerado.HistoricoConsumoPeriodoEnum.Dia,                    
                    Data_Fim = DateTime.Now,
                    Data_Fim_String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return View(request);
            }                         
        }

        public async Task<ActionResult> AcessoEquipamentoCliente(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            using (var repoONU = new Conexao<ONU>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                if (loginRadius == null)
                {
                    Id_Login_Radius = 0;
                }
                var onu = await repoONU.selectFirst(p => p.LoginRadius.Id == loginRadius.Id);
                var planoProdutoRequest = new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest();
                planoProdutoRequest.IdClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id;

                var IpLogin = await new IntegracaoRadiusApiController().GetIPLoginSessao(planoProdutoRequest);
                
                if(onu == null)
                {
                    var novoRetorno = new AcessoEquipamentoClienteModel
                    {
                        Id_Login_Radius = Id_Login_Radius,
                        Desc_Login_Radius = loginRadius?.Login ?? "",
                        Desc_Cliente = loginRadius?.Cliente?.NomeFantasia ?? "",
                        Ultimo_Ip = IpLogin ?? "",
                        Framed_Ipv6 = loginRadius?.EnderecoIPV6 ?? "",
                        Delegated_Ipv6 = loginRadius?.EnderecoIPV6 ?? "",
                        Senha_Equipamento = loginRadius?.SenhaRoteador ?? "",
                        Login_Equipamento = loginRadius?.UsuarioRoteador ?? "",
                        Porta_Http = "80",
                        Endereco_Mac = ""
                    };

                    return View(novoRetorno);
                }
                var endereco_MAC = "";
                if(String.IsNullOrEmpty(onu?.Mac))
                {
                    endereco_MAC = loginRadius?.MAC;
                }
                else if(String.IsNullOrEmpty(loginRadius?.MAC))
                {
                    endereco_MAC = onu?.Serial;
                }

                var retorno = new AcessoEquipamentoClienteModel
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Login_Radius = loginRadius?.Login,
                    Desc_Cliente = loginRadius?.Cliente?.NomeFantasia,
                    Ultimo_Ip = IpLogin,
                    Framed_Ipv6 = loginRadius?.EnderecoIPV6,
                    Delegated_Ipv6 = loginRadius?.EnderecoIPV6,
                    Senha_Equipamento = loginRadius?.SenhaRoteador,
                    Login_Equipamento = loginRadius?.UsuarioRoteador,
                    Porta_Http = onu.PortaHttp.HasValue ? Convert.ToString(onu.PortaHttp) : "80",
                    Endereco_Mac = endereco_MAC
                };
              
                return View(retorno);
            }

        }

        public async Task<ActionResult> GraficoPingClienteConcentradora(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                var planoProdutoRequest = new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest();
                planoProdutoRequest.IdClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id;

                var IpLogin = await new IntegracaoRadiusApiController().GetIPLoginSessao(planoProdutoRequest);
                var request = new GraficoPing_Request
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Login_Radius = loginRadius?.Login,
                    Desc_Cliente = loginRadius?.Cliente?.NomeFantasia,
                    Ip_Login_Radius = IpLogin,
                    Ip_Concentradora = loginRadius?.Concentradora?.Ip,
                };

                return View(request);
            }
        }

        public async Task<ActionResult> GraficoPotenciaRxTx(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            using (var repoOnu = new Conexao<ONU>(session))
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                var onu = await repoOnu.selectFirst(p => p.LoginRadius.Id == loginRadius.Id);

                var request = new GraficoPotenciaRxTxModel
                {
                    Id_Login_Radius = Id_Login_Radius,
                    Desc_Onu = onu?.Descricao,
                    MAC = onu?.Mac ?? loginRadius?.MAC,
                    Periodo_Consumo = Model.enumerado.HistoricoConsumoPeriodoEnum.Dia,
                    Data_Fim = DateTime.Now,
                    Data_Fim_String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                return View(request);
            }
        }

        public ActionResult PainelDiagnosticoFrame(Int64 Id_Login_Radius)
        {
            PainelDiagnostico_FrameModel retorno = new PainelDiagnostico_FrameModel();
            retorno.Id_Login_Radius = Id_Login_Radius;

            return View(retorno);
        }

        public async Task<ActionResult> AlteracaoIP(Int64 Id_Login_Radius)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))           
            {
                var loginRadius = await repoLoginRadius.selectFirst(p => p.Id == Id_Login_Radius);
                long? idPoolIp = loginRadius.Pool_Ip_Endereco?.Pool_Ip?.Id;
                long? idIp = loginRadius.Pool_Ip_Endereco?.Id;
                bool? isIpFixo = loginRadius.Pool_Ip_Endereco?.Pool_Ip?.IsPoolIpFixo;
                if (idPoolIp == null)
                {
                    idPoolIp = loginRadius.Cliente_PlanoProduto?.Pool_Ip?.Id;
                }
                if (idIp == null)
                {
                    idIp = 0;
                }
                if(isIpFixo == null)
                {
                    isIpFixo = loginRadius.Cliente_PlanoProduto.Pool_Ip.IsPoolIpFixo;
                }

                var request = new Pool_Ip_Endereco_VincularLoginModels
                {
                    Id_Cliente = loginRadius.Cliente.Id,
                    Id_ClientePlanoProduto = loginRadius.Cliente_PlanoProduto.Id,
                    //Id_Pool_Ip = loginRadius.Pool_Ip_Endereco?.Pool_Ip?.Id ?? loginRadius.Cliente_PlanoProduto?.Pool_Ip?.Id,
                    Id_Pool_Ip = idPoolIp,
                    //Id_Ip = loginRadius.Pool_Ip_Endereco?.Id ?? 0,
                    Id_Ip = idIp,
                    IsPoolIpFixo = isIpFixo
                };

                return View(request);
            }
        }
    }
}