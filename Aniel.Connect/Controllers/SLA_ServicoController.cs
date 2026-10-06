using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class SLA_ServicoController : BaseController
    {

        public SLA_ServicoController() : base("SLA_Servico", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/sla_Servicoapi/FindById?id={0}",
                                                   "api/sla_Servicoapi/Gravar",
                                                   "api/sla_Servicoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: SLA_Servico
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            


            ViewResult retorno = (ViewResult)base.CadastroBase<SLA_ServicoCadastroModel, Int64?>(id);

            //if (!id.HasValue)
                //((SLA_ServicoCadastroModel)retorno.Model).Calendario_Padrao = true;

            return retorno;
        }

        // POST: SLA_Servico/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(SLA_ServicoCadastroModel pModel)
        {
           


            ActionResult retorno = base.CadastroBase<SLA_ServicoCadastroModel, Int64>(pModel);
            if (pModel.SLA_Retroativo)
            {
                SLA_ServicoApiController servicoApiController = new SLA_ServicoApiController();
                servicoApiController.RecalcularSLA(pModel.Id).Wait();
            }
            return retorno;
        }

        // GET: SLA_Servico/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<SLA_ServicoCadastroModel, Int64>(id.Value);
        }

        // POST: SLA_Servico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(SLA_ServicoCadastroModel pModel)
        {
            return base.DeleteBase<SLA_ServicoCadastroModel, Int64>(pModel);
        }


        protected override void FillListasView<T>(T pModel)
        {
            
        }

    }
}