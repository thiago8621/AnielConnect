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
    public class Tipo_Checkpoint_VendedorController : BaseController
    {

        public Tipo_Checkpoint_VendedorController() : base("Tipo_Checkpoint_Vendedor",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Tipo_Checkpoint_Vendedorapi/FindById?id={0}",
                                                   "api/Tipo_Checkpoint_Vendedorapi/Gravar",
                                                   "api/Tipo_Checkpoint_Vendedorapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Tipo_Checkpoint_Vendedor
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            return base.CadastroBase<Tipo_Checkpoint_VendedorCadastroModel, Int64?>(id);
        }

        // POST: Tipo_Checkpoint_Vendedor/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Tipo_Checkpoint_VendedorCadastroModel pModel)
        {
            ViewBag.Areas = Areas;
            ActionResult retorno = base.CadastroBase<Tipo_Checkpoint_VendedorCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            
                                    
        }

        // GET: Tipo_Checkpoint_Vendedor/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Tipo_Checkpoint_VendedorCadastroModel, Int64>(id.Value);
        }

        // POST: Tipo_Checkpoint_Vendedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Tipo_Checkpoint_VendedorCadastroModel pModel)
        {
            return base.DeleteBase<Tipo_Checkpoint_VendedorCadastroModel, Int64>(pModel);
        }
    }
}