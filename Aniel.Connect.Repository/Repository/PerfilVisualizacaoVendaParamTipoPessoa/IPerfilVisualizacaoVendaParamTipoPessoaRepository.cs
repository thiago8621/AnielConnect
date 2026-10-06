using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamTipoPessoa
{
    public interface IPerfilVisualizacaoVendaParamTipoPessoaRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa> GetAll();
        Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa param);
        void Update(Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa param);
        void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamTipoPessoa param);
    }
}
