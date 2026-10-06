using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using NHibernate;
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
    public class ConcentradoraApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ConcentradoraApi/GetAll")]
        public async Task<DataTableResponse<ConcentradoraListModel>> GetAll(ConcentradoraDatatableRequest request)
        {
            List<ConcentradoraListModel> retorno = new List<ConcentradoraListModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID, A.NOME, A.TIPO, A.IP, B.NOME AS POP, C.DESCRICAO AS RADIUS ");
                sql.AppendLine(" FROM TB_CONCENTRADORA A ");

                sql.AppendLine(" INNER JOIN TB_POP B ");
                sql.AppendLine(" ON(B.ID = A.ID_POP) ");

                sql.AppendLine(" INNER JOIN TB_RADIUS C ");
                sql.AppendLine(" ON(C.ID = A.ID_RADIUS) ");
                sql.AppendLine(" ORDER BY A.ID  ");

                #region Filtros

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (sql.ToString().Contains("WHERE"))
                        sql.AppendLine(" AND A.NOME = @NOME ");
                    else
                        sql.AppendLine(" WHERE A.NOME = @NOME ");

                    parametros.Add("@NOME", request.search.value);
                }

                if (request.Tipo.HasValue)
                {
                    if (sql.ToString().Contains("WHERE"))
                        sql.AppendLine(" AND A.TIPO = @TIPO ");
                    else
                        sql.AppendLine(" WHERE A.TIPO = @TIPO ");

                    parametros.Add("@TIPO", request.Tipo.Value);
                }

                if (request.IdPop.HasValue)
                {
                    if (sql.ToString().Contains("WHERE"))
                        sql.AppendLine(" AND A.ID_POP = @ID_POP ");
                    else
                        sql.AppendLine(" WHERE A.ID_POP = @ID_POP ");

                    parametros.Add("@ID_POP", request.IdPop.Value);
                }

                if (request.IdRadius.HasValue)
                {
                    if (sql.ToString().Contains("WHERE"))
                        sql.AppendLine(" AND A.ID_RADIUS = @ID_RADIUS ");
                    else
                        sql.AppendLine(" WHERE A.ID_RADIUS = @ID_RADIUS ");

                    parametros.Add("@ID_RADIUS", request.IdRadius.Value);
                }

                #endregion

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new ConcentradoraListModel
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Nome = Convert.ToString(reader["NOME"]),

                            Tipo = HtmlEnumExtensions.GetEnumDescription((Sinapse.Model.enumerados.Marca_Modelo_Concentradora)Convert.ToInt32(reader["TIPO"])),
                            Ip = Convert.ToString(reader["IP"]),

                            POP = Convert.ToString(reader["POP"]),
                            Radius = Convert.ToString(reader["RADIUS"])
                        });
                    }
                }
            }

            return new DataTableResponse<ConcentradoraListModel>
            {
                data = retorno
            };
        }

        private ConcentradoraCadastroModel GetViewObj(Concentradora objBanco)
        {
            ConcentradoraCadastroModel model = new ConcentradoraCadastroModel();

            model.Id = objBanco.Id;

            model.Nome = objBanco.Nome;
            model.Tipo = objBanco.Tipo;

            model.IdPop = objBanco.POP?.Id ?? 0;
            model.Ip = objBanco.Ip;

            model.Login = objBanco.Login;
            model.Senha = objBanco.Senha;

            model.IdRadius = objBanco.Radius?.Id ?? 0;

            model.PortaTelnet = objBanco.PortaTelnet;
            model.PortaAPI = objBanco.PortaAPI;
            model.PortaHTTP = objBanco.PortaHTTP;
            model.PortaSSH = objBanco.PortaSSH;
            model.PortaIncoming = objBanco.PortaIncoming;

            model.Cod_Escritorio = String.Empty;
            model.Cod_Localidade = String.Empty;
            model.Cod_Area = String.Empty;

            var listaGruposAreaView = new List<Concentradora_Grupo_AreaListModel>();
            if (objBanco.GruposArea != null && objBanco.GruposArea.Count > 0)
            {
                foreach (var grupo in objBanco.GruposArea)
                {
                    if (grupo.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        listaGruposAreaView.Add(new Concentradora_Grupo_AreaListModel
                        {
                            Id = grupo.Id,
                            Cod_Area = grupo.Cod_Area,
                            Cod_Escritorio = grupo.Cod_Escritorio,
                            Cod_Localidade = grupo.Cod_Localidade,
                            Desc_Area = grupo.Desc_Area,
                            Desc_Escritorio = grupo.Desc_Escritorio,
                            Localidade = grupo.Localidade,

                        });
                }
            }

            model.GruposArea = listaGruposAreaView;

            return model;
        }

        private async Task BindViewToObj(ISession session, Concentradora objBanco, ConcentradoraCadastroModel view)
        {
            using (var repoPOP = new Conexao<POP>(session))
            using (var repoRadius = new Conexao<Sinapse.Model.Radius>(session))
            {
                objBanco.Nome = view.Nome;
                objBanco.Tipo = view.Tipo;

                objBanco.POP = await repoPOP.selectFirst(p => p.Id == view.IdPop);
                objBanco.Ip = view.Ip;

                objBanco.Login = view.Login;
                objBanco.Senha = view.Senha;

                objBanco.Radius = await repoRadius.selectFirst(p => p.Id == view.IdRadius);

                objBanco.PortaTelnet = view.PortaTelnet;
                objBanco.PortaAPI = view.PortaAPI;
                objBanco.PortaHTTP = view.PortaHTTP;
                objBanco.PortaSSH = view.PortaSSH;
                objBanco.PortaIncoming = view.PortaIncoming;

            }
        }

        private async Task BindViewToObjGrupoArea(ISession session, Concentradora objBanco, ConcentradoraCadastroModel view, List<Concentradora_Grupo_Area> listagruposArea)
        {
            foreach (var grupoArea in view.GruposArea)
            {
                if (grupoArea.Id == 0)
                {
                    listagruposArea.Add(new Concentradora_Grupo_Area
                    {
                        Cod_Escritorio = grupoArea.Cod_Escritorio,
                        Cod_Area = grupoArea.Cod_Area,
                        Cod_Localidade = grupoArea.Cod_Localidade.HasValue ? grupoArea.Cod_Localidade : (int?)null,
                        Desc_Escritorio = grupoArea.Desc_Escritorio,
                        Desc_Area = grupoArea.Desc_Area,
                        Localidade = grupoArea.Localidade,
                        Concentradora = objBanco,
                        Status = Sinapse.Model.enumerados.Status_Default.Ativo
                    });
                }
            }
        }

        [HttpPost]
        [Route("api/ConcentradoraApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ConcentradoraCadastroModel> param)
        {
            var retorno = new PostApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoConcentradora = new Conexao<Concentradora>(session))
                using (var repoConcentradoraGrupoArea = new Conexao<Concentradora_Grupo_Area>(session))
                {
                    Concentradora model = await repoConcentradora.selectFirst(p => p.Id == param.Objeto.Id);

                    if (model == null)
                    {
                        model = new Concentradora
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = param.Usuario,
                            Secret = ""
                        };
                    }
                    else
                    {
                        model.DataHoraAlteracao = DateTime.Now;
                        model.UsuarioAlteracao = param.Usuario;
                    }

                    var novoRegistro = model.Id == 0;

                    await BindViewToObj(session, model, param.Objeto);
                    await repoConcentradora.SaveOrUpdate(model);

                    List<Concentradora_Grupo_Area> listagruposArea = new List<Concentradora_Grupo_Area>();

                    await BindViewToObjGrupoArea(session, model, param.Objeto, listagruposArea);
                    await repoConcentradoraGrupoArea.SaveOrUpdate(listagruposArea);

                    using (var integracao = new IntegracaoRadiusApiController())
                    {
                        if (novoRegistro)
                            integracao.CadastrarConcentradora(model);
                        else
                            integracao.EditarConcentradora(model);
                    }

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
        [Route("api/ConcentradoraApi/FindById")]
        public async Task<GetApiRetorno<ConcentradoraCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ConcentradoraCadastroModel> retorno = new GetApiRetorno<ConcentradoraCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Concentradora>(session))
                {
                    Concentradora model = await repository.selectFirst(p => p.Id == Id);

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
        [Route("api/ConcentradoraApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CONCENTRADORA A ");

                    List<String> condicoes = new List<String>();

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
                        sqlFrom.AppendLine($" WHERE A.NOME != '' AND ( {String.Join(" AND ", condicoes) } ) ");
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

        public async Task<Int64?> GetConcentradoraByUsuarios(String Cod_Escritorio, String Cod_Area, Int64 Cod_Localidade)
        {
            Int64? retorno = null;

            var lista_concentradoras = new List<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {

                    //PEGAR CONCENTRADORAS POR GRUPO/AREA
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder("SELECT A.ID FROM TB_CONCENTRADORA A ");
                        sql.AppendLine("INNER JOIN TB_CONCENTRADORA_GRUPO_AREA B ON(A.ID = B.ID_CONCENTRADORA) ");
                        sql.AppendLine("INNER JOIN TB_RADIUS C ON(A.ID_RADIUS = C.ID) ");
                        sql.AppendLine("INNER JOIN TB_POOL_IP D ON(C.ID = D.ID_RADIUS) ");
                        sql.AppendLine("WHERE (B.COD_ESCRITORIO = @COD_ESCRITORIO AND B.COD_AREA = @COD_AREA AND B.COD_LOCALIDADE = @COD_LOCALIDADE) ");
                        sql.AppendLine("GROUP BY A.ID ");

                        parametros.Add("@COD_ESCRITORIO", Cod_Escritorio);
                        parametros.Add("@COD_AREA", Cod_Area);
                        parametros.Add("@COD_LOCALIDADE", Cod_Localidade);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                                lista_concentradoras.Add(Convert.ToInt64(reader["ID"]));
                        }

                        if (lista_concentradoras.Count <= 0) //SE NÃO HOUVER CONCENTRADORAS PARA O GRUPO/AREA
                        {
                            sql.Replace("WHERE (B.COD_ESCRITORIO = @COD_ESCRITORIO AND B.COD_AREA = @COD_AREA AND B.COD_LOCALIDADE = @COD_LOCALIDADE)", String.Empty);
                            //parametros.Add("@ID_POOL_IP", Id_Pool_Ip);

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                    lista_concentradoras.Add(Convert.ToInt64(reader["ID"]));
                            }
                        }
                    }


                    //FILTRAR CONCENTRADORA POR QUANTIDADE DE USUÁRIOS VINCULADOS
                    using (var com = session.Connection.CreateCommand())
                    {

                        var SqlWhere = new StringBuilder();
                        StringBuilder sql = new StringBuilder("SELECT FIRST 1 A.ID, COUNT(B.ID) AS USUARIOS FROM TB_CONCENTRADORA A ");
                        sql.AppendLine("INNER JOIN TB_LOGIN_RADIUS B ON (A.ID = B.ID_CONCENTRADORA) ");
                        
                        if (lista_concentradoras.Count > 0)
                        {                             
                            SqlWhere.AppendLine($"WHERE A.ID IN({String.Join(",", lista_concentradoras)})");
                        }

                        sql.AppendLine(SqlWhere.ToString());
                        sql.AppendLine("GROUP BY A.ID ");
                        sql.AppendLine("ORDER BY USUARIOS ASC");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<string, object>(), session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno = Convert.ToInt64(reader["ID"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno = null;
                }
            }

            return retorno;
        }
    }
}