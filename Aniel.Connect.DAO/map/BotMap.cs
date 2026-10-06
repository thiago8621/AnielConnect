using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class BotMap : ClassMapBase<Bot>
    {
        public BotMap() : base()
        {
            Table("TB_BOT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BOT");

            Map(x => x.Avatar, "AVATAR").Length(100);
            Map(x => x.Nome, "NOME").Length(100);
            Map(x => x.TextoInvalido, "TEXTO_INVALIDO").Length(400);
            Map(x => x.Transferencia, "TRANSFERENCIA");
            Map(x => x.Minutos, "MINUTOS");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Habilitado, "HABILITADO").CustomType<Habilitado_Bot>();
            //HasMany(x => x.Eventos).KeyColumn("ID_BOT").Cascade.All().Inverse();
            HasMany(x => x.Eventos).KeyColumn("ID_BOT").Cascade.All().Inverse().Not.LazyLoad();
        }
    }
}
