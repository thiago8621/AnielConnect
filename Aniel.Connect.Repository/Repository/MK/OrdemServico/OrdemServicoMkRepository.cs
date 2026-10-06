using Aniel.Connect.Repository.Model.Mk;
using Npgsql;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aniel.Connect.Repository.MK.OrdemServico
{
    public class OrdemServicoMkRepository : IOrdemServicoMkRepository
    {
        public OrdemServicoMkResponse GetOsMk(string mkConnectionString, string numOs)
        {
            using (var connMk = new NpgsqlConnection(mkConnectionString))
            {
                var parametros = new Dictionary<string, object> { { "numOs", numOs.Trim() } };
                var sql = new StringBuilder();

                sql.AppendLine("SELECT ");
                sql.AppendLine("  mos.codos AS cod_os, ");
                sql.AppendLine("  mos.cliente AS cod_cliente, ");
                sql.AppendLine("  mpe.nome_razaosocial, ");
                sql.AppendLine("  mpe.cpf, ");
                sql.AppendLine("  mpe.cnpj, ");
                sql.AppendLine("  mpe.fone01 AS fone_celular, ");
                sql.AppendLine("  mpe.fone02 AS fone_fixo, ");
                sql.AppendLine("  mpe.email, ");
                sql.AppendLine("  mpe.contato, ");
                sql.AppendLine("  mcd.descricao AS descricao_condominio, ");
                sql.AppendLine("  mpe.user_sac AS usuario_sac, ");
                sql.AppendLine("  mpe.pass_sac AS senha_sac, ");
                sql.AppendLine("  mos.cd_contrato AS cod_contrato, ");
                sql.AppendLine("  mos.data_abertura, ");
                sql.AppendLine("  mos.hora_abertura, ");
                sql.AppendLine("  mlo.logradouro, ");
                sql.AppendLine("  mos.num_endereco, ");
                sql.AppendLine("  coalesce(mco.complemento, mos.complemento) AS complemento, ");
                sql.AppendLine("  mos.cep, ");
                sql.AppendLine("  mes.siglaestado AS estado, ");
                sql.AppendLine("  mos.conexao_associada AS cod_conexao, ");
                sql.AppendLine("  mco.username AS usuario, ");
                sql.AppendLine("  mco.password AS senha, ");
                sql.AppendLine("  mpa.ssid AS ponto_acesso, ");
                sql.AppendLine("  mpla.descricao AS plano_acesso, ");
                sql.AppendLine("  mpla2.descricao AS plano_crm, ");
                sql.AppendLine("  mco.mac_address AS mac, ");
                sql.AppendLine("  mco.user_eap AS usuario_wpa, ");
                sql.AppendLine("  mco.wpa2_key AS senha_wpa, ");
                sql.AppendLine("  mos.tipo_os, ");
                sql.AppendLine("  mot.descricao AS tipo_os_descricao, ");
                sql.AppendLine("  mod.descricao_defeito AS defeito_descricao, ");
                sql.AppendLine("  mos.defeito_reclamado, ");
                sql.AppendLine("  ms.descricao AS sla_tipo, ");
                sql.AppendLine("  mos.observacoes, ");
                sql.AppendLine("  mos.indicacoes, ");
                sql.AppendLine("  mos.status, ");
                sql.AppendLine("  mos.encerrado, ");
                sql.AppendLine("  mba.bairro, ");
                sql.AppendLine("  mci.cidade, ");
                sql.AppendLine("  mpla.codplano AS cod_plano, ");
                sql.AppendLine("  coalesce(mos.cd_atendimento, mk_atend.codatendimento) AS cd_atendimento, ");
                sql.AppendLine("  mos.operador, ");
                sql.AppendLine("  mp.id_porta AS port_code, ");
                sql.AppendLine("  s.descricao AS cto_description ");
                sql.AppendLine("FROM ");
                sql.AppendLine("  mk_os AS mos ");
                sql.AppendLine("  FULL JOIN mk_pessoas AS mpe ON mos.cliente = mpe.codpessoa ");
                sql.AppendLine("  FULL JOIN mk_logradouros AS mlo ON mos.cd_logradouro = mlo.codlogradouro ");
                sql.AppendLine("  FULL JOIN mk_bairros AS mba ON mos.cd_bairro = mba.codbairro ");
                sql.AppendLine("  FULL JOIN mk_cidades AS mci ON mos.cd_cidade = mci.codcidade ");
                sql.AppendLine("  FULL JOIN mk_estados AS mes ON mos.cd_uf = mes.codestado ");
                sql.AppendLine("  FULL JOIN mk_conexoes AS mco ON mos.conexao_associada = mco.codconexao ");
                sql.AppendLine("  FULL JOIN mk_contratos AS mcon ON mos.cd_contrato = mcon.codcontrato ");
                sql.AppendLine("  LEFT JOIN mk_atendimento AS mk_atend ON mos.protocolo_os = mk_atend.protocolo ");
                sql.AppendLine("  LEFT JOIN maps_splitters s ON s.codsplitter = mco.cd_splitter ");
                sql.AppendLine("  LEFT JOIN maps_portas mp ON mp.cd_conexao = mco.codconexao AND mp.tipo = 2 AND mp.codporta = mco.id_porta_splitter ");
                sql.AppendLine("  FULL JOIN mk_pontos_acesso AS mpa ON mco.codponto_acesso = mpa.codpontoacesso ");
                sql.AppendLine("  FULL JOIN mk_planos_acesso AS mpla ON mco.codplano_acesso = mpla.codplano ");
                sql.AppendLine("  FULL JOIN mk_planos_acesso AS mpla2 ON mcon.plano_acesso = mpla2.codplano ");
                sql.AppendLine("  FULL JOIN mk_os_defeitos AS mod ON mos.defeito_associado = mod.coddefeito ");
                sql.AppendLine("  FULL JOIN mk_os_tipo AS mot ON mos.tipo_os = mot.codostipo ");
                sql.AppendLine("  FULL JOIN mk_condominios AS mcd ON mpe.condominio = mcd.codcondominio ");
                sql.AppendLine("  FULL JOIN mk_sla AS ms ON mcon.cd_tabela_sla = ms.codsla ");
                sql.AppendLine("WHERE ");
                sql.AppendLine("  mos.codos = @numOs ");

                OrdemServicoMkResponse ordemServico = null;

                using (DbDataReader reader = CommandHelper.ExecuteReaderPostgres(sql, parametros, connMk))
                {
                    if (reader.Read())
                    {
                        ordemServico = new OrdemServicoMkResponse
                        {
                            NumOS = Convert.ToString(reader["cod_os"]),
                            CodCliente = Convert.ToString(reader["cod_cliente"]),
                            NomeCliente = Convert.ToString(reader["nome_razaosocial"]),
                            Cpf = Convert.ToString(reader["cpf"]),
                            Cnpj = Convert.ToString(reader["cnpj"]),
                            TelefoneCelular = Convert.ToString(reader["fone_celular"]),
                            TelefoneFixo = Convert.ToString(reader["fone_fixo"]),
                            Email = Convert.ToString(reader["email"]),
                            Contato = Convert.ToString(reader["contato"]),
                            DescricaoCondominio = Convert.ToString(reader["descricao_condominio"]),
                            UsuarioSac = Convert.ToString(reader["usuario_sac"]),
                            SenhaSac = Convert.ToString(reader["senha_sac"]),
                            CodContrato = Convert.ToString(reader["cod_contrato"]),
                            DataAbertura = reader["data_abertura"] != DBNull.Value ? Convert.ToDateTime(reader["data_abertura"]) : (DateTime?)null,
                            HoraAbertura = reader["hora_abertura"] != DBNull.Value ? (TimeSpan?)reader["hora_abertura"] : null,
                            Logradouro = Convert.ToString(reader["logradouro"]),
                            NumeroEndereco = Convert.ToString(reader["num_endereco"]),
                            Complemento = Convert.ToString(reader["complemento"]),
                            Cep = Convert.ToString(reader["cep"]),
                            UF = Convert.ToString(reader["estado"]),
                            CodConexao = Convert.ToString(reader["cod_conexao"]),
                            UsuarioPPPoE = Convert.ToString(reader["usuario"]),
                            SenhaPPPoE = Convert.ToString(reader["senha"]),
                            PontoAcesso = Convert.ToString(reader["ponto_acesso"]),
                            PlanoAcesso = Convert.ToString(reader["plano_acesso"]),
                            PlanoCrm = Convert.ToString(reader["plano_crm"]),
                            CodPlano = Convert.ToString(reader["cod_plano"]),
                            Mac = Convert.ToString(reader["mac"]),
                            UsuarioWpa = Convert.ToString(reader["usuario_wpa"]),
                            SenhaWpa = Convert.ToString(reader["senha_wpa"]),
                            TipoOs = Convert.ToString(reader["tipo_os"]),
                            TipoOsDescricao = Convert.ToString(reader["tipo_os_descricao"]),
                            DefeitoDescricao = Convert.ToString(reader["defeito_descricao"]),
                            DefeitoReclamado = Convert.ToString(reader["defeito_reclamado"]),
                            SlaTipo = Convert.ToString(reader["sla_tipo"]),
                            Observacoes = Convert.ToString(reader["observacoes"]),
                            Indicacoes = Convert.ToString(reader["indicacoes"]),
                            Status = Convert.ToString(reader["status"]),
                            Encerrado = Convert.ToString(reader["encerrado"]),
                            Bairro = Convert.ToString(reader["bairro"]),
                            Cidade = Convert.ToString(reader["cidade"]),
                            CdAtendimento = Convert.ToString(reader["cd_atendimento"]),
                            Operador = Convert.ToString(reader["operador"]),
                            PortCode = Convert.ToString(reader["port_code"]),
                            CtoDescription = Convert.ToString(reader["cto_description"])
                        };
                    }
                }

                return ordemServico;
            }
        }
    }
}
