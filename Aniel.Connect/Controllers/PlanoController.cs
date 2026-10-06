using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class PlanoController : BaseController
    {
        public PlanoController() : base("Plano",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Planoapi/FindById?id={0}",
                                                   "api/Planoapi/Gravar",
                                                   "api/Planoapi/Excluir",
                                                   String.Empty)

        {
        }
            // GET: Configuracao_Planos_Produtos
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            
            return base.CadastroBase<PlanoCadastroModel, Int64?>(id);
        }

        // POST: Desconto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(PlanoCadastroModel pModel)
        {
            var retorno = base.CadastroBase<PlanoCadastroModel, Int64>(pModel);
            
            if (retorno.GetType() == typeof(RedirectToRouteResult))
                return RedirectToAction("Index", "Plano_Produto");

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Atividadenf = new List<Atividade_NfListModel>();
            ViewBag.Atividadenf_Instalacao = new List<Atividade_NfListModel>();
            ViewBag.ModRecibo = new List<Modelo_ReciboListaModel>();            
            ViewBag.ModContrato = new List<Modelo_ContratoListaModel>();

            var _retornoAtividadenf = new Atividade_NfApiController().GetList(Tipo_Atividade_Nf.Plano_Produto).Result;
            var _retornoAtividadenf_Instalacao = new Atividade_NfApiController().GetList(Tipo_Atividade_Nf.Instalacao).Result;
            var _retornoModContrato = new Modelo_ContratoApiController().GetList().Result;

            if (!_retornoAtividadenf.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoAtividadenf.Mensagem, _retornoAtividadenf.InnerException);
            
            if (!_retornoAtividadenf_Instalacao.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoAtividadenf_Instalacao.Mensagem, _retornoAtividadenf_Instalacao.InnerException);

            
            if (!_retornoModContrato.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoModContrato.Mensagem, _retornoModContrato.InnerException);

            ViewBag.Atividadenf = _retornoAtividadenf.Objeto;
            ViewBag.Atividadenf_Instalacao = _retornoAtividadenf_Instalacao.Objeto;
            ViewBag.ModContrato = _retornoModContrato.Objeto;

            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                ViewBag.UtilizaEva = configBusiness.GetConfiguracoesGerais()?.UtilizaEva;
            }

        }

        

        public ActionResult Descontos()
        {
            //var planoViewModel = new ConfiguracaoPlanoModel();
            return PartialView("DescontosViewModel.cshtml", null);
        }

        public ActionResult FaixaMensalidade()
        {
            //var planoViewModel = new ConfiguracaoPlanoModel();
            return PartialView("FaixaMensalidadeViewModel.cshtml", null);
        }

        public ActionResult Detalhes(Int64 Id, Int64? Id_Campanha)
        {
            var retorno = new PlanoApiController().GetListDetalhes(Id, Id_Campanha);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }
            
            return View(retorno.Objeto);
        }

    }
}