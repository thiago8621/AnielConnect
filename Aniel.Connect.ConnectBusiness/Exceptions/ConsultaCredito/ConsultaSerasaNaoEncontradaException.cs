using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class ConsultaSerasaNaoEncontradaException : Exception
    {
        private string _documento;

        public ConsultaSerasaNaoEncontradaException(string documento)
        {
            this._documento = documento;
        }

        public override string Message
        {
            get
            {
                return "Consulta do Serasa não encontrada para o documento " + this._documento;
            }
        }
    }
}
