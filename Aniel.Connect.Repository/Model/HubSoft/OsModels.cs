using Aniel.Connect.Repository.Model.Hubsoft.ClienteModel;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model.Hubsoft
{
    #region Model Consulta OS

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Atendimento
    {
        public int? id_atendimento { get; set; }
        public int? id_atendimento_status { get; set; }
        public int? id_tipo_atendimento { get; set; }
        public int? id_cliente_servico { get; set; }
        public object id_usuario_responsavel { get; set; }
        public string protocolo { get; set; }
        public string data_cadastro { get; set; }
        public object data_fechamento { get; set; }
        public bool rascunho { get; set; }
        public object deleted_at { get; set; }
        public string data_cadastro_br { get; set; }
        public long data_cadastro_timestamp { get; set; }
        public object data_fechamento_br { get; set; }
        public object data_fechamento_timestamp { get; set; }
        public int minutos_em_aberto { get; set; }
        public string tempo_restante { get; set; }
        public double? percentual_tempo_restante { get; set; }
        public string percentual_color_class { get; set; }
        public ClienteServico cliente_servico { get; set; }
    }

    public class Cidade
    {
        public int? id_cidade { get; set; }
        public int? id_estado { get; set; }
        public string nome { get; set; }
        public string ibge { get; set; }
        public string cep { get; set; }
        public bool ativo { get; set; }
        public object nome_nfse { get; set; }
        public Estado estado { get; set; }
    }

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
        public string inscricao_estadual { get; set; }
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

    public class ClienteServico
    {
        public int? id_cliente_servico { get; set; }
        public int id_cliente { get; set; }
        public int? numero_plano { get; set; }
        public object display { get; set; }
        public string endereco_numero_completo { get; set; }
        public Cliente cliente { get; set; }
        public EnderecoInstalacao endereco_instalacao { get; set; }
        public int? id_servico { get; set; }
        public int? id_servico_status { get; set; }
        public Servico servico { get; set; }
        public ServicoStatus servico_status { get; set; }
        public List<ClienteServicoEndereco> cliente_servico_endereco { get; set; }
    }

    public class ClienteServicoEndereco
    {
        public int? id_cliente_servico_endereco { get; set; }
        public int? id_endereco_numero { get; set; }
        public int? id_cliente_servico { get; set; }
        public string tipo { get; set; }
        public EnderecoNumero endereco_numero { get; set; }
    }

    public class Coordenadas
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class OrdemServicoHubSoft
    {
        public int? id_ordem_servico { get; set; }
        public int? id_tipo_ordem_servico { get; set; }
        public string descricao_abertura { get; set; }
        public string descricao_servico { get; set; }
        public object descricao_fechamento { get; set; }
        public string duracao { get; set; }
        public object gera_custo { get; set; }
        public object valor_custo { get; set; }
        public string numero_ordem_servico { get; set; }
        public object id_forma_cobranca { get; set; }
        public int? id_atendimento { get; set; }
        public int? id_usuario_abertura { get; set; }
        public object id_usuario_fechamento { get; set; }
        public string data_cadastro { get; set; }
        public string ip_cadastro { get; set; }
        public string status { get; set; }
        public object status_fechamento { get; set; }
        public bool valor_custo_processado { get; set; }
        public int id_cliente_servico { get; set; }
        public bool reservada { get; set; }
        public object assinatura { get; set; }
        public int? id_prioridade { get; set; }
        public string data_inicio_programado { get; set; }
        public string data_termino_programado { get; set; }
        public object data_inicio_executado { get; set; }
        public object data_termino_executado { get; set; }
        public string id_agenda_ordem_servico { get; set; }
        public long? id_pop { get; set; }
        public object id_prospecto { get; set; }
        public bool permite_modificar_horario_os { get; set; }
        public int? checklists_count { get; set; }
        public int? motivo_remocao_agendamento_count { get; set; }
        public int? mensagens_count { get; set; }
        public string data_cadastro_br { get; set; }
        public long? data_cadastro_timestamp { get; set; }
        public string data_inicio_programado_br { get; set; }
        public long? data_inicio_programado_timestamp { get; set; }
        public object data_inicio_executado_br { get; set; }
        public object data_inicio_executado_timestamp { get; set; }
        public object data_termino_executado_br { get; set; }
        public object data_termino_executado_timestamp { get; set; }
        public string data_termino_programado_br { get; set; }
        public long? data_termino_programado_timestamp { get; set; }
        public string hora_inicio_programado { get; set; }
        public string hora_termino_programado { get; set; }
        public object hora_inicio_executado { get; set; }
        public object hora_termino_executado { get; set; }
        public string disponibilidade_str { get; set; }
        public int? minutos_em_aberto { get; set; }
        public string tempo_restante { get; set; }
        public double? percentual_tempo_restante { get; set; }
        public string destino { get; set; }
        public bool executando { get; set; }
        public string inicio_execucao { get; set; }
        public List<object> equipamentos { get; set; }
        public object ordem_servico_assinatura { get; set; }
        public List<OrdemServicoTecnico> ordem_servico_tecnico { get; set; }
        public List<Tecnico> tecnicos { get; set; }
        public TipoOrdemServico tipo_ordem_servico { get; set; }
        public Atendimento atendimento { get; set; }
        public List<object> insumos { get; set; }
        public ClienteServico cliente_servico { get; set; }
        public UsuarioAbertura usuario_abertura { get; set; }
        public object usuario_fechamento { get; set; }
        public Prioridade prioridade { get; set; }
        public List<Disponibilidade> disponibilidade { get; set; }
        public List<object> anexos { get; set; }
        public Agenda agenda { get; set; }
        public List<object> reservas { get; set; }
        public List<object> motivos_fechamento { get; set; }
        public object agendamento_ordem_servico { get; set; }
        public List<object> servicos_cobrados { get; set; }
        public object pop { get; set; }
        public object prospecto { get; set; }
    }


    public class Disponibilidade
    {
        public int? id_periodo_dia { get; set; }
        public string descricao { get; set; }
        public string prefixo { get; set; }
        public bool padrao_sistema { get; set; }
        public string hora_inicio { get; set; }
        public string hora_fim { get; set; }
        public string display { get; set; }
        public string inicial { get; set; }
        public Pivot pivot { get; set; }
    }

    

    public class EnderecoNumero
    {
        public int? id_endereco_numero { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public Coordenadas coordenadas { get; set; }
        public bool ativo { get; set; }
        public int? id_cidade { get; set; }
        public string bairro { get; set; }
        public object referencia { get; set; }
        public string endereco { get; set; }
        public string cep { get; set; }
        public bool atualizar_coords_auto { get; set; }
        public object id_condominio { get; set; }
        public Cidade cidade { get; set; }
    }

    public class Estado
    {
        public int? id_estado { get; set; }
        public string nome { get; set; }
        public string sigla { get; set; }
        public string ibge { get; set; }
        public int id_pais { get; set; }
        public bool ativo { get; set; }
    }

    public class Imagem
    {
        public int? id_imagem_upload { get; set; }
        public string link { get; set; }
        public string link_thumb { get; set; }
    }

    public class OrdemServicoTecnico
    {
        public int? id_ordem_servico_tecnico { get; set; }
        public int? id_ordem_servico { get; set; }
        public int? id_usuario { get; set; }
        public Usuario usuario { get; set; }
    }

    public class ResponseOS
    {
        public int current_page { get; set; }
        public List<OrdemServicoHubSoft> data { get; set; }
        public string first_page_url { get; set; }
        public int from { get; set; }
        public int last_page { get; set; }
        public string last_page_url { get; set; }
        public string next_page_url { get; set; }
        public string path { get; set; }
        public string per_page { get; set; }
        public object prev_page_url { get; set; }
        public int? to { get; set; }
        public int? total { get; set; }
    }

    public class Pivot
    {
        public int? id_ordem_servico { get; set; }
        public int? id_usuario { get; set; }
        public int? id_setor { get; set; }
        public int? id_periodo_dia { get; set; }
        public int? id_porta_atendimento { get; set; }
        public int? id_cliente_servico { get; set; }
        public int? id_caixa_optica { get; set; }
        public int? id_interface_conexao { get; set; }
    }

    public class Prioridade
    {
        public int? id_prioridade { get; set; }
        public string descricao { get; set; }
        public string prefixo { get; set; }
        public int peso { get; set; }
    }

    public class ResponseConsultaOrdemServico
    {
        public string status { get; set; }
        public string msg { get; set; }
        public ResponseOS ordens_servico { get; set; }
        public object cliente { get; set; }
    }

    public class Servico
    {
        public string descricao { get; set; }
        public int? id_cliente_servico { get; set; }
        public int? id_servico { get; set; }
        public int? numero_plano { get; set; }
        public string nome { get; set; }
        public double? valor { get; set; }
        public string status { get; set; }
        public string status_prefixo { get; set; }
        public object data_habilitacao { get; set; }
        public string tecnologia { get; set; }
        public string velocidade_download { get; set; }
        public string velocidade_upload { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
        public string referencia { get; set; }
        public string ipv4 { get; set; }
        public string ipv6 { get; set; }
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
        public EnderecoNumero endereco_numero { get; set; }
    }


    public class ServicoStatus
    {
        public int? id_servico_status { get; set; }
        public string descricao { get; set; }
    }

    public class Setore
    {
        public int id_setor { get; set; }
        public string descricao { get; set; }
        public string data_cadastro_br { get; set; }
        public Pivot pivot { get; set; }
    }

    public class Tecnico
    {
        public int? id { get; set; }
        public int? id_imagem_upload { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public object telefone_primario { get; set; }
        public string telefone_secundario { get; set; }
        public string display { get; set; }
        public Pivot pivot { get; set; }
        public Imagem imagem { get; set; }
        public List<Setore> setores { get; set; }
    }

    public class TipoOrdemServico
    {
        public int? id_tipo_ordem_servico { get; set; }
        public int? id_prioridade { get; set; }
        public int? quantidade_minima_descricao_fechamento { get; set; }
        public string descricao { get; set; }
        public int? prazo_execucao { get; set; }
        public string destino { get; set; }
        public string cor_pendente_agenda_tecnico { get; set; }
        public string cor_finalizado_agenda_tecnico { get; set; }
        public string cor_aguardando_agendamento_agenda_tecnico { get; set; }
        public string display { get; set; }
        public bool valor_obrigatorio { get; set; }
        public string destino_display { get; set; }
        public Prioridade prioridade { get; set; }
    }

    public class Usuario
    {
        public int? id { get; set; }
        public int? id_imagem_upload { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public object telefone_primario { get; set; }
        public string telefone_secundario { get; set; }
        public string display { get; set; }
        public Imagem imagem { get; set; }
    }

    public class UsuarioAbertura
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string display { get; set; }
    }



    #endregion

    public class Paginacao
    {
        public int? primeira_pagina { get; set; }
        public int? ultima_pagina { get; set; }
        public int? pagina_atual { get; set; }
        public int? total_registros { get; set; }
    }

    public class Agenda
    {
        public int? id_agenda_ordem_servico { get; set; }
        public string descricao { get; set; }
        public int? limite { get; set; }
        public bool ativo { get; set; }
        public string tipo_limite { get; set; }
        public bool respeita_feriado { get; set; }
        public string display { get; set; }
    }

    #region ConsultarOS api HubSoft
    //Endpoint de consulta de O.S Hubsoft
    //api/v1/integracao/ordem_servico/consultar

    public class AutenticacaoServico
    {
        public string login { get; set; }
        public string password { get; set; }
        public string mac_addr { get; set; }
        public string phy_addr { get; set; }
    }

    public class MotivoFechamento
    {
        public int? id_motivo_fechamento { get; set; }
        public string descricao { get; set; }
    }

    public class OrdensServicoConsultar
    {
        public string cliente { get; set; }
        public string servico { get; set; }
        public AutenticacaoServico autenticacao_servico { get; set; }
        public string endereco_instalacao { get; set; }
        public object pop { get; set; }
        public object prospecto { get; set; }
        public List<object> anexos { get; set; }
        public int? id_ordem_servico { get; set; }
        public string numero { get; set; }
        public string tipo { get; set; }
        public int? id_tipo_ordem_servico { get; set; }
        public string data_inicio_programado { get; set; }
        public string data_termino_programado { get; set; }
        public string data_inicio_executado { get; set; }
        public string data_termino_executado { get; set; }
        public string data_cadastro { get; set; }
        public string descricao_abertura { get; set; }
        public string descricao_servico { get; set; }
        public string descricao_fechamento { get; set; }
        public List<MotivoFechamento> motivo_fechamento { get; set; }
        public string status { get; set; }
        public string disponibilidade { get; set; }
        public int? id_cliente { get; set; }
        public int? id_cliente_servico { get; set; }
    }

    public class RetornoOrdemDeServicoConsultar
    {
        public string status { get; set; }
        public string msg { get; set; }
        public List<OrdensServicoConsultar> ordens_servico { get; set; }
    }

    public class ConsultaOrdemServicoDetalhada
    {
        public string status { get; set; }
        public string msg { get; set; }
        public OrdemServicoHubSoft ordem_servico { get; set; }
    }

    public class ConsultaIdOsHubsoft
    {
        public string adicional_1 { get; set; }
        public string id_hubsoft { get; set; }
        public string equipeaux { get; set; }
        public string nome { get; set; }
        public bool saldo_adicionado { get; set; }
        public string equipe {  get; set; }
        public string projeto {  get; set; }
        public string codct { get; set; }
        public string num_doc { get; set; }
        public DateTime? data_agendamento {  get; set; }
        public string cod_tipo_serv {  get; set; }
        public string tarefa { get; set; }
        public string id_cliente { get; set; }


    }

    public class ConsultaNumOs
    {
        public string projeto { get; set; }
        public string codct { get; set; }
        public string num_obra { get; set; }

    }

    public class Material_Aplicado
    {
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public String Num_Obra { get; set; }
        public Int32? Num_Doc { get; set; }
        public Int32? Status_Auditoria { get; set; }


        public Boolean Permitir_TI_Automatica { get; set; }
        public String CodigoTmp { get; set; }

        public String Tarefa { get; set; }
        public String Codigo { get; set; }
        public String CodigoAuxiliar { get; set; }

        public String Descricao { get; set; }
        public String Codigo_Complementar { get; set; }
        public String Unidade { get; set; }
        public String Equipe { get; set; }
        public String Valor_Unitario { get; set; }
        public String Quantidade { get; set; }
        public String Valor_Total { get; set; }
        public String Comodato { get; set; }

        public String Data_Aplic { get; set; }
        public Boolean Retirada { get; set; }
        public int? Id { get; set; }
        public string Estoque { get; set; }
    }


    #endregion
}

namespace Aniel.Connect.Repository.Model.AplicacaoMaterialHubsoft
{
    public class AplicacaoRemocaoMaterial
    {
        public long Id { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumObra { get; set; }
        public string Equipe { get; set; }
        public string Codmat { get; set; }
        public double Quantidade { get; set; }
        public int IdHubsoft { get; set; }
        public string Codcpl { get; set; }
        public long LocalEstoque { get; set; }
        public string TipoDoc { get; set; }
        public string ClassificacaoMaterial { get; set; }
        public string Importado { get; set; }
        public int Etapa { get; set; }
        public string DepositoTecnico { get; set; }
        /// <summary>
        /// usado apenas na api de material tarefa para trazer a descrição do produto
        /// </summary>
        public string DescricaoProduto { get; set; }
    }

    public class MovimentoProdutoUtilizadoFecharOS
    {
        public long? id_cliente_servico { get; set; }
        public int id_local_estoque { get; set; }
        public int id_tipo_movimento_estoque { get; set; }
        public List<ProdutoFecharOS> produtos { get; set; }
        public string observacao { get; set; }
    }

    public class MovimentoProdutoUtilizadoFecharOSPop
    {
        public long? id_pop { get; set; }
        public int id_local_estoque { get; set; }
        public List<ProdutoFecharOS> produtos { get; set; }
        public string observacao { get; set; }
    }

    public class PatrimonioFecharOS
    {
        public int id_produto_item { get; set; }
    }

    public class ProdutoFecharOS
    {
        public int id_produto { get; set; }
        public double quantidade { get; set; }
        public string campo_identificacao_patrimonio { get; set; }
        public List<PatrimonioFecharOS> patrimonios { get; set; }
    }

}

namespace Aniel.Connect.Repository.Model.RetiradaMaterialHubsoft
{
    public class RetiradaHubsoftRequest
    {
        public long id_cliente_servico { get; set; }
        public List<Produto> produtos { get; set; }
        public string observacao { get; set; }
    }

    public class Produto
    {
        public int id_local_estoque { get; set; }
        public int id_produto_item_status { get; set; }
        public int id_produto { get; set; }
        public double quantidade { get; set; }
        public string campo_identificacao_patrimonio { get; set; }
        public List<Patrimonio> patrimonios { get; set; }
    }

    public class Patrimonio
    {
        public int id_produto_item { get; set; }
    }
}