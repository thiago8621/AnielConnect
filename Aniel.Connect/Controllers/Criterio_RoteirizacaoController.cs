
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using FluentNHibernate.Testing.Values;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    public class Criterio_RoteirizacaoController : BaseController
    {
        public Criterio_RoteirizacaoController() : base("Criterio_Roteirizacao",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            "api/Criterio_Roteirizacaoapi/FindById?id={0}",
                                            "api/Criterio_Roteirizacaoapi/Gravar",
                                            "api/Criterio_Roteirizacaoapi/Excluir",
                                            String.Empty)
        {

        }

        [HttpGet()]
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<Criterio_RoteirizacaoCadastroModel, Int64?>(0);
            
            return View();
        }

        [HttpPost()]
        public ActionResult Index(Criterio_RoteirizacaoCadastroModel valor)
        {
            return base.CadastroBase<Criterio_RoteirizacaoCadastroModel, Int64>(valor);

        }


    }
}