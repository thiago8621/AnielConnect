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



    public class Vincular_UsuarioPerfilListaModel
    {
        public virtual String Id { get; set; }

        public virtual String Usuario { get; set; }

        public virtual String Login { get; set; }
        public virtual String PerfilDashboard { get; set; }
        public virtual String PerfilAgente { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String Secao { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Departamento { get; set; }
        public virtual string PerfilVisualizacaoServico { get; set; }
        public virtual string PerfilVisualizacaoVenda { get; set; }        

    }

    public class VincularUsuarioVendaModel
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

    public class Vincular_UsuarioPerfilCadastroModel : IViewModel<String>
    {
        public virtual bool Bloqueia_Modelo_Visualizacao { get; set; }

        // 2. RECEBE o ID do Modelo ÚNICO permitido (int? para permitir valor nulo)
        public virtual int? Modelo_Unico_Permitido_ID { get; set; }
        public virtual String Id { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Login { get; set; }
        public virtual String SecaoConnect { get; set; }
        public virtual String Desc_Secao { get; set; }
        public virtual String PerfilAgente { get; set; }
        public virtual String TipoUsuario { get; set; }
        public virtual String Desc_Tipo_Usuario { get; set; }
        public virtual Int64? Id_Perfil { get; set; }
        public virtual Int64? Id_Departamento { get; set; }
        public virtual String Perfil { get; set; }
        public virtual String PerfilVenda { get; set; }
        public virtual String Desc_Perfil_Vinculado { get; set; }
        public virtual long? IdPerfilVisualizacaoServico { get; set; }
        public virtual long? IdPerfilVisualizacaoVenda { get; set; }

        private List<VincularCadastroModel> VincularLista;
        public virtual List<VincularCadastroModel> Vincular
        {
            get
            {
                if (VincularLista == null)
                    VincularLista = new List<VincularCadastroModel>();
                return VincularLista;

            }
            set
            {
                VincularLista = value;
            }

        }
        public virtual long? IdParamPreQual { get; set; }
    }

    public class Vincular_MultiploPerfilCadastroModel : IViewModel<String>
    {
        public virtual List<String> Id { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Login { get; set; }
        public virtual String SecaoConnect { get; set; }
        public virtual String PerfilAgente { get; set; }
        public virtual String TipoUsuario { get; set; }
        public virtual Int64? Id_Perfil { get; set; }
        public virtual String Perfil { get; set; }
        public virtual String PerfilVenda { get; set; }
        public virtual bool TemPerfil { get; set; }
        string IViewModel<string>.Id { get; set; }
        public virtual long? IdPerfilVisualizacaoServico { get; set; }
        public virtual long? IdPerfilVisualizacaoVenda { get; set; }
        public virtual long? IdParamPreQual { get; set; }
        public virtual bool Bloqueia_Modelo_Visualizacao { get; set; }

        // 2. RECEBE o ID do Modelo ÚNICO permitido (int? para permitir valor nulo)
        public virtual int? Modelo_Unico_Permitido_ID { get; set; }
    }


    public class Vinculo_UsuarioPerfil_DataTableRequest : DataTableRequest
    {
        public int? Secao { get; set; }
        public int? TipoUsuario { get; set; }
        public int? PerfilAgente { get; set; }
        public int? PerfilVenda { get; set; }
        public int? Perfil { get; set; }
        public bool Todos { get; set; }
        public bool Configurados { get; set; }
        public bool NaoConfigurados { get; set; }
        public string Nome { get; set; }
    }

    public class VincularCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String SecaoConnect { get; set; }
        public virtual String Desc_Secao { get; set; }

        public virtual String TipoUsuario { get; set; }
        public virtual String Desc_Tipo_Usuario { get; set; }

        public virtual String PerfilAgente { get; set; }
         public virtual String Desc_Perfil_Vinculado { get; set; }


    }

    public class HistoricoAlteracaoPVincularUsuario_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
        public virtual String Observacao { get; set; }
    }

}