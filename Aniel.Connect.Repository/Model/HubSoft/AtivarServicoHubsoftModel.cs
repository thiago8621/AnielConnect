using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.HubSoft.AtivarServico
{
    public class AtivarServicoHubsoftModel
    {
        public class Cliente
        {
            public int id_cliente { get; set; }
            public int codigo_cliente { get; set; }
            public string nome_razaosocial { get; set; }
        }

        public class ClienteServico
        {
            public int? id_cliente_servico { get; set; }
            public int? numero_plano { get; set; }
            public Servico servico { get; set; }
            public ServicoStatus servico_status { get; set; }
            public Cliente cliente { get; set; }
        }

        public class AtivarContratoHubsoftResponse
        {
            public string status { get; set; }
            public string exception { get; set; }
            public string msg { get; set; }
            public string message { get; set; }
            public List<string> errors { get; set; }
            public ClienteServico cliente_servico { get; set; }
        }

        public class Servico
        {
            public int? id_servico { get; set; }
            public string descricao { get; set; }
        }

        public class ServicoStatus
        {
            public int? id_servico_status { get; set; }
            public string descricao { get; set; }
        }
    }
}
