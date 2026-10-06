using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.AplicacaoMaterialHubsoft;
using Aniel.Connect.Repository.Model.Azza;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.HubSoft.MovimentoCliente;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using HubSoftService.Model.HubSoft.Entrada;
using Microsoft.SqlServer.Server;
using MongoDB.Driver;
using NHibernate;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Aniel.Connect.Repository.T5.DocumentoProducao
{
    public class DocumentoProducaoAnielRepository : IDocumentoProducaoAnielRepository
    {
        private readonly ISession _sessionT5Nh;
        private readonly ISession _sessionT5Sql;

        public DocumentoProducaoAnielRepository(ISession sessionT5Nh, ISession sessionT5Sql)
        {
            this._sessionT5Nh = sessionT5Nh;
            this._sessionT5Sql = sessionT5Sql;
        }

        public IQueryable<Documento_ProducaoAniel> GetAll()
        {
            using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(this._sessionT5Nh))
            {
                return repoDocProd.QueryableSelect();
            }
        }


        public void SaveOrUpdate(Documento_ProducaoAniel documento)
        {
            using (var repoDocProd = new Conexao<Documento_ProducaoAniel>(this._sessionT5Nh))
            {
                repoDocProd.SaveOrUpdate(documento).Wait();
            }
        }

        public ChavesDocumentoProducao GetOSReincidente(string codct, string projeto, string numObra, int numDoc, DateTime dataCriacao)
        {
            ChavesDocumentoProducao osOrigemReincidencia = null;

            var configReincidencia =
                this.GetAll()
                    .Where(os => os.CodCt == codct &&
                                 os.Projeto == projeto &&
                                 os.Num_Obra == numObra &&
                                 os.Num_Doc == numDoc)
                    .Select(os => new
                    {
                        GeraReincidencia = os.TipoServico.Natureza.GeraReincidencia,
                        QtdDiasReincidencia = os.TipoServico.Natureza.QtdDias_GeraReincidencia
                    })
                    .FirstOrDefault();

            if (configReincidencia != null &&
                configReincidencia.GeraReincidencia == "S" &&
                configReincidencia.QtdDiasReincidencia.HasValue)
            {
                using (var com = this._sessionT5Sql.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();
                    var param = new Dictionary<string, object>();

                    sql.AppendLine("SELECT FIRST 1 DOCUMENTO.CODCT, DOCUMENTO.PROJETO, DOCUMENTO.NUM_OBRA, DOCUMENTO.NUM_DOC");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO DOCUMENTO");
                    sql.AppendLine("WHERE DOCUMENTO.CODCT = @CODCT AND");
                    sql.AppendLine("      DOCUMENTO.PROJETO = @PROJETO AND");
                    sql.AppendLine("      DOCUMENTO.NUM_OBRA <> @NUM_OBRA AND");
                    sql.AppendLine("      DOCUMENTO.NUM_DOC = @NUM_DOC AND");
                    sql.AppendLine("      DOCUMENTO.STATUS_EXECUCAO = 2 AND");
                    sql.AppendLine("      (@DIA_CRIACAO BETWEEN DOCUMENTO.DIA AND DATEADD(DAY, @QTD_DIAS_REINCIDENCIA, DOCUMENTO.DIA)) AND");
                    sql.AppendLine("      NOT EXISTS(SELECT FIRST 1 R.*");
                    sql.AppendLine("                 FROM TB_REINCIDENCIA_DOCPROD R");
                    sql.AppendLine("                 WHERE R.CODCT_ANTERIOR = DOCUMENTO.CODCT AND");
                    sql.AppendLine("                       R.PROJETO_ANTERIOR = DOCUMENTO.PROJETO AND");
                    sql.AppendLine("                       R.NUM_DOC_ANTERIOR = DOCUMENTO.NUM_DOC AND");
                    sql.AppendLine("                       R.NUM_OBRA_ANTERIOR = DOCUMENTO.NUM_OBRA)");

                    param.Add("@CODCT", codct);
                    param.Add("@PROJETO", projeto);
                    param.Add("@NUM_OBRA", numObra);
                    param.Add("@NUM_DOC", numDoc);

                    param.Add("@DIA_CRIACAO", dataCriacao);
                    param.Add("@QTD_DIAS_REINCIDENCIA", configReincidencia.QtdDiasReincidencia);

                    using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionT5Sql.Connection, null).Result)
                    {
                        if (reader.Read())
                        {
                            osOrigemReincidencia = new ChavesDocumentoProducao
                            {
                                Codct = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                NumObra = Convert.ToString(reader["NUM_OBRA"]),
                                NumDoc = Convert.ToInt32(reader["NUM_DOC"])
                            };
                        }
                    }
                }
            }

            return osOrigemReincidencia;
        }

        public RelProdTecnicoExtratoProdutividadeResponse GetExtratoProdutividade(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool considerarAgendamentoPeriodo)
        {
            int total = 0;
            var servicos = new List<RelProdTecnicoExtratoProdutividadeListModel>();

            #region SQL From

            var sqlFrom = new StringBuilder();
            var param = new Dictionary<string, object>();

            sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");

            sqlFrom.AppendLine("LEFT JOIN TB_TURNO_SERVICO TS");
            sqlFrom.AppendLine("ON(D.COD_TURNO_SERV = TS.ID)");

            sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE E");
            sqlFrom.AppendLine("ON(D.EQUIPE = E.EQUIPE)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO S");
            sqlFrom.AppendLine("ON(S.NUM_FRE = E.COD_SUPERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_SENHA SEN");
            sqlFrom.AppendLine("ON(SEN.USUARIO = D.USUARIO_MONITOR)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO M");
            sqlFrom.AppendLine("ON(M.NUM_FRE = SEN.NUM_FRE)");

            sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE T");
            sqlFrom.AppendLine("ON(D.COD_TIPO_SERV = T.COD_TIPO_SERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO N");
            sqlFrom.AppendLine("ON(T.COD_NATUREZA = N.ID)");

            sqlFrom.AppendLine("LEFT JOIN TB_ESCRITORIO ESC");
            sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = ESC.COD_ESCRITORIO)");

            sqlFrom.AppendLine("LEFT JOIN TB_AREA A");
            sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = A.COD_ESCRITORIO AND");
            sqlFrom.AppendLine("   A.COD_CLIENTE = 1 AND");
            sqlFrom.AppendLine("   D.COD_AREA = A.COD_AREA)");

            sqlFrom.AppendLine("LEFT JOIN TB_LOCALIDADE L");
            sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = L.COD_ESCRITORIO AND");
            sqlFrom.AppendLine("   L.COD_CLIENTE = 1 AND");
            sqlFrom.AppendLine("   D.COD_AREA = L.COD_AREA AND");
            sqlFrom.AppendLine("   D.COD_LOCALIDADE = L.COD_LOCALIDADE)");

            sqlFrom.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO C ");
            sqlFrom.AppendLine("ON (D.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND ");
            sqlFrom.AppendLine("    D.CODCT = C.CODCT AND ");
            sqlFrom.AppendLine("    (C.COD_TIPO_SERV = 0 OR C.COD_TIPO_SERV = D.COD_TIPO_SERV)) ");


            sqlFrom.AppendLine("LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AG");
            sqlFrom.AppendLine("ON(AG.ID = D.ID_AGENDAMENTO)");

            sqlFrom.AppendLine("LEFT JOIN TB_SENHA SENHA_USUARIO_BAIXA");
            sqlFrom.AppendLine("ON(SENHA_USUARIO_BAIXA.USUARIO = D.USUARIO_BAIXA)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_BAIXA");
            sqlFrom.AppendLine("ON(FUNCIONARIO_BAIXA.NUM_FRE = SENHA_USUARIO_BAIXA.NUM_FRE)");

            // sqlFrom.AppendLine("WHERE D.EQUIPE IS NOT NULL ");
            sqlFrom.AppendLine(" WHERE D.STATUS_EXECUCAO IN('1', '2') ");

            // Para não vir tecnico nulo com OS baixada como produtiva
            sqlFrom.AppendLine(" AND NOT (D.STATUS_EXECUCAO = '2' AND D.EQUIPE IS NULL) ");

            #endregion

            #region Filtros

            var filtros = new List<string>();

            if (request.search != null && !String.IsNullOrEmpty(request.search.value))
            {
                string search = request.search.value.ToUpperInvariant();
                filtros.Add($@"(UPPER(E.NOME) LIKE '%{search}%' OR UPPER(D.EQUIPE) LIKE '%{search}%')");
            }

            if (!String.IsNullOrEmpty(request.DataInicio) && !String.IsNullOrEmpty(request.DataFim))
            {
                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                filtros.Add("D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);
            }

            if (!String.IsNullOrEmpty(request.GrupoArea))
            {
                filtros.Add("D.COD_ESCRITORIO = @GRUPO_AREA");
                param.Add("@GRUPO_AREA", request.GrupoArea);

                if (!String.IsNullOrEmpty(request.Area))
                {
                    filtros.Add("D.COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }
            }

            if (!String.IsNullOrEmpty(request.Monitor))
            {
                filtros.Add("D.USUARIO_MONITOR = @MONITOR");
                param.Add("@MONITOR", request.Monitor);
            }

            if (!String.IsNullOrEmpty(request.Supervisor))
            {
                filtros.Add("E.COD_SUPERV = @SUPERVISOR");
                param.Add("@SUPERVISOR", request.Supervisor);
            }

            if (!String.IsNullOrEmpty(request.Equipe))
            {
                filtros.Add("D.EQUIPE = @EQUIPE");
                param.Add("@EQUIPE", request.Equipe);
            }

            if (request.Natureza.HasValue)
            {
                filtros.Add("N.ID = @NATUREZA");
                param.Add("@NATUREZA", request.Natureza);
            }

            if (filtros.Count > 0)
            {
                foreach (string filtro in filtros)
                {
                    sqlFrom.AppendLine("  AND (" + filtro + ")");
                }
            }
            #endregion

            using (var comTotal = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT COUNT(*) AS QTD_SERVICOS");
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = CommandHelper.ExecuteReader(comTotal, sql, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_SERVICOS"]);
                }
            }

            using (var comRelatorio = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT D.CODCT, D.PROJETO, D.NUM_DOC, D.NUM_OBRA, D.STATUS_AUDITORIA, D.DIA AS ABERTURA_OS,D.USUARIO_BAIXA,");
                sql.AppendLine("       COALESCE (D.EQUIPE,D.USUARIO_BAIXA) AS MATRICULA, COALESCE(E.NOME,FUNCIONARIO_BAIXA.NOME) AS TECNICO,");
                sql.AppendLine("       S.NOME AS NOME_SUPERVISOR, T.DESCRICAO AS TIPO_SERVICO,");
                sql.AppendLine("       N.DESCRICAO AS NATUREZA, M.NOME AS MONITOR, ESC.DESCRICAO AS GRUPO_AREA, A.DESCRICAO AS AREA,");
                sql.AppendLine("       L.LOCALIDADE AS LOCALIDADE, D.STATUS_EXECUCAO, D.COD_ENCERRAMENTO, C.DESCRICAO AS ENCERRAMENTO,");
                sql.AppendLine("       N.BASE_PRAZO, (D.DATA_PROD + D.HORA_PROD) AS DATA_HORA_BAIXA, (D.DATA_INDICADOR + D.HORA_INDICADOR) AS DATA_HORA_SLA,");
                sql.AppendLine("       (D.DATA_MAXIMA + COALESCE(TS.HORA_FIM, CAST('23:59:59' AS TIME))) AS DATA_HORA_FIM_TURNO, AG.DATAHORA_FIM AS DATA_HORA_FIM_AGEND_SEM_TURNO");
                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine("ORDER BY E.NOME, N.DESCRICAO");

                using (var reader = CommandHelper.ExecuteReaderFirst(comRelatorio, sql, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var servico = new RelProdTecnicoExtratoProdutividadeListModel();

                        servico.Codct = Convert.ToString(reader["CODCT"]);
                        servico.Projeto = Convert.ToString(reader["PROJETO"]);
                        servico.NumDoc = Convert.ToInt32(reader["NUM_DOC"]);
                        servico.NumObra = Convert.ToString(reader["NUM_OBRA"]);

                        servico.Abertura_OS = reader["ABERTURA_OS"] != DBNull.Value ?
                                Convert.ToDateTime(reader["ABERTURA_OS"]).ToString("dd/MM/yyyy") :
                                String.Empty;

                        servico.DataHoraBaixa =
                            reader["DATA_HORA_BAIXA"] != DBNull.Value ?
                                Convert.ToDateTime(reader["DATA_HORA_BAIXA"]).ToString("dd/MM/yyyy HH:mm") :
                                String.Empty;

                        servico.UsuarioBaixa = Convert.ToString(reader["USUARIO_BAIXA"]);

                        servico.TipoServico = Convert.ToString(reader["TIPO_SERVICO"]);
                        servico.Natureza = Convert.ToString(reader["NATUREZA"]);

                        servico.Equipe = Convert.ToString(reader["MATRICULA"]);
                        servico.Nome = Convert.ToString(reader["TECNICO"]);
                        servico.Supervisor =
                            reader["NOME_SUPERVISOR"] != DBNull.Value ?
                                Convert.ToString(reader["NOME_SUPERVISOR"]) :
                                String.Empty;
                        servico.Monitor =
                            reader["MONITOR"] != DBNull.Value ?
                                Convert.ToString(reader["MONITOR"]) :
                                String.Empty;

                        servico.GrupoArea = Convert.ToString(reader["GRUPO_AREA"]);
                        servico.Area = Convert.ToString(reader["AREA"]);
                        servico.Localidade = Convert.ToString(reader["LOCALIDADE"]);

                        servico.Status =
                            HtmlEnumExtensions.GetEnumDescription(
                                (StatusExecucaoAniel)Convert.ToInt32(reader["STATUS_EXECUCAO"])
                            );

                        #region Situação do Prazo

                        BasePrazoEnum? basePrazo =
                            reader["BASE_PRAZO"] != DBNull.Value ?
                                (BasePrazoEnum)Convert.ToInt32(reader["BASE_PRAZO"]) :
                                (BasePrazoEnum?)null;

                        if (basePrazo.HasValue)
                        {
                            DateTime? dataHoraBaixa =
                                reader["DATA_HORA_BAIXA"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_BAIXA"]) :
                                    (DateTime?)null;

                            if (dataHoraBaixa.HasValue)
                            {
                                if (basePrazo.Value == BasePrazoEnum.SLA)
                                {
                                    DateTime? dataHoraSla =
                                        reader["DATA_HORA_SLA"] != DBNull.Value ?
                                            Convert.ToDateTime(reader["DATA_HORA_SLA"]) :
                                            (DateTime?)null;

                                    if (dataHoraSla.HasValue)
                                    {
                                        if (dataHoraBaixa <= dataHoraSla)
                                            servico.SituacaoPrazo = RsCadastros.DENTRO_PRAZO;
                                        else
                                            servico.SituacaoPrazo = RsCadastros.FORA_PRAZO;
                                    }
                                    else
                                    {
                                        servico.SituacaoPrazo = "-";
                                    }
                                }
                                else
                                {
                                    DateTime? dataHoraAgendamento;

                                    if (considerarAgendamentoPeriodo)
                                    {
                                        dataHoraAgendamento =
                                            reader["DATA_HORA_FIM_TURNO"] != DBNull.Value ?
                                                Convert.ToDateTime(reader["DATA_HORA_FIM_TURNO"]) :
                                                (DateTime?)null;
                                    }
                                    else
                                    {
                                        dataHoraAgendamento =
                                            reader["DATA_HORA_FIM_AGEND_SEM_TURNO"] != DBNull.Value ?
                                                Convert.ToDateTime(reader["DATA_HORA_FIM_AGEND_SEM_TURNO"]) :
                                                (DateTime?)null;
                                    }

                                    if (dataHoraAgendamento.HasValue)
                                    {
                                        if (dataHoraBaixa <= dataHoraAgendamento)
                                            servico.SituacaoPrazo = RsCadastros.DENTRO_PRAZO;
                                        else
                                            servico.SituacaoPrazo = RsCadastros.FORA_PRAZO;
                                    }
                                    else
                                    {
                                        servico.SituacaoPrazo = "-";
                                    }
                                }
                            }
                            else
                            {
                                servico.SituacaoPrazo = "-";
                            }
                        }

                        #endregion

                        servico.CodigoEncerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]);
                        servico.DescricaoEncerramento =
                            reader["ENCERRAMENTO"] != DBNull.Value ?
                                Convert.ToString(reader["ENCERRAMENTO"]) :
                                String.Empty;

                        servico.Status_Auditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_AuditoriaAniel>(Convert.ToString(reader["STATUS_AUDITORIA"])) : String.Empty;

                        servicos.Add(servico);
                    }
                }
            }

            return new RelProdTecnicoExtratoProdutividadeResponse
            {
                Total = total,
                Data = servicos
            };
        }

        public RelProdTecnicoExtratoPrazoResponse GetExtratoPrazo(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool considerarAgendamentoPeriodo)
        {
            int total = 0;
            var servicos = new List<RelProdTecnicoExtratoPrazoListModel>();

            #region SQL From

            var sqlFrom = new StringBuilder();
            var param = new Dictionary<string, object>();




            sqlFrom.AppendLine(" FROM SP_EXTRATO_PRAZO(@DATA_INI,@DATA_FIM)  ");

            //sqlFrom.AppendLine("LEFT JOIN TB_TURNO_SERVICO TS");
            //sqlFrom.AppendLine("ON(D.COD_TURNO_SERV = TS.ID)");

            //sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE E");
            //sqlFrom.AppendLine("ON(D.EQUIPE = E.EQUIPE)");

            //sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO S");
            //sqlFrom.AppendLine("ON(S.NUM_FRE = E.COD_SUPERV)");

            //sqlFrom.AppendLine("LEFT JOIN TB_SENHA SEN");
            //sqlFrom.AppendLine("ON(SEN.USUARIO = D.USUARIO_MONITOR)");

            //sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO M");
            //sqlFrom.AppendLine("ON(M.NUM_FRE = SEN.NUM_FRE)");

            //sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE T");
            //sqlFrom.AppendLine("ON(D.COD_TIPO_SERV = T.COD_TIPO_SERV)");

            //sqlFrom.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO N");
            //sqlFrom.AppendLine("ON(T.COD_NATUREZA = N.ID)");

            //sqlFrom.AppendLine("LEFT JOIN TB_ESCRITORIO ESC");
            //sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = ESC.COD_ESCRITORIO)");

            //sqlFrom.AppendLine("LEFT JOIN TB_AREA A");
            //sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = A.COD_ESCRITORIO AND");
            //sqlFrom.AppendLine("   D.COD_AREA = A.COD_AREA)");

            //sqlFrom.AppendLine("LEFT JOIN TB_LOCALIDADE L");
            //sqlFrom.AppendLine("ON(D.COD_ESCRITORIO = L.COD_ESCRITORIO AND");
            //sqlFrom.AppendLine("   D.COD_AREA = L.COD_AREA AND");
            //sqlFrom.AppendLine("   D.COD_LOCALIDADE = L.COD_LOCALIDADE)");

            //sqlFrom.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO C");
            //sqlFrom.AppendLine("ON(D.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO)");

            //sqlFrom.AppendLine("LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AG");
            //sqlFrom.AppendLine("ON(AG.ID = D.ID_AGENDAMENTO)");

            //sqlFrom.AppendLine("WHERE D.EQUIPE IS NOT NULL ");
            //sqlFrom.AppendLine("  AND D.STATUS_EXECUCAO IN('1', '2')");

            #endregion

            #region Filtros

            var filtros = new List<string>();

            if (request.search != null && !String.IsNullOrEmpty(request.search.value))
            {
                string search = request.search.value.ToUpperInvariant();
                filtros.Add($@"(UPPER(TECNICO) LIKE '%{search}%' OR UPPER(MATRICULA) LIKE '%{search}%')");
            }

            if (!String.IsNullOrEmpty(request.DataInicio) && !String.IsNullOrEmpty(request.DataFim))
            {
                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                //filtros.Add("D.DATA_PROD BETWEEN @DATA_INI AND @DATA_FIM");

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);
            }

            if (!String.IsNullOrEmpty(request.GrupoArea))
            {
                filtros.Add("COD_ESCRITORIO = @GRUPO_AREA");
                param.Add("@GRUPO_AREA", request.GrupoArea);

                if (!String.IsNullOrEmpty(request.Area))
                {
                    filtros.Add("COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }
            }

            if (!String.IsNullOrEmpty(request.Monitor))
            {
                filtros.Add("USUARIO_MONITOR = @MONITOR");
                param.Add("@MONITOR", request.Monitor);
            }

            if (!String.IsNullOrEmpty(request.Supervisor))
            {
                filtros.Add("COD_SUPERV = @SUPERVISOR");
                param.Add("@SUPERVISOR", request.Supervisor);
            }

            if (!String.IsNullOrEmpty(request.Equipe))
            {
                filtros.Add("MATRICULA = @EQUIPE");
                param.Add("@EQUIPE", request.Equipe);
            }

            if (request.Natureza.HasValue)
            {
                filtros.Add("COD_NATUREZA = @NATUREZA");
                param.Add("@NATUREZA", request.Natureza);
            }

            if (filtros.Count > 0)
            {
                sqlFrom.AppendLine(" WHERE ");
                sqlFrom.AppendLine(filtros[0]);

                for (int i = 1; i < filtros.Count; i++)
                {
                    sqlFrom.AppendLine("  AND (" + filtros[i] + ")");
                }
            }

            #endregion

            using (var comTotal = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT COUNT(*) AS QTD_SERVICOS");
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = CommandHelper.ExecuteReader(comTotal, sql, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_SERVICOS"]);
                }
            }

            using (var comRelatorio = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                //sql.AppendLine("SELECT D.CODCT, D.PROJETO, D.NUM_DOC, D.NUM_OBRA,");
                //sql.AppendLine("       D.EQUIPE AS MATRICULA, E.NOME AS TECNICO,");
                //sql.AppendLine("       S.NOME AS NOME_SUPERVISOR, T.DESCRICAO AS TIPO_SERVICO,");
                //sql.AppendLine("       N.DESCRICAO AS NATUREZA, M.NOME AS MONITOR, ESC.DESCRICAO AS GRUPO_AREA, A.DESCRICAO AS AREA,");
                //sql.AppendLine("       L.LOCALIDADE AS LOCALIDADE, D.STATUS_EXECUCAO, D.COD_ENCERRAMENTO, C.DESCRICAO AS ENCERRAMENTO,");
                //sql.AppendLine("       N.BASE_PRAZO, (D.DATA_PROD + D.HORA_PROD) AS DATA_HORA_BAIXA,");
                //sql.AppendLine("       (D.DATA_INDICADOR + D.HORA_INDICADOR) AS DATA_HORA_SLA, TS.DESCRICAO AS TURNO,");
                //sql.AppendLine("       (D.DATA_MAXIMA + TS.HORA_INI) AS DATA_HORA_INI_TURNO,");
                //sql.AppendLine("       (D.DATA_MAXIMA + TS.HORA_FIM) AS DATA_HORA_FIM_TURNO,");
                //sql.AppendLine("       AG.DATAHORA_INICIO AS DATA_HORA_INI_AGEND_SEM_TURNO,");
                //sql.AppendLine("       AG.DATAHORA_FIM AS DATA_HORA_FIM_AGEND_SEM_TURNO");
                //sql.AppendLine(sqlFrom.ToString());
                //sql.AppendLine("ORDER BY E.NOME, N.DESCRICAO");

                sql.AppendLine("SELECT CODCT, PROJETO, NUM_DOC, NUM_OBRA, USUARIO_BAIXA,");
                sql.AppendLine("       MATRICULA, TECNICO,");
                sql.AppendLine("       NOME_SUPERVISOR, TIPO_SERVICO,");
                sql.AppendLine("       NATUREZA, MONITOR, GRUPO_AREA, AREA,");
                sql.AppendLine("       LOCALIDADE, STATUS_EXECUCAO, COD_ENCERRAMENTO, ENCERRAMENTO,");
                sql.AppendLine("       BASE_PRAZO, DATA_HORA_BAIXA,");
                sql.AppendLine("       DATA_HORA_SLA, TURNO,");
                sql.AppendLine("       DATA_HORA_INI_TURNO,");
                sql.AppendLine("       DATA_HORA_FIM_TURNO,");
                sql.AppendLine("       DATA_HORA_INI_AGEND_SEM_TURNO,");
                sql.AppendLine("       DATA_HORA_FIM_AGEND_SEM_TURNO");
                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine("ORDER BY TECNICO,NATUREZA");

                using (var reader = CommandHelper.ExecuteReaderFirst(comRelatorio, sql, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var servico = new RelProdTecnicoExtratoPrazoListModel();

                        servico.Codct = Convert.ToString(reader["CODCT"]);
                        servico.Projeto = Convert.ToString(reader["PROJETO"]);
                        servico.NumDoc = Convert.ToInt32(reader["NUM_DOC"]);
                        servico.NumObra = Convert.ToString(reader["NUM_OBRA"]);

                        if (considerarAgendamentoPeriodo && reader["TURNO"] != DBNull.Value)
                        {
                            servico.DataHoraInicioAgendamento =
                                reader["DATA_HORA_INI_TURNO"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_INI_TURNO"]).ToString("dd/MM/yyyy HH:mm") :
                                    "-";
                            servico.DataHoraFimAgendamento =
                                reader["DATA_HORA_FIM_TURNO"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_FIM_TURNO"]).ToString("dd/MM/yyyy HH:mm") :
                                    "-";

                            servico.Turno =
                                reader["TURNO"] != DBNull.Value ?
                                    Convert.ToString(reader["TURNO"]) :
                                    "-";
                        }
                        else
                        {
                            servico.DataHoraInicioAgendamento =
                                reader["DATA_HORA_INI_AGEND_SEM_TURNO"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_INI_AGEND_SEM_TURNO"]).ToString("dd/MM/yyyy HH:mm") :
                                    "-";

                            servico.DataHoraFimAgendamento =
                                reader["DATA_HORA_FIM_AGEND_SEM_TURNO"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_FIM_AGEND_SEM_TURNO"]).ToString("dd/MM/yyyy HH:mm") :
                                    "-";

                            servico.Turno = "-";
                        }

                        servico.DataHoraBaixa =
                            reader["DATA_HORA_BAIXA"] != DBNull.Value ?
                                Convert.ToDateTime(reader["DATA_HORA_BAIXA"]).ToString("dd/MM/yyyy HH:mm") :
                                String.Empty;

                        servico.DataHoraSLA =
                            reader["DATA_HORA_SLA"] != DBNull.Value ?
                                Convert.ToDateTime(reader["DATA_HORA_SLA"]).ToString("dd/MM/yyyy HH:mm") :
                                String.Empty;

                        servico.TipoServico = Convert.ToString(reader["TIPO_SERVICO"]);
                        servico.Natureza = Convert.ToString(reader["NATUREZA"]);
                        servico.UsuarioBaixa = Convert.ToString(reader["USUARIO_BAIXA"]);

                        servico.Equipe = Convert.ToString(reader["MATRICULA"]);
                        servico.Nome = Convert.ToString(reader["TECNICO"]);
                        servico.Supervisor =
                            reader["NOME_SUPERVISOR"] != DBNull.Value ?
                                Convert.ToString(reader["NOME_SUPERVISOR"]) :
                                String.Empty;
                        servico.Monitor =
                            reader["MONITOR"] != DBNull.Value ?
                                Convert.ToString(reader["MONITOR"]) :
                                String.Empty;

                        servico.GrupoArea = Convert.ToString(reader["GRUPO_AREA"]);
                        servico.Area = Convert.ToString(reader["AREA"]);
                        servico.Localidade = Convert.ToString(reader["LOCALIDADE"]);

                        servico.Status =
                            HtmlEnumExtensions.GetEnumDescription(
                                (StatusExecucaoAniel)Convert.ToInt32(reader["STATUS_EXECUCAO"])
                            );

                        #region Situação do Prazo

                        DateTime? dataHoraBaixa =
                            reader["DATA_HORA_BAIXA"] != DBNull.Value ?
                                Convert.ToDateTime(reader["DATA_HORA_BAIXA"]) :
                                (DateTime?)null;

                        if (dataHoraBaixa.HasValue)
                        {

                            DateTime? dataHoraSla =
                                reader["DATA_HORA_SLA"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_SLA"]) :
                                    (DateTime?)null;

                            if (dataHoraSla.HasValue)
                            {
                                if (dataHoraBaixa <= dataHoraSla)
                                    servico.SituacaoPrazoSla = RsCadastros.DENTRO_PRAZO;
                                else
                                    servico.SituacaoPrazoSla = RsCadastros.FORA_PRAZO;
                            }
                            else
                            {
                                servico.SituacaoPrazoSla = RsCadastros.FORA_PRAZO;
                            }


                            DateTime? dataHoraAgendamento;

                            if (considerarAgendamentoPeriodo && reader["TURNO"] != DBNull.Value)
                            {
                                dataHoraAgendamento =
                                    reader["DATA_HORA_FIM_TURNO"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["DATA_HORA_FIM_TURNO"]) :
                                        (DateTime?)null;
                            }
                            else
                            {
                                dataHoraAgendamento =
                                    reader["DATA_HORA_FIM_AGEND_SEM_TURNO"] != DBNull.Value ?
                                        Convert.ToDateTime(reader["DATA_HORA_FIM_AGEND_SEM_TURNO"]) :
                                        (DateTime?)null;
                            }

                            if (dataHoraAgendamento.HasValue)
                            {
                                if (dataHoraBaixa <= dataHoraAgendamento)
                                    servico.SituacaoPrazoAgendamento = RsCadastros.DENTRO_PRAZO;
                                else
                                    servico.SituacaoPrazoAgendamento = RsCadastros.FORA_PRAZO;
                            }
                            else
                            {
                                servico.SituacaoPrazoAgendamento = RsCadastros.FORA_PRAZO;
                            }

                        }



                        #endregion

                        servico.CodigoEncerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]);
                        servico.DescricaoEncerramento =
                            reader["ENCERRAMENTO"] != DBNull.Value ?
                                Convert.ToString(reader["ENCERRAMENTO"]) :
                                String.Empty;

                        servicos.Add(servico);
                    }
                }
            }

            return new RelProdTecnicoExtratoPrazoResponse
            {
                Total = total,
                Data = servicos
            };
        }

        public RelProdTecnicoExtratoReincidenciaResponse GetExtratoReincidencia(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool apenasServicosProdutivosReinc)
        {
            int total = 0;
            var servicos = new List<RelProdTecnicoExtratoReincidenciaListModel>();

            #region SQL From

            var sqlFrom = new StringBuilder();
            var param = new Dictionary<string, object>();

            sqlFrom.AppendLine("FROM TB_REINCIDENCIA_DOCPROD R");

            sqlFrom.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO D_REINC_ORIGINAL");
            sqlFrom.AppendLine("ON(R.CODCT = D_REINC_ORIGINAL.CODCT AND");
            sqlFrom.AppendLine("    R.PROJETO = D_REINC_ORIGINAL.PROJETO AND");
            sqlFrom.AppendLine("    R.NUM_OBRA = D_REINC_ORIGINAL.NUM_OBRA AND");
            sqlFrom.AppendLine("    R.NUM_DOC = D_REINC_ORIGINAL.NUM_DOC)");

            sqlFrom.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO D_REINC");
            sqlFrom.AppendLine("ON(R.CODCT = D_REINC.CODCT AND");
            sqlFrom.AppendLine("    R.PROJETO = D_REINC.PROJETO AND");
            sqlFrom.AppendLine("    D_REINC_ORIGINAL.NUM_OBRA_ORIGINAL = D_REINC.NUM_OBRA_ORIGINAL AND");
            sqlFrom.AppendLine("    R.NUM_DOC = D_REINC.NUM_DOC AND");
            sqlFrom.AppendLine("    D_REINC.ULTIMO_SEQ = 'S')");

            sqlFrom.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO COD_ENC_REINC ");
            sqlFrom.AppendLine("  ON(COD_ENC_REINC.CODCT = D_REINC.CODCT AND COD_ENC_REINC.COD_ENCERRAMENTO = D_REINC.COD_ENCERRAMENTO) ");

            sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_REINC");
            sqlFrom.AppendLine("ON(D_REINC.COD_TIPO_SERV = T_SERV_REINC.COD_TIPO_SERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO N_REINC");
            sqlFrom.AppendLine("ON(T_SERV_REINC.COD_NATUREZA = N_REINC.ID)");

            sqlFrom.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO D_ANT");
            sqlFrom.AppendLine("ON(R.CODCT_ANTERIOR = D_ANT.CODCT AND");
            sqlFrom.AppendLine("    R.PROJETO_ANTERIOR = D_ANT.PROJETO AND");
            sqlFrom.AppendLine("    R.NUM_OBRA_ANTERIOR = D_ANT.NUM_OBRA AND");
            sqlFrom.AppendLine("    R.NUM_DOC_ANTERIOR = D_ANT.NUM_DOC)");

            sqlFrom.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO COD_ENC_ANT ");
            sqlFrom.AppendLine("  ON(COD_ENC_ANT.CODCT = D_ANT.CODCT AND COD_ENC_ANT.COD_ENCERRAMENTO = D_ANT.COD_ENCERRAMENTO) ");

            sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE T_SERV_ANT");
            sqlFrom.AppendLine("ON(D_ANT.COD_TIPO_SERV = T_SERV_ANT.COD_TIPO_SERV)");

            sqlFrom.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO N_ANT");
            sqlFrom.AppendLine("ON(T_SERV_ANT.COD_NATUREZA = N_ANT.ID)");

            sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE E");
            sqlFrom.AppendLine("ON(D_ANT.EQUIPE = E.EQUIPE)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO S");
            sqlFrom.AppendLine("ON(E.COD_SUPERV = S.NUM_FRE)");

            sqlFrom.AppendLine("LEFT JOIN TB_SENHA SEN");
            sqlFrom.AppendLine("ON(SEN.USUARIO = D_ANT.USUARIO_MONITOR)");

            sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO M");
            sqlFrom.AppendLine("ON(M.NUM_FRE = SEN.NUM_FRE)");

            sqlFrom.AppendLine("LEFT JOIN TB_ESCRITORIO ESC");
            sqlFrom.AppendLine("ON(ESC.COD_ESCRITORIO = D_REINC.COD_ESCRITORIO)");

            sqlFrom.AppendLine("LEFT JOIN TB_AREA U");
            sqlFrom.AppendLine("ON(D_REINC.COD_AREA = U.COD_AREA AND");
            sqlFrom.AppendLine("    D_REINC.COD_ESCRITORIO = U.COD_ESCRITORIO)");

            sqlFrom.AppendLine("LEFT JOIN TB_LOCALIDADE L");
            sqlFrom.AppendLine("ON(D_REINC.COD_LOCALIDADE = L.COD_LOCALIDADE AND");
            sqlFrom.AppendLine("    D_REINC.COD_AREA = L.COD_AREA AND");
            sqlFrom.AppendLine("    D_REINC.COD_ESCRITORIO = L.COD_ESCRITORIO)");

            sqlFrom.AppendLine("WHERE D_ANT.EQUIPE IS NOT NULL ");

            if (apenasServicosProdutivosReinc)
            {
                sqlFrom.AppendLine("  AND D_REINC.STATUS_EXECUCAO = '2'");
            }
            else
            {
                sqlFrom.AppendLine("  AND (D_REINC.STATUS_EXECUCAO = '2' OR D_REINC.STATUS_EXECUCAO = '1')");
            }

            var filtros = new List<string>();

            if (request.search != null && !String.IsNullOrEmpty(request.search.value))
            {
                string search = request.search.value.ToUpperInvariant();
                filtros.Add($@"(UPPER(E.NOME) LIKE '%{search}%' OR UPPER(D_ANT.EQUIPE) LIKE '%{search}%')");
            }

            if (!String.IsNullOrEmpty(request.DataInicio) && !String.IsNullOrEmpty(request.DataFim))
            {
                //buscando atraves da D_REINC_ORIGINAL porque é necessario filtrar
                //a data de criação da primeira OS e não do ultimo sequencial
                filtros.Add("CAST(D_REINC_ORIGINAL.DIA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM");

                DateTime dataInicio = DateTime.Parse(request.DataInicio);
                DateTime dataFim = DateTime.Parse(request.DataFim);

                param.Add("@DATA_INI", dataInicio);
                param.Add("@DATA_FIM", dataFim);
            }

            if (!String.IsNullOrEmpty(request.Equipe))
            {
                filtros.Add("D_ANT.EQUIPE = @EQUIPE");
                param.Add("@EQUIPE", request.Equipe);
            }

            if (request.Natureza.HasValue)
            {
                filtros.Add("N_ANT.ID = @COD_NATUREZA");
                param.Add("@COD_NATUREZA", request.Natureza.Value);
            }

            if (!String.IsNullOrEmpty(request.GrupoArea))
            {
                filtros.Add("D_ANT.COD_ESCRITORIO = @GRUPO_AREA");
                param.Add("@GRUPO_AREA", request.GrupoArea);

                if (!String.IsNullOrEmpty(request.Area))
                {
                    filtros.Add("D_ANT.COD_AREA = @AREA");
                    param.Add("@AREA", request.Area);
                }
            }

            if (!String.IsNullOrEmpty(request.Monitor))
            {
                filtros.Add("D_ANT.USUARIO_MONITOR = @MONITOR");
                param.Add("@MONITOR", request.Monitor);
            }

            if (!String.IsNullOrEmpty(request.Supervisor))
            {
                filtros.Add("E.COD_SUPERV = @SUPERVISOR");
                param.Add("@SUPERVISOR", request.Supervisor);
            }

            if (filtros.Count > 0)
            {
                foreach (string filtro in filtros)
                {
                    sqlFrom.AppendLine($"  AND ({filtro})");
                }
            }

            #endregion

            using (var comTotal = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT COUNT(*) AS QTD_SERVICOS");
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = CommandHelper.ExecuteReader(comTotal, sql, param, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_SERVICOS"]);
                }
            }

            using (var comRelatorio = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT R.CODCT, R.PROJETO, R.NUM_DOC, R.NUM_OBRA_ANTERIOR,");
                sql.AppendLine("       D_ANT.DIA AS DATA_OS_ANTERIOR, D_ANT.DATA_PROD AS DATA_BAIXA_OS_ANTERIOR, T_SERV_ANT.DESCRICAO AS TIPO_SERV_ANTERIOR,");
                sql.AppendLine("       N_ANT.DESCRICAO AS NATUREZA_OS_ANTERIOR, R.NUM_OBRA,");
                sql.AppendLine("       D_REINC_ORIGINAL.DIA AS DATA_OS_REINCIDENTE,");
                sql.AppendLine(" (SELECT FIRST 1 COD_ENC_REINC.DESCRICAO FROM TB_DOCUMENTO_PRODUCAO DOC_R ");
                sql.AppendLine("     LEFT JOIN TB_CODIGO_ENCERRAMENTO ENC_R ON (DOC_R.COD_ENCERRAMENTO = ENC_R.COD_ENCERRAMENTO AND DOC_R.CODCT = ENC_R.CODCT) ");
                sql.AppendLine("     WHERE DOC_R.CODCT = D_REINC.CODCT AND DOC_R.PROJETO = D_REINC.PROJETO AND DOC_R.NUM_OBRA_ORIGINAL = D_REINC.NUM_OBRA_ORIGINAL ");
                sql.AppendLine("     AND DOC_R.NUM_DOC = D_REINC.NUM_DOC ");
                sql.AppendLine("     ORDER BY DOC_R.SEQ DESC) AS DESC_OS_REINCIDENTE, ");
                sql.AppendLine(" (SELECT FIRST 1 ENC_ANT.DESCRICAO FROM TB_DOCUMENTO_PRODUCAO DOC_ANT ");
                sql.AppendLine("     LEFT JOIN TB_CODIGO_ENCERRAMENTO ENC_ANT ON (DOC_ANT.COD_ENCERRAMENTO = ENC_ANT.COD_ENCERRAMENTO AND DOC_ANT.CODCT = ENC_ANT.CODCT) ");
                sql.AppendLine("     WHERE DOC_ANT.CODCT = D_ANT.CODCT AND DOC_ANT.PROJETO = D_ANT.PROJETO AND DOC_ANT.NUM_OBRA_ORIGINAL = D_ANT.NUM_OBRA_ORIGINAL ");
                sql.AppendLine("     AND DOC_ANT.NUM_DOC = D_ANT.NUM_DOC ");
                sql.AppendLine("     ORDER BY DOC_ANT.SEQ DESC) AS DESC_OS_ANTERIOR, ");
                sql.AppendLine("       T_SERV_REINC.DESCRICAO AS TIPO_SERVICO_OS_REINCIDENTE,");
                sql.AppendLine("       N_REINC.DESCRICAO AS NATUREZA_OS_REINCIDENTE,");
                sql.AppendLine("       D_ANT.EQUIPE, E.NOME AS NOME_TECNICO, S.NOME AS NOME_SUPERVISOR,");
                sql.AppendLine("       M.NOME AS NOME_MONITOR, ESC.DESCRICAO AS GRUPO_AREA, U.DESCRICAO AS AREA,");
                sql.AppendLine("       L.LOCALIDADE");
                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine("ORDER BY E.NOME, N_ANT.DESCRICAO");

                using (var reader = CommandHelper.ExecuteReaderFirst(comRelatorio, sql, param, this._sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.Read())
                    {
                        var servico = new RelProdTecnicoExtratoReincidenciaListModel();


                        servico.Codct = Convert.ToString(reader["CODCT"]);
                        servico.Projeto = Convert.ToString(reader["PROJETO"]);
                        servico.NumDoc = Convert.ToInt32(reader["NUM_DOC"]);

                        servico.NumObraOsReincidente = Convert.ToString(reader["NUM_OBRA"]).Contains("/") ? Convert.ToString(reader["NUM_OBRA"]).Split('/')[0] : Convert.ToString(reader["NUM_OBRA"]);
                        servico.DataCriacaoOsReincidente = Convert.ToDateTime(reader["DATA_OS_REINCIDENTE"]).ToString("dd/MM/yyyy");
                        servico.TipoServicoOsReincidente = Convert.ToString(reader["TIPO_SERVICO_OS_REINCIDENTE"]);
                        servico.NaturezaOsReincidente = Convert.ToString(reader["NATUREZA_OS_REINCIDENTE"]);
                        servico.DescricaoOsReincidente = Convert.ToString(reader["DESC_OS_REINCIDENTE"]);

                        servico.NumObraOsAnterior = Convert.ToString(reader["NUM_OBRA_ANTERIOR"]).Contains("/") ? Convert.ToString(reader["NUM_OBRA_ANTERIOR"]).Split('/')[0] : Convert.ToString(reader["NUM_OBRA_ANTERIOR"]);
                        servico.DataCriacaoOsAnterior = Convert.ToDateTime(reader["DATA_OS_ANTERIOR"]).ToString("dd/MM/yyyy");

                        DateTime dataBaixaOsAnterior = Convert.ToDateTime(reader["DATA_BAIXA_OS_ANTERIOR"]);

                        servico.DataBaixaOsAnterior = dataBaixaOsAnterior.ToString("dd/MM/yyyy");

                        DateTime dataCriacaoOsReincidente = Convert.ToDateTime(reader["DATA_OS_REINCIDENTE"]);
                        int qtdDiasBaixaServicoAnterior = Math.Abs(Convert.ToInt32(dataCriacaoOsReincidente.Subtract(dataBaixaOsAnterior).TotalDays));

                        servico.QtdDiasBaixaServicoAnterior = Convert.ToString(qtdDiasBaixaServicoAnterior);

                        servico.TipoServicoOsAnterior = Convert.ToString(reader["TIPO_SERV_ANTERIOR"]);
                        servico.NaturezaOsAnterior = Convert.ToString(reader["NATUREZA_OS_ANTERIOR"]);
                        servico.DescricaoOsAnterior = Convert.ToString(reader["DESC_OS_ANTERIOR"]);

                        servico.Equipe = Convert.ToString(reader["EQUIPE"]);
                        servico.Nome = Convert.ToString(reader["NOME_TECNICO"]);
                        servico.Supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]);
                        servico.Monitor = Convert.ToString(reader["NOME_MONITOR"]);

                        servico.GrupoArea = Convert.ToString(reader["GRUPO_AREA"]);
                        servico.Area = Convert.ToString(reader["AREA"]);
                        servico.Localidade = Convert.ToString(reader["LOCALIDADE"]);

                        servicos.Add(servico);
                    }
                }
            }

            return new RelProdTecnicoExtratoReincidenciaResponse
            {
                Total = total,
                Data = servicos
            };
        }



        public DocumentoProducao_GetConfigFluxoBaixa GetTipoServDepOS(String pNucleo, String pTipo, DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa)
        {
            using (var session = HibernateUtil.GetSessionT5())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COALESCE(COD_TIPO_SERV,0)AS COD_TIPO_SERV,COALESCE(COD_DEPTO,0)AS COD_DEPTO ");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO ");
                sql.AppendLine(" WHERE CODCT = @pCODCT ");
                sql.AppendLine(" AND PROJETO = @pPROJETO ");
                sql.AppendLine(" AND NUM_DOC = @pNUM_DOC ");
                sql.AppendLine(" AND NUM_OBRA_ORIGINAL = @pNUM_OBRA_ORIGINAL ");

                if (pTipo == "FIRST")
                {
                    sql.AppendLine(" ORDER BY SEQ ");
                }
                else if (pTipo == "LAST")
                {
                    sql.AppendLine(" ORDER BY SEQ DESC ");
                }

                parametros.Add("@pCODCT", pGetConfigFluxoBaixa.codCt);
                parametros.Add("@pPROJETO", pGetConfigFluxoBaixa.projeto);
                parametros.Add("@pNUM_DOC", pGetConfigFluxoBaixa.numDoc);
                parametros.Add("@pNUM_OBRA_ORIGINAL", pGetConfigFluxoBaixa.numObraOriginal);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        if (pNucleo == "TIPOSERV")
                        {
                            pGetConfigFluxoBaixa.codTipoServ = Convert.ToInt32(reader["COD_TIPO_SERV"]);
                        }
                        else if (pNucleo == "SETOR")
                        {
                            pGetConfigFluxoBaixa.codDepto = Convert.ToInt32(reader["COD_DEPTO"]);
                        }

                        return pGetConfigFluxoBaixa;
                    }

                }
            }
            return pGetConfigFluxoBaixa;
        }

        public DocumentoProducao_GetConfigFluxoBaixa GetConfigFluxoBaixa(DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa)
        {
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                DocumentoProducao_GetConfigFluxoBaixa _getConfigFluxoBaixa = new DocumentoProducao_GetConfigFluxoBaixa();

                _getConfigFluxoBaixa = pGetConfigFluxoBaixa;

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 COALESCE(A.FLUXO_AO_ENCERRAR,0)AS FLUXO_AO_ENCERRAR,COALESCE(A.COMPORTAMENTO_TIPO_SERVICO,0)AS COMPORTAMENTO_TIPO_SERVICO, ");
                sql.AppendLine("        A.COD_TIPO_SERV, COALESCE(COMPORTAMENTO_SETOR,0)AS COMPORTAMENTO_SETOR,COALESCE(A.COD_DEPTO,0)AS COD_DEPTO,COALESCE(B.UTILIZA_NATUREZA,'N')AS UTILIZA_NATUREZA  ");
                sql.AppendLine(" FROM TB_VINC_ENCERRAMENTO_NATUREZA A ");
                sql.AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO B ON (A.COD_ENCERRAMENTO = B.COD_ENCERRAMENTO AND A.CODCT = B.CODCT) ");
                sql.AppendLine(" WHERE A.COD_NATUREZA = @pCOD_NATUREZA AND A.COD_ENCERRAMENTO = @pCOD_ENCERRAMENTO AND A.CODCT = @CODCT ");
                parametros.Add("@CODCT", pGetConfigFluxoBaixa.codCt);
                parametros.Add("@pCOD_ENCERRAMENTO", pGetConfigFluxoBaixa.codEncerramento);
                parametros.Add("@pCOD_NATUREZA", pGetConfigFluxoBaixa.codNatureza);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        _getConfigFluxoBaixa = pGetConfigFluxoBaixa;
                        _getConfigFluxoBaixa.fluxoAoEncerrar = Convert.ToString(reader["FLUXO_AO_ENCERRAR"]);
                        _getConfigFluxoBaixa.comportamentoTipoServ = reader["COMPORTAMENTO_TIPO_SERVICO"] != DBNull.Value ? Convert.ToInt32(reader["COMPORTAMENTO_TIPO_SERVICO"]) : (int?)null;
                        _getConfigFluxoBaixa.codTipoServ = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt32(reader["COD_TIPO_SERV"]) : (int?)null;
                        _getConfigFluxoBaixa.comportamentoSetor = reader["COMPORTAMENTO_SETOR"] != DBNull.Value ? Convert.ToInt32(reader["COMPORTAMENTO_SETOR"]) : (int?)null;
                        _getConfigFluxoBaixa.codDepto = reader["COD_DEPTO"] != DBNull.Value ? Convert.ToInt32(reader["COD_DEPTO"]) : (int?)null;
                        _getConfigFluxoBaixa.utilizaNatureza = Convert.ToString(reader["UTILIZA_NATUREZA"]);
                        return _getConfigFluxoBaixa;
                    }

                }
            }
            return pGetConfigFluxoBaixa;
        }

        public void AplicaProducaoPrevista(Documento_ProducaoAniel documento, string usuario)
        {

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder(@"UPDATE TB_CUSTOS_TAREFA A SET A.QUANT_PROD = A.QUANT_CATAL,A.DATA_PROD  = @DATA_AGENDAMENTO, A.USUARIO_ALTERACAO = @USUARIO, A.DIA_ALTERACAO = CURRENT_DATE, A.HORA_ALTERACAO = CURRENT_TIME
                           WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND
                           A.NUM_DOC = @NUM_DOC AND COALESCE(A.QUANT_PROD,0) = 0 AND A.QUANT_CATAL > 0");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                parametros.Add("@CODCT", documento.CodCt);
                parametros.Add("@PROJETO", documento.Projeto);
                parametros.Add("@NUM_OBRA", documento.Num_Obra);
                parametros.Add("@NUM_DOC", documento.Num_Doc);
                parametros.Add("@USUARIO", usuario);
                parametros.Add("@DATA_AGENDAMENTO", documento.Data_Maxima);

                //if (transaction != null)
                //{
                //    transaction.Enlist(com);
                //}

                CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction).Wait();

            }
        }

        public void AtualizarDataProducao(Documento_ProducaoAniel documento)
        {

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder(@"UPDATE TB_CUSTOS_TAREFA A SET A.DATA_PROD = @DATA_AGENDAMENTO
                                                        WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                parametros.Add("@CODCT", documento.CodCt);
                parametros.Add("@PROJETO", documento.Projeto);
                parametros.Add("@NUM_OBRA", documento.Num_Obra);
                parametros.Add("@NUM_DOC", documento.Num_Doc);
                parametros.Add("@DATA_AGENDAMENTO", documento.Data_Maxima);

                //if (transaction != null)
                //{
                //    transaction.Enlist(com);
                //}

                CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction).Wait();

            }
        }

        public void InsertLog_Lanc_Servico(long? CodMotivoReagendamento, String ObsMotivoReagendamento, String CodCt,
            String Projeto,
            String Num_Os,
            Int32 Num_Doc,
            String Alteracao,
            String Valor_Anterior,
            String Usuario,
            DateTime? Data_Maxima,
            Int16? Cod_Tipo_Serv,
            String Valor_Atualizado,
            String Obs_Cobranca_Status,
            ITransaction transaction = null)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder("INSERT INTO TB_LOG_LANC_SERVICO (CODCT, PROJETO, NUM_OBRA, NUM_DOC, ALTERACAO, VALOR_ANT, VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, ")
                            .AppendLine(" USUARIO, DIA, HORA, DATA_MAXIMA, COD_TIPO_SERV) ")
                            .AppendLine(" VALUES(@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @ALTERACAO, @VALOR_ANT,@VALOR_ATUALIZADO,@OBS_STATUS_COBRANCA, ")
                            .AppendLine(" @USUARIO, CURRENT_DATE, CURRENT_TIME, @DATA_MAXIMA, @COD_TIPO_SERV) ");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Os);
                parametros.Add("@NUM_DOC", Num_Doc);

                parametros.Add("@ALTERACAO", Alteracao);
                parametros.Add("@VALOR_ANT", Valor_Anterior);
                parametros.Add("@VALOR_ATUALIZADO", Valor_Atualizado);
                parametros.Add("@OBS_STATUS_COBRANCA", Obs_Cobranca_Status);
                parametros.Add("@USUARIO", Usuario);
                parametros.Add("@DATA_MAXIMA", Data_Maxima);
                parametros.Add("@COD_TIPO_SERV", Cod_Tipo_Serv);

                if (transaction != null)
                {
                    transaction.Enlist(com);
                }

                CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction != null ? com.Transaction : null).Wait();

            }

        }

        public Int64 GerarSeq(String Codct, String Projeto, String OS, Int32 Num_Doc)
        {
            using (var repository = new Conexao<Documento_ProducaoAniel>(this._sessionT5Nh))
            {
                var seq = repository.Max(p => p.Seq, w => w.CodCt == Codct && w.Projeto == Projeto && w.Numero_Obra_Original == OS && w.Num_Doc == Num_Doc).Result;
                return seq + 1;
            }
        }

        public String GetNum_Obra(Configuracoes_Gerais config)
        {
            using (var repository = new Conexao<Documento_ProducaoAniel>(this._sessionT5Nh))
            {
                try
                {
                    return (config.Formato_Protocolo_OS == Formato_Protocolo_OS.Ano_Mes_Dia_Hora_Sequencial) ?
                        String.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), (repository.GetSequence("GEN_NUM_OBRA").Result)) :
                        String.Format("{0:D8}", (repository.GetSequence("GEN_NUM_OBRA").Result));
                }
                catch
                {
                    throw new Exception("Empresa já possui OSs com o formato (Ano+Mes+Dia+Hora+Sequencial). Altere as configurações gerais para continuar.");
                }
            }

        }

        public Cobranca_Servico BuscarCobrancaDocumento(Documento_ProducaoAniel documento)
        {
            Cobranca_Servico cobServico = null;
            List<Cobranca_Servico> lsCobServ = new List<Cobranca_Servico>();

            using (var session = HibernateUtil.GetSession())
            using (var repoTipoCob = new Conexao<TipoServico_Cobranca>(session))
            using (var repoPlanProd = new Conexao<Plano_Produto_Cobranca>(session))
            using (var repoCobServ = new Conexao<Cobranca_Servico>(session))
            {
                var planProdCobs = repoPlanProd.select(p => p.Produto.Id == documento.Id_PlanoProduto || 
                                                   p.Plano.Id == documento.Id_PlanoProduto).Result;

                if (planProdCobs == null)
                    return cobServico;

                foreach(var planCob in planProdCobs) {
                    var cob = repoCobServ.selectFirst(c => c.Id == planCob.Cobranca_Servico.Id).Result;

                    if (cob == null) continue;

                    if (cob.Status == Status_Default.Inativo) continue;

                    var tipoCob = repoTipoCob.selectFirst(t => t.Cobranca_Servico.Id == cob.Id).Result;

                    if (tipoCob == null) continue;

                    if (documento?.CodCt != tipoCob?.Segmento) continue;

                    if (documento?.Cod_Tipo_Serv?.ToString() != tipoCob?.Tipo_Servico) continue;

                    if (documento?.Cod_Sub_Tipo_Serv != null && !tipoCob.TodasSubTipoServico) {
                        if (documento?.Cod_Sub_Tipo_Serv != tipoCob.Sub_Tipo_Servico)
                            continue;
                    }

                    if (documento?.Cod_Encerramento != null && !tipoCob.TodasCod_Encerramento) {
                        if (documento?.Cod_Encerramento != tipoCob.Cod_Encerramento)
                            continue;
                    }

                    lsCobServ.Add(tipoCob.Cobranca_Servico);
                }

                if (lsCobServ == null) return cobServico;           
            }

            return lsCobServ.FirstOrDefault();
        }

        public List<string> ListarObservacoesBaixa(string Codct, string Projeto, int Num_Doc, string NumObraOriginal)
        {
            List<string> retorno = new List<string>();
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.OBSERVACAO_TESTE ");
                sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A");
                sql.AppendLine(" WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUMDOC AND NUM_OBRA_ORIGINAL = @NUM_OBRA_ORIGINAL ");
                sql.AppendLine(" ORDER BY A.SEQ ");

                parametros.Add("@CODCT", Codct);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUMDOC", Num_Doc);
                parametros.Add("@NUM_OBRA_ORIGINAL", NumObraOriginal);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var observacao = Convert.ToString(reader["OBSERVACAO_TESTE"]);
                        retorno.Add(observacao);
                    }
                }
            }

            return retorno;
        }

        public int GetNumDocManutencao()
        {
            int numDoc = 0;
            try
            {
                using (var com = this._sessionT5Sql.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.AppendLine("SELECT GEN_ID(GN_NUM_DOC_MANU, 1) AS NUM_DOC_MANU FROM RDB$DATABASE");

                    using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                    {
                        if (reader.Read())
                        {
                            numDoc = Convert.ToInt32(reader["NUM_DOC_MANU"]);
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Erro buscar número da manutenção");
            }

            return numDoc;
        }

        public TimeSpan? GetTempoExecucao(Configuracoes_Gerais config, int? codTipoImovel, short? codTipoServ)
        {
            TimeSpan? retorno = config.Tempo_Execucao_Padrao.HasValue ? (TimeSpan?)config.Tempo_Execucao_Padrao.Value.TimeOfDay : null;
            using (var com = _sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("SELECT FIRST 1 M.HORAS AS TEMPO_EXECUCAO ")
                .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ");

                List<string> condicoes = new List<string>();

                if (codTipoImovel.HasValue)
                {
                    condicoes.Add("(M.ID_TIPO_IMOVEL = (@COD_TIPO_IMOVEL) OR M.ID_TIPO_IMOVEL IS NULL)");
                }

                if (codTipoServ.HasValue)
                {
                    condicoes.Add("(M.ID_TIPO_SERVICO = @COD_TIPO_SERV)");
                }

                if (condicoes.Count > 0)
                {
                    var whereSql = String.Join(" AND ", condicoes);
                    sql.AppendLine($"WHERE {whereSql} ");
                }

                sql.AppendLine("ORDER BY M.ID_TIPO_IMOVEL DESC ");
                parametros.Add("@COD_TIPO_IMOVEL", codTipoImovel);
                parametros.Add("@COD_TIPO_SERV", codTipoServ);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        return reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan?)(reader["TEMPO_EXECUCAO"]) : retorno;
                    }
                }
            }

            return retorno;
        }

        public TimeSpan? GetTempoExecucao(Configuracoes_Gerais config, DateTime inicio, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc)
        {
            TimeSpan? retorno = config.Tempo_Execucao_Padrao.HasValue ? (TimeSpan?)config.Tempo_Execucao_Padrao.Value.TimeOfDay : null;
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("SELECT (SELECT FIRST 1 M.HORAS ")
                .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")
                .AppendLine("   FROM TB_DOCUMENTO_PRODUCAO A ");

                sql.AppendLine($"WHERE (A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUMOBRA AND A.NUM_DOC = @NUMDOC)  ");
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUMOBRA", Num_Os);
                parametros.Add("@NUMDOC", Num_Doc);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        return reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan?)(reader["TEMPO_EXECUCAO"]) : retorno;
                    }
                }
            }

            return retorno;

        }

        public String GetManutencao_OT_OR(string CodCt, string Projeto, string Num_Obra, Int32 Num_Doc)
        {
            String retorno = null;
            using (var com = _sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine(@"SELECT FIRST 1 E.NUM_OT_OR
                                 FROM TB_DOCUMENTO_PRODUCAO A
                                 LEFT JOIN TB_MANUTENCAO_OT_OR E
                                 ON (E.CODCT=A.CODCT AND E.PROJETO=A.PROJETO AND E.UNIDADE_CUSTO=A.UNIDADE_CUSTO AND CAST(E.NUM_EMPRESA AS INTEGER)=A.NUM_DOC)
                                 WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA
                                 AND A.NUM_DOC = @NUM_DOC");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Obra);
                parametros.Add("@NUM_DOC", Num_Doc);

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        return reader["NUM_OT_OR"] != DBNull.Value ? Convert.ToString(reader["NUM_OT_OR"]) : retorno;
                    }
                }
            }

            return retorno;
        }

        public Int32 GetManutencao_OT_OR(string CodCt, string Projeto, string Num_Ot_Or)
        {
            int retorno = 0;
            using (var com = _sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine(@"SELECT FIRST 1 E.NUM_OT_OR, CAST(E.NUM_EMPRESA AS INTEGER) AS NUM_EMPRESA
                                 FROM TB_MANUTENCAO_OT_OR E                                 
                                 WHERE E.CODCT=@CODCT AND E.PROJETO=@PROJETO AND E.NUM_OT_OR = @NUM_OT_OR");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OT_OR", Num_Ot_Or);                

                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, _sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                    {
                        return reader["NUM_OT_OR"] != DBNull.Value ? Convert.ToInt32(reader["NUM_EMPRESA"]) : retorno;
                    }
                }
            }

            return retorno;
        }

        public GestaoOperacionalServicosResponse GetServicosPainelGestaoOperacional(GestaoCapacidadeRoteirizacaoRequest request,
                                                                                    TimeSpan? tempoExecucaoPadrao,
                                                                                    TimeSpan? tempoDeslocamentoPadrao)
        {
            int total = 0;
            List<DocumentoPlanejamento> servicos = new List<DocumentoPlanejamento>();
            var sqlFrom = new StringBuilder();
            List<String> condicoes = new List<string>();
            sqlFrom.AppendLine(@"FROM TB_DOCUMENTO_PRODUCAO A
                                LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)
                                LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)
                                LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE P
                                ON(A.COD_TIPO_SERV = P.COD_TIPO_SERV)
                                LEFT JOIN TB_NATUREZA_PRODUCAO L
                                ON(L.ID = P.COD_NATUREZA) 
                                LEFT JOIN TB_EQUIPE M 
                                ON(A.EQUIPE = M.EQUIPE) 
                                LEFT JOIN TB_FUNCIONARIO K 
                                ON(K.NUM_FRE = M.COD_SUPERV)
                                LEFT JOIN TB_SENHA Q
                                ON(Q.USUARIO = M.USUARIO_MONITOR)
                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE N
                                ON(A.COD_TIPO_SERV = N.COD_TIPO_SERV)
                                LEFT JOIN TB_NATUREZA_PRODUCAO O
                                ON(O.ID = N.COD_NATUREZA) 
                                LEFT JOIN TB_LOCALIDADE R
                                ON(A.COD_ESCRITORIO = R.COD_ESCRITORIO AND A.COD_AREA = R.COD_AREA AND A.COD_LOCALIDADE = R.COD_LOCALIDADE) 
                                LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL S
                                ON(A.NUM_OBRA = S.NUM_OBRA AND A.PROJETO = S.PROJETO AND A.CODCT = S.CODCT AND A.NUM_DOC = S.NUM_DOC)
                                LEFT JOIN TB_TURNO_SERVICO T
                                ON(A.COD_TURNO_SERV = T.ID)
                                LEFT JOIN TB_CODIGO_ENCERRAMENTO ENC
                                ON(ENC.CODCT = A.CODCT AND ENC.COD_ENCERRAMENTO = A.COD_ENCERRAMENTO) ");


            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            if (String.IsNullOrEmpty(request.Contrato) == false)
            {
                condicoes.Add("A.CODCT = @CODCT");
                parametros.Add("@CODCT", request.Contrato);
            }
            if (String.IsNullOrEmpty(request.Projeto) == false)
            {
                condicoes.Add("A.PROJETO = @PROJETO");
                parametros.Add("@PROJETO", request.Projeto);
            }
            if (String.IsNullOrEmpty(request.Equipe) == false)
            {
                condicoes.Add("A.EQUIPE = @EQUIPE");
                parametros.Add("@EQUIPE", request.Equipe);
            }
            if (String.IsNullOrEmpty(request.GrupoArea) == false)
            {
                condicoes.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
            }
            if (String.IsNullOrEmpty(request.Area) == false)
            {
                condicoes.Add("A.COD_AREA = @AREA");
                parametros.Add("@AREA", request.Area);
            }
            if (String.IsNullOrEmpty(request.Natureza) == false)
            {
                condicoes.Add("O.ID IN (@NATUREZA)");
                parametros.Add("@NATUREZA", request.Natureza);
            }
            if (String.IsNullOrEmpty(request.TipoServico) == false)
            {
                condicoes.Add("A.COD_TIPO_SERV = @COD_TIPO_SERV");
                parametros.Add("@COD_TIPO_SERV", request.TipoServico);
            }
            if (String.IsNullOrEmpty(request.Localidade) == false)
            {
                condicoes.Add("R.LOCALIDADE = @LOCALIDADE");
                parametros.Add("@LOCALIDADE", request.Localidade);
            }
            if (String.IsNullOrEmpty(request.Periodo) == false)
            {
                condicoes.Add("A.COD_TURNO_SERV = @PERIODO");
                parametros.Add("@PERIODO", request.Periodo);
            }

            if (String.IsNullOrEmpty(request.Supervisor) == false)
            {
                condicoes.Add("M.COD_SUPERV = @SUPERVISOR");
                parametros.Add("@SUPERVISOR", request.Supervisor);
            }

            condicoes.Add("((A.DATA_MAXIMA BETWEEN @DATA_INICIO_DEMANDA AND @DATA))");
            parametros.Add("@DATA", request.Data);
            parametros.Add("@DATA_INICIO_DEMANDA", request.DataInicioDemanda);

            if (condicoes.Count > 0)
            {
                sqlFrom.AppendLine(" WHERE ");
                sqlFrom.AppendLine(String.Join(" AND ", condicoes));
            }


            using (var comTotal = this._sessionT5Sql.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine("SELECT COUNT(*) AS QTD_SERVICOS");
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = CommandHelper.ExecuteReader(comTotal, sql, parametros, this._sessionT5Sql.Connection, null).Result)
                {
                    if (reader.Read())
                        total = Convert.ToInt32(reader["QTD_SERVICOS"]);
                }
            }

            using (var com = _sessionT5Sql.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder(@"SELECT A.NUM_OBRA,A.PROJETO,A.CODCT, A.NUM_OBRA_ORIGINAL,A.NUM_DOC,B.DESCRICAO AS ESCRITORIO,A.EQUIPE,A.OXBDPEE,
                                                                A.DATA_MAXIMA,A.HORA_MAXIMA,C.DESCRICAO AS AREA,D.LOCALIDADE, A.COD_ESCRITORIO,A.COD_AREA,A.COD_LOCALIDADE,
                                                                L.TIPO_HABILIDADE,K.NOME AS SUPERVISOR,M.USUARIO_MONITOR,M.COD_SUPERV,Q.NOME AS MONITOR,S.DESLOCAMENTO,A.STATUS_EXECUCAO,
                                                                A.DATA_INDICADOR,A.HORA_INDICADOR,A.DATA_PROD,A.HORA_PROD,A.DATA_PROD + A.HORA_PROD  AS DATA_HORA_ENCERRAMENTO,T.DESCRICAO AS PERIODO,A.COD_ENCERRAMENTO,ENC.DESCRICAO AS DESCRICAO_ENCERRAMENTO,
                                                                A.DIA + A.HORA AS DATAHORACRIACAO , P.DESCRICAO AS DESCRICAO_TIPOSERV, O.DESCRICAO AS DESCRICAO_NATUREZA,M.NOME AS NOME_TECNICO,A.USUARIO_BAIXA,A.STATUS_AUDITORIA,
                                                                COALESCE((A.DATA_ABERTURA + COALESCE(A.HORA_ABERTURA, CAST('00:00' AS TIME))),(CASE WHEN
                                                                               COALESCE(A.SEQ,1) > 1 THEN
                                                                                   (SELECT FIRST 1 M.DIA + COALESCE(M.HORA, CAST('00:00' AS TIME))
                                                                                   FROM TB_DOCUMENTO_PRODUCAO M
                                                                                   WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO
                                                                                   AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL
                                                                                   AND A.NUM_DOC = M.NUM_DOC ORDER BY M.SEQ)
                                                                          WHEN
                                                                               COALESCE(A.SEQ,1) <= 1 THEN
                                                                                   A.DIA + COALESCE(A.HORA, CAST('00:00' AS TIME))
                                                                
                                                                END)) AS DATAHORA_ABERTURA_OS_ORIGINAL,                                                                 
                                                                (SELECT FIRST 1 M.HORAS
                                                                FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M
                                                                WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO");


                sql.AppendLine($" {sqlFrom.ToString()} ");

                if (request.length == 0)
                {
                    request.length = int.MaxValue;
                }

                //Busca com paginacao
                using (var reader = CommandHelper.ExecuteReaderFirst(com, sql, parametros, _sessionT5Sql.Connection, null, request.length, request.start).Result)
                {
                    while (reader.ReadAsync().Result)
                    {
                        var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToInt16(reader["DESLOCAMENTO"]) : tempoDeslocamentoPadrao.Value.TotalMinutes;
                        var tempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? ((TimeSpan)(reader["TEMPO_EXECUCAO"])) : tempoExecucaoPadrao.Value;
                        tempoExecucao = tempoExecucao.Add(TimeSpan.FromMinutes(deslocamento));

                        var datahora_indicador = reader["DATA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INDICADOR"]) : (DateTime?)null;
                        if (datahora_indicador.HasValue && reader["HORA_INDICADOR"] != DBNull.Value)
                        {
                            datahora_indicador = datahora_indicador.Value.Add((TimeSpan)reader["HORA_INDICADOR"]);
                        }

                        var data_hora_encerramento = reader["DATA_HORA_ENCERRAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_ENCERRAMENTO"]) : (DateTime?)null;
                        var dataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;

                        servicos.Add(new DocumentoPlanejamento
                        {
                            Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                            CodCt = Convert.ToString(reader["CODCT"]),
                            Projeto = Convert.ToString(reader["PROJETO"]),
                            Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null,
                            Data_Maxima = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null,
                            Hora_Maxima = reader["HORA_MAXIMA"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)reader["HORA_MAXIMA"]) : (DateTime?)null,
                            DataMaximaStr = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy") : null,
                            HoraMaximaStr = reader["HORA_MAXIMA"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)reader["HORA_MAXIMA"]).ToString("HH:mm") : null,
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                            Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                            Cod_Area = Convert.ToString(reader["COD_AREA"]),
                            Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                            LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]),
                            Descricao_Grupo_Area = Convert.ToString(reader["ESCRITORIO"]),
                            Descricao_Area = Convert.ToString(reader["AREA"]),
                            TempoExecucaoMinutos = tempoExecucao.TotalMinutes,
                            Tempo_Execucao = tempoExecucao,
                            TipoHabilidade = reader["TIPO_HABILIDADE"] != DBNull.Value ? (TipoHabilidade?)Enum.Parse(typeof(TipoHabilidade), Convert.ToString(reader["TIPO_HABILIDADE"])) : (TipoHabilidade?)null,
                            NumFre_Supervisor = Convert.ToString(reader["COD_SUPERV"]),
                            Usuario_Monitor = Convert.ToString(reader["USUARIO_MONITOR"]),
                            NomeSupervisor = Convert.ToString(reader["SUPERVISOR"]),
                            NomeMonitor = Convert.ToString(reader["MONITOR"]),
                            DeslocamentoMinutos = (short)deslocamento,
                            Status = (StatusExecucaoAniel)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])),
                            DentroPrazo = (data_hora_encerramento ?? DateTime.Now) < datahora_indicador,
                            Periodo = Convert.ToString(reader["PERIODO"]),
                            CodEncerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                            DescricaoEncerramento = Convert.ToString(reader["DESCRICAO_ENCERRAMENTO"]),
                            Data_hora_Encerramento = data_hora_encerramento,
                            Data_hora_Criacao = dataHoraCriacao,
                            Descricao_Tipo_Serv = Convert.ToString(reader["DESCRICAO_TIPOSERV"]),
                            Descricao_Natureza = Convert.ToString(reader["DESCRICAO_NATUREZA"]),
                            StatusAuditoria = reader["STATUS_AUDITORIA"] == DBNull.Value ? (Status_AuditoriaAniel?)null : (Status_AuditoriaAniel)Enum.Parse(typeof(Status_AuditoriaAniel), Convert.ToString(reader["STATUS_AUDITORIA"])),
                            NomeTecnico = Convert.ToString(reader["NOME_TECNICO"]),
                            UsuarioBaixa = Convert.ToString(reader["USUARIO_BAIXA"]),
                            DataHoraAberturaOriginal = reader["DATAHORA_ABERTURA_OS_ORIGINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ABERTURA_OS_ORIGINAL"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            OXBDPEE = Convert.ToString(reader["OXBDPEE"])

                        });
                    }
                }

                return new GestaoOperacionalServicosResponse
                {
                    Total = total,
                    Data = servicos
                };
            }


        }
        public string GetJoinCountPainelServico(string WhereCondition)
        {
            StringBuilder sqlJoinCount = new StringBuilder(@"FROM TB_DOCUMENTO_PRODUCAO A ");

            if (WhereCondition.Contains("K."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)");
                sqlJoinCount.AppendLine("LEFT JOIN TB_FUNCIONARIO K ON(K.NUM_FRE = B.COD_SUPERV)");
            }
            else if (WhereCondition.Contains("B."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)");
            }


            if (WhereCondition.Contains("C."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_ESCRITORIO C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO)");
            }
            if (WhereCondition.Contains("D."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_AREA D ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA)");
            }
            if (WhereCondition.Contains("E."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_LOCALIDADE E ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND A.COD_LOCALIDADE = E.COD_LOCALIDADE)");
            }

            if (WhereCondition.Contains("L."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE F ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV)");
                sqlJoinCount.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO L ON(L.ID = F.COD_NATUREZA)");
            }
            else if (WhereCondition.Contains("F."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE F ON(A.COD_TIPO_SERV = F.COD_TIPO_SERV)");
            }


            if (WhereCondition.Contains("G."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_SUB_TIPO_SERVICO G ON(A.COD_TIPO_SERV = G.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = G.COD_SUB_TIPO_SERV)");
            }

            if (WhereCondition.Contains("M."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_CLIENTE H ON(A.ID_CLIENTE = H.COD_CLIENTE)");
                sqlJoinCount.AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE_CONNECT M ON(M.ID = H.ID_CATEGORIA)");
            }
            else if (WhereCondition.Contains("H."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_CLIENTE H ON(A.ID_CLIENTE = H.COD_CLIENTE)");
            }

            if (WhereCondition.Contains("I."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO I ON(I.CODCT = A.CODCT AND A.COD_ENCERRAMENTO = I.COD_ENCERRAMENTO)");
            }
            if (WhereCondition.Contains("J."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_TIPO_IMOVEL J ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL)");
            }
            if (WhereCondition.Contains("S."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_SEGMENTO_SERVICO S  ON(S.CODIGO = A.SEGMENTO)");
            }

            if (WhereCondition.Contains("N."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_PLANO_PRODUTO N ON(N.ID = A.ID_PLANOPRODUTO)");
            }
            if (WhereCondition.Contains("O."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_CONTRATO O ON(O.CODCT = A.CODCT)");
            }
            if (WhereCondition.Contains("Q."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_TURNO_SERVICO Q ON(Q.ID = A.COD_TURNO_SERV)");
            }
            if (WhereCondition.Contains("R."))
            {
                sqlJoinCount.AppendLine("LEFT JOIN TB_FOLHA_DEPTO R ON(R.COD_DEPTO = A.COD_DEPTO)");
            }

            return sqlJoinCount.ToString();
        }



        public Documento_ProducaoAniel GetFirstDocProducao(string codCt, string projeto, int num_doc, string numObra)
        {
            return this.GetAll().FirstOrDefault(o => o.CodCt == codCt &&
                                                     o.Projeto == projeto &&
                                                     o.Num_Doc == num_doc &&
                                                     o.Num_Obra == numObra);
        }

        public List<DocProducaoBaixaAutomatica> GetOrdensServicoAuto()
        {
            List<DocProducaoBaixaAutomatica> retorno = new List<DocProducaoBaixaAutomatica>();


            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT ");
            sql.AppendLine("DOC.CODCT, DOC.PROJETO, DOC.NUM_OBRA, DOC.NUM_DOC, DOC.OBSERVACAO, ");
            sql.AppendLine("DOC.COD_ENCERRAMENTO, DOC.EQUIPE, DOC.DIA, DOC.DATAHORA_INI_ATEND, DOC.DATAHORA_FIM_ATEND, ");
            sql.AppendLine("DOC.CAIXA_FTTH, DOC.PORTA_FTTH, DOC.USUARIO, DOC.STATUS_EXECUCAO, ");
            sql.AppendLine("CTT.ENCERRA_OS_AUTO, CTT.DIAS_ENCERRAMENTO_AUTO, CTT.COD_ENCERRAMENTO_AUTO, ");
            sql.AppendLine("COD_ENC.STATUS_EXECUCAO AS DEFINE_ENCERRAMENTO ");
            sql.AppendLine("FROM ");
            sql.AppendLine("TB_DOCUMENTO_PRODUCAO DOC ");
            sql.AppendLine("LEFT JOIN ");
            sql.AppendLine("TB_CONTRATO CTT ON CTT.CODCT = DOC.CODCT ");
            sql.AppendLine("LEFT JOIN ");
            sql.AppendLine("TB_CODIGO_ENCERRAMENTO COD_ENC ON COD_ENC.COD_ENCERRAMENTO = CTT.COD_ENCERRAMENTO_AUTO ");
            sql.AppendLine("AND COD_ENC.CODCT = CTT.CODCT ");
            sql.AppendLine("WHERE ");
            sql.AppendLine("DOC.STATUS_EXECUCAO NOT IN (1, 2, 9) ");
            sql.AppendLine("AND CTT.ENCERRA_OS_AUTO = 'S' ");
            sql.AppendLine("AND DOC.DIA NOT BETWEEN(CURRENT_DATE - CTT.DIAS_ENCERRAMENTO_AUTO) AND CURRENT_DATE; ");

            Dictionary<string, string> parametros = new Dictionary<string, string>();

            using (var com = this._sessionT5Nh.Connection.CreateCommand())
            {
                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Nh.Connection, null).Result)
                {
                    while (reader.Read())
                    {
                        var doc = new DocProducaoBaixaAutomatica()
                        {
                            CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : string.Empty,
                            Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : string.Empty,
                            Num_Obra = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : string.Empty,
                            Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : 0,
                            Observacao = reader["OBSERVACAO"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO"]) : string.Empty,
                            Equipe = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : string.Empty,
                            Data_Ini_Atend = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]) : (DateTime?)null,
                            Data_Fim_Atend = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null,
                            CaixaFTTH = reader["CAIXA_FTTH"] != DBNull.Value ? Convert.ToString(reader["CAIXA_FTTH"]) : string.Empty,
                            PortaFTTH = reader["PORTA_FTTH"] != DBNull.Value ? Convert.ToString(reader["PORTA_FTTH"]) : string.Empty,
                            Usuario = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : string.Empty,
                            StatusExecucaoAtual = reader["STATUS_EXECUCAO"] != DBNull.Value ? Convert.ToInt64(reader["STATUS_EXECUCAO"]) : 0,
                            EncerraOSAuto = reader["ENCERRA_OS_AUTO"] != DBNull.Value ? Convert.ToChar(reader["ENCERRA_OS_AUTO"]) : ' ',
                            DiasEncerramentoAuto = reader["DIAS_ENCERRAMENTO_AUTO"] != DBNull.Value ? Convert.ToInt32(reader["DIAS_ENCERRAMENTO_AUTO"]) : 0,
                            CodEncerramentoAuto = reader["COD_ENCERRAMENTO_AUTO"] != DBNull.Value ? Convert.ToString(reader["COD_ENCERRAMENTO_AUTO"]) : string.Empty,
                            StatusExecucaoFinal = reader["DEFINE_ENCERRAMENTO"] != DBNull.Value ? Convert.ToString(reader["DEFINE_ENCERRAMENTO"]) : string.Empty,
                        };

                        retorno.Add(doc);
                    }
                }
            }


            return retorno;
        }


        public async Task<bool> SaveUpdateDocumentoAsync(Documento_ProducaoAniel documento)
        {
            using (var repository = new Conexao<Documento_ProducaoAniel>(_sessionT5Sql))
            {
                await repository.SaveOrUpdate(documento);
            }


            return true;

        }
        public DocumentoProducaoMongo GetOSMongo(Documento_ProducaoAniel documento)
        {
            var retorno = new DocumentoProducaoMongo();
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {

                MongoDB.Driver.Linq.IMongoQueryable<Aniel.T5.Model.mongo.DocumentoProducaoMongo> query =
                    (MongoDB.Driver.Linq.IMongoQueryable<DocumentoProducaoMongo>)repositoryMongo.GetQueryable().Where(x =>
                                                                            x.codCt == documento.CodCt &&
                                                                            x.projeto == documento.Projeto &&
                                                                            x.contrato == documento.Num_Doc &&
                                                                            x.numObra == documento.Num_Obra);
                retorno = query.SingleOrDefault();
            }
            return retorno;
        }
        public async Task<SenhaAniel> GetSenhaAniel(string usuario)
        {
            using (var repository = new Conexao<SenhaAniel>(_sessionT5Sql))
            {
                return await repository.selectFirst(p => p.Usuario == usuario);
            }
        }
        public async Task<FuncionarioAniel> GetFuncionario(string Num_Fre)
        {
            var retorno = new FuncionarioAniel();
            using (var repository = new Conexao<FuncionarioAniel>(_sessionT5Sql))
            {
                retorno = await repository.selectFirst(p => p.Num_Fre == Num_Fre);
            }
            return retorno;
        }
        public async Task<bool> UpdateMongo(DocumentoProducaoMongo documento)
        {
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {

                await repositoryMongo.Update(x => x.codCt == documento.codCt &&
                                                                               x.projeto == documento.projeto &&
                                                                               x.contrato == documento.contrato &&
                                                                               x.numObra == documento.numObra, documento);
            }
            return true;
        }
        public async Task<bool> SaveMongo(DocumentoProducaoMongo documento)
        {
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {

                await repositoryMongo.Save(documento);
            }
            return true;
        }

        public async Task<bool> AtualizaVinculoManutencao(AlteraVinculoManutencao request)
        {
            Int32 recordsTotal = 0;
            var retorno = false;
            try
            {
                string[] splitManutencao = request.Projeto_Manut.Split(new string[] { "__" }, StringSplitOptions.None);
                var numobra = splitManutencao[0].ToString();
                var codct = splitManutencao[1].ToString();
                var projeto = splitManutencao[2].ToString();
                var numdoc = splitManutencao[3].ToString();
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder($"UPDATE TB_DOCUMENTO_PRODUCAO SET NUMDOC_MANUT ='{numdoc}',CODCT_MANUT='{codct}' ,PROJETO_MANUT='{projeto}' ,NUMOBRA_MANUT='{numobra}' WHERE    num_obra =  '{request.Num_Obra}' and CODCT= '{request.CodCt}' ");
                    await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                    return true;
                }
            }
            catch (Exception e)
            {
                return retorno;
            }

        }


        public async Task<Boolean> isOsManutencao(string numObra)
        {
            using (var docProdRepo = new Conexao<Documento_ProducaoAniel>(_sessionT5Nh))
            {
                var docProdOS = await docProdRepo.selectFirst(d => d.Num_Obra == numObra);

                if (docProdOS is null)
                    return false;

                if (docProdOS.Tipo == "C")
                    return false;

                return true;
            }
        }

        public Documento_ProducaoAniel GetProximaOSAgendadaPraTecnico(string equipe)
        {
            var os = this.GetAll()
                       .Where(p => p.Equipe == equipe && p.Data_Maxima == DateTime.Now.Date && p.Hora_Maxima.HasValue)
                       .OrderBy(p => p.Hora_Maxima).
                       FirstOrDefault();

            return os;
		}

        //Verifica se material já foi integrado
        //Retorna True se o material já foi integrado, senão false
        public async Task<bool> ConfereMaterialIntegrado(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            bool foiIntegrado = false;
            Dictionary<String, Object> param = new Dictionary<string, object>
                {
                    { "@codct", codCt },
                    { "@projeto", projeto },
                    { "@numobra", numObra },
                    { "@codMat", codMat },
                    { "@codCpl", codCpl }
                };

            string paramString = string.Join(", ", param.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [ConfereMaterialIntegrado] -> {paramString}", Usuario, Detalhe, null);

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT INTEGRADO FROM TB_MATERIAL_TAREFA");
                    sql.AppendLine(" WHERE CODCT = @codct AND PROJETO = @projeto AND NUM_OBRA = @numobra AND CODMAT = @codMat AND CODCPL = @codCpl ");

                    

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                    {
                        if (reader.Read())
                        {
                            foiIntegrado = reader["INTEGRADO"] != DBNull.Value && Convert.ToInt32(reader["INTEGRADO"]) == 1;

                        }

                    }
                }

                
            }
            catch (Exception ex)
            {
                await enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                throw new Exception("Erro ao conferir se material já foi integrado.");
            }

            string integrado = "";

            if (foiIntegrado)
            {
                integrado = "Material já foi integrado!";
            }
            else
            {
                integrado = "Material não foi integrado!";
            }

            await enviarLog(integrado, Usuario, Detalhe, idLog.Result);
            return foiIntegrado;

        }

        public async Task<bool> RemoverAplicacao(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            bool retorno = true;
            Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@codct", codCt },
                    { "@projeto", projeto },
                    { "@numobra", numObra },
                    { "@codMat", codMat },
                    { "@codCpl", codCpl }
                };

            string paramString = string.Join(", ", parametros.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [RemoverAplicacao] -> {paramString}", Usuario, Detalhe, null);

            using (var com = sessionT5.Connection.CreateCommand())
            {
                try
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" DELETE FROM TB_MATERIAL_TAREFA ");
                    sql.AppendLine(" WHERE CODCT = @codct AND PROJETO = @projeto AND NUM_OBRA = @numobra AND CODMAT = @codMat AND CODCPL = @codCpl ");


                    var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
                }
                catch (Exception ex)
                {
                    await enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                    retorno = false;
                }

                await enviarLog("Aplicação removida com sucesso!", Usuario, Detalhe, idLog.Result);
                return retorno;
            }
        }

        //Coleta dados utilizados na movimentação de estoque da integração HubSoft
        //consultando a TB_DOCUMENTO_PRODUCAO e a TB_EQUIPE
        public async Task<ConsultaIdOsHubsoft> GetDadosOSByNumObra(ISession sessionT5, string numObra, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            ConsultaIdOsHubsoft retorno = null;
            Dictionary<String, Object> param = new Dictionary<string, object>
                {
                    { "@numobra", numObra }
                };

            string paramString = string.Join(", ", param.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [GetDadosOSByNumObra] -> {paramString}", Usuario, Detalhe, null);

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ADICIONAL_1, A.ID_HUBSOFT, A.SALDO_ADICIONADO, A.EQUIPE, A.PROJETO, A.CODCT, ");
                    sql.AppendLine("A.NUM_DOC, A.DATA_MAXIMA, A.COD_TIPO_SERV, A.TAREFA, A.ID_CLIENTE, G.EQUIPEAUX, G.NOME FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE G ON(A.EQUIPE = G.EQUIPE) ");
                    sql.AppendLine(" WHERE A.NUM_OBRA = @numobra ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                    {
                        if (reader.Read())
                        {
                            retorno = new ConsultaIdOsHubsoft
                            {
                                adicional_1 = reader["ADICIONAL_1"] == DBNull.Value ? null : Convert.ToString(reader["ADICIONAL_1"]),
                                id_hubsoft = reader["ID_HUBSOFT"] == DBNull.Value ? null : Convert.ToString(reader["ID_HUBSOFT"]),
                                equipeaux = reader["EQUIPEAUX"] == DBNull.Value ? null : Convert.ToString(reader["EQUIPEAUX"]),
                                nome = reader["NOME"] == DBNull.Value ? null : Convert.ToString(reader["NOME"]),
                                saldo_adicionado = reader["SALDO_ADICIONADO"] != DBNull.Value && Convert.ToInt32(reader["SALDO_ADICIONADO"]) == 1,
                                equipe = reader["EQUIPE"] == DBNull.Value ? null : Convert.ToString(reader["EQUIPE"]),
                                projeto = reader["PROJETO"] == DBNull.Value ? null : Convert.ToString(reader["PROJETO"]),
                                codct = reader["CODCT"] == DBNull.Value ? null : Convert.ToString(reader["CODCT"]),
                                num_doc = reader["NUM_DOC"] == DBNull.Value ? null : Convert.ToString(reader["NUM_DOC"]),
                                data_agendamento = reader["DATA_MAXIMA"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DATA_MAXIMA"]),
                                cod_tipo_serv = reader["COD_TIPO_SERV"] == DBNull.Value ? null : Convert.ToString(reader["COD_TIPO_SERV"]),
                                tarefa = reader["TAREFA"] == DBNull.Value ? null : Convert.ToString(reader["TAREFA"]),
                                id_cliente = reader["ID_CLIENTE"] == DBNull.Value ? null : Convert.ToString(reader["ID_CLIENTE"]),

                            };


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                throw new Exception(ex.Message);
            }

            string json = JsonSerializer.Serialize(retorno);
            await enviarLog(json, Usuario, Detalhe, idLog.Result);
            return retorno;
        }


        //Coleta dados utilizados na movimentação de estoque da integração HubSoft
        //consultando a TB_DOCUMENTO_PRODUCAO
        public async Task<ConsultaNumOs> GetDadosOSByNumOs(ISession sessionT5, string numOs, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            ConsultaNumOs retorno = null;
            Dictionary<String, Object> param = new Dictionary<string, object>
                {
                    { "@numOs", numOs }
                };

            string paramString = string.Join(", ", param.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [GetDadosOSByNumOs] -> {paramString}", Usuario, Detalhe, null);

            try
            {
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT NUM_OBRA, PROJETO, CODCT FROM TB_DOCUMENTO_PRODUCAO ");
                    sql.AppendLine(" WHERE NUM_OBRA_ORIGINAL = @numOs ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                    {
                        if (reader.Read())
                        {
                            retorno = new ConsultaNumOs
                            {
                                num_obra = Convert.ToString(reader["NUM_OBRA"]),
                                projeto = Convert.ToString(reader["PROJETO"]),
                                codct = Convert.ToString(reader["CODCT"])
                            };


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                throw new Exception(ex.Message);
            }

            string json = JsonSerializer.Serialize(retorno);
            await enviarLog(json, Usuario, Detalhe, idLog.Result);
            return retorno;
        }

        //Busca materiais aplicados para utilizar na integração com o HubSoft
        public List<Material_Aplicado> MateriaisAplicados(ISession sessionT5, String Codct, String Projeto, String OS, string Detalhe, string Usuario, string CodMat, bool provisionamento, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {

            List<Material_Aplicado> retorno = new List<Material_Aplicado>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>
            {
                { "@CODCT", Codct },
                { "@PROJETO", Projeto },
                { "@NUM_OBRA", OS },
                { "@CODMAT", CodMat}
            };
            string paramString = string.Join(", ", parametros.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [MateriaisAplicados] -> {paramString}", Usuario, Detalhe, null);

            using (var com = sessionT5.Connection.CreateCommand())
            {
               

                StringBuilder sql = new StringBuilder($@" SELECT A.CODCT,A.CODCPL , A.PROJETO, A.NUM_OBRA, A.TAREFA, A.DATA_APLIC, A.CODMAT, B.DESCRICAO, A.CODCPL, B.UNID, C.NOME, B.VALOR, 
                                                    A.QUANTIDADE, (B.VALOR * A.QUANTIDADE) AS TOTAL, A.QUANT_REMOV, (B.VALOR * A.QUANT_REMOV) AS TOTAL_REMOV,B.COMODATO,B.CODMAT_AUX,
                                                    E.D3_LOCAL AS ESTOQUE
                                                    FROM TB_MATERIAL_TAREFA A 
                                                    INNER JOIN TB_MATERIAL B 
                                                    ON(A.CODMAT = B.CODMAT) 
                                                    INNER JOIN TB_EQUIPE C 
                                                    ON(A.EQUIPE = C.EQUIPE) 
                                                    INNER JOIN TB_INTEGRACAO_PROTHEUS_REVERSE E
                                                    ON(E.CODCT = A.CODCT AND E.PROJETO = A.PROJETO)
                                                    WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND 
                                                    A.NUM_OBRA = @NUM_OBRA ");

                sql.AppendLine("AND B.CLASSIFICACAO IN('I','B') AND A.QUANTIDADE > 0 AND A.CODMAT = @CODMAT"); // material aplicado 

                if(provisionamento)
                {
                    sql.AppendLine(" AND(COALESCE(A.INTEGRADO, 0) = 0) ");
                }
                else
                {
                    sql.AppendLine(" AND(COALESCE(A.INTEGRADO, 1) = 1) ");
                }

                    //Busca com paginacao
                    using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                    {
                        while (_reader.Read())
                        {
                            var materialAplicado = new Material_Aplicado();

                            materialAplicado.CodCt = Convert.ToString(_reader["CODCT"]);
                            materialAplicado.Projeto = Convert.ToString(_reader["PROJETO"]);
                            materialAplicado.Num_Obra = Convert.ToString(_reader["NUM_OBRA"]);
                            materialAplicado.Tarefa = Convert.ToString(_reader["TAREFA"]);
                            materialAplicado.Data_Aplic = Convert.ToString(_reader["DATA_APLIC"]);
                            materialAplicado.Codigo = Convert.ToString(_reader["CODMAT"]);
                            materialAplicado.CodigoAuxiliar = Convert.ToString(_reader["CODMAT_AUX"]);
                            materialAplicado.Codigo_Complementar = Convert.ToString(_reader["CODCPL"]);
                            materialAplicado.Descricao = Convert.ToString(_reader["DESCRICAO"]);
                            materialAplicado.Equipe = Convert.ToString(_reader["NOME"]);
                            materialAplicado.Quantidade = String.Format("{0:N2}", Convert.ToDouble(_reader["QUANTIDADE"] != DBNull.Value ? _reader["QUANTIDADE"] : 0));
                            materialAplicado.Unidade = Convert.ToString(_reader["UNID"]);
                            materialAplicado.Valor_Unitario = String.Format("{0:N2}", Convert.ToDouble(_reader["VALOR"] != DBNull.Value ? _reader["VALOR"] : 0));
                            materialAplicado.Valor_Total = String.Format("{0:N2}", Convert.ToDouble(_reader["TOTAL"] != DBNull.Value ? _reader["TOTAL"] : 0));
                            materialAplicado.Comodato = Convert.ToString(_reader["COMODATO"]);
                            materialAplicado.Estoque = Convert.ToString(_reader["ESTOQUE"]);

                            retorno.Add(materialAplicado);
                        }
                    }
            }

            string json = JsonSerializer.Serialize(retorno);
            enviarLog(json, Usuario, Detalhe, idLog.Result);
            return retorno;

        }

        //Método responsável por marcar o material como integrado 
        public Task<long> MarcarIntegrado(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, bool provisionamento, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            Task<long> retorno = Task.FromResult(0L);
            Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@codct", codCt },
                    { "@projeto", projeto },
                    { "@numobra", numObra },
                    { "@codMat", codMat },
                    { "@codCpl", codCpl }
                };

            string paramString = string.Join(", ", parametros.Select(kv => $"{kv.Key}={kv.Value}"));
            var idLog = enviarLog($"Integração HubSoft [MarcarIntegrado] -> {paramString}", Usuario, Detalhe, null);

            using (var com = sessionT5.Connection.CreateCommand())
            {
                try
                {
                    StringBuilder sql = new StringBuilder();

                    if(provisionamento)
                    {
                        sql.Append(" UPDATE TB_MATERIAL_TAREFA SET INTEGRADO = 1 ");
                    }
                    else
                    {
                        sql.Append(" UPDATE TB_MATERIAL_TAREFA SET INTEGRADO = 0 ");
                    }
                        
                    sql.Append(" WHERE CODCT = @codct AND PROJETO = @projeto AND NUM_OBRA = @numobra AND CODMAT = @codMat AND CODCPL = @codCpl ");

                    

                    var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
                }
                catch (Exception ex)
                {
                    enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                }

                enviarLog("Material gravado com sucesso!", Usuario, Detalhe, idLog.Result);
                return retorno;
            }

        }

        public bool SetSaldoAdicionado(ISession sessionT5, String codct, String projeto, String num_obra)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();


            using (var com = sessionT5.Connection.CreateCommand())
            {

                sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO A SET A.SALDO_ADICIONADO = 1");
                sql.AppendLine("WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA");

                parametros.Add("@CODCT", codct);
                parametros.Add("@PROJETO", projeto);
                parametros.Add("@NUM_OBRA", num_obra);

                var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);



            }

            return true;
        }

        public void AdicionaSaldo(ISession sessionT5, LoginHubsoftModel login, string urlBase, string detalheHubsoft, string usuarioHubsoft, string codCt, string projeto, string numObra, List<Material_Aplicado> materiais, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var materiaisAdicionarHubsoft = new List<Material_Aplicado>();

            //filtra apenas insumo e seriais que NÃO existem no Hubsoft
            foreach (var materialSerial in materiais.Where(x => x.Codigo_Complementar != "-"))
            {
                if (!EstoqueHubSoftDao.SerialJaExiste(materialSerial.Codigo_Complementar, login, urlBase, detalheHubsoft, usuarioHubsoft, enviarLog))
                    materiaisAdicionarHubsoft.Add(materialSerial);
            }

            materiaisAdicionarHubsoft.AddRange(materiais.Where(x => x.Codigo_Complementar == "-").ToList());

            if (materiaisAdicionarHubsoft.Count > 0)
            {
                DateTime dataAplic;

                if (!string.IsNullOrWhiteSpace(materiais[0].Data_Aplic) &&
                    DateTime.TryParse(materiais[0].Data_Aplic, out var parsedDate))
                {
                    dataAplic = parsedDate;
                }
                else
                {
                    dataAplic = DateTime.Now;
                }

                //adiciona o saldo no estoque 
                var modelCompra = new CompraEstoqueModel
                {
                    movimentos = new List<Movimento>
                    {
                        new Movimento
                        {
                           data_compra = dataAplic.ToString("yyyy-MM-dd"),
                           data_movimento = dataAplic.ToString("yyyy-MM-dd"),
                            tipo = "entrada",
                            tipo_destino = "produto_compra",
                            local_estoque = new HubSoftService.Model.HubSoft.Entrada.LocalEstoque { id_local_estoque = Convert.ToInt32(materiais[0].Estoque) },
                            fornecedor = new HubSoftService.Model.HubSoft.Entrada.Fornecedor { id_fornecedor = CONSTANTES.Id_Fornecedor },
                            usuario_responsavel = new HubSoftService.Model.HubSoft.Entrada.UsuarioResponsavel { id = CONSTANTES.Id_Usuario_Responsavel },
                            status_compra = new HubSoftService.Model.HubSoft.Entrada.StatusCompra { id_status_compra = CONSTANTES.Id_Status_Compra },
                            produtos = new List<HubSoftService.Model.HubSoft.Entrada.Produto>()
                        }
                    }
                };

                foreach (var material in materiaisAdicionarHubsoft)
                {
                    modelCompra.movimentos[0].produtos.Add(new HubSoftService.Model.HubSoft.Entrada.Produto
                    {
                        produto = new HubSoftService.Model.HubSoft.Entrada.Produto2 { id_produto = Convert.ToInt32(material.Codigo) },
                        quantidade = Convert.ToDouble(material.Quantidade),
                        valor_unitario = Convert.ToDouble(material.Valor_Unitario),
                        utilizar_casa_decimal_valor_unitario = true
                    });
                }

                var retornoCriacao = EstoqueHubSoftDao.CriarEntradaEstoque(modelCompra, login, urlBase, detalheHubsoft, usuarioHubsoft, enviarLog);

                //cadastro dos seriais
                //pega os IDs dos materiais que entraram como compra no Hubsoft e junta com seus respectivos seriais (ID patrimonio Hubsoft - Serial aniel)
                var consultaEntrada = EstoqueHubSoftDao.ConsultaEntradaEstoque(retornoCriacao.movimento.id_vinculo_origem, login, urlBase, detalheHubsoft, usuarioHubsoft, enviarLog);

                var materiaisSerializados = materiaisAdicionarHubsoft.Where(x => x.Codigo_Complementar != "-").ToList();

                Dictionary<int, string> idSerial = new Dictionary<int, string>();

                for (int i = 0; i < materiaisSerializados.Count(); i++)
                {
                    idSerial.Add(consultaEntrada.produtos_item.data[i].id_produto_item, materiaisSerializados[i].Codigo_Complementar);
                }

                var modeloSeriais = new HubSoftService.Model.HubSoft.Entrada.CadastroSerial.CadastrarSerialModel { itens = new List<HubSoftService.Model.HubSoft.Entrada.CadastroSerial.Item>() };

                foreach (var modelo in idSerial)
                {
                    modeloSeriais.itens.Add(new HubSoftService.Model.HubSoft.Entrada.CadastroSerial.Item { id_produto_item = modelo.Key, numero_serie = modelo.Value });
                }

                if (modeloSeriais.itens.Count > 0)
                    EstoqueHubSoftDao.CadastrarSeriais(modeloSeriais, login, urlBase, detalheHubsoft, usuarioHubsoft, enviarLog);
            }

            SetSaldoAdicionado(sessionT5, codCt, projeto, numObra);
        }

        public void AdicionarEquipamento(ISession sessionT5, string detalheHubsoft, string usuarioHubsoft, string serial, string codmat, 
            string usuario, string numObra, ConsultaIdOsHubsoft ordemServico)
        {
           try
           {
                StringBuilder mensagem = new StringBuilder();


                //String codct = ConfigurationManager.AppSettings["ContratoPadrao"].ToString();
                //String projeto = ConfigurationManager.AppSettings["ProjetoPadrao"].ToString();

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var com = sessionT5.Connection.CreateCommand())
                {

                    sql.Append("update tb_material_tarefa ");
                    sql.Append("set QUANTIDADE = @pQuantidade, QUANT_REMOV = @pQuantRemov, QUANT_PROVISORIO = @pQuantProvisorio, ");
                    sql.Append("DATA_APLIC = @pDataAplic, USUARIO = @pUsuario, DIA = @pDia, HORA = @pHora, APLICADO_CLIENTE = @pAplicadoCliente, ID_CLIENTE = @idCliente, CODMAT = @CodMat ");
                    sql.Append("where (codct = @pCodCt) and (projeto = @pProjeto) and (num_obra = @pNumObra) ");
                    sql.Append("and (tarefa = @pTarefa) and (codmat = @CodMat) and (codcpl = @pCodCpl) and (num_doc = @pNumDoc) ");
                    sql.Append("and (equipe = @pEquipe) and (cod_tipo_serv = @pCodTipoServ)");


                    parametros.Add("@pCodCt", ordemServico.codct);
                    parametros.Add("@pProjeto", ordemServico.projeto);
                    parametros.Add("@CodMat", codmat);


                    var temp = 1;
                    parametros.Add("@pQuantidade", temp);
                    parametros.Add("@pQuantRemov", "0");
                    parametros.Add("@pQuantProvisorio", "0");



                    try { parametros.Add("@pCodCpl", serial.ToUpper()); }
                    catch { parametros.Add("@pCodCpl", "-"); }


                    parametros.Add("@pDataAplic", ordemServico.data_agendamento?.ToShortDateString().Replace("/", "."));
                    parametros.Add("@pTarefa", ordemServico.tarefa);
                    parametros.Add("@pEquipe", usuarioHubsoft);
                    parametros.Add("@pCodTipoServ", ordemServico.cod_tipo_serv);
                    parametros.Add("@pNumDoc", ordemServico.num_doc);

                    parametros.Add("@pNumObra", numObra);

                    parametros.Add("@pUsuario", usuario);
                    parametros.Add("@pDia", DateTime.Now.ToShortDateString().Replace("/", "."));
                    parametros.Add("@pHora", DateTime.Now.ToShortTimeString());

                    parametros.Add("@pAplicadoCliente", "N");
                    parametros.Add("@idCliente", ordemServico.id_cliente);

                    int rowsAffect = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null).GetAwaiter().GetResult();

                    if (rowsAffect == 0)
                    {

                        sql.Clear();
                        parametros.Clear();

                        sql.Append("insert into TB_MATERIAL_TAREFA(CODCT, PROJETO, CODMAT, CODCPL, QUANTIDADE, ");
                        sql.Append("QUANT_REMOV, DATA_APLIC, TAREFA, EQUIPE, COD_TIPO_SERV, NUM_DOC, NUM_OBRA, ");
                        sql.Append("USUARIO, DIA, HORA, APLICADO_CLIENTE,QUANT_PROVISORIO, ID_CLIENTE) ");
                        sql.Append("VALUES(@pCodCt,@pProjeto, @pCodMat, @pCodCpl, @pQuantidade, @pQuantRemov, ");
                        sql.Append("@pDataAplic, @pTarefa, @pEquipe, @pCodTipoServ, @pNumDoc, @pNumObra, ");
                        sql.Append("@pUsuario, @pDia, @pHora, @pAplicadoCliente,@pQuantProvisorio, @idCliente) ");

                        parametros.Add("@pCodCt", ordemServico.codct);
                        parametros.Add("@pProjeto", ordemServico.projeto);
                        parametros.Add("@pCodMat", codmat);


                        temp = 1;
                        parametros.Add("@pQuantidade", temp);
                        parametros.Add("@pQuantRemov", "0");
                        parametros.Add("@pQuantProvisorio", "0");



                        try { parametros.Add("@pCodCpl", serial.ToUpper()); }
                        catch { parametros.Add("@pCodCpl", "-"); }

                        //parametros.Add("@pDataAplic", DateTime.Now.ToShortDateString().Replace("/", "."));
                        parametros.Add("@pDataAplic", ordemServico.data_agendamento?.ToShortDateString().Replace("/", "."));
                        parametros.Add("@pTarefa", ordemServico.tarefa);
                        parametros.Add("@pEquipe", usuario);
                        parametros.Add("@pCodTipoServ", ordemServico.cod_tipo_serv);
                        parametros.Add("@pNumDoc", ordemServico.num_doc);

                        parametros.Add("@pNumObra", numObra);

                        parametros.Add("@pUsuario", usuario);
                        parametros.Add("@pDia", DateTime.Now.ToShortDateString().Replace("/", "."));
                        parametros.Add("@pHora", DateTime.Now.ToShortTimeString());

                        parametros.Add("@pAplicadoCliente", "N");
                        parametros.Add("@idCliente", ordemServico.id_cliente);

                        var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
                    }


                }
            }
            catch(Exception ex)
            {
                
                throw new Exception(ex.Message);
            }
            
        }

        public AplicacaoRemocaoMaterial GetAplicacaoNaoIntegrados(ISession sessionT5, string numObra, string codCpl, string codMat, string projeto, bool provisionamento, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            AplicacaoRemocaoMaterial material = new AplicacaoRemocaoMaterial();
            var idLog = enviarLog($"Integração HubSoft [GetAplicacaoNaoImportados] -> Número OS: {numObra}", Usuario, Detalhe, null);

            try
            {
                StringBuilder mensagem = new StringBuilder();

                StringBuilder sql = new StringBuilder();

               

                using (var com = sessionT5.Connection.CreateCommand())
                {
                    sql.AppendLine(@"SELECT DISTINCT B.NUM_OBRA,DOCU.ADICIONAL_1 ,B.CODMAT,B.CODCPL ,B.INTEGRADO, B.PROJETO, B.CODCT, C.D3_LOCAL LOCAL,
                            E.EQUIPEAUX, D.CLASSIFICACAO, E.EQUIPE_INTEGRATOR DEPOSITO_TECNICO
                            FROM TB_MATERIAL_TAREFA B                                    
                            INNER JOIN TB_INTEGRACAO_PROTHEUS_REVERSE C 
                            ON (C.CODCT = B.CODCT AND C.PROJETO = B.PROJETO ) 
                            INNER JOIN TB_MATERIAL D
                            ON (D.CODMAT = B.CODMAT)
                            LEFT JOIN TB_DOCUMENTO_PRODUCAO DOCU
                            ON(DOCU.CODCT = B.CODCT AND DOCU.PROJETO = B.PROJETO AND DOCU.NUM_OBRA = B.NUM_OBRA AND DOCU.NUM_DOC = B.NUM_DOC )
                            INNER JOIN TB_EQUIPE E
                            ON(E.EQUIPE = DOCU.EQUIPE )
                            WHERE B.CODCPL = @codCpl
                            AND B.NUM_OBRA = @numObra 
                            AND B.CODMAT = @codMat 
                            AND B.PROJETO = @projeto ");

                    if(!provisionamento)
                    {
                        sql.AppendLine(" AND COALESCE(B.INTEGRADO,1) = 1");
                    }
                    else
                    {
                        sql.AppendLine(" AND COALESCE(B.INTEGRADO,0) = 0");

                    }

                        Dictionary<String, Object> parametros = new Dictionary<string, object>()
                    {
                        {"idConfig",CONSTANTES.IdConfigIntegrator}, {"numObra",numObra}, {"codCpl", codCpl}, {"codMat", codMat}, {"projeto", projeto}
                    };

                    using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                    {
                        if(_reader.Read())
                        {
                            var registro = new AplicacaoRemocaoMaterial()
                            {
                                NumObra = _reader["NUM_OBRA"] == DBNull.Value ? null : Convert.ToString(_reader["NUM_OBRA"]),
                                LocalEstoque = _reader["LOCAL"] == DBNull.Value ? 0 : Convert.ToInt64(_reader["LOCAL"]),
                                Codmat = _reader["CODMAT"] == DBNull.Value ? null : Convert.ToString(_reader["CODMAT"]),
                                Codcpl = _reader["CODCPL"] == DBNull.Value ? null : Convert.ToString(_reader["CODCPL"]),
                                IdHubsoft = _reader["ADICIONAL_1"] == DBNull.Value ? 0 : Convert.ToInt32(_reader["ADICIONAL_1"]),
                                Equipe = _reader["EQUIPEAUX"] == DBNull.Value ? null : Convert.ToString(_reader["EQUIPEAUX"]),
                                DepositoTecnico = _reader["DEPOSITO_TECNICO"] == DBNull.Value ? null : Convert.ToString(_reader["DEPOSITO_TECNICO"]),
                                Projeto = _reader["PROJETO"] == DBNull.Value ? null : Convert.ToString(_reader["PROJETO"]),
                                Codct = _reader["CODCT"] == DBNull.Value ? null : Convert.ToString(_reader["CODCT"]),
                            };

                            if (_reader["CLASSIFICACAO"] != DBNull.Value &&
                                Convert.ToString(_reader["CLASSIFICACAO"]) != "I" &&
                                !string.IsNullOrEmpty(registro.Codmat) &&
                                registro.Codmat.Length > 1)
                            {
                                registro.Codmat = registro.Codmat.Substring(1);
                            }

                            material = registro;
                        }

                        string json = JsonSerializer.Serialize(material);
                        enviarLog(json, Usuario, Detalhe, idLog.Result);
                    }
                }

            }
            catch (Exception ex)
            {
                enviarLog(ex.Message, Usuario, Detalhe, idLog.Result);
                throw new Exception(ex.Message);
            }

            return material;
        }

        public MovimentoProdutoUtilizadoFecharOS BindMateriaisAplicacao(AplicacaoRemocaoMaterial materialPendente, OrdemServicoHubSoft ordemServicoHubSoft,
            LoginHubsoftModel loginHubsoft, IntegracoesExternas dadosIntegracaoHubsoft, string detalheHubsoft, string usuarioHubsoft,
            Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)

        {
            MovimentoProdutoUtilizadoFecharOS retorno = new MovimentoProdutoUtilizadoFecharOS()
            {
                produtos = new List<ProdutoFecharOS>(),
                id_local_estoque = Convert.ToInt32(materialPendente?.DepositoTecnico),
                id_tipo_movimento_estoque = CONSTANTES.IdTipoMovimentoComodato,
                id_cliente_servico = ordemServicoHubSoft.id_cliente_servico,
            };

            if (materialPendente != null)
            {

                var listaPatrimonios = new List<PatrimonioFecharOS>();


                var patrimonio = EstoqueHubSoftDao.GetPatrimonio(materialPendente.Codcpl, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, enviarLog);

                if(patrimonio?.produto.Count() == 0)
                {
                    return null;
                }
                                        
                 listaPatrimonios.Add(new PatrimonioFecharOS { id_produto_item = patrimonio.produto.FirstOrDefault().id_produto_item });
                   
                 retorno.produtos.Add(
                    new ProdutoFecharOS
                       {
                         id_produto = Convert.ToInt32(materialPendente.Codmat),
                         quantidade = 1,
                         campo_identificacao_patrimonio = "id_produto_item",
                         patrimonios = listaPatrimonios
                       }
                    );
                
            }

            retorno.observacao = $"Aplicação de material OS {materialPendente?.NumObra}";
            return retorno;
        }

        public MovimentoProdutoUtilizadoFecharOSPop BindMateriaisAplicacaoPop(AplicacaoRemocaoMaterial materialPendente,
             LoginHubsoftModel loginHubsoft, IntegracoesExternas dadosIntegracaoHubsoft, string detalheHubsoft, string usuarioHubsoft,
            Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            MovimentoProdutoUtilizadoFecharOSPop retorno = new MovimentoProdutoUtilizadoFecharOSPop()
            {
                produtos = new List<ProdutoFecharOS>(),
                id_local_estoque = Convert.ToInt32(materialPendente?.DepositoTecnico),
            };

            if (materialPendente != null)
            {

                var listaPatrimonios = new List<PatrimonioFecharOS>();
                var patrimonio = EstoqueHubSoftDao.GetPatrimonio(materialPendente.Codcpl, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, enviarLog);

                if (patrimonio?.produto.Count() == 0)
                {
                    return null;
                }

                listaPatrimonios.Add(new PatrimonioFecharOS { id_produto_item = patrimonio.produto.FirstOrDefault().id_produto_item });

                retorno.produtos.Add(
                    new ProdutoFecharOS
                    {
                        id_produto = Convert.ToInt32(materialPendente.Codmat),
                        quantidade = Convert.ToDouble(materialPendente.Quantidade),
                        campo_identificacao_patrimonio = "id_produto_item",
                        patrimonios = listaPatrimonios
                    }
                );

            }

            retorno.observacao = $"Aplicação de material OS {materialPendente?.NumObra}";
            return retorno;
        }

        public Int64 GetIDConsumoProtheus(ISession sessionT5, string numObra, string codCpl, string codMat, string projeto, string codCt,
            string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var idLog = enviarLog($"Integração HubSoft [GetIDConsumoProtheus] -> Número OS: {numObra}", Usuario, Detalhe, null);
            Int64 resultado = 0;
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ID FROM TB_CONSUMO_PROTHEUS WHERE ");
            sql.Append("NUM_OBRA = @numObra AND CODCPL = @codCpl ");
            sql.Append("AND CODMAT = @codMat AND PROJETO = @projeto ");
            sql.Append("AND CODCT = @codCt ");

            Dictionary<String, Object> parametros = new Dictionary<string, object>()
            {
               {"numObra",numObra}, {"codCpl", codCpl}, {"codMat", codMat}, {"projeto", projeto}, { "codCt", codCt}
            };

            using (var com = sessionT5.Connection.CreateCommand())
            {
                using (DbDataReader _reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null).Result)
                {
                    if (_reader.Read())
                    {
                        resultado = _reader["ID"] == DBNull.Value ? 0 : Convert.ToInt64(_reader["ID"]);
                    }
                }
            }

            enviarLog($"ID retornado: {resultado}", Usuario, Detalhe, idLog.Result);
            return resultado;
        }

        public void MarcarImportados(ISession sessionT5, long id, string envio, string retorno)
        {
            using (var com = sessionT5.Connection.CreateCommand())
            {

                    StringBuilder sql = new StringBuilder();

                    sql.Append("UPDATE TB_CONSUMO_PROTHEUS ");
                    sql.Append("SET IMPORTADO = 'S', DATAHORA_IMPORTACAO = CURRENT_TIMESTAMP, ENVIO = @ENVIO, RETORNO = @RETORNO, EXCECAO = NULL ");
                    sql.Append("WHERE ID = @ID ");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                      { "@ID", id },
                      { "@ENVIO", envio },
                      { "@RETORNO", retorno }
                    };

                    var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);


                
            }
           
        }

        public void MarcarExcecao(ISession sessionT5, long id, string envio, string excecao)
        {
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.Append("UPDATE TB_CONSUMO_PROTHEUS ");
                sql.Append("SET IMPORTADO = 'N', DATAHORA_IMPORTACAO = CURRENT_TIMESTAMP, ENVIO = @ENVIO, EXCECAO = @EXCECAO ");
                sql.Append("WHERE ID = @ID ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@ENVIO", envio },
                    { "@EXCECAO", excecao }
                };

                var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
            }
                
            
        }

        public void AtualizaPPPOEDocumentoProducao(ISession sessionT5, LoginRadius loginRadius, string numObra, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var idLog = enviarLog($"Integração HubSoft [AtualizaPPPOEDocumentoProducao] -> Número OS: {numObra}, PPPOE: {loginRadius.Login}, ID Login: {loginRadius.Id}", Usuario, Detalhe, null);

            try
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                using (var com = sessionT5.Connection.CreateCommand())
                {

                    sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO A SET A.ID_LOGIN = @ID_LOGIN, ");
                    sql.AppendLine("A.PPPOE = @PPPOE, A.SENHA_BAIXA = @SENHA_PPPOE");
                    sql.AppendLine("WHERE A.NUM_OBRA = @NUM_OBRA");

                    parametros.Add("@ID_LOGIN", loginRadius.Id);
                    parametros.Add("@PPPOE", loginRadius.Login);
                    parametros.Add("@SENHA_PPPOE", loginRadius.SenhaPPPOE);
                    parametros.Add("@NUM_OBRA", numObra);

                    var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);


                    enviarLog($"Dados do login radius atualizados com sucesso na ordem de serviço!", Usuario, Detalhe, idLog.Result);

                }
            }
            catch (Exception ex)
            {
                enviarLog($"{ex.Message}", Usuario, Detalhe, idLog.Result);
            }
           
        }

    }
}
