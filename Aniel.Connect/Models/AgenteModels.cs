using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class AgenteModel
    {
        public virtual List<Int64?> Categorias { get; set; }
        public virtual List<Escritorio_Area> Localidades { get; set; }
        public virtual String Foto { get; set; }
        public virtual long QtdeMaximaAtendimentos { get; set; }
        public virtual String InicioAtendimento { get; set; }
        public virtual String TerminoAtendimento { get; set; }
        public virtual String NomeAgente { get; set; }
        public virtual bool AtendeChat { get; set; }


    }

    public class AgenteChat
    {
        public virtual bool AtendeChat { get; set; }
        public virtual string? Departamento { get; set; }
    }

    public class AgenteListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome{ get; set; }
        public virtual String Status { get; set; }
    }
    public class AgenteCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Bot Bot { get; set; }
        public virtual Int64? IdBot { get; set; }

        private IEnumerable<AgenteCategoriaModel> categorias;

        public virtual IEnumerable<AgenteCategoriaModel> Categorias

        {
            get
            {
                if (categorias == null)
                    categorias = new List<AgenteCategoriaModel>();
                return categorias;
            }
            set
            {
                categorias = value;
            }
        }
        private IEnumerable<AgenteTipo_atendimentoModel> Tipo_atendimento;

        public virtual IEnumerable<AgenteTipo_atendimentoModel> tipo_atendimento

        {
            get
            {
                if (Tipo_atendimento == null)
                    Tipo_atendimento = new List<AgenteTipo_atendimentoModel>();
                return Tipo_atendimento;
            }
            set
            {
                Tipo_atendimento = value;
            }
        }

        private IEnumerable<AgenteLocalidadeModel> localidades;

        public virtual IEnumerable<AgenteLocalidadeModel> Localidades

        {
            get
            {
                if (localidades == null)
                    localidades = new List<AgenteLocalidadeModel>();
                return localidades;
            }
            set
            {
                localidades = value;
            }
        }

        private IEnumerable<AgenteUsuarioModel> usuarios;

        public virtual IEnumerable<AgenteUsuarioModel> Usuarios

        {
            get
            {
                if (usuarios == null)
                    usuarios = new List<AgenteUsuarioModel>();
                return usuarios;
            }
            set
            {
                usuarios = value;
            }
        }

        //Mensagens Padrões de Atendimento
        public virtual Boolean Perfil_Atende_Chat { get; set; }
        public virtual String Cliente_Aguardando_Atendimento { get; set; }
        public virtual String Inicio_Atendimento { get; set; }
        public virtual String Termino_Atendimento { get; set; }
        public virtual String Todos_Agentes_Offline { get; set; }
        public virtual Boolean Ativar_Inatividade { get; set; }
        public virtual String Mensagem_Notif_Conclusao_Inatividade { get; set; }
        public virtual String Mensagem_Enviada_Concluir_Inatividade { get; set; }
        public virtual Int32? Enviar_Notif_Cliente_Conclusao_Auto_Apos_x_Minutos { get; set; }
        public virtual Int32? Concluir_Atendi_Auto_Apos_x_Minutos_Envio_Notif_Inatividade { get; set; }
        public virtual Int32? Numero_Max_Atendi_Simultaneo_Por_Agente { get; set; }

    }

    public class AgenteCategoriaModel {
        public virtual Int64 Id{ get; set; }
        public virtual Int64 Id_Categoria { get; set; }
        public virtual String Categoria { get; set; }
    }
    public class AgenteTipo_atendimentoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Tipo_atendimento { get; set; }
        public virtual String Tipo_atendimento { get; set; }
    }

    public class AgenteLocalidadeModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Desc_Escritorio { get; set; }
        public virtual String Desc_Area { get; set; }
        public virtual String Desc_Localidade { get; set; }
        public virtual Boolean Todas_Localidades { get; set; }
        public virtual Boolean Todas_Areas { get; set; }
    }
    public class Escritorio_Area {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }

    }
    public class AgenteUsuarioModel
    {
        public virtual String Id { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String Usuario_Id { get; set; }

    }

    public class Agente_DataTableRequest : DataTableRequest
    {

    }
}