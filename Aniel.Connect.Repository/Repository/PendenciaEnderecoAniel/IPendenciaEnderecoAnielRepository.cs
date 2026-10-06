using Aniel.Connect.Repository.Model;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PendenciaEnderecoAniel
{

    public interface IPendenciaEnderecoAnielRepository
    {
        /// <summary>
        ///     Busca de endereços com pendências de fatura no banco do Connect
        /// </summary>
        /// <param name="logradouro">Logradouro filtrado</param>
        /// <param name="numero">Número do logradouro filtrado</param>
        IEnumerable<PendenciaEnderecoListModel>  GetPendenciasEndereco(string logradouro, string numero);
    }

}
