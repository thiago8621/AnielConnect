using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ClienteContrato;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Repository.ClienteContrato
{
    public class ClienteContratoRepository : IClienteContratoRepository
    {
        private readonly ISession _sessionNh;

        public ClienteContratoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.ClienteContrato> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.ClienteContrato>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }
        public Aniel.Connect.Model.nhibernate.ClienteContrato GetById(long Id)
        {
            return this.GetAll()
                        .Where(c => c.Id == Id)
                        .FirstOrDefault();
        }

        public void Save(Connect.Model.nhibernate.ClienteContrato clientecontrato)
        {

            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.ClienteContrato>(this._sessionNh))
            {
                repo.SaveOrUpdate(clientecontrato).Wait();
            }
        }
    }
}
