using Aniel.Connect.Repository.Model;
using System.Linq;

namespace Aniel.Connect.Repository.IntegracaoExterna
{
    public interface IIntegracaoExternaRepository
    {
        /// <summary>
        /// Consulta todas as integracoes externas cadastradas nas configurações da empresa.
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.IntegracoesExternas> GetAll();

        /// <summary>
        ///     Retorna os dados para autenticação com a API de integração do Playhub, caso tenham sido configurados.
        /// </summary>
        DadosAcessoIntegracaoPlayhub GetDadosIntegracaoPlayhub();

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o MK Solutions para a consulta de pendência financeira
        /// </summary>
        bool UtilizaMkConsultaPendenciaFinanceira();

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o middleware Alcans para a consulta de pendência financeira
        /// </summary>
        bool UtilizaAlcansConsultaPendenciaFinanceira();

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o middleware Alcans para a consulta de pendência por endereço
        /// </summary>
        bool UtilizaAlcansConsultaPendenciaEndereco();

        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o MK Solutions para a consulta de pendência no endereço
        /// </summary>
        bool UtilizaMkConsultaPendenciaEndereco();
        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o Azza para a consulta de pendência no endereço
        /// </summary>
        bool UtilizaIXCConsultaPendenciaEndereco();
        /// <summary>
        /// Informa se o ambiente está configurado para utilizar a Voalle para a consulta de pendência no endereço
        /// </summary>
        /// <returns></returns>
        bool UtilizaVoalleConsultaPendenciaFinanceira();
        /// <summary>
        /// Grava a integração correspondente
        /// </summary>
        /// <param name="integracao"></param>
        void save(Aniel.Connect.Model.nhibernate.IntegracoesExternas integracao);
        /// <summary>
        /// Captura a integração com informações da prequal
        /// </summary>
        /// <returns></returns>
        Aniel.Connect.Model.nhibernate.IntegracoesExternas GetIntegracaPrequal();

        /// <summary>
        ///     Retorna os dados para autenticação com a API de integração de Provisionamento, caso tenham sido configurados.
        /// </summary>
        Aniel.Connect.Model.nhibernate.IntegracoesExternas GetIntegracaoProvisionamento();
        /// <summary>
        ///     Informa se o ambiente está configurado para utilizar o Azza para a consulta de pendência financeira
        /// </summary>
        bool UtilizaIXCConsultaPendenciaFinanceira();
    }
}
