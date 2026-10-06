using System;

namespace Aniel.Connect.API_Aniel.Models
{

    #region MyRegion
    public class AreaListModel
    {
       public virtual String Contrato { get; set; }
       public virtual String Projeto { get; set; }
       public virtual String Equipe  { get; set; }
       public virtual String Número_da_Obra { get; set; }
       public virtual String RDO { get; set; }
       public virtual String Contrato_Assinante { get; set; }
       public virtual String Numero_OS { get; set; }
       public virtual String DataHora_Agendamento { get; set; }
       public virtual String DataHora_Aplicacao { get; set; }
       public virtual String Cod_Material { get; set; }
       public virtual String CodCpl { get; set; }
       public virtual String CodCpl_Aux { get; set; }
       public virtual String Quantidade_Aplic { get; set; }
       public virtual String Quantidade_Removida { get; set; }
    } 
    #endregion
    public class AreaManutencao_ApiRequest : ApiRequest 
    {
        /// <summary> Data Inicial do Período de Busca </summary>
        public virtual String Data_Ini { get; set; }
        /// <summary> Data Final do Período de Busca </summary>
        public virtual String Data_Fim { get; set; }
        /// <summary> Pesquisa por um Contrato </summary>
        public virtual String Contrato { get; set; } 
        /// <summary>Pesquisa por um Projeto </summary>
        public virtual String Projeto { get; set; } 
    }

}