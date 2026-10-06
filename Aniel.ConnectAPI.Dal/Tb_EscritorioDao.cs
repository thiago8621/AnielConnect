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
    public class Tb_EscritorioDao : DaoBase
    {
        private Tb_EscritorioDao()
        {

        }

        public Tb_EscritorioDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Escritorio> GetPv(String Escritorio)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Escritorio, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Escritorio A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Escritorio))
            {
                sql.AppendLine(" AND A.Cod_Escritorio = @Cod_Escritorio");
                parametros.Add("@Cod_Escritorio", Escritorio);
            }

            List<Tb_Pv_Escritorio> retorno = new List<Tb_Pv_Escritorio>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Escritorio
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Escritorio = Convert.ToString(reader["Cod_Escritorio"]),
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

        public Boolean AtualizacaoPv(String Cod_Escritorio)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Escritorio A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Escritorio = @Cod_Escritorio");

            parametros.Add("@Cod_Escritorio", Cod_Escritorio);

            List<Tb_Pv_Escritorio> retorno = new List<Tb_Pv_Escritorio>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Cod_Escritorio, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Escritorio A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND AND A.Cod_Escritorio = @Cod_Escritorio");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Escritorio", Cod_Escritorio);

            List<Tb_Pv_Escritorio> retorno = new List<Tb_Pv_Escritorio>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Escritorio> Get(List<String> Escritorios)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _EscritorioStr = from o in Escritorios
                             select String.Format("'{0}'", o);

            sql.AppendLine("SELECT A.Cod_Escritorio, A.Descricao");
            sql.AppendLine("FROM TB_Escritorio A");
            sql.AppendLine(String.Format("WHERE A.Cod_Escritorio IN ({0})", String.Join(',', _EscritorioStr)));

            List<Tb_Escritorio> retorno = new List<Tb_Escritorio>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Escritorio
                {
                    Cod_Escritorio = Convert.ToString(reader["Cod_Escritorio"]),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
