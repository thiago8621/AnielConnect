using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.Excel;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Tipo_AtendimentoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/tipo_atendimentoapi/GetAll")]
        public async Task<DataTableResponse<Tipo_AtendimentoListaModel>> GetAll(Tipo_Atendimento_DataTableRequest request)
        {
            List<Tipo_AtendimentoListaModel> retorno = new List<Tipo_AtendimentoListaModel>();
            IOrderedEnumerable<Tipo_AtendimentoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_TIPO_ATENDIMENTO A ");
                        //.AppendLine("WHERE STATUS = 1");

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

                    sql.AppendLine("SELECT A.ID, A.NOME, B.NOME AS NOME_SCRIPT, A.PRIORIDADE_PADRAO, A.STATUS ");
                    sql.AppendLine("FROM TB_TIPO_ATENDIMENTO A");
                    sql.AppendLine("LEFT JOIN TB_SCRIPT_ATENDIMENTO B");
                    sql.AppendLine("ON (A.ID_SCRIPT = B.ID)");
                    //sql.AppendLine("WHERE A.STATUS = 1 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Tipo_AtendimentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Script = Convert.ToString(reader["NOME_SCRIPT"]),
                                Prioridade_Padrao = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(reader["PRIORIDADE_PADRAO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }


            #region  //Ordenacao
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
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case "nome":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }

                    case "prioridade_Padrao":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Prioridade_Padrao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Prioridade_Padrao);
                            break;

                        }

                    case "script":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Script);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Script);
                            break;
                        }

                    case "status":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }

                }
            }

            #endregion

            //Page e retornando DataTable
            return new DataTableResponse<Tipo_AtendimentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        #region Mobile
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost]
        [Route("api/tipo_atendimentoapi/GetListMobile")]
        public async Task<GetApiRetorno<IEnumerable<Tipo_AtendimentoMobile>>> GetListMobile()
        {
            GetApiRetorno<IEnumerable<Tipo_AtendimentoMobile>> retorno = new GetApiRetorno<IEnumerable<Tipo_AtendimentoMobile>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            {
                try
                {
                    var lista = await repoTipo_Atendimento.select(p => p.Status == Status_Default.Ativo && p.Exibir_Ola == true);
                    var listaRetorno = new List<Tipo_AtendimentoMobile>();

                    foreach (var obj in lista)
                    {
                        Tipo_AtendimentoMobile tmp = new Tipo_AtendimentoMobile
                        {
                            Id = obj.Id,
                            Nome = obj.Nome,
                            prioridade_padrao = Convert.ToString(obj.Prioridade_Padrao),
                            Id_Script_SuporteTecnico = obj.Script_SuporteTecnico?.Id,
                            Id_Tipo_Servico = !String.IsNullOrEmpty(obj.Desc_Tipo_Servico) ? Convert.ToInt64(obj.Desc_Tipo_Servico) : (long?)null
                        };

                        listaRetorno.Add(tmp);
                    }

                    retorno.Objeto = listaRetorno;

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
        #endregion


        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Tipo_AtendimentoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Tipo_AtendimentoListaModel>> retorno = new GetApiRetorno<IEnumerable<Tipo_AtendimentoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            {
                try
                {
                    var lista = await repoTipo_Atendimento.select(p => p.Status == Status_Default.Ativo);

                    var listaRetorno = new List<Tipo_AtendimentoListaModel>();

                    foreach (var obj in lista)
                    {
                        Tipo_AtendimentoListaModel tmp = new Tipo_AtendimentoListaModel
                        {
                            Id = obj.Id,
                            Nome = obj.Nome,
                            Categoria_Atendimento = Convert.ToString(obj.Categoria_Atendimento?.Id),
                            Prioridade_Padrao = Convert.ToString(obj.Prioridade_Padrao.Value),
                            Criar_Os_Encerrar = Convert.ToString(obj.Criar_Os_Encerrar),
                            Exibir_Ola = Convert.ToString(obj.Exibir_Ola)
                        };

                        tmp.Script = obj.Script?.Nome;

                        listaRetorno.Add(tmp);
                    }

                    retorno.Objeto = listaRetorno;

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
        [Route("api/Tipo_Atendimentoapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_ATENDIMENTO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CATEGORIA_ATENDIMENTO = {request.idpai}");
                    }


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CATEGORIA_ATENDIMENTO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME, A.ID_CATEGORIA_ATENDIMENTO,A.PRIORIDADE_PADRAO,A.ID_SCRIPT,A.CRIAR_OS_ENCERRAR, A.VINCULAR_MOTIVOS ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Tipo_AtendimentoComboModel
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
                                    Categoria_Atendimento = Convert.ToString(reader["ID_CATEGORIA_ATENDIMENTO"]),
                                    Prioridade_Padrao = Convert.ToString(reader["PRIORIDADE_PADRAO"]),
                                    Script = Convert.ToString(reader["ID_SCRIPT"]),
                                    Criar_Os_Encerrar = Convert.ToString(reader["CRIAR_OS_ENCERRAR"]),
                                    Vincular_Motivos = Convert.ToString(reader["VINCULAR_MOTIVOS"]),
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


        private Tipo_AtendimentoCadastroModel GetViewObj(Tipo_Atendimento obj)
        {
            return new Tipo_AtendimentoCadastroModel
            {
                Id = obj.Id,
                Id_Script = obj.Script?.Id,
                Nome = obj.Nome,
                Prioridade_Padrao = obj.Prioridade_Padrao,
                Status = obj.Status == Status_Default.Ativo,
                Id_Categoria_Atendimento = obj.Categoria_Atendimento?.Id,
                Criar_Os_Encerrar = obj.Criar_Os_Encerrar,
                Exibir_Ola = obj.Exibir_Ola,
                Vincular_Motivos = obj.Vincular_Motivos,
                Desc_Tipo_Servico = obj.Desc_Tipo_Servico,
                Solicitacao_Cancelamento = obj.Solicitacao_Cancelamento,                
                CodCt = obj.CodCt,
                Projeto = obj.Projeto,
                Id_Script_SuporteTecnico = obj?.Script_SuporteTecnico?.Id,
                Trava_Atendimento_OS = obj.Trava_Atendimento_OS,
                Utilizar_Criacao_Dominio_Email_Atendimento = obj.Criacao_Atendimento_DominioEmail,
                Utilizar_Criacao_Email_Atendimento = obj.Criacao_Atendimento_Email,
                Encerrar_Atendimento_Junto_OS = obj.Encerrar_Atendimento_Junto_OS,
                Motivos = obj.Motivos.Select(p => new Tipo_AtendimentoMotivoCadastroModel
                {
                    Id = p.Id,
                    Id_Motivo = p.Motivo?.Id ?? 0,
                    Desc_Motivo = p.Motivo?.Descricao ?? String.Empty
                }).ToList(),
                MotivosStatus = new List<Motivo_Status_Tipo_AtendimentoListModel>()
            };
        }

        private void BindViewToObj(ISession session, ref Tipo_Atendimento objBanco, Tipo_AtendimentoCadastroModel view)
        {
            using (var repoScript_Atendimento = new Conexao<Script_Atendimento>(session))
            using (var repoCategoria_Atendimento = new Conexao<Categoria_Atendimento>(session))
            using (var repoScript_SuporteTecnico = new Conexao<Script_SuporteTecnico>(session))
            using (var repoMotivo_Status_Atendimento = new Conexao<Motivo_Status_Atendimento>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Nome = view.Nome;
                objBanco.Prioridade_Padrao = view.Prioridade_Padrao;
                objBanco.Script = view.Id_Script.HasValue ? repoScript_Atendimento.selectFirst(p => p.Id == view.Id_Script.Value).Result : null;
                objBanco.Criar_Os_Encerrar = view.Criar_Os_Encerrar;
                objBanco.Exibir_Ola = view.Exibir_Ola;
                objBanco.Solicitacao_Cancelamento = view.Solicitacao_Cancelamento;
                objBanco.Encerrar_Atendimento_Junto_OS = view.Encerrar_Atendimento_Junto_OS;
                objBanco.Trava_Atendimento_OS = view.Trava_Atendimento_OS;
                objBanco.Categoria_Atendimento = view.Id_Categoria_Atendimento.HasValue ? repoCategoria_Atendimento.selectFirst(p => p.Id == view.Id_Categoria_Atendimento.Value).Result : null;
                objBanco.Desc_Tipo_Servico = view.Desc_Tipo_Servico;
                objBanco.Script_SuporteTecnico = view.Id_Script_SuporteTecnico.HasValue ? repoScript_SuporteTecnico.selectFirst(p => p.Id == view.Id_Script_SuporteTecnico.Value).Result : null;
                objBanco.CodCt = view.CodCt;
                objBanco.Vincular_Motivos = view.Vincular_Motivos;
                objBanco.Projeto = view.Projeto;
                List<Tipo_Atendimento_Motivo_Status> meiosTmp = objBanco.Motivos.ToList();
                foreach (var motivo in view.Motivos.FindAll(p => p.Id == 0))
                {
                    meiosTmp.Add(new Tipo_Atendimento_Motivo_Status
                    {
                        Motivo = repoMotivo_Status_Atendimento.selectFirst(p => p.Id == motivo.Id_Motivo.Value).Result,
                        Tipo_Atendimento = objBanco
                    });
                }

                objBanco.Motivos = meiosTmp;
            }
        }

        [HttpPost()]
        [Route("api/tipo_atendimentoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_TIPO_ATENDIMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/tipo_atendimentoapi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine("FROM TB_TIPO_ATENDIMENTO A");
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
                                });
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
        [Route("api/tipo_atendimentoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_TIPO_ATENDIMENTO WHERE ID IN {_ids}");

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
        [Route("api/Tipo_AtendimentoApi/FindById")]
        public async Task<GetApiRetorno<Tipo_AtendimentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Tipo_AtendimentoCadastroModel> retorno = new GetApiRetorno<Tipo_AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Tipo_Atendimento>(session))
            using (var repositoryFluxoAtendimento = new Conexao<FluxoAtendimento>(session))
            using (var repositoryEncerramentoAtendimento = new Conexao<EncerramentoAtendimento>(session))
            using (var repositoryContrato = new Conexao<ContratoAniel>(sessionT5))
            using (var repositoryCodigoEncerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5))
            using (var repositoryDepartamento = new Conexao<Departamento>(session))
            using (var repositoryMotivoStatusTipo = new Conexao<Motivo_Status_Tipo_Atendimento>(session))
            using (var repositoryMotivoStatus = new Conexao<Motivo_Status_Atendimento>(session))            
            using (var repositoryContaEmail = new Conexao<Conta_Email>(session))
            {
                try
                {
                    var tipoAtendimento = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(tipoAtendimento);
                    var etapasFluxo = await repositoryFluxoAtendimento.select(p => p.Tipo_Atendimento.Id == Id);

                    var emails = await repositoryContaEmail.select(p => p.Status == Status_Default.Ativo);
                    retorno.Objeto.Emails = (emails.Count() == 0) ? new List<EmailListaModel>() :
                                                   emails.Select(p => new EmailListaModel
                                                   {
                                                       Id = p.Id,
                                                       Email = p.Email,
                                                       ServidorEnvio = p.Servidor_Smtp,
                                                       PortaEnvio = Convert.ToString(p.Porta_Smtp),
                                                       ServidorRecebimento = Convert.ToString(p.Servidor_Imap),
                                                       PortaRecebimento = Convert.ToString(p.Porta_Imap),
                                                       Utilizar_SSL_Imap = p.Utilizar_Ssl_Imap,
                                                       Utilizar_SSL_Smtp = p.Utilizar_Ssl_Smtp,
                                                       Principal = p.Principal,
                                                       Senha = p.Senha,
                                                       AuthMethodImap = p.AuthMethodImap,
                                                       AuthMethodImap_Descricao = HtmlEnumExtensions.GetEnumDescription<AuthMethodImap>(p.AuthMethodImap)
                                                   }).ToList();

                    var fluxos = await Task.WhenAll(etapasFluxo.Select(async p =>
                    {
                        var encerramentos = await repositoryEncerramentoAtendimento.select(y => y.FluxoAtendimento.Id == p.Id);
                        
                        var obj = new TipoAtendimento_EtapaFluxoCadastroModel();
                        obj.Id = p.Id;
                        obj.Cod_Encerramento = p.Cod_Encerramento;
                        obj.Contrato = p.Contrato;
                        obj.Contrato_Desc = String.IsNullOrEmpty(p.Contrato) ? String.Empty : repositoryContrato.selectFirst(y => y.CodCt == p.Contrato).Result?.Descricao;
                        obj.Encerra_Atendimento = p.Encerra_Atendimento;
                        obj.Etapa_Decisoria = p.Etapa_Decisoria;
                        obj.Proxima_Etapa = String.IsNullOrEmpty(p.Proxima_Etapa) ? String.Empty : repositoryFluxoAtendimento.selectFirst(a => a.Proxima_Etapa == p.Proxima_Etapa).Result?.Proxima_Etapa;
                        //obj.Proxima_Etapa_Desc = String.IsNullOrEmpty(p.Proxima_Etapa) ? String.Empty : repositoryFluxoAtendimento.selectFirst(a => a.Proxima_Etapa == p.Proxima_Etapa).Result?.Descricao;
                        obj.Sequencia = p.Sequencia;
                        obj.Setor = p.Setor_Departamento;
                        obj.AlterarSetor_Responsavel = p.AlterarSetor_Responsavel;
                        obj.Etapa_CriaOS = p.Etapa_CriaOS;
                        obj.Baixar_OS = p.Baixar_OS;
                        obj.Desc_AlterarSetor_Responsavel = p.AlterarSetor_Responsavel ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        obj.Desc_Etapa_CriaOS = p.Etapa_CriaOS ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        obj.Desc_Baixar_OS = p.Baixar_OS ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        obj.Responsavel = !String.IsNullOrEmpty(p.Responsavel) ? p.Responsavel : String.Empty;
                        obj.Setor_Desc = String.IsNullOrEmpty(p.Setor_Departamento) ? String.Empty : repositoryDepartamento.selectFirst(x => x.Cod_Depto == p.Setor_Departamento).Result?.Descricao;
                        obj.Tempo_Exc = StringService.HoraParaString(p.Tempo_Exc) ?? String.Empty;
                        obj.Cod_Encerramento_Desc = String.IsNullOrEmpty(p.Cod_Encerramento) ? String.Empty : repositoryCodigoEncerramento.selectFirst(Z => Z.Cod_Encerramento == p.Cod_Encerramento).Result?.Descricao;
                        obj.Descricao = p.Descricao;
                        obj.Encerra_Atendimento_Desc = p.Encerra_Atendimento ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        obj.Etapa_Decisoria_Desc = p.Etapa_Decisoria ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO;
                        obj.StatusFluxoAtendimento = p.Status == Status_Default.Ativo ? true : false;
                        obj.StatusAtendimento = p.StatusAtendimento;
                        obj.StatusAtendimentoDesc = !String.IsNullOrEmpty(Convert.ToString(p.StatusAtendimento)) ? p.StatusAtendimento != 0 ? HtmlEnumExtensions.GetEnumDescription<Status_AtendimentoEtapa>(p.StatusAtendimento.Value) : string.Empty : string.Empty;
                        obj.Motivo_Status_Atendimento = p.Motivo_Status_Atendimento;
                        obj.Motivo_Status_AtendimentoDesc = String.IsNullOrEmpty(p.Motivo_Status_Atendimento) ? String.Empty : repositoryMotivoStatus.selectFirst(m => m.Id == Convert.ToInt64(p.Motivo_Status_Atendimento)).Result?.Descricao;
                        obj.Status_Desc = HtmlEnumExtensions.GetEnumDescription<Status_Default>(p.Status);

                        obj.Encerramentos = encerramentos.Count() == 0 ? new List<Tipo_AtendimentoEtapaEncerramentoCadastroModel>() : encerramentos.Select(y => new Tipo_AtendimentoEtapaEncerramentoCadastroModel
                        {
                            Id = y.Id,
                            Cod_Encerramento = y.Cod_Encerramento,
                            Contrato = y.Contrato,
                            Contrato_Desc = String.IsNullOrEmpty(y.Contrato) ? String.Empty : (repositoryContrato.selectFirst(x => x.CodCt == y.Contrato))?.Result.Descricao,
                            Cod_Encerramento_Desc = String.IsNullOrEmpty(y.Cod_Encerramento) ? String.Empty : (repositoryCodigoEncerramento.selectFirst(Z => Z.Cod_Encerramento == y.Cod_Encerramento))?.Result.Descricao,
							IdFluxoEtapa = y.IdFluxoEtapa,
							DescFluxoEtapa = repositoryFluxoAtendimento.selectFirst(p => p.Id == y.IdFluxoEtapa).Result?.Descricao
                        }).ToList();                      

                        return obj;
                    }));

                    retorno.Objeto.Fluxo = fluxos.ToList();


                    retorno.Objeto.Possui_Etapa = etapasFluxo.Count() > 0;


                    //Lista o Motivo Status Tipo Atendimento
                    retorno.Objeto.MotivosStatus = (await repositoryMotivoStatusTipo.select(p => p.Tipo_Atendimento.Id == Id)).ToList().Select(p => new Motivo_Status_Tipo_AtendimentoListModel
                    {
                        Id = p.Id,
                        Motivo_Status_Atendimento = Convert.ToString(p.Motivo_Status_Atendimento.Id),
                        Motivo_Status_Atendimento_Desc = p.Motivo_Status_Atendimento.Descricao,
                        Status_Motivo = Convert.ToString(p.Status_Motivo),
                        Status_Motivo_Desc = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(p.Status_Motivo),
                        Id_Tipo_Atendimento = Id

                    }).ToList();

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
        [Route("api/Tipo_AtendimentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Tipo_AtendimentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Tipo_Atendimento modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Atendimento>(session))
                    using (var repositoryFluxo = new Conexao<FluxoAtendimento>(session))
                    using (var repositoryEncerramentoAtendimento = new Conexao<EncerramentoAtendimento>(session))
                    using (var repositoryContaEmail = new Conexao<Conta_Email>(session))
                    using (var repositoryMotivo = new Conexao<Motivo_Status_Tipo_Atendimento>(session))
                    using (var repositoryMotivoStatus = new Conexao<Motivo_Status_Atendimento> (session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Tipo_Atendimento
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

                        BindViewToObj(session, ref modelo, value.Objeto);

                        List<FluxoAtendimento> fluxos = new List<FluxoAtendimento>();
                        List<EncerramentoAtendimento> encerramentos = new List<EncerramentoAtendimento>();

                        foreach (var etapa in value.Objeto.Fluxo)
                        {
                            var fluxo = new FluxoAtendimento
                            {
                                Id = etapa.Id,
                                Sequencia = etapa.Sequencia.Value,
                                Descricao = etapa.Descricao,
                                Proxima_Etapa = etapa.Proxima_Etapa,
                                //Cod_Encerramento = etapa.Cod_Encerramento,
                                //Contrato = etapa.Contrato,
                                Encerra_Atendimento = etapa.Encerra_Atendimento,
                                Etapa_Decisoria = etapa.Etapa_Decisoria,
                                Setor_Departamento = etapa.Setor,
                                AlterarSetor_Responsavel = etapa.AlterarSetor_Responsavel,
                                Etapa_CriaOS = etapa.Etapa_CriaOS,
                                Baixar_OS = etapa.Baixar_OS,
                                Responsavel = etapa.Responsavel,
                                Tempo_Exc = StringService.StringParaHora(etapa.Tempo_Exc),
                                Status = etapa.StatusFluxoAtendimento == true ? Status_Default.Ativo : Status_Default.Inativo,
                                StatusAtendimento = etapa.StatusAtendimento,
                                Motivo_Status_Atendimento = etapa.Motivo_Status_Atendimento,
                                Tipo_Atendimento = modelo,

                            };
                            fluxos.Add(fluxo);


                            foreach (var encerramento in etapa.Encerramentos.FindAll(p => p.Id == 0))
                            {
                                bool existeEncerramento = repositoryEncerramentoAtendimento.select(P => P.Cod_Encerramento == encerramento.Cod_Encerramento
                                              && P.Contrato == encerramento.Contrato
                                              && P.FluxoAtendimento.Id == fluxo.Id).Result.Count > 0 ? true : false;

                                if (!existeEncerramento)
                                {
                                    var itemEncerramento = new EncerramentoAtendimento
                                    {
                                        Cod_Encerramento = encerramento.Cod_Encerramento,
                                        Contrato = encerramento.Contrato,
                                    IdFluxoEtapa = encerramento.IdFluxoEtapa,
                                        FluxoAtendimento = fluxo
                                    };
                                    encerramentos.Add(itemEncerramento);
                                }
                            }
                        }

                        await repository.SaveOrUpdate(modelo);

                        if (fluxos.Count() > 0)
                        {
                            await repositoryFluxo.SaveOrUpdate(fluxos);

                            if (encerramentos.Count() > 0)
                            {
                                await repositoryEncerramentoAtendimento.SaveOrUpdate(encerramentos);
                            }
                        }

                        ////Grava a lista de Motivo Status Tipo Atendimento
                        List<Motivo_Status_Tipo_Atendimento> motivos = new List<Motivo_Status_Tipo_Atendimento>();

                        foreach (var motivoStatus in value.Objeto.MotivosStatus)
                        {
                            if (motivoStatus.Id == 0)
                            {
                                var listaMotivos = new Motivo_Status_Tipo_Atendimento
                                {
                                    Id = motivoStatus.Id,
                                    Status_Motivo = (Status_Atendimento)Enum.Parse(typeof(Status_Atendimento), Convert.ToString(motivoStatus.Status_Motivo)),
                                    Motivo_Status_Atendimento = (await repositoryMotivoStatus.select(p => p.Id == Convert.ToInt32(motivoStatus.Motivo_Status_Atendimento))).FirstOrDefault(),
                                    Tipo_Atendimento = modelo,
                                };
                                motivos.Add(listaMotivos);
                            }
                        }

                        if (motivos.Count() > 0)
                        {
                            await repositoryMotivo.SaveOrUpdate(motivos);
                        }
                    }
                    retorno.IdObjeto = modelo.Id;
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

        [Route("api/Tipo_AtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Atendimento>(session))
                    {

                        Tipo_Atendimento modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

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
    }


}
