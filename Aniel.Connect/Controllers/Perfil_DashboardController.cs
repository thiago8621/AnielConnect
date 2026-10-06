using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Perfil_DashboardController : BaseController
    {

        public Perfil_DashboardController() : base("Perfil_Dashboard",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Perfil_Dashboardapi/FindById?id={0}",
                                                   "api/Perfil_Dashboardapi/Gravar",
                                                   "api/Perfil_Dashboardapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Perfil_Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Perfil_DashboardCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((Perfil_DashboardCadastroModel)retorno.Model).TipoValor = TipoValor_Perfil_DashboardCadastroModel.Valor;

            return retorno;
        }

        // POST: Perfil_Dashboard/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Perfil_DashboardCadastroModel pModel)
        {
            //ActionResult retorno = base.CadastroBase<Perfil_DashboardCadastroModel, Int64>(pModel);

            //return retorno;

            return base.CadastroBase<Perfil_DashboardCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {

            //ViewBag.Planos = new List<PlanoProdutoListModel>();

            //var _retornoPlanos = new PlanoProdutoApiController().GetList().Result;

            //if (!_retornoPlanos.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            
            //ViewBag.Planos = _retornoPlanos.Objeto;

        }

        // GET: Perfil_Dashboard/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Perfil_DashboardCadastroModel, Int64>(id.Value);
        }

        // POST: Perfil_Dashboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Perfil_DashboardCadastroModel pModel)
        {
            return base.DeleteBase<Perfil_DashboardCadastroModel, Int64>(pModel);
        }
    }
}