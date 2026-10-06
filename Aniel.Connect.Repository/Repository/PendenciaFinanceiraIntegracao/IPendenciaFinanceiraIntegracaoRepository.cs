using Aniel.Connect.Repository.Model;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.PendenciaFinanceiraIntegracao
{
    public interface IPendenciaFinanceiraIntegracaoRepository
    {
        /// <summary>
        /// Interface consulta de pendência financeira pelo cpf ou cnpj por entre as integrações disponíveis
        /// </summary>
        /// <param name="cpfCnpj">Request cpf ou cnpj da consulta</param>
        IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj);
    }
}
