using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Natureza_ProducaoMap : ClassMapBase<Natureza_Producao>
    {
        public Natureza_ProducaoMap() : base()
        {
            Table("TB_NATUREZA_PRODUCAO");

            Id(x => x.Id);

            Map(x => x.Descricao, "DESCRICAO").Length(30);
        }
    }
}
