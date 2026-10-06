using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubCreateSubscriptionException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro na criação de assinatura pela API de integração com o Playhub";
            }
        }
    }
}
