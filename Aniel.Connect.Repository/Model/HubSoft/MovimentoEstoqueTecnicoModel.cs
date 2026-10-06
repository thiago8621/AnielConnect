using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.HubSoft.MovimentoTecnico
{
    public class Patrimonio
    {
        public int id_produto_item { get; set; }
    }

    public class Produto
    {
        public int id_produto { get; set; }
        public double quantidade { get; set; }
        public List<Patrimonio> patrimonios { get; set; }
        public string observacao { get; set; }
    }

    public class MovimentoTecnicoModel
    {
        public int id_local_estoque { get; set; }
        public int id_usuario { get; set; }
        public string tipo_utilizacao { get; set; }
        public bool solicitar_confirmacao { get; set; }
        public List<Produto> produtos { get; set; }
    }
}

namespace HubSoftService.Model.HubSoft.MovimentoTecnicoRetorno
{
    public class Patrimonio
    {
        public int id_produto_item { get; set; }
    }

    public class Produto
    {
        public int id_local_estoque { get; set; }
        public int id_produto { get; set; }
        public double quantidade { get; set; }
        public string campo_identificacao_patrimonio { get; set; }
        public List<Patrimonio> patrimonios { get; set; }
    }

    public class MovimentoTecnicoRetorno
    {
        public int id_usuario { get; set; }
        public string tipo_utilizacao { get; set; }
        public List<Produto> produtos { get; set; }
        public string observacao { get; set; }
    }
}
