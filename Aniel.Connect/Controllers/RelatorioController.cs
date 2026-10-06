using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class RelatorioController : BaseController
    {
        public RelatorioController() : base("Relatorio",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            String.Empty,
                                            String.Empty,
                                            String.Empty,
                                            String.Empty)
        {

        }

        public ActionResult ProdutividadeTecnico()
        {
            return View();
        }

        public ActionResult ProdutividadeTecnicoExtratoProdutividade()
        {
            return View();
        }

        public ActionResult ProdutividadeTecnicoExtratoPrazo()
        {
            return View();
        }

        public ActionResult ProdutividadeTecnicoExtratoReincidencia()
        {
            return View();
        }

        public ActionResult Producao()
        {
            FillListasView(new CampanhaListaModel());
            return View();
        }

        public ActionResult AreasHabilitadas()
        {
            return View();
        }

        public ActionResult RelatorioVendasMesmoEndereco()
        {
            return View();
        }

        public ActionResult NaturezasHabilitadas()
        {
            return View();
        }

        public ActionResult RelatorioRankingTecnicos()
        {
            List<SelectValue> anos = new List<SelectValue>();
            List<SelectValue> meses = new List<SelectValue>();

            foreach (int ano in Enumerable.Range(1990, DateTime.Now.Year).Where(number => number <= DateTime.Now.Year).ToList())
            {
                anos.Add(new SelectValue { IdString = ano.ToString(), Descricao = ano.ToString() });
            }

            for (long i = 0; i < 12; i++)
            {
                meses.Add(new SelectValue { Id = i, Descricao = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.MonthNames[i] });
            }

            ViewBag.Anos = anos;
            ViewBag.Meses = meses;

            return View();
        }

        public ActionResult RelatorioMediaServicosExecutados()
        {
            return View();
        }

        public ActionResult RelatorioAnatel()
        {
            List<SelectValue> anos = new List<SelectValue>();
            List<SelectValue> meses = new List<SelectValue>();

            foreach (int ano in Enumerable.Range(1990, DateTime.Now.Year).Where(number => number <= DateTime.Now.Year).ToList())
            {
                anos.Add(new SelectValue { IdString = ano.ToString(), Descricao = ano.ToString() });
            }

            for (long i = 0; i < 12; i++)
            {
                meses.Add(new SelectValue { Id = i, Descricao = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.MonthNames[i] });
            }

            ViewBag.Anos = anos;
            ViewBag.Meses = meses;
            return View();
        }
        public ActionResult RelatorioChecklist()
        {
            List<SelectValue> anos = new List<SelectValue>();
            List<SelectValue> meses = new List<SelectValue>();

            foreach (int ano in Enumerable.Range(1990, DateTime.Now.Year).Where(number => number <= DateTime.Now.Year).ToList())
            {
                anos.Add(new SelectValue { IdString = ano.ToString(), Descricao = ano.ToString() });
            }

            for (long i = 0; i < 12; i++)
            {
                meses.Add(new SelectValue { Id = i, Descricao = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.MonthNames[i] });
            }

            ViewBag.Anos = anos;
            ViewBag.Meses = meses;
            return View();
        }

		public ActionResult RelatorioContratoCliente()
        {
            List<SelectValue> anos = new List<SelectValue>();
            List<SelectValue> meses = new List<SelectValue>();

            foreach (int ano in Enumerable.Range(1990, DateTime.Now.Year).Where(number => number <= DateTime.Now.Year).ToList())
            {
                anos.Add(new SelectValue { IdString = ano.ToString(), Descricao = ano.ToString() });
            }

            for (long i = 0; i < 12; i++)
            {
                meses.Add(new SelectValue { Id = i, Descricao = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.MonthNames[i] });
            }

            ViewBag.Anos = anos;
            ViewBag.Meses = meses;
            return View();
        }

        public ActionResult ServicosImprodutivos()
        {
            return View();
        }

        public ActionResult ServicosExecutadosPorTecnico()
        {
            return View();
        }

        public ActionResult RelatorioPlanosClientes()
        {
            return View();
        }

        public ActionResult AcompanhamentoServico()
        {
            return View();
        }
        public ActionResult ProducaoServico_Analitico()
        {
            return View();
        }
        public ActionResult ProducaoServico_Tecnicos_Auxiliares()
        {

            return View();
        }

        public ActionResult ProducaoServico_Sintetico()
        {
            return View();
        }

        public ActionResult Materiais_AplicadosRetirados_Sintetico()
        {
            return View();
        }

        public ActionResult Materiais_AplicadosRetirados_Analitico()
        {
            return View();
        }

        public ActionResult RelatorioCapacidadeAgendamento()
        {
            return View();
        }
        public ActionResult RelatorioCapacidadeAgendamentoExtratoAgendamento()
        {
            return View();
        }
        public ActionResult PerfilAreasAtuacaoNaturezasVinculadas()
        {
            return View();
        }
        public ActionResult AreasAtuacaoNaturezaVinculadaTecnico()
        {
            return View();
        }

        public ActionResult Relatorio_Historico_Chamada()
        {
            return View();
        }
        public ActionResult RelatorioSaldoMateriaisAplicados()
        {
            return View();
        }

        public ActionResult RelatorioSaldoMateriaisAplicadosPOP()
        {
            return View();
        }

        public ActionResult QuilometrosPercorridos()
        {
            return View();
        }

        public ActionResult ProjecaoVendas()
        {
            List<SelectValue> anos = new List<SelectValue>();

            foreach (int ano in Enumerable.Range(DateTime.Now.Year, DateTime.Now.Year + 20).Where(number => number >= DateTime.Now.Year).ToList())
            {
                anos.Add(new SelectValue { IdString = ano.ToString(), Descricao = ano.ToString() });
            }

            ViewBag.Anos = anos;

            return View();
        }

        public ActionResult VendasNaoEfetivadas()
        {
            return View();
        }

        public ActionResult EscalaTrabalho()
        {
            return View();
        }
        public ActionResult Relatorio_Custos_Imp_Servico()
        {
            return View();
        }

        public ActionResult PainelPerformance()
        {
            return View();
        }


        public ActionResult RelatorioReagendamentoOs()
        {
            return View();
        }

        /*public ActionResult RelatorioChecklist() 
        {
            return View();
        }*/


        public async Task<ActionResult> PowerBI()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                var config = await repoConfig.selectFirst();

                if (config.UrlPowerBI == null)
                {
                    throw new Exception("Não existe URL de Relatório Indicadores CRM configuradas!");
                }
                else
                {
                    ViewBag.UrlPowerBI = config?.UrlPowerBI;
                    return View();
                }
            }
        }

        public ActionResult RelatorioLogProvisionamento()
        {
            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Campanha = new CampanhaListaModel();

            var _lista_campanha = new CampanhaApiController().GetList().Result;

            if (!_lista_campanha.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR);

            ViewBag.Campanha = _lista_campanha.Objeto;
        }

        public ActionResult RelatorioSOS()
        {
            return View();
        }

        public ActionResult RelatorioVendasReprovadasScore()
        {
            return View();
        }

        public async Task<ActionResult> RelatorioPainelAcompanhamentoAlertasSOS()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                var config = await repoConfig.selectFirst();
                ViewBag.Tempo_Atualizacao_Alerta_SOS = config?.Tempo_Atualizacao_Alerta_SOS ?? 1;
            }

            ViewBag.Areas = Areas;

            return View();
        }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}