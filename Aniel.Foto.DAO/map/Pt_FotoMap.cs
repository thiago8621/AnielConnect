using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Pt_FotoMap : ClassMap<Pt_Foto>
    {
        public Pt_FotoMap() : base()
        {
            Table("TB_PT_FOTO");

            Id(u => u.Id, "ID").GeneratedBy.Identity();
            Map(x => x.Data, "DATA");
            Map(x => x.NumFre, "NUMFRE").Length(6);
            Map(x => x.Id_Mascacao, "ID_MARCACAO").Index("IDX_FOTO_MARCACAO");
            Map(x => x.Foto, "FOTO");
            Map(x => x.Id_Assinatura, "ID_ASSINATURA").Index("IDX_FOTO_ASSINATURA");
        }
    }
}
