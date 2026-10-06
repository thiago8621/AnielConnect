using Aniel.Connect.Repository.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.DashBoardVendas
{
    public interface IDashBoardVendasRepository
    {
        Task<long> GetDesistencias(IList<string> Condicoes, Dictionary<string, object> parametros);
        Task<long> GetVendasAprovadas(IList<string> Condicoes, Dictionary<string, object> parametros);
        Task<long> GetVendasPendentes(IList<string> Condicoes, Dictionary<string, object> parametros);
        Task<DashboardConversaoVendasInstaladas> GetVendasInstaladas(IList<string> Condicoes, Dictionary<string, object> parametros);
        Task<DashBoardFunilVendas> GetFunilVendas(Dictionary<string,List<string>> Condicoes, Dictionary<string, Dictionary<string,object>> parametros);
        Task<Dictionary<long,double>> GetMetaEquipe(string Mes, IList<string> Condicoes, Dictionary<string, object> Parametros);
        Task<List<double>> GetComissao(Dictionary<long, double> MetaFuncionarios, IList<string> Condicoes, Dictionary<string, object> Parametros);        
    }
}
