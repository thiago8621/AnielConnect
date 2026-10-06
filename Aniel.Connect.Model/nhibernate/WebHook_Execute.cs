using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model
{
    public class WebHook_Execute : BaseModel
    {
        public virtual WebHook WebHook { get; set; }
        public virtual WebHook_Type Type { get; set; }
        public virtual WebHook_ExecuteState State { get; set; }
        public virtual String Request { get; set; }
        public virtual String Response { get; set; }
        public virtual Int32 Attempts { get; set; }
    }
}
