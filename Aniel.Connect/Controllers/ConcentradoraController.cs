using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ConcentradoraController : BaseController
    {
        public ConcentradoraController() : base("Concentradora",
                                                CONSTANTES.URLPortalInterno,
                                                String.Empty,
                                                "api/ConcentradoraApi/FindById?id={0}",
                                                "api/ConcentradoraApi/Gravar",
                                                String.Empty,
                                                String.Empty)
        {

        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(Int64? Id)
        {
            return base.CadastroBase<ConcentradoraCadastroModel, Int64?>(Id);
        }

        [HttpPost]
        public ActionResult Cadastro(ConcentradoraCadastroModel model)
        {
            return base.CadastroBase<ConcentradoraCadastroModel, Int64>(model);
        }

        protected override void FillListasView<T>(T pModel)
        {
        
           
            //ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();

        }
    }
}