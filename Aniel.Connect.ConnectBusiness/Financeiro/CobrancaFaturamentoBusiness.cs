using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Financeiro.CobrancaFaturamento;
using System.Collections.Generic;

namespace Aniel.Connect.ConnectBusiness.Financeiro
{
    public class CobrancaFaturamentoBusiness
    {
        private readonly ICobrancaFaturamentoRepository _repository;

        public CobrancaFaturamentoBusiness(ICobrancaFaturamentoRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<CobrancaFaturamento> GetCobrancasVencidas()
        {
            return this._repository.GetCobrancasVencidas();
        }

        public void Update(CobrancaFaturamento cobrancaFaturamento)
        {
            this._repository.Update(cobrancaFaturamento);
        }
    }
}
