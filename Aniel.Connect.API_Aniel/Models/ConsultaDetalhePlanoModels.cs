using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ConsultaDetalhePlanoModels
    {  
        public int id {  get; set; }
        public int id_cliente {  get; set; }
        public int status { get; set; }
        public int id_cliente_contrato { get; set; }
        public int id_combo { get; set; }
        public int id_troca_plano { get; set; }
        public int is_plano_adicional { get; set; }
        public int id_plano_atrelado { get; set; }
        public string descricao { get; set; }
        public string codigo { get; set; }
        public double valorplano { get; set; }
        public int situacaoplano { get; set; }
        public int id_tipo_servico { get; set; }
        public int integracao_externa { get; set; }
        public int utiliza_integracao_externa { get; set; }
        public string codigo_auxiliar { get; set; }
        public int os_mudanca_plano { get; set; }
        public string product_id { get; set; }
        public string codigo_tipo_plano_integracao { get; set; }
        public string codigo_integracao { get; set; }
        public int tipo { get; set; }
        public string codigo_integracao_aux { get; set; }
        public string detalhesplano { get; set; }
        public int usa_plano_adicional { get; set; }



    }

    public class PlanosListaModels
    {
        public ConsultaDetalhePlanoModels PlanoPrincipal { get; set;}
        public List<ConsultaDetalhePlanoModels> PlanosAdicionais {get; set;}
    }
}