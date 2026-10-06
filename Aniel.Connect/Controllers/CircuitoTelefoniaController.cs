using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class CircuitoTelefoniaController : BaseController
    {
        public CircuitoTelefoniaController() : base("CircuitoTelefonia",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/CircuitoTelefoniaapi/FindById?id={0}",
                                                   "api/CircuitoTelefoniaapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<CircuitoTelefoniaCadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(CircuitoTelefoniaCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<CircuitoTelefoniaCadastroModel, Int64>(model);
            return retorno;
        }

        [HttpGet]
        public async Task<ActionResult> Detalhes(long? IdVenda, numero_Virtual? TelefoniaType)
        {
            using (CircuitoTelefoniaApiController api = new CircuitoTelefoniaApiController())
            {
                GetApiRetorno<CircuitoTelefoniaDetalhesModel> retorno = await api.GetDidsVenda(IdVenda);
                retorno.Objeto.Tipo_Numero = TelefoniaType.GetValueOrDefault(numero_Virtual.DID);

                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                }

                return View(retorno.Objeto);
            }
        }


        // TODO: IMPLEMENTAR GRAVAÇÃO DO DID MANUAL
        // TODO: PASSAR O ID DA VENDA PARA O MODEL DE DETALHES
        [HttpPost]
        public async Task<ActionResult> CadastroDidManual(CircuitoTelefoniaDetalhesModel model)
        {
            using (var api = new CircuitoTelefoniaApiController())
            {
                await api.GravarDidManual(model);
                
                return RedirectToAction("Detalhes", new { IdVenda = model.IdVenda });
            }
        }
    }
}