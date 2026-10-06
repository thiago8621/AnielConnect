using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class ConfigImportacaoDados
    {
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual long IdConfig { get; set; }
        public virtual int Planilha { get; set; }
        public virtual string CampoPlanilha { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ConfigImportacaoDados other = obj as ConfigImportacaoDados;

            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && IdConfig == other.IdConfig && Planilha == other.Planilha && CampoPlanilha == other.CampoPlanilha)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodCt + "|" + Projeto + "|" + IdConfig).GetHashCode();
        }

    }
}
