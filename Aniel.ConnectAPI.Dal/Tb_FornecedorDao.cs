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
    public class Tb_FornecedorDao : DaoBase
    {
        private Tb_FornecedorDao()
        {

        }

        public Tb_FornecedorDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Fornecedor> GetPv(String Cod_Fornecedor)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.Cod_Fornecedor, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_Fornecedor A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (!String.IsNullOrEmpty(Cod_Fornecedor))
            {
                sql.AppendLine(" AND A.Cod_Fornecedor = @Cod_Fornecedor");
                parametros.Add("@Cod_Fornecedor", Cod_Fornecedor);
            }

            List<Tb_Pv_Fornecedor> retorno = new List<Tb_Pv_Fornecedor>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Fornecedor
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Cod_Fornecedor = Convert.ToString(reader["Cod_Fornecedor"]),
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

        public Boolean AtualizacaoPv(String Cod_Fornecedor)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_Fornecedor A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Fornecedor = @Cod_Fornecedor");

            parametros.Add("@Cod_Fornecedor", Cod_Fornecedor);

            List<Tb_Pv_Fornecedor> retorno = new List<Tb_Pv_Fornecedor>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Cod_Fornecedor, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            sql.AppendLine("UPDATE TB_PV_Fornecedor A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.Cod_Fornecedor = @Cod_Fornecedor");

            parametros.Add("@LOG", Log);
            parametros.Add("@Cod_Fornecedor", Cod_Fornecedor);

            List<Tb_Pv_Fornecedor> retorno = new List<Tb_Pv_Fornecedor>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Fornecedor> Get(List<String> Fornecedors)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _FornecedorsStr = from o in Fornecedors
                             select String.Format("'{0}'", o);

            sql.AppendLine("SELECT Cod_Fornecedor, RAZAO_SOCIAL, CNPJ, ENDERECO, COMPLEMENTO, ENDERECO_NUMERO, BAIRRO, MUNICIPIO, CEP, UF, PAIS, INSC_ESTADUAL, TELEFONE, EMAIL");
            sql.AppendLine("FROM TB_Fornecedor A");
            sql.AppendLine(String.Format("WHERE A.Cod_Fornecedor IN ({0})", String.Join(',', _FornecedorsStr)));

            List<Tb_Fornecedor> retorno = new List<Tb_Fornecedor>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Fornecedor
                {
                    Cod_Fornecedor = Convert.ToString(reader["Cod_Fornecedor"]),
                    RazaoSocial = Convert.ToString(reader["RAZAO_SOCIAL"]),
                    Cnpj = Convert.ToString(reader["CNPJ"]),

                    Endereco = Convert.ToString(reader["ENDERECO"]),
                    Complemento = Convert.ToString(reader["COMPLEMENTO"]),
                    Numero = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToInt32(reader["ENDERECO_NUMERO"]) : (Int32?)null,
                    Bairro = Convert.ToString(reader["BAIRRO"]),
                    Municipio = Convert.ToString(reader["MUNICIPIO"]),
                    Cep = Convert.ToString(reader["CEP"]),
                    Uf = Convert.ToString(reader["UF"]),
                    Pais = Convert.ToString(reader["PAIS"]),
                    Insc_Estadual = Convert.ToString(reader["INSC_ESTADUAL"]),
                    Telefone = Convert.ToString(reader["TELEFONE"]),
                    Email = Convert.ToString(reader["EMAIL"]),
                    //Descricao = Convert.ToString(reader["DESCRICAO"])

                });
            }

            return retorno;
        }
    }
}
