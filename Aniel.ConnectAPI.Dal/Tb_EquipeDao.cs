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

namespace Aniel.ConnectAPI.Dal.Aniel
{
    public class Tb_EquipeDao : DaoBase
    {
        private Tb_EquipeDao()
        {

        }

        public Tb_EquipeDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Equipe> GetPv(String Equipe)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.EQUIPE, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_EQUIPE A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Equipe))
            {
                sql.AppendLine(" AND A.EQUIPE = @EQUIPE");
                parametros.Add("@EQUIPE", Equipe);
            }

            List<Tb_Pv_Equipe> retorno = new List<Tb_Pv_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Equipe
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Equipe = Convert.ToString(reader["EQUIPE"]),
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

        public Boolean AtualizacaoPv(String Equipe)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_EQUIPE A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.EQUIPE = @EQUIPE");

            parametros.Add("@EQUIPE", Equipe);

            List<Tb_Pv_Equipe> retorno = new List<Tb_Pv_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Equipe, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_EQUIPE A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND AND A.EQUIPE = @EQUIPE");

            parametros.Add("@LOG", Log);
            parametros.Add("@EQUIPE", Equipe);

            List<Tb_Pv_Equipe> retorno = new List<Tb_Pv_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Equipe> Get(List<String> Equipes)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _equipeStr = from o in Equipes
                             select String.Format("'{0}'", o);

            sql.AppendLine("SELECT A.EQUIPE, A.EQUIPEAUX, A.NUM_FRE, A.NOME, A.COD_SUPERV, A.COD_FORNEC_SERV, A.COD_FORNECEDOR, A.COD_PERFIL_EQUIPE,");
            sql.AppendLine("       A.COD_REGIAO, A.ENCERRAMENTO");
            sql.AppendLine("FROM TB_EQUIPE A");
            sql.AppendLine(String.Format("WHERE A.EQUIPE IN ({0})", String.Join(',', _equipeStr)));

            List<Tb_Equipe> retorno = new List<Tb_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Equipe
                {
                    Equipe = Convert.ToString(reader["Equipe"]),
                    EquipeAux = Convert.ToString(reader["EquipeAux"]),
                    Num_Fre = Convert.ToString(reader["NUM_FRE"]),
                    Nome = Convert.ToString(reader["Nome"]),
                    Cod_Perfil_Equipe = reader["Cod_Perfil_Equipe"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["Cod_Perfil_Equipe"].ToString()),
                    Cod_Regiao = reader["Cod_Regiao"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["Cod_Regiao"].ToString()),
                    Cod_Superv = Convert.ToString(reader["Cod_Superv"]),
                    Encerramento = reader["Encerramento"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["Encerramento"].ToString())
                });
            }

            return retorno;
        }
    }
}
