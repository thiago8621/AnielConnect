using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
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
    public class IdentityUserController : BaseController
    {

        public IdentityUserController() : base("IdentityUser",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/IdentityUserapi/FindById?id={0}",
                                                   "api/IdentityUserapi/Gravar",
                                                   "api/IdentityUserapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: IdentityUser
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult AdicionarMonitor(String id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<IdentityUserMonitorModel, String>(id);
            return retorno;
        }

        public ActionResult VincularMonitorLog(String id)
        {
            ViewBag.Areas = Areas;
            
            return View();
        }

        //[HttpGet()]
        //public ActionResult Cadastro(Int64? id)
        //{
        //    ViewResult retorno = (ViewResult)base.CadastroBase<IdentityUserCadastroModel, Int64?>(id);

        //    if (!id.HasValue)
        //        ((IdentityUserCadastroModel)retorno.Model).TipoValor = TipoValor_IdentityUserCadastroModel.Valor;

        //    return retorno;
        //}

        // post: identityuser/cadastro/5
        [HttpPost()]
        public ActionResult AdicionarMonitor(IdentityUserMonitorModel pmodel)
        {
            ViewBag.Areas = Areas;
            ActionResult retorno = base.CadastroBase<IdentityUserMonitorModel, String>(pmodel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                ViewBag.Sucesso = true;

                FillListasView(pmodel);

                return RedirectToAction("AdicionarMonitor", "IdentityUser");
            }

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoUsuarios = new IdentityUserApiController().GetList().Result;

            if (!_retornoUsuarios.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoUsuarios.Mensagem, _retornoUsuarios.InnerException);

            ViewBag.Usuarios = _retornoUsuarios.Objeto;
        }
        [HttpGet()]
        public ActionResult VincularMonitorLog()
        {
            return View();
        }


    }
}