using Aniel.T5.Model.enumerado;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class FuncionarioAniel
    {
        public virtual String Num_Fre { get; set; }
        public virtual String Matricula_Aux { get; set; }
        public virtual String Nome { get; set; }

        public virtual String Cpf { get; set; }

        public virtual SituacaoFuncionarioAniel Situacao { get; set; }
        public virtual String Telefone_Celular { get; set; }
        public virtual String Email { get; set; }
        public virtual Int16? Cod_Depto { get; set; }

        public virtual String Supervisor { get; set; }

        public virtual Int64 Id_Municipio_Trabalho { get; set; } // 30
        public virtual String Municipio_Trabalho { get; set; } // 30
        public virtual String Uf_Trabalho { get; set; }
        public virtual HorarioAtribuido? HorarioServicoAtribuido { get; set; }
        public virtual Centro_CustoAniel Cod_CCusto { get; set; }
    }
}
