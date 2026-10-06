using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamMeioCaptacao
{
    public interface IPerfilVisualizacaoVendaParamMeioCaptacaoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param);
    }
}
