using Aniel.Connect.Model.enumerado;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Financeiro.CobrancaFaturamento
{
    public class CobrancaFaturamentoRepository : ICobrancaFaturamentoRepository
    {
        private readonly ISession _sessionNh;

        public CobrancaFaturamentoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.CobrancaFaturamento> GetAll()
        {
            using (var repoCobrancaFaturamento = new Conexao<Aniel.Connect.Model.nhibernate.CobrancaFaturamento>(this._sessionNh))
            {
                return repoCobrancaFaturamento.QueryableSelect();
            }
        }

        public IEnumerable<Connect.Model.nhibernate.CobrancaFaturamento> GetCobrancasVencidas()
        {
            return this.GetAll()
                       .Where(c => c.Status == Cobranca_StatusPagamento.Vencido);
        }

        public void Update(Connect.Model.nhibernate.CobrancaFaturamento cobrancaFaturamento)
        {
            using (var repoCobrancaFaturamento = new Conexao<Aniel.Connect.Model.nhibernate.CobrancaFaturamento>(this._sessionNh))
            {
                repoCobrancaFaturamento.Update(cobrancaFaturamento).Wait();
            }
        }
    }
}
