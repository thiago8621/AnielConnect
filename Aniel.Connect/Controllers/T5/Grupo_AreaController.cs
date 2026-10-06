using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using Resources;
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
    public class Grupo_AreaController : BaseController
    {

        public Grupo_AreaController() : base("Grupo_Area", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/GrupoAreaapi/FindById?Id={0}",
                                                   "api/GrupoAreaapi/Gravar",
                                                   "api/GrupoAreaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Grupo_Area

        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        // GET: Grupo_Area
        [HttpGet()]
        public ActionResult Cadastro(String id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<GrupoArea_CadastroModel, String>(id);
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


        [HttpPost()]
        public ActionResult Cadastro(GrupoArea_CadastroModel value)
        {

            ActionResult retorno = base.CadastroBase<GrupoArea_CadastroModel, String>(value);

            return retorno;
        }

        protected override void FillListasView<T>(T pModel)
        {
        }

        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Grupo_Area_Importacao> modelo = new ExcelServiceLog<Grupo_Area_Importacao>
            {
                Items = new List<Grupo_Area_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Arquivo)
        {


            ExcelServiceLog<Grupo_Area_Importacao> modelo = new ExcelServiceLog<Grupo_Area_Importacao>
            {
                Items = new List<Grupo_Area_Importacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Grupo_Area_Importacao)));
                        modelo = ExcelService.CastDataTableImportacao<Grupo_Area_Importacao>(dt);


                        using (Grupo_AreaAnielApiController grupoApi = new Grupo_AreaAnielApiController())
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
                                        modelo.Log.AddRange(await grupoApi.Importacao(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem
                                        {
                                            Chave = item.Codigo,
                                            Erro = $"Erro ao processar com Código: {item.Codigo}"
                                        });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }
                        }
                    }

                    modelo.Items.AddRange(modelo.Items);
                    modelo.Log.AddRange(modelo.Log);

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