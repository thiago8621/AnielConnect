using Aniel.Connect.Core;
using Aniel.Connect.Repository.Repository.T5.Material;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class MaterialAnielBusiness
    {
        private readonly IMaterialAnielRepository _material;

        public MaterialAnielBusiness(IMaterialAnielRepository material)
        {
            _material = material;
        }

        public async Task<MaterialAniel> GetById(string CodMat)
        {
            return await _material.GetById(CodMat);
        }

        public void SaveOrUpdate(MaterialAniel materialAniel)
        {
            _material.SaveOrUpdate(materialAniel);
        }

        public string GetCodMaterialDesc()
        {
            var numMaterialDescAux_material = _material.GetGnMaterialDescAux();
            return Convert.ToString(numMaterialDescAux_material);
        }
    }
}
