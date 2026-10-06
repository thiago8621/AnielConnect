using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Documento_Producao : BaseModel
    {
        public virtual Projeto Projeto { get; set; }
        public virtual Equipe Equipe { get; set; }

        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        public virtual String Num_RVT { get; set; }

        public virtual String Tarefa { get; set; }

        public virtual Escritorio Escritorio { get; set; }
        public virtual Area Area { get; set; }
        public virtual Localidade Localidade { get; set; }

        public virtual Codigo_Encerramento Codigo_Encerramento { get; set; }

        public virtual Tipo_Servico_Equipe Tipo_Servico_Equipe { get; set; }
        public virtual Sub_Tipo_Servico Sub_Tipo_Servico { get; set; }

        public virtual DateTime? Data_Agendamento { get; set; }
        public virtual DateTime? Data_Producao { get; set; }
        public virtual DateTime? Hora_Producao { get; set; }

        public virtual String Observacao { get; set; }

        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cep { get; set; }

        public virtual Double? Latitude_Baixa { get; set; }
        public virtual Double? Longitude_Baixa { get; set; }
    }
}
