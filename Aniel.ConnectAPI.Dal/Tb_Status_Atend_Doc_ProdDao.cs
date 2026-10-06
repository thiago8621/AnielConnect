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

namespace Aniel.ConnectAPI.Dal.Aniel
{
    public class Tb_Status_Atend_Doc_ProdDao : DaoBase
    {
        private Tb_Status_Atend_Doc_ProdDao()
        {

        }

        public Tb_Status_Atend_Doc_ProdDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_St_Atend_Docprod> GetPv(Int16? Status)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Status, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_St_Atend_Docprod A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Status.HasValue)
            {
                sql.AppendLine(" AND A.Status = @Status");
                parametros.Add("@Status", Status.Value);
            }

            List<Tb_Pv_St_Atend_Docprod> retorno = new List<Tb_Pv_St_Atend_Docprod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_St_Atend_Docprod
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Status = reader["STATUS"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["STATUS"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 Status)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_St_Atend_Docprod A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Status = @Status");

            parametros.Add("@Status", Status);

            List<Tb_Pv_St_Atend_Docprod> retorno = new List<Tb_Pv_St_Atend_Docprod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Status, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_St_Atend_Docprod A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND AND A.Status = @Status");

            parametros.Add("@LOG", Log);
            parametros.Add("@Status", Status);

            List<Tb_Pv_St_Atend_Docprod> retorno = new List<Tb_Pv_St_Atend_Docprod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Status_Atend_Doc_Prod> Get(List<Int16> Statuss)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _StatusStr = from o in Statuss
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT A.Status, A.Descricao");
            sql.AppendLine("FROM TB_STATUS_ATEND_DOC_PROD A");
            sql.AppendLine(String.Format("WHERE A.Status IN ({0})", String.Join(',', _StatusStr)));

            List<Tb_Status_Atend_Doc_Prod> retorno = new List<Tb_Status_Atend_Doc_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Status_Atend_Doc_Prod
                {
                    Status = reader["Status"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Status"].ToString()),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
