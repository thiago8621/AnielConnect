using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.AplicacaoMaterialHubsoft;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.DocumentoProducao
{
    public interface IDocumentoProducaoAnielRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta às ordens de serviço registadas
        ///     no sistema
        /// </summary>
        IQueryable<Documento_ProducaoAniel> GetAll();

        /// <summary>
        ///     Verifica se a OS associada às chaves informadas é uma OS reincidente e, caso seja,
        ///     retorna a OS que originou essa reincidência
        /// </summary>
        /// <param name="codct">CODCT da OS que pode ser reincidente</param>
        /// <param name="projeto">PROJETO da OS que pode ser reincidente</param>
        /// <param name="numObra">NUM_OBRA da OS que pode ser reincidente</param>
        /// <param name="numDoc">NUM_DOC da OS que pode ser reincidente</param>
        /// <param name="dataCriacao">DIA da OS que pode ser reincidente</param>
        ChavesDocumentoProducao GetOSReincidente(
            string codct,
            string projeto,
            string numObra,
            int numDoc,
            DateTime dataCriacao
        );

        /// <summary>
        ///     Retorna o extrato de produtividade contendo todas as OS contadas para a contagem
        ///     de OSs produtivas e improdutivas no relatório de produtividade por técnico
        /// </summary>
        /// <param name="filtros">Filtros do relatório</param>
        /// <param name="considerarAgendamentoPeriodo">Define se a empresa trabalha com agendamento por turno</param>
        RelProdTecnicoExtratoProdutividadeResponse GetExtratoProdutividade(
            RelatorioProdutividadeTecnicoDataTableRequest filtros,
            bool considerarAgendamentoPeriodo);

        /// <summary>
        ///     Retorna o extrato de prazo contendo todas as OS usadas na contagem
        ///     de OSs dentro e fora no relatório de produtividade por técnico
        /// </summary>
        /// <param name="filtros">Filtros do relatório</param>
        /// <param name="considerarAgendamentoPeriodo">Define se a empresa trabalha com agendamento por turno</param>
        RelProdTecnicoExtratoPrazoResponse GetExtratoPrazo(
            RelatorioProdutividadeTecnicoDataTableRequest filtros,
            bool considerarAgendamentoPeriodo);

        /// <summary>
        ///     Retorna o extrato de reincidência contendo todas as OS usadas na contagem
        ///     de OSs reincidentes no relatório de produtividade por técnico
        /// </summary>
        /// <param name="filtros">Filtros do relatório</param>
        /// <param name="apenasServicosProdutivosReinc">
        ///     Define se apenas serviços reincidentes fechados com status produtivo serão 
        ///     levados em consideração no extrato
        /// </param>
        RelProdTecnicoExtratoReincidenciaResponse GetExtratoReincidencia(
            RelatorioProdutividadeTecnicoDataTableRequest filtros,
            bool apenasServicosProdutivosReinc);

        /// <summary>
        /// preenche campos de tipo de serviço e codigo departamento
        /// </summary>
        /// <param name="pTipo">use LAST para retornar ultima OS da busca ou FIRST pra retornar a primeira</param>
        /// <param name="pGetConfigFluxoBaixa">Representa dados do serviço</param>
        /// <returns></returns>
        DocumentoProducao_GetConfigFluxoBaixa GetTipoServDepOS(String pNucleo,
            String pTipo,
            DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa);

        /// <summary>
        /// Retorna dados referentes a tabela TB_VINC_ENCERRAMENTO_NATUREZA
        /// </summary>
        /// <param name="pGetConfigFluxoBaixa">Representa dados do serviço</param>
        /// <returns></returns>
        DocumentoProducao_GetConfigFluxoBaixa GetConfigFluxoBaixa(
            DocumentoProducao_GetConfigFluxoBaixa pGetConfigFluxoBaixa);

        /// <summary>
        /// Atualiza DATA_PROD
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        void AtualizarDataProducao(Documento_ProducaoAniel documento);

        /// <summary>
        /// Aplica a producao prevista pra OS
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        /// <param name="usuario">Usuario responsavel pela aplicacao</param>
        void AplicaProducaoPrevista(Documento_ProducaoAniel documento, string usuario);

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
        void InsertLog_Lanc_Servico(long? CodMotivoReagendamento, String ObsMotivoReagendamento ,  String CodCt,
            String Projeto, 
            String Num_Os, 
            Int32 Num_Doc,
            String Alteracao, 
            String Valor_Anterior, 
            String Usuario, 
            DateTime? Data_Maxima, 
            Int16? Cod_Tipo_Serv, 
            String Valor_Atualizado, 
            String Obs_Cobranca_Status,
            ITransaction transaction = null);

        /// <summary>
        /// Gera novo sequencial pra proxima OS de mesmo NUM_OBRA_ORIGINAL
        /// </summary>
        /// <param name="repository">repositorio de TB_DOCUMENTO_PRODUCAO</param>
        /// <param name="Codct">CODCT/Contrato da OS</param>
        /// <param name="Projeto">Projeto da OS</param>
        /// <param name="OS">Numero da OS</param>
        /// <param name="Num_Doc">NUM_DOC/Codigo do cliente</param>
        /// <returns></returns>
        Int64 GerarSeq(String Codct, String Projeto, String OS, Int32 Num_Doc);

        /// <summary>
        /// Busca numero do serviço pra criação de novo serviço
        /// </summary>
        /// <param name="config">entidade que representa TB_CONFIGURACOES_GERAIS</param>
        /// <returns></returns>
        String GetNum_Obra(Configuracoes_Gerais config);

        /// <summary>
        /// Busca tipo de cobranca que se encaixa com as caracteristicas do serviço
        /// </summary>
        /// <param name="documento">Objeto que representa Entidade de TB_DOCUMENTO_PRODUCAO</param>
        /// <returns></returns>
        Cobranca_Servico BuscarCobrancaDocumento(Documento_ProducaoAniel documento);

        /// <summary>
        /// Listar Observacoes feitas em cada baixa 
        /// </summary>
        /// <param name="repository">repositorio de TB_DOCUMENTO_PRODUCAO</param>
        /// <param name="Codct">CODCT/Contrato da OS</param>
        /// <param name="Projeto">Projeto da OS</param>
        /// <param name="NumObraOriginal">Numero da OS</param>
        /// <param name="Num_Doc">NUM_DOC/Codigo do cliente</param>
        /// <returns></returns>
        List<string> ListarObservacoesBaixa(String Codct, String Projeto, Int32 Num_Doc, String NumObraOriginal);

        /// <summary>
        /// Retorna o próximo numDoc sequencial para serviços de manutenção
        /// </summary>
        /// <returns></returns>
        int GetNumDocManutencao();

        /// <summary>
        /// Retorna o tempo configurado para execucao do servico baseado no tipo de imovel e tipo de servico
        /// </summary>
        /// <param name="config"></param>
        /// <param name="codTipoImovel"></param>
        /// <param name="codTipoServ"></param>
        /// <returns></returns>
        TimeSpan? GetTempoExecucao(Configuracoes_Gerais config, int? codTipoImovel, short? codTipoServ);

        Documento_ProducaoAniel GetProximaOSAgendadaPraTecnico(string equipe);

        /// <summary>
        /// Retorna o tempo configurado para execucao do servico baseado nas informações da OS
        /// </summary>
        /// <param name="config"></param>
        /// <param name="inicio"></param>
        /// <param name="CodCt"></param>
        /// <param name="Projeto"></param>
        /// <param name="Num_Os"></param>
        /// <param name="Num_Doc"></param>
        /// <returns></returns>
        TimeSpan? GetTempoExecucao(Configuracoes_Gerais config, DateTime inicio, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc);

        /// <summary>
        /// Retorna os servicos para o painel de gestao operacional
        /// </summary>
        /// <param name="request">filtros da requisicao</param>
        /// <param name="tempoExecucaoPadrao">tempo padrao de execucao configurado</param>
        /// <param name="tempoDeslocamentoPadrao">tempo padrao de deslocamento configurado</param>
        /// <returns></returns>
        GestaoOperacionalServicosResponse GetServicosPainelGestaoOperacional(GestaoCapacidadeRoteirizacaoRequest request,
                                                                             TimeSpan? tempoExecucaoPadrao,
                                                                             TimeSpan? tempoDeslocamentoPadrao);
        string GetJoinCountPainelServico(string WhereCondition);


        /// <summary>
        /// Retorna a primeira OS encontrada, dado os parametros
        /// </summary>
        /// <param name="codCt"></param>
        /// <param name="projeto"></param>
        /// <param name="num_doc"></param>
        /// <param name="numObra"></param>
        /// <returns></returns>
        Documento_ProducaoAniel GetFirstDocProducao(string codCt, string projeto, int num_doc, string numObra);

        /// <summary>
        /// Atualiza uma OS ou Salva uma nova no banco
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        void SaveOrUpdate(Documento_ProducaoAniel documento);

        /// <summary>
        /// Retorna lista com DocPrd/OS em abertos com o tempo expirado e seus codigos de encerramento
        /// </summary>
        /// <returns></returns>
        List<DocProducaoBaixaAutomatica> GetOrdensServicoAuto();

        Task<bool> SaveUpdateDocumentoAsync(Documento_ProducaoAniel documento);
        DocumentoProducaoMongo GetOSMongo(Documento_ProducaoAniel documento);
        Task<FuncionarioAniel> GetFuncionario(string Num_Fre);
        Task<SenhaAniel> GetSenhaAniel(string usuario);
        Task<bool> UpdateMongo(DocumentoProducaoMongo documento);
        Task<bool> SaveMongo(DocumentoProducaoMongo documento);
        Task<bool> AtualizaVinculoManutencao(AlteraVinculoManutencao request);
        Task<Boolean> isOsManutencao(string numObra);
        string GetManutencao_OT_OR(string CodCt, string Projeto, string Num_Obra, Int32 Num_Doc);
        Int32 GetManutencao_OT_OR(string CodCt, string Projeto, string Num_Ot_Or);
        Task<bool> ConfereMaterialIntegrado(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        Task<ConsultaIdOsHubsoft> GetDadosOSByNumObra(ISession sessionT5, string numObra, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        List<Material_Aplicado> MateriaisAplicados(ISession sessionT5, String Codct, String Projeto, String OS, string Detalhe, string Usuario, string CodMat, bool provisionamento, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        Task<long> MarcarIntegrado(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, bool provisionamento, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        Task<ConsultaNumOs> GetDadosOSByNumOs(ISession sessionT5, string numOs, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        void AdicionaSaldo(ISession sessionT5, LoginHubsoftModel login, string urlBase, string detalheHubsoft, string usuarioHubsoft, string codCt, string projeto, string numObra, List<Material_Aplicado> materiais, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        void AdicionarEquipamento(ISession sessionT5, string detalheHubsoft, string usuarioHubsoft, string serial, string codmat, string usuario, string numObra, ConsultaIdOsHubsoft ordemServico);
        AplicacaoRemocaoMaterial GetAplicacaoNaoIntegrados(ISession sessionT5, string numObra, string codCpl, string codMat, string projeto, bool provisionamento, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        MovimentoProdutoUtilizadoFecharOSPop BindMateriaisAplicacaoPop(AplicacaoRemocaoMaterial materialPendente, LoginHubsoftModel loginHubsoft, IntegracoesExternas dadosIntegracaoHubsoft, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        MovimentoProdutoUtilizadoFecharOS BindMateriaisAplicacao(AplicacaoRemocaoMaterial materialPendente, OrdemServicoHubSoft ordemServicoHubSoft, LoginHubsoftModel loginHubsoft, IntegracoesExternas dadosIntegracaoHubsoft, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        void MarcarImportados(ISession sessionT5, long id, string envio, string retorno);
        void MarcarExcecao(ISession sessionT5, long id, string envio, string excecao);
        void AtualizaPPPOEDocumentoProducao(ISession sessionT5, LoginRadius loginRadius, string numObra, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        Int64 GetIDConsumoProtheus(ISession sessionT5, string numObra, string codCpl, string codMat, string projeto, string codCt, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        Task<bool> RemoverAplicacao(ISession sessionT5, string codCt, string projeto, string numObra, string codMat, string codCpl, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodo"></param>
        /// <returns></returns>        
    }
}
