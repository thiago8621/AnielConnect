using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class WebHookMap : ClassMapBase<WebHook>
    {
        public WebHookMap() : base()
        {
            Table("TB_WEBHOOK");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_WEBHOOK");

            Map(x => x.Url, "URL");
            Map(x => x.Type, "TYPE").CustomType<WebHook_Type>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
