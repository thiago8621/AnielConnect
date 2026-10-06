using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Area_ListModel
    {
        public virtual String Id { get; set; }

        [Display(Name = "iDescricao", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual string Status { get; set; }

        [Display(Name = "iGrupoAreaDescricao", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string GrupoAreaDescricao { get; set; }
        public virtual string IdGrupoArea { get; set; }
        public virtual String IdCadastro { get; set; }

    }


    //elementos carregados na pagina cadastro / alteracao
    public class Area_CadastroModel : IViewModel<String>
    {
        public virtual compositeId CompositeId { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Id { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String GrupoAreaDescricao { get; set; }
        public virtual String IdGrupoArea { get; set; }
        public virtual String IdArea { get; set; }
        public virtual String IdCadastro { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? Distancia { get; set; }

        public class compositeId
        {
            public virtual String IdGrupoArea { get; set; }
            public virtual String IdArea { get; set; }
        }

    }

    public class Area_DataTableRequest : DataTableRequest
    {
        public virtual string Descricao { get; set; }
        public virtual string GrupoAreaDescricao { get; set; }
        public virtual String Id { get; set; }
        public virtual String Status { get; set; }
        public virtual String IdGrupoArea { get; set; }

    }

    public class LogArea
    {
        public virtual long Id { get; set; }
        public virtual long IdLog { get; set; }
        public virtual String Nome_usuario { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String DataHoraCriacao { get; set; }
        public virtual String TipoOperacao { get; set; }

        /// <summary>
        /// Identificador do Objeto
        /// </summary>
        public virtual String Chave { get; set; }

        /// <summary>
        /// Texto que representa o Identificador do Objeto 
        /// </summary>
        public virtual String DescricaoResourse { get; set; }

        public virtual String Campo { get; set; }
        public virtual String Campo_ValorAntes { get; set; }
        public virtual String Campo_ValorDepois { get; set; }
        public virtual String Observacao { get; set; }
    }

    public class HistoricoAlteracaoArea_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
        public virtual String Observacao { get; set; }
    }


    [ExcelServiceKey("Codigo")]
    public class Area_Importacao : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Codigo")]
        public virtual String Codigo { get; set; }

        [MaxLength(50)]
        [ExcelService("Grupo_Area")]
        public virtual String Grupo_Area { get; set; }

        [ExcelService("Descricao")]
        public virtual String Descricao { get; set; }
        public Boolean Importar { get; set; }
    }

    public class RelatorioAreaListModel
    {
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Matricula { get; set; }
        public virtual String Perfil { get; set; }
        public virtual String Habilidades { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String Tecnico { get; set; }
    }

    public class EquipeAreaDataTableRequest : DataTableRequest
    {
        public String Equipe { get; set; }
        public String Grupo_Area { get; set; }
        public String Area { get; set; }
        public String Localidade { get; set; }
        public String Perfil { get; set; }
        public String Habilidades { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String Tecnico { get; set; }
        public virtual String Agrupamento { get; set; }
        public String TipoRelatorio { get; set; }
    }

    public class ObjConsultaArea
    {
        public String UF { get; set; }
        public String Descricao { get; set; }
        public String Bairro { get; set; }
    }
}
