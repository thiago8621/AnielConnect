using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class ComboMap : ClassMapBase<Combo>
    {
        public ComboMap()
        {
            Table("TB_COMBO");

            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_COMBO");

            Map(p => p.Descricao, "DESCRICAO");
            Map(p => p.DetalhesCombo, "DETALHES_COMBO").Length(6000);
            Map(p => p.Status, "STATUS").CustomType<Status_Default>();
            Map(p => p.EmiteContrato, "EMITE_CONTRATO");
            Map(p => p.CodigoIntegracao, "CODIGO_INTEGRACAO");
            Map(p => p.CodigoIntegracaoAuxiliar, "CODIGO_INTEGRACAO_AUXILIAR");
            Map(p => p.CodigoTipoComboIntegracao, "CODIGO_TIPO_PLANO_INTEGRACAO");
            Map(p => p.CodigoIntegracaoProcesso, "CODIGO_INTEGRACAO_PROCESSO");
            References(p => p.ModeloContrato, "ID_MODELO_CONTRATO").ForeignKey("FK_COMBO_MODELO_CT");
            HasMany(p => p.PlanosProdutos).KeyColumn("ID_COMBO").Cascade.All().Inverse().Not.LazyLoad();
        }
    }
}
