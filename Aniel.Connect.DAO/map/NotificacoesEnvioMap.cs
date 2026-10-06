using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class NotificacoesEnvioMap : ClassMapBase<NotificacoesEnvio>
    {
        public NotificacoesEnvioMap() : base()
        {
            Table("TB_NOTIFICACOES_ENVIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_NOTIFICACOES_ENVIO");
            Map(x => x.Texto_Notificacao, "TEXTO_NOTIFICACAO").Length(16000);
            Map(x => x.Status_Notificacao, "STATUS").CustomType<Status_Notificacao>().Index("IDX_NOTIFICACAO_LAYOUT");
            Map(x => x.Data_Envio, "DATA_ENVIO").Index("IDX_NOTIFICACAO_LAYOUT");
            Map(x => x.Chave, "CHAVE");
            Map(x => x.Usuario_Destino, "USUARIO_DESTINO").Index("IDX_NOTIFICACAO_LAYOUT");
            Map(x => x.Acao, "ACAO").CustomType<Notificacoes_Acao_Executada>().Index("IDX_NOTIFICACAO_LAYOUT");
            References(x => x.Notificacoes, "ID_NOTIFICACOES").ForeignKey("FK_NOTIFICACOES");
        }
    }
}
