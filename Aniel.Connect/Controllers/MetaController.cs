using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Models.HubDesenvolvedor;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class MetaController : BaseController
    {
        public MetaController() : base("Meta",
                                        CONSTANTES.URLPortalInterno,
                                        "api/metaapi/getall",
                                        "api/metaapi/FindById?id={0}",
                                        "api/metaapi/gravar",
                                        "api/metaapi/excluir",
                                        String.Empty)
        {

        }

        public ActionResult Index()            
        {
            ViewBag.Areas = Areas;
            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Sucesso = false;
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            FillListasView();

            return base.CadastroBase<MetaCadastroModel, Int64?>(id);
        }

        // POST: Meta/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(MetaCadastroModel pModel)
        {
            FillListasView();

            ActionResult retorno = base.CadastroBase<MetaCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {
                ViewBag.Sucesso = true;
                return View(pModel);
            }

            return retorno;
        }

        // GET: Meta/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<MetaCadastroModel, Int64>(id.Value);
        }

        // POST: Meta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MetaCadastroModel pModel)
        {
            return base.DeleteBase<MetaCadastroModel, Int64>(pModel);
        }

        protected void FillListasView()
        {
            // ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>();
            
            var _retornoFilial = new FilialApiController().GetList().Result;

            if (!_retornoFilial.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoFilial.Mensagem, _retornoFilial.InnerException);

            ViewBag.Filiais = _retornoFilial.Objeto;


        }

    }
}