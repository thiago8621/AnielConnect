using Aniel.Connect.Repository.Model;
using NHibernate;
using NHibernate.Util;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.DashBoardVendas
{
    public class DashBoardVendasRepository : IDashBoardVendasRepository
    {
        private readonly ISession _session;
        private readonly ISession _sessionT5;
        public DashBoardVendasRepository(ISession session, ISession sessionT5)
        {
            _session = session;
            _sessionT5 = sessionT5;
        }
        public async Task<long> GetDesistencias(IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            long desistencias = 0;
            using (var comm = this._session.Connection.CreateCommand())
            {
                if (Parametros.ContainsKey("E_SUPERVISOR"))
                {
                    var usuarios = new List<string>();
                    var parametrosUsuario = Parametros.Where(p => p.Key == "E_SUPERVISOR" || p.Key == "@USUARIO").ToDictionary(p => p.Key, p => p.Value);
                    //Select
                    using (var com = this._session.Connection.CreateCommand())
                    {
                        var sqluser = new StringBuilder();
                        sqluser.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                        sqluser.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                        sqluser.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                        sqluser.AppendLine($"LEFT JOIN TB_META B");
                        sqluser.AppendLine($"ON(E.ID_META = B.ID)");
                        sqluser.AppendLine("Where E.NUM_FRE = @USUARIO");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqluser, parametrosUsuario, this._session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(Convert.ToString(reader["ID"]));
                            }
                        }

                        Condicoes.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                        Parametros.Remove("@USUARIO");
                    }
                }
                var sql = new StringBuilder("SELECT COUNT(C.ID) CO");

                sql.AppendLine(" FROM ASPNETUSERS A");
                sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO B");
                sql.AppendLine(" ON(B.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine(" ON(D.ID = B.ID_SUPERVISOR)");              
                sql.AppendLine(" LEFT JOIN TB_VENDA C");
                sql.AppendLine(" ON (C.ID_VENDEDOR = B.ID)");
                if (Condicoes.Count > 0)
                {
                    sql.AppendLine(" WHERE " + String.Join(" AND ", Condicoes.ToList()));
                }

                using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros, this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        desistencias = Convert.ToInt64(reader["CO"]);
                    }
                }
            }
            return desistencias;
        }
        public async Task<Dictionary<long, double>> GetMetaEquipe(string mes, IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            var metaFuncionario = new Dictionary<long, double>();

            if (Parametros.ContainsKey("E_SUPERVISOR"))
            {
                var usuarios = new List<string>();
                //Select
                using (var com = this._session.Connection.CreateCommand())
                {                                        
                    var sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                    sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                    sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                    sql.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                    sql.AppendLine($"LEFT JOIN TB_META B");
                    sql.AppendLine($"ON(E.ID_META = B.ID)");

                    sql.AppendLine($"WHERE {String.Join(" AND ", Condicoes)} ");
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, Parametros, this._session.Connection, null, 10, 0))
                    {
                        while (await reader.ReadAsync())
                        {
                            usuarios.Add(Convert.ToString(reader["ID"]));
                        }
                    }
                    if (usuarios.Count > 0) 
                    {
                        Condicoes.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                    }                
                }
            }
            using (var com = this._session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($" SELECT B.{mes} AS META, D.ID AS ID_FUNCIONARIO FROM ASPNETUSERS A");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                sql.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");

                sql.AppendLine($" LEFT JOIN TB_META B");
                sql.AppendLine($" ON(D.ID_META = B.ID)");

                if (Condicoes.Count > 0)
                {
                    sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                }
                sql.AppendLine($" GROUP BY B.{mes} ,ID_FUNCIONARIO");                

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, Parametros, this._session.Connection, null, 10, 0))
                {
                    while (await reader.ReadAsync())
                    {
                        var equipe = Convert.ToInt64(reader["ID_FUNCIONARIO"]);
                        var meta = reader["META"] != DBNull.Value ? Convert.ToDouble(reader["META"]) : 0;

                        metaFuncionario.Add(equipe, meta);
                    }
                }
            }
            return metaFuncionario;
        }
        public async Task<List<double>> GetComissao(Dictionary<long, double> MetaFuncionarios, IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            List<double> comissoes = new List<double>();
            foreach (KeyValuePair<long, double> funcionario in MetaFuncionarios)
            {
                using (var comm = this._session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT SUM(F.QUANTIDADE) AS PONTOS ");

                    sql.AppendLine(" FROM ASPNETUSERS A ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO B "); // VENDEDOR DO USUÁRIO LOGADO
                    sql.AppendLine(" ON(B.ID = A.ID_FUNCIONARIO) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C  "); //SUPERVISOR
                    sql.AppendLine(" ON(C.ID = B.ID_SUPERVISOR) ");
                    sql.AppendLine(" LEFT JOIN TB_VENDA D ");
                    sql.AppendLine(" ON(D.ID_SUPERVISOR = C.ID) ");
                    sql.AppendLine(" INNER JOIN TB_CLIENTE_PLANO_PRODUTO E ");
                    sql.AppendLine(" ON(E.ID_VENDA = D.ID) ");
                    sql.AppendLine(" INNER JOIN TB_COMISSAO_PLANO_PRODUTO F ");
                    sql.AppendLine(" ON(F.ID_COMISSIONAMENTO = D.ID_COMISSIONAMENTO AND(F.ID_PLANO = E.ID_PLANO OR F.ID_PRODUTO = E.ID_PRODUTO)) ");
                    if (Condicoes.Count > 0)
                    {
                        sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros, this._session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            comissoes.Add(reader["PONTOS"] != DBNull.Value ? Convert.ToDouble(reader["PONTOS"]) : 0);
                        }
                    }
                }
            }
            return comissoes;
        }
        public async Task<long> GetVendasAprovadas(IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            long vendasAprovadas = 0;
            //Select
            using (var comm = this._session.Connection.CreateCommand())
            {
                if (Parametros.ContainsKey("E_SUPERVISOR"))
                {
                    var usuarios = new List<string>();
                    var parametrosUsuario = Parametros.Where(p=>p.Key == "E_SUPERVISOR" || p.Key == "@USUARIO").ToDictionary(p => p.Key, p => p.Value);
                    
                    //Select
                    using (var com = this._session.Connection.CreateCommand())
                    {
                        var sqluser = new StringBuilder();
                        sqluser.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                        sqluser.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                        sqluser.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                        sqluser.AppendLine($"LEFT JOIN TB_META B");
                        sqluser.AppendLine($"ON(E.ID_META = B.ID)");
                        sqluser.AppendLine("Where E.NUM_FRE = @USUARIO");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqluser, parametrosUsuario, this._session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(Convert.ToString(reader["ID"]));
                            }
                        }

                        Condicoes.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                        Parametros.Remove("@USUARIO");
                    }
                }
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine($" LEFT JOIN TB_VENDA C");
                sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                if (Condicoes.Count > 0)
                {
                    sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                }

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros, this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        vendasAprovadas = Convert.ToInt64(reader["CO"]);
                    }
                }
            }
            return vendasAprovadas;
        }
        public async Task<long> GetVendasPendentes(IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            long vendasPendentes = 0;
            //Select
            using (var comm = this._session.Connection.CreateCommand())
            {
                if (Parametros.ContainsKey("E_SUPERVISOR"))
                {
                    var usuarios = new List<string>();
                    var parametrosUsuario = Parametros.Where(p => p.Key == "E_SUPERVISOR" || p.Key == "@USUARIO").ToDictionary(p => p.Key, p => p.Value);
                    //Select
                    using (var com = this._session.Connection.CreateCommand())
                    {
                        var sqluser = new StringBuilder();
                        sqluser.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                        sqluser.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                        sqluser.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                        sqluser.AppendLine($"LEFT JOIN TB_META B");
                        sqluser.AppendLine($"ON(E.ID_META = B.ID)");
                        sqluser.AppendLine("Where E.NUM_FRE = @USUARIO");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqluser, parametrosUsuario, this._session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(Convert.ToString(reader["ID"]));
                            }
                        }

                        Condicoes.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                        Parametros.Remove("@USUARIO");
                    }
                }
                StringBuilder sql = new StringBuilder();

                sql.AppendLine($" SELECT COUNT(C.ID) CO FROM ASPNETUSERS A");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine($" LEFT JOIN TB_VENDA C");
                sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros, this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        vendasPendentes = Convert.ToInt64(reader["CO"]);
                    }
                }
            }
            return vendasPendentes;
        }
        public async Task<DashboardConversaoVendasInstaladas> GetVendasInstaladas(IList<string> Condicoes, Dictionary<string, object> Parametros)
        {
            var listagemVendas = new List<long>();
            var ConversaoVendasInstaladas = new DashboardConversaoVendasInstaladas();
            using (var comm = this._session.Connection.CreateCommand())
            {
                if (Parametros.ContainsKey("E_SUPERVISOR"))
                {
                    var usuarios = new List<string>();
                    var parametrosUsuario = Parametros.Where(p => p.Key == "E_SUPERVISOR" || p.Key == "@USUARIO").ToDictionary(p => p.Key, p => p.Value);
                    //Select
                    using (var com = this._session.Connection.CreateCommand())
                    {
                        var sqluser = new StringBuilder();
                        sqluser.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                        sqluser.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                        sqluser.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                        sqluser.AppendLine($"LEFT JOIN TB_META B");
                        sqluser.AppendLine($"ON(E.ID_META = B.ID)");
                        sqluser.AppendLine("Where E.NUM_FRE = @USUARIO");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqluser, parametrosUsuario, this._session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(Convert.ToString(reader["ID"]));
                            }
                        }

                        Condicoes.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                        Parametros.Remove("@USUARIO");
                    }
                }
                var sql = new StringBuilder();
                sql.AppendLine($" SELECT C.ID CO FROM ASPNETUSERS A");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine($" LEFT JOIN TB_VENDA C");
                sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros, this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        listagemVendas.Add(Convert.ToInt64(reader["CO"]));
                    }
                }
            }
            ConversaoVendasInstaladas.VendasAprovadas = listagemVendas.Count;

            if (listagemVendas.Count > 0)
            {
                var listaQuebrada = ListService.GetListaQuebrada(listagemVendas, 100);
                foreach (var p in listaQuebrada)
                {
                    using (var comm = this._sessionT5.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();
                        sql.AppendLine($" SELECT COUNT (A.CODCT) CO FROM TB_DOCUMENTO_PRODUCAO A");
                        sql.AppendLine($" WHERE A.ID_VENDA IN ({String.Join(", ", p.Value)}) AND  STATUS_EXECUCAO = 2");
                        using (var reader = await CommandHelper.ExecuteReader(comm, sql, this._sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (Convert.ToInt32(reader["CO"]) != 0)
                                {
                                    ConversaoVendasInstaladas.VendasInstaladas += 1;
                                }
                            }
                        }
                    }
                }
            }

            return ConversaoVendasInstaladas;
        }

        public async Task<DashBoardFunilVendas> GetFunilVendas(Dictionary<string, List<string>> Condicoes, Dictionary<string, Dictionary<string, Object>> Parametros)
        {
            var listagemVendas = new List<long>();
            var funil = new DashBoardFunilVendas();

            long leadsCadastrados = 0;
            using (var comm = this._session.Connection.CreateCommand())
            {
                if (Parametros.ContainsKey("E_SUPERVISOR"))
                {
                    var usuarios = new List<string>();
                    var parametrosUsuario = Parametros["LEADS_CADASTRADOS"].Where(p => p.Key == "E_SUPERVISOR" || p.Key == "@USUARIO").ToDictionary(p => p.Key, p => p.Value);
                    //Select
                    using (var com = this._session.Connection.CreateCommand())
                    {
                        var sqluser = new StringBuilder();
                        sqluser.AppendLine("SELECT A.ID FROM ASPNETUSERS A");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                        sqluser.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                        sqluser.AppendLine($" LEFT JOIN TB_FUNCIONARIO E");
                        sqluser.AppendLine($" ON(E.ID = D.ID_SUPERVISOR)");
                        sqluser.AppendLine($"LEFT JOIN TB_META B");
                        sqluser.AppendLine($"ON(E.ID_META = B.ID)");
                        sqluser.AppendLine("Where E.NUM_FRE = @USUARIO");
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sqluser, parametrosUsuario, this._session.Connection, null, 10, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                usuarios.Add(Convert.ToString(reader["ID"]));
                            }
                        }
                        foreach (var condicoes in Condicoes) 
                        {
                            condicoes.Value.Add("A.ID IN (" + string.Join(", ", usuarios.Select(p => $"'{p}'").ToList()) + ")");
                        }
                        foreach (var parametros in Parametros) 
                        {
                            parametros.Value.Remove("@USUARIO");
                        }
                        
                    }
                }
                StringBuilder sql = new StringBuilder();

                List<string> condicoesLead = new List<string>();
                var parametrosLead = new Dictionary<string, object>();

                sql.AppendLine($" SELECT COUNT(CLIENT.ID) CO FROM ASPNETUSERS A");
                sql.AppendLine($" LEFT JOIN TB_FUNCIONARIO D");
                sql.AppendLine($" ON(D.ID = A.ID_FUNCIONARIO)");
                sql.AppendLine($" LEFT JOIN TB_VENDA C");
                sql.AppendLine($" ON(C.ID_VENDEDOR = D.ID)");
                sql.AppendLine($" LEFT JOIN TB_CLIENTE CLIENT ON (C.ID_CLIENTE = CLIENT.ID)");
                if (Condicoes.Count > 0)
                {
                    sql.AppendLine($" WHERE {String.Join(" AND ", Condicoes["LEADS_CADASTRADOS"])}");
                }

                //Busca com paginacao
                using (var reader = await CommandHelper.ExecuteReader(comm, sql, Parametros["LEADS_CADASTRADOS"], this._session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        leadsCadastrados = Convert.ToInt64(reader["CO"]);
                    }
                }
            }

            long vendasCadastradas = await this.GetVendasAprovadas(Condicoes["VENDAS_CADASTRADAS"], Parametros["VENDAS_CADASTRADAS"]);
            long vendasAprovadas = await this.GetVendasAprovadas(Condicoes["VENDAS_APROVADAS"], Parametros["VENDAS_APROVADAS"]);
            long vendasInstaladas = (await this.GetVendasInstaladas(Condicoes["VENDAS_INSTALADAS"], Parametros["VENDAS_INSTALADAS"])).VendasInstaladas;

            funil.Leads_Cadastrados = leadsCadastrados;
            funil.Vendas_Cadastradas = vendasCadastradas;
            funil.Vendas_Aprovadas = vendasAprovadas;
            funil.Vendas_Instaladas = vendasInstaladas;

            return funil;

        }
    }
}
