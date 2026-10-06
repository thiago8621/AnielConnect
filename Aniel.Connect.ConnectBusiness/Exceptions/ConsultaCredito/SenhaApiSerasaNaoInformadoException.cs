using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class SenhaApiSerasaNaoInformadoException : Exception
    {
        public override string Message
        {
            get
            {
                return "Senha da API do Serasa não informado";
            }
        }
    }
}
