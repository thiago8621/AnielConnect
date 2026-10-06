using Aniel.T5.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.T5.Model.nhibernate
{
    public class QuizQuestion
    {
        private IList<QuizQuestionAnswer> _respostasPossiveis;

        public virtual long Id { get; set; }

        public virtual string UsuarioExclusao { get; set; }
        public virtual DateTime? DiaExclusao { get; set; }
        public virtual DateTime? HoraExclusao { get; set; }
        
        public virtual Quiz Quiz { get; set; }

        public virtual short Ordem { get; set; }
        public virtual EnumQuizTipo Tipo { get; set; } 
        public virtual string Pergunta { get; set; }
        public virtual string Obrigatoria { get; set; }
        public virtual string LiberaGaleria { get; set; }
        public virtual string ExibirRvt { get; set; }
        public virtual string VinculaRestricao { get; set; }
        public virtual ConsultasRestricaoVenda? Restricao { get; set; }
        public virtual DadosSISOP? DadosSISOP { get; set; }
        
        public virtual IList<QuizQuestionAnswer> RespostasPossiveis
        {
            get
            {
                if (_respostasPossiveis == null)
                    _respostasPossiveis = new List<QuizQuestionAnswer>();

                return _respostasPossiveis;
            }
            set
            {
                _respostasPossiveis = value;
            }
        }
    }
}
