using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using Aniel.Connect.Repository.Workflow;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Workflow_vendasApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/workflow_vendasapi/GetAll")]
        public async Task<DataTableResponse<Workflow_VendasListModel>> GetAll(Workflow_VendasDataTableResquest request)
        {
            List<Workflow_VendasListModel> retorno = new List<Workflow_VendasListModel>();
            IOrderedEnumerable<Workflow_VendasListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                sqlFrom.Append("FROM TB_WORKFLOW_VENDA A");
                sqlFrom.AppendLine(" WHERE A.ID > 0 ");

                if (!String.IsNullOrEmpty(request.Status))
                {
                    sqlFrom.Append(" AND STATUS = @STATUS ");
                    parametros.Add("@STATUS", Convert.ToInt16(request.Status));
                }
                        
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO ");
                    sql.Append(sqlFrom);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS, A.DATAHORACRIACAO ");
                    sql.Append(sqlFrom);
                    


                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {

                            case "descricao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.DESCRICAO");
                                    else
                                        sql.AppendLine(" ORDER BY A.DESCRICAO DESC");
                                    break;
                                }
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.STATUS");
                                    else
                                        sql.AppendLine(" ORDER BY A.STATUS");
                                    break;
                                }
                            case "id":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.ID");
                                    else
                                        sql.AppendLine(" ORDER BY A.ID DESC");
                                    break;
                                }

                        }
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Workflow_VendasListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = Convert.ToString(reader["STATUS"]) == "0" ? RsCadastros.INATIVO : RsCadastros.ATIVO,
                                //Status_Desc = HtmlEnumExtensions.GetEnumDescription<Status_Default>(reader[""]),


                            });
                        }
                    }

                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Workflow_VendasListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        private Workflow_VendasCadastroModel GetViewObj(Workflow_Vendas obj, ISession session, ISession sessionT5)
        {
            using (var repoStatusVenda = new Conexao<Atividade_StatusVenda>(session))
            using (var repoHoraAtendimento = new Conexao<Horario_Connect>(sessionT5))
            {
                var retorno = new Workflow_VendasCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,
                    Status = obj.Status == Status_Default.Ativo,                  

                    TipoPessoa = obj.TipoPessoa,
                    Categoria = obj.Categoria,
                    Captacao = obj.Captacao,
                    Uf = obj.Uf,
                    Cidade = obj.Cidade,
                    Prazo = obj.Prazo,
                    Tipodias = obj.Tipodias,

                    Atividades = obj.Atividades.Select(p =>
                    {

                        //var statusVenda = repoStatusVenda.selectFirst(param => param.Id == Convert.ToInt64(p.Ativ_StatusVenda)).Result;

                        return new Workflow_VendasAtividadeModel
                        {
                            Id = p.Id,
                            Ativ_AgendaTarefa = p.Ativ_AgendaTarefa,

                            Ativ_Status = p.Ativ_Status == Status_Default.Ativo,
                            Ativ_Status_Desc = HtmlEnumExtensions.GetEnumDescription<Status_Default>(p.Ativ_Status),

                            Ativ_Decisao = p.Ativ_Decisao,
                            Ativ_Decisao_Desc = HtmlEnumExtensions.GetEnumDescription<Atividade_Decisoria>((Atividade_Decisoria)p.Ativ_Decisao),

                            Ativ_Descricao = p.Ativ_Descricao,
                            Ativ_NAtivAprovada = p.Ativ_NAtivAprovada,
                            Ativ_NAtivReprovada = p.Ativ_NAtivReprovada,
                            Ativ_Prazo = IntService.ToTime99StringWorkflow(p.Ativ_Prazo_Minutos),
                            Ativ_StatusVenda = Convert.ToString(p.Ativ_StatusVenda?.Id),

                            Ativ_StatusVenda_Desc = p?.Ativ_StatusVenda?.Descricao,
                            StatusPadrao = p?.Ativ_StatusVenda?.StatusPadrao != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(p.Ativ_StatusVenda.StatusPadrao.Value) : null,
                            Id_Status_Padrao = p?.Ativ_StatusVenda?.StatusPadrao,
                            Horario_Atendimento = p.Horario_Atendimento,
                            Horario_Atendimento_Desc = repoHoraAtendimento.selectFirst(param => param.Id == Convert.ToInt64(p.Horario_Atendimento)).Result?.Descricao ?? String.Empty,

                            Ativ_Tipodias = p.Ativ_Tipodias,
                            Ativ_Tipodias_Desc = HtmlEnumExtensions.GetEnumDescription<Tipo_HORA>(p.Ativ_Tipodias),

                            Definir_Responsavel = p.Definir_Responsavel,

                            Setor_Departamento = p.SetorResponsavel?.Id,
                            Setor_Departamento_Desc = p?.SetorResponsavel?.Descricao != null ? p?.SetorResponsavel?.Descricao : String.Empty,
                            Responsavel = p.Responsavel?.Id ,
                            Responsavel_Desc = p?.Responsavel?.Nome != null ? p?.Responsavel?.Nome : String.Empty

                        };
                    }).ToList(),

                };

                return retorno;
            }
        }

        private void BindViewToObj(ISession session, ISession sessionT5, Workflow_Vendas objBanco, Workflow_VendasCadastroModel view)
        {
            objBanco.Id = (long)view.Id;
            objBanco.Descricao = view.Descricao;
            objBanco.TipoPessoa = view.TipoPessoa;
            objBanco.Categoria = view.Categoria;
            objBanco.Captacao = view.Captacao;
            objBanco.Uf = view.Uf;
            objBanco.Cidade = view.Cidade;
            view.Prazo = 0;
            //objBanco.Tipodias = view.Tipodias.Value;


            //Buscar o Atividade Status Venda através do id Ativ_StatusVenda em WorkFlow atividades
            using (var repo = new Conexao<Atividade_StatusVenda>(session))
            using (var repoDepartamento = new Conexao<Departamento>(session))            
            using (var repoUsuario = new Conexao<IdentityUser>(session))            
            {
                objBanco.Atividades = view.Atividades.Select(p => new Workflow_VendasAtividade
                {
                    Id = p.Id,
                    Ativ_AgendaTarefa = p.Ativ_AgendaTarefa,
                    Ativ_Status = p.Ativ_Status ? Status_Default.Ativo : Status_Default.Inativo,
                    Ativ_Decisao = p.Ativ_Decisao,
                    Ativ_Descricao = p.Ativ_Descricao,
                    Ativ_NAtivAprovada = p.Ativ_NAtivAprovada,
                    Ativ_NAtivReprovada = p.Ativ_NAtivReprovada,
                    Ativ_Prazo_Minutos = IntService.GetMinutos(p.Ativ_Prazo),
                    Ativ_StatusVenda = repo.selectFirst(x => x.Id == Convert.ToInt64(p.Ativ_StatusVenda)).Result,
                    Horario_Atendimento = p.Horario_Atendimento,
                    Ativ_Tipodias = p.Ativ_Tipodias,
                    Definir_Responsavel = p.Definir_Responsavel,
                    SetorResponsavel =  repoDepartamento.selectFirst(x => x.Id == p.Setor_Departamento).Result,
                    Responsavel = repoUsuario.selectFirst(x => x.Id == p.Responsavel).Result,
                    WorkFlow = objBanco,

                }).ToList();
            }

            foreach (var Lista_Atividades in objBanco.Atividades)
            {
                view.Prazo = (int)(view.Prazo + Lista_Atividades.Ativ_Prazo_Minutos);
            }
            objBanco.Prazo = view.Prazo;


        }

        [HttpGet()]
        [Route("api/workflow_vendasapi/FindById")]
        public async Task<GetApiRetorno<Workflow_VendasCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Workflow_VendasCadastroModel> retorno = new GetApiRetorno<Workflow_VendasCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<Workflow_Vendas>(session))

            {
                try
                {
                    Workflow_Vendas flow = await repo.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(flow, session, sessionT5);
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
        [Route("api/workflow_vendasapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Workflow_VendasCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Workflow_Vendas modelo = new Workflow_Vendas();

                using (ISession session = HibernateUtil.GetSession())                
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())                
                using (var repo = new Conexao<Workflow_Vendas>(session))
                using (var repoAtividade = new Conexao<Workflow_VendasAtividade>(session))




                {
                    using (var repository = new Conexao<Workflow_Vendas>(session))
                    {

                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                        //var minutos = (await repoAtividade.select(p => p.Ativ_Prazo_Minutos == IntService.GetMinutos(value.Objeto.Ativ_Prazo)));    


                        if (modelo == null)
                        {
                            modelo = new Workflow_Vendas
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session,sessionT5, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                    }

                    retorno.Ok = true;
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

        [Route("api/workflow_vendasapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Workflow_VendasAtividade>(session))
                    {

                        Workflow_VendasAtividade modelo = null;
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
        [Route("api/workflow_vendasapi/EditarAtividades")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> EditarAtividades(Workflow_VendasAtividadeModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            retorno.Ok = true;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Workflow_VendasAtividade>(session))
                using (var repositoryAtiv_StatusVenda = new Conexao<Atividade_StatusVenda>(session))
                using (var repoDepartamento = new Conexao<Departamento>(session))
                using (var repoUsuario = new Conexao<IdentityUser>(session))
                {

                    Workflow_VendasAtividade conta = await repository.selectFirst(p => p.Id == request.Id);

                    if (conta != null)
                    {
                        conta.Id = request.Id;
                        conta.Ativ_Descricao = request.Ativ_Descricao;
                        conta.Ativ_Decisao = request.Ativ_Decisao;
                        conta.Ativ_Status = request.Ativ_Status ? Status_Default.Ativo : Status_Default.Inativo;
                        conta.Ativ_Prazo_Minutos = IntService.GetMinutos(request.Ativ_Prazo);
                        conta.Ativ_StatusVenda = repositoryAtiv_StatusVenda.selectFirst(p => p.Id == Convert.ToInt64(request.Ativ_StatusVenda)).Result;
                        conta.Ativ_NAtivReprovada = (request.Ativ_NAtivReprovada);
                        conta.Ativ_NAtivAprovada = request.Ativ_NAtivAprovada;
                        //conta.Ativ_TipoDecisao = request.Ativ_TipoDecisao;
                        conta.Horario_Atendimento = request.Horario_Atendimento;
                        conta.Ativ_Tipodias = request.Ativ_Tipodias;
                        conta.Definir_Responsavel = request.Definir_Responsavel;
                        conta.SetorResponsavel = repoDepartamento.selectFirst(x => x.Id == request.Setor_Departamento).Result;
                        conta.Responsavel = repoUsuario.selectFirst(x => x.Id == request.Responsavel).Result;
                        await repository.Update(conta);
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
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
        [Route("api/workflow_vendasapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(workflow_VendasGetComboConfig request)
        {
            var retorno = new ObjCombo<String>()
            {
                results = new List<IObjComboResult<String>>()

            };
            using (var session = HibernateUtil.GetSession())
            using (var repo_Work_Atividade = new Conexao<Workflow_VendasAtividade>(session))
            using (var repo_AtividadeStatus = new Conexao<Atividade_StatusVenda>(session))
            using (var com = session.Connection.CreateCommand())
            {
                //StringBuilder sql = new StringBuilder();


                List<Workflow_VendasAtividade> Lista_Works = new List<Workflow_VendasAtividade>();

                if (request.idpai.HasValue)
                {
                    Lista_Works = (await repo_Work_Atividade.select(p => p.WorkFlow.Id == request.idpai)).ToList();
                }
                else
                {
                    Lista_Works = (await repo_Work_Atividade.select()).ToList();
                }

                if (request.Config == true)
                {
                    Lista_Works = Lista_Works.Where(p => p.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Cancelada || p.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Reprovada).ToList();
                    //Lista_Works = Lista_Works.Where(p => p.Id_Status_Padrao == Status_Venda.Venda_Cancelada || p.Id_Status_Padrao == Status_Venda.Venda_Reprovada).ToList();

                }
                if (request.Config == false)
                {
                    Lista_Works = Lista_Works.Where(p => p.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Aprovada || p.Ativ_StatusVenda.StatusPadrao == Status_Venda.Analise_Cliente).ToList();
                    //Lista_Works = Lista_Works.Where(p => p.Id_Status_Padrao == Status_Venda.Venda_Aprovada || p.Id_Status_Padrao == Status_Venda.Analise_Cliente).ToList();
                }


                foreach (var Atividade in Lista_Works)
                {

                    retorno.results.Add(new WorkFlow_VendasGetComboModel
                    {
                        Id = Convert.ToString(Atividade.Id),

                        Text = Atividade.Ativ_StatusVenda.Descricao,//repo_AtividadeStatus.selectFirst(p => p.Id == Convert.ToInt64(Atividade.Ativ_StatusVenda)).Result?.Descricao,
                        Atividade_Status = "",
                        Ativ_AgendaTarefa = Atividade.Ativ_AgendaTarefa,
                        Ativ_Status = Convert.ToBoolean(Atividade.Ativ_Status),
                        Ativ_Descricao = Atividade.Ativ_Descricao,
                        Ativ_Prazo = Atividade.Ativ_Prazo_Minutos,
                        Ativ_Tipodias = Convert.ToInt64(Atividade.Ativ_Tipodias),
                        Ativ_StatusVenda = Atividade.Ativ_StatusVenda.Id,//repo_AtividadeStatus.selectFirst(p => p.Id == Convert.ToInt64(Atividade.Ativ_StatusVenda)).Result.Id,
                        Horario_Atendimento = Atividade.Horario_Atendimento,
                        Ativ_Decisao = Convert.ToInt64(Atividade.Ativ_Decisao),
                        Ativ_NAtivAprovada = Atividade.Ativ_NAtivAprovada,
                        Ativ_NAtivReprovada = Atividade.Ativ_NAtivReprovada,
                        Ativ_StatusVenda_Desc = "",


                    });

                }

                //using (var reader = await CommandHelper.ExecuteReader())
                //{
                //    while (await reader.ReadAsync())
                //    {

                //    }
                //}

            }
            return retorno;

        }


        [HttpPost()]
        [Route("api/workflow_vendasapi/GetComboFiltroAprovadoReprovado")]
        public async Task<ObjCombo<String>> GetComboFiltroAprovadoReprovado(workflow_VendasGetComboConfig request)
        {
            var retorno = new ObjCombo<String>()
            {
                results = new List<IObjComboResult<String>>()

            };

            using (var session = HibernateUtil.GetSession())
            using (var repo_Work_Atividade = new Conexao<Workflow_VendasAtividade>(session))
            using (var repo_AtividadeStatus = new Conexao<Atividade_StatusVenda>(session))
            {
                List<Workflow_VendasAtividade> Lista_Works = new List<Workflow_VendasAtividade>();

                Lista_Works = (await repo_Work_Atividade.select()).ToList();
                var idWorflowVendas = repo_Work_Atividade.select(x => x.Id == request.idpai).Result.Select(x => x.WorkFlow.Id).First();

                if (request.Config == true)
                {
                    Lista_Works = Lista_Works.Where(x => x.WorkFlow.Id == idWorflowVendas && x.Id != request.idpai && (x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Analise_Cliente ||
                    x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Avaliação_Aprovada || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Aguardando_Avaliacao_Comercial ||
                    x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Aprovada || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Aguardando_Liberacao
                    || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.liberacao_Aprovada || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.aguardando_Pagamento))
                        .ToList();
                }
                else
                {
                    Lista_Works = Lista_Works.Where(x => x.WorkFlow.Id == idWorflowVendas && x.Id != request.idpai && (x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Avaliação_Reprovada ||
                    x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Cancelada || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.Venda_Reprovada ||
                    x.Ativ_StatusVenda.StatusPadrao == Status_Venda.liberacao_Reprovada || x.Ativ_StatusVenda.StatusPadrao == Status_Venda.pagamento_Rejeitado))
                        .ToList();
                }

                foreach (var Atividade in Lista_Works)
                {
                    retorno.results.Add(new WorkFlow_VendasGetComboModel
                    {
                        Text = Atividade.Ativ_Descricao,
                        Id = Convert.ToString(Atividade.Id)
                    });
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/workflow_vendasapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_WORKFLOW_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }



        public async Task<Workflow_Vendas> BuscaWorkflow(ISession session, Int64 IdWorkflow)
        {
            using (Conexao<Workflow_Vendas> repo = new Conexao<Workflow_Vendas>(session))
            {
                return await repo.selectFirst(p => p.Id == IdWorkflow);
            }
        }



        [HttpGet()]
        [Route("api/workflow_vendasapi/TestaAtribuicaoWorkflow")]
        public async Task<GetApiRetorno> TestaAtribuicaoWorkflow(Int64 IdVenda)
        {

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repo = new Conexao<Venda>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                {
                    var venda = await repo.selectFirst(p => p.Id == IdVenda);
                    var cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente);
                    IWorkflowRepository _workflowRepository = new WorkflowRepository(session);

                    VerificaWorkflowRequestModel requestVerificar = new VerificaWorkflowRequestModel
                    {
                        UF = HtmlEnumExtensions.GetEnumDescription<UF>(cliente.Endereco_Principal.UF),
                        TipoPessoa = cliente.Tipo_Pessoa.ToString(),
                        Categoria = cliente.Categoria_Cliente?.ToString(),
                        Captacao = cliente.Qual_Meio.Id.ToString(),
                        Cidade = cliente.Endereco_Principal.Id_Cidade.ToString(),
                    };

                    var worlflow = await _workflowRepository.FindWorkFlow(requestVerificar);
                    return new GetApiRetorno
                    {
                        Ok = true,
                        Mensagem = worlflow?.FirstOrDefault()?.Descricao
                    };
                }

            }
            catch (Exception ex)
            {
                return new GetApiRetorno
                {
                    Ok = false,
                    Mensagem = ex.Message
                };
            }
        }

        [HttpPost()]
        [Route("api/workflow_vendasapi/VerificaWorkflow")]
        public async Task<GetApiRetorno<List<ObjComboResult<string>>>> VerificaWorkflow(VerificaWorkflowRequestModel request)
        {

            try
            {
                using (var session = HibernateUtil.GetSession())
                {
                    IWorkflowRepository _workflowRepository = new WorkflowRepository(session);
                    var workflowValidos = await _workflowRepository.FindWorkFlow(request);

                    if (workflowValidos.Count > 1)
                    {
                        return new GetApiRetorno<List<ObjComboResult<string>>>
                        {
                            Ok = false,
                            Mensagem = "deve escolher workflow",
                            Objeto = workflowValidos.Where(p => p != null).Select(p => new ObjComboResult<string>
                            {
                                Id = p.Id.ToString(),
                                Text = p.Descricao
                            }).ToList()
                        };
                    }
                    else
                    {
                        return new GetApiRetorno<List<ObjComboResult<string>>>
                        {
                            Ok = true,
                        };
                    }


                }

            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<ObjComboResult<string>>>
                {
                    Ok = false,
                    Mensagem = ex.Message
                };
            }
        }

    }
}