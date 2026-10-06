using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_Contato : BaseModel
    {
        public virtual Cliente Cliente { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Email { get; set; }
        public virtual String Informacao_Adicional { get; set; }
        public virtual Tipo_Contato_Cliente? Tipo_Contato { get; set; }
        public virtual string NomeContato { get; set; }

    }
}
