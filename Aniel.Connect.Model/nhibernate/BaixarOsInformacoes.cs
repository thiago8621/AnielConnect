using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class BaixarOsInformacoes
    {
        public Boolean Ok { get; set; }

        public String Mensagem { get; set; }

        public Exception InnerException { get; set; }
        public Documento_ProducaoAniel DocProducao { get; set; }
    }
}
