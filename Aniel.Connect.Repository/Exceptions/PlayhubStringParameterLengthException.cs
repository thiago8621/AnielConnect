using Aniel.Connect.Repository.Exceptions.Base;
using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubStringParameterLengthException : PlayhubParameterException
    {
        private readonly string _parametro;

        private readonly int? _minLength;
        private readonly int? _maxLength;

        public PlayhubStringParameterLengthException(string parametro, int? minLength = null, int? maxLength = null) : base()
        {
            if (String.IsNullOrEmpty(parametro))
                throw new ArgumentNullException("parametro");

            this._parametro = parametro;

            this._minLength = minLength;
            this._maxLength = maxLength;
        }

        public override string Message
        {
            get
            {
                if (this._minLength.HasValue && this._maxLength.HasValue)
                {
                    return String.Format(
                        "O parâmetro \"{0}\" deve ter entre {1} e {2} caracteres", 
                        this._parametro, 
                        this._minLength.Value, 
                        this._maxLength.Value
                    );
                }

                if (this._maxLength.HasValue)
                {
                    return String.Format(
                        "O parâmetro \"{0}\" deve ter {1} caracteres no máximo",
                        this._parametro,
                        this._maxLength.Value
                    );
                }

                if (this._minLength.HasValue)
                {
                    return String.Format(
                        "O parâmetro \"{0}\" deve ter {1} caracteres no mínimo",
                        this._parametro,
                        this._minLength.Value
                    );
                }

                return String.Format(
                        "O parâmetro \"{0}\" está violando o limite de caracteres",
                        this._parametro,
                        this._minLength.Value
                    );
            }
        }
    }
}
