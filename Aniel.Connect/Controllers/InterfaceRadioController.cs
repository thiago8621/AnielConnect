using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class InterfaceRadioController : BaseController
    {
        public InterfaceRadioController() : base("InterfaceRadio",
                                                  CONSTANTES.URLPortalInterno,
                                                  String.Empty,
                                                  "api/InterfaceRadioApi/FindById?id={0}",
                                                  "api/InterfaceRadioApi/Gravar",
                                                  "api/InterfaceRadioApi/Excluir",
                                                  String.Empty)
        {

        }

        // GET: InterfaceRadio
        public ActionResult Index()
        {
            //FillListasView();

            return View();
        }

        protected void FillListasView()
        {


            var _retornoInterface = new InterfaceRadioApiController().GetListInterfaceRadio().Result;

            if (!_retornoInterface.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoInterface.Mensagem, _retornoInterface.InnerException);

            ViewBag.InterfaceRadio = _retornoInterface.Objeto;

            //Ponto acesso
            var _retornoPontoAcessoRadio = new PontoAcessoRadioApiController().GetList().Result;

            if (!_retornoPontoAcessoRadio.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPontoAcessoRadio.Mensagem, _retornoPontoAcessoRadio.InnerException);

            ViewBag.Ponto_Acesso_Radio = _retornoPontoAcessoRadio.Objeto;

        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<InterfaceRadioCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: InterfaceRadio/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(InterfaceRadioCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<InterfaceRadioCadastroModel, Int64>(pModel);

            return retorno;
        }

     

        // GET: InterfaceRadio/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<InterfaceRadioCadastroModel, Int64>(id.Value);
        }

        // POST: InterfaceRadio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(InterfaceRadioCadastroModel pModel)
        {
            return base.DeleteBase<InterfaceRadioCadastroModel, Int64>(pModel);
        }

    }

}