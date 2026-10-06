using Aniel.Connect.Repository.Repository.T5.Estoque;
using Aniel.Connect.Repository.Repository.T5.Material;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.T5.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class EstoqueAnielBusiness
    {
        private readonly IEstoqueAnielRepository _repoEstoque;
        private readonly IMaterialAnielRepository _repoMaterial;

        public EstoqueAnielBusiness(IEstoqueAnielRepository _repoEstoque)
        {
            this._repoEstoque = _repoEstoque;
        }

        public void EntradaDireta(MaterialAniel material, string codcpl, string codct, string projeto, DateTime dataEmissao, string numeroPedido, long quantidade,string seqrmdm)
        {
            var estoque = new EstoqueAniel
            {
                CodCt = codct,
                Projeto = projeto,
                CodMat = material.CodMat,
                CodCpl = codcpl,
                Data = dataEmissao,
                Pedido = numeroPedido,
                Entrada = quantidade,//quantidade
                Saida = 0,
                Cod_CCusto = null,
                Cod_Fornecedor = null,
                Transferencia = 'N',
                NotaFiscal = numeroPedido,
                Valor_Compra = null,
                Qtde_Tarefa = quantidade,
                Tipo_Atividade = 'C',
                Dia = DateTime.Now,
                Hora = DateTime.Now,
                CodEmpresa = null,
                Valor_Total = null,
                SeqLivre = "-",
                SeqRmdm = seqrmdm,
                SeqTransf = "-",
                ProjDestino = "-",
                ProjOrigem = "-"
            };

            _repoEstoque.SaveOrUpdate(estoque);
        }
    }
}
