using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Campanha;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class CampanhaBusiness
    {
        private readonly ICampanhaRepository _repository;
        public CampanhaBusiness(ICampanhaRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retorna campanha associado por id informado
        /// </summary>
        /// <param name="id">Id da Campanha</param>
        /// <returns></returns>
        public Campanha GetCampanhaById(long id)
        {
            return this._repository.GetById(id);
        }

    }
}
