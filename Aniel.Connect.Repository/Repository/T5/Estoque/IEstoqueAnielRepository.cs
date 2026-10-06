using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Estoque
{
    public interface IEstoqueAnielRepository
    {
        Task<MaterialAniel> GetById(string CodMat);
        void SaveOrUpdate(EstoqueAniel estoqueAniel);
    }
}
