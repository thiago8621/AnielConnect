using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogEquipe_LocalidadeAniel : BaseLog
    {
        public virtual String Equipe { get; set; }

        public virtual String Cod_Escritorio { get; set; }

        public virtual String Cod_Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Nome_Equipe { get; set; }

        public virtual String Descricao_Escritorio { get; set; }

        public virtual String Descricao_Area { get; set; }

        public virtual String Descricao_Localidade { get; set; }

        public virtual Boolean Todas_Localidades { get; set; }
        public virtual Boolean Todas_Areas { get; set; }

    }
}
