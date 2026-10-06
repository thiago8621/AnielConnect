using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Assinatura_Termo_Adesao_Map : ClassMap<Assinatura_Termo_Adesao>
    {
        public Assinatura_Termo_Adesao_Map(): base()
        {
            Table("TB_ASSINATURA_TERMO_ADESAO");

            Id(u => u.Id_Venda, "ID_VENDA").GeneratedBy.Assigned();
            Map(x => x.Assinatura, "ASSINATURA");
        }
    }
}