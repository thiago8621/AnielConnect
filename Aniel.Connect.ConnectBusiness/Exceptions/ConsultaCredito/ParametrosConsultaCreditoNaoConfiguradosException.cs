using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class ParametrosConsultaCreditoNaoConfiguradosException : Exception
    {
        public override string Message
        {
            get
            {
                return "Os parâmetros das faixas da consulta de crédito não foram configurados";
            }
        }
    }
}
