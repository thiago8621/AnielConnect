using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Conta_Bancaria : BaseModel
    {

        public virtual Status_Default Status { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual String NossoNumero { get; set; }
        public virtual String Agencia { get; set; }
        public virtual String Digito_Agencia { get; set; }
        public virtual String Numero_Conta { get; set; }
        public virtual String Digito_Conta { get; set; }
        public virtual String Gerente_Conta { get; set; }
        public virtual String Convenio { get; set; }
        public virtual String Telefone_Agencia { get; set; }
        public virtual DateTime? Data_Saldo { get; set; }
        public virtual Double? Saldo_Inicial { get; set; }
        public virtual Empresa Empresa{ get; set; }
        public virtual Filial Filial{ get; set; }
        public virtual Boolean Rateio { get; set; }
        //public virtual Grupo_Rateio_Empresa_Filial Grupo_Rateio_Filiais { get; set; }
        public virtual String Nome_Conta { get; set; }
        public virtual String Observacoes { get; set; }
        


        public override string ToString()
        {
            return Nome_Conta;
        }
    }
}
