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
    public class Atendimento_AcaoController : BaseController
    {

        public Atendimento_AcaoController() : base("Atendimento_Acao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Atendimento_Acaoapi/FindById?id={0}",
                                                   "api/Atendimento_Acaoapi/Gravar",
                                                   "api/Atendimento_Acaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Atendimento
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Atendimento_AcaoCadastroModel, Int64?>(id);
        }

        // POST: Atendimento_Acao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Atendimento_AcaoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Atendimento_AcaoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>();


            var _retornoOrigem_Atendimento = new Origem_AtendimentoApiController().GetList().Result;

            if (!_retornoOrigem_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoOrigem_Atendimento.Mensagem, _retornoOrigem_Atendimento.InnerException);

            ViewBag.Origem_Atendimento = _retornoOrigem_Atendimento.Objeto;
                                    
        }

        // GET: Atendimento_Acao/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Atendimento_AcaoCadastroModel, Int64>(id.Value);
        }

        // POST: Atendimento_Acao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Atendimento_AcaoCadastroModel pModel)
        {
            return base.DeleteBase<Atendimento_AcaoCadastroModel, Int64>(pModel);
        }
    }
}