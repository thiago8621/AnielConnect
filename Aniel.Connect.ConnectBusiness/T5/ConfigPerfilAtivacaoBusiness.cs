using Aniel.Connect.Repository.Repository.T5.ConfigPerfilAtivacao;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ConfigPerfilAtivacaoBusiness
    {
        private IConfigPerfilAtivacaoRepository repo;

        public ConfigPerfilAtivacaoBusiness(IConfigPerfilAtivacaoRepository repo)
        {
            this.repo = repo;
        }

        public async Task<ConfigPerfilAtivacaoAniel> GetByVinculo(string Codmat, string Id_OLT)
        {
            return await repo.GetByVinculo(Codmat, Id_OLT);
        }

        public async Task InsertOrUpdate(ConfigPerfilAtivacaoAniel objeto)
        {
            await repo.InsertOrUpdate(objeto);
        }

        public async Task<long> GetProximoId()
        {
            return await GetProximoId();
        }
    }
}
