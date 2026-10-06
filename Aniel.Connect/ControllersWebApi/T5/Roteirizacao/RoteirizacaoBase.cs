using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.SignalR;
using NHibernate.Linq;
using Resources;
using Sinapse.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class RoteirizacaoBase
    {

        public DateTime HoraRoteirizacao { get; set; }
        public bool IgnorarCriteriosHabilidadeArea { get; set; }
        public bool ManterOrdemRota { get; set; }
        public RoteirizacaoBase() { }

        /// <summary>
        /// agrupa os documentos baseados nos criterios ativos nas configuracoes gerais da empresa
        /// </summary>
        /// <param name="config"></param>
        /// <param name="documentos"></param>
        /// <returns></returns>
        public IEnumerable<Painel_ReteirizacaoDoc_Agrupado> AgrupaDocumentosCriterio(Configuracoes_Gerais config, List<DocumentoRoteirizar> documentos)
        {

            var retorno = new List<Painel_ReteirizacaoDoc_Agrupado>();
            if (config.Criterio_Area && config.Criterio_Habilidade)
            {
                return from o in documentos
                       group o by new { o.Id_Natureza, o.Grupo_Area, o.Area, o.Localidade } into g
                       select new Painel_ReteirizacaoDoc_Agrupado
                       {
                           Cod_Natureza = g.Key.Id_Natureza,
                           Cod_Escritorio = g.Key.Grupo_Area,
                           Cod_Area = g.Key.Area,
                           LocalidadeStr = g.Key.Localidade
                       };
            }

            else if (config.Criterio_Area)
            {
                return from o in documentos
                       group o by new { o.Grupo_Area, o.Area, o.Localidade } into g
                       select new Painel_ReteirizacaoDoc_Agrupado
                       {
                           Cod_Escritorio = g.Key.Grupo_Area,
                           Cod_Area = g.Key.Area,
                           LocalidadeStr = g.Key.Localidade
                       };
            }

            else if (config.Criterio_Habilidade)
                return from o in documentos
                       group o by new { o.Id_Natureza } into g
                       select new Painel_ReteirizacaoDoc_Agrupado
                       {
                           Cod_Natureza = g.Key.Id_Natureza
                       };


            return new List<Painel_ReteirizacaoDoc_Agrupado>();

        }

        /// <summary>
        /// retorna o documento com menor distancia em relação a latitude e longitude de referencia
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="documentos"></param>
        /// <returns></returns>
        public DocumentoRoteirizar GetDocumentoMaisProximo(Double? Latitude, Double? Longitude, List<DocumentoRoteirizar> documentos)
        {
            if (documentos.Count() == 0)
                return null;

            var distancias = GetDistancias(Latitude, Longitude, documentos);

            return distancias.OrderBy(p => p.Value).FirstOrDefault().Key;
        }

        /// <summary>
        /// retorna dicionario com a distancia para cada documento
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="documentos"></param>
        /// <returns></returns>
        public Dictionary<DocumentoRoteirizar, Double> GetDistancias(Double? Latitude, Double? Longitude, List<DocumentoRoteirizar> documentos)
        {
            if (documentos.Count() == 0)
                return new Dictionary<DocumentoRoteirizar, double>();

            Dictionary<DocumentoRoteirizar, Double> distancias = new Dictionary<DocumentoRoteirizar, Double>();

            foreach (DocumentoRoteirizar documento in documentos)
            {
                double? latitudeReferenciaDocumento = documento.GetLatitude();
                double? longitudeReferenciaDocumento = documento.GetLongitude();

                if (latitudeReferenciaDocumento == null)
                    latitudeReferenciaDocumento = documento.Latitude_Localidade;

                if (longitudeReferenciaDocumento == null)
                    longitudeReferenciaDocumento = documento.Longitude_Localidade;


                Double distancia = 0;

                if (latitudeReferenciaDocumento == null || longitudeReferenciaDocumento == null || Latitude == null || Longitude == null)
                    distancia = double.MaxValue;
                else
                    distancia = MapaService.GetMenorDistancia(Latitude, Longitude, latitudeReferenciaDocumento, longitudeReferenciaDocumento);

                if (!distancias.ContainsKey(documento))
                    distancias.Add(documento, distancia);
            }

            return distancias;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempoExecucaoMinutos"></param>
        /// <param name="tempoDeslocamentoMinutos"></param>
        /// <param name="configDoDia"></param>
        /// <param name="JaAgendados"></param>
        /// <returns></returns>
        public List<AgendamentoHorasDisponiveis> GetHorarioEquipeDisponivel(Configuracoes_Gerais config_gerais, DocumentoRoteirizar documento, Double tempoDeslocamentoMinutos,
                                                                            AgendamentoDiaModel configDoDia, List<Painel_Servicos_AgendamentosEquipe> JaAgendadosTecnico, List<Painel_Servicos_AgendamentosEquipe> JaAgendados,
                                                                            List<Painel_Servicos_AgendamentosEquipe> listaAusencia)
        {

            Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;

            if (configDoDia == null)
                return null;

            if (documento.CodTurnoServ.HasValue &&
                config_gerais.Considerar_Agendamento_Periodo &&
                documento.TurnoHoraIni.HasValue &&
                documento.TurnoHoraFim.HasValue &&
                documento.Data_hora_ini_agendamento.HasValue &&
                documento.Data_hora_fim_agendamento.HasValue)
            {
                GetHorasDisponiveisEquipe(documento, config_gerais, configDoDia, tempoExecucaoMinutos, tempoDeslocamentoMinutos, JaAgendadosTecnico, JaAgendados, listaAusencia);
                var horas = configDoDia.Horas.FindAll(p => p.Hora_Inicio >= this.HoraRoteirizacao);
                return horas.FindAll(p => PeriodoApiController.Between(p.Hora_Inicio, documento.TurnoHoraIni, documento.TurnoHoraFim));
            }


            else if (documento.Data_hora_ini_agendamento.HasValue && documento.Data_hora_fim_agendamento.HasValue)
            {

                var query_ja_agendado = JaAgendadosTecnico.Find(p => ((documento.Data_hora_ini_agendamento >= p.Data_Inicial && documento.Data_hora_ini_agendamento < p.Data_Final) ||
                                                            (documento.Data_hora_fim_agendamento > p.Data_Inicial && documento.Data_hora_fim_agendamento < p.Data_Final)));

                var query_ausente = listaAusencia.Find(p => ((documento.Data_hora_ini_agendamento >= p.Data_Inicial && documento.Data_hora_ini_agendamento < p.Data_Final) ||
                                            (documento.Data_hora_fim_agendamento > p.Data_Inicial && documento.Data_hora_fim_agendamento < p.Data_Final)));

                if (query_ja_agendado == null && query_ausente == null)
                {

                    var horarioapi = new HorarioApiController();
                    var apto = horarioapi.AptoHorario(configDoDia.HoraInicio, configDoDia.HoraInicioIntervalo, configDoDia.HoraTerminoIntervalo, configDoDia.HoraTermino,
                                                documento.Data_hora_ini_agendamento.Value, documento.Data_hora_fim_agendamento.Value);

                    if (apto)
                    {
                        configDoDia.Horas.Add(new AgendamentoHorasDisponiveis
                        {
                            Display = documento.Data_hora_ini_agendamento.Value.ToString("HH:mm"),
                            Hora_Inicio = documento.Data_hora_ini_agendamento.Value,
                            Hora_Final = documento.Data_hora_ini_agendamento.Value.AddMinutes(tempoExecucaoMinutos + (configDoDia.HoraInicio != documento.Data_hora_ini_agendamento.Value ? documento.DeslocamentoMinutos : 0)),
                            Deslocamento = (configDoDia.HoraInicio != documento.Data_hora_ini_agendamento.Value ? documento.DeslocamentoMinutos : 0),
                            AgendamentoAntigo = true,
                        });
                    }
                }

            }
            else
            {
                GetHorasDisponiveisEquipe(documento, config_gerais, configDoDia, tempoExecucaoMinutos, tempoDeslocamentoMinutos, JaAgendadosTecnico, JaAgendados, listaAusencia);
            }

            //Pega apenas os horarios maiores que o hora atual
            return configDoDia.Horas.FindAll(p => p.Hora_Inicio >= this.HoraRoteirizacao);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configDoDia"></param>
        /// <param name="tempoExecucaoMinutos"></param>
        /// <param name="tempoDeslocamento"></param>
        /// <param name="ja_agendados"></param>
        public void GetHorasDisponiveisEquipe(DocumentoRoteirizar documento, Configuracoes_Gerais config_gerais, AgendamentoDiaModel configDoDia, Double tempoExecucaoMinutos, Double tempoDeslocamento,
                                               List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos, List<Painel_Servicos_AgendamentosEquipe> ja_agendados, List<Painel_Servicos_AgendamentosEquipe> listaAusencia)
        {

            var inicio = new DateTime(configDoDia.HoraInicio.Value.Ticks); //HorarioReferencia.HasValue ? configDoDia.HoraInicio.Value.Date.Add(HorarioReferencia.Value) : 
            var termino = new DateTime(configDoDia.HoraTermino.Value.Ticks);

            if (documento.CodTurnoServ.HasValue &&
                config_gerais.Considerar_Agendamento_Periodo &&
                documento.TurnoHoraIni.HasValue &&
                documento.TurnoHoraFim.HasValue &&
                documento.Data_hora_fim_agendamento.HasValue)
            {
                if (ja_agendadosTecnicos.Find(p => p.Data_Final.Value.TimeOfDay > documento.TurnoHoraIni.Value.TimeOfDay) == null &&
                    PeriodoApiController.Between(inicio.Date.Add(documento.TurnoHoraIni.Value.TimeOfDay), inicio, termino))
                {
                    inicio = inicio.Date.Add(documento.TurnoHoraIni.Value.TimeOfDay);
                }
            }


            if (ja_agendadosTecnicos.Where(p => p.Intervalo).Count() == 0)
            {
                for (DateTime i = inicio; i <= termino;)//i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    if (this.ManterOrdemRota == true || Convert.ToDateTime(configDoDia.Data).Date == this.HoraRoteirizacao.Date)
                    {
                        if (i < this.HoraRoteirizacao)
                            i = this.HoraRoteirizacao;
                    }


                    var tempoDeslocLooping = tempoDeslocamento;

                    if (documento.TurnoHoraIni != null)
                    {
                        if (i.TimeOfDay == documento.TurnoHoraIni.Value.TimeOfDay && config_gerais.DesconsiderarDeslocamento)
                        {
                            tempoDeslocLooping = 0;
                        }
                    }

                    i = GetHorasDisponiveisEquipe_Calc(configDoDia, tempoExecucaoMinutos, tempoDeslocLooping, i, ja_agendadosTecnicos, ja_agendados, listaAusencia);

                    if (documento.CodTurnoServ.HasValue &&
                        config_gerais.Considerar_Agendamento_Periodo &&
                        documento.TurnoHoraIni.HasValue &&
                        documento.TurnoHoraFim.HasValue &&
                        documento.Data_hora_fim_agendamento.HasValue)
                    {
                        var horas = configDoDia.Horas.FindAll(p => p.Hora_Inicio >= this.HoraRoteirizacao);
                        if (horas.FindAll(p => PeriodoApiController.Between(p.Hora_Inicio, documento.TurnoHoraIni, documento.TurnoHoraFim)).Count > 0)
                            break;
                    }
                    else if (configDoDia.Horas.Count(p => p.Hora_Inicio >= this.HoraRoteirizacao) > 0) break;
                }
            }
            else
            {

                var intervalo = ja_agendadosTecnicos.Where(p => p.Intervalo).First();

                for (DateTime i = inicio; i <= intervalo.Data_Inicial;) //i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    if (this.ManterOrdemRota == true || Convert.ToDateTime(configDoDia.Data).Date == this.HoraRoteirizacao.Date)
                    {
                        if (i < this.HoraRoteirizacao)
                            i = this.HoraRoteirizacao;
                    }

                    var tempoDeslocLooping = tempoDeslocamento;
                    if (documento.TurnoHoraIni != null)
                    {
                        if (i.TimeOfDay == documento.TurnoHoraIni.Value.TimeOfDay && config_gerais.DesconsiderarDeslocamento)
                        {
                            tempoDeslocLooping = 0;
                        }
                    }

                    i = GetHorasDisponiveisEquipe_Calc(configDoDia, tempoExecucaoMinutos, tempoDeslocLooping, i, ja_agendadosTecnicos, ja_agendados, listaAusencia);
                    if (documento.CodTurnoServ.HasValue &&
                        config_gerais.Considerar_Agendamento_Periodo &&
                        documento.TurnoHoraIni.HasValue &&
                        documento.TurnoHoraFim.HasValue &&
                        documento.Data_hora_fim_agendamento.HasValue)
                    {
                        var horas = configDoDia.Horas.FindAll(p => p.Hora_Inicio >= this.HoraRoteirizacao);
                        if (horas.FindAll(p => PeriodoApiController.Between(p.Hora_Inicio, documento.TurnoHoraIni, documento.TurnoHoraFim)).Count > 0)
                            break;
                    }
                    else if (configDoDia.Horas.Count(p => p.Hora_Inicio >= this.HoraRoteirizacao) > 0) break;
                }
                for (DateTime i = intervalo.Data_Final.Value; i <= termino;) //i = i.AddMinutes(tempoExecucaoMinutos)
                {
                    if (this.ManterOrdemRota == true || Convert.ToDateTime(configDoDia.Data).Date == this.HoraRoteirizacao.Date)
                    {
                        if (i < this.HoraRoteirizacao)
                            i = this.HoraRoteirizacao;
                    }

                    var tempoDeslocLooping = tempoDeslocamento;
                    if (documento.TurnoHoraIni != null)
                    {
                        if (i.TimeOfDay == documento.TurnoHoraIni.Value.TimeOfDay && config_gerais.DesconsiderarDeslocamento)
                        {
                            tempoDeslocLooping = 0;
                        }
                    }


                    i = GetHorasDisponiveisEquipe_Calc(configDoDia, tempoExecucaoMinutos, tempoDeslocLooping, i, ja_agendadosTecnicos, ja_agendados, listaAusencia);
                    if (documento.CodTurnoServ.HasValue && config_gerais.Considerar_Agendamento_Periodo && documento.TurnoHoraIni.HasValue && documento.TurnoHoraFim.HasValue && documento.Data_hora_fim_agendamento.HasValue)
                    {
                        var horas = configDoDia.Horas.FindAll(p => p.Hora_Inicio >= this.HoraRoteirizacao);
                        if (horas.FindAll(p => PeriodoApiController.Between(p.Hora_Inicio, documento.TurnoHoraIni, documento.TurnoHoraFim)).Count > 0)
                            break;
                    }
                    else if (configDoDia.Horas.Count(p => p.Hora_Inicio >= this.HoraRoteirizacao) > 0) break;
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dia"></param>
        /// <param name="feriado"></param>
        /// <param name="horario_Connect"></param>
        /// <returns></returns>
        public AgendamentoDiaModel CarregaConfiguracoesDoDocumento(DateTime dia, bool feriado, Horario_Connect horario_Connect, String equipe)
        {
            AgendamentoDiaModel configDoDia = new AgendamentoDiaModel();
            /*******************/
            DayOfWeek diaSemana = dia.DayOfWeek;

            if (feriado)
            {
                configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, true, horario_Connect.Inicio_Feriado, horario_Connect.Termino_Feriado,
                        horario_Connect.Inicio_Intervalo_Feriado, horario_Connect.Termino_Intervalo_Feriado);
            }
            else
            {
                switch (diaSemana)
                {
                    case DayOfWeek.Sunday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Domingo, horario_Connect.Termino_Domingo,
                                    horario_Connect.Inicio_Intervalo_Domingo, horario_Connect.Termino_Intervalo_Domingo);
                            break;
                        }
                    case DayOfWeek.Monday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Segunda, horario_Connect.Termino_Segunda,
                                    horario_Connect.Inicio_Intervalo_Segunda, horario_Connect.Termino_Intervalo_Segunda);
                            break;
                        }
                    case DayOfWeek.Tuesday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Terca, horario_Connect.Termino_Terca,
                                    horario_Connect.Inicio_Intervalo_Terca, horario_Connect.Termino_Intervalo_Terca);
                            break;
                        }
                    case DayOfWeek.Wednesday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Quarta, horario_Connect.Termino_Quarta,
                                    horario_Connect.Inicio_Intervalo_Quarta, horario_Connect.Termino_Intervalo_Quarta);
                            break;
                        }
                    case DayOfWeek.Thursday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Quinta, horario_Connect.Termino_Quinta,
                                    horario_Connect.Inicio_Intervalo_Quinta, horario_Connect.Termino_Intervalo_Quinta);
                            break;
                        }
                    case DayOfWeek.Friday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Sexta, horario_Connect.Termino_Sexta,
                                    horario_Connect.Inicio_Intervalo_Sexta, horario_Connect.Termino_Intervalo_Sexta);
                            break;
                        }
                    case DayOfWeek.Saturday:
                        {
                            configDoDia = GetDatasValidas_Agendamento(diaSemana, dia, false, horario_Connect.Inicio_Sabado, horario_Connect.Termino_Sabado,
                                    horario_Connect.Inicio_Intervalo_Sabado, horario_Connect.Termino_Intervalo_Sabado);
                            break;
                        }
                }
            }

            if (configDoDia != null)
                configDoDia.Equipe = equipe;

            return configDoDia;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diaSemana"></param>
        /// <param name="dia"></param>
        /// <param name="feriado"></param>
        /// <param name="inicio"></param>
        /// <param name="termino"></param>
        /// <param name="inicio_intervalo"></param>
        /// <param name="termino_intervalo"></param>
        /// <returns></returns>
        public AgendamentoDiaModel GetDatasValidas_Agendamento(DayOfWeek diaSemana, DateTime dia, Boolean feriado, DateTime? inicio, DateTime? termino, DateTime? inicio_intervalo, DateTime? termino_intervalo)
        {
            if (inicio.HasValue)
            {
                DateTime diaTermino = dia.Date;
                DateTime diaInicioIntervalo = dia.Date;
                DateTime diaTerminoIntervalo = dia.Date;

                if (termino.Value.TimeOfDay < inicio.Value.TimeOfDay)
                {
                    diaTermino = diaTermino.AddDays(1);
                }

                if (inicio_intervalo.HasValue)
                {
                    if (inicio_intervalo.Value.TimeOfDay < inicio.Value.TimeOfDay)
                    {
                        diaInicioIntervalo = diaInicioIntervalo.AddDays(1);
                    }
                }

                if (termino_intervalo.HasValue)
                {
                    if (termino_intervalo.Value.TimeOfDay < inicio.Value.TimeOfDay)
                    {
                        diaTerminoIntervalo = diaTerminoIntervalo.AddDays(1);
                    }
                }

                return new AgendamentoDiaModel
                {
                    DiaDaSemana = diaSemana,
                    Data = dia.ToString("dd/MM/yyyy"),
                    Feriado = feriado,
                    HoraInicio = dia.AddMinutes(inicio.Value.TimeOfDay.TotalMinutes),
                    HoraTermino = diaTermino.AddMinutes(termino.Value.TimeOfDay.TotalMinutes),
                    HoraInicioIntervalo = inicio_intervalo.HasValue ? diaInicioIntervalo.AddMinutes(inicio_intervalo.Value.TimeOfDay.TotalMinutes) : (DateTime?)null,
                    HoraTerminoIntervalo = termino_intervalo.HasValue ? diaTerminoIntervalo.AddMinutes(termino_intervalo.Value.TimeOfDay.TotalMinutes) : (DateTime?)null
                };
            }
            else
            {
                if (feriado)
                {
                    return null;//throw new Exception(RsCadastros.NAO_EXISTE_HORARIOS_CONFIGURADOS_FERIADOS);
                }
                else
                {
                    return null;//throw new Exception(RsCadastros.NAO_EXISTE_HORARIOS_CONFIGURADOS_UTEIS);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configDoDia"></param>
        /// <param name="tempoExecucaoMinutos"></param>
        /// <param name="tempoDeslocamento"></param>
        /// <param name="hora"></param>
        /// <param name="ja_agendados"></param>
        /// <returns></returns>
        public DateTime GetHorasDisponiveisEquipe_Calc(AgendamentoDiaModel configDoDia, Double tempoExecucaoMinutos, Double tempoDeslocamento,
                                                    DateTime hora, List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos, List<Painel_Servicos_AgendamentosEquipe> ja_agendados,
                                                    List<Painel_Servicos_AgendamentosEquipe> listaAusencia)
        {

            AgendamentoHorasDisponiveis obj = new AgendamentoHorasDisponiveis
            {
                Display = hora.ToString("HH:mm"),
                Hora_Inicio = hora,
                Hora_Final = hora.AddMinutes(tempoExecucaoMinutos + (this.ManterOrdemRota ? tempoDeslocamento : (configDoDia.HoraInicio != hora ? tempoDeslocamento : 0))),
                Deslocamento = (this.ManterOrdemRota ? tempoDeslocamento : (configDoDia.HoraInicio != hora ? tempoDeslocamento : 0))
            };

            if (configDoDia.HoraInicioIntervalo.HasValue && this.ManterOrdemRota == false)
            {
                var agendamento_no_almoco = ja_agendadosTecnicos.Find(p => ((obj.Hora_Inicio >= p.Data_Inicial && obj.Hora_Inicio < p.Data_Final) ||
                                                            (obj.Hora_Final > p.Data_Inicial && obj.Hora_Final < p.Data_Final)));
                if (obj.Hora_Final >= configDoDia.HoraInicioIntervalo &&
                    obj.Hora_Inicio < configDoDia.HoraInicioIntervalo)
                {
                    if (agendamento_no_almoco == null && ja_agendadosTecnicos.Where(p => p.Intervalo).Count() == 0)
                    {
                        if (obj.Hora_Inicio >= this.HoraRoteirizacao)
                        {
                            var intervalo = new Painel_Servicos_AgendamentosEquipe
                            {
                                Data_Inicial = obj.Hora_Final,
                                Data_Final = obj.Hora_Final.Add(configDoDia.HoraTerminoIntervalo.Value - configDoDia.HoraInicioIntervalo.Value),
                                Deslocamento = 0,
                                Intervalo = true,
                                Equipe = configDoDia.Equipe
                            };

                            ja_agendadosTecnicos.Add(intervalo);
                            ja_agendados.Add(intervalo);
                        }
                        else
                        {
                            var intervalo = new Painel_Servicos_AgendamentosEquipe
                            {
                                Data_Inicial = configDoDia.HoraInicioIntervalo,
                                Data_Final = configDoDia.HoraTerminoIntervalo,
                                Deslocamento = 0,
                                Intervalo = true,
                                Equipe = configDoDia.Equipe
                            };

                            ja_agendadosTecnicos.Add(intervalo);
                            ja_agendados.Add(intervalo);

                            hora = configDoDia.HoraTerminoIntervalo.Value;
                        }
                    }

                }
                else if (obj.Hora_Final >= configDoDia.HoraInicioIntervalo &&
                         obj.Hora_Inicio >= configDoDia.HoraInicioIntervalo)
                {
                    if (agendamento_no_almoco == null && ja_agendadosTecnicos.Where(p => p.Intervalo).Count() == 0)
                    {
                        var intervalo = new Painel_Servicos_AgendamentosEquipe
                        {
                            Data_Inicial = configDoDia.HoraInicioIntervalo,
                            Data_Final = configDoDia.HoraTerminoIntervalo,
                            Deslocamento = 0,
                            Intervalo = true,
                            Equipe = configDoDia.Equipe
                        };

                        ja_agendadosTecnicos.Add(intervalo);
                        ja_agendados.Add(intervalo);

                        hora = configDoDia.HoraTerminoIntervalo.Value;
                        return hora;
                    }
                }
            }



            if (obj.Hora_Final > configDoDia.HoraTermino)
            {
                return obj.Hora_Final;
            }

            obj.Hora_InicioStr = obj.Hora_Inicio.ToString("HH:mm");
            obj.Hora_FinalStr = obj.Hora_Final.ToString("HH:mm");

            hora = obj.Hora_Final;

            //var jaAgendadoMesmoHorario = ja_agendadosTecnicos.Find(p => PeriodoApiController.Between(obj.Hora_Inicio, p.Data_Inicial, p.Data_Final) ||
            //                                                            PeriodoApiController.Between(obj.Hora_Final, p.Data_Inicial, p.Data_Final));

            var jaAgendadoMesmoHorario = jaExisteAgendadoNoMesmoHorario(ja_agendadosTecnicos, obj);

            var ausentesMesmoHorario = jaExisteAusenciaNoMesmoHorario(listaAusencia, obj);

            if (jaAgendadoMesmoHorario == null && ausentesMesmoHorario == null)
            {
                configDoDia.Horas.Add(obj);

            }
            else if (jaAgendadoMesmoHorario != null)
            {
                return jaAgendadoMesmoHorario.Data_Final.Value;
            }
            else if (ausentesMesmoHorario != null)
            {
                return ausentesMesmoHorario.Data_Final.Value;
            }



            return hora;
        }

        public Painel_Servicos_AgendamentosEquipe jaExisteAusenciaNoMesmoHorario(List<Painel_Servicos_AgendamentosEquipe> ja_ausentes, AgendamentoHorasDisponiveis obj)
        {
            var jaAgendadoMesmoHorario = ja_ausentes.Find(p => ((obj.Hora_Inicio >= p.Data_Inicial && obj.Hora_Inicio < p.Data_Final) ||
                                        (obj.Hora_Final > p.Data_Inicial && obj.Hora_Final <= p.Data_Final)));

            return jaAgendadoMesmoHorario;
        }

        public Painel_Servicos_AgendamentosEquipe jaExisteAgendadoNoMesmoHorario(List<Painel_Servicos_AgendamentosEquipe> ja_agendadosTecnicos, AgendamentoHorasDisponiveis obj)
        {
            var jaAgendadoMesmoHorario = ja_agendadosTecnicos.Find(p => ((obj.Hora_Inicio >= p.Data_Inicial && obj.Hora_Inicio < p.Data_Final) ||
                                        (obj.Hora_Final > p.Data_Inicial && obj.Hora_Final <= p.Data_Final)));

            return jaAgendadoMesmoHorario;
        }

        /// <summary>
        /// retorna latitude e longitude que será usada como referencia para medir distancia
        /// </summary>
        /// <param name="equipe"></param>
        /// <param name="JaAgendados"></param>
        /// <param name="referenciaDistancia"></param>
        /// <returns></returns>
        public (Double? Latitude, Double? Longitude, TimeSpan? Horario) GetLatitudeLongitudeReferencia(TecnicoRoteirizar equipe, List<Painel_Servicos_AgendamentosEquipe> JaAgendados, ReferenciaDistancia referenciaDistancia)
        {

            Double? Latitude = equipe.Latitude;
            Double? Longitude = equipe.Longitude;
            TimeSpan? Horario = (TimeSpan?)null;


            switch (referenciaDistancia)
            {
                case ReferenciaDistancia.PontoPartida:
                    {
                        Latitude = equipe.Latitude;
                        Longitude = equipe.Longitude;
                        Horario = (TimeSpan?)null;
                        break;
                    }
                case ReferenciaDistancia.PrimeiroServico:
                    {
                        var primeiroServico = JaAgendados.OrderBy(p => p.Data_Inicial).ToList().FindLast(p => p.ReferenciaAgrupamento) ?? JaAgendados.OrderBy(p => p.Data_Inicial).FirstOrDefault();
                        if (primeiroServico != null)
                        {
                            Latitude = primeiroServico.Latitude;
                            Longitude = primeiroServico.Longitude;
                            Horario = primeiroServico.Data_Final?.TimeOfDay;
                        }
                        break;
                    }
                case ReferenciaDistancia.UltimoServico:
                    {
                        var ultimoServico = JaAgendados.Where(p => p.Intervalo == false).OrderBy(p => p.Data_Inicial).LastOrDefault();
                        if (ultimoServico != null)
                        {
                            Latitude = ultimoServico.Latitude;
                            Longitude = ultimoServico.Longitude;
                            Horario = ultimoServico.Data_Final?.TimeOfDay;
                        }
                        break;
                    }
            }

            return (Latitude, Longitude, Horario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="doc"></param>
        /// <param name="equipe"></param>
        /// <param name="documentosRestantes"></param>
        /// <returns></returns>
        public List<DocumentoRoteirizar> FiltrarOrdensTecnicoCapaz(Configuracoes_Gerais config, Painel_ReteirizacaoDoc_Agrupado doc, TecnicoRoteirizar equipe, List<DocumentoRoteirizar> documentosRestantes, Int32? nivelLocalidade)
        {
            if (VerificarTecnicoCapaz(config, equipe, doc, nivelLocalidade))
            {
                if (config.Criterio_Area && config.Criterio_Habilidade)
                {
                    return documentosRestantes.Where(p => (p.Grupo_Area, p.Area, p.Localidade, p.Id_Natureza) == (doc.Cod_Escritorio, doc.Cod_Area, doc.LocalidadeStr, doc.Cod_Natureza)).ToList();
                }
                else if (config.Criterio_Area)
                {
                    return documentosRestantes.Where(p => (p.Grupo_Area, p.Area, p.Localidade) == (doc.Cod_Escritorio, doc.Cod_Area, doc.LocalidadeStr)).ToList();
                }
                else if (config.Criterio_Habilidade)
                    return documentosRestantes.Where(p => p.Id_Natureza == doc.Cod_Natureza).ToList();
                else
                    return documentosRestantes;
            }

            return new List<DocumentoRoteirizar>();
        }

        public bool FiltrarOrdensTodosTecnicosCapazes(Configuracoes_Gerais config, Painel_ReteirizacaoDoc_Agrupado doc, TecnicoRoteirizar equipe, List<DocumentoRoteirizar> documentosRestantes)
        {
            var tecnicosAprovados = new List<TecnicoRoteirizar>();
            //verificar localidade para todos os técnicos
            for (int nivelLocalidade = 1; nivelLocalidade <= 3; nivelLocalidade++)
            {
                if (VerificarTecnicoCapaz(config, equipe, doc, nivelLocalidade))
                {
                    if (config.Criterio_Area && config.Criterio_Habilidade)
                    {
                        if (documentosRestantes.Where(p => (p.Grupo_Area, p.Area, p.Localidade, p.Id_Natureza) == (doc.Cod_Escritorio, doc.Cod_Area, doc.LocalidadeStr, doc.Cod_Natureza)).ToList().Count > 0)
                        {
                            return true;
                        }
                    }
                    else if (config.Criterio_Area)
                    {
                        if (documentosRestantes.Where(p => (p.Grupo_Area, p.Area, p.Localidade) == (doc.Cod_Escritorio, doc.Cod_Area, doc.LocalidadeStr)).ToList().Count > 0)
                        {
                            return true;
                        }
                    }
                    else if (config.Criterio_Habilidade)
                    {
                        if (documentosRestantes.Where(p => p.Id_Natureza == doc.Cod_Natureza).ToList().Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// retorna se o tecnico atende os criterios para realizar a tarefa
        /// </summary>
        /// <param name="config"></param>
        /// <param name="tecnico"></param>
        /// <param name="doc_Agrupado"></param>
        /// <returns></returns>
        public bool VerificarTecnicoCapaz(Configuracoes_Gerais config, TecnicoRoteirizar tecnico, Painel_ReteirizacaoDoc_Agrupado doc_Agrupado, Int32? nivelLocalidade)
        {

            if (this.IgnorarCriteriosHabilidadeArea)
            {
                return true;
            }
            var localidades = tecnico.Localidades.ToList();
            if (nivelLocalidade.HasValue)
            {
                localidades = localidades.Where(p => p.Nivel == nivelLocalidade).ToList();
            }

            bool capaz_area = VerificacaoEscritorio_Area_Localidade(localidades, doc_Agrupado);
            bool capaz_habilidade = false;


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

        /// <summary>
        /// Retorna se o técnico tem disponibilidade na região para realizar a tarefa
        /// </summary>
        /// <param name="localidades"></param>
        /// <param name="documento"></param>
        /// <returns></returns>
        public Boolean VerificacaoEscritorio_Area_Localidade(IEnumerable<LocalidadeRoteirizacao> localidades, Painel_ReteirizacaoDoc_Agrupado documento)
        {
            var retorno = false;


            try
            {
                foreach (var localidade in localidades)
                {
                    if (localidade.Todas_Areas && localidade.Todas_Localidades)
                    {
                        if (localidade.Cod_Escritorio == documento.Cod_Escritorio)
                        {
                            retorno = true;
                            break;
                        }
                    }
                    else if (localidade.Todas_Areas)
                    {
                        if ((localidade.Cod_Escritorio, localidade.LocalidadeStr) == (documento.Cod_Escritorio, documento.LocalidadeStr))
                        {
                            retorno = true;
                            break;
                        }
                    }
                    if (localidade.Todas_Localidades)
                    {
                        if ((localidade.Cod_Escritorio, localidade.Cod_Area) == (documento.Cod_Escritorio, documento.Cod_Area))
                        {
                            retorno = true;
                            break;
                        }
                    }

                    if (documento.LocalidadeStr != null)
                    {
                        if ((localidade.Cod_Escritorio, localidade.Cod_Area, localidade.LocalidadeStr) == (documento.Cod_Escritorio, documento.Cod_Area, documento.LocalidadeStr))
                        {
                            retorno = true;
                            break;
                        }
                    }
                    else
                    {
                        if ((localidade.Cod_Escritorio, localidade.Cod_Area) == (documento.Cod_Escritorio, documento.Cod_Area))
                        {
                            retorno = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //  Dev Tip: Use SHIFT + F9 para acessar quick watch e então avaliar com exatidão o erro gerado.
            }
            return retorno;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="vistosPelaEquipe"></param>
        /// <param name="tecnico"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public DocumentoRoteirizar GetDocumento(Double? Latitude, Double? Longitude, List<DocumentoRoteirizar> vistosPelaEquipe, TecnicoRoteirizar tecnico, List<Painel_Servicos_AgendamentosEquipe> JaAgendados, Configuracoes_Gerais config)
        {
            if (!config.Criterio_SLA_Execucao && !config.Criterio_SLA_Hora && !config.Criterio_Area)
            {
                var distancias = GetDistancias(Latitude, Longitude, vistosPelaEquipe);
                if (config.RoteirizarSemLatLong == false)
                {
                    distancias = distancias.Where(p => String.IsNullOrEmpty(p.Key.Latitude_Str) == false && String.IsNullOrEmpty(p.Key.Longitude_Str) == false).ToDictionary(p => p.Key, p => p.Value);
                }

                IOrderedEnumerable<KeyValuePair<DocumentoRoteirizar, double>> distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Value);


                if (config.Considerar_Agendamento_Periodo)
                {
                    distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Key.Data_hora_ini_agendamento != null &&
                                                                                p.Key.TurnoHoraIni != null ?
                                                                                p.Key.Data_hora_ini_agendamento.Value.Date.Add(p.Key.TurnoHoraIni.Value.TimeOfDay) :
                                                                                p.Key.Data_hora_ini_agendamento.GetValueOrDefault(DateTime.MaxValue));
                }
                else
                {
                    distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Key.Data_hora_ini_agendamento.GetValueOrDefault(DateTime.MaxValue));
                }

                return distanciasOrdenadasPorAgendamento.
                                              ThenBy(p => p.Value).
                                              ThenByDescending(p => tecnico.JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                              ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                              Select(p => p.Key).
                                              FirstOrDefault();
            }
            else
            {
                if (config.RoteirizarSemLatLong == false)
                {
                    vistosPelaEquipe = vistosPelaEquipe.Where(p => String.IsNullOrEmpty(p.Latitude_Str) == false && String.IsNullOrEmpty(p.Longitude_Str) == false).ToList();
                }

                var distancias = GetDistancias(Latitude, Longitude, vistosPelaEquipe);
                var distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Value);

                if (config.Considerar_Agendamento_Periodo)
                {
                    distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Key.Data_hora_ini_agendamento != null && 
                                                                                p.Key.TurnoHoraIni != null ? 
                                                                                p.Key.Data_hora_ini_agendamento.Value.Date.Add(p.Key.TurnoHoraIni.Value.TimeOfDay) : 
                                                                                p.Key.Data_hora_ini_agendamento.GetValueOrDefault(DateTime.MaxValue));
                }
                else
                {
                    distanciasOrdenadasPorAgendamento = distancias.OrderBy(p => p.Key.Data_hora_ini_agendamento.GetValueOrDefault(DateTime.MaxValue));
                }



                if (config.Criterio_Area)
                {
                    if (config.Criterio_SLA_Execucao)
                    {
                        if (config.Criterio_SLA_Hora)
                        {
                            return distanciasOrdenadasPorAgendamento.
                                              ThenBy(p => p.Key.SLA.GetValueOrDefault(DateTime.MaxValue)).
                                              ThenBy(p => p.Value).
                                              ThenByDescending(p => JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                              ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                              Select(p => p.Key).
                                              FirstOrDefault();
                        }
                        else
                        {
                            return distanciasOrdenadasPorAgendamento.
                                              ThenBy(p => p.Key.SLA.GetValueOrDefault(DateTime.MaxValue).Date).
                                              ThenBy(p => p.Value).
                                              ThenByDescending(p => JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                              ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                              Select(p => p.Key).
                                              FirstOrDefault();
                        }
                    }
                    else
                    {
                        return distanciasOrdenadasPorAgendamento.
                                          ThenBy(p => p.Value).
                                          ThenByDescending(p => JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                          ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                          Select(p => p.Key).
                                          FirstOrDefault();
                    }
                }
                else
                {

                    if (config.Criterio_SLA_Hora)
                    {
                        return distanciasOrdenadasPorAgendamento.
                                          ThenBy(p => p.Key.SLA.GetValueOrDefault(DateTime.MaxValue)).
                                          ThenBy(p => p.Value).
                                          ThenByDescending(p => JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                          ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                          Select(p => p.Key).
                                          FirstOrDefault();
                    }
                    else
                    {
                        return distanciasOrdenadasPorAgendamento.
                                          ThenBy(p => p.Key.SLA.GetValueOrDefault(DateTime.MaxValue).Date).
                                          ThenBy(p => p.Value).
                                          ThenByDescending(p => JaAgendados.Where(y => y.Localidade == p.Key.Localidade &&
                                                                                                   y.Area == p.Key.Area &&
                                                                                                   y.GrupoArea == p.Key.Grupo_Area).Count()).
                                          ThenBy(p => p.Key.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          ThenBy(p => p.Key.GetPrioridade_Nivel(tecnico.Localidades, p.Key)).
                                          Select(p => p.Key).
                                          FirstOrDefault();
                    }
                }

            }

        }

        /// <summary>
        /// RETORNAR O TEMPO DE DESLOCAMENTO NECESSARIO. 
        /// quando tiver fazendo o processo de agrupamento usando como referencia para a distancia o primeiro serviço, 
        /// não aplicar deslocamento para nao afetar a roteirizaçao.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="distancia">campo em km</param>
        /// <param name="referenciaDistancia"></param>
        /// <returns></returns>
        public TimeSpan GetTempoDeslocamento(Configuracoes_Gerais config, Double distancia, ReferenciaDistancia referenciaDistancia)
        {

            if (referenciaDistancia == ReferenciaDistancia.PrimeiroServico && config.Utiliza_AgrupamentoDistancia == true)
            {
                return TimeSpan.Zero;
            }
            else if (distancia < (config.DistanciaMinimaDeslocamento / 1000))
            {
                return TimeSpan.Zero;
            }
            else if (config.TempoBaseDeslocamentoPorKm.HasValue && config.TempoBaseDeslocamentoPorKm.Value.TimeOfDay.TotalMinutes > 0)
            {
                try
                {
                    double totalMinutos = distancia * (config.TempoBaseDeslocamentoPorKm.Value.TimeOfDay.TotalMinutes);
                    return TimeSpan.FromMinutes(totalMinutos);
                }
                catch
                {
                    return TimeSpan.Zero;
                }
            }
            else if (config.Tempo_Medio_Deslocamento.HasValue)
            {
                return config.Tempo_Medio_Deslocamento.Value.TimeOfDay;
            }

            return TimeSpan.Zero;
        }

        public void PrenchendoLocalizacaoConfigCaixa(Configuracoes_Gerais config, List<DocumentoRoteirizar> documentos)
        {

            foreach (var item in documentos)
            {
                if (String.IsNullOrEmpty(item.Latitude_Str))
                {
                    if (config.RoteirizarPorCaixa)
                    {
                        item.Latitude_Str = item.Latitude_Cx;
                        item.Longitude_Str = item.Longitude_Cx;
                    }
                }

            }


        }

        public bool AtendeSla(Configuracoes_Gerais config, DocumentoRoteirizar documento, AgendamentoHorasDisponiveis agendamentoHorario)
        {
            if (config.Criterio_SLA_Hora)
            {
                return documento.Data_Hora_Vencimento_OS >= agendamentoHorario.Hora_Inicio;
            }
            else
                return true;



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="Longitude"></param>
        /// <param name="vistosPelaEquipe"></param>
        /// <param name="tecnico"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public DocumentoRoteirizar GetDocumento(List<DocumentoRoteirizar> documentos, Configuracoes_Gerais config)
        {
            if (!config.Criterio_SLA_Execucao && !config.Criterio_SLA_Hora && !config.Criterio_Area)
            {
                return documentos.OrderByDescending(p => p.Tecnico == null ? 0 : p.Tecnico.JaAgendados.Where(y => y.Localidade == p.Localidade &&
                                                                                                   y.Area == p.Area &&
                                                                                                   y.GrupoArea == p.Grupo_Area).Count()).
                                              ThenBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              Select(p => p).
                                              FirstOrDefault();
            }
            else
            {
                if (config.RoteirizarSemLatLong == false)
                {
                    documentos = documentos.Where(p => String.IsNullOrEmpty(p.Latitude_Str) == false && String.IsNullOrEmpty(p.Longitude_Str) == false).ToList();
                }

                if (config.Criterio_Area)
                {
                    if (config.Criterio_SLA_Execucao)
                    {
                        if (config.Criterio_SLA_Hora)
                        {
                            return documentos.OrderBy(p => p.SLA.GetValueOrDefault(DateTime.MaxValue)).
                                              ThenBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              Select(p => p).
                                              FirstOrDefault();
                        }
                        else
                        {
                            return documentos.
                                              OrderBy(p => p.SLA.GetValueOrDefault(DateTime.MaxValue).Date).
                                              ThenBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                              Select(p => p).
                                              FirstOrDefault();
                        }
                    }
                    else
                    {
                        return documentos.OrderBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          Select(p => p).
                                          FirstOrDefault();
                    }
                }
                else
                {

                    if (config.Criterio_SLA_Hora)
                    {
                        return documentos.OrderBy(p => p.SLA.GetValueOrDefault(DateTime.MaxValue)).
                                          ThenBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          Select(p => p).
                                          FirstOrDefault();
                    }
                    else
                    {
                        return documentos.OrderBy(p => p.SLA.GetValueOrDefault(DateTime.MaxValue).Date).
                                          ThenBy(p => p.Data_hora_Criacao.GetValueOrDefault(DateTime.MaxValue)).
                                          Select(p => p).
                                          FirstOrDefault();
                    }
                }

            }

        }

    }
}