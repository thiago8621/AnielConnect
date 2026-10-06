using System.Collections.Generic;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ClientePlanoProduto
{
    public class ClientePlanoProdutoRepository : IClientePlanoProdutoRepository
    {
        private ISession _sessionNh;
        public ClientePlanoProdutoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Cliente_PlanoProduto> GetAll()
        {
            using (var repo = new Conexao<Cliente_PlanoProduto>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Cliente_PlanoProduto GetById(long id)
        {
            return this
                .GetAll()
                .FirstOrDefault(c => c.Id == id);
        }

        public void Save(Cliente_PlanoProduto clientePlanoProduto)
        {
            using (var repo = new Conexao<Cliente_PlanoProduto>(this._sessionNh))
            {
                repo.SaveOrUpdate(clientePlanoProduto).Wait();
            }
        }

        public void SaveOrUpdate(List<Cliente_PlanoProduto> clientePlanoProduto)
        {
            using (var repo = new Conexao<Cliente_PlanoProduto>(this._sessionNh))
            {
                foreach (var item in clientePlanoProduto)
                {
                    repo.SaveOrUpdate(item).Wait();
                }

            }
        }

        public void SaveOrUpdateLog(Cliente_PlanoProduto_StatusLog clientePlanoProdutoStatusLog)
        {
            using (var repo = new Conexao<Cliente_PlanoProduto_StatusLog>(this._sessionNh))
            {
                repo.SaveOrUpdate(clientePlanoProdutoStatusLog).Wait();
            }
        }
    }
}

