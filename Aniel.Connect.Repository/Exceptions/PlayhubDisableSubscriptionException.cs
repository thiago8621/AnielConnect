using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubDisableSubscriptionException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro na remoção da assinatura pela API de integração com o Playhub";
            }
        }
    }
}
