using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Codigo_EncerramentoAniel
    {
        public virtual String Cod_Encerramento { get; set; }
        public virtual String CodCt { get; set; }
        public virtual Int16 Cod_Tipo_Serv { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Int16 Status_Execucao { get; set; }

        public virtual String Encerra_OS_Apos_Baixa { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Codigo_EncerramentoAniel other = obj as Codigo_EncerramentoAniel;
            if (other == null)
                return false;
            if (Cod_Encerramento == other.Cod_Encerramento && CodCt == other.CodCt && Cod_Tipo_Serv == other.Cod_Tipo_Serv)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
