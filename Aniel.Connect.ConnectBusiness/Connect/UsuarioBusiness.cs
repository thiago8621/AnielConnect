using Aniel.Connect.Repository.Usuario;
using Sinapse.Model.Identity;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class UsuarioBusiness
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioBusiness(IUsuarioRepository repository)
        {
            this._repository = repository;
        }

        public IdentityUser GetById(string usuario)
        {
            return this._repository.GetById(usuario);
        }
    }
}
