using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Aniel.Connect.Core.Service;
using NHibernate;
using NHibernate.Criterion;
using Renci.SshNet;
using Sinapse.DAO.repository;
using System;
using Aniel.T5.Model.enumerado;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using Sinapse.Model.enumerados;
using Newtonsoft.Json;
using Sinapse.Util;
using Aniel.Connect.ControllersWebApi;
using System.IO;
using System.Reflection;

namespace Aniel.Connect.EmailService
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;
        private Int32 horaIni;
        private Int32 horaFim;
        private Double intervalo;
        private DateTime? ultimaExecucaoDiaria = null;
        //private DateTime? ultimaExecucaoSemanal = null;

        public Service1()
        {
            CanStop = true;
            AutoLog = false;

            InitializeComponent();
            //if (!EventLog.SourceExists("AnielConnectEmailService"))
            //    EventLog.CreateEventSource("AnielConnectEmailService", "AnielConnectEmailService");
            //log.Source = "AnielConnectEmailService";

        }

        protected override void OnStart(string[] args)
        {
            CONSTANTES.log4Net.Info("INICIO DO SERVICO");
            if (ConfigurationManager.AppSettings["horaExecucaoInicio"] == null)
                throw new Exception("horaExecucaoInicio Não informado no appconfig");
            if (ConfigurationManager.AppSettings["horaExecucaoFim"] == null)
                throw new Exception("horaExecucaoFim Não informado no appconfig");
            if (ConfigurationManager.AppSettings["intervaloMinutos"] == null)
                throw new Exception("intervaloMinutos Não informado no appconfig");

            horaIni = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoInicio"]).Hour;
            horaFim = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoFim"]).Hour;
            intervalo = Convert.ToDouble(ConfigurationManager.AppSettings["intervaloMinutos"]);

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {

                }

                using (ISession session = HibernateUtil.GetSessionT5())
                {

                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }

            //log.WriteEntry("Iniciando Serviço.");

            _timer = new System.Timers.Timer
            {
                //_timer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
                Interval = TimeSpan.FromMinutes(intervalo).TotalMilliseconds
            };
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            _timer.Start();
            _timer.Enabled = true;

            OnTimer(_timer, null);
            //log.WriteEntry("Serviço Iniciado com Sucesso.");
            CONSTANTES.log4Net.Debug("Serviço Iniciado com Sucesso.");
        }

        protected override void OnStop()
        {
            //log.WriteEntry("Serviço Parado com Sucesso.");
            CONSTANTES.log4Net.Debug("Serviço Parado com Sucesso.");
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            List<Task> listaProcessos = new List<Task>();
            _timer.Enabled = false;

            try
            {
                HibernateUtil.DestroySessionFactory();
                HibernateUtil.DestroySessionFactoryLog();
                HibernateUtil.DestroySessionFactoryT5();

                if (ultimaExecucaoDiaria?.Date != DateTime.Now.Date)
                {

                    ultimaExecucaoDiaria = DateTime.Now;


                    Task.WaitAll(listaProcessos.ToArray());


                }

                if ((DateTime.Now.Hour >= horaIni) && (DateTime.Now.Hour <= horaFim))
                {
                    Boolean retorno = Executar();
                }
            }
            catch (Exception ex)
            {
                //log.WriteEntry(String.Format("Falha na Execução: {0} {1}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message));
                CONSTANTES.log4Net.ErrorFormat("Falha na Execução: {0} {1} {2}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message, ex.StackTrace);
            }
            finally
            {
                _timer.Enabled = true;
            }

        }

        public Boolean Executar()
        {
            String arquivoOsDigital = $@"{CONSTANTES.CaminhoFisico}\ultimaexecucaoosdigital.txt";
            DateTime proximaExecucao = DateTime.Now;
            DateTime? ultimaExecucao = null;
            DateTime? ultimaExecucaoOsDigital = null;

            if (File.Exists(arquivoOsDigital))
            {
                ultimaExecucaoOsDigital = String.IsNullOrEmpty(File.ReadAllText(arquivoOsDigital).TrimEnd()) ? DateTime.Today : Convert.ToDateTime(File.ReadAllText(arquivoOsDigital));
            }
            else
            {
                File.Create(arquivoOsDigital);
                ultimaExecucaoOsDigital = DateTime.Now.Date;
            }

            using (var api = new EmailServiceApiController())
            {
                api.log4net = CONSTANTES.log4Net;
                api.caminhoFisico = CONSTANTES.CaminhoFisico;
                api.importarOsDigitalApenasPorData = CONSTANTES.IMPORTAR_OSDIGITAL_APENASDATA;

                if (CONSTANTES.IMPORTAR_ATENDIMENTO == "S")
                {
                    api.ImportarAtendimento(CONSTANTES.CaminhoFisicoConnect, CONSTANTES.URLPortalConnect).Wait();
                    api.EnviarEmailAtendimento(CONSTANTES.CaminhoFisicoConnect).Wait();
                }

                if (CONSTANTES.IMPORTAR_OSDIGITAL == "S") 
                {
                    var configEmails = JsonConvert.DeserializeObject<List<ConfigEmailOsDigital>>(CONSTANTES.EMAILS_OSDIGITAL);
                    foreach (var item in configEmails)
                    {
                        api.ImportarOsDigital(ultimaExecucaoOsDigital.GetValueOrDefault(DateTime.Now.Date), item.OSDIGITAL_SERVIDORIMAP, item.OSDIGITAL_EMAIL, item.OSDIGITAL_SENHA, item.OSDIGITAL_USASSL == "S", Convert.ToInt16(item.OSDIGITAL_PORTA));
                    }
                    
                }

            }


            //Task.WaitAll(listaProcessos.ToArray());


            return true;
        }

        private void log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
