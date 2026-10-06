using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Dashboard_Atendimento : BaseModel
    {
        public virtual Int32 Atendimentos_Pendentes { get; set; }
        public virtual Int32 Total_Atendimentos_Mes_Usuario { get; set; }
        public virtual Int32 Total_Atendimentos_Mes_Geral { get; set; }
        public virtual Int32 Tempo_Medio_Atendimento { get; set; }
        public virtual Int32 Atendimentos_Pendentes_Equipe { get; set; }
        public virtual Int32 A_Iniciar { get; set; }
        public virtual Int32 Em_Atendimento { get; set; }
        public virtual Int32 Aguardando { get; set; }

        public virtual Int32 Risco_Perda_Prazo { get; set; }
        public virtual Int32 Prazo_Vencido { get; set; }
        public virtual Int32 Vencimento_Proximo { get; set; }
        public virtual Int32 Atencao { get; set; }
        public virtual Int32 Dentro_Prazo { get; set; }

        public virtual Boolean Chat { get; set; }
        public virtual Boolean Novidades_Sinapse { get; set; }
    }
}
