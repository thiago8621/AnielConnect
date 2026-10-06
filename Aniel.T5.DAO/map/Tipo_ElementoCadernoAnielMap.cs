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
   public class Tipo_ElementoCadernoAnielMap: ClassMap<Tipo_ElementoCaderno>
    {
        public Tipo_ElementoCadernoAnielMap()
        {
            Table("TB_TIPO_ELEMENTO_CADERNO");

            Id(x=>x.Id,"ID").GeneratedBy.Sequence("GN_TB_TIPO_ELEMENTO_CADERNO_ID");

            Map(x=>x.Unidade_Custo,"UNIDADE_CUSTO").Length(10);
            Map(x => x.Codigo, "CODIGO").Length(20);

            References(x=>x.Tipo,"TIPO").ForeignKey("FK_TIPO_ELEMENTO_TIPO"); 
        }
    }
}
