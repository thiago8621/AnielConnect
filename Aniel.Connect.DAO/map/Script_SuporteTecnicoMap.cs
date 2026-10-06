using Aniel.Connect.Model.nhibernate;
using NHibernate.Criterion;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Script_SuporteTecnicoMap : ClassMapBase<Script_SuporteTecnico>
    {
        public Script_SuporteTecnicoMap() : base()
        {
            Table("TB_SCRIPT_SUPORTETECNICO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SCRIPT_SUPORTETECNICO");

            Map(x => x.Descricao, "DESCRICAO").Length(50);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            HasMany(x => x.Perguntas).KeyColumn("ID_SCRIPT").Cascade.All().Inverse();
        }
    }
}
