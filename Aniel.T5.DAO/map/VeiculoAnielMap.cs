using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class VeiculoAnielMap : ClassMap<VeiculoAniel>
    {
        public VeiculoAnielMap()
        {
            Table("TB_VEICULO");

            Id(x => x.CodFrota, "COD_FROTA");
            Map(x => x.Placa, "PLACA").Length(10);
            Map(x => x.KmOdometro, "ULTIMO_KM_MOV").Length(10);

        }
    }
}
