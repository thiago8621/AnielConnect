using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Mk;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Voalle;
using log4net;
using Sinapse.DAO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.MK
{
    public class VoalleBusiness
    {

        private readonly IPendenciaFinanceiraIntegracaoRepository _repoPendenciaFinanceira;
        private readonly IVoalleRepository _repoVoalle;

        public VoalleBusiness(

            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceira,
            IVoalleRepository repoVoalle
        )
        {

            this._repoPendenciaFinanceira = repoPendenciaFinanceira;
            this._repoVoalle = repoVoalle;
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            return this._repoPendenciaFinanceira.GetPendenciasFinanceira(cpfCnpj);
        }


        /// <summary>
        ///     Retorna o código do cliente no MK caso ele esteja registrado por lá
        /// </summary>
        /// <param name="cliente">Dados do Cliente</param>
        /// <param name="venda">Dados da venda a ser realizada</param>
        public void CreateLead(Cliente cliente, Venda venda, String codIBGE)
        {
            string documento = cliente.CpfCnpj?.Replace(".", String.Empty)
                   ?.Replace("-", String.Empty)
                   ?.Replace("/", String.Empty);

            string rg = cliente.Rg?.Replace(".", String.Empty)
                                  ?.Replace("-", String.Empty);

            int tipo_pessoa = (int)cliente.Tipo_Pessoa.Value;

            tipo_pessoa = (tipo_pessoa == 0 || tipo_pessoa == 2) ? 2 : 1;

            // 1solteiro(a), 2 casado(a), 3 divorciado(a), 4 viúvo(a), 5 união estável e 6 separado(a)
            int estadocivil;
            switch (cliente.Estado_Civil)
            {
                case Sinapse.Model.enumerados.Estado_Civil.Solteiro:
                    estadocivil = 1;
                    break;

                case Sinapse.Model.enumerados.Estado_Civil.Casado:
                    estadocivil = 2;
                    break;

                case Sinapse.Model.enumerados.Estado_Civil.Divorciado:
                    estadocivil = 3;
                    break;

                case Sinapse.Model.enumerados.Estado_Civil.Viuvo:
                    estadocivil = 4;
                    break;

                case Sinapse.Model.enumerados.Estado_Civil.UniaoEstavel:
                    estadocivil = 5;
                    break;

                default:
                    estadocivil = 1; // Valor padrão se nenhum caso coincidir
                    break;
            }

            //personalData
            PersonalData personalData = new PersonalData()
            {
                Name = cliente.NomeFantasia.Trim(),
                TypeTxId = tipo_pessoa, //1 Pessoa física ou  2 jurídica
                TxId = documento,
                BirthDate = cliente.Data_Nascimento.HasValue ? cliente.Data_Nascimento.Value.ToString("yyyy-MM-dd") : null,
                Identity = rg,
                CivilStatus = estadocivil,
                Gender = cliente.Sexo == Sinapse.Model.enumerados.Sexo.Masculino ? 1 : 2, //1 masculino e 2 feminino
                ParentsName = cliente.Filiacao_Mae
            };

            //Contact
            Contact contact = new Contact()
            {
                Phone = !string.IsNullOrEmpty(cliente.Telefone_Fixo) ? Regex.Replace(cliente.Telefone_Fixo, @"[^0-9]", "") : null,
                CellPhone = !string.IsNullOrEmpty(cliente.Telefone_Celular) ? Regex.Replace(cliente.Telefone_Celular, @"[^0-9]", "") : null,
                Email = cliente.Email_Principal
            };



            //Adress
            Address address = new Address()
            {
                PostalCode = cliente.Endereco_Principal.Cep,
                Street = cliente.Endereco_Principal.Logradouro,
                StreetType = CONSTANTES.StreetTypeVoalle,
                Number = cliente.Endereco_Principal.Numero,
                Neighborhood = cliente.Endereco_Principal.Bairro,
                AddressComplement = cliente.Endereco_Principal.Complemento,
                AddressReference = cliente.Endereco_Principal.Ponto_Referencia,
                CodeCityId = codIBGE != null ? Convert.ToInt32(codIBGE) : 0,
                City = codIBGE != null ? null : cliente.Endereco_Principal?.Cidade,
                State = codIBGE != null ? null : cliente.Endereco_Principal?.UF,
                Country = cliente.Endereco_Principal?.Pais?.Nome
            };


            //integratorData
            IntegratorData integratorData = new IntegratorData()
            {
                CrmContactOriginCode = CONSTANTES.crmContactOriginCode,
                IntegratorAlias = CONSTANTES.integratorAlias,
                IntegrationCode = Convert.ToString(cliente.Id),
                IntegrationSellerTxId = venda.Vendedor?.Nome, //Nome do vendedor vinculado à venda
                IntegrationTeamCode = venda.Vendedor?.Departamento?.Descricao, //Setor do Aniel que está vinculado ao Vendedor 
            };


            var dadosCliente = new CriarLeadVoalle
            {
                PersonalData = personalData,
                Contact = contact,
                Address = address,
                IntegratorData = integratorData
            };

            this._repoVoalle.CreateLead(dadosCliente);
        }

        /// <summary>
        ///     Atualiza os dados da conexão de um cliente já existente na Voalle
        /// </summary>
        /// /// <param name="onu">Dados da ONU</param>
        public VoalleConnectionResponse UpdateConnection(ONU onu, string usuario = null, string detalhe = null, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var clienteConnection = new UpdateConnectionVoalle();

            switch (onu.OLT.Marca_Modelo)
            {
                case Sinapse.Model.enumerados.Marca_Modelo_OLT.Datacom:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.DATACOM;
                    break;
                case Sinapse.Model.enumerados.Marca_Modelo_OLT.Fiberhome_A1:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.FiberHome;
                    break;
                case Sinapse.Model.enumerados.Marca_Modelo_OLT.Fiberhome_A_B:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.FiberHome;
                    break;
                case Sinapse.Model.enumerados.Marca_Modelo_OLT.Huawei:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.Huawei;
                    break;
                case Sinapse.Model.enumerados.Marca_Modelo_OLT.ZTE:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.Zte;
                    break;
                default:
                    clienteConnection.equipmentType = EquipmentTypeVoalle.Uninformed;
                    break;
            }

            long id_conexao_voalle = 0;
            long.TryParse(onu.LoginRadius?.CodigoAuxiliar, out id_conexao_voalle);
            if (id_conexao_voalle == 0)
            {
                throw new Exception("Id de Conexão do Cliente do Voalle não informado ou inválido.");
            }

            clienteConnection.id = id_conexao_voalle;
            clienteConnection.equipmentSerialNumber = onu.OLT.ConverteOlt == Sinapse.Model.enumerados.Status_Default.Ativo ? onu.SerialConvertido : onu.Serial;

            if (onu.Slot.NumSlot.HasValue)
                clienteConnection.slotOlt = onu.Slot.NumSlot.Value;
            else
                throw new Exception("Numero do Slot inválido.");

            if (onu.PON.Num_PON.HasValue)
                clienteConnection.portOlt = onu.PON.Num_PON.Value;
            else
                throw new Exception("Numero da PON inválido.");

            clienteConnection.technologyType = 8;

            if (!String.IsNullOrEmpty(onu.OLT.CodigoAuxiliar))
                clienteConnection.authenticationAccessPointId = Convert.ToInt64(onu.OLT.CodigoAuxiliar);
            else
                throw new Exception("Cod. Auxiliar da OLT (Id de integração da OLT) não informado.");

         
            if (onu.OntID.HasValue)
                clienteConnection.oltId = Convert.ToInt64(onu.OntID);
            else
                throw new Exception("Id da ONU na OLT (OntID) inválido.");


            clienteConnection.updateConnectionParameter = true;
            clienteConnection.password = onu.LoginRadius.SenhaPPPOE;


            return this._repoVoalle.UpdateConnection(clienteConnection, usuario, detalhe, enviarLog);
        }

        public VoalleAuthResponse GetAccessToken()
        {
            return this._repoVoalle.GetAccessToken();
        }

        public VoalleAuthResponse RefreshAcessToken()
        {
            return this._repoVoalle.RefreshAcessToken();
        }

        public VoalleAuthResponse AuthenticateVoalle(IntegracoesExternas dadosIntegracaoVoalle)
        {
            return this._repoVoalle.AuthenticateVoalle(dadosIntegracaoVoalle);
        }

        /// <summary>
        /// Retorna uma Ordem de Serviço do Banco de Dados Voalle 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="numOs"></param>
        /// <returns></returns>
        public VoalleResponseOS ObterOrdemServico(string VoalleConnectionString, string numOs)
        {
            return this._repoVoalle.GetOsVoalle(VoalleConnectionString, numOs);
        }

        public OsApiModel BindObject(VoalleResponseOS voalleOs,
                                     LocalidadeResponse dadosLocalidade,
                                     Configuracoes_Gerais configEmpresa,
                                     Config_Aniel_Integrator config,
                                     string descricaoEscritorio,
                                     string descricaoArea)
        {
            OsApiModel servico;
            try
            {
                servico = new OsApiModel
                {
                    UsuarioIntegracao = configEmpresa.Usuario_APIANIEL,
                    TelefoneFixoCliente = voalleOs.Telefone_Residencial,
                    TelefoneCelularCliente = voalleOs.Telefone_Celular,
                    NumOS = voalleOs.Protocolo,
                    Endereco = voalleOs.Endereco,
                    NumeroEndereco = voalleOs.Numero,
                    Complemento = voalleOs.Complemento,
                    Bairro = voalleOs.Bairro,
                    Cidade = voalleOs.Cidade,
                    Cep = voalleOs.Cep,
                    UF = voalleOs.UF,
                    NomeCliente = voalleOs.NomeCliente,
                    Cpf = voalleOs.Cnpj_Cnpf,
                    TipoServico = voalleOs.Solicitacao,
                    SubTipoServico = voalleOs.Problema,
                    Observacao = voalleOs.Descricao,
                    CodCt = String.IsNullOrEmpty(dadosLocalidade?.CodCt) ? config?.CodCtPadrao : dadosLocalidade?.CodCt,
                    Projeto = String.IsNullOrEmpty(dadosLocalidade?.Projeto) ? config?.ProjetoPadrao : dadosLocalidade?.Projeto,
                    Uni = config.Unidade_Custo,
                    ContratoCliente = Convert.ToString(voalleOs.Contrato),
                    GrupoArea = descricaoEscritorio,
                    Area = descricaoArea,
                    Localidade = dadosLocalidade?.LocalidadeStr,
                    DataHoraAgendamento = voalleOs.Data_Prox,
                    Latitude = Convert.ToString(voalleOs.Latitude),
                    Longitude = Convert.ToString(voalleOs.Longitude),
                    LoginRadius = voalleOs.LoginRadius,
                    SenhaRadius = voalleOs.SenhaRadius,
                    Plano = new PlanoRequest
                    {
                        Descricao = voalleOs.DescricaoPlanoProduto,
                        Login = voalleOs.LoginRadius,
                        Senha = voalleOs.SenhaRadius,
                    },

                    CodigoAuxiliarLogin = voalleOs.CodigoLogin
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro importar OS {voalleOs.Protocolo}: " + ex.Message);
            }
            

            return servico;
        }

        public UpdateConnectionVoalle GetLoginVoalle(string VoalleConnectionString, string idLoginVoalle)
        {
            return this._repoVoalle.GetLoginVoalle(VoalleConnectionString, idLoginVoalle);
        }
        public CaixaPortaVoalle GetCaixaPortaVoalle(string VoalleConnectionString, long? idCaixa, long? numPorta, long? idCaixaGeogrid)
        {
            return this._repoVoalle.GetCaixaPortaVoalle(VoalleConnectionString, idCaixa, numPorta, idCaixaGeogrid);
        }

        public VoalleConnectionResponse ReservarCaixaPorta(UpdateConnectionVoalle request, ILog log = null)
        {
            return this._repoVoalle.ReservarCaixaPorta(request, log);
        }

        public VoalleConnectionResponse LimparCaixaPorta(UpdateConnectionVoalle request, ILog log = null)
        {
            return this._repoVoalle.LimparCaixaPorta(request);
        }
    }
}
