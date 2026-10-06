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
    public class Log_LocalizacaoAniel : BaseModel
    {
        public virtual EquipeAniel Equipe { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Contrato { get; set; }

        public virtual DateTime? Data { get; set; }
        public virtual DateTime? DataHora_Servidor { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual DateTime? Hora_Fim { get; set; }

        public virtual StatusExecucaoAniel? Status { get; set; }

        public virtual Double? Direcao { get; set; }
        public virtual Double? Velocidade { get; set; }
        public virtual Double? Altitude { get; set; }

        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String TransferenciaMongo { get; set; }

    }
}
