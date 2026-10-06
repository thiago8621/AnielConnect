using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ElementoRedeEnderecoInsertRequestModel
    {
        public long IdElementoRede { get; set; }
        public long IdEnderecoLogradouro { get; set; }
        public string Numero { get; set; }
    }

    public class ElementoRedePortaInsertRequestModel
    {
        public long Id_Caixa { get; set; }
        public long? Cliente_Plano_Prod { get; set; }
        public int? Porta { get; set; }
        public DateTime? Data_Expiracao_Reserva { get; set; }
        public string Login { get; set; }
        public long? IdLogin { get; set; }
        public Sinapse.Model.enumerados.Situacao_Porta Status { get; set; }
    }

    public class ElementoRedeRequestModel : ApiRequest
    {
        public Int32 Id { get; set; }
        public String Bairro { get; set; }
        public String Pais { get; set; }
        public virtual long? Cod_Caixa { get; set; }
        public string Cabo { get; set; }
        public string Caixa { get; set; }
        public string Poste { get; set; }
        public virtual int Capacidade { get; set; }
        public String CEP { get; set; }
        public virtual bool Certificado { get; set; }
        public String Cidade { get; set; }
        public String Complemento { get; set; }
        public String Contrato { get; set; }
        public String Cor_linha_Ativa { get; set; }
        public String Cor_linha_Inativa { get; set; }

        public String Descricao { get; set; }
        public String Endereco { get; set; }
        public  Int64? Id_POP { get; set; }
        public  Int64? Id_OLT { get; set; }
        public Int64? Id_Slot { get; set; }
        public Int64? Id_Pon { get; set; }
        public  double? Latitude { get; set; }
        public  double? Longitude { get; set; }
        public String Numero { get; set; }
        public String Observacao { get; set; }
        public String Projeto { get; set; }
        public  Status_Elemento Situacao { get; set; }
        public string Id_Tipo { get; set; }
      
        public String UF { get; set; }
        public String Coordenadas { get; set; }
        public String AreaAtendimento { get; set; }
        public  long? Num_Coordenadas { get; set; }
        public virtual Double? Metragem { get; set; }
        public virtual Padrao_Cor Padrao_Cores { get; set; }

    }

}