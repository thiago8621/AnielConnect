//using Aniel.Connect.ControllersWebApi.SMS.Integracoes;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi.SMS.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Cliente;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.SMS.Integracoes;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.SMS
{
    [Authorize]
    public class IntegracaoSMSApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/IntegracaoSMSApiController/EnviarSMS")]
        public async Task<GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel>> EnviarSMS(EnviarSMS_RequestModel request)
        {
            GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel> retorno = new GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel>();
            var retornoIntegracao = new Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel();
            try
            {
                Sinapse.Model.SMS sms = new Sinapse.Model.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Sinapse.Model.SMS>(session))
                using (var repoconfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config = await repoconfig.selectFirst();
                    if (request.Id_Integracao.HasValue)
                    {
                        sms = await repoSMS.selectFirst(p => p.Id == request.Id_Integracao);
                    }
                    else
                    {
                        sms = new Sinapse.Model.SMS
                        {
                            Num_Celular = request.Num_Celular,
                            Mensagem = request.Mensagem,
                            Tipo_Integracao = request.Tipo_Integracao
                        };

                        if(request.Salvar_Em_Banco)
                        await repoSMS.SaveOrUpdate(sms);
                    }

                    switch (request.Tipo_Integracao)
                    {
                        case Tipo_Integracao_SMS.BVTelecom:

                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarSMS(new BVTelecom(config.Token_Integracao_SMS), sms);
                            break;
                        case Tipo_Integracao_SMS.Blip:
                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarSMS(new Blip(CONSTANTES.contractid, CONSTANTES.keyblip,request.Cliente.GetValueOrDefault()), sms);
                            break;
                        default:
                            throw new Exception("Integração não informada ou não implementada");
                    }

                    sms.Enviado = retornoIntegracao.Sucesso;

                    if (request.Salvar_Em_Banco)
                    {
                        await repoSMS.SaveOrUpdate(sms);
                    }
                }

                retorno.Objeto = retornoIntegracao;

                if (!retornoIntegracao.Sucesso)
                {
                    throw new Exception(retornoIntegracao.MensagemErro);
                }

                retorno.Ok = true;                
            }
            catch (Exception ex)
            {
                retorno.Mensagem = $"{ex.Message} {ex.InnerException?.Message}";
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/IntegracaoSMSApiController/EnviarLoteSMS")]
        public async Task<GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel>> EnviarLoteSMS(EnviarLoteSMS_RequestModel request)
        {
            GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel> retorno = new GetApiRetorno<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel>();
            var retornoIntegracao = new Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel();

            List<Sinapse.Model.SMS> listaSMS = new List<Sinapse.Model.SMS>();
            try
            {
                Sinapse.Model.SMS sms = new Sinapse.Model.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Sinapse.Model.SMS>(session))
                using (var repoconfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config = await repoconfig.selectFirst();
                    foreach (var smsModel in request.Mensagens)
                    {
                        if (smsModel.Id_Integracao.HasValue)
                        {
                            sms = await repoSMS.selectFirst(p => p.Id == smsModel.Id_Integracao);
                            if (sms != null)
                            {
                                listaSMS.Add(sms);
                            }
                        }
                        else
                        {
                            listaSMS.Add(sms = new Sinapse.Model.SMS
                            {
                                Num_Celular = smsModel.Num_Celular,
                                Mensagem = smsModel.Mensagem,
                                Tipo_Integracao = smsModel.Tipo_Integracao
                            });

                            if (request.Salvar_Em_Banco)
                                await repoSMS.SaveOrUpdate(sms);
                        }
                    }                    

                    switch (request.Tipo_Integracao)
                    {
                        case Tipo_Integracao_SMS.BVTelecom:

                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarLoteSMS(new BVTelecom(config.Token_Integracao_SMS), listaSMS);
                            break;
                        case Tipo_Integracao_SMS.Blip:
                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarLoteSMS(new Blip(CONSTANTES.contractid, CONSTANTES.keyblip,0), listaSMS);
                            break;
                        default:
                            throw new Exception("Integração não informada ou não implementada");
                    }

                    if (request.Salvar_Em_Banco)
                        await repoSMS.SaveOrUpdate(listaSMS);
                }

                

                retorno.Objeto = retornoIntegracao;

                if (!retornoIntegracao.Sucesso)
                {
                    throw new Exception(retornoIntegracao.MensagemErro);
                }

                retorno.Ok = true;               
            }
            catch (Exception ex)
            {
                retorno.Mensagem = $"{ex.Message} {ex.InnerException?.Message}";
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/IntegracaoSMSApiController/ContatoBloqueiaVenda")] 
        public async Task<GetApiRetorno> ContatoBloqueiaVenda(long Cliente) 
        {
            var retorno = new GetApiRetorno();
            try 
            {
                using (ISession session = HibernateUtil.GetSession()) 
                {
                    IClienteRepository repository = new ClienteRepository(session);
                    
                    var business = new ClienteBusiness(repository);

                    if (business.GetById(Cliente) != null) 
                    {
                        //business.hasAnyContatoValidado(Cliente);
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro) 
            {
                retorno.Ok = false;
                retorno.Mensagem = $"{erro.Message} - {erro?.InnerException?.Message}";
            }
            return retorno;
        }
    }
}