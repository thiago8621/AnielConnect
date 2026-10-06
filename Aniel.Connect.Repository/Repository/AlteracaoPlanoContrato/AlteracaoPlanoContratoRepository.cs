using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Repository.Model;
using MongoDB.Driver;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.AlteracaoPlanoContrato
{
    public class AlteracaoPlanoContratoRepository: IAlteracaoPlanoContratoRepository
    {
        private ISession _session;                      
        private ISession _sessionT5;                      
        private ISession _sessionLog;                      
        public AlteracaoPlanoContratoRepository(ISession session, ISession sessionT5, ISession sessionLog) 
        {
            _session = session;            
            _sessionT5 = sessionT5;
            _sessionLog = sessionLog;
        }
        public async Task<int> Count(Dictionary<string, object> parametros, List<string> condicoes) 
        {
            #region [Count]
            var recordsFiltered = 0;
            using (var comm = this._session.Connection.CreateCommand())
            {
                var sqlCount = new StringBuilder();
                var sqlFrom = new StringBuilder();
                var sqlWhere = new StringBuilder();                
                var sqlGroupBy = new StringBuilder();
                sqlCount.AppendLine(@" SELECT COUNT(A.ID) CONT ");

                //--CASE WHEN(SELECT FROM tb_cliente_PLANO_PRODUTO CLIP WHERE CLIP.ID_VENDA = ) THEN 'UPGRADE'ELSE 'DOWNGRADE' END
                sqlFrom.AppendLine(" FROM TB_VENDA A  ")
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
                       .AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR S")
                       .AppendLine("ON (S.ID_SUPERVISOR = A.ID)")
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
                       .AppendLine(" LEFT JOIN TB_BAIRRO U ON U.ID =EL.ID_BAIRRO  ");

                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine($" WHERE A.ID > 0 AND {String.Join("AND", condicoes)} ");
                }

                sqlGroupBy.AppendLine("GROUP BY A.ID, M.ID , M.DESCRICAO, B.NOMEFANTASIA, B.NUMERO_CONTRATO , B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, CASE WHEN EL.DESCRICAO IS NOT NULL THEN U.DESCRICAO ELSE D.ENDERECO_BAIRRO END , N.DESCRICAO,N.ID,A.ID_DEPARTAMENTO,")
                          .AppendLine("CASE WHEN EL.DESCRICAO IS NOT NULL THEN T.DESCRICAO ELSE D.ENDERECO_CIDADE END , CASE WHEN EL.DESCRICAO IS NOT NULL THEN R.SIGLA  ELSE D.ENDERECO_UF END ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME,")
                          .AppendLine("I.NOME, A.ID_CAMPANHA, J.DESCRICAO, B.ID_MEIO_CAPTACAO,A.STATUS_AUDITORIA, V.DESCRICAO, ")
                          .AppendLine("A.STATUS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO,")
                          .AppendLine("D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA, A.DATAHORACRIACAO, A.DATA_VENDA, A.USUARIOALTERACAO,")
                          .AppendLine("G.ID_CLIENTE_CONTRATO, A.ID_CLIENTE, A.ID_VENDEDOR, A.ID_SUPERVISOR, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, A.USUARIOCRIACAO, ET.DESCRICAO, W.ATIV_STATUSVENDA ");

                sqlCount.AppendLine(sqlFrom.ToString())
                        .AppendLine(sqlWhere.ToString())
                        .AppendLine(sqlGroupBy.ToString());                        
                using (var reader = await CommandHelper.ExecuteReader(comm, sqlCount, parametros, this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        recordsFiltered++;
                    }
                }
            }
            return recordsFiltered;
            #endregion
        }
        public async Task<IEnumerable<AlteracaoPlanoContratoListModel>> GetAll(Dictionary<string, object> parametros, List<string> condicoes, Dictionary<string, string> ordenacao, int first, int skip) 
        {
            #region [GetAll]
            var records = new List<AlteracaoPlanoContratoListModel>();
            using (var comm = _session.Connection.CreateCommand()) 
            {
                var sql = new StringBuilder();
                var sqlFrom = new StringBuilder();
                var sqlWhere = new StringBuilder();
                var sqlOrder = new StringBuilder();
                var sqlGroupBy = new StringBuilder();

                /*CASE WHEN(SELECT FROM tb_cliente_PLANO_PRODUTO CLIP WHERE CLIP.ID_VENDA = ) THEN 'UPGRADE'ELSE 'DOWNGRADE' END*/


                sql.AppendLine(" SELECT A.ID, B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, CASE WHEN EL.DESCRICAO IS NOT NULL THEN U.DESCRICAO ELSE D.ENDERECO_BAIRRO END AS ENDERECO_BAIRRO , N.DESCRICAO AS CATEGORIA_CLIENTE,"); // A.DATA_ENCERRAMENTO, data comentada por nao ser mais usada
                sql.AppendLine(" D.ID_CIDADE, M.DESCRICAO AS DESC_TIPO_IMOVEL, CASE WHEN EL.DESCRICAO IS NOT NULL THEN T.DESCRICAO ELSE D.ENDERECO_CIDADE END AS ENDERECO_CIDADE , CASE WHEN EL.DESCRICAO IS NOT NULL THEN R.SIGLA ELSE D.ENDERECO_UF END AS ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME AS VENDEDOR, ");
                sql.AppendLine(" I.NOME AS SUPERVISOR, A.ID_CAMPANHA, J.DESCRICAO AS MEIO_CAPTACAO, "); //H.TITULO AS CAMPANHA,
                sql.AppendLine(" A.STATUSVENDA AS STATUS_VENDA,A.STATUS,A.ID_WORKFLOW_VENDAS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO, ");
                sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA AS QUEM_INDICOU, A.DATAHORACRIACAO, A.DATAHORAALTERACAO, A.DATA_VENDA, A.USUARIOALTERACAO, G.ID_CLIENTE_CONTRATO AS ID_CONTRATO, ");
                sql.AppendLine(" A.ID_CLIENTE,MAX(L.DATA_ULTIMO_CONTATO) AS DATA_ULTIMO_CONTATO, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, O.DESCRICAO AS DEPARTAMENTO, A.STATUS_LIBERACAO, ");
                sql.AppendLine(" V.DESCRICAO  AS EQUIPEVENDA, P.DESCRICAO AS MOTIVO_DESISTENCIA_VENDA, ET.DESCRICAO AS ETAPA_ATUAL, W.ATIV_STATUSVENDA, A.TIPO_ALTERACAO ");

                sqlFrom.AppendLine(" FROM TB_VENDA A  ")
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

                        .AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR S")
                        .AppendLine("ON (S.ID_SUPERVISOR = A.ID)")

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

                       .AppendLine(" LEFT JOIN TB_WORKFLOW_ATIVIDADE W ")
                       .AppendLine(" ON(W.ID_WORKFLOW_VENDAS = A.ID_WORKFLOW_VENDAS AND W.ID_STATUS_PADRAO = A.Status) ")

                       .AppendLine(" LEFT JOIN TB_ATIVSTATUS_VENDA Q ")
                       .AppendLine(" ON(Q.ID = W.ATIV_STATUSVENDA AND Q.STATUS_PADRAO = A.STATUS) ")

                       .AppendLine(" LEFT JOIN TB_ETAPA_VENDA ET ")
                       .AppendLine(" ON(ET.ID = A.ID_ETAPA_VENDA) ")

                       .AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO EL  ")
                       .AppendLine(" ON EL.ID  = D.ID_ENDERECO_LOGRADOURO ")

                       .AppendLine(" LEFT JOIN TB_UF R ON R.ID = EL.ID_UF  ")

                       .AppendLine(" LEFT JOIN TB_CIDADE T ON T.ID = EL.ID_CIDADE  ")

                       .AppendLine(" LEFT JOIN TB_BAIRRO U ON U.ID = EL.ID_BAIRRO  ");

                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine($" WHERE A.ID > 0 AND {String.Join("AND", condicoes)} ");
                }

                sqlGroupBy.AppendLine(" GROUP BY A.ID, B.NUMERO_CONTRATO , B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.PROFISSAO, CASE WHEN EL.DESCRICAO IS NOT NULL THEN U.DESCRICAO ELSE D.ENDERECO_BAIRRO END, N.DESCRICAO, N.ID,A.ID_DEPARTAMENTO, ")
                          .AppendLine(" D.ID_CIDADE, M.ID , DESC_TIPO_IMOVEL, CASE WHEN EL.DESCRICAO IS NOT NULL THEN T.DESCRICAO ELSE D.ENDERECO_CIDADE END, CASE WHEN EL.DESCRICAO IS NOT NULL THEN R.SIGLA ELSE D.ENDERECO_UF END ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME, ")
                          .AppendLine(" I.NOME, A.ID_CAMPANHA, J.DESCRICAO, B.ID_MEIO_CAPTACAO,  CATEGORIA_CLIENTE ,A.STATUS_AUDITORIA, ")
                          .AppendLine(" A.STATUS, A.STATUSVENDA, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO,")
                          .AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA, A.DATAHORACRIACAO,A.DATAHORAALTERACAO, A.DATA_VENDA, A.USUARIOALTERACAO, ")
                          .AppendLine(" G.ID_CLIENTE_CONTRATO, A.ID_CLIENTE, A.ID_VENDEDOR, A.ID_SUPERVISOR, A.TERMO_ADESAO_ASSINADO, A.DATA_TERMO_ASSINADO, A.USUARIOCRIACAO,O.DESCRICAO,A.STATUS_LIBERACAO,V.DESCRICAO, ")
                          .AppendLine("A.ID_WORKFLOW_VENDAS, P.DESCRICAO, ET.DESCRICAO, W.ATIV_STATUSVENDA, A.TIPO_ALTERACAO ");

                foreach (KeyValuePair<string,string> ordem in ordenacao) 
                {
                    sqlOrder.AppendLine($" ORDER BY {ordem.Key}");
                    if (!String.IsNullOrEmpty(ordem.Value) & ordem.Value != "asc") 
                    {
                        sqlOrder.Append(" desc");
                    }
                }

                sql.AppendLine(sqlFrom.ToString())
                   .AppendLine(sqlWhere.ToString())
                   .AppendLine(sqlGroupBy.ToString())
                   .AppendLine(sqlOrder.ToString());

                using (var reader = await CommandHelper.ExecuteReaderFirst(comm, sql, parametros, this._session.Connection, null, first, skip))
                {
                    while (await reader.ReadAsync())
                    {
                        int Status = Convert.ToInt32(reader["STATUS"]);
                        DateTime? DataHoraAprovacao = reader["DATA_VENDA"] != DBNull.Value &&
                            (Status_Venda)Enum.Parse(typeof(Status_Venda), Convert.ToString(Status)) == Status_Venda.Venda_Aprovada ? 
                            Convert.ToDateTime(reader["DATA_VENDA"]) : (DateTime?)null;
                        DateTime? DataHoraCancelamento = reader["DATA_VENDA"] != DBNull.Value && 
                            (Status_Venda)Enum.Parse(typeof(Status_Venda), Convert.ToString(Status)) == Status_Venda.Venda_Cancelada ? 
                            Convert.ToDateTime(reader["DATA_VENDA"]) : (DateTime?)null;
                        DateTime? DataHoraCriacao = Convert.ToDateTime(reader["DATAHORACRIACAO"]);
                        DateTime? DataAssinatura = reader["DATA_TERMO_ASSINADO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_TERMO_ASSINADO"]) : (DateTime?)null;
                        var alteracao = new AlteracaoPlanoContratoListModel();

                        //Int64
                        alteracao.Id = Convert.ToInt64(reader["ID"]);                            
                        alteracao.NumeroContrato = reader["ID_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONTRATO"]) : (long?)null;
                        //String
                        alteracao.CategoriaCliente = reader["CATEGORIA_CLIENTE"] != DBNull.Value? Convert.ToString(reader["CATEGORIA_CLIENTE"]): "-";
                        alteracao.Supervisor = reader["SUPERVISOR"] != DBNull.Value? Convert.ToString(reader["SUPERVISOR"]): "-";
                        alteracao.Departamento = reader["DEPARTAMENTO"] != DBNull.Value? Convert.ToString(reader["DEPARTAMENTO"]): "-";
                        alteracao.Vendedor = reader["VENDEDOR"] != DBNull.Value? Convert.ToString(reader["VENDEDOR"]): "-";
                        alteracao.NomeFantasia = reader["NOMEFANTASIA"] != DBNull.Value? Convert.ToString(reader["NOMEFANTASIA"]): "-";
                        alteracao.Documento = reader["CPF"] != DBNull.Value? Convert.ToString(reader["CPF"]): "-";
                        alteracao.TipoAlteracao = reader["TIPO_ALTERACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoAlteracaoPlanoContratoEnum>(Convert.ToString(reader["TIPO_ALTERACAO"])) : "-";
                        //Boolean
                        alteracao.TermoAdesaoAssinado = Convert.ToBoolean(reader["TERMO_ADESAO_ASSINADO"]) == true ? RsBase.SIM : RsBase.NAO;
                        //DateTime
                        alteracao.DataHoraAprovacao = DataHoraAprovacao != null ? DataHoraAprovacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
                        alteracao.DataHoraCancelamento = DataHoraCancelamento != null ? DataHoraCancelamento.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
                        alteracao.DataHoraCriacao = DataHoraCriacao != null ? DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
                        alteracao.DataAssinatura = DataAssinatura != null ? DataAssinatura.Value.ToString("dd/MM/yyyy") : "-";
                        alteracao.Status = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS"]));
                        
                        records.Add(alteracao);                        
                    }
                }
            }        
            return records;
            #endregion
        }
        public async Task<long> Save(Connect.Model.nhibernate.AlteracaoPlanoContrato obj) 
        {
            #region [Save]
            using (var comm = this._session.Connection.CreateCommand()) 
            {
                var parametros = new Dictionary<string, object>();
                var sql = new StringBuilder();

                parametros.Add("@ID", obj.Id != 0 ? Convert.ToString(obj.Id) : "GEN_ID(GN_ALT_PLAN_CONTRATO, 1)");
                parametros.Add("@ID_VENDA", Convert.ToString(obj.IdVenda));
                parametros.Add("@ID_PLANO_ANTERIOR", obj.IdPlanoAnterior != null ? Convert.ToString(obj.IdPlanoAnterior) : "NULL");
                parametros.Add("@ID_CONTRATO_ANTERIOR", obj.IdPlanoAnterior != null ? Convert.ToString(obj.IdContratoAnterior) : "NULL");
                parametros.Add("@ID_PLANO", Convert.ToString(obj.IdPlano));

                parametros.Add("@TIPO_ALTERACAO", Convert.ToInt32(obj.Alteracao));
                parametros.Add("@DATAHORACRIACAO", !obj.DataHoraCriacao.HasValue ? "CURRENT_TIMESTAMP" : obj.DataHoraCriacao.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                parametros.Add("@DATAHORAALTERACAO", obj.DataHoraCriacao.HasValue ? "CURRENT_TIMESTAMP" : "NULL");
                parametros.Add("@STATUS", Convert.ToInt32(obj.Status));

                sql.AppendLine("UPDATE OR INSERT INTO TB_ALTERACAO_PLAN_CONTRATO(ID, ID_VENDA, ID_CONTRATO_ANTERIOR, ID_PLANO_ANTERIOR, ID_PLANO, DATAHORACRIACAO, DATAHORAALTERACAO,STATUS)")
                    .AppendLine($" VALUES(@ID, @ID_VENDA, @ID_CONTRATO_ANTERIOR, @ID_PLANO_ANTERIOR, @ID_PLANO, CURRENT_TIMESTAMP, @DATAHORAALTERACAO, @STATUS)");
                
                return await CommandHelper.ExecuteNonQueryReturning(comm, sql, new Dictionary<string,object>(), this._session.Connection, null);
            }
            #endregion
        }

        public async Task<Connect.Model.nhibernate.Venda> BindVenda(AlteracaoPlanoContratoCadastroModel view, string user) 
        {
            #region [BindVenda]
            using (var repositoryVenda = new Conexao<Connect.Model.nhibernate.Venda>(this._session))
            using (var repositoryFuncionario = new Conexao<Sinapse.Model.Funcionario>(this._session))
            using (var repositoryDepartamento = new Conexao<Sinapse.Model.Departamento>(this._session))
            using (var repositoryPerfil = new Conexao<Perfil_Vendedor>(this._session))
            using (var repositoryEnderecoCliente = new Conexao<Endereco_Cliente>(this._session))            
            using (var repositoryMaterialPlano = new Conexao<MaterialOs>(this._session))            
            {
                var vendedor = (await repositoryFuncionario.selectFirst(p => p.Id == view.Id_Vendedor));
                var departamento = await repositoryDepartamento.selectFirst(p => p.Id == view.Id_Departamento);

                if (view.IdVenda == 0)
                {
                    return new Connect.Model.nhibernate.Venda
                    {
                        Status = Status_Venda.Analise_Cliente,
                        EnderecoInstalacao = await repositoryEnderecoCliente.selectFirst(p => p.Id == view.PlanoProduto.Id_Endereco),
                        Mudanca_Plano = true,
                        Tipo_Alteracao = view.TipoAlteracao,
                        DataHoraCriacao = DateTime.Now,
                        Origem_Venda = "CONNECT",
                        //PerfilVendedor = ,
                        Data_Venda = DateTime.Now,
                        Departamento = departamento,

                        Data_Ultimo_Contato = DateTime.Now,
                        //IdConsultaPendenciaFinanceira = view.VendaBase.IdConsultaPendenciaFinanceira,                                        
                        //Consulta_Financeiro_Aprovada = view.VendaBase.Consulta_Financeiro_Aprovada,
                        //Consulta_Financeiro_Realizada = view.VendaBase.Consulta_Financeiro_Realizada,  

                        Id_Cliente = view.Id_Cliente,
                        Id_Comissionamento = vendedor?.Id_Comissionamento,
                        Id_Meta = vendedor?.Meta?.Id,
                        Id_Faixa = vendedor?.Faixa?.Id,
                        Vendedor = vendedor,
                        UsuarioCriacao = user,
                        Supervisor = (await repositoryFuncionario.selectFirst(p => p.Id == view.Id_Supervisor)),
                    };
                }
                else return await repositoryVenda.selectFirst(p => p.Id == view.IdVenda);
            }
            #endregion
        }
        public async Task BindEtapasVenda(AlteracaoPlanoContratoCadastroModel view, Connect.Model.nhibernate.Venda venda, string user, Cliente_PlanoProduto cliente_PlanoProduto)
        {
            #region [BindEtapasVenda]
            var listagem = GetDadosEtapasVenda(view.IdVenda);
            EtapaVendaViewModel ultimaEtapa = listagem.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado && e.Etapa != 7) ||
                                                                            e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente);

            // Caso todas as etapas tenham sido aprovadas a etapa atual será a última etapa da venda
            if (ultimaEtapa == null)
            {
                ultimaEtapa = listagem.LastOrDefault();
            }

            Connect.Model.nhibernate.Plano plano = null;

            var intetapaAtual = ultimaEtapa.Etapa;

            var listagemConcluidas =
                listagem != null &&
                listagem.Where(t => t.Etapa < 7 && t.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente).Count() == 0;            

            using (var repositoryEtapaVenda = new Conexao<Connect.Model.nhibernate.EtapaVenda>(this._session))
            using (var repositoryLogs = new Conexao<LogEtapaVendas>(this._sessionLog))
            {
                Connect.Model.nhibernate.EtapaVenda modelo = null;
                Connect.Model.nhibernate.EtapaVenda modeloAnterior = null;
                bool existemEtapas = (await repositoryEtapaVenda.select(p => p.Venda == venda)).Count != 0;
                if (!existemEtapas)
                {
                    foreach (EtapaVendaViewModel etapa in listagem)
                    {
                        Connect.Model.nhibernate.EtapaVenda etapaVenda = null;
                        if (etapa.Etapa != Convert.ToInt64(EnumEtapaVenda.OrdemServico) || (cliente_PlanoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS || cliente_PlanoProduto.Plano.OS_Mudanca_Plano))
                        {
                            etapaVenda = new Connect.Model.nhibernate.EtapaVenda
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = user,

                                Etapa = (EnumEtapaVenda)etapa.Etapa,
                                Descricao = etapa.Descricao,

                                // O status deve ser preenchido no input hidden que existe na view
                                Status = (StatusEtapaVenda)etapa.Status,
                                Venda = venda,
                            };
                            //Log de mudança de status da primeira etapa
                            if ((Sinapse.Model.enumerados.StatusEtapaVenda)etapa.Status != StatusEtapaVenda.Pendente)
                            {
                                modelo = (Connect.Model.nhibernate.EtapaVenda)etapaVenda.Clone();
                                modelo.Status = StatusEtapaVenda.Pendente;
                                var logs = etapaVenda.GeraLog(modelo, user, "", Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLogs.Save(logs);
                            }
                            await repositoryEtapaVenda.Save(etapaVenda);
                        }
                    }
                }
                else
                {
                    EtapaVendaViewModel etapaAtual = listagem.Where(e => e.Etapa == intetapaAtual).FirstOrDefault();

                    //Log 
                    Connect.Model.nhibernate.EtapaVenda etapaVenda = new Connect.Model.nhibernate.EtapaVenda
                    {
                        DataHoraCriacao = DateTime.Now,
                        UsuarioCriacao = user,
                        Etapa = (EnumEtapaVenda)etapaAtual.Etapa,
                        Descricao = etapaAtual.Descricao,
                        Status = (StatusEtapaVenda)etapaAtual.Status,
                        Venda = venda
                    };

                    modelo = await repositoryEtapaVenda.selectFirst(p => p.Venda.Id == venda.Id && p.Etapa == (EnumEtapaVenda)etapaAtual.Etapa);

                    if (modelo != null)
                    {
                        modelo.Status = StatusEtapaVenda.Pendente;
                        var logs = etapaVenda.GeraLog(modelo, user, "", Sinapse.ModelLog.TipoOperLog.Alteracao); 
                        await repositoryLogs.Save(logs);
                    }


                    if (etapaAtual != null)
                    {
                        //etapaVendaBusiness.MudarStatusEtapa(venda.Id, (EnumEtapaVenda)etapaAtual.Etapa, (StatusEtapaVenda)etapaAtual.Status);
                    }
                }

                venda.Etapa_Atual = VerificaUltimaEtapa(venda.Id).Result;                
            }
            #endregion
        }

        public async Task<long?> GetVenda(long? idVenda, List<string>condicoes, Dictionary<string, object> parametros) 
        {
            #region [GetVenda]
            if (!idVenda.HasValue)
            {                
                using (var comm = this._session.Connection.CreateCommand())                
                {                    
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID_VENDA FROM TB_CLIENTE_PLANO_PRODUTO A ")
                       .AppendLine("LEFT JOIN TB_CLIENTE C ON A.ID_CLIENTE = C.ID LEFT JOIN TB_CLIENTE_CONTRATO D ")
                       .AppendLine("ON A.ID_CLIENTE_CONTRATO = D.ID")
                       .AppendLine("LEFT JOIN TB_PLANO E ON A.ID_PLANO = E.ID");
                   
                    if (condicoes.Count > 0)
                    {
                        sql.AppendLine($"WHERE {string.Join(" AND ", condicoes)}");
                    }
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, this._session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            idVenda = reader["ID_VENDA"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENDA"]) : (long?)null;
                        }
                    }
                }
            }            
            return idVenda.Value;
            #endregion
        }

        public async Task<IEnumerable<IObjComboResult<long>>> GetComboContrato(List<string> condicoes, Dictionary<string, object> parametros, int page)
        {
            #region [GetComboContrato]
            var listagem = new List<IObjComboResult<long>>();
            using (var comm = this._session.Connection.CreateCommand()) 
            {
                var sql = new StringBuilder();
                var sqlFrom = new StringBuilder();
                var sqlWhere = new StringBuilder();

                sql.AppendLine("SELECT A.ID, A.NUMERO_CONTRATO, A.DESCRICAO");

                sqlFrom.AppendLine("FROM TB_CLIENTE_CONTRATO A")
                       .AppendLine(" LEFT JOIN TB_CLIENTE B ON A.ID_CLIENTE = B.ID ")
                       .AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO C ON A.ID = C.ID_CLIENTE_CONTRATO ")
                       .AppendLine(" LEFT JOIN TB_PLANO D ON C.ID_PLANO = D.ID");
                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine($"WHERE {string.Join(" AND ", condicoes)}");
                }
                sql.AppendLine(sqlFrom.ToString())
                   .AppendLine(sqlWhere.ToString());
                sql.AppendLine("GROUP BY A.ID, A.NUMERO_CONTRATO, A.DESCRICAO");

                using (var reader = await CommandHelper.ExecuteReaderPage(comm, sql, parametros, this._session.Connection, null, page))
                {
                    while (await reader.ReadAsync())
                    {
                        var id = Convert.ToInt64(reader["Id"]);
                        var desc = Convert.ToString(reader["Descricao"]);
                        listagem.Add(new ObjComboResult<long>
                        {
                            Id = id,
                            Text = $"{id} - {desc}",
                        });
                    }
                }
            }
            return listagem;
            #endregion
        }
       
        public async Task<(long Venda, long ClientePlanoProduto)> GetVendaByContratoPlano(long cliente, long contrato, long plano)
        {
            #region [GetVendaByContratoPlano]
            long IdVenda = 0;
            long IdClientePlanoProduto = 0;
            using (var comm = this._session.Connection.CreateCommand()) 
            {                 
                var sql = new StringBuilder("SELECT B.ID AS VENDA, C.ID FROM TB_CLIENTE_CONTRATO A INNER JOIN TB_VENDA B ")
                                .AppendLine("ON B.ID_CLIENTE = A.ID_CLIENTE INNER JOIN TB_CLIENTE_PLANO_PRODUTO C ON C.ID_VENDA = B.ID ")
                                .AppendLine($"WHERE A.ID = {contrato} AND C.ID_PLANO = {plano}");
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._session.Connection, null))                 
                {
                    if (await reader.ReadAsync()) 
                    {
                        IdVenda = Convert.ToInt64(reader["VENDA"]);
                        IdClientePlanoProduto = Convert.ToInt64(reader["ID"]);
                    }
                }

            }
            return (IdVenda,IdClientePlanoProduto);
            #endregion
        }

        public async Task<long?> GetClientePlanoProdutoByPlano(long cliente, long plano) 
        {
            #region [GetClientePlanoProdutoByPlano]
            long? IdClientePlanoProduto = null;
            using (var comm = this._session.Connection.CreateCommand()) 
            {
                var sql = new StringBuilder($"SELECT A.ID FROM TB_CLIENTE_PLANO_PRODUTO A WHERE A.ID_CLIENTE = {cliente} AND A.ID_PLANO = {plano} ORDER BY A.ID DESC");
                using (var reader = await CommandHelper.ExecuteReaderFirst(comm, sql, new Dictionary<string,object>(),this._session.Connection, null, 1, 1)) 
                {
                    if (await reader.ReadAsync())
                    {
                        IdClientePlanoProduto = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null;
                    }
                }
            }
            return IdClientePlanoProduto;
            #endregion
        }
        public Task<GetApiRetorno<VendaDadosGeraisViewModel>> GerarVendaByClientePlano(long cliente, long plano)
        {            
            return null;
        }

        public async Task<long> GetPlanoByContrato(long cliente, long contrato)
        {
            #region [GetPlanoByContrato]
            long idPlano = 0;
            using (var comm = this._session.Connection.CreateCommand()) 
            {
                var sql = new StringBuilder("SELECT A.ID FROM TB_PLANO A INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ON B.ID_PLANO = A.ID LEFT JOIN TB_CLIENTE_CONTRATO C ON B.ID_CLIENTE_CONTRATO  = C.ID  ")
                                .AppendLine($" WHERE C.ID = {contrato}");
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._session.Connection, null)) 
                {
                    if (await reader.ReadAsync())
                    {
                        idPlano = Convert.ToInt64(reader["ID"]);
                    }
                }
            }
            return idPlano;
            #endregion
        }
        
        public async Task<IEnumerable<MaterialosCadastroModel>> GetMateriaisPlanoOs(long plano) 
        {
            #region [GetMateriaisPlanoOs]
            List<MaterialosCadastroModel> materiais = new List<MaterialosCadastroModel>();
            using (var comm = this._session.Connection.CreateCommand())
            {
                var sql = new StringBuilder($"SELECT A.ID, A.CODIGO, A.DESCRICAO FROM TB_MATERIAL_OS A Where A.Id_Plano = {plano}");
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._session.Connection, null))
                {
                    while (await reader.ReadAsync()) 
                    {
                        var obj = new MaterialosCadastroModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Codigo = Convert.ToString(reader["CODIGO"]),
                            Descricao = Convert.ToString(reader["DESCRICAO"]),
                        };
                        
                        materiais.Add(obj);
                    }
                }
            }
            return materiais;
            #endregion
        }
        
        private IEnumerable<EtapaVendaViewModel> GetDadosEtapasVenda(long idVenda)
        {
            #region [GetDadosEtapasVenda]
            var etapasVenda = new List<EtapaVendaViewModel>();
            using (var repo = new Conexao<Connect.Model.nhibernate.EtapaVenda>(this._session))
            {
                var etapasEncontradas = idVenda != 0 ? repo.select(p => p.Venda.Id == idVenda).Result : new List<Connect.Model.nhibernate.EtapaVenda>();

                if (idVenda == 0 || etapasEncontradas.Count() == 0)
                {
                    IEnumerable<EnumEtapaVenda> etapasHabilitadas = this.GetEtapasVendaHabilitadas(idVenda);

                    foreach (EnumEtapaVenda etapa in etapasHabilitadas)
                    {
                        switch (etapa)
                        {                            
                            case EnumEtapaVenda.ConsultaPendenciaFinanceira:
                                {
                                    etapasVenda.Add(new EtapaVendaViewModel
                                    {
                                        Etapa = (int)EnumEtapaVenda.ConsultaPendenciaFinanceira,
                                        Descricao = "Consulta de Pendência Financeira",
                                        Status = (int)StatusEtapaVenda.Aprovado
                                    });

                                    break;
                                }
                            
                            case EnumEtapaVenda.TermoAdesao:
                                {
                                    etapasVenda.Add(new EtapaVendaViewModel
                                    {
                                        Etapa = (int)EnumEtapaVenda.TermoAdesao,
                                        Descricao = "Termo de Adesão",
                                        Status = (int)StatusEtapaVenda.Pendente
                                    });

                                    break;
                                }
                            case EnumEtapaVenda.OrdemServico:
                                {
                                    etapasVenda.Add(new EtapaVendaViewModel
                                    {
                                        Etapa = (int)EnumEtapaVenda.OrdemServico,
                                        Descricao = "Ordem de Serviço",
                                        Status = (int)StatusEtapaVenda.Pendente
                                    });

                                    break;
                                }
                        }
                    }
                }
                else
                {
                    etapasVenda = etapasEncontradas
                            .Select(e => new EtapaVendaViewModel
                            {
                                Id = Convert.ToInt32(e.Id),
                                Etapa = (int)e.Etapa,
                                Descricao = e.Descricao,
                                Status = (int)e.Status,
                                MotivoAprovacaoManual = e.MotivoAprovManual
                            })
                            .OrderBy(e => e.Etapa)
                            .ToList();

                }
            }

            return etapasVenda;
            #endregion
        }
        private async Task<Connect.Model.nhibernate.EtapaVenda> VerificaUltimaEtapa(long idVenda)
        {
            #region [VerificaUltimaEtapa]
            using (var repo = new Conexao<Connect.Model.nhibernate.EtapaVenda>(this._session))
            {
                var etapasVenda = await repo.select(p => p.Venda.Id == idVenda);

                Connect.Model.nhibernate.EtapaVenda ultimaEtapaPendente = etapasVenda.FirstOrDefault(e => e.Status == StatusEtapaVenda.Reprovado || e.Status == StatusEtapaVenda.Pendente);
                var descEtapa = ultimaEtapaPendente ?? etapasVenda.LastOrDefault();

                return descEtapa;
            }
            #endregion
        }
        private IEnumerable<EnumEtapaVenda> GetEtapasVendaHabilitadas(long idVenda)
        {
            #region [GetEtapasVendaHabilitadas]
            using (var repo = new Conexao<Connect.Model.nhibernate.EtapaVenda>(this._session))
            using (var repoConfig = new Conexao<Connect.Model.nhibernate.Configuracoes_Gerais>(this._session))
            {
                var etapasEncontradas = idVenda != 0 ? repo.select(p => p.Venda.Id == idVenda).Result : new List<Connect.Model.nhibernate.EtapaVenda>();

                // Caso a venda seja nova pegamos as etapas das configurações da empresa
                if (idVenda == 0 || etapasEncontradas.Count() == 0)
                {
                    // A lista é inicializada com as etapas que não podem ser desabilitadas
                    // nas configurações da empresa
                    var etapas = new List<EnumEtapaVenda>
                {
                    EnumEtapaVenda.ConsultaPendenciaFinanceira,
                    EnumEtapaVenda.TermoAdesao,
                    EnumEtapaVenda.OrdemServico
                    //EnumEtapaVenda.Resumo,
                };

                    Configuracoes_Gerais config = repoConfig.selectFirst().Result;

                    if (config.ConsultaViabilidadeExibeVenda)
                    {
                        etapas.Add(EnumEtapaVenda.ConsultaViabilidade);
                    }

                    if (config.ConsultaPendenciaFinanceiraExibeVenda)
                    {
                        etapas.Add(EnumEtapaVenda.ConsultaPendenciaFinanceira);
                    }

                    if (config.ConsultaPendenciaEnderecoExibeVenda)
                    {
                        etapas.Add(EnumEtapaVenda.ConsultaPendenciaEndereco);
                    }

                    if (config.ConsultaCreditoExibeVenda)
                    {
                        etapas.Add(EnumEtapaVenda.ConsultaCredito);
                    }

                    //etapas.Add(EnumEtapaVenda.CartaoCredito);

                    return etapas.OrderBy(etapa => etapa);
                }
                // Caso a venda já tenha sido gravada pegamos as etapas da TB_ETAPA_VENDA
                else
                {
                    return etapasEncontradas.OrderBy(e => e.Etapa)
                                            .Select(e => e.Etapa);
                }
            }
            #endregion
        }

        public async Task Save(Connect.Model.nhibernate.Venda venda)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Venda>(this._session)) 
            {
                await repo.Save(venda);
            }
        }
        public async Task Update(Connect.Model.nhibernate.Venda venda)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Venda>(this._session))
            {
                await repo.Update(venda);
            }
        }
        public async Task SavePlanoProduto(Cliente_PlanoProduto clientePlanoProduto)
        {
            using (var repo = new Conexao<Cliente_PlanoProduto>(this._session))
            {
                await repo.SaveOrUpdate(clientePlanoProduto);
            }
        }

        public async Task SaveLog(List<LogAlteracaoPlanoContrato> logs)
        {
            using (var repo = new Conexao<LogAlteracaoPlanoContrato>(this._sessionLog)) 
            {
                await repo.Save(logs.ToList());
            }
        }
    }
}
