using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Model.Params
{
    public interface IParamWebApi
    {
        String Usuario { get; set; }

        String Token { get; set; }
    }
}
