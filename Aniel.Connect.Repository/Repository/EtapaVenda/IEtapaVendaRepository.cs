using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.EtapaVenda
{
    public interface IEtapaVendaRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ETAPA_VENDA
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.EtapaVenda> GetAll();

        /// <summary>
        ///     Retorna as etapas de uma determinada venda
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        IEnumerable<Aniel.Connect.Model.nhibernate.EtapaVenda> GetByVenda(long idVenda);


        /// <summary>
        ///     Retorna as etapas de uma determinada Troca de Titularidade
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        IEnumerable<Aniel.Connect.Model.nhibernate.EtapaVenda> GetByTrocaTitularidade(long idTroca);

        /// <summary>
        ///     Salva uma etapa da venda na TB_ETAPA_VENDA
        /// </summary>
        /// <param name="etapa">Objeto com os dados da etapa da venda</param>
        void Save(Aniel.Connect.Model.nhibernate.EtapaVenda etapa);

        /// <summary>
        ///     Atualiza uma etapa da venda na TB_ETAPA_VENDA
        /// </summary>
        /// <param name="etapa">Objeto com os dados da etapa da venda</param>
        void Update(Aniel.Connect.Model.nhibernate.EtapaVenda etapa);

        /// <summary>
        /// RemoveEtapasVenda não utilizadas
        /// </summary>
        /// <param name="etapasVendas"></param>
        /// <returns></returns>
        Task RemoverEtapasVenda(List<Aniel.Connect.Model.nhibernate.EtapaVenda> etapasVendas);

        /// <summary>
        /// Atualiza a Venda dado uma etapa especifico
        /// </summary>
        /// <param name="etapa"></param>
        /// <param name="idVenda"></param>
        /// <returns></returns>
        Task UpdateEtapaAtualVenda(Aniel.Connect.Model.nhibernate.EtapaVenda etapa, long idVenda);

        /// <summary>
        /// Verifica a condição para a criação de uma etapa específica, que é a etapa de criação de de ordens de serviço
        /// </summary>
        /// <param name="idVenda"></param>
        /// <returns></returns>
        Task<bool> ConsultaGeraEtapaOS(long idVenda);
    }
}
