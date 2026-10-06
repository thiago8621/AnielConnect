using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Projeto
{
    public class ProjetoAnielRepository : IProjetoAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public ProjetoAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<ProjetoAniel> GetAll()
        {
            using (var repoProjeto = new Conexao<ProjetoAniel>(this._sessionT5Nh))
            {
                return repoProjeto.QueryableSelect();
            }
        }

        public ProjetoAniel GetByCodigo(string codCt, string projeto)
        {
            return this.GetAll()
                       .Where(p => p.CodCt == codCt &&
                                   p.Codigo == projeto)
                       .FirstOrDefault();
        }
    }
}
