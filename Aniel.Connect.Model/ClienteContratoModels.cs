using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Model
{
    public class ClienteContratoModels
    {
    }


    public class ContratoFormularioClienteModel
    {
        public long IdContrato { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string UrlLogo { get; set; }
        public string Termo { get; set; }
        public bool Aceito { get; set; }
        public bool isIntegracao { get; set; }
        public bool ExigeFotoTermo { get; set; }
        public string Acoes_Prova_Vida { get; set; }
        public bool Permite_Anexar_Doc { get; set; }
        public Tipo_Comprovante_Termo? TipoComprovanteTermo { get; set; }
        public Tipo_Doc_Termo_Anexo? TipoDocumento { get; set; }

        private IEnumerable<DocumentoAnexo_ListModel> _anexos { get; set; }
        public virtual IEnumerable<DocumentoAnexo_ListModel> Anexos
        {
            get
            {
                if (_anexos == null)
                    _anexos = new List<DocumentoAnexo_ListModel>();
                return _anexos;
            }
            set
            {
                _anexos = value;
            }
        }

        public virtual List<HttpPostedFileBase> FilesAnexoFrente { get; set; }
        public virtual HttpFileCollectionBase FilesSelfie { get; set; }
        public virtual string InputBase64Vio { get; set; }
        public virtual string InputBase64Content { get; set; }
        public virtual string Base64 { get; set; }
        public virtual string InputBase64Type { get; set; }
        public virtual List<HttpPostedFileBase> FilesAnexoVerso { get; set; }
    }

    public class DocumentoAnexo_ListModel
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "iNome", ResourceType = typeof(Resources.RsConfiguracao))]
        public virtual string Nome { get; set; }

        public virtual string Tipo { get; set; }

        public virtual string Extensao { get; set; }
        public virtual string Url { get; set; }
    }


    public class ContratoFormularioCliModelConcluidoRequestModel
    {
        public long IdContrato { get; set; }
        public string Cliente { get; set; }
        public string MensagemErro { get; set; }
        public bool IsAssinado { get; set; }
    }

    public class ContratoFormularioCliResponseModel
    {
        public bool IsAssinado { get; set; }
        public virtual Int64 IdContrato { get; set; }
    }

    public class ContratoFormularioCliConcluidoModel
    {
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string UrlLogo { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public Int64 IdContrato { get; set; }
    }

    public class ExcluirTempContrato
    {
        public Int64 idContrato { get; set; }
    }

    public class ConsultaContratoModel
    {
        public bool IntegracaoPendente { get; set; }
    }

}
