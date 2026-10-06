using Aniel.Connect.Core;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.ControllersWebApi
{
    public interface IEndereco_Repository
    {
        Task<ObjCombo<Int64>> GetComboUf(RequestObjComboPai<Int64?, String> request);
        ObjCombo<Int64> GetComboUfEnum(RequestObjComboPai<Int64?, String> request);
        Task<ObjCombo<Int64>> GetComboPais(RequestObjCombo<int> request);
        Task<List<EnderecoComboObj>> GetCidades(String UF);
        Task<List<Core.Models.ComboObj>> GetBairros(Int64 Id_Cidade);
        Task<List<Core.Models.ComboObj>> GetBairrosViaCep(string Bairro);
        Task<List<UFMunicipioObj>> PlanilhaEstadosMunicipios();
        Task<ObjCombo<Int32>> GetComboBairros(RequestObjComboPai<Int32?, String> request);
        Task<ObjCombo<String>> GetComboCidadesDescricao(RequestObjComboPai<Int64?, String> request);
        Task<ObjCombo<Int64>> GetComboCidades(RequestObjComboPai<Int64?, String> request);

    }

    public class Endereco_Repository : IEndereco_Repository
    {
        public async Task<ObjCombo<Int64>> GetComboUf(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            //var obj = new List<IObjComboResult<Int64>>();

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    System.Text.StringBuilder sqlFrom = new System.Text.StringBuilder(" FROM TB_UF A LEFT JOIN TB_PAIS B ON A.ID_PAIS = B.ID ");
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
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
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

        public ObjCombo<Int64> GetComboUfEnum(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };
            var obj = new List<IObjComboResult<Int64>>();

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {

                    Array Uf_Values = Enum.GetValues(typeof(Model.enumerado.UF));
                    string[] Uf_Names = Enum.GetNames(typeof(Model.enumerado.UF));
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

        public async Task<ObjCombo<Int64>> GetComboPais(RequestObjCombo<int> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder();
                        sql.AppendLine("SELECT A.ID, A.NOME FROM TB_PAIS A");
                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
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
        //[Obsolete]
        public async Task<List<EnderecoComboObj>> GetCidades(String UF)
        {
            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
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

                        System.Text.StringBuilder sql = new System.Text.StringBuilder();
                        //Where A.CODIGO_IBGE IS NOT NULL
                        sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.DDD");
                        sql.AppendLine("FROM TB_CIDADE A");
                        sql.AppendLine("LEFT JOIN TB_UF B ON B.ID =A.ID_UF ");
                        sql.AppendLine("WHERE B.SIGLA = @UF ");



                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(
                                    new EnderecoComboObj
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Descricao = Convert.ToString(reader["DESCRICAO"]),
                                        DDD = Convert.ToString(reader["DDD"])
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
        public async Task<List<Core.Models.ComboObj>> GetBairros(Int64 Id_Cidade)
        {
            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                List<Core.Models.ComboObj> retorno = new List<Core.Models.ComboObj>();

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

                        System.Text.StringBuilder sql = new System.Text.StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_BAIRRO A ");
                        sql.AppendLine("WHERE A.ID_CIDADE = @ID_CIDADE ");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Core.Models.ComboObj
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
        public async Task<List<Core.Models.ComboObj>> GetBairrosViaCep(string Bairro)
        {
            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                List<Core.Models.ComboObj> retorno = new List<Core.Models.ComboObj>();

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

                        System.Text.StringBuilder sql = new System.Text.StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_BAIRRO A ");
                        sql.AppendLine("WHERE A.DESCRICAO = UPPER(@DESCRICAO)");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Core.Models.ComboObj
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
        public async Task<List<UFMunicipioObj>> PlanilhaEstadosMunicipios()
        {
            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                List<UFMunicipioObj> retorno = new List<UFMunicipioObj>();

                try
                {
                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        System.Text.StringBuilder sql = new System.Text.StringBuilder();

                        sql.AppendLine("SELECT UF, DESCRICAO");
                        sql.AppendLine("FROM TB_CIDADE ");


                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, session.Connection, null))
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
        public async Task<ObjCombo<Int32>> GetComboBairros(RequestObjComboPai<Int32?, String> request)
        {
            ObjCombo<Int32> retorno = new ObjCombo<Int32>
            {
                results = new List<IObjComboResult<Int32>>()
            };

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    System.Text.StringBuilder sqlFrom = new System.Text.StringBuilder(" FROM TB_BAIRRO A ");
                    List<String> condicoes = new List<String>();



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
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
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
        public async Task<ObjCombo<String>> GetComboCidadesDescricao(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    System.Text.StringBuilder sqlFrom = new System.Text.StringBuilder(" FROM TB_CIDADE A ");
                    List<String> condicoes = new List<String>();



                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"B.SIGLA  = '{request.idpai.ToUpperInvariant()}'");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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
        public async Task<ObjCombo<Int64>> GetComboCidades(RequestObjComboPai<Int64?, String> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (NHibernate.ISession session = Sinapse.DAO.repository.HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    System.Text.StringBuilder sqlFrom = new System.Text.StringBuilder(" FROM TB_CIDADE A LEFT JOIN TB_UF B ON A.ID_UF = B.ID ");
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
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{Sinapse.Util.StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        System.Text.StringBuilder sql = new System.Text.StringBuilder("SELECT A.ID, A.DESCRICAO,A.DDD ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await Sinapse.DAO.repository.CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
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
    }
}