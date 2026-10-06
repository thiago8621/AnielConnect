using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.IntegracaoExterna;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class IntegracaoExternaBusiness
    {
        private readonly IIntegracaoExternaRepository _repository;

        public IntegracaoExternaBusiness(IIntegracaoExternaRepository repository)
        {
            this._repository = repository;
        }


        public IntegracoesExternas GetIntegracaoExterna(EnumTipoIntegracao tipoIntegracao, IntegracaoExterna integracaoExterna)
        {
            return this._repository
                       .GetAll()
                       .Where(i => i.TipoIntegracao == tipoIntegracao && i.IntegracaoExterna == integracaoExterna)
                       .FirstOrDefault();                                
        }
        public IEnumerable<KeyValuePair<int,string>> GetIntegracoesExternas(EnumTipoIntegracao tipoIntegracao)
        {
            return this._repository
                       .GetAll()
                       .Where(i => i.TipoIntegracao == tipoIntegracao)
                       .ToDictionary(p => (int)p.IntegracaoExterna, p => HtmlEnumExtensions.GetEnumDescription(p.IntegracaoExterna)).ToList();
        }

        public IntegracoesExternas GetIntegracaoExterna(EnumTipoIntegracao tipoIntegracao)
        {
            return this._repository
                       .GetAll()
                       .Where(i => i.TipoIntegracao == tipoIntegracao)
                       .FirstOrDefault();
        }

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o MK Solutions para a consulta de pendência financeira
        /// </summary>
        public bool UtilizaMkConsultaPendenciaFinanceira()
        {
            return this._repository.UtilizaMkConsultaPendenciaFinanceira();
        }

        public bool UtilizaAlcansConsultaPendenciaFinanceira()
        {
            return this._repository.UtilizaAlcansConsultaPendenciaFinanceira();
        }

        public bool UtilizaAlcansConsultaPendenciaEndereco()
        {
            return this._repository.UtilizaAlcansConsultaPendenciaEndereco();
        }

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o MK Solutions para a consulta de pendência no endereço
        /// </summary>
        public bool UtilizaMkConsultaPendenciaEndereco()
        {
            return this._repository.UtilizaMkConsultaPendenciaEndereco();
        }

        public bool UtilizaVoalleConsultaPendenciaFinanceira() {
            return this._repository.UtilizaVoalleConsultaPendenciaFinanceira();
        }

        public bool UtilizaIXCConsultaPendenciaFinanceira()
        {
            return this._repository.UtilizaIXCConsultaPendenciaFinanceira();
        }

        public bool UtilizaIXCConsultaPendenciaEndereco()
        {
            return this._repository.UtilizaIXCConsultaPendenciaEndereco();
        }

    }
}
