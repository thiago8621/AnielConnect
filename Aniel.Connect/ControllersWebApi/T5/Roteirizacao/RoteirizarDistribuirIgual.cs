using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class RoteirizarDistribuirIgual : RoteirizacaoBusiness, IRoterizar
    {
        public Configuracoes_Gerais config { get; set; }
        public RoteirizacaoDiaConf dia { get; set; }
        public List<Turno_ServicoAniel> turnos { get; set; }
        public Queue<TecnicoRoteirizar> tecnicos { get;set; }
        public List<Painel_Servicos_AgendamentosEquipe> listaAusencias { get; set; }

        public RoteirizarDistribuirIgual(Configuracoes_Gerais _config, RoteirizacaoDiaConf _dia, List<Turno_ServicoAniel> _turnos, Queue<TecnicoRoteirizar> _tecnicos)
        {
            dia = _dia;
            config = _config;
            turnos = _turnos;
            tecnicos = _tecnicos;
        }

        public (List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados) Run(List<DocumentoRoteirizar> documentos)
        {
            PrenchendoLocalizacaoConfigCaixa(this.config, documentos);
            List<DocumentoRoteirizar> documentosRestantes = new List<DocumentoRoteirizar>(documentos);
            List<DocumentoRoteirizar> documentosRoteirizados = new List<DocumentoRoteirizar>();
            var docAgrupado = AgrupaDocumentosCriterio(config, documentos).ToList();
            var ja_agendados = tecnicos.SelectMany(p => p.JaAgendados).ToList();

            if (this.config.RoteirizarSemLatLong == false)
            {
                foreach (var item in documentosRestantes.FindAll(p => String.IsNullOrEmpty(p.Latitude_Str) && String.IsNullOrEmpty(p.Longitude_Str)))
                {
                    item.Log = "Configuracao da empresa nao permite roteirizar sem latitude e longitude";
                }

            }

            for (int i = 1; i <= 3; i++)
            {

                while (documentosRestantes.Count > 0)
                {
                    var fila_tmp = new Queue<TecnicoRoteirizar>(tecnicos);
                    var NaoRoteirizadosTemp = new List<DocumentoRoteirizar>();
                    var RoteirizadosTemp = new List<DocumentoRoteirizar>();

                    while (fila_tmp.Count > 0)
                    {

                        var resultado = RunEquipe(docAgrupado, fila_tmp.Peek(), ja_agendados, documentosRestantes, ReferenciaDistancia.UltimoServico,i);
                        documentosRestantes = new List<DocumentoRoteirizar>(resultado.documentosNaoRoteirizados);
                        if (resultado.documentoRoteirizado != null)
                        {
                            resultado.documentoRoteirizado = AtualizarDocumentos(new List<DocumentoRoteirizar>() { resultado.documentoRoteirizado }, fila_tmp.Peek()).First();
                            RoteirizadosTemp.Add(resultado.documentoRoteirizado);
                            documentosRoteirizados.Add(resultado.documentoRoteirizado);
                        }

                        fila_tmp.Dequeue();

                    }



                    if (RoteirizadosTemp.Count == 0)
                        break;

                }
            }

            return (documentosRoteirizados, documentosRestantes);
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

                if (this.config.Considerar_Agendamento_Periodo)
                    p.CodTurnoServ = PeriodoApiController.BuscarTurno(this.turnos, p.Data_hora_ini_agendamento.Value, p.Tempo_Execucao)?.Id;

                return p;

            }).ToList();

            return roteirizados;
        }

        private (DocumentoRoteirizar documentoRoteirizado, List<DocumentoRoteirizar> documentosNaoRoteirizados) RunEquipe(List<Painel_ReteirizacaoDoc_Agrupado> docAgrupado, TecnicoRoteirizar equipe, List<Painel_Servicos_AgendamentosEquipe> ja_agendados, List<DocumentoRoteirizar> documentosRestantes, ReferenciaDistancia referenciaDistancia, Int32 nivelLocalidade)
        {
            List<DocumentoRoteirizar> vistosPelaEquipe = docAgrupado.Count() > 0 ? docAgrupado.Select(doc => FiltrarOrdensTecnicoCapaz(config, doc, equipe, documentosRestantes, nivelLocalidade)).SelectMany(p => p).ToList() : new List<DocumentoRoteirizar>(documentosRestantes);
            DocumentoRoteirizar documentoRoteirizado = null;
            List<DocumentoRoteirizar> documentosNaoRoteirizados = new List<DocumentoRoteirizar>(documentosRestantes.Where(p => !vistosPelaEquipe.Contains(p)));
            List<Painel_Servicos_AgendamentosEquipe> ja_Agendados_Tecnico = ja_agendados.Where(p => p.Equipe == equipe.Equipe).ToList();

            while (vistosPelaEquipe.Count() > 0 &&
                   ja_Agendados_Tecnico.Where(p => p.Intervalo == false).Select(p => p.Num_Doc).Distinct().Count() < dia.NumeroMaxAtendimentoTecnico &&
                   documentoRoteirizado == null)
            {
                var referencia = GetLatitudeLongitudeReferencia(equipe, ja_Agendados_Tecnico, referenciaDistancia);
                DocumentoRoteirizar documento = GetDocumento(referencia.Latitude, referencia.Longitude, vistosPelaEquipe, equipe, ja_Agendados_Tecnico,config);

                if (documento == null)
                    break;

                double distancia = Math.Ceiling(GetDistancias(referencia.Latitude, referencia.Longitude, new List<DocumentoRoteirizar>() { documento }).Values.First() / 1852);
                Double tempoExecucaoMinutos = documento.Tempo_Execucao.Value.TotalMinutes;
                AgendamentoDiaModel configDoDia = CarregaConfiguracoesDoDocumento(dia.Data, dia.feriado, equipe.HorarioConnect, equipe.Equipe);
                var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                TimeSpan tempo_deslocamento = (documento.Latitude_Str != null && documento.Longitude_Str != null && referencia.Latitude != null && referencia.Longitude != null) ? GetTempoDeslocamento(config, distancia, referenciaDistancia) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/
                List<AgendamentoHorasDisponiveis> horarios = configDoDia != null ?
                                                           GetHorarioEquipeDisponivel(this.config, documento,
                                                                                      tempo_deslocamento.TotalMinutes,
                                                                                      configDoDia,
                                                                                      ja_Agendados_Tecnico,
                                                                                      ja_agendados,
                                                                                      this.listaAusencias) :
                                                           new List<AgendamentoHorasDisponiveis>();






                if (ja_agendados.Count(p => p.Intervalo && p.Equipe == equipe.Equipe) == 0 && ja_Agendados_Tecnico.Count(p => p.Intervalo && p.Equipe == equipe.Equipe) > 0)
                {
                    ja_agendados.Add(ja_Agendados_Tecnico.Where(p => p.Intervalo && p.Equipe == equipe.Equipe).First());
                }


                if (horarios.Count > 0)
                {
                    Painel_Servicos_AgendamentosEquipe novoAgendamento = new Painel_Servicos_AgendamentosEquipe
                    {
                        Equipe = equipe.Equipe,
                        Data_Inicial = horarios[0].Hora_Inicio,
                        Data_Final = horarios[0].Hora_Final,
                        Deslocamento = horarios[0].Deslocamento,
                        CodCt = documento.CodCt,
                        Numero_Obra = documento.Num_Obra,
                        Num_Doc = documento.Num_Doc.GetValueOrDefault(0),
                        Projeto = documento.Projeto,
                        Latitude = documento.GetLatitude() ?? documento.Latitude_Localidade,
                        Longitude = documento.GetLongitude() ?? documento.Longitude_Localidade,
                        GrupoArea = documento.Grupo_Area,
                        Area = documento.Area,
                        CodLocalidade = Convert.ToString(documento.Cod_Localidade),
                        Localidade = documento.Localidade,
                    };

                    ja_agendados.Add(novoAgendamento);
                    ja_Agendados_Tecnico.Add(novoAgendamento);

                    if (!horarios[0].AgendamentoAntigo)
                    {
                        documento.Data_hora_ini_agendamento = novoAgendamento.Data_Inicial.Value;
                        documento.Data_hora_fim_agendamento = novoAgendamento.Data_Final.Value;
                        documento.DeslocamentoMinutos = (short)novoAgendamento.Deslocamento;
                    }
                    if (equipe.Equipe != documento.Equipe_Anterior && !String.IsNullOrEmpty(documento.Equipe_Anterior))
                    {
                        ja_agendados.RemoveAll(p => p.Equipe == documento.Equipe_Anterior &&
                                                    p.Numero_Obra == documento.Num_Obra &&
                                                    p.Num_Doc == documento.Num_Doc &&
                                                    p.Projeto == documento.Projeto &&
                                                    p.CodCt == documento.CodCt);
                    }

                    documentoRoteirizado = documento;
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
            return (documentoRoteirizado, documentosNaoRoteirizados);
        }







    }
}