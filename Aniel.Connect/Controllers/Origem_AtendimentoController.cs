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
    public class Origem_AtendimentoController : BaseController
    {

        public Origem_AtendimentoController() : base("Origem_Atendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Origem_Atendimentoapi/FindById?id={0}",
                                                   "api/Origem_Atendimentoapi/Gravar",
                                                   "api/Origem_Atendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Origem_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Origem_AtendimentoCadastroModel, Int64?>(id);
        }

        // POST: Origem_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Origem_AtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Origem_AtendimentoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Origem_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Origem_AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: Origem_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Origem_AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<Origem_AtendimentoCadastroModel, Int64>(pModel);
        }
    }
}