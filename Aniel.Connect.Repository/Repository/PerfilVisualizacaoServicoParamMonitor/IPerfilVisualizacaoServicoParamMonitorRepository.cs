using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamMonitor
{
    public interface IPerfilVisualizacaoServicoParamMonitorRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_MONITOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor> GetAll();
        Sinapse.Model.PerfilVisualizacaoServicoParamMonitor GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de monitor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param);
        void Update(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param);
        void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param);
    }
}
