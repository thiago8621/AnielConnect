using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.IntegracaoExterna;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class FuncionarioController : BaseController
    {
        public FuncionarioController() : base("Funcionario",
                                        CONSTANTES.URLPortalInterno,
                                        "api/Funcionarioapi/getall",
                                        "api/Funcionarioapi/FindById?id={0}",
                                        "api/Funcionarioapi/gravar",
                                        "api/Funcionarioapi/excluir",
                                         String.Empty)
        {

        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Supervisores = await new FuncionarioApiController().GetSupervisores();

            List<SelectValue> metas = new List<SelectValue>();
            List<SelectValue> comissionamentos = new List<SelectValue>();
            List<SelectValue> faixas = new List<SelectValue>();
            List<SelectValue> equipevenda = new List<SelectValue>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Meta>(session))
            using (var repoCom = new Conexao<Comissionamento>(session))
            using (var repoFaixa = new Conexao<Faixa_Comissionamento>(session))
            using (var repoEquipeVenda = new Conexao<EquipeVenda>(session))
            {
                var lista = await repository.queryover().ListAsync();

                foreach (var meta in lista)
                {
                    metas.Add(new SelectValue { Descricao = meta.Descricao, Id = meta.Id });
                }

                foreach (var com in await repoCom.select())
                {
                    comissionamentos.Add(new SelectValue { Descricao = com.Descricao, Id = com.Id });
                }

                foreach (var faixa in await repoFaixa.select())
                {
                    faixas.Add(new SelectValue { Id = faixa.Id, Descricao = faixa.Descricao });
                }
                foreach (var equipe in await repoEquipeVenda.select())
                {
                    equipevenda.Add(new SelectValue { Id = equipe.Id, Descricao = equipe.Descricao });
                }

            }

            ViewBag.Meta = metas;
            ViewBag.Comissionamento = comissionamentos;
            ViewBag.Faixa_Comissionamento = faixas;
            ViewBag.EquipeVenda = equipevenda;

            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Vendedor = new List<FuncionarioListaModel>();
            ViewBag.Supervisor = new List<FuncionarioListaModel>();
            ViewBag.Meta = new List<MetaListaModel>();
            ViewBag.Faixa_Comissionamento = new List<Faixa_ComissionamentoListModel>();
            ViewBag.SupervisorAniel = new List<FuncionarioListaModel>();
            ViewBag.Comissionamento = new List<ComissionamentoListaModel>();
            ViewBag.EquipeVenda = new List<EquipeVendasListaModel>();

            var _retornoVendedor = new FuncionarioApiController().GetListVendedor().Result;
            var _retornoSupervisor = new FuncionarioApiController().GetListSupervisor().Result;
            var _retornoMeta = new MetaApiController().GetList().Result;
            var _retornoFaixa = new Faixa_ComissionamentoApiController().GetList().Result;
            var _retornoSupervisorAniel = new FuncionarioApiController().GetListSupervisorAniel().Result;
            var _retornoSupervisorView = new FuncionarioApiController().GetListSupervisorView().Result;
            var _retornoComissionamento = new ComissionamentoApiController().GetList().Result;
            var _retornoEquipeVenda = new EquipeVendasApiController().GetList().Result;


            if (!_retornoVendedor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoVendedor.Mensagem, _retornoVendedor.InnerException);

            if (!_retornoSupervisor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSupervisor.Mensagem, _retornoVendedor.InnerException);

            if (!_retornoMeta.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMeta.Mensagem, _retornoMeta.InnerException);

            if (!_retornoFaixa.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoFaixa.Mensagem, _retornoFaixa.InnerException);

            if (!_retornoSupervisorAniel.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSupervisorAniel.Mensagem, _retornoFaixa.InnerException);

            if (!_retornoComissionamento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoComissionamento.Mensagem, _retornoFaixa.InnerException);

            if (!_retornoComissionamento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoComissionamento.Mensagem, _retornoFaixa.InnerException);

            if (!_retornoSupervisorView.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSupervisorView.Mensagem, _retornoFaixa.InnerException);

            if (!_retornoEquipeVenda.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSupervisorView.Mensagem, _retornoFaixa.InnerException);

            ViewBag.Vendedor = _retornoVendedor.Objeto;
            ViewBag.Meta = _retornoMeta.Objeto;
            ViewBag.Faixa_Comissionamento = _retornoFaixa.Objeto;
            ViewBag.Comissionamento = _retornoComissionamento.Objeto.ToList();
            ViewBag.EquipeVenda = new SelectList(_retornoEquipeVenda.Objeto.ToList(), "Id", "Descricao");



            ViewBag.Supervisores = new SelectList(_retornoSupervisorView.Objeto.ToList(), "Id", "Descricao");
        }

        [HttpGet()]
        public async Task<ActionResult> Cadastro(Int64? id, Boolean? E_Supervisor, String Id_Supervisor, String Tipo, Int64? IdMeta)
        {

            using (ISession sessionNh = HibernateUtil.GetSession())
            {
                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                ViewBag.UsaIntegracaoComercialMk = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MK_SOLUTION) != null;
                ViewBag.Supervisores = new SelectList(new FuncionarioApiController().GetListSupervisorView().Result.Objeto.ToList(), "Id", "Descricao");


                using (var RepoMeta = new Conexao<Meta>(sessionNh))
                using (var repoCom = new Conexao<Comissionamento>(sessionNh))
                using (var repoFaixa = new Conexao<Faixa_Comissionamento>(sessionNh))
                using (var repoEquipeVenda = new Conexao<EquipeVenda>(sessionNh))
                {
                    var lista = await RepoMeta.queryover().ListAsync();
                    List<SelectValue> metas = new List<SelectValue>();
                    List<SelectValue> comissionamentos = new List<SelectValue>();
                    List<SelectValue> faixas = new List<SelectValue>();
                    List<SelectValue> equipevenda = new List<SelectValue>();
                    foreach (var meta in lista)
                    {
                        metas.Add(new SelectValue { Descricao = meta.Descricao, Id = meta.Id });
                    }

                    foreach (var com in await repoCom.select())
                    {
                        comissionamentos.Add(new SelectValue { Descricao = com.Descricao, Id = com.Id });
                    }

                    foreach (var faixa in await repoFaixa.select())
                    {
                        faixas.Add(new SelectValue { Id = faixa.Id, Descricao = faixa.Descricao });
                    }
                    foreach (var equipe in await repoEquipeVenda.select())
                    {
                        equipevenda.Add(new SelectValue { Id = equipe.Id, Descricao = equipe.Descricao });
                    }

                    ViewBag.Meta = metas;
                    ViewBag.Comissionamento = comissionamentos;
                    ViewBag.Faixa_Comissionamento = faixas;
                    ViewBag.EquipeVenda = equipevenda;
                    ViewBag.Id_Funcionario = id;
                }

                if (ViewBag.UsaIntegracaoComercialMk != null && ViewBag.UsaIntegracaoComercialMk)
                {
                    ViewResult retorno = (ViewResult)base.CadastroBase<FuncionarioCadastroModel, Int64?>(id);
                    return await Task.FromResult(retorno);
                }
                else
                {

                    if (id == 0)
                    {
                        return View(new FuncionarioCadastroModel
                        {
                            Id = Convert.ToInt64(id),
                            E_SUPERVISOR = Convert.ToBoolean(E_Supervisor),
                            Id_Supervisor = Convert.ToInt64(Id_Supervisor),
                            Tipo = Tipo,
                            Id_Meta = IdMeta
                        });
                    }
                    else
                    {
                        return (ViewResult)base.CadastroBase<FuncionarioCadastroModel, Int64?>(id);
                    }
                }
            }
        }


        // POST: Funcionario/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(FuncionarioCadastroModel pModel)
        {
            ActionResult retorno = null;

            if (pModel.Id_Supervisor != null && pModel.E_SUPERVISOR == false)
            {
                pModel.Vendedor = true;
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Funcionario>(session))
                {
                    var modelo = new Conexao<Funcionario>(session).queryover().Where(m => m.Id == Convert.ToInt64(pModel.Id_Supervisor)).ListAsync().Result;
                    var equipevendaid = repository.select(p => p.IdEquipeVenda == Convert.ToInt64(pModel.IdEquipeVenda)).Result;

                    if (modelo.SingleOrDefault().Meta != null)
                    {
                        var func = modelo.SingleOrDefault();

                        if (pModel.Id_Meta == func?.Meta?.Id)
                        {
                            AddModelError("Meta não pode ser igual a do supervisor");
                            FillListasView(pModel);
                            return View(pModel);
                        }

                        if (pModel.Id_Comissionamento == func.Id_Comissionamento)
                        {
                            AddModelError("Tabela de Comissionamento não pode ser igual a do supervisor");
                            FillListasView(pModel);
                            return View(pModel);
                        }

                        if (pModel.Id_Faixa == func.Faixa?.Id)
                        {
                            AddModelError("Faixa de Comissionamento não pode ser igual a do supervisor");
                            FillListasView(pModel);
                            return View(pModel);
                        }

                        if (equipevendaid == null)
                        {
                            AddModelError("Adicione uma Equipe de venda");
                            FillListasView(pModel);
                            return View(pModel);
                        }
                    }
                }
            }

            if (pModel.Tipo == "Supervisor comercial")
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Funcionario>(session))
                using (var repositoryMeta = new Conexao<Meta>(session))
                using (var repositoryFaixa = new Conexao<Faixa_Comissionamento>(session))
                using (var repositoryComissionamento = new Conexao<Comissionamento>(session))
                {
                    List<Funcionario> funcionarios = repository.select(m => m.Supervisor != null).Result.Where(p => p.Supervisor?.Id == Convert.ToInt64(pModel.Id_Supervisor)).ToList();
                    Meta meta = repositoryMeta.selectFirst(p => p.Id == Convert.ToInt64(pModel.Id_Meta)).Result;
                    Faixa_Comissionamento faixa = repositoryFaixa.selectFirst(p => p.Id == Convert.ToInt64(pModel.Id_Faixa)).Result;
                    Comissionamento comissionamento = repositoryComissionamento.selectFirst(p => p.Id == Convert.ToInt64(pModel.Id_Comissionamento)).Result;

                    if (funcionarios.FindAll(p => p.Meta == meta).Count > 0)
                    {
                        AddModelError("Meta não pode ser igual a de funcionários supervisionados.");
                        FillListasView(pModel);
                        return View(pModel);
                    }

                    if (funcionarios.FindAll(p => p.Faixa == faixa).Count > 0)
                    {
                        AddModelError("Faixa de Comissionamento não pode ser igual a de funcionários supervisionados.");
                        FillListasView(pModel);
                        return View(pModel);
                    }

                    if (funcionarios.FindAll(p => p.Id_Comissionamento == comissionamento.Id).Count > 0)
                    {
                        AddModelError("Tabela de Comissionamento não pode ser igual a de funcionários supervisionados.");
                        FillListasView(pModel);
                        return View(pModel);
                    }

                }
            }

            retorno = base.CadastroBase<FuncionarioCadastroModel, Int64>(pModel);

            if (retorno.GetType() == typeof(RedirectToRouteResult))
            {

                ViewBag.Sucesso = true;
                FillListasView(pModel);
                //return View(pModel);
                return RedirectToAction("Cadastro", "Funcionario", new { id = pModel.Id, E_Supervisor = pModel.E_SUPERVISOR, Id_Supervisor = pModel.Id_Supervisor, Sucesso = "S", Id_Funcionario = pModel.Id_Funcionario, EquipeVenda = pModel.IdEquipeVenda });
            }

            return View(pModel);
        }


        [HttpGet]
        public ActionResult CadastroSupervisor(Int64? id)
        {
            return base.CadastroBase<FuncionarioCadastroModel, Int64?>(id);
        }


        // Cadastrar criterios em massa
        [HttpGet]
        public ActionResult CadastroEmMassa(Int64? id, Int64? meta, String faixa, Int64? comissionamento, Int64? Id_Supervisor, String Tipo, String funcionarios)
        {
            if (id == 0)
            {
                return View(new FuncionarioCadastroModel
                {
                    Desc_Faixa = faixa,
                    Id_Comissionamento = comissionamento,
                    Id_Meta = meta,
                    Id_Supervisor = Id_Supervisor,
                    Tipo = Tipo,
                    Funcionarios = funcionarios
                });
            }
            else
            {
                return base.CadastroBase<FuncionarioCadastroModel, Int64?>(id);
            }
        }

        // GET: Funcionario/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<FuncionarioCadastroModel, Int64>(id.Value);
        }

        // POST: Funcionario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(FuncionarioCadastroModel pModel)
        {
            return base.DeleteBase<FuncionarioCadastroModel, Int64>(pModel);
        }

    }
}