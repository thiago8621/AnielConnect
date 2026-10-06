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
    public class Cobranca_ServicoController : BaseController
    {

        public Cobranca_ServicoController() : base("Cobranca_Servico",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Cobranca_ServicoApi/FindById?id={0}",
                                                   "api/Cobranca_ServicoApi/Gravar",
                                                   "api/Cobranca_ServicoApi/Excluir",
                                                   String.Empty)


            
                                                  
        {

        }

        // GET: Cobranca_Servico
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Cobranca_ServicoCadastroModel, Int64?>(id);
        }

        // POST:Cobranca_Servico/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Cobranca_ServicoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Cobranca_ServicoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoMotivos = new Motivo_Status_AtendimentoApiController().GetList().Result;

            if (!_retornoMotivos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMotivos.Mensagem, _retornoMotivos.InnerException);

            ViewBag.Motivos = _retornoMotivos.Objeto;
        }

        // GET: Categoria_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Cobranca_ServicoCadastroModel, Int64>(id.Value);
        }

        // POST: Categoria_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Cobranca_ServicoCadastroModel pModel)
        {
            return base.DeleteBase<Cobranca_ServicoCadastroModel, Int64>(pModel);
        }
    }
}