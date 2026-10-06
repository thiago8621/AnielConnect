using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Tipo_Checkpoint_VendedorMap : ClassMapBase<Tipo_Checkpoint_Vendedor>
    {
        public Tipo_Checkpoint_VendedorMap() : base()
        {
            Table("TB_TIPO_CHECKPOINT_VENDEDOR");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_CHECKPOINT_VENDEDOR");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
