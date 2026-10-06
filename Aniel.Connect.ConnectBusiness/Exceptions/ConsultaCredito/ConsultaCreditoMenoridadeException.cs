using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class ConsultaCreditoMenoridadeException : Exception
    {
        public override string Message 
        {
            get 
            {
                return "MENOR DE IDADE PROTEGIDO LEGALMENTE"; 
            }
        }
    }
}
