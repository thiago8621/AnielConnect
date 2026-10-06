using Aniel.Connect.ConnectBusiness.SMS;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.SMS;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Sinapse.Core;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.Tasks
{
    public class NotificacaoTask : BackgroundTask
    {
        public async Task EnviarSMSAlertaSOS()
        {
            try 
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config_gerais = await repoConfigGerais.selectFirst();

                    if (string.IsNullOrEmpty(CONSTANTES.URLPortalApiAniel))
                        throw new EAnielException("Url do API Aniel não configurada na TB_CONFIG_URL.");

                    if (config_gerais.Tempo_Confirmacao_Ciencia_SOS.HasValue)
                    {
                        using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                        using (var repositoryMongo = new RepositoryMongo<AtividadeMongo>(conexaoMongo))
                        {
                            DateTime dataHoraAtual = DateTime.Now;

                            var alertasPendenteSMS = await repositoryMongo.FindAll(p => p.DataHoraEnvioNotificacao != null &&
                                                                p.DataHoraEnvioNotificacao >= dataHoraAtual.Date &&
                                                                p.DataHoraEnvioNotificacao <= dataHoraAtual.Date.AddDays(1).AddSeconds(-1) &&
                                                                p.ConfirmadoCiencia == false &&
                                                                p.SmsEnviado == false &&
                                                                (p.Resposta == EnumRespostaSOS.Sem_Resposta || p.Resposta == EnumRespostaSOS.SOS));                            

                            foreach (var alerta in alertasPendenteSMS)
                            {
                                try
                                {           
                                    if(alerta.DataHoraEnvioNotificacao.HasValue && dataHoraAtual.Subtract(alerta.DataHoraEnvioNotificacao.Value.ToLocalTime()).Minutes > config_gerais.Tempo_Confirmacao_Ciencia_SOS)
                                    {
                                        ISmsRepository repoSms =
                                        new SmsRepository(
                                            CONSTANTES.URLPortalApiAniel + "/api",
                                            config_gerais.Usuario_APIANIEL,
                                            config_gerais.Senha_APIANIEL,
                                            config_gerais.Token_APIANIEL
                                        );

                                        var smsBusiness = new IntegracaoSmsBusiness(repoSms);

                                        var repositoryEquipeAniel = new Repository.T5.Equipe.EquipeAnielRepository(sessionT5);
                                        var equipeAnielBusiness = new EquipeAnielBusiness(repositoryEquipeAniel,null,null);
                                        var equipeTecnico = equipeAnielBusiness.GetByEquipe(alerta.Equipe);

                                        string mensagemSms =
                                            String.Format(alerta.Resposta == EnumRespostaSOS.Sem_Resposta ?
                                                "O técnico {0} excedeu a tolerância de avisos. Entre em contato com o mesmo imediatamente."
                                                :
                                                "O técnico {0} solicitou ajuda. Entre em contato com o mesmo imediatamente.",
                                                alerta.Nome
                                            );

                                        try
                                        {
                                            if (!string.IsNullOrEmpty(equipeTecnico.Supervisor?.Telefone_Celular))
                                            {
                                                smsBusiness.EnviarSms(equipeTecnico.Supervisor?.Telefone_Celular, mensagemSms);
                                                alerta.SmsEnviado = true;
                                            }                                            
                                        }
                                        catch(Exception ex)
                                        {
                                            log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"[EnviarSMSAlertaSOS] - Erro ao enviar SMS de alerta do técnico para o supervisor {equipeTecnico.Supervisor?.Num_Fre} - {equipeTecnico.Supervisor?.Nome}. UUID Notificacao: {alerta.UuidNotificacao} | Mensagem Erro: {ex.Message}", ex);
                                        }
                                        
                                        try
                                        {
                                            var repositoryFuncionarioAniel = new Repository.T5.Funcionario.FuncionarioAnielRepository(sessionT5);
                                            var equipeMonitor = repositoryFuncionarioAniel.GetByNumFre(equipeTecnico.Monitor);

                                            if (!string.IsNullOrEmpty(equipeMonitor?.Telefone_Celular))
                                            {
                                                smsBusiness.EnviarSms(equipeMonitor.Telefone_Celular, mensagemSms);
                                                alerta.SmsEnviado = true;
                                            }                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"[EnviarSMSAlertaSOS] - Erro ao enviar SMS de alerta do técnico para o monitor {equipeTecnico.Monitor}. UUID Notificacao: {alerta.UuidNotificacao} | Mensagem Erro: {ex.Message}", ex);
                                        }  
                                        
                                        if(alerta.SmsEnviado)
                                        {
                                            await repositoryMongo.Update(p => p.UuidNotificacao == alerta.UuidNotificacao, alerta);
                                        }
                                    }                                                                                                          
                                }
                                catch(Exception ex)
                                {
                                    log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"[EnviarSMSAlertaSOS] - Erro ao enviar SMS de alerta do técnico. UUID Notificacao: {alerta.UuidNotificacao} | Mensagem Erro: {ex.Message}", ex);
                                }
                            }
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"[EnviarSMSAlertaSOS] - {ex.Message}", ex);
            }
        }
    }
}