using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
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
    public class Cobranca_ServicoApiController : BaseApiController
    {
        [HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/Cobranca_Servicoapi/GetAll")]
        public async Task<DataTableResponse<Cobranca_ServicoListaModel>> GetAll(Cobranca_Servico_DataTableRequest request)
        {
            List<Cobranca_ServicoListaModel> retorno = new List<Cobranca_ServicoListaModel>();
            IOrderedEnumerable<Cobranca_ServicoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder("* from TB_COBRANCA_SERVICO A");


                //Dictionary<String, Object> parametros = new Dictionary<string, object>();
                //GetAllFiltros(request, sqlFrom, parametros);
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_COBRANCA_SERVICO");


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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.ESCOLHA_COBRANCA,A.STATUS ");
                    sql.AppendLine("FROM TB_COBRANCA_SERVICO A");
                    sql.Append("LEFT JOIN TB_TIPO_SERVICO_COBRANCA B ");
                    sql.Append("ON B.ID_COBRANCA_SERVICO =A.ID ");
                    sql.Append("LEFT  JOIN TB_PLANO_PRODUTO_COBRANCA C ");
                    sql.Append("ON C.ID_COBRANCA_SERVICO =A.ID ");
                    sql.AppendLine("WHERE A.ID>0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        //sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }
                    if (!String.IsNullOrEmpty(request.Id_Segmento))
                    {
                        sql.AppendLine("AND B.SEGMENTO = @SEGMENTO");
                        parametros.Add("@SEGMENTO", request.Id_Segmento);
                    }

                    if (!String.IsNullOrEmpty(request.Id_Tipo_Servico))
                    {
                        sql.AppendLine("AND B.TIPO_SERVICO = @TIPO_SERVICO");
                        parametros.Add("@TIPO_SERVICO", request.Id_Tipo_Servico);
                    }
                    if (!String.IsNullOrEmpty(request.Id_Sub_Tipo_Servico))
                    {
                        sql.AppendLine("AND B.SUB_TIPO_SERVICO = @SUB_TIPO_SERVICO");
                        parametros.Add("@SUB_TIPO_SERVICO", request.Id_Sub_Tipo_Servico);
                    }
                    if (!String.IsNullOrEmpty(request.Id_Cod_Encerramento))
                    {
                        sql.AppendLine("AND B.COD_ENCERRAMENTO = @COD_ENCERRAMENTO");
                        parametros.Add("@COD_ENCERRAMENTO", request.Id_Cod_Encerramento);
                    }

                    if (request.Id_Plano_Produto.HasValue)
                    {
                        sql.AppendLine(" AND C.ID_PLANO = @PLANO_PRODUTO OR ID_PRODUTO = @PLANO_PRODUTO  ");
                        parametros.Add("@PLANO_PRODUTO", request.Id_Plano_Produto);
                    }

                    //if (request.Id_Plano_Produto.HasValue)
                    //{
                    //    sql.AppendLine(" AND C.ID_PRODUTO = @PLANO ");
                    //    parametros.Add("@PLANO", request.Id_Plano_Produto);
                    //}



                    sql.AppendLine("GROUP BY A.ID,A.DESCRICAO,A.ESCOLHA_COBRANCA,A.STATUS ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Cobranca_ServicoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Escolha_Cobranca = Convert.ToString(reader["ESCOLHA_COBRANCA"]),
                                //Segmento = Convert.ToString(reader["SEGMENTO"]),
                                //Tipo_Servico = Convert.ToString(reader["TIPO_SERVICO"]),
                                //Cod_Encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                //Plano_Produto = Convert.ToString(reader["PLANO_PRODUTO"]),
                                ////Valor_Cobrado = reader["VALORO"] != DBNull.Value ? Convert.ToDouble(reader["VALORPLANO"]).ToString("N2") : "0.00",
                                //Sub_Tipo_Servico = Convert.ToString(reader["SUB_TIPO_SERVICO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            };

                            retorno.Add(obj);
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

                    case 1:     // DESCIRCAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }

                    case 2:     // ECOLHA_COBRANCA
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Escolha_Cobranca);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Escolha_Cobranca);
                            break;
                        }
                    case 3:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                        //case 4:     // TipoServico
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.TipoServico);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.TipoServico);
                        //        break;
                        //    }
                        //case 5:     // Segmento
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.Segmento);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.Segmento);
                        //        break;
                        //    }
                        //case 6:     // Cod_Encerramento
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.Cod_Encerramento);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.Cod_Encerramento);
                        //        break;
                        //    }
                        //case 7:     // Cod_Encerramento
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.Sub_Tipo_Servico);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.Sub_Tipo_Servico);
                        //        break;
                        //    }
                        //case 8:     // Cod_Encerramento
                        //    {
                        //        if (sortDirection == "asc")
                        //            listaOrdenada = retorno.OrderBy(p => p.Plano_Produto);
                        //        else
                        //            listaOrdenada = retorno.OrderByDescending(p => p.Plano_Produto);
                        //        break;
                        //    }
                }



            }



            //Page e retornando DataTable
            return new DataTableResponse<Cobranca_ServicoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Cobranca_ServicoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Cobranca_ServicoListaModel>> retorno = new GetApiRetorno<IEnumerable<Cobranca_ServicoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Atendimento = new Conexao<Cobranca_Servico>(session))
            {
                try
                {
                    var lista = await repoTipo_Atendimento.select(p => p.Status == Status_Default.Ativo);

                    var listaRetorno = new List<Cobranca_ServicoListaModel>();

                    foreach (var obj in lista)
                    {
                        Cobranca_ServicoListaModel tmp = new Cobranca_ServicoListaModel
                        {
                            Id = obj.Id,
                            Descricao = obj.Descricao,
                            //Escolha_Cobranca = obj.Escolha_Cobranca,

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


        private async Task BindViewToObj(ISession session, Cobranca_Servico objBanco, Cobranca_ServicoCadastroModel view)
        {
            using (var repository = new Conexao<Cobranca_Servico>(session))
            {
                objBanco.Descricao = view.Descricao;
            }
        }

        [HttpPost()]
        [Route("api/Cobranca_Servicoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_COBRANCA_SERVICO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Cobranca_Servicoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_TIPO_SERVICO_COBRANCA WHERE ID IN {_ids}");

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

        [Route("api/Cobranca_Servicoapi/ExcluirPlanoCobGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirPlanoGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Plano_Produto_Cobranca>(session))
                    {

                        Plano_Produto_Cobranca modelo = null;
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

        [Route("api/Cobranca_Servicoapi/ExcluirTipoCobGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirTipoCobGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<TipoServico_Cobranca>(session))
                    {

                        TipoServico_Cobranca modelo = null;
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


        [Route("api/Cobranca_Servicoapi/ConsultaTipoServico")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> ConsultaTipoServico(ObjetoTipoServico value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<TipoServico_Cobranca>(session))
                    {
                        TipoServico_Cobranca modelo = null;

                        var lista = await repository.select(x => x.Segmento == value.Id_Segmento && x.Tipo_Servico == value.Id_Tipo_Servico &&
                                                            (x.Sub_Tipo_Servico == value.Id_Sub_Tipo_Servico || x.TodasSubTipoServico == true) &&
                                                            (x.Cod_Encerramento == value.Id_Cod_Encerramento || x.TodasCod_Encerramento == true));
                        lista.ToList().ForEach(x =>
                        {
                            if (x.Cobranca_Servico.Status == Status_Default.Ativo)
                            {
                                modelo = x;
                            }
                        });

                        if (modelo != null)
                        {
                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                            retorno.Mensagem = @$"Já existe um registro com essas informações - {modelo.Cobranca_Servico.Descricao}.";
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
        [Route("api/Cobranca_ServicoApi/FindById")]
        public async Task<GetApiRetorno<Cobranca_ServicoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Cobranca_ServicoCadastroModel> retorno = new GetApiRetorno<Cobranca_ServicoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Cobranca_Servico>(session))
            using (var repositoryTipoServico = new Conexao<TipoServico_Cobranca>(session))
            using (var repositoryPlanoProduto = new Conexao<Plano_Produto_Cobranca>(session))
            using (var repositorySegmento = new Conexao<ContratoAniel>(sessionT5))
            using (var repositoryTipo_Servico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repositoryCodigoEncerramento = new Conexao<Codigo_EncerramentoAniel>(sessionT5))
            using (var repositorySub_Tipo_Servico = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
            using (var repositoryPlano = new Conexao<Plano>(session))
            using (var repositoryProduto = new Conexao<Produto>(session))


            {
                try
                {
                    retorno.Objeto = GetViewObj(obj: await repository.selectFirst(p => p.Id == Id));
                    var cobrancaServico = await repositoryTipoServico.select(p => p.Cobranca_Servico.Id == Id);

                    retorno.Objeto.TipoServico = cobrancaServico.Select(p =>
                    {
                        var tipoServico = repositoryTipoServico.select(y => y.Cobranca_Servico.Id == p.Id).Result;
                        var cobrancaServico = repository.selectFirst(y => y.Id == p.Cobranca_Servico.Id).Result;
                        var obj = new Cobranca_ServicoTipoServicoCadastroModel();
                        obj.Id = p.Id;
                        obj.Segmento = p.Segmento;
                        obj.Segmento_Desc = String.IsNullOrEmpty(p.Segmento) ? String.Empty : repositorySegmento.selectFirst(y => y.CodCt == p.Segmento).Result?.Descricao;
                        obj.Cod_Encerramento = p.Cod_Encerramento;
                        obj.Desc_Cod_Encerramento = String.IsNullOrEmpty(p.Cod_Encerramento) ? String.Empty : repositoryCodigoEncerramento.selectFirst(x => x.Cod_Encerramento == p.Cod_Encerramento).Result?.Descricao;
                        obj.Tipo_Servico = p.Tipo_Servico;
                        obj.Desc_Tipo_Servico = String.IsNullOrEmpty(p.Tipo_Servico) ? String.Empty : repositoryTipo_Servico.selectFirst(m => m.Cod_Tipo_Serv == Convert.ToInt16(p.Tipo_Servico)).Result?.Descricao;
                        obj.Sub_Tipo_Servico = p.Sub_Tipo_Servico;
                        obj.Desc_Sub_Tipo_Servico = String.IsNullOrEmpty(p.Sub_Tipo_Servico) ? String.Empty : repositorySub_Tipo_Servico.selectFirst(z => z.Codigo_Sub_Tipo == p.Sub_Tipo_Servico && z.Codigo_Tipo == Convert.ToInt16(p.Tipo_Servico)).Result?.Descricao;
                        obj.TodasSubTipoServico = p.TodasSubTipoServico;
                        obj.TodasCod_Encerramento = p.TodasCod_Encerramento;
                        obj.Cod_Cobranca = Convert.ToString(cobrancaServico?.Escolha_Cobranca);
                        obj.Desc_Forma_Cobranca = HtmlEnumExtensions.GetEnumDescription<Escolha_Cobranca>(Convert.ToString(cobrancaServico?.Escolha_Cobranca));

                        return obj;

                    }).ToList();

                    var planoProduto = await repositoryPlanoProduto.select(y => y.Cobranca_Servico.Id == Id);
                    retorno.Objeto.PlanoProduto = planoProduto.Select(y =>
                    {
                        var objpalno = new Cobranca_ServicoPlanoProdutoCadastroModel();
                        objpalno.Id = y.Id;
                        objpalno.Plano_Produto = Convert.ToString(y.Plano?.Id ?? y.Produto?.Id);
                        objpalno.Desc_Plano_Produto = y.Plano?.Descricao ?? y.Produto?.Descricao;

                        var valor = (y.Plano?.ValorPlano ?? y.Produto?.ValorProduto);
                        objpalno.Valor_Cobrado = valor.HasValue ? valor.Value.ToString("N2") : null;
                        return objpalno;

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
        [Route("api/Cobranca_ServicoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Cobranca_ServicoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Cobranca_Servico modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cobranca_Servico>(session))
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var repositoryEncerramentoAtendimento = new Conexao<EncerramentoAtendimento>(session))
                    using (var repositoryTipoServico = new Conexao<TipoServico_Cobranca>(session))
                    using (var repositoryPlanoProduto = new Conexao<Plano_Produto_Cobranca>(session))
                    using (var repositoryTipo_Servico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                    using (var repositorySub_Tipo_Servico = new Conexao<Sub_Tipo_ServicoAniel>(sessionT5))
                    using (var repositoryPlano = new Conexao<Plano>(session))
                    using (var repositoryProduto = new Conexao<Produto>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Cobranca_Servico
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

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                        foreach (var servico in value.Objeto.TipoServico)
                        {
                            if (!String.IsNullOrEmpty(servico.Cod_Cobranca))
                            {
                                var escolha = Convert.ToInt32(servico.Cod_Cobranca);
                                modelo.Escolha_Cobranca = (Escolha_Cobranca)escolha;
                                await repository.SaveOrUpdate(modelo);
                            }

                            var tipo = new TipoServico_Cobranca
                            {
                                Id = servico.Id,
                                Segmento = servico.Segmento,
                                Cod_Encerramento = servico.Cod_Encerramento,
                                Tipo_Servico = servico.Tipo_Servico,
                                Sub_Tipo_Servico = servico.Sub_Tipo_Servico,
                                TodasSubTipoServico = string.IsNullOrEmpty(servico.Sub_Tipo_Servico) ? true : false,
                                TodasCod_Encerramento = string.IsNullOrEmpty(servico.Cod_Encerramento) ? true : false,
                                Cobranca_Servico = modelo,
                            };

                            await repositoryTipoServico.SaveOrUpdate(tipo);
                        }

                        await repository.SaveOrUpdate(modelo);
                        foreach (var plano in value.Objeto.PlanoProduto)
                        {
                            var produto = new Plano_Produto_Cobranca
                            {
                                Id = plano.Id,
                                Plano = await repositoryPlano.selectFirst(p => p.Id == Convert.ToInt64(plano.Plano_Produto)),
                                Produto = await repositoryProduto.selectFirst(p => p.Id == Convert.ToInt64(plano.Plano_Produto)),
                                Cobranca_Servico = modelo


                            };

                            await repositoryPlanoProduto.SaveOrUpdate(produto);

                        }
                        await repository.SaveOrUpdate(modelo);


                        retorno.IdObjeto = modelo.Id;
                        retorno.Ok = true;
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

        [Route("api/Cobranca_ServicoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<TipoServico_Cobranca>(session))
                    {

                        TipoServico_Cobranca modelo = null;
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
        [Route("api/Cobranca_Servicoapi/busca")]
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine("FROM TB_COBRANCA_SERVICO A");
                        sql.AppendLine("WHERE (UPPER(A.DESCRICAO) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
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
        [Route("api/Cobranca_Servicoapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_COBRANCA_SERVICO A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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

        [HttpGet]
        [Route("api/Cobranca_Servicoapi/VerificaPagamentoInstalacao")]
        public async Task<bool> VerificaPagamentoInstalacao(string idPlanoProduto)
        {
            try
            {
                using ISession session = HibernateUtil.GetSession();
                using var repoPlano = new Conexao<Plano>(session);

                var plano = await repoPlano.selectFirst(x => x.Id == Convert.ToInt64(idPlanoProduto));

                if (plano?.CobrancaInstalacao == false)
                {
                    return false;
                }

                var sql = new StringBuilder();

                sql.AppendLine(" SELECT 1 AS POSSUI_CR_ABERTO ");
                sql.AppendLine(" FROM TB_FATURAMENTO_CLIENTE_PLANO A ");
                sql.AppendLine(" LEFT JOIN TB_FATURAMENTO_CLIENTE B ON(A.ID_FATURAMENTO_CLIENTE = B.ID) ");
                sql.AppendLine(" LEFT JOIN TB_CONTA_RECEBER C ON(C.ID_FATURAMENTO_CLIENTE = B.ID) ");
                sql.AppendLine(" LEFT JOIN TB_FATURAMENTO_FORMA_PAGAMENTO D  ON(D.ID = B.ID_FATURAMENTO_FORMA) ");
                sql.AppendLine(" LEFT JOIN TB_FATURAMENTO_PROTOCOLO E ON(E.ID = D.ID_FATURAMENTO_PROTOCOLO) ");
                sql.AppendLine(" WHERE A.ID_PLANO_PRODUTO = @ID_PLANO_PRODUTO ");
                sql.AppendLine(" AND E.COBRAR_INSTALACAO = 1 ");
                sql.AppendLine(" AND C.STATUS <> '1'");

                var parametros = new Dictionary<string, object> { { "ID_PLANO_PRODUTO", idPlanoProduto } };

                var retorno = new List<CobrancaInstalacaoList>();
                bool possuiContaAberto = false;

                using (var command = session.Connection.CreateCommand())
                using (var reader = await CommandHelper.ExecuteReader(command, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        possuiContaAberto = reader["POSSUI_CR_ABERTO"] != DBNull.Value ? Convert.ToBoolean(reader["POSSUI_CR_ABERTO"]) : false;
                    }
                }
             
                return possuiContaAberto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar pagamento de instalação para o plano/produto {idPlanoProduto}.", ex);
            }
        }

        [HttpGet]
        [Route("api/Cobranca_Servicoapi/VerificaTipoServico")]
        public async Task<bool> VerificaTipoServico(string idTipoServico)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoTipoServEquipe = new Conexao<Tipo_Servico_EquipeAniel>(session))
                {

                    var sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.EXIGE_PAGAMENTO_INSTALACAO AS EXIGE_PAG ");
                    sql.AppendLine(" FROM TB_TIPO_SERVICO_EQUIPE A ");
                    sql.AppendLine(" WHERE A.COD_TIPO_SERV = @CODIGO ");

                    var parametros = new Dictionary<string, object> { { "@CODIGO", idTipoServico } };

                    bool? exigePagInstalacao = null;

                    using (var command = session.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(command, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            exigePagInstalacao = reader["EXIGE_PAG"] != DBNull.Value ? Convert.ToChar(reader["EXIGE_PAG"]) == 'S' : false;
                        }
                    }

                    if (!exigePagInstalacao.HasValue) //n tem valor ou é nao exige pagamento
                        return false;

                    return exigePagInstalacao.Value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar Tipo do Serviço para a OS", ex);
            }
        }

        private Cobranca_ServicoCadastroModel GetViewObj(Cobranca_Servico obj)
        {
            return new Cobranca_ServicoCadastroModel
            {
                Id = obj.Id,

                Descricao = obj.Descricao,
                Escolha_Cobranca = obj?.Escolha_Cobranca,
                Status = obj.Status == Status_Default.Ativo,

            };
        }



    }
}