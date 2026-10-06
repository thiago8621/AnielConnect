using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Endereco_LogradouroListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Cep { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual Tipo_Endereco_Logradouro? Tipo_Endereco_Logradouro_Filtro { get; set; }
    }


    public class Endereco_LogradouroCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "CEP", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Cep { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Tipo_Atendimento_Zona? Tipo_Zona { get; set; }

        public virtual Boolean Status { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? UF { get; set; }

        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        public virtual long? Pais { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? Cidade { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? Bairro { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                 ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "TIPO_ENDERECO_LOGRADOURO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Tipo_Endereco_Logradouro? Tipo_Endereco_Logradouro { get; set; }

        public virtual String Cod_Localidade { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual string CodigoAuxiliar { get; set; }
        public virtual Boolean ViaCep { get; set; }
    }

    public class Endereco_Logradouro_DataTableRequest : DataTableRequest
    {
        public virtual String Cep { get; set; }
        public virtual String Tipo_Endereco_Logradouro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Status_Filtro { get; set; }
    }

    public class GetEnderecoByRedeRequestModel
    {
        public virtual long? IdElementoRede { get; set; }
        public virtual long? IdPop { get; set; }
    }

    [ExcelServiceKey("Descricao_Logradouro")]
    [ExcelServiceSheetName("Endereco_Logradouro")]
    public class Endereco_LogradouroImportacao : IExcelService
    {
        [MaxLength(8)]
        [ExcelService("CEP")]
        public virtual String Cep { get; set; }

        [MaxLength(50)]
        [ExcelService("Descrição do Endereço/Logradouro")]
        public virtual String Descricao_Logradouro { get; set; }

        [MaxLength(2)]
        [ExcelService("Tipo de Endereço/Logradouro")]
        public virtual String Tipo_Endereco_Logradouro { get; set; }
        public virtual String Tipo_Zona { get; set; }

        [MaxLength(30)]
        [ExcelService("País")]
        public virtual String Pais { get; set; }

        [MaxLength(2)]
        [ExcelService("UF")]
        public virtual String Sigla_UF { get; set; }

        [MaxLength(50)]
        [ExcelService("Cidade")]
        public virtual String Cidade { get; set; }

        [MaxLength(50)]
        [ExcelService("Bairro")]
        public virtual String Bairro { get; set; }

        public Boolean Importar { get; set; }
    }
    public class ObjComboResultTipoEndereco<T> : ObjComboResult<T>
    {
        public virtual String Tipo_Endereco_Logradouro { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_Pais { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual Int64? Id_Bairro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Descricao { get; set; }
        public string CodGrupoArea { get; internal set; }
        public string Localidade { get; internal set; }
        public string CodArea { get; internal set; }
    }

    public class ObjComboResultEndereco_Logradouro_Elem<T> : ObjComboResult<T>
    {
        public virtual String Tipo_Endereco_Logradouro { get; set; }
        public virtual String Tipo_Atendimento_Zona { get; set; }
        public virtual String Status { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_Pais { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Descricao { get; set; }

    }

    public class EnderecoLogradouroMobileModel
    {
        public virtual long Id_Logradouro { get; set; }
        public virtual string Logradouro { get; set; }
        public virtual Tipo_Endereco_Logradouro TipoLogradouro { get; set; }
        public virtual string Numero { get; set; }
        public virtual string Cep { get; set; }
        public virtual string Uf { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Pais { get; set; }
        public virtual long Id_Pais { get; set; }
        public virtual long Id_Uf { get; set; }
        public virtual long Id_Cidade { get; set; }
        public virtual long Id_Bairro { get; set; }


    }

    public class EnderecoIntegracaoMKDatatable : DataTableRequest
    {
        public virtual String Cod_Cliente { get; set; }
        public virtual String Cod_Contrato { get; set; }
        public virtual String CodConexao { get; set; }
        public virtual String Cliente { get; set; }
        public virtual String Cpf { get; set; }
        public virtual String Cnpj { get; set; }
        public virtual String Contrato_Cancelado { get; set; }
        public virtual String Conexao_Bloqueada { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String CodFatura { get; set; }
        public virtual String Valor_Total { get; set; }
        public virtual String Liquidado { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Fatura_Suspensa { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Motivo_Suspensao { get; set; }
    }
    public class Endereco_LogradouroResponseModel
    {
        public long? Id { get; set; }
        public long? IdBairro { get; set; }
        public long? IdCidade { get; set; }
        public long? IdUf { get; set; }
        public string Descricao { get; set; }
    }

    public class DadosEnderecoValidacao
    {
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string UF { get; set; }
        public string Pais { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Tipo_Imovel { get; set; }
        public string Ponto_Referencia { get; set; }
        public string Complemento { get; set; }
        public string Cod_Localidade { get; set; }
        public string Cod_Area { get; set; }
        public string Cod_Escritorio { get; set; }
        public string Busca_Endereco { get; set; }
        public string Bloco { get; set; }
        public string Apartamento { get; set; }
        public String mensagem { get; set; }
        public bool vendasemacesso { get; set; }

    }

}