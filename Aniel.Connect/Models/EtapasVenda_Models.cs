using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Aniel.Connect.Models
{
    public class EtapasVenda_ListModel
    {
        public Int64 Id { get; set; }

        public Boolean Pendencia { get; set; }
        public String Supervisor { get; set; }
        public String Vendedor { get; set; }
        public String Departamento { get; set; }

        public String Plano { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Status { get; set; }

        public String StatusAtutomatico { get; set; }
        public Status_Venda StatusManual { get; set; }

        public String Data_Encerramento { get; set; }
        public String DataHoraAprovacao { get; set; }
        public String DataHoraCancelamento { get; set; }
        public String DiasSemContato { get; set; }

        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeFantasia { get; set; }

        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string CpfCnpj { get; set; }

        [Display(Name = "CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria { get; set; }

        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        [Display(Name = "ESTADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Estado { get; set; }

        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Cidade { get; set; }

        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Bairro { get; set; }
        public String Logradouro { get; set; }
        public String Numero { get; set; }
        public String Complemento { get; set; }

        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Tipo_Imovel { get; set; }
        public string Data_Expiracao { get; set; }
        public string Avaliacao_Expirada { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Categoria_Cliente { get; set; }
        public Perfil_Vendedor Perfil_Vendedor_Id { get; set; }
        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Forma_Pagamento { get; set; }

        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Identificacao_Cliente { get; set; }

        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }
        public String TelefoneCelular { get; set; }
        public String Email { get; set; }
        public String Profissao { get; set; }

        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataVenda { get; set; }
        public String DataUltimoContato { get; set; }

        [Display(Name = "CAMPANHA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Campanha { get; set; }
        public String Meio_Captacao { get; set; }

        public string Quem_Indicou { get; set; }
        public String Data_Hora_Criacao { get; set; }
        public String Tipo_Cliente { get; set; }
        public String Tipo_Pessoa { get; set; }

        public Int64 Id_Cliente { get; set; }

        [Display(Name = "TERMO_ADESAO_ASSINADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Termo_Adesao_Assinado { get; set; }
        public String Data_Termo_Assinado { get; set; }
        public String EquipeVenda { get; set; }
        public String Setor_Responsavel { get; set; }
        public String Responsavel { get; set; }

        public String Etapa_Atual { get; set; }
        public String Motivo_Desistencia_Venda { get; set; }
        public String Checklist { get; set; }
        public String Data_Preenchimento_Checklist { get; set; }
        public String DadosGerais { get; set; }
        public String Financeiro { get; set; }
        public String ConsultaViabilidade { get; set; }
        public String PendenciaEndereco { get; set; }
        public String PendenciaFinanceira { get; set; }
        public String ConsultaCredito { get; set; }
        public String PlanoProduto { get; set; }
        public String EtapaChecklist { get; set; }
        public String Resumo { get; set; }
        public String TermoAdesao { get; set; }
        public String OrdemServico { get; set; }
    }

    public class EtapasVenda_CadastroModel : IViewModel<Int64>
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

    public class EtapasVenda_ConfiguracaoGeralModel : IViewModel<Int64>
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

    public class EtapasVenda_CustomizacaoModel : IViewModel<Int64>
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

    public class EtapasVenda_DataTableRequest : DataTableRequest
    {
        public String StatusAuditoria { get; set; }
        [Venda_Modelo(Filtro_PainelVendaEnum.StatusVenda)]
        public String Status { get; set; }
        public String Nome { get; set; }
        public String StatusVenda { get; set; }

        public String Estado { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Cidade)]
        public String Cidade { get; set; }

        public String Bairro { get; set; }
        public String Departamento { get; set; }

        public String Data_Inicial { get; set; }

        public Int32? TipoData { get; set; }

        public String Data_Final { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Vendedor)]
        public String Vendedor { get; set; }

        [Venda_Modelo(Filtro_PainelVendaEnum.Supervisor)]
        public String Supervisor { get; set; }
        public Int64? Id_Cliente { get; set; }
        public String AcessoUsuario { get; set; }
        public String AcessoProspect { get; set; }
        public String acessoTempoVisualização { get; set; }
        public String AcessoDepartamento_Setor { get; set; }
        public String EquipeVenda { get; set; }
        public String MotivoAprovacao { get; set; }
        public String Etapa_Atual { get; set; }
        public String Setor_Responsavel { get; set; }
        public String Responsavel { get; set; }

        public List<Int64?> StatusDadosGerais { get; set; } = new List<Int64?>();
        public List<Int64?> StatusPendenciaEndereco { get; set; } = new List<Int64?>();
        public List<Int64?> StatusPendenciaFinanceira { get; set; } = new List<Int64?>();
        public List<Int64?> StatusConsultaViabilidade { get; set; } = new List<Int64?>();
        public List<Int64?> StatusTermoAdesao { get; set; } = new List<Int64?>();
        public List<Int64?> StatusChecklist { get; set; } = new List<Int64?>();
        public List<Int64?> StatusConsultaCredito { get; set; } = new List<Int64?>();
        public List<Int64?> StatusFinanceiro { get; set; } = new List<Int64?>();
        public List<Int64?> StatusResumo { get; set; } = new List<Int64?>();
        public List<Int64?> StatusPlanoProduto { get; set; } = new List<Int64?>();
        public List<Int64?> StatusOrdemServico { get; set; } = new List<Int64?>();
    }
}
