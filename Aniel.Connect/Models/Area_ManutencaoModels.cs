using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class Area_Cadastro_ManutencaoModels
    {
        #region Area_Cadastro_ManutencaoModels
        public virtual Int64? Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String HoraInicial { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual String HoraFinal { get; set; }
        public virtual String ElementoRede { get; set; }
        public virtual String Ids_Elementos_Afetados { get; set; }
        public virtual String Ids_Nodes { get; set; }
        public virtual Boolean Status { get; set; }
        #endregion

    }
    public class Area_Cliente_ManutencaoModels
    {

        public virtual String CPF { get; set; }
        public virtual String Id_cod_plan_prod { get; set; }

        public virtual String DataFinal { get; set; }
        public virtual String HoraFinal { get; set; }
    }

    public class Area_ManutencaoModels
    {
        #region Area_ManutencaoModels
        public virtual Int64? Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String DataInicial { get; set; }
        public virtual String HoraInicial { get; set; }
        public virtual String DataFinal { get; set; }
        public virtual String HoraFinal { get; set; }
        public virtual String ElementoRede { get; set; }
        public virtual String Status { get; set; }
        #endregion

    }
    public class Area_ManutencaoDataTableRequest : DataTableRequest
    {

        public virtual Boolean Endereco { get; set; }
        public virtual Boolean GrupoArea { get; set; }

    }
    public class GetTreeViewElementosRede : DataTableRequest
    {
        #region GetTreeViewElementosRede

        public virtual long Id { get; set; }

        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Olt { get; set; }
        public virtual String Pop { get; set; }
        public virtual String Area_Cobertura { get; set; }

        public virtual long? IdTipo { get; set; }
        public virtual long? IdModelo { get; set; }
        public virtual long? IdPop { get; set; }
        public virtual long? IdOlt { get; set; }
        public virtual long? IdContrato { get; set; }
        public virtual long? IdProjeto { get; set; }
        public virtual long? IdArea_Cobertura { get; set; }
        #endregion
    }
    public class Area_ManutencaoTreeViewModel : ITreeViewModel
    {
        #region Area_ManutencaoTreeViewModelContrato
        public virtual String Id { get; set; }
        public virtual String Text { get; set; }
        public Boolean factor { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual List<Area_ManutencaoTreeViewModel> Children { get; set; }
        public TipoAgrupamento Tipo { get; set; }

        #endregion
    }
    public class Area_ManutencaoTreeViewRequestModel
    {
        public String Contrato { get; set; }
        public String Projeto { get; set; }
    }

}