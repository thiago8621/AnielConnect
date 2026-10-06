using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class CampoImportacaoMantConfModel
    {
        public virtual long Id { get; set; }
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string IdConfig { get; set; }
        public virtual int Planilha { get; set; }
        public virtual char Interno { get; set; }
        public virtual long GerarSequencial { get; set; }
    }
}
