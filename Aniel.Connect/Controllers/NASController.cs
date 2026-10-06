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
    public class NASController : BaseController
    {

        public NASController() : base("NAS", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/NASapi/FindById?id={0}",
                                                   "api/NASapi/Gravar",
                                                   "api/NASapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: NAS
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<NASCadastroModel, Int64?>(id);
        }

        // POST: NAS/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(NASCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<NASCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: NAS/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<NASCadastroModel, Int64>(id.Value);
        }

        // POST: NAS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(NASCadastroModel pModel)
        {
            return base.DeleteBase<NASCadastroModel, Int64>(pModel);
        }
    }
}