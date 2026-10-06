using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model
{
    public class DocProducaoBaixaAutomatica
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Observacao { get; set; }
        public virtual DateTime DataCriacao { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String Equipe { get; set; }
        public virtual DateTime? Data_Ini_Atend { get; set; }
        public virtual DateTime? Data_Fim_Atend { get; set; }
        public virtual String CaixaFTTH { get; set; }
        public virtual String PortaFTTH { get; set; }
        public virtual String Usuario { get; set; }
        public virtual Int64? StatusExecucaoAtual { get; set; }
        public virtual char EncerraOSAuto { get; set; }
        public virtual int DiasEncerramentoAuto { get; set; }
        public virtual String CodEncerramentoAuto { get; set; }

        public virtual String StatusExecucaoFinal { get; set; }

    }
}
