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
    public class Motivo_EncerramentoController : BaseController
    {

        public Motivo_EncerramentoController() : base("Motivo_Encerramento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Motivo_Encerramentoapi/FindById?id={0}",
                                                   "api/Motivo_Encerramentoapi/Gravar",
                                                   "api/Motivo_Encerramentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Motivo_Encerramento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            //var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            //if (!_retornoScripts.Ok)
            //    AddModelError(Resources.Resources.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            //ViewBag.Scripts = _retornoScripts.Objeto;

            return base.CadastroBase<Motivo_EncerramentoCadastroModel, Int64?>(id);
        }

        // POST: Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Motivo_EncerramentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Motivo_EncerramentoCadastroModel, Int64>(pModel);

            return retorno;
        }

        // GET: Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Motivo_EncerramentoCadastroModel, Int64>(id.Value);
        }

        // POST: Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Motivo_EncerramentoCadastroModel pModel)
        {
            return base.DeleteBase<Motivo_EncerramentoCadastroModel, Int64>(pModel);
        }
    }
}