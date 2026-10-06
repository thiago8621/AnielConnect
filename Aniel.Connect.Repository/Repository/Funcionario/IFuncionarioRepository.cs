using System.Linq;

namespace Aniel.Connect.Repository.Funcionario
{
    /// <summary>
    ///     Interface para os registros de funcionários do Connect, que
    ///     são trazidos através de uma integração com o T5 mas são registrados
    ///     em bancos diferentes
    /// </summary>
    public interface IFuncionarioRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_FUNCIONARIO
        /// </summary>
        IQueryable<Sinapse.Model.Funcionario> GetAll();

        /// <summary>
        ///     Retorna o funcionário associado ao ID informado
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        Sinapse.Model.Funcionario GetById(long id);
    }
}
