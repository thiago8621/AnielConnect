using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.SERPRO.Models;
using Aniel.Connect.Integracoes.SERPRO.Models.Enums;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Services.SERPRO.Services;
using Aniel.Connect.Services.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Xceed.Words.NET;
using EnumEtapaVenda = Sinapse.Model.enumerados.EnumEtapaVenda;
using StatusEtapaVenda = Sinapse.Model.enumerados.StatusEtapaVenda;
using Aniel.Connect.ControllersWebApi.T5;
using System.Reflection;
using NHibernate.Hql.Ast;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Cliente_ProspectoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/GetAll")]
        public async Task<DataTableResponse<Cliente_ProspectoListViewModel>> GetAll(Cliente_Prospecto_DataTableRequest request)
        {
            List<Cliente_ProspectoListViewModel> retorno = new List<Cliente_ProspectoListViewModel>();
            IOrderedEnumerable<Cliente_ProspectoListViewModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            var mensagem = String.Empty;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoPerfilVisVenda = new Conexao<PerfilVisualizacaoVenda>(sessionNh))
                using (var repoFuncionario = new Conexao<IdentityUser>(sessionNh))
                {
                    IConfiguracoesGeraisRepository repoCfg = new ConfiguracoesGeraisRepository(sessionNh);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoCfg);

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CLIENTE A ")
                    .AppendLine("LEFT OUTER JOIN TB_ENDERECO_CLIENTE B ")
                    .AppendLine("ON(A.ID_ENDERECO_PROSPECTO = B.ID) ")
                    .AppendLine("LEFT OUTER JOIN TB_CATEGORIA_CLIENTE C ")
                    .AppendLine("ON(A.ID_CATEGORIA = C.ID) ")
                    .AppendLine("LEFT OUTER JOIN ASPNETUSERS D ")
                    .AppendLine("ON(A.USUARIOCRIACAO = D.ID) ")
                    .AppendLine("LEFT OUTER JOIN TB_PAIS E ")
                    .AppendLine("ON(E.ID = B.ID_PAIS) ")
                    .AppendLine("LEFT JOIN TB_DEPARTAMENTO F ")
                    .AppendLine("ON(F.ID = A.ID_DEPARTAMENTO) ")
                    .AppendLine("LEFT OUTER JOIN TB_MEIO_CAPTACAO G ")
                    .AppendLine("ON(G.ID = A.ID_MEIO_CAPTACAO) ")
                    .AppendLine("LEFT OUTER JOIN TB_FUNCIONARIO H ")
                    .AppendLine("ON(H.ID = D.ID_FUNCIONARIO) ")
                    .AppendLine("LEFT OUTER JOIN TB_EQUIPE_VENDA I ")
                    .AppendLine("ON(I.ID = H.EQUIPE_VENDA) ")
                        .AppendLine("LEFT OUTER JOIN TB_CLIENTE_PLANO_PRODUTO J ")
                        .AppendLine("ON(J.ID_CLIENTE = A.ID) ")
                        .AppendLine("LEFT OUTER JOIN TB_CAMPANHA K ")
                        .AppendLine("ON(K.ID = J.ID_CAMPANHA OR K.ID = A.ID_CAMPANHA)")
                        .AppendLine("LEFT OUTER JOIN TB_CLIENTE L ")
                        .AppendLine("ON(L.ID = A.ID_QUEM_INDICOU) ");

                    List<(Filtro_PainelVendaEnum tipo_filtro, string valores)> filtros_modelo = GetModeloFiltros(request.Id_Modelo, request);

                    StringBuilder _sqlWhere = new StringBuilder();
                    StringBuilder sqlGroup = new StringBuilder();
                    #region Perfil de usuário visualização leads
                    //1) Buscar informações do perfil de usuário
                    var usuario = await repoFuncionario.selectFirst(p => p.Id == User.Identity.GetUserId());

                    //2) Buscar as informações dos parametros de visualização
                    long? idPerfilVisualizacao = usuario?.PerfilVisualizacaoVenda?.Id;
                    var paramVisuVenda = await repoPerfilVisVenda.selectFirst(p => p.Id == idPerfilVisualizacao);
                    var visualizaTodosLeadsAcesso = AcessoHelper.TemAcesso(this.Areas, "333");

                    //if (config.Bloqueia_Lead && !AcessoHelper.TemAcesso(this.Areas, "333"))                    
                    if (!String.IsNullOrEmpty(request.StatusEtapa))
                    {
                        if (request.StatusEtapa == "2")
                        {

                            sqlFrom.AppendLine(" INNER JOIN TB_ETAPA_VENDA ET ");
                            sqlFrom.AppendLine(" ON ET.ID_CLIENTE = A.ID  AND  ET.STATUS = @STATUSEP ");
                            parametros.Add("@STATUSEP", request.StatusEtapa);

                            List<string> etapavenda = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.EtapaVenda).
                             Select(p => p.valores.Replace("COLUNA", "H.ETAPA")).
                             ToList();

                            if (etapavenda.Count() > 0)
                            {
                                _sqlWhere.AppendLine($" AND ({String.Join(" AND ", etapavenda)}) ");
                            }
                        }
                        if (request.StatusEtapa == "3")
                        {

                            sqlFrom.AppendLine(" INNER JOIN TB_ETAPA_VENDA ET ");
                            sqlFrom.AppendLine(" ON ET.ID_CLIENTE = A.ID  AND  ET.STATUS = @STATUSEP ");
                            parametros.Add("@STATUSEP", request.StatusEtapa);

                            List<string> etapavenda = filtros_modelo.FindAll(p => p.tipo_filtro == Filtro_PainelVendaEnum.EtapaVenda).
                             Select(p => p.valores.Replace("COLUNA", "H.ETAPA")).
                             ToList();

                            if (etapavenda.Count() > 0)
                            {
                                _sqlWhere.AppendLine($" AND ({String.Join(" AND ", etapavenda)}) ");
                            }
                        }

                        if (request.StatusEtapa == "5")
                        {
                            sqlFrom.AppendLine(" INNER JOIN TB_ETAPA_VENDA ET ");
                            sqlFrom.AppendLine(" ON ET.ID_CLIENTE =A.ID AND  ET.STATUS <>1 ");
                        }
                    }

                    sqlFrom.AppendLine("WHERE A.ID > 0 AND A.STATUS_PROSPECTO IS NOT NULL AND A.STATUS_PROSPECTO <> 4");

                    if (!String.IsNullOrEmpty(request.StatusEtapa) && request.ConsultasRestricao.Count > 0)
                    {
                        var consultas = String.Join(",", request.ConsultasRestricao.Select(p => $"{p}"));
                        sqlFrom.AppendLine($" AND M.ETAPA IN ({consultas}) ");
                    }

                    sqlGroup.AppendLine("GROUP BY A.ID,A.NUMERO_CONTRATO, C.DESCRICAO , A.CPF, A.TIPOPESSOA, A.TELEFONE_FIXO,F.DESCRICAO , ");
                    sqlGroup.AppendLine("A.NOMEFANTASIA, B.ENDERECO_CIDADE, B.ENDERECO_UF, B.ENDERECO_BAIRRO,A.STATUS, D.USERNAME,C.DESCRICAO, A.STATUS_PROSPECTO, ");
                    sqlGroup.AppendLine("A.SEXO, A.ESTADO_CIVIL, E.NOME, A.DATAHORACRIACAO, G.DESCRICAO, L.NOMEFANTASIA, A.OUTRO_MEIO, K.TITULO, A.ID_QUEM_INDICOU, D.NOME,A.TELEFONE_CELULAR ");
                    if (paramVisuVenda == null && !visualizaTodosLeadsAcesso)
                    {
                        sqlFrom.AppendLine("AND (A.USUARIOCRIACAO = @USUARIO)");
                        parametros.Add("@USUARIO", User.Identity.GetUserId());
                    }
                    else if (!visualizaTodosLeadsAcesso)
                    {
                        if (paramVisuVenda?.PadraoVisualizaLead == false && paramVisuVenda.Qtd_Dias_Bloqueio_Lead != null)
                        {
                            sqlFrom.AppendLine("AND (A.USUARIOCRIACAO = @USUARIO OR CAST(A.DATAHORACRIACAO AS DATE) <= DATEADD(DAY, -" + paramVisuVenda.Qtd_Dias_Bloqueio_Lead + ", CURRENT_DATE))");
                            parametros.Add("@USUARIO", User.Identity.GetUserId());
                        }
                        else
                        {
                            sqlFrom.AppendLine("AND (A.USUARIOCRIACAO = @USUARIO)");
                            parametros.Add("@USUARIO", User.Identity.GetUserId());
                        }

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

                            sqlFrom.AppendLine($"AND F.ID IN ({setorIds})");
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

                            sqlFrom.AppendLine($"AND A.ID_MEIO_CAPTACAO IN ({meioCaptacaoIds})");
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

                            sqlFrom.AppendLine($"AND A.ID_CATEGORIA IN ({categoriasIds})");
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

                            sqlFrom.AppendLine($"AND A.TIPOPESSOA IN ({tipoPessoaIds})");
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

                            sqlFrom.AppendLine($"AND B.ID_CIDADE IN ({cidadeIds})");
                        }

                        //Equipe Venda
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

                            sqlFrom.AppendLine($"AND H.EQUIPE_VENDA IN ({equipesIds})");
                        }

                    }

                    if (!String.IsNullOrEmpty(request.Data_Inicial) && !String.IsNullOrEmpty(request.Data_Final))
                    {
                        sqlFrom.AppendLine("AND A.DATAHORACRIACAO BETWEEN @DATA_INI AND @DATA_FIM");
                        parametros.Add("@DATA_INI", request.Data_Inicial);
                        parametros.Add("@DATA_FIM", request.Data_Final);
                    }
                    #endregion

                    // TODO: corrigir alias pós merge  ADICIONAR FILTRO DE ETAPA              if (!String.IsNullOrEmpty(request.StatusEtapa))



                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlFrom.AppendLine("AND ((A.CPF LIKE @SEARCHCPF) OR (UPPER(A.NOMEFANTASIA) LIKE @SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                        parametros.Add("@SEARCHCPF", StringService.Truncate(String.Format("%{0}%", request.search.value.ToUpper().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)), 18));
                    }

                    if (!String.IsNullOrEmpty(request.AcessoProspect))//OK
                    {
                        //sqlFrom.AppendLine($" AND A.TIPO_CLIENTE = '{request.AcessoProspect}' ");
                        sqlFrom.AppendLine($" AND F.ID = (SELECT P.ID_DEPARTAMENTO FROM ASPNETUSERS P WHERE P.ID = '{request.AcessoUsuario}' )");
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Pessoa))
                    {
                        sqlFrom.AppendLine("AND (A.TIPOPESSOA = @TIPOPESSOA)");
                        parametros.Add("@TIPOPESSOA", Convert.ToInt32(request.Tipo_Pessoa));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                    }

                    if (!String.IsNullOrEmpty(request.Pais))
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ID_PAIS) = @PAIS)");
                        parametros.Add("@PAIS", request.Pais);
                    }

                    if (!String.IsNullOrEmpty(request.Estado))
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_UF) = @ESTADO)");
                        parametros.Add("@ESTADO", request.Estado);
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ID_CIDADE) = @CIDADE)");
                        parametros.Add("@CIDADE", request.Cidade);
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlFrom.AppendLine("AND (UPPER(B.ENDERECO_BAIRRO) = @BAIRRO)");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }

                    if (!String.IsNullOrEmpty(request.Sexo))
                    {
                        sqlFrom.AppendLine("AND (A.SEXO = @SEXO)");
                        parametros.Add("@SEXO", request.Sexo);
                    }

                    if (!String.IsNullOrEmpty(request.Estado_Civil))
                    {
                        sqlFrom.AppendLine("AND (A.ESTADO_CIVIL = @ESTADO_CIVIL)");
                        parametros.Add("@ESTADO_CIVIL", request.Estado_Civil);
                    }

                    if (!String.IsNullOrEmpty(request.Grau_Instrucao))
                    {
                        sqlFrom.AppendLine("AND (A.GRAU_INSTRUCAO = @GRAU_INSTRUCAO)");
                        parametros.Add("@GRAU_INSTRUCAO", request.Grau_Instrucao);
                    }

                    if (!String.IsNullOrEmpty(request.IdMeioCaptcao))
                    {
                        sqlFrom.AppendLine("AND (G.ID = @MEIOCAPT)");
                        parametros.Add("@MEIOCAPT", request.IdMeioCaptcao);
                    }

                    if (!String.IsNullOrEmpty(request.IdCampanha))
                    {
                        sqlFrom.AppendLine("AND (K.ID = @CAMPANHA)");
                        parametros.Add("@CAMPANHA", request.IdCampanha);
                    }

                    if (!String.IsNullOrEmpty(request.Id_QuemIndicou))
                    {
                        sqlFrom.AppendLine("AND (L.ID_QUEM_INDICOU = @QINDICOU)");
                        parametros.Add("@QINDICOU", request.Id_QuemIndicou);
                    }

                    if (!String.IsNullOrEmpty(request.Status_Prospecto))
                    {
                        sqlFrom.AppendLine("AND (A.STATUS_PROSPECTO = @STATUS_PROSPECTO)");
                        parametros.Add("@STATUS_PROSPECTO", request.Status_Prospecto);
                    }
                    if (!String.IsNullOrEmpty(request.Departamento))
                    {
                        sqlFrom.AppendLine("AND (F.ID = @DEPARTAMENTO)");
                        parametros.Add("@DEPARTAMENTO", request.Departamento);
                    }

                    if (!String.IsNullOrEmpty(request.FiltroEndereco))
                    {
                        if (request.FiltroEndereco == "sem_endereco")
                        {
                            sqlFrom.AppendLine("AND (B.ENDERECO_LOGRADOURO IS NULL OR B.ENDERECO_LOGRADOURO = '')");
                        }
                        else if (request.FiltroEndereco == "com_endereco")
                        {
                            sqlFrom.AppendLine("AND (B.ENDERECO_LOGRADOURO IS NOT NULL AND B.ENDERECO_LOGRADOURO <> '')");
                        }
                    }

                    if (request.Somente_Leads /*&& String.IsNullOrEmpty(request.Status_Prospecto)*/)
                    {
                        sqlFrom.AppendLine("AND A.STATUS_PROSPECTO IN (0,1) ");
                    }

                    if (!string.IsNullOrEmpty(request.UsuarioCriacao))
                    {
                        sqlFrom.AppendLine(" AND A.USUARIOCRIACAO = @USUARIO");
                        parametros.Add("@USUARIO", StringService.Truncate(request.UsuarioCriacao, 30));
                    }

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(_sqlWhere.ToString());
                        sql.AppendLine(sqlGroup.ToString());


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal++;
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID,A.NUMERO_CONTRATO, C.DESCRICAO AS CATEGORIA_CLIENTE, A.CPF, A.TIPOPESSOA, A.TELEFONE_FIXO,F.DESCRICAO AS DEPARTAMENTO, A.NOMEFANTASIA, B.ENDERECO_CIDADE, B.ENDERECO_UF, ")
                           .AppendLine("B.ENDERECO_BAIRRO,A.STATUS, D.USERNAME,C.DESCRICAO AS CATEGORIA, A.STATUS_PROSPECTO,A.SEXO, A.ESTADO_CIVIL, E.NOME AS PAIS, A.DATAHORACRIACAO, G.DESCRICAO AS MEIOCAPTACAO, L.NOMEFANTASIA AS QINDICACAO, ")
                           .AppendLine("K.TITULO AS CAMPANHA, A.OUTRO_MEIO  AS OUTRO_MEIO, A.ID_QUEM_INDICOU, D.NOME,A.TELEFONE_CELULAR");
                        sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(_sqlWhere.ToString());
                        sql.AppendLine(sqlGroup.ToString());

                        //Ordenacao
                        if (request.order != null && request.order.Any())
                        {
                            int sortColumnIndex = request.order[0].column;
                            var coluna = request.columns[sortColumnIndex].data;
                            string sortDirection = request.order[0].dir;
                            switch (coluna)
                            {
                                case "numero_Contrato":     // Status
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.NUMERO_CONTRATO");
                                        else
                                            sql.AppendLine(" ORDER BY A.NUMERO_CONTRATO DESC ");
                                        break;
                                    }
                                case "status":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY STATUS_ORDENAR ");
                                        else
                                            sql.AppendLine(" ORDER BY STATUS_ORDENAR DESC ");
                                        break;
                                    }
                                case "tipoPessoa":     // TipoPessoa
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.TIPOPESSOA ");
                                        else
                                            sql.AppendLine(" ORDER BY A.TIPOPESSOA DESC ");
                                        break;
                                    }
                                case "nomeFantasia":     // NomeFantasia
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.NOMEFANTASIA ");
                                        else
                                            sql.AppendLine(" ORDER BY A.NOMEFANTASIA DESC ");
                                        break;
                                    }
                                case "cpfCnpj":     // CPF CNPJ
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.CPF ");
                                        else
                                            sql.AppendLine(" ORDER BY A.CPF DESC ");
                                        break;
                                    }
                                case "categoria":     // Categoria
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY CATEGORIA ");

                                        else
                                            sql.AppendLine(" ORDER BY CATEGORIA DESC ");
                                        break;
                                    }
                                case "status_Prospecto":     // Status_Prospecto
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.STATUS_PROSPECTO ");

                                        else
                                            sql.AppendLine(" ORDER BY A.STATUS_PROSPECTO DESC ");
                                        break;
                                    }
                                case "estado":     // Estado
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.ENDERECO_UF ");
                                        else
                                            sql.AppendLine(" ORDER BY B.ENDERECO_UF DESC ");
                                        break;
                                    }
                                case "cidade":     // Cidade
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.ENDERECO_CIDADE ");
                                        else
                                            sql.AppendLine(" ORDER BY B.ENDERECO_CIDADE DESC ");
                                        break;
                                    }
                                case "bairro":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY B.ENDERECO_BAIRRO ");
                                        else
                                            sql.AppendLine(" ORDER BY B.ENDERECO_BAIRRO DESC ");
                                        break;
                                    }
                                case "estado_Civil":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.ESTADO_CIVIL ");
                                        else
                                            sql.AppendLine(" ORDER BY A.ESTADO_CIVIL DESC ");
                                        break;
                                    }
                                case "data":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO ");

                                        else
                                            sql.AppendLine(" ORDER BY A.DATAHORACRIACAO DESC ");
                                        break;
                                    }
                                case "usuarioCriacao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY D.NOME ");

                                        else
                                            sql.AppendLine(" ORDER BY D.NOME DESC ");
                                        break;
                                    }
                                case "meioCaptcao":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY MEIOCAPTACAO ");

                                        else
                                            sql.AppendLine(" ORDER BY MEIOCAPTACAO DESC ");
                                        break;
                                    }
                                case "sexo":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY A.SEXO ");

                                        else
                                            sql.AppendLine(" ORDER BY A.SEXO DESC ");
                                        break;
                                    }
                                case "departamento":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY F.DESCRICAO ");

                                        else
                                            sql.AppendLine(" ORDER BY F.DESCRICAO DESC ");
                                        break;
                                    }
                                case "pais":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY E.NOME ");

                                        else
                                            sql.AppendLine(" ORDER BY E.NOME DESC ");
                                        break;
                                    }
                            }
                        }


                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {

                                long? idIndicacaoCli = reader["ID_QUEM_INDICOU"] != DBNull.Value ? Convert.ToInt64(reader["ID_QUEM_INDICOU"]) : (Int64?)null;
                                string nomeIndicacaoCli = Convert.ToString(reader["QINDICACAO"]);
                                string nomeIndicacao = idIndicacaoCli + " - " + nomeIndicacaoCli;
                                string outroMeioCapt = Convert.ToString(reader["OUTRO_MEIO"]);

                                if (!string.IsNullOrEmpty(outroMeioCapt))
                                {
                                    outroMeioCapt = int.TryParse(outroMeioCapt, out int value) ? "-" : outroMeioCapt;
                                }

                                retorno.Add(new Cliente_ProspectoListViewModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Numero_Contrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                                    Pendencia = false,
                                    Categoria = (Convert.ToString(reader["CATEGORIA"])),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    TipoPessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                    TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    TelefoneCelular = Convert.ToString(reader["TELEFONE_FIXO"]),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                    Pais = Convert.ToString(reader["PAIS"]),
                                    Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                    Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Cliente>(Convert.ToString(reader["STATUS"])),
                                    Login = Convert.ToString(reader["USERNAME"]),
                                    Status_Prospecto = HtmlEnumExtensions.GetEnumDescription<Status_Prospecto>(Convert.ToString(reader["STATUS_PROSPECTO"])),
                                    Sexo = HtmlEnumExtensions.GetEnumDescription<Sexo>(Convert.ToString(reader["SEXO"])),
                                    Estado_Civil = HtmlEnumExtensions.GetEnumDescription<Estado_Civil>(Convert.ToString(reader["ESTADO_CIVIL"])),
                                    Departamento = Convert.ToString(reader["DEPARTAMENTO"]),
                                    Data = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy") : String.Empty,
                                    UsuarioCriacao = Convert.ToString(reader["NOME"]),
                                    MeioCaptcao = Convert.ToString(reader["MEIOCAPTACAO"]),
                                    NomeIndicacao = nomeIndicacao,
                                    NomeCampanha = Convert.ToString(reader["CAMPANHA"]),
                                    OutroMeio = outroMeioCapt,
                                    
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
            }
            return new DataTableResponse<Cliente_ProspectoListViewModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = mensagem != String.Empty ? mensagem : ""
            };

        }



        //Page e retornando DataTable


        private async Task<Cliente_ProspectoDadosGeraisViewModel> GetViewObj(Cliente obj, ISession sessionNh, ISession sessionT5Nh)
        {
            var retorno = new Cliente_ProspectoDadosGeraisViewModel
            {
                Responsavel_Financeiro = obj.Nome_Resp_Financeiro,
                Telefone_Fixo2 = obj.Telefone_Fixo2,
                Site = obj.Site,
                Email_Aux = obj.Email_Aux,
                Informacao_Complementares = obj.Informacao_Complementares,
                Status_Prospecto = obj.Status_Prospecto,
                Id_Departamento = obj.Departamento?.Id,
                TipoCliente = obj.Tipo_Cliente,
                Cargo_Responsavel = obj.Cargo_Responsavel,
                CPFResponsavel = obj.Cpf_Responsavel,
                Representante = obj.Nome_Resp_Financeiro,
                Inscricao_Municipal = obj.Inscricao_Municipal,
                Inscricao_Estadual = obj.Inscricao_Estadual,
                Porte = obj.Porte,
                Data_Abertura = obj.Data_Abertura_Empresa,
                NomeFantasia = obj.NomeFantasia,
                Ramo_Atividade = obj.Ramo_Atividade,
                Razao_Social = obj.Razao_Social,
                NumeroValidado = obj.Numero_Validado,

                Numero_Contrato = obj.Numero_Contrato,

                Id = obj.Id,
                NomeCompleto = obj.NomeFantasia,
                DataNascimento = Convert.ToString(obj.Data_Nascimento?.ToString("dd/MM/yyyy")),
                Nacionalidade = obj.Nacionalidade,
                Sexo = obj.Sexo,
                EstadoCivil = obj.Estado_Civil,
                RG = obj.Rg,
                DataEmissao = Convert.ToString(obj.Data_Emissao_Rg),
                Orgao = obj.Orgao_Emissao_Rg,
                Instrucao = obj.Grau_Instrucao,
                Profissao = obj.Profissao,
                Pai = obj.Filiacao_Pai,
                Mae = obj.Filiacao_Mae,
                Qual_Meio = obj.Outro_Meio,
                TipoPessoa = obj.Tipo_Pessoa,
                IdMotivoInviabilidade = obj.Motivo_Inviabilidade?.Id,

                End_Cobranca_Igual_Principal = obj.End_Cobranca_Igual_Principal,

                Status = obj.Status == Status_Cliente.Ativo ? true : false,
                Email = obj.Email_Principal,
                TelefoneFixo = obj.Telefone_Fixo,
                Telefone_Celular = obj.Telefone_Celular,
                TelefoneRecado = obj.Telefone_Recado,
                NomeRecado = obj.Pessoa_Tel_Recado,
                Id_Campanha = obj.IdCampanha
            };

            if (obj.Tipo_Pessoa != null)
            {
                if (obj.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica)
                {
                    retorno.Cpf = obj.CpfCnpj;

                    retorno.Id_Quem_Indicou = obj.Quem_Indicou?.Id;
                    retorno.Quem_Indicou = obj.Quem_Indicou?.NomeFantasia;

                    retorno.Id_Meio_Captacao = obj.Qual_Meio?.Id;
                    retorno.Meio_Captacao = obj.Qual_Meio?.Descricao;

                    retorno.Id_Categoria_Cliente = obj.Categoria_Cliente?.Id;
                }
                else
                {
                    retorno.Cnpj = obj.CpfCnpj;

                    retorno.Id_Quem_IndicouJuridico = obj.Quem_Indicou?.Id;
                    retorno.Quem_IndicouJuridico = obj.Quem_Indicou?.NomeFantasia;

                    retorno.Id_Meio_CaptacaoJuridico = obj.Qual_Meio?.Id;
                    retorno.Meio_CaptacaoJuridico = obj.Qual_Meio?.Descricao;

                    retorno.Id_Categoria_ClienteJuridico = obj.Categoria_Cliente?.Id;
                }
            }

            obj.Endereco_Prospecto = obj.Endereco_Prospecto ?? obj.Endereco_Principal;

            retorno.EnderecoProspecto = new EnderecoClienteViewModel();

            retorno.EnderecoProspecto.Id_Pais = obj.Endereco_Prospecto?.Pais?.Id;
            retorno.EnderecoProspecto.Pais = obj.Endereco_Prospecto?.Pais?.Nome;
            retorno.EnderecoProspecto.Id_Tipo_Imovel = obj.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel;
            retorno.EnderecoProspecto.Id_Bairro = obj.Endereco_Prospecto?.Id_Bairro;
            retorno.EnderecoProspecto.Bairro = obj.Endereco_Prospecto?.Bairro;
            retorno.EnderecoProspecto.CEP = obj.Endereco_Prospecto?.Cep;
            retorno.EnderecoProspecto.Id_Cidade = obj.Endereco_Prospecto?.Id_Cidade;
            retorno.EnderecoProspecto.Cidade = obj.Endereco_Prospecto?.Cidade;
            retorno.EnderecoProspecto.Complemento = obj.Endereco_Prospecto?.Complemento;
            retorno.EnderecoProspecto.Id_Endereco = obj.Endereco_Prospecto != null ? obj.Endereco_Prospecto.Id : (Int64)0;
            retorno.EnderecoProspecto.Latitude = Convert.ToString(obj.Endereco_Prospecto?.Latitude);
            retorno.EnderecoProspecto.Logradouro = obj.Endereco_Prospecto?.Logradouro;
            retorno.EnderecoProspecto.Longitude = Convert.ToString(obj.Endereco_Prospecto?.Longitude);
            retorno.EnderecoProspecto.Numero = obj.Endereco_Prospecto?.Numero;
            retorno.EnderecoProspecto.Ponto_Referencia = obj.Endereco_Prospecto?.Ponto_Referencia;
            retorno.EnderecoProspecto.Tipo = Tipo_EnderecoCliente.Prospecto;
            retorno.EnderecoProspecto.TipoLogradouro = obj.Endereco_Prospecto?.Endereco_Logradouro?.Tipo;
            retorno.EnderecoProspecto.DescTipoLogradouro = obj.Endereco_Prospecto?.Endereco_Logradouro?.Tipo.EnumDescription();
            retorno.EnderecoProspecto.UF = obj.Endereco_Prospecto?.UF;
            retorno.EnderecoProspecto.Busca_Endereco = obj.Endereco_Prospecto?.Busca_Endereco != null ? obj.Endereco_Prospecto.Busca_Endereco : null;
            retorno.EnderecoProspecto.Id_Endereco_Logradouro = obj.Endereco_Prospecto?.Endereco_Logradouro?.Id;
            retorno.EnderecoProspecto.Id_Condominio_Predio = obj.Endereco_Prospecto?.Condominio_Predio?.Id;
            retorno.EnderecoProspecto.Id_Bloco = obj.Endereco_Prospecto?.Bloco;
            retorno.EnderecoProspecto.Id_Apartamento = obj.Endereco_Prospecto?.Apartamento;

            retorno.EnderecoProspecto.Cod_Escritorio = obj.Endereco_Prospecto?.Cod_Escritorio;
            retorno.EnderecoProspecto.Cod_Area = obj.Endereco_Prospecto?.Cod_Area;
            retorno.EnderecoProspecto.Cod_Localidade = Convert.ToString(obj.Endereco_Prospecto?.Cod_Localidade);


            #region Status das consultas de restrição

            //IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(sessionNh);
            //ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(sessionNh);
            //IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(sessionNh);
            //IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

            //var consultaCreditoBusiness =
            //    new ConsultaCreditoBusiness(
            //        repoConsultaCredito,
            //        repoCriterios,
            //        repoParamsConsulta,
            //        repoConfig
            //    );

            //IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
            //IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(sessionNh);

            //var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

            //IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
            //var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);


            //IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
            //IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
            //IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(sessionT5Nh);

            //using (var sessionMongo = new ConexaoMongoGeography())
            //{
            //    new AreaCoberturaMongoRepository(sessionMongo);
            //}

            //var consultaViabilidadeBusiness =
            //new ConsultaViabilidadeBusiness(
            //    repoConsultaViabilidade,
            //    repoElemRede,
            //    repoPorta,
            //    repoConfig
            //);

            //var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

            //Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

            //if (!String.IsNullOrEmpty(obj.CpfCnpj) && config.ConsultaCreditoExibeLead)
            //{
            //    if (retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica || retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica_Rural)
            //    {
            //        retorno.StatusConsultaCredito =
            //            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCpf(obj.CpfCnpj);
            //    }
            //    else
            //    {
            //        retorno.StatusConsultaCredito =
            //            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCnpj(obj.CpfCnpj);
            //    }
            //}

            //if (!String.IsNullOrEmpty(obj.CpfCnpj) && config.ConsultaPendenciaFinanceiraExibeLead)
            //{
            //    if (retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica || retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica_Rural)
            //    {
            //        retorno.StatusConsultaPendenciaFinanceira =
            //            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(obj.CpfCnpj);
            //    }
            //    else
            //    {
            //        retorno.StatusConsultaPendenciaFinanceira =
            //            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCnpj(obj.CpfCnpj);
            //    }
            //}
            var dataHoraCriacaoConsultaEndereco = new DateTime?();
            using (var _repoEtavaVenda = new Conexao<EtapaVenda>(sessionNh))
            {
                var listaEtapaVenda = await _repoEtavaVenda.select(x => x.Cliente.Id == retorno.Id);

                listaEtapaVenda.ToList().ForEach(x =>
                {
                    if (x.Etapa == EnumEtapaVenda.ConsultaViabilidade)
                    {
                        retorno.StatusConsultaViabilidade = x.Status;
                    }
                    else if (x.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira)
                    {
                        retorno.StatusConsultaPendenciaFinanceira = x.Status;
                    }
                    else if (x.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco)
                    {
                        retorno.StatusConsultaPendenciaEndereco = x.Status;
                        dataHoraCriacaoConsultaEndereco = x.DataHoraCriacao;
                    }
                    else if (x.Etapa == EnumEtapaVenda.ConsultaCredito)
                    {
                        retorno.StatusConsultaCredito = x.Status;
                    }
                });
            }

            IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
            var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);
            Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

            string logradouro;

            if (config.Utiliza_Cadastro_Endereco)
            {
                logradouro =
                    obj.Endereco_Prospecto?.Endereco_Logradouro != null ?
                         obj.Endereco_Prospecto.Endereco_Logradouro.Descricao :
                         obj.Endereco_Prospecto?.Condominio_Predio?.Descricao;
            }
            else
            {
                logradouro = retorno.EnderecoProspecto.Logradouro;
            }

            //retorno.StatusConsultaPendenciaEndereco =
            //    pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(logradouro, retorno.EnderecoProspecto.Numero);

            //retorno.StatusConsultaViabilidade =
            //    consultaViabilidadeBusiness.GetStatusConsultaViabilidade(
            //        (obj.Endereco_Prospecto?.Latitude).GetValueOrDefault(0),
            //        (obj.Endereco_Prospecto?.Longitude).GetValueOrDefault(0)
            //    );

            IEnumerable<EnumEtapaVenda> consultasHabilitadas = configBusiness.GetConsultasRestricaoHabilitadasLead();

            if (consultasHabilitadas.Count() > 0)
            {
                retorno.ConsultaViabilidadeHabilitada =
                    consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaViabilidade);

                retorno.ConsultaPendenciaFinanceiraHabilitada =
                    consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaFinanceira);

                retorno.ConsultaPendenciaEnderecoHabilitada =
                    consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaPendenciaEndereco);

                retorno.ConsultaCreditoHabilitada =
                    consultasHabilitadas.Any(consulta => consulta == EnumEtapaVenda.ConsultaCredito);
            }

            #region Visualizacao das Consultas pelo vendedor

            retorno.ConsultaViabilidadeVendedorVisualiza = config.ConsultaViabilidadeVendedorVisualiza;
            retorno.ConsultaCreditoVendedorVisualiza = config.ConsultaCreditoVendedorVisualiza;
            retorno.ConsultaPendenciaFinanceiraVendedorVisualiza = config.ConsultaPendenciaFinanceiraVendedorVisualiza;
            retorno.ConsultaPendenciaEnderecoVendedorVisualiza = config.ConsultaPendenciaEnderecoVendedorVisualiza;

            #endregion

            #endregion

            retorno.ConsultaPendenciaEnderecoValidade = config.ConsultaViabilidadeTempoValidade != null ? config.ConsultaViabilidadeTempoValidade : 0;
            retorno.ConsultaPendenciaViabilidadeValidade = config.ConsultaPendenciaEnderecoTempoValidade != null ? config.ConsultaPendenciaEnderecoTempoValidade : null;

            //var dataHoraCriacaoConsultaEndereco = pendenciaEnderecoBusiness.GetConsultaPendenciaEndereco(obj.Endereco_Prospecto?.Logradouro, obj.Endereco_Prospecto?.Numero)?.DataHoraCriacao;
            retorno.DataHoraCriacaoConsultaEndereco = dataHoraCriacaoConsultaEndereco != null ? dataHoraCriacaoConsultaEndereco : (DateTime?)null;
            retorno.End_CadastroCondominioProspecto = obj.End_CadastroCondominioProspecto;

            var cliVincEndereco = await new ClienteApiController().HasCliEndVinculado(new EnderecoCliCheck
            {
                CEP = retorno.EnderecoProspecto.CEP,
                Logradouro = retorno.EnderecoProspecto.Logradouro,
                Bloco = $"{retorno.EnderecoProspecto.Id_Bloco}",
                Apartamento = $"{retorno.EnderecoProspecto.Id_Apartamento}",
                Numero = retorno.EnderecoProspecto.Numero,
                Id_Condominio_Predio = retorno.EnderecoProspecto.Id_Condominio_Predio.HasValue ? retorno.EnderecoProspecto.Id_Condominio_Predio.Value : (Int64?)null,
                Id_Endereco_Logradouro = retorno.EnderecoProspecto.Id_Endereco_Logradouro.HasValue ? retorno.EnderecoProspecto.Id_Endereco_Logradouro.Value : (Int64?)null,
            });

            if (cliVincEndereco.Ok)
            {
                retorno.ClientesAtrelados = cliVincEndereco.IdObjeto;
            }

            return retorno;
        }

        private async void BindViewToObj(ISession sessionNh, ISession sessionT5Nh, Cliente objBanco, Cliente_ProspectoDadosGeraisViewModel view)
        {
            using (var configDao = new Conexao<Configuracoes_Gerais>(sessionNh))
            using (var clienteDao = new Conexao<Cliente>(sessionNh))
            using (var meioCaptacaoDao = new Conexao<Meio_Captacao>(sessionNh))
            using (var categoriaClienteDao = new Conexao<Categoria_Cliente>(sessionNh))
            using (var paisDao = new Conexao<Pais>(sessionNh))
            using (var tipoImovelDao = new Conexao<Tipo_Imovel>(sessionNh))
            using (var motivoInviabilidadeDao = new Conexao<Motivo_Inviabilidade>(sessionNh))
            using (var deptoDao = new Conexao<Departamento>(sessionNh))
            using (var enderecoLogradouroDao = new Conexao<Endereco_Logradouro>(sessionNh))
            using (var condominioPredioDao = new Conexao<Condominio_Predio>(sessionNh))
            using (var sessionMongo = new ConexaoMongoGeography())
            {
                objBanco.Nome_Resp_Financeiro = view.Responsavel_Financeiro;
                objBanco.Telefone_Fixo2 = view.Telefone_Fixo2;
                objBanco.Site = view.Site;
                objBanco.Email_Aux = view.Email_Aux;
                objBanco.Departamento = view.Id_Departamento.HasValue ? deptoDao.selectFirst(p => p.Id == view.Id_Departamento).Result : null;
                objBanco.Cargo_Responsavel = view.Cargo_Responsavel;
                objBanco.Cpf_Responsavel = String.IsNullOrEmpty(view.CPFResponsavel) ? String.Empty : new String(view.CPFResponsavel.Where(Char.IsDigit).ToArray());
                objBanco.Inscricao_Municipal = view.Inscricao_Municipal;
                objBanco.Inscricao_Estadual = view.Inscricao_Estadual;
                objBanco.Razao_Social = view.Razao_Social;
                objBanco.Ramo_Atividade = view.Ramo_Atividade;
                objBanco.Porte = view.Porte;
                objBanco.Data_Abertura_Empresa = view.Data_Abertura;
                objBanco.End_CadastroCondominioProspecto = view.End_CadastroCondominioProspecto;
                objBanco.IdCampanha = view.Id_Campanha.HasValue ? view.Id_Campanha.Value : (Int64?)null;
                objBanco.Tipo_Pessoa = view.TipoPessoa;
                objBanco.Informacao_Complementares = view.Informacao_Complementares;
                objBanco.NomeFantasia =
                    objBanco.Tipo_Pessoa == Tipo_Pessoa.Fisica ? view.NomeCompleto : view.NomeFantasia;

                if (objBanco.Tipo_Pessoa == Tipo_Pessoa.Fisica)
                {
                    if (!String.IsNullOrEmpty(view.Cpf))
                    {
                        objBanco.CpfCnpj = view.Cpf.Replace("-", String.Empty)
                                                   .Replace(".", String.Empty);
                    }

                    objBanco.Qual_Meio = view.Id_Meio_Captacao.HasValue ? meioCaptacaoDao.selectFirst(p => p.Id == view.Id_Meio_Captacao.Value).Result : null;

                    objBanco.Categoria_Cliente = view.Id_Categoria_Cliente.HasValue ? categoriaClienteDao.selectFirst(p => p.Id == view.Id_Categoria_Cliente.Value).Result : null;
                    objBanco.Quem_Indicou = view.Id_Quem_Indicou.HasValue ? clienteDao.selectFirst(p => p.Id == view.Id_Quem_Indicou.Value).Result : null;

                    objBanco.Outro_Meio = objBanco.Qual_Meio?.Id == 0 ? view.Qual_Meio : null;

                    objBanco.Motivo_Inviabilidade = motivoInviabilidadeDao.selectFirst(p => p.Id == view.IdMotivoInviabilidade).Result;

                }
                else
                {
                    if (!String.IsNullOrEmpty(view.Cnpj))
                    {
                        objBanco.CpfCnpj = view.Cnpj.Replace("-", String.Empty)
                                                    .Replace(".", String.Empty)
                                                    .Replace("/", String.Empty);
                    }

                    objBanco.Qual_Meio = view.Id_Meio_CaptacaoJuridico.HasValue ? meioCaptacaoDao.selectFirst(p => p.Id == view.Id_Meio_CaptacaoJuridico.Value).Result : null;

                    objBanco.Categoria_Cliente = view.Id_Categoria_ClienteJuridico.HasValue ? categoriaClienteDao.selectFirst(p => p.Id == view.Id_Categoria_ClienteJuridico.Value).Result : null;
                    objBanco.Quem_Indicou = view.Id_Quem_IndicouJuridico.HasValue ? clienteDao.selectFirst(p => p.Id == view.Id_Quem_IndicouJuridico.Value).Result : null;

                    objBanco.Outro_Meio = objBanco.Qual_Meio?.Id == 0 ? view.Qual_MeioJuridico : null;
                }

                objBanco.Data_Nascimento = !String.IsNullOrEmpty(view.DataNascimento) ? DateTime.Parse(view.DataNascimento) : (DateTime?)null;
                objBanco.Nacionalidade = view.Nacionalidade;
                objBanco.Sexo = view.Sexo;

                objBanco.Estado_Civil = view.EstadoCivil;
                objBanco.Rg = view.RG;
                objBanco.Data_Emissao_Rg = !String.IsNullOrEmpty(view.DataEmissao) ? DateTime.ParseExact(view.DataEmissao, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
                objBanco.Orgao_Emissao_Rg = view.Orgao;
                objBanco.Grau_Instrucao = view.Instrucao;
                objBanco.Filiacao_Mae = view.Mae;
                objBanco.Filiacao_Pai = view.Pai;
                objBanco.Profissao = view.Profissao;

                // no prospecto o nome da propriedade se mantém numero de contato (nao possui conflito com o numero de contrato do plano/produto)
                objBanco.Numero_Contrato = (configDao.selectFirst().Result).Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato ? view.Numero_Contrato :
                                           (view.Id == 0 || String.IsNullOrEmpty(objBanco.Numero_Contrato) ? Convert.ToString(clienteDao.MaxString("NUMERO_CONTRATO", "TB_CLIENTE").Result + 1) : view.Numero_Contrato);


                objBanco.Email_Principal = view.Email;
                objBanco.Telefone_Fixo = view.TelefoneFixo;
                objBanco.Telefone_Celular = view.Telefone_Celular;
                objBanco.Telefone_Celular = view.Telefone_Celular;
                objBanco.Telefone_Recado = view.TelefoneRecado;
                objBanco.Pessoa_Tel_Recado = view.NomeRecado;
                objBanco.Telefone_Fixo2 = view.Telefone_Fixo2;

                // ****************************************
                // Endereço Principal
                // ****************************************

                if (objBanco.Endereco_Prospecto == null) objBanco.Endereco_Prospecto = new Endereco_Cliente();
                if (objBanco.Id == 0) objBanco.Endereco_Prospecto.Id = 0;
                objBanco.Endereco_Prospecto.Pais = view.EnderecoProspecto.Id_Pais != null ? paisDao.selectFirst(p => p.Id == view.EnderecoProspecto.Id_Pais.Value).Result : null;
                objBanco.Endereco_Prospecto.Tipo_Imovel = view.EnderecoProspecto.Id_Tipo_Imovel != null ? tipoImovelDao.selectFirst(p => p.Id == Convert.ToInt32(view.EnderecoProspecto.Id_Tipo_Imovel.Value)).Result : null;
                objBanco.Endereco_Prospecto.Bairro = view.EnderecoProspecto?.Bairro;
                objBanco.Endereco_Prospecto.Id_Bairro = view.EnderecoProspecto?.Id_Bairro;
                objBanco.Endereco_Prospecto.Cep = view.EnderecoProspecto?.CEP;
                objBanco.Endereco_Prospecto.Cidade = view.EnderecoProspecto?.Cidade;
                objBanco.Endereco_Prospecto.Id_Cidade = view.EnderecoProspecto?.Id_Cidade;
                objBanco.Endereco_Prospecto.Complemento = view.EnderecoProspecto?.Complemento;
                objBanco.Endereco_Prospecto.Latitude = !String.IsNullOrEmpty(view.EnderecoProspecto?.Latitude) ? Convert.ToDouble(view.EnderecoProspecto.Latitude?.Replace('.', ',')) : (Double?)null;
                objBanco.Endereco_Prospecto.Longitude = !String.IsNullOrEmpty(view.EnderecoProspecto?.Longitude) ? Convert.ToDouble(view.EnderecoProspecto?.Longitude?.Replace('.', ',')) : (Double?)null;
                objBanco.Endereco_Prospecto.Logradouro = view.EnderecoProspecto?.Logradouro;
                objBanco.Endereco_Prospecto.Numero = view.EnderecoProspecto?.Numero;
                objBanco.Endereco_Prospecto.Ponto_Referencia = view.EnderecoProspecto?.Ponto_Referencia;
                objBanco.Endereco_Prospecto.UF = view.EnderecoProspecto?.UF;

                objBanco.Endereco_Prospecto.Cod_Escritorio = view.EnderecoProspecto?.Cod_Escritorio;
                objBanco.Endereco_Prospecto.Cod_Area = view.EnderecoProspecto?.Cod_Area;
                objBanco.Endereco_Prospecto.Cod_Localidade =
                    !String.IsNullOrEmpty(view.EnderecoProspecto?.Cod_Localidade) ?
                        Convert.ToInt32(view.EnderecoProspecto.Cod_Localidade) :
                        (int?)null;


                objBanco.Endereco_Prospecto.Busca_Endereco = view.EnderecoProspecto.Busca_Endereco;
                objBanco.Endereco_Prospecto.Condominio_Predio = condominioPredioDao.selectFirst(p => p.Id == view.EnderecoProspecto.Id_Condominio_Predio).Result;
                objBanco.Endereco_Prospecto.Bloco = view.EnderecoProspecto.Id_Bloco;
                objBanco.Endereco_Prospecto.Apartamento = view.EnderecoProspecto.Id_Apartamento;

                if (view.EnderecoProspecto.Id_Endereco_Logradouro.HasValue)
                {
                    objBanco.Endereco_Prospecto.Busca_Endereco = Busca_Endereco.Endereco_Logradouro;
                    objBanco.Endereco_Prospecto.Endereco_Logradouro = enderecoLogradouroDao.selectFirst(p => p.Id == view.EnderecoProspecto.Id_Endereco_Logradouro).Result;

                    objBanco.Endereco_Prospecto.Endereco_Logradouro.Cod_Escritorio = !String.IsNullOrEmpty(objBanco.Endereco_Prospecto?.Cod_Escritorio) ? objBanco.Endereco_Prospecto.Cod_Escritorio : "";
                    objBanco.Endereco_Prospecto.Endereco_Logradouro.Cod_Area = !String.IsNullOrEmpty(objBanco.Endereco_Prospecto?.Cod_Area) ? objBanco.Endereco_Prospecto.Cod_Area : "";
                    objBanco.Endereco_Prospecto.Endereco_Logradouro.Cod_Localidade = objBanco.Endereco_Prospecto.Cod_Localidade.HasValue ? Convert.ToString(objBanco.Endereco_Prospecto.Cod_Localidade) : "";
                }

                #region Status das consultas de restrição

                //IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(sessionNh);
                //ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(sessionNh);
                //IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(sessionNh);
                //IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                //var consultaCreditoBusiness =
                //    new ConsultaCreditoBusiness(
                //        repoConsultaCredito,
                //        repoCriterios,
                //        repoParamsConsulta,
                //        repoConfig
                //    );

                //IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                //IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(sessionNh);
                //IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);

                //var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                //IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                //var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                //IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                //IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                //IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(sessionT5Nh);


                //var consultaViabilidadeBusiness =
                //    new ConsultaViabilidadeBusiness(
                //        repoConsultaViabilidade,
                //        repoElemRede,
                //        repoPorta,
                //        repoConfig
                //    );

                //var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                //Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                //StatusEtapaVenda statusConsultaViabilidade = StatusEtapaVenda.Pendente;
                //StatusEtapaVenda statusConsultaPendenciaEndereco = StatusEtapaVenda.Pendente;
                //StatusEtapaVenda statusConsultaPendenciaFinanceira = StatusEtapaVenda.Pendente;
                //StatusEtapaVenda statusConsultaCredito = StatusEtapaVenda.Pendente;

                //if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaCreditoExibeLead)
                //{
                //    if (objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica || objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica_Rural)
                //    {
                //        statusConsultaCredito =
                //            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCpf(objBanco.CpfCnpj);
                //    }
                //    else
                //    {
                //        statusConsultaCredito =
                //            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCnpj(objBanco.CpfCnpj);
                //    }
                //}

                //if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaPendenciaFinanceiraExibeLead)
                //{
                //    if (objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica || objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica_Rural)
                //    {
                //        statusConsultaPendenciaFinanceira =
                //            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(objBanco.CpfCnpj);
                //    }
                //    else
                //    {
                //        statusConsultaPendenciaFinanceira =
                //            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCnpj(objBanco.CpfCnpj);
                //    }
                //}

                //string logradouro;

                //if (config.Utiliza_Cadastro_Endereco)
                //{
                //    logradouro =
                //        objBanco.Endereco_Prospecto?.Endereco_Logradouro != null ?
                //             objBanco.Endereco_Prospecto.Endereco_Logradouro.Descricao :
                //             objBanco.Endereco_Prospecto?.Condominio_Predio?.Descricao;
                //}
                //else
                //{
                //    logradouro = objBanco.Endereco_Prospecto.Logradouro;
                //}

                //statusConsultaPendenciaEndereco =
                //    pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(logradouro, objBanco.Endereco_Prospecto.Numero);

                //statusConsultaViabilidade =
                //    consultaViabilidadeBusiness.GetStatusConsultaViabilidade(
                //        (objBanco.Endereco_Prospecto?.Latitude).GetValueOrDefault(0),
                //        (objBanco.Endereco_Prospecto?.Longitude).GetValueOrDefault(0)
                //    );

                #endregion

                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);
                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                var statusConsultaViabilidade = view.StatusConsultaViabilidade;
                var statusConsultaPendenciaEndereco = view.StatusConsultaPendenciaEndereco;
                var statusConsultaPendenciaFinanceira = view.StatusConsultaPendenciaFinanceira;
                var statusConsultaCredito = view.StatusConsultaCredito;

                bool restricoesImpedemQualificacaoLead =
                    (config.ConsultaViabilidadeQualificaLead && config.ConsultaViabilidadeExibeLead && statusConsultaViabilidade != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaPendenciaEnderecoQualificaLead && config.ConsultaPendenciaEnderecoExibeLead && statusConsultaPendenciaEndereco != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaPendenciaFinanceiraQualificaLead && config.ConsultaPendenciaFinanceiraExibeLead && statusConsultaPendenciaFinanceira != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaCreditoQualificaLead && config.ConsultaCreditoExibeLead && statusConsultaCredito != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente);

                if (!restricoesImpedemQualificacaoLead)
                {
                    objBanco.Status_Prospecto =
                        !String.IsNullOrEmpty(objBanco.CpfCnpj) ?
                            Status_Prospecto.Lead_Qualificado :
                            Status_Prospecto.Lead_Nao_Qualificado;
                }
                else
                {
                    objBanco.Status_Prospecto = Status_Prospecto.Lead_Com_Restricao;
                }
            }
        }

        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/FindById")]
        public async Task<GetApiRetorno<Cliente_ProspectoDadosGeraisViewModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Cliente_ProspectoDadosGeraisViewModel> retorno = new GetApiRetorno<Cliente_ProspectoDadosGeraisViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Cliente>(session))
            {
                try
                {
                    Cliente lead = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = await GetViewObj(lead, session, sessionT5);

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
        [Route("api/Cliente_ProspectoApi/ConsultarDocumentoSERPRO")]
        public async Task<PostApiRetorno<ConsultarDocResponse>> ConsultarDocumentoSERPRO(ConsultarDocRequest request)
        {
            PostApiRetorno<ConsultarDocResponse> response = new PostApiRetorno<ConsultarDocResponse>();

            try
            {
                string auth;
                using (var session = HibernateUtil.GetSession())
                using (var repoConfigGeral = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config = await repoConfigGeral.selectFirst();

                    if (config == null)
                        throw new Exception("Configuração da Empresa não encontrada");

                    if (request.TipoPessoa == Tipo_Pessoa.Fisica)
                    {
                        String _cpf = request.CPF.Replace(".", String.Empty).Replace("-", String.Empty);

                        if (new ClienteApiController().Existecpf(_cpf).Result > 0)
                            throw new Exception(String.Format("O CPF {0} já se encontra cadastrado", _cpf));

                        if (CONSTANTES.AmbienteTestesSERPRO == "N")
                            auth = await new AutenticacaoService(config.ConsumerKeyCPF, config.ConsumerSecretCPF).ObterTokenAutorizacao();
                        else
                            auth = CONSTANTES.TokenConsultaTrial;

                        ConsultaService consultaService = new ConsultaService(auth);

                        var responseConsulta = await consultaService.ObterCadastroCPF(_cpf);

                        if (responseConsulta is null)
                            throw new Exception("Erro em realizar consulta com a API do SERPRO");

                        var situacao = consultaService.ConsultarSituacaoFisica(responseConsulta.situacao.codigo);

                        if (situacao != SituacaoFisicaEnum.Regular)
                        {
                            response.IdObjeto = null;
                            response.Ok = false;
                            response.Mensagem = $"Lead não se encontra válido, situação: {HtmlEnumExtensions.GetEnumDescription(situacao)}";
                        }
                        else
                        {
                            string dataFormatada = responseConsulta.nascimento.Insert(2, "/").Insert(5, "/").ToString();

                            response.IdObjeto = new ConsultarDocResponse
                            {
                                nascimento = DateTime.Parse(dataFormatada).Date.ToString(),
                                nome = responseConsulta.nome,
                                tipoPessoa = 0
                            };
                            response.Ok = true;
                            response.Mensagem = $"Lead encontra-se em situação REGULAR!";
                        }
                    }

                    else if (request.TipoPessoa == Tipo_Pessoa.Juridica)
                    {
                        String _cnpj = request.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);

                        if (new ClienteApiController().Existecpf(_cnpj).Result > 0)
                            throw new Exception(String.Format("O CNPJ {0} já se encontra cadastrado", _cnpj));

                        if (CONSTANTES.AmbienteTestesSERPRO == "N")
                            auth = await new AutenticacaoService(config.ConsumerKeyCNPJ, config.ConsumerSecretCNPJ).ObterTokenAutorizacao();
                        else
                            auth = CONSTANTES.TokenConsultaTrial;

                        ConsultaService consultaService = new ConsultaService(auth);

                        var responseConsulta = await consultaService.ObterCadastroCNPJ(_cnpj);

                        if (responseConsulta is null)
                            throw new Exception("Erro em realizar consulta com a API do SERPRO");

                        var situacao = consultaService.ConsultarSituacaoJuridica(responseConsulta.situacaoCadastral.codigo);

                        if (situacao != SituacaoJuridicaEnum.Ativa)
                        {
                            response.IdObjeto = null;
                            response.Ok = false;
                            response.Mensagem = $"Lead não se encontra válido, situação: {HtmlEnumExtensions.GetEnumDescription(situacao)}";
                        }
                        else
                        {
                            response.IdObjeto = new ConsultarDocResponse
                            {
                                dataAbertura = DateTime.Parse(responseConsulta.dataAbertura).Date.ToString(),
                                nomeFantasia = responseConsulta.nomeFantasia,
                                razaoSocial = responseConsulta.nomeEmpresarial,
                                tipoPessoa = 1
                            };
                            response.Ok = true;
                            response.Mensagem = $"Lead encontra-se em situação REGULAR!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
                response.IdObjeto = null;
                return response;
            }
            return response;
        }

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Cliente_ProspectoDadosGeraisViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            int count = 0;

            if (!String.IsNullOrEmpty(value.Objeto.Cpf))
                count = existecpf(value.Objeto.Cpf).Result;
            else if (!String.IsNullOrEmpty(value.Objeto.Cnpj))
                count = existecpf(value.Objeto.Cnpj).Result;

            try
            {
                if (count > 0 && value.Objeto.Id == 0)
                {
                    throw new Exception(Resources.RsCadastros.CPFCNPJ_CADASTRADO);
                }

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    retorno.IdObjeto = await GravarProspecto(session, sessionT5, value);
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

        public async Task<Int64> GravarProspecto(ISession sessionNh, ISession sessionT5Nh, ParamWebApi<Cliente_ProspectoDadosGeraisViewModel> value)
        {
            Cliente modelo = null;
            Forma_Pagamento_Cliente forma_Pagamento_Cliente = new Forma_Pagamento_Cliente();
            using (var repository = new Conexao<Cliente>(sessionNh))
            using (var repoEtapaVenda = new Conexao<EtapaVenda>(sessionNh))
            {
                if (value.Objeto.Id != 0)
                    modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                if (modelo == null)
                {
                    modelo = new Cliente
                    {
                        Status = Status_Cliente.Ativo,

                        DataHoraCriacao = DateTime.Now,
                        UsuarioCriacao = value.Usuario,

                        Tipo_Cliente = Tipo_Cliente.Prospecto
                    };
                }
                else
                {
                    modelo.Status = value.Objeto.Status ? Status_Cliente.Ativo : Status_Cliente.Inativo;

                    modelo.DataHoraAlteracao = DateTime.Now;
                    modelo.UsuarioAlteracao = value.Usuario;
                }

                BindViewToObj(sessionNh, sessionT5Nh, modelo, value.Objeto);

                try { 
                    await repository.SaveOrUpdate(modelo);
                }catch (Exception ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? "";
                    if (innerMessage.Contains("unavailable database"))
                    {
                        throw new Exception("Não foi possível acessar a base de dados do T5.\r\nVerifique se o caminho configurado está correto na tabela de Configurações Gerais.");
                    }
                    else
                    {
                        throw new Exception(innerMessage);
                    }
                }
                

                if (modelo != null)
                {
                    var listaEtapaVenda = new List<EtapaVenda>();
                    var clienteId = value.Objeto.Id > 0 ? value.Objeto.Id : 0;
                    var listaEtapaVendaTmp = (await repoEtapaVenda.select(p => p.Cliente.Id == clienteId)).ToList();

                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);
                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                    for (int index = 0; index < listaEtapaVendaTmp.Count; index++)
                    {
                        var Id = listaEtapaVendaTmp[index].Id;
                        var obj = await repoEtapaVenda.selectFirst(x => x.Id == Id);

                        obj.DataHoraAlteracao = DateTime.Now;
                        obj.UsuarioAlteracao = value.Usuario;
                        obj.Cliente = modelo;

                        if (EnumEtapaVenda.ConsultaViabilidade == obj.Etapa && config.ConsultaViabilidadeExibeLead)
                        {
                            obj.Status = value.Objeto.StatusConsultaViabilidade;
                        }
                        else if (EnumEtapaVenda.ConsultaPendenciaFinanceira == obj.Etapa && config.ConsultaPendenciaFinanceiraExibeLead)
                        {
                            obj.Status = value.Objeto.StatusConsultaPendenciaFinanceira;
                        }
                        else if (EnumEtapaVenda.ConsultaPendenciaEndereco == obj.Etapa && config.ConsultaPendenciaEnderecoExibeLead)
                        {
                            obj.Status = value.Objeto.StatusConsultaPendenciaEndereco;
                        }
                        else if (EnumEtapaVenda.ConsultaCredito == obj.Etapa && config.ConsultaCreditoExibeLead)
                        {
                            obj.Status = value.Objeto.StatusConsultaCredito;
                        }

                        listaEtapaVenda.Add(obj);
                    }

                    if (listaEtapaVenda.Where(x => x.Etapa == EnumEtapaVenda.ConsultaViabilidade).Count() == 0 && config.ConsultaViabilidadeExibeLead)
                    {
                        var etapa_venda = new EtapaVenda()
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = value.Usuario,
                            Cliente = modelo,
                            Etapa = EnumEtapaVenda.ConsultaViabilidade,
                            Descricao = "Consulta de Viabilidade",
                            Status = value.Objeto.StatusConsultaViabilidade
                        };
                        listaEtapaVenda.Add(etapa_venda);
                    }

                    if (listaEtapaVenda.Where(x => x.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira).Count() == 0 && config.ConsultaPendenciaFinanceiraExibeLead)
                    {
                        var etapa_venda = new EtapaVenda()
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = value.Usuario,
                            Cliente = modelo,
                            Etapa = EnumEtapaVenda.ConsultaPendenciaFinanceira,
                            Descricao = "Consulta de Pendência Financeira",
                            Status = value.Objeto.StatusConsultaPendenciaFinanceira
                        };
                        listaEtapaVenda.Add(etapa_venda);
                    }

                    if (listaEtapaVenda.Where(x => x.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco).Count() == 0 && config.ConsultaPendenciaEnderecoExibeLead)
                    {
                        var etapa_venda = new EtapaVenda()
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = value.Usuario,
                            Cliente = modelo,
                            Etapa = EnumEtapaVenda.ConsultaPendenciaEndereco,
                            Descricao = "Consulta Pendência Endereço",
                            Status = value.Objeto.StatusConsultaPendenciaEndereco
                        };
                        listaEtapaVenda.Add(etapa_venda);
                    }

                    if (listaEtapaVenda.Where(x => x.Etapa == EnumEtapaVenda.ConsultaCredito).Count() == 0 && config.ConsultaCreditoExibeLead)
                    {
                        var etapa_venda = new EtapaVenda()
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = value.Usuario,
                            Cliente = modelo,
                            Etapa = EnumEtapaVenda.ConsultaCredito,
                            Descricao = "Consulta de Crédito",
                            Status = value.Objeto.StatusConsultaCredito
                        };
                        listaEtapaVenda.Add(etapa_venda);
                    }

                    await repoEtapaVenda.SaveOrUpdate(listaEtapaVenda);
                }

                return modelo.Id;
            }
        }

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/AlterarStatus")]
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
                        var lista_temp = value.ids.ToList();
                        foreach (var id in value.ids)
                        {
                            if (await VerificarVendaProspecto(id))
                            {
                                lista_temp.Remove(id);
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Mensagem = "prospecto possui venda em andamento",
                                    Tipo = PostApiMensagemType.Erro,
                                });

                            }
                        }

                        value.ids = lista_temp;



                        if (value.ids.Count > 0)
                        {
                            String _ids = $"({String.Join(",", value.ids)})";

                            StringBuilder sql = new StringBuilder($"UPDATE TB_CLIENTE SET STATUS = {value.status} WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                            retorno.Ok = true;
                        }

                        else
                        {
                            retorno.Ok = false;
                        }
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

        public async Task<Boolean> VerificarVendaProspecto(Int64? id_cliente)
        {
            Boolean retorno = new Boolean();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sqlFrom = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_VENDA A ");
                    sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE B ");
                    sqlFrom.AppendLine("ON (A.ID_CLIENTE = @CLIENTE_ID) ");
                    sqlFrom.AppendLine("WHERE B.STATUS IN (0,5) AND B.TIPO_CLIENTE = 1 ");

                    parametros.Add("@CLIENTE_ID", id_cliente);

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlFrom, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = Convert.ToBoolean(reader["CO"]);
                        }
                    }
                }

                return retorno;

            }

        }

        #region Endereco
        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/FindByIdEndereco")]
        public async Task<GetApiRetorno<AlterarEnderecoClienteViewModel>> FindByIdEndereco(Int64 Id)
        {
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = new GetApiRetorno<AlterarEnderecoClienteViewModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Endereco_Cliente>(session))
            {
                try
                {
                    retorno.Objeto = new AlterarEnderecoClienteViewModel
                    {
                        EnderecoInstalacao = GetViewObj(await repository.selectFirst(p => p.Id == Id))
                    };

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
        [Route("api/Cliente_ProspectoApi/GravarEndereco")]
        public async Task<PostApiRetorno<Int64>> GravarEndereco([FromBody] ParamWebApi<EnderecoClienteViewModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Endereco_Cliente modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Endereco_Cliente>(session))
                    {
                        if (value.Objeto.Id_Endereco != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id_Endereco)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Endereco_Cliente
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        retorno.IdObjeto = modelo.Id;
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




        /// <summary>
        /// Bind de Endereço
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objBanco"></param>
        /// <param name="view"></param>
        private void BindViewToObj(ISession session, Endereco_Cliente objBanco, EnderecoClienteViewModel view)
        {
            using (var repoEnderecoLogradouro = new Conexao<Endereco_Logradouro>(session))
            using (var repoCondominioPredio = new Conexao<Condominio_Predio>(session))
            {
                objBanco.Condominio_Predio = view.Busca_Endereco == Busca_Endereco.Condominio_Predio ? repoCondominioPredio.selectFirst(p => p.Id == view.Id_Condominio_Predio).Result : null;
                objBanco.Endereco_Logradouro = view.Busca_Endereco == Busca_Endereco.Endereco_Logradouro ? repoEnderecoLogradouro.selectFirst(p => p.Id == view.Id_Endereco_Logradouro).Result : null;
            }
            objBanco.Bairro = view.Bairro;
            objBanco.Cep = view.CEP;
            objBanco.Cidade = view.Cidade;
            objBanco.Complemento = view.Complemento;
            objBanco.Id = view.Id_Endereco;
            objBanco.Id_Bairro = view.Id_Bairro;
            objBanco.Id_Cidade = view.Id_Cidade;
            objBanco.Latitude = view.Latitude != null ? Convert.ToDouble(view.Latitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Longitude = view.Longitude != null ? Convert.ToDouble(view.Longitude?.Replace('.', ',')) : (Double?)null;
            objBanco.Logradouro = view.Logradouro;
            objBanco.Numero = view.Numero;
            objBanco.Ponto_Referencia = view.Ponto_Referencia;
            objBanco.UF = view.UF;
            objBanco.Busca_Endereco = view.Busca_Endereco != null ? view.Busca_Endereco : null;

        }

        private EnderecoClienteViewModel GetViewObj(Endereco_Cliente obj)
        {
            return new EnderecoClienteViewModel
            {
                Bairro = obj.Bairro,
                CEP = obj.Cep,
                Cidade = obj.Cidade,
                Complemento = obj.Complemento,
                Id_Bairro = obj.Id_Bairro,
                Id_Cidade = obj.Id_Cidade,
                Id_Endereco = obj.Id,
                Latitude = Convert.ToString(obj.Latitude),
                Logradouro = obj.Logradouro,
                Longitude = Convert.ToString(obj.Longitude),
                Numero = obj.Numero,
                Ponto_Referencia = obj.Ponto_Referencia,
                Tipo = Tipo_EnderecoCliente.Instalacao,
                UF = obj.UF,
                Busca_Endereco = obj?.Busca_Endereco != null ? obj?.Busca_Endereco : null,
                Id_Endereco_Logradouro = obj?.Endereco_Logradouro?.Id,
                Id_Condominio_Predio = obj?.Condominio_Predio?.Id
            };
        }

        #endregion

        #region Anexos
        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/GetAnexos")]
        public async Task<GetApiRetorno<List<ClienteAnexoModelView>>> GetAnexos(Int64 Id_Cliente)
        {
            GetApiRetorno<List<ClienteAnexoModelView>> retorno = new GetApiRetorno<List<ClienteAnexoModelView>>
            {
                Objeto = new List<ClienteAnexoModelView>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@ID_CLIENTE", Convert.ToInt64(Id_Cliente) }
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.TITULO, A.NOME_ARQUIVO, A.EXTENSAO_ARQUIVO, A.ID_CLIENTE");
                        sql.AppendLine("FROM TB_CLIENTE_ANEXO A");
                        sql.AppendLine("WHERE A.ID_CLIENTE = @ID_CLIENTE");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new ClienteAnexoModelView
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Nome_Arquivo = Convert.ToString(reader["NOME_ARQUIVO"]),
                                    Titulo = Convert.ToString(reader["TITULO"]),
                                    Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
                                    Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"])
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/GravarAnexo")]
        public async Task<PostApiRetorno<Int64>> GravarAnexo([FromBody] ParamWebApi<ClienteAnexoModelView> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                Cliente_Anexo modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cliente_Anexo>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Cliente_Anexo
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        retorno.IdObjeto = modelo.Id;
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
        [Route("api/Cliente_ProspectoApi/ExcluirAnexo")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirAnexo(Int64 Id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cliente_Anexo>(session))
                    {
                        Cliente_Anexo modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();

                        try
                        {
                            String caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{modelo.Cliente.Id}\{modelo.Id}.{modelo.Extensao_Arquivo}";

                            if (File.Exists(caminhoArquivo))
                                File.Delete(caminhoArquivo);
                        }
                        catch { }

                        await repository.Remove(modelo);

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
        [Route("api/cliente_prospectoapi/existecpf")]
        public async Task<Int32> existecpf(String cpf)
        {

            Int32 recordsTotal = 0;

            if (String.IsNullOrEmpty(cpf))
                return recordsTotal;

            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
            cpf = cpf.Replace("/", "");
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT COUNT(A.ID) CO ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE (A.CPF = @VALOR) ");
                        parametros.Add("@VALOR", cpf);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                recordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return recordsTotal;
        }


        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/busca")]
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
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA ");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR)");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
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
        #region BuscaQuemIndicouMobile

        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/buscaQuemIndicouMobile")]
        public async Task<GetApiRetorno<List<Object>>> buscaQuemIndicouMobile(String cpf_nome)
        {
            GetApiRetorno<List<Object>> retorno = new GetApiRetorno<List<Object>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.CPF, A.NOMEFANTASIA");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (UPPER(A.cpf || A.NOMEFANTASIA) LIKE @VALOR)");
                        parametros.Add("@VALOR", $"%{cpf_nome.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            retorno.Objeto = new List<Object>();
                            while (await reader.ReadAsync())
                            {
                                retorno.Objeto.Add(new
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"])
                                });
                            }
                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }

            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/buscaQuemIndicou")]
        public async Task<List<ObjBusca<Int64>>> buscaQuemIndicou(String cpfcnpj, String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA");
                        sql.AppendLine("FROM TB_CLIENTE A");
                        sql.AppendLine("WHERE (UPPER(A.NOMEFANTASIA) LIKE @VALOR) AND (A.CPF <> @CPF)");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");
                        parametros.Add("@CPF", cpfcnpj);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
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



        [HttpGet()]
        [Route("api/Cliente_ProspectoApi/buscaAtedimento")]
        public async Task<List<ClienteListaBuscaAtendimento>> buscaAtedimento(String term)
        {
            List<ClienteListaBuscaAtendimento> retorno = new List<ClienteListaBuscaAtendimento>();

            using (ISession session = HibernateUtil.GetSession())
            using (var api = new PlanoProdutoApiController())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
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

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
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
                                    PlanosProdutos = api.getByCliente(Convert.ToInt64(reader["ID"])).Result,
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


        private void BindViewToObj(ISession session, ref Cliente_Anexo objBanco, ClienteAnexoModelView view)
        {
            using (var repository = new Conexao<Cliente>(session))
                objBanco.Cliente = repository.selectFirst(p => p.Id == view.Id_Cliente).Result;
            objBanco.Titulo = view.Titulo;
            objBanco.Nome_Arquivo = view.Nome_Arquivo;
            objBanco.Extensao_Arquivo = view.Extensao_Arquivo;
        }
        #endregion




        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, Cliente_ProspectoImportacao item, Cliente cliente_prospecto)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Pais> repoPais = new Conexao<Pais>(session))
            using (Conexao<Categoria_Cliente> repoCategoriaCliente = new Conexao<Categoria_Cliente>(session))
            using (Conexao<Meio_Captacao> repoMeioCaptacao = new Conexao<Meio_Captacao>(session))
            using (Conexao<Tipo_Imovel> repoTipoImovel = new Conexao<Tipo_Imovel>(session))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            using (ClienteApiController clienteApi = new ClienteApiController())
            {

                if ((await repoCliente.selectFirst(p => p.Numero_Contrato == item.Numero_Contrato)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "A Identificação Cliente pertence a um outro Cliente/Prospect criado. Prospect não foi importado."
                    });

                Pais pais = null;
                Meio_Captacao meio_Captacao = null;
                Categoria_Cliente categoria_Cliente = null;
                if (!String.IsNullOrEmpty(item.PAIS))
                {
                    pais = await repoPais.selectFirst(p => p.Nome == item.PAIS.ToUpperInvariant());
                    if (pais == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "País não encontrado no sistema" });
                }
                if (item.Meio_Captacao.HasValue)
                {
                    meio_Captacao = await repoMeioCaptacao.selectFirst(p => p.Id == Convert.ToInt64(item.Meio_Captacao));
                    if (meio_Captacao == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código do Meio de Captação não encontrado no sistema" });
                }
                if (item.Categoria_Cliente.HasValue)
                {
                    categoria_Cliente = await repoCategoriaCliente.selectFirst(p => p.Id == Convert.ToInt64(item.Categoria_Cliente));
                    if (categoria_Cliente == null)
                        retorno.Add(new ExcelServiceLogItem { Chave = item.Numero_Contrato, Erro = "Código da Categoria de Cliente não encontrado no sistema" });
                }

                if (retorno.Count() > 0)
                    return retorno;

                cliente_prospecto.Qual_Meio = meio_Captacao;
                cliente_prospecto.Categoria_Cliente = categoria_Cliente;
                cliente_prospecto.Numero_Contrato = item.Numero_Contrato;
                cliente_prospecto.Telefone_Fixo = item.Telefone_Fixo;
                cliente_prospecto.Telefone_Celular = item.Telefone_Celular;
                cliente_prospecto.Telefone_Recado = item.Telefone_Recado;
                cliente_prospecto.Pessoa_Tel_Recado = item.Pessoa_Recado;
                cliente_prospecto.Email_Principal = item.Email_Principal;
                cliente_prospecto.Tipo_Cliente = Tipo_Cliente.Prospecto;
                cliente_prospecto.Status_Prospecto = Status_Prospecto.Lead_Nao_Qualificado;
                cliente_prospecto.Status = Status_Cliente.Ativo;
                cliente_prospecto.Endereco_Principal = new Endereco_Cliente
                {
                    Bairro = item.Bairro,
                    Cep = item.Cep,
                    Cidade = item.Cidade,
                    Numero = item.Numero,
                    UF = item.UF,
                    Pais = pais,
                    Logradouro = item.Logradouro,
                    Complemento = item.Complemento,
                    Ponto_Referencia = item.Ponto_Referencia,
                    Tipo_Imovel = item.Tipo_Imovel.HasValue ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == item.Tipo_Imovel) : null
                };

                await clienteApi.GetEnderecoImportacao(cliente_prospecto, Tipo_EnderecoCliente.Principal);
            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloFisico(Cliente_ProspectoFisicaImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                Cliente cliente_prospecto = new Cliente();
                retorno = await ImportacaoModeloBase(session, item, cliente_prospecto);
                cliente_prospecto.CpfCnpj = item.Cpf.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                List<Cliente> clientes_cpf = (await repoCliente.select(p => p.CpfCnpj == cliente_prospecto.CpfCnpj)).ToList();
                if (clientes_cpf.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Cpf já utilizado por outro Cliente/Prospect"
                    });

                cliente_prospecto.NomeFantasia = item.Nome;
                cliente_prospecto.Filiacao_Mae = item.Mae;
                cliente_prospecto.Filiacao_Pai = item.Pai;
                cliente_prospecto.Rg = item.RG;
                cliente_prospecto.Data_Nascimento = item.Data_Nascimento;
                cliente_prospecto.Data_Emissao_Rg = item.Data_Emissao_RG;
                cliente_prospecto.Orgao_Emissao_Rg = item.Orgao_RG;
                cliente_prospecto.Nacionalidade = !String.IsNullOrEmpty(item.Nacionalidade) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Nacionalidade) : null;
                cliente_prospecto.Profissao = item.Profissao;
                cliente_prospecto.Tipo_Pessoa = Tipo_Pessoa.Fisica;

                if (item.Sexo == "M") cliente_prospecto.Sexo = Sexo.Masculino; else if (item.Sexo == "F") cliente_prospecto.Sexo = Sexo.Feminino;

                try
                {
                    cliente_prospecto.Estado_Civil = item.Estado_Civil == null ? (Estado_Civil?)null : ((Estado_Civil?)Enum.Parse(typeof(Estado_Civil), item.Estado_Civil));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Por favor corrija o campo Estado Civil"
                    });
                }

                try
                {
                    cliente_prospecto.Grau_Instrucao = item.Grau_Instrucao == null ? (Grau_Instrucao?)null : ((Grau_Instrucao?)Enum.Parse(typeof(Grau_Instrucao), item.Grau_Instrucao));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Por favor corrija o campo Grau de Instrução"
                    });
                }

                if (retorno.Count() > 0)
                    return retorno;


                await repoCliente.SaveOrUpdate(cliente_prospecto);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Numero_Contrato,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloJuridico(Cliente_ProspectoJuridicaImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
            {
                Cliente cliente_prospecto = new Cliente();

                retorno = await ImportacaoModeloBase(session, item, cliente_prospecto);
                cliente_prospecto.CpfCnpj = item.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                List<Cliente> clientes_cpf = (await repoCliente.select(p => p.CpfCnpj == cliente_prospecto.CpfCnpj)).ToList();
                if (clientes_cpf.Count > 0)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Cnpj já utilizado por outro Cliente/Prospect"
                    });

                cliente_prospecto.NomeFantasia = item.Nome;
                cliente_prospecto.Razao_Social = item.Razao_Social;
                cliente_prospecto.Numero_Contrato = item.Numero_Contrato;
                cliente_prospecto.Telefone_Fixo = item.Telefone_Fixo;
                cliente_prospecto.Telefone_Celular = item.Telefone_Celular;
                cliente_prospecto.Telefone_Recado = item.Telefone_Recado;
                cliente_prospecto.Pessoa_Tel_Recado = item.Pessoa_Recado;
                cliente_prospecto.Cargo_Responsavel = item.Cargo_Responsavel;
                cliente_prospecto.Cpf_Responsavel = String.IsNullOrEmpty(item.Cpf_Responsavel) ? null : item.Cpf_Responsavel.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                cliente_prospecto.Data_Abertura_Empresa = item.Data_Abertura;
                cliente_prospecto.Inscricao_Estadual = item.Inscricao_Estadual;
                cliente_prospecto.Inscricao_Municipal = item.Inscricao_Municipal;
                cliente_prospecto.Ramo_Atividade = item.Ramo_Atividade;
                cliente_prospecto.Nome_Resp_Financeiro = item.Responsavel;
                cliente_prospecto.Tipo_Pessoa = Tipo_Pessoa.Juridica;

                try
                {
                    cliente_prospecto.Porte = String.IsNullOrEmpty(item.Porte) ? (Porte?)null : ((Porte?)Enum.Parse(typeof(Porte), item.Porte));
                }
                catch
                {
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Numero_Contrato,
                        Erro = "Por favor corrija o campo Porte"
                    });
                }

                if (retorno.Count() > 0)
                    return retorno;

                await repoCliente.SaveOrUpdate(cliente_prospecto);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Numero_Contrato,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        #region Mobile
        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/GravarMobile")]
        public async Task<GetApiRetorno<Int64>> GravarMobile(ParamWebApi<Cliente_ProspectoMobile> value)
        {
            GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();
            Int32 count = existecpf(value.Objeto.CpfCnpj).Result;

            try
            {
                if (count > 0 && value.Objeto.Id == 0)
                {
                    throw new Exception(Resources.RsCadastros.CPFCNPJ_CADASTRADO);
                }

                ParamWebApi<Cliente_ProspectoDadosGeraisViewModel> prospecto = new ParamWebApi<Cliente_ProspectoDadosGeraisViewModel>
                {
                    Objeto = GetViewObjMobile(value.Objeto),
                    Usuario = value.Usuario,
                    NomeUsuario = value.NomeUsuario,
                    Token = value.Token
                };

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    retorno.Objeto = await GravarProspecto(session, sessionT5, prospecto);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;
        }

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/ConsultarProspectoMobile")]
        public async Task<GetApiRetorno<List<Cliente_ProspectoMobile>>> ConsultarProspectoMobile(ParamWebApi<ConsultarCliente_ProspectoMobileRequest> value)
        {
            GetApiRetorno<List<Cliente_ProspectoMobile>> retorno = new GetApiRetorno<List<Cliente_ProspectoMobile>>();

            var lista_clientes = new List<Cliente>();
            try
            {
                if (String.IsNullOrEmpty(value.Objeto.Busca))
                {
                    throw new Exception("É necessário informar CPF ou Nome para realizar a consulta.");
                }

                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOMEFANTASIA, A.CPF FROM TB_CLIENTE A ");
                        sql.AppendLine("WHERE A.STATUS = 1 ");

                        if (value.Objeto.IsCpfCnpj)
                        {
                            sql.AppendLine("AND A.CPF = @CPF_CNPJ ");
                            parametros.Add("@CPF_CNPJ", value.Objeto.Busca.Replace(".", String.Empty).Replace("-", String.Empty));
                        }
                        else
                        {
                            sql.AppendLine("AND UPPER(A.NOMEFANTASIA) LIKE @NOMEFANTASIA");
                            parametros.Add("@NOMEFANTASIA", String.Format("%{0}%", value.Objeto.Busca.ToUpperInvariant()));
                        }


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                lista_clientes.Add(new Cliente
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"])
                                });
                            }
                        }
                    }
                }

                var lista_retorno = new List<Cliente_ProspectoMobile>();

                foreach (var cliente in lista_clientes)
                {
                    lista_retorno.Add(BindViewToObjMobile(cliente));
                }

                retorno.Objeto = lista_retorno;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/FindByIdMobile")]
        public async Task<GetApiRetorno<Cliente_ProspectoMobile>> FindByIdMobile(ParamWebApi<long> value)
        {
            GetApiRetorno<Cliente_ProspectoMobile> retorno = new GetApiRetorno<Cliente_ProspectoMobile>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Cliente>(session))
            {
                try
                {
                    retorno.Objeto = BindViewToObjMobile(await repository.selectFirst(p => p.Id == value.Objeto));

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


        private Cliente_ProspectoMobile BindViewToObjMobile(Cliente model)
        {
            var cliente_prospectoMobile = new Cliente_ProspectoMobile();

            cliente_prospectoMobile.Id = model.Id;
            cliente_prospectoMobile.TipoCliente = model.Tipo_Cliente;
            cliente_prospectoMobile.TipoPessoa = model.Tipo_Pessoa;
            cliente_prospectoMobile.Status = model.Status == Status_Cliente.Ativo;
            cliente_prospectoMobile.Status_Prospecto = model.Status_Prospecto;
            cliente_prospectoMobile.CpfCnpj = model.CpfCnpj;

            cliente_prospectoMobile.NomeCompleto = model.NomeFantasia;
            cliente_prospectoMobile.RG = model.Rg;
            cliente_prospectoMobile.DataEmissao = Convert.ToString(model.Data_Emissao_Rg?.ToString("dd/MM/yyyy"));
            cliente_prospectoMobile.Orgao = model.Orgao_Emissao_Rg;
            cliente_prospectoMobile.DataNascimento = Convert.ToString(model.Data_Nascimento?.ToString("dd/MM/yyyy"));
            cliente_prospectoMobile.Nacionalidade = model.Nacionalidade;
            cliente_prospectoMobile.Sexo = model.Sexo;
            cliente_prospectoMobile.EstadoCivil = model.Estado_Civil;
            cliente_prospectoMobile.Instrucao = model.Grau_Instrucao;
            cliente_prospectoMobile.Id_Categoria_Cliente = model.Categoria_Cliente?.Id;
            cliente_prospectoMobile.Profissao = model.Profissao;
            cliente_prospectoMobile.Pai = model.Filiacao_Pai;
            cliente_prospectoMobile.Mae = model.Filiacao_Mae;

            cliente_prospectoMobile.NomeFantasia = model.NomeFantasia;
            cliente_prospectoMobile.Data_Abertura = model.Data_Abertura_Empresa;
            cliente_prospectoMobile.Razao_Social = model.Razao_Social;
            cliente_prospectoMobile.Inscricao_Estadual = model.Inscricao_Estadual;
            cliente_prospectoMobile.Inscricao_Municipal = model.Inscricao_Municipal;
            cliente_prospectoMobile.Porte = model.Porte;
            cliente_prospectoMobile.Representante = model.Nome_Resp_Financeiro;
            cliente_prospectoMobile.Cargo_Responsavel = model.Cargo_Responsavel;
            cliente_prospectoMobile.Ramo_Atividade = model.Ramo_Atividade;
            cliente_prospectoMobile.CPFResponsavel = model.Cpf_Responsavel;

            cliente_prospectoMobile.TelefoneFixo = model.Telefone_Fixo;
            cliente_prospectoMobile.Telefone_Celular = model.Telefone_Celular;
            cliente_prospectoMobile.TelefoneRecado = model.Telefone_Recado;
            cliente_prospectoMobile.NomeRecado = model.Pessoa_Tel_Recado;
            cliente_prospectoMobile.Email = model.Email_Principal ?? model.Email_Auxiliar;

            cliente_prospectoMobile.Id_Meio_Captacao = model.Qual_Meio?.Id;
            cliente_prospectoMobile.Id_Meio_Captacao = model.Qual_Meio?.Id;

            cliente_prospectoMobile.Endereco_Prospecto = new EnderecoClienteViewModel();
            cliente_prospectoMobile.Endereco_Prospecto.Id_Pais = model.Endereco_Prospecto?.Pais?.Id;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Tipo_Imovel = model.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Bairro = model.Endereco_Prospecto?.Id_Bairro;
            cliente_prospectoMobile.Endereco_Prospecto.Bairro = model.Endereco_Prospecto?.Bairro;
            cliente_prospectoMobile.Endereco_Prospecto.CEP = model.Endereco_Prospecto?.Cep;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Cidade = model.Endereco_Prospecto?.Id_Cidade;
            cliente_prospectoMobile.Endereco_Prospecto.Cidade = model.Endereco_Prospecto?.Cidade;
            cliente_prospectoMobile.Endereco_Prospecto.Complemento = model.Endereco_Prospecto?.Complemento;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Endereco = model.Endereco_Prospecto != null ? model.Endereco_Prospecto.Id : (Int64)0;
            cliente_prospectoMobile.Endereco_Prospecto.Latitude = Convert.ToString(model.Endereco_Prospecto?.Latitude);
            cliente_prospectoMobile.Endereco_Prospecto.Logradouro = model.Endereco_Prospecto?.Endereco_Logradouro?.Descricao ?? model.Endereco_Prospecto?.Logradouro;
            cliente_prospectoMobile.Endereco_Prospecto.Longitude = Convert.ToString(model.Endereco_Prospecto?.Longitude);
            cliente_prospectoMobile.Endereco_Prospecto.Numero = model.Endereco_Prospecto?.Numero;
            cliente_prospectoMobile.Endereco_Prospecto.Ponto_Referencia = model.Endereco_Prospecto?.Ponto_Referencia;
            cliente_prospectoMobile.Endereco_Prospecto.Tipo = Tipo_EnderecoCliente.Prospecto;
            cliente_prospectoMobile.Endereco_Prospecto.UF = model.Endereco_Prospecto?.UF;
            cliente_prospectoMobile.Endereco_Prospecto.Busca_Endereco = model.Endereco_Prospecto?.Busca_Endereco != null ? model.Endereco_Prospecto.Busca_Endereco : null;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Endereco_Logradouro = model.Endereco_Prospecto?.Endereco_Logradouro?.Id;
            cliente_prospectoMobile.Endereco_Prospecto.Id_Condominio_Predio = model.Endereco_Prospecto?.Condominio_Predio?.Id;
            cliente_prospectoMobile.Endereco_Prospecto.Descricao_Condominio = model.Endereco_Prospecto?.Condominio_Predio?.Descricao;
            cliente_prospectoMobile.Endereco_Prospecto.TipoLogradouro = model.Endereco_Prospecto?.Endereco_Logradouro?.Tipo ?? model.Endereco_Prospecto?.Condominio_Predio?.Endereco_Logradouro?.Tipo;

            return cliente_prospectoMobile;
        }

        private Cliente_ProspectoDadosGeraisViewModel GetViewObjMobile(Cliente_ProspectoMobile obj)
        {
            var objretorno = new Cliente_ProspectoDadosGeraisViewModel
            {
                Id = obj.Id,
                TipoCliente = obj.TipoCliente,
                TipoPessoa = obj.TipoPessoa,
                Status = obj.Status,
                Status_Prospecto = obj.Status_Prospecto,

                NomeCompleto = obj.NomeFantasia,
                RG = obj.RG,
                DataEmissao = obj.DataEmissao,
                Orgao = obj.Orgao,
                DataNascimento = obj.DataNascimento,
                Nacionalidade = obj.Nacionalidade,
                Sexo = obj.Sexo,
                EstadoCivil = obj.EstadoCivil,
                Instrucao = obj.Instrucao,
                Id_Categoria_Cliente = obj.Id_Categoria_Cliente,
                Profissao = obj.Profissao,
                Pai = obj.Pai,
                Mae = obj.Mae,

                NomeFantasia = obj.NomeFantasia,
                Data_Abertura = obj.Data_Abertura,
                Razao_Social = obj.Razao_Social,
                Inscricao_Estadual = obj.Inscricao_Estadual,
                Inscricao_Municipal = obj.Inscricao_Municipal,
                Porte = obj.Porte,
                Representante = obj.Representante,
                Cargo_Responsavel = obj.Cargo_Responsavel,
                Ramo_Atividade = obj.Ramo_Atividade,
                CPFResponsavel = obj.CPFResponsavel,

                TelefoneFixo = obj.TelefoneFixo,
                Telefone_Celular = obj.Telefone_Celular,
                TelefoneRecado = obj.TelefoneRecado,
                NomeRecado = obj.NomeRecado,
                Email = obj.Email,

                Id_Meio_Captacao = obj.Id_Meio_Captacao,

                Id_Departamento = obj.Id_Departamento,
            };

            if (obj.TipoPessoa.Value == Tipo_Pessoa.Fisica)
            {
                objretorno.Cpf = obj.CpfCnpj;

                objretorno.Id_Meio_Captacao = obj.Id_Meio_Captacao;
                objretorno.Id_Categoria_Cliente = obj.Id_Categoria_Cliente;
            }
            else
            {
                objretorno.Cnpj = obj.CpfCnpj;

                objretorno.Id_Meio_CaptacaoJuridico = obj.Id_Meio_Captacao;
                objretorno.Id_Categoria_ClienteJuridico = obj.Id_Categoria_Cliente;
            }

            objretorno.EnderecoProspecto = new EnderecoClienteViewModel();

            objretorno.EnderecoProspecto.Id_Pais = obj.Endereco_Prospecto?.Id_Pais;
            objretorno.EnderecoProspecto.Id_Tipo_Imovel = obj.Endereco_Prospecto?.Id_Tipo_Imovel;
            objretorno.EnderecoProspecto.Id_Bairro = obj.Endereco_Prospecto?.Id_Bairro;
            objretorno.EnderecoProspecto.Bairro = obj.Endereco_Prospecto?.Bairro;
            objretorno.EnderecoProspecto.CEP = obj.Endereco_Prospecto?.CEP;
            objretorno.EnderecoProspecto.Id_Cidade = obj.Endereco_Prospecto?.Id_Cidade;
            objretorno.EnderecoProspecto.Cidade = obj.Endereco_Prospecto?.Cidade;
            objretorno.EnderecoProspecto.Complemento = obj.Endereco_Prospecto?.Complemento;
            objretorno.EnderecoProspecto.Id_Endereco = obj.Endereco_Prospecto != null ? obj.Endereco_Prospecto.Id : (Int64)0;
            objretorno.EnderecoProspecto.Latitude = Convert.ToString(obj.Endereco_Prospecto?.Latitude);
            objretorno.EnderecoProspecto.Logradouro = obj.Endereco_Prospecto?.Logradouro;
            objretorno.EnderecoProspecto.Longitude = Convert.ToString(obj.Endereco_Prospecto?.Longitude);
            objretorno.EnderecoProspecto.Numero = obj.Endereco_Prospecto?.Numero;
            objretorno.EnderecoProspecto.Ponto_Referencia = obj.Endereco_Prospecto?.Ponto_Referencia;
            objretorno.EnderecoProspecto.Tipo = Tipo_EnderecoCliente.Prospecto;
            objretorno.EnderecoProspecto.UF = obj.Endereco_Prospecto?.UF;
            objretorno.EnderecoProspecto.Busca_Endereco = obj.Endereco_Prospecto?.Busca_Endereco;
            objretorno.EnderecoProspecto.Id_Condominio_Predio = obj.Endereco_Prospecto?.Id_Condominio_Predio;
            objretorno.EnderecoProspecto.Id_Endereco_Logradouro = obj.Endereco_Prospecto?.Id_Endereco_Logradouro;

            objretorno.EnderecoPrincipal.Id_Pais = obj.Endereco_Prospecto?.Id_Pais;
            objretorno.EnderecoPrincipal.Id_Tipo_Imovel = obj.Endereco_Prospecto?.Id_Tipo_Imovel;
            objretorno.EnderecoPrincipal.Id_Bairro = obj.Endereco_Prospecto?.Id_Bairro;
            objretorno.EnderecoPrincipal.Bairro = obj.Endereco_Prospecto?.Bairro;
            objretorno.EnderecoPrincipal.CEP = obj.Endereco_Prospecto?.CEP;
            objretorno.EnderecoPrincipal.Id_Cidade = obj.Endereco_Prospecto?.Id_Cidade;
            objretorno.EnderecoPrincipal.Cidade = obj.Endereco_Prospecto?.Cidade;
            objretorno.EnderecoPrincipal.Complemento = obj.Endereco_Prospecto?.Complemento;
            objretorno.EnderecoPrincipal.Id_Endereco = obj.Endereco_Prospecto != null ? obj.Endereco_Prospecto.Id : (Int64)0;
            objretorno.EnderecoPrincipal.Latitude = Convert.ToString(obj.Endereco_Prospecto?.Latitude);
            objretorno.EnderecoPrincipal.Logradouro = obj.Endereco_Prospecto?.Logradouro;
            objretorno.EnderecoPrincipal.Longitude = Convert.ToString(obj.Endereco_Prospecto?.Longitude);
            objretorno.EnderecoPrincipal.Numero = obj.Endereco_Prospecto?.Numero;
            objretorno.EnderecoPrincipal.Ponto_Referencia = obj.Endereco_Prospecto?.Ponto_Referencia;
            objretorno.EnderecoPrincipal.Tipo = Tipo_EnderecoCliente.Prospecto;
            objretorno.EnderecoPrincipal.UF = obj.Endereco_Prospecto?.UF;
            objretorno.EnderecoPrincipal.Busca_Endereco = obj.Endereco_Prospecto?.Busca_Endereco;
            objretorno.EnderecoPrincipal.Id_Condominio_Predio = obj.Endereco_Prospecto?.Id_Condominio_Predio;
            objretorno.EnderecoPrincipal.Id_Endereco_Logradouro = obj.Endereco_Prospecto?.Id_Endereco_Logradouro;

            return objretorno;
        }

        #endregion

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/ValidarCpfCnpjReceita")]
        public async Task<PostApiRetorno<Int64>> ValidarCpfCnpjReceita(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        var lista_temp = value.ids.ToList();
                        foreach (var id in value.ids)
                        {
                            if (await VerificarVendaProspecto(id))
                            {
                                lista_temp.Remove(id);
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Mensagem = "prospecto possui venda em andamento",
                                    Tipo = PostApiMensagemType.Erro,
                                });

                            }
                        }

                        value.ids = lista_temp;



                        if (value.ids.Count > 0)
                        {
                            String _ids = $"({String.Join(",", value.ids)})";

                            StringBuilder sql = new StringBuilder($"UPDATE TB_CLIENTE SET STATUS = {value.status} WHERE ID IN {_ids}");

                            await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                            retorno.Ok = true;
                        }

                        else
                        {
                            retorno.Ok = false;
                        }
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
        public List<(Filtro_PainelVendaEnum tipo_filtro, String valores)> GetModeloFiltros(Int64? Id_Modelo_PainelServico, Cliente_Prospecto_DataTableRequest request)
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
                    if (prop.Name == "Status")
                    {
                        var statusQuery = String.Join(",", Convert.ToString(prop.GetValue(request)).Split(',').Where(p => string.IsNullOrEmpty(p) == false).Select(p => $"'{p}'"));

                        if (string.IsNullOrEmpty(statusQuery) == false)
                        {
                            retorno.Add((attr.Recurso, $"(COLUNA IN ({statusQuery}))"));
                        }
                    }
                    else if (prop.Name == "EtapaVenda")
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

        [HttpPost()]
        [Route("api/Cliente_ProspectoApi/ConsultaRestricaoQualificativa")]
        public async Task<Response> ConsultaRestricaoQualificativa(ConsultaRestricaoQualificativaRequest request)
        {
            var retorno = new Response();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    IConfiguracoesGeraisRepository repoCfg = new ConfiguracoesGeraisRepository(sessionNh);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoCfg);

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                        StatusEtapaVenda? statusConsultaViabilidade = null;
                        StatusEtapaVenda? statusConsultaPendenciaFinanceira = null;
                        StatusEtapaVenda? statusConsultaPendenciaEndereco = null;
                        StatusEtapaVenda? statusConsultaCredito = null;

                        using (var _repoEtavaVenda = new Conexao<EtapaVenda>(sessionNh))
                        {
                            var listaEtapaVenda = await _repoEtavaVenda.select(x => x.Cliente.Id == request.Id_Cliente);

                            listaEtapaVenda.ToList().ForEach(x =>
                            {
                                if (x.Etapa == EnumEtapaVenda.ConsultaViabilidade)
                                {
                                    statusConsultaViabilidade = x.Status;
                                }
                                else if (x.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira)
                                {
                                    statusConsultaPendenciaFinanceira = x.Status;
                                }
                                else if (x.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco)
                                {
                                    statusConsultaPendenciaEndereco = x.Status;
                                    
                                }
                                else if (x.Etapa == EnumEtapaVenda.ConsultaCredito)
                                {
                                    statusConsultaCredito = x.Status;
                                }
                            });
                        }

                    var validacoes = new List<(bool Ativo, StatusEtapaVenda? Status)>
                    {
                        (config.ConsultaViabilidadeQualificaLead, statusConsultaViabilidade),
                        (config.ConsultaPendenciaEnderecoQualificaLead, statusConsultaPendenciaEndereco),
                        (config.ConsultaPendenciaFinanceiraQualificaLead, statusConsultaPendenciaFinanceira),
                        (config.ConsultaCreditoQualificaLead, statusConsultaCredito)
                    };

                    //Valor será mantido como true, a menos que
                    //a rotina caia em uma das condições abaixo
                    retorno.result = true;

                    //Verifica se a restrição qualificativa está ativa. Se sim, valida o status da consulta.
                    //Se não, apenas continua o foreach.
                    foreach (var validacao in validacoes)
                    {
                        if (!validacao.Ativo)
                            continue;

                        if (!validacao.Status.HasValue || validacao.Status == StatusEtapaVenda.Pendente)
                        {
                            retorno.result = false;
                            retorno.message = "A consulta de restrição qualificativa não foi realizada. Por favor, realize as consultas necessárias e tente novamente.";
                            return retorno;
                        }

                        if (validacao.Status == StatusEtapaVenda.Reprovado)
                        {
                            retorno.result = false;
                            retorno.message = "Este lead encontra-se com restrições ativas. Para continuar com o processo, é necessário verificar e resolver as pendências existentes.";
                            return retorno;
                        }

                        if (validacao.Status == StatusEtapaVenda.AguardandoAprovacaoGestor)
                        {
                            retorno.result = false;
                            retorno.message = "Este lead encontra-se com aprovação pendente do gestor. Para continuar com o processo, é necessário que a aprovação seja realizada.";
                            return retorno;
                        }
                    }
                                                             
                }
            }
            catch(Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.Message ?? "Um erro ocorreu durante o processo de consulta de restrição qualificativa";
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
           

           return retorno;
        }
    }
}

public class ConsultaRestricaoQualificativaRequest
{
    public long? Id_Cliente { get; set; }
}

public class ConsultarDocRequest
{
    public Tipo_Pessoa TipoPessoa { get; set; }
    public string CPF { get; set; }
    public string CNPJ { get; set; }
}

public class ConsultarDocResponse
{
    public string nome { get; set; }
    public string nomeFantasia { get; set; }
    public string razaoSocial { get; set; }
    public string nascimento { get; set; }
    public string dataAbertura { get; set; }
    public int tipoPessoa { get; set; }
}
