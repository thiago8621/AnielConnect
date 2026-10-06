//using Aniel.Connect.ControllersWebApi.Ativacao.OLTs;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.RetiradaMaterialHubsoft;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.Connect.Repository.Repository.MK;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.Voalle;
using log4net;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

//using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Proxy;
using NHibernate.Util;
using Resources;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Integracao.Ativacao.OLTs;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi.Ativacao
{
    [Authorize]
    public class IntegracaoAtivacaoApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ONUsPendentesRegistro")]
        public async Task<DataTableResponse<AutorizarONUListModel>> ONUsPendentesRegistro(Models.AtivaEquipamento request)
        {
            var listaOnus = new List<AutorizarONUListModel>();
            var listaOnusRetorno = new List<AutorizarONUListModel>();
            string msgErro = String.Empty;
            OLT _olt = new OLT();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {
                    _olt = await repository.selectFirst(p => p.Id == request.IdOLT);

                    if (_olt.PortaSSH == null && _olt.PortaTelnet == null && _olt.PortaTL1 == null && _olt.PortaTL1_SSH == null)
                        throw new EAnielException("Não foi possível se comunicar com a OLT pois não foi configurada nenhuma porta de comunicação SSH, Telnet ou TL1 no Cadastro de OLT.");

                    switch (_olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new Fiberhome_A_B(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new Fiberhome_A1(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Huawei:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new Huawei(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Datacom:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new Datacom(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.ZTE:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new ZTE(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Nokia:
                            listaOnus = await new AtivacaoRepository().ONUsPendentesRegistro(new Nokia(_olt), request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoSlot = new Conexao<Slot>(session))
                using (var reposPon = new Conexao<PON>(session))
                using (var apiCodCpl = new Codigo_ComplementarApiController())
                using (var api = new AcessoApiController())
                {
                    GetApiRetorno<List<Models.T5.AreaModuloModel>> areas = await api.GetArea_ModuloUsuario(User.Identity.GetUserId());
                    bool temAcesso257 = Core.Helpers.AcessoHelper.TemAcesso(areas.Objeto, "257");

                    listaOnusRetorno = listaOnus;
                    foreach (var onu in listaOnus)
                    {
                        var slot = await repoSlot.selectFirst(p => p.OLT == _olt &&
                                                                         p.NumSlot == Convert.ToInt64(onu.Slot));

                        var pon = await reposPon.selectFirst(p => p.OLT == _olt &&
                                                            p.Slot == slot &&
                                                            p.Num_PON == Convert.ToInt64(onu.PON));

                        //TODO: Necessário ver se posição do estoque deve ser atualizada para as outras fabricantes
                        if (_olt.Marca_Modelo == Marca_Modelo_OLT.Huawei)
                        {
                            var posicaoEstoque = await apiCodCpl.PosicaoEstoque(onu.Mac_Serial);

                            onu.Posicao_Estoque = posicaoEstoque.Posicao;
                            onu.Informacao = posicaoEstoque.InfoAdicional;
                        }


                        onu.Desc_Slot = slot?.Descricao ?? "Não cadastrado";
                        onu.Desc_PON = pon?.Descricao ?? "Não cadastrado";

                        if (!request.Origem_Mobile)
                        {
                            onu.Pode_Autorizar_Sem_OS = true;

                           /* if (!await TemVinculoOS(sessionT5, onu.Mac_Serial.ToUpperInvariant()))
                            {
                                if (temAcesso257)
                                {
                                    onu.Pode_Autorizar_Sem_OS = true;
                                }
                                else
                                {
                                    CONSTANTES.log4Net.Error($"ONU sem vinculo com OS. MAC/Serial: {onu.Mac_Serial}");
                                    listaOnusRetorno.RemoveAll(p => p.Mac_Serial.Equals(onu.Mac_Serial));
                                    listaOnusRetorno.Add(onu);
                                }
                            } */
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                msgErro = ex.Message;

            }

            CONSTANTES.log4Net.Error($"{Sinapse.Util.JsonService.SerializeObject(listaOnusRetorno)} - ERRO: {msgErro}");

            return new DataTableResponse<AutorizarONUListModel>
            {
                data = listaOnusRetorno,
                error = msgErro
            };
        }

        public async Task<Response<int?>> AtivarEquipamento(Models.AtivaEquipamento request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOLT = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                {
                    OLT olt = await repoOLT.selectFirst(p => p.Id == request.IdOLT);
                    ONU onu = await repoONU.selectFirst(p => p.Id == request.IdONU);

                    switch (olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new Fiberhome_A_B(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new Fiberhome_A1(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);
                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Huawei:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new Huawei(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Datacom:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new Datacom(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.ZTE:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new ZTE(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Nokia:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().AtivarEquipamentoAsync(new Nokia(olt), model, request.ScriptOLT, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if(model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Response<int?>
                {
                    valor = null,
                    result = false,
                    exception = ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString(),
                    message = ex.Message
                };
            }
        }

        public async Task<List<Sinapse.Integracao.Ativacao.Models.StatusONU>> GetStatusONUsByPON(long idOLT, string numSlot, string numPON, List<string> listSerial, List<string> listONTID, List<(string,string)> listSerialConvertido, bool lancarException = false)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOLT = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                {
                    OLT olt = await repoOLT.selectFirst(p => p.Id == idOLT);

                    switch (olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            return await new AtivacaoRepository().GetStatusONUsByPON(new Fiberhome_A_B(olt), numSlot, numPON, listSerial);
                        case Marca_Modelo_OLT.Fiberhome_A1:                           
                            return await new AtivacaoRepository().GetStatusONUsByPON(new Fiberhome_A1(olt), numSlot, numPON, listSerial, listSerialConvertido, this.VerificaIdONUBySerial); //Em alguns casos em TL1, será necessário comparar o ONTID para identificar a ONU
                        case Marca_Modelo_OLT.Huawei:
                            return await new AtivacaoRepository().GetStatusONUsByPON(new Huawei(olt), numSlot, numPON, listSerial, listSerialConvertido);
                        case Marca_Modelo_OLT.Datacom:
                            return await new AtivacaoRepository().GetStatusONUsByPON(new Datacom(olt), numSlot, numPON, listSerial);
                        case Marca_Modelo_OLT.ZTE:
                            return await new AtivacaoRepository().GetStatusONUsByPON(new ZTE(olt), numSlot, numPON, listONTID); //ZTE não retorna serial na consulta como as outras OLTs, passado id das ONTs como alternativa
                        case Marca_Modelo_OLT.Nokia:
                            return await new AtivacaoRepository().GetStatusONUsByPON(new Nokia(olt), numSlot, numPON, listSerial);
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                if (lancarException)
                    throw new EAnielException("Ocorreu um erro na consulta de status da ONU.", ex);

                return new List<Sinapse.Integracao.Ativacao.Models.StatusONU>();
            }
        }

        [HttpGet]
        [Route("api/IntegracaoAtivacaoApiController/GetStatusONUById")]
        public async Task<GetApiRetorno<Sinapse.Integracao.Ativacao.Models.StatusONU>> GetStatusONUById(Int64 IdOnu)
        {
            GetApiRetorno<Sinapse.Integracao.Ativacao.Models.StatusONU> retorno = new GetApiRetorno<Sinapse.Integracao.Ativacao.Models.StatusONU>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    ONU onu = await repoONU.selectFirst(p => p.Id == IdOnu);

                    if(onu?.OLT?.Marca_Modelo == Marca_Modelo_OLT.ZTE && !onu.OntID.HasValue)
                    {
                        var OntId = await new ZTE(onu.OLT).GetOntIdBySerial(onu.SerialConvertido ?? onu.Serial);

                        if (OntId.HasValue)
                            onu.OntID = OntId;
                        else
                            throw new Exception("Não foi possível localizar o equipamento na OLT pois o não há OntId preenchido no cadastro");
                    }

                    retorno.Objeto = (await GetStatusONUsByPON(onu.OLT.Id, onu.Slot.NumSlot.ToString(), onu.PON.Num_PON.ToString(), new List<string> { onu.Serial }, new List<string> { Convert.ToString(onu.OntID) }, new List<(string, string)> { (onu.Serial, onu.SerialConvertido) }, true)).FirstOrDefault();
                    
                    if (String.IsNullOrEmpty(retorno.Objeto.StatusStr))
                    {
                        retorno.Objeto.StatusStr = retorno.Objeto.Status.ToString();
                    }

                    retorno.Objeto.DataHoraUltimaAtualizacao = DateTime.Now;
                    retorno.Objeto.DataHoraUltimaAtualizacaoStr = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");                        
                    
                    onu.StatusOnu = retorno.Objeto.Status;
                    onu.UltimaAtualizacaoStatus = retorno.Objeto.DataHoraUltimaAtualizacao;

                    await repoONU.Update(onu);

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);

                var dataHoraAtual = DateTime.Now;

                using (ISession session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    ONU onu = await repoONU.selectFirst(p => p.Id == IdOnu);
                    onu.StatusOnu = Sinapse.Model.enumerados.StatusONU.Erro;
                    onu.UltimaAtualizacaoStatus = dataHoraAtual;

                    await repoONU.Update(onu);
                }

                return new GetApiRetorno<Sinapse.Integracao.Ativacao.Models.StatusONU>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Objeto = new Sinapse.Integracao.Ativacao.Models.StatusONU()
                    {
                        DataHoraUltimaAtualizacao = dataHoraAtual,
                        DataHoraUltimaAtualizacaoStr = dataHoraAtual.ToString("dd/MM/yyyy HH:mm:ss"),
                        Status = Sinapse.Model.enumerados.StatusONU.Erro,
                        StatusStr = Sinapse.Model.enumerados.StatusONU.Erro.ToString()
                    }
                };
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/DiagnosticoONU")]
        public async Task<DiagnosticoONU> DiagnosticoONU(Models.AtivaEquipamento request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    ONU onu = await repoONU.selectFirst(p => p.Id == request.IdONU);

                    switch (onu.OLT.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new Fiberhome_A_B(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new Fiberhome_A1(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        case Marca_Modelo_OLT.Huawei:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new Huawei(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        case Marca_Modelo_OLT.Datacom:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new Datacom(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        case Marca_Modelo_OLT.ZTE:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new ZTE(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        case Marca_Modelo_OLT.Nokia:
                            return FormatarRetornoDiagnosticoOnu(await new AtivacaoRepository().DiagnosticoONU(new Nokia(onu.OLT), BindObjToConfigurarONUModel(onu)));
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new DiagnosticoONU();
            }
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/GetInfoConfiguracoesONU")]
        public async Task<InfoConfigONU> GetInfoConfiguracoesONU(Models.AtivaEquipamento request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    ONU onu = await repoONU.selectFirst(p => p.Id == request.IdONU);

                    switch (onu.OLT.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            return await new AtivacaoRepository().GetConfigEquipamento(new Fiberhome_A_B(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            return await new AtivacaoRepository().GetConfigEquipamento(new Fiberhome_A1(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Huawei:
                            return await new AtivacaoRepository().GetConfigEquipamento(new Huawei(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Datacom:
                            return await new AtivacaoRepository().GetConfigEquipamento(new Datacom(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.ZTE:
                            return await new AtivacaoRepository().GetConfigEquipamento(new ZTE(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Nokia:
                            return await new AtivacaoRepository().GetConfigEquipamento(new Nokia(onu.OLT), BindObjToConfigurarONUModel(onu));
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/GetInfoHardwareONU")]
        public async Task<InfoHardwareONU> GetInfoHardwareONU(Models.AtivaEquipamento request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    ONU onu = await repoONU.selectFirst(p => p.Id == request.IdONU);

                    switch (onu.OLT.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new Fiberhome_A_B(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new Fiberhome_A1(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Huawei:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new Huawei(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Datacom:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new Datacom(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.ZTE:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new ZTE(onu.OLT), BindObjToConfigurarONUModel(onu));
                        case Marca_Modelo_OLT.Nokia:
                            return await new AtivacaoRepository().GetInfoHardwareONU(new Nokia(onu.OLT), BindObjToConfigurarONUModel(onu));
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return null;
            }
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ListaOnusConectadas")]
        public Response<List<Object>> ListaOnusConectadas(Models.AtivaEquipamento request)
        {
            try
            {
                var response = new Response<List<Object>>();
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {
                    var ListaOnus = new List<Object>();

                    OLT _olt = repository.selectFirst(p => p.Id == request.IdOLT).Result;

                    switch (_olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            ListaOnus.Add(new AtivacaoRepository().ListaOnusConectadas(new Fiberhome_A_B(_olt)));
                            break;
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            ListaOnus.Add(new AtivacaoRepository().ListaOnusConectadas(new Fiberhome_A1(_olt)));
                            break;
                        case Marca_Modelo_OLT.ZTE:
                            ListaOnus.Add(new AtivacaoRepository().ListaOnusConectadas(new ZTE(_olt)));
                            break;
                        case Marca_Modelo_OLT.Nokia:
                            ListaOnus.Add(new AtivacaoRepository().ListaOnusConectadas(new Nokia(_olt)));
                            break;
                        default:
                            throw new EAnielException("Integração não implementada");
                    }

                    response.valor = ListaOnus;
                }
                response.result = true;
                return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Response<List<Object>>
                {
                    result = false,
                    exception = ex.InnerException.ToString(),
                    message = ex.Message

                };
            }
        }


        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ReiniciarONU")]
        public async Task<Response> ReiniciarONU(Models.AtivaEquipamento request)
        {
            try
            {
                var response = new Response();
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPon = new Conexao<PON>(session))
                {
                    Int64? idPON = null;
                    Int64? idOLT = null;

                    ONU onu = repoONU.selectFirst(x => x.Id == request.IdONU).Result;

                    if (request.IdPON != null && request.IdPON != 0)
                        idPON = request.IdPON;
                    else
                        idPON = onu.PON?.Id;

                    if (request.IdOLT != null && request.IdOLT != 0)
                        idOLT = request.IdOLT;
                    else
                        idOLT = onu.OLT?.Id;

                    PON pon = repoPon.selectFirst(q => q.Id == idPON).Result;
                    OLT olt = repository.selectFirst(p => p.Id == idOLT).Result;

                    switch (olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            response = await new AtivacaoRepository().ReiniciarONU(new Fiberhome_A_B(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            response = await new AtivacaoRepository().ReiniciarONU(new Fiberhome_A1(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Huawei:
                            response = await new AtivacaoRepository().ReiniciarONU(new Huawei(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Datacom:
                            response = await new AtivacaoRepository().ReiniciarONU(new Datacom(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.ZTE:
                            response = await new AtivacaoRepository().ReiniciarONU(new ZTE(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Nokia:
                            response = await new AtivacaoRepository().ReiniciarONU(new Nokia(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }

                if (!response.result)
                {
                    throw new EAnielException("Erro ao reiniciar ONU", new Exception(response.message));
                }

                return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Response
                {
                    result = false,
                    exception = ex.InnerException.ToString(),
                    message = ex.Message

                };
            }
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ResetarONU")]
        public async Task<Response> ResetarONU(Aniel.Connect.Models.AtivaEquipamento request)
        {
            try
            {
                var response = new Response();
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPon = new Conexao<PON>(session))
                {
                    ONU onu = repoONU.selectFirst(x => x.Id == request.IdONU).Result;
                    OLT olt = repository.selectFirst(p => p.Id == onu.OLT.Id).Result;

                    switch (olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            response = await new AtivacaoRepository().ResetarONU(new Fiberhome_A_B(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            response = await new AtivacaoRepository().ResetarONU(new Fiberhome_A1(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Huawei:
                            response = await new AtivacaoRepository().ResetarONU(new Huawei(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Datacom:
                            throw new EAnielException("Este recurso não está disponível para OLTs Datacom.");
                        case Marca_Modelo_OLT.ZTE:
                            response = await new AtivacaoRepository().ResetarONU(new ZTE(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        case Marca_Modelo_OLT.Nokia:
                            response = await new AtivacaoRepository().ResetarONU(new Nokia(olt), BindObjToConfigurarONUModel(onu));
                            break;
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }

                if (!response.result)
                {
                    throw new EAnielException("Erro ao resetar ONU", new Exception(response.message));
                }

                return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Response
                {
                    result = false,
                    exception = ex.InnerException.ToString(),
                    message = ex.Message

                };
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/DesautorizarOnu")]
        public async Task<Response> DesautorizarONU(Models.AtivaEquipamento request)
        {
            
            try
            {
                var response = new Response();
                var getDadosONUnaOLT = new GetDadosONUOLT();

                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPon = new Conexao<PON>(session))
                {

                    ONU onu = repoONU.selectFirst(x => x.Id == request.IdONU).Result;

                    /*if(request.Origem_Web == false)
                    //{
                        var retornoStatus = await GetStatusONUsByPON(onu.OLT.Id,
                            onu.Slot.NumSlot.ToString(),
                            onu.PON.Num_PON.ToString(),
                            new List<string>() { onu.SerialConvertido ?? onu.Serial },
                            new List<string>() { onu.OntID.ToString() });

                        if (retornoStatus != null && retornoStatus.Count > 0)
                        {
                            if (retornoStatus.FirstOrDefault().Status == Sinapse.Model.enumerados.StatusONU.Indisponivel)
                            {   
                           
                                     await repoONU.Remove(onu);
                                     response.message = $"ONU de serial {onu.Mac} foi desautorizada com sucesso";
                                     response.result = true;
                                     return response;                              
                                                                                    
                            }
                            else if(retornoStatus.FirstOrDefault().Status == Sinapse.Model.enumerados.StatusONU.Erro)
                            {
                            
                                return new Response
                                {
                                message = RsCadastros.CRITICA_STATUS_ONU_INDISPONIVEL
                                }; 
                            }
                        }
                    //} */

                        //Se a ONU não existir no Connect e a requisição vier do Touch
                        //esta condição será atendida.
                        if(onu == null && request.Origem_Mobile == true)
                        {

                            //Na requisição, está sendo enviada uma listagem com todas as OLTs
                            //Vinculadas ao grupo/área da O.S.
                            if (request.IdsOLts?.Count > 0)
                            {

                            //Para cada id da OLT presente na lista, o método GetDadosONUnaOLT é chamado de forma assíncrona 
                            //e paralela, para que a aplicação possa verificar em mais OLTs simultaneamente. 
                            //Enquanto as "tasks" estão sendo executadas, o loop while irá buscar pela task que tenha encontrado
                            //uma ONU. Caso tenha encontrado, irá selecionar os dados necessários para a requisição da desautorização
                            //da ONU e irá encerrar o looping.

                                var tasks = request.IdsOLts.Select(async id =>
                                {
                                    var newRequest = new AtivaEquipamento
                                    {
                                        IdOLT = id,
                                        Serial_Convertido = request.Serial_Convertido,
                                        Serial = request.Serial
                                    };
                                    var result = await GetDadosONUnaOLT(newRequest);
                                    return (Id: id, Result: result);
                                }).ToList();

                                long? idOlt = null;
                                var pending = tasks.ToList();

                                while (pending.Count > 0)
                                {
                                    var finished = await Task.WhenAny(pending);
                                    pending.Remove(finished);

                                    var tuple = await finished;
                                    if (tuple.Result?.Onu_Encontrada == true)
                                    {
                                        idOlt = tuple.Id;
                                        request.IdOLT = (long)idOlt;
                                        request.OntId = Convert.ToInt64(tuple.Result.Onu_Id);
                                        request.NumSlot = Convert.ToInt64(tuple.Result.Slot);
                                        request.NumPon = Convert.ToInt64(tuple.Result.Pon);
                                        break;
                                    }
                                    
                                }

                                //Caso a ONU não tenha sido localizada na listagem de OLTs, o usuário será informado.
                                if (idOlt == null)
                                {
                                    throw new EAnielException("A ONU não foi localizada em nenhuma das OLTs vinculadas à área da ordem de serviço.");
                                }





                                // Monta o objeto da requisição
                                onu = new ONU
                                {
                                    OntID = Convert.ToInt32(request.OntId),
                                    Slot = new Slot { NumSlot = request.NumSlot },
                                    PON = new PON { Num_PON = request.NumPon },
                                    OLT = await repository.selectFirst(p => p.Id == request.IdOLT),
                                    Serial = request.Serial,
                                    SerialConvertido = request.Serial_Convertido,
                                };

                                //Executa a desautorização
                                response = await ExecutaDesautorizacaoONU(onu, request);

                                //Verifica se a ONU ainda consta na OLT
                                getDadosONUnaOLT = await GetDadosONUnaOLT(request);

                                //Se ainda constar, executar novamente o desprovisionamento
                                if (getDadosONUnaOLT.Onu_Encontrada == true)
                                {
                                    response = await ExecutaDesautorizacaoONU(onu, request);
                                    if (response.result == false)
                                    {
                                        throw new EAnielException($"Ocorreu um erro na desautorização da ONU de serial {onu.Mac}", new Exception(response.exception ?? response.message));
                                    }
                                    else
                                    {
                                        response.message = $" A ONU de serial {onu.Mac} foi desautorizada com sucesso";
                                        response.result = true;
                                        return response;
                                    }
                                }
                                else
                                {

                                    //Se não constar, retornar mensagem de sucesso.
                                    response.message = $" A ONU de serial {onu.Mac} foi desautorizada com sucesso";
                                    response.result = true;
                                    return response;
                                }

                            }
                            else
                            {
                                throw new EAnielException("Não foi possivel localizar a OLT para o equipamento baseado no Grupo Area/Area da Ordem de Serviço");
                            }



                        }
                        else if(onu == null && request.Origem_Mobile == false)
                        {

                            throw new Exception("Não foi possível encontrar a ONU no banco de dados. Por favor, tente novamente mais tarde.");

                        }
                        else
                        {
                              //Se a ONU estiver no banco de dados, este bloco else será executado.


                              request.IdOLT = onu.OLT.Id;
                              request.Serial = onu.Serial;
                              request.Serial_Convertido = onu.SerialConvertido ?? onu.Mac;




                             //Verifica se a ONU ainda consta na OLT
                             getDadosONUnaOLT = await GetDadosONUnaOLT(request);

                            //Se sim, valida se o slot, pon e ontId são iguais aos do cadastro.
                            // Caso sejam, a requisição ocorrerá com o valor do slot/pon/ontid que já consta no banco.
                            //Caso não, a requisição ocorrerá com o valor do slot/pon/ontid coletados no GetDadosONUnaOLT.

                            if (getDadosONUnaOLT.Onu_Encontrada == true)
                            {
                                var slot = onu.Slot.NumSlot;
                                var pon = onu.PON.Num_PON;
                                var ontId = onu.OntID;

                                if (ontId != Convert.ToInt32(getDadosONUnaOLT.Onu_Id)
                                           || slot != Convert.ToInt32(getDadosONUnaOLT.Slot)
                                           || pon != Convert.ToInt32(getDadosONUnaOLT.Pon))
                                {
                                    onu.Slot.NumSlot = Convert.ToInt64(getDadosONUnaOLT.Slot);
                                    onu.PON.Num_PON = Convert.ToInt64(getDadosONUnaOLT.Pon);
                                    onu.OntID = Convert.ToInt32(getDadosONUnaOLT.Onu_Id);

                                    response = await ExecutaDesautorizacaoONU(onu, request);
                                }
                                else
                                {
                                   response = await ExecutaDesautorizacaoONU(onu, request);
                                }
                                                            
                            }
                            else
                            {
                                //Se a ONU não consta mais na OLT, significa que a desautorização ocorreu com sucesso.
                                //True será retornado e os dados da ONU serão removidos do Connect.
                                response.result = true;
                            }

                        }




                                                                                  
                    if (response.result)
                    {
                        #region Integrações Externas de Provisionamento
                        try
                        {
                            IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                            var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                            IntegracoesExternas externas =
                            integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                            if (externas != null)
                            {
                                switch (externas.IntegracaoExterna)
                                {
                                  
                                    case IntegracaoExterna.MK_SOLUTION:


                                        IMKProvisionamentoRepository repoMK = new MKProvisionamentoApiRepository(externas);
                                        var mkBusiness = new ConnectBusiness.MK.MKBusiness(null, null, repoMK);
                                        var retornoVinculoConexaoMK = await mkBusiness.DesvinculaConexao(onu.LoginRadius.Login);


                                        if (retornoVinculoConexaoMK.success == false)
                                        {
                                            throw new EAnielException($"Erro no vínculo de login no MK. Retorno da API: {Sinapse.Util.JsonService.SerializeObject(retornoVinculoConexaoMK)}");
                                        }

                                        break;

                                    case IntegracaoExterna.HUBSOFT:

                                        IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                        if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                             String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                             String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                             String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                             String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                        {
                                            string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                            throw new EAnielException(message);
                                        }

                                        var loginHubsoft = new LoginHubsoftModel
                                        {
                                            client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                            client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                            username = dadosIntegracaoHubsoft.Usuario,
                                            password = dadosIntegracaoHubsoft.Senha
                                        };

                                        #region Movimento material cliente HubSoft
                                        if (CONSTANTES.AtivaMovimentacaoMat)
                                        {
                                            if(request.Origem_Mobile)
                                            {
                                                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                                                {
                                                    IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                                                    var dadosOS = await docProdRepo.GetDadosOSByNumOs(sessionT5, request.Detalhe, request.Detalhe, request.Usuario, this.EnviarLog);
                                                    var NumObra = dadosOS.num_obra;
                                                    var CodCt = dadosOS.codct;
                                                    var Projeto = dadosOS.projeto;
                                                    var CodMat = request.CodMat;
                                                    var CodCpl = request.CodCpl;

                                                    var osAniel = await docProdRepo.GetDadosOSByNumObra(sessionT5, NumObra, request.Detalhe, request.Usuario, this.EnviarLog);

                                                    if (osAniel == null)
                                                    {
                                                        throw new Exception("O.S não encontrada na base de dados do Aniel.");
                                                    }


                                                    string idHubsoft = osAniel.adicional_1;

                                                    if (string.IsNullOrEmpty(idHubsoft))
                                                    {
                                                        idHubsoft = osAniel.id_hubsoft;

                                                        if (string.IsNullOrEmpty(idHubsoft))
                                                        {
                                                            throw new Exception("ID da O.S está vazio.");
                                                        }

                                                    }

                                                    var osHubsoft = OrdemServicoHubSoftDao.GetOrdemServicoById(dadosIntegracaoHubsoft.UrlApi, Convert.ToInt32(idHubsoft), loginHubsoft, request.Detalhe, request.Usuario, this.EnviarLog);
                                                    bool provisionamento = false;                                                    
                                                    var materialAplicado = docProdRepo.GetAplicacaoNaoIntegrados(sessionT5, NumObra, CodCpl, CodMat, osAniel.projeto, provisionamento, request.Usuario, request.Detalhe, this.EnviarLog);



                                                    if (!String.IsNullOrEmpty(materialAplicado?.Codcpl))
                                                    {
                                                        var estoqueHubSoft = EstoqueHubSoftDao.GetEstoque(dadosIntegracaoHubsoft.UrlApi, CONSTANTES.UtilizaLocalEstoqueTecnico ? materialAplicado.DepositoTecnico.ToString() : materialAplicado.LocalEstoque.ToString(), loginHubsoft, request.Detalhe, request.Usuario, this.EnviarLog);
                                                        if (estoqueHubSoft != null)
                                                        {
                                                            string envioRemocao = "";
                                                            string retornoRemocao = "";

                                                            RetiradaHubsoftRequest modelRetirada = new RetiradaHubsoftRequest
                                                            {
                                                                id_cliente_servico = (long)osHubsoft.cliente_servico.id_cliente_servico,
                                                                observacao = $"Remoção de material OS {materialAplicado.NumObra}",
                                                                produtos = new List<Repository.Model.RetiradaMaterialHubsoft.Produto>()
                                                            };

                                                            var patrimonio = EstoqueHubSoftDao.GetPatrimonio(materialAplicado.Codcpl, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, request.Detalhe, request.Usuario, this.EnviarLog);

                                                            if (patrimonio?.produto.Count() == 0)
                                                            {
                                                                throw new Exception($"Não foi possível encontrar o patrimônio de serial {CodCpl}");
                                                            }

                                                            var patrimonioProduto = patrimonio.produto.FirstOrDefault();

                                                            if (patrimonioProduto?.cliente_servico == null ||
                                                                patrimonioProduto?.cliente_servico?.cliente?.id_cliente !=
                                                                osHubsoft?.cliente_servico?.id_cliente)
                                                            {
                                                                var nomeCliente = osHubsoft?.cliente_servico?.cliente?.nome_razaosocial ?? "desconhecido";

                                                                retornoRemocao = $"Nao foi possivel encontrar vinculo do serial {materialAplicado.Codcpl} com cliente {nomeCliente}";
                                                                throw new Exception(retornoRemocao);
                                                            }

                                                            modelRetirada.produtos.Add(new Repository.Model.RetiradaMaterialHubsoft.Produto
                                                            {
                                                                id_local_estoque = estoqueHubSoft.id_local_estoque,
                                                                campo_identificacao_patrimonio = "id_produto_item",
                                                                quantidade = 1,
                                                                id_produto = Convert.ToInt32(materialAplicado.Codmat),
                                                                id_produto_item_status = 6,
                                                                patrimonios = new List<Repository.Model.RetiradaMaterialHubsoft.Patrimonio> { new Repository.Model.RetiradaMaterialHubsoft.Patrimonio { id_produto_item = patrimonioProduto.id_produto_item } }
                                                            });

                                                            envioRemocao = JsonConvert.SerializeObject(modelRetirada);
                                                            retornoRemocao = EstoqueHubSoftDao.RetornarComodatoEstoque(modelRetirada, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, request.Detalhe, request.Usuario, this.EnviarLog);
                                                           
                                                            //Remove aplicação realizada anteriormente no provisionamento
                                                            await docProdRepo.RemoverAplicacao(sessionT5, materialAplicado.Codct, materialAplicado.Projeto, materialAplicado.NumObra, materialAplicado.Codmat, materialAplicado.Codcpl, request.Detalhe, request.Usuario, this.EnviarLog);

                                                        }

                                                    }
                                                }


                                            }


                                        }

                                        #endregion

                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            await repoONU.Remove(onu);                           
                            throw new EAnielException($"ONU de serial {onu.Mac} foi desautorizada com sucesso, porém integração externa falhou.");


                        }

                        #endregion

                        await repoONU.Remove(onu);
                        response.message = $"ONU de serial {onu.Mac} foi desautorizada com sucesso";
                    }
                    else
                    {
                        throw new EAnielException($"Ocorreu um erro na desautorização da ONU de serial {onu.Mac}", new Exception(response.exception ?? response.message));
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"{ex} \n InnerException: {ex.InnerException}");
                return new Response
                {
                    result = false,
                    exception = ex.InnerException?.ToString(),
                    message = ex.Message


                };
            }
        }

        public async Task<Response> ExecutaDesautorizacaoONU(ONU onu, Models.AtivaEquipamento request)
        {
            //Caso a ONU não exista no banco do Connect, ocorrerá um "bind" parcial
            //apenas com os dados necessários para o desprovisionamento.
            //Caso a ONU exista no banco, o processo de bind seguirá normalmente.

            var bindONU = new ConfigurarONUModel();

            if(onu.Id == 0 && request.Origem_Mobile == true)
            {
                bindONU.OntID = onu.OntID;
                bindONU.IdSlot = onu.Slot.NumSlot;
                bindONU.IdPon = onu.PON.Num_PON;
                bindONU.Serial = (onu.OLT.ConverteOlt == Status_Default.Ativo && !string.IsNullOrEmpty(onu.SerialConvertido))
                                    ? onu.SerialConvertido
                                    : onu.Serial;
                bindONU.Mac = (onu.OLT.ConverteOlt == Status_Default.Ativo && !string.IsNullOrEmpty(onu.SerialConvertido))
                                    ? onu.SerialConvertido
                                    : onu.Serial;
                bindONU.Tipo_Identificacao = onu.OLT.Tipo_Identificacao; 
            }
            else
            {
                bindONU = BindObjToConfigurarONUModel(onu);
            }

            var response = new Response();
            switch (onu.OLT.Marca_Modelo)
            {
                case Marca_Modelo_OLT.Fiberhome_A_B:
                    response = await new AtivacaoRepository().DesautorizarONU(new Fiberhome_A_B(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                case Marca_Modelo_OLT.Fiberhome_A1:
                    response = await new AtivacaoRepository().DesautorizarONU(new Fiberhome_A1(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                case Marca_Modelo_OLT.Huawei:
                    response = await new AtivacaoRepository().DesautorizarONU(new Huawei(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                case Marca_Modelo_OLT.Datacom:
                    response = await new AtivacaoRepository().DesautorizarONU(new Datacom(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                case Marca_Modelo_OLT.ZTE:
                    response = await new AtivacaoRepository().DesautorizarONU(new ZTE(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                case Marca_Modelo_OLT.Nokia:
                    response = await new AtivacaoRepository().DesautorizarONU(new Nokia(onu.OLT), bindONU, request.Detalhe, request.Usuario, this.EnviarLog);
                    break;
                default:
                    throw new EAnielException("Integração não implementada");
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/TesteIntegracoesDesprovisionamento")]
        public async Task<PostApiRetorno> TesteIntegracoesDesprovisionamento(Models.AtivaEquipamento request)
        {
            PostApiRetorno retorno = new PostApiRetorno();

            string serial = "";
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoONU = new Conexao<ONU>(session))
                {

                        IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                        var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                        IntegracoesExternas externas =
                        integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                        if (externas != null)
                        {
                            switch (externas.IntegracaoExterna)
                            {
                              case IntegracaoExterna.HUBSOFT:

                                var onu = await repoONU.selectFirst(p => p.Id == request.IdONU);
                                serial = onu.Serial;

                                IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                {
                                    string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                    throw new EAnielException(message);
                                }

                                var loginHubsoft = new LoginHubsoftModel
                                {
                                    client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                    client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                    username = dadosIntegracaoHubsoft.Usuario,
                                    password = dadosIntegracaoHubsoft.Senha
                                };

                                #region Movimento material cliente HubSoft
                                if (CONSTANTES.AtivaMovimentacaoMat)
                                {
                                    if (request.Origem_Mobile)
                                    {
                                            IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                                            var dadosOS = await docProdRepo.GetDadosOSByNumOs(sessionT5, request.Detalhe, request.Detalhe, request.Usuario, this.EnviarLog);
                                            var NumObra = dadosOS.num_obra;
                                            var CodCt = dadosOS.codct;
                                            var Projeto = dadosOS.projeto;
                                            var CodMat = request.CodMat;
                                            var CodCpl = request.CodCpl;

                                            var osAniel = await docProdRepo.GetDadosOSByNumObra(sessionT5, NumObra, request.Detalhe, request.Usuario, this.EnviarLog);

                                            if (osAniel == null)
                                            {
                                                throw new Exception("O.S não encontrada na base de dados do Aniel.");
                                            }


                                            string idHubsoft = osAniel.adicional_1;

                                            if (string.IsNullOrEmpty(idHubsoft))
                                            {
                                                idHubsoft = osAniel.id_hubsoft;

                                                if (string.IsNullOrEmpty(idHubsoft))
                                                {
                                                    throw new Exception("ID da O.S está vazio.");
                                                }

                                            }

                                            var osHubsoft = OrdemServicoHubSoftDao.GetOrdemServicoById(dadosIntegracaoHubsoft.UrlApi, Convert.ToInt32(idHubsoft), loginHubsoft, request.Detalhe, request.Usuario, this.EnviarLog);
                                            bool provisionamento = false;
                                            var materialAplicado = docProdRepo.GetAplicacaoNaoIntegrados(sessionT5, NumObra, CodCpl, CodMat, osAniel.projeto, provisionamento, request.Usuario, request.Detalhe, this.EnviarLog);



                                        if (!String.IsNullOrEmpty(materialAplicado?.Codcpl))
                                        {
                                            var estoqueHubSoft = EstoqueHubSoftDao.GetEstoque(dadosIntegracaoHubsoft.UrlApi, CONSTANTES.UtilizaLocalEstoqueTecnico ? materialAplicado.DepositoTecnico.ToString() : materialAplicado.LocalEstoque.ToString(), loginHubsoft, request.Detalhe, request.Usuario, this.EnviarLog);
                                            if (estoqueHubSoft != null)
                                            {
                                                string envioRemocao = "";
                                                string retornoRemocao = "";

                                                RetiradaHubsoftRequest modelRetirada = new RetiradaHubsoftRequest
                                                {
                                                    id_cliente_servico = (long)osHubsoft.cliente_servico.id_cliente_servico,
                                                    observacao = $"Remoção de material OS {materialAplicado.NumObra}",
                                                    produtos = new List<Repository.Model.RetiradaMaterialHubsoft.Produto>()
                                                };

                                                var patrimonio = EstoqueHubSoftDao.GetPatrimonio(materialAplicado.Codcpl, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, request.Detalhe, request.Usuario, this.EnviarLog);

                                                if(patrimonio?.produto.Count() == 0)
                                                {
                                                    throw new Exception($"Não foi possível encontrar o patrimônio de serial {CodCpl}");
                                                }

                                                var patrimonioProduto = patrimonio.produto.FirstOrDefault();

                                                if (patrimonioProduto?.cliente_servico == null ||
                                                      patrimonioProduto?.cliente_servico?.cliente?.id_cliente !=
                                                      osHubsoft?.cliente_servico?.id_cliente)
                                                {
                                                    var nomeCliente = osHubsoft?.cliente_servico?.cliente?.nome_razaosocial ?? "desconhecido";

                                                    retornoRemocao = $"Nao foi possivel encontrar vinculo do serial {materialAplicado.Codcpl} com cliente {nomeCliente}";
                                                    throw new Exception(retornoRemocao);
                                                }

                                                modelRetirada.produtos.Add(new Repository.Model.RetiradaMaterialHubsoft.Produto
                                                {
                                                    id_local_estoque = estoqueHubSoft.id_local_estoque,
                                                    campo_identificacao_patrimonio = "id_produto_item",
                                                    quantidade = 1,
                                                    id_produto = Convert.ToInt32(materialAplicado.Codmat),
                                                    id_produto_item_status = 6,
                                                    patrimonios = new List<Repository.Model.RetiradaMaterialHubsoft.Patrimonio> { new Repository.Model.RetiradaMaterialHubsoft.Patrimonio { id_produto_item = patrimonioProduto.id_produto_item } }
                                                });

                                                envioRemocao = JsonConvert.SerializeObject(modelRetirada);
                                                retornoRemocao = EstoqueHubSoftDao.RetornarComodatoEstoque(modelRetirada, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, request.Detalhe, request.Usuario, this.EnviarLog);                                                
                                                
                                                //Remove aplicação realizada anteriormente no provisionamento
                                                await docProdRepo.RemoverAplicacao(sessionT5, materialAplicado.Codct, materialAplicado.Projeto, materialAplicado.NumObra, materialAplicado.Codmat, materialAplicado.Codcpl, request.Detalhe, request.Usuario, this.EnviarLog);
                                                
                                            }

                                        }



                                    }


                                }

                                #endregion

                                break;


                            default:
                                    break;
                            }
                        }
                    

                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagens.Add(new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = ex.Message
                });
            }


            return retorno;
        }
        



        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/DesautorizarOnuEmMassa")]
        public PostApiRetorno DesautorizarONUEmMassa(Models.DesautorizarONUEmMassaRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();
            try
            {
                var response = new Response();
                var tasks = new List<Task>();

                foreach (var idONU in request.ListIdONU)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var resposta = await DesautorizarONU(new Models.AtivaEquipamento
                            {
                                IdONU = idONU
                            }
                        );
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = resposta.result == true ? PostApiMensagemType.Info : PostApiMensagemType.Erro,
                                Mensagem = resposta.message
                            });
                        }
                        catch (Exception ex)
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = PostApiMensagemType.Erro,
                                Mensagem = ex.Message
                            });
                        }



                    });

                    tasks.Add(task);
                }
                retorno.Ok = true;
                Task.WaitAll(tasks.ToArray());

                response.result = true;

                //return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);

                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                /*return new Response
                {
                    result = false,

                    exception = ex.ToString(),
                    message = ex.Message
                };*/
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ReiniciarOnuEmMassa")]
        public PostApiRetorno ReiniciarOnuEmMassa(Models.ReiniciarONUEmMassaRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();
            try
            {
                var response = new Response();
                var tasks = new List<Task>();

                foreach (var idONU in request.ListIdONU)
                {
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var resposta = await ReiniciarONU(new Models.AtivaEquipamento
                            {
                                IdONU = idONU
                            }
                        );
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = resposta.result == true ? PostApiMensagemType.Info : PostApiMensagemType.Erro,
                                Mensagem = resposta.message
                            });
                        }
                        catch (Exception ex)
                        {
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = PostApiMensagemType.Erro,
                                Mensagem = ex.Message
                            });
                        }



                    });

                    tasks.Add(task);
                }
                retorno.Ok = true;
                Task.WaitAll(tasks.ToArray());

                response.result = true;

                //return response;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);

                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                /*return new Response
                {
                    result = false,

                    exception = ex.ToString(),
                    message = ex.Message
                };*/
            }
            return retorno;
        }

        private async Task<Boolean> TemVinculoOS(ISession sessionT5, string macSerial)
        {
            long idClientePlanoProduto = 0;

            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql.AppendLine("SELECT FIRST 1 DOC_PROD.ID_CLIENTE_PLANOPRODUTOCONNECT");
                sql.AppendLine("FROM TB_MATERIAL_TAREFA MAT_TRF");

                sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_PROD");
                sql.AppendLine("ON(DOC_PROD.NUM_DOC = MAT_TRF.NUM_DOC AND");
                sql.AppendLine("    DOC_PROD.CODCT = MAT_TRF.CODCT AND");
                sql.AppendLine("    DOC_PROD.PROJETO = MAT_TRF.PROJETO AND");
                sql.AppendLine("    DOC_PROD.NUM_OBRA = MAT_TRF.NUM_OBRA)");

                sql.AppendLine("WHERE MAT_TRF.CODCPL = @MAC_SERIAL");
                sql.AppendLine("ORDER BY MAT_TRF.DATA_APLIC DESC");

                param.Add("@MAC_SERIAL", macSerial);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        var value = reader["ID_CLIENTE_PLANOPRODUTOCONNECT"];
                        if(value != DBNull.Value)
                        {
                            idClientePlanoProduto = Convert.ToInt64(value);
                        }
                    }
                        
                }
            }

            return idClientePlanoProduto > 0;
        }

        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/ConfigurarONUAsync")]
        public async Task<Response> ConfigurarONUAsync(Models.AtivaEquipamento request)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoOLT = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                {
                    OLT olt = await repoOLT.selectFirst(p => p.Id == request.IdOLT);
                    ONU onu = await repoONU.selectFirst(p => p.Id == request.IdONU);

                    switch (olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new Fiberhome_A_B(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new Fiberhome_A1(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);
                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Huawei:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new Huawei(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Datacom:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new Datacom(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.ZTE:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new ZTE(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        case Marca_Modelo_OLT.Nokia:
                            {
                                var model = BindObjToConfigurarONUModel(onu);
                                var responseOlt = await new AtivacaoRepository().ConfigurarONUAsync(new Nokia(olt), model, request.ScriptONU, request.Detalhe, request.Usuario, this.EnviarLog);

                                if (model.OntID.HasValue)
                                {
                                    onu.OntID = model.OntID;
                                    await repoONU.Update(onu);
                                }

                                responseOlt.result = true;
                                return ConvertResponseOLT(responseOlt, model.OntID);
                            }
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new Response<int?>
                {
                    valor = null,
                    result = false,
                    exception = ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString(),
                    message = ex.Message
                };
            }
        }

        //Coleta dados da ONU na OLT para realizar o desprovisionamento.
        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/GetDadosONUnaOLT")]
        public async Task<GetDadosONUOLT> GetDadosONUnaOLT(Models.AtivaEquipamento request)
        {
            var dadosONU = new GetDadosONUOLT();
            string msgErro = String.Empty;
            OLT _olt = new OLT();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                {
                    _olt = repository.selectFirst(p => p.Id == request.IdOLT).Result;

                    switch (_olt.Marca_Modelo)
                    {
                        case Marca_Modelo_OLT.Fiberhome_A_B:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new Fiberhome_A_B(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Fiberhome_A1:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new Fiberhome_A1(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Huawei:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new Huawei(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Datacom:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new Datacom(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.ZTE:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new ZTE(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        case Marca_Modelo_OLT.Nokia:
                            dadosONU = await new AtivacaoRepository().GetDadosONUnaOLT(new Nokia(_olt), request.Serial_Convertido, request.Serial, request.Usuario, request.Detalhe, this.EnviarLog);
                            break;
                        default:
                            throw new EAnielException("Integração não implementada");
                    }
                }
            }
            catch(Exception ex)
            {
                return new GetDadosONUOLT
                {
                    Onu_Encontrada = false,
                    Onu_Id = "",
                    Slot = "",
                    Pon = "",
                    Serial = ""
                   
                };
            }

            return dadosONU;
    
        }

        #region Mobile

        [OlaAuthorize]
        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/TesteCompleto")]
        public async Task<ResponseSucesso> TesteCompleto(TesteCompleto request)
        {
            try
            {
                var response = new Response();
                var retorno = new ResponseSucesso();
                var listaResponse = new List<Response>();
                bool erro = true;
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                {
                    //1) Testar Login
                    var integracaoApiRadius = new IntegracaoRadiusApiController();
                    var conexaoApi = new Relatorio_ConexaoApiController();
                    var testarLogin = new PlanoProdutoRadiusRequest
                    {
                        IdClientePlanoProduto = request.IdClientePlanoProduto
                    };
                    Response responseTesteLogin = await integracaoApiRadius.TesteLogin(testarLogin);


                    if (responseTesteLogin.result)
                    {
                        //2)Desconectar Login
                        var desconectarLogin = new PlanoProdutoRadiusRequest
                        {
                            IdClientePlanoProduto = request.IdClientePlanoProduto
                        };
                        Response responseDescLogin = await integracaoApiRadius.DesconectarLogin(desconectarLogin);


                        if (responseDescLogin.result)
                        {
                            //3) Reiniciar ONU
                            var apiONU = new ListagemONUApiController();
                            ONUListModel dadosONU = apiONU.GetDadosONU(request.IdClientePlanoProduto.ToString()).Result;
                            var valueONU = new AtivaEquipamento
                            {
                                IdONU = Convert.ToInt64(dadosONU.Id),
                                IdOLT = Convert.ToInt64(dadosONU.IdOLT),
                                IdPON = Convert.ToInt64(dadosONU.NumPON),
                            };
                            Response responseReiniciar = await ReiniciarONU(valueONU);

                            if (responseReiniciar.result)
                            {
                                //4) Get Ping
                                Model.nhibernate.LoginRadius radius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.IdClientePlanoProduto);
                                var valuePing = new Sinapse.Integracao.Ativacao.Models.GraficoPing_Request
                                {
                                    Id_Login_Radius = radius.Id
                                };
                                listaResponse.Add(await conexaoApi.GetPing(valuePing));

                                erro = false;
                            }
                        }
                    }

                    if (erro)
                    {
                        retorno.sucesso = false;
                        retorno.msg = "Ocorreram erros ao realizar o teste.";
                    }
                    else
                    {
                        retorno.sucesso = true;
                        retorno.msg = "Teste completado com sucesso!";
                    }

                }
                return retorno;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new ResponseSucesso
                {
                    msg = ex.Message,
                    sucesso = false
                };
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/GetIdONU")]
        public async Task<Response<Int64>> GetIdONU(Models.GetIdONURequest request)
        {
            Response<Int64> response = new Response<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoONU = new Conexao<ONU>(session))
                using (var apiAtivacao = new IntegracaoAtivacaoApiController())
                {

                    var idONU = await GetIdONUBySerialOuSerialConvertido(request.Mac, request.serialConvertido);
                    
                    if(idONU != 0)
                    {
                        var onu = await repoONU.selectFirst(p => p.Id == idONU);
                        
                        var macUpper = request.Mac.ToUpperInvariant();
                        var serialUpper = request.serialConvertido.ToUpperInvariant();

                        if (onu != null &&
                            (onu.Mac?.ToUpperInvariant() == macUpper ||
                             onu.Serial?.ToUpperInvariant() == macUpper ||
                             onu.SerialConvertido?.ToUpperInvariant() == macUpper ||
                             onu.Serial?.ToUpperInvariant() == serialUpper ||
                             onu.Mac?.ToUpperInvariant() == serialUpper ||
                             onu.SerialConvertido?.ToUpperInvariant() == serialUpper))
                        {
                            /* var listStatusONU = await apiAtivacao.GetStatusONUsByPON(onu.OLT.Id,
                                Convert.ToString(onu.Slot.NumSlot),
                                Convert.ToString(onu.PON.Num_PON),
                                new List<string> { (String.IsNullOrEmpty(onu.SerialConvertido) ? onu.Serial : onu.SerialConvertido) },
                                new List<string> { Convert.ToString(onu.OntID) });

                            if (listStatusONU.Count() > 0)
                            {
                                var statusOnu = listStatusONU.First().Status;
                                if (statusOnu != Sinapse.Model.enumerados.StatusONU.Erro)
                                    response.valor = onu.Id;
                                else
                                    throw new EAnielException("Nenhuma ONU ativa foi encontrada para o serial informado");
                            }
                            else
                            {
                                throw new EAnielException("Nenhuma ONU ativa foi encontrada para o serial informado");
                            }*/

                            response.result = true;
                            response.valor = onu.Id;
                        }
                        else
                        {
                            throw new EAnielException("Serial informado não corresponde ao do equipamento com sinal ativo para este Cliente");
                        }
                            
                    }
                    else
                    {
                        throw new EAnielException("Serial informado não corresponde ao do equipamento com sinal ativo para este Cliente");
                    }

                   
                }

                response.result = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                response.result = false;
                response.exception = ex.InnerException != null ? ex.InnerException.ToString() : null;
                response.message = ex.Message;
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/AtualizarInfoWifiLogin")]
        public async Task<Response<Int64>> AtualizarInfoWifiLogin(Models.AtualizarInfoWifiRequest request)
        {
            Response<Int64> response = new Response<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<OLT>(session))
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
                {
                    var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == request.Id_Cliente_Plano_Produto);

                    if (loginRadius != null)
                    {
                        if (!String.IsNullOrEmpty(request.SSID_Wifi))
                            loginRadius.SSID_WIFI = request.SSID_Wifi;

                        if (!String.IsNullOrEmpty(request.Senha_Wifi))
                            loginRadius.Senha_WIFI = request.Senha_Wifi;

                        await repoLoginRadius.SaveOrUpdate(loginRadius);
                    }

                    else
                        throw new EAnielException("Login Radius não encontrado para este Cliente");
                }

                response.result = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                response.result = false;
                response.exception = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                response.message = ex.Message;
            }

            return response;
        }
        #endregion

        private ConfigurarONUModel BindObjToConfigurarONUModel(ONU obj)
        {
            ConfigurarONUModel retorno = new ConfigurarONUModel();

            retorno.ClienteCpfCnpj = obj.Cliente_PlanoProduto.Cliente.CpfCnpj;
            retorno.Descricao = obj.Descricao;
            retorno.DescricaoPerfilAtivacao = obj?.PerfilAtivacao?.Nome;
            retorno.DescricaoPlano = obj.Cliente_PlanoProduto.Plano.Descricao;
            retorno.DescricaoPON = obj.PON.Descricao;
            retorno.Gabinete = obj.OLT.Gabinete;
            retorno.GemPort = obj.GemPort;
            retorno.Id = obj.Id;
            retorno.IdCliente = obj.Cliente_PlanoProduto.Cliente.Id;
            retorno.IdLogin = obj.LoginRadius.Id;

            if (!String.IsNullOrEmpty(obj.OLT.NumOLT))
            {
                int idOlt = 0;
                int.TryParse(obj.OLT.NumOLT, out idOlt);
                retorno.IdOLT = idOlt != 0 ? idOlt : (int?)null;
            }

            
            retorno.IdPerfilAtivacao = obj?.PerfilAtivacao?.Id;
            retorno.IdPlano = obj?.Cliente_PlanoProduto?.Plano?.Id;

            retorno.IdPon = obj.PON.Num_PON;
            retorno.IdSlot = obj.Slot.NumSlot;
            retorno.IpGerencia = obj.IpGerencia;
            retorno.IpOLT = obj.OLT.IP;
            retorno.LineProfile = obj?.PerfilAtivacao?.LineProfile;
            retorno.Login = obj.Login;
            //retorno.Mac = obj.OLT.ConverteOlt == Status_Default.Ativo ? obj.SerialConvertido : obj.Mac;
            retorno.Mac = (obj.OLT.ConverteOlt == Status_Default.Ativo && !string.IsNullOrEmpty(obj.SerialConvertido))
            ? obj.SerialConvertido
            : obj.Mac;
            retorno.MarcaModelo = obj.OLT.Marca_Modelo;
            retorno.OntID = obj.OntID;
            retorno.PerfilAtivacao_IpGerencia = obj?.PerfilAtivacao?.IpGerencia;
            retorno.PerfilAtivacao_Login = obj?.PerfilAtivacao?.Login;
            retorno.PerfilAtivacao_PortaTelnet = obj?.PerfilAtivacao?.PortaTelnet;
            retorno.PerfilAtivacao_Senha = obj?.PerfilAtivacao?.Senha;
            retorno.PonIDFiberhome = obj.PON.GetPonIDFiberhome();
            retorno.PortaTelnet = obj.PortaTelnet;
            retorno.PortaHttp = obj.PortaHttp;
            retorno.ScriptOLT = obj.ScriptOLT?.Length > 0 ? Encoding.UTF8.GetString(obj.ScriptOLT) : String.Empty;
            retorno.ScriptONU = obj.ScriptONU?.Length > 0 ? Encoding.UTF8.GetString(obj.ScriptONU) : String.Empty;
            retorno.Senha = obj.Senha;
            retorno.SenhaPPPOE = obj.LoginRadius.SenhaPPPOE;
            retorno.SenhaRoteador = obj.LoginRadius.SenhaRoteador;
            retorno.SenorId = obj.SenorId;
            //retorno.Serial = obj.OLT.ConverteOlt == Status_Default.Ativo ? obj.SerialConvertido : obj.Serial;
            retorno.Serial = (obj.OLT.ConverteOlt == Status_Default.Ativo && !string.IsNullOrEmpty(obj.SerialConvertido))
            ? obj.SerialConvertido
            : obj.Serial;
            retorno.ServicePort = obj.ServicePort;
            retorno.SSID_WIFI = obj.LoginRadius.SSID_WIFI;
            retorno.Subrack = obj.OLT.Subrack;
            retorno.Tipo_Identificacao = obj.Tipo_Identificacao;
            retorno.VLAN_Uplink = obj.VLAN_Uplink;
            retorno.VLAN_PPPOE = obj.VLAN_PPPOE;
            retorno.VLAN_Profile = obj.VLAN_Profile;
            

            return retorno;
        }

        private DiagnosticoONU FormatarRetornoDiagnosticoOnu(DiagnosticoONU model)
        {
            if (!String.IsNullOrEmpty(model.Rx))
                model.Rx += " Dbm";
            else
                model.Rx = "-";

            if (!String.IsNullOrEmpty(model.Tx))
                model.Tx += " Dbm";
            else
                model.Tx = "-";

            if (!String.IsNullOrEmpty(model.Rx_OLT))
                model.Rx_OLT += " Dbm";
            else
                model.Rx_OLT = "-";

            if (!String.IsNullOrEmpty(model.Tx_OLT))
                model.Tx_OLT += " Dbm";
            else
                model.Tx_OLT = "-";

            if (!String.IsNullOrEmpty(model.Voltagem))
                model.Voltagem += " V";
            else
                model.Voltagem = "-";

            if (String.IsNullOrEmpty(model.Type))
                model.Type = "-";

            if (!String.IsNullOrEmpty(model.Temperatura))
                model.Temperatura += " Cº";
            else
                model.Temperatura = "-";

            if (!String.IsNullOrEmpty(model.Bias_Current))
                model.Bias_Current += " mA";
            else
                model.Bias_Current = "-";

            if (String.IsNullOrEmpty(model.Uptime))
                model.Uptime = "-";

            return model;
        }

        private Response<int?> ConvertResponseOLT(Response response, int? ontID)
        {
            var retorno = new Response<int?>();

            retorno.exception = response.exception;
            retorno.message = response.message;
            retorno.stacktrace = response.stacktrace;
            retorno.result = response.result;

            if (ontID.HasValue)
                retorno.valor = ontID.Value;

            return retorno;
        }

        public string VerificaIdONUBySerial(string serial)
        {
            string ontId = "";
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var onuRepo = new Conexao<ONU>(session))
                {
                    var task = onuRepo.selectFirst(p => p.Mac == serial);
                    task.Wait(); 
                    var onu = task.Result; 

                    if (onu != null)
                    {
                        if (onu.OntID != null)
                        {
                            ontId = Convert.ToString(onu.OntID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return ontId;

        }

        public Task<long> EnviarLog(string json, String usuario, String codOrdemServico, long? id = null)
        {
            Task<long> retorno = Task.FromResult(0L);

            try
            {
                using (ISession session = HibernateUtil.GetSessionLog())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        if (id == null || id == 0)
                        {
                            sql.AppendLine("INSERT INTO TB_LOG_COMUNICACAO(SERVICO, DETALHE, DATA_HORA, USUARIO, XML_REQUEST, TIPO) ");
                            sql.AppendLine("VALUES (@SERVICO, @DETALHE, CURRENT_TIMESTAMP, @USUARIO, @REQUEST, @TIPO)");
                            sql.AppendLine("RETURNING ID");

                            parametros.Add("@REQUEST", json);
                            if (codOrdemServico.Contains("OPERAÇÃO REALIZADA SEM O.S"))
                            {
                                parametros.Add("@SERVICO", "ATIVAÇÃO DE SINAL PELO CONNECT");
                            }
                            else
                            {
                                parametros.Add("@SERVICO", "ATIVAÇÃO DE SINAL COM TOUCH");
                            }                           
                            parametros.Add("@DETALHE", codOrdemServico);
                            parametros.Add("@USUARIO", usuario);
                            parametros.Add("@TIPO", 5);

                            retorno = CommandHelper.ExecuteNonQueryReturning(com, sql, parametros, session.Connection, null);
                        }
                        else
                        {
                            sql.AppendLine("UPDATE TB_LOG_COMUNICACAO ");
                            sql.AppendLine("SET XML_RESPONSE = @RESPONSE ");
                            sql.AppendLine("WHERE ID = @ID ");

                            parametros.Add("@RESPONSE", json);
                            parametros.Add("@ID", id);

                            var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType().Assembly.GetName().Name).Error(String.Format("ERROR:{0}{1}STACKTRACE:{2}{3}OBJETO:{4}{5}{6}",
                   ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, "", Environment.NewLine, Environment.NewLine));
            }

            return retorno;
        }

       public async Task<long> GetIdONUBySerialOuSerialConvertido(string serial, string serialConvertido)
        {
            long id = 0;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> param = new Dictionary<string, object>();

                    sql.AppendLine("SELECT FIRST 1 ID");
                    sql.AppendLine("FROM TB_ONU");

                    sql.AppendLine("WHERE UPPER(SERIAL) = UPPER(@SERIAL) ");
                    sql.AppendLine("OR UPPER(MAC) = UPPER(@SERIAL) ");
                    sql.AppendLine("OR UPPER(SERIAL_CONVERTIDO) = UPPER(@SERIAL_CONVERTIDO) ");
                    sql.AppendLine("OR UPPER(MAC) = UPPER(@SERIAL_CONVERTIDO) ");
                    sql.AppendLine("OR UPPER(SERIAL) = UPPER(@SERIAL_CONVERTIDO) ");
                    sql.AppendLine("OR UPPER(SERIAL_CONVERTIDO) = UPPER(@SERIAL) ");


                    param.Add("@SERIAL", serial);
                    param.Add("@SERIAL_CONVERTIDO", serialConvertido);


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            var value = reader["ID"];
                            if (value != DBNull.Value)
                            {
                                id = Convert.ToInt64(value);
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return id;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/IntegracaoAtivacaoApiController/LimparMac")]
        public async Task<Response> LimparMac(LimparMacRequest request)
        {
            var retorno = new Response();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                    if (externas != null)
                    {
                        switch (externas.IntegracaoExterna)
                        {

                            case IntegracaoExterna.HUBSOFT:

                                if(String.IsNullOrEmpty(request.Num_Obra))
                                {
                                    throw new Exception("Número da Ordem de Serviço não consta na requisição.");
                                }

                                Documento_ProducaoAniel dadosOs = null;
                                string idClienteServico = String.Empty;

                                using(var repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                                {
                                    dadosOs = await repoDocProd.selectFirst(p => p.Num_Obra == request.Num_Obra);
                                }

                                if(!String.IsNullOrEmpty(dadosOs.LoginRadius))
                                {
                                    using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                                    {
                                        LoginRadius login = await repoLoginRadius.selectFirst(p => p.Login == dadosOs.LoginRadius);

                                        if (!string.IsNullOrEmpty(login?.CodigoAuxiliar))
                                            idClienteServico = login.CodigoAuxiliar;
                                                                                  
                                    }
                                }
                                else
                                {
                                    idClienteServico = dadosOs.Num_Doc.ToString();
                                }

                                if(String.IsNullOrEmpty(idClienteServico))
                                {
                                    throw new Exception("Id Cliente Serviço não consta na Ordem de Serviço.");
                                }

                                

                                IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                {
                                    string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                    throw new EAnielException(message);
                                }

                                var loginHubsoft = new LoginHubsoftModel
                                {
                                    client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                    client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                    username = dadosIntegracaoHubsoft.Usuario,
                                    password = dadosIntegracaoHubsoft.Senha
                                };

                                var dadosBodyRequest = new LimpezaMacHubsoft
                                {
                                    id_cliente_servico = idClienteServico
                                };

                                string usuario = request.Usuario;
                                string detalhe = dadosOs.Numero_Obra_Original;


                                var limpezaMac = ProvisionamentoHubsoft.LimpezaMac(dadosIntegracaoHubsoft.UrlApi, dadosBodyRequest,
                                    loginHubsoft, usuario, detalhe, this.EnviarLog);

                                if (limpezaMac.status != "success")
                                {
                                    throw new EAnielException("Erro durante o processo de limpeza de MAC do cliente.",
                                     new Exception($"Retorno do Hubsoft: {Sinapse.Util.JsonService.SerializeObject(limpezaMac)}"));
                                }

                                retorno.result = true;
                                retorno.message = limpezaMac.msg;

                                break;

                            default:
                                break;
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                retorno.result = false;
                retorno.message = ex?.Message;
                retorno.exception = ex?.InnerException?.Message;
            }

            return retorno;
        }
    }
}