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
    public class Tb_Sub_Tipo_ServicoDao : DaoBase
    {
        private Tb_Sub_Tipo_ServicoDao()
        {

        }

        public Tb_Sub_Tipo_ServicoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Sub_Tipo_Servico> GetPv(Int32? Cod_Tipo_Serv, String Cod_Sub_Tipo_Serv)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Tipo_Serv, A.Cod_Sub_Tipo_Serv, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Sub_Tipo_Servico A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_Tipo_Serv.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv");
                parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);
            }

            if (!String.IsNullOrEmpty(Cod_Sub_Tipo_Serv))
            {
                sql.AppendLine(" AND A.Cod_Sub_Tipo_Serv = @Cod_Sub_Tipo_Serv");
                parametros.Add("@Cod_Sub_Tipo_Serv", Cod_Sub_Tipo_Serv);
            }

            List<Tb_Pv_Sub_Tipo_Servico> retorno = new List<Tb_Pv_Sub_Tipo_Servico>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Sub_Tipo_Servico
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Tipo_Serv = reader["Cod_Tipo_Serv"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt32(reader["Cod_Tipo_Serv"].ToString()),
                    Cod_Sub_Tipo_Serv = Convert.ToString(reader["Cod_Sub_Tipo_Serv"]),
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

        public Boolean AtualizacaoPv(Int32 Cod_Tipo_Serv, String Cod_Sub_Tipo_Serv)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Sub_Tipo_Servico A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv AND A.Cod_Sub_Tipo_Serv = @Cod_Sub_Tipo_Serv");

            parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);
            parametros.Add("@Cod_Sub_Tipo_Serv", Cod_Sub_Tipo_Serv);

            List<Tb_Pv_Sub_Tipo_Servico> retorno = new List<Tb_Pv_Sub_Tipo_Servico>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Cod_Tipo_Serv, String Cod_Sub_Tipo_Serv, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Sub_Tipo_Servico A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv AND A.Cod_Sub_Tipo_Serv = @Cod_Sub_Tipo_Serv");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);
            parametros.Add("@Cod_Sub_Tipo_Serv", Cod_Sub_Tipo_Serv);

            List<Tb_Pv_Sub_Tipo_Servico> retorno = new List<Tb_Pv_Sub_Tipo_Servico>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Sub_Tipo_Servico> Get(List<ChaveSub_Tipo_Servico> Cod_Sub_Tipo_Servs)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Cod_Sub_Tipo_ServsStr = from o in Cod_Sub_Tipo_Servs
                                select String.Format("(Cod_Tipo_Serv = {0} AND Cod_Sub_Tipo_Serv = '{1}')", o.Cod_Tipo_Serv, o.Cod_Sub_Tipo_Serv);

            sql.AppendLine("SELECT Cod_Tipo_Serv, Cod_Sub_Tipo_Serv, Descricao");
            sql.AppendLine("FROM Tb_Sub_Tipo_Servico A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _Cod_Sub_Tipo_ServsStr)));

            List<Tb_Sub_Tipo_Servico> retorno = new List<Tb_Sub_Tipo_Servico>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Sub_Tipo_Servico
                {
                    Cod_Tipo_Serv = Convert.ToInt32(reader["Cod_Tipo_Serv"]),
                    Cod_Sub_Tipo_Serv = Convert.ToString(reader["Cod_Sub_Tipo_Serv"]),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
