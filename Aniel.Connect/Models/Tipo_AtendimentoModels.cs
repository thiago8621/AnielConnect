using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Aniel.Connect.Models
{
    public class Tipo_AtendimentoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Script { get; set; }

        public virtual String Prioridade_Padrao { get; set; }



        public virtual String Status { get; set; }
        public virtual String Codigo { get; set; }
        public virtual Int64 Sequencia { get; set; }

        public virtual String Setor { get; set; }

        public virtual String Contrato { get; set; }

        public virtual String Tempo_Exc { get; set; }

        public virtual bool Encerrar_Atendimento { get; set; }

        public virtual bool Etapa_Decisoria { get; set; }

        public virtual String Proxima_Etapa { get; set; }




        public virtual String Categoria_Atendimento { get; set; }

        public virtual String Criar_Os_Encerrar { get; set; }
        public virtual String Exibir_Ola { get; set; }

        public virtual Int64 Id_Motivo { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Cod_Encerramento { get; set; }

        public virtual Boolean Encerrar_Atendimento_Junto_OS { get; set; }
        public virtual Boolean Trava_Atendimento_OS { get; set; }
        public virtual Boolean Utilizar_Criacao_Email_Atendimento { get; set; }

        public virtual Boolean Utilizar_Criacao_Dominio_Email_Atendimento { get; set; }


    }


    public class Tipo_AtendimentoMobile
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual String prioridade_padrao { get; set; }
        public virtual long? Id_Script_SuporteTecnico { get; set; }
        public virtual long? Id_Tipo_Servico { get; set; }
    }


    public class Tipo_AtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "NOME", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Nome { get; set; }

        public virtual Int64? Id_Script { get; set; }

        [Display(Name = "PRIORIDADE_PADRAO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Prioridade_Atendimento? Prioridade_Padrao { get; set; }

        public virtual Prioridade_Atendimento? Categoria_Atendimento { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Status { get; set; }

        [Display(Name = "CATEGORIA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int64? Id_Categoria_Atendimento { get; set; }

        [Display(Name = "CRIAR_OS_ENCERRAR", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Criar_Os_Encerrar { get; set; }

        public virtual Boolean Possui_Etapa { get; set; }

        public virtual Boolean Exibir_Ola { get; set; }
        public virtual Boolean Vincular_Motivos { get; set; }
        public virtual Boolean Solicitacao_Cancelamento { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Codigo { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Desc_Tipo_Servico { get; set; }

        public virtual Int64? Id_Script_SuporteTecnico { get; set; }

        public virtual Int64? Id_Tipo_Atendimento { get; set; }

        public virtual String ProximaEtapa { get; set; }
        public virtual Boolean StatusFluxoAtendimento { get; set; }

        [Display(Name = "SEQUENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int64? Sequencia { get; set; }

        [Display(Name = "SETOR", ResourceType = typeof(Resources.RsCadastros))]
       
        public virtual String Setor { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual Boolean AlterarSetor_Responsavel { get; set; }
        public virtual String Cod_Encerramento { get; set; }
        public virtual Int64? IdFluxoEtapa { get; set; }
        public virtual String Tempo_Exc { get; set; }

        public virtual bool Etapa_Decisoria { get; set; }

        public virtual Boolean Etapa_CriaOS { get; set; }
        public virtual Boolean Baixar_OS { get; set; }

        [Display(Name = "ENCERRA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual bool Encerrar_Atendimento { get; set; }


        public virtual String Contrato { get; set; }
       
        public virtual String Motivo_Status_Atendimento { get; set; }

        public virtual String Proxima_Etapa { get; set; }
        public virtual Status_AtendimentoEtapa? Status_Atendimento { get; set; }

        public virtual Boolean Trava_Atendimento_OS { get; set; }
        public virtual Boolean Encerrar_Atendimento_Junto_OS { get; set; }

        [Display(Name = "UTILIZAR_CRIACAO_EMAIL_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Utilizar_Criacao_Email_Atendimento { get; set; }
        public virtual Boolean Utilizar_Criacao_Dominio_Email_Atendimento { get; set; }
        public virtual Boolean Utilizar_SSL { get; set; }
        public virtual AuthMethodImap AuthMethodImap { get; set; }

        private List<URL_SistemaListaModel> URL_Sistemas_Lista;
        private List<EmailListaModel> Emails_Lista;




        private List<TipoAtendimento_EtapaFluxoCadastroModel> FluxoLista;

        public virtual List<TipoAtendimento_EtapaFluxoCadastroModel> Fluxo

        {
            get
            {
                if (FluxoLista == null)
                    FluxoLista = new List<TipoAtendimento_EtapaFluxoCadastroModel>();
                return FluxoLista;
            }
            set
            {
                FluxoLista = value;
            }
        }
        public virtual List<EmailListaModel> Emails
        {
            get
            {
                if (Emails_Lista == null)
                    Emails_Lista = new List<EmailListaModel>();
                return Emails_Lista;
            }
            set
            {
                Emails_Lista = value;
            }
        }



        private List<Tipo_AtendimentoMotivoCadastroModel> MotivosLista;

        public virtual List<Tipo_AtendimentoMotivoCadastroModel> Motivos

        {
            get
            {
                if (MotivosLista == null)
                    MotivosLista = new List<Tipo_AtendimentoMotivoCadastroModel>();
                return MotivosLista;
            }
            set
            {
                MotivosLista = value;
            }
        }


        //Teste
        private List<Motivo_Status_Tipo_AtendimentoListModel> MotivosListaStatus;

        public virtual List<Motivo_Status_Tipo_AtendimentoListModel> MotivosStatus

        {
            get
            {
                if (MotivosListaStatus == null)
                    MotivosListaStatus = new List<Motivo_Status_Tipo_AtendimentoListModel>();
                return MotivosListaStatus;
            }
            set
            {
                MotivosListaStatus = value;
            }
        }

        public Int64? Id_Motivo { get; set; }


    }

    public class TipoAtendimento_EtapaFluxoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        [Display(Name = "SEQUENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int64? Sequencia { get; set; }

        [Display(Name = "SETOR", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        public virtual String Descricao { get; set; }
        public virtual String Desc_AlterarSetor_Responsavel { get; set; }
        public virtual Boolean AlterarSetor_Responsavel { get; set; }
        public virtual String Responsavel { get; set; }
        public virtual String Setor { get; set; }
        public virtual String Setor_Desc { get; set; }
        public virtual Boolean Etapa_CriaOS { get; set; }
        public virtual String Desc_Etapa_CriaOS { get; set; }
        public virtual Boolean Baixar_OS { get; set; }
        public virtual String Desc_Baixar_OS { get; set; }
        public virtual String Cod_Encerramento { get; set; }
        public virtual String Cod_Encerramento_Desc { get; set; }
        public virtual String Tempo_Exc { get; set; }

        public virtual bool Etapa_Decisoria { get; set; }
        public virtual String Etapa_Decisoria_Desc { get; set; }
        public virtual String Status_Desc { get; set; }
        public virtual Status_AtendimentoEtapa? StatusAtendimento { get; set; }
        public virtual String StatusAtendimentoDesc { get; set; }
        public virtual String Motivo_Status_Atendimento { get; set; }
        public virtual String Motivo_Status_AtendimentoDesc { get; set; }

        public virtual bool Status { get; set; }
        public virtual Boolean StatusFluxoAtendimento { get; set; }
        [Display(Name = "ENCERRA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual bool Encerra_Atendimento { get; set; }
        public virtual String Encerra_Atendimento_Desc { get; set; }


        public virtual String Contrato { get; set; }
        public virtual String Contrato_Desc { get; set; }

        public virtual String Proxima_Etapa { get; set; }
        public virtual String Proxima_Etapa_Desc { get; set; }
        private List<Tipo_AtendimentoEtapaEncerramentoCadastroModel> EncerramentosLista;

        public virtual List<Tipo_AtendimentoEtapaEncerramentoCadastroModel> Encerramentos
        {
            get
            {
                if (EncerramentosLista == null)
                    EncerramentosLista = new List<Tipo_AtendimentoEtapaEncerramentoCadastroModel>();
                return EncerramentosLista;
            }
            set
            {
                EncerramentosLista = value;
            }
        }

        private IEnumerable<Motivo_Status_AtendimentoCadastroModel> _motivos { get; set; }
        public virtual IEnumerable<Motivo_Status_AtendimentoCadastroModel> Motivos
        {
            get
            {
                if (_motivos == null)
                    _motivos = new List<Motivo_Status_AtendimentoCadastroModel>();
                return _motivos;
            }
            set
            {
                _motivos = value;
            }
        }

    }

    public class Tipo_AtendimentoMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual Int64? Id_Motivo { get; set; }
        public virtual String Desc_Motivo { get; set; }
    }

    public class Tipo_AtendimentoEtapaEncerramentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Cod_Encerramento_Desc { get; set; }
        public virtual String Cod_Encerramento { get; set; }
        public virtual Int64? IdFluxoEtapa { get; set; }
        public virtual String DescFluxoEtapa { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Contrato_Desc { get; set; }
    }


    public class Motivo_Status_Tipo_AtendimentoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Motivo_Status_Atendimento { get; set; }
        public virtual String Motivo_Status_Atendimento_Desc { get; set; }
        public virtual String Status_Motivo { get; set; }
        public virtual String Status_Motivo_Desc { get; set; }
        public virtual Int64? Id_Tipo_Atendimento { get; set; }
    }

    public class Tipo_Atendimento_DataTableRequest : DataTableRequest
    {

    }

}