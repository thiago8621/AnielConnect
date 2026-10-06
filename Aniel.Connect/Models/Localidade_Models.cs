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
    public class Localidade_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "iDescricao", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }
        public virtual string IdGrupoArea { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string IdArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string IdContrato { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Status { get; set; }
        public virtual string IdCadastro { get; set; }
        public virtual string Padrao { get; set; }
    }


    //elementos carregados na pagina cadastro / alteracao
    public class Localidade_CadastroModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Int64 Codigo { get; set; }
        public virtual String Descricao { get;  set; }
        public virtual string IdGrupoArea { get; set; }
        public virtual string GrupoArea { get; set; }
        public virtual string IdArea { get; set; }
        public virtual string Area { get; set; }
        public virtual string Contrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string IdContrato { get; set; }
        public virtual string IdProjeto { get; set; }
        public virtual string IdContrato_Infra { get; set; }
        public virtual string IdProjeto_Infra { get; set; }
        public virtual string IdCadastro { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),ErrorMessageResourceName = "MAXLENGHT")]
        public virtual string PontoRoteamento { get; set; }
        public virtual string Latidude { get; set; }
        public virtual string Longitude { get; set; }
        public virtual Boolean Padrao { get; set; }

    }

    public class AlterarLocalidadeStatusViewModel
    {
        public List<string> ids { get; set; }
        public Int16 status { get; set; }
        public String usuario { get; set; }
        public Int64? id_motivo_cancelamento { get; set; }
    }

    public class BuscarGeolocalizacaoLocalidadeRequestModel
    {
        public List<BuscarGeolocalizacaoLocalidadeViewModel> ids { get; set; }
        public String usuario { get; set; }
    }

    public class BuscarGeolocalizacaoLocalidadeViewModel
    {
        public string CodGrupoArea { get; set; }
        public string CodArea { get; set; }
        public string Localidade { get; set; }

    }

    public class Localidade_DataTableRequest : DataTableRequest
    { 
        public virtual string Descricao { get; set; }
        public virtual String Id { get; set; }
        public virtual String IdGrupoArea { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String IdArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String IdContrato { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Status { get; set; }
        public virtual bool VisualizarSemLocalizacao { get; set; }


    }

    public class ObjConsultaLocalidade
    {
        public virtual String Cod_Grupo { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual String Bairro { get; set; }
    }

    public class LogLocalidade
    {
        public virtual long Id { get; set; }
        public virtual long IdLog { get; set; }
        public virtual String Nome_usuario { get; set; }
        public virtual String DescricaoLocalidade { get; set; }
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

    public class HistoricoAlteracaoLocalidade_DataTableRequest : DataTableRequest
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
    public class Localidade_Importacao : IExcelService
    {
        [MaxLength(50)]
        [ExcelService("Codigo")]
        public virtual String Codigo { get; set; }

        [MaxLength(50)]
        [ExcelService("Area")]
        public virtual String Area { get; set; }

        [MaxLength(50)]
        [ExcelService("Grupo_Area")]
        public virtual String Grupo_Area { get; set; }

        [ExcelService("Descricao")]
        public virtual String Descricao { get; set; }

        [ExcelService("Contrato")]
        public virtual String Contrato { get; set; }

        [ExcelService("Projeto")]
        public virtual String Projeto { get; set; }

        public Boolean Importar { get; set; }
    }

    public class ObjComboLocalidade<T> : IObjComboResult<T>
    {
        public virtual T Id { get; set; }
        public virtual String Text { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
    }

}
