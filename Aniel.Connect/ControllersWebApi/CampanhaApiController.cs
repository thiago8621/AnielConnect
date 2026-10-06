using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.nhibernate.mongo;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Hql.Ast;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class CampanhaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Campanhaapi/GetAll")]
        public async Task<DataTableResponse<CampanhaListaModel>> GetAll(Campanha_DataTableRequest request)
        {

            List<CampanhaListaModel> retorno = new List<CampanhaListaModel>();
            IOrderedEnumerable<CampanhaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> Condicoes = new List<String>();
                StringBuilder sqlWhere = new StringBuilder();

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    Condicoes.Add($"((UPPER(A.TITULO) LIKE '%{request.search.value.ToUpper()}%')) ");
                }

                if (!String.IsNullOrEmpty(request.DataInicial) && !String.IsNullOrEmpty(request.DataFinal))
                {
                    Condicoes.Add("A.DATA_FINAL BETWEEN @pDATAINI AND @pDATAFIM");

                    parametros.Add("pDATAINI", Convert.ToDateTime(request.DataInicial).ToString("dd/MM/yyyy"));
                    parametros.Add("pDATAFIM", Convert.ToDateTime(request.DataFinal).ToString("dd/MM/yyyy"));
                }

                if (request.EquipeVendas.HasValue)
                {
                    Condicoes.Add("A.DEFINIR_CAMPANHA_EQUIPE_VENDA = @pEQUIPEVENDA");
                    parametros.Add("pEQUIPEVENDA", $"{Convert.ToInt32(request.EquipeVendas)}");
                }

                if (request.Regioes.HasValue)
                {
                    Condicoes.Add("A.DEFINIR_CAMPANHA_REGIOES = @pREGIOES");
                    parametros.Add("pREGIOES", $"{Convert.ToInt32(request.Regioes)}");
                }

                if (request.CampanhaTempoDeterminado.HasValue)
                {
                    Condicoes.Add("A.CAMP_TEMPO_DETERMINADO = @pTEMPODETERMINADO");
                    parametros.Add("pTEMPODETERMINADO", $"{Convert.ToInt32(request.CampanhaTempoDeterminado)}");
                }

                if (request.AreaCobertura.HasValue)
                {
                    Condicoes.Add("A.DEFINIR_AREA_COBERTURA = @pAREA");
                    parametros.Add("pAREA", $"{Convert.ToInt32(request.Regioes)}");
                }

                if (Condicoes.Count() > 0)
                {
                    sqlWhere.AppendLine($" WHERE {String.Join(" AND ", Condicoes)}");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    var sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_CAMPANHA A")
                        .AppendLine(sqlWhere.ToString()); //WHERE STATUS = 1  

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
                    var sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.TITULO, A.DATA_INICIO, A.DATA_FINAL, A.STATUS, A.DEFINIR_CAMPANHA_REGIOES, A.DEFINIR_AREA_COBERTURA, A.CAMP_TEMPO_DETERMINADO");
                    sql.AppendLine(" FROM TB_CAMPANHA A");

                    sql.AppendLine(String.Format("{0}", sqlWhere));


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            String enumDefault = Enum.GetName(typeof(Status_Default), Status_Default.Ativo);
                            retorno.Add(new CampanhaListaModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                //Titulo = Convert.ToString(reader["TITULO"]),
                                Titulo = reader["TITULO"] != DBNull.Value ? Convert.ToString(reader["TITULO"]) : String.Empty,
                                Data_Final = reader["DATA_FINAL"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_FINAL"]).ToString("dd/MM/yyyy") : String.Empty,
                                Data_Inicio = reader["DATA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INICIO"]).ToString("dd/MM/yyyy") : String.Empty,
                                //Meta = Convert.ToInt64(reader["META"]),
                                //Meta = reader["META"] != DBNull.Value ? Convert.ToInt64(reader["META"]) : 0,
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                Definir_Campanha_Regioes = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["DEFINIR_CAMPANHA_REGIOES"])) == enumDefault ? RsBase.SIM : RsBase.NAO,
                                Camp_Tempo_Determinado = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["CAMP_TEMPO_DETERMINADO"])) == enumDefault ? RsBase.SIM : RsBase.NAO,
                                Definir_Area_Cobertura = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["DEFINIR_AREA_COBERTURA"])) == enumDefault ? RsBase.SIM : RsBase.NAO,

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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Titulo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Titulo);
                            break;
                        }

                    case 2:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => Convert.ToDateTime(p.Data_Inicio));
                            else
                                listaOrdenada = retorno.OrderByDescending(p => Convert.ToDateTime(p.Data_Inicio));
                            break;
                        }

                    case 3:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => Convert.ToDateTime(p.Data_Final));
                            else
                                listaOrdenada = retorno.OrderByDescending(p => Convert.ToDateTime(p.Data_Final));
                            break;
                        }

                    case 4:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }



                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Titulo);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<CampanhaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private CampanhaCadastroModel GetViewObj(Campanha obj, ISession session, ISession sessionT5)
        {
            //Listagens Extras
            using (var repoEquipeVendas = new Conexao<Campanha_EquipeVendas>(session))
            using (var repoRegioes = new Conexao<Campanha_Regioes>(session))
            using (var repoArea_Cobertura = new Conexao<Campanha_AreaCobertura>(session))
            using (var repoPlanoProduto = new Conexao<Campanha_PlanoProduto>(session))
            using (var repoDepartamento = new Conexao<Departamento>(session))
            using (var repoUsuario = new Conexao<Funcionario>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoPlanos = new Conexao<Plano>(session))
            using (var repoAreaCobertura = new Conexao<Area_Cobertura>(sessionT5))
            {
                //obj.EquipeVendas = repoEquipeVendas.select(p => p.Campanha == obj).Result.ToList();
                //obj.Regioes = repoRegioes.select(p => p.Campanha == obj).Result.ToList();
                //obj.AreaCobertura = repoArea_Cobertura.select(p => p.Campanha == obj).Result.ToList();
                //obj.PlanosProdutos = repoPlanoProduto.select(p => p.Campanha == obj).Result.ToList();
                var retorno = new CampanhaCadastroModel();

                retorno.Id = obj.Id;
                retorno.Data_Inicio = obj.Data_Inicio;
                retorno.Data_Final = obj.Data_Final;
                retorno.Status = obj.Status.Value == Status_Default.Ativo ? true : false;
                retorno.Definir_CampanhaRegioes = obj.Definir_Campanha_Regioes.HasValue ? (obj.Definir_Campanha_Regioes.Value == Status_Default.Ativo ? true : false) : false;
                retorno.Camp_Tempo_Determinado = obj.Camp_Tempo_Determinado.HasValue ? (obj.Camp_Tempo_Determinado.Value == Status_Default.Ativo ? true : false) : false;
                retorno.Definir_Area_Cobertura = obj.Definir_Area_Cobertura.HasValue ? (obj.Definir_Area_Cobertura.Value == Status_Default.Ativo ? true : false) : false;
                retorno.Titulo = obj.Titulo;
                retorno.Definir_CampanhaEquipeVenda = obj.Definir_CampanhaEquipeVenda.HasValue ? (obj.Definir_CampanhaEquipeVenda.Value == Status_Default.Ativo ? true : false) : false;
                retorno.CampanhaUtilizadaPor = obj.CampanhaUtilizadaPor;
                retorno.PrefixoCupom = obj.PrefixoCupom;
                retorno.Id_Meio_Captacao = obj.Id_Meio_Captacao;
                retorno.EquipeVendas = obj.EquipeVendas.Select(p => new CampanhaListaModelEquipeVendas
                {
                    Id = p.Id,
                    Setor_Departamento = Convert.ToString(p.Setor_Departamento),
                    Setor_Departamento_Desc = repoDepartamento.selectFirst(y => y.Id == p.Setor_Departamento).Result?.Descricao,
                    Id_Funcionario = p.Id_Funcionario,
                    Id_Supervisor = p.Id_Supervisor,
                    Id_Funcionario_Desc = repoUsuario.selectFirst(y => y.Id == Convert.ToInt64(p.Id_Funcionario)).Result?.Nome,
                    Id_Supervisor_Desc = repoUsuario.selectFirst(y => y.Id == Convert.ToInt64(p.Id_Supervisor)).Result?.Nome,
                    Meta = p.Meta.HasValue ? p.Meta.Value.ToString("N2") : null,

                }).ToList();

                retorno.Regioes = obj.Regioes.Select(p =>
                {
                    Pais pais = repoPais.selectFirst(y => y.Id == Convert.ToInt64(p.Pais)).Result;
                    Uf uf = repoUf.selectFirst(y => y.Id == Convert.ToInt64(p.Estado)).Result;
                    Cidade cidade = repoCidade.selectFirst(y => y.Id == Convert.ToInt64(p.Cidade)).Result;

                    return new CampanhaListaModelRegioes
                    {
                        Id = p.Id,

                        Pais = Convert.ToString(pais?.Id),
                        Pais_Desc = pais?.Nome,

                        Estado = Convert.ToString(uf?.Id),
                        Estado_Desc = uf?.Descricao,

                        Cidade = Convert.ToString(cidade?.Id),
                        Cidade_Desc = cidade?.Descricao
                    };
                }).ToList();

                retorno.AreaCobertura = obj.AreaCobertura.Select(p => new CampanhaListaModelArea_Cobertura
                {
                    Id = p.Id,
                    Area_Cobertura = p.Area_Cobertura,
                    Area_Cobertura_Desc = p.Area_Cobertura.HasValue ? repoAreaCobertura.selectFirst(y => y.Id == p.Area_Cobertura).Result?.Descricao : null
                }).ToList();

                retorno.PlanosProdutos = obj.PlanosProdutos.Select(p => new Campanha_Plano_ProdutoListModel
                {
                    Id = p.Id,
                    Id_Plano_Produto = p.Id_Plano_Produto,
                    IsPlanProd = p.IsPlanProd,
                    Id_Plano_Produto_Desc = repoPlanos.selectFirst(param => param.Id == Convert.ToInt64(p.Id_Plano_Produto)).Result?.Descricao ?? String.Empty,


                    Parcela_Instalacao = Convert.ToString(p.Parcela_Instalacao),

                    Novo_Nome_Plano_Produto = p.Novo_Nome_Plano_Produto,
                    Valor_Plano_Produto = p.Valor_Plano_Produto.ToString("N2"),
                    Valor_Instalacao = p.Valor_Instalacao.ToString("N2"),
                    Validade_Plano_Produto = p.Validade_Plano_Produto.HasValue ? Convert.ToString((int)p.Validade_Plano_Produto) : null,
                    Validade_Plano_Produto_Desc = p.Validade_Plano_Produto.HasValue ? HtmlEnumExtensions.GetEnumDescription<Atividade_Decisoria>(p.Validade_Plano_Produto.Value) : Resources.RsBase.NAO,
                    Data_Validade = p.Data_Validade.HasValue ? Convert.ToString(p.Data_Validade.Value.ToString("dd/MM/yyyy")) : String.Empty,
                }).ToList();

                return retorno;

            }
        }

        private void BindViewToObj(ISession session, Campanha objBanco, CampanhaCadastroModel view)
        {
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoProduto = new Conexao<Produto>(session))
            using (var repoFilial = new Conexao<Filial>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                objBanco.Titulo = view.Titulo;
                objBanco.Data_Inicio = view.Data_Inicio;
                objBanco.Camp_Tempo_Determinado = view.Camp_Tempo_Determinado ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.Data_Final = view.Data_Final;
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.Id = view.Id;
                objBanco.Definir_Campanha_Regioes = view.Definir_CampanhaRegioes ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.Definir_Area_Cobertura = view.Definir_Area_Cobertura ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.Definir_CampanhaEquipeVenda = view.Definir_CampanhaEquipeVenda ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.CampanhaUtilizadaPor = view.CampanhaUtilizadaPor;
                objBanco.PrefixoCupom = view.PrefixoCupom;
                objBanco.Id_Meio_Captacao = view.Id_Meio_Captacao;
                objBanco.EquipeVendas = view.EquipeVendas.Where(p => p.Id == 0).Select(p => new Campanha_EquipeVendas
                {
                    Id = Convert.ToInt64(p.Id),
                    Setor_Departamento = Convert.ToInt64(p.Setor_Departamento),
                    Id_Funcionario = p.Id_Funcionario,
                    Id_Supervisor = p.Id_Supervisor,
                    Meta = Convert.ToDouble(p.Meta),
                    Campanha = objBanco,
                }).ToList();

                objBanco.Regioes = view.Regioes.Where(p => p.Id == 0).Select(p => new Campanha_Regioes
                {
                    Id = p.Id,
                    Pais = p.Pais,
                    Estado = p.Estado,
                    Cidade = p.Cidade,
                    Estado_Desc = p.Estado_Desc,
                    //CampanhaRegioes = p.CampanhaRegioes,
                    Campanha = objBanco,
                }).ToList();

                objBanco.AreaCobertura = view.AreaCobertura.Where(p => p.Id == 0).Select(p => new Campanha_AreaCobertura
                {
                    //Definir_Area_Cobertura = p.Definir_Area_Cobertura,
                    Area_Cobertura = p.Area_Cobertura,
                    Campanha = objBanco,
                    Id = p.Id
                }).ToList();

                objBanco.PlanosProdutos = view.PlanosProdutos.Where(p => p.Id == 0).Select(p => new Campanha_PlanoProduto
                {

                    Id = p.Id,
                    //Plano = repoPlano.selectFirst(param => param.Id == p.Plano)?.Result,
                    Id_Plano_Produto = p.Id_Plano_Produto,
                    Validade_Plano_Produto = p.Validade_Plano_Produto_Desc == HtmlEnumExtensions.GetEnumDescription<Atividade_Decisoria>(Atividade_Decisoria.Sim) ? Atividade_Decisoria.Sim : Atividade_Decisoria.Não,
                    IsPlanProd = repoPlano.selectFirst(plan => plan.Id == p.Id_Plano_Produto).Result != null ? PlanoProdutoEnum.Plano : PlanoProdutoEnum.Produto,

                    Data_Validade = Convert.ToDateTime(p.Data_Validade),
                    Novo_Nome_Plano_Produto = p.Novo_Nome_Plano_Produto,
                    Valor_Plano_Produto = Convert.ToDouble(p.Valor_Plano_Produto),
                    Valor_Instalacao = Convert.ToDouble(p.Valor_Instalacao),
                    Parcela_Instalacao = Convert.ToInt64(view.Parcela_Instalacao),
                    Campanha = objBanco,
                }).ToList().Select(prop => { if (prop.Data_Validade == DateTime.MinValue) { prop.Data_Validade = null; }; return prop; }).ToList();


            }
        }

        [HttpPost()]
        [Route("api/Campanhaapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CAMPANHA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Campanhaapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_CAMPANHA WHERE ID IN {_ids}");

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
        [Route("api/Campanhaapi/FindById")]
        public async Task<GetApiRetorno<CampanhaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<CampanhaCadastroModel> retorno = new GetApiRetorno<CampanhaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Campanha>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id), session, sessionT5);

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
        [Route("api/Campanhaapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<CampanhaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Campanha modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha>(session))
                    using (var repoEquipe = new Conexao<Campanha_EquipeVendas>(session))
                    using (var repoRegioes = new Conexao<Campanha_Regioes>(session))
                    using (var repoAreaCobertura = new Conexao<Campanha_AreaCobertura>(session))
                    using (var repoPlanoProduto = new Conexao<Campanha_PlanoProduto>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        bool enviaNotificacao = false;
                        if (modelo == null)
                        {
                            enviaNotificacao = true;
                            modelo = new Campanha
                            {
                                Status = Status_Default.Ativo
                            };
                        }

                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        if (!String.IsNullOrEmpty(value.Objeto.PrefixoCupom))
                        {
                            var campanhaCupomIgual = await repository.selectFirst(p => p.PrefixoCupom == value.Objeto.PrefixoCupom);
                            if (campanhaCupomIgual != null && campanhaCupomIgual.Id != modelo.Id)
                            {
                                throw new Exception("Não Pode Haver Duplicidade de Campanhas com o Mesmo Prefixo");
                            }
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                        await repoEquipe.SaveOrUpdate(modelo.EquipeVendas.ToList());
                        await repoRegioes.SaveOrUpdate(modelo.Regioes.ToList());
                        await repoAreaCobertura.SaveOrUpdate(modelo.AreaCobertura.ToList());
                        await repoPlanoProduto.SaveOrUpdate(modelo.PlanosProdutos.ToList());

                        if (enviaNotificacao)
                        { 
                            var notificacao = new NotificacoesApiController().EnviaNotificacoesComercial(34, null, modelo.Titulo);
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
                CONSTANTES.log4Net.ErrorFormat("Mensagem: {0} \r\n InnerException: {1} \r\n StackTrace: {2}", ex.Message, ex?.InnerException?.Message, ex.StackTrace);
            }

            return retorno;
        }

        [Route("api/Campanhaapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha>(session))
                    {

                        Campanha modelo = null;
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

        [Route("api/Campanhaapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha_PlanoProduto>(session))
                    {

                        Campanha_PlanoProduto modelo = null;
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

        [Route("api/Campanhaapi/ExcluirGridRegioes")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridRegioes(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha_Regioes>(session))
                    {

                        Campanha_Regioes modelo = null;
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

        [Route("api/Campanhaapi/ExcluirGridEquipe")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridEquipe(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha_EquipeVendas>(session))
                    {

                        Campanha_EquipeVendas modelo = null;
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

        [Route("api/Campanhaapi/ExcluirGridAreaCobertura")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridAreaCobertura(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Campanha_AreaCobertura>(session))
                    {

                        Campanha_AreaCobertura modelo = null;
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

        [Route("api/Campanhaapi/GetList")]
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<CampanhaListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<CampanhaListaModel>> retorno = new GetApiRetorno<IEnumerable<CampanhaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Campanha>(session))
            {
                try
                {
                    //var lista = await new Conexao<Campanha>(session).queryover().ListAsync();
                    var lista = await repository.select(s => s.Status == Status_Default.Ativo);

                    retorno.Objeto = lista.Select(p => new CampanhaListaModel
                    {
                        Id = p.Id,
                        Titulo = p.Titulo

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

        [Route("api/Campanhaapi/GetListVendedor")]
        [HttpGet()]
        public async Task<GetApiRetorno<IEnumerable<CampanhaListaModel>>> GetListVendedor(Int64 idFuncionario)
        {
            GetApiRetorno<IEnumerable<CampanhaListaModel>> retorno = new GetApiRetorno<IEnumerable<CampanhaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Funcionario>(session))
            using (var repoCampanha = new Conexao<Campanha>(session))
            {
                try
                {
                    Funcionario fun = await repository.selectFirst(s => s.Id == idFuncionario);
                    Int64 idFun = fun?.Id ?? -1;
                    Int64 idSup = fun?.Supervisor?.Id ?? -1;
                    Int64 idFil = fun?.Filial?.Id ?? -1;



                    DateTime hoje = DateTime.Now.Date;
                    /*var lista = await repoCampanha.select(s => s.Status == Status_Default.Ativo &&
                                                                                (s.Data_Inicio.Value.Date <= hoje && s.Data_Final.Value.Date >= hoje) &&
                                                                                (((s.Filial.Id == idFil) && (s.Supervisor == null) && (s.Funcionario == null)) ||
                                                                                 ((s.Filial.Id == idFil) && (s.Supervisor.Id == idSup) && (s.Funcionario == null)) ||
                                                                                 ((s.Filial.Id == idFil) && (s.Supervisor.Id == idSup) && (s.Funcionario.Id == idFun))));

                    retorno.Objeto = lista.Select(p => new CampanhaListaModel
                    {
                        Id = p.Id,
                        Titulo = p.Titulo

                    });*/

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

        #region Mobile
        [Route("api/Campanhaapi/GetListByVendedor")]
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<CampanhaListaModel>>> GetListByVendedor(ParamWebApi<CampanhaByVendendor_Mobile> values)
        {
            GetApiRetorno<IEnumerable<CampanhaListaModel>> retorno = new GetApiRetorno<IEnumerable<CampanhaListaModel>>();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCampanhaEquipeVenda = new Conexao<Campanha_EquipeVendas>(session))
            using (var repoCampanha = new Conexao<Campanha>(session))
            using (var repoFuncionario = new Conexao<Funcionario>(session))
            {
                try
                {

                    DateTime hoje = DateTime.Now.Date;
                    List<Campanha> campanhas = (await repoCampanha.select(p => p.Status == Status_Default.Ativo)).ToList();
                    Dictionary<Campanha, int> pontos_por_campanha = new Dictionary<Campanha, int>();
                    Funcionario funcionario = await repoFuncionario.selectFirst(s => s.Id == values.Objeto.Id_Vendedor);


                    foreach (Campanha item in campanhas)
                    {
                        int value = 0;

                        if (item.Definir_Campanha_Regioes.HasValue && item.Definir_Campanha_Regioes > 0)
                        {
                            if (item.Regioes.FirstOrDefault(p => p.Pais == values.Objeto.Pais &&
                                                                    p.Estado == values.Objeto.Estado &&
                                                                    p.Cidade == values.Objeto.Cidade) != null)
                            {
                                value++;
                            }
                            else if (item.Regioes.FirstOrDefault(p => p.Pais == values.Objeto.Pais &&
                                                                    p.Estado == values.Objeto.Cidade &&
                                                                    String.IsNullOrEmpty(p.Cidade)) != null)
                            {
                                value++;
                            }
                            else if (item.Regioes.FirstOrDefault(p => p.Pais == values.Objeto.Pais &&
                                                                    String.IsNullOrEmpty(p.Estado) &&
                                                                    String.IsNullOrEmpty(p.Cidade)) != null)
                            {
                                value++;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (item.Definir_CampanhaEquipeVenda.HasValue && item.Definir_CampanhaEquipeVenda == Status_Default.Ativo)
                        {

                            if (item.EquipeVendas.FirstOrDefault(p => p.Setor_Departamento == funcionario.Departamento.Id &&
                                                                        p.Id_Supervisor == Convert.ToString(funcionario.Supervisor.Id) &&
                                                                        p.Id_Funcionario == Convert.ToString(values.Objeto.Id_Vendedor)) != null)
                            {
                                value++;
                            }
                            else if (item.EquipeVendas.FirstOrDefault(p => p.Setor_Departamento == funcionario.Departamento.Id &&
                                                                        p.Id_Supervisor == Convert.ToString(funcionario.Supervisor.Id) &&
                                                                        String.IsNullOrEmpty(p.Id_Funcionario)) != null)
                            {
                                value++;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (item.Definir_Area_Cobertura.HasValue && item.Definir_Area_Cobertura == Status_Default.Ativo)
                        {
                            try
                            {
                                using (var GeographyHandler = new GeographyPattern())
                                using (var conexao = new ConexaoMongoGeography())
                                using (var repoAreas = new RepositoryMongo<Area_CoberturaMongo>(conexao))
                                {
                                    var point = new Location(Convert.ToDouble(values.Objeto.Latitude), Convert.ToDouble(values.Objeto.Longitude));
                                    List<Location> coordenadaCampanha = new List<Location>();
                                    List<Area_CoberturaMongo> listaareas = new List<Area_CoberturaMongo>();

                                    foreach (var area in item.AreaCobertura)
                                    {
                                        var areas = await repoAreas.FindAll(p => p.Id == area.Id);
                                        listaareas.AddRange(areas);
                                    }

                                    foreach (var polygon in listaareas)
                                    {

                                        foreach (var coordenada in polygon.Coordenadas)
                                        {

                                            for (var x = 0; x < coordenada.Count(); x++)
                                            {
                                                double locLat = coordenada[x].HasValue ? coordenada[x].Value : 0,
                                                        locLong = coordenada[++x].HasValue ? coordenada[++x].Value : 0;

                                                coordenadaCampanha.Add(new Location(locLat, locLong));
                                            }
                                        }
                                    }

                                    if (GeographyHandler.IsPointInPolygon(coordenadaCampanha, point))
                                    {
                                        value++;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        pontos_por_campanha.Add(item, value);

                    }

                    retorno.Objeto = (pontos_por_campanha.Where(p => p.Value == pontos_por_campanha.Values.Max()).Select(p => p.Key).Select(p => new CampanhaListaModel
                    {

                        Id = p.Id,
                        Titulo = p.Titulo

                    }));

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
        [Route("api/campanhaapi/GetCombo")]
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

                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder("FROM TB_CAMPANHA A "),
                                  sqlWhere = new StringBuilder(),
                                  sqlCount = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");

                        condicoes.Add(@" 
                                       ((A.CAMP_TEMPO_DETERMINADO = 1 AND (CURRENT_DATE BETWEEN CAST(A.DATA_INICIO AS DATE) AND CAST(A.DATA_FINAL AS DATE))) 
                                       OR (A.CAMP_TEMPO_DETERMINADO = 0 AND  A.Data_inicio IS NULL))");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendFormat("SELECT COUNT(A.ID) AS CO \r\n {0} \r\n {1}", sqlFrom, sqlWhere);

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.TITULO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["TITULO"])}"
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
        [Route("api/campanhaapi/GetComboAtivos")]
        public async Task<ObjCombo<Int64>> GetComboAtivos(RequestObjCombo<Int64?> request)
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

                    StringBuilder sql = new StringBuilder(),
                                  sqlFrom = new StringBuilder("FROM TB_CAMPANHA A "),
                                  sqlWhere = new StringBuilder(),
                                  sqlCount = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }
                    else
                        condicoes.Add($"STATUS = 1");

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)} ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendFormat("SELECT COUNT(A.ID) AS CO \r\n {0} \r\n {1}", sqlFrom, sqlWhere);

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.TITULO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["TITULO"])}"
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
        [Route("api/campanhaapi/GetComboMeioCaptacao")]
        public async Task<ObjCombo<Int64>> GetComboMeioCaptacao(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            { results = new List<IObjComboResult<Int64>>() };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                { //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder(),
                        sqlFrom = new StringBuilder("FROM TB_CAMPANHA A "),
                        sqlWhere = new StringBuilder(),
                        sqlCount = new StringBuilder();

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }
                    else
                    {
                        if (request.idpai.HasValue && request.idpai.Value > 0)
                        {
                            condicoes.Add($@"( A.STATUS = 1 AND ( A.ID_MEIO_CAPTACAO = {request.idpai.Value} OR  A.ID_MEIO_CAPTACAO IS NULL))");
                        }
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%'");
                    }

                    if (condicoes.Count == 0)
                    {
                        condicoes.Add($" A.STATUS = 1");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlWhere.AppendLine($"WHERE {String.Join(" AND ", condicoes)} ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        sqlCount.AppendFormat("SELECT COUNT(A.ID) AS CO \r\n {0} \r\n {1}", sqlFrom, sqlWhere);
                        using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync()) retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select 
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(sqlWhere.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.TITULO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64> { Id = Convert.ToInt64(reader["ID"]), Text = $"{Convert.ToString(reader["TITULO"])}" };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            return retorno;
        }

        [HttpGet]
        [Route("api/campanhaapi/GetMeioByCampanha")]
        public async Task<long?> GetMeioByCampanha(long idCampanha)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT ID_MEIO_CAPTACAO FROM TB_CAMPANHA WHERE ID = @ID");

                using (var com = session.Connection.CreateCommand())
                {
                    var parametros = new Dictionary<string, object>
                        {
                            { "ID", idCampanha }
                        };

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            return reader["ID_MEIO_CAPTACAO"] == DBNull.Value ? (long?)null : Convert.ToInt64(reader["ID_MEIO_CAPTACAO"]);
                        }
                    }
                }
            }

            return null;
        }




        [HttpPost()]
        [Route("api/campanhaapi/GetComboAnalise")]
        public async Task<ObjCombo<Int64>> GetComboAnalise(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CAMPANHA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");

                    }
                    else
                    {
                        condicoes.Add($" (CURRENT_DATE BETWEEN CAST(A.DATA_INICIO AS DATE) AND CAST(A.DATA_FINAL AS DATE)) ");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"UPPER(A.ID) = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" UPPER(A.TITULO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.TITULO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.TITULO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["TITULO"])}"
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
        [Route("api/campanhaapi/GetComboVenda")]
        public async Task<ObjCombo<Int64>> GetComboVenda(RequestObjComboConfig<long?, Campanha_Request_ObjComboVenda> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCamapanha = new Conexao<Campanha>(sessionNh))
            using (var repoUf = new Conexao<Uf>(sessionNh))
            {
                try
                {
                    List<Campanha> campanhas = (await repoCamapanha.select(p => p.Status == Status_Default.Ativo)).ToList();
                    if (request.id.HasValue)
                    {
                        campanhas = campanhas.FindAll(p => p.Id == request.id.Value);
                    }


                    var pontosPorCampanha = new Dictionary<Campanha, int>();

                    foreach (Campanha campanha in campanhas)
                    {
                        int pontos = 0;

                        if (campanha.Definir_Campanha_Regioes > 0)
                        {
                            long idUf = (await repoUf.selectFirst(p => p.Sigla == request.config.ObjRegioes.Uf)).Id;

                            if (campanha.Regioes.FirstOrDefault(p => p.Pais == request.config.ObjRegioes.Pais &&
                                                                 p.Estado == Convert.ToString(idUf) &&
                                                                 p.Cidade == request.config.ObjRegioes.Cidade) != null)
                            {
                                pontos++;
                            }
                            else if (campanha.Regioes.FirstOrDefault(p => p.Pais == request.config.ObjRegioes.Pais &&
                                                                  p.Estado == Convert.ToString(idUf) &&
                                                                  String.IsNullOrEmpty(p.Cidade)) != null)
                            {
                                pontos++;
                            }
                            else if (campanha.Regioes.FirstOrDefault(p => p.Pais == request.config.ObjRegioes.Pais &&
                                                                  String.IsNullOrEmpty(p.Estado) &&
                                                                  String.IsNullOrEmpty(p.Cidade)) != null)
                            {
                                pontos++;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (campanha.Definir_CampanhaEquipeVenda == Status_Default.Ativo)
                        {

                            if (campanha.EquipeVendas.FirstOrDefault(p => Convert.ToString(p.Setor_Departamento) == request.config.ObjEquipeVendas.Id_Setor_Departamento &&
                                                                      Convert.ToString(p.Id_Supervisor) == Convert.ToString(request.config.ObjEquipeVendas.Id_Supervisor) &&
                                                                      Convert.ToString(p.Id_Funcionario) == Convert.ToString(request.config.ObjEquipeVendas.Id_Vendedor)) != null)
                            {
                                pontos++;
                            }
                            else if (campanha.EquipeVendas.FirstOrDefault(p => Convert.ToString(p.Setor_Departamento) == request.config.ObjEquipeVendas.Id_Setor_Departamento &&
                                                                      p.Id_Supervisor == Convert.ToString(request.config.ObjEquipeVendas.Id_Supervisor) &&
                                                                      String.IsNullOrEmpty(p.Id_Funcionario)) != null)
                            {
                                pontos++;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (campanha.Definir_Area_Cobertura == Status_Default.Ativo)
                        {
                            if (request.config.Acesso_Area_Cobertura)
                            {
                                using (var GeographyHandler = new GeographyPattern())
                                using (var conexao = new ConexaoMongoGeography())
                                using (var repoAreas = new RepositoryMongo<Area_CoberturaMongo>(conexao))
                                {
                                    var point = new Location(Convert.ToDouble(request.config.ObjArea_Cobertura.Latitude), Convert.ToDouble(request.config.ObjArea_Cobertura.Longitude));
                                    var coordenadaCampanha = new List<Location>();
                                    var listaAreas = new List<Area_CoberturaMongo>();

                                    foreach (var area in campanha.AreaCobertura)
                                    {
                                        var areas = await repoAreas.FindAll(p => p.Id == area.Id);
                                        listaAreas.AddRange(areas);
                                    }

                                    foreach (var polygon in listaAreas)
                                    {
                                        foreach (var coordenada in polygon.Coordenadas)
                                        {
                                            for (var x = 0; x < coordenada.Count(); x++)
                                            {
                                                double locLat = coordenada[x].HasValue ? coordenada[x].Value : 0;
                                                double locLong = coordenada[++x].HasValue ? coordenada[++x].Value : 0;

                                                coordenadaCampanha.Add(new Location(locLat, locLong));
                                            }
                                        }
                                    }

                                    if (GeographyHandler.IsPointInPolygon(coordenadaCampanha, point))
                                    {
                                        pontos++;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        pontosPorCampanha.Add(campanha, pontos);
                    }



                    retorno.results.AddRange(
                        pontosPorCampanha
                            .Where(p => p.Value == pontosPorCampanha.Values.Max())
                            .Select(p => p.Key)
                            .Select(p => new ObjCampanhaResultado
                            {
                                Id = p.Id,
                                Text = p.Titulo
                            })
                    );

                    retorno.total_count = retorno.results.Count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/campanhaapi/GetComboPlanoProduto")]
        public async Task<ObjCombo<Int64>> GetComboPlanoProduto(RequestObjComboPai<Int64?, long?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    #region Plano

                    #region Filtro

                    Dictionary<String, Object> parametrosPlano = new Dictionary<string, object>();

                    StringBuilder sqlFromPlano = new StringBuilder();

                    sqlFromPlano.AppendLine("FROM TB_CAMPANHA_PLANO_PRODUTO CAMP_PP");

                    sqlFromPlano.AppendLine("INNER JOIN TB_CLIENTE_PLANO_PRODUTO CLIENTE_PP");
                    sqlFromPlano.AppendLine("ON CLIENTE_PP.ID = CAMP_PP.ID_PLANO_PRODUTO");

                    sqlFromPlano.AppendLine("LEFT JOIN TB_PLANO P");
                    sqlFromPlano.AppendLine("ON P.ID = CLIENTE_PP.ID_PLANO");

                    List<String> condicoesPlano = new List<String>();

                    if (request.idpai.HasValue)
                    {
                        condicoesPlano.Add($"CAMP_PP.ID_CAMPANHA = {request.idpai}");
                    }

                    if (request.id.HasValue)
                    {
                        condicoesPlano.Add($"P.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoesPlano.Add($" UPPER(P.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoesPlano.Count() > 0)
                    {
                        sqlFromPlano.AppendLine($"WHERE ( {String.Join(" AND ", condicoesPlano)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlPlano = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sqlPlano.AppendLine(sqlFromPlano.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlPlano, parametrosPlano, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlPlano = new StringBuilder();

                        sqlPlano.AppendLine("SELECT P.ID, P.DESCRICAO"); // Adicionar o resto dos campos
                        sqlPlano.AppendLine(sqlFromPlano.ToString());


                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sqlPlano, parametrosPlano, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new PlanoProdutoComboModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Tipo = PlanoProdutoEnum.Plano
                                };

                                retorno.results.Add(obj);
                            }
                        }
                    }

                    #endregion

                    #region Produto

                    #region Filtro

                    Dictionary<String, Object> parametrosProduto = new Dictionary<string, object>();

                    StringBuilder sqlFromProduto = new StringBuilder();

                    sqlFromProduto.AppendLine("FROM TB_CAMPANHA_PLANO_PRODUTO CAMP_PP");

                    sqlFromProduto.AppendLine("INNER JOIN TB_CLIENTE_PLANTO_PRODUTO CLIENTE_PP");
                    sqlFromProduto.AppendLine("ON CLIENTE_PP.ID = CAMP_PP.ID_PLANO_PRODUTO");

                    sqlFromProduto.AppendLine("LEFT JOIN TB_PRODUTO P");
                    sqlFromProduto.AppendLine("ON P.ID = CLIENTE_PP.ID_PRODUTO");

                    List<String> condicoesProduto = new List<String>();

                    if (request.idpai.HasValue)
                    {
                        condicoesProduto.Add($"CAMP_PP.ID_CAMPANHA = {request.idpai}");
                    }

                    if (request.id.HasValue)
                    {
                        condicoesProduto.Add($"P.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoesProduto.Add($" UPPER(P.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoesProduto.Count() > 0)
                    {
                        sqlFromProduto.AppendLine($"WHERE ( {String.Join(" AND ", condicoesProduto)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlProduto = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sqlProduto.AppendLine(sqlFromPlano.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlProduto, parametrosProduto, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count += Convert.ToInt32(reader["CO"]);
                        }
                    }

                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sqlProduto = new StringBuilder();

                        sqlProduto.AppendLine("SELECT P.ID, P.DESCRICAO"); // Adicionar o resto dos campos
                        sqlProduto.AppendLine(sqlFromPlano.ToString());


                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sqlProduto, parametrosProduto, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new PlanoProdutoComboModels
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Tipo = PlanoProdutoEnum.Produto
                                };

                                retorno.results.Add(obj);
                            }
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/campanhaapi/GetValorPlanoProdutoCampanha")]
        public async Task<GetApiRetorno<IEnumerable<Campanha_PlanoProduto>>> GetValorPlanoProdutoCampanha(long idCampanha, long idPlanoProduto)
        {
            GetApiRetorno<IEnumerable<Campanha_PlanoProduto>> retorno = new GetApiRetorno<IEnumerable<Campanha_PlanoProduto>>();

            try
            {

                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCamapanha = new Conexao<Campanha_PlanoProduto>(sessionNh))
                using (var repoUf = new Conexao<Uf>(sessionNh))
                {
                    var campanhas = (await repoCamapanha.select(p => p.Id_Plano_Produto == idPlanoProduto && p.Campanha.Id == idCampanha));
                    retorno.Objeto = campanhas.Select(p => new Campanha_PlanoProduto
                    {   
                        Valor_Plano_Produto = Convert.ToDouble(p.Valor_Plano_Produto),
                        Id_Plano_Produto = p.Id_Plano_Produto,
                        Valor_Instalacao = Convert.ToDouble(p.Valor_Instalacao),
                    });

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }


            return retorno;

        }



        [HttpPost()]
        [Route("api/Campanhaapi/InativarCampanha")]
        public async Task<Boolean> InativarCampanha(Campanha_DataTableRequest request)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("UPDATE TB_CAMPANHA SET STATUS = 0 ");
                    sql.AppendLine("WHERE CURRENT_DATE > DATA_FINAL");

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }

            return true;
        }

        public async Task<GetApiRetorno<Int32>> GetMetaCampanha(Int64 Id_Funcionario)
        {
            GetApiRetorno<Int32> retorno = new GetApiRetorno<int>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT SUM(A.META) AS META_FUNCIONARIO ");

                        sql.AppendLine(" FROM TB_CAMPANHA A ");

                        sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO B ");
                        sql.AppendLine(" ON(B.E_SUPERVISOR = 1 AND A.ID_SUPERVISOR = B.ID) ");

                        sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ");
                        sql.AppendLine(" ON(C.E_SUPERVISOR = 0 AND A.ID_FUNCIONARIO = C.ID) ");

                        sql.AppendLine(" LEFT JOIN TB_FILIAL D ");
                        sql.AppendLine(" ON(D.ID = A.ID_FILIAL) ");

                        sql.AppendLine(" WHERE A.STATUS = 1 ");
                        sql.AppendLine("   AND NOT( ");
                        sql.AppendLine("     (B.ID IS NULL) ");
                        sql.AppendLine("     AND(C.ID IS NULL) ");
                        sql.AppendLine("     AND(D.ID IS NULL) ");
                        sql.AppendLine("   ) ");
                        sql.AppendLine("   AND( ");
                        sql.AppendLine("     C.ID = @ID_FUNC ");
                        sql.AppendLine("     OR( ");
                        sql.AppendLine("       (SELECT E.ID_SUPERVISOR ");
                        sql.AppendLine("          FROM TB_FUNCIONARIO E ");
                        sql.AppendLine("        WHERE E.ID = @ID_FUNC) = B.ID ");
                        sql.AppendLine("        AND C.ID IS NULL ");
                        sql.AppendLine("     ) ");
                        sql.AppendLine("     OR( ");
                        sql.AppendLine("       (SELECT F.ID_FILIAL ");
                        sql.AppendLine("         FROM TB_FUNCIONARIO F ");
                        sql.AppendLine("        WHERE F.ID = @ID_FUNC) = D.ID ");
                        sql.AppendLine("        AND B.ID IS NULL ");
                        sql.AppendLine("        AND C.ID IS NULL ");
                        sql.AppendLine("     ) ");
                        sql.AppendLine("   ) ");

                        parametros.Add("@ID_FUNC", Id_Funcionario);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = Convert.ToInt32(reader["META_FUNCIONARIO"]);
                                retorno.Ok = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Campanhaapi/EditarCampanhaEquipeVenda")]
        public async Task<PostApiRetorno<long>> EditarCampanhaEquipeVenda(CampanhaListaModelEquipeVendas request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (Conexao<Campanha_EquipeVendas> repository = new Conexao<Campanha_EquipeVendas>(session))
                {
                    Campanha_EquipeVendas CEquipeVendas = await repository.selectFirst(p => p.Id == Convert.ToInt64(request.Id));
                    if (CEquipeVendas == null)
                    {
                        throw new Exception("Não foi encontrado o registro de Equipe de Vendas na Campanha selecionada");
                    }

                    BindEquipeVendaInCampanha(request, CEquipeVendas);

                    await repository.SaveOrUpdate(CEquipeVendas);

                    retorno.IdObjeto = CEquipeVendas.Id;
                }
                retorno.Ok = true;
            }

            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }
            return retorno;
        }

        private void BindEquipeVendaInCampanha(CampanhaListaModelEquipeVendas request, Campanha_EquipeVendas Camp_Equip_Vendas)
        {
            Camp_Equip_Vendas.Setor_Departamento = Convert.ToInt64(request.Setor_Departamento);
            Camp_Equip_Vendas.Id_Funcionario = request.Id_Funcionario;
            Camp_Equip_Vendas.Id_Supervisor = request.Id_Supervisor;
            Camp_Equip_Vendas.Meta = Convert.ToDouble(request.Meta);
            Camp_Equip_Vendas.UsuarioAlteracao = User.Identity.GetUserId();
            Camp_Equip_Vendas.DataHoraAlteracao = DateTime.Now;

        }

        [HttpPost]
        [Route("api/Campanhaapi/EditarCampanhaPlano")]
        public async Task<PostApiRetorno<long>> EditarCampanhaPlano(Campanha_Plano_ProdutoListModel request)
        {
            var retorno = new PostApiRetorno<long>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (Conexao<Campanha_PlanoProduto> repository = new Conexao<Campanha_PlanoProduto>(session))
                {
                    Campanha_PlanoProduto Cpp = await repository.selectFirst(p => p.Id == request.Id);
                    if (Cpp == null)
                    {
                        throw new Exception("Não foi encontrado o registro de Plano/Produto na Campanha selecionada");
                    }

                    BindPlanoProdutoinCampanha(request, Cpp);

                    await repository.SaveOrUpdate(Cpp);

                    retorno.IdObjeto = Cpp.Id;
                }
                retorno.Ok = true;
            }

            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
            }
            return retorno;
        }

        private void BindPlanoProdutoinCampanha(Campanha_Plano_ProdutoListModel request, Campanha_PlanoProduto Cpp)
        {
            Cpp.DataHoraAlteracao = DateTime.Now;
            Cpp.UsuarioAlteracao = User.Identity.GetUserId();
            Cpp.Id_Plano_Produto = request.Id_Plano_Produto;
            Cpp.Validade_Plano_Produto = request.Validade_Plano_Produto_Desc == HtmlEnumExtensions.GetEnumDescription<Atividade_Decisoria>(Atividade_Decisoria.Sim) ? Atividade_Decisoria.Sim : Atividade_Decisoria.Não;
            Cpp.Data_Validade = Convert.ToDateTime(request.Data_Validade);
            Cpp.Novo_Nome_Plano_Produto = request.Novo_Nome_Plano_Produto;
            Cpp.Valor_Plano_Produto = Convert.ToDouble(request.Valor_Plano_Produto);
            Cpp.Valor_Instalacao = Convert.ToDouble(request.Valor_Instalacao);
            Cpp.Parcela_Instalacao = Convert.ToInt64(request.Parcela_Instalacao);
        }

        [Route("api/Campanhaapi/ProcedureSetType")]
        [HttpGet()]
        public async Task<GetApiRetorno<List<long>>> Execute_Procedure_FixCampo()
        {
            GetApiRetorno<List<long>> retorno = new GetApiRetorno<List<Int64>> { Objeto = new List<Int64>() };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("select RNUM  from SP_SET_CAMP_PLAN_PROD_TYPE");
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                        while (await reader.ReadAsync())
                        {
                            long? Parametro = reader["NUM"] != DBNull.Value ? Convert.ToInt64(reader["NUM"]) : 0;
                            retorno.Objeto.Add(Parametro.Value);
                        }
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = true;
                retorno.Mensagem = erro.Message;
            }
            return retorno;
        }
    }
}
