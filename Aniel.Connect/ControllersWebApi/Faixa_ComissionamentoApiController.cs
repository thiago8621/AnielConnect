using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
//using Sinapse.Model;
//using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Faixa_ComissionamentoApiController : ApiController
    {
        [HttpPost()]
        [Route("api/Faixa_Comissionamentoapi/GetAll")]
        public async Task<DataTableResponse<Faixa_ComissionamentoListModel>> GetAll(Faixa_Comissionamento_DataTableRequest request)
        {
            List<Faixa_ComissionamentoListModel> retorno = new List<Faixa_ComissionamentoListModel> ();
            IOrderedEnumerable<Faixa_ComissionamentoListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder(" FROM TB_FAIXA_COMISSIONAMENTO A ");
             //   sqlFrom.AppendLine(" WHERE A.STATUS = 1 ");


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

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.CODIGO, A.DESCRICAO, A.STATUS ");// , A.FAIXA_INICIAL, A.FAIXA_FINAL, COEFICIENTE,
                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) OR (A.CODIGO LIKE @SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
                    }

                    
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Faixa_ComissionamentoListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Codigo = Convert.ToInt64(reader["CODIGO"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
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
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Faixa_ComissionamentoListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/Faixa_ComissionamentoApi/busca")]
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
                        sql.AppendLine("FROM TB_FAIXA_COMISSIONAMENTO A");
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
        public async Task<GetApiRetorno<IEnumerable<Faixa_ComissionamentoListModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Faixa_ComissionamentoListModel>> retorno = new GetApiRetorno<IEnumerable<Faixa_ComissionamentoListModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Faixa_Comissionamento>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Faixa_ComissionamentoListModel
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Descricao = p.Descricao,
                        //Filial = 
                        //Faixa_Inicial = p.Faixa_Inicial,
                        //Faixa_Final = p.Faixa_Final,
                        //Coeficiente = p.Coeficiente

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

        private async Task<Faixa_ComissionamentoCadastroModel> GetViewObj(ISession session, Faixa_Comissionamento obj)
        {
            using (var repoFilial = new Conexao<Filial>(session))            
            {
                var _filial = await repoFilial.selectFirst(p => p.Faixa_Comissionamento.Id == obj.Id);

                return new Faixa_ComissionamentoCadastroModel
                {
                    Id = obj.Id,
                    Codigo = Convert.ToString(obj.Codigo),
                    Descricao = obj.Descricao,
                    Status = obj.Status.Value == Status_Default.Ativo ? true : false,
                    Id_Filial = _filial?.Id,
                    Itens = (obj.Itens.Count() == 0) ? new List<Faixa_Comissionamento_ItemModel>() :
                                                       obj.Itens.Select(p => new Faixa_Comissionamento_ItemModel
                                                       {
                                                           Id = p.Id,
                                                           Id_Faixa_Comissionamento = obj.Id,
                                                           Coeficiente = p.Coeficiente.HasValue ? p.Coeficiente.Value.ToString("N2") : String.Empty,
                                                           Faixa_Final = p.Faixa_Final.HasValue ? p.Faixa_Final.Value.ToString("N2") : String.Empty,
                                                           Faixa_Inicial = p.Faixa_Inicial.HasValue ? p.Faixa_Inicial.Value.ToString("N2") : String.Empty
                                                       }).ToList()
                };
            }
        }

        private void BindViewToObj(ISession session, ref Faixa_Comissionamento objBanco, Faixa_ComissionamentoCadastroModel view)
        {
            objBanco.Id = view.Id;
            if (objBanco.Id != 0)
            {
                objBanco.Codigo = Convert.ToInt64(view.Codigo);
            }
            
            objBanco.Descricao = view.Descricao;

            List<Faixa_Comissionamento_Item> faixaItemTmp = objBanco.Itens.ToList();
            foreach (var item in view.Itens.FindAll(p => p.Id == 0))
            {
                faixaItemTmp.Add(new Faixa_Comissionamento_Item
                {
                    Coeficiente = String.IsNullOrEmpty(item.Coeficiente) ? (Double?)null : Convert.ToDouble(item.Coeficiente),
                    Faixa_Inicial = String.IsNullOrEmpty(item.Faixa_Inicial) ? (Double?)null : Convert.ToDouble(item.Faixa_Inicial),
                    Faixa_Final = String.IsNullOrEmpty(item.Faixa_Final) ? (Double?)null : Convert.ToDouble(item.Faixa_Final),
                    Faixa_Comissionamento = objBanco
                    
                });
            }
            objBanco.Itens = faixaItemTmp;

        }

        [HttpPost()]
        [Route("api/Faixa_ComissionamentoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_FAIXA_COMISSIONAMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Faixa_ComissionamentoApi/FindById")]
        public async Task<GetApiRetorno<Faixa_ComissionamentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Faixa_ComissionamentoCadastroModel> retorno = new GetApiRetorno<Faixa_ComissionamentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Faixa_Comissionamento>(session))
            {
                try
                {
                    retorno.Objeto = await GetViewObj(session, await repository.selectFirst(p => p.Id == Id));
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
        [Route("api/Faixa_ComissionamentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Faixa_ComissionamentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Faixa_Comissionamento modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Faixa_Comissionamento> (session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Faixa_Comissionamento
                            {
                                Status = Status_Default.Ativo,
                                
                                UsuarioCriacao = value.Usuario,

                                Codigo = await repository.Max(p => p.Codigo) + 1
                            };

                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);


                        using (var repositoryFilial = new Conexao<Filial>(session))
                        {
                            var filialAnterior = await repositoryFilial.selectFirst(p => p.Faixa_Comissionamento == modelo);

                            if (filialAnterior != null && filialAnterior?.Id != value.Objeto.Id_Filial.Value)
                            {
                                filialAnterior.Faixa_Comissionamento = null;
                                await repositoryFilial.SaveOrUpdate(filialAnterior);
                            }

                            var filialAtual = await repositoryFilial.selectFirst(p => p.Id == value.Objeto.Id_Filial.Value);
                            filialAtual.Faixa_Comissionamento = modelo;
                            await repositoryFilial.SaveOrUpdate(filialAtual);
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

        [Route("api/Faixa_ComissionamentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Faixa_Comissionamento>(session))
                    {

                        Faixa_Comissionamento modelo = null;
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
        [Route("api/Faixa_ComissionamentoApi/ExcluirLista")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirLista(String ids)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            var Ids = ids.Split(',');

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Faixa_Comissionamento>(session))
                    using (var repositoryFuncionario = new Conexao<Funcionario>(session))
                    using (var repositoryItens = new Conexao<Faixa_Comissionamento_Item>(session))
                    using (var repositoryFilial = new Conexao<Filial>(session))
                    {

                        List<Faixa_Comissionamento_Item> modelo = new List<Faixa_Comissionamento_Item>();
                        if (Ids.Length != 0)
                        {
                            foreach(var id in Ids)
                            {
                                foreach(var funcionario in await repositoryFuncionario.select(p => p.Faixa.Id == Convert.ToInt64(id)))
                                {
                                    funcionario.Faixa = null;
                                    await repositoryFuncionario.Save(funcionario);
                                }

                                foreach (var filial in await repositoryFilial.select(p => p.Faixa_Comissionamento.Id == Convert.ToInt64(id)))
                                {
                                    filial.Faixa_Comissionamento = null;
                                    await repositoryFilial.Save(filial);
                                }

                                await repositoryItens.Remove(await repositoryItens.select(p => p.Faixa_Comissionamento.Id == Convert.ToInt64(id)));
                                await repository.Remove(await repository.select(p => p.Id == Convert.ToInt64(id)));
                            }

                            retorno.IdObjeto = Convert.ToInt64(Ids.Last());
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

        [Route("api/Faixa_ComissionamentoApi/ExcluirFaixaItem")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirFaixaItem(Int64 item, Int64 faixa)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Faixa_Comissionamento_Item>(session))
                    {
                        Faixa_Comissionamento_Item modelo = new Faixa_Comissionamento_Item();

                        modelo = await repository.selectFirst(p => p.Id == item &&
                                                                   p.Faixa_Comissionamento.Id == faixa);

                        if (modelo != null)
                            await repository.Remove(modelo);
                        else
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);

                        retorno.IdObjeto = 0;
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

    }
}
