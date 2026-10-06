

using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class DocTermoAnexo : BaseModel
    {
        public virtual String Nome { get; set; }
        public virtual String Extensao { get; set; }
        public virtual Venda Venda { get; set; }
        public virtual TrocaTitularidade TrocaTitularidade { get; set; }
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual Tipo_Doc_Termo_Anexo TipoDocumento { get; set; }
        public virtual Status_Doc_Aceite Status { get; set; }
        public virtual Lado_Doc_Aceite Lado { get; set; }
        public virtual string Url { get; set; }
    }
}
