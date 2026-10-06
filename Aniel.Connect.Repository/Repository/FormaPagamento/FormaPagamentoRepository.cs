using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.FormaPagamento
{
    /// <summary>
    /// Implementação da Interface Forma de Pagamento 
    /// </summary>
    public class FormaPagamentoRepository : IFormaPagamentoRepository
    {
        private readonly ISession _sessionNh;
        public FormaPagamentoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public Forma_Pagamento GetFormaPagamentoById(long Id)
        {
            using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(this._sessionNh))
            {
                return repoFormaPagamento.selectFirst(p => p.Id == Id).Result;
            }
        }

        public IQueryable<Forma_Pagamento> GetAllFormaPagamento()
        {
            using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(this._sessionNh))
            {
                return repoFormaPagamento.QueryableSelect();
            }
        } 
    }
}
