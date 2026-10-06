using Sinapse.Model;

namespace Aniel.Connect.Repository.Banco
{
    /// <summary>
    /// Interface banco forma de pagamento
    /// </summary>
    public interface IBancoRepository 
    {
       Sinapse.Model.Banco GetBancoById(long id);
    }
}
