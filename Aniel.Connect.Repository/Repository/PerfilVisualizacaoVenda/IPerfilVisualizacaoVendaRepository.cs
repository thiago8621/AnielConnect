using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVenda
{
    public interface IPerfilVisualizacaoVendaRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PERFIL_VIS_VENDA
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVenda> GetAll();

        /// <summary>
        ///     Retorna o registro do perfil de visualização de serviço que possui o ID informado
        /// </summary>
        /// <param name="id">ID do perfil de visualização de serviço</param>
        Sinapse.Model.PerfilVisualizacaoVenda GetById(long id);

        void Save(Sinapse.Model.PerfilVisualizacaoVenda perfil);
        void Update(Sinapse.Model.PerfilVisualizacaoVenda perfil);
    }
}
