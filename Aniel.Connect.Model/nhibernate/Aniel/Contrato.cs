using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Contrato : BaseModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Fornec_Mat_Padrao { get; set; }
        public virtual String TipoSaldo { get; set; }
        public virtual String TipoSaldoAplic { get; set; }

        public virtual Projeto Projeto_Regulador { get; set; }

        public virtual Filial Filial { get; set; }
    }
}
