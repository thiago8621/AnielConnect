using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.ConfigPerfilAtivacao
{
    public interface IConfigPerfilAtivacaoRepository
    {
        Task<ConfigPerfilAtivacaoAniel> GetByVinculo(string Codmat, string Id_OLT);
        Task InsertOrUpdate(ConfigPerfilAtivacaoAniel objeto);
        Task<long> GetProximoId();
    }
}
