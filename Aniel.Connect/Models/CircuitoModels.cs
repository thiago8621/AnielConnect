using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class CircuitoTelefoniaListaModel
    {
        public virtual string Id { get; set; }
        public virtual string Numerosip { get; set; }
        public virtual string Cliente { get; set; }
        public virtual string Plano { get; set; }
        public virtual string Status { get; set; }
    }

    public class CircuitoTelefoniaCadastroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64 Id { get; set; }
        public virtual String Numero_Sip { get; set; }
        public virtual Int64? IdCliente { get; set; }
        public virtual Int64? IdPlanoProduto { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String PlanoProduto { get; set; }
        public virtual Int64? IdRota { get; set; }
        public virtual string Senha { get; set; }
        public virtual string QtdLigacaoesSimultaneas { get; set; }
        public virtual bool Status { get; set; }
        public virtual Status_PlanoTelefonia Status_Circuito { get; set; }
        public virtual Boolean HabilitarLc { get; set; }
        public virtual Boolean HabilitarLdn { get; set; }
        public virtual Boolean HabilitarVc1 { get; set; }
        public virtual Boolean HabilitarVc2 { get; set; }
        public virtual Boolean HabilitarVc3 { get; set; }
        public virtual Boolean HabilitarBina { get; set; }
        public virtual Boolean BloqueioChamadaCobrar { get; set; }
        public virtual Boolean HabilitarConsumoMinimo { get; set; }
        public virtual List<DidVinculado> DidVinculados { get; set; }
        public virtual TipoDiscagem TipoDiscagem { get; set; }
    }

    public class DidVinculado
    {
        public virtual string Did { get; set; }
        public virtual string Mensalidade { get; set; }
        public virtual string SigaMe { get; set; }
        public virtual string BlackList { get; set; }
        public virtual string ChamadaCobrar { get; set; }
    }

    public class CircuitoTelefonia_DataTableRequest : DataTableRequest
    {
    }
    public class CircuitoTelefoniaSenharequest
    {
        public virtual bool SomenteNumeros { get; set; }
        public virtual bool SomenteLetras { get; set; }
        public virtual bool SomenteEspeciais { get; set; }
    }

    public class CircuitoTelefoniaDetalhesModel
    {
        private IList<DidsVinculados> dids { get; set; }
        public IList<DidsVinculados> DidsVinculados 
        {
            get 
            {
                if (dids == null) 
                {
                    dids =  new List<DidsVinculados>();
                }

                return dids; 
            } 
            set 
            { 
                dids = value;
            } 
        }
        public virtual long Id { get; set; }
        public virtual long IdVenda { get; set; }
        public virtual numero_Virtual Tipo_Numero { get; set; }
        public virtual long? Selecione_Did { get; set; }
        public virtual long? Selecione_Plano { get; set; }
        public virtual string Valor_Mensalidade { get; set; }
    }

    public class DidsVinculados
    {
        public long IdCircuito { get; set; }
        public long IdDid { get; set; }
        public string Did { get; set; }
        public string Ddr { get; set; }
        public string Sigame { get; set; }
        public string BlackList { get; set; }
        public string Chamada_Cobrar { get; set; }
        public long IdPlanoVinculado { get; set; }
        public string PlanoVinculado { get; set; }
        public string Mensalidade { get; set; }
    }
    public class requestDDrSequenciaModel 
    {
        public long Numero_ItensDDr { get; set; }
        public string JsonItensSequenciaDDR { get; set; }
        
    }

    public class AdicionarDidRequestModel
    {
        public long? IdDid { get; set; }
        public long IdVenda { get; set; }
        public long? IdPlano { get; set; }
        public string Mensalidade { get; set; }

    }

}