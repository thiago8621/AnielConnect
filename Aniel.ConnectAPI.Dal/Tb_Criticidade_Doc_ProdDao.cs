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
    public class Tb_Criticidade_Doc_ProdDao : DaoBase
    {
        private Tb_Criticidade_Doc_ProdDao()
        {

        }

        public Tb_Criticidade_Doc_ProdDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Criticidade_Prod> GetPv(Int64? Criticidade_Prod)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Criticidade_Prod, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Criticidade_Prod A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Criticidade_Prod.HasValue)
            {
                sql.AppendLine(" AND A.Criticidade_Prod = @Criticidade_Prod");
                parametros.Add("@Criticidade_Prod", Criticidade_Prod);
            }

            List<Tb_Pv_Criticidade_Prod> retorno = new List<Tb_Pv_Criticidade_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Criticidade_Prod
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Criticidade_Prod = reader["Criticidade_Prod"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Criticidade_Prod"].ToString()),
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

        public Boolean AtualizacaoPv(Int64 Criticidade_Prod)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Criticidade_Prod A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Criticidade_Prod = @Criticidade_Prod");

            parametros.Add("@Criticidade_Prod", Criticidade_Prod);

            List<Tb_Pv_Criticidade_Prod> retorno = new List<Tb_Pv_Criticidade_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int64 Criticidade_Prod, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Criticidade_Prod A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Criticidade_Prod = @Criticidade_Prod");

            parametros.Add("@LOG", Log);
            parametros.Add("@Criticidade_Prod", Criticidade_Prod);

            List<Tb_Pv_Criticidade_Prod> retorno = new List<Tb_Pv_Criticidade_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Criticidade_Doc_Prod> Get(List<Int64> Criticidades)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _CriticidadesStr = from o in Criticidades
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT ID, DESCRICAO");
            sql.AppendLine("FROM Tb_Criticidade_Doc_Prod A");
            sql.AppendLine(String.Format("WHERE A.Criticidade_Prod IN ({0})", String.Join(',', _CriticidadesStr)));

            List<Tb_Criticidade_Doc_Prod> retorno = new List<Tb_Criticidade_Doc_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Criticidade_Doc_Prod
                {
                    Id = reader["ID"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["ID"].ToString()),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
