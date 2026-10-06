using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Aniel.Connect.Core.Models;

namespace Aniel.Connect.API_Aniel.Models
{
    // 16- API 
    //        para receber as notas fiscais de entrada de material no estoque.
    //        Deverão ser enviados as informações de número do pedido, número da NF, CNPJ do fornecedor,
    //        data de emissão, data de recebimento, código do material, unidade, quantidade recebida e valor unitário.
    public class NF_Entrada_Estoque
    {
        public virtual Int64? Id { get; set; }

        public virtual String Numero_Pedido { get; set; }
        public virtual String Numero_NF { get; set; }

        public virtual String CNPJ_Fornecedor { get; set; }

        public virtual String Data_Emissao { get; set; }

        public virtual String Data_Recebimento { get; set; }

        public virtual String Cod_Material { get; set; }

        public virtual String Unidade { get; set; }

        public virtual Int64? Quantidade { get; set; }

        public virtual Double? Valor { get; set; }

    }

    public class NF_entradaApiRequest : ApiRequest
    {
        /// <summary> Data Inicial do Período de Busca </summary>
        public virtual String DataInicial { get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        public virtual String DataFinal { get; set; }
    }

}