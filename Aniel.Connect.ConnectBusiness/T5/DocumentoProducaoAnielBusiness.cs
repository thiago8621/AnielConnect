using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.CodigoEncerramento;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Uf;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class DocumentoProducaoAnielBusiness
    {
        private readonly IDocumentoProducaoAnielRepository _repoDocProd;
        private readonly INaturezaAnielRepository _repoNatureza;
        private readonly ICodigoEncerramentoAnielRepository _repoCodEncerramento;
        private readonly IContratoAnielRepository _repoContratoAniel;
        private readonly IConfiguracoesGeraisRepository _repoConfigGerais;
        private readonly IClientePlanoProdutoRepository _repoCliPlanProd;
        private readonly IReincidenciaDocProdAnielRepository _repoReincidenciaDocProd;
        private readonly IDocumentoProducaoFotoAnielRepository _repoFoto;

        //private readonly IPesquisaSatisfacaoRepository _repoPesqSatisfacao;

        public DocumentoProducaoAnielBusiness(IDocumentoProducaoAnielRepository repoDocProd)
        {
            this._repoDocProd = repoDocProd;
        }
        public DocumentoProducaoAnielBusiness(IDocumentoProducaoAnielRepository repoDocProd, IDocumentoProducaoFotoAnielRepository repoFoto)
        {
            this._repoDocProd = repoDocProd;
            this._repoFoto = repoFoto;
        }
        public DocumentoProducaoAnielBusiness(IDocumentoProducaoAnielRepository repoDocProd, IReincidenciaDocProdAnielRepository repoReincidenciaDocProd)
        {
            this._repoDocProd = repoDocProd;
            this._repoReincidenciaDocProd = repoReincidenciaDocProd;
        }
        public DocumentoProducaoAnielBusiness(IDocumentoProducaoAnielRepository repoDocProd,
            INaturezaAnielRepository repoNatureza, ICodigoEncerramentoAnielRepository repoCodEncerramento,
            IContratoAnielRepository repoContrato, IConfiguracoesGeraisRepository repoConfigGerais, IClientePlanoProdutoRepository repoCliPlanProd)
        {
            this._repoDocProd = repoDocProd;
            this._repoNatureza = repoNatureza;
            this._repoCodEncerramento = repoCodEncerramento;
            this._repoContratoAniel = repoContrato;
            this._repoConfigGerais = repoConfigGerais;
            this._repoCliPlanProd = repoCliPlanProd;
            //this._repoPesqSatisfacao = repoPesquisa;
        }

        /// <summary>
        ///     Retorna a data de criação do primeiro serviço
        /// </summary>
        public DateTime? GetDataCriacaoPrimeiroServico()
        {
            int qtdRegistros =
                this._repoDocProd
                    .GetAll()
                    .Count();

            if (qtdRegistros == 0)
                return null;

            return this._repoDocProd
                       .GetAll()
                       .Select(os => os.Dia)
                       ?.Min();
        }

        /// <summary>
        ///     Retorna os serviços criados dentro do período informado
        /// </summary>
        /// <param name="dataInicio">Data inicial do período para procura dos serviços</param>
        /// <param name="dataFim">Data final do período para procura dos serviços</param>
        public IEnumerable<Documento_ProducaoAniel> GetServicosCriadosPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            dataInicio = dataInicio.Date;
            dataFim = dataFim.Date;

            return this._repoDocProd
                       .GetAll()
                       .Where(p => dataInicio <= p.Dia && p.Dia <= dataFim);
        }

        /// <summary>
        ///     Retorna o histórico de serviços agendados de acordo com o cpf e status de execução
        /// </summary>
        /// <param name="Cpf">CPF Cliente</param>
        /// <param name="Status">Status de Execução</param>
        public List<Documento_ProducaoAniel> GetServicosHistoricoByCpf(String Cpf, StatusExecucaoAniel? Status)
        {
            return this._repoDocProd
                       .GetAll()
                       .Where(p => p.Cliente.CPF == Cpf && p.Status == Status).ToList();
        }

        public List<Documento_ProducaoAniel> GetServicosHistoricoByCpf(String Cpf)
        {
            return this._repoDocProd
                       .GetAll()
                       .Where(p => p.Cliente.CPF == Cpf).ToList();
        }

        /// <summary>
        ///     Recebe uma OS e, caso ela seja uma OS reincidente, retorna a OS causadora dessa reincidência
        /// </summary>
        /// <param name="codCt">CONTRATO da OS</param>
        /// <param name="projeto">PROJETO da OS</param>
        /// <param name="numObra">NUM_OBRA da OS</param>
        /// <param name="numDoc">NUM_DOC da OS</param>
        /// <param name="dataCriacao">Data de criação da OS</param>
        public ChavesDocumentoProducao GetOSReincidente(
            string codCt,
            string projeto,
            string numObra,
            int numDoc,
            DateTime dataCriacao
        )
        {
            return this._repoDocProd
                       .GetOSReincidente(
                           codCt,
                           projeto,
                           numObra,
                           numDoc,
                           dataCriacao
                       );
        }

        /// <summary>
        /// preenche campos de tipo de serviço e codigo departamento
        /// </summary>
        /// <param name="pTipo">use LAST para retornar ultima OS da busca ou FIRST pra retornar a primeira</param>
        /// <param name="pGetConfigFluxoBaixa">Representa dados do serviço</param>
        /// <returns></returns>
        public DocumentoProducao_GetConfigFluxoBaixa GetTipoServDepOS(String pNucleo, String pTipo,
            DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa)

        {
            return this._repoDocProd
                       .GetTipoServDepOS(pNucleo,
                           pTipo,
                           pGetConfigFluxoBaixa
                       );
        }

        /// <summary>
        /// Retorna dados referentes a tabela TB_VINC_ENCERRAMENTO_NATUREZA
        /// </summary>
        /// <param name="pGetConfigFluxoBaixa">Representa dados do serviço</param>
        /// <returns></returns>
        public DocumentoProducao_GetConfigFluxoBaixa GetConfigFluxoBaixa(
            DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa)
        {
            return this._repoDocProd.GetConfigFluxoBaixa(pGetConfigFluxoBaixa);
        }

        /// <summary>
        /// Atualiza DATA_PROD
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        public void AtualizarDataProducao(Documento_ProducaoAniel documento)
        {
            this._repoDocProd.AtualizarDataProducao(documento);
        }

        /// <summary>
        /// Aplica a producao prevista pra OS
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        /// <param name="usuario">Usuario responsavel pela aplicacao</param>
        public void AplicaProducaoPrevista(Documento_ProducaoAniel documento, string usuario)
        {
            this._repoDocProd
                .AplicaProducaoPrevista(
                    documento,
                    usuario
                );
        }

        /// <summary>
        /// Insere log na tabela TB_LOG_LANC_SERV
        /// </summary>
        /// <param name="CodCt">CODCT</param>
        /// <param name="Projeto">PROJETO</param>
        /// <param name="Num_Os">NUM_OBRA</param>
        /// <param name="Num_Doc">NUM_DOC</param>
        /// <param name="Alteracao"></param>
        /// <param name="Valor_Anterior"></param>
        /// <param name="Usuario"></param>
        /// <param name="Data_Maxima">DATA_MAXIMA</param>
        /// <param name="Cod_Tipo_Serv">COD_TIPO_SERV</param>
        /// <param name="Valor_Atualizado">VALOR_ATUALIZADO</param>
        /// <param name="Obs_Cobranca_Status"></param>
        public void InsertLog_Lanc_Servico(long? CodMotivoReagendamento, String ObsMotivoReagendamento, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc,
                String Alteracao, String Valor_Anterior, String Usuario, DateTime? Data_Maxima, Int16? Cod_Tipo_Serv, String Valor_Atualizado, String Obs_Cobranca_Status)
        {
            this._repoDocProd
                .InsertLog_Lanc_Servico(CodMotivoReagendamento, ObsMotivoReagendamento,
                    CodCt,
                    Projeto,
                    Num_Os,
                    Num_Doc,
                    Alteracao,
                    Valor_Anterior,
                    Usuario,
                    Data_Maxima,
                    Cod_Tipo_Serv,
                    Valor_Atualizado,
                    Obs_Cobranca_Status
                );
        }

        /// <summary>
        /// Gera novo sequencial pra proxima OS de mesmo NUM_OBRA_ORIGINAL
        /// </summary>
        /// <param name="Codct">CODCT/Contrato da OS</param>
        /// <param name="Projeto">Projeto da OS</param>
        /// <param name="OS">Numero da OS</param>
        /// <param name="Num_Doc">NUM_DOC/Codigo do cliente</param>
        /// <returns></returns>
        public Int64 GerarSeq(String Codct, String Projeto, String OS, Int32 Num_Doc)
        {
            return this._repoDocProd
                .GerarSeq(
                    Codct,
                    Projeto,
                    OS,
                    Num_Doc
                );
        }

        /// <summary>
        /// Busca numero do serviço pra criação de novo serviço
        /// </summary>
        /// <returns></returns>
        public String GetNum_Obra(Configuracoes_Gerais config)
        {
            return this._repoDocProd.GetNum_Obra(config);
        }

        /// <summary>
        /// Busca tipo de cobranca que se encaixa com as caracteristicas do serviço
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        /// <returns></returns>
        public Cobranca_Servico BuscarCobrancaDocumento(Documento_ProducaoAniel documento)
        {
            return this._repoDocProd.BuscarCobrancaDocumento(documento);
        }

        public Documento_ProducaoAniel GerarOrdemCopia(Documento_ProducaoAniel documento, long _seq)
        {
            return new Documento_ProducaoAniel
            {
                CodCt = documento.CodCt,
                Projeto = documento.Projeto,
                Seq = Convert.ToInt32(_seq),
                Num_Obra = $"{documento.Numero_Obra_Original}/{_seq}",
                Num_Doc = documento.Num_Doc,
                OXBDPEE = documento.OXBDPEE,
                Cod_Tipo_Serv = documento.Cod_Tipo_Serv,
                Cod_Sub_Tipo_Serv = documento.Cod_Sub_Tipo_Serv,
                Usuario = documento.Usuario,
                Cep = documento.Cep,
                Latitude = documento.Latitude,
                Longitude = documento.Longitude,
                justificativaAprova = documento.justificativaAprova,
                justificativaReprova = documento.justificativaReprova,
                Cod_Tipo_Imovel = documento.Cod_Tipo_Imovel,
                Cod_Area = documento.Cod_Area,
                Cod_Escritorio = documento.Cod_Escritorio,
                Cod_Localidade = documento.Cod_Localidade,
                Bairro = documento.Bairro,
                CelularCliente = documento.CelularCliente,
                Email_Assinante = documento.Email_Assinante,
                Cidade = documento.Cidade,
                Cliente = documento.Cliente,
                Complemento = documento.Complemento,
                CPF_CLIENTE = documento.CPF_CLIENTE,
                UF = documento.UF,
                Id_Atendimento = documento.Id_Atendimento,
                Id_Cliente_PlanoProdutoConnect = documento.Id_Cliente_PlanoProdutoConnect,
                Id_PlanoProduto = documento.Id_PlanoProduto,
                Id_Venda = documento.Id_Venda,
                LoginRadius = documento.LoginRadius,
                Logradouro = documento.Logradouro,
                Nome_Cliente = documento.Nome_Cliente,
                Numero = documento.Numero,
                Unidade_Custo = documento.Unidade_Custo,
                Tarefa = documento.Tarefa,
                Pais = documento.Pais,
                Origem_Documento = documento.Origem_Documento,
                Observacao_Aux = documento.Observacao_Aux,
                Observacao = documento.Observacao,
                Titular = documento.Titular,
                Responsabilidade = ResponsabilidadeOS.COP,
                Sigla_Modulo = documento.Sigla_Modulo,
                Tipo_Producao = documento.Tipo_Producao,
                TelefonePrincipal = documento.TelefonePrincipal,
                SenhaRadius = documento.SenhaRadius,
                TelefoneComercial = documento.TelefoneComercial,
                Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                Roterizar = "N",
                Numero_Obra_Original = documento.Numero_Obra_Original,
                RG_Assinante = documento.RG_Assinante,
                Segmento = documento.Segmento,
                Latitude_Str = documento.Latitude_Str,
                Longitude_Str = documento.Longitude_Str,
                Dia = DateTime.Today,
                Hora = DateTime.Now,
                Data_Indicador = documento.Data_Indicador,
                Ssid_Wifi = documento.Ssid_Wifi,
                Senha_Wifi = documento.Senha_Wifi,
                Adicional_1 = documento.Adicional_1,
                Adicional_2 = documento.Adicional_2,
                Adicional_3 = documento.Adicional_3,
                Adicional_4 = documento.Adicional_4,
                Adicional_5 = documento.Adicional_5,
                Adicional_6 = documento.Adicional_6,
                Adicional_7 = documento.Adicional_7,
                Adicional_8 = documento.Adicional_8,
                Adicional_9 = documento.Adicional_9,
                Adicional_10 = documento.Adicional_10,
                TelefoneInstalacao = documento.TelefoneInstalacao,
                NomeInstalacao = documento.NomeInstalacao,
                CpfInstalacao = documento.CpfInstalacao,
                // Status_Cobranca=documento.Status_Cobranca
                DataAbertura = documento.DataAbertura,
                HoraAbertura = documento.HoraAbertura,
                Id_Cliente_ContratoConnect = documento.Id_Cliente_ContratoConnect,
                Tipo_Prioridade = documento.Tipo_Prioridade,
                Integracao = documento.Integracao
            };
        }

        /// <summary>
        /// Lista as observacoes feitas em cada baixa, ordenada por SEQ
        /// </summary>
        /// <param name="CodCt">CODCT</param>
        /// <param name="Projeto">PROJETO</param>
        /// <param name="Num_Os">NUM_OBRA</param>
        /// <param name="Num_Doc">NUM_DOC</param>
        public List<string> ListarObservacoesBaixa(String CodCt, String Projeto, Int32 Num_Doc, String Num_Os)
        {
            return this._repoDocProd
                .ListarObservacoesBaixa(
                    CodCt,
                    Projeto,
                    Num_Doc,
                    Num_Os
                );
        }

        public int GetNumDocManutencao()
        {
            return this._repoDocProd.GetNumDocManutencao();
        }

        /// <summary>
        /// Retorna o tempo configurado para execucao do servico baseado no tipo de imovel e tipo de servico
        /// </summary>
        /// <param name="config"></param>
        /// <param name="codTipoImovel"></param>
        /// <param name="codTipoServ"></param>
        /// <returns></returns>
        public TimeSpan? GetTempoExecucao(Configuracoes_Gerais config, int? codTipoImovel, short? codTipoServ)
        {
            return this._repoDocProd.GetTempoExecucao(config, codTipoImovel, codTipoServ);
        }

        public bool NumeroOsJaExiste(string codCt, string projeto, string numOS, int numDoc)
        {
            var os = this._repoDocProd
                       .GetAll()
                       .FirstOrDefault(p => p.CodCt == codCt &&
                                            p.Projeto == projeto &&
                                            p.Numero_Obra_Original == numOS &&
                                            p.Num_Doc == numDoc);

            return os == null ? false : true;
        }

        public bool NumeroOsJaExiste(string codCt, string projeto, string numOS)
        {
            var os = this._repoDocProd
                       .GetAll()
                       .FirstOrDefault(p => p.CodCt == codCt &&
                                            p.Projeto == projeto &&
                                            p.Numero_Obra_Original == numOS);

            return os == null ? false : true;
        }

        public bool ValidaTipoDocumentoRDO(Documento_ProducaoAniel os)
        {
            return os.Sigla_Modulo == "MCO" ? true : false;
        }

        public BaixarOsInformacoes BaixarOrdemServico(DocProdBaixa value, ISession session, bool Constantes)
        {
            BaixarOsInformacoes retorno = new BaixarOsInformacoes();

            try
            {
                //obtem documento completo
                var documento = _repoDocProd.GetFirstDocProducao(value.CodCt, value.Projeto,
                    Convert.ToInt32(value.Num_Doc), value.Num_Obra);

                if (documento is null)
                    throw new Exception("Não foi encontrado a Ordem de Serviço");

                var equipeAntiga = documento.Equipe;

                if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva
                    || documento.Status == StatusExecucaoAniel.Fechada_Improdutiva
                    || documento.Status == StatusExecucaoAniel.Atendimento_Finalizado
                    || !String.IsNullOrEmpty(documento.Cod_Encerramento))
                    throw new Exception("Ordem de Serviço já esta baixada");

                if (documento.Status == StatusExecucaoAniel.Cancelado)
                    throw new Exception("Ordem de Serviço já esta cancelada");

                if (documento.Data_Maxima == null)
                    throw new Exception("Baixa da ordem de serviço não foi realizada , informe data de agendamento");

                if (documento.Responsabilidade == ResponsabilidadeOS.TECNICO)
                    throw new Exception("Não é possível baixar o serviço, pois está sob responsabilidade do técnico.");

                //obtem codigo encerramento
                var encerramentoAniel = _repoCodEncerramento.GetFirstCodigoEncerramento(value.Codigo_Encerramento, value.CodCt);

                if (encerramentoAniel is null)
                    throw new Exception("Encerramento nao foi encontrado");

                if (encerramentoAniel.Status_Execucao == 2 && String.IsNullOrEmpty(value.Equipe))
                    throw new Exception("Baixa produtiva da ordem de serviço não foi realizada , informe técnico.");

                if (value.DataHora_Ini_Atend is null)
                    throw new Exception("A Data e Hora Início do Atendimento não estão em um formato válido.");

                if (value.DataHora_Fim_Atend is null)
                    throw new Exception("A Data e Hora Fim do Atendimento não estão em um formato válido.");

                var contrato = _repoContratoAniel.GetByCodCt(documento.CodCt);

                if (contrato.Exigir_Mudanca_Status == "S")
                {
                    if (value.DataHora_Ini_Atend.HasValue == false || value.DataHora_Fim_Atend.HasValue == false)
                        throw new Exception("data/hora início do atendimento e fim do atendimento são obrigatórios, pois o contrato utilizar a configuração Exigir Mudança de Status.");
                }

                var outrosDocumentos = _repoDocProd.GetAll().Where(p => p.Num_Obra != documento.Num_Obra && p.Numero_Obra_Original == documento.Numero_Obra_Original && p.Num_Doc == value.Num_Doc && p.Projeto == value.Projeto && p.CodCt == value.CodCt).ToList();
                var listaEncerramentos = outrosDocumentos.Select(p => _repoCodEncerramento.GetFirstCodigoEncerramento(p.Cod_Encerramento, documento.CodCt)).Where(p => p != null);

                if (contrato.Baixa_Produtiva_Duplicada == "S" && listaEncerramentos.Any(p => p.Status_Execucao == 2) && value.BaixaOsProdDuplicada != "S")
                    throw new Exception("Este serviço já foi baixado como produtivo anteriormente. Deseja realmente prosseguir com esta nova baixa?");

                var config = _repoConfigGerais.GetFirstConfigGeral();

                documento.Cod_Encerramento = encerramentoAniel.Cod_Encerramento;
                documento.Status = (StatusExecucaoAniel?)encerramentoAniel.Status_Execucao;
                documento.Data_Prod = DateTime.Today;
                documento.Hora_Prod = DateTime.Now;
                documento.Responsabilidade = ResponsabilidadeOS.COP;
                documento.Usuario_Baixa = value.Usuario;
                documento.Observacao_Teste = value.Observacao;
                var Observacao_OS = documento.Observacao_Teste;
                documento.Modulo_Baixa = "CONNECT";
                documento.Roterizar = "N";
                documento.Equipe = value.Equipe;
                documento.DataHora_Ini_Atend = value.DataHora_Ini_Atend;
                documento.DataHora_Fim_Atend = value.DataHora_Fim_Atend;
                documento.CaixaFTTH = string.IsNullOrEmpty(value.CaixaFTTH) ? documento.CaixaFTTH : value.CaixaFTTH;
                documento.PortaFTTH = string.IsNullOrEmpty(value.PortaFTTH) ? documento.PortaFTTH : value.PortaFTTH;

                Cobranca_Servico cobrancaEncontrada = BuscarCobrancaDocumento(documento);

                if (cobrancaEncontrada != null)
                {
                    if (cobrancaEncontrada.Escolha_Cobranca == Escolha_Cobranca.Aguarda_Avaliacao)
                    {
                        documento.Status_Cobranca = Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca;

                        InsertLog_Lanc_Servico(null, "", value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value,
                                               "Aguardando Avaliação", "", "", null, null, "Aguardando Avaliação",
                                               "Cobrança necessita de avaliação");
                    }
                    else
                    {
                        documento.Status_Cobranca = Status_CobrancaAniel.Automatica;

                        InsertLog_Lanc_Servico(null, "", value.CodCt, value.Projeto, value.Num_Obra, value.Num_Doc.Value,
                                               "Cobrança Automática", "", "", null, null, "Cobrança Automática",
                                               "Cobrança Automática");
                    }
                }

                _repoDocProd.SaveOrUpdate(documento);

                if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva)
                {
                    AplicaProducaoPrevista(documento, value.Usuario);
                    AtualizarDataProducao(documento);
                }

                if (documento.Equipe != equipeAntiga)
                {
                    InsertLog_Lanc_Servico(documento.CodMotivoReagendamento, documento.ObsMotivoReagendamento, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc,
                            documento.Equipe == null ? "Remoção de Equipe" : "Equipe", $"{equipeAntiga}",
                           documento.Usuario_Baixa, null, null, documento.Equipe, null);
                }

                InsertLog_Lanc_Servico(documento.CodMotivoReagendamento, documento.ObsMotivoReagendamento, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Baixou OS",
                    null, documento.Usuario_Baixa, documento.Data_Maxima,
                    documento.Cod_Tipo_Serv, documento.Num_Obra + $" - Encerramento: {encerramentoAniel.Cod_Encerramento} - {encerramentoAniel.Descricao}",
                    value.Observacao);

                DocumentoProducao_GetConfigFluxoBaixa _getConfigFluxoBaixa = new DocumentoProducao_GetConfigFluxoBaixa();
                Int32 _codNatureza = _repoNatureza.GetNaturezaByCodTipoServ(Convert.ToInt32(documento.Cod_Tipo_Serv), documento.CodCt);
                _getConfigFluxoBaixa.codCt = documento.CodCt;
                _getConfigFluxoBaixa.projeto = documento.Projeto;
                _getConfigFluxoBaixa.codEncerramento = documento.Cod_Encerramento;
                _getConfigFluxoBaixa.codNatureza = _codNatureza;
                _getConfigFluxoBaixa.numObraOriginal = documento.Numero_Obra_Original;
                _getConfigFluxoBaixa.numDoc = documento.Num_Doc;
                _getConfigFluxoBaixa = GetConfigFluxoBaixa(_getConfigFluxoBaixa);

                //Adicionado para o processo do novo fluxo de baixa ...

                var oldCodTipoServ = documento.Cod_Tipo_Serv;
                var oldCodDepto = documento.CodDepto;
                Int16? codTipoServFluxo = null;
                Int16? codDeptoFluxo = null;

                if (_getConfigFluxoBaixa.fluxoAoEncerrar == "S")
                {
                    if (_getConfigFluxoBaixa.comportamentoTipoServ == 1)
                    {
                        codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                    }
                    else if (_getConfigFluxoBaixa.comportamentoTipoServ == 2)
                    {
                        _getConfigFluxoBaixa = GetTipoServDepOS("TIPOSERV", "LAST", _getConfigFluxoBaixa);
                        codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                    }
                    else if (_getConfigFluxoBaixa.comportamentoTipoServ == 3)
                    {
                        _getConfigFluxoBaixa = GetTipoServDepOS("TIPOSERV", "FIRST", _getConfigFluxoBaixa);
                        codTipoServFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);
                    }

                    if (_getConfigFluxoBaixa.comportamentoSetor == 1)
                    {
                        codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                    }
                    else if (_getConfigFluxoBaixa.comportamentoSetor == 2)
                    {
                        _getConfigFluxoBaixa = GetTipoServDepOS("SETOR", "LAST", _getConfigFluxoBaixa);
                        codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                    }
                    else if (_getConfigFluxoBaixa.comportamentoSetor == 3)
                    {
                        _getConfigFluxoBaixa = GetTipoServDepOS("SETOR", "FIRST", _getConfigFluxoBaixa);
                        codDeptoFluxo = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                    }
                }

                if (encerramentoAniel.Encerra_OS_Apos_Baixa == "N")
                {
                    var novoDocumento = GerarOrdemCopia(documento, GerarSeq(documento.CodCt, documento.Projeto, documento.Numero_Obra_Original, documento.Num_Doc));

                    if (Observacao_OS != null)
                        novoDocumento.Observacao_Teste = Observacao_OS;

                    if (codTipoServFluxo != null)
                        novoDocumento.Cod_Tipo_Serv = codTipoServFluxo;

                    if (codDeptoFluxo != null)
                        novoDocumento.CodDepto = codDeptoFluxo;

                    _repoDocProd.SaveOrUpdate(novoDocumento);

                    InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, documento.Num_Obra, novoDocumento.Num_Doc, "Fluxo - Criação Aut. Serviço", documento.Num_Obra,
                      documento.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, novoDocumento.Num_Obra, null);

                }
                else if (encerramentoAniel.Encerra_OS_Apos_Baixa == "S")
                {
                    var novoDocumento = GerarOrdemCopia(documento, GerarSeq(documento.CodCt, documento.Projeto, documento.Numero_Obra_Original, documento.Num_Doc));
                    var novoNumObra = GetNum_Obra(config);
                    novoDocumento.Num_Obra = novoNumObra + "/1";
                    novoDocumento.Numero_Obra_Original = novoNumObra;
                    novoDocumento.Seq = 1;
                    novoDocumento.CodDepto = Convert.ToInt16(_getConfigFluxoBaixa.codDepto);
                    novoDocumento.Cod_Tipo_Serv = Convert.ToInt16(_getConfigFluxoBaixa.codTipoServ);

                    if (_getConfigFluxoBaixa.fluxoAoEncerrar == "S")
                    {
                        InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, documento.Num_Obra, novoDocumento.Num_Doc, "Fluxo - Criação Aut. Serviço", null,
                              documento.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, novoDocumento.Num_Obra, null);

                        if ((oldCodDepto != novoDocumento.CodDepto) && (_getConfigFluxoBaixa.codDepto != null))
                            InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, documento.Num_Obra, novoDocumento.Num_Doc, "Alteração no Departamento na criação do documento: " + novoDocumento.Num_Obra,
                                Convert.ToString(oldCodDepto), documento.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, Convert.ToString(novoDocumento.CodDepto), null);

                        if ((oldCodTipoServ != novoDocumento.Cod_Tipo_Serv) && (_getConfigFluxoBaixa.codTipoServ != null))
                            InsertLog_Lanc_Servico(novoDocumento.CodMotivoReagendamento, novoDocumento.ObsMotivoReagendamento, novoDocumento.CodCt, novoDocumento.Projeto, documento.Num_Obra, novoDocumento.Num_Doc, "Alteração do Tipo de Serviço na criação do documento: " + novoDocumento.Num_Obra,
                                Convert.ToString(oldCodTipoServ), documento.Usuario_Baixa, novoDocumento.Data_Maxima, novoDocumento.Cod_Tipo_Serv, Convert.ToString(novoDocumento.Cod_Tipo_Serv), null);

                        if (_getConfigFluxoBaixa.utilizaNatureza == "S")
                            _repoDocProd.SaveOrUpdate(novoDocumento);
                    }
                }

                retorno.DocProducao = documento;
                retorno.Mensagem = "Baixa Efetuada com Sucesso!";
                retorno.Ok = true;

            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException;
                retorno.Ok = false;
            }

            return retorno;
        }

        public void HabilitarPlano(ISession session, Documento_ProducaoAniel documento, bool Constantes)
        {
            // ******* Alterando o status dos Planos/Produtos da OS  *******
            var planoProduto = documento.Id_Cliente_PlanoProdutoConnect.HasValue ? _repoCliPlanProd.GetById(documento.Id_Cliente_PlanoProdutoConnect.Value) : null;
            if (planoProduto?.Status == Status_PlanoProduto.Aguardando_Instalacao ||
                planoProduto?.Status == Status_PlanoProduto.Aguardando_Criacao_OS)
            {
                Status_PlanoProduto? status_PlanoAntigo = planoProduto.Status;
                IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, HibernateUtil.GetSessionT5());
                var elementoRedePortaBusiness = new ElementoRedePortaAnielBusiness(repository);
                Elemento_ReservaCadastroModel elemento_ = new Elemento_ReservaCadastroModel();
                using (var repositoryLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
                {

                    if (planoProduto?.Id_TrocaPlano > 0)
                    {
                        using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                        using (var repoCC = new Conexao<ClienteContrato>(session))
                        using (var repositoryONU = new Conexao<ONU>(session))
                        using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                        using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
                        using (var repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
                        {
                            var objPlanoDescontinuado = _repoCliPlanProd.GetById((long)planoProduto.Id_TrocaPlano);
                            var objONU = repositoryONU.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id_TrocaPlano).Result;

                            objPlanoDescontinuado.Status = Status_PlanoProduto.Cancelado_Mudanca_Plano;
                            objPlanoDescontinuado.Motivo_Mudanca_PP = Motivo_MudancaPlano_Produto.Motivo_Mudanca_PP;

                            var atualizaPlanoDescontinuadoLog = new Cliente_PlanoProduto_StatusLog
                            {
                                AntigoStatus = Status_PlanoProduto.Aguardando_Instalacao,
                                NovoStatus = objPlanoDescontinuado.Status,
                                PlanoProduto = objPlanoDescontinuado,
                                ClienteContrato = objPlanoDescontinuado.ClienteContrato,
                                DataHoraAcao = DateTime.Now
                            };

                            #region [Objeto - ClienteContrato]
                            planoProduto.ClienteContrato = objPlanoDescontinuado.ClienteContrato;
                            #endregion


                            var objLoginRadius_PlanoNovo = repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id).Result;
                            if (objLoginRadius_PlanoNovo != null)
                            {
                                objLoginRadius_PlanoNovo.Cliente_PlanoProduto = null;

                                if (objLoginRadius_PlanoNovo.Pool_Ip_Endereco != null)
                                {
                                    objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Login = null;
                                    objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                    repoPoolIpEndereco.Update(objLoginRadius_PlanoNovo.Pool_Ip_Endereco).Wait();
                                }

                                repositoryLogin.Remove(objLoginRadius_PlanoNovo).Wait();
                            }

                            var objLoginRadius = repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id_TrocaPlano).Result;
                            if (objLoginRadius != null)
                            {
                                objLoginRadius.Cliente_PlanoProduto = planoProduto;
                                /*
                                * Limpeza da porta do novo plano, pois está reservada automaticamente
                                * *E é necessário, que a porta, esteja na mesma do antigo. 
                                * **/

                                elemento_.Login = objLoginRadius.Login;
                                elemento_.Cliente_PlanoProduto_Novo = planoProduto.Id;
                                repository.LimparPorta(elemento_).Wait();

                                elemento_.Cliente_PlanoProduto_Antigo = planoProduto.Id_TrocaPlano.Value;
                                repository.AlterarPortaPlano(elemento_, true).Wait();

                                if (planoProduto?.Plano?.AutenticacaoRadius == true && objLoginRadius?.Concentradora?.Radius != null)
                                {
                                    var sessionRadius = HibernateUtil.GetSessionRadius();
                                    var anielRadiusRepo = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(objLoginRadius?.Concentradora?.Radius, sessionRadius);
                                    new IntegracaoRadiusBusiness(anielRadiusRepo).EditarUsuario(objLoginRadius, Constantes).Wait();
                                    if (objLoginRadius.Status == Status_Default.Inativo)
                                    {
                                        var poolsBloqueio = repoPoolIp.select(p => p.IsPoolBloqueio == true && p.Concentradora.Id == objLoginRadius.Concentradora.Id).Result;
                                        new IntegracaoRadiusBusiness(anielRadiusRepo).BloquearUsuario(objLoginRadius, Constantes, poolsBloqueio).Wait();
                                    }
                                    else
                                    {
                                        new IntegracaoRadiusBusiness(anielRadiusRepo).DesbloquearUsuario(objLoginRadius, Constantes).Wait();
                                    }
                                }

                                repositoryLogin.SaveOrUpdate(objLoginRadius).Wait();
                            }

                            if (objONU != null)
                            {
                                objONU.Cliente_PlanoProduto = planoProduto;
                                repositoryONU.SaveOrUpdate(objONU).Wait();
                            }

                            _repoCliPlanProd.Save(objPlanoDescontinuado);
                            _repoCliPlanProd.SaveOrUpdateLog(atualizaPlanoDescontinuadoLog);
                        }
                    }
                    else
                    {
                        var objLoginRadius = repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id).Result;

                        if (objLoginRadius != null)
                        {
                            elemento_.Login = objLoginRadius.Login;
                        }
                        elemento_.Cliente_PlanoProduto_Novo = planoProduto.Id;
                        elemento_.Cliente_PlanoProduto_Antigo = planoProduto.Id;
                        repository.AlterarPortaPlano(elemento_, true).Wait();
                    }
                }

                planoProduto.Status = Status_PlanoProduto.Habilitado;
                _repoCliPlanProd.Save(planoProduto);

                var atualizaPlanoLog = new Cliente_PlanoProduto_StatusLog
                {
                    AntigoStatus = status_PlanoAntigo,
                    NovoStatus = Status_PlanoProduto.Habilitado,
                    PlanoProduto = planoProduto,
                    ClienteContrato = planoProduto.ClienteContrato,
                    DataHoraAcao = documento.Data_Prod.HasValue && documento.Hora_Prod.HasValue ? documento.Data_Prod.Value.Add(documento.Hora_Prod.Value.TimeOfDay) : DateTime.Now
                };

                _repoCliPlanProd.SaveOrUpdateLog(atualizaPlanoLog);
            }
        }

        public async Task<List<ValidacaoCobDocProd>> ValidarCobDocProd(Documento_ProducaoAniel docProd, int? statusAprovado)
        {
            List<ValidacaoCobDocProd> retorno = new List<ValidacaoCobDocProd>();
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoFormaPag = new Conexao<Forma_Pagamento>(session))
                using (var repositoryTipoServico = new Conexao<TipoServico_Cobranca>(session))
                using (var repositoryPlano_Produto_Cobranca = new Conexao<Plano_Produto_Cobranca>(session))
                {
                    var listaTipoServicoCobranca = await repositoryTipoServico.select(x => x.Segmento == docProd.CodCt &&
                    x.Tipo_Servico == Convert.ToString(docProd.TipoServico.Cod_Tipo_Serv) &&
                    ((x.Sub_Tipo_Servico == docProd.Cod_Sub_Tipo_Serv) || (x.TodasSubTipoServico == true)) &&
                    ((x.Cod_Encerramento == docProd.Cod_Encerramento) || (x.TodasCod_Encerramento == true)));

                    var objTipoServicoCobranca = listaTipoServicoCobranca.FirstOrDefault(x => x.Cobranca_Servico.Status == Status_Default.Ativo);
                    if (objTipoServicoCobranca != null)
                    {
                        var listaPlanoProdutoCobranca = await repositoryPlano_Produto_Cobranca.select(x => x.Cobranca_Servico.Id == objTipoServicoCobranca.Cobranca_Servico.Id);
                        var objCliente_PlanoProduto = await repoCliente_PlanoProduto.selectFirst(x => x.Id == docProd.Id_Cliente_PlanoProdutoConnect);

                        if (objTipoServicoCobranca?.Cobranca_Servico?.Escolha_Cobranca == Escolha_Cobranca.Cobranca_Automatica || statusAprovado == 2)
                        {
                            /*APROVADO*/
                            docProd.Status_Cobranca = statusAprovado == 2 ? Status_CobrancaAniel.Aprovado : Status_CobrancaAniel.Automatica;
                            foreach (var objPlanoProdutoCobranca in listaPlanoProdutoCobranca)
                            {
                                retorno.Add(new ValidacaoCobDocProd
                                {
                                    FormaPagamento = objPlanoProdutoCobranca.Plano?.Cobranca_Realizada == Cobranca_Realizada.Boleto ? await repoFormaPag.selectFirst(x => x.Id == objPlanoProdutoCobranca.Plano.FormaPagamentoCob.Id) : objCliente_PlanoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento,
                                    DiasVencimentoBoleto = objPlanoProdutoCobranca.Plano?.Dias_Vencimento,
                                    MomentoCobranca = objPlanoProdutoCobranca.Plano?.Momento_Cobranca.Value,
                                    Message = objPlanoProdutoCobranca.Plano?.Momento_Cobranca == Momento_Cobranca.Baixa_Servico ? "Configuração de Cobrança de Serviço Automático habilitado - Gerada uma conta para este cliente." : "Configuração de Cobrança de Serviço habilitado - valor será cobrado apenas no próximo fechamendo do faturamento para este cliente.",
                                    IdPlanoProduto = Convert.ToInt64(objPlanoProdutoCobranca.Plano?.Id),
                                    Status = true
                                });
                            }
                        }
                        else if (objTipoServicoCobranca?.Cobranca_Servico?.Escolha_Cobranca == Escolha_Cobranca.Aguarda_Avaliacao)
                        {
                            docProd.Status_Cobranca = Status_CobrancaAniel.Aguardando_Avaliacao_Cobranca;
                        }
                    }
                    _repoDocProd.SaveOrUpdate(docProd);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return retorno;
        }





        public class ValidacaoCobDocProd
        {
            public Momento_Cobranca? MomentoCobranca { get; set; }
            public double ValorCobrar { get; set; }
            public Forma_Pagamento FormaPagamento { get; set; }
            public bool Status { get; set; }
            public string Message { get; set; }
            public long? DiasVencimentoBoleto { get; set; }
            public long IdPlanoProduto { get; set; }
        }

        public async Task ParalisarAtendimento(string codCt, string projeto, int numDoc, string numObra, string justificativa, string usuario)
        {
            Documento_ProducaoAniel documento = this._repoDocProd.GetAll().Where(p => p.CodCt == codCt && p.Num_Doc == numDoc && p.Num_Obra == numObra && p.Projeto == projeto).FirstOrDefault();
            if (documento.Status != StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado && documento.Status != StatusExecucaoAniel.Reiniciado_Atendimento)
            {
                throw new Exception(String.Format(RsBase.ORDEM_SERVICO_SAT, documento.Numero_Obra_Original));
            }

            if (documento.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                      documento.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                      documento.Status == StatusExecucaoAniel.Atendimento_Finalizado)
            {
                throw new Exception(String.Format(RsBase.ORDEM_SERVICO_BAIXA_PAR, documento.Numero_Obra_Original));
            }


            documento.Status = StatusExecucaoAniel.Atendimento_Paralizado;
            documento.DataHora_Pausou = DateTime.Now;
            documento.ObservacaoPausa = StringService.Truncate(justificativa, 255);
            await this._repoDocProd.SaveUpdateDocumentoAsync(documento);
            var ordemlocalizada = this._repoDocProd.GetOSMongo(documento);
            var status = new List<StatusDocumentoProducao>();
            SenhaAniel usuarioAniel = await this._repoDocProd.GetSenhaAniel(usuario);
            FuncionarioAniel funcionario = null;
            if (usuarioAniel != null)
            {
                funcionario = await this._repoDocProd.GetFuncionario(usuarioAniel.NumFre);
            }

            status.Add(new StatusDocumentoProducao()
            {

                statusExecucao = (int)StatusExecucaoAniel.Atendimento_Paralizado,
                equipe = usuario,
                nome = funcionario?.Nome,
                numFre = funcionario?.Num_Fre,
                data = DateTime.Now,
                dataHoraServidor = DateTime.Now,
                hora = DateTime.Now,
                latitude = 0,
                longitude = 0,
                justificativaPausaMongo = justificativa

            });
            if (ordemlocalizada != null)
            {
                //ordemServicoLocalizada.justificativaPausaMongo = localizacao.documentoProducao.justificativaPausa != null ? localizacao.documentoProducao.justificativaPausa : null;
                ordemlocalizada.status.AddRange(status);

                await this._repoDocProd.UpdateMongo(ordemlocalizada);
            }
            else
            {
                var documentoMongo = new DocumentoProducaoMongo()
                {
                    codCt = documento.CodCt,
                    projeto = documento.Projeto,
                    contrato = documento.Num_Doc,
                    numObra = documento.Num_Obra,
                    numObraOriginal = documento.Numero_Obra_Original,
                    dataAgendamento = documento.Data_Maxima.HasValue ? documento.Data_Maxima.Value : new DateTime(),
                    status = status

                };
                await this._repoDocProd.SaveMongo(documentoMongo);
            }
            if (documento.EquipeAniel != null)
            {
                MobileService.NotificarSmartphone(documento.EquipeAniel.Device_Id, String.Empty, "Houve mudanas em sua rotina.",
                                                            "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS",
                                                            "", "", "", "");
            }

        }

        public async void AtualizaVinculoManutencaoOSs(VincularOsModel request)
        {
            Int32 recordsTotal = 0;
            try
            {
                string[] splitManutencao = request.Projeto_Manut.Split(new string[] { "__" }, StringSplitOptions.None);
                var numobra = splitManutencao[0].ToString();
                var codct = splitManutencao[1].ToString();
                var projeto = splitManutencao[2].ToString();
                var numdoc = splitManutencao[3].ToString();
                List<AlteraVinculoManutencao> chaves_documento = request.ids.Select(chave =>
                {
                    String[] _chaves = chave.Split('|');
                    String CodCt = _chaves[0];
                    String Projeto = _chaves[1];
                    String Num_Obra = _chaves[2];
                    Int32 Num_Doc = Convert.ToInt32(_chaves[3]);
                    return new AlteraVinculoManutencao
                    {
                        CodCt = CodCt,
                        numDoc = Num_Doc,
                        Num_Obra = Num_Obra,
                        Projeto = Projeto
                    };

                }).ToList();
                foreach (var item in chaves_documento)
                {
                    Documento_ProducaoAniel documento = this._repoDocProd.GetAll().Where(p => p.CodCt == item.CodCt && p.Num_Doc == item.numDoc && p.Num_Obra == item.Num_Obra && p.Projeto == item.Projeto).FirstOrDefault();

                    if (documento.Tipo != "M")
                    {
                        item.Projeto_Manut = request.Projeto_Manut;
                        await this._repoDocProd.AtualizaVinculoManutencao(item);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        public List<Documento_ProducaoAniel> GetOSManutencao(DateTime dtInicio, DateTime dtFinal)
        {
            List<Documento_ProducaoAniel> lstOs = this._repoDocProd.GetAll().Where(X => X.Tipo == "M" && X.Dia >= dtInicio && X.Dia <= dtFinal).ToList();
            return lstOs;
        }

        public async Task<Response> CriarOsIntegracao(OsApiModel request)
        {
            Response response = new Response();
            #region Condicao
            var cpf = "";
            if (String.IsNullOrEmpty(request.Tipo))
            {
                request.Tipo = "C";
            }
            if (request.Tipo != "C" && request.Tipo != "M")
            {
                throw new Exception("Tipo de OS não é válido.");
            }

            if (!String.IsNullOrEmpty(request.Cpf) && request.Tipo == "C")
            {
                cpf = request.Cpf.Replace(".", "").Replace("-", "").Replace("/", "");
            }
            if (cpf.Length > 20)
            {
                throw new Exception("Houve um problema no campo de CPF.");
            }
            if (String.IsNullOrEmpty(request.TipoServico))
            {
                throw new Exception("Houve um problema no campo de Tipo de Serviço.");
            }

            //if (String.IsNullOrEmpty(request.TipoAtendimento.ToString()) || request.TipoAtendimento.ToString().Length > 9)
            //{ throw new Exception("Houve um problema no Campo de CEP."); }

            #endregion



            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoCT = new Conexao<ContratoAniel>(sessionT5))
                using (var repoProjeto = new Conexao<ProjetoAniel>(sessionT5))
                using (var repoClienteA = new Conexao<ClienteAniel>(sessionT5))
                using (var repoClienteC = new Conexao<Cliente>(session))
                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                using (var repoEndereco = new Conexao<Endereco_Cliente>(session))
                using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
                using (var repoTipoE = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                using (var reposub = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
                using (var repoU = new Conexao<Unidade_CustoAniel>(sessionT5))
                using (var repoImovel = new Conexao<Tipo_ImovelAniel>(sessionT5))
                using (var repoEscritorio = new Conexao<EscritorioAniel>(sessionT5))
                using (var repoArea = new Conexao<AreaAniel>(sessionT5))
                using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
                using (var repoCategoria = new Conexao<Categoria_ClienteAniel>(sessionT5))
                using (var repoCategoriaConnect = new Conexao<Categoria_Cliente>(session))
                using (var repoTipoImovel = new Conexao<Tipo_ImovelAniel>(sessionT5))
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (var repositoryPop = new Conexao<POP>(session))
                using (var repositoryOlt = new Conexao<OLT>(session))
                using (var repositoryElementoRede = new Conexao<Elemento_Rede>(sessionT5))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var repoSegmento = new Conexao<Segmento>(sessionT5))
                using (var repoCidade = new Conexao<Cidade>(session))
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                    TurnoRepository turnoRepository = new TurnoRepository(sessionT5);
                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    IEquipeAnielRepository equipeAnielRepository = new EquipeAnielRepository(sessionT5);
                    var business = new DocumentoProducaoAnielBusiness(_repoDocProd);

                    #region CriarOs


                    #region objetos
                    Documento_ProducaoAniel doc = new Documento_ProducaoAniel();
                    EquipeIntegracaoModel equipe = request.Tecnico != null ? equipeAnielRepository.GetEquipe(request.Tecnico) : null;

                    ContratoAniel contrato = null;
                    ProjetoAniel projeto = null;
                    Unidade_CustoAniel custo = null;
                    ClienteContrato clienteContrato = null;
                    ClienteContrato clienteContratoAnterior = null;
                    Categoria_ClienteAniel categoria = null;
                    LoginRadius loginRadius = await repoLoginRadius.selectFirst(p => p.Login == request.LoginRadius);

                    if (!String.IsNullOrEmpty(request.CategoriaCliente))
                        categoria = await repoCategoria.selectFirst(p => p.Descricao == request.CategoriaCliente);

                    ClienteAniel clientA = !String.IsNullOrEmpty(cpf) ? await repoClienteA.selectFirst(p => p.CPF == cpf) : null;
                    EscritorioAniel escritorio = await repoEscritorio.selectFirst(x => x.Descricao == StringService.Truncate(request.GrupoArea, 70));
                    AreaAniel area = escritorio != null ? await repoArea.selectFirst(x => x.Cod_Escritorio == escritorio.Cod_Escritorio && x.Descricao == StringService.Truncate(request.Area, 35)) : null;
                    LocalidadeAniel localidade = area != null ? await repoLocalidade.selectFirst(x => x.Cod_Escritorio == area.Cod_Escritorio && x.Cod_Area == area.Cod_Area && x.LocalidadeStr == StringService.Truncate(request.Localidade, 50)) : null;
                    #endregion

                    if (contrato == null && !String.IsNullOrEmpty(request.CodCt))
                    {
                        contrato = await repoCT.selectFirst(p => p.CodCt == request.CodCt);
                        if (contrato == null)
                            throw new Exception("O contrato buscado não foi encontrado");
                    }

                    if (equipe != null && contrato == null && projeto == null)
                    {
                        contrato = await repoCT.selectFirst(p => p.CodCt == equipe.CodCtPadrao);
                        projeto = await repoProjeto.selectFirst(p => p.CodCt == equipe.CodCtPadrao && p.Codigo == equipe.ProjetoPadrao);
                    }

                    if (localidade != null && contrato == null && projeto == null)
                    {
                        contrato = await repoCT.selectFirst(p => p.CodCt == localidade.CodCt);
                        projeto = await repoProjeto.selectFirst(p => p.CodCt == localidade.CodCt && p.Codigo == localidade.Projeto);
                    }

                    if (contrato != null && projeto == null)
                    {
                        projeto = await repoProjeto.selectFirst(p => p.CodCt == contrato.CodCt && p.Codigo == request.Projeto);
                    }

                    Cidade cidade = null;
                    if (!String.IsNullOrEmpty(request.Cidade) && !String.IsNullOrEmpty(request.UF))
                    {
                        cidade = await repoCidade.selectFirst(p => p.Descricao == request.Cidade.ToUpper() && p.UF == StringService.Truncate(request.UF, 2).ToUpper() && p.Id_UF != null);
                    }
                    long? IdCidadeConnect = cidade?.Id;
                    int CodIBGEConnect = Convert.ToInt32(cidade?.Codigo_IBGE);

                    using (var repoMunicipio = new Conexao<MunicipioAniel>(sessionT5))
                    {
                        MunicipioAniel cidadeAniel = await repoMunicipio.selectFirst(p => p.CodIBGE == CodIBGEConnect);
                        short? IdCidade = cidadeAniel?.CodMunicipio;
                        if (request.Tipo != "M")
                        {
                            if (clientA == null)
                            {
                                clientA = new ClienteAniel
                                {
                                    NomeFantasia = request.NomeCliente,
                                    CPF = cpf,
                                    Telefone_Celular = request.TelefoneCelularCliente,
                                    Telefone_Fixo = request.TelefoneFixoCliente,
                                    Email_Principal = StringService.Truncate(request.EmailCliente, 150),
                                    Data_Cadastro = DateTime.Now,
                                    Tipo_Pessoa = cpf.Length > 11 ? Tipo_Pessoa.Juridica : Tipo_Pessoa.Fisica,
                                    Status = Status_Cliente.Ativo,
                                    Cod_Cliente = Convert.ToInt32(repoClienteA.Max(p => p.Cod_Cliente).Result) + 1,
                                    Categoria_Cliente = categoria,
                                    Cep = StringService.Truncate(request.Cep, 10),
                                    Bairro = StringService.Truncate(request.Bairro, 30),
                                    UF = StringService.Truncate(request.UF, 2),
                                    Cidade = IdCidade,
                                    Numero = StringService.Truncate(request.NumeroEndereco, 10),
                                    Logradouro = StringService.Truncate(request.Endereco, 100),


                                };

                                try
                                {
                                    await repoClienteA.SaveOrUpdate(clientA);
                                }
                                catch (Exception ex)
                                {
                                    if (ex.InnerException.Message.Contains("EX_VERIF_CLIENTE_DUPLICADO"))
                                        throw new Exception("Já existe um cliente cadastrado com este CPF/CNPJ");
                                    else
                                        throw new Exception(ex.Message);
                                }
                            }
                            else
                            {
                                clientA.NomeFantasia = request.NomeCliente;
                                clientA.CPF = cpf;
                                clientA.Telefone_Celular = request.TelefoneCelularCliente;
                                clientA.Telefone_Fixo = request.TelefoneFixoCliente;
                                clientA.Email_Principal = StringService.Truncate(request.EmailCliente, 150);
                                clientA.Status = Status_Cliente.Ativo;
                                clientA.Categoria_Cliente = categoria;
                                clientA.Cep = StringService.Truncate(request.Cep, 10);
                                clientA.Bairro = StringService.Truncate(request.Bairro, 30);
                                clientA.UF = StringService.Truncate(request.UF, 2);
                                clientA.Cidade = IdCidade;
                                clientA.Numero = StringService.Truncate(request.NumeroEndereco, 10);
                                clientA.Logradouro = StringService.Truncate(request.Endereco, 100);


                                await repoClienteA.SaveOrUpdate(clientA);
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(request.NumDoc) == false)
                    {
                        int numDoc;
                        if (Int32.TryParse(request.NumDoc, out numDoc))
                        {
                            doc.Num_Doc = numDoc;
                        }
                        else
                        {
                            throw new Exception("erro ao tentar converter NumDoc");
                        }
                    }
                    else
                    {

                        if (request.Tipo == "M")
                            doc.Num_Doc = documentoBusiness.GetNumDocManutencao();
                        else
                            doc.Num_Doc = Convert.ToInt32(clientA.Cod_Cliente);
                    }

                    if (request.UsarNumeroSequencial)
                    {
                        request.NumOS = business.GetNum_Obra(config);
                        doc.Numero_Obra_Original = request.NumOS;
                    }
                    else if (!request.UsarNumeroSequencial && !String.IsNullOrEmpty(request.NumOS))
                    {
                        doc.Numero_Obra_Original = request.NumOS;
                    }
                    else
                    {
                        throw new Exception("Número da OS não preenchido");
                    }

                    if (request.CadastrarSequencia)
                    {
                        doc.Seq = (int)business.GerarSeq(contrato.CodCt, projeto.Codigo, request.NumOS, doc.Num_Doc);
                    }
                    else
                    {
                        doc.Seq = 1;
                    }


                    doc.Num_Obra = request.NumOS + $"/{doc.Seq}";
                    doc.WO = request.Wo;
                    List<Documento_ProducaoAniel> OsOriginais = new List<Documento_ProducaoAniel>();
                    if (request.CadastrarSequencia)
                    {
                        OsOriginais = (await repoDoc.select(p => p.CodCt == contrato.CodCt
                                        && p.Num_Obra == doc.Num_Obra
                                        && p.Projeto == projeto.Codigo
                                        && p.Num_Doc == doc.Num_Doc)).ToList();
                    }
                    else
                    {
                        OsOriginais = (await repoDoc.select(p => p.CodCt == contrato.CodCt
                                        && p.Numero_Obra_Original == request.NumOS
                                        && p.Projeto == projeto.Codigo
                                        && p.Num_Doc == doc.Num_Doc)).ToList();
                    }


                    if (OsOriginais.Count > 0)
                        throw new Exception("Ordem de servico ja cadastrada");

                    Cliente client = await repoClienteC.selectFirst(p => p.CpfCnpj == cpf);
                    Tipo_ImovelAniel tipoImovel = await repoImovel.selectFirst(x => x.Descricao == request.TipoImovel);

                    Categoria_Cliente categoriaConnect = null;
                    if (!String.IsNullOrEmpty(request.CategoriaCliente))
                        categoriaConnect = await repoCategoriaConnect.selectFirst(p => p.Descricao == request.CategoriaCliente);

                    Cliente_PlanoProduto PlanoProduto = request.Plano?.Id != null && client != null ?
                                                        await repoClientePlano.selectFirst(p => p.Cliente.Id == client.Id && p.Plano.Id == request.Plano.Id)
                                                        : null;

                    Cliente_PlanoProduto PlanoProdutoAnterior = request.PlanoAnterior?.Id != null && client != null ?
                                                        await repoClientePlano.selectFirst(p => p.Cliente.Id == client.Id && p.Plano.Id == request.PlanoAnterior.Id)
                                                        : null;

                    Tipo_Servico_EquipeAniel serv = await repoTipoE.selectFirst(p => p.Descricao == StringService.Truncate(request.TipoServico, 150) &&
                                                                                     p.CodCt == contrato.CodCt);

                    if (request.NaoCriaOsSemTipo && serv == null)
                        throw new Exception($"Tipo servico {request.TipoServico} nao encontrado");

                    if (escritorio == null && projeto != null)
                    {
                        escritorio = await repoEscritorio.selectFirst(x => x.Descricao == projeto.CodEscritorio);
                    }

                    if (categoria == null && String.IsNullOrEmpty(request.CategoriaCliente) == false)
                    {
                        categoria = new Categoria_ClienteAniel { Id = Convert.ToInt16(repoCategoria.Max(p => p.Id).Result + 1), Descricao = StringService.Truncate(request.CategoriaCliente, 100) };
                        await repoCategoria.Save(categoria);
                    }

                    if (categoriaConnect == null && String.IsNullOrEmpty(request.CategoriaCliente) == false)
                    {
                        categoriaConnect = new Categoria_Cliente { Id = 0, Descricao = StringService.Truncate(request.CategoriaCliente, 100), Status = Status_Default.Ativo };
                        await repoCategoriaConnect.Save(categoriaConnect);
                    }

                    //Pega o resultado que é igual ao plano digitado
                    #region condicoes
                    if (request.Tipo != "M")
                    {
                        if (client == null)
                        {
                            client = new Cliente
                            {
                                NomeFantasia = request.NomeCliente,
                                CpfCnpj = cpf,
                                Telefone_Celular = request.TelefoneCelularCliente,
                                Telefone_Fixo = request.TelefoneFixoCliente,
                                Email_Principal = StringService.Truncate(request.EmailCliente, 150),
                                Data_Criacao_Cliente = DateTime.Now,
                                Tipo_Pessoa = cpf.Length > 11 ? Tipo_Pessoa.Juridica : Tipo_Pessoa.Fisica,
                                Tipo_Cliente = Tipo_Cliente.Cliente,
                                Status = Status_Cliente.Ativo,
                                Id = 0,
                                Endereco_Principal = new Endereco_Cliente
                                {
                                    Cep = StringService.Truncate(request.Cep, 10),
                                    Bairro = StringService.Truncate(request.Bairro, 50),
                                    UF = StringService.Truncate(request?.UF, 2),
                                    Cidade = StringService.Truncate(request?.Cidade, 50),
                                    Numero = StringService.Truncate(request.NumeroEndereco, 10),
                                    Logradouro = StringService.Truncate(request.Endereco, 100),
                                    Id_Cidade = IdCidadeConnect
                                },
                                Categoria_Cliente = categoriaConnect

                            };

                            await repoClienteC.SaveOrUpdate(client);
                        }
                        else
                        {
                            client.NomeFantasia = request.NomeCliente;
                            client.CpfCnpj = cpf;
                            client.Telefone_Celular = request.TelefoneCelularCliente;
                            client.Telefone_Fixo = request.TelefoneFixoCliente;
                            client.Email_Principal = StringService.Truncate(request.EmailCliente, 150);
                            client.Status = Status_Cliente.Ativo;
                            client.Tipo_Cliente = Tipo_Cliente.Cliente;

                            if (client.Endereco_Principal == null)
                            {
                                client.Endereco_Principal = new Endereco_Cliente
                                {
                                    Cep = StringService.Truncate(request.Cep, 10),
                                    Bairro = StringService.Truncate(request.Bairro, 50),
                                    UF = StringService.Truncate(request?.UF, 2),
                                    Cidade = StringService.Truncate(request?.Cidade, 50),
                                    Numero = StringService.Truncate(request.NumeroEndereco, 10),
                                    Logradouro = StringService.Truncate(request.Endereco, 100),
                                    Id_Cidade = IdCidadeConnect
                                };
                            }
                            else
                            {
                                client.Endereco_Principal.Cep = StringService.Truncate(request.Cep, 10);
                                client.Endereco_Principal.Bairro = StringService.Truncate(request.Bairro, 50);
                                client.Endereco_Principal.UF = StringService.Truncate(request?.UF, 2);
                                client.Endereco_Principal.Cidade = StringService.Truncate(request?.Cidade, 50);
                                client.Endereco_Principal.Numero = StringService.Truncate(request.NumeroEndereco, 10);
                                client.Endereco_Principal.Logradouro = StringService.Truncate(request.Endereco, 100);
                            }

                            client.Categoria_Cliente = categoriaConnect;
                            await repoClienteC.SaveOrUpdate(client);
                        }
                    }


                    if (client != null && string.IsNullOrEmpty(request.ContratoCliente) == false)
                    {
                        clienteContrato = await repoClienteContrato.selectFirst(p => p.Cliente.Id == client.Id && p.NumeroContrato == request.ContratoCliente);
                        if (clienteContrato == null)
                        {
                            var endereco = new Endereco_Cliente
                            {
                                Cep = StringService.Truncate(request.Cep, 10),
                                Bairro = StringService.Truncate(request.Bairro, 50),
                                UF = StringService.Truncate(request?.UF, 2),
                                Cidade = StringService.Truncate(request?.Cidade, 50),
                                Numero = StringService.Truncate(request.NumeroEndereco, 10),
                                Logradouro = StringService.Truncate(request.Endereco, 100),
                                Id_Cidade = IdCidadeConnect
                            };

                            await repoEndereco.Save(endereco);

                            clienteContrato = new ClienteContrato
                            {
                                Cliente = client,
                                NumeroContrato = request.ContratoCliente,
                                Descricao = request.ContratoCliente,
                                Endereco_Cliente = endereco
                            };

                            await repoClienteContrato.Save(clienteContrato);
                        }
                        else
                        {
                            if(String.IsNullOrEmpty(request.Latitude) && String.IsNullOrEmpty(request.Longitude))
                            {
                                request.Latitude = clienteContrato.Endereco_Cliente?.Latitude?.ToString(CultureInfo.InvariantCulture);
                                request.Longitude = clienteContrato.Endereco_Cliente?.Longitude?.ToString(CultureInfo.InvariantCulture);
                            }
                            
                        }
                    }

                    if (client != null && string.IsNullOrEmpty(request.ContratoClienteAnterior) == false)
                    {
                        clienteContratoAnterior = await repoClienteContrato.selectFirst(p => p.Cliente.Id == client.Id && p.NumeroContrato == request.ContratoClienteAnterior);
                        if (clienteContratoAnterior == null)
                        {
                            clienteContratoAnterior = new ClienteContrato
                            {
                                Cliente = client,
                                NumeroContrato = request.ContratoClienteAnterior,
                                Descricao = request.ContratoClienteAnterior
                            };

                            await repoClienteContrato.Save(clienteContratoAnterior);
                        }
                    }

                    if (projeto == null)
                    {
                        projeto = await repoProjeto.selectFirst(p => p.CodCt == request.CodCt && p.Codigo == request.Projeto);
                        if (projeto == null)
                            throw new Exception("O projeto buscado não foi encontrado");
                    }

                    if (contrato?.Unidade_Custo != null)
                    {
                        custo = await repoU.selectFirst(p => p.Unidade_Custo == contrato.Unidade_Custo);
                    }

                    if (serv == null)
                    {
                        serv = new Tipo_Servico_EquipeAniel
                        {
                            Cod_Tipo_Serv = Convert.ToInt16(repoTipoE.Max(p => p.Cod_Tipo_Serv).Result + 1),
                            Descricao = StringService.Truncate(request.TipoServico, 150),
                            CodCt = contrato.CodCt,

                            CodigoIntegrator = request.CodigoIntegratorTipoServico
                        };

                        await repoTipoE.Save(serv);
                    }

                    Sub_Tipo_ServicoAniel sub = serv != null ? await reposub.selectFirst(p => p.Codigo_Tipo == serv.Cod_Tipo_Serv && p.Descricao == request.SubTipoServico) : null;

                    if (PlanoProduto == null && request.Plano?.Id != null)
                    {
                        throw new Exception("ClientePlanoProduto buscado não foi encontrado");
                    }


                    doc.DescricaoPlanoAssinante = request.Plano?.Descricao;

                    if (String.IsNullOrEmpty(request.Plano?.Descricao) == false && PlanoProduto == null)
                    {
                        PlanoProduto = await GetPlano(request.Plano,
                                                      client,
                                                      clienteContrato,
                                                      request.Plano?.Login ?? request.LoginRadius,
                                                      request.Plano?.Senha ?? request.SenhaRadius,
                                                      session);

                        if (PlanoProduto != null)
                        {
                            await repoClientePlano.SaveOrUpdate(PlanoProduto);
                        }

                    }

                    if (request.PlanosAdicionais != null)
                    {
                        foreach (var item in request.PlanosAdicionais)
                        {
                            if (String.IsNullOrEmpty(item?.Descricao) == false)
                            {
                                var PlanoProdutoAdicional = await GetPlano(item,
                                                                           client,
                                                                           clienteContrato,
                                                                           request.Plano?.Login,
                                                                           request.Plano?.Senha,
                                                                           session);
                                if (PlanoProdutoAdicional != null)
                                {
                                    await repoClientePlano.SaveOrUpdate(PlanoProdutoAdicional);
                                }
                            }
                        }
                    }

                    if (loginRadius != null && !String.IsNullOrEmpty(request.LoginRadius))
                    {
                        if (loginRadius.Cliente_PlanoProduto?.Id != PlanoProduto.Id)
                        {
                            loginRadius.Cliente_PlanoProduto = PlanoProduto;

                        }

                        if (loginRadius.CodigoAuxiliar != request.CodigoAuxiliarLogin)
                        {
                            loginRadius.CodigoAuxiliar = request.CodigoAuxiliarLogin;
                        }

                        await repoLoginRadius.SaveOrUpdate(loginRadius);
                    }
                    else if (loginRadius == null && String.IsNullOrEmpty(request.LoginRadius) == false)
                    {
                        loginRadius = new Model.nhibernate.LoginRadius
                        {
                            Login = request.LoginRadius,
                            SenhaPPPOE = request.SenhaRadius,
                            Cliente = client,
                            CodigoAuxiliar = request.CodigoAuxiliarLogin ?? request.Plano?.IdLogin,
                            Cliente_PlanoProduto = PlanoProduto,
                            Status = Status_Default.Ativo,
                        };

                        await repoLoginRadius.SaveOrUpdate(loginRadius);
                    }


                    if (custo == null)
                    {
                        custo = await repoU.selectFirst(p => p.Unidade_Custo == request.Uni);
                        if (custo == null)
                            throw new Exception("Unidade de Custo buscada não foi encontrada");
                    }
                    if (sub == null)
                    {
                        if (String.IsNullOrEmpty(request.SubTipoServico) == false && serv != null)
                        {
                            sub = new Sub_Tipo_ServicoAniel { Codigo_Sub_Tipo = Convert.ToString(reposub.MaxString("COD_SUB_TIPO_SERV", "TB_SUB_TIPO_SERVICO", "").Result + 1), Descricao = StringService.Truncate(request.SubTipoServico, 70), Codigo_Tipo = serv.Cod_Tipo_Serv.Value };
                            await reposub.Save(sub);
                        }
                    }
                    if (tipoImovel == null)
                    {
                        if (String.IsNullOrEmpty(request.TipoImovel) == false)
                        {

                            tipoImovel = new Tipo_ImovelAniel { Cod_Tipo_Imovel = Convert.ToInt32(repoImovel.Max(p => p.Cod_Tipo_Imovel).Result + 1), Descricao = request.TipoImovel };
                            await repoImovel.Save(tipoImovel);
                        }
                    }

                    #endregion

                    #region data 
                    if (request.DataHoraAgendamento.HasValue)
                    {
                        doc.Hora_Maxima = request.DataHoraAgendamento.Value;
                        doc.Data_Maxima = request.DataHoraAgendamento.Value.Date;

                    }
                    if (request.DataHoraAgendamento.HasValue)
                    {
                        doc.Hora_Maxima = request.DataHoraAgendamento.Value;
                        doc.Data_Maxima = request.DataHoraAgendamento.Value.Date;

                    }
                    if (String.IsNullOrWhiteSpace(request.DataCadastro))
                    {
                        doc.Dia = DateTime.Now;
                        doc.Hora = DateTime.Now;

                    }
                    else
                    {
                        doc.Dia = Convert.ToDateTime(request.DataCadastro);

                        doc.Hora = Convert.ToDateTime(request.DataCadastro);
                    }
                    if (!String.IsNullOrWhiteSpace(request.DataEncerramento))
                    {
                        doc.Data_Encerramento = Convert.ToDateTime(request.DataEncerramento);

                    }

                    if (!String.IsNullOrWhiteSpace(request.Cod_Encerramento_TOA))
                    {
                        doc.Cod_Encerramento_TOA = request.Cod_Encerramento_TOA;

                    }

                    doc.DataAbertura = request.DataAbertura.ToString("dd/MM/yyyy");
                    doc.HoraAbertura = request.DataAbertura.ToString("HH:mm");

                    doc.Usuario = StringService.Truncate(request.UsuarioIntegracao, 10);
                    #endregion

                    //var novoseq = OsOriginais.Count() > 0 ? OsOriginais.Select(p => p.Seq).Max() + 1 : 1;

                    #region request
                    doc.Origem_Documento = request.Origem ?? Origem_Documento_Producao.Connect;
                    doc.CodCt = contrato.CodCt;
                    doc.Projeto = projeto.Codigo;
                    doc.Integracao = "S";

                    doc.Cod_Tipo_Imovel = tipoImovel?.Cod_Tipo_Imovel;

                    if (serv?.Natureza?.IdDepartamentoPadrao != null)
                    {
                        doc.CodDepto = serv?.Natureza?.IdDepartamentoPadrao;
                    }

                    #endregion

                    #region cliente
                    doc.Cliente = clientA;
                    doc.Nome_Cliente = StringService.Truncate(client?.NomeFantasia, 25);
                    doc.CPF_CLIENTE = StringService.Truncate(client?.CpfCnpj, 20);
                    doc.Titular = StringService.Truncate(client?.NomeFantasia, 100);
                    doc.RG_Assinante = StringService.Truncate(client?.Rg, 20);
                    doc.CelularCliente = StringService.Truncate(client?.Telefone_Celular, 15);
                    doc.TelefonePrincipal = StringService.Truncate(client?.Telefone_Fixo, 15);
                    doc.Tarefa = String.IsNullOrEmpty(StringService.Truncate(client?.Telefone_Fixo, 15)) ? "1" : StringService.Truncate(client?.Telefone_Fixo, 15);
                    #endregion

                    #region plano_tiposerv_atendimento
                    doc.Equipe = equipe?.Matricula;
                    doc.Tipo_Producao = "AC";

                    if (String.IsNullOrEmpty(request.Latitude) == false && String.IsNullOrEmpty(request.Longitude) == false)
                    {
                        double latitudeDouble;
                        double longitudeDouble;
                        if (Double.TryParse(request.Latitude, out latitudeDouble) && Double.TryParse(request.Longitude, out longitudeDouble))
                        {
                            if (latitudeDouble != 0 && longitudeDouble != 0)
                            {
                                doc.Latitude_Str = request.Latitude;
                                doc.Longitude_Str = request.Longitude;
                            }
                        }
                    }

                    doc.CPF_CLIENTE = StringService.Truncate(request.Cpf, 20);
                    doc.Cidade = StringService.Truncate(request?.Cidade, 100);
                    doc.Bairro = StringService.Truncate(request?.Bairro, 30);
                    doc.UF = StringService.Truncate(request?.UF, 2);
                    doc.Complemento = StringService.Truncate(request.Complemento, 100);
                    doc.Logradouro = StringService.Truncate(request.Endereco, 100);
                    doc.Numero = StringService.Truncate(request.NumeroEndereco, 10);
                    doc.Cep = StringService.Truncate(request.Cep, 100);
                    doc.Cod_Localidade = localidade?.Cod_Localidade;
                    doc.Cod_Area = area == null ? null : StringService.Truncate(area?.Cod_Area, 4);
                    doc.Cod_Escritorio = escritorio == null ? null : StringService.Truncate(escritorio?.Cod_Escritorio, 4);
                    doc.OXBDPEE = StringService.Truncate(request.ContratoCliente, 50);
                    doc.Observacao = StringService.Truncate(request.PontoReferencia, 150);
                    doc.Observacao_Aux = !String.IsNullOrEmpty(request.Observacao) ? StringService.Truncate(StringService.StripHTML(request.Observacao), 900) : null;
                    doc.Observacao_Teste = request.Facilidade;
                    doc.LoginRadius = StringService.Truncate(request.LoginRadius, 100);
                    doc.SenhaRadius = StringService.Truncate(request.SenhaRadius, 100);
                    doc.IdLogin = loginRadius?.Id;
                    doc.Id_Cliente_ContratoConnect = clienteContrato?.Id;
                    doc.Email_Assinante = StringService.Truncate(request.EmailCliente, 50);
                    doc.Id_Pop = request.IdPop.HasValue ? (await repositoryPop.selectFirst(p => p.Id == request.IdPop.Value))?.Id : (long?)null;
                    doc.Id_Elemento_Rede = request.IdElementoRede.HasValue ? (await repositoryElementoRede.selectFirst(p => p.Id == request.IdElementoRede.Value))?.Id : (long?)null;
                    doc.CaixaFTTH = request?.CaixaFTTH;
                    doc.PortaFTTH = Convert.ToString(request?.PortaFTTH);
                    doc.Id_Olt = request.IdOlt.HasValue ? (await repositoryPop.selectFirst(p => p.Id == request.IdOlt.Value))?.Id : (long?)null;
                    doc.Tipo_Prioridade = request?.Prioridade != null ? request.Prioridade : serv?.Natureza?.TipoPrioridade;
                    doc.Segmento = !String.IsNullOrEmpty(request.Segmento) ? (await repoSegmento.selectFirst(p => p.Descricao == request.Segmento))?.Codigo : null;
                    doc.CodDepto = !String.IsNullOrEmpty(request.Departamento) ? (await repoDepartamento.selectFirst(p => p.Descricao == request.Departamento))?.Id : (long?)null;
                    doc.Tipo = request.Tipo;
                    doc.Tecnologia = request.Tecnologia;

                    doc.Adicional_1 = request.Adicional1;
                    doc.Adicional_2 = request.Adicional2;
                    doc.Adicional_3 = request.Adicional3;
                    doc.Adicional_4 = request.Adicional4;
                    doc.Adicional_5 = request.Adicional5;
                    doc.Adicional_6 = request.Adicional6;
                    doc.Adicional_7 = request.Adicional7;
                    doc.Adicional_8 = request.Adicional8;
                    doc.Adicional_9 = request.Adicional9;
                    doc.Adicional_10 = request.Adicional10;

                    if (PlanoProduto != null)
                    {

                        if (doc.Cod_Localidade.HasValue == false &&
                            string.IsNullOrEmpty(doc.Cod_Area) &&
                            string.IsNullOrEmpty(doc.Cod_Escritorio))
                        {
                            doc.Cod_Localidade = PlanoProduto.Cod_Localidade;
                            doc.Cod_Area = StringService.Truncate(PlanoProduto.Cod_Area, 4);
                            doc.Cod_Escritorio = StringService.Truncate(PlanoProduto.Cod_Escritorio, 4);
                        }


                        if (String.IsNullOrEmpty(doc.LoginRadius) && String.IsNullOrEmpty(doc.SenhaRadius))
                        {
                            doc.LoginRadius = StringService.Truncate(PlanoProduto.Login_Radius, 100);
                            doc.SenhaRadius = StringService.Truncate(PlanoProduto.Senha_Radius, 100);
                        }

                        doc.Id_Cliente_PlanoProdutoConnect = PlanoProduto.Id;
                        doc.Id_PlanoProduto = PlanoProduto?.Plano?.Id ?? PlanoProduto?.Produto?.Id;
                    }

                    if (request.PlanoAnterior != null)
                    {
                        doc.CategoriaPlanoAnterior = request.PlanoAnterior.CodigoTipoPlanoIntegracao;
                        doc.DescPlanoAnterior = request.PlanoAnterior.Descricao;
                    }

                    doc.Responsabilidade = ResponsabilidadeOS.LIVRE;
                    doc.Cod_Tipo_Serv = serv.Cod_Tipo_Serv;
                    doc.Cod_Sub_Tipo_Serv = sub?.Codigo_Sub_Tipo;
                    doc.Unidade_Custo = StringService.Truncate(custo.Unidade_Custo, 10);

                    if (doc.CodDepto == null)
                    {

                        if (serv?.Natureza != null)
                        {
                            doc.CodDepto = serv.Natureza?.IdDepartamentoPadrao ?? 0;
                        }
                    }


                    if (doc.Equipe == null)
                    {
                        if (doc.Data_Maxima == null)
                            doc.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                        else
                            doc.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                    }
                    else
                    {
                        doc.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
                    }

                    #endregion

                    #endregion

                    if (doc.Data_Maxima.HasValue && doc.Hora.HasValue)
                    {
                        var dataHoraInicio = (doc?.Data_Maxima.Value.Add(doc.Hora_Maxima.Value.TimeOfDay)).Value;
                        var tempoExecucao = _repoDocProd.GetTempoExecucao(config, (doc?.Data_Maxima.Value.Add(doc.Hora_Maxima.Value.TimeOfDay)).Value, doc.CodCt, doc.Projeto, doc.Num_Obra, doc.Num_Doc);
                        doc.CodTurnoServ = turnoRepository.BuscarTurno(dataHoraInicio, tempoExecucao)?.Id;
                        Agendamento_EquipeAniel agendamento = new Agendamento_EquipeAniel
                        {
                            CodCt = doc.CodCt,
                            Projeto = doc.Projeto,
                            Num_Obra = doc.Num_Obra,
                            Num_Doc = doc.Num_Doc,
                            Cod_Escritorio = doc.Cod_Escritorio,
                            Cod_Area = doc.Cod_Area,
                            Cod_Localidade = doc.Cod_Localidade,
                            DataHora_Inicio = dataHoraInicio,
                            DataHora_Fim = tempoExecucao.HasValue ? dataHoraInicio.Add(tempoExecucao.Value) : dataHoraInicio,
                            Flexivel = null,
                            Deslocamento = 0,
                            //Log = GerarLog(value.Objeto.Lista_Forca_Trabalho, value.Objeto.Lista_Ja_Agendados, config),
                            Tipo_Servico = doc.Cod_Tipo_Serv.HasValue ? await repoTipoE.selectFirst(p => p.Cod_Tipo_Serv == doc.Cod_Tipo_Serv) : null,
                            Tipo_Imovel = doc.Cod_Tipo_Imovel.HasValue ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == doc.Cod_Tipo_Imovel) : null,
                            UsuarioCriacao = "INTEGRACAO",
                            Origem = OrigemAgendamentoOS.Integracao,
                            Latitude = doc.GetLatitude_Str(),
                            Longitude = doc.GetLongitude_Str(),

                        };

                        await repositoryAgendamento.Save(agendamento);
                    }
                    if (!String.IsNullOrWhiteSpace(request.DataVencimento))
                    {
                        doc.Data_Indicador = Convert.ToDateTime(request.DataVencimento);

                    }

                    doc.InformacoesComplementares = request.InformacoesComplementares;
                    await repoDoc.Save(doc);

                    business.InsertLog_Lanc_Servico(doc.CodMotivoReagendamento, doc.ObsMotivoReagendamento, doc.CodCt, doc.Projeto, doc.Num_Obra, doc.Num_Doc, "Criação do Serviço", null,
                    StringService.Truncate(request.UsuarioIntegracao,10), null, null, null, null);
                    response.result = true;
                    response.message = $"Registro inserido com sucesso - OS {doc.Numero_Obra_Original}";
                }
            }

            return response;

        }


        private async Task<Cliente_PlanoProduto> GetPlano(PlanoRequest planoRequest, Cliente client, ClienteContrato clienteContrato, String login, string senha, ISession session)
        {

            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoPlanoProduto = new Conexao<PlanoProdutoAniel>(sessionT5))

            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))

            using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            {
                Cliente_PlanoProduto PlanoProduto = null;
                var plano = await repoPlano.selectFirst(p => p.Descricao == planoRequest.Descricao);
                var produto = await repoProduto.selectFirst(p => p.Descricao == planoRequest.Descricao);
                var planoProdutoAniel = await repoPlanoProduto.selectFirst(p => p.Descricao == planoRequest.Descricao);

                if (!String.IsNullOrEmpty(login))
                {
                    PlanoProduto = await repoClientePlano.selectFirst(p => p.Cliente.Id == client.Id &&
                                                                       (p.Plano == plano && p.Plano != null || p.Produto == produto && p.Produto != null) &&
                                                                       (p.Login_Radius == login || p.ClienteContrato.Id == clienteContrato.Id));
                }
                else
                {
                    PlanoProduto = await repoClientePlano.selectFirst(p => p.Cliente.Id == client.Id &&
                                                                       (p.Plano == plano && p.Plano != null || p.Produto == produto && p.Produto != null) &&
                                                                       p.ClienteContrato.Id == clienteContrato.Id);
                }

                if (plano != null || produto != null)
                {
                    if (planoProdutoAniel == null)
                    {
                        PlanoProdutoAniel planoAniel = new PlanoProdutoAniel();
                        planoAniel.Id = plano.Id;
                        planoAniel.Descricao = planoRequest.Descricao;
                        await repoPlanoProduto.Save(planoAniel);
                    }
                }
                else
                {
                    plano = new Plano
                    {
                        Descricao = planoRequest.Descricao,
                        CodigoAuxiliar = planoRequest.CodigoIntegracao,
                        Tipo_Tecnologia = Tipo_Tecnologia.Internet,
                        CodigoTipoPlanoIntegracao = planoRequest.CodigoTipoPlanoIntegracao,
                        SituacaoPlano = Situacao_plano.Ativo
                    };

                    await repoPlano.Save(plano);
                    if (planoProdutoAniel == null)
                    {
                        PlanoProdutoAniel planoAniel = new PlanoProdutoAniel();
                        planoAniel.Id = plano.Id;
                        planoAniel.Descricao = planoRequest.Descricao;
                        await repoPlanoProduto.Save(planoAniel);

                    }
                }

                if (PlanoProduto == null && client != null && (plano != null || produto != null))
                {
                    PlanoProduto = new Cliente_PlanoProduto
                    {
                        Cliente = client,
                        Plano = plano,
                        Produto = produto,
                        Login_Radius = login,
                        Senha_Radius = senha,
                        ClienteContrato = clienteContrato,
                        CodigoIntegracao = String.IsNullOrEmpty(planoRequest.IdClientePlano) ? null : planoRequest.IdClientePlano,
                        Status = Status_PlanoProduto.Habilitado
                    };
                }

                if (PlanoProduto != null)
                {
                    PlanoProduto.CodigoIntegracao = String.IsNullOrEmpty(planoRequest.IdClientePlano) ? null : planoRequest.IdClientePlano;
                }



                return PlanoProduto;
            }
        }

        /// <summary>
        /// Método para reabertura de OS 
        /// </summary>
        /// <param name="documento">Objeto da OS indicada para reabertura</param>
        /// <param name="Usuario">Usuário responsável pela alteração</param>
        /// <exception cref="Exception"></exception>
        public async void ReabrirOS(Documento_ProducaoAniel documento, string Usuario)
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                if (documento == null)
                {
                    throw new Exception("Ordem de Servico não foi encontrada");
                }
                if (documento.Status != StatusExecucaoAniel.Fechada_Improdutiva &&
                   documento.Status != StatusExecucaoAniel.Fechada_Produtiva &&
                   documento.Status != StatusExecucaoAniel.Atendimento_Finalizado)
                {
                    throw new Exception("Ordem de Servico não se encontrada fechada");
                }
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    var parametros = new Dictionary<string, object>();
                    sql.AppendLine("UPDATE TB_DOCUMENTO_PRODUCAO SET DATA_PROD = @DATA_PROD, HORA_PROD = @HORA_PROD, COD_ENCERRAMENTO = @COD_ENCERRAMENTO, ")
                       .AppendLine("MODULO_BAIXA = @MODULO_BAIXA, ROTERIZAR = @ROTERIZAR, RESPONSABILIDADE = @RESPONSABILIDADE, STATUS_EXECUCAO = @STATUS")
                       .AppendLine($"WHERE CODCT = '{documento.CodCt}' AND PROJETO = '{documento.Projeto}' AND NUM_OBRA = '{documento.Num_Obra}' AND NUM_DOC = {documento.Num_Doc};");

                    parametros.Add("@DATA_PROD", null);
                    parametros.Add("@HORA_PROD", null);
                    parametros.Add("@COD_ENCERRAMENTO", null);

                    parametros.Add("@MODULO_BAIXA", null);
                    parametros.Add("@ROTERIZAR", "N");
                    parametros.Add("@RESPONSABILIDADE", 0);

                    parametros.Add("@STATUS_AUDITORIA", null);

                    if (documento.Equipe == null)
                    {
                        if (documento.Data_Maxima == null)
                            parametros.Add("@STATUS", 4);
                        else
                            parametros.Add("@STATUS", 13);
                    }
                    else
                    {
                        parametros.Add("@STATUS", 0);
                    }
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);
                }

                this._repoDocProd.InsertLog_Lanc_Servico(null, String.Empty, documento.CodCt,
                                                                            documento.Projeto,
                                                                            documento.Num_Obra,
                                                                            documento.Num_Doc,
                                                                            "Reabriu OS",
                                                                            null,
                                                                            Usuario,
                                                                            documento.Data_Maxima,
                                                                            documento.Cod_Tipo_Serv,
                                                                            null, null, null);
            }
        }

        /// <summary>
        /// Métodos para inserção de imagens e anexos respectivos a uma Ordem de Serviço específica
        /// </summary>
        /// <param name="documento">Objeto de OS</param>
        /// <param name="arquivos">Listagens de Arquivos a serem adicionados à OS</param>
        /// <param name="usuario">usuário responsável pela inserção dos arquivos</param>
        /// <param name="transaction">transação dessa inserção no banco</param>
        public void InserirAnexos(Documento_ProducaoAniel documento, IEnumerable<Arquivo_DocProd> arquivos, IEnumerable<Imagem_DocPord_Mobile> fotos, string usuario, ITransaction transaction)
        {
            var extensions = arquivos.Select(p => p.Tipo).Concat(fotos.Select(p => p.Tipo));
            if (arquivos.Count() > 0)
            {
                this._repoFoto.SaveOrUpdateArquivosSql(arquivos);
            }
            if (fotos.Count() > 0)
            {
                this._repoFoto.SaveOrUpdateFotosSql(fotos);
            }

            this._repoDocProd.InsertLog_Lanc_Servico(null, String.Empty, documento.CodCt,
                                                                         documento.Projeto,
                                                                         documento.Num_Obra,
                                                                         documento.Num_Doc,

                                                                         StringService.TruncateTresPontinhos($"Upload de Arquivos/Fotos nas extensões: {String.Join(" || ", extensions)}", 200),
                                                                         null,
                                                                         usuario,
                                                                         documento.Data_Maxima,

                                                                         documento.Cod_Tipo_Serv,
                                                                         StringService.TruncateTresPontinhos(String.Join(" || ", arquivos.Select(p => p.Descricao).ToList()), 800),
                                                                         null,
                                                                         transaction);
        }


        public async Task<bool> CapturaPlanoSemOs(long? idClientePlanoProdutoConnect)
        {
            var retorno = false;
            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Venda>(session))
            using (var repoEt = new Conexao<EtapaVenda>(session))
            using (var repoClipp = new Conexao<Cliente_PlanoProduto>(session))
            {
                var plano = repoClipp.QueryableSelect().Where(p => p.Id == idClientePlanoProdutoConnect).FirstOrDefault();
                if (plano != null)
                {
                    var venda = plano.Id_Venda;
                    var planosLogica = repoClipp.QueryableSelect().Where(p => p.Id_Venda == venda && p.Status == Status_PlanoProduto.Aguardando_Criacao_OS).ToList();
                    if (plano != null && !planosLogica.Any(p => p.Id != idClientePlanoProdutoConnect))
                    {
                        var etapaOS = await repoEt.selectFirst(p => p.Venda.Id == plano.Id_Venda &&
                                                             p.Etapa == EnumEtapaVenda.OrdemServico);

                        if (etapaOS != null)
                        {
                            etapaOS.Status = StatusEtapaVenda.Aprovado;
                            await repoEt.Update(etapaOS);
                        }

                        retorno = true;

                    }
                }
            }
            return retorno;
        }

        public void ApiAlterarOS(Documento_ProducaoAniel servico, RequestAlteraOs request, ref Agendamento_EquipeAniel agendamento)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoEscritorio = new Conexao<EscritorioAniel>(sessionT5))
            using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
            using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
            using (var repoPeriodo = new Conexao<Turno_ServicoAniel>(sessionT5))
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5)) //TODO: confimrar
            using (var repoSubTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
            using (var repoSegmento = new Conexao<Segmento>(sessionT5)) //TODO: confimrar
            using (var repoSetor = new Conexao<FolhaDeptoAniel>(sessionT5))
            using (var repoTipoImovel = new Conexao<Tipo_ImovelAniel>(sessionT5))
            using (var repoArea = new Conexao<AreaAniel>(sessionT5))
            using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
            using (var repoconfig = new Conexao<Configuracoes_Gerais>(session))
            {
                if (request.GrupoArea != null)
                {
                    if (request.GrupoArea.IsEmpty())
                    {
                        request.GrupoArea = null;
                    }

                    EscritorioAniel escritorio = repoEscritorio.selectFirst(x => x.Descricao == StringService.Truncate(request.GrupoArea, 70)).Result;

                    if (escritorio == null && !String.IsNullOrEmpty(request.GrupoArea))
                        throw new Exception("O campo GrupoArea não encontrou valores correspondentes ao registro enviado.");

                    if (escritorio?.Cod_Escritorio != servico.Cod_Escritorio)
                    {
                        servico.Cod_Escritorio = escritorio?.Cod_Escritorio;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Grupo Area",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }
                if (request.Area != null)
                {
                    if (request.Area.IsEmpty())
                    {
                        request.Area = null;
                    }

                    AreaAniel area = repoArea.selectFirst(x => x.Descricao == StringService.Truncate(request.Area, 35) && x.Cod_Escritorio == servico.Cod_Escritorio).Result;

                    if (area == null && !String.IsNullOrEmpty(request.Area))
                        throw new Exception("O campo Area não encontrou valores correspondentes ao registro enviado.");

                    if (area?.Cod_Area != servico.Cod_Area)
                    {
                        servico.Cod_Area = area?.Cod_Area;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Area",
                            servico.Cod_Area,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }
                if (request.Localidade != null)
                {
                    if (request.Localidade.IsEmpty())
                    {
                        request.Localidade = null;
                    }
                    LocalidadeAniel localidade = repoLocalidade.selectFirst(x => x.LocalidadeStr == StringService.Truncate(request.Localidade, 50) &&
                                                                                 x.Cod_Escritorio == servico.Cod_Escritorio &&
                                                                                 x.Cod_Area == servico.Cod_Area).Result;

                    if (localidade == null && !String.IsNullOrEmpty(request.Localidade))
                        throw new Exception("O campo Localidade não encontrou valores correspondentes ao registro enviado.");

                    if (localidade?.Cod_Localidade != servico.Cod_Localidade)
                    {
                        servico.Cod_Localidade = Convert.ToInt32(localidade?.Cod_Localidade);
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                             servico.Num_Doc,
                            "Alteração de Localidade",
                            Convert.ToString(servico.Cod_Localidade),
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            Convert.ToString(request.Localidade),
                            null);
                    }
                }


                if (request.Tecnico != null && servico.Equipe != request.Tecnico)
                {
                    if (request.Tecnico.IsEmpty())
                    {
                        request.Tecnico = null;
                    }
                    EquipeAniel equipe = repoEquipe.selectFirst(x => x.Matricula == StringService.Truncate(request.Tecnico, 50)).Result;

                    if (equipe == null && !String.IsNullOrEmpty(request.Tecnico))
                        throw new Exception("O campo Tecnico não encontrou valores correspondentes ao registro enviado.");

                    servico.Equipe = equipe?.Matricula;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Técnico",
                        servico.Cod_Escritorio,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }
                if (request.Responsabilidade != null && servico.Responsabilidade != request.Responsabilidade)
                {
                    if (!Enum.IsDefined(typeof(ResponsabilidadeOS), request.Responsabilidade))
                        throw new Exception("O campo Responsabilidade não possui um valor válido.");

                    servico.Responsabilidade = request.Responsabilidade.Value;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Responsabilidade",
                        servico.Cod_Escritorio,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                #region Agendamento
                if (request.Data_HoraAgendamento != null)
                {                            
                    if (request.Data_HoraAgendamento.IsEmpty())
                    {
                        // Veio vazio: limpa a data no banco
                        servico.DiaHora_Agendamento = null;
                        servico.Data_Maxima = null;
                        servico.Hora_Maxima = null;
                        servico.Agendamento = null;
                    }
                    else
                    {
                        DateTime dataAgendamento;
                        bool parseOk = DateTime.TryParseExact(
                            request.Data_HoraAgendamento,
                            "dd/MM/yyyy HH:mm:ss",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out dataAgendamento);

                        if (!parseOk)
                        {
                            throw new Exception(
                                "O campo Data_HoraAgendamento estava no formato incorreto. " +
                                "Para validar o valor, seria necessário que estivesse no formato: dd/MM/yyyy HH:mm:ss");
                        }

                        if (servico.DiaHora_Agendamento != dataAgendamento)
                        {
                            servico.DiaHora_Agendamento = dataAgendamento;
                            servico.Data_Maxima = dataAgendamento;
                            servico.Hora_Maxima = dataAgendamento;
                            var config = repoconfig.selectFirst().Result;

                            var tempoExecucao = _repoDocProd.GetTempoExecucao(config, (servico?.Data_Maxima.Value.Add(servico.Hora_Maxima.Value.TimeOfDay)).Value, servico.CodCt, servico.Projeto, servico.Num_Obra, servico.Num_Doc);


                            agendamento = new Agendamento_EquipeAniel()
                            {
                                CodCt = servico.CodCt,
                                Projeto = servico.Projeto,
                                Num_Obra = servico.Num_Obra,
                                Num_Doc = servico.Num_Doc,
                                Cod_Escritorio = servico.Cod_Escritorio,
                                Cod_Area = servico.Cod_Area,
                                Cod_Localidade = servico.Cod_Localidade,
                                DataHora_Inicio = dataAgendamento,
                                DataHora_Fim = tempoExecucao.HasValue ? dataAgendamento.Add(tempoExecucao.Value) : dataAgendamento,
                                Flexivel = null,
                                Deslocamento = 0,
                                Tipo_Servico = servico.Cod_Tipo_Serv.HasValue ? repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == servico.Cod_Tipo_Serv).Result : null,
                                Tipo_Imovel = servico.Cod_Tipo_Imovel.HasValue ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == servico.Cod_Tipo_Imovel).Result : null,
                                UsuarioCriacao = request.Usuario,
                                Origem = OrigemAgendamentoOS.Manual,
                                Latitude = servico.GetLatitude_Str(),
                                Longitude = servico.GetLongitude_Str()
                            };
                        

                            this.InsertLog_Lanc_Servico(
                                servico.CodMotivoReagendamento,
                                servico.ObsMotivoReagendamento,
                                servico.CodCt,
                                servico.Projeto,
                                servico.Num_Obra,
                                servico.Num_Doc,
                                "Alteração da Data de Agendamento",
                                servico.Cod_Escritorio,
                                request.Usuario,
                                servico.Data_Maxima,
                                servico.Cod_Tipo_Serv,
                                request.GrupoArea,
                                null);
                        }
                    }
                }

                #endregion

                if (request.Periodo != null)
                {
                    if (request.Periodo.IsEmpty())
                    {
                        request.Periodo = null;
                    }
                    Turno_ServicoAniel turno = repoPeriodo.selectFirst(x => x.Descricao == StringService.Truncate(request.Periodo, 50)).Result;

                    if (turno == null && !String.IsNullOrEmpty(request.Periodo))
                        throw new Exception("O campo Periodo não encontrou valores correspondentes ao registro enviado.");

                    if (turno?.Id != servico.CodTurnoServ)
                    {
                        servico.CodTurnoServ = turno?.Id;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Turno",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }

                if (request.TipoServico != null)
                {
                    if (request.TipoServico.IsEmpty())
                    {
                        request.TipoServico = null;
                    }

                    Tipo_Servico_EquipeAniel tipo_Servico = repoTipoServico.selectFirst(x => x.Descricao == StringService.Truncate(request.TipoServico, 50)).Result;
                    if (tipo_Servico == null)
                        throw new Exception("O campo TipoServico não encontrou valores correspondentes ao registro enviado.");

                    if (tipo_Servico?.Cod_Tipo_Serv != servico.Cod_Tipo_Serv)
                    {

                        servico.Cod_Tipo_Serv = tipo_Servico?.Cod_Tipo_Serv;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Tipo de Serviço",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }
                if (request.SubtipoServico != null)
                {

                    if (request.SubtipoServico.IsEmpty())
                    {
                        request.SubtipoServico = null;
                    }
                    Sub_Tipo_ServicoAniel subServico = repoSubTipoServico.selectFirst(x => x.Descricao == StringService.Truncate(request.SubtipoServico, 50)).Result;

                    if (subServico == null && !String.IsNullOrEmpty(request.SubtipoServico))
                        throw new Exception("O campo SubtipoServico não encontrou valores correspondentes ao registro enviado.");

                    if (subServico?.Codigo_Sub_Tipo != servico.Cod_Sub_Tipo_Serv)
                    {
                        servico.Cod_Sub_Tipo_Serv = subServico?.Codigo_Sub_Tipo;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de SubTipo de Serviço",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }

                if (request.Segmento != null && servico.Segmento != request.Segmento)
                {

                    if (request.Segmento.IsEmpty())
                    {
                        request.Segmento = null;
                    }

                    Segmento segmento = repoSegmento.selectFirst(x => x.Descricao == StringService.Truncate(request.Segmento, 50)).Result;
                    if (segmento == null && !String.IsNullOrEmpty(request.Segmento))
                        throw new Exception("O campo Segmento não encontrou valores correspondentes ao registro enviado.");

                    servico.Segmento = segmento?.Codigo;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Segmento",
                        servico.Cod_Escritorio,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.Setor != null)
                {

                    if (request.Setor.IsEmpty())
                    {
                        request.Setor = null;
                    }

                    var setor = repoSetor.selectFirst(x => x.Descricao == request.Setor).Result;
                    if (setor == null && !String.IsNullOrEmpty(request.Setor))
                        throw new Exception("O campo Setor não encontrou valores correspondentes ao registro enviado.");

                    if (setor?.CodDepto != servico.CodDepto)
                    {

                        servico.CodDepto = Convert.ToInt16(setor?.CodDepto);
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Departamento",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }

                if (request.Prioridade != null && servico.Tipo_Prioridade.ToString() != request.Prioridade)
                {

                    if (request.Prioridade.IsEmpty())
                    {
                        request.Prioridade = null;
                    }
                    if (Enum.TryParse<Tipo_PrioridadeT5>(request.Prioridade, true, out var prioridadeEnum))
                    {
                        servico.Tipo_Prioridade = prioridadeEnum;
                    }

                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Prioridade",
                        servico.Cod_Escritorio,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.Numero != null && servico.Numero != request.Numero)
                {

                    if (request.Numero.IsEmpty())
                    {
                        request.Numero = null;
                    }
                    servico.Numero = request.Numero;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Número do Endereço informado",
                        servico.Cod_Area,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.Cep != null && servico.Cep != request.Cep)
                {

                    if (request.Cep.IsEmpty())
                    {
                        request.Cep = null;
                    }
                    servico.Cep = request.Cep;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Cep do Endereço informado",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }

                if (request.Complemento != null && servico.Complemento != request.Complemento)
                {


                    if (request.Complemento.IsEmpty())
                    {
                        request.Complemento = null;
                    }
                    servico.Complemento = request.Complemento;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Complemento do Endereço informado",
                        servico.Cod_Area,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.Logradouro != null && servico.Logradouro != request.Logradouro)
                {

                    if (request.Logradouro.IsEmpty())
                    {
                        request.Logradouro = null;
                    }
                    servico.Logradouro = request.Logradouro;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de  Endereço ",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }
                if (request.Bairro != null && servico.Bairro != request.Bairro)
                {

                    if (request.Bairro.IsEmpty())
                    {
                        request.Bairro = null;
                    }
                    servico.Bairro = request.Bairro;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Bairro do Endereço informado",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }

                if (request.Cidade != null && servico.Cidade != request.Cidade)
                {
                    if (request.Cidade.IsEmpty())
                    {
                        request.Cidade = null;
                    }

                    servico.Cidade = request.Cidade;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Cidade",
                        servico.Cod_Area,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.UF != null && servico.UF != request.UF)
                {

                    if (request.UF.IsEmpty())
                    {
                        request.UF = null;
                    }
                    servico.UF = request.UF;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Localidade",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }

                if (request.TipoImovel != null)
                {

                    if (request.TipoImovel.IsEmpty())
                    {
                        request.TipoImovel = null;
                    }

                    var tipo = repoTipoImovel.selectFirst(x => x.Descricao == request.TipoImovel).Result;
                    if (tipo == null)
                        throw new Exception("O campo TipoImovel não encontrou valores correspondentes ao registro enviado.");

                    if (tipo?.Cod_Tipo_Imovel != servico.Cod_Tipo_Imovel)
                    {
                        servico.Cod_Tipo_Imovel = tipo?.Cod_Tipo_Imovel;
                        this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, servico.ObsMotivoReagendamento, servico.CodCt,
                            servico.Projeto,
                            servico.Num_Obra,
                            servico.Num_Doc,
                            "Alteração de Tipo de Imovel",
                            servico.Cod_Escritorio,
                            request.Usuario,
                            servico.Data_Maxima,
                            servico.Cod_Tipo_Serv,
                            request.GrupoArea,
                            null);
                    }
                }

                if (request.Latitude != null)
                {
                    if (request.Latitude.IsEmpty())
                    {
                        request.Latitude = null;
                    }

                    request.Latitude = request.Latitude.Replace(",", ".");
                    servico.Latitude = Double.TryParse(request.Latitude, out var Latitude) ? Latitude : throw new Exception("O campo Latitude estava no formato incorreto. Para validar o valor, seria necessário que estivesse no formato: XX,XXXXX");
                    servico.Latitude_Str = request.Latitude;

                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Latitude",
                        servico.Cod_Area,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.Longitude != null)
                {
                    if (request.Longitude.IsEmpty())
                    {
                        request.Longitude = null;
                    }
                    request.Longitude = request.Longitude.Replace(",", ".");

                    servico.Longitude = Double.TryParse(request.Longitude, out var Longitude) ? Longitude : throw new Exception("O campo Longitude estava no formato incorreto. Para validar o valor, seria necessário que estivesse no formato: XX,XXXXX");
                    servico.Longitude_Str = request.Longitude;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Longitude",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }

                if (request.DetalhesOS != null && servico.Observacao_Aux != request.DetalhesOS)
                {
                    if (request.DetalhesOS.IsEmpty())
                    {
                        request.DetalhesOS = null;
                    }

                    servico.Observacao_Aux = request.DetalhesOS;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Detalhes",
                        servico.Cod_Area,
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        request.GrupoArea,
                        null);
                }

                if (request.InformacoesComplementares != null && servico.InformacoesComplementares != request.InformacoesComplementares)
                {

                    string labelInformacoesComplementares = $"[Inserção via API: {request.Usuario} - {DateTime.Now}]: ";
                    if (request.InformacoesComplementares.IsEmpty())
                    {
                        request.InformacoesComplementares = null;
                    }
                    servico.InformacoesComplementares = string.IsNullOrEmpty(servico.InformacoesComplementares) ? labelInformacoesComplementares + request.InformacoesComplementares :
                                                                                                                  servico.InformacoesComplementares + "\r\n" + labelInformacoesComplementares + request.InformacoesComplementares;
                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de Informações Complementares",
                        Convert.ToString(servico.Cod_Localidade),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.Localidade),
                        null);
                }

                if (request.ObservacaoTeste != null && servico.Observacao_Teste != request.ObservacaoTeste)
                {

                    string labelObservacaoTeste = $"[Inserção via API: {request.Usuario} - {DateTime.Now}]: ";
                    if (request.ObservacaoTeste.IsEmpty())
                    {
                        request.ObservacaoTeste = null;
                    }

                    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                        servico.Projeto,
                        servico.Num_Obra,
                        servico.Num_Doc,
                        "Alteração de ObservacaoTeste",
                        Convert.ToString(servico.Observacao_Teste),
                        request.Usuario,
                        servico.Data_Maxima,
                        servico.Cod_Tipo_Serv,
                        Convert.ToString(request.ObservacaoTeste),
                        null);

                    servico.Observacao_Teste = string.IsNullOrEmpty(servico.Observacao_Teste) ? labelObservacaoTeste + request.ObservacaoTeste :
                                                                                                                  servico.Observacao_Teste + "\r\n" + labelObservacaoTeste + request.ObservacaoTeste;

                }

                //if (!String.IsNullOrEmpty(request.CamposAdicionais) && servico.Adicional_1 != request.CamposAdicionais)
                //{
                //    servico.Adicional_1 = request.Localidade;
                //    this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                //        servico.Projeto,
                //        servico.Num_Obra,
                //        servico.Num_Doc,
                //        "Alteração de Localidade",
                //        Convert.ToString(servico.Cod_Localidade),
                //        request.Usuario,
                //        servico.Data_Maxima,
                //        servico.Cod_Tipo_Serv,
                //        Convert.ToString(request.Localidade),
                //        null);
                //}
                if (request.CamposAdicionais != null)
                {
                    var camposRequest = request.CamposAdicionais.GetType().GetProperties();
                    var camposServico = servico.GetType().GetProperties();

                    foreach (var campo in camposRequest)
                    {
                        var valorRequest = campo.GetValue(request.CamposAdicionais)?.ToString();
                        var campoServico = camposServico.FirstOrDefault(p => p.Name == campo.Name);

                        if (campoServico != null)
                        {
                            var valorServico = campoServico.GetValue(servico)?.ToString();

                            if (!string.IsNullOrEmpty(valorRequest) && valorRequest != valorServico)
                            {
                                campoServico.SetValue(servico, valorRequest);

                                this.InsertLog_Lanc_Servico(servico.CodMotivoReagendamento, null, servico.CodCt,
                                    servico.Projeto, servico.Num_Obra, servico.Num_Doc,
                                    $"Alteração no {campo.Name}", servico.Cod_Area, request.Usuario,
                                    servico.Data_Maxima, servico.Cod_Tipo_Serv, request.GrupoArea, null);
                            }
                        }
                    }
                }

                if (servico.Data_Maxima != null && !string.IsNullOrEmpty(servico.Equipe))
                {
                    servico.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
                }
                else if (servico.Data_Maxima != null && string.IsNullOrEmpty(servico.Equipe))
                {
                    servico.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                }
                else if (servico.Data_Maxima != null)
                {
                    servico.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                }
                else if (servico.Data_Maxima == null)
                {
                    servico.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                }

            }
        }

        public async Task InterromperDeslocamento(string codCt, string projeto, int numDoc, string numObraOriginal, string usuario)
        {

            Documento_ProducaoAniel documento = null;
            if (!String.IsNullOrEmpty(codCt) && !String.IsNullOrEmpty(projeto) && numDoc != 0 && !String.IsNullOrEmpty(numObraOriginal))
            {
                documento = this._repoDocProd.GetAll()
                .Where(p => p.CodCt == codCt && p.Num_Doc == numDoc && p.Num_Obra == numObraOriginal && p.Projeto == projeto)
                .OrderByDescending(p => p.Seq).FirstOrDefault();
            }
            else
            {
                documento = this._repoDocProd.GetAll().Where(p => p.Numero_Obra_Original == numObraOriginal)
                                                      .OrderByDescending(p => p.Seq)
                                                      .FirstOrDefault();
            }

            if (documento == null)
            {
                throw new Exception("Ordem de Serviço não foi encontrada");
            }

            if (documento.Status != StatusExecucaoAniel.OS_Em_deslocamento)
            {
                throw new Exception(String.Format(RsBase.ORDEM_SERVICO_DESLOCAMENTO3, documento.Numero_Obra_Original));
            }

            documento.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
            await this._repoDocProd.SaveUpdateDocumentoAsync(documento);

            var ordemLocalizada = this._repoDocProd.GetOSMongo(documento);
            var status = new List<StatusDocumentoProducao>();

            SenhaAniel usuarioAniel = await this._repoDocProd.GetSenhaAniel(usuario);
            FuncionarioAniel funcionario = usuarioAniel != null
                ? await this._repoDocProd.GetFuncionario(usuarioAniel.NumFre)
                : null;

            status.Add(new StatusDocumentoProducao()
            {
                statusExecucao = (int)StatusExecucaoAniel.Interromper_Deslocamento,
                equipe = usuario,
                nome = funcionario?.Nome,
                numFre = funcionario?.Num_Fre,
                data = DateTime.Now,
                dataHoraServidor = DateTime.Now,
                hora = DateTime.Now,
                latitude = 0,
                longitude = 0,
            });

            if (ordemLocalizada != null)
            {
                ordemLocalizada.status.AddRange(status);
                await this._repoDocProd.UpdateMongo(ordemLocalizada);
            }
            else
            {
                var documentoMongo = new DocumentoProducaoMongo()
                {
                    codCt = documento.CodCt,
                    projeto = documento.Projeto,
                    contrato = documento.Num_Doc,
                    numObra = documento.Num_Obra,
                    dataAgendamento = documento.Data_Maxima.HasValue ? documento.Data_Maxima.Value : new DateTime(),
                    status = status
                };
                await this._repoDocProd.SaveMongo(documentoMongo);
            }

            if (documento.EquipeAniel != null)
            {
                MobileService.NotificarSmartphone(documento.EquipeAniel.Device_Id, String.Empty, "Houve mudanças em sua rotina.",
                    "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS",
                    "", "", "", "");
            }
        }
    }

}
