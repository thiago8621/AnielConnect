using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
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
    public class Modelo_PainelAtendimentoController : BaseController
    {

        public Modelo_PainelAtendimentoController() : base("Modelo_PainelAtendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_PainelAtendimentoapi/FindById?id={0}",
                                                   "api/Modelo_PainelAtendimentoapi/Gravar",
                                                   "api/Modelo_PainelAtendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Categoria_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas, "162");
            return base.CadastroBase<Modelo_PainelAtendimentoCadastroModel, Int64?>(id);
        }

        // POST: Categoria_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Modelo_PainelAtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Modelo_PainelAtendimentoCadastroModel, Int64>(pModel);
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas,"162");
            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
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