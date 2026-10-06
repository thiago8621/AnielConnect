using Sinapse.Model;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ElementoRedeEnderecos
{
    public interface IElementoRedeEnderecosAnielRepository
    {
        IQueryable<Elemento_Rede_Enderecos> GetAll();        
    }
}
