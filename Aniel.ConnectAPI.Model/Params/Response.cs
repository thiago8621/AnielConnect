using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Model.Params
{
    public class Response
    {
        public bool result { get; set; }

        public string message { get; set; }

        public string exception { get; set; }

        public string stacktrace { get; set; }
    }

    public class Response<T> : Response
    {
        public T valor { get; set; }
    }
}
