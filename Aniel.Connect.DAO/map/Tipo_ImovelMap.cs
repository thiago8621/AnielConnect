using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Tipo_ImovelMap : ClassMapBase<Tipo_Imovel>
    {
        public Tipo_ImovelMap() : base()
        {
            Table("TB_TIPO_IMOVEL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_IMOVEL");

            Map(x => x.Cod_Tipo_Imovel, "COD_TIPO_IMOVEL").Index("IDX_COD_TIPO_IMOVEL");
            Map(x => x.Descricao, "DESCRICAO");
        }
    }
}
