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
    public class Pool_IpController : BaseController
    {

        public Pool_IpController() : base("Pool_Ip", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Pool_Ipapi/FindById?id={0}",
                                                   "api/Pool_Ipapi/Gravar",
                                                   "api/Pool_Ipapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Pool_Ip
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<Pool_IpCadastroModel, Int64?>(id);
        }

        // POST: Pool_Ip/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Pool_IpCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Pool_IpCadastroModel, Int64>(pModel);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;
        }

        // GET: Pool_Ip/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Pool_IpCadastroModel, Int64>(id.Value);
        }

        // POST: Pool_Ip/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Pool_IpCadastroModel pModel)
        {
            return base.DeleteBase<Pool_IpCadastroModel, Int64>(pModel);
        }
    }
}