using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Models
{
    public class Documento_ProducaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Cliente { get; set; }

        public virtual String Numero_OS { get; set; }

        public virtual String Id_PlanoProduto { get; set; }
        public virtual String PlanoProduto { get; set; }
       

        public virtual String Cliente { get; set; }

        public virtual String Cpfcnpj_Cliente { get; set; }

        public virtual String Tempo_Cadastro { get; set; }

        public virtual String Categoria_Cliente { get; set; }

        public virtual String Telefone_Celular { get; set; }

        public virtual String Email { get; set; }

        public virtual String Tipo_Imovel { get; set; }

        public virtual String Cep { get; set; }

        public virtual String Id_Endereco_Cliente { get; set; }

        public virtual String Endereco_Cliente { get; set; }

        public virtual String Logradouro { get; set; }

        public virtual String Telefone_Recado { get; set; }
        public virtual String Telefone_Fixo { get; set; }

        public virtual String Bairro { get; set; }

        public virtual String Estado { get; set; }

        public virtual String Complemento { get; set; }
        public virtual String Uf { get; set; }

        public virtual String Ponto_Referencia_Cliente { get; set; }

        public virtual List<PlanoProdutoComboAntendimentoViewModel> PlanosProdutos { get; set; }
    }

    public class Documento_ProducaoChaveModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
    }
    public class AlterarCaixaModel
    {
        public String porta { get; set; }
        public String caixa { get; set; }
        public String portaAntinga { get; set; }
        public String codct { get; set; }
        public String projeto { get; set; }
        public String num_obra { get; set; }
        public int num_doc { get; set; }

    }


    public class Documento_ProducaoCadastroModel
    {
        public virtual Int64? Id_Atendimento { get; set; }
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }

        public virtual String Num_Obra { get; set; }

        public virtual Int16? Id_Tipo_servico { get; set; }
        public virtual String Id_SubTipo_servico { get; set; }
        public Boolean DefinirTipo_SubTipoOS { get; set; }
        public long? Id_Tipo_ServicoOS { get; set; }
        public long? Id_Subtipo_ServicoOS { get; set; }
        public virtual Boolean TemResponsavel { get; set; }

        public virtual String Id_Responsavel { get; set; }
        public virtual String Exige_Pagamento_Instalacao { get; set; }

        public virtual String Data_Agendamento { get; set; }
        public virtual String Hora_Agendamento { get; set; }

        public virtual String Responsavel { get; set; }

        public virtual String Segmento { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Contrato { get; set; }
        public virtual Int64? IdDepartamento { get; set; }

        public virtual Origem_Documento_Producao? Origem_OS { get; set; }

        
        public virtual Int64? Id_Cliente { get; set; }

        public virtual String Nome_Cliente { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        [MaxLength(890, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        [AllowHtml]
        public virtual String Observacao_OS { get; set; }

        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual String Localidade { get; set; }

        public virtual Tipo_Imovel Tipo_Imovel { get; set; }

        public virtual Int64? Id_Tipo_Imovel { get; set; }
        public virtual Int64? Id_Tipo_Imovel_Manut { get; set; }

        public virtual String Desc_Tipo_Imovel { get; set; }

        public virtual StatusExecucaoAniel? Status { get; set; }

        public virtual Status_CobrancaAniel? Status_Cobranca { get; set; }

        public virtual String Identificacao_Cliente { get; set; }
        public virtual String Data_Cadastro_Cliente { get; set; }

        public virtual String Categoria_Cliente { get; set; }
        public virtual long? Id_Categoria_Cliente { get; set; }

        public virtual String Telefone_Fixo { get; set; }

        public virtual String Email_Principal { get; set; }

        public virtual String Telefone_Celular { get; set; }

        public virtual Int64? Id_Endereco { get; set; }

        public EnderecoClienteViewModel EnderecoPrincipal { get; set; }

        public virtual String Logradouro { get; set; }
        public virtual String Bloco { get; set; }
        public virtual String Apartamento { get; set; }
        public virtual String CEP { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }

        public virtual String Ponto_Referencia { get; set; }

        public virtual String CPF_CNPJ_Cliente { get; set; }

        public virtual String Adcional_1 { get; set; }

        public virtual String Adcional_2 { get; set; }

        public virtual String Adcional_3 { get; set; }

        public virtual String Adcional_4 { get; set; }

        public virtual String Adcional_5 { get; set; }

        public virtual String Adcional_6 { get; set; }

        public virtual String Adcional_7 { get; set; }

        public virtual String Adcional_8 { get; set; }

        public virtual String Adcional_9 { get; set; }

        public virtual String Adcional_10 { get; set; }
        public virtual Boolean UtilizaEndeCondo { get; set; }
        public virtual long? IdCondominioPredioEndereco { get; set; }

        public virtual Int64? Id_Venda { get; set; }
        public virtual Int64? Id_Troca_Plano { get; set; }
        public virtual String NomeInstalacao { get; set; }
        public virtual String CpfInstalacao { get; set; }
        public virtual String TelefoneInstalacao { get; set; }
        public virtual Tipo_PrioridadeT5? Tipo_Prioridade { get; set; }
        public virtual String Tipo { get; set; }
        public virtual String TipoManutencao { get; set; }
        public virtual Int64? Id_Pop { get; set; }
        public virtual Int64? Id_Olt { get; set; }
        public virtual Int64? Id_Elemento_Rede { get; set; }
        public virtual Tipo_PrioridadeT5? Prioridade_Cliente { get; set; }
        public virtual bool CadastrarNovoCliente { get; set; }
        public Tipo_Pessoa? TipoPessoa { get; set; }
    }
    public class Documento_ProducaoBaixaModel
    {
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32? Num_Doc { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Codigo_Encerramento { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Data_Ini_Atend { get; set; }
        public virtual String Data_Fim_Atend { get; set; }
        public virtual String Hora_Ini_Atend { get; set; }
        public virtual String Hora_Fim_Atend { get; set; }
        public virtual String CaixaFTTH { get; set; }
        public virtual String PortaFTTH { get; set; }
        public virtual String Usuario { get; set; }
        public virtual String BaixaOsProdDuplicada { get; set; }
    }

    
    public class Documento_Producao_DataTableRequest : DataTableRequest
    {
        public String Plano_Produto { get; set; }

        public String Cpfcnpj_Cliente { get; set; }
    }

    public class ConsultaClienteDocumentoProducaoModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Numero_OS { get; set; }

        public virtual String Cliente { get; set; }

        public virtual String Plano { get; set; }

        public virtual String Produto { get; set; }


    }

    public class Relatorio_Producao_SinteticoModel
    {
        public virtual String equipe { get; set; }
        public virtual String nome_equipe { get; set; }
        public virtual String producao_acumulada { get; set; }
        public virtual String valor_producao { get; set; }
        public virtual String faixa_aplicada { get; set; }
        public virtual String valor_final_producao { get; set; }
        public virtual String contrato_aniel { get; set; }
        public virtual String projeto { get; set; }
        public virtual String supervisor { get; set; }
        public virtual String desc_tipo_serv { get; set; }
        public virtual String mes_ano { get; set; }
    }

    public class MotivoParalizacaoRequest
    {
        public virtual string CodCt { get; set; }
        public virtual string NumObra { get; set; }
        public virtual int Contrato { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Tecnico { get; set; }
    }

    public class MotivoParalizacaoResponse
    {
        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual string MotivoParalizacao { get; set; }
        public virtual string NumObra { get; set; }
        public virtual string TecnicoOs { get; set; }
        public virtual string Tecnico { get; set; }
    }

    public class DadosOSMobile
    {
        public virtual Int64? Id_PlanoProduto_Venda { get; set; }
        public virtual String Descricao_PlanoProduto_Venda { get; set; }
        public virtual String CodCt_OS { get; set; }
        public virtual String Projeto_OS { get; set; }
        public virtual String Num_Obra_OS { get; set; }
        public virtual Int32? Num_Doc_OS { get; set; }
        public virtual String Numero_OS { get; set; }
        public virtual String Status_OS { get; set; }
        public virtual String Tipo_Serv_OS { get; set; }
        public virtual String PlanoProduto_OS { get; set; }
        public virtual String Endereco_Cliente_OS { get; set; }
        public virtual String DataHora_Agendamento_OS { get; set; }
    }

    public class CriarOSMobileRequest
    {
        public virtual Int64? IdVenda { get; set; }
        public virtual Int64? IdPlanoProduto { get; set; }
    }

    public class Documento_ProducaoCriarOSMobile
    {
        public virtual String Num_Obra { get; set; }

        public virtual long? Id_Tipo_servico { get; set; }
      

        public virtual long? Id_SubTipo_servico { get; set; }

        public virtual String Id_Responsavel { get; set; }

        public virtual String Segmento { get; set; }

        public virtual Int64? Id_Cliente { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        [MaxLength(890, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Observacao_OS { get; set; }

        public virtual String Cod_Escritorio { get; set; }

        public virtual String Cod_Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }

        public virtual Int64? Id_Venda { get; set; }
    }

    public class ServicoCobrancaAlterarStatusViewModel
    {
        public Int64 id { get; set; }      
        public Int16 status { get; set; }
        public String Usuario { get; set; }
        public String justificativa { get; set; }
        public String status_atual { get; set; }
        public long IdCliente { get; set; }
        public virtual String CodCt { get; set; }

        public virtual String Projeto { get; set; }
         public virtual String Num_Obra { get; set; }

        public virtual Int32? Num_Doc { get; set; }
        public long IdPlanoProduto { get;  set; }
        public string Motivo { get;  set; }
    }


    
    [ExcelServiceKey("OrdemServico")]
    public class ExclusaoServicoPlanilha : IExcelService
    {
        [Required]
        [ExcelService("Contrato")]
        public virtual String Contrato { get; set; }

        [Required]
        [ExcelService("Projeto")]
        public virtual String Projeto { get; set; }

        //[Required]
        //[ExcelService("Número do Cliente")]
        //public virtual String NumeroCliente { get; set; }

        [Required]
        [ExcelService("Identificação do Cliente")]
        public virtual String IdentificacaoCliente { get; set; }

        [Required]
        [ExcelService("Ordem de Serviço")]
        public virtual String OrdemServico { get; set; }

        [Required]
        [ExcelService("Data de Criação")]
        public virtual String DataCriacao { get; set; }
        public bool Importar { get; set; }
    }

    public class LogExclusaoOS
    {
        public virtual string data_exclusao { get; set; }
        public virtual string hora_exclusao { get; set; }
        public virtual string usuario_exclusao { get; set; }
        public virtual string codct { get; set; }
        public virtual string projeto { get; set; }
        public virtual string num_doc { get; set; }
        public virtual string num_obra { get; set; }
        public virtual string num_obra_original { get; set; }
        public virtual string data_maxima { get; set; }
        public virtual string usuario_criacao { get; set; }
        public virtual string data_criacao { get; set; }
        public virtual string hora_criacao { get; set; }
        public virtual string nome_maquina { get; set; }
        public virtual string ip_maquina { get; set; }
        public virtual string obs { get; set; }
    }
}