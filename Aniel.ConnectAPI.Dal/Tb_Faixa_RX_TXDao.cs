using Aniel.ConnectAPI.Dal.Helper;
using Aniel.ConnectAPI.Dal;
using Aniel.ConnectAPI.Model;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.ConnectAPI.Model.Aniel;
using Aniel.ConnectAPI.Model.Temporarios;

namespace Aniel.ConnectAPI.Dal.Aniel
{
    public class Tb_Faixa_RX_TXDao : DaoBase
    {
        private Tb_Faixa_RX_TXDao()
        {

        }

        public Tb_Faixa_RX_TXDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Faixa_Rx_Tx> GetPv(String Codct, String Projeto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.ID_FAIXA, A.CODCT, A.PROJETO, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Faixa_Rx_Tx A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Codct))
            {
                sql.AppendLine(" AND A.CODCT = @CODCT");
                parametros.Add("@CODCT", Codct);
            }

            if (!String.IsNullOrEmpty(Projeto))
            {
                sql.AppendLine(" AND A.PROJETO = @PROJETO");
                parametros.Add("@PROJETO", Projeto);
            }

            List<Tb_Pv_Faixa_Rx_Tx> retorno = new List<Tb_Pv_Faixa_Rx_Tx>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Faixa_Rx_Tx
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_Faixa = Convert.ToInt64(reader["ID_FAIXA"]),
                    CodCt = Convert.ToString(reader["CODCT"]),
                    Projeto = Convert.ToString(reader["PROJETO"]),
                    Usuario = Convert.ToString(reader["USUARIO"]),
                    Dia = reader["DIA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA"].ToString()),
                    Hora = reader["HORA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA"].ToString()),
                    Executado = Convert.ToString(reader["EXECUTADO"]),
                    Dia_Execucao = reader["DIA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA_EXECUCAO"].ToString()),
                    Hora_Execucao = reader["HORA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA_EXECUCAO"].ToString()),
                    Log = Convert.ToString(reader["LOG"])
                });
            }

            return retorno;
        }

        public Boolean AtualizacaoPv(Int64 Id_Faixa, String Codct, String Projeto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Faixa_Rx_Tx A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND ID_FAIXA = @ID_FAIXA");

            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);
            parametros.Add("@ID_FAIXA", Id_Faixa);

            List<Tb_Pv_Faixa_Rx_Tx> retorno = new List<Tb_Pv_Faixa_Rx_Tx>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int64 Id_Faixa, String Codct, String Projeto, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Faixa_Rx_Tx A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND ID_FAIXA = @ID_FAIXA");

            parametros.Add("@LOG", Log);
            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);
            parametros.Add("@ID_FAIXA", Projeto);

            List<Tb_Pv_Faixa_Rx_Tx> retorno = new List<Tb_Pv_Faixa_Rx_Tx>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Faixas_Rx_Tx> Get(List<ChaveFaixa> Projetos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _projetosStr = from o in Projetos
                                select String.Format("(ID = {0} AND CODCT = '{1}' AND PROJETO = '{2}')", o.Id_Faixa, o.Codct, o.Projeto);

            sql.AppendLine("SELECT ID, CODCT, PROJETO, MIN_RX, MAX_RX, MIN_TX, MAX_TX, MIN_SNR, MAX_SNR");
            sql.AppendLine("FROM TB_FAIXAS_RX_TX_SNR A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _projetosStr)));

            List<Tb_Faixas_Rx_Tx> retorno = new List<Tb_Faixas_Rx_Tx>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Faixas_Rx_Tx
                {
                    Id= reader["Id"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Id"].ToString()),
                    Codct = Convert.ToString(reader["CodCt"]),
                    Projeto = Convert.ToString(reader["Projeto"]),
                    
                    Max_Rx = reader["Max_Rx"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Max_Rx"].ToString()),
                    Max_Snr = reader["Max_Snr"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Max_Snr"].ToString()),
                    Max_Tx = reader["Max_Tx"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Max_Tx"].ToString()),
                    Min_Rx = reader["Min_Rx"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Min_Rx"].ToString()),
                    Min_Snr = reader["Min_Snr"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Min_Snr"].ToString()),
                    Min_Tx = reader["Min_Tx"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Min_Tx"].ToString()),

                });
            }

            return retorno;
        }
    }
}
