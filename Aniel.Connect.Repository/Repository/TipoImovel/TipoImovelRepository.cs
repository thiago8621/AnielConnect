using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.TipoImovel
{
    public class TipoImovelRepository : ITipoImovelRepository
    {
        private readonly ISession _sessionNh;

        public TipoImovelRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Tipo_Imovel> GetAll()
        {
            using (var repo = new Conexao<Tipo_Imovel>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Tipo_Imovel GetByCodTipoImovel(int codTipoImovel)
        {
            return this.GetAll()
                       .Where(t => t.Cod_Tipo_Imovel == codTipoImovel)
                       .FirstOrDefault();
        }
    }
}
