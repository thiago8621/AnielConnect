using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.Material
{
    public class MaterialAnielRepository : IMaterialAnielRepository
    {
        private readonly ISession _sessionT5;

        public MaterialAnielRepository(ISession sessionT5)
        {
            _sessionT5 = sessionT5;
        }

        public async Task<MaterialAniel> GetById(string CodMat)
        {
            using (var _repoMaterial = new Conexao<MaterialAniel>(this._sessionT5))
            {
                var material = await _repoMaterial.selectFirst(x => x.Desc_Auxiliar == CodMat);
                return material is null && CodMat.Length <= 12 ? await _repoMaterial.selectFirst(x => x.CodMat == CodMat) : material;
            }
        }

        public async Task<MaterialAniel> GetByCod(string CodMat)
        {
            using (var _repoMaterial = new Conexao<MaterialAniel>(this._sessionT5))
            {
                var material = await _repoMaterial.selectFirst(x => x.CodMat == CodMat);
                return material is null && CodMat.Length <= 12 ? await _repoMaterial.selectFirst(x => x.CodMat == CodMat) : material;
            }
        }

        public async Task<MaterialAniel> GetByDescAux(string DescAux)
        {
            using (var _repoMaterial = new Conexao<MaterialAniel>(this._sessionT5))
            {
                return await _repoMaterial.selectFirst(x => x.Desc_Auxiliar == DescAux);
            }
        }

        public void SaveOrUpdate(MaterialAniel materialAniel)
        {
            using (var repoDocProd = new Conexao<MaterialAniel>(this._sessionT5))
            {
                repoDocProd.SaveOrUpdate(materialAniel).Wait();
            }
        }

        /// <summary>
        /// Buscar o proximo valor do generator MATERIAL_DESCAUX
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int GetGnMaterialDescAux()
        {
            int codMaterialDesc = 0;
            try
            {
                using (var com = this._sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.AppendLine("SELECT GEN_ID(GN_MATERIAL_DESCAUX, 1) AS MATERIAL_DESCAUX FROM RDB$DATABASE");

                    using (var reader = CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5.Connection, null).Result)
                    {
                        if (reader.Read())
                        {
                            codMaterialDesc = Convert.ToInt32(reader["MATERIAL_DESCAUX"]);
                        }
                    }
                }
            }
            catch
            {
                throw new Exception("Erro buscar número do material");
            }

            return codMaterialDesc;
        }
     

    }
}
