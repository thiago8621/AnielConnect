using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class EquipeController : BaseController
    {

        public EquipeController() : base("Equipe", 
                                         CONSTANTES.URLPortalInterno,
                                         String.Empty,
                                         "api/Equipeapi/FindById_HabilitarServico?equipe={0}",
                                         "api/Equipeapi/Gravar",
                                         "api/Equipeapi/Excluir",
                                         String.Empty)
        {

        }

        // GET: Equipe
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipeDisponivel()
        {
            return View();
        }

        public ActionResult LogEquipeLocalidade()
        {
            return View();
        }

        public ActionResult LogEquipeNatureza()
        {
            return View();
        }

        public async Task<ActionResult> EquipeStatus(String Equipe)
        {
            var retorno = await new EquipeApiController().GetEquipeStatus(Equipe);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

        public ActionResult Adicionar_Servico(String Equipe, String Equipes)
        {

            if (!String.IsNullOrEmpty(Equipe))
            {
                return base.CadastroBase<Habilitar_ServicoCadastroModel, String>(Equipe);
            }
            else
            {

                var retorno = new Habilitar_ServicoCadastroModel();
                retorno.Id = "0";
                retorno.Ids = Equipes;

                FillListasView(retorno);
                return View(retorno);
                
            }
        }

        public ActionResult Adicionar_Area(String Equipe, String Equipes)
        {
           
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();

            if (!String.IsNullOrEmpty(Equipe))
            {
                GetApiRetorno<Habilitar_AreaCadastroModel> retorno = new EquipeApiController().FindById_HabilitarArea(Equipe).Result;
                FillListasView(retorno);
                return View(retorno.Objeto);
            }
            else
            {

                var retorno = new Habilitar_AreaCadastroModel();
                retorno.Id = "0";
                retorno.Ids = Equipes;

                FillListasView(retorno);
                return View(retorno);

            }


        }

        [HttpPost]
        public async Task<ActionResult> Adicionar_Servico(Habilitar_ServicoCadastroModel valor)
        {

            var retorno = await new EquipeApiController().Gravar(new ParamWebApi<Habilitar_ServicoCadastroModel>{
                Objeto = valor,
                Usuario = User.Identity.GetUserId(),
                //Token = GetTokenByUser(User.Identity.Name)
            });

            FillListasView(valor);
            FillListasView(retorno);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
                return View(valor);
            }
            else
            {
                ViewBag.Sucesso = true;
                return View(valor);
            }
            


        }

        [HttpPost]
        public async Task<ActionResult> Adicionar_Area(Habilitar_AreaCadastroModel valor)
        {
            
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();

            var retorno = await new EquipeApiController().GravarArea(new ParamWebApi<Habilitar_AreaCadastroModel>
            {
                Objeto = valor,
                Usuario = User.Identity.GetUserId(),
                //Token = GetTokenByUser(User.Identity.Name)
            });

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
                return View(valor);
            }
            else
            {
                ViewBag.Sucesso = true;
                return View(valor);
            }

        }

        public ActionResult Habilitar_Servicos()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        public ActionResult Habilitar_Areas()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        //[HttpGet()]
        //public ActionResult Cadastro(Int64? id)
        //{
        //    return base.CadastroBase<EquipeCadastroModel, Int64?>(id);
        //}

        // POST: Equipe/Cadastro/5
        //[HttpPost]
        //public ActionResult Cadastro(EquipeCadastroModel pModel)
        //{
        //    ActionResult retorno = base.CadastroBase<EquipeCadastroModel, Int64>(pModel);

        //    return retorno;
        //}

        protected override void FillListasView<T>(T pModel)
        {
            var _retornoScripts = new Script_AtendimentoApiController().GetList().Result;

            if (!_retornoScripts.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoScripts.Mensagem, _retornoScripts.InnerException);

            ViewBag.Scripts = _retornoScripts.Objeto;

            ViewBag.Natureza = new Natureza_ProducaoApiController().GetList().Result.Objeto;
        }

        // GET: Equipe/Delete/5
        //public ActionResult Delete(Int64? id)
        //{
        //    if (!id.HasValue)
        //        id = 0;

        //    return base.DeleteBase<EquipeCadastroModel, Int64>(id.Value);
        //}

        // POST: Equipe/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(EquipeCadastroModel pModel)
        //{
        //    return base.DeleteBase<EquipeCadastroModel, Int64>(pModel);
        //}
    }
}