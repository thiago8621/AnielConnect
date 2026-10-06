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
    public class Tb_Perfil_EquipeDao : DaoBase
    {
        private Tb_Perfil_EquipeDao()
        {

        }

        public Tb_Perfil_EquipeDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Perfil_Equipe> GetPv(Int32? Cod_Perfil_Equipe)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Perfil_Equipe, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_Perfil_Equipe A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_Perfil_Equipe.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Perfil_Equipe = @Cod_Perfil_Equipe");
                parametros.Add("@Cod_Perfil_Equipe", Cod_Perfil_Equipe.Value);
            }

            List<Tb_Pv_Perfil_Equipe> retorno = new List<Tb_Pv_Perfil_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Perfil_Equipe
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Perfil_Equipe = reader["Cod_Perfil_Equipe"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["Cod_Perfil_Equipe"].ToString()),
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

        public Boolean AtualizacaoPv(Int32 Cod_Perfil_Equipe)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Perfil_Equipe A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Perfil_Equipe = @Cod_Perfil_Equipe");

            parametros.Add("@Cod_Perfil_Equipe", Cod_Perfil_Equipe);

            List<Tb_Pv_Perfil_Equipe> retorno = new List<Tb_Pv_Perfil_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Cod_Perfil_Equipe, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_Perfil_Equipe A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Perfil_Equipe = @Cod_Perfil_Equipe");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Perfil_Equipe", Cod_Perfil_Equipe);

            List<Tb_Pv_Perfil_Equipe> retorno = new List<Tb_Pv_Perfil_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Perfil_Equipe> Get(List<Int32> Perfil_Equipes)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Perfil_EquipesStr = from o in Perfil_Equipes
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT Cod_Perfil_Equipe, Nome");
            sql.AppendLine("FROM TB_Perfil_Equipe A");
            sql.AppendLine(String.Format("WHERE A.Cod_Perfil_Equipe IN ({0})", String.Join(',', _Perfil_EquipesStr)));

            List<Tb_Perfil_Equipe> retorno = new List<Tb_Perfil_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Perfil_Equipe
                {
                    Cod_Perfil_Equipe = reader["Cod_Perfil_Equipe"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["Cod_Perfil_Equipe"].ToString()),
                    Nome = Convert.ToString(reader["Nome"])
                });
            }

            return retorno;
        }
    }
}
