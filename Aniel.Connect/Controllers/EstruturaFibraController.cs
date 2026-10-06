using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.Ativacao;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using Sinapse.Integracao.Ativacao.Models;
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
    public class EstruturaFibraController : BaseController
    {

        public EstruturaFibraController() : base("Provisionamento", CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/ConfigurarONUApi/FindById?id={0}",
                                                   "api/ConfigurarONUApi/Gravar",
                                                   String.Empty)
        {

        }

        // GET: EstruturaFibra
        public ActionResult Index()
        {
            return View();
        }

        /*[HttpGet()]
        public ActionResult CadastrarONU(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<ConfigurarONUModel, Int64?>(id);

            return retorno;
        } */

       /* [HttpPost]
        public ActionResult ConfigurarONU(ConfigurarONUModel pModel)
        {
            ActionResult retorno = base.CadastroBase<ConfigurarONUModel, Int64>(pModel);

            return retorno;
        } */
      

        [HttpGet]
        public ActionResult AutorizarONU()
        {
            return View();
        }

        public ActionResult ListagemONU()
        {
            ViewBag.Areas = Areas;
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ConfigurarONU(Int64? IdONU)
        {
            using (var api = new ConfigurarONUApiController())
            {
                var model = await api.FindById(IdONU);

                return View(model);
            }
        } 

      /*  [HttpGet()]
        public ActionResult ConfigurarONU(Int64? id)
        {
            ViewResult retorno = (ViewResult)base.CadastroBase<ConfigurarONUModel, Int64?>(id);

            return retorno;
        }
      */


        /*  [HttpPost]
          public async Task<ActionResult> CadastrarONU(Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel model)
          {
              using (var api = new CadastrarONUApiController())
              {
                  var retorno = await api.Gravar(new ParamWebApi<Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel>
                  {
                      Usuario = model.Usuario,
                      Objeto = model
                  });

                  if (retorno.Ok)
                  {
                      return RedirectToAction("ListagemONU");
                  }
                  else
                  {
                      AddModelError(retorno.Mensagem, retorno.InnerException);
                      return View(model);
                  }
              }
          }
        */


      [HttpPost]
        public async Task<ActionResult> ConfigurarONU(Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel model)
        {
            using (var api = new ConfigurarONUApiController())
            {
                var retorno = await api.Gravar(new ParamWebApi<Sinapse.Integracao.Ativacao.Models.ConfigurarONUModel>
                {
                    Usuario = model.Usuario,
                    Objeto = model
                });

                if (retorno.Ok)
                {
                    return RedirectToAction("ListagemONU");
                }
                else
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                    return View(model);
                }
            }
        } 


        public async Task<ActionResult> VincularONUCliente(Int32 SlotId, Int32 PonId, String Mac, String TipoONU, Int64 Id_OLT)
        {
            using (var api = new PonApiController())
            {
                GetApiRetorno<PON> _retorno = await api.FindByPonId(SlotId, PonId, Id_OLT);

                if (!_retorno.Ok)
                {
                    AddModelError(_retorno.Mensagem);
                }

                return View(new VinculoONUCliente
                {
                    Id_Pon = _retorno.Objeto.Id,
                    Mac = Mac,
                    TipoONU = TipoONU,
                    PONID = _retorno.Objeto.GetPonID()
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> VincularONUCliente(Int64? Id_Olt, Int64? PonId, Int64? SlotId, String Mac, String Serial, String TipoOnu, Int64? Id_ONU)
        {

            VinculoONUCliente retorno = null;

            if (Id_ONU.HasValue)
            {
                using (var apiONU = new ONUApiController())
                {
                    var retornoApi = await apiONU.FindById(Id_ONU.Value);

                    if (!retornoApi.Ok)
                    {
                        AddModelError(retornoApi.Mensagem);
                    }

                    retorno = retornoApi.Objeto;
                }
            }
            else
            {
                using (var apiPON = new PonApiController())
                {
                    var retornoApi = (await apiPON.FindByPonId(PonId.Value, SlotId.Value, Id_Olt.Value));

                    if (!retornoApi.Ok)
                    {
                        AddModelError(retornoApi.Mensagem);
                    }

                    if (retornoApi.Objeto.Id == 0)
                    {
                        AddModelError("PON não encontrada.");
                    }

                    retorno = new VinculoONUCliente
                    {
                        Mac = Mac,
                        Serial = Serial,
                        Id_Pon = retornoApi.Objeto.Id,
                        DescricaoPon = $"{retornoApi.Objeto?.Descricao}-(PONID: {retornoApi.Objeto.GetPonID()})",
                        TipoONU = TipoOnu,
                        Nro_Ip = retornoApi.Objeto?.Slot?.OLT?.DHCP_Lan_IP,
                        Ip_Inicio = retornoApi.Objeto?.Slot?.OLT?.DHCP_Inicio_Range,
                        Ip_Final = retornoApi.Objeto?.Slot?.OLT?.DHCP_Final_Range,
                        Ip_Mask = retornoApi.Objeto?.Slot?.OLT?.DHCP_Lan_Mascara,
                        Tipo_Identificacao = retornoApi.Objeto?.Slot?.OLT?.Tipo_Identificacao,
                        PortaVLan = 1
                    };
                }
            }

            return View(retorno);


        }

        [HttpPost]
        public async Task<ActionResult> VincularONUCliente(VinculoONUCliente pModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var api = new ONUApiController())
                    {
                        PostApiRetorno<Int64> retorno = await api.GravarONUVinculoCliente(new ParamWebApi<VinculoONUCliente>
                        {
                            Usuario = User.Identity.GetUserId(),
                            Objeto = pModel
                        });

                        if (retorno.Ok)
                        {
                            using (var apiIntegracao = new IntegracaoFiberHomeApiController())
                            {
                                GetApiRetorno<Int64> retornoAtivacao = await apiIntegracao.AtivarONU(new ParamWebApi<AtivacaoONU>
                                {
                                    Objeto = new AtivacaoONU
                                    {
                                        CodCt = pModel.CodCt,
                                        Projeto = pModel.Projeto,
                                        Num_Doc = pModel.Num_Doc.Value,
                                        Num_Obra = pModel.Num_Obra,
                                        CodMat = pModel.CodMat,
                                        DescricaoMat = pModel.TipoONU,
                                        CodCpl = pModel.CodCpl,
                                        Id_Cliente_Plano_Produto = retorno.IdObjeto
                                    },
                                    Usuario = User.Identity.GetUserId()
                                });

                                if (!retornoAtivacao.Ok)
                                {
                                    AddModelError(retornoAtivacao.Mensagem);
                                    return RedirectToAction("VincularONUCliente", "EstruturaFibra", new { Id_ONU = retorno.IdObjeto });
                                }

                            }

                            return RedirectToAction("VincularONUCliente", "EstruturaFibra", new { Sucesso = "S", Id_ONU = retorno.IdObjeto });
                        }
                        else
                        {
                            AddModelError(retorno.Mensagem, retorno.InnerException);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(pModel);
        }

        [HttpGet]
        public async Task<ActionResult> Status_Onu_Informacoes_Basicas(Int64 Id_ONU)
        {
            var model = new StatusOnuInformacoesBasicas();

            try
            {
                using (var api = new IntegracaoAtivacaoApiController())
                {
                    model.Id_Onu = Id_ONU;

                    var diagnosticoONU = await api.DiagnosticoONU(new AtivaEquipamento 
                    {
                        IdONU = Id_ONU
                    });

                    model.Temperatura = diagnosticoONU.Temperatura;
                    model.Tx = diagnosticoONU.Tx;
                    model.Rx = diagnosticoONU.Rx;
                    model.Distancia = diagnosticoONU.Type;
                    model.Rx_OLT = diagnosticoONU.Rx_OLT;
                    model.Tx_OLT = diagnosticoONU.Tx_OLT;
                    model.TempoOnline = diagnosticoONU.Uptime;
                    model.Voltagem = diagnosticoONU.Voltagem;

                    return View(model);

                }
            }

            catch (Exception ex)
            {
                AddModelError(ex.Message, String.Empty, String.Empty);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Status_Onu_Hardware(Int64 Id_ONU)
        {
            StatusOnuHardware model = new StatusOnuHardware();

            try
            {
                using (IntegracaoAtivacaoApiController integracao = new IntegracaoAtivacaoApiController())
                {
                    var hardware = await integracao.GetInfoHardwareONU(new AtivaEquipamento
                    {
                        IdONU = Id_ONU
                    });

                    model.Id_Onu = Id_ONU;

                    if (hardware != null)
                    {                        
                        model.Modelo = !String.IsNullOrEmpty(hardware.Modelo) ? hardware.Modelo : "-";
                        model.Versao_Software = !String.IsNullOrEmpty(hardware.Modelo) ? hardware.Versao_Software : "-";
                        model.CPU = !String.IsNullOrEmpty(hardware.Modelo) ? hardware.CPU : "-";
                        model.Memoria = !String.IsNullOrEmpty(hardware.Modelo) ? hardware.Memoria : "-";
                    }
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, String.Empty, String.Empty);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Status_Onu_Configuracoes(Int64 Id_ONU)
        {
            StatusOnuConfiguracoes model = new StatusOnuConfiguracoes();

            try
            {
                using (IntegracaoAtivacaoApiController integracao = new IntegracaoAtivacaoApiController())
                {
                    var configuracoes_pppoe = await integracao.GetInfoConfiguracoesONU(new AtivaEquipamento
                    {
                        IdONU = Id_ONU
                    });

                    model.Id_Onu = Id_ONU;

                    if (configuracoes_pppoe != null)
                    {
                        var objeto = configuracoes_pppoe;
                        model.Modo_Conexao = Convert.ToString(objeto?.GetType().GetProperty("Modo_Conexao")?.GetValue(objeto, null));
                        model.NAT = Convert.ToString(objeto?.GetType().GetProperty("NAT")?.GetValue(objeto, null));
                        model.Nome_Servico = Convert.ToString(objeto?.GetType().GetProperty("Nome_Servico")?.GetValue(objeto, null));
                        model.PPPOEMODE = Convert.ToString(objeto?.GetType().GetProperty("PPPOEMODE")?.GetValue(objeto, null));
                        model.PPPOE_Proxy = Convert.ToString(objeto?.GetType().GetProperty("PPPOE_Proxy")?.GetValue(objeto, null));
                        model.PPPOE_Usuario = Convert.ToString(objeto?.GetType().GetProperty("PPPOE_Usuario")?.GetValue(objeto, null));
                        model.QoS = Convert.ToString(objeto?.GetType().GetProperty("QoS")?.GetValue(objeto, null));
                        model.Tipo_Conexao = Convert.ToString(objeto?.GetType().GetProperty("Tipo_Conexao")?.GetValue(objeto, null));
                        model.VLAN_COS = Convert.ToString(objeto?.GetType().GetProperty("VLAN_COS")?.GetValue(objeto, null));
                        model.VLAN_ID = Convert.ToString(objeto?.GetType().GetProperty("VLAN_ID")?.GetValue(objeto, null));
                        model.Tipo_IP = Convert.ToString(objeto?.GetType().GetProperty("Tipo_IP")?.GetValue(objeto, null));
                        model.PPPOE_ServicoName = Convert.ToString(objeto?.GetType().GetProperty("PPPOE_ServicoName")?.GetValue(objeto, null));
                        model.IP_Estatico = Convert.ToString(objeto?.GetType().GetProperty("IP_Estatico")?.GetValue(objeto, null));
                        model.IP_Gateway = Convert.ToString(objeto?.GetType().GetProperty("IP_Gateway")?.GetValue(objeto, null));
                        model.DNS_Master = Convert.ToString(objeto?.GetType().GetProperty("DNS_Master")?.GetValue(objeto, null));
                        model.DNS_Slave = Convert.ToString(objeto?.GetType().GetProperty("DNS_Slave")?.GetValue(objeto, null));
                        model.Porta_ONU = Convert.ToString(objeto?.GetType().GetProperty("Porta_ONU")?.GetValue(objeto, null));
                        model.IP_Mascara = Convert.ToString(objeto?.GetType().GetProperty("IP_Mascara")?.GetValue(objeto, null));
                    }
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, String.Empty, String.Empty);
            }



            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Status_Onu_Ping(Int64 Id_ONU)
        {
            StatusOnuPing model = new StatusOnuPing();

            try
            {
                using (IntegracaoFiberHomeApiController integracao = new IntegracaoFiberHomeApiController())
                {
                    var retorno = (await integracao.TestePingONU(Id_ONU));

                    model.Id_Onu = Id_ONU;

                    if (retorno.Ok)
                    {
                        if (retorno.Objeto != null)
                        {
                            model.Respostas_Ping = retorno.Objeto;
                        }
                    }

                    else
                    {
                        AddModelError(retorno.Mensagem, String.Empty, String.Empty);
                    }



                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, String.Empty, String.Empty);
            }

            return View(model);
        }

        [HttpGet()]
        public ActionResult ImportacaoONU()
        {
            ExcelServiceLog<ONU_Importacao> modelo = new ExcelServiceLog<ONU_Importacao>
            {
                Items = new List<ONU_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

      

        [HttpPost]
        public async Task<ActionResult> ImportacaoONU(String Arquivo)
        {
            ExcelServiceLog<ONU_Importacao> modeloFisico = new ExcelServiceLog<ONU_Importacao>
            {
                Items = new List<ONU_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<ONU_Importacao> modelo = new ExcelServiceLog<ONU_Importacao>
            {
                Items = new List<ONU_Importacao>(),
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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(ONU_Importacao)));
                        modeloFisico = ExcelService.CastDataTableImportacao<ONU_Importacao>(dt);                        

                        using (ONUApiController onu_api = new ONUApiController())
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

                                        foreach(var erro in errosCampo)
                                        {
                                            await onu_api.GravaLogImportacao(
                                            new ModelLog.nhibernate.LogImportacaoOnu { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Serial_Equipamento = erro.Chave ?? "-", Erro = erro.Erro, Usuario = User.Identity.GetUserId() });
                                        }
                                    }
                                    catch(Exception ex)
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
                                            var retornoImportacao = await onu_api.ImportacaoModelo(item);
                                            if (!retornoImportacao.Ok)
                                            {
                                                modeloFisico.Log.Add(new ExcelServiceLogItem
                                                {
                                                    Chave = item.Serial,
                                                    Erro = $"Falha: {item.Serial} - erro: {retornoImportacao.Mensagem} {retornoImportacao.InnerException ?? String.Empty}"
                                                });

                                                ViewBag.Erro = "S";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ViewBag.Erro = "S";
                                            modeloFisico.Log.Add(new ExcelServiceLogItem
                                            {
                                                Chave = item.Serial,
                                                Erro = $"Erro ao processar com Identificação da ONU: {item.Serial}"
                                            });

                                            await onu_api.GravaLogImportacao(
                                                new ModelLog.nhibernate.LogImportacaoOnu { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Serial_Equipamento = item.Serial, Erro = ex.Message + ex.InnerException?.Message, Usuario = User.Identity.GetUserId() });

                                            CONSTANTES.log4Net.Error(ex.Message, ex);
                                        }
                                    }

                                    if(isBackground)
                                    {
                                        try
                                        {
                                            await new NotificacoesApiController().EnviaNotificacaoImportacao(new Notificacoes_CadastroModel
                                            {
                                                Descricao = modeloFisico.Log.Count > 0 ? "Importação de ONUs concluída com sucesso, analise o log para mais detalhes." : "Importação de ONUs concluída com sucesso.",
                                                Acao_Executada = Model.enumerado.Notificacoes_Acao_Executada.Importacao_ONU,
                                                Destinatarios = new List<NotificacoesDestinatarios_ListModel> { new NotificacoesDestinatarios_ListModel() { Usuario = UsuarioNotificar } }

                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            CONSTANTES.log4Net.Error(ex.Message, ex);
                                        }
                                    }
                                        
                                }));
                               
                                if (modeloFisico.Items.Count > 500)
                                {
                                    //Faz a importação em background sem aguardar retorno
                                    isBackground = true;
                                    _ = Task.Run(() => listaProcessos.First());                                    
                                }
                                else
                                {
                                    //Importa em primeiro plano e retorna para a tela
                                    Task.WaitAll(listaProcessos.ToArray());
                                }
                            }                          
                        }
                    }

                    modelo.Items.AddRange(modeloFisico.Items);
                    modelo.Log.AddRange(modeloFisico.Log);                 

                    ViewBag.Sucesso = modelo.Log.Count == 0 ? "S" : "N";
                    ViewBag.Background = isBackground ? "S" : "N";
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

        [HttpGet()]
        public ActionResult ExclusaoONU()
        {
            ExcelServiceLog<ONU_Exclusao> modelo = new ExcelServiceLog<ONU_Exclusao>
            {
                Items = new List<ONU_Exclusao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> ExclusaoONU(String Arquivo)
        {
            ExcelServiceLog<ONU_Exclusao> modeloFisico = new ExcelServiceLog<ONU_Exclusao>
            {
                Items = new List<ONU_Exclusao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<ONU_Exclusao> modelo = new ExcelServiceLog<ONU_Exclusao>
            {
                Items = new List<ONU_Exclusao>(),
                Log = new List<ExcelServiceLogItem>()
            };

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

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(ONU_Exclusao)));
                        modeloFisico = ExcelService.CastDataTableImportacao<ONU_Exclusao>(dt);

                        using (ONUApiController onu_api = new ONUApiController())
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
                                            await onu_api.GravaLogExclusao(
                                            new ModelLog.nhibernate.LogExclusaoOnu { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Info_Equipamento = erro.Chave ?? "-", Erro = erro.Erro, Usuario = User.Identity.GetUserId() });
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
                                            var retornoImportacao = await onu_api.ExclusaoModelo(item);
                                            if (!retornoImportacao.Ok)
                                            {
                                                modeloFisico.Log.Add(new ExcelServiceLogItem
                                                {
                                                    Chave = item.Serial,
                                                    Erro = $"Falha: {item.Serial} - erro: {retornoImportacao.Mensagem} {retornoImportacao.InnerException ?? String.Empty}"
                                                });

                                                ViewBag.Erro = "S";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ViewBag.Erro = "S";
                                            modeloFisico.Log.Add(new ExcelServiceLogItem
                                            {
                                                Chave = item.Serial,
                                                Erro = $"Erro ao processar com Identificação da ONU: {item.Serial}"
                                            });

                                            await onu_api.GravaLogExclusao(
                                                new ModelLog.nhibernate.LogExclusaoOnu { Data = DateTime.Now.Date.ToString("dd/MM/yyyy"), Hora = DateTime.Now.ToString("HH:mm"), Info_Equipamento = item.Serial, Erro = ex.Message + ex.InnerException?.Message, Usuario = User.Identity.GetUserId() });

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

        public ActionResult LogImportacaoONU()
        {
            return View();
        }

        public ActionResult LogExclusaoONU()
        {
            return View();
        }
    }

}