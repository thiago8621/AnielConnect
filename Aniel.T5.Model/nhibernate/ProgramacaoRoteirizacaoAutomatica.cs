using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class ProgramacaoRoteirizacaoAutomatica : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual DateTime? DataHoraInicio { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual FormatoDisparoAutomaticoRoteirizacao? FormatoDisparoAutomaticoRoteirizacao { get; set; }
        public virtual Boolean? Servico_SemAgendamento { get; set; }
        public virtual Boolean? Servico_Agendados { get; set; }
        public virtual Boolean? Servico_Paralisados { get; set; }
        public virtual Boolean? Servico_Nao_Executados { get; set; }
        public virtual Boolean? Servico_Roteirizados { get; set; }
        public virtual int? NumUltimosDias { get; set; }
        public virtual int? NumPrimeirosDias { get; set; }
        public virtual Boolean? TodosDias { get; set; }
        public virtual Boolean? NoDia { get; set; }
        public virtual Boolean? NaSemana { get; set; }
        public virtual Boolean? NoMes { get; set; }
        public virtual Boolean? NosPrimeiros { get; set; }
        public virtual Boolean? NosUltimos { get; set; }
        public virtual String Meses { get; set; }
        public virtual String DiasMes { get; set; }
        public virtual String DiasSemana { get; set; }
        public virtual DateTime? DataExpira { get; set; }
        public virtual short? NumeroMaximoAtendimento { get; set; }
        public virtual bool? AlterarDataHoraSemAgendamentoAniel { get; set; }

        private IList<ProgramacaoRoteirizacaoCategoriaCliente> _categoriasCliente;
        private IList<ProgramacaoRoteirizacaoTecnico> _tecnicos;
        private IList<ProgramacaoRoteirizacaoSupervisor> _supervisores;
        private IList<ProgramacaoRoteirizacaoNaturezaProducao> _naturezas;
        private IList<ProgramacaoRoteirizacaoFilial> _filiais;
        private IList<ProgramacaoRoteirizacaoProjeto> _projetos;
        private IList<ProgramacaoRoteirizacaoLocalidade> _localidades;

        public virtual IList<ProgramacaoRoteirizacaoCategoriaCliente> CategoriasCliente
        {
            get
            {
                if (_categoriasCliente == null)
                    _categoriasCliente = new List<ProgramacaoRoteirizacaoCategoriaCliente>();
                return _categoriasCliente;
            }
            set
            {
                _categoriasCliente = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoTecnico> Tecnicos
        {
            get
            {
                if (_tecnicos == null)
                    _tecnicos = new List<ProgramacaoRoteirizacaoTecnico>();
                return _tecnicos;
            }
            set
            {
                _tecnicos = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoSupervisor> Supervisores
        {
            get
            {
                if (_supervisores == null)
                    _supervisores = new List<ProgramacaoRoteirizacaoSupervisor>();
                return _supervisores;
            }
            set
            {
                _supervisores = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoNaturezaProducao> Naturezas
        {
            get
            {
                if (_naturezas == null)
                    _naturezas = new List<ProgramacaoRoteirizacaoNaturezaProducao>();
                return _naturezas;
            }
            set
            {
                _naturezas = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoFilial> Filiais
        {
            get
            {
                if (_filiais == null)
                    _filiais = new List<ProgramacaoRoteirizacaoFilial>();
                return _filiais;
            }
            set
            {
                _filiais = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoProjeto> Projetos
        {
            get
            {
                if (_projetos == null)
                    _projetos = new List<ProgramacaoRoteirizacaoProjeto>();
                return _projetos;
            }
            set
            {
                _projetos = value;
            }
        }

        public virtual IList<ProgramacaoRoteirizacaoLocalidade> Localidades
        {
            get
            {
                if (_localidades == null)
                    _localidades = new List<ProgramacaoRoteirizacaoLocalidade>();
                return _localidades;
            }
            set
            {
                _localidades = value;
            }
        }
    }
}
