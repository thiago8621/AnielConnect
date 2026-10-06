using Aniel.Connect.Repository.T5.ElementoRedeEnderecos;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Linq;

namespace Aniel.Connect.Repository.Repository.T5.ElementoRedeEnderecos
{
    public class ElementoRedeEnderecosAnielRepository: IElementoRedeEnderecosAnielRepository
    {        
        private readonly ISession _sessionT5Nh;
        public ElementoRedeEnderecosAnielRepository(ISession sessionT5) 
        {                            
            _sessionT5Nh = sessionT5;
        }
        public IQueryable<Elemento_Rede_Enderecos> GetAll() 
        {
            using (var repository = new Conexao<Elemento_Rede_Enderecos>(this._sessionT5Nh)) 
            {
                return repository.QueryableSelect();
            }
        }
    }
}
