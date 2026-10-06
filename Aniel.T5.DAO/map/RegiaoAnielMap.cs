using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class RegiaoAnielMap : ClassMap<RegiaoAniel>
    {
        public RegiaoAnielMap()
        {
            Table("TB_REGIAO");

            Id(x => x.Cod_Regiao, "COD_REGIAO");
            Map(x => x.Nome, "NOME").Length(50);
        }
    }
}
