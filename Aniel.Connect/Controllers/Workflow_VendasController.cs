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
using System.Globalization;
using System.Security.Claims;
using System.IO;
using System.Reflection;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Workflow_VendasController : BaseController
    {

        public Workflow_VendasController() : base("Workflow_Vendas",
                                                   CONSTANTES.URLPortalInterno,
                                                   "api/workflow_vendasapi/GetAll",
                                                   "api/workflow_vendasapi/FindById?id={0}",
                                                   "api/workflow_vendasapi/Gravar",
                                                   String.Empty,
                                                   String.Empty)
        {

        }

        // GET: Agente
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Workflow_VendasCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Agente/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Workflow_VendasCadastroModel pModel)
        {
            //
            return base.CadastroBase<Workflow_VendasCadastroModel, Int64>(pModel);
        }


    }
}