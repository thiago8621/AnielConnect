using Aniel.Connect.ConnectBusiness.Log;
using Aniel.Connect.Repository.Repository.Log;
using Aniel.Connect.Repository.T5.Funcionario;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Aniel.Connect.Tasks.LogsAgendados
{
    public class LogsTask : BackgroundTask
    {       
        public async Task<bool> EnviarEmailLogs()
        {
            using(var session = HibernateUtil.GetSessionLog())
            {
                try
                {
                    ILogRepository logRepository = new LogRepository(session);
                    LogBusiness logBusiness = new LogBusiness(logRepository);
                    string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                    var crvFile = Path.Combine(path, "Arquivos", "LogIntegracaoOS", $"log{DateTime.Now.Date.ToString("dd-MM-yyyy")}.csv");

                    var obterLogsDia = await logBusiness.ObterLogsDia();

                    if (obterLogsDia == null){
                        CONSTANTES.log4Net.Info($"Não foram logs gerados no {DateTime.Now.Date}");
                        return false;
                    }
                    
                  //  var writeCSVArquivo = CSVUtil.WriteCSVArquivo(path, obterLogsDia);

                    //if (!writeCSVArquivo){
                    //    CONSTANTES.log4Net.Error($"Não foi possível criar o arquivo CSV.");
                    //    return false;
                    //}

                    await EnviarEmail(crvFile);
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error($"{ex.Message}");
                    return false;
                }
            }                   
            return true;
        }


        private async Task EnviarEmail(string filePath)
        {
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IFuncionarioAnielRepository repoFunc = new FuncionarioAnielRepository(sessionT5);
                    var emailEnvio = Convert.ToString(ConfigurationManager.AppSettings["Email_De"]);
                    var senha = Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
                    var smtp = Convert.ToString(ConfigurationManager.AppSettings["Email_Smtp"]);
                    var porta = Convert.ToInt32(ConfigurationManager.AppSettings["Email_Porta"]);
                    var file = new FileInfo(filePath);
                    List<string> emails = await repoFunc.GetEmailsFuncionario("50");
                    var corpoEmail = "Olá! Segue em anexo o relatório de logs com os erros obtidos durante " +
                                     "a integração automática, integração manual e atualização entre Voalle e Aniel.";

                    var emailService = new EmailUtil(emailEnvio, senha, smtp, porta);
                    emailService.Anexar(filePath);
                    emailService.EnviarMensagem(String.Join(",", emails), "LOGS INTEGRAÇÃO DE OS", corpoEmail,
                                                Convert.ToString(ConfigurationManager.AppSettings["Email_UsaSsl"]) == "S"); 
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"Não foi possível enviar arquivo CSV de logs via email. Exception: {ex.Message}");
            }
        }
    }
}