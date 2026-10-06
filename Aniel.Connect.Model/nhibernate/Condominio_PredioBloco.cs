using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{

    public class Condominio_PredioBloco : BaseModel
    {
        public virtual Condominio_Predio Condominio { get; set; }        
        public virtual String Descricao { get; set; }        
        public virtual Status_Default? Status { get; set; }            
    }
}

