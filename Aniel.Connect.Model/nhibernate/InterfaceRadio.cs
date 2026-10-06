using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class InterfaceRadio : BaseModel
    {
        public virtual Ponto_Acesso_Radio Ponto_Acesso_Radio { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
    }
}

