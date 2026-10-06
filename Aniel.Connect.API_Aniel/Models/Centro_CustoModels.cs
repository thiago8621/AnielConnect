using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aniel.Connect.Core;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary> Lista de Campos referentes a Centro de Custo </summary>
    public class Centro_Custo_ListaModel 
    {

        /// <summary> Código do Centro de Custo.  </summary>
        public virtual String Cod_CentroCusto { get; set; }
        /// <summary> Descrição do Centro de Custo.  </summary>
        public virtual String Descricao { get; set; }
        /// <summary> Status do Centro de Custo  </summary>
        public virtual String Ativo { get; set; }
        /// <summary> codigo auxliar do centro de custo  </summary>
        public virtual String Codigo_Auxiliar { get; set; }

    }
    /// <summary> Campos para cadastro de uma nova função </summary>
    public class Centro_Custo_CadastroModel : ApiRequest
    {
        
        /// <summary> Descrição do Centro de Custo </summary>
        public virtual String Descricao { get; set; }
        /// <summary> Código Auxliar do Centro de Custo  </summary>
        public virtual String Codigo_Auxiliar { get; set; }

    }

    public class Centro_Custo_ApiRequest : ApiRequest
    {
        /// <summary> Código do Centro de Custo </summary>
        
        public virtual int? Cod_CentroCusto { get; set; }

    }
}