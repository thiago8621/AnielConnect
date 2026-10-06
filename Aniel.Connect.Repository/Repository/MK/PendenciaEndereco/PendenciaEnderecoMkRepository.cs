using Aniel.Connect.Repository.Model;
using Npgsql;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aniel.Connect.Repository.MK.PendenciaEndereco
{
    public class PendenciaEnderecoMkRepository : IPendenciaEnderecoMkRepository
    {
        private readonly NpgsqlConnection _connection;

        public PendenciaEnderecoMkRepository(NpgsqlConnection connection)
        {
            this._connection = connection;
        }

        public RetornoConsultaDescontos BuscarFormasPagamento(string codigoCombo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero,string cidade)
        {
            var dados = new List<PendenciaEnderecoListModel>();

            var sql = new StringBuilder();
            var param = new Dictionary<string, object>();

            sql.AppendLine(" SELECT ");
            sql.AppendLine(" mpe.codpessoa AS cod_cliente, ");
            sql.AppendLine(" mco.codcontrato AS cod_contrato, ");
            sql.AppendLine(" mcon.codconexao, ");
            sql.AppendLine(" mpe.nome_razaosocial AS cliente, ");
            sql.AppendLine(" mpe.cpf, ");
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
            sql.AppendLine(" mfa.cd_motivo_suspensao, ");
            sql.AppendLine(" mms.descricao ");
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
            sql.AppendLine(" mlo.logradouro = @logradouro AND ");
            sql.AppendLine(" UPPER(mci.cidade) = UPPER(@cidade) AND ");
            sql.AppendLine(" mcon.numero = @numero ");
            sql.AppendLine(" ORDER BY ");
            sql.AppendLine(" mfa.data_vencimento ");

            param.Add("logradouro", logradouro);
            param.Add("numero", numero);
            param.Add("cidade", cidade);

            using (DbDataReader reader = CommandHelper.ExecuteReaderPostgres(sql, param, this._connection))
            {
                while (reader.Read())
                {
                    var endereco = new PendenciaEnderecoListModel();

                    endereco.Cod_Contrato = Convert.ToString(reader["cod_contrato"]);
                    endereco.CodConexao = Convert.ToString(reader["codconexao"]);
                    endereco.Cod_Cliente = Convert.ToString(reader["cod_cliente"]);
                    endereco.Cpf = Convert.ToString(reader["cpf"]);
                    endereco.Contrato_Cancelado = Convert.ToString(reader["contrato_cancelado"]);
                    endereco.Conexao_Bloqueada = Convert.ToString(reader["conexao_bloqueada"]);
                    endereco.Logradouro = Convert.ToString(reader["logradouro"]);
                    endereco.Bairro = Convert.ToString(reader["bairro"]);
                    endereco.Cidade = Convert.ToString(reader["cidade"]);
                    endereco.CodFatura = Convert.ToString(reader["codfatura"]);
                    endereco.Valor_Total = Convert.ToString(reader["valor_total"]);
                    endereco.Liquidado = Convert.ToString(reader["liquidado"]);
                    endereco.Descricao = Convert.ToString(reader["descricao"]);
                    endereco.Fatura_Suspensa = Convert.ToString(reader["fatura_suspensa"]);
                    endereco.Data_Vencimento = Convert.ToString(reader["data_vencimento"]);
                    endereco.Motivo_Suspensao = Convert.ToString(reader["cd_motivo_suspensao"]);

                    endereco.Numero = Convert.ToString(reader["numero"]);
                    endereco.Cliente = Convert.ToString(reader["cliente"]);

                    dados.Add(endereco);
                }
            }

            return dados;
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEnderecoAlcans(string codigoUF, string codigoCidade, string codigoBairro, string codigoLogradouro, string numero)
        {
            throw new NotImplementedException();
        }
    }
}
