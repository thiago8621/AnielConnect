using System.Linq;

namespace Aniel.Connect.Repository.Departamento
{
    /// <summary>
    ///     Repository para acessar os departamentos registrados
    ///     no sistema do Connect. 
    ///     Esse repository não olha a tabela do banco do Aniel T5
    /// </summary>
    public interface IDepartamentoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_DEPARTAMENTO
        /// </summary>
        IQueryable<Sinapse.Model.Departamento> GetAll();

        /// <summary>
        ///     Retorna o departamento associado ao ID informado
        /// </summary>
        /// <param name="id">ID do departamento</param>
        Sinapse.Model.Departamento GetById(long id);
    }
}
