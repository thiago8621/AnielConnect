using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{

    //elementos carregados na pagina index
    public class PerfilAgendamentoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }      
        public virtual String GrupoArea { get; set; }
        public virtual String? Perfil { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String Tipo_Alteracao { get; set; }
        public virtual String DataHora_Alteracao { get; set; }
        public virtual String Usuario_Alteracao { get; set; }
        public virtual String PeriodoInicio { get; set; }
        public virtual String PeriodoFim { get; set; }

        public bool Configurados { get; set; }
        public bool NaoConfigurados { get; set; }
        public virtual Int64? PerfilSearch { get; set; }

    }
    public class PerfilAgendamentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
      //  public virtual FolhaDeptoAniel Departamento { get; set; }
        public virtual Boolean Status { get; set; }
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


    public class PerfilAgendamentoDatatableRequest : DataTableRequest
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }

        public virtual String? Perfil { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String Tipo_Alteracao { get; set; }
        public virtual String DataHora_Alteracao { get; set; }
        public virtual String Usuario_Alteracao { get; set; }
        public virtual String PeriodoInicio { get; set; }
        public virtual String PeriodoFim { get; set; }

        public bool Configurados { get; set; }
        public bool NaoConfigurados { get; set; }
        public virtual Int64? PerfilSearch { get; set; }


    }

    public class GetLogVincularPerfilDatatableRequest : DataTableRequest
    {

    }

    public class LogVincularPerfilListaModel
    {
        public virtual Int64 Id { get; set;}
        public virtual String UsuarioAlteracao { get;set; }
        public virtual String DataHoraAlteracao { get;set; }
        public virtual String TipoAlteracao { get;set; }
        public virtual String Tecnico { get;set; }
        public virtual String ValorAnterior { get;set; }
        public virtual String ValorAtual { get;set; }
        
    }

    public class VincularPerfilAgendamentoListaModel
    {
        public virtual String Equipe { get; set; }
        public virtual String PerfilAgendamento { get; set; }
        public virtual String NomeEquipe { get; set; }
        public virtual String Usuario { get; set; }


    }

    public class VincularPerfilRequestModel
    {
        public List<String> equipes { get; set; }
        public String usuario { get; set; }
        public Int64? id_perfil { get; set; }
    }

    public class LogPerfilAgendamentoListaModel
    {
        public virtual String Perfil { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String Tipo_Alteracao { get; set; }
   
        public virtual String PeriodoInicio { get; set; }
        public virtual String PeriodoFim { get; set; }
        public virtual String Usuario_Alteracao { get; set; }



    }
}