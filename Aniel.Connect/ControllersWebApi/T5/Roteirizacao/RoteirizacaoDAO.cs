using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ControllersWebApi.T5
{
    public class RoteirizacaoDAO : IDisposable
    {

        private ISession sessionT5 = null;
        private ISession sessionT5NH = null;
        private ISession sessionLog = null;
        private ITransaction transactionT5 = null;
        public RoteirizacaoDAO()
        {
            sessionT5 = HibernateUtil.GetSessionT5();
            sessionT5NH = HibernateUtil.GetSessionT5();
            sessionLog = HibernateUtil.GetSessionLog();
        }

        public RoteirizacaoDAO(ISession sessionT5, ISession sessionT5NH)
        {
            this.sessionT5 = sessionT5;
            this.sessionT5NH = sessionT5NH;
            //sessionLog = HibernateUtil.GetSessionLog();
        }

        public void Dispose()
        {
            sessionT5.Dispose();
            sessionT5NH.Dispose();
            sessionLog.Dispose();
        }

        public async Task GravarRoteirizacao(List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados, Configuracoes_Gerais config, String Usuario, RoteirizacaoDiaConf conf, List<TecnicoRoteirizar> tecnicos, bool Simulacao = false)
        {
            if (Simulacao == false)
            {
                documentosRoteirizados.ToList().ForEach(async documento =>
                {
                    try
                    {
                        transactionT5 = sessionT5.BeginTransaction();
                        await GravarAgendamento(documento, config, Usuario);
                        await GravarDocumento(documento, Usuario);
                        transactionT5.Commit();
                    }
                    catch (Exception ex)
                    {

                        transactionT5.Rollback();
                        documentosRoteirizados.Remove(documento);
                        documento.Log = ex.Message;
                        documentosNaoRoteirizados.Add(documento);
                    }
                });
            }

            await GravarLog(documentosValidos, documentosRoteirizados, documentosNaoRoteirizados, Usuario, conf, config, tecnicos, Simulacao);


        }

        public async Task<List<DocumentoRoteirizar>> GetDocumentoRoteirizacaoSql(List<DocumentoProducao_GetDocumento> documentos, string equipe, string data, Configuracoes_Gerais config)
        {

            Boolean Exibe_RDOs = config.ExibeRDOs;
            TimeSpan? Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao?.TimeOfDay;
            List<DocumentoRoteirizar> retorno = new List<DocumentoRoteirizar>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = sessionT5.Connection.CreateCommand())
            using (var repoTipoImovel = new Conexao<Tipo_ImovelAniel>(sessionT5NH))
            {

                StringBuilder sql = new StringBuilder("SELECT A.NUM_OBRA,A.PROJETO,A.CODCT, A.NUM_OBRA_ORIGINAL,A.NUM_DOC, L.ID AS ID_NATUREZA, A.DATA_MAXIMA,A.HORA_MAXIMA,A.EQUIPE,O.DEVICE_ID,O.NOME AS NOME_EQUIPE,A.LATITUDE_STR,A.LONGITUDE_STR, ")
                .AppendLine("A.DATAHORA_AGENDAMENTO AS DATAHORA_AGENDAMENTO_OS, A.DATA_PROD AS DATAHORA_ENCERRAMENTO_OS,A.DATA_INDICADOR,A.HORA_INDICADOR, ")
                .AppendLine("A.COD_ESCRITORIO, C.DESCRICAO AS DESCRICAO_ESCRITORIO, A.COD_LOCALIDADE,")
                .AppendLine("A.COD_AREA, D.DESCRICAO AS DESCRICAO_AREA, E.LOCALIDADE, P.DESCRICAO AS DESCRICAO_TIPO_SERV,A.STATUS_EXECUCAO, ")
                .AppendLine(" H.COD_CLIENTE,H.RAZAO_SOCIAL, R.LATITUDE AS CX_LATITUDE, R.LONGITUDE AS CX_LONGITUDE,")
                .AppendLine("A.PARPRIM_PARA AS COD_TIPO_IMOVEL, L.DESCRICAO AS NATUREZA_OS, A.PARPRIM_PARA AS ID_TIPO_IMOVEL, A.COD_TIPO_SERV, A.DIA + A.HORA AS DATA_HORA_CRIACAO, A.DATA_PROD + A.HORA_PROD  AS DATA_HORA_ENCERRAMENTO,A.COD_TURNO_SERV,Q.HORA_INI,Q.HORA_FIM,E.LATITUDE AS LATITUDE_LOCALIDADE,E.LONGITUDE AS LONGITUDE_LOCALIDADE,L.DESCRICAO AS DESC_NATUREZA,")


                //PEGA TEMPO DE EXECUÇÃO INDIFERENTE OU

                .AppendLine("(SELECT FIRST 1 N.ID")
                .AppendLine("FROM TB_AGENDAMENTO_EQUIPEANIEL N")
                .AppendLine("WHERE(A.NUM_OBRA = N.NUM_OBRA AND A.PROJETO = N.PROJETO AND A.CODCT = N.CODCT AND A.NUM_DOC = N.NUM_DOC)) AS ID_AGENDAMENTO,")
                .AppendLine("(SELECT FIRST 1 N.ORIGEM")
                .AppendLine("FROM TB_AGENDAMENTO_EQUIPEANIEL N")
                .AppendLine("WHERE(A.NUM_OBRA = N.NUM_OBRA AND A.PROJETO = N.PROJETO AND A.CODCT = N.CODCT AND A.NUM_DOC = N.NUM_DOC)) AS ORIGEM_AGENDAMENTO,")
                .AppendLine("(SELECT FIRST 1 M.HORAS ")
                .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")

                .AppendLine("   FROM TB_DOCUMENTO_PRODUCAO A ")
                .AppendLine("LEFT JOIN TB_ESCRITORIO C ")
                .AppendLine("ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO) ")
                .AppendLine("LEFT JOIN TB_AREA D ")
                .AppendLine("ON(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ")
                .AppendLine("LEFT JOIN TB_LOCALIDADE E ")
                .AppendLine("ON(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ")
                .AppendLine("A.COD_LOCALIDADE = E.COD_LOCALIDADE) ")
                .AppendLine("LEFT JOIN TB_CLIENTE H ") // ID CLIENTE
                .AppendLine("ON(A.ID_CLIENTE = H.COD_CLIENTE) ")
                .AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE P")
                .AppendLine("ON(A.COD_TIPO_SERV = P.COD_TIPO_SERV)")
                .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO L")
                .AppendLine("ON(L.ID = P.COD_NATUREZA) ")
                .AppendLine("LEFT JOIN TB_EQUIPE O")
                .AppendLine("ON(A.EQUIPE = O.EQUIPE) ")
                .AppendLine("LEFT JOIN TB_TURNO_SERVICO Q")
                .AppendLine("ON(A.COD_TURNO_SERV = Q.ID) ")
                .AppendLine("LEFT JOIN TB_ELEMENTO_REDE R")
                .AppendLine("ON(CAST(R.ID  AS VARCHAR(255)) = Cast(A.CAIXA_FTTH AS VARCHAR(255))) ")




                .AppendLine($"  WHERE  ");

                if (String.IsNullOrEmpty(equipe) == false)
                {
                    sql.AppendLine(" (A.EQUIPE = @EQUIPE) ");
                    parametros.Add("@EQUIPE", equipe);

                    if (String.IsNullOrEmpty(data) == false)
                    {
                        sql.AppendLine(" AND (A.DATA_MAXIMA = @DATA) ");
                        parametros.Add("@DATA", data);
                    }
                }

                for (int i = 0; i < documentos.Count; i++)
                {
                    sql.AppendLine($" (A.CODCT = '{documentos[i].CodCt}' AND A.PROJETO = '{documentos[i].Projeto}' AND A.NUM_OBRA = '{documentos[i].Num_Obra}' AND A.NUM_DOC = {documentos[i].Num_Doc})  ");
                    if ((i + 1) < documentos.Count)
                        sql.AppendLine(" OR ");
                }

                if (!Exibe_RDOs)
                    sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");



                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var datahora_indicador = reader["DATA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INDICADOR"]) : (DateTime?)null;

                        if (datahora_indicador.HasValue && reader["HORA_INDICADOR"] != DBNull.Value)
                        {
                            datahora_indicador = datahora_indicador.Value.Add((TimeSpan)reader["HORA_INDICADOR"]);
                        }

                        var data_hora_criacao = reader["DATA_HORA_CRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_CRIACAO"]) : (DateTime?)null;
                        var data_hora_encerramento = reader["DATA_HORA_ENCERRAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_ENCERRAMENTO"]) : (DateTime?)null;
                        var tipoImovel = reader["COD_TIPO_IMOVEL"] != DBNull.Value ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == Convert.ToInt32(reader["COD_TIPO_IMOVEL"])).Result?.Cod_Tipo_Imovel : (Int32?)null;

                        retorno.Add(new DocumentoRoteirizar
                        {
                            Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                            CodCt = Convert.ToString(reader["CODCT"]),
                            Projeto = Convert.ToString(reader["PROJETO"]),
                            Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null,
                            Id_Natureza = reader["ID_NATUREZA"] != DBNull.Value ? Convert.ToInt64(reader["ID_NATUREZA"]) : (Int64?)null,
                            DescNatureza = Convert.ToString(reader["DESC_NATUREZA"]),
                            Grupo_Area = Convert.ToString(reader["COD_ESCRITORIO"]),
                            Data_Maxima = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null,
                            Hora_Maxima = reader["HORA_MAXIMA"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)reader["HORA_MAXIMA"]) : (DateTime?)null,
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            OrigemAgendamento = Convert.ToString(reader["ORIGEM_AGENDAMENTO"]),
                            Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                            Area = Convert.ToString(reader["COD_AREA"]),
                            Latitude_Str = reader["LATITUDE_STR"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_STR"]) : null,
                            Longitude_Str = reader["LONGITUDE_STR"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_STR"]) : null,
                            Latitude_Cx = reader["CX_LATITUDE"] != DBNull.Value ? Convert.ToString(reader["CX_LATITUDE"]) : null,
                            Longitude_Cx = reader["CX_LONGITUDE"] != DBNull.Value ? Convert.ToString(reader["CX_LONGITUDE"]) : null,
                            Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                            Localidade = Convert.ToString(reader["LOCALIDADE"]),
                            //Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                            //Monitor = Convert.ToString(reader["MONITOR"]),
                            Tempo_Execucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan)(reader["TEMPO_EXECUCAO"]) : Tempo_Execuxao_Padrao,
                            Longitude_Localidade = reader["LONGITUDE_LOCALIDADE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE_LOCALIDADE"]) : (Double?)null,
                            Latitude_Localidade = reader["LATITUDE_LOCALIDADE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE_LOCALIDADE"]) : (Double?)null,
                            SLA = datahora_indicador,
                            Data_hora_Criacao = data_hora_criacao,
                            Data_hora_Encerramento = data_hora_encerramento,
                            Data_Hora_Vencimento_OS = datahora_indicador,
                            Cod_Tipo_Imovel = tipoImovel.HasValue ? tipoImovel.Value : (Int32?)null,
                            Cod_Tipo_Serv = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt16(reader["COD_TIPO_SERV"]) : (Int16?)null,
                            Cod_Cliente = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["COD_CLIENTE"]) : (Int64?)null,
                            Nome_Cliente = Convert.ToString(reader["RAZAO_SOCIAL"]),
                            Id_Agendamento = reader["ID_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_AGENDAMENTO"]) : (Int64?)null,
                            Equipe_Anterior = Convert.ToString(reader["EQUIPE"]),
                            Descricao_Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                            Descricao_Grupo_Area = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                            Descricao_Tipo_Serv = Convert.ToString(reader["DESCRICAO_TIPO_SERV"]),
                            StatusOld = reader["STATUS_EXECUCAO"] != DBNull.Value ? (StatusExecucaoAniel?)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])) : (StatusExecucaoAniel?)null,
                            Tecnico_Anterior = new TecnicoRoteirizar
                            {
                                Equipe = Convert.ToString(reader["EQUIPE"]),
                                Device_Id = Convert.ToString(reader["DEVICE_ID"]),
                                Nome = Convert.ToString(reader["NOME_EQUIPE"]),
                            },
                            CodTurnoServ = reader["COD_TURNO_SERV"] != DBNull.Value ? Convert.ToInt32(reader["COD_TURNO_SERV"]) : (int?)null,
                            TurnoHoraIni = reader["HORA_INI"] != DBNull.Value ? DateTime.Today.Add(((TimeSpan)reader["HORA_INI"])) : (DateTime?)null,
                            TurnoHoraFim = reader["HORA_FIM"] != DBNull.Value ? DateTime.Today.Add(((TimeSpan)reader["HORA_FIM"])) : (DateTime?)null
                        });
                    }
                }

                return retorno;


            }
        }
        public async Task<List<DocumentoRoteirizar>> GetDocumentoRoteirizacaoProgramada(ProgramacaoRoteirizacaoAutomatica programacao)
        {
            using (var repo = new Conexao<Documento_ProducaoAniel>(sessionT5NH))
            {
                var consulta = repo.QueryableSelect();
                if (programacao.Servico_Agendados.GetValueOrDefault(false))
                {
                    if (programacao.NoDia.GetValueOrDefault(false))
                    {
                        consulta.Where(p => p.Data_Maxima == DateTime.Now);
                    }
                }


                if (programacao.Tecnicos.Count > 0)
                {
                    consulta.Where(p => programacao.Tecnicos.Select(y => y.Equipe).Contains(p.Equipe));
                }

                return consulta.ToList().Select(p => new DocumentoRoteirizar
                {
                    CodCt = p.CodCt,
                    Projeto = p.Projeto,
                    Num_Doc = p.Num_Doc,
                    Num_Obra = p.Num_Obra,
                    Num_Obra_Original = p.Numero_Obra_Original,
                    Latitude_Str = p.Latitude_Str,
                    Longitude_Str = p.Longitude_Str,
                    StatusOld = p.Status,
                    Area = p.Cod_Area,
                    Grupo_Area = p.Cod_Escritorio,
                    Cod_Localidade = p.Cod_Localidade
                }).ToList();

            }
        }
        public async Task<List<TecnicoRoteirizar>> GetTecnicoRoteirizacaoSql(Configuracoes_Gerais config, RoteirizacaoDiaConf diaConf, String equipe)
        {
            List<TecnicoRoteirizar> retorno = new List<TecnicoRoteirizar>();
            Horario_Connect horario_servico_padrao = diaConf.horarios_servico.Find(p => p.Padrao == true);
            List<PerfilAgendamento> perfilAgendamentoCache = new List<PerfilAgendamento>();
            //Select

            using (var repository = new Conexao<PerfilAgendamento>(sessionT5NH))
            using (var repositoryArea = new Conexao<PerfilAgendamentoArea>(sessionT5NH))
            using (var repositoryNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var com = sessionT5.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();
                List<PerfilAgendamento> perfis = (await repository.select()).ToList();


                sql = new StringBuilder(@"SELECT B.EQUIPE, B.NOME,B.DEVICE_ID, H.LATITUDE, H.LONFITUDE,SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR, SUPERV.NOME AS NOME_SUPERVISOR,B.USUARIO_MONITOR,FUNCIONARIO_MONITOR.NOME NOME_MONITOR,
                                                        B.ID_PERFIL_AGENDAMENTO,F.HORARIO_ATRIBUIDO,H.DESCRICAO AS PONTO_PARTIDA, B.COD_FORNECEDOR,J.LATITUDE AS LATITUDE_ATUAL,J.LONGITUDE AS LONGITUDE_ATUAL,J.DATAHORACRIACAO AS DATA_EQUIPE_STATUS, ");

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


                if (diaConf.Data.Date == DateTime.Today)
                    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");

                if (String.IsNullOrEmpty(equipe) == false)
                {
                    sql.AppendLine("AND (B.EQUIPE = @EQUIPE)");
                    parametros.Add("@EQUIPE", equipe);
                }



                parametros.Add("@DATA", diaConf.Data.Date);

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        //decido no dia 11/11/25 que tecnicos sem ponto de partida deveriam ser excluidos da rotina de roteirização
                        if (reader["LATITUDE"] == DBNull.Value || reader["LONFITUDE"] == DBNull.Value) 
                        {
                            continue;
                        }

                        var dataEquipeStatus = reader["DATA_EQUIPE_STATUS"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EQUIPE_STATUS"]) : (DateTime?)null;
                        long? idHorarioEquipe = null;
                        long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                        HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ?
                                                                                                            (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"]))
                                                                                                        :
                                                                                                            (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);

                        if (reader["ID_HORARIO_EQUIPE"] != DBNull.Value)
                        {
                            Horario_Equipe horario = await repositoryHorarioEquipe.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_HORARIO_EQUIPE"]));
                            idPerfil = horario?.PerfilAgendamento?.Id ?? idPerfil;
                        }

                        if (horarioAtribuido == HorarioAtribuido.AnielPoint)
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null;
                            var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                            if (String.IsNullOrEmpty(indisponivelPoint) == false || idHorarioEquipe.HasValue == false)
                                continue;

                        }
                        else
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null;
                            var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;
                            if (indisponivelConnect)
                                continue;
                        }

                        PerfilAgendamento perfil = idPerfil != null ? perfis.FirstOrDefault(p => p.Id == idPerfil) : null;
                        List<LocalidadeRoteirizacao> localidades = perfil == null ? new List<LocalidadeRoteirizacao>() : perfil.Areas.Select(p =>
                        {
                            return new LocalidadeRoteirizacao
                            {
                                Cod_Escritorio = p.Escritorio,
                                Cod_Area = p.Area,
                                LocalidadeStr = p.Localidade,
                                Todas_Localidades = p.Todas_Localidades,
                                Nivel = p.Nivel,
                                Todas_Areas = p.Todas_Areas
                            };

                        }).ToList();

                        List<Int64?> naturezas = perfil == null ? new List<Int64?>() : perfil.Naturezas.Select(p => (Int64?)p?.Natureza?.Id).ToList();


                        retorno.Add(new TecnicoRoteirizar
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Nome = Convert.ToString(reader["NOME"]),
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null,
                            Longitude = reader["LONFITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONFITUDE"]) : (Double?)null,
                            LatitudeAtual = dataEquipeStatus.HasValue && dataEquipeStatus.Value.Date == DateTime.Today ? (reader["LATITUDE_ATUAL"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE_ATUAL"]) : (Double?)null) : (Double?)null,
                            LongitudeAtual = dataEquipeStatus.HasValue && dataEquipeStatus.Value.Date == DateTime.Today ? (reader["LONGITUDE_ATUAL"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE_ATUAL"]) : (Double?)null) : (Double?)null,
                            PontoPartida = Convert.ToString(reader["PONTO_PARTIDA"]),
                            HorarioConnect = diaConf.horarios_servico.FindAll(p => p.Id == idHorarioEquipe).FirstOrDefault() ?? horario_servico_padrao,
                            Naturezas = naturezas,
                            Localidades = localidades,
                            JaAgendados = DocumentosJaAgendados(config, diaConf, Convert.ToString(reader["EQUIPE"])),
                            Device_Id = Convert.ToString(reader["DEVICE_ID"]),
                            NumFre_Supervisor = Convert.ToString(reader["NUM_FRE_SUPERVISOR"]),
                            Nome_Supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]),
                            Usuario_Monitor = Convert.ToString(reader["USUARIO_MONITOR"]),
                            Nome_Monitor = Convert.ToString(reader["NOME_MONITOR"]),
                            Cod_Fornecedor = reader["COD_FORNECEDOR"] != DBNull.Value ? Convert.ToString(reader["COD_FORNECEDOR"]) : ""

                        });
                    }
                }

                return retorno;
            }
        }

        public async Task<List<TecnicoRoteirizar>> GetTecnicoPlanejamentoRoteirizacaoSql(Configuracoes_Gerais config, RoteirizacaoDiaConf diaConf, GestaoCapacidadeRoteirizacaoRequest request)
        {
            List<TecnicoRoteirizar> retorno = new List<TecnicoRoteirizar>();
            Horario_Connect horario_servico_padrao = diaConf.horarios_servico.Find(p => p.Padrao == true);
            List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)> perfilAgendamentoCache = new List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)>();
            List<Horario_Connect> horarioConnectCache = new List<Horario_Connect>();
            //Select

            using (var com = sessionT5.Connection.CreateCommand())
            using (var repository = new Conexao<PerfilAgendamento>(sessionT5NH))
            using (var repositoryEscritorioAniel = new Conexao<EscritorioAniel>(sessionT5NH))
            using (var repositoryAreaAniel = new Conexao<AreaAniel>(sessionT5NH))
            using (var repositoryArea = new Conexao<PerfilAgendamentoArea>(sessionT5NH))
            using (var repositoryNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryHorarioConnect = new Conexao<Horario_Connect>(sessionT5NH))
            using (var horarioApi = new HorarioApiController())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();



                if (config.UtilizaPoint_RoteirizacaoAgendamento)
                {
                    sql = new StringBuilder(@"SELECT  
                                            B.EQUIPE,
                                            B.NOME,
                                            B.DEVICE_ID,
                                            H.LATITUDE,
                                            H.LONFITUDE,
                                            SUPERV.NUM_FRE AS NUM_FRE_SUPERVISOR,
                                            SUPERV.NOME AS NOME_SUPERVISOR,
                                            B.USUARIO_MONITOR,
                                            FUNCIONARIO_MONITOR.NOME AS NOME_MONITOR,
                                            B.ID_PERFIL_AGENDAMENTO,
                                            F.HORARIO_ATRIBUIDO,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 F.ID_PERFIL_AGENDAMENTO
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_PERFIL_AGENDAMENTO_HORARIO,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 F.ID
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_HORARIO_EQUIPE,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 G.ID
                                                    FROM TB_HORARIO_EQUIPE_CONNECT F
                                                    LEFT JOIN TB_HORARIO_CONNECT G ON F.ID_HORARIO = G.ID
                                                    WHERE F.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (F.APENAS_NO_DIA = 1 AND F.DATA = @DATA) OR
                                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)
                                                      )
                                                    ORDER BY F.DATA DESC
                                                )
                                            END) AS ID_HORARIO_CONNECT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 COALESCE(B.ID_HORARIO_CONNECT, D.ID_HORARIO_CONNECT) 
                                                    FROM TB_PT_DIA_FUNCIONARIO A
                                                    LEFT JOIN TB_PT_JORNADA_FUNCIONARIO B ON A.ID_JORNADA = B.ID
                                                    LEFT JOIN TB_FUNCIONARIO C ON C.NUM_FRE = A.NUM_FRE
                                                    LEFT JOIN TB_PT_JORNADA_FUNCIONARIO D ON C.ID_JORNADA = D.ID
                                                    WHERE A.NUM_FRE = F.NUM_FRE 
                                                      AND A.DATA = @DATA
                                                )
                                            END) AS ID_HORARIO_CONNECT_POINT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.INDISPONIVEL 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 H.INDISPONIVEL
                                                    FROM TB_HORARIO_EQUIPE_CONNECT H
                                                    LEFT JOIN TB_HORARIO_CONNECT I ON H.ID_HORARIO = I.ID
                                                    WHERE H.EQUIPE = B.EQUIPE 
                                                      AND H.APENAS_NO_DIA = 1
                                                      AND H.DATA = @DATA
                                                      AND H.INDISPONIVEL = 1
                                                )
                                            END) AS INDISPONIVEL_CONNECT,

    
                                            (CASE 
                                                WHEN (
                                                    SELECT FIRST 1 X.ID_PERFIL_AGENDAMENTO 
                                                    FROM TB_HORARIO_EQUIPE_CONNECT X
                                                    WHERE X.EQUIPE = B.EQUIPE 
                                                      AND (
                                                          (X.APENAS_NO_DIA = 1 AND X.DATA = @DATA) OR
                                                          (X.APENAS_NO_DIA = 0 AND X.DATA <= @DATA)
                                                      )
                                                ) IS NULL 
                                                THEN NULL 
                                                ELSE (
                                                    SELECT FIRST 1 A.NUM_FRE
                                                    FROM TB_FUNCIONARIO A
                                                    INNER JOIN TB_PT_DIA_FUNCIONARIO B ON A.NUM_FRE = B.NUM_FRE
                                                    LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA C ON B.ID_EVENTO = C.ID
                                                    WHERE C.PERMTITE_ROTEIRIZAR = 0
                                                      AND A.NUM_FRE = F.NUM_FRE 
                                                      AND B.DATA = @DATA
                                                )
                                            END) AS INDISPONIVEL_POINT

                                        FROM TB_EQUIPE B
                                        INNER JOIN TB_FUNCIONARIO F ON B.NUM_FRE = F.NUM_FRE
                                        LEFT JOIN TB_PONTO_PARTIDA H ON B.ID_PONTO_PARTIDA = H.ID
                                        LEFT JOIN TB_EQUIPE_STATUS J ON B.EQUIPE = J.EQUIPE
                                        LEFT JOIN TB_FUNCIONARIO SUPERV ON SUPERV.NUM_FRE = B.COD_SUPERV
                                        LEFT JOIN TB_SENHA USUARIO_MONITOR ON USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR
                                        LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR ON FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE

                                        WHERE 
                                            (F.SITUACAO = 0 OR F.SITUACAO = 4)
                                            AND B.EQUIPE_CONNECT = 'S'
                                        ");
                }
                else
                {

                    sql = new StringBuilder(@"SELECT
                                                B.EQUIPE,
                                                B.NOME,
                                                B.DEVICE_ID,
                                                H.LATITUDE,
                                                H.LONFITUDE,
                                                SUPERV.NUM_FRE           AS NUM_FRE_SUPERVISOR,
                                                SUPERV.NOME              AS NOME_SUPERVISOR,
                                                B.USUARIO_MONITOR,
                                                FUNCIONARIO_MONITOR.NOME AS NOME_MONITOR,
                                                B.ID_PERFIL_AGENDAMENTO,
                                                F.HORARIO_ATRIBUIDO,

                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID_PERFIL_AGENDAMENTO END AS ID_PERFIL_AGENDAMENTO_HORARIO,
                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID                    END AS ID_HORARIO_EQUIPE,
                                                CASE WHEN HR.EQUIPE IS NULL THEN NULL ELSE HR.ID_HORARIO    END AS ID_HORARIO_CONNECT,

                                                CASE
                                                    WHEN HR.EQUIPE IS NULL            THEN NULL
                                                    WHEN HR.APENAS_NO_DIA = 1
                                                     AND HR.DATA          = @DATA
                                                     AND HR.INDISPONIVEL  = 1         THEN HR.INDISPONIVEL
                                                    ELSE NULL
                                                END AS INDISPONIVEL_CONNECT
                                            FROM TB_EQUIPE B
                                            INNER JOIN TB_FUNCIONARIO F
                                                ON B.NUM_FRE = F.NUM_FRE
                                               AND (F.SITUACAO = 0 OR F.SITUACAO = 4)
                                            LEFT JOIN TB_HORARIO_EQUIPE_CONNECT HR
                                                ON HR.EQUIPE = B.EQUIPE and hr.data = @DATA
                                            LEFT JOIN TB_PONTO_PARTIDA H
                                                ON B.ID_PONTO_PARTIDA = H.ID
                                            LEFT JOIN TB_EQUIPE_STATUS J
                                                ON B.EQUIPE = J.EQUIPE
                                            LEFT JOIN TB_FUNCIONARIO SUPERV
                                                ON SUPERV.NUM_FRE = B.COD_SUPERV
                                            LEFT JOIN TB_SENHA USUARIO_MONITOR
                                                ON USUARIO_MONITOR.USUARIO = B.USUARIO_MONITOR
                                            LEFT JOIN TB_FUNCIONARIO FUNCIONARIO_MONITOR
                                                ON FUNCIONARIO_MONITOR.NUM_FRE = USUARIO_MONITOR.NUM_FRE
                                            WHERE B.EQUIPE_CONNECT = 'S'");
                }


                if (diaConf.Data.Date == DateTime.Today)
                    sql.AppendLine("AND (COALESCE(J.TIPO, 0) <> 1 OR (COALESCE(J.TIPO, 0) = 1 AND CAST(J.DATAHORACRIACAO AS DATE) <> @DATA))");

                if (String.IsNullOrEmpty(request.Supervisor) == false)
                {
                    sql.AppendLine("AND SUPERV.NUM_FRE = @SUPERV");
                    parametros.Add("@SUPERV", request.Supervisor);
                }

                if (String.IsNullOrEmpty(request.Monitor) == false)
                {
                    sql.AppendLine("AND B.USUARIO_MONITOR = @MONITOR");
                    parametros.Add("@MONITOR", request.Monitor);
                }



                parametros.Add("@DATA", diaConf.Data.Date);

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {

                        long? idHorarioEquipe = null;
                        long? idPerfil = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (Int64?)null;
                        HorarioAtribuido horarioAtribuido = reader["HORARIO_ATRIBUIDO"] != DBNull.Value ?
                                                                                                            (HorarioAtribuido)Enum.Parse(typeof(HorarioAtribuido), Convert.ToString(reader["HORARIO_ATRIBUIDO"]))
                                                                                                        :
                                                                                                            (config.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect);

                        double HorasTrabalhadasHorario = 0;
                        bool escalado = false;
                        if (reader["ID_PERFIL_AGENDAMENTO_HORARIO"] != DBNull.Value)
                        {
                            idPerfil = Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO_HORARIO"]);
                        }

                        if (config.UtilizaPoint_RoteirizacaoAgendamento && horarioAtribuido == HorarioAtribuido.AnielPoint)
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT_POINT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT_POINT"]) : (Int64?)null;
                            var indisponivelPoint = reader["INDISPONIVEL_POINT"] != DBNull.Value ? Convert.ToString(reader["INDISPONIVEL_POINT"]) : null;
                            if (String.IsNullOrEmpty(indisponivelPoint) || idHorarioEquipe.HasValue == false)
                            {
                                escalado = false;
                            }
                            else
                            {
                                var cacheHorario = horarioConnectCache.Find(p => p.Id == idHorarioEquipe);
                                Horario_Connect horarioConnect = null;
                                if (cacheHorario == null)
                                {
                                    horarioConnect = repositoryHorarioConnect.selectFirst(p => p.Id == idHorarioEquipe).Result;
                                    horarioConnectCache.Add(horarioConnect);
                                }
                                else
                                {
                                    horarioConnect = cacheHorario;
                                }

                                var horarioDia = horarioApi.RetornaHorarioDia(horarioConnect, diaConf.Data.Date, false);
                                if (horarioDia.inicio != null)
                                {
                                    escalado = true;
                                    HorasTrabalhadasHorario = horarioApi.MinutosTrabalhadosHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                                }

                            }


                        }
                        else
                        {
                            idHorarioEquipe = reader["ID_HORARIO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO_CONNECT"]) : (Int64?)null;
                            var indisponivelConnect = reader["INDISPONIVEL_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["INDISPONIVEL_CONNECT"]) : false;

                            if (idHorarioEquipe.HasValue == false)
                            {
                                idHorarioEquipe = diaConf.horarios_servico.Where(p => p.Padrao == true && p.Tipo == Tipo_Hora_Connect.Horario_Servico).FirstOrDefault()?.Id;
                            }

                            if (indisponivelConnect)
                            {
                                escalado = false;
                            }
                            else
                            {
                                var cacheHorario = horarioConnectCache.Find(p => p.Id == idHorarioEquipe);
                                Horario_Connect horarioConnect = null;
                                if (cacheHorario == null)
                                {
                                    horarioConnect = repositoryHorarioConnect.selectFirst(p => p.Id == idHorarioEquipe).Result;
                                    horarioConnectCache.Add(horarioConnect);
                                }
                                else
                                {
                                    horarioConnect = cacheHorario;
                                }

                                var horarioDia = horarioApi.RetornaHorarioDia(horarioConnect, diaConf.Data.Date, false);
                                if (horarioDia.inicio != null)
                                {
                                    escalado = true;
                                    HorasTrabalhadasHorario = horarioApi.MinutosTrabalhadosHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                                }

                            }

                        }

                        var cache = perfilAgendamentoCache.Find(p => p.perfil == idPerfil);
                        List<LocalidadeRoteirizacao> localidades = new List<LocalidadeRoteirizacao>();
                        List<Natureza_ProducaoAniel> naturezas = new List<Natureza_ProducaoAniel>();
                        if (cache.perfil == null)
                        {
                            localidades = idPerfil == null ? new List<LocalidadeRoteirizacao>() : GetLocalidadeRoteirizacaoByPerfil(idPerfil.Value);
                            naturezas = idPerfil == null ? new List<Natureza_ProducaoAniel>() : (await repositoryNatureza.select(p => p.PerfilAgendamento.Id == idPerfil)).Select(p => p?.Natureza).ToList();
                            perfilAgendamentoCache.Add((perfil: idPerfil, localidades: localidades, naturezas: naturezas));

                        }
                        else
                        {
                            localidades = cache.localidades;
                            naturezas = cache.naturezas;
                        }

                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            if (localidades.Find(p => p.Cod_Escritorio == request.GrupoArea) == null)
                                continue;
                            else
                                localidades = localidades.FindAll(p => p.Cod_Escritorio == request.GrupoArea);

                            if (String.IsNullOrEmpty(request.Area) == false)
                            {
                                if (localidades.Find(p => p.Cod_Escritorio == request.GrupoArea && (p.Cod_Area == request.Area || p.Todas_Areas)) == null)
                                    continue;
                                else
                                    localidades = localidades.FindAll(p => p.Cod_Escritorio == request.GrupoArea && (p.Cod_Area == request.Area || p.Todas_Areas));

                                if (String.IsNullOrEmpty(request.Localidade) == false)
                                {
                                    if (localidades.Find(p => p.Cod_Escritorio == request.GrupoArea && (p.Cod_Area == request.Area || p.Todas_Areas) && (p.LocalidadeStr == request.Localidade || p.Todas_Localidades)) == null)
                                        continue;
                                    else
                                        localidades = localidades.FindAll(p => p.Cod_Escritorio == request.GrupoArea && (p.Cod_Area == request.Area || p.Todas_Areas) && (p.LocalidadeStr == request.Localidade || p.Todas_Localidades));
                                }
                            }
                        }

                        if (String.IsNullOrEmpty(request.Natureza) == false)
                        {
                            if (naturezas.Find(p => p.Id == Convert.ToInt64(request.Natureza)) == null)
                                continue;
                            else
                                naturezas = naturezas.FindAll(p => p.Id == Convert.ToInt64(request.Natureza));
                        }

                        TipoHabilidade? tipoHabilidade = GetTipoHabilidade(naturezas);

                        retorno.Add(new TecnicoRoteirizar
                        {
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Nome = Convert.ToString(reader["NOME"]),
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null,
                            Longitude = reader["LONFITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONFITUDE"]) : (Double?)null,
                            HorarioConnect = diaConf.horarios_servico.FindAll(p => p.Id == idHorarioEquipe).FirstOrDefault() ?? horario_servico_padrao,
                            Naturezas = naturezas.Select(p => (long?)p.Id).ToList(),
                            TipoHabilidade = tipoHabilidade,
                            Escalado = escalado,
                            Localidades = localidades,
                            HorasTrabalhadasHorario = HorasTrabalhadasHorario,
                            //JaAgendados = DocumentosJaAgendados(config, diaConf, Convert.ToString(reader["EQUIPE"])),
                            Device_Id = Convert.ToString(reader["DEVICE_ID"]),
                            NumFre_Supervisor = Convert.ToString(reader["NUM_FRE_SUPERVISOR"]),
                            Nome_Supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]),
                            Usuario_Monitor = Convert.ToString(reader["USUARIO_MONITOR"]),
                            Nome_Monitor = Convert.ToString(reader["NOME_MONITOR"]),
                            IdPerfilAgendamento = idPerfil,

                        });
                    }

                }

                return retorno;
            }
        }


        private TipoHabilidade? GetTipoHabilidade(List<Natureza_ProducaoAniel> naturezasProducao)
        {
            TipoHabilidade? tipoHabilidade = null;
            if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Instalacao).Any() && naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Reparo).Any())
            {
                tipoHabilidade = TipoHabilidade.Multi;
            }
            else if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Instalacao).Any())
            {
                tipoHabilidade = TipoHabilidade.Instalacao;
            }
            else if (naturezasProducao.Where(p => p.TipoHabilidade == TipoHabilidade.Reparo).Any())
            {
                tipoHabilidade = TipoHabilidade.Reparo;
            }

            return tipoHabilidade;
        }

        private List<Painel_Servicos_AgendamentosEquipe> DocumentosJaAgendados(Configuracoes_Gerais config, RoteirizacaoDiaConf diaConf, String Equipe)
        {
            List<Painel_Servicos_AgendamentosEquipe> JaAgendados = new List<Painel_Servicos_AgendamentosEquipe>();
            TimeSpan? Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao?.TimeOfDay;

            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                if (diaConf.Alterar_DataHora_SemAgendamentoAniel)
                {
                    sql.AppendLine("SELECT A.ID,A.EQUIPE, A.DATAHORA_INICIO, A.DATAHORA_FIM,A.DESLOCAMENTO, A.LATITUDE, A.LONGITUDE,A.CODCT,A.PROJETO,A.NUM_OBRA,A.NUM_DOC,NULL AS TEMPO_EXECUCAO,B.COD_AREA,B.COD_ESCRITORIO,B.COD_LOCALIDADE ")
                    .AppendLine("   FROM TB_AGENDAMENTO_EQUIPEANIEL A ")
                    .AppendLine("   INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC) ")
                    .AppendLine("  WHERE (A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM) ")
                    .AppendLine("  AND B.COD_ENCERRAMENTO IS NULL AND A.EQUIPE = @EQUIPE ");
                }

                else
                {
                    sql.AppendLine("SELECT 0 AS ID,A.EQUIPE, A.DATA_MAXIMA + A.HORA_MAXIMA AS DATAHORA_INICIO,NULL AS DATAHORA_FIM, 0 AS DESLOCAMENTO, A.LATITUDE, A.LONGITUDE,A.CODCT,A.PROJETO,A.NUM_OBRA,A.NUM_DOC,A.COD_AREA,A.COD_ESCRITORIO,A.COD_LOCALIDADE, ")
                    .AppendLine("(SELECT FIRST 1 M.HORAS ")
                    .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                    .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")

                    .AppendLine("   FROM TB_DOCUMENTO_PRODUCAO A ")
                    .AppendLine("  WHERE ((A.DATA_MAXIMA + A.HORA_MAXIMA) BETWEEN @DATA_INICIO AND @DATA_FIM) ")
                    .AppendLine("  AND A.COD_ENCERRAMENTO IS NULL AND A.EQUIPE = @EQUIPE ");
                }

                parametros.Add("@DATA_INICIO", Convert.ToDateTime(diaConf.Data.ToString("dd/MM/yyyy 00:00")));
                parametros.Add("@DATA_FIM", Convert.ToDateTime(diaConf.Data.ToString("dd/MM/yyyy 23:59")));
                parametros.Add("@EQUIPE", Equipe);


                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, com.Transaction).Result)
                {
                    while (reader.ReadAsync().Result)
                    {
                        var TempoExecucao = reader["TEMPO_EXECUCAO"] != DBNull.Value ? (TimeSpan)(reader["TEMPO_EXECUCAO"]) : Tempo_Execuxao_Padrao;
                        var area = Convert.ToString(reader["COD_AREA"]);
                        var grupoArea = Convert.ToString(reader["COD_ESCRITORIO"]);
                        var codLocalidade = Convert.ToString(reader["COD_LOCALIDADE"]);

                        var agendamentoOs = new Painel_Servicos_AgendamentosEquipe
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Equipe = Convert.ToString(reader["EQUIPE"]),
                            Data_Inicial = reader["DATAHORA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INICIO"]) : (DateTime?)null,
                            Data_Final = reader["DATAHORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM"]) : Convert.ToDateTime(reader["DATAHORA_INICIO"]) + TempoExecucao,
                            Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null,
                            Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : (Double?)null,
                            Numero_Obra = Convert.ToString(reader["NUM_OBRA"]),
                            CodCt = Convert.ToString(reader["CODCT"]),
                            Projeto = Convert.ToString(reader["PROJETO"]),
                            Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : 0,
                            Deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0,
                            GrupoArea = grupoArea,
                            Area = area,
                            CodLocalidade = codLocalidade
                        };

                        if (String.IsNullOrEmpty(grupoArea) == false && String.IsNullOrEmpty(area) == false && String.IsNullOrEmpty(codLocalidade) == false)
                        {
                            agendamentoOs.Localidade = GetLocalidadeStr(grupoArea, area, codLocalidade).Result;
                        }

                        JaAgendados.Add(agendamentoOs);


                    }
                }
            }


            return JaAgendados;
        }

        private List<LocalidadeRoteirizacao> GetLocalidadeRoteirizacaoByPerfil(long IdPerfil)
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


        private async Task GravarDocumento(DocumentoRoteirizar documento, String Usuario)
        {

            StringBuilder sql = new StringBuilder();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (var com = sessionT5.Connection.CreateCommand())
            {

                sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO SET DATA_MAXIMA = @PDATA_MAXIMA,HORA_MAXIMA = @PHORA_MAXIMA,");
                sql.AppendLine("USUARIO_AGENDAMENTO = @PUSUARIO_AGENDAMENTO ,DATAHORA_AGENDAMENTO = @PDIAHORA_AGENDAMENTO,");
                sql.AppendLine("RESPONSABILIDADE = @PRESPONSABILIDADE , ROTERIZAR = @PROTERIZAR, STATUS_EXECUCAO = @PSTATUS,EQUIPE = @PEQUIPE,COD_TURNO_SERV = @PCOD_TURNO_SERV");
                sql.AppendLine("WHERE CODCT = @PCODCT AND PROJETO = @PPROJETO");
                sql.AppendLine("AND NUM_OBRA = @PNUM_OBRA AND NUM_DOC = @PNUM_DOC");

                //update aqui
                parametros.Add("@PCODCT", documento.CodCt);
                parametros.Add("@PPROJETO", documento.Projeto);
                parametros.Add("@PNUM_OBRA", documento.Num_Obra);
                parametros.Add("@PNUM_DOC", documento.Num_Doc);

                parametros.Add("@PDATA_MAXIMA", documento.Data_hora_ini_agendamento);
                parametros.Add("@PHORA_MAXIMA", documento.Data_hora_ini_agendamento.Value.AddMinutes(documento.DeslocamentoMinutos));
                parametros.Add("@PUSUARIO_AGENDAMENTO", Usuario);
                parametros.Add("@PDIAHORA_AGENDAMENTO", DateTime.Now);
                parametros.Add("@PRESPONSABILIDADE", 0);
                parametros.Add("@PROTERIZAR", "N");
                parametros.Add("@PSTATUS", (int)StatusExecucaoAniel.Aberta_Aguard_Atendimento);
                parametros.Add("@PEQUIPE", documento.Equipe);
                parametros.Add("@PCOD_TURNO_SERV", documento.CodTurnoServ);

                if (transactionT5 != null)
                {
                    transactionT5.Enlist(com);
                }

                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);

            }
        }

        private async Task GravarAgendamento(DocumentoRoteirizar documento, Configuracoes_Gerais config, String Usuario)
        {
            if (!documento.Id_Agendamento.HasValue)
            {
                StringBuilder sqlDelete = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var log = GerarLogAgendamento(config);

                using (var com = sessionT5.Connection.CreateCommand())
                {


                    sqlDelete.AppendLine(@"DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL A WHERE
                                    A.NUM_OBRA = @PNUM_OBRA AND A.NUM_DOC = @PNUM_DOC AND A.CODCT = @PCODCT AND A.PROJETO = @PPROJETO");

                    //update aqui
                    parametros.Add("@PCODCT", documento.CodCt);
                    parametros.Add("@PPROJETO", documento.Projeto);
                    parametros.Add("@PNUM_OBRA", documento.Num_Obra);
                    parametros.Add("@PNUM_DOC", documento.Num_Doc);

                    if (transactionT5 != null)
                    {
                        transactionT5.Enlist(com);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sqlDelete, parametros, sessionT5.Connection, com.Transaction);

                }

                StringBuilder sql = new StringBuilder();
                parametros.Clear();
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    sql.AppendLine(@"INSERT INTO TB_AGENDAMENTO_EQUIPEANIEL(ID,USUARIOCRIACAO,DATAHORACRIACAO, 
                                        DATAHORA_INICIO, DATAHORA_FIM,  COD_ESCRITORIO, COD_AREA, LOCALIDADE,
                                        CODCT, PROJETO, NUM_OBRA, NUM_DOC, ID_TIPO_IMOVEL,
                                        ID_TIPO_SERV, COD_LOCALIDADE, ORIGEM, LATITUDE, LONGITUDE, DESLOCAMENTO,EQUIPE,LOG)
                                VALUES((SELECT GEN_ID(GN_AGENDAMENTO_EQUIPEANIEL,1) FROM RDB$DATABASE),@USUARIOCRIACAO,@DATAHORACRIACAO, @DATAHORA_INICIO,
                                        @DATAHORA_FIM, @COD_ESCRITORIO, @COD_AREA, @LOCALIDADE, @PCODCT, @PPROJETO, @PNUM_OBRA, @PNUM_DOC,
                                        @ID_TIPO_IMOVEL,  @ID_TIPO_SERV, @COD_LOCALIDADE, @ORIGEM, @LATITUDE, @LONGITUDE,
                                        @DESLOCAMENTO,@EQUIPE,@LOG)");

                    //update aqui
                    parametros.Add("@PCODCT", documento.CodCt);
                    parametros.Add("@PPROJETO", documento.Projeto);
                    parametros.Add("@PNUM_OBRA", documento.Num_Obra);
                    parametros.Add("@PNUM_DOC", documento.Num_Doc);

                    parametros.Add("@USUARIOCRIACAO", Usuario);
                    parametros.Add("@DATAHORACRIACAO", DateTime.Now);
                    parametros.Add("@DATAHORA_INICIO", documento.Data_hora_ini_agendamento);
                    parametros.Add("@DATAHORA_FIM", documento.Data_hora_fim_agendamento);
                    parametros.Add("@COD_ESCRITORIO", documento.Grupo_Area);
                    parametros.Add("@COD_AREA", documento.Area);
                    parametros.Add("@LOCALIDADE", documento.Localidade);
                    parametros.Add("@ID_TIPO_IMOVEL", documento.Cod_Tipo_Imovel);
                    parametros.Add("@ID_TIPO_SERV", documento.Cod_Tipo_Serv);
                    parametros.Add("@COD_LOCALIDADE", documento.Cod_Localidade);
                    parametros.Add("@ORIGEM", OrigemAgendamentoOS.Roteirizacao);
                    parametros.Add("@LATITUDE", documento.GetLatitude());
                    parametros.Add("@LONGITUDE", documento.GetLongitude());
                    parametros.Add("@DESLOCAMENTO", documento.DeslocamentoMinutos);
                    parametros.Add("@LOG", log);
                    parametros.Add("@EQUIPE", documento.Equipe);

                    if (transactionT5 != null)
                    {
                        transactionT5.Enlist(com);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);

                }
            }
            else
            {
                StringBuilder sqlDelete = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                var log = GerarLogAgendamento(config);

                using (var com = sessionT5.Connection.CreateCommand())
                {


                    sqlDelete.AppendLine(@"DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL A WHERE
                                    A.NUM_OBRA = @PNUM_OBRA AND A.NUM_DOC = @PNUM_DOC AND A.CODCT = @PCODCT AND A.PROJETO = @PPROJETO AND A.ID <> @ID");

                    //update aqui
                    parametros.Add("@PCODCT", documento.CodCt);
                    parametros.Add("@PPROJETO", documento.Projeto);
                    parametros.Add("@PNUM_OBRA", documento.Num_Obra);
                    parametros.Add("@PNUM_DOC", documento.Num_Doc);
                    parametros.Add("@ID", documento.Id_Agendamento);

                    if (transactionT5 != null)
                    {
                        transactionT5.Enlist(com);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sqlDelete, parametros, sessionT5.Connection, com.Transaction);

                }

                StringBuilder sql = new StringBuilder();
                parametros.Clear();
                using (var com = sessionT5.Connection.CreateCommand())
                {


                    sql.AppendLine(@"UPDATE TB_AGENDAMENTO_EQUIPEANIEL SET EQUIPE = @NOVA_EQUIPE, 
                                                                           USUARIOALTERACAO = @USUARIOALTERACAO,
                                                                           DATAHORAALTERACAO = @DATAHORAALTERACAO,
                                                                           LOG = @LOG,
                                                                           DATAHORA_INICIO = @DATAHORA_INICIO, 
                                                                           DATAHORA_FIM = @DATAHORA_FIM,
                                                                           DESLOCAMENTO = @DESLOCAMENTO
                                      WHERE ID = @ID");

                    //update aqui
                    parametros.Add("@ID", documento.Id_Agendamento);


                    parametros.Add("@USUARIOALTERACAO", Usuario);
                    parametros.Add("@DATAHORAALTERACAO", DateTime.Now);
                    parametros.Add("@LOG", log);
                    parametros.Add("@NOVA_EQUIPE", documento.Equipe);
                    parametros.Add("@DATAHORA_INICIO", documento.Data_hora_ini_agendamento);
                    parametros.Add("@DATAHORA_FIM", documento.Data_hora_fim_agendamento);
                    parametros.Add("@DESLOCAMENTO", documento.DeslocamentoMinutos);

                    if (transactionT5 != null)
                    {
                        transactionT5.Enlist(com);
                    }

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);

                }
            }
        }

        private String GerarLogAgendamento(Configuracoes_Gerais config)
        {
            return $"Exige Agendamento Para Criação dos Serviços ? {config.Exige_Agendamento_Servico}\n" +
                   $"Empresa Utilizará Roteirização de Serviços ? {config.Empresa_Roteirizacao_Servico}\n" +
                   $"Utiliza AnielPoint ? {config.Empresa_Roteirizacao_Servico}\n" +
                   $"Criterio Habilidade:{config.Criterio_Habilidade}\nCriterio Area: {config.Criterio_Area}\nCriterio SLA:{config.Criterio_SLA_Execucao}";
        }

        private async Task GravarLog(List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados, String Usuario, RoteirizacaoDiaConf conf, Configuracoes_Gerais config, List<TecnicoRoteirizar> tecnicos, bool Simulacao = false)
        {
            Int64 id_log = 0;

            using (var com = sessionLog.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine("SELECT GEN_ID(GN_LOG_ROTEIRIZACAO, 1) AS ID_LOG FROM RDB$DATABASE");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLog.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        id_log = Convert.ToInt64(reader["ID_LOG"]);
                    }
                }

            }

            using (var com = sessionLog.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                sql.AppendLine(@"INSERT INTO TB_LOG_ROTEIRIZACAO(ID, DATAHORACRIACAO,USUARIOCRIACAO, STATUS,SIMULACAO,DATA_ROTEIRIZACAO,CONFIGURACOES_GERAIS,CONFIGURACOES_ROTEIRIZACAO,TECNICOS_ROTEIRIZACAO,DOCUMENTOS_VALIDOS)
                                VALUES(@ID, @DATAHORACRIACAO,@USUARIOCRIACAO, @STATUS,@SIMULACAO,@DATA_ROTEIRIZACAO,@CONFIGURACOES_GERAIS,@CONFIGURACOES_ROTEIRIZACAO,@TECNICOS_ROTEIRIZACAO,@DOCUMENTOS_VALIDOS)");

                parametros.Add("@ID", id_log);
                parametros.Add("@USUARIOCRIACAO", Usuario);
                parametros.Add("@DATAHORACRIACAO", DateTime.Now);
                parametros.Add("@STATUS", StatusRoteirizacao.Processado);
                parametros.Add("@SIMULACAO", Simulacao);
                parametros.Add("@DATA_ROTEIRIZACAO", conf.Data);

                parametros.Add("@CONFIGURACOES_GERAIS", JsonService.SerializeObject(config));
                parametros.Add("@CONFIGURACOES_ROTEIRIZACAO", JsonService.SerializeObject(conf));
                parametros.Add("@TECNICOS_ROTEIRIZACAO", JsonService.SerializeObject(tecnicos));
                parametros.Add("@DOCUMENTOS_VALIDOS", JsonService.SerializeObject(documentosValidos));


                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionLog.Connection, null);
            }


            foreach (var documento in documentosNaoRoteirizados)
            {
                using (var com = sessionLog.Connection.CreateCommand())
                using (var documentoApi = new Documento_ProducaoApiController())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.AppendLine(@"  insert into TB_LOG_ROTEIRIZACAO_ITEM (ID, USUARIOCRIACAO, DATAHORACRIACAO,MOTIVO, 
                                                                CODCT, PROJETO, NUM_OBRA, NUM_OBRA_ORIGINAL, NUM_DOC, COD_TIPO_SERV,
 
                                                                DESC_TIPO_SERV, CLIENTE, ID_CLIENTE, EQUIPE, NOME_EQUIPE,POSICAO_FILA,
                                                                DATA_HORA_INI_AGENDAMENTO, DATA_HORA_FIM_AGENDAMENTO, DESLOCAMENTO,
                                                                COD_ESCRITORIO, COD_AREA, COD_LOCALIDADE, DESC_ESCRITORIO, DESC_AREA, LOCALIDADE,
                                                                ID_LOG,DATA_VENCIMENTO, SLA,NOME_SUPERVISOR,NUMFRE_SUPERVISOR,NOME_MONITOR,USUARIO_MONITOR,
                                                                POSSUILATLONG,LATITUDE_OS,LONGITUDE_OS,LATITUDE_CAIXA,LONGITUDE_CAIXA,
                                                                LATITUDE_LOCALIDADE,LONGITUDE_LOCALIDADE,DESC_NATUREZA,ORDEM_DISTRIBUIDA,NIVEL_RODADA_ROTEIRIZADA,AGRUPAMENTO_RODADA_ROTEIRIZADA,
                                                                PONTO_PARTIDA,LATITUDE_PONTO_PARTIDA,LONGITUDE_PONTO_PARTIDA,ORIGEM_AGENDAMENTO)
                          values ((SELECT GEN_ID(GN_LOG_ROTEIRIZACAO_ITEM, 1) FROM RDB$DATABASE), @USUARIOCRIACAO,@DATAHORACRIACAO,@MOTIVO,  @CODCT, @PROJETO,
                                  @NUM_OBRA, @NUM_OBRA_ORIGINAL, @NUM_DOC, @COD_TIPO_SERV, @DESC_TIPO_SERV, @CLIENTE, @ID_CLIENTE,
                                  @EQUIPE, @NOME_EQUIPE,@POSICAO_FILA, @DATA_HORA_INI_AGENDAMENTO, @DATA_HORA_FIM_AGENDAMENTO, @DESLOCAMENTO, @COD_ESCRITORIO,
                                  @COD_AREA, @COD_LOCALIDADE, @DESC_ESCRITORIO, @DESC_AREA, @LOCALIDADE, @ID_LOG,@DATAHORAVENCIMENTO, @SLA,@NOME_SUPERVISOR,@NUMFRE_SUPERVISOR,@NOME_MONITOR,@USUARIO_MONITOR,
                                  @POSSUILATLONG,@LATITUDE_OS,@LONGITUDE_OS,@LATITUDE_CAIXA,@LONGITUDE_CAIXA,@LATITUDE_LOCALIDADE,@LONGITUDE_LOCALIDADE,@DESC_NATUREZA,@ORDEM_DISTRIBUIDA,@NIVEL_RODADA_ROTEIRIZADA,@AGRUPAMENTO_RODADA_ROTEIRIZADA,
                                  @PONTO_PARTIDA,@LATITUDE_PONTO_PARTIDA,@LONGITUDE_PONTO_PARTIDA,@ORIGEM_AGENDAMENTO)");

                    parametros.Add("@USUARIOCRIACAO", Usuario);
                    parametros.Add("@DATAHORACRIACAO", DateTime.Now);
                    parametros.Add("@CODCT", documento.CodCt);
                    parametros.Add("@PROJETO", documento.Projeto);
                    parametros.Add("@NUM_OBRA", documento.Num_Obra);
                    parametros.Add("@NUM_OBRA_ORIGINAL", documento.Num_Obra_Original);
                    parametros.Add("@NUM_DOC", documento.Num_Doc);
                    parametros.Add("@DATAHORAVENCIMENTO", documento.Data_Hora_Vencimento_OS = documento.SLA);
                    parametros.Add("@SLA", documentoApi.GetDescricaoSla(documento.SLA, documento.Data_hora_Criacao, documento.Data_hora_Encerramento));
                    parametros.Add("@CLIENTE", documento.Nome_Cliente);
                    parametros.Add("@NOME_EQUIPE", documento?.Tecnico?.Nome);
                    parametros.Add("@DATA_HORA_INI_AGENDAMENTO", documento.Data_hora_ini_agendamento);
                    parametros.Add("@DATA_HORA_FIM_AGENDAMENTO", documento.Data_hora_fim_agendamento);
                    parametros.Add("@DESLOCAMENTO", documento.DeslocamentoMinutos);
                    parametros.Add("@COD_ESCRITORIO", documento.Grupo_Area);
                    parametros.Add("@COD_AREA", documento.Area);
                    parametros.Add("@COD_LOCALIDADE", documento.Cod_Localidade);
                    parametros.Add("@DESC_ESCRITORIO", documento.Descricao_Grupo_Area);
                    parametros.Add("@DESC_AREA", documento.Descricao_Area);
                    parametros.Add("@LOCALIDADE", documento.Localidade);
                    parametros.Add("@ID_CLIENTE", documento.Cod_Cliente);
                    parametros.Add("@EQUIPE", documento.Equipe);
                    parametros.Add("@COD_TIPO_SERV", documento.Cod_Tipo_Serv);
                    parametros.Add("@DESC_TIPO_SERV", documento.Descricao_Tipo_Serv);
                    parametros.Add("@POSICAO_FILA", documento?.Tecnico?.Posicao_Fila);

                    parametros.Add("@NOME_SUPERVISOR", documento?.Tecnico?.Nome_Supervisor);
                    parametros.Add("@NUMFRE_SUPERVISOR", documento?.Tecnico?.NumFre_Supervisor);
                    parametros.Add("@NOME_MONITOR", documento?.Tecnico?.Nome_Monitor);
                    parametros.Add("@USUARIO_MONITOR", documento?.Tecnico?.Usuario_Monitor);
                    parametros.Add("@POSSUILATLONG", String.IsNullOrEmpty(documento.Latitude_Str) == false);
                    parametros.Add("@LATITUDE_OS", documento?.Latitude_Str);
                    parametros.Add("@LONGITUDE_OS", documento?.Longitude_Str);
                    parametros.Add("@LATITUDE_CAIXA", documento?.Latitude_Cx);
                    parametros.Add("@LONGITUDE_CAIXA", documento?.Longitude_Cx);
                    parametros.Add("@LATITUDE_LOCALIDADE", documento?.Latitude_Localidade);
                    parametros.Add("@LONGITUDE_LOCALIDADE", documento?.Longitude_Localidade);
                    parametros.Add("@DESC_NATUREZA", documento?.DescNatureza);
                    parametros.Add("@ORDEM_DISTRIBUIDA", documento?.OrdemDistribuida);
                    parametros.Add("@NIVEL_RODADA_ROTEIRIZADA", documento?.NivelRodadaRoteirizada);
                    parametros.Add("@AGRUPAMENTO_RODADA_ROTEIRIZADA", documento?.AgrupamentoRodadaRoteirizada);
                    parametros.Add("@PONTO_PARTIDA", documento?.Tecnico?.PontoPartida);
                    parametros.Add("@LATITUDE_PONTO_PARTIDA", documento?.Tecnico?.Latitude);
                    parametros.Add("@LONGITUDE_PONTO_PARTIDA", documento?.Tecnico?.Longitude);
                    parametros.Add("@ORIGEM_AGENDAMENTO", documento?.OrigemAgendamento);



                    if (String.IsNullOrEmpty(documento.Log))
                        parametros.Add("@MOTIVO", $"{documento.Num_Obra} - Não há capacidade para roteirizar o serviço.");
                    else
                        parametros.Add("@MOTIVO", documento.Log);

                    parametros.Add("@ID_LOG", id_log);


                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionLog.Connection, null);
                }
            }

            foreach (var documento in documentosRoteirizados)
            {
                using (var com = sessionLog.Connection.CreateCommand())
                using (var documentoApi = new Documento_ProducaoApiController())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.AppendLine(@"  insert into TB_LOG_ROTEIRIZACAO_ITEM (ID, USUARIOCRIACAO, DATAHORACRIACAO, MOTIVO,
                                                                CODCT, PROJETO, NUM_OBRA, NUM_OBRA_ORIGINAL, NUM_DOC, COD_TIPO_SERV,
                                                                DESC_TIPO_SERV, CLIENTE, ID_CLIENTE, EQUIPE, NOME_EQUIPE,POSICAO_FILA,
                                                                DATA_HORA_INI_AGENDAMENTO, DATA_HORA_FIM_AGENDAMENTO, DESLOCAMENTO,
                                                                COD_ESCRITORIO, COD_AREA, COD_LOCALIDADE, DESC_ESCRITORIO, DESC_AREA, LOCALIDADE,
                                                                ID_LOG, DATA_VENCIMENTO, SLA ,NOME_SUPERVISOR,NUMFRE_SUPERVISOR,NOME_MONITOR,USUARIO_MONITOR,POSSUILATLONG,LATITUDE_OS,LONGITUDE_OS,LATITUDE_CAIXA,LONGITUDE_CAIXA,
                                                                LATITUDE_LOCALIDADE,LONGITUDE_LOCALIDADE,DESC_NATUREZA,ORDEM_DISTRIBUIDA,NIVEL_RODADA_ROTEIRIZADA,AGRUPAMENTO_RODADA_ROTEIRIZADA,
                                                                PONTO_PARTIDA,LATITUDE_PONTO_PARTIDA,LONGITUDE_PONTO_PARTIDA,ORIGEM_AGENDAMENTO)
                          values ((SELECT GEN_ID(GN_LOG_ROTEIRIZACAO_ITEM, 1) FROM RDB$DATABASE), @USUARIOCRIACAO,@DATAHORACRIACAO,@MOTIVO,  @CODCT, @PROJETO,
                                  @NUM_OBRA, @NUM_OBRA_ORIGINAL, @NUM_DOC, @COD_TIPO_SERV, @DESC_TIPO_SERV, @CLIENTE, @ID_CLIENTE,
                                  @EQUIPE, @NOME_EQUIPE,@POSICAO_FILA, @DATA_HORA_INI_AGENDAMENTO, @DATA_HORA_FIM_AGENDAMENTO, @DESLOCAMENTO, @COD_ESCRITORIO,
                                  @COD_AREA, @COD_LOCALIDADE, @DESC_ESCRITORIO, @DESC_AREA, @LOCALIDADE, @ID_LOG, @DATAHORAVENCIMENTO, @SLA,@NOME_SUPERVISOR,@NUMFRE_SUPERVISOR,@NOME_MONITOR,@USUARIO_MONITOR,@POSSUILATLONG,@LATITUDE_OS,
                                  @LONGITUDE_OS,@LATITUDE_CAIXA,@LONGITUDE_CAIXA,@LATITUDE_LOCALIDADE,@LONGITUDE_LOCALIDADE,@DESC_NATUREZA,@ORDEM_DISTRIBUIDA,@NIVEL_RODADA_ROTEIRIZADA,@AGRUPAMENTO_RODADA_ROTEIRIZADA,
                                  @PONTO_PARTIDA,@LATITUDE_PONTO_PARTIDA,@LONGITUDE_PONTO_PARTIDA,@ORIGEM_AGENDAMENTO)");

                    parametros.Add("@USUARIOCRIACAO", Usuario);
                    parametros.Add("@DATAHORACRIACAO", DateTime.Now);
                    parametros.Add("@DATAHORAVENCIMENTO", documento.Data_Hora_Vencimento_OS = documento.SLA);
                    parametros.Add("@CODCT", documento.CodCt);
                    parametros.Add("@PROJETO", documento.Projeto);
                    parametros.Add("@NUM_OBRA", documento.Num_Obra);
                    parametros.Add("@NUM_OBRA_ORIGINAL", documento.Num_Obra_Original);
                    parametros.Add("@NUM_DOC", documento.Num_Doc);
                    parametros.Add("@COD_TIPO_SERV", documento.Cod_Tipo_Serv);
                    parametros.Add("@DESC_TIPO_SERV", documento.Descricao_Tipo_Serv);
                    parametros.Add("@CLIENTE", documento.Nome_Cliente);
                    parametros.Add("@NOME_EQUIPE", documento.Tecnico.Nome);
                    parametros.Add("@DATA_HORA_INI_AGENDAMENTO", documento.Data_hora_ini_agendamento);
                    parametros.Add("@DATA_HORA_FIM_AGENDAMENTO", documento.Data_hora_fim_agendamento);
                    parametros.Add("@DESLOCAMENTO", documento.DeslocamentoMinutos);
                    parametros.Add("@COD_ESCRITORIO", documento.Grupo_Area);
                    parametros.Add("@COD_AREA", documento.Area);
                    parametros.Add("@COD_LOCALIDADE", documento.Cod_Localidade);
                    parametros.Add("@DESC_ESCRITORIO", documento.Descricao_Grupo_Area);
                    parametros.Add("@DESC_AREA", documento.Descricao_Area);
                    parametros.Add("@LOCALIDADE", documento.Localidade);
                    parametros.Add("@ID_LOG", id_log);
                    parametros.Add("@ID_CLIENTE", documento.Cod_Cliente);
                    parametros.Add("@EQUIPE", documento.Equipe);
                    parametros.Add("@POSICAO_FILA", documento?.Tecnico?.Posicao_Fila);
                    parametros.Add("@MOTIVO", "Roteirizado com Sucesso");
                    parametros.Add("@SLA", documentoApi.GetDescricaoSla(documento.SLA, documento.Data_hora_Criacao, documento.Data_hora_Encerramento));
                    parametros.Add("@NOME_SUPERVISOR", documento?.Tecnico?.Nome_Supervisor);
                    parametros.Add("@NUMFRE_SUPERVISOR", documento?.Tecnico?.NumFre_Supervisor);
                    parametros.Add("@NOME_MONITOR", documento?.Tecnico?.Nome_Monitor);
                    parametros.Add("@USUARIO_MONITOR", documento?.Tecnico?.Usuario_Monitor);
                    parametros.Add("@POSSUILATLONG", String.IsNullOrEmpty(documento.Latitude_Str) == false);
                    parametros.Add("@LATITUDE_OS", documento?.Latitude_Str);
                    parametros.Add("@LONGITUDE_OS", documento?.Longitude_Str);
                    parametros.Add("@LATITUDE_CAIXA", documento?.Latitude_Cx);
                    parametros.Add("@LONGITUDE_CAIXA", documento?.Longitude_Cx);
                    parametros.Add("@LATITUDE_LOCALIDADE", documento?.Latitude_Localidade);
                    parametros.Add("@LONGITUDE_LOCALIDADE", documento?.Longitude_Localidade);
                    parametros.Add("@DESC_NATUREZA", documento?.DescNatureza);
                    parametros.Add("@ORDEM_DISTRIBUIDA", documento?.OrdemDistribuida);
                    parametros.Add("@NIVEL_RODADA_ROTEIRIZADA", documento?.NivelRodadaRoteirizada);
                    parametros.Add("@AGRUPAMENTO_RODADA_ROTEIRIZADA", documento?.AgrupamentoRodadaRoteirizada);
                    parametros.Add("@PONTO_PARTIDA", documento?.Tecnico?.PontoPartida);
                    parametros.Add("@LATITUDE_PONTO_PARTIDA", documento?.Tecnico?.Latitude);
                    parametros.Add("@LONGITUDE_PONTO_PARTIDA", documento?.Tecnico?.Longitude);
                    parametros.Add("@ORIGEM_AGENDAMENTO", documento?.OrigemAgendamento);


                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionLog.Connection, null);
                }
            }

        }

        public async Task ApagarAgendamento(List<Int64> lista_ids_Agendamento)
        {
            foreach (var listaQuebrada in ListService.GetListaQuebrada(lista_ids_Agendamento, 500))
            {
                if (listaQuebrada.Value.Count > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        sql.AppendLine("DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL WHERE ");

                        for (int i = 0; i < listaQuebrada.Value.Count; i++)
                        {
                            sql.AppendLine($" (ID = {listaQuebrada.Value[i]})  ");
                            if ((i + 1) < listaQuebrada.Value.Count)
                                sql.AppendLine(" OR ");
                        }


                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);

                    }
                }
            }
        }

        public async Task<List<Agendamento_EquipeAniel>> GetAgendamentos(List<Int64?> ids_Agendamento)
        {
            List<Agendamento_EquipeAniel> retorno = new List<Agendamento_EquipeAniel>();
            using (var repository = new Conexao<Agendamento_EquipeAniel>(sessionT5NH))
            {
                var listas_agendamento = ListService.GetListaQuebrada(ids_Agendamento, 100);
                foreach (var lista_agendamento in listas_agendamento.Values)
                {
                    retorno.AddRange((await repository.select(p => p.Id.IsIn(lista_agendamento))).ToList());
                }
            }

            return retorno;
        }

        public async Task Documento_RemoverEquipe(List<DocumentoRoteirizar> documentos)
        {
            List<DocumentoProducao_GetDocumento> chaves_documento = documentos.Select(doc =>
            {
                return new DocumentoProducao_GetDocumento
                {
                    CodCt = doc.CodCt,
                    Num_Doc = doc.Num_Doc,
                    Num_Obra = doc.Num_Obra,
                    Projeto = doc.Projeto
                };

            }).ToList();

            foreach (var listaQuebrada in ListService.GetListaQuebrada(chaves_documento, 200))
            {
                if (listaQuebrada.Value.Count > 0)
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO SET EQUIPE = NULL,STATUS_EXECUCAO = ");
                        sql.AppendLine($"  CASE WHEN (DATA_MAXIMA IS NOT NULL AND STATUS_EXECUCAO IN (4,0,10,13)) THEN 13 ");
                        sql.AppendLine($"       WHEN (DATA_MAXIMA IS NULL AND STATUS_EXECUCAO IN (4,0,10,13)) THEN 4 ");
                        sql.AppendLine($"       WHEN (STATUS_EXECUCAO NOT IN (4,0,10,13)) THEN STATUS_EXECUCAO ");
                        sql.AppendLine($"  END ");
                        sql.AppendLine($"  WHERE  ");

                        for (int i = 0; i < listaQuebrada.Value.Count; i++)
                        {
                            sql.AppendLine($" (CODCT = '{listaQuebrada.Value[i].CodCt}' AND PROJETO = '{listaQuebrada.Value[i].Projeto}' AND NUM_OBRA = '{listaQuebrada.Value[i].Num_Obra}' AND NUM_DOC = {listaQuebrada.Value[i].Num_Doc})  ");
                            if ((i + 1) < listaQuebrada.Value.Count)
                                sql.AppendLine(" OR ");
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);

                    }
                }
            }



        }

        public async Task<string> GetLocalidadeStr(String GrupoArea, String Area, String CodLocalidade)
        {

            try
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = sessionT5.Connection.CreateCommand())
                {

                    sql.AppendLine("SELECT LOCALIDADE FROM TB_LOCALIDADE WHERE COD_AREA = @AREA AND COD_ESCRITORIO = @GRUPOAREA AND COD_LOCALIDADE = @COD_LOCALIDADE");
                    parametros.Add("@AREA", Area);
                    parametros.Add("@GRUPOAREA", GrupoArea);
                    parametros.Add("@COD_LOCALIDADE", CodLocalidade);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            return Convert.ToString(reader["LOCALIDADE"]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("teste");
            }

            return null;


        }

        public async Task<List<UsuarioTecnicoRoteirizacao>> GetUsuarioTecnicoRoteirizacao(string usuario)
        {
            using (var repoUsuarioTecnicoRoteirizacao = new Conexao<UsuarioTecnicoRoteirizacao>(sessionT5NH))
            {
                return (await repoUsuarioTecnicoRoteirizacao.select(p => p.Usuario == usuario)).ToList();
            }
        }

    }
}