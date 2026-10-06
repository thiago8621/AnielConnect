using System;

namespace Aniel.Connect.Repository.Exceptions.Mk
{
    public class MkCrmRequestException : Exception
    {
        private readonly string _messageMk;

        public MkCrmRequestException() : base()
        {

        }

        public MkCrmRequestException(string messageMk) : base()
        {
            this._messageMk = messageMk;
        }

        public override string Message
        {
            get
            {
                if (!String.IsNullOrEmpty(this._messageMk))
                    return _messageMk;
                else
                    return "Houve um erro no processamento da requisição para API do MK Solutions";
            }
        }
    }
}
