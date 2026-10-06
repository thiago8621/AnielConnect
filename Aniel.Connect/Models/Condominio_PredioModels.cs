using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Condominio_PredioListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Cep { get; set; }
        public virtual String Tipo_Endereco_Logradouro { get; set; }        
        public virtual String Descricao { get; set; }
        public virtual String Endereco_Logradouro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Status { get; set; }
        public virtual String Numero { get; set; }
        public virtual Status_Default? Status_Filtro { get; set; }
        public virtual Tipo_Endereco_Logradouro? Tipo_Endereco_Logradouro_Filtro { get; set; }
    }


    

    public class Condominio_Predio_DataTableRequest : DataTableRequest
    {
        public virtual String Cep { get; set; }
        public virtual String Tipo_Endereco_Logradouro { get; set; }        
        public virtual String Endereco_Logradouro { get; set; }                
        public virtual String Endereco_Logradouro_Desc { get; set; }                
        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }       
        public virtual String Status_Filtro { get; set; }
    }

    [ExcelServiceKey("Descricao_Condominio_Predio")]
    [ExcelServiceSheetName("Condominio_Predio")]
    public class Condominio_PredioImportacao : IExcelService
    {
        [MaxLength(8)]
        [ExcelService("Código Condominio/Predio no sistema")]
        public virtual String Id { get; set; }

        [MaxLength(8)]
        [ExcelService("CEP")]
        public virtual String Cep { get; set; }

        [MaxLength(100)]
        [ExcelService("Descrição do Condominio/Predio")]
        public virtual String Descricao_Condominio_Predio { get; set; }

        [ExcelService("Logradouro")]
        public virtual String Logradouro { get; set; }

        [ExcelService("País")]
        public virtual String País { get; set; }
        
        [ExcelService("UF")]
        public virtual String UF { get; set; }
        
        [ExcelService("Cidade")]
        public virtual String Cidade { get; set; }

        [ExcelService("Bairro")]
        public virtual String Bairro { get; set; }
        
        [ExcelService("Bloco/Quadra")]
        public virtual String Bloco_Quadra { get; set; }
        
        [ExcelService("Apartamento/Número")]
        public virtual String Apto_Numero { get; set; }

        [ExcelService("Código do Endereço/Logradouro no sistema")]
        public virtual String Id_Endereço_Logradouro { get; set; }

        [MaxLength(8)]
        [ExcelService("Número")]
        public virtual String Numero { get; set; }

        public Boolean Importar { get; set; }
    }
    public class ObjComboResultTipoCondominio<T> : ObjComboResult<T>
    {
        public virtual String Tipo_Endereco_Logradouro { get; set; }
        public virtual String Tipo_Atendimento_Zona { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Numero { get; set; }        
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }
        public virtual Int64? Id_Pais { get; set; }
        public virtual Int64? Id_Cidade{ get; set; }
        public virtual Int64? Id_Bairro { get; set; }
        public virtual Int64? Id_EnderecoLogradouro { get; set; }        
        public virtual String Descricao { get; set; }
        public virtual String Endereco_Logradouro { get; set; }   
        public virtual Int64? Id_Uf { get; set; }
        public string CodArea { get; internal set; }
        public string CodGrupoArea { get; internal set; }
        public string Localidade { get; internal set; }
    }

    public class ObjComboResultBloco<T> : ObjComboResult<T>
    {
        public virtual String IdCondominio { get; set; }
        public virtual Int64 Codigo { get; set; }
    }

    public class Condominio_PredioMobileModel
    {
        public virtual long Id_Condominio { get; set; }       
        public virtual string Descricao_Condominio { get; set; }
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
}