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
    public class FeriadoMap : ClassMapBase<Feriado>
    {
        public FeriadoMap() : base()
        {
            Table("TB_FERIADO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FERIADO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Data, "DATA");
            Map(x => x.Abrangencia, "ABRANGENCIA").CustomType<Abrangencia>();
            Map(x => x.Tipo_Feriado, "TIPO").CustomType<Tipo_Feriado>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Pais, "PAIS");

            Map(x => x.UF, "UF").Length(2);
            Map(x => x.Cidade, "CIDADE").Length(30);
        }
    }
}
