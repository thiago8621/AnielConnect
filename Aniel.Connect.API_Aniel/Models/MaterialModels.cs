using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary> Lista de Campos referentes a Centro de Custo </summary>
    public class Material_ListaModel
    {

        /// <summary> Código do material.  </summary>
        public virtual String Codigo_Material { get; set; }

        /// <summary> Descrição do centro de custo.  </summary>
        public virtual String Descricao { get; set; }


        public virtual String Unidade { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Precisao { get; set; }
        public virtual String Familia { get; set; }
        public virtual String Grupo { get; set; }
        public virtual String Classificacao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Hora_Criacao { get; set; }
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Ativacao_Sinal { get; set; }
        public virtual String Apelido { get; set; }
        public virtual String Codigo_Complementar { get; set; }
        public virtual String CodMat_Integra { get; set; }
        public virtual String CodMat_Auxiliar { get; set; }
        public virtual String Codigo_Material_Ret { get; set; }
        public virtual String Codigo_Material_Det { get; set; }
        public virtual String Codigo_Material_Reap { get; set; }
        public virtual String Codigo_Material_Defeito { get; set; }
        public virtual String Codigo_Material_Sucata { get; set; }
        public virtual String Estoque_Minimo { get; set; }


    }
    /// <summary> Campos para cadastro de uma nova função </summary>
    public class Material_CadastroModel : ApiRequest
    {
        /// <summary> Código do Material </summary>
        public virtual String Codigo_Material { get; set; }
        /// <summary> Descrição do Material </summary>
        public virtual String Descricao { get; set; }
        /// <summary> Unidade de medida do Materia </summary>
        public virtual String Unidade { get; set; }
        /// <summary> Valor do Material </summary>
        public virtual Double? Valor { get; set; }
        /// <summary> Precisão do Material </summary>
        public virtual String Precisao { get; set; }
        /// <summary> Família do Material </summary>
        public virtual String Familia { get; set; }
        /// <summary> Grupo do Material
        /// </summary>
        public virtual String Grupo { get; set; }
        /// <summary> Classificação do Material </summary>
        public virtual String Classificacao { get; set; }
        /// <summary> Tipo do Material </summary>
        public virtual String Tipo { get; set; }
        public virtual String SubCod { get; set; }
        public virtual String SubCodExclusivo { get; set; }
        public virtual String Desc_Auxiliar { get; set; }
        /// <summary>
        /// define o tipo de equipamento no aniel, campo usado durante o processo de provisionamento
        /// ONU = 1,Roteador = 2,TV = 3,Telefonia = 4
        /// </summary>
        public virtual TipoEquipamento? TipoEquipamento { get; set; }
        /// <summary>
        /// define o material deve ser listado durante o processo de provisionamento para ativar sinal
        /// </summary>
        public virtual bool? AtivaSinal { get; set; }


    }

    public class Material_ApiRequest : ApiRequest
    {
        /// <summary> Código do Material </summary>

        public virtual String Codigo_Material { get; set; }



    }
}