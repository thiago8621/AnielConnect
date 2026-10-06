using System;

namespace Aniel.Connect.Models
{
    public class SincronizarElementosRedeIntegracoes
    {
        public virtual string ElementosRede { get; set; }
    }

    public class ResultElementoIntegracao
    {
        public String ElementoRede { get; set; }
        public String Mensagem { get; set; }
        public Boolean Situacao { get; set; }
    }
}