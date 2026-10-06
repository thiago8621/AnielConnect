using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class BotEventoAnexoMap : ClassMapBase<BotEventoAnexo>
    {
        public BotEventoAnexoMap() : base()
        {
            Table("TB_BOT_EVENTO_ANEXO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BOT_EVENTO_ANEXO");
            Map(x => x.Nome, "NOME");
            Map(x => x.Extensao, "EXTENSAO");
            Map(x => x.Url, "URL");
            Map(x => x.Tipo, "TIPO").CustomType<Tipo_Bot_Evento_Anexo>();
            References(x => x.BotEvento, "ID_BOT_EVENTO").ForeignKey("FK_EVENTO_ANEXO_BOT");
        }

    }
}