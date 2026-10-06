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
    public class Script_SuporteTecnicoController : BaseController
    {

        public Script_SuporteTecnicoController() : base("Script_SuporteTecnico",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Script_SuporteTecnicoapi/FindById?id={0}",
                                                   "api/Script_SuporteTecnicoapi/Gravar",
                                                   "api/Script_SuporteTecnicoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Script_SuporteTecnico
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Script_SuporteTecnicoCadastroModel, Int64?>(id);
        }

        // POST: Script_SuporteTecnico/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Script_SuporteTecnicoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Script_SuporteTecnicoCadastroModel, Int64>(pModel);

            return retorno;
        }
                
        [HttpPost]
        public ActionResult AdicionarPergunta(Script_SuporteTecnicoItemModel model)
        {
            return PartialView("~/Views/Shared/EditorTemplates/_ScriptsPerguntas.cshtml", model);
        }

        protected override void FillListasView<T>(T pModel)
        {
            //var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            //if (!_retornoScripts.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            //ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Script_SuporteTecnico/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Script_SuporteTecnicoCadastroModel, Int64>(id.Value);
        }

        // POST: Script_SuporteTecnico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Script_SuporteTecnicoCadastroModel pModel)
        {
            return base.DeleteBase<Script_SuporteTecnicoCadastroModel, Int64>(pModel);
        }
    }
}