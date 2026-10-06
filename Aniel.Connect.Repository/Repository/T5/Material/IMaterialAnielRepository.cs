using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Material
{
    public interface IMaterialAnielRepository
    {
        Task<MaterialAniel> GetById(string CodMat);
        void SaveOrUpdate(MaterialAniel material);
        int GetGnMaterialDescAux();
    }
}
