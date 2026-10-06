using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Area;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class AreaAnielBusiness
    {
        private readonly IAreaAnielRepository _repoAreaAniel;

        public AreaAnielBusiness(IAreaAnielRepository repoAreaAniel)
        {
            this._repoAreaAniel = repoAreaAniel;
        }

        public AreaResponse GetByCidade(string cidade)
        {
            return this._repoAreaAniel.GetByCidade(cidade);
        }
    }
}
