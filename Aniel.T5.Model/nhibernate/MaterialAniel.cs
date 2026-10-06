using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class MaterialAniel
    {
        public virtual String CodMat { get; set; }
        /// <summary>
        ///     Código do Perfil de Ativação
        /// </summary>
        public virtual String CodMatIntegra { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Unid { get; set; }
        public virtual Double? Valor { get; set; }
        public virtual String Classificacao { get; set; }

        public virtual String Familia { get; set; }
        public virtual String SubCod { get; set; }
        public virtual String SubCod_Exclusivo { get; set; }

        public virtual String Gerar_Saldo_Auto { get; set; }

        public virtual String Tipo { get; set; }
        public virtual Int16? Grupo { get; set; }

        public virtual String ENVIA_RESERV { get; set; }

        public virtual DateTime? Data_Criacao { get; set; }
        public virtual DateTime? Hora_Criacao { get; set; }
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Desc_Auxiliar { get; set; }
        public virtual TipoEquipamento? TipoEquipamento { get; set; }
        public virtual string AtivacaoSinal { get; set; }
        public virtual string CodMat_Ret { get; set; }
        public virtual string CodMat_Det { get; set; }
        public virtual string CodMat_Reap { get; set; }
        public virtual string CodMat_Defeito { get; set; }
        public virtual string CodMat_Sucata { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MaterialAniel other = obj as MaterialAniel;
            if (other == null)
                return false;
            if (CodMat == other.CodMat)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodMat).GetHashCode();
        }
    }

   
}
