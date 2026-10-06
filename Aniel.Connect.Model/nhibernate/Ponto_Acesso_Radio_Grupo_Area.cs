using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Ponto_Acesso_Radio_Grupo_Area : BaseModel
    {
      
            public virtual string Cod_Escritorio { get; set; }
            public virtual string Cod_Area { get; set; }
            public virtual String Desc_Escritorio { get; set; }
            public virtual String Desc_Area { get; set; }
            public virtual Ponto_Acesso_Radio Ponto_Acesso_Radio { get; set; }
            public virtual Status_Default Status { get; set; }

    }
}
