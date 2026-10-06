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
    public class Tb_Turno_ServicoDao : DaoBase
    {
        private Tb_Turno_ServicoDao()
        {

        }

        public Tb_Turno_ServicoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Turno_Servico> GetPv(Int32? Id_Turno_Servico)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Id_Turno_Servico, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Turno_Servico A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Id_Turno_Servico.HasValue)
            {
                sql.AppendLine(" AND A.Id_Turno_Servico = @Id_Turno_Servico");
                parametros.Add("@Id_Turno_Servico", Id_Turno_Servico.Value);
            }

            List<Tb_Pv_Turno_Servico> retorno = new List<Tb_Pv_Turno_Servico>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Turno_Servico
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_Turno_Servico = reader["Id_Turno_Servico"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["Id_Turno_Servico"].ToString()),
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

        public Boolean AtualizacaoPv(Int32 Id_Turno_Servico)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Turno_Servico A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Id_Turno_Servico = @Id_Turno_Servico");

            parametros.Add("@Id_Turno_Servico", Id_Turno_Servico);

            List<Tb_Pv_Turno_Servico> retorno = new List<Tb_Pv_Turno_Servico>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Id_Turno_Servico, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Turno_Servico A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Id_Turno_Servico = @Id_Turno_Servico");

            parametros.Add("@LOG", Log);
            parametros.Add("@Id_Turno_Servico", Id_Turno_Servico);

            List<Tb_Pv_Turno_Servico> retorno = new List<Tb_Pv_Turno_Servico>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Turno_Servico> Get(List<Int32> Turno_Servicos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Turno_ServicosStr = from o in Turno_Servicos
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT Id, DESCRICAO");
            sql.AppendLine("FROM Tb_Turno_Servico A");
            sql.AppendLine(String.Format("WHERE A.Id IN ({0})", String.Join(',', _Turno_ServicosStr)));

            List<Tb_Turno_Servico> retorno = new List<Tb_Turno_Servico>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Turno_Servico
                {
                    Id = reader["ID"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["ID"].ToString()),
                    Descricao = Convert.ToString(reader["DESCRICAO"])
                });
            }

            return retorno;
        }
    }
}
