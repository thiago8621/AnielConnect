using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.T5.CodigoComplementar;
using Aniel.Connect.Repository.Repository.T5.Requisicao;
using Aniel.T5.Model.nhibernate;
using System;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class RequisicaoAnielBusiness
    {
        private readonly IRequisicaoAnielRepository _requisicao;
        private readonly ICodigoComplementarAnielRepository _codigo;

        public RequisicaoAnielBusiness(IRequisicaoAnielRepository requisicao, ICodigoComplementarAnielRepository codigo)
        {
            _requisicao = requisicao;
            _codigo = codigo;
        }

        public async Task<String> GetSeqRMDM(String codct, String projeto)
        {
            return await _requisicao.GetSeqRMDM(codct, projeto);
        }

        public Requisicao_MovimentoRmdm GetObjRmdm(ProductToApproval model, Requisicao_SequenciaRmdmAniel obj,MaterialAniel materialAniel)
        {
            _codigo.GetById(model, obj, materialAniel);

            return new Requisicao_MovimentoRmdm()
            {
                CodCt = obj.Codct,
                Projeto = obj.Projeto,
                SeqRmdm = obj.Seqrmdm,
                CodMat = materialAniel.CodMat,
                CodCpl = model.Serie,
                Qtde_Solicitada = model.Quantity,
                Qtde_Entregue = model.Quantity
            };
        }
    }
}
