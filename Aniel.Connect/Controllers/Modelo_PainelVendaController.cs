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
    public class Modelo_PainelVendaController : BaseController
    {

        public Modelo_PainelVendaController() : base("Modelo_PainelVenda",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Modelo_PainelVendaApi/FindById?id={0}",
                                                   "api/Modelo_PainelVendaApi/Gravar",
                                                   "api/Modelo_PainelVendaApi/Excluir",
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
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas, "162");
            return base.CadastroBase<Modelo_PainelVendaCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(Modelo_PainelVendaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Modelo_PainelVendaCadastroModel, Int64>(pModel);
            ViewBag.AdmistradorModelos = AcessoHelper.TemAcesso(Areas, "162");
            return retorno;
        }
    }
}