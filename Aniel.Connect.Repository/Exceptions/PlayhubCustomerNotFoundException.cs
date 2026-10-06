using System;

namespace Aniel.Connect.Repository.Exceptions
{
    public class PlayhubCustomerNotFoundException : Exception
    {
        private readonly string _username;

        public PlayhubCustomerNotFoundException(string username) : base()
        {
            this._username = username;
        }

        public override string Message
        {
            get
            {
                return String.Format("Cliente com o username \"{0}\" não encontrado no Playhub", this._username);
            }
        }
    }
}
