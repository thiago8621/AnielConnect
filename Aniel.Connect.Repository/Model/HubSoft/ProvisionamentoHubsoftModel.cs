using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model.Hubsoft
{
    public class ConfigurarAutenticacaoHubsoft
    {
        public string id_cliente_servico { get; set; }
        public string phy_addr { get; set; }
        public string id_interface_conexao { get; set; }
    }

    public class HabilitarContratoHubsoft
    {
        public string motivo_habilitacao { get; set; }
        public bool migrar_atendimentos { get; set; }

    }

    public class ProvisionamentoHubsoftResponse
    {
        public string status { get; set; }
        public string exception { get; set; }
        public string msg { get; set; }
        public string message { get; set; }
        public List<string> errors { get; set; }
    }

    public class LimpezaMacHubsoft
    {
        public string id_cliente_servico { get; set; }
    }

    public class AtualizaCredencialWifi
    {
        public string id_cliente_servico { get; set;}
        public string descricao { get; set;}
        public string usuario {get; set;}
        public string senha { get; set; }
    }

   

}
