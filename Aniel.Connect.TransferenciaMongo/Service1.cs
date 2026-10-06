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
using FluentNHibernate.Conventions;
using Aniel.Connect.ControllersWebApi.T5;

namespace Aniel.Connect.ExecucaoService
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;
        private Int32 horaIni;
        private Int32 horaFim;
        private Int32 intervalo;
        private DateTime? ultimaExecucaoDiaria = null;
        //private DateTime? ultimaExecucaoSemanal = null;

        public Service1()
        {
            CanStop = true;
            AutoLog = false;

            InitializeComponent();

            if (!EventLog.SourceExists("TransferenciaMongo"))
                EventLog.CreateEventSource("TransferenciaMongo", "TransferenciaMongo");
            log.Source = "TransferenciaMongo";
        }

        protected override void OnStart(string[] args)
        {
            if (ConfigurationManager.AppSettings["horaExecucaoInicio"] == null)
                throw new Exception("horaExecucaoInicio Não informado no appconfig");
            if (ConfigurationManager.AppSettings["horaExecucaoFim"] == null)
                throw new Exception("horaExecucaoFim Não informado no appconfig");
            if (ConfigurationManager.AppSettings["intervaloMinutos"] == null)
                throw new Exception("intervaloMinutos Não informado no appconfig");

            horaIni = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoInicio"]).Hour;
            horaFim = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoFim"]).Hour;
            intervalo = Convert.ToInt32(ConfigurationManager.AppSettings["intervaloMinutos"]);

            //try
            //{
            //    using (ISession session = HibernateUtil.GetSession())
            //    {

            //    }

            //    using (ISession session = HibernateUtil.GetSessionT5())
            //    {

            //    }
            //}
            //catch (Exception ex)
            //{
            //    CONSTANTES.log4Net.Error(ex);
            //}

            log.WriteEntry("Iniciando Serviço.");

            _timer = new System.Timers.Timer
            {
                //_timer.Interval = TimeSpan.FromHours(1).TotalMilliseconds;
                Interval = TimeSpan.FromMinutes(intervalo).TotalMilliseconds
            };
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            _timer.Start();
            _timer.Enabled = true;

            OnTimer(_timer, null);
            log.WriteEntry("Serviço Iniciado com Sucesso.");
            CONSTANTES.log4Net.Debug("Serviço Iniciado com Sucesso.");
        }

        protected override void OnStop()
        {
            log.WriteEntry("Serviço Parado com Sucesso.");
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


                if ((DateTime.Now.Hour >= horaIni) && (DateTime.Now.Hour <= horaFim))
                {

                    var api = new Log_LocalizacaoAnielApiController();
                    Console.WriteLine("iniciar transferencia");
                    api.ServicoTransferenciaFireBirdMongo(CONSTANTES.DataInicio,CONSTANTES.DataFim).Wait();
                    //Boolean retorno = Executar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} {ex.InnerException?.Message} {ex.StackTrace}");

                log.WriteEntry(String.Format("Falha na Execução: {0} {1}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message));
                CONSTANTES.log4Net.ErrorFormat("Falha na Execução: {0} {1} {2}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message, ex.StackTrace);
            }
            finally
            {
                _timer.Enabled = true;
            }

        }

        private void log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            OnStop();
        }
    }
}
