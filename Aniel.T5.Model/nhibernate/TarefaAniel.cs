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
    public class TarefaAniel
    {
        public virtual String CodCt { get; set; }
        public virtual String CodOs { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Tarefa { get; set; }
        public virtual String Unidade_Custo { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Equipe_Prevista { get; set; }
        public virtual Int16? Cod_Tipo_Serv { get; set; }
        public virtual String Cod_Sub_Tipo_Serv { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Usuario_Alteracao { get; set; }
        public virtual DateTime? Dia { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual DateTime? Data_Inicial { get; set; }
        public virtual DateTime? Hora_Inicial { get; set; }
        public virtual DateTime? Data_Final { get; set; }
        public virtual DateTime? Hora_Final { get; set; }
        public virtual DateTime? Data_Prevista { get; set; }

        public virtual string Mobile { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TarefaAniel other = obj as TarefaAniel;
            if (other == null)
                return false;
            if (CodCt == other.CodCt && Projeto == other.Projeto && Num_Obra == other.Num_Obra && Num_Doc == other.Num_Doc && Tarefa == other.Tarefa)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {

            return $"{CodCt} | {Projeto} | {Num_Obra} | {Num_Doc} | {Tarefa}".GetHashCode();
        }
    }
}
