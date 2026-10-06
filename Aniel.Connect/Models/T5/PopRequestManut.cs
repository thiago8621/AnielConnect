using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Aniel.Connect.Models.T5
{
    public class PopRequestManut : ManutRequest
    {
        public long IdPop { get; set; }
        public long IdOlt { get; set; }
        public long IdSlot { get; set; }
        public List<long?> IdsOlts { get; set; }
        public List<long?> IdsSlots { get; set; }
    }

    public class ElementoRedeRequestManut : ManutRequest
    {
        public long IdTipoElemento { get; set; }
        public long IdElemento { get; set; }
        public long IdPorta { get; set; }
        public string TipoElemento { get; set; }
        public List<long?> IdsTiposElementos { get; set; }
        public List<long?> IdElementos { get; set; }
    }


    public class EnderecosRequestManut : ManutRequest
    {
        public long IdTipoEndereco { get; set; }
        public long Id { get; set; }
        public string TipoEndereco { get; set; }
        public string DescBairro { get; set; }
        public long? IdUf { get; set; }
        public long? IdCidade { get; set; }
        public long? IdBairro { get; set; }
    }

    public class PontosAfetadosManut
    {
        public long? IdPop { get; set; }
        public long? IdElementoRede { get; set; }
        public long? IdTipoElementoRede { get; set; }
        public long? IdTipoEndereco { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }
        public string Tipo { get; set; }
        public List<string> IdsTipos { get; set; }
        public List<long?> Ids { get; set; }
        public List<ElementosRedeRequest> ElementosRedeRequests {get;set;}
    }

    public class PontosAfetadosRequestModel : ManutRequest
    {
        public string NodeId { get; set; }
        public string NodePai { get; set; }
        public string NodeAvo { get; set; }
        public string NodeBizAvo { get; set; }
        public string NodeProximo { get; set; }
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public string NumDoc { get; set; }
        public string NumObra { get; set; }
        public string Tipo { get; set; }
        public string Skip { get; set; }
    }
}