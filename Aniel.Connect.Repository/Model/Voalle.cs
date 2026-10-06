using System.Collections.Generic;
using System;

namespace Aniel.Connect.Repository.Model
{

    public class VoalleResponse
    {
        public bool Success { get; set; }
        public object Messages { get; set; }
        public ResponseData ResponseData { get; set; }
        public string DataResponseType { get; set; }
        public object ElapsedTime { get; set; }
    }

    public class ResponseData
    {
        public List<VoalleCustomer> Data { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public object Totalizers { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    ///     Dados do cliente formatado para a API de integração com a Voalle
    /// </summary>
    public class VoalleCustomer
    {
        public int Id { get; set; }
        public string AddressComplement { get; set; }
        public string AddressReference { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Candidate { get; set; }
        public bool Carrier { get; set; }
        public string CellPhone1 { get; set; }
        public string CellPhone2 { get; set; }
        public string City { get; set; }
        public int? CivilStatus { get; set; }
        public bool Client { get; set; }
        public int CodeCityId { get; set; }
        public string CodeCountry { get; set; }
        public bool Collaborator { get; set; }
        public object CompanyPlaceId { get; set; }
        public bool Competitor { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string EmailNfe { get; set; }
        public bool Expert { get; set; }
        public int Gender { get; set; }
        public bool HouseLawyer { get; set; }
        public string Identity { get; set; }
        public string MunicipalRegistration { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Neighborhood { get; set; }
        public string Number { get; set; }
        public string ParentsName { get; set; }
        public int PeopleAddressMainId { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public bool RuralProducer { get; set; }
        public int Situation { get; set; }
        public string State { get; set; }
        public string StateRegistration { get; set; }
        public int StateRegistrationType { get; set; }
        public int Status { get; set; }
        public string Street { get; set; }
        public string StreetType { get; set; }
        public bool Supervisor { get; set; }
        public bool Supplier { get; set; }
        public bool Syndicate { get; set; }
        public bool Technical { get; set; }
        public string TxId { get; set; }
        public string TxIdFormated { get; set; }
        public int TypeTxId { get; set; }
        public bool UnlikeLawyer { get; set; }
        public int UserId { get; set; }
        public bool Vendor { get; set; }
    }

    /// <summary>
    ///     Dados exigidos pelo endpoint de autenticação da API da Voalle
    /// </summary>
    public class VoalleAuthRequest
    {
        public string client_id { get; set; } //synauth valor fixo
        public string client_secret { get; set; } // df956154024a425eb80f1a2fc12fef0c valor fixo
        public string UrlClient { get; set; }
        public string grant_type { get; set; } //refresh_token Valor Fixo
        public string refresh_token { get; set; } //Refresh Token Obtido no método de autenticação
    }

    /// <summary>
    ///     Dados retornados pelo endpoint de autenticação da API da Voalle
    /// </summary>
    public class VoalleAuthResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
    }

    public class PersonalData
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public int TypeTxId { get; set; }
        public string TxId { get; set; }
        public string BirthDate { get; set; }
        public string Identity { get; set; }
        public int CivilStatus { get; set; }
        public int Gender { get; set; }
        public string ParentsName { get; set; }
    }

    public class Contact
    {
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public bool CellPhoneHasWhatsApp { get; set; }
        public bool CellPhoneHasTelegram { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }

    public class Address
    {
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string StreetType { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string AddressComplement { get; set; }
        public string AddressReference { get; set; }
        public int CodeCityId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class IntegratorData
    {
        public string CrmContactOriginCode { get; set; }
        public string IntegratorAlias { get; set; }
        public string IntegrationCode { get; set; }
        public string IntegrationTeamCode { get; set; }
        public string IntegrationSellerTxId { get; set; }
    }

    /// <summary>
    /// CriarLeadVoalle
    /// Dados do cliente formatado para criação de Lead na Voalle
    /// </summary>

    public class CriarLeadVoalle
    {
        public PersonalData PersonalData { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public IntegratorData IntegratorData { get; set; }
    }

    /// <summary>
    /// CriarLeadVoalle
    /// Dados do cliente formatado para criação de Cliente na Voalle
    /// </summary>
    public class CriarPessoaVoalle
    {
        public string TypeTxId { get; set; }
        public string TxId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Client { get; set; }
        public int Situation { get; set; }
        public string StreetType { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string AddressComplement { get; set; }
        public string AddressReference { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public int CodeCityId { get; set; }
        public string State { get; set; }
        public string CodeCountry { get; set; }
    }

    /// <summary>
    /// UpdateConnectionVoalle
    /// Dados de conexão do cliente formatado para atualização de Conexão na Voalle
    /// </summary>
    public class UpdateConnectionVoalle
    {
        public long id { get; set; } //Obrigatorio
        public string fiberMac { get; set; }
        public string mac { get; set; }
        public string password { get; set; }
        public EquipmentTypeVoalle? equipmentType { get; set; } //Obrigatorio
        public long oltId { get; set; } //Obrigatorio
        public long slotOlt { get; set; } //Obrigatorio
        public long portOlt { get; set; } //Obrigatorio
        public string equipmentSerialNumber { get; set; } //Obrigatorio
        public long ipType { get; set; }
        public string equipmentUser { get; set; }
        public string equipmentPassword { get; set; }
        /// <summary>
        /// Id da CTO
        /// </summary>
        public long? authenticationSplitterId { get; set; } //Obrigatorio (?)
        /// <summary>
        /// Num Porta da CTO
        /// </summary>
        public long? port { get; set; } //Obrigatorio (?)
        public string wifiName { get; set; }
        public string wifiPassword { get; set; }
        public long technologyType { get; set; } //Obrigatorio, sempre fixo 8 (Fibra - FTTH)
        public long authenticationAccessPointId { get; set; } //Obrigatorio, Cod. Auxiliar da OLT
        public bool updateConnectionParameter { get; set; } //Obrigatorio, sempre true
        public bool shouldMacUpdate { get; set; }
        public string user { get; set; }
        public string complement { get; set; }
        public bool isIPoE { get; set; }
    }    

    /// <summary>
    /// Enumerado que define o tipo de Tecnologia/Modelo da OLT na Voalle
    /// </summary>
    public enum EquipmentTypeVoalle
    {
        Uninformed = 0,
        Mikrotik = 1,
        Ubiquiti = 2,
        Intelbras = 3,
        Parks = 4,
        Huawei = 5,
        FiberHome = 6,
        Zte = 7,
        DATACOM = 8,
        Digistar = 14,
        Nokia = 15,
        Gigatelco = 16,
        Raisecom = 17
    }

    /// <summary>
    /// Retorno da atualização de Conexão na Voalle
    /// </summary>
    public class VoalleConnectionResponse
    {
        public bool success { get; set; }
        public List<VoalleConnectionMessages> messages { get; set; }
        public bool response { get; set; }
        public string dataResponseType { get; set; }
        public object elapsedTime { get; set; }
    }

    public class VoalleConnectionMessages
    {
        public string message { get; set; }
        public object code { get; set; }
        public string type { get; set; }
    }

    public class VoalleResponseOS
    {
        public Int64? Atendimento { get; set; }
        public string Protocolo { get; set; }
        public Int64? Cliente { get; set; }
        public Int64? Contrato { get; set; }
        public string NomeCliente { get; set; }
        public string Cnpj_Cnpf { get; set; }
        public string Usuario { get; set; }
        public string Usuario_Abertura { get; set; }
        public DateTime? Data_HoraAbertura { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Tipo { get; set; }
        public DateTime? Data_Prox { get; set; }
        public string Situacao_Os { get; set; }
        public string Assunto { get; set; }
        public string Solicitacao { get; set; }
        public string Problema { get; set; }
        public string Telefone_Comercial { get; set; }
        public string Telefone_Residencial { get; set; }
        public string Telefone_Celular { get; set; }
        public string Descricao { get; set; }
        public string LoginRadius { get; set; }
        public string CodigoLogin { get; set; }
        public string SenhaRadius { get; set; }
        public string DescricaoPlanoProduto { get; set; }
        public string IdCliente { get; set; }
        public string assignmentId { get; set; }
        public int incident_status_id { get; set; }
        public string sector_area_id { get; set; }
    }

    /// <summary>
    /// Dados de Caixa/Porta no Voalle
    /// </summary>
    public class CaixaPortaVoalle
    {
        /// <summary>
        /// Id da CTO
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// Descrição CTO
        /// </summary>
        public string title { get; set; }
        public string geogrid_id { get; set; }
        public long? port_id { get; set; }

        /// <summary>
        /// Num porta
        /// </summary>
        public long? port { get; set; }

        /// <summary>
        /// Id do Login
        /// </summary>
        public long? authentication_contract_id { get; set; }
    }

}
