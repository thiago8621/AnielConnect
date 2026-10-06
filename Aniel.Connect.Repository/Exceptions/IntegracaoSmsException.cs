using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class IntegracaoSmsException : Exception
    {
        private readonly string _message = "Houve um erro ao enviar o SMS";

        public IntegracaoSmsException() : base()
        {

        }

        public IntegracaoSmsException(string message) : base()
        {
            this._message = message;
        }

        public IntegracaoSmsException(string message, Exception innerException) : base(message, innerException)
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
