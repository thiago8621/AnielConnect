using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Bibliography;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
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
    public class Perfil_VendedorApiController : BaseApiController
    {
        [HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/Perfil_Vendedorapi/GetAll")]
        public async Task<DataTableResponse<Perfil_VendedorModels>> GetAll(Perfil_Vendedor_DataTableRequest request)
        {
            List<Perfil_VendedorModels> retorno = new List<Perfil_VendedorModels>();
            IOrderedEnumerable<Perfil_VendedorModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder("* FROM TB_PERFIL_VENDEDOR A");

                //Dictionary<String, Object> parametros = new Dictionary<string, object>();
                //GetAllFiltros(request, sqlFrom, parametros);
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PERFIL_VENDEDOR");
                    

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
                    sql.AppendLine("SELECT A.ID ,A.DESCRICAO ,A.AREA_COBERTURA ,A.CATEGORIA_CLIENTE ,A.CAMPANA_REGIAO ,A.TIPO_PESSOA ,A.MEIO_CAPITACAO,A.STATUS ");
                    sql.AppendLine("FROM TB_PERFIL_VENDEDOR A");
                  
                    sql.AppendLine("WHERE A.ID>0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }
                    if (!String.IsNullOrEmpty(request.id_Status))                                         
                    {
                        sql.AppendLine("AND A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.id_Status);
                    }

                    if (!String.IsNullOrEmpty(request.id_Area_Cobertura))                                       
                    {
                        sql.AppendLine("AND A.AREA_COBERTURA  = @TIPO_AREA ");
                        parametros.Add("@TIPO_AREA ", request.id_Area_Cobertura);
                    }
                    if (!String.IsNullOrEmpty(request.id_Tipo_Pessoa))                     
                    {
                        sql.AppendLine("AND A.TIPO_PESSOA = @TIPO_PESSOA");
                        parametros.Add("@TIPO_PESSOA", request.id_Tipo_Pessoa);
                    }
                    if (!String.IsNullOrEmpty(request.id_Categoria_Cliente))                      
                    {
                        sql.AppendLine("AND A.CATEGORIA_CLIENTE = @TIPO_CATEGORIA");
                        parametros.Add("@TIPO_CATEGORIA", request.id_Categoria_Cliente);
                    }
                    if (!String.IsNullOrEmpty(request.id_Meio_Captacao))
                    {
                        sql.AppendLine("AND A.MEIO_CAPITACAO = @MEIO_CAPITACAO");
                        parametros.Add("@MEIO_CAPITACAO", request.id_Meio_Captacao);
                    }
                    if (!String.IsNullOrEmpty(request.id_Regiao))
                    {
                        sql.AppendLine("AND A.CAMPANA_REGIAO = @CAMPANA_REGIAO");
                        parametros.Add("@CAMPANA_REGIAO", request.id_Regiao);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Perfil_VendedorModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao= Convert.ToString(reader["DESCRICAO"]),
                                Tipo_Especificas = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPO_PESSOA"])),
                                Regioes_Espeficias = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["AREA_COBERTURA"])),
                                Categoria_Especificas = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["CATEGORIA_CLIENTE"])),
                                MeioEspecificos = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["MEIO_CAPITACAO"])),
                                Area_Especificas = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["AREA_COBERTURA"])),

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

                   
                    case 3:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }                    
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Perfil_VendedorModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Perfil_VendedorModels>>> GetList()
        {
            GetApiRetorno<IEnumerable<Perfil_VendedorModels>> retorno = new GetApiRetorno<IEnumerable<Perfil_VendedorModels>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Atendimento = new Conexao<Perfil_Vendedor>(session))
            {
                try
                {
                    var lista = await repoTipo_Atendimento.select(p => p.Status == Status_Default.Ativo);

                    var listaRetorno = new List<Perfil_VendedorModels>();

                    foreach (var obj in lista)
                    {
                        Perfil_VendedorModels tmp = new Perfil_VendedorModels
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


        private async Task BindViewToObj(ISession session, Perfil_Vendedor objBanco, Perfil_VendedorCadastroModel view)
        {
            using (var repository = new Conexao<Perfil_Vendedor>(session))
            {
                
                objBanco.Descricao = view.Descricao;             
                objBanco.Area_Cobertura = view.Area_Especificas;             
                objBanco.Tipo_Pessoa = view.Tipo_Especificas;             
                objBanco.Campanha_Regiao = view.Regioes_Espeficias;             
                objBanco.Categoria_Cliente = view.Categoria_Especificas;             
                objBanco.Meio_Capitacao = view.MeioEspecificos;             
                objBanco.Autorizacao_Vendas = view.Autorizacao_Vendas;             
                
            }
        }

        [HttpPost()]
        [Route("api/Perfil_Vendedorapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PERFIL_VENDEDOR SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Perfil_VendedorApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_PERFIL_VENDEDOR WHERE ID IN {_ids}");

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

        [Route("api/Perfil_VendedorApi/ExcluirGridTipoPessoa")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridTipoPessoa(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Pessoa_Vendedor>(session))
                    {

                        Tipo_Pessoa_Vendedor modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            if (modelo == null)
                            {
                                throw new Exception(Resources.RsBase.NAOENCONTRADO);
                            }

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
                retorno.Ok = false;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/Perfil_VendedorApi/ExcluirTipoMeio")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirTipoMeio([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Meio_Captacao_Vendedor>(session))
                    {

                        Meio_Captacao_Vendedor modelo = null;
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



        [Route("api/Perfil_VendedorApi/ExcluirGridCategoria")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridCategoria(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Categoria_Vendedor>(session))
                    {

                        Tipo_Categoria_Vendedor modelo = null;
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



        [Route("api/Perfil_VendedorApi/ExcluirTipoRegiaoGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirTipoRegiaoGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Endereco_Vendedor>(session))
                    {

                        Tipo_Endereco_Vendedor modelo = null;
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
         
        [Route("api/Perfil_VendedorApi/ExcluirTipoCategoriaGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirTipoCategoriaGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Categoria_Vendedor>(session))
                    {

                        Tipo_Categoria_Vendedor modelo = null;
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

        [Route("api/Perfil_VendedorApi/ExcluirTipoAreaGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirTipoAreaGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Tipo_Area_Vendedor>(session))
                    {

                        Tipo_Area_Vendedor modelo = null;
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

        [HttpGet()]
        [Route("api/Perfil_VendedorApi/FindById")]
        public async Task<GetApiRetorno<Perfil_VendedorCadastroModel>> FindById(Int64 Id, IEnumerable<Tipo_Endereco_Vendedor> merioEnderenco)
        {
            GetApiRetorno<Perfil_VendedorCadastroModel> retorno = new GetApiRetorno<Perfil_VendedorCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Perfil_Vendedor>(session))
            using (var repositoryTipoPessoa = new Conexao<Tipo_Pessoa_Vendedor>(session))
            using (var repositoryPlanoProduto = new Conexao<Plano_Produto_Cobranca>(session))
            using (var repositoryMeioCaptacaoVendedor = new Conexao<Meio_Captacao_Vendedor>(session))
            using (var repositoryMeioCaptacao = new Conexao<Meio_Captacao>(session))  
            using (var repositoryCategoria = new Conexao<Categoria_Cliente>(session))
            using (var repositoryTipoCategoria = new Conexao<Tipo_Categoria_Vendedor>(session))
            using (var repositoryEnderecoVendedor = new Conexao<Tipo_Endereco_Vendedor>(session))
            using (var repositoryEndereco = new Conexao<Endereco_Cliente>(session))
            using (var repositoryAreaVendedor = new Conexao<Tipo_Area_Vendedor>(session))
            using (var repositoryArea = new Conexao<Area_Cobertura>(sessionT5))
            using (var repoCidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(obj: await repository.selectFirst(p => p.Id == Id));
                    var perfilVendedor = await repositoryTipoPessoa.select(p => p.Perfil_Vendedor.Id == Id);

                    retorno.Objeto.TipoPessoa = perfilVendedor.Select(p =>
                    {
                        var perfilVenda = repositoryTipoPessoa.select(y => y.Perfil_Vendedor.Id == p.Id).Result;
                        var obj = new Perfil_VendedorTipoPessoaCadastroModel();
                        obj.Id = p.Id;
                        obj.Tipo_Pessoa = p.Tipo_Pessoa;
                        obj.Tipo_Pessoa_Desc = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(p.Tipo_Pessoa));

                        return obj;

                    }).ToList();

                    var meioCaptacao = await repositoryMeioCaptacaoVendedor.select(y => y.Perfil_Vendedor.Id == Id);
                    retorno.Objeto.MeioCaptacao = meioCaptacao.Select(y =>
                    {
                        var objcaptacao = new Perfil_VendedorMeioCaptacaoCadastroModel();
                        objcaptacao.Id = y.Id;
                        objcaptacao.Meio_Captacao = Convert.ToString(y.Meio_Captacao);
                        objcaptacao.Desc_Meio_Captacao = String.IsNullOrEmpty(y.Meio_Captacao) ? String.Empty : repositoryMeioCaptacao.selectFirst(x => x.Id == Convert.ToInt32(y.Meio_Captacao)).Result?.Descricao;


                        return objcaptacao;

                    }).ToList();

                    var meioCategoria = await repositoryTipoCategoria.select(x => x.Perfil_Vendedor.Id == Id);
                    retorno.Objeto.TipoCategoria = meioCategoria.Select(z =>
                    {
                        var objCategoria = new Perfil_VendedorTipoCategoriaCadastroModel();
                        objCategoria.Id = z.Id;
                        objCategoria.Tipo_Categoria = Convert.ToString(z.Tipo_Categoria);
                        objCategoria.Tipo_Categoria_Desc = String.IsNullOrEmpty(z.Tipo_Categoria) ? String.Empty : repositoryCategoria.selectFirst(x => x.Id == Convert.ToInt32(z.Tipo_Categoria)).Result?.Descricao;
                        return objCategoria;

                    }).ToList();

                    var meioEnderenco = await repositoryEnderecoVendedor.select(x => x.Perfil_Vendedor.Id == Id);
                    retorno.Objeto.TipoEndereco = meioEnderenco.Select(z =>
                    {
                        var objEnderenco = new Perfil_VendedorTipoEnderecoCadastroModel();
                        objEnderenco.Id = z.Id;
                        objEnderenco.Pais = Convert.ToString(z.Pais);
                        objEnderenco.Pais_Desc = String.IsNullOrEmpty(z.Pais) ? String.Empty : repoPais.selectFirst(x => x.Id == Convert.ToInt32(z.Pais)).Result?.Nome;
                        objEnderenco.Cidade = Convert.ToString(z.Cidade);
                        objEnderenco.Cidade_Desc = String.IsNullOrEmpty(z.Cidade) ? String.Empty : repoCidade.selectFirst(x => x.Id == Convert.ToInt32(z.Cidade)).Result?.Descricao;
                        objEnderenco.Uf = Convert.ToString(z.Uf);
                        objEnderenco.Uf_Desc = String.IsNullOrEmpty(z.Uf) ? String.Empty : repoUf.selectFirst(x => x.Id == Convert.ToInt32(z.Uf)).Result?.Descricao;
                        
                        return objEnderenco;

                    }).ToList();

                    var meioArea = await repositoryAreaVendedor.select(x => x.Perfil_Vendedor.Id == Id);
                    retorno.Objeto.TipoArea = meioArea.Select(z =>
                    {
                        var objArea = new Perfil_VendedorTipoAreaCadastroModel();
                        objArea.Id = z.Id;
                        objArea.Tipo_Area = Convert.ToString(z.Tipo_Area);
                        objArea.Tipo_Area_Desc = String.IsNullOrEmpty(z.Tipo_Area) ? String.Empty : repositoryArea.selectFirst(x => x.Id == Convert.ToInt32(z.Tipo_Area)).Result?.Descricao;
                        return objArea;

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
        [Route("api/Perfil_VendedorApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Perfil_VendedorCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Perfil_Vendedor modelo = null;
                Perfil_Vendedor modeloAnterior = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (ISession sessionLog = HibernateUtil.GetSessionLog())
                    using (ISession sessionUser = HibernateUtil.GetSession())
                    using (var repository = new Conexao<Perfil_Vendedor>(session))
                    using (var repositoryEncerramentoAtendimento = new Conexao<EncerramentoAtendimento>(session))
                    using (var repositoryTipoPessoa = new Conexao<Tipo_Pessoa_Vendedor>(session))
                    using (var repositoryTipoCategoria = new Conexao<Tipo_Categoria_Vendedor>(session))
                    using (var repositoryTipoEndereco = new Conexao<Tipo_Endereco_Vendedor>(session))
                    using (var repositoryTipo_Servico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                    using (var repositoryMeioCaptcao = new Conexao<Meio_Captacao_Vendedor>(session))
                    using (var repositoryArea = new Conexao<Tipo_Area_Vendedor>(session))
                    using (var repositoryProduto = new Conexao<Produto>(session))
                    using (var repoUser = new Conexao<IdentityUser>(sessionUser))
                    {
                        IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == value.Usuario));

                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo != null)
                        {                            
                            modeloAnterior = (Perfil_Vendedor)modelo.Clone();
                        }

                        if (modelo == null)
                        {
                            modelo = new Perfil_Vendedor
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
                        foreach (var pessoa in value.Objeto.TipoPessoa)
                        {
                            var tipo = new Tipo_Pessoa_Vendedor
                            {
                                Id = pessoa.Id,
                                Tipo_Pessoa = pessoa.Tipo_Pessoa,

                                Perfil_Vendedor = modelo

                            };

                            await repositoryTipoPessoa.SaveOrUpdate(tipo);

                        }

                        await repository.SaveOrUpdate(modelo);

                        foreach (var endereco in value.Objeto.TipoEndereco)
                        {
                            var tipo = new Tipo_Endereco_Vendedor
                            {
                                Id = endereco.Id,
                                Pais = endereco.Pais,
                                Cidade = endereco.Cidade,
                                Uf = endereco.Uf,
                                Perfil_Vendedor = modelo
                            };

                            await repositoryTipoEndereco.SaveOrUpdate(tipo);

                        }
                        foreach (var meio in value.Objeto.MeioCaptacao)
                        {
                            var tipo = new Meio_Captacao_Vendedor
                            {
                                Id = meio.Id,
                                Meio_Captacao = meio.Meio_Captacao,
                                Perfil_Vendedor = modelo
                            };

                            await repositoryMeioCaptcao.SaveOrUpdate(tipo);

                        }
                        foreach (var categoria in value.Objeto.TipoCategoria)
                        {
                            var tipo = new Tipo_Categoria_Vendedor
                            {
                                Id = categoria.Id,
                                Tipo_Categoria = categoria.Tipo_Categoria,
                                Perfil_Vendedor = modelo
                            };

                            await repositoryTipoCategoria.SaveOrUpdate(tipo);

                        }

                        foreach (var area in value.Objeto.TipoArea)
                        {
                            var tipo = new Tipo_Area_Vendedor
                            {
                                Id = area.Id,
                                Tipo_Area = area.Tipo_Area,
                                Perfil_Vendedor = modelo
                            };

                            await repositoryArea.SaveOrUpdate(tipo);

                        }


                        //  log
                        try
                        {
                            if (modeloAnterior != null)
                            {
                                using (var repositoryLogs = new Conexao<LogHistoricoVenda>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                            else
                            {
                                using (var repositoryLogs = new Conexao<LogHistoricoVenda>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, usuario.Id, usuario.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.IdObjeto = 0;
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
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

       

        [HttpGet()]
        [Route("api/Perfil_VendedorApi/busca")]
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
                        sql.AppendLine("FROM TB_PERFIL_VENDEDOR A");
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
        [Route("api/Perfil_VendedorApi/HistoricoAlteracao")]
        public async Task<DataTableResponse<LogHistoricoVendaModel>> HistoricoAlteracao(HistoricoAlteracaoPerfil_Vendedor_DataTableRequest request)
        {
            List<LogHistoricoVendaModel> retornoLog = new List<LogHistoricoVendaModel>();
            List<LogHistoricoVendaModel> retorno = new List<LogHistoricoVendaModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_HISTORICO_VENDAlOG A ");

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
                    parametros.Add("@ID", request.Id);
                }

                if (request.TipoOperacao != null)
                {
                    sqlFrom.AppendLine(" AND A.TIPOOPERACAO = @TIPOOPERACAO ");
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Objeto))
                {
                    sqlFrom.AppendLine(" AND A.OBJETO = @OBJETO ");
                    parametros.Add("@OBJETO", request.Objeto);
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFrom.AppendLine(" AND A.CAMPO = @CAMPO ");
                    parametros.Add("@CAMPO", request.Campo);
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(COD_PERFIL_VENDEDOR) CO ");

                    sql.Append(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.COD_PERFIL_VENDEDOR, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS, A.OBSERVACAO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // CHAVE
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY OBJETO ");
                                    else
                                        sql.Append(" ORDER BY OBJETO DESC ");
                                    break;
                                }
                            case 1:     // CAMPO
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY CAMPO ");
                                    else
                                        sql.Append(" ORDER BY CAMPO DESC ");
                                    break;
                                }
                            case 6: //Data Hora
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY DATAHORACRIACAO ");
                                    else
                                        sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
                                    break;
                                }
                        }
                    }

                    sql.Append(" ORDER BY DATAHORACRIACAO DESC ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retornoLog.Add(new LogHistoricoVendaModel
                            {
                                Id = Convert.ToInt64(reader["COD_PERFIL_VENDEDOR"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Observacao = Convert.ToString(reader["OBSERVACAO"]),
                            });

                        }
                    }

                }
            }

            foreach (var ret in retornoLog)
            {
                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Importacao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Insercao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Alteracao)))
                {
                    using ISession session = HibernateUtil.GetSessionT5();
                    if (ret.Campo == "Grupo Área")
                    {
                        using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                        {
                            var antes = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorAntes));
                            var depois = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }
                }

                retorno.Add(ret);
            }

            //Page e retornando DataTable
            return new DataTableResponse<LogHistoricoVendaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }


        [HttpPost()]
        [Route("api/Perfil_VendedorApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PERFIL_VENDEDOR A WHERE ");

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
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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

        private Perfil_VendedorCadastroModel GetViewObj(Perfil_Vendedor obj)
        {
            return new Perfil_VendedorCadastroModel
            {
                Id = obj.Id,
               
                Descricao = obj.Descricao,
                Area_Especificas = obj.Area_Cobertura,
                Tipo_Especificas = obj.Tipo_Pessoa,
                Regioes_Espeficias = obj.Campanha_Regiao,
                Categoria_Especificas = obj.Categoria_Cliente,
                MeioEspecificos = obj.Meio_Capitacao,
                Autorizacao_Vendas = obj.Autorizacao_Vendas,
           
               
                Status = obj.Status == Status_Default.Ativo,
               
            };
        }



    }
}