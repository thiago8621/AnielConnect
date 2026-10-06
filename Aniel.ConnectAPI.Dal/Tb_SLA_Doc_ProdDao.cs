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
    public class Tb_SLA_Doc_ProdDao : DaoBase
    {
        private Tb_SLA_Doc_ProdDao()
        {

        }

        public Tb_SLA_Doc_ProdDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_SLA_Doc_Prod> GetPv(Int64? Id_SLA_Prod)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Id_SLA_Prod, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_SLA_Prod A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Id_SLA_Prod.HasValue)
            {
                sql.AppendLine(" AND A.Id_SLA_Prod = @Id_SLA_Prod");
                parametros.Add("@Id_SLA_Prod", Id_SLA_Prod);
            }

            List<Tb_Pv_SLA_Doc_Prod> retorno = new List<Tb_Pv_SLA_Doc_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_SLA_Doc_Prod
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_SLA_Prod = reader["Id_SLA_Prod"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Id_SLA_Prod"].ToString()),
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

        public Boolean AtualizacaoPv(Int64 Id_SLA_Prod)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_SLA_Prod A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Id_SLA_Prod = @Id_SLA_Prod");

            parametros.Add("@Id_SLA_Prod", Id_SLA_Prod);

            List<Tb_Pv_SLA_Doc_Prod> retorno = new List<Tb_Pv_SLA_Doc_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int64 Id_SLA_Prod, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_SLA_Prod A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Id_SLA_Prod = @Id_SLA_Prod");

            parametros.Add("@LOG", Log);
            parametros.Add("@Id_SLA_Prod", Id_SLA_Prod);

            List<Tb_Pv_SLA_Doc_Prod> retorno = new List<Tb_Pv_SLA_Doc_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_SLA_Doc_Prod> Get(List<Int64> Criticidades)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _CriticidadesStr = from o in Criticidades
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT A.ID, A.COD_TIPO_SERV, A.ID_CATEGORIA, A.ID_CRITICIDADE, A.PORC_INI, A.PORC_FIM, A.COR");
            sql.AppendLine("FROM TB_SLA_DOC_PROD A");
            sql.AppendLine(String.Format("WHERE A.Id_SLA_Prod IN ({0})", String.Join(',', _CriticidadesStr)));

            List<Tb_SLA_Doc_Prod> retorno = new List<Tb_SLA_Doc_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_SLA_Doc_Prod
                {
                    Id = reader["ID"].Equals(DBNull.Value) ? (Int64)0 : Convert.ToInt64(reader["ID"].ToString()),
                    Cod_Tipo_Serv = reader["Cod_Tipo_Serv"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Tipo_Serv"].ToString()),
                    Id_Categoria = reader["Id_Categoria"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Id_Categoria"].ToString()),
                    Id_Criticidade = reader["Id_Criticidade"].Equals(DBNull.Value) ? (Int64)0 : Convert.ToInt64(reader["Id_Criticidade"].ToString()),
                    Porc_Fim = reader["Porc_Fim"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Porc_Fim"].ToString()),
                    Porc_Ini = reader["Porc_Ini"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Porc_Ini"].ToString()),
                    Cor = reader["Cor"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cor"].ToString()),
                });
            }

            return retorno;
        }
    }
}
