using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;


namespace Aniel.T5.Model.nhibernate
{
    public class Modelo_Elemento_Rede : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
    }

}

