using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PerfilAgendamentoController : BaseController
    {
        public PerfilAgendamentoController() : base("PerfilAgendamento", CONSTANTES.URLPortalInterno,
                                                    String.Empty,
                                                   "api/PerfilAgendamentoApi/FindById?id={0}",
                                                   "api/PerfilAgendamentoApi/Gravar",
                                                   "api/PerfilAgendamentoApi/Excluir",
                                                   String.Empty)
        { 
        }

        // GET: Comercial Index
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<PerfilAgendamentoCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(PerfilAgendamentoCadastroModel model)
        {
            return base.CadastroBase<PerfilAgendamentoCadastroModel, Int64>(model);
        }

        public ActionResult VincularPerfil()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult LogAlteracao()
        {
            ViewBag.Areas = Areas;
            return View();
        }
        
        public ActionResult LogVincularPerfil()
        {
            ViewBag.Areas = Areas;
            return View();
        }

    }
}