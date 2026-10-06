using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Area_Cobertura : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Situacao { get; set; }
        public virtual String Cor { get; set; }
        public virtual String Coordenadas { get; set; }
        public virtual Tipo_Poligono Tipo { get; set; }
    }
}
