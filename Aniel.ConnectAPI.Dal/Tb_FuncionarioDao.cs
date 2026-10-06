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
    public class Tb_FuncionarioDao : DaoBase
    {
        private Tb_FuncionarioDao()
        {

        }

        public Tb_FuncionarioDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Funcionario> GetPv(String Num_Fre)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Num_Fre, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_Funcionario A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Num_Fre))
            {
                sql.AppendLine(" AND A.Num_Fre = @Num_Fre");
                parametros.Add("@Num_Fre", Num_Fre);
            }

            List<Tb_Pv_Funcionario> retorno = new List<Tb_Pv_Funcionario>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Funcionario
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Num_Fre = Convert.ToString(reader["Num_Fre"]),
                    Usuario = Convert.ToString(reader["USUARIO"]),
                    Dia = reader["DIA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA"].ToString()),
                    Hora = reader["HORA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA"].ToString()),
                    Executado = Convert.ToString(reader["EXECUTADO"]),
                    Dia_Execucao = reader["DIA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA_EXECUCAO"].ToString()),
                    Hora_Execucao = reader["HORA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA_EXECUCAO"].ToString()),
                    Log = Convert.ToString(reader["LOG"]),
                    
                });
            }

            return retorno;
        }

        public Boolean AtualizacaoPv(String Num_Fre)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_Funcionario A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Num_Fre = @Num_Fre");

            parametros.Add("@Num_Fre", Num_Fre);

            List<Tb_Pv_Funcionario> retorno = new List<Tb_Pv_Funcionario>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Num_Fre, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_Funcionario A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Num_Fre = @Num_Fre");

            parametros.Add("@LOG", Log);
            parametros.Add("@Num_Fre", Num_Fre);

            List<Tb_Pv_Funcionario> retorno = new List<Tb_Pv_Funcionario>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Funcionario> Get(List<String> Funcionarios)
        {
            
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _FuncionariosStr = from o in Funcionarios
                             select String.Format("'{0}'", o);




            //sql.AppendLine("SELECT Num_Fre, NOME, TEL_CELULAR, SITUACAO, CODEMPRESA,CODFILIAL, SUPERVISOR, COD_DEPTO");
            //sql.AppendLine("FROM TB_Funcionario A");
            StringBuilder sql = new StringBuilder(@" SELECT A.NUM_FRE, A.NOME, A.TEL_CELULAR, A.SITUACAO, A.CODEMPRESA, A.CODFILIAL, A.SUPERVISOR, A.COD_DEPTO, B.DESCRICAO AS DEPTO,
                                                            A.BAIRRO, A.CEP, A.COMPLEMENTO, A.CPF, A.ENDERECO, A.MUNICIPIO, A.NUMERO, A.RG, A.UF, A.COD_CCUSTO
                                                     FROM TB_FUNCIONARIO A
                                                     LEFT JOIN TB_FOLHA_DEPTO B
                                                     ON (A.COD_DEPTO = B.COD_DEPTO) ");

            if (Funcionarios.Count < 1500)
                sql.AppendLine(String.Format("WHERE A.Num_Fre IN ({0})", String.Join(',', _FuncionariosStr)));

            List <Tb_Funcionario> retorno = new List<Tb_Funcionario>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Funcionario
                {
                    Num_Fre = Convert.ToString(reader["NUM_FRE"]),
                    Nome = Convert.ToString(reader["NOME"]),
                    Tel_Celular = Convert.ToString(reader["TEL_CELULAR"]),
                    Codempresa = reader["CODEMPRESA"] == DBNull.Value ? (Int16)0 : Convert.ToInt16(reader["CODEMPRESA"].ToString()),
                    Codfilial = reader["CODFILIAL"] == DBNull.Value ? (Int16)0 : Convert.ToInt16(reader["CODFILIAL"].ToString()),
                    Cod_CCusto = reader["COD_CCUSTO"] == DBNull.Value ? (Int32)0 : Convert.ToInt32(reader["COD_CCUSTO"].ToString()),
                    Situacao = reader["SITUACAO"] == DBNull.Value ? (Int16)0 : Convert.ToInt16(reader["SITUACAO"].ToString()),
                    Supervisor = reader["SUPERVISOR"] == DBNull.Value ? "N" : Convert.ToString(reader["SUPERVISOR"]),
                    Cod_Depto = reader["COD_DEPTO"] == DBNull.Value ? (Int16?)null : Convert.ToInt16(reader["COD_DEPTO"]),
                    Depto = reader["DEPTO"] == DBNull.Value ? String.Empty : Convert.ToString(reader["DEPTO"]),
                    Bairro = reader["BAIRRO"] == DBNull.Value ? null : Convert.ToString(reader["BAIRRO"]),
                    Cep = reader["CEP"] == DBNull.Value ? null : Convert.ToString(reader["CEP"]),
                    Complemento = reader["COMPLEMENTO"] == DBNull.Value ? null : Convert.ToString(reader["COMPLEMENTO"]),
                    Cpf = reader["CPF"] == DBNull.Value ? null : Convert.ToString(reader["CPF"]),
                    Endereco = reader["ENDERECO"] == DBNull.Value ? null : Convert.ToString(reader["ENDERECO"]),
                    Municipio = reader["MUNICIPIO"] == DBNull.Value ? null : Convert.ToString(reader["MUNICIPIO"]),
                    Numero = reader["NUMERO"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["NUMERO"]),
                    Rg = reader["RG"] == DBNull.Value ? null : Convert.ToString(reader["RG"]),
                    Uf = reader["UF"] == DBNull.Value ? null : Convert.ToString(reader["UF"]),
                });
            }

            return retorno;
        }
    }
}
