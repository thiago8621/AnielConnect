using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.MK.OrdemServico;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Mk;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.MK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace Aniel.Connect.ConnectBusiness.MK
{
    public class MKBusiness
    {


        private readonly IPendenciaEnderecoMkRepository _repoPendenciaEndereco;
        private readonly IPendenciaFinanceiraIntegracaoRepository _repoPendenciaFinanceira;
        private readonly IMKProvisionamentoRepository _mKProvisionamentoRepository;
        private readonly IOrdemServicoMkRepository _repoOrdemServico;

        public MKBusiness(
            IPendenciaEnderecoMkRepository repoPendenciaEndereco,
            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceira,
            IMKProvisionamentoRepository mKProvisionamentoRepository,
            IOrdemServicoMkRepository repoOrdemServico = null
        )
        {
            this._repoPendenciaEndereco = repoPendenciaEndereco;
            this._repoPendenciaFinanceira = repoPendenciaFinanceira;
            this._mKProvisionamentoRepository = mKProvisionamentoRepository;
            this._repoOrdemServico = repoOrdemServico;
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero,string cidade)
        {
            return this._repoPendenciaEndereco.GetPendenciasEndereco(logradouro, numero,cidade);
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            return this._repoPendenciaFinanceira.GetPendenciasFinanceira(cpfCnpj);
        }

        public Task<RetornoMK> VinculaConexao(string nomeCaixa, int? numPorta, string usuario, string onuSerial)
        {
            return this._mKProvisionamentoRepository.VinculaConexao(nomeCaixa, numPorta, usuario, onuSerial);
        }

        public Task<RetornoMK> DesvinculaConexao(string usuario)
        {
            return this._mKProvisionamentoRepository.DesvinculaConexao(usuario);
        }

        /// <summary>
        ///     Retorna uma Ordem de Serviço do Banco de Dados do MK Solutions
        /// </summary>
        /// <param name="mkConnectionString">Connection string do banco do MK Solutions</param>
        /// <param name="numOs">Número da Ordem de Serviço</param>
        public OrdemServicoMkResponse ObterOrdemServico(string mkConnectionString, string numOs)
        {
            return this._repoOrdemServico.GetOsMk(mkConnectionString, numOs);
        }

        public OsApiModel BindObject(OrdemServicoMkResponse mkOs,
                                      LocalidadeResponse dadosLocalidade,
                                      Configuracoes_Gerais configEmpresa,
                                      Config_Aniel_Integrator config,
                                      string descricaoEscritorio,
                                      string descricaoArea)
        {
            OsApiModel servico;
            try
            {
                var dataAbertura = mkOs.DataAbertura ?? DateTime.Now;
                var horaAbertura = mkOs.HoraAbertura ?? DateTime.Now.TimeOfDay;
                string codContrato = String.IsNullOrEmpty(mkOs.CodContrato) ? mkOs.CodConexao : mkOs.CodContrato;

                servico = new OsApiModel
                {
                    UsuarioIntegracao = configEmpresa.Usuario_APIANIEL,
                    NumOS = mkOs.NumOS,
                    TelefoneCelularCliente = mkOs.TelefoneCelular,
                    TelefoneFixoCliente = mkOs.TelefoneFixo,
                    EmailCliente = mkOs.Email,
                    Endereco = mkOs.Logradouro,
                    NumeroEndereco = mkOs.NumeroEndereco,
                    Complemento = mkOs.Complemento,
                    Bairro = mkOs.Bairro,
                    Cidade = mkOs.Cidade,
                    Cep = mkOs.Cep,
                    UF = mkOs.UF,
                    NomeCliente = mkOs.NomeCliente,
                    Cpf = String.IsNullOrEmpty(mkOs.Cpf) ? mkOs.Cnpj : mkOs.Cpf,
                    CodigoIntegratorTipoServico = mkOs.TipoOs,
                    TipoServico = mkOs.TipoOsDescricao,
                    SubTipoServico = mkOs.DefeitoDescricao,
                    Observacao = $"Obs: {mkOs.Observacoes} / Defeito reclamado: {mkOs.DefeitoReclamado} / Possível solução: {mkOs.Indicacoes}",
                    InformacoesComplementares = $"Usuário PPPOE: {mkOs.UsuarioPPPoE} / Senha PPPOE: {mkOs.SenhaPPPoE} / Usuário Sac: {mkOs.UsuarioSac} / Senha Sac: {mkOs.SenhaSac} / Mac: {mkOs.Mac} / Usuário Wpa: {mkOs.UsuarioWpa} / Senha Wpa: {mkOs.SenhaWpa} / Plano: {mkOs.PlanoAcesso} / Ponto Acesso: {mkOs.PontoAcesso}",
                    LoginRadius = mkOs.UsuarioPPPoE,
                    SenhaRadius = mkOs.SenhaPPPoE,
                    CodCt = String.IsNullOrEmpty(dadosLocalidade?.CodCt) ? config?.CodCtPadrao : dadosLocalidade?.CodCt,
                    Projeto = String.IsNullOrEmpty(dadosLocalidade?.Projeto) ? config?.ProjetoPadrao : dadosLocalidade?.Projeto,
                    Uni = config?.Unidade_Custo,
                    CategoriaCliente = mkOs.SlaTipo,
                    ContratoCliente = mkOs.CodCliente,
                    NumDoc = codContrato,
                    GrupoArea = descricaoEscritorio,
                    Area = descricaoArea,
                    Localidade = dadosLocalidade?.LocalidadeStr,
                    CodigoAuxiliarLogin = mkOs.CodConexao,
                    DataAbertura = dataAbertura.Date.Add(horaAbertura),
                    Plano = new PlanoRequest
                    {
                        Descricao = mkOs.PlanoAcesso,
                        Login = mkOs.UsuarioPPPoE,
                        Senha = mkOs.SenhaPPPoE,
                        CodigoIntegracao = mkOs.CodPlano,
                        IdLogin = mkOs.CodConexao
                    },
                    Adicional1 = mkOs.DescricaoCondominio,
                    Adicional2 = mkOs.Contato,
                    Adicional3 = mkOs.CdAtendimento,
                    Adicional4 = mkOs.Operador,
                    Adicional5 = mkOs.PlanoCrm,
                    Adicional6 = mkOs.CtoDescription,
                    Adicional7 = mkOs.PortCode
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro importar OS {mkOs.NumOS}: " + ex.Message);
            }

            return servico;
        }

    }
}
