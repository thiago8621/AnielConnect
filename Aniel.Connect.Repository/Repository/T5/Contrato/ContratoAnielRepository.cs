using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Contrato
{
    public class ContratoAnielRepository : IContratoAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public ContratoAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<ContratoAniel> GetAll()
        {
            using (var repoContrato = new Conexao<ContratoAniel>(this._sessionT5Nh))
            {
                return repoContrato.QueryableSelect();
            }
        }

        public ContratoAniel GetByCodCt(string codCt)
        {
            return this.GetAll()
                       .Where(c => c.CodCt == codCt)
                       .FirstOrDefault();
        }
    }
}
