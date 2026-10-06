using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogEquipe_NaturezaAniel : BaseLog
    {
        public virtual String Equipe { get; set; }
        public virtual String Nome_Equipe { get; set; }
        public virtual Int32 Id_Natureza_ProducaoAniel { get; set; }
        public virtual String Descricao_Natureza { get; set; }

    }
}
