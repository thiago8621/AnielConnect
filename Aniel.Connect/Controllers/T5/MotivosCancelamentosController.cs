using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers.T5
{
    public class MotivosCancelamentosController : BaseController
    {
        public MotivosCancelamentosController() : base("MotivosCancelamentos",  
                                                       CONSTANTES.URLPortalInterno,
                                                       String.Empty,
                                                       "api/MotivosCancelamentosapi/FindById?Id={0}",  
                                                       "api/MotivosCancelamentosapi/Gravar",          
                                                       "api/MotivosCancelamentosapi/Excluir",         
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
            return (ViewResult)base.CadastroBase<MotivosCancelamentos_CadastroModels, string>(id);  
        }

        [HttpPost()]
        public ActionResult Cadastro(MotivosCancelamentos_CadastroModels value)  
        {
            return base.CadastroBase<MotivosCancelamentos_CadastroModels, String>(value);  
        }
    }
}