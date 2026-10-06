using Aniel.Connect.ControllersWebApi;
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
    public class Configuracao_Envio_MensagensController : BaseController
    {
        public Configuracao_Envio_MensagensController() : base("Configuracao_Envio_Mensagens",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Configuracao_Envio_MensagensApi/FindById?id={0}",
                                                   "api/Configuracao_Envio_MensagensApi/Gravar",
                                                   String.Empty)
        {

        }

        //Ok
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }


        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Configuracao_Envio_MensagensCadastroModel, Int64?>(id);
        }

        //[HttpPost]
        //public async Task<ActionResult> Cadastro(Configuracao_Envio_MensagensCadastroModel pModel)
        //{
        //    var api = new Configuracao_Envio_MensagensApiController();

        //    var retorno = await api.Gravar(pModel);

        //    return RedirectToAction("Index", "Configuracao_Envio_Mensagens");
        //    //return retorno;
        //}

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Cadastro(Configuracao_Envio_MensagensCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Configuracao_Envio_MensagensCadastroModel, Int64>(pModel);

            return retorno;
        }
    }
}