using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{

    public class Tipo_ElementoIcone : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Url { get; set; }
        public virtual String Arquivo { get; set; }
        public virtual String Caminho { get; set; }
        public virtual Padrao Padrao_Connect { get; set; }
    } 

}
