using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Npgsql;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aniel.Connect.Repository.MK.PendenciaFinanceira
{
    public class PendenciaFinanceiraMkRepository : IPendenciaFinanceiraIntegracaoRepository
    {
        private readonly NpgsqlConnection _npgsqlConnection;

        public PendenciaFinanceiraMkRepository(NpgsqlConnection npgsqlConnection)
        {
            this._npgsqlConnection = npgsqlConnection;
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

            sql.AppendLine(" SELECT ");
            sql.AppendLine(" mpe.codpessoa AS cod_cliente, ");
            sql.AppendLine(" mco.codcontrato AS cod_contrato, ");
            sql.AppendLine(" mcon.codconexao AS cod_conexao, ");
            sql.AppendLine(" mpe.nome_razaosocial AS cliente, ");

            if (pessoaFisica)
                sql.AppendLine(" mpe.cpf, ");
            else
                sql.AppendLine(" mpe.cnpj,");

            sql.AppendLine(" mco.cancelado AS contrato_cancelado, ");
            sql.AppendLine(" mcon.conexao_bloqueada, ");
            sql.AppendLine(" mlo.logradouro, ");
            sql.AppendLine(" mcon.numero, ");
            sql.AppendLine(" mba.bairro, ");
            sql.AppendLine(" mci.cidade, ");
            sql.AppendLine(" mfa.codfatura, ");
            sql.AppendLine(" mfa.valor_total, ");
            sql.AppendLine(" mfa.liquidado, ");
            sql.AppendLine(" mfa.descricao, ");
            sql.AppendLine(" mfa.suspenso AS fatura_suspensa, ");
            sql.AppendLine(" mfa.data_vencimento, ");
            sql.AppendLine(" mms.descricao AS motivo_suspensao ");
            sql.AppendLine(" FROM ");
            sql.AppendLine(" mk_pessoas AS mpe ");
            sql.AppendLine(" INNER JOIN mk_faturas AS mfa ON(mfa.cd_pessoa = mpe.codpessoa) ");
            sql.AppendLine(" INNER JOIN mk_contas_faturadas AS mcf ON(mcf.cd_fatura = mfa.codfatura) ");
            sql.AppendLine(" INNER JOIN mk_plano_contas AS mpc ON(mpc.codconta = mcf.cd_conta) ");
            sql.AppendLine(" INNER JOIN mk_contratos AS mco ON(mco.codcontrato = mpc.codvinculado) ");
            sql.AppendLine(" INNER JOIN mk_conexoes AS mcon ON(mcon.contrato = mco.codcontrato) ");
            sql.AppendLine(" INNER JOIN mk_logradouros AS mlo ON(mlo.codlogradouro = mcon.logradouro) ");
            sql.AppendLine(" INNER JOIN mk_bairros AS mba ON(mba.codbairro = mcon.bairro) ");
            sql.AppendLine(" INNER JOIN mk_cidades AS mci ON(mci.codcidade = mcon.cidade) ");
            sql.AppendLine(" FULL JOIN mk_motivo_suspensao AS mms ON(mms.codmotsusp = mfa.cd_motivo_suspensao) ");
            sql.AppendLine(" WHERE ");
            sql.AppendLine(" mfa.liquidado = 'N' AND ");
            sql.AppendLine(" mfa.excluida = 'N' AND ");
            sql.AppendLine(" mfa.tipo = 'R' AND ");
            sql.AppendLine(" mfa.data_vencimento < NOW() AND ");
            sql.AppendLine(" (mfa.cd_motivo_suspensao IN(1015, 1016, 1023, 1033, 1034, 1038, 1042, 1045, 1046, 1049, 1050, 1051) ");
            sql.AppendLine(" OR mfa.cd_motivo_suspensao IS NULL) AND ");

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

                sql.AppendLine(" mpe.cpf = @cpf ");
                param.Add("cpf", cpfFormatado);
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

                sql.AppendLine(" mpe.cnpj = @cnpj ");
                param.Add("cnpj", cnpjFormatado);
            }

            sql.AppendLine(" ORDER BY ");
            sql.AppendLine(" mfa.data_vencimento ");

            using (DbDataReader reader = CommandHelper.ExecuteReaderPostgres(sql, param, this._npgsqlConnection))
            {
                while (reader.Read())
                {
                    var values = new PendenciaFinanceiraListModel();

                    values.Cod_Contrato = Convert.ToString(reader["cod_contrato"]);
                    values.CodConexao = Convert.ToString(reader["cod_conexao"]);
                    values.Cod_Cliente = Convert.ToString(reader["cod_cliente"]);
                    values.Cliente = Convert.ToString(reader["cliente"]);

                    if (pessoaFisica)
                        values.CpfCnpj = Convert.ToString(reader["cpf"]);
                    else
                        values.CpfCnpj = Convert.ToString(reader["cnpj"]);

                    values.Contrato_Cancelado = Convert.ToString(reader["contrato_cancelado"]);
                    values.Conexao_Bloqueada = Convert.ToString(reader["conexao_bloqueada"]);
                    values.Logradouro = Convert.ToString(reader["logradouro"]);
                    values.Numero = Convert.ToString(reader["numero"]);
                    values.Bairro = Convert.ToString(reader["bairro"]);
                    values.Cidade = Convert.ToString(reader["cidade"]);
                    values.CodFatura = Convert.ToString(reader["codfatura"]);
                    values.Valor_Total = Convert.ToString(reader["valor_total"]);
                    values.Liquidado = Convert.ToString(reader["liquidado"]);
                    values.Descricao = Convert.ToString(reader["descricao"]);
                    values.Fatura_Suspensa = Convert.ToString(reader["fatura_suspensa"]);
                    values.Data_Vencimento = Convert.ToString(reader["data_vencimento"]);
                    values.Motivo_Suspensao = Convert.ToString(reader["motivo_suspensao"]);

                    dados.Add(values);
                }
            }

            return dados;
        }
    }
}
