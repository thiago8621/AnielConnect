using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PerfilVisualizacaoVendaController : BaseController
    {
        public PerfilVisualizacaoVendaController() : base("PerfilVisualizacaoVenda",
                                                            CONSTANTES.URLPortalInterno,
                                                            String.Empty,
                                                            "api/PerfilVisualizacaoVendaApi/FindById?id={0}",
                                                            "api/PerfilVisualizacaoVendaApi/Gravar",
                                                            String.Empty)
        {

        }

        public ActionResult Index()
        {
            ViewBag.Areas = this.Areas;
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(long? id)
        {
            return this.CadastroBase<PerfilVisualizacaoVendaCadastroModel, long?>(id);
        }
        
        [HttpPost]
        public ActionResult Cadastro(PerfilVisualizacaoVendaCadastroModel model)
        {
            return this.CadastroBase<PerfilVisualizacaoVendaCadastroModel, long?>(model);
        }
    }
}