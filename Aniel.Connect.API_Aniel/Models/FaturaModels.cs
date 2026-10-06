using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    //Portanto, deverão ser enviadas as informações de 
    //id fatura, cliente, plano, data de vencimento, data de pagamento,
    //valor, status, código de barras e link para baixar a fatura.
    public class FaturaListModel
    {
        public virtual String Id_Fatura { get; set; }             
        public virtual String Id_Cliente { get; set; }             
        public virtual String Cpf_Cnpj_Cliente { get; set; }             
        public virtual String Data_Pagamento { get; set; }        
        public virtual String Valor { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Código_Barras { get; set; }
        public virtual String Link_Fatura { get; set; }
    }
    public class Fatura_ApiRequest : ApiRequest
    {   /// <summary> Data Inicial do Período de Busca </summary>
        [Required]
        public virtual String DataInicial{ get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        [Required]
        public virtual String DataFinal { get; set; }
        /// <summary> CPF/CNPJ do cliente </summary>
        [Required]       
        public virtual String Cpf_Cnpj{get;set;}

    }
    public class Fatura_Cadastro_Model :ApiRequest
    {
        [Required]
        public virtual String Cpf_Cliente{get;set;}
        [Required]
        public virtual String Data_Vencimento{get;set;}
        [Required]
        public virtual long Id_fatura{get;set;}
        [Required]
        public virtual String URL {get;set;}
        public virtual char FaturaNegociacao {get;set;}
        public virtual long IdFaturaNegociada {get;set;}
        public virtual String DataNegociacao {get;set;}
        public virtual String CodigoBarras {get;set;}


    }
  
    
}