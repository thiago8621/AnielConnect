using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.PendenciaEndereco
{
    public class PendenciaEnderecoRepository : IPendenciaEnderecoRepository
    {
        private readonly ISession _sessionNh;

        public PendenciaEnderecoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        
        public IQueryable<Sinapse.Model.PendenciaEndereco> GetAll()
        {
            using (var repo = new Conexao<Sinapse.Model.PendenciaEndereco>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Sinapse.Model.PendenciaEndereco GetConsultaPendenciaEnderecoById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public Sinapse.Model.PendenciaEndereco GetConsultaPendenciaEndereco(string logradouro, string numero)
        {
            return this.GetAll()
                       .Where(p => p.Logradouro == logradouro && p.Numero == numero)
                       .OrderByDescending(p => p.DataHoraCriacao)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.PendenciaEndereco consultaPendencia)
        {
            using (var repo = new Conexao<Sinapse.Model.PendenciaEndereco>(this._sessionNh))
            {
                repo.Save(consultaPendencia).Wait();
            }
        }
    }
}
