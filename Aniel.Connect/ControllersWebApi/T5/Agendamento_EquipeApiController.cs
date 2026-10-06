using Aniel.Connect.ConnectBusiness.SMS;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.SMS;
using Aniel.Connect.Repository.T5.Agendamento;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.GrupoArea;
using Aniel.Connect.Repository.T5.Horario;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.T5.PainelPlanejamentoCapacidade;
using Aniel.Connect.Repository.T5.Turno;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Conventions;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Util;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity.DomainModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Repository.T5.Agendamento;
using Aniel.Connect.Repository.T5.Horario;
using Aniel.Connect.Repository.T5.Turno;
using Aniel.Connect.Repository.T5.GrupoArea;
using System.Linq.Expressions;
using OpenXmlPowerTools;


namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Agendamento_EquipeApiController : BaseApiController
    {
        public int QuantidadeDias { get; set; }

        public Agendamento_EquipeApiController(int quantidadeDias)
        {
            this.QuantidadeDias = quantidadeDias;
        }

        public Agendamento_EquipeApiController()
        {
            this.QuantidadeDias = 14;
        }


        List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)> perfilAgendamentoCache = new List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)>();

        #region Index
        [HttpGet()]
        [Route("api/Agendamento_Equipeapi/GetDatasDisponiveis")]
        public async Task<GetApiRetorno<List<AgendamentoDiaModel>>> GetDatasDisponiveis(Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area,
            Int32? Cod_Localidade, String LocalidadeStr, double? TempoExecucaoServicoMinutos, DateTime? DataInicial)
        {
            try
            {
                //if (!Cod_Tipo_Serv.HasValue)
                //{
                //    throw new Exception("Tipo de Servico não informado");
                //}
                //if (String.IsNullOrEmpty(Cod_Grupo_Area))
                //{
                //    throw new Exception("Grupo Area não informado");
                //}
                //if (String.IsNullOrEmpty(Cod_Area))
                //{
                //    throw new Exception("Area não informado");
                //}
                //if (!Cod_Localidade.HasValue)
                //{
                //    throw new Exception("Localidade não informado");
                //}

                GetApiRetorno<List<ObjBusca<DateTime>>> retorno = new GetApiRetorno<List<ObjBusca<DateTime>>>();
                DateTime? TempoExecucaoServico = TempoExecucaoServicoMinutos == null ? (DateTime?)null : DateTime.Today.Add(TimeSpan.FromMinutes(TempoExecucaoServicoMinutos.Value));

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    return await GetDatasDisponiveis_Det(session, sessionT5, Cod_Tipo_Serv, Cod_Natureza, Cod_Grupo_Area, Cod_Area, Cod_Localidade, LocalidadeStr, TempoExecucaoServico, DataInicial.GetValueOrDefault(DateTime.Today));
                }
            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<AgendamentoDiaModel>>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }
        }

        private async Task<GetApiRetorno<List<AgendamentoDiaModel>>> GetDatasDisponiveis_Det(ISession session, ISession sessionT5, Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area,
                                 Int32? Cod_Localidade, String LocalidadeStr, DateTime? TempoExecucaoServico, DateTime dataInicial)
        {
            using (var repoFeriado = new Conexao<Feriado>(session))
            {
                List<Feriado> feriados = (await repoFeriado.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Data >= dataInicial && p.Data <= dataInicial.AddDays(this.QuantidadeDias))).ToList();

                var horariosAgendamento = await GetHorarioAgendamento(sessionT5);

                if (horariosAgendamento == null)
                {
                    throw new Exception("Horario padrão de agendamento não definido no sistema");
                }

                return new GetApiRetorno<List<AgendamentoDiaModel>>
                {
                    Ok = true,
                    Objeto = await GetDatasValidas(session, sessionT5, horariosAgendamento, feriados, Cod_Tipo_Serv, Cod_Natureza,
                                                                       Cod_Grupo_Area, Cod_Area, Cod_Localidade, LocalidadeStr,
                                                                       TempoExecucaoServico, dataInicial)
                };
            }
        }

        private async Task<GetApiRetorno<List<AgendamentoDiaModel>>> GetDatasDisponiveis_DetMobile(ISession session, ISession sessionT5, Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area,
                         Int32? Cod_Localidade, String LocalidadeStr, DateTime? TempoExecucaoServico, DateTime DataIni, DateTime DataFin, DateTime dataInicial)
        {
            using (var repoFeriado = new Conexao<Feriado>(session))
            {
                List<Feriado> feriados = (await repoFeriado.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Data >= dataInicial && p.Data <= dataInicial.AddDays(this.QuantidadeDias))).ToList();

                var horariosAgendamento = await GetHorarioAgendamento(sessionT5);

                if (horariosAgendamento == null)
                {
                    throw new Exception("Horario padrão de agendamento não definido no sistema");
                }

                return new GetApiRetorno<List<AgendamentoDiaModel>>
                {
                    Ok = true,
                    Objeto = await GetDatasValidasMobile(session, sessionT5, horariosAgendamento, feriados, Cod_Tipo_Serv, Cod_Natureza,
                                                                       Cod_Grupo_Area, Cod_Area, Cod_Localidade, LocalidadeStr,
                                                                       TempoExecucaoServico, DataIni, DataFin)
                };
            }
        }

        private async Task<Horario_Connect> GetHorarioAgendamento(ISession session)
        {
            using (var repo = new Conexao<Horario_Connect>(session))
                return await repo.selectFirst(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Padrao == true && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento);
        }

        private async Task<List<AgendamentoDiaModel>> GetDatasValidas(ISession session, ISession sessionT5, Horario_Connect horario_Connect, List<Feriado> feriados,
            Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area, Int32? Cod_Localidade, String LocalidadeStr, DateTime? TempoExecucaoServico, DateTime dataInicial)
        {
            var retorno = new List<AgendamentoDiaModel>();
            string usuario = User.Identity.GetUserId();
            var acessos = await (new AcessoApiController()).GetArea_ModuloUsuario(usuario);
            bool ignorarTempo = AcessoHelper.TemAcesso(acessos.Objeto, "287");


            using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
            {
                //Pega as configurações da Empresa
                Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();
                var turnosBanco = repositoryTurno.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList();
                if (config.Criterio_Area)
                {
                    if (String.IsNullOrEmpty(Cod_Grupo_Area) || String.IsNullOrEmpty(Cod_Area) || String.IsNullOrEmpty(LocalidadeStr))
                    {
                        throw new Exception("Serviço sem localidade vinculada.");
                    }
                }

                if (config.Criterio_Habilidade)
                {
                    if (Cod_Natureza.HasValue == false)
                    {
                        throw new Exception("Serviço sem natureza vinculada.");
                    }
                }

                DateTime? tempoExecucao = !TempoExecucaoServico.HasValue ? config.Tempo_Execucao_Padrao : TempoExecucaoServico;

                if (!tempoExecucao.HasValue || tempoExecucao.Value.TimeOfDay == new TimeSpan(0))
                {
                    throw new Exception("Tempo de Execução não definido no sistema");
                }
                if (!config.Tempo_Medio_Deslocamento.HasValue)
                {
                    throw new Exception("Tempo Medio de Deslocamento não definido no sistema");
                }

                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                using (var horarioApiController = new HorarioApiController())
                {
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                    ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5);
                    var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();

                    Horario_Connect horario_servico_padrao = horarios_servico.Find(p => p.Padrao == true);

                    if (horario_servico_padrao == null)
                    {
                        throw new Exception("Horario de Serviço Padrão não definido no sistema");
                    }

                    Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);
                    Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));
                    List<AgendamentoDadosModel> jaAgendados = await GetJaAgendados(sessionT5, dataInicial, dataInicial.AddDays(this.QuantidadeDias));
                    var listaLimites = limiteAgendamentoBusiness.GetPercNaturezaByArea(Cod_Grupo_Area,
                                                                                       Cod_Area,
                                                                                       Cod_Natureza.Value);

                    var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                    var perfisLocalidades = GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                    for (DateTime dia = dataInicial; dia <= dataInicial.AddDays(this.QuantidadeDias); dia = dia.AddDays(1))
                    {
                        DayOfWeek diaSemana = dia.DayOfWeek;
                        bool feriado = feriados.Find(p => p.Data == dia) != null;
                        List<AgendamentoEquipeHorarioModel> forcaTrabalho = await GetForcaTrabalho(sessionT5, dia, config, perfisNaturezas, perfisLocalidades);
                        List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == dia).Select(p =>
                        {
                            AgendamentoEquipeHorarioModel forcaTrabalhoEquipe = forcaTrabalho.Find(y => y.Equipe == p.Equipe);
                            bool capaz = false;
                            if (forcaTrabalhoEquipe != null)
                            {
                                RoteirizacaoBusiness helper = new RoteirizacaoBusiness();
                                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                                {
                                    Cod_Escritorio = p.DadosServico.Cod_Escritorio,
                                    Cod_Area = p.DadosServico.Cod_Area,
                                    Cod_Localidade = p.DadosServico.Cod_Localidade,
                                    Cod_Natureza = p.DadosServico.Cod_Natureza,
                                    LocalidadeStr = p.DadosServico.LocalidadeStr
                                };


                                TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = forcaTrabalhoEquipe.Naturezas,
                                    Localidades = forcaTrabalhoEquipe.Localidades,
                                    Equipe = forcaTrabalhoEquipe.Equipe,

                                };

                                capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                                p.Capaz = capaz;
                            }

                            return p;
                        }).ToList();


                        LimiteAgendamentoAniel limAgendData = null;

                        if (Cod_Natureza.HasValue)
                            limAgendData = listaLimites.FirstOrDefault(p => p.DataInicio <= dia && dia <= p.DataFim);



                        AgendamentoDiaModel resultadoDia = BuscarAgendamentoDia(dia,
                                                                                jaAgendadosDia,
                                                                                forcaTrabalho,
                                                                                horarios_servico,
                                                                                horario_Connect,
                                                                                config,
                                                                                feriado,
                                                                                new DadosServicoAgendamento
                                                                                {
                                                                                    Cod_Area = Cod_Area,
                                                                                    Cod_Escritorio = Cod_Grupo_Area,
                                                                                    Cod_Localidade = Cod_Localidade,
                                                                                    Cod_Natureza = Cod_Natureza,
                                                                                    LocalidadeStr = LocalidadeStr,
                                                                                    TempoExecucaoMinutos = tempoExecucao
                                                                                },
                                                                                turnosBanco,
                                                                                limAgendData,
                                                                                ignorarTempo);

                        if (resultadoDia != null)
                            retorno.Add(resultadoDia);
                    }



                    foreach (var dia in retorno)
                    {
                        List<AgendamentoHorasDisponiveis> horasCapacidadeAgrupadas = new List<AgendamentoHorasDisponiveis>();
                        foreach (var item in dia.HorasCapacidade.OrderBy(p => p.Hora_Inicio).GroupBy(p => new { p.Hora_Inicio, p.Hora_Final, p.IdPerfil }))
                        {
                            var itemAgrupado = item.FirstOrDefault(p => p.Equipe_Tmp != null) ?? item.FirstOrDefault();
                            itemAgrupado.Aptos = item.Sum(p => p.Aptos);
                            itemAgrupado.Forca_Trabalho = item.Sum(p => p.Forca_Trabalho);
                            itemAgrupado.ForcaTrabalhoLimiteAgendamento = item.Sum(p => p.ForcaTrabalhoLimiteAgendamento);
                            itemAgrupado.Lista_Ja_Agendados = item.ToList().SelectMany(p => p.Lista_Ja_Agendados).ToList();
                            itemAgrupado.Lista_Forca_Trabalho = item.ToList().SelectMany(p => p.Lista_Forca_Trabalho).ToList();
                            itemAgrupado.ListaForcaTrabalhoComLimite = item.ToList().SelectMany(p => p.ListaForcaTrabalhoComLimite).ToList();
                            itemAgrupado.ForcaTrabalhoMinutosPrevistos = item.Sum(p => p.ForcaTrabalhoMinutosPrevistos);
                            itemAgrupado.ForcaTrabalhoMinutosPrevistosComLimite = item.Sum(p => p.ForcaTrabalhoMinutosPrevistosComLimite);
                            horasCapacidadeAgrupadas.Add(itemAgrupado);
                        }

                        dia.HorasCapacidade = horasCapacidadeAgrupadas;
                    }

                    if (config.Considerar_Agendamento_Periodo)
                    {
                        foreach (var item in retorno.SelectMany(p => p.HorasCapacidade))
                        {
                            item.Turno = PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.Value.TimeOfDay)?.Descricao;
                        }

                        foreach (var item in retorno.SelectMany(p => p.Horas))
                        {
                            item.Turno = PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.Value.TimeOfDay)?.Descricao;
                        }

                        foreach (var item in retorno)
                        {
                            var turnos = item.HorasCapacidade.GroupBy(p => new { p.Turno, p.IdPerfil });
                            item.HorasTurno = turnos.Select(p =>
                            {
                                var forcaTrabalhoPerfilCapacidadeTecnicoTmp = item.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);
                                var forcaTrabalhoPerfilTmp = item.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);


                                var forcaTrabalhoBaseadoEmTempo = forcaTrabalhoPerfilTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                var forcaTrabalho = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                var forcaTrabalhoComLimite = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistosComLimite);
                                var listaNumOS = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList()
                                                                       .Where(y => y.DadosServico.Periodo == p.Key.Turno)
                                                                       .Select(y => y.DadosServico.NumObra).Distinct();


                                var agendamentosForaFaixaNoMesmoTurnoMinutos = item.ListaJaAgendadosForaFaixa.FindAll(y => p.Key.IdPerfil == y.IdPerfil && PeriodoApiController.BuscarTurno(turnosBanco, y.DataHoraInicio, tempoExecucao.Value.TimeOfDay)?.Descricao == p.Key.Turno).Sum(p => p.DuracaoMinutos);
                                var todosAgendamentos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Sum(p => p.DuracaoMinutos);
                                var agendamentosManuaisMinutos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);

                                if (forcaTrabalhoPerfilTmp.FindAll(p => p.Deslocamento == 0).Count > 0)
                                {
                                    double minutosExtrasDeslocamento = 0;
                                    foreach (var itemHorario in forcaTrabalhoPerfilTmp.FindAll(y => y.Deslocamento == 0))
                                    {
                                        minutosExtrasDeslocamento = itemHorario.Forca_Trabalho * tempoDeslocamentoMinutos;
                                        forcaTrabalho += minutosExtrasDeslocamento;
                                        forcaTrabalhoComLimite += minutosExtrasDeslocamento;
                                        forcaTrabalhoBaseadoEmTempo += minutosExtrasDeslocamento;
                                    }
                                }

                                var capacidadeRestante = (forcaTrabalhoBaseadoEmTempo - (todosAgendamentos + agendamentosForaFaixaNoMesmoTurnoMinutos));
                                int capacidadePeriodoBaseadoEmTempoHoraisTotais = Convert.ToInt32(Math.Floor(capacidadeRestante / (tempoExecucaoMinutos + tempoDeslocamentoMinutos)));
                                int capacidadePeriodoBaseadoEmTempo = Convert.ToInt32(Math.Floor((forcaTrabalho - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucaoMinutos + tempoDeslocamentoMinutos)));
                                int capacidadePeriodo = item.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(p => p.Lista_Forca_Trabalho.Count);

                                int capacidadePeriodoBaseadoEmTempoComLimite = Convert.ToInt32(Math.Floor((forcaTrabalhoComLimite - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucaoMinutos + tempoDeslocamentoMinutos)));
                                int capacidadePeriodoComLimite = item.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(p => p.ListaForcaTrabalhoComLimite.Count);

                                if (capacidadePeriodoBaseadoEmTempo < capacidadePeriodo)
                                {
                                    capacidadePeriodo = capacidadePeriodoBaseadoEmTempo;
                                }

                                if (capacidadePeriodoBaseadoEmTempoComLimite < capacidadePeriodoComLimite)
                                {
                                    capacidadePeriodoComLimite = capacidadePeriodoBaseadoEmTempoComLimite;
                                }

                                if (capacidadePeriodoBaseadoEmTempoHoraisTotais < capacidadePeriodo)
                                {
                                    capacidadePeriodo = capacidadePeriodoBaseadoEmTempoHoraisTotais;
                                }

                                var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null).OrderBy(p => p.LimiteAgendamentoUltrapassado);
                                if (horariosEquipesTemp.FirstOrDefault() == null)
                                    return null;

                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key.Turno,
                                    Aptos = (long)capacidadePeriodo,
                                    Turno = p.Key.Turno,
                                    Equipe_Tmp = horariosEquipesTemp.FirstOrDefault()?.Equipe_Tmp,
                                    EquipeTmpComLimite = horariosEquipesTemp.FirstOrDefault()?.EquipeTmpComLimite,
                                    Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                                    Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                                    Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                                    Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                                    Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                                    AptosPorLimite = (long)capacidadePeriodoComLimite,
                                    LimiteAgendamentoUltrapassado = item.LimiteAgendamentoUltrapassadoDia
                                };

                            }).Where(p => p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null).ToList();
                        }

                        foreach (var item in retorno)
                        {
                            var turnos = item.HorasTurno.GroupBy(p => new { p.Turno });
                            item.HorasTurno = turnos.Select(p =>
                            {
                                var forcaTrabalhoPerfilTmp = item.Horas.FindAll(y => y.Turno == p.Key.Turno);
                                var forcaTrabalhoBaseadoEmTempo = forcaTrabalhoPerfilTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                var listaNumOS = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList()
                                                                       .Where(y => y.DadosServico.Periodo == p.Key.Turno)
                                                                       .Select(y => y.DadosServico.NumObra).Distinct();

                                var agendamentosForaFaixaNoMesmoTurnoMinutos = item.ListaJaAgendadosForaFaixa.FindAll(y => PeriodoApiController.BuscarTurno(turnosBanco, y.DataHoraInicio, tempoExecucao.Value.TimeOfDay)?.Descricao == p.Key.Turno).Sum(p => p.DuracaoMinutos);
                                var todosAgendamentos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Sum(p => p.DuracaoMinutos);
                                var capacidadeRestante = Math.Floor(forcaTrabalhoBaseadoEmTempo - (todosAgendamentos + agendamentosForaFaixaNoMesmoTurnoMinutos));
                                int capacidadePeriodoBaseadoEmTempoHoraisTotais = Convert.ToInt32(capacidadeRestante / (tempoExecucaoMinutos + tempoDeslocamentoMinutos));

                                var capacidade = p.ToList().Sum(p => p.Aptos);

                                if (capacidadePeriodoBaseadoEmTempoHoraisTotais < capacidade)
                                    capacidade = capacidadePeriodoBaseadoEmTempoHoraisTotais;


                                var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null).OrderBy(p => p.LimiteAgendamentoUltrapassado);
                                var capacidadePeriodoComLimite = p.ToList().Sum(p => p.AptosPorLimite);

                                //temporario para nao gerar confusão do relatorio
                                var capacidadeRestanteRetorno = capacidade * (tempoExecucaoMinutos + tempoDeslocamentoMinutos);

                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key.Turno,
                                    Aptos = capacidade,
                                    Turno = p.Key.Turno,
                                    Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(p => p != null).FirstOrDefault(),
                                    EquipeTmpComLimite = p.ToList().Select(y => y.EquipeTmpComLimite).Where(p => p != null).FirstOrDefault(),
                                    Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                                    Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                                    Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                                    Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                                    Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                                    AptosPorLimite = (long)capacidadePeriodoComLimite,
                                    LimiteAgendamentoUltrapassado = item.LimiteAgendamentoUltrapassadoDia,
                                    MinutosCapacidadeRestante = Convert.ToInt64(capacidadeRestanteRetorno),
                                    MinutosAgendamentoDentroFaixa = Convert.ToInt64(todosAgendamentos),
                                    MinutosAgendamentoForaFaixa = Convert.ToInt64(agendamentosForaFaixaNoMesmoTurnoMinutos)
                                };

                            }).Where(p => p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null).ToList();
                        }

                        retorno = retorno.FindAll(p => p.HorasTurno.FindAll(y => y.Aptos > 0).Count > 0);
                    }
                    else
                    {
                        foreach (var item in retorno)
                        {
                            item.Horas = item.Horas.Where(p => p.Aptos > 0).ToList();
                            var horarios = item.Horas.GroupBy(p => new { p.Hora_InicioStr, p.Hora_FinalStr, p.Hora_Inicio, p.Hora_Final, p.Display, p.IdPerfil });
                            item.Horas = horarios.Select(p =>
                            {
                                var forcaTrabalho = p.ToList().Sum(y => y.Forca_Trabalho);
                                var forcaTrabalhoLimiteAgendamento = p.ToList().Sum(y => y.ForcaTrabalhoLimiteAgendamento);
                                var listaNumOS = p.ToList().SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                var agendamentos = listaNumOS.Select(y => p.ToList().SelectMany(z => z.Lista_Ja_Agendados).First(z => z.DadosServico.NumObra == y));
                                var capacidadePeriodo = forcaTrabalho - agendamentos.Count();
                                var capacidadePeriodoComLimite = forcaTrabalhoLimiteAgendamento - agendamentos.Count();


                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key.Display,
                                    Aptos = capacidadePeriodo,
                                    Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(p => p != null).FirstOrDefault(),
                                    Hora_InicioStr = p.Key.Hora_InicioStr,
                                    Hora_FinalStr = p.Key.Hora_FinalStr,
                                    Hora_Inicio = p.Key.Hora_Inicio,
                                    Hora_Final = p.Key.Hora_Final,
                                    AptosPorLimite = (long)capacidadePeriodoComLimite,
                                    EquipeTmpComLimite = p.ToList().Select(y => y.EquipeTmpComLimite).Where(p => p != null).FirstOrDefault(),
                                    LimiteAgendamentoUltrapassado = capacidadePeriodoComLimite <= 0 ? true : false
                                };
                            }).Where(p => p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null).ToList();
                        }

                        foreach (var item in retorno)
                        {
                            var agrupamento = item.Horas.GroupBy(p => new { p.Hora_InicioStr, p.Hora_FinalStr, p.Hora_Inicio, p.Hora_Final, p.Display });

                            item.Horas = agrupamento.Select(p =>
                            {
                                var capacidadePeriodoComLimite = p.ToList().Sum(p => p.AptosPorLimite);
                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key.Display,
                                    Aptos = p.ToList().Sum(p => p.Aptos),
                                    Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(p => p != null).FirstOrDefault(),
                                    Hora_InicioStr = p.Key.Hora_InicioStr,
                                    Hora_FinalStr = p.Key.Hora_FinalStr,
                                    Hora_Inicio = p.Key.Hora_Inicio,
                                    Hora_Final = p.Key.Hora_Final,
                                    AptosPorLimite = (long)capacidadePeriodoComLimite,
                                    EquipeTmpComLimite = p.ToList().Select(y => y.EquipeTmpComLimite).Where(p => p != null).FirstOrDefault(),
                                    LimiteAgendamentoUltrapassado = capacidadePeriodoComLimite <= 0 ? true : false
                                };
                            }).Where(p => p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null).ToList();
                        }

                        retorno = retorno.FindAll(p => p.Horas.Count > 0);

                    }


                    return retorno;
                }
            }
        }


        private async Task<List<AgendamentoDiaModel>> GetDatasValidasMobile(ISession session, ISession sessionT5, Horario_Connect horario_Connect, List<Feriado> feriados,
            Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area, Int32? Cod_Localidade, String LocalidadeStr, DateTime? TempoExecucaoServico, DateTime DataIni, DateTime DataFin)
        {
            var retorno = new List<AgendamentoDiaModel>();
            string usuario = User.Identity.GetUserId();
            var acessos = await (new AcessoApiController()).GetArea_ModuloUsuario(usuario);
            bool ignorarTempo = AcessoHelper.TemAcesso(acessos.Objeto, "287");

            using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
            {
                //Pega as configurações da Empresa
                Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();
                var turnosBanco = repositoryTurno.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList();

                DateTime? tempoExecucao = !TempoExecucaoServico.HasValue ? config.Tempo_Execucao_Padrao : TempoExecucaoServico;

                if (!tempoExecucao.HasValue || tempoExecucao.Value.TimeOfDay == new TimeSpan(0))
                {
                    throw new Exception("Tempo de Execução não definido no sistema");
                }
                if (!config.Tempo_Medio_Deslocamento.HasValue)
                {
                    throw new Exception("Tempo Medio de Deslocamento não definido no sistema");
                }

                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var horarioApiController = new HorarioApiController())
                using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                {

                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();

                    Horario_Connect horario_servico_padrao = horarios_servico.Find(p => p.Padrao == true);

                    if (horario_servico_padrao == null)
                    {
                        throw new Exception("Horario de Serviço Padrão não definido no sistema");
                    }

                    Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);
                    Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));
                    List<AgendamentoDadosModel> jaAgendados = await GetJaAgendados(sessionT5, DataIni, DataFin);
                    var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                    var perfisLocalidades = GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                    for (DateTime dia = DataIni; dia <= DataFin; dia = dia.AddDays(1))
                    {
                        DayOfWeek diaSemana = dia.DayOfWeek;
                        bool feriado = feriados.Find(p => p.Data == dia) != null;
                        List<AgendamentoEquipeHorarioModel> forcaTrabalho = await GetForcaTrabalho(sessionT5, dia, config, perfisNaturezas, perfisLocalidades);

                        List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == dia).Select(p =>
                        {
                            AgendamentoEquipeHorarioModel forcaTrabalhoEquipe = forcaTrabalho.Find(y => y.Equipe == p.Equipe);
                            bool capaz = false;
                            if (forcaTrabalhoEquipe != null)
                            {
                                RoteirizacaoBusiness helper = new RoteirizacaoBusiness();
                                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                                {
                                    Cod_Escritorio = p.DadosServico.Cod_Escritorio,
                                    Cod_Area = p.DadosServico.Cod_Area,
                                    Cod_Localidade = p.DadosServico.Cod_Localidade,
                                    Cod_Natureza = p.DadosServico.Cod_Natureza,
                                    LocalidadeStr = p.DadosServico.LocalidadeStr
                                };


                                TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = forcaTrabalhoEquipe.Naturezas,
                                    Localidades = forcaTrabalhoEquipe.Localidades,
                                    Equipe = forcaTrabalhoEquipe.Equipe,

                                };

                                capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                                p.Capaz = capaz;
                            }

                            return p;
                        }).ToList();

                        ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5);
                        INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);

                        var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);

                        LimiteAgendamentoAniel limAgendData = null;

                        if (Cod_Natureza.HasValue)
                            limAgendData =
                                limiteAgendamentoBusiness
                                    .GetPercNaturezaByAreaData(
                                        Cod_Grupo_Area,
                                        Cod_Area,
                                        Cod_Natureza.Value,
                                        dia
                                    );

                        AgendamentoDiaModel resultadoDia = BuscarAgendamentoDia(dia,
                                                                                jaAgendadosDia,
                                                                                forcaTrabalho,
                                                                                horarios_servico,
                                                                                horario_Connect,
                                                                                config,
                                                                                feriado,
                                                                                new DadosServicoAgendamento
                                                                                {
                                                                                    Cod_Area = Cod_Area,
                                                                                    Cod_Escritorio = Cod_Grupo_Area,
                                                                                    Cod_Localidade = Cod_Localidade,
                                                                                    Cod_Natureza = Cod_Natureza,
                                                                                    LocalidadeStr = LocalidadeStr,
                                                                                    TempoExecucaoMinutos = tempoExecucao
                                                                                },
                                                                                turnosBanco,
                                                                                limAgendData,
                                                                                ignorarTempo);

                        if (resultadoDia != null)
                            retorno.Add(resultadoDia);
                    }

                    foreach (var dia in retorno)
                    {
                        List<AgendamentoHorasDisponiveis> horasAgrupadas = new List<AgendamentoHorasDisponiveis>();
                        foreach (var item in dia.Horas.OrderBy(p => p.Hora_Inicio).GroupBy(p => new { p.Hora_Inicio, p.Hora_Final }))
                        {
                            var itemAgrupado = item.FirstOrDefault();
                            itemAgrupado.Aptos = item.Sum(p => p.Aptos);
                            itemAgrupado.Forca_Trabalho = item.Sum(p => p.Forca_Trabalho);
                            itemAgrupado.Ja_Agendados = item.Sum(p => p.Ja_Agendados);
                            horasAgrupadas.Add(itemAgrupado);
                        }

                        dia.Horas = horasAgrupadas;
                    }




                    if (config.Considerar_Agendamento_Periodo)
                    {
                        foreach (var item in retorno.SelectMany(p => p.Horas))
                        {


                            item.Turno = PeriodoApiController.BuscarTurno(sessionT5, item.Hora_Inicio, tempoExecucao.Value.TimeOfDay)?.Descricao;

                        }

                        foreach (var item in retorno)
                        {
                            var turnos = item.Horas.GroupBy(p => p.Turno);
                            item.HorasTurno = turnos.Select(p =>
                            {
                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key,
                                    Aptos = p.ToList().Sum(p => p.Aptos),
                                    Turno = p.Key,
                                    Equipe_Tmp = p.ToList().FirstOrDefault()?.Equipe_Tmp
                                };

                            }).Where(p => p != null && p.Display != null).ToList();
                        }
                    }
                    return retorno;
                }
            }
        }

        public AgendamentoDiaModel GetConfigDoDia(DayOfWeek diaSemana, DateTime dia, bool feriado, Horario_Connect horario_Connect)
        {
            AgendamentoDiaModel configDoDia = null;
            if (horario_Connect == null)
            {
                throw new Exception("Não existe a configuração de horário.");

            }
            else
            {


                if (feriado)
                {
                    GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, true, horario_Connect.Inicio_Feriado, horario_Connect.Termino_Feriado,
                            horario_Connect.Inicio_Intervalo_Feriado, horario_Connect.Termino_Intervalo_Feriado);
                }
                else
                {

                    switch (diaSemana)
                    {
                        case DayOfWeek.Sunday:
                            if (horario_Connect.Inicio_Domingo == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Domingo, horario_Connect.Termino_Domingo,
                                    horario_Connect.Inicio_Intervalo_Domingo, horario_Connect.Termino_Intervalo_Domingo);
                            break;

                        case DayOfWeek.Monday:
                            if (horario_Connect.Inicio_Segunda == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Segunda, horario_Connect.Termino_Segunda,
                                    horario_Connect.Inicio_Intervalo_Segunda, horario_Connect.Termino_Intervalo_Segunda);
                            break;

                        case DayOfWeek.Tuesday:
                            if (horario_Connect.Inicio_Terca == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Terca, horario_Connect.Termino_Terca,
                                    horario_Connect.Inicio_Intervalo_Terca, horario_Connect.Termino_Intervalo_Terca);
                            break;

                        case DayOfWeek.Wednesday:
                            if (horario_Connect.Inicio_Quarta == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Quarta, horario_Connect.Termino_Quarta,
                                    horario_Connect.Inicio_Intervalo_Quarta, horario_Connect.Termino_Intervalo_Quarta);
                            break;

                        case DayOfWeek.Thursday:
                            if (horario_Connect.Inicio_Quinta == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Quinta, horario_Connect.Termino_Quinta,
                                    horario_Connect.Inicio_Intervalo_Quinta, horario_Connect.Termino_Intervalo_Quinta);
                            break;

                        case DayOfWeek.Friday:
                            if (horario_Connect.Inicio_Sexta == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Sexta, horario_Connect.Termino_Sexta,
                                    horario_Connect.Inicio_Intervalo_Sexta, horario_Connect.Termino_Intervalo_Sexta);
                            break;

                        case DayOfWeek.Saturday:
                            if (horario_Connect.Inicio_Sabado == null)
                                return null;

                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false,
                                horario_Connect.Inicio_Sabado, horario_Connect.Termino_Sabado,
                                    horario_Connect.Inicio_Intervalo_Sabado, horario_Connect.Termino_Intervalo_Sabado);
                            break;
                    }

                }
            }
            return configDoDia;
        }

        public void GetDatasValidas_Agendamento(ref AgendamentoDiaModel configDoDia, DayOfWeek diaSemana, DateTime dia, Boolean feriado, DateTime? inicio, DateTime? termino, DateTime? inicio_intervalo, DateTime? termino_intervalo)
        {
            if (inicio.HasValue)
            {
                configDoDia = new AgendamentoDiaModel
                {
                    DiaDaSemana = diaSemana,
                    Data = dia.ToString("dd/MM/yyyy"),
                    Feriado = feriado,
                    HoraInicio = dia.AddMinutes(inicio.Value.TimeOfDay.TotalMinutes),
                    HoraTermino = dia.AddMinutes(termino.Value.TimeOfDay.TotalMinutes),
                    HoraInicioIntervalo = inicio_intervalo.HasValue ? dia.AddMinutes(inicio_intervalo.Value.TimeOfDay.TotalMinutes) : (DateTime?)null,
                    HoraTerminoIntervalo = termino_intervalo.HasValue ? dia.AddMinutes(termino_intervalo.Value.TimeOfDay.TotalMinutes) : (DateTime?)null
                };
            }
        }

        private void GetHorasDisponiveis(
            AgendamentoDiaModel configDoDia,
            double tempoExecucaoMinutos,
            double tempoDeslocamento,
            List<AgendamentoDadosModel> jaAgendadosDia,
            List<AgendamentoDadosModel> jaAgendadosTotal,
            List<AgendamentoEquipeHorarioModel> forcaTrabalho,
            List<AgendamentoEquipeHorarioModel> forcaTrabalhoTotal,
            DateTime dia,
            List<Horario_Connect> horarios_servico,
            DadosServicoAgendamento servico,
            LimiteAgendamentoAniel limiteAgendamento,
            Turno_ServicoAniel turno,
            bool capacidade
        )
        {
            var inicio = new DateTime(configDoDia.HoraInicio.Value.Ticks);

            var tecnicos = forcaTrabalho.Select(p => p.Equipe).ToList();


            if (configDoDia.HoraInicioIntervalo.HasValue)
            {
                for (DateTime hora = inicio; hora < configDoDia.HoraInicioIntervalo;) //i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    hora = GetHorasDisponiveis_Calc(
                        configDoDia,
                        tempoExecucaoMinutos,
                        tempoDeslocamento,
                        dia,
                        hora,
                        horarios_servico,
                        jaAgendadosDia,
                        jaAgendadosTotal,
                        forcaTrabalho,
                        forcaTrabalhoTotal,
                        servico,
                        limiteAgendamento,
                        turno,
                        capacidade
                    );
                }

                if (inicio < configDoDia.HoraTerminoIntervalo)
                {
                    inicio = configDoDia.HoraTerminoIntervalo.Value;
                }

                for (DateTime hora = inicio; hora < configDoDia.HoraTermino;) //i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    hora = GetHorasDisponiveis_Calc(
                        configDoDia,
                        tempoExecucaoMinutos,
                        tempoDeslocamento,
                        dia,
                        hora,
                        horarios_servico,
                        jaAgendadosDia,
                        jaAgendadosTotal,
                        forcaTrabalho,
                        forcaTrabalhoTotal,
                        servico,
                        limiteAgendamento,
                        turno,
                        capacidade
                    );
                }
            }
            else
            {
                for (DateTime hora = inicio; hora < configDoDia.HoraTermino;) //i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    hora = GetHorasDisponiveis_Calc(
                        configDoDia,
                        tempoExecucaoMinutos,
                        tempoDeslocamento,
                        dia,
                        hora,
                        horarios_servico,
                        jaAgendadosDia,
                        jaAgendadosTotal,
                        forcaTrabalho,
                        forcaTrabalhoTotal,
                        servico,
                        limiteAgendamento,
                        turno,
                        capacidade
                    );
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sessionT5"></param>
        /// <param name="configDoDia"></param>
        /// <param name="tempoExecucaoMinutos"></param>
        /// <param name="tempoDeslocamento"></param>
        /// <param name="documento"></param>
        /// <param name="dia"></param>
        /// <param name="hora"></param>
        /// <param name="horarios_servico"></param>
        /// <param name="criticaIntervalo"></param>
        /// <returns>Retorna a Hora para atualizar no for</returns>
        private DateTime GetHorasDisponiveis_Calc(
            AgendamentoDiaModel configDoDia,
            Double tempoExecucaoMinutos,
            Double tempoDeslocamento,
            DateTime dia,
            DateTime hora,
            List<Horario_Connect> horarios_servico,
            List<AgendamentoDadosModel> jaAgendadosDia,
            List<AgendamentoDadosModel> jaAgendadosTotal,
            List<AgendamentoEquipeHorarioModel> forcaTrabalho,
            List<AgendamentoEquipeHorarioModel> forcaTrabalhoTotal,
            DadosServicoAgendamento servico,
            LimiteAgendamentoAniel limiteAgendamento,
            Turno_ServicoAniel turno,
            bool capacidade
        )
        {
            AgendamentoHorasDisponiveis obj = new AgendamentoHorasDisponiveis
            {
                Display = hora.ToString("HH:mm"),
                Hora_Inicio = hora,
                Hora_Final = hora.AddMinutes(tempoExecucaoMinutos + (configDoDia.HoraInicio != hora ? tempoDeslocamento : 0)),
                Deslocamento = (configDoDia.HoraInicio != hora ? tempoDeslocamento : 0),
                IdPerfil = forcaTrabalho.FirstOrDefault()?.IdPerfilAgendamento
            };

            if (obj.Deslocamento != 0)
                obj.Display = obj.Hora_Inicio.AddMinutes(obj.Deslocamento).ToString("HH:mm");

            if (capacidade)
            {
                var tecnicos = forcaTrabalho.Select(p => p.Equipe).ToList();
                var jaAgendadosDiaForcaTrabalho = jaAgendadosDia.FindAll(p => tecnicos.Contains(p.Equipe)
                                                                              && p.Origem != OrigemAgendamentoOS.Manual
                                                                              && p.DataHoraFim.TimeOfDay < turno.Hora_Fim.Value.TimeOfDay
                                                                              && p.DataHoraInicio >= hora);


                jaAgendadosDiaForcaTrabalho = jaAgendadosDiaForcaTrabalho.FindAll(p => HorarioApiController.BetweenIgual(p.DataHoraInicio, obj.Hora_Inicio, obj.Hora_Final.AddSeconds(-1))
                                                             || HorarioApiController.BetweenIgual(p.DataHoraFim, obj.Hora_Inicio.AddSeconds(1), obj.Hora_Final)
                                                             || HorarioApiController.BetweenIgual(obj.Hora_Inicio, p.DataHoraInicio, p.DataHoraFim.AddSeconds(-1))
                                                             || HorarioApiController.BetweenIgual(obj.Hora_Final, p.DataHoraInicio.AddSeconds(1), p.DataHoraFim)).ToList().ToList();

                if (jaAgendadosDiaForcaTrabalho.Count > 0)
                {
                    var PrimeiroAgendamento = jaAgendadosDiaForcaTrabalho.Min(p => p.DataHoraFim);
                    return PrimeiroAgendamento;
                }
            }



            if (configDoDia.HoraInicioIntervalo.HasValue)
            {
                if ((obj.Hora_Inicio > configDoDia.HoraInicioIntervalo && obj.Hora_Inicio < configDoDia.HoraTerminoIntervalo) ||
                    (configDoDia.HoraInicioIntervalo > obj.Hora_Inicio && configDoDia.HoraInicioIntervalo < obj.Hora_Final))
                {
                    return obj.Hora_Final;
                }

                if ((obj.Hora_Final > configDoDia.HoraInicioIntervalo && obj.Hora_Final < configDoDia.HoraTerminoIntervalo) ||
                    (configDoDia.HoraTerminoIntervalo > obj.Hora_Inicio && configDoDia.HoraTerminoIntervalo < obj.Hora_Final))
                {
                    return obj.Hora_Final;
                }
            }

            if (obj.Hora_Final > configDoDia.HoraTermino)
            {
                return obj.Hora_Final;
            }

            if (turno != null)
            {
                if (obj.Hora_Inicio.TimeOfDay < turno.Hora_Ini.Value.TimeOfDay)
                {
                    return obj.Hora_Inicio.Date.Add(turno.Hora_Ini.Value.TimeOfDay);
                }

                if (BetweenIgual(obj.Hora_Inicio, turno.Hora_Ini, turno.Hora_Fim) == false || BetweenIgual(obj.Hora_Final, turno.Hora_Ini, turno.Hora_Fim) == false)
                {
                    return obj.Hora_Final;
                }

            }

            obj.Hora_InicioStr = obj.Hora_Inicio.ToString("HH:mm");
            obj.Hora_FinalStr = obj.Hora_Final.ToString("HH:mm");

            hora = obj.Hora_Final;

            obj = GetAllAcompanhamentoEquipe(
                configDoDia,
                dia,
                horarios_servico,
                obj,
                jaAgendadosDia,
                jaAgendadosTotal,
                forcaTrabalho,
                forcaTrabalhoTotal,
                servico,
                limiteAgendamento
            );

            //if (!String.IsNullOrEmpty(obj.Equipe_Tmp) && obj.Aptos > 0)
            //{
            if (capacidade)
            {
                configDoDia.HorasCapacidade.Add(obj);
            }
            else
            {
                configDoDia.Horas.Add(obj);
            }
            //}

            return hora;
        }


        private async Task<List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>> GetJaAgendados(ISession sessionT5, DateTime dia)
        {

            List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)> JaAgendados = new List<(string equipe, DateTime dataHoraInicio, DateTime dataHoraFim)>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();




                StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM    FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         WHERE A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         UNION
                                                         SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM    FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         WHERE A.DATAHORA_FIM BETWEEN @DATA_INICIO AND @DATA_FIM");

                //parametros.Add("@DATA_INICIO", agendamentoDisponiveis.Hora_Inicio.AddSeconds(1));
                //parametros.Add("@DATA_FIM", agendamentoDisponiveis.Hora_Final.AddSeconds(-1));
                parametros.Add("@DATA_INICIO", dia.Date);
                parametros.Add("@DATA_FIM", dia.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        JaAgendados.Add((Convert.ToString(reader["EQUIPE"]), Convert.ToDateTime(reader["DATAHORA_INICIO"]), Convert.ToDateTime(reader["DATAHORA_FIM"])));
                    }
                }
            }

            return JaAgendados;
        }

        public async Task<List<AgendamentoDadosModel>> GetJaAgendados(ISession sessionT5, DateTime dataInicio, DateTime dataFim)
        {

            List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();




                StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND (COALESCE(B.COD_ENCERRAMENTO,'') = '')
                                                         
                                                         UNION

                                                         SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_FIM BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND (COALESCE(B.COD_ENCERRAMENTO,'') = '')");

                //parametros.Add("@DATA_INICIO", agendamentoDisponiveis.Hora_Inicio.AddSeconds(1));
                //parametros.Add("@DATA_FIM", agendamentoDisponiveis.Hora_Final.AddSeconds(-1));
                parametros.Add("@DATA_INICIO", dataInicio.Date);
                parametros.Add("@DATA_FIM", dataFim.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var equipe = Convert.ToString(reader["EQUIPE"]);
                        var dataHoraInicio = Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                        var dataHoraFim = Convert.ToDateTime(reader["DATAHORA_FIM"]);
                        var tmp = Convert.ToBoolean(reader["TMP"]);
                        var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0;
                        var dataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                        var usuario = Convert.ToString(reader["USUARIOCRIACAO"]);
                        var usuarioAgendamento = Convert.ToString(reader["USUARIO_AGENDAMENTO"]);
                        var origem = reader["ORIGEM"] != DBNull.Value ? (OrigemAgendamentoOS)reader["ORIGEM"] : (OrigemAgendamentoOS?)null;

                        dataHoraInicio = Convert.ToDateTime(dataHoraInicio.ToString("dd/MM/yyyy HH:mm:00"));
                        dataHoraFim = Convert.ToDateTime(dataHoraFim.ToString("dd/MM/yyyy HH:mm:00"));

                        DadosServicoAgendamento dadosServico = new DadosServicoAgendamento
                        {
                            Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                            Cod_Area = Convert.ToString(reader["COD_AREA"]),
                            Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (int?)null,
                            Cod_Natureza = reader["COD_NATUREZA"] != DBNull.Value ? Convert.ToInt32(reader["COD_NATUREZA"]) : (int?)null,
                            LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]),
                            NumObra = Convert.ToString(reader["NUM_OBRA"]),
                            DescricaoArea = Convert.ToString(reader["DESCRICAO_AREA"]),
                            DescricaoEscritorio = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                            TipoServico = Convert.ToString(reader["TIPO_SERVICO"]),
                            DescricaoNatureza = Convert.ToString(reader["DESCRICAO_NATUREZA"]),
                            Periodo = Convert.ToString(reader["PERIODO"]),
                            IdPeriodo = reader["COD_TURNO_SERV"] != DBNull.Value ? Convert.ToInt64(reader["COD_TURNO_SERV"]) : (long?)null,
                            Status = reader["STATUS_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])) : null,
                        };

                        JaAgendados.Add(new AgendamentoDadosModel
                        {
                            DadosServico = dadosServico,
                            DataHoraFim = dataHoraFim,
                            DataHoraInicio = dataHoraInicio,
                            Equipe = equipe,
                            Temporario = tmp,
                            Deslocamento = deslocamento,
                            DataHoraCriacao = dataHoraCriacao,
                            UsuarioCriacao = usuario,
                            UsuarioAgendamento = usuarioAgendamento,
                            Origem = origem,
                            Capaz = false
                        });
                    }
                }
            }

            return JaAgendados;
        }
        public async Task<List<AgendamentoDadosModel>> GetJaAgendadosDetalhamento(ISession sessionT5, DateTime dataInicio, DateTime dataFim)
        {

            List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();




                StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.EQUIPE AS EQUIPE_DEFINITIVA,A.EQUIPE_TMP AS EQUIPE_TEMPORARIA, A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND ((COALESCE(B.COD_ENCERRAMENTO,'') = '') OR ( (B.DATA_PROD = @DATA_INICIO) AND ((B.DATA_PROD+B.HORA_PROD) < CURRENT_TIMESTAMP)) )
                                                         
                                                         UNION

                                                         SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.EQUIPE AS EQUIPE_DEFINITIVA,A.EQUIPE_TMP AS EQUIPE_TEMPORARIA,A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_FIM BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND ((COALESCE(B.COD_ENCERRAMENTO,'') = '') OR (  (B.DATA_PROD = @DATA_INICIO) AND  ((B.DATA_PROD+B.HORA_PROD) < CURRENT_TIMESTAMP)))");

                //parametros.Add("@DATA_INICIO", agendamentoDisponiveis.Hora_Inicio.AddSeconds(1));
                //parametros.Add("@DATA_FIM", agendamentoDisponiveis.Hora_Final.AddSeconds(-1));
                parametros.Add("@DATA_INICIO", dataInicio.Date);
                parametros.Add("@DATA_FIM", dataFim.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var equipe = Convert.ToString(reader["EQUIPE"]);
                        var equipeDefinitiva = Convert.ToString(reader["EQUIPE_DEFINITIVA"]);
                        var equipeTemporaria = Convert.ToString(reader["EQUIPE_TEMPORARIA"]);

                        var dataHoraInicio = Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                        var dataHoraFim = Convert.ToDateTime(reader["DATAHORA_FIM"]);
                        var tmp = Convert.ToBoolean(reader["TMP"]);
                        var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0;
                        var dataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                        var usuario = Convert.ToString(reader["USUARIOCRIACAO"]);
                        var usuarioAgendamento = Convert.ToString(reader["USUARIO_AGENDAMENTO"]);
                        var origem = reader["ORIGEM"] != DBNull.Value ? (OrigemAgendamentoOS)reader["ORIGEM"] : (OrigemAgendamentoOS?)null;

                        dataHoraInicio = Convert.ToDateTime(dataHoraInicio.ToString("dd/MM/yyyy HH:mm:00"));
                        dataHoraFim = Convert.ToDateTime(dataHoraFim.ToString("dd/MM/yyyy HH:mm:00"));

                        DadosServicoAgendamento dadosServico = new DadosServicoAgendamento
                        {
                            Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                            Cod_Area = Convert.ToString(reader["COD_AREA"]),
                            Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (int?)null,
                            Cod_Natureza = reader["COD_NATUREZA"] != DBNull.Value ? Convert.ToInt32(reader["COD_NATUREZA"]) : (int?)null,
                            LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]),
                            NumObra = Convert.ToString(reader["NUM_OBRA"]),
                            DescricaoArea = Convert.ToString(reader["DESCRICAO_AREA"]),
                            DescricaoEscritorio = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                            TipoServico = Convert.ToString(reader["TIPO_SERVICO"]),
                            DescricaoNatureza = Convert.ToString(reader["DESCRICAO_NATUREZA"]),
                            Periodo = Convert.ToString(reader["PERIODO"]),
                            IdPeriodo = reader["COD_TURNO_SERV"] != DBNull.Value ? Convert.ToInt64(reader["COD_TURNO_SERV"]) : (long?)null,
                            Status = reader["STATUS_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])) : null,
                        };

                        JaAgendados.Add(new AgendamentoDadosModel
                        {
                            DadosServico = dadosServico,
                            DataHoraFim = dataHoraFim,
                            DataHoraInicio = dataHoraInicio,
                            Equipe = equipe,
                            Temporario = tmp,
                            Deslocamento = deslocamento,
                            DataHoraCriacao = dataHoraCriacao,
                            UsuarioCriacao = usuario,
                            UsuarioAgendamento = usuarioAgendamento,
                            Origem = origem,
                            Capaz = false,
                            EquipeDefinitiva = equipeDefinitiva,
                            EquipeTemporaria = equipeTemporaria
                        });
                    }
                }
            }

            return JaAgendados;
        }

        public async Task<List<AgendamentoEquipeHorarioModel>> GetForcaTrabalho(ISession sessionT5,
                                                                                DateTime dia,
                                                                                Configuracoes_Gerais config,
                                                                                List<PerfilAgendamentoNatureza> perfisNaturezas,
                                                                                Dictionary<long, List<LocalidadeRoteirizacao>> perfisLocalidades,
                                                                                String Equipe = null)
        {

            var _temCriterioArea = config.Criterio_Area;
            var _temCriterioHabilidades = config.Criterio_Habilidade;
            var _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;
            var _Utiliza_AnielPoint = config.UtilizaPoint_RoteirizacaoAgendamento;
            List<AgendamentoEquipeHorarioModel> equipeshorarios = new List<AgendamentoEquipeHorarioModel>();


            using (var sessionT5NH = HibernateUtil.GetSessionT5())
            using (var horarioapi = new HorarioApiController())
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryTipoServicoEquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
            using (var com = sessionT5.Connection.CreateCommand())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var sql = new StringBuilder();

                if (config.UtilizaPoint_RoteirizacaoAgendamento)
                {
                    sql = new StringBuilder(@"SELECT  
                                            B.EQUIPE,
                                            B.NOME,
                                            B.DEVICE_ID,
                                            H.LATITUDE,
                                            H.LONFITUDE,
                                            SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR,
                                            SUPERV.NOME AS NOME_SUPERVISOR,
                                            B.USUARIO_MONITOR,
                                            FUNCIONARIO_MONITOR.NOME AS NOME_MONITOR,
                                            B.ID_PERFIL_AGENDAMENTO,
                                            F.HORARIO_ATRIBUIDO,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 F.ID_PERFIL_AGENDAMENTO
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_PERFIL_AGENDAMENTO_HORARIO,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 F.ID
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_HORARIO_EQUIPE,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 G.ID
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_HORARIO_CONNECT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 COALESCE(B.ID_HORARIO_CONNECT, D.ID_HORARIO_CONNECT) 
                                                    FROM TB_PT_DIA_FUNCIONARIO A
                                                    LEFT JOIN TB_PT_JORNADA_FUNCIONARIO B ON A.ID_JORNADA = B.ID
                                                    LEFT JOIN TB_FUNCIONARIO C ON C.NUM_FRE = A.NUM_FRE
                                                    LEFT JOIN TB_PT_JORNADA_FUNCIONARIO D ON C.ID_JORNADA = D.ID
                                                    WHERE A.NUM_FRE = F.NUM_FRE 
                                                      AND A.DATA = @DATA
                                                )
                                            END) AS ID_HORARIO_CONNECT_POINT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.INDISPONIVEL 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 H.INDISPONIVEL
                                                    FROM TB_HORARIO_EQUIPE_CONNECT H
                                                    LEFT JOIN TB_HORARIO_CONNECT I ON H.ID_HORARIO = I.ID
                                                    WHERE H.EQUIPE = B.EQUIPE 
                                                      AND H.APENAS_NO_DIA = 1
                                                      AND H.DATA = @DATA
                                                      AND H.INDISPONIVEL = 1
                                                )
                                            END) AS INDISPONIVEL_CONNECT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 A.NUM_FRE
                                                    FROM TB_FUNCIONARIO A
                                                    INNER JOIN TB_PT_DIA_FUNCIONARIO B ON A.NUM_FRE = B.NUM_FRE
                                                    LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA C ON B.ID_EVENTO = C.ID
                                                    WHERE C.PERMTITE_ROTEIRIZAR = 0
                                                      AND A.NUM_FRE = F.NUM_FRE 
                                                      AND B.DATA = @DATA
                                                )
                                            END) AS INDISPONIVEL_POINT

                                        FROM TB_EQUIPE B
                                        INNER JOIN TB_FUNCIONARIO F ON B.NUM_FRE = F.NUM_FRE
                                        LEFT JOIN TB_PONTO_PARTIDA H ON B.ID_PONTO_PARTIDA = H.ID
                                        LEFT JOIN TB_EQUIPE_STATUS J ON B.EQUIPE = J.EQUIPE
                                        LEFT JOIN TB_FUNCIONARIO SUPERV ON SUPERV.NUM_FRE = B.COD_SUPERV
                                        LEFT JOIN TB_SENHA USUARIO_MONITOR ON USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR
                                        LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR ON FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE

                                        WHERE 
                                            (F.SITUACAO = 0 OR F.SITUACAO = 4)
                                            AND B.EQUIPE_CONNECT = 'S'
                                        ");
                }
                else
                {

                    sql = new StringBuilder(@"SELECT
                                                B.EQUIPE,
                                                B.NOME,
                                                B.DEVICE_ID,
                                                H.LATITUDE,
                                                H.LONFITUDE,
                                                SUPERV.NUM_FRE           AS NUM_FRE_SUPERVISOR,
                                                SUPERV.NOME              AS NOME_SUPERVISOR,
                                                B.USUARIO_MONITOR,
                                                FUNCIONARIO_MONITOR.NOME AS NOME_MONITOR,
                                                B.ID_PERFIL_AGENDAMENTO,
                                                F.HORARIO_ATRIBUIDO,

                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID_PERFIL_AGENDAMENTO END AS ID_PERFIL_AGENDAMENTO_HORARIO,
                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID                    END AS ID_HORARIO_EQUIPE,
                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID_HORARIO    END AS ID_HORARIO_CONNECT,

                                                CASE
                                                    WHEN HR.EQUIPE IS NULL            THEN NULL
                                                    WHEN HR.APENAS_NO_DIA = 1
                                                     AND HR.DATA          = @DATA
                                                     AND HR.INDISPONIVEL  = 1         THEN HR.INDISPONIVEL
                                                    ELSE NULL
                                                END AS INDISPONIVEL_CONNECT
                                            FROM TB_EQUIPE B
                                            INNER JOIN TB_FUNCIONARIO F
                                                ON B.NUM_FRE = F.NUM_FRE
                                               AND (F.SITUACAO = 0 OR F.SITUACAO = 4)
                                            LEFT JOIN TB_HORARIO_EQUIPE_CONNECT HR
                                                ON HR.EQUIPE = B.EQUIPE and hr.data = @DATA
                                            LEFT JOIN TB_PONTO_PARTIDA H
                                                ON B.ID_PONTO_PARTIDA = H.ID
                                            LEFT JOIN TB_EQUIPE_STATUS J
                                                ON B.EQUIPE = J.EQUIPE
                                            LEFT JOIN TB_FUNCIONARIO SUPERV
                                                ON SUPERV.NUM_FRE = B.COD_SUPERV
                                            LEFT JOIN TB_SENHA USUARIO_MONITOR
                                                ON USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR
                                            LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR
                                                ON FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE
                                            WHERE B.EQUIPE_CONNECT = 'S'");
                }


                if (dia.Date == DateTime.Today)
                {
                    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");
                }
                parametros.Add("@DATA", dia.Date);

                if (!String.IsNullOrEmpty(Equipe))
                {
                    sql.AppendLine("AND (B.EQUIPE) = @EQUIPE");
                    parametros.Add("@EQUIPE", Equipe);
                }

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var disponivel = true;
                        long? idHorarioEquipe = null;
                        long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                        HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ?
                                                                                                            (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"]))
                                                                                                        :
                                                                                                            (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);

                        if (reader["ID_PERFIL_AGENDAMENTO_HORARIO"] != DBNull.Value)
                        {
                            idPerfil = Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO_HORARIO"]);
                        }

                        idHorarioEquipe = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null;
                        var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;
                        if (indisponivelConnect)
                            disponivel = false;

                        if (config.UtilizaPoint_RoteirizacaoAgendamento && horarioAtribuido == HorarioAtribuido.AnielPoint)
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null;
                            var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                            if (String.IsNullOrEmpty(indisponivelPoint) == false || idHorarioEquipe.HasValue == false)
                                disponivel = false;

                        }



                        var cache = perfilAgendamentoCache.Find(p => p.perfil == idPerfil);
                        List<LocalidadeRoteirizacao> localidades = new List<LocalidadeRoteirizacao>();
                        List<Natureza_ProducaoAniel> naturezas = new List<Natureza_ProducaoAniel>();
                        if (cache.perfil == null)
                        {
                            if (idPerfil != null)
                            {
                                perfisLocalidades.TryGetValue(idPerfil.Value, out localidades);
                                if (localidades == null)
                                    localidades = new List<LocalidadeRoteirizacao>();
                                naturezas = perfisNaturezas.FindAll(p => p.PerfilAgendamento.Id == idPerfil).Select(p => p?.Natureza).ToList();
                                perfilAgendamentoCache.Add((perfil: idPerfil, localidades: localidades, naturezas: naturezas));
                            }
                        }
                        else
                        {
                            localidades = cache.localidades;
                            naturezas = cache.naturezas;
                        }

                        if (equipeshorarios.Find(p => Convert.ToString(reader["EQUIPE"]) == p.Equipe) != null)
                        {
                            continue;
                        }

                        var forcaobj = new AgendamentoEquipeHorarioModel
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Id_Horario_Connect = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null,
                            Naturezas = naturezas.Select(p => (long?)p.Id).ToList(),
                            Localidades = localidades,
                            Disponivel = disponivel,
                            CodSuperv = Convert.ToString(reader["NUM_FRE_SUPERVISOR"]),
                            IdPerfilAgendamento = idPerfil
                        };

                        if (config.UtilizaPoint_RoteirizacaoAgendamento)
                        {
                            forcaobj.IdHorarioConnectPoint = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null;
                        }

                        equipeshorarios.Add(forcaobj);

                    }
                }
            }

            return equipeshorarios;
        }

        public async Task<List<AgendamentoEquipeHorarioModel>> GetForcaTrabalhoPeriodo(ISession sessionT5,
                                                                                DateTime dia,
                                                                                Configuracoes_Gerais config,
                                                                                List<PerfilAgendamentoNatureza> perfisNaturezas,
                                                                                Dictionary<long, List<LocalidadeRoteirizacao>> perfisLocalidades)
        {

            var _temCriterioArea = config.Criterio_Area;
            var _temCriterioHabilidades = config.Criterio_Habilidade;
            var _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;
            var _Utiliza_AnielPoint = config.UtilizaPoint_RoteirizacaoAgendamento;
            List<AgendamentoEquipeHorarioModel> equipeshorarios = new List<AgendamentoEquipeHorarioModel>();


            using (var sessionT5NH = HibernateUtil.GetSessionT5())
            using (var horarioapi = new HorarioApiController())
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryTipoServicoEquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
            using (var com = sessionT5.Connection.CreateCommand())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var sql = new StringBuilder(@"SELECT * FROM SP_EQUIPES_CONNECT_POR_PERIODO('2025-03-01', '2025-03-07')");


                //if (dia.Date == DateTime.Today)
                //    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");


                parametros.Add("@DATA", dia.Date);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var disponivel = true;
                        long? idHorarioEquipe = null;
                        long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                        HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ?
                                                                                                            (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"]))
                                                                                                        :
                                                                                                            (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);

                        if (reader["ID_PERFIL_AGENDAMENTO_HORARIO"] != DBNull.Value)
                        {
                            idPerfil = Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO_HORARIO"]);
                        }

                        if (horarioAtribuido == HorarioAtribuido.AnielPoint)
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null;
                            var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                            if (String.IsNullOrEmpty(indisponivelPoint) == false || idHorarioEquipe.HasValue == false)
                                disponivel = false;

                        }
                        else
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null;
                            var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;
                            if (indisponivelConnect)
                                disponivel = false;
                        }


                        var cache = perfilAgendamentoCache.Find(p => p.perfil == idPerfil);
                        List<LocalidadeRoteirizacao> localidades = new List<LocalidadeRoteirizacao>();
                        List<Natureza_ProducaoAniel> naturezas = new List<Natureza_ProducaoAniel>();
                        if (cache.perfil == null)
                        {
                            if (idPerfil != null)
                            {
                                perfisLocalidades.TryGetValue(idPerfil.Value, out localidades);
                                if (localidades == null)
                                    localidades = new List<LocalidadeRoteirizacao>();
                                naturezas = perfisNaturezas.FindAll(p => p.PerfilAgendamento.Id == idPerfil).Select(p => p?.Natureza).ToList();
                                perfilAgendamentoCache.Add((perfil: idPerfil, localidades: localidades, naturezas: naturezas));
                            }
                        }
                        else
                        {
                            localidades = cache.localidades;
                            naturezas = cache.naturezas;
                        }

                        equipeshorarios.Add(new AgendamentoEquipeHorarioModel
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Id_Horario_Connect = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null,
                            Naturezas = naturezas.Select(p => (long?)p.Id).ToList(),
                            IdHorarioConnectPoint = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null,
                            Localidades = localidades,
                            Disponivel = disponivel,
                            CodSuperv = Convert.ToString(reader["NUM_FRE_SUPERVISOR"]),
                            IdPerfilAgendamento = idPerfil
                        });

                    }
                }
            }

            return equipeshorarios;
        }

        private AgendamentoHorasDisponiveis GetAllAcompanhamentoEquipe(
            AgendamentoDiaModel configDoDia,
            DateTime dia,
            List<Horario_Connect> horarios_servico,
            AgendamentoHorasDisponiveis agendamentoDisponiveis,
            List<AgendamentoDadosModel> jaAgendadosDia,
            List<AgendamentoDadosModel> jaAgendadosTotal,
            List<AgendamentoEquipeHorarioModel> forcaTrabalho,
            List<AgendamentoEquipeHorarioModel> forcaTrabalhoTotal,
            DadosServicoAgendamento servico,
            LimiteAgendamentoAniel limiteAgendamento
        )
        {

            using (var horarioapi = new HorarioApiController())
            {

                List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
                JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => HorarioApiController.BetweenIgual(p.DataHoraInicio, agendamentoDisponiveis.Hora_Inicio, agendamentoDisponiveis.Hora_Final.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(p.DataHoraFim, agendamentoDisponiveis.Hora_Inicio.AddSeconds(1), agendamentoDisponiveis.Hora_Final)
                                                                         || HorarioApiController.BetweenIgual(agendamentoDisponiveis.Hora_Inicio, p.DataHoraInicio, p.DataHoraFim.AddSeconds(-1))
                                                                         || HorarioApiController.BetweenIgual(agendamentoDisponiveis.Hora_Final, p.DataHoraInicio.AddSeconds(1), p.DataHoraFim)).ToList()).ToList();


                List<Int64> ids_Horarios_Connect = (from o in horarios_servico
                                                    where ((configDoDia.DiaDaSemana == DayOfWeek.Sunday && (o.Inicio_Domingo.HasValue)
                                                                                                           && (o.Inicio_Domingo.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Domingo.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Monday && (o.Inicio_Segunda.HasValue)
                                                                                                           && (o.Inicio_Segunda.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Segunda.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Tuesday && (o.Inicio_Terca.HasValue)
                                                                                                           && (o.Inicio_Terca.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Terca.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Wednesday && (o.Inicio_Quarta.HasValue)
                                                                                                           && (o.Inicio_Quarta.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Quarta.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Thursday && (o.Inicio_Quinta.HasValue)
                                                                                                           && (o.Inicio_Quinta.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Quinta.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Friday && (o.Inicio_Sexta.HasValue)
                                                                                                           && (o.Inicio_Sexta.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Sexta.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay))

                                                        || (configDoDia.DiaDaSemana == DayOfWeek.Saturday && (o.Inicio_Sabado.HasValue)
                                                                                                           && (o.Inicio_Sabado.Value.TimeOfDay <= agendamentoDisponiveis.Hora_Inicio.TimeOfDay && o.Termino_Sabado.Value.TimeOfDay >= agendamentoDisponiveis.Hora_Final.TimeOfDay)))
                                                    select o.Id).ToList();

                var horario_servico_padrao = horarios_servico.Find(p => p.Padrao);

                var forcaTrabalhoApta = forcaTrabalho.Select(p =>
                {
                    p.Id_Horario_Connect = p.Id_Horario_Connect ?? horario_servico_padrao?.Id;
                    var horario_servico = horarios_servico.Where(y => y.Id == p.Id_Horario_Connect).FirstOrDefault() ?? horario_servico_padrao;
                    var horario_dia = horarioapi.RetornaHorarioDia(horario_servico, dia, configDoDia.Feriado);

                    var apto = horarioapi.AptoHorario(horario_dia.inicio, horario_dia.inicio_intervalo, horario_dia.termino_intervalo, horario_dia.termino,
                                        agendamentoDisponiveis.Hora_Inicio, agendamentoDisponiveis.Hora_Final);

                    if (apto == false)
                        return null;
                    else
                        return p;
                }).Where(p => p != null);

                #region Limite de agendamento para a natureza na área

                agendamentoDisponiveis.LimiteAgendamentoUltrapassado = false;

                List<AgendamentoEquipeHorarioModel> forcaTrabalhoAptaLimiteAgendamento = new List<AgendamentoEquipeHorarioModel>(forcaTrabalhoApta);

                if (limiteAgendamento != null)
                {
                    var forcaTrabalhoArea =
                        forcaTrabalhoTotal
                            .Where(f => f.Localidades.Any(l => l.Cod_Escritorio == servico.Cod_Escritorio &&
                                                               l.Cod_Area == servico.Cod_Area) &&
                                        f.Naturezas.Contains(servico.Cod_Natureza) &&
                                        f.Naturezas.Count > 1 && f.Disponivel == true);

                    List<string> MatriculasTrabalhoArea = forcaTrabalhoArea.Select(p => p.Equipe).ToList();

                    // Total de tempo já agendado para serviços da mesma natureza na área
                    double duracaoJaAgendadosTotalMinutos =
                        jaAgendadosTotal
                            .Where(p => p.DadosServico.Cod_Escritorio == servico.Cod_Escritorio &&
                                        p.DadosServico.Cod_Area == servico.Cod_Area &&
                                        p.DadosServico.Cod_Natureza == servico.Cod_Natureza &&
                                        MatriculasTrabalhoArea.Contains(p.Equipe)
                                        )
                            .Sum(a => a.DuracaoMinutos);


                    // Adicionando o tempo do serviço que queremos agendar ao total dos já agendados
                    duracaoJaAgendadosTotalMinutos += (agendamentoDisponiveis.Hora_Final - agendamentoDisponiveis.Hora_Inicio).TotalMinutes;

                    // Capacidade total dos técnicos multiskill que atendem a área e a natureza do serviço
                    double capacidadeTotalMultiskillDiaMinutos = 0;

                    // Adicionando tempo de outras janelas ja montadas
                    //var janelasMinutos = configDoDia.HorasCapacidade.FindAll(p => MatriculasTrabalhoArea.Contains(p.Equipe_Tmp)).
                    //                                                 Select(p => (p.Hora_Final - p.Hora_Inicio).TotalMinutes).
                    //                                                 Sum();
                    //duracaoJaAgendadosTotalMinutos += janelasMinutos;

                    foreach (AgendamentoEquipeHorarioModel forca in forcaTrabalhoArea)
                    {
                        forca.Id_Horario_Connect = forca.Id_Horario_Connect ?? horario_servico_padrao?.Id;

                        Horario_Connect horarioServico =
                            horarios_servico.Where(y => y.Id == forca.Id_Horario_Connect).FirstOrDefault() ?? horario_servico_padrao;

                        var horarioDia = horarioapi.RetornaHorarioDia(horarioServico, dia, configDoDia.Feriado);

                        capacidadeTotalMultiskillDiaMinutos += horarioapi.MinutosTrabalhadosHorario(
                            horarioDia.inicio,
                            horarioDia.inicio_intervalo,
                            horarioDia.termino_intervalo,
                            horarioDia.termino
                        );
                    }

                    double percLimiteAgendamento = ((double)limiteAgendamento.Percentual) / 100;

                    // TESTE DO LIMITE
                    if (capacidadeTotalMultiskillDiaMinutos > 0 && (duracaoJaAgendadosTotalMinutos / capacidadeTotalMultiskillDiaMinutos) >= percLimiteAgendamento)
                    {
                        // Caso os minutos dos serviços da natureza agendados dentro da área ultrapasse o percentual configurado
                        // em relação ao total da capacidade dos técnicos multiskill daquela área marcamos esse campo como true para informar
                        // que o limite de agendamento do dia foi ultrapassado.

                        agendamentoDisponiveis.LimiteAgendamentoUltrapassado = true;
                        forcaTrabalhoAptaLimiteAgendamento =
                            forcaTrabalhoAptaLimiteAgendamento.Where(f => !(f.Localidades.Any(l => l.Cod_Escritorio == servico.Cod_Escritorio &&
                                                                                  l.Cod_Area == servico.Cod_Area) &&
                                                           f.Naturezas.Contains(servico.Cod_Natureza) &&
                                                           f.Naturezas.Count > 1)).ToList();



                    }
                }

                #endregion

                var _forca_trabalho = from o in forcaTrabalhoApta
                                      where ids_Horarios_Connect.Contains(o.Id_Horario_Connect.Value) &&
                                            !JaAgendados.Select(p => p.Equipe).Contains(o.Equipe)
                                      select o.Equipe;

                var _forca_trabalhoComLimite = from o in forcaTrabalhoAptaLimiteAgendamento
                                               where ids_Horarios_Connect.Contains(o.Id_Horario_Connect.Value) &&
                                                     !JaAgendados.Select(p => p.Equipe).Contains(o.Equipe)
                                               select o.Equipe;

                agendamentoDisponiveis.Equipe_Tmp = _forca_trabalho.FirstOrDefault();
                agendamentoDisponiveis.Lista_Forca_Trabalho = _forca_trabalho.ToList();
                agendamentoDisponiveis.Ja_Agendados = JaAgendados.Count;
                agendamentoDisponiveis.Forca_Trabalho = forcaTrabalhoApta.Count();
                agendamentoDisponiveis.ForcaTrabalhoMinutosPrevistos = (agendamentoDisponiveis.Hora_Final - agendamentoDisponiveis.Hora_Inicio).TotalMinutes * forcaTrabalhoApta.Count();
                agendamentoDisponiveis.Lista_Ja_Agendados = JaAgendados;
                agendamentoDisponiveis.Aptos = forcaTrabalho.Count() - JaAgendados.Count();//agendamentoDisponiveis.Lista_Forca_Trabalho.Count();
                agendamentoDisponiveis.ForcaTrabalhoMinutosPrevistosComLimite = (agendamentoDisponiveis.Hora_Final - agendamentoDisponiveis.Hora_Inicio).TotalMinutes * forcaTrabalhoAptaLimiteAgendamento.Count();
                agendamentoDisponiveis.ForcaTrabalhoLimiteAgendamento = forcaTrabalhoAptaLimiteAgendamento.Count();

                agendamentoDisponiveis.EquipeTmpComLimite = _forca_trabalhoComLimite.FirstOrDefault();
                agendamentoDisponiveis.ListaForcaTrabalhoComLimite = _forca_trabalhoComLimite.ToList();

            }

            return agendamentoDisponiveis;
        }

        public async Task<AgendamentoHorasDisponiveis> GetAgendamentoHorasDisponiveis(
            Configuracoes_Gerais config,
            List<Horario_Connect> horarios_servico,
            DateTime inicio,
            TimeSpan tempoExecucao,
            DadosServicoAgendamento servico
        )
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<Horario_Connect>(sessionT5NH))
            using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5NH))
            using (var repoTipo = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
            using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
            {
                var horarioAgendamento = await repo.selectFirst(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Padrao == true && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento);
                if (horarioAgendamento == null) throw new Exception("Horário de Agendamento Padrão não cadastrado. Por favor, defina um Horário de Agendamento como padrão antes de prosseguir.");
                LocalidadeAniel localidade = await repoLocalidade.selectFirst(p => p.Cod_Escritorio == servico.Cod_Escritorio && p.Cod_Area == servico.Cod_Area && p.Cod_Localidade == servico.Cod_Localidade);
                var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                var perfisLocalidades = GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                List<AgendamentoEquipeHorarioModel> forcaTrabalho = await GetForcaTrabalho(sessionT5, inicio.Date, config, perfisNaturezas, perfisLocalidades);
                List<AgendamentoDadosModel> jaAgendados = await GetJaAgendados(sessionT5, inicio, inicio);
                RoteirizacaoBusiness helper = new RoteirizacaoBusiness();



                List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == inicio.Date).Select(p =>
                {
                    AgendamentoEquipeHorarioModel forcaTrabalhoEquipe = forcaTrabalho.Find(y => y.Equipe == p.Equipe);
                    bool capaz = false;
                    if (forcaTrabalhoEquipe != null)
                    {

                        Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                        {
                            Cod_Escritorio = p.DadosServico.Cod_Escritorio,
                            Cod_Area = p.DadosServico.Cod_Area,
                            Cod_Localidade = p.DadosServico.Cod_Localidade,
                            Cod_Natureza = p.DadosServico.Cod_Natureza,
                            LocalidadeStr = p.DadosServico.LocalidadeStr
                        };


                        TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                        {
                            Naturezas = forcaTrabalhoEquipe.Naturezas,
                            Localidades = forcaTrabalhoEquipe.Localidades,
                            Equipe = forcaTrabalhoEquipe.Equipe,

                        };

                        capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                        p.Capaz = capaz;
                    }

                    return p;
                }).ToList();

                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                {
                    Cod_Escritorio = servico.Cod_Escritorio,
                    Cod_Area = servico.Cod_Area,
                    Cod_Localidade = servico.Cod_Localidade,
                    Cod_Natureza = servico.Cod_Natureza,
                    LocalidadeStr = localidade?.LocalidadeStr
                };

                var aptos = forcaTrabalho.Select(p =>
                {
                    TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                    {
                        Naturezas = p.Naturezas,
                        Localidades = p.Localidades,
                        Equipe = p.Equipe,
                    };

                    var capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                    return (p.Equipe, capaz);

                }).ToDictionary(p => p.Equipe, p => p.capaz);
                forcaTrabalho = forcaTrabalho.FindAll(p => aptos[p.Equipe] && p.Disponivel);


                AgendamentoHorasDisponiveis obj = new AgendamentoHorasDisponiveis
                {
                    Display = inicio.ToString("HH:mm"),
                    Hora_Inicio = inicio,
                    Hora_Final = inicio.AddMinutes(tempoExecucao.TotalMinutes),
                    Deslocamento = 0
                };

                obj.Hora_InicioStr = obj.Hora_Inicio.ToString("HH:mm");
                obj.Hora_FinalStr = obj.Hora_Final.ToString("HH:mm");

                DayOfWeek diaSemana = inicio.DayOfWeek;
                AgendamentoDiaModel configDoDia = GetConfigDoDia(diaSemana, inicio.Date, false, horarioAgendamento);

                ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5NH);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5NH);

                var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);

                LimiteAgendamentoAniel limAgendData = null;

                if (servico.Cod_Natureza.HasValue)
                    limAgendData =
                        limiteAgendamentoBusiness
                            .GetPercNaturezaByAreaData(
                                servico.Cod_Escritorio,
                                servico.Cod_Area,
                                Convert.ToInt32(servico.Cod_Natureza.Value),
                                inicio.Date
                            );

                if (configDoDia != null)
                    obj = GetAllAcompanhamentoEquipe(
                        configDoDia,
                        inicio.Date,
                        horarios_servico,
                        obj,
                        jaAgendadosDia,
                        jaAgendados,
                        forcaTrabalho,
                        forcaTrabalho,
                        servico,
                        limAgendData
                    );

                return obj;
            }

        }

        #endregion

        #region Index Post

        [HttpPost()]
        [Route("api/Agendamento_Equipeapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(ParamWebApi<AgendamentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (var repositoryLogLanServico = new Conexao<Log_Lanc_Servico>(session))
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    using (var api_Roteirizacao = new Painel_RoteirizacaoApiController())
                    using (var api = new Documento_ProducaoApiController())
                    {
                        Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                        using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                        {
                            if (String.IsNullOrEmpty(value.Objeto.CodCt) || String.IsNullOrEmpty(value.Objeto.Projeto) || String.IsNullOrEmpty(value.Objeto.Num_Obra) ||
                                !value.Objeto.Num_Doc.HasValue)
                                throw new Exception("Contrato, Projeto, Obra e Documento não informado");

                            modelo = await repository.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto &&
                                                                       p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);

                            var status_old = modelo.Status;


                            // busca se o horario escolhido se mantem valido no momento de gravar
                            var documentos = await new Documento_ProducaoApiController().GetDocumentoAgendamento(value.Objeto.CodCt, value.Objeto.Projeto, value.Objeto.Num_Obra, value.Objeto.Num_Doc.GetValueOrDefault(0));
                            var documento = documentos.FirstOrDefault();

                            double? tempoExecucaoEmMinutos = documento.TempoExecucao.HasValue ? documento.TempoExecucao.Value.TimeOfDay.TotalMinutes : (double?)null;
                            var retornoApi = await GetDatasDisponiveis(documento.Cod_Tipo_Serv, documento.Cod_Natureza, documento.Cod_Grupo_Area, documento.Cod_Area, documento.Cod_Localidade, documento.Localidade, tempoExecucaoEmMinutos, Convert.ToDateTime(value.Objeto.Data));
                            this.QuantidadeDias = 1;

                            bool disponivel = true;

                            if (config.Considerar_Agendamento_Periodo == false)
                            {
                                var horariosDisponiveis = retornoApi.Objeto.SelectMany(p => p.Horas);
                                var DataHora_Inicio = Convert.ToDateTime(String.Concat(value.Objeto.Data, ' ', value.Objeto.Hora_Inicio));
                                var DataHora_Fim = Convert.ToDateTime(String.Concat(value.Objeto.Data, ' ', value.Objeto.Hora_Final));
                                disponivel = horariosDisponiveis.FirstOrDefault(p => p.Hora_Inicio == DataHora_Inicio && p.Hora_Final == DataHora_Fim) != null;
                            }
                            else
                            {
                                var horariosDisponiveis = retornoApi.Objeto.SelectMany(p => p.HorasTurno);
                                var horario = horariosDisponiveis.FirstOrDefault(p => p.Turno == value.Objeto.Turno &&
                                                                                      p.Hora_Inicio.Date == Convert.ToDateTime(value.Objeto.Data) &&
                                                                                      p.Equipe_Tmp != null);
                                if (horario != null)
                                {
                                    value.Objeto.Data = horario.Hora_Inicio.ToString("dd/MM/yyyy");
                                    value.Objeto.Hora = horario.Hora_InicioStr;
                                    value.Objeto.Hora_Inicio = horario.Hora_InicioStr;
                                    value.Objeto.Hora_Final = horario.Hora_FinalStr;
                                    value.Objeto.Deslocamento = horario.Deslocamento;
                                    value.Objeto.Equipe_Tmp = horario.LimiteAgendamentoUltrapassado ? horario.EquipeTmpComLimite : horario.Equipe_Tmp;
                                    disponivel = true;
                                }
                                else
                                {
                                    disponivel = false;
                                }

                            }

                            if (disponivel == false)
                            {
                                throw new Exception(Resources.RsCadastros.AGENDAMENTO_NAO_DISPONIVEL);
                            }



                            if (modelo.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                                modelo.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                                modelo.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                            }
                            if (modelo.Status == StatusExecucaoAniel.Cancelado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra cancelado.");
                            }

                            if (modelo == null)
                            {
                                throw new Exception("Documento Produção não encontrado");
                            }

                            string textoReagendamento = !String.IsNullOrEmpty(value.Objeto.CodMotivoReagendamento) ? "Reagendamento do serviço {0} {1} (Cliente)" : "Agendamento do serviço {0} {1} (Cliente)";

                            var ObsMotivoReagendamento = "";
                            long CodMotivoReagendamento = 0;
                            if (!String.IsNullOrEmpty(value.Objeto.CodMotivoReagendamento))
                            {


                                CodMotivoReagendamento = value.Objeto.CodMotivoReagendamento != "0" ? Convert.ToInt64(value.Objeto.CodMotivoReagendamento) : 0;
                                ObsMotivoReagendamento = !String.IsNullOrEmpty(value.Objeto.ObsMotivoReagendamento) ? value.Objeto.ObsMotivoReagendamento : null;

                                await api.InsertLog_Lanc_ServicoReagendamento(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                             String.Format(textoReagendamento, value.Objeto.Data, value.Objeto.Hora),
                                                             modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             value.Usuario, Convert.ToDateTime(value.Objeto.Data), null, value.Objeto.Data + " " + value.Objeto.Hora, null, CodMotivoReagendamento, ObsMotivoReagendamento, transaction);

                            }
                            else
                            {
                                var flagRetorno = await api.InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                             String.Format(textoReagendamento, value.Objeto.Data, value.Objeto.Hora),
                                                             modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             value.Usuario, Convert.ToDateTime(value.Objeto.Data), null, value.Objeto.Data + " " + value.Objeto.Hora, null, transaction);
                            }
                            modelo.ObsMotivoReagendamento = ObsMotivoReagendamento;
                            modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                            modelo.Data_Maxima = Convert.ToDateTime(value.Objeto.Data);
                            modelo.Hora_Maxima = Convert.ToDateTime(value.Objeto.Hora);
                            modelo.Usuario_Agendamento = value.Usuario;
                            modelo.DiaHora_Agendamento = DateTime.Now;
                            modelo.Equipe = null;
                            modelo.Responsabilidade = 0;
                            modelo.Roterizar = "N";
                            modelo.CodMotivoReagendamento = CodMotivoReagendamento;
                            if (config.Considerar_Agendamento_Periodo)
                            {
                                modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(session, (modelo.Data_Maxima.Value.Date + modelo.Hora_Maxima.Value.TimeOfDay), Convert.ToDateTime(value.Objeto.Hora_Final).TimeOfDay - Convert.ToDateTime(value.Objeto.Hora_Inicio).TimeOfDay)?.Id;
                            }

                            await repository.SaveOrUpdate(modelo);

                            if (modelo.Data_Maxima.HasValue && modelo.Hora_Maxima.HasValue)
                            {
                                try
                                {
                                    ISmsRepository repoSms =
                                        new SmsRepository(
                                            CONSTANTES.URLPortalApiAniel + "/api",
                                            config.Usuario_APIANIEL,
                                            config.Senha_APIANIEL,
                                            config.Token_APIANIEL
                                        );

                                    var smsBusiness = new IntegracaoSmsBusiness(repoSms);

                                    string numeroCliente = modelo.CelularCliente ?? modelo.Cliente?.Telefone_Celular;
                                    string mensagem =
                                        String.Format(
                                            "Serviço {0} agendado para {1} {2}",
                                            modelo.Num_Obra,
                                            modelo.Data_Maxima.Value.ToString("dd/MM/yyyy"),
                                            modelo.Hora_Maxima.Value.ToString("HH:mm")
                                        );

                                    if (!String.IsNullOrEmpty(numeroCliente))
                                        smsBusiness.EnviarSms(numeroCliente, mensagem);
                                }
                                catch (Exception ex)
                                {
                                    CONSTANTES.log4Net.Error("ERRO AO ENVIAR SMS DE AGENDAMENTO - " + ex.Message);
                                }
                            }

                            if (status_old != modelo.Status)
                            {
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                            modelo.CodCt,
                                                                            modelo.Projeto,
                                                                            modelo.Num_Obra,
                                                                            modelo.Num_Doc,
                                                                            "Mudança de Status",
                                                                            HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                            value?.Usuario,
                                                                            null,
                                                                            null,
                                                                            HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value),
                                                                            null,
                                                                            transaction
                                                                            );

                                var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(21, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc, null);
                            }
                        }

                        using (var repository = new Conexao<Agendamento_EquipeAniel>(session))
                        {
                            List<Agendamento_EquipeAniel> agendamentos = (await repository.select(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto &&
                                                                                                    p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc)).ToList();
                            // p.DataHora_Inicio.Date >= Convert.ToDateTime(value.Objeto.Data)


                            await repository.Remove(agendamentos);

                            using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(session))
                            using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(session))
                            {
                                var agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Cod_Escritorio = modelo.Cod_Escritorio,
                                    Cod_Area = modelo.Cod_Area,
                                    Cod_Localidade = modelo.Cod_Localidade,
                                    DataHora_Inicio = Convert.ToDateTime(String.Concat(value.Objeto.Data, ' ', value.Objeto.Hora_Inicio)),
                                    DataHora_Fim = Convert.ToDateTime(String.Concat(value.Objeto.Data, ' ', value.Objeto.Hora_Final)),
                                    Flexivel = value.Objeto.Flexivel,
                                    Deslocamento = value.Objeto.Deslocamento,
                                    Equipe = null,//!String.IsNullOrEmpty(modelo.Equipe) ? await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe) : null,
                                    Equipe_Tmp = value.Objeto.LimiteAgendamentoUltrapassado ? value.Objeto.EquipeTmpComLimite : value.Objeto.Equipe_Tmp,
                                    Log = GerarLog(value.Objeto.Lista_Forca_Trabalho, value.Objeto.Lista_Ja_Agendados, config),
                                    Tipo_Servico = modelo.Cod_Tipo_Serv.HasValue ? await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv) : null,
                                    Tipo_Imovel = modelo.Cod_Tipo_Imovel.HasValue ? await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel) : null,
                                    UsuarioCriacao = value.Usuario,
                                    Origem = OrigemAgendamentoOS.Agendamento,
                                    Latitude = modelo.GetLatitude_Str(),
                                    Longitude = modelo.GetLongitude_Str(),
                                    //Status_Cobranca=modelo.Status_Cobranca
                                };

                                await repository.Save(agendamento);

                                if (agendamento.Equipe != null)
                                {
                                    if (!String.IsNullOrEmpty(agendamento.Equipe.Device_Id))
                                    {
                                        MobileService.NotificarSmartphone(agendamento.Equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                    }
                                }

                            }





                        }



                        transaction.Commit();

                        if (modelo.Data_Maxima.HasValue && value.Objeto.Roteirizar)
                        {
                            var resultado_roteirizacao = await api_Roteirizacao.Roteirizar(new ParamWebApi<RoteirizarServicosModel>
                            {
                                Objeto = new RoteirizarServicosModel
                                {
                                    ids = new List<string> { String.Join("|", new List<String> {
                                            modelo.CodCt, modelo.Projeto,
                                            modelo.Num_Obra,
                                            modelo.Num_Doc.ToString(),
                                        })
                                    },
                                    data = modelo.Data_Maxima.Value.ToString("dd/MM/yyyy"),
                                    numero_max_atendimento = 1000,
                                    encaixe = true
                                },
                                Usuario = value.Usuario,
                                Token = value.Token
                            });

                            if (resultado_roteirizacao.Ok == false)
                            {
                                throw new Exception($"Não foi possível roteirizar essa Ordem de Serviço.Verifique o log de roteirização para obter detalhes.");
                            }

                        }

                        if (transaction.WasCommitted == false)
                            transaction.Commit();


                    }
                }

                retorno.IdObjeto = 1;
                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;
        }

        private String GerarLog(List<String> Lista_Forca_Trabalho, List<String> Lista_Ja_Agendados, Configuracoes_Gerais config)
        {
            return $"Força de Trabalho: {String.Join(",", Lista_Forca_Trabalho)}\nJa Agendados:{String.Join(",", Lista_Ja_Agendados)}\n" +
                   $"Exige Agendamento Para Criação dos Serviços ? {config.Exige_Agendamento_Servico}\n" +
                   $"Empresa Utilizará Roteirização de Serviços ? {config.Empresa_Roteirizacao_Servico}\n" +
                   $"Criterio Habilidade:{config.Criterio_Habilidade}\nCriterio Area: {config.Criterio_Area}\nCriterio SLA:{config.Criterio_SLA_Execucao}";
        }

        #endregion

        #region Mobile

        private void ValidaMobile(ISession session, ISession sessionT5, Int64? Id_Cliente_PlanoProduto, Int64? Id_Tipo_Atendimento, short? Cod_Tipo_Serv_OS, ref Cliente_PlanoProduto clientePlanoProduto, ref Tipo_Atendimento tipoAtendimento,
            ref short? Cod_Tipo_Serv)
        {
            using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                clientePlanoProduto = repoCliente_PlanoProduto.selectFirst(p => p.Id == Id_Cliente_PlanoProduto).Result;

            if (clientePlanoProduto == null)
            {
                throw new Exception("Plano/Produto não encontrado");
            }

            if (Cod_Tipo_Serv_OS.HasValue && Cod_Tipo_Serv_OS != 0)
            {
                Cod_Tipo_Serv = Cod_Tipo_Serv_OS.Value;
            }

            if (!Cod_Tipo_Serv.HasValue)
            {
                using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
                    tipoAtendimento = repoTipo_Atendimento.selectFirst(p => p.Id == Id_Tipo_Atendimento).Result;

                if (tipoAtendimento == null)
                {
                    throw new Exception("Tipo Atendimento não encontrado");
                }

                if (String.IsNullOrEmpty(tipoAtendimento.Desc_Tipo_Servico))
                {
                    throw new Exception("Tipo Atendimento não vinculado a tipo de serviço");
                }

                Cod_Tipo_Serv = Convert.ToInt16(tipoAtendimento.Desc_Tipo_Servico);
            }


            if (String.IsNullOrEmpty(clientePlanoProduto?.Localidade))
            {
                throw new Exception("Localidade não informada na instalação");
            }
        }

        private void ValidaMobileLocalidade(ISession sessionT5, String Cod_Escritorio, String Cod_Area, Int32? Cod_Localidade, ref LocalidadeAniel localidade)
        {
            using (var repo = new Conexao<LocalidadeAniel>(sessionT5))
            {
                localidade = repo.selectFirst(p => p.Cod_Escritorio == Cod_Escritorio &&
                                                                                      p.Cod_Area == Cod_Area &&
                                                                                      p.Cod_Localidade == Cod_Localidade).Result;
            }
            if (localidade == null)
            {
                throw new Exception("Localidade não encontrada");
            }
            if (String.IsNullOrEmpty(localidade.CodCt) || String.IsNullOrEmpty(localidade.Projeto))
            {
                throw new Exception("CodCt/Projeto não informado na Localidade");
            }
        }

        // Endpoint utilizado pelo Olá
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet()]
        [Route("api/Agendamento_Equipeapi/GetDatasDisponiveisMobile")]
        public async Task<GetApiRetorno<List<AgendamentoDiaModel>>> GetDatasDisponiveisMobile(Int64? Id_Cliente_PlanoProduto, Int64? Id_Tipo_Atendimento, short? Cod_Tipo_Serv_OS)
        {
            try
            {
                if (!Id_Cliente_PlanoProduto.HasValue)
                {
                    throw new Exception("Cliente Plano Produto não informado");
                }

                GetApiRetorno<List<ObjBusca<DateTime>>> retorno = new GetApiRetorno<List<ObjBusca<DateTime>>>();

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoLocalidadeAniel = new Conexao<LocalidadeAniel>(sessionT5))
                using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var repoTipo_Servico_Equipe_ImovelAniel = new Conexao<Tipo_Servico_Equipe_ImovelAniel>(sessionT5))
                {
                    Cliente_PlanoProduto clientePlanoProduto = null;
                    Tipo_Atendimento tipoAtendimento = null;
                    short? Cod_Tipo_Serv = null;

                    ValidaMobile(session, sessionT5, Id_Cliente_PlanoProduto, Id_Tipo_Atendimento, Cod_Tipo_Serv_OS, ref clientePlanoProduto, ref tipoAtendimento, ref Cod_Tipo_Serv);

                    DateTime? tempoExecucao = null;

                    LocalidadeAniel localidade = await repoLocalidadeAniel.selectFirst(p => p.Cod_Escritorio == clientePlanoProduto.Cod_Escritorio &&
                                                    p.Cod_Area == clientePlanoProduto.Cod_Area && p.Cod_Localidade == clientePlanoProduto.Cod_Localidade);

                    Tipo_Servico_EquipeAniel tipoServicoEquipe = await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == Cod_Tipo_Serv && p.CodCt == localidade.CodCt);

                    if (tipoServicoEquipe == null)
                    {
                        throw new Exception($"Tipo de Serviço {Cod_Tipo_Serv} não encontrado no contrato {localidade.CodCt}.");
                    }

                    if (clientePlanoProduto?.Endereco_Instalacao?.Tipo_Imovel != null)
                    {
                        Tipo_Servico_Equipe_ImovelAniel objTempo = await repoTipo_Servico_Equipe_ImovelAniel.selectFirst(p => p.Tipo_Servico == tipoServicoEquipe && p.Tipo_Imovel.Cod_Tipo_Imovel == clientePlanoProduto.Endereco_Instalacao.Tipo_Imovel.Cod_Tipo_Imovel);
                        if (objTempo == null)
                        {
                            objTempo = await repoTipo_Servico_Equipe_ImovelAniel.selectFirst(p => p.Tipo_Servico == tipoServicoEquipe && p.Tipo_Imovel.Cod_Tipo_Imovel == clientePlanoProduto.Endereco_Instalacao.Tipo_Imovel.Cod_Tipo_Imovel);
                        }


                        if (objTempo != null)
                        {
                            tempoExecucao = objTempo.Horas;
                        }
                    }

                    return await GetDatasDisponiveis_Det(session, sessionT5, tipoServicoEquipe?.Cod_Tipo_Serv, tipoServicoEquipe?.Cod_Natureza, clientePlanoProduto.Cod_Escritorio,
                        clientePlanoProduto.Cod_Area, clientePlanoProduto.Cod_Localidade, localidade.LocalidadeStr, tempoExecucao, DateTime.Today);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return new GetApiRetorno<List<AgendamentoDiaModel>>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message,
                };
            }
        }

        // Endpoint utilizado pelo Olá
        [AllowAnonymous]
        [HttpPost()]
        [Route("api/Agendamento_Equipeapi/GravarMobile")]
        public async Task<PostApiRetorno<Int64>> GravarMobile(AgendamentoMobileRequest request)
        {

            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                if (string.IsNullOrEmpty(request.HoraInicio))
                {
                    throw new Exception(String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATAINICIAL));
                }
                if (string.IsNullOrEmpty(request.HoraFinal))
                {
                    throw new Exception(String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATAFINAL));
                }

                DateTime horaInicio = DateTime.MinValue;
                if (!DateTime.TryParse(request.HoraInicio, out horaInicio))
                {
                    throw new Exception(String.Format(Resources.RsBase.INVALIDO_FORMAT, Resources.RsCadastros.DATAINICIAL));
                }

                DateTime horaFinal = DateTime.MinValue;
                if (!DateTime.TryParse(request.HoraFinal, out horaFinal))
                {
                    throw new Exception(String.Format(Resources.RsBase.INVALIDO_FORMAT, Resources.RsCadastros.DATAFINAL));
                }

                Documento_ProducaoAniel modelo = new Documento_ProducaoAniel();

                using (var documentoProducaoApi = new Documento_ProducaoApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var apiDocProd = new Documento_ProducaoApiController())
                {
                    using (ITransaction transaction = sessionT5.BeginTransaction())
                    {
                        using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                        using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
                        {
                            IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                            var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                            var config = await repositoryConfig.selectFirst();
                            Cliente_PlanoProduto clientePlanoProduto = null;
                            Tipo_Atendimento tipoAtendimento = null;
                            short? Cod_Tipo_Serv = null;
                            LocalidadeAniel localidade = null;

                            ValidaMobile(session, sessionT5, request.Id_Cliente_PlanoProduto, request.Id_Tipo_Atendimento, null, ref clientePlanoProduto, ref tipoAtendimento, ref Cod_Tipo_Serv);
                            ValidaMobileLocalidade(sessionT5, clientePlanoProduto.Cod_Escritorio, clientePlanoProduto.Cod_Area, clientePlanoProduto.Cod_Localidade, ref localidade);

                            String num_Obra = documentoBusiness.GetNum_Obra(config);
                            Int64 _seq = documentoBusiness.GerarSeq(localidade.CodCt, localidade.Projeto, num_Obra, Convert.ToInt32(clientePlanoProduto.Cliente.Numero_Contrato));

                            String unidade_Custo = String.Empty;
                            using (var repoContrato = new Conexao<ContratoAniel>(sessionT5))
                            {
                                unidade_Custo = (await repoContrato.selectFirst(p => p.CodCt == localidade.CodCt))?.Unidade_Custo;
                            }

                            await documentoProducaoApi.InsertLog_Lanc_Servico(sessionT5, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                            String.Format("Agendamento do serviço {0} (Cliente)", request.HoraInicio),
                                                            modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                            CONSTANTES.USUARIOOLA, horaInicio, null, request.HoraInicio, null, transaction);

                            modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                            modelo.CodCt = localidade.CodCt;
                            modelo.Projeto = localidade.Projeto;
                            modelo.Num_Obra = String.Format("{0}/{1}", num_Obra, _seq);
                            modelo.Seq = Convert.ToInt32(_seq);
                            modelo.Numero_Obra_Original = num_Obra;
                            modelo.Num_Doc = Convert.ToInt32(clientePlanoProduto.Cliente.Numero_Contrato);
                            modelo.OXBDPEE = clientePlanoProduto.Cliente.Numero_Contrato;
                            modelo.Tarefa = clientePlanoProduto.Cliente.Numero_Contrato;
                            modelo.Unidade_Custo = unidade_Custo;
                            modelo.Usuario = CONSTANTES.USUARIOOLA;
                            modelo.Data_Maxima = horaInicio;
                            modelo.Hora_Maxima = horaInicio;
                            modelo.Dia = DateTime.Now;
                            modelo.Hora = DateTime.Now;
                            modelo.Equipe = null;
                            modelo.Responsabilidade = 0;
                            modelo.Roterizar = "N";
                            modelo.Observacao = request.DescricaoProblema;
                            modelo.Observacao_Aux = request.DescricaoProblema;
                            modelo.Usuario_Agendamento = CONSTANTES.USUARIOOLA;
                            modelo.DiaHora_Agendamento = DateTime.Now;
                            modelo.Id_Atendimento = request.Id_Tipo_Atendimento;


                            await apiDocProd.BindViewToObjMobile(sessionT5, session, modelo, localidade, clientePlanoProduto, Cod_Tipo_Serv.Value);

                            await repository.SaveOrUpdate(modelo);
                        }

                        using (var repository = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                        {
                            Agendamento_EquipeAniel agendamento = await repository.selectFirst(p => p.CodCt == modelo.CodCt && p.Projeto == modelo.Projeto &&
                                                                                                    p.Num_Obra == modelo.Num_Obra && p.Num_Doc == modelo.Num_Doc &&
                                                                                                    p.DataHora_Inicio.Date >= horaInicio);

                            if (agendamento != null)
                                await repository.Remove(agendamento);

                            using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                            using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(sessionT5))
                            {
                                agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Cod_Escritorio = modelo.Cod_Escritorio,
                                    Cod_Area = modelo.Cod_Area,
                                    Cod_Localidade = modelo.Cod_Localidade,
                                    DataHora_Inicio = horaInicio,
                                    DataHora_Fim = horaFinal,
                                    Deslocamento = request.Deslocamento,
                                    Equipe = null,//await new Conexao<EquipeAniel>(sessionT5).selectFirst(p => p.Matricula == modelo.Equipe),
                                    Equipe_Tmp = request.Equipe_Tmp,
                                    Log = $"Força de Trabalho: {String.Join(",", request.Lista_Forca_Trabalho)}\nJa Agendados:{String.Join(",", request.Lista_Ja_Agendados)}",
                                    Tipo_Servico = await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv),
                                    Tipo_Imovel = await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel),
                                    UsuarioCriacao = CONSTANTES.USUARIOOLA,
                                    Origem = OrigemAgendamentoOS.Mobile,
                                    Latitude = modelo.GetLatitude_Str(),
                                    Longitude = modelo.GetLongitude_Str()
                                };
                            }

                            await repository.Save(agendamento);
                        }

                        transaction.Commit();
                    }
                    retorno.IdObjeto = 1;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;
        }

        /// <summary>
        /// Endpoint utilizado pelo Premier para consulta de disponibilidade de datas e horários
        /// </summary>
        /// <param name="IdVenda"></param>
        /// <param name="IdPlanoProduto"></param>
        /// <param name="Dias">Quantidade de dias a partir do dia atual que serão retornados.
        ///                     0 para o dia atual, -1 para quantidade padrão, 1+ para demais dias</param>
        /// <returns>Retorna a disponibilidade dos dias</returns>
        [HttpGet()]
        [Route("api/Agendamento_Equipeapi/GetDisponibilidadeDatasMobile")]
        public async Task<GetApiRetorno<List<AgendamentoDiaModel>>> GetDisponibilidadeDatasMobile(Int64? IdVenda, Int64? IdPlanoProduto, String DataIni)
        {
            var retorno = new GetApiRetorno<List<AgendamentoDiaModel>>();

            try
            {
                if (!IdVenda.HasValue && !IdPlanoProduto.HasValue)
                {
                    throw new Exception("ID da venda ou do plano/produto não informado");
                }
                else if (String.IsNullOrEmpty(DataIni))
                {
                    throw new Exception("Período não informado");
                }

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoLocalidadeAniel = new Conexao<LocalidadeAniel>(sessionT5))
                using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var repoTipo_Servico_Equipe_ImovelAniel = new Conexao<Tipo_Servico_Equipe_ImovelAniel>(sessionT5))
                {
                    Venda venda = null;
                    Cliente cliente = null;
                    Cliente_PlanoProduto clientePlanoProduto = null;

                    venda = await repoVenda.selectFirst(p => p.Id == IdVenda);
                    cliente = (await repoCliente.queryover().Where(p => p.Id == venda.Id_Cliente).ListAsync()).FirstOrDefault();
                    clientePlanoProduto = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id && p.Id == IdPlanoProduto).FirstOrDefault();

                    long? Id_Tipo_Servico = clientePlanoProduto.Plano != null ? clientePlanoProduto.Plano.Id_Tipo_Servico : clientePlanoProduto.Produto.Id_Tipo_Servico;

                    if (Id_Tipo_Servico == null)
                    {
                        throw new Exception("Tipo de Serviço não configurado no cadastro de produto.");
                    }

                    DateTime? tempoExecucao = null;

                    LocalidadeAniel localidade = await repoLocalidadeAniel.selectFirst(p => p.Cod_Escritorio == clientePlanoProduto.Cod_Escritorio &&
                                                    p.Cod_Area == clientePlanoProduto.Cod_Area && p.Cod_Localidade == clientePlanoProduto.Cod_Localidade);

                    Tipo_Servico_EquipeAniel tipoServicoEquipe = await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == Id_Tipo_Servico && p.CodCt == localidade.CodCt);

                    if (tipoServicoEquipe == null)
                    {
                        throw new Exception($"Tipo de Serviço não encontrado no contrato {localidade.CodCt}.");
                    }

                    if (clientePlanoProduto?.Endereco_Instalacao?.Tipo_Imovel != null)
                    {
                        Tipo_Servico_Equipe_ImovelAniel objTempo = await repoTipo_Servico_Equipe_ImovelAniel.selectFirst(p => p.Tipo_Servico == tipoServicoEquipe && p.Tipo_Imovel.Cod_Tipo_Imovel == clientePlanoProduto.Endereco_Instalacao.Tipo_Imovel.Cod_Tipo_Imovel);
                        if (objTempo == null)
                        {
                            objTempo = await repoTipo_Servico_Equipe_ImovelAniel.selectFirst(p => p.Tipo_Servico == tipoServicoEquipe && p.Tipo_Imovel.Cod_Tipo_Imovel == clientePlanoProduto.Endereco_Instalacao.Tipo_Imovel.Cod_Tipo_Imovel);
                        }


                        if (objTempo != null)
                        {
                            tempoExecucao = objTempo.Horas;
                        }
                    }

                    DateTime DataInicial = DateTime.ParseExact(DataIni, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //DateTime DataFinal = DateTime.ParseExact(DataFin, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    retorno = await GetDatasDisponiveis_Det(session, sessionT5, tipoServicoEquipe.Cod_Tipo_Serv, tipoServicoEquipe.Cod_Natureza, clientePlanoProduto.Cod_Escritorio,
                        clientePlanoProduto.Cod_Area, clientePlanoProduto.Cod_Localidade, clientePlanoProduto.Localidade, tempoExecucao, DataInicial);
                }
            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<AgendamentoDiaModel>>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }

            return retorno;
        }

        /// <summary>
        /// Endpoint utilizado pelo Premier para agendamento de horário em uma OS
        /// </summary>
        /// <param name="request">Objeto do tipo AgendarMobileRequest, todos os atributos devem ser preenchidos.</param>
        /// <returns>Retorna 1 se gravado com sucesso, 0 caso ocorra erro</returns>
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Agendamento_Equipeapi/AgendarMobile")]
        public async Task<GetApiRetorno<Int64>> AgendarMobile(ParamWebApi<AgendarMobileRequest> request)
        {
            GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var api_Roteirizacao = new Painel_RoteirizacaoApiController())
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    using (ITransaction transaction = sessionT5.BeginTransaction())
                    using (var api = new Documento_ProducaoApiController())
                    {
                        Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                        using (var repository = new Conexao<Documento_ProducaoAniel>(sessionT5))
                        {
                            if (String.IsNullOrEmpty(request.Objeto.CodCt) || String.IsNullOrEmpty(request.Objeto.Projeto) || String.IsNullOrEmpty(request.Objeto.Num_Obra) ||
                                !request.Objeto.Num_Doc.HasValue)
                                throw new Exception("Contrato, Projeto, Obra e Documento não informado");

                            modelo = await repository.selectFirst(p => p.CodCt == request.Objeto.CodCt && p.Projeto == request.Objeto.Projeto &&
                                                                       p.Num_Obra == request.Objeto.Num_Obra && p.Num_Doc == request.Objeto.Num_Doc);

                            #region Codigo Novo
                            var status_old = modelo.Status;


                            // busca se o horario escolhido se mantem valido no momento de gravar
                            var documentos = await new Documento_ProducaoApiController().GetDocumentoAgendamento(request.Objeto.CodCt, request.Objeto.Projeto, request.Objeto.Num_Obra, request.Objeto.Num_Doc.GetValueOrDefault(0));
                            var documento = documentos.FirstOrDefault();

                            double? tempoExecucaoEmMinutos = documento?.TempoExecucao.HasValue == true ? documento.TempoExecucao.Value.TimeOfDay.TotalMinutes : (double?)null;
                            var retornoApi = await GetDatasDisponiveis(documento.Cod_Tipo_Serv, documento.Cod_Natureza, documento.Cod_Grupo_Area, documento.Cod_Area, documento.Cod_Localidade, documento.Localidade, tempoExecucaoEmMinutos, Convert.ToDateTime(request.Objeto.Data));


                            bool disponivel = true;

                            if (config.Considerar_Agendamento_Periodo == false)
                            {
                                var horariosDisponiveis = retornoApi.Objeto.SelectMany(p => p.Horas);
                                var DataHora_Inicio = Convert.ToDateTime(String.Concat(request.Objeto.Data, ' ', request.Objeto.Hora_Inicio));
                                var DataHora_Fim = Convert.ToDateTime(String.Concat(request.Objeto.Data, ' ', request.Objeto.Hora_Final));
                                disponivel = horariosDisponiveis.FirstOrDefault(p => p.Hora_Inicio == DataHora_Inicio && p.Hora_Final == DataHora_Fim) != null;
                            }
                            else
                            {
                                var horariosDisponiveis = retornoApi.Objeto.SelectMany(p => p.HorasTurno);
                                var horario = horariosDisponiveis.FirstOrDefault(p => p.Turno == request.Objeto.Turno &&
                                                                                      p.Hora_Inicio.Date == Convert.ToDateTime(request.Objeto.Data) &&
                                                                                      p.Equipe_Tmp != null);
                                if (horario != null)
                                {
                                    disponivel = true;
                                }
                                else
                                {
                                    disponivel = false;
                                }

                            }

                            if (disponivel == false)
                            {
                                throw new Exception(Resources.RsCadastros.AGENDAMENTO_NAO_DISPONIVEL);
                            }
                            #endregion

                            if (modelo.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                                modelo.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                                modelo.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                            }
                            if (modelo.Status == StatusExecucaoAniel.Cancelado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra cancelado.");
                            }

                            if (modelo == null)
                            {
                                throw new Exception("Documento Produção não encontrado");
                            }

                            await api.InsertLog_Lanc_Servico(sessionT5, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                             String.Format("Agendamento do serviço {0} {1}", request.Objeto.Data, request.Objeto.Hora),
                                                             modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             request.Usuario, Convert.ToDateTime(request.Objeto.Data), null, request.Objeto.Data + request.Objeto.Hora, null, transaction);

                            modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                            modelo.Data_Maxima = Convert.ToDateTime(request.Objeto.Data);
                            modelo.Hora_Maxima = DateTime.ParseExact(request.Objeto.Hora, "HH:mm", CultureInfo.InvariantCulture);
                            modelo.Usuario_Agendamento = request.Usuario;
                            modelo.DiaHora_Agendamento = DateTime.Now;
                            modelo.Equipe = null;
                            modelo.Responsabilidade = 0;
                            modelo.Roterizar = "N";

                            if (config.Considerar_Agendamento_Periodo)
                            {
                                modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(sessionT5, modelo.Data_Maxima.Value.Date + modelo.Hora_Maxima.Value.TimeOfDay, DateTime.ParseExact(request.Objeto.Hora_Final, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay - DateTime.ParseExact(request.Objeto.Hora_Inicio, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay)?.Id;
                            }

                            await repository.SaveOrUpdate(modelo);
                        }

                        using (var repository = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                        {
                            List<Agendamento_EquipeAniel> agendamentos = (await repository.select(p => p.CodCt == request.Objeto.CodCt && p.Projeto == request.Objeto.Projeto &&
                                                                                                    p.Num_Obra == request.Objeto.Num_Obra && p.Num_Doc == request.Objeto.Num_Doc)).ToList();

                            await repository.Remove(agendamentos);

                            using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                            using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(sessionT5))
                            {
                                var agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Cod_Escritorio = modelo.Cod_Escritorio,
                                    Cod_Area = modelo.Cod_Area,
                                    Cod_Localidade = modelo.Cod_Localidade,
                                    DataHora_Inicio = Convert.ToDateTime(String.Concat(request.Objeto.Data, ' ', request.Objeto.Hora_Inicio)),
                                    DataHora_Fim = Convert.ToDateTime(String.Concat(request.Objeto.Data, ' ', request.Objeto.Hora_Final)),
                                    Deslocamento = request.Objeto.Deslocamento,
                                    Equipe = null,
                                    Equipe_Tmp = request.Objeto.Equipe_Tmp,
                                    Log = GerarLog(request.Objeto.Lista_Forca_Trabalho, request.Objeto.Lista_Ja_Agendados, config),
                                    Tipo_Servico = modelo.Cod_Tipo_Serv.HasValue ? await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv) : null,
                                    Tipo_Imovel = modelo.Cod_Tipo_Imovel.HasValue ? await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel) : null,
                                    UsuarioCriacao = request.Usuario,
                                    Origem = OrigemAgendamentoOS.Agendamento,
                                    Latitude = modelo.GetLatitude_Str(),
                                    Longitude = modelo.GetLongitude_Str()
                                };

                                await repository.Save(agendamento);

                                if (agendamento.Equipe != null)
                                {
                                    if (!String.IsNullOrEmpty(agendamento.Equipe.Device_Id))
                                    {
                                        MobileService.NotificarSmartphone(agendamento.Equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                    }
                                }
                            }
                        }

                        transaction.Commit();

                        if (modelo.Data_Maxima.HasValue && request.Objeto.Roteirizar)
                        {
                            var resultado_roteirizacao = await api_Roteirizacao.Roteirizar(new ParamWebApi<RoteirizarServicosModel>
                            {
                                Objeto = new RoteirizarServicosModel
                                {
                                    ids = new List<string> { String.Join("|", new List<String> {
                                            modelo.CodCt, modelo.Projeto,
                                            modelo.Num_Obra,
                                            modelo.Num_Doc.ToString(),
                                        })
                                    },
                                    data = modelo.Data_Maxima.Value.ToString("dd/MM/yyyy"),
                                    numero_max_atendimento = 1000
                                },
                                Usuario = request.Usuario,
                                Token = request.Token
                            });

                            if (resultado_roteirizacao.Ok == false)
                            {
                                throw new Exception($"Não foi possível roteirizar essa Ordem de Serviço.Verifique o log de roteirização para obter detalhes.");
                            }

                        }

                        if (transaction.WasCommitted == false)
                            transaction.Commit();

                    }
                }

                retorno.Objeto = 1;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                retorno.Objeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        #endregion

        public AgendamentoDiaModel BuscarAgendamentoDia(DateTime dia,
                                                        List<AgendamentoDadosModel> jaAgendados,
                                                        List<AgendamentoEquipeHorarioModel> forcaTrabalho,
                                                        List<Horario_Connect> horariosServico,
                                                        Horario_Connect horarioAgendamento,
                                                        Configuracoes_Gerais config,
                                                        bool feriado,
                                                        DadosServicoAgendamento servico,
                                                        List<Turno_ServicoAniel> turnos,
                                                        LimiteAgendamentoAniel limiteAgendamento,
                                                        bool ignorarTempo = false,
                                                        bool ignorarAgendamentosPassados = true)
        {
            AgendamentoDiaModel diaModel = null;

            DayOfWeek diaSemana = dia.DayOfWeek;

            DateTime? tempoExecucao = !servico.TempoExecucaoMinutos.HasValue ? config.Tempo_Execucao_Padrao : servico.TempoExecucaoMinutos;
            Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);

            Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));

            var forcaTrabalhoTotal = new List<AgendamentoEquipeHorarioModel>(forcaTrabalho);

            using (HorarioApiController horarioApiController = new HorarioApiController())
            {
                RoteirizacaoBusiness helper = new RoteirizacaoBusiness();
                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                {
                    Cod_Area = servico.Cod_Area,
                    Cod_Escritorio = servico.Cod_Escritorio,
                    Cod_Localidade = servico.Cod_Localidade,
                    LocalidadeStr = servico.LocalidadeStr,
                    Cod_Natureza = servico.Cod_Natureza
                };

                forcaTrabalho = forcaTrabalho.Select(p =>
                {
                    if (p.Id_Horario_Connect == null)
                    {
                        p.Id_Horario_Connect = horariosServico.Find(p => p.Padrao)?.Id;
                    }
                    return p;
                }).ToList();

                var aptos = forcaTrabalho.Select(p =>
                {
                    TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                    {
                        Naturezas = p.Naturezas,
                        Localidades = p.Localidades,
                        Equipe = p.Equipe,

                    };

                    var capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                    return (p.Equipe, capaz);

                }).ToDictionary(p => p.Equipe, p => p.capaz);

                forcaTrabalho = forcaTrabalho.FindAll(p => aptos[p.Equipe]);
                var forcaTrabalhoAptos = new List<AgendamentoEquipeHorarioModel>(forcaTrabalho); // todos aptos, independente de disponibilidade
                List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == dia.Date);
                var horarioDiaAgendamento = horarioApiController.RetornaHorarioDia(horarioAgendamento, dia, feriado);
                AgendamentoDiaModel configDoDiaAgendamento = GetConfigDoDia(diaSemana, dia, feriado, horarioAgendamento);

                if (configDoDiaAgendamento == null)
                    return null;


                Dictionary<long?, List<AgendamentoDadosModel>> agendamentosPorPerfil = AgendamentosCargaForcaTrabalho(jaAgendadosDia.ToList(),
                                                                                                                     forcaTrabalho,
                                                                                                                     config,
                                                                                                                     forcaTrabalho.Select(p => p.IdPerfilAgendamento).Distinct().Where(p => p != null).ToList());

                forcaTrabalho = forcaTrabalho.FindAll(p => p.Disponivel);
                var gruposForcaTrabalho = forcaTrabalho.Where(p => p.IdPerfilAgendamento != null).GroupBy(p => new { p.Id_Horario_Connect, p.IdPerfilAgendamento });

                foreach (var item in gruposForcaTrabalho)
                {
                    var forcaTrabalhoItem = forcaTrabalho.Where(p => p.IdPerfilAgendamento == item.Key.IdPerfilAgendamento && p.Id_Horario_Connect == item.Key.Id_Horario_Connect);
                    var forcaTrabalhoFiltrada = forcaTrabalho.Where(p => p.IdPerfilAgendamento == item.Key.IdPerfilAgendamento && p.Id_Horario_Connect == item.Key.Id_Horario_Connect).Select(p => p.Equipe);
                    var horarioServico = horariosServico.Find(p => p.Id == item.Key.Id_Horario_Connect) ?? horariosServico.Find(p => p.Padrao);
                    var horarioDiaServico = horarioApiController.RetornaHorarioDia(horarioServico, dia, feriado);

                    AgendamentoDiaModel configDoDia = GetConfigDoDia(diaSemana, dia, feriado, horarioServico);
                    //List<AgendamentoDadosModel> jaAgendadosDiaGrupo = agendamentosPorPerfil[item.Key.IdPerfilAgendamento];
                    List<AgendamentoDadosModel> jaAgendadosDiaGrupo = agendamentosPorPerfil[item.Key.IdPerfilAgendamento].Where(p => forcaTrabalhoFiltrada.Contains(p.Equipe)).ToList();
                    if (configDoDia != null && configDoDiaAgendamento != null)
                    {
                        if (configDoDia.HoraInicio < configDoDiaAgendamento.HoraInicio)
                        {
                            configDoDia.HoraInicio = configDoDiaAgendamento.HoraInicio;
                        }

                        if (configDoDia.HoraTermino > configDoDiaAgendamento.HoraTermino)
                        {
                            configDoDia.HoraTermino = configDoDiaAgendamento.HoraTermino;
                        }

                        foreach (var turno in turnos)
                        {
                            GetHorasDisponiveis(
                                configDoDia,
                                tempoExecucaoMinutos,
                                tempoDeslocamentoMinutos,
                                jaAgendadosDiaGrupo,
                                jaAgendados,
                                item.ToList(),
                                forcaTrabalhoTotal,
                                dia,
                                horariosServico,
                                servico,
                                limiteAgendamento,
                                turno,
                                false
                            );
                        }

                        foreach (var tecnico in item.ToList())
                        {
                            foreach (var turno in turnos)
                            {
                                // TODO: Testar limite de agendamento nesse cenário por turno. Talvez seja necessário modificar essa lista da 
                                //       força de trabalho total
                                GetHorasDisponiveis(
                                    configDoDia,
                                    tempoExecucaoMinutos,
                                    tempoDeslocamentoMinutos,
                                    jaAgendadosDiaGrupo,
                                    jaAgendados,
                                    new List<AgendamentoEquipeHorarioModel>() { tecnico },
                                    forcaTrabalhoTotal,
                                    dia,
                                    horariosServico,
                                    servico,
                                    limiteAgendamento,
                                    turno,
                                    true
                                );
                            }
                        }

                        //Caso não tenha horarios não exibe data
                        if (configDoDia.Horas.Count() > 0)
                        {
                            //Se dia igual a hoje e hora maior que agora
                            if (dia == DateTime.Now.Date)
                            {
                                if (ignorarAgendamentosPassados)
                                {
                                    List<AgendamentoHorasDisponiveis> remover = new List<AgendamentoHorasDisponiveis>();
                                    foreach (var hora in configDoDia.Horas)
                                    {
                                        var dataComparacaoPrimeiroAgendamento = ignorarTempo ? DateTime.Now : DateTime.Now.Add(config.TempoMinimoPrimeiraJanela.GetValueOrDefault(DateTime.Today.Date).TimeOfDay);
                                        if (hora.Hora_Inicio < dataComparacaoPrimeiroAgendamento)
                                            remover.Add(hora);
                                    }

                                    foreach (var hora in remover)
                                        configDoDia.Horas.Remove(hora);


                                    List<AgendamentoHorasDisponiveis> removerCapacidade = new List<AgendamentoHorasDisponiveis>();
                                    foreach (var hora in configDoDia.HorasCapacidade)
                                    {
                                        var dataComparacaoPrimeiroAgendamento = ignorarTempo ? DateTime.Now : DateTime.Now.Add(config.TempoMinimoPrimeiraJanela.GetValueOrDefault(DateTime.Today.Date).TimeOfDay);
                                        if (hora.Hora_Inicio < dataComparacaoPrimeiroAgendamento)
                                            removerCapacidade.Add(hora);
                                    }

                                    foreach (var hora in removerCapacidade)
                                        configDoDia.HorasCapacidade.Remove(hora);

                                }

                                if (configDoDia.Horas.Count() > 0)
                                {
                                    if (diaModel != null)
                                    {
                                        diaModel.Horas.AddRange(configDoDia.Horas);
                                        diaModel.HorasCapacidade.AddRange(configDoDia.HorasCapacidade);
                                    }
                                    else
                                    {
                                        diaModel = configDoDia;
                                    }
                                }
                            }
                            else
                            {
                                if (diaModel != null)
                                {
                                    diaModel.Horas.AddRange(configDoDia.Horas);
                                    diaModel.HorasCapacidade.AddRange(configDoDia.HorasCapacidade);
                                }
                                else
                                {
                                    diaModel = configDoDia;
                                }
                            }
                        }
                    }

                    if (diaModel != null)
                    {
                        var TodosServicosEncontradasEmFaixas = diaModel.Horas
                                                                            .SelectMany(p => p.Lista_Ja_Agendados)
                                                                            .Select(p => p.DadosServico.NumObra)
                                                                            .ToList();


                        var agendadosCompativeisForaFaixaDentroPerfil = jaAgendadosDia.Where(agendamento =>
                        {

                            var docOS = new Painel_ReteirizacaoDoc_Agrupado
                            {
                                Cod_Area = agendamento.DadosServico.Cod_Area,
                                Cod_Escritorio = agendamento.DadosServico.Cod_Escritorio,
                                Cod_Localidade = agendamento.DadosServico.Cod_Localidade,
                                LocalidadeStr = agendamento.DadosServico.LocalidadeStr,
                                Cod_Natureza = agendamento.DadosServico.Cod_Natureza
                            };

                            bool OsPertenceForcaTrabalhoFiltrada = forcaTrabalhoFiltrada.Contains(agendamento.Equipe);
                            bool perfilAgrupamentoAtendeOs = forcaTrabalhoItem.Any(f =>
                            {
                                var tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = f.Naturezas,
                                    Localidades = f.Localidades,
                                    Equipe = f.Equipe
                                };
                                return helper.VerificarTecnicoCapaz(config, tecnico, docOS, null);
                            });

                            return OsPertenceForcaTrabalhoFiltrada && perfilAgrupamentoAtendeOs;
                        }).ToList();

                        var agendadosCompativeisForaFaixaForaPerfil = jaAgendadosDia.Where(agendamento =>
                        {

                            var docOS = new Painel_ReteirizacaoDoc_Agrupado
                            {
                                Cod_Area = agendamento.DadosServico.Cod_Area,
                                Cod_Escritorio = agendamento.DadosServico.Cod_Escritorio,
                                Cod_Localidade = agendamento.DadosServico.Cod_Localidade,
                                LocalidadeStr = agendamento.DadosServico.LocalidadeStr,
                                Cod_Natureza = agendamento.DadosServico.Cod_Natureza
                            };

                            bool perfilAgrupamentoAtendeOs = forcaTrabalhoItem.Any(f =>
                            {
                                var tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = f.Naturezas,
                                    Localidades = f.Localidades,
                                    Equipe = f.Equipe
                                };
                                return helper.VerificarTecnicoCapaz(config, tecnico, docOS, null);
                            });

                            var tecnicoOriginal = forcaTrabalhoTotal.FindAll(p => p.Equipe == agendamento.Equipe);
                            bool tecnicoOriginalAtendeOs = tecnicoOriginal.Any(f =>
                            {
                                var tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = f.Naturezas,
                                    Localidades = f.Localidades,
                                    Equipe = f.Equipe
                                };
                                return helper.VerificarTecnicoCapaz(config, tecnico, docOS, null);
                            });

                            return agendamento.Temporario && perfilAgrupamentoAtendeOs && !tecnicoOriginalAtendeOs;

                        }).ToList();

                        var listaJaAgendadosForaFaixa = agendadosCompativeisForaFaixaDentroPerfil.FindAll(p => TodosServicosEncontradasEmFaixas.FirstOrDefault(y => y == p.DadosServico.NumObra) == null && diaModel.ListaJaAgendadosForaFaixa.FirstOrDefault(y => y.DadosServico.NumObra == p.DadosServico.NumObra) == null);
                        listaJaAgendadosForaFaixa.AddRange(agendadosCompativeisForaFaixaForaPerfil.FindAll(p => TodosServicosEncontradasEmFaixas.FirstOrDefault(y => y == p.DadosServico.NumObra) == null && diaModel.ListaJaAgendadosForaFaixa.FirstOrDefault(y => y.DadosServico.NumObra == p.DadosServico.NumObra) == null));
                        diaModel.ListaJaAgendadosForaFaixa.AddRange(listaJaAgendadosForaFaixa);

                    }
                }
            }

            return diaModel;
        }

        public Dictionary<long, List<LocalidadeRoteirizacao>> GetLocalidadeRoteirizacaoByPerfil(ISession sessionT5)
        {

            Dictionary<long, List<LocalidadeRoteirizacao>> retorno = new Dictionary<long, List<LocalidadeRoteirizacao>>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                sql.AppendLine("SELECT A.ID_PERFIL_AGENDAMENTO,A.COD_ESCRITORIO,A.COD_AREA,A.LOCALIDADE,D.COD_LOCALIDADE,B.DESCRICAO AS DESCRICAO_ESCRITORIO,C.DESCRICAO AS DESCRICAO_AREA,A.NIVEL,A.TODAS_LOCALIDADES,A.TODAS_AREAS");
                sql.AppendLine("FROM TB_PERFIL_AGENDAMENTO_AREA A");
                sql.AppendLine("LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)");
                sql.AppendLine("LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)");
                sql.AppendLine("LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.LOCALIDADE = D.LOCALIDADE)");


                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, com.Transaction).Result)
                {
                    while (reader.ReadAsync().Result)
                    {
                        long idPerfilAgendamento = Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]);
                        if (retorno.ContainsKey(idPerfilAgendamento) == false)
                        {
                            retorno.Add(idPerfilAgendamento, new List<LocalidadeRoteirizacao>());
                        }

                        retorno[idPerfilAgendamento].Add(new LocalidadeRoteirizacao
                        {
                            Cod_Area = Convert.ToString(reader["COD_AREA"]),
                            Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                            Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : 0,
                            Descricao_Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                            Descricao_Escritorio = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                            LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]),
                            Nivel = reader["NIVEL"] != DBNull.Value ? Convert.ToInt16(reader["NIVEL"]) : (short?)null,
                            Todas_Localidades = reader["TODAS_LOCALIDADES"] != DBNull.Value ? Convert.ToBoolean(reader["TODAS_LOCALIDADES"]) : false,
                            Todas_Areas = reader["TODAS_AREAS"] != DBNull.Value ? Convert.ToBoolean(reader["TODAS_AREAS"]) : false,
                        });
                    }
                }
            }


            return retorno;
        }

        [HttpPost()]
        [Route("api/Agendamento_Equipeapi/GetDetalhamentoDisponibilidade")]
        public async Task<DetalhamentoDisponibilidadeDataTableResponse<DetalhamentoDisponibilidadeListaModel>> GetDetalhamentoDisponibilidade(DetalhamentoDisponibilidadeDataTableRequest request)
        {
            try
            {

                GetApiRetorno<List<DetalhamentoDisponibilidadeListaModel>> retorno = new GetApiRetorno<List<DetalhamentoDisponibilidadeListaModel>>();
                retorno.Objeto = new List<DetalhamentoDisponibilidadeListaModel>();
                List<DadosPerfilDisponibilidadeListaModel> dadosPerfil = new List<DadosPerfilDisponibilidadeListaModel>();

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var tipoServicoApi = new Tipo_Servico_EquipeApiController())
                {
                    using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                    using (var repoFeriado = new Conexao<Feriado>(session))
                    using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                    using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                    using (var repoTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
                    using (var repoPerfil = new Conexao<PerfilAgendamento>(sessionT5))
                    using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
                    using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                    {
                        RoteirizacaoBusiness helper = new RoteirizacaoBusiness();
                        Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();
                        List<PerfilAgendamento> perfis = (await repoPerfil.select()).ToList();
                        List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();
                        Turno_ServicoAniel turno = await repoTurno.selectFirst(p => p.Id == request.IdPeriodo);
                        Tipo_Servico_EquipeAniel tipo_Servico_EquipeAniel = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == request.Cod_Tipo_Serv);
                        TimeSpan tempoExecucao = (await tipoServicoApi.TempoServico(sessionT5, Convert.ToInt32(request.Cod_Tipo_Serv), request.IdTipoImovel, config)).valor.Value;
                        DateTime? TempoExecucaoServico = DateTime.Today.Add(tempoExecucao);
                        var turnosBanco = repositoryTurno.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList();
                        List<Feriado> feriados = (await repoFeriado.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Data == Convert.ToDateTime(request.Data))).ToList();

                        var horariosAgendamento = await GetHorarioAgendamento(sessionT5);

                        if (horariosAgendamento == null)
                        {
                            throw new Exception("Horario padrão de agendamento não definido no sistema");
                        }

                        var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                        var perfisLocalidades = GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                        var forcaTrabalhoLista = await GetForcaTrabalho(sessionT5, Convert.ToDateTime(request.Data), config, perfisNaturezas, perfisLocalidades);

                        if (request.IdPerfil.HasValue)
                        {
                            forcaTrabalhoLista = forcaTrabalhoLista.FindAll(p => p.IdPerfilAgendamento == request.IdPerfil);
                        }

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
                                Cod_Escritorio = request.Cod_Grupo_Area,
                                Cod_Area = request.Cod_Area,
                                Cod_Localidade = request.Cod_Localidade,
                                Cod_Natureza = tipo_Servico_EquipeAniel.Cod_Natureza,
                                LocalidadeStr = request.LocalidadeStr
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
                            return horario != null ? GetHorarioPleno(turno, horario, Convert.ToDateTime(request.Data)) : TimeSpan.Zero;
                        }).Sum(p => p.Ticks));


                        var agendamento = (await GetDatasValidasDetalhamento(session,
                                                                             sessionT5,
                                                                             horariosAgendamento,
                                                                             feriados,
                                                                             request.Cod_Tipo_Serv,
                                                                             tipo_Servico_EquipeAniel.Cod_Natureza,
                                                                             request.Cod_Grupo_Area,
                                                                             request.Cod_Area,
                                                                             request.Cod_Localidade,
                                                                             request.LocalidadeStr,
                                                                             TempoExecucaoServico,
                                                                             Convert.ToDateTime(request.Data),
                                                                             request.IdPeriodo,
                                                                             request.IdPerfil)).FirstOrDefault();

                        if (agendamento != null)
                        {
                            foreach (var item in agendamento.Horas)
                            {
                                item.Turno = Convert.ToString(PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao)?.Id);
                            }

                            foreach (var item in agendamento.HorasCapacidade)
                            {
                                item.Turno = Convert.ToString(PeriodoApiController.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao)?.Id);
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

                        retorno.Ok = true;

                        TimeSpan CapacidadeTotal = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                        TimeSpan CapacidadeUtilTotal = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                        var listaNumOSTotal = agendamento != null ? agendamento.Horas.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct() : new List<string>();
                        var agendamentosForaFaixaNoMesmoTurnoMinutosTotal = agendamento != null ? agendamento.ListaJaAgendadosForaFaixa.Sum(p => p.DuracaoMinutos) : 0;
                        var agendamentosManuaisMinutosTotal = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos) : 0;
                        var agendamentosMinutosTotal = agendamento != null ? listaNumOSTotal.Select(y => agendamento.Horas.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos) : 0;
                        //CapacidadeTotal = CapacidadeTotal.Add(TimeSpan.FromMinutes(agendamentosMinutosTotal));

                        TimeSpan Capacidade = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;


                        if (agendamento != null)
                        {
                            retorno.Objeto = agendamento.Horas.SelectMany(p =>
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

                            retorno.Objeto = retorno.Objeto.Concat(agendamento.ListaJaAgendadosForaFaixa.Select(y =>
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

                            var listaNumOS = retorno.Objeto.Select(y => y.NumObra).Distinct();
                            retorno.Objeto = listaNumOS.Select(y => retorno.Objeto.First(z => z.NumObra == y)).ToList();
                            retorno.Objeto = retorno.Objeto.OrderBy(p => p.PerfilAgendamento).ThenBy(p => p.TecnicoTemporario).ThenBy(p => p.DataHoraCriacao).Select(p =>
                            {
                                Capacidade = Capacidade.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                p.CapacidadeRestante = ((int)Capacidade.TotalHours).ToString("D2") + Capacidade.ToString(@"\:mm");
                                return p;
                            }).ToList();


                            List<PerfilAgendamento> perfisForcaTrabalho = perfis.FindAll(y => forcaTrabalhoLista.Select(p => p.IdPerfilAgendamento).Contains(y.Id));
                            dadosPerfil = perfisForcaTrabalho.Select(groupPerfil =>
                            {
                                List<string> turnosEncontrados = agendamento.Horas.Select(p => p.Turno).Distinct().ToList();
                                TimeSpan CapacidadePerfil = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;


                                var forcaTrabalhoPerfilTmp = agendamento.Horas.FindAll(y => y.IdPerfil == groupPerfil.Id);
                                var forcaTrabalhoBaseadoEmTempo = forcaTrabalhoPerfilTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                var listaNumOS = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                var agendamentosForaFaixaNoMesmoTurnoMinutos = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => groupPerfil.Id == y.IdPerfil).Sum(p => p.DuracaoMinutos);
                                var todosAgendamentos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Sum(p => p.DuracaoMinutos);
                                int capacidadePeriodoBaseadoEmTempoHoraisTotais = Convert.ToInt32(Math.Floor((forcaTrabalhoBaseadoEmTempo - (todosAgendamentos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucao.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));

                                var agendamentoPerfil = agendamento.Horas.FindAll(y => y.IdPerfil == groupPerfil.Id);
                                List<DadosPerfilDisponibilidadeListaModel> dadosPerfilTurno = new List<DadosPerfilDisponibilidadeListaModel>();
                                var agendamentoPerfilCapacidadeTecnico = agendamento.HorasCapacidade.FindAll(y => y.IdPerfil == groupPerfil.Id && y.Equipe_Tmp != null);
                                var ForcaTrabalho = agendamentoPerfilCapacidadeTecnico.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                var agendamentosManuaisMinutos = listaNumOS.Select(y => agendamentoPerfil.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                var agendamentosMinutos = listaNumOS.Select(y => agendamentoPerfil.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                //CapacidadePerfil = CapacidadePerfil.Add(TimeSpan.FromMinutes(agendamentosMinutos));
                                var CapacidadePerfilParcial = TimeSpan.FromMinutes(CapacidadePerfil.TotalMinutes);
                                TimeSpan CapacidadeUtilPerfil = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;

                                if (config.Considerar_Agendamento_Periodo)
                                {
                                    foreach (var turnoEncontrado in turnosEncontrados)
                                    {
                                        Turno_ServicoAniel turnoBanco = turnosBanco.Find(p => p.Id.ToString() == turnoEncontrado);
                                        TimeSpan CapacidadePerfilTurno = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
                                        var agendamentoPerfilTurno = agendamento.Horas.FindAll(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id);
                                        var agendamentoPerfilCapacidadeTecnicoTurno = agendamento.HorasCapacidade.FindAll(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id && y.Equipe_Tmp != null);
                                        var ForcaTrabalhoPerfil = agendamentoPerfilCapacidadeTecnicoTurno.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                        var listaNumOSPerfil = agendamentoPerfilTurno.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                        var agendamentosForaFaixaNoMesmoTurnoMinutosPerfil = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => BetweenIgual(y.DataHoraInicio, turnoBanco.Hora_Ini, turnoBanco.Hora_Fim) && groupPerfil.Id == y.IdPerfil).Sum(p => p.DuracaoMinutos);
                                        var agendamentosManuaisMinutosPerfil = listaNumOSPerfil.Select(y => agendamentoPerfilTurno.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                        var agendamentosMinutosPerfil = listaNumOSPerfil.Select(y => agendamentoPerfilTurno.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                                        //CapacidadePerfilTurno = CapacidadePerfilTurno.Add(TimeSpan.FromMinutes(agendamentosMinutosPerfil));
                                        TimeSpan CapacidadePerfilTurnoParcial = TimeSpan.FromMinutes(CapacidadePerfilTurno.TotalMinutes);

                                        var forcaTrabalhoPerfilTmpTurno = agendamento.Horas.FindAll(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id);
                                        var forcaTrabalhoBaseadoEmTempoTurno = forcaTrabalhoPerfilTmpTurno.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                                        var listaNumOSTurno = forcaTrabalhoPerfilTmpTurno.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                                        var todosAgendamentosTurno = listaNumOSTurno.Select(y => forcaTrabalhoPerfilTmpTurno.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Sum(p => p.DuracaoMinutos);
                                        int capacidadePeriodoBaseadoEmTempoHoraisTotaisTurno = Convert.ToInt32(Math.Floor((forcaTrabalhoBaseadoEmTempo - (todosAgendamentos + agendamentosForaFaixaNoMesmoTurnoMinutosPerfil)) / (tempoExecucao.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));

                                        TimeSpan CapacidadeUtilPerfilTurno = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Where(y => y.Turno == turnoEncontrado && y.IdPerfil == groupPerfil.Id).Sum(y => y.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;

                                        retorno.Objeto.FindAll(p => p.PerfilAgendamento == groupPerfil.Descricao
                                                                    && BetweenIgual(Convert.ToDateTime(p.DataInicioAgendamento), turnoBanco.Hora_Ini, turnoBanco.Hora_Fim)).Select(p =>
                                                                    {
                                                                        CapacidadePerfilTurnoParcial = CapacidadePerfilTurnoParcial.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
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


                                        int capacidadeServicosPerfilBaseadoEmTempo = Convert.ToInt32(Math.Floor((CapacidadePerfilTurnoParcialCalculoTempo.TotalMinutes) / (tempoExecucao.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));
                                        if (capacidadeServicosPerfilBaseadoEmTempo < capacidadePerfilServicos)
                                        {
                                            capacidadePerfilServicos = capacidadeServicosPerfilBaseadoEmTempo;
                                        }

                                        if (capacidadePeriodoBaseadoEmTempoHoraisTotaisTurno < capacidadePerfilServicos)
                                        {
                                            capacidadePerfilServicos = capacidadePeriodoBaseadoEmTempoHoraisTotais;
                                        }

                                        if (capacidadePerfilServicos < 0)
                                            capacidadePerfilServicos = 0;


                                        dadosPerfilTurno.Add(new DadosPerfilDisponibilidadeListaModel
                                        {
                                            perfilAgendamento = groupPerfil.Descricao,
                                            capacidadeRestante = CapacidadePerfilTurnoParcial.TotalMinutes > 0 ? ((int)CapacidadePerfilTurnoParcial.TotalHours).ToString("D2") + CapacidadePerfilTurnoParcial.ToString(@"\:mm") : "00:00",
                                            capacidadeRestanteTimespan = CapacidadePerfilTurnoParcial.TotalMinutes > 0 ? CapacidadePerfilTurnoParcial : TimeSpan.Zero,
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
                                retorno.Objeto.FindAll(p => p.PerfilAgendamento == groupPerfil.Descricao).Select(p =>
                                {
                                    CapacidadePerfilParcial = CapacidadePerfilParcial.Subtract(TimeSpan.Parse(p.HorasAgendadas) + TimeSpan.Parse(p.TempoDeslocamento));
                                    p.CapacidadeRestante = CapacidadePerfilParcial.TotalMinutes > 0 ? ((int)CapacidadePerfilParcial.TotalHours).ToString("D2") + CapacidadePerfilParcial.ToString(@"\:mm") : "00:00";
                                    return p;
                                }).ToList();

                                int capacidadeRestanteServicoTotalizada = 0;
                                TimeSpan capacidadeRestanteTimespanTotalizada = TimeSpan.Zero;
                                TimeSpan capacidadeHorasTimespanTotalizada = TimeSpan.Zero;
                                TimeSpan capacidadeRestanteTimespanCalculoTempoTotalizada = TimeSpan.Zero;

                                if (config.Considerar_Agendamento_Periodo)
                                {
                                    capacidadeRestanteServicoTotalizada = dadosPerfilTurno.Sum(p => Convert.ToInt16(p.capacidadeRestanteServicos));
                                    capacidadeRestanteTimespanTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeRestanteTimespan.Ticks));
                                    capacidadeRestanteTimespanCalculoTempoTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeRestanteCalculoTempoTimespan.Ticks));
                                    capacidadeHorasTimespanTotalizada = TimeSpan.FromTicks(dadosPerfilTurno.Sum(p => p.capacidadeHorasTimespan.Ticks));
                                }
                                else
                                {
                                    capacidadeRestanteTimespanTotalizada = CapacidadePerfilParcial.TotalMinutes > 0 ? CapacidadePerfilParcial : TimeSpan.Zero;
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
                                    int capacidadeServicosPerfilBaseadoEmTempo = Convert.ToInt32(Math.Floor(CapacidadePerfilParcialCalculoTempo.TotalMinutes / (tempoExecucao.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));
                                    if (capacidadeServicosPerfilBaseadoEmTempo < capacidadePerfilServicos)
                                    {
                                        capacidadePerfilServicos = capacidadeServicosPerfilBaseadoEmTempo;
                                    }

                                    if (capacidadePeriodoBaseadoEmTempoHoraisTotais < capacidadePerfilServicos)
                                    {
                                        capacidadePerfilServicos = capacidadePeriodoBaseadoEmTempoHoraisTotais;
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
                                    capacidadeRestanteCalculoTempoTimespan = capacidadeRestanteTimespanCalculoTempoTotalizada.TotalMinutes > 0 ? capacidadeRestanteTimespanCalculoTempoTotalizada : TimeSpan.Zero,
                                    capacidadeHoras = ((int)capacidadeHorasTimespanTotalizada.TotalHours).ToString("D2") + capacidadeHorasTimespanTotalizada.ToString(@"\:mm"),
                                    capacidadeHorasTimespan = capacidadeHorasTimespanTotalizada.TotalMinutes > 0 ? capacidadeHorasTimespanTotalizada : TimeSpan.Zero,
                                    forcaTrabalho = agendamentoPerfil.Count > 0 ? agendamentoPerfil.Max(p => p.Forca_Trabalho).ToString() : "0",
                                    capacidadeRestanteServicos = capacidadeRestanteServicoTotalizada.ToString(),
                                    capacidadeUtilHoras = ((int)CapacidadeUtilPerfil.TotalHours).ToString("D2") + CapacidadeUtilPerfil.ToString(@"\:mm"),
                                };

                            }).ToList();
                        }


                        TimeSpan capacidadeRestanteTimespan = TimeSpan.FromTicks(dadosPerfil.Sum(p => p.capacidadeRestanteTimespan.Ticks));
                        TimeSpan capacidadeRestanteCalculoTempoTimespan = TimeSpan.FromTicks(dadosPerfil.Sum(p => p.capacidadeRestanteCalculoTempoTimespan.Ticks));
                        string capacidadeRestante = capacidadeRestanteTimespan.TotalMinutes > 0 ? ((int)capacidadeRestanteTimespan.TotalHours).ToString("D2") + capacidadeRestanteTimespan.ToString(@"\:mm") : "00:00";//Capacidade.TotalMinutes > 0 ? ((int)Capacidade.TotalHours).ToString("D2") + Capacidade.ToString(@"\:mm") : "00:00";
                        long forcaTrabalho = agendamento?.Horas != null ? agendamento.Horas.GroupBy(p => p.IdPerfil).Select(p => p.ToList().Max(p => p.Forca_Trabalho)).Sum() : 0;
                        var capacidadeServicos = agendamento?.Horas != null ? agendamento.HorasCapacidade.Sum(p => p.Lista_Forca_Trabalho.Count) : 0;


                        if (dadosPerfil.Count > 0)
                        {
                            capacidadeServicos = dadosPerfil.Sum(p => Convert.ToInt32(p.capacidadeRestanteServicos));
                        }
                        else
                        {
                            int capacidadeServicosBaseadoEmTempo = Convert.ToInt32(Math.Floor(capacidadeRestanteCalculoTempoTimespan.TotalMinutes / (tempoExecucao.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));
                            if (capacidadeServicosBaseadoEmTempo < capacidadeServicos)
                            {
                                capacidadeServicos = capacidadeServicosBaseadoEmTempo;
                            }

                            if (capacidadeServicos < 0)
                                capacidadeServicos = 0;
                        }


                        TimeSpan totalLacunasTecnico = TimeSpan.Zero;
                        var dadosTecnico = retorno.Objeto.Select(p => ((String.IsNullOrEmpty(p.Tecnico) ? p.TecnicoTemporario : p.Tecnico), p.DataInicioAgendamento, p.DataFimAgendamento, p.Periodo)).
                                                          OrderBy(p => p.DataInicioAgendamento);

                        if (config.Considerar_Agendamento_Periodo)
                        {
                            foreach (var item in dadosTecnico.GroupBy(p => new { p.Item1, p.Periodo }))
                            {
                                var servicos = item.ToList();
                                for (int i = 0; i < servicos.Count; i++)
                                {
                                    if ((i + 1) < servicos.Count && servicos[i].DataFimAgendamento != servicos[i + 1].DataInicioAgendamento)
                                    {
                                        var lacuna = Convert.ToDateTime(servicos[i + 1].DataInicioAgendamento) - Convert.ToDateTime(servicos[i].DataFimAgendamento);
                                        totalLacunasTecnico = totalLacunasTecnico.Add(lacuna);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in dadosTecnico.GroupBy(p => new { p.Item1 }))
                            {
                                var servicos = item.ToList();
                                for (int i = 0; i < servicos.Count; i++)
                                {
                                    if ((i + 1) < servicos.Count && servicos[i].DataFimAgendamento != servicos[i + 1].DataInicioAgendamento)
                                    {
                                        var lacuna = Convert.ToDateTime(servicos[i + 1].DataInicioAgendamento) - Convert.ToDateTime(servicos[i].DataFimAgendamento);
                                        totalLacunasTecnico = totalLacunasTecnico.Add(lacuna);
                                    }
                                }
                            }
                        }

                        var retornoApi = await new Agendamento_EquipeApiController(1).GetDatasDisponiveis(request.Cod_Tipo_Serv, tipo_Servico_EquipeAniel.Cod_Natureza, request.Cod_Grupo_Area,
                                                                                                         request.Cod_Area, request.Cod_Localidade, request.LocalidadeStr, tempoExecucao.TotalMinutes, Convert.ToDateTime(request.Data));
                        var agendamentoDia = retornoApi.Objeto.Find(p => Convert.ToDateTime(p.Data).Date == Convert.ToDateTime(request.Data));

                        if(turno != null)
                        {
                            agendamentoDia.HorasTurno = agendamentoDia.HorasTurno.FindAll(p => p.Turno == turno.Descricao);
                        }

                        var capacidadeServicosAgendamentoDia = agendamentoDia != null ? Convert.ToString(agendamentoDia.HorasTurno.Sum(p => p.Aptos)) : "0";
                        var minutosAgendamentoDentroFaixa = agendamentoDia != null ? TimeSpan.FromMinutes(agendamentoDia.HorasTurno.Sum(p => p.MinutosAgendamentoDentroFaixa)) : TimeSpan.Zero;
                        var minutosAgendamentoForaFaixa = agendamentoDia != null ? TimeSpan.FromMinutes(agendamentoDia.HorasTurno.Sum(p => p.MinutosAgendamentoForaFaixa)) : TimeSpan.Zero;
                        var minutosCapacidadeRestante = agendamentoDia != null ? TimeSpan.FromMinutes(agendamentoDia.HorasTurno.Sum(p => p.MinutosCapacidadeRestante)) : TimeSpan.Zero;


                        return new DetalhamentoDisponibilidadeDataTableResponse<DetalhamentoDisponibilidadeListaModel>
                        {
                            data = retorno.Objeto,
                            capacidadeHoras = ((int)CapacidadeTotal.TotalHours).ToString("D2") + CapacidadeTotal.ToString(@"\:mm"),
                            capacidadeUtilHoras = ((int)CapacidadeUtilTotal.TotalHours).ToString("D2") + CapacidadeUtilTotal.ToString(@"\:mm"),
                            capacidadePlena = ((int)capacidadePlena.TotalHours).ToString("D2") + capacidadePlena.ToString(@"\:mm"),
                            capacidadeRestante = capacidadeRestante,
                            dadosPerfil = dadosPerfil,
                            tempoExecucaoTipoServico = ((int)tempoExecucao.TotalHours).ToString("D2") + tempoExecucao.ToString(@"\:mm"),
                            tempoDeslocamentoPadrao = config.Tempo_Medio_Deslocamento.HasValue ? ((int)config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalHours).ToString("D2") + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.ToString(@"\:mm") : TimeSpan.Zero.ToString(@"hh\:mm"),
                            qtdeTecnicosEscalados = forcaTrabalho.ToString(),
                            capacidadeRestanteServicos = capacidadeServicosAgendamentoDia,
                            tempoLacunaTecnico = ((int)totalLacunasTecnico.TotalHours).ToString("D2") + totalLacunasTecnico.ToString(@"\:mm"),
                            minutosAgendamentoDentroFaixa = ((int)minutosAgendamentoDentroFaixa.TotalHours).ToString("D2") + minutosAgendamentoDentroFaixa.ToString(@"\:mm"),
                            minutosAgendamentoForaFaixa = ((int)minutosAgendamentoForaFaixa.TotalHours).ToString("D2") + minutosAgendamentoForaFaixa.ToString(@"\:mm"),
                            minutosCapacidadeRestante = ((int)minutosCapacidadeRestante.TotalHours).ToString("D2") + minutosCapacidadeRestante.ToString(@"\:mm")
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                return new DetalhamentoDisponibilidadeDataTableResponse<DetalhamentoDisponibilidadeListaModel>
                {
                    error = ex.Message,
                    data = new List<DetalhamentoDisponibilidadeListaModel>()
                };
            }
        }

        public TimeSpan GetHorarioPleno(Turno_ServicoAniel turno, Horario_Connect horario, DateTime data)
        {
            TimeSpan capacidade = TimeSpan.Zero;
            using (var horarioApi = new HorarioApiController())
            {
                if (horario != null)
                {
                    var horarioDia = horarioApi.RetornaHorarioDia(horario, data, false);

                    if (turno == null)
                    {
                        var horasTrabalhadas = horarioApi.MinutosTrabalhadosHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                        capacidade = capacidade.Add(TimeSpan.FromMinutes(horasTrabalhadas));
                    }
                    else
                    {

                        var inicio = horarioDia.inicio != null ? horarioDia.inicio.Value.TimeOfDay.TotalHours : 0;
                        var fim = horarioDia.termino != null ? horarioDia.termino.Value.TimeOfDay.TotalHours : 0;
                        if (turno?.Hora_Ini != null && turno?.Hora_Fim != null)
                        {
                            if (inicio < turno.Hora_Ini.Value.TimeOfDay.TotalHours)
                            {
                                inicio = turno.Hora_Ini.Value.TimeOfDay.TotalHours;
                            }

                            if (fim > turno.Hora_Fim.Value.TimeOfDay.TotalHours)
                            {
                                fim = turno.Hora_Fim.Value.TimeOfDay.TotalHours;
                            }
                        }
                        var horas = fim > inicio ? fim - inicio : 0;
                        if (horarioDia.inicio_intervalo.HasValue && horarioDia.termino_intervalo.HasValue)
                        {
                            var inicioIntervaloDatetime = horarioDia.inicio_intervalo.Value;
                            var fimIntervaloDatetime = horarioDia.termino_intervalo.Value;
                            var inicioIntervalo = inicioIntervaloDatetime.TimeOfDay.TotalHours;
                            var fimIntervalo = fimIntervaloDatetime.TimeOfDay.TotalHours;

                            if (turno?.Hora_Ini != null && turno?.Hora_Fim != null)
                            {
                                if (inicioIntervalo >= turno.Hora_Ini.Value.TimeOfDay.TotalHours &&
                                    fimIntervalo >= turno.Hora_Ini.Value.TimeOfDay.TotalHours &&
                                    inicioIntervalo <= turno.Hora_Fim.Value.TimeOfDay.TotalHours &&
                                    fimIntervalo <= turno.Hora_Fim.Value.TimeOfDay.TotalHours)
                                {
                                    if (fimIntervalo > turno.Hora_Fim.Value.TimeOfDay.TotalHours)
                                    {
                                        fim = turno.Hora_Fim.Value.TimeOfDay.TotalHours;
                                    }

                                    horas -= fimIntervalo - inicioIntervalo;
                                }
                                else if (fimIntervalo > inicio && BetweenIgual(turno.Hora_Ini.Value, inicioIntervaloDatetime, fimIntervaloDatetime))
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

                        capacidade = capacidade.Add(TimeSpan.FromHours(horas));
                    }
                }

            }
            return capacidade;
        }

        public static bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }


        public async Task<List<AgendamentoDiaModel>> GetDatasValidasDetalhamento(ISession session, ISession sessionT5, Horario_Connect horario_Connect, List<Feriado> feriados,
            Int16? Cod_Tipo_Serv, Int32? Cod_Natureza, String Cod_Grupo_Area, String Cod_Area, Int32? Cod_Localidade, String LocalidadeStr, DateTime? TempoExecucaoServico, DateTime dataInicial, Int64? IdPeriodo, Int64? idPerfil, String Equipe = null)
        {
            var retorno = new List<AgendamentoDiaModel>();
            string usuario = User.Identity.GetUserId();
            var acessos = await (new AcessoApiController()).GetArea_ModuloUsuario(usuario);
            bool ignorarTempo = true;//AcessoHelper.TemAcesso(acessos.Objeto, "287");


            using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
            {
                //Pega as configurações da Empresa
                Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();
                var turnosBanco = repositoryTurno.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList();


                //if (config.Criterio_Area)
                //{
                //    if (String.IsNullOrEmpty(Cod_Grupo_Area) || String.IsNullOrEmpty(Cod_Area) || String.IsNullOrEmpty(LocalidadeStr))
                //    {
                //        throw new Exception("Serviço sem localidade vinculada.");
                //    }
                //}

                if (config.Criterio_Habilidade)
                {
                    if (Cod_Natureza.HasValue == false)
                    {
                        throw new Exception("Serviço sem natureza vinculada.");
                    }
                }

                DateTime? tempoExecucao = !TempoExecucaoServico.HasValue ? config.Tempo_Execucao_Padrao : TempoExecucaoServico;

                if (!tempoExecucao.HasValue || tempoExecucao.Value.TimeOfDay == new TimeSpan(0))
                {
                    throw new Exception("Tempo de Execução não definido no sistema");
                }
                if (!config.Tempo_Medio_Deslocamento.HasValue)
                {
                    throw new Exception("Tempo Medio de Deslocamento não definido no sistema");
                }

                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var horarioApiController = new HorarioApiController())
                using (var repositoryPerfilNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5))
                {
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();

                    Horario_Connect horario_servico_padrao = horarios_servico.Find(p => p.Padrao == true);

                    if (horario_servico_padrao == null)
                    {
                        throw new Exception("Horario de Serviço Padrão não definido no sistema");
                    }

                    Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);
                    Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));
                    List<AgendamentoDadosModel> jaAgendados = await GetJaAgendadosDetalhamento(sessionT5, dataInicial, dataInicial);

                    if (IdPeriodo.HasValue)
                        jaAgendados = jaAgendados.FindAll(p => p.DadosServico.IdPeriodo == IdPeriodo);


                    var perfisNaturezas = (await repositoryPerfilNatureza.select()).ToList();
                    var perfisLocalidades = GetLocalidadeRoteirizacaoByPerfil(sessionT5);
                    for (DateTime dia = dataInicial; dia <= dataInicial; dia = dia.AddDays(1))
                    {
                        DayOfWeek diaSemana = dia.DayOfWeek;
                        bool feriado = feriados.Find(p => p.Data == dia) != null;
                        List<AgendamentoEquipeHorarioModel> forcaTrabalho = await GetForcaTrabalho(sessionT5, dia, config, perfisNaturezas, perfisLocalidades, Equipe);

                        if (idPerfil.HasValue)
                            forcaTrabalho = forcaTrabalho.FindAll(p => idPerfil == p.IdPerfilAgendamento);

                        List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == dia).Select(p =>
                        {
                            AgendamentoEquipeHorarioModel forcaTrabalhoEquipe = forcaTrabalho.Find(y => y.Equipe == p.Equipe);
                            bool capaz = false;
                            if (forcaTrabalhoEquipe != null)
                            {
                                RoteirizacaoBusiness helper = new RoteirizacaoBusiness();
                                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                                {
                                    Cod_Escritorio = p.DadosServico.Cod_Escritorio,
                                    Cod_Area = p.DadosServico.Cod_Area,
                                    Cod_Localidade = p.DadosServico.Cod_Localidade,
                                    Cod_Natureza = p.DadosServico.Cod_Natureza,
                                    LocalidadeStr = p.DadosServico.LocalidadeStr
                                };


                                TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                                {
                                    Naturezas = forcaTrabalhoEquipe.Naturezas,
                                    Localidades = forcaTrabalhoEquipe.Localidades,
                                    Equipe = forcaTrabalhoEquipe.Equipe,

                                };

                                capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                                p.Capaz = capaz;
                            }

                            return p;
                        }).ToList();

                        ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5);
                        INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);

                        var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);

                        LimiteAgendamentoAniel limAgendData = null;

                        if (Cod_Natureza.HasValue)
                            limAgendData =
                                limiteAgendamentoBusiness
                                    .GetPercNaturezaByAreaData(
                                        Cod_Grupo_Area,
                                        Cod_Area,
                                        Cod_Natureza.Value,
                                        dia
                                    );

                        AgendamentoDiaModel resultadoDia = BuscarAgendamentoDia(dia,
                                                                                jaAgendadosDia,
                                                                                forcaTrabalho,
                                                                                horarios_servico,
                                                                                horario_Connect,
                                                                                config,
                                                                                feriado,
                                                                                new DadosServicoAgendamento
                                                                                {
                                                                                    Cod_Area = Cod_Area,
                                                                                    Cod_Escritorio = Cod_Grupo_Area,
                                                                                    Cod_Localidade = Cod_Localidade,
                                                                                    Cod_Natureza = Cod_Natureza,
                                                                                    LocalidadeStr = LocalidadeStr,
                                                                                    TempoExecucaoMinutos = tempoExecucao
                                                                                },
                                                                                turnosBanco,
                                                                                limAgendData,
                                                                                ignorarTempo,
                                                                                false);

                        if (resultadoDia != null)
                            resultadoDia.PercentualLimiteAgendamento = limAgendData?.Percentual;

                        retorno.Add(resultadoDia);
                    }




                    return retorno;
                }
            }
        }

        private Dictionary<long?, List<AgendamentoDadosModel>> AgendamentosCargaForcaTrabalho(List<AgendamentoDadosModel> jaAgendadosDia, List<AgendamentoEquipeHorarioModel> forcaTrabalho, Configuracoes_Gerais config, List<long?> perfis)
        {
            Dictionary<long?, List<AgendamentoDadosModel>> retorno = new Dictionary<long?, List<AgendamentoDadosModel>>();
            RoteirizacaoBusiness helper = new RoteirizacaoBusiness();


            var jaAgendadosDiaGrupo = new List<AgendamentoDadosModel>();
            //lista que serve para verificar possiveis agendamentos temporarios que devem ser somado a carga da equipe de trabalho
            foreach (var item in perfis)
            {
                var forcaTrabalhoPerfil = forcaTrabalho.FindAll(p => p.IdPerfilAgendamento == item);
                jaAgendadosDiaGrupo = jaAgendadosDia.FindAll(p => forcaTrabalhoPerfil.Any(y => y.Equipe == p.Equipe) && p.Temporario == false)
                                                .Concat(jaAgendadosDia.FindAll(p => forcaTrabalhoPerfil.Any(y => y.Equipe == p.Equipe) && p.Temporario && p.Capaz))
                                                .ToList();
                retorno.Add(item, jaAgendadosDiaGrupo);
            }

            forcaTrabalho = forcaTrabalho.FindAll(p => p.Disponivel);
            var jaAgendadosDiaForaForcaTrabalho = jaAgendadosDia.FindAll(p => (forcaTrabalho.Any(y => y.Equipe != p.Equipe) && p.Temporario && p.Capaz == false) || String.IsNullOrEmpty(p.Equipe));
            jaAgendadosDiaForaForcaTrabalho = jaAgendadosDiaForaForcaTrabalho.Select(p =>
            {
                Painel_ReteirizacaoDoc_Agrupado doc = new Painel_ReteirizacaoDoc_Agrupado
                {
                    Cod_Area = p.DadosServico.Cod_Area,
                    Cod_Escritorio = p.DadosServico.Cod_Escritorio,
                    Cod_Localidade = p.DadosServico.Cod_Localidade,
                    LocalidadeStr = p.DadosServico.LocalidadeStr,
                    Cod_Natureza = p.DadosServico.Cod_Natureza
                };

                var buscaAptidao = forcaTrabalho.Select(y =>
                {
                    TecnicoRoteirizar tecnico = new TecnicoRoteirizar
                    {
                        Naturezas = y.Naturezas,
                        Localidades = y.Localidades,
                        Equipe = y.Equipe,

                    };

                    var capaz = helper.VerificarTecnicoCapaz(config, tecnico, doc, null);
                    return (y.Equipe, capaz);

                }).ToList();

                var aptos = buscaAptidao.Where(y => y.capaz == true).ToList().ToDictionary(y => y.Equipe, y => y.capaz);

                if (aptos.Where(y => y.Value).Count() > 0)
                {

                    var idsPerfilAgendamento = forcaTrabalho.FindAll(z => aptos.Where(y => y.Value).Select(p => p.Key).Contains(z.Equipe)).
                                      Select(p => p.IdPerfilAgendamento).
                                      Distinct();

                    if (idsPerfilAgendamento.Count() == 1)
                    {
                        var idPerfil = idsPerfilAgendamento.First();
                        List<AgendamentoDadosModel> agendamentosPerfil;
                        if (retorno.TryGetValue(idPerfil, out agendamentosPerfil))
                        {
                            agendamentosPerfil.Add(p);
                        }

                        return p;
                    }
                    else
                    {
                        var TotalDuracaoPorPerfil = retorno.Where(y => idsPerfilAgendamento.Contains(y.Key)).ToList().Select(y =>
                        {
                            return (y.Key, (y.Value.Sum(z => z.DuracaoMinutos)));
                        });

                        var idPerfilComMenosTempoAgendado = TotalDuracaoPorPerfil.Aggregate((curMin, x) => x.Item2 < curMin.Item2 ? x : curMin).Key;
                        List<AgendamentoDadosModel> agendamentosPerfil;
                        if (retorno.TryGetValue(idPerfilComMenosTempoAgendado, out agendamentosPerfil))
                        {
                            agendamentosPerfil.Add(p);
                        }

                        return p;
                    }
                }
                else
                    return null;

            }).ToList();


            return retorno;
        }

        [HttpGet()]
        [Route("api/Agendamento_Equipeapi/GetVerificaAgendamento")]
        public async Task<PostApiRetorno<bool>> GetVerificaAgendamento(String num_obra, String codCt, String projeto, String num_Doc)
        {
            PostApiRetorno<bool> retorno = new PostApiRetorno<bool>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoAgenda = new Conexao<Agendamento_EquipeAniel>(session))
                {
                    Int64 numDoc = Convert.ToInt64(num_Doc);
                    var obj = await repoAgenda.selectFirst(x => x.Num_Obra == num_obra && x.CodCt == codCt && x.Projeto == projeto && x.Num_Doc == numDoc);
                    if (obj != null)
                    {
                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Agendamento_Equipeapi/GetLimitacao")]
        public DataTableResponse<GetLimitacaoResponse> GetLimitacao(GetLimitacaoRequest request)
        {
            var retorno = new List<GetLimitacaoResponse>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5, sessionT5Nh);
                IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5Nh);
                IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);
                var natureza = repoNatureza.GetAll().Where(p => p.Id == Convert.ToInt32(request.Natureza)).FirstOrDefault();

                List<LimiteAgendamentoAniel> limites = new List<LimiteAgendamentoAniel>();
                if (request.Areas != null)
                {
                    foreach (var item in request.Areas)
                    {
                        var limite = repoLimAgend.GetPercNaturezaByAreaData(item.grupoArea,
                                                                                 item.area,
                                                                                 Convert.ToInt32(request.Natureza),
                                                                                 Convert.ToDateTime(request.Data));


                        retorno.Add(new GetLimitacaoResponse
                        {
                            area = item.area,
                            grupoArea = item.grupoArea,
                            areaDescricao = repoArea.GetByCodigo(item.grupoArea, item.area)?.Descricao,
                            grupoAreaDescricao = repoGrupoArea.GetByCodigo(item.grupoArea)?.Descricao,
                            percentual = Convert.ToString(limite?.Percentual),
                            idNatureza = natureza.Id,
                            natureza = natureza?.Descricao
                        });

                    }
                }


                return new DataTableResponse<GetLimitacaoResponse>
                {
                    draw = request.draw,
                    data = retorno
                };
            }
        }

        [HttpPost]
        [Route("api/Agendamento_Equipeapi/GravarLimitacao")]
        public PostApiRetorno GravarLimitacao(GravarLimitacaoRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5, sessionT5Nh);
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    IGrupoAreaAnielRepository repoGrupoArea = new GrupoAreaAnielRepository(sessionT5Nh);

                    List<LimiteAgendamentoAniel> limites = new List<LimiteAgendamentoAniel>();

                    foreach (var item in request.Areas)
                    {
                        var limiteArea = repoLimAgend.GetPercNaturezaByAreaData(item.grupoArea,
                                                                        item.area,
                                                                        Convert.ToInt32(item.natureza),
                                                                        Convert.ToDateTime(request.Data));
                        if (limiteArea == null)
                        {
                            var limite = new LimiteAgendamentoAniel
                            {
                                CodArea = item.area,
                                CodEscritorio = item.grupoArea,
                                DataInicio = Convert.ToDateTime(request.Data),
                                DataFim = Convert.ToDateTime(request.Data),
                                IdNatureza = Convert.ToInt32(item.natureza),
                                Percentual = Convert.ToInt32(item.percentual)
                            };

                            repoLimAgend.Save(limite);
                        }
                        else if (limiteArea.Percentual != Convert.ToInt32(item.percentual))
                        {
                            limiteArea.Percentual = Convert.ToInt32(item.percentual);
                            repoLimAgend.Update(limiteArea);

                        }
                    }


                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Agendamento_Equipeapi/GetPlanejamentoCapacidade")]
        public DataTableResponse<PainelPlanejamentoCapacidadeModel> GetPlanejamentoCapacidade(PainelPlanejamentoCapacidadeRequest request)
        {
            var retorno = new List<PainelPlanejamentoCapacidadeModel>();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IAreaAnielRepository repoArea = new AreaAnielRepository(sessionT5Nh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5);
                    ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5, sessionT5Nh);
                    IPainelPlanejamentoCapacidadeAnielRepository repoPainel = new PainelPlanejamentoCapacidadeAnielRepository(sessionT5, sessionT5Nh);
                    IAgendamentoRepository repoAgendamento = new AgendamentoRepository(sessionT5Nh, sessionT5);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5Nh);
                    IConfiguracoesGeraisRepository repoConfiguracoesGerais = new ConfiguracoesGeraisRepository(sessionNh);
                    IHorarioAnielRepository repoHorario = new HorarioAnielRepository(sessionT5Nh);
                    ITurnoAnielRepository repoTurno = new TurnoAnielRepository(sessionT5Nh);

                    var painelPlanejamentoCapacidadeBusiness =
                                new PainelPlanejamentoCapacidadeAnielBusiness(
                                    repoPainel,
                                    repoLimAgend,
                                    repoDocProd,
                                    repoConfig,
                                    repoArea,
                                    repoAgendamento,
                                    repoNatureza,
                                    repoConfiguracoesGerais,
                                    repoHorario,
                                    repoTurno
                                );

                    return new DataTableResponse<PainelPlanejamentoCapacidadeModel>
                    {
                        draw = request.draw,
                        data = painelPlanejamentoCapacidadeBusiness.GetPainelPlanejamentoCapacidade(request)
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<PainelPlanejamentoCapacidadeModel>
                {
                    error = ex.Message
                };
            }
        }

    }
}