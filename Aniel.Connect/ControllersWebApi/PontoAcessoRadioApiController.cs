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
    public class PontoAcessoRadioApiController : BaseApiController
    {
        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>> retorno = new GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Ponto_Acesso_Radio>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new PontoAcessoRadioListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,


                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/PontoAcessoRadioApi/GetAll")]
        public async Task<DataTableResponse<PontoAcessoRadioListaModel>> GetAll(PontoAcessoRadio_Request request)
        {

            List<PontoAcessoRadioListaModel> retorno = new List<PontoAcessoRadioListaModel>();
            IOrderedEnumerable<PontoAcessoRadioListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Sql From
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_PONTO_ACESSO_RADIO A ");
                sqlFrom.AppendLine(" WHERE A.ID > 0");

                //Filtrando                    
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "ip")
                    {
                        sqlFrom.AppendLine(" AND (A.IP LIKE @SEARCH) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "pontoAcessoRadio")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }


                if (!String.IsNullOrEmpty(request.Id_Ponto))
                {
                    sqlFrom.AppendLine("AND (A.ID = @IDPONTO)");
                    parametros.Add("@IDPONTO", Convert.ToInt32(request.Id_Ponto));
                }

                if (!String.IsNullOrEmpty(request.Ip))
                {
                    sqlFrom.AppendLine("AND (A.IP = @IP)");
                    parametros.Add("@IP", request.Ip);
                }


                if (!String.IsNullOrEmpty(request.Status_Filtro))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
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


                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.MAC, A.IP, A.STATUS ");
                    sql.Append(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PontoAcessoRadioListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Mac = Convert.ToString(reader["MAC"]),
                                Ip = Convert.ToString(reader["IP"]),
                                Situacao = reader["STATUS"] != DBNull.Value ? Convert.ToInt64(reader["STATUS"]) == 1 ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO : String.Empty

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
            return new DataTableResponse<PontoAcessoRadioListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpGet()]
        [Route("api/PontoAcessoRadioApi/GetTreeView")]
        public async Task<GetApiRetorno<List<PontoAcessoRadioTreeViewModel>>> GetTreeView(Int64? Id_Ponto_Acesso)
        {
            GetApiRetorno<List<PontoAcessoRadioTreeViewModel>> retorno = new GetApiRetorno<List<PontoAcessoRadioTreeViewModel>>
            {
                Objeto = new List<PontoAcessoRadioTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>
                        {
                            { "@ID_PONTO_ACESSO", Id_Ponto_Acesso }
                        };
                        StringBuilder sql = new StringBuilder($"SELECT ID, DESCRICAO FROM TB_PONTO_ACESSO_RADIO WHERE ID = @ID_PONTO_ACESSO");
                       

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new PontoAcessoRadioTreeViewModel
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
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        private PontoAcessoRadioCadastroModel GetViewObj(Ponto_Acesso_Radio obj)
        {
            var listaGruposAreaView = new List<Ponto_Acesso_Radio_Grupo_AreaModels>();
            if (obj.GruposArea != null && obj.GruposArea.Count > 0)
            {
                foreach (var grupo in obj.GruposArea)
                {
                    if (grupo.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        listaGruposAreaView.Add(new Ponto_Acesso_Radio_Grupo_AreaModels
                        {
                            Id = grupo.Id,
                            Cod_Area = grupo.Cod_Area,
                            Cod_Escritorio = grupo.Cod_Escritorio,
                            Desc_Area = grupo.Desc_Area,
                            Desc_Escritorio = grupo.Desc_Escritorio,
                        });
                }
            }

            return new PontoAcessoRadioCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Codigo_Auxiliar = obj.Codigo_Auxiliar,
                Mac = obj.Mac,
                Ip = obj.Ip,
                Status = obj.Status == Status_Default.Ativo,
                GruposArea = listaGruposAreaView

            };
        }

        private async Task BindViewToObj(ISession session, Ponto_Acesso_Radio objBanco, PontoAcessoRadioCadastroModel view)
        {
            using (var repoPontoAcessoRadio = new Conexao<Ponto_Acesso_Radio>(session))
            using (var repoInterface = new Conexao<InterfaceRadio>(session))
            {
                objBanco.Descricao = view.Descricao;              
                objBanco.Codigo_Auxiliar = view.Codigo_Auxiliar;
                objBanco.Ip = view.Ip;
                objBanco.Mac = view.Mac;

            }
        }

        private async Task BindViewToObjGrupoArea(ISession session, Ponto_Acesso_Radio objBanco, PontoAcessoRadioCadastroModel view, List<Ponto_Acesso_Radio_Grupo_Area> listagruposArea)
        {
            foreach (var grupoArea in view.GruposArea)
            {
                if (grupoArea.Id == 0)
                {
                    listagruposArea.Add(new Ponto_Acesso_Radio_Grupo_Area
                    {
                        Cod_Escritorio = grupoArea.Cod_Escritorio,
                        Cod_Area = grupoArea.Cod_Area,
                        Desc_Escritorio = grupoArea.Desc_Escritorio,
                        Desc_Area = grupoArea.Desc_Area,
                        Ponto_Acesso_Radio = objBanco,
                        Status = Sinapse.Model.enumerados.Status_Default.Ativo
                    });
                }
            }
        }


        [HttpPost()]
        [Route("api/PontoAcessoRadioApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<PontoAcessoRadioCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Ponto_Acesso_Radio modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Acesso_Radio>(session))
                    using (var repoPontoAcessoRadioGrupoArea = new Conexao<Ponto_Acesso_Radio_Grupo_Area>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Ponto_Acesso_Radio
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        List<Ponto_Acesso_Radio_Grupo_Area> listagruposArea = new List<Ponto_Acesso_Radio_Grupo_Area>();
                        await BindViewToObjGrupoArea(session, modelo, value.Objeto, listagruposArea);

                        await repoPontoAcessoRadioGrupoArea.SaveOrUpdate(listagruposArea);

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
        [Route("api/PontoAcessoRadioApi/FindById")]
        public async Task<GetApiRetorno<PontoAcessoRadioCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<PontoAcessoRadioCadastroModel> retorno = new GetApiRetorno<PontoAcessoRadioCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Ponto_Acesso_Radio>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [Route("api/PontoAcessoRadioApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Ponto_Acesso_Radio>(session))
                    {

                        Ponto_Acesso_Radio modelo = null;
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
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>>> GetListIp()
        {
            GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>> retorno = new GetApiRetorno<IEnumerable<PontoAcessoRadioListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoInterface = new Conexao<Ponto_Acesso_Radio>(session))
            {

                try
                {

                    var lista = await repoInterface.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new PontoAcessoRadioListaModel
                    {
                        Ip = p.Ip,


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


        [HttpPost()]
        [Route("api/PontoAcessoRadioApi/CadastrarInterfaceRadio")]
        public async Task<GetApiRetorno<bool>> CadastrarInterfaceRadio(CadastrarInterfaceRadio_Request value)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPontoAcessoRadio = new Conexao<Ponto_Acesso_Radio>(session))
                using (var repoInterface = new Conexao<InterfaceRadio>(session))
                {


                    Ponto_Acesso_Radio pontoAcessoRadio = await repoPontoAcessoRadio.selectFirst(p => p.Id == value.Id_Ponto_Acesso.Value);
                    
                    InterfaceRadio interfaceRadio = new InterfaceRadio
                    {
                        DataHoraCriacao = DateTime.Now,
                        Ponto_Acesso_Radio = pontoAcessoRadio,
                        Descricao = value.Descricao,
                        Codigo_Auxiliar = value.Codigo_Auxiliar

                    };

                    await repoInterface.SaveOrUpdate(interfaceRadio);


                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/PontoAcessoRadioApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_PONTO_ACESSO_RADIO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/PontoAcessoRadioApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PONTO_ACESSO_RADIO A ");

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







    }

}