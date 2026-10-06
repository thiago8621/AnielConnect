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
    public class DescontoController : BaseController
    {

        public DescontoController() : base("Desconto", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Descontoapi/FindById?id={0}",
                                                   "api/Descontoapi/Gravar",
                                                   "api/Descontoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Desconto
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<DescontoCadastroModel, Int64?>(id);

            if (!id.HasValue) 
            {
                ((DescontoCadastroModel)retorno.Model).TipoValor = TipoValor_DescontoCadastroModel.Valor;
                ((DescontoCadastroModel)retorno.Model).Valor_Unico = true;
            }


            return retorno;
        }

        // POST: Desconto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(DescontoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<DescontoCadastroModel, Int64>(pModel);
            ViewBag.Tipo_Desconto = new Tipo_DescontoApiController().GetList().Result.Objeto;

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoDescontoMeio = new Desconto_Meio_CaptacaoApiController().GetList().Result;
            var _retornoMeios = new Meio_CaptacaoApiController().GetList().Result;

            if (!_retornoMeios.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMeios.Mensagem, _retornoMeios.InnerException);
            if (!_retornoDescontoMeio.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoDescontoMeio.Mensagem, _retornoDescontoMeio.InnerException);

            ViewBag.Meios = _retornoMeios.Objeto;
            ViewBag.Desconto_Meio_Captacao = _retornoDescontoMeio.Objeto;
        }

        // GET: Desconto/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<DescontoCadastroModel, Int64>(id.Value);
        }

        // POST: Desconto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DescontoCadastroModel pModel)
        {
            return base.DeleteBase<DescontoCadastroModel, Int64>(pModel);
        }
    }
}