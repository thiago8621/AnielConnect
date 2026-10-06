using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.EnderecoLogradouro
{
    public class EnderecoLogradouroRepository : IEnderecoLogradouroRepository
    {
        private readonly ISession _sessionNh;

        public EnderecoLogradouroRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Endereco_Logradouro> GetAll()
        {
            using (var repo = new Conexao<Endereco_Logradouro>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Endereco_Logradouro GetById(long id)
        {
            return this.GetAll()
                       .Where(e => e.Id == id)
                       .FirstOrDefault();
        }
    }
}
