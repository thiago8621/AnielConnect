using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Acompanhamento_EquipesController : BaseController
    {

        public Acompanhamento_EquipesController() : base("Acompanhamento_Equipes",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Acompanhamento_Equipes/FindById?id={0}",
                                                   "api/Acompanhamento_Equipes/Gravar",
                                                   "api/Acompanhamento_Equipes/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Acompanhamento_Equipes
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;
                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;

            }
            ViewResult retorno = (ViewResult)base.CadastroBase<CampanhaCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Acompanhamento_Equipes/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(CampanhaCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<CampanhaCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;
                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;

            }
            //ViewBag.Planos = new List<PlanoProdutoListModel>();

            //var _retornoPlanos = new PlanoProdutoApiController().GetList().Result;

            //if (!_retornoPlanos.Ok)
            //    AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);


            //ViewBag.Planos = _retornoPlanos.Objeto;

        }

        // GET: Acompanhaeto_Equipes/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<CampanhaCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(CampanhaCadastroModel pModel)
        {
            return base.DeleteBase<CampanhaCadastroModel, Int64>(pModel);
        }
    }
}