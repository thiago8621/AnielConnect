using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Combo
{
    public class ComboRepository : IComboRepository
    {
        private readonly ISession _sessionNh;

        public ComboRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Combo> GetAll()
        {
            using (var repoCombo = new Conexao<Connect.Model.nhibernate.Combo>(this._sessionNh))
            {
                return repoCombo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Combo GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public void Save(Aniel.Connect.Model.nhibernate.Combo combo)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Combo>(this._sessionNh))
            {
                repo.Save(combo).Wait();
            }
        }

        public void Update(Aniel.Connect.Model.nhibernate.Combo combo)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Combo>(this._sessionNh))
            {
                repo.Update(combo).Wait();
            }
        }
    }
}
