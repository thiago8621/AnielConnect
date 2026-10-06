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
    public class Tb_Codigo_EncerramentoDao : DaoBase
    {
        private Tb_Codigo_EncerramentoDao()
        {

        }

        public Tb_Codigo_EncerramentoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Codigo_Encerramento> GetPv(String Cod_Encerramento, String Codct)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.COD_ENCERRAMENTO, A.CODCT, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_COD_ENCERRAMENTO A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Cod_Encerramento))
            {
                sql.AppendLine(" AND A.Cod_Encerramento = @Cod_Encerramento");
                parametros.Add("@Cod_Encerramento", Cod_Encerramento);
            }

            if (!String.IsNullOrEmpty(Codct))
            {
                sql.AppendLine(" AND A.Codct = @Codct");
                parametros.Add("@Codct", Codct);
            }

            List<Tb_Pv_Codigo_Encerramento> retorno = new List<Tb_Pv_Codigo_Encerramento>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Codigo_Encerramento
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                    Codct = Convert.ToString(reader["CODCT"]),
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

        public Boolean AtualizacaoPv(String Cod_Encerramento, String Codct)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Cod_Encerramento A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Encerramento = @Cod_Encerramento AND A.Codct = @Codct");

            parametros.Add("@Cod_Encerramento", Cod_Encerramento);
            parametros.Add("@Codct", Codct);

            List<Tb_Pv_Codigo_Encerramento> retorno = new List<Tb_Pv_Codigo_Encerramento>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Cod_Encerramento, String Codct, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Cod_Encerramento A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Encerramento = @Cod_Encerramento AND A.Codct = @Codct");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Encerramento", Cod_Encerramento);
            parametros.Add("@Codct", Codct);

            List<Tb_Pv_Codigo_Encerramento> retorno = new List<Tb_Pv_Codigo_Encerramento>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Codigo_Encerramento> Get(List<ChaveCod_Encerramento> Filiais)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _projetosStr = from o in Filiais
                               select String.Format("(Cod_Encerramento = '{0}' AND Codct = '{1}')", o.Cod_Encerramento, o.Codct);

            sql.AppendLine("SELECT Cod_Encerramento, Codct, Descricao, Status_Execucao");
            sql.AppendLine("FROM TB_Codigo_Encerramento A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _projetosStr)));

            List<Tb_Codigo_Encerramento> retorno = new List<Tb_Codigo_Encerramento>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Codigo_Encerramento
                {
                    Cod_Encerramento = Convert.ToString(reader["Cod_Encerramento"]),
                    Codct = Convert.ToString(reader["Codct"]),
                    Descricao = Convert.ToString(reader["Descricao"]),
                    Status_Execucao = reader["Status_Execucao"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Status_Execucao"].ToString())
                });
            }

            return retorno;
        }
    }
}
