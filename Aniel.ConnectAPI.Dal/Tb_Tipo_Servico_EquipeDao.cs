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
    public class Tb_Tipo_Servico_EquipeDao : DaoBase
    {
        private Tb_Tipo_Servico_EquipeDao()
        {

        }

        public Tb_Tipo_Servico_EquipeDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Tp_Serv_Equipe> GetPv(Int16? Cod_Tipo_Serv)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Tipo_Serv, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM Tb_Pv_Tp_Serv_Equipe A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Cod_Tipo_Serv.HasValue)
            {
                sql.AppendLine(" AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv");
                parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);
            }

            List<Tb_Pv_Tp_Serv_Equipe> retorno = new List<Tb_Pv_Tp_Serv_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Tp_Serv_Equipe
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Tipo_Serv = reader["Cod_Tipo_Serv"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Tipo_Serv"].ToString()),
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

        public Boolean AtualizacaoPv(Int16 Cod_Tipo_Serv)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Tp_Serv_Equipe A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv");

            parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);

            List<Tb_Pv_Tp_Serv_Equipe> retorno = new List<Tb_Pv_Tp_Serv_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Cod_Tipo_Serv, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Tp_Serv_Equipe A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Tipo_Serv = @Cod_Tipo_Serv");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Tipo_Serv", Cod_Tipo_Serv);

            List<Tb_Pv_Tp_Serv_Equipe> retorno = new List<Tb_Pv_Tp_Serv_Equipe>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Tipo_Servico_Equipe> Get(List<Int16> Empresas)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _EmpresasStr = from o in Empresas
                               select String.Format("{0}", o);

            sql.AppendLine("SELECT Cod_Tipo_Serv, DESCRICAO, DESCRICAO_AUX, COD_NATUREZA");
            sql.AppendLine("FROM Tb_Tipo_Servico_Equipe A");
            sql.AppendLine(String.Format("WHERE A.Cod_Tipo_Serv IN ({0})", String.Join(',', _EmpresasStr)));

            List<Tb_Tipo_Servico_Equipe> retorno = new List<Tb_Tipo_Servico_Equipe>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Tipo_Servico_Equipe
                {
                    Cod_Tipo_Serv = reader["Cod_Tipo_Serv"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["Cod_Tipo_Serv"].ToString()),
                    Descricao = Convert.ToString(reader["Descricao"]),
                    Descricao_Aux = Convert.ToString(reader["Descricao_Aux"]),
                    Cod_Natureza = reader["Cod_Natureza"].Equals(DBNull.Value) ? (Int32?)null : Convert.ToInt32(reader["Cod_Natureza"].ToString()),
                });
            }

            return retorno;
        }
    }
}
