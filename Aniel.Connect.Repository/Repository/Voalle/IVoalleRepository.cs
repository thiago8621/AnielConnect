using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Voalle
{
    public interface IVoalleRepository
    {
        /// <summary>
        ///     Retorna as informações de um cliente registrado na Voalle
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        VoalleCustomer GetCustomer(string username);

        /// <summary>
        ///     Retorna as informações de um cliente registrado na Voalle através de seu CPF ou CNPJ
        /// </summary>
        /// <param name="cpf_cnpj">CPF ou CNPJ válidos</param>
        VoalleCustomer GetCustomerCPFCNPJ(string cpf_cnpj);

        /// <summary>
        ///     Registra uma nova Pessoa na Voalle,
        ///     situation: 1 - 
        /// </summary>
        /// <param name="criarPessoaVoalle">Objeto com os dados do novo cadastro</param>
        void CreateCustomer(CriarPessoaVoalle criarPessoaVoalle);

        /// <summary>
        ///     Registra um novo Leand na Voalle
        /// </summary>
        /// <param name="criarLeadVoalle">Objeto com os dados do novo Lead</param>
        void CreateLead(CriarLeadVoalle criarLeadVoalle);

        /// <summary>
        ///     Atualiza os dados de um cliente já existente na Voalle
        /// </summary>
        /// <param name="username">Identificador do cliente</param>
        /// <param name="customer">Objeto com os dados do cliente</param>
        void UpdateCustomer(string username, VoalleCustomer customer);

        /// <summary>
        ///     Atualiza os dados da conexão de um cliente já existente na Voalle
        /// </summary>
        VoalleConnectionResponse UpdateConnection(UpdateConnectionVoalle updateConnectionVoalle, string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);

        /// <summary>
        ///     Obtem informações do Token
        /// </summary>
        VoalleAuthResponse GetAccessToken();

        /// <summary>
        ///     Atualiza o Token
        /// </summary>
        VoalleAuthResponse RefreshAcessToken();


        /// <summary>
        ///     Valida informações do Token
        /// <param name="dadosIntegracaoVoalle">Objeto com os dados da integração atualizados</param>
        /// </summary>
        VoalleAuthResponse AuthenticateVoalle(IntegracoesExternas dadosIntegracaoVoalle);

        VoalleResponseOS GetOsVoalle(string VoalleConnectionString, string numOs);
        List<(string idCliente, double? finalLatitude, double? finalLongitude)> GetFinalGeoPostgresQuery(DbConnection connMS);

        /// <summary>
        /// Retorna dados do Login consultados na base do Voalle
        /// </summary>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="idLoginVoalle"></param>
        /// <returns></returns>
        UpdateConnectionVoalle GetLoginVoalle(string VoalleConnectionString, string idLoginVoalle);

        /// <summary>
        /// Retorna dados de Caixa/Porta consultados na base do Voalle
        /// </summary>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="idCaixa"></param>
        /// <param name="numPorta"></param>
        /// <returns></returns>
        CaixaPortaVoalle GetCaixaPortaVoalle(string VoalleConnectionString, long? idCaixa, long? numPorta, long? idCaixaGeogrid);

        /// <summary>
        /// Realiza a ocupação da Caixa/Porta, vinculando ao Login no Voalle.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        VoalleConnectionResponse ReservarCaixaPorta(UpdateConnectionVoalle request, ILog log = null);
        /// <summary>
        /// Limpa a Caixa/Porta, desfazendo o vínculo com o Login no Voalle.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        VoalleConnectionResponse LimparCaixaPorta(UpdateConnectionVoalle request, ILog log = null);
    }
}
