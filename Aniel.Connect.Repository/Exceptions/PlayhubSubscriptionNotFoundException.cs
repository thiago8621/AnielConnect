using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubSubscriptionNotFoundException : Exception
    {
        private readonly string _username;

        public PlayhubSubscriptionNotFoundException(string username) : base()
        {
            this._username = username;
        }

        public override string Message
        {
            get
            {
                return String.Format("As assinaturas do usuário com o username \"{0}\" não foram encontradas no Playhub", this._username);
            }
        }
    }
}
