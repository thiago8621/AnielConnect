using Aniel.Connect.Repository.Exceptions.Base;
using System;

namespace Aniel.Connect.Repository.Exceptions.Voalle
{
    public class MkCrmObterTokenAutenticacaoException : Exception
    {
        private readonly string _messageVoalle;


        public MkCrmObterTokenAutenticacaoException() : base()
        {

        }

        public MkCrmObterTokenAutenticacaoException(string messageVoalle)
        {
            this._messageVoalle = messageVoalle;
        }

        public override string Message
        {
            get
            {
                if (!String.IsNullOrEmpty(this._messageVoalle))
                    return "Erro ao obter o token para autenticação com a API da Voalle - " + this._messageVoalle;
                else
                    return "Erro ao obter o token para autenticação com a API da Voalle";
            }
        }
    }


    public class VoalleCustomerNotFoundException : Exception 
    {
        private readonly string _username;

        public VoalleCustomerNotFoundException(string username) : base()
        {
            this._username = username;
        }

        public override string Message
        {
            get
            {
                return String.Format("Cliente com o username \"{0}\" não encontrado na Voalle", this._username);
            }
        }
    }

    public class VoalleClientConnectionNotFoundException : Exception
    {
        private readonly long _idConnectionVoalle;

        public VoalleClientConnectionNotFoundException(long idConnectionVoalle) : base()
        {
            this._idConnectionVoalle = idConnectionVoalle;
        }

        public override string Message
        {
            get
            {
                return String.Format("Cliente com o Id de Conexão \"{0}\" não encontrado na Voalle", Convert.ToString(this._idConnectionVoalle));
            }
        }
    }

    public class VoalleAuthenticationException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro na autenticação com a API de integração com a Voalle";
            }
        }
    }

    public class VoalleRefusedException : Exception
    {
        public override string Message
        {
            get
            {
                return "Requisição recusada pela API de integração com a Voalle";
            }
        }
    }

    public class VoalleStringParameterLengthException : Exception
    {
        private readonly string _parametro;

        private readonly int? _minLength;
        private readonly int? _maxLength;

        public VoalleStringParameterLengthException(string parametro, int? minLength = null, int? maxLength = null) : base()
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


    public class VoalleRequestProcessingException : Exception
    {
        public override string Message
        {
            get
            {
                return "Erro no processamento da requisição para a API de integração com a Voalle";
            }
        }
    }
}

