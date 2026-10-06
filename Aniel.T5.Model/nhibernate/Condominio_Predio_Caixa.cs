using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Condominio_Predio_Caixa : BaseModel
    {
        public virtual Elemento_Rede Id_Caixa { get; set; }
        public virtual Int64 Id_Condominio_Predio { get; set; }     
    }

}
