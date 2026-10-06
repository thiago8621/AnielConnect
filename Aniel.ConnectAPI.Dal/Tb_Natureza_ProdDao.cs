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
    public class Tb_Natureza_ProdDao : DaoBase
    {
        private Tb_Natureza_ProdDao()
        {

        }

        public Tb_Natureza_ProdDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Natureza_Prod> GetPv(Int32? Id)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.ID_NATUREZA, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Natureza_Prod A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Id.HasValue)
            {
                sql.AppendLine(" AND A.ID_NATUREZA = @ID_NATUREZA");
                parametros.Add("@ID_NATUREZA", Id);
            }

            List<Tb_Pv_Natureza_Prod> retorno = new List<Tb_Pv_Natureza_Prod>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Natureza_Prod
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_Natureza = Convert.ToInt32(reader["ID_NATUREZA"]),
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

            sql.AppendLine("UPDATE Tb_Pv_Natureza_Prod A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_NATUREZA = @ID_NATUREZA");

            parametros.Add("@ID_NATUREZA", Id);

            List<Tb_Pv_Natureza_Prod> retorno = new List<Tb_Pv_Natureza_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Id, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Natureza_Prod A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_NATUREZA = @ID_NATUREZA");

            parametros.Add("@LOG", Log);
            parametros.Add("@ID_NATUREZA", Id);

            List<Tb_Pv_Natureza_Prod> retorno = new List<Tb_Pv_Natureza_Prod>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Natureza_Producao> Get(List<Int32> Naturezas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _EmpresasStr = from o in Naturezas
                               select String.Format("{0}", o);

            sql.AppendLine("SELECT Id, Descricao");
            sql.AppendLine("FROM Tb_Natureza_Producao A");
            sql.AppendLine(String.Format("WHERE A.Id IN ({0})", String.Join(',', _EmpresasStr)));

            List<Tb_Natureza_Producao> retorno = new List<Tb_Natureza_Producao>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Natureza_Producao
                {
                    Id = reader["Id"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Id"].ToString()),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
