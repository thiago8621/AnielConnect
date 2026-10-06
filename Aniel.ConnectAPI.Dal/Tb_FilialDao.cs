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
    public class Tb_FilialDao : DaoBase
    {
        private Tb_FilialDao()
        {

        }

        public Tb_FilialDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Filial> GetPv(Int16? Codempresa, Int16? Codfilial)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.CODEMPRESA, A.CODFILIAL, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_FILIAL A");
            sql.AppendLine("WHERE A.EXECUTADO = 'N'");

            if (Codempresa.HasValue)
            {
                sql.AppendLine(" AND A.CODEMPRESA = @CODEMPRESA");
                parametros.Add("@CODEMPRESA", Codempresa.Value);
            }

            if (Codfilial.HasValue)
            {
                sql.AppendLine(" AND A.CODFILIAL = @CODFILIAL");
                parametros.Add("@CODFILIAL", Codfilial.Value);
            }

            List<Tb_Pv_Filial> retorno = new List<Tb_Pv_Filial>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Pv_Filial
                {
                    Id = Convert.ToInt64(reader["ID"]),
                    Acao = Convert.ToString(reader["ACAO"]),
                    Codempresa = reader["CODEMPRESA"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CODEMPRESA"]),
                    Codfilial = reader["CODFILIAL"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CODFILIAL"]),
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

        public Boolean AtualizacaoPv(Int16 Codempresa, Int16 Codfilial)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_FILIAL A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODEMPRESA = @CODEMPRESA AND A.CODFILIAL = @CODFILIAL");

            parametros.Add("@CODEMPRESA", Codempresa);
            parametros.Add("@CODFILIAL", Codfilial);

            List<Tb_Pv_Filial> retorno = new List<Tb_Pv_Filial>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(Int16 Codempresa, Int16 Codfilial, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_FILIAL A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODEMPRESA = @CODEMPRESA AND A.CODFILIAL = @CODFILIAL");

            parametros.Add("@LOG", Log);
            parametros.Add("@CODEMPRESA", Codempresa);
            parametros.Add("@CODFILIAL", Codfilial);

            List<Tb_Pv_Filial> retorno = new List<Tb_Pv_Filial>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoIdConnect(Int16 Codempresa, Int16 Codfilial, Int64 Id_Filial_Connect)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_FILIAL A");
            sql.AppendLine("SET ID_FILIAL_CONNECT = @ID_FILIAL_CONNECT");
            sql.AppendLine("WHERE A.CODEMPRESA = @CODEMPRESA AND A.CODFILIAL = @CODFILIAL");

            parametros.Add("@ID_FILIAL_CONNECT", Id_Filial_Connect);
            parametros.Add("@CODEMPRESA", Codempresa);
            parametros.Add("@CODFILIAL", Codfilial);

            List<Tb_Pv_Filial> retorno = new List<Tb_Pv_Filial>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Filial> Get(List<ChaveFilial> Filiais)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _projetosStr = from o in Filiais
                               select String.Format("(CODEMPRESA = {0} AND CODFILIAL = {1})", o.Codempresa, o.Codfilial);

            sql.AppendLine("SELECT CODEMPRESA, CODFILIAL, NOME, CNPJ, CODIGOCEDENTE, DIGITOVERIFICADORCEDENTE, CARTEIRA, ");
            sql.AppendLine("ENDERECO, CIDADE, BAIRRO, NUMERO, UF, CEP, TELEFONE, ID_FILIAL_CONNECT, NATUREZA_TRIBUTACAO, RAZAO_SOCIAL, OPTANTE_SIMPLES, REGIME_TRIBUTARIO,");
            sql.AppendLine("INCENTIVO_CULTURAL, INCENTIVO_FISCAL, REGIME_TRIBUTARIO_ESPECIAL, DDD, EMAIL_NFE, TIPO_TRIBUTACAO_ISS,");
            sql.AppendLine("EXIGIBILIDADE_ISS, NUM_PROCESSO_SUSPENSAO, CENAE_FISCAL,INSCRICAO_MUNICIPAL,INSCRICAO ");
            sql.AppendLine("FROM TB_FILIAL A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _projetosStr)));

            List<Tb_Filial> retorno = new List<Tb_Filial>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Filial
                {
                    Codempresa = reader["CODEMPRESA"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CODEMPRESA"].ToString()),
                    Codfilial = reader["CODFILIAL"].Equals(DBNull.Value) ? (Int16)0 : Convert.ToInt16(reader["CODFILIAL"].ToString()),
                    CodigoCedente = reader["CODIGOCEDENTE"].Equals(DBNull.Value) ? "" : Convert.ToString(reader["CODIGOCEDENTE"].ToString()),
                    DigitoVerificadorCedente = reader["DIGITOVERIFICADORCEDENTE"].Equals(DBNull.Value) ? "" : Convert.ToString(reader["DIGITOVERIFICADORCEDENTE"].ToString()),
                    Carteira = reader["CARTEIRA"].Equals(DBNull.Value) ? "" : Convert.ToString(reader["CARTEIRA"].ToString()),
                    Nome = Convert.ToString(reader["NOME"]),
                    Cnpj = Convert.ToString(reader["CNPJ"]),
                    Endereco = reader["ENDERECO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO"]) : null,
                    Cidade = reader["CIDADE"] != DBNull.Value ? Convert.ToString(reader["CIDADE"]) : null,
                    Bairro = reader["BAIRRO"] != DBNull.Value ? Convert.ToString(reader["BAIRRO"]) : null,
                    Cep = reader["CEP"] != DBNull.Value ? Convert.ToString(reader["CEP"]) : null,
                    Numero = reader["NUMERO"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO"]) : (int?)null,
                    Uf = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : null,
                    Telefone = reader["TELEFONE"] != DBNull.Value ? Convert.ToString(reader["TELEFONE"]) : null,
                    Id_Filial_Connect = reader["ID_FILIAL_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_FILIAL_CONNECT"]) : (Int64?)null,
                    Natureza_tributacao = reader["NATUREZA_TRIBUTACAO"] != DBNull.Value ? Convert.ToInt32(reader["ID_FILIAL_CONNECT"]) : (Int32?)null,
                    Razao_Social = Convert.ToString(reader["RAZAO_SOCIAL"]),
                    Optante_Simples_Nacional =  Convert.ToString(reader["OPTANTE_SIMPLES"]) == "S" ? true : false,
                    Regime_Tributario = reader["REGIME_TRIBUTARIO"] != DBNull.Value ? Convert.ToInt32(reader["REGIME_TRIBUTARIO"]) : (Int32?)null,
                    Incentivo_Cultural = Convert.ToString(reader["OPTANTE_SIMPLES"]) == "S" ? true : false,
                    Incentivo_Fiscal = Convert.ToString(reader["OPTANTE_SIMPLES"]) == "S" ? true : false,
                    Regime_Tributario_Especial = reader["REGIME_TRIBUTARIO_ESPECIAL"] != DBNull.Value ? Convert.ToInt32(reader["REGIME_TRIBUTARIO_ESPECIAL"]) : (Int32?)null,
                    DDD = Convert.ToString(reader["DDD"]),
                    Email = Convert.ToString(reader["EMAIL_NFE"]),
                    Tipo_Tributacao_ISS = reader["TIPO_TRIBUTACAO_ISS"] != DBNull.Value ? Convert.ToInt32(reader["TIPO_TRIBUTACAO_ISS"]) : (Int32?)null,
                    Exigibilidade_ISS = reader["EXIGIBILIDADE_ISS"] != DBNull.Value ? Convert.ToInt32(reader["EXIGIBILIDADE_ISS"]) : (Int32?)null,
                    Num_Processo_Suspensao = Convert.ToString(reader["NUM_PROCESSO_SUSPENSAO"]),
                    CNAE_Fiscal = Convert.ToString(reader["CENAE_FISCAL"]),
                    Inscricao_Estadual = Convert.ToString(reader["INSCRICAO"]),
                    Inscricao_Municipal = Convert.ToString(reader["INSCRICAO_MUNICIPAL"]),

                });
            }

            return retorno;
        }
    }
}
