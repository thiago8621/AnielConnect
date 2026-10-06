using Aniel.Connect.Core;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class LimiteAgendamentoListModel
    {
        public long Id { get; set; }

        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public int IdNatureza { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Natureza { get; set; }
        public string Data { get; set; }
        public string Percentual { get; set; }
    }

    public class LimiteAgendamentoModel
    {
        private IEnumerable<LimiteAgendamentoPercentualNatureza> _percentualNatureza;

        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        
        /// <summary>
        ///     Campo passado para editar uma data em específico após clicar em um registro na listagem
        /// </summary>
        public string Data { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }

        public IEnumerable<LimiteAgendamentoPercentualNatureza> PercentualNatureza 
        { 
            get
            {
                if (this._percentualNatureza == null)
                    this._percentualNatureza = new List<LimiteAgendamentoPercentualNatureza>();

                return this._percentualNatureza;
            }
            set
            {
                this._percentualNatureza = value;
            }
        }
    }

    /// <summary>
    ///     Representa um registro na tabela que registra os percentuais para uma 
    ///     natureza no cadastro de limite de agendamento
    /// </summary>
    public class LimiteAgendamentoPercentualNatureza
    {
        public int IdNatureza { get; set; }
        public string Natureza { get; set; }
        public string Percentual { get; set; }
    }

    public class LimiteAgendamentoListResponse
    {
        private IEnumerable<LimiteAgendamentoListModel> _data;

        public int Total { get; set; }
        public IEnumerable<LimiteAgendamentoListModel> Data 
        { 
            get
            {
                if (this._data == null)
                    this._data = new List<LimiteAgendamentoListModel>();

                return this._data;
            }
            set 
            {
                this._data = value;
            }
        }
    }

    public class LimiteAgendamentoDataTableRequest : DataTableRequest
    {
        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public int? IdNatureza { get; set; }

        public string DataInicio { get; set; }
        public string DataFim { get; set; }
    }
}
