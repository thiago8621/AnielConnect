using NHibernate;
using Sinapse.DAO.repository;

namespace Aniel.Connect.Repository.Vencimento
{
    public class VencimentoRepository : IVencimentoRepository
    {
        private readonly ISession _sessionNh;
        public VencimentoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public Connect.Model.nhibernate.Vencimento GetByIdVencimento(long id)
        {
            using (var repoVencimento = new Conexao<Connect.Model.nhibernate.Vencimento>(this._sessionNh))
            {
                return repoVencimento.selectFirst(p => p.Id == id).Result;
            }
        }
    }
}
