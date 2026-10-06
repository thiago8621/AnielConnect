using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ParametrosConsultaPreQualController : BaseController
    {
        public ParametrosConsultaPreQualController() : base("ParametrosConsultaPreQual",
                                                            CONSTANTES.URLPortalInterno,
                                                            String.Empty,
                                                            "api/ParametrosConsultaPreQualApi/FindById?id={0}",
                                                            "api/ParametrosConsultaPreQualApi/Gravar",
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
            return this.CadastroBase<ParametrosConsultaPreQualCadastroModel, long?>(id);
        }
        
        [HttpPost]
        public ActionResult Cadastro(ParametrosConsultaPreQualCadastroModel model)
        {
            return this.CadastroBase<ParametrosConsultaPreQualCadastroModel, long?>(model);
        }
    }
}