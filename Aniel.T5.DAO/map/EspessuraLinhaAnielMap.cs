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
    public class EspessuraLinhaAnielMap: ClassMap<Espessura_Linha>
    {
        public EspessuraLinhaAnielMap() 
        {
            Table("TB_ESPESSURA_LINHA");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_ESPESSURA_LINHA");
            
            Map(x=>x.Descricao,"DESCRICAO").Length(50);

        }
    }
}
