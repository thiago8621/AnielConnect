using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Sinapse.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.ElementoRedePorta
{
    public interface IElementoRedePortaAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ELEMENTO_REDE_PORTA
        /// </summary>
        IQueryable<Elemento_Rede_Porta> GetAll();

        /// <summary>
        ///     Retorna o registro da porta do elemento de rede associado ao ID informado
        /// </summary>
        /// <param name="id">ID da porta do elemento de rede</param>
        Elemento_Rede_Porta GetById(long id);

        /// <summary>
        ///     Retorna o registro da porta do elemento de rede associado a caixa e ao numero de porta
        /// </summary>
        /// <param name="id">ID da porta do elemento de rede</param>
        Elemento_Rede_Porta GetByNumeroPorta(long idCaixa,int porta);
        Task<bool> LimparPorta(Elemento_ReservaCadastroModel request);
        Task<bool> AlterarPortaPlano(Elemento_ReservaCadastroModel request, bool ocuparPorta);
        Task<Elemento_Reserva> GetParametrosReservaPortaByClientePlanoProduto(ReservaRequest request);
        Task<GetApiRetorno<CaixaPortaReservada>> GetPortaReservadaClientePlanoProduto(CaixaPortaReservadaRequest request);
    }
}
