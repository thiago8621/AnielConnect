using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Requisicao_MovimentoRmdm
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String SeqRmdm { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String CodCpl { get; set; }
        public virtual Int64? Qtde_Solicitada { get; set; }
        public virtual String Rir { get; set; }
        public virtual Int64? Qtde_Entregue { get; set; }
        public virtual Int64? Qtde_Descontada { get; set; }
        public virtual Int64? Valor_Descontar { get; set; }
        public virtual Int64? Qtde_Devolver { get; set; }
        public virtual DateTime? Data_Dm_Prog { get; set; }
        public virtual Int64? Saldo_Dia { get; set; }
        public virtual String Classificacao_Rmdm { get; set; }
        public virtual String Observacao { get; set; }
        public virtual Char? Automatico { get; set; }
        public virtual Int64? Sequencial { get; set; }
        public virtual String Usuario { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual Int64? Id_Delegacia { get; set; }
        public virtual String BO { get; set; }
        public virtual String CodCt_Regulador { get; set; }
        public virtual String Xct { get; set; }
        public virtual DateTime? Dia_Alterou { get; set; }
        public virtual DateTime? Hora_Alterou { get; set; }
        public virtual Char? XRegulador { get; set; }
        public virtual Char? Devolver_Regul { get; set; }
        public virtual String Projeto_Regulador { get; set; }
        public virtual String XProj { get; set; }
        public virtual String Usuario_Alterou { get; set; }
        public virtual String Tipo_Saldo { get; set; }
        public virtual String Tipo_Saldo_Aplic { get; set; }
        public virtual String Grupo_Projeto { get; set; }
        public virtual Int64? Prog_Rm { get; set; }
        public virtual Int64? Prog_Dm { get; set; }
        public virtual Int64? Prog_Transf { get; set; }
        public virtual Int64? Prog_Transf_Regul { get; set; }
        public virtual Int64? Prog_Rm_Regul { get; set; }
        public virtual Int64? Prog_Transf_Regul_Auto { get; set; }
        public virtual Int64? Prog_Transf_Auto { get; set; }
        public virtual Int64? Saldo_Fisico { get; set; }
        public virtual Int64? Saldo_Fisico_Regul { get; set; }
        public virtual Int64? Aplic { get; set; }
        public virtual Int64? Remov { get; set; }
        public virtual Int64? Ent { get; set; }
        public virtual Int64? Sai { get; set; }
        public virtual Int64? Limite_Kit_Equipe { get; set; }
        public virtual Int64? Id_Solicitacao_Material { get; set; }
        public virtual String Localizacao_Real { get; set; }
        public virtual Int64? Id_Devolucao_Os_Mat { get; set; }
        public virtual Int64? Peso { get; set; }
        public virtual String Store_Location { get; set; }
        public virtual Char? Cancelado { get; set; }
        public virtual Char? Terminal_Retirado { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Requisicao_MovimentoRmdm other = obj as Requisicao_MovimentoRmdm;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && CodMat == other.CodMat && CodCpl == other.CodCpl && SeqRmdm == other.SeqRmdm)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
