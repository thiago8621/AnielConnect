using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.T5.InventarioCliente
{
    public class InventarioClienteAnielRepository: IInventarioClienteAnielRepository
    {
        private readonly ISession _sessionT5Sql;
        private readonly ITransaction _transactionT5Sql;

        public InventarioClienteAnielRepository(ISession sessionT5Sql, ITransaction transactionT5Sql)
        {
            _sessionT5Sql = sessionT5Sql;
            _transactionT5Sql = transactionT5Sql;
        }

        public async Task<bool> ExisteInventarioPendenteAvaliacao(string cpf_cnpj, string codMat, string serial)
        {
            bool retorno = false;

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.Append("SELECT I.SEQUENCIAL ");
                sql.Append("FROM TB_INVENTARIO_CLIENTE I ");
                sql.Append("LEFT JOIN TB_INVENTARIO_CLIENTE_MAT M ON I.SEQUENCIAL = M.SEQUENCIAL ");
                sql.Append("INNER JOIN TB_MATERIAL MAT ON MAT.codmat = M.codmat ");
                sql.Append("INNER JOIN TB_CLIENTE C ON C.COD_CLIENTE = I.ID_CLIENTE ");
                sql.Append("WHERE I.DATA BETWEEN DATEADD(DAY, -30, CURRENT_DATE) AND CURRENT_DATE AND C.CNPJ = @CPFCNPJ AND I.STATUS = @STATUS ");

                if(!String.IsNullOrEmpty(codMat) && !String.IsNullOrEmpty(serial))
                {
                    sql.Append("AND UPPER(M.CODMAT) = UPPER(@CODMAT) AND UPPER(M.CODCPL) = UPPER(@SERIAL) ");

                    parametros.Add("@CODMAT", codMat);
                    parametros.Add("@SERIAL", serial);
                }
                
                parametros.Add("@CPFCNPJ", cpf_cnpj);
                parametros.Add("@STATUS", 0); //Aguardando Avaliação

                this._transactionT5Sql?.Enlist(com);
                using (var _reader = await CommandHelper.ExecuteReader(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction))
                {
                    if (await _reader.ReadAsync())
                        retorno = true;
                }
            }                      

            return retorno;
        }

        public async Task<Int64> GerarProtocoloInvCapa(Aniel.T5.Model.nhibernate.ClienteAniel cliente, string usuarioCriacao)
        {
            Int64 retorno = 0;

            using (var com = this._sessionT5Sql.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.Append("INSERT INTO TB_INVENTARIO_CLIENTE(SEQUENCIAL, ID_CLIENTE, DATA, NOME_CLIENTE, ORIGEM, CPF_CLIENTE, USUARIO_CRIACAO, STATUS) ");
                sql.Append("VALUES(@SEQUENCIAL, @CLIENTE, @DATA, @NOME, @ORIGEM, @CPF, @USUARIO_CRIACAO, @STATUS) ");
                sql.Append("RETURNING SEQUENCIAL ");

                var proxseq = await getProximoSequencialInventario();
                parametros.Add("@SEQUENCIAL", proxseq);
                parametros.Add("@CLIENTE", cliente.Cod_Cliente);
                parametros.Add("@DATA", DateTime.Now);
                parametros.Add("@ORIGEM", 2); //Origem Connect
                parametros.Add("@NOME", cliente.NomeFantasia);
                parametros.Add("@CPF", cliente.CPF);
                parametros.Add("@USUARIO_CRIACAO", usuarioCriacao);
                parametros.Add("@STATUS", 0); //Aguardando Avaliação

                this._transactionT5Sql?.Enlist(com);
                var gravou = await CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction);
                if (gravou > 0)
                {
                    retorno = Convert.ToInt64(proxseq);
                }
            }
            
            return retorno;
        }

        public async Task InserirMaterialCliente(string codMat, string serial, string usuario, Int64 protocolo)
        {           
            using (var com = this._sessionT5Sql.Connection.CreateCommand()) 
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.Append("INSERT INTO TB_INVENTARIO_CLIENTE_MAT(ID, SEQUENCIAL, CODMAT, CODCPL, USUARIO, DIA, HORA, QTDE_LEVANTADA) ");
                sql.Append("VALUES(@ID, @SEQUENCIAL, @CODMAT, @CODCPL, @USUARIO, CURRENT_DATE, CURRENT_TIME, @QTDE_LEVANTADA) ");

                var proxId = (await getProximoSequencialMaterial()).ToString();

                parametros.Add("@ID", proxId);
                parametros.Add("@SEQUENCIAL", String.Format("{0:0000000000}", protocolo));
                parametros.Add("@USUARIO", usuario);
                parametros.Add("@QTDE_LEVANTADA", 1);
                parametros.Add("@CODMAT", codMat);

                if (!String.IsNullOrEmpty(serial))
                {
                    parametros.Add("@CODCPL", serial);
                }
                else
                {
                    parametros.Add("@CODCPL", "-");
                }

                this._transactionT5Sql?.Enlist(com);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionT5Sql.Connection, com.Transaction);
            }                                 
        }

        public async Task<string> getProximoSequencialInventario()
        {
            int proxSequencial = 0;
            using (var com = this._sessionT5Sql.Connection.CreateCommand()) 
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT COALESCE(MAX(CAST(A.SEQUENCIAL AS INTEGER)),0) + 1 AS SEQ FROM TB_INVENTARIO_CLIENTE A ");

                this._transactionT5Sql?.Enlist(com);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, this._sessionT5Sql.Connection, com.Transaction))
                if (await reader.ReadAsync())
                {
                    proxSequencial = Convert.ToInt32(reader["SEQ"].ToString());
                }
            }            
            
            return String.Format("{0:0000000000}", proxSequencial);
        }

        public async Task<long> getProximoSequencialMaterial()
        {
            int proxId = 0;

            using (var com = this._sessionT5Sql.Connection.CreateCommand()) 
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT COALESCE(MAX(CAST(A.ID AS INTEGER)),0) + 1 AS ID FROM TB_INVENTARIO_CLIENTE_MAT A");

                this._transactionT5Sql?.Enlist(com);
                using (var reader = await CommandHelper.ExecuteReader(com, sql, this._sessionT5Sql.Connection, com.Transaction))
                if (await reader.ReadAsync())
                {
                    proxId = Convert.ToInt32(reader["ID"].ToString());
                }
            }
            
            return proxId;
        }
    }
}
