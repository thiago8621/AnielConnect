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
    public class Tb_Perfil_De_Usuario_AcessoDao : DaoBase
    {
        private Tb_Perfil_De_Usuario_AcessoDao()
        {

        }

        public Tb_Perfil_De_Usuario_AcessoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Perfil_Usu_Acesso> GetPv(String Sigla_Modulo, String Cod_Area, Int32? Cod_Perfil_Usuario)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Sigla_Modulo, A.Cod_Area, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Perfil_Usu_Acesso A");
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

            if (Cod_Perfil_Usuario.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario");
                parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);
            }

            List<Tb_Pv_Perfil_Usu_Acesso> retorno = new List<Tb_Pv_Perfil_Usu_Acesso>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Perfil_Usu_Acesso
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Cod_Perfil_Usuario = Convert.ToInt32(reader["Cod_Perfil_Usuario"]),
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

        public Boolean AtualizacaoPv(String Sigla_Modulo, String Cod_Area, Int32 Cod_Perfil_Usuario)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Perfil_Usu_Acesso A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Sigla_Modulo = @Sigla_Modulo AND A.Cod_Area = @Cod_Area AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario");

            parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);

            List<Tb_Pv_Perfil_Usu_Acesso> retorno = new List<Tb_Pv_Perfil_Usu_Acesso>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Sigla_Modulo, String Cod_Area, Int32 Cod_Perfil_Usuario, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Perfil_Usu_Acesso A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Sigla_Modulo = @Sigla_Modulo AND A.Cod_Area = @Cod_Area AND A.Cod_Perfil_Usuario = @Cod_Perfil_Usuario");

            parametros.Add("@LOG", Log);
            parametros.Add("@Sigla_Modulo", Sigla_Modulo);
            parametros.Add("@Cod_Area", Cod_Area);
            parametros.Add("@Cod_Perfil_Usuario", Cod_Perfil_Usuario);           

            List<Tb_Pv_Perfil_Usu_Acesso> retorno = new List<Tb_Pv_Perfil_Usu_Acesso>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Perfil_Usuario_Acesso> Get(List<ChavePerfil_Usuario_Acesso> Cod_Areas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Cod_AreasStr = from o in Cod_Areas
                                select String.Format("(Sigla_Modulo = '{0}' AND Cod_Area = '{1}' AND Cod_Perfil_Usuario = '{2}')", o.Sigla_Modulo, o.Cod_Area, o.Cod_Perfil_Usuario);

            sql.AppendLine("SELECT Sigla_Modulo, Cod_Area, Cod_Perfil_Usuario, Tipo_Acesso, Todos_Acessos");
            sql.AppendLine("FROM TB_Cod_Perfil_Usuario A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Cod_AreasStr)));

            List<Tb_Perfil_Usuario_Acesso> retorno = new List<Tb_Perfil_Usuario_Acesso>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Perfil_Usuario_Acesso
                {
                    Sigla_Modulo = Convert.ToString(reader["Sigla_Modulo"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Cod_Perfil_Usuario = Convert.ToInt32(reader["Cod_Perfil_Usuario"]),
                    Tipo_Acesso = Convert.ToString(reader["Tipo_Acesso"]),
                    Todos_Acessos = Convert.ToString(reader["Todos_Acessos"])
                });
            }

            return retorno;
        }
    }
}
