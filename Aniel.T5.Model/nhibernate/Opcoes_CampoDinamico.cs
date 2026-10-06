using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;


namespace Aniel.T5.Model.nhibernate
{
    public class Opcoes_CampoDinamico:BaseModel
    {
        public virtual Modelo_CampoDinamico Campo { get; set; }
        public virtual String Resposta { get; set; }



    }
}
