using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamRegiao
{
    public interface IPerfilVisualizacaoVendaParamRegiaoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_VENDA_MEIO_CAPTACAO
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamRegiao GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param);
    }
}
