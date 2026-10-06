using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class ChecklistListModel
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public string Contrato { get; set; }
        public string ObrigarImprodutivo { get; set; }
        public string PreencherInicioAtividade { get; set; }
        public string ChecklistNatureza { get; set; }
        public string Status { get; set; }
    }

    public class ChecklistListData
    {
        public int RecordsFiltered { get; set; }
        public IList<ChecklistListModel> Data { get; set; }
    }

    public class ChecklistDataTableRequest : DataTableRequest
    {
        public string Contrato { get; set; }
        public TipoChecklist? Tipo { get; set; }
        public Status_Default? Ativo { get; set; }
        public Sinapse.Model.enumerados.Padrao? ObrigarImprodutivo { get; set; }
        public Sinapse.Model.enumerados.Padrao? PreencherInicioAtividade { get; set; }
        public Sinapse.Model.enumerados.Padrao? CheckListporNatureza { get; set; }

    }

    public class ChecklistCadastroModel
    {
        private IList<ChecklistQuestaoViewModel> _questoes = null;

        public long Id { get; set; }

        public string Descricao { get; set; }
        public TipoChecklist Tipo { get; set; }
        public bool Status { get; set; }

        #region Campos de serviço

        public string CodCt { get; set; }
        public bool ObrigImprodutivo { get; set; }
        public bool PreencheInicioAtividade { get; set; }
        public bool ChecklistNatureza { get; set; }

        #endregion

        #region Campos da venda

        public Tipo_Pessoa? TipoPessoa { get; set; }
        public long? IdCategoriaCliente { get; set; }
        public long? IdMeioCaptacao { get; set; }
        public long? IdUf { get; set; }
        public long? IdCidade { get; set; }
        public string MensagemConcluido { get; set; }

        public bool VinculaRestricao { get; set; } // campo usado só para o razor
        public ConsultasRestricaoVenda? Restricao { get; set; } // campo usado só para o razor

        #endregion

        #region Campos SISOP
        public virtual TipoApiSISOP? TipoApiSISOP { get; set; }
        public virtual DadosSISOP? DadosSISOP { get; set; }
        #endregion

        #region Questoes

        public int? Ordem { get; set; }
        public EnumQuizTipo? TipoQuestao { get; set; }
        public string Pergunta { get; set; }
        public bool ExibirRVTDigital { get; set; }
        public bool LiberarGaleria { get; set; }
        public bool RespostaObrigatoria { get; set; }

        #endregion

        public IList<ChecklistQuestaoViewModel> Questoes
        {
            get
            {
                if (this._questoes == null)
                    this._questoes = new List<ChecklistQuestaoViewModel>();

                return this._questoes;
            }
            set
            {
                this._questoes = value;
            }
        }
    }

    public class ChecklistQuestaoViewModel
    {
        private IList<ChecklistQuestaoAlternativa> _alternativas = null;

        public long Id { get; set; }
        public short Ordem { get; set; }
        public string Pergunta { get; set; }

        public string TipoQuestaoDescricao { get; set; }
        public string RespostaObrigatoriaDescricao { get; set; }
        public string LiberarGaleriaDescricao { get; set; }
        public string ExibirRVTDigitalDescricao { get; set; }
        public string VinculaRestricaoDescricao { get; set; }
        public string RestricaoDescricao { get; set; }

        public EnumQuizTipo TipoQuestao { get; set; }
        public bool RespostaObrigatoria { get; set; }
        public bool LiberarGaleria { get; set; }
        public bool ExibirRVTDigital { get; set; }
        public bool VinculaRestricao { get; set; }
        public ConsultasRestricaoVenda? Restricao { get; set; }
        public virtual DadosSISOP? DadosSISOP { get; set; }
        public string DadosSISOPDescricao { get; set; }

        public IList<ChecklistQuestaoAlternativa> Alternativas
        {
            get
            {
                if (this._alternativas == null)
                    this._alternativas = new List<ChecklistQuestaoAlternativa>();

                return this._alternativas;
            }

            set
            {
                this._alternativas = value;
            }
        }
    }

    public class ChecklistQuestaoAlternativa
    {
        public long Id { get; set; }
        public string Resposta { get; set; }
    }

    public class ExcluirQuestaoChecklistRequest
    {
        public long IdChecklist { get; set; }
        public long IdQuestao { get; set; }
    }

    public class AlterarStatusChecklistRequest
    {
        public IEnumerable<long> Ids { get; set; }
        public Status_Default Status { get; set; }
    }

    #region Classes para visualização das respostas do checklist

    public class QuizAnswer
    {
        private List<QuizAnswerItemViewModel> _respostas;
        private List<QuizAnswerProducaoViewModel> _producoes;
        private List<QuizAnswerMaterialViewModel> _materiais;

        public long? id { get; set; }
        public long? idA { get; set; }
        public long? idQuiz { get; set; }
        public long? idQuizTarefa { get; set; }
        public string titulo { get; set; }
        public string tituloQuiz { get; set; }
        public string protocolo { get; set; }
        public string usuario { get; set; }
        public DateTime? diaInicio { get; set; }
        public DateTime? horaInicio { get; set; }

        #region Campos de serviço

        public string codct { get; set; }
        public string projeto { get; set; }
        public string num_obra { get; set; }
        public string tarefa { get; set; }
        public int numDoc { get; set; }
        public short codTipoServico { get; set; }
        public string codSubTipoServico { get; set; }

        #endregion

        #region Campos do comercial

        public long? idVenda { get; set; }
        public Tipo_Pessoa? tipoPessoa { get; set; }
        public long? idCategoriaCliente { get; set; }
        public long? idMeioCaptacao { get; set; }
        public long? idUf { get; set; }
        public long? idCidade { get; set; }

        #endregion

        public List<QuizAnswerItemViewModel> respostas
        {
            get
            {
                if (_respostas == null)
                    _respostas = new List<QuizAnswerItemViewModel>();
                return _respostas;
            }
            set
            {
                _respostas = value;
            }
        }

        public List<QuizAnswerProducaoViewModel> producoes
        {
            get
            {
                if (_producoes == null)
                    _producoes = new List<QuizAnswerProducaoViewModel>();
                return _producoes;
            }
            set
            {
                _producoes = value;
            }
        }

        public List<QuizAnswerMaterialViewModel> materiais
        {
            get
            {
                if (_materiais == null)
                    _materiais = new List<QuizAnswerMaterialViewModel>();
                return _materiais;
            }
            set
            {
                _materiais = value;
            }
        }
    }

    public class QuizAnswerItemViewModel
    {
        private List<QuizAnswerItemMultiViewModel> _multi;

        public long? id { get; set; }
        public long? idQuestion { get; set; }
        public long? idQuestionAnswer { get; set; }
        public string pergunta { get; set; }
        public EnumQuizTipo tipo { get; set; }
        public long ordem { get; set; }
        public object resposta { get; set; }
        public bool isRespondido { get; set; }
        public string obrigatoria { get; set; }
        public bool liberaGaleria { get; set; }
        public string idAssinatura { get; set; }

        #region Campos do servico na pergunta

        public bool exibirRvt { get; set; }

        #endregion

        #region Campos da venda na pergunta

        public bool vinculaRestricao { get; set; }
        public ConsultasRestricaoVenda? restricao { get; set; }

        #endregion

        #region Mobile
        public long? idA { get; set; }
        #endregion

        public List<QuizQuestionAnswerViewModel> opcoesResposta;

        public List<QuizAnswerItemMultiViewModel> multi
        {
            get
            {
                if (_multi == null)
                    _multi = new List<QuizAnswerItemMultiViewModel>();
                return _multi;
            }
            set
            {
                _multi = value;
            }
        }
    }

    public class QuizAnswerItemMultiViewModel
    {
        public long? id { get; set; }
        public long? idQuestionAnswerItem { get; set; }
        public long? idQuestionAnswer { get; set; }
        public object resposta { get; set; }

        public string foto64 { get; set; }
        public string urlfoto { get; set; }
        public long? idBDFoto { get; set; }
    }

    public class QuizAnswerMaterialViewModel
    {
        public long? id { get; set; }

        public string codmat { get; set; }
        public double quantidade { get; set; }
        public double antes { get; set; }
        public double comprimento_lance { get; set; }
        public double rolo { get; set; }
        public double sobra { get; set; }
    }

    public class QuizAnswerProducaoViewModel
    {
        public long? id { get; set; }

        public string unidadeCusto { get; set; }
        public string codigo { get; set; }
        public double quant_prod { get; set; }
    }

    public class QuizQuestionAnswerViewModel
    {
        public long? id { get; set; }
        public string resposta { get; set; }
    }
    #endregion
    
    public class DatatableChecklistVenda
    {
        public long? Id { get; set; }
        public long? Tipo { get; set; }
        public string Ativo { get; set; }
        public string Titulo { get; set; }
        public string Tipo_Pessoa { get; set; }
        public long? IdMeioCaptacao { get; set; }
        public long? CategoriaCliente { get; set; }
        public long? Id_Uf { get; set; }
        public long? Id_Cidade { get; set; }
        public long Perguntas { get; set; }
        public long Respondidas { get; set; }
        public string UsuarioCriacao { get; set; }
        public string DataHoraCriacao { get; set; }

    }
    
    public class DatatableChecklistRequest : DataTableRequest
    {
        public Tipo_Pessoa? Tipo_Pessoa { get; set; }
        public long? MeioCaptacao { get; set; }
        public long? CategoriaCliente { get; set; }
        public long? Uf { get; set; }
        public long? Cidade { get; set; }
    }

    public abstract class EmailChecklist
    {        
        public string LogoClienteBase64 { get; set; }
        public string NomeEmpresa { get; set; }
        public string Cnpj { get; set; }

        /// <summary>
        ///     Mensagem para exibir nos checklists concluídos
        /// </summary>
        public string MensagemConclusao { get; set; }
        public bool ChecklistPreenchido { get; set; }
        public string DataEncerramento { get; set; }
    }

    public class EmailChecklistModel : EmailChecklist
    {
        public long IdVenda { get; set; }

        public Status_Venda Status { get; set; }
    }

    public class EmailChecklistTrocaModel : EmailChecklist
    {
        public long IdTroca { get; set; }

        public StatusTrocaTitularidade Status { get; set; }
    }

    #region Mobile
    public class QuizCombinedModel
    {
        public virtual long? Id { get; set; }

        public virtual string UsuarioExclusao { get; set; }
        public virtual DateTime? DiaExclusao { get; set; }
        public virtual DateTime? HoraExclusao { get; set; }

        public virtual Aniel.T5.Model.nhibernate.Quiz Quiz { get; set; }

        public virtual short Ordem { get; set; }
        public virtual EnumQuizTipo Tipo { get; set; }
        public virtual string Pergunta { get; set; }
        public virtual string Obrigatoria { get; set; }
        public virtual string LiberaGaleria { get; set; }
        public virtual string ExibirRvt { get; set; }
        public virtual string VinculaRestricao { get; set; }
        public virtual ConsultasRestricaoVenda? Restricao { get; set; }


        public List<Connect.Model.nhibernate.QuizAnswerModel> RespostasPossiveis { get; set; }
    }
    #endregion
}