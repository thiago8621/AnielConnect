using Aniel.Connect.Core;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Log_LocalizacaoAnielApiController : BaseApiController
    {


        [HttpGet()]
        [Route("api/Log_LocalizacaoAnielApi/Gravar")]
        public async Task Gravar(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc, String Usuario, StatusExecucaoAniel Status)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                using (var repo = new Conexao<Log_LocalizacaoAniel>(session))
                {
                    await repo.Save(new Log_LocalizacaoAniel
                    {
                        Id = repo.GetSequence("gen_tb_log_localizacao_id").Result,
                        CodCt = CodCt,
                        Projeto = Projeto,
                        Num_Obra = Num_Obra,
                        Contrato = Num_Doc.Value,
                        UsuarioCriacao = Usuario,
                        Status = Status,
                        Data = DateTime.Now,
                        Hora = Convert.ToDateTime(DateTime.Now.ToString("HH:mm:ss"))
                    });
                }
            }
        }

        [HttpGet()]
        [Route("api/Log_LocalizacaoAnielApi/ServicoTransferenciaFireBirdMongo")]
        public async Task ServicoTransferenciaFireBirdMongo(String DataInicio, String DataFim)
        {
            Console.WriteLine("iniciar sessao t5");
            using (ISession session = HibernateUtil.GetSessionT5())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Console.WriteLine("iniciar sessao mongo");
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repositoryMongo = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                using (var repoFireBird = new Conexao<Log_LocalizacaoAniel>(session))
                using (var repositoryLogMongo = new RepositoryMongo<LogLocalizacaoDataMongo>(conexaoMongo))
                {
                    Console.WriteLine("buscar log_localizacao");
                    var originais_log_firebird = await GetLogLocalizacaoFirebird(session, DataInicio, DataFim);
                    Console.WriteLine("buscar log_localizacao_jornada");
                    var originais_firebird = await repoFireBird.select(p => p.Data >= Convert.ToDateTime(DataInicio) && p.Data <= Convert.ToDateTime(DataFim) && (p.TransferenciaMongo == null || p.TransferenciaMongo == "N"));
                    var agrupado_ordens = originais_firebird.GroupBy(p => new { p.Num_Obra, p.Contrato, p.CodCt, p.Projeto });
                    var agrupado_log_ordens = originais_log_firebird.GroupBy(p => new { p.Equipe });
                    List<DocumentoProducaoMongo> documentosMongo = new List<DocumentoProducaoMongo>();
                    List<LogLocalizacaoDataMongo> logsMongo = new List<LogLocalizacaoDataMongo>();

                    try
                    {
                        foreach (var grupo in agrupado_ordens)
                        {
                            if (grupo.Count() > 0)
                            {
                                DocumentoProducaoMongo documentoProducaoMongo = await repositoryMongo.Find(p => p.codCt == grupo.Key.CodCt && p.projeto == grupo.Key.Projeto
                                                                                                                && p.numObra == grupo.Key.Num_Obra && p.contrato == grupo.Key.Contrato);

                                if (documentoProducaoMongo == null)
                                {
                                    documentoProducaoMongo = new DocumentoProducaoMongo();
                                    documentoProducaoMongo.codCt = grupo.Key.CodCt;
                                    documentoProducaoMongo.projeto = grupo.Key.Projeto;
                                    documentoProducaoMongo.numObra = grupo.Key.Num_Obra;
                                    documentoProducaoMongo.contrato = grupo.Key.Contrato;
                                }

                                if (documentoProducaoMongo.status == null)
                                    documentoProducaoMongo.status = new List<StatusDocumentoProducao>();


                                foreach (var item in grupo)
                                {
                                    documentoProducaoMongo.status.Add(new StatusDocumentoProducao
                                    {
                                        equipe = item.Equipe?.Matricula,
                                        latitude = item.Latitude.GetValueOrDefault(0),
                                        longitude = item.Longitude.GetValueOrDefault(0),
                                        nome = item.Equipe?.Nome,
                                        numFre = item.Equipe?.Num_Fre,
                                        data = item.Data.Value,
                                        hora = item.Data.Value.Add(item.Hora.Value.TimeOfDay),
                                        dataHoraServidor = item.DataHora_Servidor ?? DateTime.MinValue,
                                        statusExecucao = (int?)item.Status
                                    });
                                }

                                documentosMongo.Add(documentoProducaoMongo);

                            }

                        }


                        foreach (var grupo in agrupado_log_ordens)
                        {
                            if (grupo.Count() > 0)
                            {
                                


                                foreach (var item in grupo)
                                {
                                    LogLocalizacaoDataMongo logLocalizacaoMongo = await repositoryLogMongo.Find(p => p.Equipe == grupo.Key.Equipe && p.DataDocumento == item.DataHora.Date);

                                    if (logLocalizacaoMongo == null)
                                    {
                                        logLocalizacaoMongo = new LogLocalizacaoDataMongo();
                                        logLocalizacaoMongo.Equipe = grupo.Key.Equipe;
                                    }

                                    if (logLocalizacaoMongo.localizacoes == null)
                                        logLocalizacaoMongo.localizacoes = new List<Localizacao>();

                                    logLocalizacaoMongo.localizacoes.Add(new Localizacao
                                    {
                                        Aparelho = new Aparelho
                                        {
                                            Carregando = (TipoCarregamentoBateria)item.Carregando,
                                            Imei = item.IMEI,
                                            Modelo = item.ModeloAparelho,
                                            NivelBateria = item.NivelBateria.GetValueOrDefault(0),
                                            NivelRedeMovel = item.NivelRedeMovel,
                                            StatusWifi = item.StatusWifi
                                        },
                                        latitude = item.Latitude.Value,
                                        longitude = item.Longitude.Value,
                                        DataHora = item.DataHora,
                                        statusGps = item.StatusGPS,
                                        precisao = item.Precisao.Value,
                                        justificativa = item.Justificativa,
                                        idCheckpoint = item.Id_Checkpoint,
                                        statusTecnico = (EquipeStatusTipo)item.TipoLocalizacao

                                    });

                                    logsMongo.Add(logLocalizacaoMongo);
                                }

                                

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }


                    Console.WriteLine("atualizar firebird com Transferencia Mongo S");

                    if (originais_firebird.Count > 0)
                        await UpdateLocalizacaoFirebird(session, originais_log_firebird);

                    await repoFireBird.SaveOrUpdate(originais_firebird.Select(p => { p.TransferenciaMongo = "S"; return p; }).ToList());

                    Console.WriteLine("ATUALIZAR DADOS NO MONGO DB");

                    foreach (var documentoProducaoMongo in documentosMongo)
                    {
                        if (documentoProducaoMongo._id != null)
                            await repositoryMongo.Delete(p => p._id == documentoProducaoMongo._id);

                        await repositoryMongo.Save(documentoProducaoMongo);
                    }

                    foreach (var logLocalizacaoMongo in logsMongo)
                    {
                        if (logLocalizacaoMongo._id != null)
                            await repositoryLogMongo.Delete(p => p._id == logLocalizacaoMongo._id);

                        await repositoryLogMongo.Save(logLocalizacaoMongo);
                    }

                    Console.WriteLine("FIM DE TRANSFERENCIA");

                    transaction.Commit();
                }

            }






        }

        public async Task<List<LogLocalizacaoJornada>> GetLogLocalizacaoFirebird(ISession sessionT5, String DataInicio, String DataFim)
        {
            List<LogLocalizacaoJornada> retorno = new List<LogLocalizacaoJornada>();
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"SELECT CODCT, EQUIPE, CONTRATO, NUM_OBRA, PROJETO, USUARIO, ID, DATAHORA, TIPO_LOCALIZACAO, LATITUDE, LONGITUDE,
                                                        NIVEL_BATERIA, STATUS_WIFI, MODELO_APARELHO, NIVEL_REDE_MOVEL, IMEI, CARREGANDO, STATUS_GPS, PRECISAO,
                                                        ID_TIPO_CHECKPOINT, JUSTIFICATIVA
                                                        FROM TB_LOG_LOCALIZACAO_JORNADA");


                sql.AppendLine(" WHERE  (DATAHORA BETWEEN @DATA_INI AND @DATA_FIM) AND (COALESCE(TRANSFERENCIA_MONGO,'N') = 'N')");


                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@DATA_INI", DataInicio);
                parametros.Add("@DATA_FIM", DataFim);


                if (sessionT5.Transaction != null)
                    sessionT5.Transaction.Enlist(com);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, com.Transaction))
                {
                    while (await reader.ReadAsync())
                    {
                        var log = new LogLocalizacaoJornada();

                        log.Carregando = reader["CARREGANDO"] != DBNull.Value ? Convert.ToInt32(reader["CARREGANDO"]) : (int?)null;
                        log.CodCt = Convert.ToString(reader["CODCT"]);
                        log.Projeto = Convert.ToString(reader["PROJETO"]);
                        log.Contrato = reader["CONTRATO"] != DBNull.Value ? Convert.ToInt32(reader["CONTRATO"]) : 0;
                        log.Num_Obra = Convert.ToString(reader["NUM_OBRA"]);
                        log.DataHora = Convert.ToDateTime(reader["DATAHORA"]);
                        log.Equipe = Convert.ToString(reader["EQUIPE"]);
                        log.Usuario = Convert.ToString(reader["USUARIO"]);
                        log.Id = Convert.ToInt64(reader["ID"]);
                        log.Id_Checkpoint = reader["ID_TIPO_CHECKPOINT"] != DBNull.Value ? Convert.ToInt32(reader["ID_TIPO_CHECKPOINT"]) : 0;
                        log.IMEI = Convert.ToString(reader["IMEI"]);
                        log.Latitude = Convert.ToDouble(reader["LATITUDE"]);
                        log.Longitude = Convert.ToDouble(reader["LONGITUDE"]);
                        log.Justificativa = Convert.ToString(reader["JUSTIFICATIVA"]);
                        log.ModeloAparelho = Convert.ToString(reader["MODELO_APARELHO"]);
                        log.NivelBateria = reader["NIVEL_BATERIA"] != DBNull.Value ? Convert.ToDouble(reader["NIVEL_BATERIA"]) : (double?)null;
                        log.NivelRedeMovel = Convert.ToString(reader["NIVEL_REDE_MOVEL"]);
                        log.StatusGPS = Convert.ToString(reader["STATUS_GPS"]);
                        log.Precisao = reader["PRECISAO"] != DBNull.Value ? Convert.ToDouble(reader["PRECISAO"]) : (double?)null;
                        log.StatusWifi = Convert.ToInt32(reader["STATUS_WIFI"]);
                        log.TipoLocalizacao = reader["TIPO_LOCALIZACAO"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_LOCALIZACAO"]) : (int?)null;

                        retorno.Add(log);
                    };
                }
            }

            return retorno;

        }



        public async Task UpdateLocalizacaoFirebird(ISession sessionT5, List<LogLocalizacaoJornada> logs)
        {
            foreach (var listaQuebrada in ListService.GetListaQuebrada(logs, 300))
            {
                List<LogLocalizacaoJornada> retorno = new List<LogLocalizacaoJornada>();
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    if (sessionT5.Transaction != null)
                        sessionT5.Transaction.Enlist(com);

                    var ids = new List<string>();
                    foreach (var item in listaQuebrada.Value.Where(p => p.Id != null).ToList())
                    {
                        ids.Add(item.Id.Value.ToString());
                    }


                    StringBuilder sql = new StringBuilder($@"UPDATE TB_LOG_LOCALIZACAO_JORNADA SET TRANSFERENCIA_MONGO = 'S' WHERE ID IN({String.Join(",", ids)})");
                    await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), sessionT5.Connection, com.Transaction);

                }
            }
        }

        public class LogLocalizacaoJornada
        {
            public virtual String Equipe { get; set; }
            public virtual String CodCt { get; set; }
            public virtual String Projeto { get; set; }
            public virtual String Num_Obra { get; set; }
            public virtual Int32 Contrato { get; set; }
            public virtual String Usuario { get; set; }
            public virtual String ModeloAparelho { get; set; }
            public virtual String NivelRedeMovel { get; set; }
            public virtual String IMEI { get; set; }
            public virtual String StatusGPS { get; set; }
            public virtual String Justificativa { get; set; }
            public virtual int? Carregando { get; set; }
            public virtual Int64? Id { get; set; }
            public virtual DateTime DataHora { get; set; }
            public virtual int? TipoLocalizacao { get; set; }

            public virtual int Id_Checkpoint { get; set; }


            public virtual Double? Precisao { get; set; }
            public virtual Double? NivelBateria { get; set; }
            public virtual int StatusWifi { get; set; }
            public virtual Double? Latitude { get; set; }
            public virtual Double? Longitude { get; set; }
        }


    }
}