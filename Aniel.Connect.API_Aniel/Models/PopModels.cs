using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{

    public class PopRequest : ApiRequest
    {
        public virtual Int64? Id { get; set; }

    }

    public class PopListModel
    {
        public virtual Int64 Id { get; set; }

        public String Nome { get; set; }
        public string Status { get; set; }
    }
    public class PopModels : ApiRequest
    {
        public int ID { get; set; }
        public string TOKEN { get; set; }
        public string NOME { get; set; }
        public string CODCT { get; set; }
        public string PROJETO { get; set; }
        public string CEP { get; set; }
        [Required(ErrorMessage = "O campo Endereço é obrigatório.")]
        public string ENDERECO { get; set; }
        [Required(ErrorMessage = "O campo Número é obrigatório.")]
        public string NUMERO { get; set; }
        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        public string BAIRRO { get; set; }

        [Required(ErrorMessage = "O campo ID_Cidade é obrigatório.")]

        /// <summary>
        /// Valor deve ser obtido pelo endpoint CIdade 
        /// </summary>
        public int? ID_CIDADE { get; set; }
        [Required(ErrorMessage = "O campo UF é obrigatório.")]
        public string UF { get; set; }
        public string COMPLEMENTO { get; set; }
        [Required(ErrorMessage = "O campo LATITUDE é obrigatório.")]
        public decimal LATITUDE { get; set; }
        [Required(ErrorMessage = "O campo LONGITUDE é obrigatório.")]
        public decimal LONGITUDE { get; set; }
        [Required(ErrorMessage = "O campo Id Empresa é obrigatório.")]
        public int? ID_EMPRESA { get; set; }
        [Required(ErrorMessage = "O campo Id Filial é obrigatório.")]
        public int? ID_FILIAL { get; set; }
        public int? STATUS { get; set; }
        [Required(ErrorMessage = "O campo DESCRICAO é obrigatório.")]

        public string DESCRICAO { get; set; }
        [Required(ErrorMessage = "O campo CODIGO é obrigatório.")]

        public int? CODIGO { get; set; }
        public string Cidade { get; set; }

        //   public int FILIAL { get; set; }
        //public int IDBAIRRO { get; set; }
        //public int IDCIDADE { get; set; }
    }
    public class PopModelsResponse
    {
        public int ID { get; set; }
        public string TOKEN { get; set; }
        public string NOME { get; set; }
        public string CIDADE { get; set; }

        public string CODCT { get; set; }
        public string PROJETO { get; set; }
        public string CEP { get; set; }
        [Required(ErrorMessage = "O campo Endereço é obrigatório.")]
        public string ENDERECO { get; set; }
        [Required(ErrorMessage = "O campo Número é obrigatório.")]
        public string NUMERO { get; set; }
        [Required(ErrorMessage = "O campo Bairro é obrigatório.")]
        public string BAIRRO { get; set; }

        [Required(ErrorMessage = "O campo ID_Cidade é obrigatório.")]
        public int? ID_CIDADE { get; set; }
        [Required(ErrorMessage = "O campo UF é obrigatório.")]
        public string UF { get; set; }
        public string COMPLEMENTO { get; set; }
        [Required(ErrorMessage = "O campo LATITUDE é obrigatório.")]
        public decimal LATITUDE { get; set; }
        [Required(ErrorMessage = "O campo LONGITUDE é obrigatório.")]
        public decimal LONGITUDE { get; set; }
        [Required(ErrorMessage = "O campo Id Empresa é obrigatório.")]
        public int? ID_EMPRESA { get; set; }
        [Required(ErrorMessage = "O campo Id Filial é obrigatório.")]
        public int? ID_FILIAL { get; set; }
        public int? STATUS { get; set; }
        [Required(ErrorMessage = "O campo DESCRICAO é obrigatório.")]

        public string DESCRICAO { get; set; }
        [Required(ErrorMessage = "O campo CODIGO é obrigatório.")]

        public int? CODIGO { get; set; }

        //   public int FILIAL { get; set; }
        //public int IDBAIRRO { get; set; }
        //public int IDCIDADE { get; set; }
    }
}


