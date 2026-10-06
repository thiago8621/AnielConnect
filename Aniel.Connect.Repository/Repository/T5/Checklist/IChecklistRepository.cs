using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Checklist
{
    public interface IChecklistRepository
    {
        /// <summary>
        ///     Retorna a listagem dos checklists para o datatable da tela de listagem anterior ao cadastro de checklist
        /// </summary>
        /// <param name="request">Dados da requisição do datatable</param>
        ChecklistListData GetListagemChecklist(ChecklistDataTableRequest request);

        /// <summary>
        ///     Retorna a representação de uma consulta na TB_QUIZ
        /// </summary>
        IQueryable<Quiz> GetAll();

        /// <summary>
        ///     Retorna os dados do checklist associado ao ID informado
        /// </summary>
        /// <param name="id">ID do checklist</param>
        Quiz GetChecklistById(long id);

        /// <summary>
        ///     Retorna quiz associado com a venda
        /// </summary>
        /// <param name="idVenda">ID da Venda</param>
        Quiz GetQuizVenda(long idVenda);        
        
        /// <summary>
        ///     Retorna quiz associado com a uma Troca de Titularidade
        /// </summary>
        /// <param name="idVenda">ID da Venda</param>
        Quiz GetQuizTroca(long idTroca);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numObra"></param>
        /// <param name="codCt"></param>
        /// <param name="projeto"></param>
        /// <param name="numDoc"></param>
        /// <returns></returns>
        Quiz GetQuizOS(string codTipoServico, string codSubTipoServico);
        IEnumerable<Quiz> GetQuizzes(string codTipoServico, string codSubTipoServico);

        IEnumerable<QuizAnswerItem> GetQuizItens(long Id);
        IEnumerable<QuizAnswerItemMulti> GetQuizItensMultiplo(long Id);
        byte[] GetAssinatura(long Id);

        /// <summary>
        ///     Retorna os registros de respostas de questões do checklist associados a venda informada
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        IEnumerable<QuizAnswerItem> GetAnswersByVenda(long idVenda);


        /// <summary>
        ///     Retorna os registros de respostas de questões do checklist associados a Troca informada
        /// </summary>
        /// <param name="idTroca">ID da Troca</param>
        IEnumerable<QuizAnswerItem> GetAnswersByTroca(long idTroca);

        /// <summary>
        ///     Retorna o ID do quiz mais apropriado para a venda, de acordo com seus dados
        /// </summary>
        /// <param name="tipoPessoa">Tipo de pessoa do cliente (física ou jurídica)</param>
        /// <param name="idCategoriaCliente">ID da categoria do cliente</param>
        /// <param name="idMeioCaptacao">ID do meio de captação</param>
        /// <param name="idUf">ID da UF</param>
        /// <param name="idCidade">ID da cidade</param>
        long? GetIdQuizParaVenda(
            Tipo_Pessoa tipoPessoa, 
            long? idCategoriaCliente,
            long? idMeioCaptacao,
            long? idUf,
            long? idCidade
        );

        /// <summary>
        ///     Insere um novo checklist no banco do Aniel
        /// </summary>
        /// <param name="quiz">Dados do checklist</param>
        void Save(Quiz quiz);

        /// <summary>
        ///     Atualiza os dados de um checklist que já existe no banco do Aniel
        /// </summary>
        /// <param name="quiz">Dados do checklist</param>
        void Update(Quiz quiz);

        /// <summary>
        ///     Insere um novo registro de capa para o checklist
        /// </summary>
        /// <param name="capa">Dados da capa</param>
        void SaveCapa(QuizAnswerCapa capa);

        /// <summary>
        ///     Insere um novo registro para associar uma venda à um quiz
        /// </summary>
        /// <param name="quizVenda">Dados do quiz e da venda</param>
        void SaveQuizVenda(QuizAnswerVenda quizVenda);

        /// <summary>
        ///     Insere um novo registro para conter a resposta de uma questão de um quiz
        /// </summary>
        /// <param name="answerItem">Dados do container da resposta</param>
        void SaveAnswerItem(QuizAnswerItem answerItem);

        /// <summary>
        ///     Insere um novo registro para conter a resposta de uma questão de um quiz com múltiplas alternativas
        /// </summary>
        /// <param name="answerItemMulti">Dados do container da resposta</param>
        void SaveAnswerItemMulti(QuizAnswerItemMulti answerItemMulti);
    }
}
