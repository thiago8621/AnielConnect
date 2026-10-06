using Aniel.Connect.Core;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class PainelPerformanceTecnicoResponse
    {
        private IEnumerable<PainelPerformanceTecnicoListModel> _data;

        public int Total { get; set; }
        public IEnumerable<PainelPerformanceTecnicoListModel> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<PainelPerformanceTecnicoListModel>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }

    public class PainelPerformanceTecnicoListModel
    {
        public string Matricula { get; set; }
        public string Tecnico { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        //public string QtdServicosAtrbuidas { get; set; }
        //public string QtdServicosRepetidosGarantia { get; set; }
        public int TotalServicos
        {
            get
            {
                return this.QtdServicosProdutivos + this.QtdServicosImprodutivos + this.QtdServicosAbertos;
            }
        }
        public int QtdServicosAbertos { get; set; }
        public int QtdServicosImprodutivos { get; set; }
        public int QtdServicosProdutivos { get; set; }
        

        public string PercentualEfetividade
        {
            get
            {
                return this.QtdServicosProdutivos == 0 ? "0" : Math.Round(((double)this.QtdServicosProdutivos / (double)(this.QtdServicosProdutivos + this.QtdServicosImprodutivos + this.QtdServicosAbertos)) * 100,1) + "%";
            }
        }

        public double TotalKmEsperado { get; set; }
        public double TotalKmReal { get; set; }
        public double VariacaoKm
        {
            get
            {
                return this.TotalKmEsperado - this.TotalKmReal;
            }
        }
        public string PercentualDeslocamentoRota
        {
            get
            {
                return this.TotalKmReal == 0 ? "0%" : (Math.Round(this.TotalKmEsperado / this.TotalKmReal,1) * 100) + "%";
            }
        }
        public double TempoDeslocamentoPrevisto { get; set; }
        public double TempoDeslocamentoReal { get; set; }
        public double DesperdicioDeslocamento { get {
                return Math.Round(this.TempoDeslocamentoPrevisto - this.TempoDeslocamentoReal, 1);
            }
        }
        public string PercentualEfetividadeDeslocamento {
            get
            {
                return this.TempoDeslocamentoReal == 0 ? "0%" : (Math.Round(this.TempoDeslocamentoPrevisto / this.TempoDeslocamentoReal, 1) * 100) + "%";
            }
        }
        public double TempoExecucaoPrevisto { get; set; }
        //public string TempoExecucaoPrevistoHora { 
        //    get {
        //        return TimeSpan.FromHours(this.TempoExecucaoPrevisto).ToString(@"hh\:mm");
        //    } 
        //}
        public double TempoExecucaoReal { get; set; }
        public double DesperdicioExecucao {
            get
            {
                return Math.Round(this.TempoExecucaoPrevisto - this.TempoExecucaoReal,1);
            }
        }
        public string AgilidadeProdutivas
        {
            get
            {
                return this.TempoExecucaoReal == 0 ?  "0%" : (Math.Round(this.TempoExecucaoPrevisto / this.TempoExecucaoReal, 1) * 100) + "%";
            }
        }
        public double TempoNovoDeslocamento { get; set; }
        public double TempoTodosServicos { get; set; }
        public double TempoIntervaloJornalada { get; set; }
        public double TotalParadoLegitimo
        {
            get
            {
                return Math.Round(this.TempoNovoDeslocamento + this.TempoTodosServicos + this.TempoIntervaloJornalada, 1);
            }
        }
        public double TempoMotorParalisado { get; set; }
        //public int PercentualAgilidadeProdutiva { get; set; }
        //public int TempoMotorParado { get; set; }
        //public int TempoParadoLegitimo { get; set; }
        //public int TempoIntervaloJornada { get; set; }
        //public int TempoParadoTotal { get; set; }
        public double VariacaoTempoParado
        {
            get
            {
                return Math.Round(this.TempoMotorParalisado - this.TotalParadoLegitimo);
            }
        }
        //public int DesperdicioParado { get; set; }
        public string PercentualEfetividadeParado {
            get
            {
                return this.TempoMotorParalisado == 0 ? "0%" : (Math.Round(this.TotalParadoLegitimo / this.TempoMotorParalisado,1) * 100) + "%";
            }
        }
        public double TempoJornadaUtil { get; set; }
        public double TempoJornadaProdutiva { get; set; }
        public double DesperdicioTotal
        {
            get
            {
                return Math.Round(this.TempoJornadaUtil - this.TempoJornadaProdutiva, 1);
            }
        }

        public string PercentualProdutividade
        {
            get
            {
                return this.TempoJornadaProdutiva == 0 ? "0%" : (Math.Round(this.TempoJornadaUtil / this.TempoJornadaProdutiva, 1) * 100) + "%";
            }
        }
        //public int MediaProdutividade { get; set; }
    }

    public class PainelPerformanceTecnicoDataTableRequest : DataTableRequest
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }

        public string Monitor { get; set; }
        public string Supervisor { get; set; }
        public string Equipe { get; set; }
        public int? Natureza { get; set; }
    }

    


}
