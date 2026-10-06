using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class NotificacoesMap : ClassMapBase<Notificacoes>
    {
        public NotificacoesMap() : base()
        {
            Table("TB_NOTIFICACOES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_NOTIFICACOES");
            Map(x => x.Notificacao, "NOTIFICACAO").Length(16000);
            Map(x => x.Descricao, "DESCRICAO").Length(16000);
            Map(x => x.VariaveisDisponiveis, "VARIAVEIS_DISPONIVEIS").CustomType<Notificacoes_Variaveis>(); //Lista de variáveis disponíveis para o texto (ENUM)
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.MomentoExecucao, "MOMENTO_EXECUCAO").CustomType<Notificacoes_Mom_Execucao>(); //Lista de quando será enviado (ENUM)
            Map(x => x.AcaoExecutada, "ACAO_EXECUTADA").CustomType<Notificacoes_Acao_Executada>(); //O que deverá acontecer quando clicar em uma notificação
            Map(x => x.SecaoConnect, "SECAO_CONNECT").CustomType<Model.enumerado.SecaoConnectPerfilEnum>(); //O que deverá acontecer quando clicar em uma notificação
            Map(x => x.FrequenciaNotificaoMinutos, "FREQUENCIA_NOTIFICAO_MINUTOS"); // Quantidade de minutos referentes para proxima notificacao em casos de notificacoes que devem ser repetidas automaticamente 
            Map(x => x.TempoDisparoNotificacaoMinutos, "DISPARO_NOTIFICAO_MINUTOS"); // Tempo em minutos para a notificação ser disparada de acordo com regras específicas a ela. 
            HasMany(x => x.Destinatarios).KeyColumn("ID_NOTIFICACOES").Cascade.All().Inverse();
        }
    }
}
