using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ClientePlanoParcela;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ClientePlanoProduto
{
    public class ClientePlanoProdutoParcRepository : IClientePlanoProdutoParcRepository
    {
        private ISession _sessionNh;
        public ClientePlanoProdutoParcRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public void Save(Cliente_PlanoProduto_Parcela clientePlanoProduto)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela>(this._sessionNh))
            {
                repo.Save(clientePlanoProduto).Wait();
            }
        }
    }
}
