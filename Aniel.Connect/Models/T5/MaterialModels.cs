using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    

    public class MaterialListaModel : ObjComboResult<String>
    {
        public String Unidade { get; set; }
        public Double? Valor { get; set; }
        public String SubCod { get; set; }
        public String SubCod_Exclusivo { get; set; }
    }

    public class MaterialConfigCombo
    {
        public String classificacao { get; set; }
        public String complementar { get; set; }
    }

    public class ConversaoSerial
    {
        public string codMat { get; set; }
        public string mac_de { get; set; }
        public string mac_para { get; set; }
        public string convertOlt { get; set; }
    }
}