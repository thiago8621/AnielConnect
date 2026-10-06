using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Uf
{
    public class UfRepository : IUfRepository
    {
        private readonly ISession _sessionNh;

        public UfRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Uf> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Uf>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Uf GetBySigla(string siglaUf, string nomePais)
        {
            return this.GetAll()
                       .Where(u => u.Pais.Nome == nomePais &&
                                   u.Sigla == siglaUf)
                       .FirstOrDefault();
        }
    }
}
