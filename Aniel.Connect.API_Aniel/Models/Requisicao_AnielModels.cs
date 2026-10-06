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
    //17- API para disponibilizar uma consulta de requisições gravadas no sistema Aniel.
    //    Na request dessa API deverá ser enviada o filtro período(obrigatório, não superior a 90 dias), contrato(opicional)
    //    e projeto(opicional). Serão disponibilizadas as informações de Contrato, Projeto, Número do Documento,
    //    Data/Hora/Usuário Criação, Data/Hora/Usuário Baixa, Data do Movimento, Equipe, Número da Obra, 
    //    Código de Material, Código Complementar, Cód Complementar Auxiliar, Quantidade Pedida, 
    //    Quantidade Recebida e Valor Unitário.
    #region MyRegion
    public class Requisicao_AnielListModel
    {
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Documento { get; set; }
        public virtual String Data_Hora_Criacao { get; set; }
        public virtual String Data_Hora_Baixa { get; set; }
        public virtual String Data_Movimento { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Numero_Obra { get; set; }
        public virtual String Cod_Material { get; set; }
        public virtual String Cod_Complementar { get; set; }
        public virtual String Cod_Complementar_Aux { get; set; }
        public virtual String Quantidade_Pedida { get; set; }
        public virtual String Quantidade_Recebida { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String UsuarioBaixa { get; set; }
        public virtual String Valor { get; set; }

    }
    #endregion
    public class Requiscao_ApiRequest : ApiRequest
    {
        /// <summary> Data inicial do período de Busca </summary>
        [Required]
        public virtual String Data_Ini { get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        [Required] 
        public virtual String Data_Fim { get; set; }
        /// <summary> Pesquisa por um Contrato Mais específico </summary>
        public virtual String Contrato { get; set; }
        /// <summary> Pesquisa por um Projeto mais específico </summary>
        public virtual String Projeto { get; set; }

    }

    public class Requisicao_CadastroModel : ApiRequest
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
        public virtual String GeraEstoque { get; set; }
        public virtual String  acceptURL { get; set; }
        public virtual String  declineURL { get; set; }
    }
}