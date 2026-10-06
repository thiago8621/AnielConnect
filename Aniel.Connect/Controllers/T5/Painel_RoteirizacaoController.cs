using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Painel_RoteirizacaoController : BaseController
    {

        public Painel_RoteirizacaoController() : base("Painel_Roteirizacao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Painel_Roteirizacaoapi/FindById?id={0}",
                                                   "api/Painel_Roteirizacaoapi/Gravar",
                                                   "api/Painel_Roteirizacaoapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Timeline_Servicos
        [HttpGet()]
        public ActionResult Index()
        {
            if (Areas.Find(p => p.Cod_Area == "128") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();

            ViewBag.Usuarios = new UsuarioApiController().GetList().Result.Objeto;

            return View(new Painel_RoteirizacaoModel
            {
                Usuario = User.Identity.GetUserId(),
                Data_Log = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }

        [HttpGet()]
        public ActionResult Painel_Rotas()
        {
            ViewBag.TipoServico = new Tipo_Servico_EquipeApiController().GetList().Result.Objeto;
            ViewBag.Supervisor = new FuncionarioApiController().GetListSupervisorAniel().Result.Objeto;
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();
            return View();
        }


        public ActionResult Log_Roteirizacao()
        {

            return View();
        }

        public ActionResult Log_RoteirizacaoItem()
        {

            return View();
        }

        public ActionResult Cadastro(AlterarStatusViewModel value)
        {
            return View();
        }


        //// POST: Timeline_Servicos/Cadastro/5
        //[HttpPost]
        //public ActionResult Cadastro(Timeline_ServicosCadastroModel pModel)
        //{
        //    ActionResult retorno = base.CadastroBase<Timeline_ServicosCadastroModel, Int64>(pModel);

        //    return retorno;
        //}

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.TipoServico = new Tipo_Servico_EquipeApiController().GetList().Result.Objeto;
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();
        }

        [HttpGet()]
        public ActionResult Mapa_Equipes(string Equipe, DateTime? Data, String Nome_Equipe, String Pagina, long IdLog)
        {
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            {
                using (var repositoryLog = new Conexao<Log_RoteirizacaoAniel>(sessionLog))
                {
                    var log = repositoryLog.selectFirst(p => p.Id == IdLog).Result;
                    if (String.IsNullOrEmpty(Equipe))
                    {

                        return View(new Equipe_MapaLogItemModel() { Data_Busca = log.DataRoteirizacao.HasValue ? log.DataRoteirizacao.Value.ToString("dd/MM/yyyy") : null,
                            Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
                        });

                    }

                    var retorno = new Log_RoteirizacaoApiController().GetEquipeNoMapa(Equipe, Data, IdLog).Result;
                    

                    if (!retorno.Ok)
                    {
                        retorno.Objeto = retorno.Objeto == null ? new Equipe_MapaLogItemModel() : retorno.Objeto;
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }
                    else
                    {
                        retorno.Objeto.Data_Busca = Data != null ? Convert.ToDateTime(Data).ToString("dd/MM/yyyy") : String.Empty;
                    }

                    retorno.Objeto.Data_Busca = log.DataRoteirizacao.HasValue ? log.DataRoteirizacao.Value.ToString("dd/MM/yyyy") : null;
                    return View(retorno.Objeto);
                }
            }
        }

        //// GET: Timeline_Servicos/Delete/5
        //public ActionResult Delete(Int64? id)
        //{
        //    if (!id.HasValue)
        //        id = 0;

        //    return base.DeleteBase<Timeline_ServicosCadastroModel, Int64>(id.Value);
        //}

        //// POST: Timeline_Servicos/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(Timeline_ServicosCadastroModel pModel)
        //{
        //    return base.DeleteBase<Timeline_ServicosCadastroModel, Int64>(pModel);
        //}
    }
}