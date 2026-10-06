using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Models
{
    public class TermoAdesaoCadastroModel
    {
        public string NomeArquivo { get; set; }
        public string MensagemConcluido { get; set; }
    }

    public class TermoAdesaoMobile
    {
        public long IdVenda { get; set; }
        public string Cliente { get; set; }
        public string Plano { get; set; }
        public long? Fidelidade { get; set; }
        public string ValorPlano { get; set; }
        public string Vencimento { get; set; }
        public string ValorInstalacao { get; set; }
        public long? TesteGratis { get; set; }
        public string Empresa { get; set; }
        public string UrlLogo { get; set; }
        public string Cnpj { get; set; }
        public string EnderecoEmpresa { get; set; }
    }

    public class AssinaturaTermoAdesaoDados
    {
        public long IdVenda { get; set; }
        public bool AtualizarStatus { get; set; }
        public string Assinatura { get; set; }
    }


    public class ContratosList
    {
        public int Id { get; set; }
        public string Caminho { get; set; }
        public string Nome_Contrato { get; set; }     
    }
    public class AnexoContrato 
    {
        public long Id { get; set; }
        public string Url { get; set; } 
        public string NomeArquivo { get; set; }
        public string Extensao { get; set; }
    }

    public class TermoAdesaoFormularioModel
    {
        public long IdVenda { get; set; }
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string UrlLogo { get; set; }
        public string Termo { get; set; }
        public List<ContratosList> Contratos { get; set; }
        public List<AnexoContrato> AnexosContrato { get; set; }
        public bool Aceito { get; set; }
        public bool Aceito_Termo_Contrato { get; set; }
        public bool isIntegracao { get; set; }
        public bool ExigeFotoTermo { get; set; }
        public bool ExigeTermoContrato { get; set; }
        public bool Utiliza_Envio_Automatico_Contrato { get; set; }
        public bool TemAnexosDocumento { get; set; }
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

    public class TermoAdesaoFormularioConcluidoRequestModel
    {
        public long IdVenda { get; set; }
        public long TrocaId { get; set; }
        public string Cliente { get; set; }
        public string MensagemErro { get; set; }
        public bool IsAssinado { get; set; }
    }

    public class TermoAdesaoFormularioResponseModel
    {
        public bool IsAssinado { get; set; }
        public virtual Int64 IdVenda { get; set; }
    }

    public class TermoAdesaoFormularioConcluidoModel
    {
        public string Cliente { get; set; }
        public string Empresa { get; set; }
        public string UrlLogo { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public Int64 IdVenda { get; set; }
    }

    public class InfoEmpresaTermoAdesao
    {
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public byte[] Logo { get; set; }
        public string Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }

    public class GravarAnexoAvulsoModel
    {
        public HttpPostedFileBase arquivo { get; set; }
        public Int64 idLado { get; set; }
        public Int64 idTipoDoc { get; set; }
        public Int64 status { get; set; }
        public Int64? idVenda { get; set; }
        public Int64? idTroca { get; set; }
        public String ext { get; set; }
    }

    public class ExcluirTempTermo
    {
        public Int64 idVenda { get; set;}
    }

    public class ConsultaTermoAdesaoModel 
    {
        public bool IntegracaoPendente { get; set; }
    }
}