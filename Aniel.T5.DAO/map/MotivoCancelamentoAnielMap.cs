using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class MotivoCancelamentosAnielMap : ClassMap<MotivoCancelamentoAniel>
    {
        public MotivoCancelamentosAnielMap() : base()
        {
            Table("TB_MOTIVOS_CANCELAMENTOS");

            Id(x => x.Codigo, "ID").GeneratedBy.Sequence("GN_MOTIVOS_CANCELAMENTOS");

            Map(x => x.Descricao, "DESCRICAO").Length(110);
            Map(x => x.Status, "STATUS");
            Map(x => x.Cod_Auxiliar, "COD_AUXILIAR");  // Adicionando o campo Código Auxiliar
        }
    }
}
