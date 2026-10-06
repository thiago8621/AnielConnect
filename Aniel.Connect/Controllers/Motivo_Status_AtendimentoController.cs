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
    public class Motivo_Status_AtendimentoController : BaseController
    {

        public Motivo_Status_AtendimentoController() : base("Motivo_Status_Atendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/motivo_status_atendimentoapi/FindById?id={0}",
                                                   "api/motivo_status_atendimentoapi/Gravar",
                                                   "api/motivo_status_atendimentoapi/Excluir",
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
            return base.CadastroBase<Motivo_Status_AtendimentoCadastroModel, Int64?>(id);
        }

        // POST: Meios_De_Captacao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Motivo_Status_AtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Motivo_Status_AtendimentoCadastroModel, Int64>(pModel);

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

            return base.DeleteBase<Motivo_Status_AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: Meios_De_Captacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Motivo_Status_AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<Motivo_Status_AtendimentoCadastroModel, Int64>(pModel);
        }
    }
}