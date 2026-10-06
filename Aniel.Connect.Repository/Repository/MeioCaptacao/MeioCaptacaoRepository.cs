using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.MeioCaptacao
{
    public class MeioCaptacaoRepository : IMeioCaptacaoRepository
    {
        private readonly ISession _sessionNh;

        public MeioCaptacaoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Meio_Captacao> GetAll()
        {
            using (var repo = new Conexao<Meio_Captacao>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Meio_Captacao GetById(long id)
        {
            return this.GetAll()
                       .Where(m => m.Id == id)
                       .FirstOrDefault();
        }
    }
}
