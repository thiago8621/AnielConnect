using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using System.Threading.Tasks;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi.T5;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Modelo_Elemento_RedeController : BaseController
    {
        public Modelo_Elemento_RedeController() : base("Modelo_Elemento_Rede",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_Elemento_RedeApi/FindById?id={0}",
                                                   "api/Modelo_Elemento_RedeApi/Gravar",
                                                   "api/Modelo_Elemento_RedeApi/Excluir",
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
            return base.CadastroBase<Modelo_Elemento_RedeCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(Modelo_Elemento_RedeCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<Modelo_Elemento_RedeCadastroModel, Int64>(model);
            return retorno;
        }

        [HttpGet]
        //public ActionResult CadastroCampo()
        //{
        public ActionResult CadastroCampo(long? Id)
        {
            ActionResult retorno = base.CadastroBase<Modelo_Elemento_RedeCampoDinamicoViewModel, Int64?>(Id);
            return View(new Modelo_Elemento_RedeApiController().GetCampoDinamico(null).Result.Objeto);
        }

        [HttpPost]
        public ActionResult CadastroCampo(Modelo_Elemento_RedeCampoDinamicoViewModel model)
        {
            var retorno = new Modelo_Elemento_RedeApiController().GravarCampoDinamico(model).Result;
            //FillListasView();
            return View(new Modelo_Elemento_RedeApiController().GetCampoDinamico(null).Result.Objeto);
        }
    }
}