using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Usuario
{
    public interface IUsuarioAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_SENHA
        /// </summary>
        IQueryable<SenhaAniel> GetAll();

        /// <summary>
        ///     Retorna o usuário associado ao username informado
        /// </summary>
        /// <param name="usuario">username do usuário</param>
        SenhaAniel GetByUsuario(string usuario);
    }
}
