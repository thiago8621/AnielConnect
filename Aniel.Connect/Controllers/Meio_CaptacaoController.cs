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
    public class Meio_CaptacaoController : BaseController
    {

        public Meio_CaptacaoController() : base("Meio_Captacao", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Meio_Captacaoapi/FindById?id={0}",
                                                   "api/Meio_Captacaoapi/Gravar",
                                                   "api/Meio_Captacaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Meios_De_Captacao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Meio_CaptacaoCadastroModel, Int64?>(id);
        }

        // POST: Meios_De_Captacao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Meio_CaptacaoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Meio_CaptacaoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Meios_De_Captacao/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Meio_CaptacaoCadastroModel, Int64>(id.Value);
        }

        // POST: Meios_De_Captacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Meio_CaptacaoCadastroModel pModel)
        {
            return base.DeleteBase<Meio_CaptacaoCadastroModel, Int64>(pModel);
        }
    }
}