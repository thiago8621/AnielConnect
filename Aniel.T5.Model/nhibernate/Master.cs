using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class Master
    {
        public virtual Int32 Id { get; set; }

        public virtual Enum_Tipo_Sla_ SLA_Data_Abertura { get; set; }

        public virtual Int32? SLA_Servico_Padrao { get; set; }

        public virtual String Config_Doc { get; set; }

        public virtual String Usa_Roteirizacao { get; set; }

        public virtual String Encerrar_Atendimento_OS { get; set; }
        public virtual String Deslocamento_Dinamico { get; set; }
        public virtual String ConfigParalisaOS { get; set; }
        public virtual string Autoriza_Baixa_Servico_Backoffice { get; set; }
        public virtual int? Tipo_Integracao_CTO { get; set; }
    }
}
