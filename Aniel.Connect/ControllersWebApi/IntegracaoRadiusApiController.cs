using Aniel.Connect.Core;
using Aniel.Connect.Models;
using FluentNHibernate.Cfg;
using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Criterion;
using PrimS.Telnet;
using Renci.SshNet;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Aniel.Connect.ControllersWebApi
{
    public class IntegracaoRadiusApiController : BaseApiController
    {

        public Response InserindoClientNas(Model.nhibernate.Radius radius, NAS nas)
        {
            Response response = new Response();
            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        if (VerificarPermissaoDeAcesso(client))
                        {
                            var command = conn.CreateCommand();
                            command.CommandText = $"INSERT INTO nas (nasname,shortname,type,ports,secret,server) values ('{nas.Ip}','{nas.Nome}','{HtmlEnumExtensions.GetEnumDescription<Tecnologia_Nas>(nas.Tecnologia)}', '{nas.Porta}', '{nas.Secret}',null); ";
                            command.ExecuteNonQuery();

                            if (ReiniciarServicoRadius(client))
                            {
                                response.result = true;
                                response.message = "NAS cadastrado com sucesso";
                            }
                            else
                            {
                                response.result = false;
                                response.message = "Equipamento Cadastro no banco com sucesso entretanto houve uma falha na reinicialização do serviço do Radius\n Verifique o Servidor Radius.";
                            }

                        }
                        else
                        {
                            response.result = false;
                            response.message = "Usuario informado não pertence ao Grupo de Usuário (radius). ";
                        }


                    }
                    else
                    {
                        response.result = false;
                        response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                    }


                }
                catch (Exception ex)
                {

                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }

                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                }
            }
            return response;
        }


        public Response<List<NAS>> BuscaTodosEquipamentosNasCadastradoNoRadius(Model.nhibernate.Radius radius)
        {
            Response<List<NAS>> response = new Response<List<NAS>>();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {

                    var command = conn.CreateCommand();
                    command.CommandText = "SELECT id,nasname,shortname,type,ports,secret FROM nas; ";
                    var _reader = command.ExecuteReader();

                    var listaTmp = new List<NAS>();
                    while (_reader.Read())
                    {
                        listaTmp.Add(new NAS
                        {
                            Codigo_Radius = _reader["id"].ToString(),
                            Ip = _reader["nasname"].ToString(),
                            Nome = _reader["shortname"].ToString(),
                            Tecnologia = ConvertTextForEnum(_reader["type"].ToString()),
                            Porta = _reader["ports"].ToString(),
                            Secret = _reader["secret"].ToString()

                        });

                    }
                    response.result = true;
                    response.valor = listaTmp;
                    response.message = "Registros encontrados com sucesso";
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.stacktrace = ex.StackTrace;
                    response.message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }

            }

            return response;

        }


        public Response<NAS> BuscaEquipamentoNasCadastradoNoRadius(Model.nhibernate.Radius radius, NAS nas)
        {
            Response<NAS> response = new Response<NAS>();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    if (String.IsNullOrEmpty(nas.Ip))
                    {
                        response.result = false;
                        response.message = "Endereço de IP do Equipamento não informado.";
                    }
                    else if (String.IsNullOrEmpty(nas.Secret))
                    {
                        response.result = false;
                        response.message = "Endereço de Secret do Equipamento não informado.";
                    }
                    else
                    {

                        var command = conn.CreateCommand();
                        command.CommandText = $"SELECT id,nasname,shortname,type,ports,secret FROM nas WHERE nasname = '{nas.Ip}' and secret = '{nas.Secret}'; ";
                        var _reader = command.ExecuteReader();

                        response.result = true;

                        while (_reader.Read())
                        {
                            response.valor = new NAS
                            {
                                Codigo_Radius = _reader["id"].ToString(),
                                Ip = _reader["nasname"].ToString(),
                                Nome = _reader["shortname"].ToString(),
                                Tecnologia = ConvertTextForEnum(_reader["type"].ToString()),
                                Porta = _reader["ports"].ToString(),
                                Secret = _reader["secret"].ToString()

                            };

                        }

                        _reader.Close();
                        response.message = "Registros encontrados com sucesso";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.stacktrace = ex.StackTrace;
                    response.message = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }

            }
            return response;
        }


        public Response AtualizaEquipamentoNasCadastradoNoRadius(Model.nhibernate.Radius radius, NAS nas)
        {
            Response response = new Response();
            if (String.IsNullOrEmpty(nas.Codigo_Radius))
            {
                response.result = false;
                response.message = "ID do Servidor Radius não informado.";
            }
            else
            {

                using (var conn = CreateSession(radius))
                using (var transaction = conn.BeginTransaction())
                using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
                {
                    try
                    {
                        client.Connect();
                        if (client.IsConnected)
                        {
                            if (VerificarPermissaoDeAcesso(client))
                            {
                                var command = conn.CreateCommand();
                                command.CommandText = $"UPDATE nas SET nasname = {nas.Ip},shortname = {nas.Nome},type = {HtmlEnumExtensions.GetEnumDescription(nas.Tecnologia)},ports = {nas.Porta},secret = {nas.Secret} WHERE id = {nas.Codigo_Radius}";
                                command.ExecuteNonQuery();

                                if (ReiniciarServicoRadius(client))
                                {
                                    response.result = true;
                                    response.message = "NAS Atualizado com sucesso";
                                }
                                else
                                {
                                    response.result = false;
                                    response.message = "Equipamento Atualizado no banco com sucesso entretanto houve uma falha na reinicialização do serviço do Radius\n Verifique o Servidor Radius.";
                                }

                            }
                            else
                            {
                                response.result = false;
                                response.message = "Usuario informado não Pertence ao Grupo de Usuário (radius). ";
                            }


                        }
                        else
                        {
                            response.result = false;
                            response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                        }


                    }
                    catch (Exception ex)
                    {

                        response.result = false;
                        response.exception = ex.Message;
                        response.stacktrace = ex.StackTrace;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            transaction.Commit();
                            conn.Close();
                        }

                        if (client.IsConnected)
                        {
                            client.Disconnect();
                        }
                    }
                }



            }
            return response;
        }



        public Response AtualizaListaDeEquipamentosNasCadastradoNoRadius(Model.nhibernate.Radius radius, List<NAS> ListaNas)
        {

            Response response = new Response();

            var nasEncontrado = ListaNas.FindAll(x => x.Codigo_Radius == null || x.Codigo_Radius == "");
            if (nasEncontrado.Count > 0)
            {
                response.result = false;
                var mensagem = "Não foi informado o ID do Banco Radius nos seguintes Equipamentos.: \n\r";
                foreach (var faltaID in nasEncontrado)
                {
                    mensagem += faltaID.Codigo.ToString();
                }

                response.message = mensagem;

            }
            else
            {
                using (var conn = CreateSession(radius))
                using (var transaction = conn.BeginTransaction())
                using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
                {
                    try
                    {
                        client.Connect();
                        if (client.IsConnected)
                        {
                            if (VerificarPermissaoDeAcesso(client))
                            {
                                var command = conn.CreateCommand();
                                foreach (var nas in ListaNas)
                                {
                                    command.CommandText = $"UPDATE nas SET nasname = {nas.Ip},shortname = {nas.Nome},type = {HtmlEnumExtensions.GetEnumDescription(nas.Tecnologia)},ports = {nas.Porta},secret = {nas.Secret} WHERE id = {nas.Codigo_Radius}";
                                    command.ExecuteNonQuery();
                                }

                                if (ReiniciarServicoRadius(client))
                                {
                                    response.result = true;
                                    response.message = "Equipamentos Atualizados com sucesso";
                                }
                                else
                                {
                                    response.result = false;
                                    response.message = "Equipamentos Atualizado no banco com sucesso entretanto houve uma falha na reinicialização do serviço do Radius\n Verifique o Servidor Radius.";
                                }


                            }
                            else
                            {
                                response.result = false;
                                response.message = "Usuario informado não Pertence ao Grupo de Usuário (radius). ";
                            }


                        }
                        else
                        {
                            response.result = false;
                            response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                        }


                    }
                    catch (Exception ex)
                    {

                        response.result = false;
                        response.exception = ex.Message;
                        response.stacktrace = ex.StackTrace;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {

                            transaction.Commit();
                            conn.Close();
                        }

                        if (client.IsConnected)
                        {
                            client.Disconnect();
                        }
                    }
                }


            }
            return response;

        }


        public Response DeletarRegistroDoEquipamentoNasCadastradoNoRadius(Model.nhibernate.Radius radius, NAS nas)
        {
            Response response = new Response();
            if (String.IsNullOrEmpty(nas.Codigo_Radius))
            {
                response.result = false;
                response.message = "ID do Servidor Radius não informado.";
            }
            else
            {

                using (var conn = CreateSession(radius))
                using (var transaction = conn.BeginTransaction())
                using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
                {
                    try
                    {
                        client.Connect();
                        if (client.IsConnected)
                        {
                            if (VerificarPermissaoDeAcesso(client))
                            {
                                var command = conn.CreateCommand();
                                command.CommandText = $"DELETE FROM nas  WHERE id = {nas.Codigo_Radius}";
                                command.ExecuteNonQuery();

                                if (ReiniciarServicoRadius(client))
                                {
                                    response.result = true;
                                    response.message = "NAS deletado com sucesso";
                                }
                                else
                                {
                                    response.result = false;
                                    response.message = "Equipamento deletado no banco com sucesso entretanto houve uma falha na reinicialização do serviço do Radius\n Verifique o Servidor Radius.";
                                }

                            }
                            else
                            {
                                response.result = false;
                                response.message = "Usuario informado não Pertence ao Grupo de Usuário (radius). ";
                            }


                        }
                        else
                        {
                            response.result = false;
                            response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                        }


                    }
                    catch (Exception ex)
                    {

                        response.result = false;
                        response.exception = ex.Message;
                        response.stacktrace = ex.StackTrace;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            transaction.Commit();
                            conn.Close();
                        }

                        if (client.IsConnected)
                        {
                            client.Disconnect();
                        }
                    }
                }


            }
            return response;

        }


        public Response CriacaoUsuarioRadiusSemGrupo(Model.nhibernate.Radius radius, Cliente_PlanoProduto cliente_PlanoProduto, Pool_Ip pool, NAS nas)
        {
            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {

                    var command = conn.CreateCommand();
                    if (cliente_PlanoProduto.Status == Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado)
                    {
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values('{cliente_PlanoProduto.Login_Radius}', 'Cleartext-Password', ':=','{cliente_PlanoProduto.Senha_Radius}', 'S' );");
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values ('{cliente_PlanoProduto.Login_Radius}', 'Pool-Name', ':=', '{pool.Descricao}', 'S' );");
                        sql.Append($"INSERT INTO  radcheck (username,attribute,op,value, ativacao_aniel) VALUES ('{cliente_PlanoProduto.Login_Radius}', 'Simultaneous-Use', ':=', '1', 'S');");
                    }
                    else
                    {
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values('{cliente_PlanoProduto.Login_Radius}', 'Cleartext-Password', ':=','{cliente_PlanoProduto.Senha_Radius}', 'N' );");
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values ('{cliente_PlanoProduto.Login_Radius}', 'Pool-Name', ':=', '{pool.Descricao}', 'N' );");
                        sql.Append($"INSERT INTO  radcheck (username,attribute,op,value, ativacao_aniel) VALUES ('{cliente_PlanoProduto.Login_Radius}', 'Simultaneous-Use', ':=', '1', 'N');");
                    }


                    sql.Append(InserirVelocidadeDownloadUploadRadius(nas.Tecnologia, cliente_PlanoProduto));


                    command.CommandText = sql.ToString();
                    command.ExecuteNonQuery();

                    response.result = true;
                    response.message = "Usuário Cadastrado com sucesso.";

                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }



            }




            return response;

        }

        public bool ExisteUsuarioCadastradoNoRadius(Model.nhibernate.Radius radius, string login_Radius)
        {
            var quantidadeUsuario = 0;
            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = conn.CreateCommand();
                    command.CommandText = $"SELECT COUNT(*) AS QUANT_USUARIO FROM radcheck WHERE username = '{login_Radius}' AND ATTRIBUTE = 'Cleartext-Password'";
                    var _reader = command.ExecuteReader();
                    while (_reader.Read())
                    {
                        quantidadeUsuario = Convert.ToInt32(_reader["QUANT_USUARIO"].ToString());
                    }


                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return quantidadeUsuario > 0;
        }


        public Response EditarUsuarioRadiusSemGrupo(Model.nhibernate.Radius radius, Cliente_PlanoProduto cliente_PlanoProduto, Pool_Ip pool, NAS nas)
        {
            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        if (VerificarPermissaoDeAcesso(client))
                        {

                            var command = conn.CreateCommand();
                            if (cliente_PlanoProduto.Status == Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado)
                            {
                                sql.Append($"UPDATE radcheck SET value = '{cliente_PlanoProduto.Senha_Radius}', ativacao_aniel = 'S' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Cleartext-Password'; ");
                                sql.Append($"UPDATE radcheck SET value ='{pool.Descricao}', ativacao_aniel = 'S' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Pool-Name' ;");

                            }
                            else
                            {
                                sql.Append($"UPDATE radcheck SET value = '{cliente_PlanoProduto.Senha_Radius}', ativacao_aniel = 'N' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Cleartext-Password'; ");
                                sql.Append($"UPDATE radcheck SET value ='{pool.Descricao}', ativacao_aniel = 'N' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Pool-Name' ;");
                            }

                            sql.Append($"DELETE FROM radreply WHERE username = '{cliente_PlanoProduto.Login_Radius}';");



                            sql.Append(InserirVelocidadeDownloadUploadRadius(nas.Tecnologia, cliente_PlanoProduto));
                            command.CommandText = sql.ToString();
                            command.ExecuteNonQuery();


                            if (DesconectarUsuarioDoNas(client, nas, cliente_PlanoProduto))
                            {
                                response.result = true;
                                response.message = "Usuário Atualizado com sucesso";
                            }
                            else
                            {
                                response.result = false;
                                response.message = "Houve um problema ao desconectar o Usuário, favor verifique o Servidor Radius. ";
                            }
                        }
                        else
                        {
                            response.result = false;
                            response.message = "Usuário informado não pertence ao Grupo de Usuário (radius). ";
                        }
                    }
                    else
                    {
                        response.result = false;
                        response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                    }



                    response.result = true;
                    response.message = "Usuário Atualizado com sucesso.";

                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }

                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                }



            }




            return response;
        }

        private bool DesconectarUsuarioDoNas(SshClient client, NAS nas, Cliente_PlanoProduto cliente_PlanoProduto)
        {

            var retorno = client.RunCommand($"echo User-Name = {cliente_PlanoProduto.Login_Radius} | radclient -x {nas.Ip}:{nas.Porta} disconnect '{nas.Secret}' ");
            Thread.Sleep(10000);
            if (String.IsNullOrEmpty(retorno.Error))
            {
                return false;
            }
            else
            {
                return true;
            }


        }


        public Response DeletarTodosRegistroDoUsuarioNoRadius(Model.nhibernate.Radius radius, Cliente_PlanoProduto cliente_PlanoProduto, Pool_Ip pool, NAS nas)
        {
            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        var command = conn.CreateCommand();

                        sql.Append($"DELETE FROM radcheck  WHERE username = '{cliente_PlanoProduto.Login_Radius}'; ");
                        sql.Append($"DELETE FROM radreply WHERE username = '{cliente_PlanoProduto.Login_Radius}';");
                        sql.Append($"DELETE FROM radusergroup WHERE username= '{cliente_PlanoProduto.Login_Radius}'");



                        if (DesconectarUsuarioDoNas(client, nas, cliente_PlanoProduto))
                        {
                            response.result = true;
                            response.message = "Usuário deletado com sucesso.";
                        }
                        else
                        {
                            response.result = false;
                            response.message = "Houve um problema ao desconectar o Usuário, favor verifique o Servidor Radius. ";
                        }

                        command.CommandText = sql.ToString();
                        command.ExecuteNonQuery();


                    }
                    else
                    {
                        response.result = false;
                        response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }

                    if (client.IsConnected)
                    {
                        client.Disconnect();
                    }
                }



            }




            return response;
        }


        public Response<List<String>> InformacaoDosUsuariosCadastradosNoRadius(Model.nhibernate.Radius radius, Cliente_PlanoProduto cliente_PlanoProduto)
        {
            Response<List<String>> response = new Response<List<String>>();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = conn.CreateCommand();

                    sql.Append($"SELECT A.username as USUARIO, A.attribute AS ATRIBUTO , A.value AS VALOR FROM radcheck AS A ");
                    sql.Append($"WHERE A.username = '{cliente_PlanoProduto.Login_Radius}' ");
                    sql.Append("UNION");
                    sql.Append("SELECT B.username as USUARIO, B.attribute AS ATRIBUTO , B.value AS VALOR FROM radreply AS B ");
                    sql.Append($"WHERE B.username = '{cliente_PlanoProduto.Login_Radius}' ");
                    sql.Append("UNION");
                    sql.Append("SELECT  C.groupname as USUARIO,C.attribute AS ATRIBUTO, C.value AS VALOR FROM radgroupreply as C");
                    sql.Append("LEFT JOIN radusergroup as D ON C.groupname = D.groupname");
                    sql.Append($"WHERE b.username = '{cliente_PlanoProduto.Login_Radius}';");



                    command.CommandText = sql.ToString();
                    var _reader = command.ExecuteReader();
                    var listTemp = new List<String>();
                    while (_reader.Read())
                    {
                        listTemp.Add($"{_reader["USUARIO"].ToString()} | {_reader["ATRIBUTO"].ToString()} = {_reader["VALOR"].ToString()}");
                    }


                    response.result = true;
                    response.valor = listTemp;

                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }

        public Response CriacaoDeGruposNoRadius(Model.nhibernate.Radius radius, NAS nas, Cliente_PlanoProduto cliente_PlanoProduto, Pool_Ip pool)
        {
            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = conn.CreateCommand();
                    if (cliente_PlanoProduto.Status == Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado)
                    {
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values('{cliente_PlanoProduto.Login_Radius}', 'Cleartext-Password', ':=','{cliente_PlanoProduto.Senha_Radius}', 'S' );");
                    }
                    else
                    {
                        sql.Append($"INSERT INTO radcheck (username,attribute,op,value, ativacao_aniel) values('{cliente_PlanoProduto.Login_Radius}', 'Cleartext-Password', ':=','{cliente_PlanoProduto.Senha_Radius}', 'N' );");
                    }
                    sql.Append($"INSERT INTO radusergroup(username, groupname, priority) values('{cliente_PlanoProduto.Login_Radius}', '{cliente_PlanoProduto.Plano.Descricao}', '1');");
                    sql.Append($"INSERT INTO radgroupcheck(groupname, attribute, op, value) values('{cliente_PlanoProduto.Plano.Descricao}', 'Pool-Name', ':=', '{pool.Descricao}');");
                    sql.Append($"INSERT INTO radgroupcheck (groupname, attribute, op, value) VALUES ('{cliente_PlanoProduto.Plano.Descricao}', 'Simultaneous-Use', ':=', '1');");


                    sql.Append(InserirVelocidadeDownloadUploadRadiusGrupo(nas.Tecnologia, cliente_PlanoProduto));

                    command.CommandText = sql.ToString();
                    command.ExecuteNonQuery();



                    response.result = true;
                    response.message = "Grupo Cadastrado e Usuario associado com sucesso.";


                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }



        public Response AtualizacaoDoGruposNoRadius(Model.nhibernate.Radius radius, NAS nas, Cliente_PlanoProduto cliente_PlanoProduto, Pool_Ip pool)
        {
            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        if (VerificarPermissaoDeAcesso(client))
                        {
                            var command = conn.CreateCommand();
                            if (cliente_PlanoProduto.Status == Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado)
                            {
                                sql.Append($"UPDATE radcheck SET value='{cliente_PlanoProduto.Senha_Radius}',ativacao_aniel='S' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Cleartext-Password' ;");
                            }
                            else
                            {
                                sql.Append($"UPDATE radcheck SET value='{cliente_PlanoProduto.Senha_Radius}',ativacao_aniel='N' WHERE username = '{cliente_PlanoProduto.Login_Radius}' AND attribute = 'Cleartext-Password' ;");
                            }

                            sql.Append($"UPDATE radusergroup SET groupname ={cliente_PlanoProduto.Plano.Descricao} WHERE username ='{cliente_PlanoProduto.Login_Radius}' ;");
                            sql.Append($"UPDATE radgroupcheck SET value ={pool.Descricao} WHERE groupname = '{cliente_PlanoProduto.Plano.Descricao}' AND attribute = 'Pool-Name' ;");


                            if (DesconectarUsuarioDoNas(client, nas, cliente_PlanoProduto))
                            {
                                response.result = true;
                                response.message = "Grupo Atualizado e Usuário associado com sucesso.";
                            }
                            else
                            {
                                response.result = false;
                                response.message = "Houve um problema ao desconectar o Usuário, favor verifique o Servidor Radius. ";
                            }


                            command.CommandText = sql.ToString();
                            command.ExecuteNonQuery();


                        }
                        else
                        {
                            response.result = false;
                            response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                        }
                    }
                    else
                    {
                        response.result = false;
                        response.message = "Usuário informado não pertence ao Grupo de Usuário (radius). ";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }




        public Response DeletarGrupoDoRadius(Model.nhibernate.Radius radius, Cliente_PlanoProduto cliente_PlanoProduto)
        {


            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            //using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                try
                {
                    var command = conn.CreateCommand();


                    sql.Append($"DELETE * FROM radgroupcheck WHERE groupname = {cliente_PlanoProduto.Plano.Descricao}; ");
                    sql.Append($"DELETE * FROM radusergroup WHERE username = {cliente_PlanoProduto.Login_Radius};");

                    command.CommandText = sql.ToString();



                    command.CommandText = sql.ToString();
                    command.ExecuteNonQuery();



                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;

        }


        public Response DesconectarUsuario(Cliente_PlanoProduto cliente_PlanoProduto, NAS nas)
        {
            Response response = new Response();
            Model.nhibernate.Radius radius = cliente_PlanoProduto.Plano.Radius;
            using (SshClient client = new SshClient(radius.Ip, Convert.ToInt32(radius.Porta_SSH), radius.Usuario, radius.Senha))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    if (VerificarPermissaoDeAcesso(client))
                    {
                        if (DesconectarUsuarioDoNas(client, nas, cliente_PlanoProduto))
                        {
                            response.result = true;
                            response.message = "Usuário Desconectado com sucesso";
                        }
                        else
                        {
                            response.result = false;
                            response.message = "Houve um problema ao desconectar o Usuário, favor verifique o Servidor Radius. ";
                        }

                    }
                    else
                    {
                        response.result = false;
                        response.message = "Usuário informado não pertence ao Grupo de Usuário (radius). ";
                    }
                }
                else
                {
                    response.result = false;
                    response.message = "Problemas com a conexão, tente novamente mais tarde. ";
                }
            }



            return response;
        }

        public bool ReiniciarServicoRadius(SshClient client)
        {
            client.RunCommand("sudo systemctl stop freeradius; sudo systemctl start freeradius");
            var resultado = client.RunCommand("sudo systemctl status freeradius | grep Active:");
            return resultado.Result.Contains("running");

        }

        public Response criarIPPool(Model.nhibernate.Radius radius, Pool_Ip pool)
        {
            var totalizado = 0;

            Response response = new Response();
            StringBuilder sql = new StringBuilder();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = conn.CreateCommand();
                    if (!String.IsNullOrEmpty(pool.Ip_Inicio_Range) && !String.IsNullOrEmpty(pool.Ip_Fim_Range))
                    {
                        var listaIpsGerados = new List<String>();

                        var listaOctetosIP1 = new List<Int32>();
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[0]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[1]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[2]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[3]));

                        var listaOctetosIP2 = new List<Int32>();
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[0]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[1]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[2]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[3]));

                        //if (pool.Ip_Mascara.Equals("255.0.0.0") && (listaOctetosIP1[0] == listaOctetosIP2[0]))
                        //{
                        listaIpsGerados = gerarIpsMascara8(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else if (pool.Ip_Mascara.Equals("255.255.0.0") && (listaOctetosIP1[0] == listaOctetosIP2[0] && listaOctetosIP1[1] == listaOctetosIP2[1]))
                        //{
                        //    listaIpsGerados = gerarIpsMascara16(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else if (pool.Ip_Mascara.Equals("255.255.255.0") && (listaOctetosIP1[0] == listaOctetosIP2[0] && listaOctetosIP1[1] == listaOctetosIP2[1] && listaOctetosIP1[2] == listaOctetosIP2[2]))
                        //{
                        //    listaIpsGerados = gerarIpsMascara24(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else
                        //{
                        //    response.result = false;
                        //    response.message = "Faixa de endereços informada é inválida.";
                        //}

                        if (listaIpsGerados.Count != 0)
                        {
                            totalizado = listaIpsGerados.Count;
                            foreach (String ip in listaIpsGerados)
                            {
                                sql.Append($"INSERT INTO radippool (`id`, pool_name, framedipaddress, nasipaddress, calledstationid, callingstationid, expiry_time, username, `pool_key`) VALUES (NULL, '{pool.Descricao}', '{ip}', '', '', '', NULL, '', '');");
                            }
                            //response.result = true;
                            //response.message = $"Pool de IPs criado com sucesso! A faixa de IPs possui {totalizado} endereços.";
                        }

                        command.CommandText = sql.ToString();
                        command.ExecuteNonQuery();

                        response.result = true;
                        response.message = $"Pool de IPs criado com sucesso! A faixa de IPs possui {totalizado} endereços.";
                    }
                    else
                    {
                        response.result = false;
                        response.message = "Não é possível criar um pool de IPs com um IP inicial ou final vazio!";

                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }

        public Response deletarIpPool(Model.nhibernate.Radius radius, Pool_Ip pool)
        {

            Response response = new Response();

            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    if (!String.IsNullOrEmpty(pool.Descricao))
                    {
                        var command = conn.CreateCommand();
                        command.CommandText = $"DELETE FROM radippool WHERE pool_name = '{pool.Descricao}'";
                        command.ExecuteNonQuery();
                        response.result = true;
                        response.message = $"Pool de IPs {pool.Descricao} deletado com sucesso!";
                    }
                    else
                    {
                        response.result = false;
                        response.message = $"Nome do Pool de IPs não pode ser vazio ou em branco.";
                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }

        public Response atualizarIpPool(Model.nhibernate.Radius radius, Pool_Ip pool)
        {

            Response response = new Response();
            StringBuilder sql = new StringBuilder();
            var totalizado = 0;
            using (var conn = CreateSession(radius))
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    var command = conn.CreateCommand();
                    if (!String.IsNullOrEmpty(pool.Ip_Inicio_Range) && !String.IsNullOrEmpty(pool.Ip_Fim_Range))
                    {
                        var listaIpsGerados = new List<String>();

                        var listaOctetosIP1 = new List<Int32>();
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[0]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[1]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[2]));
                        listaOctetosIP1.Add(Convert.ToInt32(pool.Ip_Inicio_Range.Split('.')[3]));

                        var listaOctetosIP2 = new List<Int32>();
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[0]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[1]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[2]));
                        listaOctetosIP2.Add(Convert.ToInt32(pool.Ip_Fim_Range.Split('.')[3]));

                        //if (pool.Ip_Mascara.Equals("255.0.0.0") && (listaOctetosIP1[0] == listaOctetosIP2[0]))
                        //{
                        listaIpsGerados = gerarIpsMascara8(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else if (pool.Ip_Mascara.Equals("255.255.0.0") && (listaOctetosIP1[0] == listaOctetosIP2[0] && listaOctetosIP1[1] == listaOctetosIP2[1]))
                        //{
                        //    listaIpsGerados = gerarIpsMascara16(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else if (pool.Ip_Mascara.Equals("255.255.255.0") && (listaOctetosIP1[0] == listaOctetosIP2[0] && listaOctetosIP1[1] == listaOctetosIP2[1] && listaOctetosIP1[2] == listaOctetosIP2[2]))
                        //{
                        //    listaIpsGerados = gerarIpsMascara24(listaOctetosIP1, listaOctetosIP2);
                        //}
                        //else
                        //{
                        //    response.result = false;
                        //    response.message = "Faixa de endereços informada é inválida.";
                        //}
                        if (listaIpsGerados.Count != 0)
                        {
                            sql.Append($"DELETE FROM radippool WHERE pool_name = '{pool.Descricao}';");
                            totalizado = listaIpsGerados.Count;
                            foreach (String ip in listaIpsGerados)
                            {
                                sql.Append($"INSERT INTO radippool (`id`, pool_name, framedipaddress, nasipaddress, calledstationid, callingstationid, expiry_time, username, `pool_key`) VALUES (NULL, '{pool.Descricao}', '{ip}', '', '', '', NULL, '', '');");
                            }
                            command.CommandText = sql.ToString();
                            command.ExecuteNonQuery();
                        }
                        response.result = true;
                        response.message = "Pool de IPs atualizada com sucesso!";

                    }
                    else
                    {
                        response.result = false;
                        response.message = "Não é possível atualizar o pool de IPs com endereço IP inicial ou final da faixa vazio!";

                    }
                }
                catch (Exception ex)
                {
                    response.result = false;
                    response.exception = ex.Message;
                    response.stacktrace = ex.StackTrace;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        transaction.Commit();
                        conn.Close();
                    }
                }
            }
            return response;
        }


        #region MetodosPrivados

        private String InserirVelocidadeDownloadUploadRadiusGrupo(Tecnologia_Nas tecnologia, Cliente_PlanoProduto cliente_PlanoProduto)
        {
            var plano = cliente_PlanoProduto.Plano;
            if (tecnologia == Tecnologia_Nas.Mikrotik)
            {

                return $"INSERT INTO radgroupreply(groupname, attribute, op, value) values('{plano.Descricao}', 'Mikrotik-Rate-Limit', ':=', '{plano.VelocidadeUpload}M/{plano.VelocidadeDownload}M');";
            }
            else
            {
                return "";
            }
        }

        private string InserirVelocidadeDownloadUploadRadius(Tecnologia_Nas tecnologia, Cliente_PlanoProduto cliente_PlanoProduto)
        {
            var plano = cliente_PlanoProduto.Plano;
            if (tecnologia == Tecnologia_Nas.Mikrotik)
            {

                return $"INSERT INTO radreply (username,attribute,op,value) values ('{cliente_PlanoProduto.Login_Radius}','Mikro-Rate-Limit',':=','{plano.VelocidadeUpload}M/{plano.VelocidadeDownload}M');";
            }
            else
            {
                return "";
            }
        }

        private Tecnologia_Nas ConvertTextForEnum(string tecnologia)
        {
            if (tecnologia.Equals(HtmlEnumExtensions.GetEnumDescription<Tecnologia_Nas>(Tecnologia_Nas.Mikrotik)))
            {
                return Tecnologia_Nas.Mikrotik;

            }
            else
            {
                return Tecnologia_Nas.other;
            }


        }



        private bool VerificarPermissaoDeAcesso(SshClient client)
        {
            var resultado = client.RunCommand("id -g -n");
            return resultado.Result.Replace("\n", "").Equals("radius");
        }



        private MySqlConnection CreateSession(Model.nhibernate.Radius radius)
        {
            var connString = $"Server={radius.Ip};Database={radius.Banco};Uid={radius.Usuario_Banco};Pwd={radius.Senha_Banco};Port={radius.Porta_Banco}";
            var connection = new MySqlConnection(connString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        private List<String> gerarIpsMascara8(List<Int32> octetosIP1, List<Int32> octetosIP2)
        {
            var listaIp = new List<String>();

            var QuantidadeVezesParteI = 0;
            if ((octetosIP2[1] >= octetosIP1[1]) && (octetosIP2[2] >= octetosIP1[2]))
            {
                QuantidadeVezesParteI = (octetosIP2[1] - octetosIP1[1]) + (octetosIP2[2] - octetosIP1[2]);
            }
            else if ((octetosIP2[1] > octetosIP1[1]) && (octetosIP2[2] < octetosIP1[2]))
            {
                QuantidadeVezesParteI = (octetosIP2[1] - octetosIP1[1]) + (255 + octetosIP2[2] - octetosIP1[2]);
            }
            var PrimeiroOcteto = octetosIP1[0];
            var SegundoOcteto = octetosIP1[1];
            var TerceiroOcteto = octetosIP1[2];
            var QuartoOcteto = octetosIP1[3];

            while (QuantidadeVezesParteI != 0)
            {
                //Console.WriteLine("IPs formatos .:" + PrimeiroOcteto + "." + SegundoOcteto + "." + TerceiroOcteto + "." + QuartoOcteto);
                listaIp.Add($"{PrimeiroOcteto}.{SegundoOcteto}.{TerceiroOcteto}.{QuartoOcteto}");
                QuartoOcteto++;
                if (SegundoOcteto < 255)
                {
                    if (QuartoOcteto > 255)
                    {
                        //Console.WriteLine("IPs formatos .:"+PrimeiroOcteto+"."+SegundoOcteto+"."+(TerceiroOcteto)+"."+(QuartoOcteto));
                        TerceiroOcteto += 1;
                        QuartoOcteto = 1;
                        QuantidadeVezesParteI--;
                    }
                    if (TerceiroOcteto > 255)
                    {
                        //Console.WriteLine("IPs formatos .:"+PrimeiroOcteto+"."+(SegundoOcteto+=1)+"."+(TerceiroOcteto+=0)+"."+QuartoOcteto);
                        TerceiroOcteto = 0;
                        SegundoOcteto += 1;
                    }
                    if (QuantidadeVezesParteI == 0)
                    {
                        for (var i = 1; i <= octetosIP2[3]; i++)
                        {
                            //Console.WriteLine("IPs formatos .:" + PrimeiroOcteto + "." + SegundoOcteto + "." + TerceiroOcteto + "." + i);
                            listaIp.Add($"{PrimeiroOcteto}.{SegundoOcteto}.{TerceiroOcteto}.{i}");

                        }
                    }
                }
            }

            return listaIp;
        }
        private List<String> gerarIpsMascara16(List<Int32> octetosIP1, List<Int32> octetosIP2)
        {
            var listaIp = new List<String>();

            var QuantidadeVezesParteI = 0;
            if (octetosIP2[2] >= octetosIP1[2])
            {
                QuantidadeVezesParteI = (octetosIP2[2] - octetosIP1[2]);
            }
            else if (octetosIP2[2] < octetosIP1[2])
            {
                QuantidadeVezesParteI = (255 + octetosIP2[2] - octetosIP1[2]);
            }
            var PrimeiroOcteto = octetosIP1[0];
            var SegundoOcteto = octetosIP1[1];
            var TerceiroOcteto = octetosIP1[2];
            var QuartoOcteto = octetosIP1[3];

            while (QuantidadeVezesParteI != 0)
            {
                //Console.WriteLine("IPs formatos .:" + PrimeiroOcteto + "." + SegundoOcteto + "." + TerceiroOcteto + "." + QuartoOcteto);
                listaIp.Add($"{PrimeiroOcteto}.{SegundoOcteto}.{TerceiroOcteto}.{QuartoOcteto}");
                QuartoOcteto++;
                if (SegundoOcteto < 255)
                {
                    if (QuartoOcteto > 255)
                    {
                        //Console.WriteLine("IPs formatos .:"+PrimeiroOcteto+"."+SegundoOcteto+"."+(TerceiroOcteto)+"."+(QuartoOcteto));
                        TerceiroOcteto += 1;
                        QuartoOcteto = 1;
                        QuantidadeVezesParteI--;
                    }
                    if (QuantidadeVezesParteI == 0)
                    {
                        for (var i = 1; i <= octetosIP2[3]; i++)
                        {
                            //Console.WriteLine("IPs formatos .:" + PrimeiroOcteto + "." + SegundoOcteto + "." + TerceiroOcteto + "." + i);
                            listaIp.Add($"{PrimeiroOcteto}.{SegundoOcteto}.{TerceiroOcteto}.{i}");

                        }
                    }
                }
            }

            return listaIp;
        }
        private List<String> gerarIpsMascara24(List<Int32> octetosIP1, List<Int32> octetosIP2)
        {
            var listaIp = new List<String>();
            var PrimeiroOcteto = octetosIP1[0];
            var SegundoOcteto = octetosIP1[1];
            var TerceiroOcteto = octetosIP1[2];
            var QuartoOcteto = octetosIP1[3];
            for (var i = octetosIP1[3]; i <= octetosIP2[3]; i++)
            {
                listaIp.Add($"{PrimeiroOcteto}.{SegundoOcteto}.{TerceiroOcteto}.{i}");
                //Console.WriteLine("IPs formatos .:" + primeiroOctetoIP1 + "." + SegundoOctetoIP1 + "." + TerceiroOctetoIP2 + "." + i);
            }
            return listaIp;
        }


        #endregion


    }
}