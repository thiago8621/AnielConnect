using Sinapse.DAO;
using System.Text;

namespace Aniel.T5.DAO
{


    public class _20251128_0847_ATUALIZACAO_AREA_ACESSO : AnielMigration
    {
        public _20251128_0847_ATUALIZACAO_AREA_ACESSO()
        {
            Codigo = "20260416_1000";
            Descricao = "Areas de Acesso Connect";
            Tipo = TipoAnielMigration.ScriptMultiplo;
        }

        public override string GetSql()
        {
            return new StringBuilder().AppendLine(@"UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '01', 'ACESSO AO SISTEMA', 'N', 'Reabre Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '02', 'Cadastrar / Alterar Atendimento', 'N', 'Cadastra e altera Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '03', 'Excluir Atendimento', 'N', 'Exclui Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '04', 'Visualizar Atendimento', 'N', 'Visualiza Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '05', 'Cadastrar / Alterar Atividade NF', 'N', 'Cadastra e altera Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '06', 'Excluir Atividade NF', 'N', 'Exclui Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '07', 'Visualizar Atividade NF', 'N', 'Visualiza Atividade NF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '08', 'Cadastrar / Alterar Calendario', 'N', 'Cadastra e altera Calendario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '09', 'Excluir Calendario', 'N', 'Exclui Calendario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '10', 'Visualizar Calendario', 'N', 'Visualiza Calendario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '11', 'Cadastrar / Alterar Campanha', 'N', 'Cadastra e altera Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '12', 'Excluir Campanha', 'N', 'Exclui Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '13', 'Visualizar Campanha', 'N', 'Visualiza Campanha')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '14', 'Cadastrar / Alterar Categoria Atendimento', 'N', 'Cadastra e altera Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '15', 'Excluir Categoria Atendimento', 'N', 'Exclui Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '16', 'Visualizar Categoria Atendimento', 'N', 'Visualiza Categoria Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '17', 'Cadastrar / Alterar Categoria Cliente', 'N', 'Cadastra e altera Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '18', 'Excluir Categoria Cliente', 'N', 'Exclui Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '19', 'Visualizar Categoria Cliente', 'N', 'Visualiza Categoria Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '20', 'Cadastrar / Alterar Cliente', 'N', 'Cadastra e altera Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '21', 'Excluir Cliente', 'N', 'Exclui Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '22', 'Visualizar Cliente', 'N', 'Visualiza Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '23', 'Cadastrar / Alterar Cliente Prospecto', 'N', 'Cadastra e altera Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '24', 'Excluir Cliente Prospecto', 'N', 'Exclui Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '25', 'Visualizar Cliente Prospecto', 'N', 'Visualiza Cliente Prospecto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '26', 'Cadastrar / Alterar Commisionamento', 'N', 'Cadastra e altera Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '27', 'Excluir Commisionamento', 'N', 'Exclui Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '28', 'Visualizar Commisionamento', 'N', 'Visualiza Commisionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '29', 'Cadastrar / Alterar Configuracoes Gerais', 'N', 'Cadastra e altera Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '30', 'Excluir Configuracoes Gerais', 'N', 'Exclui Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '31', 'Visualizar Configuracoes Gerais', 'N', 'Visualiza Configuracoes Gerais')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '32', 'Cadastrar / Alterar Desconto', 'N', 'Cadastra e altera Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '33', 'Excluir Desconto', 'N', 'Exclui Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '34', 'Visualizar Desconto', 'N', 'Visualiza Desconto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '35', 'Cadastrar / Alterar Faixa Comissionamento', 'N', 'Cadastra e altera Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '36', 'Excluir Faixa Comissionamento', 'N', 'Exclui Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '37', 'Visualizar Faixa Comissionamento', 'N', 'Visualiza Faixa Comissionamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '38', 'Cadastrar / Alterar Feriado', 'N', 'Cadastra e altera Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '39', 'Excluir Feriado', 'N', 'Exclui Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '40', 'Visualizar Feriado', 'N', 'Visualiza Feriado')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '41', 'Cadastrar / Alterar Filial', 'N', 'Cadastra e altera Filial')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '42', 'Excluir Filial', 'N', 'Exclui Filial')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '43', 'Visualizar Filial', 'N', 'Visualiza Filial')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '44', 'Cadastrar / Alterar Funcionario', 'N', 'Cadastra e altera Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '45', 'Excluir Funcionario', 'N', 'Exclui Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '46', 'Visualizar Funcionario', 'N', 'Visualiza Funcionario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '47', 'Cadastrar / Alterar Meio de Captacao', 'N', 'Cadastra e altera Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '48', 'Excluir Meio de Captacao', 'N', 'Exclui Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '49', 'Visualizar Meio de Captacao', 'N', 'Visualiza Meio de Captacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '50', 'Cadastrar / Alterar Meta', 'N', 'Cadastra e altera Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '51', 'Excluir Meta', 'N', 'Exclui Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '52', 'Visualizar Meta', 'N', 'Visualiza Meta')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '53', 'Cadastrar / Alterar Modelo Contrato', 'N', 'Cadastra e altera Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '54', 'Inativar / Ativar Modelo Contrato', 'N', 'Inativar / Ativar Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '55', 'Visualizar Modelo Contrato', 'N', 'Visualiza Modelo Contrato')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '56', 'Cadastrar / Alterar Modelo Recibo', 'N', 'Cadastra e altera Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '57', 'Excluir Modelo Recibo', 'N', 'Exclui Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '58', 'Visualizar Modelo Recibo', 'N', 'Visualiza Modelo Recibo')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '59', 'Cadastrar / Alterar Motivo Encerramento', 'N', 'Cadastra e altera Motivo Encerramento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '60', 'Excluir Motivo Encerramento', 'N', 'Exclui Motivo Encerramento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '61', 'Visualizar Motivo Encerramento', 'N', 'Visualiza Motivo Encerramento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '62', 'Cadastrar / Alterar Motivo Status Atendimento', 'N', 'Cadastra e altera Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '63', 'Excluir Motivo Status Atendimento', 'N', 'Exclui Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '64', 'Visualizar Motivo Status Atendimento', 'N', 'Visualiza Motivo Status Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '65', 'Cadastrar / Alterar NAS', 'N', 'Cadastra e altera NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '66', 'Excluir NAS', 'N', 'Exclui NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '67', 'Visualizar NAS', 'N', 'Visualiza NAS')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '68', 'Cadastrar / Alterar Origem Atendimento', 'N', 'Cadastra e altera Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '69', 'Excluir Origem Atendimento', 'N', 'Exclui Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '70', 'Visualizar Origem Atendimento', 'N', 'Visualiza Origem Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '71', 'Cadastrar / Alterar Pesquisa Satisfacao', 'N', 'Cadastra e altera Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '72', 'Excluir Pesquisa Satisfacao', 'N', 'Exclui Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '73', 'Visualizar Pesquisa Satisfacao', 'N', 'Visualiza Pesquisa Satisfacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '74', 'Cadastrar / Alterar Plano/Produto', 'N', 'Cadastra e altera Planos e Produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '75', 'Excluir Plano/Produto', 'N', 'Exclui Planos e Produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '76', 'Visualizar Plano/Produto', 'N', 'Visualiza Planos e Produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '80', 'Cadastrar / Alterar Ponto de Venda', 'N', 'Cadastra e altera Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '81', 'Excluir Ponto de Venda', 'N', 'Exclui Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '82', 'Visualizar Ponto de Venda', 'N', 'Visualiza Ponto de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '83', 'Cadastrar / Alterar Script Atendimento', 'N', 'Cadastra e altera Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '84', 'Excluir Script Atendimento', 'N', 'Exclui Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '85', 'Visualizar Script Atendimento', 'N', 'Visualiza Script Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '86', 'Cadastrar / Alterar Sla Atendimento', 'N', 'Cadastra e altera Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '87', 'Excluir Sla Atendimento', 'N', 'Exclui Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '88', 'Visualizar Sla Atendimento', 'N', 'Visualiza Sla Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '89', 'Cadastrar / Alterar Sla Execucao', 'N', 'Cadastra e altera Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '90', 'Excluir Sla Execucao', 'N', 'Exclui Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '91', 'Visualizar Sla Execucao', 'N', 'Visualiza Sla Execucao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '92', 'Cadastrar / Alterar Tipo Atendimento', 'N', 'Cadastra e altera Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '93', 'Excluir Tipo Atendimento', 'N', 'Exclui Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '94', 'Visualizar Tipo Atendimento', 'N', 'Visualiza Tipo Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '95', 'Cadastrar / Alterar Tipo Checkpoint Vendedor', 'N', 'Cadastra e altera Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '96', 'Excluir Tipo Checkpoint Vendedor', 'N', 'Exclui Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '97', 'Visualizar Tipo Checkpoint Vendedor', 'N', 'Visualiza Tipo Checkpoint Vendedor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '98', 'Visualizar Escalas de Todas as Equipes', 'N', 'Visualizar escalas de todas as Equipes no Connect.')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '99', 'Cadastrar / Alterar Horario', 'N', 'Cadastra e Alter Horario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '100', 'Cadastrar / Alterar Campos Obrigatorios', 'N', 'Cadastra e altera Campos Obrigatorios')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '101', 'Cadastrar / Alterar Radius', 'N', 'Cadastrar e alterar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '102', 'Cadastra / Alterar OLT', 'N', 'Cadastrar e Alterar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '103', 'Visualizar Monitor Equipe', 'N', 'Visualizar (Monitor Equipes)')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '104', 'Visualizar Ponto Partida', 'N', 'Visualizar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '105', 'Visualizar Habilitar Servicos Equipe', 'N', 'Visualizar Habilitar Servicos Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '106', 'Visualizar Habilitar Area Equipe', 'N', 'Visualizar Habilitar Area Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '107', 'Ativar / Inativar Horario', 'N', 'Ativar ou Inativar Horario')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '108', 'Visualizar Criterio Roteirizacao', 'N', 'Visualizar Criterio Roteirizacao')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '109', 'Consultar Pesquisa Satisfacao CadCliente', 'N', 'Consultar Pesquisa Satisfacao CadCliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '110', 'Visualizar Radius', 'N', 'Visualizar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '111', 'Inativar / Ativar Radius', 'N', 'Inativar ou Ativar Radius')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '112', 'Visualizar OLT', 'N', 'Visualizar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '113', 'Inativar / Ativar OLT', 'N', 'Inativar ou Ativar OLT')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '114', 'Cadastro Escala de Trabalho', 'N', 'Cadastro Escala de Trabalho')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '115', 'Adicionar Monitor Equipe', 'N', 'Adicionar Monitor Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '116', 'Excluir  Monitor Equipe', 'N', 'Excluir Monitor Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '117', 'Cadastrar Ponto Partida', 'N', 'Cadastrar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '118', 'Inativar / Ativar Ponto Partida', 'N', 'Inativar ou Ativar Ponto Partida')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '119', 'Adicionar Servico', 'N', 'Adicionar Servico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '120', 'Adicionar Area Equipe', 'N', 'Adicionar Area Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '121', 'Acesso Menu Comercial', 'N', 'Acesso Menu Comercial')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '122', 'Visualizar Venda', 'N', 'Visualizar Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '123', 'Visualizar Acompanhamento de Vendedores', 'N', 'Visualizar Acompanhamento de Vendedores')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '124', 'Visualizar Gestão de Equipe', 'N', 'Visualizar Gestão de Equipe')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '125', 'Visualizar Configurações', 'N', 'Visualizar Configurações')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '126', 'Visualizar Relatórios', 'N', 'Visualizar Relatórios')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '127', 'Visualizar Indicadores', 'N', 'Visualizar Indicadores')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '128', 'Visualizar Roteirização', 'N', 'Visualizar Roteirização')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '129', 'Visualizar Ajuda', 'N', 'Visualizar Ajuda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '130', 'Acompanhamento de Equipes', 'N', 'Acompanhamento de Equipes')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '131', 'Equipes no Mapa', 'N', 'Equipes no Mapa')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '132', 'Criar Ordem de Serviço', 'N', 'Criar Ordem de Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '133', 'Painel de Serviços', 'N', 'Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '134', 'Timeline de Serviços', 'N', 'Timeline de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '135', 'Cadastrar / Alterar Venda', 'N', 'Cadastrar / Alterar Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '136', 'Excluir Venda', 'N', 'Excluir Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '137', 'Habilitar Usuário a cadastrar sem acesso RF', 'N', 'Habilitar Usuário a cadastrar sem acesso RF')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '138', 'Alterar Endereço do Cliente', 'N', 'Alterar Endereço do Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '139', 'Definir Data/Hora Execução do Serviço', 'S', 'Definir Data/Hora Execução do Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '140', 'Cadastrar / Alterar Script Suporte Técnico', 'N', 'Cadastrar / Alterar Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '141', 'Excluir Script Suporte Técnico', 'N', 'Excluir Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '142', 'Visualizar Script Suporte Técnico', 'N', 'Visualizar Script Suporte Técnico')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '143', 'Cadastrar / Alterar Perfil de Dashboard', 'N', 'Cadastrar / Alterar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '144', 'Excluir Perfil de Dashboard', 'N', 'Excluir Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '145', 'Visualizar Perfil de Dashboard', 'N', 'Visualizar Perfil de Dashboard')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '146', 'Cadastrar / Alterar Vincular Usuario ao Perfil', 'N', 'Cadastrar / Alterar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '147', 'Excluir Vincular Usuario ao Perfil', 'N', 'Excluir Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '148', 'Visualizar Vincular Usuario ao Perfil', 'N', 'Visualizar Vincular Usuario ao Perfil')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '149', 'Visualizar Gerencia de Rede', 'N', 'Visualizar Gerencia de Rede')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '150', 'Reabrir Atendimento', 'N', 'Reabrir Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '151', 'ESTORNO DE LANCAMENTO DE FATURAMENTO', 'N', NULL)
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '152', 'Permitir aprovar venda', 'N', 'Permitir aprovar venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '153', 'Alterar Vendedor/Supervisor', 'N', 'Alterar Vendedor/Supervisor')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '154', 'Visualizar Modelo de Painel de Atendimento', 'N', 'Visualizar Modelo de Painel de Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '155', 'Inativar / Ativar Modelo de Painel de Atendimento', 'N', 'Inativar / Ativar Modelo de Painel de Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '156', 'Cadastrar/Alterar Modelo de Painel de Atendimento', 'N', 'Cadastrar/Alterar Modelo de Painel de Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '157', 'Visualizar Motivo de Desistência da Venda', 'N', 'Visualizar Motivo de Desistência da Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '158', 'Inativar / Ativar Motivo de Desistência da Venda', 'N', 'Inativar / Ativar Motivo de Desistência da Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '159', 'Cadastrar/Alterar Motivo de Desistência da Venda', 'N', 'Cadastrar/Alterar Motivo de Desistência da Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '160', 'Visualizar Motivo de Canc. de Plano/Produto', 'N', 'Visualizar Motivo de Canc. de Plano/Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '161', 'Cadastrar/Alterar Motivo de Canc de Plano/Produto', 'N', 'Cadastrar/Alterar Motivo de Canc de Plano/Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '162', 'Gerenciar Modelos de Visualização dos Painéis', 'N', 'Permite Gerenciar o Modelo de Visualização dos painéis de Serviço, Vendas e Atendimento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '163', 'Inativar/Ativar Motivo de Canc. de Plano/Produto', 'N', 'Inativar/Ativar Motivo de Canc. de Plano/Produto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '164', 'Atribuir responsável em massa', 'N', 'Atribuir responsável em massa')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '165', 'Relatório de Serviços Improdutivos', 'N', 'Relatório de Serviços Improdutivos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '166', 'Relatório de Acompanhamento de Serviços', 'N', 'Relatório de Acompanhamento de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '167', 'Relatório de Produção', 'N', 'Relatório de Produção')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '168', 'Relatório de Materiais Aplicados/Retirados', 'N', 'Relatório de Relatório de Materiais Aplicados/Retirados')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '169', 'Cadastrar/Alterar Modelo de Painel de Serviços', 'N', 'Cadastrar/Alterar Modelo de Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '170', 'Visualizar Modelo de Painel de Serviços', 'N', 'Visualizar Modelo de Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '171', 'Inativar / Ativar Modelo de Painel de Serviços', 'N', 'Inativar / Ativar Modelo de Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '172', 'Cadastrar/Alterar Modelo de Painel de Vendas', 'N', 'Cadastrar/Alterar Modelo de Painel de Vendas')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '173', 'Visualizar Modelo de Painel de Vendas', 'N', 'Visualizar Modelo de Painel de Vendas')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '174', 'Inativar / Ativar Modelo de Painel de Vendas', 'N', 'Inativar / Ativar Modelo de Painel de Vendas')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '175', 'Acesso ao Relatório de Capacidade Agendamento', 'N', NULL)
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '176', 'Visualizar todos técnicos painel acompanhamento', 'N', 'Visualizar todos os técnicos no painel de acompanhamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '177', 'Inativar / Ativar Ponto de Venda', 'N', 'Inativar / Ativar Ponto de Venda');
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '178', 'Visualizar Atendimentos do Painel de Atendimentos', 'N', 'Visualizar Atendimentos do Painel de Atendimentos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '179', 'Visualizar Acompanhamento dos Agentes', 'N', 'Visualizar Acompanhamento dos Agentes')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '180', 'Visualizar Acompanhamento dos Atendimentos', 'N', 'Visualizar Acompanhamento dos Atendimentos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '181', 'Visualizar Atendimentos Via Chat', 'N', 'Visualizar Atendimentos Via Chat')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '182', 'Cancelamento de Ordem de Serviço', 'N', 'Cancelamento de Ordem de Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '183', 'Desbloqueios em confiança sem frequência limitada', 'N', 'Realizar o desbloqueio em confiança fora da frequência especificada no cadastro do plano')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '184', 'Mesclar Atendimento', 'N', 'Realizar a mesclagem entre atendimentos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '185', 'Visualizar Webhook', 'N', 'Visualiza Webhook')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '186', 'Cadastrar / Alterar Webhook', 'N', 'Cadastra e altera Webhook')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '187', 'Visualizar os chats de todos os setores', 'N', 'Visualizar os chats de todos os setores')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '188', 'Permitir Atribuir tecnico que possue agendamento', 'N', 'Permitir possibilidade de atribuir tecnico que possue agendamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '189', 'Cadastrar/Alterar Bots/Eventos', 'N', 'Cadastrar/Alterar Bots/Eventos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '190', 'Ativar/Inativar/Excluir Bots/Eventos', 'N', 'Ativar/Inativar/Excluir Bots/Eventos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '191', 'Visualizar Canais de Comunicação', 'N', 'Visualizar Canais de Comunicação')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '192', 'Cadastrar/Alterar Canais de Comunicação', 'N', 'Cadastrar/Alterar Canais de Comunicação')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '193', 'Ativar/Inativar/Excluir Canais de Comunicação', 'N', 'Ativar/Inativar/Excluir Canais de Comunicação')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '194', 'Excluir Checklist de Serviço', 'N', 'Excluir Checklist de Serviço')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '195', 'Editar as respostas do checklist', 'N', 'Editar as respostas do checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '196', 'Inserir/Alterar/Excluir as fotos do checklist', 'N', 'Inserir/Alterar/Excluir as fotos do checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '197', 'Relatório Alterações no checklist', 'N', 'Relatório Alterações no checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '198', 'Visualizar Bots/Eventos', 'N', 'Visualizar Bots/Eventos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '199', 'Atribuir Técnico sem regra', 'N', 'Atribuir Técnico sem regra')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '200', 'Permitir auditoria de venda', 'N', 'Permitir auditoria de venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '201', 'Visualizar Motivo de Inviabilidade', 'N', 'Visualizar Motivo de Inviabilidade')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '202', 'Cadastrar / Alterar Motivo de Inviabilidade', 'N', 'Cadastrar / Alterar Motivo de Inviabilidade')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','203','Inativar Manutenção de Rede','N','Inativar Manutenção de Rede')
                            MATCHING (SIGLA_MODULO,COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '204', 'Alterar Atendimento de outro Responsavel', 'N', 'Alterar Atendimento de outro Responsavel')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '205', 'Visualizar apenas Prospecto do meu setor', 'N', 'Visualizar apenas Prospecto do meu setor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','209','Visualizar Portabilidade','N','O Usuário só irá Visualizar as Portabilidades Cadastradas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','210','Cadastrar/Alterar Portabilidade','N','O Usuário poderá cadastrar e alterar qualquer informação da Portabilidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','211','Excluir Portabilidade','N','O Usuário poderá excluir a Portabilidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','212','Visualizar Did','N','O Usuário só irá Visualizar os DIDs Cadastrados')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','213','Cadastrar DIDs','N','O Usuário poderá cadastrar novos DIDs')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','214','Inativar/Ativar Rota','N','O Usuário poderá ativar ou inativar qualquer Rota')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','215','Visualizar Circuito Cadastrado','N','O Usuário só irá Visualizar os Circuitos Cadastrados')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','216','Cadastrar/Alterar Circuito','N','O Usuário poderá cadastrar e alterar qualquer informação sobre os DIDs')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','217','Excluir DIDs de um circuito vinculado a cliente','N','O Usuário poderá excluir os DIDs de um circuito vinculado a um cliente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','218','Alterar DIDs','N','O Usuário poderá alterar informações relacionadas aos DIDs')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','219','Visualizar Franquia','N','O usuário poderá visualizar as Franquias cadastradas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','220','Cadastrar Franquia','N','O Usuário poderá cadastrar novas Franquias')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','221','Alterar Franquia','N','Permite alterar qualquer informação de uma franquia já criada.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','222','Ativar ou Desativar Franquia','N','O Usuário poderá Ativar ou Desativar Franquias')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '206', 'Visualizar apenas Vendas do meu setor', 'N', 'Visualizar apenas Vendas do meu setor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '207', 'Ocultar visualização de vendas pela data limite', 'N', 'Ocultar visualização de vendas pela data limite')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '223', 'Visualizar Workflow de Vendas', 'N', 'Visualizar Workflow de Vendas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '224', 'Criar Workflow de Vendas', 'N', 'Criar Workflow de Vendas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '225', 'Alterar Workflow de Vendas', 'N', 'Alterar Workflow de Vendas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '226', 'Visualizar Status de Venda', 'N', 'Visualizar Status de Venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '227', 'Criar Status de Venda', 'N', 'Criar Status de Venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '228', 'Alterar Status de Venda', 'N', 'Alterar Status de Venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);


UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '229', 'Visualizar apenas Area de Cobrança de Serviço', 'N', 'Visualizar apenas Area de Cobrança de Serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '230', 'Permitir Cadastro de Cobrança de Serviço', 'N', 'Caso o Usuario não tenha acesso, Notificar ao Usuario ')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '231', 'Ocultar Alterar Registro de Cobrança de Serviço', 'N', 'Caso o Usuario não tenha acesso, Notificar ao Usuario ')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '232', 'Visualizar apenas Os Botões de Ap/Rp Cob.Ser', 'N', 'Visualizar e Manipular Botões de Ap/Rp Cob.Ser')
                              MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '233', 'Visualizar Áreas Cadastradas', 'N', 'Visualizar Áreas Cadastradas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '234', 'Cadastrar Área', 'N', 'Cadastrar Área')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '235', 'Alterar Área', 'N', 'Alterar Área')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '236', 'Visualizar Grupo Área Cadastrados', 'N', 'Visualizar Grupo Área Cadastrados')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '237', 'Cadastrar Grupo Área', 'N', 'Cadastrar Grupo Área')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '238', 'Alterar Grupo Área', 'N', 'Alterar Grupo Área')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '239', 'Visualizar Localidades Cadastradas', 'N', 'Visualizar Localidades Cadastradas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '240', 'Cadastrar Localidade', 'N', 'Cadastrar Localidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '241', 'Alterar Localidade', 'N', 'Alterar Localidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '242', 'Visualizar UF', 'N', 'Visualizar UF')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '243', 'Visualizar Cidade', 'N', 'Visualizar Cidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '244', 'Visualizar Bairro', 'N', 'Visualizar Bairro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '245', 'Visualizar Endereço/Logradouro', 'N', 'Visualizar Endereço/Logradouro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '246', 'Visualizar Condomínio/Prédio', 'N', 'Visualizar Condomínio/Prédio')
                              MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '247', 'Cadastrar UF', 'N', 'Cadastrar UF')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '248', 'Cadastrar Cidade', 'N', 'Cadastrar Cidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '249', 'Cadastrar Bairro', 'N', 'Cadastrar Bairro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '250', 'Cadastrar Endereço/Logradouro', 'N', 'Cadastrar Endereço/Logradouro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '251', 'Cadastrar Condomínio/Prédio', 'N', 'Cadastrar Condomínio/Prédio')
                              MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '252', 'Alterar UF', 'N', 'Alterar UF')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '253', 'Alterar Cidade', 'N', 'Alterar Cidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '254', 'Alterar Bairro', 'N', 'Alterar Bairro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '255', 'Alterar Endereço/Logradouro', 'N', 'Alterar Endereço/Logradouro')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '256', 'Alterar Condomínio/Prédio', 'N', 'Alterar Condomínio/Prédio')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','257','Visualizar ONU desprovisionadas sem vinculo de OS','N','O Usuário poderá visualizar ONUs pendentes de autorização que não possuem vinculo com OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','258','Movimentar OS','N','O Usuário poderá movimentar as OSs na timeline de serviços')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','259','Visualizar Rotas','N','Visualizar Rotas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','260','Cadastrar Rota','N','O Usuário poderá cadastrar novas Rotas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','261','Alterar Rota','N','O Usuário poderá alterar informações relacionadas as Rotas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','262','Responsável Aprovar/Reprovar auditoria OS','N','Responsável Aprovar/Reprovar auditoria OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','263','Visualizar Anexo','N','Usuário poderá Visualizar os Anexos de Venda,Cliente e Prospect')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','264','Adicionar Anexo','N','Usuário poderá Adicionar os Anexos de Venda,Cliente e Prospect')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','265','Deletar Anexo','N','Usuário poderá Deletar os Anexos de Venda,Cliente e Prospect')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','266','Editar Supervisor e Departamento','N','Usuário poderá Editar Supervisor e Departamento')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','267','Visualizar Perfil do Vendedor','N','Usuário poderá EVisualizar Perfil do Vendedor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','268','Alterar Perfil do Vendedor','N','Usuário poderá Alterar Perfil do Vendedor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','269','Cadastrar Perfil do Vendedor','N','Usuário poderá Cadastrar Perfil do Vendedor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','270','Autorizar Venda','N','Usuário poderá Autorizar Venda no Painel de Vendas')
                              MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','271','Vincular Venda a Atendimentos','N','Usuário poderá Vincular Venda a Atendimentos Existentes')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','272','Planejamento Roteirização','N','Painel de Planejamento Roteirização')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','273','Painel de Gestao Operacional','N','Painel de Gestao Operacional')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','274','Relatório de Quilometragem Percorrida','N','Relatório de Quilometragem Percorrida')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','275','Vincular Atendimento a Vendas Existentes','N','Usuário poderá Vincular Atendimento a Vendas Existentes')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '280', 'Relatório de Saldo de Materiais Aplic. no Cliente', 'N', 'Exibir Relatório de Saldo de Materiais Aplicados no Cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES('CONNECT','282','Visualizar/Enviar Mensagem Tecnicos ChatBackOffice','N','Usuário poderá visualizar e enviar mensagens BackOffice')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','283','Paralisar Atendimento do Servico','N','Paralisar Atendimento do Servico')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','284','Interromper Deslocamento do Servico','N','Interromper Deslocamento do Servico')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','285','Desvincular Técnico','N','Desvincular Técnico')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','286','Excluir Serviço','N','Excluir Serviço no painel de serviços')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','287','Visualizar a agenda completa ','N','Visualizar a agenda completa ignorando tempo mínimo configurado')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','288','Visualizar todas conversas no Painel de Conversas','N','Visualizar todas conversas no Painel de Conversas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','291','Relatório de Produtividade por Técnico','N','Relatório de Produtividade por Técnico')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','293','Visualizar Geography','N','Visualizar Geography')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','294','Importar OS','N','Importar OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','295','Excluir OS','N','Excluir OS na gestão de técnicos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','297','Relatório da situação de DIDs','N','Relatório da situação de DIDs')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','298','Relátório do Histórico de Chamadas Realizadas','N','Relátório do Histórico de Chamadas Realizadas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','299','Painel Avaliação de Comportamento e Performance','N','Painel de Avaliação de Comportamento e Performance')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','303','Painel de Planejamento de Capacidade','N','Painel de Planejamento de Capacidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','304','Log de Integração de Ordens de Serviço','N','Log de Integração de Ordens de Serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','305','Agendar e Roteirizar','N','Agendar e Roteirizar')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','308','Parâmetros de Visualização de Gestão de Técnicos','N','Acesso a Listagem e Cadastro dos Parâmetros de Visualização de Gestão de Técnicos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','309','Baixar OS','N','Permite dar Baixa nas Ordens de Serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','310','Reabrir OS','N','Permite Reabrir Ordens de Serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','311','Cadastro de OS Manutenção','N','Exibe e permite cadastrar OS manutenção')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','312','Editar SLA Acompanhamento de Serviço','N','Editar SLA Acompanhamento de Serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','314','Alterar Forma de Pagamento','N','Alterar Forma de Pagamento')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','315','Adicionar Materiais nas OS','N','Permite adicionar materiais nas abas de materiais aplicados e retirados no acompanhamento de serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','316','Excluir Materiais nas OS','N','Permite excluir materiais nas abas de materiais aplicados e retirados no acompanhamento de serviço')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','317','Painel de Diagnósticos','N','Acesso ao Painel de Diagnósticos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','318','Alteração/Cadastro Configurações do Serviço OLT','N','Cadastrar/Alterar Configurações do Serviço OLT')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','319','Ativar/Inativar Configurações do Serviço OLT','N','Ativar/Inativar Configurações do Serviço OLT')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','321','Preencher dados cartão manualmente','N','Preenchimento de dados do cartão manualmente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','322','Vizualizar Resultado Consulta de Restricoes','N','Vizualizar Resultado Consulta de Restricoes')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','323','Definir a qtd. de parcelas de instalacao na venda','N','Definir a quantidade de parcelas de instalacao na venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','324','Visualizar Segmentos','N','Permite Visualizar Segmentos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','325','Cadastrar Segmentos','N','Permite Cadastrar Segmentos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','326','Alterar Segmentos','N','Permite alterar segmento')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','327','Inativar / Ativar Segmentos','N','Inativar / Ativar Segmentos')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','328','Visualizar Motivo de Ausencia','N','O usuário poderá visualizar os motivos cadastrados')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','329','Cadastrar Motivo de Ausencia','N','Permite cadastrar um motivo de ausência.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','330','Ativar ou Desativar Motivo Ausencia','N','O Usuário poderá Ativar ou Desativar Motivos de Ausencia')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','331','Alterar Motivo de Ausencia','N','O Usuário poderá alterar Motivos de Ausencia já criado')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','332','Alterar Grupo Área, Área e Localidade na OS','N','Permite alterar as informações nos dados gerais da OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','333','Visualizar todos os leads na listagem de leads','N','Visualizar todos os leads na listagem de leads')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','334','Acessar cadastro de endereço pelo lead','N','Visualizar botão para entrar no cadastro de endereço pelo cadastro de lead')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','336','Aprovar/Reprovar etapa do workflow de vendas','N','Aprovar/Reprovar etapa do workflow de vendas')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','337','Editar dados do plano na venda','N','Editar dados do plano na venda')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','338','Aprovar consulta de viabilidade manualmente','N','Aprovar consulta de viabilidade manualmente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '339', 'Visualizar Perfil de Agendamento/Roteirização', 'N', 'Visualizar Perfil de Agendamento/Roteirização')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '340', 'Visualizar Vincular Técnico Perfil de Agendamento', 'N', 'Visualizar Vincular Técnico ao Perfil de Agendamento/Roteirização')
                            MATCHING (SIGLA_MODULO, COD_AREA);

UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','341','Aprovar pendência financeira manualmente','N','Aprovar pendência financeira manualmente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','342','Aprovar pendência de endereço manualmente','N','Aprovar pendência de endereço manualmente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '344', 'Enviar Termo de Adesão Manualmente', 'N', 'Enviar Termo de Adesão Manualmente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '345', 'Visualizar status das consultas de restrição', 'N', 'Visualizar status das consultas de restrição')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '346', 'Visualizar parametros da consulta de crédito', 'N', 'Visualizar parametros da consulta de crédito')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '347', 'Visualizar cadastro de checklist', 'N', 'Visualizar cadastro de checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '348', 'Permitir venda duplicada', 'N', 'Permite criar venda com outras em aberto')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '349', 'Permitir copiar link do Termo de Adesão', 'N', 'Permite copiar link do Termo de Adesão')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '350', 'Enviar checklist para o cliente', 'N', 'Enviar checklist para o cliente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '351', 'Configurar Param. de Visu. de Gestão de Vendas', 'N', 'Configurar Param. de Visu. de Gestão de Vendas')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '352', 'Visualizar Painel de Ac. das Etapas de Venda', 'N', 'Visualizar Painel de Ac. das Etapas de Venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '355', 'Configurar parâmetros PreQual', 'N', 'Configurar parâmetros PreQual')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '356', 'Visualizar Motivo de Reagendamento', 'N', 'Visualizar Motivo de Reagendamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '357', 'Cadastrar Motivo de Reagendamento', 'N', 'Cadastrar Motivo de Reagendamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '358', 'Alterar Motivo de Reagendamento', 'N', 'Alterar Motivo de Reagendamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '359', 'Ativar/Inativar Motivo Reagendamento', 'N', 'Ativar/Inativar Motivo Reagendamento')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '360', 'Filtro de data sem restrição no Painel de Serviços', 'N', 'Permissão para usar filtro de data sem restrição no Painel de Serviços')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','361','Aprovar consulta de credito manualmente','N','Aprovar consulta de credito manualmente')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','362','Visualização do Relatório de Checklist','N','Visualização do Relatório de Checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','363','Relatório Indicadores CRM','N','Visualizar Relatório Indicadores CRM')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','364','Permitir alterar endereço da OS','N','Permitir alterar endereço da OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','365','Importar OS via Integração','N','Importar OS via Integração')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','366','Adicionar/Remover Participantes OS','N','Adicionar/Remover Participantes OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','367','Relatorio Tecnicos Auxiliares na OS','N','Adicionar/Remover Participantes OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                            VALUES ('CONNECT', '368', 'Permitir visualizar listagem de produtos', 'N', 'Permitir visualizar listagem de produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                            VALUES ('CONNECT', '369', 'Permitir cadastrar produtos', 'N', 'Permitir cadastrar produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                            VALUES ('CONNECT', '370', 'Permitir ativar/inativar produtos', 'N', 'Permitir ativar/inativar produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                            VALUES ('CONNECT', '371', 'Permitir alterar produtos', 'N', 'Permitir alterar produtos')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                                VALUES ('CONNECT', '372', 'Relat Custos de Implementacao ', 'N', 'Relat Custos de Implementacao')
                                MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES('CONNECT','375','Visualizar Cadastro de Motivo de Paralisacao da OS','N','Visualizar Cadastro de Motivo de Paralisacao da OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','376','Cadastrar o Motivo de Paralisacao da OS','N','Cadastrar o Motivo de Paralisacao da OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','377','Alterar o Motivo de Paralisacao da OS','N','Alterar o Motivo de Paralisacao da OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','378','Painel de Acompanhamento de Vendedores','N','Acessar Painel de Acompanhamento de Vendedores')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','379','Consulta de Viabilidade','N','Consulta Viabilidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','380','Agenda Vendedor/Supoervisor','N','Adicionar/Remover Agenda Vendedor/Supoervisor')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','381','Importar Vinculos de Perfil de Ativacao','N','Importar Vinculos de Perfil de Ativacao')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','382','Legenda Timeline Tipo de Servico', 'N', 'Legenda Timeline Tipo de Servico')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','383','Alterar Cor Timeline Tipo de Serv', 'N', 'Alterar Cor Timeline Tipo de Serv')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','384','Visualizar Motivo de Suspensão Contratual','N','Visualizar Motivo de Suspensão Contratual')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','385','Cadastrar o Motivo de Suspensão Contratual','N','Cadastrar o Motivo de Suspensão Contratual')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','386','Alterar o Motivo de Suspensão Contratual','N','Alterar o Motivo de Suspensão Contratual')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','387','Suspender Plano/Produto e Contrato','N','Suspender Plano/Produto e Contrato')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','388','Cancelamento do Contrato','N','Cancelamento do Contrato')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','362','Visualização do Relatório de Checklist','N','Visualização do Relatório de Checklist')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','389','Visualizar Motivo de Cancelamento de OS','N','Visualizar Cadastro de Motivo de Cancelamento de OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','390','Cadastrar  Motivo de Cancelamento de OS','N','Cadastrar Motivo de Cancelamento de OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','391','Alterar  Motivo de Cancelamento de OS','N','Alterar Motivo de Cancelamento de OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','392','Autorizar Baixa de OS no Aniel Touch','N','Autorizar Baixa de OS no Aniel Touch')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','393','Relatório de Reagendamento de OS','N','Visualizar Relat. Reag. de OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','396','Aprovação/Reprovação de area termo de adesão','N','Aprovação/Reprovação de area termo de adesão')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','395','Relatório Materiais Aplicados no Inventário POP','N','Relatório Materiais Aplicados no POP')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','394','Resetar Assinatura Funcionario','N','Resetar Assinatura Funcionario')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','400','Resetar Assinatura Funcionario','N','Resetar Assinatura Funcionario')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','397','Cadastrar e Editar Modelos de Mensagem WhatsApp','N','Cadastrar e Editar Modelos de Mensagem WhatsApp')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','398','Relatorio Cliente Contrato','N','Relatorio Cliente Contrato')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','399','Excluir ONUs','N','Excluir registro de ONU do sistema')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','401','Troca de Titularidade','N','Troca de Titularidade')
							  MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','406','Histórico de Autenticações de Login Radius','N','Visualizar Painel de Histórico de Autenticações de Login Radius')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','402','Visualizar Troca Titularidade','N','Visualizar Troca Titularidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','403','Cadastrar Troca Titularidade','N','Cadastrar Troca Titularidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','409','Visualizar botão para validar documento no SERPRO','N','Visualizar botão para validar documento no SERPRO')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','410','Enviar requisição de consulta ao SERPRO','N','Enviar requisição de consulta ao SERPRO')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','411','Visualizar Anexo no Contrato','N','Definir visibilidade dos anexos no Contrato')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','404','Alterar Troca Titularidade','N','Alterar Troca Titularidade')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','405','Permitir cancelar titularidade em massa','N','Permitir cancelar titularidade em massa')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO)
                              VALUES('CONNECT','406','Visualizar Painel de Histórico de Autenticações de Login Radius','N','Visualizar Painel de Histórico de Autenticações de Login Radius')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','407','Visualizar botão de resetar ONU','N','Visualizar botão de resetar ONU')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','408','Configurar Áreas de Acesso do Olá Cliente','N','Definir visibilidade dos recursos no app Olá Cliente')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','409','Visualizar Alteracao de Plano','N','Permitir visualizar alteracao de plano')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','411','Acesso à tela de Composição Fiscal','N','Acesso à tela de Composição Fiscal')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','412','Vincular Atendimento','N','Vincular Atendimento')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','413','VISUALIZAR ALERTA MANU DE REDE','N','Permite ao usuário apenas visualizar as informações do alerta, sem permissão para inclusão ou edição de dados')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','414','ADICIONAR/ALTERAR ALERTA MANU DE REDE','N','Concede ao usuário permissão para adicionar e/ou editar as informações do alerta.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','413','Vincular OS','N','Vincular OS')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','415','VISUALIZAR CADASTRO CAMPO PERSONALIZADO','N','Permissão a usuários para visualização do recurso de cadastro de campo personalizado.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','416','CADASTRAR CAMPO PERSONALIZADO','N','Permite ao usuário realizar o cadastro de novos campos.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','417','ALTERAR CAMPO PERSONALIZADO','N','Permite ao usuário modificar os dados de campos existentes.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','418','ATIVAR/INATIVAR CAMPO PERSONALIZADO','N','Permite ao usuário ativar ou inativar qualquer campo personalizado.')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','419','Alterar Tipo Atividade Atendimento','N','Permitir alterar o tipo de atividade de Atendimento')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','420','Visualizar Informações do Contrato','N','Visualizar Informações do Contrato')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','422','Permitir copiar link do Checklist', 'N', 'Permite copiar link do Checklist')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','428','Permitir refazer consulta de crédito', 'N', 'Permitir refazer consulta de crédito')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','427','Cadastro Motivo de Aprovação de Restrição', 'N', 'Cadastro Motivo de Aprovação de Restrição')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO,COD_AREA,DESCRICAO,SUB_ACESSO,OBSERVACAO) 
                              VALUES('CONNECT','429','Visualizar Motivo de Aprovação de Restrição', 'N','Visualizar Motivo de Aprovação de Restrição')
                              MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '430', 'Inativar/Ativar Motivo de Aprovação de Restrição', 'N', 'Inativar/Ativar Motivo de Aprovação de Restrição')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '430', 'Inativar/Ativar Motivo de Aprovação de Restrição', 'N', 'Inativar/Ativar Motivo de Aprovação de Restrição')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '433', 'RELATORIO_VENDAS_MESMO_END/COND', 'N', 'RELATORIO_VENDAS_MESMO_END/COND')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '431', 'Permitir cancelar venda', 'N', 'Permitir cancelar venda')
                            MATCHING (SIGLA_MODULO, COD_AREA);
UPDATE OR INSERT INTO TB_AREA_MODULO (SIGLA_MODULO, COD_AREA, DESCRICAO, SUB_ACESSO, OBSERVACAO)
                              VALUES ('CONNECT', '432', 'Visualizar Relatório de Vendas Reprovadas por Score', 'N', 'Visualizar Relatório de Vendas Reprovadas por Score')
                            MATCHING (SIGLA_MODULO, COD_AREA);

").ToString();
        }
    }
}

