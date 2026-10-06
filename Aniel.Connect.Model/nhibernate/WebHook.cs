using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class WebHook: BaseModel
    {
        public virtual String Url { get; set; }
        public virtual WebHook_Type Type { get; set; }
        public virtual Status_Default Status { get; set; }
    }
}
