using AnielTV.Hibernate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Entity
{
    
    public class Fornecedor : BaseClass
    {
        public virtual String Cod_Fornecedor { get; set; }
        public virtual String RazaoSocial { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Cnpj { get; set; }

        public virtual String Endereco { get; set; }
        public virtual String Complemento { get; set; }
        public virtual Int32? Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Municipio { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Pais { get; set; }

        public virtual String Insc_Estadual { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Email { get; set; }
    }
}
