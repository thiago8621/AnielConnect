using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.Tasks
{
    public class IntegrarOlaTask : BackgroundTask
    {
        /// <summary>
        /// Integra no banco do Olá Cliente todos os clientes que se tiveram planos habilitados antes da implantação do app no ambiente.
        /// </summary>
        /// <returns></returns>
        public async Task InserirClientesPassivosOla()
        {
            try
            {
                List<Cliente> listaClientes = new List<Cliente>();

                using (var session = HibernateUtil.GetSession())
                using (var sessionNh = HibernateUtil.GetSession())
                {
                    IConfiguracoesGeraisRepository configuracoesRepository = new ConfiguracoesGeraisRepository(sessionNh);
                    ConfiguracoesGeraisBusiness confBusiness = new ConfiguracoesGeraisBusiness(configuracoesRepository);

                    string urlOla = confBusiness.GetUrlOla();

                    if (string.IsNullOrEmpty(urlOla))
                        throw new EAnielException("Url do Olá não está configurada no ambiente, necessário cadastrar nas Configurações da Empresa.");

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOMEFANTASIA, A.CPF, A.TELEFONE_CELULAR, A.EMAIL_PRINCIPAL, A.STATUS, A.INTEGRADO_OLA ");
                        sql.AppendLine("FROM TB_CLIENTE A ");
                        sql.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ");
                        sql.AppendLine("ON (A.ID = B.ID_CLIENTE) ");
                        sql.AppendLine("WHERE (A.INTEGRADO_OLA IS NULL OR A.INTEGRADO_OLA = 0) AND B.STATUS = 2");

                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while(await reader.ReadAsync())
                            {
                                listaClientes.Add(new Cliente { 
                                    Id = Convert.ToInt64(reader["ID"]),
                                    NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                    CpfCnpj = Convert.ToString(reader["CPF"]),
                                    Telefone_Celular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                    Email_Principal = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                    Status = reader["STATUS"] != DBNull.Value ? (Status_Cliente)Enum.Parse(typeof(Status_Cliente), Convert.ToString(reader["STATUS"])) : (Status_Cliente?)null,
                                });
                            }
                        }
                    }

                    if(listaClientes.Count > 0)
                    {
                        log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"Clientes para inserir no Olá: {listaClientes.Count}");
                        IClienteRepository clienteRepository = new ClienteRepository(sessionNh);
                        ClienteBusiness clienteBusiness = new ClienteBusiness(clienteRepository);

                        foreach (var cliente in listaClientes)
                        {
                            try
                            {
                                var configGeral = confBusiness.GetConfiguracoesGerais();

                                clienteBusiness.AtualizarClientesOla(confBusiness.GetUrlOla(), CONSTANTES.SenhaOla, configGeral?.CNPJ, configGeral?.Uuid_Provedor_Ola, cliente);
                                clienteBusiness.AtualizarIntegradoOla(cliente.Id);

                                //Comentado para não inflar o log
                                //log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"Cliente inserido no Olá. IdCliente: {cliente.Id}");
                            }
                            catch(Exception ex)
                            {
                                log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error($"Erro ao inserir cliente no Olá. IdCliente: {cliente.Id} | Mensagem Erro: {ex.Message}", ex);
                            }                            
                        }

                        log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error("Todos os clientes foram inseridos no Olá.");
                    }       
                    else
                    {
                        log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error("Não há clientes para inserir no Olá.");
                    }
                }                
            }
            catch (Exception ex)
            {                
                log4net.LogManager.GetLogger("Aniel.Connect.Tasks").Error(ex.Message, ex);
            }
        }
    }
}