using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Agendamento;
using Aniel.Connect.Repository.T5.Area;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Horario;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.T5.PainelPlanejamentoCapacidade;
using Aniel.Connect.Repository.T5.Turno;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class PainelPlanejamentoCapacidadeAnielBusiness
    {
        private readonly IPainelPlanejamentoCapacidadeAnielRepository _repoPainel;
        private readonly ILimiteAgendamentoAnielRepository _repoLimAgend;
        private readonly IDocumentoProducaoAnielRepository _repoDocProd;
        private readonly IConfiguracoesGeraisRepository _repoConfig;
        private readonly IAreaAnielRepository _repoArea;
        private readonly IAgendamentoRepository _repoAgendamento;
        private readonly INaturezaAnielRepository _repoNatureza;
        private readonly IConfiguracoesGeraisRepository _repoConfiguracoesGerais;
        private readonly IHorarioAnielRepository _repoHorario;
        private readonly ITurnoAnielRepository _repoTurno;

        public PainelPlanejamentoCapacidadeAnielBusiness(
            IPainelPlanejamentoCapacidadeAnielRepository repoPainel,
            ILimiteAgendamentoAnielRepository repoLimAgend,
            IDocumentoProducaoAnielRepository repoDocProd,
            IConfiguracoesGeraisRepository repoConfig,
            IAreaAnielRepository repoArea,
            IAgendamentoRepository repoAgendamento,
            INaturezaAnielRepository repoNatureza,
            IConfiguracoesGeraisRepository repoConfiguracoesGerais,
            IHorarioAnielRepository repoHorario,
            ITurnoAnielRepository repoTurno)
        {
            this._repoPainel = repoPainel;
            this._repoLimAgend = repoLimAgend;
            this._repoDocProd = repoDocProd;
            this._repoConfig = repoConfig;
            this._repoArea = repoArea;
            this._repoAgendamento = repoAgendamento;
            this._repoNatureza = repoNatureza;
            this._repoConfiguracoesGerais = repoConfiguracoesGerais;
            this._repoHorario = repoHorario;
            this._repoTurno = repoTurno;
        }

        public IEnumerable<PainelPlanejamentoCapacidadeModel> GetPainelPlanejamentoCapacidade(PainelPlanejamentoCapacidadeRequest filtros)
        {
            var retorno = new List<PainelPlanejamentoCapacidadeModel>();

            string codEscritorio = filtros.GrupoArea;
            string codArea = filtros.Area;
            int idNatureza = Convert.ToInt32(filtros.Natureza);
            var idsPerfilAgendamento = ObterPerfisAgendNatureza(idNatureza, codEscritorio, codArea).Result;

            // Preenche inicialmente a lista com o grupo e a área filtrada
            var queryDadosArea = this._repoArea.GetAll();
            if (!string.IsNullOrEmpty(codEscritorio))
            {
                queryDadosArea = queryDadosArea.Where(p => p.Cod_Escritorio == codEscritorio);
            }
            if (!string.IsNullOrEmpty(codArea))
            {
                queryDadosArea = queryDadosArea.Where(p => p.Cod_Area == codArea);
            }

            var listaDadosArea = queryDadosArea.ToList();
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            var tempoMedioNatureza = this._repoPainel.GetTempoMedioNatureza(config, idNatureza);

            foreach (var dadosArea in listaDadosArea)
            {

                var servicosAgendar = this._repoPainel.GetServicosAgendar(config, idNatureza, dadosArea.Cod_Escritorio, dadosArea.Cod_Area);


                DateTime dataInicio = Convert.ToDateTime(filtros.DataInicio);
                DateTime dataFim = Convert.ToDateTime(filtros.DataFim);

                for (DateTime data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {


                    var servicosAgendados = this._repoPainel.GetQtdServicosAgendados(config, idNatureza, dadosArea.Cod_Escritorio, dadosArea.Cod_Area, data);
                    var servicosAtribuidosTecnico = this._repoPainel.GetQtdServicosAtribuidosTecnico(config, idNatureza, dadosArea.Cod_Escritorio, dadosArea.Cod_Area, data);

                    var dadosPainel = new PainelPlanejamentoCapacidadeModel
                    {
                        Data = data.ToString("dd/MM/yyyy"),
                        Grupo = dadosArea.Grupo_Area.Descricao,
                        Area = dadosArea.Descricao
                    };

                    IEnumerable<PainelPlanejamentoCapacidadeDadosEscalaEquipe> escalaTecnicosDia =
                        this._repoPainel
                            .GetEscalaTecnicos(
                                config,
                                data,
                                dadosArea.Cod_Escritorio,
                                dadosArea.Cod_Area,
                                idNatureza
                            );


                    escalaTecnicosDia = escalaTecnicosDia.Where(x => idsPerfilAgendamento.Contains(x.IdPerfilAgendamento)); //escala de tecnicos apenas com perfis de agendamento da natureza/grupo area/area
                    int qtdTecnicos = escalaTecnicosDia.Count();
                    int qtdTecnicosEscalados = escalaTecnicosDia.Where(e => e.Escalado).Count();
                    int qtdTecnicosMultiskill = escalaTecnicosDia.Where(e => e.Escalado && e.Multiskill).Count();
                    int qtdTecnicosExclusivos = escalaTecnicosDia.Where(e => e.Escalado && !e.Multiskill).Count();

                    dadosPainel.Tecnicos = Convert.ToString(qtdTecnicos);
                    dadosPainel.Escalados = Convert.ToString(qtdTecnicosEscalados);
                    dadosPainel.Multiskill = Convert.ToString(qtdTecnicosMultiskill);
                    dadosPainel.ExclusivosNatureza = Convert.ToString(qtdTecnicosExclusivos);
                    dadosPainel.TempoMedioServico = tempoMedioNatureza.ToString(@"hh\:mm");


                    TimeSpan tempoAgendar = TimeSpan.FromMilliseconds(servicosAgendar.Select(p => p.TotalMilliseconds).Sum());
                    TimeSpan tempoAgendado = TimeSpan.FromMilliseconds(servicosAgendados.Select(p => p.TotalMilliseconds).Sum());
                    TimeSpan tempoAtribuidoTecnico = TimeSpan.FromMilliseconds(servicosAtribuidosTecnico.Select(p => p.TotalMilliseconds).Sum());
                    TimeSpan tempoEsforcoNecessario = tempoAgendar.Add(tempoAgendado);

                    dadosPainel.HorasAguardandoAgendamento = string.Format("{0:00}:{1:00}", (int)tempoAgendar.TotalHours, tempoAgendar.Minutes);
                    dadosPainel.HorasAgendado = string.Format("{0:00}:{1:00}", (int)tempoAgendado.TotalHours, tempoAgendar.Minutes);
                    dadosPainel.HorasAtribuidosTecnico = string.Format("{0:00}:{1:00}", (int)tempoAtribuidoTecnico.TotalHours, tempoAtribuidoTecnico.Minutes);
                    dadosPainel.HorasEsforcoNecessario = string.Format("{0:00}:{1:00}", (int)tempoEsforcoNecessario.TotalHours, tempoEsforcoNecessario.Minutes);

                    dadosPainel.RestaAtribuirTecnico = tempoEsforcoNecessario.Subtract(tempoAtribuidoTecnico).ToString(@"hh\:mm");

                    double capacidade = escalaTecnicosDia.Where(e => e.Escalado).Sum(e => e.HorasTrabalhadas);
                    double capacidadeMultiskill = escalaTecnicosDia.Where(e => e.Escalado && e.Multiskill).Sum(e => e.HorasTrabalhadas);
                    double capacidadeExclusiva = escalaTecnicosDia.Where(e => e.Escalado && !e.Multiskill).Sum(e => e.HorasTrabalhadas);

                    dadosPainel.CapacidadePlena = Convert.ToString(capacidade);
                    dadosPainel.CapacidadeMultiskill = Convert.ToString(capacidadeMultiskill);
                    dadosPainel.CapacidadeExclusiva = Convert.ToString(capacidadeExclusiva);

                    dadosPainel.RealizavelCapacidadePlena = capacidade == 0 ? "-" : Convert.ToString(Math.Round((capacidade / tempoEsforcoNecessario.TotalHours) * 100, 1));


                    if (capacidade > 0)
                    {
                        if (Convert.ToDouble(dadosPainel.RealizavelCapacidadePlena) > 100)
                        {
                            dadosPainel.RealizavelCapacidadePlena = "100";
                        }

                        dadosPainel.RealizavelCapacidadePlena += "%";
                    }



                    LimiteAgendamentoAniel limiteAgendamentoData =
                        this._repoLimAgend
                            .GetPercNaturezaByAreaData(
                                dadosArea.Cod_Escritorio,
                                dadosArea.Cod_Area,
                                idNatureza,
                                data
                            );

                    dadosPainel.LimiteMultiskillPerc = Convert.ToString(limiteAgendamentoData?.Percentual);




                    if (limiteAgendamentoData != null)
                    {
                        dadosPainel.CapacidadeLimitadaMultiskill = Convert.ToString(capacidadeMultiskill * (Convert.ToDouble(limiteAgendamentoData.Percentual) / 100));
                    }
                    else
                    {
                        dadosPainel.CapacidadeLimitadaMultiskill = dadosPainel.CapacidadeMultiskill;
                    }

                    var capacidadeLimitadaMultiskill = Convert.ToDouble(dadosPainel.CapacidadeMultiskill);
                    dadosPainel.RealizavelCapacidadeLimitada = capacidadeLimitadaMultiskill == 0 ? "-" : Convert.ToString(Math.Round((capacidadeLimitadaMultiskill / tempoEsforcoNecessario.TotalHours) * 100, 1));


                    if (capacidadeLimitadaMultiskill > 0)
                    {
                        if (Convert.ToDouble(dadosPainel.RealizavelCapacidadeLimitada) > 100)
                        {
                            dadosPainel.RealizavelCapacidadeLimitada = "100";
                        }

                        dadosPainel.RealizavelCapacidadeLimitada += "%";
                    }

                    AgendamentoAnielBusiness agendamentoAnielBusiness = new AgendamentoAnielBusiness(_repoAgendamento,
                                                                                                      _repoLimAgend,
                                                                                                      _repoNatureza,
                                                                                                      _repoConfiguracoesGerais,
                                                                                                      _repoHorario,
                                                                                                      _repoTurno);

                    var resultadoAgendamentoExclusivo = agendamentoAnielBusiness.GetDatasValidasDetalhamento(idNatureza,
                                                                         dadosArea.Cod_Escritorio,
                                                                         dadosArea.Cod_Area,
                                                                         null,
                                                                         null,
                                                                         DateTime.Today.Add(tempoMedioNatureza),
                                                                         dataInicio,
                                                                         null,
                                                                         idsPerfilAgendamento,
                                                                         false,
                                                                         true,
                                                                         false).Result;


                    dadosPainel.CapacidadeExclusiva = Convert.ToString(GetCapacidadeUtilAgendamento(resultadoAgendamentoExclusivo.FirstOrDefault()).TotalHours);



                    var resultadoAgendamentoMultiskill = agendamentoAnielBusiness.GetDatasValidasDetalhamento(idNatureza,
                                                     dadosArea.Cod_Escritorio,
                                                     dadosArea.Cod_Area,
                                                     null,
                                                     null,
                                                     DateTime.Today.Add(tempoMedioNatureza),
                                                     dataInicio,
                                                     null,
                                                     idsPerfilAgendamento,
                                                     false,
                                                     false,
                                                     false).Result;

                    dadosPainel.CapacidadeMultiskill = Convert.ToString(GetCapacidadeUtilAgendamento(resultadoAgendamentoMultiskill.FirstOrDefault()).TotalHours);

                    var resultadoAgendamento = agendamentoAnielBusiness.GetDatasValidasDetalhamento(idNatureza,
                                                     dadosArea.Cod_Escritorio,
                                                     dadosArea.Cod_Area,
                                                     null,
                                                     null,
                                                     DateTime.Today.Add(tempoMedioNatureza),
                                                     dataInicio,
                                                     null,
                                                     idsPerfilAgendamento,
                                                     false,
                                                     null,
                                                     false).Result;


                    var agendamento = resultadoAgendamento.FirstOrDefault();


                    if (agendamento != null)
                    {
                        dadosPainel.CapacidadePlena = Convert.ToString(GetCapacidadeUtilAgendamento(agendamento).TotalHours);
                        dadosPainel.janelas = agendamento.HorasTurnoTotal.Select(p =>
                        {
                            var agendamentoCompartilhadoOutrasNaturezas = this.GetAgendamentoCompartilhadoNaturezaArea(idNatureza, dadosArea.Cod_Escritorio,
                                                     dadosArea.Cod_Area,
                                                     null,
                                                     null,
                                                     DateTime.Today.Add(tempoMedioNatureza),
                                                     dataInicio,
                                                     null,
                                                     null,
                                                     false,
                                                     null,
                                                     false
                                ).Result;
                            var horasTurnoTotalPeriodo = agendamento.HorasTurnoTotal.Find(y => y.Display == p.Display);
                            var perc = horasTurnoTotalPeriodo.ForcaTrabalhoMinutosPrevistos == 0 || p.JaAgendadosMinutosConsumidos == 0 ? 0 : 
                                       p.JaAgendadosMinutosConsumidos / horasTurnoTotalPeriodo.ForcaTrabalhoMinutosPrevistos;
                            return new PainelPlanejamentoCapacidadeAgendamento
                            {
                                Janela = p.Display,
                                AgendamentoRealizado = Convert.ToString(Math.Round(p.JaAgendadosMinutosConsumidos / 60, 1)),
                                AgendamentoMaximo = Convert.ToString(Math.Round(horasTurnoTotalPeriodo.ForcaTrabalhoMinutosPrevistos / 60, 1)),
                                PercOcupado = string.Format("{0:P}", perc),
                                AgendamentoRestante = p.ForcaTrabalhoMinutosPrevistos > 0 ? Convert.ToString(Math.Round((p.ForcaTrabalhoMinutosPrevistos - p.JaAgendadosMinutosConsumidos) / 60, 1)): Convert.ToString(p.ForcaTrabalhoMinutosPrevistos),
                                AgendamentoCompartilhadoNaturezaArea = Convert.ToString(Math.Round((agendamentoCompartilhadoOutrasNaturezas - p.JaAgendadosMinutosConsumidos) / 60, 1))
                            };

                        }).ToList();
                    }



                    dadosPainel.CodGrupoArea = dadosArea.Cod_Escritorio;
                    dadosPainel.CodArea = dadosArea.Cod_Area;




                    retorno.Add(dadosPainel);
                }
            }

            return retorno;
        }

        private async Task<List<long?>> ObterPerfisAgendNatureza(int idNatureza, string codEscritorio, string codArea)
        {
            List<long?> listaPerfis = new List<long?>();
            using (var session = HibernateUtil.GetSessionT5())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, string> parametros = new Dictionary<string, string>();

                sql.AppendLine("SELECT DISTINCT A.ID AS ID_PERFIL_AGD ");
                sql.AppendLine("FROM TB_PERFIL_AGENDAMENTO A ");
                sql.AppendLine("LEFT JOIN TB_PERFIL_AGENDAMENTO_NATUREZA B ON (B.ID_PERFIL_AGENDAMENTO = A.ID) ");
                sql.AppendLine("LEFT JOIN TB_PERFIL_AGENDAMENTO_AREA C ON (C.ID_PERFIL_AGENDAMENTO = A.ID) ");
                sql.AppendLine("WHERE A.STATUS = '1' ");
                sql.AppendLine("AND B.ID_NATUREZA = @IDNATUREZA ");
                sql.AppendLine("AND ((C.TODAS_AREAS = '1' AND C.TODAS_LOCALIDADES = '1') OR (C.COD_ESCRITORIO = @GRUPOAREA AND C.COD_AREA = @CODAREA)) ");

                using (var comm = session.Connection.CreateCommand())
                {
                    parametros.Add("@IDNATUREZA", idNatureza.ToString());
                    parametros.Add("@GRUPOAREA", codEscritorio);
                    parametros.Add("@CODAREA", codArea);

                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            listaPerfis.Add(Convert.ToInt64(reader["ID_PERFIL_AGD"]));
                        }
                    }
                }
            }
            return listaPerfis;
        }

        private TimeSpan GetCapacidadeUtilAgendamento(AgendamentoDiaModel agendamento)
        {
            TimeSpan CapacidadeTotal = agendamento != null ? TimeSpan.FromMinutes(agendamento.Horas.Sum(p => p.ForcaTrabalhoMinutosPrevistos)) : TimeSpan.Zero;
            return CapacidadeTotal;
        }

        public async Task<double> GetAgendamentoCompartilhadoNaturezaArea(int idNaturezaComparacao,
                                                                                 String Cod_Grupo_Area,
                                                                                 String Cod_Area,
                                                                                 Int32? Cod_Localidade,
                                                                                 String LocalidadeStr,
                                                                                 DateTime? TempoExecucaoServico,
                                                                                 DateTime dataInicial,
                                                                                 Int64? IdPeriodo,
                                                                                 Int64? idPerfil,
                                                                                 bool limiteAgendamento,
                                                                                 bool? Exclusivo = null,
                                                                                 bool apenasAptos = true)

        {
            double minutosAcumulados = 0;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            AgendamentoAnielBusiness agendamentoAnielBusiness = new AgendamentoAnielBusiness(_repoAgendamento,
                                                                                                      _repoLimAgend,
                                                                                                      _repoNatureza,
                                                                                                      _repoConfiguracoesGerais,
                                                                                                      _repoHorario,
                                                                                                      _repoTurno);
            foreach (int idNatureza in this._repoNatureza.GetAll().Where(p => p.Id != idNaturezaComparacao).Select(p => p.Id).ToList()) 
            {
                var tempoMedioNatureza = this._repoPainel.GetTempoMedioNatureza(config, idNatureza);
                var datasValidas = await agendamentoAnielBusiness.GetDatasValidasDetalhamento(idNatureza,
                                                         Cod_Grupo_Area,
                                                         Cod_Area,
                                                         null,
                                                         null,
                                                         DateTime.Today.Add(tempoMedioNatureza),
                                                         dataInicial,
                                                         null,
                                                         null,
                                                         false,
                                                         null,
                                                         false);
                if (datasValidas.Count > 0) 
                {
                    datasValidas[0].HorasTurnoTotal.ForEach(p => { minutosAcumulados += p.JaAgendadosMinutosConsumidos; });
                }
            }
            return minutosAcumulados;
        }
    }
}
