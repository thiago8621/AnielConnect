using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class FluxoAtendimento : BaseModel
    {
        public virtual string Codigo { get; set; }
        public virtual Int64 Sequencia { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Boolean AlterarSetor_Responsavel { get; set; }
        public virtual Boolean Etapa_CriaOS { get; set; }
        public virtual Boolean Baixar_OS { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual string Setor_Departamento { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Cod_Encerramento { get; set; }
        public virtual Double Tempo_Exc { get; set; }
        public virtual bool Encerra_Atendimento { get; set; }
        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }

        public virtual bool Etapa_Decisoria { get; set; }

        public virtual String Proxima_Etapa { get; set; }
        public virtual String Motivo_Status_Atendimento { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Status_AtendimentoEtapa? StatusAtendimento { get; set; }

    }
}
