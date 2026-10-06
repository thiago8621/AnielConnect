using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Newtonsoft.Json;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class PlanoIndexModel
    {
        public virtual String Plano { get; set; }
        public virtual String Situacao { get; set; }
        public virtual StatusONU Status { get; set; }
    }
    public class PlanoApiRequest : ApiRequest
    {
        /// <summary>
        /// Codigo do Plano
        /// </summary>
        public virtual String Cod_Plano { get; set; }
        /// <summary>
        /// Cpf ou Cnpj
        /// </summary>
        public virtual String Cpf_Cnpj { get; set; }

    }
    /// <summary>
    /// Model contendo o Status da ONU
    /// </summary>
    public class StatusONUModel
    {
        /// <summary>
        /// Serial Da ONU
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Status da ONU
        /// </summary>
        public StatusONU Status { get; set; }
    }
    public class DesbloqueioPlano
    {
        public virtual String Plano { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String Justificativa { get; set; }
    }
    public class DesbloqueioPlanoApiRequest : ApiRequest
    {
        /// <summary>
        /// Codigo do Plano
        /// </summary>
        public virtual String Cod_Plano { get; set; }
        /// <summary>
        /// Cpf ou Cnpj
        /// </summary>
        public virtual String Cpf_Cnpj { get; set; }

    }
    /// <summary>
    /// Request de PlanoProduto para Api de desbloqueio
    /// </summary>
    public class PlanoProdutoRadiusRequest
    {
        public PlanoProdutoRadiusRequest() { }
        public PlanoProdutoRadiusRequest(long idPlanoProduto) 
        {
            this.IdClientePlanoProduto = idPlanoProduto;
        }

        public long IdClientePlanoProduto { get; set; }
        public string Usuario { get; set; }
    }
}