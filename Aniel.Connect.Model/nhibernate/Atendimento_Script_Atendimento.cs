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
    /// <summary>
    ///     Entidade que liga as questões de um script a um atendimento
    /// </summary>
    public class Atendimento_Script_Atendimento : BaseModel
    {
        public virtual Atendimento Atendimento { get; set; }
        public virtual Script_Atendimento Script_Atendimento { get; set; }
        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }
        public virtual ScriptAtendimentoQuestion ScriptAtendimentoQuestion { get; set; }

        public virtual String Pergunta { get; set; }

        public virtual String Resposta { get; set; }

        public virtual String Observacao { get; set; }
    }
}
