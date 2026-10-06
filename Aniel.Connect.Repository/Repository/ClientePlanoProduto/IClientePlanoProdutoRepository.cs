using System.Collections.Generic;
using System.Linq;
using Aniel.Connect.Model.nhibernate;

namespace Aniel.Connect.Repository.ClientePlanoProduto
{
    public interface IClientePlanoProdutoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CLIENTE_PLANOPRODUTO
        /// </summary>
        /// <returns></returns>
        IQueryable<Cliente_PlanoProduto> GetAll();

        /// <summary>
        ///     Retorna o cliente plano produto associado ao ID informado
        /// </summary>
        /// <param name="id">ID do combo</param>
        Cliente_PlanoProduto GetById(long id);

        /// <summary>
        /// Grava plano produto do cliente no banco 
        /// </summary>
        void Save(Cliente_PlanoProduto clientePlanoProduto);
        /// <summary>
        /// Grava uma lista de plano produto do cliente no banco 
        /// </summary>
        void SaveOrUpdate(List<Cliente_PlanoProduto> clientePlanoProduto);
        void SaveOrUpdateLog(Cliente_PlanoProduto_StatusLog clientePlanoProdutoStatusLog);
    }
}