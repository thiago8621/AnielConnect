using Aniel.Connect.Model.enumerado;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    public class TipoImovelApiRequest : ApiRequest
    {
    }
    public class EnderecoLogradouroApiRequest : ApiRequest
    {
       [Required]
       ///Cep consultado no cadastro de Endereço
        public string CEP { get; set; }        
        public string Bairro { get; set; }        
        public string Logradouro { get; set; }        

    }
    public class UFApiRequest : ApiRequest
    {
        [Required]      
        public string Descricao { get; set; }
        [Required]
        public string Sigla { get; set; }     
        public string Codigo_Ibge { get; set; }  
        public string CodigoAuxiliar { get; set; }  
        public Int64? Pais { get; set; }  
     
    }
    public class CidadeApiRequest : ApiRequest
    {
        [Required]      
        public string Descricao { get; set; }
        [Required]
        public Int64? IdUf { get; set; }     
        public string Codigo_Ibge { get; set; }  
        public string CodigoAuxiliar { get; set; }  
     
    }
    public class BairroApiRequest : ApiRequest
    {
        [Required]      
        public string Descricao { get; set; }
        [Required]
        public Int64? IdCidade { get; set; }   
        public string CodigoAuxiliar { get; set; }  
     
    }

    public class LogradouroCadastrarApiRequest : ApiRequest
    {
        [Required]      
        public String Logradouro { get; set; }
        [Required]
        public String Cep { get; set; }   
        [Required]
        public Int64? IdBairro { get; set; }
        [Required]
        public Tipo_Endereco_Logradouro? Tipo { get; set; }           
        public Tipo_Atendimento_Zona? Tipo_Zona { get; set; }  
        public String Cod_Area { get; set; }  
        public String Cod_Escritorio { get; set; }  
        public Int32? Cod_Localidade { get; set; }        
        public String Cod_Auxiliar { get; set; }
        public Int64? IdLogradouro { get; set; }

    }
    public class CondominioCadastrarApiRequest : ApiRequest
    {
        [Required]      
        public String Descricao { get; set; }
        [Required]
        public Int64? IdLogradouro { get; set; }

        public Int64? IdCondominio { get; set; }
        public String Numero { get; set; }
    }


    public class EnderecoCondominioApiRequest : ApiRequest
    {
       [Required]
       ///Cep consultado no cadastro de Endereço
        public string Cep { get; set; }        
        public string Bairro { get; set; }        
        public string Cidade { get; set; }        
        public string Logradouro { get; set; }        

    }

    public class CondominioPredioApiRequest : ApiRequest
    {
        /// <summary>
        /// Cep Consultado no cadastro de Condomínios
        /// </summary>
        [Required]
        public string CEP { get; set; }
        /// <summary>
        /// Número Consultado no cadastro de condomínios
        /// </summary>
         [Required]
        public string Numero { get; set; }
    }
    public class TipoImovelListModel
    {
        public long CodigoTipoImovel { get; set; }
        public string Descricao { get; set; }
        public string Digito { get; set; }
    }
    public class EnderecoLogradouroListModel
    {
        public long Id { get; set; }
        public long IdPais { get; set; }
        public long IdCidade { get; set; }
        public string Descricao { get; set; }
        public string Cep { get; set; }
        public long IdBairro { get; set; }
        public long IdUf { get; set; }
        public string Numero { get; set; }
        public string Status { get; set; }
        public string CodigoEscritorio { get; set; }
        public string CodigoArea { get; set; }
        public string CodigoLocalidade { get; set; }        
        public string Tipo { get; set; }
        public string Tipo_Zona { get; set; }
    }
    public class CondominioPredioListModel
    {
        public long Id { get; set; }
        public long IdPais { get; set; }
        public long IdCidade { get; set; }
        public string Descricao { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
        public string Uf { get; set; }
        public string Status{ get; set; }
        public long? IdEnderecoLogradouro { get; set; }
    }
}