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
    public class Tb_ProjetoDao : DaoBase
    {
        private Tb_ProjetoDao()
        {

        }

        public Tb_ProjetoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Projeto> GetPv(String Codct, String Projeto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.CODCT, A.PROJETO, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_PROJETO A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Codct))
            {
                sql.AppendLine(" AND A.CODCT = @CODCT");
                parametros.Add("@CODCT", Codct);
            }

            if (!String.IsNullOrEmpty(Projeto))
            {
                sql.AppendLine(" AND A.PROJETO = @PROJETO");
                parametros.Add("@PROJETO", Projeto);
            }

            List<Tb_Pv_Projeto> retorno = new List<Tb_Pv_Projeto>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Projeto
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Codct = Convert.ToString(reader["CODCT"]),
                    Projeto = Convert.ToString(reader["PROJETO"]),
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

        public Boolean AtualizacaoPv(String Codct, String Projeto)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_PROJETO A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO");

            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);

            List<Tb_Pv_Projeto> retorno = new List<Tb_Pv_Projeto>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Codct, String Projeto, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_PROJETO A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO");

            parametros.Add("@LOG", Log);
            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);

            List<Tb_Pv_Projeto> retorno = new List<Tb_Pv_Projeto>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Projeto> Get(List<ChaveProjeto> Projetos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _projetosStr = from o in Projetos
                                select String.Format("(CODCT = '{0}' AND PROJETO = '{1}')", o.Codct, o.Projeto);

            sql.AppendLine("SELECT CODCT, PROJETO, CODEMPRESA,CODFILIAL,LOCALIDADE, CODCT_REGULADOR, PROJETO_REGULADOR, INICPREV, INICREAL,");
            sql.AppendLine("TERMPREV, TERMREAL, TERMREAL2");
            sql.AppendLine("FROM TB_PROJETO A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _projetosStr)));

            List<Tb_Projeto> retorno = new List<Tb_Projeto>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Projeto
                {
                    CodCt = Convert.ToString(reader["CodCt"]),
                    Projeto = Convert.ToString(reader["Projeto"]),
                    CodEmpresa = reader["CodEmpresa"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["CodEmpresa"].ToString()),
                    CodFilial = reader["CodFilial"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["CodFilial"].ToString()),
                    Localidade = Convert.ToString(reader["Localidade"]),
                    CodCt_Regulador = Convert.ToString(reader["CodCt_Regulador"]),
                    Projeto_Regulador = Convert.ToString(reader["Projeto_Regulador"]),
                    InicPrev = reader["InicPrev"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["InicPrev"].ToString()),
                    InicReal = reader["InicReal"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["InicReal"].ToString()),
                    TermPrev = reader["TermPrev"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["TermPrev"].ToString()),
                    TermReal = reader["TermReal"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["TermReal"].ToString()),
                    TermReal2 = reader["TermReal2"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["TermReal2"].ToString())
                });
            }

            return retorno;
        }
    }
}
