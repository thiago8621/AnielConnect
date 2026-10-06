using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class LocalidadeController : BaseController
    {

        public LocalidadeController() : base("Localidade", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Localidadeapi/FindById?Id={0}",
                                                   "api/Localidadeapi/Gravar",
                                                   "api/Localidadeapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Localidade

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        // GET: Localidade
        [HttpGet()]
        public ActionResult Cadastro(string id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<Localidade_CadastroModel, string>(id);
            return retorno;
        }


        [HttpPost()]
        public ActionResult Cadastro(Localidade_CadastroModel value)
        {

            ActionResult retorno = base.CadastroBase<Localidade_CadastroModel, String>(value);

            return retorno;
        }


        [HttpGet()]
        public ActionResult Historico_Alteracao(Int64? id)
        {
            ViewBag.Chave = "Id";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }

        protected override void FillListasView<T>(T pModel)
        {
           


            
        }


        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Localidade_Importacao> modelo = new ExcelServiceLog<Localidade_Importacao>
            {
                Items = new List<Localidade_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Arquivo)
        {


            ExcelServiceLog<Localidade_Importacao> modelo = new ExcelServiceLog<Localidade_Importacao>
            {
                Items = new List<Localidade_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Localidade_Importacao)));
                        modelo = ExcelService.CastDataTableImportacao<Localidade_Importacao>(dt);


                        using (LocalidadeApiController localidadeApi = new LocalidadeApiController())
                        {
                            if (dt.Rows.Count == 0 && dt.Rows.Count == 0)
                            {
                                throw new Exception("A planilha carregada está vazia!");
                            }

                            if (dt.Rows.Count > 0)
                            {
                                foreach (var item in modelo.Items)
                                {
                                    try
                                    {
                                        modelo.Log.AddRange(await localidadeApi.Importacao(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem
                                        {
                                            Chave = item.Codigo,
                                            Erro = $"Erro ao processar com Código: {item.Codigo}. {ex.Message}."
                                        });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }
                        }
                    }

                    modelo.Items.AddRange(modelo.Items);
                    /*modelo.Log.AddRange(modelo.Log);*/

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