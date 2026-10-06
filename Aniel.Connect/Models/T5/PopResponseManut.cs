using System;

namespace Aniel.Connect.Models.T5
{
    public class PopManutResponse
    {
        public long Id { get; set; }
        public long IdPai { get; set; }
        public long IdAvo { get; set; }
        public long IdBizAvo { get; set; }
        public string Descricao { get; set; }
    } 
    public class ElementoRedeManutResponse
    {
        public long Id { get; set; }
        public long IdPai { get; set; }
        public long IdAvo { get; set; }
        public long IdBizAvo { get; set; }
        public string Descricao { get; set; }
        public string IsCabo { get; set; }
        public bool IsCaixa { get; set; }
    }  
    
    public class EnderecoManutResponse
    {
        public string Id { get; set; }
        public string IdPai { get; set; }
        public string Descricao { get; set; }
        public string TipoEndereco { get; set; }
        public string Titulo { get; set; }
    } 
    
    public class PopItensManutResponse
    {
        public Boolean TemRadius { get; set; }
        public Boolean TemConcentradora { get; set; }
        public Boolean TemOlt { get; set; }
    }
}