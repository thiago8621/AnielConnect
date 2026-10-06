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
    public class Tb_Documento_ProducaoDao : DaoBase
    {
        private Tb_Documento_ProducaoDao()
        {

        }

        public Tb_Documento_ProducaoDao(DbConnection pConnection, DbTransaction pTransaction)
            : base(pConnection, pTransaction)
        {

        }

        public List<Tb_Pv_Documento_Producao> GetPv(String Codct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("SELECT A.ID, A.ACAO, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.USUARIO, A.DIA, A.HORA, A.EXECUTADO, A.DIA_EXECUCAO, A.HORA_EXECUCAO, A.LOG");
            sql.AppendLine("FROM TB_PV_DOC_PRODUCAO A");
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

            if (!String.IsNullOrEmpty(Num_Obra))
            {
                sql.AppendLine(" AND A.NUM_OBRA = @NUM_OBRA");
                parametros.Add("@NUM_OBRA", Num_Obra);
            }

            if (Num_Doc.HasValue)
            {
                sql.AppendLine(" AND A.NUM_DOC = @NUM_DOC");
                parametros.Add("@NUM_DOC", Num_Doc);
            }

            List<Tb_Pv_Documento_Producao> retorno = new List<Tb_Pv_Documento_Producao>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                Tb_Pv_Documento_Producao obj = new Tb_Pv_Documento_Producao();
                obj.Id = Convert.ToInt64(reader["ID"]);
                obj.Acao = Convert.ToString(reader["ACAO"]);
                obj.Codct = Convert.ToString(reader["CODCT"]);
                obj.Projeto = Convert.ToString(reader["PROJETO"]);
                obj.Num_Obra = Convert.ToString(reader["NUM_OBRA"]);
                obj.Num_Doc = Convert.ToInt32(reader["NUM_DOC"]);
                obj.Usuario = Convert.ToString(reader["USUARIO"]);
                obj.Dia = reader["DIA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA"].ToString());
                obj.Hora = reader["HORA"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA"].ToString());
                obj.Executado = Convert.ToString(reader["EXECUTADO"]);
                obj.Dia_Execucao = reader["DIA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["DIA_EXECUCAO"].ToString());
                obj.Hora_Execucao = reader["HORA_EXECUCAO"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["HORA_EXECUCAO"].ToString());
                obj.Log = Convert.ToString(reader["LOG"]);

                retorno.Add(obj);
            }

            return retorno;
        }

        public Boolean AtualizacaoPv(String Codct, String Projeto, String Num_Obra, Int32 Num_Doc)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_DOC_PRODUCAO A");
            sql.AppendLine("SET EXECUTADO = 'S', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC");

            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);
            parametros.Add("@NUM_OBRA", Num_Obra);
            parametros.Add("@NUM_DOC", Num_Doc);

            List<Tb_Pv_Documento_Producao> retorno = new List<Tb_Pv_Documento_Producao>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public Boolean AtualizacaoPvErro(String Codct, String Projeto, String Num_Obra, Int32 Num_Doc, String Log)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, Object> parametros = new Dictionary<string, Object>();

            sql.AppendLine("UPDATE TB_PV_PROJETO A");
            sql.AppendLine("SET EXECUTADO = 'N', DIA_EXECUCAO = CURRENT_DATE, HORA_EXECUCAO = CURRENT_TIME, LOG = @LOG");
            sql.AppendLine("WHERE A.EXECUTADO = 'N' AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC");

            parametros.Add("@LOG", Log);
            parametros.Add("@CODCT", Codct);
            parametros.Add("@PROJETO", Projeto);
            parametros.Add("@NUM_OBRA", Num_Obra);
            parametros.Add("@NUM_DOC", Num_Doc);

            List<Tb_Pv_Projeto> retorno = new List<Tb_Pv_Projeto>();

            CommandHelper.ExecuteNonQuery(sql, parametros, GetConnection(), GetTransaction());

            return true;
        }

        public List<Tb_Documento_Producao> Get(List<ChaveDocumento_Producao> Projetos)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            var _projetosStr = from o in Projetos
                                select String.Format("(CODCT = '{0}' AND PROJETO = '{1}' AND NUM_OBRA = '{2}' AND NUM_DOC = {3})", o.Codct, o.Projeto, o.Num_Obra, o.Num_Doc);

            sql.AppendLine("SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.EQUIPE, ");
            sql.AppendLine("A.NUM_RVT, A.TAREFA, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE,");
            sql.AppendLine("A.COD_ENCERRAMENTO, A.COD_TIPO_SERV, A.COD_SUB_TIPO_SERV, A.DATA_MAXIMA, A.HORA_MAXIMA,");
            sql.AppendLine("A.DATA_PROD, A.HORA_PROD, A.OBSERVACAO, A.ENDERECO, A.ENDERECO_NUN, A.ENDERECO_COMPL,");
            sql.AppendLine("A.ENDERECO_BAIRRO, A.ENDERECO_CPL, A.LATITUDE, A.LONGITUDE");
            sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A");
            sql.AppendLine(String.Format("WHERE {0}", String.Join(" OR ", _projetosStr)));

            List<Tb_Documento_Producao> retorno = new List<Tb_Documento_Producao>();

            FbDataReader reader = CommandHelper.ExecuteReader(sql, parametros, GetConnection(), GetTransaction());

            while (reader.Read())
            {
                retorno.Add(new Tb_Documento_Producao
                {
                    CodCt = Convert.ToString(reader["CodCt"]),
                    Projeto = Convert.ToString(reader["Projeto"]),
                    Num_Obra = Convert.ToString(reader["Num_Obra"]),
                    Num_Doc = reader["Num_Doc"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt16(reader["Num_Doc"].ToString()),
                    Equipe = Convert.ToString(reader["Equipe"]),
                    Num_RVT = Convert.ToString(reader["Num_RVT"]),
                    Tarefa = Convert.ToString(reader["Tarefa"]),
                    Cod_Escritorio = Convert.ToString(reader["Cod_Escritorio"]),
                    Cod_Area = Convert.ToString(reader["Cod_Area"]),
                    Cod_Localidade = reader["Cod_Localidade"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["Cod_Localidade"].ToString()),
                    Cod_Encerramento = Convert.ToString(reader["Cod_Encerramento"]),
                    Cod_Tipo_Serv = reader["Cod_Tipo_Serv"].Equals(DBNull.Value) ? (Int16?)null : Convert.ToInt16(reader["Num_Doc"].ToString()),
                    Cod_Sub_Tipo_Serv = Convert.ToString(reader["Cod_Sub_Tipo_Serv"]),
                    Data_Maxima = reader["Data_Maxima"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["Data_Maxima"].ToString()),
                    Hora_Maxima = reader["Hora_Maxima"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["Hora_Maxima"].ToString()),
                    Data_Prod = reader["Data_Prod"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["Data_Prod"].ToString()),
                    Hora_Prod = reader["Hora_Prod"].Equals(DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(reader["Hora_Prod"].ToString()),
                    Observacao = Convert.ToString(reader["Observacao"]),
                    Endereco = Convert.ToString(reader["Endereco"]),
                    Endereco_Num = Convert.ToString(reader["ENDERECO_NUN"]),
                    Endereco_Compl = Convert.ToString(reader["Endereco_Compl"]),
                    Endereco_Bairro = Convert.ToString(reader["Endereco_Bairro"]),
                    Endereco_Cpl = Convert.ToString(reader["Endereco_Cpl"]),
                    Latitude = reader["Latitude"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt16(reader["Latitude"].ToString()),
                    Longitude = reader["Longitude"].Equals(DBNull.Value) ? (Int32)0 : Convert.ToInt16(reader["Longitude"].ToString())

                });
            }

            return retorno;
        }
    }
}
