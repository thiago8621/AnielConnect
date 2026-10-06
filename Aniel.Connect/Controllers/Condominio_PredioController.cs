using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Condominio_PredioController : BaseController
    {

        public Condominio_PredioController() : base("Condominio_Predio",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/CondominioPredioapi/FindById?id={0}",
                                                   "api/CondominioPredioapi/Gravar",
                                                   "api/CondominioPredioapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Condominio_Predio
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<Condominio_PredioCadastroModel, Int64?>(id);
            FillListasView(retorno);

            return retorno;

        }

        // POST: Condominio_Predio/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Condominio_PredioCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<Condominio_PredioCadastroModel, Int64>(pModel);
            FillListasView(retorno);
            return retorno;
        }

        // GET: Condominio_Predio/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Condominio_PredioCadastroModel, Int64>(id.Value);
        }

        // POST: Condominio_Predio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Condominio_PredioCadastroModel pModel)
        {
            return base.DeleteBase<Condominio_PredioCadastroModel, Int64>(pModel);
        }
        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Cadastro_Endereco = false;
            using (ISession session = HibernateUtil.GetSession())
            {
                var repositorio = new ConfiguracoesGeraisRepository(session);
                ViewBag.Cadastro_Endereco = new ConfiguracoesGeraisBusiness(repositorio).GetConfiguracoesGerais().Utiliza_Cadastro_Endereco;
                ViewBag.Tipos_Imovel = new Tipo_ImovelAnielApiController().GetList().Result.Objeto;
                ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;
                ViewBag.ComboTmp = new List<ComboObj>();
            }
        }
      
        [HttpGet()]
        public ActionResult Importacao()
        {
            ViewBag.Utiliza_Cadastro_Endereco = false;
            using (ISession session = HibernateUtil.GetSession())
            {
                var repositorio = new ConfiguracoesGeraisRepository(session);
                ViewBag.Utiliza_Cadastro_Endereco = new ConfiguracoesGeraisBusiness(repositorio).GetConfiguracoesGerais().Utiliza_Cadastro_Endereco;
            }

            ExcelServiceLog<Condominio_PredioImportacao> modelo = new ExcelServiceLog<Condominio_PredioImportacao>
            {
                Items = new List<Condominio_PredioImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Planilha)
        {

            ExcelServiceLog<Condominio_PredioImportacao> modelo = new ExcelServiceLog<Condominio_PredioImportacao>
            {
                Items = new List<Condominio_PredioImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ViewBag.Utiliza_Cadastro_Endereco = false;
            using (ISession session = HibernateUtil.GetSession())
            {
                var repositorio = new ConfiguracoesGeraisRepository(session);
                ViewBag.Utiliza_Cadastro_Endereco = new ConfiguracoesGeraisBusiness(repositorio).GetConfiguracoesGerais().Utiliza_Cadastro_Endereco;
            }

            try
            {
                if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                {
                    AddModelError("Informe Planilha a ser Importada");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Condominio_PredioImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<Condominio_PredioImportacao>(dt);


                        using (Condominio_PredioApiController CondominioPredio_api = new Condominio_PredioApiController())
                        {
                            if (dt.Rows.Count == 0)
                            {
                                throw new Exception("A planilha carregada está vazia!");
                            }

                            if (dt.Rows.Count > 0)
                            {
                                foreach (var item in modelo.Items)
                                {
                                    try
                                    {
                                        modelo.Log.AddRange(await CondominioPredio_api.ImportacaoModelo(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem { Chave = item.Descricao_Condominio_Predio, Erro = ex.Message });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }

                            }
                        }
                    }


                    ViewBag.Sucesso = modelo.Log.Count == 0 ? "S" : "N";
                    return View(modelo);

                }
                else
                {
                    return View(modelo);
                }

            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(modelo);
            }
        }
    }
}