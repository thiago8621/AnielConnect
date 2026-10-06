using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using Situacao_Porta = Sinapse.Model.enumerados.Situacao_Porta;

namespace Aniel.Connect.Models.T5
{
    public class Elemento_Rede_PortaListModel
    {
        #region MyRegion
        public virtual Int64? Id { get; set; }
        public virtual String Caixa { get; set; }
        public virtual Int32? Porta { get; set; }
        public virtual String Status { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Login { get; set; }
        public virtual String Olt { get; set; }
        public virtual String Pop { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        #endregion
    }
    public class Elemento_Rede_PortaRequest : DataTableRequest
    {
        //Id que será usado para filtrar a tabela buscando pelas portas daquele cadastro
        public virtual String Id { get; set; }
        public virtual Int64? IdTipoElemento { get; set; }
        public virtual String Caixa { get; set; }
        public virtual String Porta { get; set; }
        public virtual List<Int64?> Status { get; set; } = new List<Int64?>();
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Login { get; set; }
        public virtual String Olt { get; set; }
        public virtual String Pop { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Uf { get; set; }
    }
    //public class Elemento_Rede_PortaCadastroModel
    //{
    //    //public virtual Elemento_Rede Caixa { get; set; }
    //    public virtual Int64 Id_Caixa { get; set; }
    //    public virtual Int64 Id { get; set; }
    //    public virtual Int32 Porta { get; set; }
    //    public virtual Situacao_Porta Status { get; set; }
    //}
    public class Elemento_Rede_PortaCombomodel
    {
        public virtual String Combo { get; set; }
    }    
    public class GeositeCaixaPortaFtthModel
    {

        public long? fidCaixa { get; set; }
        public long? fidCliente { get; set; }
    }
    public class Ligacao_Porta_GeositeRequestModel
    {
        public long porta { get; set; }
        public List<long> portas { get; set; }

        public long cliente_PlanoProduto { get; set; }
    }
    public class Elemento_Rede_PortaGetListModel
    {

        public virtual Int64? Id { get; set; }
        public virtual String Caixa { get; set; }
        public virtual Int32? Porta { get; set; }
        public virtual String Status { get; set; }
        

    }
}
