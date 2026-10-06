using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Core;

namespace Aniel.Connect.Repository.T5.ElementoRedePorta
{
    public class ElementoRedePortaAnielRepository : IElementoRedePortaAnielRepository
    {
        private readonly ISession _sessionNh;
        private readonly ISession _sessionT5Nh;

        public ElementoRedePortaAnielRepository(ISession sessionNh, ISession sessionT5Nh)
        {
            this._sessionNh = sessionNh;
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<Elemento_Rede_Porta> GetAll()
        {
            using (var repo = new Conexao<Elemento_Rede_Porta>(this._sessionT5Nh))
            {
                return repo.QueryableSelect();
            }
        }

        public Elemento_Rede_Porta GetById(long id)
        {
            return this.GetAll()
                       .Where(e => e.Id == id)
                       .FirstOrDefault();
        }

        public Elemento_Rede_Porta GetByNumeroPorta(long idCaixa, int porta)
        {
            return this.GetAll()
                       .Where(e => e.Caixa.Id == idCaixa && e.Porta == porta)
                       .FirstOrDefault();
        }
        public async Task<bool> LimparPorta(Elemento_ReservaCadastroModel request)
        {
            bool isCommited = false;

            try
            {
                var parametros = new Dictionary<String, Object>();
                parametros.Add("@CLIENTEPLANOPRODUTO", request.Cliente_PlanoProduto_Novo);
                StringBuilder sql = new StringBuilder(),
                              updateSql = new StringBuilder();
                using (var com = this._sessionNh.Connection.CreateCommand())
                {
                    sql.AppendLine(@"SELECT B.LOGIN
                                      FROM TB_LOGIN_RADIUS B
                                      WHERE  B.ID_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO");

                    this._sessionNh.Transaction.Enlist(com);
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, this._sessionNh.Connection, com.Transaction))
                    {
                        while (await reader.ReadAsync())
                        {
                            request.Login = Convert.ToString(reader["LOGIN"]);

                        }
                    }
                }

                using (var comt5 = this._sessionT5Nh.Connection.CreateCommand())
                {
                    updateSql.AppendLine(@"
                            UPDATE TB_ELEMENTO_REDE_PORTA A SET A.LOGIN = null,
                            A.CLIENTE_PLANO_PRODUTO = NULL,A.STATUS = 1, A.ID_LOGIN = NULL
                            WHERE A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO");

                    this._sessionT5Nh.Transaction.Enlist(comt5);
                    await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, this._sessionT5Nh.Connection, comt5.Transaction);
                }

                isCommited = true;
            }
            catch (Exception)
            {
                isCommited = false;
            }
            return isCommited;


        }
        public async Task<bool> AlterarPortaPlano(Elemento_ReservaCadastroModel request, bool ocuparPorta)
        {
            bool isCommited = false;

            try
            {
                var parametros = new Dictionary<String, Object>();
                parametros.Add("@CLIENTEPLANOPRODUTO_NOVO", request.Cliente_PlanoProduto_Novo);
                parametros.Add("@CLIENTEPLANOPRODUTO_ANTIGO", request.Cliente_PlanoProduto_Antigo);
                string query = string.Empty;
                if (ocuparPorta)
                {
                    parametros.Add("@STATUS", 5);
                    query = @", A.STATUS = @STATUS";
                }

                StringBuilder sql = new StringBuilder(),
                              updateSql = new StringBuilder();
                using (var comt5 = this._sessionT5Nh.Connection.CreateCommand())
                {
                    updateSql.AppendLine(@"UPDATE TB_ELEMENTO_REDE_PORTA A SET A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO_NOVO");
                    updateSql.AppendLine(query);
                    updateSql.AppendLine(@"WHERE A.CLIENTE_PLANO_PRODUTO = @CLIENTEPLANOPRODUTO_ANTIGO");

                    this._sessionT5Nh.Transaction.Enlist(comt5);
                    await CommandHelper.ExecuteNonQuery(comt5, updateSql, parametros, this._sessionT5Nh.Connection, comt5.Transaction);
                }
                
                isCommited = true;
            }
            catch (Exception)
            {
                isCommited = false;
            }


            return isCommited;
        }
        public async Task<Elemento_Reserva> GetParametrosReservaPortaByClientePlanoProduto(ReservaRequest request)
        {
            var requestReserva = new Elemento_Reserva();
            var sql = new StringBuilder();
            var sqlT5 = new StringBuilder();

            sql.AppendLine(" SELECT FIRST 1 A.ID, A.ID_CLIENTE, C.ID as ID_LOGIN, C.LOGIN  FROM TB_VENDA A INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ON B.ID_VENDA = A.ID LEFT JOIN TB_LOGIN_RADIUS C ON C.ID_PLANO_PRODUTO = B.ID")
               .AppendLine($" WHERE B.ID = {request.ClientePlanoProduto}");

            sqlT5.AppendLine(" SELECT FIRST 1 A.ID FROM TB_ELEMENTO_REDE_PORTA A")
                 .AppendLine($" WHERE A.ID_CAIXA = {request.Caixa} AND (A.STATUS = 1)");
            using (var comm = this._sessionNh.Connection.CreateCommand())
            {
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._sessionNh.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        requestReserva.Cod_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : 0;
                        requestReserva.Id_Login = reader["ID_LOGIN"] != DBNull.Value ? Convert.ToInt64(reader["ID_LOGIN"]) : (long?)null;
                    }
                }
            }
            using (var comm = this._sessionT5Nh.Connection.CreateCommand())
            {
                using (var reader = await CommandHelper.ExecuteReader(comm, sqlT5, this._sessionT5Nh.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        requestReserva.Id_Porta = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null;
                    }
                }
            }
            requestReserva.Cliente_PlanoProduto = request.ClientePlanoProduto ?? 0;
            return requestReserva;
        }
        public async Task<GetApiRetorno<CaixaPortaReservada>> GetPortaReservadaClientePlanoProduto(CaixaPortaReservadaRequest request)
        {
            var retorno = new GetApiRetorno<CaixaPortaReservada> { Objeto = new CaixaPortaReservada() };

            using (var comm = this._sessionT5Nh.Connection.CreateCommand())
            {
                var sql = new StringBuilder();

                sql.AppendLine($" SELECT A.ID, A.PORTA, A.ID_CAIXA, B.DESCRICAO FROM TB_ELEMENTO_REDE_PORTA A ")
                   .AppendLine($" INNER JOIN TB_ELEMENTO_REDE B ON B.ID = A.ID_CAIXA ")
                   .AppendLine($" WHERE A.CLIENTE_PLANO_PRODUTO = {request.ClientePlanoProduto}");

                using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._sessionT5Nh.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno.Objeto.Id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0;
                        retorno.Objeto.IdCaixa = reader["ID_CAIXA"] != DBNull.Value ? Convert.ToInt32(reader["ID_CAIXA"]) : 0;
                        retorno.Objeto.Caixa = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : String.Empty;
                        retorno.Objeto.Porta = reader["PORTA"] != DBNull.Value ? Convert.ToString(reader["PORTA"]) : String.Empty;
                    }
                }
            }
            retorno.Ok = true;
            return retorno;
        }
    }
}
