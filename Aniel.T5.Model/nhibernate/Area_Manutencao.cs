using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model;

namespace Aniel.T5.Model.nhibernate

{
    public class Area_Manutencao: BaseModel
    {

        public virtual String Descricao { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
     
        public virtual Elemento_Rede Elemento_de_Rede { get; set; }
        public virtual DateTime? Data_HoraInicial { get; set; }
        public virtual DateTime? Data_HoraFinal { get; set; }

        public virtual Status_Default Status { get; set; }
    }
}
