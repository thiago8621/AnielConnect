using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Forma_Pagamento : BaseModel
    {
        public virtual Faturamento_Protocolo Faturamento_Protocolo { get; set; }
        public virtual Forma_Pagamento Forma_pagto { get; set; }
        public virtual Boolean? Automatico { get; set; }
    }
}
