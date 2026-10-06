using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Workflow
{
    public interface IWorkflowRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na Pool IP
        /// </summary>
        /// <returns></returns>
        IQueryable<Connect.Model.nhibernate.Workflow_Vendas> GetAll();

        /// <summary>
        ///     Retorna uma representação de uma consulta na Pool Ip por ID
        /// </summary>
        /// <returns></returns> 
        Connect.Model.nhibernate.Workflow_Vendas GetById(long Id);

        Task<List<Workflow_Vendas>> FindWorkFlow(VerificaWorkflowRequestModel dadosVerificar);
    }
}
