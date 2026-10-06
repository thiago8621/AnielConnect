using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Repository.T5.ConfigPerfilAtivacao;
using Aniel.Connect.Repository.Repository.T5.Material;
using Microsoft.AspNet.Identity;
using NHibernate;
using Org.BouncyCastle.Crypto.Engines;
using Resources;
using Sinapse.Core;
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
    public class PerfilAtivacaoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/PerfilAtivacaoApi/GetAll")]
        public async Task<DataTableResponse<PerfilAtivacaoListaModel>> GetAll(LoginRadiusDatatableRequest request)
        {
            List<PerfilAtivacaoListaModel> retorno = new List<PerfilAtivacaoListaModel>();
            IOrderedEnumerable<PerfilAtivacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_PERFIL_ATIVACAO");

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

                    sql.AppendLine("SELECT A.ID,A.NOME");
                    sql.AppendLine("FROM TB_PERFIL_ATIVACAO A");
                    

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.ID) LIKE UPPER(@SEARCH)) OR (UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new PerfilAtivacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"])


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
                    case 1:     // CODIGO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case 2:     // DESCIRCAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<PerfilAtivacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        public PerfilAtivacaoCadastroModel GetViewObj(PerfilAtivacao model)
        {
            var view = new PerfilAtivacaoCadastroModel();

            var listaGruposAreaView = new List<Perfil_Ativacao_Grupo_AreaListModel>();
            if (model.GruposArea != null && model.GruposArea.Count > 0)
            {
                foreach (var grupo in model.GruposArea)
                {
                    if (grupo.Status == Sinapse.Model.enumerados.Status_Default.Ativo)
                        listaGruposAreaView.Add(new Perfil_Ativacao_Grupo_AreaListModel
                        {
                            Id = grupo.Id,
                            Cod_Area = grupo.Cod_Area,
                            Cod_Escritorio = grupo.Cod_Escritorio,
                            Desc_Area = grupo.Desc_Area,
                            Desc_Escritorio = grupo.Desc_Escritorio,
                        });
                }
            }

            view.GruposArea = listaGruposAreaView;
            view.Id = model.Id;

            view.Nome = model.Nome;
            view.LineProfile = Convert.ToString(model.LineProfile);
            view.ScriptOLTBase64 = model.ScriptOLT != null ? Encoding.UTF8.GetString(model.ScriptOLT) : null;
            view.ScriptOLTVozBase64 = model.ScriptOLTVoz != null ? Encoding.UTF8.GetString(model.ScriptOLTVoz) : null;

            view.IpGerencia = model.IpGerencia;
            view.Login = model.Login;
            view.Senha = model.Senha;
            view.PortaTelnet = Convert.ToString(model.PortaTelnet);
            view.PortaHttp = Convert.ToString(model.PortaHttp);
            
            view.GemPort = Convert.ToString(model.GemPort);
            view.ServicePort = Convert.ToString(model.ServicePort);
            view.SenorId = Convert.ToString(model.SenorId);

            view.ScriptONUBase64 = model.ScriptONU != null ? Encoding.UTF8.GetString(model.ScriptONU) : null;
            view.ObrigaWifi = model.ObrigaWifi;

            return view;
        }

        [HttpGet()]
        [Route("api/PerfilAtivacaoApi/FindById")]
        public async Task<GetApiRetorno<PerfilAtivacaoCadastroModel>> FindById(Int64 id)
        {
            var retorno = new GetApiRetorno<PerfilAtivacaoCadastroModel>();
            
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<PerfilAtivacao>(session))
                {
                    var model = await repository.selectFirst(p => p.Id == id);
                    
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

        [HttpPost()]
        [Route("api/PerfilAtivacaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<PerfilAtivacaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                PerfilAtivacao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<PerfilAtivacao>(session))
                    using (var repoPerfilAtivacaoGrupoArea = new Conexao<Perfil_Ativacao_Grupo_Area>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new PerfilAtivacao
                            {
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        List<Perfil_Ativacao_Grupo_Area> listagruposArea = new List<Perfil_Ativacao_Grupo_Area>();
                        await BindViewToObjGrupoArea(session, modelo, value.Objeto, listagruposArea);

                        await repoPerfilAtivacaoGrupoArea.SaveOrUpdate(listagruposArea);
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

        private void BindViewToObj(PerfilAtivacao objBanco, PerfilAtivacaoCadastroModel view)
        {
            objBanco.Nome = view.Nome;
            objBanco.LineProfile = view.LineProfile;
            objBanco.Login = view.Login;
            objBanco.Senha = view.Senha;
            objBanco.PortaTelnet = !String.IsNullOrEmpty(view.PortaTelnet) ? Convert.ToInt32(view.PortaTelnet) : (Int32?)null;
            objBanco.PortaHttp = !String.IsNullOrEmpty(view.PortaHttp) ? Convert.ToInt32(view.PortaHttp) : (Int32?)null;

            if (!String.IsNullOrEmpty(view.ScriptOLTBase64))
                objBanco.ScriptOLT = Encoding.UTF8.GetBytes(view.ScriptOLTBase64);

            if (!String.IsNullOrEmpty(view.ScriptOLTVozBase64))
                objBanco.ScriptOLTVoz = Encoding.UTF8.GetBytes(view.ScriptOLTVozBase64);

            if (!String.IsNullOrEmpty(view.ScriptONUBase64))
                objBanco.ScriptONU = Encoding.UTF8.GetBytes(view.ScriptONUBase64);

            objBanco.SenorId = !String.IsNullOrEmpty(view.SenorId) ? Convert.ToInt32(view.SenorId) : (Int32?)null;
            objBanco.ServicePort = !String.IsNullOrEmpty(view.ServicePort) ? Convert.ToInt32(view.ServicePort) : (Int32?)null;
            objBanco.GemPort = !String.IsNullOrEmpty(view.GemPort) ? Convert.ToInt32(view.GemPort) : (Int32?)null;
            objBanco.IpGerencia = view.IpGerencia;
            objBanco.ObrigaWifi = view.ObrigaWifi;
        }

        private async Task BindViewToObjGrupoArea(ISession session, PerfilAtivacao objBanco, PerfilAtivacaoCadastroModel view, List<Perfil_Ativacao_Grupo_Area> listagruposArea)
        {
            foreach (var grupoArea in view.GruposArea)
            {
                if (grupoArea.Id == 0)
                {
                    listagruposArea.Add(new Perfil_Ativacao_Grupo_Area
                    {
                        Cod_Escritorio = grupoArea.Cod_Escritorio,
                        Cod_Area = grupoArea.Cod_Area,
                        Desc_Escritorio = grupoArea.Desc_Escritorio,
                        Desc_Area = grupoArea.Desc_Area,
                        Perfil_Ativacao = objBanco,
                        Status = Sinapse.Model.enumerados.Status_Default.Ativo
                    });
                }
            }
        }


        [HttpPost()]
        [Route("api/PerfilAtivacaoApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PERFIL_ATIVACAO A ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME, A.SCRIPT_ONU, A.SCRIPT_OLT ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new PerfilAtivacaoObjComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
                                    ScriptAtivacao = Convert.ToString(reader["SCRIPT_OLT"]),
                                    ScriptONU = Convert.ToString(reader["SCRIPT_ONU"])
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

        public async Task<GetApiRetorno> ImportacaoModelo(Perfil_Ativacao_Vinculo_Importacao item)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {
                using (NHibernate.ISession session = HibernateUtil.GetSession())                
                using (NHibernate.ISession sessionT5 = HibernateUtil.GetSessionT5())                
                using (Conexao<OLT> repoOLT = new Conexao<OLT>(session))                                            
                {                                       

                    OLT olt = null;                                      

                    if (!item.Id_Perfil_Ativacao.HasValue)
                        throw new EAnielException("Id do Perfil de Ativação não informado.");

                 
                    
                    if(item.Usa_Provisionamento_Connect == "S")
                    {   

                        if (item.Id_Olt.HasValue)
                        {
                            olt = await repoOLT.selectFirst(p => p.Id == item.Id_Olt); //Busca pelo Id de OLT do Connect
                        }
                        else
                        {
                            throw new EAnielException("É obrigatório inserir o ID da OLT na planilha.");
                        }                            
                    }
                    else
                    {
                        if (!item.Id_Olt.HasValue && String.IsNullOrEmpty(item.Codigo_Auxiliar_Olt))
                        {
                            throw new EAnielException("Id e Código Auxiliar de OLT não informados. Necessário informar ao menos um dos dados.");
                        }
                            
                        if (!String.IsNullOrEmpty(item.Codigo_Auxiliar_Olt))
                        {
                            olt = await repoOLT.selectFirst(p => p.CodigoAuxiliar == item.Codigo_Auxiliar_Olt); //Busca pelo Código Auxiliar (Id da OLT de integração. Ex: Id no IXC)
                        }
                            
                    }

                    if (olt == null)
                        throw new EAnielException($"OLT não encontrada no sistema. Id OLT: {(item.Id_Olt.HasValue ? Convert.ToString(item.Id_Olt) : "Não informado")} / Código Auxiliar: {(item.Codigo_Auxiliar_Olt ?? "Não informado")}");

                    var materialAnielRepository = new MaterialAnielRepository(sessionT5);
                    var materialAnielBusiness = new MaterialAnielBusiness(materialAnielRepository);
                    var material = await materialAnielBusiness.GetById(item.CodMat);
                    
                    if(material == null)
                        throw new EAnielException("Material não encontrado no sistema.");

                    if (material.AtivacaoSinal != "S")
                        throw new EAnielException("Material não está configurado para ativação de sinal.");

                    var configPerfilAtivacaoRepository = new ConfigPerfilAtivacaoRepository(sessionT5);
                    var configPerfilAtivacaoBusiness = new ConfigPerfilAtivacaoBusiness(configPerfilAtivacaoRepository);
                    var configPerfilAtivacao = await configPerfilAtivacaoBusiness.GetByVinculo(item.CodMat, Convert.ToString(item.Id_Olt));

                    if(configPerfilAtivacao == null)
                    {
                        var idOlt = "";

                        if(item.Usa_Provisionamento_Connect == "S")
                        {  
                            if(item.Id_Olt == null)
                            {
                                throw new EAnielException("Provisionamento via Connect, é obrigatório inserir Id da OLT na planilha.");
                            }

                            idOlt = Convert.ToString(item.Id_Olt);
                        }
                        else
                        {
                            idOlt = olt.CodigoAuxiliar;
                        }

                        configPerfilAtivacao = new Aniel.T5.Model.nhibernate.ConfigPerfilAtivacaoAniel()
                        {
                            Codmat = item.CodMat,
                            Id_OLT = idOlt,
                            Id_Perfil_Ativacao = Convert.ToString(item.Id_Perfil_Ativacao),
                            Id_Perfil_Ativacao_Onu = Convert.ToString(item.Id_Perfil_Ativacao_Onu),
                            Usuario_Importacao = User.Identity.GetUserId(),
                            DataHora_Importacao = DateTime.Now
                        };
                    }
                    else
                    {
                        configPerfilAtivacao.Id_Perfil_Ativacao = Convert.ToString(item.Id_Perfil_Ativacao);
                        configPerfilAtivacao.Id_Perfil_Ativacao_Onu = Convert.ToString(item.Id_Perfil_Ativacao_Onu);
                        configPerfilAtivacao.Usuario_Importacao = User.Identity.GetUserId();
                        configPerfilAtivacao.DataHora_Importacao = DateTime.Now;
                    }

                    await configPerfilAtivacaoBusiness.InsertOrUpdate(configPerfilAtivacao);

                    retorno.Ok = true;
                }
            }
            catch (Exception e)
            {
                var logImportacaoOnu = new LogImportacaoVinculoPerfilAtivacao
                {
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Codmat = item.CodMat,
                    Id_OLT = item.Id_Olt ?? (!String.IsNullOrEmpty(item.Codigo_Auxiliar_Olt) ? Convert.ToInt64(item.Codigo_Auxiliar_Olt) : (long?) null),
                    Erro = e.InnerException?.Message != null ? (e.Message + e.InnerException?.Message) : e.Message,
                    Usuario = User.Identity.GetUserId()
                };

                await GravaLogImportacao(logImportacaoOnu);

                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;
            }

            return retorno;
        }

        public async Task GravaLogImportacao(LogImportacaoVinculoPerfilAtivacao log)
        {
            using (var session = HibernateUtil.GetSessionLog())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_LOG_IMPORT_VINC_PERF_ATIV (ID,DATA,HORA,ERRO,CODMAT,ID_OLT,USUARIO)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_LOG_IMPORT_VINC_PERF_ATIV, 1) FROM RDB$DATABASE),@DATA,@HORA,@ERRO,@CODMAT,@ID_OLT,@USUARIO)");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@DATA", log.Data.Replace('/', '-'));
                parametros.Add("@HORA", log.Hora);
                parametros.Add("@ERRO", StringService.Truncate(log.Erro, 255));
                parametros.Add("@CODMAT", log.Codmat);
                parametros.Add("@ID_OLT", log.Id_OLT);
                parametros.Add("@USUARIO", log.Usuario);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        [HttpPost]
        [Route("api/PerfilAtivacaoApi/LogImportacaoVinculoPerfilAtivacao")]
        public async Task<DataTableResponse<LogImportacaoVinculoPerfilAtivacao>> LogImportacaoVinculoPerfilAtivacao(LogImportacaoVinculoPerfilAtivacaoRequest request)
        {
            List<LogImportacaoVinculoPerfilAtivacao> retorno = new List<LogImportacaoVinculoPerfilAtivacao>();
            var recordsTotal = 0;

            using (var session = HibernateUtil.GetSessionLog())
            {
                StringBuilder sqlWhere = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("DATA", Convert.ToDateTime(request.Data).ToString("dd-MM-yyyy"));

                    sqlWhere.AppendLine(" WHERE DATA = @DATA ");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) AS CO FROM TB_LOG_IMPORT_VINC_PERF_ATIV ");
                    sqlCount.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT * FROM TB_LOG_IMPORT_VINC_PERF_ATIV ");
                    sql.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(
                                new LogImportacaoVinculoPerfilAtivacao 
                                { 
                                    Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy"), 
                                    Hora = Convert.ToDateTime(reader["HORA"].ToString()).ToString("HH:mm"), 
                                    Erro = reader["ERRO"].ToString(), 
                                    Codmat = reader["CODMAT"].ToString(), 
                                    Id_OLT = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : (long?)null, 
                                    Usuario = reader["USUARIO"].ToString() 
                                }
                            );
                        }
                    }
                }
            }

            return new DataTableResponse<LogImportacaoVinculoPerfilAtivacao>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        public async Task<PerfilAtivacao> FindByIdSQL(long idPerfil)
        {
            PerfilAtivacao retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 ID, NOME, IP_GERENCIA, LOGIN, SENHA, PORTA_TELNET, GEM_PORT, SERVICE_PORT, ");
                sql.AppendLine(" SENOR_ID, PORTA_HTTP, LINE_PROFILE, SCRIPT_ONU, SCRIPT_OLT, SCRIPT_OLT_VOZ");
                sql.AppendLine(" FROM TB_PERFIL_ATIVACAO ");
                sql.AppendLine(" WHERE ID = @ID ");

                parametros.Add("@ID", idPerfil);               

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new PerfilAtivacao();
                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Nome = Convert.ToString(reader["NOME"]);
                        retorno.IpGerencia = Convert.ToString(reader["IP_GERENCIA"]);
                        retorno.Login = Convert.ToString(reader["LOGIN"]);
                        retorno.Senha = Convert.ToString(reader["SENHA"]);
                        retorno.PortaTelnet = reader["PORTA_TELNET"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_TELNET"]) : (int?)null;
                        retorno.GemPort = reader["GEM_PORT"] != DBNull.Value ? Convert.ToInt32(reader["GEM_PORT"]) : (int?)null;
                        retorno.ServicePort = reader["SERVICE_PORT"] != DBNull.Value ? Convert.ToInt32(reader["SERVICE_PORT"]) : (int?)null;
                        retorno.SenorId = reader["SENOR_ID"] != DBNull.Value ? Convert.ToInt32(reader["SENOR_ID"]) : (int?)null;
                        retorno.PortaHttp = reader["PORTA_HTTP"] != DBNull.Value ? Convert.ToInt32(reader["PORTA_HTTP"]) : (int?)null;
                        retorno.LineProfile = Convert.ToString(reader["LINE_PROFILE"]);                                                
                    }
                }
            }

            return retorno;
        }
    }
}