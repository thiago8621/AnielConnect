using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServico
{
    public interface IPerfilVisualizacaoServicoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PERFIL_VIS_SERV
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoServico> GetAll();

        /// <summary>
        ///     Retorna o registro do perfil de visualização de serviço que possui o ID informado
        /// </summary>
        /// <param name="id">ID do perfil de visualização de serviço</param>
        Sinapse.Model.PerfilVisualizacaoServico GetById(long id);

        void Save(Sinapse.Model.PerfilVisualizacaoServico perfil);
        void Update(Sinapse.Model.PerfilVisualizacaoServico perfil);
    }
}
