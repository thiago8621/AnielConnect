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
    public class Motivo_InviabilidadeController : BaseController
    {

        public Motivo_InviabilidadeController() : base("Motivo_Inviabilidade", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Motivo_Inviabilidadeapi/FindById?id={0}",
                                                   "api/Motivo_Inviabilidadeapi/Gravar",
                                                   "api/Motivo_Inviabilidadeapi/Excluir",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Motivo_InviabilidadeCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Desconto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Motivo_InviabilidadeCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Motivo_InviabilidadeCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Motivo_InviabilidadeCadastroModel, Int64>(id.Value);
        }

        // POST: Desconto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Motivo_InviabilidadeCadastroModel pModel)
        {
            return base.DeleteBase<Motivo_InviabilidadeCadastroModel, Int64>(pModel);
        }
    }
}