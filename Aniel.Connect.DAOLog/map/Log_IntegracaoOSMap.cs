using Aniel.Connect.ModelLog.enumerados;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAOLog.map
{
    public class Log_IntegracaoOSMap : ClassMapBaseLog<LogIntegracaoOS>
    {
        public Log_IntegracaoOSMap() : base()
        {
            Table("TB_LOG_INTEGRACAO_OS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TB_LOG_INTEGRACAO_OS");
            Map(c => c.Servico, "SERVICO");
            Map(c => c.NumeroOS, "NUMERO_OS");
            Map(c => c.Mensagem, "MENSAGEM");
            Map(c => c.Data, "DATA").CustomType("Date");
            Map(c => c.Hora, "HORA").CustomType("Time");
            Map(c => c.Momento, "MOMENTO").CustomType<MomentoImportacao>();
            Map(c => c.Status, "STATUS");
            Map(c => c.Projeto, "PROJETO");
            Map(c => c.Num_Doc  , "NUM_DOC");
            Map(c => c.Cod_CT, "CODCT");
            Map(c => c.Json_Envio, "JSON_ENVIO");
            Map(c => c.Json_Retorno, "JSON_RETORNO");
        }
    }
}
