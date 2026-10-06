using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.Hubsoft.ClienteModel
{
    public class Cliente
    {
        public int id_cliente { get; set; }
        public int codigo_cliente { get; set; }
        public string nome_razaosocial { get; set; }
        public object nome_fantasia { get; set; }
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string telefone_primario { get; set; }
        public string telefone_secundario { get; set; }
        public object telefone_terciario { get; set; }
        public string email_principal { get; set; }
        public object email_secundario { get; set; }
        public string rg { get; set; }
        public object rg_emissao { get; set; }
        public object inscricao_municipal { get; set; }
        public object inscricao_estadual { get; set; }
        public string data_cadastro { get; set; }
        public object data_nascmento { get; set; }
        public object nome_pai { get; set; }
        public object nome_mae { get; set; }
        public object estado_civil { get; set; }
        public object genero { get; set; }
        public string nacionalidade { get; set; }
        public object profissao { get; set; }
        public List<Servico> servicos { get; set; }
    }

    public class Paginacao
    {
        public int primeira_pagina { get; set; }
        public int ultima_pagina { get; set; }
        public int pagina_atual { get; set; }
        public int total_registros { get; set; }
    }

    public class ResponseConsultaCliente
    {
        public string status { get; set; }
        public string msg { get; set; }
        public Paginacao paginacao { get; set; }
        public List<Cliente> clientes { get; set; }
    }

    public class Coordenadas
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class EnderecoCadastral
    {
        public string completo { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public object complemento { get; set; }
        public object referencia { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public string cidade { get; set; }
        public Coordenadas coordenadas { get; set; }
    }

    public class EnderecoCobranca
    {
        public string completo { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public object complemento { get; set; }
        public object referencia { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public string cidade { get; set; }
        public Coordenadas coordenadas { get; set; }
    }

    public class EnderecoFiscal
    {
        public string completo { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public object complemento { get; set; }
        public object referencia { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public string cidade { get; set; }
        public Coordenadas coordenadas { get; set; }
    }

    public class EnderecoInstalacao
    {
        public string completo { get; set; }
        public string endereco { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string referencia { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string estado { get; set; }
        public string uf { get; set; }
        public string cidade { get; set; }
        public Coordenadas coordenadas { get; set; }
    }

    public class EquipamentoConexao
    {
        public string nome { get; set; }
        public string ipv4 { get; set; }
        public object ipv6 { get; set; }
    }

    public class EquipamentoRoteamento
    {
        public string nome { get; set; }
        public string ipv4 { get; set; }
        public object ipv6 { get; set; }
    }

    public class Grupo
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public string data_cadastro { get; set; }
        public bool ativo { get; set; }
        public string display { get; set; }
    }

    public class Interface
    {
        public string nome { get; set; }
        public string tipo { get; set; }
        public object called_sid { get; set; }
    }

    public class InterfaceRoteamento
    {
        public string nome { get; set; }
        public string tipo { get; set; }
        public object called_sid { get; set; }
    }

    public class Servico
    {
        public int id_cliente_servico { get; set; }
        public int id_servico { get; set; }
        public int numero_plano { get; set; }
        public string nome { get; set; }
        public double valor { get; set; }
        public string status { get; set; }
        public string status_prefixo { get; set; }
        public object data_habilitacao { get; set; }
        public string tecnologia { get; set; }
        public string velocidade_download { get; set; }
        public string velocidade_upload { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public object referencia { get; set; }
        public object ipv4 { get; set; }
        public object ipv6 { get; set; }
        public string mac_addr { get; set; }
        public string phy_addr { get; set; }
        public List<object> ultima_conexao { get; set; }
        public object motivo_cancelamento { get; set; }
        public object motivo_cancelamento_prefixo { get; set; }
        public object anotacoes { get; set; }
        public List<Grupo> grupos { get; set; }
        public object porta_atendimento { get; set; }
        public Interface @interface { get; set; }
        public InterfaceRoteamento interface_roteamento { get; set; }
        public EquipamentoConexao equipamento_conexao { get; set; }
        public EquipamentoRoteamento equipamento_roteamento { get; set; }
        public List<object> pacotes { get; set; }
        public List<object> contratos { get; set; }
        public object contratos_sem_assinatura { get; set; }
        public object contratos_pendentes { get; set; }
        public List<object> alarmes_conexoes { get; set; }
        public List<object> senhas { get; set; }
        public EnderecoInstalacao endereco_instalacao { get; set; }
        public EnderecoCadastral endereco_cadastral { get; set; }
        public EnderecoCobranca endereco_cobranca { get; set; }
        public EnderecoFiscal endereco_fiscal { get; set; }
    }
}
