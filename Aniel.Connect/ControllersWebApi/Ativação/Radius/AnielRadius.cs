using Aniel.Connect.ControllersWebApi.Ativacao.Interfaces;
using Aniel.Connect.Model.nhibernate;
using NetTools;
using NHibernate;
using Renci.SshNet;
using Sinapse.Util.RemoteConnection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Aniel.Connect.Models;
using Aniel.Connect.Models.Ativacao;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Radius
{
    public class AnielRadius : IntegracaoRadius
    {
        private Model.nhibernate.Radius radius;
        private ISession sessionRadius;

        public AnielRadius(Model.nhibernate.Radius radius)
        {
            this.radius = radius;
            if(String.IsNullOrEmpty(this.radius.Dominio))
                throw new Exception($"Usuário do Radius \"{this.radius.Descricao}\" não configurado");

            if (String.IsNullOrEmpty(this.radius.Senha))
                throw new Exception($"Senha do Radius \"{this.radius.Descricao}\" não configurada");
        
            if(String.IsNullOrEmpty(this.radius.Ip))
                throw new Exception($"Endereço Ip do Radius \"{this.radius.Descricao}\" não configurado");

            //TODO: Necessário definir estrutura para salvar credenciais do banco para criar session dinâmica
            if(!String.IsNullOrEmpty(this.radius.Usuario_Banco) && !String.IsNullOrEmpty(this.radius.Senha_Banco))
            {
                sessionRadius = Sinapse.DAO.repository.HibernateUtil.GetSessionRadius(this.radius.Ip, this.radius.Usuario_Banco, this.radius.Senha_Banco);
            }               
        }

        //Não usar por enquanto
        public ISession GetSessionRadius()
        {
            return sessionRadius;
        }

        #region Concentradora

        public async Task CadastrarConcentradora(ISession session, Concentradora concentradora)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("INSERT INTO nas (nasname, shortname, type, ports, secret, server, community, Id_Concentradora) ");
                sql.AppendLine("VALUES (@NASNAME, @SHORTNAME, @TYPE, @PORTS, @SECRET, @SERVER, @COMMUNITY, @IdConcentradora) ");

                parametros.Add("@NASNAME", concentradora.Ip);
                parametros.Add("@SHORTNAME", concentradora.Nome);

                switch (concentradora.Tipo)
                {
                    default:
                        parametros.Add("@TYPE", "other");
                        break;
                }

                parametros.Add("@PORTS", null);
                parametros.Add("@SECRET", "testing123"); // SECRET DE TESTE
                parametros.Add("@SERVER", null);
                parametros.Add("@COMMUNITY", null);
                parametros.Add("@IdConcentradora", concentradora.Id);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                ReiniciarServicoRadius();
            }
        }

        public async Task EditarConcentradora(ISession session, Concentradora concentradora)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine("UPDATE nas SET ");
                sql.AppendLine("nasname = @NASNAME, ");
                sql.AppendLine("shortname = @SHORTNAME,");
                sql.AppendLine("type = @TYPE,ports = @PORTS, ");
                sql.AppendLine("server = @SERVER, community = @COMMUNITY ");
                sql.AppendLine("WHERE Id_Concentradora = @IdConcentradora ");

                parametros.Add("@NASNAME", concentradora.Ip);
                parametros.Add("@SHORTNAME", concentradora.Nome);

                switch (concentradora.Tipo)
                {
                    default:
                        parametros.Add("@TYPE", "other");
                        break;
                }

                parametros.Add("@PORTS", null);
                parametros.Add("@SERVER", null);
                parametros.Add("@COMMUNITY", null);
                parametros.Add("@IdConcentradora", concentradora.Id);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                ReiniciarServicoRadius();
            }
        }

        public async Task DeletarConcentradora(ISession session, long idConcentradora)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine("DELETE FROM nas WHERE Id_Concentradora = @IdConcentradora ");

                parametros.Add("@IdConcentradora", idConcentradora);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                ReiniciarServicoRadius();
            }
        }

        #endregion

        #region Usuário

        public async Task CadastrarUsuario(ISession session, string Usuario, string Senha, string NasIp, long idCliente, Pool_Ip pool = null, string mac = null)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                // TODO: Estudar a questão das conexões agarradas com o Simultaneous-Use = 1 antes de adicionar novamente

                sql.AppendLine("INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES ");
                sql.AppendLine("                     (@USERNAME, 'Cleartext-Password', ':=', @SENHA, @IDCLIENTE), ");
                //sql.AppendLine("                     (@USERNAME, 'Simultaneous-Use' , ':=', '1', @IDCLIENTE), ");
                sql.AppendLine("                     (@USERNAME, 'Framed-Protocol', ':=', 'PPP', @IDCLIENTE), ");
                sql.AppendLine("                     (@USERNAME, 'NAS-IP-Address', '==', @NAS_IP, @IDCLIENTE) ");

                if (pool != null)
                {
                    if(pool.IsPoolConcentradora) //Pool controlada por Concentradora
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Framed-Pool', '==', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else if(pool.IsDistribuicaoBlocoIp) //Pool oferecida como bloco de IPs fixos
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Framed-Route', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else if(!pool.IsPoolConcentradora) //Pool controlada pelo Radius
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Pool-Name', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else //Default
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Pool-Name', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }                    
                }

                if (mac != null)
                {
                    sql.AppendLine("                   ,(@USERNAME, 'Calling-Station', ':=', @MAC, @IDCLIENTE) ");
                }

                parametros.Add("@USERNAME", Usuario);
                parametros.Add("@SENHA", Senha);
                parametros.Add("@NAS_IP", NasIp);
                parametros.Add("@IDCLIENTE", idCliente);
                parametros.Add("@POOL_NAME", pool.Descricao);
                parametros.Add("@MAC", mac);


                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task CadastrarUsuario(ISession session, string usuario, string senha, long idCliente)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES ");
                sql.AppendLine("                      (@USERNAME, 'Cleartext-Password', ':=', @SENHA, @IDCLIENTE) ");

                parametros.Add("@USERNAME", usuario);
                parametros.Add("@SENHA", senha);
                parametros.Add("@IDCLIENTE", idCliente);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task FinalizarCadastroUsuario(ISession session, string usuario, string nasIp, long idCliente, Pool_Ip pool = null, string mac = null)
        {
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES ");
                sql.AppendLine("                      (@USERNAME, 'Simultaneous-Use', ':=', '1', @IDCLIENTE), ");
                sql.AppendLine("                      (@USERNAME, 'Framed-Protocol', ':=', 'PPP', @IDCLIENTE), ");
                sql.AppendLine("                      (@USERNAME, 'NAS-IP-Address', '==', @NAS_IP, @IDCLIENTE) ");

                if (pool!= null)
                {
                    if (pool.IsPoolConcentradora) //Pool controlada por Concentradora
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Framed-Pool', '==', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else if (pool.IsDistribuicaoBlocoIp) //Pool oferecida como bloco de IPs fixos
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Framed-Route', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else if (!pool.IsPoolConcentradora) //Pool controlada pelo Radius
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Pool-Name', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }
                    else //Default
                    {
                        sql.AppendLine("                   ,(@USERNAME, 'Pool-Name', ':=', @POOL_NAME, @IDCLIENTE) ");
                    }

                    parametros.Add("@POOL_NAME", pool.Descricao);
                }

                if (mac != null)
                {
                    sql.AppendLine(",(@USERNAME, 'Calling-Station', ':=', @MAC, @IDCLIENTE) ");
                    parametros.Add("@MAC", mac);
                }

                parametros.Add("@USERNAME", usuario);
                parametros.Add("@IDCLIENTE", idCliente);
                parametros.Add("@NAS_IP", nasIp);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }


        public async Task EditarUsuario(ISession session, string Usuario, string Senha, string NasIp, long idCliente, Pool_Ip pool = null, string mac = null)
        {
            var cadastroSimplificado = false;
            var temIpPoolCadastrado = false;
            var temMacCadastrado = false;
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT COUNT(id) as QTD_REGISTROS FROM radcheck WHERE Id_Cliente = @IDCLIENTE AND attribute LIKE 'Calling-Station' ");
                parametros.Add("@IDCLIENTE", idCliente);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        temMacCadastrado = Convert.ToInt32(reader["QTD_REGISTROS"]) == 1;
                    }
                }
            }

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT COUNT(id) as QTD_REGISTROS FROM radcheck WHERE Id_Cliente = @IDCLIENTE AND (attribute LIKE 'Pool-Name' OR attribute LIKE 'Framed-Route' OR attribute LIKE 'Framed-Pool') ");
                parametros.Add("@IDCLIENTE", idCliente);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        temIpPoolCadastrado = Convert.ToInt32(reader["QTD_REGISTROS"]) == 1;
                    }
                }
            }

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" SELECT COUNT(id) as QTD_REGISTROS FROM radcheck WHERE Id_Cliente = @IDCLIENTE ");
                parametros.Add("@IDCLIENTE", idCliente);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        cadastroSimplificado = Convert.ToInt32(reader["QTD_REGISTROS"]) == 1;
                    }
                }
            }

            using (var com = session.Connection.CreateCommand())
            {
                if (cadastroSimplificado)
                {
                    await FinalizarCadastroUsuario(session, Usuario, NasIp, idCliente, pool, mac);
                }

                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radcheck set username = @USERNAME WHERE Id_Cliente = @IDCLIENTE;");
                sql.AppendLine("UPDATE radcheck set value = @SENHA WHERE attribute = 'Cleartext-Password' and Id_Cliente = @IDCLIENTE;");
                sql.AppendLine("UPDATE radcheck set value = @NAS_IP WHERE attribute = 'NAS-IP-Address' and Id_Cliente = @IDCLIENTE;");

                if (pool != null)
                {
                    if (!temIpPoolCadastrado)
                    {                        
                        if (pool.IsPoolConcentradora) //Pool controlada por Concentradora
                        {
                            sql.AppendLine(" INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES (@USERNAME, 'Framed-Pool', '==', @POOL_NAME, @IDCLIENTE);");
                        }
                        else if (pool.IsDistribuicaoBlocoIp) //Pool oferecida como bloco de IPs fixos
                        {
                            sql.AppendLine(" INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES (@USERNAME, Framed-Route', '==', @POOL_NAME, @IDCLIENTE);");                            
                        }
                        else if (!pool.IsPoolConcentradora) //Pool controlada pelo Radius
                        {
                            sql.AppendLine(" INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES (@USERNAME, 'Pool-Name', '==', @POOL_NAME, @IDCLIENTE);");                            
                        }
                    }
                    else
                    {
                        sql.AppendLine("UPDATE radcheck set value = @POOL_NAME WHERE attribute = 'Pool-Name' and Id_Cliente = @IDCLIENTE;");

                        if (pool.IsPoolConcentradora) //Pool controlada por Concentradora
                        {
                            sql.AppendLine("UPDATE radcheck set value = @POOL_NAME WHERE attribute = 'Framed-Pool' and Id_Cliente = @IDCLIENTE;");                            
                        }
                        else if (pool.IsDistribuicaoBlocoIp) //Pool oferecida como bloco de IPs fixos
                        {
                            sql.AppendLine("UPDATE radcheck set value = @POOL_NAME WHERE attribute = 'Framed-Route' and Id_Cliente = @IDCLIENTE;");                            
                        }
                        else if (!pool.IsPoolConcentradora) //Pool controlada pelo Radius
                        {
                            sql.AppendLine("UPDATE radcheck set value = @POOL_NAME WHERE attribute = 'Pool-Name' and Id_Cliente = @IDCLIENTE;");                            
                        }
                    }

                    parametros.Add("@POOL_NAME", pool.Descricao);
                }

                if (mac != null)
                {
                    if (!temMacCadastrado)
                    {
                        sql.AppendLine("INSERT INTO radcheck (username, attribute, op, value, Id_Cliente) VALUES (@USERNAME, 'Calling-Station', ':=', @MAC, @IDCLIENTE);");
                    }
                    else
                    {
                        sql.AppendLine("UPDATE radcheck set value = @MAC WHERE attribute = 'Calling-Station' and Id_Cliente = @IDCLIENTE;");
                    }

                    parametros.Add("@MAC", mac);
                }

                parametros.Add("@USERNAME", Usuario);
                parametros.Add("@SENHA", Senha);
                parametros.Add("@NAS_IP", NasIp);
                parametros.Add("@IDCLIENTE", idCliente);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task EditarUsuario(ISession session, string Usuario, string Senha, long idCliente)
        {
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine("UPDATE radcheck set username = @USERNAME WHERE Id_Cliente = @IDCLIENTE;");
                sql.AppendLine("UPDATE radcheck set value = @SENHA WHERE attribute = 'Cleartext-Password' and Id_Cliente = @IDCLIENTE;");

                parametros.Add("@USERNAME", Usuario);
                parametros.Add("@SENHA", Senha);

                parametros.Add("@IDCLIENTE", idCliente);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task DeletarUsuario(ISession session, long idCliente)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine("DELETE FROM radcheck WHERE Id_Cliente = @IdCliente; ");
                sql.AppendLine("DELETE FROM radreply WHERE id_Cliente = @IdCliente;");
                parametros.Add("@IdCliente", idCliente);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }


        public async Task BloquearUsuario(ISession session, string Usuario, string IpConcentradora, string PortaIncoming, string Secret)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radusergroup set groupname = 'Inativo' WHERE username = @USERNAME;");

                parametros.Add("@USERNAME", Usuario);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }

            using (var rc = new RemoteConnection(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    if (String.IsNullOrEmpty(PortaIncoming))
                        PortaIncoming = "3799";

                    string comando = CONSTANTES.UsaRadiusSemContainer == true ? "echo \"User-Name = " + Usuario + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123"
                        : "docker exec -i radius bash -c 'echo \"User-Name = " + Usuario + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123'";

                    rc.ExecuteCommand(comando);
                }
            }
        }

        public async Task DesbloquearUsuario(ISession session, string Usuario, string Plano)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radusergroup set groupname = @PLANO WHERE username = @USERNAME;");

                string nomePlanoTratado = Plano.Replace(",", " ");

                parametros.Add("@PLANO", nomePlanoTratado);
                parametros.Add("@USERNAME", Usuario);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task BloquearSinalPorPool(ISession session, string Usuario, string IpConcentradora, string PortaIncoming, string nomePoolBloqueio)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radcheck set value = @POOLNAME WHERE username = @USERNAME AND attribute = 'Pool-Name';");

                parametros.Add("@USERNAME", Usuario);
                parametros.Add("@POOLNAME", nomePoolBloqueio);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }

            using (var rc = new RemoteConnection(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    if (String.IsNullOrEmpty(PortaIncoming))
                        PortaIncoming = "3799";

                    string comando = CONSTANTES.UsaRadiusSemContainer == true ? "echo \"User-Name = " + Usuario + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123"
                        : "docker exec -i radius bash -c 'echo \"User-Name = " + Usuario + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123'";

                    rc.ExecuteCommand(comando);
                }
            }
        }

        public async Task DesbloquearSinalPorPool(ISession session, string Usuario, string NomePool)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radcheck set value = @POOLNAME WHERE username = @USERNAME AND attribute = 'Pool-Name';");

                parametros.Add("@USERNAME", Usuario);
                parametros.Add("@POOLNAME", NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        //Apenas para HUAWEI
        public async Task BloquearSinalPorDomain(ISession session, String Login, String IpConcentradora, String PortaIncoming, String IdDomainName, long IdCliente)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("INSERT INTO radreply (username, `attribute`, op, value, id_Cliente)");
                sql.AppendLine("VALUES");
                sql.Append($" (@USERNAME, 'Huawei-Domain-Name', '=', @DOMAIN_NAME, @ID_CLIENTE); ");

                parametros.Add("@USERNAME", Login);
                parametros.Add("@DOMAIN_NAME", IdDomainName);
                parametros.Add("@ID_CLIENTE", IdCliente);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }

            using (var rc = new RemoteConnection(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    if (String.IsNullOrEmpty(PortaIncoming))
                        PortaIncoming = "3799";

                    string comando = CONSTANTES.UsaRadiusSemContainer == true ? "echo \"User-Name = " + Login + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123"
                        : "docker exec -i radius bash -c 'echo \"User-Name = " + Login + "\" | radclient " + IpConcentradora + ":" + PortaIncoming + " disconnect testing123'";

                    rc.ExecuteCommand(comando);
                }
            }
        }

        //Apenas para HUAWEI
        public async Task DesbloquearSinalPorDomain(ISession session, ISession sessionConnect, LoginRadius login)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("DELETE FROM radreply ");
                sql.AppendLine("WHERE `attribute` = 'Huawei-Domain-Name' ");
                sql.AppendLine("  AND id_Cliente = @ID_CLIENTE");
                sql.AppendLine("  AND username = @USERNAME");

                parametros.Add("@USERNAME", login.Login);
                parametros.Add("@ID_CLIENTE", login.Cliente.Id);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }

            using (var rc = new RemoteConnection(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {
                    if (String.IsNullOrEmpty(login.Concentradora.PortaIncoming))
                        login.Concentradora.PortaIncoming = "3799";

                    string comando = CONSTANTES.UsaRadiusSemContainer == true ? "echo \"User-Name = " + login.Login + "\" | radclient " + login.Concentradora.Ip + ":" + login.Concentradora.PortaIncoming + " disconnect testing123"
                        : "docker exec -i radius bash -c 'echo \"User-Name = " + login.Login + "\" | radclient " + login.Concentradora.Ip + ":" + login.Concentradora.PortaIncoming + " disconnect testing123'";

                    rc.ExecuteCommand(comando);
                }
            }
        }

        public async Task AssociaUsuarioAoPlano(ISession session, string UsuarioPPPOE, string NomePlano, string Prioridade, long IdClientePlanoProduto)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                sql.AppendLine(" INSERT INTO radusergroup (username, groupname, priority, idCliente_PlanoProduto) VALUES ");
                sql.AppendLine("                          (@USERNAME, @GROUPNAME, @PRIORITY, @IDCLIENTE_PLANO_PRODUTO); ");

                string nomePlanoTratado = NomePlano.Replace(",", " ");

                parametros.Add("@USERNAME", UsuarioPPPOE);
                parametros.Add("@GROUPNAME", nomePlanoTratado);
                parametros.Add("@PRIORITY", Prioridade);
                parametros.Add("@IDCLIENTE_PLANO_PRODUTO", IdClientePlanoProduto);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task CadastrarPoolIp(ISession session, string NomePool, IPAddressRange RangeIP)
        {
            using (var com = session.Connection.CreateCommand())
            {
                int i = 1;
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                foreach (var ip in RangeIP)
                {
                    sql.AppendLine(" INSERT INTO radippool (pool_name, framedipaddress) VALUES ");
                    sql.AppendLine($" (@NOMEPOOL, @IP{i}); ");

                    parametros.Add($"@IP{i}", ip.ToString());

                    i++;
                }

                parametros.Add("@NOMEPOOL", NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task EditarPoolIp(ISession session, string NomePool, IPAddressRange Novo_RangeIP, string Novo_NomePool = null)
        {

            await DeletarPoolIp(session, NomePool);
            if (!String.IsNullOrEmpty(Novo_NomePool))
            {
                await CadastrarPoolIp(session, Novo_NomePool, Novo_RangeIP);
                //Editar na Radcheck caso o nome da pool tenha mudado
                await AtualizarPoolIpDosUsuarios(session, NomePool, Novo_NomePool);
            }
            else
                await CadastrarPoolIp(session, NomePool, Novo_RangeIP);
        }

        public async Task DeletarPoolIp(ISession session, string NomePool)
        {
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" DELETE FROM radippool ");
                sql.AppendLine(" WHERE pool_name = @POOLNAME ");

                parametros.Add("@POOLNAME", NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task DesvincularPoolIpDosUsuarios(ISession session, string NomePool)
        {
            // WHERE attribute = 'Pool-Name' and value = @POOL_NAME and Id_Cliente = @IDCLIENTE;"
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine(" DELETE FROM radcheck ");
                sql.AppendLine(" WHERE attribute = 'Pool-Name' and value = @POOL_NAME "/*and Id_Cliente = @IDCLIENTE*/ );

                parametros.Add("@POOL_NAME", NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task AtualizarPoolIpDosUsuarios(ISession session, string Antigo_PoolName, string Novo_NomePool)
        {
            // WHERE attribute = 'Pool-Name' and value = @POOL_NAME and Id_Cliente = @IDCLIENTE;"
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radcheck set value = @POOL_NAME_NOVO WHERE attribute = 'Pool-Name' and value = @POOL_NAME_ANTIGO " /*and Id_Cliente = @IDCLIENTE;"*/);

                parametros.Add("@POOL_NAME_ANTIGO", Antigo_PoolName);
                parametros.Add("@POOL_NAME_NOVO", Novo_NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task AtualizarPoolIpUsuario(ISession session, long Id_Cliente, string Novo_NomePool)
        {
            // WHERE attribute = 'Pool-Name' and value = @POOL_NAME and Id_Cliente = @IDCLIENTE;"
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("UPDATE radcheck set value = @POOL_NAME_NOVO WHERE and Id_Cliente = @IDCLIENTE ");

                parametros.Add("@IDCLIENTE", Id_Cliente);
                parametros.Add("@POOL_NAME_NOVO", Novo_NomePool);

                await Sinapse.DAO.repository.CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        public async Task<string> GetPoolBloqueioDisponivel(ISession session, List<String> Pools_Bloqueio)
        {
            List<String> listaRetorno = new List<String>();

            //SELECT pool_name FROM radippool WHERE(username IS NULL OR username LIKE '')
            //AND pool_name IN('Pool1', 'Pool2')
            //GROUP BY pool_name
            string listaPools = string.Join(", ", Pools_Bloqueio.Where(p => !string.IsNullOrEmpty(p)).Select(p => string.Format("'{0}'", p)));
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("SELECT pool_name FROM radippool WHERE(username IS NULL OR username LIKE '') ");
                sql.AppendLine(String.Format("AND pool_name IN({0})", Pools_Bloqueio));
                sql.AppendLine("GROUP BY pool_name");

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        listaRetorno.Add(Convert.ToString(reader["pool_name"]));
                    }
                }
            }

            return listaRetorno.First();
        }

        public async Task<string> GetPoolReducaoVelocidadeDisponivel(ISession session, List<String> Pools_Reducao)
        {
            List<String> listaRetorno = new List<String>();
            
            string listaPools = string.Join(", ", Pools_Reducao.Where(p => !string.IsNullOrEmpty(p)).Select(p => string.Format("'{0}'", p)));
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                sql.AppendLine("SELECT pool_name FROM radippool WHERE(username IS NULL OR username LIKE '') ");
                sql.AppendLine(String.Format("AND pool_name IN({0})", Pools_Reducao));
                sql.AppendLine("GROUP BY pool_name");

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        listaRetorno.Add(Convert.ToString(reader["pool_name"]));
                    }
                }
            }

            return listaRetorno.First();
        }

        public bool TesteLogin(string ipConcentradora, string usuario, string senha)
        {
            var retorno = false;
            var logSh = new List<String>();
            using (var rc = new RemoteConnection(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha, Protocol.Ssh))
            {
                rc.Connect();

                if (rc.IsConnected())
                {


                    rc.InteractiveExecute<ShellStream>((sh) =>
                    {
                        string comando = CONSTANTES.UsaRadiusSemContainer == true ? $"radtest {usuario} {senha} {this.radius.Ip} 0 testing123 1 {ipConcentradora}"
                        : $"docker exec radius sh -c \"radtest {usuario} {senha} {this.radius.Ip} 0 testing123 1 {ipConcentradora}\"";

                        sh.ReadTimeout = 10000;
                        sh.WriteLine(comando);
                        string currLine;
                        Thread.Sleep(2000);

                        var precisaPermissao = false;
                        while ((currLine = sh.ReadLine()) != null)
                        {
                            logSh.Add(currLine);
                            Console.WriteLine(currLine);
                            if (currLine.Contains("Received Access-Accept"))
                            {
                                retorno = true;
                                break;
                            }
                            else if (currLine.Contains("Received Access-Reject"))
                            {
                                break;
                            }
                            else if (currLine.Contains("permission denied"))
                            {
                                precisaPermissao = true;
                                break;
                            }
                        }

                        if (precisaPermissao)
                        {
                            sh.WriteLine($"sudo {comando}");
                            Thread.Sleep(1500);

                            sh.WriteLine(this.radius.Senha);
                            while ((currLine = sh.ReadLine()) != null)
                            {
                                Console.WriteLine(currLine);
                                logSh.Add(currLine);
                                if (currLine.Contains("Received Access-Accept"))
                                {
                                    retorno = true;
                                    break;
                                }
                                else if (currLine.Contains("Received Access-Reject") || currLine.Contains("No reply from server"))
                                {
                                    break;
                                }
                            }
                        }

                    });
                }
            }

            return retorno;
        }

        #endregion

        public void ReiniciarServicoRadius()
        {
            using (SshClient client = new SshClient(this.radius.Ip, Convert.ToInt32(this.radius.Porta_SSH), this.radius.Dominio, this.radius.Senha))
            {
                client.Connect();
                var command = client.CreateCommand("");
                command.Execute("docker restart radius");
                client.Disconnect();

            }
        }

        public async Task<GetHistoricoConexaoRadius_Response> GetHistoricoConexao(ISession session, GetHistoricoConexaoRadius_DataTableRequest request)
        {
            GetHistoricoConexaoRadius_Response retorno = new GetHistoricoConexaoRadius_Response();
            Int32 recordsTotal = 0;
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            StringBuilder sqlFrom = new StringBuilder("FROM radacct A")
                .AppendLine(" WHERE A.username = @USERNAME ");

            parametros.Add("@USERNAME", request.Login_Radius);

            //Count
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder("SELECT COUNT(A.radacctid) CO ");
                sql.AppendLine(sqlFrom.ToString());

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        recordsTotal = Convert.ToInt32(reader["CO"]);
                    }
                }
            }

            List<RelatorioHistoricoConexaoModel> listaRetorno = new List<RelatorioHistoricoConexaoModel>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                
                sql.AppendLine("SELECT COALESCE(A.framedipaddress, A.framedipv6address) AS ENDERECO_IP, A.callingstationid AS MAC, A.connectinfo_start AS HORA_LOGIN, ");
                sql.AppendLine(" A.connectinfo_stop AS HORA_LOGOUT, A.acctinputoctets AS DOWNLOAD, A.acctoutputoctets AS UPLOAD, A.acctterminatecause AS MOTIVO_DESCONEXAO ");
                //sql.AppendLine(" FROM radacct A ");
                //sql.AppendLine(" WHERE A.username = @USERNAME");
                sql.AppendLine(sqlFrom.ToString());
                          
                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                {
                    while (await reader.ReadAsync())
                    {
                        listaRetorno.Add(new RelatorioHistoricoConexaoModel { 
                            Endereco_Ip = Convert.ToString(reader["ENDERECO_IP"]),
                            Mac = Convert.ToString(reader["MAC"]),
                            Hora_Login = FormatPadraoData(Convert.ToString(reader["HORA_LOGIN"])),
                            Hora_Logout = FormatPadraoData(Convert.ToString(reader["HORA_LOGOUT"])),
                            Tempo_Conectado = FormatTempoConectado(Convert.ToString(reader["HORA_LOGIN"]), Convert.ToString(reader["HORA_LOGOUT"])),
                            Upload = reader["UPLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToInt64(reader["UPLOAD"])) : "-",
                            Download = reader["DOWNLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToInt64(reader["DOWNLOAD"])) : "-",
                            Motivo_Desconexao = reader["MOTIVO_DESCONEXAO"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_DESCONEXAO"]) : "-"
                        });
                    }
                }
            }

            retorno.lista = listaRetorno;
            retorno.recordsTotal = recordsTotal;

            return retorno;
        }

        public async Task<List<ConsumoPeriodoModel>> GetConsumoPeriodo(ISession session, String Login_Radius, HistoricoConsumoPeriodoEnum Periodo_Consumo, DateTime? Data_Inicio, DateTime? Data_Fim)
        {
            List<ConsumoPeriodoModel> retorno = new List<ConsumoPeriodoModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            //BASEADO NO EXEMPLO
            /*
             --      SELECT
                --          DATE_FORMAT(period_start, '%Y-%M') AS month,
                --          SUM(acctinputoctets)/1000/1000/1000 AS GB_in,
                --          SUM(acctoutputoctets)/1000/1000/1000 AS GB_out
                --      FROM
                --          data_usage_by_period
                --      WHERE
                --          username='bob' AND
                --          period_end IS NOT NULL
                --      GROUP BY
                --          YEAR(period_start), MONTH(period_start);
                --
                --      +----------------+----------------+-----------------+
                --      | month          | GB_in          | GB_out          |
                --      +----------------+----------------+-----------------+
                --      | 2019-July      | 5.782279230000 | 50.545664820000 |
                --      | 2019-August    | 4.230543340000 | 48.523096420000 |
                --      | 2019-September | 4.847360590000 | 48.631835480000 |
                --      | 2019-October   | 6.456763250000 | 51.686231930000 |
                --      | 2019-November  | 6.362537730000 | 52.385710570000 |
                --      | 2019-December  | 4.301524440000 | 50.762240270000 |
                --      | 2020-January   | 5.436280540000 | 49.067775280000 |
                --      +----------------+----------------+-----------------+
             */

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                string sqlDateFormat = String.Empty;
                StringBuilder sqlGroupBy = new StringBuilder();

                //Caso não seja consulta por periodo dinâmico, Data_Fim passa a ser a data do momento da consulta
                if (!Data_Fim.HasValue && Periodo_Consumo == HistoricoConsumoPeriodoEnum.Periodo)
                    Data_Fim = DateTime.Now;

                switch (Periodo_Consumo)
                {                    
                    case HistoricoConsumoPeriodoEnum.Dia:
                        sqlDateFormat = "A.period_start";
                        //sqlGroupBy.AppendLine(" GROUP BY HOUR(A.period_start), DAY(A.period_start);");
                        parametros.Add("@DATA_INICIO", String.Format("{0}", Data_Fim.Value.AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm:ss")));
                        parametros.Add("@DATA_FIM", String.Format("{0}", Data_Fim.Value.ToString("yyyy/MM/dd HH:mm:ss")));                        
                        break;
                    case HistoricoConsumoPeriodoEnum.Semana:
                        sqlDateFormat = "A.period_start";
                        //sqlDateFormat = "DATE_FORMAT(A.period_start, '%W')";
                       // sqlGroupBy.AppendLine(" GROUP BY DAY(A.period_start);");
                        parametros.Add("@DATA_INICIO", String.Format("{0}", Data_Fim.Value.AddDays(-6).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm")));
                        parametros.Add("@DATA_FIM", String.Format("{0}", Data_Fim.Value.ToString("yyyy/MM/dd HH:mm:ss")));
                        break;
                    case HistoricoConsumoPeriodoEnum.Mes:
                        sqlDateFormat = "A.period_start";
                        //sqlDateFormat = "DATE_FORMAT(A.period_start, '%d/%m')";
                        //sqlGroupBy.AppendLine(" GROUP BY DAY(A.period_start);");
                        parametros.Add("@DATA_INICIO", String.Format("{0}", Data_Fim.Value.AddDays(-29).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString("yyyy/MM/dd HH:mm")));
                        parametros.Add("@DATA_FIM", String.Format("{0}", Data_Fim.Value.ToString("yyyy/MM/dd HH:mm:ss")));
                        break;                                        
                    case HistoricoConsumoPeriodoEnum.Periodo:
                        sqlDateFormat = "A.period_start";
                        //sqlDateFormat = "DATE_FORMAT(A.period_start, '%m/%Y')";
                        //sqlGroupBy.AppendLine(" GROUP BY DAY(A.period_start);");
                        parametros.Add("@DATA_INICIO", String.Format("{0}", Data_Inicio.Value.ToString("yyyy/MM/dd HH:mm:ss")));
                        parametros.Add("@DATA_FIM", String.Format("{0}", Data_Fim.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("yyyy/MM/dd HH:mm:ss")));
                        break;
                    default:
                        break;
                }               


                sql.AppendLine("SET SESSION sql_mode=(SELECT REPLACE(@@sql_mode,'ONLY_FULL_GROUP_BY','')); ");
                sql.AppendLine("SET lc_time_names = 'pt_PT'; ");           
                //if(Periodo_Consumo == HistoricoConsumoPeriodoEnum.Dia)
                    sql.AppendLine($"SELECT {sqlDateFormat} AS PERIODO, A.acctinputoctets AS DOWNLOAD, A.acctoutputoctets AS UPLOAD ");
                //else
                    //sql.AppendLine($"SELECT {sqlDateFormat} AS PERIODO, SUM(A.acctinputoctets) AS DOWNLOAD, SUM(A.acctoutputoctets) AS UPLOAD ");
                sql.AppendLine(" FROM data_usage_by_period A ");
                sql.AppendLine(" WHERE A.username=@USERNAME AND A.period_end IS NOT NULL AND (A.period_start > @DATA_INICIO AND A.period_start < @DATA_FIM) ");
                sql.AppendLine(sqlGroupBy.ToString());                

                parametros.Add("@USERNAME", Login_Radius);                                                
               
                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ConsumoPeriodoModel
                        {
                            DataHoraConsumoFormatada = Convert.ToDateTime(Convert.ToString(reader["PERIODO"])).ToString("yyyy-MM-dd HH:mm:ss"),
                            DataHoraConsumo = Convert.ToDateTime(Convert.ToString(reader["PERIODO"])),
                            UploadString = reader["UPLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToDouble(reader["UPLOAD"])) : "-",
                            Upload = reader["UPLOAD"] != DBNull.Value ? Convert.ToDouble(reader["UPLOAD"]) : 0.0,
                            DownloadString = reader["DOWNLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToInt64(reader["DOWNLOAD"])) : "-",
                            Download = reader["DOWNLOAD"] != DBNull.Value ? Convert.ToDouble(reader["DOWNLOAD"]) : 0.0
                        });
                    }
                }
            }

            return retorno;
        }

        public async Task<ConsumoPeriodoModel> GetConexaoTempoReal(ISession session, String Login_Radius, Medida_Velocidade_ConexaoEnum Unidade_Medida_Velocidade)
        {
            ConsumoPeriodoModel retorno = new ConsumoPeriodoModel();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            //CALL fr_new_data_usage_period_by_user(usuario);
            //SELECT A.period_start AS PERIODO, A.acctinputoctets AS DOWNLOAD, A.acctoutputoctets AS UPLOAD 
            //FROM data_usage_by_period A 
            //WHERE A.username = 'yago.dias' AND A.period_end IS NOT NULL            
            //ORDER BY A.period_end DESC LIMIT 1

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();                                   
                               
                sql.AppendLine("SET lc_time_names = 'pt_PT'; ");

                #if DEBUG
                    sql.AppendLine(UpdateRegistroRadacct()); //TODO: REMOVER ESSA ROTINA POIS ELA GERA DADOS DE TESTE NO RADIUS
                #endif

                sql.AppendLine(" CALL fr_new_data_usage_period_by_user(@USERNAME); ");
                sql.AppendLine($" SELECT A.period_start AS PERIODO, A.acctinputoctets AS DOWNLOAD, A.acctoutputoctets AS UPLOAD ");                
                sql.AppendLine(" FROM data_usage_by_period A ");
                sql.AppendLine(" WHERE A.username=@USERNAME AND A.period_end IS NOT NULL ");
                sql.AppendLine(" ORDER BY A.period_end DESC LIMIT 1; ");                

                parametros.Add("@USERNAME", Login_Radius);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if(await reader.ReadAsync())
                    {
                        retorno = new ConsumoPeriodoModel
                        {
                            DataHoraConsumoFormatada = Convert.ToDateTime(Convert.ToString(reader["PERIODO"])).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                            DataHoraConsumo = Convert.ToDateTime(Convert.ToString(reader["PERIODO"])).ToLocalTime(),
                            UploadString = reader["UPLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToDouble(reader["UPLOAD"]), Unidade_Medida_Velocidade) : "-",
                            Upload = reader["UPLOAD"] != DBNull.Value ? Convert.ToDouble(reader["UPLOAD"]) : 0.0,
                            DownloadString = reader["DOWNLOAD"] != DBNull.Value ? OctetsToKbMbGb(Convert.ToInt64(reader["DOWNLOAD"]), Unidade_Medida_Velocidade) : "-",
                            Download = reader["DOWNLOAD"] != DBNull.Value ? Convert.ToDouble(reader["DOWNLOAD"]) : 0.0
                        };
                    }
                }
            }

            return retorno;
        }

        public async Task<List<TempoOnlineOfflineModel>> GetTempoOnline(ISession session, String Login_Radius)
        {
            List<TempoOnlineOfflineModel> retorno = new List<TempoOnlineOfflineModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();


            List<dynamic> lista = new List<dynamic>();
            StringBuilder sql = new StringBuilder();
            /*SELECT connectinfo_start AS DATAHORA_INICIO_CONEXAO, connectinfo_stop AS DATAHORA_FIM_CONEXAO
            FROM radacct 
            WHERE username = 'yago.dias' 
            AND (
            (connectinfo_start > '2022/09/01 00:00:00' AND connectinfo_start < '2022/09/08 11:14:00')
            OR
            (connectinfo_stop > '2022/09/01 00:00:00' AND connectinfo_stop < '2022/09/08 11:14:00')
            )*/
            DateTime data_atual = DateTime.Now;
            using (var com = session.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT connectinfo_start AS DATAHORA_INICIO_CONEXAO, connectinfo_stop AS DATAHORA_FIM_CONEXAO ");
                sql.AppendLine("FROM radacct ");
                sql.AppendLine("WHERE username = @USERNAME ");
                sql.AppendLine("AND ((connectinfo_start > @DATA_INICIO AND connectinfo_start < @DATA_FIM) ");
                sql.AppendLine("OR (connectinfo_stop > @DATA_INICIO AND connectinfo_stop < @DATA_FIM)) ");

                parametros.Add("@DATA_INICIO", String.Format("{0}", data_atual.AddDays(-7).ToString("yyyy/MM/dd HH:mm")));
                parametros.Add("@DATA_FIM", String.Format("{0}", data_atual.ToString("yyyy/MM/dd HH:mm:ss")));
                parametros.Add("@USERNAME", Login_Radius);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new 
                        {
                            DataHora_Inicio_Conexao = Convert.ToDateTime(Convert.ToString(reader["DATAHORA_INICIO_CONEXAO"])).ToLocalTime(),
                            DataHora_Termino_Conexao = Convert.ToDateTime(Convert.ToString(reader["DATAHORA_FIM_CONEXAO"])).ToLocalTime()
                        });
                    }
                }
            }       
            
            for(var i = -7; i < 0; i++)
            {
                retorno.Add(new TempoOnlineOfflineModel
                {
                    Data_Conexao = DateTime.Now.AddDays(i).Date,
                    Data_Conexao_String = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd")
                });
            }
            retorno.Add(new TempoOnlineOfflineModel
            {
                Data_Conexao = DateTime.Now.Date,
                Data_Conexao_String = DateTime.Now.ToString("yyyy-MM-dd")
            });

            if (lista.Count > 0)
            {
                foreach(var item in lista)
                {
                    
                    var dataHora_Inicio = ((DateTime)item.DataHora_Inicio_Conexao);
                    var dataHora_Fim = ((DateTime)item.DataHora_Termino_Conexao);

                    var model = retorno.FirstOrDefault(p => p.Data_Conexao.Value == dataHora_Inicio.Date || p.Data_Conexao.Value == dataHora_Fim.Date);
                    if (model != null)
                    {
                        var horasOnline = model.Horas_Online_Offline;
                        if (dataHora_Inicio.Date == dataHora_Fim.Date) //Login e Logoff no mesmo dia
                        {
                            horasOnline += dataHora_Fim.Subtract(dataHora_Inicio).Hours;
                        }
                        else if (dataHora_Inicio.Date == model.Data_Conexao.Value.Date) //Login no dia mas Logoff não
                        {
                            horasOnline += model.Data_Conexao.Value.AddHours(23).AddMinutes(59).AddSeconds(59).Subtract(dataHora_Inicio).Hours;
                        }
                        else //Logoff no dia mas Login não
                        {
                            horasOnline += model.Data_Conexao.Value.AddHours(23).AddMinutes(59).AddSeconds(59).Subtract(dataHora_Fim).Hours;
                        }

                        if (horasOnline > 24) horasOnline = 24;
                        retorno.First(p => p.Data_Conexao == model.Data_Conexao).Horas_Online_Offline = horasOnline;
                    }
                    else continue;
                }
            }

            return retorno;
        }

        public async Task<List<TempoOnlineOfflineModel>> GetTempoOffline(ISession session, String Login_Radius)
        {
            List<TempoOnlineOfflineModel> retorno = new List<TempoOnlineOfflineModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();


            List<dynamic> lista = new List<dynamic>();
            StringBuilder sql = new StringBuilder();
            /*SELECT connectinfo_start AS DATAHORA_INICIO_CONEXAO, connectinfo_stop AS DATAHORA_FIM_CONEXAO
            FROM radacct 
            WHERE username = 'yago.dias' 
            AND (
            (connectinfo_start > '2022/09/01 00:00:00' AND connectinfo_start < '2022/09/08 11:14:00')
            OR
            (connectinfo_stop > '2022/09/01 00:00:00' AND connectinfo_stop < '2022/09/08 11:14:00')
            )*/
            using (var com = session.Connection.CreateCommand())
            {
                sql.AppendLine("SELECT connectinfo_start AS DATAHORA_INICIO_CONEXAO, connectinfo_stop AS DATAHORA_FIM_CONEXAO ");
                sql.AppendLine("FROM radacct ");
                sql.AppendLine("WHERE username = @USERNAME ");
                sql.AppendLine("AND ((connectinfo_start > @DATA_INICIO AND connectinfo_start < @DATA_FIM) ");
                sql.AppendLine("OR (connectinfo_stop > @DATA_INICIO AND connectinfo_stop < @DATA_FIM)) ");

                parametros.Add("@DATA_INICIO", String.Format("{0}", DateTime.Now.AddDays(-6).ToString("yyyy/MM/dd HH:mm")));
                parametros.Add("@DATA_FIM", String.Format("{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                parametros.Add("@USERNAME", Login_Radius);

                using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new
                        {
                            DataHora_Inicio_Conexao = Convert.ToDateTime(Convert.ToString(reader["DATAHORA_INICIO_CONEXAO"])).ToLocalTime(),
                            DataHora_Termino_Conexao = Convert.ToDateTime(Convert.ToString(reader["DATAHORA_FIM_CONEXAO"])).ToLocalTime()
                        });
                    }
                }
            }

            for (var i = -6; i < 0; i++)
            {
                retorno.Add(new TempoOnlineOfflineModel
                {
                    Data_Conexao = DateTime.Now.AddDays(i).Date,
                    Data_Conexao_String = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"),
                    Horas_Online_Offline = 24
                });
            }
            retorno.Add(new TempoOnlineOfflineModel
            {
                Data_Conexao = DateTime.Now.Date,
                Data_Conexao_String = DateTime.Now.ToString("yyyy-MM-dd"),
                Horas_Online_Offline = 24
            });

            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {

                    var dataHora_Inicio = ((DateTime)item.DataHora_Inicio_Conexao);
                    var dataHora_Fim = ((DateTime)item.DataHora_Termino_Conexao);

                    var model = retorno.FirstOrDefault(p => p.Data_Conexao.Value == dataHora_Inicio.Date || p.Data_Conexao.Value == dataHora_Fim.Date);
                    if (model != null)
                    {
                        var horasOffline = model.Horas_Online_Offline;
                        if (dataHora_Inicio.Date == dataHora_Fim.Date) //Login e Logoff no mesmo dia
                        {
                            horasOffline -= dataHora_Fim.Subtract(dataHora_Inicio).Hours;
                        }
                        else if (dataHora_Inicio.Date == model.Data_Conexao.Value.Date) //Login no dia mas Logoff não
                        {
                            horasOffline -= model.Data_Conexao.Value.AddHours(23).AddMinutes(59).AddSeconds(59).Subtract(dataHora_Inicio).Hours;
                        }
                        else //Logoff no dia mas Login não
                        {
                            horasOffline -= model.Data_Conexao.Value.AddHours(23).AddMinutes(59).AddSeconds(59).Subtract(dataHora_Fim).Hours;
                        }

                        if (horasOffline < 0) horasOffline = 0;
                        retorno.First(p => p.Data_Conexao == model.Data_Conexao).Horas_Online_Offline = horasOffline;
                    }
                    else continue;
                }
            }

            return retorno;
        }

        private String OctetsToKbMbGb(Double octets_velocidade, Medida_Velocidade_ConexaoEnum? unidade_Medida_Velocidade = null)
        {
            String velocidade_convertida = String.Empty;
            try
            {
                Double velocidade_Kb = (octets_velocidade / 1000);
                Double velocidade_Mb = velocidade_Kb / 1000;
                Double velocidade_Gb = velocidade_Mb / 1000;

                if (unidade_Medida_Velocidade.HasValue)
                {
                    switch(unidade_Medida_Velocidade)
                    {
                        case Medida_Velocidade_ConexaoEnum.Kbs:
                            velocidade_convertida = String.Format("{0} Kb", velocidade_Kb.ToString("0.##"));
                            break;
                        case Medida_Velocidade_ConexaoEnum.Mps:
                            velocidade_convertida = String.Format("{0} Mb", velocidade_Mb.ToString("0.##"));
                            break;
                        case Medida_Velocidade_ConexaoEnum.Gbs:
                            velocidade_convertida = String.Format("{0} Gb", velocidade_Gb.ToString("0.##"));
                            break;
                        default:
                            velocidade_convertida = String.Format("{0} Kb", velocidade_Kb.ToString("0.##"));
                            break;
                    }
                }
                else
                {                  
                    if (velocidade_Mb > 1024d) //Manter em Gb 
                    {
                        velocidade_convertida = String.Format("{0} Gb", velocidade_Gb.ToString("0.##"));
                    }
                    else if (velocidade_Kb > 1024d) //Manter em Mb
                    {
                        velocidade_convertida = String.Format("{0} Mb", velocidade_Mb.ToString("0.##"));
                    }
                    else //Manter em Kb
                    {
                        velocidade_convertida = String.Format("{0} Kb", velocidade_Kb.ToString("0.##"));
                    }
                }               

                return velocidade_convertida;
            }
            catch(Exception ex)
            {
                return "-";
            }
        }

        private String FormatTempoConectado(String Hora_Login, String Hora_Logout)
        {
            String retorno = String.Empty;

            try 
            {
                DateTime DataHora_Login = Convert.ToDateTime(Hora_Login);
                DateTime DataHora_Logout = Convert.ToDateTime(Hora_Logout);

                TimeSpan Tempo_Conectado = DataHora_Logout.Subtract(DataHora_Login);

                //retorno = String.Format("{0}d, {1}", Tempo_Conectado.Days, Tempo_Conectado.ToString(@"hh\:mm\:ss"));
                retorno = String.Format("{0}d, {1}", Tempo_Conectado.Days, Tempo_Conectado.ToString(@"hh\:mm\:ss"));

                return retorno;
            }
            catch(Exception ex)
            {
                return "-";
            }
        }

        private string FormatPadraoData(String DataString)
        {
            String retorno = String.Empty;

            try
            {
                retorno = Convert.ToDateTime(DataString).ToString("dd/MM/yyyy HH:mm:ss");

                return retorno;
            }
            catch(Exception ex)
            {
                return DataString;
            }            
        }

        #region DEBUG
        private String UpdateRegistroRadacct()
        { 
            StringBuilder sql = new StringBuilder();            

            sql.AppendLine("UPDATE radacct SET ");
            sql.AppendLine($"acctinputoctets = acctinputoctets * 1.1, ");
            sql.AppendLine($"acctoutputoctets = acctoutputoctets * 1.1 ");
            sql.AppendLine(" WHERE acctstoptime IS NULL AND username = @USERNAME; ");            

            return sql.ToString();
        }

        #endregion
    }
}