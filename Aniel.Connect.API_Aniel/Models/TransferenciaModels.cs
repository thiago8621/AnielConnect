using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{

    public class TransferenciaListaModel
    {
        public virtual String Contrato_Origem { get; set; }
        public virtual String Projeto_Origem { get; set; }
        public virtual String Contrato_Destino { get; set; }
        public virtual String Projeto_Destino { get; set; }
        public virtual String Seq_Transf { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Hora_Criacao { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String CodCentroCusto { get; set; }
        public virtual String DescricaoCCentroCusto { get; set; }

        public virtual String Data_Documento { get; set; }
        public virtual String Codigo_Material { get; set; }
        public virtual String Descricao_Material { get; set; }
        public virtual String Codigo_Complementar { get; set; }
        public virtual String Codigo_ComplementarAux { get; set; }
        public virtual String QuantidadeTransferida { get; set; }


    }

    public class TransferenciaFornecedorListaModel
    {
        
        
        public virtual String Contrato{ get; set; }
        public virtual String Projeto{ get; set; }
        public virtual String Sequencial { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Hora_Criacao { get; set; }
        public virtual String UsuarioCriacao { get; set; }

        public virtual String Data_Documento { get; set; }
        public virtual String Codigo_Material { get; set; }
        public virtual String Codigo_Complementar { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Codigo_ComplementarAux { get; set; }
        public virtual String QuantidadeTransferida { get; set; }
        public virtual String Cnpj_Fornecedor { get; set; }


    }

    public class Transferencia_ApiRequest : ApiRequest
    {
        /// <summary> Data Inicial do Período </summary>
        [Required]
        public virtual String DataInicial { get; set; }

        /// <summary> Data Final do Período </summary>
        [Required]
        public virtual String DataFinal { get; set; }

        /// <summary> Contrato do aniel que sera usado como filtro </summary>
        public virtual String ContratoAniel { get; set; }

        /// <summary> Projeto do aniel que sera usado como filtro </summary>
        public virtual String ProjetoAniel { get; set; }
        public virtual bool Alterado { get; set; }

    }
}