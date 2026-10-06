using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class IntegracaoSmsConnectionException : Exception
    {
        private readonly string _message = "Houve um erro ao se conectar com a integração de SMS";

        public IntegracaoSmsConnectionException() : base()
        {

        }

        public IntegracaoSmsConnectionException(string message) : base()
        {
            this._message = message;
        }

        public override string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}
