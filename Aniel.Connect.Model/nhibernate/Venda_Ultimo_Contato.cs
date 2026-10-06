using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Venda_Ultimo_Contato : BaseModel
    {
        public virtual Int64 Id_Venda { get; set; }

        public virtual Venda Venda { get; set; }

        public virtual DateTime Data_Ultimo_Contato { get; set; }

        public virtual String Forma_Contato { get; set; }

        public virtual String Observacao { get; set; }

        public virtual String Acao { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Valor_Atualizado { get; set; }

        public virtual String Status_Venda { get; set; }
        public virtual Int64 Id_Motivo_Desistencia_Venda { get; set; }
    }
}
