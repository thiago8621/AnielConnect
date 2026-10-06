using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubRequestProcessingException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro no processamento da requisição para a API de integração com o Playhub";
            }
        }
    }
}
