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
    public class Tb_Centro_CustoDao : DaoBase
    {
        private Tb_Centro_CustoDao()
        {

        }

        public Tb_Centro_CustoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Centro_Custo> GetPv(Int32? Cod_CCusto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_CCusto, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Centro_Custo A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_CCusto.HasValue)
            {
                sql.AppendLine(" AND A.Cod_CCusto = @Cod_CCusto");
                parametros.Add("@Cod_CCusto", Cod_CCusto);
            }

            List<Tb_Pv_Centro_Custo> retorno = new List<Tb_Pv_Centro_Custo>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Centro_Custo
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_CCusto = reader["Cod_Ccusto"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Cod_Ccusto"].ToString()),
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

        public Boolean AtualizacaoPv(Int32 Cod_CCusto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Centro_Custo A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_CCusto = @Cod_CCusto");

            parametros.Add("@Cod_CCusto", Cod_CCusto);

            List<Tb_Pv_Centro_Custo> retorno = new List<Tb_Pv_Centro_Custo>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Cod_CCusto, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Centro_Custo A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_CCusto = @Cod_CCusto");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_CCusto", Cod_CCusto);

            List<Tb_Pv_Centro_Custo> retorno = new List<Tb_Pv_Centro_Custo>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Centro_Custo> Get(List<Int32> Cod_CCustos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            var _Cod_CCustoStr = from o in Cod_CCustos
                                 select String.Format("{0}", o);

            sql.AppendLine("SELECT A.COD_CCUSTO, A.CODAUX, A.DESCRICAO");
            sql.AppendLine("FROM TB_CENTRO_CUSTO A");
            sql.AppendLine(String.Format("WHERE A.Cod_CCusto IN ({0})", String.Join(',', _Cod_CCustoStr)));

            List<Tb_Centro_Custo> retorno = new List<Tb_Centro_Custo>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Centro_Custo
                {
                    Cod_Ccusto = reader["Cod_Ccusto"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Cod_Ccusto"].ToString()),
                    Cod_Aux = Convert.ToString(reader["CodAux"]),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
