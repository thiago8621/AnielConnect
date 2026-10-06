using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Setor
{
    public class SetorAnielRepository : ISetorAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public SetorAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<FolhaDeptoAniel> GetAll()
        {
            using (var repoFolhaDepto = new Conexao<FolhaDeptoAniel>(this._sessionT5Nh))
            {
                return repoFolhaDepto.QueryableSelect();
            }
        }

        public FolhaDeptoAniel GetByCodigo(short codDepto)
        {
            return this.GetAll()
                       .Where(d => d.CodDepto == codDepto)
                       .FirstOrDefault();
        }
    }
}
