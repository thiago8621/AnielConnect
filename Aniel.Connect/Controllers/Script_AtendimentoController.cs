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
    public class Script_AtendimentoController : BaseController
    {

        public Script_AtendimentoController() : base("Script_Atendimento",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Script_Atendimentoapi/FindById?id={0}",
                                                   "api/Script_Atendimentoapi/Gravar",
                                                   "api/Script_Atendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Script_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Script_AtendimentoCadastroModel, Int64?>(id);
        }

        // POST: Script_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Script_AtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Script_AtendimentoCadastroModel, Int64>(pModel);

            return retorno;
        }
                
        [HttpPost]
        public ActionResult AdicionarPergunta(Script_AtendimentoItemModel model)
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

        // GET: Script_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Script_AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: Script_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Script_AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<Script_AtendimentoCadastroModel, Int64>(pModel);
        }
    }
}