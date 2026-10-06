using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Integracoes.Saperx.Services;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model.HubSoft;
using Aniel.Connect.Repository.Repository.Alcans;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PlanoApiController : BaseApiController
    {
        private PlanoCadastroModel GetViewObj(Plano obj)
        {
            var retorno = new PlanoCadastroModel();
            //atributo_modelview= objeto_banco
            //Bloco1

            retorno.Id = obj.Id;
            retorno.Codigo = obj.Codigo;
            retorno.Descricao = obj.Descricao;
            retorno.Nome_Venda = obj.Nome_Venda;
            retorno.Id_Radius = obj.Radius?.Id;
            retorno.AutenticacaoRadius = obj.AutenticacaoRadius;
            retorno.SituacaoPlano = obj.SituacaoPlano == Situacao_plano.Ativo ? true : false;
            retorno.VelocidadeNominal = obj.VelocidadeNominal;
            retorno.VelocidadeDownload = obj.VelocidadeDownload;
            retorno.AtributoIPOE = obj.UtilizaAtributoIPOE;
            retorno.NomeIPOE = obj.Atributo_IPOE?.Id;
            //Desbloqueio_Conf = obj.UtilizaDesbloqueioConfianca;
            retorno.BLOQUEIO_AUTO_DEBITO = obj.UtilizaBloqueioDebito;
            //Frequencia_Desbloqueio = obj.FrequenciaDesbloqueio;
            retorno.VelocidadeUpload = obj.VelocidadeUpload;
            retorno.Id_Tecnologia = obj.Tecnologia?.Id;
            retorno.Tipo_Tecnologia = obj.Tipo_Tecnologia;
            retorno.ValorPlano = String.Format("{0:N2}", obj.ValorPlano);
            retorno.Detalhes = obj.DetalhesPlano;
            retorno.StatusInicial = obj.StatusInicial;
            retorno.Tipo = obj.Tipo;
            retorno.ProdutoQtdVariavel = obj.ProdutoQtdVariavel;

            //Bloco2
            retorno.CobrancaInstalacao = obj.CobrancaInstalacao;
            retorno.CobrancaInstalacaoEntrada = obj.CobrancaInstalacaoEntrada;
            retorno.ValorInstalacao = String.Format("{0:N2}", obj.ValorInstalacao);
            retorno.Id_ReciboInstalacaco = obj.Modelo_Recibo_Instalacao?.Id;
            retorno.Id_ContaFinanceiraInstalacao = obj.Conta_FinanceiraInstalacao?.Id;
            retorno.DescContaFinanceiraInstalacao = obj.Conta_FinanceiraInstalacao?.Descricao;
            retorno.Id_ContaFinanceiraPlano = obj.Conta_FinanceiraPlano?.Id;
            retorno.DescContaFinanceiraPlano = obj.Conta_FinanceiraPlano?.Descricao;
            retorno.ModoCobrancaInstalacao = obj.ModoCobrancaInstalacao;
            retorno.QtdParcelas = obj?.QtdParcelas;
            retorno.OSTrocaEndereco = obj.OSTrocaEndereco;
            retorno.DiasBoleto = obj.DiasBoleto;
            retorno.FormaPagamentoCobranca = obj.FormaPagamentoCobranca;

            //Bloco3
            retorno.ReducaoVelocidade = obj.ReducaoVelocidade;
            retorno.QtdDiasAtraso = obj.QtdDiasAtraso;
            retorno.QtdDiasbloqueados = obj.QtdDiasbloqueados;

            retorno.VelocidadeDownloadReduzida = obj.VelocidadeDownloadReduzida;
            retorno.VelocidadeUploadReduzida = obj.VelocidadeUploadReduzida;
            retorno.AtivarValidade = obj.AtivarValidade;
            retorno.Validade = obj.Validade;
            retorno.IdFormaPagamentoCob = obj?.FormaPagamentoCob?.Id ?? null;
            //Bloco4
            retorno.FrequenciaCobranca = obj.FrequenciaCobranca;
            retorno.Momento_Cobranca_Plano = obj.Momento_Cobranca_Plano;
            retorno.ComprovantePagamento = obj.ComprovantePagamento;
            retorno.ComprovantePagamentoFinanceiro = obj.ComprovantePagamentoFinanceiro;
            retorno.Momento_Cobra_Instalacao = obj.Momento_Cobra_Instalacao;
            retorno.Id_Recibo = obj.Modelo_Recibo?.Id;
            retorno.Desc_Recibo = obj.Modelo_Recibo?.Descricao;
            retorno.Id_Cfop = obj.Cfop?.Id;
            retorno.Codigo_Cfop = obj.Cfop?.Codigo;
            retorno.FormatoCobranca = obj.FormatoCobranca;
            retorno.EmitirContrato = obj.EmitirContrato;
            retorno.IdModeloContrato = obj.Modelo_Contrato?.Id;
            retorno.DescModeloContrato = obj.Modelo_Contrato?.Descricao;
            retorno.Tem_Carencia = obj.Modelo_Contrato?.Tem_Carencia ?? false;
            retorno.Periodo_Carencia = obj.Modelo_Contrato?.Periodo_Carencia;
            retorno.Disponibiliza_Telefonia = obj.Disponibiliza_Telefonia;
            retorno.RetecaoImposto = obj.RetencaoImposto;
            retorno.QtdParcelas_ProdutoVariavel = obj.QtdParcelas_ProdutoVariavel;
            retorno.PossuiValorEntrada = obj.PossuiValorEntrada;

            //Bloco5
            retorno.ModoCobranca = obj.Modo_Cobranca_Plano;
            retorno.Cobranca_Recorrente = obj.Cobranca_Recorrente;
            retorno.Utiliza_Desconto = obj.UtilizaDesconto;
            retorno.Desconto = obj.Desconto;
            retorno.MensalidadeInicial = obj.MensalidadeInicial;
            retorno.MensalidadeFinal = obj.MensalidadeFinal;
            retorno.PercentualDescontoProgressivo = String.Format("{0:N2}", obj.PercentualDescontoProgressivo);
            retorno.ValorDescontoProgressivo = String.Format("{0:N2}", obj.ValorDescontoProgressivo);
            retorno.Plano_Comercializavel = obj.Plano_Comercializavel;
            retorno.Dias_Vencimento = obj.Dias_Vencimento;
            retorno.Momento_Cobranca = obj.Momento_Cobranca;
            retorno.Cobranca_Realizada = obj.Cobranca_Realizada;
            retorno.Modelo_Anatel = obj.Modelo_Anatel;
            retorno.Tecnologia_Anatel = obj.Tecnologia_Anatel;
            retorno.Tipo_Produto = obj.Tipo_Produto;
            retorno.Tipo_ProdutoScm = obj.Tipo_ProdutoScm;
            retorno.Meio_Acesso_Anatel = obj.Meio_Acesso_Anatel;
            retorno.Regime = obj.Regime;
            retorno.Modalidade_Anatel = obj.Modalidade_Anatel;
            retorno.Tarifa_Anatel = obj.Tarifa_Anatel;
            retorno.Id_Tipo_Servico = obj.Id_Tipo_Servico;
            retorno.Id_Subtipo_Servico = obj.Id_Subtipo_Servico;
            retorno.PossuiConsumoMinimo = obj.PossuiConsumoMinimo;
            retorno.PossuiFranquia = obj.PossuiFranquia;
            retorno.IdFranquia = obj.Franquia?.Id;
            retorno.ConsumoMinimo = obj.ConsumoMinimo.HasValue ? obj.ConsumoMinimo.Value.ToString("N2") : null;
            retorno.FormatoTarifaTelefonia = obj.FormatoTarifaTelefonia;
            retorno.Cobrar_Por_Telefonia = obj.Cobrar_Por_Telefonia;
            retorno.Utiliza_Integraca_Externa = obj.Utiliza_Integracao_Externa;
            retorno.Integracao = obj.Integracao;
            retorno.Num_Dids = obj.Num_Dids;
            retorno.Formato_do_plano = obj.Formato_do_Plano;
            retorno.IntragrupoLcGratuita = obj.IntragrupoLcGratuita;
            retorno.IntragrupoGratuita = obj.IntragrupoGratuita;
            retorno.CobrarBina = obj.CobrarBina;
            retorno.CobrarBlacklist = obj.CobrarBlacklist;
            retorno.CobrarBloqueioChamadaCobrar = obj.CobrarBloqueioChamadaCobrar;
            retorno.CobrarSigame = obj.CobrarSigame;
            retorno.ValorBina = obj.ValorBina.HasValue ? obj.ValorBina.Value.ToString("N2") : null;
            retorno.ValorBlacklist = obj.ValorBlacklist.HasValue ? obj.ValorBina.Value.ToString("N2") : null;
            retorno.ValorBloqueioChamadaCobrar = obj.ValorBloqueioChamadaCobrar.HasValue ? obj.ValorBloqueioChamadaCobrar.Value.ToString("N2") : null;
            retorno.CadenciaLc = obj.CadenciaLc;
            retorno.CadenciaLdn = obj.CadenciaLdn;
            retorno.CadenciaLdi = obj.CadenciaLdi;
            retorno.CadenciaVc1 = obj.CadenciaVc1;
            retorno.CadenciaVc2 = obj.CadenciaVc2;
            retorno.CadenciaVc3 = obj.CadenciaVc3;
            retorno.ValorLc = obj.ValorLc.HasValue ? obj.ValorLc.Value.ToString("N2") : null;
            retorno.ValorLdn = obj.ValorLdn.HasValue ? obj.ValorLdn.Value.ToString("N2") : null;
            retorno.ValorLdi = obj.ValorLdi.HasValue ? obj.ValorLdi.Value.ToString("N2") : null;
            retorno.ValorVc1 = obj.ValorVc1.HasValue ? obj.ValorVc1.Value.ToString("N2") : null;
            retorno.ValorVc2 = obj.ValorVc2.HasValue ? obj.ValorVc2.Value.ToString("N2") : null;
            retorno.ValorVc3 = obj.ValorVc3.HasValue ? obj.ValorVc3.Value.ToString("N2") : null;
            retorno.ValorSigame = obj.ValorSigame.HasValue ? obj.ValorSigame.Value.ToString("N2") : null;
            retorno.HabilitarLc = obj.HabilitarLc;
            retorno.HabilitarLdn = obj.HabilitarLdn;
            retorno.HabilitarLdi = obj.HabilitarLdi;
            retorno.HabilitarVc1 = obj.HabilitarVc1;
            retorno.HabilitarVc2 = obj.HabilitarVc2;
            retorno.HabilitarVc3 = obj.HabilitarVc3;
            retorno.BloqueioChamadaCobrar = obj.BloqueioChamadaCobrar;

            retorno.Id_Tipo_Servico = obj.Id_Tipo_Servico;
            retorno.Id_Subtipo_Servico = obj.Id_Subtipo_Servico;
            retorno.Id_Tipo_ServicoOS = obj.Id_Tipo_ServicoOS;
            retorno.Id_Subtipo_ServicoOS = obj.Id_Subtipo_ServicoOS;
            retorno.DefinirTipo_SubTipoOS = obj.DefinirTipo_SubTipoOS;
            retorno.Os_Mudanca_Plano = obj.OS_Mudanca_Plano;
            retorno.PossuiConsumoMinimo = obj.PossuiConsumoMinimo;
            retorno.PossuiFranquia = obj.PossuiFranquia;
            retorno.IdFranquia = obj.Franquia?.Id;
            retorno.ConsumoMinimo = obj.ConsumoMinimo.HasValue ? obj.ConsumoMinimo.Value.ToString("N2") : null;
            retorno.FormatoTarifaTelefonia = obj.FormatoTarifaTelefonia;
            retorno.Cobrar_Por_Telefonia = obj.Cobrar_Por_Telefonia;
            retorno.Utiliza_Integraca_Externa = obj.Utiliza_Integracao_Externa;
            retorno.Integracao = obj.Integracao;
            retorno.Num_Dids = obj.Num_Dids;
            retorno.Formato_do_plano = obj.Formato_do_Plano;
            retorno.IntragrupoLcGratuita = obj.IntragrupoLcGratuita;
            retorno.IntragrupoGratuita = obj.IntragrupoGratuita;
            retorno.CobrarBina = obj.CobrarBina;
            retorno.CobrarBlacklist = obj.CobrarBlacklist;
            retorno.CobrarBloqueioChamadaCobrar = obj.CobrarBloqueioChamadaCobrar;
            retorno.CobrarSigame = obj.CobrarSigame;
            retorno.ValorBina = obj.ValorBina.HasValue ? obj.ValorBina.Value.ToString("N2") : null;
            retorno.ValorBlacklist = obj.ValorBlacklist.HasValue ? obj.ValorBina.Value.ToString("N2") : null;
            retorno.ValorBloqueioChamadaCobrar = obj.ValorBloqueioChamadaCobrar.HasValue ? obj.ValorBloqueioChamadaCobrar.Value.ToString("N2") : null;
            retorno.CadenciaLc = obj.CadenciaLc;
            retorno.CadenciaLdn = obj.CadenciaLdn;
            retorno.CadenciaLdi = obj.CadenciaLdi;
            retorno.CadenciaVc1 = obj.CadenciaVc1;
            retorno.CadenciaVc2 = obj.CadenciaVc2;
            retorno.CadenciaVc3 = obj.CadenciaVc3;
            retorno.ValorLc = obj.ValorLc.HasValue ? obj.ValorLc.Value.ToString("N2") : null;
            retorno.ValorLdn = obj.ValorLdn.HasValue ? obj.ValorLdn.Value.ToString("N2") : null;
            retorno.ValorLdi = obj.ValorLdi.HasValue ? obj.ValorLdi.Value.ToString("N2") : null;
            retorno.ValorVc1 = obj.ValorVc1.HasValue ? obj.ValorVc1.Value.ToString("N2") : null;
            retorno.ValorVc2 = obj.ValorVc2.HasValue ? obj.ValorVc2.Value.ToString("N2") : null;
            retorno.ValorVc3 = obj.ValorVc3.HasValue ? obj.ValorVc3.Value.ToString("N2") : null;
            retorno.ValorSigame = obj.ValorSigame.HasValue ? obj.ValorSigame.Value.ToString("N2") : null;
            retorno.HabilitarLc = obj.HabilitarLc;
            retorno.HabilitarLdn = obj.HabilitarLdn;
            retorno.HabilitarLdi = obj.HabilitarLdi;
            retorno.HabilitarVc1 = obj.HabilitarVc1;
            retorno.HabilitarVc2 = obj.HabilitarVc2;
            retorno.HabilitarVc3 = obj.HabilitarVc3;
            retorno.BloqueioChamadaCobrar = obj.BloqueioChamadaCobrar;

            //Ativação de Sinal
            retorno.UtilizaQoSProfile = obj.UtilizaQoSProfile;
            retorno.IdQoSProfile = obj.IdQoSProfile;
            retorno.IdQoSProfile_Reduzido = obj.IdQoSProfileReduzido;
            retorno.UtilizaBloqueioDomainName = obj.BloqueioDomainName;
            //UtilizaPolicyName = obj.UtilizaPolicyName;
            retorno.IdPolicyNameDownload = obj.IdPolicyNameDownload;
            retorno.IdPolicyNameDownload_Reduzido = obj.IdPolicyNameDownloadReduzido;
            retorno.IdPolicyNameUpload = obj.IdPolicyNameUpload;
            retorno.IdPolicyNameUpload_Reduzido = obj.IdPolicyNameUploadReduzido;
            retorno.UtilizaPolicyNameIPv6 = obj.UtilizaPolicyNameIPv6;
            retorno.UtilizaBloqueioPoolIp = obj.BloqueioPoolIp;
            retorno.IdDomainName = obj.IdDomainName;
            retorno.ProductId = obj.ProductId;
            retorno.CodigoIntegracao = obj.CodigoIntegracao;
            retorno.CodigoIntegracaoAuxiliar = obj.CodigoIntegracaoAuxiliar;
            retorno.ProductId = obj.ProductId;
            retorno.CodigoTipoPlanoIntegracao = obj.CodigoTipoPlanoIntegracao;
            retorno.UtilizaVendaAgroup = obj.UtilizaVendaAgroup;
            retorno.ValorVendaAgrupada = obj.ValorVendaAgrupada != 0 ? obj.ValorVendaAgrupada.ToString("F2") : "";

            retorno.ProductId = obj.ProductId;
            retorno.CodigoTipoPlanoIntegracao = obj.CodigoTipoPlanoIntegracao;
            retorno.CodigoIntegracao = obj.CodigoIntegracao;
            retorno.CodigoIntegracaoAuxiliar = obj.CodigoIntegracaoAuxiliar;
            retorno.ExclusivoCliente = obj.ExclusivoCliente;
            /*UtilizaQoSProfile = obj.UtilizaQoSProfile,
            IdQoSProfile = obj.IdQoSProfile,

            IdQoSProfile_Reduzido = obj.IdQoSProfileReduzido,
            UtilizaBloqueioDomainName = obj.BloqueioDomainName,
            //UtilizaPolicyName = obj.UtilizaPolicyName,
            IdPolicyNameDownload = obj.IdPolicyNameDownload,
            IdPolicyNameDownload_Reduzido = obj.IdPolicyNameDownloadReduzido,
            IdPolicyNameUpload = obj.IdPolicyNameUpload,
            IdPolicyNameUpload_Reduzido = obj.IdPolicyNameUploadReduzido,
            UtilizaPolicyNameIPv6 = obj.UtilizaPolicyNameIPv6,
            UtilizaBloqueioPoolIp = obj.BloqueioPoolIp,
            IdDomainName = obj.IdDomainName,

            CobrancaInstalacaoEntrada = obj.CobrancaInstalacaoEntrada,*/

            #region UsaPlanoProdutoAdicional

            retorno.UsaPlanoAdicional = obj.UsaPlanoAdicional;

            if (retorno.UsaPlanoAdicional)
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoplanoadicional = new Conexao<PlanoProdutoAdicional>(session))
                {
                    List<PlanosProdutosAdicionaisCadastroModel> planosProdAdicionais = new List<PlanosProdutosAdicionaisCadastroModel>();

                    var listPlanProductAdc = repoplanoadicional.select(p => p.Id_Plano.Id == retorno.Id).Result;

                    var retornoPlanosAdicionais = listPlanProductAdc.Select(p => new PlanosProdutosAdicionaisCadastroModel
                    {
                        Id = p.Id_Plano_Produto_Adicional,
                        Descricao = p.Descricao,
                    }).ToList();

                    retorno.PlanosProdutosAdicionais = retornoPlanosAdicionais;
                }
            }
            #endregion

            retorno.Mensalidades = obj.Faixas_Mensalidades.Select(p => new PlanoMensalidadeCadastroModel
            {
                Id = p.Id,
                Parcela_Fin = p.Parcela_Fin.ToString(),
                Parcela_Ini = p.Parcela_Ini.ToString(),
                Percentual_Desconto = p.Percentual_Desconto.Value < 0 ? String.Format("{0}%", p.Percentual_Desconto.Value) : null,
                Valor_Desconto = p.Valor_Desconto.HasValue ? p.Valor_Desconto.Value.ToString("N2") : null
            }).ToList();

            retorno.Material_Os = obj.MaterialOSPlano.Select(p => new Models.MaterialosCadastroModel
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Descricao = p.Descricao
            }).ToList();

            foreach (var item in obj.Config_NF_PlanoProduto.Where(p => !p.Instalacao))
            {
                var objItem = new PlanoProdutoConfigNfCadastroModel();

                objItem.Id = item.Id;
                objItem.ModeloNotaFiscal = item.ModeloNotaFiscal;
                var descModelo = String.Empty;
                if (item.ModeloNotaFiscal != null)
                {
                    switch (item.ModeloNotaFiscal.Value)
                    {
                        case Modelo_NotaFiscal.Modelo13:
                            descModelo = "Modelo 13";
                            break;
                        case Modelo_NotaFiscal.Modelo21:
                            descModelo = "Modelo 21";
                            break;
                        case Modelo_NotaFiscal.Modelo22:
                            descModelo = "Modelo 22";
                            break;
                        case Modelo_NotaFiscal.Modelo55:
                            descModelo = "Modelo 55";
                            break;
                        case Modelo_NotaFiscal.NotaFiscalDebito:
                            descModelo = "Nota Fiscal de Débito";
                            break;
                        case Modelo_NotaFiscal.NotaFiscal62:
                            descModelo = "Nota Fiscal 62";
                            break;
                        default:
                            descModelo = String.Empty;
                            break;
                    }
                }
                //objItem.DescModeloNotaFiscal = item.ModeloNotaFiscal != null ? HtmlEnumExtensions.GetEnumDescription<Modelo_NotaFiscal>(item.ModeloNotaFiscal.Value) : "";
                objItem.DescModeloNotaFiscal = descModelo;

                objItem.IdClassificacaoTributaria = item.Configuracao_Tributaria != null ? item.Configuracao_Tributaria?.Id : (Int64?)null;
                objItem.DescClassificacaoTributaria = item.Configuracao_Tributaria != null ? item.Configuracao_Tributaria?.Descricao : "";
                objItem.ValorDiscriminado = item.ValorDiscriminado.HasValue ? item.ValorDiscriminado?.ToString("N2") : (0).ToString("N2");
                objItem.PercentualDiscriminado = item.PercentualDiscriminado.HasValue ? item.PercentualDiscriminado?.ToString() + "%" : "%";
                objItem.Id_Produtos_Comp = item.ProdutosComp != null ? item.ProdutosComp?.Id : (Int64?)null;
                objItem.DescProduto = item.ProdutosComp != null ? item.ProdutosComp?.Descricao : "";
                objItem.ValorDiscriminadoNf = item.ValorDiscriminadoNf.HasValue ? item.ValorDiscriminadoNf?.ToString("N2") : (0).ToString("N2");
                objItem.Id_Empresa = item.Empresa != null ? item.Empresa?.Id : (Int64?)null;
                objItem.DescEmpresa = item.Empresa != null ? item.Empresa?.Nome : "";
                objItem.Id_Filial = item.Filial != null ? item.Filial?.Id : (Int64?)null;
                objItem.DescFilial = item.Filial != null ? item.Filial?.Nome : "";

                retorno.ConfiguracaoNF.Add(objItem);
            }

            retorno.ConfiguracaoNFInstalacao = obj.Config_NF_PlanoProduto.Where(p => p.Instalacao).Select(p => new PlanoProdutoConfigNfCadastroModel
            {
                Id = p.Id,
                ModeloNotaFiscal = p.ModeloNotaFiscal,
                DescModeloNotaFiscal = HtmlEnumExtensions.GetEnumDescription(p.ModeloNotaFiscal),
                DescEmpresa = p.Empresa?.Nome ?? p.Empresa?.Razao_Social,
                DescFilial = p.Filial?.Nome ?? p.Filial?.Razao_Social,
                IdClassificacaoTributaria = p.Configuracao_Tributaria?.Id,
                DescClassificacaoTributaria = p.Configuracao_Tributaria?.Descricao ?? "",
                PercentualDiscriminado = p.PercentualDiscriminado.HasValue ? p.PercentualDiscriminado?.ToString() + "%" : "",
                ValorDiscriminado = p.ValorDiscriminado.HasValue ? p.ValorDiscriminado?.ToString("N2") : (0).ToString("N2")
            }).ToList();

              
            retorno.PlanoProdutosComp = obj.Plano_Produto_Comp.Select(p => new PlanoProdutosCompCadastroModel
            {
                Id = p.Id,
                IdProdutosComp = p.ProdutosComp.Id,
                DescricaoProdutosComp = p.ProdutosComp.Descricao,
                ValorFixo = p.ValorFixo.HasValue ? p.ValorFixo?.ToString("N2") : (0).ToString("N2"),
                ValorPercentual = p.ValorPercentual.HasValue ? FormatValorPercentual(p.ValorPercentual.Value) : "%",
                ValorTotal = p.ValorTotal.HasValue ? p.ValorTotal?.ToString("N2") : (0).ToString("N2")
            }).ToList();

            retorno.ProRata = obj.ProRata;
            retorno.Pro_Rata_Cancelamento = obj.Pro_Rata_Cancelamento;
            retorno.Pro_Rata_Habilitacao = obj.Pro_Rata_Habilitacao;
            retorno.Pro_Rata_Interrupcao = obj.Pro_Rata_Interrupcao;
            return retorno;
        }

        public static string FormatValorPercentual(double valor)
        {
            if (valor >= 10)
            {
                valor = valor / 100;
            }

            return valor.ToString("N2", new CultureInfo("pt-BR")) + "%";
        }

        private async Task BindViewToObj(ISession session, Plano objBanco, PlanoCadastroModel view)
        {
            //objeto_banco= atributo_modelview
            //Bloco1 Documentacao
            objBanco.Id = view.Id;
            objBanco.UsaPlanoAdicional = view.UsaPlanoAdicional;
            objBanco.Descricao = view.Descricao;
            objBanco.Nome_Venda = view.Nome_Venda;
            objBanco.DetalhesPlano = view.Detalhes;
            objBanco.AutenticacaoRadius = view.AutenticacaoRadius;
            objBanco.VelocidadeNominal = view.VelocidadeNominal;
            objBanco.VelocidadeDownload = view.VelocidadeDownload;
            objBanco.VelocidadeUpload = view.VelocidadeUpload;
            objBanco.ValorPlano = !String.IsNullOrEmpty(view.ValorPlano) ? Convert.ToDouble(view.ValorPlano) : 0;
            objBanco.StatusInicial = view.StatusInicial;
            objBanco.UtilizaAtributoIPOE = view.AtributoIPOE;
            objBanco.Num_Dids = view.Num_Dids;
            objBanco.Integracao = view.Integracao;
            objBanco.FormaPagamentoCobranca = view.FormaPagamentoCobranca;
            objBanco.DiasBoleto = view.DiasBoleto;
            objBanco.Tipo = Tipo_Plano_Produto.Plano;//view.Tipo;
            objBanco.ProdutoQtdVariavel = view.ProdutoQtdVariavel;
            objBanco.QtdParcelas_ProdutoVariavel = view.QtdParcelas_ProdutoVariavel;
            objBanco.PossuiValorEntrada = view.PossuiValorEntrada;

            objBanco.Modo_Cobranca_Plano = view.ModoCobranca;
            objBanco.Cobranca_Recorrente = view.Cobranca_Recorrente;
            objBanco.Cobrar_Por_Telefonia = view.Cobrar_Por_Telefonia;
            objBanco.Formato_do_Plano = view.Formato_do_plano;
            objBanco.FormatoTarifaTelefonia = view.FormatoTarifaTelefonia;
            objBanco.UtilizaBloqueioDebito = view.BLOQUEIO_AUTO_DEBITO;
            //objBanco.UtilizaDesbloqueioConfianca = view.Desbloqueio_Conf;
            objBanco.Dias_bloqueio = view.QtdDiasbloqueados;
            //objBanco.QtdDiasdesbloqueados = view.dias_desbloq;
            //objBanco.FrequenciaDesbloqueio = view.Frequencia_Desbloqueio;
            objBanco.Tipo_Tecnologia = view.Tipo_Tecnologia;
            objBanco.Desconto = view.Desconto;
            objBanco.UtilizaDesconto = view.Utiliza_Desconto;
            objBanco.Plano_Comercializavel = view.Plano_Comercializavel;
            objBanco.Momento_Cobranca = view.Momento_Cobranca;
            objBanco.Dias_Vencimento = view.Dias_Vencimento;
            objBanco.Cobranca_Realizada = view.Cobranca_Realizada;
            objBanco.Modelo_Anatel = view.Modelo_Anatel;
            objBanco.Tecnologia_Anatel = view.Tecnologia_Anatel;
            objBanco.Tipo_Produto = view.Tipo_Produto;
            objBanco.Tipo_ProdutoScm = view.Tipo_ProdutoScm;
            objBanco.Meio_Acesso_Anatel = view.Meio_Acesso_Anatel;
            objBanco.Regime = view.Regime;
            objBanco.Modalidade_Anatel = view.Modalidade_Anatel;
            objBanco.Tarifa_Anatel = view.Tarifa_Anatel;

            using (var repoFormaPag = new Conexao<Forma_Pagamento>(session))
            {
                objBanco.FormaPagamentoCob = await repoFormaPag.selectFirst(p => p.Id == view.IdFormaPagamentoCob);
            }

            using (var repoAtributo_IPOE = new Conexao<Atributo_IPOE>(session))
            {
                objBanco.Atributo_IPOE = await repoAtributo_IPOE.selectFirst(p => p.Id == view.NomeIPOE);
            }

            using (var repoTecnologia = new Conexao<Tecnologia>(session))
            {
                objBanco.Tecnologia = await repoTecnologia.selectFirst(p => p.Id == view.Id_Tecnologia);
            }

            using (var repoRadius = new Conexao<Sinapse.Model.Radius>(session))
            {
                objBanco.Radius = await repoRadius.selectFirst(p => p.Id == view.Id_Radius);
            }

            //Bloco2 Documentacao
            using (var repoModelo_Recibo = new Conexao<Modelo_Recibo>(session))
            using (var repoConta_Financeira = new Conexao<Conta_Financeira>(session))
            {
                objBanco.CobrancaInstalacao = view.CobrancaInstalacao;
                objBanco.CobrancaInstalacaoEntrada = view.CobrancaInstalacaoEntrada;
                objBanco.ValorInstalacao = !String.IsNullOrEmpty(view.ValorInstalacao) ? Convert.ToDouble(view.ValorInstalacao) : 0;
                objBanco.ModoCobrancaInstalacao = view.ModoCobrancaInstalacao;
                objBanco.Modelo_Recibo_Instalacao = view.Id_ReciboInstalacaco.HasValue ? repoModelo_Recibo.selectFirst(p => p.Id == view.Id_ReciboInstalacaco.Value).Result : null;
                objBanco.QtdParcelas = view.QtdParcelas;
                objBanco.OSTrocaEndereco = view.OSTrocaEndereco;
                objBanco.Conta_FinanceiraInstalacao = view.Id_ContaFinanceiraInstalacao.HasValue ? repoConta_Financeira.selectFirst(p => p.Id == view.Id_ContaFinanceiraInstalacao).Result : null;
                objBanco.Conta_FinanceiraPlano = view.Id_ContaFinanceiraPlano.HasValue ? repoConta_Financeira.selectFirst(p => p.Id == view.Id_ContaFinanceiraPlano).Result : null;
            }

            //Bloco3 Documentacao
            objBanco.ReducaoVelocidade = view.ReducaoVelocidade;
            objBanco.QtdDiasAtraso = view.QtdDiasAtraso;
            objBanco.QtdDiasbloqueados = view.QtdDiasbloqueados;
            objBanco.VelocidadeDownloadReduzida = view.VelocidadeDownloadReduzida;
            objBanco.VelocidadeUploadReduzida = view.VelocidadeUploadReduzida;
            objBanco.AtivarValidade = view.AtivarValidade;
            objBanco.Validade = view.Validade;
            //objBanco.TesteGratis = view.TesteGratis;
            //objBanco.DiasTeste = view.DiasTeste;
            objBanco.ProRata = view.ProRata;
            objBanco.Pro_Rata_Habilitacao = view.Pro_Rata_Habilitacao;
            objBanco.Pro_Rata_Interrupcao = view.Pro_Rata_Interrupcao;
            objBanco.Pro_Rata_Cancelamento = view.Pro_Rata_Cancelamento;


            //Bloco4 Documentacao
            using (var repoModelo_Contrato = new Conexao<Modelo_Contrato>(session))
            using (var repoModelo_Recibo = new Conexao<Modelo_Recibo>(session))
            using (var repoCfop = new Conexao<Cfop>(session))
            {
                objBanco.EmitirContrato = view.EmitirContrato;
                objBanco.Modelo_Contrato = view.IdModeloContrato.HasValue ? repoModelo_Contrato.selectFirst(p => p.Id == view.IdModeloContrato.Value).Result : null;
                objBanco.FrequenciaCobranca = view.FrequenciaCobranca;
                objBanco.Momento_Cobranca_Plano = view.Momento_Cobranca_Plano;
                objBanco.Cobranca_Recorrente = view.Cobranca_Recorrente;
                objBanco.ComprovantePagamento = view.ComprovantePagamento;
                objBanco.ComprovantePagamentoFinanceiro = view.ComprovantePagamentoFinanceiro;
                objBanco.Modelo_Recibo = view.Id_Recibo.HasValue ? repoModelo_Recibo.selectFirst(p => p.Id == view.Id_Recibo.Value).Result : null;
                objBanco.Cfop = view.Id_Cfop.HasValue ? repoCfop.selectFirst(p => p.Id == view.Id_Cfop.Value).Result : null;
                objBanco.RetencaoImposto = view.RetecaoImposto;
                objBanco.Momento_Cobra_Instalacao = view.Momento_Cobra_Instalacao;
                //objBanco.ImpedirBloqueio_Automatico = view.ImpedirBloqueio_Automatico;
                //objBanco.ImpedirBloqueio_Sexta = view.ImpedirBloqueio_Sexta;
                //objBanco.ImpedirBloqueio_Sabado = view.ImpedirBloqueio_Sabado;
                //objBanco.ImpedirBloqueio_Domingo = view.ImpedirBloqueio_Domingo;
                //objBanco.ImpedirBloqueio_Feriados = view.ImpedirBloqueio_Feriados;
                objBanco.FormatoCobranca = view.FormatoCobranca;
            }
            //Bloco5 Documentacao

            //objBanco.AtribuirValorIntegral = view.AtribuirValorIntegral;
            //objBanco.DescontoAntecipado = view.DescontoAntecipado;
            //objBanco.QtdDiasVencto = view.QtdDiasVencto;
            //objBanco.PercentualDescontoAntecipado = String.IsNullOrEmpty(view.PercentualDescontoAntecipado) ? (Double?)null : Convert.ToDouble(view.PercentualDescontoAntecipado.Replace("%",""));
            //objBanco.ValorDescontoAntecipado = !String.IsNullOrEmpty(view.ValorDescontoAntecipado) ? Convert.ToDouble(view.ValorDescontoAntecipado) : 0;
            //objBanco.DescontoProgressivo = view.DescontoProgressivo;
            objBanco.MensalidadeInicial = view.MensalidadeInicial;
            objBanco.MensalidadeFinal = view.MensalidadeFinal;
            objBanco.PercentualDescontoProgressivo = String.IsNullOrEmpty(view.PercentualDescontoProgressivo) ? (Double?)null : Convert.ToDouble(view.PercentualDescontoProgressivo.Replace("%", ""));
            objBanco.ValorDescontoProgressivo = !String.IsNullOrEmpty(view.ValorDescontoProgressivo) ? Convert.ToDouble(view.ValorDescontoProgressivo) : 0;

            objBanco.Id_Tipo_Servico = view.Id_Tipo_Servico;
            objBanco.Id_Subtipo_Servico = view.Id_Subtipo_Servico;
            objBanco.Id_Tipo_ServicoOS = view.Id_Tipo_ServicoOS;
            objBanco.Id_Subtipo_ServicoOS = view.Id_Subtipo_ServicoOS;
            objBanco.OS_Mudanca_Plano = view.Os_Mudanca_Plano;
            objBanco.DefinirTipo_SubTipoOS = view.DefinirTipo_SubTipoOS;
            objBanco.Plano_Comercializavel = view.Plano_Comercializavel;
            objBanco.Momento_Cobranca = view.Momento_Cobranca;
            objBanco.Cobranca_Realizada = view.Cobranca_Realizada;
            objBanco.Dias_Vencimento = view.Dias_Vencimento;

            using (var repoFranquia = new Conexao<Franquia>(session))
            {
                objBanco.PossuiConsumoMinimo = view.PossuiConsumoMinimo;
                objBanco.PossuiFranquia = view.PossuiFranquia;
                objBanco.Franquia = repoFranquia.selectFirst(p => p.Id == view.IdFranquia).Result;
                objBanco.ConsumoMinimo = !String.IsNullOrEmpty(view.ConsumoMinimo) ? Convert.ToDouble(view.ConsumoMinimo) : (double?)null;
                objBanco.FormatoTarifaTelefonia = view.FormatoTarifaTelefonia;
                objBanco.IntragrupoGratuita = view.IntragrupoGratuita;
                objBanco.IntragrupoLcGratuita = view.IntragrupoLcGratuita;

                objBanco.HabilitarLc = view.HabilitarLc;
                objBanco.HabilitarLdn = view.HabilitarLdn;
                objBanco.HabilitarLdi = view.HabilitarLdi;
                objBanco.HabilitarVc1 = view.HabilitarVc1;
                objBanco.HabilitarVc2 = view.HabilitarVc2;
                objBanco.HabilitarVc3 = view.HabilitarVc3;
                objBanco.CobrarBina = view.CobrarBina;
                objBanco.CobrarBlacklist = view.CobrarBlacklist;
                objBanco.CobrarSigame = view.CobrarSigame;


                objBanco.IntragrupoGratuita = view.IntragrupoGratuita;
                objBanco.IntragrupoLcGratuita = view.IntragrupoLcGratuita;

                objBanco.CadenciaLc = view.CadenciaLc;
                objBanco.CadenciaLdn = view.CadenciaLdn;
                objBanco.CadenciaLdi = view.CadenciaLdi;
                objBanco.CadenciaVc1 = view.CadenciaVc1;
                objBanco.CadenciaVc2 = view.CadenciaVc2;
                objBanco.CadenciaVc3 = view.CadenciaVc3;

                objBanco.ValorLc = !String.IsNullOrEmpty(view.ValorLc) ? Convert.ToDouble(view.ValorLc) : (double?)null;
                objBanco.ValorLdn = !String.IsNullOrEmpty(view.ValorLdn) ? Convert.ToDouble(view.ValorLdn) : (double?)null;
                objBanco.ValorLdi = !String.IsNullOrEmpty(view.ValorLdi) ? Convert.ToDouble(view.ValorLdi) : (double?)null;
                objBanco.ValorVc1 = !String.IsNullOrEmpty(view.ValorVc1) ? Convert.ToDouble(view.ValorVc1) : (double?)null;
                objBanco.ValorVc2 = !String.IsNullOrEmpty(view.ValorVc2) ? Convert.ToDouble(view.ValorVc2) : (double?)null;
                objBanco.ValorVc3 = !String.IsNullOrEmpty(view.ValorVc3) ? Convert.ToDouble(view.ValorVc3) : (double?)null;
                objBanco.ValorSigame = !String.IsNullOrEmpty(view.ValorSigame) ? Convert.ToDouble(view.ValorSigame) : (double?)null;
                objBanco.ValorBlacklist = !String.IsNullOrEmpty(view.ValorBlacklist) ? Convert.ToDouble(view.ValorBlacklist) : (double?)null;
                objBanco.ValorBina = !String.IsNullOrEmpty(view.ValorBina) ? Convert.ToDouble(view.ValorBina) : (double?)null;
                objBanco.Utiliza_Integracao_Externa = view.Utiliza_Integraca_Externa;

                objBanco.CobrarBloqueioChamadaCobrar = view.CobrarBloqueioChamadaCobrar;
                objBanco.ValorBloqueioChamadaCobrar = !String.IsNullOrEmpty(view.ValorBloqueioChamadaCobrar) ? Convert.ToDouble(view.ValorBloqueioChamadaCobrar) : (double?)null;
                objBanco.BloqueioChamadaCobrar = view.BloqueioChamadaCobrar;
            }


            //Material OS
            List<MaterialOs> materialOsPlano = objBanco.MaterialOSPlano.ToList();
            foreach (var material in view.Material_Os.FindAll(p => p.Id == 0))
            {
                materialOsPlano.Add(new MaterialOs
                {
                    Codigo = material.Codigo,
                    Descricao = material.Descricao,
                    Plano = objBanco,
                    Produto = null
                });
            }
            objBanco.MaterialOSPlano = materialOsPlano;

            //Mensalidade
            List<Faixas_Mensalidades> mensalidades = objBanco.Faixas_Mensalidades.ToList();
            foreach (var mensalidade in view.Mensalidades.FindAll(p => p.Id == 0))
            {
                mensalidades.Add(new Faixas_Mensalidades
                {
                    Parcela_Fin = Convert.ToInt16(mensalidade.Parcela_Fin),
                    Parcela_Ini = Convert.ToInt16(mensalidade.Parcela_Ini),
                    Percentual_Desconto = String.IsNullOrEmpty(mensalidade.Percentual_Desconto) ? 0 : Convert.ToDouble(mensalidade.Percentual_Desconto.Replace("%", "")),
                    Valor_Desconto = Convert.ToDouble(mensalidade.Valor_Desconto),
                    Plano = objBanco
                });
            }
            objBanco.Faixas_Mensalidades = mensalidades;

            //Ativação de Sinal
            objBanco.BloqueioPoolIp = view.UtilizaBloqueioPoolIp;
            //Atributos Huawei
            objBanco.UtilizaQoSProfile = view.UtilizaQoSProfile;
            objBanco.IdQoSProfile = view.IdQoSProfile;
            objBanco.IdQoSProfileReduzido = view.IdQoSProfile_Reduzido;
            objBanco.BloqueioDomainName = view.UtilizaBloqueioDomainName;
            objBanco.IdDomainName = view.IdDomainName;

            //Atributos Juniper
            //objBanco.UtilizaPolicyName = view.UtilizaPolicyName;
            objBanco.IdPolicyNameDownload = view.IdPolicyNameDownload;
            objBanco.IdPolicyNameDownloadReduzido = view.IdPolicyNameDownload_Reduzido;
            objBanco.IdPolicyNameUpload = view.IdPolicyNameUpload;
            objBanco.IdPolicyNameUploadReduzido = view.IdPolicyNameUpload_Reduzido;
            objBanco.UtilizaPolicyNameIPv6 = view.UtilizaPolicyNameIPv6;

            objBanco.ProductId = view.ProductId;

            //Config NF
            using (var repoConfigTributaria = new Conexao<Configuracao_Tributaria>(session))
            using (var repoProdutosComp = new Conexao<ProdutosComp>(session))
            using (var repoEmpresa = new Conexao<Sinapse.Model.Empresa>(session))
            using (var repoFilial = new Conexao<Filial>(session))
            using (var repoConfig_NF_PlanoProduto = new Conexao<Config_NF_PlanoProduto>(session))
            using (var repoPlano_Produto_Comp = new Conexao<Plano_Produto_Comp>(session))
            {
                List<Config_NF_PlanoProduto> configs_nf = objBanco.Config_NF_PlanoProduto.ToList();
                foreach (var config in view.ConfiguracaoNF.FindAll(p => p.Id == 0))
                {
                    var vlrDiscriminado = !string.IsNullOrEmpty(config.ValorDiscriminado) ? config.ValorDiscriminado.Replace(".", ",") : "0";
                    var percentual = !string.IsNullOrEmpty(config.PercentualDiscriminado) ? config.PercentualDiscriminado.Replace(".", ",") : "0";
                    var vlrNf = !string.IsNullOrEmpty(config.ValorDiscriminadoNf) ? config.ValorDiscriminadoNf.Replace(".", ",") : "0";

                    configs_nf.Add(new Config_NF_PlanoProduto
                    {
                        ValorDiscriminado = Convert.ToDouble(vlrDiscriminado),
                        PercentualDiscriminado = Convert.ToDouble(percentual),
                        ValorDiscriminadoNf = Convert.ToDouble(vlrNf),
                        ModeloNotaFiscal = config?.ModeloNotaFiscal,
                        Configuracao_Tributaria = config.IdClassificacaoTributaria.HasValue ? await repoConfigTributaria.selectFirst(p => p.Id == config.IdClassificacaoTributaria) : null,
                        ProdutosComp = config.Id_Produtos_Comp.HasValue ? await repoProdutosComp.selectFirst(p => p.Id == config.Id_Produtos_Comp) : null,
                        Empresa = config.Id_Empresa.HasValue ? await repoEmpresa.selectFirst(p => p.Id == config.Id_Empresa) : null,
                        Filial = config.Id_Filial.HasValue ? await repoFilial.selectFirst(p => p.Id == config.Id_Filial) : null,
                        Plano = objBanco
                    });
                }

                foreach (var config in view.ConfiguracaoNF.FindAll(p => p.Id != 0))
                {
                    if (view.DuplicarPlano)
                    {
                        var obj = await repoConfig_NF_PlanoProduto.selectFirst(p => p.Id == config.Id);
                        configs_nf.Add(new Config_NF_PlanoProduto
                        {
                            ValorDiscriminado = obj?.ValorDiscriminado,
                            PercentualDiscriminado = obj?.PercentualDiscriminado,
                            ValorDiscriminadoNf = obj?.ValorDiscriminadoNf,
                            Configuracao_Tributaria = obj?.Configuracao_Tributaria,
                            ModeloNotaFiscal = obj?.ModeloNotaFiscal,
                            Plano = objBanco,
                            Empresa = obj?.Empresa,
                            Filial = obj?.Filial,
                            ProdutosComp = obj?.ProdutosComp,
                            Instalacao = obj.Instalacao
                        });
                    }
                    else
                    {
                        var vlrDiscriminado = !string.IsNullOrEmpty(config.ValorDiscriminado) ? config.ValorDiscriminado : "0";
                        var percentual = !string.IsNullOrEmpty(config.PercentualDiscriminado) ? config.PercentualDiscriminado.Replace(".", ",") : "0";
                        var vlrNf = !string.IsNullOrEmpty(config.ValorDiscriminadoNf) ? config.ValorDiscriminadoNf : "0";
                        var configNH = await repoConfig_NF_PlanoProduto.selectFirst(p => p.Id == config.Id);
                        configNH.ValorDiscriminado = Math.Round(Convert.ToDouble(vlrDiscriminado), 2);
                        configNH.PercentualDiscriminado = percentual.Contains("%") ? Convert.ToDouble(percentual.Replace("%", "")) : Convert.ToDouble(percentual);
                        configNH.ValorDiscriminadoNf = Math.Round(Convert.ToDouble(vlrNf), 2);
                        configNH.ModeloNotaFiscal = config?.ModeloNotaFiscal;
                        configNH.Configuracao_Tributaria = config.IdClassificacaoTributaria.HasValue ? await repoConfigTributaria.selectFirst(p => p.Id == config.IdClassificacaoTributaria) : null;
                        configNH.ProdutosComp = config.Id_Produtos_Comp.HasValue ? await repoProdutosComp.selectFirst(p => p.Id == config.Id_Produtos_Comp) : null;
                        configNH.Empresa = config.Id_Empresa.HasValue ? await repoEmpresa.selectFirst(p => p.Id == config.Id_Empresa) : null;
                        configNH.Filial = config.Id_Filial.HasValue ? await repoFilial.selectFirst(p => p.Id == config.Id_Filial) : null;
                        configNH.Plano = objBanco;
                    }
                }

                foreach (var config_instalacao in view.ConfiguracaoNFInstalacao.FindAll(p => p.Id == 0))
                {
                    Double? valorDiscriminado = (double?)null;
                    Double? valorPercentual = (double?)null;

                    if (String.IsNullOrEmpty(config_instalacao.ValorDiscriminado))
                    {
                        valorPercentual = !string.IsNullOrEmpty(config_instalacao.PercentualDiscriminado) ? Convert.ToDouble(config_instalacao.PercentualDiscriminado) : 0;

                        valorDiscriminado = valorPercentual != 0 ? ((objBanco.ValorInstalacao * valorPercentual) / 100) : null;
                    }
                    else
                    {
                        valorDiscriminado = Convert.ToDouble(config_instalacao.ValorDiscriminado) == 0 ? (double?)null : Convert.ToDouble(config_instalacao.ValorDiscriminado);
                        valorPercentual = !string.IsNullOrEmpty(config_instalacao.PercentualDiscriminado) ? Convert.ToDouble(config_instalacao.PercentualDiscriminado) : (double?)null;
                    }

                    configs_nf.Add(new Config_NF_PlanoProduto
                    {
                        Instalacao = true,
                        Empresa = config_instalacao.Id_Empresa.HasValue ? await repoEmpresa.selectFirst(p => p.Id == config_instalacao.Id_Empresa) : null,
                        Filial = config_instalacao.Id_Filial.HasValue ? await repoFilial.selectFirst(p => p.Id == config_instalacao.Id_Filial) : null,
                        ValorDiscriminado = valorDiscriminado,
                        PercentualDiscriminado = valorPercentual,
                        Configuracao_Tributaria = await repoConfigTributaria.selectFirst(p => p.Id == config_instalacao.IdClassificacaoTributaria),
                        ModeloNotaFiscal = config_instalacao.ModeloNotaFiscal,

                        Plano = objBanco
                    });
                }

                objBanco.Config_NF_PlanoProduto = configs_nf;

                objBanco.CodigoTipoPlanoIntegracao = view.CodigoTipoPlanoIntegracao;
                objBanco.CodigoIntegracao = view.CodigoIntegracao;
                objBanco.CodigoIntegracaoAuxiliar = view.CodigoIntegracaoAuxiliar;
                objBanco.ExclusivoCliente = view.ExclusivoCliente;


                var produtosComp = objBanco.Plano_Produto_Comp.ToList();
                if (!view.DuplicarPlano)
                {
                    foreach (var item in view.PlanoProdutosComp.FindAll(p => p.Id == 0))
                    {
                        var vlrFixo = !string.IsNullOrEmpty(item.ValorFixo) ? item.ValorFixo : "0";
                        var vlrPercentual = !string.IsNullOrEmpty(item.ValorPercentual) ? item.ValorPercentual.Replace("%", "") : "0";
                        var vlrTotal = !string.IsNullOrEmpty(item.ValorTotal) ? item.ValorTotal : "0"; ;

                        produtosComp.Add(new Plano_Produto_Comp
                        {
                            ValorFixo = double.Parse(vlrFixo),
                            ValorPercentual = double.Parse(vlrPercentual),
                            ValorTotal = double.Parse(vlrTotal),

                            ProdutosComp = await repoProdutosComp.selectFirst(x => x.Id == item.IdProdutosComp),
                            Plano = objBanco
                        });
                    }
                }
                else
                {
                    foreach (var item in view.PlanoProdutosComp)
                    {
                        var planoProdutoComp = await repoPlano_Produto_Comp.selectFirst(x => x.Id == item.Id);
                        produtosComp.Add(new Plano_Produto_Comp
                        {
                            ValorFixo = planoProdutoComp.ValorFixo,
                            ValorPercentual = planoProdutoComp.ValorPercentual,
                            ValorTotal = planoProdutoComp.ValorTotal,

                            ProdutosComp = await repoProdutosComp.selectFirst(x => x.Id == planoProdutoComp.ProdutosComp.Id),
                            Plano = objBanco
                        });
                    }
                }

                objBanco.Plano_Produto_Comp = produtosComp;
            }

            objBanco.UtilizaVendaAgroup = view.UtilizaVendaAgroup;
            objBanco.ValorVendaAgrupada = !String.IsNullOrEmpty(view.ValorVendaAgrupada) ? double.Parse(view.ValorVendaAgrupada) : double.Parse("0.0");
        }

        

        [HttpPost()]
        [Route("api/Planoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PLANO SET SITUACAOPLANO = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Planoapi/excluirlista")]
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

                        var planos = value.objs.FindAll(p => p.tipo == "Plano");

                        if (planos.Count > 0)
                        {
                            String _ids = $"({String.Join(",", planos)})";

                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_PLANO WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }

                        var produtos = value.objs.FindAll(p => p.tipo == "Produto");

                        if (planos.Count > 0)
                        {
                            String _ids = $"({String.Join(",", produtos)})";

                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_PRODUTO WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);
                        }
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

        [HttpGet()]
        [Route("api/Planoapi/FindById")]
        public async Task<GetApiRetorno<PlanoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<PlanoCadastroModel> retorno = new GetApiRetorno<PlanoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Plano>(session))
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
        [Route("api/Planoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<PlanoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Plano modelo = null;
                PlanoProdutoAniel modeloAnielT5 = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Plano>(session))
                    using (var repositoryT5 = new Conexao<PlanoProdutoAniel>(sessionT5))
                    using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                    using (var repoPlanoProdutoAdicional = new Conexao<PlanoProdutoAdicional>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Plano
                            {
                                SituacaoPlano = Situacao_plano.Ativo,
                                Codigo = Convert.ToString((await repository.MaxString("CODIGO", "TB_PLANO")) + 1),
                                UsuarioCriacao = value.Usuario,
                                FormaPagamentoCobranca = value.Objeto.FormaPagamentoCobranca,
                                DiasBoleto = value.Objeto.DiasBoleto
                            };
                        }
                        else
                        {

                            modeloAnielT5 = await repositoryT5.selectFirst(p => p.Id == value.Objeto.Id);

                            if (modeloAnielT5 != null)
                            {
                                modeloAnielT5.UsuarioAlteracao = value.Usuario;
                                modeloAnielT5.Descricao = value.Objeto.Descricao;
                            }

                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.SituacaoPlano = value.Objeto.SituacaoPlano ? Situacao_plano.Ativo : Situacao_plano.Inativo;
                        }
                        var novoRegistro = modelo.Id == 0;
                        await BindViewToObj(session, modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);

                        var listPlanProductAdc = await repoPlanoProdutoAdicional.select(p => p.Id_Plano.Id == modelo.Id);

                        foreach (var item in value.Objeto.PlanosProdutosAdicionais)
                        {
                            if (listPlanProductAdc != null)
                            {
                                var hasAny = listPlanProductAdc.Any(x => x.Id_Plano_Produto_Adicional == item.Id);

                                if (hasAny)
                                    continue;
                            }

                            PlanoProdutoAdicional planoAdicional = new PlanoProdutoAdicional
                            {
                                Descricao = item.Descricao,
                                Id_Plano_Produto_Adicional = item.Id,
                                Id_Plano = modelo
                            };

                            await repoPlanoProdutoAdicional.SaveOrUpdate(planoAdicional);
                        }


                        if (modeloAnielT5 != null)
                            await repositoryT5.SaveOrUpdate(modeloAnielT5);

                        var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                        if (integracao != null)
                        {
                            if (value.Objeto.Id == 0)
                            {
                                var response = await Integracao_Cadastro_Plano(modelo);
                                modelo.CodigoAuxiliar = Convert.ToString(response?.id);
                                await repository.SaveOrUpdate(modelo);
                            }
                            else if (String.IsNullOrEmpty(modelo.CodigoAuxiliar) == false)
                            {
                                await Integracao_Editar_Plano(modelo);
                                if (modelo.PossuiFranquia)
                                {
                                    if (String.IsNullOrEmpty(modelo.CodigoAuxiliarFranquia))
                                    {
                                        using (var franquiaapi = new FranquiaApiController())
                                        {
                                            var resultado = await franquiaapi.Integracao_Cadastro_Franquia(modelo.Franquia, modelo);
                                            modelo.CodigoAuxiliarFranquia = resultado.result?.id;
                                            await repository.SaveOrUpdate(modelo);
                                        }
                                    }
                                    else if (String.IsNullOrEmpty(modelo.CodigoAuxiliarFranquia) == false)
                                    {
                                        using (var franquiaapi = new FranquiaApiController())
                                        {
                                            var resultado = await franquiaapi.Integracao_Editar_Franquia(modelo.Franquia, modelo);
                                        }
                                    }
                                }
                            }
                        }

                        if (!novoRegistro)
                        {
                            try
                            {
                                if (modelo.AutenticacaoRadius)
                                {
                                    using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                                    using (var api = new IntegracaoRadiusApiController())
                                    {
                                        Model.nhibernate.LoginRadius loginAlias = null;
                                        Cliente_PlanoProduto planoProdutoAlias = null;
                                        Plano planoAlias = null;

                                        var listLogin = (await session
                                                                .QueryOver(() => loginAlias)
                                                                .JoinAlias(() => loginAlias.Cliente_PlanoProduto, () => planoProdutoAlias)
                                                                .JoinAlias(() => planoProdutoAlias.Plano, () => planoAlias)
                                                                .Where(p => planoAlias.Id == modelo.Id)
                                                                .ListAsync()).ToList();

                                        foreach (var login in listLogin)
                                        {
                                            await api.EditarPlano(new PlanoProdutoRadiusRequest
                                            {
                                                IdClientePlanoProduto = login.Cliente_PlanoProduto.Id
                                            });
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CONSTANTES.log4Net.Error(ex.Message, ex);
                            }
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

        [HttpPost()]
        [Route("api/PlanoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFromPlano = new StringBuilder("FROM TB_PLANO A ")
                        .AppendLine("LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ")
                        .AppendLine("ON(B.ID_PLANO = A.ID)");

                    #region Filtros

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"SITUACAOPLANO = 1 AND 0 = (SELECT COUNT(ID) FROM TB_ONU WHERE ID_CLIENTE_PLANO_PRODUTO = B.ID) ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"UPPER(B.ID_CLIENTE) = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 20)}%' OR " +
                                       $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFromPlano.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFromPlano.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    #endregion

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT A.ID, A.CODIGO, A.DESCRICAO, A.VELOCIDADEDOWNLOAD, A.VELOCIDADEUPLOAD ");
                        sql.AppendLine(sqlFromPlano.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new PlanoComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Download = Convert.ToString(reader["VELOCIDADEDOWNLOAD"]),
                                    Upload = Convert.ToString(reader["VELOCIDADEUPLOAD"])
                                };

                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/PlanoApi/GetComboTelefoniaVenda")]
        public async Task<ObjCombo<long>> GetComboTelefoniaVenda(RequestObjComboPai<long?, long> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                //Filtros 
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_CLIENTE_PLANO_PRODUTO PLANO_PROD ");
                sqlFrom.AppendLine(" INNER JOIN TB_PLANO PLANO ");
                sqlFrom.AppendLine(" ON(PLANO.ID = PLANO_PROD.ID_PLANO) ");

                List<String> condicoes = new List<String>();

                condicoes.Add($"PLANO_PROD.ID_VENDA = {request.idpai}");
                condicoes.Add("(PLANO.TIPO_TECNOLOGIA = 1 OR PLANO.TIPO_TECNOLOGIA = 4)");

                if (request.id.HasValue)
                {
                    condicoes.Add($"PLANO.ID = {request.id}");
                }

                if (request != null && !String.IsNullOrEmpty(request.q))
                {
                    condicoes.Add($"( UPPER(PLANO.CODIGO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 20)}%' OR " +
                                   $" UPPER(PLANO.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
                }

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(PLANO.ID) AS CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            retorno.total_count = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine(" SELECT PLANO.ID, PLANO.DESCRICAO ");
                    sql.AppendLine(sqlFrom.ToString());
                    sql.AppendLine(" ORDER BY UPPER(PLANO.DESCRICAO)");

                    using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ObjComboResult<long>
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Text = Convert.ToString(reader["DESCRICAO"])
                            };

                            retorno.results.Add(obj);
                        }
                    }
                }
            }

            return retorno;
        }

        [Route("api/Planoapi/ExcluirMensalidade")]
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

        [Route("api/Planoapi/ExcluirConfigNF")]
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

        #region [ExcluirMaterialOS_PlanoProduto]
        [Route("api/Planoapi/ExcluirMaterialOS_PlanoProduto")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirMaterialOS_PlanoProduto(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<MaterialOs>(session))
                    {

                        MaterialOs modelo = null;
                        if (value > 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();
                            if (modelo != null)
                            {
                                await repository.Remove(modelo);

                                retorno.IdObjeto = modelo.Id;
                                retorno.Ok = true;
                            }
                            else
                            {
                                retorno.IdObjeto = 0;
                                retorno.Ok = false;
                                retorno.Mensagem = RsBase.NAOENCONTRADO;
                            }
                        }
                        else
                        {
                            retorno.IdObjeto = 0;
                            retorno.Ok = false;
                            retorno.Mensagem = RsBase.NAOENCONTRADO;
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
        #endregion

        [Route("api/Planoapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Plano>(session))
                    {

                        Plano modelo = null;
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

        [HttpGet()]
        [Route("api/PlanoApi/GetListDetalhes")]
        public GetApiRetorno<PlanoDetalhesModels> GetListDetalhes(Int64 Id_Plano, Int64? Id_Campanha = null)
        {
            GetApiRetorno<PlanoDetalhesModels> retorno = new GetApiRetorno<PlanoDetalhesModels>
            {
                Objeto = new PlanoDetalhesModels()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Plano>(session))
            using (var repositoryContrato = new Conexao<Modelo_Contrato>(session))
            using (var repoPlanoAdicional = new Conexao<PlanoProdutoAdicional>(session))
            using (var repoCamapanha = new Conexao<Campanha_PlanoProduto>(session))
            {
                try
                {
                    Campanha_PlanoProduto campanha = null;
                    if (Id_Campanha > 0)
                        campanha = repoCamapanha.selectFirst(p => p.Campanha.Id == Id_Campanha && p.Id_Plano_Produto == Id_Plano).Result;

                    Plano plan = repository.selectFirst(p => p.Id == Id_Plano).Result;

                    if (plan != null)
                    {
                        PlanoDetalhesModels planoDetalhes = new PlanoDetalhesModels();

                        planoDetalhes.ValorTotalSemInstalacao = Convert.ToString(String.Format("{0:N2}", plan.ValorPlano));
                        planoDetalhes.ValorTotalComInstalacao = Convert.ToString(String.Format("{0:N2}", Math.Round(plan.ValorPlano + plan.ValorInstalacao)));
                        planoDetalhes.ValorPlano = campanha != null ? Convert.ToString(String.Format("{0:N2}", campanha.Valor_Plano_Produto)) : Convert.ToString(String.Format("{0:N2}", plan.ValorPlano));
                        planoDetalhes.ValorPlanoProdutoFinal = campanha != null ? Convert.ToString(String.Format("{0:N2}", campanha.Valor_Plano_Produto)) : Convert.ToString(String.Format("{0:N2}", plan.ValorPlano));
                        planoDetalhes.ValorInstalacao = campanha != null ? Convert.ToString(String.Format("{0:N2}", campanha.Valor_Instalacao)) : Convert.ToString(String.Format("{0:N2}", plan.ValorInstalacao));
                        planoDetalhes.ValorInstalacaoFinal = campanha != null ? Convert.ToString(String.Format("{0:N2}", campanha.Valor_Instalacao)) : Convert.ToString(String.Format("{0:N2}", plan.ValorInstalacaoFinal));

                        planoDetalhes.Periodo_Carencia = (plan.Modelo_Contrato?.Tem_Carencia ?? false) ? "Sim" : "Não";
                        planoDetalhes.Numero_Parcelas = campanha != null ? campanha?.Parcela_Instalacao: plan.QtdParcelas;
                        planoDetalhes.Nome_NAS = plan.Servidor_NAS?.Nome;
                        planoDetalhes.IP = plan.Servidor_NAS?.Ip;

                        planoDetalhes.Qtde_Dias = plan.Modelo_Contrato?.DiasTeste != null ? plan.Modelo_Contrato.DiasTeste.Value : 0;
                        planoDetalhes.Multa_Carencia_Periodo = Convert.ToString(plan.Modelo_Contrato?.Periodo_Carencia);
                        planoDetalhes.Tem_Reajuste = plan.Modelo_Contrato?.Tem_Reajuste == true ? RsBase.SIM : RsBase.NAO;
                        planoDetalhes.Frequencia_Reajuste = plan.Modelo_Contrato?.Frequencia_Reajuste != null ? HtmlEnumExtensions.GetEnumDescription(plan.Modelo_Contrato.Frequencia_Reajuste) :
                        "Não possui frequencia Vinculada";

                        //Carencia - Fidelidade
                        planoDetalhes.Multa_Carencia_Reais = plan.Modelo_Contrato?.Multa_Carencia != null ?
                        Convert.ToString(plan.Modelo_Contrato.Multa_Carencia.Value) :
                        Convert.ToString(0);


                        planoDetalhes.Cancelar_Contrato_Automatico_Fim_Carencia = plan.Modelo_Contrato?.Cancela_Contrato_Automatico_Final_Carencia == true ?
                        RsBase.SIM : RsBase.NAO;

                        planoDetalhes.Renovar_Carencia_Automaticamente = plan.Modelo_Contrato?.Renova_Carencia_Automaticamente == Renova_FidelidadeEnum.Sim ?
                        RsBase.SIM : RsBase.NAO;

                        planoDetalhes.VelocidadeNominal = plan.VelocidadeNominal;
                        planoDetalhes.VelocidadeDownload = plan.VelocidadeDownload;
                        planoDetalhes.VelocidadeUpload = plan.VelocidadeUpload;
                        planoDetalhes.Id_Tecnologia = plan.Tecnologia?.Id;

                        planoDetalhes.VelocidadeDownloadReduzida = plan.VelocidadeDownloadReduzida;
                        planoDetalhes.VelocidadeUploadReduzida = plan.VelocidadeUploadReduzida;
                        planoDetalhes.AutenticacaoRadius = plan.AutenticacaoRadius ? "Sim" : "Não";
                        planoDetalhes.Detalhes = plan.DetalhesPlano;

                        var retornoPlanosAdicionais = new List<PlanosProdutosAdicionaisCadastroModel>();
                        if (plan.UsaPlanoAdicional)
                        {
                            var listPlanProductAdc = repoPlanoAdicional.select(p => p.Id_Plano.Id == plan.Id).Result;
                            retornoPlanosAdicionais = listPlanProductAdc.Select(p => new PlanosProdutosAdicionaisCadastroModel
                            {
                                Id = p.Id_Plano_Produto_Adicional,
                                Descricao = p.Descricao,
                            }).ToList();
                        }
                        planoDetalhes.PlanosProdutosAdicionais = retornoPlanosAdicionais;

                        retorno.Objeto = planoDetalhes;
                    }
                    else
                    {
                        retorno.Objeto = new PlanoDetalhesModels();
                    }

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
        private async Task<PlanoIntegracaoResponse> Integracao_Cadastro_Plano(Plano obj)
        {
            PlanoIntegracaoSaperxService integracao = new PlanoIntegracaoSaperxService();
            return integracao.Cadastrar(obj);

        }

        private async Task<PlanoIntegracaoResponse> Integracao_Editar_Plano(Plano obj)
        {
            PlanoIntegracaoSaperxService integracao = new PlanoIntegracaoSaperxService();
            return integracao.Atualizar(obj);

        }

        [HttpPost()]
        [Route("api/PlanoApi/RelatorioAnatel")]
        public async Task<DataTableResponse<dynamic>> RelatorioAnatel(RelatorioAnatel request)
        {

            DateTime dataInicio = new DateTime(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1, 1);
            DateTime dataFim = new DateTime(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1, DateTime.DaysInMonth(Convert.ToInt32(request.Ano), Convert.ToInt32(request.Mes) + 1));


            List<dynamic> retorno = new List<dynamic>();
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    parametros.Add("DATA_INI", dataInicio);
                    parametros.Add("DATA_FIM", dataFim);

                    sql.AppendLine(" SELECT E.NOME,E.CNPJ,H.CODIGO_IBGE,C.TIPO_CLIENTE,I.TIPO_ZONA AS TIPOATENDIMENTO,A.MEIO_ACESSO_ANATEL AS TIPO_MEIO,A.TIPO_PRODUTO_ANATEL,A.TIPO_PRODUTO_ANATELSMP ,	A.TECNOLOGIA_ANATEL, " +
                                     "A.VELOCIDADENOMINAL,COUNT(B.ID_PLANO) AS QTDACESSO ,G.ENDERECO_UF AS UF,G.COD_AREA,A.TARIFA_ANATEL,B.LOCALIDADE_ANATEL," +
                                     "SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL <> 2 AND B.STATUS = 2 THEN 1 ELSE 0 END) AS ESTACAOPADRAOHABILITADA,SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL <> 2 AND B.STATUS = 2 AND B.DATAHORAALTERACAO IS NOT NULL  THEN 1 ELSE 0 END) AS ESTACAOPADRAOREAABILITADA,SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL <> 2  AND B.STATUS in (3,4,5) THEN 1 ELSE 0 END) AS ESTACAOPADRAOREABILITADA, " +
                                     "SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL = 2 AND B.STATUS = 2 THEN 1 ELSE 0 END)AS ESTACAOM2MHABILITADA,SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL = 2  AND B.STATUS in (3,4,5) THEN 1 ELSE 0 END)AS ESTACAOM2MDESABILITADA,SUM(CASE WHEN A.TIPO_PRODUTO_ANATEL = 2  AND B.STATUS = 2 THEN 1 ELSE 0 END)AS ESTACAOM2MREABILITADA,A.FORMATOCOBRANCA,C.TIPOPESSOA,A.TIPO_TECNOLOGIA,B.COD_LOCALIDADE   ");

                    sql.AppendLine(" FROM TB_PLANO A ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO B ");
                    sql.AppendLine(" ON B.ID_PLANO = A.ID ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE C ");
                    sql.AppendLine(" ON C.ID = B.ID_CLIENTE ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ");
                    sql.AppendLine(" ON D.NUM_FRE  = C.USUARIOCRIACAO  ");
                    sql.AppendLine(" LEFT JOIN TB_FILIAL E ");
                    sql.AppendLine(" ON E.ID = D.ID_FILIAL ");
                    sql.AppendLine(" LEFT JOIN TB_EMPRESA F ");
                    sql.AppendLine(" ON F.CODEMPRESA = E.ID_EMPRESA ");
                    sql.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE G ");
                    sql.AppendLine(" ON G.ID = C.ID_ENDERECO_PRINCIPAL ");
                    sql.AppendLine("LEFT JOIN TB_CIDADE H ");
                    sql.AppendLine("ON H.ID = G.ID_CIDADE ");
                    sql.AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO I ");
                    sql.AppendLine("ON I.ID =G.ID_ENDERECO_LOGRADOURO  ");


                    sql.AppendLine(" WHERE A.ID IS NOT NULL  ");




                    //if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    //{
                    //    if (request.Tipo_Data == "agendamento")
                    //        sql.AppendLine("AND (D.DATA_MAXIMA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                    //    else
                    //        sql.AppendLine("AND (D.DATA_PROD BETWEEN @DATA_INICIO AND @DATA_FIM)");

                    //    parametros.Add("@DATA_INICIO", Convert.ToDateTime(request.Data_Inicio).ToString("dd/MM/yyyy 00:00"));
                    //    parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).ToString("dd/MM/yyyy 23:59"));
                    //}

                    if (!String.IsNullOrEmpty(request.Empresa))
                    {
                        sql.AppendLine("AND (F.ID = @EMPRESA)");
                        parametros.Add("@EMPRESA", request.Empresa);
                    }

                    if (!String.IsNullOrEmpty(request.Filial))
                    {
                        sql.AppendLine("AND (E.ID = @ID)");
                        parametros.Add("@ID", Convert.ToInt32(request.Empresa));
                    }

                    if (!String.IsNullOrEmpty(request.Numero_Fistel))
                    {
                        sql.AppendLine("AND (E.NUMERO_FISTEL = @NUMERO_FISTEL)");
                        parametros.Add("@NUMERO_FISTEL", request.Numero_Fistel);
                    }

                    if (request.SCM)
                    {
                        sql.AppendLine(" AND A.MODELO_ANATEL = 1");
                    }

                    if (request.SMP)
                    {
                        sql.AppendLine(" AND A.MODELO_ANATEL = 2");
                    }

                    if (request.STFCPublico)
                    {
                        sql.AppendLine(" AND A.REGIME = 1");
                    }
                    if (request.STFCPubPriv)
                    {
                        sql.AppendLine(" AND A.REGIME = 2");
                    }

                    if (request.TV_Assinatura)
                    {
                        sql.AppendLine(" AND A.MODELO_ANATEL = 4");
                    }

                    //sql.AppendLine("AND CAST(B.DATAHORACRIACAO AS date)  BETWEEN @DATA_INI AND @DATA_FIM");

                    sql.AppendLine(" GROUP BY E.CNPJ,H.CODIGO_IBGE,C.TIPO_CLIENTE,C.ID,I.TIPO_ZONA,A.MEIO_ACESSO_ANATEL,A.TIPO_PRODUTO_ANATEL,A.TECNOLOGIA_ANATEL,A.VELOCIDADENOMINAL,A.TARIFA_ANATEL,B.LOCALIDADE_ANATEL,B.ID_CLIENTE,G.ENDERECO_UF,G.COD_AREA,ID_PLANO,E.NOME,A.FORMATOCOBRANCA,C.TIPOPESSOA,A.TIPO_TECNOLOGIA,B.COD_LOCALIDADE,A.TIPO_PRODUTO_ANATELSMP   ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new
                            {

                                CNPJ = Convert.ToString(reader["CNPJ"]),
                                // Numero_Fistel = Convert.ToString(reader["CODIGO_IBGE"]),
                                Empresa = Convert.ToString(reader["NOME"]),
                                Codigo_IBGE = Convert.ToString(reader["CODIGO_IBGE"]),
                                Tipo_Cliente = reader["TIPO_CLIENTE"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])) : String.Empty,
                                Tipo_Pessoa = reader["TIPOPESSOA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])) : String.Empty,
                                Tipo_Cobranca = reader["FORMATOCOBRANCA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Formato_Cobranca>(Convert.ToString(reader["FORMATOCOBRANCA"])) : String.Empty,
                                Tipo_Atendimento = reader["TIPOATENDIMENTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Atendimento_Zona>(Convert.ToString(reader["TIPOATENDIMENTO"])) : String.Empty,
                                Tipo_Meio = reader["TIPO_MEIO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Meio_Acesso_Anatel>(Convert.ToString(reader["TIPO_MEIO"])) : String.Empty,
                                Tipo_Produto = reader["TIPO_PRODUTO_ANATEL"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Produto_Anatel>(Convert.ToString(reader["TIPO_PRODUTO_ANATEL"])) : String.Empty,
                                Tipo_ProdutoSMP = reader["TIPO_PRODUTO_ANATELSMP"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Produto_AnatelSCM>(Convert.ToString(reader["TIPO_PRODUTO_ANATELSMP"])) : String.Empty,
                                Tecnologia = reader["TECNOLOGIA_ANATEL"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tecnologia_Anatel>(Convert.ToString(reader["TECNOLOGIA_ANATEL"])) : String.Empty,
                                Velocidade = Convert.ToString(reader["VELOCIDADENOMINAL"]),
                                Qtd_Acesso = Convert.ToInt32(reader["QTDACESSO"]),
                                Uf = Convert.ToString(reader["UF"]),
                                Area = Convert.ToString(reader["COD_AREA"]),
                                Tarifa_Basica = reader["TARIFA_ANATEL"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tarifa_Anatel>(Convert.ToString(reader["TARIFA_ANATEL"])) : String.Empty,
                                Localidade = reader["LOCALIDADE_ANATEL"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Localidade_Anatel>(Convert.ToString(reader["LOCALIDADE_ANATEL"])) : String.Empty,
                                Estacao_Padrao = Convert.ToInt32(reader["ESTACAOPADRAOhABILITADA"]),
                                Estacao_PadraoRe = Convert.ToInt32(reader["ESTACAOPADRAOREAABILITADA"]),
                                Estacao_PadraoDe = Convert.ToInt32(reader["ESTACAOPADRAOREABILITADA"]),
                                Estacao_M2m = Convert.ToInt32(reader["ESTACAOM2MHABILITADA"]),
                                Estacao_M2mRe = Convert.ToInt32(reader["ESTACAOM2MREABILITADA"]),
                                Estacao_M2mDe = Convert.ToInt32(reader["ESTACAOM2MDESABILITADA"]),
                                Cod_Localidade = Convert.ToString(reader["COD_LOCALIDADE"]),
                                Ano = request.Ano,
                                Mes = request.Mes,
                                Tipo_Tecnologia = reader["TIPO_TECNOLOGIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Tecnologia>(Convert.ToString(reader["TIPO_TECNOLOGIA"])) : String.Empty,



                            });
                        }
                    }

                }
            }

            return new DataTableResponse<dynamic>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }


        [HttpPost()]
        [Route("api/PlanoApi/NFDebitoGetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_NFDEBITO A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }
                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 40)}%')");
                    }
                    condicoes.Add("A.STATUS = 1");

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Planoapi/VerificaProdutoQtdVariavelAssociadoCombo")]
        public async Task<GetApiRetorno<PlanoCadastroModel>> VerificaProdutoQtdVariavelAssociadoCombo(Int64 IdPlano)
        {
            GetApiRetorno<PlanoCadastroModel> retorno = new GetApiRetorno<PlanoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<ComboPlanoProduto>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Plano.Id == IdPlano);
                    if (lista != null && lista.Count() > 0)
                    {
                        retorno.Ok = true;
                        retorno.Mensagem = "Não é possível adicionar quantidade variável para plano vinculado a um combo.";
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }
        
        [HttpGet()]
        [Route("api/PlanoApi/PlanoDisponivel/{idPlano}/{cpf}")]
        public async Task<bool> PlanoDisponivel(long idPlano, string cpf)
        {
            var retorno = true;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoIntegracoes = new Conexao<IntegracoesExternas>(session))
            {
                var plano = await repoPlano.selectFirst(x => x.Id == idPlano);

                var integracaoHub = await repoIntegracoes.selectFirst(x => x.IntegracaoExterna == IntegracaoExterna.MK_SOLUTION && x.TipoIntegracao == EnumTipoIntegracao.EXCLUSIVIDADE_PLANO);

                var alcans = new AlcansRepository(integracaoHub.Usuario, integracaoHub.Senha, integracaoHub.UrlApi);

                var responseAlcans = await alcans.ContratosDisponiveisAllhub(new ContratosDisponiveisRequest
                {
                    accessPlans = new List<int> { Convert.ToInt32(plano.CodigoIntegracao) },
                    document = cpf
                });

                if (responseAlcans != null && responseAlcans.Any())
                {
                    var planoResponseAlcans = responseAlcans.FirstOrDefault(x => x.accessPlan.ToString() == plano.CodigoIntegracao);

                    if (!planoResponseAlcans.permit && plano.ExclusivoCliente)
                        return false;
                }
            }

            return retorno;
        }
    }
}
