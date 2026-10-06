using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Requisicao
{
    public interface IRequisicaoAnielRepository
    {
        Task<String> GetSeqRMDM(String codct, String projeto);
    }
}
