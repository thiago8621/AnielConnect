using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class AtendimentoCamposController : BaseController
    {

        public AtendimentoCamposController() : base("AtendimentoCampos",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/AtendimentoCamposApi/FindById?id={0}",
                                                   "api/AtendimentoCamposApi/Gravar",
                                                   "api/AtendimentoCamposApi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: AtendimentoCampos
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }


        [HttpGet()]
        public ActionResult Cadastro(Int64? id, bool sucessoGravacao = false)
        {
            ViewBag.Areas = Areas;
            ViewBag.Sucesso = sucessoGravacao;
            return base.CadastroBase<AtendimentoCamposCadastroModels, Int64?>(id);
        } 
        
        [HttpGet()]
        public ActionResult HistoricoAlteracoesCampos()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        // POST: Atendimento_Acao/Cadastro/5
        [HttpPost]
        public async Task<ActionResult> Cadastro(AtendimentoCamposCadastroModels pModel)
        {
            ViewBag.Areas = Areas;
            
            try
            {
                if (ModelState.IsValid)
                {
                    //PostApiRetorno<Int64> retorno = PostByUrl<VendaDadosGeraisViewModel, Int64>(valor, UrlPortal, UrlPost);
                    ParamWebApi<AtendimentoCamposCadastroModels> modelWebPai = new ParamWebApi<AtendimentoCamposCadastroModels>
                    {
                        Usuario = User.Identity.GetUserId(),
                        NomeUsuario = User.Identity.GetUserName(),
                        Objeto = pModel
                    };

                    PostApiRetorno<Int64> retorno = await new AtendimentoCamposApiController().Gravar(modelWebPai);

                    if (retorno.Ok) //Edição
                    {
                        ViewBag.Sucesso = retorno.Ok;
                        return RedirectToAction("Cadastro", new { id = retorno.IdObjeto, sucessoGravacao = true });
                    }
                    else
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(pModel);
        }

    }
}