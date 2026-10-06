using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Configuracao_Servico_OLTModel
    {
        public virtual long Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Marca_Modelo_OLT Marca_Modelo_OLT { get; set; }        
        public virtual string Desc_Marca_Modelo_OLT { get; set; }
        public virtual Periodicidade_ExecucaoEnum? Periodicidade_Execucao { get; set; }
        public virtual string Desc_Periodicidade_Execucao { get; set; }
        public virtual bool Status { get; set; }
        public virtual string Desc_Status { get; set; }
        public virtual string Hora_Execucao { get; set; }
        public virtual string Data_Ultima_Execucao { get; set; }
        public virtual bool Extrair_Diagnostico_Onu { get; set; }
        public virtual bool Consultar_Status_Onu { get; set; }
        public virtual int? Minutos_Execucao_Status_Onu { get; set; }
    }

    public class Configuracao_Servico_OLT_DataTableRequest : DataTableRequest
    {
        public Int32? Status { get; set; }
    }
}