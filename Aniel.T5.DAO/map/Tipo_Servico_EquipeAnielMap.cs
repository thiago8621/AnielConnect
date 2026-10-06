using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class Tipo_Servico_EquipeAnielMap : ClassMap<Tipo_Servico_EquipeAniel>
    {
        public Tipo_Servico_EquipeAnielMap()
        {
            Table("TB_TIPO_SERVICO_EQUIPE");

            Id(x => x.Cod_Tipo_Serv, "COD_TIPO_SERV");
            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Descricao, "DESCRICAO").Length(150);
            Map(x => x.Descricao_Aux, "DESCRICAO_AUX").Length(50);
            
            Map(x => x.Cod_Natureza, "COD_NATUREZA");
            Map(x => x.CodigoIntegrator, "CODIGO_INTEGRATOR").Length(10);
            References(x => x.Natureza, "COD_NATUREZA").LazyLoad().ReadOnly();
            
            Map(x => x.Exige_Manutencao, "EXIGE_MANUTENCAO");            
            Map(m => m.Exige_Pagamento_Instalacao, "EXIGE_PAGAMENTO_INSTALACAO");
            Map(m => m.Mudanca_Endereco, "MUDANCA_ENDERECO");
            Map(m => m.TempoLimiteEncaixe, "TEMPO_LIMITE_ENCAIXE");
        }
    }
}
