using System;

namespace Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito
{
    public class UsuarioApiSerasaNaoInformadoException : Exception
    {
        public override string Message
        {
            get
            {
                return "Usuário da API do Serasa não informado";
            }
        }
    }
}
