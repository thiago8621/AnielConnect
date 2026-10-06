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
    public class Tb_EmpresaDao : DaoBase
    {
        private Tb_EmpresaDao()
        {

        }

        public Tb_EmpresaDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Empresa> GetPv(Int16? Codempresa)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Codempresa, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_Empresa A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Codempresa.HasValue)
            {
                sql.AppendLine(" AND A.Codempresa = @Codempresa");
                parametros.Add("@Codempresa", Codempresa);
            }

            List<Tb_Pv_Empresa> retorno = new List<Tb_Pv_Empresa>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Empresa
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Codempresa = reader["CODEMPRESA"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CODEMPRESA"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 Codempresa)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Empresa A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Codempresa = @Codempresa");

            parametros.Add("@Codempresa", Codempresa);

            List<Tb_Pv_Empresa> retorno = new List<Tb_Pv_Empresa>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Codempresa, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Empresa A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Codempresa = @Codempresa");

            parametros.Add("@LOG", Log);
            parametros.Add("@Codempresa", Codempresa);

            List<Tb_Pv_Empresa> retorno = new List<Tb_Pv_Empresa>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Empresa> Get(List<Int16> Empresas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _EmpresasStr = from o in Empresas
                               select String.Format("{0}", o);

            sql.AppendLine("SELECT Codempresa, EMPRESA, RAZAO_SOCIAL, DATA_ALTERACAO, LOGOTIPO");
            sql.AppendLine("FROM TB_Empresa A");
            sql.AppendLine(String.Format("WHERE A.Codempresa IN ({0})", String.Join(',', _EmpresasStr)));

            List<Tb_Empresa> retorno = new List<Tb_Empresa>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Empresa
                {
                    Codempresa = reader["Codempresa"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CodEmpresa"].ToString()),
                    Empresa = Convert.ToString(reader["Empresa"]),
                    Razao_Social = Convert.ToString(reader["Razao_Social"]),
                    Data_Alteracao = reader["DATA_ALTERACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ALTERACAO"]) : (DateTime ?)null,
                    Logotipo = GetBytes(reader, 4)
                });
            }

            return retorno;
        }

        private byte[] GetBytes(FbDataReader reader, int ordinal)
        {
            byte[] result = null;

            if (!reader.IsDBNull(ordinal))
            {
                long size = reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data 
                result = new byte[size];
                int bufferSize = 1024;
                long bytesRead = 0;
                int curPos = 0;
                while (bytesRead < size)
                {
                    bytesRead += reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
                    curPos += bufferSize;
                }
            }

            return result;
        }
    }
}
