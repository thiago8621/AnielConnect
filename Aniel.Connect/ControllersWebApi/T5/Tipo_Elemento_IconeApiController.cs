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
using System.Web;
using System.Web.Http;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Core.Helpers;
using Sinapse.Model.enumerados;
using Aniel.T5.Model.enumerado;
using System.IO;
using Sinapse.DAO.MongoDB;
using Aniel.T5.Model.nhibernate.mongo;
using Sinapse.DAO.Repository;
using Sinapse.Model;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Tipo_Elemento_IconeApiController : BaseApiController
    {/// <summary>
     /// Listagem dos Ícones dos Tipos de Elementos
     /// </summary>
     /// <param name="request"></param>
     /// <returns></returns>
        [HttpPost]
        [Route("api/Tipo_Elemento_IconeApi/GetAll")]
        #region GetAll
        public async Task<DataTableResponse<Tipo_ElementoIconeListModel>> GetAll(Tipo_ElementoIconeDataTableRequest request)
        {
            List<Tipo_ElementoIconeListModel> retorno = new List<Tipo_ElementoIconeListModel>();
            int RecordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlWhere = new StringBuilder();

                    //Count
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT")

                                .AppendLine(" COUNT (A.ID) CO")
                                .AppendLine(" FROM TB_TIPO_ELEMENTO_ICONE A ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
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
                        StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_ELEMENTO_ICONE");


                        StringBuilder sql = new StringBuilder("SELECT")

                        .AppendLine(" A.ID, A.DESCRICAO, A.CLASSE ")
                        .AppendLine(sqlFrom.ToString());

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(sqlWhere.ToString());
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Tipo_ElementoIconeListModel
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Classe = Convert.ToString(reader["CLASSE"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"])

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
            return new DataTableResponse<Tipo_ElementoIconeListModel>
            {
                data = retorno,
            };
        }
        #endregion

        /// <summary>
        /// GetCombo dos Ícones dos Tipos de Elementos
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Tipo_Elemento_IconeApi/GetCombo")]
        #region GetCombo
        public async Task<ObjCombo<String>> GetCombo(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    List<String> condicoes = new List<String>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_ELEMENTO_ICONE A ");


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
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.PADRAO_CONNECT,A.ARQUIVO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Tipo_ElementoIconeComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Padrao_connect = reader["PADRAO_CONNECT"] != DBNull.Value ? Convert.ToBoolean(reader["PADRAO_CONNECT"]) : false,
                                    Text=Convert.ToString(reader["DESCRICAO"]) + @$"<div id='iElementoIcone'>{Convert.ToString(reader["ARQUIVO"])}</div>"
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
        #endregion

        [Route("api/Tipo_Elemento_IconeApi/ExcluirGrid")]
        [HttpGet()]
        #region ExcluirGrid
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Tipo_ElementoIcone>(session))
                    {

                        Tipo_ElementoIcone modelo = null;
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
        #endregion

        [HttpGet()]
        [Route("api/Tipo_Elemento_IconeApi/Get_Icones")]
        #region Get_Icones
        public async Task<GetApiRetorno<List<Tipo_ElementoIconeViewModel>>> Get_Icones(long? Id = null)
        {
            var retorno = new GetApiRetorno<List<Tipo_ElementoIconeViewModel>>();

            try
            {
                using (ConexaoMongoGeography repositoryMongo = new ConexaoMongoGeography())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Tipo_ElementoIcone> repoIcone = new Conexao<Tipo_ElementoIcone>(sessionT5))
                using (RepositoryMongo<Elemento_RedeMongo> repoElMongo = new RepositoryMongo<Elemento_RedeMongo>(repositoryMongo))
                {

                    Tipo_ElementoIcone icon = await repoIcone.selectFirst(p => p.Id == Id);

                    #region No caso de Fazer uma lista
                    // IList<Tipo_ElementoIcone> listIcon = await repoIcone.select();
                    //retorno.Objeto = icon.Select(p => {
                    //                                  return new Tipo_ElementoIconeViewModel
                    //                                  {
                    //                                      Id = p.Id,
                    //                                      Caminho = p.Caminho,
                    //                                      Descricao = p.Descricao,
                    //                                      Tipo = p.Tipo
                    //                                  };
                    //                                  }).ToList();
                    //foreach (var icone in icon)
                    //{  
                    #endregion

                    String path = icon.Url;
                    
                    if (System.IO.File.Exists(path) == true)
                    {
                        byte[] allBytes = System.IO.File.ReadAllBytes(path);
                        icon.Arquivo = new StreamReader(new MemoryStream(allBytes)).ReadToEnd();
                    }
                    List<Tipo_ElementoIcone> ListIcon = new List<Tipo_ElementoIcone>();
                    ListIcon.Add(icon);
                    retorno.Ok = true; retorno.Objeto = ListIcon.Select(p =>
                    {
                        return new Tipo_ElementoIconeViewModel
                        {
                            Arquivo_Str = p.Arquivo,
                            Caminho = p.Caminho,
                            Descricao = p.Descricao,
                            Id = p.Id,
                            Tipo = p.Tipo,
                        };
                    }).ToList();


                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = Erro.Message;
                retorno.InnerException = Erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion

        [HttpGet()]
        [Route("api/Tipo_Elemento_IconeApi/GetIconeByTipo")]
        #region Get_Icones
        public async Task<GetApiRetorno<Tipo_ElementoIconeViewModel>> GetIconeByTipo(long? IdTipo = null)
        {
            var retorno = new GetApiRetorno<Tipo_ElementoIconeViewModel>();

            try
            {
                using (ConexaoMongoGeography repositoryMongo = new ConexaoMongoGeography())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                using (RepositoryMongo<Elemento_RedeMongo> repoElMongo = new RepositoryMongo<Elemento_RedeMongo>(repositoryMongo))
                {
                    Tipo_Elemento_Rede tipo = await repoTipo.selectFirst(p => p.Id == IdTipo);

                    if (tipo.Icone != null)
                    {
                        String icone_path = String.Empty;
                        if (!String.IsNullOrEmpty(tipo.Icone.Arquivo))
                        {
                            var xmlDoc = new System.Xml.XmlDocument();
                            xmlDoc.LoadXml(tipo.Icone.Arquivo);
                            foreach (System.Xml.XmlNode x in xmlDoc.GetElementsByTagName("path"))
                            {
                                if (!String.IsNullOrEmpty(x.Attributes.GetNamedItem("d").Value))
                                    icone_path = x.Attributes.GetNamedItem("d").Value;

                                break;
                            }
                        }

                        retorno.Ok = true;
                        retorno.Objeto = new Tipo_ElementoIconeViewModel
                        {
                            Arquivo_Str = icone_path,
                            Caminho = tipo.Icone.Caminho,
                            Descricao = tipo.Icone.Descricao,
                            Id = tipo.Icone.Id,
                            Tipo = tipo.Icone.Tipo,
                            Cor = !String.IsNullOrEmpty(tipo.Cor) ? tipo.Cor : "f000000"
                        };
                    }
                    else
                    {
                        throw new Exception("É necessário cadastrar um ícone para este Tipo de Elemento!");
                    }


                }   
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = Erro.Message;
                retorno.InnerException = Erro.InnerException.Message;
            }
            return retorno;
        }
        #endregion
    
    }
}
