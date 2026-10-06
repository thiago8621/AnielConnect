using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Configuracoes_GeraisController : BaseController
    {

        public Configuracoes_GeraisController() : base("Configuracoes_Gerais", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Configuracoes_Geraisapi/FindById?id={0}",
                                                   "api/Configuracoes_Geraisapi/Gravar",
                                                   "api/Configuracoes_Geraisapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Meios_De_Captacao
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro()
        {
            return base.CadastroBase<Configuracoes_GeraisCadastroModel, Int64?>(1);
        }

        // POST: Meios_De_Captacao/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Configuracoes_GeraisCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<Configuracoes_GeraisCadastroModel, Int64>(model);
            if (retorno.GetType() == typeof(RedirectToRouteResult))
                return RedirectToAction("Index", "Configuracao");
            else
                return retorno;
        }
     

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoSistemas = new URL_SistemaApiController().GetList().Result;

            if (!_retornoSistemas.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSistemas.Mensagem, _retornoSistemas.InnerException);
            ViewBag.Sistemas = _retornoSistemas.Objeto;


            ViewBag.Horarios = new List<Horario_Connect>();
            var _retornohorarios = new HorarioApiController().GetListServicos().Result;

            //if (!_retornohorarios.Ok)
            //{
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornohorarios.Mensagem, _retornohorarios.InnerException);
            //}
            ViewBag.Horarios = _retornohorarios.Objeto;
        }





    }
}