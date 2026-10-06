using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    class SegmentoMap : ClassMap<Segmento>
    {
        public SegmentoMap()
        {
            Table("TB_SEGMENTO_SERVICO");

            Id(u => u.Codigo, "CODIGO");

            Map(x => x.Descricao, "DESCRICAO");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
