using Aniel.Connect.Repository.Model;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Connect.Repository.PendenciaEnderecoAniel
{
    public class PendenciaEnderecoAnielRepository : IPendenciaEnderecoAnielRepository
    {
        private readonly ISession _sessionNh;
        public PendenciaEnderecoAnielRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero)
        {
            var dados = new List<PendenciaEnderecoListModel>();

            var sql = new StringBuilder();
            var param = new Dictionary<string, object>();

            sql.AppendLine(" SELECT ");
            sql.AppendLine(" A.ID AS COD_CLIENTE, A.CPF, A.NOMEFANTASIA, D.ENDERECO_LOGRADOURO, ");
            sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_CIDADE, D.ENDERECO_UF, D.ENDERECO_BAIRRO, ");
            sql.AppendLine(" H.ID AS COD_FATURA, H.VALOR_RECEBIDO VALOR_PAGO, H.STATUS, H.VALOR_BRUTO VALOR_TOTAL, H.DATA_VENCIMENTO, ");
            sql.AppendLine(" H.DESCRICAO, COALESCE(B.VALOR_NEGOCIACAO, 0) VALOR_NEGOCIACAO, E.QTDDIASATRASO, F.CODIGO, G.MOTIVO ");

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

            sql.AppendLine(" WHERE UPPER(D.ENDERECO_LOGRADOURO) = @ENDERECO_LOGRADOURO ");

            sql.AppendLine(" AND D.ENDERECO_NUMERO = @ENDERECO_NUMERO ");

            DateTime data_atual = DateTime.Now.Date;
            sql.AppendLine($" AND (H.STATUS = 2 OR H.DATA_VENCIMENTO < '{data_atual.ToString("yyyy-MM-dd")}' AND H.STATUS = 0)");

            sql.AppendLine(" GROUP BY COD_CLIENTE, A.CPF, A.NOMEFANTASIA, D.ENDERECO_LOGRADOURO,  D.ENDERECO_NUMERO, D.ENDERECO_CIDADE, D.ENDERECO_UF, D.ENDERECO_BAIRRO,");
            sql.AppendLine(" COD_FATURA, VALOR_PAGO, STATUS, VALOR_TOTAL, DATA_VENCIMENTO, DESCRICAO, VALOR_NEGOCIACAO, E.QTDDIASATRASO, G.MOTIVO, F.CODIGO");
            sql.AppendLine(" ORDER BY H.DATA_VENCIMENTO ");

            param.Add("@ENDERECO_LOGRADOURO", logradouro.ToUpper());
            param.Add("@ENDERECO_NUMERO", numero);

            using (var com = _sessionNh.Connection.CreateCommand())
            using (var reader = CommandHelper.ExecuteReader(com, sql, param, this._sessionNh.Connection, null).Result)
            {
                while (reader.ReadAsync().Result)
                {
                    var endereco = new PendenciaEnderecoListModel();

                    //endereco.Cod_Contrato = reader["NUMERO_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["NUMERO_CONTRATO"]) : String.Empty; ;
                    endereco.CodConexao = reader["CODIGO"] != DBNull.Value ? Convert.ToString(reader["CODIGO"]): String.Empty;;
                    endereco.Cod_Cliente = reader["COD_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["COD_CLIENTE"]): String.Empty;;
                    endereco.Cpf = reader["CPF"] != DBNull.Value ? Convert.ToString(reader["CPF"]): String.Empty;;
                    //endereco.Contrato_Cancelado = reader["STATUS_CONTRATO"] != DBNull.Value ? Convert.ToString(reader["STATUS_CONTRATO"]) : String.Empty; ;
                    endereco.Conexao_Bloqueada = reader["STATUS"] != DBNull.Value ? Convert.ToString(reader["STATUS"]): String.Empty;;
                    endereco.Logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]): String.Empty;;
                    endereco.Numero = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_NUMERO"]): String.Empty;;
                    endereco.Bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]): String.Empty;;
                    endereco.Cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]): String.Empty;;
                    endereco.CodFatura = reader["COD_FATURA"] != DBNull.Value ? Convert.ToString(reader["COD_FATURA"]): String.Empty;;
                    endereco.Valor_Total = reader["VALOR_TOTAL"] != DBNull.Value ? Convert.ToString(reader["VALOR_TOTAL"]): String.Empty;;
                    endereco.Liquidado = reader["VALOR_NEGOCIACAO"] != DBNull.Value ? Convert.ToString(reader["VALOR_NEGOCIACAO"]): String.Empty;;
                    endereco.Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]): String.Empty;;
                    endereco.Fatura_Suspensa = reader["STATUS"] != DBNull.Value ? Convert.ToString(reader["STATUS"]): String.Empty;;
                    endereco.Data_Vencimento = reader["DATA_VENCIMENTO"] != DBNull.Value ? Convert.ToString(reader["DATA_VENCIMENTO"]): String.Empty;;
                    endereco.Motivo_Suspensao = reader["MOTIVO"] != DBNull.Value ? Convert.ToString(reader["MOTIVO"]): String.Empty;;

                    dados.Add(endereco);
                }
            }

            return dados;
        }
    }
}
