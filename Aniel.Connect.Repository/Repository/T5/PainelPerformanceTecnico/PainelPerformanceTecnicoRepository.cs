using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;

namespace Aniel.Connect.Repository.T5.ReincidenciaDocProd
{
    public class PainelPerformanceTecnicoRepository : IPainelPerformanceTecnicoRepository
    {
        private readonly ISession _sessionT5;
        private readonly ISession _sessionT5NH;
        private readonly ISession _sessionConnectNH;
        private readonly ConexaoMongo _conexaoMongo;

        public PainelPerformanceTecnicoRepository(ISession sessionT5NH, ISession sessionT5, ISession sessionConnectNH, ConexaoMongo conexaoMongo)
        {
            this._sessionT5NH = sessionT5NH;
            this._sessionT5 = sessionT5;
            this._sessionConnectNH = sessionConnectNH;
            this._conexaoMongo = conexaoMongo;
        }

        public PainelPerformanceTecnicoResponse GetAll(PainelPerformanceTecnicoDataTableRequest request)
        {
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(this._sessionConnectNH))
            using (var repositoryHorario = new Conexao<Horario_Connect>(this._sessionT5NH))
            using (var repositoryDocumentoProducao = new Conexao<Documento_ProducaoAniel>(this._sessionT5NH))
            using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(this._conexaoMongo))
            {
                List<Horario_Connect> horarios = (repositoryHorario.QueryableSelect()).ToList();
                var config = repositoryConfig.selectFirst().Result;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<PainelPerformanceTecnicoListModel> retorno = new List<PainelPerformanceTecnicoListModel>();
                StringBuilder sqlFrom = new StringBuilder("");
                StringBuilder sql = new StringBuilder();
                sql.AppendLine(@"SELECT EQUIPE,NOME_TECNICO,DATA_MAXIMA,DISTANCIA_PLANEJADA,DISTANCIA_REAL,MINUTOS_PARALISADO,ID_HORARIO_CONNECT,INDISPONIVEL_CONNECT,");
                sql.AppendLine(@"SUM (CASE (STATUS_EXECUCAO)
                                    WHEN 2 THEN (COALESCE(TEMPO_EXECUCAO,CAST('00:30:00.000' AS TIME)) - CAST('00:00:00.000' AS TIME)) / 3600
                                    ELSE 0
                                  END) AS TEMPO_EXECUCAO_TOTAL,");
                sql.AppendLine(@"SUM(CASE (STATUS_EXECUCAO)");
                sql.AppendLine(@"WHEN 1 THEN 1");
                sql.AppendLine(@"ELSE 0");
                sql.AppendLine("END) AS IMPRODUTIVAS,");
                sql.AppendLine("SUM(CASE (STATUS_EXECUCAO)");
                sql.AppendLine("WHEN 1 THEN 0");
                sql.AppendLine("WHEN 2 THEN 0");
                sql.AppendLine(@"ELSE 1");
                sql.AppendLine("END) AS ABERTAS,");
                sql.AppendLine("SUM(CASE (STATUS_EXECUCAO)");
                sql.AppendLine("WHEN 2 THEN 1");
                sql.AppendLine("ELSE 0 ");
                sql.AppendLine("END) AS PRODUTIVAS");

                List<string> condicoes = new List<string>();
                if (!String.IsNullOrEmpty(request.DataInicio))
                {
                    condicoes.Add("(A.DATA_MAXIMA >= @DATAINICIO)");
                    parametros.Add("@DATAINICIO", request.DataInicio);
                }

                if (!String.IsNullOrEmpty(request.DataFim))
                {
                    condicoes.Add("(A.DATA_MAXIMA <= @DATAFIM)");
                    parametros.Add("@DATAFIM", request.DataFim);
                }

                if (!String.IsNullOrEmpty(request.Equipe))
                {
                    condicoes.Add("(A.EQUIPE = @EQUIPE)");
                    parametros.Add("@EQUIPE", request.Equipe);
                }

                if (!String.IsNullOrEmpty(request.Supervisor))
                {
                    condicoes.Add($"(A.COD_SUPERV = @COD_SUPERV)");
                    parametros.Add("@COD_SUPERV", request.Supervisor);
                }

                if (!String.IsNullOrEmpty(request.Monitor))
                {
                    condicoes.Add($"(A.USUARIO_MONITOR = @ID_MONITOR)");
                    parametros.Add("@ID_MONITOR", request.Monitor);
                }

                condicoes.Add($"(B.EQUIPE_CONNECT = 'S')");


                sqlFrom.AppendLine("FROM(");
                sqlFrom.AppendLine("SELECT A.EQUIPE, B.NOME AS NOME_TECNICO, A.DATA_MAXIMA, C.DISTANCIA_PLANEJADA, COALESCE(C.DISTANCIA_REAL_RASTREAMENTO,C.DISTANCIA_REALIZADA) AS DISTANCIA_REAL,C.MINUTOS_PARALISADO, A.STATUS_EXECUCAO, (SELECT FIRST 1 M.HORAS");
                sqlFrom.AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M");
                sqlFrom.AppendLine("WHERE((M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO,");
                sqlFrom.AppendLine("(SELECT FIRST 1 G.ID")
                    .AppendLine(" FROM TB_HORARIO_EQUIPE_CONNECT F")
                    .AppendLine("LEFT JOIN TB_HORARIO_CONNECT G")
                    .AppendLine("ON(F.ID_HORARIO = G.ID)")
                    .AppendLine("WHERE F.EQUIPE = B.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = A.DATA_MAXIMA) OR")
                    .AppendLine("(F.APENAS_NO_DIA = 0 AND F.DATA <= A.DATA_MAXIMA))")
                    .AppendLine("ORDER BY F.DATA DESC)");
                sqlFrom.AppendLine(" AS ID_HORARIO_CONNECT, ");
                sqlFrom.AppendLine("(SELECT FIRST 1 H.ID");
                sqlFrom.AppendLine("FROM TB_HORARIO_EQUIPE_CONNECT H");
                sqlFrom.AppendLine("LEFT JOIN TB_HORARIO_CONNECT I");
                sqlFrom.AppendLine("ON(H.ID_HORARIO = I.ID)");
                sqlFrom.AppendLine("WHERE H.EQUIPE = B.EQUIPE AND H.APENAS_NO_DIA = 1");
                sqlFrom.AppendLine("AND H.DATA = A.DATA_MAXIMA AND H.INDISPONIVEL = 1)");
                sqlFrom.AppendLine(" AS INDISPONIVEL_CONNECT ");
                sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
                sqlFrom.AppendLine("INNER JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)");
                sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE_QUILOMETRAGEM C ON(A.EQUIPE = C.EQUIPE AND A.DATA_MAXIMA = C.DATA)");
                sqlFrom.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                sqlFrom.AppendLine(")");

                sqlFrom.AppendLine("   GROUP BY EQUIPE,NOME_TECNICO,DATA_MAXIMA,DISTANCIA_PLANEJADA,DISTANCIA_REAL,MINUTOS_PARALISADO,ID_HORARIO_CONNECT,INDISPONIVEL_CONNECT ");
                sqlFrom.AppendLine("   ORDER BY NOME_TECNICO ");
                sql.AppendLine(sqlFrom.ToString());


                using (var com = this._sessionT5.Connection.CreateCommand())
                {
                    using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5.Connection, null).Result)
                    {
                        while (reader.Read())
                        {
                            double kmPlanejada = reader["DISTANCIA_PLANEJADA"] != DBNull.Value ? Convert.ToInt32(reader["DISTANCIA_PLANEJADA"]) : 0;
                            double KmReal = reader["DISTANCIA_REAL"] != DBNull.Value ? Convert.ToInt32(reader["DISTANCIA_REAL"]) : 0;
                            double TempoMotor = (reader["MINUTOS_PARALISADO"] != DBNull.Value ? Convert.ToInt32(reader["MINUTOS_PARALISADO"]) : 0) / 60;
                            Horario_Connect horarioConnect = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? horarios.Find(p => p.Id == Convert.ToInt64(reader["ID_HORARIO_CONNECT"])) : horarios.Find(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico && p.Padrao);
                            DateTime DataAgendamento = Convert.ToDateTime(reader["DATA_MAXIMA"]);
                            var horarioDia = RetornaHorarioDia(horarioConnect, DataAgendamento, false);
                            double horasTrabalhadas = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? 0 : HorasTrabalhadasHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                            double horasIntervalo = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? 0 : HorasIntervaloHorario(horarioDia.inicio_intervalo, horarioDia.termino_intervalo);
                            var equipe = Convert.ToString(reader["EQUIPE"]);
                            var listaDocumentos = repositoryDocumentoProducao.select(p => p.Data_Maxima == DataAgendamento && p.Equipe == Convert.ToString(reader["EQUIPE"])).Result.ToList();
                            var tempoTotalProdutivo = GetTempoAtendimentoDeslocamento(repositoryMongo, listaDocumentos, equipe);
                            var tempoDeslocamento = GetTempoDeslocamento(repositoryMongo, listaDocumentos, equipe);
                            var tempoNovoDeslocamento = GetTempoNovoDeslocamento(repositoryMongo, listaDocumentos, equipe);
                            var tempoAtendimento = GetTempoAtendimento(repositoryMongo, listaDocumentos.FindAll(p => p.Data_Prod != null), equipe);

                            retorno.Add(new PainelPerformanceTecnicoListModel
                            {
                                Matricula = equipe,
                                Tecnico = Convert.ToString(reader["NOME_TECNICO"]),
                                QtdServicosProdutivos = Convert.ToInt32(reader["PRODUTIVAS"]),
                                QtdServicosImprodutivos = Convert.ToInt32(reader["IMPRODUTIVAS"]),
                                QtdServicosAbertos = Convert.ToInt32(reader["ABERTAS"]),
                                DataInicial = request.DataInicio,
                                DataFinal = request.DataFim,
                                TotalKmEsperado = Math.Round(kmPlanejada, 1),
                                TotalKmReal = Math.Round(KmReal, 1),
                                TempoExecucaoPrevisto = reader["TEMPO_EXECUCAO_TOTAL"] != DBNull.Value ? Convert.ToDouble(reader["TEMPO_EXECUCAO_TOTAL"]) : 0,
                                TempoJornadaUtil = Math.Round(horasTrabalhadas, 1),
                                TempoExecucaoReal = Math.Round(tempoAtendimento.TotalHours, 1),
                                TempoJornadaProdutiva = Math.Round(tempoTotalProdutivo.TotalHours, 1),
                                TempoDeslocamentoReal = Math.Round(tempoDeslocamento.TotalHours, 1),
                                TempoDeslocamentoPrevisto = kmPlanejada > 0 && config.TempoBaseDeslocamentoPorKm.HasValue ? Math.Round(kmPlanejada * config.TempoBaseDeslocamentoPorKm.Value.TimeOfDay.TotalHours, 1) : 0,
                                TempoIntervaloJornalada = horasIntervalo,
                                TempoNovoDeslocamento = Math.Round(tempoNovoDeslocamento.TotalHours, 1),
                                TempoTodosServicos = Math.Round(tempoAtendimento.TotalHours, 1),
                                TempoMotorParalisado = TempoMotor
                            });
                        }
                    }
                }

                
                retorno = retorno.GroupBy(p => new { p.Matricula, p.Tecnico }).Select(p =>
                {
                    List<PainelPerformanceTecnicoListModel> dadosEquipe = p.ToList();
                    return new PainelPerformanceTecnicoListModel
                    {
                        Matricula = p.Key.Matricula,
                        DataInicial = request.DataInicio,
                        DataFinal = request.DataFim,
                        Tecnico = p.Key.Tecnico,
                        QtdServicosProdutivos = dadosEquipe.Sum(y => y.QtdServicosProdutivos),
                        QtdServicosImprodutivos = dadosEquipe.Sum(y => y.QtdServicosImprodutivos),
                        QtdServicosAbertos = dadosEquipe.Sum(y => y.QtdServicosAbertos),
                        TotalKmEsperado = Math.Round(dadosEquipe.Sum(y => y.TotalKmEsperado), 1),
                        TotalKmReal = Math.Round(dadosEquipe.Sum(y => y.TotalKmReal), 1),
                        TempoExecucaoPrevisto = Math.Round(dadosEquipe.Sum(y => y.TempoExecucaoPrevisto), 1),
                        TempoJornadaUtil = Math.Round(dadosEquipe.Sum(y => y.TempoJornadaUtil), 1),
                        TempoExecucaoReal = Math.Round(dadosEquipe.Sum(y => y.TempoExecucaoReal), 1),
                        TempoJornadaProdutiva = Math.Round(dadosEquipe.Sum(y => y.TempoJornadaProdutiva), 1),
                        TempoDeslocamentoReal = Math.Round(dadosEquipe.Sum(y => y.TempoDeslocamentoReal), 1),
                        TempoDeslocamentoPrevisto = Math.Round(dadosEquipe.Sum(y => y.TempoDeslocamentoPrevisto), 1),
                        TempoIntervaloJornalada = Math.Round(dadosEquipe.Sum(y => y.TempoIntervaloJornalada), 1),
                        TempoNovoDeslocamento = Math.Round(dadosEquipe.Sum(y => y.TempoNovoDeslocamento)),
                        TempoTodosServicos = Math.Round(dadosEquipe.Sum(y => y.TempoTodosServicos), 1),
                        TempoMotorParalisado = Math.Round(dadosEquipe.Sum(y => y.TempoMotorParalisado), 1)
                    };
                }).ToList();



                return new PainelPerformanceTecnicoResponse
                {
                    Data = retorno
                };
            }
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

        public double HorasTrabalhadasHorario(DateTime? horario_inicio, DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo, DateTime? horario_termino)
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

            return totalMinutos / 60;
        }

        public double HorasIntervaloHorario(DateTime? horario_inicio_intervalo, DateTime? horario_termino_intervalo)
        {
            double totalMinutos = 0;
            if (horario_termino_intervalo.HasValue && horario_termino_intervalo.HasValue)
            {
                totalMinutos += (horario_termino_intervalo.Value - horario_inicio_intervalo.Value).TotalMinutes;
            }

            return totalMinutos / 60;
        }

        public TimeSpan GetTempoAtendimentoDeslocamento(RepositoryMongo<DocumentoProducaoMongo> repositoryMongo, List<Documento_ProducaoAniel> documentos, string equipe)
        {
            TimeSpan retorno = new TimeSpan();
            IMongoCollection<DocumentoProducaoMongo> _collection = repositoryMongo.GetCollection();

            foreach (var documento in documentos)
            {
                DateTime? DataHoraProd = documento.Data_Prod;
                if (DataHoraProd.HasValue && documento.Hora_Prod.HasValue)
                    DataHoraProd = DataHoraProd?.Add(documento.Hora_Prod.Value.TimeOfDay);


                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == documento.CodCt && p.numObra == documento.Num_Obra && p.contrato == documento.Num_Doc && p.projeto == documento.Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).Where(p => p.equipe == equipe && p.hora >= documento.Data_Maxima.Value.Date && p.hora <= Convert.ToDateTime(documento.Data_Maxima.Value.Date.ToString("dd/MM/yyyy 23:59"))).ToList();
                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado) || status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Reiniciado_Atendimento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }


                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.OS_Em_deslocamento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

            }
            return retorno;
        }

        public TimeSpan GetTempoAtendimento(RepositoryMongo<DocumentoProducaoMongo> repositoryMongo, List<Documento_ProducaoAniel> documentos, string equipe)
        {
            TimeSpan retorno = new TimeSpan();
            IMongoCollection<DocumentoProducaoMongo> _collection = repositoryMongo.GetCollection();

            foreach (var documento in documentos)
            {
                DateTime? DataHoraProd = documento.Data_Prod;
                if (DataHoraProd.HasValue && documento.Hora_Prod.HasValue)
                    DataHoraProd = DataHoraProd?.Add(documento.Hora_Prod.Value.TimeOfDay);


                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == documento.CodCt && p.numObra == documento.Num_Obra && p.contrato == documento.Num_Doc && p.projeto == documento.Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).Where(p => p.equipe == equipe && p.hora >= documento.Data_Maxima.Value.Date && p.hora <= Convert.ToDateTime(documento.Data_Maxima.Value.Date.ToString("dd/MM/yyyy 23:59"))).ToList();
                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado) || status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Reiniciado_Atendimento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

            }
            return retorno;
        }

        public TimeSpan GetTempoDeslocamento(RepositoryMongo<DocumentoProducaoMongo> repositoryMongo, List<Documento_ProducaoAniel> documentos, string equipe)
        {
            TimeSpan retorno = new TimeSpan();
            IMongoCollection<DocumentoProducaoMongo> _collection = repositoryMongo.GetCollection();

            foreach (var documento in documentos)
            {
                DateTime? DataHoraProd = documento.Data_Prod;
                if (DataHoraProd.HasValue && documento.Hora_Prod.HasValue)
                    DataHoraProd = DataHoraProd?.Add(documento.Hora_Prod.Value.TimeOfDay);


                var mongoDocumentos = _collection.Aggregate().Match(p => p.codCt == documento.CodCt && p.numObra == documento.Num_Obra && p.contrato == documento.Num_Doc && p.projeto == documento.Projeto).ToList();
                var status = mongoDocumentos.FirstOrDefault()?.status.OrderBy(p => p.hora).Where(p => p.equipe == equipe && p.hora >= documento.Data_Maxima.Value.Date && p.hora <= Convert.ToDateTime(documento.Data_Maxima.Value.Date.ToString("dd/MM/yyyy 23:59"))).ToList();
                if (status == null)
                    return retorno;

                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].statusExecucao == Convert.ToInt32(StatusExecucaoAniel.OS_Em_deslocamento))
                    {
                        if (i + 1 < status.Count)
                        {
                            retorno = retorno.Add(status[i + 1].hora - status[i].hora);
                        }
                        else
                        {
                            retorno = retorno.Add((DataHoraProd ?? DateTime.Now) - status[i].hora);
                        }

                    }
                }

            }
            return retorno;
        }

        public TimeSpan GetTempoNovoDeslocamento(RepositoryMongo<DocumentoProducaoMongo> repositoryMongo, List<Documento_ProducaoAniel> documentos, string equipe)
        {
            TimeSpan retorno = new TimeSpan();
            IMongoCollection<DocumentoProducaoMongo> _collection = repositoryMongo.GetCollection();
            var mongoDocumentos = documentos.OrderBy(p => p.DataHora_Ini_Atend)
                                            .Select(p => _collection.Aggregate().
                                                                     Match(y => y.codCt == p.CodCt && y.numObra == p.Num_Obra && y.contrato == p.Num_Doc && y.projeto == p.Projeto).
                                                                     FirstOrDefault())
                                            .Where(p => p != null)
                                            .ToList();

            var listaStatus = mongoDocumentos.SelectMany(p => p.status).OrderBy(p => p.hora).ToList();
            var listaStatusAtendimentoFinalizado = listaStatus.FindAll(p => p.statusExecucao == Convert.ToInt32(StatusExecucaoAniel.Atendimento_Finalizado));

            foreach (var item in listaStatusAtendimentoFinalizado)
            {
                var novoDeslocamento = listaStatus.Find(p => p.hora > item.hora && p.statusExecucao == Convert.ToInt32(StatusExecucaoAniel.OS_Em_deslocamento));
                if (novoDeslocamento != null)
                    retorno = retorno.Add(novoDeslocamento.hora - item.hora);
            }


            return retorno;
        }
    }
}
