using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class ParametrosConsultaCreditoController : BaseController
    {

        public ParametrosConsultaCreditoController() : base("ParametrosConsultaCredito",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            "api/ParametrosConsultaCreditoapi/FindById?id={0}",
                                            "api/ParametrosConsultaCreditoapi/Gravar",
                                            "api/ParametrosConsultaCreditoapi/ExcluirCriterios",
                                            String.Empty)
        {

        }

        // GET:
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro()
        {
            return base.CadastroBase<ParametrosConsultaCreditoCadastroModel, Int64?>(1);
        }

        // POST: /Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ParametrosConsultaCreditoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<ParametrosConsultaCreditoCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
                return RedirectToAction("Index", "Configuracao");
            else
                return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.ListaCriterios = new List<CriteriosParamCredito>();

        }
      

    }
}