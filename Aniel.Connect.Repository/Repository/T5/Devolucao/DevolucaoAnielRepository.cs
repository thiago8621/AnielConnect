using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Devolucao
{
    public class DevolucaoAnielRepository : IDevolucaoAnielRepository
    {
        private readonly ISession _sessionT5;

        public DevolucaoAnielRepository(ISession sessionT5)
        {
            _sessionT5 = sessionT5;
        }

        public async Task<EstoqueAniel> GetObjEstoque(Requisicao_SequenciaRmdmAniel sequencia, Requisicao_MovimentoRmdm movimento)
        {
            using (var repoFuncionario = new Conexao<FuncionarioAniel>(_sessionT5))
            {
                var objFunc = repoFuncionario.selectFirst(x => x.Num_Fre == sequencia.Equipe.Num_Fre).Result;

                var obj = new EstoqueAniel
                {
                    CodCt = sequencia.Codct,
                    Projeto = sequencia.Projeto,
                    CodMat = movimento.CodMat,
                    CodCpl = !string.IsNullOrEmpty(movimento.CodCpl) ? movimento.CodCpl : "-",
                    Data = DateTime.Now,
                    SeqLivre = "-",
                    SeqRmdm = sequencia.Seqrmdm,
                    SeqTransf = "-",
                    Pedido = "-",
                    ProjOrigem = "-",
                    ProjDestino = "-",
                    Entrada = 0,
                    Saida = (long)movimento.Qtde_Solicitada,
                    NotaFiscal = "-",
                    CodOs = "-",
                    CodLib = "-",
                    CtOrigem = "-",
                    CtDestino = "-",
                    Equipe = sequencia.Equipe.Matricula,
                    Cod_CCusto = objFunc.Cod_CCusto.Cod_Ccusto,
                    Transferencia = 'N',
                    Observacao = sequencia.ObservacaoGenerecia,
                    Saldo_Dia = 0,
                    Dia = DateTime.Now,
                    Hora = DateTime.Now,
                    Cod_Tipo_Movto = 3,
                    Num_Contrato_Cliente = 0
                };

                return await Task.FromResult(obj);
            }

        }
    }
}
