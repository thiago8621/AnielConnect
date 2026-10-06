using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Pool_Ip_EnderecoController : BaseController
    {
        public Pool_Ip_EnderecoController() : base("Pool_Ip_Endereco",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   String.Empty,
                                                   String.Empty,
                                                   "api/Pool_Ip_Enderecoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Pool_Ip_Endereco
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }
    }
}