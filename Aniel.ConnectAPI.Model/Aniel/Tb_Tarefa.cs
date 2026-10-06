using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.ConnectAPI.Model.Aniel
{
    public class Tb_Tarefa
    {
        public String Codct { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public String Tarefa { get; set; }
        public Int16 Status_Auditor { get; set; }
        public Int16 Status_Supervisor { get; set; }
    }
}
