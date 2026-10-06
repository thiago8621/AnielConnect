using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Model.Except
{
    public class EAnielException : Exception
    {
        public EAnielException()
        {

        }
        public EAnielException(string message)
            : base(message)
        {
        }
        public EAnielException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
