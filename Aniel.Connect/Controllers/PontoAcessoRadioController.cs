using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
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
    public class PontoAcessoRadioController : BaseController
    {

        public PontoAcessoRadioController() : base("PontoAcessoRadio",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/PontoAcessoRadioApi/FindById?id={0}",
                                                   "api/PontoAcessoRadioApi/Gravar",
                                                   "api/PontoAcessoRadioApi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Ponto_Acesso_Radio
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;

            FillListasView();

            return View();
        }


        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<PontoAcessoRadioCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: PontoAcessoRadio/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(PontoAcessoRadioCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<PontoAcessoRadioCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected void FillListasView()
        {

            var _retornoPonto = new PontoAcessoRadioApiController().GetList().Result;

            if (!_retornoPonto.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPonto.Mensagem, _retornoPonto.InnerException);

            ViewBag.Ponto_Acesso_Radio = _retornoPonto.Objeto;

            var _retornoIp = new PontoAcessoRadioApiController().GetListIp().Result;

            if (!_retornoIp.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoIp.Mensagem, _retornoIp.InnerException);

            ViewBag.IP = _retornoIp.Objeto;

         

        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
        }

        // GET: PontoAcessoRadio/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<PontoAcessoRadioCadastroModel, Int64>(id.Value);
        }

        // POST: PontoAcessoRadio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PontoAcessoRadioCadastroModel pModel)
        {
            return base.DeleteBase<PontoAcessoRadioCadastroModel, Int64>(pModel);
        }
    }
}