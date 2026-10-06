using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class SegmentoController : BaseController
    {

        public SegmentoController() : base ("Segmento",
                                            CONSTANTES.URLPortal,
                                            "api/Segmentoapi/GetAll",
                                            "api/Segmentoapi/FindById?codigo={0}",
                                            "api/Segmentoapi/Gravar",
                                            "api/Segmentoapi/Delete")
        {

        }
        // GET: Segmento
        public ActionResult Index()
        {
            if (Areas.Find(p => p.Cod_Area == "324") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;

            return View();
        }
        

        [HttpGet()]
        public ActionResult Cadastro(String codigo = null)
        {
            if (codigo == null)
            {
                if (Areas.Find(func => func.Cod_Area == "325") == null)
                {
                    return RedirectToAction("ErroSemAcesso", "Home");
                }
            }
            else
            {
                if (Areas.Find(func => func.Cod_Area == "326") == null)
                {
                    return RedirectToAction("ErroSemAcesso", "Home");
                }
            }

            ViewBag.Areas = Areas;

            return base.CadastroBase<Segmento_CadastroModel, String>(codigo);
        }

        [HttpPost]
        public ActionResult Cadastro(Segmento_CadastroModel segmento_Cadastro)
        {
            ActionResult result = base.CadastroBase<Segmento_CadastroModel, String>(segmento_Cadastro);
            return result;
        }

        //[HttpPost()]
        //public ActionResult Cadastro(Segmento_CadastroModel model)
        //{
        //    return;
        //}

    }
}