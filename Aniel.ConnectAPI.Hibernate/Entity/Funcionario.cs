using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Funcionario : BaseClass
    {
        public virtual String Num_Fre { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Tel_Celular { get; set; }
        public virtual Int16 Situacao { get; set; }

        //public virtual Filial Filial { get; set; }
        public virtual Int64? Id_Filial { get; set; }

        public virtual Int64? Id_Centro_Custo { get; set; }
        public virtual Int16 Supervisor { get; set; }

        public virtual Int64? Id_Departamento { get; set; }

        public virtual String Cpf { get; set; }
        public virtual String Rg { get; set; }

        public virtual String Endereco { get; set; }
        public virtual String Complemento { get; set; }

        public virtual Int32? Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Municipio { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }
    }
}
