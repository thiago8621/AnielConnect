using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.ControllersWebApi.ControllersWebApi_Business.EnderecoBusiness
{
    public class Endereco_Business
    {
        private IEndereco_Repository _repository;

        public Endereco_Business(IEndereco_Repository repository)
        {
            this._repository = repository;
        }
        #region Pais
    
        public async System.Threading.Tasks.Task<Core.ObjCombo<Int64>> GetComboPais(Core.RequestObjCombo<int> request)
        {
            return await this._repository.GetComboPais(request);
        }
        #endregion

        #region Uf
        public async System.Threading.Tasks.Task<Core.ObjCombo<Int64>> GetComboUf(Core.RequestObjComboPai<Int64?, String> request)
        {
            return await this._repository.GetComboUf(request);
        }
        public Core.ObjCombo<Int64> GetComboUfEnum(Core.RequestObjComboPai<Int64?, String> request)
        {
            return this._repository.GetComboUfEnum(request);
        }
        #endregion

        #region Cidade
        public async System.Threading.Tasks.Task<List<Models.EnderecoComboObj>> GetCidades(String UF)
        {
            return await _repository.GetCidades(UF);
        }
        public async System.Threading.Tasks.Task<Core.ObjCombo<String>> GetComboCidadesDescricao(Core.RequestObjComboPai<Int64?, String> request)
        {
            return await _repository.GetComboCidadesDescricao(request);
        }
        public async System.Threading.Tasks.Task<Core.ObjCombo<Int64>> GetComboCidades(Core.RequestObjComboPai<Int64?, String> request)
        {
            return await _repository.GetComboCidades(request);
        }
        #endregion

        #region Bairro
        public async System.Threading.Tasks.Task<List<Core.Models.ComboObj>> GetBairros(Int64 Id_Cidade)
        {
            return await _repository.GetBairros(Id_Cidade);
        }
        public async System.Threading.Tasks.Task<List<Core.Models.ComboObj>> GetBairrosViaCep(string Bairro)
        {
            return await GetBairrosViaCep(Bairro);
        }
        public async System.Threading.Tasks.Task<Core.ObjCombo<Int32>> GetComboBairros(Core.RequestObjComboPai<Int32?, String> request)
        {
            return await _repository.GetComboBairros(request);
        }
        #endregion

        #region PlanilhaEstadosMunicipios 
        public async System.Threading.Tasks.Task<List<Models.UFMunicipioObj>> PlanilhaEstadosMunicipios()
        {
            return await _repository.PlanilhaEstadosMunicipios();
        }
        #endregion

    }
}