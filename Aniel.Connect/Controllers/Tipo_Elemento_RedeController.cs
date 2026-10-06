using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Controllers;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Tipo_Elemento_RedeController: BaseController
    {
        public Tipo_Elemento_RedeController() : base("Tipo_Elemento_Rede",
                                                      CONSTANTES.URLPortalInterno,
                                                      "api/Tipo_Elemento_RedeApi/GetAll",
                                                      "api/Tipo_Elemento_RedeApi/FindById?Id={0}", 
                                                      "api/Tipo_Elemento_RedeApi/Gravar",
                                                      String.Empty) 
        {
        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [HttpGet]
        public ActionResult Cadastro(Int64? id) 
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Tipo_Elemento_RedeCadastroModel,Int64?>(id);
            return retorno;

        }

        [HttpPost]
        public ActionResult Cadastro(Tipo_Elemento_RedeCadastroModel model) 
        {
            ActionResult retorno = base.CadastroBase<Tipo_Elemento_RedeCadastroModel, Int64>(model);
            return retorno;
        }

    }
}