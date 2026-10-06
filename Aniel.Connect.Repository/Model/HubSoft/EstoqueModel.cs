using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.HubSoft
{

        public class Cest
        {
            public int id_cest { get; set; }
            public string codigo { get; set; }
            public string descricao { get; set; }
        }

        public class Ncm
        {
            public int id_ncm { get; set; }
            public string codigo { get; set; }
            public string descricao { get; set; }
        }

        public class ProdutoHubSoft
        {
            public int id_produto { get; set; }
            public string nome { get; set; }
            public object descricao { get; set; }
            public double valor_compra { get; set; }
            public double valor_venda { get; set; }
            public object codigo_ean { get; set; }
            public object codigo_ean_trib { get; set; }
            public string data_cadastro { get; set; }
            public Ncm ncm { get; set; }
            public object cst_origem { get; set; }
            public Cest cest { get; set; }
            public UnidadeMedida unidade_medida { get; set; }
        }

        public class ProdutoHubSoftRetorno
        {
            public string status { get; set; }
            public string msg { get; set; }
            public ProdutoHubSoft produto { get; set; }
        }

        public class UnidadeMedida
        {
            public int id_unidade_medida { get; set; }
            public string nome { get; set; }
            public string abreviacao { get; set; }
        }



        public class PatrimonioSaida
        {
            public int? id_produto_item { get; set; }
        }

        public class ProdutoSaida
        {
            public int quantidade { get; set; }
            public ProdutoId produto { get; set; }
            public List<PatrimonioSaida> patrimonios { get; set; }
            public ProdutoSaidaItemStatus produto_item_status { get; set; }
        }

        public class ProdutoSaidaItemStatus
        {
            public int id_produto_item_status { get; set; }
        }

        public class ProdutoId
        {
            public int? id_produto { get; set; }
        }

        public class SaidaMercadoriaHubSoft
        {
            public int id_local_estoque { get; set; }
            public List<ProdutoSaida> produtos { get; set; }
            public string observacao { get; set; }
        }




        public class DataConsultaEstoque
        {
            public int id_local_estoque { get; set; }
            public int id_empresa { get; set; }
            public string descricao { get; set; }
            public int id_endereco_numero { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public object deleted_at { get; set; }
            public bool ativo { get; set; }
            public bool fiscal { get; set; }
            public bool permite_vinculo_composicao { get; set; }
            public int id_local_estoque_reparticao_padrao { get; set; }
            public int local_estoque_reparticao_count { get; set; }
            public string endereco_numero_completo { get; set; }
            public string display { get; set; }
            public Empresa empresa { get; set; }
        }

        public class Empresa
        {
            public int id_empresa { get; set; }
            public string nome_razaosocial { get; set; }
            public string cnpj { get; set; }
            public string data_cadastro_br { get; set; }
            public object data_cadastro_timestamp { get; set; }
            public string data_abertura_br { get; set; }
            public object data_abertura_timestamp { get; set; }
            public string display { get; set; }
        }

        public class LocalEstoqueHubSoft
        {
            public int current_page { get; set; }
            public List<DataConsultaEstoque> data { get; set; }
            public string first_page_url { get; set; }
            public int from { get; set; }
            public int last_page { get; set; }
            public string last_page_url { get; set; }
            public object next_page_url { get; set; }
            public string path { get; set; }
            public int per_page { get; set; }
            public object prev_page_url { get; set; }
            public int to { get; set; }
            public int total { get; set; }
        }

        public class ConsultaEstoques
        {
            public string status { get; set; }
            public string msg { get; set; }
            public LocalEstoqueHubSoft locais_estoque { get; set; }
        }

        public class TransferenciaEstoqueHubSoft
        {
            public int id_local_estoque_origem { get; set; }
            public int id_local_estoque_destino { get; set; }
            public List<ProdutoTransferencia> produtos { get; set; }
        }

        public class ProdutoTransferencia
        {
            public int quantidade { get; set; }
            public ProdutoTransf produto { get; set; }
            public List<PatrimonioTransf> patrimonios { get; set; }
        }

        public class ProdutoTransf
        {
            public int id_produto { get; set; }
        }

        public class PatrimonioTransf
        {
            public int id_produto_item { get; set; }
        }
    
}
