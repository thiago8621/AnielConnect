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
    public class ProgramacaoRoteirizacaoAutomaticaController : BaseController
    {

        public ProgramacaoRoteirizacaoAutomaticaController() : base("ProgramacaoRoteirizacaoAutomatica",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ProgramacaoRoteirizacaoAutomaticaapi/FindById?id={0}",
                                                   "api/ProgramacaoRoteirizacaoAutomaticaapi/Gravar",
                                                   "api/ProgramacaoRoteirizacaoAutomaticaapi/Excluir",
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
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<ProgramacaoRoteirizacaoAutomaticaCadastroModel, Int64?>(id);
            
            return retorno;
        }

        // POST: Agente/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ProgramacaoRoteirizacaoAutomaticaCadastroModel pModel)
        {
            ViewBag.Areas = Areas;
            return base.CadastroBase<ProgramacaoRoteirizacaoAutomaticaCadastroModel, Int64>(pModel);
        }

        
    }
}