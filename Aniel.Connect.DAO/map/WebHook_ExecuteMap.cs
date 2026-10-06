using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class WebHook_ExecuteMap : ClassMapBase<WebHook_Execute>
    {
        public WebHook_ExecuteMap() : base()
        {
            Table("TB_WEBHOOK_EXECUTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_WEBHOOK_EXECUTE");

            References(u => u.WebHook, "ID_WEBHOOK").ForeignKey("FK_WEBHOOK_EXEC_WEB");

            Map(x => x.Type, "TYPE").CustomType<WebHook_Type>();
            Map(x => x.State, "STATE").CustomType<WebHook_ExecuteState>();

            Map(x => x.Request, "REQUEST").Length(5000);

            Map(x => x.Response, "RESPONSE").Length(5000);
            Map(x => x.Attempts, "ATTEMPTS");
        }
    }
}
