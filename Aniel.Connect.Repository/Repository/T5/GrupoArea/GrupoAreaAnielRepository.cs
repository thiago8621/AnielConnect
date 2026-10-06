using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.GrupoArea
{
    public class GrupoAreaAnielRepository : IGrupoAreaAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public GrupoAreaAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<EscritorioAniel> GetAll()
        {
            using (var repoEscritorio = new Conexao<EscritorioAniel>(this._sessionT5Nh))
            {
                return repoEscritorio.QueryableSelect();
            }
        }

        public EscritorioAniel GetByCodigo(string codEscritorio)
        {
            return this.GetAll()
                       .Where(e => e.Cod_Escritorio == codEscritorio)
                       .FirstOrDefault();
        }
    }
}
