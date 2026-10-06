using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class RelatorioCapacidadeAgendamentoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/RelatorioCapacidadeAgendamentoApi/GetAll")]
        public async Task<DataTableResponse<RelatorioCapacidadeAgendamentoListModel>> GetAll(RelatorioCapacidadeAgendamentoDataTableRequest request)
        {
            try
            {
                GetApiRetorno<List<DetalhamentoDisponibilidadeListaModel>> detalhamento = new GetApiRetorno<List<DetalhamentoDisponibilidadeListaModel>>();
                detalhamento.Objeto = new List<DetalhamentoDisponibilidadeListaModel>();
                List<RelatorioCapacidadeAgendamentoListModel> retorno = new List<RelatorioCapacidadeAgendamentoListModel>();

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNh))
                using (var repoPeriodo = new Conexao<Turno_ServicoAniel>(sessionT5))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var repoJornada = new Conexao<Tb_Pt_Jornada_Funcionario>(sessionT5))
                using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                using (var agendamentoApi = new Agendamento_EquipeApiController())
                using (var horarioApi = new HorarioApiController())
                using (var repoFeriado = new Conexao<Feriado>(sessionT5))
                using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
                using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5NH))
                using (var repoPerfil = new Conexao<PerfilAgendamento>(sessionT5NH))
                using (var repoArea = new Conexao<AreaAniel>(sessionT5NH))
                using (var repoEscritorio = new Conexao<EscritorioAniel>(sessionT5NH))
                using (var tipoServicoApi = new Tipo_Servico_EquipeApiController())
                {

                    #region [Lista e Objetos]
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();
                    Tipo_Servico_EquipeAniel tipo_Servico_EquipeAniel = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == Convert.ToInt16(request.TipoServico));
                    List<PerfilAgendamento> perfis = (await repoPerfil.select()).ToList();
                    var turnosBanco = repositoryTurno.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList();
                    //Turno_ServicoAniel turno = await repositoryTurno.selectFirst(p => p.Id == request.IdPeriodo);
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    RoteirizacaoBase helper = new RoteirizacaoBase();
                    var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                    var horariosAgendamento = await GetHorarioAgendamento(sessionT5);
                    var config = await repoConfig.selectFirst();
                    #endregion

                    //TimeSpan tempoExecucaoTimeSpanTmp = (await tipoServicoApi.TempoServico(sessionT5NH, Convert.ToInt32(request.TipoServico), null, config)).valor.Value;
                    //DateTime? tempoExecucaoDt = DateTime.Today.Add(tempoExecucaoTimeSpanTmp);
                    //String tempoExecucao = tempoExecucaoDt.Value.ToString("HH:mm:ss");

                    #region [Variaveis]
                    var dataInicio = Convert.ToDateTime(request.DataInicio);
                    var dataFim = Convert.ToDateTime(request.DataFim);
                    string tempoExecucao = null;
                    if (horariosAgendamento == null)
                    {
                        throw new Exception("O Horário de Agendamento Padrão não foi definido em Configurações Gerais > Horários ");
                    }
                    TimeSpan? tempoExecucaoTimeSpan = null;
                    #endregion

                    #region [Validações]
                    if (request.TipoServico != null)
                    {
                        tempoExecucaoTimeSpan = documentoBusiness.GetTempoExecucao(config, null, Convert.ToInt16(request.TipoServico));
                        tempoExecucaoTimeSpan = tempoExecucaoTimeSpan.HasValue ? tempoExecucaoTimeSpan.Value : config.Tempo_Execucao_Padrao.Value.TimeOfDay;
                        tempoExecucaoTimeSpan = tempoExecucaoTimeSpan.Value.Add(config.Tempo_Medio_Deslocamento.Value.TimeOfDay);
                        tempoExecucao = tempoExecucaoTimeSpan.Value.ToString(@"hh\:mm\:ss");
                    }

                    var areas = await repoArea.select(p => p.Status == Status_Default.Ativo);

                    if (!String.IsNullOrEmpty(request.CodEscritorio))
                    {
                        areas = areas.Where(p => p.Cod_Escritorio == request.CodEscritorio).ToList();

                        if (!String.IsNullOrEmpty(request.CodArea))
                        {
                            areas = areas.Where(p => p.Cod_Area == request.CodArea).ToList();
                        }
                    }

                    var periodos = new Queue<Turno_ServicoAniel>();
                    if (request.AgruparPorPeriodo == "S")
                        periodos = new Queue<Turno_ServicoAniel>(repoPeriodo.select().Result.Where(p => p.Hora_Ini.HasValue && p.Hora_Fim.HasValue && p.Status == Status_Default.Ativo).OrderBy(p => p.Hora_Ini));

                    if (request.IdPeriodo.HasValue)
                    {
                        periodos = new Queue<Turno_ServicoAniel>(repoPeriodo.select().Result.Where(p => p.Id == request.IdPeriodo && p.Status == Status_Default.Ativo));
                    }
                    var configGerais = await repoConfig.selectFirst();

                    ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                    var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);

                    #endregion

                    var index_array_periodo = 0;
                    var tam_periodo = periodos.ToList().Count;
                    var array_periodo = periodos.ToList();

                    for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                    {
                        List<Feriado> feriados = (await repoFeriado.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Data == data)).ToList();
                        foreach (var areaAniel in areas)
                        {
                            do
                            {
                                #region [Identificação de Periodo e Agrupamento]
                                Turno_ServicoAniel turno = null;
                                //if (periodos.Any())
                                //{
                                //    turno = periodos.Dequeue();
                                //}
                                if (request.AgruparPorPeriodo == "S")
                                {
                                    if (index_array_periodo == tam_periodo)
                                        index_array_periodo = 0;

                                    turno = array_periodo[index_array_periodo];
                                }
                                else
                                {
                                    index_array_periodo = 0;
                                }
                                index_array_periodo++;
                                #endregion

                                #region [Consultas]
                                var capacidadeDia = new RelatorioCapacidadeAgendamentoListModel();
                                capacidadeDia.Data = data.ToString("dd/MM/yyyy");
                                var agendamentoEquipeAniel = new Agendamento_EquipeApiController();
                                var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                                var perfisLocalidades = agendamentoEquipeAniel.GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                                var forcaTrabalhoLista = await agendamentoEquipeAniel.GetForcaTrabalho(sessionT5, data, config, perfisNaturezas, perfisLocalidades, request.Equipe);

                                forcaTrabalhoLista = forcaTrabalhoLista.Select(p =>
                                {
                                    if (p.Id_Horario_Connect == null)
                                    {
                                        p.Id_Horario_Connect = horarios_servico.Find(p => p.Padrao)?.Id;
                                    }
                                    return p;
                                }).ToList();

                                forcaTrabalhoLista = forcaTrabalhoLista.Select(p =>
                                {
                                    Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                                    {
                                        Cod_Escritorio = areaAniel.Cod_Escritorio,
                                        Cod_Area = areaAniel.Cod_Area,
                                        Cod_Localidade = !String.IsNullOrEmpty(request.CodLocalidade) ? Convert.ToInt32(request.CodLocalidade) : (int?)null,
                                        Cod_Natureza = !String.IsNullOrEmpty(request.CodNatureza) ? Convert.ToInt32(request.CodNatureza) : tipo_Servico_EquipeAniel.Cod_Natureza,
                                        LocalidadeStr = request.Localidade
                                    };


                                    TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                                    {
                                        Naturezas = p.Naturezas,
                                        Localidades = p.Localidades,
                                        Equipe = p.Equipe,
                                    };

                                    var capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                                    if (capaz)
                                        return p;
                                    else
                                        return null;

                                }).Where(p => p != null && p.Disponivel).ToList();

                                TimeSpan capacidadePlena = TimeSpan.FromTicks(forcaTrabalhoLista.Select(p =>
                                {
                                    var horario = repoHorario.selectFirst(y => y.Id == p.Id_Horario_Connect).Result;
                                    return horario != null ? agendamentoEquipeAniel.GetHorarioPleno(turno, horario, data) : TimeSpan.Zero;
                                }).Sum(p => p.Ticks));

                                List<DadosPerfilDisponibilidadeListaModel> dadosPerfil = new List<DadosPerfilDisponibilidadeListaModel>();
                                var agendamento = (await agendamentoEquipeAniel.GetDatasValidasDetalhamento(sessionNh,
                                                                                    sessionT5,
                                                                                    horariosAgendamento,
                                                                                    feriados,
                                                                                    Convert.ToInt16(request.TipoServico),
                                                                                     !String.IsNullOrEmpty(request.CodNatureza) ? Convert.ToInt32(request.CodNatureza) : tipo_Servico_EquipeAniel.Cod_Natureza,
                                                                                    areaAniel.Cod_Escritorio,
                                                                                    areaAniel.Cod_Area,
                                                                                    Convert.ToInt32(request.CodLocalidade),
                                                                                    request.Localidade,
                                                                                    DateTime.Today.Add(documentoBusiness.GetTempoExecucao(config, null, Convert.ToInt16(request.TipoServico)).Value),
                                                                                    data,
                                                                                    request.IdPeriodo,
                                                                                    null,
                                                                                    request.Equipe)).FirstOrDefault();

                                var dao = new RoteirizacaoDAO(sessionT5: sessionT5, sessionT5NH: sessionT5NH);
                                var tecnicos = dao.GetTecnicoPlanejamentoRoteirizacaoSql(configGerais, new RoteirizacaoDiaConf
                                {
                                    Data = DateTime.Today,
                                    Alterar_DataHora_SemAgendamentoAniel = true,
                                    horarios_servico = horarios_servico,
                                }, new GestaoCapacidadeRoteirizacaoRequest()
                                {
                                    GrupoArea = request.CodEscritorio,
                                    Area = request.CodArea,
                                    Localidade = request.Localidade,
                                    Natureza = request.CodNatureza
                                }).Result;

                                var quantidade_tecnicos_escalados = tecnicos.Count;

                                LimiteAgendamentoAniel limAgendData = null;
                                var listaLimites = limiteAgendamentoBusiness.GetPercNaturezaByArea(areaAniel.Cod_Escritorio, areaAniel.Cod_Area,
                                    !String.IsNullOrEmpty(request.CodNatureza) ? Convert.ToInt32(request.CodNatureza) : Convert.ToInt32(tipo_Servico_EquipeAniel.Cod_Natureza));
                                if (!String.IsNullOrEmpty(request.CodNatureza))
                                    limAgendData = listaLimites.FirstOrDefault(p => p.DataInicio <= data && data <= p.DataFim);
                                #endregion

                                #region [Atualizações Agendamentos]
                                if (agendamento != null)
                                {
                                    foreach (var item in agendamento.Horas)
                                    {
                                        item.Turno = Convert.ToString(PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucaoTimeSpan)?.Id);
                                    }

                                    foreach (var item in agendamento.HorasCapacidade)
                                    {
                                        item.Turno = Convert.ToString(PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucaoTimeSpan)?.Id);
                                    }

                                    if (request.IdPeriodo.HasValue)
                                    {
                                        agendamento.Horas = agendamento.Horas.FindAll(p => p.Turno == request.IdPeriodo.ToString());
                                        agendamento.HorasCapacidade = agendamento.HorasCapacidade.FindAll(p => p.Turno == request.IdPeriodo.ToString());
                                    }

                                    List<AgendamentoHorasDisponiveis> horasAgrupadas = new List<AgendamentoHorasDisponiveis>();
                                    foreach (var item in agendamento.Horas.OrderBy(p => p.Hora_Inicio).GroupBy(p => new { p.Hora_Inicio, p.Hora_Final, p.IdPerfil }))
                                    {
                                        var itemAgrupado = item.FirstOrDefault();
                                        itemAgrupado.Aptos = item.Sum(p => p.Aptos);
                                        itemAgrupado.Forca_Trabalho = item.Sum(p => p.Forca_Trabalho);
                                        itemAgrupado.Lista_Forca_Trabalho = item.ToList().SelectMany(p => p.Lista_Forca_Trabalho).ToList();
                                        itemAgrupado.Lista_Ja_Agendados = item.ToList().SelectMany(p => p.Lista_Ja_Agendados).ToList();
                                        itemAgrupado.ForcaTrabalhoMinutosPrevistos = item.Sum(p => p.ForcaTrabalhoMinutosPrevistos);
                                        horasAgrupadas.Add(itemAgrupado);
                                    }

                                    agendamento.Horas = horasAgrupadas;


                                    List<AgendamentoHorasDisponiveis> horasCapacidadeAgrupadas = new List<AgendamentoHorasDisponiveis>();
                                    foreach (var item in agendamento.HorasCapacidade.OrderBy(p => p.Hora_Inicio).GroupBy(p => new { p.Hora_Inicio, p.Hora_Final, p.IdPerfil }))
                                    {
                                        var itemAgrupado = item.FirstOrDefault();
                                        itemAgrupado.Aptos = item.Sum(p => p.Aptos);
                                        itemAgrupado.Forca_Trabalho = item.Sum(p => p.Forca_Trabalho);
                                        itemAgrupado.Lista_Forca_Trabalho = item.ToList().SelectMany(p => p.Lista_Forca_Trabalho).ToList();
                                        itemAgrupado.Lista_Ja_Agendados = item.ToList().SelectMany(p => p.Lista_Ja_Agendados).ToList();
                                        itemAgrupado.ForcaTrabalhoMinutosPrevistos = item.Sum(p => p.ForcaTrabalhoMinutosPrevistos);
                                        horasCapacidadeAgrupadas.Add(itemAgrupado);
                                    }

                                    agendamento.HorasCapacidade = horasCapacidadeAgrupadas;


                                    if (request.IdPeriodo.HasValue)
                                    {
                                        agendamento.ListaJaAgendadosForaFaixa = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => y.DadosServico.IdPeriodo == request.IdPeriodo);
                                    }
                                }
                                #endregion

                                #region [Calculos]
                                TimeSpan CapacidadeUtilTotalLimite = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Sum(p => p.ForcaTrabalhoMinutosPrevistosComLimite)) : TimeSpan.Zero;
                                TimeSpan CapacidadeUtilTotal = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                                var listaNumOSTotal = agendamento != null ? agendamento.Horas.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct() : new List<string>();
                                var agendamentosForaFaixaNoMesmoTurnoMinutosTotal = agendamento != null ? agendamento.ListaJaAgendadosForaFaixa.Sum(p => p.DuracaoMinutos) : 0;
                                var agendamentosManuaisMinutosTotal = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos) : 0;
                                var agendamentosMinutosTotal = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos) : 0;
                                CapacidadeUtilTotalLimite = CapacidadeUtilTotalLimite.Add(TimeSpan.FromMinutes(agendamentosMinutosTotal));

                                var agendamentosForaFaixaNoMesmoTurnoMinutosTotalDeslocamento = agendamento != null ? agendamento.ListaJaAgendadosForaFaixa.Sum(p => p.Deslocamento) : 0;
                                var agendamentosManuaisMinutosTotalDeslocamento = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.Deslocamento) : 0;
                                var agendamentosMinutosTotalDeslocamento = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.Deslocamento) : 0;

                                TimeSpan Capacidade = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                                #endregion

                                #region [Agendamento]
                                if (agendamento != null)
                                {
                                    detalhamento.Objeto = agendamento.Horas.SelectMany(p =>
                                    {
                                        var listaNumOS = p.Lista_Ja_Agendados.Select(y => y.DadosServico.NumObra).Distinct();
                                        p.Lista_Ja_Agendados = listaNumOS.Select(y => p.Lista_Ja_Agendados.First(z => z.DadosServico.NumObra == y)).OrderBy(p => p.UsuarioAgendamento).ToList();
                                        return p.Lista_Ja_Agendados.Select(y =>
                                        {
                                            var horasAgendadasExecucao = TimeSpan.FromMinutes(y.DuracaoMinutos - y.Deslocamento);
                                            return new DetalhamentoDisponibilidadeListaModel
                                            {
                                                NumObra = y.DadosServico.NumObra,
                                                DataFimAgendamento = y.DataHoraFim.ToString("dd/MM/yyyy HH:mm"),
                                                DataInicioAgendamento = y.DataHoraInicio.ToString("dd/MM/yyyy HH:mm"),
                                                OrigemAgendamento = y.Origem.HasValue ? HtmlEnumExtensions.GetEnumDescription<OrigemAgendamentoOS>(y.Origem.Value) : String.Empty,
                                                DataHoraCriacao = y.DataHoraCriacao.HasValue ? y.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                                TempoDeslocamento = TimeSpan.FromMinutes(y.Deslocamento).ToString(@"hh\:mm"),
                                                HorasAgendadas = horasAgendadasExecucao.ToString(@"hh\:mm"),
                                                UsuarioCriacao = y.UsuarioCriacao,
                                                UsuarioAgendamento = y.UsuarioAgendamento,
                                                GrupoArea = y.DadosServico.DescricaoEscritorio,
                                                Area = y.DadosServico.DescricaoArea,
                                                Localidade = y.DadosServico.LocalidadeStr,
                                                TipoServico = y.DadosServico.TipoServico,
                                                Natureza = y.DadosServico.DescricaoNatureza,
                                                Periodo = y.DadosServico.Periodo,
                                                Status = y.DadosServico.Status,
                                                PerfilAgendamento = perfis.Find(k => k.Id == p.IdPerfil)?.Descricao ?? "Sem Perfil",
                                                Tecnico = y.EquipeDefinitiva,
                                                TecnicoTemporario = y.EquipeTemporaria,
                                            };
                                        });
                                    }).ToList();

                                    detalhamento.Objeto = detalhamento.Objeto.Concat(agendamento.ListaJaAgendadosForaFaixa.Select(y =>
                                    {
                                        var horasAgendadasExecucao = TimeSpan.FromMinutes(y.DuracaoMinutos - y.Deslocamento);
                                        string perfil = perfis.Find(k => k.Id == forcaTrabalhoLista.Find(z => z.Equipe == y.Equipe)?.IdPerfilAgendamento)?.Descricao ?? "";

                                        return new DetalhamentoDisponibilidadeListaModel
                                        {
                                            NumObra = y.DadosServico.NumObra,
                                            DataFimAgendamento = y.DataHoraFim.ToString("dd/MM/yyyy HH:mm"),
                                            DataInicioAgendamento = y.DataHoraInicio.ToString("dd/MM/yyyy HH:mm"),
                                            OrigemAgendamento = y.Origem.HasValue ? HtmlEnumExtensions.GetEnumDescription<OrigemAgendamentoOS>(y.Origem.Value) : String.Empty,
                                            DataHoraCriacao = y.DataHoraCriacao.HasValue ? y.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                            TempoDeslocamento = TimeSpan.FromMinutes(y.Deslocamento).ToString(@"hh\:mm"),
                                            HorasAgendadas = horasAgendadasExecucao.ToString(@"hh\:mm"),
                                            UsuarioCriacao = y.UsuarioCriacao,
                                            UsuarioAgendamento = y.UsuarioAgendamento,
                                            GrupoArea = y.DadosServico.DescricaoEscritorio,
                                            Area = y.DadosServico.DescricaoArea,
                                            Localidade = y.DadosServico.LocalidadeStr,
                                            TipoServico = y.DadosServico.TipoServico,
                                            Natureza = y.DadosServico.DescricaoNatureza,
                                            Periodo = y.DadosServico.Periodo,
                                            Status = y.DadosServico.Status,
                                            Tecnico = y.EquipeDefinitiva,
                                            TecnicoTemporario = y.EquipeTemporaria,
                                            PerfilAgendamento = string.IsNullOrEmpty(perfil) ? "Sem Perfil" : perfil
                                        };
                                    })).ToList();

                                    var listaNumOS = detalhamento.Objeto.Select(y => y.NumObra).Distinct();
                                    detalhamento.Objeto = listaNumOS.Select(y => detalhamento.Objeto.First(z => z.NumObra == y)).ToList();
                                    detalhamento.Objeto = detalhamento.Objeto.OrderBy(p => p.PerfilAgendamento).ThenBy(p => p.TecnicoTemporario).ThenBy(p => p.DataHoraCriacao).Select(p =>
                                    {
                                        Capacidade = Capacidade.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                        p.CapacidadeRestante = ((int)Capacidade.TotalHours).ToString("D2") + Capacidade.ToString(@"\:mm");
                                        return p;
                                    }).ToList();


                                    List<PerfilAgendamento> perfisForcaTrabalho = perfis.FindAll(y => forcaTrabalhoLista.Select(p => p.IdPerfilAgendamento).Contains(y.Id));
                                    dadosPerfil = perfisForcaTrabalho.Select(groupPerfil =>
                                    {
                                        List<string> turnosEncontrados = agendamento.Horas.Select(p => p.Turno).Distinct().ToList();
                                        TimeSpan CapacidadePerfil = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Where(y => y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                                        TimeSpan CapacidadePerfilComLimite = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Where(y => y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistosComLimite)) : TimeSpan.Zero;

                                        var agendamentoPerfil = agendamento.Horas.FindAll(y => y.IdPerfil == groupPerfil.Id);
                                        List<DadosPerfilDisponibilidadeListaModel> dadosPerfilTurno = new List<DadosPerfilDisponibilidadeListaModel>();
                                        var agendamentoPerfilCapacidadeTecnico = agendamento.HorasCapacidade.FindAll(y => y.IdPerfil == groupPerfil.Id && y.Equipe_Tmp != null);
                                        var ForcaTrabalho = agendamentoPerfilCapacidadeTecnico.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                        var listaNumOS = agendamentoPerfil.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                        var agendamentosForaFaixaNoMesmoTurnoMinutos = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => groupPerfil.Id == y.IdPerfil).Sum(p => p.DuracaoMinutos);
                                        var agendamentosManuaisMinutos = listaNumOS.Select(y => agendamentoPerfil.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                        var agendamentosMinutos = listaNumOS.Select(y => agendamentoPerfil.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                        CapacidadePerfil = CapacidadePerfil.Add(TimeSpan.FromMinutes(agendamentosMinutos));
                                        CapacidadePerfilComLimite = CapacidadePerfil.Add(TimeSpan.FromMinutes(agendamentosMinutos));


                                        var CapacidadePerfilParcial = TimeSpan.FromMinutes(CapacidadePerfil.TotalMinutes);
                                        var CapacidadePerfilParcialComLimite = TimeSpan.FromMinutes(CapacidadePerfilComLimite.TotalMinutes);
                                        TimeSpan CapacidadeUtilPerfil = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;

                                        if (config.Considerar_Agendamento_Periodo)
                                        {
                                            foreach (var turnoEncontrado in turnosEncontrados)
                                            {
                                                Turno_ServicoAniel turnoBanco = turnosBanco.Find(p => p.Id.ToString() == turnoEncontrado);
                                                TimeSpan CapacidadePerfilTurno = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                                                TimeSpan CapacidadePerfilTurnoComLimite = agendamento != null ? TimeSpan.FromMinutes(agendamento.HorasCapacidade.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistosComLimite)) : TimeSpan.Zero;
                                                var agendamentoPerfilTurno = agendamento.Horas.FindAll(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id);
                                                var agendamentoPerfilCapacidadeTecnicoTurno = agendamento.HorasCapacidade.FindAll(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id && y.Equipe_Tmp != null);
                                                var ForcaTrabalhoPerfil = agendamentoPerfilCapacidadeTecnicoTurno.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                                var listaNumOSPerfil = agendamentoPerfilTurno.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                                var agendamentosForaFaixaNoMesmoTurnoMinutosPerfil = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => BetweenIgual(y.DataHoraInicio, turnoBanco.Hora_Ini, turnoBanco.Hora_Fim) && groupPerfil.Id == y.IdPerfil).Sum(p => p.DuracaoMinutos);
                                                var agendamentosManuaisMinutosPerfil = listaNumOSPerfil.Select(y => agendamentoPerfilTurno.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                                var agendamentosMinutosPerfil = listaNumOSPerfil.Select(y => agendamentoPerfilTurno.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                                CapacidadePerfilTurno = CapacidadePerfilTurno.Add(TimeSpan.FromMinutes(agendamentosMinutosPerfil));
                                                CapacidadePerfilTurnoComLimite = CapacidadePerfilTurnoComLimite.Add(TimeSpan.FromMinutes(agendamentosMinutosPerfil));
                                                TimeSpan CapacidadePerfilTurnoParcial = TimeSpan.FromMinutes(CapacidadePerfilTurno.TotalMinutes);
                                                TimeSpan CapacidadePerfilTurnoParcialComLimite = TimeSpan.FromMinutes(CapacidadePerfilTurnoComLimite.TotalMinutes);

                                                TimeSpan CapacidadeUtilPerfilTurno = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;

                                                detalhamento.Objeto.FindAll(p => p.PerfilAgendamento == groupPerfil.Descricao
                                                                          && BetweenIgual(Convert.ToDateTime(p.DataInicioAgendamento), turnoBanco.Hora_Ini, turnoBanco.Hora_Fim)).Select(p =>
                                                                          {
                                                                              CapacidadePerfilTurnoParcial = CapacidadePerfilTurnoParcial.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                                                              CapacidadePerfilTurnoParcialComLimite = CapacidadePerfilTurnoParcialComLimite.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                                                              return p;
                                                                          }).ToList();



                                                var capacidadePerfilServicos = agendamento.HorasCapacidade.FindAll(p => p.IdPerfil == groupPerfil.Id && p.Turno == turnoEncontrado).Sum(p => p.Lista_Forca_Trabalho.Count);


                                                var CapacidadePerfilTurnoParcialCalculoTempo = TimeSpan.FromMinutes(agendamento.HorasCapacidade.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos));
                                                CapacidadePerfilTurnoParcialCalculoTempo = CapacidadePerfilTurnoParcialCalculoTempo.Subtract(TimeSpan.FromMinutes(agendamentosManuaisMinutosPerfil + agendamentosForaFaixaNoMesmoTurnoMinutosPerfil));
                                                if (agendamentoPerfilTurno.FindAll(p => p.Deslocamento == 0).Count > 0)
                                                {
                                                    double minutosExtrasDeslocamentoTurno = 0;
                                                    foreach (var itemHorario in agendamentoPerfilTurno.FindAll(p => p.Deslocamento == 0))
                                                    {
                                                        minutosExtrasDeslocamentoTurno = itemHorario.Forca_Trabalho * config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes;
                                                        CapacidadePerfilTurnoParcialCalculoTempo = CapacidadePerfilTurnoParcialCalculoTempo.Add(TimeSpan.FromMinutes(minutosExtrasDeslocamentoTurno));
                                                    }
                                                }

                                                int capacidadeServicosPerfilBaseadoEmTempo = Convert.ToInt32(Math.Floor((CapacidadePerfilTurnoParcialCalculoTempo.TotalMinutes) / (tempoExecucaoTimeSpan.Value.TotalMinutes)));
                                                if (capacidadeServicosPerfilBaseadoEmTempo < capacidadePerfilServicos)
                                                {
                                                    capacidadePerfilServicos = capacidadeServicosPerfilBaseadoEmTempo;
                                                }

                                                if (capacidadePerfilServicos < 0)
                                                    capacidadePerfilServicos = 0;

                                                dadosPerfilTurno.Add(new DadosPerfilDisponibilidadeListaModel
                                                {
                                                    perfilAgendamento = groupPerfil.Descricao,
                                                    capacidadeRestante = CapacidadePerfilTurnoParcial.TotalMinutes > 0 ? ((int)CapacidadePerfilTurnoParcial.TotalHours).ToString("D2") + CapacidadePerfilTurnoParcial.ToString(@"\:mm") : "00:00",
                                                    capacidadeRestanteTimespan = CapacidadePerfilTurnoParcial.TotalMinutes > 0 ? CapacidadePerfilTurnoParcial : TimeSpan.Zero,
                                                    capacidadeRestanteTempoComLimiteTimespan = CapacidadePerfilTurnoParcialComLimite.TotalMinutes > 0 ? CapacidadePerfilTurnoParcialComLimite : TimeSpan.Zero,
                                                    capacidadeRestanteCalculoTempoTimespan = CapacidadePerfilTurnoParcialCalculoTempo.TotalMinutes > 0 ? CapacidadePerfilTurnoParcialCalculoTempo : TimeSpan.Zero,
                                                    capacidadeHoras = ((int)CapacidadePerfilTurno.TotalHours).ToString("D2") + CapacidadePerfilTurno.ToString(@"\:mm"),
                                                    capacidadeHorasTimespan = CapacidadePerfilTurno.TotalMinutes > 0 ? CapacidadePerfilTurno : TimeSpan.Zero,
                                                    forcaTrabalho = agendamentoPerfilTurno.Count > 0 ? agendamentoPerfilTurno.Max(p => p.Forca_Trabalho).ToString() : "0",
                                                    capacidadeRestanteServicos = capacidadePerfilServicos.ToString(),
                                                    capacidadeUtilHoras = ((int)CapacidadeUtilPerfilTurno.TotalHours).ToString("D2") + CapacidadeUtilPerfilTurno.ToString(@"\:mm"),
                                                });
                                            }
                                        }


                                        //preenchimento da capacidade restante por perfil
                                        detalhamento.Objeto.FindAll(p => p.PerfilAgendamento == groupPerfil.Descricao).Select(p =>
                                        {
                                            CapacidadePerfilParcial = CapacidadePerfilParcial.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                            CapacidadePerfilParcialComLimite = CapacidadePerfilParcialComLimite.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                            p.CapacidadeRestante = CapacidadePerfilParcial.TotalMinutes > 0 ? ((int)CapacidadePerfilParcial.TotalHours).ToString("D2") + CapacidadePerfilParcial.ToString(@"\:mm") : "00:00";
                                            return p;
                                        }).ToList();

                                        int capacidadeRestanteServicoTotalizada = 0;
                                        TimeSpan capacidadeRestanteTimespanTotalizada = TimeSpan.Zero;
                                        TimeSpan capacidadeRestanteComLimiteTimespanTotalizada = TimeSpan.Zero;
                                        TimeSpan capacidadeHorasTimespanTotalizada = TimeSpan.Zero;
                                        TimeSpan capacidadeRestanteTimespanCalculoTempoTotalizada = TimeSpan.Zero;

                                        if (config.Considerar_Agendamento_Periodo)
                                        {
                                            capacidadeRestanteServicoTotalizada = dadosPerfilTurno.Sum(p => Convert.ToInt16(p.capacidadeRestanteServicos));
                                            capacidadeRestanteTimespanTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeRestanteTimespan.Ticks));
                                            capacidadeRestanteTimespanCalculoTempoTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeRestanteCalculoTempoTimespan.Ticks));
                                            capacidadeHorasTimespanTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeHorasTimespan.Ticks));
                                            capacidadeRestanteComLimiteTimespanTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeRestanteTempoComLimiteTimespan.Ticks));
                                        }
                                        else
                                        {
                                            capacidadeRestanteTimespanTotalizada = CapacidadePerfilParcial.TotalMinutes > 0 ? CapacidadePerfilParcial : TimeSpan.Zero;
                                            capacidadeRestanteComLimiteTimespanTotalizada = CapacidadePerfilParcialComLimite.TotalMinutes > 0 ? CapacidadePerfilParcialComLimite : TimeSpan.Zero;
                                            capacidadeHorasTimespanTotalizada = CapacidadePerfil.TotalMinutes > 0 ? CapacidadePerfil : TimeSpan.Zero;

                                            var CapacidadePerfilParcialCalculoTempo = TimeSpan.FromTicks(CapacidadePerfilParcial.Ticks);
                                            if (agendamentoPerfil.FindAll(p => p.Deslocamento == 0).Count > 0)
                                            {
                                                double minutosExtrasDeslocamentoTurno = 0;
                                                foreach (var item in agendamentoPerfil.FindAll(p => p.Deslocamento == 0))
                                                {
                                                    minutosExtrasDeslocamentoTurno = item.Lista_Forca_Trabalho.Count * config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes;
                                                    CapacidadePerfilParcialCalculoTempo = CapacidadePerfilParcialCalculoTempo.Add(TimeSpan.FromMinutes(minutosExtrasDeslocamentoTurno));
                                                }
                                            }

                                            var capacidadePerfilServicos = agendamento.HorasCapacidade.FindAll(p => p.IdPerfil == groupPerfil.Id).Sum(p => p.Lista_Forca_Trabalho.Count);
                                            int capacidadeServicosPerfilBaseadoEmTempo = Convert.ToInt32(Math.Floor(CapacidadePerfilParcialCalculoTempo.TotalMinutes / (tempoExecucaoTimeSpan.Value.TotalMinutes)));
                                            if (capacidadeServicosPerfilBaseadoEmTempo < capacidadePerfilServicos)
                                            {
                                                capacidadePerfilServicos = capacidadeServicosPerfilBaseadoEmTempo;
                                            }

                                            if (capacidadePerfilServicos < 0)
                                                capacidadePerfilServicos = 0;

                                            capacidadeRestanteServicoTotalizada = capacidadePerfilServicos;

                                        }

                                        return new DadosPerfilDisponibilidadeListaModel
                                        {
                                            perfilAgendamento = groupPerfil.Descricao,
                                            capacidadeRestante = capacidadeRestanteTimespanTotalizada.TotalMinutes > 0 ? ((int)capacidadeRestanteTimespanTotalizada.TotalHours).ToString("D2") + capacidadeRestanteTimespanTotalizada.ToString(@"\:mm") : "00:00",
                                            capacidadeRestanteTimespan = capacidadeRestanteTimespanTotalizada.TotalMinutes > 0 ? capacidadeRestanteTimespanTotalizada : TimeSpan.Zero,
                                            capacidadeRestanteTempoComLimiteTimespan = capacidadeRestanteComLimiteTimespanTotalizada.TotalMinutes > 0 ? capacidadeRestanteComLimiteTimespanTotalizada : TimeSpan.Zero,
                                            capacidadeRestanteCalculoTempoTimespan = capacidadeRestanteTimespanCalculoTempoTotalizada.TotalMinutes > 0 ? capacidadeRestanteTimespanCalculoTempoTotalizada : TimeSpan.Zero,
                                            capacidadeHoras = ((int)capacidadeHorasTimespanTotalizada.TotalHours).ToString("D2") + capacidadeHorasTimespanTotalizada.ToString(@"\:mm"),
                                            capacidadeHorasTimespan = capacidadeHorasTimespanTotalizada.TotalMinutes > 0 ? capacidadeHorasTimespanTotalizada : TimeSpan.Zero,
                                            forcaTrabalho = agendamentoPerfil.Count > 0 ? agendamentoPerfil.Max(p => p.Forca_Trabalho).ToString() : "0",
                                            capacidadeRestanteServicos = capacidadeRestanteServicoTotalizada.ToString(),
                                            capacidadeUtilHoras = ((int)CapacidadeUtilPerfil.TotalHours).ToString("D2") + CapacidadeUtilPerfil.ToString(@"\:mm"),
                                        };

                                    }).ToList();
                                }
                                #endregion

                                #region [Calculos - Retorno]
                                TimeSpan capacidadeRestanteTimespan = TimeSpan.FromTicks(dadosPerfil.Sum(p => p.capacidadeRestanteTimespan.Ticks));
                                TimeSpan capacidadeRestanteTimespanComLimite = TimeSpan.FromTicks(dadosPerfil.Sum(p => p.capacidadeRestanteTempoComLimiteTimespan.Ticks));

                                TimeSpan capacidadeRestanteCalculoTempoTimespan = TimeSpan.FromTicks(dadosPerfil.Sum(p => p.capacidadeRestanteCalculoTempoTimespan.Ticks));
                                string capacidadeRestante = capacidadeRestanteTimespan.TotalMinutes > 0 ? ((int)capacidadeRestanteTimespan.TotalHours).ToString("D2") + capacidadeRestanteTimespan.ToString(@"\:mm") : "00:00";//Capacidade.TotalMinutes > 0 ? ((int)Capacidade.TotalHours).ToString("D2") + Capacidade.ToString(@"\:mm") : "00:00";
                                long forcaTrabalho = agendamento?.Horas != null ? agendamento.Horas.GroupBy(p => p.IdPerfil).Select(p => p.ToList().Max(p => p.Forca_Trabalho)).Sum() : 0;
                                var capacidadeServicos = agendamento?.Horas != null ? agendamento.HorasCapacidade.Sum(p => p.Lista_Forca_Trabalho.Count) : 0;
                                var horasAgendadas = TimeSpan.FromMinutes(agendamentosMinutosTotal + agendamentosManuaisMinutosTotal + agendamentosForaFaixaNoMesmoTurnoMinutosTotal);
                                var horasDeslocamento = TimeSpan.FromMinutes(agendamentosMinutosTotalDeslocamento + agendamentosManuaisMinutosTotalDeslocamento + agendamentosForaFaixaNoMesmoTurnoMinutosTotalDeslocamento);

                                if (dadosPerfil.Count > 0)
                                {
                                    capacidadeServicos = dadosPerfil.Sum(p => Convert.ToInt32(p.capacidadeRestanteServicos));
                                }
                                else
                                {
                                    int capacidadeServicosBaseadoEmTempo = Convert.ToInt32(Math.Floor(capacidadeRestanteCalculoTempoTimespan.TotalMinutes / (tempoExecucaoTimeSpan.Value.TotalMinutes)));
                                    if (capacidadeServicosBaseadoEmTempo < capacidadeServicos)
                                    {
                                        capacidadeServicos = capacidadeServicosBaseadoEmTempo;
                                    }

                                    if (capacidadeServicos < 0)
                                        capacidadeServicos = 0;
                                }

                                var totalHorasAproveitamento = horasAgendadas.Add(horasDeslocamento);
                                var percentualAproveitamentoPlena = (totalHorasAproveitamento.TotalMinutes / capacidadePlena.TotalMinutes) * 100;
                                var percentualAproveitamentoUtil = (totalHorasAproveitamento.TotalMinutes / CapacidadeUtilTotal.TotalMinutes) * 100;


                                IEnumerable<AgendamentoDadosModel> lista_servicos_agendados_natureza = new List<AgendamentoDadosModel>();
                                IEnumerable<AgendamentoDadosModel> lista_outros_servicos_agendados = new List<AgendamentoDadosModel>();

                                if (!String.IsNullOrEmpty(request.CodNatureza))
                                    lista_servicos_agendados_natureza = listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y && z.DadosServico.Cod_Natureza == Convert.ToInt64(request.CodNatureza)));

                                var cod_natureza = !String.IsNullOrEmpty(request.CodNatureza) ? Convert.ToInt64(request.CodNatureza) : 0;
                                lista_outros_servicos_agendados = listaNumOSTotal.Select(y => agendamento.Horas.Where(x => x.Lista_Ja_Agendados.Count > 0).SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y && z.DadosServico.Cod_Natureza != cod_natureza));

                                double servicos_agendados_natureza = 0, outros_servicos_agendados = 0;
                                foreach (var item in lista_servicos_agendados_natureza)
                                {
                                    servicos_agendados_natureza += item != null ? item.DuracaoMinutos + item.Deslocamento : 0;
                                }

                                var lista_outros_servicos_agendados_TMP = lista_outros_servicos_agendados.ToList();

                                foreach (var item in lista_outros_servicos_agendados_TMP)
                                {
                                    outros_servicos_agendados += item != null ? item.DuracaoMinutos + item.Deslocamento : 0;
                                }

                                capacidadeDia.OutrosServicosAgendados = outros_servicos_agendados > 0 ? TimeSpan.FromMinutes(outros_servicos_agendados).ToString(@"hh\:mm") : "00:00";
                                capacidadeDia.ServicosAgendadosNatureza = servicos_agendados_natureza > 0 ? TimeSpan.FromMinutes(servicos_agendados_natureza).ToString(@"hh\:mm") : "00:00";

                                double hr_mn_capacidade_util = ((int)CapacidadeUtilTotal.TotalHours * 60) + CapacidadeUtilTotal.Minutes;
                                var hr_mn_servicos = servicos_agendados_natureza + outros_servicos_agendados;
                                var subtracao_minutos = hr_mn_capacidade_util - hr_mn_servicos;
                                var horas_capacidade_util = (int)subtracao_minutos / 60;
                                var minutos_capacidade_util = (int)subtracao_minutos % 60;
                                var ts = new TimeSpan((int)horas_capacidade_util, minutos_capacidade_util, 0);
                                capacidadeDia.CapacidadeDisponivel = ts.ToString(@"hh\:mm");

                                capacidadeDia.HorasAgendadas = ((int)horasAgendadas.TotalHours).ToString("D2") + horasAgendadas.ToString(@"\:mm");
                                capacidadeDia.HorasDeslocamento = ((int)horasDeslocamento.TotalHours).ToString("D2") + horasDeslocamento.ToString(@"\:mm");
                                capacidadeDia.TempoExecucao = tempoExecucao;
                                capacidadeDia.LimiteMultiskill = agendamento != null ? Convert.ToString(agendamento.PercentualLimiteAgendamento) : "";

                                var calc_perc_aproveitamento_capacidade = (hr_mn_servicos / hr_mn_capacidade_util) * 100;
                                capacidadeDia.PercAproveitamentoCapacidade = calc_perc_aproveitamento_capacidade.ToString("F");
                                if (agendamento?.PercentualLimiteAgendamento != null)
                                {
                                    hr_mn_capacidade_util = ((int)CapacidadeUtilTotal.TotalHours * 60) + CapacidadeUtilTotal.Minutes;
                                    double percentual_capacidade_util = Convert.ToDouble((double)agendamento?.PercentualLimiteAgendamento / 100);
                                    var totalMinutos_capacidade_util = hr_mn_capacidade_util * percentual_capacidade_util;

                                    horas_capacidade_util = (int)totalMinutos_capacidade_util / 60;
                                    minutos_capacidade_util = (int)totalMinutos_capacidade_util % 60;
                                    ts = new TimeSpan((int)horas_capacidade_util, minutos_capacidade_util, 0);
                                    capacidadeDia.CapacidadeComLimite = ts.ToString(@"hh\:mm");

                                    totalMinutos_capacidade_util -= servicos_agendados_natureza;
                                    horas_capacidade_util = (int)totalMinutos_capacidade_util / 60;
                                    minutos_capacidade_util = (int)totalMinutos_capacidade_util % 60;
                                    ts = new TimeSpan((int)horas_capacidade_util, minutos_capacidade_util, 0);
                                    capacidadeDia.CapacidadeDisponivelComLimite = ts.ToString(@"hh\:mm");
                                }

                                if (!String.IsNullOrEmpty(capacidadeDia.CapacidadeComLimite))
                                {
                                    var partes = capacidadeDia.CapacidadeComLimite.Split(':');
                                    int horas = int.Parse(partes[0]);
                                    int minutos = int.Parse(partes[1]);
                                    int totalMinutos = (horas * 60) + minutos;
                                    var soma_PercAproveitamentoCapacidadeUtil = (servicos_agendados_natureza / totalMinutos) * 100;
                                    capacidadeDia.PercAproveitamentoCapacidadeUtil = soma_PercAproveitamentoCapacidadeUtil.ToString("F");
                                }
                                else
                                {
                                    capacidadeDia.PercAproveitamentoCapacidadeUtil = "0";
                                }

                                string sCapacidadePlena = String.Empty, sCapacidadeUtil = String.Empty;
                                if (request.FormatoVisualizacao == "servicos")
                                {
                                    double totalMinutos = (int)capacidadePlena.TotalHours * 60;
                                    string[] split = tempoExecucao.Split(new string[] { ":" }, StringSplitOptions.None);
                                    int hora = Convert.ToInt32(split[0]);
                                    int minuto = Convert.ToInt32(split[1]);

                                    double divisorMinutos = hora * 60 + minuto;

                                    double resultado = totalMinutos / divisorMinutos;
                                    int arredondado = (int)Math.Round(resultado);
                                    sCapacidadePlena = Convert.ToString(arredondado);

                                    var inteiro_capacidade_util = (hr_mn_capacidade_util / divisorMinutos);
                                    Double inteiro_capacidade_com_limite = 0;
                                    arredondado = (int)Math.Round(inteiro_capacidade_util);
                                    sCapacidadeUtil = Convert.ToString(arredondado);

                                    if (!String.IsNullOrEmpty(capacidadeDia.CapacidadeComLimite))
                                    {
                                        split = capacidadeDia.CapacidadeComLimite.Split(new string[] { ":" }, StringSplitOptions.None);
                                        hora = Convert.ToInt32(split[0]);
                                        minuto = Convert.ToInt32(split[1]);
                                        var divisorCapacidadeComLimite = hora * 60 + minuto;
                                        inteiro_capacidade_com_limite = (divisorCapacidadeComLimite / divisorMinutos);
                                        arredondado = (int)Math.Round(inteiro_capacidade_com_limite);
                                        capacidadeDia.CapacidadeComLimite = Convert.ToString(arredondado);
                                    }
                                    else
                                    {
                                        capacidadeDia.CapacidadeComLimite = "0";
                                    }

                                    capacidadeDia.ServicosAgendadosNatureza = lista_servicos_agendados_natureza.Where(x => x != null).Count().ToString();
                                    capacidadeDia.OutrosServicosAgendados = lista_outros_servicos_agendados_TMP.Where(x => x != null).Count().ToString();

                                    var calculo_capacidade_disponivel = (inteiro_capacidade_util - (lista_servicos_agendados_natureza.Where(x => x != null).Count() + lista_outros_servicos_agendados_TMP.Where(x => x != null).Count()));
                                    arredondado = (int)Math.Round(calculo_capacidade_disponivel);
                                    capacidadeDia.CapacidadeDisponivel = Convert.ToString(arredondado);

                                    var calculo_capacidade_disponivel_com_limite = inteiro_capacidade_com_limite - lista_servicos_agendados_natureza.Where(x => x != null).Count();
                                    arredondado = (int)Math.Round(calculo_capacidade_disponivel_com_limite);
                                    capacidadeDia.CapacidadeDisponivelComLimite = Convert.ToString(arredondado);

                                    double percentual_aproveitamento_capacidade_servicos = Convert.ToInt32(capacidadeDia.ServicosAgendadosNatureza) + Convert.ToInt32(capacidadeDia.OutrosServicosAgendados);
                                    percentual_aproveitamento_capacidade_servicos = Convert.ToInt32(sCapacidadeUtil) > 0 ? percentual_aproveitamento_capacidade_servicos / Convert.ToInt32(sCapacidadeUtil) : 0;
                                    percentual_aproveitamento_capacidade_servicos = percentual_aproveitamento_capacidade_servicos * 100;
                                    capacidadeDia.PercAproveitamentoCapacidade = percentual_aproveitamento_capacidade_servicos.ToString("F");

                                    double percentual_aproveitamento_capacidade_limite_servicos = Convert.ToInt32(capacidadeDia.ServicosAgendadosNatureza);
                                    percentual_aproveitamento_capacidade_limite_servicos = Convert.ToInt32(capacidadeDia.CapacidadeComLimite) > 0 ? percentual_aproveitamento_capacidade_limite_servicos / Convert.ToInt32(capacidadeDia.CapacidadeComLimite) : 0;
                                    percentual_aproveitamento_capacidade_limite_servicos = percentual_aproveitamento_capacidade_limite_servicos * 100;
                                    capacidadeDia.PercAproveitamentoCapacidadeUtil = percentual_aproveitamento_capacidade_limite_servicos.ToString("F");
                                }
                                else
                                {
                                    sCapacidadePlena = ((int)capacidadePlena.TotalHours).ToString("D2") + capacidadePlena.ToString(@"\:mm");
                                    sCapacidadeUtil = ((int)CapacidadeUtilTotal.TotalHours).ToString("D2") + CapacidadeUtilTotal.ToString(@"\:mm");
                                }

                                capacidadeDia.CapacidadePlena = sCapacidadePlena;
                                capacidadeDia.CapacidadeUtil = sCapacidadeUtil;
                                capacidadeDia.Turno = turno?.Descricao;

                                capacidadeDia.GrupoArea = repoEscritorio.selectFirst(p => p.Cod_Escritorio == areaAniel.Cod_Escritorio).Result?.Descricao;
                                capacidadeDia.Area = areaAniel?.Descricao;

                                capacidadeDia.TecnicosEscalados = Convert.ToString(quantidade_tecnicos_escalados);

                                retorno.Add(capacidadeDia);
                                #endregion

                            } while (index_array_periodo < tam_periodo);
                            //while (periodos.Any());
                        }
                    }
                }

                return new DataTableResponse<RelatorioCapacidadeAgendamentoListModel>
                {
                    draw = request.draw,
                    data = retorno,
                    recordsFiltered = retorno.Count()
                };
            }
            catch (Exception ex)
            {
                return new DataTableResponse<RelatorioCapacidadeAgendamentoListModel>
                {
                    data = new List<RelatorioCapacidadeAgendamentoListModel>(),
                    recordsFiltered = 0
                };
            }
        }

        public async Task<int> GetServicosAgendadosDia(ISession session, DateTime data, RelatorioCapacidadeAgendamentoDataTableRequest filtros)
        {
            int qtdServicos = 0;

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT COUNT(A.NUM_DOC) AS QTD_SERVICOS ");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine(" WHERE A.DATA_MAXIMA = @DATA_AGENDAMENTO ");
                sql.AppendLine(" AND ((A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.DATA_PROD = @DATA_AGENDAMENTO AND A.STATUS_EXECUCAO <> 9))");

                #region Filtros

                if (!String.IsNullOrEmpty(filtros.CodEscritorio))
                {
                    sql.AppendLine("   AND A.COD_ESCRITORIO = @COD_ESCRITORIO ");
                    parametros.Add("@COD_ESCRITORIO", filtros.CodEscritorio);

                    if (!String.IsNullOrEmpty(filtros.CodArea))
                    {
                        sql.AppendLine("   AND A.COD_AREA = @COD_AREA ");
                        parametros.Add("@COD_AREA", filtros.CodArea);

                        if (!String.IsNullOrEmpty(filtros.CodLocalidade))
                        {
                            sql.AppendLine("   AND A.COD_LOCALIDADE = @COD_LOCALIDADE ");
                            parametros.Add("@COD_LOCALIDADE", filtros.CodLocalidade);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(filtros.Equipe))
                {
                    sql.AppendLine("   AND A.EQUIPE = @EQUIPE ");
                    parametros.Add("@EQUIPE", filtros.Equipe);
                }

                if (!String.IsNullOrEmpty(filtros.CodNatureza))
                {
                    sql.AppendLine("  AND B.COD_NATUREZA = @COD_NATUREZA ");
                    parametros.Add("@COD_NATUREZA", filtros.CodNatureza);
                }

                if (filtros.IdPeriodo.HasValue)
                {
                    sql.AppendLine("   AND A.COD_TURNO_SERV = @COD_TURNO ");
                    parametros.Add("@COD_TURNO", filtros.IdPeriodo);
                }

                #endregion

                parametros.Add("@DATA_AGENDAMENTO", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader["QTD_SERVICOS"] != DBNull.Value)
                            qtdServicos = Convert.ToInt32(reader["QTD_SERVICOS"]);
                    }
                }
            }

            return qtdServicos;
        }

        public async Task<double> GetTempoMedioDemanda(ISession sessionT5, RelatorioCapacidadeAgendamentoDataTableRequest request, Configuracoes_Gerais config)
        {
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            TimeSpan? Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao?.TimeOfDay;
            List<TimeSpan> listaTempoExecucao = new List<TimeSpan>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"SELECT A.NUM_OBRA,A.PROJETO,A.CODCT, A.NUM_OBRA_ORIGINAL,A.NUM_DOC,B.DESCRICAO AS ESCRITORIO,A.EQUIPE,
                                                                A.DATA_MAXIMA,A.HORA_MAXIMA,C.DESCRICAO AS AREA,D.LOCALIDADE, A.COD_ESCRITORIO,A.COD_AREA,A.COD_LOCALIDADE,
                                                                L.TIPO_HABILIDADE,K.NOME AS SUPERVISOR,M.USUARIO_MONITOR,M.COD_SUPERV,Q.NOME AS MONITOR,S.DESLOCAMENTO,
                                                                S.DATAHORA_INICIO,S.DATAHORA_FIM

                                                                FROM TB_DOCUMENTO_PRODUCAO A
                                                                LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)
                                                                LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)
                                                                LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE P
                                                                ON(A.COD_TIPO_SERV = P.COD_TIPO_SERV)
                                                                LEFT JOIN TB_NATUREZA_PRODUCAO L
                                                                ON(L.ID = P.COD_NATUREZA) 
                                                                LEFT JOIN TB_EQUIPE M 
                                                                ON(A.EQUIPE = M.EQUIPE) 
                                                                LEFT JOIN TB_FUNCIONARIO K 
                                                                ON(K.NUM_FRE = M.COD_SUPERV)
                                                                LEFT JOIN TB_SENHA Q
                                                                ON(Q.USUARIO = M.USUARIO_MONITOR)
                                                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE N
                                                                ON(A.COD_TIPO_SERV = N.COD_TIPO_SERV)
                                                                LEFT JOIN TB_NATUREZA_PRODUCAO O
                                                                ON(O.ID = N.COD_NATUREZA) 
                                                                LEFT JOIN TB_LOCALIDADE R
                                                                ON(A.COD_ESCRITORIO = R.COD_ESCRITORIO AND A.COD_AREA = R.COD_AREA AND A.COD_LOCALIDADE = R.COD_LOCALIDADE) 
                                                                LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL S
                                                                ON(A.NUM_OBRA = S.NUM_OBRA AND A.PROJETO = S.PROJETO AND A.CODCT = S.CODCT AND A.NUM_DOC = S.NUM_DOC) ");


                List<String> condicoes = new List<string>();
                condicoes.Add("((A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.DATA_PROD = @DATA AND A.STATUS_EXECUCAO <> 9))");


                if (String.IsNullOrEmpty(request.DataInicio))
                {
                    condicoes.Add("((A.DATA_MAXIMA <= @DATA) OR(A.DATA_MAXIMA IS NULL))");
                    parametros.Add("@DATA", request.DataInicio);
                }
                else
                {
                    condicoes.Add("((A.DATA_MAXIMA BETWEEN @DATA_INICIO_DEMANDA AND @DATA) OR(A.DATA_MAXIMA IS NULL))");
                    parametros.Add("@DATA", request.DataInicio);
                    parametros.Add("@DATA_INICIO_DEMANDA", request.DataInicio);

                }

                if (String.IsNullOrEmpty(request.Equipe) == false)
                {
                    condicoes.Add("A.EQUIPE = @EQUIPE");
                    parametros.Add("@EQUIPE", request.Equipe);
                }
                if (String.IsNullOrEmpty(request.CodEscritorio) == false)
                {
                    condicoes.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                    parametros.Add("@COD_ESCRITORIO", request.CodEscritorio);
                }
                if (String.IsNullOrEmpty(request.CodArea) == false)
                {
                    condicoes.Add("A.COD_AREA = @AREA");
                    parametros.Add("@AREA", request.CodArea);
                }
                if (String.IsNullOrEmpty(request.CodNatureza) == false)
                {
                    condicoes.Add("O.ID = @NATUREZA");
                    parametros.Add("@NATUREZA", request.CodNatureza);
                }
                if (String.IsNullOrEmpty(request.Localidade) == false)
                {
                    condicoes.Add("R.LOCALIDADE = @LOCALIDADE");
                    parametros.Add("@LOCALIDADE", request.Localidade);
                }
                if (condicoes.Count > 0)
                {
                    sql.AppendLine(" WHERE ");
                    sql.AppendLine(String.Join(" AND ", condicoes));
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var tempoExecucao = reader["DATAHORA_INICIO"] != DBNull.Value && reader["DATAHORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM"]).Subtract(Convert.ToDateTime(reader["DATAHORA_INICIO"])) : Tempo_Execuxao_Padrao.Value;
                        var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToInt16(reader["DESLOCAMENTO"]) : config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes;
                        listaTempoExecucao.Add(tempoExecucao.Add(TimeSpan.FromMinutes(deslocamento)));
                    }
                }

                if (listaTempoExecucao.Count > 0)
                {
                    return Math.Round(listaTempoExecucao.Select(p => p.TotalMinutes).Sum() / listaTempoExecucao.Count);
                }
                else
                {
                    return Tempo_Execuxao_Padrao.Value.TotalMinutes + (config.Tempo_Medio_Deslocamento.HasValue ? config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes : 0);
                }

            }


        }

        public async Task<TimeSpan> GetHorasDeslocamentoDia(ISession session, DateTime data, Configuracoes_Gerais config, RelatorioCapacidadeAgendamentoDataTableRequest filtros)
        {
            TimeSpan horasDeslocamento = TimeSpan.Zero;

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT B.DESLOCAMENTO AS DESLOCAMENTO_DIA ");

                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                sql.AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL B");
                sql.AppendLine(" ON (A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC AND A.NUM_OBRA = B.NUM_OBRA)");
                sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON(A.COD_TIPO_SERV = C.COD_TIPO_SERV)");
                sql.AppendLine(" WHERE A.DATA_MAXIMA = @DATA_AGENDAMENTO ");

                #region Filtros

                if (!String.IsNullOrEmpty(filtros.CodEscritorio))
                {
                    sql.AppendLine("   AND A.COD_ESCRITORIO = @COD_ESCRITORIO ");
                    parametros.Add("@COD_ESCRITORIO", filtros.CodEscritorio);

                    if (!String.IsNullOrEmpty(filtros.CodArea))
                    {
                        sql.AppendLine("   AND A.COD_AREA = @COD_AREA ");
                        parametros.Add("@COD_AREA", filtros.CodArea);

                        if (!String.IsNullOrEmpty(filtros.CodLocalidade))
                        {
                            sql.AppendLine("   AND A.COD_LOCALIDADE = @COD_LOCALIDADE ");
                            parametros.Add("@COD_LOCALIDADE", filtros.CodLocalidade);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(filtros.Equipe))
                {
                    sql.AppendLine("   AND A.EQUIPE = @EQUIPE ");
                    parametros.Add("@EQUIPE", filtros.Equipe);
                }

                if (!String.IsNullOrEmpty(filtros.CodSuperv))
                {
                    sql.AppendLine("   AND(SELECT B.COD_SUPERV FROM TB_EQUIPE B ");
                    sql.AppendLine("       WHERE B.EQUIPE = A.EQUIPE) = @SUPERVISOR ");

                    parametros.Add("@SUPERVISOR", filtros.CodSuperv);
                }

                if (!String.IsNullOrEmpty(filtros.CodNatureza))
                {
                    sql.AppendLine("  AND C.COD_NATUREZA = @COD_NATUREZA ");
                    parametros.Add("@COD_NATUREZA", filtros.CodNatureza);
                }

                if (filtros.IdPeriodo.HasValue)
                {
                    sql.AppendLine("   AND A.COD_TURNO_SERV = @COD_TURNO ");
                    parametros.Add("@COD_TURNO", filtros.IdPeriodo);
                }

                #endregion

                parametros.Add("@DATA_AGENDAMENTO", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader["DESLOCAMENTO_DIA"] != DBNull.Value && Convert.ToDouble(reader["DESLOCAMENTO_DIA"]) != 0)
                            horasDeslocamento = horasDeslocamento.Add(TimeSpan.FromMinutes(Convert.ToDouble(reader["DESLOCAMENTO_DIA"])));
                        else if (config.Tempo_Medio_Deslocamento.HasValue)
                            horasDeslocamento = horasDeslocamento.Add(config.Tempo_Medio_Deslocamento.Value.TimeOfDay);
                    }
                }
            }

            return horasDeslocamento;
        }

        public async Task<TimeSpan> GetCapacidadeDiaConnect(ISession session, DateTime data, RelatorioCapacidadeAgendamentoDataTableRequest filtros, Configuracoes_Gerais config)
        {
            TimeSpan capacidade = TimeSpan.Zero;
            List<(string equipe, TimeSpan capacidade)> capacidades = new List<(string equipe, TimeSpan capacidade)>();
            var horario_servico_padrao = await (new Conexao<Horario_Connect>(session)).selectFirst(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Padrao);

            using (var sessionNh = HibernateUtil.GetSessionT5())
            using (var repoPerfil = new Conexao<PerfilAgendamento>(sessionNh))
            using (var repoLocalidadeAniel = new Conexao<LocalidadeAniel>(sessionNh))
            using (var repoTurno = new Conexao<Turno_ServicoAniel>(sessionNh))
            using (var com = session.Connection.CreateCommand())
            {
                LocalidadeAniel localidadeAniel = null;
                Turno_ServicoAniel PeriodoAniel = null;

                if (!String.IsNullOrEmpty(filtros.CodLocalidade) && !String.IsNullOrEmpty(filtros.CodArea) && !String.IsNullOrEmpty(filtros.CodEscritorio))
                {
                    localidadeAniel = await repoLocalidadeAniel.selectFirst(p => p.Cod_Escritorio == filtros.CodEscritorio &&
                                                         p.Cod_Area == filtros.CodArea &&
                                                         p.Cod_Localidade == Convert.ToInt32(filtros.CodLocalidade));
                }

                if (filtros.IdPeriodo.HasValue)
                {
                    PeriodoAniel = await repoTurno.selectFirst(p => p.Id == Convert.ToInt32(filtros.IdPeriodo));
                }


                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT B.NOME, ");
                sql.AppendLine(@"   CASE 
                                        WHEN E.ID IS NOT NULL THEN D.INICIO_FERIADO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN D.INICIO_DOMINGO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN D.INICIO_SEGUNDA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN D.INICIO_TERCA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN D.INICIO_QUARTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN D.INICIO_QUINTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN D.INICIO_SEXTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN D.INICIO_SABADO
                                    END AS INICIO,
                                    CASE
                                        WHEN E.ID IS NOT NULL THEN D.TERMINO_FERIADO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN D.TERMINO_DOMINGO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN D.TERMINO_SEGUNDA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN D.TERMINO_TERCA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN D.TERMINO_QUARTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN D.TERMINO_QUINTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN D.TERMINO_SEXTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN D.TERMINO_SABADO
                                    END AS TERMINO,
                                    CASE
                                        WHEN E.ID IS NOT NULL THEN D.INICIO_INTERVALO_FERIADO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN D.INICIO_INTERVALO_DOMINGO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN D.INICIO_INTERVALO_SEGUNDA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN D.INICIO_INTERVALO_TERCA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN D.INICIO_INTERVALO_QUARTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN D.INICIO_INTERVALO_QUINTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN D.INICIO_INTERVALO_SEXTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN D.INICIO_INTERVALO_SABADO
                                    END AS INICIO_INTERVALO,
                                    CASE
                                        WHEN E.ID IS NOT NULL THEN D.TERMINO_INTERVALO_FERIADO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN D.TERMINO_INTERVALO_DOMINGO
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN D.TERMINO_INTERVALO_SEGUNDA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN D.TERMINO_INTERVALO_TERCA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN D.TERMINO_INTERVALO_QUARTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN D.TERMINO_INTERVALO_QUINTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN D.TERMINO_INTERVALO_SEXTA
                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN D.TERMINO_INTERVALO_SABADO
                                    END AS TERMINO_INTERVALO,");

                sql.AppendLine("COALESCE(C.ID_PERFIL_AGENDAMENTO,B.ID_PERFIL_AGENDAMENTO) AS PERFIL_AGENDAMENTO,A.HORARIO_ATRIBUIDO,C.INDISPONIVEL AS INDISPONIVEL_CONNECT,");
                sql.AppendLine("   (SELECT FIRST 1 A.NUM_FRE FROM TB_FUNCIONARIO FUNC")
                        .AppendLine("   INNER JOIN TB_PT_DIA_FUNCIONARIO DIA_FUNC ON(FUNC.NUM_FRE = DIA_FUNC.NUM_FRE)")
                        .AppendLine("   LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA EVENTO ON(DIA_FUNC.ID_EVENTO = EVENTO.ID)")
                        .AppendLine("   WHERE EVENTO.PERMTITE_ROTEIRIZAR = 0 AND FUNC.NUM_FRE = A.NUM_FRE AND DIA_FUNC.DATA = @DATA)");
                sql.AppendLine(" AS INDISPONIVEL_POINT ");

                sql.AppendLine(" FROM TB_FUNCIONARIO A ");

                sql.AppendLine(" INNER JOIN TB_EQUIPE B ");
                sql.AppendLine(" ON (B.NUM_FRE = A.NUM_FRE) ");
                sql.AppendLine(" LEFT JOIN TB_EQUIPE_STATUS J");
                sql.AppendLine(" ON(B.EQUIPE = J.EQUIPE)");

                sql.AppendLine(" LEFT JOIN TB_HORARIO_EQUIPE_CONNECT C ");
                sql.AppendLine(" ON (C.EQUIPE = B.EQUIPE AND C.DATA = @DATA) ");

                sql.AppendLine(" INNER JOIN TB_HORARIO_CONNECT D ");
                sql.AppendLine($" ON (D.ID = COALESCE(C.ID_HORARIO,{horario_servico_padrao.Id})) ");

                sql.AppendLine(" LEFT JOIN TB_FERIADO E ");
                sql.AppendLine(" ON (E.DATA = @DATA AND ");
                sql.AppendLine("     D.TABALHA_FERIADO = 1 AND ");
                sql.AppendLine("     (CASE ");
                sql.AppendLine("          WHEN E.ABRANGENCIA = 3 THEN IIF(E.UF = A.UF_TRABALHO, 1, 0) ");
                sql.AppendLine("          ELSE 1 ");
                sql.AppendLine("      END) = 1 AND ");
                sql.AppendLine("     (CASE ");
                sql.AppendLine("          WHEN E.ABRANGENCIA = 2 THEN IIF(E.CIDADE = A.MUNICIPIO_TRABALHO, 1, 0) ");
                sql.AppendLine("          ELSE 1 ");
                sql.AppendLine("      END) = 1) ");

                sql.AppendLine("  WHERE(A.SITUACAO = 0 OR A.SITUACAO = 4) AND B.EQUIPE_CONNECT = 'S'");


                if (data == DateTime.Today)
                    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");




                #region Filtros

                if (!String.IsNullOrEmpty(filtros.CodSuperv))
                {
                    sql.AppendLine("   AND B.COD_SUPERV = @SUPERVISOR ");
                    parametros.Add("@SUPERVISOR", filtros.CodSuperv);
                }

                if (!String.IsNullOrEmpty(filtros.Equipe))
                {
                    sql.AppendLine("   AND B.EQUIPE = @EQUIPE ");
                    parametros.Add("@EQUIPE", filtros.Equipe);
                }



                #endregion

                parametros.Add("@DATA", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ? (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"])) : (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);
                        if (horarioAtribuido == HorarioAtribuido.AnielPoint)
                        {
                            var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                            if (String.IsNullOrEmpty(indisponivelPoint) == false)
                                continue;

                        }
                        else
                        {
                            var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;
                            if (indisponivelConnect)
                                continue;
                        }

                        bool apto = true;
                        if (reader["PERFIL_AGENDAMENTO"] != DBNull.Value)
                        {
                            var perfil = await repoPerfil.selectFirst(p => p.Id == Convert.ToInt64(reader["PERFIL_AGENDAMENTO"]));
                            if (!String.IsNullOrEmpty(filtros.CodEscritorio))
                            {
                                if (perfil.Areas.Any(p => p.Escritorio == filtros.CodEscritorio) == false)
                                {
                                    apto = false;
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(filtros.CodArea))
                                    {
                                        if (perfil.Areas.Any(p => p.Escritorio == filtros.CodEscritorio && p.Area == filtros.CodArea) == false)
                                        {
                                            apto = false;
                                        }
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(filtros.CodLocalidade))
                                            {
                                                if (perfil.Areas.Any(p => p.Escritorio == filtros.CodEscritorio &&
                                                                      p.Area == filtros.CodArea &&
                                                                      (p.Todas_Localidades || p.Localidade == localidadeAniel?.LocalidadeStr)) == false)
                                                {
                                                    apto = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(filtros.CodNatureza))
                            {
                                if (perfil.Naturezas.Select(p => p.Natureza?.Id).ToList().Contains(Convert.ToInt32(filtros.CodNatureza)) == false)
                                    apto = false;
                            }

                        }
                        else
                        {
                            continue;
                        }

                        if (reader["INICIO"] != DBNull.Value && reader["TERMINO"] != DBNull.Value && apto)
                        {

                            var inicio = Convert.ToDateTime(reader["INICIO"]).TimeOfDay.TotalHours;
                            var fim = Convert.ToDateTime(reader["TERMINO"]).TimeOfDay.TotalHours;


                            if (PeriodoAniel?.Hora_Ini != null && PeriodoAniel?.Hora_Fim != null)
                            {
                                if (inicio < PeriodoAniel.Hora_Ini.Value.TimeOfDay.TotalHours)
                                {
                                    inicio = PeriodoAniel.Hora_Ini.Value.TimeOfDay.TotalHours;
                                }

                                if (fim > PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours)
                                {
                                    fim = PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours;
                                }
                            }



                            var horas = fim > inicio ? fim - inicio : 0;



                            if (reader["INICIO_INTERVALO"] != DBNull.Value && reader["TERMINO_INTERVALO"] != DBNull.Value)
                            {
                                var inicioIntervaloDatetime = Convert.ToDateTime(reader["INICIO_INTERVALO"]);
                                var fimIntervaloDatetime = Convert.ToDateTime(reader["TERMINO_INTERVALO"]);
                                var inicioIntervalo = Convert.ToDateTime(reader["INICIO_INTERVALO"]).TimeOfDay.TotalHours;
                                var fimIntervalo = Convert.ToDateTime(reader["TERMINO_INTERVALO"]).TimeOfDay.TotalHours;

                                if (PeriodoAniel?.Hora_Ini != null && PeriodoAniel?.Hora_Fim != null)
                                {
                                    if (inicioIntervalo >= PeriodoAniel.Hora_Ini.Value.TimeOfDay.TotalHours &&
                                        fimIntervalo >= PeriodoAniel.Hora_Ini.Value.TimeOfDay.TotalHours &&
                                        inicioIntervalo <= PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours &&
                                        fimIntervalo <= PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours)
                                    {
                                        if (fimIntervalo > PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours)
                                        {
                                            fim = PeriodoAniel.Hora_Fim.Value.TimeOfDay.TotalHours;
                                        }

                                        horas -= fimIntervalo - inicioIntervalo;
                                    }
                                    else if (fimIntervalo > inicio && BetweenIgual(PeriodoAniel.Hora_Ini.Value, inicioIntervaloDatetime, fimIntervaloDatetime))
                                    {
                                        inicio = fimIntervalo;
                                        horas = fim > inicio ? fim - inicio : 0;
                                    }

                                }
                                else
                                {
                                    horas -= fimIntervalo - inicioIntervalo;
                                }
                            }

                            string equipe = Convert.ToString(reader["NOME"]);
                            capacidades.Add((equipe, TimeSpan.FromHours(horas)));
                            capacidade += TimeSpan.FromHours(horas);
                        }
                    }
                }
            }

            return capacidade;
        }

        public static bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }

        public async Task<TimeSpan> GetCapacidadeDiaPoint(ISession session, DateTime data, RelatorioCapacidadeAgendamentoDataTableRequest filtros)
        {
            TimeSpan capacidade = TimeSpan.Zero;

            using (var apiJornadaPoint = new JornadaPontoApiController())
            using (var com = session.Connection.CreateCommand())
            using (ISession sessionNh = HibernateUtil.GetSessionT5())
            using (var repoJornada = new Conexao<Tb_Pt_Jornada_Funcionario>(sessionNh))
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT A.NUM_FRE, B.ID_JORNADA, C.ID AS ID_EVENTO ");
                sql.AppendLine(" FROM TB_FUNCIONARIO A ");

                sql.AppendLine(" INNER JOIN TB_PT_DIA_FUNCIONARIO AS B ");
                sql.AppendLine(" ON(B.NUM_FRE = A.NUM_FRE) ");
                sql.AppendLine(" LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA AS C ");
                sql.AppendLine(" ON(C.ID = B.ID_EVENTO) ");
                sql.AppendLine(" LEFT JOIN TB_EQUIPE AS EQUIPE ");
                sql.AppendLine(" ON(A.NUM_FRE = EQUIPE.NUM_FRE) ");

                sql.AppendLine(" WHERE B.DATA = @DATA ");

                #region Filtros



                if (!String.IsNullOrEmpty(filtros.CodSuperv))
                {
                    sql.AppendLine(" AND EQUIPE.COD_SUPERV = @SUPERVISOR ");

                    parametros.Add("@SUPERVISOR", filtros.CodSuperv);
                }

                if (!String.IsNullOrEmpty(filtros.Equipe))
                {
                    sql.AppendLine(" AND EQUIPE.EQUIPE = @EQUIPE ");

                    parametros.Add("@EQUIPE", filtros.Equipe);
                }

                #endregion

                parametros.Add("@DATA", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        string numFre = Convert.ToString(reader["NUM_FRE"]);
                        bool feriado = await apiJornadaPoint.GetFeriado(numFre, data);

                        if (reader["ID_JORNADA"] != DBNull.Value)
                        {
                            long idJornada = Convert.ToInt64(reader["ID_JORNADA"]);
                            var jornada = await repoJornada.selectFirst(p => p.Id == idJornada);

                            long? idEvento = reader["ID_EVENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_EVENTO"]) : (long?)null;
                            var folhaPonto = (data, idEvento);

                            TimeSpan capacidadeFuncionario = await apiJornadaPoint.GetHorasPrevistasDia(sessionNh, jornada, folhaPonto, feriado);

                            capacidade = capacidade.Add(capacidadeFuncionario);
                        }
                    }
                }
            }

            return capacidade;
        }

        public async Task<TimeSpan> GetHorasAgendadas(ISession session, DateTime data, RelatorioCapacidadeAgendamentoDataTableRequest filtros)
        {
            TimeSpan horasAgendadas = TimeSpan.Zero;

            using (var com = session.Connection.CreateCommand())
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNh))
            {
                var config = await repoConfig.selectFirst();
                var tempoExecucaoPadrao = config.Tempo_Execucao_Padrao?.TimeOfDay;

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT S.DATAHORA_INICIO,S.DATAHORA_FIM ");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A ");
                sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL S");
                sql.AppendLine(" ON(A.NUM_OBRA = S.NUM_OBRA AND A.PROJETO = S.PROJETO AND A.CODCT = S.CODCT AND A.NUM_DOC = S.NUM_DOC)");

                sql.AppendLine(" WHERE A.DATA_MAXIMA = @DATA ");

                sql.AppendLine(" AND ((A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 9) OR (A.DATA_PROD = @DATA AND A.STATUS_EXECUCAO <> 9))");
                #region Filtros

                if (!String.IsNullOrEmpty(filtros.CodEscritorio))
                {
                    sql.AppendLine("   AND A.COD_ESCRITORIO = @COD_ESCRITORIO ");
                    parametros.Add("@COD_ESCRITORIO", filtros.CodEscritorio);

                    if (!String.IsNullOrEmpty(filtros.CodArea))
                    {
                        sql.AppendLine("   AND A.COD_AREA = @COD_AREA ");
                        parametros.Add("@COD_AREA", filtros.CodArea);

                        if (!String.IsNullOrEmpty(filtros.CodLocalidade))
                        {
                            sql.AppendLine("   AND A.COD_LOCALIDADE = @COD_LOCALIDADE ");
                            parametros.Add("@COD_LOCALIDADE", filtros.CodLocalidade);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(filtros.Equipe))
                {
                    sql.AppendLine("   AND A.EQUIPE = @EQUIPE ");
                    parametros.Add("@EQUIPE", filtros.Equipe);
                }

                if (!String.IsNullOrEmpty(filtros.CodSuperv))
                {
                    sql.AppendLine("   AND(SELECT C.COD_SUPERV FROM TB_EQUIPE C ");
                    sql.AppendLine("       WHERE C.EQUIPE = A.EQUIPE) = @SUPERVISOR ");

                    parametros.Add("@SUPERVISOR", filtros.CodSuperv);
                }

                if (!String.IsNullOrEmpty(filtros.CodNatureza))
                {
                    sql.AppendLine("  AND B.COD_NATUREZA = @COD_NATUREZA ");
                    parametros.Add("@COD_NATUREZA", filtros.CodNatureza);
                }

                if (filtros.IdPeriodo.HasValue)
                {
                    sql.AppendLine("   AND A.COD_TURNO_SERV = @COD_TURNO ");
                    parametros.Add("@COD_TURNO", filtros.IdPeriodo);
                }

                #endregion

                parametros.Add("@DATA", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        TimeSpan tempoExecucao = TimeSpan.Zero;

                        if (reader["DATAHORA_INICIO"] != DBNull.Value && reader["DATAHORA_FIM"] != DBNull.Value)
                            tempoExecucao = Convert.ToDateTime(reader["DATAHORA_FIM"]).Subtract(Convert.ToDateTime(reader["DATAHORA_INICIO"]));
                        else if (tempoExecucaoPadrao.HasValue)
                            tempoExecucao = tempoExecucaoPadrao.Value;

                        horasAgendadas = horasAgendadas.Add(tempoExecucao);
                    }
                }
            }

            return horasAgendadas;
        }
        private async Task<Horario_Connect> GetHorarioAgendamento(ISession session)
        {
            using (var repo = new Conexao<Horario_Connect>(session))
                return await repo.selectFirst(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Padrao == true && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento);
        }
    }
}