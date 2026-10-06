using Aniel.ConnectAPI.Dal.Helper;
using Aniel.ConnectAPI.Model.Aniel;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aniel.ConnectAPI.Dal
{
    public class Tb_CfopDao: DaoBase
    {
        public Tb_CfopDao()
        {

        }

        public Tb_CfopDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Cfop> GetPv(String CodCfop)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.COD_CFOP, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_CFOP A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(CodCfop))
            {
                sql.AppendLine(" AND A.Cod_CFOP = @Cod_CFOP");
                parametros.Add("@Cod_CFOP", CodCfop);
            }

            List<Tb_Pv_Cfop> retorno = new List<Tb_Pv_Cfop>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Cfop
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Codigo = reader["COD_CFOP"].Equals(DBNull.Value) ? "" : Convert.ToString(reader["COD_CFOP"].ToString()),
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

        public Boolean AtualizacaoPv(String CodCfop)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_CFOP A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.COD_CFOP = @COD_CFOP");

            parametros.Add("@COD_CFOP", CodCfop);

            List<Tb_Pv_Cfop> retorno = new List<Tb_Pv_Cfop>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String CodCfop, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_CFOP A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.COD_CFOP = @COD_CFOP");

            parametros.Add("@LOG", Log);
            parametros.Add("@COD_CFOP", CodCfop);

            List<Tb_Pv_Departamento> retorno = new List<Tb_Pv_Departamento>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Cfop> Get(List<String> Cfops)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _CfopStr = from o in Cfops
                           select String.Format("{0}", o);

            sql.AppendLine("SELECT CFOP, DESCRICAO");
            sql.AppendLine("FROM TB_CFOP A");
            sql.AppendLine(String.Format("WHERE A.CFOP IN ({0})", String.Join(',', _CfopStr)));

            List<Tb_Cfop> retorno = new List<Tb_Cfop>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Cfop
                {
                    Cfop = reader["CFOP"].Equals(DBNull.Value) ? "" : reader["CFOP"].ToString(),                   
                    Descricao = Convert.ToString(reader["DESCRICAO"])
                });
            }

            return retorno;
        }
    }
}
