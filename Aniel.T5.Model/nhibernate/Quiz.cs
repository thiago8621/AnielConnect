using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.T5.Model.nhibernate
{
    public class Quiz
    {
        private IList<QuizQuestion> _questoes = null;

        public virtual long Id { get; set; }
        
        public virtual DateTime? Dia { get; set; } 
        public virtual DateTime? Hora { get; set; } 
        public virtual string Usuario { get; set; }
        
        public virtual string Titulo { get; set; }

        public virtual TipoChecklist Tipo { get; set; }

        #region Campos Serviço

        public virtual string Codct { get; set; }
        public virtual string ObrigarImprodutivo { get; set; }
        public virtual string PreencherInicioAtividade { get; set; }
        public virtual string ChecklistNatureza { get; set; }
        public virtual int? IdNatureza { get; set; }

        #endregion

        #region Campos Comercial

        public virtual Tipo_Pessoa? TipoPessoa { get; set; }
        public virtual long? IdCategoriaCliente { get; set; }
        public virtual long? IdMeioCaptacao { get; set; }
        public virtual long? IdUf { get; set; }
        public virtual long? IdCidade { get; set; }

        /// <summary>
        ///     Mensagem que será exibida para um checklist de vendas
        ///     após o cliente gravar as respostas
        /// </summary>
        public virtual string MensagemConcluido { get; set; }

        #endregion

        #region Campos SISOP
        public virtual TipoApiSISOP? TipoApiSISOP { get; set; }
        #endregion

        public virtual string Ativo { get; set; }

        public virtual IList<QuizQuestion> Questoes
        {
            get
            {
                if (this._questoes == null)
                    this._questoes = new List<QuizQuestion>();

                return this._questoes;
            }

            set
            {
                this._questoes = value;
            }
        }
    }
}
