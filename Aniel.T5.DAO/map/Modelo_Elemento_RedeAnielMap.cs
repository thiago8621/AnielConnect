using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
namespace Aniel.T5.DAO.map
{
    public class Modelo_Elemento_RedeAnielMap : ClassMap<Modelo_Elemento_Rede>
    {
        public Modelo_Elemento_RedeAnielMap()
        {
            Table("TB_MODELO_ELEMENTO_REDE");

            Id(t => t.Id, "ID").GeneratedBy.Sequence("GN_MOD_ELEMENTO_REDE");

            Map(t => t.Descricao, "DESCRICAO").Length(50);

            Map(t => t.Status, "STATUS").CustomType<Status_Default>();

        }
    }
}
