using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.API_Aniel.Models
{
    //18- API para disponibilizar uma consulta de devoluções gravadas no sistema Aniel.
    //    Na request dessa API deverá ser enviada o filtro período(obrigatório, não superior a 90 dias), contrato(opicional) e projeto(opicional). 
    //    Serão disponibilizadas as informações de Contrato, Projeto, Número do Documento, Data/Hora/Usuário Criação, Data/Hora/Usuário Baixa, 
    //    Data do Movimento, Equipe, Número da Obra, Código de Material, Código Complementar, Cód Complementar Auxiliar, Quantidade Pedida, 
    //    Quantidade Recebida, Valor Unitário, Quantidade Descontada, Valor Descontado e Classificação.
    #region MyRegion
    public class Devolucao_AnielListModel
    {
        public virtual String Codct { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Documento { get; set; }
        public virtual String Data_Hora_Criacao { get; set; }
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Data_Hora_Baixa { get; set; }
        public virtual String Usuario_Baixa { get; set; }
        public virtual String Data_Movimento { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Numero_Obra { get; set; }
        public virtual String Cod_Material { get; set; }
        public virtual String Cod_Complementar { get; set; }
        public virtual String Cod_Complementar_Auxiliar { get; set; }
        public virtual Int32? Quantidade_Pedida { get; set; }
        public virtual Int32? Quantidade_Recebida { get; set; }
        public virtual Int32? Quantidade_Desconto { get; set; }
        public virtual Double? Valor { get; set; }
        public virtual Double? Valor_Desconto { get; set; }
        public virtual String Classificacao { get; set; }
    }
    #endregion

    public class Devolucao_AnielApiRequest : ApiRequest
    {
        /// <summary> Pesquisa por um Contrato </summary>
        public virtual String Contrato { get; set; }
        /// <summary> Pesquisa por um Projeto </summary>
        public virtual String Projeto { get; set; }
        /// <summary> Data Inicial do Período de Busca </summary>
        public virtual String Data_Ini { get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        public virtual String Data_Fim { get; set; }

    }

    public class EstoqueDevolucao_ApiRequest : ApiRequest
    {
        [Required]
        public virtual String CodEmpresa { get; set; }
        [Required]
        public virtual String CodFilial { get; set; }
        [Required]
        public virtual String CodLocal { get; set; }
        [Required]
        public virtual String CodTecnico { get; set; }
        [Required]
        public virtual List<ProductToApproval> productToApproval { get; set; }
        public virtual String Doc_EasyWear { get; set; }
    }
}