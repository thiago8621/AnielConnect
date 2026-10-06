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
    public class Tb_AcessoDao : DaoBase
    {
        private Tb_AcessoDao()
        {

        }

        public Tb_AcessoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Acesso> GetPv(String Usuario_Aniel, String Sigla_Modulo, String Cod_Area)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Usuario_Aniel, A.Cod_Area, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Acesso A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Usuario_Aniel))
            {
                sql.AppendLine(" AND A.Usuario_Aniel = @Usuario_Aniel");
                parametros.Add("@Usuario_Aniel", Usuario_Aniel);
            }

            if (!String.IsNullOrEmpty(Cod_Area))
            {
                sql.AppendLine(" AND A.Cod_Area = @Cod_Area");
                parametros.Add("@Cod_Area", Cod_Area);
            }

            if (!String.IsNullOrEmpty(Sigla_Modulo))
            {
                sql.AppendLine(" AND A.Sigla_Modulo = @Sigla_Modulo");
                parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            }

            List<Tb_Pv_Acesso> retorno = new List<Tb_Pv_Acesso>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Acesso
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Usuario_Aniel = Convert.ToString(reader["Usuario_Aniel"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
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

        public Boolean AtualizacaoPv(String Usuario_Aniel, String Sigla_Modulo, String Cod_Area)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Acesso A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Usuario_Aniel = @Usuario_Aniel AND A.Cod_Area = @Cod_Area AND A.Sigla_Modulo = @Sigla_Modulo");

            parametros.Add("@Usuario_Aniel", Usuario_Aniel);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Sigla_Modulo", Sigla_Modulo);

            List<Tb_Pv_Acesso> retorno = new List<Tb_Pv_Acesso>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Usuario_Aniel, String Sigla_Modulo, String Cod_Area, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE Tb_Pv_Acesso A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Usuario_Aniel = @Usuario_Aniel AND A.Cod_Area = @Cod_Area AND A.Sigla_Modulo = @Sigla_Modulo");

            parametros.Add("@LOG", Log);
            parametros.Add("@Usuario_Aniel", Usuario_Aniel);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Sigla_Modulo", Sigla_Modulo);           

            List<Tb_Pv_Acesso> retorno = new List<Tb_Pv_Acesso>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Acesso> Get(List<ChaveAcesso> Cod_Areas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Cod_AreasStr = from o in Cod_Areas
                                select String.Format("(Usuario_Aniel = '{0}' AND Cod_Area = '{1}' AND Sigla_Modulo = '{2}')", o.Usuario_Aniel, o.Cod_Area, o.Sigla_Modulo);

            sql.AppendLine("SELECT Usuario_Aniel, Cod_Area, Sigla_Modulo, Tipo_Acesso, Todos_Acessos");
            sql.AppendLine("FROM TB_Sigla_Modulo A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Cod_AreasStr)));

            List<Tb_Acesso> retorno = new List<Tb_Acesso>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Acesso
                {
                    Usuario = Convert.ToString(reader["Usuario_Aniel"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
                    Tipo_Acesso = Convert.ToString(reader["Tipo_Acesso"]),
                    Todos_Acessos = Convert.ToString(reader["Todos_Acessos"]),
                });
            }

            return retorno;
        }
    }
}
