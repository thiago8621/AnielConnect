using Aniel.Connect.ControllersWebApi.SMS.Integracoes;
using Aniel.Connect.ControllersWebApi.SMS.Repositorio;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.SMS
{
    public class IntegracaoSMSApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/IntegracaoSMSApiController/EnviarSMS")]
        public async Task<GetApiRetorno<EnviarSMS_ResponseModel>> EnviarSMS(EnviarSMS_RequestModel request)
        {
            GetApiRetorno<EnviarSMS_ResponseModel> retorno = new GetApiRetorno<EnviarSMS_ResponseModel>();
            var retornoIntegracao = new EnviarSMS_ResponseModel();
            try
            {
                Aniel.Connect.Model.nhibernate.SMS sms = new Model.nhibernate.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Aniel.Connect.Model.nhibernate.SMS>(session))
                {
                    if (request.Id_Integracao.HasValue)
                    {
                        sms = await repoSMS.selectFirst(p => p.Id == request.Id_Integracao);
                    }
                    else
                    {
                        sms = new Model.nhibernate.SMS
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
                        case Model.enumerado.Tipo_Integracao_SMS.BVTelecom:

                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarSMS(new BVTelecom(), sms);
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
        public async Task<GetApiRetorno<EnviarLoteSMS_ResponseModel>> EnviarLoteSMS(EnviarLoteSMS_RequestModel request)
        {
            GetApiRetorno<EnviarLoteSMS_ResponseModel> retorno = new GetApiRetorno<EnviarLoteSMS_ResponseModel>();
            var retornoIntegracao = new EnviarLoteSMS_ResponseModel();

            List<Aniel.Connect.Model.nhibernate.SMS> listaSMS = new List<Model.nhibernate.SMS>();
            try
            {
                Aniel.Connect.Model.nhibernate.SMS sms = new Model.nhibernate.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Aniel.Connect.Model.nhibernate.SMS>(session))
                {
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
                            listaSMS.Add(sms = new Model.nhibernate.SMS
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
                        case Model.enumerado.Tipo_Integracao_SMS.BVTelecom:

                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarLoteSMS(new BVTelecom(), listaSMS);
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
    }
}