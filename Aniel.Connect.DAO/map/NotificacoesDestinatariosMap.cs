using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class NotificacoesDestinatariosMap : ClassMapBase<NotificacoesDestinatarios>
    {
        public NotificacoesDestinatariosMap() : base()
        {
            Table("TB_NOTIFICACOES_DESTINATARIOS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_NOTIFICACOES_DESTINATARIOS");

            Map(x => x.Usuario_Destinatario, "USUARIO_DESTINO").Index("IDX_NOTIFICACAO_LAYOUT");
            Map(x => x.Tipo_Destinatario, "TIPO").CustomType<Model.enumerado.TipoUsuarioEnum>();
            References(x => x.Notificacoes, "ID_NOTIFICACOES").ForeignKey("FK_DESTINATARIOS_NOTIFICACOES");
        }

    }
}