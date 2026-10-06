using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
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
    public class PortabilidadeController : BaseController
    {
        public PortabilidadeController() : base("Portabilidade",
                                                CONSTANTES.URLPortalInterno,                                                
                                                "api/PortabilidadeApi/GetAll",
                                                "api/PortabilidadeApiController/FindById",
                                                "api/PortabilidadeApiController/Gravar",
                                                "api/PortabilidadeApiController/Excluir"
                                                )

        {
        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            ViewBag.VisualizarPort = Areas.Find(p => p.Cod_Area == "209");
            ViewBag.EditarPort = Areas.Find(p => p.Cod_Area == "210");
            ViewBag.ExcluirPort = Areas.Find(p => p.Cod_Area == "211");
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<Portabilidade_CadastroViewModel, Int64?>(id);

            return retorno;
        }

        // POST: Agente/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Portabilidade_CadastroViewModel pModel)
        {
            ViewBag.Areas = Areas;
            return base.CadastroBase<Portabilidade_CadastroViewModel, Int64>(pModel);
        }


    }
}