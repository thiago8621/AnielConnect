using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class IntegracaoProtheusReverseAnielMap : ClassMap<IntegracaoProtheusReverseAniel>
    {
        public IntegracaoProtheusReverseAnielMap()
        {
            Table("TB_INTEGRACAO_PROTHEUS_REVERSE");

            Id(u => u.Id, "ID");

            Map(x => x.CodCt, "CODCT");
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.D3_Filial, "D3_FILIAL");
            Map(x => x.D3_Local, "D3_LOCAL");
            Map(x => x.Empresa, "EMPRESA");
        }
    }
}
