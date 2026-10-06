using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Perfil_VendedorController : BaseController
    {

        public Perfil_VendedorController() : base("Perfil_Vendedor",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Perfil_VendedorApi/FindById?id={0}",
                                                   "api/Perfil_VendedorApi/Gravar",
                                                   "api/Perfil_VendedorApi/Excluir",
                                                   String.Empty)            
                                                  
        {

        }

        // GET: Cobranca_Servico
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Perfil_VendedorCadastroModel, Int64?>(id);
        }

        // POST:Cobranca_Servico/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Perfil_VendedorCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Perfil_VendedorCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoMotivos = new Motivo_Status_AtendimentoApiController().GetList().Result;

            if (!_retornoMotivos.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMotivos.Mensagem, _retornoMotivos.InnerException);

            ViewBag.Motivos = _retornoMotivos.Objeto;
        }

        // GET: Categoria_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Perfil_VendedorModels, Int64>(id.Value);
        }

        // POST: Categoria_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Perfil_VendedorCadastroModel pModel)
        {
            return base.DeleteBase<Perfil_VendedorCadastroModel, Int64>(pModel);
        }

        [HttpGet()]
        public ActionResult Historico_Alteracao(Int64? id)
        {
            ViewBag.Chave = "Id";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }


    }
}