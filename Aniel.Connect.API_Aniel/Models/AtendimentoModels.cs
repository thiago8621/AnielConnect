using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tipo_Atendimento = Aniel.Connect.Model.nhibernate.Tipo_Atendimento;

namespace Aniel.Connect.API_Aniel.Models
{
    public class AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Numero { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }

        public virtual String Status { get; set; }

        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual String Assunto { get; set; }

        public virtual String Pessoa_Contato { get; set; }



        public virtual String Cliente { get; set; }
        public virtual Int64? Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Origem { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual String Tipo_Atendimento { get; set; }

        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }

        public virtual String DataHoraFechamento { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Status_Atendimento { get; set; }

        public virtual String Sla { get; set; }

        public virtual Int32? SlaOrdem { get; set; }

        public virtual String Login_Radius { get; set; }
        public virtual String Email_Cliente { get; set; }
        public virtual String Telefone { get; set; }
        public virtual String Celular { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Produto { get; set; }
        public virtual String Id_Motivo_Status { get; set; }

        public virtual Boolean Tecnico_Ativos { get; set; }

        public virtual Boolean Tecnico_Em_Execucao { get; set; }
    }

    public class AcompanhamentoAgenteListaModel
    {

        public virtual String Status_Chat { get; set; }

        public virtual String Agente { get; set; }
        public virtual String Motivo { get; set; }

        public virtual Int64? Qtd_Atendimento { get; set; }

        public virtual String Setor { get; set; }

        public virtual Int64? Numero_Max_Atendimento { get; set; }
    }

    public class AtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Status_Cliente? Cliente_Status { get; set; }

        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Nome_Cliente { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String CPF_CNPJ_Cliente { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Data_Cadastro_Cliente { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Categoria_Cliente { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Fixo { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Celular { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Telefone_Recado { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Email_Principal { get; set; }


        public virtual String Id_Motivo_Status { get; set; }
        public virtual String Desc_Motivo_Status { get; set; }

        public virtual String Forma_Pagamento_Utilizada { get; set; }

        public virtual String Endereco_Principal { get; set; }
        public virtual Int64? Id_Endereco { get; set; }

        // Plano/Produto        
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //         ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "ID", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Int64? Id_Cliente_Plano_Produto { get; set; }

        public virtual Int64? Id_Plano_Produto { get; set; }

        public virtual String Nome_Plano_Produto { get; set; }

        [Display(Name = "LOGIN_DO_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        public virtual String Login_Radius { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Endereco_Instalacao { get; set; }

        public virtual String Valor_Final_Plano { get; set; }



        // Sobre o Atendimento 
        public virtual String Numero { get; set; }

        public virtual String Protocolo { get; set; }

        public virtual Int64? Id_Origem_Atendimento { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Assunto { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }

        public virtual Boolean Criar_Os_Encerrar { get; set; }

        public virtual String Criar_Os_Encerrar_Parametro_Controller { get; set; }


        public virtual Int64? Id_Categoria_Atendimento { get; set; }

        public virtual Int64? Id_Setor { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Id_Responsavel { get; set; }

        public virtual Prioridade_Atendimento Prioridade { get; set; }

        public virtual Tipo_Pessoa_Contato Tipo_Pessoa_Contato { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Pessoa_Contato { get; set; }

        //public virtual String Grau_Ligacao { get; set; }

        public virtual Grau_Ligacao? Grau_Ligacao { get; set; }

        public virtual String Nome_Outra_Pessoa_Contato { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Problema_Relatado { get; set; }

        public virtual Int64? Id_Acao { get; set; }

        [MaxLength(800, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Ultima_Acao { get; set; }


        public virtual String Arquivo { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Resources.RsBase),
               ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual List<ArquivoAcaoModel> Arquivos { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Data_Hora_Criacao { get; set; }

        public virtual String Motivo_Status { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Status_Atendimento Status_Atendimento { get; set; }

        public virtual Int32 Tipo_Cliente { get; set; }

        public virtual String Cliente_Ha { get; set; }
        public virtual Int64? Id_Email_Abertura { get; set; }
        public virtual Boolean AcaoGeraEmail { get; set; }

        //public virtual String Login_Radius { get; set; }
    }

    public class AtendimentoOla
    {
        public virtual String Observacao { get; set; }
        public virtual StatusExecucaoAniel? StatusAtendimento { get; set; }
        public virtual String DataMaxima { get; set; }
        public virtual String HoraMaxima { get; set; }
        public virtual Int64 IdCliente { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }
        public virtual DateTime? DiaCriacao { get; set; }
        public virtual DateTime? HoraCriacao { get; set; }
        public virtual String Num_Obra { get; set; }

    }


    public class AtendimentoFiltroModel
    {
        public virtual Int64 Id_Tipo_Atend { get; set; }
        public virtual String Tipo_Atendimento { get; set; }


        public virtual Int64 Id_Categoria_Cliente { get; set; }
        public virtual String Categoria_Cliente { get; set; }

        public virtual Int64 Id_Categoria_Atendimento { get; set; }
        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Setor { get; set; }
        public virtual String Setor { get; set; }


        public virtual Status_Atendimento? Status_Atendimento { get; set; }

        public virtual Int64 Id_Motivo_Status { get; set; }
        public virtual String Motivo_Status { get; set; }


        public virtual Int64? Num_Os { get; set; }


        //Período
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }


        //Prioridade de Atendimento
        public virtual Boolean Baixa { get; set; }
        public virtual Boolean Media { get; set; }
        public virtual Boolean Alta { get; set; }


        //SLA de Atendimento
        public virtual Boolean Dentro_Prazo { get; set; }
        public virtual Boolean Atencao { get; set; }
        public virtual Boolean Vencimento_Proximo { get; set; }
        public virtual Boolean Risco_Perda_Prazo { get; set; }
        public virtual Boolean Prazo_Vencido { get; set; }
    }

    public class AtendimentoMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Motivo { get; set; }
        public virtual String Desc_Motivo { get; set; }
    }

    public class AtendimentoHistoricoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Numero { get; set; }
        public virtual String Assunto { get; set; }

        public virtual String Tipo_Atendimento { get; set; }


        public virtual String Data_Limite { get; set; }

        public virtual String Setor { get; set; }

        public virtual String Extensao { get; set; }

        public virtual String Responsavel { get; set; }

        public virtual Status_Atendimento Status { get; set; }

        private IEnumerable<AtendimentoHistoricoAcoesCadastroModel> historico_atendimento;

        public virtual IEnumerable<AtendimentoHistoricoAcoesCadastroModel> Historico_Atendimento

        {
            get
            {
                if (historico_atendimento == null)
                    historico_atendimento = new List<AtendimentoHistoricoAcoesCadastroModel>();
                return historico_atendimento;
            }
            set
            {
                historico_atendimento = value;
            }
        }
    }

    public class Atendimento_ModeloAttribute : Attribute
    {
        public Recurso_PainelAtendimentoEnum Recurso { get; set; }
        public Atendimento_ModeloAttribute(Recurso_PainelAtendimentoEnum value)
        {
            Recurso = value;
        }
    }

    public class AcompanhamentoAgente_DataTableRequest : DataTableRequest
    {

        public virtual Enum_Status_Chat? Status_Chat { get; set; }

        public virtual String Agente { get; set; }
        public virtual String Motivo { get; set; }

        public virtual Int64? Qtd_Atendimento { get; set; }

        public virtual String Setor { get; set; }

        public virtual Int64? Numero_Max_Atendimento { get; set; }
    }

    public class Atendimento_DataTableRequest : DataTableRequest
    {
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Tipo_Atendimento)]
        public virtual Int64? Tipo_Atendimento { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Categoria_Cliente)]
        public virtual Int64? Categoria_Cliente { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Categoria_Atendimento)]
        public virtual Int64? Categoria_Atendimento { get; set; }
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Setor)]
        public virtual Int64? Setor { get; set; }

        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Status)]
        public virtual Status_Atendimento? Status_Atendimento { get; set; }

        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Motivo_Status)]
        public virtual Int64? Motivo_Status { get; set; }

        public virtual Int64? Num_Os { get; set; }

        //Período
        public virtual Int32? Tipo_Data { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Final { get; set; }

        //Prioridade de Atendimento
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.Prioridade_Atendimento)]
        public virtual Prioridade_Atendimento? Prioridade_Atendimento { get; set; }

        //SLA de Atendimento
        [Atendimento_Modelo(Recurso_PainelAtendimentoEnum.SLA_Atendimento)]
        public virtual Status_SLA_Atendimento? SLA_Atendimento { get; set; }

        public virtual Boolean Dentro_Prazo { get; set; }
        public virtual Boolean Atencao { get; set; }
        public virtual Boolean Vencimento_Proximo { get; set; }
        public virtual Boolean Risco_Perda_Prazo { get; set; }
        public virtual Boolean Prazo_Vencido { get; set; }

        public virtual Int64? Id_Modelo_PainelAtendimento { get; set; }
        public virtual String Usuario_Logado { get; set; }
        public virtual Int64? Id_DepartamentoUsuario_Logado { get; set; }
    }

    public class AtendimentoHistoricoAcoesCadastroModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Responsavel { get; set; }
        public virtual String Status { get; set; }

        public virtual String Cor_SLA { get; set; }

        public virtual String Setor { get; set; }

        public virtual String DescricaoAcao { get; set; }
        public virtual String HTML { get; set; }
        public virtual Boolean Mensagens_Chat { get; set; }

        public virtual String DataCriacao { get; set; }
        public virtual String Usuario { get; set; }
        public virtual List<ArquivoAcaoModel> Arquivos { get; set; }

    }

    public class ArquivoAcaoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Arquivo { get; set; }
        public virtual String Extensao { get; set; }
        public virtual String Nome_Arquivo { get; set; }

    }

    public class Atendimento_Script_AtendimentoModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64 Id_Tipo_Atendimento { get; set; }
        public virtual String Descricao_Tipo_Atendimento { get; set; }

        public virtual Int64 Id_Atendimento { get; set; }

        public virtual Int32 Row_Pergunta { get; set; }

        public virtual Int64 Id_Pergunta { get; set; }
        public virtual String Pergunta { get; set; }

        public virtual String Instrucao_Atendente { get; set; }
        public virtual String Instrucao_Cliente { get; set; }

        public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }

        public virtual String Resposta { get; set; }

        public virtual List<Boolean> RespostasMultiplas { get; set; }

        public virtual List<String> RespostasPossiveis { get; set; }

        public virtual String Observacao_Atendimento { get; set; }
    }



    public class ConsultaClienteAtendimentoModel
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Numero { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }

        //public virtual String Status { get; set; }

        public virtual String Categoria_Atendimento { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual String Assunto { get; set; }

        public virtual String Pessoa_Contato { get; set; }

        public virtual String Ultima_Acao { get; set; }

        public virtual String Cliente { get; set; }

        public virtual String Origem_Atendimento { get; set; }
        public virtual String Plano { get; set; }
        public virtual String Produto { get; set; }
        public virtual String Tipo_Atendimento { get; set; }

        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Motivo_Status { get; set; }

        public virtual String DataHoraCriacao { get; set; }
        public virtual String Data_Hora_Fechamento { get; set; }

        public virtual String NomeAtendimento { get; set; }

        public virtual String Status { get; set; }

    }

    public class DadosAtendimentoChatModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome_Cliente { get; set; }
        public virtual String Cpf { get; set; }
        public virtual String Categoria_Cliente { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual Int64? Id_Categoria_Atendimento { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual Int64? Id_Setor { get; set; }
        public virtual String Id_Responsavel { get; set; }


    }

    public class ConsultaClienteAtendimento_DataTableRequest : DataTableRequest
    {
        public virtual String Dt_Inicial_Filtro { get; set; }
        public virtual String Dt_Final_Filtro { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
        public virtual Status_Atendimento? Status { get; set; }

    }
    //public class Tipo_Atendimento
    //{
    //    public virtual String Id { get; set; }
    //    public virtual String UsuarioCriacao { get; set; }
    //    public virtual String UsuarioAlteracao { get; set; }
    //    public virtual String DatahoraCriacao { get; set; }
    //    public virtual String DataHoraAlteracao { get; set; }
    //    public virtual String Token { get; set; }
    //    public virtual String Nome { get; set; }
    //    public virtual String Prioridade_Padrao { get; set; }
    //    public virtual String Status { get; set; }
    //    public virtual String Criar_OS { get; set; }
    //    public virtual String Exibir_App_Ola { get; set; }
    //    public virtual String Contrato { get; set; }
    //    public virtual String Descricao { get; set; }
    //    public virtual String Id_Script { get; set; }
    //    public virtual String Id_Categoria { get; set; }
    //    public virtual String id_Script_SuporteTecnico { get; set; }
    //}
    public class Categoria_Atendimento
    {
        public virtual String Id { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String UsuarioAlteracao { get; set; }
        public virtual String DataHoraCriacao { get; set; }
        public virtual String DataHoraAlteracao { get; set; }
        public virtual String Token { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Status { get; set; }
    }
    public class CategoriaAtendimento_ApiRequest : ApiRequest 
    {
        /// <summary> Busca por Status Ativo(1) ou Inativo(0) </summary>
        public Status_Default Status { get; set; }
    }
    public class TipoAtendimento_ApiRequest : ApiRequest 
    {
        /// <summary> Busca por Status Ativo(1) ou Inativo(0) </summary>
        public Status_Default Status { get; set; } 
    }

}