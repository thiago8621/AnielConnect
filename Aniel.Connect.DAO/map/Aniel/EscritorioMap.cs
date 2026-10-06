using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class EscritorioMap : ClassMapBase<Escritorio>
    {
        public EscritorioMap() : base()
        {
            Table("TB_ESCRITORIO");

            Id(x => x.Id);

            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(x => x.Descricao, "DESCRICAO").Length(70);
        }
    }
}
