using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class TarefasVendedorApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/TarefasVendedorApi/Gravar")]
        public async Task<PostApiRetorno<TarefaCardModel>> Gravar([FromBody] TarefaVendedorCadastroModel model)
        {
            try
            {
                PostApiRetorno<TarefaCardModel> retorno = new PostApiRetorno<TarefaCardModel>();
                using (var tarefaApi = new TipoTarefaApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
                using (var participantesRepo = new Conexao<ParticipanteTarefa>(session))
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var logVendasRepo = new Conexao<LogVendas>(sessionLog))
                {
                    List<ParticipanteTarefa> participanteList = new List<ParticipanteTarefa>();


                    var objBanco = BindViewToObj(model);

                    if (model.Id == 0)
                    {
                        await tarefaRepo.SaveOrUpdate(objBanco);

                        if (objBanco.IdVenda != 0)
                        {
                            var logVenda = new LogVendas
                            {
                                Id = 0,
                                TipoOperacao = Sinapse.ModelLog.TipoOperLog.Alteracao,
                                Campo = "Criação de Tarefa",
                                IdLog = objBanco.IdVenda,
                                Campo_ValorDepois = objBanco.Assunto,
                                Chave = objBanco.IdVenda.ToString(),
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                NomeUsuarioCriacao = User.Identity.GetUserName(),
                                DescricaoResource = "Criação de Tarefa"
                            };

                            await logVendasRepo.SaveOrUpdate(logVenda);
                        }
                    }
                    else
                    {
                        await tarefaRepo.Save(objBanco);

                        if (objBanco.IdVenda != 0)
                        {
                            var logVenda = new LogVendas
                            {
                                Id = 0,
                                TipoOperacao = Sinapse.ModelLog.TipoOperLog.Alteracao,
                                Campo = "Alteração de Tarefa",
                                IdLog = objBanco.IdVenda,
                                Campo_ValorDepois = objBanco.Assunto,
                                Chave = objBanco.IdVenda.ToString(),
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                NomeUsuarioCriacao = User.Identity.GetUserName(),
                                DescricaoResource = "Alteração de Tarefa"
                            };

                            await logVendasRepo.SaveOrUpdate(logVenda);
                        }
                    }

                    foreach (var participante in model.Emails)
                    {
                        if (model.Id != 0)
                        {
                            string[] participanteExistente = await GetParticipantes(objBanco.Id);

                            if (!participanteExistente.Contains(participante))
                            {
                                var objPart = new ParticipanteTarefa { IdTarefa = objBanco.Id, Email = participante };
                                participanteList.Add(objPart);
                            }
                        }
                        else
                        {
                            var objPart = new ParticipanteTarefa { IdTarefa = objBanco.Id, Email = participante };
                            participanteList.Add(objPart);
                        }
                    }

                    if (model.Id != 0)
                        await participantesRepo.Remove(participanteList);

                    if (participanteList.Count > 0)
                        await participantesRepo.Save(participanteList);

                    return new PostApiRetorno<TarefaCardModel>()
                    {
                        IdObjeto = new TarefaCardModel
                        {
                            Assunto = objBanco.Assunto,
                            Tarefa = tarefaApi.GetById(objBanco.IdTipoTarefa).Result?.Descricao,
                            HoraInicio = objBanco.HorarioInicio,
                            HoraFim = objBanco.HorarioFim,
                            Data = objBanco.DataFim.ToString("dd/MM/yyyy"),
                            Local = objBanco.Local,
                            Responsavel = objBanco.Responsavel,
                            Id = objBanco.Id.ToString(),
                            Prioridade = objBanco.Prioridade,
                            NovoCadastro = model.Id == 0
                        },
                        Ok = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno<TarefaCardModel>()
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex?.InnerException?.Message
                };
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/GetById")]
        public async Task<TarefaVendedor> GetById(long idTarefa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            {
                return await tarefaRepo.selectFirst(x => x.Id == idTarefa);
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/GetParticipantes")]
        public async Task<string[]> GetParticipantes(long idTarefa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<ParticipanteTarefa>(session))
            {
                var result = await tarefaRepo.select(x => x.IdTarefa == idTarefa);
                return result.Select(x => x.Email).ToArray();
            }
        }


        [HttpPost()]
        [Route("api/TarefasVendedorApi/GetDetalhes")]
        public async Task<TarefaVendedorCadastroModel> GetDetalhes(long idTarefa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            {
                var tarefa = await tarefaRepo.selectFirst(x => x.Id == idTarefa);

                return new TarefaVendedorCadastroModel()
                {
                    TipoTarefa = tarefa.IdTipoTarefa,
                    Venda = tarefa.IdVenda,
                    Assunto = tarefa.Assunto,
                    DataFim = tarefa.DataFim.ToString("dd/MM/yyyy"),
                    DataInicio = tarefa.DataInicio.ToString("dd/MM/yyyy"),
                    HoraInicio = tarefa.HorarioInicio,
                    HoraFim = tarefa.HorarioFim,
                    Local = tarefa.Local,
                    Observacao = tarefa.Observacao,
                    Emails = await GetParticipantes(idTarefa),
                    Id = tarefa.Id
                };
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/FinalizarTarefa")]
        public async Task<TarefaVendedor> FinalizarTarefa(long idTarefa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            {
                var tarefa = await tarefaRepo.selectFirst(x => x.Id == idTarefa);
                tarefa.Status = StatusTarefaEnum.Concluida;
                await tarefaRepo.SaveOrUpdate(tarefa);
                return tarefa;
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/ExcluirTarefa")]
        public async Task<TarefaVendedor> ExcluirTarefa(long idTarefa)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var logVendasRepo = new Conexao<LogVendas>(sessionLog))
            {
                var tarefa = await tarefaRepo.selectFirst(x => x.Id == idTarefa);
                tarefa.Status = StatusTarefaEnum.Excluida;

                if (tarefa.IdVenda != 0)
                {
                    var logVenda = new LogVendas
                    {
                        Id = 0,
                        TipoOperacao = Sinapse.ModelLog.TipoOperLog.Alteracao,
                        Campo = "Exclusão de Tarefa",
                        IdLog = tarefa.IdVenda,
                        Campo_ValorDepois = tarefa.Assunto,
                        Chave = tarefa.IdVenda.ToString(),
                        DataHoraCriacao = DateTime.Now,
                        UsuarioCriacao = User.Identity.GetUserId(),
                        NomeUsuarioCriacao = User.Identity.GetUserName(),
                        DescricaoResource = "Exclusão de Tarefa"
                    };

                    await logVendasRepo.SaveOrUpdate(logVenda);
                }


                await tarefaRepo.SaveOrUpdate(tarefa);
                return tarefa;
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/GetByVendedor")]
        public async Task<List<object>> GetByVendedor(long idVendedor)
        {
            List<object> retorno = new List<object>();

            using (var tarefaApi = new TipoTarefaApiController())
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            {
                foreach (var tarefa in await tarefaRepo.select(x => x.IdVendedor == idVendedor && x.Status == StatusTarefaEnum.Aberta))
                {
                    retorno.Add(new TarefaCardModel()
                    {
                        Id = tarefa.Id.ToString(),
                        Tarefa = tarefaApi.GetById(tarefa.IdTipoTarefa).Result?.Descricao,
                        Assunto = tarefa.Assunto,
                        HoraInicio = tarefa.HorarioInicio,
                        HoraFim = tarefa.HorarioFim,
                        Data = tarefa.DataFim.ToString("dd/MM/yyyy"),
                        Local = tarefa.Local,
                        Responsavel = tarefa.Responsavel,
                        Prioridade = tarefa.Prioridade,
                        Observacao = tarefa.Observacao,
                    });
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/GetByUsuario")]
        public async Task<List<object>> GetByUsuario(string usuario, string buscaVendedor)
        {
            List<object> retorno = new List<object>();

            using (var tarefaApi = new TipoTarefaApiController())
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            using (var userRepo = new Conexao<IdentityUser>(session))
            using (var funcionarioRepo = new Conexao<Funcionario>(session))
            {
                IdentityUser user = await userRepo.selectFirst(p => p.Id == usuario);

                //vendedor 
                if (user?.Tipo_Usuario == 3 && user?.Funcionario?.Id != null)
                {
                    foreach (var tarefa in await tarefaRepo.select(x => x.IdVendedor == user.Funcionario.Id && x.Status == StatusTarefaEnum.Aberta))
                    {
                        retorno.Add(new TarefaCardModel()
                        {
                            Id = tarefa.Id.ToString(),
                            Tarefa = tarefaApi.GetById(tarefa.IdTipoTarefa).Result?.Descricao,
                            Assunto = tarefa.Assunto,
                            HoraInicio = tarefa.HorarioInicio,
                            HoraFim = tarefa.HorarioFim,
                            Data = tarefa.DataFim.ToString("dd/MM/yyyy"),
                            Local = tarefa.Local,
                            Responsavel = tarefa.Responsavel,
                            Prioridade = tarefa.Prioridade,
                            Observacao = tarefa.Observacao,
                        });
                    }
                }

                //supervisor 
                if (user?.Tipo_Usuario == 2 && user?.Funcionario?.Id != null)
                {
                    var funcionarios = (await funcionarioRepo.select(p => p.Supervisor.Id == user.Funcionario.Id)).ToList();
                    if (String.IsNullOrEmpty(buscaVendedor) == false)
                    {
                        funcionarios = funcionarios.FindAll(p => p.Nome.Contains(buscaVendedor));
                    }

                    foreach (var item in funcionarios)
                    {
                        foreach (var tarefa in await tarefaRepo.select(x => x.IdVendedor == item.Id && x.Status == StatusTarefaEnum.Aberta))
                        {
                            retorno.Add(new TarefaCardModel()
                            {
                                Id = tarefa.Id.ToString(),
                                Tarefa = tarefaApi.GetById(tarefa.IdTipoTarefa).Result?.Descricao,
                                Assunto = tarefa.Assunto,
                                HoraInicio = tarefa.HorarioInicio,
                                HoraFim = tarefa.HorarioFim,
                                Data = tarefa.DataFim.ToString("dd/MM/yyyy"),
                                Local = tarefa.Local,
                                Responsavel = tarefa.Responsavel,
                                Prioridade = tarefa.Prioridade,
                                Observacao = tarefa.Observacao,
                            });
                        }
                    }
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/Relatorio")]
        public async Task<DataTableResponse<TarefaVendedorResponse>> Relatorio(TarefaRelatorioRequest request)
        {
            List<TarefaVendedorResponse> retorno = new List<TarefaVendedorResponse>();

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT * FROM TB_TAFERA A ");
                    sql.AppendLine(" INNER JOIN TB_TIPO_TAREFA B ON (B.ID = A.ID_TIPO_TAREFA) ");
                    sql.AppendLine(" WHERE A.STATUS = 2 ");

                    if (request.Venda > 0)
                    {
                        parametros.Add("idVenda", request.Venda);
                        sql.AppendLine(" AND A.ID_VENDA = @idVenda ");
                    }
                    if (request.Vendedor > 0)
                    {
                        parametros.Add("idVendedor", request.Vendedor);
                        sql.AppendLine(" AND A.ID_VENDEDOR = @idVendedor ");
                    }
                    if (request.TipoTarefa > 0)
                    {
                        parametros.Add("idTipo", request.TipoTarefa);
                        sql.AppendLine(" AND A.ID_TIPO_TAREFA = @idTipo ");
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {
                            var prioridade = reader["PRIORIDADE"] != DBNull.Value ? Convert.ToInt16(reader["PRIORIDADE"]) : 1;

                            retorno.Add(new TarefaVendedorResponse
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                DataInicio = Convert.ToDateTime(reader["DATA_INICIO"]).ToString("dd/MM/yyyy"),
                                DataFim = Convert.ToDateTime(reader["DATA_FIM"]).ToString("dd/MM/yyyy"),
                                HorarioFim = Convert.ToString(reader["HORA_FIM"]),
                                HorarioInicio = Convert.ToString(reader["HORA_INICIO"]),
                                Local = Convert.ToString(reader["LOCAL"]),
                                Assunto = Convert.ToString(reader["ASSUNTO"]),
                                Venda = Convert.ToString(reader["ID_VENDA"]),
                                TipoTarefa = Convert.ToString(reader["DESCRICAO"]),
                                Prioridade = HtmlEnumExtensions.GetEnumDescription((NivelPrioridadeTarefaVendedorEnum)prioridade),
                                Observacao = Convert.ToString(reader["OBSERVACAO"]),
                            });
                        }
                    }

                }
            }

            return new DataTableResponse<TarefaVendedorResponse>
            {
                draw = request.draw,
                recordsFiltered = retorno.Count,
                data = retorno,
                error = ""
            };
        }

        TarefaVendedor BindViewToObj(TarefaVendedorCadastroModel model)
        {
            return new TarefaVendedor
            {
                Id = model.Id,
                IdVenda = model.Venda,
                IdTipoTarefa = model.TipoTarefa,
                Assunto = model.Assunto,
                DataInicio = Convert.ToDateTime(model.DataInicio),
                DataFim = Convert.ToDateTime(model.DataFim),
                HorarioInicio = model.HoraInicio,
                HorarioFim = model.HoraFim,
                Observacao = model.Observacao,
                Local = model.Local,
                Prioridade = (NivelPrioridadeTarefaVendedorEnum)model.Prioridade,
                Repetir = Convert.ToBoolean(model.Repetir),
                Responsavel = model.Responsavel,
                IdVendedor = model.Vendedor,
                Status = StatusTarefaEnum.Aberta,
            };
        }

        [HttpPost()]
        [Route("api/TarefasVendedorApi/Reabrir")]
        public async Task Reabrir(RequestReabrir request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var tarefaRepo = new Conexao<TarefaVendedor>(session))
            {
                foreach (long id in request.Ids)
                {
                    var tarefa = await tarefaRepo.selectFirst(x => x.Id == id);
                    tarefa.Status = StatusTarefaEnum.Aberta;
                    tarefa.DataFim = DateTime.Now;

                    await tarefaRepo.SaveOrUpdate(tarefa);
                }
            }
        }
    }

    public class RequestReabrir
    {
        public long[] Ids { get; set; }
    }

    public class TarefaVendedorViewModel
    {
        public List<TarefaVendedor> Tarefas { get; set; }
    }

    public class TarefaVendedorResponse
    {
        public string TipoTarefa { get; set; }
        public string Venda { get; set; }
        public string Assunto { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string HorarioInicio { get; set; }
        public string HorarioFim { get; set; }
        public string Local { get; set; }
        public string Observacao { get; set; }
        public string Prioridade { get; set; }
        public long Id { get; internal set; }
    }

    public class TarefaRelatorioRequest : DataTableRequest
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public long TipoTarefa { get; set; }
        public long Venda { get; set; }
        public long Prioridade { get; set; }
        public long Vendedor { get; set; }
    }

    public class TarefaCardModel
    {
        public string Id { get; set; }
        public string Tarefa { get; set; }
        public string Assunto { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string Data { get; set; }
        public string Local { get; set; }
        public string Responsavel { get; set; }
        public NivelPrioridadeTarefaVendedorEnum Prioridade { get; set; }
        public bool NovoCadastro { get; set; }
        public string Observacao { get; internal set; }
    }


    public class TarefaVendedorCadastroModel
    {
        public long Id { get; set; }
        public long TipoTarefa { get; set; }
        public long Venda { get; set; }
        public long Vendedor { get; set; }
        public string Responsavel { get; set; }
        public string Assunto { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }
        public string Local { get; set; }
        public string Observacao { get; set; }
        public string Repetir { get; set; }
        public string[] Emails { get; set; }
        public int Prioridade { get; set; }
    }
}