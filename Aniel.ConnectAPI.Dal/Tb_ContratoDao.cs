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
    public class Tb_ContratoDao : DaoBase
    {
        private Tb_ContratoDao()
        {

        }

        public Tb_ContratoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Contrato> GetPv(String Codct)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.CODCT, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_CONTRATO A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Codct))
            {
                sql.AppendLine(" AND A.CODCT = @CODCT");
                parametros.Add("@CODCT", Codct);
            }

            List<Tb_Pv_Contrato> retorno = new List<Tb_Pv_Contrato>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Contrato
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Codct = Convert.ToString(reader["CODCT"]),
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

        public Boolean AtualizacaoPv(String Codct)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_CONTRATO A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT");

            parametros.Add("@CODCT", Codct);

            List<Tb_Pv_Contrato> retorno = new List<Tb_Pv_Contrato>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Codct, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_CONTRATO A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT");

            parametros.Add("@LOG", Log);
            parametros.Add("@CODCT", Codct);

            List<Tb_Pv_Contrato> retorno = new List<Tb_Pv_Contrato>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Contrato> Get(List<String> Contratos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _contratosStr = from o in Contratos
                             select String.Format("'{0}'", o);

            sql.AppendLine("SELECT CODCT, CODEMPRESA,CODFILIAL,LOCALIDADE, FORNEC_MAT_PADRAO, TIPOSALDO, TIPOSALDOAPLIC, CODCT_REGULADOR, PROJETO_REGULADOR");
            sql.AppendLine("FROM TB_CONTRATO A");
            sql.AppendLine(String.Format("WHERE A.CODCT IN ({0})", String.Join(',', _contratosStr)));

            List<Tb_Contrato> retorno = new List<Tb_Contrato>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Contrato
                {
                    CodCt = Convert.ToString(reader["CodCt"]),
                    CodCt_Regulador = Convert.ToString(reader["CodCt_Regulador"]),
                    CodEmpresa = reader["CodEmpresa"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["CodEmpresa"].ToString()),
                    CodFilial = reader["CodFilial"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["CodFilial"].ToString()),
                    Fornec_Mat_Padrao = Convert.ToString(reader["Fornec_Mat_Padrao"]),
                    Localidade = Convert.ToString(reader["Localidade"]),
                    Projeto_Regulador = Convert.ToString(reader["Projeto_Regulador"]),
                    TipoSaldo = Convert.ToString(reader["TipoSaldo"]),
                    TipoSaldoAplic = Convert.ToString(reader["TipoSaldoAplic"])
                });
            }

            return retorno;
        }
    }
}
