using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ComboController : BaseController
    {
        public ComboController()
            : base("Combo",
                   CONSTANTES.URLPortalInterno,
                   String.Empty,
                   "api/ComboApi/FindById?id={0}",
                   "api/ComboApi/Gravar",
                   String.Empty)
        { }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(long? id)
        {
            return CadastroBase<ComboCadastroModel, long?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(ComboCadastroModel model)
        {
            return CadastroBase<ComboCadastroModel, long?>(model);
        }

        /// <summary>
        ///     Exibe os detalhes do combo na tela de venda
        /// </summary>
        /// <param name="Id">ID do combo que será detalhado</param>
        public async Task<ActionResult> Detalhes(long Id)
        {
            using (var api = new ComboApiController())
            {
                var retorno = await api.GetListDetalhes(Id);
                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                }

                return View(retorno.Objeto);
            }
        }
    }
}