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
    public class OrganizacaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Organizacaoapi/GetAll")]
        public async Task<DataTableResponse<OrganizacaoListaModel>> GetAll(Organizacao_DataTableRequest request)
        {

            List<OrganizacaoListaModel> retorno = new List<OrganizacaoListaModel>();
            IOrderedEnumerable<OrganizacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_ORGANIZACAO A");

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

                    sql.AppendLine(" SELECT A.STATUS,A.ID, A.NOME_FANTASIA,A.RAZAO_SOCIAL,A.CNPJ,A.CLASSIFICACAO_ORGANIZACAO ");
                    sql.AppendLine(" FROM TB_ORGANIZACAO A ");
                    

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.NOME_FANTASIA) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new OrganizacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome_Fantasia = Convert.ToString(reader["NOME_FANTASIA"]),
                                Classificacao = HtmlEnumExtensions.GetEnumDescription<Classificacao_Organizacao>(Convert.ToString(reader["CLASSIFICACAO_ORGANIZACAO"])),
                                CNPJ = Convert.ToString(reader["CNPJ"]),
                                Razao_Social = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                
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
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome_Fantasia);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome_Fantasia);
                            break;
                        }
                    case 3:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Nome_Fantasia);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<OrganizacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [Route("api/Organizacaoapi/GetList")]
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<DescontoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<DescontoListaModel>> retorno = new GetApiRetorno<IEnumerable<DescontoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoDesconto = new Conexao<Desconto>(session))
            {
                try
                {
                    var lista = await repoDesconto.queryover().ListAsync();

                    retorno.Objeto = new List<DescontoListaModel>(lista.Where(p => p.Status == Status_Default.Ativo).Select(p => new DescontoListaModel
                    {
                        Id = p.Id,
                        Descricao = Convert.ToString(p.Descricao),
                        Valor_Desconto = Convert.ToString(p.Valor_Desconto),
                        Percentual_Desconto = Convert.ToString(p.Percentual_Desconto),
                        Tipo_Desconto = Convert.ToString(p.Tipo_Desconto?.Descricao)
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

        [HttpPost()]
        [Route("api/Organizacaoapi/GetCombo")]
        public async Task<ObjCombo<Int64?>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64?> retorno = new ObjCombo<Int64?>
            {
                results = new List<IObjComboResult<Int64?>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_ORGANIZACAO A ");
                        

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
                        condicoes.Add($"(UPPER(A.NOME_FANTASIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE  ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME_FANTASIA ")
                                                    .AppendLine(sqlFrom.ToString())
                                                    .AppendLine("ORDER BY UPPER(A.NOME_FANTASIA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64?>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME_FANTASIA"]),
                                    
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

        private OrganizacaoCadastroModel GetViewObj(Organizacao obj)
        {
            return new OrganizacaoCadastroModel
            {
                Id = obj.Id,
                Nome_Fantasia = obj.Nome_Fantasia,
                Classificacao_Organizacao = obj.Classificacao_Organizacao,
                CNPJ = obj.CNPJ,
                Razao_Social = obj.Razao_Social,
                Status = obj.Status == Status_Default.Ativo ? true : false,
                Id_Bairro = obj.Id_Bairro,
                Id_Cidade = obj.Id_Cidade,
                Cep = obj.Cep,
                Complemento = obj.Complemento,
                Latitude = Convert.ToString(obj.Latitude),
                Logradouro = obj.Logradouro,
                Longitude = Convert.ToString(obj.Longitude),
                Numero = obj.Numero,
                Uf = obj.Uf,
                Dominios =  (obj.Dominios.Count() == 0) ? new List<Organizacao_DominioModel>() :
                                                   obj.Dominios.Select(p => new Organizacao_DominioModel
                                                   {
                                                       Id = p.Id,
                                                       Dominio = p.Dominio
                                                   }).ToList(),

                Contatos = (obj.Contatos.Count() == 0) ? new List<Organizacao_ContatoModel>() :
                                                   obj.Contatos.Select(p => new Organizacao_ContatoModel
                                                   {
                                                       Id = p.Id,
                                                       Contato = p.Contato,
                                                       Tipo_Contato = p.Tipo_Contato,
                                                       Informacao_Adicional = p.Informacao_Adicional,
                                                       Tipo_Contato_Descricao = HtmlEnumExtensions.GetEnumDescription<Tipo_Contato>(p.Tipo_Contato)
                                                   }).ToList()
            };

        }

        private void BindViewToObj(ISession session, Organizacao objBanco, OrganizacaoCadastroModel view)
        {
            using (var repoTipoDesconto = new Conexao<Tipo_Desconto>(session))
            using (var repoMeio_Captacao = new Conexao<Meio_Captacao>(session))
            {
                objBanco.Nome_Fantasia = view.Nome_Fantasia;
                objBanco.Razao_Social = view.Razao_Social;
                objBanco.CNPJ = view.CNPJ;
                objBanco.Classificacao_Organizacao = view.Classificacao_Organizacao.Value;
                objBanco.Cep = !String.IsNullOrWhiteSpace(view.Cep) ? view.Cep : "";
                objBanco.Logradouro = !String.IsNullOrWhiteSpace(view.Logradouro) ? view.Logradouro : "";
                objBanco.Numero = view.Numero != null ? view.Numero : (Int32?)null;
                objBanco.Uf = !String.IsNullOrWhiteSpace(view.Uf) ? view.Uf : "";
                objBanco.Complemento = !String.IsNullOrWhiteSpace(view.Complemento) ? view.Complemento : "";

                

                objBanco.Latitude = !String.IsNullOrWhiteSpace(view.Latitude) ? Convert.ToDouble(view.Latitude.Replace('.', ',')) : (Double?)null;
                objBanco.Longitude = !String.IsNullOrWhiteSpace(view.Longitude) ? Convert.ToDouble(view.Longitude.Replace('.', ',')) : (Double?)null;
                objBanco.Id_Cidade = view.Id_Cidade;
                objBanco.Id_Bairro = view.Id_Bairro;

                var dominiosTmp = objBanco.Dominios.ToList();
                foreach (var dominio in view.Dominios.FindAll(p => p.Id == 0))
                {
                    dominiosTmp.Add(new Dominio_Organizacao
                    {
                        Organizacao = objBanco,
                        Dominio = dominio.Dominio
                    });
                }
                objBanco.Dominios = dominiosTmp;


                objBanco.Contatos = objBanco.Contatos.Concat(view.Contatos.FindAll(p => p.Id == 0).Select(p => new Organizacao_Contato
                {
                    Contato = p.Contato,
                    Tipo_Contato = p.Tipo_Contato,
                    Informacao_Adicional = p.Informacao_Adicional,
                    Organizacao = objBanco
                })).ToList();
                
            }
        }

        [HttpPost()]
        [Route("api/Organizacaoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ORGANIZACAO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Organizacaoapi/FindById")]
        public async Task<GetApiRetorno<OrganizacaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<OrganizacaoCadastroModel> retorno = new GetApiRetorno<OrganizacaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoOrganizacao = new Conexao<Organizacao>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoOrganizacao.selectFirst(p => p.Id == Id));

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
        [Route("api/Organizacaoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<OrganizacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Organizacao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Organizacao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Organizacao
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



        [Route("api/Organizacaoapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Dominio_Organizacao>(session))
                    {

                        Dominio_Organizacao modelo = null;
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

        [Route("api/Organizacaoapi/EditarGrid")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> EditarGrid(Organizacao_DominioModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Dominio_Organizacao>(session))
                    {

                        Dominio_Organizacao modelo = null;
                        if (value.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Id)).FirstOrDefault();
                            modelo.Dominio = value.Dominio;

                            await repository.Update(modelo);

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

        [Route("api/Organizacaoapi/EditarGridContato")]
        [HttpPost()]
        public async Task<PostApiRetorno<Int64>> EditarGridContato(Organizacao_ContatoModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Organizacao_Contato>(session))
                    {

                        Organizacao_Contato modelo = null;
                        if (value.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Id)).FirstOrDefault();
                            modelo.Contato = value.Contato;
                            modelo.Informacao_Adicional = value.Informacao_Adicional;
                            modelo.Tipo_Contato = value.Tipo_Contato;
                            
                            await repository.Update(modelo);

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



        [Route("api/Organizacaoapi/ExcluirContatoGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirContatoGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Organizacao_Contato>(session))
                    {

                        Organizacao_Contato modelo = null;
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



    }
}
