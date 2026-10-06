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
    public class Tb_Classif_QualidDao : DaoBase
    {
        private Tb_Classif_QualidDao()
        {

        }

        public Tb_Classif_QualidDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Classif_Qualid> GetPv(Int32? Id)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.ID_CLASSIF_QUALID, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Classif_Qualid A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Id.HasValue)
            {
                sql.AppendLine(" AND A.ID_CLASSIF_QUALID = @ID_CLASSIF_QUALID");
                parametros.Add("@ID_CLASSIF_QUALID", Id);
            }

            List<Tb_Pv_Classif_Qualid> retorno = new List<Tb_Pv_Classif_Qualid>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Classif_Qualid
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_Classif = reader["ID_CLASSIF_QUALID"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["ID_CLASSIF_QUALID"].ToString()),
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

        public Boolean AtualizacaoPv(Int32 Id)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Classif_Qualid A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_CLASSIF_QUALID = @ID_CLASSIF_QUALID");

            parametros.Add("@ID_CLASSIF_QUALID", Id);

            List<Tb_Pv_Classif_Qualid> retorno = new List<Tb_Pv_Classif_Qualid>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Id, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Classif_Qualid A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_CLASSIF_QUALID = @ID_CLASSIF_QUALID");

            parametros.Add("@LOG", Log);
            parametros.Add("@ID_CLASSIF_QUALID", Id);

            List<Tb_Pv_Classif_Qualid> retorno = new List<Tb_Pv_Classif_Qualid>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Classificacao_Qualidade> Get(List<Int32> Empresas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _EmpresasStr = from o in Empresas
                               select String.Format("{0}", o);

            sql.AppendLine("SELECT Id, QUALIDADE");
            sql.AppendLine("FROM Tb_Classificacao_Qualidade A");
            sql.AppendLine(String.Format("WHERE A.Id IN ({0})", String.Join(',', _EmpresasStr)));

            List<Tb_Classificacao_Qualidade> retorno = new List<Tb_Classificacao_Qualidade>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Classificacao_Qualidade
                {
                    Id = reader["Id"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Id"].ToString()),
                    Qualidade = Convert.ToString(reader["Qualidade"])
                });
            }

            return retorno;
        }
    }
}
