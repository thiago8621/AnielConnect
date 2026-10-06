using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    

    public class Caderno_EncargosListaModel : ObjComboResult<String>
    {
        public String Unidade_Custo { get; set; }
        public String Unidade { get; set; }
        public Double? Quantidade { get; set; }
        public String Classe_MO { get; set; }
    }

}