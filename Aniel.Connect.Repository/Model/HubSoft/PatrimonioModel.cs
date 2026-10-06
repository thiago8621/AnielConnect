using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Repository.Model.Hubsoft;

namespace Aniel.Connect.Repository.Model.HubSoft
{
    public class ClienteServico
    {
        public int id_cliente_servico { get; set; }
        public int numero_plano { get; set; }
        public string servico { get; set; }
        public string display { get; set; }
        public Hubsoft.ClienteModel.Cliente cliente { get; set; }
    }

    public class LocalEstoque
    {
        public int id_local_estoque { get; set; }
        public string descricao { get; set; }
    }

    public class Produto
    {
        public int id_produto_item { get; set; }
        public Produto produto { get; set; }
        public LocalEstoque local_estoque { get; set; }
        public ProdutoItemStatus produto_item_status { get; set; }
        public object identificador_proprio { get; set; }
        public int codigo_item { get; set; }
        public string numero_serie { get; set; }
        public string mac_address { get; set; }
        public object observacoes { get; set; }
        public ClienteServico cliente_servico { get; set; }
        public int id_produto { get; set; }
        public string nome { get; set; }
        public bool controle_patrimonial { get; set; }
        public bool epi { get; set; }
    }

    public class ProdutoItemStatus
    {
        public int id_produto_item_status { get; set; }
        public string descricao { get; set; }
        public string prefixo { get; set; }
    }

    public class RetornoBuscaPatrimonio
    {
        public string status { get; set; }
        public string msg { get; set; }
        public List<Produto> produto { get; set; }
    }
}
