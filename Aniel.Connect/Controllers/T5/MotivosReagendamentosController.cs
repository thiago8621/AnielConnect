using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers.T5
{
    public class MotivosReagendamentosController : BaseController
    {
        public MotivosReagendamentosController() : base("MotivosReagendamentos",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/MotivosReagendamentosapi/FindById?Id={0}",
                                                   "api/MotivosReagendamentosapi/Gravar",
                                                   "api/MotivosReagendamentosapi/Excluir",
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
            return (ViewResult)base.CadastroBase<MotivosReagendamentos_CadastroModels, string>(id);
        }

        [HttpPost()]
        public ActionResult Cadastro(MotivosReagendamentos_CadastroModels value)
        {
            return base.CadastroBase<MotivosReagendamentos_CadastroModels, String>(value);
        }
    }
}