using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamSetorDepto
{
    public interface IParametrosConsultaPreQualParamSetorDeptoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto> GetAll();
        Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param);
        void Update(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param);
        void Delete(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param);
    }
}
