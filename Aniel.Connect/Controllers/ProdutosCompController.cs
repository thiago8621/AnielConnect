using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ProdutosCompController : BaseController
    {
        public ProdutosCompController() : base("ProdutosComp",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ProdutosCompapi/FindById?id={0}",
                                                   "api/ProdutosCompapi/Gravar",
                                                   "api/ProdutosCompapi/Excluir",
                                                   String.Empty)
        { }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;

            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return (ViewResult)base.CadastroBase<ProdutosComp_CadastroModel, Int64?>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Cadastro(ProdutosComp_CadastroModel pModel)
        {
            using var produtosCompApi = new ProdutosCompApiController();
            
            var resultado = await produtosCompApi.Gravar(new ParamWebApi<ProdutosComp_CadastroModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });

            return RedirectToAction("Index", "ProdutosComp", new { Id = resultado.IdObjeto });
        }
    }
}