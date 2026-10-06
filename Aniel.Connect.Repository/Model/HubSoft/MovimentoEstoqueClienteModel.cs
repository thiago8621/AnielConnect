using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.HubSoft.MovimentoCliente
{
    public class Cliente
    {
        public long id_cliente { get; set; }
    }

    public class ClienteServico
    {
        public long id_cliente_servico { get; set; }
    }

    public class Empresa
    {
        public int id_empresa { get; set; }
        public List<LocaisEstoque> locais_estoque { get; set; }
    }

    public class LocaisEstoque
    {
        public int id_local_estoque { get; set; }
        public int id_empresa { get; set; }
        public bool movimentar { get; set; }
        public List<Produto> produtos { get; set; }
        public TipoMovimentoEstoque tipo_movimento_estoque { get; set; }
    }

    public class Patrimonio
    {
        public int id_produto_item { get; set; }
        public int id_local_estoque { get; set; }
        public int codigo_item { get; set; }
    }

    public class Produto
    {
        public int id_usuario { get; set; }
        public int id_produto { get; set; }
        public int id_local_estoque { get; set; }
        public double quantidade { get; set; }
        public string tipo_utilizacao { get; set; }
        public Produto2 produto { get; set; }
        public bool movimentar { get; set; }
        public List<Patrimonio> patrimonios { get; set; }
        public double quantidade_preenchida { get; set; }
    }

    public class Produto2
    {
        public int id_produto { get; set; }
    }

    public class MovimentoEstoqueCliente
    {
        public Cliente cliente { get; set; }
        public ClienteServico cliente_servico { get; set; }
        public Usuario usuario { get; set; }
        public TipoMovimentoEstoque tipo_movimento_estoque { get; set; }
        public List<Empresa> empresas { get; set; }
    }

    public class TipoMovimentoEstoque
    {
        public int id_tipo_movimento_estoque { get; set; }
    }

    public class Usuario
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
