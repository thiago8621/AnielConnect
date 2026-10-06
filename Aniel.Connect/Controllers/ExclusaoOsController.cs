using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ExclusaoOsController : BaseController
    {

        public ExclusaoOsController() : base("ExclusaoOS",
                                              CONSTANTES.URLPortalInterno,
                                              String.Empty,
                                              "api/ExclusaoOsApiController/FindById?id={0}",
                                              "api/ExclusaoOsApiController/Gravar",
                                              "api/ExclusaoOsApiController/Excluir",
                                              String.Empty)
        {

        }


        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Log()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpPost]
        public ActionResult EnviarPlanilha()
        {
            using (var api = new ExclusaoOsApiController())
            {
                try
                {
                    if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                    {
                        AddModelError("Informe Planilha a ser Importada");
                        return RedirectToAction("Index", "ExclusaoOs");
                    }

                    if (ModelState.IsValid)
                    {
                        if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                        {
                            DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false);
                            var ordensServico = ExcelService.CastDataTableImportacao<ExclusaoServicoPlanilha>(dt);
                            List<String> Erros = api.ExcluiOrdemServico(ordensServico.Items).Result;
                            this.TempData["Erros"] = Erros;

                        }
                    }
                }
                catch(Exception ex)
                {
                    this.TempData["Erros"] = new List<string> { ex.Message };
                    //throw ex;
                }
            }

            return RedirectToAction("Index", "ExclusaoOs");
        }
    }
}