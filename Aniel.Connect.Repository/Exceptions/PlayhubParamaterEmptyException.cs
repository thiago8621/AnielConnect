using Aniel.Connect.Repository.Exceptions.Base;
using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubParamaterEmptyException : PlayhubParameterException
    {
        private readonly string _parametro;

        public PlayhubParamaterEmptyException(string parametro) : base()
        {
            if (String.IsNullOrEmpty(parametro))
                throw new ArgumentNullException("parametro");

            this._parametro = parametro;
        }

        public override string Message
        {
            get
            {
                return String.Format("O parâmetro \"{0}\" exigido pela API do Playhub não foi informado", this._parametro);
            }
        }
    }
}
