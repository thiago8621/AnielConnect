using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamCategoriaCliente
{
    public interface IParametrosConsultaPreQualParamCategoriaClienteRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PERFIL_VIS_SERV_SETOR
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente> GetAll();
        Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente GetById(long id);

        /// <summary>
        ///     Retorna os parâmetros de setor de um perfil
        /// </summary>
        /// <param name="idPerfil">ID do perfil de visualização</param>
        IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente> GetParamsByPerfil(long idPerfil);

        void Save(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param);
        void Update(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param);
        void Delete(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param);
    }
}
