using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Sinapse.DAO.service;
using Hangfire;
using Hangfire.Dashboard;
using Aniel.Connect.Tasks;
using System;
using Aniel.Connect.Tasks.T5;
using Aniel.Connect.Tasks.LogsAgendados;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.repository;
using Microsoft.Owin.Cors;

[assembly: OwinStartupAttribute(typeof(Aniel.Connect.Startup))]
[assembly: OwinStartupAttribute(typeof(Aniel.Connect.Socket.ChatHub.ChatHub))]
namespace Aniel.Connect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            HibernateService.GetSessionFactory();
            HibernateService.GetSessionFactoryLog();
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            //app.MapSignalR(hubConfiguration);

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(hubConfiguration);
            });




            //RecurringJob.AddOrUpdate(() => new AtualizarPortaCaixaVendaTask().AtualizarPortaCaixaVenda(null), Cron.Daily(1));
            //Rodar essa rotina no ambiente de DEBUG pode gerar Notificacoes Duplicadas , interessante que rode apenas no ambiente de producao
#if !DEBUG
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new IDashboardAuthorizationFilter[]
                {
                        new AuthorizationFilter {
                            Users = "admin, administrador"
                        }
                }
            });

            var cronExpression = string.Empty;
            using (var session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            {
                Configuracoes_Gerais config = repoConfig.selectFirst().Result;

                if (!String.IsNullOrEmpty(Convert.ToString(config.HorarioCorteRoteirizacaoDashBoard)))
                {

                    var valorHora = config.HorarioCorteRoteirizacaoDashBoard.Value;
                    int hora = valorHora.Hour;
                    int minuto = valorHora.Minute;

                    cronExpression = $"{minuto} {hora} * * *";
                }
                else
                {
                    cronExpression = $"00 08 * * *";
                }

            }

            if (CONSTANTES.AgendarVerificacaoAgendamentoTask == "S")
                RecurringJob.AddOrUpdate(() => new AgendarVerificacaoAgendamentoTask().AgendarVerificacaoAgendamento(), Cron.Hourly(1));

            if (CONSTANTES.BloquearContratoTask == "S")
            {
                RecurringJob.AddOrUpdate("jobBloquearContrato", () => new BloquearContratoTask().BloquearContrato(), "0 1 * * *", TimeZoneInfo.Local);
            }
            if (CONSTANTES.AtualizarVeiculoTask == "S")
            {
                RecurringJob.AddOrUpdate("jobAtualizarVeiculoMeiaNoite", () => new AtualizarVeiculoTask().AtualizarVeiculo(), "0 0 * * *", TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate("jobAtualizarVeiculoMeiodia", () => new AtualizarVeiculoTask().AtualizarVeiculo(), "0 12 * * *", TimeZoneInfo.Local);
            }
            if (CONSTANTES.PreencherQuilometragemTask == "S")
            {
                RecurringJob.AddOrUpdate(() => new EquipeQuilometragemTask().PreencherQuilometragem(null, null, true), Cron.Daily(1));
            }
            if (CONSTANTES.UtilizaBaixaOSAutomatica == "S")
            {
                RecurringJob.AddOrUpdate(() => new BaixarAutomaticaOS().GerarBaixaAutoOs(), Cron.Daily());
            }

            if (CONSTANTES.UtilizaEnvioLogsEmail == "S")
            {
                RecurringJob.AddOrUpdate(() => new LogsTask().EnviarEmailLogs(), "0 23 * * *");
            }

            if (CONSTANTES.RoteirizacaoSlaTask == "S")
            {
                RecurringJob.AddOrUpdate("jobRoteirizacaoSla", () => new RoteirizarOsSlaTask().RoteirizarOsSla(), "*/2 * * * *", TimeZoneInfo.Local);
            }

            if (CONSTANTES.RoteirizacaoEncaixe == "S")
            {
                RecurringJob.AddOrUpdate("jobRoteirizacaoEncaixe", () => new RoteirizacaoEncaixeTask().RoteirizacaoEncaixe(), cronExpression, TimeZoneInfo.Local);
            }

            if (CONSTANTES.UtilizaOla)
            {
                RecurringJob.AddOrUpdate(() => new IntegrarOlaTask().InserirClientesPassivosOla(), Cron.Hourly(1));
            }
            if (CONSTANTES.UtilizarTransAut == "S")
            {
                RecurringJob.AddOrUpdate(() => new ConsultaTempoTransferenciaBotTask().ConsultaTempoTransferenciaBot(), "*/1 * * * *", TimeZoneInfo.Local);
            }
            if (CONSTANTES.FinalizarAtendimentoWhatsApp == "S")
            {
                RecurringJob.AddOrUpdate(() => new ConsultaTempoTransferenciaBotTask().ConsultaTempoAtendimentoAberto24h(), "*/1 * * * *", TimeZoneInfo.Local);
            }

                if(CONSTANTES.EnviarSMSAlertaSOS == "S")
                {
                    RecurringJob.AddOrUpdate(() => new NotificacaoTask().EnviarSMSAlertaSOS(), "* * * * *", TimeZoneInfo.Local);
                }

            //*Notifições de vendas canceladas
            RecurringJob.AddOrUpdate("jobVendasCanceladas17Horas", () => new VendasCanceladasTask().VendasCanceladas(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("jobVendasCanceladasMeioDia", () => new VendasCanceladasTask().VendasCanceladas(), "0 17 * * *", TimeZoneInfo.Local);

            //*Notifições de vendas aprovadas
            RecurringJob.AddOrUpdate("jobVendasAprovadas7Horas", () => new VendasAprovadasTask().VendasAprovadas(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("jobVendasAprovadasMeioDia", () => new VendasAprovadasTask().VendasAprovadas(), "0 17 * * *", TimeZoneInfo.Local);

            //*Notifições Novos Prospects
            RecurringJob.AddOrUpdate("jobNovosProspects17Horas", () => new NovosProspectsTask().NovosProspects(), "0 12 * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("jobNovosProspectsMeioDia", () => new NovosProspectsTask().NovosProspects(), "0 17 * * *", TimeZoneInfo.Local);

            //*Notifições Meta Dia // Superada e Não Batida
            RecurringJob.AddOrUpdate("jobMetaMeiaNoite", () => new MetasVendasTask().MetaVendaAsync(), "0 0 * * *", TimeZoneInfo.Local);

            //*Notifições CampanhaEncerrando
            RecurringJob.AddOrUpdate("jobCampanhaEncerrandoMeiaNoite", () => new CampanhaTask().CampanhaEncerrando(), "0 0 * * *", TimeZoneInfo.Local);

            //*Notifições DiasSemContato
            RecurringJob.AddOrUpdate("jobDiasSemContatoMeiaNoite", () => new DiasSemContatoTask().DiasSemContato(), "0 0 * * *", TimeZoneInfo.Local);
            //BackgroundJob.Enqueue(() => new AtualizarPortaCaixaVendaTask().AtualizarPortaCaixaVenda(null));
            RecurringJob.AddOrUpdate(() => new EquipeQuilometragemTask().PreencherQuilometragem(null, null, true), Cron.Daily(1));
#endif



        }
    }
}
