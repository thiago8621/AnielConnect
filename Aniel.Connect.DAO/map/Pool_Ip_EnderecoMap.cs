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
    public class Pool_Ip_EnderecoMap : ClassMapBase<Pool_Ip_Endereco>
    {
        public Pool_Ip_EnderecoMap() : base()
        {
            Table("TB_POOL_IP_ENDERECO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_POOL_IP_ENDERECO");

            Map(x => x.Ip, "IP");
            Map(x => x.Status_Ip, "STATUS_IP").CustomType<Pool_Ip_Range_StatusEnum>();
            References(x => x.Pool_Ip, "ID_POOL_IP").NotFound.Ignore().Fetch.Join().ForeignKey("FK_POOL_IP_END_POOL");
            References(x => x.Login, "ID_LOGIN_RADIUS").ForeignKey("FK_POOL_IP_END_LOGIN_RADIUS");
        }
    }
}
