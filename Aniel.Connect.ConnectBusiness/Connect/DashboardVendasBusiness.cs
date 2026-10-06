using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Repository.DashBoardVendas;
using Aniel.Connect.Repository.Model;
using Microsoft.SqlServer.Server;
using NHibernate;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.DAO.service;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class DashboardVendasBusiness
    {
        private readonly IDashBoardVendasRepository _repo;
        public DashboardVendasBusiness(IDashBoardVendasRepository repo)
        {
            this._repo = repo;
        }
        public async Task<PostApiRetorno<long>> GetDesistencias(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            try
            {
                AddCondicoesFiltroVenda(request, condicoes, parametros);
                var desistencias = await this._repo.GetDesistencias(condicoes, parametros);

                retorno.IdObjeto = desistencias;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<DashBoardMetaResults>> GetMeta(DashBoardVendasPeriodoFiltro request, int? diaConfig)
        {
            var retorno = new PostApiRetorno<DashBoardMetaResults>();
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            try
            {
                #region logica MetaAnterior
                var ano = DateTime.Now.Year;
                string dataInicio;
                string dataFim;

                if (diaConfig <= 30)
                    dataInicio = (Convert.ToInt32(ano) - 1).ToString() + "-12-" + (Convert.ToInt32(diaConfig) + 1).ToString();
                else
                    dataInicio = ano + "-01-" + "01";

                dataFim = ano + "-12-" + diaConfig;


                DateTime dateTimeInicio = Convert.ToDateTime(dataInicio);
                DateTime dateTimeFim = Convert.ToDateTime(dataFim);
                var mes = DateTime.Now.ToString("MMMM").ToUpper();

                if (mes == "MARÇO")
                {
                    string mesTratado = mes.Replace(mes, "MARCO");
                    mes = mesTratado;
                }
                #endregion

                AddCondicoesFiltroMeta(request, condicoes, parametros);
                var equipe = await this._repo.GetMetaEquipe(mes, condicoes, parametros);
                
                parametros.Clear();
                condicoes.Clear();
                var resultados = new List<long>();                
                                   
                AddCondicoesFiltroVenda(request, condicoes, parametros, "abertas");
                resultados.Add(await this._repo.GetVendasAprovadas(condicoes, parametros));

                parametros.Clear();
                condicoes.Clear();
                                
                var results = new DashBoardMetaResults
                {
                    Vendas = resultados.Count > 0 ? resultados.Sum() / equipe.Count : 0,
                    Meta = equipe.Sum(p => p.Value),
                    VendasTotais = resultados.Sum()
                };
                retorno.IdObjeto = results;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<long>> GetVendasAprovadas(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            try
            {
                AddCondicoesFiltroVenda(request, condicoes, parametros);
                var vendasAprovadas = await this._repo.GetVendasAprovadas(condicoes, parametros);
                retorno.IdObjeto = vendasAprovadas;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<long>> GetVendasPendentes(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<long>();
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            try
            {
                AddCondicoesFiltroVenda(request, condicoes, parametros, "abertas");
                var vendasAprovadas = await this._repo.GetVendasPendentes(condicoes, parametros);
                retorno.IdObjeto = vendasAprovadas;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<DashboardConversaoVendasInstaladas>> GetVendasInstaladas(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<DashboardConversaoVendasInstaladas>();
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            try
            {
                AddCondicoesFiltroVenda(request, condicoes, parametros);
                var vendas = await this._repo.GetVendasInstaladas(condicoes, parametros);
                vendas.PercentualVendasInstaladas = vendas.VendasAprovadas > 0 ? Math.Round(Convert.ToDouble(vendas.VendasInstaladas) / Convert.ToDouble(vendas.VendasAprovadas) * 100, 1) : 0;

                retorno.IdObjeto = vendas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<DashBoardVendasByStatus>> GetVendasByStatus(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<DashBoardVendasByStatus>();
            var condicoes = new Dictionary<string, List<string>>();
            var parametros = new Dictionary<string, Dictionary<string, object>>();

            condicoes.Add("VENDAS_PENDENTES", new List<string>());
            condicoes.Add("DESISTENCIAS", new List<string>());
            condicoes.Add("VENDAS_APROVADAS", new List<string>());
            condicoes.Add("VENDAS_INSTALADAS", new List<string>());

            parametros.Add("VENDAS_PENDENTES", new Dictionary<string, object>());
            parametros.Add("DESISTENCIAS", new Dictionary<string, object>());
            parametros.Add("VENDAS_APROVADAS", new Dictionary<string, object>());
            parametros.Add("VENDAS_INSTALADAS", new Dictionary<string, object>());

            var requestCopy = request;
            try
            {
                requestCopy.StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Cancelada };
                AddCondicoesFiltroVenda(request, condicoes["DESISTENCIAS"], parametros["DESISTENCIAS"]);

                requestCopy.StatusFiltrados = new List<Status_Venda>
                {
                    Status_Venda.Analise_Cliente,
                    Status_Venda.Aguardando_Avaliacao_Comercial,
                    Status_Venda.aguardando_Pagamento
                };
                AddCondicoesFiltroVenda(request, condicoes["VENDAS_PENDENTES"], parametros["VENDAS_PENDENTES"], "abertas");

                requestCopy.StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Aprovada };
                AddCondicoesFiltroVenda(requestCopy, condicoes["VENDAS_APROVADAS"], parametros["VENDAS_APROVADAS"]);
                AddCondicoesFiltroVenda(requestCopy, condicoes["VENDAS_INSTALADAS"], parametros["VENDAS_INSTALADAS"]);

                var pendentes = await this._repo.GetVendasPendentes(condicoes["VENDAS_PENDENTES"], parametros["VENDAS_PENDENTES"]);
                var desistencias = await this._repo.GetDesistencias(condicoes["DESISTENCIAS"], parametros["DESISTENCIAS"]);
                var aprovadas = await this._repo.GetVendasAprovadas(condicoes["VENDAS_APROVADAS"], parametros["VENDAS_APROVADAS"]);
                var instaladas = (await this._repo.GetVendasInstaladas(condicoes["VENDAS_INSTALADAS"], parametros["VENDAS_INSTALADAS"])).VendasInstaladas;

                retorno.IdObjeto = new DashBoardVendasByStatus
                {
                    VendasPendentes = pendentes,
                    Desistencias = desistencias,
                    VendasAprovadas = aprovadas,
                    VendasInstaladas = instaladas
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }

        public async Task<PostApiRetorno<DashBoardFunilVendas>> GetFunilVendas(DashBoardVendasPeriodoFiltro request)
        {
            var retorno = new PostApiRetorno<DashBoardFunilVendas>();
            var condicoes = new Dictionary<string, List<string>>();
            var parametros = new Dictionary<string, Dictionary<string, object>>();

            condicoes.Add("LEADS_CADASTRADOS", new List<string>());
            condicoes.Add("VENDAS_CADASTRADAS", new List<string>());
            condicoes.Add("VENDAS_APROVADAS", new List<string>());
            condicoes.Add("VENDAS_INSTALADAS", new List<string>());

            parametros.Add("LEADS_CADASTRADOS", new Dictionary<string, object>());
            parametros.Add("VENDAS_CADASTRADAS", new Dictionary<string, object>());
            parametros.Add("VENDAS_APROVADAS", new Dictionary<string, object>());
            parametros.Add("VENDAS_INSTALADAS", new Dictionary<string, object>());

            var requestCopy = request;
            try
            {
                AddCondicoesFiltroLead(request, condicoes["LEADS_CADASTRADOS"], parametros["LEADS_CADASTRADOS"]);

                requestCopy.StatusFiltrados = new List<Status_Venda>();
                AddCondicoesFiltroVenda(request, condicoes["VENDAS_CADASTRADAS"], parametros["VENDAS_CADASTRADAS"], "abertas");

                requestCopy.StatusFiltrados = new List<Status_Venda> { Status_Venda.Venda_Aprovada };
                AddCondicoesFiltroVenda(requestCopy, condicoes["VENDAS_APROVADAS"], parametros["VENDAS_APROVADAS"]);
                AddCondicoesFiltroVenda(requestCopy, condicoes["VENDAS_INSTALADAS"], parametros["VENDAS_INSTALADAS"]);

                var funil = await this._repo.GetFunilVendas(condicoes, parametros);

                retorno.IdObjeto = funil;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retorno;
        }
        #region privateMethods
        private void AddCondicoesFiltroVenda(DashBoardVendasPeriodoFiltro request, List<string> condicoes, Dictionary<string, object> parametros, string consulta = "", bool fimDia = false)
        {
            var dataAtual = fimDia ? DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59) : DateTime.Now;
            int diasDiferenca = 0;
            string referenciaHorario = consulta == "abertas" ? "DATAHORACRIACAO" : "DATA_VENDA";
            DateTime periodo;
            switch (request.Filtro)
            {
                // Date implica em passar o dia atual a 0 hora
                case EnumPeriodoFitlroDashVendas.DataAtual:
                    condicoes.Add($"C.{referenciaHorario} BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    parametros.Add("@DATA_INICIO", dataAtual.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EstaSemana:
                    condicoes.Add($"C.{referenciaHorario} BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    diasDiferenca = (int)dataAtual.DayOfWeek - (int)DayOfWeek.Monday;
                    periodo = diasDiferenca < 0 ? dataAtual.AddDays(-7) : dataAtual.AddDays(-diasDiferenca);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos7Dias:
                    condicoes.Add($"C.{referenciaHorario} BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-7);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EsteMes:
                    condicoes.Add($"C.{referenciaHorario} BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos30Dias:
                    condicoes.Add($"C.{referenciaHorario} BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-30);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;
            }
            using (var session = HibernateUtil.GetSession())
            using (var repository = new Conexao<IdentityUser>(session))
            {
                var usuario = repository.selectFirst(p => p.Id == request.Usuario).Result;                

                if (request.FormatoVisualizacao != 2)
                {
                    string condicao = (Enum_VisualizacaoDashboard)request.FormatoVisualizacao == Enum_VisualizacaoDashboard.TODOS ? "" : "A.ID = @USUARIO";
                    if (!String.IsNullOrEmpty(condicao))
                    {
                        condicoes.Add(condicao);
                    }
                    parametros.Add("@USUARIO", request.Usuario);
                }
                else 
                {
                    parametros.Add("E_SUPERVISOR", true);
                    parametros.Add("@USUARIO", usuario.Funcionario.Supervisor.Num_Fre);
                }
            }
            if (request.StatusFiltrados.Count >= 1)
            {
                var dadosListagem = request.StatusFiltrados.Count > 0 ? request.StatusFiltrados.Select(p => "'" + Convert.ToInt32(p) + "'").ToList() : new List<string> { "'" + Convert.ToInt32(request.StatusFiltrados.First()) + "'" };
                condicoes.Add($" C.STATUS IN ({String.Join(", ", dadosListagem)})");
            }
        }

        private void AddCondicoesFiltroMeta(DashBoardVendasPeriodoFiltro request, List<string> condicoes, Dictionary<string, object> parametros)
        {
            if (request.FormatoVisualizacao != 2)
            {
                string condicao = (Enum_VisualizacaoDashboard)request.FormatoVisualizacao == Enum_VisualizacaoDashboard.TODOS ? "" : "A.ID = @USUARIO";
                if (!String.IsNullOrEmpty(condicao))
                {
                    condicoes.Add(condicao);
                    parametros.Add("@USUARIO", request.Usuario);                    
                }
                else
                {
                    //parametros.Add("E_SUPERVISOR", true);                    
                }                
            }
            else 
            {
                using (var session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {
                    var usuario = repository.selectFirst(p => p.Id == request.Usuario).Result;
                    parametros.Add("E_SUPERVISOR", true);
                    parametros.Add("@USUARIO", usuario.Funcionario.Supervisor.Num_Fre);
                    condicoes.Add("E.NUM_FRE = @USUARIO");
                }                
            }            
            condicoes.Add($" B.ANO = '{DateTime.Now.Year}'");
            //condicoes.Add($" E.ID IS NOT NULL");
            condicoes.Add($" D.SITUACAO = 0");
            condicoes.Add($" A.SECAO_CONNECT = 1");

        }
        private void AddCondicoesFiltroComissao(DashBoardVendasPeriodoFiltro request, List<string> condicoes, Dictionary<string, object> parametros)
        {
            var dataAtual = DateTime.Now;

            int diasDiferenca = 0;
            DateTime periodo;
            switch (request.Filtro)
            {
                // Date implica em passar o dia atual a 0 hora
                case EnumPeriodoFitlroDashVendas.DataAtual:
                    condicoes.Add("D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    parametros.Add("@DATA_INICIO", dataAtual.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EstaSemana:
                    condicoes.Add("D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    diasDiferenca = (int)dataAtual.DayOfWeek - (int)DayOfWeek.Monday;
                    periodo = diasDiferenca < 0 ? dataAtual.AddDays(-7) : dataAtual.AddDays(-diasDiferenca);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos7Dias:
                    condicoes.Add("D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-7);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EsteMes:
                    condicoes.Add("D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos30Dias:
                    condicoes.Add("D.DATA_VENDA BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-30);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;
            }
            if (request.FormatoVisualizacao != 2)
            {
                string condicao = (Enum_VisualizacaoDashboard)request.FormatoVisualizacao == Enum_VisualizacaoDashboard.TODOS ? "" : "A.ID = @USUARIO";
                if (!String.IsNullOrEmpty(condicao))
                {
                    condicoes.Add(condicao);
                }
                else
                {
                    parametros.Add("E_SUPERVISOR", true);
                }
                parametros.Add("@USUARIO", request.Usuario);
            }
        }
        private void AddCondicoesFiltroLead(DashBoardVendasPeriodoFiltro request, List<string> condicoes, Dictionary<string, object> parametros, bool fimDia = false)
        {
            var dataAtual = fimDia ? DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59) : DateTime.Now;
            int diasDiferenca = 0;
            DateTime periodo;
            switch (request.Filtro)
            {
                // Date implica em passar o dia atual a 0 hora
                case EnumPeriodoFitlroDashVendas.DataAtual:
                    condicoes.Add("C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    parametros.Add("@DATA_INICIO", dataAtual.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EstaSemana:
                    condicoes.Add("C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    diasDiferenca = (int)dataAtual.DayOfWeek - (int)DayOfWeek.Monday;
                    periodo = diasDiferenca < 0 ? dataAtual.AddDays(-7) : dataAtual.AddDays(-diasDiferenca);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos7Dias:
                    condicoes.Add("C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-7);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.EsteMes:
                    condicoes.Add("C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;

                case EnumPeriodoFitlroDashVendas.Ultimos30Dias:
                    condicoes.Add("C.DATAHORACRIACAO BETWEEN @DATA_INICIO AND @DATA_ATUAL");
                    periodo = dataAtual.AddDays(-30);
                    parametros.Add("@DATA_INICIO", periodo.Date);
                    parametros.Add("@DATA_ATUAL", dataAtual);
                    break;
            }
            if (request.FormatoVisualizacao != 2)
            {
                string condicao = (Enum_VisualizacaoDashboard)request.FormatoVisualizacao == Enum_VisualizacaoDashboard.TODOS ? "" : "A.ID = @USUARIO";
                if (!String.IsNullOrEmpty(condicao))
                {
                    condicoes.Add(condicao);
                }
                else
                {
                    parametros.Add("E_SUPERVISOR", true);
                }
                parametros.Add("@USUARIO", request.Usuario);
            }
        }
        #endregion
        
        public GetApiRetorno<PainelVendasFiltrosDashBoard> GetFiltrosPainelVendas(string periodoFiltrado, string tipoFiltro, string formato, string usuario) 
        {
            var retorno = new GetApiRetorno<PainelVendasFiltrosDashBoard>();
            var filtros = new PainelVendasFiltrosDashBoard();
            try 
            {
                using (ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
                using (var repositoryUser = new Sinapse.DAO.repository.Conexao<IdentityUser>(session))
                using (var repositoryFuncionario = new Sinapse.DAO.repository.Conexao<Funcionario>(session))
                {
                    var user = repositoryUser.QueryableSelect().Where(p => p.Id == usuario).FirstOrDefault();
                    var supervisor = repositoryFuncionario.QueryableSelect().Where(p => p == user.Funcionario).FirstOrDefault()?.Supervisor;
                    if (formato != "2")
                    {                        
                        if (user.Funcionario != null && !user.Funcionario.E_SUPERVISOR && formato == "1")
                        {
                            filtros.Id = user?.Funcionario?.Id;                            
                        }
                        else if (user.Funcionario.E_SUPERVISOR && formato == "1")
                        {
                            filtros.Id = user?.Funcionario?.Id; 
                            filtros.Supervisor = true;
                        }
                    }
                    else 
                    {
                        filtros.Id = supervisor?.Id;
                        filtros.Supervisor = true;
                    }
                }

                Dashboard_Comercial filtroDashBoard;
                EnumPeriodoFitlroDashVendas dataFiltrada;
                Enum.TryParse(periodoFiltrado, out dataFiltrada);
                Enum.TryParse(tipoFiltro, out filtroDashBoard);

                DateTime? dataInicio = null;
                DateTime dataAtual = DateTime.Today.AddHours(23).AddMinutes(59);
                int diasDiferenca = 0;
                switch (dataFiltrada)
                {
                    case EnumPeriodoFitlroDashVendas.DataAtual:
                        dataInicio = dataAtual.Date;
                        break;

                    case EnumPeriodoFitlroDashVendas.EstaSemana:
                        diasDiferenca = (int)dataAtual.DayOfWeek - (int)DayOfWeek.Monday;
                        dataInicio = diasDiferenca < 0 ? dataAtual.AddDays(-7) : dataAtual.AddDays(-diasDiferenca);
                        break;

                    case EnumPeriodoFitlroDashVendas.Ultimos7Dias:
                        dataInicio = dataAtual.AddDays(-7);
                        break;

                    case EnumPeriodoFitlroDashVendas.EsteMes:
                        dataInicio = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                        break;

                    case EnumPeriodoFitlroDashVendas.Ultimos30Dias:
                        dataInicio = dataAtual.AddDays(-30);
                        break;
                }
                switch (filtroDashBoard)
                {
                    case Dashboard_Comercial.Vendas_Pendentes: 
                        filtros.StatusFiltrados.AddRange(new List<Status_Venda> { Status_Venda.Analise_Cliente, Status_Venda.Aguardando_Liberacao, Status_Venda.Aguardando_Avaliacao_Comercial });
                        break;

                    case Dashboard_Comercial.Desistencias: 
                        filtros.StatusFiltrados.Add(Status_Venda.Venda_Cancelada);
                        break;

                    case Dashboard_Comercial.Vendas_Aprovadas: 
                        filtros.StatusFiltrados.Add(Status_Venda.Venda_Aprovada);
                        break;
                        
                    case Dashboard_Comercial.Vendas_Instaladas: 
                        filtros.StatusFiltrados.Add(Status_Venda.Venda_Aprovada);
                        filtros.ApenasInstalados = true;
                        break;                        
                }
                
                filtros.DataInicial = dataInicio.Value.ToString("dd/MM/yyyy");
                filtros.DataAtual = dataAtual.ToString("dd/MM/yyyy");

                retorno.Objeto = filtros;
                retorno.Ok = true;
            }             
            catch (Exception ex) 
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;                
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
    }    
}
