using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {

        UserManager<IdentityUser> _userManager;
        // GET: Chat
        public ChatController() : base("Chat",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Chatapi/FindById?id={0}",
                                                   "api/Chatapi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: Chat
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        //GET: FrameChat
        public ActionResult ChatFrame(ChatBackOfficeModel pModel, String Num_OS, String Num_Obra, String Cod_Ct, String Projeto, String Num_Doc)
        {
            if (pModel == null)
            {
                pModel = new ChatBackOfficeModel();
            }

            ViewBag.NumOs = Num_Obra;
            pModel.Num_Obra = Num_Obra;
            pModel.Num_doc = Num_Doc;
            pModel.Cod_ct = Cod_Ct;
            pModel.Projeto = Projeto;

            string numFre = null;
            using (ISession session = HibernateUtil.GetSession())
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Nome_Func = String.IsNullOrEmpty(usuario.Nome) ? usuario.UserName : usuario.Nome;
                pModel.Usuario = String.IsNullOrEmpty(usuario.Nome) ? usuario.UserName : usuario.Nome;
                numFre = usuario?.Funcionario?.Num_Fre;
            }

            //verifica se o perfil é de técnico, monitor ou supervisor 
            //Se não, Verifica se possui área de acesso para visualizar mensagens
            //Se não, exibe mensagem "Este usuário não possui perfil de supervisor ou monitor e também não tem área de acesso para visualizar todos os técnicos."
            var chatApi = new ChatApiController();
            var usuarioVinculado = chatApi.GetPermissaoOS(numFre, Num_Obra, Cod_Ct, Projeto, Num_Doc).Result;

            if (usuarioVinculado.Count == 0)
            {
                ViewBag.temAcesso = true;
                ViewBag.bloqueiaMensagem = true;
                ViewBag.tipoUsuario = 3;
                pModel.Tipo_Usuario = "3";

                if (Areas.Find(a => a.Cod_Area == "282") == null)
                {
                    pModel.bloqueiaMensagem = true;
                }
                else
                {
                    pModel.bloqueiaMensagem = false;
                }

            }
            else
            {
                ViewBag.temAcesso = false;
                ViewBag.bloqueiaMensagem = false;
                ViewBag.tipoUsuario = null;

                pModel.Tipo_Usuario = null;
                pModel.bloqueiaMensagem = false;
            }

            ViewBag.Areas = Areas;


            return View(pModel);
        }

        public ActionResult Chat_iFrame(String Num_Obra, String Cod_Ct, String Projeto, String Num_Doc)
        {
            string numFre = null;
            using (ISession session = HibernateUtil.GetSession())
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Nome_Func = String.IsNullOrEmpty(usuario.Nome) ? usuario.UserName : usuario.Nome;
                numFre = usuario?.Funcionario?.Num_Fre;
            }

            //verifica se o perfil é de técnico, monitor ou supervisor 
            //Se não, Verifica se possui área de acesso para visualizar mensagens
            //Se não, exibe mensagem "Este usuário não possui perfil de supervisor ou monitor e também não tem área de acesso para visualizar todos os técnicos."
            var chatApi = new ChatApiController();
            var usuarioVinculado = chatApi.GetPermissaoOS(numFre, Num_Obra, Cod_Ct, Projeto, Num_Doc).Result;

            if (usuarioVinculado.Count == 0)
            {
                if (Areas.Find(p => p.Cod_Area == "282") != null)
                {
                    ViewBag.temAcesso = true;
                    ViewBag.bloqueiaMensagem = false;
                    ViewBag.tipoUsuario = 3;
                }
                else
                {
                    return RedirectToAction("ErroSemAcesso", "Home");
                }
            }
            else
            {
                if (Areas.Find(p => p.Cod_Area == "282") != null)
                {
                    ViewBag.temAcesso = true;
                }
                else
                {
                    ViewBag.temAcesso = false;
                }

                ViewBag.bloqueiaMensagem = false;
                ViewBag.tipoUsuario = null;
            }

            ViewBag.Areas = Areas;


            return View();
        }

        //public ActionResult ChatFrame()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult Painel_Chat()
        {
            string numFre = null;
            using (ISession session = HibernateUtil.GetSession())
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Nome_Func = String.IsNullOrEmpty(usuario.Nome) ? usuario.UserName : usuario.Nome;
                numFre = usuario?.Funcionario?.Num_Fre;
            }

            //verifica se o perfil é de técnico, monitor ou supervisor 
            //Se não, Verifica se possui área de acesso para visualizar mensagens
            //Se não, exibe mensagem "Este usuário não possui perfil de supervisor ou monitor e também não tem área de acesso para visualizar todos os técnicos."
            var chatApi = new ChatApiController();
            var usuarioVinculado = chatApi.GetPermissaoOS(numFre, null, null, null, null).Result;

            if (usuarioVinculado.Count == 0)
            {
                if (Areas.Find(p => p.Cod_Area == "282") != null)
                {
                    ViewBag.temAcesso = true;
                    ViewBag.bloqueiaMensagem = true;
                    ViewBag.tipoUsuario = 3;
                }
                else
                {
                    return RedirectToAction("ErroSemAcesso", "Home");
                }
            }
            else
            {
                if (Areas.Find(p => p.Cod_Area == "282") != null)
                {
                    ViewBag.temAcesso = true;
                }
                else
                {
                    ViewBag.temAcesso = false;
                }

                ViewBag.bloqueiaMensagem = false;
                ViewBag.tipoUsuario = null;
            }

            ViewBag.Areas = Areas;
            return View();
        }


        private UserManager<IdentityUser> GetUserManager(ISession session)
        {
            if (_userManager == null)
                _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(session));
            return _userManager;
        }

    }
}
