using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class AlteracaoVendaVendedor : BaseModel
    {
        public virtual Int64 id_venda { get; set; }

        public virtual Int64 id_supervisor { get; set; }

        public virtual Int64 id_vendedor_de { get; set; }

        public virtual Int64 id_vendedor_para { get; set; }

        public virtual Status_Alteracao_Vendedor status { get; set; }
    }
}
