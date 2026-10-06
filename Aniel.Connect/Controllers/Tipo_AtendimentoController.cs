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
    public class Tipo_AtendimentoController : BaseController
    {

        public Tipo_AtendimentoController() : base("Tipo_Atendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/tipo_atendimentoapi/FindById?id={0}",                                                    
                                                   "api/tipo_atendimentoapi/Gravar",
                                                   "api/tipo_atendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Tipo_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            var _retornoMotivos = new Motivo_Status_AtendimentoApiController().GetList().Result;

            if (!_retornoMotivos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMotivos.Mensagem, _retornoMotivos.InnerException);

            ViewBag.Motivos = _retornoMotivos.Objeto;

            ViewBag.Areas = Areas;
            return base.CadastroBase<Tipo_AtendimentoCadastroModel, Int64?>(id);
        }

        // POST: Tipo_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Tipo_AtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Tipo_AtendimentoCadastroModel, Int64>(pModel);
            ViewBag.Areas = Areas;

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoMotivos = new Motivo_Status_AtendimentoApiController().GetList().Result;

            if (!_retornoMotivos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMotivos.Mensagem, _retornoMotivos.InnerException);

            ViewBag.Motivos = _retornoMotivos.Objeto;
        }

        // GET: Tipo_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Tipo_AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: Tipo_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Tipo_AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<Tipo_AtendimentoCadastroModel, Int64>(pModel);
        }
    }
}