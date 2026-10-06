using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class SLA_AtendimentoController : BaseController
    {

        public SLA_AtendimentoController() : base("SLA_Atendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/sla_atendimentoapi/FindById?id={0}",
                                                   "api/sla_atendimentoapi/Gravar",
                                                   "api/sla_atendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: SLA_Atendimento
        public ActionResult Index()
        {
            ViewBag.Categorias_Cliente = new Categoria_ClienteApiController().GetList().Result.Objeto;
            ViewBag.Horarios = new HorarioApiController().GetListAtendimento().Result.Objeto;
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<SLA_AtendimentoCadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((SLA_AtendimentoCadastroModel)retorno.Model).Calendario_Padrao = true;

            return retorno;
        }

        // POST: SLA_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(SLA_AtendimentoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<SLA_AtendimentoCadastroModel, Int64>(pModel);

            return retorno;
        }

        // GET: SLA_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<SLA_AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: SLA_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(SLA_AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<SLA_AtendimentoCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Planos = new List<PlanoProdutoListModel>();

            ViewBag.HorariosPadrao = new List<Horario_ListModel>();

            ViewBag.Horarios = new List<Horario_ListModel>();

            ViewBag.Categorias_Cliente = new List<Categoria_ClienteListaModel>();

            var _retornoPlanos = new PlanoProdutoApiController().GetList().Result;

            var _retornoHorarioPadrao = new HorarioApiController().GetListAtendimentoPadrao().Result;

            var _retornoHorarios = new HorarioApiController().GetListAtendimento().Result;

            var _retornoCategorias = new Categoria_ClienteApiController().GetList().Result;

            if (!_retornoPlanos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoPlanos.Mensagem, _retornoPlanos.InnerException);

            if (!_retornoHorarios.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoHorarios.Mensagem, _retornoHorarios.InnerException);

            if (!_retornoHorarioPadrao.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoHorarioPadrao.Mensagem, _retornoHorarioPadrao.InnerException);

            if (!_retornoCategorias.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoHorarios.Mensagem, _retornoHorarios.InnerException);

            ViewBag.Planos = _retornoPlanos.Objeto;
            ViewBag.HorariosPadrao = _retornoHorarioPadrao.Objeto.FirstOrDefault()?.Id;
            ViewBag.Categorias_Cliente = _retornoCategorias.Objeto; 
            ViewBag.Horarios = _retornoHorarios.Objeto;
            ViewBag.ComboTmp = new List<ComboObj>();

        }

    }
}