using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Natureza_ProducaoAnielMap : ClassMap<Natureza_ProducaoAniel>
    {
        public Natureza_ProducaoAnielMap()
        {
            Table("TB_NATUREZA_PRODUCAO");

            Id(x => x.Id);

            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Nao_Obriga_Agendamento, "NAO_OBRIGA_AGENDAMENTO");
            Map(x => x.QtdDias_GeraReincidencia, "QTDDIAS_GERAREINCIDENCIA");
            Map(x => x.TipoHabilidade, "TIPO_HABILIDADE").CustomType<TipoHabilidade>();
            Map(x => x.TipoPrioridade, "TIPO_PRIORIDADE").CustomType<Tipo_PrioridadeT5>();
            Map(x => x.GeraReincidencia, "REINCIDENCIA");

            Map(x => x.MetaReincidencia, "META_REINCIDENCIA");
            Map(x => x.MetaBaixaDentroPrazo, "META_BX_PRAZO");
            Map(x => x.MetaServicosProdutivos, "META_PRODUTIVA");

            Map(x => x.BasePrazo, "BASE_PRAZO").CustomType<BasePrazoEnum>();
            Map(m => m.IdDepartamentoPadrao, "ID_DEPARTAMENTO_PADRAO");
            Map(x => x.Departamento, "DEPARTAMENTO");
            //References(x => x.Departamento, "DEPARTAMENTO").ForeignKey("FK_DEPARTAMENTO");
        }
    }
}
