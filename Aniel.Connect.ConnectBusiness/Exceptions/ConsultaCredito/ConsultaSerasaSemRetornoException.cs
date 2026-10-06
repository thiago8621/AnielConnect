using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class ConsultaSerasaSemRetornoException : Exception
    {
        public override string Message
        {
            get
            {
                return "A consulta de crédito do Serasa não retornou resultado";
            }
        }
    }
}
