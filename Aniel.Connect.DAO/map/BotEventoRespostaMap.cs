using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class BotEventoRespostaMap : ClassMapBase<BotEventoResposta>
    {
        public BotEventoRespostaMap() : base()
        {
            Table("TB_BOT_EVENTO_RES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BOT_EVENTO_RES");
            Map(x => x.Descricao, "DESCRICAO");
            References(x => x.BotEvento, "ID_BOT_EVENTO").ForeignKey("FK_EVENTO_RES_BOT");
            References(x => x.ProximoEvento, "ID_BOT_PROX_EV").ForeignKey("FK_PROX_EV_RES_BOT");
        }

    }
}