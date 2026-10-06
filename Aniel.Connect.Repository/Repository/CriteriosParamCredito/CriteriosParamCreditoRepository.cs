using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.CriteriosParamCredito
{
    public class CriteriosParamCreditoRepository : ICriteriosParamCreditoRepository
    {
        private readonly ISession _sessionNh;

        public CriteriosParamCreditoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.CriteriosParamCredito> GetAll()
        {
            using (var repoCriterios = new Conexao<Sinapse.Model.CriteriosParamCredito>(this._sessionNh))
            {
                return repoCriterios.QueryableSelect();
            }
        }
    }
}
