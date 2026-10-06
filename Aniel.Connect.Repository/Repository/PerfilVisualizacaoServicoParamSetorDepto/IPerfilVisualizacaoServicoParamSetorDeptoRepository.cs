using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSetorDepto
{
    public interface IPerfilVisualizacaoServicoParamSetorDeptoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto> GetAll();
        Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param);
        void Update(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param);
        void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param);
    }
}
