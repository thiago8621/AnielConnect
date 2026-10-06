using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Campanha
{
    public class CampanhaRepository : ICampanhaRepository
    {
        private readonly ISession _sessionNh;

        public CampanhaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Connect.Model.nhibernate.Campanha> GetAll()
        {
            using (var repoCampanha = new Conexao<Connect.Model.nhibernate.Campanha>(this._sessionNh))
            {
                return repoCampanha.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Campanha GetById(long id)
        {
            using (var repoCampanha = new Conexao<Connect.Model.nhibernate.Campanha>(this._sessionNh))
            {
                return repoCampanha.selectFirst(p => p.Id == id).Result;
            }
        }
    }
}

