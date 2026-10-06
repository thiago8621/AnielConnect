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
    public class SLA_ExecucaoController : BaseController
    {

        public SLA_ExecucaoController() : base("SLA_Atendimento", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/SLA_Execucaoapi/FindById?id={0}",
                                                   "api/SLA_Execucaoapi/Gravar",
                                                   "api/SLA_Execucaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: SLA_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<SLA_ExecucaoCadastroModel, Int64?>(id);

            if (!id.HasValue)
                ((SLA_ExecucaoCadastroModel)retorno.Model).Calendario_Padrao = true;

            return retorno;
        }

        // POST: SLA_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(SLA_ExecucaoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<SLA_ExecucaoCadastroModel, Int64>(pModel);

            return retorno;
        }

        // GET: SLA_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<SLA_ExecucaoCadastroModel, Int64>(id.Value);
        }

        // POST: SLA_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(SLA_ExecucaoCadastroModel pModel)
        {
            return base.DeleteBase<SLA_ExecucaoCadastroModel, Int64>(pModel);
        }
    }
}