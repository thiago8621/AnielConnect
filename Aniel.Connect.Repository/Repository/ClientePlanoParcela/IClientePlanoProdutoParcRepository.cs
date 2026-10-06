using System.Linq;

namespace Aniel.Connect.Repository.ClientePlanoParcela
{
    public interface IClientePlanoProdutoParcRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CLIENTE_PLANO_PARC
        /// </summary>
        /// <returns></returns>
        IQueryable<Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela> GetAll();

        /// <summary>
        ///     Retorna o cliente plano parcela associado ao ID informado
        /// </summary>
        /// <param name="id">ID do cliente plano produto</param>
        Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela GetById(long id);

        /// <summary>
        /// Grava parcela do cliente no banco 
        /// </summary>
        void Save(Connect.Model.nhibernate.Cliente_PlanoProduto_Parcela clientePlanoProduto);
    }
}
