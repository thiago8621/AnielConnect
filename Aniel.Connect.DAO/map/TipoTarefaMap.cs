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
    public class TipoTarefaMap : ClassMapBase<TipoTarefa>
    {
        public TipoTarefaMap() : base()
        {
            Table("TB_TIPO_TAREFA");

            Id(x => x.Id).GeneratedBy.Sequence("GN_TIPO_TAREFA");
            Map(x => x.Codigo, "CODIGO");
            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
