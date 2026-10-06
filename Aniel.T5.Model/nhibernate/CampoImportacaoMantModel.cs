using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class CampoImportacaoMantModel
    {
        public virtual long Id { get; set; }
        public virtual string NomeCampoAniel { get; set; }
        public virtual string ApelidoCampoAniel { get; set; }
        public virtual string CampoPlanilha { get; set; }
        public virtual int Planilha { get; set; }
        public virtual char Interno { get; set; }
        public virtual string ConfigPasta { get; set; }
    }
}
