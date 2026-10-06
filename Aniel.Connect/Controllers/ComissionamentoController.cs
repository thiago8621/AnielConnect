using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ComissionamentoController : BaseController
    {

        public ComissionamentoController() : base("Comissionamento",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Comissionamentoapi/FindById?id={0}",
                                                   "api/Comissionamentoapi/Gravar",
                                                   "api/Comissionamentoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Campanha
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<ComissionamentoCadastroModel, Int64?>(id);



            return retorno;
        }

        // POST: Campanha/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(ComissionamentoCadastroModel pModel)
        {
            Boolean novo = pModel.Id == 0;

            //SetTokenByUser();

            try
            {
                if (ModelState.IsValid)
                {
                    PostApiRetorno<Int64> retorno = PostByUrl<ComissionamentoCadastroModel, Int64>(pModel, UrlPortal, UrlPost);

                    //Verifica se esta nulo e se é diferente do valor default
                    if (retorno.IdObjeto != 0) //Edição
                    {
                        if (novo)
                        {
                            return RedirectToAction("Cadastro", new { Id = retorno.IdObjeto });
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, retorno.Mensagem, retorno.InnerException);
                    }
                }

                FillListasView(pModel);
            }
            catch (Exception ex)
            {
                FillListasView(pModel);

                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(pModel);

        }

        protected override void FillListasView<T>(T pModel)
        {

        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<ComissionamentoCadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ComissionamentoCadastroModel pModel)
        {
            return base.DeleteBase<ComissionamentoCadastroModel, Int64>(pModel);
        }

        public ActionResult Comissao(Int64 Id)
        {
            var retorno = new ComissionamentoApiController().GetComissionamentoPlanoProduto(Id);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

        [HttpPost]
        public async Task<ActionResult> Comissao(Comissionamento_PlanoProdutoListaModel pModel)
        {
            //Gravar
            var retorno = await new ComissionamentoApiController().GravarPlanoProduto(pModel);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
                return View(pModel);
            }
            else
            {
                ViewBag.Sucesso = true;
                return View(pModel);
            }
        }

    }
}