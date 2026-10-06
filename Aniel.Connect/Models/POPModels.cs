using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class POPListModel
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
       
        public String Empresa { get; set; }
        public String Filial { get; set; }
        public String Contrato { get; set; }
        public String Projeto { get; set; }

        public string Status { get; set; }

        #region Endereço

        public String Endereco { get; set; }
        public String Pais { get; set; }
        public String Cidade { get; set; }
        public String UF { get; set; }
        public String Bairro { get; set; }
        public String Cep { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }

        #endregion
    }
    public class POPCadastroModel
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }
        public String CodigoAuxiliar { get; set; }

        public bool Status { get; set; }

        public Int64? IdEmpresa { get; set; }
        public Int64? IdFilial { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }

        #region Endereço

        public String Cep { get; set; }
        public String Endereco { get; set; }
        public String Numero { get; set; }
        public Int64? IdBairro { get; set; }
        public String Bairro { get; set; }
        public Int64? IdPais { get; set; }
        public string Pais { get; set; }
        public Int64? IdCidade { get; set; }
        public String Cidade { get; set; }
        public String UF { get; set; }
        public String Complemento { get; set; }
               
        public String Latitude { get; set; }
        public String Longitude { get; set; }

        #endregion
    }

    public class POPDatatableRequest : DataTableRequest
    {
        public Int64? IdEmpresa { get; set; }
        public Int64? IdFilial { get; set; }

        public String CodCt { get; set; }
        public String Projeto { get; set; }

        public String UF { get; set; }
        public Int64? IdPais { get; set; }
        public Int64? IdCidade { get; set; }
        public Int64? IdBairro { get; set; }
    }

    [ExcelServiceKey("ID")]
    [ExcelServiceSheetName("POP")]
    public class POPImportacao : IExcelService
    {

        [ExcelService("ID")]
        public virtual Int64? ID { get; set; }
        [Required]
        [ExcelService("NOME")]
        public virtual String NOME { get; set; }

        [ExcelService("STATUS")]
        public virtual bool STATUS { get; set; }

        [ExcelService("ID_EMPRESA")]
        public virtual Int64? ID_EMPRESA { get; set; }

        [ExcelService("ID_FILIAL")]
        public virtual Int64? ID_FILIAL { get; set; }
        [Required]
        [ExcelService("CODCT")]
        public virtual String CODCT { get; set; }
        [Required]
        [ExcelService("PROJETO")]
        public virtual String PROJETO { get; set; }

        [Required]
        [ExcelService("CEP")]
        public virtual String CEP { get; set; }
        [Required]
        [ExcelService("ENDERECO")]
        public virtual String ENDERECO { get; set; }
        [Required]
        [ExcelService("NUMERO")]
        public virtual String NUMERO { get; set; }
        [Required]
        [ExcelService("BAIRRO")]
        public virtual String BAIRRO { get; set; }

        [Required]
        [ExcelService("CIDADE")]
        public virtual String CIDADE { get; set; }

        [ExcelService("COMPLEMENTO")]
        public virtual String COMPLEMENTO { get; set; }

        [ExcelService("LATITUDE")]
        public virtual String LATITUDE { get; set; }

        [ExcelService("LONGITUDE")]
        public virtual String LONGITUDE { get; set; }

        [ExcelService("DESCRICAO")]
        public virtual String DESCRICAO { get; set; }
        [ExcelService("UF")]
        public virtual String UF { get; set; }   
        [ExcelService("PAIS")]
        public virtual String PAIS { get; set; }

        public Boolean Importar { get; set; }

    }
}