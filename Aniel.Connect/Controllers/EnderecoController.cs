using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aniel.Connect.Models;
using Aniel.Connect.ControllersWebApi;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class EnderecoController : BaseController
    {
        public EnderecoController() : base("Endereco", CONSTANTES.URLPortalInterno,null,null,null,null) 
        { }
        public ActionResult Index() 
        {
            return View();
        }

    }
}