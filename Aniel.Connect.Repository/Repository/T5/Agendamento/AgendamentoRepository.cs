using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.Agendamento
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly ISession _sessionT5Nh;
        private readonly ISession _sessionT5Sql;

        public AgendamentoRepository(ISession sessionT5Nh, ISession sessionT5Sql)
        {
            this._sessionT5Sql = sessionT5Sql;
            this._sessionT5Nh = sessionT5Nh;
        }

        public async Task<List<AgendamentoEquipeHorarioModel>> GetForcaTrabalho(DateTime dia, Configuracoes_Gerais config)
        {
            List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)> perfilAgendamentoCache = new List<(long? perfil, List<LocalidadeRoteirizacao> localidades, List<Natureza_ProducaoAniel> naturezas)>();

            var _temCriterioArea = config.Criterio_Area;
            var _temCriterioHabilidades = config.Criterio_Habilidade;
            var _temCriterioSLA_Execucao = config.Criterio_SLA_Execucao;
            var _Utiliza_AnielPoint = config.UtilizaPoint_RoteirizacaoAgendamento;
            List<AgendamentoEquipeHorarioModel> equipeshorarios = new List<AgendamentoEquipeHorarioModel>();


            using (var sessionT5NH = HibernateUtil.GetSessionT5())
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryNatureza = new Conexao<PerfilAgendamentoNatureza>(sessionT5NH))
            using (var repositoryTipoServicoEquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5NH))
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
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

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null))
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
                                localidades = idPerfil == null ? new List<LocalidadeRoteirizacao>() : GetLocalidadeRoteirizacaoByPerfil(idPerfil.Value);
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

        private List<LocalidadeRoteirizacao> GetLocalidadeRoteirizacaoByPerfil(long IdPerfil)
        {

            List<LocalidadeRoteirizacao> retorno = new List<LocalidadeRoteirizacao>();
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                Dictionary<String, Object> parametros = new Dictionary<string, object>();


                sql.AppendLine("SELECT A.COD_ESCRITORIO,A.COD_AREA,A.LOCALIDADE,D.COD_LOCALIDADE,B.DESCRICAO AS DESCRICAO_ESCRITORIO,C.DESCRICAO AS DESCRICAO_AREA,A.NIVEL,A.TODAS_LOCALIDADES");
                sql.AppendLine("FROM TB_PERFIL_AGENDAMENTO_AREA A");
                sql.AppendLine("LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)");
                sql.AppendLine("LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)");
                sql.AppendLine("LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.LOCALIDADE = D.LOCALIDADE)");
                sql.AppendLine("WHERE A.ID_PERFIL_AGENDAMENTO = @ID_PERFIL");
                parametros.Add("@ID_PERFIL", IdPerfil);


                using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction).Result)
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
                        });
                    }
                }
            }


            return retorno;
        }

        public async Task<List<AgendamentoDadosModel>> GetJaAgendadosDetalhamento(DateTime dataInicio, DateTime dataFim)
        {

            List<AgendamentoDadosModel> JaAgendados = new List<AgendamentoDadosModel>();
            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();




                StringBuilder sql = new StringBuilder(@"SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.EQUIPE AS EQUIPE_DEFINITIVA,A.EQUIPE_TMP AS EQUIPE_TEMPORARIA, A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND ((COALESCE(B.COD_ENCERRAMENTO,'') = '') OR ( (B.DATA_PROD = @DATA_INICIO) AND ((B.DATA_PROD+B.HORA_PROD) < CURRENT_TIMESTAMP)) )
                                                         
                                                         UNION

                                                         SELECT COALESCE(A.EQUIPE, A.EQUIPE_TMP) AS EQUIPE,A.EQUIPE AS EQUIPE_DEFINITIVA,A.EQUIPE_TMP AS EQUIPE_TEMPORARIA,A.DATAHORA_INICIO,A.DATAHORA_FIM,A.DESLOCAMENTO,A.DATAHORACRIACAO,A.USUARIOCRIACAO,A.ORIGEM,B.COD_ESCRITORIO,B.COD_AREA,B.COD_LOCALIDADE,C.COD_NATUREZA,C.DESCRICAO AS TIPO_SERVICO,
                                                         IIF( A.EQUIPE_TMP IS NOT NULL AND A.EQUIPE IS NULL, 1, 0) TMP,D.LOCALIDADE,B.NUM_OBRA, E.DESCRICAO AS DESCRICAO_ESCRITORIO, F.DESCRICAO AS DESCRICAO_AREA, G.DESCRICAO AS DESCRICAO_NATUREZA,H.DESCRICAO AS PERIODO,B.COD_TURNO_SERV,B.STATUS_EXECUCAO, B.USUARIO_AGENDAMENTO
                                                         FROM TB_AGENDAMENTO_EQUIPEANIEL A
                                                         INNER JOIN TB_DOCUMENTO_PRODUCAO B ON (A.NUM_OBRA = B.NUM_OBRA AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_DOC = B.NUM_DOC)
                                                         LEFT JOIN TB_TIPO_SERVICO_EQUIPE C ON (B.COD_TIPO_SERV = C.COD_TIPO_SERV)
                                                         LEFT JOIN TB_LOCALIDADE D ON (B.COD_ESCRITORIO = D.COD_ESCRITORIO AND B.COD_AREA = D.COD_AREA AND B.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                         LEFT JOIN TB_ESCRITORIO E ON (B.COD_ESCRITORIO = E.COD_ESCRITORIO)
                                                         LEFT JOIN TB_AREA F ON (B.COD_ESCRITORIO = F.COD_ESCRITORIO AND B.COD_AREA = F.COD_AREA)
                                                         LEFT JOIN TB_NATUREZA_PRODUCAO G ON (G.ID = C.COD_NATUREZA)
                                                         LEFT JOIN TB_TURNO_SERVICO H ON (H.ID = B.COD_TURNO_SERV)
                                                         WHERE A.DATAHORA_FIM BETWEEN @DATA_INICIO AND @DATA_FIM
                                                         AND ((COALESCE(B.COD_ENCERRAMENTO,'') = '') OR (  (B.DATA_PROD = @DATA_INICIO) AND  ((B.DATA_PROD+B.HORA_PROD) < CURRENT_TIMESTAMP)))");

                //parametros.Add("@DATA_INICIO", agendamentoDisponiveis.Hora_Inicio.AddSeconds(1));
                //parametros.Add("@DATA_FIM", agendamentoDisponiveis.Hora_Final.AddSeconds(-1));
                parametros.Add("@DATA_INICIO", dataInicio.Date);
                parametros.Add("@DATA_FIM", dataFim.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var equipe = Convert.ToString(reader["EQUIPE"]);
                        var equipeDefinitiva = Convert.ToString(reader["EQUIPE_DEFINITIVA"]);
                        var equipeTemporaria = Convert.ToString(reader["EQUIPE_TEMPORARIA"]);

                        var dataHoraInicio = Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                        var dataHoraFim = Convert.ToDateTime(reader["DATAHORA_FIM"]);
                        var tmp = Convert.ToBoolean(reader["TMP"]);
                        var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0;
                        var dataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                        var usuario = Convert.ToString(reader["USUARIOCRIACAO"]);
                        var usuarioAgendamento = Convert.ToString(reader["USUARIO_AGENDAMENTO"]);
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
                            UsuarioAgendamento = usuarioAgendamento,
                            Origem = origem,
                            Capaz = false,
                            EquipeDefinitiva = equipeDefinitiva,
                            EquipeTemporaria = equipeTemporaria
                        });
                    }
                }
            }

            return JaAgendados;
        }
        
        
        public async Task<Boolean> PerfilAgendamentoAtivado(long idPerfilAgendamento)
        {
            Boolean response = false;

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionT5))
            {
                var perfilAgendamento = await repoPerfilAgendamento.selectFirst(p => p.Id == idPerfilAgendamento);

                if (perfilAgendamento != null)
                    response = perfilAgendamento.Status == Status_Default.Ativo;
            }

            return response;
        }
    }
}
