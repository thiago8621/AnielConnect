using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Socket.ChatHub;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class BairroController : BaseController
    {

        public BairroController() : base("Bairro",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Bairroapi/FindById?id={0}",
                                                   "api/Bairroapi/Gravar",
                                                   "api/Bairroapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Bairro
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {

            ViewResult retorno = (ViewResult)base.CadastroBase<BairroCadastroModel, Int64?>(id);

            return retorno;

        }

        // POST: Bairro/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(BairroCadastroModel pModel)
        {
            ActionResult retorno = base.CadastroBase<BairroCadastroModel, Int64>(pModel);

            return retorno;
        }

        // GET: Bairro/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<BairroCadastroModel, Int64>(id.Value);
        }

        // POST: Bairro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(BairroCadastroModel pModel)
        {
            return base.DeleteBase<BairroCadastroModel, Int64>(pModel);
        }
        protected override void FillListasView<T>(T pModel)
        {
        }

        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<BairroImportacao> modelo = new ExcelServiceLog<BairroImportacao>
            {
                Items = new List<BairroImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Planilha)
        {

            ExcelServiceLog<BairroImportacao> modelo = new ExcelServiceLog<BairroImportacao>
            {
                Items = new List<BairroImportacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(BairroImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<BairroImportacao>(dt);


                        using (BairroApiController Bairro_api = new BairroApiController())
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
                                        modelo.Log.AddRange(await Bairro_api.ImportacaoModelo(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem { Chave = item.Descricao_Bairro, Erro = ex.Message });
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