using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class EscritorioAnielMap : ClassMap<EscritorioAniel>
    {
        public EscritorioAnielMap()
        {
            Table("TB_ESCRITORIO");

            Id(x => x.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(x => x.Descricao, "DESCRICAO").Length(70);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
