using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubAuthenticationException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro na autenticação com a API de integração com o Playhub";
            }
        }
    }
}
