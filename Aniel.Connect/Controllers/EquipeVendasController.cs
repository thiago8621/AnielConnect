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
using static Aniel.Connect.Models.EquipeVendasModel;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class EquipeVendasController : BaseController
    {
        public EquipeVendasController() : base("EquipeVendas", CONSTANTES.URLPortalInterno,
                                                    String.Empty,
                                                   "api/EquipeVendaApi/FindById?id={0}",
                                                   "api/EquipeVendaApi/Gravar",
                                                   "api/EquipeVendaApi/Excluir",
                                                   String.Empty)
        {
        }
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<EquipeVendasCadastroModel, Int64?>(id);
            return retorno;
        }

        [HttpPost]
        public ActionResult Cadastro(EquipeVendasCadastroModel model)
        {           
            ActionResult retorno = base.CadastroBase<EquipeVendasCadastroModel, Int64>(model);
            return retorno;
        }
    }
}