using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaCredito
{
    public class ParametrosConsultaCreditoRepository : IParametrosConsultaCreditoRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaCreditoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaCredito> GetAll()
        {
            using (var repoParams = new Conexao<Sinapse.Model.ParametrosConsultaCredito>(this._sessionNh))
            {
                return repoParams.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaCredito GetParametrosConsultaCredito()
        {
            return this.GetAll().FirstOrDefault();
        }
    }
}
