using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class ParametrosConsultaPreQualListModel
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public string Padrao { get; set; }
    }

    public class ParamConsultaPreQualAlterarStatusRequest
    {
        public IEnumerable<long> Ids { get; set; }
        public Status_Default Status { get; set; }
    }

    public class ParamConsultaPreQualTipoPessoa
    {
        public long Id { get; set; }
        public string TipoPessoa { get; set;}
        public int CodTipoPessoa { get; set;}
    }

    public class ParamConsultaPreQualCategoriaCliente
    {
        public long Id { get; set; }
        public string CodCategoriaCliente { get; set; }
        public string CategoriaCliente { get; set; }
    }

    public class ParamConsultaPreQualSetor
    {
        public long Id { get; set; }

        public string CodDepto { get; set; }
        public string Setor { get; set; }
    }

    public class ParamConsultaPreQualMeioCaptacao
    {
        public long Id { get; set; }

        public string CodMeioCaptacao { get; set; }
        public string MeioCaptacao { get; set; }
    }

    public class ParamConsultaPreQualRegiao
    {
        public long Id { get; set; }
        public string CodUF { get; set; }
        public string CodCidade { get; set; }
        public string CodBairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Bairro { get; set; }
    }
    
    public class ParamConsultaPreQualEquipeVenda
    {
        public long Id { get; set; }
        public string Equipe { get; set; }
        public string CodEquipe { get; set; }
    }

    public class ParametrosConsultaPreQualCadastroModel
    {
        private IEnumerable<ParamConsultaPreQualTipoPessoa> _paramsTipoPessoa;
        private IEnumerable<ParamConsultaPreQualCategoriaCliente> _paramsCategoriaCliente;
        private IEnumerable<ParamConsultaPreQualSetor> _paramsSetor;
        private IEnumerable<ParamConsultaPreQualMeioCaptacao> _paramsMeioCaptacao;
        private IEnumerable<ParamConsultaPreQualRegiao> _paramsRegiao;
        private IEnumerable<ParamConsultaPreQualEquipeVenda> _paramsEquipeVenda;

        public long Id { get; set; }
        public string Descricao { get; set; }
        public bool Status { get; set; }
        public bool Padrao { get; set; } 

        public IEnumerable<ParamConsultaPreQualTipoPessoa> ParamsTipoPessoa
        {
            get
            {
                if (this._paramsTipoPessoa == null)
                    this._paramsTipoPessoa = new List<ParamConsultaPreQualTipoPessoa>();

                return this._paramsTipoPessoa;
            }
            set
            {
                this._paramsTipoPessoa = value;
            }
        }

        public IEnumerable<ParamConsultaPreQualCategoriaCliente> ParamsCategoriaCliente
        {
            get
            {
                if (this._paramsCategoriaCliente == null)
                    this._paramsCategoriaCliente = new List<ParamConsultaPreQualCategoriaCliente>();

                return this._paramsCategoriaCliente;
            }
            set
            {
                this._paramsCategoriaCliente = value;
            }
        }

        public IEnumerable<ParamConsultaPreQualSetor> ParamsSetor
        {
            get
            {
                if (this._paramsSetor == null)
                    this._paramsSetor = new List<ParamConsultaPreQualSetor>();

                return this._paramsSetor;
            }
            set
            {
                this._paramsSetor = value;
            }
        }

        public IEnumerable<ParamConsultaPreQualMeioCaptacao> ParamsMeioCaptacao
        {
            get
            {
                if (this._paramsMeioCaptacao == null)
                    this._paramsMeioCaptacao = new List<ParamConsultaPreQualMeioCaptacao>();

                return this._paramsMeioCaptacao;
            }
            set
            {
                this._paramsMeioCaptacao = value;
            }
        }

        public IEnumerable<ParamConsultaPreQualRegiao> ParamsRegiao
        {
            get
            {
                if (this._paramsRegiao == null)
                    this._paramsRegiao = new List<ParamConsultaPreQualRegiao>();

                return this._paramsRegiao;
            }
            set
            {
                this._paramsRegiao = value;
            }
        }
        
        public IEnumerable<ParamConsultaPreQualEquipeVenda> ParamsEquipeVenda
        {
            get
            {
                if (this._paramsEquipeVenda == null)
                    this._paramsEquipeVenda = new List<ParamConsultaPreQualEquipeVenda>();

                return this._paramsEquipeVenda;
            }
            set
            {
                this._paramsEquipeVenda = value;
            }
        }
    }
}
