using Aniel.Connect.Repository.Repository.T5.Devolucao;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class DevolucaoAnielBusiness
    {
        private readonly IDevolucaoAnielRepository _devolucao;

        public DevolucaoAnielBusiness(IDevolucaoAnielRepository devolucao)
        {
            _devolucao = devolucao;
        }

        public async Task<EstoqueAniel> GetObjEstoque(Requisicao_SequenciaRmdmAniel sequencia, Requisicao_MovimentoRmdm movimento)
        {
            return await _devolucao.GetObjEstoque(sequencia, movimento);
        }
    }
}
