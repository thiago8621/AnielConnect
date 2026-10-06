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

namespace Aniel.Connect.Models
{
    public class BotEvento_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "iDescricao", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Descricao { get; set; }

        public virtual string Status { get; set; }

        public virtual string Disparo { get; set; }
        public virtual string Inicial { get; set; }
        public virtual string AcaoExecutada { get; set; }
        public virtual string PesquisaSatisfacao { get; set; }
        public virtual string SetorAtendimento { get; set; }
        public virtual Boolean AtivarAcao { get; set; }
        public virtual Boolean CondicaoEvento { get; set; }
    }


    //elementos carregados na pagina cadastro / alteracao
    public class BotEvento_CadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual String Descricao { get;  set; }
        public virtual bool Disparo { get;  set; }
        public virtual bool Inicial { get;  set; }
        public virtual Boolean CondicaoEvento { get;  set; }
        public virtual String TextoEnviado { get; set; }
        public virtual String NomeBot { get; set; }

        public virtual Boolean AtivarAcao { get; set; }
        public virtual Int64? IdBot { get; set; }
        public virtual Int64? AutoProximoEvento { get; set; }
        public virtual Int64? EventoFalso { get; set; }
        public virtual Int64? EventoVerdadeiro { get; set; }
        public virtual Tipo_Bot_Evento_Acoes? AcaoExecutada { get; set; }
        public virtual EventoArmazenamento? EventoArmazenamento { get; set; }
        public virtual String VariaveisDisponiveis { get; set; }
        public virtual Int64? PesquisaSatisfacao { get; set; }
        public virtual Int64? SetorAtendimento { get; set; }

        private IEnumerable<BotEventoAnexo_ListModel> _anexos { get; set; }
        public virtual IEnumerable<BotEventoAnexo_ListModel> Anexos
        {
            get
            {
                if (_anexos == null)
                    _anexos = new List<BotEventoAnexo_ListModel>();
                return _anexos;
            }
            set
            {
                _anexos = value;
            }
        }

        private IEnumerable<BotEventoResposta_ListModel> _respostas { get; set; }
        public virtual IEnumerable<BotEventoResposta_ListModel> Respostas
        {
            get
            {
                if (_respostas == null)
                    _respostas = new List<BotEventoResposta_ListModel>();
                return _respostas;
            }
            set
            {
                _respostas = value;
            }
        }


        public virtual List<HttpPostedFileBase> FilesAnexoImagem { get; set; }
        public virtual List<HttpPostedFileBase> FilesAnexoVideo { get; set; }
        public virtual List<HttpPostedFileBase> FilesAnexoAudio { get; set; }
        public virtual List<HttpPostedFileBase> FilesAnexoArquivo { get; set; }
        public virtual string FilesAnexoUrl { get; set; }
    }

    public class BotEvento_ConfiguracaoGeralModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }
    }

    public class BotEvento_CustomizacaoModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "AVATAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Avatar { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string Nome { get; set; }

        [MaxLength(400, ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "MAXLENGHT")]
        [Display(Name = "TEXTO_INVALIDO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual string TextoInvalido { get; set; }
        public virtual Boolean Transferencia { get; set; }
        public virtual int? Minutos { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Habilitado { get; set; }

        public virtual List<HttpPostedFileBase> Files { get; set; }
    }

    public class BotEvento_DataTableRequest : DataTableRequest
    { 
        public virtual string Descricao { get; set; }
        public virtual String IdBot { get; set; }
        public virtual String Disparo { get; set; }
        public virtual String Status { get; set; }
        public virtual String Inicial { get; set; }
   
    }
    
    public class BotEventoAnexo_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "iNome", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Nome { get; set; }

        public virtual string Tipo { get; set; }

        public virtual string Extensao { get; set; }
        public virtual string Url { get;  set; }
    }

    public class BotEventoResposta_ListModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Descricao { get; set; }

        public virtual Int64? IdProximoEvento { get; set; }
    }

    public class BotEventoAnexo_DataTableRequest : DataTableRequest
    {
        
        public virtual string Nome { get; set; }
        public virtual String IdEvento { get; set; }
        public string IdAnexo { get; internal set; }
    }

    public class BotEventoResposta_DataTableRequest : DataTableRequest
    {

        public virtual string Resposta { get; set; }
        public virtual Int64 IdProximoEvento { get; set; }
        public string IdResposta { get; internal set; }
    }
}
