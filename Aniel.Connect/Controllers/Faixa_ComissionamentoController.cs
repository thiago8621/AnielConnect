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
    public class Faixa_ComissionamentoController : BaseController
    {

        public Faixa_ComissionamentoController() : base("Faixa_Comissionamento",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Faixa_Comissionamentoapi/FindById?id={0}",
                                                   "api/Faixa_Comissionamentoapi/Gravar",
                                                   "api/Faixa_Comissionamentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Campanha
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Faixa_ComissionamentoCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Campanha/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Faixa_ComissionamentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Faixa_ComissionamentoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

            ViewBag.Planos = new List<Faixa_ComissionamentoListModel>();

            var _retornoPlanos = new Faixa_ComissionamentoApiController().GetList().Result;

            if (!_retornoPlanos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            
            ViewBag.Planos = _retornoPlanos.Objeto;

        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Faixa_ComissionamentoCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Faixa_ComissionamentoCadastroModel pModel)
        {
            return base.DeleteBase<Faixa_ComissionamentoCadastroModel, Int64>(pModel);
        }
    }
}