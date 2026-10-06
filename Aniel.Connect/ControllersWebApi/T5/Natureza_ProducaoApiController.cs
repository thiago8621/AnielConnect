using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
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
    public class Natureza_ProducaoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Natureza_ProducaoApi/GetAll")]
        public async Task<DataTableResponse<Natureza_ProducaoListaModel>> GetAll(Natureza_Producao_DataTableRequest request)
        {
            List<Natureza_ProducaoListaModel> retorno = new List<Natureza_ProducaoListaModel>();
            IOrderedEnumerable<Natureza_ProducaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(EQUIPE) CO FROM TB_NATUREZA_PRODUCAO ");

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

                    sql.AppendLine(" SELECT A.DESCRICAO ");
                    sql.AppendLine(" FROM TB_NATUREZA_PRODUCAO A ");
                   
                    //sql.AppendLine(" WHERE A.STATUS = 1 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Natureza_ProducaoListaModel
                            {
                                Id= Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"])
                                
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
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Natureza_ProducaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }
        [HttpPost()]
        [Route("api/Natureza_ProducaoApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<ObjBusca<Int32>>>> GetList()
        {
            GetApiRetorno<IEnumerable<ObjBusca<Int32>>> retorno = new GetApiRetorno<IEnumerable<ObjBusca<Int32>>> {
                Objeto = new List<ObjBusca<Int32>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    var lista = await new Conexao<Natureza_ProducaoAniel>(session).select();

                    retorno.Objeto = lista.Select(p => new ObjBusca<Int32>
                    {
                        Value = p.Id,
                        Label = p.Descricao
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

        [HttpGet()]
        [Route("api/Natureza_ProducaoApi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine("FROM TB_NATUREZA_PRODUCAO A ");
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
        [Route("api/Natureza_ProducaoApi/GetLogEquipeNatureza")]
        public async Task<DataTableResponse<LogEquipeNaturezaListaModel>> GetLogEquipeNatureza(EquipeNaturezaDataTableRequest request)
        {
            List<LogEquipeNaturezaListaModel> retorno = new List<LogEquipeNaturezaListaModel>();
            retorno = new List<LogEquipeNaturezaListaModel>();

            using (ISession session = HibernateUtil.GetSessionLog())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();


                        sql.AppendLine("SELECT A.DESCRICAO_NATUREZA, A.NOME_EQUIPE,A.USUARIOCRIACAO,A.DATAHORACRIACAO,A.TIPOOPERACAO");
                        sql.AppendLine("FROM TB_LOG_EQUIPENATUREZA A");

                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("A.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }

                        if (!String.IsNullOrEmpty(request.Natureza))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("A.ID_NATUREZA_PRODUCAOANIEL = @NATUREZA");
                            parametros.Add("@NATUREZA", request.Natureza);
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new LogEquipeNaturezaListaModel
                                {
                                    Natureza = Convert.ToString(reader["DESCRICAO_NATUREZA"]),
                                    Equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                    Data_Alteracao = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                    Tipo_Alteracao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                    Usuario_Alteracao = Convert.ToString(reader["USUARIOCRIACAO"]),
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
            return new DataTableResponse<LogEquipeNaturezaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Natureza_ProducaoApi/RelatorioEquipeNatureza")]
        public async Task<DataTableResponse<RelatorioEquipeNaturezaListaModel>> RelatorioEquipeNatureza(EquipeNaturezaDataTableRequest request)
        {
            List<RelatorioEquipeNaturezaListaModel> retorno = new List<RelatorioEquipeNaturezaListaModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();


                        sql.AppendLine("SELECT A.NUM_FRE AS MATRICULA, A.NOME AS NOME_EQUIPE, B.DESCRICAO AS NATUREZA,VINCULO.USUARIOCRIACAO,VINCULO.DATAHORACRIACAO");
                        sql.AppendLine("FROM TB_EQUIPE_NATUREZA VINCULO");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE A");
                        sql.AppendLine("ON (A.EQUIPE = VINCULO.EQUIPE)");
                        sql.AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO B");
                        sql.AppendLine("ON (B.ID = VINCULO.ID_NATUREZA)");

                        if (!String.IsNullOrEmpty(request.Equipe))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("VINCULO.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }

                        if (!String.IsNullOrEmpty(request.Natureza))
                        {
                            sql.AppendLine(sql.ToString().Contains("WHERE") ? "AND" : "WHERE");
                            sql.AppendLine("VINCULO.ID_NATUREZA = @NATUREZA");
                            parametros.Add("@NATUREZA", request.Natureza);
                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new RelatorioEquipeNaturezaListaModel
                                {
                                    Natureza = Convert.ToString(reader["NATUREZA"]),
                                    Equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                    Data_Criacao = Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm"),
                                    Usuario_Criacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    Num_Fre = Convert.ToString(reader["MATRICULA"])
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
            return new DataTableResponse<RelatorioEquipeNaturezaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.ToList().Count,
                data = retorno.ToList(),
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Natureza_ProducaoApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_NATUREZA_PRODUCAO A  ");

                    List<String> condicoes = new List<String>();

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



        //[HttpPost()]
        //[Route("api/Natureza_ProducaoApi/GetList")]
        //public async Task<GetApiRetorno<Habilitar_ServicoCadastroModel>> GetListLogin(String Id_Equipe)
        //{
        //    GetApiRetorno<Habilitar_ServicoCadastroModel> retorno = new GetApiRetorno<Habilitar_ServicoCadastroModel>
        //    {
        //        Objeto = new Habilitar_ServicoCadastroModel()
        //    };

        //    using (ISession session = HibernateUtil.GetSessionT5())
        //    {
        //        try
        //        {
        //            var listaNatureza = await new Conexao<Equipe_NaturezaAniel>(session).queryover().Where(p => p.Equipe.Matricula == Id_Equipe).ListAsync();

        //            retorno.Objeto.Naturezas(p => p.)

        //            retorno.Ok = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            retorno.Mensagem = ex.Message;
        //            if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
        //        }
        //    }
        //    return retorno;

        //}

    }
}