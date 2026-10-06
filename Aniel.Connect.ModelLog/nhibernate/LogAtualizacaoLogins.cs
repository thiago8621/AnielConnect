using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class LogAtualizacaoLogins : BaseModel
    {
        public virtual string Login { get; set; }
        public virtual string Data { get; set; }
        public virtual string Hora { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Detalhe { get; set; }
        public virtual string Sucesso { get; set; }
    }
}