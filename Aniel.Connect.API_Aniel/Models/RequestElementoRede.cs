using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.API_Aniel.Models
{
    public class RequestTipo_Modelo
    {
        public long? Id_Tipo { get; set; }
        public long? Id_Modelo { get; set; }
        public long? Capacidade { get; set; }
        public String Fabricante { get; set; }
        public bool E_Caixa { get; set; }
        public Int64 Id_Icone { get; set; }

    }
}
