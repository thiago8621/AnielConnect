using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.Repository.T5.ProdutividadeTecnico
{
    public interface IProdutividadeTecnicoRepository
    {
        /// <summary>
        ///     Extrai o Relatório de Produtividade por Técnico
        /// </summary>
        /// <param name="request">Filtros vindos pela requisição do Datatables</param>
        /// <param name="usaAgendamentoPeriodo">
        ///     Define se a empresa está configurada para utilizar agendamento por período. 
        ///     Impacta na contagem de serviços dentro e fora do prazo
        /// </param>
        /// <param name="apenasServicosProdutivosReinc">
        ///     Define se apenas serviços reincidentes fechados com status produtivo serão 
        ///     levados em consideração no relatório
        /// </param>
        RelatorioProdutividadeResponse GetRelatorioProdutividadeTecnico(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc);

        /// <summary>
        ///     Extrai o Relatório de Produtividade por Técnico com os serviços agrupados por supervisor
        /// </summary>
        /// <param name="request">Filtros vindos pela requisição do Datatables</param>
        /// <param name="usaAgendamentoPeriodo">
        ///     Define se a empresa está configurada para utilizar agendamento por período. 
        ///     Impacta na contagem de serviços dentro e fora do prazo
        /// </param>
        /// <param name="apenasServicosProdutivosReinc">
        ///     Define se apenas serviços reincidentes fechados com status produtivo serão 
        ///     levados em consideração no relatório
        /// </param>
        RelatorioProdutividadeResponse GetRelatorioProdutividadeSupervisor(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc);

        /// <summary>
        ///     Extrai o Relatório de Produtividade por Técnico com os serviços agrupados por monitor
        /// </summary>
        /// <param name="request">Filtros vindos pela requisição do Datatables</param>
        /// <param name="usaAgendamentoPeriodo">
        ///     Define se a empresa está configurada para utilizar agendamento por período. 
        ///     Impacta na contagem de serviços dentro e fora do prazo
        /// </param>
        /// <param name="apenasServicosProdutivosReinc">
        ///     Define se apenas serviços reincidentes fechados com status produtivo serão 
        ///     levados em consideração no relatório
        /// </param>
        RelatorioProdutividadeResponse GetRelatorioProdutividadeMonitor(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc);

        /// <summary>
        ///     Extrai o Relatório de Produtividade por Técnico com os serviços agrupados por Técnico
        /// </summary>
        /// <param name="request">Filtros vindos pela requisição do Datatables</param>
        /// <param name="usaAgendamentoPeriodo">
        ///     Define se a empresa está configurada para utilizar agendamento por período. 
        ///     Impacta na contagem de serviços dentro e fora do prazo
        /// </param>
        /// <param name="apenasServicosProdutivosReinc">
        ///     Define se apenas serviços reincidentes fechados com status produtivo serão 
        ///     levados em consideração no relatório
        /// </param>
        RelatorioProdutividadeResponse GetRelatorioProdutividadeAgrupadoTecnico(
            RelatorioProdutividadeTecnicoDataTableRequest request,
            bool usaAgendamentoPeriodo,
            bool apenasServicosProdutivosReinc);
    }
}
