using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers.T5
{
    public class ChecklistController : BaseController
    {
        public ChecklistController() : base("Checklist", 
                                                   CONSTANTES.URLPortalInterno,
                                                   "api/ChecklistApi/GetAll",
                                                   "api/ChecklistApi/FindById?Id={0}",
                                                   "api/ChecklistApi/Gravar",
                                                   String.Empty) 
        {

        }
        public ViewResult Index() 
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(long? id)
        {
            return base.CadastroBase<ChecklistCadastroModel, long?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(ChecklistCadastroModel pModel)
        {
            return base.CadastroBase<ChecklistCadastroModel, long>(pModel);
        }
    }
}