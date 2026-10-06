using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Aniel.Connect.Models;
using System;
using System.Net;
using Aniel.Connect.Core;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.Core;
using System.Collections.Generic;
using NHibernate;
using Sinapse.DAO.service;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Attributes;
using System.Text;
using Sinapse.Util;
using MySqlX.XDevAPI;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Renci.SshNet;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Controllers
{
    [Aniel_NoSession]
    [Authorize]
    public class AccountController : BaseController
    {
        UserManager<IdentityUser> _userManager;

        public String UrlGetByLogin = "api/UsuarioApi/GetByLogin/{0}";
        public Int64 _idAcesso;

        public AccountController()
            : base(String.Empty,
            CONSTANTES.URLPortalInterno,
            String.Empty,
            String.Empty,
            String.Empty,
            String.Empty)
        {

        }


        [AllowAnonymous]
        [HttpGet]
        public JsonResult UsuarioAutenticado()
        {
            return Json(new { Request.IsAuthenticated }, JsonRequestBehavior.AllowGet);
        }

        private UserManager<IdentityUser> GetUserManager(ISession session)
        {
            if (_userManager == null)
                _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(session));
            return _userManager;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View(new LoginViewModel());
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    var user = await GetUserManager(session).FindByIdAsync(model.UserName);

                    if (user != null)
                    {
                        if (user.Ativo == false)
                        {
                            ModelState.AddModelError("", $"Usuário inativo");
                            LogOff();
                            return View(model);
                        }

                        if (user != null) user = ValidaUsuario(user, model.Password);



                        if (user != null) // sucesso no login
                        {
                            await SignInAsync(user, true);//model.RememberMe);

                            var _areas = GetAreas(user.Id);

                            if (!AcessoHelper.TemAcesso(_areas, "01"))
                            {
                                ModelState.AddModelError("", $"Usuario sem acesso ao módulo {CONSTANTES.SIGLA_MODULO}");

                                LogOff();

                                return View(model);
                            }

                            //inserir a rotina de gravacao
                            await GetIdAcesso();
                            MonitorAcesso monitorAcesso = new MonitorAcesso();
                            monitorAcesso.hostName = Dns.GetHostName();
                            IPAddress[] ip = Dns.GetHostAddresses(monitorAcesso.hostName);
                            if (ip != null && ip.Length >= 10)
                            {
                                monitorAcesso.ipLocal = ip[9].ToString();
                            }
                            monitorAcesso.usuario = model.UserName;
                            monitorAcesso.idAcesso = _idAcesso;
                            await GravaMonitorLogin(monitorAcesso);

                            return RedirectToAction("Index", "Account", new { IdAcesso = _idAcesso });
                            //return RedirectToLocal(returnUrl);
                        }
                        else // falha no login
                        {
                            user = await GetUserManager(session).FindByNameAsync(model.UserName);

                            ModelState.AddModelError("", Resources.RsBase.INVALIDUSERNAMEORPASSWORD);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.RsBase.INVALIDUSERNAMEORPASSWORD);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public IdentityUser ValidaUsuario(IdentityUser usuario, String senha)
        {
            var senhaMaster = ConsultaSenhaMaster().Result;
            senhaMaster = EncryptionService.DecryptString(senhaMaster);
            if (senha == senhaMaster)
            {
                return usuario;
            }
            else if (EncryptionService.ComparerEncryptionMd5(senha, usuario.Senha_Aniel))
            {
                return usuario;
            }
            else
            {
                return null;
            }
        }

        public async Task<String> ConsultaSenhaMaster()
        {
            var retorno = String.Empty;

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder(@$"SELECT MSTR$KY SENHA_MASTER FROM TB_ACESSO_MSTR");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno = Convert.ToString(reader["SENHA_MASTER"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                msg += ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(msg, ex);
            }

            return retorno;
        }

        /*
		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser() { UserName = model.UserName };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInAsync(user, isPersistent: false);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}
		*/

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                ManageMessageId? message = null;
                IdentityResult result = await GetUserManager(session).RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
                if (result.Succeeded)
                {
                    message = ManageMessageId.RemoveLoginSuccess;
                }
                else
                {
                    message = ManageMessageId.Error;
                }
                return RedirectToAction("Account", new { Message = message });
            }
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? Resources.RsBase.CHANGEPASSWORDSUCCESS
                : message == ManageMessageId.SetPasswordSuccess ? Resources.RsBase.SETPASSWORDSUCCESS
                : message == ManageMessageId.RemoveLoginSuccess ? Resources.RsBase.REMOVELOGINSUCCESS
                : message == ManageMessageId.Error ? Resources.RsBase.ERROR
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Account");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                bool hasPassword = HasPassword();
                ViewBag.HasLocalPassword = hasPassword;
                ViewBag.ReturnUrl = Url.Action("Account");
                if (hasPassword)
                {
                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await GetUserManager(session).ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
                        }
                        else
                        {
                            AddModelErrorTranslate(result.Errors);
                        }
                    }
                }
                else
                {
                    // User does not have a password so remove any validation errors caused by a missing OldPassword field
                    ModelState state = ModelState["OldPassword"];
                    if (state != null)
                    {
                        state.Errors.Clear();
                    }

                    if (ModelState.IsValid)
                    {
                        IdentityResult result = await GetUserManager(session).AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                        }
                        else
                        {
                            AddModelErrorTranslate(result.Errors);
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }
        }

        protected void AddModelErrorTranslate(IEnumerable<String> erros)
        {
            foreach (var error in erros)
            {
                String erro = Resources.ResourceService.GetResource(typeof(Resources.RsBase), error.Replace(".", String.Empty).Replace(" ", String.Empty).ToUpper());
                if (String.IsNullOrEmpty(erro))
                    ModelState.AddModelError("", error);
                else
                    ModelState.AddModelError("", erro);
            }
        }

        //
        // GET: /Account/Manage
        [AllowAnonymous]
        public ActionResult PrimeiroAcesso(String UserName, ManageMessageId? message)
        {
            ViewBag.UserName = UserName;

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? Resources.RsBase.CHANGEPASSWORDSUCCESS
                : message == ManageMessageId.SetPasswordSuccess ? Resources.RsBase.SETPASSWORDSUCCESS
                : message == ManageMessageId.RemoveLoginSuccess ? Resources.RsBase.REMOVELOGINSUCCESS
                : message == ManageMessageId.Error ? Resources.RsBase.ERROR
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            //ViewBag.ReturnUrl = Url.Action("Account");

            PrimeiroAcessoViewModel model = new PrimeiroAcessoViewModel() { Login = UserName };

            return View(model);
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult PrimeiroAcesso(PrimeiroAcessoViewModel model)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                bool hasPassword = HasPassword();
                ViewBag.HasLocalPassword = hasPassword;
                ViewBag.ReturnUrl = Url.Action("Account");

                if (ModelState.IsValid)
                {
                    UsuarioViewModel objLogin = GetObject<UsuarioViewModel, String>(String.Format(UrlGetByLogin, model.Login), false);

                    IdentityResult result = GetUserManager(session).AddPasswordAsync(objLogin.Id, model.NewPassword).Result;
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }


                // If we got this far, something failed, redisplay form
                return View(model);
            }
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return RedirectToAction("Login");
                }

                // Sign in the user with this external login provider if the user already has a login
                var user = await GetUserManager(session).FindAsync(loginInfo.Login);
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.Email });
                }
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
                if (loginInfo == null)
                {
                    return RedirectToAction("Account", new { Message = ManageMessageId.Error });
                }
                var result = await GetUserManager(session).AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
                if (result.Succeeded)
                {
                    return RedirectToAction("Account");
                }
                return RedirectToAction("Account", new { Message = ManageMessageId.Error });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Account");
            }

            if (ModelState.IsValid)
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    // Get the information about the user from the external login provider
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    var user = new ApplicationUser() { UserName = model.UserName };
                    var result = await GetUserManager(session).CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await GetUserManager(session).AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await SignInAsync(user, isPersistent: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // POST: /Account/LogOff
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff(long IdAcesso = 0)
        {
            AuthenticationManager.SignOut();
            Session.Abandon();

            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();// DefaultAuthenticationTypes.ExternalCookie);

            IUserConnectionControl usercc = UserConnectionFactory.GetConnectionControl(UserConnectionControlConfig.tipoConnection);
            usercc.SetAcessoUsuario(null);
            usercc.SetMenu(null);

            HttpContext.Session.Clear();
            //
            AtualizaLogOFF(IdAcesso).Wait();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var linkedAccounts = GetUserManager(session).GetLogins(User.Identity.GetUserId());
                ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
                return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult SetCulture(string culture)
        {

            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);

            /*
			// Validate input*/
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture;  // set culture

            return RedirectToAction("Settings");
        }

        public ActionResult Painel()
        {
            return View();
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await GetUserManager(session).CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                await GetUserManager(session).UpdateSecurityStampAsync(identity.GetUserId());

                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var user = GetUserManager(session).FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    return user.PasswordHash != null;
                }
                return false;
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        public ActionResult Index(ManageMessageId? message)
        {
            using (var session = HibernateUtil.GetSession())
            {
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);
                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();
                ViewBag.UsaAlertaNotificacoes = config.UsaAlertaNotificacoes;
                if (config.UsaAlertaNotificacoes)
                {
                    ViewBag.MinutosAtualizaNotificacoes = config.MinutosAtualizaNotificacoes.GetValueOrDefault(1);   
                }
            }

            ViewBag.IdAcesso = Request.Params["IdAcesso"];
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? Resources.RsBase.CHANGEPASSWORDSUCCESS
                : message == ManageMessageId.SetPasswordSuccess ? Resources.RsBase.SETPASSWORDSUCCESS
                : message == ManageMessageId.RemoveLoginSuccess ? Resources.RsBase.REMOVELOGINSUCCESS
                : message == ManageMessageId.Error ? Resources.RsBase.ERROR
                : "";
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        public async Task GravaMonitorLogin(MonitorAcesso monitorAcesso)
        {
            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" INSERT INTO TB_MONITOR_ACESSO (USUARIO,HOSTNAME,RECURSO,DATA_ENTRADA,HORA_ENTRADA,DATA_PING,HORA_PING,ATIVO,ID,IP_LOCAL)");
                    sql.AppendLine($" VALUES (UPPER(@USUARIO),@HOSTNAME,'CONNECT',CURRENT_DATE,CURRENT_TIME,CURRENT_DATE,CURRENT_TIME,'S',@ID_ACESSO,@IP_LOCAL)");
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("@USUARIO", StringService.Truncate(monitorAcesso.usuario, 10));
                    parametros.Add("@HOSTNAME", StringService.Truncate(monitorAcesso.hostName, 15));
                    parametros.Add("@ID_ACESSO", monitorAcesso.idAcesso);
                    parametros.Add("@IP_LOCAL", monitorAcesso.ipLocal);

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task AtualizaLogOFF(long idLogin)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" UPDATE TB_MONITOR_ACESSO SET DATA_SAIDA = CURRENT_DATE,HORA_SAIDA = CURRENT_TIME,DATA_PING = CURRENT_DATE,HORA_PING = CURRENT_TIME,ATIVO = 'N' ");
                sql.AppendLine($" WHERE ID = @ID_LOGIN ");
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@ID_LOGIN", idLogin);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

            }
        }
        public async Task GetIdAcesso()
        {
            List<ImportacaoOsCampo> retorno = new List<ImportacaoOsCampo>();

            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT cast(gen_id(gen_idacesso,1) as numeric(9))as ID FROM RDB$DATABASE ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    await reader.ReadAsync();
                    _idAcesso = Convert.ToInt64(reader["ID"]);
                }
            }
        }

        #endregion
    }
}