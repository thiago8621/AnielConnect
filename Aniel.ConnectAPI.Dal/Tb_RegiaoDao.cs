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
    public class Tb_RegiaoDao : DaoBase
    {
        private Tb_RegiaoDao()
        {

        }

        public Tb_RegiaoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Regiao> GetPv(Int16? Cod_Regiao)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Regiao, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_Regiao A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_Regiao.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Regiao = @Cod_Regiao");
                parametros.Add("@Cod_Regiao", Cod_Regiao);
            }

            List<Tb_Pv_Regiao> retorno = new List<Tb_Pv_Regiao>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Regiao
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Regiao = reader["Cod_Regiao"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Regiao"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 Cod_Regiao)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Regiao A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Regiao = @Cod_Regiao");

            parametros.Add("@Cod_Regiao", Cod_Regiao);

            List<Tb_Pv_Regiao> retorno = new List<Tb_Pv_Regiao>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Cod_Regiao, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Regiao A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Regiao = @Cod_Regiao");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Regiao", Cod_Regiao);

            List<Tb_Pv_Regiao> retorno = new List<Tb_Pv_Regiao>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Regiao> Get(List<Int16> Regiaos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _RegiaosStr = from o in Regiaos
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT Cod_Regiao, NOME");
            sql.AppendLine("FROM TB_Regiao A");
            sql.AppendLine(String.Format("WHERE A.Cod_Regiao IN ({0})", String.Join(',', _RegiaosStr)));

            List<Tb_Regiao> retorno = new List<Tb_Regiao>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Regiao
                {
                    Cod_Regiao = reader["Cod_Regiao"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Regiao"].ToString()),
                    Nome = Convert.ToString(reader["NOME"])
                });
            }

            return retorno;
        }
    }
}
