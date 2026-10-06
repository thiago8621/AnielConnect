using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{

    public class EstornoListaModel
    {

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String CodCtOrigem { get; set; }
        public virtual String ProjetoOrigem { get; set; }
        public virtual String CodMat { get; set; }
        public virtual String DescricaoMaterial { get; set; }
        public virtual String CodigoComplementar { get; set; }
        public virtual String CodCentroCusto { get; set; }
        public virtual String CodFornecedor { get; set; }
        public virtual String CnpjFornecedor { get; set; }
        public virtual String Operacao { get; set; }
        public virtual String SeqLivre { get; set; }
        public virtual String SeqRmdm { get; set; }
        public virtual String SeqTransf { get; set; }
        public virtual String Pedido { get; set; }
        public virtual String Data { get; set; }
        public virtual String Entrada { get; set; }
        public virtual String Saida { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String NotaFiscal { get; set; }
        public virtual String MatriculaEquipe { get; set; }
        public virtual String NomeEquipe { get; set; }
        public virtual String MotivoExclusao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Dia { get; set; }
        public virtual String Hora { get; set; }
        public virtual String Unidade { get; set; }
        public virtual String TipoMovimento { get; set; }

    }

    public class Estorno_ApiRequest : ApiRequest
    {
        /// <summary> Data Inicial do Periodo </summary>
        [Required]
        public virtual String DataInicial { get; set; }

        /// <summary> Data Final do Periodo </summary>
        [Required]
        public virtual String DataFinal { get; set; }

        /// <summary> Contrato do aniel que sera usado como filtro </summary>
        public virtual String ContratoAniel { get; set; }

        /// <summary> Projeto do aniel que sera usado como filtro </summary>
        public virtual String ProjetoAniel { get; set; }

    }
}