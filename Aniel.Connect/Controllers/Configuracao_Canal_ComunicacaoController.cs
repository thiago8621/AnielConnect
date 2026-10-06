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
    public class Configuracao_Canal_ComunicacaoController : BaseController
    {

        public Configuracao_Canal_ComunicacaoController() : base("Configuracao_Canal_Comunicacao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Configuracao_Canal_ComunicacaoApi/FindById?id={0}",
                                                   "api/Configuracao_Canal_ComunicacaoApi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Configuracao_Canal_Comunicacao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Configuracao_Canal_ComunicacaoCadastroModel, Int64?>(id);
        }

        // POST: Bot/Cadastro/5
        [HttpPost]
        public async Task<ActionResult> Cadastro(Configuracao_Canal_ComunicacaoCadastroModel pModel)
        {
            using var configApi = new Configuracao_Canal_ComunicacaoApiController();
            var resultado = await configApi.Gravar(new ParamWebApi<Configuracao_Canal_ComunicacaoCadastroModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });
            return RedirectToAction("Index", "Canal_Comunicacao", new { IdBot = resultado.IdObjeto });

        }
    }
}