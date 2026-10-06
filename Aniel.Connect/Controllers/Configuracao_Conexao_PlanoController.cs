using Aniel.Connect.Core;
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
    public class Configuracao_Conexao_PlanoController : BaseController
    {
        public Configuracao_Conexao_PlanoController() : base("Configuracao_Conexao_Plano",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/PlanoProdutoapi/FindConfigConexaoById?id={0}",
                                                   "api/PlanoProdutoapi/GravarConfigConexao",
                                                   String.Empty)
        {

        }

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            return base.CadastroBase<ConfigConexaoPlano, Int64?>(id);
        }
        
        [HttpPost]
        public ActionResult Cadastro(ConfigConexaoPlano pModel)
        {
            ActionResult retorno = base.CadastroBase<ConfigConexaoPlano, Int64>(pModel);

            return retorno;
        }
    }
}