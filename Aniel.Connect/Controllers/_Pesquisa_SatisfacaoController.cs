using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class _Pesquisa_SatisfacaoController : Controller
    {
        // GET: PesquisaSatisfacao
        public ActionResult Index()
        {
            return View();
        }
        
        // GET: PesquisaSatisfacao
        public ActionResult Cadastro()
        {
            return View();
        }
    }
}