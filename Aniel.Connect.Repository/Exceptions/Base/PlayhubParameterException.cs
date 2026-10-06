using System;

namespace Aniel.Connect.Repository.Exceptions.Base
{
    /// <summary>
    ///     Classe base para exceptions relacionadas a parâmetros da API do Playhub
    /// </summary>
    public abstract class PlayhubParameterException : Exception
    {
        public PlayhubParameterException() : base()
        {

        }
    }
}
