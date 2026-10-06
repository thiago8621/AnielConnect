using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class CamposObrigatoriosListModel
    {
        public virtual List<CamposObrigatoriosClienteModels> Campos_Cliente { get; set; }
        public virtual List<CamposObrigatoriosAtendimentoModels> Campos_Atendimento { get; set; }
        public virtual List<CamposObrigatoriosLeadModels> Campos_Lead { get; set; }
        public virtual List<CamposObrigatoriosVendaModels> Campos_Venda { get; set; }
        public virtual List<CamposObrigatoriosDocumento_ProducaoModels> Campos_Documento_Producao { get; set; }
    }
    public interface ICamposObrigatoriosModel
    {
        public Int64 Id { get; set; }
        //public T Campo { get; set; }
        public String CampoString { get; set; }
        public Boolean Bloqueado { get; set; }
        public Boolean Obrigatorio { get; set; }
    }

    public class CamposObrigatoriosClienteModels : ICamposObrigatoriosModel
    {
        public virtual Int64 Id { get; set; }
        public virtual CamposObrigatoriosClienteEnum Campo { get; set; }
        public virtual String CampoString { get; set; }
        public virtual Boolean Bloqueado { get; set; }
        public virtual Boolean Obrigatorio { get; set; }
    }
    public class CamposObrigatoriosAtendimentoModels : ICamposObrigatoriosModel
    {
        public Int64 Id { get; set; }
        public CamposObrigatoriosAtendimentoEnum Campo { get; set; }
        public String CampoString { get; set; }
        public Boolean Bloqueado { get; set; }
        public Boolean Obrigatorio { get; set; }
    }
    public class CamposObrigatoriosDocumento_ProducaoModels : ICamposObrigatoriosModel
    {
        public Int64 Id { get; set; }
        public CamposObrigatoriosDocumentoProducaoEnum Campo { get; set; }
        public String CampoString { get; set; }
        public Boolean Bloqueado { get; set; }
        public String Titulo { get; set; }
        public Boolean Obrigatorio { get; set; }
    }
    public class CamposObrigatoriosLeadModels : ICamposObrigatoriosModel
    {
        public Int64 Id { get; set; }
        public CamposObrigatoriosLeadEnum Campo { get; set; }
        public String CampoString { get; set; }
        public Boolean Bloqueado { get; set; }
        public Boolean Obrigatorio { get; set; }
    }
    public class CamposObrigatoriosVendaModels : ICamposObrigatoriosModel
    {
        public Int64 Id { get; set; }
        public CamposObrigatoriosVendaEnum Campo { get; set; }
        public String CampoString { get; set; }
        public Boolean Bloqueado { get; set; }
        public Boolean Obrigatorio { get; set; }
    }

    public class CamposObrigatoriosDataTableRequest : DataTableRequest
    {

    }

}