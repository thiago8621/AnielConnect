using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model
{
    public class ClienteContratoModel
    {
        public long IdVenda { get; set; }
        public long Id { get; set; }
        public string CodIntegracao { get; set; }        
        public string NumeroContrato { get; set; }        
        public long IdClientePlanoProduto { get; set; }
    }
}
