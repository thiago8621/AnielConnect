using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Padrao = Sinapse.Model.enumerados.Padrao;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Modelo_PainelServicoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Modelo_PainelServicoApi/GetAll")]
        public async Task<DataTableResponse<Modelo_PainelServicoListaModel>> GetAll(Modelo_PainelServico_DataTableRequest request)
        {
            List<Modelo_PainelServicoListaModel> retorno = new List<Modelo_PainelServicoListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_MODELO_PAINEL_SERVICO A");

                if (!request.AdministradorModelos)
                {
                    sqlFrom.AppendLine("WHERE ((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER))");
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                    else
                        sqlFrom.AppendLine(" WHERE (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");

                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                parametros.Add("@USER", User.Identity.GetUserId());

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.STATUS,A.VISIBILIDADE,A.COLUNA_ORDENAR, A.PADRAO, ");
                    sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Visibilidade>("A.VISIBILIDADE", "VISIBILIDADE_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Coluna_PainelServicoEnum>("A.COLUNA_ORDENAR", "ORDENAR_POR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Aniel.T5.Model.enumerado.Padrao>("A.PADRAO", "PADRAO"));
                    sql.AppendLine(sqlFrom.ToString());

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
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR DESC");
                                    break;
                                }
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY COLUNA_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY COLUNA_ORDENAR DESC");
                                    break;
                                }
                            case "ordenar_Por":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY ORDENAR_POR");
                                    else
                                        sql.AppendLine("ORDER BY ORDENAR_POR DESC");
                                    break;
                                }
                            case "visibilidade":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY VISIBILIDADE_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY VISIBILIDADE_ORDENAR DESC");
                                    break;
                                }
                            case "padrao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY PADRAO");
                                    else
                                        sql.AppendLine("ORDER BY PADRAO DESC");
                                    break;
                                }
                            case "id":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.ID");
                                    else
                                        sql.AppendLine("ORDER BY A.ID DESC");
                                    break;
                                }
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Modelo_PainelServicoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Visibilidade = HtmlEnumExtensions.GetEnumDescription<Visibilidade>(Convert.ToString(reader["VISIBILIDADE"])),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Ordenar_Por = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelServicoEnum>(Convert.ToString(reader["COLUNA_ORDENAR"])),
                                Padrao = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["PADRAO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }
                }
            }

            return new DataTableResponse<Modelo_PainelServicoListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno.ToList(),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Modelo_PainelServicoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Modelo_PainelServicoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            Modelo_PainelServico modelo = null;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelServico>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Modelo_PainelServico
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

                        BindViewToObj(session, modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);
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

        [HttpGet()]
        [Route("api/Modelo_PainelServicoApi/FindById")]
        public async Task<GetApiRetorno<Modelo_PainelServicoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_PainelServicoCadastroModel> retorno = new GetApiRetorno<Modelo_PainelServicoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelServico>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id), session);
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

        [Route("api/Modelo_PainelAtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelServico>(session))
                    {

                        Modelo_PainelServico modelo = null;
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

        [HttpGet()]
        [Route("api/Modelo_PainelServicoApi/ExcluirGridFiltro")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridFiltro(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Filtro_PainelServico>(session))
                    {

                        Filtro_PainelServico modelo = null;
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

        [Route("api/Modelo_PainelServicoApi/ExcluirGridColuna")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridColuna(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Coluna_PainelServico>(session))
                    {

                        Coluna_PainelServico modelo = null;
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
        [Route("api/Modelo_PainelServicoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_SERVICO SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/Modelo_PainelServicoApi/tornarpublico")]
        public async Task<PostApiRetorno<Int64>> TornarPublico(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_SERVICO SET VISIBILIDADE = 1 WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/Modelo_PainelServicoApi/GetCombo")]
        public async Task<ObjComboOptGroup<Int64, String>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjComboOptGroup<Int64, String> retorno = new ObjComboOptGroup<Int64, String>
            {
                results = new List<IObjComboOptGroupResult<Int64, String>>()
            };

            using (var acessoApi = new AcessoApiController())
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    List<AreaModuloModel> areas = acessoApi.GetArea_ModuloUsuario(request.id_usuario).Result.Objeto;

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_MODELO_PAINEL_SERVICO A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id_usuario))
                    {
                        // Caso o usuário não possua a área de acesso 170 ele só poderá ver modelos de painel público ou
                        // modelos privados criados por ele mesmo
                        if (areas.Where(a => a.Cod_Area == "170").Count() == 0)
                        {
                            condicoes.Add("((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER) OR (A.VISIBILIDADE = 1) )");
                            parametros.Add("@USER", request.id_usuario);
                        }
                    }
                    else
                    {
                        condicoes.Add("((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER) OR (A.VISIBILIDADE = 1) )");
                        parametros.Add("@USER", request.id_usuario);
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }
                    else
                    {
                        if (request.soativos)
                        {
                            condicoes.Add("A.STATUS = 1");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                        }
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
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

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.VISIBILIDADE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.VISIBILIDADE, A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                String cod_grupo = HtmlEnumExtensions.GetEnumDescription<Visibilidade>(Convert.ToString(reader["VISIBILIDADE"]));
                                var grupo_result = retorno.results.Find(res => res.Id_Grupo == cod_grupo);

                                areas = acessoApi.GetArea_ModuloUsuario(User.Identity.GetUserId()).Result.Objeto;

                                var option = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                };

                                if (grupo_result == null)
                                {
                                    retorno.results.Add(new ObjComboOptGroupResult<Int64, String>
                                    {
                                        Id_Grupo = cod_grupo,
                                        Text = cod_grupo,
                                        Children = new List<IObjComboResult<long>> { option }
                                    });
                                }
                                else
                                {
                                    grupo_result.Children.Add(option);
                                }
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

        [HttpGet()]
        [Route("api/Modelo_PainelServicoApi/GetColunas")]
        public async Task<GetApiRetorno<GetColunas_Model>> GetColunas(Int64 id_modelo)
        {
            GetApiRetorno<GetColunas_Model> retorno = new GetApiRetorno<GetColunas_Model>();
            retorno.Objeto = new GetColunas_Model();
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelServico>(session))
            {
                try
                {
                    var modelo = await repository.selectFirst(p => p.Id == id_modelo);
                    retorno.Objeto.colunas = modelo.Colunas.Select(p => Convert.ToInt32(p.Coluna)).ToList();
                    retorno.Objeto.ordenar_por = Convert.ToInt32(modelo.Coluna_Ordenar);
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        Modelo_PainelServicoCadastroModel GetViewObj(Modelo_PainelServico obj, ISession session)
        {
            using (var repository = new Conexao<Modelo_PainelServico>(session))
            {
                return new Modelo_PainelServicoCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,
                    Status = obj.Status == Status_Default.Ativo,
                    Visibilidade = obj.Visibilidade,
                    Coluna_Ordenar = obj.Coluna_Ordenar,
                    Padrao = obj.Padrao,
                    Filtros = obj.Filtros.Select(p => getFiltroViewObj(p, session)).ToList(),

                    Colunas = obj.Colunas.Select(p => new Coluna_PainelServicoModel
                    {
                        Coluna = p.Coluna,
                        Descricao = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelServicoEnum>(p.Coluna),
                        Id = p.Id
                    }).ToList()

                };
            }
        }

        private void BindViewToObj(ISession session, Modelo_PainelServico objBanco, Modelo_PainelServicoCadastroModel view)
        {
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Visibilidade = view.Visibilidade;
                objBanco.Coluna_Ordenar = view.Coluna_Ordenar;
                objBanco.Padrao = view.Padrao;
                objBanco.Filtros = view.Filtros.Where(p => p.Id == 0).Select(p =>
                {
                    var retorno = new Filtro_PainelServico
                    {
                        Modelo_PainelServico = objBanco,
                        Filtro = p.Filtro,
                        Valor = p.Valor,
                        ValorPai = p.ValorPai,
                        ValorAvo = p.ValorAvo
                    };

                    if (p.Filtro == Filtro_PainelServicoEnum.DataAgendamento || p.Filtro == Filtro_PainelServicoEnum.DataCriacao || p.Filtro == Filtro_PainelServicoEnum.DataEncerramento)
                        retorno.Data = (ModeloPainel_Data)Convert.ToInt32(p.Valor);

                    return retorno;

                }).ToList();

                objBanco.Colunas = view.Colunas.Where(p => p.Id == 0).Select(p => new Coluna_PainelServico
                {
                    Modelo_PainelServico = objBanco,
                    Coluna = p.Coluna
                }).ToList();
            }
        }

        private Filtro_PainelServicoModel getFiltroViewObj(Filtro_PainelServico filtro, ISession session)
        {
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (var repository_tipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repository_Natureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            {

                Filtro_PainelServicoModel viewobj = new Filtro_PainelServicoModel();
                viewobj.Id = filtro.Id;
                viewobj.Filtro = filtro.Filtro;
                viewobj.Filtro_Descricao = HtmlEnumExtensions.GetEnumDescription<Filtro_PainelServicoEnum>(filtro.Filtro);
                viewobj.Valor = filtro.Valor;
                viewobj.ValorPai = filtro.ValorPai;


                switch (filtro.Filtro)
                {
                    case Filtro_PainelServicoEnum.Tipo_Servico:
                        var id_tipo_servico = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = repository_tipoServico.selectFirst(p => p.Cod_Tipo_Serv == id_tipo_servico).Result.Descricao;
                        break;
                    case Filtro_PainelServicoEnum.Sub_Tipo_Servico:
                        using (var repository_subTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
                        {
                            var id_subtipo_servico = Convert.ToString(filtro.Valor);
                            var tipo_servico = Convert.ToInt16(filtro.ValorPai);
                            viewobj.Descricao = repository_subTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == id_subtipo_servico && p.Codigo_Tipo == tipo_servico).Result.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Natureza_Producao:
                        var id_natureza = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = repository_Natureza.selectFirst(p => p.Id == id_natureza).Result.Descricao;
                        break;
                    case Filtro_PainelServicoEnum.Status:
                        var status = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>((StatusExecucaoAniel)Convert.ToInt32(filtro.Valor));
                        break;

                    case Filtro_PainelServicoEnum.Tipo_Imovel:
                        using (var repository_TipoImovel = new Conexao<Tipo_Imovel>(sessionT5))
                        {
                            var id_tipo_imovel = Convert.ToInt64(filtro.Valor);
                            viewobj.Descricao = repository_TipoImovel.selectFirst(p => p.Id == id_tipo_imovel).Result.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Categoria_Cliente:
                        using (var repository_categoria = new Conexao<Categoria_ClienteAniel>(sessionT5))
                        {
                            var id_categoria = Convert.ToInt64(filtro.Valor);
                            viewobj.Descricao = repository_categoria.selectFirst(p => p.Id == id_categoria).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.UF:
                        viewobj.Descricao = filtro.Valor;
                        break;
                    case Filtro_PainelServicoEnum.Cidade:
                        using (var repository_cidade = new Conexao<Cidade>(sessionConnect))
                        {
                            viewobj.Descricao = filtro.Valor;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Contrato:
                        using (var repository_contrato = new Conexao<ContratoAniel>(sessionT5))
                        {
                            var contrato = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_contrato.selectFirst(p => p.CodCt == contrato).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Projeto:
                        using (var repository_projeto = new Conexao<ProjetoAniel>(sessionT5))
                        {
                            var projeto = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_projeto.selectFirst(p => p.Codigo == projeto && p.CodCt == filtro.ValorPai).Result?.Codigo;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Encerramento:
                        using (var repository_encerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5))
                        {
                            var encerramento = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_encerramento.selectFirst(p => p.Cod_Encerramento == encerramento && p.CodCt == filtro.ValorPai).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.UsuarioAgendamento:
                        using (var repository_usuario_agendamento = new Conexao<IdentityUser>(sessionConnect))
                        {
                            var usuario_agendamento = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_usuario_agendamento.selectFirst(p => p.Id == usuario_agendamento).Result?.Nome;
                        }
                        break;
                    case Filtro_PainelServicoEnum.UsuarioAbertura:
                        using (var repository_usuario_abertura = new Conexao<IdentityUser>(sessionConnect))
                        {
                            var usuario_abertura = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_usuario_abertura.selectFirst(p => p.Id == usuario_abertura).Result?.Nome ?? usuario_abertura;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Responsavel:
                        using (var repository_equipe = new Conexao<EquipeAniel>(sessionT5))
                        {
                            var equipe = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_equipe.selectFirst(p => p.Matricula == equipe).Result?.Nome;
                        }
                        break;
                    case Filtro_PainelServicoEnum.DataAgendamento:

                        var dataAgendamento = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Data));

                        break;
                    case Filtro_PainelServicoEnum.DataCriacao:

                        var dataCriacao = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Valor));

                        break;
                    case Filtro_PainelServicoEnum.DataEncerramento:

                        var dataEncerramento = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Valor));

                        break;
                    case Filtro_PainelServicoEnum.GrupoArea:
                        using (var repository_escritorio = new Conexao<EscritorioAniel>(sessionT5))
                        {
                            viewobj.Descricao = repository_escritorio.selectFirst(p => p.Cod_Escritorio == filtro.Valor).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Area:
                        using (var repository_area = new Conexao<AreaAniel>(sessionT5))
                        {
                            var equipe = Convert.ToString(filtro.Valor);
                            viewobj.Descricao = repository_area.selectFirst(p => p.Cod_Area == filtro.Valor && p.Cod_Escritorio == filtro.ValorPai).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Localidade:
                        using (var repository_localidade = new Conexao<LocalidadeAniel>(sessionT5))
                        {
                            viewobj.Descricao = repository_localidade.selectFirst(p => p.Cod_Localidade == Convert.ToInt32(filtro.Valor) && p.Cod_Area == filtro.ValorPai && p.Cod_Escritorio == filtro.ValorAvo).Result?.LocalidadeStr;
                        }
                        break;
                    case Filtro_PainelServicoEnum.PlanoProduto:
                        using (var repository_plano_produto = new Conexao<PlanoProdutoAniel>(sessionT5))
                        {
                            viewobj.Descricao = repository_plano_produto.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Empresa:
                        using (var repository_plano_produto = new Conexao<EmpresaAniel>(sessionT5))
                        {
                            viewobj.Descricao = repository_plano_produto.selectFirst(p => p.CodEmpresa == Convert.ToInt64(filtro.Valor)).Result?.Razao_Social;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Monitor_Tecnico:
                        using (var repository_usuario_abertura = new Conexao<IdentityUser>(sessionConnect))
                        {
                            viewobj.Descricao = repository_usuario_abertura.selectFirst(p => p.Id == filtro.Valor).Result?.Nome;
                        }
                        break;
                    case Filtro_PainelServicoEnum.Filial:
                        using (var filialApi = new FilialApiController())
                        {
                            viewobj.Descricao = filialApi.GetComboFilialAniel(new RequestObjComboPai<string, string> { id = filtro.Valor, idpai = filtro.ValorPai }).Result.results.FirstOrDefault()?.Text;
                        }
                        break;
                    case Filtro_PainelServicoEnum.SLA:
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Status_SLA_Servico>((Status_SLA_Servico)Convert.ToInt32(filtro.Valor));
                        break;

                }

                return viewobj;
            }
        }


        public static String ViewModeloPainelData(ModeloPainel_Data data_enum)
        {

            switch (data_enum)
            {
                case ModeloPainel_Data.DIA_ATUAL:
                    return $"{DateTime.Now.ToString("dd.MM.yyyy")}";
                case ModeloPainel_Data.MES_ATUAL:
                    return $"{DateTime.Now.ToString("01.MM.yyyy")} {RsBase.ATE} {DateTime.Now.ToString("dd.MM.yyyy")}";
                case ModeloPainel_Data.SEMANA_ATUAL:
                    return $"{DateTimeUtil.GetLastDayofTheWeek(DayOfWeek.Monday).ToString("dd.MM.yyyy")} {RsBase.ATE} {DateTime.Now.ToString("dd.MM.yyyy")}";
                case ModeloPainel_Data.ULTIMOS_30_DIAS:
                    return $"{DateTime.Now.AddDays(-30).ToString("dd.MM.yyyy")} {RsBase.ATE} {DateTime.Now.ToString("dd.MM.yyyy")}";
                case ModeloPainel_Data.ULTIMOS_3_MESES:
                    return $"{DateTime.Now.AddDays(-90).ToString("dd.MM.yyyy")} {RsBase.ATE} {DateTime.Now.ToString("dd.MM.yyyy")}";
                case ModeloPainel_Data.ULTIMOS_7_DIAS:
                    return $"{DateTime.Now.AddDays(-7).ToString("dd.MM.yyyy")} {RsBase.ATE} {DateTime.Now.ToString("dd.MM.yyyy")}";
            }
            return null;
        }

    }
}
