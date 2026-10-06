using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamArea
{
    public interface IPerfilVisualizacaoServicoParamAreaRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_AREA
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamArea> GetAll();
        Sinapse.Model.PerfilVisualizacaoServicoParamArea GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de área de atuação de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamArea> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoServicoParamArea param);
        void Update(Sinapse.Model.PerfilVisualizacaoServicoParamArea param);
        void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamArea param);
    }
}
