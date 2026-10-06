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
    public class Elemento_RedeCampos : BaseModel
    {
      public virtual String Descricao{get;set;}
      public virtual String Valor{get;set;}
      public virtual String Id_Campo_Dinamico{get;set;}
      public virtual Elemento_Rede Elemento{get;set;}
      public virtual String Tipo{get;set;} 
    }

}
