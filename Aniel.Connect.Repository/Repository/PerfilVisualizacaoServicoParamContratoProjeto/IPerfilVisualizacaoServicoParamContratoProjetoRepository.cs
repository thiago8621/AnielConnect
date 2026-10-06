using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamContratoProjeto
{
    public interface IPerfilVisualizacaoServicoParamContratoProjetoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_CONTRATO
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto> GetAll();
        Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de contrato e projeto de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param);
        void Update(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param);
        void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param);
    }
}
