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
    public class RadiusController : BaseController
    {

        public RadiusController() : base("Radius", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Radiusapi/FindById?id={0}",
                                                   "api/Radiusapi/Gravar",
                                                   "api/Radiusapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Radius
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<RadiusCadastroModel, Int64?>(id);
        }

        // POST: Radius/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(RadiusCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<RadiusCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Radius/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<RadiusCadastroModel, Int64>(id.Value);
        }

        // POST: Radius/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(RadiusCadastroModel pModel)
        {
            return base.DeleteBase<RadiusCadastroModel, Int64>(pModel);
        }
    }
}