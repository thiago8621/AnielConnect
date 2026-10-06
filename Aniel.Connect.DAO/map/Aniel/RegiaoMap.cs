using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class RegiaoMap : ClassMapBase<Regiao>
    {
        public RegiaoMap() : base()
        {
            Table("TB_REGIAO");

            Id(x => x.Id);

            Map(x => x.Cod_Regiao, "COD_REGIAO");
            Map(x => x.Nome, "NOME").Length(50);
        }
    }
}
