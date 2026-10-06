using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSupervisor
{
    public interface IPerfilVisualizacaoServicoParamSupervisorRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SUPERVISOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor> GetAll();
        Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de supervisor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param);
        void Update(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param);
        void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param);
    }
}
