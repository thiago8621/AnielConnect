using System.Collections.Generic;

namespace HubSoftService.Model.HubSoft.Entrada
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Fornecedor
    {
        public int id_fornecedor { get; set; }
    }

    public class LocalEstoque
    {
        public int id_local_estoque { get; set; }
    }

    public class Movimento
    {
        public string data_movimento { get; set; }
        public string tipo { get; set; }
        public string tipo_destino { get; set; }
        public List<Produto> produtos { get; set; }
        public LocalEstoque local_estoque { get; set; }
        public Fornecedor fornecedor { get; set; }
        public UsuarioResponsavel usuario_responsavel { get; set; }
        public StatusCompra status_compra { get; set; }
        public string data_compra { get; set; }
        public double valor_total_compra { get; set; }
    }

    public class Produto
    {
        public Produto2 produto { get; set; }
        public double quantidade { get; set; }
        public double valor_unitario { get; set; }
        public double valor_total_produto { get; set; }
        public bool utilizar_casa_decimal_valor_unitario { get; set; }
    }

    public class Produto2
    {
        public int id_produto { get; set; }
    }

    public class CompraEstoqueModel
    {
        public List<Movimento> movimentos { get; set; }
    }

    public class StatusCompra
    {
        public int id_status_compra { get; set; }
    }

    public class UsuarioResponsavel
    {
        public int id { get; set; }
    }
}

namespace HubSoftService.Model.HubSoft.Entrada.Retorno
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Movimento
    {
        public string codigo_movimento { get; set; }
        public string tipo { get; set; }
        public int id_tipo_movimento_estoque { get; set; }
        public int id_usuario_cadastro { get; set; }
        public int id_local_estoque { get; set; }
        public string vinculo_origem { get; set; }
        public string vinculo_destino { get; set; }
        public int id_vinculo_origem { get; set; }
        public int id_vinculo_destino { get; set; }
        public object id_ordem_servico { get; set; }
        public object observacao { get; set; }
        public double valor_total { get; set; }
        public bool emitir_nfe { get; set; }
        public string data_movimento { get; set; }
        public string data_cadastro { get; set; }
        public string data_atualizacao { get; set; }
        public string plataforma { get; set; }
        public bool contabilizar { get; set; }
        public int id_movimento_estoque { get; set; }
        public string data_movimento_br { get; set; }
        public string hora_movimento_br { get; set; }
        public string data_cadastro_br { get; set; }
        public string data_atualizacao_br { get; set; }
        public List<MovimentoEstoqueProduto> movimento_estoque_produto { get; set; }
    }

    public class MovimentoEstoqueProduto
    {
        public int id_movimento_estoque_produto { get; set; }
        public int id_movimento_estoque { get; set; }
        public int id_produto { get; set; }
        public int quantidade { get; set; }
        public string valor { get; set; }
        public object observacao { get; set; }
        public string valor_unitario { get; set; }
        public object tipo_utilizacao { get; set; }
        public int id_local_estoque_reparticao { get; set; }
    }

    public class RetornoCriacaoEntrada
    {
        public string status { get; set; }
        public string msg { get; set; }
        public Movimento movimento { get; set; }
    }
}

namespace HubSoftService.Model.HubSoft.Entrada.Consulta
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Datum
    {
        public int id_produto_item { get; set; }
        public int id_produto { get; set; }
        public int id_local_estoque { get; set; }
        public int id_produto_compra { get; set; }
        public object numero_serie { get; set; }
        public int codigo_item { get; set; }
        public object deleted_at { get; set; }
        public string mac_address { get; set; }
        public int quantidade { get; set; }
        public object observacoes { get; set; }
        public object id_produto_item_pai { get; set; }
        public object motivo_deletado { get; set; }
        public object identificador_proprio { get; set; }
        public object data_limite { get; set; }
        public int id_produto_item_status { get; set; }
        public object data_validade { get; set; }
        public object epi_ca { get; set; }
        public object id_vinculo { get; set; }
        public object tipo_vinculo { get; set; }
        public bool recondicionado { get; set; }
        public string valor_compra { get; set; }
        public int id_local_estoque_reparticao { get; set; }
        public string descricao_vinculo { get; set; }
        public bool permite_alterar_status { get; set; }
        public object data_validade_br { get; set; }
        public string display { get; set; }
        public object data_limite_br { get; set; }
        public string status { get; set; }
        public LocalEstoque local_estoque { get; set; }
        public LocalEstoqueReparticao local_estoque_reparticao { get; set; }
        public List<object> anexos { get; set; }
        public ProdutoCompra produto_compra { get; set; }
        public Produto produto { get; set; }
        public ProdutoItemStatus produto_item_status { get; set; }
    }

    public class LocalEstoque
    {
        public int id_local_estoque { get; set; }
        public string descricao { get; set; }
        public object deleted_at { get; set; }
        public string display { get; set; }
    }

    public class LocalEstoqueReparticao
    {
        public int id_local_estoque_reparticao { get; set; }
        public string nome { get; set; }
        public object deleted_at { get; set; }
        public string display { get; set; }
    }

    public class Produto
    {
        public int id_produto { get; set; }
        public string nome { get; set; }
        public int id_unidade_medida { get; set; }
        public double valor_venda { get; set; }
        public double valor_compra { get; set; }
        public bool controle_patrimonial { get; set; }
        public bool epi { get; set; }
        public object deleted_at { get; set; }
        public string display { get; set; }
        public string tipo_display { get; set; }
        public UnidadeMedida unidade_medida { get; set; }
        public List<object> imagem_padrao { get; set; }
    }

    public class ProdutoCompra
    {
        public int id_produto_compra { get; set; }
        public int codigo_compra { get; set; }
    }

    public class ProdutoItemStatus
    {
        public int id_produto_item_status { get; set; }
        public string descricao { get; set; }
        public string prefixo { get; set; }
        public string cor { get; set; }
        public bool padrao_sistema { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public object deleted_at { get; set; }
        public string display { get; set; }
    }

    public class ProdutosItem
    {
        public int current_page { get; set; }
        public List<Datum> data { get; set; }
        public string first_page_url { get; set; }
        public int from { get; set; }
        public int last_page { get; set; }
        public string last_page_url { get; set; }
        public object next_page_url { get; set; }
        public string path { get; set; }
        public string per_page { get; set; }
        public object prev_page_url { get; set; }
        public int to { get; set; }
        public int total { get; set; }
    }

    public class RetornoConsultaEntrada
    {
        public string status { get; set; }
        public string msg { get; set; }
        public ProdutosItem produtos_item { get; set; }
    }

    public class UnidadeMedida
    {
        public int id_unidade_medida { get; set; }
        public string nome { get; set; }
        public string abreviacao { get; set; }
        public string display { get; set; }
    }
}

namespace HubSoftService.Model.HubSoft.Entrada.CadastroSerial
{
    public class Item
    {
        public int id_produto_item { get; set; }
        public object identificador_proprio { get; set; }
        public string numero_serie { get; set; }
        public string mac_address { get; set; }
        public object epi_ca { get; set; }
        public object data_validade { get; set; }
    }

    public class CadastrarSerialModel
    {
        public List<Item> itens { get; set; }
    }
}