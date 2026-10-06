using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Aniel.Connect.Repository.T5.CodigoEncerramento
{
    public class CodigoEncerramentoAnielRepository : ICodigoEncerramentoAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public CodigoEncerramentoAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<Codigo_EncerramentoAniel> GetAll()
        {
            using (var repoCodigoEncerramento = new Conexao<Codigo_EncerramentoAniel>(this._sessionT5Nh))
            {
                return repoCodigoEncerramento.QueryableSelect();
            }
        }

        public string GetDescricao(string codEncerramento, string codCt)
        {
            return this.GetAll()
                    .Where(p => p.Cod_Encerramento == codEncerramento &&
                                p.CodCt == codCt)
                    .Select(p => p.Descricao)
                    .FirstOrDefault();
        }

        public Codigo_EncerramentoAniel GetFirstCodigoEncerramento(string codEncerramento, string codCt)
        {
            return this.GetAll().FirstOrDefault(c =>
                c.CodCt == codCt && c.Cod_Encerramento == codEncerramento);
        }
    }
}
