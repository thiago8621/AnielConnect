using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
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
    public class Atributo_IPOEController : BaseController
    {

        public Atributo_IPOEController() : base("Atributo_IPOE",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Atributo_IPOEapi/FindById?id={0}",
                                                   "api/Atributo_IPOEapi/Gravar",
                                                   "api/Atributo_IPOEapi/Excluir",
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
            ViewResult retorno = (ViewResult)base.CadastroBase<Atributo_IPOECadastroModel, Int64?>(id);

            //if (!id.HasValue)
            //    ((CampanhaCadastroModel)retorno.Model).TipoValor = TipoValor_CampanhaCadastroModel.Valor;

            return retorno;
        }

        // POST: Campanha/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Atributo_IPOECadastroModel pModel)
        {
            return base.CadastroBase<Atributo_IPOECadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            
        }

        // GET: Campanha/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Atributo_IPOECadastroModel, Int64>(id.Value);
        }

        // POST: Campanha/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Atributo_IPOECadastroModel pModel)
        {
            return base.DeleteBase<Atributo_IPOECadastroModel, Int64>(pModel);
        }
    }
}