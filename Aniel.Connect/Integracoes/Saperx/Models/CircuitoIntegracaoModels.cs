using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Models
{

    public class CircuitoIntegracaoResponseResult
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sip_password { get; set; }
        public string active { get; set; }
        public string sip_number { get; set; }
        public int? invoice_day { get; set; }
    }

    public class CircuitoIntegracaoResponse
    {
        public string message { get; set; }
        public CircuitoIntegracaoResponseResult result { get; set; }
    }

    public class CircuitoIntegracaoRequest
    {
        /// <summary>
        /// Nome do circuito *(Obrigatório)
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Senha SIP do circuito* (Obrigatório)
        /// </summary>
        public string sip_password { get; set; }
        /// <summary>
        ///0 – Bloqueio Parcial (Sip registra, não faz, mas recebe ligações) 1 – Ativado(100% funcional) 2 – Bloqueio Total(Sip registra, não faz e não recebe ligações) 3 – Cancelado(Sip não registra) *(Obrigatório)
        /// </summary>
        public string active { get; set; }
        /// <summary>
        ///Senha de acesso web
        /// </summary>
        public string internet_password { get; set; }
        /// <summary>
        ///0 – Circuito Pós-pago 1 – Circuito Pré-pago
        /// </summary>
        public bool pre_paid { get; set; }
        /// <summary>
        ///Campo LIVRE para uso do ERP. Pode ser o código do cliente do ERP. Não interfere no funcionamento do softswitch.
        /// </summary>
        public string cod_cli { get; set; }
        /// <summary>
        ///ID do plano internacional
        /// </summary>
        public int? plan_ddi { get; set; }
        /// <summary>
        ///ID do plano nacional
        /// </summary>
        public int? plan_national { get; set; }
        /// <summary>
        /// scm stfc custom-route
        /// </summary>
        public string plan_type { get; set; }
        /// <summary>
        ///ID da rota personalizada Ao informar uma rota personalizada, automaticamente o circuito irá receber o plan_type como sendo SCM. Caso não tiver rota personalizada via ser SCM ou STFC plan_type vai ser custom-route caso a rota personalizada tiver a opção E.164 desmarcada.Caso tiver rota personalizada e o E.164 estiver marcado, vai ser SCM.
        /// </summary>
        public int? custom_route { get; set; }
        /// <summary>
        ///Ativação da bina 0 – Bina desativada 1 – Bina ativada
        /// </summary>
        public bool show_callerid { get; set; }
        /// <summary>
        ///Habilitar ligações locais 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_LC { get; set; }
        /// <summary>
        ///Habilitar ligações longa distância nacional 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_LDN { get; set; }
        /// <summary>
        ///Habilitar ligações longa distância internacional 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_LDI { get; set; }
        /// <summary>
        ///Habilitar ligações locais móveis 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_VC1 { get; set; }
        /// <summary>
        ///Habilitar ligações longa distância nacional móveis 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_VC2 { get; set; }
        /// <summary>
        ///Habilitar ligações longa distância internacional móveis 0 – Desativado 1 – Ativado
        /// </summary>
        public bool call_VC3 { get; set; }
        /// <summary>
        ///Dia do fechamento. Valores permitidos: 1 a 27
        /// </summary>
        public int? invoice_day { get; set; }
        /// <summary>
        ///Canais Simultâneos ” ” – Vazio = Ilimitado 1 até 60
        /// </summary>
        public string call_limit { get; set; }
        /// <summary>
        /// CNPJ do cliente final
        /// </summary>
        public string cnpj { get; set; }
        
        

    }
}