using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class BlacklistMap  : ClassMapBase<Blacklist>
    {
        public BlacklistMap()
        {
            Table("TB_BLACKLIST");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BLACKLIST");
            
            Map(x => x.Numero_Origem, "NUMERO_ORIGEM").Length(100);

            Map(x => x.Numero_Destino, "NUMERO_DESTINO").Length(100);
            Map(x => x.Observacao, "OBSERVACAO").Length(100);

            References(x => x.Plano, "ID_PLANO").ForeignKey("FK_PLANO");            
        }
    }
}
