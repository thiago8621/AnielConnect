using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Devolucao
{
    public interface IDevolucaoAnielRepository
    {
        Task<EstoqueAniel> GetObjEstoque(Requisicao_SequenciaRmdmAniel sequencia, Requisicao_MovimentoRmdm movimento);
    }
}
