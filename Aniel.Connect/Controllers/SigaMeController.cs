using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class SigaMeController : BaseController
    {
        public SigaMeController() : base("Sigame", CONSTANTES.URLPortal,
                                                    String.Empty,
                                                   "api/SigaMeApi/FindById?id={0}",
                                                   "api/SigaMeApi/Gravar",
                                                   "api/SigaMeApi/Excluir",
                                                   String.Empty)
        {            
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<SigaMeCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(SigaMeCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<SigaMeCadastroModel, Int64>(model);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
                return RedirectToAction("Cadastro", "SigaMe");
            else
                return retorno;

          //  return base.CadastroBase<SigaMeCadastroModel, Int64>(model);
        }
    }
}