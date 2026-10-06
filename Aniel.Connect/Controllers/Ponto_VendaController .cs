using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Ponto_VendaController : BaseController
    {

        public Ponto_VendaController() : base("Ponto_Venda",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ponto_vendaapi/FindById?id={0}",
                                                   "api/ponto_vendaapi/Gravar",
                                                   "api/ponto_vendaapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Ponto_Venda
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;

            FillListasView();

            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            FillListasView();
                       
            return base.CadastroBase<Ponto_VendaCadastroModel, Int64?>(id);
        }

        // POST: Ponto_Venda/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(Ponto_VendaCadastroModel Model)
        {
            //PostApiRetorno<Int64> retornoPost = await base.PostByUrl<Ponto_VendaCadastroModel, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Ponto_VendaApi/Gravar");

            //return RedirectToAction("Index");

            FillListasView();

            return base.CadastroBase<Ponto_VendaCadastroModel, Int64>(Model);

        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>();

            ViewBag.ComboTmp = new List<ComboObj>();

                       
        }

        protected void FillListasView()
        {
            ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>();

            ViewBag.ComboTmp = new List<ComboObj>();
            
            var _retornoSupervisor = new FuncionarioApiController().GetListSupervisor().Result;

            if (!_retornoSupervisor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoSupervisor.Mensagem, _retornoSupervisor.InnerException);

            ViewBag.Supervisor = _retornoSupervisor.Objeto;


            var _retornoVendedor = new FuncionarioApiController().GetListVendedor().Result;

            if (!_retornoVendedor.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoVendedor.Mensagem, _retornoVendedor.InnerException);

            ViewBag.Vendedor = _retornoVendedor.Objeto;

        }

        // GET: Ponto_Venda/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<Ponto_VendaCadastroModel, Int64>(id.Value);
        }

        // POST: Ponto_Venda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Ponto_VendaCadastroModel pModel)
        {
            return base.DeleteBase<Ponto_VendaCadastroModel, Int64>(pModel);
        }


        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Ponto_VendaImportacao> modelo = new ExcelServiceLog<Ponto_VendaImportacao>
            {
                Items = new List<Ponto_VendaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Arquivo)
        {

            ExcelServiceLog<Ponto_VendaImportacao> modelo = new ExcelServiceLog<Ponto_VendaImportacao>
            {
                Items = new List<Ponto_VendaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<VendedorImportacao> modeloVendedor = new ExcelServiceLog<VendedorImportacao>
            {
                Items = new List<VendedorImportacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Ponto_VendaImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<Ponto_VendaImportacao>(dt);

                        DataTable dtVendedor = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(VendedorImportacao)));
                        modeloVendedor = ExcelService.CastDataTableImportacao<VendedorImportacao>(dtVendedor);

                        using (Ponto_VendaApiController Ponto_Venda_api = new Ponto_VendaApiController())
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
                                        modelo.Log.AddRange(await Ponto_Venda_api.ImportacaoModelo(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modelo.Log.Add(new ExcelServiceLogItem { Chave = item.Descricao, Erro = ex.Message });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }

                                foreach (var item in modeloVendedor.Items)
                                {
                                    try
                                    {
                                        modeloVendedor.Log.AddRange(await Ponto_Venda_api.ImportacaoModeloVendedor(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modeloVendedor.Log.Add(new ExcelServiceLogItem { Chave = item.MatriculaVendedor, Erro = ex.Message });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }
                        }
                    }

                    modelo.Log.AddRange(modeloVendedor.Log);
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