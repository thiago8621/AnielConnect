using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class LoginRadiusController : BaseController
    {

        public LoginRadiusController() : base("LoginRadius", 
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/LoginRadiusapi/FindById?Id={0}",
                                                   "api/LoginRadiusapi/Gravar",
                                                   "api/LoginRadiusapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Categoria_Atendimento
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            FillListasView();
            return View();
        }

        [HttpGet()]
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            return base.CadastroBase<LoginRadiusCadastroModel, Int64?>(id);
        }

        // POST: Categoria_Atendimento/Cadastro/5
        [HttpPost]
        public ActionResult Cadastro(LoginRadiusCadastroModel pModel)
        {
            //ActionResult retorno = base.CadastroBase<LoginRadiusCadastroModel, Int64>(pModel);
            ViewBag.Areas = Areas;
            ParamWebApi<LoginRadiusCadastroModel> paramWebApi = new ParamWebApi<LoginRadiusCadastroModel>
            {
                Usuario = User.Identity.GetUserId(),               
                Objeto = pModel
            };

            var retornoApi = new LoginRadiusApiController().Gravar(paramWebApi).Result;
            
            if(!retornoApi.Ok)
            {
                AddModelError(retornoApi.Mensagem, retornoApi.InnerException);
            }
            else 
            {
                pModel.Id = retornoApi.IdObjeto;
                if (retornoApi.Mensagens.Count > 0)
                {
                    AddModelError(retornoApi.Mensagens.First().Mensagem);
                }                
            }
            
            FillListasView(pModel);

            //return retorno;
            return View(pModel);
        }

        protected override void FillListasView<T>(T pModel)
        {
            

        }
        protected void FillListasView()
        {
            var _retornoOLT = new OLTApiController().GetListOLT().Result;

            if (!_retornoOLT.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoOLT.Mensagem, _retornoOLT.InnerException);

            ViewBag.OLT = _retornoOLT.Objeto;


        }
        // GET: Categoria_Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<LoginRadiusCadastroModel, Int64>(id.Value);
        }

        // POST: Categoria_Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(LoginRadiusCadastroModel pModel)
        {
            return base.DeleteBase<LoginRadiusCadastroModel, Int64>(pModel);
        }

        [HttpGet()]
        public ActionResult AtualizaLogins()
        {
            ExcelServiceLog<AtualizacaoLogins> modelo = new ExcelServiceLog<AtualizacaoLogins>
            {
                Items = new List<AtualizacaoLogins>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> AtualizaLogins(String Arquivo)
        {
            ExcelServiceLog<AtualizacaoLogins> modeloFisico = new ExcelServiceLog<AtualizacaoLogins>
            {
                Items = new List<AtualizacaoLogins>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<AtualizacaoLogins> modelo = new ExcelServiceLog<AtualizacaoLogins>
            {
                Items = new List<AtualizacaoLogins>(),
                Log = new List<ExcelServiceLogItem>()
            };

            bool isBackground = false;
            string UsuarioNotificar = User.Identity.GetUserId();
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(AtualizacaoLogins)));
                        modeloFisico = ExcelService.CastDataTableImportacao<AtualizacaoLogins>(dt);

                        using (LoginRadiusApiController login_radius_api = new LoginRadiusApiController())
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
                                            await login_radius_api.GravaLogAtualizacao(
                                            new ModelLog.nhibernate.LogAtualizacaoLogins { Login = erro.Chave, Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Detalhe = erro.Erro, Sucesso = "Não", Usuario = User.Identity.GetUserId() });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }

                                List<Task> listaProcessos = new List<Task>();
                                listaProcessos.Add(Task.Run(async () => {
                                    foreach (var item in modeloFisico.Items)
                                    {
                                        try
                                        {
                                            var retornoImportacao = await login_radius_api.AtualizacaoModelo(item);
                                            if (!retornoImportacao.Ok)
                                            {
                                                modeloFisico.Log.Add(new ExcelServiceLogItem
                                                {
                                                    Chave = item.Login,
                                                    Erro = $"Falha: {item.Login} - erro: {retornoImportacao.Mensagem} {retornoImportacao.InnerException ?? String.Empty}"
                                                });

                                                ViewBag.Erro = "S";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ViewBag.Erro = "S";
                                            modeloFisico.Log.Add(new ExcelServiceLogItem
                                            {
                                                Chave = item.Login,
                                                Erro = $"Erro ao processar login: {item.Login}"
                                            });

                                            await login_radius_api.GravaLogAtualizacao(
                                                new ModelLog.nhibernate.LogAtualizacaoLogins { Login = item.Login, Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Detalhe = ex.Message + ex.InnerException?.Message, Sucesso = "Não", Usuario = User.Identity.GetUserId() });

                                            CONSTANTES.log4Net.Error(ex.Message, ex);
                                        }
                                    }                                  

                                }));

                                    Task.WaitAll(listaProcessos.ToArray());
                                
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

        public ActionResult LogAtualizacaoLogins()
        {
            return View();
        }
    }
}