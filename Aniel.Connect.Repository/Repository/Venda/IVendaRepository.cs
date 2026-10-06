
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.Model.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Venda
{
    public interface IVendaRepository
    {
        /// <summary>
        ///     Grava um registro de venda no banco.
        /// </summary>
        /// <param name="venda">Objeto com as informações da venda</param>
        void Save(Aniel.Connect.Model.nhibernate.Venda venda);

        /// <summary>
        ///     Atualiza os dados de um registro de venda no banco.
        /// </summary>
        /// <param name="venda">Objeto com as informações da venda</param>
        void Update(Aniel.Connect.Model.nhibernate.Venda venda);

        /// <summary>
        ///     Retorna a venda associado ao ID informado.
        /// </summary>
        /// <param name="id">ID da venda</param>
        Aniel.Connect.Model.nhibernate.Venda GetById(long id);

        /// <summary>
        ///     Retorna os planos e produtos associados à venda informada.
        /// </summary>
        /// <param name="id">ID da venda</param>
        IEnumerable<Aniel.Connect.Model.nhibernate.Cliente_PlanoProduto> GetPlanosProdutosVenda(long id);
        Task<IEnumerable<ClienteContratoModel>> GetPlanosProdutosVendaSQL(long id);

        /// <summary>
        ///     Captura os passos do workflow vinculado a venda.
        /// </summary>
        /// <param name="venda"> venda onde o Workflow está atribuído</param>
        /// <param name="atividade"> padrão 0, mas pode receber outro valor</param>
        /// <returns></returns>
        Task<IEnumerable<AtividadeVendaRequestModel>> GetAtividadesReprovacaoAutomatica(long venda, long atividade = 0);
        /// <summary>
        ///     Captura um dos passos do workflow vinculado a venda dada a atividade parametrizada.
        /// </summary>
        /// <param name="venda">venda onde o Workflow está atribuído</param>
        /// <param name="atividade">Atividade atual cadastrada na venda</param>
        /// <returns></returns>
        Task<AtividadeVendaRequestModel> GetItemReprovacaoAutomaticaByAtividade(long venda, long atividade);        
        /// <summary>
        ///     Responsável por atualizar o workflow da venda com todos os updates de forma a considerar todas as etapas até sua conclusão.
        /// </summary>
        /// <param name="comandosSql">Comandos construídos considerando o fluxo que á rotina faria ao ir reprovando passo a passo no Workflow</param>
        /// <returns></returns>
        Task ReprovaAtividadesWorkflowVendaAutomatico(ReprovacaoAutomaticaComandos request);
        /// <summary>
        ///     Responsável por gerar o log das atividades do workflow, assim que reprovamos automaticamente no fluxo.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="atividade"></param>
        /// <param name="sessionLog"></param>
        /// <returns></returns>
        Task GeraLogAtividadeReprovada(ReprovacaoAutomaticaComandos request, long atividade, ISession sessionLog);
        /// <summary>
        ///     Pega os status cadastrados na tabela TB_ATIVSTATUS_VENDA
        /// </summary>
        /// <returns></returns>
        IEnumerable<Atividade_StatusVenda>GetAtivStatusVenda();

        /// <summary>
        ///     Pega os status da venda
        /// </summary>
        /// <returns></returns>
        IQueryable<Atividade_StatusVenda> GetAllStatusVenda();
    }
}
