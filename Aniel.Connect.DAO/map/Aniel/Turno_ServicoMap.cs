using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Turno_ServicoMap : ClassMapBase<Turno_Servico>
    {
        public Turno_ServicoMap() : base()
        {
            Table("TB_TURNO_SERVICO");

            Id(x => x.Id);

            Map(x => x.Descricao, "DESCRICAO").Length(30);
        }
    }
}
