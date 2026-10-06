using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Connect.Repository.T5.ProdutividadeTecnico
{
    public class ProdutividadeTecnicoRepository : IProdutividadeTecnicoRepository
    {
        private ISession _sessionT5Sql;

        public ProdutividadeTecnicoRepository(ISession sessionT5Sql)
        {
            this._sessionT5Sql = sessionT5Sql;
        }

        public RelatorioProdutividadeResponse GetRelatorioProdutividadeTecnico(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc)
        {
            int total = 0;
            var data = new List<RelatorioProdutividadeTecnicoListModel>();

            #region SQL From

            var sqlFrom = new StringBuilder();
            var param = new Dictionary<string, object>();

            sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");

            sqlFrom.AppendLine("INNER JOIN TB_EQUIPE E");
            sqlFrom.AppendLine("ON(E.EQUIPE = D.EQUIPE)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO S");
            sqlFrom.AppendLine("ON(S.NUM_FRE = E.COD_SUPERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_SENHA U_MON");
            sqlFrom.AppendLine("ON(U_MON.USUARIO = D.USUARIO_MONITOR)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO M");
            sqlFrom.AppendLine("ON(M.NUM_FRE = U_MON.NUM_FRE)");

            sqlFrom.AppendLine("INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV");
            sqlFrom.AppendLine("ON(T_SERV.COD_TIPO_SERV = D.COD_TIPO_SERV)");

            sqlFrom.AppendLine("INNER JOIN TB_NATUREZA_PRODUCAO N");
            sqlFrom.AppendLine("ON(N.ID = T_SERV.COD_NATUREZA)");

            sqlFrom.AppendLine("LEFT JOIN TB_TURNO_SERVICO T");
            sqlFrom.AppendLine("ON(T.ID = D.COD_TURNO_SERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL A");
            sqlFrom.AppendLine("ON(A.ID = D.ID_AGENDAMENTO)");

            sqlFrom.AppendLine("WHERE D.STATUS_EXECUCAO IN(1, 2)");

            #region Filtros

            DateTime dataInicio = DateTime.Parse(request.DataInicio);
            DateTime dataFim = DateTime.Parse(request.DataFim);

            sqlFrom.AppendLine("  AND D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

            param.Add("@DATA_INI", dataInicio);
            param.Add("@DATA_FIM", dataFim);

            if (request.search != null && !String.IsNullOrEmpty(request.search.value))
            {
                string search = request.search.value.ToUpperInvariant();
                sqlFrom.AppendLine($@"  AND UPPER(D.EQUIPE) LIKE '%{search}%' OR UPPER(E.NOME) LIKE '%{search}%'");
            }

            if (!String.IsNullOrEmpty(request.GrupoArea))
            {
                sqlFrom.AppendLine($@"  AND D.COD_ESCRITORIO = @GRUPO_AREA");
                param.Add("@GRUPO_AREA", request.GrupoArea);
            }

            if (!String.IsNullOrEmpty(request.Area))
            {
                sqlFrom.AppendLine($@"  AND D.COD_AREA = @AREA");
                param.Add("@AREA", request.Area);
            }

            if (!String.IsNullOrEmpty(request.Monitor))
            {
                sqlFrom.AppendLine($@"  AND D.USUARIO_MONITOR = @MONITOR");
                param.Add("@MONITOR", request.Monitor);
            }

            if (!String.IsNullOrEmpty(request.Supervisor))
            {
                sqlFrom.AppendLine($@"  AND E.COD_SUPERV = @SUPERVISOR");
                param.Add("@SUPERVISOR", request.Supervisor);
            }

            if (!String.IsNullOrEmpty(request.Equipe))
            {
                sqlFrom.AppendLine($@"  AND D.EQUIPE = @EQUIPE");
                param.Add("@EQUIPE", request.Equipe);
            }

            if (request.Natureza.HasValue)
            {
                sqlFrom.AppendLine($@"  AND N.ID = @NATUREZA");
                param.Add("@NATUREZA", request.Natureza.Value);
            }

            #endregion

            sqlFrom.AppendLine("GROUP BY D.EQUIPE, E.NOME, N.ID, N.DESCRICAO, N.BASE_PRAZO, S.NOME, M.NOME,");
            sqlFrom.AppendLine("         N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO");

            sqlFrom.AppendLine("ORDER BY E.NOME, N.DESCRICAO");

            #endregion

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sqlCount = new StringBuilder();

                sqlCount.AppendLine("SELECT COUNT(*) AS QTD_REGISTROS");
                sqlCount.AppendLine("FROM(");
                sqlCount.AppendLine("SELECT D.EQUIPE AS MATRICULA, E.NOME AS TECNICO, N.DESCRICAO AS NATUREZA, ");
                sqlCount.AppendLine("       N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO");
                sqlCount.AppendLine(sqlFrom.ToString());
                sqlCount.AppendLine(")");

                using (var reader = CommandHelper.ExecuteReader(com, sqlCount, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_REGISTROS"]);
                }
            }

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT D.EQUIPE AS MATRICULA, E.NOME AS TECNICO, N.DESCRICAO AS NATUREZA, S.NOME AS SUPERVISOR,");
                sql.AppendLine("       M.NOME AS MONITOR, N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO,");
                sql.AppendLine("       SUM(IIF(D.STATUS_EXECUCAO = 2, 1, 0)) AS QTD_SERVICOS_PRODUTIVOS,");
                sql.AppendLine("       SUM(IIF(D.STATUS_EXECUCAO = 1, 1, 0)) AS QTD_SERVICOS_IMPRODUTIVOS,");

                sql.AppendLine("       (SELECT COUNT(*)");
                sql.AppendLine("        FROM TB_REINCIDENCIA_DOCPROD R");

                sql.AppendLine("        INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_ANT");
                sql.AppendLine("        ON(DOC_ANT.CODCT = R.CODCT_ANTERIOR AND");
                sql.AppendLine("           DOC_ANT.PROJETO = R.PROJETO_ANTERIOR AND");
                sql.AppendLine("           DOC_ANT.NUM_OBRA = R.NUM_OBRA_ANTERIOR AND");
                sql.AppendLine("           DOC_ANT.NUM_DOC = R.NUM_DOC_ANTERIOR)");

                sql.AppendLine("        INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_ANT");
                sql.AppendLine("        ON(T_SERV_ANT.COD_TIPO_SERV = DOC_ANT.COD_TIPO_SERV)");

                sql.AppendLine("        INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_R");
                sql.AppendLine("        ON(DOC_R.CODCT = R.CODCT AND");
                sql.AppendLine("            DOC_R.PROJETO = R.PROJETO AND");
                sql.AppendLine("            DOC_R.NUM_OBRA = R.NUM_OBRA AND");
                sql.AppendLine("            DOC_R.NUM_DOC = R.NUM_DOC)");

                sql.AppendLine("        WHERE DOC_ANT.EQUIPE = D.EQUIPE");
                sql.AppendLine("          AND T_SERV_ANT.COD_NATUREZA = N.ID");

                if (apenasServicosProdutivosReinc)
                {
                    sql.AppendLine("          AND DOC_R.STATUS_EXECUCAO = 2");
                }
                else
                {
                    sql.AppendLine("          AND DOC_R.STATUS_EXECUCAO <> 9");
                }

                sql.AppendLine("          AND DOC_R.DIA BETWEEN @DATA_INI AND @DATA_FIM) AS QTD_SERVICOS_REINC,");

                sql.AppendLine("      (CASE");
                sql.AppendLine("           WHEN N.BASE_PRAZO = 1 THEN"); /* Usa SLA como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD < D.DATA_INDICADOR OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = D.DATA_INDICADOR AND");
                sql.AppendLine("                           D.HORA_PROD <= D.HORA_INDICADOR");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("           ELSE"); /* Usa agendamento como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD < CAST(A.DATAHORA_FIM AS DATE) OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sql.AppendLine("                               AS TIME) <=");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sql.AppendLine("                               AS TIME)");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("       END) AS BX_DENTRO_PRAZO,");

                sql.AppendLine("      (CASE");
                sql.AppendLine("           WHEN N.BASE_PRAZO = 1 THEN"); /* Usa SLA como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD > D.DATA_INDICADOR OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = D.DATA_INDICADOR AND");
                sql.AppendLine("                           D.HORA_PROD > D.HORA_INDICADOR");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("           ELSE"); /* Usa agendamento como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD > CAST(A.DATAHORA_FIM AS DATE) OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sql.AppendLine("                               AS TIME) >");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sql.AppendLine("                               AS TIME)");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("       END) AS BX_FORA_PRAZO,");

                sql.AppendLine("      (CASE");
                sql.AppendLine("           WHEN N.BASE_PRAZO = 1 THEN"); /* Usa SLA como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD < D.DATA_INDICADOR OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = D.DATA_INDICADOR AND");
                sql.AppendLine("                           D.HORA_PROD <= D.HORA_INDICADOR");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("           ELSE"); /* Usa agendamento como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       T.ID IS NOT NULL AND");

                sql.AppendLine("                       D.DATA_PROD IS NOT NULL AND");
                sql.AppendLine("                       D.HORA_PROD IS NOT NULL AND");

                sql.AppendLine("                       D.DATA_PROD < D.DATA_MAXIMA OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = D.DATA_MAXIMA AND");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sql.AppendLine("                               AS TIME) <=");
                sql.AppendLine("                           CAST(");
                sql.AppendLine("                               (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sql.AppendLine("                                AS TIME)");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("       END) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");

                sql.AppendLine("      (CASE");
                sql.AppendLine("           WHEN N.BASE_PRAZO = 1 THEN"); /* Usa SLA como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       D.DATA_PROD > D.DATA_INDICADOR OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD = D.DATA_INDICADOR AND");
                sql.AppendLine("                           D.HORA_PROD > D.HORA_INDICADOR");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("           ELSE"); /* Usa agendamento como base do prazo */
                sql.AppendLine("               SUM(");
                sql.AppendLine("                   IIF(");
                sql.AppendLine("                       T.ID IS NULL OR");
                sql.AppendLine("                       (");
                sql.AppendLine("                           D.DATA_PROD IS NOT NULL AND");
                sql.AppendLine("                           D.HORA_PROD IS NOT NULL AND");
                sql.AppendLine("                           (");
                sql.AppendLine("                               D.DATA_PROD > D.DATA_MAXIMA OR");
                sql.AppendLine("                               (");
                sql.AppendLine("                                   D.DATA_PROD = D.DATA_MAXIMA AND");
                sql.AppendLine("                                   CAST(");
                sql.AppendLine("                                       (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sql.AppendLine("                                       AS TIME) >");
                sql.AppendLine("                                   CAST(");
                sql.AppendLine("                                       (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sql.AppendLine("                                       AS TIME)");
                sql.AppendLine("                               )");
                sql.AppendLine("                           )");
                sql.AppendLine("                       ), 1, 0)");
                sql.AppendLine("               )");
                sql.AppendLine("       END) AS BX_FORA_PRAZO_AGEND_PERIODO");
                
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = CommandHelper.ExecuteReaderFirst(com, sql, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var produtividadeTecnico = new RelatorioProdutividadeTecnicoListModel();

                        produtividadeTecnico.Matricula = Convert.ToString(reader["MATRICULA"]);
                        produtividadeTecnico.Tecnico = Convert.ToString(reader["TECNICO"]);
                        
                        produtividadeTecnico.Supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) : "-";
                        produtividadeTecnico.Monitor = reader["MONITOR"] != DBNull.Value ? Convert.ToString(reader["MONITOR"]) : "-";

                        produtividadeTecnico.NaturezaServico = Convert.ToString(reader["NATUREZA"]);

                        produtividadeTecnico.QtdServicosProdutivos = Convert.ToInt32(reader["QTD_SERVICOS_PRODUTIVOS"]);
                        produtividadeTecnico.QtdServicosImprodutivos = Convert.ToInt32(reader["QTD_SERVICOS_IMPRODUTIVOS"]);
                        produtividadeTecnico.MetaPercentualServicosProdutivos =
                            reader["META_PRODUTIVA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_PRODUTIVA"]) :
                                (double?)null;

                        produtividadeTecnico.QtdReincidencia = Convert.ToInt32(reader["QTD_SERVICOS_REINC"]);
                        produtividadeTecnico.MetaPercentualReincidencia =
                            reader["META_REINCIDENCIA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_REINCIDENCIA"]) :
                                (double?)null;

                        if (usaAgendamentoPeriodo)
                        {
                            produtividadeTecnico.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO_AGEND_PERIODO"]);
                            produtividadeTecnico.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO_AGEND_PERIODO"]);
                        }
                        else
                        {
                            produtividadeTecnico.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO"]);
                            produtividadeTecnico.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO"]);
                        }

                        produtividadeTecnico.MetaPercentualBaixasDentroPrazo = 
                            reader["META_BX_PRAZO"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_BX_PRAZO"]) :
                                (double?)null;

                        data.Add(produtividadeTecnico);
                    }
                }
            }

            return new RelatorioProdutividadeResponse
            {
                Total = total,
                Data = data
            };
        }

        public RelatorioProdutividadeResponse GetRelatorioProdutividadeSupervisor(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc)
        {
            int total = 0;
            var data = new List<RelatorioProdutividadeTecnicoListModel>();            

            var sqlRelatorio = new StringBuilder();
            var param = new Dictionary<string, object>();

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                sqlRelatorio.AppendLine("SELECT MATRICULA_SUPERV, SUPERVISOR, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_PRODUTIVOS) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_IMPRODUTIVOS) AS QTD_SERVICOS_IMPRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_REINC) AS QTD_SERVICOS_REINC,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO) AS BX_DENTRO_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO) AS BX_FORA_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO_AGEND_PERIODO) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO_AGEND_PERIODO) AS BX_FORA_PRAZO_AGEND_PERIODO");
                sqlRelatorio.AppendLine("FROM(");

                sqlRelatorio.AppendLine("    SELECT S.NUM_FRE AS MATRICULA_SUPERV, S.NOME AS SUPERVISOR, N.DESCRICAO AS NATUREZA, N.META_PRODUTIVA,");
                sqlRelatorio.AppendLine("           N.META_REINCIDENCIA, N.META_BX_PRAZO,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 2, 1, 0)) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 1, 1, 0)) AS QTD_SERVICOS_IMPRODUTIVOS,");
                
                sqlRelatorio.AppendLine("           (SELECT COUNT(*)");
                sqlRelatorio.AppendLine("            FROM TB_REINCIDENCIA_DOCPROD R");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_ANT");
                sqlRelatorio.AppendLine("            ON(DOC_ANT.CODCT = R.CODCT_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.PROJETO = R.PROJETO_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_OBRA = R.NUM_OBRA_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_DOC = R.NUM_DOC_ANTERIOR)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_ANT");
                sqlRelatorio.AppendLine("            ON(T_SERV_ANT.COD_TIPO_SERV = DOC_ANT.COD_TIPO_SERV)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_R");
                sqlRelatorio.AppendLine("            ON(DOC_R.CODCT = R.CODCT AND");
                sqlRelatorio.AppendLine("                DOC_R.PROJETO = R.PROJETO AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_OBRA = R.NUM_OBRA AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_DOC = R.NUM_DOC)");
                sqlRelatorio.AppendLine("            WHERE DOC_ANT.EQUIPE = D.EQUIPE");
                sqlRelatorio.AppendLine("              AND T_SERV_ANT.COD_NATUREZA = N.ID");

                if (apenasServicosProdutivosReinc)
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO = 2");
                }
                else
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO <> 9");
                }

                sqlRelatorio.AppendLine("              AND DOC_R.DIA BETWEEN @DATA_INI AND @DATA_FIM) AS QTD_SERVICOS_REINC,");
                
                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) >");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                    AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NULL OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               (");
                sqlRelatorio.AppendLine("                                   D.DATA_PROD > D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                                   (");
                sqlRelatorio.AppendLine("                                       D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME) >");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME)");
                sqlRelatorio.AppendLine("                                   )");
                sqlRelatorio.AppendLine("                               )");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO_AGEND_PERIODO");

                sqlRelatorio.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO D");
                
                sqlRelatorio.AppendLine("    INNER JOIN TB_EQUIPE E");
                sqlRelatorio.AppendLine("    ON(E.EQUIPE = D.EQUIPE)");
                
                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO S");
                sqlRelatorio.AppendLine("    ON(S.NUM_FRE = E.COD_SUPERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_SENHA U_MON");
                sqlRelatorio.AppendLine("    ON(U_MON.USUARIO = D.USUARIO_MONITOR)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO M");
                sqlRelatorio.AppendLine("    ON(M.NUM_FRE = U_MON.NUM_FRE)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV");
                sqlRelatorio.AppendLine("    ON(T_SERV.COD_TIPO_SERV = D.COD_TIPO_SERV)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_NATUREZA_PRODUCAO N");
                sqlRelatorio.AppendLine("    ON(N.ID = T_SERV.COD_NATUREZA)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_TURNO_SERVICO T");
                sqlRelatorio.AppendLine("    ON(T.ID = D.COD_TURNO_SERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL A");
                sqlRelatorio.AppendLine("    ON(A.ID = D.ID_AGENDAMENTO)");

                sqlRelatorio.AppendLine("    WHERE D.STATUS_EXECUCAO IN(1, 2)");

                #region Filtros

                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                sqlRelatorio.AppendLine("  AND D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);

                if (request.search != null && !String.IsNullOrEmpty(request.search.value))
                {
                    string search = request.search.value.ToUpperInvariant();
                    sqlRelatorio.AppendLine($@"  AND UPPER(D.EQUIPE) LIKE '%{search}%' OR UPPER(E.NOME) LIKE '%{search}%'");
                }

                if (!String.IsNullOrEmpty(request.GrupoArea))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_ESCRITORIO = @GRUPO_AREA");
                    param.Add("@GRUPO_AREA", request.GrupoArea);
                }

                if (!String.IsNullOrEmpty(request.Area))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }

                if (!String.IsNullOrEmpty(request.Monitor))
                {
                    sqlRelatorio.AppendLine($@"  AND D.USUARIO_MONITOR = @MONITOR");
                    param.Add("@MONITOR", request.Monitor);
                }

                if (!String.IsNullOrEmpty(request.Supervisor))
                {
                    sqlRelatorio.AppendLine($@"  AND E.COD_SUPERV = @SUPERVISOR");
                    param.Add("@SUPERVISOR", request.Supervisor);
                }

                if (!String.IsNullOrEmpty(request.Equipe))
                {
                    sqlRelatorio.AppendLine($@"  AND D.EQUIPE = @EQUIPE");
                    param.Add("@EQUIPE", request.Equipe);
                }

                if (request.Natureza.HasValue)
                {
                    sqlRelatorio.AppendLine($@"  AND N.ID = @NATUREZA");
                    param.Add("@NATUREZA", request.Natureza.Value);
                }

                #endregion

                sqlRelatorio.AppendLine("    GROUP BY D.EQUIPE, E.NOME, S.NUM_FRE, E.COD_SUPERV, N.ID, N.DESCRICAO, N.BASE_PRAZO, S.NOME, M.NOME,");
                sqlRelatorio.AppendLine("             N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO");
                sqlRelatorio.AppendLine(")");

                sqlRelatorio.AppendLine("GROUP BY MATRICULA_SUPERV, SUPERVISOR, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO");

                sqlRelatorio.AppendLine("ORDER BY SUPERVISOR, NATUREZA");

                using (var reader = CommandHelper.ExecuteReaderFirst(com, sqlRelatorio, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var prodSupervisor = new RelatorioProdutividadeTecnicoListModel();

                        prodSupervisor.Matricula = Convert.ToString(reader["MATRICULA_SUPERV"]);
                        prodSupervisor.Supervisor = Convert.ToString(reader["SUPERVISOR"]);

                        if (String.IsNullOrEmpty(prodSupervisor.Supervisor))
                            prodSupervisor.Supervisor = "Sem supervisor";

                        prodSupervisor.NaturezaServico = Convert.ToString(reader["NATUREZA"]);

                        prodSupervisor.QtdServicosProdutivos = Convert.ToInt32(reader["QTD_SERVICOS_PRODUTIVOS"]);
                        prodSupervisor.QtdServicosImprodutivos = Convert.ToInt32(reader["QTD_SERVICOS_IMPRODUTIVOS"]);
                        prodSupervisor.MetaPercentualServicosProdutivos =
                            reader["META_PRODUTIVA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_PRODUTIVA"]) :
                                (double?)null;

                        prodSupervisor.QtdReincidencia = Convert.ToInt32(reader["QTD_SERVICOS_REINC"]);
                        prodSupervisor.MetaPercentualReincidencia =
                            reader["META_REINCIDENCIA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_REINCIDENCIA"]) :
                                (double?)null;

                        if (usaAgendamentoPeriodo)
                        {
                            prodSupervisor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO_AGEND_PERIODO"]);
                            prodSupervisor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO_AGEND_PERIODO"]);
                        }
                        else
                        {
                            prodSupervisor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO"]);
                            prodSupervisor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO"]);
                        }

                        prodSupervisor.MetaPercentualBaixasDentroPrazo =
                            reader["META_BX_PRAZO"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_BX_PRAZO"]) :
                                (double?)null;

                        data.Add(prodSupervisor);
                    }
                }
            }

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sqlCount = new StringBuilder();

                sqlCount.AppendLine("SELECT COUNT(*) AS QTD_REGISTROS");
                sqlCount.AppendLine("FROM(");
                sqlCount.AppendLine(sqlRelatorio.ToString());
                sqlCount.AppendLine(")");

                using (var reader = CommandHelper.ExecuteReader(com, sqlCount, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_REGISTROS"]);
                }
            }

            return new RelatorioProdutividadeResponse
            {
                Total = total,
                Data = data
            };
        }

        public RelatorioProdutividadeResponse GetRelatorioProdutividadeMonitor(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc)
        {
            int total = 0;
            var data = new List<RelatorioProdutividadeTecnicoListModel>();

            var sqlRelatorio = new StringBuilder();
            var param = new Dictionary<string, object>();

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                sqlRelatorio.AppendLine("SELECT MATRICULA_MON, MONITOR, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_PRODUTIVOS) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_IMPRODUTIVOS) AS QTD_SERVICOS_IMPRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_REINC) AS QTD_SERVICOS_REINC,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO) AS BX_DENTRO_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO) AS BX_FORA_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO_AGEND_PERIODO) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO_AGEND_PERIODO) AS BX_FORA_PRAZO_AGEND_PERIODO");
                sqlRelatorio.AppendLine("FROM(");

                sqlRelatorio.AppendLine("    SELECT M.NUM_FRE AS MATRICULA_MON, M.NOME AS MONITOR, N.DESCRICAO AS NATUREZA, N.META_PRODUTIVA,");
                sqlRelatorio.AppendLine("           N.META_REINCIDENCIA, N.META_BX_PRAZO,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 2, 1, 0)) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 1, 1, 0)) AS QTD_SERVICOS_IMPRODUTIVOS,");

                sqlRelatorio.AppendLine("           (SELECT COUNT(*)");
                sqlRelatorio.AppendLine("            FROM TB_REINCIDENCIA_DOCPROD R");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_ANT");
                sqlRelatorio.AppendLine("            ON(DOC_ANT.CODCT = R.CODCT_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.PROJETO = R.PROJETO_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_OBRA = R.NUM_OBRA_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_DOC = R.NUM_DOC_ANTERIOR)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_ANT");
                sqlRelatorio.AppendLine("            ON(T_SERV_ANT.COD_TIPO_SERV = DOC_ANT.COD_TIPO_SERV)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_R");
                sqlRelatorio.AppendLine("            ON(DOC_R.CODCT = R.CODCT AND");
                sqlRelatorio.AppendLine("                DOC_R.PROJETO = R.PROJETO AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_OBRA = R.NUM_OBRA AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_DOC = R.NUM_DOC)");
                sqlRelatorio.AppendLine("            WHERE DOC_ANT.EQUIPE = D.EQUIPE");
                sqlRelatorio.AppendLine("              AND T_SERV_ANT.COD_NATUREZA = N.ID");

                if (apenasServicosProdutivosReinc)
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO = 2");
                }
                else
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO <> 9");
                }

                sqlRelatorio.AppendLine("              AND DOC_R.DIA BETWEEN @DATA_INI AND @DATA_FIM) AS QTD_SERVICOS_REINC,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) >");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                    AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NULL OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               (");
                sqlRelatorio.AppendLine("                                   D.DATA_PROD > D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                                   (");
                sqlRelatorio.AppendLine("                                       D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME) >");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME)");
                sqlRelatorio.AppendLine("                                   )");
                sqlRelatorio.AppendLine("                               )");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO_AGEND_PERIODO");

                sqlRelatorio.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO D");
                sqlRelatorio.AppendLine("    INNER JOIN TB_EQUIPE E");

                sqlRelatorio.AppendLine("    ON(E.EQUIPE = D.EQUIPE)");
                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO S");
                sqlRelatorio.AppendLine("    ON(S.NUM_FRE = E.COD_SUPERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_SENHA U_MON");
                sqlRelatorio.AppendLine("    ON(U_MON.USUARIO = D.USUARIO_MONITOR)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO M");
                sqlRelatorio.AppendLine("    ON(M.NUM_FRE = U_MON.NUM_FRE)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV");
                sqlRelatorio.AppendLine("    ON(T_SERV.COD_TIPO_SERV = D.COD_TIPO_SERV)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_NATUREZA_PRODUCAO N");
                sqlRelatorio.AppendLine("    ON(N.ID = T_SERV.COD_NATUREZA)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_TURNO_SERVICO T");
                sqlRelatorio.AppendLine("    ON(T.ID = D.COD_TURNO_SERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL A");
                sqlRelatorio.AppendLine("    ON(A.ID = D.ID_AGENDAMENTO)");

                sqlRelatorio.AppendLine("    WHERE D.STATUS_EXECUCAO IN(1, 2)");

                #region Filtros

                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                sqlRelatorio.AppendLine("  AND D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);

                if (request.search != null && !String.IsNullOrEmpty(request.search.value))
                {
                    string search = request.search.value.ToUpperInvariant();
                    sqlRelatorio.AppendLine($@"  AND UPPER(D.EQUIPE) LIKE '%{search}%' OR UPPER(E.NOME) LIKE '%{search}%'");
                }

                if (!String.IsNullOrEmpty(request.GrupoArea))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_ESCRITORIO = @GRUPO_AREA");
                    param.Add("@GRUPO_AREA", request.GrupoArea);
                }

                if (!String.IsNullOrEmpty(request.Area))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }

                if (!String.IsNullOrEmpty(request.Monitor))
                {
                    sqlRelatorio.AppendLine($@"  AND D.USUARIO_MONITOR = @MONITOR");
                    param.Add("@MONITOR", request.Monitor);
                }

                if (!String.IsNullOrEmpty(request.Supervisor))
                {
                    sqlRelatorio.AppendLine($@"  AND E.COD_SUPERV = @SUPERVISOR");
                    param.Add("@SUPERVISOR", request.Supervisor);
                }

                if (!String.IsNullOrEmpty(request.Equipe))
                {
                    sqlRelatorio.AppendLine($@"  AND D.EQUIPE = @EQUIPE");
                    param.Add("@EQUIPE", request.Equipe);
                }

                if (request.Natureza.HasValue)
                {
                    sqlRelatorio.AppendLine($@"  AND N.ID = @NATUREZA");
                    param.Add("@NATUREZA", request.Natureza.Value);
                }

                #endregion

                sqlRelatorio.AppendLine("    GROUP BY D.EQUIPE, E.NOME, E.COD_SUPERV, N.ID, N.DESCRICAO, N.BASE_PRAZO, S.NOME, M.NUM_FRE, M.NOME,");
                sqlRelatorio.AppendLine("             N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO");
                sqlRelatorio.AppendLine(")");

                sqlRelatorio.AppendLine("GROUP BY MATRICULA_MON, MONITOR, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO");

                sqlRelatorio.AppendLine("ORDER BY MONITOR, NATUREZA");

                using (var reader = CommandHelper.ExecuteReaderFirst(com, sqlRelatorio, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var prodMonitor = new RelatorioProdutividadeTecnicoListModel();

                        prodMonitor.Matricula = Convert.ToString(reader["MATRICULA_MON"]);
                        prodMonitor.Monitor = Convert.ToString(reader["MONITOR"]);

                        if (String.IsNullOrEmpty(prodMonitor.Monitor))
                            prodMonitor.Monitor = "Sem monitor";

                        prodMonitor.NaturezaServico = Convert.ToString(reader["NATUREZA"]);

                        prodMonitor.QtdServicosProdutivos = Convert.ToInt32(reader["QTD_SERVICOS_PRODUTIVOS"]);
                        prodMonitor.QtdServicosImprodutivos = Convert.ToInt32(reader["QTD_SERVICOS_IMPRODUTIVOS"]);
                        prodMonitor.MetaPercentualServicosProdutivos =
                            reader["META_PRODUTIVA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_PRODUTIVA"]) :
                                (double?)null;

                        prodMonitor.QtdReincidencia = Convert.ToInt32(reader["QTD_SERVICOS_REINC"]);
                        prodMonitor.MetaPercentualReincidencia =
                            reader["META_REINCIDENCIA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_REINCIDENCIA"]) :
                                (double?)null;

                        if (usaAgendamentoPeriodo)
                        {
                            prodMonitor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO_AGEND_PERIODO"]);
                            prodMonitor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO_AGEND_PERIODO"]);
                        }
                        else
                        {
                            prodMonitor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO"]);
                            prodMonitor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO"]);
                        }

                        prodMonitor.MetaPercentualBaixasDentroPrazo =
                            reader["META_BX_PRAZO"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_BX_PRAZO"]) :
                                (double?)null;

                        data.Add(prodMonitor);
                    }
                }
            }

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sqlCount = new StringBuilder();

                sqlCount.AppendLine("SELECT COUNT(*) AS QTD_REGISTROS");
                sqlCount.AppendLine("FROM(");
                sqlCount.AppendLine(sqlRelatorio.ToString());
                sqlCount.AppendLine(")");

                using (var reader = CommandHelper.ExecuteReader(com, sqlCount, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_REGISTROS"]);
                }
            }

            return new RelatorioProdutividadeResponse
            {
                Total = total,
                Data = data
            };
        }

        public RelatorioProdutividadeResponse GetRelatorioProdutividadeAgrupadoTecnico(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc)
        {
            int total = 0;
            var data = new List<RelatorioProdutividadeTecnicoListModel>();

            var sqlRelatorio = new StringBuilder();
            var param = new Dictionary<string, object>();

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                sqlRelatorio.AppendLine("SELECT EQUIPE, NOME_EQUIPE, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_PRODUTIVOS) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_IMPRODUTIVOS) AS QTD_SERVICOS_IMPRODUTIVOS,");
                sqlRelatorio.AppendLine("       SUM(QTD_SERVICOS_REINC) AS QTD_SERVICOS_REINC,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO) AS BX_DENTRO_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO) AS BX_FORA_PRAZO,");
                sqlRelatorio.AppendLine("       SUM(BX_DENTRO_PRAZO_AGEND_PERIODO) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");
                sqlRelatorio.AppendLine("       SUM(BX_FORA_PRAZO_AGEND_PERIODO) AS BX_FORA_PRAZO_AGEND_PERIODO");
                sqlRelatorio.AppendLine("FROM(");

                sqlRelatorio.AppendLine("    SELECT D.EQUIPE, E.NOME AS NOME_EQUIPE, N.DESCRICAO AS NATUREZA, N.META_PRODUTIVA,");
                sqlRelatorio.AppendLine("           N.META_REINCIDENCIA, N.META_BX_PRAZO,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 2, 1, 0)) AS QTD_SERVICOS_PRODUTIVOS,");
                sqlRelatorio.AppendLine("           SUM(IIF(D.STATUS_EXECUCAO = 1, 1, 0)) AS QTD_SERVICOS_IMPRODUTIVOS,");

                sqlRelatorio.AppendLine("           (SELECT COUNT(*)");
                sqlRelatorio.AppendLine("            FROM TB_REINCIDENCIA_DOCPROD R");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_ANT");
                sqlRelatorio.AppendLine("            ON(DOC_ANT.CODCT = R.CODCT_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.PROJETO = R.PROJETO_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_OBRA = R.NUM_OBRA_ANTERIOR AND");
                sqlRelatorio.AppendLine("                DOC_ANT.NUM_DOC = R.NUM_DOC_ANTERIOR)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_ANT");
                sqlRelatorio.AppendLine("            ON(T_SERV_ANT.COD_TIPO_SERV = DOC_ANT.COD_TIPO_SERV)");
                sqlRelatorio.AppendLine("            INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_R");
                sqlRelatorio.AppendLine("            ON(DOC_R.CODCT = R.CODCT AND");
                sqlRelatorio.AppendLine("                DOC_R.PROJETO = R.PROJETO AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_OBRA = R.NUM_OBRA AND");
                sqlRelatorio.AppendLine("                DOC_R.NUM_DOC = R.NUM_DOC)");
                sqlRelatorio.AppendLine("            WHERE DOC_ANT.EQUIPE = D.EQUIPE");
                sqlRelatorio.AppendLine("              AND T_SERV_ANT.COD_NATUREZA = N.ID");

                if (apenasServicosProdutivosReinc)
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO = 2");
                }
                else
                {
                    sqlRelatorio.AppendLine("              AND DOC_R.STATUS_EXECUCAO <> 9");
                }

                sqlRelatorio.AppendLine("              AND DOC_R.DIA BETWEEN @DATA_INI AND @DATA_FIM) AS QTD_SERVICOS_REINC,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > CAST(A.DATAHORA_FIM AS DATE) OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = CAST(A.DATAHORA_FIM AS DATE) AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) >");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM A.DATAHORA_FIM) || ':' || EXTRACT(MINUTE FROM A.DATAHORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD <= D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                           D.DATA_PROD < D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                   AS TIME) <=");
                sqlRelatorio.AppendLine("                               CAST(");
                sqlRelatorio.AppendLine("                                   (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                    AS TIME)");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_DENTRO_PRAZO_AGEND_PERIODO,");

                sqlRelatorio.AppendLine("          (CASE");
                sqlRelatorio.AppendLine("               WHEN N.BASE_PRAZO = 1 THEN");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           D.DATA_PROD > D.DATA_INDICADOR OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD = D.DATA_INDICADOR AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD > D.HORA_INDICADOR");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("               ELSE");
                sqlRelatorio.AppendLine("                   SUM(");
                sqlRelatorio.AppendLine("                       IIF(");
                sqlRelatorio.AppendLine("                           T.ID IS NULL OR");
                sqlRelatorio.AppendLine("                           (");
                sqlRelatorio.AppendLine("                               D.DATA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               D.HORA_PROD IS NOT NULL AND");
                sqlRelatorio.AppendLine("                               (");
                sqlRelatorio.AppendLine("                                   D.DATA_PROD > D.DATA_MAXIMA OR");
                sqlRelatorio.AppendLine("                                   (");
                sqlRelatorio.AppendLine("                                       D.DATA_PROD = D.DATA_MAXIMA AND");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM D.HORA_PROD) || ':' || EXTRACT(MINUTE FROM D.HORA_PROD) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME) >");
                sqlRelatorio.AppendLine("                                       CAST(");
                sqlRelatorio.AppendLine("                                           (EXTRACT(HOUR FROM T.HORA_FIM) || ':' || EXTRACT(MINUTE FROM T.HORA_FIM) || ':00')");
                sqlRelatorio.AppendLine("                                           AS TIME)");
                sqlRelatorio.AppendLine("                                   )");
                sqlRelatorio.AppendLine("                               )");
                sqlRelatorio.AppendLine("                           ), 1, 0)");
                sqlRelatorio.AppendLine("                   )");
                sqlRelatorio.AppendLine("           END) AS BX_FORA_PRAZO_AGEND_PERIODO");

                sqlRelatorio.AppendLine("    FROM TB_DOCUMENTO_PRODUCAO D");
                sqlRelatorio.AppendLine("    INNER JOIN TB_EQUIPE E");

                sqlRelatorio.AppendLine("    ON(E.EQUIPE = D.EQUIPE)");
                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO S");
                sqlRelatorio.AppendLine("    ON(S.NUM_FRE = E.COD_SUPERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_SENHA U_MON");
                sqlRelatorio.AppendLine("    ON(U_MON.USUARIO = D.USUARIO_MONITOR)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_FUNCIONARIO M");
                sqlRelatorio.AppendLine("    ON(M.NUM_FRE = U_MON.NUM_FRE)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_TIPO_SERVICO_EQUIPE T_SERV");
                sqlRelatorio.AppendLine("    ON(T_SERV.COD_TIPO_SERV = D.COD_TIPO_SERV)");

                sqlRelatorio.AppendLine("    INNER JOIN TB_NATUREZA_PRODUCAO N");
                sqlRelatorio.AppendLine("    ON(N.ID = T_SERV.COD_NATUREZA)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_TURNO_SERVICO T");
                sqlRelatorio.AppendLine("    ON(T.ID = D.COD_TURNO_SERV)");

                sqlRelatorio.AppendLine("    LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL A");
                sqlRelatorio.AppendLine("    ON(A.ID = D.ID_AGENDAMENTO)");

                sqlRelatorio.AppendLine("    WHERE D.STATUS_EXECUCAO IN(1, 2)");

                #region Filtros

                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                sqlRelatorio.AppendLine("  AND D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);

                if (request.search != null && !String.IsNullOrEmpty(request.search.value))
                {
                    string search = request.search.value.ToUpperInvariant();
                    sqlRelatorio.AppendLine($@"  AND UPPER(D.EQUIPE) LIKE '%{search}%' OR UPPER(E.NOME) LIKE '%{search}%'");
                }

                if (!String.IsNullOrEmpty(request.GrupoArea))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_ESCRITORIO = @GRUPO_AREA");
                    param.Add("@GRUPO_AREA", request.GrupoArea);
                }

                if (!String.IsNullOrEmpty(request.Area))
                {
                    sqlRelatorio.AppendLine($@"  AND D.COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }

                if (!String.IsNullOrEmpty(request.Monitor))
                {
                    sqlRelatorio.AppendLine($@"  AND D.USUARIO_MONITOR = @MONITOR");
                    param.Add("@MONITOR", request.Monitor);
                }

                if (!String.IsNullOrEmpty(request.Supervisor))
                {
                    sqlRelatorio.AppendLine($@"  AND E.COD_SUPERV = @SUPERVISOR");
                    param.Add("@SUPERVISOR", request.Supervisor);
                }

                if (!String.IsNullOrEmpty(request.Equipe))
                {
                    sqlRelatorio.AppendLine($@"  AND D.EQUIPE = @EQUIPE");
                    param.Add("@EQUIPE", request.Equipe);
                }

                if (request.Natureza.HasValue)
                {
                    sqlRelatorio.AppendLine($@"  AND N.ID = @NATUREZA");
                    param.Add("@NATUREZA", request.Natureza.Value);
                }

                #endregion

                sqlRelatorio.AppendLine("    GROUP BY D.EQUIPE, E.NOME, E.COD_SUPERV, N.ID, N.DESCRICAO, N.BASE_PRAZO, S.NOME, M.NUM_FRE, M.NOME,");
                sqlRelatorio.AppendLine("             N.META_PRODUTIVA, N.META_REINCIDENCIA, N.META_BX_PRAZO");
                sqlRelatorio.AppendLine(")");

                sqlRelatorio.AppendLine("GROUP BY EQUIPE, NOME_EQUIPE, NATUREZA, META_PRODUTIVA, META_REINCIDENCIA, META_BX_PRAZO");

                sqlRelatorio.AppendLine("ORDER BY NOME_EQUIPE, NATUREZA");

                using (var reader = CommandHelper.ExecuteReaderFirst(com, sqlRelatorio, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var prodMonitor = new RelatorioProdutividadeTecnicoListModel();

                        prodMonitor.Matricula = Convert.ToString(reader["EQUIPE"]);
                        prodMonitor.Tecnico = Convert.ToString(reader["NOME_EQUIPE"]);

                        

                        prodMonitor.NaturezaServico = Convert.ToString(reader["NATUREZA"]);

                        prodMonitor.QtdServicosProdutivos = Convert.ToInt32(reader["QTD_SERVICOS_PRODUTIVOS"]);
                        prodMonitor.QtdServicosImprodutivos = Convert.ToInt32(reader["QTD_SERVICOS_IMPRODUTIVOS"]);
                        prodMonitor.MetaPercentualServicosProdutivos =
                            reader["META_PRODUTIVA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_PRODUTIVA"]) :
                                (double?)null;

                        prodMonitor.QtdReincidencia = Convert.ToInt32(reader["QTD_SERVICOS_REINC"]);
                        prodMonitor.MetaPercentualReincidencia =
                            reader["META_REINCIDENCIA"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_REINCIDENCIA"]) :
                                (double?)null;

                        if (usaAgendamentoPeriodo)
                        {
                            prodMonitor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO_AGEND_PERIODO"]);
                            prodMonitor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO_AGEND_PERIODO"]);
                        }
                        else
                        {
                            prodMonitor.BaixasDentroPrazo = Convert.ToInt32(reader["BX_DENTRO_PRAZO"]);
                            prodMonitor.BaixasForaPrazo = Convert.ToInt32(reader["BX_FORA_PRAZO"]);
                        }

                        prodMonitor.MetaPercentualBaixasDentroPrazo =
                            reader["META_BX_PRAZO"] != DBNull.Value ?
                                Convert.ToDouble(reader["META_BX_PRAZO"]) :
                                (double?)null;

                        data.Add(prodMonitor);
                    }
                }
            }

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sqlCount = new StringBuilder();

                sqlCount.AppendLine("SELECT COUNT(*) AS QTD_REGISTROS");
                sqlCount.AppendLine("FROM(");
                sqlCount.AppendLine(sqlRelatorio.ToString());
                sqlCount.AppendLine(")");

                using (var reader = CommandHelper.ExecuteReader(com, sqlCount, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_REGISTROS"]);
                }
            }

            return new RelatorioProdutividadeResponse
            {
                Total = total,
                Data = data
            };
        }
    }
}
