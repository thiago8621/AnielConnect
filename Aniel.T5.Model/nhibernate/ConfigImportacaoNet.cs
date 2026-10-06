using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class ConfigImportacaoNet
    {
        public virtual int Id { get; set; }
        public virtual string CampoAniel { get; set; }
        public virtual string ApelidoAniel { get; set; }
        public virtual string CampoPlanilha { get; set; }
        public virtual bool Planilha { get; set; }
    }
}
