using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Cliente
{
    public class ClienteAnielRepository : IClienteAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public ClienteAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<ClienteAniel> GetAll()
        {
            using (var repoClienteAniel = new Conexao<ClienteAniel>(this._sessionT5Nh))
            {
                return repoClienteAniel.QueryableSelect();
            }
        }

        public string GetNome(int codCliente)
        {
            return this.GetAll()
                    .Where(p => p.Cod_Cliente == codCliente)
                    .Select(p => p.NomeFantasia)
                    .FirstOrDefault();
        }
    }
}
