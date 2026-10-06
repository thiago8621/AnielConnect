using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class PerfilVisualizacaoVendaListModel
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
    }

    public class ParamVisualizacaoVendaAlterarStatusRequest
    {
        public IEnumerable<long> Ids { get; set; }
        public Status_Default Status { get; set; }
    }

    public class ParamVisualizacaoVendaTipoPessoa
    {
        public long Id { get; set; }
        public string TipoPessoa { get; set;}
        public int CodTipoPessoa { get; set;}
    }

    public class ParamVisualizacaoVendaCategoriaCliente
    {
        public long Id { get; set; }
        public string CodCategoriaCliente { get; set; }
        public string CategoriaCliente { get; set; }
    }

    public class ParamVisualizacaoVendaSetor
    {
        public long Id { get; set; }

        public string CodDepto { get; set; }
        public string Setor { get; set; }
    }

    public class ParamVisualizacaoVendaMeioCaptacao
    {
        public long Id { get; set; }

        public string CodMeioCaptacao { get; set; }
        public string MeioCaptacao { get; set; }
    }

    public class ParamVisualizacaoVendaRegiao
    {
        public long Id { get; set; }
        public string CodUF { get; set; }
        public string CodCidade { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
    }
    
    public class ParamVisualizacaoVendaEquipeVenda
    {
        public long Id { get; set; }
        public string Equipe { get; set; }
        public string CodEquipe { get; set; }
    }

    public class PerfilVisualizacaoVendaCadastroModel
    {
        private IEnumerable<ParamVisualizacaoVendaTipoPessoa> _paramsTipoPessoa;
        private IEnumerable<ParamVisualizacaoVendaCategoriaCliente> _paramsCategoriaCliente;
        private IEnumerable<ParamVisualizacaoVendaSetor> _paramsSetor;
        private IEnumerable<ParamVisualizacaoVendaMeioCaptacao> _paramsMeioCaptacao;
        private IEnumerable<ParamVisualizacaoVendaRegiao> _paramsRegiao;
        private IEnumerable<ParamVisualizacaoVendaEquipeVenda> _paramsEquipeVenda;

        public long Id { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }
        public bool PerfilSolicitaAutorizacao { get; set; } 
        public bool PadraoVisualizacaoLead { get; set; } 
        public bool PadraoVisualizacaoVenda { get; set; } 
        public int? Qtd_Dias_Bloqueio_Lead { get; set; } 

        public IEnumerable<ParamVisualizacaoVendaTipoPessoa> ParamsTipoPessoa
        {
            get
            {
                if (this._paramsTipoPessoa == null)
                    this._paramsTipoPessoa = new List<ParamVisualizacaoVendaTipoPessoa>();

                return this._paramsTipoPessoa;
            }
            set
            {
                this._paramsTipoPessoa = value;
            }
        }

        public IEnumerable<ParamVisualizacaoVendaCategoriaCliente> ParamsCategoriaCliente
        {
            get
            {
                if (this._paramsCategoriaCliente == null)
                    this._paramsCategoriaCliente = new List<ParamVisualizacaoVendaCategoriaCliente>();

                return this._paramsCategoriaCliente;
            }
            set
            {
                this._paramsCategoriaCliente = value;
            }
        }

        public IEnumerable<ParamVisualizacaoVendaSetor> ParamsSetor
        {
            get
            {
                if (this._paramsSetor == null)
                    this._paramsSetor = new List<ParamVisualizacaoVendaSetor>();

                return this._paramsSetor;
            }
            set
            {
                this._paramsSetor = value;
            }
        }

        public IEnumerable<ParamVisualizacaoVendaMeioCaptacao> ParamsMeioCaptacao
        {
            get
            {
                if (this._paramsMeioCaptacao == null)
                    this._paramsMeioCaptacao = new List<ParamVisualizacaoVendaMeioCaptacao>();

                return this._paramsMeioCaptacao;
            }
            set
            {
                this._paramsMeioCaptacao = value;
            }
        }

        public IEnumerable<ParamVisualizacaoVendaRegiao> ParamsRegiao
        {
            get
            {
                if (this._paramsRegiao == null)
                    this._paramsRegiao = new List<ParamVisualizacaoVendaRegiao>();

                return this._paramsRegiao;
            }
            set
            {
                this._paramsRegiao = value;
            }
        }
        
        public IEnumerable<ParamVisualizacaoVendaEquipeVenda> ParamsEquipeVenda
        {
            get
            {
                if (this._paramsEquipeVenda == null)
                    this._paramsEquipeVenda = new List<ParamVisualizacaoVendaEquipeVenda>();

                return this._paramsEquipeVenda;
            }
            set
            {
                this._paramsEquipeVenda = value;
            }
        }
    }
}
