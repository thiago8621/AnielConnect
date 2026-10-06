using Sinapse.Model.Identity;
using System.Linq;

namespace Aniel.Connect.Repository.Usuario
{
    public interface IUsuarioRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na ASPNETUSERS
        /// </summary>
        IQueryable<IdentityUser> GetAll();

        /// <summary>
        ///     Retorna o usuário associado ao ID informado
        /// </summary>
        /// <param name="usuario">ID do usuário</param>
        IdentityUser GetById(string usuario);
    }
}
