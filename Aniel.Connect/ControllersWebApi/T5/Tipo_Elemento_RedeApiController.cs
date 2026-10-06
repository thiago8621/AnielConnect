using Aniel.Connect.Core;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Core.Helpers;
using Sinapse.Model.enumerados;
using Aniel.T5.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Sinapse.Model;
using Tipo_Cabo = Sinapse.Model.Tipo_Cabo;
using Aniel.Connect.Repository.Model;

namespace Aniel.Connect.ControllersWebApi.T5
{

    [Authorize]
    public class Tipo_Elemento_RedeApiController : BaseApiController, ITipo_Elemento_RedeApi
    {

      
        [HttpPost()]
        [Route("api/Tipo_Elemento_RedeApi/GetAll")]

        #region GetAll

        public async Task<DataTableResponse<Tipo_Elemento_RedeListaModel>> GetAll(Tipo_Elemento_RedeRequest request)
        {
            List<Tipo_Elemento_RedeListaModel> retorno = new List<Tipo_Elemento_RedeListaModel>();
            int RecordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlWhere = new StringBuilder();

                    #region requests
                    if (request.E_Cabo)
                    {
                        parametros.Add("@CABO", request.E_Cabo);
                    }

                    if (request.Id_Espessura_linha.HasValue)
                    {
                        parametros.Add("@ESPESSURA_LINHA", request.Id_Espessura_linha);
                    }

                    if (request.Id_Modelo.HasValue)
                    {
                        parametros.Add("@MODELO", request.Id_Modelo);
                    }

                    if (request.Id_Tipo_Cabo.HasValue)
                    {
                        parametros.Add("@TIPO_CABO", request.Id_Tipo_Cabo);
                    }

                    if (request.Icone.HasValue)
                    {
                        parametros.Add("@ICONE", request.Icone);
                    }

                    if (request.Situacao.HasValue)
                    {
                        parametros.Add("@STATUS", request.Situacao);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    #endregion

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        using (var reader = await this.sqlGetAll(session, com, parametros, true))
                        {
                            while (await reader.ReadAsync())
                            {
                                RecordsTotal = Convert.ToInt32(reader["CO"]);
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        using (var reader = await this.sqlGetAll(session, com, parametros, false))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Tipo_Elemento_RedeListaModel
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : 0,
                                    Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    E_Cabo = Convert.ToString(reader["E_CABO"]) != Convert.ToString(Status_Default.Ativo) ? "SIM" : "NAO",

                                    Fabricante = reader["FABRICANTE"] != DBNull.Value ? Convert.ToString(reader["FABRICANTE"]) : "-",
                                    Situacao = reader["SITUACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["SITUACAO"])) : "-",
                                    Cor = reader["COR"] != DBNull.Value ? Convert.ToString(reader["COR"]) : "-",

                                    Icone = reader["CAMINHO"] != DBNull.Value ? Convert.ToString(reader["CAMINHO"]) : "-",

                                };
                                retorno.Add(obj);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return new DataTableResponse<Tipo_Elemento_RedeListaModel>
            {
                data = retorno,
                draw = request.draw,
                error = "",
                recordsFiltered = RecordsTotal
            };

        }
        #endregion
        private async Task<System.Data.Common.DbDataReader> sqlGetAll(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, Object> Filters, bool Count)
        {
            #region sql_GetAll 
            System.Data.Common.DbDataReader reader = null;

            try
            {
                #region requests
                List<String> condicoes = new List<String>();
                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@CABO":
                            condicoes.Add($"A.E_CABO={request.Key}");
                            break;

                        case "@ESPESSURA_LINHA":
                            condicoes.Add($"A.ID_ESPESSURA_LINHA={request.Key}");
                            break;

                        case "@MODELO":
                            condicoes.Add($"A.MODELO={request.Key}");
                            break;

                        case "@TIPO_CABO":
                            condicoes.Add($"A.ID_TIPO_CABO={request.Key}");
                            break;

                        case "@ICONE":
                            condicoes.Add($"A.ID_ICONE={request.Key}");
                            break;

                        case "@STATUS":
                            condicoes.Add($"A.SITUACAO={request.Key}");
                            break;

                        case "@SEARCH":
                            condicoes.Add($"UPPER(A.DESCRICAO) LIKE {request.Key}");
                            break;

                    }
                });
                #endregion

                StringBuilder sqlOrder = new StringBuilder(),
                              sqlWhere = new StringBuilder(),
                              sqlFrom = new StringBuilder(),
                              sql = new StringBuilder();

                sqlOrder.AppendLine("ORDER BY A.ID");

                if (condicoes.Count > 0)
                {
                    sqlWhere.AppendLine($"WHERE {String.Join("AND", condicoes)}");
                }
                sqlFrom.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A ")
                       .AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_ICONE B ")
                       .AppendLine(" ON A.ID_ICONE= B.ID");

                if (Count)
                {
                    sql.AppendFormat("SELECT COUNT (A.ID) CO \r\n {0} \r\n {1}", sqlFrom, sqlWhere);
                }
                else
                {
                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.SITUACAO,B.DESCRICAO AS ICONE_DESC,B.CAMINHO,")
                       .AppendFormat("A.FABRICANTE, A.E_CABO, A.COR \r\n {0} \r\n {1} \r\n {2}", sqlFrom, sqlWhere, sqlOrder);
                }

                reader = await CommandHelper.ExecuteReader(comm, sql, Filters, session.Connection, null);
            }
            catch (Exception)
            {
                //                                      *******      Pro Tip     *******
                //Use o Quick Watch com o comando (shift + F9) quando debugar a variável $exception para mais detalhes do erro.
            }

            return reader;
            #endregion
        }



        [HttpPost]
        [Route("api/Tipo_Elemento_RedeApi/GetCombo")]

        #region GetCombo
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionNH = HibernateUtil.GetSessionT5())
            using (var repositoryColuna = new Conexao<ColunaElementoRede>(sessionNH))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_ELEMENTO_REDE A ")
                        .AppendLine("LEFT JOIN TB_MODELO_ELEMENTO_REDE B ON A.ID_MODELO = B.ID");


                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.ID_MODELO, A.CAPACIDADE, B.ID, B.DESCRICAO AS DESCRICAO_MODELO ");
                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {


                            while (await reader.ReadAsync())
                            {
                                var idModelo = reader["ID_MODELO"] != DBNull.Value ? Convert.ToInt64(reader["ID_MODELO"]) : (Int64?)null;
                                var capacidade = reader["CAPACIDADE"] != DBNull.Value ? Convert.ToString(reader["CAPACIDADE"]) : "0";

                                var obj = new TipoElementoRedeComboResult
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = capacidade == "0" ? Convert.ToString(reader["DESCRICAO"]) : Convert.ToString(reader["DESCRICAO"]) + " - " + $"{capacidade} Portas",
                                    capacidade = capacidade,
                                    Modelo_Vinculo = Convert.ToString(reader["DESCRICAO_MODELO"]),
                                    Colunas = idModelo.HasValue ? repositoryColuna.select(p => p.Modelo_Elemento_Rede.Id == idModelo).Result.Select(p => new ColunaModeloRedeListaModel
                                    {
                                        Coluna = p.Coluna,
                                        Obrigatorio = p.Obrigatorio,
                                        Id_Dinamico = p.CampoDinamco?.Id

                                    }).ToList() : new List<ColunaModeloRedeListaModel>()
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                    _ = ex.InnerException.Message;
                    throw ex;
                }
            }

            return retorno;
        }
        #endregion


        [HttpPost]
        [Route("api/Tipo_Elemento_RedeApi/GetComboOptGroup")]

        #region GetComboOptGroup
        public async Task<ObjComboOptGroup<String, String>> GetComboOptGroup(RequestObjCombo<String> request)
        {
            ObjComboOptGroup<String, String> retorno = new ObjComboOptGroup<String, String>
            {
                results = new List<IObjComboOptGroupResult<string, string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession sessionNH = HibernateUtil.GetSessionT5())
            using (var repositoryColuna = new Conexao<ColunaElementoRede>(sessionNH))
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();                  

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                        parametros.Add("@SO_ATIVOS", request.soativos);
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        //condicoes.Add($"A.ID = @ID");
                        parametros.Add("@ID", request.id);                        
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        parametros.Add("@SEARCH", StringService.Truncate(request.q.ToUpperInvariant(), 100));                        
                    }

                    parametros.Add("@PAGE", request.page);
                    parametros.Add("@COUNT", false);

                    using (var com = session.Connection.CreateCommand())
                    {                        
                        parametros["@COUNT"] = true;

                        using (var reader = await sqlGetComboOpGroup(session, com, parametros))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }
                    //Generate_Group_Elements
                    using (var com = session.Connection.CreateCommand())
                    {
                        parametros["@COUNT"] = false;

                        using (var reader = await sqlGetComboOpGroup(session, com, parametros))
                        {
                            while (await reader.ReadAsync())
                            {
                                var id = Convert.ToString(reader["ID_MODELO"]);
                                retorno.results.Add(
                                    new ObjComboOptGroupResult<String, String>
                                    {
                                        Id_Grupo = Convert.ToString(reader["ID_MODELO"]),
                                        Text = Convert.ToString(reader["DESCRICAO_MODELO"]),
                                        Children = await sqlGetOptionsFromGroup(Convert.ToInt64(reader["ID_MODELO"]), request.page, sessionNH)
                                    });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = String.IsNullOrEmpty(ex.InnerException.Message) ? ex.Message : $"{ex.Message} - {ex.InnerException.Message}";

                    throw ex;
                }
            }

            return retorno;
        }
        #endregion
        private async Task<System.Data.Common.DbDataReader> sqlGetComboOpGroup(ISession session, System.Data.Common.DbCommand comm, Dictionary<String, object> Filters)
        {
            #region sqlGetComboOpGroup

            System.Data.Common.DbDataReader retorno = null;
            bool count = Convert.ToBoolean(Filters["@COUNT"]);
            int page = Convert.ToInt32(Filters["@PAGE"]);

            try
            {
                List<String> condicoes = new List<String>();

                StringBuilder sqlOrder = new StringBuilder(),
                              sqlWhere = new StringBuilder(),
                              sqlFrom = new StringBuilder(),
                              sql = new StringBuilder();

                sqlFrom.AppendLine(" FROM TB_TIPO_ELEMENTO_REDE A ")
                       .AppendLine(" LEFT JOIN TB_MODELO_ELEMENTO_REDE B ON A.ID_MODELO = B.ID ")
                       .AppendLine(" LEFT JOIN TB_TIPO_ELEMENTO_ICONE C ON A.ID_ICONE = C.ID ");

                //Filtros
                Filters.ForEach(request =>
                {
                    switch (request.Key)
                    {
                        case "@SO_ATIVOS":
                            condicoes.Add($"A.SITUACAO = {request.Key}");
                            break;
                        case "@ID":
                            condicoes.Add($"A.ID = {request.Key}");
                            break;
                        case "@SEARCH":
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{request.Key}%')");
                            break;
                    }
                });


                if (condicoes.Count() > 0)
                {
                    sqlWhere.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                }

                if (count)
                {
                    sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                }

                else
                {
                    sqlOrder.AppendLine("ORDER BY ID_MODELO");
                    sql.AppendLine(" SELECT DISTINCT A.ID_MODELO, B.DESCRICAO AS DESCRICAO_MODELO ");
                }

                sql.AppendFormat("{0} \r\n {1} \r\n {2}", sqlFrom, sqlWhere, sqlOrder);

                retorno = await CommandHelper.ExecuteReaderPage(comm, sql, Filters, session.Connection, null, page);

            }
            catch (Exception ex)
            {
                _ = String.IsNullOrEmpty(ex.InnerException.Message) ? ex.Message : $"{ex.Message} - {ex.InnerException.Message}";

                throw ex;
            }
            return retorno;
            #endregion
        }

        private async Task<List<IObjComboResult<String>>> sqlGetOptionsFromGroup(long Id, int page, ISession session)
        #region OptionsFromGroup
        {
            List<String> condicoes = new List<String>();
            var groupCombo = new List<IObjComboResult<String>>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {
                condicoes.Add($"A.Id_Modelo = '{Id}'");

                StringBuilder sqlWhere = new StringBuilder($"WHERE ( {String.Join(" AND ", condicoes)} ) "),
                              sqlFrom = new StringBuilder(@" FROM TB_TIPO_ELEMENTO_REDE A 
                                                                 LEFT JOIN TB_MODELO_ELEMENTO_REDE B ON A.ID_MODELO = B.ID 
                                                                 LEFT JOIN TB_TIPO_ELEMENTO_ICONE C ON A.ID_ICONE = C.ID "),

                               sql = new StringBuilder()
                                         .AppendFormat(@"SELECT DISTINCT A.ID, A.DESCRICAO, A.ID_MODELO, A.CAPACIDADE, B.ID MODELO_ID,                                                                               
                                                         B.DESCRICAO AS DESCRICAO_MODELO, A.COR, C.ARQUIVO, A.E_CABO,                                                                               
                                                         A.ESTILO_LINHA_ATIVA,A.ESTILO_LINHA_INATIVA,A.ESPESSURA                                                                                
                                                         {0}
                                                         {1}
                                                         ORDER BY DESCRICAO", sqlFrom, sqlWhere);

                Func<string, string> local_GetPaths = (arquivo) =>
                {

                    string icone_path = String.Empty;
                    var xmlDoc = new System.Xml.XmlDocument();

                    xmlDoc.LoadXml(arquivo);

                    foreach (System.Xml.XmlNode path in xmlDoc.GetElementsByTagName("path"))
                    {
                        if (!String.IsNullOrEmpty(path.Attributes.GetNamedItem("d").Value))
                            icone_path += path.Attributes.GetNamedItem("d").Value;
                    }
                    return icone_path;
                };

                using (var reader = CommandHelper.ExecuteReaderPage(com, sql, new Dictionary<String, Object>(), session.Connection, null, page).Result)
                {
                    while (await reader.ReadAsync())
                    {
                        String Arquivo = Convert.ToString(reader["ARQUIVO"]);

                        var option = new ObjComboResultTipoElemento<String>
                        {
                            Id = Convert.ToString(reader["ID"]),
                            IdModelo = Convert.ToString(reader["ID_MODELO"]),
                            Text = Convert.ToString(reader["DESCRICAO"]),

                            IconeCor = Convert.ToString(reader["COR"]),
                            E_Cabo = Convert.ToBoolean(reader["E_CABO"]),
                            IconePath = String.IsNullOrEmpty(Arquivo) ? String.Empty : local_GetPaths(Arquivo),

                            EstiloLinhaAtiva = Convert.ToString(reader["ESTILO_LINHA_ATIVA"]),
                            EstiloLinhaInativa = Convert.ToString(reader["ESTILO_LINHA_INATIVA"]),
                            Espessura = reader["ESPESSURA"] != DBNull.Value ? Convert.ToInt32(reader["ESPESSURA"]) : 1
                        };

                        groupCombo.Add(option);
                    }
                }
            }

            return groupCombo;
        }
        #endregion


        [HttpPost]
        [Route("api/Tipo_Elemento_RedeApi/Gravar")]

        #region Gravar

        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Tipo_Elemento_RedeCadastroModel> request)
        {
            PostApiRetorno<long> response = new PostApiRetorno<Int64>();
            String certo = "Salvo com sucesso";

            Tipo_Elemento_Rede tipo = null;

            try
            {
                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var repo = new Conexao<Tipo_Elemento_Rede>(session))
                    using (var repoEspessura = new Conexao<Espessura_Linha>(session))
                    using (var repocabo = new Conexao<Tipo_Cabo>(session))

                    using (var repoTipoElementoMaterial = new Conexao<Tipo_ElementoMaterial>(session))
                    using (var repoTipo_ElementoCaderno = new Conexao<Tipo_ElementoCaderno>(session))
                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(session))

                    using (var repoIcone = new Conexao<Tipo_ElementoIcone>(session))
                    using (var repoPorta = new Conexao<Elemento_Rede_Porta>(session))
                    using (var repoElemento = new Conexao<Elemento_Rede>(session))
                    {
                        bool possuiPortasVinculadas = false;
                        List<Elemento_Rede_Porta> portas = new List<Elemento_Rede_Porta>();
                        if (request.Objeto.Id != 0) { tipo = await repo.selectFirst(p => p.Id == request.Objeto.Id); }

                        //cabo = await repocabo.selectFirst(x => x.Id == request.Objeto.Id);   //trocar para o id do objeto certo 
                        //linha = await repoEspessura.selectFirst(x => x.Id == request.Objeto.Id);  //trocar para o id do objeto certo         
                        //icone = await repoicone.selectFirst(x => x.Id == request.Objeto.Id);  //trocar para o id do objeto certo 

                        if (tipo == null)
                        {

                            tipo = new Tipo_Elemento_Rede
                            {
                                Situacao = Status_Default.Ativo,
                                UsuarioCriacao = request.Usuario,
                                DataHoraCriacao = DateTime.Now,

                            };

                        }
                        else
                        {
                            tipo.UsuarioAlteracao = request.Usuario;
                            tipo.DataHoraAlteracao = DateTime.Now;
                            tipo.Situacao = request.Objeto.Situacao ? Status_Default.Ativo : Status_Default.Inativo;

                            var elementos = await repoElemento.select(p => p.Tipo == tipo);
                            foreach (var item in elementos)
                            {
                                portas.AddRange((await repoPorta.select(p => p.Caixa == item)).ToList());
                            }

                            possuiPortasVinculadas = portas.Where(p => p.Cliente_Plano_Prod != null).Count() > 0 ? true : false;

                        }

                        BindTipo(session, request.Objeto, tipo, possuiPortasVinculadas);
                        tipo.Modelo = await repoModelo.selectFirst(p => p.Id == request.Objeto.Id_Modelo);
                        //tipo.Espessura_linha = await repoEspessura.selectFirst(p => p == linha);
                        tipo.Icone = await repoIcone.selectFirst(p => p.Id == request.Objeto.Id_Icone);
                        //tipo.Tipo_Cabo = await repocabo.selectFirst(p => p == cabo);

                        var materiais = request.Objeto.Materiais.Select(p =>
                        {
                            return new Tipo_ElementoMaterial
                            {
                                CodMat = p.CodMat,
                                Tipo = tipo,
                                Id = p.Id
                            };
                        }).ToList();

                        var servicos = request.Objeto.Servicos.Select(p =>
                        {
                            return new Tipo_ElementoCaderno
                            {
                                Unidade_Custo = p.UnidadeCusto,
                                Codigo = p.Codigo,
                                Tipo = tipo,
                                Id = p.Id
                            };
                        }).ToList();


                        await repo.SaveOrUpdate(tipo);
                        await repoTipoElementoMaterial.SaveOrUpdate(materiais);
                        await repoTipo_ElementoCaderno.SaveOrUpdate(servicos);

                        response.IdObjeto = tipo.Id;
                        response.Mensagem = certo;

                    }

                    response.Ok = true;
                    return response;
                }
            }
            catch (Exception e)
            {
                response.Ok = false;
                response.Mensagem = e.Message;
                response.InnerException = e.InnerException?.Message;
                return response;
            }
        }
        #endregion

        #region BindTipo
        private void BindTipo(ISession session, Tipo_Elemento_RedeCadastroModel tipoModelView, Tipo_Elemento_Rede tipo, bool possuiPortaVinculada = false)
        {

            if (possuiPortaVinculada)
            {

                if (tipo.E_Cabo != tipoModelView.E_Cabo)
                {
                    throw new Exception("Elemento possui portas vinculadas a clientes");
                }

                if (tipo.E_Caixa != tipoModelView.E_Caixa)
                {
                    throw new Exception("Elemento possui portas vinculadas a clientes");
                }
            }

            Conexao<Tipo_ElementoIcone> repoIcones = new Conexao<Tipo_ElementoIcone>(session);
            tipo.Cor = tipoModelView.Cor;
            tipo.Capacidade = tipoModelView.E_Caixa == true ? Convert.ToInt64(tipoModelView.Capacidade) : 0;
            tipo.Num_Pares_Fibra = tipoModelView.Num_Pares_Fibra;
            tipo.Descricao = tipoModelView.Descricao;
            tipo.Fabricante = tipoModelView.Fabricante;
            //tipo.Icone = repoicone.selectFirst(p => p.Id == tipoModelView.Id_Icone);
            tipo.Cor_Linha_Ativa = tipoModelView.E_Cabo == true ? tipoModelView.Cor_Linha_Ativa : null;
            tipo.Cor_Linha_Inativa = tipoModelView.E_Cabo == true ? tipoModelView.Cor_Linha_Inativa : null;
            tipo.Espessura = tipoModelView.E_Cabo == true ? tipoModelView.Espessura : null;
            tipo.Espessura_Linha = tipoModelView.E_Cabo == true ? tipoModelView.Espessura_linha : null;
            tipo.Estilo_Linha_Ativa = tipoModelView.E_Cabo == true ? tipoModelView.Estilo_Linha_Ativa : null;
            tipo.Estilo_Linha_Inativa = tipoModelView.E_Cabo == true ? tipoModelView.Estilo_Linha_Inativa : null;
            //banco vai salvar o total de tubos loose considerando que o número de pares vai influenciar
            tipo.Num_Tubos_Loose = tipoModelView.E_Cabo == true ? tipoModelView.Num_Tubos_Loose : null;
            tipo.Total_Tubos_loose = tipoModelView.E_Cabo == true ? tipoModelView.Num_Pares_Fibra * tipoModelView.Num_Tubos_Loose : null;
            tipo.E_Cabo = tipoModelView.E_Cabo;
            tipo.E_Caixa = tipoModelView.E_Caixa;
            //tipo.Tipo_Cabo = tipoModelView.Tipo_Cabo;
            tipo.Tipo_de_Cabo = tipoModelView.E_Cabo == true ? tipoModelView.TipoCabo : null;
            tipo.Padrao = tipoModelView.Padrao_Cor;
            tipo.Icone = tipoModelView.Id_Icone != null ? repoIcones.selectFirst(p => p.Id == tipoModelView.Id_Icone.Value).Result : null;
            tipo.Codigo_Auxiliar = tipoModelView.Codigo_Auxiliar;
        }
        #endregion


        [HttpGet]
        [Route("api/Tipo_Elemento_RedeApi/MaterialVinculado")]

        #region MaterialJaVinculado
        public async Task<GetApiRetorno<TipoElementoMaterial>> MaterialVinculado(String CodMat)
        {
            GetApiRetorno<TipoElementoMaterial> retorno = new GetApiRetorno<TipoElementoMaterial>();

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
               
                    using (var repoTipoElementoMaterial = new Conexao<Tipo_ElementoMaterial>(sessionT5))
                    using (var repoMaterial = new Conexao<MaterialAniel>(sessionT5))
                    {

                        var MaterialVinculado = await repoTipoElementoMaterial.selectFirst(p => p.CodMat == CodMat);
                        var Descricao = await repoMaterial.selectFirst(p => p.CodMat == CodMat);

                        if (MaterialVinculado != null)
                        {
                            retorno.Objeto = new TipoElementoMaterial
                            {
                                CodMat = MaterialVinculado.CodMat,
                                Descricao = Descricao?.Descricao
                            };
                        }

                        retorno.Ok = true;

                    }
                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }
        #endregion
        
        [HttpGet]
        [Route("api/Tipo_Elemento_RedeApi/FindById")]

        #region FindById
        public async Task<GetApiRetorno<Tipo_Elemento_RedeCadastroModel>> FindById(Int64? Id)
        {
            GetApiRetorno<Tipo_Elemento_RedeCadastroModel> retorno = new GetApiRetorno<Tipo_Elemento_RedeCadastroModel>();

            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {

                    using (var RepoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                    using (var repoTipoElementoMaterial = new Conexao<Tipo_ElementoMaterial>(sessionT5))
                    using (var repoMaterial = new Conexao<MaterialAniel>(sessionT5))
                    using (var repoCaderno = new Conexao<Caderno_EncargosAniel>(sessionT5))
                    using (var repoModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                    using (var repoTipoElementoServico = new Conexao<Tipo_ElementoCaderno>(sessionT5))
                    {
                        retorno.Objeto = GetViewObj(await RepoTipo.selectFirst(p => p.Id == Id));


                        retorno.Objeto.Materiais = repoTipoElementoMaterial.select(p => p.Tipo.Id == retorno.Objeto.Id).Result.Select(p => new TipoElementoMaterial
                        {
                            Id = p.Id,
                            CodMat = p.CodMat,
                            Descricao = repoMaterial.selectFirst(y => y.CodMat == p.CodMat).Result?.Descricao
                        }).ToList();

                        retorno.Objeto.Servicos = repoTipoElementoServico.select(p => p.Tipo.Id == retorno.Objeto.Id).Result.Select(p => new TipoElementoServico
                        {
                            Id = p.Id,
                            Codigo = p.Codigo,
                            UnidadeCusto = p.Unidade_Custo,
                            Descricao = repoCaderno.selectFirst(y => y.Unidade_Custo == p.Unidade_Custo && y.Codigo == p.Codigo).Result?.Descricao
                        }).ToList();
                    }
                    retorno.Ok = true;

                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;

            }
            return retorno;
        }
        #endregion

        #region GetViewObj
        private Tipo_Elemento_RedeCadastroModel GetViewObj(Tipo_Elemento_Rede banco)
        {
            using (var sessionT5 = HibernateUtil.GetSession())
            {

                using (var RepoTipo = new Conexao<Tipo_Elemento_RedeCadastroModel>(sessionT5))
                {

                    return new Tipo_Elemento_RedeCadastroModel
                    {
                        Codigo_Auxiliar = banco.Codigo_Auxiliar,
                        Id = banco.Id,
                        Descricao = banco.Descricao,
                        Espessura = banco.Espessura,
                        Espessura_linha = banco.Espessura,
                        //Espessura_linha = banco.Espessura_linha,
                        E_Cabo = banco.E_Cabo,
                        Fabricante = banco.Fabricante,
                        Num_Pares_Fibra = banco.Num_Pares_Fibra,
                        //View vai mostrar os numeros de  tubos loose considerando que o número de pares vai influenciar
                        Num_Tubos_Loose = banco.Num_Tubos_Loose,
                        Total_Tubos_Loose = banco.Total_Tubos_loose,
                        Cor = banco.E_Cabo == true ? banco.Cor_Linha_Ativa : banco.Cor,
                        Cor_Linha_Ativa = banco.Cor_Linha_Ativa,
                        Cor_Linha_Inativa = banco.Cor_Linha_Inativa,
                        Estilo_Linha_Ativa = banco.Estilo_Linha_Ativa,
                        Estilo_Linha_Inativa = banco.Estilo_Linha_Inativa,
                        //Estilo_Linha_Ativa_Demonstracao = banco.Estilo_Linha_Ativa_Demonstracao,
                        //Estilo_Linha_Inativa_Demonstracao = banco.Estilo_Linha_Inativa_Demonstracao,
                        Id_Icone = banco.Icone?.Id,
                        Id_Modelo = banco.Modelo?.Id,
                        TipoCabo = banco.Tipo_de_Cabo,
                        //Materiais =banco.Material,
                        //Servicos = banco.Servico,
                        Situacao = banco.Situacao == Status_Default.Ativo,
                        Capacidade = banco.Capacidade.ToString(),
                        Padrao_Cor = banco.Padrao,
                        E_Caixa = banco.E_Caixa
                        //Tipo_Cabo = banco.Tipo_Cabo,

                    };
                }
            }
        }
        #endregion

     
        [HttpPost()]
        [Route("api/Tipo_Elemento_RedeApi/Excluir")]
        #region Excluir
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                Tipo_Elemento_Rede Tipo = null;
                List<Tipo_ElementoCaderno> Encargos = null;
                List<Tipo_ElementoMaterial> Materiais = null;

                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repo_Caderno = new Conexao<Tipo_ElementoCaderno>(sessionT5))
                    using (var repo_Materiais = new Conexao<Tipo_ElementoMaterial>(sessionT5))
                    using (var repositorio = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                    {
                        Tipo = await repositorio.selectFirst(p => p.Id == value.Objeto);
                        Encargos = (await repo_Caderno.select(prop => prop.Tipo == Tipo)).ToList();
                        Materiais = (await repo_Materiais.select(prop => prop.Tipo == Tipo)).ToList();

                        await repo_Caderno.Remove(Encargos);
                        await repo_Materiais.Remove(Materiais);
                        await repositorio.Remove(Tipo);

                        retorno.IdObjeto = Tipo.Id;
                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception erro)
            {
                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro?.InnerException?.Message;
            }
            return retorno;

        }
        #endregion


        [HttpPost()]
        [Route("api/Tipo_Elemento_RedeApi/AlterarStatus")]

        #region AlterarStatus
        public async Task<PostApiRetorno<Tipo_Elemento_RedeListaModel>> AlterarStatus(AlterarStatusViewModel request)
        {
            PostApiRetorno<Tipo_Elemento_RedeListaModel> retorno = new PostApiRetorno<Tipo_Elemento_RedeListaModel>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {

                StringBuilder sqlUp = new StringBuilder(" UPDATE TB_TIPO_ELEMENTO_REDE ");
                StringBuilder sql = new StringBuilder();
                using (var session = HibernateUtil.GetSessionT5())
                {
                    string _ids = $"({String.Join(",", request.ids)})";
                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlUp.ToString());
                        sql.AppendLine($"  SET SITUACAO = @STATUS WHERE ID IN {_ids}  ");


                        parametros.Add("@STATUS", request.status);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                        retorno.Ok = true;
                    }

                }
                //retorno.Objeto.Id = Id;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.InnerException.Message;
                retorno.Ok = false;
            }
            return retorno;
        }
        #endregion


        [HttpGet()]
        [Route("api/Tipo_Elemento_RedeApi/GetTreeView")]
        #region GetTreeView
        public async Task<GetApiRetorno<List<Tipo_Elemento_RedeTreeViewModel>>> GetTreeView(String ContratoProjeto)
        {
            GetApiRetorno<List<Tipo_Elemento_RedeTreeViewModel>> retorno = new GetApiRetorno<List<Tipo_Elemento_RedeTreeViewModel>>
            {
                Objeto = new List<Tipo_Elemento_RedeTreeViewModel>()
            };

            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            List<String> condicoes = new List<String>();
            try
            {
                StringBuilder sqlFrom = new StringBuilder();
                StringBuilder sqlWhere = new StringBuilder();

                if (!String.IsNullOrEmpty(ContratoProjeto))
                {
                    parametros.Add("@ID", ContratoProjeto);
                    condicoes.Add("AND B.CONTRATO = @ID");
                }


                sqlFrom.AppendLine("FROM TB_TIPO_ELEMENTO_REDE A")
                       .AppendLine("LEFT JOIN TB_ELEMENTO_REDE B ON A.ID = B.ID_TIPO");

                using (var session = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT DISTINCT A.ID,A.DESCRICAO,B.CONTRATO");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("WHERE A.SITUACAO = 1");

                        if (!String.IsNullOrEmpty(ContratoProjeto))
                        {
                            sqlWhere.AppendLine($"{String.Join("AND", condicoes)}");
                            sql.AppendLine(sqlWhere.ToString());
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Tipo_Elemento_RedeTreeViewModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Contrato = Convert.ToString(reader["CONTRATO"])
                                };

                                retorno.Objeto.Add(obj);
                            }
                        }
                    }
                }
                retorno.Ok = true;

            }
            catch (Exception erro)
            {
                retorno.Mensagem = erro.Message;
                retorno.InnerException = erro.Message != null ? erro.InnerException.Message : null;
            }
            return retorno;
        }
        #endregion


    } 

    public interface ITipo_Elemento_RedeApi
    {
        #region LocalInterface 
        /// <summary>
        /// Lista todos os Tipos de Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DataTableResponse<Tipo_Elemento_RedeListaModel>> GetAll(Tipo_Elemento_RedeRequest request);
        /// <summary>
        /// GetCombo dos Tipos de Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request);
        /// <summary>
        /// GetCombo dos Tipos de Elementos de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ObjComboOptGroup<String, String>> GetComboOptGroup(RequestObjCombo<String> request);
        /// <summary>
        /// Verifica se o material está vinculado ao Tipo de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<TipoElementoMaterial>> MaterialVinculado(String CodMat);
        /// <summary>                                                                                                               
        /// Grava um novo Tipo de Elemento de Rede                                                                                                               
        /// </summary>                                                                                                               
        /// <param name="request"></param>                                                                                                               
        /// <returns></returns>
        public Task<PostApiRetorno<Int64>> Gravar(ParamWebApi<Tipo_Elemento_RedeCadastroModel> request);
        public Task<GetApiRetorno<Tipo_Elemento_RedeCadastroModel>> FindById(Int64? Id);
        /// <summary>
        /// Deleta registros por Id do Tipo de Elemento de Rede
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<DeleteApiRetorno<Int64>> Excluir(ParamWebApi<Int64> value);
        /// <summary>
        /// Alterar o Status de um Elemento de Rede
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public Task<PostApiRetorno<Tipo_Elemento_RedeListaModel>> AlterarStatus(AlterarStatusViewModel request);
        /// <summary>
        /// listagem dos Tipos de Elemento de Rede em formato de Árvore
        /// </summary>
        /// <param name="ContratoProjeto"></param>
        /// <returns></returns>
        public Task<GetApiRetorno<List<Tipo_Elemento_RedeTreeViewModel>>> GetTreeView(String ContratoProjeto);
        #endregion
    }

}