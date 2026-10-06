using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Pt_AssinaturaMap : ClassMap<Pt_Assinatura>
    {
        public Pt_AssinaturaMap() : base()
        {
            Table("TB_PT_ASSINATURA");

            Id(u => u.Id, "ID").GeneratedBy.Identity();
            Map(x => x.NumFre, "NUMFRE").Length(6);
            Map(x => x.Vinculo_Atual, "VINCULO_ATUAL");
            Map(x => x.Assinatura, "ASSINATURA");
            Map(x => x.DataHoraCriacao, "DATAHORACRIACAO");
        }
    }
}
