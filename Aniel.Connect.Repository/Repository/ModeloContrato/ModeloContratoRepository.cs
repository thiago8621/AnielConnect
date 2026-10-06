using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ModeloContrato
{
    public class ModeloContratoRepository : IModeloContratoRepository
    {
        private ISession _sessionNh;
        public ModeloContratoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Connect.Model.nhibernate.Modelo_Contrato> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Modelo_Contrato>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Modelo_Contrato GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }

    }
}
