using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Agendamento;
using Aniel.Connect.Repository.T5.Horario;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.T5.Turno;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Testing.Values;
using MongoDB.Driver;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class AgendamentoApiController : BaseApiController
    {
        List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)> perfilAgendamentoCache = new List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)>();

        [HttpPost]
        [Route("api/AgendamentoApiController/ConsultaDisponibilidadeAgendamento")]
        public async Task<ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>> ConsultaDisponibilidadeAgendamento(ConsultaDisponibilidadeAgendamentoApiRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                return await ConsultaDisponibilidadeAgendamentoBase(new ConsultaDisponibilidadeAgendamentoBaseRequest
                {
                    Area = request.Area,
                    DataInicio = request.Data,
                    DataFim = request.Data,
                    GrupoArea = request.GrupoArea,
                    Localidade = request.Localidade,
                    settings = request.settings,
                    TipoServico = request.TipoServico,
                    Contrato = request.Contrato
                });

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                return new ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>()
                {
                    data = null,
                    error = ex.Message
                };
            }
        }

        [HttpPost]
        [Route("api/AgendamentoApiController/ConsultaDisponibilidadeAgendamentoPeriodo")]
        public async Task<ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>> ConsultaDisponibilidadeAgendamento(ConsultaDisponibilidadeAgendamentoPeriodoApiRequest request)
        {
            ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse> retorno = null;
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            DateTime dataInicio;
            DateTime dataFim;
            DateTime.TryParse(request.DataInicio, out dataInicio);
            DateTime.TryParse(request.DataFim, out dataFim);

            if (dataInicio == null || dataInicio == DateTime.MinValue || dataInicio.Date < DateTime.Today)
            {
                throw new Exception("Data informada não é válida");
            }

            if (dataFim == null || dataFim == DateTime.MinValue || dataFim.Date < dataInicio.Date)
            {
                throw new Exception("Data informada não é válida");
            }

            try
            {
                foreach (DateTime data in DateTimeUtil.EachDay(dataInicio, dataFim))
                {
                    try
                    {
                        var resultado = await ConsultaDisponibilidadeAgendamentoBase(new ConsultaDisponibilidadeAgendamentoBaseRequest
                        {
                            Area = request.Area,
                            DataInicio = data.ToString("dd/MM/yyyy"),
                            DataFim = data.ToString("dd/MM/yyyy"),
                            GrupoArea = request.GrupoArea,
                            Localidade = request.Localidade,
                            settings = request.settings,
                            TipoServico = request.TipoServico,
                            Contrato = request.Contrato

                        }, false);

                        if (retorno == null)
                        {
                            retorno = resultado;
                        }
                        else
                        {
                            retorno.data = retorno.data.Concat(resultado.data);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = new ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>
                {
                    error = ex.Message
                };
            }

            return retorno;
        }



        private async Task<ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>> ConsultaDisponibilidadeAgendamentoBase(ConsultaDisponibilidadeAgendamentoBaseRequest request, Boolean exibeDiasHoras = true)
        {
            List<ConsultaDisponibilidadeAgendamentoApiResponse> retorno = new List<ConsultaDisponibilidadeAgendamentoApiResponse>();

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var session = HibernateUtil.GetSession())
            using (var repositoryEscritorio = new Conexao<EscritorioAniel>(sessionT5))
            using (var repositoryArea = new Conexao<AreaAniel>(sessionT5))
            using (var repositoryLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
            using (var repositoryTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
            using (var repoFeriado = new Conexao<Feriado>(session))
            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repo = new Conexao<Horario_Connect>(sessionT5))
            using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
            using (var repositoryLimiteAgendamentoAniel = new Conexao<LimiteAgendamentoAniel>(sessionT5))
            {
                IAgendamentoRepository repoAgendamento = new AgendamentoRepository(sessionT5, sessionT5);
                ILimiteAgendamentoAnielRepository repoLimAgendamento = new LimiteAgendamentoRepository(sessionT5, sessionT5);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IHorarioAnielRepository repositoryHorario = new HorarioAnielRepository(sessionT5);
                ITurnoAnielRepository repoTurno = new TurnoAnielRepository(sessionT5);
                var limiteAgendamentoBusiness = new LimiteAgendamentoAnielBusiness(repoLimAgendamento, repoNatureza);
                AgendamentoAnielBusiness agendamentoAnielBusiness = new AgendamentoAnielBusiness(repoAgendamento, repoLimAgendamento,
                                                                                                 repoNatureza, repoConfig, repositoryHorario,
                                                                                                 repoTurno);

                EscritorioAniel escritorio = repositoryEscritorio.QueryableSelect()
                                                                                  .Where(p => p.Descricao != null &&
                                                                                                p.Descricao.ToUpperInvariant() == request.GrupoArea.ToUpperInvariant() &&
                                                                                                p.Status == Status_Default.Ativo)
                                                                                  .FirstOrDefault();

                List<Turno_ServicoAniel> turnosBanco = repositoryTurno.select(p => p.Status == Status_Default.Ativo).Result.ToList();

                if (escritorio == null)
                    throw new Exception("GrupoArea não Encontrado");

                AreaAniel area = repositoryArea.QueryableSelect().Where(p => p.Cod_Escritorio.ToUpperInvariant() == escritorio.Cod_Escritorio.ToUpperInvariant() &&
                                                                                        p.Descricao != null &&
                                                                                        p.Descricao.ToUpperInvariant() == request.Area.ToUpperInvariant() &&
                                                                                        p.Status == Status_Default.Ativo).FirstOrDefault();

                if (area == null)
                    throw new Exception("Area não Encontrada");

                LocalidadeAniel localidade = repositoryLocalidade.QueryableSelect()
                                                                       .Where(p => p.Cod_Escritorio.ToUpperInvariant() == escritorio.Cod_Escritorio.ToUpperInvariant() &&
                                                                                     p.Cod_Area.ToUpperInvariant() == area.Cod_Area.ToUpperInvariant() &&
                                                                                     p.LocalidadeStr != null &&
                                                                                     p.LocalidadeStr.ToUpperInvariant() == request.Localidade.ToUpperInvariant() &&
                                                                                     p.Status == Status_Default.Ativo).FirstOrDefault();

                if (localidade == null)
                    throw new Exception("Localidade não Encontrada");

                Tipo_Servico_EquipeAniel tipoServico = null;
                 if (String.IsNullOrEmpty(request.Contrato))
                {
                    tipoServico = repositoryTipoServico.QueryableSelect().ToList()
                                                                                   .FindAll(p => p.Descricao != null &&
                                                                                                 p.Descricao.ToUpperInvariant() == request.TipoServico.ToUpperInvariant())
                                                                                   .FirstOrDefault();
                }
                else
                {
                    tipoServico = repositoryTipoServico.QueryableSelect().ToList()
                                                                                   .FindAll(p => p.Descricao != null &&
                                                                                                 p.Descricao.ToUpperInvariant() == request.TipoServico.ToUpperInvariant() &&
                                                                                                 p.CodCt == request.Contrato)
                                                                                   .FirstOrDefault();
                }

                if (tipoServico == null)
                    throw new Exception("Tipo Serviço não Encontrada");


                var listaLimites = limiteAgendamentoBusiness.GetPercNaturezaByArea(localidade.Cod_Escritorio,
                                                                                   localidade.Cod_Area,
                                                                                   tipoServico.Cod_Natureza.Value);



                DateTime dataInicio;
                DateTime dataFim;
                DateTime.TryParse(request.DataInicio, out dataInicio);
                DateTime.TryParse(request.DataFim, out dataFim);

                if (dataInicio == null || dataInicio == DateTime.MinValue || dataInicio.Date < DateTime.Today)
                {
                    throw new Exception("Data informada não é válida");
                }

                if (dataFim == null || dataFim == DateTime.MinValue || dataFim.Date < dataInicio.Date)
                {
                    throw new Exception("Data informada não é válida");
                }
                ConsultaDisponibilidadeAgendamentoApiResponse datas = new ConsultaDisponibilidadeAgendamentoApiResponse();

                List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico).Result).ToList();
                List<Feriado> feriados = (await repoFeriado.select(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Data >= dataInicio && p.Data <= dataFim)).ToList();
                var resultadoAgendamento = new AgendamentoDiaModel();
                foreach (DateTime data in DateTimeUtil.EachDay(dataInicio, dataFim))
                {
                    LimiteAgendamentoAniel limAgendData = null;
                    if (tipoServico.Cod_Natureza.HasValue)
                        limAgendData = listaLimites.FirstOrDefault(p => p.DataInicio <= data && data <= p.DataFim);

                    Horario_Connect horario_servico_padrao = horarios_servico.Find(p => p.Padrao == true);
                    bool feriado = feriados.Find(p => p.Data == data) != null;
                    Configuracoes_Gerais config = await repoConfigGerais.selectFirst();
                    List<AgendamentoDadosModel> jaAgendados = await GetJaAgendados(sessionT5, data, data.AddDays(0));
                    List<AgendamentoEquipeHorarioModel> forcaTrabalho = await GetForcaTrabalho(sessionT5, data, config);

                    Horario_Connect horarioAgendamento = await repo.selectFirst(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Padrao == true && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento);

                    var tempoExecucao = data.Add(TempoServico(sessionT5, (int)tipoServico.Cod_Tipo_Serv, null, config).Result.valor.Value);
                    Double tempoDeslocamentoMinutos = ((config.Tempo_Medio_Deslocamento.Value.Hour * 60) + (config.Tempo_Medio_Deslocamento.Value.Minute));

                    resultadoAgendamento = agendamentoAnielBusiness.BuscarAgendamentoDia(data,
                        jaAgendados,
                        forcaTrabalho,
                        horarios_servico,
                        horarioAgendamento,
                        config,
                        feriado,
                        new DadosServicoAgendamento
                        {
                            Cod_Area = area.Cod_Area,
                            Cod_Escritorio = escritorio.Cod_Escritorio,
                            Cod_Localidade = localidade.Cod_Localidade,
                            Cod_Natureza = tipoServico.Cod_Natureza,
                            LocalidadeStr = localidade.LocalidadeStr,
                            TempoExecucaoMinutos = tempoExecucao
                        },
                        turnosBanco,
                        limAgendData,
                        false,
                        true);

                    if (resultadoAgendamento == null)
                    {
                        continue;
                    }
                    resultadoAgendamento._diasHoras = new List<string>();
                    List<AgendamentoHorasDisponiveis> horasAgrupadas = new List<AgendamentoHorasDisponiveis>();
                    foreach (var item in resultadoAgendamento.HorasCapacidade.OrderBy(p => p.Hora_Inicio).GroupBy(p => new { p.Hora_Inicio, p.Hora_Final, p.IdPerfil }))
                    {
                        var itemAgrupado = item.FirstOrDefault(p => p.Equipe_Tmp != null) ?? item.FirstOrDefault();
                        itemAgrupado.Aptos = item.Sum(p => p.Aptos);
                        itemAgrupado.AptosPorLimite = item.Sum(p => p.AptosPorLimite);
                        itemAgrupado.Forca_Trabalho = item.Sum(p => p.Forca_Trabalho);
                        itemAgrupado.ForcaTrabalhoLimiteAgendamento = item.Sum(p => p.ForcaTrabalhoLimiteAgendamento);
                        itemAgrupado.Lista_Ja_Agendados = item.ToList().SelectMany(p => p.Lista_Ja_Agendados).ToList();
                        itemAgrupado.Lista_Forca_Trabalho = item.ToList().SelectMany(p => p.Lista_Forca_Trabalho).ToList();
                        itemAgrupado.ListaForcaTrabalhoComLimite = item.ToList().SelectMany(p => p.ListaForcaTrabalhoComLimite).ToList();
                        itemAgrupado.ForcaTrabalhoMinutosPrevistos = item.Sum(p => p.ForcaTrabalhoMinutosPrevistos);
                        itemAgrupado.ForcaTrabalhoMinutosPrevistosComLimite = item.Sum(p => p.ForcaTrabalhoMinutosPrevistosComLimite);
                        horasAgrupadas.Add(itemAgrupado);
                    }

                    resultadoAgendamento.HorasCapacidade = horasAgrupadas;
                    var dat = new List<string>();
                    foreach (var item in horasAgrupadas)
                    {
                        resultadoAgendamento._diasHoras.Add(item.Hora_Inicio.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    }
                    if (config.Considerar_Agendamento_Periodo)
                    {
                        foreach (var item in resultadoAgendamento.Horas)
                        {
                            var turno = BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.TimeOfDay);
                            item.Turno = turno?.Descricao;

                            if (turno?.Hora_Ini != null && turno?.Hora_Fim != null)
                            {
                                item.InicioTurno = turno.Hora_Ini.Value.ToString("HH:mm");
                                item.FimTurno = turno.Hora_Fim.Value.ToString("HH:mm");
                            }

                        }

                        foreach (var item in resultadoAgendamento.HorasCapacidade)
                        {
                            item.Turno = BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.TimeOfDay)?.Descricao;
                            var turno = BuscarTurno(turnosBanco, item.Hora_Inicio, tempoExecucao.TimeOfDay);
                            item.Turno = turno?.Descricao;

                            if (turno?.Hora_Ini != null && turno?.Hora_Fim != null)
                            {
                                item.InicioTurno = turno.Hora_Ini.Value.ToString("HH:mm");
                                item.FimTurno = turno.Hora_Fim.Value.ToString("HH:mm");
                            }
                        }

                        var turnosPerfil = resultadoAgendamento.HorasCapacidade.GroupBy(p => new { p.Turno, p.IdPerfil });
                        resultadoAgendamento.HorasTurno = turnosPerfil.Select(p =>
                        {
                            var forcaTrabalhoPerfilTmp = resultadoAgendamento.Horas.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);
                            var forcaTrabalhoPerfilCapacidadeTecnicoTmp = resultadoAgendamento.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil);

                            var forcaTrabalhoComLimite = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistosComLimite);
                            var forcaTrabalho = forcaTrabalhoPerfilCapacidadeTecnicoTmp.Sum(y => y.ForcaTrabalhoMinutosPrevistos);
                            var listaNumOS = forcaTrabalhoPerfilTmp.SelectMany(y => y.Lista_Ja_Agendados).ToList()
                                                                       .Where(y => y.DadosServico.Periodo == p.Key.Turno)
                                                                       .Select(y => y.DadosServico.NumObra).Distinct();
                            var agendamentosForaFaixaNoMesmoTurnoMinutos = resultadoAgendamento.ListaJaAgendadosForaFaixa.FindAll(y => p.Key.IdPerfil == y.IdPerfil && BuscarTurno(turnosBanco, y.DataHoraInicio, tempoExecucao.TimeOfDay)?.Descricao == p.Key.Turno).Sum(p => p.DuracaoMinutos);
                            var agendamentosManuaisMinutos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem == OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);
                            var agendamentosMinutos = listaNumOS.Select(y => forcaTrabalhoPerfilTmp.SelectMany(z => z.Lista_Ja_Agendados).FirstOrDefault(z => z.DadosServico.NumObra == y)).Where(y => y.Origem != OrigemAgendamentoOS.Manual).Sum(p => p.DuracaoMinutos);

                            if (forcaTrabalhoPerfilTmp.FindAll(p => p.Deslocamento == 0).Count > 0)
                            {
                                double minutosExtrasDeslocamento = 0;
                                foreach (var item in forcaTrabalhoPerfilTmp.FindAll(p => p.Deslocamento == 0))
                                {
                                    minutosExtrasDeslocamento = item.Lista_Forca_Trabalho.Count * tempoDeslocamentoMinutos;
                                    forcaTrabalho += minutosExtrasDeslocamento;
                                    forcaTrabalhoComLimite += minutosExtrasDeslocamento;
                                }
                            }

                            int capacidadePeriodoBaseadoEmTempo = Convert.ToInt32(Math.Floor((forcaTrabalho - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucao.TimeOfDay.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));
                            int capacidadePeriodo = resultadoAgendamento.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(p => p.Lista_Forca_Trabalho.Count);

                            int capacidadePeriodoBaseadoEmTempoComLimite = Convert.ToInt32(Math.Floor((forcaTrabalhoComLimite - (agendamentosManuaisMinutos + agendamentosForaFaixaNoMesmoTurnoMinutos)) / (tempoExecucao.TimeOfDay.TotalMinutes + config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes)));
                            int capacidadePeriodoComLimite = resultadoAgendamento.HorasCapacidade.FindAll(y => y.Turno == p.Key.Turno && y.IdPerfil == p.Key.IdPerfil).Sum(p => p.ListaForcaTrabalhoComLimite.Count);

                            if (capacidadePeriodoBaseadoEmTempo < capacidadePeriodo)
                            {
                                capacidadePeriodo = capacidadePeriodoBaseadoEmTempo;
                            }

                            if (capacidadePeriodoBaseadoEmTempoComLimite < capacidadePeriodoComLimite)
                            {
                                capacidadePeriodoComLimite = capacidadePeriodoBaseadoEmTempoComLimite;
                            }

                            var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null);

                            if (horariosEquipesTemp.FirstOrDefault() == null)
                                return null;
                            return new AgendamentoHorasDisponiveis
                            {
                                Display = p.Key.Turno,
                                Aptos = (long)capacidadePeriodo,
                                AptosPorLimite = (long)capacidadePeriodoComLimite,
                                Turno = p.Key.Turno,
                                Equipe_Tmp = horariosEquipesTemp.FirstOrDefault()?.Equipe_Tmp,
                                Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                                Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                                Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                                Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                                Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                                InicioTurno = horariosEquipesTemp.FirstOrDefault()?.InicioTurno,
                                FimTurno = horariosEquipesTemp.FirstOrDefault()?.FimTurno,
                                LimiteAgendamentoUltrapassado = horariosEquipesTemp.FirstOrDefault()?.LimiteAgendamentoUltrapassado ?? false
                            };

                        }).Where(p => p != null && p.Aptos >= 0 && p.Display != null).ToList();

                        var turnos = resultadoAgendamento.HorasTurno.GroupBy(p => new { p.Turno });
                        resultadoAgendamento.HorasTurno = turnos.Select(p =>
                        {
                            var horariosEquipesTemp = p.ToList().Where(y => y.Equipe_Tmp != null);
                            return new AgendamentoHorasDisponiveis
                            {
                                Display = p.Key.Turno,
                                Aptos = p.ToList().Sum(p => p.Aptos),
                                AptosPorLimite = p.ToList().Sum(p => p.AptosPorLimite),
                                Turno = p.Key.Turno,
                                Equipe_Tmp = horariosEquipesTemp.FirstOrDefault()?.Equipe_Tmp,
                                Hora_InicioStr = horariosEquipesTemp.FirstOrDefault()?.Hora_InicioStr,
                                Hora_FinalStr = horariosEquipesTemp.FirstOrDefault()?.Hora_FinalStr,
                                Hora_Inicio = horariosEquipesTemp.FirstOrDefault()?.Hora_Inicio ?? DateTime.MinValue,
                                Hora_Final = horariosEquipesTemp.FirstOrDefault()?.Hora_Final ?? DateTime.MinValue,
                                Deslocamento = horariosEquipesTemp.FirstOrDefault()?.Deslocamento ?? 0,
                                InicioTurno = horariosEquipesTemp.FirstOrDefault()?.InicioTurno,
                                FimTurno = horariosEquipesTemp.FirstOrDefault()?.FimTurno,
                                LimiteAgendamentoUltrapassado = horariosEquipesTemp.FirstOrDefault()?.LimiteAgendamentoUltrapassado ?? false
                            };

                        }).Where(p => p != null && p.Display != null).ToList();

                        retorno.AddRange(resultadoAgendamento.HorasTurno.Select(p => new ConsultaDisponibilidadeAgendamentoApiResponse
                        {
                            dia = data.ToString("dd/MM/yyyy"),
                            Horario = p.Display,
                            Disponivel = true,
                            AgendamentoModelDetalhes = p,
                            DiasHoras = resultadoAgendamento._diasHoras.Where(y => BuscarTurno(turnosBanco, Convert.ToDateTime(y), tempoExecucao.TimeOfDay).Descricao == p.Display).ToList()
                        }).ToList());
                    }
                    else
                    {
                        foreach (var item in horasAgrupadas)
                        {
                            resultadoAgendamento._diasHoras.Add(item.Hora_Inicio.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                        }
                        resultadoAgendamento.Horas = resultadoAgendamento.Horas.Where(p => p.Aptos > 0).ToList();
                        var horarios = resultadoAgendamento.Horas.GroupBy(p => new { p.Hora_InicioStr, p.Hora_FinalStr, p.Hora_Inicio, p.Hora_Final, p.Display, p.IdPerfil });
                        resultadoAgendamento.Horas = horarios.Select(p =>
                        {
                            var forcaTrabalho = p.ToList().Sum(y => y.Forca_Trabalho);
                            var listaNumOS = p.ToList().SelectMany(y => y.Lista_Ja_Agendados).ToList().Select(y => y.DadosServico.NumObra).Distinct();
                            var agendamentos = listaNumOS.Select(y => p.ToList().SelectMany(z => z.Lista_Ja_Agendados).First(z => z.DadosServico.NumObra == y));
                            var capacidadePeriodo = forcaTrabalho - agendamentos.Count();

                            return new AgendamentoHorasDisponiveis
                            {
                                Display = p.Key.Display,
                                Aptos = capacidadePeriodo,
                                Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(p => p != null).FirstOrDefault(),
                                Hora_InicioStr = p.Key.Hora_InicioStr,
                                Hora_FinalStr = p.Key.Hora_FinalStr,
                                Hora_Inicio = p.Key.Hora_Inicio,
                                Hora_Final = p.Key.Hora_Final,

                            };

                        }).Where(p => p.Aptos >= 0 && p != null && p.Display != null).ToList();

                        var agrupamento = resultadoAgendamento.Horas.GroupBy(p => new { p.Hora_InicioStr, p.Hora_FinalStr, p.Hora_Inicio, p.Hora_Final, p.Display });
                        resultadoAgendamento.Horas = agrupamento.Select(p =>
                        {
                            return new AgendamentoHorasDisponiveis
                            {
                                Display = p.Key.Display,
                                Aptos = p.ToList().Sum(p => p.Aptos),
                                AptosPorLimite = p.ToList().Sum(p => p.AptosPorLimite),
                                Equipe_Tmp = p.ToList().Select(y => y.Equipe_Tmp).Where(p => p != null).FirstOrDefault(),
                                Hora_InicioStr = p.Key.Hora_InicioStr,
                                Hora_FinalStr = p.Key.Hora_FinalStr,
                                Hora_Inicio = p.Key.Hora_Inicio,
                                Hora_Final = p.Key.Hora_Final,
                            };

                        }).Where(p => p != null && p.Display != null).ToList();

                        retorno.AddRange(resultadoAgendamento.Horas.Select(p => new ConsultaDisponibilidadeAgendamentoApiResponse
                        {
                            dia = data.ToString("dd/MM/yyyy"),
                            Horario = p.Display,
                            Disponivel = true,
                            AgendamentoModelDetalhes = p,
                            DiasHoras = exibeDiasHoras ? resultadoAgendamento._diasHoras : null
                        }).ToList());
                    }

                    retorno = retorno.FindAll(p => !p.AgendamentoModelDetalhes.LimiteAgendamentoUltrapassado);


                }
                if (retorno.Count == 0)
                {
                    throw new Exception("Não foi encontrada disponibilidade de agendamento.");
                }

                if (retorno.Where(p => !p.AgendamentoModelDetalhes.LimiteAgendamentoUltrapassado).Count() == 0)
                {
                    throw new Exception("Limite de agendamento ultrapassado.");
                }

                return new ApiResponse<ConsultaDisponibilidadeAgendamentoApiResponse>()
                {
                    draw = request.settings.length,
                    error = null,
                    data = retorno
                };
            }
        }

        private async Task<List<AgendamentoEquipeHorarioModel>> GetForcaTrabalho(ISession sessionT5, DateTime dia, Configuracoes_Gerais config)
        {

            var _temCriterioArea = config.Criterio_Area;
            var _temCriterioHabilidades = config.Criterio_Habilidade;
            var _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;
            var _Utiliza_AnielPoint = config.UtilizaPoint_RoteirizacaoAgendamento;
            List<AgendamentoEquipeHorarioModel> equipeshorarios = new List<AgendamentoEquipeHorarioModel>();

            using (var sessionT5NH = HibernateUtil.GetSessionT5())
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
            using (var repositoryTipoServicoEquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
            using (var com = sessionT5.Connection.CreateCommand())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var sql = new StringBuilder(@"SELECT B.EQUIPE, B.NOME,B.DEVICE_ID, H.LATITUDE, H.LONFITUDE,SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR, SUPERV.NOME AS NOME_SUPERVISOR,B.USUARIO_MONITOR,FUNCIONARIO_MONITOR.NOME NOME_MONITOR,
                                                        B.ID_PERFIL_AGENDAMENTO,F.HORARIO_ATRIBUIDO,");

                sql.AppendLine("(SELECT FIRST 1 F.ID_PERFIL_AGENDAMENTO")
                    .AppendLine(" FROM TB_HORARIO_EQUIPE_CONNECT F")
                    .AppendLine("LEFT JOIN TB_HORARIO_CONNECT G")
                    .AppendLine("ON(F.ID_HORARIO = G.ID)")
                    .AppendLine("WHERE F.EQUIPE = B.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = @DATA) OR")
                    .AppendLine("(F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA))")
                    .AppendLine("ORDER BY F.DATA DESC) AS ID_PERFIL_AGENDAMENTO_HORARIO,");

                sql.AppendLine("(SELECT FIRST 1 F.ID")
                    .AppendLine(" FROM TB_HORARIO_EQUIPE_CONNECT F")
                    .AppendLine("LEFT JOIN TB_HORARIO_CONNECT G")
                    .AppendLine("ON(F.ID_HORARIO = G.ID)")
                    .AppendLine("WHERE F.EQUIPE = B.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = @DATA) OR")
                    .AppendLine("(F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA))")
                    .AppendLine("ORDER BY F.DATA DESC) AS ID_HORARIO_EQUIPE,");

                sql.AppendLine("(SELECT FIRST 1 G.ID")
                .AppendLine(" FROM TB_HORARIO_EQUIPE_CONNECT F")
                .AppendLine("LEFT JOIN TB_HORARIO_CONNECT G")
                .AppendLine("ON(F.ID_HORARIO = G.ID)")
                .AppendLine("WHERE F.EQUIPE = B.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = @DATA) OR")
                .AppendLine("(F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA))")
                .AppendLine("ORDER BY F.DATA DESC)");
                sql.AppendLine(" AS ID_HORARIO_CONNECT, ");

                sql.AppendLine("(SELECT FIRST 1 COALESCE(B.ID_HORARIO_CONNECT, D.ID_HORARIO_CONNECT) FROM TB_PT_DIA_FUNCIONARIO A");
                sql.AppendLine("LEFT JOIN TB_PT_JORNADA_FUNCIONARIO B ON(A.ID_JORNADA  = B.ID)");
                sql.AppendLine("LEFT JOIN TB_FUNCIONARIO C ON(C.NUM_FRE = A.NUM_FRE)");
                sql.AppendLine("LEFT JOIN TB_PT_JORNADA_FUNCIONARIO D ON(C.ID_JORNADA = D.ID)");
                sql.AppendLine("WHERE A.NUM_FRE = F.NUM_FRE AND A.DATA = @DATA)");
                sql.AppendLine(" AS ID_HORARIO_CONNECT_POINT, ");


                sql.AppendLine("(SELECT FIRST 1 H.ID");
                sql.AppendLine("FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("LEFT JOIN TB_HORARIO_CONNECT I");
                sql.AppendLine("ON(H.ID_HORARIO = I.ID)");
                sql.AppendLine("WHERE H.EQUIPE = B.EQUIPE AND H.APENAS_NO_DIA = 1");
                sql.AppendLine("AND H.DATA = @DATA AND H.INDISPONIVEL = 1)");
                sql.AppendLine(" AS INDISPONIVEL_CONNECT, ");

                sql.AppendLine("   (SELECT FIRST 1 A.NUM_FRE FROM TB_FUNCIONARIO A")
                        .AppendLine("   INNER JOIN TB_PT_DIA_FUNCIONARIO B ON(A.NUM_FRE = B.NUM_FRE)")
                        .AppendLine("   LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA C ON(B.ID_EVENTO = C.ID)")
                        .AppendLine("   WHERE C.PERMTITE_ROTEIRIZAR = 0 AND A.NUM_FRE = F.NUM_FRE AND B.DATA = @DATA)");

                sql.AppendLine(" AS INDISPONIVEL_POINT ");

                sql.AppendLine(@"FROM TB_EQUIPE B
                                INNER JOIN TB_FUNCIONARIO F
                                ON(B.NUM_FRE = F.NUM_FRE)
                                LEFT JOIN TB_PONTO_PARTIDA H
                                ON(B.ID_PONTO_PARTIDA = H.ID)
                                LEFT JOIN TB_EQUIPE_STATUS J
                                ON(B.EQUIPE = J.EQUIPE)
                                LEFT JOIN TB_FUNCIONARIO SUPERV
                                ON(SUPERV.NUM_FRE = B.COD_SUPERV) 
                                LEFT JOIN TB_SENHA USUARIO_MONITOR
                                ON(USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR) 
                                LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR
                                ON(FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE) 

                                WHERE(F.SITUACAO = 0 OR F.SITUACAO = 4) AND B.EQUIPE_CONNECT = 'S'");

                if (dia.Date == DateTime.Today)
                    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");

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
                                localidades = idPerfil == null ? new List<LocalidadeRoteirizacao>() : GetLocalidadeRoteirizacaoByPerfil(sessionT5, idPerfil.Value);
                                naturezas = idPerfil == null ? new List<Natureza_ProducaoAniel>() : (await repositoryNatureza.select(p => p.PerfilAgendamento.Id == idPerfil)).Select(p => p?.Natureza).ToList();
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

        private List<LocalidadeRoteirizacao> GetLocalidadeRoteirizacaoByPerfil(ISession sessionT5, long IdPerfil)
        {

            List<LocalidadeRoteirizacao> retorno = new List<LocalidadeRoteirizacao>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                sql.AppendLine("SELECT A.COD_ESCRITORIO,A.COD_AREA,A.LOCALIDADE,D.COD_LOCALIDADE,B.DESCRICAO AS DESCRICAO_ESCRITORIO,C.DESCRICAO AS DESCRICAO_AREA,A.NIVEL,A.TODAS_LOCALIDADES,A.TODAS_AREAS");
                sql.AppendLine("FROM TB_PERFIL_AGENDAMENTO_AREA A");
                sql.AppendLine("LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)");
                sql.AppendLine("LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)");
                sql.AppendLine("LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.LOCALIDADE = D.LOCALIDADE)");
                sql.AppendLine("WHERE A.ID_PERFIL_AGENDAMENTO = @ID_PERFIL");
                parametros.Add("@ID_PERFIL", IdPerfil);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, com.Transaction).Result)
                {
                    while (reader.ReadAsync().Result)
                    {
                        retorno.Add(new LocalidadeRoteirizacao
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

        private async Task<List<AgendamentoDadosModel>> GetJaAgendados(ISession sessionT5, DateTime dataInicio, DateTime dataFim)
        {

            List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO
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
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO
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
                            Origem = origem,
                            Capaz = false
                        });
                    }
                }
            }

            return JaAgendados;
        }

        private Turno_ServicoAniel BuscarTurno(List<Turno_ServicoAniel> turnos, DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);
            return turno;
        }

        private async Task<Response<TimeSpan?>> TempoServico(ISession sessionT5, int CodTipoServ, long? IdTipoImovel, Configuracoes_Gerais config)
        {
            TimeSpan? tempoMinimo = (TimeSpan?)null;

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT FIRST 1 M.HORAS ")
                    .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                    .AppendLine("WHERE (M.ID_TIPO_SERVICO = @COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC  ");
                    parametros.Add("@COD_TIPO_SERV", CodTipoServ);

                    if (IdTipoImovel.HasValue)
                    {
                        sql.AppendLine(" M.ID_TIPO_IMOVEL = @ID_TIPO_IMOVEL");
                        parametros.Add("@ID_TIPO_IMOVEL", IdTipoImovel);
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            tempoMinimo = reader["HORAS"] != DBNull.Value ? (TimeSpan)(reader["HORAS"]) : config.Tempo_Execucao_Padrao.Value.TimeOfDay;
                        }
                        else
                        {
                            tempoMinimo = config.Tempo_Execucao_Padrao.Value.TimeOfDay;
                        }
                    }
                }

                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    result = true,
                };

            }

            catch (Exception ex)
            {
                return new Response<TimeSpan?>
                {
                    valor = tempoMinimo,
                    message = ex.Message,
                    stacktrace = ex.StackTrace,
                    result = false,
                    exception = ex.Message

                };
            }
        }

        [HttpPost]
        [Route("api/AgendamentoApiController/ConsultaDisponibilidadeAgendamento/Days")]
        public async Task<ApiResponseScheduling<List<string>>> ConsultaDisponibilidadeAgendamentoDays(ConsultaDisponibilidadeAgendamentoDaysRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                List<string> dias = new List<string>();
                var diasHoras = await ConsultaDisponibilidadeAgendamentoBase(new ConsultaDisponibilidadeAgendamentoBaseRequest
                {
                    Area = request.Area,
                    DataInicio = request.DataInicio,
                    DataFim = request.DataFim,
                    GrupoArea = request.GrupoArea,
                    Localidade = request.Localidade,
                    settings = request.settings,
                    TipoServico = request.TipoServico,
                    Contrato = request.Contrato

                });

                foreach (var item in diasHoras.data)
                {
                    var dia = Convert.ToDateTime(item.DiasHoras.FirstOrDefault()).Date.ToString("yyyy-MM-dd");
                    if (!dias.Contains(dia))
                    {
                        dias.Add(dia);
                    }
                }

                return new ApiResponseScheduling<List<string>>
                {
                    success = true,
                    data = new List<IEnumerable<string>> { dias }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseScheduling<List<string>>
                {
                    success = false,
                    data = null,
                    message = ex.Message
                };
            }
        }

        [HttpPost]
        [Route("api/AgendamentoApiController/ConsultaDisponibilidadeAgendamento/Nearest")]
        public async Task<ApiResponseScheduling<List<string>>> ConsultaDisponibilidadeAgendamentoNearest(ConsultaDisponibilidadeAgendamentoNearestRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                List<string> dias = new List<string>();
                var diasHoras = await ConsultaDisponibilidadeAgendamentoBase(new ConsultaDisponibilidadeAgendamentoBaseRequest
                {
                    Area = request.Area,
                    DataInicio = request.DataInicio,
                    DataFim = request.DataFim,
                    GrupoArea = request.GrupoArea,
                    Localidade = request.Localidade,
                    settings = request.settings,
                    TipoServico = request.TipoServico,
                    Contrato = request.Contrato

                });

                foreach (var item in diasHoras.data)
                {
                    var dia = Convert.ToDateTime(item.DiasHoras.FirstOrDefault()).ToString("yyyy-MM-ddTHH:mm:ss");
                    if (!dias.Contains(dia))
                    {
                        dias.Add(item.DiasHoras.FirstOrDefault());
                        break;
                    }
                }

                return new ApiResponseScheduling<List<string>>
                {
                    success = true,
                    data = new List<IEnumerable<string>> { dias }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseScheduling<List<string>>
                {
                    success = false,
                    data = null,
                    message = ex.Message
                };
            }
        }

        [HttpPost]
        [Route("api/AgendamentoApiController/ConsultaDisponibilidadeAgendamento/Times")]
        public async Task<ApiResponseScheduling<List<TimesApiResponse>>> ConsultaDisponibilidadeAgendamentoTimes(ConsultaDisponibilidadeAgendamentoTimesRequest request)
        {
            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                List<TimesApiResponse> dias = new List<TimesApiResponse>();
                var diasHoras = await ConsultaDisponibilidadeAgendamentoBase(new ConsultaDisponibilidadeAgendamentoBaseRequest
                {
                    Area = request.Area,
                    DataInicio = request.Data,
                    DataFim = request.Data,
                    GrupoArea = request.GrupoArea,
                    Localidade = request.Localidade,
                    settings = request.settings,
                    TipoServico = request.TipoServico,
                    Contrato = request.Contrato

                });

                foreach (var item in diasHoras.data)
                {
                    //var dia = Convert.ToDateTime(item.DiasHoras).ToString();
                    if (!dias.Select(p => p.value).Contains(item.AgendamentoModelDetalhes.Display))
                    {
                        dias.Add(new TimesApiResponse
                        {
                            value = item.AgendamentoModelDetalhes.Display,
                            description = !String.IsNullOrEmpty(item.AgendamentoModelDetalhes.Turno) ?
                                          $"{item.AgendamentoModelDetalhes.InicioTurno} -> {item.AgendamentoModelDetalhes.FimTurno}" :
                                          $"{item.AgendamentoModelDetalhes.Hora_InicioStr} -> {item.AgendamentoModelDetalhes.Hora_FinalStr}"
                        });
                    }
                }

                return new ApiResponseScheduling<List<TimesApiResponse>>
                {
                    success = true,
                    data = new List<IEnumerable<TimesApiResponse>> { dias }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseScheduling<List<TimesApiResponse>>
                {
                    success = false,
                    data = null,
                    message = ex.Message
                };
            }
        }
    }
}