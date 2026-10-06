using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Agendamento;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.Horario;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.T5.Turno;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class AgendamentoAnielBusiness
    {


        private readonly IAgendamentoRepository _repoAgendamento;
        private readonly ILimiteAgendamentoAnielRepository _repoLimAgendamento;
        private readonly INaturezaAnielRepository _repoNatureza;
        private readonly IConfiguracoesGeraisRepository _repoConfiguracoesGerais;
        private readonly IHorarioAnielRepository _repoHorario;
        private readonly ITurnoAnielRepository _repoTurno;
        public AgendamentoAnielBusiness(IAgendamentoRepository repoAgendamento,
                                        ILimiteAgendamentoAnielRepository repoLimAgendamento,
                                        INaturezaAnielRepository repoNatureza,
                                        IConfiguracoesGeraisRepository repoConfiguracoesGerais,
                                        IHorarioAnielRepository repoHorario,
                                        ITurnoAnielRepository repoTurno)
        {
            this._repoAgendamento = repoAgendamento;
            this._repoLimAgendamento = repoLimAgendamento;
            this._repoNatureza = repoNatureza;
            this._repoConfiguracoesGerais = repoConfiguracoesGerais;
            this._repoHorario = repoHorario;
            this._repoTurno = repoTurno;
        }

        public AgendamentoDiaModel BuscarAgendamentoDia(
            DateTime dia,
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
                    p.Id_Horario_Connect = horariosServico.Find(y => y.Padrao)?.Id;
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

                var capaz = VerificarTecnicoCapaz(config, tecnico, doc, null);
                return (p.Equipe, capaz);

            }).ToDictionary(p => p.Equipe, p => p.capaz);

            forcaTrabalho = forcaTrabalho.FindAll(p => aptos[p.Equipe]);
            forcaTrabalho = forcaTrabalho.FindAll(p => p.Disponivel);
            var gruposForcaTrabalho = forcaTrabalho.GroupBy(p => new { p.Id_Horario_Connect, p.IdPerfilAgendamento });
            List<AgendamentoDadosModel> jaAgendadosDia = jaAgendados.FindAll(p => p.DataHoraInicio.Date == dia.Date).Select(p =>
            {
                AgendamentoEquipeHorarioModel forcaTrabalhoEquipe = forcaTrabalho.Find(y => y.Equipe == p.Equipe);
                bool capaz = false;
                if (forcaTrabalhoEquipe != null)
                {
                    Painel_ReteirizacaoDoc_Agrupado painel_ReteirizacaoDoc_Agrupado = new Painel_ReteirizacaoDoc_Agrupado
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

                    capaz = VerificarTecnicoCapaz(config, tecnico, doc, null);
                    p.Capaz = capaz;
                }

                return p;
            }).ToList();

            var horarioDiaAgendamento = RetornaHorarioDia(horarioAgendamento, dia, feriado);
            AgendamentoDiaModel configDoDiaAgendamento = GetConfigDoDia(diaSemana, dia, feriado, horarioAgendamento);

            if (configDoDiaAgendamento == null)
                return null;



            Dictionary<long?, List<AgendamentoDadosModel>> agendamentosPorPerfil = AgendamentosCargaForcaTrabalho(jaAgendadosDia.ToList(),
                                                                                                                 forcaTrabalho,
                                                                                                                 config,
                                                                                                                 forcaTrabalho.Select(p => p.IdPerfilAgendamento).Distinct().ToList());

            foreach (var item in gruposForcaTrabalho)
            {
                var forcaTrabalhoFiltrada = forcaTrabalho.Where(p => p.IdPerfilAgendamento == item.Key.IdPerfilAgendamento && p.Id_Horario_Connect == item.Key.Id_Horario_Connect).Select(p => p.Equipe);

                var horarioServico = horariosServico.Find(p => p.Id == item.Key.Id_Horario_Connect) ?? horariosServico.Find(p => p.Padrao);
                var horarioDiaServico = RetornaHorarioDia(horarioServico, dia, feriado);
                AgendamentoDiaModel configDoDia = GetConfigDoDia(diaSemana, dia, feriado, horarioServico);

                List<AgendamentoDadosModel> jaAgendadosDiaGrupo = null;

                if (item.Key.IdPerfilAgendamento != null)
                    jaAgendadosDiaGrupo = agendamentosPorPerfil[item.Key.IdPerfilAgendamento].Where(p => forcaTrabalhoFiltrada.Contains(p.Equipe)).ToList();
                else
                    continue;

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
                            false,
                            diaModel
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
                                true,
                                diaModel
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
                    var TodosServicosEncontradasEmFaixas = diaModel.Horas.SelectMany(p => p.Lista_Ja_Agendados).Select(p => p.DadosServico.NumObra).ToList();
                    var listaJaAgendadosForaFaixa = jaAgendadosDiaGrupo.FindAll(p => TodosServicosEncontradasEmFaixas.FirstOrDefault(y => y == p.DadosServico.NumObra) == null).Select(p =>
                    {
                        p.IdPerfil = item.Key.IdPerfilAgendamento;
                        return p;
                    });

                    diaModel.ListaJaAgendadosForaFaixa.AddRange(listaJaAgendadosForaFaixa);
                }


            }

            if (diaModel != null)
                diaModel.SomenteTecnicosMultiskillDisponiveis = !forcaTrabalhoTotal.Any(f => f.Naturezas.Count == 1);





            return diaModel;
        }


        private Dictionary<long?, List<AgendamentoDadosModel>> AgendamentosCargaForcaTrabalho(List<AgendamentoDadosModel> jaAgendadosDia, List<AgendamentoEquipeHorarioModel> forcaTrabalho, Configuracoes_Gerais config, List<long?> perfis)
        {
            Dictionary<long?, List<AgendamentoDadosModel>> retorno = new Dictionary<long?, List<AgendamentoDadosModel>>();



            var jaAgendadosDiaGrupo = new List<AgendamentoDadosModel>();
            //lista que serve para verificar possiveis agendamentos temporarios que devem ser somado a carga da equipe de trabalho
            foreach (var item in perfis)
            {
                if (item != null)
                {
                    var forcaTrabalhoPerfil = forcaTrabalho.FindAll(p => p.IdPerfilAgendamento == item);

                    //var perfilAtivado = _repoAgendamento.PerfilAgendamentoAtivado(item.Value).Result;

                    //if (!perfilAtivado)
                    //{
                    //    forcaTrabalho.RemoveAll(f => f.IdPerfilAgendamento == item);
                    //    continue;
                    //}

                    jaAgendadosDiaGrupo = jaAgendadosDia.FindAll(p => forcaTrabalhoPerfil.Any(y => y.Equipe == p.Equipe) && p.Temporario == false)
                                                    .Concat(jaAgendadosDia.FindAll(p => forcaTrabalhoPerfil.Any(y => y.Equipe == p.Equipe) && p.Temporario && p.Capaz))
                                                    .ToList();

                    retorno.Add(item, jaAgendadosDiaGrupo);
                }
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

                    var capaz = VerificarTecnicoCapaz(config, tecnico, doc, null);
                    return (y.Equipe, capaz);

                }).ToList();

                var aptos = buscaAptidao.Where(y => y.capaz == true).ToList().ToDictionary(y => y.Equipe, y => y.capaz);

                if (aptos.Where(y => y.Value).Count() > 0)
                {

                    var idsPerfilAgendamento = forcaTrabalho.FindAll(z => aptos.Where(y => y.Value).Select(y => y.Key).Contains(z.Equipe)).
                                      Select(y => y.IdPerfilAgendamento).
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

        public AgendamentoDiaModel GetConfigDoDia(DayOfWeek diaSemana, DateTime dia, bool feriado, Horario_Connect horario_Connect)
        {
            AgendamentoDiaModel configDoDia = null;


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
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Domingo, horario_Connect.Termino_Domingo,
                                    horario_Connect.Inicio_Intervalo_Domingo, horario_Connect.Termino_Intervalo_Domingo);
                            break;
                        }
                    case DayOfWeek.Monday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Segunda, horario_Connect.Termino_Segunda,
                                    horario_Connect.Inicio_Intervalo_Segunda, horario_Connect.Termino_Intervalo_Segunda);
                            break;
                        }
                    case DayOfWeek.Tuesday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Terca, horario_Connect.Termino_Terca,
                                    horario_Connect.Inicio_Intervalo_Terca, horario_Connect.Termino_Intervalo_Terca);
                            break;
                        }
                    case DayOfWeek.Wednesday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Quarta, horario_Connect.Termino_Quarta,
                                    horario_Connect.Inicio_Intervalo_Quarta, horario_Connect.Termino_Intervalo_Quarta);
                            break;
                        }
                    case DayOfWeek.Thursday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Quinta, horario_Connect.Termino_Quinta,
                                    horario_Connect.Inicio_Intervalo_Quinta, horario_Connect.Termino_Intervalo_Quinta);
                            break;
                        }
                    case DayOfWeek.Friday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Sexta, horario_Connect.Termino_Sexta,
                                    horario_Connect.Inicio_Intervalo_Sexta, horario_Connect.Termino_Intervalo_Sexta);
                            break;
                        }
                    case DayOfWeek.Saturday:
                        {
                            GetDatasValidas_Agendamento(ref configDoDia, diaSemana, dia, false, horario_Connect.Inicio_Sabado, horario_Connect.Termino_Sabado,
                                    horario_Connect.Inicio_Intervalo_Sabado, horario_Connect.Termino_Intervalo_Sabado);
                            break;
                        }
                }
            }
            return configDoDia;
        }

        public static bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            date1 = date1.Value.AddMinutes(1);
            date2 = date2.Value.AddMinutes(-1);
            return (input?.TimeOfDay > date1.Value.TimeOfDay && input.Value.TimeOfDay < date2.Value.TimeOfDay);
        }
        public static bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }
        public double MinutosTrabalhadosHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino)
        {
            double totalMinutos = 0;
            if (horario_inicio.HasValue && horario_termino.HasValue)
            {
                totalMinutos += (horario_termino.Value - horario_inicio.Value).TotalMinutes;

                if (horario_inicio_intervalo.HasValue && horario_termino_intervalo.HasValue)
                {
                    var minutosIntervalo = (horario_termino_intervalo.Value - horario_inicio_intervalo.Value).TotalMinutes;
                    totalMinutos -= minutosIntervalo;
                }
            }

            return totalMinutos;
        }

        public bool AptoHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino,
                                DateTime hora_inicio, DateTime? hora_fim)
        {
            if (!horario_inicio.HasValue || !horario_termino.HasValue)
                return false;

            if (hora_inicio.TimeOfDay < horario_inicio?.TimeOfDay || hora_fim?.TimeOfDay > horario_termino?.TimeOfDay)
            {
                return false;
            }

            if (horario_inicio_intervalo.HasValue && horario_termino_intervalo.HasValue)
            {

                if (Between(hora_inicio, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (Between(hora_fim, horario_inicio_intervalo, horario_termino_intervalo))
                {
                    return false;
                }
                if (hora_inicio.TimeOfDay < horario_inicio_intervalo?.TimeOfDay && hora_fim?.TimeOfDay > horario_inicio_intervalo?.TimeOfDay)
                    return false;

            }



            return true;
        }

        public (DateTime? inicio, DateTime? inicio_intervalo, DateTime? termino_intervalo, DateTime? termino) RetornaHorarioDia(Horario_Connect horario, DateTime date, bool feriado)
        {
            if (feriado)
                return (horario.Inicio_Feriado, horario.Inicio_Intervalo_Feriado, horario.Termino_Intervalo_Feriado, horario.Termino_Feriado);

            switch (date.Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return (horario.Inicio_Domingo, horario.Inicio_Intervalo_Domingo, horario.Termino_Intervalo_Domingo, horario.Termino_Domingo);
                case DayOfWeek.Monday:
                    return (horario.Inicio_Segunda, horario.Inicio_Intervalo_Segunda, horario.Termino_Intervalo_Segunda, horario.Termino_Segunda);
                case DayOfWeek.Tuesday:
                    return (horario.Inicio_Terca, horario.Inicio_Intervalo_Terca, horario.Termino_Intervalo_Terca, horario.Termino_Terca);
                case DayOfWeek.Wednesday:
                    return (horario.Inicio_Quarta, horario.Inicio_Intervalo_Quarta, horario.Termino_Intervalo_Quarta, horario.Termino_Quarta);
                case DayOfWeek.Thursday:
                    return (horario.Inicio_Quinta, horario.Inicio_Intervalo_Quinta, horario.Termino_Intervalo_Quinta, horario.Termino_Quinta);
                case DayOfWeek.Friday:
                    return (horario.Inicio_Sexta, horario.Inicio_Intervalo_Sexta, horario.Termino_Intervalo_Sexta, horario.Termino_Sexta);
                case DayOfWeek.Saturday:
                    return (horario.Inicio_Sabado, horario.Inicio_Intervalo_Sabado, horario.Termino_Intervalo_Sabado, horario.Termino_Sabado);
                default:
                    return (null, null, null, null);

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
           bool capacidade,
           AgendamentoDiaModel configDoDiaTotal
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
                        capacidade,
                        configDoDiaTotal
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
                        capacidade,
                        configDoDiaTotal
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
                        capacidade,
                        configDoDiaTotal
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
            bool capacidade,
            AgendamentoDiaModel configDoDiaTotal
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


                jaAgendadosDiaForcaTrabalho = jaAgendadosDiaForcaTrabalho.FindAll(p => BetweenIgual(p.DataHoraInicio, obj.Hora_Inicio, obj.Hora_Final.AddSeconds(-1))
                                                             || BetweenIgual(p.DataHoraFim, obj.Hora_Inicio.AddSeconds(1), obj.Hora_Final)
                                                             || BetweenIgual(obj.Hora_Inicio, p.DataHoraInicio, p.DataHoraFim.AddSeconds(-1))
                                                             || BetweenIgual(obj.Hora_Final, p.DataHoraInicio.AddSeconds(1), p.DataHoraFim)).ToList().ToList();

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
                limiteAgendamento,
                capacidade,
                turno,
                configDoDiaTotal
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
            LimiteAgendamentoAniel limiteAgendamento,
            bool capacidade,
            Turno_ServicoAniel turno,
            AgendamentoDiaModel configDoDiaTotal


        )
        {



            List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
            JaAgendados = JaAgendados.Concat(jaAgendadosDia.FindAll(p => BetweenIgual(p.DataHoraInicio, agendamentoDisponiveis.Hora_Inicio, agendamentoDisponiveis.Hora_Final.AddSeconds(-1))
                                                                     || BetweenIgual(p.DataHoraFim, agendamentoDisponiveis.Hora_Inicio.AddSeconds(1), agendamentoDisponiveis.Hora_Final)
                                                                     || BetweenIgual(agendamentoDisponiveis.Hora_Inicio, p.DataHoraInicio, p.DataHoraFim.AddSeconds(-1))
                                                                     || BetweenIgual(agendamentoDisponiveis.Hora_Final, p.DataHoraInicio.AddSeconds(1), p.DataHoraFim)).ToList()).ToList();


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
                var horario_dia = RetornaHorarioDia(horario_servico, dia, configDoDia.Feriado);

                var apto = AptoHorario(horario_dia.inicio, horario_dia.inicio_intervalo, horario_dia.termino_intervalo, horario_dia.termino,
                                    agendamentoDisponiveis.Hora_Inicio, agendamentoDisponiveis.Hora_Final);

                if (apto == false)
                    return null;
                else
                    return p;
            }).Where(p => p != null).ToList();

            #region Limite de agendamento para a natureza na área
            agendamentoDisponiveis.LimiteAgendamentoUltrapassado = false;
            double? percLimiteAgendamento = limiteAgendamento != null ? ((double)limiteAgendamento.Percentual) / 100 : (double?)null;
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
                                    p.DadosServico.IdPeriodo == turno.Id &&
                                    MatriculasTrabalhoArea.Contains(p.Equipe)
                                    )
                        .Sum(a => a.DuracaoMinutos);


                // Adicionando o tempo do serviço que queremos agendar ao total dos já agendados
                duracaoJaAgendadosTotalMinutos += (agendamentoDisponiveis.Hora_Final - agendamentoDisponiveis.Hora_Inicio).TotalMinutes;


                if (capacidade)
                {
                    foreach (var item in configDoDia.HorasCapacidade)
                    {
                        if (PeriodoBusiness.Between(item.Hora_Inicio, item.Hora_Inicio.Date.Add(turno.Hora_Ini.Value.TimeOfDay), item.Hora_Inicio.Date.Add(turno.Hora_Fim.Value.TimeOfDay)))
                        {
                            duracaoJaAgendadosTotalMinutos += item.ForcaTrabalhoMinutosPrevistos;
                        }
                    }

                    if (configDoDiaTotal != null)
                    {
                        foreach (var item in configDoDiaTotal.HorasCapacidade)
                        {
                            if (PeriodoBusiness.Between(item.Hora_Inicio, item.Hora_Inicio.Date.Add(turno.Hora_Ini.Value.TimeOfDay), item.Hora_Inicio.Date.Add(turno.Hora_Fim.Value.TimeOfDay)))
                            {
                                duracaoJaAgendadosTotalMinutos += item.ForcaTrabalhoMinutosPrevistos;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in configDoDia.Horas)
                    {
                        if (PeriodoBusiness.Between(item.Hora_Inicio, item.Hora_Inicio.Date.Add(turno.Hora_Ini.Value.TimeOfDay), item.Hora_Inicio.Date.Add(turno.Hora_Fim.Value.TimeOfDay)))
                        {
                            duracaoJaAgendadosTotalMinutos += item.ForcaTrabalhoMinutosPrevistos;
                        }
                    }

                    if (configDoDiaTotal != null)
                    {
                        foreach (var item in configDoDiaTotal.Horas)
                        {
                            if (PeriodoBusiness.Between(item.Hora_Inicio, item.Hora_Inicio.Date.Add(turno.Hora_Ini.Value.TimeOfDay), item.Hora_Inicio.Date.Add(turno.Hora_Fim.Value.TimeOfDay)))
                            {
                                duracaoJaAgendadosTotalMinutos += item.ForcaTrabalhoMinutosPrevistos;
                            }
                        }
                    }
                }


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

                    var horarioDia = RetornaHorarioDia(horarioServico, dia, configDoDia.Feriado);


                    AjustarParaTurno(ref horarioDia.inicio, ref horarioDia.inicio_intervalo, ref horarioDia.termino_intervalo, ref horarioDia.termino, turno);

                    capacidadeTotalMultiskillDiaMinutos += MinutosTrabalhadosHorario(
                        horarioDia.inicio,
                        horarioDia.inicio_intervalo,
                        horarioDia.termino_intervalo,
                        horarioDia.termino
                    );
                }



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
            return agendamentoDisponiveis;
        }

        public bool VerificarTecnicoCapaz(Configuracoes_Gerais config, TecnicoRoteirizar tecnico, Painel_ReteirizacaoDoc_Agrupado doc_Agrupado, Int32? nivelLocalidade)
        {

            //if (this.IgnorarCriteriosHabilidadeArea)
            //{
            //    return true;
            //}

            bool capaz_area = false;
            bool capaz_habilidade = false;
            var localidades = tecnico.Localidades.ToList();
            if (nivelLocalidade.HasValue)
            {
                localidades = localidades.Where(p => p.Nivel == nivelLocalidade).ToList();
            }


            foreach (var localidade in localidades)
            {
                if (localidade.Todas_Areas)
                {
                    if (localidade.Cod_Escritorio == doc_Agrupado.Cod_Escritorio)
                    {
                        capaz_area = true;
                        break;
                    }
                }
                
                if (localidade.Todas_Localidades)
                {
                    if ((localidade.Cod_Escritorio, localidade.Cod_Area) == (doc_Agrupado.Cod_Escritorio, doc_Agrupado.Cod_Area))
                    {
                        capaz_area = true;
                        break;
                    }
                }

                if ((localidade.Cod_Escritorio, localidade.Cod_Area, localidade.LocalidadeStr) == (doc_Agrupado.Cod_Escritorio, doc_Agrupado.Cod_Area, doc_Agrupado.LocalidadeStr))
                {
                    capaz_area = true;
                    break;
                }

            }
            if (tecnico.Naturezas.Contains(doc_Agrupado?.Cod_Natureza))
            {
                capaz_habilidade = true;
            }
            if (config.Criterio_Area && !capaz_area)
            {
                return false;
            }

            if (config.Criterio_Habilidade && !capaz_habilidade)
            {
                return false;
            }

            return true;
        }

        public async Task<List<AgendamentoDiaModel>> GetDatasValidasDetalhamento(Int32? Cod_Natureza,
                                                                                 String Cod_Grupo_Area,
                                                                                 String Cod_Area,
                                                                                 Int32? Cod_Localidade,
                                                                                 String LocalidadeStr,
                                                                                 DateTime? TempoExecucaoServico,
                                                                                 DateTime dataInicial,
                                                                                 Int64? IdPeriodo,
                                                                                 List<long?> idsPerfis,
                                                                                 bool limiteAgendamento,
                                                                                 bool? Exclusivo = null,
                                                                                 bool apenasAptos = true)
        {
            var retorno = new List<AgendamentoDiaModel>();
            bool ignorarTempo = true;

            Configuracoes_Gerais config = this._repoConfiguracoesGerais.GetConfiguracoesGerais();
            var turnosBanco = this._repoTurno.GetAll()
                                             .Where(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                                             .ToList();


            if (config.Criterio_Area)
            {
                if (String.IsNullOrEmpty(Cod_Grupo_Area) || String.IsNullOrEmpty(Cod_Area) /*|| String.IsNullOrEmpty(LocalidadeStr)*/)
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


            List<Horario_Connect> horarios_servico = this._repoHorario.GetAll()
                                                                       .Where(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico)
                                                                       .ToList();

            Horario_Connect horarioAgendamento = this._repoHorario.GetAll()
                                                                  .Where(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento)
                                                                  .FirstOrDefault();

            Horario_Connect horario_servico_padrao = horarios_servico.Find(p => p.Padrao == true);

            if (horario_servico_padrao == null)
            {
                throw new Exception("Horario de Serviço Padrão não definido no sistema");
            }

            Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);
            Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));
            List<AgendamentoDadosModel> jaAgendados = await _repoAgendamento.GetJaAgendadosDetalhamento(dataInicial, dataInicial);

            if (IdPeriodo.HasValue)
                jaAgendados = jaAgendados.FindAll(p => p.DadosServico.IdPeriodo == IdPeriodo);

            for (DateTime dia = dataInicial; dia <= dataInicial; dia = dia.AddDays(1))
            {
                DayOfWeek diaSemana = dia.DayOfWeek;

                //implementar consulta de feriado
                bool feriado = false; //feriados.Find(p => p.Data == dia) != null;
                List<AgendamentoEquipeHorarioModel> forcaTrabalho = await _repoAgendamento.GetForcaTrabalho(dia, config);

                if (Exclusivo.HasValue)
                {
                    if (Exclusivo.Value)
                        forcaTrabalho = forcaTrabalho.FindAll(p => p.Naturezas.Count <= 1 && p.IdPerfilAgendamento != null);
                    else
                        forcaTrabalho = forcaTrabalho.FindAll(p => p.Naturezas.Count > 1 && p.IdPerfilAgendamento != null);
                }

                if (idsPerfis != null && idsPerfis.Count > 0)
                    forcaTrabalho = forcaTrabalho.FindAll(p => idsPerfis.Contains(p.IdPerfilAgendamento));
                else
                    forcaTrabalho = new List<AgendamentoEquipeHorarioModel>(); //caso nao tenha perfil agendado/configurado, zera a forca de trabalho para nao retornar nada

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

                var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(this._repoLimAgendamento, this._repoNatureza);

                LimiteAgendamentoAniel limAgendData = null;

                if (Cod_Natureza.HasValue && limiteAgendamento)
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
                                                                        horarioAgendamento,
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
                    retorno.Add(resultadoDia);

            }

            if (retorno != null)
            {
                foreach (var agendamento in retorno)
                {
                    foreach (var item in agendamento.Horas)
                    {
                        item.Turno = Convert.ToString(PeriodoBusiness.BuscarTurno(turnosBanco, item.Hora_Inicio, TempoExecucaoServico.Value.TimeOfDay)?.Id);
                    }

                    foreach (var item in agendamento.HorasCapacidade)
                    {
                        item.Turno = Convert.ToString(PeriodoBusiness.BuscarTurno(turnosBanco, item.Hora_Inicio, TempoExecucaoServico.Value.TimeOfDay)?.Id);
                    }

                    //if (request.IdPeriodo.HasValue)
                    //{
                    //    agendamento.Horas = agendamento.Horas.FindAll(p => p.Turno == request.IdPeriodo.ToString());
                    //    agendamento.HorasCapacidade = agendamento.HorasCapacidade.FindAll(p => p.Turno == request.IdPeriodo.ToString());
                    //}

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


                    //if (request.IdPeriodo.HasValue)
                    //{
                    //    agendamento.ListaJaAgendadosForaFaixa = agendamento.ListaJaAgendadosForaFaixa.FindAll(y => y.DadosServico.IdPeriodo == request.IdPeriodo);
                    //}
                }

            }

            if (config.Considerar_Agendamento_Periodo)
            {
                foreach (var item in retorno.SelectMany(p => p.HorasCapacidade))
                {
                    item.Turno = PeriodoBusiness.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.Value.TimeOfDay)?.Descricao;
                }

                foreach (var item in retorno.SelectMany(p => p.Horas))
                {
                    item.Turno = PeriodoBusiness.BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.Value.TimeOfDay)?.Descricao;
                }

                retorno = GetTurnosTotais(retorno, turnosBanco, tempoDeslocamentoMinutos, tempoExecucao, apenasAptos).ToList();

                if (apenasAptos)
                {
                    retorno = retorno.FindAll(p => p.HorasTurno.FindAll(y => y.Aptos > 0).Count > 0);
                }
            }

            return retorno;
        }
        private IEnumerable<AgendamentoDiaModel> GetTurnosTotais(IEnumerable<AgendamentoDiaModel> retorno, IEnumerable<Turno_ServicoAniel> turnosBanco, double tempoDeslocamentoMinutos, DateTime? tempoExecucao, bool apenasAptos)
        {
            Double tempoExecucaoMinutos = (tempoExecucao.Value.Hour * 60) + (tempoExecucao.Value.Minute);
            foreach (var item in retorno)
            {
                var turnosTotal = item.Horas.GroupBy(p => new { p.Turno, p.IdPerfil });
                item.HorasTurnoTotal = turnosTotal.Select(p =>
                {
                    var forcaTrabalhoPerfilCapacidadeTecnicoTmp = item.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);
                    var forcaTrabalhoPerfilTmp = item.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);


                    var forcaTrabalho = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                    var forcaTrabalhoComLimite = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistosComLimite);
                    var listaNumOS = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                    var totalMinutosAgendados = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DuracaoMinutos).Sum();
                    var agendamentosForaFaixaNoMesmoTurnoMinutos = item.ListaJaAgendadosForaFaixa.FindAll(y => p.Key.IdPerfil == y.IdPerfil &&
                                                                       PeriodoBusiness.BuscarTurno(turnosBanco.ToList(), y.DataHoraInicio, tempoExecucao.Value.TimeOfDay)?.Descricao == p.Key.Turno)
                                                                       .Sum(y => y.DuracaoMinutos);
                    var agendamentosManuaisMinutos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y))
                                                               .Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(y => y.DuracaoMinutos);

                    if (forcaTrabalhoPerfilTmp.FindAll(y => y.Deslocamento == 0).Count > 0)
                    {
                        double minutosExtrasDeslocamento = 0;
                        foreach (var itemHorario in forcaTrabalhoPerfilTmp.FindAll(y => y.Deslocamento == 0))
                        {
                            minutosExtrasDeslocamento = itemHorario.Forca_Trabalho * tempoDeslocamentoMinutos;
                            forcaTrabalho += minutosExtrasDeslocamento;
                            forcaTrabalhoComLimite += minutosExtrasDeslocamento;
                        }
                    }

                    int capacidadePeriodoBaseadoEmTempo = Convert.ToInt32(Math.Floor((forcaTrabalho - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucaoMinutos + tempoDeslocamentoMinutos)));
                    int capacidadePeriodo = item.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(y => y.Lista_Forca_Trabalho.Count);

                    int capacidadePeriodoBaseadoEmTempoComLimite = Convert.ToInt32(Math.Floor((forcaTrabalhoComLimite - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucaoMinutos + tempoDeslocamentoMinutos)));
                    int capacidadePeriodoComLimite = item.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(y => y.ListaForcaTrabalhoComLimite.Count);

                    if (capacidadePeriodoBaseadoEmTempo < capacidadePeriodo)
                    {
                        capacidadePeriodo = capacidadePeriodoBaseadoEmTempo;
                    }

                    if (capacidadePeriodoBaseadoEmTempoComLimite < capacidadePeriodoComLimite)
                    {
                        capacidadePeriodoComLimite = capacidadePeriodoBaseadoEmTempoComLimite;
                    }

                    var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null).OrderBy(y => y.LimiteAgendamentoUltrapassado);
                    if (horariosEquipesTemp.FirstOrDefault() == null && apenasAptos)
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
                        LimiteAgendamentoUltrapassado = item.LimiteAgendamentoUltrapassadoDia,
                        JaAgendadosMinutosConsumidos = totalMinutosAgendados,
                        ForcaTrabalhoMinutosPrevistos = forcaTrabalho

                    };
                }).Where(p => (p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null) || (apenasAptos == false)).ToList();

            }

            foreach (var item in retorno)
            {
                var turnos = item.HorasTurno.GroupBy(p => new { p.Turno });
                item.HorasTurno = turnos.Select(p =>
                            {
                                var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null).OrderBy(y => y.LimiteAgendamentoUltrapassado);
                                var capacidadePeriodoComLimite = p.ToList().Sum(y => y.AptosPorLimite);

                                return new AgendamentoHorasDisponiveis
                                {
                                    Display = p.Key.Turno,
                                    Aptos = p.ToList().Sum(y => y.Aptos),
                                    Turno = p.Key.Turno,
                                    Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(y => y != null).FirstOrDefault(),
                                    EquipeTmpComLimite = p.ToList().Select(y => y.EquipeTmpComLimite).Where(y => y != null).FirstOrDefault(),
                                    Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                                    Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                                    Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                                    Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                                    Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                                    AptosPorLimite = (long)capacidadePeriodoComLimite,
                                    LimiteAgendamentoUltrapassado = item.LimiteAgendamentoUltrapassadoDia,
                                    JaAgendadosMinutosConsumidos = p.ToList().Sum(y => y.JaAgendadosMinutosConsumidos),
                                    ForcaTrabalhoMinutosPrevistos = p.ToList().Sum(y => y.ForcaTrabalhoMinutosPrevistos)
                                };

                            }).Where(p => (p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null) || (apenasAptos == false)).ToList();

                var turnosTotal = item.HorasTurnoTotal.GroupBy(p => new { p.Turno });
                item.HorasTurnoTotal = turnosTotal.Select(p =>
                {
                    var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null).OrderBy(y => y.LimiteAgendamentoUltrapassado);
                    var capacidadePeriodoComLimite = p.ToList().Sum(y => y.AptosPorLimite);

                    return new AgendamentoHorasDisponiveis
                    {
                        Display = p.Key.Turno,
                        Aptos = p.ToList().Sum(y => y.Aptos),
                        Turno = p.Key.Turno,
                        Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(y => y != null).FirstOrDefault(),
                        EquipeTmpComLimite = p.ToList().Select(y => y.EquipeTmpComLimite).Where(y => y != null).FirstOrDefault(),
                        Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                        Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                        Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                        Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                        Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                        AptosPorLimite = (long)capacidadePeriodoComLimite,
                        LimiteAgendamentoUltrapassado = item.LimiteAgendamentoUltrapassadoDia,
                        JaAgendadosMinutosConsumidos = p.ToList().Sum(y => y.JaAgendadosMinutosConsumidos),
                        ForcaTrabalhoMinutosPrevistos = p.ToList().Sum(y => y.ForcaTrabalhoMinutosPrevistos)
                    };

                }).Where(p => (p != null && p.Aptos > 0 && p.Display != null && p.Equipe_Tmp != null) || (apenasAptos == false)).ToList();
            }
            return retorno;
        }

        public void AjustarParaTurno(
    ref DateTime? inicio,
    ref DateTime? inicio_intervalo,
    ref DateTime? termino_intervalo,
    ref DateTime? termino,
    Turno_ServicoAniel turno)
        {
            if (turno.Hora_Ini == null || turno.Hora_Fim == null)
                return;

            DateTime horaIniTurno = inicio.Value.Date.Add(turno.Hora_Ini.Value.TimeOfDay);
            DateTime horaFimTurno = termino.Value.Date.Add(turno.Hora_Fim.Value.TimeOfDay);

            // Ajusta inicio
            if (inicio.HasValue)
            {
                if (inicio.Value < horaIniTurno) inicio = horaIniTurno;
                if (inicio.Value > horaFimTurno) inicio = horaFimTurno;
            }

            // Ajusta inicio intervalo
            if (inicio_intervalo.HasValue)
            {
                if (inicio_intervalo.Value < horaIniTurno) inicio_intervalo = horaIniTurno;
                if (inicio_intervalo.Value > horaFimTurno) inicio_intervalo = horaFimTurno;
            }

            // Ajusta termino intervalo
            if (termino_intervalo.HasValue)
            {
                if (termino_intervalo.Value < horaIniTurno) termino_intervalo = horaIniTurno;
                if (termino_intervalo.Value > horaFimTurno) termino_intervalo = horaFimTurno;
            }

            // Ajusta termino
            if (termino.HasValue)
            {
                if (termino.Value < horaIniTurno) termino = horaIniTurno;
                if (termino.Value > horaFimTurno) termino = horaFimTurno;
            }
        }
    }
}
