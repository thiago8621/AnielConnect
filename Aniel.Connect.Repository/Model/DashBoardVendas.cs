using Aniel.Connect.Model.enumerado;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class DashBoardVendasPeriodoFiltro
    {
        public EnumPeriodoFitlroDashVendas Filtro { get; set; }
        public int FormatoVisualizacao { get; set; }
        public string Usuario { get; set; }
        private List<Status_Venda> _StatusFiltrados;
        public List<Status_Venda> StatusFiltrados 
        {
            get 
            {
                if (_StatusFiltrados == null) 
                {
                    _StatusFiltrados = new List<Status_Venda>();
                }
                return _StatusFiltrados;
            }
            set { _StatusFiltrados = value; } 
        }
    }
    public class DashboardConversaoVendasInstaladas
    {
        public double PercentualVendasInstaladas { get; set; }
        public long VendasAprovadas { get; set; }
        public long VendasInstaladas { get; set; }
        
    }
    public class DashBoardMetaResults 
    {
        public double VendasTotais { get; set; }
        public double Vendas { get; set; }
        public double Meta { get; set; }
    }
    public class DashBoardFunilVendas
    {
        public long Leads_Cadastrados { get; set; }
        public long Vendas_Aprovadas { get; set; }
        public long Vendas_Cadastradas { get; set; }
        public long Vendas_Instaladas { get; set; }        
    }
    public class DashBoardVendasByStatus 
    {
        public long Desistencias { get; set; }
        public long VendasAprovadas { get; set; }
        public long VendasInstaladas { get; set; }
        public long VendasPendentes { get; set; }
    }
    public class DashBoardComparacaoMeta 
    {
        public double Alcancado { get; set; }
        public double Meta { get; set; }
    }
}
