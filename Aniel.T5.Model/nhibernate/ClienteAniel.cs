using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class ClienteAniel
    {
        public virtual Int32 Cod_Cliente { get; set; }

        public virtual String NomeFantasia { get; set; }
        public virtual String CPF { get; set; }

        public virtual DateTime? Data_Cadastro { get; set; }

        public virtual String Telefone_Fixo { get; set; }
        public virtual String Telefone_Celular { get; set; }

        public virtual String Email_Principal { get; set; }

        public virtual Tipo_Pessoa? Tipo_Pessoa { get; set; }
        public virtual Tipo_ClienteAniel Tipo_Cliente { get; set; }
        public virtual Status_Cliente Status { get; set; }
        public virtual String Id_Integrador { get; set; }

        public virtual Status_ProspectoAniel? Status_Prospecto { get; set; }
        public virtual Categoria_ClienteAniel Categoria_Cliente { get; set; }
        public virtual string Cep { get; set; }

        public virtual string Bairro { get; set; }

        public virtual string UF { get; set; }

        public virtual short? Cidade { get; set; }

        public virtual string Numero { get; set; }

        public virtual string Logradouro { get; set; }

        public virtual string Tipo { get; set; }
        public virtual String Numero_Contrato { get; set; }
        public virtual long? Id_Organizacao { get; set; }

    }
}
