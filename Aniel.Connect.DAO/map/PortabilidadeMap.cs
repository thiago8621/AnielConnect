using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class PortabilidadeMap : ClassMap<Portabilidade>
    {
        public PortabilidadeMap() : base()
        {
            Table("TB_PORTABILIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PORTABILIDADE_CODIGO");
            Map(param => param.Dia_Previsto_Portabilidade, "DIA_PORTABILIDADE");
            Map(param => param.Numero_Sip, "NUM_SIP");
            Map(param => param.NumeroDid_Portabilidade, "NUMERO_DID_PORTABILIDADE");
            Map(param => param.Status, "STATUS").CustomType<Status_Portabilidade>();
            //Map(param => param.Did_Substituido, "DID_SUBSTITUIDO");

            //FK'S
            References(param => param.Cliente, "ID_CLIENTE").ForeignKey("FK_PORTABILIDADE_CLIENTE");
            References(param => param.Did_Substituido, "ID_DID_SUBSTITUIDO").ForeignKey("FK_PORTABILIDADE_DID");
            References(param => param.CircuitoSelecionado, "ID_CircuitoSelecionado").ForeignKey("FK_PORTABILIDADE_CIRCUITO");

            //Localização
            Map(param => param.Pais, "PAIS");
            Map(param => param.UF, "UF").Length(2);
            Map(param => param.Cidade, "CIDADE").Length(30);

        }
    }
}
