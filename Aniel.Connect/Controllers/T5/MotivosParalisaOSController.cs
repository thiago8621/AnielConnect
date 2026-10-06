using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers.T5
{
    public class MotivosParalisaOSController : BaseController
    {
        public MotivosParalisaOSController() : base("MotivosParalisaOS",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/MotivosParalisaOSapi/FindById?Id={0}",
                                                   "api/MotivosParalisaOSapi/Gravar",
                                                   "api/MotivosParalisaOSapi/Excluir",
                                                   String.Empty)
        { }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(string id)
        {
            return (ViewResult)base.CadastroBase<MotivosParalisaOS_CadastroModels, string>(id);
        }

        [HttpPost()]
        public ActionResult Cadastro(MotivosParalisaOS_CadastroModels value)
        {
            return base.CadastroBase<MotivosParalisaOS_CadastroModels, String>(value);
        }
    }
}