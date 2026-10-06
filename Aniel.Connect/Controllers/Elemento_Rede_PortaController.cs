using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Elemento_Rede_PortaController : BaseController
    {
        public Elemento_Rede_PortaController() : base("Elemento_Rede_Porta", CONSTANTES.URLPortalInterno, 
            "api/Elemento_Rede_PortaApi/GetAll", String.Empty, "api/Elemento_Rede_PortaApi/Gravar", String.Empty) 
        {
        
        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [HttpGet]
        public ActionResult Cadastro(Int32? porta) 
        { 
            ViewBag.Areas = Areas; 
            return View();
        }

        [HttpPost]
        public ActionResult Cadastro(Elemento_Rede_PortaCadastroModel porta) 
        {
            Elemento_Rede_PortaCadastroModel retorno = new Elemento_Rede_PortaCadastroModel();
            retorno = porta;
            FillListasView(retorno);
            return View();

        }
    }
}
