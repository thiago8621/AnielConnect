using System;

namespace Aniel.Connect.Repository.Exceptions.Mk
{
    public class MkCrmObterTokenAutenticacaoException : Exception
    {
        private readonly string _messageMk;

        public MkCrmObterTokenAutenticacaoException() : base()
        {

        }

        public MkCrmObterTokenAutenticacaoException(string messageMk)
        {
            this._messageMk = messageMk;
        }

        public override string Message
        {
            get
            {
                if (!String.IsNullOrEmpty(this._messageMk))
                    return "Erro ao obter o token para autenticação com a API do MK - " + this._messageMk;
                else
                    return "Erro ao obter o token para autenticação com a API do MK";
            }
        }
    }
}
