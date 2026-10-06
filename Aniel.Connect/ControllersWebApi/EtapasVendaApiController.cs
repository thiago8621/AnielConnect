using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.DAO.map;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebGrease.Css.Ast.Selectors;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EtapasVendaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/EtapasVendaApi/GetAll")]
        public async Task<DataTableResponse<EtapasVenda_ListModel>> GetAll(EtapasVenda_DataTableRequest request)
        {
            List<EtapasVenda_ListModel> retorno = new List<EtapasVenda_ListModel>();
            int recordsTotal = 0;

            String Status = $"({request.Status})";

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNH = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNH))
                using (var repoEtapa = new Conexao<EtapaVenda>(sessionNH))
                using (var repoPerfilVisVenda = new Conexao<PerfilVisualizacaoVenda>(sessionNH))
                using (var repoFuncionario = new Conexao<IdentityUser>(sessionNH))
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_VENDA A  ")
                        .AppendLine(" LEFT OUTER JOIN TB_CLIENTE B ")
                        .AppendLine(" ON(A.ID_CLIENTE = B.ID) ")

                        .AppendLine(" LEFT OUTER JOIN TB_FUNCIONARIO C ")
                        .AppendLine(" ON(A.ID_VENDEDOR = C.ID) ")

                        .AppendLine(" LEFT OUTER JOIN TB_ENDERECO_CLIENTE D ")
                        .AppendLine(" ON(B.ID_ENDERECO_PRINCIPAL = D.ID) ")

                         .AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR S")
                         .AppendLine("ON (S.ID_SUPERVISOR = A.ID_SUPERVISOR)")

                        .AppendLine(" LEFT JOIN TB_EQUIPE_VENDA V")
                        .AppendLine(" ON (V.ID = S.ID_EQUIPE_VENDA)")

                        .AppendLine(" LEFT JOIN TB_VENCIMENTO E ")
                        .AppendLine(" ON(B.ID_VENCIMENTO = E.ID) ")

                        .AppendLine(" LEFT JOIN TB_FUNCIONARIO I ")
                        .AppendLine(" ON(I.ID = A.ID_SUPERVISOR) ")

                        .AppendLine(" LEFT JOIN TB_MEIO_CAPTACAO J ")
                        .AppendLine(" ON(B.ID_MEIO_CAPTACAO = J.ID) ")

                        .AppendLine(" LEFT JOIN TB_CLIENTE K ")
                        .AppendLine(" ON(K.ID = B.ID_QUEM_INDICOU) ")

                        .AppendLine(" LEFT JOIN TB_VENDA_HISTORICO_CONTATO L")
                        .AppendLine(" ON(A.ID = L.ID_VENDA)")

                        .AppendLine(" LEFT JOIN TB_TIPO_IMOVEL M ")
                        .AppendLine(" ON(M.ID = D.ID_TIPO_IMOVEL) ")

                        .AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE N ")
                        .AppendLine(" ON(N.ID = B.ID_CATEGORIA) ")

                        .AppendLine(" LEFT JOIN TB_DEPARTAMENTO O ")
                        .AppendLine(" ON(O.ID = A.ID_DEPARTAMENTO) ")

                         .AppendLine(" LEFT JOIN TB_MOTIVO_DESISTENCIA_VENDA P ")
                        .AppendLine(" ON(P.ID = A.ID_MOTIVO_DESISTENCIA_VENDA) ")

                        .AppendLine(" LEFT JOIN TB_ETAPA_VENDA Q ")
                        .AppendLine(" ON(Q.ID = A.ID_ETAPA_VENDA) ")

                        .AppendLine(" LEFT JOIN TB_WORKFLOW_ATIVIDADE R ")
                        .AppendLine(" ON(R.ID = A.ID_WORKFLOW_VENDAS_ATIVIDADE) ")

                        .AppendLine(" LEFT JOIN TB_ATIVSTATUS_VENDA T ")
                        .AppendLine(" ON(T.ID = R.ATIV_STATUSVENDA) ")
                        
                        .AppendLine(" LEFT JOIN TB_DEPARTAMENTO U  ")
                        .AppendLine(" ON U.ID = A.ID_SETOR_DEPARTAMENTO ")
                        
                        .AppendLine(" LEFT JOIN ASPNETUSERS X ")
                        .AppendLine(" ON X.ID = A.ID_RESPONSAVEL ");
                        


                    StringBuilder sqlWhere = new StringBuilder();
                    StringBuilder sqlStatus = new StringBuilder();

                    List<String> condicoes = new List<String>();



                    #region Filtro de Status
                    if (request.StatusDadosGerais.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 0 AND ETA.STATUS IN ({String.Join(", ", request.StatusDadosGerais)}))");
                    }

                    if (request.StatusConsultaViabilidade.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 1 AND ETA.STATUS IN ({String.Join(", ", request.StatusConsultaViabilidade)}))");
                    }

                    if (request.StatusPendenciaFinanceira.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 2 AND ETA.STATUS IN ({String.Join(", ", request.StatusPendenciaFinanceira)}))");
                    }

                    if (request.StatusPendenciaEndereco.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 3 AND ETA.STATUS IN ({String.Join(", ", request.StatusPendenciaEndereco)}))");
                    }

                    if (request.StatusConsultaCredito.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 4 AND ETA.STATUS IN ({String.Join(", ", request.StatusConsultaCredito)}))");
                    }

                    if (request.StatusPlanoProduto.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 5 AND ETA.STATUS IN ({String.Join(", ", request.StatusPlanoProduto)}))");
                    }
                    
                    if (request.StatusFinanceiro.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA =  6 AND ETA.STATUS IN ({String.Join(", ", request.StatusFinanceiro)}))");
                    }

                    if (request.StatusChecklist.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 7 AND ETA.STATUS IN ({String.Join(", ", request.StatusChecklist)}))");
                    }

                    if (request.StatusResumo.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 8 AND ETA.STATUS IN ({String.Join(", ", request.StatusResumo)}))");
                    }

                    if (request.StatusTermoAdesao.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 9 AND ETA.STATUS IN ({String.Join(", ", request.StatusTermoAdesao)}))");
                    }

                    if (request.StatusOrdemServico.Count != 0)
                    {
                        condicoes.Add($" (ETA.ETAPA = 11 AND ETA.STATUS IN ({String.Join(", ", request.StatusOrdemServico)}))");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlStatus.AppendLine($" AND {condicoes.Count} = (SELECT COUNT(VEN.ID) FROM TB_VENDA VEN INNER JOIN TB_ETAPA_VENDA ETA ON ETA.ID_VENDA = VEN.ID");
                        sqlStatus.AppendLine($"WHERE VEN.ID = A.ID AND ( {String.Join(" OR ", condicoes)} )) ");

                        sqlWhere.Append(sqlStatus);
                    }

                    #endregion


                    #region Perfil de usuário
                    //1) Buscar informações do perfil de usuário
                    var usuario = await repoFuncionario.selectFirst(p => p.Id == User.Identity.GetUserId());

                    if (usuario.PerfilVisualizacaoVenda != null && usuario.Funcionario?.Id != null)
                    {
                        //2) Buscar as informações dos parametros de visualização
                        var paramVisuVenda = await repoPerfilVisVenda.selectFirst(p => p.Id == usuario.PerfilVisualizacaoVenda.Id);

                        if (paramVisuVenda.PadraoVisualizaVenda == true && usuario.Funcionario?.Id != null)
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
                            parametros.Add("@SEARCHNOME", String.Format("%{0}%", request.search.value.ToUpper()));
                        }
                    }                   

                    if (!String.IsNullOrEmpty(request.Estado))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_UF) = @ESTADO) ");
                        parametros.Add("@ESTADO", request.Estado);
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_CIDADE) = @CIDADE) ");
                        parametros.Add("@CIDADE", request.Cidade);
                    }

                    if (!String.IsNullOrEmpty(request.Departamento))
                    {
                        sqlWhere.AppendLine(" AND (A.ID_DEPARTAMENTO = @DEPARTAMENTO) ");
                        parametros.Add("@DEPARTAMENTO", request.Departamento);
                    }
                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_BAIRRO) = @BAIRRO) ");
                        parametros.Add("@BAIRRO", request.Bairro);
                    }


                    if (!String.IsNullOrEmpty(request.Vendedor))
                    {
                        sqlWhere.AppendLine(" AND (A.ID_VENDEDOR = @ID_VENDEDOR) ");
                        parametros.Add("@ID_VENDEDOR", request.Vendedor);
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sqlWhere.AppendLine(" AND (A.ID_SUPERVISOR = @ID_SUPERVISOR) ");
                        parametros.Add("@ID_SUPERVISOR", request.Supervisor);
                    }

                    if (!String.IsNullOrEmpty(request.StatusVenda))
                    {
                        sqlWhere.AppendLine(" AND (T.DESCRICAO = @STATUSVENDA) ");
                        parametros.Add("@STATUSVENDA", request.StatusVenda);
                    }

                    if (!String.IsNullOrEmpty(request.Data_Inicial))
                    {
                        request.Data_Inicial = request.Data_Inicial + " 00:00";
                        request.Data_Final = request.Data_Final + " 23:59";
                        sqlWhere.AppendLine("AND A.DATAHORACRIACAO BETWEEN @DATAINICIAL_ABERTURA_VENDA AND @DATAFINAL_ABERTURA_VENDA");
                        parametros.Add("@DATAINICIAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Inicial));
                        parametros.Add("@DATAFINAL_ABERTURA_VENDA", String.Format("{0}", request.Data_Final));
                    }

                    if (!String.IsNullOrEmpty(request.EquipeVenda))
                    {
                        sqlWhere.AppendLine(" AND V.ID = @EQUIPEVENDA ");
                        parametros.Add("@EQUIPEVENDA", request.EquipeVenda);
                    }
                    if (!String.IsNullOrEmpty(request.MotivoAprovacao))
                    {
                        sqlWhere.AppendLine(" AND P.ID = @MOTIVO_DESISTENCIA_VENDA ");
                        parametros.Add("@MOTIVO_DESISTENCIA_VENDA", request.MotivoAprovacao);
                    }


                    if (!String.IsNullOrEmpty(request.Etapa_Atual))
                    {
                        sqlWhere.AppendLine(" AND (Q.ETAPA = @ETAPA)");
                        parametros.Add("@ETAPA", request.Etapa_Atual);
                    }

                    if (!String.IsNullOrEmpty(request.Setor_Responsavel))
                    {
                        sqlWhere.AppendLine(" AND (U.ID = @SETOR_RESPONSAVEL)");
                        parametros.Add("@SETOR_RESPONSAVEL", request.Setor_Responsavel);
                    }

                    if (!String.IsNullOrEmpty(request.Responsavel))
                    {
                        sqlWhere.AppendLine(" AND (X.ID = @RESPONSAVEL)");
                        parametros.Add("@RESPONSAVEL", request.Responsavel);
                    }

                    //Count
                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID)  CO ");
                        sql.AppendLine(sqlFrom.ToString())
                           .AppendLine("WHERE A.ID > 0 " + sqlWhere.ToString());

                        sql.AppendLine("GROUP BY A.ID, M.ID , M.DESCRICAO, B.NOMEFANTASIA, B.NUMERO_CONTRATO , B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, D.ENDERECO_BAIRRO, N.DESCRICAO,N.ID,A.ID_DEPARTAMENTO,")
                           .AppendLine("D.ID_CIDADE, D.ENDERECO_CIDADE, D.ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME,")
                           .AppendLine("I.NOME, A.ID_CAMPANHA, J.DESCRICAO, B.ID_MEIO_CAPTACAO,A.STATUS_AUDITORIA, ")
                           .AppendLine("A.STATUS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO,")
                           .AppendLine("D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA, A.DATAHORACRIACAO, A.DATA_VENDA,")
                           .AppendLine("B.NUMERO_CONTRATO, A.ID_CLIENTE, A.ID_VENDEDOR, A.ID_SUPERVISOR, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, A.USUARIOCRIACAO, P.DESCRICAO, Q.DESCRICAO,U.DESCRICAO ,X.NOME ");


                        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                recordsTotal++;
                            }
                        }
                    }

                    //Select
                    using (DbCommand com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, D.ENDERECO_BAIRRO, N.DESCRICAO AS CATEGORIA_CLIENTE,"); // A.DATA_ENCERRAMENTO, data comentada por nao ser mais usada
                        sql.AppendLine(" D.ID_CIDADE, M.DESCRICAO AS DESC_TIPO_IMOVEL, D.ENDERECO_CIDADE, D.ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME AS VENDEDOR, ");
                        sql.AppendLine(" I.NOME AS SUPERVISOR, A.ID_CAMPANHA, J.DESCRICAO AS MEIO_CAPTACAO,  "); //H.TITULO AS CAMPANHA,
                        sql.AppendLine(" A.STATUSVENDA AS STATUS_VENDA,A.STATUS,A.ID_WORKFLOW_VENDAS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO, ");
                        sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA AS QUEM_INDICOU, A.DATAHORACRIACAO, A.DATA_VENDA, B.NUMERO_CONTRATO AS NUMERO_CONTRATO, ");
                        sql.AppendLine(" A.ID_CLIENTE,MAX(L.DATA_ULTIMO_CONTATO) AS DATA_ULTIMO_CONTATO, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, O.DESCRICAO AS DEPARTAMENTO,A.STATUS_LIBERACAO,V.DESCRICAO  AS EQUIPEVENDA, P.DESCRICAO AS MOTIVO_DESISTENCIA_VENDA, Q.DESCRICAO AS ETAPA_ATUAL, ");
                        sql.AppendLine(" T.DESCRICAO AS DESCRICAO_ATIVIDADE,U.DESCRICAO AS SETOR_RESPONSAVEL,X.NOME AS RESPONSAVEL ");


                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine(" WHERE A.ID > 0 " + sqlWhere.ToString());

                        sql.AppendLine(" GROUP BY A.ID, B.NUMERO_CONTRATO , B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, D.ENDERECO_BAIRRO, N.DESCRICAO, N.ID,A.ID_DEPARTAMENTO, ");
                        sql.AppendLine(" D.ID_CIDADE, M.ID , DESC_TIPO_IMOVEL, D.ENDERECO_CIDADE, D.ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME, ");
                        sql.AppendLine(" I.NOME, A.ID_CAMPANHA, J.DESCRICAO, B.ID_MEIO_CAPTACAO,  CATEGORIA_CLIENTE ,A.STATUS_AUDITORIA,");
                        sql.AppendLine(" A.STATUS, A.STATUSVENDA, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO, ");
                        sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA, A.DATAHORACRIACAO, A.DATA_VENDA,");
                        sql.AppendLine(" B.NUMERO_CONTRATO, A.ID_CLIENTE, A.ID_VENDEDOR, A.ID_SUPERVISOR, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, A.USUARIOCRIACAO,O.DESCRICAO,A.STATUS_LIBERACAO,V.DESCRICAO, A.ID_WORKFLOW_VENDAS, P.DESCRICAO, Q.DESCRICAO, ");
                        sql.AppendLine(" T.DESCRICAO,U.DESCRICAO ,X.NOME ");


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
                                            sql.AppendLine(" ORDER BY DATA_ULTIMO_CONTATO");
                                        else
                                            sql.AppendLine(" ORDER BY DATA_ULTIMO_CONTATO DESC");
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
                                case "equipeVenda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY V.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY V.DESCRICAO DESC");
                                        break;
                                    }
                                case "status":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY T.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY T.DESCRICAO DESC");
                                        break;
                                    }
                                case "etapa_Atual":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY Q.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY Q.DESCRICAO DESC");
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
                                case "motivo_Desistencia_Venda":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY P.DESCRICAO");
                                        else
                                            sql.AppendLine(" ORDER BY P.DESCRICAO DESC");
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
                                case "setor_Responsavel":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY U.DESCRICAO ");
                                        else
                                            sql.AppendLine(" ORDER BY U.DESCRICAO  DESC");
                                        break;
                                    }
                                case "responsavel":
                                    {
                                        if (sortDirection == "asc")
                                            sql.AppendLine(" ORDER BY X.NOME");
                                        else
                                            sql.AppendLine(" ORDER BY X.NOME DESC");
                                        break;
                                    }


                            }
                        }

                        //Busca com paginacao
                        using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))

                        {
                            while (await reader.ReadAsync())
                            {
                                string statusV = Convert.ToString(reader["DESCRICAO_ATIVIDADE"]);
                                int statusVenda = reader["STATUS"] != DBNull.Value ? Convert.ToInt32(reader["STATUS"]) : 0;
                                string idVenda = reader["ID"] != DBNull.Value ? Convert.ToString(reader["ID"]) : "0";

                                var etapas = await repoEtapa.select(p => p.Venda.Id == Convert.ToInt64(idVenda));

                                retorno.Add(new EtapasVenda_ListModel
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
                                    DataHoraCancelamento = reader["DATA_VENDA"] != DBNull.Value && (statusVenda == 3 || statusVenda == 4) ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    DataUltimoContato = reader["DATA_ULTIMO_CONTATO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ULTIMO_CONTATO"]).ToString("dd/MM/yyyy HH:mm") : "",
                                    Meio_Captacao = Convert.ToString(reader["MEIO_CAPTACAO"]),
                                    Quem_Indicou = Convert.ToString(reader["QUEM_INDICOU"]),
                                    Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                    EquipeVenda = Convert.ToString(reader["EQUIPEVENDA"]),
                                    Setor_Responsavel = Convert.ToString(reader["SETOR_RESPONSAVEL"]),
                                    Responsavel = Convert.ToString(reader["RESPONSAVEL"]),

                                    Status = !String.IsNullOrEmpty(statusV) ? statusV : "",// HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS"])),
                                    Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : 0,
                                    Termo_Adesao_Assinado = HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.Padrao>(Convert.ToString(reader["TERMO_ADESAO_ASSINADO"] != DBNull.Value ? reader["TERMO_ADESAO_ASSINADO"] : 0)),
                                    Data_Termo_Assinado = reader["DATA_TERMO_ASSINADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_TERMO_ASSINADO"]).ToString("dd/MM/yyyy hh:mm") : "",
                                    Etapa_Atual = reader["ETAPA_ATUAL"] != DBNull.Value ? Convert.ToString(reader["ETAPA_ATUAL"]) : "",
                                    Motivo_Desistencia_Venda = reader["MOTIVO_DESISTENCIA_VENDA"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_DESISTENCIA_VENDA"]) : "",
                                    DadosGerais = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.DadosGerais)),
                                    ConsultaViabilidade = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.ConsultaViabilidade)),
                                    PendenciaEndereco = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco)),
                                    PendenciaFinanceira = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira)),
                                    ConsultaCredito = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.ConsultaCredito)),
                                    Financeiro = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.Financeiro)),
                                    PlanoProduto = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.PlanoProduto)),
                                    Checklist = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.Checklist)),
                                    TermoAdesao = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.TermoAdesao)),
                                    OrdemServico = GetValueOrDefault(etapas.FirstOrDefault(p => p.Etapa == EnumEtapaVenda.OrdemServico)),
                                });
                            }
                        }
                    }
                }

                //Page e retornando DataTable
                return new DataTableResponse<EtapasVenda_ListModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = retorno,
                    error = ""
                };

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        string GetValueOrDefault(EtapaVenda etapa) =>
    etapa == null ? "N/A" : HtmlEnumExtensions.GetEnumDescription<StatusEtapaVenda>(Convert.ToString(etapa.Status));



        [HttpPost()]
        [Route("api/EtapasVendaApi/GetComboStatus")]
        public async Task<ObjCombo<Int64>> GetComboStatus(RequestObjComboPai<Int64?, Int64?> request)
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

                    foreach (var status in Enum.GetValues(typeof(StatusEtapaVenda)))
                    {
                        var obj = new ObjComboResult<Int64>
                        {
                            Id = Convert.ToInt64(status),
                            Text = HtmlEnumExtensions.GetEnumDescription<StatusEtapaVenda>(Convert.ToString(status))
                        };
                        retorno.results.Add(obj);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }
    }
}