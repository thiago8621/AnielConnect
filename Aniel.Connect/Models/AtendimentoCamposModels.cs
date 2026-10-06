using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class AtendimentoCamposCadastroModels
    {
        private List<AtendimentoCamposRespostaPerso> _AtendimentoCamposRespostaPerso;
        private List<AtendimentoCamposTiposModels> _AtendimentoCamposTiposModels;
        public long? Id { get; set; }
        public long? Id_Tipo_Atendimento { get; set; }
        public Boolean CampoObrigatorio { get; set; }
        public Boolean Status { get; set; }
        public string Titulo { get; set; }
        public List<AtendimentoCamposRespostaPerso> AtendimentoCamposRespostaPerso
        {
            get
            {
                if (_AtendimentoCamposRespostaPerso == null)
                {
                    _AtendimentoCamposRespostaPerso = new List<AtendimentoCamposRespostaPerso>();
                }
                return _AtendimentoCamposRespostaPerso;
            }
            set { _AtendimentoCamposRespostaPerso = value; }
        }
        public List<AtendimentoCamposTiposModels> AtendimentoCamposTiposModels
        {
            get
            {
                if (_AtendimentoCamposTiposModels == null)
                {
                    _AtendimentoCamposTiposModels = new List<AtendimentoCamposTiposModels>();
                }
                return _AtendimentoCamposTiposModels;
            }
            set { _AtendimentoCamposTiposModels = value; }
        }
        public TipoCampoGravar? TipoCampoGravar { get; set; }
        public string TipoCampoGravarString { get; set; }
        public bool ExibirTodosAtendimentos { get; set; }
    }

    public class AtendimentoCamposRespostasResponse
    {
        public string RespostaPersonalizada { get; set; }
        public long Id { get; set; }
    }

    public class AtendimentoCamposTiposModels
    {
        public string DescricaoTipo { get; set; }
        public long Id { get; set; }
        public string Id_Tipo_Atendimento { get; set; }
    }
    public class AtendimentoCamposRespostaPerso
    {
        public string RespostaPersonalizada { get; set; }
        public long Id { get; set; }
    }

    public class AtendimentoCamposDataTableRequest : DataTableRequest
    {
        public long? Id { get; set; }
        public string TipoCampo { get; set; }
        public string CampoObrigatorio { get; set; }
        public string Status { get; set; }
        public List<string> IdsTipoAtendimento { get; set; }
    }
    public class AtendimentoCamposHistoricoRequest : DataTableRequest
    {
        public string DataInicial { get; set; }
        public string DataFim { get; set; }
        public string[] TipoOperacao { get; set; }
        public string Usuario { get; set; }
        public long? IdCampoPersonalizado { get; set; }
    }


    public class AtendimentoCamposHistoricoResponse
    {
        public string Usuario { get; set; }
        public string NomeUsuario { get; set; }
        public string Operacao { get; set; }
        public string Campo { get; set; }
        public string CodigoCampo { get; set; }
        public string ValorAntes { get; set; }
        public string ValorDepois { get; set; }
        public string DataHora { get; set; }
    }

    public class CamposPersonalizadosResponse
    {
        public bool Obrigatorio { get; set; }
        public string Titulo { get; set; }
        public string IdCampo { get; set; }
        public string TipoCampo { get; set; }
        public string Resposta { get; set; }
        public List<RespostasPersonalizadasUnicas> ListaRespostas { get; set; }
    }

    public class RespostasPersonalizadasUnicas
    {
        public string IdResposta { get; set; }
        public string IdAtendimentoCampo { get; set; }
        public string Descricao { get; set; }
    }
}