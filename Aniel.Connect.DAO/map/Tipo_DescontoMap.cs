using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Tipo_DescontoMap : ClassMapBase<Tipo_Desconto>
    {
        public Tipo_DescontoMap() : base()
        {
            Table("TB_TIPO_DESCONTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_DESCONTO");

            Map(x => x.Codigo, "CODIGO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Formato, "FORMATO").CustomType<Formato_Tipo_Desconto>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

        }


    }
}
