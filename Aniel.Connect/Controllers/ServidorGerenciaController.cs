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
    public class ServidorGerenciaController : BaseController
    {

        public ServidorGerenciaController() : base("ServidorGerencia", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ServidorGerenciaapi/FindById?id={0}",
                                                   "api/ServidorGerenciaapi/Gravar",
                                                   "api/ServidorGerenciaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: ServidorGerencia
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<ServidorGerenciaCadastroModel, Int64?>(id);
        }

        // POST: ServidorGerencia/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ServidorGerenciaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<ServidorGerenciaCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoMotivos = new Motivo_Status_AtendimentoApiController().GetList().Result;

            if (!_retornoMotivos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMotivos.Mensagem, _retornoMotivos.InnerException);

            ViewBag.Motivos = _retornoMotivos.Objeto;
        }

        // GET: ServidorGerencia/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<ServidorGerenciaCadastroModel, Int64>(id.Value);
        }

        // POST: ServidorGerencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ServidorGerenciaCadastroModel pModel)
        {
            return base.DeleteBase<ServidorGerenciaCadastroModel, Int64>(pModel);
        }
    }
}