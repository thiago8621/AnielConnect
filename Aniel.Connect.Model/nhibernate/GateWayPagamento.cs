using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class GateWayPagamento : BaseModel
    {
        public virtual String ClientId { get; set; }
        public virtual String ClientSecret { get; set; }
        public virtual String ResourceToken { get; set; }
        public virtual String PublicToken { get; set; }
        public virtual String PrivateToken { get; set; }
        public virtual String AuthorizationToken { get; set; }
        public virtual String SellerId { get; set; }
        public virtual DateTime? DataHora_AuthTokenExpira { get; set; }
        public virtual Conta_Bancaria Conta_Bancaria { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Filial Filial { get; set; }
        public virtual Boolean Todas_Filiais { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual Int16 Digito_Conta { get; set; }
        public virtual Int16? Digito_Agencia { get; set; }
        public virtual Int16 Agencia { get; set; }
        public virtual Int64 Conta_Corrente { get; set; }
        public virtual Boolean Utilizar_Transferencia { get; set; }
        public virtual Boolean UtilizarAmbienteSandBox { get; set; }
        public virtual String Cooperativa { get; set; }
        public virtual String Posto { get; set; }
        public virtual String CodigoBeficiario { get; set; }
    }
}
