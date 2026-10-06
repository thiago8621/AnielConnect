using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class TarefasVendedorController : Controller
    {
        // GET: TarefasVendedor
        public ActionResult Index()
        {

            using (ISession session = HibernateUtil.GetSession())
            using (var userRepo = new Conexao<IdentityUser>(session))
            {
                var usuario = userRepo.selectFirst(p => p.Id == User.Identity.GetUserId()).Result;
                ViewBag.TipoUsuario = usuario?.Tipo_Usuario;
                ViewBag.IdFuncionario = usuario?.Funcionario?.Id;
            }

            return View();
        }

        public ActionResult Relatorio()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }
    }
}