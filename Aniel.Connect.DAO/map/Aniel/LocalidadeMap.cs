using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class LocalidadeMap : ClassMapBase<Localidade>
    {
        public LocalidadeMap() : base()
        {
            Table("TB_LOCALIDADE");

            Id(x => x.Id);

            Map(x => x.LocalidadeStr, "LOCALIDADE").Length(30);
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");

            References(x => x.Area, "ID_AREA");
        }
    }
}
