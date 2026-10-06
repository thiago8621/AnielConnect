using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Core;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary> Lista de Campos referentes a Função </summary>
    public class Funcao_ListaModel 
    {
        public virtual Int64 Cod_Funcao { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Classe  { get; set; }
        public virtual String Coluna { get; set; }
        public virtual String Revisao { get; set; }
        public virtual String HXH { get; set; }
        public virtual String Salario { get; set; }
        public virtual String Num_Cbo { get; set; }
        public virtual String Cod_Sindicato { get; set; }
        public virtual String Integracao { get; set; }
        public virtual String Adic_Insalubre { get; set; }
        public virtual String Adic_Periculosidade { get; set; }
        public virtual String Adic_Transferencia { get; set; }
        public virtual String Requisito { get; set; }
        public virtual String Objetivo { get; set; }
        public virtual String Desc_At_Ppp { get; set; }
        public virtual String Responsabilidade { get; set; }
        public virtual String Piso_Minimo { get; set; }
        public virtual String Cod_Funcao_Aux { get; set; }
        public virtual String Funcao_Tributavel { get; set; }
        public virtual String Cod_Perfil_Equipe { get; set; }
        public virtual String Permite_Gerar_Equipe { get; set; }
        public virtual String Grupo_Funcao { get; set; }
        public virtual String Status { get; set; }
    }
    /// <summary> Campos para cadastro de uma nova função</summary>
    public class Funcao_CadastroModel : ApiRequest
    {
        /// <summary> Código da Função </summary>
        public virtual Int64 Cod_Funcao { get; set; }
        /// <summary> Nome da Função </summary>
        public virtual String Nome { get; set; }
        /// <summary> Plano da Função </summary>
        public virtual String Plano { get; set; }
        /// <summary> Classe da Função </summary>
        public virtual String Classe  { get; set; }
        /// <summary> Coluna da Função </summary>
        public virtual String Coluna { get; set; }
        /// <summary> Revisão da Função </summary>
        public virtual String Revisao { get; set; }
        /// <summary> HXH da Função </summary>
        public virtual Int64? HXH { get; set; }
        /// <summary> Salário da Função </summary>
        public virtual Double? Salario { get; set; }
        /// <summary> Número CBO da Função </summary>
        public virtual String Num_Cbo { get; set; }
        /// <summary> Código de Sindicato da Função </summary>
        public virtual String Cod_Sindicato { get; set; }
        /// <summary> Integração da Função </summary>
        public virtual String Integracao { get; set; }
        /// <summary> Adicional Insalubridade da Função </summary>
        public virtual Double? Adic_Insalubre { get; set; }
        /// <summary> Adicional Periculosidade da Função </summary>
        public virtual Double? Adic_Periculosidade { get; set; }
        /// <summary> Adicional Transferência da Função </summary>
        public virtual Double? Adic_Transferencia { get; set; }
        /// <summary> Requisito da Função </summary>
        public virtual String Requisito { get; set; }
        /// <summary> Objetivo da Função </summary>
        public virtual String Objetivo { get; set; }
        /// <summary> Descrição At ppp da Função </summary>
        public virtual String Desc_At_Ppp { get; set; }
        /// <summary> Responsabilidades da Função </summary>
        public virtual String Responsabilidade { get; set; }
        /// <summary> Piso Mínimo da Função </summary>        
        public virtual Double? Piso_Minimo { get; set; }
        /// <summary> Codigo Auxiliar da Função </summary>
        public virtual String Cod_Funcao_Aux { get; set; }
        /// <summary> Tributo da Função </summary>
        public virtual String Funcao_Tributavel { get; set; }
        /// <summary> Codigo de Perfil de Equipe da Função </summary>
        public virtual String Cod_Perfil_Equipe { get; set; }
        /// <summary> Permite Gerar Equipe da Função </summary>
        public virtual String Permite_Gerar_Equipe { get; set; }
        /// <summary> Grupo da Função </summary>
        public virtual String Grupo_Funcao { get; set; }
        ///<summary> Status da Função </summary>
        public virtual Int32? Status { get; set; }
    }
}