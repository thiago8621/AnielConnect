using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class FilialController : BaseController
    {

        public FilialController() : base("Filial",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/filialapi/FindById?id={0}",
                                                   "api/filialapi/Gravar",
                                                   "api/filialapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Filial
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<FilialCadastroModel, Int64?>(id);
        }

        // POST: Filial/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(FilialCadastroModel Model)
        {
            return base.CadastroBase<FilialCadastroModel, Int64>(Model);
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Filiais = new List<ObjBusca<Int64>>();

            ViewBag.ComboTmp = new List<ComboObj>();

        }

        // GET: Filial/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<FilialCadastroModel, Int64>(id.Value);
        }

        // POST: Filial/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FilialCadastroModel pModel)
        {
            return base.DeleteBase<FilialCadastroModel, Int64>(pModel);
        }

        
    }
}