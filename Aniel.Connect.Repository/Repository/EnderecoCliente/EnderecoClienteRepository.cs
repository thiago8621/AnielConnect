using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;

namespace Aniel.Connect.Repository.EnderecoCliente
{
    public class EnderecoClienteRepository : IEnderecoClienteRepository
    {
        private readonly ISession _sessionNh;

        public EnderecoClienteRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public void Save(Endereco_Cliente endereco)
        {
            using (var repo = new Conexao<Endereco_Cliente>(this._sessionNh))
            {
                repo.Save(endereco).Wait();
            }
        }

        public void Update(Endereco_Cliente endereco)
        {
            using (var repo = new Conexao<Endereco_Cliente>(this._sessionNh))
            {
                repo.Update(endereco).Wait();
            }
        }
    }
}
