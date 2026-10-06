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
    public class Tipo_DescontoController : BaseController
    {

        public Tipo_DescontoController() : base("Tipo_Desconto", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Tipo_DescontoApi/FindById?id={0}",
                                                   "api/Tipo_DescontoApi/Gravar",
                                                   "api/Tipo_DescontoApi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Desconto
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Tipo_DescontoCadastroModel, Int64?>(id);

            

            return retorno;
        }

        // POST: Desconto/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Tipo_DescontoCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Tipo_DescontoCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {

        }
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Tipo_DescontoCadastroModel, Int64>(id.Value);
        }

        // POST: Desconto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Tipo_DescontoCadastroModel pModel)
        {
            return base.DeleteBase<Tipo_DescontoCadastroModel, Int64>(pModel);
        }
    }
}