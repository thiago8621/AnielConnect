using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamTipoPessoa
{
    public interface IParametrosConsultaPreQualParamTipoPessoaRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa> GetAll();
        Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param);
        void Update(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param);
        void Delete(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param);
    }
}
