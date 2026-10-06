using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamEquipeVenda
{
    public interface IPerfilVisualizacaoVendaParamEquipeVendaRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_VENDA_EQUIPE_VENDA
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param);
    }
}
