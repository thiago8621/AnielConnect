using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Centro_CustoMap : ClassMapBase<Centro_Custo>
    {
        public Centro_CustoMap() : base()
        {
            Table("TB_CENTRO_CUSTO");

            Id(x => x.Id);

            Map(x => x.Cod_Ccusto, "COD_CCUSTO");
            Map(x => x.Descricao, "DESCRICAO").Length(30);
        }
    }
}
