using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Hql.Ast;
using Sinapse.DAO.repository;
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
    public class POPApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/POPApi/GetAll")]
        public async Task<DataTableResponse<POPListModel>> GetAll(POPDatatableRequest request)
        {
            List<POPListModel> retorno = new List<POPListModel>();
            IOrderedEnumerable<POPListModel> listaOrdenada = null;
            int recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlFrom = new StringBuilder();

                sql.AppendLine(" SELECT A.ID, A.NOME, B.RAZAO_SOCIAL AS EMPRESA, C.NOME AS FILIAL, A.CODCT, A.PROJETO, A.PAIS,");
                sql.AppendLine(" A.CIDADE, A.UF, A.ENDERECO, A.CEP, A.LATITUDE, A.LONGITUDE, A.STATUS ");

                sqlFrom.AppendLine(" FROM TB_POP A ");

                sqlFrom.AppendLine(" LEFT JOIN TB_EMPRESA B ");
                sqlFrom.AppendLine(" ON(B.ID = A.ID_EMPRESA) ");

                sqlFrom.AppendLine(" LEFT JOIN TB_FILIAL C ");
                sqlFrom.AppendLine(" ON(C.ID = A.ID_FILIAL) ");


                #region Filtros

                if (request.IdEmpresa.HasValue)
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND B.ID = @ID_EMPRESA ");
                    else
                        sqlFrom.AppendLine(" WHERE B.ID = @ID_EMPRESA ");

                    parametros.Add("@ID_EMPRESA", request.IdEmpresa.Value);
                }

                if (request.IdFilial.HasValue)
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND C.ID = @ID_FILIAL ");
                    else
                        sqlFrom.AppendLine(" WHERE C.ID = @ID_FILIAL ");

                    parametros.Add("@ID_FILIAL", request.IdFilial.Value);
                }

                if (!String.IsNullOrEmpty(request.CodCt))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND A.CODCT = @CODCT ");
                    else
                        sqlFrom.AppendLine(" WHERE A.CODCT = @CODCT ");

                    parametros.Add("@CODCT", request.CodCt);
                }

                if (!String.IsNullOrEmpty(request.Projeto))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND A.PROJETO = @PROJETO ");
                    else
                        sqlFrom.AppendLine(" WHERE A.PROJETO = @PROJETO ");

                    parametros.Add("@PROJETO", request.Projeto);
                }

                if (request.IdPais.HasValue) 
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND A.ID_PAIS = @PAIS ");
                    else
                        sqlFrom.AppendLine(" WHERE A.ID_PAIS = @PAIS ");

                    parametros.Add("@PAIS", request.IdPais);

                }
                if (!String.IsNullOrEmpty(request.UF))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND A.UF = @UF ");
                    else
                        sqlFrom.AppendLine(" WHERE A.UF = @UF ");

                    parametros.Add("@UF", request.UF);

                    if (request.IdCidade.HasValue)
                    {
                        sqlFrom.AppendLine(" AND A.ID_CIDADE = @ID_CIDADE ");
                        parametros.Add("@ID_CIDADE", request.IdCidade.Value);

                        if (request.IdBairro.HasValue)
                        {
                            sqlFrom.AppendLine(" AND A.ID_BAIRRO = @ID_BAIRRO ");
                            parametros.Add("@ID_BAIRRO", request.IdBairro.Value);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND UPPER(A.NOME) LIKE UPPER(@NOME) ");
                    else
                        sqlFrom.AppendLine(" WHERE UPPER(A.NOME) LIKE UPPER(@NOME) ");

                    parametros.Add("@NOME", "%" + request.search.value.ToUpper() + "%");
                }
                            
                #endregion

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sqlCount.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                sql.AppendLine(sqlFrom.ToString());
                sql.AppendLine(" ORDER BY A.ID ");

                using (var com = session.Connection.CreateCommand())
                using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new POPListModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Nome = Convert.ToString(reader["NOME"]),

                            Empresa = Convert.ToString(reader["EMPRESA"]),
                            Filial = Convert.ToString(reader["FILIAL"]),

                            Contrato = Convert.ToString(reader["CODCT"]),
                            Projeto = Convert.ToString(reader["PROJETO"]),

                            Pais = Convert.ToString(reader["PAIS"]),
                            Cidade = Convert.ToString(reader["CIDADE"]),
                            UF = Convert.ToString(reader["UF"]),

                            Endereco = Convert.ToString(reader["ENDERECO"]),
                            Cep = Convert.ToString(reader["CEP"]),

                            Latitude = Convert.ToString(reader["LATITUDE"]),
                            Longitude = Convert.ToString(reader["LONGITUDE"]),

                            Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),

                        });
                    }
                }
            }


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

                    case "empresa":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Empresa);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Empresa);
                            break;
                        }

                   case "filial":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Filial);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Filial);
                            break;
                        }

                   case "contrato":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Contrato);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                    
                   case "projeto":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Projeto);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Projeto);
                            break;
                        }    
                        
                   case "pais":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Pais);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Pais);
                            break;
                        }     

                   case "cidade":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cidade);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cidade);
                            break;
                        }  
                        
                   case "uf":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.UF);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.UF);
                            break;
                        }
                              
                   case "endereco":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Endereco);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Endereco);
                            break;
                        }
                              
                   case "cep":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cep);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cep);
                            break;
                        }
                              
                   case "latitude":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Latitude);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Latitude);
                            break;
                        }  
                        
                   case "longitude":
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Longitude);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Longitude);
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


            return new DataTableResponse<POPListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private POPCadastroModel GetViewObj(POP objBanco)
        {
            POPCadastroModel model = new POPCadastroModel();

            model.Id = objBanco.Id;
            model.Nome = objBanco.Nome;
            model.CodigoAuxiliar = objBanco.CodigoAuxiliar;

            model.IdEmpresa = objBanco?.Empresa?.CodEmpresa;
            model.IdFilial = objBanco?.Filial?.CodFilial;

            model.CodCt = objBanco.CodCt;
            model.Projeto = objBanco.Projeto;

            #region Endereço

            model.Cep = objBanco.Cep;

            model.Endereco = objBanco.Endereco;
            model.Numero = objBanco.Numero;

            model.IdCidade = objBanco.IdCidade;
            model.Cidade = objBanco.Cidade;
            //model.IdBairro = objBanco.IdBairro;
            model.Bairro = objBanco.Bairro;

            model.UF = objBanco.UF;
            model.IdPais = objBanco.IdPais;
            model.Pais = objBanco.Pais;
            model.Complemento = objBanco.Complemento;

            model.Latitude = Convert.ToString(objBanco.Latitude);
            model.Longitude = Convert.ToString(objBanco.Longitude);
            #endregion

            model.Status = objBanco.Status == Sinapse.Model.enumerados.Status_Default.Ativo;

            return model;
        }

        private async Task BindViewToObj(ISession session, POP objBanco, POPCadastroModel view)
        {
            using (var repoEmpresa = new Conexao<Empresa>(session))
            using (var repoFilial = new Conexao<Filial>(session))
            using (EnderecoApiController apiEndereco = new EnderecoApiController())
            {
                objBanco.Nome = view.Nome;
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;

                objBanco.Empresa = await repoEmpresa.selectFirst(p => p.Id == view.IdEmpresa);
                objBanco.Filial = await repoFilial.selectFirst(p => p.CodFilial == view.IdFilial && p.Empresa.Id== view.IdEmpresa);

                objBanco.CodCt = view.CodCt;
                objBanco.Projeto = view.Projeto;

                #region Endereço

                objBanco.Cep = view.Cep;

                objBanco.Endereco = view.Endereco;
                objBanco.Numero = view.Numero;

                objBanco.IdCidade = view.IdCidade;
                var cidadePeloCep = await apiEndereco.GetEnderecoByCep(view.Cep);
                objBanco.Cidade = cidadePeloCep.Objeto.Cidade;

                objBanco.IdPais = view.IdPais;
                objBanco.Pais = view.Pais;

                //objBanco.IdBairro = view.IdBairro;
                objBanco.Bairro = view.Bairro;

                objBanco.UF = view.UF;
                objBanco.Complemento = view.Complemento;

                objBanco.Latitude = String.IsNullOrEmpty(view.Latitude) ? (double?)null : Convert.ToDouble(view.Latitude.Replace(".", ","));
                objBanco.Longitude = String.IsNullOrEmpty(view.Longitude) ? (double?)null : Convert.ToDouble(view.Longitude.Replace(".", ","));

                #endregion
            }
        }

        [HttpPost]
        [Route("api/POPApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<POPCadastroModel> param)
        {
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPOP = new Conexao<POP>(session))
                {
                    POP model = await repoPOP.selectFirst(p => p.Id == param.Objeto.Id);

                    if (model == null)
                    {
                        model = new POP
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = param.Usuario,
                            Status = Sinapse.Model.enumerados.Status_Default.Ativo
                        };
                    }
                    else
                    {
                        model.DataHoraAlteracao = DateTime.Now;
                        model.UsuarioAlteracao = param.Usuario;
                        model.Status = param.Objeto.Status ?Sinapse.Model.enumerados.Status_Default.Ativo:Sinapse.Model.enumerados.Status_Default.Inativo;
                    }

                    await BindViewToObj(session, model, param.Objeto);

                    await repoPOP.SaveOrUpdate(model);

                    retorno.Ok = true;
                    retorno.IdObjeto = model.Id;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/POPapi/FindById")]
        public async Task<GetApiRetorno<POPCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<POPCadastroModel> retorno = new GetApiRetorno<POPCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<POP>(session))
                {
                    POP model = await repository.selectFirst(p => p.Id == Id);

                    retorno.Objeto = GetViewObj(model);
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/POPapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_POP A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos) 
                    {
                        condicoes.Add("COALESCE(A.STATUS,0) <> 0");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NOME != '' AND ( {String.Join(" AND ", condicoes)} ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME  ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.NOME) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NOME"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["NOME"])
                                    });
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
        [Route("api/POPapi/GetContratoProjeto")]
        public async Task<GetApiRetorno<POP>> GetContratoProjeto(Int64? popId)
        {
            GetApiRetorno<POP> retorno = new GetApiRetorno<POP>();
            retorno.Ok = true;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder(" SELECT A.ID, A.NOME, A.CODCT, A.PROJETO FROM TB_POP A ");
                    sql.AppendLine(" WHERE A.ID = @ID ");

                    parametros.Add("@ID", popId);

                    using (var com = session.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Objeto = new POP
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    Nome = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : null,
                                    CodCt = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : null,
                                    Projeto = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

    }
}