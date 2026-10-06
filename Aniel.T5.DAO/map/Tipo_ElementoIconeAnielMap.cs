using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using Sinapse.Model.enumerado;

namespace Aniel.T5.DAO.map
{
    public class Tipo_ElementoIconeAnielMap : ClassMap<Tipo_ElementoIcone>
    {
        public Tipo_ElementoIconeAnielMap()
        {
            Table("TB_TIPO_ELEMENTO_ICONE");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ELEMENTO_ICONE");
            
            Map(x=>x.Descricao,"DESCRICAO").Length(255);
            Map(x => x.Tipo, "TIPO").Length(20);
            Map(x => x.Url, "URL").Length(255);
            Map(x => x.Arquivo, "ARQUIVO").Length(5000);
            Map(x => x.Caminho, "CAMINHO").Length(5000);
            Map(x => x.Padrao_Connect, "PADRAO_CONNECT").CustomType<Padrao>();
        }
    } 
}
