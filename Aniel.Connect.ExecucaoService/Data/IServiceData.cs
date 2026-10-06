using Aniel.Connect.ExecucaoService.Data.Model;

namespace Aniel.Connect.ExecucaoService.Data
{
    /// <summary>
    ///     Interface que representa uma classe capaz de acessar os dados persistentes sobre o serviço
    /// </summary>
    public interface IServiceDataAccessor
    {
        /// <summary>
        ///     Retorna os dados do serviço persistidos
        /// </summary>        
        ServiceData GetServiceData();

        /// <summary>
        ///     Persiste um objeto contendo os dados do serviço
        /// </summary>
        /// <param name="data">Objeto contendo os dados do serviço</param>
        void WriteServiceData(ServiceData data);
    }
}
