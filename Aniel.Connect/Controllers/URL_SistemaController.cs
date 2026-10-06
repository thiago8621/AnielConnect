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
    public class URL_SistemaController : BaseController
    {

        public URL_SistemaController() : base("URL_Sistema", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/URL_Sistemaapi/FindById?id={0}",
                                                   "api/URL_Sistemaapi/Gravar",
                                                   "api/URL_Sistemaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: URL_Sistema
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<URL_SistemaCadastroModel, Int64?>(id);


            /*
            if (!id.HasValue)
                ((URL_SistemaCadastroModel)retorno.Model).TipoValor = TipoValor_URL_SistemaCadastroModel.Valor;
                */


            return retorno;
        }

        // POST: URL_Sistema/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(URL_SistemaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<URL_SistemaCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoMeios = new Meio_CaptacaoApiController().GetList().Result;

            if (!_retornoMeios.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMeios.Mensagem, _retornoMeios.InnerException);

            ViewBag.Meios = _retornoMeios.Objeto;
        }

        // GET: URL_Sistema/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<URL_SistemaCadastroModel, Int64>(id.Value);
        }

        // POST: URL_Sistema/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(URL_SistemaCadastroModel pModel)
        {
            return base.DeleteBase<URL_SistemaCadastroModel, Int64>(pModel);
        }
    }
}