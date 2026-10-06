using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamSetorDepto
{
    public interface IPerfilVisualizacaoVendaParamSetorDeptoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param);
    }
}
