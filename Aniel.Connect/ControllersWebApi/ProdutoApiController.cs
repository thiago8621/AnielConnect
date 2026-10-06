using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ProdutoApiController : BaseApiController
    {

        private ProdutoCadastroModel GetViewObj(Produto obj)
        {

            ProdutoCadastroModel retorno = new ProdutoCadastroModel ();
            retorno.Id = obj.Id;
            retorno.Descricao = obj.Descricao;
            retorno.Nome_Venda = obj.Nome_Venda;
            retorno.OsPremier = obj.OsPremier;
            retorno.Codigo = obj.Codigo;
            retorno.Detalhes = obj.Detalhes;
            retorno.StatusInicialProduto = obj.Status_Inicial;
            retorno.AtivarValidade = obj.AtivarValidade;
            retorno.Data_Validade = obj.Data_Validade;
            retorno.ValorProduto = obj.ValorProduto?.ToString("N2");
            retorno.ProRataProduto = obj.Tem_Pro_Rata;
            retorno.Pro_Rata_Habilitacao = obj.Pro_Rata_Habilitacao;
            retorno.Pro_Rata_Interrupcao = obj.Pro_Rata_Interrupcao;
            retorno.Pro_Rata_Cancelamento = obj.Pro_Rata_Cancelamento;
            retorno.EmitirContratoProduto = obj.EmitirContratoProduto;
            retorno.IdModeloContrato = obj.EmitirContratoProduto ? obj.ModeloContratoProduto?.Id : null;
            retorno.ModeloContratoProduto = obj.ModeloContratoProduto?.Descricao;
            retorno.Tem_Carencia = obj.EmitirContratoProduto && obj.ModeloContratoProduto != null ? obj.ModeloContratoProduto.Tem_Carencia : false;
            retorno.Periodo_Carencia = obj.EmitirContratoProduto && obj.ModeloContratoProduto != null ? obj.ModeloContratoProduto.Periodo_Carencia : null;
            retorno.SituacaoProduto = obj.Situacao == Situacao_Produto.Ativo;
            retorno.Id_Conta_Financeira = obj.Conta_Financeira?.Id;
            retorno.Desc_Conta_Financeira = obj.Conta_Financeira?.Descricao;
            retorno.FrequenciaCobrancaProduto = obj.FrequenciaCobrancaProduto;
            retorno.ComprovantePagamentoProduto = obj.ComprovantePagamentoProduto;
            retorno.Atividade = obj.Atividade_Nf?.Descricao;
            retorno.Id_Cfop = obj.Cfop?.Id;
            retorno.Codigo_Cfop = obj.Cfop?.Codigo;
            retorno.FormatoCobrancaProduto = obj.FormatoCobrancaProduto;
            retorno.Id_Recibo = obj.Modelo_Recibo?.Id;
            retorno.Desc_Recibo = obj.Modelo_Recibo?.Descricao;      
            retorno.DescontoProgressivoProduto = obj.DescontoProgressivoProduto;
            retorno.MensalidadeInicialProduto = obj.MensalidadeInicialProduto;
            retorno.MensalidadeFinalProduto = obj.MensalidadeFinalProduto;
            retorno.PercentualDescontoProgressivo = obj.PercentualDescontoProgressivo;
            retorno.ValorDescontoProgressivo = obj.ValorDescontoProgressivo?.ToString("N2");
            retorno.Id_Tipo_Servico = obj.Id_Tipo_Servico;
            retorno.Id_Subtipo_Servico = obj.Id_Subtipo_Servico;
            retorno.Tipo_Produto = obj.Tipo_Produto;
            retorno.Utiliza_Desconto = obj.UtilizaDesconto;
            retorno.Desconto = obj.Desconto;
            retorno.Plano_Comercializavel = obj.Plano_Comercializavel;
            retorno.Dias_Vencimento = obj.Dias_Vencimento;
            retorno.Momento_Cobranca = obj.Momento_Cobranca;
            retorno.Cobranca_Realizada = obj.Cobranca_Realizada;
            retorno.IdFormaPagamentoCob = obj?.FormaPagamentoCob?.Id ?? null;

            #region Dados da instalação
            retorno.CobrancaInstalacao = obj.CobrancaInstalacao;
            retorno.ValorInstalacao = obj.ValorInstalacao.HasValue ? String.Format("{0:N2}", obj.ValorInstalacao) : null;
            retorno.IdContaFinanceiraInstalacao = obj.ContaFinanceiraInstalacao?.Id ?? null;
            retorno.MomentoCobrancaInstalacao = obj.MomentoCobrancaInstalacao;
            retorno.ComprovantePagamentoInstalacao = obj.ComprovantePagamentoInstalacao;
            retorno.OSTrocaEndereco = obj.OSTrocaEndereco;

            #endregion

            #region UsaPlanoProdutoAdicional

            retorno.UsaPlanoAdicional = obj.UsaPlanoAdicional;

            if (retorno.UsaPlanoAdicional)
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPlanoAdicional = new Conexao<PlanoProdutoAdicional>(session))
                {

                    List<PlanosProdutosAdicionaisCadastroModel> planosProdAdicionais = new List<PlanosProdutosAdicionaisCadastroModel>();

                    var listPlanProductAdc =  repoPlanoAdicional.select(p => p.Id_Produto.Id == retorno.Id).Result;

                    var retornoPlanosAdicionais = listPlanProductAdc.Select(p => new PlanosProdutosAdicionaisCadastroModel
                    {
                        Id = p.Id_Plano_Produto_Adicional,
                        Descricao = p.Descricao,
                    }).ToList();

                    retorno.PlanosProdutosAdicionais = retornoPlanosAdicionais;
                }           
                
            }
            #endregion

            retorno.MensalidadesProd = obj.Faixas_Mensalidades.Select(p => new PlanoMensalidadeCadastroModel
            {
                Id = p.Id,

                Parcela_Fin = Convert.ToString(p.Parcela_Fin),
                Parcela_Ini = Convert.ToString(p.Parcela_Ini),

                Percentual_Desconto = p.Percentual_Desconto.HasValue ? String.Format("{0}%", p.Percentual_Desconto.Value) : null,

                Valor_Desconto = p.Valor_Desconto.HasValue ? p.Valor_Desconto.Value.ToString("N2") : null
            }).ToList();

            retorno.ConfiguracaoProdNF = obj.Config_NF_PlanoProduto.Select(p => new PlanoProdutoConfigNfCadastroModel
            {
                Id = p.Id,

                ModeloNotaFiscal = p.ModeloNotaFiscal,
                DescModeloNotaFiscal = HtmlEnumExtensions.GetEnumDescription(p.ModeloNotaFiscal),

                IdClassificacaoTributaria = p.Configuracao_Tributaria?.Id,
                DescClassificacaoTributaria = p.Configuracao_Tributaria?.Descricao ?? "",
                PercentualDiscriminado = p.PercentualDiscriminado.HasValue ? p.PercentualDiscriminado?.ToString() + "%" : "",
                ValorDiscriminado = p.ValorDiscriminado.HasValue ? p.ValorDiscriminado?.ToString("N2") : 0.ToString("N2")
            }).ToList();

            retorno.UtilizaVendaAgroup = obj.UtilizaVendaAgroup;
            retorno.ValorVendaAgrupada = obj.ValorVendaAgrupada != 0 ? obj.ValorVendaAgrupada.ToString("F2") : "";

            return retorno;
        }

        async private Task BindViewToObj(ISession session, Produto objBanco, ProdutoCadastroModel view)
        {
            using (var repoModelo_Contrato = new Conexao<Modelo_Contrato>(session))
            using (var repoConta_Financeira = new Conexao<Conta_Financeira>(session))
            using (var repoAtividade_Nf = new Conexao<Atividade_Nf>(session))
            using (var repoCfop = new Conexao<Cfop>(session))
            using (var repoModelo_Recibo = new Conexao<Modelo_Recibo>(session))
            using (var repoConfigTributaria = new Conexao<Configuracao_Tributaria>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Descricao = view.Descricao;
                objBanco.Nome_Venda = view.Nome_Venda;
                objBanco.Detalhes = view.Detalhes;

                objBanco.Status_Inicial = view.StatusInicialProduto;
                objBanco.AtivarValidade = view.AtivarValidade;
                objBanco.Data_Validade = view.Data_Validade != null ? Convert.ToDateTime(view.Data_Validade) : (DateTime?)null;
                objBanco.ValorProduto = view.ValorProduto != null ? Convert.ToDouble(view.ValorProduto) : (Double?)null;

                objBanco.Tem_Pro_Rata = view.ProRataProduto;
                objBanco.Pro_Rata_Habilitacao = view.Pro_Rata_Habilitacao;
                objBanco.Pro_Rata_Interrupcao = view.Pro_Rata_Interrupcao;
                objBanco.Pro_Rata_Cancelamento = view.Pro_Rata_Cancelamento;

                objBanco.EmitirContratoProduto = view.EmitirContratoProduto;
                objBanco.ModeloContratoProduto = view.IdModeloContrato.HasValue ? await repoModelo_Contrato.selectFirst(p => p.Id == view.IdModeloContrato.Value) : null;

                objBanco.Conta_Financeira = view.Id_Conta_Financeira.HasValue ? await repoConta_Financeira.selectFirst(p => p.Id == view.Id_Conta_Financeira.Value) : null;
                objBanco.FrequenciaCobrancaProduto = view.FrequenciaCobrancaProduto;
                objBanco.ComprovantePagamentoProduto = view.ComprovantePagamentoProduto;

                objBanco.Atividade_Nf = view.Id_Atividade.HasValue ? await repoAtividade_Nf.selectFirst(p => p.Id == view.Id_Atividade.Value) : null;
                objBanco.Cfop = view.Id_Cfop.HasValue ? await repoCfop.selectFirst(p => p.Id == view.Id_Cfop.Value) : null;
                objBanco.FormatoCobrancaProduto = view.FormatoCobrancaProduto;
                objBanco.Modelo_Recibo = view.Id_Recibo.HasValue ? await repoModelo_Recibo.selectFirst(p => p.Id == view.Id_Recibo.Value) : null;

                objBanco.DescontoProgressivoProduto = view.DescontoProgressivoProduto;
                objBanco.MensalidadeInicialProduto = view.MensalidadeInicialProduto;
                objBanco.MensalidadeFinalProduto = view.MensalidadeFinalProduto;
                objBanco.PercentualDescontoProgressivo = view.PercentualDescontoProgressivo;
                objBanco.ValorDescontoProgressivo = view.ValorDescontoProgressivo == null ? (Double?)null : Convert.ToDouble(view.ValorDescontoProgressivo);

                objBanco.Percentual_Desconto = view.PercentualDesconto;

                objBanco.Id_Tipo_Servico = view.Id_Tipo_Servico;
                objBanco.Id_Subtipo_Servico = view.Id_Subtipo_Servico;
                objBanco.Tipo_Produto = view.Tipo_Produto;

                objBanco.UtilizaDesconto = view.Utiliza_Desconto;
                objBanco.Desconto = view.Desconto;
                objBanco.Plano_Comercializavel = view.Plano_Comercializavel;
                objBanco.Momento_Cobranca = view.Momento_Cobranca;
                objBanco.Dias_Vencimento = view.Dias_Vencimento;
                objBanco.Cobranca_Realizada = view.Cobranca_Realizada;
                objBanco.UtilizaVendaAgroup = view.UtilizaVendaAgroup;
                objBanco.ValorVendaAgrupada = view.ValorVendaAgrupada != String.Empty ? double.Parse(view.ValorVendaAgrupada) : 0.0;
                objBanco.UsaPlanoAdicional = view.UsaPlanoAdicional;
                objBanco.Nome_Venda = view.Nome_Venda;

                using (var repoFormaPag = new Conexao<Forma_Pagamento>(session)) {
                    objBanco.FormaPagamentoCob = await repoFormaPag.selectFirst(p => p.Id == view.IdFormaPagamentoCob);
                }

                #region Dados da Instalação

                objBanco.CobrancaInstalacao = view.CobrancaInstalacao;

                if (objBanco.CobrancaInstalacao)
                {
                    objBanco.ValorInstalacao = !String.IsNullOrEmpty(view.ValorInstalacao) ? Convert.ToDouble(view.ValorInstalacao) : (double?)null;
                    objBanco.ContaFinanceiraInstalacao = await repoConta_Financeira.selectFirst(p => p.Id == view.IdContaFinanceiraInstalacao.Value);
                    objBanco.MomentoCobrancaInstalacao = view.MomentoCobrancaInstalacao;
                    objBanco.ComprovantePagamentoInstalacao = view.ComprovantePagamentoInstalacao;
                }
                else
                {
                    objBanco.ValorInstalacao = null;
                    objBanco.ContaFinanceiraInstalacao = null;
                    objBanco.MomentoCobrancaInstalacao = null;
                    objBanco.ComprovantePagamentoInstalacao = null;
                }

                objBanco.OSTrocaEndereco = view.OSTrocaEndereco;

                #endregion

                //Mensalidade
                List<Faixas_Mensalidades> mensalidades = objBanco.Faixas_Mensalidades.ToList();
                foreach (var mensalidade in view.MensalidadesProd.FindAll(p => p.Id == 0))
                {
                    mensalidades.Add(new Faixas_Mensalidades
                    {
                        Parcela_Fin = Convert.ToInt16(mensalidade.Parcela_Fin),
                        Parcela_Ini = Convert.ToInt16(mensalidade.Parcela_Ini),
                        Percentual_Desconto = String.IsNullOrEmpty(mensalidade.Percentual_Desconto) ? 0 : Convert.ToDouble(mensalidade.Percentual_Desconto.Replace("%", "")),
                        Valor_Desconto = Convert.ToDouble(mensalidade.Valor_Desconto),
                        Produto = objBanco
                    });
                }
                objBanco.Faixas_Mensalidades = mensalidades;

                List<Config_NF_PlanoProduto> configs_nf = objBanco.Config_NF_PlanoProduto.ToList();

                foreach (var config in view.ConfiguracaoProdNF.FindAll(p => p.Id == 0))
                {
                    configs_nf.Add(new Config_NF_PlanoProduto
                    {
                        ValorDiscriminado = String.IsNullOrEmpty(config.ValorDiscriminado) ? (double?)null : Convert.ToDouble(config.ValorDiscriminado),
                        PercentualDiscriminado = String.IsNullOrEmpty(config.PercentualDiscriminado) ? (double?)null : Convert.ToDouble(config.PercentualDiscriminado),
                        Configuracao_Tributaria = await repoConfigTributaria.selectFirst(p => p.Id == config.IdClassificacaoTributaria),
                        ModeloNotaFiscal = config.ModeloNotaFiscal,
                        Produto = objBanco
                    });
                }
                objBanco.Config_NF_PlanoProduto = configs_nf;
            }
        }

        [HttpPost()]
        [Route("api/Produtoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PRODUTO SET SITUACAO = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Produtoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirPlanosProdutosListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())

                    {
                        var produtos = value.objs.FindAll(p => p.tipo == "Produto");

                        if (produtos.Count > 0)
                        {

                            String _ids = $"({String.Join(",", produtos)})";

                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_PRODUTO WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }
                        var planos = value.objs.FindAll(p => p.tipo == "Plano");

                        if (planos.Count > 0)
                        {
                            String _ids = $"({String.Join(",", planos)})";

                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_PLANO WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Produtoapi/FindById")]
        public async Task<GetApiRetorno<ProdutoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ProdutoCadastroModel> retorno = new GetApiRetorno<ProdutoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Produto>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        [HttpPost()]
        [Route("api/Produtoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ProdutoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Produto modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Produto>(session))
                    using (var repoPlanoProdutoAdicional = new Conexao<PlanoProdutoAdicional>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Produto
                            {
                                Situacao = Situacao_Produto.Ativo,
                                Codigo = Convert.ToString((await repository.MaxString("CODIGO", "TB_PRODUTO")) + 1),
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.Situacao = value.Objeto.SituacaoProduto ? Situacao_Produto.Ativo : Situacao_Produto.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        var listPlanProductAdc = await repoPlanoProdutoAdicional.select(p => p.Id_Produto.Id == modelo.Id);

                        foreach (var item in value.Objeto.PlanosProdutosAdicionais)
                        {
                            if(listPlanProductAdc != null)
                            {
                                var hasAny = listPlanProductAdc.Any(x => x.Id_Plano_Produto_Adicional == item.Id);

                                if (hasAny)
                                    continue;
                            }
                            
                            PlanoProdutoAdicional planoAdicional = new PlanoProdutoAdicional
                            {
                                Descricao = item.Descricao,
                                Id_Plano_Produto_Adicional = item.Id,
                                Id_Produto = modelo
                            };

                            await repoPlanoProdutoAdicional.SaveOrUpdate(planoAdicional);
                        }                     
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Produtoapi/ExcluirMensalidade")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirMensalidade(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Faixas_Mensalidades>(session))
                    {

                        Faixas_Mensalidades modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Produtoapi/ExcluirConfigNF")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirConfigNF(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Config_NF_PlanoProduto>(session))
                    {

                        Config_NF_PlanoProduto modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        //[HttpGet()]
        //[Route("api/PlanoApi/GetListDetalhes")]
        //public GetApiRetorno<ProdutoDetalhesModels> GetListDetalhes(Int64 Id_Plano)
        //{
        //    GetApiRetorno<ProdutoDetalhesModels> retorno = new GetApiRetorno<ProdutoDetalhesModels>
        //    {
        //        Objeto = new ProdutoDetalhesModels()
        //    };

        //    using (ISession session = HibernateUtil.GetSession())
        //    using (var repoProduto = new Conexao<Produto>(session))
        //    {
        //        try
        //        {
        //            var queryProduto = repoProduto.selectFirst(p => p.Id == Id_Plano).Result;

        //            if (queryProduto != null)
        //            {
        //                var produtoDetalhes = new ProdutoDetalhesModels();

        //                produtoDetalhes.ValorPlano = Convert.ToString(String.Format("{0:N2}", queryProduto.ValorProduto));
        //                produtoDetalhes.ValorPlanoProdutoFinal = Convert.ToString(String.Format("{0:N2}", queryProduto.ValorProduto));
        //                produtoDetalhes.Data_Validade = queryProduto.Data_Validade.HasValue ? queryProduto.Data_Validade.Value.ToString("dd/MM/yyyy") : String.Empty;
        //                produtoDetalhes.Qtde_Dias = queryProduto.Data_Validade.HasValue ? (queryProduto.Data_Validade.Value.Date - DateTime.Now.Date).Days : 0;
        //                produtoDetalhes.Detalhes = queryProduto.Detalhes;

        //                retorno.Objeto = produtoDetalhes;
        //            }
        //            else
        //            {
        //                retorno.Objeto = new ProdutoDetalhesModels();
        //            }

        //            retorno.Ok = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            retorno.Mensagem = ex.Message;
        //            if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
        //        }
        //    }
        //    return retorno;

        //}

        [Route("api/Produtoapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Produto>(session))
                    {

                        Produto modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
    }
}
