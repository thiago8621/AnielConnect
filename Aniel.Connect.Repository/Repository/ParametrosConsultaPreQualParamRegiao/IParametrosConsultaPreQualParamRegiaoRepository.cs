using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamRegiao
{
    public interface IParametrosConsultaPreQualParamRegiaoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_VENDA_MEIO_CAPTACAO
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamRegiao> GetAll();
        Sinapse.Model.ParametrosConsultaPreQualParamRegiao GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamRegiao> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param);
        void Update(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param);
        void Delete(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param);
    }
}
