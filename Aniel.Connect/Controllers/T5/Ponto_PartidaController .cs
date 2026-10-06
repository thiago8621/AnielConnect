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
    [Authorize]
    public class Ponto_PartidaController : BaseController
    {

        public Ponto_PartidaController() : base("Ponto_Partida",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Ponto_Partidaapi/FindById?id={0}",
                                                   "api/Ponto_Partidaapi/Gravar",
                                                   "api/Ponto_Partidaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Ponto_Partida
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Ponto_PartidaCadastroModel, Int64?>(id);
        }

        // POST: Ponto_Partida/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Ponto_PartidaCadastroModel Model)
        {
            PostApiRetorno<Int64> retornoPost = base.PostByUrl<Ponto_PartidaCadastroModel, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Ponto_PartidaApi/Gravar");

            return RedirectToAction("Index");

            //return View(Model);
        }

        protected override void FillListasView<T>(T pModel)
        {
            

            ViewBag.ComboTmp = new List<ComboObj>();

        }

        // GET: Ponto_Partida/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Ponto_PartidaCadastroModel, Int64>(id.Value);
        }

        // POST: Ponto_Partida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Ponto_PartidaCadastroModel pModel)
        {
            return base.DeleteBase<Ponto_PartidaCadastroModel, Int64>(pModel);
        }

        
    }
}