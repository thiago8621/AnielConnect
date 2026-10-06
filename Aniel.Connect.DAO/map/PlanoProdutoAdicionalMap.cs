using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class PlanoProdutoAdicionalMap : ClassMap<PlanoProdutoAdicional>
    {
        public PlanoProdutoAdicionalMap()
        {
            Table("TB_PLANO_PRODUTO_ADICIONAL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("Plano_Produto_Adicional");

            Map(x => x.Descricao, "DESCRICAO").Length(600);

            References(x => x.Id_Plano, "ID_PLANO").ForeignKey("FK_ID_PLANO");
            References(x => x.Id_Produto, "ID_PRODUTO").ForeignKey("FK_ID_PRODUTO");
            Map(x => x.Id_Plano_Produto_Adicional, "ID_PLANOS_ADICIONAIS");
        }
    }
}
