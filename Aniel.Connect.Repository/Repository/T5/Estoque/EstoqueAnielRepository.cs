using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Estoque
{
    public class EstoqueAnielRepository : IEstoqueAnielRepository
    {
        private readonly ISession _sessionT5;

        public EstoqueAnielRepository(ISession sessionT5)
        {
            _sessionT5 = sessionT5;
        }

        public async Task<MaterialAniel> GetById(string CodMat)
        {
            using (var _repoMaterial = new Conexao<MaterialAniel>(this._sessionT5))
            {
                
                return CodMat.Length <= 6 ? await _repoMaterial.selectFirst(x => x.CodMat == CodMat) : await _repoMaterial.selectFirst(x => x.Desc_Auxiliar == CodMat);
            }
        }

        public void SaveOrUpdate(EstoqueAniel estoque)
        {
            using (var _repoEstoqueAniel = new Conexao<EstoqueAniel>(this._sessionT5))
            {
                _repoEstoqueAniel.SaveOrUpdate(estoque).Wait();
            }
        }

        

    }
}
