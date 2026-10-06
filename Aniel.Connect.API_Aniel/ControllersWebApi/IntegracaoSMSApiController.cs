using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.SMS.Integracoes;
using Sinapse.Integracao.SMS.Repositorio;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class IntegracaoSMSApiController : BaseApiController
    {       

        [HttpPost]
        [Route("api/IntegracaoSMSApiController/EnviarSMS")]
        public async Task<ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel>> EnviarSMS(EnviarSMS_RequestModel request)
        {
            Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel retorno = new Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel();
            
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                request.Num_Celular = FormatarNumCelular(request.Num_Celular);

                if (!CelularIsValido(request.Num_Celular))
                {
                    throw new Exception("Número de Celular em padrão inválido. Informar o número celular do destinatário contendo DDD no seguinte padrão: 11988887777.");
                }

                //var purl = CONSTANTES.URLConnect;
                //var retornoApi = Post<EnviarSMS_ResponseModel>(purl, $"api/IntegracaoSMSApiController/EnviarSMS", request);
                var retornoIntegracao = new Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel();
                Sinapse.Model.SMS sms = new Sinapse.Model.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Sinapse.Model.SMS>(session))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var configGerais = await repoConfigGerais.selectFirst();

                    if (!configGerais.Utiliza_Integracao_SMS)
                    {
                        throw new Exception("Integração de SMS não configurada no sistema");
                    }

                    if (configGerais.Tipo_Integracao_SMS == null)
                    {
                        throw new Exception("Tipo de Integração SMS não definido no sistema");
                    }

                    if (String.IsNullOrEmpty(configGerais.Token_Integracao_SMS))
                    {
                        throw new Exception("Token de Integração SMS não configurado no sistema");
                    }

                    if (request.Id_Integracao.HasValue && request.Id_Integracao != 0)
                    {
                        sms = await repoSMS.selectFirst(p => p.Id == request.Id_Integracao);
                    }

                    if (!request.Id_Integracao.HasValue || request.Id_Integracao == 0 || sms == null)
                    {
                        sms = new Sinapse.Model.SMS
                        {
                            Num_Celular = request.Num_Celular,
                            Mensagem = request.Mensagem,
                            Tipo_Integracao = configGerais.Tipo_Integracao_SMS.Value
                        };

                        //if (request.Salvar_Em_Banco)
                        await repoSMS.SaveOrUpdate(sms);
                    }

                    switch (configGerais.Tipo_Integracao_SMS)
                    {
                        case Sinapse.Model.enumerados.Tipo_Integracao_SMS.BVTelecom:
                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarSMS(new BVTelecom(configGerais.Token_Integracao_SMS), sms);
                            break;
                        case Sinapse.Model.enumerados.Tipo_Integracao_SMS.PontalTech:
                            if (String.IsNullOrEmpty(configGerais.Usuario_Integracao_SMS) || String.IsNullOrEmpty(configGerais.Senha_Integracao_SMS))
                            {
                                throw new Exception("Credenciais da integração de SMS da PontalTech não cadastradas corretamente");
                            }

                            retornoIntegracao = 
                                await new IntegracaoSMSRepository()
                                    .EnviarSMS(
                                        new PontalTech(
                                            configGerais.Usuario_Integracao_SMS,
                                            configGerais.Senha_Integracao_SMS
                                        ), 
                                        sms
                                    );
                            break;
                        case Sinapse.Model.enumerados.Tipo_Integracao_SMS.Blip:
                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarSMS(new Blip(CONSTANTES.contractid, CONSTANTES.keyblip, request.Cliente.GetValueOrDefault()), sms);
                            break;
                        default:
                            throw new Exception("Integração não informada ou não implementada");
                    }

                    sms.Enviado = retornoIntegracao.Sucesso;
                   
                    await repoSMS.SaveOrUpdate(sms);                
                }
               

                if (!retornoIntegracao.Sucesso)
                {
                    throw new Exception(retornoIntegracao.Mensagem ?? retornoIntegracao.MensagemErro);
                }                

                retorno = retornoIntegracao;
                
                return new ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel> { data = retorno, ok = true };
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel> { error = ex.Message };                
            }            
        }

        [HttpPost]
        [Route("api/IntegracaoSMSApiController/EnviarLoteSMS")]
        public async Task<ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel>> EnviarLoteSMS(EnviarLoteSMS_RequestModel request)
        {
            Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel retorno = new Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                List<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel> lista_mal_formatados = new List<Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel>();
                EnviarLoteSMS_RequestModel request_tratado = new EnviarLoteSMS_RequestModel();
                request_tratado.settings = request.settings;
                foreach(var item in request.Mensagens)
                {
                    if(CelularIsValido(item.Num_Celular))
                    {
                        request_tratado.Mensagens.Add(item);
                    }
                    else
                    {
                        lista_mal_formatados.Add(new Sinapse.Integracao.SMS.Models.EnviarSMS_ResponseModel { 
                            Sucesso = false,
                            MensagemErro = $"Número de Celular {item.Num_Celular} em padrão inválido. Informar o número celular do destinatário contendo DDD no seguinte padrão: 11988887777.",
                        });
                    }
                }


                //var purl = CONSTANTES.URLConnect;
                //var retornoApi = Post<EnviarLoteSMS_ResponseModel>(purl, $"api/IntegracaoSMSApiController/EnviarLoteSMS", request);

                var retornoIntegracao = new Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel();

                List<Sinapse.Model.SMS> listaSMS = new List<Sinapse.Model.SMS>();

                Sinapse.Model.SMS sms = new Sinapse.Model.SMS();
                using (ISession session = HibernateUtil.GetSession())
                using (var repoSMS = new Conexao<Sinapse.Model.SMS>(session))
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var configGerais = await repoConfigGerais.selectFirst();

                    if (!configGerais.Utiliza_Integracao_SMS)
                    {
                        throw new Exception("Integração de SMS não configurada no sistema");
                    }

                    if (configGerais.Tipo_Integracao_SMS == null)
                    {
                        throw new Exception("Tipo de Integração SMS não definido no sistema");
                    }

                    if (String.IsNullOrEmpty(configGerais.Token_Integracao_SMS))
                    {
                        throw new Exception("Token de Integração SMS não configurado no sistema");
                    }

                    foreach (var smsModel in request.Mensagens)
                    {
                        if (smsModel.Id_Integracao.HasValue && smsModel.Id_Integracao != 0)
                        {
                            sms = await repoSMS.selectFirst(p => p.Id == smsModel.Id_Integracao);
                            if (sms != null)
                            {
                                listaSMS.Add(sms);
                            }
                        }
                        if (!smsModel.Id_Integracao.HasValue || smsModel.Id_Integracao == 0 || sms == null)
                        {
                            listaSMS.Add(sms = new Sinapse.Model.SMS
                            {
                                Num_Celular = smsModel.Num_Celular,
                                Mensagem = smsModel.Mensagem,
                                Tipo_Integracao = configGerais.Tipo_Integracao_SMS.Value
                            });

                            //if (request.Salvar_Em_Banco)
                            await repoSMS.SaveOrUpdate(sms);
                        }
                    }

                    switch (configGerais.Tipo_Integracao_SMS)
                    {
                        case Sinapse.Model.enumerados.Tipo_Integracao_SMS.BVTelecom:
                            retornoIntegracao = await new IntegracaoSMSRepository().EnviarLoteSMS(new BVTelecom(configGerais.Token_Integracao_SMS), listaSMS);
                            break;
                        case Sinapse.Model.enumerados.Tipo_Integracao_SMS.PontalTech:
                            retornoIntegracao =
                                await new IntegracaoSMSRepository()
                                    .EnviarLoteSMS(
                                        new PontalTech(
                                            configGerais.Usuario_Integracao_SMS,
                                            configGerais.Senha_Integracao_SMS
                                        ),
                                        listaSMS
                                    );
                            break;
                        default:
                            throw new Exception("Integração não informada ou não implementada");
                    }

                    //if (request.Salvar_Em_Banco)
                    await repoSMS.SaveOrUpdate(listaSMS);
                }                

                if (!retornoIntegracao.Sucesso)
                {
                    throw new Exception(retornoIntegracao.MensagemErro);
                }                

                retorno = retornoIntegracao;
                if(lista_mal_formatados.Count > 0)
                {
                    retorno.Retorno.Concat(lista_mal_formatados);
                }

                return new ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel> { data = retorno, ok = true };
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponseObj<Sinapse.Integracao.SMS.Models.EnviarLoteSMS_ResponseModel> { error = ex.Message };

            }
        }

        internal Boolean CelularIsValido(string num_celular)
        {
            var num_celular_formatado = Regex.Replace(num_celular, @"\s+", String.Empty)
                .Replace("(", String.Empty)
                .Replace(")", String.Empty)
                .Replace("-", String.Empty)
                .Replace(" ", String.Empty)              
                .Trim();

            return num_celular_formatado.Length == 11;            
        }

        internal String FormatarNumCelular(string num_celular) => Regex.Replace(num_celular, @"[^0-9]", String.Empty).Trim();
    }
}