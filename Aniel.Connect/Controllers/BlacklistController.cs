using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using System;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Aniel.Connect.Controllers
{

    public class BlacklistController : BaseController
    {

        public BlacklistController() : base("Blacklist", CONSTANTES.URLPortal,
                                                    String.Empty,
                                                   "api/BlacklistApi/FindById?id={0}",
                                                   "api/BlacklistApi/Gravar",
                                                   "api/BlacklistApi/Excluir",
                                                   String.Empty)
        {           

        }

        [HttpGet()]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? idPlano)
        {
            
            return base.CadastroBase<BlacklistCadastroModel, Int64?>(idPlano);
            
        }

        [HttpPost]
        public ActionResult Cadastro(BlacklistCadastroModel model)
        {
            return base.CadastroBase<BlacklistCadastroModel, Int64>(model);
        }

    }
}