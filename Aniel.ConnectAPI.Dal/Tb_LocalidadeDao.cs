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
    public class Tb_LocalidadeDao : DaoBase
    {
        private Tb_LocalidadeDao()
        {

        }

        public Tb_LocalidadeDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Localidade> GetPv(String Cod_Escritorio, String Cod_Area, String Localidade)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Escritorio, A.LOCALIDADE, A.Cod_Area, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Localidade A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Cod_Escritorio))
            {
                sql.AppendLine(" AND A.Cod_Escritorio = @Cod_Escritorio");
                parametros.Add("@Cod_Escritorio", Cod_Escritorio);
            }

            if (!String.IsNullOrEmpty(Cod_Area))
            {
                sql.AppendLine(" AND A.Cod_Area = @Cod_Area");
                parametros.Add("@Cod_Area", Cod_Area);
            }

            if (!String.IsNullOrEmpty(Localidade))
            {
                sql.AppendLine(" AND A.Localidade = @Localidade");
                parametros.Add("@Localidade", Localidade);
            }

            List<Tb_Pv_Localidade> retorno = new List<Tb_Pv_Localidade>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Localidade
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Escritorio = Convert.ToString(reader["Cod_Escritorio"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Localidade = Convert.ToString(reader["Localidade"]),
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

        public Boolean AtualizacaoPv(String Cod_Escritorio, String Cod_Area, String Localidade)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Localidade A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Escritorio = @Cod_Escritorio AND A.Cod_Area = @Cod_Area AND A.Localidade = @Localidade");

            parametros.Add("@Cod_Escritorio", Cod_Escritorio);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Localidade", Localidade);

            List<Tb_Pv_Localidade> retorno = new List<Tb_Pv_Localidade>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Cod_Escritorio, String Cod_Area, String Localidade, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Localidade A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Escritorio = @Cod_Escritorio AND A.Cod_Area = @Cod_Area AND A.LOCALIDADE = @Localidade");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Escritorio", Cod_Escritorio);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Localidade", Localidade);           

            List<Tb_Pv_Localidade> retorno = new List<Tb_Pv_Localidade>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Localidade> Get(List<ChaveLocalidade> Cod_Areas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Cod_AreasStr = from o in Cod_Areas
                                select String.Format("(Cod_Escritorio = '{0}' AND Cod_Area = '{1}' AND Localidade = '{2}')", o.Cod_Escritorio, o.Cod_Area, o.Localidade);

            sql.AppendLine("SELECT Cod_Escritorio, Cod_Area, Localidade, Cod_Localidade");
            sql.AppendLine("FROM TB_Localidade A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Cod_AreasStr)));

            List<Tb_Localidade> retorno = new List<Tb_Localidade>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Localidade
                {
                    Cod_Escritorio = Convert.ToString(reader["Cod_Escritorio"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Localidade = Convert.ToString(reader["Localidade"]),
                    Cod_Localidade = Convert.ToInt32(reader["Cod_Localidade"])
                });
            }

            return retorno;
        }
    }
}
