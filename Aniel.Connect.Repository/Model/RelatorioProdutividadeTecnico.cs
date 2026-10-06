using Aniel.Connect.Core;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class RelatorioProdutividadeResponse
    {
        private IEnumerable<RelatorioProdutividadeTecnicoListModel> _data;

        public int Total { get; set; }
        public IEnumerable<RelatorioProdutividadeTecnicoListModel> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<RelatorioProdutividadeTecnicoListModel>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }

    public class RelatorioProdutividadeTecnicoListModel
    {
        public string Matricula { get; set; }
        public string Tecnico { get; set; }

        public string Supervisor { get; set; }
        public string Monitor { get; set; }

        public string NaturezaServico { get; set; }

        public int QtdServicosImprodutivos { get; set; }
        public int QtdServicosProdutivos { get; set; }
        public int TotalServicos
        {
            get
            {
                return this.QtdServicosProdutivos + this.QtdServicosImprodutivos;
            }
        }
        public double? MetaPercentualServicosProdutivos { get; set; }
        public double? PercentualServicosProdutivos
        {
            get
            {
                if (this.TotalServicos > 0)
                    return ((double)this.QtdServicosProdutivos / (double)this.TotalServicos) * 100d;
                else
                    return null;
            }
        }
        public string MetaPercentualServicosProdutivosStr
        {
            get
            {
                if (this.MetaPercentualServicosProdutivos.HasValue)
                    return String.Format("{0:N2}%", this.MetaPercentualServicosProdutivos.Value);
                else
                    return "-";
            }
        }
        public string PercentualServicosProdutivosStr
        {
            get
            {
                if (this.PercentualServicosProdutivos.HasValue)
                    return String.Format("{0:N2}%", this.PercentualServicosProdutivos.Value);
                else
                    return "-";
            }
        }
        public string SituacaoServicosProdutivos {
            get
            {
                if (this.PercentualServicosProdutivos.HasValue && this.MetaPercentualServicosProdutivos.HasValue)
                {
                    if (this.PercentualServicosProdutivos >= this.MetaPercentualServicosProdutivos)
                    {
                        return "Venceu";
                    }
                    else
                    {
                        return "Perdeu";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        public int QtdReincidencia { get; set; }
        public double? MetaPercentualReincidencia { get; set; }
        public double? PercentualReincidencia
        {
            get
            {
                if (this.QtdServicosProdutivos > 0)
                    return ((double)this.QtdReincidencia / (double)this.QtdServicosProdutivos) * 100d;
                else
                    return null;
            }
        }
        public string MetaPercentualReincidenciaStr
        {
            get
            {
                if (this.MetaPercentualReincidencia.HasValue)
                    return String.Format("{0:N2}%", this.MetaPercentualReincidencia.Value);
                else
                    return "-";
            }
        }
        public string PercentualReincidenciaStr
        {
            get
            {
                if (this.PercentualReincidencia.HasValue)
                    return String.Format("{0:N2}%", this.PercentualReincidencia.Value);
                else
                    return "-";
            }
        }
        public string SituacaoReincidencia {
            get
            {
                if (this.PercentualReincidencia.HasValue && this.MetaPercentualReincidencia.HasValue)
                {
                    if (this.PercentualReincidencia <= this.MetaPercentualReincidencia)
                    {
                        return "Venceu";
                    }
                    else
                    {
                        return "Perdeu";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }

        public double? MetaPercentualBaixasDentroPrazo { get; set; }
        public double? PercentualBaixasDentroPrazo
        {
            get
            {
                if (this.TotalServicos > 0)
                    return ((double)this.BaixasDentroPrazo / (double)this.TotalServicos) * 100d;
                else
                    return null;
            }
        }
        public string MetaPercentualBaixasDentroPrazoStr
        {
            get
            {
                if (this.MetaPercentualBaixasDentroPrazo.HasValue)
                    return String.Format("{0:N2}%", this.MetaPercentualBaixasDentroPrazo.Value);
                else
                    return "-";
            }
        }
        public string PercentualBaixasDentroPrazoStr
        {
            get
            {
                if (this.PercentualBaixasDentroPrazo.HasValue)
                    return String.Format("{0:N2}%", this.PercentualBaixasDentroPrazo.Value);
                else
                    return "-";
            }
        }
        public int BaixasDentroPrazo { get; set; }
        public int BaixasForaPrazo { get; set; }
        public string SituacaoPrazo {
            get
            {
                if (this.PercentualBaixasDentroPrazo.HasValue && this.MetaPercentualBaixasDentroPrazo.HasValue)
                {
                    if (this.PercentualBaixasDentroPrazo >= this.MetaPercentualBaixasDentroPrazo)
                    {
                        return "Venceu";
                    }
                    else
                    {
                        return "Perdeu";
                    }
                }
                else
                {
                    return "-";
                }
            }
        }
    }


    public class RelProdTecnicoExtratoProdutividadeListModel
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public int NumDoc { get; set; }
        public string NumObra { get; set; }
        public string Abertura_OS { get; set; }
        public string DataHoraAgendamento { get; set; }
        public string DataHoraBaixa { get; set; }
        public string UsuarioBaixa { get; set; }

        public string TipoServico { get; set; }
        public string Natureza { get; set; }

        public string Equipe { get; set; }
        public string Nome { get; set; }
        public string Supervisor { get; set; }
        public string Monitor { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }

        public string Status { get; set; }
        public string SituacaoPrazo { get; set; }

        public string CodigoEncerramento { get; set; }
        public string DescricaoEncerramento { get; set; }
        public string Status_Auditoria { get; set; }
        public String DataHoraAberturaOriginal { get; set; }
        public String NumeroCliente { get; set; }
    }

    public class RelProdTecnicoExtratoProdutividadeResponse
    {
        private IEnumerable<RelProdTecnicoExtratoProdutividadeListModel> _data;

        public int Total { get; set; }
        public IEnumerable<RelProdTecnicoExtratoProdutividadeListModel> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<RelProdTecnicoExtratoProdutividadeListModel>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }


    public class RelProdTecnicoExtratoPrazoListModel
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public int NumDoc { get; set; }
        public string NumObra { get; set; }

        public string DataHoraInicioAgendamento { get; set; }
        public string DataHoraFimAgendamento { get; set; }
        public string Turno { get; set; }

        public string DataHoraBaixa { get; set; }
        public string DataHoraSLA { get; set; }

        public string TipoServico { get; set; }
        public string Natureza { get; set; }

        public string Equipe { get; set; }
        public string Nome { get; set; }
        public string Supervisor { get; set; }
        public string Monitor { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }

        public string Status { get; set; }
        public string SituacaoPrazoSla { get; set; }
        public string SituacaoPrazoAgendamento { get; set; }
        public string CodigoEncerramento { get; set; }
        public string DescricaoEncerramento { get; set; }
        public string UsuarioBaixa { get; set; }
    }

    public class RelProdTecnicoExtratoPrazoResponse
    {
        private IEnumerable<RelProdTecnicoExtratoPrazoListModel> _data;

        public int Total { get; set; }
        public IEnumerable<RelProdTecnicoExtratoPrazoListModel> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<RelProdTecnicoExtratoPrazoListModel>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }


    public class RelProdTecnicoExtratoReincidenciaListModel
    {
        public string Codct { get; set; }
        public string Projeto { get; set; }
        public int NumDoc { get; set; }

        public string NumObraOsReincidente { get; set; }
        public string DataCriacaoOsReincidente { get; set; }
        public string TipoServicoOsReincidente { get; set; }
        public string NaturezaOsReincidente { get; set; }
        public string DescricaoOsReincidente { get; set; }

        public string NumObraOsAnterior { get; set; }
        public string DataCriacaoOsAnterior { get; set; }
        public string DataBaixaOsAnterior { get; set; }
        public string QtdDiasBaixaServicoAnterior { get; set; }
        public string TipoServicoOsAnterior { get; set; }
        public string NaturezaOsAnterior { get; set; }
        public string DescricaoOsAnterior { get; set; }

        public string Equipe { get; set; }
        public string Nome { get; set; }
        public string Supervisor { get; set; }
        public string Monitor { get; set; }

        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Localidade { get; set; }
    }

    public class RelProdTecnicoExtratoReincidenciaResponse
    {
        private IEnumerable<RelProdTecnicoExtratoReincidenciaListModel> _data;

        public int Total { get; set; }
        public IEnumerable<RelProdTecnicoExtratoReincidenciaListModel> Data
        {
            get
            {
                if (this._data == null)
                    this._data = new List<RelProdTecnicoExtratoReincidenciaListModel>();

                return this._data;
            }

            set
            {
                this._data = value;
            }
        }
    }

    public class RelatorioProdutividadeTecnicoDataTableRequest : DataTableRequest
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
