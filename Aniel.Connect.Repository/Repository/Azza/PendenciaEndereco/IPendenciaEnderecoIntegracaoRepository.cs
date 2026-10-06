using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco
{
    public interface IPendenciaEnderecoIntegracaoRepository
    {
        Task<IEnumerable<PendenciaEnderecoListModel>> GetPendenciasEndereco(string endereco, string numero, string id_cidade, string cep);
    }
}
