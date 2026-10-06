
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    public class Cliente_ProspectoDadosGeraisViewModel : IViewModel<Int64>
    {
        public Cliente_ProspectoDadosGeraisViewModel()
        {
            EnderecoPrincipal = new EnderecoClienteViewModel { Tipo = Connect.Model.enumerado.Tipo_EnderecoCliente.Principal };
            EnderecoInstalacao = new EnderecoClienteViewModel { Tipo = Connect.Model.enumerado.Tipo_EnderecoCliente.Instalacao };
            EnderecoCobranca = new EnderecoClienteViewModel { Tipo = Connect.Model.enumerado.Tipo_EnderecoCliente.Cobranca };
        }

        public Configuracoes_Gerais configGerais { get; set; }

        public Int64 Id { get; set; }
        public string NumeroValidado { get; set; }


        //DADOS GERAIS****************************************************************************************************

        [Display(Name = "SITE", ResourceType = typeof(Resources.RsCadastros))]
        public String Site { get; set; }

        [Display(Name = "EMAIL_AUX", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Email_Aux { get; set; }

        [Display(Name = "TELEFONE_FIXO2", ResourceType = typeof(Resources.RsCadastros))]
        public String Telefone_Fixo2 { get; set; }

        [Display(Name = "RESPONSAVEL_FINANCEIRO", ResourceType = typeof(Resources.RsCadastros))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Responsavel_Financeiro { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Modelo_Contrato { get; set; }

        [Display(Name = "MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        public String Numero_Contrato { get; set; }

        [Display(Name = "COD_AREA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Area { get; set; }

        [Display(Name = "COD_ESCRITORIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Escritorio { get; set; }
        public String Informacao_Complementares { get; set; }
        
        public String Regime { get; set; }


        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cod_Localidade { get; set; }


        [Display(Name = "COD_AREA", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Desc_Cod_Area { get; set; }

        [Display(Name = "COD_ESCRITORIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Desc_Cod_Escritorio { get; set; }

        [Display(Name = "COD_LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Desc_Cod_Localidade { get; set; }

        #region Fisico

        public Connect.Model.enumerado.Tipo_Cliente? TipoCliente { get; set; }

        public Tipo_Pessoa? TipoPessoa { get; set; }

        [Display(Name = "CPF", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cpf { get; set; }

        [Display(Name = "CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Cnpj { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_Captacao { get; set; }

        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String Meio_CaptacaoJuridico { get; set; }

        [Display(Name = "COMO_FICOU_SABENDO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_Captacao { get; set; }
        public Int64? Id_Campanha { get; set; }

        [Display(Name = "COMO_FICOU_SABENDO", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Meio_CaptacaoJuridico { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public Boolean Status { get; set; }

        [Display(Name = "STATUS_PROSPECTO", ResourceType = typeof(Resources.RsCadastros))]
        public Connect.Model.enumerado.Status_Prospecto? Status_Prospecto { get; set; }

        [Display(Name = "NOMECOMPLETO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeCompleto { get; set; }
        public Int64? Id_Departamento { get; set; }
        public Boolean End_Cobranca_Igual_Principal { get; set; }
        public Boolean End_Instalacao_Igual_Principal { get; set; }
        public Boolean End_CadastroCondominioProspecto { get; set; }
        public Connect.Model.enumerado.Enum_MotivoRestricaoComercial MotivoRestricaoComercial { get; set; }

        public EnderecoClienteViewModel EnderecoPrincipal { get; set; }
        private EnderecoClienteViewModel _enderecoProspecto { get; set; }
        public EnderecoClienteViewModel EnderecoProspecto
        {
            get
            {
                if (_enderecoProspecto == null)
                {
                    _enderecoProspecto = new EnderecoClienteViewModel();
                }

                _enderecoProspecto.Tipo = Connect.Model.enumerado.Tipo_EnderecoCliente.Prospecto;
                return _enderecoProspecto;

            }
            set
            {
                _enderecoProspecto = value;
            }
        }


        public EnderecoClienteViewModel EnderecoInstalacao { get; set; }

        public EnderecoClienteViewModel EnderecoCobranca { get; set; }

        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public String DataNascimento { get; set; }


        [Display(Name = "NACIONALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Nacionalidade { get; set; }

        [Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Sexo? Sexo { get; set; }

        [Display(Name = "ESTADOCIVIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Estado_Civil? EstadoCivil { get; set; }

        [Display(Name = "RG", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string RG { get; set; }

        [Display(Name = "DATAEMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string DataEmissao { get; set; }

        [Display(Name = "ORGAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Orgao { get; set; }

        [Display(Name = "PAI", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Pai { get; set; }

        [Display(Name = "MAE", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Mae { get; set; }

        [Display(Name = "TELEFONEFIXO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneFixo { get; set; }

        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Telefone_Celular { get; set; }

        [Display(Name = "TELEFONERECADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefoneRecado { get; set; }

        [Display(Name = "NOMERECADO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string NomeRecado { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string TelefonePrincipal { get; set; }

        [Display(Name = "INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Grau_Instrucao? Instrucao { get; set; }

        [Display(Name = "PROFISSAO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(25, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Profissao { get; set; }

        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public string Email { get; set; }

        [Display(Name = "MOTIVO_INVIABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? IdMotivoInviabilidade { get; set; }




        #endregion

        #region Juridico

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String NomeFantasia { get; set; }
        public DateTime? Data_Abertura { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Razao_Social { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Inscricao_Estadual { get; set; }

        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Inscricao_Municipal { get; set; }
        public Porte? Porte { get; set; }
        public String Representante { get; set; }

        [MaxLength(30, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Cargo_Responsavel { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public String Ramo_Atividade { get; set; }
        public String CPFResponsavel { get; set; }


        #endregion

        //PLANOS E PRODUTOS****************************************************************************************************
        #region
        [Display(Name = "QUAL_MEIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Qual_Meio { get; set; }

        [Display(Name = "QUAL_MEIO", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Qual_MeioJuridico { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Quem_Indicou { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public string Quem_IndicouJuridico { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Quem_Indicou { get; set; }

        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        [DataType(DataType.Text)]
        public Int64? Id_Quem_IndicouJuridico { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Categoria_Cliente { get; set; }

        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        public Int64? Id_Categoria_ClienteJuridico { get; set; }

        #endregion

        public bool ConsultaViabilidadeHabilitada { get; set; }
        public bool ConsultaPendenciaFinanceiraHabilitada { get; set; }
        public bool ConsultaPendenciaEnderecoHabilitada { get; set; }
        public bool ConsultaCreditoHabilitada { get; set; }

        public StatusEtapaVenda StatusConsultaViabilidade { get; set; }
        public StatusEtapaVenda StatusConsultaPendenciaFinanceira { get; set; }
        public StatusEtapaVenda StatusConsultaPendenciaEndereco { get; set; }
        public StatusEtapaVenda StatusConsultaCredito { get; set; }
        public bool ConsultaCreditoVendedorVisualiza { get; set; }
        public bool ConsultaViabilidadeVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaFinanceiraVendedorVisualiza { get; set; }
        public bool ConsultaPendenciaEnderecoVendedorVisualiza { get; set; }
        public int? ConsultaPendenciaViabilidadeValidade { get; set; }
        public int? ConsultaPendenciaEnderecoValidade { get; set; }
        public System.DateTime? DataHoraCriacaoConsultaEndereco { get; set; }
        public List<ClientesAtreladosEndereco> ClientesAtrelados { get; set; }
    }
}
