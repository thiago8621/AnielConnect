using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using FluentNHibernate.Testing.Values;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class RoteirizarEncaixe : RoteirizacaoBase, IRoterizar
    {
        public Configuracoes_Gerais config { get; set; }
        public Queue<TecnicoRoteirizar> tecnicos { get; set; }
        public RoteirizacaoDiaConf dia { get; set; }
        public List<Turno_ServicoAniel> turnos { get; set; }
        private int OrdemDistribuida { get; set; } = 0;
        public List<Painel_Servicos_AgendamentosEquipe> listaAusencias { get; set; }
        public RoteirizarEncaixe(Configuracoes_Gerais _config, RoteirizacaoDiaConf _dia, List<Turno_ServicoAniel> _turnos, Queue<TecnicoRoteirizar> _tecnicos,
                                          DateTime _horaRoteirizacao, bool _ignorarCriteriosHabilidadeArea, bool _manterOrdemRota, List<Painel_Servicos_AgendamentosEquipe> _listaAusencias)
        {
            dia = _dia;
            config = _config;
            turnos = _turnos;
            tecnicos = _tecnicos;
            HoraRoteirizacao = _horaRoteirizacao;
            IgnorarCriteriosHabilidadeArea = _ignorarCriteriosHabilidadeArea;
            ManterOrdemRota = _manterOrdemRota;
            listaAusencias = _listaAusencias;

        }

        public (List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados) Run(List<DocumentoRoteirizar> documentos)
        {
            PrenchendoLocalizacaoConfigCaixa(this.config, documentos);
            var AtribuirOSReincTec = config.AtribuirOSReincTec;
            List<DocumentoRoteirizar> documentosRestantes = new List<DocumentoRoteirizar>(documentos);
            List<DocumentoRoteirizar> documentosRoteirizados = new List<DocumentoRoteirizar>();
            List<DocumentoRoteirizar> documentosNaoRoteirizados = new List<DocumentoRoteirizar>();

            var docAgrupado = AgrupaDocumentosCriterio(config, documentos).ToList();
            var ja_agendados = tecnicos.SelectMany(p => p.JaAgendados).ToList();
            while (documentosRestantes.Any())
            {
                var documento = GetDocumento(documentosRestantes, config);
                if(documento == null)
                    break;

                var tecnico = RunOs(tecnicos.ToList(), documento, ReferenciaDistancia.UltimoServico, ja_agendados);
                if (tecnico != null)
                {
                    documentosRoteirizados.AddRange(AtualizarDocumentos(new List<DocumentoRoteirizar> { documento }, tecnico));
                }
                else
                {
                    documentosNaoRoteirizados.Add(documento);
                }

                documentosRestantes.Remove(documento);
            }

            if (this.config.RoteirizarSemLatLong == false)
            {
                foreach (var item in documentosRestantes.FindAll(p => String.IsNullOrEmpty(p.Latitude_Str) && String.IsNullOrEmpty(p.Longitude_Str)))
                {
                    item.Log = "Configuracao da empresa nao permite roteirizar sem latitude e longitude";                    
                    documentosNaoRoteirizados.Add(item);
                }

            }

            return (documentosRoteirizados, documentosNaoRoteirizados);


        }

        public TecnicoRoteirizar RunOs(List<TecnicoRoteirizar> filaTecnicos,
                                       DocumentoRoteirizar documento,
                                       ReferenciaDistancia referenciaDistancia,
                                       List<Painel_Servicos_AgendamentosEquipe> ja_Agendados
                                       )
        {
            var docAgrupado = AgrupaDocumentosCriterio(config, new List<DocumentoRoteirizar> { documento }).ToList().FirstOrDefault();

            List<TecnicoRoteirizar> filaTecnicostmp = new List<TecnicoRoteirizar>();
            foreach (var equipe in filaTecnicos)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var resultadoFiltro = FiltrarOrdensTecnicoCapaz(config, docAgrupado, equipe, new List<DocumentoRoteirizar> { documento }, i);
                    if (resultadoFiltro.Count > 0)
                    {
                        equipe.NivelLocalidadeEscolhido = i;

                        if (filaTecnicostmp.Any(p => p.Equipe == equipe.Equipe) == false)
                            filaTecnicostmp.Add(equipe);
                    }

                }
            }

            filaTecnicos = filaTecnicostmp;

            Dictionary<TecnicoRoteirizar, (AgendamentoHorasDisponiveis horario, Double distancia)> distancias = new Dictionary<TecnicoRoteirizar, (AgendamentoHorasDisponiveis horario, Double distancia)>();
            foreach (var item in filaTecnicos)
            {
                List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_Agendados.Where(p => p.Equipe == item.Equipe).ToList();

                Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;
                AgendamentoDiaModel configDoDia = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, item.HorarioConnect, item.Equipe);

                //rotina foi aplicada mais internamente na rotina de montagem de horarios no metodo GetHorasDisponiveisEquipe
                if (this.ManterOrdemRota == true || this.dia.Data.Date == this.HoraRoteirizacao.Date)
                {
                    //configDoDia.HoraTermino = Convert.ToDateTime(configDoDia.HoraTermino.Value.ToString("dd/MM/yyyy 23:59"));
                    if (configDoDia != null && configDoDia.HoraInicio < this.HoraRoteirizacao)
                        configDoDia.HoraInicio = this.HoraRoteirizacao;
                }
                var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                TimeSpan tempo_deslocamento = TimeSpan.FromMinutes(tempoPadraoDeslocamento);



                if (documento.CodTurnoServ.HasValue && this.config.Considerar_Agendamento_Periodo && documento.TurnoHoraIni.HasValue && documento.TurnoHoraFim.HasValue && documento.Data_hora_ini_agendamento.HasValue && documento.Data_hora_fim_agendamento.HasValue)
                {
                    tempo_deslocamento = TimeSpan.FromMinutes(this.config.Tempo_Medio_Deslocamento.HasValue ? this.config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes : 0);
                }

                List<AgendamentoHorasDisponiveis> horarios = configDoDia != null ?
                                                         GetHorarioEquipeDisponivel(this.config, documento,
                                                         tempo_deslocamento.TotalMinutes,
                                                         configDoDia,
                                                         ja_Agendados_Tecnico,
                                                         ja_Agendados,
                                                         this.listaAusencias) :
                                                         new List<AgendamentoHorasDisponiveis>();

                if (horarios.Count > 0)
                {
                    if ((config.Criterio_SLA_Hora || config.Criterio_SLA_Execucao) &&
                       this.dia.EncaixeAutomatico &&
                       documento.Data_Hora_Vencimento_OS.Value.Date == DateTime.Today &&
                       horarios[0].Hora_Inicio.TimeOfDay > documento.Data_Hora_Vencimento_OS.Value.TimeOfDay)
                    {
                        continue;
                    }

                    var agendadoAnterior = ja_Agendados_Tecnico.FirstOrDefault(p => p.Data_Inicial < horarios[0].Hora_Inicio);
                    Double distancia = 0;
                    double? latitudeReferencia = null;
                    double? longitudeReferencia = null;

                    if (this.dia.Data.Date == DateTime.Today)
                    {
                        latitudeReferencia = agendadoAnterior?.Latitude ?? (item.LatitudeAtual ?? item.Latitude);
                        longitudeReferencia = agendadoAnterior?.Longitude ?? (item.LongitudeAtual ?? item.Longitude);
                    }
                    else
                    {
                        latitudeReferencia = agendadoAnterior?.Latitude ?? (item.Latitude);
                        longitudeReferencia = agendadoAnterior?.Longitude ?? (item.Longitude);
                    }

                    if (latitudeReferencia == null || longitudeReferencia == null || documento.Latitude_Str == null || documento.Longitude_Str == null)
                        distancia = double.MaxValue;
                    else
                        distancia = MapaService.GetMenorDistancia(latitudeReferencia,
                                                                  longitudeReferencia,
                                                                  documento.GetLatitude(),
                                                                  documento.GetLongitude());


                    distancias.Add(item, (horarios[0], distancia));
                }

            }

            if (distancias.Count() == 0 && this.dia.EncaixeAutomatico)
            {
                foreach (var item in filaTecnicos)
                {
                    List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_Agendados.Where(p => p.Equipe == item.Equipe).ToList();
                    Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;
                    AgendamentoDiaModel configDoDia = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, item.HorarioConnect, item.Equipe);

                    //rotina foi aplicada mais internamente na rotina de montagem de horarios no metodo GetHorasDisponiveisEquipe
                    if (this.ManterOrdemRota == true || this.dia.Data.Date == this.HoraRoteirizacao.Date)
                    {
                        //configDoDia.HoraTermino = Convert.ToDateTime(configDoDia.HoraTermino.Value.ToString("dd/MM/yyyy 23:59"));
                        if (configDoDia != null && configDoDia.HoraInicio < this.HoraRoteirizacao)
                            configDoDia.HoraInicio = this.HoraRoteirizacao;
                    }
                    var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                    TimeSpan tempo_deslocamento = TimeSpan.FromMinutes(tempoPadraoDeslocamento);


                    Painel_Servicos_AgendamentosEquipe agendadoPosterior = null;


                    if ((config.Criterio_SLA_Hora || config.Criterio_SLA_Execucao) &&
                       this.dia.EncaixeAutomatico &&
                       documento.Data_Hora_Vencimento_OS.Value.Date == DateTime.Today)
                    {
                        agendadoPosterior = DateTime.Now > documento.Data_Hora_Vencimento_OS ? ja_Agendados_Tecnico.OrderBy(p => p.Data_Inicial).FirstOrDefault(p => p.Data_Inicial > DateTime.Now) : ja_Agendados_Tecnico.OrderBy(p => p.Data_Inicial).FirstOrDefault(p => p.Data_Inicial > DateTime.Now &&
                                                                                                                                                                                         p.Data_Inicial.Value.TimeOfDay <= documento.Data_Hora_Vencimento_OS.Value.TimeOfDay);
                    }
                    else
                    {
                        agendadoPosterior = ja_Agendados_Tecnico.OrderBy(p => p.Data_Inicial).FirstOrDefault(p => p.Data_Inicial > DateTime.Now);
                    }
                    


                    var hora = agendadoPosterior?.Data_Inicial != null ? agendadoPosterior.Data_Inicial.Value : DateTime.Now;
                    var agendarHorario = new AgendamentoHorasDisponiveis
                    {
                        Hora_Inicio = hora,
                        Hora_Final = hora + TimeSpan.FromMinutes(tempoExecucaoMinutos) + tempo_deslocamento,
                        Deslocamento = tempo_deslocamento.Minutes,
                        Display = hora.ToString("dd/MM/yyyy")
                    };

                    var agendadoAnterior = ja_Agendados_Tecnico.OrderByDescending(p => p.Data_Inicial).FirstOrDefault(p => p.Data_Inicial < hora);
                    Double distancia = 0;
                    double? latitudeReferencia = null;
                    double? longitudeReferencia = null;

                    if (this.dia.Data.Date == DateTime.Today)
                    {
                        latitudeReferencia = agendadoAnterior?.Latitude ?? (item.LatitudeAtual ?? item.Latitude);
                        longitudeReferencia = agendadoAnterior?.Longitude ?? (item.LongitudeAtual ?? item.Longitude);
                    }
                    else
                    {
                        latitudeReferencia = agendadoAnterior?.Latitude ?? (item.Latitude);
                        longitudeReferencia = agendadoAnterior?.Longitude ?? (item.Longitude);
                    }

                    if (latitudeReferencia == null || longitudeReferencia == null || documento.Latitude_Str == null || documento.Longitude_Str == null)
                        distancia = double.MaxValue;
                    else
                        distancia = MapaService.GetMenorDistancia(latitudeReferencia,
                                                                  longitudeReferencia,
                                                                  documento.GetLatitude(),
                                                                  documento.GetLongitude());


                    if (agendarHorario.Hora_Inicio.TimeOfDay > configDoDia.HoraTermino.Value.TimeOfDay)
                        continue;

                    distancias.Add(item, (agendarHorario, distancia));
                }
            }


            List<KeyValuePair<TecnicoRoteirizar, (AgendamentoHorasDisponiveis horario, Double distancia)>> retornoOrdenadas = null;
            if (config.Criterio_SLA_Execucao && documento.SLA.HasValue)
            {
                retornoOrdenadas = distancias.OrderBy(p => p.Key.NivelLocalidadeEscolhido).
                                              ThenByDescending(p => p.Value.horario.Hora_Inicio <= documento.SLA).
                                              ThenBy(p => p.Value.distancia).ToList();

            }
            else
            {
                retornoOrdenadas = distancias.OrderBy(p => p.Value.distancia).ToList();
            }


            foreach (var item in retornoOrdenadas)
            {
                List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_Agendados.Where(p => p.Equipe == item.Key.Equipe).ToList();
                Painel_Servicos_AgendamentosEquipe novoAgendamento = new Painel_Servicos_AgendamentosEquipe
                {
                    Equipe = item.Key.Equipe,
                    Data_Inicial = item.Value.horario.Hora_Inicio,
                    Data_Final = item.Value.horario.Hora_Final,
                    Deslocamento = item.Value.horario.Deslocamento,
                    Latitude = documento.GetLatitude() ?? documento.Latitude_Localidade,
                    Longitude = documento.GetLongitude() ?? documento.Longitude_Localidade,
                    CodCt = documento.CodCt,
                    Numero_Obra = documento.Num_Obra,
                    Num_Doc = documento.Num_Doc.GetValueOrDefault(0),
                    Projeto = documento.Projeto,
                    GrupoArea = documento.Grupo_Area,
                    Area = documento.Area,
                    CodLocalidade = Convert.ToString(documento.Cod_Localidade),
                    Localidade = documento.Localidade,
                    ReferenciaAgrupamento = item.Value.distancia > config.DistanciaMaximaAgrupamento ? true : false
                };

                ja_Agendados.Add(novoAgendamento);
                ja_Agendados_Tecnico.Add(novoAgendamento);

                if (!item.Value.horario.AgendamentoAntigo)
                {
                    if (referenciaDistancia != ReferenciaDistancia.PrimeiroServico)
                    {
                        documento.Data_hora_ini_agendamento = novoAgendamento.Data_Inicial.Value;
                        documento.Data_hora_fim_agendamento = novoAgendamento.Data_Final.Value;
                        documento.DeslocamentoMinutos = (short)novoAgendamento.Deslocamento;
                    }
                }

                if (item.Key.Equipe != documento.Equipe_Anterior && !String.IsNullOrEmpty(documento.Equipe_Anterior))
                {
                    ja_Agendados.RemoveAll(p => p.Equipe == documento.Equipe_Anterior &&
                                                p.Numero_Obra == documento.Num_Obra &&
                                                p.Num_Doc == documento.Num_Doc &&
                                                p.Projeto == documento.Projeto &&
                                                p.CodCt == documento.CodCt);
                }


                return item.Key;

            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roteirizados"></param>
        /// <param name="tecnico"></param>
        /// <returns></returns>
        private List<DocumentoRoteirizar> AtualizarDocumentos(List<DocumentoRoteirizar> roteirizados, TecnicoRoteirizar tecnico)
        {

            roteirizados = roteirizados.Select(p =>
            {
                p.Equipe = tecnico.Equipe;
                p.Tecnico = tecnico;

                if (this.config.Considerar_Agendamento_Periodo && p.Data_hora_ini_agendamento.HasValue)
                    p.CodTurnoServ = PeriodoApiController.BuscarTurno(this.turnos, p.Data_hora_ini_agendamento.Value, p.Tempo_Execucao)?.Id;

                return p;

            }).ToList();

            return roteirizados;
        }
    }
}