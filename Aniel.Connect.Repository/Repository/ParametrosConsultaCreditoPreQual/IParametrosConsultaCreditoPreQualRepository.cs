using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQual
{
    public interface IParametrosConsultaPreQualRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PERFIL_VIS_VENDA
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaPreQual> GetAll();

        /// <summary>
        ///     Retorna o registro do perfil de visualização de serviço que possui o ID informado
        /// </summary>
        /// <param name="id">ID do perfil de visualização de serviço</param>
        Sinapse.Model.ParametrosConsultaPreQual GetById(long id);

        void Save(Sinapse.Model.ParametrosConsultaPreQual perfil);
        void Update(Sinapse.Model.ParametrosConsultaPreQual perfil);
    }
}
