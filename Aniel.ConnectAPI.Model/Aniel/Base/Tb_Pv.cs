using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel.Base
{
    public class Tb_Pv
    {
        public Int64 Id { get; set; }
        public String Acao { get; set; }
        public String Usuario { get; set; }
        public DateTime? Dia { get; set; }
        public DateTime? Hora { get; set; }
        public String Executado { get; set; }
        public DateTime? Dia_Execucao { get; set; }
        public DateTime? Hora_Execucao { get; set; }
        public String Log { get; set; }
    }
}
