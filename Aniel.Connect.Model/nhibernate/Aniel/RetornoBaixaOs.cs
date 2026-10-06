using System;

namespace Aniel.Connect.Model.nhibernate.Aniel
{
    public class RetornoBaixaInformacoes
    {
        public Boolean Ok { get; set; }

        public String Mensagem { get; set; }

        public String InnerException { get; set; }
    }
}
