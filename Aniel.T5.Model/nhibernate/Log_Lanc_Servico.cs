using Aniel.T5.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Log_Lanc_Servico
    {
        public virtual Int32 Id { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Contrato { get; set; }

        public virtual DateTime Dia { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Valor_Ant { get; set; }
        public virtual String Alteracao { get; set; }
        public virtual DateTime? Data_Maxima { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Motivo_Exclusao { get; set; }
        public virtual String Valor_Atualizado { get; set; }

        public virtual String Obs_Cobranca_Status { get; set; }
        public virtual Int64? CodMotivoReagendamento { get; set; }
        public virtual String  ObsMotivoReagendamento { get; set; }
    }
}
