using Sinapse.Model.nhibernate;
using System;
namespace Aniel.T5.Model.nhibernate

{
    public class UsuarioTecnicoRoteirizacao : BaseModel
    {
        public virtual string Usuario { get; set; }
        public virtual string Equipe { get; set; }
    }
}
