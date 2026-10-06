using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Assinatura_Termo_Adesao
    {
        public virtual long Id_Venda { get; set; }
        public virtual Byte[] Assinatura { get; set; }
    }
}