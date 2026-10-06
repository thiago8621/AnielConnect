using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Motivo_EncerramentoMap : ClassMapBase<Motivo_Encerramento>
    {
        public Motivo_EncerramentoMap() : base()
        {
            Table("TB_MOTIVO_ENCERRAMENTO");

            Id(x => x.Id);

            Map(x => x.Status, "STATUS");

            Map(x => x.Descricao, "DESCRICAO").Length(50);
                       
        }
    }
}
