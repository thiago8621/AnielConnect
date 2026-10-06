using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.T5.Localidade;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class EnderecoApiController : BaseApiController
    {

        [AcceptVerbs("Get", "Post")]
        [Route("api/enderecoapi/getcidades")]
        public async Task<List<EnderecoComboObj>> GetCidades(String UF)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                var retorno = new List<EnderecoComboObj>();

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@UF", UF }
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        //Where A.CODIGO_IBGE IS NOT NULL
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.DDD, B.ID ID_UF");
                        sql.AppendLine("FROM TB_CIDADE A");
                        sql.AppendLine("LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");
                        sql.AppendLine("WHERE B.SIGLA = @UF ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(
                                    new EnderecoComboObj
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Descricao = Convert.ToString(reader["DESCRICAO"]),
                                        DDD = Convert.ToString(reader["DDD"]),
                                        ID_UF = Convert.IsDBNull(reader["ID_UF"]) ? String.Empty : Convert.ToString(reader["ID_UF"])
                                    });

                            }
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [HttpPost()]
        [Route("api/enderecoapi/GetComboPais")]
        public async Task<ObjCombo<Int64>> GetComboPais(RequestObjCombo<string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME FROM TB_PAIS A");

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            sql.AppendLine($"WHERE A.ID = '{request.id}'");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
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
        [Route("api/enderecoapi/GetComboPaisAtreladoVenda")]
        public async Task<ObjCombo<Int64>> GetComboPaisAtreladoVenda(RequestObjCombo<string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime dataHoje = DateTime.Now;
                        DateTime dataPeriodoMaximo = DateTime.Now.AddDays(-90);

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT C.ID, C.NOME  FROM TB_ENDERECO_CLIENTE A  ");
                        sql.AppendLine(" LEFT JOIN TB_VENDA B ON (B.ID_ENDERECO_INSTALACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CONDOMINIO_PREDIO D ON (D.ID = A.ID_CONDOMINIO_PREDIO) ");
                        sql.AppendLine($" LEFT JOIN TB_ENDERECO_LOGRADOURO E ON (E.ID = A.ID_ENDERECO_LOGRADOURO)");
                        sql.AppendLine($" LEFT JOIN TB_PAIS C ON (C.ID = A.ID_PAIS OR C.ID = D.ID_PAIS OR C.ID = E.ID_PAIS) ");
                        sql.AppendLine($" WHERE A.ID IS NOT NULL AND C.ID IS NOT NULL AND C.NOME IS NOT NULL ");
                        sql.AppendLine($" AND B.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAFIM ");
    
                        parametros.Add("@DATAINICIO", dataPeriodoMaximo.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataHoje.ToString("yyyy-MM-dd 23:59:59"));
                        
                        if (!String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($" AND C.NOME LIKE '%{request.q}%'");
                        }

                        sql.AppendLine(" GROUP BY C.ID, C.NOME");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
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
        [Route("api/enderecoapi/GetComboUFAtreladoVenda")]
        public async Task<ObjCombo<Int64>> GetComboUFAtreladoVenda(RequestObjComboPai<string, string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime dataHoje = DateTime.Now;
                        DateTime dataPeriodoMaximo = DateTime.Now.AddDays(-90);

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT F.ID, F.DESCRICAO FROM TB_ENDERECO_CLIENTE A ");
                        sql.AppendLine(" LEFT JOIN TB_VENDA B ON(B.ID_ENDERECO_INSTALACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CONDOMINIO_PREDIO C ON(C.ID = A.ID_CONDOMINIO_PREDIO) ");
                        sql.AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO D ON(D.ID = A.ID_ENDERECO_LOGRADOURO) ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE E ON(E.ID = A.ID_CIDADE OR E.ID = C.ID_CIDADE OR E.ID = D.ID_CIDADE) ");
                        sql.AppendLine(" LEFT JOIN TB_UF F ON(F.ID = E.ID_UF) ");
                        sql.AppendLine(" WHERE A.ENDERECO_CEP IS NOT NULL AND F.ID IS NOT NULL AND F.DESCRICAO IS NOT NULL ");
                        sql.AppendLine($" AND B.DATAHORACRIACAO BETWEEN  @DATAINICIO AND @DATAFIM ");

                        parametros.Add("@DATAINICIO", dataPeriodoMaximo.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataHoje.ToString("yyyy-MM-dd 23:59:59"));
                        
                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            sql.AppendLine($" AND F.ID_PAIS = @IDPAIS ");
                            parametros.Add("@IDPAIS", request.idpai);
                        }

                        if (!String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($" AND F.DESCRICAO LIKE '%{request.q}%'");
                        }

                        sql.AppendLine(" GROUP BY F.ID, F.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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
        [Route("api/enderecoapi/GetComboCidadeAtreladoVenda")]
        public async Task<ObjCombo<Int64>> GetComboCidadeAtreladoVenda(RequestObjComboPai<string, string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime dataHoje = DateTime.Now;
                        DateTime dataPeriodoMaximo = DateTime.Now.AddDays(-90);

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT E.ID, E.DESCRICAO FROM TB_ENDERECO_CLIENTE A ");
                        sql.AppendLine(" LEFT JOIN TB_VENDA B ON(B.ID_ENDERECO_INSTALACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CONDOMINIO_PREDIO C ON(C.ID = A.ID_CONDOMINIO_PREDIO) ");
                        sql.AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO D ON(D.ID = A.ID_ENDERECO_LOGRADOURO) ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE E ON(E.ID = A.ID_CIDADE OR E.ID = C.ID_CIDADE OR E.ID = D.ID_CIDADE) ");
                        sql.AppendLine(" WHERE A.ENDERECO_CEP IS NOT NULL AND E.ID IS NOT NULL AND E.DESCRICAO IS NOT NULL ");
                        sql.AppendLine($" AND B.DATAHORACRIACAO BETWEEN  @DATAINICIO AND @DATAFIM ");
                     
                        parametros.Add("@DATAINICIO", dataPeriodoMaximo.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataHoje.ToString("yyyy-MM-dd 23:59:59"));
                        
                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            sql.AppendLine($" AND E.ID_UF = @IDUF ");
                            parametros.Add("@IDUF", request.idpai);
                        }

                        if (!String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($" AND E.DESCRICAO LIKE '%{request.q}%'");
                        }

                        sql.AppendLine(" GROUP BY E.ID, E.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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
        [Route("api/enderecoapi/GetComboBairroAtreladoVenda")]
        public async Task<ObjCombo<Int64>> GetComboBairroAtreladoVenda(RequestObjComboPai<string, string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime dataHoje = DateTime.Now;
                        DateTime dataPeriodoMaximo = DateTime.Now.AddDays(-90);

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT F.ID, F.DESCRICAO FROM TB_ENDERECO_CLIENTE A ");
                        sql.AppendLine(" LEFT JOIN TB_VENDA B ON(B.ID_ENDERECO_INSTALACAO = A.ID) ");
                        sql.AppendLine(" LEFT JOIN TB_CONDOMINIO_PREDIO C ON(C.ID = A.ID_CONDOMINIO_PREDIO) ");
                        sql.AppendLine(" LEFT JOIN TB_ENDERECO_LOGRADOURO D ON(D.ID = A.ID_ENDERECO_LOGRADOURO) ");
                        sql.AppendLine(" LEFT JOIN TB_CIDADE E ON(E.ID = A.ID_CIDADE OR E.ID = C.ID_CIDADE OR E.ID = D.ID_CIDADE) ");
                        sql.AppendLine(" LEFT JOIN TB_BAIRRO F ON (F.ID_CIDADE = E.ID) ");
                        sql.AppendLine(" WHERE A.ENDERECO_CEP IS NOT NULL AND F.ID IS NOT NULL AND F.DESCRICAO IS NOT NULL ");
                        sql.AppendLine($" AND B.DATAHORACRIACAO BETWEEN  @DATAINICIO AND @DATAFIM ");
                     
                        parametros.Add("@DATAINICIO", dataPeriodoMaximo.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataHoje.ToString("yyyy-MM-dd 23:59:59"));
                        
                        if (!String.IsNullOrEmpty(request.idpai))
                        {
                            sql.AppendLine($" AND F.ID_CIDADE = @IDCIDADE ");
                            parametros.Add("@IDCIDADE", request.idpai);
                        }

                        if (!String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($" AND F.DESCRICAO LIKE '%{request.q}%'");
                        }

                        sql.AppendLine(" GROUP BY F.ID, F.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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
        [Route("api/enderecoapi/GetComboCampanhaAtreladaVenda")]
        public async Task<ObjCombo<Int64>> GetComboCampanhaAtreladaVenda(RequestObjComboPai<string, string> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        DateTime dataHoje = DateTime.Now;
                        DateTime dataPeriodoMaximo = DateTime.Now.AddDays(-90);

                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID, A.TITULO FROM TB_CAMPANHA A ");
                        sql.AppendLine(" LEFT JOIN TB_VENDA B ON (B.ID_CAMPANHA = A.ID) ");
                        sql.AppendLine($" WHERE B.DATAHORACRIACAO BETWEEN  @DATAINICIO AND @DATAFIM ");

                        parametros.Add("@DATAINICIO", dataPeriodoMaximo.ToString("yyyy-MM-dd 00:00:00"));
                        parametros.Add("@DATAFIM", dataHoje.ToString("yyyy-MM-dd 23:59:59"));
                        
                        if (!String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($" AND A.TITULO LIKE '%{request.q}%'");
                        }

                        sql.AppendLine($" GROUP BY A.ID, A.TITULO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["TITULO"]),
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
        [Route("api/enderecoapi/GetComboPaisDescricao")]
        public async Task<ObjCombo<string>> GetComboPaisDescricao(RequestObjCombo<string> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME FROM TB_PAIS A");

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            sql.AppendLine($"WHERE UPPER(A.NOME) = '{request.id.ToUpperInvariant()}'");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
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
        [Route("api/enderecoapi/GetComboPaisDescricaoId")]
        public async Task<ObjCombo<string>> GetComboPaisDescricaoId(RequestObjCombo<string> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME FROM TB_PAIS A");

                        if (!String.IsNullOrEmpty(request.id))
                        {
                            sql.AppendLine($"WHERE UPPER(A.NOME) = '{request.id.ToUpperInvariant()}'");
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.results.Add(new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"]),
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
        [Route("api/enderecoapi/GetComboUfEnum")]
        public ObjCombo<Int64> GetComboUfEnum(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            var obj = new List<IObjComboResult<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {

                    Array Uf_Values = Enum.GetValues(typeof(UF));
                    string[] Uf_Names = Enum.GetNames(typeof(UF));
                    for (var Uf_Id = 0; Uf_Id < Uf_Values.Length; Uf_Id++)
                    {

                        obj.Add(new ObjComboResult<long>
                        {
                            Id = Uf_Id,
                            Text = Uf_Names[Uf_Id],
                        });
                    }
                    if (request.id != null) { obj = obj.Where(p => p.Id == request.id).ToList(); }
                    if (!String.IsNullOrEmpty(request.q))
                    {
                        obj = obj.Where(p => p.Text.ToUpper().Contains(request.q.ToUpper())).ToList();
                    }
                    retorno.results = obj;
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/enderecoapi/GetComboUf")]
        public async Task<ObjCombo<Int64>> GetComboUf(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            //var obj = new List<IObjComboResult<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_UF A LEFT JOIN TB_PAIS B ON A.ID_PAIS = B.ID ");
                    List<String> condicoes = new List<String>();



                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.ID = '{request.idpai.ToUpperInvariant()}'");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.SIGLA ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboUf
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    Sigla = Convert.ToString(reader["SIGLA"])
                                    //DDD = Convert.ToString(reader["DDD"])

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/enderecoapi/GetComboUfSigla")]
        public async Task<ObjCombo<Int64>> GetComboUfSigla(RequestObjCombo<String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            //var obj = new List<IObjComboResult<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_UF A ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add(" A.SIGLA = @SIGLA ");
                        parametros.Add("@SIGLA", request.id);
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.SIGLA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.SIGLA ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.ID)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboUf
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["SIGLA"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/enderecoapi/GetComboUfDesc")]
        public async Task<ObjCombo<string>> GetComboUfDesc(RequestObjCombo<String> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };
            //var obj = new List<IObjComboResult<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_UF A ");
                    List<String> condicoes = new List<String>();


                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.SIGLA = '{request.id}' ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.SIGLA ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["SIGLA"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
                                    //DDD = Convert.ToString(reader["DDD"])

                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string Message = ex.Message;
                }
            }
            return retorno;

        }


        [HttpPost()]
        [Route("api/enderecoapi/GetComboCidades")]
        public async Task<ObjCombo<Int64>> GetComboCidades(RequestObjComboPai<Object, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CIDADE A LEFT JOIN TB_UF B ON A.ID_UF = B.ID ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        if (int.TryParse(request.idpai, out int value))
                        {
                            condicoes.Add($"B.ID = '{Convert.ToInt64(request.idpai)}'");
                        }
                        else
                        {
                            condicoes.Add($" B.SIGLA LIKE '%{request.idpai.ToUpperInvariant()}%' ");
                        }
                    }

                    if (request.id != null)
                    {
                        if (int.TryParse((string)request.id, out int value))
                        {
                            condicoes.Add($" A.ID = {request.id} ");
                        }
                        else
                        {
                            condicoes.Add($" UPPER(A.DESCRICAO) = '{Convert.ToString(request.id).ToUpperInvariant()}' ");
                        }
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO,A.DDD ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new EnderecoComboModels
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    DDD = Convert.ToString(reader["DDD"])

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

        [HttpPost()]
        [Route("api/enderecoapi/GetComboCidadesDescricao")]
        public async Task<ObjCombo<String>> GetComboCidadesDescricao(RequestObjComboPai<String, String> request, String UF)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@UF", UF }
                    };
                    //Filtros
                    // Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CIDADE A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.SIGLA  = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(UF))
                    {
                        condicoes.Add($"B.SIGLA  = '{UF.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.DESCRICAO = '{request.id}'");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(" FROM TB_CIDADE A ");
                        sql.AppendLine(" LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");

                        if (condicoes.Count() > 0)
                        {
                            sql.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                        }


                        sql.AppendLine(" ORDER BY UPPER(A.DESCRICAO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["DESCRICAO"]),
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

        [HttpPost()]
        [Route("api/enderecoapi/GetComboCidadesDesc")]
        public async Task<ObjCombo<String>> GetComboCidadesDesc(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    //Filtros
                    // Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CIDADE A LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.SIGLA  = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.DESCRICAO) = '{request.id.ToUpperInvariant()}'");
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
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["DESCRICAO"]),
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

        [HttpPost()]
        [Route("api/enderecoapi/GetComboEndCidadeDesc")]
        public async Task<ObjCombo<string>> GetComboEndCidadeDesc(RequestObjComboPai<string, string> request)
        {
            var retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    //Filtros
                    // Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CIDADE A LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        if (int.TryParse(request.idpai, out int result))
                        {
                            condicoes.Add($"B.ID = @ID_UF");
                            parametros.Add("@ID_UF", request.idpai);
                        }
                        else
                        {
                            condicoes.Add($"B.SIGLA = @SIGLA");
                            parametros.Add("@SIGLA", request.idpai.ToUpperInvariant());
                        }
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($" ( CAST(A.DESCRICAO AS VARCHAR(60) CHARACTER SET WIN1252) COLLATE WIN_PTBR = @CIDADE) ");
                        parametros.Add("@CIDADE", StringService.Truncate(request.id.ToUpperInvariant(), 60));
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($" ( CAST(A.DESCRICAO AS VARCHAR(60) CHARACTER SET WIN1252) COLLATE WIN_PTBR LIKE @CIDADE) ");
                        parametros.Add("@CIDADE", $"%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%");


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
                                var obj = new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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


        [HttpPost()]
        [Route("api/enderecoapi/GetComboCidadesDescId")]
        public async Task<ObjCombo<String>> GetComboCidadesDescId(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    //Filtros
                    // Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CIDADE A LEFT JOIN TB_UF B ON B.ID = A.ID_UF ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.SIGLA  = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.DESCRICAO) = '{request.id.ToUpperInvariant()}'");
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
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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


        [HttpPost()]
        [Route("api/enderecoapi/GetComboBairros")]
        public async Task<ObjCombo<Int32>> GetComboBairros(RequestObjComboAvo<Int32?, String, String> request)
        {
            ObjCombo<Int32> retorno = new ObjCombo<Int32>
            {
                results = new List<IObjComboResult<Int32>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_BAIRRO A ");
                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.idavo))
                    {
                        condicoes.Add($" UPPER(A.DESCRICAO)  = '{request.idavo.ToUpperInvariant()}'");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"A.ID_CIDADE  = '{request.idpai.ToUpperInvariant()}'");
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
                                var obj = new ObjComboResult<Int32>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),

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

        [HttpPost()]
        [Route("api/enderecoapi/GetComboBairrosDescricao")]
        public async Task<ObjCombo<string>> GetComboBairrosDescricao(RequestObjComboPai<String, String> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_BAIRRO A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_CIDADE B ON (A.ID_CIDADE = B.ID) ");
                    List<String> condicoes = new List<String>();



                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"(UPPER(B.DESCRICAO)  = '{request.idpai.ToUpperInvariant()}')");
                    }

                    if (String.IsNullOrEmpty(request.id) == false)
                    {
                        condicoes.Add($"(UPPER(A.DESCRICAO) = '{request.id.ToUpperInvariant()}')");
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
                                var obj = new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["DESCRICAO"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),

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

        #region [GetPaisViaCep]
        [HttpGet()]
        [Route("api/enderecoapi/GetPaisViaCep")]
        public async Task<List<ComboObj>> GetPaisViaCep(int idPais)
        {
            List<ComboObj> retorno = new List<ComboObj>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPais = new Conexao<Pais>(session))
                {
                    var obj = repoPais.select().Result.ToList();
                    for (int i = 0; i < obj.Count; i++)
                    {
                        retorno.Add(new ComboObj
                        {
                            Id = obj[i].Id,
                            Descricao = obj[i].Nome
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = null;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return await Task.FromResult(retorno);
        }
        #endregion

        #region [GetUFViaCep]
        public async Task<List<ComboObj>> GetUFViaCep()
        {
            List<ComboObj> retorno = new List<ComboObj>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPais = new Conexao<Uf>(session))
                {
                    var obj = repoPais.select().Result.ToList();
                    for (int i = 0; i < obj.Count; i++)
                    {
                        retorno.Add(new ComboObj
                        {
                            Id = obj[i].Id,
                            Descricao = obj[i].Sigla
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = null;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return await Task.FromResult(retorno);
        }
        #endregion

        [HttpGet()]
        [Route("api/enderecoapi/PlanilhaEstadosMunicipios")]
        public async Task<List<UFMunicipioObj>> PlanilhaEstadosMunicipios()
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                List<UFMunicipioObj> retorno = new List<UFMunicipioObj>();

                try
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT UF, DESCRICAO");
                        sql.AppendLine("FROM TB_CIDADE ");


                        using (var reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(
                                    new UFMunicipioObj
                                    {
                                        UF = Convert.ToString(reader["UF"]),
                                        Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    });

                            }
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [HttpGet()]
        [Route("api/enderecoapi/GetEnderecoCliente")]
        public async Task<EnderecoCliente_Models> GetEnderecoCliente(Int64 Id)
        {
            var retorno = new EnderecoCliente_Models();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        Dictionary<String, Object> parametros = new Dictionary<string, object>()
                        {
                            { "@ID", Id }
                        };

                        sql.AppendLine("SELECT COALESCE(A.ENDERECO_LOGRADOURO, '') ENDERECO_LOGRADOURO, COALESCE(A.ENDERECO_NUMERO, '') ENDERECO_NUMERO, COALESCE(A.ENDERECO_UF, '') ENDERECO_UF, COALESCE(A.ENDERECO_CIDADE, '') ENDERECO_CIDADE, COALESCE(A.ID_CIDADE, 0) ID_CIDADE,");
                        sql.AppendLine("COALESCE(A.ENDERECO_BAIRRO, '') ENDERECO_BAIRRO, COALESCE(A.ENDERECO_COMPLEMENTO, '') ENDERECO_COMPLEMENTO, COALESCE(A.ENDERECO_PONTO_REFERENCIA, '') ENDERECO_PONTO_REFERENCIA,");
                        sql.AppendLine("COALESCE(A.ENDERECO_LATITUDE, '') ENDERECO_LATITUDE, COALESCE(A.ENDERECO_LONGITUDE, '') ENDERECO_LONGITUDE, COALESCE(A.ENDERECO_CEP, '') ENDERECO_CEP, COALESCE(A.APARTAMENTO, '') APARTAMENTO,");
                        sql.AppendLine("COALESCE(A.BLOCO, '') BLOCO, COALESCE(A.COD_EESCRITORIO, '') COD_EESCRITORIO, COALESCE(A.COD_AREA, '') COD_AREA, COALESCE(A.COD_LOCALIDADE, 0) COD_LOCALIDADE, COALESCE(A.LOCALIDADE, '') LOCALIDADE,");
                        sql.AppendLine("(SELECT FIRST 1 B.NOME FROM TB_PAIS B WHERE B.ID = A.ID_PAIS) NOME_PAIS, (SELECT FIRST 1 C.DESCRICAO FROM TB_TIPO_IMOVEL C WHERE C.ID = A.ID_TIPO_IMOVEL) DESCRICAO_TIPO_IMOVEL, A.ID_TIPO_IMOVEL");
                        sql.AppendLine("FROM TB_ENDERECO_CLIENTE A");
                        sql.AppendLine("WHERE ID = @ID");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno.Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]);
                                retorno.Numero = Convert.ToString(reader["ENDERECO_NUMERO"]);
                                retorno.Uf = Convert.ToString(reader["ENDERECO_UF"]);
                                retorno.Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]);
                                retorno.IdCidade = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : (Int64?)null;
                                retorno.Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                                retorno.Pais = Convert.ToString(reader["NOME_PAIS"]);
                                retorno.Complemento = Convert.ToString(reader["ENDERECO_COMPLEMENTO"]);
                                retorno.Ponto_Referencia = Convert.ToString(reader["ENDERECO_PONTO_REFERENCIA"]);
                                retorno.Latitude = Convert.ToString(reader["ENDERECO_LATITUDE"]);
                                retorno.Longitude = Convert.ToString(reader["ENDERECO_LONGITUDE"]);
                                retorno.Cep = Convert.ToString(reader["ENDERECO_CEP"]);
                                retorno.Apartamento = Convert.ToString(reader["APARTAMENTO"]);
                                retorno.Bloco = Convert.ToString(reader["BLOCO"]);
                                retorno.Cod_Escritorio = Convert.ToString(reader["COD_EESCRITORIO"]);
                                retorno.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                                retorno.Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt64(reader["COD_LOCALIDADE"]) : (Int64?)null;
                                retorno.Localidade = Convert.ToString(reader["LOCALIDADE"]);
                                retorno.Tipo_Imovel = Convert.ToString(reader["DESCRICAO_TIPO_IMOVEL"]);
                                retorno.Cod_Tipo_Imovel = Convert.ToString(reader["ID_TIPO_IMOVEL"]);
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(retorno.Cod_Escritorio) && !String.IsNullOrEmpty(retorno.Cod_Area) && retorno?.Cod_Localidade > 0)
                    {
                        ILocalidadeAnielRepository _repo = new LocalidadeAnielRepository(sessionT5);
                        var localidadeAniel = _repo.GetByCodigo(retorno.Cod_Escritorio, retorno.Cod_Area, Convert.ToInt32(retorno.Cod_Localidade));

                        retorno.Cod_CT = localidadeAniel.CodCt;
                        retorno.Projeto = localidadeAniel.Projeto;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/enderecoapi/GetBairrosViaCep")]
        public async Task<List<ComboObj>> GetBairrosViaCep(string Bairro)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                List<ComboObj> retorno = new List<ComboObj>();

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@DESCRICAO", Bairro}
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_BAIRRO A ");
                        sql.AppendLine("WHERE A.DESCRICAO = UPPER(@DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ComboObj
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"])
                                });
                            }
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        [HttpGet()]
        [Route("api/enderecoapi/GetBairros")]
        public async Task<List<ComboObj>> GetBairros(Int64 Id_Cidade)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                List<ComboObj> retorno = new List<ComboObj>();

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@ID_CIDADE", Id_Cidade}
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_BAIRRO A ");
                        sql.AppendLine("WHERE A.ID_CIDADE = @ID_CIDADE ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ComboObj
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"])
                                });
                            }
                        }
                    }
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        [HttpGet()]
        [Route("api/enderecoapi/GetLatLong")]
        public object GetLatLong(string logradouro, string numero, string pais, string uf, string cidade, string bairro)
        {
            object retorno = null;
            try
            {
                string chave = CONSTANTES.TokenGoogleAPI;
                string url = "https://maps.googleapis.com/maps/api/geocode/json?";

                List<string> eList = new List<string>();
                if (!string.IsNullOrEmpty(numero))
                {
                    eList.Add(numero.Trim());
                }

                if (!string.IsNullOrEmpty(logradouro))
                {
                    eList.Add(logradouro.Trim().Replace(" ", "+"));
                }

                if (!string.IsNullOrEmpty(bairro))
                {
                    eList.Add(bairro.Trim().Replace(" ", "+"));
                }

                if (!string.IsNullOrEmpty(cidade))
                {
                    eList.Add(cidade.Trim().Replace(" ", "+"));
                }

                if (!string.IsNullOrEmpty(uf))
                {
                    eList.Add(uf.Trim().Replace(" ", "+"));
                }

                if (!string.IsNullOrEmpty(pais))
                {
                    eList.Add(pais.Trim().Replace(" ", "+"));
                }

                string endereco = string.Join(",+", eList.ToArray());

                string chamada = string.Format("{0}key={1}&address={2}", url, chave, endereco);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(chamada);
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = "application/json";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        object obj = JsonConvert.DeserializeObject(reader.ReadToEnd());
                        retorno = new
                        {
                            lat = ((dynamic)obj).results[0].geometry.location.lat,
                            lng = ((dynamic)obj).results[0].geometry.location.lng
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = ex.Message;
            }

            return retorno;
        }

        [HttpGet]
        [Route("api/EnderecoApi/GetEnderecoByCep")]
        public async Task<GetApiRetorno<EnderecoViaCep>> GetEnderecoByCep(String cep)
        {
            GetApiRetorno<EnderecoViaCep> retorno = new GetApiRetorno<EnderecoViaCep>();
            retorno.Objeto = new EnderecoViaCep();

            try
            {
                string url = $"https://viacep.com.br/ws/{cep}/json/";

                WebRequest request = HttpWebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = "application/json";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        object obj = JsonConvert.DeserializeObject(await reader.ReadToEndAsync());

                        string cidadeStr = ((dynamic)obj).localidade;
                        string bairroStr = ((dynamic)obj).bairro;

                        long idCidade = (await GetCidadeByDesc(cidadeStr)).Objeto;

                        retorno.Objeto = new EnderecoViaCep
                        {
                            Cidade = cidadeStr,
                            Bairro = bairroStr,
                            Id_Cidade = idCidade,
                            Id_Bairro = (await GetBairroByDesc(idCidade, bairroStr)).Objeto,
                            Uf = ((dynamic)obj).uf,
                            Endereco = ((dynamic)obj).logradouro
                        };

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/enderecoapi/GetCidadeByDesc")]
        public async Task<GetApiRetorno<Int64>> GetCidadeByDesc(String nomeCidade)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();
                retorno.Objeto = 0;

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>
                    {
                        { "@CIDADE", nomeCidade }
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT FIRST 1 ID FROM TB_CIDADE WHERE UPPER(DESCRICAO) = UPPER(@CIDADE)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.Objeto = Convert.ToInt64(reader["ID"]);

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

                return retorno;
            }
        }

        [HttpGet()]
        [Route("api/enderecoapi/GetBairroByDesc")]
        public async Task<GetApiRetorno<Int64>> GetBairroByDesc(Int64? idCidade, String nomeBairro)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();
                retorno.Objeto = 0;

                try
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {


                        //Filtros
                        Dictionary<String, Object> parametros = new Dictionary<string, object>
                        {
                            { "@CIDADE", Convert.ToInt64(idCidade) },
                            { "@NOME_BAIRRO", Convert.ToString(nomeBairro).ToUpper() },
                        };

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT FIRST 1 ID FROM TB_BAIRRO ");
                        sql.AppendLine(" WHERE UPPER(DESCRICAO) = @NOME_BAIRRO AND ID_CIDADE = @CIDADE ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.Objeto = Convert.ToInt64(reader["ID"]);

                            retorno.Ok = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/enderecoapi/GetEnderecoByRede")]
        public async Task<PostApiRetorno<EnderecoByRedeModel>> GetEnderecoByRede(GetEnderecoByRedeRequestModel request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<POP>(session))
            using (var repositoryPais = new Conexao<Pais>(session))
            using (var repositoryCidade = new Conexao<Cidade>(session))
            using (var repoElementoRede = new Conexao<Elemento_Rede>(sessionT5))
            using (var repoGrupoArea = new Conexao<EscritorioAniel>(sessionT5))
            using (var repoArea = new Conexao<AreaAniel>(sessionT5))
            using (var repoLocalidade = new Conexao<LocalidadeAniel>(sessionT5))
            {
                PostApiRetorno<EnderecoByRedeModel> retorno = new PostApiRetorno<EnderecoByRedeModel>();

                try
                {
                    if (request.IdElementoRede.HasValue)
                    {
                        var elementoRede = await repoElementoRede.selectFirst(p => p.Id == request.IdElementoRede);
                        retorno.IdObjeto = new EnderecoByRedeModel
                        {
                            Bairro = elementoRede.Bairro,
                            Cep = elementoRede.CEP,
                            Cidade = elementoRede.Cidade,
                            Uf = elementoRede.UF,
                            Numero = elementoRede.Numero,
                            Logradouro = elementoRede.Endereco,
                            Latitude = Convert.ToString(elementoRede.Latitude),
                            Longitude = Convert.ToString(elementoRede.Longitude),
                            Complemento = elementoRede.Complemento,
                            Pais = elementoRede.Pais,
                        };

                    }
                    else if (request.IdPop.HasValue)
                    {
                        POP pop = await repository.selectFirst(p => p.Id == request.IdPop);
                        Cidade cidade = await repositoryCidade.selectFirst(p => p.Id == pop.IdCidade);
                        retorno.IdObjeto = new EnderecoByRedeModel
                        {
                            Bairro = pop.Bairro,
                            Cep = pop.Cep,
                            Cidade = cidade?.Descricao,
                            IdCidade = pop.IdCidade,
                            Uf = pop.UF,
                            Logradouro = pop.Endereco,
                            Latitude = Convert.ToString(pop.Latitude),
                            Longitude = Convert.ToString(pop.Longitude),
                            Complemento = pop.Complemento,
                            Numero = pop.Numero,
                            Pais = Convert.ToString(pop.IdPais),
                        };
                    }

                    if (!String.IsNullOrEmpty(retorno.IdObjeto.Cidade))
                    {
                        var area = repoArea.select(p => p.Descricao.IsInsensitiveLike(retorno.IdObjeto.Cidade) && p.Status == Status_Default.Ativo).Result.ToList();
                        var codAreas = area.Select(x => x.Cod_Area).ToList();

                        if (!String.IsNullOrEmpty(retorno.IdObjeto.Bairro) && area != null)
                        {
                            var localidades = repoLocalidade.select(p => p.LocalidadeStr.IsInsensitiveLike(retorno.IdObjeto.Bairro) && p.Status == Status_Default.Ativo).Result;

                            var localidade = localidades.Where(p => codAreas.Contains(p.Cod_Area)).ToList().FirstOrDefault();

                            retorno.IdObjeto.Cod_Escritorio = localidade?.Cod_Escritorio;
                            retorno.IdObjeto.Cod_Area = localidade?.Cod_Area;
                            retorno.IdObjeto.Cod_Localidade = localidade?.Cod_Localidade;

                        }

                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    retorno.Ok = false;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }

                return retorno;
            }
        }
    }
}
