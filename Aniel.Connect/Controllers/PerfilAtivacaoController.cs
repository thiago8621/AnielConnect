using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
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
    public class PerfilAtivacaoController : BaseController
    {

        public PerfilAtivacaoController() : base("PerfilAtivacao",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/PerfilAtivacaoApi/FindById?id={0}",
                                                   "api/PerfilAtivacaoApi/Gravar",
                                                   "api/PerfilAtivacaoApi/Excluir",
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
            return base.CadastroBase<PerfilAtivacaoCadastroModel, Int64?>(id);
        }

        [HttpPost]
        public ActionResult Cadastro(PerfilAtivacaoCadastroModel pModel)
        {
            return base.CadastroBase<PerfilAtivacaoCadastroModel, Int64>(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
        }

        [HttpGet()]
        public ActionResult ImportacaoVinculosPerfilAtivacao()
        {
            ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao> modelo = new ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao>
            {
                Items = new List<Perfil_Ativacao_Vinculo_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> ImportacaoVinculosPerfilAtivacao(String Arquivo)
        {
            ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao> modeloFisico = new ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao>
            {
                Items = new List<Perfil_Ativacao_Vinculo_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao> modelo = new ExcelServiceLog<Perfil_Ativacao_Vinculo_Importacao>
            {
                Items = new List<Perfil_Ativacao_Vinculo_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            try
            {
                ViewBag.Erro = "N";
                if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                {
                    AddModelError("Informe Planilha a ser Importada");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Perfil_Ativacao_Vinculo_Importacao)));
                        modeloFisico = ExcelService.CastDataTableImportacao<Perfil_Ativacao_Vinculo_Importacao>(dt);

                        using (PerfilAtivacaoApiController perfil_api = new PerfilAtivacaoApiController())
                        {
                            if (dt.Rows.Count == 0)
                            {
                                throw new Exception("A planilha carregada está vazia!");
                            }

                            if (dt.Rows.Count > 0)
                            {
                                if (modeloFisico.Log.Count() > 0)
                                {
                                    ViewBag.Erro = "S";
                                    try
                                    {
                                        var errosCampo = modeloFisico.Log.Select(p => new { p.Chave, p.Erro }).ToList();

                                        foreach (var erro in errosCampo)
                                        {
                                            await perfil_api.GravaLogImportacao(
                                            new ModelLog.nhibernate.LogImportacaoVinculoPerfilAtivacao { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Codmat = erro.Chave ?? "-", Erro = erro.Erro, Usuario = User.Identity.GetUserId() });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }

                                foreach (var item in modeloFisico.Items)
                                {
                                    try
                                    {
                                        var retornoImportacao = await perfil_api.ImportacaoModelo(item);
                                        if (!retornoImportacao.Ok)
                                        {
                                            modeloFisico.Log.Add(new ExcelServiceLogItem
                                            {
                                                Chave = item.CodMat,
                                                Erro = $"Falha: Cod. Mat - {item.CodMat} | Id Olt - {item.Id_Olt} - erro: {retornoImportacao.Mensagem} {retornoImportacao.InnerException ?? String.Empty}"
                                            });

                                            ViewBag.Erro = "S";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ViewBag.Erro = "S";
                                        modeloFisico.Log.Add(new ExcelServiceLogItem
                                        {
                                            Chave = item.CodMat,
                                            Erro = $"Erro ao processar: Cod. Mat - {item.CodMat} | Id Olt - {item.Id_Olt}"
                                        });

                                        await perfil_api.GravaLogImportacao(
                                            new ModelLog.nhibernate.LogImportacaoVinculoPerfilAtivacao { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Codmat = item.CodMat, Erro = ex.Message + ex.InnerException?.Message, Usuario = User.Identity.GetUserId() });

                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }
                        }
                    }

                    modelo.Items.AddRange(modeloFisico.Items);
                    modelo.Log.AddRange(modeloFisico.Log);

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

        public ActionResult LogImportacaoVinculosPerfilAtivacao()
        {
            return View();
        }
    }
}