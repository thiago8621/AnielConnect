using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Hql.Ast;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.T5.PainelPlanejamentoCapacidade
{
    public class PainelPlanejamentoCapacidadeAnielRepository : IPainelPlanejamentoCapacidadeAnielRepository
    {
        private readonly ISession _sessionT5;
        private readonly ISession _sessionT5Nh;

        private IDictionary<long, Horario_Connect> _cacheHorarioConnect;

        public PainelPlanejamentoCapacidadeAnielRepository(ISession sessionT5, ISession sessionT5Nh)
        {
            this._sessionT5 = sessionT5;
            this._sessionT5Nh = sessionT5Nh;

            this._cacheHorarioConnect = new Dictionary<long, Horario_Connect>();
        }

        public IEnumerable<PainelPlanejamentoCapacidadeDadosEscalaEquipe> GetEscalaTecnicos(
            Configuracoes_Gerais config,
            DateTime data,
            string codEscritorio,
            string codArea,
            int idNatureza
        )
        {
            var dadosEscalaEquipe = new List<PainelPlanejamentoCapacidadeDadosEscalaEquipe>();

            using (var com = this._sessionT5.Connection.CreateCommand())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(this._sessionT5Nh))
            {
                var param = new Dictionary<string, object>();
                var sql = new StringBuilder();

                sql.AppendLine("SELECT E.NUM_FRE AS MATRICULA_EQUIPE, F.NOME AS NOME_EQUIPE,");
                sql.AppendLine("       F.HORARIO_ATRIBUIDO AS TIPO_HORARIO,");

                sql.AppendLine("       (SELECT FIRST 1 H.ID_HORARIO");
                sql.AppendLine("        FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("        WHERE H.EQUIPE = E.NUM_FRE");
                sql.AppendLine("          AND H.DATA = @DATA) AS ID_HORARIO_CONNECT,");

                sql.AppendLine("       (SELECT FIRST 1 J.ID_HORARIO_CONNECT");
                sql.AppendLine("        FROM TB_PT_DIA_FUNCIONARIO DF");
                sql.AppendLine("        INNER JOIN TB_PT_JORNADA_FUNCIONARIO J");
                sql.AppendLine("        ON J.ID = DF.ID_JORNADA");
                sql.AppendLine("        WHERE DF.NUM_FRE = F.NUM_FRE");
                sql.AppendLine("          AND DF.DATA = @DATA) AS ID_HORARIO_POINT,");

                sql.AppendLine("       COALESCE(");
                sql.AppendLine("           (SELECT FIRST 1 CASE");
                sql.AppendLine("                               WHEN H.INDISPONIVEL = 1 THEN 0");
                sql.AppendLine("                               ELSE 1");
                sql.AppendLine("                           END");
                sql.AppendLine("            FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("            WHERE H.EQUIPE = E.EQUIPE");
                sql.AppendLine("              AND H.DATA = @DATA), 0) AS DISPONIVEL_CONNECT,");

                sql.AppendLine("       COALESCE(");
                sql.AppendLine("           (SELECT FIRST 1 C.PERMTITE_ROTEIRIZAR");
                sql.AppendLine("            FROM TB_PT_DIA_FUNCIONARIO DF");
                sql.AppendLine("            INNER JOIN TB_PT_CLASSIFICACAO_ESCALA C");
                sql.AppendLine("            ON C.ID = DF.ID_EVENTO");
                sql.AppendLine("            WHERE DF.NUM_FRE = F.NUM_FRE");
                sql.AppendLine("              AND DF.DATA = @DATA), 0) AS DISPONIVEL_POINT,");

                sql.AppendLine("       IIF(");
                sql.AppendLine("           (SELECT COUNT(P_NAT.ID)");
                sql.AppendLine("            FROM TB_PERFIL_AGENDAMENTO_NATUREZA P_NAT ");
                sql.AppendLine("            LEFT JOIN TB_PERFIL_AGENDAMENTO PERFIL_AGND ON (PERFIL_AGND.ID = P_NAT.ID_PERFIL_AGENDAMENTO) ");
                sql.AppendLine("            WHERE PERFIL_AGND.STATUS <> 0 AND P_NAT.ID_PERFIL_AGENDAMENTO = COALESCE((SELECT FIRST 1 H.ID_PERFIL_AGENDAMENTO");
                sql.AppendLine("                                                          FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("                                                          WHERE H.EQUIPE = E.EQUIPE");
                sql.AppendLine("                                                            AND(");
                sql.AppendLine("                                                                (H.APENAS_NO_DIA = 1 AND H.DATA = @DATA) OR");
                sql.AppendLine("                                                                (H.APENAS_NO_DIA = 0 AND H.DATA <= @DATA)");
                sql.AppendLine("                                                            )");
                sql.AppendLine("                                                          ), E.ID_PERFIL_AGENDAMENTO)) > 1, 1, 0) AS MULTISKILL");
                sql.AppendLine("  , E.ID_PERFIL_AGENDAMENTO AS ID_PERFIL_EQ ");

                sql.AppendLine("FROM TB_EQUIPE E");

                sql.AppendLine("INNER JOIN TB_FUNCIONARIO F");
                sql.AppendLine("ON(F.NUM_FRE = E.NUM_FRE)");

                sql.AppendLine("WHERE F.SITUACAO = 0");

                // Filtro para verificar se a equipe atende a área

                sql.AppendLine("  AND (SELECT FIRST 1 P_AREA.ID");
                sql.AppendLine("       FROM TB_PERFIL_AGENDAMENTO_AREA P_AREA");
                sql.AppendLine("       LEFT JOIN TB_PERFIL_AGENDAMENTO PERFIL_AGND ON (PERFIL_AGND.ID = P_AREA.ID_PERFIL_AGENDAMENTO) ");
                sql.AppendLine("       WHERE PERFIL_AGND.STATUS <> 0 AND P_AREA.COD_ESCRITORIO = @COD_ESCRITORIO");
                sql.AppendLine("         AND (P_AREA.COD_AREA = @COD_AREA OR P_AREA.TODAS_AREAS = 1)");
                sql.AppendLine("         AND P_AREA.ID_PERFIL_AGENDAMENTO = COALESCE((SELECT FIRST 1 H.ID_PERFIL_AGENDAMENTO");
                sql.AppendLine("                                                      FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("                                                      WHERE H.EQUIPE = E.EQUIPE");
                sql.AppendLine("                                                        AND(");
                sql.AppendLine("                                                            (H.APENAS_NO_DIA = 1 AND H.DATA = @DATA) OR");
                sql.AppendLine("                                                            (H.APENAS_NO_DIA = 0 AND H.DATA <= @DATA)");
                sql.AppendLine("                                                        )");
                sql.AppendLine("                                                      ), E.ID_PERFIL_AGENDAMENTO)) IS NOT NULL");

                // Filtro para verificar se a equipe atende a natureza

                sql.AppendLine("  AND (SELECT FIRST 1 P_NAT.ID");
                sql.AppendLine("       FROM TB_PERFIL_AGENDAMENTO_NATUREZA P_NAT");
                sql.AppendLine("       LEFT JOIN TB_PERFIL_AGENDAMENTO PERFIL_AGND ON (PERFIL_AGND.ID = P_NAT.ID_PERFIL_AGENDAMENTO) ");
                sql.AppendLine("       WHERE PERFIL_AGND.STATUS <> 0 AND P_NAT.ID_NATUREZA = @ID_NATUREZA");
                sql.AppendLine("         AND P_NAT.ID_PERFIL_AGENDAMENTO = COALESCE((SELECT FIRST 1 H.ID_PERFIL_AGENDAMENTO");
                sql.AppendLine("                                                     FROM TB_HORARIO_EQUIPE_CONNECT H");
                sql.AppendLine("                                                     WHERE H.EQUIPE = E.EQUIPE");
                sql.AppendLine("                                                       AND(");
                sql.AppendLine("                                                           (H.APENAS_NO_DIA = 1 AND H.DATA = @DATA) OR");
                sql.AppendLine("                                                           (H.APENAS_NO_DIA = 0 AND H.DATA <= @DATA)");
                sql.AppendLine("                                                       )");
                sql.AppendLine("                                                     ), E.ID_PERFIL_AGENDAMENTO)) IS NOT NULL");

                param.Add("@DATA", data);

                param.Add("@COD_ESCRITORIO", codEscritorio);
                param.Add("@COD_AREA", codArea);
                param.Add("@ID_NATUREZA", idNatureza);

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var escalaEquipe = new PainelPlanejamentoCapacidadeDadosEscalaEquipe();

                        escalaEquipe.NumFre = Convert.ToString(reader["MATRICULA_EQUIPE"]);
                        escalaEquipe.Nome = Convert.ToString(reader["NOME_EQUIPE"]);
                        escalaEquipe.Multiskill = Convert.ToBoolean(reader["MULTISKILL"]);
                        escalaEquipe.IdPerfilAgendamento = Convert.ToInt64(reader["ID_PERFIL_EQ"]);

                        #region Escala e Horário de Trabalho

                        HorarioAtribuido tipoHorario;

                        if (reader["TIPO_HORARIO"] != DBNull.Value)
                        {
                            tipoHorario = (HorarioAtribuido)Convert.ToInt32(reader["TIPO_HORARIO"]);
                        }
                        else
                        {
                            tipoHorario =
                                config.UtilizaPoint_RoteirizacaoAgendamento ?
                                    HorarioAtribuido.AnielPoint :
                                    HorarioAtribuido.AnielConnect;
                        }

                        if (tipoHorario == HorarioAtribuido.AnielPoint)
                        {
                            long? idHorarioEquipePoint =
                                reader["ID_HORARIO_POINT"] != DBNull.Value ?
                                    Convert.ToInt64(reader["ID_HORARIO_POINT"]) :
                                    (long?)null;

                            bool disponivelPoint = Convert.ToBoolean(reader["DISPONIVEL_POINT"]);

                            if (!disponivelPoint || !idHorarioEquipePoint.HasValue)
                            {
                                escalaEquipe.Escalado = false;
                            }
                            else
                            {
                                Horario_Connect horarioConnect = null;

                                if (!this._cacheHorarioConnect.ContainsKey(idHorarioEquipePoint.Value))
                                {
                                    this._cacheHorarioConnect.Add(
                                        idHorarioEquipePoint.Value,
                                        repoHorarioConnect.selectFirst(p => p.Id == idHorarioEquipePoint.Value).Result
                                    );
                                }

                                horarioConnect = this._cacheHorarioConnect[idHorarioEquipePoint.Value];

                                HorariosTrabalho horariosDia = this.GetHorariosTrabalhoDia(horarioConnect, data);

                                escalaEquipe.Escalado = horariosDia.Inicio.HasValue && horariosDia.Termino.HasValue;

                                if (escalaEquipe.Escalado)
                                {
                                    escalaEquipe.HorasTrabalhadas =
                                        this.HorasTrabalhadas(
                                            horariosDia.Inicio.Value,
                                            horariosDia.InicioIntervalo,
                                            horariosDia.TerminoIntervalo,
                                            horariosDia.Termino.Value
                                        );
                                }
                            }
                        }
                        else
                        {
                            long? idHorarioEquipeConnect =
                                reader["ID_HORARIO_CONNECT"] != DBNull.Value ?
                                    Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) :
                                    (long?)null;

                            bool disponivelConnect = Convert.ToBoolean(reader["DISPONIVEL_CONNECT"]);

                            if (idHorarioEquipeConnect.HasValue && !disponivelConnect)
                            {
                                escalaEquipe.Escalado = false;
                            }
                            else
                            {
                                Horario_Connect horarioConnect = null;

                                if (idHorarioEquipeConnect.HasValue)
                                {
                                    if (!this._cacheHorarioConnect.ContainsKey(idHorarioEquipeConnect.Value))
                                    {
                                        this._cacheHorarioConnect.Add(
                                            idHorarioEquipeConnect.Value,
                                            repoHorarioConnect.selectFirst(p => p.Id == idHorarioEquipeConnect).Result
                                        );
                                    }
                                }
                                else
                                {
                                    if (!this._cacheHorarioConnect.ContainsKey(-1))
                                    {
                                        this._cacheHorarioConnect.Add(
                                            -1,
                                            repoHorarioConnect.selectFirst(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico &&
                                                                                p.Status == Status_Default.Ativo &&
                                                                                p.Padrao).Result
                                        );
                                    }
                                }

                                horarioConnect = this._cacheHorarioConnect[idHorarioEquipeConnect ?? -1];

                                HorariosTrabalho horariosDia = this.GetHorariosTrabalhoDia(horarioConnect, data);

                                escalaEquipe.Escalado = horariosDia.Inicio.HasValue && horariosDia.Termino.HasValue;

                                if (escalaEquipe.Escalado)
                                {
                                    escalaEquipe.HorasTrabalhadas =
                                        this.HorasTrabalhadas(
                                            horariosDia.Inicio.Value,
                                            horariosDia.InicioIntervalo,
                                            horariosDia.TerminoIntervalo,
                                            horariosDia.Termino.Value
                                        );
                                }
                            }
                        }

                        #endregion
                        if (escalaEquipe.IdPerfilAgendamento != null)
                            dadosEscalaEquipe.Add(escalaEquipe);
                    }
                }
            }

            return dadosEscalaEquipe;
        }

        /// <summary>
        ///     Retorna os horários de uma escala para determinado dia
        /// </summary>
        /// <param name="horario">Objeto com os horários da escala</param>
        /// <param name="dia">Data para consultar a escala</param>
        private HorariosTrabalho GetHorariosTrabalhoDia(Horario_Connect horario, DateTime dia)
        {
            switch (dia.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Domingo,
                        InicioIntervalo = horario.Inicio_Intervalo_Domingo,
                        TerminoIntervalo = horario.Termino_Intervalo_Domingo,
                        Termino = horario.Termino_Domingo
                    };
                case DayOfWeek.Monday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Segunda,
                        InicioIntervalo = horario.Inicio_Intervalo_Segunda,
                        TerminoIntervalo = horario.Termino_Intervalo_Segunda,
                        Termino = horario.Termino_Segunda
                    };
                case DayOfWeek.Tuesday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Terca,
                        InicioIntervalo = horario.Inicio_Intervalo_Terca,
                        TerminoIntervalo = horario.Termino_Intervalo_Terca,
                        Termino = horario.Termino_Terca
                    };
                case DayOfWeek.Wednesday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Quarta,
                        InicioIntervalo = horario.Inicio_Intervalo_Quarta,
                        TerminoIntervalo = horario.Termino_Intervalo_Quarta,
                        Termino = horario.Termino_Quarta
                    };
                case DayOfWeek.Thursday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Quinta,
                        InicioIntervalo = horario.Inicio_Intervalo_Quinta,
                        TerminoIntervalo = horario.Termino_Intervalo_Quinta,
                        Termino = horario.Termino_Quinta
                    };
                case DayOfWeek.Friday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Sexta,
                        InicioIntervalo = horario.Inicio_Intervalo_Sexta,
                        TerminoIntervalo = horario.Termino_Intervalo_Sexta,
                        Termino = horario.Termino_Sexta
                    };
                case DayOfWeek.Saturday:
                    return new HorariosTrabalho
                    {
                        Inicio = horario.Inicio_Sabado,
                        InicioIntervalo = horario.Inicio_Intervalo_Sabado,
                        TerminoIntervalo = horario.Termino_Intervalo_Sabado,
                        Termino = horario.Termino_Sabado
                    };
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Retorna as horas trabalhados em um determinado horário
        /// </summary>
        /// <param name="inicio">Horário de início</param>
        /// <param name="inicioIntervalo">Horário de início do intervalo, pode ser nulo caso não haja intervalo no horário de trabalho</param>
        /// <param name="terminoIntervalo">Horário de término do intervalo, pode ser nulo caso não haja intervalo no horário de trabalho</param>
        /// <param name="termino">Horário de término</param>
        private double HorasTrabalhadas(DateTime inicio, DateTime? inicioIntervalo, DateTime? terminoIntervalo, DateTime termino)
        {
            double totalhoras = (termino - inicio).TotalHours;

            if (inicioIntervalo.HasValue && terminoIntervalo.HasValue)
            {
                double horasIntervalo = (terminoIntervalo.Value - inicioIntervalo.Value).TotalHours;
                totalhoras -= horasIntervalo;
            }

            return totalhoras;
        }

        public List<TimeSpan> GetServicosAgendar(
            Configuracoes_Gerais config,
            int? natureza,
            string codEscritorio,
            string codArea
        )
        {
            var dadosServicosAgendar = new List<TimeSpan>();
            TimeSpan Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay : TimeSpan.Zero;
            using (var com = this._sessionT5.Connection.CreateCommand())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(this._sessionT5Nh))
            {
                var param = new Dictionary<string, object>();
                var sql = new StringBuilder();

                sql.AppendLine("SELECT A.NUM_OBRA,");
                sql.AppendLine("(SELECT FIRST 1 M.HORAS ");
                sql.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ");
                sql.AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine("LEFT JOIN  TB_TIPO_SERVICO_EQUIPE B ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine("WHERE A.STATUS_EXECUCAO = 4");

                if (natureza.HasValue)
                {
                    sql.AppendLine("AND B.COD_NATUREZA = @NATUREZA");
                    param.Add("@NATUREZA", natureza);
                }

                if (string.IsNullOrEmpty(codEscritorio) == false)
                {
                    sql.AppendLine("AND A.COD_ESCRITORIO = @ESCRITORIO");
                    param.Add("@ESCRITORIO", codEscritorio);

                    if (string.IsNullOrEmpty(codArea) == false)
                    {
                        sql.AppendLine("AND A.COD_AREA = @AREA");
                        param.Add("@AREA", codArea);
                    }
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var tempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan)(reader["TEMPO_EXECUCAO"]) : Tempo_Execuxao_Padrao;
                        dadosServicosAgendar.Add(tempoExecucao);
                    }
                }
            }

            return dadosServicosAgendar;
        }

        public List<TimeSpan> GetQtdServicosAgendados(
            Configuracoes_Gerais config,
            int? natureza,
            string codEscritorio,
            string codArea,
            DateTime data
        )
        {
            var dadosServicosAgendados = new List<TimeSpan>();
            TimeSpan Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay : TimeSpan.Zero;
            using (var com = this._sessionT5.Connection.CreateCommand())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(this._sessionT5Nh))
            {
                var param = new Dictionary<string, object>();
                var sql = new StringBuilder();
                sql.AppendLine("SELECT A.NUM_OBRA,C.DATAHORA_INICIO,C.DATAHORA_FIM,");
                sql.AppendLine("(SELECT FIRST 1 M.HORAS ");
                sql.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ");
                sql.AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine("LEFT JOIN  TB_TIPO_SERVICO_EQUIPE B ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine("LEFT JOIN  TB_AGENDAMENTO_EQUIPEANIEL C ON (A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.NUM_DOC = C.NUM_DOC AND A.NUM_OBRA = C.NUM_OBRA)");

                sql.AppendLine("WHERE A.STATUS_EXECUCAO = 13 ");
                sql.AppendLine("AND A.DATA_MAXIMA = @DATA ");
                param.Add("@DATA", data);

                if (natureza.HasValue)
                {
                    sql.AppendLine("AND B.COD_NATUREZA = @NATUREZA");
                    param.Add("@NATUREZA", natureza);
                }

                if (string.IsNullOrEmpty(codEscritorio) == false)
                {
                    sql.AppendLine("AND A.COD_ESCRITORIO = @ESCRITORIO");
                    param.Add("@ESCRITORIO", codEscritorio);

                    if (string.IsNullOrEmpty(codArea) == false)
                    {
                        sql.AppendLine("AND A.COD_AREA = @AREA");
                        param.Add("@AREA", codArea);
                    }
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        TimeSpan tempoAgendamento = TimeSpan.Zero;
                        if (reader["DATAHORA_INICIO"] != DBNull.Value && reader["DATAHORA_FIM"] != DBNull.Value)
                        {
                            tempoAgendamento = Convert.ToDateTime(reader["DATAHORA_FIM"]) - Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                        }

                        var tempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan)(reader["TEMPO_EXECUCAO"]) : Tempo_Execuxao_Padrao;
                        dadosServicosAgendados.Add(tempoAgendamento.TotalMilliseconds > 0 ? tempoAgendamento : tempoExecucao);
                    }
                }
            }

            return dadosServicosAgendados;
        }

        public List<TimeSpan> GetQtdServicosAtribuidosTecnico(
            Configuracoes_Gerais config,
            int? natureza,
            string codEscritorio,
            string codArea,
            DateTime data
        )
        {
            var dadosServicosAtribuidosTecnico = new List<TimeSpan>();
            TimeSpan Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay : TimeSpan.Zero;
            using (var com = this._sessionT5.Connection.CreateCommand())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(this._sessionT5Nh))
            {
                var param = new Dictionary<string, object>();
                var sql = new StringBuilder();
                sql.AppendLine("SELECT A.NUM_OBRA,C.DATAHORA_INICIO,C.DATAHORA_FIM,");
                sql.AppendLine("(SELECT FIRST 1 M.HORAS ");
                sql.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ");
                sql.AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ");
                sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine("LEFT JOIN  TB_TIPO_SERVICO_EQUIPE B ON (A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine("LEFT JOIN  TB_AGENDAMENTO_EQUIPEANIEL C ON (A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.NUM_DOC = C.NUM_DOC AND A.NUM_OBRA = C.NUM_OBRA)");

                sql.AppendLine("WHERE A.STATUS_EXECUCAO = 13 ");
                sql.AppendLine("AND A.DATA_MAXIMA = @DATA ");
                param.Add("@DATA", data);

                if (natureza.HasValue)
                {
                    sql.AppendLine("AND B.COD_NATUREZA = @NATUREZA");
                    param.Add("@NATUREZA", natureza);
                }

                if (string.IsNullOrEmpty(codEscritorio) == false)
                {
                    sql.AppendLine("AND A.COD_ESCRITORIO = @ESCRITORIO");
                    param.Add("@ESCRITORIO", codEscritorio);

                    if (string.IsNullOrEmpty(codArea) == false)
                    {
                        sql.AppendLine("AND A.COD_AREA = @AREA");
                        param.Add("@AREA", codArea);
                    }
                }

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        TimeSpan tempoAgendamento = TimeSpan.Zero;
                        if (reader["DATAHORA_INICIO"] != DBNull.Value && reader["DATAHORA_FIM"] != DBNull.Value)
                        {
                            tempoAgendamento = Convert.ToDateTime(reader["DATAHORA_FIM"]) - Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                        }

                        var tempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan)(reader["TEMPO_EXECUCAO"]) : Tempo_Execuxao_Padrao;
                        dadosServicosAtribuidosTecnico.Add(tempoAgendamento.TotalMilliseconds > 0 ? tempoAgendamento : tempoExecucao);
                    }
                }
            }

            return dadosServicosAtribuidosTecnico;
        }

        public TimeSpan GetTempoMedioNatureza(
            Configuracoes_Gerais config,
            int? natureza
        )
        {
            var tempos = new List<TimeSpan>();
            TimeSpan Tempo_Execuxao_Padrao = TimeSpan.Zero;

            using (var com = this._sessionT5.Connection.CreateCommand())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(this._sessionT5Nh))
            using (var repoSlaServico = new Conexao<SLA_Servico>(this._sessionT5Nh))
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(this._sessionT5Nh))
            {
                Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay : TimeSpan.Zero;
                var param = new Dictionary<string, object>();
                var sql = new StringBuilder();

                sql.AppendLine("SELECT A.HORAS ");
                sql.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL A ");
                sql.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE B ");
                sql.AppendLine("ON(A.ID_TIPO_SERVICO = B.COD_TIPO_SERV) ");
                sql.AppendLine("WHERE B.COD_NATUREZA = @NATUREZA ");
                param.Add("@NATUREZA", natureza);

                using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var tempoExecucao = reader["HORAS"] != DBNull.Value ? (TimeSpan)(reader["HORAS"]) : Tempo_Execuxao_Padrao;
                        tempos.Add(tempoExecucao);
                    }
                }
            }

            return tempos.Count > 0 ? TimeSpan.FromMilliseconds((tempos.Select(p => p.TotalMilliseconds).Sum() / tempos.Count)) : Tempo_Execuxao_Padrao;
        }


    }
}
