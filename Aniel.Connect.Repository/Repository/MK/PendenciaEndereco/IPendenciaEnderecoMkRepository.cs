using Aniel.Connect.Repository.Model;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.MK.PendenciaEndereco
{
    /// <summary>
    ///     Interface que representa consultas de pendência de endereço na base de dados do MKSolutions
    /// </summary>
    public interface IPendenciaEnderecoMkRepository
    {
        /// <summary>
        ///     Busca de endereços com pendências de fatura no banco do MKSolutions
        /// </summary>
        /// <param name="logradouro">Logradouro filtrado</param>
        /// <param name="numero">Número do logradouro filtrado</param>
        IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero, string cidade);

        /// <summary>
        /// Metodo específico para middleware cliente Alcans
        /// </summary>
        IEnumerable<PendenciaEnderecoListModel> GetPendenciasEnderecoAlcans(string codigoUF, string codigoCidade, string codigoBairro ,string codigoLogradouro, string numero);

        RetornoConsultaDescontos BuscarFormasPagamento(string codigoCombo);
    }
}