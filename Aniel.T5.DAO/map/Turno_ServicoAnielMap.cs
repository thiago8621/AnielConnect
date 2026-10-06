using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Turno_ServicoAnielMap : ClassMap<Turno_ServicoAniel>
    {
        public Turno_ServicoAnielMap()
        {
            Table("TB_TURNO_SERVICO");

            Id(x => x.Id);

            Map(x => x.Descricao, "DESCRICAO").Length(30);
            Map(x => x.Hora_Ini, "HORA_INI").CustomType("Time");
            Map(x => x.Hora_Fim, "HORA_FIM").CustomType("Time");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
