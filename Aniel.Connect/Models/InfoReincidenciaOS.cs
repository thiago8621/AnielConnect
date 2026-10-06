using System;

namespace Aniel.Connect.Models
{
    public class InfoReincidenciaOS
    {
        public string EquipeAnteriorNome {get; set; }
        public string FornecedorAnterior { get; set; } 
        public string FornecedorAtual { get; set; }
        public bool IsReincidencia { get; set; }
    }
}