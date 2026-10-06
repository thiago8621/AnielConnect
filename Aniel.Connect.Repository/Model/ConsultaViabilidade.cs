using Sinapse.Model.enumerados;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    /// <summary>
    ///     Classe com os dados da caixa trazidos da consulta de viabilidade utilizada no projeto do Connect
    /// </summary>
    public class Consulta_ViabilidadeCaixa
    {
        public virtual long? Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string CampanhaVinculada { get; set; }
        public virtual double? Latitude { get; set; }
        public virtual double? Longitude { get; set; }
        public virtual IEnumerable<int?> PortasDisponiveis { get; set; }
        public virtual IEnumerable<long> IdsPortasDisponiveis { get; set; }
        public virtual double? DistanciaKm { get; set; }
        public virtual long? Capacidade { get; set; }
        public virtual bool reservada { get; set; }
        public virtual string path { get; set; }
        public virtual string cor { get; set; }
        public EtapasValidacaoConsultaViabilidadeEnum EtapaValidacao { get; set; }
        //public virtual long? IdMotivoInviabilidade { get; set; }
    }
    public class MotivoInviabilidadeConsulta 
    {
        public long? Id {get; set;}
        public  string Motivo {get; set;}
        public EtapasValidacaoConsultaViabilidadeEnum EtapaValidacao { get; set; }

    }

    public class ConsultaViabilidadeResponse 
    {
        private IEnumerable<MotivoInviabilidadeConsulta> _motivos = null;
        private IEnumerable<Consulta_ViabilidadeCaixa> _caixas = null;
        private IEnumerable<string> _passos = null;
        
        /// <summary>
        ///     ID do registro com os dados da consulta de viabilidade realizada
        /// </summary>
        public long IdConsultaViabilidade { get; set; }

        public string MotivoAprovacaoManual { get; set; }

        public IEnumerable<MotivoInviabilidadeConsulta>Motivos 
        {
            get 
            { 
                if (_motivos == null) 
                { 
                    _motivos = new List<MotivoInviabilidadeConsulta>();
                }

                return _motivos; 
            } 
            set 
            { 
                _motivos = value;
            } 
        }
        public IEnumerable<Consulta_ViabilidadeCaixa> Caixas
        {
            get
            {
                if (_caixas == null)
                {
                    _caixas = new List<Consulta_ViabilidadeCaixa>();
                }

                return _caixas;
            }
            set
            {
                _caixas = value;
            }
        }
        public IEnumerable<string> Passos
        {
            get
            {
                if (_passos == null)
                {
                    _passos = new List<string>();                     
                }

                return _passos;
            }
            set
            {
                _passos = value;
            }
        }

        public StatusConsultaViabilidade PossuiDisponibilidade { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class ViabilidadeResponse : ConsultaViabilidadeResponse 
    {
        public long? ClientePlanoProduto { get; set; }
    }
    public class ExisteConsultaViabilidadeResponse
    {
        public bool ExisteConsulta { get; set; }
        public long? IdConsultaViabilidade { get; set; }
        public IEnumerable<Consulta_ViabilidadeCaixa> Dados { get; set; }
        public StatusConsultaViabilidade? PossuiDisponibilidade { get; set; }
    }
}
