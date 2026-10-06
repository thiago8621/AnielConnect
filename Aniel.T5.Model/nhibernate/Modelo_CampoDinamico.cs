using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model;

namespace Aniel.T5.Model.nhibernate
{
    public class Modelo_CampoDinamico : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Tipo_CampoModelo Tipo { get; set; }
        public virtual bool Gravado { get; set; }
        public virtual List<Opcoes_CampoDinamico> Opcoes { get; set; }
        public virtual Modelo_Elemento_Rede Modelo { get; set; }

    }

}