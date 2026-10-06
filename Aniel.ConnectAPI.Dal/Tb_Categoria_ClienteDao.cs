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
    public class Tb_Categoria_ClienteDao : DaoBase
    {
        private Tb_Categoria_ClienteDao()
        {

        }

        public Tb_Categoria_ClienteDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Cat_Cliente> GetPv(Int32? Id_Categoria_Cliente)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.ID_CAT_CLIENTE, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_CAT_CLIENTE A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Id_Categoria_Cliente.HasValue)
            {
                sql.AppendLine(" AND A.ID_CAT_CLIENTE = @Id_Categoria_Cliente");
                parametros.Add("@Id_Categoria_Cliente", Id_Categoria_Cliente.Value);
            }

            List<Tb_Pv_Cat_Cliente> retorno = new List<Tb_Pv_Cat_Cliente>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Cat_Cliente
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Id_Cat_Cliente = reader["Id_Cat_Cliente"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["Id_Cat_Cliente"].ToString()),
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

        public Boolean AtualizacaoPv(Int32 Id_Categoria_Cliente)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Cat_Cliente A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_CAT_CLIENTE = @Id_Categoria_Cliente");

            parametros.Add("@Id_Categoria_Cliente", Id_Categoria_Cliente);

            List<Tb_Pv_Cat_Cliente> retorno = new List<Tb_Pv_Cat_Cliente>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int32 Id_Categoria_Cliente, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE Tb_Pv_Cat_Cliente A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.ID_CAT_CLIENTE = @Id_Cat_Cliente");

            parametros.Add("@LOG", Log);
            parametros.Add("@Id_Categoria_Cliente", Id_Categoria_Cliente);

            List<Tb_Pv_Cat_Cliente> retorno = new List<Tb_Pv_Cat_Cliente>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Categoria_Cliente> Get(List<Int32> Perfil_Equipes)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _Perfil_EquipesStr = from o in Perfil_Equipes
                             select String.Format("{0}", o);

            sql.AppendLine("SELECT Id, DESCRICAO");
            sql.AppendLine("FROM Tb_Categoria_Cliente A");
            sql.AppendLine(String.Format("WHERE A.Id IN ({0})", String.Join(',', _Perfil_EquipesStr)));

            List<Tb_Categoria_Cliente> retorno = new List<Tb_Categoria_Cliente>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Categoria_Cliente
                {
                    Id = reader["Id"].Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader["Id"].ToString()),
                    Descricao = Convert.ToString(reader["Descricao"])
                });
            }

            return retorno;
        }
    }
}
