using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamCategoriaCliente
{
    public interface IPerfilVisualizacaoVendaParamCategoriaClienteRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param);
    }
}
