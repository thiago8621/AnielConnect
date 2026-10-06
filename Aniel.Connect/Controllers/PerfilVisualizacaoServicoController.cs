using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PerfilVisualizacaoServicoController : BaseController
    {
        public PerfilVisualizacaoServicoController() : base("PerfilVisualizacaoServico",
                                                            CONSTANTES.URLPortalInterno,
                                                            String.Empty,
                                                            "api/PerfilVisualizacaoServicoApi/FindById?id={0}",
                                                            "api/PerfilVisualizacaoServicoApi/Gravar",
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
            return this.CadastroBase<PerfilVisualizacaoServicoCadastroModel, long?>(id);
        }
        
        [HttpPost]
        public ActionResult Cadastro(PerfilVisualizacaoServicoCadastroModel model)
        {
            return this.CadastroBase<PerfilVisualizacaoServicoCadastroModel, long?>(model);
        }
    }
}