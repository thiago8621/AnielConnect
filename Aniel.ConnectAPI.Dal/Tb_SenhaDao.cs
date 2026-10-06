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
    public class Tb_SenhaDao : DaoBase
    {
        private Tb_SenhaDao()
        {

        }

        public Tb_SenhaDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Senha> GetPv(String Usuario_Aniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Usuario_Aniel, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Senha A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Usuario_Aniel))
            {
                sql.AppendLine(" AND A.Usuario_Aniel = @Usuario_Aniel");
                parametros.Add("@Usuario_Aniel", Usuario_Aniel);
            }

            List<Tb_Pv_Senha> retorno = new List<Tb_Pv_Senha>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Senha
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Usuario_Aniel = Convert.ToString(reader["Usuario_Aniel"]),
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

        public Boolean AtualizacaoPv(String Usuario_Aniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Senha A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Usuario_Aniel = @Usuario_Aniel");

            parametros.Add("@Usuario_Aniel", Usuario_Aniel);

            List<Tb_Pv_Senha> retorno = new List<Tb_Pv_Senha>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Usuario_Aniel, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Senha A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Usuario_Aniel = @Usuario_Aniel");

            parametros.Add("@LOG", Log);
            parametros.Add("@Usuario_Aniel", Usuario_Aniel);

            List<Tb_Pv_Senha> retorno = new List<Tb_Pv_Senha>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Senha> Get(List<String> Usuarios)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _UsuariosStr = from o in Usuarios
                             select String.Format("'{0}'", o);

            sql.AppendLine("SELECT A.USUARIO, A.NOME, A.SENHA, A.ATIVO, A.NUM_FRE ");
            sql.AppendLine("FROM Tb_Senha A ");
            sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ON (A.NUM_FRE = B.NUM_FRE) ");
            sql.AppendLine(String.Format("WHERE A.Usuario IN ({0})", String.Join(',', _UsuariosStr)));

            List<Tb_Senha> retorno = new List<Tb_Senha>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Senha
                {
                    Usuario = Convert.ToString(reader["Usuario"]),
                    Nome = Convert.ToString(reader["Nome"]),
                    Senha = Convert.ToString(reader["Senha"]),
                    Num_Fre = Convert.ToString(reader["Num_Fre"]),
                    Ativo = Convert.ToString(reader["Ativo"])
                });
            }

            return retorno;
        }
    }
}
