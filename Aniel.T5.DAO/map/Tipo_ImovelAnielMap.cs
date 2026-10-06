using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class Tipo_ImovelAnielMap : ClassMap<Tipo_ImovelAniel>
    {
        public Tipo_ImovelAnielMap()
        {
            Table("TB_TIPO_IMOVEL");

            Id(x => x.Cod_Tipo_Imovel, "COD_TIPO_IMOVEL");
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Digito, "DIGITO");
        }
    }
}
