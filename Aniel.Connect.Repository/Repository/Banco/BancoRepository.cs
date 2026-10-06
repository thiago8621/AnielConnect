using Aniel.Connect.Repository.Banco;
using NHibernate;
using Sinapse.DAO.repository;


namespace Aniel.Connect.Repository.Banco
{
    /// <summary>
    /// Retorna registros de banco da forma de pagamento 
    /// </summary>
    public class BancoRepository : IBancoRepository
    {
        private readonly ISession _sessionNh;
        public BancoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public Sinapse.Model.Banco GetBancoById(long id)
        {
            using (var repoBanco = new Conexao<Sinapse.Model.Banco>(this._sessionNh))
            {
                return repoBanco.selectFirst(p=> p.Id == id).Result;
            }
        }
    }
}
