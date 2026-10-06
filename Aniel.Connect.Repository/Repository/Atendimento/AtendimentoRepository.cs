using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Atendimento
{
    public class AtendimentoRepository : IAtendimentoRepository
    {
        private readonly ISession _session;
        public AtendimentoRepository(ISession session) 
        {
            _session = session;
        }
        public IQueryable<Connect.Model.nhibernate.Atendimento> GetAll() 
        {
            using (var repoAtendimento = new Conexao<Connect.Model.nhibernate.Atendimento>(this._session))
            {
                return repoAtendimento.QueryableSelect();
            }
        }
        public IEnumerable<Connect.Model.nhibernate.Atendimento> GetAtendimentosByContratos(IEnumerable<long> contratos) 
        {
            var retorno = new List<Connect.Model.nhibernate.Atendimento>();
            foreach (var contrato in contratos)
            {
                retorno.AddRange(this.GetAll().Where(p => p.PlanoProduto.Id == contrato).ToList());
            }
            return retorno;
        }
    }
}
