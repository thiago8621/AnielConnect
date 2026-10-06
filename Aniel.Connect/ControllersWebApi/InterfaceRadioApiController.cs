using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Sinapse.Util.RemoteConnection;
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
    public class InterfaceRadioApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/InterfaceRadioApi/GetAll")]
        public async Task<DataTableResponse<InterfaceRadioListaModel>> GetAll(InterfaceRadio_DataTableRequest request)
        {

            List<InterfaceRadioListaModel> retorno = new List<InterfaceRadioListaModel>();
            IOrderedEnumerable<InterfaceRadioListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Sql From
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_INTERFACE_RADIO A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_PONTO_ACESSO_RADIO C ON A.ID_PONTO_ACESSO_RADIO = C.ID ");
                sqlFrom.AppendLine(" WHERE A.ID > 0");

                //Filtrando                    
                //Filtrando                    
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "interface_radio")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "ponto_acesso")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(C.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }

                if (!String.IsNullOrEmpty(request.Id_Ponto))
                {
                    sqlFrom.AppendLine("AND (C.ID = @IDPONTOACESSO)");
                    parametros.Add("@IDPONTOACESSO", Convert.ToInt32(request.Id_Ponto));
                }


                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.CODIGO_AUXILIAR, C.DESCRICAO AS DESCRICAO_PONTO_ACESSO ");
                    sql.Append(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new InterfaceRadioListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo_Auxiliar = Convert.ToString(reader["CODIGO_AUXILIAR"]),
                                Ponto_Acesso_Radio = Convert.ToString(reader["DESCRICAO_PONTO_ACESSO"])
                               
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
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Id);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<InterfaceRadioListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/InterfaceRadioApi/GetTreeView")]
        public async Task<GetApiRetorno<List<InterfaceRadioTreeViewModel>>> GetTreeView(Int64? IdInterface)
        {
            GetApiRetorno<List<InterfaceRadioTreeViewModel>> retorno = new GetApiRetorno<List<InterfaceRadioTreeViewModel>>
            {
                Objeto = new List<InterfaceRadioTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                        StringBuilder sql = new StringBuilder($"SELECT ID, DESCRICAO FROM TB_INTERFACE_RADIO ");
                     

                        if (IdInterface.HasValue)
                        {
                            sql.AppendLine(" AND ID = @IDINTERFACE");
                            parametros.Add("@IDINTERFACE", IdInterface);
                        }

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new InterfaceRadioTreeViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"])
                            });
                        }

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

        private InterfaceRadioCadastroModel GetViewObj(InterfaceRadio obj)
        {

          

            return new InterfaceRadioCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,              
                Codigo_Auxiliar = obj.Codigo_Auxiliar,     
                Id_Ponto_Acesso_Radio = obj.Ponto_Acesso_Radio?.Id

            };

        }


        private async Task BindViewToObj(ISession session, InterfaceRadio objBanco, InterfaceRadioCadastroModel view)
        {
            using (var repoPontoAcessoRadio = new Conexao<Ponto_Acesso_Radio>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Codigo_Auxiliar = view.Codigo_Auxiliar;
                objBanco.Ponto_Acesso_Radio = view.Id_Ponto_Acesso_Radio.HasValue ? await repoPontoAcessoRadio.selectFirst(p => p.Id == view.Id_Ponto_Acesso_Radio) : null;


            }
        }


        [HttpPost()]
        [Route("api/InterfaceRadioApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<InterfaceRadioCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                InterfaceRadio modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<InterfaceRadio>(session))                   
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new InterfaceRadio
                            {
                                UsuarioCriacao = value.Usuario,
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);
                      
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
        [Route("api/InterfaceRadioApi/FindById")]
        public async Task<GetApiRetorno<InterfaceRadioCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<InterfaceRadioCadastroModel> retorno = new GetApiRetorno<InterfaceRadioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<InterfaceRadio>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

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
        [Route("api/InterfaceRadioApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_INTERFACE_RADIO A ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = $"{reader["DESCRICAO"]}"
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    throw ex;
                }
            }
            return retorno;

        }





        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<InterfaceRadioListaModel>>> GetListInterfaceRadio()
        {
            GetApiRetorno<IEnumerable<InterfaceRadioListaModel>> retorno = new GetApiRetorno<IEnumerable<InterfaceRadioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoInterface = new Conexao<InterfaceRadio>(session))
            {

                try
                {
                    var lista = await repoInterface.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new InterfaceRadioListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,


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


      

    }

}