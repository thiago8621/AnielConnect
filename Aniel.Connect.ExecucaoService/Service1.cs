using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ExecucaoService.OlaCliente;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using FluentNHibernate.Conventions;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using Aniel.Connect.ExecucaoService.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Aniel.Connect.ExecucaoService.Data.Model;

namespace Aniel.Connect.ExecucaoService
{
    public partial class Service1 : ServiceBase
    {
        private Timer _timer;

        private int horaIni;
        private int horaFim;
        private int intervalo;

        private int qtdMaximaDiasReincidencia;

        private DateTime? ultimaExecucaoDiaria = null;
        private DateTime ultimaExecucaoReincidencia = DateTime.MinValue;

        //private DateTime? ultimaExecucaoSemanal = null;

        /// <summary>
        ///     Classe para acessar os dados persistentes sobre o serviço
        /// </summary>
        private IServiceDataAccessor _serviceDataAccessor;

        public Service1()
        {
            CanStop = true;
            AutoLog = false;

            InitializeComponent();

            if (!EventLog.SourceExists("AnielConnectExecucaoService"))
                EventLog.CreateEventSource("AnielConnectExecucaoService", "AnielConnectExecucaoService");
            log.Source = "AnielConnectExecucaoService";
        }

        protected override void OnStart(string[] args)
        {
            if (ConfigurationManager.AppSettings["horaExecucaoInicio"] == null)
                throw new Exception("horaExecucaoInicio Não informado no appconfig");
            if (ConfigurationManager.AppSettings["horaExecucaoFim"] == null)
                throw new Exception("horaExecucaoFim Não informado no appconfig");
            if (ConfigurationManager.AppSettings["intervaloMinutos"] == null)
                throw new Exception("intervaloMinutos Não informado no appconfig");
            if (ConfigurationManager.AppSettings["qtdMaximaDiasReincidencia"] == null)
                throw new Exception("qtdMaximaDiasReincidencia Não informado no appconfig");
            if (ConfigurationManager.AppSettings["dataImplantacaoReincidencia"] == null)
                throw new Exception("dataImplantacaoReincidencia Não informado no appconfig");

            this.horaIni = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoInicio"]).Hour;
            this.horaFim = Convert.ToDateTime(ConfigurationManager.AppSettings["horaExecucaoFim"]).Hour;
            this.intervalo = Convert.ToInt32(ConfigurationManager.AppSettings["intervaloMinutos"]);

            this.qtdMaximaDiasReincidencia = Convert.ToInt32(ConfigurationManager.AppSettings["qtdMaximaDiasReincidencia"]);

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

            log.WriteEntry("Iniciando Serviço.");

            var serializer = new JsonSerializer();

            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            this._serviceDataAccessor = new JsonServiceDataAccessor(serializer);

            this._timer = new System.Timers.Timer
            {
                Interval = TimeSpan.FromMinutes(this.intervalo).TotalMilliseconds
            };

            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            this._timer.Start();
            this._timer.Enabled = true;

            OnTimer(this._timer, null);

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
            this._timer.Enabled = false;

            try
            {
                HibernateUtil.DestroySessionFactory();
                HibernateUtil.DestroySessionFactoryLog();
                HibernateUtil.DestroySessionFactoryT5();

                if (this.ultimaExecucaoDiaria?.Date != DateTime.Now.Date)
                {
                    this.ultimaExecucaoDiaria = DateTime.Now;

                    listaProcessos.Add(Task.Run(() => Inativacao_Campanha_Expirada()));
                    listaProcessos.Add(Task.Run(() => InativarPlanoProdutoForaValidade()));

                    Task.WaitAll(listaProcessos.ToArray());
                }

                #region Lançamento de reincidência

                ServiceData data = this._serviceDataAccessor.GetServiceData();

                if (data == null)
                    data = new ServiceData();

                if (!data.UltimaDataProcessadaReincidencia.HasValue)
                {
                    // Caso nenhum dia tenha sido processado pela rotina de lançamento retroativo de reincidência. A última data processada
                    // é inicializada com a data de implantação da trigger para lançamento de reincidência automático definida no App.config
                    data.UltimaDataProcessadaReincidencia = DateTime.Parse(ConfigurationManager.AppSettings["dataImplantacaoReincidencia"]);
                    this._serviceDataAccessor.WriteServiceData(data);
                }

                if (this.ultimaExecucaoReincidencia.Date != DateTime.Now.Date)
                {
                    this.ultimaExecucaoReincidencia = DateTime.Now.Date;
                    Task.Run(() => LancarServicosReincidentesRetroativos());
                }

                #endregion

                if ((DateTime.Now.Hour >= horaIni) && (DateTime.Now.Hour <= horaFim))
                {
                    Boolean retorno = Executar();
                }
            }
            catch (Exception ex)
            {
                log.WriteEntry(String.Format("Falha na Execução: {0} {1}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message));
                CONSTANTES.log4Net.ErrorFormat("Falha na Execução: {0} {1} {2}", ex.Message, ex.InnerException == null ? String.Empty : ex.InnerException.Message, ex.StackTrace);
            }
            finally
            {
                _timer.Enabled = true;
            }

        }

        public Boolean Executar()
        {
            try
            {
                List<Task> listaProcessos = new List<Task>
                {
                    Task.Run(() => Conexao_OLA()),
                    Task.Run(() => ApiConnect_AtualizarTodos())
                };

                Task.WaitAll(listaProcessos.ToArray());
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
            }

            return true;
        }

        private async Task Inativacao_Campanha_Expirada()
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha>(session))
                    {
                        IList<Campanha> modelo = repository.select(p => p.Data_Final != null && p.Data_Final.Value < DateTime.Now).Result;

                        foreach (var item in modelo)
                        {
                            item.Status = Sinapse.Model.enumerados.Status_Default.Inativo;
                            await repository.SaveOrUpdate(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
            }
        }

        private void ApiConnect_AtualizarTodos()
        {
            try
            {
                if (!String.IsNullOrEmpty(CONSTANTES.UrlConnectAPI))
                {
                    CONSTANTES.log4Net.Info("AtualizaTodos ConnectAPI - Iniciando");
                    new PostHttp(CONSTANTES.log4Net).ExecuteAsync($"{CONSTANTES.UrlConnectAPI}/api/Jobs/v1/AtualizaTodos");
                    CONSTANTES.log4Net.Info("AtualizaTodos foi chamado no ConnectAPI");
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        ///     Lança retroativamente os serviços reincidentes criados antes da implantação da trigger que registra
        ///     reincidência automaticamente na TB_REINCIDENCIA_DOCPROD
        /// </summary>
        private void LancarServicosReincidentesRetroativos()
        {
            try
            {
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repoDocProd =
                        new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                    IReincidenciaDocProdAnielRepository repoReincidencia =
                        new ReincidenciaDocProdAnielRepository(sessionT5Nh);

                    var docProdBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                    var reincidenciaBusiness = new ReincidenciaDocProdAnielBusiness(repoReincidencia);

                    ServiceData serviceData = this._serviceDataAccessor.GetServiceData();

                    DateTime? dataPrimeiroServico = docProdBusiness.GetDataCriacaoPrimeiroServico();

                    if (!dataPrimeiroServico.HasValue)
                        throw new Exception("Não foi identificado nenhum serviço no sistema");

                    DateTime dataInicio = dataPrimeiroServico.Value.Date;
                    DateTime dataFim;

                    DateTime? dataPrimeiraReincidencia = reincidenciaBusiness.GetDataPrimeiraReincidencia();

                    if (!dataPrimeiraReincidencia.HasValue ||
                        dataPrimeiraReincidencia.Value >= serviceData.UltimaDataProcessadaReincidencia.Value)
                    {
                        // Se não houver reincidencia registrada ou se houver reincidência registrada somente após
                        // o último dia processado pelo serviço de reincidência, o período de lançamento
                        // de reincidência retroativa irá até ao último dia processado
                        dataFim = serviceData.UltimaDataProcessadaReincidencia.Value;
                    }
                    else
                    {
                        // Se houver reincidência lançada retroativamente com data anterior ao último dia processado
                        // pelo serviço de reincidência, o período de lançamento de reincidência retroativa irá até a
                        // data da última reincidência gerada retroativamente
                        dataFim = dataPrimeiraReincidencia.Value;
                    }

                    // Se a diferença entre a última data processada pelo serviço de reincidência e a data de criação do
                    // primeiro serviço lançado for maior do que a quantidade máxima de dias que podem ser processados por vez
                    // (configurado no App.config) a data de início do periodo que será processado é movida para
                    // dentro do limite em relação à última data processada.
                    if ((dataFim - dataInicio).TotalDays > this.qtdMaximaDiasReincidencia)
                        dataInicio = dataFim.AddDays(this.qtdMaximaDiasReincidencia * -1);

                    IEnumerable<Documento_ProducaoAniel> servicosPeriodo =
                        docProdBusiness
                            .GetServicosCriadosPeriodo(dataInicio, dataFim)
                            .OrderByDescending(p => p.Dia);

                    foreach (Documento_ProducaoAniel docProd in servicosPeriodo)
                    {
                        if (docProd.Status != StatusExecucaoAniel.Cancelado)
                        {
                            ChavesDocumentoProducao osReincidente =
                                docProdBusiness
                                    .GetOSReincidente(
                                        docProd.CodCt,
                                        docProd.Projeto,
                                        docProd.Num_Obra,
                                        docProd.Num_Doc,
                                        docProd.Dia.Value
                                    );

                            if (osReincidente != null)
                            {
                                var reincidencia = new ReincidenciaDocProdAniel
                                {
                                    CodCt = docProd.CodCt,
                                    Projeto = docProd.Projeto,
                                    NumObra = docProd.Num_Obra,
                                    NumDoc = docProd.Num_Doc,

                                    CodCtAnterior = osReincidente.Codct,
                                    ProjetoAnterior = osReincidente.Projeto,
                                    NumObraAnterior = osReincidente.NumObra,
                                    NumDocAnterior = osReincidente.NumDoc,

                                    Data = docProd.Dia.Value
                                };

                                reincidenciaBusiness.Gravar(reincidencia);

                                CONSTANTES.log4NetReincidencia.Info(
                                    String.Format(
                                        "Reincidencia identificada - CODCT_ANTERIOR: {0} PROJETO_ANTERIOR: {1} NUM_DOC_ANTERIOR: {2} NUM_OBRA_ANTERIOR: {3}\n" +
                                        "CODCT: {4} PROJETO: {5} NUM_DOC: {6} NUM_OBRA: {7}",
                                        osReincidente.Codct,
                                        osReincidente.Projeto,
                                        osReincidente.NumDoc,
                                        osReincidente.NumObra,

                                        docProd.CodCt,
                                        docProd.Projeto,
                                        docProd.Num_Doc,
                                        docProd.Num_Obra
                                    )
                                );
                            }
                        }

                        if (docProd.Dia.Value != serviceData.UltimaDataProcessadaReincidencia.Value)
                        {
                            CONSTANTES.log4NetReincidencia.Info(String.Format("Dia {0} processado", docProd.Dia.Value.ToString("dd/MM/yyyy")));

                            serviceData.UltimaDataProcessadaReincidencia = docProd.Dia.Value;
                            this._serviceDataAccessor.WriteServiceData(serviceData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4NetReincidencia.Error(ex.Message, ex.InnerException);
            }
        }

        private async Task InativarPlanoProdutoForaValidade()
        {
            try
            {
                using (PlanoProdutoApiController planoProdutoApi = new PlanoProdutoApiController())
                {
                    await planoProdutoApi.InativarPlanoProdutoForaValidade();
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
            }
        }

        private void Conexao_OLA()
        {
            using (LogExecucao logExecucao = new LogExecucao("LOG_SERVICO_OLA", $"{CONSTANTES.CaminhoFisico}/log"))
            {
                try
                {
                    using (ISession session = HibernateUtil.GetSession())
                    using (BusinessClientesOla ola = new BusinessClientesOla(session))
                    {
                        ola.AtualizarProvedor();

                        List<Cliente> clientes = ola.GetClientes();
                        if (clientes.IsNotEmpty())
                        {
                            ola.atualizarClientesOla(clientes);
                            ola.clientesAtualizados();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logExecucao.RegistraLog($"Message - {ex.Message}\nInnerException - {ex.InnerException}");
                    CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
                }

                logExecucao.SalvarLog();
            }
        }

        //private async Task Conexao_OLT()
        //{
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {

        //            IList<Rack> racks = await new Conexao<Rack>(session).select();


        //            foreach (var rack in racks)
        //            {
        //                OLT olt = rack.OLTs;
        //                //await verificaAtualizacaoRadius(session, rack.OLTs.Radius, olt);
        //                await VerificaAtualizaçãoRack(session, rack, olt);

        //                foreach (var subRack in rack.SubRack)
        //                {
        //                    await VerificaAtualizaçãoSubRack(session, rack, subRack, olt);
        //                    foreach (var slot in subRack.Slots)
        //                    {

        //                        await VerificaAtualizaçãoSlot(session, rack, subRack, slot, olt);
        //                        foreach (var pon in slot.pons)
        //                        {
        //                            await verificaAtualizaçãoPon(session, rack, subRack, slot, pon, olt);
        //                            foreach (var pos in pon.Posicao)
        //                            {
        //                                await verificaAtualizaçãoONTONU(session, rack, subRack, slot, pon, pos, olt);
        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
        //    }

        //}

        //Configuração OLT *********************************************************************************************************************************
        #region
        //private async Task verificaAtualizacaoRadius(ISession session, RadiusObj radius, OLT olt)
        //{
        //    if (radius.Atualizar)
        //    {
        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {

        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }
        //            var comandoRadius = new StringBuilder();
        //            comandoRadius.Append("configure system security radius auth-server " + radius.Descricao + " router-instance base ip-address " + radius.IP + " port " + radius.Porta.ToString() + " secret " + olt.Secret);

        //            var result = client.RunCommand(comandoRadius.ToString());
        //        }
        //        radius.Atualizar = false;
        //        await session.UpdateAsync(radius);
        //    }

        //}

        //private async Task VerificaAtualizaçãoRack(ISession session, Rack rack, OLT olt)
        //{
        //    if (rack.Atualizar)
        //    {
        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {

        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }
        //            var comandoRack = new StringBuilder();
        //            comandoRack.Append("configure equipment rack " + rack.RackID);
        //            var result = client.RunCommand(comandoRack.ToString());
        //        }
        //        rack.Atualizar = false;
        //        await session.UpdateAsync(rack);
        //    }

        //}

        //private async Task VerificaAtualizaçãoSubRack(ISession session, Rack rack, SubRack subRack, OLT olt)
        //{
        //    if (subRack.Atualizar)
        //    {
        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {

        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }
        //            var comandoSubRack = new StringBuilder();
        //            comandoSubRack.Append("configure equipment shelf " + rack.RackID + "/" + subRack.SubRackID);

        //            var result = client.RunCommand(comandoSubRack.ToString());
        //        }
        //        subRack.Atualizar = false;
        //        await session.UpdateAsync(subRack);
        //    }

        //}


        //private async Task VerificaAtualizaçãoSlot(ISession session, Rack rack, SubRack subRack, Slot slot, OLT olt)
        //{
        //    if (slot.Atualizar)
        //    {
        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {

        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }
        //            var comandoSlot = new StringBuilder();
        //            comandoSlot.Append("configure equipment slot " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID);
        //            var result = client.RunCommand(comandoSlot.ToString());
        //        }
        //        slot.Atualizar = false;
        //        await session.UpdateAsync(slot);
        //    }

        //}



        //private async Task verificaAtualizaçãoPon(ISession session, Rack rack, SubRack subRack, Slot slot, PON pon, OLT olt)
        //{
        //    if (pon.Atualizar)
        //    {
        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {

        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }

        //            var comandoPON = new StringBuilder();
        //            comandoPON.Append("configure equipment slot " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID);
        //            var result = client.RunCommand(comandoPON.ToString());
        //        }
        //        pon.Atualizar = false;
        //        await session.UpdateAsync(pon);
        //    }
        //}
        #endregion

        //Configuração ONT/ONU *****************************************************************************************************************************
        #region
        //private async Task verificaAtualizaçãoONTONU(ISession session, Rack rack, SubRack subRack, Slot slot, PON pon, PosicaoPon pos, OLT olt)
        //{
        //    if (pos.Atualizar)
        //    {
        //        var Ont = await new Conexao<ONT>(session).selectFirst(p => p.Posicao_Pon == pos);

        //        using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //        {
        //            if (!client.IsConnected)
        //            {
        //                client.Connect();
        //            }

        //            // Adicionando ONT
        //            #region
        //            var comandoONT = new StringBuilder();
        //            comandoONT.Append("configure equipment ont interface " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID);
        //            comandoONT.Append(" desc1 " + Ont.Login + " desc2 " + Ont.Observacao);
        //            comandoONT.Append(" sernum ALCL:00000000 subslocid " + Ont.SerialID + " sw-dnload-version DISABLED");
        //            var result = client.RunCommand(comandoONT.ToString());
        //            #endregion

        //            //Configurando Porta
        //            #region
        //            var comandoPortaDados = new StringBuilder();
        //            comandoPortaDados.Append("configure equipment ont slot" + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoPortaDados.Append(" planned-card-type ethernet plndnumdataports 1 plndnumvoiceports 0");
        //            result = client.RunCommand(comandoPortaDados.ToString());
        //            #endregion

        //            //Configurando porta Ethernet
        //            #region
        //            var comandoPortaEthernet = new StringBuilder();
        //            comandoPortaEthernet.Append("configure ethernet ont " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoPortaEthernet.Append(" admin-state up");
        //            result = client.RunCommand(comandoPortaEthernet.ToString());
        //            #endregion

        //            //Configuração interface de Rede
        //            #region
        //            var comandoInterfaceRede = new StringBuilder();
        //            comandoInterfaceRede.Append("configure interface port uni:" + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoInterfaceRede.Append(" admin-state up");
        //            #endregion

        //            //Configurar Perfil de Banda
        //            #region
        //            var comandoPerfilBanda = new StringBuilder();
        //            comandoPerfilBanda.Append("configure qos interface " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoPerfilBanda.Append(" upstream-queue 0 bandwidth-profile name:" + Ont.PerfilBanda.Descricao);
        //            comandoPerfilBanda.Append(" shaper-profile:" + Ont.PerfilBanda.Descricao);

        //            result = client.RunCommand(comandoPerfilBanda.ToString());
        //            #endregion

        //            //Associar  a uma VLAN
        //            #region
        //            var comandoVlan = new StringBuilder();
        //            comandoVlan.Append("configure bridge port " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            result = client.RunCommand(comandoVlan.ToString());
        //            comandoVlan.Clear();

        //            comandoVlan.Append("configure bridge port " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoVlan.Append(" max-unicast-mac 10");
        //            result = client.RunCommand(comandoVlan.ToString());
        //            comandoVlan.Clear();

        //            comandoVlan.Append("configure bridge port " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoVlan.Append(" vlan-id " + Ont.Vlan.VlanID + "tag untagged vlan-scope network");
        //            result = client.RunCommand(comandoVlan.ToString());
        //            comandoVlan.Clear();

        //            comandoVlan.Append("configure bridge port " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoVlan.Append(" pvid " + Ont.Vlan.VlanID);
        //            result = client.RunCommand(comandoVlan.ToString());
        //            #endregion

        //            //Detecta as configurações de estado de Administrador
        //            #region
        //            var comandoADM = new StringBuilder();
        //            comandoADM.Append("configure equipment ont loop " + rack.RackID + "/" + subRack.SubRackID + "/" + slot.SlotID + "/" + pon.PONID + "/" + pos.PosicaoID + "/1");
        //            comandoADM.Append(" loop-detect enable");
        //            #endregion

        //        }
        //        pon.Atualizar = false;
        //        await session.UpdateAsync(pon);
        //    }
        //}
        #endregion

        //Configuraçao Perfil Banda *************************************************************************************************************************
        #region
        //private async Task Conexao_Perfil_Banda()
        //{
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {

        //            IList<PerfilBanda> perfilBandas = await new Conexao<PerfilBanda>(session).select();


        //            foreach (var perfil in perfilBandas)
        //            {
        //                IList<OLT> olts = perfil.Olt;
        //                foreach (var olt in olts)
        //                {
        //                    cadastrarPerfil(session, perfil, olt);
        //                    perfil.Atualizar = false;
        //                    await session.UpdateAsync(perfil);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
        //    }

        //}

        //Cadastrar Perfil de Banda ************************************************************************************************************************************
        #region
        //private void cadastrarPerfil(ISession session, PerfilBanda perfil, OLT olt)
        //{
        //    using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //    {

        //        if (!client.IsConnected)
        //        {
        //            client.Connect();
        //        }

        //        //Upload
        //        #region
        //        var comandUpload = new StringBuilder();
        //        comandUpload.Append("configure qos profiles bandwidth " + perfil.Descricao + " committed-info-rate 0 assured- info-rate " + perfil.TaxaUpload + " excessive-info-rate " + perfil.TaxaUpload + " delay-tolerance 8");
        //        var result = client.RunCommand(comandUpload.ToString());
        //        #endregion

        //        //Download
        //        #region
        //        var comandDownload = new StringBuilder();
        //        comandDownload.Append("configure qos profiles shaper " + perfil.Descricao + " committed-info-rate 0 committed-burst-size 0 excess-info-rate  " + perfil.TaxaDownload + " type singletonkenbucketgpon");
        //        result = client.RunCommand(comandUpload.ToString());
        //        #endregion
        //    }



        //}
        #endregion



        #endregion

        //Configuraçao VLANs *********************************************************************************************************************************
        #region

        //private async Task Conexao_Vlan()
        //{
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {

        //            IList<VLAN> vlans = await new Conexao<VLAN>(session).select();


        //            foreach (var vlan in vlans)
        //            {
        //                IList<OLT> olts = vlan.Olt;
        //                foreach (var olt in olts)
        //                {
        //                    CadastrarVlan(session, vlan, olt);
        //                    vlan.Atualizar = false;
        //                    await session.UpdateAsync(vlan);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CONSTANTES.log4Net.Error(ex.Message, ex.InnerException);
        //    }

        //}

        //Cadastrar Vlans
        #region
        //private void CadastrarVlan(ISession session, VLAN vlan, OLT olt)
        //{

        //    using (var client = new SshClient(olt.IP, olt.Porta, olt.Login, olt.Senha))
        //    {

        //        if (!client.IsConnected)
        //        {
        //            client.Connect();
        //        }

        //        //Cadastrar Vlans
        //        #region
        //        var comandVlan = new StringBuilder();
        //        comandVlan.Append("configure service vpls " + vlan.VlanID + " customer 1 v-vpls vlan");
        //        var result = client.RunCommand(comandVlan.ToString());
        //        comandVlan.Clear();

        //        comandVlan.Append("configure service vpls " + vlan.VlanID + " description '" + vlan.Descricao + "'");
        //        result = client.RunCommand(comandVlan.ToString());
        //        comandVlan.Clear();

        //        comandVlan.Append("configure service vpls "+ vlan.VlanID +" customer 1 v-vpls vlan "+ vlan.VlanID + " create");
        //        result = client.RunCommand(comandVlan.ToString());
        //        comandVlan.Clear();

        //        comandVlan.Append("configure service vpls "+ vlan.VlanID +" no shutdown");
        //        result = client.RunCommand(comandVlan.ToString());
        //        comandVlan.Clear();

        //        comandVlan.Append("configure vlan id "+vlan.VlanID+" mode residential-bridge in-qos-prof-name name:"+vlan.Descricao);

        //        #endregion
        //    }




        //}


        #endregion

        #endregion

        private void log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
    }
}
