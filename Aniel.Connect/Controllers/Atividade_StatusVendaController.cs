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
    public class Atividade_StatusVendaController : BaseController
    {

        public Atividade_StatusVendaController() : base("Atividade_StatusVenda",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Atividade_StatusVendaApi/FindById?id={0}",
                                                   "api/Atividade_StatusVendaApi/Gravar",
                                                   "api/Atividade_StatusVendaApi/Excluir",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Atividade_StatusVendaCadastroModels, Int64?>(id);



            return retorno;
        }

        // POST: Status/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Atividade_StatusVendaCadastroModels pModel)
        {
            ActionResult retorno = base.CadastroBase<Atividade_StatusVendaCadastroModels, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Atividade_StatusVendaCadastroModels, Int64>(id.Value);
        }

        // POST: Status/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Atividade_StatusVendaCadastroModels pModel)
        {
            return base.DeleteBase<Atividade_StatusVendaCadastroModels, Int64>(pModel);
        }
    }
}