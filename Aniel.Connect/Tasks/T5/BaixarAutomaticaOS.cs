using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.CodigoEncerramento;
using Aniel.Connect.Repository.T5.Contrato;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;

namespace Aniel.Connect.Tasks.T5
{
    public class BaixarAutomaticaOS : BackgroundTask
    {
        public async Task<bool> GerarBaixaAutoOs()
        {

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoContratoConexao = new Conexao<ContratoAniel>(sessionT5))
                {
                    IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5);
                    ICodigoEncerramentoAnielRepository repoCodEncerr = new CodigoEncerramentoAnielRepository(sessionT5);
                    IContratoAnielRepository repoContrato = new ContratoAnielRepository(sessionT5);
                    IConfiguracoesGeraisRepository repoConfGerais = new ConfiguracoesGeraisRepository(session);
                    IClientePlanoProdutoRepository repoCliPlanProd = new ClientePlanoProdutoRepository(session);
                    Documento_ProducaoApiController docProdApi = new Documento_ProducaoApiController();

                    var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd, repoNatureza, repoCodEncerr, repoContrato, repoConfGerais, repoCliPlanProd);

                    //obter configuracoes do contrato
                    var contratoConf = await repoContratoConexao.selectFirst(c => c.EncerraOSAuto == 'S');

                    if (contratoConf is null)
                    {
                        CONSTANTES.log4Net.Info("Não há nenhum contrato com a configuração ativada para EncerrarOSAuto");
                        return true;
                    }

                    var docsPrd = repoDocProd.GetOrdensServicoAuto();

                    if (docsPrd is null)
                    {
                        CONSTANTES.log4Net.Info("Não há nenhuma OS em aberto dada as configurações do Contrato.");
                        return true;
                    }

                    foreach (var doc in docsPrd)
                    {
                        var baixarDoc = new DocProdBaixa
                        {
                            CodCt = doc.CodCt,
                            Projeto = doc.Projeto,
                            Num_Doc = doc.Num_Doc,
                            Num_Obra = doc.Num_Obra,
                            Observacao = doc.Observacao,
                            Codigo_Encerramento = doc.CodEncerramentoAuto,
                            Equipe = doc.Equipe,
                            Data_Ini_Atend = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                            Data_Fim_Atend = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                            Hora_Ini_Atend = DateTime.Now.ToString("hh:mm:ss"),
                            Hora_Fim_Atend = DateTime.Now.ToString("hh:mm:ss"),
                            CaixaFTTH = doc.CaixaFTTH,
                            PortaFTTH = doc.PortaFTTH,
                            Usuario = "SISTEMA",
                            BaixaOsProdDuplicada = "N"
                        };

                        var documentoBaixado = documentoBusiness.BaixarOrdemServico(baixarDoc, session, CONSTANTES.UsaRadiusSemContainer);

                        if (!documentoBaixado.Ok)
                        {
                            CONSTANTES.log4Net.Error(documentoBaixado.Mensagem + " Inner: " + documentoBaixado.InnerException);
                            continue;
                        }
                            
                        var objDoc = new DocumentoProducao_GetDocumento
                        {
                            CodCt = documentoBaixado.DocProducao.CodCt,
                            Num_Obra = documentoBaixado.DocProducao.Num_Obra,
                            Num_Doc = documentoBaixado.DocProducao.Num_Doc,
                            Projeto = documentoBaixado.DocProducao.Projeto,
                            Num_Obra_Original = documentoBaixado.DocProducao.Numero_Obra_Original
                        };

                        await docProdApi.EncerrarAtendimentoJuntoOS(objDoc);
                        await docProdApi.RotinaRouter(objDoc);
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error($"Foi gerado uma exception ao baixar a OS Automaticamente: {ex}");
                return false;
            }

            return true;
        }

    }
}