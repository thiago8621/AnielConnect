using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Perfil_Agente : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        private IList<Agente_Categoria> _categorias;
        private IList<Agente_Tipo_atendimento> _Tipo_Atendimento;
        private IList<Agente_Localidade> _localidades;
        

        public virtual IList<Agente_Categoria> Categorias
        {
            get
            {
                if (_categorias == null)
                    _categorias = new List<Agente_Categoria>();
                return _categorias;
            }
            set
            {
                _categorias = value;
            }
        }
        public virtual IList<Agente_Tipo_atendimento> Tipo_atendimento
        {
            get
            {
                if (_Tipo_Atendimento == null)
                    _Tipo_Atendimento = new List<Agente_Tipo_atendimento>();
                return _Tipo_Atendimento;
            }
            set
            {
                _Tipo_Atendimento = value;
            }
        }

        public virtual IList<Agente_Localidade> Localidades
        {
            get
            {
                if (_localidades == null)
                    _localidades = new List<Agente_Localidade>();
                return _localidades;
            }
            set
            {
                _localidades = value;
            }
        }

        //Chat - Mensagens Padrões de Atendimento
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
        public virtual Bot Bot { get; set; }

    }
}
