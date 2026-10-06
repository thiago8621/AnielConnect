using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Web.Mvc;
using Aniel.Connect.ControllersWebApi;
using System.Threading.Tasks;
using Sinapse.Util;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class DidController : BaseController
    {
        public DidController() : base("Did",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Didapi/FindById?id={0}",
                                                   "api/DidApi/Gravar",
                                                   String.Empty)
        {

        }


        /// <summary>Index Action </summary> 
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            ViewBag.Api = new DidApiController().Atualizar_Did_Automatico().Result;
            return View();
        }

        /// <summary>Cadastro Action </summary>
        /// <param name="id"></param>
        public ActionResult Cadastro(Int64? id)
        {
            ViewBag.Areas = Areas;
            ViewResult retorno = (ViewResult)base.CadastroBase<DidCadastroModel, Int64?>(id);
            return retorno;
        }

        /// <summary> Post on Cadastro's Action</summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult Cadastro(DidCadastroModel model)
        {
            ActionResult retorno = base.CadastroBase<DidCadastroModel, Int64>(model);
            return retorno;
        }

        /// <summary>Cadastro_Intervalo Action </summary>
        /// <param name="id"></param>
        [HttpGet()]
        public ActionResult Cadastro_Intervalo(Int64? id)
        {
            ViewBag.Areas = Areas;


            ViewResult retorno = (ViewResult)CadastroBase<Did_CadastroIntervaloModel, Int64?>(id);
            return retorno;
        }

        /// <summary> Post on Cadastro_Intervalo's Action</summary>
        /// <param name="model"></param>
        [HttpPost()]
        public async Task<ActionResult> Cadastro_Intervalo(Did_CadastroIntervaloModel model)
        {
            var request = new ParamWebApi<Did_CadastroIntervaloModel> { Objeto = model };
            using (DidApiController api = new DidApiController())
            {
                var retorno = await api.Gravar_Intervalo(request);
                if (retorno.Ok == false)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                    //return View(model);
                }
            }

            return View(model);
        }

        /// <summary> Did_Planilha Action </summary>
        [HttpGet()]
        public ActionResult Did_Planilha()
        {
            ViewBag.Areas = Areas;
            ExcelServiceLog<Did_PlanilhaImportacao> modelo = new ExcelServiceLog<Did_PlanilhaImportacao>
            {
                Erros = new List<string>(),
                Items = new List<Did_PlanilhaImportacao>(),
                Sucessos = new List<string>(),
                Log = new List<ExcelServiceLogItem>()
            };
            return View(modelo);
        }

        /// <summary>  Post on Did_Planilha's Action </summary>
        /// <param name="Planilha"></param>
        //Erro: Coluna não encontrada Indice: -1 Resolver
        [HttpPost()]
        public ActionResult Did_Planilha(String Planilha)
        {
            ViewBag.Areas = Areas;
            ExcelServiceLog<Did_PlanilhaImportacao> modelo = new ExcelServiceLog<Did_PlanilhaImportacao>
            {
                Erros = new List<string>(),
                Items = new List<Did_PlanilhaImportacao>(),
                Sucessos = new List<string>()
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
                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Did_PlanilhaImportacao)));
                        modelo = ExcelService.CastDataTableImportacao<Did_PlanilhaImportacao>(dt);
                    }
                    using (DidApiController api = new DidApiController())
                    {
                        foreach (var item in modelo.Items)
                        {
                            try
                            {
                                modelo.Log.AddRange(api.Importar_Did(item, new Model.nhibernate.Did()).Result);
                            }
                            catch (Exception ex)
                            {
                                modelo.Log.Add(new ExcelServiceLogItem
                                {
                                    Chave = item.Numero,
                                    Erro = $"Erro ao processar o prospect com Identificação de Cliente: {item.Numero}"
                                });
                                CONSTANTES.log4Net.Error(ex.Message, ex);
                            }
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                _ = erro.Message;
                return View();
            }
            return View(modelo);
        }

        /// <summary> </summary>
        [HttpGet]
        public ActionResult Relatorio_Situacao_Did() => View();

        /// <summary> </summary>
        [HttpGet()]
        public ActionResult Relatorio_Historico_Chamada() => View();

    }
}