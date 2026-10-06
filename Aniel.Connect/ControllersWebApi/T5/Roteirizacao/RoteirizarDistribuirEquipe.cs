using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class RoteirizarDistribuirEquipe : RoteirizacaoBusiness, IRoterizar
    {
        public Configuracoes_Gerais config { get; set; }
        public Queue<TecnicoRoteirizar> tecnicos { get; set; }
        public RoteirizacaoDiaConf dia { get; set; }
        public List<Turno_ServicoAniel> turnos { get; set; }
        private int OrdemDistribuida { get; set; } = 0;
        public List<Painel_Servicos_AgendamentosEquipe> listaAusencias { get; set; }

        public RoteirizarDistribuirEquipe(Configuracoes_Gerais _config, RoteirizacaoDiaConf _dia, List<Turno_ServicoAniel> _turnos, Queue<TecnicoRoteirizar> _tecnicos,
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
            List<DocumentoRoteirizar> documentosRestantesAgendados = new List<DocumentoRoteirizar>(documentos.Where(p => p.Data_hora_ini_agendamento.HasValue));
            List<DocumentoRoteirizar> documentosRestantesNaoAgendados = new List<DocumentoRoteirizar>(documentos.Where(p => p.Data_hora_ini_agendamento.HasValue == false));
            List<DocumentoRoteirizar> documentosRoteirizados = new List<DocumentoRoteirizar>();

            var docAgrupado = AgrupaDocumentosCriterio(config, documentos).ToList();
            var ja_agendados = tecnicos.SelectMany(p => p.JaAgendados).ToList();
            Dictionary<string, TecnicoRoteirizar> totalTecnicos = tecnicos.Distinct()
                                                                          .Where(p => {
                                                                              var habilitado = false;
                                                                              foreach(var doc in docAgrupado) 
                                                                              {
                                                                                  habilitado = FiltrarOrdensTodosTecnicosCapazes(config, doc, p, documentos);

                                                                                  if (habilitado)
                                                                                      break;
                                                                              }
                                                                              
                                                                              return habilitado == true;                                                                              
                                                                          })
                                                                          .Select(p => new KeyValuePair<string, TecnicoRoteirizar> (p.Equipe, p))
                                                                          .ToDictionary(p => p.Key, p => p.Value);

            if (this.config.RoteirizarSemLatLong == false)
            {
                foreach (var item in documentosRestantesAgendados.FindAll(p => String.IsNullOrEmpty(p.Latitude_Str) && String.IsNullOrEmpty(p.Longitude_Str)))
                {
                    item.Log = "Configuracao da empresa nao permite roteirizar sem latitude e longitude";
                }

                foreach (var item in documentosRestantesNaoAgendados.FindAll(p => String.IsNullOrEmpty(p.Latitude_Str) && String.IsNullOrEmpty(p.Longitude_Str)))
                {
                    item.Log = "Configuracao da empresa nao permite roteirizar sem latitude e longitude";
                }

            }


            var jaAgendadosAgrupamento = new List<Painel_Servicos_AgendamentosEquipe>(ja_agendados);
            List<DocumentoRoteirizar> documentosRoteirizadosOtimizados = new List<DocumentoRoteirizar>();
            List<DocumentoRoteirizar> documentosNaoOtimizados = new List<DocumentoRoteirizar>();
            bool ignorarSla = false;


            var tryRoteirizarReinc = true;
            var repeticaoLimite = 3;
            for (int i = 1; i <= repeticaoLimite; i++)
            {
                bool repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = true;

                for (int y = 1; y <= 4 && repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima; y++)
                {
                    repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = false;
                    documentosRestantesAgendados = documentosRestantesAgendados.Select(p => { p.IgnoradaPorConfiguracaoDistanciaMaxima = false; return p; }).ToList();
                    var filaTecnicoRodada = new Queue<TecnicoRoteirizar>(tecnicos);
                    while (filaTecnicoRodada.Count > 0 && documentosRestantesAgendados.Count > 0)
                    {
                        var agrupamento = RunEquipe(docAgrupado, filaTecnicoRodada.Peek(), jaAgendadosAgrupamento, documentosRestantesAgendados, ReferenciaDistancia.PrimeiroServico, i, y, totalTecnicos, ignorarSla, AtribuirOSReincTec);
                        documentosRestantesAgendados = agrupamento.documentosNaoRoteirizados.ToList();
                        agrupamento.documentosRoteirizados = AtualizarDocumentos(agrupamento.documentosRoteirizados, filaTecnicoRodada.Peek());
                        documentosRoteirizados = agrupamento.documentosRoteirizados;

                        if (agrupamento.SlaNaoCumprido != null)
                        {
                            //rodadas extra na busca para cumprir sla
                            Dictionary<TecnicoRoteirizar, DateTime> janelasMinutosTecnicos = new Dictionary<TecnicoRoteirizar, DateTime>();
                            var filaTecnicoRodadaExtra = new Queue<TecnicoRoteirizar>(tecnicos);
                            while (filaTecnicoRodadaExtra.Count > 0)
                            {
                                var tecnico = filaTecnicoRodadaExtra.Peek();
                                var janelaDisponivel = MelhorJanelaSlaNaoCumprimido(tecnico, agrupamento.SlaNaoCumprido, jaAgendadosAgrupamento, i);

                                if (janelaDisponivel != null)
                                    janelasMinutosTecnicos.Add(tecnico, janelaDisponivel.Value);

                                filaTecnicoRodadaExtra.Dequeue();
                            }

                            foreach (var item in janelasMinutosTecnicos.OrderBy(p => p.Value))
                            {
                                var melhorJanela = item;
                                var agrupamentoSlaNaoCumprido = RunEquipe(docAgrupado, melhorJanela.Key, jaAgendadosAgrupamento, new List<DocumentoRoteirizar> { agrupamento.SlaNaoCumprido }, ReferenciaDistancia.PrimeiroServico, i, y, totalTecnicos, AtribuirOSReincTec);
                                agrupamentoSlaNaoCumprido.documentosRoteirizados = AtualizarDocumentos(agrupamentoSlaNaoCumprido.documentosRoteirizados, melhorJanela.Key);
                                documentosRoteirizados = agrupamentoSlaNaoCumprido.documentosRoteirizados;
                                documentosRestantesAgendados.Remove(agrupamento.SlaNaoCumprido);


                                var servicosOtimizarSla = documentosRoteirizados.Where(p => p.Equipe == melhorJanela.Key.Equipe).ToList();
                                var otimizacaoSla = RunEquipe(docAgrupado, melhorJanela.Key, ja_agendados, new List<DocumentoRoteirizar> { agrupamento.SlaNaoCumprido }, ReferenciaDistancia.UltimoServico, i, 4, totalTecnicos, true, AtribuirOSReincTec);
                                otimizacaoSla.documentosRoteirizados = AtualizarDocumentos(otimizacaoSla.documentosRoteirizados, melhorJanela.Key);
                                documentosRoteirizadosOtimizados.AddRange(otimizacaoSla.documentosRoteirizados);

                                otimizacaoSla.documentosNaoRoteirizados.Select(p =>
                                {
                                    p.Equipe = null;
                                    p.Tecnico = null;
                                    p.OrdemDistribuida = null;
                                    p.NivelRodadaRoteirizada = null;
                                    p.AgrupamentoRodadaRoteirizada = null;
                                    return p;
                                }).ToList();

                                documentosRestantesAgendados.AddRange(otimizacaoSla.documentosNaoRoteirizados);
                                jaAgendadosAgrupamento = new List<Painel_Servicos_AgendamentosEquipe>(ja_agendados);

                                if (otimizacaoSla.documentosRoteirizados.Count == 0)
                                    ignorarSla = true;
                                else
                                {
                                    ignorarSla = false;
                                    break;
                                }
                            }
                            //fila nao foi alterada, mantem o mesmo tecnico
                            continue;

                        }

                        var servicosOtimizar = documentosRoteirizados.Where(p => p.Equipe == filaTecnicoRodada.Peek().Equipe).ToList();
                        var otimizacao = RunEquipe(docAgrupado, filaTecnicoRodada.Peek(), ja_agendados, servicosOtimizar, ReferenciaDistancia.UltimoServico, i, 4, totalTecnicos, false, AtribuirOSReincTec);
                        otimizacao.documentosRoteirizados = AtualizarDocumentos(otimizacao.documentosRoteirizados, filaTecnicoRodada.Peek());
                        documentosRoteirizadosOtimizados.AddRange(otimizacao.documentosRoteirizados);

                        otimizacao.documentosNaoRoteirizados.Select(p =>
                        {
                            p.Equipe = null;
                            p.Tecnico = null;
                            p.OrdemDistribuida = null;
                            p.NivelRodadaRoteirizada = null;
                            p.AgrupamentoRodadaRoteirizada = null;
                            return p;
                        }).ToList();

                        foreach (var item in otimizacao.documentosNaoRoteirizados)
                        {
                            if (documentosNaoOtimizados.Find(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt) == null)
                                documentosNaoOtimizados.Add(item);
                        }

                        jaAgendadosAgrupamento = new List<Painel_Servicos_AgendamentosEquipe>(ja_agendados);

                        if (documentosRestantesAgendados.FindAll(p => p.IgnoradaPorConfiguracaoDistanciaMaxima).Count > 0)
                            repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = true;

                        if (otimizacao.documentosNaoRoteirizados.Count == 0 || documentosRestantesAgendados.Count == 0)
                        {
                            documentosRestantesAgendados.AddRange(documentosNaoOtimizados);
                            documentosNaoOtimizados = new List<DocumentoRoteirizar>();
                            filaTecnicoRodada.Dequeue();
                        }

                        ignorarSla = false;
                    }
                }

                repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = true;
                for (int y = 1; y <= 4 && repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima; y++)
                {
                    repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = false;
                    documentosRestantesNaoAgendados = documentosRestantesNaoAgendados.Select(p => { p.IgnoradaPorConfiguracaoDistanciaMaxima = false; return p; }).ToList();
                    var filaTecnicoRodada = new Queue<TecnicoRoteirizar>(tecnicos);
                    while (filaTecnicoRodada.Count > 0 && documentosRestantesNaoAgendados.Count > 0)
                    {
                        var agrupamento = RunEquipe(docAgrupado, filaTecnicoRodada.Peek(), jaAgendadosAgrupamento, documentosRestantesNaoAgendados, ReferenciaDistancia.PrimeiroServico, i, y, totalTecnicos, ignorarSla, AtribuirOSReincTec);
                        documentosRestantesNaoAgendados = agrupamento.documentosNaoRoteirizados.ToList();
                        agrupamento.documentosRoteirizados = AtualizarDocumentos(agrupamento.documentosRoteirizados, filaTecnicoRodada.Peek());
                        documentosRoteirizados = agrupamento.documentosRoteirizados;

                        if (agrupamento.SlaNaoCumprido != null)
                        {
                            //rodadas extra na busca para cumprir sla
                            Dictionary<TecnicoRoteirizar, DateTime> janelasMinutosTecnicos = new Dictionary<TecnicoRoteirizar, DateTime>();
                            var filaTecnicoRodadaExtra = new Queue<TecnicoRoteirizar>(tecnicos);
                            while (filaTecnicoRodadaExtra.Count > 0)
                            {
                                var tecnico = filaTecnicoRodadaExtra.Peek();
                                var janelaDisponivel = MelhorJanelaSlaNaoCumprimido(tecnico, agrupamento.SlaNaoCumprido, jaAgendadosAgrupamento, i);

                                if (janelaDisponivel != null)
                                    janelasMinutosTecnicos.Add(tecnico, janelaDisponivel.Value);

                                filaTecnicoRodadaExtra.Dequeue();
                            }

                            foreach (var item in janelasMinutosTecnicos.OrderBy(p => p.Value))
                            {
                                var melhorJanela = item;
                                var agrupamentoSlaNaoCumprido = RunEquipe(docAgrupado, melhorJanela.Key, jaAgendadosAgrupamento, new List<DocumentoRoteirizar> { agrupamento.SlaNaoCumprido }, ReferenciaDistancia.PrimeiroServico, i, y, totalTecnicos, true);
                                agrupamentoSlaNaoCumprido.documentosRoteirizados = AtualizarDocumentos(agrupamentoSlaNaoCumprido.documentosRoteirizados, melhorJanela.Key);
                                documentosRoteirizados = agrupamentoSlaNaoCumprido.documentosRoteirizados;
                                documentosRestantesNaoAgendados.Remove(agrupamento.SlaNaoCumprido);


                                var servicosOtimizarSla = documentosRoteirizados.Where(p => p.Equipe == melhorJanela.Key.Equipe).ToList();
                                var otimizacaoSla = RunEquipe(docAgrupado, melhorJanela.Key, ja_agendados, new List<DocumentoRoteirizar> { agrupamento.SlaNaoCumprido }, ReferenciaDistancia.UltimoServico, i, 4, totalTecnicos, true);
                                otimizacaoSla.documentosRoteirizados = AtualizarDocumentos(otimizacaoSla.documentosRoteirizados, melhorJanela.Key);
                                documentosRoteirizadosOtimizados.AddRange(otimizacaoSla.documentosRoteirizados);

                                otimizacaoSla.documentosNaoRoteirizados.Select(p =>
                                {
                                    p.Equipe = null;
                                    p.Tecnico = null;
                                    p.OrdemDistribuida = null;
                                    p.NivelRodadaRoteirizada = null;
                                    p.AgrupamentoRodadaRoteirizada = null;
                                    return p;
                                }).ToList();

                                documentosRestantesNaoAgendados.AddRange(otimizacaoSla.documentosNaoRoteirizados);
                                jaAgendadosAgrupamento = new List<Painel_Servicos_AgendamentosEquipe>(ja_agendados);

                                if (otimizacaoSla.documentosRoteirizados.Count == 0)
                                    ignorarSla = true;
                                else
                                {
                                    ignorarSla = false;
                                    break;
                                }

                            }


                            //fila nao foi alterada, mantem o mesmo tecnico
                            continue;
                        }

                        var servicosOtimizar = documentosRoteirizados.Where(p => p.Equipe == filaTecnicoRodada.Peek().Equipe).ToList();
                        var otimizacao = RunEquipe(docAgrupado, filaTecnicoRodada.Peek(), ja_agendados, servicosOtimizar, ReferenciaDistancia.UltimoServico, i, 4, totalTecnicos, false, AtribuirOSReincTec);
                        otimizacao.documentosRoteirizados = AtualizarDocumentos(otimizacao.documentosRoteirizados, filaTecnicoRodada.Peek());
                        documentosRoteirizadosOtimizados.AddRange(otimizacao.documentosRoteirizados);

                        otimizacao.documentosNaoRoteirizados.Select(p =>
                        {
                            p.Equipe = null;
                            p.Tecnico = null;
                            p.OrdemDistribuida = null;
                            p.NivelRodadaRoteirizada = null;
                            p.AgrupamentoRodadaRoteirizada = null;
                            return p;
                        }).ToList();

                        foreach (var item in otimizacao.documentosNaoRoteirizados)
                        {
                            if (documentosNaoOtimizados.Find(p => p.Num_Obra == item.Num_Obra && p.Num_Doc == item.Num_Doc && p.Projeto == item.Projeto && p.CodCt == item.CodCt) == null)
                                documentosNaoOtimizados.Add(item);
                        }

                        jaAgendadosAgrupamento = new List<Painel_Servicos_AgendamentosEquipe>(ja_agendados);

                        if (documentosRestantesNaoAgendados.FindAll(p => p.IgnoradaPorConfiguracaoDistanciaMaxima).Count > 0)
                            repetirAgrupamentoParaCumprirConfiguracaoDistanciaMaxima = true;

                        if (otimizacao.documentosNaoRoteirizados.Count == 0 || documentosRestantesNaoAgendados.Count == 0)
                        {
                            documentosRestantesNaoAgendados.AddRange(documentosNaoOtimizados);
                            documentosNaoOtimizados = new List<DocumentoRoteirizar>();
                            filaTecnicoRodada.Dequeue();
                        }

                        ignorarSla = false;

                    }
                }

                if (i == repeticaoLimite)
                {
                    if (documentosRestantesNaoAgendados.Any(d => d.IsDocReincidencia)) //caso alguma dessas OS seja reincidente, vai enviar os documentos para uma nova tentativa de roteirizar.
                    {
                        docAgrupado = AgrupaDocumentosCriterio(config, documentosRestantesNaoAgendados.Where(d => d.IsDocReincidencia).ToList()).ToList();

                        if (tryRoteirizarReinc)
                        {
                            tryRoteirizarReinc = false;
                            AtribuirOSReincTec = false;
                            i = 0;
                        }
                    }
                }
            }
            return (documentosRoteirizadosOtimizados, documentosRestantesNaoAgendados.Concat(documentosRestantesAgendados).ToList());
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

        private (List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados, DocumentoRoteirizar SlaNaoCumprido) RunEquipe(List<Painel_ReteirizacaoDoc_Agrupado> docAgrupado, TecnicoRoteirizar equipe, List<Painel_Servicos_AgendamentosEquipe> ja_Agendados, List<DocumentoRoteirizar> documentosRestantes, ReferenciaDistancia referenciaDistancia, Int32? nivelLocalidade, int? RodadaAgrupamento, Dictionary<string,TecnicoRoteirizar> totalTecnicos, bool ignorarAtendeSla = false, bool ValidarOsReincidente = false)
        {
            bool ignorarDistanciaMaxima = RodadaAgrupamento.HasValue == false ? false : (RodadaAgrupamento == 4 ? true : false);
            DocumentoRoteirizar slaNaoCumprido = null;
            List<DocumentoRoteirizar> vistosPelaEquipe = docAgrupado.Count() > 0 ? docAgrupado.Select(doc => FiltrarOrdensTecnicoCapaz(config, doc, equipe, documentosRestantes, nivelLocalidade)).SelectMany(p => p).ToList() : documentosRestantes;
            List<DocumentoRoteirizar> documentosRoteirizados = new List<DocumentoRoteirizar>();
            List<DocumentoRoteirizar> documentosNaoRoteirizados = new List<DocumentoRoteirizar>(documentosRestantes.Where(p => !vistosPelaEquipe.Contains(p)));
            List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_Agendados.Where(p => p.Equipe == equipe.Equipe).ToList();

            if (this.config.RoteirizarSemLatLong == false)
            {
                foreach (var item in vistosPelaEquipe.FindAll(p => String.IsNullOrEmpty(p.Latitude_Str) && String.IsNullOrEmpty(p.Longitude_Str)))
                {
                    vistosPelaEquipe.Remove(item);
                    documentosNaoRoteirizados.Add(item);
                }
            }

            while (vistosPelaEquipe.Count() > 0 && ja_Agendados_Tecnico.Where(p => p.Intervalo == false).Select(p => p.Num_Doc).Distinct().Count() < dia.NumeroMaxAtendimentoTecnico)
            {
                var referencia = GetLatitudeLongitudeReferencia(equipe, ja_Agendados_Tecnico, referenciaDistancia);
                DocumentoRoteirizar documento = this.ManterOrdemRota ? vistosPelaEquipe.FirstOrDefault() : GetDocumento(referencia.Latitude, referencia.Longitude, vistosPelaEquipe, equipe, ja_Agendados_Tecnico, config);

                if (documento == null)
                    continue;

                if (ValidarOsReincidente)
                {
                    if (RodadaAgrupamento > 1 && nivelLocalidade == 3)
                    {
                        if (documento.IsDocReincidencia && documento.CodFornecedorAnterior != equipe.Cod_Fornecedor)
                        {
                            vistosPelaEquipe.Remove(documento);
                            documentosNaoRoteirizados.Add(documento);
                            continue;
                        }
                    }
                    else
                    {
                        if (totalTecnicos.Any(p => p.Key == documento.Equipe_Anterior)) 
                        {
                            if (documento.IsDocReincidencia && documento.Equipe_Anterior != equipe.Equipe)
                            {
                                vistosPelaEquipe.Remove(documento);
                                documentosNaoRoteirizados.Add(documento);
                                continue;
                            }
                        }
                    }
                }

                double distancia = Math.Ceiling(GetDistancias(referencia.Latitude, referencia.Longitude, new List<DocumentoRoteirizar>() { documento }).Values.First() / 1852);


                Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;
                AgendamentoDiaModel configDoDia = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, equipe.HorarioConnect, equipe.Equipe);

                //rotina foi aplicada mais internamente na rotina de montagem de horarios no metodo GetHorasDisponiveisEquipe
                if (this.ManterOrdemRota == true || this.dia.Data.Date == this.HoraRoteirizacao.Date)
                {
                    //configDoDia.HoraTermino = Convert.ToDateTime(configDoDia.HoraTermino.Value.ToString("dd/MM/yyyy 23:59"));
                    if (configDoDia != null && configDoDia.HoraInicio < this.HoraRoteirizacao)
                        configDoDia.HoraInicio = this.HoraRoteirizacao;
                }

                var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                TimeSpan tempo_deslocamento = ((documento.GetLatitude() ?? documento.Latitude_Localidade) != null && (documento.GetLongitude() ?? documento.Longitude_Localidade) != null && referencia.Latitude != null && referencia.Longitude != null) ? GetTempoDeslocamento(config, distancia, referenciaDistancia) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/

                if (referenciaDistancia == ReferenciaDistancia.PrimeiroServico)
                {
                    tempo_deslocamento = TimeSpan.Zero;  // ignorar distancia ao agrupar as ordens de servico, usar distancia apenas na otimizacao de rota
                }

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

                    if (ignorarDistanciaMaxima == false && config.LimiteDistanciaParaDeslocamento.HasValue)
                    {
                        //aumenta 10 para cada rodada
                        var fatorRodadaDistanciaDeslocamento = (RodadaAgrupamento - 1) * 10;
                        if (distancia > (config.LimiteDistanciaParaDeslocamento + fatorRodadaDistanciaDeslocamento))
                        {
                            Dictionary<TecnicoRoteirizar, double> distanciasOsTecnico = new Dictionary<TecnicoRoteirizar, double>();
                            distanciasOsTecnico.Add(equipe, distancia);
                            foreach (var item in this.tecnicos)
                            {
                                var doc = AgrupaDocumentosCriterio(config, new List<DocumentoRoteirizar> { documento }).FirstOrDefault();
                                var listaCapaz = FiltrarOrdensTecnicoCapaz(config, doc, item, new List<DocumentoRoteirizar> { documento }, nivelLocalidade);
                                if (listaCapaz.Count > 0)
                                {
                                    List<Painel_Servicos_AgendamentosEquipe> jaAgendadosTecnico = ja_Agendados.Where(p => p.Equipe == item.Equipe).ToList();
                                    var referenciaTecnico = GetLatitudeLongitudeReferencia(item, jaAgendadosTecnico, ReferenciaDistancia.PrimeiroServico);
                                    double distanciaTecnico = Math.Ceiling(GetDistancias(referenciaTecnico.Latitude, referenciaTecnico.Longitude, new List<DocumentoRoteirizar>() { documento }).Values.First() / 1852);



                                    if (distanciaTecnico < distancia)
                                    {
                                        Double tempoExecucaoMinutosTecnicoProximo = documento.Tempo_Execucao.Value.TotalMinutes;
                                        AgendamentoDiaModel configDoDiaTecnicoProximo = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, item.HorarioConnect, item.Equipe);

                                        if (this.ManterOrdemRota == true)
                                        {
                                            configDoDia.HoraTermino = Convert.ToDateTime(configDoDia.HoraTermino.Value.ToString("dd/MM/yyyy 23:59"));
                                            configDoDia.HoraInicio = this.HoraRoteirizacao;
                                        }

                                        var tempoPadraoDeslocamentoTecnicoProximo = config.Tempo_Medio_Deslocamento.Value.Minute;
                                        TimeSpan tempo_deslocamentoTecnicoProximo = ((documento.GetLatitude() ?? documento.Latitude_Localidade) != null && (documento.GetLongitude() ?? documento.Longitude_Localidade) != null && referencia.Latitude != null && referencia.Longitude != null) ? GetTempoDeslocamento(config, distancia, referenciaDistancia) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/

                                        if (referenciaDistancia == ReferenciaDistancia.PrimeiroServico)
                                        {
                                            tempo_deslocamento = TimeSpan.Zero;  // ignorar distancia ao agrupar as ordens de servico, usar distancia apenas na otimizacao de rota
                                        }

                                        if (documento.CodTurnoServ.HasValue && this.config.Considerar_Agendamento_Periodo && documento.TurnoHoraIni.HasValue && documento.TurnoHoraFim.HasValue && documento.Data_hora_ini_agendamento.HasValue && documento.Data_hora_fim_agendamento.HasValue)
                                        {
                                            tempo_deslocamento = TimeSpan.FromMinutes(this.config.Tempo_Medio_Deslocamento.HasValue ? this.config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes : 0);
                                        }

                                        List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_TecnicoProximo = ja_Agendados.Where(p => p.Equipe == item.Equipe).ToList();
                                        List<AgendamentoHorasDisponiveis> horariosTecnicoProximo = configDoDia != null ?
                                                                                 GetHorarioEquipeDisponivel(this.config, documento,
                                                                                 tempo_deslocamento.TotalMinutes,
                                                                                 configDoDia,
                                                                                 ja_Agendados_Tecnico,
                                                                                 ja_Agendados,
                                                                                 this.listaAusencias) :
                                                                                 new List<AgendamentoHorasDisponiveis>();
                                        if (horariosTecnicoProximo.Count > 0)
                                        {
                                            distanciasOsTecnico.Add(item, distanciaTecnico);
                                        }
                                    }
                                }
                            }

                            if (distanciasOsTecnico.OrderBy(p => p.Value).FirstOrDefault().Key.Equipe != equipe.Equipe)
                            {

                                if (documento.Data_hora_ini_agendamento.HasValue == false)
                                {
                                    documento.IgnoradaPorConfiguracaoDistanciaMaxima = true;
                                    break;
                                }
                                else
                                {
                                    documento.IgnoradaPorConfiguracaoDistanciaMaxima = true;
                                    documentosNaoRoteirizados.Add(documento);
                                    vistosPelaEquipe.Remove(documento);
                                    continue;
                                }


                            }
                        }
                    }

                    //Verifica se o horario atende o sla (apenas se a configuracao usa SLA por hora)
                    if (ignorarAtendeSla == false)
                    {
                        if (nivelLocalidade != null && config.Criterio_SLA_Hora)
                        {
                            slaNaoCumprido = documento;
                            break;
                        }
                        else if (nivelLocalidade != null && AtendeSla(config, documento, horarios.FirstOrDefault()) == false)
                        {
                            slaNaoCumprido = documento;
                            break;
                        }
                    }

                    Painel_Servicos_AgendamentosEquipe novoAgendamento = new Painel_Servicos_AgendamentosEquipe
                    {
                        Equipe = equipe.Equipe,
                        Data_Inicial = horarios[0].Hora_Inicio,
                        Data_Final = horarios[0].Hora_Final,
                        Deslocamento = horarios[0].Deslocamento,
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
                        ReferenciaAgrupamento = distancia > config.DistanciaMaximaAgrupamento ? true : false
                    };

                    ja_Agendados.Add(novoAgendamento);
                    ja_Agendados_Tecnico.Add(novoAgendamento);

                    if (!horarios[0].AgendamentoAntigo)
                    {
                        if (referenciaDistancia != ReferenciaDistancia.PrimeiroServico)
                        {
                            documento.Data_hora_ini_agendamento = novoAgendamento.Data_Inicial.Value;
                            documento.Data_hora_fim_agendamento = novoAgendamento.Data_Final.Value;
                            documento.DeslocamentoMinutos = (short)novoAgendamento.Deslocamento;
                        }
                    }

                    if (referenciaDistancia == ReferenciaDistancia.PrimeiroServico)
                    {
                        this.OrdemDistribuida += 1;
                        documento.OrdemDistribuida = this.OrdemDistribuida;
                        documento.AgrupamentoRodadaRoteirizada = RodadaAgrupamento;
                        documento.NivelRodadaRoteirizada = nivelLocalidade;
                    }

                    if (equipe.Equipe != documento.Equipe_Anterior && !String.IsNullOrEmpty(documento.Equipe_Anterior))
                    {
                        ja_Agendados.RemoveAll(p => p.Equipe == documento.Equipe_Anterior &&
                                                    p.Numero_Obra == documento.Num_Obra &&
                                                    p.Num_Doc == documento.Num_Doc &&
                                                    p.Projeto == documento.Projeto &&
                                                    p.CodCt == documento.CodCt);
                    }


                    documentosRoteirizados.Add(documento);
                }
                else
                {
                    if (!documento.Data_hora_ini_agendamento.HasValue)
                    {
                        documentosNaoRoteirizados.AddRange(vistosPelaEquipe.FindAll(p => p.Tempo_Execucao >= documento.Tempo_Execucao));
                        vistosPelaEquipe.RemoveAll(p => p.Tempo_Execucao >= documento.Tempo_Execucao);
                    }
                    else
                    {
                        documentosNaoRoteirizados.Add(documento);
                        vistosPelaEquipe.Remove(documento);
                    }
                    //documentosNaoRoteirizados.Add(documento);
                }

                vistosPelaEquipe.Remove(documento);
            }

            documentosNaoRoteirizados.AddRange(vistosPelaEquipe);
            return (documentosRoteirizados, documentosNaoRoteirizados, slaNaoCumprido);
        }

        private DateTime? MelhorJanelaSlaNaoCumprimido(TecnicoRoteirizar equipe, DocumentoRoteirizar documento, List<Painel_Servicos_AgendamentosEquipe> ja_Agendados, Int32 nivelLocalidade)
        {
            var doc = AgrupaDocumentosCriterio(config, new List<DocumentoRoteirizar> { documento }).FirstOrDefault();
            var listaCapaz = FiltrarOrdensTecnicoCapaz(config, doc, equipe, new List<DocumentoRoteirizar> { documento }, nivelLocalidade);

            if (listaCapaz.Count > 0)
            {
                List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_Agendados.Where(p => p.Equipe == equipe.Equipe).ToList();
                var referencia = GetLatitudeLongitudeReferencia(equipe, ja_Agendados_Tecnico, ReferenciaDistancia.PrimeiroServico);
                double distancia = Math.Ceiling(GetDistancias(referencia.Latitude, referencia.Longitude, new List<DocumentoRoteirizar>() { documento }).Values.First() / 1852);
                Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;
                AgendamentoDiaModel configDoDia = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, equipe.HorarioConnect, equipe.Equipe);
                var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                TimeSpan tempo_deslocamento = (documento.Latitude_Str != null && documento.Longitude_Str != null && referencia.Latitude != null && referencia.Longitude != null) ? GetTempoDeslocamento(config, distancia, ReferenciaDistancia.PrimeiroServico) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/

                List<AgendamentoHorasDisponiveis> horarios = configDoDia != null ?
                                                             GetHorarioEquipeDisponivel(this.config, documento,
                                                             tempo_deslocamento.TotalMinutes,
                                                             configDoDia,
                                                             ja_Agendados_Tecnico,
                                                             ja_Agendados,
                                                             this.listaAusencias) :
                                                             new List<AgendamentoHorasDisponiveis>();

                AgendamentoHorasDisponiveis janela = horarios.FirstOrDefault();

                if (janela != null && documento.Data_Hora_Vencimento_OS.HasValue)
                {
                    return janela.Hora_Inicio;
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
            else
            {
                return null;
            }
        }


    }
}