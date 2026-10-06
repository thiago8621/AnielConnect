using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.Azza
{
    
    public class Equipe
    {
        public String Matricula { get; set; }
        public String CodAux { get; set; }
        public String Nome { get; set; }
        public String CodCtPadrao { get; set; }
        public String ProjetoPadrao { get; set; }

        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
    }

    
    public class Ordem_Servico
    {
        /// <summary>
        /// Contrato Aniel
        /// </summary>
        public String CodCt { get; set; }

        /// <summary>
        /// Projeto Aniel
        /// </summary>
        public String Projeto { get; set; }

        /// <summary>
        /// Contrato Cliente
        /// </summary>
        public Int32? Contrato { get; set; }

        /// <summary>
        /// Contrato Kit
        /// </summary>
        public String Contrato_Kit { get; set; }

        /// <summary>
        /// Número OS
        /// </summary>
        public String OS { get; set; }

        /// <summary>
        /// Unidade de Custo
        /// </summary>
        public String Unidade_Custo { get; set; }

        /// <summary>
        /// Tipo Produção
        /// </summary>
        public Enum_Tipo_Producao Tipo_Producao { get; set; }

        /// <summary>
        /// Número da Tarefa ou Terminal
        /// </summary>
        public String Tarefa { get; set; }

        /// <summary>
        /// Data Agendamento
        /// </summary>
        public DateTime? DataAgendamento { get; set; }

        /// <summary>
        /// Hora Agendamento
        /// </summary>
        public DateTime? HoraAgendamento { get; set; }
        /// <summary>
        /// Data Agendamento
        /// </summary>
        public DateTime? DataAbertura { get; set; }

        /// <summary>
        /// Hora Agendamento
        /// </summary>
        public DateTime? HoraAbertura { get; set; }

        /// <summary>
        /// Cliente da OS
        /// </summary>
        public String Cliente { get; set; }

        /// <summary>
        /// Id Cliente do Connect
        /// </summary>
        public Int32 Id_Cliente { get; set; }

        /// <summary>
        /// Id Contrato do Connect
        /// </summary>
        public Int64? Id_Cliente_Contrato { get; set; }

        /// <summary>
        /// Código Encerramento 1- Improdutiva / 2- Produtiva
        /// </summary>
        public Int16? Status_Execucao { get; set; }
        public Int16? Status_Execucao_Servico { get; set; }

        /// <summary>
        /// Matrícula da Equipe Executora da OS
        /// </summary>
        public String Equipe { get; set; }

        /// <summary>
        /// Matrícula da Equipe no Integrator
        /// </summary>
        public String EquipeAux { get; set; }

        /// <summary>
        /// Endereço
        /// </summary>
        public String Endereco { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        public String Cidade { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        public String Bairro { get; set; }
        /// <summary>
        /// Cep
        /// </summary>
        public String Cep { get; set; }
        /// <summary>
        /// Numero
        /// </summary>
        public String Numero { get; set; }
        /// <summary>
        /// Complemento
        /// </summary>
        public String Complemento { get; set; }
        /// <summary>
        /// Uf
        /// </summary>
        public String Uf { get; set; }

        /// <summary>
        /// Código do Tipo de Serviço
        /// </summary>       
        public Int16? Cod_Tipo_Serv { get; set; }

        /// <summary>
        /// Código do SubTipo de Serviço
        /// </summary>       
        public String Cod_SubTipo_Serv { get; set; }

        /// <summary>
        /// Descrição do Tipo de Serviço
        /// </summary>
        public String Desc_Tipo_Serv { get; set; }

        /// <summary>
        /// Código de Encerramento (Obrigado informar Cod_Tipo_Serv)
        /// </summary>
        public String Cod_Encerramento { get; set; }
        public String Cod_Encerramento_Auxiliar { get; set; }
        public String Cod_Encerramento_Auxiliar2 { get; set; }

        public String Desc_Cod_Encerramento { get; set; }

        /// <summary>
        /// Data Encerramento
        /// </summary>
        public DateTime? DataEncerramento { get; set; }


        /// <summary>
        /// Hora Encerramento
        /// </summary>
        public DateTime? HoraEncerramento { get; set; }

        /// <summary>
        /// Data Hora Inicio Atendimento
        /// </summary>
        public DateTime? DataHoraInicioAtendimento { get; set; }


        /// <summary>
        /// Observação
        /// </summary>
        public String Observacao { get; set; }

        /// <summary>
        /// Observação
        /// </summary>
        public String ObservacaoTeste { get; set; }

        /// <summary>
        /// Código de Tipo Serviço Equipe
        /// </summary>
        public String Cod_TipoServ_Integrator { get; set; }

        /// <summary>
        /// Sequencial
        /// </summary>
        public Int32? Seq { get; set; }

        /// <summary>
        /// Número da OS no Integrator
        /// </summary>
        public String OSIntegrator { get; set; }

        /// <summary>
        /// Usuário Equipe
        /// </summary>
        public String Usuario { get; set; }

        /// <summary>
        /// Matricula Funcionario
        /// </summary>
        public String Num_Fre { get; set; }

        public String LoginRadius { get; set; }
        public String SenhaRadius { get; set; }
        public String Telefone { get; set; }
        public String Telefone_Comercial { get; set; }
        public String Email_Assinante { get; set; }
        public String Rg { get; set; }
        public String CpfCliente { get; set; }
        public String Oxbdpee { get; set; }
        public String Titular { get; set; }
        public String CodPlano { get; set; }
        public String DescPlano { get; set; }

        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public int? Cod_Localidade { get; set; }
        public Int64? Cod_Tipo_Imovel { get; set; }

        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public DateTime? Agendamento_Inicio { get; set; }
        public DateTime? Agendamento_Fim { get; set; }
        public int? Cod_Turno { get; set; }
        /// <summary>
        /// Código Encerramento - ENCERRA_OS_APOS_BAIXA
        /// </summary>
        public String Encerra_OS_Apos_Baixa { get; set; }
        public String Referencia { get; set; }
        /// <summary>
        /// Id Login
        /// </summary>
        public Int64? Id_Login { get; set; }
        public Int64? Id_Caixa { get; set; }
        public Int64? Id_Porta { get; set; }
        public String IdOriginalIxc { get; set; }
        public Int64? Id_Agendamento { get; set; }
    }

    public class Tipo_Servico
    {
        /// <summary>
        /// Código
        /// </summary>
        public Int32? Codigo { get; set; }

        /// <summary>
        /// Tipo Servico
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Código do IXC
        /// </summary>
        public String Codigo_Integrator { get; set; }
    }

    public class SubTipo_Servico
    {
        /// <summary>
        /// Código
        /// </summary>
        public String CodigoSubTipo { get; set; }

        /// <summary>
        /// Tipo Servico
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Código
        /// </summary>
        public Int32? CodigoTipoServ { get; set; }
    }

    public class ProjetoIxc
    {
        /// <summary>
        /// Código
        /// </summary>
        public String Codigo { get; set; }

        /// <summary>
        /// Codigo Escritorio
        /// </summary>
        public String CodEscritorio { get; set; }

    }

    public class LocalidadeIxc
    {
        public Int32? Cod_Cliente { get; set; }
        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public String LocalidadeStr { get; set; }
        public String DescricaoArea { get; set; }
        public Int32? Cod_Localidade { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
    }

    public class EnderecoClienteIxc
    {
        public long Id { get; set; }
        public String Numero { get; set; }
        public String Cep { get; set; }
        public String Cidade { get; set; }
        public String Bairro { get; set; }
        public String Uf { get; set; }
        public String Complemento { get; set; }
        public String Referencia { get; set; }
        public String Logradouro { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public int? CodLocalidade { get; set; }
        public string Localidade { get; set; }
    }

    public class AreaIxc
    {
        public Int32? Cod_Cliente { get; set; }
        public String Cod_Escritorio { get; set; }
        public String Cod_Area { get; set; }
        public String Descricao { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
    }

    
    public class InfoCliente
    {
        public string codPlano { get; set; }
        public string rgCliente { get; set; }
        public ClienteIxc cliente { get; set; }
    }

    

    public class ClienteIxc
    {
        public Int32? Codigo { get; set; }
        public String Razao_Social { get; set; }
        public String CNPJ { get; set; }
        public String TipoCliente { get; set; }
        public Int16? Situacao { get; set; } // Situação: 0 = Ativo, 1 = Inativo
        public String Email { get; set; }

        public String Endereco { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }
        public String Bairro { get; set; }
        public String Municipio { get; set; }
        public Int16? CodMunicipio { get; set; }
        public String CEP { get; set; }
        public String UF { get; set; }

        public String Telefone { get; set; }
        public String Telefone_Comercial { get; set; }
        public String Telefone_Residencial { get; set; }
        public String Referencia { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
    }

    public enum Enum_Tipo_Producao
    {
        /// <summary>
        /// Atividade
        /// </summary>
        AT,
        /// <summary>
        /// Acesso
        /// </summary>
        AC
    }
}
