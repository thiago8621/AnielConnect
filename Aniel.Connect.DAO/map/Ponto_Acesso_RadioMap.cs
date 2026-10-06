using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{

    public class Ponto_Acesso_RadioMap : ClassMapBase<Ponto_Acesso_Radio>
    {

        public Ponto_Acesso_RadioMap() : base()
        {
            Table("TB_PONTO_ACESSO_RADIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PONTO_ACESSO_RADIO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Codigo_Auxiliar, "CODIGO_AUXILIAR");            
            Map(x => x.Ip, "IP");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            HasMany(x => x.GruposArea).KeyColumn("ID_PONTO_ACESSO_RADIO").Cascade.All().Inverse();
            Map(x => x.Mac, "MAC").Length(100);
        }
       
    }
}
