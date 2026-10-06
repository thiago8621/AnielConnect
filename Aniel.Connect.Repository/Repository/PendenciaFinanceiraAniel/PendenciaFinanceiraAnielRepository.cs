using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;

namespace Aniel.Connect.Repository.PendenciaFinanceiraAniel
{
    public class PendenciaFinanceiraAnielRepository : IPendenciaFinanceiraAnielRepository
    {
        private readonly ISession _sessionNh;
        private NHibernate.ISession sessionNh;

        public PendenciaFinanceiraAnielRepository(NHibernate.ISession sessionNh)
        {
            this.sessionNh = sessionNh;
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            var dados = new List<PendenciaFinanceiraListModel>();

            var sql = new StringBuilder();
            var param = new Dictionary<string, object>();

            string documento = cpfCnpj.Replace("-", String.Empty)
                                      .Replace("/", String.Empty)
                                      .Replace(".", String.Empty);

            bool pessoaFisica = documento.Length == 11;

            sql.AppendLine(" SELECT A.ID AS COD_CLIENTE, A.NOMEFANTASIA, D.ENDERECO_LOGRADOURO, ");
            sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_BAIRRO, D.ENDERECO_CIDADE, D.ENDERECO_UF,");
            sql.AppendLine(" H.ID AS COD_FATURA, H.VALOR_RECEBIDO VALOR_PAGO, H.STATUS STATUS, ");
            sql.AppendLine(" H.VALOR_BRUTO VALOR_TOTAL, H.DATA_VENCIMENTO DATA_VENCIMENTO, ");
            sql.AppendLine(" H.DESCRICAO AS DESCRICAO, COALESCE(B.VALOR_NEGOCIACAO, 0) VALOR_NEGOCIACAO, E.QTDDIASATRASO, G.MOTIVO, F.CODIGO, A.CPF ");

            sql.AppendLine(" FROM TB_CLIENTE A ");

            sql.AppendLine(" LEFT JOIN TB_FATURAMENTO_CLIENTE B ");
            sql.AppendLine(" ON(B.ID_CLIENTE= A.ID) ");

            sql.AppendLine(" LEFT JOIN TB_ENDERECO_CLIENTE D ");
            sql.AppendLine(" ON(D.ID = A.ID_ENDERECO_PRINCIPAL) ");

            sql.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO E ");
            sql.AppendLine(" ON(E.ID = A.ID) ");

            sql.AppendLine(" LEFT JOIN TB_PLANO F ");
            sql.AppendLine(" ON(F.ID = E.ID_PLANO) ");

            sql.AppendLine(" LEFT JOIN TB_MOTIVO_CANC_PLANOPRODUTO G ");
            sql.AppendLine(" ON(G.ID = E.ID_MOTIVO_CANCELAMENTO) ");

            sql.AppendLine(" INNER JOIN TB_CONTA_RECEBER H ");
            sql.AppendLine(" ON(H.ID_CLIENTE  = A.ID) ");

            var caracteres = new string[] { ".", "-", "/" };
            var cpf_cnpj = string.Empty;
            foreach (var item in caracteres)
            {
                cpf_cnpj = documento.Replace(item, "");
            }

            if (pessoaFisica)
            {
                if (documento.Length != 11)
                    throw new Exception("O CPF informado está incompleto");

                string cpfFormatado =
                    String.Format(
                        "{0}.{1}.{2}-{3}",
                        documento.Substring(0, 3),
                        documento.Substring(3, 3),
                        documento.Substring(6, 3),
                        documento.Substring(9, 2)
                    );

                sql.AppendLine("WHERE A.CPF = @CPF ");
                param.Add("@CPF", cpf_cnpj);
            }
            else
            {
                if (documento.Length != 14)
                    throw new Exception("O CNPJ informado está incompleto");

                string cnpjFormatado =
                    String.Format(
                        "{0}.{1}.{2}/{3}-{4}",
                        documento.Substring(0, 2),
                        documento.Substring(2, 3),
                        documento.Substring(5, 3),
                        documento.Substring(8, 4),
                        documento.Substring(12, 2)
                    );

                sql.AppendLine(" WHERE A.CPF = @CNPJ ");
                param.Add("@CNPJ", cpf_cnpj);
            }

            DateTime data_atual = DateTime.Now.Date;
            sql.AppendLine($" AND (H.STATUS = 2 OR H.DATA_VENCIMENTO < '{data_atual.ToString("yyyy-MM-dd")}' AND H.STATUS = 0)");
            sql.AppendLine(" GROUP BY COD_CLIENTE, A.NOMEFANTASIA, D.ENDERECO_LOGRADOURO,  D.ENDERECO_NUMERO, D.ENDERECO_BAIRRO, D.ENDERECO_CIDADE, D.ENDERECO_UF,");
            sql.AppendLine(" COD_FATURA, VALOR_PAGO, STATUS, VALOR_TOTAL, DATA_VENCIMENTO, DESCRICAO, VALOR_NEGOCIACAO, E.QTDDIASATRASO, G.MOTIVO, F.CODIGO, A.CPF");
            sql.AppendLine(" ORDER BY H.DATA_VENCIMENTO");

            using (var com = sessionNh.Connection.CreateCommand())
            using (var reader = CommandHelper.ExecuteReader(com, sql, param, this.sessionNh.Connection, null).Result)
            {
                while (reader.Read())
                {
                    var values = new PendenciaFinanceiraListModel();

                    //values.Cod_Contrato = reader["NUMERO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["NUMERO_CONTRATO"]) : String.Empty;
                    values.CodConexao = reader["CODIGO"] != DBNull.Value ? Convert.ToString(reader["CODIGO"]) : String.Empty;
                    values.Cod_Cliente = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["COD_CLIENTE"]) : String.Empty;
                    values.CpfCnpj = reader["CPF"] != DBNull.Value ? Convert.ToString(reader["CPF"]) : String.Empty;
                    //values.Contrato_Cancelado = reader["STATUS_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["STATUS_CONTRATO"]) : String.Empty;
                    values.Conexao_Bloqueada = reader["STATUS"] != DBNull.Value ? Convert.ToString(reader["STATUS"]) : String.Empty;
                    values.Logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]): String.Empty;
                    values.Numero = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_NUMERO"]): String.Empty;
                    values.Bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]): String.Empty;
                    values.Cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]): String.Empty;
                    values.CodFatura = reader["COD_FATURA"] != DBNull.Value ? Convert.ToString(reader["COD_FATURA"]): String.Empty;
                    values.Valor_Total = reader["VALOR_TOTAL"] != DBNull.Value ? Convert.ToString(reader["VALOR_TOTAL"]): String.Empty;
                    values.Liquidado = reader["VALOR_NEGOCIACAO"] != DBNull.Value ? Convert.ToString(reader["VALOR_NEGOCIACAO"]): String.Empty;
                    values.Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]): String.Empty;
                    values.Fatura_Suspensa = reader["STATUS"] != DBNull.Value ? Convert.ToString(reader["STATUS"]) : String.Empty;
                    values.Data_Vencimento = reader["DATA_VENCIMENTO"] != DBNull.Value ? Convert.ToString(reader["DATA_VENCIMENTO"]): String.Empty;
                    values.Motivo_Suspensao = reader["MOTIVO"] != DBNull.Value ? Convert.ToString(reader["MOTIVO"]) : String.Empty;

                    dados.Add(values);
                }
            }
            return dados;
        }
    }
}
