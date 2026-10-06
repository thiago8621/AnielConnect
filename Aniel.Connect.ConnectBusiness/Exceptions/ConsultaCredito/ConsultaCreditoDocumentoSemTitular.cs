using NHibernate.Mapping;
using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class ConsultaCreditoDocumentoSemTitularException : Exception
    {
        public override string Message 
        {
            get 
            {
                return "CPF SEM TITULAR"; 
            }
        }
    }
}
