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
    public class Tb_DepartamentoDao : DaoBase
    {
        private Tb_DepartamentoDao()
        {

        }

        public Tb_DepartamentoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Departamento> GetPv(Int16? CodDepartamento)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.COD_DEPTO, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_FOLHA_DEPTO A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (CodDepartamento.HasValue)
            {
                sql.AppendLine(" AND A.Cod_DEPTO = @Cod_DEPTO");
                parametros.Add("@Cod_DEPTO", CodDepartamento);
            }

            List<Tb_Pv_Departamento> retorno = new List<Tb_Pv_Departamento>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Departamento
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Depto = reader["Cod_DEPTO"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_DEPTO"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 CodDepartamento)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_FOLHA_DEPTO A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_DEPTO = @Cod_DEPTO");

            parametros.Add("@Cod_DEPTO", CodDepartamento);

            List<Tb_Pv_Departamento> retorno = new List<Tb_Pv_Departamento>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 CodDepartamento, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_FOLHA_DEPTO A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_DEPTO = @Cod_DEPTO");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_DEPTO", CodDepartamento);

            List<Tb_Pv_Departamento> retorno = new List<Tb_Pv_Departamento>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Folha_Depto> Get(List<Int16> Departamentos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _DepartamentosStr = from o in Departamentos
                               select String.Format("{0}", o);

            sql.AppendLine("SELECT Cod_DEPTO, NUM_DEPTO, DESCRICAO");
            sql.AppendLine("FROM TB_FOLHA_DEPTO A");
            sql.AppendLine(String.Format("WHERE A.Cod_DEPTO IN ({0})", String.Join(',', _DepartamentosStr)));

            List<Tb_Folha_Depto> retorno = new List<Tb_Folha_Depto>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Folha_Depto
                {
                    Cod_DEPTO = reader["Cod_DEPTO"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_DEPTO"].ToString()),
                    Num_DEPTO = Convert.ToString(reader["NUM_DEPTO"]),
                    Descricao = Convert.ToString(reader["DESCRICAO"])
                });
            }

            return retorno;
        }
    }
}
