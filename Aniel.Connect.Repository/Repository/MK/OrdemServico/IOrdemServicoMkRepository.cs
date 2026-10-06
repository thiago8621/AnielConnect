using Aniel.Connect.Repository.Model.Mk;

namespace Aniel.Connect.Repository.MK.OrdemServico
{
    /// <summary>
    ///     Interface que representa a consulta de uma Ordem de Serviço na base de dados do MKSolutions
    /// </summary>
    public interface IOrdemServicoMkRepository
    {
        /// <summary>
        ///     Busca uma Ordem de Serviço pelo número no banco do MKSolutions
        /// </summary>
        /// <param name="mkConnectionString">Connection string do banco do MK Solutions</param>
        /// <param name="numOs">Número da Ordem de Serviço (mk_os.codos)</param>
        OrdemServicoMkResponse GetOsMk(string mkConnectionString, string numOs);
    }
}
