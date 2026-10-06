using Aniel.ConnectAPI.Model.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.ConnectAPI.Controllers.Base
{
    public interface IAtualizacaoAutomatica
    {
        Task<Response> AtualizarTodos();
    }
}
