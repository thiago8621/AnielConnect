using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ConfiguracaoController : BaseController
    {
        public ConfiguracaoController() : base("Configuracao", CONSTANTES.URLPortalInterno)
        {

        }

        // GET: Configuracao
        public ActionResult Index()
        {
            using (Configuracoes_GeraisApiController api = new Configuracoes_GeraisApiController()) {

                var conf_gerais = api.FindById(1).Result.Objeto;
                ViewBag.Utiliza_Point = conf_gerais.UtilizaPoint_RoteirizacaoAgendamento;
            }
                ViewBag.Areas = Areas;

            return View();
        }

        // GET: Financeiras
        public ActionResult Financeira()
        {
            return View();
        }

        // GET: Relativas ao Atendimento
        public ActionResult Atendimento()
        {
            return View();
        }

        // GET: Pesquisa de Satisfacao
        public ActionResult Pesquisa()
        {
            return View();
        }

    }
}