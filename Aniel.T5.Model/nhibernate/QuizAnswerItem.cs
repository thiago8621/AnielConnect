using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.T5.Model.nhibernate
{
    public class QuizAnswerItem
    {
        private IList<QuizAnswerItemMulti> _alternativas;

        public virtual long Id { get; set; }
        public virtual long Id_Assinatura { get; set; }

        public virtual QuizAnswerCapa AnswerCapa { get; set; }
        public virtual QuizQuestion QuizQuestion { get; set; }
        public virtual QuizQuestionAnswer QuizQuestionAnswer { get; set; }

        public virtual string LiberaGaleria { get; set; }
        public virtual string ExibirRvt { get; set; }
        public virtual string VinculaRestricao { get; set; }
        public virtual ConsultasRestricaoVenda? Restricao { get; set; }
        public virtual string Obrigatoria { get; set; }
        public virtual string OpcoesResposta { get; set; }
        public virtual int Ordem { get; set; }
        public virtual string Pergunta { get; set; }
        public virtual string Respondido { get; set; }
        public virtual string Resposta { get; set; }
        public virtual DateTime? RespostaData { get; set; }
        public virtual long? RespostaFoto { get; set; }
        public virtual string RespostaTexto { get; set; }
        public virtual double? RespostaValor { get; set; }
        public virtual string RespostaCodBarra { get; set; }
        public virtual EnumQuizTipo? Tipo { get; set; }

        /// <summary>
        ///     Registros de alternativas de respostas usado para
        ///     questões do tipo única escolha, múltipla escolha e foto.
        ///     
        ///     Objeto somente para leitura.
        /// </summary>
        public virtual IList<QuizAnswerItemMulti> Alternativas
        {
            get
            {
                if (this._alternativas == null)
                    this._alternativas = new List<QuizAnswerItemMulti>();

                return this._alternativas;
            }

            set
            {
                this._alternativas = value;
            }
        }
    }
}
