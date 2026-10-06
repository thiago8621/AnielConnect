using System;

namespace Aniel.Connect.Repository.Model
{
    public class DocProdBaixa
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Data_Ini_Atend { get; set; }
        public virtual String Data_Fim_Atend { get; set; }
        public virtual String Hora_Ini_Atend { get; set; }
        public virtual String Hora_Fim_Atend { get; set; }
        public virtual String CaixaFTTH { get; set; }
        public virtual String PortaFTTH { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String BaixaOsProdDuplicada { get; set; }

        public DateTime? DataHora_Ini_Atend => (String.IsNullOrEmpty(Hora_Ini_Atend) || String.IsNullOrEmpty(Data_Ini_Atend)) ? (DateTime?)null : Convert.ToDateTime($"{Data_Ini_Atend} {Hora_Ini_Atend}");

        public DateTime? DataHora_Fim_Atend => (String.IsNullOrEmpty(Hora_Fim_Atend) || String.IsNullOrEmpty(Data_Fim_Atend)) ? (DateTime?)null : Convert.ToDateTime($"{Data_Fim_Atend} {Hora_Fim_Atend}");
    }
}
