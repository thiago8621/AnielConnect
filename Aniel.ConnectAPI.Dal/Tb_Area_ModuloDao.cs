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
    public class Tb_Area_ModuloDao : DaoBase
    {
        private Tb_Area_ModuloDao()
        {

        }

        public Tb_Area_ModuloDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Area_Modulo> GetPv(String Sigla_Modulo, String Cod_Area)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Sigla_Modulo, A.Cod_Area, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Area_Modulo A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Sigla_Modulo))
            {
                sql.AppendLine(" AND A.Sigla_Modulo = @Sigla_Modulo");
                parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            }

            if (!String.IsNullOrEmpty(Cod_Area))
            {
                sql.AppendLine(" AND A.Cod_Area = @Cod_Area");
                parametros.Add("@Cod_Area", Cod_Area);
            }

            List<Tb_Pv_Area_Modulo> retorno = new List<Tb_Pv_Area_Modulo>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Area_Modulo
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
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

        public Boolean AtualizacaoPv(String Sigla_Modulo, String Cod_Area)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Area_Modulo A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Sigla_Modulo = @Sigla_Modulo AND A.Cod_Area = @Cod_Area");

            parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            parametros.Add("@Cod_Area", Cod_Area);

            List<Tb_Pv_Area_Modulo> retorno = new List<Tb_Pv_Area_Modulo>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Sigla_Modulo, String Cod_Area, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Area_Modulo A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Sigla_Modulo = @Sigla_Modulo AND A.Cod_Area = @Cod_Area");

            parametros.Add("@LOG", Log);
            parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            parametros.Add("@Cod_Area", Cod_Area);

            List<Tb_Pv_Area_Modulo> retorno = new List<Tb_Pv_Area_Modulo>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Area_Modulo> Get(List<ChaveArea_Modulo> Cod_Areas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Cod_AreasStr = from o in Cod_Areas
                                select String.Format("(Sigla_Modulo = '{0}' AND Cod_Area = '{1}')", o.Sigla_Modulo, o.Cod_Area);

            sql.AppendLine("SELECT Sigla_Modulo, Cod_Area, DESCRICAO, SUB_ACESSO");
            sql.AppendLine("FROM TB_Area_Modulo A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Cod_AreasStr)));

            List<Tb_Area_Modulo> retorno = new List<Tb_Area_Modulo>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Area_Modulo
                {
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Descricao = Convert.ToString(reader["Descricao"]),
                    Sub_Acesso = Convert.ToString(reader["Sub_Acesso"])                    
                });
            }

            return retorno;
        }
    }
}
