using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

        public class ChatBackofficeTecnicoListModel
    {
        public String numOs { get; set; }
        public String numObra { get; set; }
        public String numDoc { get; set; }
        public String Projeto { get; set; }
        public String CodCt { get; set; }
        public String dataHoraCriacao { get; set; }
        public String usuarioCriacao { get; set; }
        public ClienteChat cliente { get; set; }
        public String DataInicio { get; set; }
        public String DataFim { get; set; }
        public String num_fre { get; set; }
        public String tipoUsuario { get; set; }
        //public String Data { get; set; }
        //public String Hora { get; set; }
        //public StatusExecucaoAniel? Status_Execucao { get; set; }
        //public Status_AuditoriaAniel? Status_Auditoria { get; set; }
        public List<Mensagem_ChatBackTecnico> mensagens { get; set; }
        private List<AtendimentoUsuario> _usuarios;
        public List<AtendimentoUsuario> usuarios
        {
            get
            {
                return (_usuarios ?? new List<AtendimentoUsuario>());
            }
            set
            {
                _usuarios = value;
            }
        }

    }

    public class ClienteChat
    {
        public Int64? idCliente { get; set; }
        public String cpf_cnpj { get; set; }
        public String email { get; set; }
        public String telefone { get; set; }
        public String nome { get; set; }
        public Int64 idTipoAtendimento { get; set; }
        public Int64 idplanoProduto { get; set; }
        public Int64? idCategoriaCliente { get; set; }
        public Escritorio Escritorio_Area { get; set; }

    }

    public class Escritorio
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }

    }

    public class Mensagem_ChatBackTecnico
    {
        public string id { get; set; }
        public String numObra { get; set; }
        public String numDoc { get; set; }
        public String Projeto { get; set; }
        public String CodCt { get; set; }
        public String mensagem { get; set; }
        public String dataHora { get; set; }
        public StatusEnvioEnum statusEnvio { get; set; }
        public String usuario { get; set; }

        private IList<ArquivoMensagem_ChatBackTecnico> _arquivos;
        public virtual IList<ArquivoMensagem_ChatBackTecnico> arquivos
        {
            get
            {
                if (_arquivos == null)
                    _arquivos = new List<ArquivoMensagem_ChatBackTecnico>();
                return _arquivos;
            }
            set
            {
                _arquivos = value;
            }
        }

    }

    public class ArquivoMensagem_ChatBackTecnico
    {
        public String id { get; set; }
        public String name { get; set; }
        public String url { get; set; }
        public Tipo_Bot_Evento_Anexo tipo { get; set; }
    }

    public class ChatBackTecnico_SalvarAnexoModel
    {
        public virtual Mensagem_ChatBackOfficeModels mensagem { get; set; }
        public MemoryStream file { get; set; }
        public virtual string num_os { get; set; }
    }

    public class ChatBackTecnico_GetAnexoBytesModel
    {
        public virtual string num_os { get; set; }
        public virtual string idMensagem { get; set; }
        public virtual string fileExtension { get; set; }
        public virtual string equipe { get; set; }
        public virtual bool isUrl { get; set; }
    }
}