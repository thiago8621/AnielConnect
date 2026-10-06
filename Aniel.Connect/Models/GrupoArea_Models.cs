using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using System.Linq;
using Sinapse.Util;
using Sinapse.Util.Excel;

namespace Aniel.Connect.Models
{
    public class GrupoArea_ListModel
    {
        public virtual String Id { get; set; }

        [Display(Name = "iDescricao", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual string Status { get; set; }
        public virtual String IdCadastro { get; set; }
    }


    //elementos carregados na pagina cadastro / alteracao
    public class GrupoArea_CadastroModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String IdCadastro { get; set; }

    }

    public class LogGrupoArea
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

    public class HistoricoAlteracaoGrupoArea_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
        public virtual String Observacao { get; set; }
    }

    public class GrupoArea_DataTableRequest : DataTableRequest
    {
        public virtual string Descricao { get; set; }
        public virtual String Id { get; set; }
        public virtual String Status { get; set; }

    }

    [ExcelServiceKey("Codigo")]
    public class Grupo_Area_Importacao : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Codigo")]
        public virtual String Codigo { get; set; }

        [ExcelService("Descricao")]
        public virtual String Descricao { get; set; }
        public Boolean Importar { get; set; }
    }

    public class GrupoAreaByBairroRequest
    {        
        public string DescricaoCidade { get; set; }
        public string DescricaoBairro { get; set; }        
    }
    public class GrupoAreaByBairroResponse
    {
        public string IdGrupoArea { get; set; }
        public string DescGrupoArea { get; set; }
        public string IdArea { get; set; }
        public string DescArea { get; set; }
        public string IdLocalidade { get; set; }
        public string DescLocalidade { get; set; }
    }

}
