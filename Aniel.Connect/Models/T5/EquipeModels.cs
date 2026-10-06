using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class EquipeListaModel
    {
        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Descricao { get; set; }
        public virtual bool ConfiguradoRoteirizacao { get; set; }

        public virtual String Monitor { get; set; }
        public virtual String Nome_usuario { get; set; }
        public virtual String DataHoraCriacao { get; set; }
        public virtual String TipoOperacao { get; set; }

        //Usado na tela de habilitar área

        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }

    }

    public class EquipeListaModelDataTable : DataTableRequest
    {
        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Monitor { get; set; }
    }

    public class EquipeDataTableListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

    }

    public class NaturezasListaModel
    {

        public virtual Int32 Id { get; set; }

        public virtual Int32? Id_Natureza { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String Departamento { get; set; }

    }



    public class EquipeStatusModel
    {
        public virtual String Equipe { get; set; }
        public virtual String Endereco { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }

        public virtual String Precisao { get; set; }
        public virtual String Bateria { get; set; }

        public virtual String StatusGPS { get; set; }
        public virtual String Dispositivo { get; set; }

        public virtual String Versao_Android { get; set; }
        public virtual String Versao_App { get; set; }

        public virtual String Tipo { get; set; }

        public virtual String InternetMovel { get; set; }
        public virtual String WiFi { get; set; }
        public virtual String IMEI { get; set; }

        public virtual String Atualizado_Em { get; set; }
        public virtual String DataHora_Login { get; set; }

        public virtual String Status_App { get; set; }

        public virtual String Justificativa_Ausencia { get; set; }
        public virtual String Motivo_Paralisacao { get; set; }

    }

    public class Habilitar_ServicoCadastroModel : IViewModel<String>
    {
        public virtual String Id { get; set; }
        public virtual String Ids { get; set; }
        public virtual String Desc_Natureza { get; set; }
        public virtual Int32? Id_Natureza { get; set; }

        public virtual String Nome { get; set; }

        private List<NaturezasListaModel> Naturezas_lista;

        public virtual List<NaturezasListaModel> Naturezas

        {
            get
            {
                if (Naturezas_lista == null)
                    Naturezas_lista = new List<NaturezasListaModel>();
                return Naturezas_lista;
            }
            set
            {
                Naturezas_lista = value;
            }
        }



    }

    public class AreasListaModel
    {

        public virtual Int32 Id { get; set; }

        public virtual String GrupoAreaStr { get; set; }
        public virtual String Cod_Escritorio { get; set; }

        public virtual String AreaStr { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int16? Nivel { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Boolean Todas_Localidades { get; set; }
        public virtual Boolean Todas_Areas { get; set; }

    }


    public class Habilitar_AreaCadastroModel : IViewModel<String>
    {

        public virtual String Id { get; set; }
        public virtual String Ids { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual Int16? Nivel { get; set; }

        public virtual Boolean Todas_Localidades { get; set; }


        private List<AreasListaModel> areas_lista;

        public virtual List<AreasListaModel> Areas

        {
            get
            {
                if (areas_lista == null)
                    areas_lista = new List<AreasListaModel>();
                return areas_lista;
            }
            set
            {
                areas_lista = value;
            }
        }



    }


    public class EquipeCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(5, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Codigo { get; set; }


        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "REQUIRED")]
        public virtual Status_Default Status { get; set; }
    }

    public class RequestObjComboCriterios<T, X, Y, Z> : RequestObjCombo
    {
        public virtual String id { get; set; }
        public virtual T Tipo_serv { get; set; }
        public virtual X Grupo_Area { get; set; }
        public virtual Y Area { get; set; }
        public virtual Z Cod_Localidade { get; set; }
        public String NumObra { get; set; }
        public String CodCt { get; set; }
        public String Projeto { get; set; }
        public Int32? NumDoc { get; set; }
    }

    public class Equipe_DataTableRequest : DataTableRequest
    {

    }

    public class ServicosExecutadosPorTecnico_DataTableRequest : DataTableRequest
    {
        public virtual String Equipe { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual Int32? Cod_Tipo_Serv { get; set; }
        public virtual Int64? Cod_Cliente { get; set; }
        public virtual String Supervisor { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String Data_Inicio { get; set; }
        public virtual String Data_Fim { get; set; }
        public virtual String Centro { get; set; }
        public virtual String Empresa { get; set; }
        public virtual String Filial { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Habilidade { get; set; }
        public virtual String Tipo_Data { get; set; }
        public virtual String Tipo_SLA { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String CodCt_Encerramento { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_PlanoProduto { get; set; }
    }

    public class RequestObjComboAtribuirTecnicoCrit<T> : RequestObjCombo
    {
        public virtual T id { get; set; }
        public virtual ObjComboConfigAtribuirTecnicoCrit? config { get; set; }
    }

    public class ObjComboConfigAtribuirTecnicoCrit
    {
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Cod_Localidade { get; set; }
        public virtual String Tipo_serv { get; set; }
    }

    public class LogVincularMonitorTecnico_DatatableRequest : DataTableRequest
    {
        public virtual String Tecnico { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
    }

    public class LogVincularMonitorTecnicoListaModel
    {
        public virtual String Tecnico { get; set; }
        public virtual String Monitor { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String DataAlteracao { get; set; }
    }

    public class EquipeComboConfig
    {
        /// <summary>
        ///     Define se o combo de equipe deverá exibir somente
        ///     técnicos dos quais o usuário logado é supervisor ou monitor
        /// </summary>
        public bool ExibirApenasTecnicosSupervisorMonitor { get; set; }
    }

    public class ObjComboResultSupervisorT5 : ObjComboResult<string>
    {
        /// <summary>
        ///     Matrícula do supervisor
        /// </summary>
        public string CodUsuario { get; set; }
        public string CodEquipe { get; set; }
    }
}