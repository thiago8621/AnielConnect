using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ImportacaoOSController : BaseController
    {

        public ImportacaoOSController() : base("ImportacaoOS",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "",
                                                   "",
                                                   "",
                                                   String.Empty)
        {

        }

        public ActionResult ConfigurarCamposImportacaoOs()
        {
            return View();
        }
        public ActionResult ImportarOs(TipoServicoImpVinculoRequest tipoServVinc)
        {
            tipoServVinc.FileField = (HttpPostedFileBase)TempData["Planilha"];
            return View(tipoServVinc);
        }

        [HttpPost]
        public async Task<ActionResult> EnviarPlanilha(TipoServicoImpVinculoRequest tipoServVinc)
        {
            ExcelServiceLog<Dictionary<String, Object>> modelo = new ExcelServiceLog<Dictionary<String, Object>>
            {
                Items = new List<Dictionary<String, Object>>(),
                Log = new List<ExcelServiceLogItem>(),
                Erros = new List<string>(),
                Sucessos = new List<string>()
            };

            List<string> TipoServicoNaoEncontrado = new List<string>();
            List<DocumentoProducao_ImportacaoOS> documentosImportar = new List<DocumentoProducao_ImportacaoOS>();
            List<DocumentoProducao_ImportacaoOS> documentos = new List<DocumentoProducao_ImportacaoOS>();
            try
            {
                String tipoOS = Request.Form.Get("importacaoTipo");
                using (ImportacaoOsApiController apiImport = new ImportacaoOsApiController())
                {
                    foreach (var item in tipoServVinc.TiposNaoEncontrado)
                    {
                        Tipo_Servico tipo = new Tipo_Servico();
                        tipo.descricaoTipoServ = item.Descricao;
                        await apiImport.OperacaoTipoServico(item, tipo, Request.Form.Get("contrato"));
                    }

                    // verifica se as ordens de servico ja estao carregadas na pagina
                    if (String.IsNullOrEmpty(tipoServVinc.DocumentosJson))
                    {
                        if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                        {
                            AddModelError("Informe Planilha a ser Importada");
                            return RedirectToAction("ImportarOs", "ImportacaoOS");
                        }

                        if (ModelState.IsValid)
                        {
                            if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                            {
                                DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false);

                                List<Dictionary<String, Object>> dadosPlanilha = ExcelService.CastDataTableToDictionary(dt);

                                using (ISession session = HibernateUtil.GetSessionT5())
                                {

                                    List<ImportacaoOsCampo> config = new List<ImportacaoOsCampo>();
                                    if (tipoOS == "C")
                                    {
                                        config = await apiImport.CarregaConfiguracaoLeitura(session, Request.Form.Get("projeto"), Request.Form.Get("contrato"));
                                    }
                                    else
                                    {
                                        config = await apiImport.CarregaConfiguracaoLeituraManut(session, Request.Form.Get("projeto"), Request.Form.Get("contrato"));
                                    }
                                    

                                    foreach (var campoPlanilha in dadosPlanilha)
                                    {
                                        foreach (var configuracao in config)
                                        {
                                            if (String.IsNullOrEmpty(configuracao.CampoPlanilha))
                                                continue;

                                            if (!campoPlanilha.ContainsKey(configuracao.CampoPlanilha.ToUpper()))
                                            {
                                                throw new Exception("Importação não pode ser realizada pois a planilha está " +
                                                                    "incompatível com as configurações definidas para o contrato e " +
                                                                    "projeto informados.Favor verificar, corrigir e tentar novamente! " +
                                                                   $"Campo nao encontrado na planilha : {configuracao.CampoPlanilha}");
                                            }
                                        }
                                    }

                                    documentos = await GetOSsImportacao(dadosPlanilha, config, session, Request.Form.Get("contrato"));

                                    if (config.Any(p=> p.Importar == true && p.CampoAniel== "COD_TIPO_SERV_PRINC") &&
                                        documentos.Any(p => String.IsNullOrEmpty(p.COD_TIPO_SERV_PRINC)))
                                    {
                                        throw new Exception(@"Importação não pode ser realizada pois o campo de Código do Tipo de Serviço foi configurado
                                                                        mas não foi encontrado na base!");
                                    }
                                }

                            }

                        }
                    }
                    else
                    {
                        documentos = JsonConvert.DeserializeObject<List<DocumentoProducao_ImportacaoOS>>(tipoServVinc.DocumentosJson);
                    }
                    
                    foreach (var item in documentos)
                    {
                        Tipo_Servico tipoServ = new Tipo_Servico();                        
                        tipoServ.descricaoTipoServ = item.COD_TIPO_SERV;
                        if (TipoServicoNaoEncontrado.Find(p => p == tipoServ.descricaoTipoServ) == null)
                        {
                            Tipo_Servico rtipoServicoNovo = await apiImport.VerificaTipoServico(tipoServ, Request.Form.Get("contrato"));
                            if (rtipoServicoNovo.codTipoServ == 0)
                            {
                                TipoServicoNaoEncontrado.Add($"{item.COD_TIPO_SERV}");
                            }
                        }
                    }

                    if (TipoServicoNaoEncontrado.Count > 0)
                    {
                        TempData["UnidadeCusto"] = Request.Form.Get("unidadeCusto");
                        TempData["Contrato"] = Request.Form.Get("contrato");
                        TempData["Projeto"] = Request.Form.Get("projeto");
                        TempData["Planilha"] = tipoServVinc.FileField;
                        TempData["DocumentosJson"] = JsonConvert.SerializeObject(documentos);
                    }

                    foreach (DocumentoProducao_ImportacaoOS documento in documentos)
                    {
                        documentosImportar.Add(documento);
                    }

                    TempData["Sucessos"] = modelo.Sucessos;
                    TempData["Erros"] = modelo.Erros;
                    TempData["TipoServicoNaoEncontrado"] = TipoServicoNaoEncontrado;
                    TempData["TipoOs"] = tipoOS;

                    TempData.Keep();

                    if (TipoServicoNaoEncontrado.Count == 0)
                    {
                        await apiImport.EnviarPlanilha(modelo, documentosImportar, Request.Form.Get("contrato"), Request.Form.Get("projeto"), Request.Form.Get("unidadeCusto"),Request.Form.Get("importacaoTipo"));
                    }
                    else
                    {
                        return RedirectToAction("ImportarOs", "ImportacaoOS");
                    }

                    return RedirectToAction("ImportarOs", "ImportacaoOS");


                }
            }
            catch (Exception ex)
            {
                var erros = new List<string>();
                erros.Add(ex.Message);

                foreach (string erro in erros)
                {
                    await new ImportacaoOsApiController().GravaLogImportacao(
                            new LogImportacao
                            {
                                Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                                Hora = DateTime.Now.ToString("HH:mm"),
                                Erro = erro,
                                Usuario = User.Identity.GetUserId()
                            });
                }

                TempData["Erros"] = erros;
                TempData.Keep();
                return RedirectToAction("ImportarOs", "ImportacaoOS");
            }
        }
        protected async Task<List<DocumentoProducao_ImportacaoOS>> GetOSsImportacao(List<Dictionary<String, Object>> dadosPlanilha, List<ImportacaoOsCampo> config, ISession session, string contrato)
        {
            try
            {
                List<DocumentoProducao_ImportacaoOS> retorno = new List<DocumentoProducao_ImportacaoOS>();


                foreach (var row in dadosPlanilha)
                {
                    DocumentoProducao_ImportacaoOS obj = new DocumentoProducao_ImportacaoOS();

                    Int16? CodTipoServLinha = null;
                    using (var banco = new Conexao<Tipo_Servico_EquipeAniel>(session))
                    using (var segmentoRep = new Conexao<Segmento>(session))
                    using (var turnoRep = new Conexao<Turno_ServicoAniel>(session))
                    using (var subTipoRep = new Conexao<Sub_Tipo_ServicoAniel>(session))
                    using (var equipeRep = new Conexao<EquipeAniel>(session))
                    using (var tipoImovelRep = new Conexao<Tipo_ImovelAniel>(session))
                    {
                        foreach (PropertyInfo campo in obj.GetType().GetProperties())
                        {
                            string campoPlanilha = GetImportacaoOsCampo(config, campo.Name);

                            if (!String.IsNullOrEmpty(campoPlanilha) && row.ContainsKey(campoPlanilha.ToUpper()))
                            {
                                switch (campo.Name)
                                {
                                    case "COD_TIPO_SERV_PRINC": {
                                            if (row[campoPlanilha] != null) 
                                            {
                                                var tipoServ = await banco.select(x => x.Cod_Tipo_Serv == Convert.ToInt32(row[campoPlanilha]));
                                                if (tipoServ.Count == 0) 
                                                { continue; }
                                                else
                                                {
                                                    CodTipoServLinha = tipoServ[0].Cod_Tipo_Serv;
                                                    campo.SetValue(obj, row[campoPlanilha].ToString());
                                                    obj.COD_TIPO_SERV = tipoServ[0].Descricao;
                                                }
                                            }
                                            continue;
                                        }
                                    case "COD_TIPO_SERV":
                                        {
                                            if (String.IsNullOrEmpty(row[campoPlanilha.ToUpper()].ToString()))
                                                continue;

                                            var tipoServ = await banco.select(x => x.Descricao == row[campoPlanilha.ToUpper()].ToString() && x.CodCt == contrato);

                                            if (tipoServ.Count > 0)
                                            {
                                                //campo.SetValue(obj, tipoServ[0].Cod_Tipo_Serv.ToString());
                                                CodTipoServLinha = tipoServ[0].Cod_Tipo_Serv;
                                            }

                                            //validacao esta na api
                                            campo.SetValue(obj, row[campoPlanilha.ToUpper()].ToString());

                                            continue;
                                        }

                                    case "SEGMENTO":
                                        {
                                            if (String.IsNullOrEmpty(row[campoPlanilha.ToUpper()].ToString()))
                                                continue;
                                            var segmentos = await segmentoRep.select(x => x.Descricao == row[campoPlanilha.ToUpper()].ToString());

                                            if (segmentos.Count == 0)
                                            {
                                                var segmentoNovo = new Segmento { Descricao = row[campoPlanilha.ToUpper()].ToString() };
                                                await segmentoRep.Save(segmentoNovo);
                                                campo.SetValue(obj, segmentoNovo.Codigo);
                                            }
                                            else
                                            {
                                                campo.SetValue(obj, segmentos[0].Codigo);
                                            }

                                            continue;
                                        }

                                    case "COD_TURNO_SERV"://período
                                        {
                                            if (String.IsNullOrEmpty(row[campoPlanilha.ToUpper()].ToString()))
                                                continue;
                                            var turnos = await turnoRep.select(x => x.Descricao == row[campoPlanilha.ToUpper()].ToString());

                                            if (turnos.Count > 0)
                                            {
                                                campo.SetValue(obj, turnos[0].Id.ToString());
                                            }

                                            continue;
                                        }

                                    case "EQUIPE":
                                        {
                                            if (String.IsNullOrEmpty(row[campoPlanilha.ToUpper()].ToString()))
                                                continue;

                                            List<EquipeAniel> equipes = new List<EquipeAniel>();
                                            var nome = StringService.Truncate(row[campoPlanilha.ToUpper()].ToString(), 50);
                                            if (row[campoPlanilha.ToUpper()].ToString().Length <= 10)
                                            {
                                                equipes = (await equipeRep.select(x => x.Matricula == row[campoPlanilha.ToUpper()].ToString() || x.Nome == nome)).ToList();
                                            }
                                            else
                                            {
                                                equipes = (await equipeRep.select(x => x.Nome == row[campoPlanilha.ToUpper()].ToString())).ToList();
                                            }

                                            if (equipes.Count > 0)
                                            {
                                                campo.SetValue(obj, equipes[0].Matricula.ToString());
                                            }

                                            continue;
                                        }

                                    case "PARPRIM_PARA":
                                        {
                                            if (String.IsNullOrEmpty(row[campoPlanilha.ToUpper()].ToString()))
                                                continue;

                                            var imoveis = await tipoImovelRep.select(x => x.Descricao == row[campoPlanilha.ToUpper()].ToString());

                                            if (imoveis.Count > 0)
                                            {
                                                campo.SetValue(obj, imoveis[0].Cod_Tipo_Imovel);
                                            }

                                            continue;
                                        }

                                }

                                campo.SetValue(obj, Convert.ToString(row[campoPlanilha.ToUpper()]));
                            }
                        }

                        retorno.Add(obj);
                    }
                }


                return retorno;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        protected string GetImportacaoOsCampo(List<ImportacaoOsCampo> config, String CampoAniel)
        {
            ImportacaoOsCampo retorno = config.Find(p => p.CampoAniel == CampoAniel);

            if (retorno != null)
                return retorno.CampoPlanilha;

            return "";

        }


        public ActionResult Log()
        {
            return View();
        }
    }
}