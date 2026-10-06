using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.Localidade;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class LocalidadeAnielBusiness
    {
        private readonly ILocalidadeAnielRepository _repoLocalidadeAniel;

        public LocalidadeAnielBusiness(ILocalidadeAnielRepository repoLocalidadeAniel)
        {
            this._repoLocalidadeAniel = repoLocalidadeAniel;
        }

        public LocalidadeResponse GetByCidadeBairro(string cidade, string bairro)
        {
            return this._repoLocalidadeAniel.GetByCidadeBairro(cidade,bairro);
        }
    
        public LocalidadeResponse GetByLocalidade(string cidade, string bairro, Config_Aniel_Integrator Config = null)
        {
            return this._repoLocalidadeAniel.GetByLocalidade(cidade,bairro, Config);
        }
    }
}
