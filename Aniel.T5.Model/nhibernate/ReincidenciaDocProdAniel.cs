using Aniel.Connect.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class ReincidenciaDocProdAniel
    {
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string NumObra { get; set; }
        public virtual int NumDoc { get; set; }

        /// <summary>
        ///     Propriedade do NHibernate somente para leitura
        /// </summary>
        public virtual Documento_ProducaoAniel ServicoReincidente { get; set; }

        public virtual string CodCtAnterior { get; set; }
        public virtual string ProjetoAnterior { get; set; }
        public virtual string NumObraAnterior { get; set; }
        public virtual int NumDocAnterior { get; set; }

        /// <summary>
        ///     Propriedade do NHibernate somente para leitura
        /// </summary>
        public virtual Documento_ProducaoAniel ServicoAnterior { get; set; }

        public virtual DateTime Data { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            var other = (ReincidenciaDocProdAniel)obj;

            if (this.CodCt == other.CodCt && 
                this.Projeto == other.Projeto && 
                this.NumObra == other.NumObra && 
                this.NumDoc == other.NumDoc)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (this.CodCt + "|" + this.Projeto + "|" + this.NumObra + "|" + this.NumDoc).GetHashCode();
        }
    }
}
