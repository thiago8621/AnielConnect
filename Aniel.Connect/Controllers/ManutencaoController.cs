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
    public class ManutencaoController : BaseController
    {


        public ManutencaoController() : base("Manutencao",
                                             CONSTANTES.URLPortalInterno,                                        
                                             "api/Area_ManutencaoApi/GetAll",
                                             "api/Area_ManutencaoApi/FindById?Id={0}",
                                             "api/Area_ManutencaoApi/Gravar",
                                             "api/Area_ManutencaoApi/Excluir"
                                             )
        {

        }


        // GET: Manutencao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        [HttpGet]
        public ActionResult Cadastro(Int64? Id)
        {
            ViewBag.Id = Id.HasValue?Id.Value:Id;
            return base.CadastroBase<Area_Cadastro_ManutencaoModels, Int64?>(Id);
        }

        [HttpPost]
        public ActionResult Cadastro(Area_Cadastro_ManutencaoModels model)
        {
            return base.CadastroBase<Area_Cadastro_ManutencaoModels, Int64>(model);
        }
        // GET: Tecnologia/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Area_Cadastro_ManutencaoModels, Int64>(id.Value);
        }

        // POST: Tecnologia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Area_Cadastro_ManutencaoModels pModel)
        {
            return base.DeleteBase<Area_Cadastro_ManutencaoModels, Int64>(pModel);
        }

    }
}