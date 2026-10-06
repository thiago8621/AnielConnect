using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Funcao
    {
        public virtual Int64 Cod_Funcao { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Classe { get; set; }
        public virtual String Coluna { get; set; }
        public virtual String Revisao { get; set; }
        public virtual Int64? HXH { get; set; }
        public virtual Double? Salario { get; set; }
        public virtual String Num_Cbo { get; set; }
        public virtual String Cod_Sindicato { get; set; }
        public virtual String Integracao { get; set; }
        public virtual Double? Adic_Insalubre { get; set; }
        public virtual Double? Adic_Periculosidade { get; set; }
        public virtual Double? Adic_Transferencia { get; set; }
        public virtual String Requisito { get; set; }
        public virtual String Objetivo { get; set; }
        public virtual String Desc_At_Ppp { get; set; }
        public virtual String Responsabilidade { get; set; }
        public virtual Double? Piso_Minimo { get; set; }
        public virtual String Cod_Funcao_Aux { get; set; }
        public virtual String Funcao_Tributavel { get; set; }
        public virtual String Cod_Perfil_Equipe { get; set; }
        public virtual String Permite_Gerar_Equipe { get; set; }
        public virtual String Grupo_Funcao { get; set; }
        public virtual Int32? Status { get; set; }
    }
}
