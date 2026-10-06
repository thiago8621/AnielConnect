using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Atividade_NfController : BaseController
    {

        public Atividade_NfController() : base("Atividade_NF",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Atividade_NFapi/FindById?id={0}",
                                                   "api/Atividade_NFapi/Gravar",
                                                   "api/Atividade_NFapi/Excluir",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Atividade_NfCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Campanha/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Atividade_NfCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Atividade_NfCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

            ViewBag.Planos = new List<Atividade_NfListModel>();

            var _retornoPlanos = new Atividade_NfApiController().GetList(Tipo_Atividade_Nf.Plano_Produto).Result;

            if (!_retornoPlanos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            
            ViewBag.Planos = _retornoPlanos.Objeto;

        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Atividade_NfCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Atividade_NfCadastroModel pModel)
        {
            return base.DeleteBase<Atividade_NfCadastroModel, Int64>(pModel);
        }
    }
}