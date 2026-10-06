using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;

using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Venda
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ISession _sessionNh;

        public VendaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public Connect.Model.nhibernate.Venda GetById(long id)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Venda>(this._sessionNh))
            {
                return repo.selectFirst(p => p.Id == id).Result;
            }
        }

        public IEnumerable<Cliente_PlanoProduto> GetPlanosProdutosVenda(long id)
        {
            using (var repo = new Conexao<Cliente_PlanoProduto>(this._sessionNh))
            {
                return repo.QueryableSelect()
                           .Where(cpp => cpp.Id_Venda == id);
            }
        }

        public async Task<IEnumerable<ClienteContratoModel>> GetPlanosProdutosVendaSQL(long id)
        {
            var retorno = new List<ClienteContratoModel>();
            
            using (var comm = this._sessionNh.Connection.CreateCommand())
            {
                var sql = new StringBuilder();
                sql.AppendLine(" SELECT A.ID, A.NUMERO_CONTRATO, A.COD_INTEGRACAO, B.ID AS CLIENTE_PLANO_PRODUTO, C.ID AS VENDA ")
                   .AppendLine(" FROM TB_CLIENTE_CONTRATO A INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ON B.ID_CLIENTE_CONTRATO = A.ID ")
                   .AppendLine(" INNER JOIN TB_VENDA C ON B.ID_VENDA = C.ID")
                   .AppendLine($" WHERE C.ID = {id} AND (B.ID_PLANO IS NOT NULL OR B.ID_COMBO IS NOT NULL)");
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._sessionNh.Connection, null))
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ClienteContratoModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            IdVenda = Convert.ToInt64(reader["VENDA"]),
                            IdClientePlanoProduto = Convert.ToInt64(reader["CLIENTE_PLANO_PRODUTO"]),
                            CodIntegracao = Convert.ToString(reader["COD_INTEGRACAO"]),
                            NumeroContrato = Convert.ToString(reader["NUMERO_CONTRATO"]),
                        });
                    }
                return retorno;
            }
        }

        public void Save(Aniel.Connect.Model.nhibernate.Venda venda)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Venda>(this._sessionNh))
            {
                repo.Save(venda).Wait();
            }
        }

        public void Update(Aniel.Connect.Model.nhibernate.Venda venda)
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Venda>(this._sessionNh))
            {
                repo.Update(venda).Wait();
            }
        }
        public async Task<IEnumerable<AtividadeVendaRequestModel>> GetAtividadesReprovacaoAutomatica(long venda, long atividade = 0) 
        {
            var condicoes = new List<string>();
            var atividades = new List<AtividadeVendaRequestModel>();
            using (var comm = this._sessionNh.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine(" SELECT B.ID, B.ATIV_STATUSVENDA AS STATUS, B.ATIV_DESCRICAO AS DESCRICAO, ")
                   .AppendLine(" B.ATIV_NATIVREPROVADA AS ATIVIDADE_REPROVADA FROM TB_WORKFLOW_VENDA A  INNER JOIN TB_WORKFLOW_ATIVIDADE B ON A.ID = B.ID_WORKFLOW_VENDAS ")
                   .AppendLine(" INNER JOIN TB_VENDA C ON C.ID_WORKFLOW_VENDAS = A.ID");
                
                condicoes.Add($" C.ID = {venda}");

                if (atividade != 0)
                {
                    condicoes.Add($" B.ATIV_DESCRICAO = {atividade}");
    }
                if (condicoes.Count > 0) 
                {
                    sql.AppendLine($" WHERE {String.Join(" AND ", condicoes)}"); 
}
                sql.AppendLine(" ORDER BY B.ID");
                var parametros = new Dictionary<string, object>();
                using (DbDataReader reader = await CommandHelper.ExecuteReader(comm, sql, parametros, this._sessionNh.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var atividadeBanco = new AtividadeVendaRequestModel
                        {
                            Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null,
                            Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : null,
                            AtividadeReprovada = reader["ATIVIDADE_REPROVADA"] != DBNull.Value ? Convert.ToInt64(reader["ATIVIDADE_REPROVADA"]) : (long?)null,
                            Status = reader["STATUS"] != DBNull.Value ? Convert.ToInt16(reader["STATUS"]) : (short?)null,
                        };

                        atividades.Add(atividadeBanco);
                    }
                }
            }
            return atividades;
        }
        public async Task<AtividadeVendaRequestModel> GetItemReprovacaoAutomaticaByAtividade(long venda, long atividade)
        {
            return (await this.GetAtividadesReprovacaoAutomatica(venda, atividade)).First();
        }
        public async Task ReprovaAtividadesWorkflowVendaAutomatico(ReprovacaoAutomaticaComandos request) 
        {
            long atividade = 0;
            //Execução das consultas                
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            {
                sessionLog.BeginTransaction();
                this._sessionNh.BeginTransaction();
                foreach (var sql in request.ComandosSql)
                {                    
                    using (var comm = this._sessionNh.Connection.CreateCommand())
                    {
                        this._sessionNh.Transaction.Enlist(comm);
                        await CommandHelper.ExecuteNonQuery(comm, sql, this._sessionNh.Connection, comm.Transaction);
                    }
                    using (var comm = this._sessionNh.Connection.CreateCommand())
                    {
                        var QueryConnect = new StringBuilder($"SELECT A.ID_WORKFLOW_VENDAS_ATIVIDADE AS ATV FROM TB_VENDA A WHERE A.ID = {request.Venda}");
                        this._sessionNh.Transaction.Enlist(comm);
                        using (var reader = await CommandHelper.ExecuteReader(comm, QueryConnect, this._sessionNh.Connection, comm.Transaction))
                        {
                            while (reader.Read())
                            {
                                atividade = Convert.ToInt64(reader["ATV"]);
                            }
                        }
                    }

                    await this.GeraLogAtividadeReprovada(request, atividade, sessionLog);
                }
            
                await this._sessionNh.Transaction.CommitAsync();
                await sessionLog.Transaction.CommitAsync();
            }                                 
        }
        public async Task GeraLogAtividadeReprovada(ReprovacaoAutomaticaComandos request, long atividade, ISession sessionLog) 
        {                                           
            using (var comm = sessionLog.Connection.CreateCommand())
            {
                var parametros = new Dictionary<string, object>();
                var sqlLog = new StringBuilder();
                sqlLog.AppendLine(" INSERT INTO TB_VENDAS (ID, TIPOOPERACAO,IDLOG,CAMPO_VALORANTES, ")
                      .AppendLine(" CAMPO_VALORDEPOIS, USUARIOCRIACAO, NOMEUSUARIOCRIACAO, DATAHORACRIACAO, CAMPO)")
                      .AppendLine(" VALUES ((SELECT GEN_ID(GN_VENDAS, 1) FROM RDB$DATABASE), @OPERACAO, @VENDA, @ANTES, @DEPOIS,")
                      .AppendLine(" @USUARIO, @NOME, @DATA, @MOTIVO)");

                parametros.Add("@OPERACAO", TipoOperacao.Integracao);
                parametros.Add("@VENDA", request.Venda);
                parametros.Add("@ANTES", request.Atividades.Dequeue());
                parametros.Add("@DEPOIS", atividade);
                parametros.Add("@USUARIO", request.Usuario);
                parametros.Add("@NOME", request.Nome);
                parametros.Add("@DATA", DateTime.Now);
                parametros.Add("@MOTIVO", "Reprovação de Workflow por cancelamento em massa");

                sessionLog.Transaction.Enlist(comm);
                await CommandHelper.ExecuteNonQuery(comm, sqlLog, parametros, sessionLog.Connection, comm.Transaction);
            }                      
        }
        public IEnumerable<Atividade_StatusVenda> GetAtivStatusVenda() 
        {
            using (var repository = new Conexao<Atividade_StatusVenda>(this._sessionNh)) 
            {
                return repository.QueryableSelect().OrderBy(p => p.Id);
            }
        }

        public IQueryable<Atividade_StatusVenda> GetAllStatusVenda()
        {
            using (var repository = new Conexao<Atividade_StatusVenda>(this._sessionNh))
            {
                return repository.QueryableSelect().OrderBy(p => p.Id);
            }
        }
    }
}
