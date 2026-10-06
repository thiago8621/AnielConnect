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
    public class OrganizacaoController : BaseController
    {

        public OrganizacaoController() : base("Organizacao", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Organizacaoapi/FindById?id={0}",
                                                   "api/Organizacaoapi/Gravar",
                                                   "api/Organizacaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Organizacao
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<OrganizacaoCadastroModel, Int64?>(id);

            return retorno;
        }

        // POST: Organizacao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(OrganizacaoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<OrganizacaoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
        }
    }
}