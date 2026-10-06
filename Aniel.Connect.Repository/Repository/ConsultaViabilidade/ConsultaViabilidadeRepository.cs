using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ConsultaViabilidade
{
    public class ConsultaViabilidadeRepository : IConsultaViabilidadeRepository
    {
        private readonly ISession _sessionT5Nh;

        public ConsultaViabilidadeRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<Sinapse.Model.ConsultaViabilidade> GetAll()
        {
            using (var repo = new Conexao<Sinapse.Model.ConsultaViabilidade>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }

        public Sinapse.Model.ConsultaViabilidade GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

        public Sinapse.Model.ConsultaViabilidade GetConsultaViabilidadeByLatLong(double lat, double lon)
        {
            return this.GetAll()
                       .Where(c => c.Latitude == lat && c.Longitude == lon)
                       .OrderByDescending(c => c.DataHoraCriacao)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.ConsultaViabilidade consultaViabilidade)
        {
            using (var repo = new Conexao<Sinapse.Model.ConsultaViabilidade>(this._sessionT5Nh))
            {
                repo.Save(consultaViabilidade).Wait();
            }
        }
    }
}
