using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Repository.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.AlteracaoPlanoContrato
{
    public interface IAlteracaoPlanoContratoRepository
    {
        /// <summary>
        /// Método para contagem de elementos na construção do Datatable de Listagem de Alteracoes de Plano no Contrato.
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="condicoes"></param>
        /// <returns></returns>
        Task<int> Count(Dictionary<string, object> parametros, List<string> condicoes);
        /// <summary>
        /// Método de construção do Datatable de listagem de Altearções de Plano no Contrato.
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="condicoes"></param>
        /// <param name="ordenacao"></param>
        /// <returns></returns>
        Task<IEnumerable<AlteracaoPlanoContratoListModel>> GetAll(Dictionary<string, object> parametros, List<string> condicoes, Dictionary<string,string>ordenacao, int first, int skip);
        /// <summary>
        /// Método para captação dos dados do novo cadastro de Alteração de Plano para a formação de uma nova venda e, subsequentemente, um novo registro de ClientePlanoProduto.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Connect.Model.nhibernate.Venda> BindVenda(AlteracaoPlanoContratoCadastroModel view, string user);
        /// <summary>
        /// Método de elaboração das etapas de uma venda no novo registro de alteração de Plano no Contrato.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="venda"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task BindEtapasVenda(AlteracaoPlanoContratoCadastroModel view, Connect.Model.nhibernate.Venda venda, string user, Cliente_PlanoProduto cliente_PlanoProduto);
        Task<long?> GetVenda(long? idVenda, List<string>condicoes, Dictionary<string, object>parametros);
        /// <summary>
        /// Método de Combo do Contrato, implementado na modal do Index, Listagem de Alteerações de Plano no Contrato.
        /// </summary>
        /// <param name="condicoes"></param>
        /// <param name="parametros"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        Task<IEnumerable<IObjComboResult<long>>> GetComboContrato(List<string>condicoes, Dictionary<string, object>parametros, int page);        
        /// <summary>
        /// Método de Construção de um registro de VendaDadosGeraisViewModel para o Cadastro de Alterações de Plano.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="contrato"></param>
        /// <param name="plano"></param>
        /// <returns></returns>
        Task <(long Venda,long ClientePlanoProduto)> GetVendaByContratoPlano(long cliente, long contrato, long plano);
        Task<long?> GetClientePlanoProdutoByPlano(long cliente, long plano);
        Task<GetApiRetorno<VendaDadosGeraisViewModel>> GerarVendaByClientePlano(long cliente, long plano);
        Task<long> GetPlanoByContrato(long cliente, long contrato);
        /// <summary>
        /// Método inicial de Gravação do registro de Alteração de Plano no Contrato, que atua sobre a Venda.
        /// </summary>
        /// <param name="venda"></param>
        /// <returns></returns>
        Task Save(Connect.Model.nhibernate.Venda venda);
        /// <summary>
        /// Método de Gravação dos logs de alteração naquele mesmo registro de ClientePlanoProduto, a alteração, de fato.
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        Task SaveLog(List<LogAlteracaoPlanoContrato> logs);
        Task Update(Connect.Model.nhibernate.Venda venda);
        /// <summary>
        /// Método de Gravação do registro de ClientePlanoProduto da alteração de Plano.
        /// </summary>
        /// <param name="clientePlanoProduto"></param>
        /// <returns></returns>
        Task SavePlanoProduto(Cliente_PlanoProduto clientePlanoProduto);
        /// <summary>
        /// Método de lógica de captura da necessidade de criação de uma OS para uma Alteração de Plano
        /// caso haja uma divergência entre os materiais necessários para que o plano funcione no endereço do Cliente.
        /// </summary>
        /// <param name="plano"></param>
        /// <returns></returns>
        Task<IEnumerable<MaterialosCadastroModel>> GetMateriaisPlanoOs(long plano);
    }
}

