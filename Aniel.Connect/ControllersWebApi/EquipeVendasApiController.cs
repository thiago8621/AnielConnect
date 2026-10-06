using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Aniel.Connect.Models.EquipeVendasCadastroModel;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EquipeVendasApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/EquipeVendasApi/GetAll")]
        public async Task<DataTableResponse<EquipeVendasListaModel>> GetAll(EquipeVendasDatatableRequest request)
        {
            List<EquipeVendasListaModel> retorno = new List<EquipeVendasListaModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID, A.DESCRICAO,A.STATUS ");
                sql.AppendLine(" FROM TB_EQUIPE_VENDA A ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        var status = Convert.ToString(reader["STATUS"]);
                        var descricao = Convert.ToString(reader["DESCRICAO"]);
                    }
                }
                //filtros
                #region

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }
                if (!String.IsNullOrEmpty(request.Status))
                {
                    sql.AppendLine(" WHERE A.STATUS = @STATUS");
                    parametros.Add("@STATUS", request.Status);
                }
                #endregion

                if (request.order != null && request.order.Any())
                {
                    int sortColumnIndex = request.order[0].column;
                    var coluna = request.columns[sortColumnIndex].data;

                    string sortDirection = request.order[0].dir;
                    switch (coluna)
                    {
                        case "id":
                            {
                                if (sortDirection == "asc")
                                    sql.AppendLine("ORDER BY A.ID");
                                else
                                    sql.AppendLine("ORDER BY A.ID DESC");
                                break;
                            }
                        case "status":
                            {
                                if (sortDirection == "asc")
                                    sql.AppendLine("ORDER BY STATUS_ORDENAR");
                                else
                                    sql.AppendLine("ORDER BY STATUS_ORDENAR DESC");
                                break;
                            }
                        case "descricao":
                            {
                                if (sortDirection == "asc")
                                    sql.AppendLine("ORDER BY A.DESCRICAO");
                                else
                                    sql.AppendLine("ORDER BY A.DESCRICAO DESC");
                                break;
                            }
                    }
                }

                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new EquipeVendasListaModel
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Descricao = Convert.ToString(reader["DESCRICAO"]),
                            Status = Convert.ToString(reader["STATUS"]) == "0" ? RsCadastros.INATIVO : RsCadastros.ATIVO,
                        });
                    }
                }
            }

            return new DataTableResponse<EquipeVendasListaModel>
            {
                data = retorno
            };
        }

        [HttpPost()]
        [Route("api/EquipeVendasApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EQUIPE_VENDA A ");

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, 0))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["DESCRICAO"])}"
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }


        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<EquipeVendasListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<EquipeVendasListaModel>> retorno = new GetApiRetorno<IEnumerable<EquipeVendasListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<EquipeVenda>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new EquipeVendasListaModel
                    {
                        Id = (int)p.Id,
                        Descricao = p.Descricao,
                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/EquipeVendaApi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_EQUIPE_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/EquipeVendaApi/FindById")]
        public async Task<GetApiRetorno<EquipeVendasCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<EquipeVendasCadastroModel> retorno = new GetApiRetorno<EquipeVendasCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<EquipeVenda>(session))
            {
                try
                {
                    EquipeVenda equipevendas = await repository.selectFirst(p => p.Id == Id);
                    
                    retorno.Objeto = GetViewObj(session, equipevendas);
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }
        private EquipeVendasCadastroModel GetViewObj(ISession session, EquipeVenda obj)
        {
            using (var repoEquipeVendas = new Conexao<EquipeVenda>(session))
            using (var repoSupervisor = new Conexao<SupervisorEquipeVenda>(session))
            using (var repoFunc = new Conexao<Funcionario>(session))
            {
                var retorno = new EquipeVendasCadastroModel();

                retorno.Id = obj.Id;
                retorno.Descricao = obj.Descricao;
                retorno.Status = obj.Status == Status_Default.Ativo ? true : false;
              
                retorno.SupervisoresLista = repoSupervisor.select(p=> p.EquipeVenda.Id == obj.Id).Result.Select(p => new SupervisoresListaModel
                {
                    Id = p.Id,
                    Id_Supervisor = p.Id,
                    Supervisor = p.Supervisor.Nome
                }).ToList();
                
                return retorno;
            }
        }
        private void BindViewToObj(EquipeVenda objBanco, EquipeVendasCadastroModel view)
        {
            objBanco.DataHoraCriacao = DateTime.Now;
            objBanco.Descricao = view.Descricao;
            objBanco.Status = Status_Default.Ativo;
        }

        private async Task BindSupervisoresToEquipeVenda(ISession session, EquipeVenda equipeVenda, EquipeVendasCadastroModel view)
        {
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            using (var repoSupervEquipeVenda = new Conexao<SupervisorEquipeVenda>(session))
            {
                foreach (SupervisoresListaModel supervisorView in view.SupervisoresLista)
                {

                    SupervisorEquipeVenda objSupervEquipeVenda =
                      await repoSupervEquipeVenda.selectFirst(p => p.EquipeVenda.Id == equipeVenda.Id && p.Supervisor.Id == supervisorView.Id_Supervisor);

                    // Caso não encontre o supervisor já associado àquela EquipeVenda, será necessário incluir o vínculo do supervisor com a equipe
                    if (objSupervEquipeVenda == null)
                    {
                        var supervisor = await repoFuncionario.selectFirst(p => p.Id == supervisorView.Id_Supervisor);

                        var objSuperv = new SupervisorEquipeVenda();
                        objSuperv.EquipeVenda = equipeVenda;
                        objSuperv.Supervisor = supervisor;

                        await repoSupervEquipeVenda.Save(objSuperv);
                    }
                }
            }
        }

        [HttpPost()]
        [Route("api/EquipeVendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<EquipeVendasCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    EquipeVenda modelo = null;
                    using (var repository = new Conexao<EquipeVenda>(session))
                    using (var repoSupervisor = new Conexao<SupervisorEquipeVenda>(session))
                    using (var repoFuncionario = new Conexao<Funcionario>(session))
                    {
                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                        }

                        if (modelo == null)
                        {
                            modelo = new EquipeVenda
                            {
                                Descricao = value.Objeto.Descricao,
                                Status = Status_Default.Ativo,
                            };
                        }
                        else
                        {
                            modelo.Descricao = value.Objeto.Descricao;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        if(modelo.Id == 0)
                        {
                            await repository.Save(modelo);
                        }
                        else
                        {
                            await repository.Update(modelo);
                        }

                        //BindViewToObj(modelo, value.Objeto);

                        //await repository.SaveOrUpdate(modelo);

                        await BindSupervisoresToEquipeVenda(session, modelo, value.Objeto);


                    }

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [Route("api/EquipeVendaApi/Excluir")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirEquipeVenda(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<EquipeVenda>(session))
                    {

                        EquipeVenda modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/EquipeVendasApi/GetComboSupervisorComercial")]
        public async Task<ObjCombo<Int64>> GetComboSupervisorComercial(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FUNCIONARIO D INNER JOIN ASPNETUSERS A ON(A.id_funcionario = D.ID) INNER JOIN tb_perfil_dashboard_connect B ON(B.ID = A.ID_PERFIL_DASH_CONNECT AND B.sistema = 1) ");

                    List<String> condicoes = new List<String>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(D.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT D.ID, D.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(D.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["NOME"])}"
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;
        }
    }
}