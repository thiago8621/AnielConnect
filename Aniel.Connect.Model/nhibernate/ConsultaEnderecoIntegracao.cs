using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class ConsultaEnderecoIntegracao : BaseModel
    {
        public virtual Int32 Id { get; set; }
        public virtual String Cod_Logradouro { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Cod_Bairro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Cod_Cidade { get; set; }
        public virtual String Cidade { get; set; }

    }
}
