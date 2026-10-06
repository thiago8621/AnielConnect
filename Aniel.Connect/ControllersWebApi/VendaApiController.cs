using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.ConnectBusiness.Playhub;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.DAO.map;
using Aniel.Connect.Data;
using Aniel.Connect.Integracoes.SerasaExperian.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Atendimento;
using Aniel.Connect.Repository.Bairro;
using Aniel.Connect.Repository.Banco;
using Aniel.Connect.Repository.Campanha;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ClienteContrato;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.Combo;
using Aniel.Connect.Repository.CondominioPredio;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.ConsultaViabilidade;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.Departamento;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EnderecoLogradouro;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.FormaPagamento;
using Aniel.Connect.Repository.Funcionario;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.MK.Crm;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Mk;
using Aniel.Connect.Repository.ModeloContrato;
using Aniel.Connect.Repository.Pais;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Repository.PendenciaEndereco;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.Plano;
using Aniel.Connect.Repository.Playhub;
using Aniel.Connect.Repository.Pool_Ip;
using Aniel.Connect.Repository.Produto;
using Aniel.Connect.Repository.Repository.Alcans;
using Aniel.Connect.Repository.Repository.Azza.Crm;
using Aniel.Connect.Repository.Repository.ClienteContrato;
using Aniel.Connect.Repository.Repository.ComboPlanoProduto;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using Aniel.Connect.Repository.Repository.T5.Porta;
using Aniel.Connect.Repository.T5.Porta;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Uf;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Repository.Vencimento;
using Aniel.Connect.Repository.Venda;
using Aniel.Connect.Repository.Voalle;
using Aniel.Connect.Repository.Workflow;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Conventions;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.ModelLog;
using Sinapse.SerasaApi.Server.Responses;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;
using Telegram.Bot.Types;
using System.Web.Http;
using System.Web.UI.WebControls;
using Xceed.Words.NET;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Elemento_Rede_Porta = Sinapse.Model.Elemento_Rede_Porta;
using EnumEtapaVenda = Sinapse.Model.enumerados.EnumEtapaVenda;
using StatusEtapaVenda = Sinapse.Model.enumerados.StatusEtapaVenda;
using System.Web.Http.Description;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Aniel.Connect.Repository.PerfilVisualizacaoVenda;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Drawing;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.Owin.Security.Notifications;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Core.Attributes;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class VendaApiController : BaseApiController
    {

        //[HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/Vendaapi/GetAll")]
        public async Task<DataTableResponse<VendaListViewModel>> GetAll(Venda_DataTableRequest request)
        {
            List<VendaListViewModel> retorno = new List<VendaListViewModel>();
            int recordsTotal = 0;

            String Status = $"({request.Status})";
            String EtapaVenda = $"({request.EtapaVenda})";
            var OSVendasInstaladas = new List<Documento_ProducaoAniel>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionNH = HibernateUtil.GetSession())
                using (ISession sessionLOG = HibernateUtil.GetSessionLog())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNH))
                using (var repoDoc = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoEtapa = new Conexao<EtapaVenda>(sessionNH))
                using (var repoPerfilVisVenda = new Conexao<PerfilVisualizacaoVenda>(sessionNH))
                using (var repoFuncionario = new Conexao<IdentityUser>(sessionNH))
                using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionNH))
                using (var repoCliente_Desconto = new Conexao<Cliente_Desconto>(sessionNH))
                using (var repoTipo_Desconto = new Conexao<Tipo_Desconto>(sessionNH))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_VENDA A  ")
                        .AppendLine(" LEFT OUTER JOIN TB_CLIENTE B ")
                        .AppendLine(" ON(A.ID_CLIENTE = B.ID) ")

                        .AppendLine(" LEFT OUTER JOIN TB_FUNCIONARIO C ")
                        .AppendLine(" ON(A.ID_VENDEDOR = C.ID) ")

                        .AppendLine(" LEFT OUTER JOIN TB_ENDERECO_CLIENTE D ")
                        .AppendLine(" ON(B.ID_ENDERECO_PRINCIPAL = D.ID) ")

                        .AppendLine("LEFT JOIN ASPNETUSERS ASP ")
                        .AppendLine("ON(ASP.ID_FUNCIONARIO = C.ID) ")

                        .AppendLine("LEFT JOIN TB_PERFIL_VIS_VENDA PERF")
                        .AppendLine("ON(PERF.id = ASP.ID_PERFIL_VIS_VENDA) ")

                        .AppendLine(" LEFT JOIN TB_EQUIPE_VENDA V")
                        .AppendLine(" ON (V.ID = C.EQUIPE_VENDA)")

                        .AppendLine(" LEFT JOIN TB_VENCIMENTO E ")
                        .AppendLine(" ON(B.ID_VENCIMENTO = E.ID) ")

                        .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO G ")
                        .AppendLine(" ON(A.ID = G.ID_VENDA AND B.ID = G.ID_CLIENTE) ")

                        .AppendLine("LEFT JOIN TB_CAMPANHA CAMP ")
                        .AppendLine("ON G.ID_CAMPANHA = CAMP.ID")

                        .AppendLine(" LEFT JOIN TB_FUNCIONARIO I ")
                        .AppendLine(" ON(I.ID = A.ID_SUPERVISOR) ")

                        .AppendLine(" LEFT JOIN TB_MEIO_CAPTACAO J ")
                        .AppendLine(" ON(B.ID_MEIO_CAPTACAO = J.ID) ")

                        .AppendLine(" LEFT JOIN TB_CLIENTE K ")
                        .AppendLine(" ON(K.ID = B.ID_QUEM_INDICOU) ")

                        .AppendLine(" LEFT JOIN TB_TIPO_IMOVEL M ")
                        .AppendLine(" ON(M.ID = D.ID_TIPO_IMOVEL) ")

                        .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE N ")
                        .AppendLine(" ON(N.ID = B.ID_CATEGORIA) ")

                        .AppendLine(" LEFT JOIN TB_DEPARTAMENTO O ")
                        .AppendLine(" ON(O.ID = A.ID_DEPARTAMENTO) ")

                         .AppendLine(" LEFT JOIN TB_MOTIVO_DESISTENCIA_VENDA P ")
                        .AppendLine(" ON(P.ID = A.ID_MOTIVO_DESISTENCIA_VENDA) ")

                        .AppendLine(" LEFT JOIN TB_WORKFLOW_ATIVIDADE W ")
                        .AppendLine(" ON(W.ID_WORKFLOW_VENDAS = A.ID_WORKFLOW_VENDAS AND W.ID_STATUS_PADRAO = A.Status) ")

                        .AppendLine(" LEFT JOIN TB_ATIVSTATUS_VENDA Q ")
                        .AppendLine(" ON(Q.ID = W.ATIV_STATUSVENDA AND Q.STATUS_PADRAO = A.STATUS) ")

                        .AppendLine(" LEFT JOIN TB_ETAPA_VENDA ET ")
                        .AppendLine(" ON(ET.ID = A.ID_ETAPA_VENDA) ")

                        .AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO EL  ")
                        .AppendLine(" ON EL.ID  = D.ID_ENDERECO_LOGRADOURO ")

                        .AppendLine(" LEFT JOIN TB_UF R ON R.ID = EL.ID_UF  ")

                        .AppendLine(" LEFT JOIN TB_CIDADE T ON T.ID =EL.ID_CIDADE  ")

                        .AppendLine(" LEFT JOIN TB_BAIRRO U ON U.ID =EL.ID_BAIRRO  ")

                        .AppendLine(" LEFT JOIN TB_DEPARTAMENTO DE ON DE.ID = A.ID_SETOR_DEPARTAMENTO  ")

                        .AppendLine(" LEFT JOIN ASPNETUSERS ASU ON ASU.ID =A.ID_RESPONSAVEL  ");

                    StringBuilder sqlWhere = new StringBuilder();

                    List<(Filtro_PainelVendaEnum tipo_filtro, string valores)> filtros_modelo = GetModeloFiltros(request.Id_Modelo, request);

                    #region Perfil de usuário
                    //1) Buscar informações do perfil de usuário
                    var usuario = await repoFuncionario.selectFirst(p => p.Id == User.Identity.GetUserId());

                    if (usuario.PerfilVisualizacaoVenda != null)
                    {
                        //2) Buscar as informações dos parametros de visualização
                        var paramVisuVenda = await repoPerfilVisVenda.selectFirst(p => p.Id == usuario.PerfilVisualizacaoVenda.Id);

                        if (paramVisuVenda.PadraoVisualizaVenda == true && usuario.Funcionario?.Id != null && !request.OrigemDashBoard)
                        {
                            sqlWhere.AppendLine($"AND A.ID_VENDEDOR = '{usuario.Funcionario?.Id}'");
                        }
                        else
                        {
                            //Setor
                            if (paramVisuVenda.ParamsSetorDepto.Count() > 0)
                            {
                                StringBuilder setorIds = new StringBuilder();
                                foreach (var setor in paramVisuVenda.ParamsSetorDepto)
                                {
                                    if (setorIds.Length > 0)
                                    {
                                        setorIds.Append(", ");
                                    }
                                    setorIds.Append(setor.CodDepto.ToString());
                                }

                                sqlWhere.AppendLine($"AND O.ID IN ({setorIds})");
                            }

                            //Meio de Captação
                            if (paramVisuVenda.ParamsMeioCaptacao.Count() > 0)
                            {
                                StringBuilder meioCaptacaoIds = new StringBuilder();
                                foreach (var meioCaptacao in paramVisuVenda.ParamsMeioCaptacao)
                                {
                                    if (meioCaptacaoIds.Length > 0)
                                    {
                                        meioCaptacaoIds.Append(", ");
                                    }
                                    meioCaptacaoIds.Append(meioCaptacao.CodMeioCaptacao.ToString());
                                }

                                sqlWhere.AppendLine($"AND B.ID_MEIO_CAPTACAO IN ({meioCaptacaoIds})");
                            }

                            //Categoria Cliente
                            if (paramVisuVenda.ParamsCategoriaCliente.Count() > 0)
                            {
                                StringBuilder categoriasIds = new StringBuilder();
                                foreach (var categorias in paramVisuVenda.ParamsCategoriaCliente)
                                {
                                    if (categoriasIds.Length > 0)
                                    {
                                        categoriasIds.Append(", ");
                                    }
                                    categoriasIds.Append(categorias.CodCategoriaCliente.ToString());
                                }

                                sqlWhere.AppendLine($"AND B.ID_CATEGORIA IN ({categoriasIds})");
                            }

                            //Tipo Pessoa
                            if (paramVisuVenda.ParamsTipoPessoa.Count() > 0)
                            {
                                StringBuilder tipoPessoaIds = new StringBuilder();
                                foreach (var tipoPessoa in paramVisuVenda.ParamsTipoPessoa)
                                {
                                    if (tipoPessoaIds.Length > 0)
                                    {
                                        tipoPessoaIds.Append(", ");
                                    }
                                    tipoPessoaIds.Append(Convert.ToInt32(tipoPessoa.CodTipoPessoa));
                                }

                                sqlWhere.AppendLine($"AND B.TIPOPESSOA IN ({tipoPessoaIds})");
                            }

                            //Região: Cidade e UF
                            if (paramVisuVenda.ParamsRegiao.Count() > 0)
                            {
                                StringBuilder cidadeIds = new StringBuilder();
                                foreach (var regiao in paramVisuVenda.ParamsRegiao)
                                {
                                    if (cidadeIds.Length > 0)
                                    {
                                        cidadeIds.Append(", ");
                                    }
                                    cidadeIds.Append(regiao.CodCidade.ToString());
                                }

                                sqlWhere.AppendLine($"AND D.ID_CIDADE IN ({cidadeIds})");
                            }

                            //Equipe
                            if (paramVisuVenda.ParamsEquipeVenda.Count() > 0)
                            {
                                StringBuilder equipesIds = new StringBuilder();
                                foreach (var equipe in paramVisuVenda.ParamsEquipeVenda)
                                {
                                    if (equipesIds.Length > 0)
                                    {
                                        equipesIds.Append(", ");
                                    }
                                    equipesIds.Append(equipe.CodEquipeVenda.ToString());
                                }

                                sqlWhere.AppendLine($"AND C.EQUIPE_VENDA IN ({equipesIds})");
                            }
                        }
                    }
                    #endregion

                    #region Filtros
                    if (!String.IsNullOrEmpty(request.EquipeVenda))
                    {
                        sqlWhere.AppendLine(" AND C.EQUIPE_VENDA = @EQUIPE_VENDA");
                        parametros.Add("@EQUIPE_VENDA", request.EquipeVenda);
                    }


                    if (!String.IsNullOrEmpty(request.StatusEtapa) && request.ConsultaRestricao.Count > 0)
                    {

                        var consultas = String.Join(",", request.ConsultaRestricao.Select(p => $"{p}"));
                        sqlFrom.AppendLine(" LEFT JOIN TB_ETAPA_VENDA EV ");
                        sqlFrom.AppendLine($" ON EV.ID_VENDA =A.ID  AND  EV.STATUS =@STATUSEP AND EV.ETAPA IN ({consultas}) ");

                        parametros.Add("@STATUSEP", request.StatusEtapa);

                        List<string> etapavenda = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.EtapaVenda).
                                                     Select(p => p.valores.Replace("COLUNA", "EV.ETAPA")).
                                                       ToList();

                        if (etapavenda.Count() > 0)
                        {
                            sqlWhere.AppendLine($" AND ({String.Join(" AND ", etapavenda)}) ");
                        }


                    }

                    if (!String.IsNullOrEmpty(request.StatusAuditoria))
                    {
                        if (request.StatusAuditoria == "2")
                        {
                            sqlWhere.AppendLine(" AND A.STATUS_AUDITORIA IS NULL");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND A.STATUS_AUDITORIA = @STATUS_AUDITORIA");
                            parametros.Add("@STATUS_AUDITORIA", request.StatusAuditoria);
                        }
                    }

                    if (!String.IsNullOrEmpty(request.StatusVenda))
                    {
                        sqlWhere.AppendLine(" AND A.STATUSVENDA = @STATUS_VENDA");
                        parametros.Add("@STATUS_VENDA", request.StatusVenda);
                    }

                    if (!String.IsNullOrEmpty(request.Motivo_Desistencia_Venda))
                    {
                        sqlWhere.AppendLine(" AND P.DESCRICAO = @MOTIVO_DESISTENCIA");
                        parametros.Add("@MOTIVO_DESISTENCIA", request.Motivo_Desistencia_Venda);
                    }

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))//ok
                    {
                        int idProcura;
                        bool ehNumero = int.TryParse(request.search.value, out idProcura);

                        if (ehNumero)
                            sqlWhere.AppendLine(" AND ((B.CPF LIKE @SEARCHCPF) OR (UPPER(B.NOMEFANTASIA) LIKE @SEARCH) OR (A.ID LIKE @SEARCH) )");
                        else
                            sqlWhere.AppendLine(" AND ((B.CPF LIKE @SEARCHCPF) OR (UPPER(B.NOMEFANTASIA) LIKE @SEARCH))");

                        if (!parametros.ContainsKey("@SEARCH"))
                        {
                            parametros.Add("@SEARCH", String.Format("%{0}%", StringService.Truncate(request.search.value.ToUpper(), 255)));
                            parametros.Add("@SEARCHCPF", String.Format("%{0}%", StringService.Truncate(request.search.value.ToUpper(), 15)));
                        }
                    }

                    if (!String.IsNullOrEmpty(request.Ultimo_Contato))//OK
                    {
                        sqlWhere.AppendLine($" AND COALESCE(A.DATA_ULTIMO_CONTATO, CURRENT_TIMESTAMP) < @DATA_ULTIMO_CONTATO AND (A.STATUS <> 4) AND (A.STATUS <> 6) ");
                        parametros.Add("@DATA_ULTIMO_CONTATO", DateTime.Now.Date.AddDays(-Convert.ToInt64(request.Ultimo_Contato)).ToString("dd/MM/yyyy 23:59:59"));
                    }


                    if (!String.IsNullOrEmpty(request.SetorResponsavel))//OK
                    {
                        sqlWhere.AppendLine($" AND A.ID_SETOR_DEPARTAMENTO = @ID_SETOR_DEPARTAMENTO ");
                        parametros.Add("@ID_SETOR_DEPARTAMENTO", Convert.ToInt64(request.SetorResponsavel));
                    }

                    if (!String.IsNullOrEmpty(request.Responsavel))//OK
                    {
                        sqlWhere.AppendLine($" AND ID_RESPONSAVEL = @ID_RESPONSAVEL ");
                        parametros.Add("@ID_RESPONSAVEL", Convert.ToInt64(request.Responsavel));
                    }


                    if (!String.IsNullOrEmpty(request.AcessoDepartamento_Setor))//OK
                        if (!String.IsNullOrEmpty(request.AcessoUsuario) && !request.OrigemDashBoard)//OK
                        {
                            sqlWhere.AppendLine($" AND A.ID_DEPARTAMENTO = (SELECT P.ID_DEPARTAMENTO FROM ASPNETUSERS P WHERE P.ID = '{request.AcessoUsuario}' )");

                        }
                    if (!String.IsNullOrEmpty(request.AcessoProspect))//OK
                    {
                        sqlWhere.AppendLine($" AND B.TIPO_CLIENTE = '{request.AcessoProspect}' ");
                        //parametros.Add("@DATA_ULTIMO_CONTATO", DateTime.Now.Date.AddDays(-Convert.ToInt64(request.Ultimo_Contato)).ToString("dd/MM/yyyy 23:59:59"));
                    }

                    if (request.Id_Modelo == 4)
                    {
                        sqlWhere.AppendLine($"AND A.ID_VENDEDOR = '{usuario.Funcionario?.Id}'");

                    }
                    if (request.Id_Modelo == 5)
                    {
                        sqlWhere.AppendLine($"AND A.ID_SUPERVISOR = '{usuario.Funcionario?.Id}'");

                    }

                    List<string> status = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.StatusVenda).
                                                          Select(p => p.valores.Replace("COLUNA", "A.STATUS")).
                                                            ToList();

                    if (status.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", status)}) ");
                    }
                    else if (!String.IsNullOrEmpty(request.Status))
                    {
                        // utiliza o filtro de status caso não esteja presente no modelo de exibição mas o usuário filtre pelo status manualmente
                        sqlWhere.AppendLine($" AND A.STATUS IN (@STATUS)");
                        parametros.Add("@STATUS", request.Status);
                    }

                    List<string> cidades = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Cidade).
                                            Select(p => p.valores.Replace("COLUNA", "D.ID_CIDADE")).
                                            ToList();
                    if (cidades.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", cidades)}) ");
                    }

                    List<string> planos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Plano).
                        Select(p => p.valores.Replace("COLUNA", "G.ID_PLANO")).
                        ToList();

                    List<string> produtos = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Produto).
                                Select(p => p.valores.Replace("COLUNA", "G.ID_PRODUTO")).
                                ToList();

                    if (planos.Count() > 0)
                    {
                        if (sqlFrom.ToString().Contains("WHERE"))
                            sqlFrom.AppendLine($" AND ({String.Join(" AND ", planos)}) ");
                        else
                            sqlFrom.AppendLine($" WHERE ({String.Join(" AND ", planos)}) ");
                    }
                    if (produtos.Count() > 0)
                    {
                        if (sqlFrom.ToString().Contains("WHERE"))
                            sqlFrom.AppendLine($" AND ({String.Join(" AND ", produtos)}) ");
                        else
                            sqlFrom.AppendLine($" WHERE ({String.Join(" AND ", produtos)}) ");
                    }

                    //var formasPagamento = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Forma_Pagamento).
                    //                Select(p => p.valores.Replace("COLUNA", "O.ID")).
                    //                ToList();

                    //if (formasPagamento.Count() > 0)
                    //{
                    //    sqlWhere.AppendLine($" AND ({String.Join(" AND ", formasPagamento)}) ");
                    //}

                    List<string> tipoImoveis = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Tipo_Imovel).
                                    Select(p => p.valores.Replace("COLUNA", "M.ID")).
                                    ToList();

                    if (tipoImoveis.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", tipoImoveis)}) ");
                    }

                    List<string> supervisores = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Supervisor).
                                    Select(p => p.valores.Replace("COLUNA", "A.ID_SUPERVISOR")).
                                    ToList();

                    if (supervisores.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", supervisores)}) ");
                    }

                    List<string> vendedores = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Vendedor).
                                    Select(p => p.valores.Replace("COLUNA", "A.ID_VENDEDOR")).
                                    ToList();

                    if (vendedores.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", vendedores)}) ");
                    }

                    List<string> categoriasCliente = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Categoria_Cliente).
                                    Select(p => p.valores.Replace("COLUNA", "N.ID")).
                                    ToList();

                    if (categoriasCliente.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", categoriasCliente)}) ");
                    }

                    List<string> UFs = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.UF).
                                    Select(p => p.valores.Replace("(COLUNA = ", "").Replace(")", "")).
                                    ToList();

                    if (UFs.Count() > 0)
                    {
                        foreach (string id in UFs)
                        {
                            string valorEnum = Enum.GetValues(typeof(UF)).GetValue(Convert.ToInt64(id)).ToString();

                            sqlWhere.AppendLine($"AND (D.ENDERECO_UF = '{valorEnum}') ");
                        }
                    }

                    if (!String.IsNullOrEmpty(request.Estado))//OK
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_UF) = @ESTADO) ");
                        parametros.Add("@ESTADO", request.Estado);
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))//ok
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_BAIRRO) = @BAIRRO) ");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }

                    if (request.Id_Plano_Produto != null || request.Id_Combo != null)
                    {
                        List<String> validaPlano = new List<string>();
                        if (request.Id_Plano_Produto != null)
                        {
                            validaPlano.Add($"(G.ID_PLANO = @PLANOPRODUTO) ");
                            parametros.Add("@PLANOPRODUTO", request.Id_Plano_Produto);
                        }

                        if (request.Id_Combo != null)
                        {
                            validaPlano.Add($"(G.ID_COMBO = @COMBO)");
                            parametros.Add("@COMBO", request.Id_Combo);
                        }

                        sqlWhere.AppendLine($" AND ({string.Join(" OR ", validaPlano)}) ");

                    }

                    if (!String.IsNullOrEmpty(request.Vendedor))//OK
                    {
                        sqlWhere.AppendLine(" AND (A.ID_VENDEDOR = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Vendedor);
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))//Ok
                    {
                        sqlWhere.AppendLine(" AND (A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        parametros.Add("@ID_SUPERVISOR", request.Supervisor);
                    }

                    //if (!String.IsNullOrEmpty(request.Vencimento))//ok
                    //{
                    //    sqlWhere.AppendLine(" AND (E.DIA_VENCIMENTO = @VENCIMENTO) ");
                    //    parametros.Add("@VENCIMENTO", request.Vencimento);
                    //}

                    List<string> datas_venda = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.Data_Venda).
                                                        Select(p => p.valores.Replace("COLUNA", "CAST(A.DATA_VENDA AS DATE)")).
                                                        ToList();
                    if (datas_venda.Count() > 0)
                    {
                        sqlWhere.AppendLine($" AND ({String.Join(" AND ", datas_venda)}) ");
                    }

                    //DS-6786 - AJUSTE DAS VALIDAÇOES DAS DATAS PARA NAO GERAR DUPLICIDADE DE FILTROS
                    //Tempo limite para listagem de painel de vendas
                    string dataInicial = string.IsNullOrWhiteSpace(request.Data_Inicial) ? null : request.Data_Inicial;
                    if (dataInicial == null)
                    {
                        DateTime _data_atual = DateTime.Now;
                        Configuracoes_Gerais config = await repoConfig.selectFirst();
                        Boolean Atribuir_limite = config.Atribuir_Tempo_Limite_Venda;
                        Int32? _Meses_Visualizacao_Venda = config.Qtde_Meses_Visualizacao_Venda;
                        if (Atribuir_limite == true && _Meses_Visualizacao_Venda.Value > 0)
                        {
                            String _data_exebicao_venda = new DateTime(_data_atual.Year, _data_atual.Month, _data_atual.Day).AddMonths(-_Meses_Visualizacao_Venda.Value).ToString("yyyy/MM/dd 00:00");
                            sqlWhere.AppendLine($"  AND A.DATAHORACRIACAO BETWEEN  '{_data_exebicao_venda}' and '{_data_atual.ToString("yyyy/MM/dd 23:59")}' ");
                        }
                        else
                        {
                            DateTime dataInicio = DateTime.Now.Date.AddDays(-30);
                            sqlWhere.AppendLine($"AND A.DATAHORACRIACAO BETWEEN  '{dataInicio.ToString("yyyy/MM/dd 00:00")}'  AND '{DateTime.Now.ToString("yyyy/MM/dd 23:59")}'");
                        }

                    }
                    else
                    {

                        if (request.Id_Modelo == 3)
                        {
                            sqlWhere.AppendLine($"AND A.DATAHORACRIACAO BETWEEN '{DateTime.Now.Date.ToString("yyyy/MM/dd 00:00")}' AND '{DateTime.Now.ToString("yyyy/MM/dd 23:59")}'");
                        }
                        else
                        {
                            request.Data_Inicial = request.Data_Inicial + " 00:00";
                            request.Data_Final = request.Data_Final + " 23:59";
                            if (!request.TipoData.HasValue || request.TipoData == 0)
                            {
                                sqlWhere.AppendLine("AND A.DATAHORACRIACAO BETWEEN @DATAINICIAL_ABERTURA_VENDA AND @DATAFINAL_ABERTURA_VENDA");
                                parametros.Add("@DATAINICIAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Inicial));
                                parametros.Add("@DATAFINAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Final));

                            }
                            else if (request.TipoData == 1)
                            {
                                sqlWhere.AppendLine("AND A.DATA_VENDA BETWEEN @DATAINICIAL_VENDA AND @DATAFINAL_VENDA");
                                parametros.Add("@DATAINICIAL_VENDA", String.Format("{0}", request.Data_Inicial));
                                parametros.Add("@DATAFINAL_VENDA", String.Format("{0}", request.Data_Final));
                            }

                            else if (request.TipoData == 2)
                            {
                                sqlWhere.AppendLine("AND A.DATA_ENCERRAMENTO BETWEEN @DATAINICIAL_ENCERRAMENTO_VENDA AND @DATAFINAL_ENCERRAMENTO_VENDA");
                                parametros.Add("@DATAINICIAL_ENCERRAMENTO_VENDA", String.Format("{0}", request.Data_Inicial));
                                parametros.Add("@DATAFINAL_ENCERRAMENTO_VENDA", String.Format("{0}", request.Data_Final));
                            }
                            else if (request.TipoData == 4)
                            {
                                sqlWhere.AppendLine("AND (A.DATA_TERMO_ASSINADO BETWEEN @DATAINICIAL AND @DATAFINAL)");
                                parametros.Add("@DATAINICIAL", String.Format("{0}", request.Data_Inicial));
                                parametros.Add("@DATAFINAL", String.Format("{0}", request.Data_Final));
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(request.Contrato))//ok
                    {
                        sqlWhere.AppendLine(" AND CC.ID_MODELO_CONTRATO = @CONTRATO ");
                        parametros.Add("@CONTRATO", request.Contrato);
                    }
                    if (!String.IsNullOrEmpty(request.EquipeVenda))//ok
                    {
                        sqlWhere.AppendLine(" AND V.DESCRICAO = @EQUIPEVENDA ");
                        parametros.Add("@EQUIPEVENDA", request.EquipeVenda);
                    }
                    if (!String.IsNullOrEmpty(request.Tipo_Pessoa))
                    {
                        sqlWhere.AppendLine("AND B.TIPOPESSOA = @PESSOA");
                        parametros.Add("@PESSOA", request.Tipo_Pessoa);
                    }

                    if (!String.IsNullOrEmpty(request.Meio_Captacao))//ok
                    {
                        sqlWhere.AppendLine(" AND (B.ID_MEIO_CAPTACAO = @MEIO_CAPTACAO)");
                        parametros.Add("@MEIO_CAPTACAO", request.Meio_Captacao);
                    }


                    if (!String.IsNullOrEmpty(request.qtde_Dias_Sem_Contato))
                    {
                        Int32 Dias_Sem_Contato = (await new Conexao<Configuracoes_Gerais>(sessionNH).selectFirst()).Max_Dias_Sem_Contato.Value;
                        DateTime Data_Ultimo_Contato = DateTime.Now.AddDays(-Dias_Sem_Contato);

                        if (request.qtde_Dias_Sem_Contato == "0")
                        {
                            sqlWhere.AppendLine(" AND (A.DATA_ULTIMO_CONTATO > @QTDE_DIAS_SEM_CONTATO)");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND (A.DATA_ULTIMO_CONTATO < @QTDE_DIAS_SEM_CONTATO)");
                        }
                        parametros.Add("@QTDE_DIAS_SEM_CONTATO", Data_Ultimo_Contato);
                    }

                    if (!String.IsNullOrEmpty(request.Porta_Reservada))
                    {
                        if (request.Porta_Reservada == "0")
                        {
                            sqlWhere.AppendLine(" AND ((A.PORTA_RESERVADA = @PORTA_RESERVADA) OR (A.PORTA_RESERVADA IS NULL))");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND (A.PORTA_RESERVADA = @PORTA_RESERVADA)");
                        }
                        parametros.Add("@PORTA_RESERVADA", request.Porta_Reservada);
                    }

                    if (!String.IsNullOrEmpty(request.Etapa_Atual))
                    {
                        sqlWhere.AppendLine(" AND (ET.ETAPA = @ETAPA)");
                        parametros.Add("@ETAPA", request.Etapa_Atual);
                    }

                    if (!String.IsNullOrEmpty(request.termo_Adesao_Assinado))
                    {
                        if (request.termo_Adesao_Assinado == "0")
                        {
                            sqlWhere.AppendLine(" AND ((A.TERMO_ADESAO_ASSINADO = @TERMO_ADESAO_ASSINADO) OR (A.TERMO_ADESAO_ASSINADO IS NULL))");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND (A.TERMO_ADESAO_ASSINADO = @TERMO_ADESAO_ASSINADO)");
                        }
                        parametros.Add("@TERMO_ADESAO_ASSINADO", request.termo_Adesao_Assinado);
                    }



                    if (request.Campanha != null)
                    {
                        sqlWhere.AppendLine(" AND (CAMP.ID = @ID_CAMPANHA)");
                        parametros.Add("@ID_CAMPANHA", request.Campanha);
                    }

                    sqlWhere.AppendLine(" AND COALESCE(A.MUDANCA_PLANO,0) = 0");

                    if (request.ApenasInstalados && request.OrigemDashBoard)
                    {
                        OSVendasInstaladas = repoDoc
                            .QueryableSelect()
                            .Where(p => p.Id_Venda != null && p.Status == StatusExecucaoAniel.Fechada_Produtiva)
                            .ToList();
                        if (OSVendasInstaladas.Count > 0)
                            sqlWhere.AppendLine($" AND A.ID IN ({string.Join(",", OSVendasInstaladas.Select(p => p.Id_Venda).ToList())})");
                    }
                    #endregion

                    //Count
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.ID)  CO ");
                        sql.AppendLine(sqlFrom.ToString())
                           .AppendLine("WHERE A.ID > 0 " + sqlWhere.ToString());

                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.IsDBNull(reader["CO"]) ? 0 : Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT DISTINCT A.ID, B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, CASE WHEN EL.DESCRICAO IS NOT NULL THEN U.DESCRICAO ELSE D.ENDERECO_BAIRRO END AS ENDERECO_BAIRRO , N.DESCRICAO AS CATEGORIA_CLIENTE, "); // A.DATA_ENCERRAMENTO, data comentada por nao ser mais usada
                        sql.AppendLine(" D.ID_CIDADE, M.DESCRICAO AS DESC_TIPO_IMOVEL, CASE WHEN EL.DESCRICAO IS NOT NULL THEN T.DESCRICAO ELSE D.ENDERECO_CIDADE END AS ENDERECO_CIDADE , CASE WHEN EL.DESCRICAO IS NOT NULL THEN R.SIGLA ELSE D.ENDERECO_UF END AS ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME AS VENDEDOR, ");
                        sql.AppendLine(" I.NOME AS SUPERVISOR, A.ID_CAMPANHA, J.DESCRICAO AS MEIO_CAPTACAO, A.STATUS_AUDITORIA, "); //H.TITULO AS CAMPANHA,
                        sql.AppendLine(" A.STATUSVENDA AS STATUS_VENDA,A.STATUS,A.ID_WORKFLOW_VENDAS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO, ");
                        sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA AS QUEM_INDICOU, A.DATAHORACRIACAO, A.DATA_VENDA, A.USUARIOALTERACAO, B.NUMERO_CONTRATO AS NUMERO_CONTRATO, ");
                        sql.AppendLine(" A.ID_CLIENTE,A.DATA_ULTIMO_CONTATO, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, O.DESCRICAO AS DEPARTAMENTO, A.STATUS_LIBERACAO, ");
                        sql.AppendLine(" V.DESCRICAO  AS EQUIPEVENDA, COALESCE(P.DESCRICAO, A.JUSTIFICATIVA_AVALIACAO) AS MOTIVO_DESISTENCIA_VENDA, ET.DESCRICAO AS ETAPA_ATUAL, W.ATIV_STATUSVENDA,ASU.NOME AS RESPONSAVEL, DE.DESCRICAO AS SETORRESP ");

                        sql.AppendLine(sqlFrom.ToString())
                        .AppendLine("WHERE A.ID > 0 " + sqlWhere.ToString());

                        //Ordenacao
                        if (request.order != null && request.order.Any())
                        {
                            int sortColumnIndex = request.order[0].column;
                            string coluna = request.columns[sortColumnIndex].data;
                            string sortDirection = request.order[0].dir;
                            switch (coluna)
                            {

                                case "vendedor":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY C.NOME");
                                        else
                                            sql.AppendLine(" ORDER BY C.NOME DESC");
                                        break;
                                    }
                                case "supervisor":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY I.NOME");
                                        else
                                            sql.AppendLine(" ORDER BY I.NOME DESC");
                                        break;
                                    }
                                case "id":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.ID");
                                        else
                                            sql.AppendLine(" ORDER BY A.ID DESC");
                                        break;
                                    }
                                case "nomeFantasia":     // NomeFantasia
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.NOMEFANTASIA");
                                        else
                                            sql.AppendLine(" ORDER BY B.NOMEFANTASIA DESC");
                                        break;
                                    }
                                case "cpfCnpj":     // CpfCnpj
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.CPF");
                                        else
                                            sql.AppendLine(" ORDER BY B.CPF DESC");
                                        break;
                                    }

                                case "estado":     // Estado
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_UF");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_UF DESC");
                                        break;
                                    }
                                case "cidade":     // Cidade
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_CIDADE");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_CIDADE DESC");
                                        break;
                                    }
                                case "telefonePrincipal":     // Telefone
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.TELEFONE_FIXO");
                                        else
                                            sql.AppendLine(" ORDER BY B.TELEFONE_FIXO DESC");
                                        break;
                                    }
                                case "data_Hora_Criacao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO DESC");
                                        break;
                                    }
                                case "dataVenda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA DESC");
                                        break;
                                    }
                                case "dataHoraAprovacao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA DESC");
                                        break;
                                    }
                                case "dataHoraCancelamento":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_VENDA DESC");
                                        break;
                                    }
                                case "dataUltimoContato":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_ULTIMO_CONTATO");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_ULTIMO_CONTATO DESC");
                                        break;
                                    }
                                case "telefoneCelular":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.TELEFONE_CELULAR");
                                        else
                                            sql.AppendLine(" ORDER BY B.TELEFONE_CELULAR DESC");
                                        break;
                                    }
                                case "complemento":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_COMPLEMENTO");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_COMPLEMENTO DESC");
                                        break;
                                    }
                                case "numero":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_NUMERO");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_NUMERO DESC");
                                        break;
                                    }
                                case "bairro":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_BAIRRO");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_BAIRRO DESC");
                                        break;
                                    }
                                case "logradouro":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.ENDERECO_LOGRADOURO");
                                        else
                                            sql.AppendLine(" ORDER BY D.ENDERECO_LOGRADOURO DESC");
                                        break;
                                    }
                                case "meio_Captacao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY J.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY J.DESCRICAO DESC");
                                        break;
                                    }
                                case "quem_Indicou":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY K.NOMEFANTASIA");
                                        else
                                            sql.AppendLine(" ORDER BY K.NOMEFANTASIA DESC");
                                        break;
                                    }
                                case "email":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.EMAIL_PRINCIPAL");
                                        else
                                            sql.AppendLine(" ORDER BY B.EMAIL_PRINCIPAL DESC");
                                        break;
                                    }
                                case "termo_Adesao_Assinado":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.TERMO_ADESAO_ASSINADO");
                                        else
                                            sql.AppendLine(" ORDER BY A.TERMO_ADESAO_ASSINADO DESC");
                                        break;
                                    }
                                case "diasSemContato":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY DATA_ULTIMO_CONTATO");
                                        else
                                            sql.AppendLine(" ORDER BY DATA_ULTIMO_CONTATO DESC");
                                        break;
                                    }
                                case "etapa_Atual":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY ET.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY ET.DESCRICAO DESC");
                                        break;
                                    }
                                case "motivo_Desistencia_Venda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY P.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY P.DESCRICAO DESC");
                                        break;
                                    }
                                case "data_Termo_Assinado":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATA_TERMO_ASSINADO");
                                        else
                                            sql.AppendLine(" ORDER BY A.DATA_TERMO_ASSINADO DESC");
                                        break;
                                    }
                                case "equipeVenda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY V.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY V.DESCRICAO DESC");
                                        break;
                                    }
                                case "departamento":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY O.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY O.DESCRICAO DESC");
                                        break;
                                    }
                                case "identificacao_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.NUMERO_CONTRATO");
                                        else
                                            sql.AppendLine(" ORDER BY B.NUMERO_CONTRATO DESC");
                                        break;
                                    }
                                case "categoria_Cliente":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY N.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY N.DESCRICAO DESC");
                                        break;
                                    }
                                case "profissao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.PROFISSAO");
                                        else
                                            sql.AppendLine(" ORDER BY B.PROFISSAO DESC");
                                        break;
                                    }

                            }
                        }


                        //Busca com paginacao
                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                string statusV = Convert.ToString(reader["STATUS_VENDA"]);

                                if (String.IsNullOrEmpty(statusV) && reader["STATUS"] != DBNull.Value)
                                {
                                    statusV = HtmlEnumExtensions.GetEnumDescription((Status_Venda)Convert.ToInt32(reader["STATUS"]));
                                }

                                int statusVenda = reader["STATUS"] != DBNull.Value ? Convert.ToInt32(reader["STATUS"]) : 0;

                                string idVenda = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "0";
                                var checklist = await VerificaCheckList(idVenda);

                                //String StatusAtutomatico = HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_LIBERACAO"]);

                                var usuario_AprovacaoCancelamento = string.Empty;
                                if (Convert.ToInt64(idVenda) > 0)
                                {
                                    if (statusVenda == 6 || statusVenda == 4)
                                    {
                                        using (DbCommand comLOG = sessionLOG.Connection.CreateCommand())
                                        {
                                            var sqlLOG = new StringBuilder();
                                            sqlLOG.AppendLine("SELECT FIRST 1 NOMEUSUARIOCRIACAO FROM TB_VENDAS");

                                            sqlLOG.AppendLine("WHERE IDLOG = @ID_LOG AND CAMPO  ='Status Venda' AND STATUSVENDA = @STATUSVENDA");

                                            sqlLOG.AppendLine("ORDER BY DATAHORACRIACAO DESC");

                                            var parametrosLOG = new Dictionary<string, object>()
                                            {
                                                {"ID_LOG", Convert.ToInt64(idVenda)},
                                                {"@STATUSVENDA",statusVenda }
                                            };


                                            using (var readerLOG = await CommandHelper.ExecuteReader(comLOG, sqlLOG, parametrosLOG, sessionLOG.Connection, null))
                                            {
                                                if (await readerLOG.ReadAsync())
                                                {
                                                    usuario_AprovacaoCancelamento = readerLOG["NOMEUSUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(readerLOG["NOMEUSUARIOCRIACAO"]) : string.Empty;
                                                }
                                            }
                                        }
                                    }
                                }

                                Double valor_total_final = 0;
                                Double? valor_instalacao = 0;
                                var lista_cliente_plano_produto = await repoCliente_PlanoProduto.select(x => x.Id_Venda == Convert.ToInt64(reader["ID"]));
                                if (lista_cliente_plano_produto.Count > 0)
                                {
                                    List<Int64> Ids = new List<Int64>();
                                    foreach (var cliente_plano_produto in lista_cliente_plano_produto)
                                    {
                                        if (cliente_plano_produto?.Valor_Final > 0)
                                        {
                                            if (cliente_plano_produto?.ClienteContrato != null)
                                            {
                                                var cliente_desconto = await repoCliente_Desconto.select(x => x.Cliente.Id == Convert.ToInt64(reader["ID_CLIENTE"]) && x.Numero_ContratoVinculado == Convert.ToString(cliente_plano_produto.ClienteContrato.Id));
                                                if (cliente_desconto.Count > 0)
                                                {
                                                    Ids.AddRange(cliente_desconto.Select(x => x.Id));
                                                }
                                            }

                                            valor_total_final += cliente_plano_produto?.Valor_Final > 0 ? Convert.ToDouble(cliente_plano_produto?.Valor_Final) : 0;
                                        }

                                        if (cliente_plano_produto?.Valor_Instalacao_Real > 0)
                                        {
                                            valor_instalacao += cliente_plano_produto.Valor_Instalacao_Real;
                                        }
                                    }

                                    if (valor_total_final > 0)
                                    {
                                        #region [variaveis acrescimo]
                                        Double valor_acrescimento = 0;
                                        Double percentual_acrescimento = 0;
                                        Double resultado_acrescimo = 0;
                                        #endregion

                                        #region [variaveis desconto]
                                        Double valor_desconto = 0;
                                        Double percentual_desconto = 0;
                                        Double resultado_desconto = 0;
                                        #endregion

                                        var listaDesconto = await repoCliente_Desconto.select(x => x.Id.IsIn(Ids));
                                        foreach (var obj in listaDesconto)
                                        {
                                            if (obj.Desconto != null)
                                            {
                                                var obj_tipo_desconto = await repoTipo_Desconto.selectFirst(x1 => x1.Id == obj.Desconto.Tipo_Desconto.Id);
                                                valor_desconto += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Desconto ? obj.Valor_Desconto : 0;
                                                percentual_desconto += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Desconto ? obj.Percentual_Desconto : 0;

                                                valor_acrescimento += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Acrescimo ? obj.Valor_Desconto : 0;
                                                percentual_acrescimento += obj_tipo_desconto.Formato == Formato_Tipo_Desconto.Acrescimo ? obj.Percentual_Desconto : 0;
                                            }
                                        }

                                        resultado_desconto = valor_desconto * 100;
                                        if (resultado_desconto > 0)
                                        {
                                            resultado_desconto = (resultado_desconto / valor_total_final);
                                        }

                                        resultado_desconto = percentual_desconto > 0 ? resultado_desconto + percentual_desconto : resultado_desconto;
                                        var total_desconto = (valor_total_final * resultado_desconto) / 100;

                                        resultado_acrescimo = valor_acrescimento * 100;
                                        if (resultado_acrescimo > 0)
                                        {
                                            resultado_acrescimo = (resultado_acrescimo / valor_total_final);
                                        }

                                        resultado_acrescimo = percentual_acrescimento > 0 ? resultado_acrescimo + percentual_acrescimento : resultado_acrescimo;
                                        var total_acrescimo = (valor_total_final * resultado_acrescimo) / 100;

                                        valor_total_final = (valor_total_final + total_acrescimo) - total_desconto;
                                    }
                                }

                                retorno.Add(new VendaListViewModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Pendencia = false,
                                    Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                    Vendedor = Convert.ToString(reader["VENDEDOR"]),
                                    Departamento = Convert.ToString(reader["DEPARTAMENTO"]),
                                    Tipo_Cliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                    Tipo_Pessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    Identificacao_Cliente = Convert.ToString(reader["NUMERO_CONTRATO"]),

                                    Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                    Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                    Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                    Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]),
                                    Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                    Complemento = Convert.ToString(reader["ENDERECO_COMPLEMENTO"]),

                                    DiasSemContato = reader["DATA_ULTIMO_CONTATO"] != DBNull.Value ? Convert.ToString(DateTime.Now.Subtract(Convert.ToDateTime(reader["DATA_ULTIMO_CONTATO"])).Days) : "",
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    TelefoneCelular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                    Email = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    Profissao = Convert.ToString(reader["PROFISSAO"]),
                                    Tipo_Imovel = Convert.ToString(reader["DESC_TIPO_IMOVEL"]),
                                    Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                    Data_Expiracao = "",
                                    Avaliacao_Expirada = "",

                                    DataVenda = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy") : "",
                                    DataHoraAprovacao = reader["DATA_VENDA"] != DBNull.Value && (statusVenda == 2 || statusVenda == 6) ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    UsuarioAprovacaoCancelamento = !string.IsNullOrEmpty(usuario_AprovacaoCancelamento) ? usuario_AprovacaoCancelamento : string.Empty,
                                    DataHoraCancelamento = reader["DATA_VENDA"] != DBNull.Value && (statusVenda == 3 || statusVenda == 4) ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    DataUltimoContato = reader["DATA_ULTIMO_CONTATO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ULTIMO_CONTATO"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    Meio_Captacao = Convert.ToString(reader["MEIO_CAPTACAO"]),
                                    Quem_Indicou = Convert.ToString(reader["QUEM_INDICOU"]),
                                    Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                    EquipeVenda = Convert.ToString(reader["EQUIPEVENDA"]),
                                    Setor_Responsavel = reader["SETORRESP"] != DBNull.Value ? Convert.ToString(reader["SETORRESP"]) : "",
                                    Responsavel = reader["RESPONSAVEL"] != DBNull.Value ? Convert.ToString(reader["RESPONSAVEL"]) : "",

                                    //if (Perfil_Vendedor_Id?.Autorizacao_Vendas !=false) { 
                                    //    Status=Status_Venda.Manual,
                                    //}


                                    //                                 //   Status = reader["STATUS_LIBERACAO"] == DBNull.Value || Convert.ToInt64(reader["STATUS_LIBERACAO"]) == 6 ? HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_VENDA"]) : HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_LIBERACAO"]),
                                    //Status = reader["STATUS_VENDA"].ToString(),
                                    // Status = reader["STATUS_LIBERACAO"] == DBNull.Value || reader["STATUS_LIBERACAO"] == 6 ? HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_VENDA"]) : HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_LIBERACAO"]),

                                    Status = statusV,

                                    //data comentada por nao ser mais usada
                                    //Data_Encerramento = reader["DATA_ENCERRAMENTO"] == DBNull.Value ? String.Empty : Convert.ToDateTime(reader["DATA_ENCERRAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss"),

                                    //Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]) reader["ID_CLIENTE"]
                                    Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : 0,

                                    Termo_Adesao_Assinado = HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.Padrao>(Convert.ToString(reader["TERMO_ADESAO_ASSINADO"] != DBNull.Value ? reader["TERMO_ADESAO_ASSINADO"] : 0)),
                                    Data_Termo_Assinado = reader["DATA_TERMO_ASSINADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_TERMO_ASSINADO"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    Etapa_Atual = reader["ETAPA_ATUAL"] != DBNull.Value ? Convert.ToString(reader["ETAPA_ATUAL"]) : "",
                                    Motivo_Desistencia_Venda = reader["MOTIVO_DESISTENCIA_VENDA"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_DESISTENCIA_VENDA"]) : "",
                                    Checklist = HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.Padrao>(checklist != null ? Sinapse.Model.enumerados.Padrao.Sim : Sinapse.Model.enumerados.Padrao.Nao),
                                    Data_Preenchimento_Checklist = checklist == null ? "" : checklist,
                                    Valor_Total_Venda = String.Format("{0:N2}", valor_total_final),
                                    Valor_Instalacao = String.Format("{0:N2}", valor_instalacao),
                                    Status_Auditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusAuditoria>(Convert.ToString(reader["STATUS_AUDITORIA"])) : ""
                                });
                            }
                        }

                    }


                    //Page e retornando DataTable
                    return new DataTableResponse<VendaListViewModel>
                    {
                        draw = request.draw,
                        //recordsTotal = recordsTotal,
                        recordsFiltered = recordsTotal,
                        data = retorno,
                        error = ""
                    };

                }


            }
            catch (Exception e)
            {
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               e.Message,
                                               e?.InnerException?.Message,
                                               e.StackTrace);

                throw e;
            }
        }

        public async Task<EnderecoVendaIntegracao> GetEnderencoByVenda(long idVenda)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" SELECT C.COD_AUX COD_CIDADE, D.COD_AUXILIAR COD_UF, E.COD_AUX COD_BAIRRO, F.COD_AUXILIAR COD_RUA, B.ENDERECO_NUMERO
                            FROM TB_VENDA A
                            INNER JOIN TB_ENDERECO_CLIENTE B
                            ON (B.ID = A.ID_ENDERECO_INSTALACAO)
                            INNER JOIN TB_CIDADE C
                            ON (C.ID = B.ID_CIDADE)
                            INNER JOIN TB_UF D
                            ON(D.SIGLA = B.ENDERECO_UF)
                            INNER JOIN TB_ENDERECO_LOGRADOURO F
                            ON(F.ID = B.ID_ENDERECO_LOGRADOURO )
                            INNER JOIN TB_BAIRRO E
                            ON(E.ID = F.ID_BAIRRO )
                            WHERE A.ID = @idVenda 
                            AND C.COD_AUX IS NOT NULL AND D.COD_AUXILIAR IS NOT NULL AND E.COD_AUX IS NOT NULL AND F.COD_AUXILIAR IS NOT NULL
            ");

            Dictionary<string, object> parametros = new Dictionary<string, object> { { "idVenda", idVenda } };

            using (ISession session = HibernateUtil.GetSession())
            using (DbCommand com = session.Connection.CreateCommand())
            {
                using DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);
                if (await reader.ReadAsync())
                {
                    return new EnderecoVendaIntegracao
                    {
                        CodCidade = Convert.ToString(reader["COD_CIDADE"]),
                        CodUf = Convert.ToString(reader["COD_UF"]),
                        CodBairro = Convert.ToString(reader["COD_BAIRRO"]),
                        CodRua = Convert.ToString(reader["COD_RUA"]),
                        Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                    };
                }

                return null;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/VendaApi/GetServicosVenda")]
        public async Task<DataTableResponse<ServicosVendaListModel>> GetServicosVenda(ServicosVendaDataTableRequest request)
        {
            var retorno = new List<ServicosVendaListModel>();
            int recordsTotal = 0;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                var parametros = new Dictionary<string, object>();

                var sqlFrom = new StringBuilder();

                sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D");

                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE T");
                sqlFrom.AppendLine("ON (T.COD_TIPO_SERV = D.COD_TIPO_SERV)");

                sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE E");
                sqlFrom.AppendLine("ON (E.EQUIPE = D.EQUIPE)");

                var filtros = new List<string>();

                #region Filtros

                filtros.Add("(D.ID_VENDA = @ID_VENDA)");
                parametros.Add("@ID_VENDA", request.IdVenda);

                if (!String.IsNullOrEmpty(request.DataInicio) && !String.IsNullOrEmpty(request.DataFim))
                {
                    var dataInicio = Convert.ToDateTime(request.DataInicio);
                    var dataFim = Convert.ToDateTime(request.DataFim);

                    filtros.Add("(D.DIA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                    parametros.Add("@DATA_INICIO", request.DataInicio);
                    parametros.Add("@DATA_FIM", request.DataFim);
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    filtros.Add("(D.STATUS_EXECUCAO = @STATUS)");
                    parametros.Add("@STATUS", request.Status);
                }

                #endregion

                //Count
                using (DbCommand com = sessionT5.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT COUNT(D.NUM_OBRA) AS CO ");

                    sql.AppendLine(sqlFrom.ToString());

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine("WHERE " + String.Join(" AND ", filtros));
                    }

                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (DbCommand com = sessionT5.Connection.CreateCommand())
                using (var docProdApi = new Documento_ProducaoApiController())
                {
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT D.CODCT, D.PROJETO, D.NUM_OBRA_ORIGINAL, D.NUM_DOC, D.NUM_OBRA,");
                    sql.AppendLine("       (D.DATA_MAXIMA + D.HORA_MAXIMA) AS DATA_HORA_AGENDAMENTO, T.DESCRICAO AS TIPO_SERVICO,");
                    sql.AppendLine("       E.NOME AS EQUIPE, D.STATUS_EXECUCAO, (D.DATA_INDICADOR + D.HORA_INDICADOR) AS DATAHORA_INDICADOR,");
                    sql.AppendLine("       (D.DIA + COALESCE(D.HORA, CAST('00:00' AS TIME))) AS DATAHORA_ABERTURA_OS,");
                    sql.AppendLine("       (D.DATA_PROD + D.HORA_PROD) AS DATAHORA_ENCERRAMENTO_OS, D.DATAHORA_FIM_ATEND, D.DATAHORA_INI_ATEND");

                    sql.AppendLine(sqlFrom.ToString());

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine("WHERE " + String.Join(" AND ", filtros));
                    }

                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var servico = new ServicosVendaListModel();

                            servico.CodCt = Convert.ToString(reader["CODCT"]);
                            servico.Projeto = Convert.ToString(reader["PROJETO"]);
                            servico.NumObra = Convert.ToString(reader["NUM_OBRA"]);
                            servico.NumObraOriginal = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);
                            servico.NumDoc = Convert.ToString(reader["NUM_DOC"]);

                            servico.DataAgendamento =
                                reader["DATA_HORA_AGENDAMENTO"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATA_HORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") :
                                    String.Empty;

                            servico.TipoServico = Convert.ToString(reader["TIPO_SERVICO"]);
                            servico.Tecnico = Convert.ToString(reader["EQUIPE"]);

                            if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                            {
                                DateTime? dataHoraIndicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                DateTime? dataHoraCriacao = reader["DATAHORA_ABERTURA_OS"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_ABERTURA_OS"]) : (DateTime?)null;
                                DateTime? dataHoraProd = null;

                                if (reader["DATAHORA_ENCERRAMENTO_OS"] != DBNull.Value)
                                    dataHoraProd = Convert.ToDateTime(reader["DATAHORA_ENCERRAMENTO_OS"]);

                                servico.Sla = docProdApi.GetDescricaoHorasSla(dataHoraIndicador, dataHoraCriacao, dataHoraProd);
                            }
                            else
                            {
                                servico.Sla = "Sem Indicador de Sla";
                            }

                            servico.Status =
                                reader["STATUS_EXECUCAO"] != DBNull.Value ?
                                    HtmlEnumExtensions.GetEnumDescription((StatusExecucaoAniel)Convert.ToInt32(reader["STATUS_EXECUCAO"])) :
                                    String.Empty;

                            servico.InicioAtendimento =
                                reader["DATAHORA_INI_ATEND"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm") :
                                    String.Empty;

                            servico.FimAtendimento =
                                reader["DATAHORA_FIM_ATEND"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm") :
                                    String.Empty;

                            retorno.Add(servico);
                        }
                    }
                }
            }

            return new DataTableResponse<ServicosVendaListModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpGet()]
        [Route("api/VendaApi/EnviarEmailChecklist")]
        public async Task<PostApiRetorno<long>> EnviarEmailChecklist(long IdVenda)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    using (var repoVenda = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var apiTermoAdesao = new Termo_AdesaoApiController())
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == IdVenda);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                        var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                        if (String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui e-mail vinculado");
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (venda?.Vendedor?.Filial?.Empresa?.CodEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");
                        }

                        Models.InfoEmpresaTermoAdesao infoEmpresa = GetInfoEmpresa(Convert.ToString(venda.Vendedor.Filial.Empresa.CodEmpresa));

                        if (infoEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");
                        }

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        var link = await apiTermoAdesao.GerarLinkTermoAdesao(venda.Id, false);
                        String urlChecklistVenda = link.IdObjeto;
                        //String urlChecklistVenda = $@"{CONSTANTES.URLPortal}/pt/Venda/EmailChecklist?Id_Venda={venda.Id}&Email=S";

                        string tableProdutos = GerarTabelaProdutosHTML(listaPlanosProdutos);

                        using var stream = System.IO.File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_checklist_venda.html");
                        String arquivo_email = stream.ReadToEnd();
                        String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                                 .Replace("{{URL_CHECKLIST_VENDA}}", urlChecklistVenda)
                                                                 .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                 .Replace("{{PRODUTOS}}", tableProdutos);

                        emailService.EnviarMensagem(cliente.Email_Principal, $@"Bem-vindo(a) à {infoEmpresa.Nome}! Segue Checklist de documentos para validação.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                    }

                    retorno.IdObjeto = venda.Id;
                    retorno.Ok = true;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);

                    transaction.Rollback();

                    retorno.IdObjeto = 0;

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        public static string GerarTabelaProdutosHTML(IEnumerable<Cliente_PlanoProduto> listaPlanosProdutos)
        {
            var tableProdutos = "<table class='tg align-horizontal emailVenda' style='border-collapse:collapse;border-spacing:0;border: 2px solid;border-color:#0ca4e7'>";

            #region CABECALHO

            var cabecalho = new StringBuilder(@"<thead>
                                    <tr class='tr-head'>");

            cabecalho.AppendLine("<th class='th-head th-head-left' style='border-style:solid;border-width:2px;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;font-weight:bold;text-align:left;vertical-align:top;border-left-width:0px;border-left-style:solid;'>Descrição</th>\r\n");

            if (CONSTANTES.ExibeVencimentoEmail)
                cabecalho.AppendLine("<th class='th-head' style='border-style:solid;border-width:2px;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;font-weight:bold;text-align:left;vertical-align:top;'>Dia de Vencimento</th>\r\n");

            if (CONSTANTES.ExibeDiasGratisEmail)
                cabecalho.AppendLine("<th class='th-head' style='border-style:solid;border-width:2px;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;font-weight:bold;text-align:left;vertical-align:top;'>Período de teste grátis</th>\r\n");

            if (CONSTANTES.ExibeValorEmail)
                cabecalho.AppendLine("<th class='th-head th-head-right' style='border-style:solid;border-width:2px;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;font-weight:bold;text-align:left;vertical-align:top;border-right-width:0px;border-right-style:solid;'>Valor Total</th>");

            cabecalho.AppendLine("</tr></thead>");

            tableProdutos += cabecalho.ToString();

            #endregion

            var itens = new StringBuilder("<tbody>");

            foreach (var planoProduto in listaPlanosProdutos)
            {
                if (planoProduto.Combo != null)
                {
                    if (CONSTANTES.ExibeApenasComboEmail)
                    {
                        var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                        var descricao = planoProduto.Combo.Descricao;
                        string vencimento = planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "";
                        string diasGratis = dias == 1 ? dias + "dia" : dias + " dias";
                        string valor = planoProduto.Combo.PlanosProdutos.Where(x => x.Plano != null).Sum(x => x.Valor).ToString("C", new CultureInfo("pt-BR")) ?? "R$0,00";

                        var item = new StringBuilder(@$"
                                <tr>
                                    <td class='desc' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:left;vertical-align:top;border-left-width:0px;border-left-style:solid;'>{descricao}</td>");

                        if (CONSTANTES.ExibeVencimentoEmail)
                            item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{vencimento}</td>\r\n");

                        if (CONSTANTES.ExibeDiasGratisEmail)
                            item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{diasGratis}</td>");

                        if (CONSTANTES.ExibeValorEmail)
                            item.AppendLine($"<td class='valor' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:right;vertical-align:top;border-right-width:0px;border-right-style:solid;'>{valor}</td>");

                        item.AppendLine("</tr>");

                        itens.Append(item);
                    }
                    else
                    {
                        foreach (var planoProdutoCombo in planoProduto.Combo.PlanosProdutos)
                        {
                            var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                            var descricao = planoProdutoCombo.Plano != null ? planoProdutoCombo.Plano.Descricao : planoProdutoCombo.Produto.Descricao;
                            string vencimento = planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "";
                            string diasGratis = dias == 1 ? dias + "dia" : dias + " dias";
                            string valor = planoProdutoCombo != null ? planoProdutoCombo.Valor.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00";

                            var item = new StringBuilder(@$"
                                <tr>
                                    <td class='desc' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:left;vertical-align:top;border-left-width:0px;border-left-style:solid;'>{descricao}</td>");

                            if (CONSTANTES.ExibeVencimentoEmail)
                                item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{vencimento}</td>\r\n");

                            if (CONSTANTES.ExibeDiasGratisEmail)
                                item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{diasGratis}</td>");

                            if (CONSTANTES.ExibeValorEmail)
                                item.AppendLine($"<td class='valor' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:right;vertical-align:top;border-right-width:0px;border-right-style:solid;'>{valor}</td>");

                            item.AppendLine("</tr>");

                            itens.Append(item);
                        }
                    }
                }
                else
                {
                    var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                    var descricao = planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao;
                    string vencimento = planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "";
                    string diasGratis = dias == 1 ? dias + "dia" : dias + " dias";
                    string valor = planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00";

                    var item = new StringBuilder(@$"
                                <tr>
                                    <td class='desc' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:left;vertical-align:top;border-left-width:0px;border-left-style:solid;'>{descricao}</td>");

                    if (CONSTANTES.ExibeVencimentoEmail)
                        item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{vencimento}</td>\r\n");

                    if (CONSTANTES.ExibeDiasGratisEmail)
                        item.AppendLine($"<td class='tg-jt00' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:center;vertical-align:top;'>{diasGratis}</td>");

                    if (CONSTANTES.ExibeValorEmail)
                        item.AppendLine($"<td class='valor' style='border-style:solid;border-width:2px;font-family:Arial, sans-serif;font-size:14px;overflow:hidden;padding-top:10px;padding-bottom:10px;padding-right:5px;padding-left:5px;word-break:normal;border-color:#0ca4e7;text-align:right;vertical-align:top;border-right-width:0px;border-right-style:solid;'>{valor}</td>");

                    item.AppendLine("</tr>");

                    itens.Append(item);
                }
            }

            itens.AppendLine("</tbody>");

            tableProdutos += itens.ToString();
            tableProdutos += "</table>";

            return tableProdutos;
        }

        public Aniel.Connect.Models.InfoEmpresaTermoAdesao GetInfoEmpresa(String codEmpresa)
        {
            Aniel.Connect.Models.InfoEmpresaTermoAdesao retorno = null;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT RAZAO_SOCIAL, EMPRESA, CNPJ, ENDERECO, BAIRRO, CIDADE, UF, LOGOTIPO ");
            sql.AppendLine("FROM TB_EMPRESA");
            sql.AppendLine("WHERE CODEMPRESA = :pCOD_EMPRESA");

            using ISession session = HibernateUtil.GetSessionT5();
            using Sinapse.DAO.repository.Query query = new Sinapse.DAO.repository.Query(session);
            {
                query.setParametro("pCOD_EMPRESA", codEmpresa);
                var values = query.Values(sql.ToString()).Result;

                if (values.Count > 0)
                    retorno = new Aniel.Connect.Models.InfoEmpresaTermoAdesao
                    {
                        Nome = Convert.ToString(values[0]["EMPRESA"]),
                        RazaoSocial = Convert.ToString(values[0]["RAZAO_SOCIAL"]),
                        Cnpj = Convert.ToString(values[0]["CNPJ"]),
                        Endereco = Convert.ToString(values[0]["ENDERECO"]),
                        Bairro = Convert.ToString(values[0]["BAIRRO"]),
                        Cidade = Convert.ToString(values[0]["CIDADE"]),
                        Estado = Convert.ToString(values[0]["UF"]),
                        Logo = (byte[])values[0]["LOGOTIPO"]

                    };
            }

            return retorno;
        }

        public List<(Filtro_PainelVendaEnum tipo_filtro, String valores)> GetModeloFiltros(Int64? Id_Modelo_PainelServico, Venda_DataTableRequest request)
        {
            List<(Filtro_PainelVendaEnum tipo_filtro, String valores)> retorno = new List<(Filtro_PainelVendaEnum tipo_filtro, String valores)>();

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Modelo_PainelVenda> repository = new Conexao<Modelo_PainelVenda>(session))
            {
                Modelo_PainelVenda modelo = repository.selectFirst(p => p.Id == Id_Modelo_PainelServico).Result;

                if (modelo != null)
                {
                    IEnumerable<Filtro_PainelVendaEnum> filtros = modelo.Filtros.Select(p => p.Filtro).Distinct();

                    retorno.AddRange(filtros.Select(p =>
                    {
                        return (p, "(" + String.Join(" OR ", modelo.Filtros.Where(y => y.Filtro == p).Select(y =>
                        {
                            if (y.Data.HasValue)
                            {
                                return $"(COLUNA {Painel_ServicosApiController.SqlModeloPainelData(y.Data.Value)})";
                            }

                            return $"COLUNA = '{y.Valor}'";
                        }).ToList()) + ")");
                    }));
                }

            }

            foreach (PropertyInfo prop in request.GetType().GetProperties())
            {
                Venda_ModeloAttribute attr = prop.GetCustomAttribute<Venda_ModeloAttribute>();

                if (attr != null)
                {
                    if (prop.Name == "Status" || prop.Name == "EtapaVenda")
                    {
                        var statusQuery = String.Join(",", Convert.ToString(prop.GetValue(request)).Split(',').Where(p => string.IsNullOrEmpty(p) == false).Select(p => $"'{p}'"));

                        if (string.IsNullOrEmpty(statusQuery) == false)
                        {
                            retorno.Add((attr.Recurso, $"(COLUNA IN ({statusQuery}))"));
                        }
                    }

                    else
                    {
                        if (prop.GetValue(request) != null)
                        {

                            retorno.Add((attr.Recurso, $"(COLUNA = '{Convert.ToString(prop.GetValue(request))}')"));

                        }
                    }
                }
            }
            return retorno;
        }

        private async Task<VendaDadosGeraisViewModel> GetViewObj(Venda venda, Cliente cliente, ISession session)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            //using (ISession session = HibernateUtil.GetSession())

            using (Conexao<Documento_ProducaoAniel> repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<Elemento_Rede_Porta> repoElemento_Rede_Porta = new Conexao<Elemento_Rede_Porta>(sessionT5Nh))
            using (Conexao<Atividade_StatusVenda> repo_AtividadeStatus = new Conexao<Atividade_StatusVenda>(session))
            using (Conexao<Workflow_VendasAtividade> repo_Atividade = new Conexao<Workflow_VendasAtividade>(session))
            using (Conexao<Venda_Ultimo_Contato> repo_VendaContato = new Conexao<Venda_Ultimo_Contato>(session))
            using (Conexao<Model.nhibernate.LoginRadius> repo_LoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (Conexao<Modelo_Contrato> repo_ModeloContrato = new Conexao<Modelo_Contrato>(session))
            using (Conexao<Uf> repo_Uf = new Conexao<Uf>(session))
            using (Conexao<DocTermoAnexo> repoDocAceite = new Conexao<DocTermoAnexo>(session))
            using (Conexao<Forma_Pagamento_Cliente> repoFormaPgtoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
            using (Conexao<Forma_Pagamento> repoFormaPgto = new Conexao<Forma_Pagamento>(session))
            {
                VendaDadosGeraisViewModel retorno = new VendaDadosGeraisViewModel();
                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                //Elemento_Rede_Porta portaRese = (await repoElemento_Rede_Porta.selectFirst());



                //LoginRadius login = (await repo_LoginRadius.selectFirst());

                retorno.Id = venda.Id;

                retorno.Id_Vendedor = venda.Vendedor != null ? venda.Vendedor.Id : (Int64?)null;
                retorno.Id_Supervisor = venda.Supervisor != null ? venda.Supervisor.Id : (Int64?)null;
                retorno.Identificacao_Cliente = cliente?.Numero_Contrato;
                retorno.Tipo_Cliente = cliente.Tipo_Cliente.Value;
                retorno.Id_Venda = venda.Id;
                retorno.Nome_Vendedor = venda?.Vendedor?.Nome;

                retorno.End_CadastroCondominioPrincipal = venda.End_CadastroCondominioPrincipal;
                retorno.End_CadastroCondominioInstalacao = venda.End_CadastroCondominioInstalacao;
                retorno.End_CadastroCondominioProspecto = cliente.End_CadastroCondominioProspecto;
                retorno.End_CadastroCondominioCobranca = venda.End_CadastroCondominioCobranca;

                retorno.Nome_Supervisor = venda?.Supervisor?.Nome;
                retorno.DataVenda = venda?.Data_Venda?.ToString("dd/MM/yyyy");
                retorno.DataCriacao = venda?.DataHoraCriacao?.ToString("dd/MM/yyyy");
                retorno.DataEncerramento = venda?.Data_Venda?.ToString("dd/MM/yyyy");
                retorno.Justificativa_Avaliacao_Comercial = venda?.Motivo_Desistencia_Venda != null ? (venda?.Motivo_Desistencia_Venda?.Descricao ?? venda?.Motivo_Desistencia_Venda?.Descricao_Outro) : venda?.Justificativa_Avaliacao_Comercial;
                retorno.Id_Departamento = venda.Departamento?.Id;
                retorno.Status_Liberacao = venda.Status_Liberacao;
                retorno.Id_Campanha_Venda = venda.Campanha?.Id;

                if (venda.Workflow_VendasAtividade != null)
                {
                    retorno.Workflow_VendasAtividade = venda.Workflow_VendasAtividade;
                    retorno.Workflow_Vendas = venda.Workflow_Vendas;

                    retorno.Status = venda.Workflow_VendasAtividade.Ativ_StatusVenda.StatusPadrao.Value != venda.Status ? venda.Status : venda.Workflow_VendasAtividade.Ativ_StatusVenda.StatusPadrao.Value;
                    if (venda.Workflow_VendasAtividade.Ativ_NAtivAprovada.HasValue)
                    {
                        Workflow_VendasAtividade workflowAtivAprovacao = await repo_Atividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivAprovada);

                        retorno.AprovarEtapaWorkflowAprovaVenda =
                            workflowAtivAprovacao?.Ativ_StatusVenda?.StatusPadrao != null &&
                            workflowAtivAprovacao?.Ativ_StatusVenda?.StatusPadrao.Value == Status_Venda.Venda_Aprovada;

                    }

                    if (venda.Workflow_VendasAtividade.Ativ_NAtivReprovada.HasValue)
                    {
                        Workflow_VendasAtividade workflowAtivReprovacao = await repo_Atividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivReprovada);

                        retorno.ReprovarEtapaWorkflowCancelaVenda =
                            workflowAtivReprovacao?.Ativ_StatusVenda?.StatusPadrao != null &&
                            workflowAtivReprovacao?.Ativ_StatusVenda?.StatusPadrao.Value == Status_Venda.Venda_Cancelada;
                    }
                }
                else
                {
                    retorno.Status = venda.Status;
                }

                retorno.Descricao_WF = venda.Workflow_Vendas?.Descricao;
                retorno.Id_Workflow_Vendas = venda.Workflow_Vendas?.Id;
                retorno.Id_Setor_Departamento = Convert.ToString(venda.Id_Setor_Departamento?.Id);
                retorno.Id_Responsavel = venda.Id_Responsavel?.Id;


                if (retorno.Id_Workflow_Vendas != null)
                {
                    retorno.StatusVenda = venda.Workflow_VendasAtividade?.Ativ_StatusVenda.Descricao;
                }
                else
                {
                    retorno.StatusVenda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status);
                    //    long desc_ativ_status = Convert.ToInt64(venda.Workflow_VendasAtividade?.Ativ_StatusVenda.StatusPadrao);
                    //    //retorno.StatusVenda = venda.StatusVenda;

                    //    retorno.StatusVenda = desc_ativ_status != 0 ? repo_AtividadeStatus.selectFirst(p => p.Id == desc_ativ_status).Result?.Descricao : null;
                }

                retorno.StatusAtividadeAprovada = venda.Workflow_VendasAtividade?.Ativ_NAtivAprovada != null ? repo_Atividade.selectFirst(p => p.Id == (long)venda.Workflow_VendasAtividade.Ativ_NAtivAprovada).Result?.Ativ_StatusVenda?.StatusPadrao : (Status_Venda?)null;
                retorno.StatusAtividadeReprovada = venda.Workflow_VendasAtividade?.Ativ_NAtivReprovada != null ? repo_Atividade.selectFirst(p => p.Id == (long)venda.Workflow_VendasAtividade.Ativ_NAtivReprovada).Result?.Ativ_StatusVenda?.StatusPadrao : (Status_Venda?)null;
                retorno.Prazo_Atividade = venda.Workflow_VendasAtividade?.Ativ_Prazo_Minutos;
                retorno.Decorrido_Atividade = venda.Data_Alteracao_Status.HasValue ? (DateTime.Now - venda.Data_Alteracao_Status.Value).ToString(@"hh\:mm") : (venda.DataHoraCriacao.HasValue ? (DateTime.Now - venda.DataHoraCriacao.Value).ToString(@"hh\:mm") : null);
                retorno.Decorrido_Workflow_Vendas = venda.DataHoraCriacao.HasValue ? (DateTime.Now - venda.DataHoraCriacao.Value).ToString(@"hh\:mm") : null;
                retorno.Prazo_Workflow_Vendas = venda.Workflow_Vendas?.Prazo;
                retorno.Atividade_Decisoria = venda.Workflow_VendasAtividade?.Ativ_Decisao;
                retorno.InformacoesComplementares = venda.InformacoesComplementares;
                retorno.Id_Forma_Pagamento = venda?.FormaPagamentoCliente?.Id;

                if (!String.IsNullOrEmpty(config.Padrao_Login) && !String.IsNullOrEmpty(config.Padrao_Login_Ultimo))
                {
                    string primeiroNome = String.Empty;
                    string ultimoNome = String.Empty;

                    int idxPrimeiroEspaco = cliente.NomeFantasia != null ? cliente.NomeFantasia.IndexOf(" ") : 0;
                    int idxUltimoEspaco = cliente.NomeFantasia != null ? cliente.NomeFantasia.LastIndexOf(" ") : 0;

                    // Se o nome tem espaço preenche o primeiro e o último nome
                    if (idxPrimeiroEspaco != -1)
                    {
                        primeiroNome = cliente.NomeFantasia != null ? cliente.NomeFantasia.Substring(0, idxPrimeiroEspaco) : null;
                        ultimoNome = cliente.NomeFantasia != null ? cliente.NomeFantasia.Substring(idxUltimoEspaco + 1, cliente.NomeFantasia.Length - idxUltimoEspaco - 1) : null;
                    }
                    // Caso o nome não tenha nenhum espaço preenche somente o primeiro nome
                    else
                    {
                        primeiroNome = cliente.NomeFantasia;
                    }

                    var planoModel = cliente?.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id).OrderByDescending(x => x.Id).ToList();
                    var numero_contrato = planoModel?.Count > 0 ? planoModel?.FirstOrDefault().ClienteContrato?.NumeroContrato : String.Empty;
                    retorno.Login = (config.Padrao_Login.Replace("PrimeiroNome", primeiroNome?.ToLowerInvariant())
                                                        .Replace("UltimoNome", ultimoNome?.ToLowerInvariant())
                                                        .Replace("Contrato", cliente?.Numero_Contrato)
                                                        .Replace("Cont.PlanoseProdutos", planoModel?.FirstOrDefault()?.ClienteContrato?.Id.ToString())
                                                        .Replace("Código", Convert.ToString(cliente?.Id))
                                                        .Replace("Email", cliente?.Email_Principal?.ToLowerInvariant())
                                                        .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                        .Replace("Uf", cliente.Endereco_Principal?.UF?.ToLowerInvariant())
                                                        .Replace("Cidade", cliente.Endereco_Principal?.Cidade?.ToLowerInvariant()));

                    var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(retorno.Login);

                    retorno.Login = verificaLoginComSequencial;

                    retorno.Senha = (config.Padrao_Login_Ultimo.Replace("PrimeiroNome", primeiroNome.ToLowerInvariant())
                                                             .Replace("UltimoNome", ultimoNome.ToLowerInvariant())
                                                             .Replace("Contrato", cliente?.Numero_Contrato)
                                                             .Replace("Cont.PlanoseProdutos", planoModel?.FirstOrDefault()?.ClienteContrato?.Id.ToString())
                                                             .Replace("Código", Convert.ToString(cliente?.Id))
                                                             .Replace("Email", cliente?.Email_Principal?.ToLowerInvariant())
                                                             .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                             .Replace("Uf", cliente.Endereco_Principal?.UF?.ToLowerInvariant())
                                                             .Replace("Cidade", cliente.Endereco_Principal?.Cidade?.ToLowerInvariant()));


                    var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(retorno.Senha);

                    retorno.Senha = verificaSenhaComSequencial;
                }

                IEnumerable<string> nome = cliente.NomeFantasia != null ? cliente.NomeFantasia.Split(' ') : null;
                string utimoNome = nome != null ? nome.Last() : null;

                retorno.IdAtendimento = venda.IdAtendimento;
                retorno.StatusAuditoria = venda.StatusAuditoria;

                retorno.Cargo_Responsavel = cliente?.Cargo_Responsavel;
                retorno.CPFResponsavel = cliente?.Cpf_Responsavel;
                retorno.Representante = cliente?.Nome_Resp_Financeiro;
                retorno.Inscricao_Municipal = cliente?.Inscricao_Municipal;
                retorno.Inscricao_Estadual = cliente?.Inscricao_Estadual;

                retorno.Porte = cliente?.Porte;
                retorno.Data_Abertura = cliente?.Data_Abertura_Empresa;
                retorno.NomeFantasia = cliente?.NomeFantasia;
                retorno.Ramo_Atividade = cliente?.Ramo_Atividade;
                retorno.Razao_Social = cliente?.Razao_Social;
                //retorno.Porta_Reservada = portaRese?.Status != Sinapse.Model.enumerados.Situacao_Porta.Reservado ? "Não" : "Sim";
                //retorno.Forma_Envio = cliente?.Forma_Envio;

                //retorno.Ultimo_Contato = venda?.Data_Ultimo_Contato;
                retorno.Ultimo_Contato = repo_VendaContato.selectFirst(p => p.Id_Venda == venda.Id).Result?.Data_Ultimo_Contato;

                if (retorno.Ultimo_Contato != null)
                {
                    int tempo = (DateTime.Now - retorno.Ultimo_Contato).Value.Days;
                    retorno.Tempo_Ultimo_Contato = tempo == 0 ? "Hoje" : String.Concat(tempo.ToString(), " ", "Dias");
                }

                retorno.Id_Cliente = cliente.Id;
                retorno.CpfCnpj = cliente?.CpfCnpj;
                retorno.NomeCompleto = cliente.NomeFantasia;
                retorno.DataNascimento = Convert.ToString(cliente?.Data_Nascimento);
                retorno.Nacionalidade = cliente?.Nacionalidade;

                retorno.Sexo = cliente?.Sexo;
                retorno.EstadoCivil = cliente?.Estado_Civil;
                retorno.RG = cliente?.Rg;
                retorno.DataEmissao = Convert.ToString(cliente?.Data_Emissao_Rg);
                retorno.Orgao = cliente?.Orgao_Emissao_Rg;

                retorno.Instrucao = cliente?.Grau_Instrucao;
                retorno.Profissao = cliente?.Profissao;
                retorno.Pai = cliente?.Filiacao_Pai;
                retorno.Mae = cliente?.Filiacao_Mae;
                retorno.Qual_Meio = cliente?.Outro_Meio;
                retorno.TipoPessoa = cliente?.Tipo_Pessoa;

                retorno.End_Instalacao_Igual_Principal = venda.Status == Status_Venda.Analise_Cliente && config.EnderecoInstalacaoMesmoPrincipal ? true : cliente.End_Instalacao_Igual_Principal;

                retorno.Email = cliente?.Email_Principal;
                retorno.TelefoneFixo = cliente?.Telefone_Fixo;
                retorno.TelefoneCelular = cliente?.Telefone_Celular;
                retorno.TelefoneRecado = cliente?.Telefone_Recado;
                retorno.NomeRecado = cliente?.Pessoa_Tel_Recado;
                //retorno.Desc_Banco = cliente?.Banco?.Descricao;

                retorno.Quem_Indicou = cliente?.Quem_Indicou?.NomeFantasia;

                retorno.Id_Categoria_Cliente = cliente?.Categoria_Cliente?.Id;
                retorno.Id_Meio_Captacao = cliente?.Qual_Meio?.Id;
                retorno.Meio_Captacao = cliente?.Qual_Meio?.Descricao;
                retorno.Id_Quem_Indicou = cliente?.Quem_Indicou?.Id;
                retorno.Quem_Indicou = Convert.ToString(cliente?.Quem_Indicou?.Id);

                if (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto &&
                    cliente?.Endereco_Principal?.Logradouro == null &&
                    cliente?.Endereco_Principal?.Endereco_Logradouro == null)
                {
                    retorno.End_CadastroCondominioPrincipal = cliente.End_CadastroCondominioProspecto;
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                        Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                        Bairro = cliente?.Endereco_Prospecto?.Bairro,
                        CEP = cliente?.Endereco_Prospecto?.Cep,
                        Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                        Cidade = cliente?.Endereco_Prospecto?.Cidade,
                        Complemento = cliente?.Endereco_Prospecto?.Complemento,
                        Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                        Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                        Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                        Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                        Numero = cliente?.Endereco_Prospecto?.Numero,
                        Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Principal,
                        UF = cliente?.Endereco_Prospecto?.UF,
                        Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,

                        Cod_Escritorio = cliente?.Endereco_Prospecto?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Prospecto?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Prospecto?.Cod_Localidade),

                        Id_Cliente = cliente?.Id
                    };
                }
                else
                {
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente?.Endereco_Principal?.Pais?.Id,
                        Id_Tipo_Imovel = cliente?.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Id_Bairro = cliente?.Endereco_Principal?.Id_Bairro,
                        Bairro = cliente?.Endereco_Principal?.Bairro,
                        CEP = cliente?.Endereco_Principal?.Cep,
                        Id_Cidade = cliente?.Endereco_Principal?.Id_Cidade,

                        Cidade = cliente?.Endereco_Principal?.Cidade,
                        Complemento = cliente?.Endereco_Principal?.Complemento,
                        Id_Endereco = cliente?.Endereco_Principal != null ? cliente.Endereco_Principal.Id : (Int64)0,
                        Latitude = Convert.ToString(cliente?.Endereco_Principal?.Latitude),
                        Logradouro = cliente?.Endereco_Principal?.Logradouro,
                        Longitude = Convert.ToString(cliente?.Endereco_Principal?.Longitude),

                        Numero = cliente?.Endereco_Principal?.Numero,
                        Ponto_Referencia = cliente?.Endereco_Principal?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Principal,
                        UF = cliente?.Endereco_Principal?.UF,
                        Busca_Endereco = cliente?.Endereco_Principal?.Busca_Endereco != null ? cliente?.Endereco_Principal?.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Principal?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Principal?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Principal?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Principal?.Apartamento,
                        Cod_Escritorio = cliente?.Endereco_Principal?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Principal?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Principal?.Cod_Localidade),

                        Id_Cliente = cliente?.Id
                    };

                }

                retorno.EnderecoProspecto = new EnderecoClienteViewModel
                {
                    Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                    Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                    Bairro = cliente?.Endereco_Prospecto?.Bairro,
                    CEP = cliente?.Endereco_Prospecto?.Cep,
                    Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                    Cidade = cliente?.Endereco_Prospecto?.Cidade,
                    Complemento = cliente?.Endereco_Prospecto?.Complemento,
                    Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                    Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                    Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                    Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                    Numero = cliente?.Endereco_Prospecto?.Numero,
                    Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Principal,
                    UF = cliente?.Endereco_Prospecto?.UF,
                    Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                    Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                    Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                    Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,
                    Cod_Escritorio = cliente.Endereco_Prospecto?.Cod_Escritorio,
                    Cod_Area = cliente.Endereco_Prospecto?.Cod_Area,
                    Cod_Localidade = Convert.ToString(cliente.Endereco_Prospecto?.Cod_Localidade),

                    Id_Cliente = cliente?.Id
                };

                if (venda.Id == 0)
                {
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Tipo = Tipo_EnderecoCliente.Instalacao,

                        Cod_Escritorio_Prospect = retorno.EnderecoProspecto.Cod_Escritorio,
                        Cod_Area_Prospect = retorno.EnderecoProspecto.Cod_Area,
                        Cod_Localidade_Prospect = retorno.EnderecoProspecto.Cod_Localidade,
                    };
                }
                else
                {
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Id_Pais = venda.EnderecoInstalacao?.Pais?.Id,
                        Id_Tipo_Imovel = venda.EnderecoInstalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Id_Bairro = venda.EnderecoInstalacao?.Id_Bairro,
                        Bairro = venda.EnderecoInstalacao?.Bairro,
                        CEP = venda.EnderecoInstalacao?.Cep,
                        Id_Cidade = venda.EnderecoInstalacao?.Id_Cidade,

                        Cidade = venda.EnderecoInstalacao?.Cidade,
                        Complemento = venda.EnderecoInstalacao?.Complemento,
                        Id_Endereco = venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Id : (Int64)0,
                        Latitude = Convert.ToString(venda.EnderecoInstalacao?.Latitude),
                        Logradouro = venda.EnderecoInstalacao?.Logradouro,
                        Longitude = Convert.ToString(venda.EnderecoInstalacao?.Longitude),
                        Numero = venda.EnderecoInstalacao?.Numero,
                        Ponto_Referencia = venda.EnderecoInstalacao?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Instalacao,
                        UF = venda.EnderecoInstalacao?.UF,
                        Busca_Endereco = venda.EnderecoInstalacao?.Busca_Endereco,
                        Id_Endereco_Logradouro = venda.EnderecoInstalacao?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = venda.EnderecoInstalacao?.Condominio_Predio?.Id,
                        Id_Bloco = venda.EnderecoInstalacao?.Bloco,
                        Id_Apartamento = venda.EnderecoInstalacao?.Apartamento,

                        Cod_Escritorio = venda.EnderecoInstalacao?.Cod_Escritorio,
                        Cod_Area = venda.EnderecoInstalacao?.Cod_Area,
                        Cod_Localidade = Convert.ToString(venda.EnderecoInstalacao?.Cod_Localidade),

                        Id_Cliente = venda?.Id_Cliente
                    };
                }
                var formasPagamentoCliente = cliente.Formas_Pagamento_Cliente.Where(x => x.Status == Status_Default.Ativo).ToList();
                retorno.Formas_Pagamento = formasPagamentoCliente.Select(f => new Forma_Pagamento_ClienteModels
                {
                    Id = f.Id,
                    Id_Cliente = f.Cliente?.Id,
                    Id_Forma_Pagamento = f.Forma_Pagamento?.Id,
                    Desc_Forma_Pagamento = f.Forma_Pagamento?.Descricao,

                    Forma_Envio = f.Forma_Envio,
                    Desc_Forma_Envio = f.Forma_Envio?.GetDisplayName(),
                    Id_Vencimento = f.Vencimento?.Id,
                    Id_Banco = f?.Banco?.Id,

                    Desc_Banco = !String.IsNullOrEmpty(f?.Banco?.Descricao) ? f?.Banco?.Descricao : "-",
                    Agencia = !String.IsNullOrEmpty(f.Agencia) ? f.Agencia : "-",
                    Conta_Corrente = !String.IsNullOrEmpty(f.Conta_Corrente) ? f.Conta_Corrente : "-",
                    Digito = !String.IsNullOrEmpty(f.Digito) ? f.Digito : "-",
                    Dia_Vencimento = f.Dia_Vencimento,

                    End_Cobranca_Igual_Principal = f.End_Cobranca_Igual_Principal,
                    Id_EnderecoCobranca = f.Endereco_Cobranca?.Id,
                    Id_Tipo_ImovelCobranca = f.Endereco_Cobranca?.Tipo_Imovel?.Id,
                    Id_PaisCobranca = f.Endereco_Cobranca?.Pais?.Id,
                    LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),

                    LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),

                    CEPCobranca = f.Endereco_Cobranca?.Cep,
                    LogradouroCobranca = f.Endereco_Cobranca?.Logradouro,
                    NumeroCobranca = f.Endereco_Cobranca?.Numero,
                    BairroCobranca = f.Endereco_Cobranca?.Bairro,
                    Id_BairroCobranca = f.Endereco_Cobranca?.Id_Bairro,
                    CidadeCobranca = f.Endereco_Cobranca?.Cidade,

                    Id_CidadeCobranca = f.Endereco_Cobranca?.Id_Cidade,
                    UFCobranca = f.Endereco_Cobranca?.UF,
                    ComplementoCobranca = f.Endereco_Cobranca?.Complemento,
                    Ponto_ReferenciaCobranca = f.Endereco_Cobranca?.Ponto_Referencia,
                    Status = f.Status,
                    Tipo_Forma_Pagamento = f.Forma_Pagamento?.Tipo_Forma_Pagamento,
                    Selecionado = venda?.FormaPagamentoCliente?.Id == f?.Id

                }).ToList();

                //var formaPgto = repoFormaPgto.selectFirst(p => p.Id > 0).Result;
                //var formaPgtoVenda = repoFormaPgtoCliente.select(p => p.Cliente.Id == venda.Id_Cliente).Result;
                //if (venda.IdFormaPagamento == null)
                //{
                //    retorno.FormaPagamentoVenda = new Forma_Pagamento_ClienteModels
                //    {
                //        Id_Cliente = retorno.Id_Cliente,
                //        Id_Forma_Pagamento = formaPgto.Id,
                //        Desc_Forma_Pagamento = formaPgto.Descricao,
                //        Forma_Envio = venda.Forma_Envio,
                //        Id_EnderecoCobranca = retorno.EnderecoCobranca != null ? retorno.EnderecoCobranca.Id : 0,
                //        Id_Vencimento = retorno.Id_Vencimento

                //    };
                //}
                //else
                //{
                //    Forma_Pagamento_ClienteModels formaPagtoCliente = new Forma_Pagamento_ClienteModels();

                //    foreach (var item in formaPgtoVenda)
                //    {
                //        formaPagtoCliente.Id_Cliente = item.Cliente.Id;
                //        formaPagtoCliente.Id_Vencimento = item.Vencimento.Id;
                //        formaPagtoCliente.Id_Forma_Pagamento = item.Forma_Pagamento.Id;
                //        formaPagtoCliente.Desc_Forma_Pagamento = item.Forma_Pagamento.Descricao != null ? item.Forma_Pagamento.Descricao : String.Empty;
                //        formaPagtoCliente.Forma_Envio = item.Forma_Envio != null ? item.Forma_Envio : Cliente_FormaEnvio.Email;
                //        formaPagtoCliente.Desc_Forma_Envio = item.Forma_Envio != null ? item.Forma_Envio?.GetDisplayName() : String.Empty;
                //    }

                //    retorno.FormaPagamentoVenda = formaPagtoCliente;
                //}

                //Descontos da Venda Específica
                IEnumerable<String> ContratosEspecificosVenda = cliente?.PlanosProdutos.Where(p => p.Id_Venda == retorno.Id && !p.isPlanoAdicional).Select(p => p.ClienteContrato?.NumeroContrato).ToList();
                IEnumerable<Cliente_Desconto> DescontosEspecificosVenda = cliente?.Descontos.Where(p => ContratosEspecificosVenda.Contains(p.Numero_ContratoVinculado)).ToList();

                retorno.Descontos = (cliente?.Descontos.Count() == 0) ? new List<DescontoListaModel>() :

                DescontosEspecificosVenda.Select(p => new DescontoListaModel
                {
                    Id = p.Id,
                    Id_Desconto = p.Desconto != null ? p.Desconto.Id : 0,
                    Descricao = p.Desconto?.Descricao,
                    //Valor_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Valor_Desconto) : p.Desconto.Valores.Sum(x => x.Valor).Value.ToString(),// : p.Desconto.,
                    //Percentual_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Percentual_Desconto) : p.Desconto.Valores.Sum(x => x.Percentual).Value.ToString(),
                    Valor_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Valor_Desconto) : p.Desconto.Valores[0].Valor.Value.ToString(),
                    Percentual_Desconto = p.Desconto.Valor_Unico ? Convert.ToString(p?.Percentual_Desconto) : p.Desconto.Valores[0].Percentual.Value.ToString(),
                    Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao,
                    Numero_ContratoVinculado = p.Numero_ContratoVinculado,
                    Formato = !String.IsNullOrEmpty(Convert.ToString(p.Desconto?.Tipo_Desconto?.Formato)) ? Convert.ToString(Convert.ToInt64(p.Desconto?.Tipo_Desconto?.Formato)) : String.Empty
                }).ToList();

                RequestObjComboPai<Int64?, Int64?> request = new RequestObjComboPai<long?, long?>();
                request.idpai = cliente.Id;
                request.soativos = true;
                var listaAgrupada = await new ClienteContratoApiController().GetCombo(request);
                if (listaAgrupada.results.Count > 0)
                {
                    listaAgrupada.results.ForEach(x =>
                    {
                        retorno.ClienteContratos.Add(new ClienteContratoListModel
                        {
                            Id = x.Id,
                            Descricao = x.Text,
                            Numero_Contrato = Convert.ToString(x.Id)
                        });
                    });
                }

                IEnumerable<Cliente_PlanoProduto> listaPlanosProdutos = cliente?.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                //Pega Planos e produtos diferentes de cancelado
                retorno.Planos = new List<PlanoProdutoListModel>();

                foreach (Cliente_PlanoProduto planoProduto in listaPlanosProdutos)
                {
                    PlanoProdutoListModel planoProdutoModel = new PlanoProdutoListModel();

                    planoProdutoModel.Id_Modelo_Contrato = planoProduto.ClienteContrato?.ModeloContrato?.Id;
                    planoProdutoModel.Id_Contrato = planoProduto.ClienteContrato?.Id;
                    planoProdutoModel.Descricao_Contrato = planoProduto.ClienteContrato?.Descricao;
                    planoProdutoModel.Numero_Contrato = planoProduto.ClienteContrato?.NumeroContrato ?? "-";


                    planoProdutoModel.Id_Venda = venda?.Id;
                    planoProdutoModel.Id_Vendedor = venda?.Vendedor?.Id;
                    planoProdutoModel.Id_Supervisor = venda?.Supervisor?.Id;

                    planoProdutoModel.Status_Venda = venda?.Status;

                    planoProdutoModel.Desc_Status_Venda = venda != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status) : null;
                    planoProdutoModel.Nome_Vendedor = venda?.Vendedor?.Nome;
                    planoProdutoModel.Nome_Supervisor = venda?.Supervisor?.Nome;

                    planoProdutoModel.Id = planoProduto.Id;

                    planoProdutoModel.ValorPlanoProduto = planoProduto.Valor_Real.HasValue ? planoProduto.Valor_Real.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.ValorInstalacao_Final = planoProduto.Valor_Instalacao_Final.HasValue ? planoProduto.Valor_Instalacao_Final.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.ValorPlanoProduto_Final = planoProduto.Valor_Final.HasValue ? planoProduto.Valor_Final.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.Quantidade = planoProduto.Quantidade_Produto_Variavel.HasValue ? planoProduto.Quantidade_Produto_Variavel : 1;
                    planoProdutoModel.Valor_Total_Plano = planoProduto.Valor_Total_Plano_Variavel.HasValue ? planoProduto.Valor_Total_Plano_Variavel.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.Parcela = Convert.ToInt32(planoProduto?.Parcela_Entrada);
                    planoProdutoModel.Valor_Entrada = planoProduto.Valor_Entrada.HasValue ? planoProduto.Valor_Entrada.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.ValorInstalacao = planoProduto.Valor_Instalacao_Real.HasValue ? planoProduto.Valor_Instalacao_Real.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";

                    planoProdutoModel.ValorTotal = planoProduto.Valor_Final_Total.HasValue ? planoProduto.Valor_Final_Total.Value.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    planoProdutoModel.Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis;
                    planoProdutoModel.Meses_Carencia = planoProduto.Meses_Carencia;

                    planoProdutoModel.Status_PlanoProduto = planoProduto.Status;
                    planoProdutoModel.Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription(planoProduto.Status.Value) : String.Empty;

                    planoProdutoModel.Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao;
                    planoProdutoModel.End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal;
                    planoProdutoModel.End_Instalacao_Igual_Prospecto = venda.End_Instalacao_Igual_Prospecto;

                    planoProdutoModel.Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0;
                    planoProdutoModel.Periodo_Carencia = planoProduto.Periodo_Carencia;
                    planoProdutoModel.Login_Radius = planoProduto.Login_Radius;
                    planoProdutoModel.Senha_Radius = planoProduto.Senha_Radius;
                    planoProdutoModel.IdIpPool = planoProduto.Pool_Ip?.Id;
                    planoProdutoModel.Id_Endereco = planoProduto.Endereco_Instalacao.Id;

                    planoProdutoModel.Id_Bairro = planoProduto.Endereco_Instalacao?.Id_Bairro;
                    planoProdutoModel.Bairro = planoProduto.Endereco_Instalacao?.Bairro;
                    planoProdutoModel.CEP = planoProduto.Endereco_Instalacao?.Cep;
                    planoProdutoModel.Id_Cidade = planoProduto.Endereco_Instalacao?.Id_Cidade;
                    planoProdutoModel.Cidade = planoProduto.Endereco_Instalacao?.Cidade;
                    planoProdutoModel.Ponto_Referencia = planoProduto.Endereco_Instalacao?.Ponto_Referencia;

                    planoProdutoModel.Complemento = planoProduto.Endereco_Instalacao?.Complemento;
                    planoProdutoModel.Logradouro = planoProduto.Endereco_Instalacao?.Logradouro;
                    planoProdutoModel.Numero = planoProduto.Endereco_Instalacao?.Numero;
                    planoProdutoModel.UF = planoProduto.Endereco_Instalacao?.UF;

                    planoProdutoModel.Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude);
                    planoProdutoModel.Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude);
                    planoProdutoModel.Id_Pais = planoProduto.Endereco_Instalacao?.Pais?.Id;
                    planoProdutoModel.Id_Tipo_Imovel = planoProduto.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

                    planoProdutoModel.Cod_Escritorio = planoProduto.Cod_Escritorio;
                    planoProdutoModel.Cod_Area = planoProduto.Cod_Area;
                    planoProdutoModel.Cod_Localidade = planoProduto.Cod_Localidade;
                    planoProdutoModel.Localidade = planoProduto.Localidade;
                    planoProdutoModel.Localidade_Anatel = planoProduto.Localidade_Anatel;

                    planoProdutoModel.Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id;
                    planoProdutoModel.Id_Forma_Pagamento_Cliente = planoProduto.Forma_Pagamento_Cliente?.Id;

                    planoProdutoModel.Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento";

                    planoProdutoModel.Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null;
                    planoProdutoModel.FlagTrocaPlano = planoProduto.FlagTrocaPlano;
                    planoProdutoModel.FlagVendaFormatoAvulso = planoProduto.FlagVendaFormatoAvulso;

                    planoProdutoModel.CobrarInstalacaoEntrada = planoProduto.CobrarInstalacaoEntrada ? planoProduto.CobrarInstalacaoEntrada : false;


                    if (planoProduto.Combo != null)
                    {
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Combo.Id;
                        planoProdutoModel.Descricao = planoProduto.Combo.Descricao;
                        planoProdutoModel.Tipo = null;

                        if (planoProduto.Combo.PlanosProdutos.Any(x => x?.Plano?.Tipo_Tecnologia ==
                                                                        Tipo_Tecnologia.Internet))
                        {
                            planoProdutoModel.Tipo_Tecnologia = Tipo_Tecnologia.Internet;
                        }

                        // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                        planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                    }
                    else if (planoProduto.Plano != null)
                    {
                        planoProdutoModel.Tipo_Tecnologia = planoProduto?.Plano?.Tipo_Tecnologia;
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Plano.Id;
                        planoProdutoModel.Descricao = planoProduto.Plano.Descricao;
                        planoProdutoModel.Tipo = PlanoProdutoEnum.Plano;

                        if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                        }
                        else
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.NAO;
                        }

                        planoProdutoModel.ValorVendaAgrupada = planoProduto.Plano?.ValorVendaAgrupada > 0 ? planoProduto.Plano.ValorVendaAgrupada.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                        planoProdutoModel.UtilizaVendaAgrupada = planoProduto?.Plano?.UtilizaVendaAgroup ?? false;
                        planoProdutoModel.ValorPlanoOriginal = planoProduto.Plano?.ValorPlano > 0 ? planoProduto.Plano.ValorPlano.ToString("N2", new CultureInfo("pt-BR")) : "0,00";
                    }
                    else if (planoProduto.Produto != null)
                    {
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Produto.Id;
                        planoProdutoModel.Descricao = planoProduto.Produto.Descricao;
                        planoProdutoModel.Tipo = PlanoProdutoEnum.Produto;

                        if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                        }
                        else
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.NAO;
                        }
                    }


                    // TODO: Descobrir como pegar a OS de uma venda de combo
                    if (planoProduto.Plano != null || planoProduto.Produto != null)
                    {
                        using (Documento_ProducaoApiController apiDocProd = new Documento_ProducaoApiController())
                        {
                            int idClienteAniel = (await apiDocProd.GetClienteAniel(sessionT5Nh, planoProduto.Cliente)).Cod_Cliente;
                            long? idPlanoProdutoAniel = (await apiDocProd.GetPlanoProdutoAniel(sessionT5Nh, planoProduto)).FirstOrDefault()?.Id;

                            Documento_ProducaoAniel planoProdutoOS = (await repoDocProd.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == planoProduto.Id &&
                                                                                                    p.Id_Venda == venda.Id)); if (planoProdutoOS != null)
                            {
                                planoProdutoModel.PossuiOSCriada = RsBase.SIM;
                            }
                            else
                            {
                                planoProdutoModel.PossuiOSCriada = RsBase.NAO;
                            }

                            planoProdutoModel.NumOS = planoProdutoOS?.Numero_Obra_Original;
                        }
                    }
                    else if (planoProduto.Combo != null)
                    {
                        planoProdutoModel.PossuiOSCriada = RsBase.NAO;
                    }

                    planoProdutoModel.UtilizaVendaAgroup = planoProduto.UtilizaVendaAgroup;

                    planoProdutoModel.Id_Plano_Atrelado = planoProduto.Id_Plano_Atrelado;
                    planoProdutoModel.isPlanoAdicional = planoProduto.isPlanoAdicional;
                    planoProdutoModel.possuiPlanosAdicionais = planoProduto.possuiPlanosAdicionais;
                    planoProdutoModel.FlagPossuiValorAgrupado = false;

                    retorno.Planos.Add(planoProdutoModel);
                }

                retorno.Termo_Adesao_Assinado = venda.Termo_Adesao_Assinado;
                retorno.Exige_Termo_Adesao = config.Exige_Termo_Adesao;
                retorno.Exige_Termo_Contrato = config.Exige_Termo_Contrato;
                retorno.Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato;
                retorno.Exige_Foto_Termo = config.Exige_Foto_Termo;
                retorno.Permite_Work_Sem_Termo = config.Permite_Work_Sem_Termo;
                retorno.Mudanca_Plano = venda.Mudanca_Plano;

                #region Consulta de Crédito

                retorno.Consulta_Viabilidade_Realizada = venda.Consulta_Viabilidade_Realizada;

                #endregion

                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IClienteRepository repoClient = new ClienteRepository(session);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);

                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoClient, cliente.Id);

                IEnumerable<EtapaVendaViewModel> etapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);

                // Variável com a última etapa da venda para saber se devemos verificar se o checklist e o termo de adesão estão preenchidos.
                // O preenchimento dos dados das etapas da venda são preenchidos depois
                EtapaVendaViewModel ultimaEtapaVenda =
                    etapasVenda.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado && e.Etapa != 7) || e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente);

                // Caso todas as etapas tenham sido aprovadas a etapa atual será a última etapa da venda
                if (ultimaEtapaVenda == null)
                {
                    ultimaEtapaVenda = etapasVenda.LastOrDefault();
                }

                #region Checklist da venda

                if (venda.Id != 0)
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    Tipo_Pessoa tipoPessoa = cliente.Tipo_Pessoa.Value;
                    long? idCategoriaCliente = cliente.Categoria_Cliente?.Id;
                    long? idMeioCaptacao = cliente.Qual_Meio?.Id;

                    long? idUf = null;
                    long? idCidade = null;

                    if (config.Utiliza_Cadastro_Endereco)
                    {
                        idUf = venda.EnderecoInstalacao?.Endereco_Logradouro?.UF?.Id;
                        idCidade = venda.EnderecoInstalacao?.Endereco_Logradouro?.Cidade?.Id;
                    }
                    else
                    {
                        string siglaUf = venda.EnderecoInstalacao?.UF;

                        if (!String.IsNullOrEmpty(siglaUf))
                        {
                            idUf = repo_Uf.selectFirst(u => u.Sigla == siglaUf).Result?.Id;
                        }

                        idCidade = venda.EnderecoInstalacao?.Id_Cidade;
                    }

                    var possivelChecklist = repoChecklist.GetIdQuizParaVenda(tipoPessoa, idCategoriaCliente, idMeioCaptacao, idUf, idCidade);
                    if (possivelChecklist.HasValue)
                    {
                        retorno.PossuiChecklist = true;
                    }

                    if (!checklistBusiness.VendaHasChecklist(retorno.Id))
                    {
                        // Cria a checklist somente caso a etapa do checklist seja a etapa atual e ainda não tenha nenhum checklist
                        // associado a venda
                        if (ultimaEtapaVenda.Etapa == (int)Sinapse.Model.enumerados.EnumEtapaVenda.Checklist)
                        {
                            checklistBusiness.AssociarChecklistVenda(
                                retorno.Id,
                                tipoPessoa,
                                idCategoriaCliente,
                                idMeioCaptacao,
                                idUf,
                                idCidade
                            );

                            // Envia a checklist por e-mail para o cliente
                            if (config.EnviarCheckListAuto)
                            {
                                await EnviarEmailChecklist(retorno.Id);
                            }
                        }
                    }
                    else if (ultimaEtapaVenda.Etapa >= (int)Sinapse.Model.enumerados.EnumEtapaVenda.Checklist)
                    {
                        if (checklistBusiness.ChecklistPreenchido(retorno.Id))
                        {
                            etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.Checklist, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                        }
                        else
                        {
                            etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.Checklist, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                        }
                    }
                }

                #endregion

                #region Termo de Adesão

                if (retorno.Id != 0 && ultimaEtapaVenda.Etapa >= (int)Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao)
                {
                    if (venda.Termo_Adesao_Assinado)
                    {
                        etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao, (StatusEtapaVenda)ultimaEtapaVenda.Status);
                    }
                }

                #endregion

                // O preenchimento das etapas da venda devem sempre estar após a verificação do checklist e do termo de adesão
                // pois essas duas verificações podem alterar os status das etapas da venda.
                #region Etapa da Venda

                retorno.EtapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);

                EtapaVendaViewModel ultimaEtapa =
                    retorno.EtapasVenda.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado && e.Etapa != 7) || e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente);

                // Caso todas as etapas tenham sido aprovadas a etapa atual será a última etapa da venda
                if (ultimaEtapa == null)
                {
                    ultimaEtapa = retorno.EtapasVenda.LastOrDefault();
                }

                retorno.EtapaAtualVenda = ultimaEtapa.Etapa;

                retorno.EtapasVendasConcluidas =
                    retorno.EtapasVenda != null &&
                    retorno.EtapasVenda.Where(t => t.Etapa < 7 && t.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente).Count() == 0;

                #endregion

                #region Status das consultas de restrição

                IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(session);
                ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(session);
                IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(session);

                var consultaCreditoBusiness =
                new ConsultaCreditoBusiness(
                        repoConsultaCredito,
                        repoCriterios,
                        repoParamsConsulta,
                        repoConfig
                );

                IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(session);
                IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(session);

                var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(session, sessionT5Nh);

                var consultaViabilidadeBusiness =
                    new ConsultaViabilidadeBusiness(
                        repoConsultaViabilidade,
                        repoElemRede,
                        repoPorta,
                        repoConfig
                    );
                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaCredito))
                {
                    if (retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica)
                    {
                        retorno.StatusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCpf(cliente.CpfCnpj);

                        retorno.StatusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(cliente.CpfCnpj);
                    }
                    else
                    {
                        retorno.StatusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCnpj(cliente.CpfCnpj);

                        retorno.StatusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCnpj(cliente.CpfCnpj);
                    }
                }

                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaFinanceira))
                {
                    // Caso a consulta de pendência financeira não retorne pendência alguma o método GetStatusConsultaPendenciaFinanceiraByCpf() e
                    // GetStatusConsultaPendenciaFinanceiraByCnpj() retornam incorretamente que a consulta ainda está pendente pois não encontrou
                    // nenhum registro em banco.

                    // Para driblar esse caso, verificamos se a etapa da consulta de pendência financeira na venda
                    // já não está mais pendente e passamos a utilizar o status da etapa
                    int statusEtapaConsultaPendenciaFinanceira = retorno.EtapasVenda.First(p => p.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaFinanceira).Status;

                    if (retorno.StatusConsultaPendenciaFinanceira == StatusEtapaVenda.Pendente &&
                        statusEtapaConsultaPendenciaFinanceira != (int)StatusEtapaVenda.Pendente)
                    {
                        retorno.StatusConsultaPendenciaFinanceira = (StatusEtapaVenda)statusEtapaConsultaPendenciaFinanceira;
                    }
                }

                string logradouro;

                if (cliente.Endereco_Principal != null)
                {
                    if (config.Utiliza_Cadastro_Endereco)
                    {
                        logradouro =
                            cliente.Endereco_Principal?.Endereco_Logradouro != null ?
                                 cliente.Endereco_Principal.Endereco_Logradouro.Descricao :
                                 cliente.Endereco_Principal?.Condominio_Predio?.Descricao;
                    }
                    else
                    {
                        logradouro = cliente.Endereco_Principal?.Logradouro;
                    }
                }
                else
                {
                    if (config.Utiliza_Cadastro_Endereco)
                    {
                        logradouro =
                            cliente.Endereco_Prospecto?.Endereco_Logradouro != null ?
                                 cliente.Endereco_Prospecto.Endereco_Logradouro.Descricao :
                                 cliente.Endereco_Prospecto?.Condominio_Predio?.Descricao;
                    }
                    else
                    {
                        logradouro = cliente.Endereco_Prospecto?.Logradouro;
                    }
                }

                retorno.StatusConsultaPendenciaEndereco =
                    pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(logradouro, retorno.EnderecoProspecto.Numero);

                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaEndereco))
                {
                    // Caso a consulta de pendência de endereço não retorne pendência alguma o método GetStatusConsultaPendenciaEndereco() retorna
                    // incorretamente que a consulta ainda está pendente pois não encontrou nenhum registro em banco.

                    // Para driblar esse caso, verificamos se a etapa da consulta de pendência de endereço na venda
                    // já não está mais pendente e passamos a utilizar o status da etapa
                    int statusEtapaConsultaPendenciaEndereco = retorno.EtapasVenda.First(p => p.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaEndereco).Status;

                    if (retorno.StatusConsultaPendenciaEndereco == StatusEtapaVenda.Pendente &&
                        statusEtapaConsultaPendenciaEndereco != (int)StatusEtapaVenda.Pendente)
                    {
                        retorno.StatusConsultaPendenciaEndereco = (StatusEtapaVenda)statusEtapaConsultaPendenciaEndereco;
                    }
                }

                if (!String.IsNullOrEmpty(venda.MotivoAprovManualViabilidade))
                {
                    retorno.StatusConsultaViabilidade = StatusEtapaVenda.Aprovado;
                }
                else
                {
                    retorno.StatusConsultaViabilidade =
                        consultaViabilidadeBusiness.GetStatusConsultaViabilidade(
                            (venda.EnderecoInstalacao?.Latitude).GetValueOrDefault(0),
                            (venda.EnderecoInstalacao?.Longitude).GetValueOrDefault(0)
                        );
                }


                retorno.ConsultaViabilidadeRestringeVenda = config.ConsultaViabilidadeRestringeVenda;
                retorno.ConsultaPendenciaEnderecoRestringeVenda = config.ConsultaPendenciaEnderecoRestringeVenda;
                retorno.ConsultaPendenciaFinanceiraRestringeVenda = config.ConsultaPendenciaFinanceiraRestringeVenda;
                retorno.ConsultaCreditoRestringeVenda = config.ConsultaCreditoRestringeVenda;

                retorno.ConsultaViabilidadeVendedorVisualiza = config.ConsultaViabilidadeVendedorVisualiza;
                retorno.ConsultaPendenciaEnderecoVendedorVisualiza = config.ConsultaPendenciaEnderecoVendedorVisualiza;
                retorno.ConsultaPendenciaFinanceiraVendedorVisualiza = config.ConsultaPendenciaFinanceiraVendedorVisualiza;
                retorno.ConsultaCreditoVendedorVisualiza = config.ConsultaCreditoVendedorVisualiza;

                retorno.TermosAceite = (await repoDocAceite.select(p => p.Venda.Id == venda.Id)).ToList();

                #endregion

                return retorno;
            }
        }

        private void BindViewToObjVenda(ISession session, Venda objVenda, Cliente cliente, VendaDadosGeraisViewModel view, String Usuario)
        {
            using (var clienteApi = new ClienteApiController())
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoDepartamento = new Conexao<Departamento>(session))
            using (var repoCampanha = new Conexao<Campanha>(session))
            {
                objVenda.Id_Cliente = view.Id_Cliente;
                objVenda.StatusVenda = view.StatusVenda;
                objVenda.Vendedor = view.Id_Vendedor.HasValue ? repoFuncionario.selectFirst(p => p.Id == view.Id_Vendedor.Value).Result : null;

                objVenda.Departamento = view.Id_Departamento.HasValue ? repoDepartamento.selectFirst(p => p.Id == view.Id_Departamento.Value).Result : null;
                objVenda.Campanha = view.Id_Campanha.HasValue ? repoCampanha.selectFirst(p => p.Id == view.Id_Campanha.Value).Result : null;
                objVenda.Supervisor = view.Id_Supervisor.HasValue ? repoFuncionario.selectFirst(p => p.Id == view.Id_Supervisor.Value).Result : null;
                objVenda.Justificativa_Avaliacao_Comercial = view.Justificativa_Avaliacao_Comercial;
                objVenda.Consulta_Viabilidade_Realizada = view.Consulta_Viabilidade_Realizada;

                if (view.Id == 0)
                {
                    objVenda.Status = Status_Venda.Analise_Cliente;
                }
                else
                {
                    objVenda.Status = view.Status;
                }

                if (!String.IsNullOrEmpty(view.DataEncerramento))
                    objVenda.Data_Venda = DateTime.ParseExact(view.DataEncerramento, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                else
                    objVenda.Data_Venda = null;

                //*************************************************************
                // Refatorado Cliente sendo repassado para ClienteApiController
                //*************************************************************

                cliente = clienteApi.BindDadosClienteVenda(new BindDadosClienteRequestModel
                {
                    Venda = view,
                    Usuario = Usuario
                }, session).Result;

                cliente.Endereco_Principal = clienteApi.BindEnderecosFromVenda(new BindEnderecoFromVendaRequestModel
                {
                    cliente = cliente,
                    tipoEndereco = Tipo_EnderecoCliente.Principal,
                    viewVenda = view,
                }, session).Result.First();

                if (objVenda.Id != 0)
                {
                    bool enderecoInstalacaoIgual =
                        objVenda.EnderecoInstalacao.Pais?.Id == view.EnderecoInstalacao?.Id_Pais &&
                        objVenda.EnderecoInstalacao.UF == view.EnderecoInstalacao?.UF &&
                        objVenda.EnderecoInstalacao.Id_Cidade == view.EnderecoInstalacao?.Id_Cidade &&
                        objVenda.EnderecoInstalacao.Cep == view.EnderecoInstalacao?.CEP &&
                        objVenda.EnderecoInstalacao.Logradouro == view.EnderecoInstalacao?.Logradouro &&
                        objVenda.EnderecoInstalacao.Numero == view.EnderecoInstalacao?.Numero &&
                        objVenda.EnderecoInstalacao.Cod_Escritorio == view.EnderecoInstalacao?.Cod_Escritorio &&
                        objVenda.EnderecoInstalacao.Cod_Area == view.EnderecoInstalacao?.Cod_Area &&
                        objVenda.EnderecoInstalacao.Cod_Localidade.GetValueOrDefault(0) == Convert.ToInt64(view.EnderecoInstalacao?.Cod_Localidade);

                    if (!enderecoInstalacaoIgual)
                    {
                        objVenda.EnderecoInstalacao = clienteApi.BindEnderecosFromVenda(new BindEnderecoFromVendaRequestModel
                        {
                            cliente = cliente,
                            tipoEndereco = Tipo_EnderecoCliente.Instalacao,
                            viewVenda = view,
                        }, session).Result.First();
                    }
                }
                else
                {
                    objVenda.EnderecoInstalacao = clienteApi.BindEnderecosFromVenda(new BindEnderecoFromVendaRequestModel
                    {
                        cliente = cliente,
                        tipoEndereco = Tipo_EnderecoCliente.Instalacao,
                        viewVenda = view,
                    }, session).Result.First();
                }
            }
        }

        private async void GravarEtapaVenda(ISession session, Venda venda, VendaDadosGeraisViewModel view, string usuario)
        {
            try
            {
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                var sessionLog = HibernateUtil.GetSessionLog();
                using (var repositoryEtapa = new Conexao<EtapaVenda>(session))
                using (var repositoryVenda = new Conexao<Venda>(session))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    bool existemEtapas = etapaVendaBusiness.ExistemEtapas(venda.Id);

                    EtapaVenda modelo = null;
                    EtapaVenda modeloAnterior = null;

                    if (view.Id == 0 || !existemEtapas)
                    {
                        foreach (EtapaVendaViewModel etapa in view.EtapasVenda)
                        {
                            EtapaVenda etapaVenda = new EtapaVenda
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = usuario,

                                Etapa = (Sinapse.Model.enumerados.EnumEtapaVenda)etapa.Etapa,
                                Descricao = etapa.Descricao,

                                // O status deve ser preenchido no input hidden que existe na view
                                Status = (Sinapse.Model.enumerados.StatusEtapaVenda)etapa.Status,
                                Venda = venda,
                            };

                            //Log de mudança de status da primeira etapa
                            if ((Sinapse.Model.enumerados.StatusEtapaVenda)etapa.Status != StatusEtapaVenda.Pendente)
                            {
                                modelo = (EtapaVenda)etapaVenda.Clone();
                                modelo.Status = StatusEtapaVenda.Pendente;
                                var logs = etapaVenda.GeraLog(modelo, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                            etapaVendaBusiness.Save(etapaVenda);
                        }
                    }
                    else
                    {
                        EtapaVendaViewModel etapaAtual = view.EtapasVenda.Where(e => e.Etapa == view.EtapaAtualVenda).FirstOrDefault();
                        //Log 
                        EtapaVenda etapaVenda = new EtapaVenda
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = usuario,
                            Etapa = (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa,
                            Descricao = etapaAtual.Descricao,
                            Status = (Sinapse.Model.enumerados.StatusEtapaVenda)etapaAtual.Status,
                            Venda = venda
                        };

                        modelo = await repositoryEtapa.selectFirst(p => p.Venda.Id == venda.Id && p.Etapa == (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa);

                        if (modelo != null)
                        {
                            modelo.Status = StatusEtapaVenda.Pendente;
                            var logs = etapaVenda.GeraLog(modelo, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            //var logs = modelo.GeraLog(etapaVenda, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }




                        if (etapaAtual != null)
                        {
                            etapaVendaBusiness.MudarStatusEtapa(venda.Id, (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa, (Sinapse.Model.enumerados.StatusEtapaVenda)etapaAtual.Status);
                        }
                    }

                    venda.Etapa_Atual = etapaVendaBusiness.VerificaUltimaEtapa(venda.Id);
                    await repositoryVenda.SaveOrUpdate(venda);
                }
            }

            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Método para asssimilação de valores da View
        /// </summary>
        /// <param name="session">Sessão gerada em outro método</param>
        /// <param name="objVenda">Model da Venda</param>
        /// <param name="objCliente">model do Cliente</param>
        /// <param name="view">Objeto da View</param>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        private async Task<List<PostApiMensagem>> BindViewToObj(ISession session, Venda objVenda, Cliente objCliente, VendaDadosGeraisViewModel view, String Usuario, ISession sessionLog)
        {

            IClienteRepository repoCliente = new ClienteRepository(session);
            IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(session);
            IVencimentoRepository repoVencimento = new VencimentoRepository(session);
            IFormaPagamentoRepository repoFormaPagamento = new FormaPagamentoRepository(session);
            IComboRepository repoCombo = new ComboRepository(session);
            ICampanhaRepository repoCampanha = new CampanhaRepository(session);
            IPlanoRepository repoPlano = new PlanoRepository(session);
            IProdutoRepository repoProduto = new ProdutoRepository(session);
            IPool_IpRepository repoPoolIp = new Pool_IpRepository(session); IVendaRepository repoVenda = new VendaRepository(session);
            IFuncionarioRepository repoFuncionario = new FuncionarioRepository(session);
            IDepartamentoRepository repoDepto = new DepartamentoRepository(session);

            IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
            IUsuarioRepository repoUsuario = new UsuarioRepository(session);
            ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(session);
            IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(session);

            IPaisRepository repoPais = new PaisRepository(session);
            IUfRepository repoUf = new UfRepository(session);
            ICidadeRepository repoCidade = new CidadeRepository(session);
            IBairroRepository repoBairro = new BairroRepository(session);

            ITipoImovelRepository repoTipoImovel = new TipoImovelRepository(session);
            IEnderecoLogradouroRepository repoEnderecoLogradouro = new EnderecoLogradouroRepository(session);
            ICondominioPredioRepository repoCondominioPredio = new CondominioPredioRepository(session);
            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
            IBancoRepository repoBanco = new BancoRepository(session);
            IClienteContratoRepository repoClienteContrato = new ClienteContratoRepository(session);

            IModeloContratoRepository repoModeloContrato = new ModeloContratoRepository(session);
            IClientePlanoProdutoRepository repoClientePlanoProduto = new ClientePlanoProdutoRepository(session);
            IComboPlanoProdutoRepository repoComboPlanoProdutoRepository = new ComboPlanoProdutoRepository(session);
            IWorkflowRepository repoWorkflowRepository = new WorkflowRepository(session);

            var vendaBusiness =
                new VendaBusiness(
                repoVenda,
                repoFuncionario,
                repoDepto,
                repoConfig,
                repoUsuario,
                repoCliente,
                repoEnderecoCliente,
                repoCategoriaCliente,
                repoMeioCaptacao,
                repoPais,
                repoUf,
                repoCidade,
                repoBairro,
                repoTipoImovel,
                repoEnderecoLogradouro,
                repoCondominioPredio,
                repoBanco,
                repoVencimento,
                repoFormaPagamento,
                repoCombo,
                repoCampanha,
                repoPlano,
                repoProduto,
                repoPoolIp,
                repoModeloContrato,
                repoClienteContrato,
                repoClientePlanoProduto,
                repoComboPlanoProdutoRepository,
                repoWorkflowRepository
                );

            // ************
            // Repositórios
            // ************
            using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
            using (Conexao<Configuracoes_Gerais> repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<IdentityUser> repoIdentityUser = new Conexao<IdentityUser>(session))
            using (Conexao<Forma_Pagamento_Cliente> repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
            using (DidApiController DidApi = new DidApiController())
            using (ClienteApiController ClienteApi = new ClienteApiController())
            using (CircuitoTelefoniaApiController CircuitoApi = new CircuitoTelefoniaApiController())
            using (Modelo_ContratoApiController ApiModelo = new Modelo_ContratoApiController())
            using (Conexao<Forma_Pagamento> repoFormaPgto = new Conexao<Forma_Pagamento>(session))
            {
                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                List<PostApiMensagem> retorno = new List<PostApiMensagem>();
                IdentityUser usuarioLogado = (await repoIdentityUser.selectFirst(p => p.Id == Usuario));
                Configuracoes_Gerais config = await repoConfigGerais.selectFirst();

                List<Forma_Pagamento_Cliente> forma_pagamento_clientetemp = objCliente.Formas_Pagamento_Cliente.ToList();

                foreach (Forma_Pagamento_ClienteModels forma_pagamento_clienteview in view.Formas_Pagamento.FindAll(p => p.Id == 0))
                {
                    Forma_Pagamento_Cliente formaPagamentoCliente =
                        vendaBusiness.BindFormaPagamentoCliente(objCliente, forma_pagamento_clienteview, view);

                    forma_pagamento_clientetemp.Add(formaPagamentoCliente);
                }
                //if (view.Formas_Pagamento.Where(p => p.Id != 0 && p.Selecionado).FirstOrDefault()?.Id != forma_pagamento_clientetemp.Where(p => p.Venda == objVenda.Id).FirstOrDefault()?.Id) 
                //{
                //    forma_pagamento_clientetemp.Where(p => p.Venda == objVenda.Id).First().Venda = null;
                //    forma_pagamento_clientetemp.Where(p => p.Id == view.Formas_Pagamento.Where(p => p.Id != 0 && p.Selecionado).FirstOrDefault().Id).First().Venda = objVenda.Id;
                //}

                //Refatorado a validação acima, pois o evento Where() e First() estava retornando erro no momento da consulta
                //A validação abaixo é a mesma, porém mais legivel para leitura
                var formaSelecionadaView = view.Formas_Pagamento.FirstOrDefault(p => p.Selecionado);
                var formaAtualCliente = forma_pagamento_clientetemp.FirstOrDefault(p => p.Venda == objVenda.Id);

                if (formaSelecionadaView?.Id != formaAtualCliente?.Id)
                {
                    if (formaAtualCliente != null)
                        formaAtualCliente.Venda = null;

                    var novaFormaCliente = forma_pagamento_clientetemp.FirstOrDefault(p => p.Id == formaSelecionadaView?.Id);

                    if (novaFormaCliente != null)
                        novaFormaCliente.Venda = objVenda.Id;
                }

                objCliente.Formas_Pagamento_Cliente = forma_pagamento_clientetemp;

                repoCliente.Update(objCliente);
                var formaSelecionadaBanco = objCliente.Formas_Pagamento_Cliente.FirstOrDefault(p => p.Venda == objVenda.Id);

                Boolean temPlanoProdutoNovo = false;

                //Pega Usuario do banco com Vendedor / Supervisor

                /****************************************
                  Planos e Produtos
                ****************************************/

                List<Cliente_PlanoProduto> planoProdutoTemp = objCliente.PlanosProdutos.ToList();

                List<PlanoProdutoListModel> ListaNovosPlanos = view.Planos.FindAll(p => p.Id == 0 || p.FlagPossuiValorAgrupado);

                List<CircuitoTelefonia> circuitoAvulso = new List<CircuitoTelefonia>();
                List<CircuitoTelefonia> circuitoCombo = new List<CircuitoTelefonia>();

                /****************************************
                    Planos e Produtos
                ****************************************/

                int comboIncremento = 1;
                var logs = new List<LogVendas>();
                Venda vendaClone = (Venda)objVenda.Clone();

                foreach (PlanoProdutoListModel planoProdutoView in ListaNovosPlanos)
                {
                    temPlanoProdutoNovo = true;

                    comboIncremento++;

                    var listaDescontosClientePlanoProduto = view.Descontos.Where(p => p.Numero_ContratoVinculado == planoProdutoView.Numero_Contrato).ToList();
                    Cliente_PlanoProduto planoProduto = vendaBusiness.BindPlanoProduto(objCliente, objVenda, planoProdutoView, planoProdutoView.UtilizaVendaAgrupada);


                    var isPagPlanProdCartao = planoProduto.Forma_Pagamento_Cliente != null ? planoProduto.Forma_Pagamento_Cliente.Forma_Pagamento.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Cartao_Credito &&
                                               planoProduto.Forma_Pagamento_Cliente.Forma_Pagamento.Gerar_Boleto_Automatico : false;

                    if (isPagPlanProdCartao)
                    {
                        if (planoProduto.Forma_Pagamento_Cliente.StatusValidoCartao == null)
                            planoProduto.Forma_Pagamento_Cliente.StatusValidoCartao = StatusValidacaoCartao.Aguardando_Validacao;

                        etapaVendaBusiness.AdcEtapaCartao(Usuario, objVenda);
                    }

                    if (planoProduto.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Internet)
                    {

                        if (!String.IsNullOrEmpty(planoProdutoView.Login_Radius) && !String.IsNullOrEmpty(planoProdutoView.Senha_Radius))
                        {
                            planoProduto.Login_Radius = planoProdutoView.Login_Radius.ToLowerInvariant();
                            planoProduto.Senha_Radius = planoProdutoView.Senha_Radius.ToLowerInvariant();
                        }
                        /*else
                        {
                            string primeiroNome = String.Empty;
                            string ultimoNome = String.Empty;

                            int idxPrimeiroEspaco = planoProduto.Cliente.NomeFantasia.IndexOf(" ");
                            int idxUltimoEspaco = planoProduto.Cliente.NomeFantasia.LastIndexOf(" ");

                            // Se o nome tem espaço preenche o primeiro e o último nome
                            if (idxPrimeiroEspaco != -1)
                            {
                                primeiroNome = planoProduto.Cliente.NomeFantasia.Substring(0, idxPrimeiroEspaco);
                                ultimoNome = planoProduto.Cliente.NomeFantasia.Substring(idxUltimoEspaco + 1, planoProduto.Cliente.NomeFantasia.Length - idxUltimoEspaco - 1);
                            }
                            // Caso o nome não tenha nenhum espaço preenche somente o primeiro nome
                            else
                            {
                                primeiroNome = planoProduto.Cliente.NomeFantasia;
                            }



                            /*planoProduto.Login_Radius = (config.Padrao_Login.Replace("PrimeiroNome", primeiroNome)
                                                     .Replace("UltimoNome", ultimoNome)
                                                     .Replace("Contrato", planoProduto.Cliente?.Numero_Contrato)
                                                     .Replace("Cont.PlanoseProdutos", planoProduto?.ClienteContrato?.Id.ToString())
                                                     .Replace("Código", Convert.ToString(planoProduto.Cliente?.Id))
                                                     .Replace("Email", planoProduto.Cliente?.Email_Principal)
                                                     .Replace("CPF/CNPJ", planoProduto.Cliente?.CpfCnpj)
                                                     .Replace("Uf", planoProduto.Cliente.Endereco_Principal?.UF)
                                                     .Replace("Cidade", planoProduto.Cliente.Endereco_Principal?.Cidade));

                            var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(planoProduto.Login_Radius);

                            planoProduto.Login_Radius = verificaLoginComSequencial;

                            planoProduto.Senha_Radius = (config.Padrao_Login_Ultimo.Replace("PrimeiroNome", primeiroNome)
                                                         .Replace("UltimoNome", ultimoNome)
                                                         .Replace("Contrato", planoProduto.Cliente?.Numero_Contrato)
                                                         .Replace("Cont.PlanoseProdutos", planoProduto?.ClienteContrato?.Id.ToString())
                                                         .Replace("Código", Convert.ToString(planoProduto.Cliente?.Id))
                                                         .Replace("Email", planoProduto.Cliente?.Email_Principal)
                                                         .Replace("CPF/CNPJ", planoProduto.Cliente?.CpfCnpj)
                                                         .Replace("Uf", planoProduto.Cliente.Endereco_Principal?.UF)
                                                         .Replace("Cidade", planoProduto.Cliente.Endereco_Principal?.Cidade));

                            var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(planoProduto.Senha_Radius);

                            planoProduto.Senha_Radius = verificaSenhaComSequencial; 
                        } */
                    }
                    repoCliente.Update(objCliente);

                    vendaBusiness.Update(objVenda);

                    objVenda.FlagVendaFormatoAvulso = planoProdutoView.FlagVendaFormatoAvulso;
                    if (planoProdutoView.Combo == false)
                        objVenda.Plano_Combo = $"{planoProduto?.Plano?.Id}-{planoProduto?.Plano?.Descricao}";
                    else
                        objVenda.Plano_Combo = $"{planoProduto?.Combo?.Id}-{planoProduto?.Combo?.Descricao}";

                    logs = objVenda.GeraLog(vendaClone, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Insercao);

                    vendaBusiness.SavePlanoProduto(planoProduto);
                    if (listaDescontosClientePlanoProduto?.Count > 0)
                    {
                        foreach (var item in listaDescontosClientePlanoProduto)
                        {
                            item.Numero_ContratoVinculado = planoProduto?.ClienteContrato?.NumeroContrato;
                        }
                    }
                }

                await repositoryLogs.SaveOrUpdate(logs);
                /*
                 Aplicação da Rotina de Mudança de Plano
                */
                //List<long> Ids_Mudanca = planoProdutoTemp.Where(p => p.Id_TrocaPlano != null).Select(p => p.Id_TrocaPlano.Value).ToList();
                //List<long> Contratos_Mudanca = planoProdutoTemp.Where(p => p.Id_TrocaPlano != null).Select(p => p.ClienteContrato.Id).ToList();

                //if (Ids_Mudanca.Count > 0)
                //{
                //    using (var PlanApi = new PlanoProdutoApiController())
                //    {
                //        await PlanApi.AlterarStatus(new AlterarStatusViewModel
                //        {
                //            ids = Ids_Mudanca,
                //            //Ids_Contrato = Contratos_Mudanca,
                //            id_motivo_cancelamento = 0,
                //            status = Convert.ToInt16(Status_PlanoProduto.Cancelado),
                //            usuario = User.Identity.GetUserName()

                //        });
                //    }
                //}

                objCliente.PlanosProdutos = planoProdutoTemp;


                // ****************
                // Forma de Pagamento 
                // ****************              
                if (planoProdutoTemp.Any(p => p?.Forma_Pagamento_Cliente != null && p?.Forma_Pagamento_Cliente != objVenda.FormaPagamentoCliente) || planoProdutoTemp.Any(p => p?.Forma_Pagamento_Cliente == null))
                {
                    objVenda.FormaPagamentoCliente = formaSelecionadaBanco;
                    objCliente.Formas_Pagamento_Cliente.Add(formaSelecionadaBanco);
                    foreach (var item in planoProdutoTemp.Where(p => p.Id_Venda == objVenda.Id))
                    {
                        if (formaSelecionadaBanco?.Id > 0)
                        {
                            item.IdFormaPagamentoVenda = objVenda?.FormaPagamentoCliente?.Id ?? formaSelecionadaBanco.Id;
                            item.Forma_Pagamento_Cliente = formaSelecionadaBanco;
                        }

                        if (item.IdFormaPagamentoVenda > 0)
                        {
                            vendaBusiness.SavePlanoProduto(item);
                        }
                    }

                }

                if (objVenda.FormaPagamentoCliente != null && planoProdutoTemp.Any(p => p.Id_Venda == objVenda.Id && p.Forma_Pagamento_Cliente == null))
                    throw new Exception("Plano/Produto sem forma de pagamento definida. Por favor, selecionar uma forma de pagamento na aba Planos e Produtos.");



                repoCliente.Update(objCliente);
                vendaBusiness.Update(objVenda);
                /****************************************
                            Aplicação dos Descontos
                    ****************************************/
                var lista_planos_com_descontos_por_modelo_contrato = objCliente.PlanosProdutos.Where(x => x.Id_Venda == objVenda.Id && x.ClienteContrato.UtilizaDesconto).ToList();
                if (lista_planos_com_descontos_por_modelo_contrato.Count > 0)
                {
                    foreach (var planoProduto in lista_planos_com_descontos_por_modelo_contrato)
                    {
                        if (view.Descontos.Any(x => x.Numero_ContratoVinculado == planoProduto.ClienteContrato.NumeroContrato))
                            continue;

                        view.Planos.Add(new PlanoProdutoListModel
                        {
                            ValorTotal = String.Format("{0:N2}", planoProduto?.Valor_Final_Total),
                            Tipo_Tecnologia = planoProduto?.Plano?.Tipo_Tecnologia,
                            Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id,
                            Id_Plano_Produto = planoProduto.Plano != null ? planoProduto.Plano.Id : planoProduto.Produto.Id,
                        });

                        view.Descontos.Add(new DescontoListaModel
                        {
                            Id = 0,
                            Id_Desconto = planoProduto.ClienteContrato.Desconto != null ? planoProduto.ClienteContrato.Desconto.Id : 0,
                            Valor_Desconto = planoProduto.ClienteContrato.Desconto != null ? String.Format("{0:N2}", planoProduto.ClienteContrato.Desconto.Valor_Desconto) : "",
                            Percentual_Desconto = planoProduto.ClienteContrato.Desconto != null ? String.Format("{0:N2}", planoProduto.ClienteContrato.Desconto.Percentual_Desconto) : "",
                            Numero_ContratoVinculado = planoProduto.ClienteContrato?.NumeroContrato,
                            Usuario = "SISTEMA"
                        });
                    }
                }
                else
                {
                    foreach (var item in view.Descontos)
                    {
                        item.Usuario = User.Identity.GetUserName();
                    }
                }

                ClienteApi.AplicacaoDescontos(session,
                    objCliente,
                    new ValidaDescontosModel
                    {
                        Descontos = view.Descontos,
                        IdCategoriaCliente = view.Id_Categoria_Cliente,
                        IdMeioCaptacao = view.Id_Meio_Captacao,
                        Planos = view.Planos,

                    },
                    retorno,
                    objVenda.Id
                    );


                //****************************************************************************
                // Checagem para determinar para o Cliente que novos planos foram adicionados
                //****************************************************************************

                if (temPlanoProdutoNovo)
                {
                    objCliente.Vendedor = usuarioLogado?.Funcionario;
                    objCliente.Supervisor = usuarioLogado?.Funcionario?.Supervisor;
                }

                repoCliente.Update(objCliente);

                return retorno;

            }
        }

        [HttpGet()]
        [Route("api/Vendaapi/FindById")]
        public async Task<GetApiRetorno<VendaDadosGeraisViewModel>> FindById(Int64? Id_Venda, Int64? Id_Cliente)
        {
            GetApiRetorno<VendaDadosGeraisViewModel> retorno = new GetApiRetorno<VendaDadosGeraisViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Venda> repoVenda = new Conexao<Venda>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;
                    if (Id_Venda.HasValue)
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == Id_Venda.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                    }
                    else if (Id_Cliente.HasValue)
                    {
                        venda = new Venda();
                        cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente.Value);
                    }

                    retorno.Objeto = await GetViewObj(venda, cliente, session);

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/Vendaapi/FindVendaById_Cpp")]
        public async Task<GetApiRetorno<long?>> FindVendaById_Cpp(long Id_cpp)
        {
            var retorno = new GetApiRetorno<long?>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoVenda = new Conexao<Venda>(session))
                {
                    var cpp = (await repoClientePlano.selectFirst(p => p.Id == Id_cpp));
                    var venda = (await repoVenda.selectFirst(venda => venda.Id == cpp.Id_Venda));
                    retorno.Objeto = venda.Id;
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = $"{erro?.Message} - {erro?.InnerException?.Message}";
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Vendaapi/GetTermoAceiteDoc")]
        public async Task<DataTableResponse<AnexosTermo>> GetTermoAceiteDoc(VendasTermoDatatableRequest request)
        {
            var response = new DataTableResponse<AnexosTermo>();
            List<AnexosTermo> retorno = new List<AnexosTermo>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoDocTermo = new Conexao<DocTermoAnexo>(session))
            {
                if (request.IdVenda != null)
                {
                    var vendasDoc = await repoDocTermo.select(p => p.Venda.Id == request.IdVenda);

                    if (vendasDoc.Count > 0)
                    {
                        foreach (var docTermoAnexo in vendasDoc)
                        {
                            retorno.Add(new AnexosTermo
                            {
                                Id = docTermoAnexo.Id,
                                Nome = docTermoAnexo.Nome,
                                Lado = HtmlEnumExtensions.GetEnumDescription<Lado_Doc_Aceite>(docTermoAnexo.Lado),
                                TipoDocumento = HtmlEnumExtensions.GetEnumDescription<Tipo_Doc_Termo_Anexo>(docTermoAnexo.TipoDocumento),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Doc_Aceite>(docTermoAnexo.Status),
                                Extensao = docTermoAnexo.Extensao,
                                Url = docTermoAnexo.Url,
                            });
                        }
                    }
                }
                else if (request.IdTroca != null)
                {
                    var trocasDoc = await repoDocTermo.select(p => p.TrocaTitularidade.Id == request.IdTroca);

                    if (trocasDoc != null)
                    {
                        foreach (var docTermoAnexo in trocasDoc)
                        {
                            retorno.Add(new AnexosTermo
                            {
                                Id = docTermoAnexo.Id,
                                Nome = docTermoAnexo.Nome,
                                Lado = HtmlEnumExtensions.GetEnumDescription<Lado_Doc_Aceite>(docTermoAnexo.Lado),
                                TipoDocumento = HtmlEnumExtensions.GetEnumDescription<Tipo_Doc_Termo_Anexo>(docTermoAnexo.TipoDocumento),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Doc_Aceite>(docTermoAnexo.Status),
                                Extensao = docTermoAnexo.Extensao,
                                Url = docTermoAnexo.Url,
                            });
                        }
                    }

                }
            }
            //Page e retornando DataTable
            return new DataTableResponse<AnexosTermo>
            {
                //draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno,
                error = ""
            };
        }
        [HttpPost()]
        [Route("api/Vendaapi/GetTermoAceite")]
        public async Task<DataTableResponse<VendasTermo>> GetTermoAceite(VendasTermoDatatableRequest request)
        {
            List<VendasTermo> retorno = new List<VendasTermo>();

            //Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoDocTermo = new Conexao<Venda>(session))
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
            using (var repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoAnexos = new Conexao<DocTermoAnexo>(session))
            {
                var docVenda = await repoDocTermo.selectFirst(p => p.Id == request.IdVenda);
                if (docVenda != null)
                {
                    var cliente = await repoCliente.selectFirst(p => p.Id == request.IdCliente);

                    var cliente_plano_produto = (await repoCliente_PlanoProduto.select(x => x.Id_Venda == request.IdVenda)).GroupBy(x => x.ClienteContrato).Select(x => x.First());
                    var lista_cliente_contrato = new List<Object>();
                    foreach (var item in cliente_plano_produto)
                    {
                        lista_cliente_contrato.Add(new
                        {
                            Id = item.ClienteContrato.Id,
                            Descricao = item.ClienteContrato.Descricao
                        });
                    }


                    EtapaVenda etapa = await repoEtapaVenda.selectFirst(p => p.Venda.Id == docVenda.Id && p.Etapa == EnumEtapaVenda.TermoAdesao);
                    var vendasDoc = (await repoAnexos.select(p => p.Venda.Id == request.IdVenda)).ToList();
                    var vendaTermo = new VendasTermo
                    {
                        Id = docVenda.Id,
                        TermoAssinado = docVenda.Termo_Adesao_Assinado ? docVenda.Termo_Adesao_Assinado : false,
                        Status = Convert.ToString(EnumEtapaVenda.TermoAdesao),
                        DataAceite = docVenda.Termo_Adesao_Assinado ? docVenda.DataHoraAssinaturaTermo?.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                        UrlTermo = docVenda.Url_Documento_Assinado?.ToString(),
                        ListaClientesContratos = lista_cliente_contrato,
                        AnexoCriado = vendasDoc.Count > 0,
                        Status_Venda = (Int64?)docVenda.Status,
                        Status_Etapa_Venda = (Int64?)etapa.Status,
                        Ip = docVenda.IpOrigemAceite
                    };

                    if (cliente != null)
                    {
                        vendaTermo.NomeCliente = cliente.NomeFantasia;
                        vendaTermo.CpfCnpj = cliente.CpfCnpj;
                    }

                    retorno.Add(vendaTermo);
                }
            }

            // Page e retornando DataTable
            return new DataTableResponse<VendasTermo>
            {
                // draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno,
                error = ""
            };
        }


        public async Task<string> RegistroAceiteDigital(InformacoesAceite infosAceite)
        {
            StringBuilder aceiteBuilder = new StringBuilder();
            aceiteBuilder.AppendLine("(pagebreak)");
            aceiteBuilder.AppendLine("______________________________________________________");
            aceiteBuilder.AppendLine("");
            aceiteBuilder.AppendLine("<b>Registro de Aceite Digital</b>");
            aceiteBuilder.AppendLine("");
            aceiteBuilder.AppendLine("<b>Este documento foi aceito digitalmente pelo titular conforme registro detalhado abaixo:</b>");
            aceiteBuilder.AppendLine("");
            aceiteBuilder.AppendLine("- <b>E-mail do destinatário:</b> @EMAILDEST");
            aceiteBuilder.AppendLine("- <b>IP de origem:</b> @IPDEST");
            aceiteBuilder.AppendLine("- <b>Porta de origem:</b> @PORTADEST");
            aceiteBuilder.AppendLine("- <b>Data e hora do aceite:</b> @DATAHORA ");
            aceiteBuilder.AppendLine("- <b>Navegador/Dispositivo (User-Agent):</b> @USERAGENT");
            aceiteBuilder.AppendLine("- <b>Código hash identificador do documento:</b> @CODIGOHASH");
            aceiteBuilder.AppendLine("- <b>URL de aceite:</b> @URLACEITE");
            aceiteBuilder.AppendLine("");
            aceiteBuilder.AppendLine("<b>O aceite foi realizado por meio de link único e individualizado enviado ao e-mail do titular.</b>");
            aceiteBuilder.AppendLine("<b>Este documento foi assinado eletronicamente de acordo com os princípios do Marco Civil da Internet (Lei 12.965/2014) e da Lei Geral de Proteção de Dados (Lei 13.709/2018).</b>");
            aceiteBuilder.AppendLine("<b>O conteúdo deste documento foi congelado no momento do aceite e permanece íntegro conforme o hash acima.</b>");
            aceiteBuilder.AppendLine("");
            aceiteBuilder.AppendLine("______________________________________________________");

            if (infosAceite.AceiteTermoAdesao && !String.IsNullOrEmpty(infosAceite.CaminhoSelfie))
            {
                try
                {
                    using (var webClient = new System.Net.WebClient())
                    {
                        byte[] imageBytes = webClient.DownloadData(infosAceite.CaminhoSelfie);

                        string base64String = Convert.ToBase64String(imageBytes);

                        string imgSrc = $"data:image/png;base64,{base64String}";

                        aceiteBuilder.AppendLine("<br/>");
                        aceiteBuilder.AppendLine("<b>Registro de Captura da Selfie</b>");
                        aceiteBuilder.AppendLine("<br/>");
                        aceiteBuilder.AppendLine($"<img src='{imgSrc}' style='width:500px; height:auto;' />");
                    }
                }
                catch (Exception ex)
                {
                    aceiteBuilder.AppendLine("<i>[Erro ao carregar imagem da selfie]</i>");
                }
            }

            string aceiteDigital = aceiteBuilder.ToString();

            if (!string.IsNullOrEmpty(infosAceite.EmailDestinatario))
                aceiteDigital = aceiteDigital.Replace("@EMAILDEST", infosAceite.EmailDestinatario);

            if (!string.IsNullOrEmpty(infosAceite.IpOrigem))
                aceiteDigital = aceiteDigital.Replace("@IPDEST", infosAceite.IpOrigem);

            if (!string.IsNullOrEmpty(infosAceite.PortaOrigem))
                aceiteDigital = aceiteDigital.Replace("@PORTADEST", infosAceite.PortaOrigem);

            if (!string.IsNullOrEmpty(infosAceite.DataHoraAceite))
                aceiteDigital = aceiteDigital.Replace("@DATAHORA", infosAceite.DataHoraAceite);

            if (!string.IsNullOrEmpty(infosAceite.UserAgent))
                aceiteDigital = aceiteDigital.Replace("@USERAGENT", infosAceite.UserAgent);

            if (!string.IsNullOrEmpty(infosAceite.HashIdentificador))
                aceiteDigital = aceiteDigital.Replace("@CODIGOHASH", infosAceite.HashIdentificador);

            if (!string.IsNullOrEmpty(infosAceite.UrlAceite))
                aceiteDigital = aceiteDigital.Replace("@URLACEITE", infosAceite.UrlAceite);

            return await Task.FromResult(aceiteDigital);
        }


        private static double ValorComDesconto_MeioCaptacao(double? valorDesconto, double? percentualDesconto, double valorPlanoProduto, List<Desconto> listaDescontos)
        {
            double somaDescontosPercentuais = 0;
            double valorComDesconto = valorPlanoProduto;

            double desconto;

            listaDescontos.Add(new Desconto
            {
                Valor_Desconto = valorDesconto,
                Percentual_Desconto = percentualDesconto
            });

            foreach (Desconto item in listaDescontos)
            {
                if (item.Percentual_Desconto.HasValue)
                {
                    desconto = (item.Percentual_Desconto.Value) / 100;
                    somaDescontosPercentuais += valorPlanoProduto * desconto;
                }
                else
                {
                    valorComDesconto -= item.Valor_Desconto.Value;
                }
            }

            valorComDesconto -= somaDescontosPercentuais;

            return valorComDesconto;
        }

        [HttpPost()]
        [Route("api/Vendaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<VendaDadosGeraisViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            Int32 count = new ClienteApiController().Existecpf(value.Objeto.CpfCnpj).Result;

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    PostApiRetorno<long> gravar_venda = await GravarVenda(session, value);

                    retorno.IdObjeto = gravar_venda.IdObjeto;
                    retorno.Mensagens = gravar_venda.Mensagens;
                    retorno.Ok = true;

                    bool possuiPendeciaEnvioTermo = await PendenciasEnvioTermo(retorno);

                    if (!possuiPendeciaEnvioTermo)
                    {
                        using (var termApi = new Termo_AdesaoApiController())
                        {
                            await termApi.EnviarTermoAdesao(retorno.IdObjeto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        /// <summary>
        /// após Gravação da venda, verificar as Pendencias na mesma
        /// </summary>
        /// <param name="retorno"></param>
        /// <returns></returns>
        public async Task<bool> PendenciasEnvioTermo(PostApiRetorno<long> retorno)
        {
            List<bool> pendencias = new List<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoEtapa = new Conexao<EtapaVenda>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            {
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                bool possuiEtapaPendenteOuReprovada =
                    (await repoEtapa.selectFirst(p => p.Venda.Id == retorno.IdObjeto &&
                                                      (p.Status == StatusEtapaVenda.Reprovado || p.Status == StatusEtapaVenda.Pendente) &&
                                                      ((int)p.Etapa < (int)EnumEtapaVenda.TermoAdesao))) != null;

                bool termoJaAssinado = (await repoVenda.selectFirst(p => p.Id == retorno.IdObjeto)).Termo_Adesao_Assinado;

                return possuiEtapaPendenteOuReprovada || termoJaAssinado || !config.EnviarTermoAdesaoAuto;
            }
        }

        /// <summary>
        ///     Altera a etapa informada para o status "Pendente" ao clicar no botão de Etapa Anterior na View
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        /// <param name="etapa">Etapa que voltará para o status de "Pendente"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/VendaApi/AlterarStatusEtapaVenda")]
        public async Task<GetApiRetorno> AlterarStatusEtapaVenda(
            long? idVenda,
            Sinapse.Model.enumerados.EnumEtapaVenda etapa,
            Sinapse.Model.enumerados.StatusEtapaVenda status,
            long? IdConsulta = null,
            long? idTrocaTitular = null
        )
        {
            var retorno = new GetApiRetorno();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    IVendaRepository repoVenda = new VendaRepository(session);
                    IEnderecoClienteRepository repoEndereco = new EnderecoClienteRepository(session);

                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                    if (idVenda != 0 && idVenda != null)
                    {
                        var vendaBusiness = new VendaBusiness(repoVenda, repoEndereco);

                        Venda venda = vendaBusiness.GetVendaById(idVenda.Value);

                        etapaVendaBusiness.MudarStatusEtapa(idVenda, etapa, status, DateTime.Now);

                        venda.Etapa_Atual = etapaVendaBusiness.VerificaUltimaEtapa(venda.Id);

                        vendaBusiness.Update(venda);

                        if (IdConsulta.HasValue)
                        {
                            vendaBusiness.GravarIdConsultaRestricao(venda, etapa, IdConsulta.Value);
                        }
                    }
                    else if (idTrocaTitular != 0 && idTrocaTitular != null)
                    {
                        using (var repoTroca = new Conexao<TrocaTitularidade>(session))
                        {
                            TrocaTitularidade troca = repoTroca.selectFirst(x => x.Id == idTrocaTitular).Result;

                            if (troca != null)
                            {
                                etapaVendaBusiness.MudarStatusEtapa(null, etapa, status, DateTime.Now, null, idTrocaTitular);

                                troca.Etapa_Atual = etapaVendaBusiness.VerificaUltimaEtapaTroca(troca.Id);

                                if (IdConsulta != null)
                                    troca.IdConsultaPendenciaFinanceira = IdConsulta.Value;

                                await repoTroca.SaveOrUpdate(troca);
                            }
                        }

                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }


        /// <summary>
        /// verificar o termo de adesão 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Vendaapi/TermoAdesaoCliente")]
        #region TermoAdesaoCliente       
        public async Task<GetApiRetorno<List<VendaListViewModel>>> TermoAdesaoCliente(VendaListViewModel request)
        {
            GetApiRetorno<List<VendaListViewModel>> retorno = new GetApiRetorno<List<VendaListViewModel>>
            {
                Objeto = new List<VendaListViewModel>()
            };
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        //Pega apenas as vendas diferentes de cancelada e com o termo não assinado
                        StringBuilder sqlWhere = new StringBuilder($@"WHERE A.ID = @ID_CLIENTE AND D.ID = @ID_CONTRATO AND B.TERMO_ADESAO_ASSINADO = 0 AND B.STATUS IN (0, 7, 8, 9, 10)
                                                                    AND C.STATUS <> 1 AND ((D.STATUS IN (1, 2, 5)) OR (D.STATUS IN (1, 2) AND D.STATUS IS NULL))
                                                                    GROUP BY B.ID, A.ID, B.TERMO_ADESAO_ASSINADO, B.STATUS, C.STATUS, D.STATUS");
                        StringBuilder sqlFrom = new StringBuilder(@" FROM TB_CLIENTE A 
                                                                     LEFT JOIN TB_VENDA B ON A.ID = B.ID_CLIENTE
                                                                     LEFT JOIN TB_ETAPA_VENDA C ON C.ID_VENDA = B.ID
                                                                     LEFT JOIN TB_CLIENTE_CONTRATO D ON D.ID_CLIENTE = A.ID"
                        );

                        StringBuilder sql = new StringBuilder(" SELECT B.ID ID_VENDA, A.ID ID_CLIENTE, B.TERMO_ADESAO_ASSINADO, B.STATUS StatusVenda, C.STATUS StatusEtapaVenda, D.STATUS StatusContrato ");

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        parametros.Add("@ID_CLIENTE", request.Id_Cliente);
                        parametros.Add("@ID_CONTRATO", request.Id_Contrato);

                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            while (await reader.ReadAsync())
                            {


                                VendaListViewModel obj = new VendaListViewModel
                                {
                                    Termo_Adesao_Assinado = Convert.ToString(reader["TERMO_ADESAO_ASSINADO"]),
                                };
                                retorno.Objeto.Add(obj);
                            }

                    }
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Vendaapi/GetCombo")]
        #region GetCombo
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Venda> repoVenda = new Conexao<Venda>(session))
            {
                foreach (Venda venda in await repoVenda.select())
                {
                    retorno.results.Add(new VendaComboResult { Id = venda.Id, Text = venda.Id.ToString() });
                }
            }

            return retorno;
        }
        #endregion


        [HttpPost()]
        [Route("api/VendaApi/AlterarStatusWorkflow")]
        public async Task<PostApiRetorno<Int64>> AlterarStatusWorkflow(string idVenda, int situacao)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Mensagens = new List<PostApiMensagem>();

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))
            using (Conexao<Workflow_Vendas> repoWork = new Conexao<Workflow_Vendas>(session))
            using (Conexao<Workflow_VendasAtividade> repoWorkAtividades = new Conexao<Workflow_VendasAtividade>(session))
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            {
                try
                {
                    Venda venda = await repositoryVenda.selectFirst(p => p.Id == Convert.ToInt64(idVenda));
                    var atividade = venda.Workflow_Vendas.Atividades.Where(p => p.Ativ_StatusVenda?.StatusPadrao == venda.Status).FirstOrDefault();
                    Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                    bool existemEtapas = etapaVendaBusiness.ExistemEtapas(venda.Id);

                    if (atividade.Ativ_Decisao == Atividade_Decisoria.Sim && existemEtapas && config.Exige_Termo_Adesao && !venda.Termo_Adesao_Assinado)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = "Necessário que todas as etapas de venda estejam concluídas para aprovar a venda";
                    }
                    else
                    {
                        //Atividade Aprovada
                        if (situacao == 0)
                        {
                            var workAtividades = await repoWorkAtividades.selectFirst(p => p.Id == atividade.Ativ_NAtivAprovada);
                            venda.Workflow_VendasAtividade = workAtividades;
                            venda.Status = (Status_Venda)workAtividades?.Ativ_StatusVenda?.StatusPadrao;

                        }

                        //Atividade Reprovada
                        else
                        {

                            var workAtividades = await repoWorkAtividades.selectFirst(p => p.Id == atividade.Ativ_NAtivReprovada);
                            venda.Workflow_VendasAtividade = workAtividades;
                            venda.Status = (Status_Venda)workAtividades?.Ativ_StatusVenda?.StatusPadrao;
                        }


                        await repositoryVenda.SaveOrUpdate(venda);
                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }

        #region GravarVenda
        public async Task<PostApiRetorno<Int64>> GravarVenda(ISession session, [FromBody] ParamWebApi<VendaDadosGeraisViewModel> value)
        {

            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Mensagens = new List<PostApiMensagem>();

            Venda venda = null;
            Venda modeloAnterior = null;
            Cliente cliente = null;

            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))
            using (Conexao<Endereco_Cliente> repositoryEndereco = new Conexao<Endereco_Cliente>(session))
            using (Conexao<Venda_Ultimo_Contato> repositoryVendaUltimoContato = new Conexao<Venda_Ultimo_Contato>(session))
            using (Conexao<Cliente> repositoryCliente = new Conexao<Cliente>(session))
            using (Conexao<Combo> repositoryCombo = new Conexao<Combo>(session))
            using (Conexao<Venda_Ultimo_Contato> repoVendaUltimoContato = new Conexao<Venda_Ultimo_Contato>(session))
            using (Conexao<Atividade_StatusVenda> repoVendaStatus = new Conexao<Atividade_StatusVenda>(session))
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<Elemento_Rede_Porta> repoElemento_Rede_Porta = new Conexao<Elemento_Rede_Porta>(sessionT5Nh))

            using (Conexao<IntegracoesExternas> repoIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
            using (Workflow_vendasApiController workflowApi = new Workflow_vendasApiController())
            {
                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                //Elemento_Rede_Porta portaResev = (await repoElemento_Rede_Porta.selectFirst());

                if (value.Objeto.Id != 0)
                    venda = (await repositoryVenda.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                if (venda == null)
                {
                    venda = new Venda
                    {
                        //Status = Connect.Model.enumerado.Status_Venda.Analise_Cliente,
                        Id_Cliente = value.Objeto.Id_Cliente,
                        UsuarioCriacao = value.Usuario,
                        Id_Forma_Pagamento_Cliente_Preferida = value.Objeto.Id_Forma_Pagamento_Cliente_Preferida,
                        EtapaIntegracao = Convert.ToInt64(EtapaIntegracaoIXC.A_Iniciar),
                        Origem_Venda = "CONNECT",
                        Formas_Pagamento = new List<Forma_Pagamento_ClienteModel>()
                    };
                }
                else
                {
                    //      --- Como analisar todos os status e garantir que este processo é válido? ---    //                    
                    if (value.Objeto.Status != venda.Status)
                    {
                        throw new Exception("Inconsistencia de Status");
                    }
                    modeloAnterior = (Venda)venda.Clone();
                    venda.UsuarioAlteracao = value.Usuario;
                    venda.Id_Forma_Pagamento_Cliente_Preferida = value.Objeto.Id_Forma_Pagamento_Cliente_Preferida;
                    venda.Formas_Pagamento = value.Objeto.Formas_Pagamento.Where(x => x.Id > 0).Select(p => new Forma_Pagamento_ClienteModel
                    {
                        Id = p.Id,
                        Id_Forma_Pagamento = p.Id_Forma_Pagamento,
                        Desc_Forma_Pagamento = p.Desc_Forma_Pagamento
                    }).ToList();
                }

                Venda vendaClone = (Venda)venda.Clone();

                if (value.Objeto.Id_Cliente != 0)
                    cliente = (await repositoryCliente.select(p => p.Id == value.Objeto.Id_Cliente)).FirstOrDefault();

                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        Status = Status_Cliente.Ativo,
                        Tipo_Cliente = Tipo_Cliente.Prospecto,
                        UsuarioCriacao = value.Usuario
                    };
                }
                else
                {
                    cliente.UsuarioAlteracao = value.Usuario;
                }

                IVendaRepository repoVenda = new VendaRepository(session);
                IFuncionarioRepository repoFuncionario = new FuncionarioRepository(session);
                IDepartamentoRepository repoDepto = new DepartamentoRepository(session);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                IClienteRepository repoCliente = new ClienteRepository(session);
                IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(session);
                ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(session);
                IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(session);
                IPaisRepository repoPais = new PaisRepository(session);
                IUfRepository repoUf = new UfRepository(session);
                ICidadeRepository repoCidade = new CidadeRepository(session);
                IBairroRepository repoBairro = new BairroRepository(session);
                ITipoImovelRepository repoTipoImovel = new TipoImovelRepository(session);
                IEnderecoLogradouroRepository repoEnderecoLogradouro = new EnderecoLogradouroRepository(session);
                ICondominioPredioRepository repoCondominioPredio = new CondominioPredioRepository(session);
                IBancoRepository repoBanco = new BancoRepository(session);
                IVencimentoRepository repoVencimento = new VencimentoRepository(session);
                IFormaPagamentoRepository repoFormaPagamento = new FormaPagamentoRepository(session);
                IComboRepository repoCombo = new ComboRepository(session);
                ICampanhaRepository repoCampanha = new CampanhaRepository(session);
                IPlanoRepository repoPlano = new PlanoRepository(session);
                IProdutoRepository repoProduto = new ProdutoRepository(session);
                IPool_IpRepository repoPoolIP = new Pool_IpRepository(session);
                IModeloContratoRepository repoModeloContrato = new ModeloContratoRepository(session);
                IClienteContratoRepository repoClienteContrato = new ClienteContratoRepository(session);
                IClientePlanoProdutoRepository repoClientePlanoProduto = new ClientePlanoProdutoRepository(session);
                IComboPlanoProdutoRepository repoComboPlanoProdutoRepository = new ComboPlanoProdutoRepository(session);
                IWorkflowRepository repoWorkflowRepository = new WorkflowRepository(session);

                var logs = new List<LogVendas>();

                var vendaBusiness =
                    new VendaBusiness(
                        repoVenda,
                        repoFuncionario,
                        repoDepto,
                        repoConfig,
                        repoUsuario,
                        repoCliente,
                        repoEnderecoCliente,
                        repoCategoriaCliente,
                        repoMeioCaptacao,
                        repoPais,
                        repoUf,
                        repoCidade,
                        repoBairro,
                        repoTipoImovel,
                        repoEnderecoLogradouro,
                        repoCondominioPredio,
                        repoBanco,
                        repoVencimento,
                        repoFormaPagamento,
                        repoCombo,
                        repoCampanha,
                        repoPlano,
                        repoProduto,
                        repoPoolIP,
                        repoModeloContrato,
                        repoClienteContrato,
                        repoClientePlanoProduto,
                        repoComboPlanoProdutoRepository,
                        repoWorkflowRepository);

                venda = vendaBusiness.BindDadosGeraisVenda(venda, cliente, value.Objeto, value.Usuario);


                using (var repositoryUsuario = new Conexao<IdentityUser>(session))
                {
                    var perfilVisualizacaoVenda = await repositoryUsuario.selectFirst(p => p.Id == value.Usuario);
                    if (perfilVisualizacaoVenda.PerfilVisualizacaoVenda?.PerfilSolicitaAutorizacao == true && cliente.Tipo_Cliente == Tipo_Cliente.Cliente)
                    {
                        EtapaVendaViewModel etapaAtual = value.Objeto.EtapasVenda.Where(e => e.Etapa == value.Objeto.EtapaAtualVenda).FirstOrDefault();

                        if ((etapaAtual.Etapa >= 6) && (venda.Status_Liberacao != Status_Venda.Aguardando_Liberacao) && (venda.Status_Liberacao != Status_Venda.liberacao_Aprovada) && (venda.Status_Liberacao != Status_Venda.liberacao_Reprovada))
                        {
                            venda.Status_Liberacao = Status_Venda.Aguardando_Liberacao;
                        }
                    }
                }

                if (modeloAnterior != null)
                {
                    foreach (Forma_Pagamento_ClienteModels forma_pagamento_clienteview in value.Objeto.Formas_Pagamento.FindAll(p => p.Id == 0))
                    {
                        venda.Formas_Pagamento.Add(new Forma_Pagamento_ClienteModel
                        {
                            Id = 0,
                            Id_Forma_Pagamento = forma_pagamento_clienteview.Id_Forma_Pagamento,
                            Desc_Forma_Pagamento = forma_pagamento_clienteview.Desc_Forma_Pagamento
                        });
                    }

                    using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
                    {
                        logs = venda.GeraLog(modeloAnterior, value.Usuario, User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogs.Save(logs);
                    }
                }
                else
                {
                    using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
                    {
                        logs = venda.GeraLog(modeloAnterior, value.Usuario, User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Insercao);
                        await repositoryLogs.Save(logs);
                    }

                }

                // O endereço de instalação precisa ser gravado antes do objeto da venda para evitar
                // um problema com o mapeamento do NHibernate ao tentar gravar a venda
                await repositoryEndereco.SaveOrUpdate(venda.EnderecoInstalacao);
                await repositoryCliente.SaveOrUpdate(cliente);
                await repositoryVenda.SaveOrUpdate(venda);

                if (value.Objeto.ProximaEtapa)
                {
                    GravarEtapaVenda(session, venda, value.Objeto, value.Usuario);
                }



                retorno.Mensagens.AddRange(await BindViewToObj(session, venda, cliente, value.Objeto, value.Usuario, sessionLog));

                await ChecarEtapaCartao(session, venda, cliente.PlanosProdutos.ToList());

                if (retorno.Mensagens.Count() > 0)
                {
                    var primeiraMensagem = retorno.Mensagens.FirstOrDefault();
                    string mensagemErro = $"Ocorreu um erro: {primeiraMensagem.Mensagem}";

                    throw new Exception(mensagemErro);
                }

                foreach (var planoproduto in cliente.PlanosProdutos.Where(p => !String.IsNullOrEmpty(p.JsonTelefonia)))
                {
                    retorno.Mensagens.AddRange(await Tarefa_Telefonia(session, planoproduto));
                }

                await repositoryCliente.SaveOrUpdate(cliente);

                await ConfigurarLoginNovosPlanos(session, cliente, config, value.Objeto.Planos, venda);


                if (venda.Id_Forma_Pagamento_Cliente_Preferida == 0)
                {
                    venda.Id_Forma_Pagamento_Cliente_Preferida = cliente.Formas_Pagamento_Cliente.FirstOrDefault(p => p != null && p.Origem_Mobile == true)?.Id;
                    await repositoryVenda.SaveOrUpdate(venda);
                }

                IEnumerable<IntegracoesExternas> Integracoes = (await repoIntegracoesExternas.select()).ToList();


                // Reservar porta Consulta de Viabilidade
                bool Reserva_automatica = config.Reserva_Porta_Automatica;
                if (Reserva_automatica == true)
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (Conexao<Elemento_Rede_Porta> repoPortas = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    using (Conexao<Model.nhibernate.LoginRadius> repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))

                    //Api Usage

                    using (Consulta_ViabilidadeApiController consulta_ViabilidadeApi = new Consulta_ViabilidadeApiController())
                    using (Elemento_Rede_PortaApiController elemento_Rede_PortaApi = new Elemento_Rede_PortaApiController())
                    {
                        foreach (Cliente_PlanoProduto cliente_Plano_Produto in cliente.PlanosProdutos.Where(p => p.Id_Venda == venda.Id).ToList())
                        {
                            Model.nhibernate.LoginRadius loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == cliente_Plano_Produto.Id);
                            long? IdPorta = loginRadius?.Id_Porta_FTTH;

                            if (IdPorta == null)
                            {
                                var DadosViabilidade = consulta_ViabilidadeApi.GetDadosViabilidadeByVenda(venda.Id);
                                var resultadoConsulta = DadosViabilidade.Ok ? DadosViabilidade : await consulta_ViabilidadeApi.ExecutarConsultaViabilidade(
                                                                                                        new ExecutarConsultaViabilidadeRequest
                                                                                                        {
                                                                                                            Latitude = cliente_Plano_Produto.Endereco_Instalacao.Latitude.Value,
                                                                                                            Longitude = cliente_Plano_Produto.Endereco_Instalacao.Longitude.Value,
                                                                                                            IdVenda = null,
                                                                                                            IdClientePlanoProduto = null
                                                                                                        });

                                if (resultadoConsulta.Objeto != null && resultadoConsulta.Ok == true)
                                {
                                    Consulta_ViabilidadeCaixa consulta_ViabilidadeObj = resultadoConsulta.Objeto.Caixas.FirstOrDefault();
                                    long? nrPorta = consulta_ViabilidadeObj != null ? consulta_ViabilidadeObj.PortasDisponiveis.FirstOrDefault() : (long?)null;
                                    Elemento_Rede_Porta porta = null;

                                    if (nrPorta.HasValue && consulta_ViabilidadeObj != null)
                                        porta = await repoPortas.selectFirst(p => p.Porta == nrPorta && consulta_ViabilidadeObj.Id == p.Caixa.Id);


                                    var isPlanoInternet = cliente_Plano_Produto?.Plano != null ? cliente_Plano_Produto?.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Internet :
                                                          cliente_Plano_Produto?.Combo?.PlanosProdutos?.Any(x => x?.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Internet);


                                    if (porta != null && (isPlanoInternet.HasValue && isPlanoInternet.Value))
                                    {
                                        porta.Status = Situacao_Porta.Reservado;
                                        porta.Login = cliente_Plano_Produto.Login_Radius;
                                        porta.Cliente_Plano_Prod = cliente_Plano_Produto.Id;
                                        porta.IdLogin = loginRadius?.Id;

                                        await repoPortas.SaveOrUpdate(porta);

                                        if (loginRadius != null)
                                        {
                                            loginRadius.Id_Caixa_FTTH = porta?.Caixa?.Id;
                                            loginRadius.Id_Porta_FTTH = porta?.Id;

                                            await repoLoginRadius.SaveOrUpdate(loginRadius);
                                        }

                                        /*  -======-         Parte de Integracação Externa de Caixa e Porta FTTH(Geosite)        -======-   */
                                        if (Integracoes.Count() > 0)
                                        {
                                            foreach (IntegracoesExternas Integracao in Integracoes)
                                            {
                                                IEnumerable<PostApiMensagem> mensagens = this.IntegracaoFTTH_Venda(Integracao.IntegracaoExterna,
                                                                                                                   cliente_Plano_Produto.Id,
                                                                                                                   porta.Id);
                                                retorno.Mensagens.AddRange(mensagens);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                if (logs.Count > 0)
                {
                    using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
                    {
                        foreach (var log in logs.Where(log => log.IdLog == 0))
                        {
                            log.IdLog = venda.Id;
                            await repositoryLogs.Update(log);
                        }
                    }
                }

                retorno.IdObjeto = venda.Id;
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se tem Plano que utiliza forma de pagamento Cartao com Recorrencias
        /// </summary>
        /// <param name="venda"></param>
        /// <param name="clientePlanos"></param>
        /// <returns></returns>
        private async Task ChecarEtapaCartao(ISession session, Venda venda, List<Cliente_PlanoProduto> clientePlanos)
        {

            if (clientePlanos != null)
            {
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                clientePlanos = clientePlanos.Where(x => x.Id_Venda == venda.Id).ToList();

                var temPagPlanProdCartao = clientePlanos.Any(x => x.Forma_Pagamento_Cliente != null) ? clientePlanos.Any(planoProduto => planoProduto.Forma_Pagamento_Cliente.Forma_Pagamento != null && planoProduto.Forma_Pagamento_Cliente.Forma_Pagamento.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Cartao_Credito &&
                                                             planoProduto.Forma_Pagamento_Cliente.Forma_Pagamento.Gerar_Boleto_Automatico) : false;

                if (!temPagPlanProdCartao)
                {


                    await etapaVendaBusiness.RmvEtapaCartao(venda);
                }
                else
                {

                    var etapas = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);
                    foreach (var plano in clientePlanos)
                    {
                        if (plano.Forma_Pagamento_Cliente.StatusValidoCartao == null)
                        {
                            plano.Forma_Pagamento_Cliente.StatusValidoCartao = StatusValidacaoCartao.Aguardando_Validacao;

                        }
                    }
                    if (!etapas.Any(p => p.Etapa == Convert.ToInt32(EnumEtapaVenda.CartaoCredito)))
                    {
                        etapaVendaBusiness.AdcEtapaCartao(venda.UsuarioAlteracao, venda);
                    }
                }
            }
        }

        #endregion

        private async Task ConfigurarLoginNovosPlanos(ISession session,
                                                      Cliente cliente,
                                                      Configuracoes_Gerais config,
                                                      List<PlanoProdutoListModel> planos,
                                                      Venda venda)
        {
            #region Pool de Ip
            //*************************
            //Repositórios Relacionados
            //*************************
            using (Conexao<Model.nhibernate.LoginRadius> repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (Conexao<Model.nhibernate.Pool_Ip_Endereco> repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
            using (Conexao<Concentradora> repoConcentradora = new Conexao<Concentradora>(session))
            using (Conexao<Model.nhibernate.Pool_Ip> repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))

            /*-====================-    Apis Utilizadas    -====================-*/
            using (var ApiConcentradora = new ConcentradoraApiController())
            using (var ApiPool_Ip = new Pool_IpApiController())
            {
                foreach (Cliente_PlanoProduto cliente_plano in cliente.PlanosProdutos.Where(x => x.Plano?.AutenticacaoRadius == true &&
                                                                                                 x.Id_Venda == venda.Id))
                {
                    Model.nhibernate.Pool_Ip_Endereco Endereco_IP_Antigo = null;
                    Model.nhibernate.Pool_Ip_Endereco Endereco_IP = null;
                    Model.nhibernate.Pool_Ip pool_Ip = null;

                    Model.nhibernate.LoginRadius login = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == cliente_plano.Id);
                    Model.nhibernate.LoginRadius loginClone = login != null ? (Model.nhibernate.LoginRadius)login.Clone() : null;
                    PlanoProdutoListModel planoModel = planos.FirstOrDefault(p => p.Id_Plano_Produto == cliente_plano.Plano?.Id);

                    if (login != null) //Login Existente
                    {

                        if (!String.IsNullOrEmpty(cliente_plano.Login_Radius) && !String.IsNullOrEmpty(cliente_plano.Senha_Radius))
                        {
                            login.Login = cliente_plano.Login_Radius.ToLowerInvariant();
                            login.SenhaPPPOE = cliente_plano.Senha_Radius.ToLowerInvariant();
                        }
                        else
                        {
                            string primeiroNome = String.Empty;
                            string ultimoNome = String.Empty;

                            int idxPrimeiroEspaco = cliente.NomeFantasia.IndexOf(" ");
                            int idxUltimoEspaco = cliente.NomeFantasia.LastIndexOf(" ");

                            // Se o nome tem espaço preenche o primeiro e o último nome
                            if (idxPrimeiroEspaco != -1)
                            {
                                primeiroNome = cliente.NomeFantasia.Substring(0, idxPrimeiroEspaco);
                                ultimoNome = cliente.NomeFantasia.Substring(idxUltimoEspaco + 1, cliente.NomeFantasia.Length - idxUltimoEspaco - 1);
                            }
                            // Caso o nome não tenha nenhum espaço preenche somente o primeiro nome
                            else
                            {
                                primeiroNome = cliente.NomeFantasia;
                            }

                            login.Login = (config.Padrao_Login.Replace("PrimeiroNome", primeiroNome)
                                                     .Replace("UltimoNome", ultimoNome)
                                                     .Replace("Contrato", cliente?.Numero_Contrato)
                                                     .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                     .Replace("Código", Convert.ToString(cliente?.Id))
                                                     .Replace("Email", cliente?.Email_Principal)
                                                     .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                     .Replace("Uf", cliente.Endereco_Principal?.UF)
                                                     .Replace("Cidade", cliente.Endereco_Principal?.Cidade));
                            var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(login.Login);

                            login.Login = verificaLoginComSequencial;

                            login.SenhaPPPOE = (config.Padrao_Login_Ultimo.Replace("PrimeiroNome", primeiroNome)
                                                         .Replace("UltimoNome", ultimoNome)
                                                         .Replace("Contrato", cliente?.Numero_Contrato)
                                                         .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                         .Replace("Código", Convert.ToString(cliente?.Id))
                                                         .Replace("Email", cliente?.Email_Principal)
                                                         .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                         .Replace("Uf", cliente.Endereco_Principal?.UF)
                                                         .Replace("Cidade", cliente.Endereco_Principal?.Cidade));

                            var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(login.SenhaPPPOE);

                            login.SenhaPPPOE = verificaSenhaComSequencial;
                        }

                        //login.SenhaPPPOE = cliente_plano.Senha_Radius;

                        if (!login.Tipo_Autenticacao_Login.HasValue)
                            login.Tipo_Autenticacao_Login = Tipo_Autenticacao_Login.PPPOE;

                        long? id_Concentradora = await ApiConcentradora.GetConcentradoraByUsuarios(cliente_plano.Cod_Escritorio, cliente_plano.Cod_Area, cliente_plano.Cod_Localidade.GetValueOrDefault(0));
                        //venda.Porta_Reservada = portaResev.Status != Situacao_Porta.Reservado ? Sinapse.Model.enumerados.Padrao.Nao : Sinapse.Model.enumerados.Padrao.Sim;
                        login.Concentradora = (await repoConcentradora.select(p => p.Id == id_Concentradora)).FirstOrDefault();

                        if (planoModel?.Id_Pool_Ip_Endereco != null)
                        {
                            pool_Ip = cliente_plano.Pool_Ip;
                            Endereco_IP_Antigo = login.Pool_Ip_Endereco;
                            Endereco_IP = cliente_plano.Pool_Ip?.Pool_Ip_Enderecos.First(p => p.Id == planoModel.Id_Pool_Ip_Endereco);
                            if (cliente_plano.Pool_Ip?.Padrao_Ip == Tipo_Ip.IPv4)
                                login.EnderecoIPV4 = Endereco_IP.Ip;
                            else
                                login.EnderecoIPV6 = Endereco_IP.Ip;

                            login.Pool_Ip_Endereco = Endereco_IP;
                            login.Pool_Ip_Endereco.Pool_Ip.Id = pool_Ip.Id;
                        }
                        else //Atribuição de Ip Automática
                        {
                            long? id_Pool_Ip = await ApiPool_Ip.GetPoolIpPadraoConcentradora(id_Concentradora) ?? await ApiPool_Ip.GetPoolIpMenosUsuarios(id_Concentradora);
                            if (id_Pool_Ip != null)
                            {

                                pool_Ip = await repoPoolIp.selectFirst(p => p.Id == id_Pool_Ip);
                                login.Cliente_PlanoProduto = cliente_plano;
                                login.Cliente_PlanoProduto.Pool_Ip = pool_Ip;
                                login.Cliente_PlanoProduto.Pool_Ip.Id = pool_Ip.Id;
                                Endereco_IP = pool_Ip.Pool_Ip_Enderecos.First(p => p.Status_Ip == Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel);
                                if (pool_Ip.IsPoolIpFixo == true)
                                {

                                    if (pool_Ip?.Padrao_Ip == Tipo_Ip.IPv4)
                                        login.EnderecoIPV4 = Endereco_IP.Ip;
                                    else
                                        login.EnderecoIPV6 = Endereco_IP.Ip;

                                    login.Pool_Ip_Endereco = Endereco_IP;
                                }

                                //login.Pool_Ip_Endereco.Pool_Ip.Id = pool_Ip.Id;
                            }
                        }
                    }
                    else if (!cliente_plano.FlagTrocaPlano) //Novo Login
                    {
                        login = new Model.nhibernate.LoginRadius();
                        long? id_Concentradora = await ApiConcentradora.GetConcentradoraByUsuarios(cliente_plano.Cod_Escritorio,
                                                                                                                       cliente_plano.Cod_Area,
                                                                                                                       cliente_plano.Cod_Localidade.GetValueOrDefault(0));

                        login.Concentradora = await repoConcentradora.selectFirst(p => p.Id == id_Concentradora);


                        if (!String.IsNullOrEmpty(cliente_plano.Login_Radius) && !String.IsNullOrEmpty(cliente_plano.Senha_Radius))
                        {
                            var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(cliente_plano.Login_Radius);
                            var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(cliente_plano.Senha_Radius);
                            login.Login = verificaLoginComSequencial.ToLowerInvariant();
                            login.SenhaPPPOE = verificaSenhaComSequencial.ToLowerInvariant();
                        }
                        else
                        {
                            string primeiroNome = String.Empty;
                            string ultimoNome = String.Empty;

                            int idxPrimeiroEspaco = cliente.NomeFantasia.IndexOf(" ");
                            int idxUltimoEspaco = cliente.NomeFantasia.LastIndexOf(" ");

                            // Se o nome tem espaço preenche o primeiro e o último nome
                            if (idxPrimeiroEspaco != -1)
                            {
                                primeiroNome = cliente.NomeFantasia.Substring(0, idxPrimeiroEspaco);
                                ultimoNome = cliente.NomeFantasia.Substring(idxUltimoEspaco + 1, cliente.NomeFantasia.Length - idxUltimoEspaco - 1);
                            }
                            // Caso o nome não tenha nenhum espaço preenche somente o primeiro nome
                            else
                            {
                                primeiroNome = cliente.NomeFantasia;
                            }

                            string gerarLogin = (config.Padrao_Login.Replace("PrimeiroNome", primeiroNome.ToLowerInvariant())
                                                     .Replace("UltimoNome", ultimoNome.ToLowerInvariant())
                                                         .Replace("Contrato", cliente?.Numero_Contrato)
                                                     .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                         .Replace("Código", Convert.ToString(cliente?.Id))
                                                     .Replace("Email", cliente?.Email_Principal?.ToLowerInvariant() ?? "default")
                                                         .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                     .Replace("Uf", cliente.Endereco_Principal?.UF.ToLowerInvariant())
                                                     .Replace("Cidade", cliente.Endereco_Principal?.Cidade.ToLowerInvariant()));
                            login.Login = gerarLogin;

                            var verificaLoginComSequencial = await new LoginRadiusApiController().GerarLoginComSequencial(login.Login);

                            login.Login = verificaLoginComSequencial;

                            string gerarSenhaPPPOE = (config.Padrao_Login_Ultimo.Replace("PrimeiroNome", primeiroNome.ToLowerInvariant())
                                                          .Replace("UltimoNome", ultimoNome.ToLowerInvariant())
                                                             .Replace("Contrato", cliente?.Numero_Contrato)
                                                          .Replace("Cont.PlanoseProdutos", cliente_plano?.ClienteContrato?.Id.ToString())
                                                             .Replace("Código", Convert.ToString(cliente?.Id))
                                                          .Replace("Email", cliente?.Email_Principal?.ToLowerInvariant() ?? "default")
                                                             .Replace("CPF/CNPJ", cliente?.CpfCnpj)
                                                          .Replace("Uf", cliente.Endereco_Principal?.UF.ToLowerInvariant())
                                                          .Replace("Cidade", cliente.Endereco_Principal?.Cidade.ToLowerInvariant()));

                            login.SenhaPPPOE = gerarSenhaPPPOE;

                            var verificaSenhaComSequencial = await new LoginRadiusApiController().GerarSenhaComSequencial(login.SenhaPPPOE);

                            login.SenhaPPPOE = verificaSenhaComSequencial;
                        }

                        if (planoModel?.Id_Pool_Ip_Endereco != null)
                        {
                            pool_Ip = cliente_plano.Pool_Ip;
                            Endereco_IP = cliente_plano.Pool_Ip?.Pool_Ip_Enderecos.FirstOrDefault(p => p.Id == planoModel.Id_Pool_Ip_Endereco);
                            login.Cliente_PlanoProduto = cliente_plano;
                            login.Cliente_PlanoProduto.Pool_Ip = pool_Ip;
                            login.Cliente_PlanoProduto.Pool_Ip.Id = pool_Ip.Id;

                            if (pool_Ip.IsPoolIpFixo == true)
                            {
                                if (cliente_plano.Pool_Ip?.Padrao_Ip == Tipo_Ip.IPv4)
                                    login.EnderecoIPV4 = Endereco_IP?.Ip;
                                else
                                    login.EnderecoIPV6 = Endereco_IP?.Ip;

                                login.Pool_Ip_Endereco = Endereco_IP;
                            }

                            //login.Pool_Ip_Endereco.Pool_Ip.Id = pool_Ip.Id;
                        }
                        else //Atribuição de Ip Automática
                        {
                            long? id_Pool_Ip = await ApiPool_Ip.GetPoolIpPadraoConcentradora(id_Concentradora) ?? await ApiPool_Ip.GetPoolIpMenosUsuarios(id_Concentradora);
                            if (id_Pool_Ip != null)
                            {
                                pool_Ip = await repoPoolIp.selectFirst(p => p.Id == id_Pool_Ip);
                                Endereco_IP = pool_Ip.Pool_Ip_Enderecos.FirstOrDefault(p => p.Status_Ip == Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel);
                                login.Cliente_PlanoProduto = cliente_plano;
                                login.Cliente_PlanoProduto.Pool_Ip = pool_Ip;
                                login.Cliente_PlanoProduto.Pool_Ip.Id = pool_Ip.Id;
                                if (pool_Ip.IsPoolIpFixo == true)
                                {
                                    if (pool_Ip?.Padrao_Ip == Tipo_Ip.IPv4)
                                        login.EnderecoIPV4 = Endereco_IP?.Ip;
                                    else
                                        login.EnderecoIPV6 = Endereco_IP?.Ip;

                                    login.Pool_Ip_Endereco = Endereco_IP;
                                }


                            }

                        }

                        login.Cliente = cliente;
                        login.Cliente_PlanoProduto = cliente_plano;
                        login.Cliente_PlanoProduto.Login_Radius = login.Login;
                        login.Cliente_PlanoProduto.Senha_Radius = login.SenhaPPPOE;

                        if (!login.Tipo_Autenticacao_Login.HasValue)
                            login.Tipo_Autenticacao_Login = Tipo_Autenticacao_Login.PPPOE;

                    }

                    bool loginPossuiAlteracao = false;
                    if (loginClone != null && login != null)
                    {
                        if (login.Login != loginClone.Login)
                        {
                            loginPossuiAlteracao = true;
                        }

                        if (login.SenhaPPPOE != loginClone.SenhaPPPOE)
                        {
                            loginPossuiAlteracao = true;
                        }

                        if (login.Tipo_Autenticacao_Login != loginClone.Tipo_Autenticacao_Login)
                        {
                            loginPossuiAlteracao = true;
                        }

                        if (login.Pool_Ip_Endereco != loginClone.Pool_Ip_Endereco)
                        {
                            loginPossuiAlteracao = true;
                        }
                    }
                    else if (loginClone == null && login != null)
                    {
                        loginPossuiAlteracao = true;
                    }

                    if (login != null && loginPossuiAlteracao == true)
                    {
                        if (login.Concentradora != null)
                        {
                            //Ignorando essa crítica do cadastro de Login pois na Venda a reserva de Caixa/Porta só está sendo realizada mais à frente.
                            bool ignorarCriticaCaixaPorta = true;
                            LoginRadiusCadastroModel loginCadastro = new LoginRadiusApiController().GetViewObj(login, null, ignorarCriticaCaixaPorta);
                            ParamWebApi<LoginRadiusCadastroModel> paramWebApi = new ParamWebApi<LoginRadiusCadastroModel>
                            {
                                Objeto = loginCadastro,
                                Usuario = User.Identity.GetUserName()

                            };

                            var cadastrarLoginRadius = await new LoginRadiusApiController().Gravar(paramWebApi);
                        }
                        else
                        {
                            await repoLoginRadius.SaveOrUpdate(login);
                        }

                        if (planoModel?.Id_Pool_Ip_Endereco != null)
                        {
                            Endereco_IP.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Ocupado;
                            Endereco_IP.Login = login;
                            await repoPoolIpEndereco.SaveOrUpdate(Endereco_IP);

                            if (Endereco_IP_Antigo != null && Endereco_IP_Antigo.Id != Endereco_IP.Id) //Libera o IP usado anteriormente
                            {
                                Endereco_IP_Antigo.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                Endereco_IP_Antigo.Login = null;
                                await repoPoolIpEndereco.SaveOrUpdate(Endereco_IP_Antigo);
                            }
                        }



                    }
                }
            }
            #endregion
        }

        #region Restricoes

        [HttpPost()]
        [Route("api/Vendaapi/ResultParametrosRestricaoCreditoVenda")]
        public async Task<GetApiRetorno<string>> ResultParametrosRestricaoVenda(RestricoesConsultaCreditoVenda request)
        {
            var retorno = new GetApiRetorno<string>();

            try
            {

                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                    IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    using (Conexao<IntegracoesExternas> repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                    using (Conexao<TrocaTitularidade> repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
                    {

                        IntegracoesExternas configIntegracaoSerasaPreQual = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.PREQUAL).Result;
                        IntegracoesExternas configIntegracaoSerasa = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA).Result;
                        IntegracoesExternas configIntegracaoSerasaExp = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;

                        var consultaCreditoBusiness =
                        new ConsultaCreditoBusiness(
                            repoConsulta,
                            repoCriterio,
                            repoParams,
                            repoConfig
                        );


                        IVendaRepository repoVenda = new VendaRepository(sessionNh);
                        var vendaBusiness = new VendaBusiness(repoVenda);

                        Venda venda = null;
                        TrocaTitularidade troca = null;

                        if (request.IdVenda != 0)
                        {
                            venda = vendaBusiness.GetVendaById(request.IdVenda);
                        }
                        else
                        {
                            troca = await repoTroca.selectFirst(x => x.Id == request.IdTroca);
                        }

                        if (venda?.IdConsultaCredito != null)
                        {
                            IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                            var clienteBusiness = new ClienteBusiness(repoCliente);

                            Cliente cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                            if (cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica)
                            {
                                if (configIntegracaoSerasa != null || configIntegracaoSerasaPreQual != null || configIntegracaoSerasaExp != null)
                                {
                                    var responseConsultaStatus = consultaCreditoBusiness.GetStatusConsultaCreditoCpfByConsulta(venda.IdConsultaCredito.Value,
                                                                                                                              cliente.CpfCnpj);

                                    retorno.Objeto = HtmlEnumExtensions.GetEnumDescription(responseConsultaStatus.valor);
                                }
                            }
                            else if (cliente.Tipo_Pessoa == Tipo_Pessoa.Juridica)
                            {
                                if (configIntegracaoSerasa != null || configIntegracaoSerasaPreQual != null || configIntegracaoSerasaExp != null)
                                {
                                    var responseConsultaStatus = consultaCreditoBusiness.GetStatusConsultaCreditoCnpjByConsulta(venda.IdConsultaCredito.Value,
                                                                                                                               cliente.CpfCnpj);

                                    retorno.Objeto = HtmlEnumExtensions.GetEnumDescription(responseConsultaStatus.valor);
                                }
                            }
                        }
                        if (troca?.IdConsultaCredito != null)
                        {
                            IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                            var clienteBusiness = new ClienteBusiness(repoCliente);

                            Cliente cliente = clienteBusiness.GetById(troca.ClienteTitularNovo.Id);

                            if (cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica)
                            {
                                if (configIntegracaoSerasa != null || configIntegracaoSerasaPreQual != null)
                                {
                                    var responseConsultaStatus = consultaCreditoBusiness.GetStatusConsultaCreditoCpfByConsulta(troca.IdConsultaCredito.Value,
                                                                                                                              cliente.CpfCnpj);

                                    retorno.Objeto = HtmlEnumExtensions.GetEnumDescription(responseConsultaStatus.valor);
                                }
                            }
                            else if (cliente.Tipo_Pessoa == Tipo_Pessoa.Juridica)
                            {
                                if (configIntegracaoSerasa != null || configIntegracaoSerasaPreQual != null)
                                {
                                    var responseConsultaStatus = consultaCreditoBusiness.GetStatusConsultaCreditoCnpjByConsulta(troca.IdConsultaCredito.Value,
                                                                                                                               cliente.CpfCnpj);

                                    retorno.Objeto = HtmlEnumExtensions.GetEnumDescription(responseConsultaStatus.valor);
                                }
                            }
                        }
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               ex.Message,
                                               ex?.InnerException?.Message,
                                               ex.StackTrace);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Vendaapi/ResultParametrosRestricaoCredito")]
        public GetApiRetorno<string> ResultParametrosRestricao(RestricoesConsultaCredito request)
        {
            var retorno = new GetApiRetorno<string>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                    IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var consultaCreditoBusiness =
                        new ConsultaCreditoBusiness(
                            repoConsulta,
                            repoCriterio,
                            repoParams,
                            repoConfig
                        );

                    string documento = request.cpf.Replace("-", String.Empty)
                                                  .Replace(".", String.Empty)
                                                  .Replace("/", String.Empty);

                    bool pessoaFisica = documento.Length == 11;

                    ConsultaSerasaDados consulta = null;


                    if (pessoaFisica)
                    {
                        consulta = consultaCreditoBusiness.GetConsultaCreditoByCpf(documento);
                    }
                    else
                    {
                        consulta = consultaCreditoBusiness.GetConsultaCreditoByCnpj(documento);
                    }


                    if (consulta != null)
                    {
                        if (pessoaFisica)
                        {
                            retorno.Objeto = consulta.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN ? HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.SerasaStatusRetorno(consulta.consulta, consulta.Id, documento, true)) :
                                             HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCpfByConsulta(consulta.Id, documento).valor);
                        }
                        else
                        {
                            retorno.Objeto = consulta.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN ? HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.SerasaStatusRetorno(consulta.consulta, consulta.Id, documento, false)) :
                                             HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCnpjByConsulta(consulta.Id, documento).valor);
                        }
                        retorno.Ok = true;
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               ex.Message,
                                               ex?.InnerException?.Message,
                                               ex.StackTrace);
            }

            return retorno;
        }


        [HttpPost]
        [Route("api/Vendaapi/TotalSomaPontosRestricaoCreditoVenda")]
        public GetApiRetorno<string> TotalSomaPontosRestricaoVenda(RestricoesConsultaCreditoVenda request)
        {
            var retorno = new GetApiRetorno<string>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoConsultadados = new Conexao<ConsultaSerasaDados>(sessionNh))
                {
                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                    IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var consultaCreditoBusiness = new ConsultaCreditoBusiness(repoConsulta, repoCriterio, repoParams, repoConfig);

                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);

                    if (venda?.IdConsultaCredito != null)
                    {
                        ConsultaSerasaDados consultaSerasa = repoConsultadados.selectFirst(d => d.Id == venda.IdConsultaCredito.Value).Result;

                        if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
                        {
                            retorno.Objeto = null;
                        }
                        else if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
                        {
                            IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                            var clienteBusiness = new ClienteBusiness(repoCliente);

                            Cliente cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                            if (cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica)
                            {
                                ConsultaSerasaResponse serasaExp = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);

                                int pontos = consultaCreditoBusiness.GetTotalPontosSerasaExpCpf(venda.IdConsultaCredito.Value, cliente.CpfCnpj, serasaExp);

                                // O valor Int32.MinValue quando retornado pelo GetTotalPontosCpf indica que a consulta de crédito foi reprovada
                                // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                                // da consulta
                                if (pontos == Int32.MinValue)
                                    retorno.Objeto = "Critério Eliminatório";
                                else
                                    retorno.Objeto = Convert.ToString(pontos);
                            }
                            else if (cliente.Tipo_Pessoa == Tipo_Pessoa.Juridica)
                            {
                                ConsultaSerasaResponse serasaExp = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);

                                int pontos = consultaCreditoBusiness.GetTotalPontosSerasaExpCnpj(venda.IdConsultaCredito.Value, cliente.CpfCnpj, serasaExp);

                                // O valor Int32.MinValue quando retornado pelo GetTotalPontosCnpj indica que a consulta de crédito foi reprovada
                                // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                                // da consulta
                                if (pontos == Int32.MinValue)
                                    retorno.Objeto = "Critério Eliminatório";
                                else
                                    retorno.Objeto = Convert.ToString(pontos);
                            }
                        }
                        else
                        {
                            IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                            var clienteBusiness = new ClienteBusiness(repoCliente);

                            Cliente cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                            if (cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica)
                            {
                                int pontos = consultaCreditoBusiness.GetTotalPontosConsultaCpf(venda.IdConsultaCredito.Value, cliente.CpfCnpj);

                                // O valor Int32.MinValue quando retornado pelo GetTotalPontosCpf indica que a consulta de crédito foi reprovada
                                // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                                // da consulta
                                if (pontos == Int32.MinValue)
                                    retorno.Objeto = "Critério Eliminatório";
                                else
                                    retorno.Objeto = Convert.ToString(pontos);
                            }
                            else if (cliente.Tipo_Pessoa == Tipo_Pessoa.Juridica)
                            {
                                int pontos = consultaCreditoBusiness.GetTotalPontosConsultaCnpj(venda.IdConsultaCredito.Value, cliente.CpfCnpj);

                                // O valor Int32.MinValue quando retornado pelo GetTotalPontosCnpj indica que a consulta de crédito foi reprovada
                                // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                                // da consulta
                                if (pontos == Int32.MinValue)
                                    retorno.Objeto = "Critério Eliminatório";
                                else
                                    retorno.Objeto = Convert.ToString(pontos);
                            }
                        }
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               ex.Message,
                                               ex?.InnerException?.Message,
                                               ex.StackTrace);
            }

            return retorno;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Vendaapi/TotalSomaPontosRestricaoCredito")]
        public GetApiRetorno<string> TotalSomaPontosRestricao(RestricoesConsultaCredito request)
        {
            var retorno = new GetApiRetorno<string>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                    IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var consultaCreditoBusiness = new ConsultaCreditoBusiness(repoConsulta, repoCriterio, repoParams, repoConfig);

                    string documento = request.cpf.Replace("-", String.Empty)
                                                  .Replace(".", String.Empty)
                                                  .Replace("/", String.Empty);

                    bool pessoaFisica = documento.Length == 11;

                    if (pessoaFisica)
                    {
                        ConsultaSerasaDados consultaCredito = consultaCreditoBusiness.GetConsultaCreditoByCpf(documento);

                        if (consultaCredito == null)
                        {
                            throw new ConsultaSerasaNaoEncontradaException(documento);
                        }

                        if (consultaCredito.consulta.Contains("CPF SEM TITULAR"))
                            throw new ConsultaCreditoDocumentoSemTitularException();
                        else if (consultaCredito.consulta.Contains("MENOR DE IDADE PROTEGIDO LEGALMENTE"))
                            throw new ConsultaCreditoMenoridadeException();


                        int pontos = consultaCreditoBusiness.GetTotalPontosConsultaCpf(consultaCredito.Id, documento);

                        // O valor Int32.MinValue quando retornado pelo GetTotalPontosCpf indica que a consulta de crédito foi reprovada
                        // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                        // da consulta
                        if (pontos == Int32.MinValue)
                            retorno.Objeto = "Critério Eliminatório";
                        else
                            retorno.Objeto = Convert.ToString(pontos);
                    }
                    else
                    {
                        int pontos = consultaCreditoBusiness.GetTotalPontosCnpj(documento);

                        // O valor Int32.MinValue quando retornado pelo GetTotalPontosCnpj indica que a consulta de crédito foi reprovada
                        // por conta de algum critério eliminatório. Nesse caso, não mostramos a soma dos pontos na exibição do resultado
                        // da consulta
                        if (pontos == Int32.MinValue)
                            retorno.Objeto = "Critério Eliminatório";
                        else
                            retorno.Objeto = Convert.ToString(pontos);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               ex.Message,
                                               ex?.InnerException?.Message,
                                               ex.StackTrace);
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Vendaapi/RetornoRestricaoCreditoVenda")]
        public async Task<GetApiRetorno<String>> RetornoRestricaoCreditoVenda(RestricoesConsultaCreditoVenda request)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();
            string result = String.Empty;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConsulta = new Conexao<ConsultaSerasaDados>(session))
                using (var repoIdentity = new Conexao<IdentityUser>(session))
                {
                    IVendaRepository repoVenda = new VendaRepository(session);
                    var vendaBusiness = new VendaBusiness(repoVenda);


                    IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(session);
                    ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(session);
                    IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(session);
                    IParametrosConsultaPreQualRepository repoParamsConsultaPrequal = new ParametrosConsultaPreQualRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    IIntegracaoExternaRepository repoIntegracao = new IntegracaoExternaRepository(session);
                    IClienteRepository repoClienteNH = new ClienteRepository(session);

                    var consultaCreditoBusiness =
                    new ConsultaCreditoBusiness(
                            repoConsultaCredito,
                            repoCriterios,
                            repoParamsConsulta,
                            repoConfig
                    );



                    Venda venda = vendaBusiness.GetVendaById(request.IdVenda);
                    var identityUser = await repoIdentity.selectFirst(p => p.Funcionario.Id == Convert.ToInt64(venda.Vendedor.Id));
                    if (venda?.IdConsultaCredito != null)
                    {
                        IClienteRepository repoCliente = new ClienteRepository(session);
                        var clienteBusiness = new ClienteBusiness(repoCliente);

                        Cliente cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                        bool pessoaFisica = cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica;
                        string documento = cliente.CpfCnpj.Replace("-", String.Empty)
                                                          .Replace(".", String.Empty)
                                                          .Replace("/", String.Empty);

                        ConsultaSerasaDados consultaSerasa = await repoConsulta.selectFirst(d => d.Id == venda.IdConsultaCredito.Value);

                        String consulta = consultaSerasa.consulta;
                        String dataConsulta = consultaSerasa.DataHoraAlteracao.HasValue ? consultaSerasa.DataHoraAlteracao.Value.ToString("dd/MM/yyyy") : consultaSerasa.DataHoraCriacao.Value.ToString("dd/MM/yyyy");

                        if (String.IsNullOrEmpty(consulta))
                        {
                            throw new ConsultaSerasaNaoEncontradaException(documento);
                        }

                        if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
                        {
                            if (pessoaFisica)
                            {
                                ResponsePreQual responsePreQual = JsonConvert.DeserializeObject<ResponsePreQual>(consultaSerasa.consulta);
                                string classificacaoDecisao = responsePreQual.data?.DVResults?.MergedResults?[0]?.ResultadoDecisao?.ClassificacaoDecisao;
                                string mensagemDecisao = responsePreQual.data?.DVResults?.MergedResults?[0]?.ResultadoDecisao?.MensagemDecisao;
                                result = $"<div class='col'>" +
                                         $"<div class='row'><b>Perfil utilizado:</b> {consultaSerasa.ParamLoja}</div>" +
                                         $"<div class='row'><b>Decisão:</b> {classificacaoDecisao}</div>" +
                                         $"<div class='row'><b> Mensagem Decisão:</b> {mensagemDecisao}</div>" +
                                         $"</div>";

                                retorno.Objeto = result;
                                retorno.Ok = true;
                            }
                            else
                            {

                                ResponsePreQual responsePreQual = JsonConvert.DeserializeObject<ResponsePreQual>(consultaSerasa.consulta);
                                string classificacaoDecisao = responsePreQual.data?.DVResults?.MergedResults?[0]?.ResultadoDecisao?.ClassificacaoDecisao;
                                string mensagemDecisao = responsePreQual.data?.DVResults?.MergedResults?[0]?.ResultadoDecisao?.MensagemDecisao;
                                result = $"<div class='col'>" +
                                         $"<div class='row'><b>Perfil utilizado:</b> {consultaSerasa.ParamLoja}</div>" +
                                         $"<div class='row'><b>Decisão:</b> {classificacaoDecisao}</div>" +
                                         $"<div class='row'><b> Mensagem Decisão:</b> {mensagemDecisao}</div>" +
                                         $"</div>";

                                retorno.Objeto = result;
                                retorno.Ok = true;
                            }
                        }
                        else if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
                        {
                            result = consultaCreditoBusiness.GetHtmlResumoConsultaSerasaExperian(consulta, documento, pessoaFisica).Result;

                            retorno.Objeto = result;
                            retorno.Ok = true;
                        }
                        else
                        {
                            result = consultaCreditoBusiness.GetHtmlResumoConsulta(consulta, documento).Result;

                            retorno.Objeto = result;
                            retorno.Ok = true;
                        }
                    }
                    else
                    {
                        throw new Exception("Não foi encontrada consulta de crédito");
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                               ex.Message,
                                               ex?.InnerException?.Message,
                                               ex.StackTrace);
            }
            return retorno;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("api/Vendaapi/RetornoRestricaoCredito")]
        public async Task<GetApiRetorno<String>> RetornoRestricaoCredito(RestricoesConsultaCredito request)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();
            string result = String.Empty;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConsulta = new Conexao<ConsultaSerasaDados>(session))
                {
                    string documento = request.cpf.Replace("-", String.Empty)
                                                  .Replace(".", String.Empty)
                                                  .Replace("/", String.Empty);

                    bool pessoaFisica = documento.Length == 11;

                    ConsultaSerasaDados consultaSerasa = null;

                    if (pessoaFisica)
                    {
                        consultaSerasa = (await repoConsulta.select(d => d.cpf == documento)).OrderByDescending(d => d.Id).FirstOrDefault();
                    }
                    else
                    {
                        consultaSerasa = (await repoConsulta.select(d => d.cnpj == documento)).OrderByDescending(d => d.Id).FirstOrDefault();
                    }

                    String consulta = consultaSerasa.consulta;
                    String dataConsulta = consultaSerasa.DataHoraAlteracao.HasValue ? consultaSerasa.DataHoraAlteracao.Value.ToString("dd/MM/yyyy") : consultaSerasa.DataHoraCriacao.Value.ToString("dd/MM/yyyy");

                    if (String.IsNullOrEmpty(consulta))
                    {
                        throw new ConsultaSerasaNaoEncontradaException(documento);
                    }

                    IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(session);
                    ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(session);
                    IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(session);
                    IParametrosConsultaPreQualRepository repoParamsConsultaPrequal = new ParametrosConsultaPreQualRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);

                    var consultaCreditoBusiness = new ConsultaCreditoBusiness(
                        repoConsultaCredito,
                            repoCriterios,
                            repoParamsConsulta,
                            repoConfig);


                    if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
                    {
                        result = consultaCreditoBusiness.GetHtmlResumoConsultaSerasaExperian(consulta, documento, pessoaFisica).Result;

                        retorno.Objeto = result;
                        retorno.Ok = true;
                    }
                    else
                    {
                        result = consultaCreditoBusiness.GetHtmlResumoConsulta(consulta, documento).Result;

                        retorno.Objeto = result;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}",
                                                ex.Message,
                                                ex?.InnerException?.Message,
                                                ex.StackTrace);
            }
            return retorno;
        }

        #endregion

        #region IntegracaoFTTH
        private List<PostApiMensagem> IntegracaoFTTH_Venda(IntegracaoExterna TipoIntegracao, long id_ClientePlanoProduto, long porta)
        {
            List<PostApiMensagem> retorno = new List<PostApiMensagem>();
            PostApiRetorno response = new PostApiRetorno();

            using (Elemento_Rede_PortaApiController elemento_Rede_PortaApi = new Elemento_Rede_PortaApiController())
            using (Elemento_RedeApiController elemento_RedeApi = new Elemento_RedeApiController())
            {

                switch (TipoIntegracao)
                {
                    case IntegracaoExterna.GEOSITE:
                        Ligacao_Porta_GeositeRequestModel requestGeosite = new Ligacao_Porta_GeositeRequestModel
                        {
                            cliente_PlanoProduto = id_ClientePlanoProduto,
                            porta = porta,
                        };
                        response = elemento_Rede_PortaApi.Ligacao_ReservaPorta_Geosite(requestGeosite).Result;

                        PostApiMensagem message = response.Ok ? new PostApiMensagem { Tipo = PostApiMensagemType.Info } :
                                                                new PostApiMensagem { Tipo = PostApiMensagemType.Erro };

                        message.Mensagem = response.Mensagem ?? String.Empty;
                        retorno.Add(message);

                        break;

                }
            }
            return retorno;
        }
        #endregion

        #region Tarefa telefonia
        private async Task<List<PostApiMensagem>> Tarefa_Telefonia(ISession session, Cliente_PlanoProduto planoproduto)
        {
            List<PostApiMensagem> retorno = new List<PostApiMensagem>();

            using (Conexao<Did> repo_Did = new Conexao<Did>(session))
            using (Conexao<CircuitoTelefonia> repoCircuitoTelefonia = new Conexao<CircuitoTelefonia>(session))
            using (Conexao<CircuitoTelefoniaDid> repo_Did_Circuito = new Conexao<CircuitoTelefoniaDid>(session))
            using (Conexao<IntegracoesExternas> repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))

            using (DidApiController DidApi = new DidApiController())
            using (CircuitoTelefoniaApiController CircuitoApi = new CircuitoTelefoniaApiController())
            {
                IntegracoesExternas integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                if (!String.IsNullOrEmpty(planoproduto.JsonTelefonia))
                {
                    //CircuitoTelefoniaDid obj = new CircuitoTelefoniaDid();
                    CircuitoTelefonia circuito = new CircuitoTelefonia();

                    //dynamic dynamic_Telefonia = JsonConvert.DeserializeObject<dynamic>(plano_produto.JsonTelefonia);
                    GetApiRetorno<Json_VendaTelefonia> teste = (Recurso_TelefoniaDynamic(planoproduto.JsonTelefonia));

                    int num_didsDidsponiveis = (await repo_Did.select(p => p.IdVenda == null)).Count();
                    //teste.Objeto.didObj.Select(p => { p.IdVenda = objVenda?.Id; return p; });

                    if (teste.Objeto.Numero_dids > num_didsDidsponiveis)
                    {
                        retorno.Add(new PostApiMensagem
                        {
                            Mensagem = "Sem Disponibilidade de Dids para a Venda",
                            Tipo = PostApiMensagemType.Erro,
                        });

                    }
                    else
                    {
                        circuito.Clientes_PlanosProdutos = planoproduto;
                        circuito.Id_Venda = planoproduto.Id_Venda.HasValue ? planoproduto.Id_Venda.Value : 0;

                        circuito.HabilitarLc = teste.Objeto.Lc.Habilitar.HasValue ? teste.Objeto.Lc.Habilitar.Value : false;
                        circuito.HabilitarLdn = teste.Objeto.Ldn.Habilitar.HasValue ? teste.Objeto.Ldn.Habilitar.Value : false;
                        circuito.HabilitarVc1 = teste.Objeto.Vc1.Habilitar.HasValue ? teste.Objeto.Vc1.Habilitar.Value : false;
                        circuito.HabilitarVc2 = teste.Objeto.Vc2.Habilitar.HasValue ? teste.Objeto.Vc2.Habilitar.Value : false;
                        circuito.HabilitarVc3 = teste.Objeto.Vc3.Habilitar.HasValue ? teste.Objeto.Vc3.Habilitar.Value : false;

                        circuito.Status = Status_Default.Ativo;
                        circuito.Senha = CircuitoApi.Generate_PasswordsCircuito().Objeto;

                        await repoCircuitoTelefonia.SaveOrUpdate(circuito);

                        if (integracao != null)
                        {

                            Integracoes.Saperx.Models.CircuitoIntegracaoResponse resultado = await CircuitoApi.Integracao_Cadastro_Circuito(circuito);
                            circuito.CodigoAuxiliar = Convert.ToString(resultado?.result?.id);
                            circuito.Numero_Sip = resultado?.result?.sip_number;
                            await repoCircuitoTelefonia.Update(circuito);
                        }

                        GetApiRetorno<List<Did>> Did_Disponivel = (await DidApi.Get_Random_Did(new Did_Random_ApiRequest
                        {
                            Id_Cidade = Convert.ToString(planoproduto.Endereco_Instalacao?.Id_Cidade),
                            Id_Pais = Convert.ToString(planoproduto.Endereco_Instalacao?.Pais?.Id),
                            UF = Convert.ToString(planoproduto.Endereco_Instalacao?.UF),
                            NumeroDids = teste.Objeto.Numero_dids
                        }, session));

                        if (Did_Disponivel?.Objeto != null && Did_Disponivel?.Objeto.Count() > 0)
                        {
                            foreach (Did did in Did_Disponivel.Objeto)
                            {
                                did.Circuito = circuito;
                                did.StatusDid = StatusDid.Reservado;
                            }

                            //Did_Disponivel.Objeto.Select(p => { p.StatusDid = StatusDid.Reservado; p.Circuito = circuito; return p; });
                            //obj.Did.Add(Did_Disponivel.Objeto);
                            circuito.Did = Did_Disponivel.Objeto;
                            await repo_Did.Update(Did_Disponivel?.Objeto);
                            await repoCircuitoTelefonia.Update(circuito);


                            if (integracao != null)
                            {
                                foreach (Did didItem in Did_Disponivel?.Objeto)
                                    await DidApi.Integracao_Editar_DID(didItem);
                            }
                        }
                    }

                }
                return retorno;
            }
        }
        #endregion

        [HttpPost()]
        [Route("api/Vendaapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(VendaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                retorno = await AlterarStatusPrivate(value);

                using (var session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (Conexao<LogVendas> repositoryLogVenda = new Conexao<LogVendas>(sessionLog))
                {
                    var repository = new ConfiguracoesGeraisRepository(session);
                    var repositoryVenda = new VendaRepository(session);
                    var repositoryEtapa = new EtapaVendaRepository(session);

                    var business = new VendaBusiness(repositoryVenda);
                    var etapaVendaBusiness = new EtapaVendaBusiness(repositoryEtapa, repository);

                    var etapasVenda = repositoryEtapa.GetByVenda(value.id);
                    var venda = business.GetVendaById(value.id);


                    //Criação de etapa de Ordens de Serviço
                    if (etapasVenda.Last().Etapa >= EnumEtapaVenda.PlanoProduto && !etapasVenda.Any(p => p.Etapa == EnumEtapaVenda.OrdemServico) && venda.Status == Status_Venda.Venda_Aprovada)
                    {
                        etapaVendaBusiness.AdcEtapaOS(venda, value.usuario);
                    }
                }

            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Vendaapi/AlterarStatusAuditoria")]
        #region Alterar Status Auditoria
        public async Task<PostApiRetorno<Int64>> AlterarStatusAuditoria(VendaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<LogVendas> repositoryLogVenda = new Conexao<LogVendas>(sessionLog))
            using (Conexao<LogAlteracaoPlanoContrato> repositoryLogAlteracaoPlanoContrato = new Conexao<LogAlteracaoPlanoContrato>(sessionLog))
            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))


                try
                {
                    Venda modeloAnterior = null;
                    Venda venda = null;

                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();

                        venda = await repositoryVenda.selectFirst(p => p.Id == Convert.ToInt64(value.id));
                        modeloAnterior = (Venda)venda.Clone();

                        venda.StatusAuditoria = (StatusAuditoria)value.status;
                        await repositoryVenda.SaveOrUpdate(venda);

                        retorno.Ok = true;
                    }
                    #region LOG
                    if (modeloAnterior != null)
                    {
                        var logs = venda.GeraLog(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                        await repositoryLogVenda.Save(logs);
                        /*MUDANÇA DE PLANO*/
                        if (venda.Mudanca_Plano)
                        {
                            var logs_alteracao_plano = venda.GeraLogAlteracao(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogAlteracaoPlanoContrato.Save(logs_alteracao_plano);
                        }
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                }

            return retorno;
        }
        #endregion

        [HttpPost()]
        [Route("api/Vendaapi/AlterarStatusAutorizacao")]
        #region AlterarStatusAutorizacao
        public async Task<PostApiRetorno<Int64>> AlterarStatusAutorizacao(VendaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder($"UPDATE TB_VENDA SET DATA_ALTERACAO_STATUS = '{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss")}', STATUS_LIBERACAO = {value.status} WHERE ID = {value.id}");

                        if (session.Transaction != null)
                        {
                            session.Transaction.Enlist(com);
                        }

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

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
        #endregion

        public async Task<PostApiRetorno<Int64>> AlterarStatusPrivate(VendaAlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            GetApiRetorno<List<AtualizaContrato>> retornoIntegracao = null;
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ISession sessionRadius = HibernateUtil.GetSessionRadius(false))
            using (ITransaction transRadius = sessionRadius != null ? sessionRadius.BeginTransaction() : null)
            using (var trans = session.BeginTransaction())
            using (var transT5 = sessionT5.BeginTransaction())
            using (var transLog = sessionLog.BeginTransaction())
            using (Conexao<LogVendas> repositoryLogVenda = new Conexao<LogVendas>(sessionLog))
            using (Conexao<LogAlteracaoPlanoContrato> repositoryLogAlteracaoPlanoContrato = new Conexao<LogAlteracaoPlanoContrato>(sessionLog))
            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))
            using (Conexao<Cidade> repositoryCidade = new Conexao<Cidade>(session))
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(session))
            using (Conexao<Log_ClientePlanoProduto> repoLogClientePlanoProduto = new Conexao<Log_ClientePlanoProduto>(sessionLog))
            {
                try
                {

                    Venda venda = null;
                    Cliente cliente = null;
                    //voltar nessa área
                    //if (venda.Mudanca_Plano) 
                    //{
                    //    Cliente_PlanoProduto clientePlanoProdutoOld = null; // encontrar plano via id_trocaPlano

                    //    var clientePlanoEncontrado = 
                    //    //repoCli

                    //    clientePlanoProdutoOld = (Cliente_PlanoProduto)clientePlanoEncontrado.Clone();

                    //    value.id


                    //    var logs = clientePlanoEncontrado.GeraLog(clientePlanoProdutoOld, User.Identity.GetUserId(), Sinapse.ModelLog.TipoOperLog.Alteracao);

                    //    if (logs.Count > 0)
                    //        await repositoryLog.Save(logs);
                    //}
                    //Recupera os dados da venda para o LOG
                    Venda modeloAnterior = null;

                    #region Workflow
                    using (Conexao<Venda_Ultimo_Contato> repositoryVendaUltimoContato = new Conexao<Venda_Ultimo_Contato>(session))
                    using (Conexao<Cliente> repositoryCliente = new Conexao<Cliente>(session))
                    using (Conexao<Venda_Ultimo_Contato> repoVendaUltimoContato = new Conexao<Venda_Ultimo_Contato>(session))
                    using (Conexao<Workflow_VendasAtividade> repoAtividade = new Conexao<Workflow_VendasAtividade>(session))
                    using (Conexao<Atividade_StatusVenda> repoVendaStatus = new Conexao<Atividade_StatusVenda>(session))
                    using (Workflow_vendasApiController workflowApi = new Workflow_vendasApiController())
                    using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                    {
                        if (value.id != 0)
                        {
                            venda = await repositoryVenda.selectFirst(p => p.Id == Convert.ToInt64(value.id));
                            modeloAnterior = (Venda)venda.Clone();
                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                            IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                            IClienteRepository repoCliente = new ClienteRepository(session);
                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoCliente, venda.Id_Cliente.Value);

                            

                            if (venda.Workflow_VendasAtividade != null && value.situacao.HasValue)
                            {
                                Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                                var primeiraAtividadeWorkflow =
                                    venda.Workflow_Vendas.Atividades.Where(p => p.Ativ_Status == Status_Default.Ativo).FirstOrDefault();

                                if (primeiraAtividadeWorkflow != null && venda.Workflow_VendasAtividade.Id == primeiraAtividadeWorkflow.Id)
                                {
                                    if (venda.Termo_Adesao_Assinado || config.Permite_Work_Sem_Termo || (!config.Permite_Work_Sem_Termo && value.situacao != 0))
                                    {
                                        if (value.situacao == 0)
                                        {
                                            var workAtividades = await repoAtividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivAprovada);
                                            if (value.idWorkflowAtividade > 0 && workAtividades == null)
                                            {
                                                workAtividades = await repoAtividade.selectFirst(p => p.Id == value.idWorkflowAtividade);
                                            }
                                            venda.Workflow_VendasAtividade = workAtividades;
                                            venda.Status = (Status_Venda)workAtividades?.Ativ_StatusVenda?.StatusPadrao;
                                            venda.StatusVenda = workAtividades?.Ativ_StatusVenda?.Descricao;
                                            value.status = (short)venda.Status;
                                        }
                                        else
                                        {
                                            var workAtividades = await repoAtividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivReprovada);
                                            if (value.idWorkflowAtividade > 0 && workAtividades == null)
                                            {
                                                workAtividades = await repoAtividade.selectFirst(p => p.Id == value.idWorkflowAtividade);
                                            }
                                            venda.Workflow_VendasAtividade = workAtividades;
                                            venda.Status = (Status_Venda)workAtividades?.Ativ_StatusVenda?.StatusPadrao;
                                            venda.StatusVenda = workAtividades?.Ativ_StatusVenda?.Descricao;
                                            value.status = (short)venda.Status;
                                        }


                                        if (venda.Workflow_VendasAtividade != null)
                                        {
                                            if (venda.Workflow_VendasAtividade.Responsavel != null)
                                                venda.Id_Responsavel = venda.Workflow_VendasAtividade.Responsavel;
                                            if (venda.Workflow_VendasAtividade.SetorResponsavel != null)
                                                venda.Id_Setor_Departamento = venda.Workflow_VendasAtividade.SetorResponsavel;
                                        }

                                        if (venda.Workflow_VendasAtividade != null)
                                        {
                                            bool atividadePossuiSetor = venda.Workflow_VendasAtividade.SetorResponsavel != null;
                                            bool atividadePossuiResponsavel = venda.Workflow_VendasAtividade.Responsavel != null;

                                            if (atividadePossuiSetor)
                                            {
                                                // Atualiza o setor conforme configuração da atividade
                                                venda.Id_Setor_Departamento = venda.Workflow_VendasAtividade.SetorResponsavel;

                                                if (atividadePossuiResponsavel)
                                                {
                                                    // Atividade define Setor E Responsável → atualiza ambos
                                                    venda.Id_Responsavel = venda.Workflow_VendasAtividade.Responsavel;
                                                }
                                                else
                                                {
                                                    // Atividade define apenas Setor → verifica se responsável atual é compatível
                                                    bool responsavelAtualPertenceAoNovoSetor = venda.Id_Responsavel != null
                                                    && venda.Id_Responsavel?.Departamento?.Id == venda.Workflow_VendasAtividade?.SetorResponsavel?.Id;

                                                    if (!responsavelAtualPertenceAoNovoSetor)
                                                    {
                                                        // Responsável atual não pertence ao novo setor → limpa
                                                        venda.Id_Responsavel = null;
                                                    }
                                                    // else: mantém o responsável atual (já pertence ao setor)
                                                }
                                            }
                                            // else: atividade sem configuração → mantém Setor e Responsável atuais (não faz nada)
                                        }


                                        bool etapaRestringeVenda = etapaVendaBusiness.EtapaRestringeVenda(venda, config.Exige_Termo_Adesao);
                                        if (venda.Status == Status_Venda.Venda_Aprovada && config.Exige_Termo_Adesao && !venda.Termo_Adesao_Assinado)
                                        {
                                            retorno.Ok = false;
                                            retorno.Mensagem = "É obrigatório que o termo de adesão seja assinado para prosseguir com a venda";
                                        }
                                        else if (venda.Status == Status_Venda.Venda_Aprovada && etapaRestringeVenda)
                                        {
                                            retorno.Ok = false;
                                            retorno.Mensagem = "Uma ou mais consultas de restrição foram reprovadas";
                                            return retorno;
                                        }
                                        else
                                        {
                                            retorno.Ok = true;
                                            await repositoryVenda.SaveOrUpdate(venda);
                                        }
                                    }

                                    if (!venda.Termo_Adesao_Assinado && !config.Permite_Work_Sem_Termo && value.situacao == 0)
                                    {
                                        retorno.Ok = false;
                                        retorno.Mensagem = "Não foi possível aprovar a etapa, termo de adesão não assinado";
                                        return retorno;
                                    }
                                }
                                else
                                {
                                    if (value.situacao == 0)
                                    {
                                        Workflow_VendasAtividade workAtividade = await repoAtividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivAprovada);
                                        if (value.idWorkflowAtividade > 0 && workAtividade == null)
                                        {
                                            workAtividade = await repoAtividade.selectFirst(p => p.Id == value.idWorkflowAtividade);
                                        }
                                        venda.Workflow_VendasAtividade = workAtividade;

                                        Status_Venda? proxStatusVenda = workAtividade?.Ativ_StatusVenda?.StatusPadrao;

                                        if (proxStatusVenda.HasValue)
                                            venda.Status = proxStatusVenda.Value;

                                        venda.StatusVenda = workAtividade?.Ativ_StatusVenda?.Descricao;
                                        value.status = (short)venda.Status;
                                        venda.UsuarioAlteracao = value.usuario; ;
                                    }
                                    else
                                    {
                                        Workflow_VendasAtividade workAtividade = await repoAtividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivReprovada);
                                        if (value.idWorkflowAtividade > 0 && workAtividade == null)
                                        {
                                            workAtividade = await repoAtividade.selectFirst(p => p.Id == value.idWorkflowAtividade);
                                        }
                                        venda.Workflow_VendasAtividade = workAtividade;

                                        Status_Venda? proxStatusVenda = workAtividade?.Ativ_StatusVenda?.StatusPadrao;

                                        if (proxStatusVenda.HasValue)
                                            venda.Status = proxStatusVenda.Value;

                                        venda.StatusVenda = workAtividade?.Ativ_StatusVenda?.Descricao;
                                        value.status = (short)venda.Status;
                                    }

                                    bool etapaRestringeVenda = etapaVendaBusiness.EtapaRestringeVenda(venda, config.Exige_Termo_Adesao);

                                    if (venda.Status == Status_Venda.Venda_Aprovada && config.Exige_Termo_Adesao && !venda.Termo_Adesao_Assinado)
                                    {
                                        retorno.Ok = false;
                                        retorno.Mensagem = "É obrigatório que o termo de adesão seja assinado para prosseguir com a venda";
                                        return retorno;
                                    }
                                    else if (venda.Status == Status_Venda.Venda_Aprovada && etapaRestringeVenda)
                                    {
                                        retorno.Ok = false;
                                        retorno.Mensagem = "Uma ou mais consultas de restrição foram reprovadas";
                                        return retorno;
                                    }
                                    else
                                    {
                                        retorno.Ok = true;
                                        await repositoryVenda.SaveOrUpdate(venda);
                                    }
                                }
                            }
                        }


                        #endregion

                        #region Contato

                        using (Conexao<Venda_Ultimo_Contato> repository = new Conexao<Venda_Ultimo_Contato>(session))
                        {
                            if (value.id != 0)
                            {
                                Venda_Ultimo_Contato venda_Ultimo_Contato = new Venda_Ultimo_Contato();
                                if (venda_Ultimo_Contato != null)
                                {
                                    venda_Ultimo_Contato = new Venda_Ultimo_Contato()
                                    {
                                        Id_Venda = value.id,
                                        Data_Ultimo_Contato = DateTime.Now,
                                        UsuarioCriacao = value.usuario,
                                        Forma_Contato = "",
                                        Observacao = "Status Alterado Para " + HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(value.status)),
                                        Acao = "Alteração de Status",
                                        Status_Venda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(value.status))
                                    };

                                    await repository.Save(venda_Ultimo_Contato);
                                }
                            }
                        }

                        #endregion

                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        using (DbCommand com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder($"UPDATE TB_VENDA SET DATA_ALTERACAO_STATUS = '{DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss")}', STATUS = {value.status} WHERE ID = {value.id}");

                            if (session.Transaction != null)
                            {
                                session.Transaction.Enlist(com);
                            }

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);

                            retorno.Ok = true;
                        }

                        #region Avaliação Reprovada

                        if (value.status == 3)
                        {
                            using (DbCommand com = session.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder($"UPDATE TB_CLIENTE A SET A.STATUS_PROSPECTO = 2 ")
                                .AppendLine($"WHERE EXISTS (SELECT B.ID FROM TB_VENDA B WHERE B.ID_CLIENTE = A.ID AND B.ID = {value.id})");

                                if (session.Transaction != null)
                                {
                                    session.Transaction.Enlist(com);
                                }

                                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);
                            }
                        }

                        #endregion

                        #region Venda Cancelada

                        if (value.status == 4)
                        {
                            using (Conexao<Cliente_PlanoProduto> repository = new Conexao<Cliente_PlanoProduto>(session))
                            using (Conexao<Cliente_PlanoProduto_StatusLog> repoStLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                            {
                                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                                List<Cliente_PlanoProduto> modelos = (await repository.select(p => p.Id_Venda == value.id)).ToList();

                                foreach (var item in modelos)
                                {
                                    if (item.CodContratoIntegracao != null && !config.Permite_Exclusao_Cancelamento_Plano_Integrado)
                                    {
                                        throw new Exception("Não será possível cancelar a venda, pois o plano/produto já foi integrado.");
                                    }
                                }
                                

                                modelos = modelos.Select(p =>
                                {
                                    p.ClienteContrato.Status_Contrato = Status_Contrato.Cancelado;
                                    p.Status = Status_PlanoProduto.Venda_Cancelada;
                                    p.Id_TrocaPlanoCancelados = p.Id_TrocaPlano > 0 ? p.Id_TrocaPlano : (long?)null;
                                    p.Id_TrocaPlano = (long?)null;
                                    return p;
                                }).ToList();

                                await repository.SaveOrUpdate(modelos);
                                await RegistraClienteStatusLog(modelos, repoStLog);
                            }

                            using (Conexao<Venda> repository = new Conexao<Venda>(session))
                            using (Conexao<Motivo_Desistencia_Venda> repositoryMotivoDesistencia = new Conexao<Motivo_Desistencia_Venda>(session))
                            {

                                venda.Status = Status_Venda.Venda_Cancelada;
                                if (!String.IsNullOrEmpty(value.justificativa))
                                    venda.Justificativa_Avaliacao_Comercial = value.justificativa;

                                if (!venda.Data_Venda.HasValue)
                                    venda.Data_Venda = DateTime.Now;

                                if (value.motivo_desistencia_venda.HasValue)
                                    venda.Motivo_Desistencia_Venda = await repositoryMotivoDesistencia.selectFirst(p => p.Id == value.motivo_desistencia_venda);

                                cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == venda.Id_Cliente);

                                await new NotificaMobileApiController(session).Notifica(
                                        venda.Vendedor.Id, "AVALIACAO_COMERCIAL",
                                    "Venda Reprovada",
                                    $"Venda Reprovada\nCliente: {cliente.NomeFantasia}\nCPF:{cliente.CpfCnpj}",
                                    null
                                );

                                await repository.SaveOrUpdate(venda);
                            }


                        }

                        #endregion

                        #region Venda Aprovada

                        if (value.status == 6)
                        {
                            using (DbCommand com = session.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder($" UPDATE TB_CLIENTE A SET A.DATA_CRIACAO_CLIENTE = COALESCE(A.DATA_CRIACAO_CLIENTE, CURRENT_TIMESTAMP), ")
                                                          .AppendLine("  A.TIPO_CLIENTE = 0, A.STATUS = 1, A.STATUS_PROSPECTO = 4 ")
                                                          .AppendLine($" WHERE EXISTS (SELECT B.ID FROM TB_VENDA B WHERE B.ID_CLIENTE = A.ID AND B.ID = {value.id}) ");

                                if (session.Transaction != null)
                                {
                                    session.Transaction.Enlist(com);
                                }

                                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);
                            }


                            using (DbCommand com = session.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder($"UPDATE TB_CLIENTE_PLANO_PRODUTO A SET A.ATUALIZA_OLA = 1 ")
                                .AppendLine($"WHERE EXISTS (SELECT B.ID FROM TB_VENDA B WHERE A.ID_VENDA = B.ID AND B.ID = {value.id})");

                                if (session.Transaction != null)
                                {
                                    session.Transaction.Enlist(com);
                                }
                                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, com.Transaction);
                            }

                            //using (Conexao<Venda> repository = new Conexao<Venda>(session))
                            using (Conexao<Cliente_PlanoProduto> repositoryClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                            using (Conexao<Elemento_Rede_Porta> repositoryElemPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                            using (Conexao<Model.nhibernate.LoginRadius> repositoryLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
                            using (Conexao<Motivo_Desistencia_Venda> repositoryMotivoDesistencia = new Conexao<Motivo_Desistencia_Venda>(session))
                            using (Conexao<ONU> repositoryONU = new Conexao<ONU>(session))
                            {
                                venda.Status = Status_Venda.Venda_Aprovada;
                                List<Cliente_PlanoProduto> planosProdutos = repositoryClientePlanoProduto.select(p => p.Id_Venda == venda.Id).Result.ToList();
                                if (planosProdutos.Any(p => p.Forma_Pagamento_Cliente == null))
                                    throw new Exception("Plano/Produto sem forma de pagamento definida. Por favor, selecionar uma forma de pagamento na aba Planos e Produtos.");

                                if (value.motivo_desistencia_venda.HasValue)
                                    venda.Motivo_Desistencia_Venda = await repositoryMotivoDesistencia.selectFirst(p => p.Id == value.motivo_desistencia_venda);

                                using (FuncionarioApiController apiFuncionario = new FuncionarioApiController())
                                {
                                    VendedorMetaFaixaComissionamentoModel metaFaixaComissionamento = await apiFuncionario.GetMetaFaixaComissionamento(venda.Vendedor.Id);

                                    venda.Id_Meta = metaFaixaComissionamento.Id_Meta;
                                    venda.Id_Faixa = metaFaixaComissionamento.Id_Faixa;
                                    venda.Id_Comissionamento = metaFaixaComissionamento.Id_Comissionamento;

                                    if (!String.IsNullOrEmpty(value.justificativa))
                                        venda.Justificativa_Avaliacao_Comercial = value.justificativa;

                                    if (!venda.Data_Venda.HasValue)
                                        venda.Data_Venda = DateTime.Now;
                                }

                                await repositoryVenda.SaveOrUpdate(venda);


                                using (Conexao<Cliente_PlanoProduto_StatusLog> repository_status_log = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                                using (Conexao<Cliente> repository_cliente = new Conexao<Cliente>(session))
                                using (Conexao<Cliente_Desconto> repository_cliente_desconto = new Conexao<Cliente_Desconto>(session))
                                using (Conexao<ClienteContrato> repository_cc = new Conexao<ClienteContrato>(session))
                                using (Conexao<Cliente_PlanoProduto> repository_cliente_plano_produto = new Conexao<Cliente_PlanoProduto>(session))
                                using (NotificaMobileApiController notifica_mobile_api = new NotificaMobileApiController(session))
                                using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
                                using (var repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
                                using (var repoClienteAniel = new Conexao<ClienteAniel>(sessionT5))
                                {
                                    cliente = repository_cliente.selectFirst(p => p.Id == venda.Id_Cliente.Value).Result;

                                    List<Cliente_PlanoProduto> listaPPsDescontinuados = new List<Cliente_PlanoProduto>();
                                    List<ONU> listaONUs = new List<ONU>();
                                    List<Elemento_Rede_Porta> listaElemRedePorta_PlanoNovo = new List<Elemento_Rede_Porta>();
                                    List<Elemento_Rede_Porta> listaElemRedePorta_PlanoDescontinuado = new List<Elemento_Rede_Porta>();
                                    IList<Cliente_Desconto> lista_cliente_desconto = new List<Cliente_Desconto>();

                                    foreach (Cliente_PlanoProduto planoProduto in planosProdutos)
                                    {
                                        var objClienteAniel = await repoClienteAniel.selectFirst(x => x.CPF == cliente.CpfCnpj);
                                        IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, sessionT5);
                                        var elementoRedePortaBusiness = new ElementoRedePortaAnielBusiness(repository);
                                        Elemento_ReservaCadastroModel elemento_ = new Elemento_ReservaCadastroModel();
                                        if (planoProduto?.Id_TrocaPlano > 0)
                                        {
                                            var objPlanoDescontinuado = await repository_cliente_plano_produto.selectFirst(x => x.Id == planoProduto.Id_TrocaPlano);
                                            var objONU = await repositoryONU.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id_TrocaPlano);
                                            if (planoProduto.Plano != null)
                                            {
                                                if (planoProduto.Plano.OS_Mudanca_Plano)
                                                {
                                                    planoProduto.Status = Status_PlanoProduto.Aguardando_Criacao_OS;
                                                    if (planoProduto.Plano?.MaterialOSPlano.Count > 0)
                                                    {
                                                        using (var repoMaterial = new Conexao<Material_TarefaAniel>(sessionT5))
                                                        {
                                                            var listaRetornoMateriais = await repoMaterial.select(x => x.Id_Cliente == objClienteAniel.Cod_Cliente);
                                                            var listaSoma = new List<double?>();
                                                            var listaMateriais = new List<string>();
                                                            listaRetornoMateriais.ForEach(x =>
                                                            {
                                                                var soma = x.Quantidade - x.Quant_Remov;
                                                                listaSoma.Add(soma);
                                                                listaMateriais.Add(x.CodMat_Inst);
                                                            });

                                                            var resultado = listaSoma.Sum();
                                                            var listaString_Codigo = planoProduto.Plano?.MaterialOSPlano.Select(x => x.Codigo).ToList();
                                                            var b0 = listaString_Codigo.Where(x => listaMateriais.Contains(x)).ToList();
                                                            if (b0.Count > 0 && resultado > 0)
                                                            {
                                                                planoProduto.Status = Status_PlanoProduto.Habilitado;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    planoProduto.Status = Status_PlanoProduto.Habilitado;
                                                }
                                            }

                                            if (planoProduto.Status == Status_PlanoProduto.Habilitado)
                                            {
                                                var objLoginRadius_PlanoNovo = await repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id);
                                                if (objLoginRadius_PlanoNovo != null)
                                                {
                                                    objLoginRadius_PlanoNovo.Cliente_PlanoProduto = null;

                                                    if (objLoginRadius_PlanoNovo.Pool_Ip_Endereco != null)
                                                    {
                                                        objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Login = null;
                                                        objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                                        await repoPoolIpEndereco.Update(objLoginRadius_PlanoNovo.Pool_Ip_Endereco);
                                                    }
                                                    await repositoryLogin.Remove(objLoginRadius_PlanoNovo);
                                                }

                                                var objLoginRadius = await repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id_TrocaPlano);

                                                objPlanoDescontinuado.Status = Status_PlanoProduto.Cancelado_Mudanca_Plano;
                                                objPlanoDescontinuado.Motivo_Mudanca_PP = Motivo_MudancaPlano_Produto.Motivo_Mudanca_PP;

                                                #region [Objeto - ClienteContrato]
                                                planoProduto.ClienteContrato = objPlanoDescontinuado.ClienteContrato;
                                                #endregion

                                                if (objLoginRadius != null)
                                                {
                                                    objLoginRadius.Cliente_PlanoProduto = planoProduto;
                                                    /*
                                                     * Limpeza da porta do novo plano, pois está reservada automaticamente
                                                     * *E é necessário, que a porta, esteja na mesma do antigo. 
                                                     * **/
                                                    elemento_.Login = objLoginRadius.Login;
                                                    elemento_.Cliente_PlanoProduto_Novo = planoProduto.Id;
                                                    await repository.LimparPorta(elemento_);

                                                    elemento_.Cliente_PlanoProduto_Antigo = planoProduto.Id_TrocaPlano.Value;
                                                    await repository.AlterarPortaPlano(elemento_, false);

                                                    await repositoryLogin.SaveOrUpdate(objLoginRadius);

                                                    if (planoProduto?.Plano?.AutenticacaoRadius == true && objLoginRadius?.Concentradora?.Radius != null)
                                                    {
                                                        var anielRadiusRepo = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(objLoginRadius?.Concentradora?.Radius, sessionRadius);
                                                        await new IntegracaoRadiusBusiness(anielRadiusRepo).EditarUsuario(objLoginRadius, CONSTANTES.UsaRadiusSemContainer);
                                                        if (objLoginRadius.Status == Status_Default.Inativo)
                                                        {
                                                            var poolsBloqueio = await repoPoolIp.select(p => p.IsPoolBloqueio == true && p.Concentradora.Id == objLoginRadius.Concentradora.Id);
                                                            await new IntegracaoRadiusBusiness(anielRadiusRepo).BloquearUsuario(objLoginRadius, CONSTANTES.UsaRadiusSemContainer, poolsBloqueio);
                                                        }
                                                        else
                                                        {
                                                            await new IntegracaoRadiusBusiness(anielRadiusRepo).DesbloquearUsuario(objLoginRadius, CONSTANTES.UsaRadiusSemContainer);
                                                        }
                                                    }

                                                }

                                                if (objONU != null)
                                                {
                                                    objONU.Cliente_PlanoProduto = planoProduto;
                                                    listaONUs.Add(objONU);
                                                }

                                                listaPPsDescontinuados.Add(objPlanoDescontinuado);

                                                var lsClienteDesconto = await repository_cliente_desconto.select(x => x.Numero_ContratoVinculado == Convert.ToString(planoProduto.ClienteContrato.Id) &&
                                                    x.Cliente.Id == planoProduto.Cliente.Id);

                                                foreach (var item in lsClienteDesconto)
                                                {
                                                    item.Numero_ContratoVinculado = Convert.ToString(objPlanoDescontinuado.ClienteContrato.Id);
                                                    item.UsuarioAlteracao = User.Identity.GetUserId();
                                                    item.DataHoraAlteracao = DateTime.Now;
                                                    lista_cliente_desconto.Add(item);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (planoProduto.Plano != null)
                                            {
                                                planoProduto.Status = planoProduto.Plano.StatusInicial;
                                            }
                                            else if (planoProduto.Produto != null)
                                            {
                                                planoProduto.Status = planoProduto.Produto.Status_Inicial;
                                            }
                                            else if (planoProduto.Combo != null)
                                            {
                                                if (planoProduto.Combo.PlanosProdutos.Where(p => p.Plano?.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS ||
                                                                                                p.Produto?.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                                                                                    .Count() > 0)
                                                {
                                                    planoProduto.Status = Status_PlanoProduto.Aguardando_Criacao_OS;
                                                }
                                                else
                                                {
                                                    planoProduto.Status = Status_PlanoProduto.Aguardando_Instalacao;
                                                }
                                            }
                                        }

                                        if (planoProduto.ClienteContrato != null)
                                        {
                                            planoProduto.ClienteContrato.Status_Contrato = Status_Contrato.Ativo;
                                        }
                                    }

                                    #region [CRUD]
                                    if (listaONUs.Count > 0)
                                    {
                                        await repositoryONU.SaveOrUpdate(listaONUs);
                                    }

                                    if (listaPPsDescontinuados.Count > 0)
                                    {
                                        await repository_cliente_plano_produto.SaveOrUpdate(listaPPsDescontinuados);
                                        await RegistraClienteStatusLog(listaPPsDescontinuados, repository_status_log);
                                    }

                                    if (lista_cliente_desconto.Count > 0)
                                    {
                                        await repository_cliente_desconto.SaveOrUpdate(lista_cliente_desconto);
                                    }

                                    await repository_cliente_plano_produto.SaveOrUpdate(planosProdutos);
                                    await RegistraClienteStatusLog(planosProdutos, repository_status_log);
                                    #endregion

                                    await notifica_mobile_api.Notifica(
                                        venda.Vendedor.Id,
                                        "AVALIACAO_COMERCIAL",
                                        "Venda Aprovada",
                                        $"Venda Aprovada\nCliente:{cliente.NomeFantasia}\nCPF:{cliente.CpfCnpj}",
                                        null
                                    );

                                    await EnviarEmailContrato(session, cliente, venda.Id, null);
                                }
                            }

                            IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                            var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                            IntegracoesExternas dadosIntegracaoComercial = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL);

                            if (dadosIntegracaoComercial != null)
                            {
                                var integracaoIXCRequest = new IntegrarClienteComercial
                                {
                                    Integracao = dadosIntegracaoComercial.IntegracaoExterna,
                                    Venda = venda.Id,
                                };

                                retornoIntegracao = await this.IntegrarVenda(integracaoIXCRequest, session, sessionT5);

                                if (!retornoIntegracao.Ok)
                                {
                                    trans.Rollback();
                                    transT5.Rollback();
                                    transLog.Rollback();

                                    if (retornoIntegracao != null && retornoIntegracao.Objeto.Count > 0)
                                        await SalvaRetornoIntegracao(integracaoIXCRequest, value, retornoIntegracao);

                                    throw new Exception(retornoIntegracao.Mensagem);
                                }
                                else
                                {
                                    if (trans != null && !trans.WasCommitted)
                                        trans.Commit();

                                    if (transT5 != null && !transT5.WasCommitted)
                                        transT5.Commit();

                                    if (transLog != null && !transLog.WasCommitted)
                                        transLog.Commit();

                                    if (transRadius != null)
                                        transRadius.Commit();

                                    await SalvaRetornoIntegracao(integracaoIXCRequest, value, retornoIntegracao);
                                }
                            }
                        }

                        #region LOG
                        if (modeloAnterior != null)
                        {
                            var logs = venda.GeraLog(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogVenda.Save(logs);
                            /*MUDANÇA DE PLANO*/
                            if (venda.Mudanca_Plano)
                            {
                                var logs_alteracao_plano = venda.GeraLogAlteracao(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogAlteracaoPlanoContrato.Save(logs_alteracao_plano);
                            }
                        }
                        #endregion

                        if (trans != null && !trans.WasCommitted)
                            trans.Commit();

                        if (transT5 != null && !transT5.WasCommitted)
                            transT5.Commit();

                        if (transLog != null && !transLog.WasCommitted)
                            transLog.Commit();

                        if (transRadius != null)
                            transRadius.Commit();


                    }
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
                #endregion

                return retorno;
            }
        }

        private async Task SalvaRetornoIntegracao(IntegrarClienteComercial integracao, VendaAlterarStatusViewModel value, GetApiRetorno<List<AtualizaContrato>> retornoIntegracao)
        {
            if (integracao.Integracao == IntegracaoExterna.MK_SOLUTION)
            {
                await SalvaRetornoIntegracaoMk(value, retornoIntegracao);
            }
            else if (integracao.Integracao == IntegracaoExterna.IXC)
            {
                await SalvaRetornoIntegracaoIxc(value, retornoIntegracao);
            }
        }

        private async Task SalvaRetornoIntegracaoMk(VendaAlterarStatusViewModel value, GetApiRetorno<List<AtualizaContrato>> retornoIntegracao)
        {
            foreach (var item in retornoIntegracao.Objeto)
            {
                using (ISession sessionContrato = HibernateUtil.GetSession(true))
                using (Conexao<Venda> repoVenda = new Conexao<Venda>(sessionContrato))
                using (Conexao<Cliente_PlanoProduto> repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionContrato))
                {
                    var vendaSalvar = await repoVenda.selectFirst(x => x.Id == value.id);
                    var clientePlanoProduto = await repoPlanoProduto.selectFirst(x => x.Id == item.IdClientePlanoProduto);

                    if (!string.IsNullOrEmpty(item.CodIntegracao) && clientePlanoProduto != null)
                    {
                        clientePlanoProduto.CodContratoIntegracao = item.CodIntegracao;
                        await repoPlanoProduto.Update(clientePlanoProduto);
                    }

                    if (!string.IsNullOrEmpty(item.CodConexao))
                    {
                        vendaSalvar.IdLoginIntegracao = item.CodConexao;
                        await repoVenda.Update(vendaSalvar);
                    }

                    if (!string.IsNullOrEmpty(item.CodAtentimento))
                    {
                        vendaSalvar.IdAtendimentoIntegracao = item.CodAtentimento;
                        await repoVenda.Update(vendaSalvar);
                    }

                    if (!string.IsNullOrEmpty(item.CodOS))
                    {
                        clientePlanoProduto.OrdemServicoIntegracao = item.CodOS;
                        await repoPlanoProduto.Update(clientePlanoProduto);
                    }

                    if (!string.IsNullOrEmpty(item.CodigoAgrupamento))
                    {
                        clientePlanoProduto.CodigoAgrupamento = item.CodigoAgrupamento;
                        await repoPlanoProduto.Update(clientePlanoProduto);
                    }
                }
            }
        }

        private async Task SalvaRetornoIntegracaoIxc(VendaAlterarStatusViewModel value, GetApiRetorno<List<AtualizaContrato>> retornoIntegracao)
        {
            foreach (var item in retornoIntegracao.Objeto)
            {
                using (ISession sessionContrato = HibernateUtil.GetSession(true))
                using (Conexao<Venda> repoVenda = new Conexao<Venda>(sessionContrato))
                using (Conexao<ClienteContrato> repoContrato = new Conexao<ClienteContrato>(sessionContrato))
                {
                    var vendaSalvar = await repoVenda.selectFirst(x => x.Id == value.id);
                    var contrato = await repoContrato.selectFirst(x => x.Id == item.IdContrato);

                    if (!string.IsNullOrEmpty(item.CodIntegracao) && contrato != null)
                    {
                        contrato.CodIntegracao = item.CodIntegracao;
                        await repoContrato.Update(contrato);
                    }

                    if (!string.IsNullOrEmpty(item.CodConexao))
                    {
                        vendaSalvar.IdLoginIntegracao = item.CodConexao;
                        await repoVenda.Update(vendaSalvar);
                    }

                    if (!string.IsNullOrEmpty(item.CodAtentimento))
                    {
                        vendaSalvar.IdAtendimentoIntegracao = item.CodAtentimento;
                        await repoVenda.Update(vendaSalvar);
                    }

                }
            }
        }

        private PeriodoRecorrencia FrequenciaCompativel(Frequencia_Cobranca freqCob)
        {
            switch ((int)freqCob)
            {
                case 1:
                    return PeriodoRecorrencia.CobrancaUnica;
                case 30:
                    return PeriodoRecorrencia.Mensal;
                case 60:
                    return PeriodoRecorrencia.Bimestral;
                case 90:
                    return PeriodoRecorrencia.Trimestral;
                case 180:
                    return PeriodoRecorrencia.Semestral;
                case 360:
                    return PeriodoRecorrencia.Anual;
                default:
                    return PeriodoRecorrencia.CobrancaUnica;
            }
        }

        [Route("api/Vendaapi/CancelarVendasEmMassa")]
        [HttpPost()]
        public async Task<List<VendasNaoCanceladas>> CancelarVendasEmMassa(VendaAlterarStatusEmMassaViewModel value)
        {
            var vendasNaoCanceladas = new List<VendasNaoCanceladas>();
            List<VendaAlterarStatusViewModel> cancelAprovados = new List<VendaAlterarStatusViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionlog = HibernateUtil.GetSessionLog())
            using (var ordemRepository = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var vendaRepository = new Conexao<Venda>(session))
            using (var vendalogRepository = new Conexao<LogVendas>(sessionlog))
            using (Conexao<Workflow_VendasAtividade> repoAtividade = new Conexao<Workflow_VendasAtividade>(session))
            using (Conexao<Atividade_StatusVenda> repoAtivStatus = new Conexao<Atividade_StatusVenda>(session))

            {
                //para cada venda, verifica os status, termo e se há ordem de serviço vinculado
                foreach (var item in value.listaVenda)
                {
                    var venda = await vendaRepository.selectFirst(v => v.Id == item.id);

                    if (venda == null)
                        continue;

                    var vendaItem = new VendasNaoCanceladas
                    {
                        NumeroVenda = venda.Id,
                        Status = HtmlEnumExtensions.GetEnumDescription(venda.Status),
                        TermoAdesaoAssinado = venda.Termo_Adesao_Assinado == true ? "Sim" : "Não"
                    };

                    if (venda.Status == Status_Venda.Venda_Cancelada || venda.Status == Status_Venda.Venda_Aprovada)
                    {
                        vendaItem.Motivo = HtmlEnumExtensions.GetEnumDescription(venda.Status);
                        vendasNaoCanceladas.Add(vendaItem);
                        continue;
                    }

                    if (venda.Termo_Adesao_Assinado)
                    {
                        vendaItem.Motivo = "Termo de Adesão Assinado";
                        vendasNaoCanceladas.Add(vendaItem);
                        continue;
                    }

                    var hasOrdemServico = await ordemRepository.selectFirst(os => os.Id_Venda == venda.Id);

                    if (hasOrdemServico != null)
                    {
                        vendaItem.Motivo = "OS vinculada";
                        vendaItem.OS = hasOrdemServico.Num_Obra;
                        vendasNaoCanceladas.Add(vendaItem);
                        continue;
                    }

                    cancelAprovados.Add(item);
                }

                //para cada cancelamento aprovado, salvar o historico da venda e cancelar a venda
                foreach (var cancelItem in cancelAprovados)
                {
                    using (Conexao<Venda_Ultimo_Contato> repositoryContato = new Conexao<Venda_Ultimo_Contato>(session))
                    {
                        if (cancelItem.id != 0)
                        {
                            Venda_Ultimo_Contato venda_Ultimo_Contato = new Venda_Ultimo_Contato();
                            if (venda_Ultimo_Contato != null)
                            {
                                venda_Ultimo_Contato = new Venda_Ultimo_Contato()
                                {
                                    Id_Venda = cancelItem.id,
                                    Data_Ultimo_Contato = DateTime.Now,
                                    UsuarioCriacao = cancelItem.usuario,
                                    Forma_Contato = "",
                                    Observacao = "Status Alterado Para " + HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(cancelItem.status)),
                                    Acao = "Alteração de Status",
                                    Status_Venda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(cancelItem.status))
                                };

                                await repositoryContato.Save(venda_Ultimo_Contato);
                            }
                        }
                    }

                    using (Conexao<Cliente_PlanoProduto> repository = new Conexao<Cliente_PlanoProduto>(session))
                    using (Conexao<Cliente_PlanoProduto_StatusLog> repoStLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                    {
                        List<Cliente_PlanoProduto> modelos = (await repository.select(p => p.Id_Venda == cancelItem.id)).ToList();

                        modelos = modelos.Select(p =>
                        {
                            p.Status = Status_PlanoProduto.Venda_Cancelada;
                            return p;
                        }).ToList();

                        await repository.SaveOrUpdate(modelos);
                        await RegistraClienteStatusLog(modelos, repoStLog);
                    }

                    using (Conexao<Workflow_Vendas> repository = new Conexao<Workflow_Vendas>(session))
                    using (Conexao<Motivo_Desistencia_Venda> repositoryMotivoDesistencia = new Conexao<Motivo_Desistencia_Venda>(session))
                    {
                        Venda modeloVenda = vendaRepository.selectFirst(p => p.Id == cancelItem.id).Result;
                        Venda vendaOld = (Venda)modeloVenda.Clone();

                        var workAtividadesStatus = await repoAtivStatus
                                                            .selectFirst(p => p.StatusPadrao != null &&
                                                                         p.StatusPadrao == Status_Venda.Venda_Cancelada);
                        var workAtividades = await repoAtividade
                                                            .selectFirst(p => p.Ativ_StatusVenda != null && p.WorkFlow == modeloVenda.Workflow_Vendas &&
                                                                         p.Ativ_StatusVenda == workAtividadesStatus);


                        modeloVenda.Status = Status_Venda.Venda_Cancelada;
                        if (!String.IsNullOrEmpty(cancelItem.justificativa))
                            modeloVenda.Justificativa_Avaliacao_Comercial = cancelItem.justificativa;

                        if (!modeloVenda.Data_Venda.HasValue)
                            modeloVenda.Data_Venda = DateTime.Now;

                        if (cancelItem.motivo_desistencia_venda.HasValue)
                            modeloVenda.Motivo_Desistencia_Venda = await repositoryMotivoDesistencia.selectFirst(p => p.Id == cancelItem.motivo_desistencia_venda);

                        var cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == modeloVenda.Id_Cliente);

                        await new NotificaMobileApiController(session).Notifica(
                            modeloVenda.Vendedor.Id, "AVALIACAO_COMERCIAL",
                            "Venda Reprovada",
                            $"Venda Reprovada\nCliente: {cliente.NomeFantasia}\nCPF:{cliente.CpfCnpj}",
                            null
                        );

                        modeloVenda.StatusVenda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Status_Venda.Venda_Cancelada);

                        if (modeloVenda.Workflow_Vendas != null && workAtividades != null)
                        {
                            modeloVenda.Workflow_VendasAtividade = workAtividades;
                        }

                        await vendaRepository.SaveOrUpdate(modeloVenda);
                        var logs = modeloVenda.GeraLog(vendaOld, User.Identity.GetUserId(), User.Identity.GetUserName(), TipoOperLog.Alteracao);
                        await vendalogRepository.SaveOrUpdate(logs);
                    }
                }
            }

            return vendasNaoCanceladas;
        }

        [HttpPost]
        [Route("api/vendaapi/ReprovacaoAutomaticaAtividades")]
        public async Task<GetApiRetorno> ReprovacaoAutomaticaAtividades(ReprovacaoAutomaticaAtividadeModel request)
        {
            var retorno = new GetApiRetorno();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    IVendaRepository repository = new VendaRepository(session);
                    var business = new VendaBusiness(repository);
                    await business.ReprovacaoAutomaticaWorkflow(request.Venda, request.Atividade, User.Identity.GetUserId(), User.Identity.GetUserName());
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Vendaapi/GetAll_Historico_UltimosContatos")]
        public async Task<DataTableResponse<VendaListViewModel>> GetAll_Historico_UltimosContatos(Venda_DataTableRequest request)
        {
            List<VendaListViewModel> retorno = new List<VendaListViewModel>();
            IOrderedEnumerable<VendaListViewModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            String Status = $"({request.Status})";

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_VENDA A  ")
                    .AppendLine(" LEFT OUTER JOIN TB_CLIENTE B ")
                    .AppendLine(" ON(A.ID_CLIENTE = B.ID) ");
                //.AppendLine(" WHERE A.STATUS <> 4 ");

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))//ok
                {
                    sqlFrom.AppendLine(" AND ((B.CPF LIKE @SEARCH) OR (UPPER(B.NOMEFANTASIA) LIKE @SEARCH)) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
                }

                //Count
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (DbCommand com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, D.ENDERECO_BAIRRO ");

                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new VendaListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Tipo_Cliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                DataVenda = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy") : null,

                                Status = HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_VENDA"]),

                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 0:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }


                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<VendaListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/Vendaapi/AjaxAtualizaPlanos")]
        public async Task<GetApiRetorno<List<PlanoProdutoListModel>>> AjaxAtualizaPlanos(Int64 Plano_Id)
        {
            GetApiRetorno<List<PlanoProdutoListModel>> retorno = new GetApiRetorno<List<PlanoProdutoListModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Cliente_PlanoProduto planoProduto = await new Conexao<Cliente_PlanoProduto>(session).selectFirst(p => p.Id == Plano_Id);
                    Venda venda = await new Conexao<Venda>(session).selectFirst(p => p.Id == planoProduto.Id_Venda);

                    PlanoProdutoListModel obj = new PlanoProdutoListModel
                    {
                        Id_Venda = planoProduto?.Id_Venda,
                        Id_Vendedor = venda?.Vendedor?.Id,
                        Id_Supervisor = venda?.Supervisor?.Id,

                        Status_Venda = venda?.Status,

                        Desc_Status_Venda = venda != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status) : null,
                        Nome_Vendedor = venda?.Vendedor?.Nome,
                        Nome_Supervisor = venda?.Supervisor?.Nome,

                        Id = planoProduto == null ? 0 : planoProduto.Id,
                        Id_Plano_Produto = planoProduto.Plano != null ? planoProduto.Plano.Id : planoProduto.Produto.Id,
                        Descricao = planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao,
                        Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null,
                        ValorPlanoProduto = Convert.ToString(planoProduto.Valor_Real),
                        ValorInstalacao_Final = Convert.ToString(planoProduto.Valor_Instalacao_Final),
                        ValorPlanoProduto_Final = Convert.ToString(planoProduto.Valor_Final),

                        ValorInstalacao = Convert.ToString(planoProduto.Valor_Instalacao_Real),
                        ValorTotal = Convert.ToString(planoProduto.Valor_Final_Total),
                        Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis,

                        Meses_Carencia = planoProduto.Meses_Carencia,
                        Tipo = planoProduto.Plano == null ? PlanoProdutoEnum.Produto : PlanoProdutoEnum.Plano,
                        Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(planoProduto.Status.Value) : String.Empty,

                        Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao,
                        End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal,

                        Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0,
                        Periodo_Carencia = planoProduto.Periodo_Carencia,

                        Login_Radius = planoProduto.Login_Radius,
                        Senha_Radius = planoProduto.Senha_Radius,

                        Id_Endereco = planoProduto.Endereco_Instalacao.Id == 0 ? (Int64)0 : planoProduto.Endereco_Instalacao.Id,
                        Id_Bairro = planoProduto.Endereco_Instalacao?.Id_Bairro,
                        Bairro = planoProduto.Endereco_Instalacao?.Bairro,

                        CEP = planoProduto.Endereco_Instalacao?.Cep,
                        Id_Cidade = planoProduto.Endereco_Instalacao?.Id_Cidade,
                        Cidade = planoProduto.Endereco_Instalacao?.Cidade,

                        Ponto_Referencia = planoProduto.Endereco_Instalacao?.Ponto_Referencia,
                        Complemento = planoProduto.Endereco_Instalacao?.Complemento,
                        Logradouro = planoProduto.Endereco_Instalacao?.Logradouro,

                        Numero = planoProduto.Endereco_Instalacao?.Numero,
                        UF = planoProduto.Endereco_Instalacao?.UF,

                        Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude),
                        Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude),
                        Id_Pais = planoProduto.Endereco_Instalacao?.Pais?.Id,

                        Id_Tipo_Imovel = planoProduto.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,

                        Cod_Escritorio = planoProduto.Cod_Escritorio,
                        Cod_Area = planoProduto.Cod_Area,

                        Cod_Localidade = planoProduto.Cod_Localidade,
                        Localidade = planoProduto.Localidade
                    };

                    retorno.Objeto = new List<PlanoProdutoListModel> { obj };
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Vendaapi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA ")
                           .AppendLine("FROM TB_CLIENTE A")
                           .AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR)");

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"])

                                });
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
        [Route("api/PlanoProdutoApi/GetListVenda")]
        public async Task<GetApiRetorno<List<PlanoProdutoListModel>>> GetListVenda()
        {
            GetApiRetorno<List<PlanoProdutoListModel>> retorno = new GetApiRetorno<List<PlanoProdutoListModel>>
            {
                Objeto = new List<PlanoProdutoListModel>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    IList<Plano> listaPlanos = await new Conexao<Plano>(session).queryover().ListAsync();

                    retorno.Objeto.AddRange(listaPlanos.Select(p => new PlanoProdutoListModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Codigo = p.Codigo,
                        ValorPlanoProduto = Convert.ToString(p.ValorPlano),

                        ValorInstalacao = Convert.ToString(p.ValorInstalacao),
                        Tipo = PlanoProdutoEnum.Plano
                    }));



                    IList<Produto> listaProdutos = await new Conexao<Produto>(session).queryover().ListAsync();

                    retorno.Objeto.AddRange(listaProdutos.Select(p => new PlanoProdutoListModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Codigo = p.Codigo,

                        ValorPlanoProduto = null,
                        ValorInstalacao = null,
                        Tipo = PlanoProdutoEnum.Produto
                    }));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }


        [HttpPost()]
        [Route("api/Vendaapi/buscaAtedimento")]
        public async Task<List<ClienteListaBuscaAtendimento>> buscaAtedimento(String term)
        {
            List<ClienteListaBuscaAtendimento> retorno = new List<ClienteListaBuscaAtendimento>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA, A.CPF_CNPJ, A.CPF, A.TELEFONE_FIXO, A.DATA_NASC, ");
                        sql.AppendLine("B.DESCRICAO AS DESC_CATEGORIA, A.TELEFONE_CELULAR, A.TELEFONE_RECADO, ");
                        sql.AppendLine("A.EMAIL_PRINCIPAL, C.DESCRICAO AS DESC_FORMA_PAGAMENTO, A.ID_ENDERECO_PRINCIPAL ");
                        sql.AppendLine("FROM TB_CLIENTE A ");

                        sql.AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE B ");
                        sql.AppendLine("ON(A.id_categoria = B.ID) ");
                        sql.AppendLine("LEFT JOIN TB_FORMA_PAGAMENTO C ");
                        sql.AppendLine("ON(A.ID_FORMA_PAGAMENTO = C.ID) ");

                        sql.AppendLine("WHERE (UPPER(A.CPF) LIKE @VALOR ) OR (UPPER(A.NOMEFANTASIA) LIKE @VALOR ) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        //sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR )");
                        //parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ClienteListaBuscaAtendimento
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Telefone_Fixo = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    Categoria_Cliente = Convert.ToString(reader["DATA_NASC"]),
                                    Telefone_Celular = Convert.ToString(reader["TELEFONE_CELULAR"]),

                                    //CPF_CNPJ_Cliente = Convert.ToString(reader["CPF_CNPJ"]),

                                    CPF_CNPJ_Cliente = Convert.ToString(reader["CPF"]),
                                    Email_Principal = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    Forma_Pagamento_Utilizada = Convert.ToString(reader["DESC_FORMA_PAGAMENTO"]),
                                    Telefone_Recado = Convert.ToString(reader["TELEFONE_RECADO"]),
                                    PlanosProdutos = new PlanoProdutoApiController().getByCliente(Convert.ToInt64(reader["ID"])).Result,
                                    Id_Endereco_Cliente = Convert.ToString(reader["ID_ENDERECO_PRINCIPAL"])
                                });
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

        //Dashboard de Vendas
        [HttpPost()]
        [Route("api/Vendaapi/Dashboard_Vendas")]
        public async Task<DataTableResponse<Dashboard_VendasListModel>> Dashboard_Vendas(Dashboard_Vendas_DataTableRequest request)
        {

            List<Dashboard_VendasListModel> retorno = new List<Dashboard_VendasListModel>();

            IOrderedEnumerable<Dashboard_VendasListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                DateTime _data_atual = DateTime.Now;
                Meta metaAno = await new Conexao<Meta>(session).selectFirst(p => p.Ano == _data_atual.Year);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    String _mes = _data_atual.ToString("MMMM").ToUpperInvariant();
                    _mes = new string(_mes.Normalize(NormalizationForm.FormD).Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark).ToArray());

                    StringBuilder sql = new StringBuilder()

                    .AppendLine(" SELECT A.ID, A.NUM_FRE, A.NOME, B.NOME AS SUPERVISOR, ")

                    //.AppendLine(" (SELECT SUM(PONTOS) ")

                    //.AppendLine(" FROM SP_COMISSIONAMENTO_VENDEDOR(A.ID, @MES, @ANO, @MES, @ANO) ")
                    //.AppendLine(" WHERE DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA) AS VENDAS_DIA, ")

                    //.AppendLine(" (SELECT SUM(PONTOS) ")

                    //.AppendLine(" FROM SP_COMISSIONAMENTO_VENDEDOR(A.ID, @MES, @ANO, @MES, @ANO) ")
                    //.AppendLine(" WHERE DATA_VENDA BETWEEN @DATA_INI_CONFIG AND CURRENT_TIMESTAMP) AS VENDAS_MES, ")

                    .AppendLine(" ( ")
                    .AppendLine("   SELECT COUNT(I.ID) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND I.STATUS = 6 ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS VENDAS_DIA, ")
                    .AppendLine(" ( ")
                    .AppendLine("   SELECT COUNT(I.ID) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND J.STATUS = 2 AND I.STATUS = 6  ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS VENDAS_HAB_DIA, ")
                    .AppendLine(" ( ")
                    .AppendLine("   SELECT COUNT(I.ID) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND I.STATUS = 6 ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_CONFIG AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS VENDAS_MES, ")
                    .AppendLine(" ( ")
                    .AppendLine("   SELECT COUNT(I.ID) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND J.STATUS =2  AND I.STATUS = 6  ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_CONFIG AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS VENDAS_HAB_MES, ")

                    .AppendLine(" ( ")
                    .AppendLine("   SELECT COUNT(I.ID) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND I.STATUS =  4 ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_CONFIG AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS VENDAS_CANC_MES, ")

                    .AppendLine(" ( ")
                    .AppendLine("   SELECT SUM(K.QUANTIDADE) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND I.STATUS =  6 ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_CONFIG AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS PONTUACAO_MENSAL, ")
                    .AppendLine(" ( ")
                    .AppendLine("   SELECT SUM(K.QUANTIDADE) ")

                    .AppendLine("   FROM TB_FUNCIONARIO H ")

                    .AppendLine("   LEFT JOIN TB_VENDA I ")
                    .AppendLine("   ON(I.ID_VENDEDOR = H.ID) ")
                    .AppendLine("   INNER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                    .AppendLine("   ON(J.ID_VENDA = I.ID) ")

                    .AppendLine("   LEFT JOIN TB_COMISSAO_PLANO_PRODUTO K ")
                    .AppendLine("   ON(K.ID_COMISSIONAMENTO = I.ID_COMISSIONAMENTO AND(K.ID_PLANO = J.ID_PLANO OR K.ID_PRODUTO = J.ID_PRODUTO)) ")
                    .AppendLine("   LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM L ")
                    .AppendLine("   ON(L.ID_FAIXA = I.ID_FAIXA AND(K.QUANTIDADE BETWEEN L.FAIXA_INICIAL AND L.FAIXA_FINAL)) ")

                    .AppendLine("   WHERE H.ID = A.ID ")
                    .AppendLine("     AND I.STATUS =  6 ")
                    .AppendLine("     AND I.DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA ")

                    .AppendLine(" ) AS PONTUACAO_DIARIA, ")

                    .AppendLine("        (SELECT FIRST 1 G.ENDERECO_CHECKIN ")
                    .AppendLine("           FROM TB_CHECKIN_VENDEDOR G ")
                    .AppendLine("          WHERE A.ID = G.ID_VENDEDOR ")
                    .AppendLine("          ORDER BY G.DATAHORACRIACAO DESC) AS ENDERECO_CHECKIN, ")

                    .AppendLine("      (SELECT SUM(I.META) ")
                    .AppendLine("        FROM TB_CAMPANHA I ")
                    .AppendLine("        WHERE I.STATUS = 1 AND ( ")
                    .AppendLine("                             (I.ID_FILIAL = A.ID_FILIAL AND I.ID_SUPERVISOR IS NULL AND I.ID_FUNCIONARIO IS NULL) ")
                    .AppendLine("                             OR (I.ID_SUPERVISOR = A.ID_SUPERVISOR AND I.ID_FUNCIONARIO IS NULL) ")
                    .AppendLine("                             OR (I.ID_FUNCIONARIO = A.ID) ")
                    .AppendLine("                            )) AS META_FUNCIONARIO, ")

                    .AppendLine($"       COALESCE(C.{_mes}, E.{_mes}) AS META_MES ")

                    .AppendLine("  FROM TB_FUNCIONARIO A ")

                    .AppendLine(" INNER JOIN TB_FUNCIONARIO B ")
                    .AppendLine(" ON(A.ID_SUPERVISOR = B.ID) ")
                    .AppendLine(" LEFT JOIN TB_META C ")
                    .AppendLine(" ON(C.ID = A.ID_META) ")
                    .AppendLine(" LEFT JOIN TB_META E ")
                    .AppendLine(" ON(E.ID = B.ID_META)  AND A.ID_COMISSIONAMENTO IS NULL ")


                    .AppendLine("  WHERE A.E_SUPERVISOR = 0 AND A.SITUACAO = 0  ");

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("AND (UPPER(A.NOME) LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }



                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sql.AppendLine("AND  A.ID_SUPERVISOR = @SUPERVISOR");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    // Primeiro dia do mês p/ comissionamento

                    using (ISession sessionNh = HibernateUtil.GetSession())
                    {
                        int ultimo_dia_config = (await new Conexao<Configuracoes_Gerais>(sessionNh).selectFirst()).Dia_Mes_Finaliza_Medicao_Comissao.Value;
                        int qtde_dias_mes_anterior = DateTime.DaysInMonth(_data_atual.AddMonths(-1).Year, _data_atual.AddMonths(-1).Month);

                        DateTime data_ini_config = new DateTime();

                        if (qtde_dias_mes_anterior >= (ultimo_dia_config + 1))
                        {
                            data_ini_config = new DateTime(_data_atual.AddMonths(-1).Year, _data_atual.AddMonths(-1).Month, ultimo_dia_config + 1);
                        }
                        else
                        {
                            data_ini_config = new DateTime(_data_atual.Year, _data_atual.Month, 1);
                        }

                        parametros.Add("@DATA_INI_CONFIG", data_ini_config);
                    }

                    //

                    DateTime ini_dia = new DateTime(_data_atual.Year, _data_atual.Month, _data_atual.Day, 0, 0, 0);

                    parametros.Add("@DATA_INI_DIA", ini_dia);
                    parametros.Add("@DATA_FIM_DIA", new DateTime(_data_atual.Year, _data_atual.Month, _data_atual.Day, 23, 59, 59));

                    parametros.Add("@DATA_INI_MES", new DateTime(_data_atual.Year, _data_atual.Month, 1));
                    parametros.Add("@DATA_FIM_MES", _data_atual);

                    parametros.Add("@ANO", DateTime.Now.Year);
                    parametros.Add("@MES", DateTime.Now.Month);

                    //Busca com paginacao
                    using (
                        DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {

                            double _vendasMes = reader["VENDAS_MES"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_MES"]) : 0;
                            double _vendasDias = reader["VENDAS_DIA"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_DIA"]) : 0;
                            double _vendasMesHab = reader["VENDAS_HAB_MES"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_HAB_MES"]) : 0;
                            double _vendasDiasHab = reader["VENDAS_HAB_DIA"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_HAB_DIA"]) : 0;
                            double _vendasCanMes = reader["VENDAS_CANC_MES"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_CANC_MES"]) : 0;



                            Dashboard_VendasListModel obj = new Dashboard_VendasListModel
                            {


                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Supervisor = Convert.ToString(reader["SUPERVISOR"]),

                                Localizacao = Convert.ToString(reader["ENDERECO_CHECKIN"]),
                                Vendas_Mes = _vendasMes != 0 ? Convert.ToString(_vendasMes) : "-",
                                Vendas_Hab_Mes = _vendasMesHab != 0 ? Convert.ToString(_vendasMesHab) : "-",
                                Vendas_Dia = _vendasDias != 0 ? Convert.ToString(_vendasDias) : "-",
                                Vendas_Hab_Dia = _vendasDiasHab != 0 ? Convert.ToString(_vendasDiasHab) : "-",
                                Vendas_Can_Dia = _vendasCanMes != 0 ? Convert.ToString(_vendasCanMes) : "-",
                                Pontuacao_Mensal = reader["PONTUACAO_MENSAL"] != DBNull.Value ? Convert.ToDouble(reader["PONTUACAO_MENSAL"]) : 0,
                                Pontuacao_Diaria = reader["PONTUACAO_DIARIA"] != DBNull.Value ? Convert.ToDouble(reader["PONTUACAO_DIARIA"]) : 0,
                            };

                            List<ComissionamentoExtrato> valor_vendas = (await new ComissionamentoApiController().GetValorNoPeriodo(obj.Id,
                                                                                                           new DateTime(_data_atual.Year,
                                                                                                                        _data_atual.Month, 1, 0, 0, 0),
                                                                                                           _data_atual)).Objeto;

                            double _metaMes = reader["META_MES"] != DBNull.Value ? Convert.ToDouble(reader["META_MES"]) : 0;
                            double _metaCampanha = reader["META_FUNCIONARIO"] != DBNull.Value ? Convert.ToDouble(reader["META_FUNCIONARIO"]) : 0;
                            //double _vendasMes = reader["VENDAS_MES"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_MES"]) : 0;
                            //double _vendasDias = reader["VENDAS_DIA"] != DBNull.Value ? Convert.ToDouble(reader["VENDAS_DIA"]) : 0;




                            int dias = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                            List<DateTime> datas = new List<DateTime>();

                            for (int i = 1; i <= dias; i++)
                            {
                                datas.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i));

                            }

                            int _dados = datas.Where(d => d.DayOfWeek > DayOfWeek.Sunday & d.DayOfWeek < DayOfWeek.Saturday).Count();
                            double _metaDia = (_metaCampanha + _metaMes) / _dados;
                            double _porc = _metaMes == 0 ? 1 : _metaMes;
                            double _porcDia = _metaDia == 0 ? 1 : _metaDia;
                            double _porVendasMes = (_vendasMes / _porc) * 100;
                            double _porVendasDia = (_vendasDias / _porcDia) * 100;
                            //return weekDays;



                            obj.Meta = (_metaCampanha + _metaMes) == 0 ? "Sem Meta" : Convert.ToString(_metaCampanha + _metaMes);
                            obj.Meta_Dia = _metaDia == 0 ? "Sem Meta" : Convert.ToString(Math.Round(_metaDia, 2));
                            obj.PorVenda_Mes = _porVendasMes != 0 ? Convert.ToString(Math.Round(_porVendasMes, 2)) : "-";
                            obj.PorVenda_Dia = _porVendasDia != 0 ? Convert.ToString(Math.Round(_porVendasDia, 2)) : "-";
                            obj.Meta = (_metaCampanha + _metaMes) == 0 ? "Sem Meta" : Convert.ToString(_metaCampanha + _metaMes);
                            obj.Meta_Dia = _metaDia == 0 ? "Sem Meta" : Convert.ToString(Math.Round(_metaDia, 2));
                            obj.PorVenda_Mes = _porVendasMes == 0 ? "-" : Convert.ToString(Math.Round(_porVendasMes, 2));

                            retorno.Add(obj);
                        }
                    }

                }
            }

            // Ordenação

            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                }
            }

            recordsTotal = retorno.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Dashboard_VendasListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        public async Task<PostApiRetorno<Int64>> EnviarEmailContrato(ISession session, Cliente cliente, Int64? IdVenda, Int64? IdCliente)
        {

            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            List<Cliente_PlanoProduto> cliente_PlanoProduto;

            try
            {
                if (String.IsNullOrEmpty(CONSTANTES.Email_De))
                {
                    retorno.Ok = true;
                    return retorno;
                }

                List<string> tmpContratos = new List<string>();


                int cont_contratos = 0;
                List<EnviarContrato> contratos = new List<EnviarContrato>();
                using (var repo = new Conexao<Configuracoes_Gerais>(session))
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var config = (await repo.selectFirst());
                    string Provedor = config.Nome_Cliente_Sinapse;

                    if (cliente == null)
                        cliente = (await new Conexao<Cliente>(session).selectFirst(p => p.Id == IdCliente));

                    EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);

                    if (config.Utiliza_Envio_Automatico_Contrato)
                    {
                        if (IdVenda.HasValue)
                            contratos = GetContratoGerarPdf(session, cliente.Id, IdVenda, null).Result.Objeto;
                        else
                            contratos = GetContratoGerarPdf(session, cliente.Id, null, null).Result.Objeto;


                        foreach (EnviarContrato contrato in contratos)
                        {
                            tmpContratos.Add(contrato.Caminho_Contrato);

                            emailService.AnexarBody(Convert.ToString(cont_contratos), contrato.Caminho_Contrato);
                            cont_contratos++;
                        }
                    }

                    Venda venda = null;

                    if (IdVenda.HasValue)
                    {
                        venda = repoVenda.selectFirst(m => m.Id == IdVenda).Result;
                        cliente_PlanoProduto = repoClientePlanoProduto.select(m => m.Cliente.Id == cliente.Id).Result.ToList();

                        if (venda.Status == Status_Venda.Venda_Aprovada)
                        {
                            await EnviaEmail_VendaAprovada(cliente_PlanoProduto, cliente, session, Provedor, emailService, venda.Id);
                            await new ClienteApiController().AtualizaClienteOla(cliente.Id);
                        }
                        else
                        {
                            emailService.EnviarMensagem(cliente.Email_Principal, "Contrato", String.Empty, CONSTANTES.Email_UsaSsl == "S");
                        }
                    }
                    else
                    {
                        emailService.EnviarMensagem(cliente.Email_Principal, "Contrato", String.Empty, CONSTANTES.Email_UsaSsl == "S");
                    }

                    foreach (string arquivo in tmpContratos)
                    {
                        System.IO.File.Delete(arquivo);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        public async Task EnviaEmail_VendaAprovada(List<Cliente_PlanoProduto> cliente_PlanoProduto, Cliente cliente, ISession session, String Provedor, EmailUtil emailService, Int64 id_venda)
        {
            cliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(session).select(m => m.Cliente.Id == cliente.Id).Result.ToList();

            string emailFilialEnvio = CONSTANTES.EmailFilialEnvio;
            string telefoneFilial = CONSTANTES.TelefoneFilial;
            string body = "";
            List<Cliente_PlanoProduto> planosProdutosVenda = cliente_PlanoProduto.Where(m => m.Id_Venda == id_venda).ToList();

            var tabelaPlanos = GerarTabelaProdutosHTML(planosProdutosVenda);

            string urlLogoProvedor = (new Conexao<Configuracoes_Gerais>(session).selectFirst()).Result.OLA_URL_Logo;
            string email_suporte = (new Conexao<Configuracoes_Gerais>(session).selectFirst()).Result.Email_Suporte;
            string telefone_suporte = (new Conexao<Configuracoes_Gerais>(session).selectFirst()).Result.Telefone_Suporte;

            body =
                System.IO.File.ReadAllText(
                    @"" + CONSTANTES.CaminhoFisico + "//Arquivos//email_venda_aprovada.html",
                    System.Text.Encoding.UTF8).Replace("{{NOME_CLIENTE}}",
                    cliente.NomeFantasia
                )
                .Replace("{{URL_LOGO}}", urlLogoProvedor)
                .Replace("{{NOME_ISP}}", Provedor)
                .Replace("{{TABELA}}", tabelaPlanos)
                .Replace("{{EMAIL_FILIAL}}", emailFilialEnvio)
                .Replace("{{TELEFONE_FILIAL}}", telefoneFilial)
                .Replace("{{NUMERO_TELENONE_ISP}}", !String.IsNullOrEmpty(telefone_suporte) ? telefone_suporte : null)
                .Replace("{{EMAIL}}", !String.IsNullOrEmpty(email_suporte) ? email_suporte : null);

            using (Termo_AdesaoApiController api = new Termo_AdesaoApiController())
            {
                string pdf = await api.GetPdfTermoAdesaoEmail(id_venda);

                if (System.IO.File.Exists(pdf))
                {
                    emailService.Anexar(pdf);
                }
            }
            ;

            emailService.EnviarMensagem_VendaAprovada(cliente.Email_Principal, "Contrato", body, true);
        }


        [HttpGet()]
        [Route("api/Vendaapi/GetHistoricoContato")]
        public async Task<GetApiRetorno<List<Venda_Historico_Contrato_View>>> GetHistoricoContato(Int64 idVenda)
        {
            GetApiRetorno<List<Venda_Historico_Contrato_View>> retorno = new GetApiRetorno<List<Venda_Historico_Contrato_View>>();
            try
            {
                List<Venda_Ultimo_Contato> historico_contatos = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (Conexao<Venda_Ultimo_Contato> repository = new Conexao<Venda_Ultimo_Contato>(session))
                    {
                        historico_contatos = (await repository.select(s => s.Id_Venda == idVenda)).ToList();
                    }
                }

                retorno.Objeto = historico_contatos.Select(p => new Venda_Historico_Contrato_View
                {
                    Data_Ultimo_Contato = p.Data_Ultimo_Contato.ToString("dd/MM/yyyy"),
                    Forma_Contato = p.Forma_Contato,
                    Observacao = p.Observacao
                }).ToList();

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Vendaapi/Historico_UltimosContatos_AcaoStatus")]
        public async Task<GetApiRetorno<List<Historico_UltimosContatos_AcaoStatusModel>>> Historico_UltimosContatos_AcaoStatus(Int64 idVenda)
        {
            GetApiRetorno<List<Historico_UltimosContatos_AcaoStatusModel>> retorno = new GetApiRetorno<List<Historico_UltimosContatos_AcaoStatusModel>>();
            try
            {
                List<Venda_Ultimo_Contato> historico_contatos = null;
                AlterarStatusViewModel value = new AlterarStatusViewModel();
                value.ids = new List<long>();
                value.ids.Add(idVenda);


                Venda venda = null;


                using (ISession session = HibernateUtil.GetSession())
                {

                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        venda = (await repository.selectFirst(s => s.Id == idVenda));
                    }
                    using (Conexao<Venda_Ultimo_Contato> repository = new Conexao<Venda_Ultimo_Contato>(session))
                    {
                        historico_contatos = (await repository.select(s => s.Id_Venda == idVenda)).ToList();
                    }
                }

                retorno.Objeto = historico_contatos.OrderByDescending(m => m.Data_Ultimo_Contato).Select(p => new Historico_UltimosContatos_AcaoStatusModel
                {
                    Data_Ultimo_Contato = p.Data_Ultimo_Contato.ToString("dd/MM/yyyy HH:mm"),
                    Forma_Contato = p.Forma_Contato,
                    Observacao = p.Observacao,
                    Usuario = p.UsuarioCriacao,

                    StatusVenda = p.Status_Venda,
                    Acao = p.Acao,
                    Valor = p.Valor,
                    Valor_Atualizado = p.Valor_Atualizado,

                    Id_venda = p.Id_Venda
                }).ToList();

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Vendaapi/HistoricoVendaAlteracao")]
        public async Task<DataTableResponse<LogGenericoModel>> HistoricoVendaAlteracao(HistoricoAlteracaoVenda_DataTableRequest request)
        {
            List<LogGenericoModel> retorno = new List<LogGenericoModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFromVendas = new StringBuilder();
                StringBuilder sqlFromEtapaVendas = new StringBuilder();

                // TB_VENDAS
                sqlFromVendas.AppendLine("    FROM TB_VENDAS");
                sqlFromVendas.AppendLine("    WHERE DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM AND LOG_INTEGRACAO IS NULL");

                //TB_ETAPA_VENDAS
                sqlFromEtapaVendas.AppendLine("    FROM TB_ETAPA_VENDAS");
                sqlFromEtapaVendas.AppendLine("    WHERE DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM");

                parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
                parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));


                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFromVendas.AppendLine("  AND ((UPPER(OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFromVendas.AppendLine("  AND IDLOG = @ID ");
                    sqlFromEtapaVendas.AppendLine("AND OBJETO = @ID ");
                    parametros.Add("@ID", request.Id);
                }

                if (request.TipoOperacao != null)
                {
                    sqlFromVendas.AppendLine("  AND TIPOOPERACAO = @TIPOOPERACAO ");
                    sqlFromEtapaVendas.AppendLine("AND TIPOOPERACAO = @TIPOOPERACAO ");
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFromVendas.AppendLine("  AND CAMPO = @CAMPO ");
                    sqlFromEtapaVendas.AppendLine("AND CAMPO = @CAMPO ");
                    parametros.Add("@CAMPO", request.Campo);
                }

                //union
                sqlFromVendas.AppendLine("  UNION");


                //Count
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) CO ");
                    StringBuilder sql = new StringBuilder("SELECT SUM(CO) AS CO FROM (");

                    sql.Append(sqlCount.ToString());
                    sql.AppendLine(sqlFromVendas.ToString());
                    sql.Append(sqlCount.ToString());
                    sql.AppendLine(sqlFromEtapaVendas.ToString());
                    sql.Append(") AS TOTAL");

                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                // Select
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT ID, NOMEUSUARIOCRIACAO, USUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, OBJETO, CAMPO, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, ID_CLIENTE_PLANO_PRODUTO, ID_FORMA_PAGTO_CLIENTE,MOTIVO FROM (");
                    sql.AppendLine($" SELECT ID, NOMEUSUARIOCRIACAO, USUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, OBJETO, CAMPO, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, ID_CLIENTE_PLANO_PRODUTO, ID_FORMA_PAGTO_CLIENTE,MOTIVO {sqlFromVendas}");
                    sql.AppendLine($"SELECT ID, NOMEUSUARIOCRIACAO, USUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, OBJETO, CAMPO, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, NULL, NULL, MOTIVO {sqlFromEtapaVendas}");
                    sql.AppendLine(") AS CONSULTA");


                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // CHAVE
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY OBJETO ");
                                    else
                                        sql.Append(" ORDER BY OBJETO DESC ");
                                    break;
                                }
                            case 1:     // CAMPO
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY CAMPO ");
                                    else
                                        sql.Append(" ORDER BY CAMPO DESC ");
                                    break;
                                }
                            case 6: //Data Hora
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY DATAHORACRIACAO ");
                                    else
                                        sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
                                    break;
                                }
                        }
                    }

                    // Busca com paginacao
                    using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogGenericoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<StatusLogVendas>(Convert.ToString(reader["TIPOOPERACAO"])),

                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                IdClientePlanoProduto = Convert.IsDBNull(reader["ID_CLIENTE_PLANO_PRODUTO"]) ? null : Convert.ToString(reader["ID_CLIENTE_PLANO_PRODUTO"]),
                                IdFormaPagamentoCliente = Convert.IsDBNull(reader["ID_FORMA_PAGTO_CLIENTE"]) ? null : Convert.ToString(reader["ID_FORMA_PAGTO_CLIENTE"])
                            });
                        }
                    }
                }
            }

            // Page e retornando DataTable
            return new DataTableResponse<LogGenericoModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno.OrderByDescending(p => p.DataHoraCriacao),
                error = ""
            };
        }

        [HttpGet]
        [Route("api/VendaApi/DetalhesLOG_HistoricoVenda")]
        public async Task<GetApiRetorno<DetalhesHistoricoVenda_LOG>> DetalhesLOG_HistoricoVenda(Int64 id, String tipo)
        {
            var retornoAPI = new GetApiRetorno<DetalhesHistoricoVenda_LOG>()
            {
                Objeto = new DetalhesHistoricoVenda_LOG()
            };

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLOG = HibernateUtil.GetSessionLog())
                using (var repositoryCPP_LOG = new Conexao<LogVendas_Detalhes>(sessionLOG))
                using (var repositoryFormaPagamentoCliente_LOG = new Conexao<LogFormaPagamentoCliente>(sessionLOG))
                using (var repositoryPlano = new Conexao<Plano>(session))
                using (var repositoryCombo = new Conexao<Combo>(session))
                using (var repositoryEndereco_Cliente = new Conexao<Endereco_Cliente>(session))
                using (var repositoryForma_Pagamento_Cliente = new Conexao<Forma_Pagamento_Cliente>(session))
                using (var repositoryForma_Pagamento = new Conexao<Forma_Pagamento>(session))
                using (var repositoryClienteContrato = new Conexao<ClienteContrato>(session))
                using (var repositoryBanco = new Conexao<Banco>(session))
                {
                    var objLog = new DetalhesHistoricoVenda_LOG();

                    if (tipo == "CPP")
                    {

                        using (var com = sessionLOG.Connection.CreateCommand())
                        {

                            var condicoes = new List<string>();
                            var parametros = new Dictionary<string, object>();

                            var sql = new StringBuilder(
                                @$"SELECT A.ID_CPP, A.ID_COMBO, A.ID_PLANO, A.VALOR_REAL, A.VALOR_INSTALACAO_REAL, A.PARCELAS_INSTALACAO, A.VALOR_INSTALACAO_ENTRADA, A.VALOR_INSTALACAO_FINAL, A.DIAS_TESTE_GRATIS,
                                    A.MESES_CARENCIA, A.ID_CLIENTE_CONTRATO, A.ID_ENDERECO_INSTALACAO, A.ID_FORMA_PAGTO_CLIENTE FROM TB_CPP_EXCLUSAO A");
                            condicoes.Add("A.ID_CPP = @ID");
                            parametros.Add("@ID", id);

                            if (condicoes.Count > 0)
                            {
                                sql.AppendLine(" WHERE " + String.Join(" AND ", condicoes));
                            }
                            sql.AppendLine(" ORDER BY A.ID DESC");

                            Int64? id_endereco_instalacao = 0, id_combo = 0, id_plano = 0, id_cliente_contrato = 0, id_forma_pagto_cliente = 0;
                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLOG.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    id_endereco_instalacao = Convert.IsDBNull(reader["ID_ENDERECO_INSTALACAO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_ENDERECO_INSTALACAO"]);
                                    id_combo = Convert.IsDBNull(reader["ID_COMBO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_COMBO"]);
                                    id_plano = Convert.IsDBNull(reader["ID_PLANO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_PLANO"]);
                                    id_cliente_contrato = Convert.IsDBNull(reader["ID_CLIENTE_CONTRATO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_CLIENTE_CONTRATO"]);
                                    id_forma_pagto_cliente = Convert.IsDBNull(reader["ID_FORMA_PAGTO_CLIENTE"]) ? (Int64?)null : Convert.ToInt64(reader["ID_FORMA_PAGTO_CLIENTE"]);

                                    if (!Convert.IsDBNull(reader["ID_COMBO"]))
                                        objLog.DescPlano_Combo = repositoryCombo.selectFirst(s => s.Id == Convert.ToInt64(reader["ID_COMBO"])).Result.Descricao;
                                    else if (!Convert.IsDBNull(reader["ID_PLANO"]))
                                        objLog.DescPlano_Combo = repositoryPlano.selectFirst(s => s.Id == Convert.ToInt64(reader["ID_PLANO"])).Result.Descricao;

                                    objLog.FlagPlano = !Convert.IsDBNull(reader["ID_COMBO"]) ? false : true;
                                    objLog.Valor_Real = Convert.IsDBNull(reader["VALOR_REAL"]) ? "R$ 0,00" : $"R$ {Convert.ToDecimal(reader["VALOR_REAL"]):N2}";
                                    objLog.Valor_Instalacao_Real = Convert.IsDBNull(reader["VALOR_INSTALACAO_REAL"]) ? "R$ 0,00" : $"R$ {Convert.ToDecimal(reader["VALOR_INSTALACAO_REAL"]):N2}";
                                    objLog.Valor_Entrada_Instalacao = Convert.IsDBNull(reader["VALOR_INSTALACAO_ENTRADA"]) ? "R$ 0,00" : $"R$ {Convert.ToDecimal(reader["VALOR_INSTALACAO_ENTRADA"]):N2}";
                                    objLog.Valor_Instalacao_Final = Convert.IsDBNull(reader["VALOR_INSTALACAO_FINAL"]) ? "R$ 0,00" : $"R$ {Convert.ToDecimal(reader["VALOR_INSTALACAO_FINAL"]):N2}";
                                    objLog.Nr_Parcelas_Instalacao = Convert.IsDBNull(reader["PARCELAS_INSTALACAO"]) ? "0" : $"{Convert.ToString(reader["PARCELAS_INSTALACAO"])}";
                                    objLog.Dias_Teste_Gratis = Convert.IsDBNull(reader["DIAS_TESTE_GRATIS"]) ? "0" : $"{Convert.ToString(reader["DIAS_TESTE_GRATIS"])}";

                                    var descMes = !Convert.IsDBNull(reader["MESES_CARENCIA"]) ? (Convert.ToInt32(reader["MESES_CARENCIA"]) == 1 ? "mês" : "meses") : "meses";
                                    objLog.Vigencia_Contrato = !Convert.IsDBNull(reader["MESES_CARENCIA"]) ? $"{Convert.ToInt32(reader["MESES_CARENCIA"])} {descMes}" : "0";

                                    if (!Convert.IsDBNull(reader["ID_CLIENTE_CONTRATO"]))
                                        objLog.Modelo_Contrato = repositoryClienteContrato.selectFirst(s => s.Id == Convert.ToInt64(reader["ID_CLIENTE_CONTRATO"])).Result.Descricao;
                                }
                            }

                            var objFormaPagtoCliente = await repositoryForma_Pagamento_Cliente.selectFirst(s => s.Id == id_forma_pagto_cliente);
                            if (objFormaPagtoCliente != null)
                            {
                                objLog.DescFormaPagtoCliente = "Vencto Dia";
                                if (objFormaPagtoCliente.Dia_Vencimento != null)
                                    objLog.DescFormaPagtoCliente += $" {objFormaPagtoCliente.Dia_Vencimento}";

                                if (!string.IsNullOrEmpty(objFormaPagtoCliente.Forma_Pagamento?.Descricao))
                                    objLog.DescFormaPagtoCliente += $" - {objFormaPagtoCliente.Forma_Pagamento.Descricao}";

                                if (!string.IsNullOrEmpty(objFormaPagtoCliente.Endereco_Cobranca?.Logradouro))
                                    objLog.DescFormaPagtoCliente += $" - {objFormaPagtoCliente.Endereco_Cobranca.Logradouro}";

                                if (!string.IsNullOrEmpty(objFormaPagtoCliente.Endereco_Cobranca?.Numero))
                                    objLog.DescFormaPagtoCliente += $", {objFormaPagtoCliente.Endereco_Cobranca.Numero}";
                            }

                            var objEnderecoInstalacao = await repositoryEndereco_Cliente.selectFirst(s => s.Id == id_endereco_instalacao);
                            if (objEnderecoInstalacao != null)
                            {
                                var partes = new List<string>();

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Logradouro))
                                    partes.Add(objEnderecoInstalacao.Logradouro);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Numero))
                                    partes.Add(objEnderecoInstalacao.Numero);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Bairro))
                                    partes.Add(objEnderecoInstalacao.Bairro);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Cidade))
                                    partes.Add(objEnderecoInstalacao.Cidade);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.UF))
                                    partes.Add(objEnderecoInstalacao.UF);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Cep))
                                    partes.Add(objEnderecoInstalacao.Cep);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Complemento))
                                    partes.Add(objEnderecoInstalacao.Complemento);

                                if (!string.IsNullOrEmpty(objEnderecoInstalacao.Ponto_Referencia))
                                    partes.Add(objEnderecoInstalacao.Ponto_Referencia);

                                objLog.DescEnderecoInstalacao = string.Join(" - ", partes);
                            }
                        }
                    }
                    else
                    {
                        using (var com = sessionLOG.Connection.CreateCommand())
                        {

                            var condicoes = new List<string>();
                            var parametros = new Dictionary<string, object>();

                            var sql = new StringBuilder(
                                @$"SELECT A.ID_ENDERECO_COBRANCA, A.ID_FORMA_PAGAMENTO_CLIENTE, A.FORMA_ENVIO, A.ID_FORMA_PAGAMENTO, A.DIA_VENCIMENTO, A.CONTA_CORRENTE, A.AGENCIA, A.ID_BANCO 
                                    FROM TB_LOG_FORMA_PAGAMENTO_CLIENTE A");
                            condicoes.Add("A.ID_FORMA_PAGAMENTO_CLIENTE = @ID");
                            parametros.Add("@ID", id);

                            if (condicoes.Count > 0)
                            {
                                sql.AppendLine(" WHERE " + String.Join(" AND ", condicoes));
                            }
                            sql.AppendLine(" ORDER BY A.ID DESC");

                            Int64? id_endereco_cobranca = 0, id_forma_pagamento_cliente = 0, forma_envio = 0, id_forma_pagamento = 0;
                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLOG.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    id_endereco_cobranca = Convert.IsDBNull(reader["ID_ENDERECO_COBRANCA"]) ? (Int64?)null : Convert.ToInt64(reader["ID_ENDERECO_COBRANCA"]);
                                    id_forma_pagamento_cliente = Convert.IsDBNull(reader["ID_FORMA_PAGAMENTO_CLIENTE"]) ? (Int64?)null : Convert.ToInt64(reader["ID_FORMA_PAGAMENTO_CLIENTE"]);
                                    id_forma_pagamento = Convert.IsDBNull(reader["ID_FORMA_PAGAMENTO"]) ? (Int64?)null : Convert.ToInt64(reader["ID_FORMA_PAGAMENTO"]);
                                    forma_envio = Convert.IsDBNull(reader["FORMA_ENVIO"]) ? (Int64?)null : Convert.ToInt64(reader["FORMA_ENVIO"]);

                                    if (forma_envio.HasValue)
                                    {
                                        objLog.FormaEnvioFatura = forma_envio == 0 ? "E-mail" : "Correio";
                                    }

                                    objLog.Dia_Vencimento = Convert.IsDBNull(reader["DIA_VENCIMENTO"]) ? "-" : Convert.ToString(reader["DIA_VENCIMENTO"]);
                                    objLog.DescConta = Convert.IsDBNull(reader["CONTA_CORRENTE"]) ? "-" : Convert.ToString(reader["CONTA_CORRENTE"]);
                                    objLog.DescAgencia = Convert.IsDBNull(reader["AGENCIA"]) ? "-" : Convert.ToString(reader["AGENCIA"]);

                                    if (!Convert.IsDBNull(reader["ID_BANCO"]))
                                        objLog.DescBanco = repositoryBanco.selectFirst(s => s.Id == Convert.ToInt64(reader["ID_BANCO"])).Result.Descricao;
                                    else
                                        objLog.DescBanco = "-";
                                }
                            }

                            var objFormaPagto = await repositoryForma_Pagamento.selectFirst(s => s.Id == id_forma_pagamento);
                            if (objFormaPagto != null)
                            {
                                objLog.DescFormaPagto = objFormaPagto?.Descricao;
                            }

                            var objEnderecoCobranca = await repositoryEndereco_Cliente.selectFirst(s => s.Id == id_endereco_cobranca);
                            if (objEnderecoCobranca != null)
                            {
                                var partes = new List<string>();

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Logradouro))
                                    partes.Add(objEnderecoCobranca.Logradouro);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Numero))
                                    partes.Add(objEnderecoCobranca.Numero);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Bairro))
                                    partes.Add(objEnderecoCobranca.Bairro);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Cidade))
                                    partes.Add(objEnderecoCobranca.Cidade);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.UF))
                                    partes.Add(objEnderecoCobranca.UF);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Cep))
                                    partes.Add(objEnderecoCobranca.Cep);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Complemento))
                                    partes.Add(objEnderecoCobranca.Complemento);

                                if (!string.IsNullOrEmpty(objEnderecoCobranca.Ponto_Referencia))
                                    partes.Add(objEnderecoCobranca.Ponto_Referencia);

                                objLog.DescEnderecoCobranca = string.Join(" - ", partes);
                            }
                        }
                    }

                    retornoAPI.Ok = true;
                    retornoAPI.Objeto = objLog;
                }
            }
            catch (Exception ex)
            {
                retornoAPI.Ok = false;
                retornoAPI.Objeto = null;
                retornoAPI.Mensagem = ex.Message;
                if (ex.InnerException != null) retornoAPI.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retornoAPI;
        }

        //[HttpPost()]
        //[Route("api/Vendaapi/HistoricoVendaAlteracao")]
        //public async Task<DataTableResponse<LogGenericoModel>> HistoricoVendaAlteracao(HistoricoAlteracaoVenda_DataTableRequest request)
        //{
        //    List<LogGenericoModel> retorno = new List<LogGenericoModel>();

        //    Int32 recordsTotal = 0;

        //    using (ISession session = HibernateUtil.GetSessionLog())
        //    {

        //        Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //        StringBuilder sqlFrom = new StringBuilder();


        //        sqlFrom.AppendLine("FROM TB_VENDAS A ");


        //        sqlFrom.AppendLine("WHERE A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");

        //        parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
        //        parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));


        //        if (!String.IsNullOrEmpty(request.search.value))
        //        {
        //            sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
        //            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
        //        }

        //        if (request.Id.HasValue)
        //        {
        //            sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
        //            parametros.Add("@ID", request.Id);
        //        }

        //        if (request.TipoOperacao != null)
        //        {
        //            sqlFrom.AppendLine(" AND A.TIPOOPERACAO = @TIPOOPERACAO ");
        //            parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
        //        }

        //        if (!String.IsNullOrEmpty(request.Objeto))
        //        {
        //            sqlFrom.AppendLine(" AND A.OBJETO = @OBJETO ");
        //            parametros.Add("@OBJETO", request.Objeto);
        //        }

        //        if (!String.IsNullOrEmpty(request.Campo))
        //        {
        //            sqlFrom.AppendLine(" AND A.CAMPO = @CAMPO ");
        //            parametros.Add("@CAMPO", request.Campo);
        //        }

        //        Count
        //        using (DbCommand com = session.Connection.CreateCommand())
        //        {
        //            StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO ");

        //            sql.Append(sqlFrom.ToString());

        //            using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    recordsTotal = Convert.ToInt32(reader["CO"]);
        //                }
        //            }
        //        }

        //        Select
        //        using (DbCommand com = session.Connection.CreateCommand())
        //        {

        //            StringBuilder sql = new StringBuilder();

        //            sql.AppendLine("SELECT A.ID, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS ");
        //            sql.AppendLine(sqlFrom.ToString());

        //            Ordenacao
        //            if (request.order != null && request.order.Any())
        //            {
        //                int sortColumnIndex = request.order[0].column;
        //                string sortDirection = request.order[0].dir;
        //                switch (sortColumnIndex)
        //                {
        //                    case 0:     // CHAVE
        //                        {
        //                            if (sortDirection == "asc")
        //                                sql.Append(" ORDER BY OBJETO ");
        //                            else
        //                                sql.Append(" ORDER BY OBJETO DESC ");
        //                            break;
        //                        }
        //                    case 1:     // CAMPO
        //                        {
        //                            if (sortDirection == "asc")
        //                                sql.Append(" ORDER BY CAMPO ");
        //                            else
        //                                sql.Append(" ORDER BY CAMPO DESC ");
        //                            break;
        //                        }
        //                    case 6: //Data Hora
        //                        {
        //                            if (sortDirection == "asc")
        //                                sql.Append(" ORDER BY DATAHORACRIACAO ");
        //                            else
        //                                sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
        //                            break;
        //                        }
        //                }
        //            }

        //            Busca com paginacao
        //            using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    retorno.Add(new LogGenericoModel
        //                    {
        //                        Id = Convert.ToInt64(reader["ID"]),
        //                        Chave = Convert.ToString(reader["OBJETO"]),
        //                        Campo = Convert.ToString(reader["CAMPO"]),

        //                        Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
        //                        Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
        //                        TipoOperacao = HtmlEnumExtensions.GetEnumDescription<StatusLogVendas>(Convert.ToString(reader["TIPOOPERACAO"])),

        //                        UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
        //                        Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
        //                        DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
        //                    });
        //                }
        //            }

        //        }
        //    }

        //    Page e retornando DataTable
        //    return new DataTableResponse<LogGenericoModel>
        //    {
        //        draw = request.draw,
        //        recordsTotal = recordsTotal,
        //        recordsFiltered = recordsTotal,
        //        data = retorno,
        //        error = ""
        //    };

        //}

        [HttpPost()]
        [Route("api/Vendaapi/Dashboard_Vendas_Item")]
        public async Task<DataTableResponse<Dashboard_Vendas_ItemListModel>> Dashboard_Vendas_Item(Dashboard_Vendas_DataTableRequest request)
        {
            List<Dashboard_Vendas_ItemListModel> retorno = new List<Dashboard_Vendas_ItemListModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Meta> repoMeta = new Conexao<Meta>(session))
            {
                Meta metaAno = await repoMeta.selectFirst(p => p.Ano == DateTime.Now.Year);

                Dictionary<string, object> parametros = new Dictionary<string, object>();

                //Select
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT COALESCE(E.DESCRICAO, F.DESCRICAO) AS NOME_PLANOPRODUTO,");
                    sql.AppendLine("       D.QUANTIDADE AS PONTOS, (D.QUANTIDADE * G.COEFICIENTE) AS VALOR,");
                    sql.AppendLine("       L.TITULO AS CAMPANHA");

                    sql.AppendLine("FROM TB_FUNCIONARIO A");

                    sql.AppendLine("LEFT JOIN TB_VENDA B");
                    sql.AppendLine("ON(B.ID_VENDEDOR = A.ID)");

                    sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO C");
                    sql.AppendLine("ON(C.ID_VENDA = B.ID AND(C.ID_PLANO IS NOT NULL OR C.ID_PRODUTO IS NOT NULL))");

                    sql.AppendLine("LEFT JOIN TB_PLANO E");
                    sql.AppendLine("ON(E.ID = C.ID_PLANO)");

                    sql.AppendLine("LEFT JOIN TB_PRODUTO F");
                    sql.AppendLine("ON(F.ID = C.ID_PRODUTO)");

                    sql.AppendLine("LEFT JOIN TB_CAMPANHA L");
                    sql.AppendLine("ON(L.ID = C.ID_CAMPANHA)");

                    sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO D");
                    sql.AppendLine("ON(D.ID_COMISSIONAMENTO = B.ID_COMISSIONAMENTO AND(D.ID_PLANO = C.ID_PLANO OR D.ID_PRODUTO = C.ID_PRODUTO))");

                    sql.AppendLine("LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM G");
                    sql.AppendLine("ON(G.ID_FAIXA = B.ID_FAIXA AND(D.QUANTIDADE BETWEEN G.FAIXA_INICIAL AND G.FAIXA_FINAL))");

                    sql.AppendLine("WHERE A.ID = @ID_FUNCIONARIO");
                    sql.AppendLine("  AND B.STATUS = 6");
                    sql.AppendLine("  AND B.DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA");

                    sql.AppendLine("UNION");

                    sql.AppendLine("SELECT PONTOS_PP.NOME_PLANOPRODUTO, PONTOS_PP.PONTOS,");
                    sql.AppendLine("       (PONTOS_PP.PONTOS * G.COEFICIENTE) AS VALOR, PONTOS_PP.CAMPANHA");
                    sql.AppendLine("FROM(");
                    sql.AppendLine("    SELECT C.DESCRICAO AS NOME_PLANOPRODUTO, SUM(D.QUANTIDADE) AS PONTOS,");
                    sql.AppendLine("           L.TITULO AS CAMPANHA, B.ID_FAIXA");

                    sql.AppendLine("    FROM TB_FUNCIONARIO A");

                    sql.AppendLine("    LEFT JOIN TB_VENDA B");
                    sql.AppendLine("    ON(B.ID_VENDEDOR = A.ID)");

                    sql.AppendLine("    INNER JOIN TB_CLIENTE_PLANO_PRODUTO CPP");
                    sql.AppendLine("    ON(CPP.ID_VENDA = B.ID AND CPP.ID_COMBO IS NOT NULL)");

                    sql.AppendLine("    INNER JOIN TB_COMBO C");
                    sql.AppendLine("    ON(C.ID = CPP.ID_COMBO)");

                    sql.AppendLine("    INNER JOIN TB_COMBO_PLANO_PRODUTO COMBO_PP");
                    sql.AppendLine("    ON(COMBO_PP.ID_COMBO = C.ID)");

                    sql.AppendLine("    LEFT JOIN TB_PLANO PLANO");
                    sql.AppendLine("    ON(PLANO.ID = COMBO_PP.ID_PLANO)");

                    sql.AppendLine("    LEFT JOIN TB_PRODUTO PROD");
                    sql.AppendLine("    ON(PROD.ID = COMBO_PP.ID_PRODUTO)");

                    sql.AppendLine("    LEFT JOIN TB_CAMPANHA L");
                    sql.AppendLine("    ON(L.ID = CPP.ID_CAMPANHA)");

                    sql.AppendLine("    LEFT JOIN TB_COMISSAO_PLANO_PRODUTO D");
                    sql.AppendLine("    ON(D.ID_COMISSIONAMENTO = B.ID_COMISSIONAMENTO");
                    sql.AppendLine("       AND(D.ID_PLANO = COMBO_PP.ID_PLANO OR D.ID_PRODUTO = COMBO_PP.ID_PRODUTO))");

                    sql.AppendLine("    WHERE A.ID = @ID_FUNCIONARIO");
                    sql.AppendLine("      AND B.STATUS = 6");
                    sql.AppendLine("      AND B.DATA_VENDA BETWEEN @DATA_INI_DIA AND @DATA_FIM_DIA");
                    sql.AppendLine("    GROUP BY C.DESCRICAO, L.TITULO, B.ID_FAIXA");
                    sql.AppendLine(") AS PONTOS_PP");

                    sql.AppendLine("LEFT JOIN TB_FAIXA_COMISSIONAMENTO_ITEM G");
                    sql.AppendLine("ON(G.ID_FAIXA = PONTOS_PP.ID_FAIXA");
                    sql.AppendLine("   AND(PONTOS_PP.PONTOS BETWEEN G.FAIXA_INICIAL AND G.FAIXA_FINAL))");

                    parametros.Add("@ID_FUNCIONARIO", request.Id_Funcionario);
                    parametros.Add("@DATA_INI_DIA", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0));
                    parametros.Add("@DATA_FIM_DIA", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59));

                    //Busca com paginacao
                    using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        List<Dashboard_Vendas_ItemListModel> tmp = new List<Dashboard_Vendas_ItemListModel>();

                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Dashboard_Vendas_ItemListModel
                            {
                                Nome_Plano_Produto = Convert.ToString(reader["NOME_PLANOPRODUTO"]),
                                Pontos = reader["PONTOS"] != DBNull.Value ? String.Format("{0:N2}", Convert.ToDouble(reader["PONTOS"])) : String.Empty,
                                Valor_Dinheiro = reader["VALOR"] != DBNull.Value ? String.Format("{0:N2}", Convert.ToDouble(reader["VALOR"])) : "-",
                                Nome_Campanha = Convert.ToString(reader["CAMPANHA"])
                            });
                        }
                    }
                }
            }

            recordsTotal = retorno.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Dashboard_Vendas_ItemListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        #region mobile Minhas pendencias
        [HttpPost()]
        [Route("api/Vendaapi/inserirDadosUltimoContato")]
        public async Task<GetApiRetorno<Boolean>> inserirDadosUltimoContato(Minhas_Pendencias_DadosUltimoContato dados)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();

            try
            {
                Venda venda = null;
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        venda = (await repository.select(s => s.Id == dados.Id_venda)).FirstOrDefault();

                        venda.Data_Ultimo_Contato = dados.DataUltimoContato;

                        await repository.Update(venda);
                    }

                    using (Conexao<Venda_Ultimo_Contato> repository = new Conexao<Venda_Ultimo_Contato>(session))
                    {
                        Venda_Ultimo_Contato venda_Ultimo_Contato = new Venda_Ultimo_Contato()
                        {
                            Id_Venda = dados.Id_venda,
                            Data_Ultimo_Contato = dados.DataUltimoContato,
                            Forma_Contato = dados.Assunto,
                            Observacao = dados.Observacao
                        };

                        await repository.Save(venda_Ultimo_Contato);
                    }

                }

                retorno.Ok = true;
                retorno.Objeto = true;

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        #endregion

        [HttpGet()]
        [Route("api/Vendaapi/getPendencias")]
        public async Task<GetApiRetorno<List<object>>> getPendencias(bool somenteExpiradas, int status, Int64 idVendedor)
        {
            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();
            try
            {
                int maxDias = 0;
                List<Cliente> clientes = null;
                List<Venda> vendas = null;
                Venda vendaEspecifica = vendas.FirstOrDefault();
                Status_Venda statusDaVenda = (Status_Venda)Enum.Parse(typeof(Status_Venda), status.ToString());

                using (ISession session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                using (Conexao<Elemento_Rede_Porta> repoElemento_Porta = new Conexao<Elemento_Rede_Porta>(session))
                {
                    Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                    IList<Elemento_Rede_Porta> listaElementos = (await repoElemento_Porta.select(p => p.Cliente_Plano_Prod == vendaEspecifica.Id_Cliente));

                    if (statusDaVenda == Status_Venda.Analise_Cliente)
                        maxDias = config.Max_Dias_Sem_Contato ?? 0;
                    else if (statusDaVenda == Status_Venda.Aguardando_Avaliacao_Comercial)
                        maxDias = config.Qtde_Dias_Avaliacao_Comercial ?? 0;


                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        List<ICriterion> criteria = new List<ICriterion>();
                        criteria.Add(Expression.Where<Venda>(p => p.Vendedor.Id == idVendedor));
                        criteria.Add(Expression.Where<Venda>(p => p.Status == statusDaVenda));
                        if (somenteExpiradas)
                        {
                            DateTime data = DateTime.Now.AddDays(-1);


                            IProjection dataStatus = null;
                            IProjection dataUltimoContato = Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property(nameof(Venda.Data_Ultimo_Contato)));
                            if (statusDaVenda == Status_Venda.Aguardando_Avaliacao_Comercial)
                                dataStatus = Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property(nameof(Venda.Data_Alteracao_Status)));
                            else if (statusDaVenda == Status_Venda.Aguardando_Avaliacao_Comercial)
                                maxDias = config.Qtde_Dias_Avaliacao_Comercial ?? 0;

                            IProjection dataVenda = Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property(nameof(Venda.Data_Venda)));

                            IProjection coalesce = Projections.SqlFunction(
                                    "coalesce",
                                    NHibernateUtil.Date,
                                    dataUltimoContato,
                                    dataStatus,
                                    dataVenda
                                );

                            criteria.Add(Expression.Le(coalesce, data));
                        }


                        vendas = new List<Venda>(await repository.selectCriteria(criteria.ToArray())).ToList();
                    }


                    using (Conexao<Cliente> repotory = new Conexao<Cliente>(session))
                    {
                        clientes = (await repotory.selectCriteria(Expression.In(nameof(Cliente.Id), vendas.Select(s => s.Id_Cliente).ToArray()))).ToList();
                    }

                }
                retorno.Ok = true;
                retorno.Objeto = new List<object>(vendas.Select(p => new
                {
                    idPendencia = p.Id,
                    nome = clientes.Where(z => z.Id == p.Id_Cliente).FirstOrDefault()?.NomeFantasia ?? null,
                    cpf = clientes.Where(w => w.Id == p.Id_Cliente).FirstOrDefault()?.CpfCnpj ?? null,
                    num_telefone = clientes.Where(x => x.Id == p.Id_Cliente).FirstOrDefault()?.getTelefone(),
                    data_ultimo_contato = p.Data_Ultimo_Contato?.ToShortDateString() ??
                                         (statusDaVenda == Status_Venda.Analise_Cliente ? p.DataHoraCriacao?.ToShortDateString()
                                                                                        : p.Data_Alteracao_Status?.ToShortDateString()
                                         ) ?? "",
                    max_dias_sem_contato = maxDias,
                    dataVenda = p.DataHoraCriacao
                }));

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Vendaapi/getPendenciasPorta")]
        public async Task<GetApiRetorno<List<object>>> getPendenciasPorta(Int64 idVenda, Int64 idVendedor, Int64 idCliente, String status, String dataEtapa)
        {
            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();
            try
            {
                var dataStatus = dataEtapa;
                int maxDias = 0;
                DateTime data = DateTime.Now.AddDays(-1);

                Status_Venda statusDaVenda = (Status_Venda)Enum.Parse(typeof(Status_Venda), status.ToString());

                using (ISession session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IPortaAnielRepository repoPorta = new PortaAnielRepository(sessionT5);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);

                    var portaBusiness = new AlteracaoPortaAnielBusiness(repoPorta);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);


                    Elemento_Rede_Porta elemento = portaBusiness.GetPortabyCliente(idCliente);

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                    if (statusDaVenda != Status_Venda.Venda_Aprovada && statusDaVenda != Status_Venda.Venda_Reprovada)
                        maxDias = config.Max_Dias_Sem_Aprovacao ?? 0;


                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        List<ICriterion> criteria = new List<ICriterion>();

                        if (dataStatus != "")
                        {
                            TimeSpan diferenca = Convert.ToDateTime(data) - Convert.ToDateTime(dataStatus);
                            int diferencaEmDias = (int)diferenca.TotalDays;

                            if (diferencaEmDias >= maxDias && (statusDaVenda != Status_Venda.Venda_Aprovada && statusDaVenda != Status_Venda.Venda_Cancelada))
                            {
                                if (elemento.Status == Situacao_Porta.Reservado)
                                    portaBusiness.AlteraStatusPortaReservada(elemento);

                            }
                        }

                    }

                }
                retorno.Ok = true;



            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex);
            }

            return retorno;
        }
        #region mobile Consulta vendas aberto ou cancelada pelo nome ou cpf.

        [HttpGet()]
        [Route("api/Vendaapi/buscaPorCliente")]
        public async Task<GetApiRetorno<List<object>>> buscaPorCliente(string listaStatus, DateTime? inicio, DateTime? fim, string cpf_cnpj, string likeNome, Int64? idVenda)
        {

            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());

                    int? maxDiasAvaliacaoCliente = config.Max_Dias_Sem_Contato;
                    int? maxDiasAvaliacaoComercial = config.Qtde_Dias_Avaliacao_Comercial;

                    List<Cliente> clientes = null;
                    if ((!string.IsNullOrEmpty(cpf_cnpj)) || (!string.IsNullOrEmpty(likeNome)))
                    {
                        using (Conexao<Cliente> repository = new Conexao<Cliente>(session))
                        {
                            List<ICriterion> criterion = new List<ICriterion>();
                            if ((!string.IsNullOrEmpty(cpf_cnpj)))
                            {
                                criterion.Add(Expression.Where<Cliente>(w => w.CpfCnpj == cpf_cnpj));
                            }

                            if ((!string.IsNullOrEmpty(likeNome)))
                            {
                                criterion.Add(Expression.InsensitiveLike(nameof(Cliente.NomeFantasia), likeNome, MatchMode.Anywhere));
                            }

                            clientes = (await repository.selectCriteria(criterion.ToArray<ICriterion>())).ToList();
                        }
                    }

                    List<Venda> vendas = null;
                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        List<ICriterion> criterion = new List<ICriterion>();

                        if (idVenda.HasValue)
                        {
                            criterion.Add(Expression.Eq(nameof(Venda.Id), idVenda.Value));
                        }
                        else
                        {
                            if (clientes != null)
                            {
                                if (clientes.Count == 0)
                                {
                                    throw new Exception(Resources.RsBase.CONSULTA_NAO_RETORNOU_DADOS);
                                }

                                criterion.Add(Expression.In(nameof(Venda.Id_Cliente), clientes.Select(s => s.Id).ToArray()));
                            }

                            Status_Venda[] status = listaStatus.Split(',').Select(s => (Status_Venda)Enum.Parse(typeof(Status_Venda), s)).ToArray();
                            criterion.Add(Expression.In(nameof(Venda.Status), status));

                            if (inicio != null)
                            {
                                IProjection dataProjection = Projections.SqlFunction("date", NHibernateUtil.Date, Projections.Property(nameof(Venda.DataHoraCriacao)));
                                criterion.Add(Expression.Between(dataProjection, inicio, fim));
                            }
                        }

                        vendas = (await repository.selectCriteria(criterion.ToArray<ICriterion>())).ToList();
                    }

                    if ((vendas.Count > 0) && (clientes == null))
                    {
                        using (Conexao<Cliente> repository = new Conexao<Cliente>(session))
                        {
                            clientes = (await repository.selectCriteria(Expression.In(nameof(Cliente.Id), vendas.Select(v => v.Id_Cliente).ToArray()))).ToList();
                        }
                    }



                    retorno.Ok = true;
                    retorno.Objeto = new List<object>(vendas.Select(s => new
                    {
                        id = s.Id,
                        status = s.Status,
                        data_Venda = s.Data_Venda,
                        data_Criacao = s.DataHoraCriacao,
                        data_ultimo_contato = s.Data_Ultimo_Contato,
                        max_dias_ultimo_contato = s.Status == Status_Venda.Aguardando_Avaliacao_Comercial ? maxDiasAvaliacaoComercial : s.Status == Status_Venda.Analise_Cliente ? maxDiasAvaliacaoCliente : null,

                        idCliente = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.Id,
                        cpf_cnpj = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.CpfCnpj,
                        nome = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.NomeFantasia,

                        bairro = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.Endereco_Principal?.Bairro,
                        cidade = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.Endereco_Principal?.Cidade,

                        telefone_fixo = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.Telefone_Fixo,
                        telefone_celular = clientes.Where(w => w.Id == s.Id_Cliente).FirstOrDefault()?.Telefone_Celular,

                        idSupervisor = s.Supervisor?.Id,
                        nomeSupervisor = s.Supervisor?.Nome,

                        idVendedor = s.Vendedor?.Id,
                        nomeVendedor = s.Vendedor?.Nome,

                        idForma_Pagamento_Cliente_Preferida = s.Id_Forma_Pagamento_Cliente_Preferida,

                        exige_Termo_Adesao = config.Exige_Termo_Adesao,
                        Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato,
                        exige_Termo_Contrato = config.Exige_Termo_Contrato,
                        termo_Adesao_Assinado = s.Termo_Adesao_Assinado,
                        consultaViabilidadeRealizada = s.Consulta_Viabilidade_Realizada,
                        exigeAgendamentoServico = config.Exige_Agendamento_Servico,

                        id_workflow_Vendas = s.Workflow_Vendas == null ? null : s.Workflow_Vendas.Id.ToString(),
                        id_workflow_VendaAtividade = s.Workflow_VendasAtividade == null ? null : s.Workflow_VendasAtividade.Id.ToString(),

                        prazo_Atividade = s.Workflow_VendasAtividade?.Ativ_Prazo_Minutos,
                        decorrido_Atividade = s.Data_Alteracao_Status.HasValue ? (DateTime.Now - s.Data_Alteracao_Status.Value).ToString(@"hh\:mm") : (s.DataHoraCriacao.HasValue ? (DateTime.Now - s.DataHoraCriacao.Value).ToString(@"hh\:mm") : null),
                        decorrido_Workflow_Vendas = s.DataHoraCriacao.HasValue ? (DateTime.Now - s.DataHoraCriacao.Value).ToString(@"hh\:mm") : null,
                        prazo_Workflow_Vendas = s.Workflow_Vendas?.Prazo,

                        utiliza_certificacao_assinatura = config.Utiliza_Certificacao_Assinatura,
                        tipo_integracao_assinatura = config.Tipo_Integracao_Assinatura,
                        url_assinar_documento = s.Url_Assinar_Documento
                    }));
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;

        }

        [HttpGet()]
        [Route("api/Vendaapi/aprovReprovVenda")]
        public async Task<GetApiRetorno<object>> aprovReprovVenda(Int64 idVenda, Boolean isAprovado)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();
            try
            {
                using (VendaApiController apiVenda = new VendaApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
                {
                    Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());
                    //Int16 statusVenda = isAprovado
                    //                    //? (config.Exige_Avaliacao_Comercial
                    //                       ? (Int16)Status_Venda.Aguardando_Avaliacao_Comercial
                    //                       : (Int16)Status_Venda.Venda_Aprovada)
                    //                    : (Int16)Status_Venda.Venda_Cancelada;

                    await apiVenda.AlterarStatusPrivate(new VendaAlterarStatusViewModel()
                    {
                        id = idVenda
                    });

                    await new ClienteApiController().notificaSupervisor(idVenda);
                }

                GetApiRetorno<List<object>> busca = await buscaPorCliente(null, null, null, null, null, idVenda);
                retorno = new GetApiRetorno<object>
                {
                    Ok = busca.Ok,
                    Objeto = busca.Objeto.FirstOrDefault(),
                    Mensagem = busca.Mensagem,
                    InnerException = busca.InnerException
                };
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }


            return retorno;
        }

        public T DeepClone<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        #endregion
        #region MOBILE Extrato vendas periodo 
        [HttpGet()]
        [Route("api/Vendaapi/exibirExtratoVendasPeriodo")]
        public async Task<GetApiRetorno<List<ExtratoVendas>>> filtro(Int64 id_Vendedor, String e_Supervisor, String data_Ini, String data_Fim, Int64? vendedor_Venda, Int64? id_Campanha)
        {
            GetApiRetorno<List<ExtratoVendas>> retorno = new GetApiRetorno<List<ExtratoVendas>>
            {
                Objeto = new List<ExtratoVendas>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        List<string> filtros = new List<string>();

                        #region Filtros

                        filtros.Add("V.STATUS = 6");
                        filtros.Add("V.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM");

                        parametros.Add("@DATAINI", DateTime.Parse(data_Ini, new CultureInfo("pt-BR")).ToString("yyyy-MM-dd"));
                        parametros.Add("@DATAFIM", DateTime.Parse(data_Fim, new CultureInfo("pt-BR")).ToString("yyyy-MM-dd 23:59:00"));

                        if (e_Supervisor == "S")
                        {
                            filtros.Add("V.ID_SUPERVISOR = @ID_VENDEDOR");
                        }
                        else
                        {
                            filtros.Add("V.ID_VENDEDOR = @ID_VENDEDOR");
                        }

                        parametros.Add("@ID_VENDEDOR", id_Vendedor);

                        if (id_Campanha != null)
                        {
                            filtros.Add("C.ID = @IDCAMPANHA");
                            parametros.Add("@IDCAMPANHA", id_Campanha);
                        }

                        if (vendedor_Venda != null)
                        {
                            filtros.Add("V.ID_VENDEDOR = @VENDEDOR_VENDA");
                            parametros.Add("@VENDEDOR_VENDA", vendedor_Venda);
                        }

                        #endregion

                        sql.AppendLine("SELECT ID_VENDA, PLANOPRODUTO, CPF, NOMEFANTASIA, DATA_VENDA, TITULO, NOME, PONTOS");
                        sql.AppendLine("FROM(");

                        sql.AppendLine("    SELECT V.ID AS ID_VENDA, COALESCE(PLANO.DESCRICAO, PROD.DESCRICAO) AS PLANOPRODUTO,");
                        sql.AppendLine("           B.CPF, B.NOMEFANTASIA, V.DATA_VENDA, C.TITULO, F.NOME, E.QUANTIDADE AS PONTOS");

                        sql.AppendLine("    FROM TB_VENDA V");

                        sql.AppendLine("    INNER JOIN TB_CLIENTE_PLANO_PRODUTO A");
                        sql.AppendLine("    ON(A.ID_VENDA = V.ID AND A.ID_COMBO IS NULL)");

                        sql.AppendLine("    LEFT JOIN TB_PLANO PLANO");
                        sql.AppendLine("    ON(PLANO.ID = A.ID_PLANO)");

                        sql.AppendLine("    LEFT JOIN TB_PRODUTO PROD");
                        sql.AppendLine("    ON(PROD.ID = A.ID_PRODUTO)");

                        sql.AppendLine("    LEFT JOIN TB_FUNCIONARIO F");
                        sql.AppendLine("    ON(V.ID_VENDEDOR = F.ID)");

                        sql.AppendLine("    LEFT JOIN TB_CLIENTE B");
                        sql.AppendLine("    ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("    LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("    ON((E.ID_PLANO = A.ID_PLANO OR E.ID_PRODUTO = A.ID_PRODUTO)");
                        sql.AppendLine("        AND V.ID_COMISSIONAMENTO = E.ID_COMISSIONAMENTO)");

                        sql.AppendLine("    LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("    ON(A.ID_CAMPANHA = C.ID)");

                        if (filtros.Count > 0)
                        {
                            sql.AppendLine($"    WHERE {String.Join(" AND ", filtros)}");
                        }

                        sql.AppendLine("    UNION");

                        sql.AppendLine("    SELECT V.ID AS ID_VENDA, COMBO.DESCRICAO AS PLANOPRODUTO, B.CPF, B.NOMEFANTASIA,");
                        sql.AppendLine("           V.DATA_VENDA, C.TITULO, F.NOME, SUM(E.QUANTIDADE) AS PONTOS");

                        sql.AppendLine("    FROM TB_VENDA V");

                        sql.AppendLine("    INNER JOIN TB_CLIENTE_PLANO_PRODUTO A");
                        sql.AppendLine("    ON(A.ID_VENDA = V.ID AND A.ID_COMBO IS NOT NULL)");

                        sql.AppendLine("    INNER JOIN TB_COMBO COMBO");
                        sql.AppendLine("    ON(COMBO.ID = A.ID_COMBO)");

                        sql.AppendLine("    INNER JOIN TB_COMBO_PLANO_PRODUTO COMBO_PP");
                        sql.AppendLine("    ON(COMBO_PP.ID_COMBO = COMBO.ID)");

                        sql.AppendLine("    LEFT JOIN TB_PLANO PLANO");
                        sql.AppendLine("    ON(PLANO.ID = COMBO_PP.ID_PLANO)");

                        sql.AppendLine("    LEFT JOIN TB_PRODUTO PROD");
                        sql.AppendLine("    ON(PROD.ID = COMBO_PP.ID_PRODUTO)");

                        sql.AppendLine("    LEFT JOIN TB_FUNCIONARIO F");
                        sql.AppendLine("    ON(V.ID_VENDEDOR = F.ID)");

                        sql.AppendLine("    LEFT JOIN TB_CLIENTE B");
                        sql.AppendLine("    ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("    LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("    ON((E.ID_PLANO = COMBO_PP.ID_PLANO OR E.ID_PRODUTO = COMBO_PP.ID_PRODUTO)");
                        sql.AppendLine("    AND V.ID_COMISSIONAMENTO = E.ID_COMISSIONAMENTO)");

                        sql.AppendLine("    LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("    ON(A.ID_CAMPANHA = C.ID)");

                        if (filtros.Count > 0)
                        {
                            sql.AppendLine($"    WHERE {String.Join(" AND ", filtros)}");
                        }

                        sql.AppendLine("    GROUP BY V.ID, COMBO.DESCRICAO, B.CPF, B.NOMEFANTASIA,");
                        sql.AppendLine("             V.DATA_VENDA, C.TITULO, F.NOME");

                        sql.AppendLine(")");

                        sql.AppendLine("ORDER BY DATA_VENDA");


                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ExtratoVendas
                                {
                                    IdVenda = Convert.ToInt64(reader["ID_VENDA"]),
                                    PlanoProduto = Convert.ToString(reader["PLANOPRODUTO"]),
                                    Titulo = reader["TITULO"] != DBNull.Value ? Convert.ToString(reader["TITULO"]) : null,
                                    NomeCliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Cpf = Convert.ToString(reader["CPF"]),
                                    DataVenda = Convert.ToDateTime(reader["DATA_VENDA"]),
                                    Pontos = reader["PONTOS"] != DBNull.Value ? Convert.ToInt64(reader["PONTOS"]) : (Int64?)null,
                                    Vendedor = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : null
                                });
                            }

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }
        #endregion

        #region MOBILE Extrado mensal vendas realizadas

        [HttpGet()]
        [Route("api/Vendaapi/exibirExtratoVendasMensal")]
        public async Task<GetApiRetorno<List<ExtratoVendas>>> extratoVendasMensal(Int64 id_Vendedor, String data_Ini, String data_Fim)
        {
            GetApiRetorno<List<ExtratoVendas>> retorno = new GetApiRetorno<List<ExtratoVendas>>
            {
                Objeto = new List<ExtratoVendas>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT B.CPF, B.NOMEFANTASIA, A.DATA_VENDA, C.TITULO, SUM(E.QUANTIDADE) AS PONTOS");
                        sql.AppendLine("FROM TB_VENDA A");

                        sql.AppendLine("INNER JOIN TB_CLIENTE B");
                        sql.AppendLine("ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO D");
                        sql.AppendLine("ON(A.ID = D.ID_VENDA AND D.ID_COMBO IS NULL)");

                        sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("ON(E.ID_COMISSIONAMENTO = A.ID_COMISSIONAMENTO");
                        sql.AppendLine("   AND(E.ID_PLANO = D.ID_PLANO OR E.ID_PRODUTO = D.ID_PRODUTO))");

                        sql.AppendLine("LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("ON(D.ID_CAMPANHA = C.ID)");

                        sql.AppendLine("WHERE A.STATUS = 6");
                        sql.AppendLine("  AND A.ID_VENDEDOR = @ID_VENDEDOR");
                        sql.AppendLine("  AND A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM");

                        sql.AppendLine("GROUP BY C.TITULO, B.NOMEFANTASIA, B.CPF, A.STATUS, A.DATA_VENDA");

                        sql.AppendLine("UNION");

                        sql.AppendLine("SELECT B.CPF, B.NOMEFANTASIA, A.DATA_VENDA, C.TITULO, SUM(E.QUANTIDADE) AS PONTOS");
                        sql.AppendLine("FROM TB_VENDA A");

                        sql.AppendLine("INNER JOIN TB_CLIENTE B");
                        sql.AppendLine("ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO D");
                        sql.AppendLine("ON(A.ID = D.ID_VENDA AND D.ID_COMBO IS NOT NULL)");

                        sql.AppendLine("INNER JOIN TB_COMBO_PLANO_PRODUTO F");
                        sql.AppendLine("ON(F.ID_COMBO = D.ID_COMBO)");

                        sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("ON(E.ID_COMISSIONAMENTO = A.ID_COMISSIONAMENTO");
                        sql.AppendLine("   AND(E.ID_PLANO = F.ID_PLANO OR E.ID_PRODUTO = F.ID_PRODUTO))");

                        sql.AppendLine("LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("ON(D.ID_CAMPANHA = C.ID)");

                        sql.AppendLine("WHERE A.STATUS = 6");
                        sql.AppendLine("  AND A.ID_VENDEDOR = @ID_VENDEDOR");
                        sql.AppendLine("  AND A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM");

                        sql.AppendLine("GROUP BY C.TITULO, B.NOMEFANTASIA, B.CPF, A.STATUS, A.DATA_VENDA");

                        parametros.Add("@ID_VENDEDOR", id_Vendedor);

                        parametros.Add("@DATAINI", Convert.ToDateTime(data_Ini).ToString("dd.MM.yyyy"));
                        parametros.Add("@DATAFIM", Convert.ToDateTime(data_Fim).ToString("dd.MM.yyyy"));

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ExtratoVendas
                                {
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    NomeCliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Cpf = Convert.ToString(reader["CPF"]),
                                    DataVenda = Convert.ToDateTime(reader["DATA_VENDA"]),
                                    Pontos = Convert.ToInt64(reader["PONTOS"])
                                });
                            }

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Vendaapi/GetContrato")]
        public async Task<GetApiRetorno<List<EnviarContrato>>> GetContrato(Int64 Id_Cliente, Int64? Id_Venda, Int64? Id_Cliente_Contrato)
        {
            using (var session = HibernateUtil.GetSession())
            {
                return await GetContratoGerarPdf(session, Id_Cliente, Id_Venda, Id_Cliente_Contrato);
            }
        }

        public async Task<GetApiRetorno<List<EnviarContrato>>> GetContratoGerarPdf(ISession session, Int64 Id_Cliente, Int64? Id_Venda, Int64? Id_Cliente_Contrato)
        {
            GetApiRetorno<List<EnviarContrato>> retorno = new GetApiRetorno<List<EnviarContrato>>();

            retorno.Objeto = new List<EnviarContrato>();

            try
            {
                using (Conexao<Cliente_Anexo> repoClienteAnexo = new Conexao<Cliente_Anexo>(session))
                using (Conexao<ClienteContrato> repoClienteContrato = new Conexao<ClienteContrato>(session))
                using (Conexao<Cliente_PlanoProduto> repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var apiVenda = new VendaApiController())
                {
                    IList<Cliente_PlanoProduto> listPlanoProduto;

                    if (Id_Venda.HasValue)
                    {
                        listPlanoProduto = await repoPlanoProduto.select(u => u.Cliente.Id == Id_Cliente &&
                                                                              u.Id_Venda == Id_Venda);
                    }
                    else
                    {
                        listPlanoProduto = await repoPlanoProduto.select(u => u.Cliente.Id == Id_Cliente &&
                                                                              u.Id_Venda == null);
                    }

                    if (Id_Cliente_Contrato.HasValue)
                    {
                        listPlanoProduto = listPlanoProduto.Where(x => x.ClienteContrato.Id == Id_Cliente_Contrato).ToList();
                    }

                    long countAnexos = await repoClienteAnexo.Max(p => p.Id);

                    List<ClienteContrato> listaContratos = new List<ClienteContrato>();

                    foreach (Cliente_PlanoProduto planoProduto in listPlanoProduto)
                    {
                        ClienteContrato contrato = planoProduto.ClienteContrato;

                        string tmpPath = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Tmp";

                        if (!Directory.Exists(tmpPath))
                        {
                            Directory.CreateDirectory(tmpPath);
                        }

                        string tmpContratoDocxPath = $@"{tmpPath}\{Guid.NewGuid()}.pdf";
                        string tmpContratoPdfPath = $@"{tmpPath}\{Guid.NewGuid()}.pdf";
                        string tmpCaminhoContratoPdf = "";
                        string caminho_pdf_local = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{contrato.Cliente.Id}\Contrato-{contrato.NumeroContrato}.pdf";
                        if (contrato.Arquivo != null)
                        {
                            if (contrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado && !contrato.AdicionadoAceiteDigital)
                            {
                                tmpCaminhoContratoPdf = await AdicionarAceiteDigitalContrato(repoClienteContrato, apiVenda, contrato, tmpPath);
                            }
                            else if (contrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado && contrato.AdicionadoAceiteDigital)
                            {
                                string caminho_docx_local = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{contrato.Cliente.Id}\Contrato-{contrato.NumeroContrato}.docx";

                                if (ConvertToPdfService.docContemRegistroDigital(caminho_docx_local))
                                    tmpCaminhoContratoPdf = caminho_pdf_local;
                                else
                                    tmpCaminhoContratoPdf = await AdicionarAceiteDigitalContrato(repoClienteContrato, apiVenda, contrato, tmpPath);
                            }
                            else
                            {
                                System.IO.File.WriteAllBytes(tmpContratoDocxPath, contrato.Arquivo);
                                System.IO.File.WriteAllBytes(tmpContratoPdfPath, contrato.Arquivo);
                            }
                        }
                        else
                        {
                            using (var apiCliente = new ClienteApiController())
                            {
                                var objTmp = new ClienteDocContratoModel
                                {
                                    IdCliente = Id_Cliente,
                                    IdModeloSelecionado = Convert.ToString(contrato?.ModeloContrato?.Id),
                                    NumeroContrato = contrato?.NumeroContrato,
                                    IdCliContrato = Convert.ToInt64(contrato?.Id)
                                };

                                await apiCliente.GerarDocumentoContratoBusiness(session, objTmp);

                                contrato = await repoClienteContrato.selectFirst(x => x.Id == contrato.Id);
                            }

                            if (contrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado)
                            {
                                tmpCaminhoContratoPdf = await AdicionarAceiteDigitalContrato(repoClienteContrato, apiVenda, contrato, tmpPath);
                            }
                            else
                            {
                                System.IO.File.WriteAllBytes(tmpContratoDocxPath, contrato.Arquivo);
                                System.IO.File.WriteAllBytes(tmpContratoPdfPath, contrato.Arquivo);
                            }
                        }

                        if (Id_Cliente_Contrato.HasValue)
                        {
                            var indexOf = tmpContratoDocxPath.IndexOf("\\Arquivos");
                            var caminho_antigo = tmpContratoDocxPath.Substring(0, indexOf);
                            tmpContratoDocxPath = tmpContratoDocxPath.Replace(caminho_antigo, $@"{CONSTANTES.URLPortal}");
                        }

                        if (!String.IsNullOrEmpty(tmpCaminhoContratoPdf))
                        {
                            var indexOf = tmpCaminhoContratoPdf.IndexOf("\\Arquivos");
                            var caminho_antigo = tmpCaminhoContratoPdf.Substring(0, indexOf);
                            tmpCaminhoContratoPdf = tmpCaminhoContratoPdf.Replace(caminho_antigo, $@"{CONSTANTES.URLPortal}");
                        }

                        retorno.Objeto.Add(new EnviarContrato
                        {
                            Caminho_Contrato = tmpContratoDocxPath,
                            Nome_Contrato = contrato.Descricao,
                            Caminho_Contrato_Pasta_Cliente_pdf = !String.IsNullOrEmpty(tmpCaminhoContratoPdf) ? tmpCaminhoContratoPdf : tmpContratoDocxPath
                        });
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private static async Task<String> AdicionarAceiteDigitalContrato(Conexao<ClienteContrato> repoClienteContrato, VendaApiController vendaApi, ClienteContrato contrato, string tmpPath)
        {
            string caminho_docx_local = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{contrato.Cliente.Id}\Contrato-{contrato.NumeroContrato}.docx";
            string caminho_html_local = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{contrato.Cliente.Id}\Contrato-{contrato.NumeroContrato}.html";
            string caminho_pdf_local = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{contrato.Cliente.Id}\Contrato-{contrato.NumeroContrato}.pdf";

            var infosAceite = new InformacoesAceite
            {
                EmailDestinatario = contrato.Cliente.Email_Principal,
                IpOrigem = contrato.IpOrigemAceite,
                PortaOrigem = contrato.PortaOrigemAceite,
                UrlAceite = contrato.UrlAceiteContrato, //forma que é obtido no front, é melhor salvar em um campo posteriormente
                HashIdentificador = contrato.CodigoHashContratoAceite,
                UserAgent = contrato.UserAgentContratoAceite,
                DataHoraAceite = contrato.DataAceiteContrato.Value.ToString("dd/MM/yyyy 'às' HH:mm:ss '(UTC-3)'")
            };

            var aceiteDigital = await vendaApi.RegistroAceiteDigital(infosAceite);
            //substituindo arquivos antigos para incluir informações importantes do aceite registrado do cliente/usuario
            ConvertToPdfService.incluirAceiteDigitalDocx(caminho_docx_local, aceiteDigital);
            ConvertToPdfService.convertDocxToHtml(caminho_html_local, caminho_docx_local);
            ConvertToPdfService.convertHtmlToPdf(caminho_pdf_local, caminho_html_local);

            contrato.AdicionadoAceiteDigital = true;
            contrato.Arquivo = System.IO.File.ReadAllBytes(caminho_pdf_local);
            await repoClienteContrato.SaveOrUpdate(contrato);

            return caminho_pdf_local;
        }

        private void CarregaCamposWord(DocX document, Cliente_PlanoProduto planoProduto)
        {
            document.ReplaceText("(NOME_FANTASIA)", planoProduto.Cliente.NomeFantasia);
            document.ReplaceText("(NUM_CONTRATO)", planoProduto.ClienteContrato?.NumeroContrato);
            document.ReplaceText("(PLANO)", planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao);
            document.ReplaceText("(VAL_PLANO)", Convert.ToString(planoProduto.Plano != null ? planoProduto.Plano.ValorPlanoFinal : planoProduto.Produto.ValorProdutoFinal));
            document.ReplaceText("(DETALHES_PLANO)", planoProduto.Plano != null ? planoProduto.Plano.DetalhesPlano : planoProduto.Produto.Detalhes);
        }

        private async Task RegistraClienteStatusLog(List<Cliente_PlanoProduto> planos_produtos, Conexao<Cliente_PlanoProduto_StatusLog> repository)
        {
            List<Cliente_PlanoProduto_StatusLog> logs = new List<Cliente_PlanoProduto_StatusLog>();
            foreach (Cliente_PlanoProduto planoproduto in planos_produtos)
            {
                Status_PlanoProduto? statusAntigo = (await repository.select(p => p.PlanoProduto.Id == planoproduto.Id))
                                                                     .OrderByDescending(p => p.DataHoraAcao).FirstOrDefault()?.NovoStatus;

                logs.Add(new Cliente_PlanoProduto_StatusLog
                {
                    AntigoStatus = statusAntigo,
                    NovoStatus = planoproduto.Status,
                    PlanoProduto = planoproduto,
                    ClienteContrato = planoproduto.ClienteContrato,
                    DataHoraAcao = DateTime.Now
                });
            }

            await repository.SaveOrUpdate(logs);

        }

        public GetApiRetorno<Json_VendaTelefonia> Recurso_TelefoniaDynamic(string JsonTelefonia)
        {
            GetApiRetorno<Json_VendaTelefonia> retorno = new GetApiRetorno<Json_VendaTelefonia>();

            {
                Json_VendaTelefonia objeto = JsonConvert.DeserializeObject<Json_VendaTelefonia>(JsonTelefonia);
                retorno.Objeto = objeto;

            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Vendaapi/BuscaFiltroConsultaVendAtd")]
        public async Task<DataTableResponse<ConsultaClienteAtendimentoModel>> BuscaFiltroConsultaVendAtd(ConsultaClienteAtendimento_DataTableRequest request)
        {
            List<ConsultaClienteAtendimentoModel> retorno = new List<ConsultaClienteAtendimentoModel>();
            IOrderedEnumerable<ConsultaClienteAtendimentoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_VENDA A ");

                sqlFrom.AppendLine("LEFT JOIN TB_ATENDIMENTO  B ");
                sqlFrom.AppendLine("ON A.ID_CLIENTE  =B.ID_CLIENTE ");

                sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO C ");
                sqlFrom.AppendLine("ON C.ID = A.ID_VENDEDOR ");

                sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR D ");
                sqlFrom.AppendLine("ON D.ID_SUPERVISOR =A.ID ");

                sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE_VENDA E ");
                sqlFrom.AppendLine("ON E.ID = D.ID_EQUIPE_VENDA ");

                sqlFrom.AppendLine("WHERE A.STATUS <> 0 ");
                sqlFrom.AppendLine("AND A.ID_CLIENTE = @VALOR");
                parametros.Add("@VALOR", request.Id_Cliente);

                if (!String.IsNullOrEmpty(request.search.value))//ok
                {
                    sqlFrom.AppendLine(" AND ((A.ID LIKE @SEARCH) )) ");

                    if (!parametros.ContainsKey("@SEARCH"))
                    {
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));

                    }
                }

                if ((!String.IsNullOrEmpty(request.Dt_Inicial_Filtro)) && (!String.IsNullOrEmpty(request.Dt_Final_Filtro)))
                {
                    sqlFrom.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM) ");
                    parametros.Add("@DATAINI", DateTime.Parse(String.Format("{0} 00:00", request.Dt_Inicial_Filtro)));
                    parametros.Add("@DATAFIM", DateTime.Parse(String.Format("{0} 23:59", request.Dt_Final_Filtro)));
                }

                if (request.Status != null)
                {
                    if (request.Status.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.STATUSVENDA = @STATUS ) ");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status.Value));
                    }
                }



                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID,B.ID AS NATENDIMENTO,C.NOME ,E.DESCRICAO AS SUPERVISOR,A.STATUSVENDA ,A.DATAHORACRIACAO ");

                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ConsultaClienteAtendimentoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Numero = reader["NATENDIMENTO"] != DBNull.Value ? Convert.ToInt64(reader["NATENDIMENTO"]) : 0,
                                Vendedor = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"].ToString()) : "",
                                Supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUSVENDA"])),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : null,


                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ConsultaClienteAtendimentoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Vendaapi/BuscaFiltroConsultaAtdVend")]
        public async Task<DataTableResponse<ConsultaClienteAtendimentoModel>> BuscaFiltroConsultaAtdVend(ConsultaClienteAtendimento_DataTableRequest request)
        {
            List<ConsultaClienteAtendimentoModel> retorno = new List<ConsultaClienteAtendimentoModel>();
            IOrderedEnumerable<ConsultaClienteAtendimentoModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO A ");

                sqlFrom.AppendLine("LEFT JOIN TB_VINC_ATENDE_VENDA  B ");
                sqlFrom.AppendLine("ON A.NUMERO = B.ID_ATENDIMENTO ");

                sqlFrom.AppendLine("LEFT JOIN ASPNETUSERS C ");
                sqlFrom.AppendLine("ON C.ID = A.ID_RESPONSAVEL ");

                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO D ");
                sqlFrom.AppendLine("ON D.ID =A.ID_TIPO_ATENDIMENTO ");

                sqlFrom.AppendLine("WHERE A.STATUS <> 0 ");
                sqlFrom.AppendLine("AND A.ID_CLIENTE = @VALOR");
                parametros.Add("@VALOR", request.Id_Cliente);

                if (!String.IsNullOrEmpty(request.search.value))//ok
                {
                    sqlFrom.AppendLine(" AND ((A.NUMERO LIKE @SEARCH) )) ");

                    if (!parametros.ContainsKey("@SEARCH"))
                    {
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));

                    }
                }

                if ((!String.IsNullOrEmpty(request.Dt_Inicial_Filtro)) && (!String.IsNullOrEmpty(request.Dt_Final_Filtro)))
                {
                    sqlFrom.AppendLine(" AND (A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM) ");
                    parametros.Add("@DATAINI", DateTime.Parse(String.Format("{0} 00:00", request.Dt_Inicial_Filtro)));
                    parametros.Add("@DATAFIM", DateTime.Parse(String.Format("{0} 23:59", request.Dt_Final_Filtro)));
                }

                if (request.Status != null)
                {
                    if (request.Status.HasValue)
                    {
                        sqlFrom.AppendLine(" AND ( A.STATUS_ATENDIMENTO = @STATUS ) ");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status.Value));
                    }
                }



                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.NUMERO ,B.ID_VENDA, C.NOME  AS RESPONSAVEL,D.NOME  AS TIPO_ATENDIMENTO,A.STATUS_ATENDIMENTO ,A.DATAHORACRIACAO ");

                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new ConsultaClienteAtendimentoModel
                            {
                                Id = Convert.ToInt64(reader["NUMERO"]),
                                Venda = reader["ID_VENDA"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENDA"]) : 0,
                                Responsavel = reader["RESPONSAVEL"] != DBNull.Value ? Convert.ToString(reader["RESPONSAVEL"]) : "",
                                Tipo_Atendimento = reader["TIPO_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["TIPO_ATENDIMENTO"]) : "",
                                Status = HtmlEnumExtensions.GetEnumDescription<StatusAtendimento>(Convert.ToString(reader["STATUS_ATENDIMENTO"])),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : null,


                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<ConsultaClienteAtendimentoModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Vendaapi/GetConsultaCpf")]
        public async Task<GetApiRetorno<long?>> ConsultaSerasaCpf(Consulta_SerasaCpfRequestModels request)
        {
            var retorno = new GetApiRetorno<long?>();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (Conexao<IntegracoesExternas> repoIntegracoes = new Conexao<IntegracoesExternas>(sessionNh))
                using (var repositoryCliente = new Conexao<Cliente>(sessionNh))
                using (var repoIdentity = new Conexao<IdentityUser>(sessionNh))
                using (var repositoryVenda = new Conexao<Venda>(sessionNh))
                using (var repositoryEtapa = new Conexao<EtapaVenda>(sessionNh))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    Venda venda = null;
                    TrocaTitularidade trocaOld = null;
                    EtapaVenda etapaOld = null;

                    IntegracoesExternas configIntegracaoSerasaPreQual = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.PREQUAL).Result;

                    IntegracoesExternas configIntegracaoSerasa = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA).Result;

                    IntegracoesExternas configIntegracaoSerasaExperian = repoIntegracoes.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;

                    /// <summary>
                    ///      CPF ou CNPJ do cliente
                    /// </summary>
                    string documento = request.cpf.Replace("-", String.Empty)
                                                  .Replace(".", String.Empty)
                                                  .Replace("/", String.Empty);

                    bool pessoaFisica = documento.Length == 11;

                    //INTEGRAÇÃO SERASA
                    if (configIntegracaoSerasa != null)
                    {
                        ConsultaSerasaDados consultaSerasa = null;

                        string usuario = configIntegracaoSerasa.Usuario;
                        string senha = configIntegracaoSerasa.Senha;
                        string url = configIntegracaoSerasa.UrlApi;

                        IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                        ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                        IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                        var consultaCreditoBusiness =
                                new ConsultaCreditoBusiness(
                                    repoConsulta,
                                    repoCriterio,
                                    repoParams,
                                    repoConfig,
                                    usuario,
                                    senha,
                                    url
                                );

                        if (pessoaFisica)
                        {
                            consultaSerasa = await consultaCreditoBusiness.ExecutarConsultaCreditoCpf(documento, User.Identity.GetUserName());
                        }
                        else
                        {
                            consultaSerasa = await consultaCreditoBusiness.ExecutarConsultaCreditoCnpj(documento, User.Identity.GetUserName());
                        }

                        if (consultaSerasa == null)
                        {
                            throw new Exception("Erro ao gravar os resultados da consulta de crédito");
                        }

                        if (request.IdVenda.HasValue)
                        {
                            IVendaRepository repoVenda = new VendaRepository(sessionNh);
                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                            IClienteRepository repoClient = new ClienteRepository(sessionNh);

                            var serasaResponse = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);
                            var vendaBusiness = new VendaBusiness(repoVenda);
                            venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                            var etapa = await repositoryEtapa.selectFirst(p => p.Etapa == EnumEtapaVenda.ConsultaCredito && p.Venda.Id == request.IdVenda);
                            etapaOld = (EtapaVenda)etapa.Clone();
                            if (serasaResponse.reports.FirstOrDefault() != null)
                                venda.ScoreConsultaCredito = serasaResponse.reports.FirstOrDefault().score.score;

                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoClient, venda.Id_Cliente.Value);

                            vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaCredito, consultaSerasa.Id);
                            retorno.Ok = true;
                            retorno.Objeto = consultaSerasa.Id;

                            var consultaCreditoBusinessSerasa = new ConsultaCreditoBusiness(
                                repoConsulta,
                                repoCriterio,
                                repoParams,
                                repoConfig
                            );

                            var statusConsulta = consultaCreditoBusinessSerasa.GetStatusConsultaCreditoRestricaoByCpf(documento);
                            etapaVendaBusiness.MudarStatusEtapa(venda.Id, EnumEtapaVenda.ConsultaCredito, statusConsulta, DateTime.Now);

                            //Log 
                            var etapa_venda = new EtapaVenda
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                Descricao = etapa.Descricao,
                                Etapa = etapa.Etapa,
                                Status = statusConsulta,
                                Venda = venda
                            };

                            var logs = etapa_venda.GeraLog(etapaOld, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                        else if (request.IdTroca.HasValue)
                        {
                            using (var repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
                            {
                                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                                IClienteRepository repoClient = new ClienteRepository(sessionNh);
                                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                                var troca = repoTroca.selectFirst(t => t.Id == request.IdTroca.Value).Result;
                                troca.IdConsultaCredito = consultaSerasa.Id;
                                await repoTroca.SaveOrUpdate(troca);

                                retorno.Ok = true;
                                retorno.Objeto = consultaSerasa.Id;

                                var consultaCreditoBusinessSerasa = new ConsultaCreditoBusiness(
                                    repoConsulta,
                                    repoCriterio,
                                    repoParams,
                                    repoConfig
                                );

                                var statusConsulta = consultaCreditoBusinessSerasa.GetStatusConsultaCreditoRestricaoByCpf(documento);
                                etapaVendaBusiness.MudarStatusEtapa(null, EnumEtapaVenda.ConsultaCredito, statusConsulta, null, null, request.IdTroca);
                            }
                        }

                        retorno.Ok = true;
                        retorno.Objeto = consultaSerasa.Id;
                    }
                    else if (configIntegracaoSerasaExperian != null)
                    {
                        ConsultaSerasaDados consultaSerasa = null;

                        string usuario = configIntegracaoSerasaExperian.Usuario;
                        string senha = configIntegracaoSerasaExperian.Senha;
                        string url = configIntegracaoSerasaExperian.UrlApi;

                        IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                        ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                        IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                        var consultaCreditoBusiness =
                                new ConsultaCreditoBusiness(
                                    repoConsulta,
                                    repoCriterio,
                                    repoParams,
                                    repoConfig,
                                    usuario,
                                    senha,
                                    url
                                );

                        var urlBaseToken = CONSTANTES.UrlBaseSerasaAuth();
                        var urlBase = CONSTANTES.UrlBaseSerasaExperian();

                        var authToken = await consultaCreditoBusiness.ObterTokenAuth(urlBaseToken);

                        if (string.IsNullOrEmpty(authToken))
                            throw new Exception("Não foi possível obter o token de autenticação da Serasa Experian.");

                        if (pessoaFisica)
                        {
                            consultaSerasa = await consultaCreditoBusiness.ConsultarSerasaPF(documento, authToken, urlBase);
                        }
                        else
                        {
                            consultaSerasa = await consultaCreditoBusiness.ConsultarSerasaPJ(documento, authToken, urlBase);
                        }

                        if (consultaSerasa == null)
                        {
                            throw new Exception("Erro ao gravar os resultados da consulta de crédito");
                        }

                        if (request.IdVenda.HasValue)
                        {
                            IVendaRepository repoVenda = new VendaRepository(sessionNh);
                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                            IClienteRepository repoClient = new ClienteRepository(sessionNh);
                            var serasaResponse = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);
                            var vendaBusiness = new VendaBusiness(repoVenda);
                            venda = vendaBusiness.GetVendaById(request.IdVenda.Value);

                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoClient, venda.Id_Cliente.Value);
                            if (serasaResponse.reports != null && serasaResponse.reports.FirstOrDefault() != null)
                                venda.ScoreConsultaCredito = serasaResponse.reports.FirstOrDefault().score.score;

                            vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaCredito, consultaSerasa.Id);
                            retorno.Ok = true;
                            retorno.Objeto = consultaSerasa.Id;

                            var statusConsulta = consultaCreditoBusiness.SerasaStatusRetorno(consultaSerasa.consulta, consultaSerasa.Id, documento, pessoaFisica);
                            etapaVendaBusiness.MudarStatusEtapa(venda.Id, EnumEtapaVenda.ConsultaCredito, ValidarStatusCredito(statusConsulta));
                        }
                        else if (request.IdTroca.HasValue)
                        {
                            using (var repoTroca = new Conexao<TrocaTitularidade>(sessionNh))
                            {
                                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                                IClienteRepository repoClient = new ClienteRepository(sessionNh);
                                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                                var troca = repoTroca.selectFirst(t => t.Id == request.IdTroca.Value).Result;
                                troca.IdConsultaCredito = consultaSerasa.Id;
                                await repoTroca.SaveOrUpdate(troca);

                                retorno.Ok = true;
                                retorno.Objeto = consultaSerasa.Id;

                                var statusConsulta = consultaCreditoBusiness.SerasaStatusRetorno(consultaSerasa.consulta, consultaSerasa.Id, documento, pessoaFisica);

                                etapaVendaBusiness.MudarStatusEtapa(troca.Id, EnumEtapaVenda.ConsultaCredito, ValidarStatusCredito(statusConsulta));
                            }
                        }

                        retorno.Ok = true;
                        retorno.Objeto = consultaSerasa.Id;
                    }

                    //INTEGRAÇÃO SERASA PREQUAL
                    else if (configIntegracaoSerasaPreQual != null)
                    {
                        ConsultaSerasaDados consultaSerasa = null;

                        string usuario = configIntegracaoSerasaPreQual.Usuario;
                        string senha = configIntegracaoSerasaPreQual.Senha;
                        string url = configIntegracaoSerasaPreQual.UrlApi;
                        string token = configIntegracaoSerasaPreQual.Token;
                        string apiKey = configIntegracaoSerasaPreQual.ApiKey;
                        string apiSecret = configIntegracaoSerasaPreQual.ApiSecret;
                        string domain = configIntegracaoSerasaPreQual.TokenAcesso;
                        string correlationId = configIntegracaoSerasaPreQual.HeaderIntegracaoExterna1;


                        IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                        ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                        IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                        IIntegracaoExternaRepository repoIntegracao = new IntegracaoExternaRepository(sessionNh);
                        IParametrosConsultaPreQualRepository repoParamsPrequal = new ParametrosConsultaPreQualRepository(sessionNh);
                        IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                        IVendaRepository repoVenda = new VendaRepository(sessionNh);

                        var ConsultaCreditoPreQualBusiness =
                                new ConsultaCreditoPreQualBusiness(
                                    repoConsulta,
                                    repoCriterio,
                                    repoParams,
                                    repoParamsPrequal,
                                    repoConfig,
                                    repoIntegracao,
                                    repoCliente,
                                    repoVenda,
                                    usuario,
                                    senha,
                                    url,
                                    apiKey,
                                    apiSecret,
                                    domain,
                                    correlationId
                                );

                        var identityUser = new IdentityUser();
                        var cliente = await repositoryCliente.selectFirst(p => p.CpfCnpj == documento);

                        if (request.IdVenda.HasValue)
                        {
                            venda = await repositoryVenda.selectFirst(p => p.Id == Convert.ToInt16(request.IdVenda));
                            identityUser = await repoIdentity.selectFirst(p => p.Funcionario.Id == Convert.ToInt64(venda.Vendedor.Id));
                        }
                        else
                        {
                            identityUser = await repoIdentity.selectFirst(p => p.Id == cliente.UsuarioCriacao);
                        }

                        if (pessoaFisica)
                        {
                            consultaSerasa = await ConsultaCreditoPreQualBusiness.ExecutarConsultaCreditoPF(documento, identityUser, request.IdVenda);
                        }
                        else
                        {
                            consultaSerasa = await ConsultaCreditoPreQualBusiness.ExecutarConsultaCreditoPJ(documento, identityUser, request.IdVenda);
                        }

                        if (consultaSerasa == null)
                        {
                            throw new Exception("Erro ao gravar os resultados da consulta de crédito");
                        }

                        if (request.IdVenda.HasValue)
                        {
                            var serasaResponse = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);
                            IVendaRepository repoVendaNH = new VendaRepository(sessionNh);
                            var vendaBusiness = new VendaBusiness(repoVendaNH);

                            venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                            var etapa = await repositoryEtapa.selectFirst(p => p.Etapa == EnumEtapaVenda.ConsultaCredito && p.Venda.Id == request.IdVenda);
                            etapaOld = (EtapaVenda)etapa.Clone();

                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                            IClienteRepository repoClient = new ClienteRepository(sessionNh);

                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoClient, venda.Id_Cliente.Value);
                            if (serasaResponse.reports.FirstOrDefault() != null)
                                venda.ScoreConsultaCredito = serasaResponse.reports.FirstOrDefault().score.score;
                            vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaCredito, consultaSerasa.Id);

                            retorno.Ok = true;
                            retorno.Objeto = consultaSerasa.Id;

                            var consultaCreditoBusinessSerasaPrequal =
                            new ConsultaCreditoBusiness(
                                repoConsulta,
                                repoCriterio,
                                repoParams,
                                repoConfig
                            );

                            var statusConsulta = pessoaFisica == true ?
                                consultaCreditoBusinessSerasaPrequal.GetStatusConsultaCreditoCpfByConsulta(consultaSerasa.Id, documento).valor :
                                consultaCreditoBusinessSerasaPrequal.GetStatusConsultaCreditoCnpjByConsulta(consultaSerasa.Id, documento).valor;

                            etapaVendaBusiness.MudarStatusEtapa(venda.Id, EnumEtapaVenda.ConsultaCredito, (statusConsulta == AprovacaoCriterioParametroCredito.APROVADO) ? StatusEtapaVenda.Aprovado : StatusEtapaVenda.Reprovado, DateTime.Now);

                            //Log 
                            var etapa_venda = new EtapaVenda
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                Descricao = etapa.Descricao,
                                Etapa = etapa.Etapa,
                                Status = (statusConsulta == AprovacaoCriterioParametroCredito.APROVADO) ? StatusEtapaVenda.Aprovado : StatusEtapaVenda.Reprovado,
                                Venda = venda
                            };

                            var logs = etapa_venda.GeraLog(etapaOld, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }
                    }
                    else
                    {
                        throw new Exception("Nenhuma integração com o Serasa/PreQual foi configurada");
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        public StatusEtapaVenda ValidarStatusCredito(AprovacaoCriterioParametroCredito aprovacao)
        {
            switch (aprovacao)
            {
                case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_COM_INSTALACAO:
                case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO:
                case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVADO:
                    {
                        return Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado;
                    }
                case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR:
                    {
                        return Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor;
                    }
                case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO:
                    {
                        return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                    }
                default:
                    {
                        return Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                    }
            }
        }


        #region [ConsultaPlanoEmVenda]
        [HttpGet]
        [Route("api/VendaApi/ConsultaPlanoEmVenda")]
        public async Task<PostApiRetorno<Int64>> ConsultaPlanoEmVenda(Int64 IdVenda)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var lista = await repo.select(p => p.Id_Venda == IdVenda);
                    if (lista.Count > 0)
                        retorno.Ok = true;
                    else
                        retorno.Ok = false;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }
        #endregion

        #region AprovarEtapaManual
        /// <summary>
        //  Aprovação feita manualmente na venda, independente se existir restrição é aprovado
        //  tarefa DS-843
        /// </summary>
        /// <param name="idVenda"></param>
        [HttpPost()]
        [Route("api/Vendaapi/AprovarEtapaManual")]
        public async Task<PostApiRetorno> AprovarEtapaManual(GravarMotivoAprovacaoVDManualRequest request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Venda venda = null;
                TrocaTitularidade troca = null;
                EtapaVenda etapa = null;

                using (var session = HibernateUtil.GetSession())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryEtapa = new Conexao<EtapaVenda>(session))
                using (var repositoryVenda = new Conexao<Venda>(session))
                using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                using (var repositoryMotivoAprovacaoResitricao = new Conexao<Motivo_Aprovacao_Resticao>(session))
                {
                    var motivo = await repositoryMotivoAprovacaoResitricao.selectFirst(p => p.Id == Convert.ToInt64(request.MotivoAprovacao));
                    if (request.IdVenda != 0)
                    {
                        etapa = await repositoryEtapa.selectFirst(p => p.Etapa == (EnumEtapaVenda)(Convert.ToInt64(request.Etapa)) && p.Venda.Id == request.IdVenda);
                        venda = await repositoryVenda.selectFirst(p => p.Id == request.IdVenda);
                    }
                    else
                    {
                        etapa = await repositoryEtapa.selectFirst(p => p.Etapa == (EnumEtapaVenda)(Convert.ToInt64(request.Etapa)) && p.TrocaTitularidade.Id == request.IdTroca);
                        troca = await repoTrocaTitular.selectFirst(p => p.Id == request.IdTroca);
                    }


                    if (etapa.Status != StatusEtapaVenda.Aprovado && etapa.Status != StatusEtapaVenda.AprovadoManualmente)
                    {
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                        IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);

                        //Log 
                        EtapaVenda etapaVenda = new EtapaVenda
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = User.Identity.GetUserId(),
                            Descricao = etapa.Descricao,
                            Etapa = etapa.Etapa,
                            Status = StatusEtapaVenda.AprovadoManualmente,
                        };

                        if (venda != null)
                            etapaVenda.Venda = venda;
                        else
                            etapaVenda.TrocaTitularidade = troca;

                        if (etapa != null)
                        {
                            var logs = etapaVenda.GeraLog(etapa, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            logs.ForEach(p => { p.Motivo = motivo.Descricao; });
                            await repositoryLogs.Save(logs);
                        }

                        etapa.MotivoAprovManual = motivo.Descricao;
                        etapa.TrocaTitularidade = troca;
                        etapa.Status = StatusEtapaVenda.AprovadoManualmente;

                        await repositoryEtapa.Update(etapa);
                    }
                    else
                    {
                        throw new Exception("Não é possível realizar a aprovação manual, pois a etapa já se encontra aprovada.");
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        //#region AprovarPendenciaEnderecoManual
        ///// <summary>
        ////  Aprovação feita manualmente na venda, independente se existir restrição é aprovado
        ////  tarefa DS-843
        ///// </summary>
        ///// <param name="idVenda"></param>
        //[HttpPost()]
        //[Route("api/Vendaapi/AprovarEtapaManual")]
        //public async void AprovarEtapaManual(Int32 idVenda, String etapa)
        //{
        //    ISession session = HibernateUtil.GetSession();
        //    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
        //    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
        //    new EtapaVendaBusiness(repoEtapaVenda, repoConfig).MudarStatusEtapa(idVenda, (EnumEtapaVenda)etapa, StatusEtapaVenda.AprovadoManualmente);

        //    //Log 
        //    EtapaVenda etapaVenda = new EtapaVenda
        //    {
        //        DataHoraCriacao = DateTime.Now,
        //        UsuarioCriacao = User.Identity.GetUserId(),
        //        Etapa = EnumEtapaVenda.ConsultaPendenciaFinanceira,
        //        Status = StatusEtapaVenda.AprovadoManualmente,
        //        Venda = venda
        //    };

        //    if (etapa_venda != null)
        //    {
        //        var logs = etapa_venda.GeraLog(etapaVenda, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
        //        await repositoryLogs.Save(logs);
        //    }
        //}
        //#endregion

        public async Task GravaHistoricoVenda(int idVenda)
        {
            try
            {
                //ISession session = HibernateUtil.GetSession();
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        //String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"INSERT INTO TB_VENDAS (ID, USUARIOCRIACAO, NOMEUSUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, OBJETO, CAMPO, ID_VENDA)VALUES(0, '', '', '', 0, 0, '', '', '', '', '' ");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost()]
        [Route("api/Vendaapi/VincularAtendimento")]
        public async Task<PostApiRetorno<Int64>> VincularAtendimento([FromBody] VendaDadosGeraisViewModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                VincularAtendimentoVenda modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<VincularAtendimentoVenda>(session))
                    {
                        if (request != null && request.Id != 0)
                            modelo = (await repository.select(p => p.Id == request.Id)).FirstOrDefault();

                        modelo = new VincularAtendimentoVenda
                        {
                            Venda = request.IdVenda,
                            Atendimento = request.IdAtendimento
                        };
                        await repository.SaveOrUpdate(modelo);


                    }

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
        [Route("api/Vendaapi/BuscaFiltroConsultaVenda")]
        public async Task<DataTableResponse<VendaListViewModel>> BuscaFiltroConsultaVenda(Venda_DataTableRequest request)
        {
            List<VendaListViewModel> retorno = new List<VendaListViewModel>();
            IOrderedEnumerable<VendaListViewModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_VENDA A ");

                sqlFrom.AppendLine("INNER JOIN TB_ETAPA_VENDA B  ");
                sqlFrom.AppendLine("ON B.ID_VENDA =A.ID  ");


                sqlFrom.AppendLine("WHERE A.STATUS <>6 AND A.STATUS <>7 AND A.STATUS <>4 ");




                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID,A.STATUS ");

                    sql.AppendLine(sqlFrom.ToString());


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new VendaListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // NUMERO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Numero);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Numero);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<VendaListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Vendaapi/verificaVendaDuplicadaCpfCnpj")]
        public async Task<PostApiRetorno<Int64>> verificaVendaDuplicadaCpfCnpj(string cpfCnpj)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            string mensagemCliente = null;
            string mensagemImovel = null;
            string mensagem = null;

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var repository = new Conexao<Venda>(session))
                using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repositoryCliente = new Conexao<Cliente>(session))
                using (var repositoryEndereco = new Conexao<Endereco_Cliente>(session))
                {
                    var config = await repositoryConfig.selectFirst();
                    string cpfSemFormatacao = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                    var cliente = await repositoryCliente.selectFirst(z => z.CpfCnpj == cpfSemFormatacao);

                    //Não permite venda para clientes com vendas em aberto
                    if (!config.Permite_Venda_Aberta_Cliente)
                    {
                        if (cliente != null)
                        {
                            IList<Venda> vendasExistentesCliente = repository.select(p =>
                            (p.Id_Cliente == cliente.Id) &&
                            p.Status != Status_Venda.Venda_Cancelada &&
                            p.Status != Status_Venda.Venda_Aprovada).Result;

                            if (vendasExistentesCliente.Count > 0)
                            {
                                mensagemCliente = @$"Existem {vendasExistentesCliente.Count} vendas em aberto para o mesmo Cliente!";
                                retorno.Mensagem = mensagemCliente;
                                retorno.IdObjeto = vendasExistentesCliente.Count;

                            }
                        }
                    }

                    retorno.Ok = true;
                    if (cliente != null && cliente.Tipo_Cliente == Tipo_Cliente.Prospecto)
                    {
                        if (config.ConsultaCreditoQualificaLead ||
                       config.ConsultaViabilidadeQualificaLead ||
                       config.ConsultaPendenciaEnderecoQualificaLead ||
                       config.ConsultaPendenciaFinanceiraQualificaLead)
                        {
                            var request = new ConsultaRestricaoQualificativaRequest
                            {
                                Id_Cliente = cliente.Id
                            };

                            var retornoConsultaRestricao = await new Cliente_ProspectoApiController().ConsultaRestricaoQualificativa(request);

                            if (retornoConsultaRestricao.result == false)
                            {
                                retorno.Ok = false;
                                retorno.Mensagem = retornoConsultaRestricao.message;
                                retorno.IdObjeto = 0;
                            }
                        }
                    }

                }
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Vendaapi/verificaVendaDuplicadaEndereco")]
        public async Task<DadosEnderecoValidacao> verificaVendaDuplicadaEndereco(DadosEnderecoValidacao endereco)
        {

            var data = new DadosEnderecoValidacao();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Venda>(session))
                    using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var repositoryCliente = new Conexao<Cliente>(session))
                    using (var repositoryEndereco = new Conexao<Endereco_Cliente>(session))
                    using (var repositoryEnderecoLo = new Conexao<Endereco_Logradouro>(session))
                    using (var repositoryCondominio = new Conexao<Condominio_Predio>(session))
                    using (var repositoryCidade = new Conexao<Cidade>(session))
                    using (var repositoryUf = new Conexao<Uf>(session))
                    {
                        IEnderecoLogradouroRepository repoEnderecoLogradouro = new EnderecoLogradouroRepository(session);
                        var config = await repositoryConfig.selectFirst();
                        var vendaDadosGerais = new VendaDadosGeraisViewModel();
                        var enderecoLogradouroBusiness = new EnderecoLogradouroBusiness(repoEnderecoLogradouro);
                        Endereco_Cliente enderecoInstalacao = null;

                        if (config.Permite_Venda_Aberta_Imovel)
                        {
                            data.vendasemacesso = false;

                            //Endereço/Logradouro
                            if (config.Utiliza_Cadastro_Endereco)
                            {
                                string cidade = null;

                                if (!String.IsNullOrEmpty(endereco.Cidade))
                                {
                                    var resultCidade = await repositoryCidade.selectFirst(p => p.Id == Convert.ToInt64(endereco.Cidade));
                                    cidade = resultCidade?.Descricao;
                                }
                                Endereco_Logradouro enderecoLogradouroInstalacao =
                                    enderecoLogradouroBusiness.GetByNomes(
                                        endereco.UF,
                                        cidade,
                                        endereco.Bairro,
                                        endereco.Logradouro
                                    );

                                var idEnderecoLogradouro = enderecoLogradouroInstalacao?.Id;
                                var buscaEndereco = Enum.Parse(typeof(Busca_Endereco), endereco.Busca_Endereco);
                                switch (buscaEndereco)
                                {
                                    case Busca_Endereco.Endereco_Logradouro:

                                        enderecoInstalacao = await repositoryEndereco.selectFirst(p => p.Endereco_Logradouro.Id == Convert.ToInt64(idEnderecoLogradouro));

                                        if (enderecoInstalacao != null)
                                        {
                                            IList<Venda> vendasExistentesEndereco = await repository.select(z => z.Status != Status_Venda.Venda_Cancelada
                                                                                                           && z.Status != Status_Venda.Venda_Aprovada);

                                            vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Endereco_Logradouro?.Id == enderecoInstalacao.Endereco_Logradouro.Id).ToList();

                                            if (vendasExistentesEndereco.Count > 0)
                                            {
                                                vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Numero == enderecoInstalacao.Numero &&
                                                                                                               p.EnderecoInstalacao?.Complemento == enderecoInstalacao.Complemento).ToList();

                                                if (vendasExistentesEndereco.Count > 0)
                                                {
                                                    data.mensagem = @$"Existem {vendasExistentesEndereco.Count} vendas em aberto para o mesmo endereço/imóvel!";
                                                }
                                            }
                                        }
                                        break;
                                    case Busca_Endereco.Condominio_Predio:
                                        var condominio = await repositoryCondominio.selectFirst(p => p.Endereco_Logradouro.Id == Convert.ToInt64(idEnderecoLogradouro));
                                        enderecoInstalacao = await repositoryEndereco.selectFirst(p => p.Condominio_Predio.Id == condominio.Id
                                                                                                    && p.Bloco == Convert.ToInt64(endereco.Bloco)
                                                                                                    && p.Apartamento == Convert.ToInt64(endereco.Apartamento));

                                        if (enderecoInstalacao != null)
                                        {
                                            IList<Venda> vendasExistentesEndereco = await repository.select(z => z.Status != Status_Venda.Venda_Cancelada
                                                                                                           && z.Status != Status_Venda.Venda_Aprovada);

                                            vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Endereco_Logradouro?.Id == enderecoInstalacao.Endereco_Logradouro.Id).ToList();

                                            if (vendasExistentesEndereco.Count > 0)
                                            {
                                                vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Numero == enderecoInstalacao.Numero &&
                                                                                                               p.EnderecoInstalacao?.Complemento == enderecoInstalacao.Complemento).ToList();

                                                if (vendasExistentesEndereco.Count > 0)
                                                {
                                                    data.mensagem = @$"Existem {vendasExistentesEndereco.Count} vendas em aberto para o mesmo endereço/imóvel!";
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            //Não utiliza cadastro de endereço
                            else
                            {
                                var uf = await repositoryUf.selectFirst(p => p.Sigla == endereco.UF);
                                if (uf is null)
                                {
                                    throw new Exception("uf não encontrado");
                                }

                                var enderecosInstalacao = await repositoryEndereco.select(p => p.Cep == endereco.Cep &&
                                           p.Logradouro == endereco.Logradouro &&
                                           p.Numero == endereco.Numero &&
                                           p.Id_Cidade == Convert.ToInt16(endereco.Cidade) &&
                                           p.Tipo_Imovel.Id == Convert.ToInt16(endereco.Tipo_Imovel) &&
                                           p.Complemento == endereco.Complemento);


                                enderecosInstalacao = enderecosInstalacao.Where(p => p.UF == uf.Sigla).ToList();


                                if (endereco.Complemento != null)
                                {
                                    enderecosInstalacao = enderecosInstalacao.Where(p => p.Complemento != null &&
                                                                   p.Complemento.ToLower().Trim() == endereco.Complemento.ToLower().Trim()).ToList();
                                }
                                else
                                {
                                    enderecosInstalacao = enderecosInstalacao.Where(p => p.Complemento == null).ToList();
                                }

                                var ids = enderecosInstalacao.Select(p => p.Id).ToList();
                                var vendas = await repository.select(p => p.EnderecoInstalacao.Id.IsIn(ids) &&
                                                                          p.Status != Status_Venda.Venda_Cancelada &&
                                                                          p.Status != Status_Venda.Venda_Aprovada);

                                if (vendas.Count() > 0)
                                {
                                    data.mensagem = @$"Existem {vendas.Count()} vendas em aberto para o mesmo endereço/imóvel!";
                                }

                            }
                        }
                        else
                        {
                            data.vendasemacesso = true;
                            //Endereço/Logradouro
                            if (config.Utiliza_Cadastro_Endereco)
                            {
                                string cidade = null;

                                if (!String.IsNullOrEmpty(endereco.Cidade))
                                {
                                    var resultCidade = await repositoryCidade.selectFirst(p => p.Id == Convert.ToInt64(endereco.Cidade));
                                    cidade = resultCidade?.Descricao;
                                }
                                Endereco_Logradouro enderecoLogradouroInstalacao =
                                    enderecoLogradouroBusiness.GetByNomes(
                                        endereco.UF,
                                        cidade,
                                        endereco.Bairro,
                                        endereco.Logradouro
                                    );

                                var idEnderecoLogradouro = enderecoLogradouroInstalacao?.Id;
                                var buscaEndereco = Enum.Parse(typeof(Busca_Endereco), endereco.Busca_Endereco);
                                switch (buscaEndereco)
                                {
                                    case Busca_Endereco.Endereco_Logradouro:

                                        enderecoInstalacao = await repositoryEndereco.selectFirst(p => p.Endereco_Logradouro.Id == Convert.ToInt64(idEnderecoLogradouro));

                                        if (enderecoInstalacao != null)
                                        {
                                            IList<Venda> vendasExistentesEndereco = await repository.select(z => z.Status != Status_Venda.Venda_Cancelada
                                                                                                           && z.Status != Status_Venda.Venda_Aprovada);

                                            vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Endereco_Logradouro?.Id == enderecoInstalacao.Endereco_Logradouro.Id).ToList();

                                            if (vendasExistentesEndereco.Count > 0)
                                            {
                                                vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Numero == enderecoInstalacao.Numero &&
                                                                                                               p.EnderecoInstalacao?.Complemento == enderecoInstalacao.Complemento).ToList();

                                                if (vendasExistentesEndereco.Count > 0)
                                                {
                                                    data.mensagem = @$"Existem {vendasExistentesEndereco.Count} vendas em aberto para o mesmo endereço/imóvel!";
                                                }
                                            }
                                        }
                                        break;
                                    case Busca_Endereco.Condominio_Predio:
                                        var condominio = await repositoryCondominio.selectFirst(p => p.Endereco_Logradouro.Id == Convert.ToInt64(idEnderecoLogradouro));
                                        enderecoInstalacao = await repositoryEndereco.selectFirst(p => p.Condominio_Predio.Id == condominio.Id
                                                                                                    && p.Bloco == Convert.ToInt64(endereco.Bloco)
                                                                                                    && p.Apartamento == Convert.ToInt64(endereco.Apartamento));

                                        if (enderecoInstalacao != null)
                                        {
                                            IList<Venda> vendasExistentesEndereco = await repository.select(z => z.Status != Status_Venda.Venda_Cancelada
                                                                                                           && z.Status != Status_Venda.Venda_Aprovada);

                                            vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Endereco_Logradouro?.Id == enderecoInstalacao.Endereco_Logradouro.Id).ToList();

                                            if (vendasExistentesEndereco.Count > 0)
                                            {
                                                vendasExistentesEndereco = vendasExistentesEndereco.Where(p => p.EnderecoInstalacao?.Numero == enderecoInstalacao.Numero &&
                                                                                                               p.EnderecoInstalacao?.Complemento == enderecoInstalacao.Complemento).ToList();

                                                if (vendasExistentesEndereco.Count > 0)
                                                {
                                                    data.mensagem = @$"Existem {vendasExistentesEndereco.Count} vendas em aberto para o mesmo endereço/imóvel!";
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            //Não utiliza cadastro de endereço
                            else
                            {
                                var uf = await repositoryUf.selectFirst(p => p.Sigla == endereco.UF);
                                if (uf is null)
                                {
                                    throw new Exception("uf não encontrado");
                                }

                                var enderecosInstalacao = await repositoryEndereco.select(p => p.Cep == endereco.Cep &&
                                           p.Logradouro == endereco.Logradouro &&
                                           p.Numero == endereco.Numero &&
                                           p.Id_Cidade == Convert.ToInt16(endereco.Cidade) &&

                                           p.Tipo_Imovel.Id == Convert.ToInt16(endereco.Tipo_Imovel) &&
                                           p.Complemento == endereco.Complemento);


                                enderecosInstalacao = enderecosInstalacao.Where(p => p.UF == uf.Sigla).ToList();


                                if (endereco.Complemento != null)
                                {
                                    enderecosInstalacao = enderecosInstalacao.Where(p => p.Complemento != null &&
                                                                   p.Complemento.ToLower().Trim() == endereco.Complemento.ToLower().Trim()).ToList();
                                }
                                else
                                {
                                    enderecosInstalacao = enderecosInstalacao.Where(p => p.Complemento == null).ToList();
                                }

                                var ids = enderecosInstalacao.Select(p => p.Id).ToList();
                                var vendas = await repository.select(p => p.EnderecoInstalacao.Id.IsIn(ids) &&
                                                                          p.Status != Status_Venda.Venda_Cancelada &&
                                                                          p.Status != Status_Venda.Venda_Aprovada);

                                if (vendas.Count() > 0)
                                {
                                    data.mensagem = @$"Existem {vendas.Count()} vendas em aberto para o mesmo endereço/imóvel!";
                                }

                                if (vendas.Count() > 0)
                                {
                                    data.mensagem = @$"Existem {vendas.Count()} vendas em aberto para o mesmo endereço/imóvel!";
                                }

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return data;
        }

        [HttpPost()]
        [Route("api/VendaApi/AdicionarInformacaoComplementar")]
        public async Task<PostApiRetorno<Int64>> AdicionarInformacaoComplementar(AdicionarInformacaoComplementarVendaModel model)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Ok = true;

            if (model.IdVenda == 0)
            {
                retorno.Ok = false;
                retorno.Mensagem = RsBase.NENHUMREGISTROSELECIONADO;
                return retorno;
            }

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Venda>(session))
                {
                    Venda venda = await repo.selectFirst(p => p.Id == model.IdVenda);

                    if (venda != null)
                    {
                        if (String.IsNullOrEmpty(venda.InformacoesComplementares))
                        {
                            venda.InformacoesComplementares = model.Informacao;
                        }
                        else
                        {
                            venda.InformacoesComplementares += $"\n{model.Informacao}";
                        }

                        await repo.Update(venda);
                        retorno.Mensagem = venda.InformacoesComplementares;

                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = RsBase.NENHUMREGISTROSELECIONADO;
                        return retorno;
                    }


                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }

        #region [Mudança Plano/Produto]
        [HttpPost()]
        [Route("api/VendaApi/MudancaPlanoProduto_Venda")]
        public async Task<PostApiRetorno<Int64>> MudancaPlanoProduto_Venda(AdicionarInformacaoComplementarVendaModel model)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var retornoCPP = await Get_MudancaPlanoProduto_Venda(model.IdPlanoProduto_Troca);
                    if (retornoCPP.Ok)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = retornoCPP.Mensagem;
                    }
                    else
                    {
                        var cliente_pp = await repo.selectFirst(p => p.Id == model.IdClientePlanoProduto);
                        if (cliente_pp != null)
                        {
                            cliente_pp.Id_TrocaPlano = model.IdPlanoProduto_Troca > 0 ? model.IdPlanoProduto_Troca : (long?)null;
                            await repo.SaveOrUpdate(cliente_pp);

                            retorno.Ok = true;
                        }
                        else
                        {
                            retorno.Ok = false;
                            retorno.Mensagem = RsBase.NENHUMREGISTROSELECIONADO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/VendaApi/Get_MudancaPlanoProduto_Venda")]
        public async Task<PostApiRetorno<Int64>> Get_MudancaPlanoProduto_Venda(Int64 IdPlanoProduto_Troca)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var obj_cpp = await repo.selectFirst(p => p.Id_TrocaPlano == IdPlanoProduto_Troca);
                    if (obj_cpp != null)
                    {
                        retorno.Ok = true;
                        retorno.Mensagem = "Atenção! Plano/Produto Programado em uma venda em aberto!\r\n \r\nPor favor! Confirme em outros planos/produtos vinculado a está venda ou em alguma outra venda em aberto para este cliente!";
                    }
                    else
                    {
                        retorno.Ok = false;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }
        #endregion

        //[HttpPost()]
        //[Route("api/Vendaapi/VincularAtendimento")]
        //public async Task<PostApiRetorno<Int64>> VincularAtendimento(VincularAtendimentoVendaRequestModels value)
        //{
        //    PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
        //    try
        //    {
        //        using (ISession session = HibernateUtil.GetSession())
        //        {
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                //String _ids = $"({String.Join(",", value.ids)})";

        //                StringBuilder sql = new StringBuilder($"UPDATE OR INSERT INTO TB_VINCATENDEVENDA VALUES (ID,ID_VENDA, ID_ATENDIMENTO) (ID,{value.Venda}, {value.Atendimento})  MATCHING (ID,ID_VENDA, ID_ATENDIMENTO) ");

        //                await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

        //                retorno.Ok = true;
        //            }g
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retorno.Mensagem = ex.Message;
        //        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
        //    }

        //    return retorno;
        //}

        #region MOBILE Extrato vendas Periodo Campanha 

        [HttpGet()]
        [Route("api/Vendaapi/exibirExtratoVendasPeriodoCampanha")]
        public async Task<GetApiRetorno<List<ExtratoVendas>>> filtro(Int64 id_Vendedor, String data_Ini, String data_Fim, Int64 id_Campanha)
        {
            GetApiRetorno<List<ExtratoVendas>> retorno = new GetApiRetorno<List<ExtratoVendas>>
            {
                Objeto = new List<ExtratoVendas>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();


                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT B.CPF, B.NOMEFANTASIA, A.DATA_VENDA, C.TITULO, SUM(E.QUANTIDADE) AS PONTOS");
                        sql.AppendLine("FROM TB_VENDA A");

                        sql.AppendLine("INNER JOIN TB_CLIENTE B");
                        sql.AppendLine("ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO D");
                        sql.AppendLine("ON(A.ID = D.ID_VENDA AND D.ID_COMBO IS NULL)");

                        sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("ON(E.ID_COMISSIONAMENTO = A.ID_COMISSIONAMENTO");
                        sql.AppendLine("   AND(E.ID_PLANO = D.ID_PLANO OR E.ID_PRODUTO = D.ID_PRODUTO))");

                        sql.AppendLine("LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("ON(D.ID_CAMPANHA = C.ID)");

                        sql.AppendLine("WHERE A.STATUS = 6");
                        sql.AppendLine("  AND A.ID_VENDEDOR = @ID_VENDEDOR");
                        sql.AppendLine("  AND A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM");
                        sql.AppendLine("  AND C.ID = @IDCAMPANHA");

                        sql.AppendLine("GROUP BY C.TITULO, B.NOMEFANTASIA, B.CPF, A.STATUS, A.DATA_VENDA");

                        sql.AppendLine("UNION");

                        sql.AppendLine("SELECT B.CPF, B.NOMEFANTASIA, A.DATA_VENDA, C.TITULO, SUM(E.QUANTIDADE) AS PONTOS");
                        sql.AppendLine("FROM TB_VENDA A");

                        sql.AppendLine("INNER JOIN TB_CLIENTE B");
                        sql.AppendLine("ON(A.ID_CLIENTE = B.ID)");

                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO D");
                        sql.AppendLine("ON(A.ID = D.ID_VENDA AND D.ID_COMBO IS NOT NULL)");

                        sql.AppendLine("INNER JOIN TB_COMBO_PLANO_PRODUTO F");
                        sql.AppendLine("ON(F.ID_COMBO = D.ID_COMBO)");

                        sql.AppendLine("LEFT JOIN TB_COMISSAO_PLANO_PRODUTO E");
                        sql.AppendLine("ON(E.ID_COMISSIONAMENTO = A.ID_COMISSIONAMENTO");
                        sql.AppendLine("   AND(E.ID_PLANO = F.ID_PLANO OR E.ID_PRODUTO = F.ID_PRODUTO))");

                        sql.AppendLine("LEFT JOIN TB_CAMPANHA C");
                        sql.AppendLine("ON(D.ID_CAMPANHA = C.ID)");

                        sql.AppendLine("WHERE A.STATUS = 6");
                        sql.AppendLine("  AND A.ID_VENDEDOR = @ID_VENDEDOR");
                        sql.AppendLine("  AND A.DATA_VENDA BETWEEN @DATAINI AND @DATAFIM");
                        sql.AppendLine("  AND C.ID = @IDCAMPANHA");

                        sql.AppendLine("GROUP BY C.TITULO, B.NOMEFANTASIA, B.CPF, A.STATUS, A.DATA_VENDA");

                        parametros.Add("@ID_VENDEDOR", id_Vendedor);
                        parametros.Add("@DATAINI", Convert.ToDateTime(data_Ini).ToString("dd/MM/yyyy"));
                        parametros.Add("@DATAFIM", Convert.ToDateTime(data_Fim).ToString("dd/MM/yyyy"));
                        parametros.Add("@IDCAMPANHA", id_Campanha);

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ExtratoVendas
                                {
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    NomeCliente = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Cpf = Convert.ToString(reader["CPF"]),
                                    DataVenda = Convert.ToDateTime(reader["DATA_VENDA"]),
                                    Pontos = Convert.ToInt64(reader["PONTOS"])
                                });
                            }

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }
        #endregion
        #region MOBILE Extrato vendas Periodo Campanha 

        [HttpGet()]
        [Route("api/Vendaapi/exibirTituloCampanha")]
        public async Task<GetApiRetorno<List<ExtratoVendas>>> ExibirTituloCampanha(Int64 id_Vendedor, String e_Supervisor)
        {
            GetApiRetorno<List<ExtratoVendas>> retorno = new GetApiRetorno<List<ExtratoVendas>>
            {
                Objeto = new List<ExtratoVendas>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Funcionario func = await getFuncionario(id_Vendedor);

                        sql.AppendLine("SELECT A.TITULO, A.ID ");
                        sql.AppendLine("FROM TB_CAMPANHA  A ");
                        if (e_Supervisor == "S")
                        {
                            sql.AppendLine("INNER JOIN TB_FUNCIONARIO B ON (B.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        sql.AppendLine("WHERE ");
                        if (e_Supervisor == "N")
                        {
                            sql.AppendLine("(A.ID_FUNCIONARIO = @ID_VENDEDOR ");
                            sql.AppendLine("OR(A.ID_FUNCIONARIO IS null AND A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        else
                        {
                            sql.AppendLine("(A.ID_FUNCIONARIO = B.ID ");
                            sql.AppendLine("OR(A.ID_FUNCIONARIO IS null AND A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        }
                        sql.AppendLine("OR(A.ID_FUNCIONARIO IS NULL AND A.ID_SUPERVISOR IS NULL AND A.ID_FILIAL = @ID_FILIAL)) ");
                        sql.AppendLine("GROUP BY A.TITULO, A.ID ");
                        if (e_Supervisor == "N")
                        {
                            parametros.Add("@ID_VENDEDOR", func.Id);
                            parametros.Add("@ID_SUPERVISOR", func.Supervisor.Id);
                        }
                        else
                        {
                            parametros.Add("@ID_SUPERVISOR", func.Id);
                        }
                        parametros.Add("@ID_FILIAL", func.Filial.Id);

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ExtratoVendas
                                {
                                    IdCampanha = Convert.ToInt64(reader["ID"]),
                                    Titulo = Convert.ToString(reader["TITULO"])
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        public async Task<Funcionario> getFuncionario(Int64 id_Vendedor)
        {
            Funcionario retorno = new Funcionario();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT ID, ID_SUPERVISOR, ID_FILIAL, E_SUPERVISOR FROM TB_FUNCIONARIO WHERE ID = @ID_VENDEDOR ");
                        parametros.Add("@ID_VENDEDOR", id_Vendedor);

                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (reader.Read())
                            {
                                retorno.Id = Convert.ToInt64(reader["ID"]);
                                retorno.Supervisor = new Funcionario();
                                if (reader["ID_SUPERVISOR"] == DBNull.Value && Convert.ToInt64(reader["E_SUPERVISOR"]) == 0)
                                {
                                    throw new Exception("Funcionário sem supervisor vinculado");
                                }
                                if (reader["ID_SUPERVISOR"] != DBNull.Value)
                                {
                                    retorno.Supervisor.Id = Convert.ToInt64(reader["ID_SUPERVISOR"]);
                                }
                                retorno.Filial = new Filial();
                                if (reader["ID_FILIAL"] == DBNull.Value)
                                {
                                    throw new Exception("Funcionário sem filial vinculada");
                                }
                                else
                                {
                                    retorno.Filial.Id = Convert.ToInt64(reader["ID_FILIAL"]);
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return retorno;
        }
        #endregion

        #region MOBILE Extrato vendas Periodo Campanha 
        [HttpGet()]
        [Route("api/Vendaapi/exibirVendedores")]
        public async Task<GetApiRetorno<List<Funcionario_Vendedor>>> ExibirVendedores(Int64 id_Supervisor)
        {
            GetApiRetorno<List<Funcionario_Vendedor>> retorno = new GetApiRetorno<List<Funcionario_Vendedor>>
            {
                Objeto = new List<Funcionario_Vendedor>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID,  A.NOME ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A ");
                        sql.AppendLine("LEFT JOIN TB_VENDA B ON(A.ID = B.ID_VENDEDOR) ");
                        sql.AppendLine("WHERE B.STATUS = 6 AND B.ID_SUPERVISOR = @ID_SUPERVISOR OR A.ID_SUPERVISOR = @ID_SUPERVISOR ");
                        sql.AppendLine("group BY A.ID, A.NOME ORDER BY UPPER(A.NOME) ");
                        parametros.Add("@ID_SUPERVISOR", id_Supervisor);

                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new Funcionario_Vendedor
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Nome = Convert.ToString(reader["NOME"])
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }
        #endregion

        #region mobile Consulta venda que precisam de alteração de vendedor
        [HttpGet()]
        [Route("api/Vendaapi/buscaVendasAlteracaoVendedor")]
        public async Task<GetApiRetorno<List<object>>> BuscaVendasAlteracaoVendedor(int supervisorId, int status)
        {
            GetApiRetorno<List<object>> retorno = new GetApiRetorno<List<object>>();

            List<AlteracaoVendaVendedor> listaAlteracaoVendaVendedor = new List<AlteracaoVendaVendedor>();
            using (ISession session = HibernateUtil.GetSession())
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ID, ID_VENDA, ID_VENDEDOR_DE, ID_VENDEDOR_PARA, STATUS FROM TB_ALTERACAO_VENDA_VENDEDOR WHERE ID_SUPERVISOR = @ID_SUPERVISOR AND STATUS = @STATUS");
                        parametros.Add("@ID_SUPERVISOR", supervisorId);
                        parametros.Add("@STATUS", status);

                        DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            listaAlteracaoVendaVendedor.Add(new AlteracaoVendaVendedor
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                id_venda = Convert.ToInt64(reader["ID_VENDA"]),
                                id_vendedor_de = Convert.ToInt64(reader["ID_VENDEDOR_DE"]),
                                id_vendedor_para = Convert.ToInt64(reader["ID_VENDEDOR_PARA"]),
                                status = (Status_Alteracao_Vendedor)Enum.Parse(typeof(Status_Alteracao_Vendedor), Convert.ToString(reader["STATUS"])),
                            });
                        }
                    }

                    List<Funcionario> vendedores = null;
                    using (Conexao<Funcionario> repository = new Conexao<Funcionario>(session))
                    {
                        List<long> ids_vendedores = listaAlteracaoVendaVendedor.Select(s => s.id_vendedor_de).ToList();
                        ids_vendedores.AddRange(listaAlteracaoVendaVendedor.Select(s => s.id_vendedor_para).ToList());

                        vendedores = (await repository.selectCriteria(Expression.In(nameof(Funcionario.Id), ids_vendedores.Distinct().ToArray()))).ToList();
                    }

                    List<Venda> vendas = null;
                    using (Conexao<Venda> repository = new Conexao<Venda>(session))
                    {
                        List<long> ids_vendas = listaAlteracaoVendaVendedor.Select(s => s.id_venda).ToList();

                        vendas = (await repository.selectCriteria(Expression.In(nameof(Venda.Id), ids_vendas.Distinct().ToArray()))).ToList();
                    }

                    List<Cliente> clientes = null;
                    using (Conexao<Cliente> repository = new Conexao<Cliente>(session))
                    {
                        clientes = (await repository.selectCriteria(Expression.In(nameof(Cliente.Id), vendas.Select(s => s.Id_Cliente).ToArray()))).ToList();
                    }

                    retorno.Ok = true;
                    retorno.Objeto = new List<object>(listaAlteracaoVendaVendedor.Select(s => new
                    {
                        id = s.Id,
                        id_venda = vendas.Where(w => w.Id == s.id_venda).FirstOrDefault()?.Id,
                        id_vendedor_para = vendedores.Where(w => w.Id == s.id_vendedor_para).FirstOrDefault()?.Id,
                        vendedor_de = vendedores.Where(w => w.Id == s.id_vendedor_de).FirstOrDefault()?.Nome,
                        vendedor_para = vendedores.Where(w => w.Id == s.id_vendedor_para).FirstOrDefault()?.Nome,
                        cpf = clientes.Where(w => w.Id == vendas.Select(v => v.Id_Cliente).FirstOrDefault()).FirstOrDefault()?.CpfCnpj ?? null,
                        data_venda = vendas.Where(w => w.Id == s.id_venda).FirstOrDefault()?.Data_Venda?.ToString("dd/MM/yyyy"),
                        usuario_prospecto = clientes.Where(w => w.Id == vendas.Select(v => v.Id_Cliente).FirstOrDefault()).FirstOrDefault()?.NomeFantasia ?? null,
                    }));
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Objeto = null;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

            return retorno;
        }
        #endregion


        #region mobile alteração de vendedor
        [HttpPost()]
        [Route("api/Vendaapi/updateStatusAlteracaoVendedor")]
        public async Task<GetApiRetorno<Boolean>> UpdateStatusAlteracaoVendedor(AlteracaoVendaVendedor alteracaoVendaVendedor)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Funcionario funcionario = null;
                    using (Conexao<Funcionario> repositoryFuncionario = new Conexao<Funcionario>(session))
                    {
                        funcionario = (await repositoryFuncionario.select(s => s.Id == alteracaoVendaVendedor.id_vendedor_para)).FirstOrDefault();
                    }

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        using (Conexao<AlteracaoVendaVendedor> repository = new Conexao<AlteracaoVendaVendedor>(session))
                        {
                            AlteracaoVendaVendedor update = (await repository.select(s => s.Id == alteracaoVendaVendedor.Id))?.FirstOrDefault();
                            if (update == null)
                                throw new Exception();

                            update.status = alteracaoVendaVendedor.status;
                            update.UsuarioAlteracao = alteracaoVendaVendedor.UsuarioAlteracao;
                            update.DataHoraAlteracao = DateTime.Now;

                            await repository.Update(update);
                        }

                        if (alteracaoVendaVendedor.status == Status_Alteracao_Vendedor.Aprovado)
                        {
                            Venda venda = null;
                            using (Conexao<Venda> repositoryVenda = new Conexao<Venda>(session))
                            {
                                venda = (await repositoryVenda.select(s => s.Id == alteracaoVendaVendedor.id_venda))?.FirstOrDefault();
                                if (venda == null)
                                    throw new Exception();

                                venda.Vendedor = funcionario;
                                venda.DataHoraAlteracao = DateTime.Now;
                                venda.UsuarioAlteracao = alteracaoVendaVendedor.UsuarioAlteracao;

                                await repositoryVenda.Update(venda);
                            }

                            using (Conexao<Cliente> repositoryCliente = new Conexao<Cliente>(session))
                            {
                                Cliente cliente = (await repositoryCliente.select(s => s.Id == venda.Id_Cliente)).FirstOrDefault();
                                if (cliente == null)
                                    throw new Exception();

                                cliente.Vendedor = funcionario;
                                cliente.DataHoraAlteracao = DateTime.Now;
                                cliente.UsuarioAlteracao = alteracaoVendaVendedor.UsuarioAlteracao;

                                await repositoryCliente.Update(cliente);
                            }
                        }

                        transaction.Commit();
                    }
                }
                retorno.Ok = true;
                retorno.Objeto = true;

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Vendaapi/alterarVendedorVenda")]
        public async Task<GetApiRetorno<Boolean>> AlterarVendedorVenda(ParamWebApi<AlteracaoVendaVendedor> value)
        {
            GetApiRetorno<Boolean> retorno = new GetApiRetorno<Boolean>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (Conexao<AlteracaoVendaVendedor> repository = new Conexao<AlteracaoVendaVendedor>(session))
                    {
                        AlteracaoVendaVendedor busca = await repository.selectFirst(s =>
                            s.id_venda == value.Objeto.id_venda &&
                            s.id_vendedor_para == value.Objeto.id_vendedor_para &&
                            s.status == Status_Alteracao_Vendedor.Aguadando_Aprovacao
                        );

                        if (busca != null)
                        {
                            throw new Exception(Resources.RsBase.JAEXISTESOLICITACAO);
                        }

                        value.Objeto.UsuarioCriacao = value.Usuario;
                        value.Objeto.DataHoraCriacao = DateTime.Now;
                        await repository.Save(value.Objeto);
                    }
                }
                retorno.Ok = true;
                retorno.Objeto = true;

            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        #endregion


        #region Consultas sobre Vendas pelo Cliente para exibir no Olá
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet()]
        [Route("api/Vendaapi/FindByIdOla")]
        public async Task<GetApiRetorno<List<VendaOlaModel>>> FindByIdOla(Int64? Id_Cliente)
        {
            GetApiRetorno<List<VendaOlaModel>> retorno = new GetApiRetorno<List<VendaOlaModel>>();

            retorno.Objeto = new List<VendaOlaModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Venda> repoVenda = new Conexao<Venda>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                try
                {
                    List<Venda> vendas = null;
                    Cliente cliente = null;
                    if (Id_Cliente.HasValue)
                    {
                        vendas = (List<Venda>)await repoVenda.select(p => p.Id_Cliente == Id_Cliente.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente.Value);
                    }

                    foreach (var venda in vendas)
                    {
                        var aux = await GetViewObjOla(venda, cliente, session);
                        retorno.Objeto.Add(aux);
                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [AllowAnonymous]
        [OlaAuthorize]
        [HttpGet()]
        [Route("api/Vendaapi/FindVendaByIdOla")]
        public async Task<GetApiRetorno<VendaOlaModel>> FindVendaByIdOla(Int64? Id_Venda, Int64? Id_Cliente)
        {
            GetApiRetorno<VendaOlaModel> retorno = new GetApiRetorno<VendaOlaModel>();

            retorno.Objeto = new VendaOlaModel();

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Venda> repoVenda = new Conexao<Venda>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;
                    if (Id_Venda.HasValue)
                    {
                        venda = (Venda)await repoVenda.selectFirst(p => p.Id == Id_Venda.Value);
                        cliente = await repoCliente.selectFirst(p => p.Id == Id_Cliente.Value);
                    }

                    var aux = await GetViewObjOla(venda, cliente, session);

                    retorno.Objeto = aux;

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Objeto = null;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }
        #endregion

        private void VerificaErros(List<PostApiMensagem> mensagens)
        {
            if (mensagens.Any(m => m.Tipo == PostApiMensagemType.Erro))
            {
                String mensagem = mensagens.Where(m => m.Tipo == PostApiMensagemType.Erro).First().Mensagem;
                throw new Exception(mensagem);
            }
        }


        private async Task<string> VerificaCheckList(string idVenda)
        {
            string dataHora = null;

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repositoryVenda = new Conexao<QuizAnswerVenda>(session))
            using (var repositoryCapa = new Conexao<QuizAnswerCapa>(session))
            {
                var venda = await repositoryVenda.selectFirst(p => p.IdVenda == Convert.ToInt64(idVenda));

                if (venda != null)
                {
                    var capa = await repositoryCapa.selectFirst(p => p.Id == Convert.ToInt64(venda.Capa.Id));

                    if (capa != null && capa.DiaInicio.HasValue && capa.HoraInicio.HasValue)
                    {
                        CultureInfo culture = CultureInfo.InvariantCulture;
                        DateTime diaInicio = Convert.ToDateTime(capa.DiaInicio, culture);
                        DateTime horaInicio = Convert.ToDateTime(capa.HoraInicio, culture);
                        dataHora = diaInicio.ToString("dd/MM/yyyy") + " " + horaInicio.ToString("HH:mm");
                    }
                }
            }

            return (dataHora);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Vendaapi/GravarMotivoAprovacaoManual")]
        public PostApiRetorno GravarMotivoAprovacaoManual(GravarMotivoAprovacaoVDManualRequest request)
        {
            var retorno = new PostApiRetorno();

            Venda venda = null;
            Venda modeloAnterior = null;

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    var vendaBusiness = new VendaBusiness(repoVenda);

                    vendaBusiness.GravarMotivoAprovacaoManualViabilidade(request.IdVenda, request.MotivoAprovacao, request.MotivoAprovacao, User.Identity.GetUserId());


                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/vendaapi/AssociarChecklistVenda")]
        public async Task<GetApiRetorno> AssociarChecklistVenda(AssociaChecklistVendaRequest request)
        {
            var retorno = new GetApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                {
                    IChecklistRepository repository = new ChecklistRepository(sessionT5Nh, sessionT5);
                    IEtapaVendaRepository repositoryEtapa = new EtapaVendaRepository(session);
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);

                    var business = new ChecklistBusiness(repository, repositoryEtapa);

                    if (business.GetChecklistVenda(request.Id) == null)
                    {
                        Venda venda = repositoryVenda.GetById(request.Id);
                        if (venda != null)
                        {
                            var etapaVenda = repositoryEtapa.GetByVenda(venda.Id);

                            if (!etapaVenda.Any(p => p.Etapa == EnumEtapaVenda.Checklist))
                            {
                                EtapaVenda etapaChecklist = new EtapaVenda() { 
                                    UsuarioCriacao = User.Identity.GetUserId(),
                                    DataHoraCriacao = DateTime.Now,
                                    Etapa = EnumEtapaVenda.Checklist,
                                    Descricao = EnumEtapaVenda.Checklist.ToString(),
                                    Status = StatusEtapaVenda.Pendente,
                                    Venda = venda
                                };

                                repositoryEtapa.Save(etapaChecklist);
                            }

                            Cliente cliente = repositoryCliente.GetById(venda.Id_Cliente.Value);

                            business.AssociarChecklistVenda(venda.Id, cliente.Tipo_Pessoa.Value, cliente?.Categoria_Cliente?.Id,
                                                            cliente?.Qual_Meio?.Id, cliente?.Endereco_Instalacao?.Id_UF,
                                                            cliente?.Endereco_Instalacao?.Id_Cidade);

                            Quiz checklist = business.GetChecklistVenda(request.Id);
                            if (checklist == null)
                            {
                                throw new Exception("Sem Checklist possível para ser associado a venda.");
                            }
                        }
                    }
                    else
                        throw new Exception("Já existe um checklist associado à venda");
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost()]
        [Route("api/vendaapi/IntegrarVenda")]
        public async void IntegrarVendaApi(IntegrarClienteComercial value)
        {
            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            {

                var retorno = await IntegrarVenda(value, session, sessionT5);

                await SalvaRetornoIntegracaoMk(new VendaAlterarStatusViewModel { id = value.Venda }, retorno);
            }
        }

        public async Task<GetApiRetorno<List<AtualizaContrato>>> IntegrarVenda(IntegrarClienteComercial value, ISession sessionConnect, ISession sessionT5)
        {
            var retorno = new GetApiRetorno<List<AtualizaContrato>>();
            retorno.Objeto = new List<AtualizaContrato>();
            try
            {
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryCidade = new Conexao<Cidade>(sessionConnect))
                using (var repoVenda = new Conexao<Venda>(sessionConnect))
                using (var repositoryLogVendas = new Conexao<LogVendas>(sessionLog))
                using (var repositoryAtendimento = new Conexao<Atendimento>(sessionConnect))
                using (var repoEndereco = new Conexao<Endereco_Cliente>(sessionConnect))
                using (var repoDesconto = new Conexao<Desconto>(sessionConnect))
                using (var repoIntegracooes = new Conexao<IntegracoesExternas>(sessionConnect))
                using (Conexao<ClienteContrato> repoClienteContrato = new Conexao<ClienteContrato>(sessionConnect))
                using (Conexao<Cliente_PlanoProduto> repoPlanoCliente = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                {
                    IIntegracaoExternaRepository repository = new IntegracaoExternaRepository(sessionConnect);
                    IVendaRepository vendarepository = new VendaRepository(sessionConnect);
                    IEnderecoClienteRepository enderecorepository = new EnderecoClienteRepository(sessionConnect);
                    IClienteRepository clienterepository = new ClienteRepository(sessionConnect);

                    var vendaBusiness = new VendaBusiness(vendarepository, enderecorepository);
                    var clienteBusiness = new ClienteBusiness(clienterepository);
                    var business = new IntegracaoExternaBusiness(repository);
                    IntegracoesExternas dadosIntegracao = null;
                    long idVendaAprovada = Convert.ToInt64(value.Venda);
                    var venda = vendaBusiness.GetVendaById(idVendaAprovada);
                    var cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);
                    ICidadeRepository cidadeRepository = new CidadeRepository(sessionConnect);
                    var etapaIntegracaoVenda = venda.EtapaIntegracao ?? (long?)EtapaIntegracaoIXC.A_Iniciar;
                    if (value.Integracao == IntegracaoExterna.IXC)
                    {
                        dadosIntegracao = business.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.IXC);

                        if (dadosIntegracao != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracao.UrlApi) ||
                                String.IsNullOrEmpty(dadosIntegracao.TokenAcesso) ||
                                String.IsNullOrEmpty(dadosIntegracao.Senha))
                            {
                                throw new Exception("Integração com a API do CRM da IXC não foi configurada corretamente");
                            }

                            IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                            var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                            string IdClienteIXC = string.Empty;
                            string idLoginIXC = string.Empty;
                            if (objIntegrator != null)
                            {

                                var constantesIXC = LocalData.GetConstantesIntegracaoIXC();

                                EtapaIntegracaoIXC etapa = (EtapaIntegracaoIXC)etapaIntegracaoVenda;

                                IIXCCrmRepository ixcCrmRepository = new IXCCrmRepository(objIntegrator.URL, objIntegrator.Token);

                                IConfiguracoesGeraisRepository configRepository = new ConfiguracoesGeraisRepository(sessionConnect);
                                var ixcCrmBusiness = new IXCCrmBusiness(ixcCrmRepository, cidadeRepository, configRepository, CONSTANTES.CpfCnpjSemCaracteres == "S");

                                IEnumerable<Cliente_PlanoProduto> planosVenda =
                                    vendaBusiness.GetPlanosProdutosVenda(venda.Id)
                                                 ?.Where(p => p.Plano != null || p.Combo != null);

                                int codLocalidade = (int)venda.EnderecoInstalacao.Cod_Localidade;
                                string codEscritorio = venda.EnderecoInstalacao.Cod_Escritorio;
                                string codArea = venda.EnderecoInstalacao.Cod_Area;

                                ILocalidadeAnielRepository _repo = new LocalidadeAnielRepository(sessionT5);
                                var localidadeAniel = _repo.GetByCodigo(codEscritorio, codArea, codLocalidade);
                                var contrato_projeto = _repoConfigAnielIntegrator.GetByProjeto(localidadeAniel.CodCt, localidadeAniel.Projeto).Result;

                                List<LogVendas> listaLogVendas = new List<LogVendas>();
                                StringBuilder logIxc = new StringBuilder();
                                try
                                {
                                    if (etapa <= EtapaIntegracaoIXC.Cliente)
                                    {
                                        etapa = EtapaIntegracaoIXC.Cliente;
                                        var findByClienteIXC = ixcCrmBusiness.FindByCliente(cliente.CpfCnpj);
                                        var retornoClienteIXC = new ResponsePostIXC();

                                        if (findByClienteIXC.registros != null)
                                        {
                                            findByClienteIXC.registros.ToList().ForEach(x =>
                                            {
                                                IdClienteIXC = x.id;
                                            });

                                            logIxc.Append($"USUÁRIO IXC - Usuário existente no IXC - ID: {IdClienteIXC} - NOME: {findByClienteIXC.registros[0].razao} - CPF/CNPJ:{findByClienteIXC.registros[0].cnpj_cpf} | ");

                                            var clienteIxcUpdate = findByClienteIXC.registros[0];
                                            if (clienteIxcUpdate.ativo == "N")
                                            {
                                                clienteIxcUpdate.ativo = "S";
                                                var retornoAtualizacaoClienteIXC = ixcCrmBusiness.AtualizarCliente(clienteIxcUpdate);
                                                logIxc.Append($"USUÁRIO IXC - Usuário atualizado no IXC - ID: {IdClienteIXC} - NOME: {findByClienteIXC.registros[0].razao} - CPF/CNPJ:{findByClienteIXC.registros[0].cnpj_cpf} - STATUS ALTERADO PARA ATIVO: S | ");
                                            }
                                            etapa = EtapaIntegracaoIXC.Contrato;
                                        }
                                        else
                                        {
                                            var clienteIXC = ixcCrmBusiness.GetViewObjClienteIXCResponse(cliente, Convert.ToString(contrato_projeto.IdFilial), venda, sessionT5, constantesIXC);
                                            retornoClienteIXC = ixcCrmBusiness.InserirCliente(clienteIXC);


                                            IdClienteIXC = retornoClienteIXC.id;
                                            if (string.IsNullOrEmpty(IdClienteIXC))
                                            {
                                                throw new Exception($"Etapa da Integração: {HtmlEnumExtensions.GetEnumDescription(etapa)} - {retornoClienteIXC.message}");
                                            }
                                            etapa = EtapaIntegracaoIXC.Contrato;
                                            logIxc.Append($"USUÁRIO IXC - ID: {retornoClienteIXC.id} - MENSAGEM: {retornoClienteIXC.message} - STATUS: {retornoClienteIXC.type} | ");
                                        }
                                        venda.EtapaIntegracao = Convert.ToInt64(etapa);
                                        vendaBusiness.Update(venda);
                                    }
                                    else
                                    {
                                        logIxc.Append("ETAPA DE INTEGRAÇÃO DO CLIENTE REALIZADA PREVIAMENTE. | ");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retorno.Ok = false;
                                    listaLogVendas.Add(new LogVendas
                                    {
                                        UsuarioCriacao = User.Identity.GetUserId(),
                                        NomeUsuarioCriacao = User.Identity.Name,
                                        DataHoraCriacao = DateTime.Now,
                                        TipoOperacao = TipoOperLog.Insercao,
                                        IdLog = venda.Id,
                                        Chave = Convert.ToString(venda.Id),
                                        Campo = RsCadastros.INTEGRACAO_EXTERNA,
                                        Log_Integracao = JsonConvert.SerializeObject(ex.Message)
                                    });
                                    await repositoryLogVendas.Save(listaLogVendas);
                                    throw ex;
                                }
                                var cont = 0;
                                string cabecalhoUsuarioTmp = logIxc.ToString();

                                foreach (var item in planosVenda)
                                {
                                    try
                                    {
                                        using (Conexao<ClienteContrato> repositoryClienteContrato = new Conexao<ClienteContrato>(sessionConnect))
                                        {

                                            if (cont > 0)
                                            {
                                                logIxc = new StringBuilder();
                                                logIxc.Append(cabecalhoUsuarioTmp);
                                            }

                                            logIxc.Append($" | ");
                                            var clienteContratoConnect = await repositoryClienteContrato.selectFirst(p => p.Id == item.ClienteContrato.Id);
                                            if (etapa <= EtapaIntegracaoIXC.Contrato)
                                            {
                                                ClienteContratoIXCResponse findByContrato = null;
                                                if (clienteContratoConnect?.CodIntegracao != null)
                                                {
                                                    findByContrato = ixcCrmBusiness.FindByIdContratoCliente(clienteContratoConnect.CodIntegracao).registros[0];

                                                }
                                                if (findByContrato == null)
                                                {
                                                    var findByClienteIXC = ixcCrmBusiness.FindByCliente(cliente.CpfCnpj);
                                                    findByClienteIXC.registros.ToList().ForEach(x =>
                                                    {
                                                        IdClienteIXC = x.id;
                                                    });

                                                    var id_motivo_inclusao = CONSTANTES.ID_MOTIVO_INCLUSAO_IXC;
                                                    var clienteContratoIXC = ixcCrmBusiness.GetViewObjClienteContratoAzzaResponse(IdClienteIXC, venda, item, Convert.ToString(contrato_projeto.IdFilial), id_motivo_inclusao, sessionT5, constantesIXC);
                                                    var retornoContratoIXC = ixcCrmBusiness.InserirContrato(clienteContratoIXC);
                                                    var codIntegacaoContrato = retornoContratoIXC.id;

                                                    if (string.IsNullOrEmpty(codIntegacaoContrato))
                                                    {
                                                        throw new Exception($"Etapa da Integração: {HtmlEnumExtensions.GetEnumDescription(etapa)} - {retornoContratoIXC.message}");
                                                    }
                                                    logIxc.Append($"CLIENTE CONTRATO IXC - ID: {codIntegacaoContrato} - MENSAGEM: {retornoContratoIXC.message} - STATUS: {retornoContratoIXC.type} | ");

                                                    clienteContratoConnect.CodIntegracao = codIntegacaoContrato;
                                                    retorno.Objeto.Add(new AtualizaContrato
                                                    {
                                                        IdContrato = clienteContratoConnect?.Id,
                                                        CodIntegracao = codIntegacaoContrato,
                                                        IdClientePlanoProduto = item.Id
                                                    });
                                                }

                                                etapa = EtapaIntegracaoIXC.Login;
                                                venda.EtapaIntegracao = Convert.ToInt64(etapa);
                                                vendaBusiness.Update(venda);
                                            }
                                            else
                                            {
                                                logIxc.Append("ETAPA DE INTEGRAÇÃO DO CONTRATO REALIZADA PREVIAMENTE. | ");
                                            }

                                            var findByContratoCliente = ixcCrmBusiness.FindByIdContratoCliente(clienteContratoConnect.CodIntegracao).registros[0];
                                            logIxc.Append($"PESQUISA CLIENTE CONTRATO IXC - ID_VD_CONTRATO: {findByContratoCliente.id_vd_contrato} - CLIENTE CONTRATO: {clienteContratoConnect.CodIntegracao} | ");
                                            var id_vd_contratoCC = findByContratoCliente.id_vd_contrato;

                                            var findByVdContratosProdutosIXC = ixcCrmBusiness.FindByVdContratosProdutos(id_vd_contratoCC).registros[0];
                                            logIxc.Append($"PESQUISA VD_CONTRATOS IXC - ID: {findByVdContratosProdutosIXC.id_plano} - CLIENTE CONTRATO: {clienteContratoConnect.CodIntegracao} | ");
                                            var id_plano_vdCP = findByVdContratosProdutosIXC.id_plano;
                                            if (etapa <= EtapaIntegracaoIXC.Login)
                                            {
                                                ResponseIXC<LoginIXCResponse> findByLogin = null;
                                                if (clienteContratoConnect?.CodIntegracao != null)
                                                {
                                                    findByLogin = ixcCrmBusiness.FindByLogin_Contrato(clienteContratoConnect.CodIntegracao);
                                                }
                                                if (findByLogin == null || findByLogin.registros == null)
                                                {
                                                    var loginIXC = ixcCrmBusiness.GetViewObjLoginIXCResponse(IdClienteIXC, venda, item, clienteContratoConnect.CodIntegracao, id_plano_vdCP, Convert.ToString(contrato_projeto.IdFilial), constantesIXC);

                                                    var retornoLoginIXC = ixcCrmBusiness.InserirLogin(loginIXC);
                                                    idLoginIXC = retornoLoginIXC.id;

                                                    if (string.IsNullOrEmpty(idLoginIXC))
                                                    {
                                                        throw new Exception($"Etapa da Integração: {HtmlEnumExtensions.GetEnumDescription(etapa)} - {retornoLoginIXC.message}");
                                                    }
                                                    logIxc.Append($"LOGIN IXC - ID: {idLoginIXC} - MENSAGEM: {retornoLoginIXC.message} - STATUS: {retornoLoginIXC.type} | ");
                                                }
                                                else
                                                {
                                                    logIxc.Append($"LOGIN IXC - Login existente no IXC - ID: {idLoginIXC} | ");
                                                }

                                                etapa = EtapaIntegracaoIXC.Atendimento;
                                                venda.EtapaIntegracao = Convert.ToInt64(etapa);
                                                vendaBusiness.Update(venda);
                                            }
                                            else
                                            {
                                                logIxc.Append("ETAPA DE INTEGRAÇÃO DO LOGIN REALIZADA PREVIAMENTE. | ");
                                            }

                                            if (etapa <= EtapaIntegracaoIXC.Atendimento)
                                            {
                                                //Verificação de atendimento
                                                ResponseIXC<AtendimentoIXCResponse> findAtendimentos = null;
                                                if (clienteContratoConnect?.CodIntegracao != null)
                                                {
                                                    findAtendimentos = ixcCrmBusiness.FindAtendimentoByContratoCliente(clienteContratoConnect.CodIntegracao);
                                                }
                                                if (findAtendimentos == null || findAtendimentos.registros == null)
                                                {
                                                    var atendimentoIXC = ixcCrmBusiness.GetViewObjAtendimentoIXCResponse(IdClienteIXC, clienteContratoConnect.CodIntegracao, idLoginIXC, item, Convert.ToString(contrato_projeto.IdFilial), constantesIXC);
                                                    var retornoAtendimentoIXC = ixcCrmBusiness.InserirAtendimento(atendimentoIXC);

                                                    if (string.IsNullOrEmpty(retornoAtendimentoIXC.id))
                                                    {
                                                        throw new Exception($"Etapa da Integração: {HtmlEnumExtensions.GetEnumDescription(etapa)} - {retornoAtendimentoIXC.message}");
                                                    }
                                                    logIxc.Append($"ATENDIMENTO IXC - ID: {retornoAtendimentoIXC.id} - MENSAGEM: {retornoAtendimentoIXC.message} - STATUS: {retornoAtendimentoIXC.type} | ");

                                                    var descPlanoProduto = item.Plano != null ? item.Plano?.Descricao : item.Produto?.Descricao;
                                                    logIxc.Append($"VENDA:{venda.Id} - PLANO/PRODUTO:{descPlanoProduto} | ");
                                                }
                                                etapa = EtapaIntegracaoIXC.Concluido;
                                                venda.EtapaIntegracao = Convert.ToInt64(etapa);
                                                vendaBusiness.Update(venda);
                                            }

                                            string logTmp = logIxc.ToString();
                                            listaLogVendas.Add(new LogVendas
                                            {
                                                UsuarioCriacao = User.Identity.GetUserId(),
                                                NomeUsuarioCriacao = User.Identity.Name,
                                                DataHoraCriacao = DateTime.Now,
                                                TipoOperacao = TipoOperLog.Insercao,
                                                IdLog = venda.Id,
                                                Chave = Convert.ToString(venda.Id),
                                                Campo = RsCadastros.INTEGRACAO_EXTERNA,
                                                Log_Integracao = JsonConvert.SerializeObject(logTmp)
                                            });

                                            cont++;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        retorno.Ok = false;
                                        listaLogVendas.Add(new LogVendas
                                        {
                                            UsuarioCriacao = User.Identity.GetUserId(),
                                            NomeUsuarioCriacao = User.Identity.Name,
                                            DataHoraCriacao = DateTime.Now,
                                            TipoOperacao = TipoOperLog.Insercao,
                                            IdLog = venda.Id,
                                            Chave = Convert.ToString(venda.Id),
                                            Campo = RsCadastros.INTEGRACAO_EXTERNA,
                                            Log_Integracao = JsonConvert.SerializeObject(ex.Message)
                                        });
                                        await repositoryLogVendas.Save(listaLogVendas);
                                        CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
                                        throw ex;
                                    }
                                }

                                await repositoryLogVendas.Save(listaLogVendas);

                            }
                        }
                    }

                    #region Integração MK Solutions

                    if (value.Integracao == IntegracaoExterna.MK_SOLUTION)
                    {

                        try
                        {
                            IntegracoesExternas dadosIntegracaoComercialMk =
                                    business.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MK_SOLUTION);

                            if (dadosIntegracaoComercialMk != null)
                            {
                                if (String.IsNullOrEmpty(dadosIntegracaoComercialMk.UrlApi) ||
                                    String.IsNullOrEmpty(dadosIntegracaoComercialMk.TokenAcesso) ||
                                    String.IsNullOrEmpty(dadosIntegracaoComercialMk.Senha))
                                {
                                    throw new Exception("Integração com a API do CRM do MK Solutions não configurada corretamente");
                                }

                                IMkCrmRepository repoMkCrm =
                                    new MkCrmRepository(
                                        dadosIntegracaoComercialMk.UrlApi,
                                        dadosIntegracaoComercialMk.TokenAcesso,
                                        dadosIntegracaoComercialMk.Senha,
                                        CONSTANTES.log4Net,
                                        dadosIntegracaoComercialMk.Token,
                                        dadosIntegracaoComercialMk.ExpirationToken,
                                        dadosIntegracaoComercialMk.Id
                                    );

                                ConstantesIntegracaoMk constantesMk = LocalData.GetConstantesIntegracaoMk();

                                var mkCrmBusiness = new MkCrmBusiness(repoMkCrm, constantesMk);

                                venda = await repoVenda.selectFirst(x => x.Id == value.Venda);

                                cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                                var clienteMK = mkCrmBusiness.GetClienteMk(cliente.CpfCnpj);

                                if (clienteMK != null && clienteMK.Situacao == "Inativo")
                                    throw new Exception("O cliente com CPF/CNPJ associado à venda está com status inativo.");

                                string codigoCliente = Convert.ToString(clienteMK?.CodigoPessoa) ?? "";
                                var enderecoIntegracao = await GetEnderencoByVenda(value.Venda);

                                DadosEstruturaEndereco dadosEnderecoInstalacaoMk = null;
                                InserirContratoResponse inserirContratoResponse = null;
                                InserirConexaoResponse conexaoResponse = null;
                                InserirAtendimentoResponse retornoCriacaoAtendimento = null;

                                if (String.IsNullOrEmpty(codigoCliente))
                                {
                                    string ufEndCliente = "";
                                    string cidadeEndCliente = "";
                                    string bairroEndCliente = "";
                                    string logradouroEndCliente = "";

                                    ufEndCliente = cliente.Endereco_Principal.UF;
                                    cidadeEndCliente = cliente.Endereco_Principal.Cidade;
                                    bairroEndCliente = cliente.Endereco_Principal.Bairro;

                                    if (cliente.Endereco_Principal.Endereco_Logradouro != null)
                                    {
                                        logradouroEndCliente = cliente.Endereco_Principal.Endereco_Logradouro.Descricao;
                                    }
                                    else
                                    {
                                        logradouroEndCliente = cliente.Endereco_Principal.Logradouro;
                                    }

                                    if (enderecoIntegracao == null)
                                    {
                                        DadosEstruturaEndereco dadosEnderecoClienteMk =
                                            mkCrmBusiness.GravarDadosEnderecoMk(
                                                ufEndCliente,
                                                cidadeEndCliente,
                                                LocalData.GetCodigoIBGECidade(ufEndCliente, cidadeEndCliente),
                                                bairroEndCliente,
                                                logradouroEndCliente,
                                                cliente.Endereco_Principal.Cep
                                            );

                                        await AtualizaCodigosAuxiliaresEndereco(venda.EnderecoInstalacao, dadosEnderecoClienteMk);

                                        dadosEnderecoInstalacaoMk = dadosEnderecoClienteMk;
                                    }
                                    else
                                    {
                                        IEnumerable<DadosEstruturaEndereco> enderecosMk = repoMkCrm.GetEnderecos();
                                        var enderecoMk = enderecosMk.FirstOrDefault(p => Convert.ToString(p.cd_logradouro) == enderecoIntegracao.CodRua.Trim());

                                        dadosEnderecoInstalacaoMk = new DadosEstruturaEndereco
                                        {
                                            cd_uf = Convert.ToInt64(enderecoMk.cd_uf),
                                            cd_cidade = Convert.ToInt64(enderecoMk.cd_cidade),
                                            cd_bairro = Convert.ToInt64(enderecoMk.cd_bairro),
                                            cd_logradouro = Convert.ToInt64(enderecoMk.cd_logradouro),
                                        };
                                    }

                                    try
                                    {
                                        codigoCliente = mkCrmBusiness.InserirCliente(cliente, dadosEnderecoInstalacaoMk, venda)?.CodigoPessoa;
                                    }
                                    catch (Exception ex)
                                    {
                                        CONSTANTES.log4Net.Error($"MK - erro ao gerar cliente. cpf cliente:{cliente.CpfCnpj} / mensagem de erro {ex.Message}");
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    string ufEndCliente = "";
                                    string cidadeEndCliente = "";
                                    string bairroEndCliente = "";
                                    string logradouroEndCliente = "";

                                    ufEndCliente = cliente.Endereco_Principal.UF;
                                    cidadeEndCliente = cliente.Endereco_Principal.Cidade;
                                    bairroEndCliente = cliente.Endereco_Principal.Bairro;

                                    if (cliente.Endereco_Principal.Endereco_Logradouro != null)
                                    {
                                        logradouroEndCliente = cliente.Endereco_Principal.Endereco_Logradouro.Descricao;
                                    }
                                    else
                                    {
                                        logradouroEndCliente = cliente.Endereco_Principal.Logradouro;
                                    }

                                    if (enderecoIntegracao == null)
                                    {
                                        DadosEstruturaEndereco dadosEnderecoClienteMk =
                                            mkCrmBusiness.GravarDadosEnderecoMk(
                                                ufEndCliente,
                                                cidadeEndCliente,
                                                LocalData.GetCodigoIBGECidade(ufEndCliente, cidadeEndCliente),
                                                bairroEndCliente,
                                                logradouroEndCliente,
                                                cliente.Endereco_Principal.Cep
                                            );

                                        await AtualizaCodigosAuxiliaresEndereco(venda.EnderecoInstalacao, dadosEnderecoClienteMk);

                                        dadosEnderecoInstalacaoMk = dadosEnderecoClienteMk;
                                    }
                                    else
                                    {
                                        IEnumerable<DadosEstruturaEndereco> enderecosMk = repoMkCrm.GetEnderecos();
                                        var enderecoMk = enderecosMk.FirstOrDefault(p => Convert.ToString(p.cd_logradouro) == enderecoIntegracao.CodRua.Trim());

                                        dadosEnderecoInstalacaoMk = new DadosEstruturaEndereco
                                        {
                                            cd_uf = Convert.ToInt64(enderecoMk.cd_uf),
                                            cd_cidade = Convert.ToInt64(enderecoMk.cd_cidade),
                                            cd_bairro = Convert.ToInt64(enderecoMk.cd_bairro),
                                            cd_logradouro = Convert.ToInt64(enderecoMk.cd_logradouro),
                                        };
                                    }
                                }

                                if (String.IsNullOrEmpty(codigoCliente))
                                {
                                    throw new Exception("Erro na integração com o MK Solutions. Não foi possível encontrar o cliente no MK Solutions");
                                }

                                IEnumerable<Cliente_PlanoProduto> planosVenda =
                                    vendaBusiness.GetPlanosProdutosVenda(venda.Id)
                                                 ?.Where(p => p.Plano != null || p.Combo != null).OrderByDescending(p =>
                                                    // Planos diretos de Internet
                                                    (p.Plano != null && p.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet) ||
                                                    // Combos que contêm Internet
                                                    (p.Combo != null && p.Combo.PlanosProdutos
                                                        .Any(x => x.Plano != null && x.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet))
                                                ).ToList();

                                if (planosVenda.Count() == 0)
                                {
                                    throw new Exception("Nenhum plano/produto foi adicionado a venda.");
                                }

                                bool atendimentoCriado = !string.IsNullOrEmpty(venda.IdAtendimentoIntegracao);

                                List<long> contratosMK = new List<long>();

                                foreach (Cliente_PlanoProduto planoProduto in planosVenda)
                                {
                                    string codigoContrato = planoProduto.CodContratoIntegracao;
                                    string codigoConexao = venda.IdLoginIntegracao;

                                    AtualizaContrato itemRetorno = new AtualizaContrato { IdClientePlanoProduto = planoProduto.Id };

                                    //caso seja plano avulso
                                    if (planoProduto.Combo == null)
                                    {
                                        #region Criar Contrato

                                        try
                                        {
                                            inserirContratoResponse = mkCrmBusiness.InserirContrato(
                                        venda,
                                        planoProduto.Plano,
                                        cliente.Tipo_Pessoa.Value,
                                        planoProduto?.Valor_Final_Total?.ToString(),
                                        planoProduto.Forma_Pagamento_Cliente.Dia_Vencimento,
                                        codigoCliente
                                    );

                                            codigoContrato = inserirContratoResponse.Mensagem.Split(' ')[1];
                                        }
                                        catch (Exception ex)
                                        {
                                            CONSTANTES.log4Net.Error($"MK - erro ao gerar CONTRATO. cliente:{codigoCliente} / mensagem de erro {ex.Message}");
                                            throw ex;
                                        }

                                        if (String.IsNullOrEmpty(inserirContratoResponse.Mensagem) || inserirContratoResponse.Mensagem.Split(' ').Length < 2)
                                        {
                                            throw new Exception("Não foi possível encontrar o código do contrato no MK Solutions");
                                        }

                                        #endregion

                                        #region Criar Conexao
                                        if (planoProduto.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet)
                                        {
                                            if (constantesMk.TempoDelayConexao.HasValue && constantesMk.TempoDelayConexao.Value > 0)
                                            {
                                                _ = Task.Run(async () =>
                                                {
                                                    try
                                                    {
                                                        await Task.Delay(constantesMk.TempoDelayConexao.Value);
                                                        mkCrmBusiness.InserirConexao(
                                                                                cliente,
                                                            venda,
                                                            planoProduto.Plano,
                                                            dadosEnderecoInstalacaoMk,
                                                            codigoCliente,
                                                            codigoContrato
                                                        );
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        CONSTANTES.log4Net.Error($"MK - erro ao gerar conexao. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    mkCrmBusiness.InserirConexao(
                                                                            cliente,
                                                        venda,
                                                        planoProduto.Plano,
                                                        dadosEnderecoInstalacaoMk,
                                                        codigoCliente,
                                                        codigoContrato
                                                    );
                                                }
                                                catch (Exception ex)
                                                {
                                                    CONSTANTES.log4Net.Error($"MK - erro ao gerar conexao. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                                    throw ex;
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Criar Atendimento

                                        if (!atendimentoCriado)
                                        {
                                            try
                                            {
                                                retornoCriacaoAtendimento = mkCrmBusiness.InserirAtendimento(new InserirAtendimentoRequest
                                                {
                                                    cd_cliente = codigoCliente,
                                                    cd_contrato = codigoContrato,
                                                    cd_processo = planoProduto.Plano.CodigoIntegracao,
                                                    cd_classificacao_ate = "12176",//buscar na API WSMKListaClassificacoesAte.rule
                                                    info = "Criação de atendimento via integração Aniel Connect",
                                                    origem_contato = "2",
                                                });
                                            }
                                            catch (Exception ex)
                                            {
                                                CONSTANTES.log4Net.Error($"MK - erro ao gerar atendimento. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                            }

                                            atendimentoCriado = true;
                                        }

                                        #endregion

                                        #region Criar Ordem de Servico

                                        if (!string.IsNullOrEmpty(planoProduto.Plano.CodigoIntegracaoAuxiliar))
                                        {
                                            try
                                            {
                                                var retornoCriacaoOS = mkCrmBusiness.InserirOrdemServico(new CriarOrdemServicoRequest
                                                {
                                                    CodigoCliente = codigoCliente,
                                                    CodigoContrato = codigoContrato,
                                                    CodigoConexao = conexaoResponse.Mensagem,
                                                    CodigoAtendimento = retornoCriacaoAtendimento.CodigoAtendimento.ToString(),
                                                    CodigoTipoOS = planoProduto.Plano.CodigoIntegracaoAuxiliar,
                                                    Cidade = dadosEnderecoInstalacaoMk.cd_cidade.Value,
                                                    Bairro = dadosEnderecoInstalacaoMk.cd_bairro.Value,
                                                    Logradouro = dadosEnderecoInstalacaoMk.cd_logradouro.Value,
                                                    Numero = venda.EnderecoInstalacao.Numero
                                                });
                                            }
                                            catch (Exception ex)
                                            {
                                                CONSTANTES.log4Net.Error($"MK - erro ao gerar atendimento. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                            }
                                        }
                                        #endregion

                                    }

                                    //se não, é um combo
                                    else
                                    {
                                        var combo = planoProduto.Combo;

                                        if (combo.PlanosProdutos.Any())
                                        {
                                            try
                                            {
                                                #region Criar Contrato

                                                using var integracoesExternas = new Conexao<IntegracoesExternas>(sessionConnect);
                                                var configIntegracao = await integracoesExternas.selectFirst(x => x.TipoIntegracao == Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA && x.IntegracaoExterna == Model.enumerado.IntegracaoExterna.ALCANS);
                                                IPendenciaEnderecoMkRepository repoPendenciaFinanceiraAlcans = new PendenciaEnderecoMkAlcansRepository(configIntegracao.Usuario, configIntegracao.Senha, configIntegracao.UrlApi);

                                                string idDesconto = "";
                                                string idFormaPgmt = "";

                                                IEnumerable<String> ContratosEspecificosVenda = cliente?.PlanosProdutos.Where(p => p.Id_Venda == venda.Id).Select(p => p.ClienteContrato?.NumeroContrato).ToList();
                                                IEnumerable<Cliente_Desconto> DescontosEspecificosVenda = cliente?.Descontos.Where(p => ContratosEspecificosVenda.Contains(p.Numero_ContratoVinculado)).ToList();
                                                var descontos = DescontosEspecificosVenda.Select(x => x.Desconto).ToList() ?? new List<Desconto>();

                                                if (descontos.Where(x => string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() > 1 || descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() > 1)
                                                    throw new Exception("Existe mais de um desconto com ou sem codigo auxiliar");

                                                //logica de desconto e forma de pagamento a definir (08/01/2026)

                                                ////não tem desconto
                                                //if(!descontos.Any())
                                                //{
                                                //    idDesconto = "";
                                                //    idFormaPgmt = constantesMk.CodigoFormaPagamento;
                                                //}
                                                ////caso haja 1 desconto sem cod auxiliar e 1 desconto com cod auxiliar
                                                //else if (descontos.Where(x => string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() == 1 && descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() == 1)
                                                //{
                                                //    var formasPgmntMK = repoPendenciaFinanceiraAlcans.BuscarFormasPagamento(combo.CodigoIntegracao);

                                                //    idDesconto = descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).FirstOrDefault().CodigoAuxiliar;
                                                //    idFormaPgmt = formasPgmntMK.payment_methods.Where(x => x.description == descontos.Where(x => string.IsNullOrEmpty(x.CodigoAuxiliar)).FirstOrDefault().Descricao).FirstOrDefault()?.id.ToString() ?? throw new Exception("Integração MK: Não existe forma de pagamento com a descrição do desconto aplicado");
                                                //}
                                                ////caso haja 1 desconto com cod auxiliar
                                                //else if(descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() == 1)
                                                //{
                                                //    idDesconto = descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).FirstOrDefault().CodigoAuxiliar;
                                                //    idFormaPgmt = constantesMk.CodigoFormaPagamento;
                                                //}
                                                ////caso haja 1 desconto sem cod auxiliar
                                                //else if (descontos.Where(x => string.IsNullOrEmpty(x.CodigoAuxiliar)).Count() == 1)
                                                //{
                                                //    //buscar na API da Alcans
                                                //    var formasPgmntMK = repoPendenciaFinanceiraAlcans.BuscarFormasPagamento(combo.CodigoIntegracao);

                                                //    idDesconto = "";
                                                //    idFormaPgmt = formasPgmntMK.payment_methods.Where(x => x.description == descontos.Where(x => string.IsNullOrEmpty(x.CodigoAuxiliar)).FirstOrDefault().Descricao).FirstOrDefault().id.ToString() ?? throw new Exception("Integração MK: Não existe forma de pagamento com a descrição do desconto aplicado");
                                                //}

                                                if (descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).Any())
                                                    idDesconto = descontos.Where(x => !string.IsNullOrEmpty(x.CodigoAuxiliar)).FirstOrDefault()?.CodigoAuxiliar;

                                                if (string.IsNullOrEmpty(codigoContrato))
                                                {
                                                    try
                                                    {
                                                        inserirContratoResponse = mkCrmBusiness.InserirContratoCombo(
                                                            venda,
                                                        combo,
                                                        idDesconto,
                                                            planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento,
                                                            cliente.Tipo_Pessoa.Value,
                                                        planoProduto?.Valor_Final?.ToString(),
                                                            planoProduto.Forma_Pagamento_Cliente.Dia_Vencimento,
                                                        codigoCliente);

                                                        codigoContrato = inserirContratoResponse.Mensagem.Split(' ')[1];
                                                        itemRetorno.CodIntegracao = codigoContrato;
                                                        contratosMK.Add(Convert.ToInt64(codigoContrato));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        CONSTANTES.log4Net.Error($"MK - erro ao gerar CONTRATO. cliente:{codigoCliente} / mensagem de erro {ex.Message}");
                                                        throw ex;
                                                    }
                                                }
                                                else
                                                {
                                                    itemRetorno.CodIntegracao = codigoContrato;
                                                    contratosMK.Add(Convert.ToInt64(codigoContrato));
                                                }

                                                if (string.IsNullOrEmpty(codigoContrato))
                                                {
                                                    throw new Exception("Não foi possível encontrar o código do contrato no MK Solutions");
                                                }

                                                #endregion

                                                #region Criar Conexao
                                                if (string.IsNullOrEmpty(codigoConexao))
                                                {
                                                    //cria uma conexão caso o combo tenha um plano do tipo internet
                                                    //cada combo só tem um plano do tipo internet
                                                    if (combo.PlanosProdutos.Where(x => x.Plano.Tipo_Tecnologia == Model.enumerado.Tipo_Tecnologia.Internet).Any())
                                                    {
                                                        try
                                                        {
                                                            conexaoResponse = mkCrmBusiness.InserirConexaoCombo(
                                                                    venda,
                                                                combo,
                                                                cliente,
                                                                    dadosEnderecoInstalacaoMk,
                                                                    codigoCliente,
                                                                    codigoContrato
                                                                );

                                                            codigoConexao = conexaoResponse.Mensagem.Split(' ')[1];
                                                            venda.IdLoginIntegracao = codigoConexao;
                                                            itemRetorno.CodConexao = codigoConexao;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            CONSTANTES.log4Net.Error($"MK - erro ao gerar conexao. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                                            throw ex;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    itemRetorno.CodConexao = codigoConexao;
                                                }

                                                #endregion

                                                #region Criar Atendimento

                                                if (!atendimentoCriado)
                                                {
                                                    try
                                                    {
                                                        if (string.IsNullOrEmpty(codigoConexao) && planosVenda.Where(x => x.Combo.PlanosProdutos.Where(x => x.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet).Any()).Any())
                                                        {
                                                            throw new Exception("A venda possui plano de internet e a conexão estava vazia");
                                                        }

                                                        retornoCriacaoAtendimento = mkCrmBusiness.InserirAtendimento(new InserirAtendimentoRequest
                                                        {
                                                            cd_cliente = codigoCliente,
                                                            cd_contrato = codigoContrato,
                                                            info = GerarDescricaoAtendimento(planosVenda, cliente, venda.EnderecoInstalacao),
                                                            cd_processo = cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica ? constantesMk.ProcessoPessoaFisica : constantesMk.ProcessoPJ,
                                                            cd_classificacao_ate = constantesMk.CodigoClassificacaoAtendimento,
                                                            origem_contato = "2",
                                                            conexaoId = codigoConexao,
                                                        });

                                                        venda.IdAtendimentoIntegracao = retornoCriacaoAtendimento.CodigoAtendimento.ToString();
                                                        itemRetorno.CodAtentimento = retornoCriacaoAtendimento.CodigoAtendimento.ToString();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        CONSTANTES.log4Net.Error($"MK - erro ao gerar atendimento. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                                        throw ex;
                                                    }

                                                    atendimentoCriado = true;
                                                }
                                                else
                                                {
                                                    itemRetorno.CodAtentimento = venda.IdAtendimentoIntegracao;
                                                }

                                                #endregion

                                                #region Criar Ordem de Servico

                                                if (string.IsNullOrEmpty(planoProduto.OrdemServicoIntegracao))
                                                {
                                                    if (combo.CodigoIntegracaoAuxiliar != "0")
                                                    {

                                                        if (string.IsNullOrEmpty(combo.CodigoIntegracaoAuxiliar))
                                                            throw new Exception($"Combo {combo.Descricao} não tem tipo OS configurado");


                                                        //criar uma OS "mãe" para o combo
                                                        var retornoCriacaoOSCombo = mkCrmBusiness.InserirOrdemServico(new CriarOrdemServicoRequest
                                                        {
                                                            CodigoCliente = codigoCliente,
                                                            CodigoContrato = codigoContrato,
                                                            CodigoConexao = codigoConexao,
                                                            CodigoAtendimento = venda.IdAtendimentoIntegracao,
                                                            CodigoTipoOS = combo.CodigoIntegracaoAuxiliar,
                                                            Cidade = dadosEnderecoInstalacaoMk.cd_cidade.Value,
                                                            Bairro = dadosEnderecoInstalacaoMk.cd_bairro.Value,
                                                            Logradouro = dadosEnderecoInstalacaoMk.cd_logradouro.Value,
                                                            Numero = venda.EnderecoInstalacao.Numero,
                                                            DescricaoProblema = GerarDescricaoAtendimento(planosVenda, cliente, venda.EnderecoInstalacao),
                                                        });

                                                        string codigoOSCombo = retornoCriacaoOSCombo.Mensagem.Split(' ')[1];

                                                        planoProduto.OrdemServicoIntegracao = codigoOSCombo;
                                                        itemRetorno.CodOS = codigoOSCombo;

                                                        //cria uma OS para cada plano no combo que tenha o codigo configurado
                                                        foreach (var plano in combo.PlanosProdutos)
                                                        {
                                                            if (!string.IsNullOrEmpty(plano.Plano.CodigoIntegracaoAuxiliar))
                                                            {
                                                                try
                                                                {
                                                                    var retornoCriacaoOS = mkCrmBusiness.InserirOrdemServico(new CriarOrdemServicoRequest
                                                                    {
                                                                        CodigoCliente = codigoCliente,
                                                                        CodigoContrato = codigoContrato,
                                                                        CodigoConexao = codigoConexao,
                                                                        CodigoAtendimento = venda.IdAtendimentoIntegracao,
                                                                        CodigoTipoOS = plano.Plano.CodigoIntegracaoAuxiliar,
                                                                        Cidade = dadosEnderecoInstalacaoMk.cd_cidade.Value,
                                                                        Bairro = dadosEnderecoInstalacaoMk.cd_bairro.Value,
                                                                        Logradouro = dadosEnderecoInstalacaoMk.cd_logradouro.Value,
                                                                        Numero = venda.EnderecoInstalacao.Numero,
                                                                        DescricaoProblema = $"#{codigoContrato}\r\n{cliente.NomeFantasia} contratou o plano {plano.Plano.Descricao} com uma\r\nmensalidade de R${plano.Plano.ValorPlano} válido por 12 meses.\r\nO pagamento deve ser feito até o dia {planoProduto.Forma_Pagamento_Cliente.Dia_Vencimento} de cada mês, e a fatura será enviada por e-mail.\r\nCaso ocorra o cancelamento tem uma taxa de R$480,00, abatendo R$40,00 por mês restante. Após esse período, um novo contrato será necessário para manter o desconto. O roteador será em locação e a ONU cedida em comodato; garanta que os equipamentos estejam em pleno funcionamento e sem danos.\r\n\r\nAgendamento: \r\nPeríodo: (manhã/tarde)\r\nTelefone: {cliente.Telefone_Celular}\r\n\r\nEndereço da Instalação:\r\nCidade:\r\nBairro:\r\nCEP:\r\nTipo de Residência:\r\nPonto de referência: \r\nAgendado pela Empresa: Sim ( ) Não ( )\r\nAutoriza um +18 acompanhar a instalação: Sim ( ) Não ( )"

                                                                    });

                                                                    string codigoOS = retornoCriacaoOS.Mensagem.Split(' ')[1];

                                                                    //definir um lugar pra salvar os IDs das OS dos planos
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    CONSTANTES.log4Net.Error($"MK - erro ao gerar Ordem de Serviço. cliente:{codigoCliente} / contrato: {codigoContrato} / mensagem de erro {ex.Message}");
                                                                    throw ex;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                        itemRetorno.CodOS = "Não cria OS";
                                                }
                                                else
                                                {
                                                    itemRetorno.CodOS = planoProduto.OrdemServicoIntegracao;
                                                }

                                                #endregion

                                                codigoContrato = null;
                                                codigoConexao = null;

                                                retorno.Objeto.Add(itemRetorno);
                                            }
                                            catch (Exception e)
                                            {
                                                retorno.Objeto.Add(itemRetorno);

                                                throw e;
                                            }
                                        }
                                    }
                                }

                                //verifica se já foi salvo agrupador na TB_CLIENTE_PLANO_PRODUTO
                                var clientePlanoProdutoVenda = await repoPlanoCliente.select(x => x.Id_Venda.Value == venda.Id);
                                var existeAgrupador = clientePlanoProdutoVenda.Where(x => x.CodigoAgrupamento != string.Empty && x.CodigoAgrupamento != null).Any(); 

                                if (!existeAgrupador)//nao existe agrupador, cria novo
                                {
                                    var intgExterna = await repoIntegracooes.selectFirst(x => x.TipoIntegracao == EnumTipoIntegracao.COMERCIAL_AGRUPADOR && x.IntegracaoExterna == IntegracaoExterna.MK_SOLUTION);

                                    if (intgExterna == null || string.IsNullOrEmpty(intgExterna.Usuario) || string.IsNullOrEmpty(intgExterna.Senha) || string.IsNullOrEmpty(intgExterna.UrlApi))
                                        throw new Exception("Integração de agrupamento não configurada corretamente");

                                    var alcansRepo = new AlcansRepository(intgExterna.Usuario, intgExterna.Senha, intgExterna.UrlApi);
                                    var retornoAgrupamentoContrato = alcansRepo.AgruparContratos(new AgruparContratosAlcansRequest
                                    {
                                        customer = Convert.ToInt64(codigoCliente),
                                        description = $"{venda.EnderecoInstalacao.Logradouro}, {venda.EnderecoInstalacao.Numero}",
                                        contracts = contratosMK
                                    });

                                    if (retornoAgrupamentoContrato?.contractGroupCode != null)
                                        retorno.Objeto.ForEach(x => { x.CodigoAgrupamento = retornoAgrupamentoContrato.contractGroupCode.ToString(); }); 
                                }
                                else
                                {
                                    //ja existe agrupador, pega o valor que está salvo no banco
                                    retorno.Objeto.ForEach(x => { x.CodigoAgrupamento = clientePlanoProdutoVenda.FirstOrDefault().CodigoAgrupamento; });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var listaLogVendas = new List<LogVendas>();

                            retorno.Ok = false;
                            listaLogVendas.Add(new LogVendas
                            {
                                UsuarioCriacao = User.Identity.GetUserId(),
                                NomeUsuarioCriacao = User.Identity.Name,
                                DataHoraCriacao = DateTime.Now,
                                TipoOperacao = TipoOperLog.Insercao,
                                IdLog = venda.Id,
                                Chave = Convert.ToString(venda.Id),
                                Campo = RsCadastros.INTEGRACAO_EXTERNA,
                                Log_Integracao = JsonConvert.SerializeObject(ex.Message)
                            });
                            await repositoryLogVendas.Save(listaLogVendas);
                            CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
                            throw ex;
                        }

                        #endregion

                        #region Integração Voalle
                        IntegracoesExternas dadosIntegracaoVoalle =
                                business.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.VOALLE);



                        if (dadosIntegracaoVoalle != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracaoVoalle.UrlApi) ||
                                String.IsNullOrEmpty(dadosIntegracaoVoalle.TokenAcesso) ||
                                String.IsNullOrEmpty(dadosIntegracaoVoalle.Senha))
                            {
                                throw new Exception("Integração com a API do CRM da Voalle não foi configurada corretamente");
                            }


                            //var autenticacao = await VoalleAuthenticate();
                            //if (DateTime.Now < dadosIntegracaoVoalle.ExpirationToken || autenticacao.Ok)
                            //{
                            //    var criarLead = await VoalleCreateLead(venda.Id);
                            //}
                            IVoalleRepository repoVoalle =
                            new VoalleApiRepository(dadosIntegracaoVoalle);

                            var VoalleBusiness = new VoalleBusiness(null, repoVoalle);

                            IVoalleRepository newRepoVoalle =
                             new VoalleApiRepository(dadosIntegracaoVoalle);

                            var newVoalleBusiness = new VoalleBusiness(null, newRepoVoalle);

                            var idCidade = cliente.Endereco_Principal?.Id_Cidade;
                            var codIBGE = string.Empty;
                            if (idCidade != null)
                            {
                                var cidade = await repositoryCidade.selectFirst(p => p.Id == idCidade);
                                codIBGE = cidade?.Codigo_IBGE;
                            }

                            VoalleBusiness.CreateLead(cliente, venda, codIBGE);
                        }

                        #endregion

                        #region Integração Playhub

                        cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);

                        IntegracoesExternas dadosIntegracaoPlayhub =
                                business.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.PLAYHUB);

                        if (dadosIntegracaoPlayhub != null)
                        {
                            if (String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiKey) ||
                                String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiSecret))
                            {
                                throw new Exception("Integração com a API do Playhub não configurada corretamente");
                            }

                            IPlayhubRepository repoPlayhub = new PlayhubApiRepository(dadosIntegracaoPlayhub.ApiKey, dadosIntegracaoPlayhub.ApiSecret);
                            var playhubBusiness = new PlayhubBusiness(repoPlayhub);

                            if (String.IsNullOrEmpty(cliente.UsuarioPlayhub))
                            {
                                string playhubUsername = playhubBusiness.CadastrarCliente(cliente);

                                cliente.UsuarioPlayhub = playhubUsername;
                                clienteBusiness.Update(cliente);
                            }

                            IEnumerable<Cliente_PlanoProduto> planosVenda =
                                vendaBusiness.GetPlanosProdutosVenda(venda.Id)
                                             ?.Where(p => p.Plano != null || p.Combo != null);

                            foreach (Cliente_PlanoProduto planoProduto in planosVenda)
                            {
                                if (planoProduto.Plano != null &&
                                    planoProduto.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Servicos_Adicionais &&
                                    !String.IsNullOrEmpty(planoProduto.Plano.ProductId))
                                {
                                    playhubBusiness.AdicionarPlanoUsuario(cliente.UsuarioPlayhub, planoProduto.Plano.ProductId);
                                }
                                else if (planoProduto.Combo != null)
                                {
                                    // Caso haja um combo, cada plano configurado com o código do playhub dentro do combo
                                    // é associado ao usuário do Playhub
                                    IEnumerable<ComboPlanoProduto> planosCombo =
                                        planoProduto
                                            .Combo
                                            .PlanosProdutos
                                            .Where(c => c.Plano != null &&
                                                        c.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Servicos_Adicionais &&
                                                        !String.IsNullOrEmpty(c.Plano.ProductId));

                                    foreach (ComboPlanoProduto planoProdutoCombo in planosCombo)
                                    {
                                        playhubBusiness.AdicionarPlanoUsuario(cliente.UsuarioPlayhub, planoProdutoCombo.Plano.ProductId);
                                    }
                                }
                            }
                        }

                        #endregion

                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
            }
            return retorno;
        }

        private static string GerarDescricaoAtendimento(IEnumerable<Cliente_PlanoProduto> planosVenda, Cliente cliente, Endereco_Cliente endereco)
        {
            var combos = planosVenda.Where(x => x.Combo != null).ToList();//pegar todos os combos

            string combosDescricao = string.Join(", ", combos.Select(x => x.Combo.Descricao).ToList());//pegar descricao todos os combos

            double valorTotal = combos.Select(x => x.Combo).SelectMany(x => x.PlanosProdutos).Sum(x => x.Valor);

            string artigo = combos.Count() == 1 ? "o plano" : "os planos";

            var descricao = $"{cliente.NomeFantasia} contratou {artigo} {combosDescricao} com uma\r\nmensalidade de {valorTotal.ToString("C", new CultureInfo("pt-BR"))} válido por 12 meses.\r\nO pagamento deve ser feito até o dia {planosVenda.FirstOrDefault().Forma_Pagamento_Cliente.Dia_Vencimento} de cada mês, e a fatura será enviada por e-mail.\r\nCaso ocorra o cancelamento tem uma taxa de R$480,00, abatendo R$40,00 por mês restante. Após esse período, um novo contrato será necessário para manter o desconto. O roteador será em locação e a ONU cedida em comodato; garanta que os equipamentos estejam em pleno funcionamento e sem danos.\r\n\r\nAgendamento: \r\nPeríodo: (manhã/tarde)\r\nTelefone: {cliente.Telefone_Celular}\r\n\r\nEndereço da Instalação: {endereco.Endereco_Logradouro.Descricao} n {endereco.Numero} \r\nCidade: {endereco.Endereco_Logradouro.Cidade.Descricao} \r\nBairro: {endereco.Endereco_Logradouro.Bairro.Descricao} \r\nCEP: {endereco.Endereco_Logradouro.Cep} \r\nTipo de Residência: {HtmlEnumExtensions.GetEnumDescription<Tipo_Endereco_Logradouro>(endereco.Endereco_Logradouro.Tipo.Value)} \r\nPonto de referência: {endereco.Ponto_Referencia} \r\nAgendado pela Empresa: Sim ( ) Não ( )\r\nAutoriza um +18 acompanhar a instalação: Sim ( ) Não ( )";

            // removendo caractere que dá erro na integração com MK
            descricao = descricao.Replace("’", "");
            return descricao;
        }

        public async Task<StatusIntegracaoVendaMk> GetStatusIntegracaoVendaMK(long idVenda)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var comm = session.Connection.CreateCommand())
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("idVenda", idVenda);

                StringBuilder sql = new StringBuilder(@"
                    SELECT A.ID_ATENDIMENTO_INTEGRACAO, B.COD_CONTRATO_INTEGRACAO , A.ID_LOGIN_INTEGRACAO, B.ORDEM_SERVICO_INTEGRACAO
                    FROM TB_VENDA A
                    INNER JOIN TB_CLIENTE_PLANO_PRODUTO B
                    ON(B.ID_VENDA = A.ID )
                    WHERE A.ID = @idVenda");

                using (var reader = await CommandHelper.ExecuteReader(comm, sql, param, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return new StatusIntegracaoVendaMk
                        {
                            CodContrato = reader["COD_CONTRATO_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["COD_CONTRATO_INTEGRACAO"]) : "",
                            CodConexao = reader["ID_LOGIN_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["ID_LOGIN_INTEGRACAO"]) : "",
                            CodAtendimento = reader["ID_ATENDIMENTO_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["ID_ATENDIMENTO_INTEGRACAO"]) : "",
                            CodOrdemServico = reader["ORDEM_SERVICO_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["ORDEM_SERVICO_INTEGRACAO"]) : "",
                        };
                    }

                    return null;
                }
            }
        }

        [HttpPost]
        [Route("api/vendaapi/VerificarIntegracao")]
        public async Task<GetApiRetorno<EtapasIntegracaoIXC>> VerificarIntegracao(IntegrarClienteComercial request)
        {
            var retorno = new GetApiRetorno<EtapasIntegracaoIXC> { Objeto = new EtapasIntegracaoIXC() };
            var etapasIntegracao = new Dictionary<string, bool>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repositoryClienteContrato = new Conexao<ClienteContrato>(session))
                using (var repositoryAtendimentoNh = new Conexao<Atendimento>(session))
                using (var repoPlanoCliente = new Conexao<Cliente_PlanoProduto>(session))
                using (var repoVenda = new Conexao<Venda>(session))
                {
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);
                    IAtendimentoRepository repositoryAtendimento = new AtendimentoRepository(session);
                    IClienteContratoRepository repositoryContrato = new ClienteContratoRepository(session);
                    ICidadeRepository cidadeRepository = new CidadeRepository(session);
                    IIntegracaoExternaRepository repositoryIntegracao = new IntegracaoExternaRepository(session);

                    var integracaoBusiness = new IntegracaoExternaBusiness(repositoryIntegracao);
                    var vendaBusiness = new VendaBusiness(repositoryVenda);
                    var clienteBusiness = new ClienteBusiness(repositoryCliente);

                    if (request.Integracao == IntegracaoExterna.IXC)
                    {
                        IntegracoesExternas dadosIntegracaoIxc = integracaoBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.IXC);
                        if (dadosIntegracaoIxc != null)
                        {
                            IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                            var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                            IIXCCrmRepository ixcCrmRepository = new IXCCrmRepository(objIntegrator.URL, objIntegrator.Token);

                            var venda = vendaBusiness.GetVendaById(request.Venda);
                            var cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);
                            var configRepository = new ConfiguracoesGeraisRepository(session);
                            var ixcCrmBusiness = new IXCCrmBusiness(ixcCrmRepository, cidadeRepository, configRepository, CONSTANTES.CpfCnpjSemCaracteres == "S");


                            var findByClienteIXC = ixcCrmBusiness.FindByCliente(cliente.CpfCnpj);

                            var contratosVenda = await vendaBusiness.GetPlanosProdutosVendaSQL(venda.Id);
                            var contratos = new Stack<ClienteContrato>();

                            if (findByClienteIXC?.registros?.Count > 0)
                                retorno.Objeto.Cliente = findByClienteIXC.registros[0].id;
                            
                            foreach (var item in contratosVenda)
                            {
                                var clienteContrato = repositoryContrato.GetById(item.Id);
                                List<Atendimento> atendimentos = (await repositoryAtendimentoNh.select(p => p.ClienteContrato.Id == item.Id))
                                                                                             .ToList();
                                contratos.Push(clienteContrato);
                                if (!string.IsNullOrEmpty(item.CodIntegracao))
                                {
                                    var findByContratoCliente = ixcCrmBusiness.FindByIdContratoCliente(item.CodIntegracao);
                                    var findByLogin = ixcCrmBusiness.FindByLogin_Contrato(item.CodIntegracao);
                                    var findAtendimento = ixcCrmBusiness.FindAtendimentoByContratoCliente(item.CodIntegracao);

                                    etapasIntegracao.Add($"Contrato_{item.CodIntegracao}", findByContratoCliente?.registros?.Count > 0);
                                    etapasIntegracao.Add($"Login_{item.CodIntegracao}", findByLogin?.registros?.Count > 0);
                                    etapasIntegracao.Add($"Atendimento_{item.CodIntegracao}", findAtendimento?.registros?.Count > 0);

                                    if (findByContratoCliente?.registros?.Count > 0)
                                        retorno.Objeto.ListaContratos = findByContratoCliente?.registros?.Select(x => x.id).ToList() ?? new List<string> { "Pendente" };

                                    if (findByLogin?.registros?.Count > 0)
                                        retorno.Objeto.ListaLogins = findByLogin?.registros?.Select(x => x.login).ToList() ?? new List<string> { "Pendente" };

                                    if (findAtendimento?.registros?.Count > 0)
                                        retorno.Objeto.ListaAtendimentos = findAtendimento?.registros?.Select(x => x.protocolo).ToList() ?? new List<string> { "Pendente" };
                                }
                                else
                                {
                                    etapasIntegracao.Add($"Contrato_{item.CodIntegracao}", false);
                                    etapasIntegracao.Add($"Login_{item.CodIntegracao}", false);
                                    etapasIntegracao.Add($"Atendimento_{item.CodIntegracao}", false);
                                }
                            }

                            using (var com = sessionLog.Connection.CreateCommand())
                            {
                                var condicoes = new List<string>();
                                var parametros = new Dictionary<string, object>();

                                var sql = new StringBuilder("SELECT A.LOG_INTEGRACAO FROM TB_VENDAS A");

                                condicoes.Add("A.LOG_INTEGRACAO IS NOT NULL");
                                condicoes.Add("A.IDLOG = @VENDA");
                                parametros.Add("@VENDA", venda.Id);

                                if (condicoes.Count > 0)
                                {
                                    sql.AppendLine(" WHERE " + String.Join(" AND ", condicoes));
                                }
                                sql.AppendLine(" ORDER BY A.ID DESC");
                                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLog.Connection, null))
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        retorno.Objeto.Mensagem = reader["LOG_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["LOG_INTEGRACAO"]) : "";
                                    }
                                }
                            }
                            etapasIntegracao.Add("Cliente", findByClienteIXC?.registros?.Count > 0);

                            retorno.Objeto.EtapaClienteIXC = etapasIntegracao["Cliente"];

                            retorno.Objeto.EtapaContratoIXC = contratos.Select(contrato => etapasIntegracao[$"Contrato_{contrato.CodIntegracao}"]).ToList();
                            retorno.Objeto.EtapaLoginIXC = contratos.Select(contrato => etapasIntegracao[$"Login_{contrato.CodIntegracao}"]).ToList();
                            retorno.Objeto.EtapaAtendimentoIXC = contratos.Select(contrato => etapasIntegracao[$"Atendimento_{contrato.CodIntegracao}"]).ToList();
                        }
                    }
                    else if (request.Integracao == IntegracaoExterna.MK_SOLUTION)
                    {
                        IntegracoesExternas dadosIntegracaoMk = integracaoBusiness.GetIntegracaoExterna(EnumTipoIntegracao.COMERCIAL, IntegracaoExterna.MK_SOLUTION);
                        if (dadosIntegracaoMk != null)
                        {
                            IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);

                            MkCrmRepository mkCrmRepository = new MkCrmRepository(dadosIntegracaoMk.UrlApi, dadosIntegracaoMk.TokenAcesso, dadosIntegracaoMk.Senha, null, dadosIntegracaoMk.Token, dadosIntegracaoMk.ExpirationToken, dadosIntegracaoMk.Id);

                            var venda = await repoVenda.selectFirst(x => x.Id == request.Venda);
                            var cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);
                            var configRepository = new ConfiguracoesGeraisRepository(session);
                            ConstantesIntegracaoMk constantesMk = LocalData.GetConstantesIntegracaoMk();

                            var mkCrmBusiness = new MkCrmBusiness(mkCrmRepository, constantesMk);

                            var clienteMk = mkCrmBusiness.GetClienteMk(cliente.CpfCnpj);

                            var clienteExisteMk = clienteMk != null && clienteMk.CodigoPessoa > 0;

                            var statusIntegracao = await GetStatusIntegracaoVendaMK(request.Venda);

                            using (var com = sessionLog.Connection.CreateCommand())
                            {
                                var condicoes = new List<string>();
                                var parametros = new Dictionary<string, object>();

                                var sql = new StringBuilder("SELECT A.LOG_INTEGRACAO FROM TB_VENDAS A");

                                condicoes.Add("A.LOG_INTEGRACAO IS NOT NULL");
                                condicoes.Add("A.IDLOG = @VENDA");
                                parametros.Add("@VENDA", venda.Id);

                                if (condicoes.Count > 0)
                                {
                                    sql.AppendLine(" WHERE " + String.Join(" AND ", condicoes));
                                }
                                sql.AppendLine(" ORDER BY A.ID DESC");
                                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionLog.Connection, null))
                                {
                                    if (await reader.ReadAsync() && venda.Status != Status_Venda.Venda_Aprovada)
                                    {
                                        retorno.Objeto.Mensagem = reader["LOG_INTEGRACAO"] != DBNull.Value ? Convert.ToString(reader["LOG_INTEGRACAO"]) : "";
                                    }
                                }
                            }
                            etapasIntegracao.Add("Cliente", clienteExisteMk);

                            if (retorno.Objeto.Mensagem == null)
                            {
                                retorno.Objeto.Mensagem = @$"Cliente: {clienteMk.CodigoPessoa}_Contrato: {statusIntegracao.CodContrato}_Conexão: {statusIntegracao.CodConexao}_Atendimento: {statusIntegracao.CodAtendimento}_Ordem Serviço: {statusIntegracao.CodOrdemServico}";
                            }

                            retorno.Objeto.EtapaClienteIXC = etapasIntegracao["Cliente"];
                            retorno.Objeto.EtapaContratoIXC = new List<bool> { !string.IsNullOrEmpty(statusIntegracao.CodContrato) };
                            retorno.Objeto.EtapaLoginIXC = new List<bool> { !string.IsNullOrEmpty(statusIntegracao.CodConexao) };
                            retorno.Objeto.EtapaAtendimentoIXC = new List<bool> { !string.IsNullOrEmpty(statusIntegracao.CodAtendimento) };
                            retorno.Objeto.EtapaOrdemServico = new List<bool> { !string.IsNullOrEmpty(statusIntegracao.CodOrdemServico) };

                            retorno.Objeto.Cliente = clienteExisteMk ? Convert.ToString(clienteMk.CodigoPessoa) : "";
                            retorno.Objeto.ListaContratos = !string.IsNullOrEmpty(statusIntegracao.CodContrato) ? new List<string>() { statusIntegracao.CodContrato } : new List<string>() { };
                            retorno.Objeto.ListaLogins = !string.IsNullOrEmpty(statusIntegracao.CodConexao) ? new List<string>() { statusIntegracao.CodConexao } : new List<string>() { };
                            retorno.Objeto.ListaAtendimentos = !string.IsNullOrEmpty(statusIntegracao.CodAtendimento) ? new List<string>() { statusIntegracao.CodAtendimento } : new List<string>() { };
                            retorno.Objeto.ListaOrdemServico = !string.IsNullOrEmpty(statusIntegracao.CodOrdemServico) ? new List<string>() { statusIntegracao.CodOrdemServico } : new List<string>() { };
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                etapasIntegracao.Clear();
                retorno.Mensagem = ex.Message;

                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/vendaapi/GetAllAnexosContrato")]
        public async Task<DataTableResponse<AnexosContratoAceiteListModel>> GetAllAnexosContrato(AnexosContratoAceiteDatatableRequest request)
        {
            var retorno = new List<AnexosContratoAceiteListModel>();
            int count = 0;
            string msgError = "";
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    var parametros = new Dictionary<string, object>();
                    var condicoes = new List<string>();

                    var sqlFrom = new StringBuilder();

                    var sqlWhere = new StringBuilder();

                    sqlFrom.AppendLine(" FROM TB_ANEXO_CONTRATO_ACEITE A INNER JOIN TB_VENDA B ")
                           .AppendLine(" ON A.ID_VENDA = B.ID ");
                    if (request.IdVenda != 0)
                    {
                        condicoes.Add("A.ID_VENDA = @VENDA");
                        parametros.Add("@VENDA", request.IdVenda);
                    }
                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine("WHERE " + string.Join(" AND ", condicoes));
                    }
                    using (var comm = session.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT COUNT(A.ID) AS RECORDS");
                        sql.AppendLine(sqlFrom.ToString())
                           .AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                count = Convert.ToInt32(reader["RECORDS"]);
                            }
                        }
                    }
                    using (var comm = session.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.ID_VENDA, A.DESCRICAO, A.NOME_ARQUIVO, A.EXTENSAO, A.EXIGE_ACEITE");
                        sql.AppendLine(sqlFrom.ToString())
                           .AppendLine(sqlWhere.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                string nomeArquivo = Convert.ToString(reader["NOME_ARQUIVO"]);
                                string ext = Convert.ToString(reader["EXTENSAO"]);
                                long venda = Convert.ToInt64(reader["ID_VENDA"]);
                                retorno.Add(new AnexosContratoAceiteListModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    ExigeAceite = Convert.ToBoolean(reader["EXIGE_ACEITE"]),
                                    Extensao = ext,
                                    NomeArquivo = nomeArquivo,
                                    Url = $"{CONSTANTES.URLPortal}/Arquivos/AnexosContrato/{venda}/{nomeArquivo}.{ext}",
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                msgError = ex.Message;
            }
            return new DataTableResponse<AnexosContratoAceiteListModel>
            {
                data = retorno,
                draw = request.draw,
                error = msgError,
                recordsFiltered = count,
            };
        }

        [HttpGet]
        [Route("api/vendaapi/DeleteAnexoContratoAceite")]
        public async Task<DeleteApiRetorno<long>> DeleteAnexoContratoAceite(long Id)
        {
            var retorno = new DeleteApiRetorno<long>();
            var parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repositoryVenda = new Conexao<Venda>(session))
            using (var repositoryAnexos = new Conexao<AnexoContratoAceiteModel>(session))
            {
                var anexo = await repositoryAnexos.selectFirst(p => p.Id == Id);
                var venda = await repositoryVenda.selectFirst(p => p.Id == anexo.IdVenda);

                using (var transaction = session.BeginTransaction())
                using (var transactionLog = sessionLog.BeginTransaction())
                {
                    try
                    {
                        using (var comm = session.Connection.CreateCommand())
                        {
                            var sqlDelete = new StringBuilder($"DELETE FROM TB_ANEXO_CONTRATO_ACEITE WHERE ID = {Id}");
                            transaction.Enlist(comm);
                            await CommandHelper.ExecuteNonQuery(comm, sqlDelete, session.Connection, comm.Transaction);
                        }
                        using (var comm = sessionLog.Connection.CreateCommand())
                        {
                            var sql = new StringBuilder();
                            parametros.Add("@USER", User.Identity.GetUserId());
                            parametros.Add("@USERNAME", User.Identity.GetUserName());
                            parametros.Add("@DATA_ATUAL", DateTime.Now);
                            parametros.Add("@OPERACAO", TipoOperLog.Alteracao);
                            parametros.Add("@VENDA", venda.Id);
                            parametros.Add("@ANEXO", anexo.Id);
                            parametros.Add("@STATUS", venda.Status);

                            sql.AppendLine("INSERT INTO TB_VENDAS (ID, USUARIOCRIACAO, NOMEUSUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, STATUSVENDA, CAMPO)")
                               .AppendLine($"VALUES (GEN_ID(GN_VENDAS, 1), @USER, @USERNAME, @DATA_ATUAL, @OPERACAO,@VENDA, @ANEXO, '', @STATUS, 'Exclusão de Anexo de Contrato')");
                            transactionLog.Enlist(comm);
                            await CommandHelper.ExecuteNonQuery(comm, sql, parametros, sessionLog.Connection, comm.Transaction);
                        }
                        transaction.Commit();
                        transactionLog.Commit();
                        retorno.Ok = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transactionLog.Rollback();

                        retorno.IdObjeto = 0;
                        retorno.Ok = false;
                        retorno.Mensagem = ex.Message;
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }
            }
            return retorno;
        }
        [HttpPost]
        [Route("api/vendaapi/AlterarStatusContratoAceite")]
        public async Task<PostApiRetorno> AlterarStatusAnexos(AlterarStatusAnexoRequest request)
        {
            var retorno = new PostApiRetorno();
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<AnexoContratoAceiteModel>(session))
            using (var repositoryVenda = new Conexao<Venda>(session))
            using (var sessionLog = HibernateUtil.GetSessionLog())
            using (var transactionConnect = session.BeginTransaction())
            using (var transaction = sessionLog.BeginTransaction())
            {
                try
                {
                    var anexos = (await repository.select(p => p.IdVenda == request.Venda && p.ExigeAceite == true)).ToList();
                    var venda = await repositoryVenda.selectFirst(p => p.Id == request.Venda);

                    var sql = new StringBuilder();
                    foreach (var anexo in anexos)
                    {
                        anexo.Status = request.Status;
                        anexo.DataHoraAlteracao = DateTime.Now;
                    }
                    await repository.SaveOrUpdate(anexos);

                    sql.AppendLine("INSERT INTO TB_VENDAS (ID, USUARIOCRIACAO, NOMEUSUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, STATUSVENDA, CAMPO)")
                       .AppendLine($"VALUES (GEN_ID(GN_VENDAS, 1), @USER, @USERNAME, @DATA_ATUAL, @OPERACAO,@VENDA, @ANTES, @DEPOIS, @STATUS, 'Alteração de Status de Anexo')");
                    foreach (var anexo in anexos)
                    {
                        using (var comm = sessionLog.Connection.CreateCommand())
                        {
                            var parametros = new Dictionary<string, object>();

                            parametros.Add("@USER", User.Identity.GetUserId());
                            parametros.Add("@USERNAME", User.Identity.GetUserName());
                            parametros.Add("@DATA_ATUAL", DateTime.Now);
                            parametros.Add("@OPERACAO", TipoOperLog.Alteracao);
                            parametros.Add("@VENDA", venda.Id);
                            parametros.Add("@ANTES", anexo.Status);
                            parametros.Add("@DEPOIS", request.Status);
                            parametros.Add("@STATUS", venda.Status);

                            transaction.Enlist(comm);
                            await CommandHelper.ExecuteNonQuery(comm, sql, parametros, sessionLog.Connection, comm.Transaction);
                        }
                    }

                    transactionConnect.Commit();
                    transaction.Commit();

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    transactionConnect.Rollback();
                    transaction.Rollback();

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }
        #region [metodos privados]        


        async Task<List<(string, string)>> BuscarDescontosCliente(long idCliente)
        {
            var retorno = new List<(string, string)>();

            using (ISession session = HibernateUtil.GetSession())
            {
                var parametros = new Dictionary<string, object>()
                { {"idCliente",idCliente} };

                var sql = new StringBuilder();
                sql.AppendLine(@"SELECT B.DESCRICAO,B.CODIGO_AUXILiAR FROM TB_CLIENTE_DESCONTO A
                                INNER JOIN TB_DESCONTO B
                                ON(B.ID = A.ID_DESCONTO)
                                WHERE A.ID_CLIENTE = @idCliente
                                ORDER BY B.DATAHORACRIACAO DESC");

                using (var comm = session.Connection.CreateCommand())
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add((Convert.ToString(reader["DESCRICAO"]), Convert.ToString(reader["CODIG_AUXILIAR"])));
                    }
                }
            }

            return retorno;
        }

        #endregion

        public class VendaComboResult : IObjComboResult<Int64>
        {
            public long Id { get; set; }
            public string Text { get; set; }
        }

        private async Task<VendaOlaModel> GetViewObjOla(Venda venda, Cliente cliente, ISession session)
        {
            VendaOlaModel retorno = new VendaOlaModel();
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (Conexao<Documento_ProducaoAniel> repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            {

                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());

                retorno.Id = venda.Id;
                retorno.DataCriacao = venda?.DataHoraCriacao?.ToString("dd/MM/yyyy");

                if (venda.Workflow_Vendas?.Id != null)
                {
                    retorno.StatusVenda = venda.Workflow_VendasAtividade?.Ativ_StatusVenda.Descricao;
                }
                else
                {
                    retorno.StatusVenda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status);
                }
                String idEncripted = Criptografia_AES256.doEncryptAES($@"{venda.Id}");
                String urlAssinarTermo = $@"{CONSTANTES.URLPortal}/pt/Termo_Adesao/Formulario/{idEncripted}";

                retorno.urlAssinarTermoOla = urlAssinarTermo;

                retorno.Nome_Vendedor = venda?.Vendedor?.Nome;
                retorno.Termo_Adesao_Assinado = venda.Termo_Adesao_Assinado;
                retorno.Exige_Termo_Adesao = config.Exige_Termo_Adesao;

                retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                {
                    Id_Pais = venda.EnderecoInstalacao?.Pais?.Id,
                    Id_Tipo_Imovel = venda.EnderecoInstalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = venda.EnderecoInstalacao?.Id_Bairro,
                    Bairro = venda.EnderecoInstalacao?.Bairro,
                    CEP = venda.EnderecoInstalacao?.Cep,
                    Id_Cidade = venda.EnderecoInstalacao?.Id_Cidade,

                    Cidade = venda.EnderecoInstalacao?.Cidade,
                    Complemento = venda.EnderecoInstalacao?.Complemento,
                    Id_Endereco = venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Id : (Int64)0,
                    Latitude = Convert.ToString(venda.EnderecoInstalacao?.Latitude),
                    Logradouro = venda.EnderecoInstalacao?.Logradouro,
                    Longitude = Convert.ToString(venda.EnderecoInstalacao?.Longitude),
                    Numero = venda.EnderecoInstalacao?.Numero,
                    Ponto_Referencia = venda.EnderecoInstalacao?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Instalacao,
                    UF = venda.EnderecoInstalacao?.UF,
                    Busca_Endereco = venda.EnderecoInstalacao?.Busca_Endereco,
                    Id_Endereco_Logradouro = venda.EnderecoInstalacao?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = venda.EnderecoInstalacao?.Condominio_Predio?.Id,
                    Id_Bloco = venda.EnderecoInstalacao?.Bloco,
                    Id_Apartamento = venda.EnderecoInstalacao?.Apartamento,

                    Cod_Escritorio = venda.EnderecoInstalacao?.Cod_Escritorio,
                    Cod_Area = venda.EnderecoInstalacao?.Cod_Area,
                    Cod_Localidade = Convert.ToString(venda.EnderecoInstalacao?.Cod_Localidade),

                    Id_Cliente = venda?.Id_Cliente
                };

                var formasPagamentoCliente = cliente.Formas_Pagamento_Cliente.ToList();

                retorno.Formas_Pagamento = formasPagamentoCliente.Select(f => new Forma_Pagamento_ClienteModels
                {
                    Id = f.Id,
                    Id_Cliente = f.Id,
                    Id_Forma_Pagamento = f.Forma_Pagamento?.Id,
                    Desc_Forma_Pagamento = f.Forma_Pagamento?.Descricao,

                    Forma_Envio = f.Forma_Envio,
                    Desc_Forma_Envio = f.Forma_Envio?.GetDisplayName(),
                    Id_Vencimento = f.Vencimento?.Id,
                    Id_Banco = f?.Banco?.Id,

                    Desc_Banco = !String.IsNullOrEmpty(f?.Banco?.Descricao) ? f?.Banco?.Descricao : "-",
                    Agencia = !String.IsNullOrEmpty(f.Agencia) ? f.Agencia : "-",
                    Conta_Corrente = !String.IsNullOrEmpty(f.Conta_Corrente) ? f.Conta_Corrente : "-",
                    Digito = !String.IsNullOrEmpty(f.Digito) ? f.Digito : "-",
                    Dia_Vencimento = f.Dia_Vencimento,

                    End_Cobranca_Igual_Principal = f.End_Cobranca_Igual_Principal,
                    Id_EnderecoCobranca = f.Endereco_Cobranca?.Id,
                    Id_Tipo_ImovelCobranca = f.Endereco_Cobranca?.Tipo_Imovel?.Id,
                    Id_PaisCobranca = f.Endereco_Cobranca?.Pais?.Id,
                    LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),

                    LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),

                    CEPCobranca = f.Endereco_Cobranca?.Cep,
                    LogradouroCobranca = f.Endereco_Cobranca?.Logradouro,
                    NumeroCobranca = f.Endereco_Cobranca?.Numero,
                    BairroCobranca = f.Endereco_Cobranca?.Bairro,
                    Id_BairroCobranca = f.Endereco_Cobranca?.Id_Bairro,
                    CidadeCobranca = f.Endereco_Cobranca?.Cidade,

                    Id_CidadeCobranca = f.Endereco_Cobranca?.Id_Cidade,
                    UFCobranca = f.Endereco_Cobranca?.UF,
                    ComplementoCobranca = f.Endereco_Cobranca?.Complemento,
                    Ponto_ReferenciaCobranca = f.Endereco_Cobranca?.Ponto_Referencia,
                    Status = f.Status,
                    Tipo_Forma_Pagamento = f.Forma_Pagamento?.Tipo_Forma_Pagamento

                }).ToList();

                IEnumerable<String> ContratosEspecificosVenda = cliente?.PlanosProdutos.Where(p => p.Id_Venda == retorno.Id).Select(p => p.ClienteContrato?.NumeroContrato).ToList();
                IEnumerable<Cliente_Desconto> DescontosEspecificosVenda = cliente?.Descontos.Where(p => ContratosEspecificosVenda.Contains(p.Numero_ContratoVinculado)).ToList();

                retorno.Descontos = (cliente?.Descontos.Count() == 0) ? new List<DescontoListaModel>() :
                DescontosEspecificosVenda.Select(p => new DescontoListaModel
                {
                    Id = p.Id,
                    Id_Desconto = p.Desconto != null ? p.Desconto.Id : 0,
                    Descricao = p.Desconto?.Descricao,
                    Valor_Desconto = Convert.ToString(p?.Valor_Desconto),
                    Percentual_Desconto = Convert.ToString(p.Desconto?.Percentual_Desconto),
                    Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao,
                    Numero_ContratoVinculado = p.Numero_ContratoVinculado,
                    Formato = !String.IsNullOrEmpty(Convert.ToString(p.Desconto?.Tipo_Desconto?.Formato)) ? Convert.ToString(Convert.ToInt64(p.Desconto?.Tipo_Desconto?.Formato)) : String.Empty
                }).ToList();

                IEnumerable<Cliente_PlanoProduto> listaPlanosProdutos = cliente?.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                //Pega Planos e produtos diferentes de cancelado
                retorno.Planos = new List<PlanoProdutoListModel>();

                foreach (Cliente_PlanoProduto planoProduto in listaPlanosProdutos)
                {
                    PlanoProdutoListModel planoProdutoModel = new PlanoProdutoListModel();

                    planoProdutoModel.Id_Modelo_Contrato = planoProduto.ClienteContrato?.ModeloContrato?.Id;
                    planoProdutoModel.Id_Contrato = planoProduto.ClienteContrato?.Id;
                    planoProdutoModel.Descricao_Contrato = planoProduto.ClienteContrato?.Descricao;
                    planoProdutoModel.Numero_Contrato = planoProduto.ClienteContrato?.NumeroContrato ?? "-";

                    planoProdutoModel.Id_Venda = venda?.Id;
                    planoProdutoModel.Id_Vendedor = venda?.Vendedor?.Id;
                    planoProdutoModel.Id_Supervisor = venda?.Supervisor?.Id;

                    planoProdutoModel.Status_Venda = venda?.Status;

                    planoProdutoModel.Desc_Status_Venda = venda != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status) : null;
                    planoProdutoModel.Nome_Vendedor = venda?.Vendedor?.Nome;
                    planoProdutoModel.Nome_Supervisor = venda?.Supervisor?.Nome;

                    planoProdutoModel.Id = planoProduto.Id;

                    planoProdutoModel.ValorPlanoProduto = Convert.ToString(planoProduto.Valor_Real);
                    planoProdutoModel.ValorInstalacao_Final = Convert.ToString(planoProduto.Valor_Instalacao_Final);
                    planoProdutoModel.ValorPlanoProduto_Final = Convert.ToString(planoProduto.Valor_Final);

                    planoProdutoModel.ValorInstalacao = Convert.ToString(planoProduto.Valor_Instalacao_Real);

                    planoProdutoModel.ValorTotal = Convert.ToString(planoProduto.Valor_Final_Total);
                    planoProdutoModel.Qtde_Dias = planoProduto.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis;
                    planoProdutoModel.Meses_Carencia = planoProduto.Meses_Carencia;

                    planoProdutoModel.Status_PlanoProduto = planoProduto.Status;
                    planoProdutoModel.Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription(planoProduto.Status.Value) : String.Empty;

                    planoProdutoModel.Numero_Parcelas = planoProduto.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao;
                    planoProdutoModel.End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal;
                    planoProdutoModel.End_Instalacao_Igual_Prospecto = venda.End_Instalacao_Igual_Prospecto;

                    planoProdutoModel.Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0;
                    planoProdutoModel.Periodo_Carencia = planoProduto.Periodo_Carencia;
                    planoProdutoModel.Login_Radius = planoProduto.Login_Radius;
                    planoProdutoModel.Senha_Radius = planoProduto.Senha_Radius;
                    planoProdutoModel.IdIpPool = planoProduto.Pool_Ip?.Id;
                    planoProdutoModel.Id_Endereco = planoProduto.Endereco_Instalacao.Id;

                    planoProdutoModel.Id_Bairro = planoProduto.Endereco_Instalacao?.Id_Bairro;
                    planoProdutoModel.Bairro = planoProduto.Endereco_Instalacao?.Bairro;
                    planoProdutoModel.CEP = planoProduto.Endereco_Instalacao?.Cep;
                    planoProdutoModel.Id_Cidade = planoProduto.Endereco_Instalacao?.Id_Cidade;
                    planoProdutoModel.Cidade = planoProduto.Endereco_Instalacao?.Cidade;
                    planoProdutoModel.Ponto_Referencia = planoProduto.Endereco_Instalacao?.Ponto_Referencia;

                    planoProdutoModel.Complemento = planoProduto.Endereco_Instalacao?.Complemento;
                    planoProdutoModel.Logradouro = planoProduto.Endereco_Instalacao?.Logradouro;
                    planoProdutoModel.Numero = planoProduto.Endereco_Instalacao?.Numero;
                    planoProdutoModel.UF = planoProduto.Endereco_Instalacao?.UF;

                    planoProdutoModel.Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude);
                    planoProdutoModel.Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude);
                    planoProdutoModel.Id_Pais = planoProduto.Endereco_Instalacao?.Pais?.Id;
                    planoProdutoModel.Id_Tipo_Imovel = planoProduto.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

                    planoProdutoModel.Cod_Escritorio = planoProduto.Cod_Escritorio;
                    planoProdutoModel.Cod_Area = planoProduto.Cod_Area;
                    planoProdutoModel.Cod_Localidade = planoProduto.Cod_Localidade;
                    planoProdutoModel.Localidade = planoProduto.Localidade;
                    planoProdutoModel.Localidade_Anatel = planoProduto.Localidade_Anatel;

                    planoProdutoModel.Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id;
                    planoProdutoModel.Id_Forma_Pagamento_Cliente = planoProduto.Forma_Pagamento_Cliente?.Id;

                    planoProdutoModel.Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento";

                    planoProdutoModel.Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null;
                    planoProdutoModel.FlagTrocaPlano = planoProduto.FlagTrocaPlano;
                    planoProdutoModel.CobrarInstalacaoEntrada = planoProduto.CobrarInstalacaoEntrada != null ? planoProduto.CobrarInstalacaoEntrada : false;

                    if (planoProduto.Plano != null)
                    {
                        planoProdutoModel.Tipo_Tecnologia = planoProduto?.Plano?.Tipo_Tecnologia;
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Plano.Id;
                        planoProdutoModel.Descricao = planoProduto.Plano.Descricao;
                        planoProdutoModel.Tipo = PlanoProdutoEnum.Plano;

                        if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                        }
                        else
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.NAO;
                        }
                    }
                    else if (planoProduto.Produto != null)
                    {
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Produto.Id;
                        planoProdutoModel.Descricao = planoProduto.Produto.Descricao;
                        planoProdutoModel.Tipo = PlanoProdutoEnum.Produto;

                        if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                        }
                        else
                        {
                            planoProdutoModel.NecessitaCriarOS = RsBase.NAO;
                        }
                    }
                    else if (planoProduto.Combo != null)
                    {
                        planoProdutoModel.Id_Plano_Produto = planoProduto.Combo.Id;
                        planoProdutoModel.Descricao = planoProduto.Combo.Descricao;
                        planoProdutoModel.Tipo = null;

                        if (planoProduto.Combo.PlanosProdutos.Any(x => x?.Plano?.Tipo_Tecnologia ==
                                                                        Tipo_Tecnologia.Internet))
                        {
                            planoProdutoModel.Tipo_Tecnologia = Tipo_Tecnologia.Internet;
                        }

                        // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                        planoProdutoModel.NecessitaCriarOS = RsBase.SIM;
                    }

                    // TODO: Descobrir como pegar a OS de uma venda de combo
                    if (planoProduto.Plano != null || planoProduto.Produto != null)
                    {
                        using (Documento_ProducaoApiController apiDocProd = new Documento_ProducaoApiController())
                        {
                            int idClienteAniel = (await apiDocProd.GetClienteAniel(sessionT5Nh, planoProduto.Cliente)).Cod_Cliente;
                            long? idPlanoProdutoAniel = (await apiDocProd.GetPlanoProdutoAniel(sessionT5Nh, planoProduto)).FirstOrDefault()?.Id;

                            Documento_ProducaoAniel planoProdutoOS = (await repoDocProd.selectFirst(p => p.Id_Cliente_PlanoProdutoConnect == planoProduto.Id &&
                                                                                                    p.Id_Venda == venda.Id));


                            if (planoProdutoOS != null)
                            {
                                planoProdutoModel.PossuiOSCriada = RsBase.SIM;
                            }
                            else
                            {
                                planoProdutoModel.PossuiOSCriada = RsBase.NAO;
                            }

                            planoProdutoModel.NumOS = planoProdutoOS?.Numero_Obra_Original;
                        }
                    }
                    else if (planoProduto.Combo != null)
                    {
                        planoProdutoModel.PossuiOSCriada = RsBase.NAO;
                    }

                    retorno.Planos.Add(planoProdutoModel);

                }

                RequestObjComboPai<Int64?, Int64?> request = new RequestObjComboPai<long?, long?>();
                request.idpai = cliente.Id;
                request.soativos = true;
                var listaAgrupada = await new ClienteContratoApiController().GetCombo(request);
                if (listaAgrupada.results.Count > 0)
                {
                    listaAgrupada.results.ForEach(x =>
                    {
                        retorno.ClienteContratos.Add(new ClienteContratoListModel
                        {
                            Id = x.Id,
                            Descricao = x.Text,
                            Numero_Contrato = Convert.ToString(x.Id)
                        });
                    });
                }
            }

            return retorno;
        }

        public async Task AtualizaCodigosAuxiliaresEndereco(Endereco_Cliente endereco, DadosEstruturaEndereco enderecoMK)
        {
            using ISession session = HibernateUtil.GetSession();
            using var repoUF = new Conexao<Uf>(session);
            using var repoCidade = new Conexao<Cidade>(session);
            using var repoBairro = new Conexao<Bairro>(session);
            using var repoLogradouro = new Conexao<Endereco_Logradouro>(session);

            var uf = await repoUF.selectFirst(x => x.Id == endereco.Endereco_Logradouro.UF.Id);
            uf.CodAuxiliar = enderecoMK.cd_uf.Value.ToString();
            await repoUF.Update(uf);

            var cidade = await repoCidade.selectFirst(x => x.Id == endereco.Endereco_Logradouro.Cidade.Id);
            cidade.CodigoAuxiliar = enderecoMK.cd_cidade.Value.ToString();
            await repoCidade.Update(cidade);

            var bairro = await repoBairro.selectFirst(x => x.Id == endereco.Endereco_Logradouro.Bairro.Id);
            bairro.CodigoAuxiliar = enderecoMK.cd_bairro.Value.ToString();
            await repoBairro.Update(bairro);

            var logradouro = await repoLogradouro.selectFirst(x => x.Id == endereco.Endereco_Logradouro.Id);
            logradouro.CodAuxiliar = enderecoMK.cd_logradouro.Value.ToString();
            await repoLogradouro.Update(logradouro);
        }
    }
}

public class EnderecoVendaIntegracao
{
    public string CodCidade { get; set; }
    public string CodUf { get; set; }
    public string CodBairro { get; set; }
    public string CodRua { get; set; }
    public string Numero { get; set; }
}

public class StatusIntegracaoVendaMk
{
    public string CodContrato { get; set; }
    public string CodConexao { get; set; }
    public string CodAtendimento { get; set; }
    public string CodOrdemServico { get; set; }
}
