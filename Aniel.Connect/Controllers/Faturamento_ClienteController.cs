using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using System.Text.RegularExpressions;
using Resources;
using Aniel.Connect.Core;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Faturamento_ClienteController : BaseController
    {

        public Faturamento_ClienteController() : base("Faturamento_Cliente",
                                                  CONSTANTES.URLPortal,
                                                  String.Empty,
                                                  "api/Faturamento_ClienteApi/FindById?id={0}",
                                                  "api/Faturamento_ClienteApi/Gravar",
                                                  "api/Faturamento_ClienteApi/Excluir",
                                                  String.Empty)
        {

        }

        public ActionResult Historico_Fatura(Int32 Id_Cliente)
        {
            var retorno =  new ClienteApiController().FindById(Id_Cliente).Result;

            var modelo = new HistoricoFaturaCadastroModel() { Id_Cliente = retorno.Objeto.Id };
                            
            //if (!retorno.Ok)
            //{
            //    AddModelError(retorno.Mensagem, retorno.InnerException);
            //}
            
            
            return View(modelo);
        }

        public async Task<ActionResult> VisualizarLog(Int64? IdFaturamentoCliente, Int64? Id_Cliente, Int64 Id_Protocolo)
        {
            String urlContasReceber = Url.Action("Cadastro", "Conta_Receber");
            List<Faturamento_Log> retorno = await new Faturamento_ClienteApiController().GetLog(IdFaturamentoCliente, Id_Cliente, Id_Protocolo, urlContasReceber);
            return View(retorno);
        }

        public async Task<ActionResult> ModalDetalhesFaturamento(Int64 Id_Faturamento_Cliente)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Faturamento_Cliente>(session))
            using (var repoFatCliPlano = new Conexao<Faturamento_Cliente_Plano>(session))
            using (var repoCredito = new Conexao<Credito>(session))

            {
                Faturamento_Cliente fatCli = await repository.selectFirst(p => p.Id == Id_Faturamento_Cliente);

                var retorno = fatCli.Faturamento_Forma_Pagto.Faturamento_Protocolo.Tem_Valor_Avulso
                    ? await new Faturamento_ClienteApiController().ModalDetalhesFaturamentoAvulso(Id_Faturamento_Cliente)
                    : await new Faturamento_ClienteApiController().ModalDetalhesFaturamento(Id_Faturamento_Cliente);

                //double desconto_antecipacao = (double)fatCli.Descontos_Total_Antecipacao;
                if (fatCli.Valor_Negociacao.HasValue)
                {
                    double valor_negociacao = (double)fatCli.Valor_Negociacao;
                    ViewBag.ValorNegociacao = String.Format("{0:N2}", valor_negociacao);
                }

                if (fatCli.Valor_Total.HasValue && fatCli.Valor_Final.HasValue)
                {
                    double valor_final = (double)fatCli.Valor_Total - (double)fatCli.Valor_Final;
                    ViewBag.Descontos = String.Format("{0:N2}", valor_final);
                }

                if (fatCli.Multa_Carencia.HasValue)
                {
                    double valorMulta = (double)fatCli.Multa_Carencia;
                    ViewBag.ValorMulta = String.Format("{0:N2}", valorMulta);
                }

                ViewBag.InstalacaoTotal = fatCli.Valor_Instalacao ?? 0.00;

                if (ViewBag.InstalacaoTotal != null)
                    ViewBag.InstalacaoTotal = String.Format("{0:N2}", ViewBag.InstalacaoTotal);

                return View(retorno);
            }
        }
    }
}