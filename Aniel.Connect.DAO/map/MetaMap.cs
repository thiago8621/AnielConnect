using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class MetaMap : ClassMapBase<Meta>
    {

        public MetaMap() : base()
        {
            Table("TB_META");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_META");

            Map(x => x.Descricao , "DESCRICAO");
            Map(x => x.Ano , "ANO");

            Map(x => x.Status, "STATUS");

            Map(x => x.Janeiro, "JANEIRO");
            Map(x => x.Fevereiro, "FEVEREIRO");
            Map(x => x.Marco, "MARCO");
            Map(x => x.Abril, "ABRIL");
            Map(x => x.Maio, "MAIO");
            Map(x => x.Junho, "JUNHO");
            Map(x => x.Julho, "JULHO");
            Map(x => x.Agosto, "AGOSTO");
            Map(x => x.Setembro, "SETEMBRO");
            Map(x => x.Outubro, "OUTUBRO");
            Map(x => x.Novembro, "NOVEMBRO");
            Map(x => x.Dezembro, "DEZEMBRO");

            //References(x => x.Id_Filial, "ID_FILIAL");
        }

    }
}
