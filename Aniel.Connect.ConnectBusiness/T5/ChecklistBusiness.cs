using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ChecklistBusiness
    {
        private readonly IChecklistRepository _repoChecklist;
        private readonly IEtapaVendaRepository _repoEtapaVenda;

        public ChecklistBusiness(IChecklistRepository repoChecklist)
        {
            this._repoChecklist = repoChecklist;
        }
        public ChecklistBusiness(IChecklistRepository repoChecklist, IEtapaVendaRepository repoEtapaVenda)
        {
            this._repoChecklist = repoChecklist;
            this._repoEtapaVenda = repoEtapaVenda;
        }

        /// <summary>
        ///     Retorna os dados para a listagem do checklist
        /// </summary>
        /// <param name="request">Parâmetros do datatable</param>
        public ChecklistListData GetListagemChecklist(ChecklistDataTableRequest request)
        {
            return this._repoChecklist.GetListagemChecklist(request);
        }

        /// <summary>
        ///     Retorna o model da view para o checklist informado
        /// </summary>
        /// <param name="checklist">Dados do checklist vindos do banco</param>
        public ChecklistCadastroModel GetViewObj(long id)
        {
            Quiz checklist = this._repoChecklist.GetChecklistById(id);

            if (checklist == null)
            {
                throw new Exception("Checklist não encontrado");
            }

            var view = new ChecklistCadastroModel();

            view.Id = checklist.Id;

            view.Descricao = checklist.Titulo;
            view.Tipo = checklist.Tipo;
            view.Status = checklist.Ativo == "S";

            view.CodCt = checklist.Codct;
            view.ObrigImprodutivo = checklist.ObrigarImprodutivo == "S";
            view.PreencheInicioAtividade = checklist.PreencherInicioAtividade == "S";
            view.ChecklistNatureza = checklist.ChecklistNatureza == "S";
            view.TipoApiSISOP = checklist.TipoApiSISOP;

            view.TipoPessoa = checklist.TipoPessoa;
            view.IdCategoriaCliente = checklist.IdCategoriaCliente;
            view.IdMeioCaptacao = checklist.IdMeioCaptacao;
            view.IdUf = checklist.IdUf;
            view.IdCidade = checklist.IdCidade;
            view.MensagemConcluido = checklist.MensagemConcluido;

            foreach (QuizQuestion questao in checklist.Questoes.ToList())
            {
                var questaoView = new ChecklistQuestaoViewModel();

                questaoView.Id = questao.Id;

                questaoView.Ordem = questao.Ordem;
                questaoView.Pergunta = questao.Pergunta;

                questaoView.TipoQuestaoDescricao = HtmlEnumExtensions.GetEnumDescription(questao.Tipo);
                questaoView.RespostaObrigatoriaDescricao = questao.Obrigatoria == "S" ? RsBase.SIM : RsBase.NAO;
                questaoView.LiberarGaleriaDescricao = questao.LiberaGaleria == "S" ? RsBase.SIM : RsBase.NAO;
                questaoView.ExibirRVTDigitalDescricao = questao.ExibirRvt == "S" ? RsBase.SIM : RsBase.NAO;
                questaoView.VinculaRestricaoDescricao = questao.VinculaRestricao == "S" ? RsBase.SIM : RsBase.NAO;
                questaoView.RestricaoDescricao = questao.Restricao.HasValue ? HtmlEnumExtensions.GetEnumDescription(questao.Restricao.Value) : String.Empty;

                questaoView.TipoQuestao = questao.Tipo;
                questaoView.RespostaObrigatoria = questao.Obrigatoria == "S";
                questaoView.LiberarGaleria = questao.LiberaGaleria == "S";
                questaoView.ExibirRVTDigital = questao.ExibirRvt == "S";
                questaoView.VinculaRestricao = questao.VinculaRestricao == "S";
                questaoView.Restricao = questao.Restricao;
                questaoView.DadosSISOP = questao.DadosSISOP;
                questaoView.DadosSISOPDescricao = questao.DadosSISOP.HasValue ? HtmlEnumExtensions.GetEnumDescription(questao.DadosSISOP.Value): String.Empty ;

                foreach (QuizQuestionAnswer alternativa in questao.RespostasPossiveis.ToList())
                {
                    var alternativaView = new ChecklistQuestaoAlternativa
                    {
                        Id = alternativa.Id,
                        Resposta = alternativa.Resposta
                    };

                    questaoView.Alternativas.Add(alternativaView);
                }

                view.Questoes.Add(questaoView);
            }

            view.Questoes = view.Questoes.OrderBy(q => q.Ordem).ToList();

            return view;
        }

        public void AlterarStatus(long idChecklist, Status_Default status)
        {
            Quiz checklist = this._repoChecklist.GetChecklistById(idChecklist);

            if (checklist == null)
            {
                throw new Exception("Checklist de ID " + idChecklist + " não encontrada");
            }

            checklist.Ativo = status == Status_Default.Ativo ? "S" : "N";

            this._repoChecklist.Update(checklist);
        }

        public void RemoveQuestao(long idChecklist, long idQuestao, string usuario)
        {
            Quiz checklist = this._repoChecklist.GetChecklistById(idChecklist);

            if (checklist == null)
            {
                throw new Exception("Checklist de ID " + idChecklist + " não encontrada");
            }

            QuizQuestion questao = checklist.Questoes.Where(q => q.Id == idQuestao).FirstOrDefault();
            if (questao == null)
            {
                throw new Exception("Questão de ID " + idQuestao + " não encontrada no checklist de ID " + idChecklist);
            }
            
            questao.DiaExclusao = DateTime.Now.Date;
            questao.HoraExclusao = DateTime.Now;
            questao.UsuarioExclusao = usuario;          

            // Como o mapeamento define que o objeto "Quiz" deve ser o responsável por gravar
            // todas as alterações nos objetos de "QuizQuestion", para realizar a remoção da questão
            // temos que removê-la da lista do "Quiz".
            checklist.Questoes = checklist.Questoes.Where(q => q.Id != idQuestao).ToList();

            this._repoChecklist.Update(checklist);
        }

        /// <summary>
        ///     Grava os dados de um checklist preenchidos pelo cadastro
        /// </summary>
        /// <param name="view">Dados do checklist vindos da view</param>
        /// <returns>ID do registro gravado</returns>
        public long GravarChecklist(ChecklistCadastroModel view)
        {
            Quiz quiz = this._repoChecklist.GetChecklistById(view.Id);

            if (quiz == null)
            {
                quiz = new Quiz
                {
                    Dia = DateTime.Now.Date,
                    Hora = DateTime.Now,
                    Ativo = "S"
                };
            }
            else
            {
                quiz.Ativo = view.Status ? "S" : "N";
            }

            quiz.Titulo = view.Descricao;
            quiz.Tipo = view.Tipo;
            quiz.TipoApiSISOP = view.TipoApiSISOP;

            if (quiz.Tipo == TipoChecklist.Servico)
            {
                quiz.Codct = view.CodCt;
                quiz.ObrigarImprodutivo = view.ObrigImprodutivo ? "S" : "N";
                quiz.PreencherInicioAtividade = view.PreencheInicioAtividade ? "S" : "N";
                quiz.ChecklistNatureza = view.ChecklistNatureza ? "S" : "N";
            }
            else
            {
                quiz.TipoPessoa = view.TipoPessoa;
                quiz.IdCategoriaCliente = view.IdCategoriaCliente;
                quiz.IdMeioCaptacao = view.IdMeioCaptacao;
                quiz.IdUf = view.IdUf;
                quiz.IdCidade = view.IdCidade;
                quiz.MensagemConcluido = view.MensagemConcluido;
            }

            foreach (ChecklistQuestaoViewModel questaoView in view.Questoes.Where(q => q.Id == 0))
            {
                var questao = new QuizQuestion
                {
                    Quiz = quiz,

                    Ordem = questaoView.Ordem,
                    Tipo = questaoView.TipoQuestao,
                    Pergunta = questaoView.Pergunta,
                    Obrigatoria = questaoView.RespostaObrigatoria ? "S" : "N",
                    LiberaGaleria = questaoView.LiberarGaleria ? "S" : "N",
                    ExibirRvt = questaoView.ExibirRVTDigital ? "S" : "N",
                    VinculaRestricao = questaoView.VinculaRestricao ? "S" : "N",
                    Restricao = questaoView.Restricao,
                    DadosSISOP = questaoView.DadosSISOP
                };

                foreach (ChecklistQuestaoAlternativa alternativaView in questaoView.Alternativas)
                {
                    var alternativa = new QuizQuestionAnswer
                    {
                        QuizQuestion = questao,
                        Resposta = alternativaView.Resposta
                    };

                    questao.RespostasPossiveis.Add(alternativa);
                }

                quiz.Questoes.Add(questao);
            }

            if (quiz.Id == 0)
            {
                this._repoChecklist.Save(quiz);
            }
            else
            {
                this._repoChecklist.Update(quiz);
            }

            return quiz.Id;
        }

        /// <summary>
        ///     Retorna o checklist associado à venda, caso haja algum
        /// </summary>
        /// <param name="idVenda">ID da Venda</param>
        public Quiz GetChecklistVenda(long idVenda)
        {
            return this._repoChecklist.GetQuizVenda(idVenda);
        }

        /// <summary>
        ///     Retorna o checklist associado à troca de titularidade, caso haja algum
        /// </summary>
        /// <param name="idTroca">ID da Venda</param>
        public Quiz GetChecklistTrocaTitularidade(long idTroca)
        {
            return this._repoChecklist.GetQuizTroca(idTroca);
        }

        public Quiz GetChecklistOS(string codTipoServico, string codSubTipoServico)
        {
            return this._repoChecklist.GetQuizOS(codTipoServico, codSubTipoServico);
        }
        public IEnumerable<Quiz> GetChecklistsOS(string codTipoServico, string codSubTipoServico)
        {
            return this._repoChecklist.GetQuizzes(codTipoServico, codSubTipoServico);
        }

        /// <summary>
        ///     Retorna um booleano que indica se a venda já possui um checklist associado
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public bool VendaHasChecklist(long idVenda)
        {
            return this._repoChecklist.GetQuizVenda(idVenda) != null;
        }

        /// <summary>
        ///     Informa se o checklist de uma venda já foi preenchido
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public bool ChecklistPreenchido(long idVenda)
        {
            bool preenchido = false;
            IEnumerable<QuizAnswerItem> questoes = this._repoChecklist.GetAnswersByVenda(idVenda);

            if (questoes == null || questoes.Count() == 0)
            {
                return preenchido;
            }

            #region Validação das questões vinculadas a restrição

            IEnumerable<Aniel.Connect.Model.nhibernate.EtapaVenda> etapasVenda = this._repoEtapaVenda.GetByVenda(idVenda);

            if (etapasVenda != null)
            {
                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaViabilidade =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaViabilidade)
                               .FirstOrDefault();

                bool consultaViabilidadePendenteOuAprovada =
                    etapaConsultaViabilidade == null ||
                    (etapaConsultaViabilidade.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaViabilidade.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaViabilidade.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaViabilidadePendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaViabilidade)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaPendenciaFinanceira =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira)
                               .FirstOrDefault();

                bool consultaPendenciaFinanceiraPendenteOuAprovada =
                    etapaConsultaPendenciaFinanceira == null ||
                    (etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaPendenciaFinanceiraPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaPendenciaFinanceira)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaPendenciaEndereco =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco)
                               .FirstOrDefault();

                bool consultaPendenciaEnderecoPendenteOuAprovada =
                    etapaConsultaPendenciaEndereco == null ||
                    (etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaPendenciaEnderecoPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaPendenciaEndereco)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaCredito =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaCredito)
                               .FirstOrDefault();

                bool consultaCreditoPendenteOuAprovada =
                    etapaConsultaCredito == null ||
                    (etapaConsultaCredito.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaCredito.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaCredito.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaCreditoPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaCredito)
                                .ToList();
                }
            }

            #endregion

            if (questoes.Count() == 0)
            {
                return preenchido;
            }

            IEnumerable<QuizAnswerItem> questoesObrigatorias = questoes.Where(q => q.QuizQuestion.Obrigatoria == "S");

            if (questoesObrigatorias.Count() > 0)
            {
                int qtdQuestoesObrigatorias = questoesObrigatorias.Count();
                int qtdQuestoesObrigatoriasPreenchidas = 0;

                foreach (QuizAnswerItem questaoObrigatoria in questoesObrigatorias)
                {
                    bool questaoObrigatoriaPreenchida = false;

                    switch (questaoObrigatoria.Tipo)
                    {
                        case EnumQuizTipo.Texto:
                            questaoObrigatoriaPreenchida = !String.IsNullOrEmpty(questaoObrigatoria.RespostaTexto);
                            break;
                        case EnumQuizTipo.Valor:
                            questaoObrigatoriaPreenchida = questaoObrigatoria.RespostaValor.HasValue;
                            break;
                        case EnumQuizTipo.Data:
                            questaoObrigatoriaPreenchida = questaoObrigatoria.RespostaData.HasValue;
                            break;
                        case EnumQuizTipo.Foto:
                            questaoObrigatoriaPreenchida =
                                questaoObrigatoria.Alternativas
                                        .Where(q => q.RespostaFoto.HasValue).Count() > 0;
                            break;
                        case EnumQuizTipo.UnicaEscolha:
                            questaoObrigatoriaPreenchida = !String.IsNullOrEmpty(questaoObrigatoria.Resposta);
                            break;
                        case EnumQuizTipo.MultiplaEscolha:
                            questaoObrigatoriaPreenchida =
                            questaoObrigatoria.Alternativas
                                    .Where(q => !String.IsNullOrEmpty(q.Resposta)).Count() > 0;
                            break;
                        case EnumQuizTipo.Assinatura:
                            // O comportamento da questão do tipo de assinatura não é conhecido ainda, por enquanto
                            // só estamos pulando esse caso
                            break;
                    }

                    if (questaoObrigatoriaPreenchida)
                    {
                        qtdQuestoesObrigatoriasPreenchidas++;
                    }
                }

                preenchido = qtdQuestoesObrigatorias == qtdQuestoesObrigatoriasPreenchidas;
            }
            else
            {
                foreach (QuizAnswerItem questao in questoes)
                {
                    switch (questao.Tipo)
                    {
                        case EnumQuizTipo.Texto:
                            preenchido = !String.IsNullOrEmpty(questao.RespostaTexto);
                            break;
                        case EnumQuizTipo.Valor:
                            preenchido = questao.RespostaValor.HasValue;
                            break;
                        case EnumQuizTipo.Data:
                            preenchido = questao.RespostaData.HasValue;
                            break;
                        case EnumQuizTipo.Foto:
                            preenchido =
                                questao.Alternativas
                                        .Where(q => q.RespostaFoto.HasValue).Count() > 0;
                            break;
                        case EnumQuizTipo.UnicaEscolha:
                            preenchido = !String.IsNullOrEmpty(questao.Resposta);
                            break;
                        case EnumQuizTipo.MultiplaEscolha:
                            preenchido =
                            questao.Alternativas
                                    .Where(q => !String.IsNullOrEmpty(q.Resposta)).Count() > 0;
                            break;
                        case EnumQuizTipo.Assinatura:
                            // O comportamento da questão do tipo de assinatura não é conhecido ainda, por enquanto
                            // só estamos pulando esse caso
                            break;
                    }

                    if (preenchido)
                        break;
                }
            }

            return preenchido;
        }



        /// <summary>
        ///     Informa se o checklist de uma Troca de Titularidade já foi preenchido
        /// </summary>
        /// <param name="idTroca">ID da Troca</param>
        public bool ChecklistTrocaPreenchido(long idTroca)
        {
            bool preenchido = false;
            IEnumerable<QuizAnswerItem> questoes = this._repoChecklist.GetAnswersByTroca(idTroca);

            if (questoes == null || questoes.Count() == 0)
            {
                return preenchido;
            }

            #region Validação das questões vinculadas a restrição

            IEnumerable<Aniel.Connect.Model.nhibernate.EtapaVenda> etapasVenda = this._repoEtapaVenda.GetByTrocaTitularidade(idTroca);

            if (etapasVenda != null)
            {
                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaViabilidade =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaViabilidade)
                               .FirstOrDefault();

                bool consultaViabilidadePendenteOuAprovada =
                    etapaConsultaViabilidade == null ||
                    (etapaConsultaViabilidade.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaViabilidade.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaViabilidade.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaViabilidadePendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaViabilidade)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaPendenciaFinanceira =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira)
                               .FirstOrDefault();

                bool consultaPendenciaFinanceiraPendenteOuAprovada =
                    etapaConsultaPendenciaFinanceira == null ||
                    (etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaPendenciaFinanceira.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaPendenciaFinanceiraPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaPendenciaFinanceira)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaPendenciaEndereco =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco)
                               .FirstOrDefault();

                bool consultaPendenciaEnderecoPendenteOuAprovada =
                    etapaConsultaPendenciaEndereco == null ||
                    (etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaPendenciaEndereco.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaPendenciaEnderecoPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaPendenciaEndereco)
                                .ToList();
                }

                Aniel.Connect.Model.nhibernate.EtapaVenda etapaConsultaCredito =
                    etapasVenda.Where(p => p.Etapa == EnumEtapaVenda.ConsultaCredito)
                               .FirstOrDefault();

                bool consultaCreditoPendenteOuAprovada =
                    etapaConsultaCredito == null ||
                    (etapaConsultaCredito.Status == StatusEtapaVenda.Pendente ||
                     etapaConsultaCredito.Status == StatusEtapaVenda.Aprovado ||
                     etapaConsultaCredito.Status == StatusEtapaVenda.AprovadoManualmente);

                if (consultaCreditoPendenteOuAprovada)
                {
                    questoes =
                        questoes.Where(r => r.Restricao != ConsultasRestricaoVenda.ConsultaCredito)
                                .ToList();
                }
            }

            #endregion

            if (questoes.Count() == 0)
            {
                return preenchido;
            }

            IEnumerable<QuizAnswerItem> questoesObrigatorias = questoes.Where(q => q.QuizQuestion.Obrigatoria == "S");

            if (questoesObrigatorias.Count() > 0)
            {
                int qtdQuestoesObrigatorias = questoesObrigatorias.Count();
                int qtdQuestoesObrigatoriasPreenchidas = 0;

                foreach (QuizAnswerItem questaoObrigatoria in questoesObrigatorias)
                {
                    bool questaoObrigatoriaPreenchida = false;

                    switch (questaoObrigatoria.Tipo)
                    {
                        case EnumQuizTipo.Texto:
                            questaoObrigatoriaPreenchida = !String.IsNullOrEmpty(questaoObrigatoria.RespostaTexto);
                            break;
                        case EnumQuizTipo.Valor:
                            questaoObrigatoriaPreenchida = questaoObrigatoria.RespostaValor.HasValue;
                            break;
                        case EnumQuizTipo.Data:
                            questaoObrigatoriaPreenchida = questaoObrigatoria.RespostaData.HasValue;
                            break;
                        case EnumQuizTipo.Foto:
                            questaoObrigatoriaPreenchida =
                                questaoObrigatoria.Alternativas
                                        .Where(q => q.RespostaFoto.HasValue).Count() > 0;
                            break;
                        case EnumQuizTipo.UnicaEscolha:
                            questaoObrigatoriaPreenchida = !String.IsNullOrEmpty(questaoObrigatoria.Resposta);
                            break;
                        case EnumQuizTipo.MultiplaEscolha:
                            questaoObrigatoriaPreenchida =
                            questaoObrigatoria.Alternativas
                                    .Where(q => !String.IsNullOrEmpty(q.Resposta)).Count() > 0;
                            break;
                        case EnumQuizTipo.Assinatura:
                            // O comportamento da questão do tipo de assinatura não é conhecido ainda, por enquanto
                            // só estamos pulando esse caso
                            break;
                    }

                    if (questaoObrigatoriaPreenchida)
                    {
                        qtdQuestoesObrigatoriasPreenchidas++;
                    }
                }

                preenchido = qtdQuestoesObrigatorias == qtdQuestoesObrigatoriasPreenchidas;
            }
            else
            {
                foreach (QuizAnswerItem questao in questoes)
                {
                    switch (questao.Tipo)
                    {
                        case EnumQuizTipo.Texto:
                            preenchido = !String.IsNullOrEmpty(questao.RespostaTexto);
                            break;
                        case EnumQuizTipo.Valor:
                            preenchido = questao.RespostaValor.HasValue;
                            break;
                        case EnumQuizTipo.Data:
                            preenchido = questao.RespostaData.HasValue;
                            break;
                        case EnumQuizTipo.Foto:
                            preenchido =
                                questao.Alternativas
                                        .Where(q => q.RespostaFoto.HasValue).Count() > 0;
                            break;
                        case EnumQuizTipo.UnicaEscolha:
                            preenchido = !String.IsNullOrEmpty(questao.Resposta);
                            break;
                        case EnumQuizTipo.MultiplaEscolha:
                            preenchido =
                            questao.Alternativas
                                    .Where(q => !String.IsNullOrEmpty(q.Resposta)).Count() > 0;
                            break;
                        case EnumQuizTipo.Assinatura:
                            // O comportamento da questão do tipo de assinatura não é conhecido ainda, por enquanto
                            // só estamos pulando esse caso
                            break;
                    }

                    if (preenchido)
                        break;
                }
            }

            return preenchido;
        }


        /// <summary>
        ///     Associa um checklist à uma venda 
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        /// <param name="tipoPessoa">Tipo de pessoa do cliente (física ou jurídica)</param>
        /// <param name="idCategoriaCliente">ID da categoria do cliente</param>
        /// <param name="idMeioCaptacao">ID do meio de captação</param>
        /// <param name="idUf">ID da UF</param>
        /// <param name="idCidade">ID da cidade</param>
        public void AssociarChecklistVenda(
            long idVenda,
            Tipo_Pessoa tipoPessoa,
            long? idCategoriaCliente,
            long? idMeioCaptacao,
            long? idUf,
            long? idCidade
        )
        {
            long? idQuiz = this._repoChecklist.GetIdQuizParaVenda(tipoPessoa, idCategoriaCliente, idMeioCaptacao, idUf, idCidade);

            if (idQuiz.HasValue)
            {
                Quiz quiz = this._repoChecklist.GetChecklistById(idQuiz.Value);

                var capa = new QuizAnswerCapa();

                capa.DiaInicio = DateTime.Now.Date;
                capa.HoraInicio = DateTime.Now;
                capa.Quiz = quiz;
                capa.Titulo = quiz.Titulo;

                this._repoChecklist.SaveCapa(capa);

                var quizVenda = new QuizAnswerVenda();

                quizVenda.IdVenda = idVenda;
                quizVenda.Capa = capa;

                this._repoChecklist.SaveQuizVenda(quizVenda);

                // Gravando nas tabelas que contêm as repostas para as questões do checklist
                // para que, ao abrir o checklist na venda, sejam identificadas as questões.

                // Inicialmente esses registros de respostas irão ser criados sem valores de resposta,
                // pois eles serão preenchidos mais a frente no fluxo quando o usuário preencher o checklist da venda
                foreach (QuizQuestion questao in quiz.Questoes)
                {
                    var answerItem = new QuizAnswerItem();

                    answerItem.AnswerCapa = capa;
                    answerItem.QuizQuestion = questao;

                    answerItem.Pergunta = questao.Pergunta;
                    answerItem.Tipo = questao.Tipo;
                    answerItem.Ordem = questao.Ordem;
                    answerItem.Respondido = "N";

                    answerItem.Obrigatoria = questao.Obrigatoria;
                    answerItem.LiberaGaleria = questao.LiberaGaleria;

                    answerItem.ExibirRvt = questao.ExibirRvt;

                    answerItem.VinculaRestricao = questao.VinculaRestricao;
                    answerItem.Restricao = questao.Restricao;

                    if (questao.RespostasPossiveis != null && questao.RespostasPossiveis.Count() > 0)
                    {
                        var opcoesResposta =
                            questao.RespostasPossiveis
                                   .Select(r => new QuizQuestionAnswerViewModel
                                   {
                                       id = r.Id,
                                       resposta = r.Resposta
                                   });

                        answerItem.OpcoesResposta = JsonService.SerializeJasonMapper(opcoesResposta);
                    }

                    this._repoChecklist.SaveAnswerItem(answerItem);

                    if (questao.RespostasPossiveis != null && questao.RespostasPossiveis.Count() > 0)
                    {
                        foreach (QuizQuestionAnswer alternativa in questao.RespostasPossiveis)
                        {
                            var answerItemMulti = new QuizAnswerItemMulti
                            {
                                QuizAnswerItem = answerItem,
                                QuizQuestionAnswer = alternativa
                            };

                            this._repoChecklist.SaveAnswerItemMulti(answerItemMulti);
                        }
                    }
                }
            }
        }


        /// <summary>
        ///     Associa um checklist à uma troca de titularidade 
        /// </summary>
        /// <param name="idTroca">ID da Troca da Titularidade</param>
        /// <param name="tipoPessoa">Tipo de pessoa do cliente (física ou jurídica)</param>
        /// <param name="idQuizSelect">ID do Quiz selecionado</param>
        /// <param name="idCategoriaCliente">ID da categoria do cliente</param>
        /// <param name="idMeioCaptacao">ID do meio de captação</param>
        /// <param name="idUf">ID da UF</param>
        /// <param name="idCidade">ID da cidade</param>
        public void AssociarChecklistTroca(
            long idTroca,
            Tipo_Pessoa tipoPessoa,
            long idQuizSelect,
            long? idCategoriaCliente,
            long? idMeioCaptacao,
            long? idUf,
            long? idCidade
        )
        {
            Quiz quiz = this._repoChecklist.GetChecklistById(idQuizSelect);

            if (quiz != null)
            {
                var capa = new QuizAnswerCapa();

                capa.DiaInicio = DateTime.Now.Date;
                capa.HoraInicio = DateTime.Now;
                capa.Quiz = quiz;
                capa.Titulo = quiz.Titulo;

                this._repoChecklist.SaveCapa(capa);

                var quizVenda = new QuizAnswerVenda();

                quizVenda.IdTrocaTitularidade = idTroca;
                quizVenda.Capa = capa;

                this._repoChecklist.SaveQuizVenda(quizVenda);

                // Gravando nas tabelas que contêm as repostas para as questões do checklist
                // para que, ao abrir o checklist na venda, sejam identificadas as questões.

                // Inicialmente esses registros de respostas irão ser criados sem valores de resposta,
                // pois eles serão preenchidos mais a frente no fluxo quando o usuário preencher o checklist da venda
                foreach (QuizQuestion questao in quiz.Questoes)
                {
                    var answerItem = new QuizAnswerItem();

                    answerItem.AnswerCapa = capa;
                    answerItem.QuizQuestion = questao;

                    answerItem.Pergunta = questao.Pergunta;
                    answerItem.Tipo = questao.Tipo;
                    answerItem.Ordem = questao.Ordem;
                    answerItem.Respondido = "N";

                    answerItem.Obrigatoria = questao.Obrigatoria;
                    answerItem.LiberaGaleria = questao.LiberaGaleria;

                    answerItem.ExibirRvt = questao.ExibirRvt;

                    answerItem.VinculaRestricao = questao.VinculaRestricao;
                    answerItem.Restricao = questao.Restricao;

                    if (questao.RespostasPossiveis != null && questao.RespostasPossiveis.Count() > 0)
                    {
                        var opcoesResposta =
                            questao.RespostasPossiveis
                                   .Select(r => new QuizQuestionAnswerViewModel
                                   {
                                       id = r.Id,
                                       resposta = r.Resposta
                                   });

                        answerItem.OpcoesResposta = JsonService.SerializeJasonMapper(opcoesResposta);
                    }

                    this._repoChecklist.SaveAnswerItem(answerItem);

                    if (questao.RespostasPossiveis != null && questao.RespostasPossiveis.Count() > 0)
                    {
                        foreach (QuizQuestionAnswer alternativa in questao.RespostasPossiveis)
                        {
                            var answerItemMulti = new QuizAnswerItemMulti
                            {
                                QuizAnswerItem = answerItem,
                                QuizQuestionAnswer = alternativa
                            };

                            this._repoChecklist.SaveAnswerItemMulti(answerItemMulti);
                        }
                    }
                }
            }
        }

        public List<Quiz> GetAll()
        {
            return _repoChecklist.GetAll().Where(x => x.Ativo == "S").ToList();
        }
    }
}
