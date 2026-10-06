using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
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
    public class SLA_AtendimentoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/SLA_Atendimentoapi/GetAll")]
        public async Task<DataTableResponse<SLA_AtendimentoListaModel>> GetAll(SLA_Atendimento_DataTableRequest request)
        {
            List<SLA_AtendimentoListaModel> retorno = new List<SLA_AtendimentoListaModel>();
            List<ObjBusca<Int64>> listaHorarios = new List<ObjBusca<Int64>>();
            IOrderedEnumerable<SLA_AtendimentoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
            Dictionary<String, Object> parametrosHorario = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID,A.DESCRICAO ");
                    sql.AppendLine("FROM TB_HORARIO_CONNECT A ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametrosHorario, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            listaHorarios.Add(new ObjBusca<Int64>
                            {
                                Label = Convert.ToString(reader["DESCRICAO"]),
                                Value = Convert.ToInt64(reader["ID"]),
                            });
                        }
                    }

                }
            }


            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_SLA_ATENDIMENTO"); //WHERE STATUS = 1  

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
                    StringBuilder sqlWhere = new StringBuilder(" WHERE A.ID > 0 ");

                    sql.AppendLine("SELECT A.STATUS,A.ID, A.MINUTOS, B.NOME AS TIPO_ATENDIMENTO, A.CONTAGEM_PRAZO, A.ID_HORARIO,");
                    sql.AppendLine("A.PRIORIDADE_ATENDIMENTO, C.DESCRICAO AS CATEGORIA_CLIENTE ");
                    sql.AppendLine("FROM TB_SLA_ATENDIMENTO A ");

                    sql.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO B ");
                    sql.AppendLine("ON (A.ID_TIPO_ATENDIMENTO = B.ID) ");

                    sql.AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE C ");
                    sql.AppendLine("ON (A.ID_CATEGORIA_CLIENTE = C.ID) ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(B.NOME) LIKE UPPER(@SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.PrioridadeAtendimento))
                    {
                        sqlWhere.AppendLine(" AND A.ID = @prioridade ");
                        parametros.Add("prioridade", request.PrioridadeAtendimento);
                    }

                    if (!String.IsNullOrEmpty(request.CategoriaCliente))
                    {
                        sqlWhere.AppendLine(" AND A.ID_CATEGORIA_CLIENTE = @categoria ");
                        parametros.Add("categoria", request.CategoriaCliente);
                    }

                    if (!String.IsNullOrEmpty(request.ContagemPrazo))
                    {
                        sqlWhere.AppendLine(" AND A.CONTAGEM_PRAZO = @contagem ");
                        parametros.Add("contagem", request.ContagemPrazo);
                    }

                    if (!String.IsNullOrEmpty(request.HorarioAtendimento))
                    {
                        sqlWhere.AppendLine(" AND A.ID_HORARIO = @horario ");
                        parametros.Add("horario", request.HorarioAtendimento);
                    }

                    if (!String.IsNullOrEmpty(request.FiltroStatus))
                    {
                        sqlWhere.AppendLine(" AND A.STATUS = @status ");
                        parametros.Add("status", request.FiltroStatus);
                    }

                    sql.Append(sqlWhere);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var id_horario = reader["ID_HORARIO"] != DBNull.Value ? Convert.ToInt64(reader["ID_HORARIO"]) : (Int64?)null;
                            ObjBusca<Int64> horario = listaHorarios.Find(p => p.Value == id_horario);
                            var _categoriastring = Convert.ToString(reader["CATEGORIA_CLIENTE"]);
                            retorno.Add(new SLA_AtendimentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Tipo_Atendimento = Convert.ToString(reader["TIPO_ATENDIMENTO"]),
                                Prioridade_Atendimento = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>(Convert.ToString(reader["PRIORIDADE_ATENDIMENTO"])),
                                Categoria_Cliente = _categoriastring == "" ? Resources.RsBase.INDIFERENTE : _categoriastring,
                                Horas = reader["MINUTOS"] == DBNull.Value ? "00:00" : IntService.ToTimeString(Convert.ToInt32(reader["MINUTOS"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Contagem_Prazo = HtmlEnumExtensions.GetEnumDescription<Contagem_Prazo>(Convert.ToString(reader["CONTAGEM_PRAZO"])),
                                Horario_Atendimento = horario?.Label
                            });
                        }
                    }

                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Tipo_Atendimento
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Atendimento);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Atendimento);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<SLA_AtendimentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private async Task<SLA_AtendimentoCadastroModel> GetViewObj(SLA_Atendimento obj)
        {
            return new SLA_AtendimentoCadastroModel
            {
                Id = obj.Id,
                Id_Tipo_Atendimento = obj.Tipo_Atendimento.Id,
                Tipo_Atendimento_Desc = obj.Tipo_Atendimento.Nome,
                Prioridade_Atendimento = obj.Prioridade_Atendimento,
                Id_Categoria_Cliente = obj.Categoria_Cliente == null ? (Int64?)null : obj.Categoria_Cliente.Id,
                Horas = IntService.ToTime99String(obj.Minutos),
                Id_Horario = obj.Id_Horario,
                Contagem_Prazo = obj.Contagem_Prazo,

                PlanosProdutos = (obj.PlanosProdutos.Count() == 0) ? new List<PlanoProdutoListModel>() :
                                                   obj.PlanosProdutos.Select(p => new PlanoProdutoListModel
                                                   {
                                                       Descricao = p.Plano != null ? p.Plano.Descricao : p.Produto.Descricao,
                                                       Tipo = p.Plano == null ? PlanoProdutoEnum.Produto : PlanoProdutoEnum.Plano,
                                                       Id_Plano_Produto = p.Plano == null ? (p.Produto.Id) : (p.Plano.Id),

                                                       Id = p.Id


                                                   }).ToList(),
                //.Status = obj.Status == Status_Cliente.Ativo ? true : false

                Status = obj?.Status.Value == Status_Default.Ativo ? true : false,
                Sla_PlanoProduto = obj?.Sla_PlanoProduto == null ? false : (obj?.Sla_PlanoProduto.Value == Status_Default.Ativo ? true : false),
            };
        }

        private void BindViewToObj(ISession session, ref SLA_Atendimento objBanco, SLA_AtendimentoCadastroModel view)
        {
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))
            using (var repoTipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repoCategoria_Cliente = new Conexao<Categoria_Cliente>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Tipo_Atendimento = repoTipo_Atendimento.selectFirst(p => p.Id == view.Id_Tipo_Atendimento).Result;
                objBanco.Prioridade_Atendimento = view.Prioridade_Atendimento;

                if (view.Id_Categoria_Cliente == null)
                    objBanco.Categoria_Cliente = null;
                else
                    objBanco.Categoria_Cliente = repoCategoria_Cliente.selectFirst(p => p.Id == view.Id_Categoria_Cliente.Value).Result;

                objBanco.Minutos = IntService.GetMinutos(view.Horas);
                objBanco.Id_Horario = view.Id_Horario;
                objBanco.Contagem_Prazo = view.Contagem_Prazo;
                objBanco.Sla_PlanoProduto = view.Sla_PlanoProduto ? Status_Default.Ativo : Status_Default.Inativo;
                if (objBanco.Id != 0)
                {
                    objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                }

                List<SLA_Atendimento_PlanoProduto> planosProdutosTmp = objBanco.PlanosProdutos.ToList();
                foreach (var planoproduto in view.PlanosProdutos.FindAll(p => p.Id == 0))
                {
                    planosProdutosTmp.Add(new SLA_Atendimento_PlanoProduto
                    {
                        SLA_Atendimento = objBanco,
                        Plano = planoproduto.Tipo == PlanoProdutoEnum.Plano ? repoPlano.selectFirst(p => p.Id == planoproduto.Id_Plano_Produto).Result : null,
                        Produto = planoproduto.Tipo == PlanoProdutoEnum.Produto ? repoProduto.selectFirst(p => p.Id == planoproduto.Id_Plano_Produto).Result : null,
                    });
                }
                objBanco.PlanosProdutos = planosProdutosTmp;
            }
        }

        [HttpPost()]
        [Route("api/SLA_Atendimentoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_SLA_ATENDIMENTO WHERE ID IN {_ids}");

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
        [Route("api/SLA_AtendimentoApi/FindById")]
        public async Task<GetApiRetorno<SLA_AtendimentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<SLA_AtendimentoCadastroModel> retorno = new GetApiRetorno<SLA_AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<SLA_Atendimento>(session))
            {
                try
                {
                    retorno.Objeto = await GetViewObj(await repository.selectFirst(p => p.Id == Id));
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
        [Route("api/SLA_AtendimentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<SLA_AtendimentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {

                SLA_Atendimento modelo = null;
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<SLA_Atendimento>(session))
                    {
                        if (value.Objeto.Id_Categoria_Cliente == null && value.Objeto.Prioridade_Atendimento == Prioridade_Atendimento.Indiferente)
                        {
                            if (value.Objeto.Status == true || value.Objeto.Id == 0)
                            {
                                var sla_cadastrados = await repository.select(p => p.Tipo_Atendimento.Id == value.Objeto.Id_Tipo_Atendimento && p.Status == Status_Default.Ativo);
                                if (sla_cadastrados.Count() > 0)
                                {
                                    throw new Exception("Não é possível criar/ativar um SLA com prioridade e categoria indiferente para este tipo de atendimento, pois já existem outros SLAs ativos");
                                }
                            }
                        }


                        if (value.Objeto.Status == true || value.Objeto.Id == 0)
                        {
                            var obj2 = await repository.select(p => p.Categoria_Cliente == null &&
                            p.Tipo_Atendimento.Id == value.Objeto.Id_Tipo_Atendimento && p.Prioridade_Atendimento == Prioridade_Atendimento.Indiferente && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo);
                            if (obj2.Count > 0)
                            {
                                throw new Exception("Este tipo de atendimento já possui um SLA com prioridade e categoria de cliente indiferente ativo.Não é possível criar outro.");
                            }
                        }

                        if (value.Objeto.Status == true || value.Objeto.Id == 0)
                        {
                            var obj = await repository.select(p => p.Categoria_Cliente.Id == value.Objeto.Id_Categoria_Cliente &&
                           p.Tipo_Atendimento.Id == value.Objeto.Id_Tipo_Atendimento && p.Prioridade_Atendimento == value.Objeto.Prioridade_Atendimento && p.Id != value.Objeto.Id && p.Status == Status_Default.Ativo);
                            if (obj.Count > 0)
                            {
                                throw new Exception("Ja possui um SLA de Atendimento com essa configuração");
                            }
                        }


                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new SLA_Atendimento
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);


                        await repository.SaveOrUpdate(modelo);
                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        [HttpPost()]
        [Route("api/SLA_AtendimentoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_SLA_ATENDIMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [Route("api/SLA_AtendimentoApi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<SLA_Atendimento_PlanoProduto>(session))
                    {

                        SLA_Atendimento_PlanoProduto modelo = null;
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
        [Route("api/SLA_AtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<SLA_Atendimento>(session))
                    {

                        SLA_Atendimento modelo = null;
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
