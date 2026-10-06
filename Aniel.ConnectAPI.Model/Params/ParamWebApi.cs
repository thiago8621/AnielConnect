using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Model.Params
{
    public class ParamWebApi<T> : ParamWebApi
    {
        public virtual T Objeto { get; set; }
    }

    public class ParamWebApi : IParamWebApi
    {
        // É o NumFre
        public virtual String Usuario { get; set; }

        // É o build
        public virtual String Token { get; set; }
    }
}
