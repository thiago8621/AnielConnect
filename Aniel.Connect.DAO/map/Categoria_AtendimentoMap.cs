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
    public class Categoria_AtendimentoMap : ClassMapBase<Categoria_Atendimento>
    {
        public Categoria_AtendimentoMap() : base()
        {
            Table("TB_CATEGORIA_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CATEGORIA_ATENDIMENTO");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
