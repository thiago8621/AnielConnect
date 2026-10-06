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
    public class Mensagem_SistemaController : BaseController
    {

        public Mensagem_SistemaController() : base("Mensagem_Sistema", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Mensagem_Sistemaapi/FindById?id={0}",
                                                   "api/Mensagem_Sistemaapi/Gravar",
                                                   "api/Mensagem_Sistemaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Origem_Atendimento
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Mensagem_SistemaCadastroModel, Int64?>(id);
        }

        // POST: Origem_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Mensagem_SistemaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Mensagem_SistemaCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            //var _retornoScripts = new Mensagem_SistemaApiController().GetList().Result;

            //if (!_retornoScripts.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            //ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Mensagem_Sistema/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Mensagem_SistemaCadastroModel, Int64>(id.Value);
        }

        // POST: Origem_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Mensagem_SistemaCadastroModel pModel)
        {
            return base.DeleteBase<Mensagem_SistemaCadastroModel, Int64>(pModel);
        }
    }
}