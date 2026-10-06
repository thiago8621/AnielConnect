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
    public class Tb_Usuario_PerfilDao : DaoBase
    {
        private Tb_Usuario_PerfilDao()
        {

        }

        public Tb_Usuario_PerfilDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Usuario_Perfil> GetPv(Int16? Cod_Perfil_Usuario, String Usuario_Aniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Perfil_Usuario, A.Usuario_Aniel, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Usuario_Perfil A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_Perfil_Usuario.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario");
                parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);
            }

            if (!String.IsNullOrEmpty(Usuario_Aniel))
            {
                sql.AppendLine(" AND A.Usuario_Aniel = @Usuario_Aniel");
                parametros.Add("@Usuario_Aniel", Usuario_Aniel);
            }

            List<Tb_Pv_Usuario_Perfil> retorno = new List<Tb_Pv_Usuario_Perfil>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Usuario_Perfil
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Perfil_Usuario = reader["Cod_Perfil_Usuario"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Perfil_Usuario"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 Cod_Perfil_Usuario, String Usuario_Aniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Usuario_Perfil A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario AND A.Usuario_Aniel = @Usuario_Aniel");

            parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);
            parametros.Add("@Usuario_Aniel", Usuario_Aniel);

            List<Tb_Pv_Usuario_Perfil> retorno = new List<Tb_Pv_Usuario_Perfil>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Cod_Perfil_Usuario, String Usuario_Aniel, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Usuario_Perfil A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario AND A.Usuario_Aniel = @Usuario_Aniel");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);
            parametros.Add("@Usuario_Aniel", Usuario_Aniel);

            List<Tb_Pv_Usuario_Perfil> retorno = new List<Tb_Pv_Usuario_Perfil>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Usuario_Perfil> Get(List<ChaveUsuario_Perfil> Usuario_Aniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Usuario_AnielsStr = from o in Usuario_Aniel
                                select String.Format("(Cod_Perfil_Usuario = {0} AND Usuario_Aniel = '{1}')", o.Cod_Perfil_Usuario, o.Usuario);

            sql.AppendLine("SELECT Cod_Perfil_Usuario, Usuario_Aniel");
            sql.AppendLine("FROM Tb_Usuario_Perfil A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Usuario_AnielsStr)));

            List<Tb_Usuario_Perfil> retorno = new List<Tb_Usuario_Perfil>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Usuario_Perfil
                {
                    Cod_Perfil_Usuario = Convert.ToInt16(reader["Cod_Perfil_Usuario"]),
                    Usuario = Convert.ToString(reader["Usuario"])
                    
                });
            }

            return retorno;
        }
    }
}
