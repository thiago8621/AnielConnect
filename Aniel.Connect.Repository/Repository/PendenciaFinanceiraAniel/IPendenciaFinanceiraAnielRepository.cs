using Aniel.Connect.Repository.Model;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel
{
    public interface IPendenciaFinanceiraAnielRepository
    {
        /// <summary>
        /// Interface consulta de pendência financeira pelo cpf ou cnpj do banco Connect
        /// </summary>
        /// <param name="cpfCnpj">Request cpf ou cnpj da consulta</param>
        IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj);
    }
}
