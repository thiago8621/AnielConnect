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
    public class Categoria_ClienteController : BaseController
    {

        public Categoria_ClienteController() : base("Categoria_Cliente", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Categoria_Clienteapi/FindById?id={0}",
                                                   "api/Categoria_Clienteapi/Gravar",
                                                   "api/Categoria_Clienteapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Categoria_Cliente
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Categoria_ClienteCadastroModel, Int64?>(id);
        }

        // POST: Categoria_Cliente/Cadastro/
        [HttpPost]
        public ActionResult Cadastro(Categoria_ClienteCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Categoria_ClienteCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {


            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Categoria_Cliente/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Categoria_ClienteCadastroModel, Int64>(id.Value);
        }

        // POST: Categoria_Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Categoria_ClienteCadastroModel pModel)
        {
            return base.DeleteBase<Categoria_ClienteCadastroModel, Int64>(pModel);
        }
    }
}