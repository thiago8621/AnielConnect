using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Modelo_PainelServicoController : BaseController
    {

        public Modelo_PainelServicoController() : base("Modelo_PainelServico",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_PainelServicoApi/FindById?id={0}",
                                                   "api/Modelo_PainelServicoApi/Gravar",
                                                   "api/Modelo_PainelServicoApi/Excluir",
                                                   String.Empty)
        {

        }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas, "162");
            
            return base.CadastroBase<Modelo_PainelServicoCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(Modelo_PainelServicoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Modelo_PainelServicoCadastroModel, Int64>(pModel);
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas, "162");
            return retorno;
        }

    }
}