using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class CampoImportacaoModel
    {
        public virtual long Id { get; set; }
        public virtual string CampoAniel { get; set; }
        public virtual string CampoPlanilha { get; set; }
        public virtual int Planilha { get; set; }
        public virtual char Interno { get; set; }
    }
}
