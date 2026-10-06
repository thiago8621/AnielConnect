using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Material_TarefaAniel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        public virtual String Tarefa { get; set; }
        public virtual String Equipe { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Cod_Sub_Tipo_Serv { get; set; }

        public virtual String CodMat { get; set; }
        public virtual String CodMat_Inst { get; set; }
        public virtual String CodCpl { get; set; }

        public virtual DateTime? Data_Aplic { get; set; }

        public virtual String Usuario { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }

        public virtual Int32? ID_DEVOLUCAO_OS_MAT { get; set; }
        public virtual Double? Quantidade { get; set; }
        public virtual Double? Quant_Remov { get; set; }
        public virtual Double? Quant_Catal { get; set; }
        public virtual Boolean Integrado { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Material_TarefaAniel other = obj as Material_TarefaAniel;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && Num_Obra == other.Num_Obra && Num_Doc == other.Num_Doc && Tarefa == other.Tarefa &&
                CodMat == other.CodMat && CodCpl == other.CodCpl && Equipe == other.Equipe &&
                Cod_Tipo_Serv == other.Cod_Tipo_Serv && Cod_Sub_Tipo_Serv == other.Cod_Sub_Tipo_Serv)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (CodCt + "|" + Projeto + "|" + Num_Obra + "|" + Num_Doc + "|" + Tarefa + "|" + CodMat + "|" + CodCpl + "|" + Equipe + "|" + Cod_Tipo_Serv + "|" + Cod_Sub_Tipo_Serv).GetHashCode();
        }
    }
}
