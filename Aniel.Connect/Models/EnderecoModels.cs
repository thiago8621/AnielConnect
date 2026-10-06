using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Endereco_Cidade_SetDDD
    {
        public virtual String Id { get; set; }
        public virtual String DDD { get; set; }
        public virtual String UF { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class EnderecoComboModels : ObjComboResult<long>
    {
        public String DDD { get; set; }
    }

    public class EnderecoComboObj : ComboObj
    {
        public String DDD { get; set; }
        public String ID_UF { get; set; }
    }

    public class ObjComboUf : IObjComboResult<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Sigla { get; set; }
        public virtual String Text { get; set; }

    }

    public class EnderecoByRedeModel
    {
        public virtual String Cep { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual long? Cod_Localidade { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual long? IdUf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual long? IdCidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Numero { get; set; }

    }

    public class EnderecoCliente_Models
    {
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual long? IdCidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Ponto_Referencia { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual long? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Pais { get; set; }
        public virtual long? IdUf { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Bloco { get; set; }
        public virtual String Apartamento { get; set; }
        public virtual String Cod_CT { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Tipo_Imovel { get; set; }
        public virtual String Cod_Tipo_Imovel { get; set; }
    }
}