using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Canal_ComunicacaoController : BaseController
    {
        public Canal_ComunicacaoController() : base("Canal_Comunicacao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Canal_ComunicacaoApi/FindById?id={0}",
                                                   "api/Canal_ComunicacaoApi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Canal_Comunicacao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            using (var api = new Configuracao_Canal_ComunicacaoApiController())
            {
                var retorno = api.FindFirst().Result;
                if (retorno.Objeto != null)
                {
                    return View(retorno.Objeto);
                }
            }


            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Canal_Comunicacao_CadastroModel, Int64?>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Cadastro(Canal_Comunicacao_CadastroModel pModel)
        {
 
            //if (new Canal_ComunicacaoApiController().VerificaCanalExistente(pModel).Result > 0)
            //    throw new WarningException(String.Format("Os dados já se encontram cadastrados em um outro canal."));

            using var botapi = new Canal_ComunicacaoApiController();
            var resultado = await botapi.Gravar(new ParamWebApi<Canal_Comunicacao_CadastroModel> { Objeto = pModel, Usuario = User.Identity.GetUserId() });

            return RedirectToAction("Index", "Canal_Comunicacao");
        }

        //[HttpPost]
        //public async Task<PostApiRetorno<Int64>> Cadastro(Canal_Comunicacao_CadastroModel value)
        //{
        //    var param = new ParamWebApi<Canal_Comunicacao_CadastroModel>()
        //    {
        //        Objeto = value
        //    };

        //    var botapi = new Canal_ComunicacaoApiController();

        //    return await botapi.Gravar(param);
        //}
    }
}